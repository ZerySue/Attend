/*************************************************************************
** 文件名:   VmDeviceItemMng.cs
** 主要类:   VmDeviceItemMng
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2013-6-14
** 修改人:   szr
** 日  期:  2014-11-11
** 描  述:   VmDeviceItemMng,设备管理
**
** 版  本:   1.0.0
** 备  注:  命名及代码编写遵守C#编码规范
**
*************************************************************************/
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Practices.Prism.Commands;
using System.Collections.Generic;
using System.ServiceModel.DomainServices.Client;
using Irisking.Web.DataModel;
using EDatabaseError;
using IriskingAttend.Web;
using IriskingAttend.Common;
using System.IO.IsolatedStorage;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using IriskingAttend.View.SystemView;
using IriskingAttend.Dialog;
using IriskingAttend.View.PeopleView;
using System.Linq;
using IriskingAttend.ViewModel;
using System.ComponentModel;

namespace IriskingAttend.YangMei
{
    public class VmDeviceItemMng : BaseViewModel
    {
        #region 私有变量：域服务声明

        /// <summary>
        /// 域服务声明
        /// </summary>
        private DomainServiceIriskingAttend _domSrvDbAccess = new DomainServiceIriskingAttend();

        /// <summary>
        /// 设备类型和启用时间表
        /// </summary>
        private ListStackPanel _listStackPanel;

        #endregion

        #region 委托定义及变量声明 

        /// <summary>
        /// 委托定义：vm层程序执行完后需要回调view层，进行界面刷新，决定是否要关闭view
        /// </summary>
        /// <param name="dialogResult"></param>
        public delegate void ChangeDialogResult(bool dialogResult);

        public bool _isContinueAddExcuted = false;//记录是否继续添加状态

        /// <summary>
        /// 委托变量声明
        /// </summary>
        public ChangeDialogResult ChangeDlgResult;

        public DeviceInfo[] BatchModifyDevInfos;

        #endregion       

        #region 与页面绑定的命令

        /// <summary>
        /// 增加一台新设备命令
        /// </summary>
        public DelegateCommand AddDeviceCommand
        {
            get;
            set;
        }

        /// <summary>
        /// 批量增加新设备命令
        /// </summary>
        public DelegateCommand BatchAddDeviceCommand
        {
            get;
            set;
        }

        /// <summary>
        /// 修改设备命令
        /// </summary>
        public DelegateCommand ModifyDeviceCommand
        {
            get;
            set;
        }

        /// <summary>
        /// 取消按钮命令
        /// </summary>
        public DelegateCommand AddCancelCammand
        {
            get;
            set;
        }

        /// <summary>
        /// 批量修改设备命令
        /// </summary>
        public DelegateCommand BatchModifyDeviceCommand
        {
            get;
            set;
        }
        #endregion
       
        #region   与页面绑定的变量属性

        /// <summary>
        /// 界面上进行操作的设备信息
        /// </summary>
        private DeviceInfo _devInfo;
        public DeviceInfo DevInfo
        {
            get
            {
                return _devInfo;
            }
            set
            {
                _devInfo = value;
                OnPropertyChanged<DeviceInfo>(() => this.DevInfo);
            }
        }

        /// <summary>
        /// 设备类型绑定：设备类型数字指示与设备类型字符之间的对应关系
        /// </summary>
        private Dictionary<int, string> _dictDeviceType;
        public Dictionary<int, string> DictDeviceType
        {
            get
            {
                return _dictDeviceType;
            }
            set
            {
                _dictDeviceType = value;
                OnPropertyChanged<Dictionary<int, string>>(() => this.DictDeviceType);
            }
        }

       //  public List<string> DevFuntion = new List<string>();

        #endregion   
        

        #region 构造函数，初始化

        /// <summary>
        /// 构造函数
        /// </summary>        
        public VmDeviceItemMng()
        {
            //与页面绑定的命令初始化：添加设备、批量添加、修改设备
            AddDeviceCommand = new DelegateCommand(new Action(AddDevice));
            BatchAddDeviceCommand = new DelegateCommand(new Action(BatchAddDevice));
            ModifyDeviceCommand = new DelegateCommand(new Action(ModifyDevice));
            BatchModifyDeviceCommand = new DelegateCommand(new Action(BatchModifyDevice));
            AddCancelCammand = new DelegateCommand(new Action(CancelBtnClicked));
            //与页面绑定的变量初始化

           

            DevInfo = new DeviceInfo();           
            DictDeviceType = PublicMethods.GetDevTypeDictionary(VmLogin.GetIsMineApplication());
           
          
        }

        /// <summary>
        /// 初始化设备类型列表
        /// </summary>
        /// <param name="listStackPanel"></param>
        public void InitDevTypes(ListStackPanel listStackPanel)
        {
            _listStackPanel = listStackPanel;
        }

        #endregion

        #region  控件绑定函数操作

        /// <summary>
        /// 添加设备
        /// </summary>
        public void AddDevice()
        {
            List<DeviceInfo> devInfos = new List<DeviceInfo>();
            if (IsDevicInfoValid(ref devInfos))
            {
                CheckAndAddDeviceRia(devInfos, false);  
            }
        }

        /// <summary>
        /// 修改设备信息
        /// </summary>
        public void ModifyDevice()
        {
             List<DeviceInfo> devInfos = new List<DeviceInfo>();
            if (IsDevicInfoValid(ref devInfos))
            {
                ModifyDeviceRia(devInfos);
            }
        }

       
        /// <summary>
        /// 修改设备信息
        /// </summary>
        public void BatchModifyDevice()
        {           
            if ( BatchModifyDevInfos.Count() > 0 )
            {
                BatchModifyDeviceRia();
            }
        }

        /// <summary>
        /// 批量添加设备
        /// </summary>
        public void BatchAddDevice()
        {
            List<DeviceInfo> devInfos = new List<DeviceInfo>();
            if (IsDevicInfoValid(ref devInfos))
            {
                CheckAndAddDeviceRia(devInfos, true);   
            }
        }

        #region 辅助函数

        /// <summary>
        /// 检查进行设备添加、修改时设备信息是否合法
        /// </summary>
        /// <param name="devInfo">如果合法，则将写入数据库的设备信息返回</param>
        /// <returns>true：合法  false：不合法</returns>
        private bool IsDevicInfoValid(ref  List<DeviceInfo> devInfos )
        {
            //设备序列号为空的话，设备信息不合法，返回false并进行提示
            if ( DevInfo.dev_sn.Equals(""))
            {
                MsgBoxWindow.MsgBox( "设备序列号不能为空！", MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);
                return false;
            }
           
            devInfos = this._listStackPanel.GetContent(DevInfo.dev_sn, DevInfo.place, DevInfo.dev_function);
            
            ////将欲增加或修改的设备信息添加到设备信息类变量
            //devInfo.dev_sn = DevInfo.dev_sn;
            //devInfo.dev_type = 0;
            //devInfo.place = DevInfo.place;
            //devInfo.start_time = "00:00:00";
            return true;
        }

        #endregion

        #endregion

        #region  通过ria连接后台，数据库相关操作

        /// <summary>
        /// 添加设备时，先检查数据库中有没有相同设备编号的设备，然后再添加。设备编号不能重复
        /// </summary>
        /// <param name="devInfo">先查看然后进行添加的设备信息</param>
        /// <param name="batch">是否为批量添加设备</param>
        private void CheckAndAddDeviceRia(List<DeviceInfo> devInfos, bool batch)
        {
            try
            {
                Action<InvokeOperation<bool>> onInvokeErrCallBack = CallBackHandleControl<bool>.OnInvokeErrorCallBack;
                CallBackHandleControl<bool>.m_sendValue = (o) =>
                {
                    WaitingDialog.HideWaiting();

                    //异步获取数据                    
                    if (o)
                    {                        
                        MsgBoxWindow.MsgBox( "此设备已存在，请重新输入！", MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);
                    }
                    else
                    {
                        //添加的设备信息在数据库中并不存在，调用后台进行添加设备操作
                        AddDeviceRia(devInfos, batch);
                    }
                   
                };
                WaitingDialog.ShowWaiting();

                //检查此设备号在数据库中是否存在
                _domSrvDbAccess.IsDeviceExist(devInfos.First().dev_sn, onInvokeErrCallBack, null);
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }

        /// <summary>
        /// ria方式调用后台添加设备
        /// </summary>        
        /// <param name="devInfo">进行添加的设备信息</param>
        /// <param name="batch">是否为批量添加设备</param>
        private void AddDeviceRia(List<DeviceInfo> devInfos, bool batch)
        {
            string devSn = devInfos.First().dev_sn;
            string place = devInfos.First().place;

            string devFunction = devInfos.First().dev_function;

            List<string> startTimes = new List<string>();
            List<int> devTypes = new List<int>();
            foreach (var item in devInfos)
            {
                startTimes.Add(item.start_time);
                devTypes.Add(item.dev_type);
            }
            try
            {
                Action<InvokeOperation<byte>> onInvokeErrCallBack = CallBackHandleControl<byte>.OnInvokeErrorCallBack;
                CallBackHandleControl<byte>.m_sendValue = (o) =>
                {
                    WaitingDialog.HideWaiting();                    
                                      
                    //异步获取数据                    
                    if (0X00 != o)
                    {
                        if (0XFF == o)
                        {
                            MsgBoxWindow.MsgBox("添加设备成功，但通知后台失败，请检查后台！", MsgBoxWindow.MsgIcon.Warning, MsgBoxWindow.MsgBtns.OK);
                        }
                        else
                        {
                            MsgBoxWindow.MsgBox( "添加设备成功！", MsgBoxWindow.MsgIcon.Succeed, MsgBoxWindow.MsgBtns.OK);                         
                        }
                        //若为批量添加设备，清空设备信息，继续进行添加
                        if (batch)
                        {
                            DevInfo.dev_sn = "";
                            DevInfo.place = "";
                            DevInfo.start_time = "";
                            _isContinueAddExcuted = true;
                        }
                        //否则调用回调函数，关闭添加设备页面
                        else
                        {
                            if (null != ChangeDlgResult)
                            {                                
                                ChangeDlgResult(true);
                            }
                        }                        
                    }
                    else
                    {
                        MsgBoxWindow.MsgBox( "添加设备失败！", MsgBoxWindow.MsgIcon.Error, MsgBoxWindow.MsgBtns.OK);
                    }                    
                   
                };
                WaitingDialog.ShowWaiting();

                //调用后台进行添加设备
                _domSrvDbAccess.AddDeviceYangMei(devSn,place,startTimes.ToArray(),devTypes.ToArray(), devFunction,onInvokeErrCallBack, null);  
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }

        private void CancelBtnClicked()
        {
            if (ChangeDlgResult != null)
            {
                ChangeDlgResult(_isContinueAddExcuted);
            }
        }
        /// <summary>
        /// ria方式调用后台修改设备
        /// </summary>
        /// <param name="devInfo">修改后的设备信息</param>
        private void ModifyDeviceRia(List<DeviceInfo> devInfos) 
        {
            string devSn = devInfos.First().dev_sn;
            string place = devInfos.First().place;
            string devfunction = devInfos.First().dev_function;
            List<string> startTimes = new List<string>();
            List<int> devTypes = new List<int>();
            foreach (var item in devInfos)
            {
                startTimes.Add(item.start_time);
                devTypes.Add(item.dev_type);
            }

            try
            {
                Action<InvokeOperation<byte>> onInvokeErrCallBack = CallBackHandleControl<byte>.OnInvokeErrorCallBack;
                CallBackHandleControl<byte>.m_sendValue = (o) =>
                {
                    WaitingDialog.HideWaiting();                    

                    //异步获取数据                    
                    if (0X00 != o)
                    {
                        if (0XFF == o)
                        {
                            MsgBoxWindow.MsgBox("修改设备成功，但通知后台失败，请检查后台！", MsgBoxWindow.MsgIcon.Warning, MsgBoxWindow.MsgBtns.OK);
                        }
                        else
                        {
                            MsgBoxWindow.MsgBox("修改设备成功！", MsgBoxWindow.MsgIcon.Succeed, MsgBoxWindow.MsgBtns.OK);
                        }
                        //修改设备成功后，调用回调函数，关闭修改设备页面
                        if (null != ChangeDlgResult)
                        {
                            ChangeDlgResult(true);
                        }                                         
                    }
                    else
                    {
                        MsgBoxWindow.MsgBox( "修改设备失败！", MsgBoxWindow.MsgIcon.Error, MsgBoxWindow.MsgBtns.OK);
                    }
                   
                };
                WaitingDialog.ShowWaiting();
                //调用后台修改设备信息
                _domSrvDbAccess.ModifyDeviceYangMei(devSn, place, startTimes.ToArray(), devTypes.ToArray(),devfunction, onInvokeErrCallBack, null);
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }

        private void BatchModifyDeviceRia()
        {
            List<DeviceInfo> devInfos = this._listStackPanel.GetContent("", "","井口设备");
            List<string> startTimes = new List<string>();
            List<int> devTypes = new List<int>();
            foreach (var item in devInfos)
            {
                startTimes.Add(item.start_time);
                devTypes.Add(item.dev_type);
            }

            try
            {
                Action<InvokeOperation<byte>> onInvokeErrCallBack = CallBackHandleControl<byte>.OnInvokeErrorCallBack;
                CallBackHandleControl<byte>.m_sendValue = (o) =>
                {
                    WaitingDialog.HideWaiting();

                    //异步获取数据                    
                    if (0X00 != o)
                    {
                        if (0XFF == o)
                        {
                            MsgBoxWindow.MsgBox("批量修改设备成功，但通知后台失败，请检查后台！", MsgBoxWindow.MsgIcon.Warning, MsgBoxWindow.MsgBtns.OK);
                        }
                        else
                        {
                            MsgBoxWindow.MsgBox("批量修改设备成功！", MsgBoxWindow.MsgIcon.Succeed, MsgBoxWindow.MsgBtns.OK);
                        }
                        //修改设备成功后，调用回调函数，关闭修改设备页面
                        if (null != ChangeDlgResult)
                        {
                            ChangeDlgResult(true);
                        }
                    }
                    else
                    {
                        MsgBoxWindow.MsgBox("批量修改设备失败！", MsgBoxWindow.MsgIcon.Error, MsgBoxWindow.MsgBtns.OK);
                    }

                };
                WaitingDialog.ShowWaiting();
                //调用后台修改设备信息
                _domSrvDbAccess.BatchModifyDevice(BatchModifyDevInfos, startTimes.ToArray(), devTypes.ToArray(), onInvokeErrCallBack, null);
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        } 
 
        #endregion


       
    }

}
