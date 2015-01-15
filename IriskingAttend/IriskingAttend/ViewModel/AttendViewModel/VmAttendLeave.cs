/*************************************************************************
** 文件名:   VmAttendLeave.cs
** 主要类:   VmAttendLeave
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   lzc
** 日  期:   2013-4-11
** 修改人:   cty
** 日  期:   2013-11-5
 *修改内容： 增加操作员日后
** 描  述:   VmAttendLeave，请假模块VM层
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
using System.Windows.Input;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Practices.Prism.Commands;
using Irisking.Web.DataModel;
using System.ServiceModel.DomainServices.Client;
using EDatabaseError;
using IriskingAttend.Dialog;
using System.Windows.Controls.Primitives;
using IriskingAttend.View;
using GalaSoft.MvvmLight.Command;
using MvvmLightCommand.SL4.TriggerActions;
using System.Collections.Generic;
using IriskingAttend.Common;
using IriskingAttend.ViewModel.SystemViewModel;

namespace IriskingAttend.ViewModel
{
    public class VmAttendLeave
    {
        #region 绑定数据

        /// <summary>
        /// 请假数据Model
        /// </summary>
        public BaseViewModelCollection<UserAttendForLeave> AttendLeaveModel { get; set; }


        /// <summary>
        /// 请假数据Model基础集合
        /// </summary>
        public BaseViewModelCollection<UserAttendForLeave> AttendLeaveModelBase { get; set; }
        /// <summary>
        /// 绑定当前选定的请假信息
        /// </summary>
        public UserAttendForLeave SelectAttendLeave
        {
            get
            {
                return _selectAttendLeave;
            }
            set
            {
                if (value != _selectAttendLeave)
                {
                    _selectAttendLeave = value;
                }
            }
        }

        /// <summary>
        /// 请假查询条件
        /// </summary>
        public QueryConditionBind QueryConditionForLeave { get; set; }

        /// <summary>
        /// 部门列表
        /// </summary>
        public BaseViewModelCollection<depart> DepartModle
        {
            get
            {
                return _vmDepart.DepartmentModel;
            }
        }

        public BaseViewModelCollection<UserPersonSimple> PersonModel
        {
            get
            {
                return _vmDepart.UserPersonSimpleForDepartModel;
            }
        }
        #endregion

        #region 私有变量
        /// <summary>
        /// 部门Vm
        /// </summary>
        private VmDepartment _vmDepart = new VmDepartment();
        //选择请假信息
        private UserAttendForLeave _selectAttendLeave = null;
        //选择请假信息，用来存储修改之前的请假信息，与修改后的请假信息对比，写操作员日志 by cty
        private UserAttendForLeave _oldSelectAttendLeave = new UserAttendForLeave();

        #endregion

        #region  构造
        public VmAttendLeave()
        {
            QueryConditionForLeave = new QueryConditionBind();
            AttendLeaveModel = new BaseViewModelCollection<UserAttendForLeave>();
            AttendLeaveModelBase = new BaseViewModelCollection<UserAttendForLeave>();
            //DepartModle = new BaseViewModelCollection<depart>();
            GetAttendLeave();
            ServiceDomDbAcess.ReOpenSever();
        }
        #endregion


        #region 异步重数据库中获取数据

        /// <summary>
        /// 重数据库中查询请假记录
        /// </summary>
        private void GetAttendLeave()
        {
            //EntityQuery<UserAttendForLeave> lstAttendForLeave = ServiceDomDbAcess.GetSever().IrisGetAttendForLeaveQuery();
            EntityQuery<UserAttendForLeave> lstAttendForLeave = ServiceDomDbAcess.GetSever().IrisGetAttendLeaveForDepartQuery(VmLogin.OperatorDepartIDList);
            ///回调异常类
            Action<LoadOperation<UserAttendForLeave>> getAttendForLeaveCallBack = new Action<LoadOperation<UserAttendForLeave>>
                (ErrorHandle<UserAttendForLeave>.OnLoadErrorCallBack);
            ///异步事件
            LoadOperation<UserAttendForLeave> lo = ServiceDomDbAcess.GetSever().Load(lstAttendForLeave, getAttendForLeaveCallBack, null);
            lo.Completed += (o, e) =>
            {
                AttendLeaveModelBase.Clear();
                AttendLeaveModel.Clear();
                foreach (var ar in lo.Entities)
                {
                    AttendLeaveModel.Add(ar);
                    AttendLeaveModelBase.Add(ar);
                }
                QueryAttendLeave();
                _vmDepart.GetDepartment();
                _vmDepart.DepartmentLoadCompleted += (a, ee) =>
                    {
                        if (_vmDepart.DepartmentModel.Count > 0)
                        {
                            QueryConditionForLeave.Depart = _vmDepart.DepartmentModel[0];
                        }
                    };
            };


        }

        #endregion

        #region DelegateCommand 变量定义

        //部门改变
        private RelayCommand<EventInformation<SelectionChangedEventArgs>> _departChangedCommand = null;

        //工号输入
        private RelayCommand<EventInformation<RoutedEventArgs>> _workSNChangedCommand = null;

        //添加请假信息命令
        private DelegateCommand<AddAttendLeaveDialog> _addAttendLeaveCmd = null;
        //继续添加请假信息命令
        private DelegateCommand<AddAttendLeaveDialog> _contuneAddAttendLeaveCmd = null;
        //更新请假信息命令
        private DelegateCommand<AddAttendLeaveDialog> _updateAttendLeaveCmd = null;

        //鼠标左键事件命令
        private RelayCommand<EventInformation<MouseButtonEventArgs>> _mouseButtonEventArgs = null;
        //鼠标移动事件命令
        private RelayCommand<EventInformation<MouseEventArgs>> _mouseMoveEventArgs = null;
        //界面更新事件命令
        private RelayCommand<EventInformation<EventArgs>> _layoutUpdateEventArgs = null;
        
        //查寻请假信息命令
        private DelegateCommand _queryAttendLeaveCmd = null;
        //删除请假信息命令
        private DelegateCommand _delAttendLeaveCmd = null;
        //显示添加请假信息对话框命令
        private DelegateCommand _showAddDialogCmd = null;
        //显示更新请假信息对话框命令
        private DelegateCommand _showUpdateDialogCmd = null;

        #endregion

        #region  Command 绑定

        /// <summary>
        /// 部门选择改变
        /// </summary>
        public RelayCommand<EventInformation<SelectionChangedEventArgs>> DepartChangedCommand
        {
            get
            {
                if (_departChangedCommand == null)
                {
                    _departChangedCommand = new RelayCommand<EventInformation<SelectionChangedEventArgs>>(DepartChanged);
                }
                return _departChangedCommand;
            }
        }

        /// <summary>
        /// 工号失去焦点
        /// </summary>
        public RelayCommand<EventInformation<RoutedEventArgs>> WorkSNChangedCommand
        {
            get
            {
                if (_workSNChangedCommand == null)
                {
                    _workSNChangedCommand = new RelayCommand<EventInformation<RoutedEventArgs>>(WorkSNChanged);
                }
                return _workSNChangedCommand;
            }
        }
        /// <summary>
        /// 排序事件绑定
        /// </summary>
        public RelayCommand<EventInformation<MouseButtonEventArgs>> SortBehaviorCommand
        {
            get
            {
                if (_mouseButtonEventArgs == null)
                {
                    _mouseButtonEventArgs = new RelayCommand<EventInformation<MouseButtonEventArgs>>(SortAttendLeave);
                }
                return _mouseButtonEventArgs;
            }

        }
        /// <summary>
        /// 通过鼠标移动来显示 DataGrid排序箭头
        /// </summary>
        public RelayCommand<EventInformation<MouseEventArgs>> MouseMoveBehaviorCommand
        {
            get
            {
                //MvvmLightCommand.SL4.TriggerActions
                if (_mouseMoveEventArgs == null)
                {
                    _mouseMoveEventArgs = new RelayCommand<EventInformation<MouseEventArgs>>(MouseMoveAttendLeave);
                }
                return _mouseMoveEventArgs;
            }

        }
        /// <summary>
        /// 通过页面更新来显示 DataGrid排序箭头
        /// </summary>
        public RelayCommand<EventInformation<EventArgs>> LayoutUpdateCommand
        {
            get
            {
                //MvvmLightCommand.SL4.TriggerActions
                if (_layoutUpdateEventArgs == null)
                {
                    _layoutUpdateEventArgs = new RelayCommand<EventInformation<EventArgs>>(LayoutUpadtaAttendLeave);
                }
                return _layoutUpdateEventArgs;
            }

        }

        /// <summary>
        /// 查询请假信息
        /// </summary>
        public ICommand QueryAttendLeaveCommand
        {
            get
            {
                if (_queryAttendLeaveCmd == null)
                {
                    _queryAttendLeaveCmd = new DelegateCommand(QueryAttendLeave);
                }
                return _queryAttendLeaveCmd;
            }
        }


        /// <summary>
        /// 显示更新对话框
        /// </summary>
        public ICommand ShowUpdateDialogCommand
        {
            get
            {
                if (_showUpdateDialogCmd == null)
                {
                    _showUpdateDialogCmd = new DelegateCommand(ShowUpdateDialog);
                }
                return _showUpdateDialogCmd;

            }
        }

        /// <summary>
        /// 显示添加请假信息对话框
        /// </summary>
        public ICommand ShowAddDialogCommand
        {
            get
            {
                if (_showAddDialogCmd == null)
                {
                    _showAddDialogCmd = new DelegateCommand(ShowAddDialog);
                }
                return _showAddDialogCmd;

            }
        }

        /// <summary>
        /// 添加请假信息
        /// </summary>
        public ICommand AddAttendLeaveCommand
        {
            get
            {
                if (_addAttendLeaveCmd == null)
                {
                    _addAttendLeaveCmd = new DelegateCommand<AddAttendLeaveDialog>(AddAttendLeave);
                }
                return _addAttendLeaveCmd;

            }
        }

        /// <summary>
        /// 添加请假信息
        /// </summary>
        public ICommand ContinueAddAttendLeaveCommand
        {
            get
            {
                if (_contuneAddAttendLeaveCmd == null)
                {
                    _contuneAddAttendLeaveCmd = new DelegateCommand<AddAttendLeaveDialog>(ContinueAddAttendLeave);
                }
                return _contuneAddAttendLeaveCmd;

            }
        }
        /// 删除请假信息
        /// </summary>
        public ICommand DelAttendLeaveCommand
        {
            get
            {
                if (_delAttendLeaveCmd == null)
                {
                    _delAttendLeaveCmd = new DelegateCommand(DelAttendLeave);
                }
                return _delAttendLeaveCmd;

            }
        }

        /// <summary>
        /// 更新请假信息
        /// </summary>
        public ICommand UpdateAttendLeaveCommand
        {
            get
            {
                if (_updateAttendLeaveCmd == null)
                {
                    _updateAttendLeaveCmd = new DelegateCommand<AddAttendLeaveDialog>(UpdateAttendLeave);
                }
                return _updateAttendLeaveCmd;

            }
        }

        #endregion

        #region Command 绑定函数


        /// <summary>
        /// 部门改变 则修改姓名列表
        /// </summary>
        private void DepartChanged(EventInformation<SelectionChangedEventArgs> ei)
        {
            ComboBox cmbDepart = ei.Sender as ComboBox;
            if (cmbDepart != null)
            {
                if (QueryConditionForLeave != null)
                {
                    _vmDepart.GetUserPersonSimple(QueryConditionForLeave.Depart.depart_id);
                }
            }
        }

        /// <summary>
        /// 工号输入则修改 部门与姓名
        /// </summary>
        private void WorkSNChanged(EventInformation<RoutedEventArgs> ei)
        {

        }


        /// <summary>
        /// 查询请假信息函数
        /// </summary>
        public void QueryAttendLeave()
        {
            DateTime? endDateTime = null;// by cty
            if (QueryConditionForLeave.EndDateTime != null)// by cty
            {
                endDateTime = (QueryConditionForLeave.EndDateTime).Value.AddDays(1);// by cty
            }
            AttendLeaveModel.Clear();
            foreach (var ar in AttendLeaveModelBase.Where(a => 
                (QueryConditionForLeave.Name != "" ? a.person_name.Contains(QueryConditionForLeave.Name) : true)
                && (QueryConditionForLeave.WorkSN != "" ? a.work_sn == QueryConditionForLeave.WorkSN : true)
                && ((QueryConditionForLeave.Depart != null && QueryConditionForLeave.Depart.depart_id > 0)
                        ? a.depart_id == QueryConditionForLeave.Depart.depart_id : true)
                && ((QueryConditionForLeave.BeginDateTime == null && endDateTime == null)
                || ((QueryConditionForLeave.BeginDateTime != null ? (endDateTime != null ? (a.leave_end_time > QueryConditionForLeave.BeginDateTime //by cty
                            && a.leave_start_time < endDateTime) : (a.leave_end_time > QueryConditionForLeave.BeginDateTime)) : false)))))         // by cty
            //|| ((QueryConditionForLeave.BeginDateTime != null ? (a.leave_end_time > QueryConditionForLeave.EndDateTime         // by cty
            //    && a.leave_start_time < QueryConditionForLeave.EndDateTime) : false)                                 // by cty
            //|| (QueryConditionForLeave.BeginDateTime != null ? (a.leave_end_time > QueryConditionForLeave.BeginDateTime // by cty
            //    && a.leave_start_time < QueryConditionForLeave.BeginDateTime) : false)))))              // by cty
            {
                AttendLeaveModel.Add(ar);
            }
        }

        /// <summary>
        /// 显示更新请假信息对话框
        /// </summary>
        public void ShowUpdateDialog()
        {
            try
            {
                AddAttendLeaveDialog dialog = new AddAttendLeaveDialog(_selectAttendLeave);

                //存储修改之前的请假信息，用于写入操作员日志 by cty
                _oldSelectAttendLeave.leave_type_name = _selectAttendLeave.leave_type_name;
                _oldSelectAttendLeave.is_leave_all_day = _selectAttendLeave.is_leave_all_day;
                _oldSelectAttendLeave.leave_start_time = _selectAttendLeave.leave_start_time;
                _oldSelectAttendLeave.leave_end_time = _selectAttendLeave.leave_end_time;
                _oldSelectAttendLeave.memo = _selectAttendLeave.memo;

                dialog.btnContinueAdd.Visibility = Visibility.Collapsed;
                dialog.Title = "修改请假";
                dialog.btnSave.Style = App.Current.Resources["OkButtonStyle"] as Style;
                dialog.btnSave.Command = UpdateAttendLeaveCommand;
                dialog.btnSave.CommandParameter = dialog;
                dialog.Show();
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }

        /// <summary>
        /// 显示添加请假信息对话框
        /// </summary>
        public void ShowAddDialog()
        {
            try
            {
                AddAttendLeaveDialog dialog = new AddAttendLeaveDialog();
                dialog.btnSave.Content = "添加";
                dialog.btnSave.Command = AddAttendLeaveCommand;
                dialog.btnSave.CommandParameter = dialog;
                dialog.btnContinueAdd.Command = ContinueAddAttendLeaveCommand;
                dialog.btnContinueAdd.CommandParameter = dialog;
                dialog.Show();
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }

        /// <summary>
        /// 添加更新请假信息
        /// </summary>
        /// <param name="dailog"></param>
        private void AddAttendLeave(AddAttendLeaveDialog dailog)
        {
            SetAttendLeave(dailog, true);
        }

        private void ContinueAddAttendLeave(AddAttendLeaveDialog dailog)
        {
            bool tag = true;
            try
            {
                UserAttendForLeave userLeave = new UserAttendForLeave();
                if (tag)
                {
                    ///新添加人员请假信息
                    if (dailog.cmbName.SelectedIndex > -1)
                    {
                        userLeave.person_id = ((UserPersonSimple)dailog.cmbName.SelectedItem).person_id;
                        userLeave.person_name = ((UserPersonSimple)dailog.cmbName.SelectedItem).person_name;
                        userLeave.work_sn = ((UserPersonSimple)dailog.cmbName.SelectedItem).work_sn;
                    }
                    else
                    {
                        MsgBoxWindow.MsgBox("请选择请假人员，请确认！",
                                                 MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);
                        return;
                    }
                }
                else
                {
                    ///更新人员请假信息
                    userLeave.attend_for_leave_id = _selectAttendLeave.attend_for_leave_id;
                    userLeave.person_id = _selectAttendLeave.person_id;
                    userLeave.person_name = _selectAttendLeave.person_name;
                    userLeave.depart_id = _selectAttendLeave.depart_id;
                    userLeave.depart_name = _selectAttendLeave.depart_name;
                    userLeave.work_sn = _selectAttendLeave.work_sn;
                }
                if (dailog.cmbLeaveType.SelectedIndex > -1)
                {
                    userLeave.leave_type_id = ((LeaveType)dailog.cmbLeaveType.SelectedItem).leave_type_id;
                    userLeave.leave_type_name = ((LeaveType)dailog.cmbLeaveType.SelectedItem).leave_type_name;

                }
                else
                {
                    MsgBoxWindow.MsgBox("请假类型不准确，请确认！",
                                          MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);
                    return;
                }

                //0是按天请假
                if (dailog.cmbType.SelectedIndex == 0)
                {
                    userLeave.is_leave_all_day = true;
                    if (dailog.dateBegin.SelectedDate != null && dailog.dateEnd.SelectedDate != null)
                    {
                        userLeave.leave_start_time = dailog.dateBegin.SelectedDate.Value.Date;
                        userLeave.leave_end_time = dailog.dateEnd.SelectedDate.Value.Date.AddDays(1).AddSeconds(-1);
                    }
                    else
                    {
                        MsgBoxWindow.MsgBox("请选择时间，请确认！",
                                             MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);
                        return;
                    }
                }
                else
                {
                    userLeave.is_leave_all_day = false;
                    if (dailog.dateBegin.SelectedDate != null && dailog.dateEnd.SelectedDate != null && dailog.timeBegin.Value != null && dailog.timeEnd.Value != null)
                    {
                        userLeave.leave_start_time = dailog.dateBegin.SelectedDate.Value.Date
                            + dailog.timeBegin.Value.Value.TimeOfDay;
                        userLeave.leave_end_time = dailog.dateEnd.SelectedDate.Value.Date
                            + dailog.timeEnd.Value.Value.TimeOfDay;
                    }
                    else
                    {
                        MsgBoxWindow.MsgBox("请选择具体时间点，请确认！",
                                             MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);
                        return;
                    }
                }

                if (userLeave.leave_end_time <= userLeave.leave_start_time)//.AddHours(1)
                {
                    MsgBoxWindow.MsgBox("请假开始时间大于结束时间，请确认！",
                                          MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);
                    return;
                }

                userLeave.memo = dailog.tbMemo.Text;
                //添加操作员日志的操作
                string str = "";
                Action<InvokeOperation<bool>> insetOrUpdateUserAttendForLeaveCallBack = CallBackHandleControl<bool>.OnInvokeErrorCallBack;
                CallBackHandleControl<bool>.m_sendValue = (o) =>
                {
                    if (o)
                    {
                        if (tag)
                        {
                            //添加操作员日志 by cty
                            str = "姓名：" + userLeave.person_name + "，工号：" + userLeave.work_sn + "\r\n请假时间：" + userLeave.leave_start_time.ToString() + "—" + userLeave.leave_end_time.ToString() + "\r\n请假类型：" + userLeave.leave_type_name + "；";
                            VmOperatorLog.InsertOperatorLog(1, "添加人员请假", str, () =>
                            {
                                MsgBoxWindow.MsgBox((tag ? "添加" : "更新") + "请假信息成功！",
                                                      MsgBoxWindow.MsgIcon.Succeed, MsgBoxWindow.MsgBtns.OK);
                                ServiceDomDbAcess.ReOpenSever();
                                //从新加载页面
                                GetAttendLeave();
                            });
                        }
                        else
                        {
                            str = "姓名：" + userLeave.person_name + "，工号：" + userLeave.work_sn;
                            if (_oldSelectAttendLeave.leave_type_name != userLeave.leave_type_name)
                            {
                                str += "，请假类型：" + _oldSelectAttendLeave.leave_type_name + "->" + userLeave.leave_type_name;
                            }
                            if (_oldSelectAttendLeave.is_leave_all_day != userLeave.is_leave_all_day)
                            {
                                str += "，请假方式：" + (_oldSelectAttendLeave.is_leave_all_day ? "按天请假" : "按小时请假") + "->" + (userLeave.is_leave_all_day ? "按天请假" : "按小时请假");
                            }
                            if (_oldSelectAttendLeave.leave_start_time != userLeave.leave_start_time)
                            {
                                str += "，请假开始时间：" + _oldSelectAttendLeave.leave_start_time.ToString() + "->" + userLeave.leave_start_time.ToString();
                            }
                            if (_oldSelectAttendLeave.leave_end_time != userLeave.leave_end_time)
                            {
                                str += "，请假结束时间：" + _oldSelectAttendLeave.leave_end_time.ToString() + "->" + userLeave.leave_end_time.ToString();
                            }
                            if (_oldSelectAttendLeave.memo != userLeave.memo)
                            {
                                str += "，备注：" + _oldSelectAttendLeave.memo + "->" + userLeave.memo;
                            }
                            str += "；";
                            if (str != "姓名：" + userLeave.person_name + "，工号：" + userLeave.work_sn + "；")
                            {
                                //添加操作员日志 by cty
                                VmOperatorLog.InsertOperatorLog(1, "修改人员请假", str, () =>
                                {
                                    MsgBoxWindow.MsgBox((tag ? "添加" : "更新") + "请假信息成功！",
                                                          MsgBoxWindow.MsgIcon.Succeed, MsgBoxWindow.MsgBtns.OK);
                                    ServiceDomDbAcess.ReOpenSever();
                                    //从新加载页面
                                    GetAttendLeave();
                                });
                            }
                        }
                    }
                    else
                    {
                        //添加操作员日志 by cty
                        str = "姓名：" + userLeave.person_name + "，工号：" + userLeave.work_sn + "\r\n请假时间：" + userLeave.leave_start_time.ToString() + "—" + userLeave.leave_end_time.ToString() + "\r\n请假类型：" + userLeave.leave_type_name;
                        VmOperatorLog.InsertOperatorLog(0, (tag ? "添加" : "修改") + "人员请假", str, () =>
                        {
                            MsgBoxWindow.MsgBox((tag ? "添加" : "更新") + "请假信息失败！",
                                                  MsgBoxWindow.MsgIcon.Error, MsgBoxWindow.MsgBtns.OK);
                        });
                    }

                };

                ServiceDomDbAcess.GetSever().IrisInsetOrUpdateUserAttendForLeave(userLeave, insetOrUpdateUserAttendForLeaveCallBack, new Object());
               // dailog.DialogResult = true;

            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }
        /// <summary>
        /// 设置请假信息 设置请假信息，tag = true  为添加请假信息
        ///                            tag = false 为更新请假信息
        /// </summary>
        /// <param name="dailog"></param>
        /// <param name="tag">是否为添加</param>
        private void SetAttendLeave(AddAttendLeaveDialog dailog, bool tag)
        {
            try
            {
                UserAttendForLeave userLeave = new UserAttendForLeave();
                if (tag)
                {
                    ///新添加人员请假信息
                    if (dailog.cmbName.SelectedIndex > -1)
                    {
                        userLeave.person_id = ((UserPersonSimple)dailog.cmbName.SelectedItem).person_id;
                        userLeave.person_name = ((UserPersonSimple)dailog.cmbName.SelectedItem).person_name;
                        userLeave.work_sn = ((UserPersonSimple)dailog.cmbName.SelectedItem).work_sn;
                    }
                    else
                    {
                        MsgBoxWindow.MsgBox( "请选择请假人员，请确认！",
                                                 MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);
                        return;
                    }
                }
                else
                {
                    ///更新人员请假信息
                    userLeave.attend_for_leave_id = _selectAttendLeave.attend_for_leave_id;
                    userLeave.person_id = _selectAttendLeave.person_id;
                    userLeave.person_name = _selectAttendLeave.person_name;
                    userLeave.depart_id = _selectAttendLeave.depart_id;
                    userLeave.depart_name = _selectAttendLeave.depart_name;
                    userLeave.work_sn = _selectAttendLeave.work_sn;
                }
                if (dailog.cmbLeaveType.SelectedIndex > -1)
                {
                    userLeave.leave_type_id = ((LeaveType)dailog.cmbLeaveType.SelectedItem).leave_type_id;
                    userLeave.leave_type_name = ((LeaveType)dailog.cmbLeaveType.SelectedItem).leave_type_name;

                }
                else
                {
                    MsgBoxWindow.MsgBox( "请假类型不准确，请确认！",
                                          MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);
                    return;
                }

                //0是按天请假
                if (dailog.cmbType.SelectedIndex == 0)
                {
                    userLeave.is_leave_all_day = true;
                    if (dailog.dateBegin.SelectedDate != null && dailog.dateEnd.SelectedDate != null)
                    {
                        userLeave.leave_start_time = dailog.dateBegin.SelectedDate.Value.Date;
                        userLeave.leave_end_time = dailog.dateEnd.SelectedDate.Value.Date.AddDays(1).AddSeconds(-1);
                    }
                    else
                    {
                        MsgBoxWindow.MsgBox( "请选择时间，请确认！",
                                             MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);
                        return;
                    }
                }
                else
                {
                    userLeave.is_leave_all_day = false;
                    if (dailog.dateBegin.SelectedDate != null && dailog.dateEnd.SelectedDate != null && dailog.timeBegin.Value != null && dailog.timeEnd.Value!=null)
                    {
                        userLeave.leave_start_time = dailog.dateBegin.SelectedDate.Value.Date
                            + dailog.timeBegin.Value.Value.TimeOfDay;
                        userLeave.leave_end_time = dailog.dateEnd.SelectedDate.Value.Date
                            + dailog.timeEnd.Value.Value.TimeOfDay;
                    }
                    else
                    {
                        MsgBoxWindow.MsgBox( "请选择具体时间点，请确认！",
                                             MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);
                        return;
                    }
                }

                if (userLeave.leave_end_time <= userLeave.leave_start_time)//.AddHours(1)
                {
                    MsgBoxWindow.MsgBox( "请假开始时间大于结束时间，请确认！",
                                          MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);
                    return;
                }

                userLeave.memo = dailog.tbMemo.Text;
                //添加操作员日志的操作
                string str = "";
                Action<InvokeOperation<bool>> insetOrUpdateUserAttendForLeaveCallBack = CallBackHandleControl<bool>.OnInvokeErrorCallBack;
                CallBackHandleControl<bool>.m_sendValue = (o) =>
                {
                    if (o)
                    {
                        if (tag)
                        {
                            //添加操作员日志 by cty
                            str = "姓名：" + userLeave.person_name + "，工号：" + userLeave.work_sn + "\r\n请假时间：" + userLeave.leave_start_time.ToString() + "—" + userLeave.leave_end_time.ToString() + "\r\n请假类型：" + userLeave.leave_type_name + "；";                            
                            VmOperatorLog.InsertOperatorLog(1, "添加人员请假",  str, () =>
                            {
                                MsgBoxWindow.MsgBox((tag ? "添加" : "更新") + "请假信息成功！",
                                                      MsgBoxWindow.MsgIcon.Succeed, MsgBoxWindow.MsgBtns.OK);
                                ServiceDomDbAcess.ReOpenSever();
                                //从新加载页面
                                GetAttendLeave();
                            });
                        }
                        else
                        {
                            str = "姓名：" + userLeave.person_name + "，工号：" + userLeave.work_sn ;
                            if (_oldSelectAttendLeave.leave_type_name != userLeave.leave_type_name)
                            {
                                str += "，请假类型：" + _oldSelectAttendLeave.leave_type_name + "->" + userLeave.leave_type_name;
                            }
                            if (_oldSelectAttendLeave.is_leave_all_day != userLeave.is_leave_all_day)
                            {
                                str += "，请假方式：" + (_oldSelectAttendLeave.is_leave_all_day ? "按天请假" : "按小时请假") + "->" +( userLeave.is_leave_all_day ? "按天请假" : "按小时请假");
                            }
                            if (_oldSelectAttendLeave.leave_start_time != userLeave.leave_start_time)
                            {
                                str += "，请假开始时间：" + _oldSelectAttendLeave.leave_start_time.ToString() + "->" + userLeave.leave_start_time.ToString() ;
                            }
                            if (_oldSelectAttendLeave.leave_end_time != userLeave.leave_end_time)
                            {
                                str += "，请假结束时间：" + _oldSelectAttendLeave.leave_end_time.ToString() + "->" + userLeave.leave_end_time.ToString() ;
                            }
                            if (_oldSelectAttendLeave.memo != userLeave.memo)
                            {
                                str += "，备注：" + _oldSelectAttendLeave.memo + "->" + userLeave.memo;
                            }
                            str += "；";
                            if (str != "姓名：" + userLeave.person_name + "，工号：" + userLeave.work_sn +"；")
                            {
                                //添加操作员日志 by cty
                                VmOperatorLog.InsertOperatorLog(1, "修改人员请假", str, () =>
                                {
                                    MsgBoxWindow.MsgBox((tag ? "添加" : "更新") + "请假信息成功！",
                                                          MsgBoxWindow.MsgIcon.Succeed, MsgBoxWindow.MsgBtns.OK);
                                    ServiceDomDbAcess.ReOpenSever();
                                    //从新加载页面
                                    GetAttendLeave();
                                });
                            }
                        }
                    }
                    else
                    {
                        //添加操作员日志 by cty
                        str = "姓名：" + userLeave.person_name + "，工号：" + userLeave.work_sn + "\r\n请假时间：" + userLeave.leave_start_time.ToString() + "—" + userLeave.leave_end_time.ToString() + "\r\n请假类型：" + userLeave.leave_type_name;
                        VmOperatorLog.InsertOperatorLog(0, (tag ? "添加" : "修改") + "人员请假", str, () =>
                        {
                            MsgBoxWindow.MsgBox((tag ? "添加" : "更新") + "请假信息失败！",
                                                  MsgBoxWindow.MsgIcon.Error, MsgBoxWindow.MsgBtns.OK);
                        });
                    }
                    
                };

                ServiceDomDbAcess.GetSever().IrisInsetOrUpdateUserAttendForLeave(userLeave, insetOrUpdateUserAttendForLeaveCallBack, new Object());
                dailog.DialogResult = true;

            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }

        /// <summary>
        /// 删除请假信息
        /// </summary>
        private void DelAttendLeave()
        {
            MsgBoxWindow.MsgBox( "请注意，您将进行如下操作：\r\n删除 \"" + _selectAttendLeave.person_name + "\"的请假信息！",
                      MsgBoxWindow.MsgIcon.Warning, MsgBoxWindow.MsgBtns.OKCancel,
                      (e) =>
                      {
                          if (e == MsgBoxWindow.MsgResult.OK)
                          {
                              //获取删除的请假信息，用于写入操作员日志 by cty
                              string deleteStr = "姓名：" + _selectAttendLeave.person_name +
                                  "，工号：" + _selectAttendLeave.work_sn +
                                  "\r\n请假开始时间：" + _selectAttendLeave.leave_start_time +
                                  "—" + _selectAttendLeave.leave_end_time +
                                  "\r\n请假类型：" + _selectAttendLeave.leave_type_name +
                                  "；";
                              try
                              {
                                  Action<InvokeOperation<bool>> deleteUserAttendForLeaveCallBack = CallBackHandleControl<bool>.OnInvokeErrorCallBack;
                                  CallBackHandleControl<bool>.m_sendValue = (o) =>
                                  {
                                      if (o)
                                      {
                                          //添加操作员日志 by cty
                                          VmOperatorLog.InsertOperatorLog(1, "删除请假信息", deleteStr, () =>
                                          {
                                              MsgBoxWindow.MsgBox("删除请假信息成功！",
                                                                    MsgBoxWindow.MsgIcon.Succeed, MsgBoxWindow.MsgBtns.OK); 
                                              ServiceDomDbAcess.ReOpenSever();
                                          });
                                      }
                                      else
                                      {
                                          //添加操作员日志 by cty
                                          VmOperatorLog.InsertOperatorLog(0, "删除请假信息", deleteStr, () =>
                                          {
                                              MsgBoxWindow.MsgBox("删除请假信息失败！",
                                                                    MsgBoxWindow.MsgIcon.Error, MsgBoxWindow.MsgBtns.OK);
                                          });
                                      }

                                      //从新加载页面
                                      GetAttendLeave();
                                  };
                                  ServiceDomDbAcess.GetSever().IrisDeleteUserAttendForLeave(_selectAttendLeave.attend_for_leave_id,
                                      deleteUserAttendForLeaveCallBack, new Object());
                              }
                              catch (Exception er)
                              {
                                  ErrorWindow err = new ErrorWindow(er);
                                  err.Show();
                              }
                          }
                      });
        }

        /// <summary>
        /// 更新请假信息
        /// </summary>
        /// <param name="dailog"></param>
        private void UpdateAttendLeave(AddAttendLeaveDialog dailog)
        {
            SetAttendLeave(dailog, false);
        }

        #endregion

        #region 绑定排序事件

        /// <summary>
        /// DataGrid 字段汉语拼音排序
        /// </summary>
        /// <param name="ei"></param>
        private void SortAttendLeave(EventInformation<MouseButtonEventArgs> ei)
        {
            try
            {
                EventInformation<MouseButtonEventArgs> eventInfo = ei as EventInformation<MouseButtonEventArgs>;

                System.Windows.Controls.DataGrid sender = eventInfo.Sender as System.Windows.Controls.DataGrid;
                MouseButtonEventArgs e = ei.EventArgs;

                MyDataGridSortInChinese.OrderData(sender, e, AttendLeaveModel);
             
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }

        /// <summary>
        /// 排序箭头显示
        /// </summary>
        /// <param name="ei"></param>
        private void MouseMoveAttendLeave(EventInformation<MouseEventArgs> ei)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        /// <summary>
        /// 排序箭头显示
        /// </summary>
        /// <param name="ei"></param>
        private void LayoutUpadtaAttendLeave(EventInformation<EventArgs> ei)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }
 
        #endregion
    }

    /// <summary>
    /// 绑定全选CheckBox 类
    /// </summary>
    public class QueryConditionBind : Entity
    {
        DateTime now = DateTime.Now;
        
        /// <summary>
        /// 起始时间
        /// </summary>
        private DateTime? _beginDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

        //DateTime nowfirstDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        //DateTime nowlastDay = nowfirstDay.AddMonths(1).AddDays(-1);
        /// <summary>
        /// 结束时间
        /// </summary>
        private DateTime? _endDateTime = new DateTime(DateTime.Now.AddMonths(1).Year,DateTime.Now.AddMonths(1).Month,1).AddDays(-1);

        private string _name;

        private string _workSN;

        private depart _depart;

        public QueryConditionBind()
        {
            _name = "";
            _workSN = "";
        }

        /// <summary>
        /// 起始时间
        /// </summary>
        public DateTime? BeginDateTime
        {
            get
            {
                return _beginDateTime;   
            }
            set
            {
                if (_beginDateTime != value)
                {
                    _beginDateTime = value;
                }
                RaiseDataMemberChanging("EndDateTime");
                ValidateProperty("EndDateTime", value);
                RaiseDataMemberChanged("EndDateTime");
            }
        }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndDateTime
        {
            get
            {
                return _endDateTime;
            }
            set
            {
                if (_endDateTime != value)
                {
                    _endDateTime = value;
                }
                RaiseDataMemberChanging("EndDateTime");
                ValidateProperty("EndDateTime", value);
                RaiseDataMemberChanged("EndDateTime");
            }
        }

        /// <summary>
        /// 人员名字
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (_name != value)
                {
                    _name = value;
                }
                RaiseDataMemberChanging("Name");
                ValidateProperty("Name", value);
                RaiseDataMemberChanged("Name");
            }
        }


        /// <summary>
        /// 人员工号
        /// </summary>
        public string WorkSN
        {
            get
            {
                return _workSN;
            }
            set
            {
                if (_workSN != value)
                {
                    _workSN = value;
                }
                RaiseDataMemberChanging("WorkSN");
                ValidateProperty("WorkSN", value);
                RaiseDataMemberChanged("WorkSN");
            }
        }


        /// <summary>
        /// 人员部门
        /// </summary>
        public depart Depart
        {
            get
            {
                return _depart;
            }
            set
            {
                if (_depart != value)
                {
                    _depart = value;
                }
                RaiseDataMemberChanging("Depart");
                ValidateProperty("Depart", value);
                RaiseDataMemberChanged("Depart");
            }
        }

    }

}
