/*************************************************************************
** 文件名:   VmLeaveTypeManagecs.cs
** 主要类:   VmLeaveTypeManagecs
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   cty
** 日  期:   2013-10-31
** 修改人:   
** 日  期:   
 *修改内容： 
** 描  述:   VmLeaveTypeManagecs，增加、修改考勤类型的页面
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
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Irisking.Web.DataModel;
using IriskingAttend.BehaviorSelf;
using Microsoft.Practices.Prism.Commands;
using IriskingAttend.Web;
using EDatabaseError;
using System.ServiceModel.DomainServices.Client;
using IriskingAttend.Common;
using IriskingAttend.Dialog;
using IriskingAttend.ViewModel.SystemViewModel;

namespace IriskingAttend.ViewModel.AttendViewModel
{
    public class VmLeaveTypeManagecs : BaseViewModel
    {
        #region 变量声明

        /// <summary>
        /// 操作模式
        /// </summary>
        private ChildWndOptionMode _mode ;

        /// <summary>
        /// 域服务声明
        /// </summary>
        private DomainServiceIriskingAttend _domainService = new DomainServiceIriskingAttend();

        /// <summary>
        /// 传递给.web的要添加或修改的考勤类型
        /// </summary>
        private LeaveType _leaveType;

        /// <summary>
        /// 修改考勤类型时记录被修改的考勤类型，用于与修改后的考勤类型比较，来写操作员日志
        /// </summary>
        private LeaveType _oldleaveType
        {
            get;
            set;
        }

        /// <summary>
        /// 点击取消按钮后是否要刷新页面
        /// </summary>
        private bool _isRefresh = false;

        #endregion

        #region 界面绑定的属性

        //标题
        private string _title;
        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get 
            {
                return _title;
               
            }
            set
            {
                _title = value;
                this.OnPropertyChanged(() => Title);
            }
        }

        //继续添加按钮可见性
        private Visibility _btnBatchAddVisibility = Visibility.Visible;
        /// <summary>
        /// 继续添加按钮可见性
        /// </summary>
        public Visibility BtnBatchAddVisibility
        {
            get
            {
                return _btnBatchAddVisibility;
            }
            set
            {
                _btnBatchAddVisibility = value;
                this.OnPropertyChanged(() => BtnBatchAddVisibility);
            }
        }

        //考勤类型id
        private int _leaveTypeId = -1;
        /// <summary>
        /// 考勤类型id
        /// </summary>
        public int LeaveTypeId
        {
            get
            {
                return _leaveTypeId;

            }
            set
            {
                _leaveTypeId = value;
                this.OnPropertyChanged(() => LeaveTypeId);
            }
        }

        //考勤类型名称
        private string _leaveTypeName;
        /// <summary>
        /// 考勤类型名称
        /// </summary>
        public string LeaveTypeName
        {
            get
            {
                return _leaveTypeName;

            }
            set
            {
                _leaveTypeName = value;
                this.OnPropertyChanged(() => LeaveTypeName);
            }
        }

        //考勤符号
        private string _attendSign;
        /// <summary>
        /// 考勤符号
        /// </summary>
        public string AttendSign
        {
            get
            {
                return _attendSign;

            }
            set
            {
                _attendSign = value;
                this.OnPropertyChanged(() => AttendSign);
            }
        }

        //备注
        private string _memo;
        /// <summary>
        /// 备注
        /// </summary>
        public string Memo
        {
            get
            {
                return _memo;

            }
            set
            {
                _memo = value;
                this.OnPropertyChanged(() => Memo);
            }
        }

        //是否记工
        private int _isSchedule;
        /// <summary>
        /// 是否记工
        /// </summary>
        public int IsSchedule
        {
            get
            {
                return _isSchedule;

            }
            set
            {
                _isSchedule = value;
                this.OnPropertyChanged(() => IsSchedule);
            }
        }

        //考勤类别
        private int _isNormalAttend;
        /// <summary>
        /// 考勤类别
        /// </summary>
        public int IsNormalAttend
        {
            get
            {
                return _isNormalAttend;

            }
            set
            {
                _isNormalAttend = value;
                this.OnPropertyChanged(() => IsNormalAttend);
            }
        }

        /// <summary>
        /// 窗口关闭事件
        /// </summary>
        public event Action<bool> CloseEvent;

        #endregion

        #region 界面命令绑定

        /// <summary>
        /// 添加命令
        /// </summary>
        public DelegateCommand AddCommand
        {
            get;
            set;
        }

        /// <summary>
        /// 继续添加命令
        /// </summary>
        public DelegateCommand BatchAddCommand
        {
            get;
            set;
        }

        /// <summary>
        /// 取消命令
        /// </summary>
        public DelegateCommand CancelCommand
        {
            get;
            set;
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public VmLeaveTypeManagecs(LeaveType leaveType, ChildWndOptionMode mode)
        {
            _leaveType = new LeaveType();
            _isRefresh = false;
            _mode = mode;

            if (_mode == ChildWndOptionMode.Modify)
            {
                _oldleaveType = new LeaveType();

                Title = "修改请假类型";
                BtnBatchAddVisibility = Visibility.Collapsed;

                _oldleaveType.leave_type_id = leaveType.leave_type_id;
                _oldleaveType.leave_type_name = leaveType.leave_type_name;
                _oldleaveType.attend_sign = leaveType.attend_sign;
                _oldleaveType.is_normal_attend = leaveType.is_normal_attend;
                _oldleaveType.is_schedule = leaveType.is_schedule;
                _oldleaveType.memo = leaveType.memo;

                _leaveType = leaveType;

                LeaveTypeId = leaveType.leave_type_id;
                LeaveTypeName = leaveType.leave_type_name;
                AttendSign = leaveType.attend_sign;
                Memo = leaveType.memo;
                IsSchedule = leaveType.is_schedule;
                IsNormalAttend = leaveType.is_normal_attend;
            }
            if (_mode == ChildWndOptionMode.Add)
            {
                Title = "添加请假类型";
                IsSchedule = 0;
                IsNormalAttend = 0;

            }

            AddCommand = new DelegateCommand(AddLeaveTypeOk);
            BatchAddCommand = new DelegateCommand(BatchAddLeaveTypeOk);
            CancelCommand = new DelegateCommand(CancelLeaveType);
        }

        #endregion

        #region 添加修改考勤类型操作

        /// <summary>
        /// 添加考勤类型确认操作
        /// </summary>
        private void AddLeaveTypeOk()
        {
            AddOperation(false);
        }

        /// <summary>
        /// 添加考勤类型继续添加操作
        /// </summary>
        private void BatchAddLeaveTypeOk()
        {
            AddOperation(true);
        }

        /// <summary>
        /// 添加考勤类型操作
        /// </summary>
        /// <param name="isBatch">是否是批量添加操作</param>
        private void AddOperation(bool isBatch)
        {
            _leaveType.leave_type_id = LeaveTypeId;
            _leaveType.leave_type_name = LeaveTypeName;
            _leaveType.attend_sign = AttendSign == "" ? null : AttendSign; 
            _leaveType.is_normal_attend = short.Parse(IsNormalAttend.ToString());
            _leaveType.is_schedule = IsSchedule;
            _leaveType.memo = Memo == "" ? null : Memo;

            if (_leaveType.leave_type_name == "" || _leaveType.leave_type_name == null)
            {
                MsgBoxWindow.MsgBox("请假类型名称不能为空，请重新输入！", MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);
                return;
            }

            WaitingDialog.ShowWaiting();
            Action<InvokeOperation<string>> onInvokeErrCallBack = CallBackHandleControl<string>.OnInvokeErrorCallBack;
            CallBackHandleControl<string>.m_sendValue = (o) =>
            {
                //异步获取数据   
                switch (o)
                {
                    case "InsertSuccess":
                        if (!isBatch)
                        {                          
                            //添加操作员日志
                            VmOperatorLog.InsertOperatorLog(1, "添加请假类型", "请假类型名称：" + LeaveTypeName, () =>
                            {
                                CloseEvent(true);
                                MsgBoxWindow.MsgBox("添加请假类型成功！", MsgBoxWindow.MsgIcon.Succeed, MsgBoxWindow.MsgBtns.OK);
                            }); 
                        }
                        else
                        {
                            //添加操作员日志
                            VmOperatorLog.InsertOperatorLog(1, "添加请假类型", "请假类型名称：" + LeaveTypeName, () =>
                            {
                                MsgBoxWindow.MsgBox("添加请假类型成功！", MsgBoxWindow.MsgIcon.Succeed, MsgBoxWindow.MsgBtns.OK);
                            }); 
                            LeaveTypeName = "";
                            AttendSign = "";
                            Memo = "";
                        }
                         _isRefresh = true;
                        break;
                    case "LeaveTypeHasExist":
                        MsgBoxWindow.MsgBox("您输入的【请假类型名称】已被占用，请重新输入！", MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);
                        break;
                    case "InsertError":
                        //添加操作员日志
                        VmOperatorLog.InsertOperatorLog(0, "添加请假类型", "请假类型名称为:" + LeaveTypeName, () =>
                        {
                            MsgBoxWindow.MsgBox("添加请假类型失败！", MsgBoxWindow.MsgIcon.Error, MsgBoxWindow.MsgBtns.OK);
                        });                    
                        break;
                    case "UpdateSuccess":
                        
                        //添加操作员日志
                        string str = "请假类型名称："+_oldleaveType.leave_type_name ;

                        if (_oldleaveType.leave_type_name != _leaveType.leave_type_name)
                        {
                            str += "\r\n请假类型名称：" + _oldleaveType.leave_type_name + "->" + _leaveType.leave_type_name;
                        }
                        if (_oldleaveType.attend_sign != (_leaveType.attend_sign == null ? "" : _leaveType.attend_sign))
                        {
                            str += "，请假符号：" + _oldleaveType.attend_sign + "->" + _leaveType.attend_sign ;
                        }
                        if (_oldleaveType.is_schedule != _leaveType.is_schedule)
                        {
                            str += "，是否记工：" + (_oldleaveType.is_schedule == 0 ? "否" : "是") + "->" + (_leaveType.is_schedule == 0 ? "否" : "是") ;
                        }
                        if (_oldleaveType.is_normal_attend != _leaveType.is_normal_attend)
                        {
                            str += "，请假类型：" + (_oldleaveType.is_normal_attend == 0 ? "不计入出勤" : (_oldleaveType.is_normal_attend == 1 ? "计入出勤" : "有条件计入出勤")) + "->" + (_leaveType.is_normal_attend == 0 ? "不计入出勤" : (_leaveType.is_normal_attend == 1 ? "计入出勤" : "有条件计入出勤")) ;
                        }
                        if (_oldleaveType.memo != (_leaveType.memo == null ? "" : _leaveType.memo))
                        {
                            str += "，备注：" + _oldleaveType.memo + "->" + _leaveType.memo ;
                        }
                        if (str != "请假类型名称：" + _oldleaveType.leave_type_name)
                        {
                            str += "；";
                            VmOperatorLog.InsertOperatorLog(1, "修改请假类型", str, () =>
                            {
                                CloseEvent(true);
                                MsgBoxWindow.MsgBox("修改请假类型成功！", MsgBoxWindow.MsgIcon.Succeed, MsgBoxWindow.MsgBtns.OK);
                            });
                        }
                        break;
                    case "UpdateError":
                        
                        VmOperatorLog.InsertOperatorLog(0, "修改请假类型", "请假类型名称：" + _oldleaveType.leave_type_name , () =>
                        {
                            CloseEvent(true);
                            MsgBoxWindow.MsgBox("修改请假类型失败！", MsgBoxWindow.MsgIcon.Error, MsgBoxWindow.MsgBtns.OK);
                        });
                        break;

                }

                WaitingDialog.HideWaiting();
            };

   
            //调用后台进行添加考勤类型
            _domainService.InsertOrUpdateLeaveType(_leaveType, onInvokeErrCallBack, null);
            
        }

        /// <summary>
        /// 添加考勤类型取消操作
        /// </summary>
        private void CancelLeaveType()
        {
            CloseEvent(_isRefresh);
        }

        #endregion

    }
}
