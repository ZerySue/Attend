/*************************************************************************
** 文件名:   VmFestivalItemMng.cs
** 主要类:   VmFestivalItemMng
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2013-12-12
** 修改人:   
** 日  期:
** 描  述:   VmFestivalItemMng,节假日条目管理
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
using IriskingAttend.View.AttendView;
using IriskingAttend.Dialog;
using System.ComponentModel;

namespace IriskingAttend.ViewModel.AttendViewModel
{
    #region 数据绑定类

    /// <summary>
    /// 数据绑定的实体类
    /// </summary>
    public class ShiftHoliday : BaseViewModel
    {
        private DateTime _shiftDate;
        /// <summary>
        /// 调休日期
        /// </summary>
        public DateTime ShiftDate
        {
            get
            {
                return _shiftDate;
            }
            set
            {
                _shiftDate = value;
                OnPropertyChanged<DateTime>(() => this.ShiftDate);
            }
        }

       
        /// <summary>
        /// 构造函数
        /// </summary>
        public ShiftHoliday()
        {
            
        }

    }
    #endregion
    public class VmFestivalItemMng : BaseViewModel
    {
        #region 私有变量：域服务声明

        /// <summary>
        /// 域服务声明
        /// </summary>
        private DomainServiceIriskingAttend _domSrvDbAccess = new DomainServiceIriskingAttend();

        #endregion

        #region 委托定义及变量声明 

        /// <summary>
        /// 委托定义：vm层程序执行完后需要回调view层，进行界面刷新，决定是否要关闭view
        /// </summary>
        /// <param name="dialogResult"></param>
        public delegate void ChangeDialogResult(bool dialogResult);

        /// <summary>
        /// 委托变量声明
        /// </summary>
        public ChangeDialogResult ChangeDlgResult;

        #endregion       

        #region 与页面绑定的命令

        /// <summary>
        /// 增加一台新节假日命令
        /// </summary>
        public DelegateCommand AddFestivalCommand
        {
            get;
            set;
        }

        /// <summary>
        /// 修改节假日命令
        /// </summary>
        public DelegateCommand ModifyFestivalCommand
        {
            get;
            set;
        }
        #endregion
       
        #region   与页面绑定的变量属性

        /// <summary>
        /// 界面上进行操作的节假日信息
        /// </summary>
        private FestivalInfo _fesvalInfo;
        public FestivalInfo FesvalInfo
        {
            get
            {
                return _fesvalInfo;
            }
            set
            {
                _fesvalInfo = value;
                OnPropertyChanged<FestivalInfo>(() => this.FesvalInfo);
            }
        }

        /// <summary>
        /// 当前选中的节假日类型绑定
        /// </summary>
        private BaseViewModelCollection<ShiftHoliday> _shiftDateList;
        public BaseViewModelCollection<ShiftHoliday> ShiftDateList
        {
            get
            {
                return _shiftDateList;
            }
            set
            {
                _shiftDateList = value;
                OnPropertyChanged<BaseViewModelCollection<ShiftHoliday>>(() => this.ShiftDateList);
            }
        }

        /// <summary>
        /// 选中节假日信息
        /// </summary>
        private ShiftHoliday _selectShiftHolidayItem;
        public ShiftHoliday SelectShiftHolidayItem
        {
            get
            {
                return _selectShiftHolidayItem;
            }
            set
            {
                _selectShiftHolidayItem = value;
                OnPropertyChanged<ShiftHoliday>(() => this.SelectShiftHolidayItem);
            }
        }

        /// <summary>
        /// 节假日名称
        /// </summary>   
        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                OnPropertyChanged<string>(() => this.Name);
            }
        }

        /// <summary>
        /// 节假日起始时间
        /// </summary>  
        private string _beginTime;
        public string BeginTime
        {
            get
            {
                return _beginTime;
            }
            set
            {
                _beginTime = value;
                OnPropertyChanged<string>(() => this.BeginTime);
            }
        }

        /// <summary>
        /// 节假日结束时间
        /// </summary>       
        private string _endTime;
        public string EndTime
        {
            get
            {
                return _endTime;
            }
            set
            {
                _endTime = value;
                OnPropertyChanged<string>(() => this.EndTime);
            }
        }

        /// <summary>
        /// 节假日备注
        /// </summary>    
        private string _memo;
        public string Memo
        {
            get
            {
                return _memo;
            }
            set
            {
                _memo = value;
                OnPropertyChanged<string>(() => this.Memo);
            }
        }

        #endregion   
     
        #region 构造函数，初始化

        /// <summary>
        /// 构造函数
        /// </summary>        
        public VmFestivalItemMng()
        {
            //与页面绑定的命令初始化：添加节假日、修改节假日
            AddFestivalCommand = new DelegateCommand(new Action(AddFestival));            
            ModifyFestivalCommand = new DelegateCommand(new Action(ModifyFestival));
            
            //与页面绑定的变量初始化
            FesvalInfo = new FestivalInfo();
            ShiftDateList = new BaseViewModelCollection<ShiftHoliday>();          
        }

        #endregion

        #region  控件绑定函数操作

        /// <summary>
        /// 添加节假日
        /// </summary>
        public void AddFestival()
        {
            FestivalInfo tempFestival = GetFestivalInfo();
            if (tempFestival != null)
            {
                CheckAndAddFestivalRia(tempFestival);  
            }
        }

        /// <summary>
        /// 修改节假日信息
        /// </summary>
        public void ModifyFestival()
        {
            FestivalInfo tempFestival = GetFestivalInfo();
            if (tempFestival != null)
            {
                CheckAndModifyFestivalRia(tempFestival);
            }
        }

        #region 辅助函数

        /// <summary>
        /// 检查进行节假日添加、修改时节假日信息是否合法
        /// </summary>
        /// <param name="fesvalInfo">如果合法，则将写入数据库的节假日信息返回</param>
        /// <returns>true：合法  false：不合法</returns>
        private FestivalInfo GetFestivalInfo()
        {
            //节假日名称为空的话，节假日信息不合法，返回false并进行提示
            if (Name == null || Name.Equals(""))
            {
                MsgBoxWindow.MsgBox( "节假日名称不能为空！", MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);
                return null;
            }

            //节假日名称为空的话，节假日信息不合法，返回false并进行提示
            if (BeginTime == null || BeginTime.Equals(""))
            {
                MsgBoxWindow.MsgBox("节假日起始时间不能为空！", MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);
                return null;
            }

            //节假日名称为空的话，节假日信息不合法，返回false并进行提示
            if (EndTime == null || EndTime.Equals(""))
            {
                MsgBoxWindow.MsgBox("节假日终止时间不能为空！", MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);
                return null;
            }

            FestivalInfo tempFestival = new FestivalInfo();
            try
            {
                if (Convert.ToDateTime(EndTime).AddDays(1).ToString("yyyy-MM-dd 00:00:00").CompareTo(Convert.ToDateTime(BeginTime).ToString("yyyy-MM-dd 00:00:00")) < 0)
                {
                    MsgBoxWindow.MsgBox("节假日终止时间不能早于起始时间！", MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);
                    return null;
                }
                
                tempFestival.festival_id = FesvalInfo.festival_id;
                tempFestival.isSelected = FesvalInfo.isSelected;
                tempFestival.memo = Memo;
                tempFestival.name = Name;                
                tempFestival.begin_time = Convert.ToDateTime(BeginTime).ToString("yyyy-MM-dd 00:00:00");
                tempFestival.end_time = Convert.ToDateTime(EndTime).AddDays(1).ToString("yyyy-MM-dd 00:00:00");
                tempFestival.ShiftHolidayList = new List<DateTime>();

                foreach( ShiftHoliday item in ShiftDateList )
                {
                    ((List<DateTime>)tempFestival.ShiftHolidayList).Add(item.ShiftDate);
                }                
            }
            catch
            {
                MsgBoxWindow.MsgBox("节假日起始与终止时间格式不正确！", MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);
                return null;
            }
            return tempFestival;
        }

        #endregion

        #endregion

        #region  通过ria连接后台，数据库相关操作

        /// <summary>
        /// 添加节假日时，先检查数据库中有没有相同节假日名称的节假日，然后再添加。节假日名称不能重复
        /// </summary>
        /// <param name="fesvalInfo">先查看然后进行添加的节假日信息</param>        
        private void CheckAndAddFestivalRia(FestivalInfo fesvalInfo) 
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
                        MsgBoxWindow.MsgBox( "此节假日已存在，请重新输入！", MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);
                    }
                    else
                    {
                        //添加的节假日信息在数据库中并不存在，调用后台进行添加节假日操作
                        AddFestivalRia(fesvalInfo);
                    }
                   
                };
                WaitingDialog.ShowWaiting();

                //检查此节假日在数据库中是否存在
                _domSrvDbAccess.IsFestivalExist(fesvalInfo, onInvokeErrCallBack, null);
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }

        /// <summary>
        /// ria方式调用后台添加节假日
        /// </summary>        
        /// <param name="fesvalInfo">进行添加的节假日信息</param>        
        private void AddFestivalRia(FestivalInfo fesvalInfo)
        {
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
                            MsgBoxWindow.MsgBox("添加节假日成功，但通知后台失败，请检查后台！", MsgBoxWindow.MsgIcon.Warning, MsgBoxWindow.MsgBtns.OK);
                        }
                        else
                        {
                            MsgBoxWindow.MsgBox( "添加节假日成功！", MsgBoxWindow.MsgIcon.Succeed, MsgBoxWindow.MsgBtns.OK);                         
                        }                        
                        if (null != ChangeDlgResult)
                        {                                
                            ChangeDlgResult(true);
                        }                                               
                    }
                    else
                    {
                        MsgBoxWindow.MsgBox( "添加节假日失败！", MsgBoxWindow.MsgIcon.Error, MsgBoxWindow.MsgBtns.OK);
                    }                    
                   
                };
                WaitingDialog.ShowWaiting();

                //调用后台进行添加节假日
                _domSrvDbAccess.AddFestival(fesvalInfo, onInvokeErrCallBack, null);  
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }

        /// <summary>
        /// 修改节假日时，先检查数据库中有没有相同节假日名称的节假日，然后再添加。节假日名称不能重复
        /// </summary>
        /// <param name="fesvalInfo">先查看然后进行添加的节假日信息</param>        
        private void CheckAndModifyFestivalRia(FestivalInfo fesvalInfo) 
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
                        MsgBoxWindow.MsgBox( "此节假日已存在，请重新输入！", MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);
                    }
                    else
                    {
                        //修改的节假日信息在数据库中并不存在，调用后台进行修改节假日操作
                        ModifyFestivalRia(fesvalInfo);
                    }
                   
                };
                WaitingDialog.ShowWaiting();

                //检查此节假日号在数据库中是否存在
                _domSrvDbAccess.IsFestivalExist(fesvalInfo, onInvokeErrCallBack, null);
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }
       
        /// <summary>
        /// ria方式调用后台修改节假日
        /// </summary>
        /// <param name="fesvalInfo">修改后的节假日信息</param>
        private void ModifyFestivalRia(FestivalInfo fesvalInfo) 
        {
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
                            MsgBoxWindow.MsgBox("修改节假日成功，但通知后台失败，请检查后台！", MsgBoxWindow.MsgIcon.Warning, MsgBoxWindow.MsgBtns.OK);
                        }
                        else
                        {
                            MsgBoxWindow.MsgBox("修改节假日成功！", MsgBoxWindow.MsgIcon.Succeed, MsgBoxWindow.MsgBtns.OK);
                        }

                        //修改节假日成功后，调用回调函数，关闭修改节假日页面
                        if (null != ChangeDlgResult)
                        {
                            ChangeDlgResult(true);
                        }                                         
                    }
                    else
                    {
                        MsgBoxWindow.MsgBox( "修改节假日失败！", MsgBoxWindow.MsgIcon.Error, MsgBoxWindow.MsgBtns.OK);
                    }
                   
                };
                WaitingDialog.ShowWaiting();
                //调用后台修改节假日信息
                _domSrvDbAccess.ModifyFestival(fesvalInfo, onInvokeErrCallBack, null);
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
