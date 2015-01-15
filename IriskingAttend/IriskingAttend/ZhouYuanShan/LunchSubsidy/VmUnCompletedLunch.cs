/*************************************************************************
** 文件名:   VmUnCompletedLunch.cs
×× 主要类:   VmUnCompletedLunch
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   wz
** 日  期:   2013-9-12
** 修改人:   
** 日  期:   
** 描  述:   VmUnCompletedLunch.cs类,未完成班中餐管理viewmodel
**
** 版  本:   1.0.0
** 备  注:  命名及代码编写遵守C#编码规范
**
 * ***********************************************************************/
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Linq;
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
using IriskingAttend.View.PeopleView;
using System.Windows.Media.Imaging;
using System.IO;
using System.Text.RegularExpressions;
using IriskingAttend.Dialog;
using IriskingAttend.BehaviorSelf;
using IriskingAttend.ViewModel;
using IriskingAttend.ViewModel.PeopleViewModel;
using IriskingAttend.View;
using IriskingAttend.ViewMine.PeopleView;
using System.Text;
using IriskingAttend.ViewModel.SystemViewModel;
using IriskingAttend.Web.ZhouYuanShan;

namespace IriskingAttend.ZhouYuanShan.LunchSubsidy
{
    public class VmUnCompletedLunch : BaseViewModel
    {

        #region 字段声明
        
       

        /// <summary>
        /// viewModel加载完毕事件
        /// </summary>
        public event EventHandler LoadCompletedEvent;

        private List<UserDepartInfo> _departs;
        private List<UserPersonInfo> _persons;

        #endregion

        #region 与页面绑定的命令

        public DelegateCommand QueryCmd
        {
            get;
            set;
        }

        #endregion

        #region   与页面绑定的属性


   

        private BaseViewModelCollection<ReportRecordInfoOnDepart_ZhouYuanShan> _reportRecordInfosOnDepart = new BaseViewModelCollection<ReportRecordInfoOnDepart_ZhouYuanShan>();

        /// <summary>
        /// 未完成班中餐列表
        /// </summary>
        public BaseViewModelCollection<ReportRecordInfoOnDepart_ZhouYuanShan> ReportRecordInfosOnDepart
        {
            get { return _reportRecordInfosOnDepart; }
            set
            {
                _reportRecordInfosOnDepart = value;
                this.OnPropertyChanged(() => ReportRecordInfosOnDepart);
            }
        }

        private ReportRecordInfoOnDepart_ZhouYuanShan _selectedRecordInfoOnDepart = null;

        /// <summary>
        /// 当前选择的未完成班中餐
        /// </summary>
        public ReportRecordInfoOnDepart_ZhouYuanShan SelectedRecordInfoOnDepart
        {
            get { return _selectedRecordInfoOnDepart; }
            set
            {
                _selectedRecordInfoOnDepart = value;
                this.OnPropertyChanged(() => SelectedRecordInfoOnDepart);
            }
        }

        private BaseViewModelCollection<ReportRecordInfoOnDepart_ZhouYuanShan> _completedReportInfosOnDepart = new BaseViewModelCollection<ReportRecordInfoOnDepart_ZhouYuanShan>();

        /// <summary>
        /// 已完成班中餐列表
        /// </summary>
        public BaseViewModelCollection<ReportRecordInfoOnDepart_ZhouYuanShan> CompletedReportInfosOnDepart
        {
            get { return _completedReportInfosOnDepart; }
            set
            {
                _completedReportInfosOnDepart = value;
                this.OnPropertyChanged(() => CompletedReportInfosOnDepart);
            }
        }

        private ReportRecordInfoOnDepart_ZhouYuanShan _selectedCompletedRecordInfoOnDepart = null;

        /// <summary>
        /// 当前选择的未上报考勤信息
        /// </summary>
        public ReportRecordInfoOnDepart_ZhouYuanShan SelectedCompletedRecordInfoOnDepart
        {
            get { return _selectedCompletedRecordInfoOnDepart; }
            set
            {
                _selectedCompletedRecordInfoOnDepart = value;
                this.OnPropertyChanged(() => SelectedCompletedRecordInfoOnDepart);
            }
        }

        /// <summary>
        /// 全选按钮的绑定类已完成
        /// </summary>
        public MarkObject MarkObjCompleted
        {
            get;
            set;
        }

          /// <summary>
        /// 全选按钮的绑定类未完成
        /// </summary>
        public MarkObject MarkObjUnCompleted
        { 
            get; 
            set; 
        }

        private DateTime _beginTime;
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime BeginTime
        {
            get
            {
                return _beginTime;
            }
            set
            {
                _beginTime = value;
                OnPropertyChanged(() => BeginTime);
            }
        }

        private DateTime _endTime;
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime
        {
            get
            {
                return _endTime;
            }
            set
            {
                _endTime = value;
                OnPropertyChanged(() => BeginTime);
            }
        }

        /// <summary>
        /// tabControl当前的选择序号
        /// 1 = 已完成班中餐
        /// 0 = 未完成班中餐
        /// </summary>
        public int TabSelectedIndex
        {
            get;
            set;
        }

        #endregion

        #region 构造函数

        public VmUnCompletedLunch()
        {
            IsolatedStorageSettings appSettings = IsolatedStorageSettings.ApplicationSettings;
            if (appSettings.Contains("PageUnCompletedLunch_BeginTime") &&
                appSettings["PageUnCompletedLunch_BeginTime"] != null)
            {
                BeginTime = (DateTime)appSettings["PageUnCompletedLunch_BeginTime"];
            }
            else
            {
                BeginTime = DateTime.Now.Date.AddDays(-1);

            }
            if (appSettings.Contains("PageUnCompletedLunch_EndTime") &&
                appSettings["PageUnCompletedLunch_EndTime"] != null)
            {
                EndTime = (DateTime)appSettings["PageUnCompletedLunch_EndTime"];
            }
            else
            {
                EndTime = DateTime.Now;
            }

            QueryCmd = new DelegateCommand(Query);
            
            WaitingDialog.ShowWaiting();
            GetDepartRia((departs) =>
                {
                    //部门按树形结构排序
                    _departs = new List<UserDepartInfo>();
                    PublicMethods.OrderDepartByTree(departs, _departs, -1,"-");
                    _departs = PublicMethods.FiterDepartById(_departs, VmLogin.OperatorDepartIDList);
                    GetPersonsRia((persons) =>
                    {
                        _persons = persons;
                        WaitingDialog.HideWaiting();
                        if (LoadCompletedEvent != null)
                        {
                            LoadCompletedEvent(this, null);
                            LoadCompletedEvent = null;
                        }
                    });
                });
            
                
            
            
        }
        #endregion

        #region 界面的事件响应

        /// <summary>
        /// 修改当前未完成班中餐信息
        /// </summary>
        public void ModifyUnCompletedReportRecord()
        {
            List<ReportRecordInfoOnDepart_ZhouYuanShan> compareRecordInfo = new List<ReportRecordInfoOnDepart_ZhouYuanShan>();
            foreach( var item in ReportRecordInfosOnDepart)
            {
                if (item.attend_day.CompareTo(SelectedRecordInfoOnDepart.attend_day) == 0 && item.class_order_id == SelectedRecordInfoOnDepart.class_order_id)
                {
                    if (item.depart_id != SelectedRecordInfoOnDepart.depart_id)
                    {
                        compareRecordInfo.Add(item);
                    }
                }
            }
            ChildWndUnCompletedLunch wnd = new ChildWndUnCompletedLunch(compareRecordInfo);
            wnd.InitBaseUI(SelectedRecordInfoOnDepart);
            wnd.InitUnCompletedUI(_departs, _persons);
            wnd.Show();
        }

        
        /// <summary>
        /// 上报当前这条未完成班中餐信息
        /// </summary>
        public void ReportRecord()
        {
            ReportRecordInfoOnDepart_ZhouYuanShan[] infos = new ReportRecordInfoOnDepart_ZhouYuanShan[1];
            infos[0] = SelectedRecordInfoOnDepart;
            ReportRecordRia(infos);
        }

        /// <summary>
        /// 查看当前完成的班中餐信息
        /// </summary>
        public void CheckCompletedReportRecord()
        {
            ChildWndUnCompletedLunch wnd = new ChildWndUnCompletedLunch( null );
            wnd.InitBaseUI(SelectedCompletedRecordInfoOnDepart);
            wnd.InitCompletedUI();
            wnd.Show();
        }

        /// <summary>
        /// 撤消已完成班中餐记录
        /// </summary>
        public void UnDoReportRecord()
        {
            ReportRecordInfoOnDepart_ZhouYuanShan[] infos = new ReportRecordInfoOnDepart_ZhouYuanShan[1];
            infos[0] = SelectedCompletedRecordInfoOnDepart;
            UndoReportRecordRia(infos);
        }


        //查询班中餐列表
        private void Query()
        {
            //查询未完成班中餐
            if (TabSelectedIndex == 0)
            {
                WaitingDialog.ShowWaiting();
                //获取未完成班中餐
                GetUnCompletedInfoOnDepartRia((result) =>
                {
                    ReportRecordInfosOnDepart = new BaseViewModelCollection<ReportRecordInfoOnDepart_ZhouYuanShan>();
                    foreach (var item in result)
                    {
                        this.ReportRecordInfosOnDepart.Add(item);
                    }
                    
                    WaitingDialog.HideWaiting();
                });
            }
            //查询已完成班中餐
            else if (TabSelectedIndex == 1)
            {
                WaitingDialog.ShowWaiting();
                GetCompletedInfoOnDepartRia((result) =>
                    {
                        CompletedReportInfosOnDepart = new BaseViewModelCollection<ReportRecordInfoOnDepart_ZhouYuanShan>();
                        foreach (var item in result)
                        {
                            this.CompletedReportInfosOnDepart.Add(item);
                        }

                        WaitingDialog.HideWaiting();
                    });
            }
        }

        /// <summary>
        /// 选中全部item或者取消选中
        /// </summary>
        /// <param name="sender">checkBox对象</param>
        public void SelectAllUnCompleted(bool? IsChecked)
        {
            if (IsChecked.Value)
            {
                foreach (var item in ReportRecordInfosOnDepart)
                {
                    item.isSelected = true;
                }
            }
            else
            {
                foreach (var item in ReportRecordInfosOnDepart)
                {
                    item.isSelected = false;
                }
            }

            //this.IsBatchBtnEnable = CheckIsAnyOneSelected();
            this.MarkObjUnCompleted.Selected = CheckIsAllSelectedUnCompleted();
        }

        /// <summary>
        /// 选中全部item或者取消选中
        /// </summary>
        /// <param name="sender">checkBox对象</param>
        public void SelectAllCompleted(bool? IsChecked)
        {
            if (IsChecked.Value)
            {
                foreach (var item in CompletedReportInfosOnDepart)
                {
                    item.isSelected = true;
                }
            }
            else
            {
                foreach (var item in CompletedReportInfosOnDepart)
                {
                    item.isSelected = false;
                }
            }

            //this.IsBatchBtnEnable = CheckIsAnyOneSelected();
            this.MarkObjCompleted.Selected = CheckIsAllSelectedCompleted();
        }

        /// <summary>
        /// 按需选择datagrid的Items
        /// </summary>
        /// <param name="sender">datagrid对象</param>
        public void SelectItemsUnCompleted(ReportRecordInfoOnDepart_ZhouYuanShan info)
        {
            info.isSelected = !info.isSelected;
            //this.IsBatchBtnEnable = CheckIsAnyOneSelected();
            this.MarkObjUnCompleted.Selected = CheckIsAllSelectedUnCompleted();
        }

        /// <summary>
        /// 按需选择datagrid的Items
        /// </summary>
        /// <param name="sender">datagrid对象</param>
        public void SelectItemsCompleted(ReportRecordInfoOnDepart_ZhouYuanShan info)
        {
            info.isSelected = !info.isSelected;
            //this.IsBatchBtnEnable = CheckIsAnyOneSelected();
            this.MarkObjCompleted.Selected = CheckIsAllSelectedCompleted();
        }

        #endregion

        #region 私有功能函数
   

        /// <summary>
        /// 检查Item是否全部被选中
        /// </summary>
        private bool CheckIsAllSelectedUnCompleted()
        {
            if (ReportRecordInfosOnDepart.Count == 0)
            {
                return false;
            }
            foreach (var item in ReportRecordInfosOnDepart)
            {
                if (!item.isSelected)
                {
                    return false;
                }
            }
            return true;

        }

        /// <summary>
        /// 检查Item是否全部被选中
        /// </summary>
        private bool CheckIsAllSelectedCompleted()
        {
            if (CompletedReportInfosOnDepart.Count == 0)
            {
                return false;
            }
            foreach (var item in CompletedReportInfosOnDepart)
            {
                if (!item.isSelected)
                {
                    return false;
                }
            }
            return true;

        }
        #endregion

        #region ria连接后台操作
        
        /// <summary>
        /// 向后台发送命令，获取未完成班中餐列表
        /// </summary>
        /// <param name="riaOperateCallBack">异步操作完成后的回调函数</param>
        private void GetUnCompletedInfoOnDepartRia(Action<IEnumerable<ReportRecordInfoOnDepart_ZhouYuanShan>> riaOperateCallBack)
        {
            try
            {
               
                ServiceDomDbAcess.ReOpenSever();
                EntityQuery<ReportRecordInfoOnDepart_ZhouYuanShan> list = ServiceDomDbAcess.GetSever().GetUnCompletedReportRecordInfoOnDepartQuery(BeginTime, EndTime,VmLogin.OperatorDepartIDList.ToArray());
                ///回调异常类
                Action<LoadOperation<ReportRecordInfoOnDepart_ZhouYuanShan>> actionCallBack = ErrorHandle<ReportRecordInfoOnDepart_ZhouYuanShan>.OnLoadErrorCallBack;
                ///异步事件
                LoadOperation<ReportRecordInfoOnDepart_ZhouYuanShan> lo = ServiceDomDbAcess.GetSever().Load(list, actionCallBack, null);

                lo.Completed += delegate
                {
                    riaOperateCallBack(lo.Entities);
                };
            }
            catch (Exception e)
            {
                WaitingDialog.HideWaiting();
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }

        /// <summary>
        /// 向后台发送命令，获取已完成班中餐列表
        /// </summary>
        /// <param name="riaOperateCallBack">异步操作完成后的回调函数</param>
        private void GetCompletedInfoOnDepartRia(Action<IEnumerable<ReportRecordInfoOnDepart_ZhouYuanShan>> riaOperateCallBack)
        {
            try
            {
               
                ServiceDomDbAcess.ReOpenSever();
                EntityQuery<ReportRecordInfoOnDepart_ZhouYuanShan> list = ServiceDomDbAcess.GetSever().GetCompletedReportRecordInfoOnDepartQuery(BeginTime, EndTime,VmLogin.OperatorDepartIDList.ToArray());
                ///回调异常类
                Action<LoadOperation<ReportRecordInfoOnDepart_ZhouYuanShan>> actionCallBack = ErrorHandle<ReportRecordInfoOnDepart_ZhouYuanShan>.OnLoadErrorCallBack;
                ///异步事件
                LoadOperation<ReportRecordInfoOnDepart_ZhouYuanShan> lo = ServiceDomDbAcess.GetSever().Load(list, actionCallBack, null);

                lo.Completed += delegate
                {
                    riaOperateCallBack(lo.Entities);
                };
            }
            catch (Exception e)
            {
                WaitingDialog.HideWaiting();
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }


        /// <summary>
        /// 获取部门信息
        /// </summary>
        private void GetDepartRia(Action<List<UserDepartInfo>> callBack)
        {
            ServiceDomDbAcess.ReOpenSever();

            EntityQuery<UserDepartInfo> list = ServiceDomDbAcess.GetSever().GetDepartsInfoQuery();
            ///回调异常类
            Action<LoadOperation<UserDepartInfo>> actionCallBack = ErrorHandle<UserDepartInfo>.OnLoadErrorCallBack;
            ///异步事件
            LoadOperation<UserDepartInfo> lo = ServiceDomDbAcess.GetSever().Load(list, actionCallBack, null);

            lo.Completed += delegate
            {
                List<UserDepartInfo> departs = new List<UserDepartInfo>();
                //异步获取数据
                foreach (UserDepartInfo ar in lo.Entities)
                {
                    departs.Add(ar);
                }

                if (callBack != null)
                {
                    callBack(departs);
                }
            };
        }

       

        /// <summary>
        /// 获取与部门相关的人员信息
        /// </summary>
        private void GetPersonsRia(Action<List<UserPersonInfo>> callBack)
        {
            ServiceDomDbAcess.ReOpenSever();

            EntityQuery<UserPersonInfo> list = ServiceDomDbAcess.GetSever().GetPersonInfo_ZhouYuanShanQuery(VmLogin.OperatorDepartIDList.ToArray());
            ///回调异常类
            Action<LoadOperation<UserPersonInfo>> actionCallBack = ErrorHandle<UserPersonInfo>.OnLoadErrorCallBack;
            ///异步事件
            LoadOperation<UserPersonInfo> lo = ServiceDomDbAcess.GetSever().Load(list, actionCallBack, null);

            lo.Completed += delegate
            {
                List<UserPersonInfo> persons = new List<UserPersonInfo>();
                //异步获取数据
                foreach (UserPersonInfo ar in lo.Entities)
                {
                    persons.Add(ar);
                }
                if (callBack != null)
                {
                    callBack(persons);
                }
            };
        }

        /// <summary>
        /// 提交未完成的班中餐记录
        /// </summary>
        private void ReportRecordRia(ReportRecordInfoOnDepart_ZhouYuanShan[] reportRecordInfos)
        {
            WaitingDialog.ShowWaiting();
            ServiceDomDbAcess.GetSever().CreateReportRecord(reportRecordInfos, CallBackHandleControl<OptionInfo>.OnInvokeErrorCallBack, null);
            CallBackHandleControl<OptionInfo>.m_sendValue = (option) =>
                {
                    //失败时弹出窗口
                    if (!option.isSuccess)
                    {
                        MsgBoxWindow.MsgBox(
                               option.option_info,
                               Dialog.MsgBoxWindow.MsgIcon.Error,
                               Dialog.MsgBoxWindow.MsgBtns.OK);
                    }
                    else //成功则从未完成班中餐列表中，移除已生成的已完成班中餐记录
                    {
                        foreach (var item in reportRecordInfos)
                        {
                            ReportRecordInfosOnDepart.Remove(item);
                        }
                        OnPropertyChanged(() => ReportRecordInfosOnDepart);
                    }
                    WaitingDialog.HideWaiting();
                    
                };
            
        }

        /// <summary>
        /// 撤消已完成的班中餐记录
        /// </summary>
        private void UndoReportRecordRia(ReportRecordInfoOnDepart_ZhouYuanShan[] reportRecordInfos)
        {
            WaitingDialog.ShowWaiting();
            ServiceDomDbAcess.GetSever().UndoReportRecord(reportRecordInfos, CallBackHandleControl<OptionInfo>.OnInvokeErrorCallBack, null);
            CallBackHandleControl<OptionInfo>.m_sendValue = (option) =>
            {
                //失败时弹出窗口
                if (!option.isSuccess)
                {
                    MsgBoxWindow.MsgBox(
                           option.option_info,
                           Dialog.MsgBoxWindow.MsgIcon.Error,
                           Dialog.MsgBoxWindow.MsgBtns.OK);
                }
                else //成功则从已完成班中餐列表中，移除已撤消的已完成班中餐记录
                {
                    foreach (var item in reportRecordInfos)
                    {
                        CompletedReportInfosOnDepart.Remove(item);
                    }
                    OnPropertyChanged(() => CompletedReportInfosOnDepart);
                }
                WaitingDialog.HideWaiting();

            };

        }

        #endregion

     
    }

 

}
