/*************************************************************************
** 文件名:   VmDeviceManage.cs
** 主要类:   VmDeviceManage
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2013-6-14
** 修改人:   
** 日  期:
** 描  述:   VmDeviceManage,设备管理
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
using System.ComponentModel;
using IriskingAttend.View;
using System.Linq;



namespace IriskingAttend.ViewModel.SystemViewModel
{
    public class VmDeviceManage : BaseViewModel
    {
        #region 私有变量：域服务声明

        /// <summary>
        /// 域服务声明
        /// </summary>
        private DomainServiceIriskingAttend _domSrvDbAccess = new DomainServiceIriskingAttend();

        #endregion

        #region   与页面绑定的属性

        /// <summary>
        /// 全选按钮绑定
        /// </summary>
        public MarkObject MarkObj 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// 设备信息表
        /// </summary>
        private BaseViewModelCollection<DeviceInfo> _systemDeviceInfo;
        public BaseViewModelCollection<DeviceInfo> SystemDeviceInfo
        {
            get
            { 
                return _systemDeviceInfo; 
            }
            set
            {
                _systemDeviceInfo = value;
                OnPropertyChanged<BaseViewModelCollection<DeviceInfo>>(() => this.SystemDeviceInfo);
            }
        }

        /// <summary>
        /// 选中设备信息
        /// </summary>
        private DeviceInfo _selectDeviceInfoItem;
        public DeviceInfo SelectDeviceInfoItem
        {
            get
            {
                return _selectDeviceInfoItem;
            }
            set
            {                
                _selectDeviceInfoItem = value;
                OnPropertyChanged<DeviceInfo>(() => this.SelectDeviceInfoItem);       
            }
        }

        /// <summary>
        /// 批量删除设备按钮的enable属性
        /// </summary>
        private bool _isBatchDeleteEnabled;
        public bool IsBatchDeleteEnabled
        {
            get 
            { 
                return _isBatchDeleteEnabled; 
            }
            set
            {
                _isBatchDeleteEnabled = value;
                OnPropertyChanged<bool>(() => this.IsBatchDeleteEnabled);  
            }
        }

        #endregion

        #region 事件

        /// <summary>
        /// 设备信息加载完成
        /// </summary>
        public event EventHandler DeviceLoadCompleted;

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数,初始化属性
        /// </summary>
        public VmDeviceManage()
        {
            //设备信息表初始化
            SystemDeviceInfo = new BaseViewModelCollection<DeviceInfo>();

            //选中设备信息初始化
            SelectDeviceInfoItem = new DeviceInfo();

            //批量删除按钮处于不可用状态
            IsBatchDeleteEnabled = false;

            //事件初始化
            DeviceLoadCompleted += (a, e) => { };
        }

        #endregion

        #region  控件绑定函数操作
      
        /// <summary>
        /// 批量删除设备
        /// </summary>
        public void BatchDeleteDevice()
        {
            //对话框提示信息
            string strInfo = "请注意，您将进行如下操作：\r\n批量删除设备信息！";
            MsgBoxWindow.MsgBox( strInfo, MsgBoxWindow.MsgIcon.Warning, MsgBoxWindow.MsgBtns.OKCancel, (result) =>　
            {
                //确定删除
                if (result == MsgBoxWindow.MsgResult.OK)
                {
                    //将选中的设备添加到删除列表中
                    List<string> deviceIDs = new List<string>();
                    foreach (DeviceInfo dev in SystemDeviceInfo)
                    {
                        if (dev.isSelected)
                        {
                            deviceIDs.Add(dev.dev_sn);
                        }
                    }

                    //通过ria向后台发送请求
                    DeleteDeviceRia(deviceIDs.ToArray());
                }
            });             
        }

        /// <summary>
        /// 删除设备
        /// </summary>
        public void DeleteDevice()
        {
            string strInfo = string.Format("请注意，您将进行如下操作：\r\n删除设备\"{0}\"的信息！", this.SelectDeviceInfoItem.dev_sn);
            MsgBoxWindow.MsgBox( strInfo, MsgBoxWindow.MsgIcon.Warning, MsgBoxWindow.MsgBtns.OKCancel, (result) =>　
            {
                //确定删除
                if (result == MsgBoxWindow.MsgResult.OK)
                {
                    //将欲删除的设备ID添加到删除列表中
                    List<string> deviceIDs = new List<string>();
                    deviceIDs.Add(this.SelectDeviceInfoItem.dev_sn);

                    //通过ria向后台发送请求
                    DeleteDeviceRia(deviceIDs.ToArray());
                }
            });  
        }

        /// <summary>
        /// 选中全部设备或者取消选中
        /// </summary>
        /// <param name="isChecked">true: 选中， false：未选中</param>
        public void SelectAllDevice(bool isChecked)
        {
            //将全部设备置为全选或全部选状态
            foreach (var item in SystemDeviceInfo)
            {
                item.isSelected = isChecked;
            }

            //更新批量删除按钮的可用状态
            IsBatchDeleteEnabled = CheckIsBatchDeleteEnabled();
        }

        /// <summary>
        /// 确定批量删除按钮的可用状态
        /// </summary>
        /// <returns>true: 可用状态， false：不可用状态</returns>
        private bool CheckIsBatchDeleteEnabled()
        {
            //遍历所有设备信息
            foreach (DeviceInfo dev in SystemDeviceInfo)
            {
                //只要有一台设备处于选中状态，批量删除按钮就可用。
                if (dev.isSelected)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 确定全选按钮的选中状态
        /// </summary>
        /// <returns>选中状态</returns>
        private bool CheckIsAllDevSelected()
        {
            //默认为选中
            bool checkAll = true;

            //遍历所有设备信息
            foreach (DeviceInfo dev in SystemDeviceInfo)
            {
                //只要有一条设备信息未选中，全选按钮就处于未选中状态
                if (!dev.isSelected)
                {
                    checkAll = false;
                    break;
                }
            }
            return checkAll;
        }
        
        /// <summary>
        /// 更改当前选中设备的选中状态
        /// </summary>
        /// <param name="selectDevInfo">选中设备的设备信息源</param>
        public void ChangeDeviceCheckedState(DeviceInfo selectDevInfo )
        {   
            //反选选中设备状态  
            selectDevInfo.isSelected = !selectDevInfo.isSelected;

            UpdateCheckAllState();
        }

        /// <summary>
        /// 更新全选按钮的状态
        /// </summary>
        public void UpdateCheckAllState()
        {
            //更新全选按钮的选中状态
            MarkObj.Selected = CheckIsAllDevSelected();

            //更新批量删除按钮的可用状态
            IsBatchDeleteEnabled = CheckIsBatchDeleteEnabled();
        }
        
        #endregion

        #region  通过ria连接后台，数据库相关操作

        /// <summary>
        /// ria方式通过后台删除设备
        /// </summary>
        /// <param name="deviceIds">欲删除的设备ID字符串数组</param>
        private void DeleteDeviceRia(string[] deviceIds)
        {
            try
            {
                Action<InvokeOperation<byte>> onInvokeErrCallBack = CallBackHandleControl<byte>.OnInvokeErrorCallBack;
                CallBackHandleControl<byte>.m_sendValue = (o) =>
                {
                    //隐藏等待界面
                    WaitingDialog.HideWaiting();                   

                    //异步获取数据                    
                    if (0X00 != o)
                    {
                        if (0XFF == o)
                        {
                            MsgBoxWindow.MsgBox("删除设备操作成功，但通知后台失败，请检查后台！", MsgBoxWindow.MsgIcon.Warning, MsgBoxWindow.MsgBtns.OK);
                        }
                        else
                        {
                            MsgBoxWindow.MsgBox("删除设备操作成功！", MsgBoxWindow.MsgIcon.Succeed, MsgBoxWindow.MsgBtns.OK);                            
                        }
                        //重新查询数据库,刷新设备列表                       
                        GetDeviceInfoTableRia();
                    }
                    else
                    {
                        MsgBoxWindow.MsgBox("删除设备操作失败！", MsgBoxWindow.MsgIcon.Error, MsgBoxWindow.MsgBtns.OK);
                    }
                   
                };

                //显示等待界面
                WaitingDialog.ShowWaiting();

                //通过后台执行删除设备动作
                _domSrvDbAccess.BatchDeleteDevice(deviceIds, onInvokeErrCallBack, null);
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }

        /// <summary>
        /// 异步获取数据库中数据, 获取设备信息表
        /// </summary>
        public void GetDeviceInfoTableRia()
        {
            try
            {
                _domSrvDbAccess = new DomainServiceIriskingAttend();

                //显示等待界面
                WaitingDialog.ShowWaiting();

                //获得设备信息列表
                EntityQuery<DeviceInfo> devList = _domSrvDbAccess.GetAllDeviceQuery();

                ///回调异常类
                Action<LoadOperation<DeviceInfo>> loadDevCallBack = ErrorHandle<DeviceInfo>.OnLoadErrorCallBack;

                ///异步事件
                LoadOperation<DeviceInfo> loadOp = this._domSrvDbAccess.Load(devList, loadDevCallBack, null);
                loadOp.Completed += delegate
                {
                    //若SystemDeviceInfo设备列表信息未分配内存，则进行内存分配
                    if (SystemDeviceInfo == null)
                    {
                        SystemDeviceInfo = new BaseViewModelCollection<DeviceInfo>();
                    }
                    else
                    {
                        //将设备列表信息清空
                        SystemDeviceInfo.Clear();
                    }

                    //异步获取数据，将获取到的设备信息添加到SystemDeviceInfo中去
                    foreach (DeviceInfo dev in loadOp.Entities)
                    {
                        //设备类型默认为未定义
                        dev.dev_type_string = "未定义";

                        if (PublicMethods.GetDevTypeDictionary( true ).ContainsKey(dev.dev_type))
                        {
                            dev.dev_type_string = PublicMethods.GetDevTypeDictionary(true)[dev.dev_type];
                        }
                        
                        //判断如果同一设备设置了多个开始时间和设备类型,则属于同一行数据
                        if (SystemDeviceInfo.Count>0 &&
                            SystemDeviceInfo.FirstOrDefault((info) => info.dev_sn == dev.dev_sn)!=null )
                        {
                            List<string> dev_type_List = SystemDeviceInfo.First((info) => info.dev_sn == dev.dev_sn).dev_type_List as List<string>;
                            string newTypeString = string.Format("{0}  {1}", dev.dev_type_string, dev.start_time);
                            dev_type_List.Add(newTypeString);
                        }
                        else //否则不同的设备对应不同的行
                        {
                            dev.dev_type_List = new List<string>();
                            string newTypeString = string.Format("{0}  {1}", dev.dev_type_string, dev.start_time);
                            ((List<string>)dev.dev_type_List).Add(newTypeString);
                            SystemDeviceInfo.Add(dev);        
                        }
                                       
                    }

                    foreach( DeviceInfo dev in SystemDeviceInfo)
                    {
                        dev.dev_type_strings = "";
                        for( int index = 0; index < ((List<string>)dev.dev_type_List).Count; index++)
                        {
                            if (index != 0)
                            {
                                dev.dev_type_strings += "\r\n";
                            }
                            dev.dev_type_strings += ((List<string>)dev.dev_type_List)[index];
                        }
                    }

                    //设备信息加载完成后，发送事件
                    DeviceLoadCompleted(this, new EventArgs());

                    //隐藏等待界面
                    WaitingDialog.HideWaiting();                    
                };
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
