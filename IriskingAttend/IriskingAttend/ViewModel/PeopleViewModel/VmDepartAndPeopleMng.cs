/*************************************************************************
** 文件名:   VmPeopleMng.cs
×× 主要类:   VmPeopleMng
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   cty
** 日  期:   2013-6-14
** 修改人:   wz 代码优化
** 日  期:   2013-7-24
** 描  述:   VmPeopleMng类,人员信息的显示及根据查询条件显示
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
using System.Linq;
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
using IriskingAttend.Dialog;
using GalaSoft.MvvmLight.Command;
using MvvmLightCommand.SL4.TriggerActions;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using IriskingAttend.View;
using IriskingAttend.BehaviorSelf;
using IriskingAttend.ViewMine.PeopleView;
using System.Collections;
using IriskingAttend.ViewModel.SystemViewModel;
using System.Text;
using IriskingAttend.AsyncControl;



namespace IriskingAttend.ViewModel.PeopleViewModel
{
    public class VmDepartAndPeopleMng : BaseViewModel
    {

        /// <summary>
        /// -1 代表全部部门
        /// </summary>
        public static int AllDepartId = -1;

        #region 字段声明
        /// <summary>
        /// 异步任务执行器
        /// </summary>
        private AsyncActionRunner _runnerTask;

       

        /// <summary>
        /// 与服务声明
        /// </summary>
        private DomainServiceIriskingAttend _serviceDomDbAccess = new DomainServiceIriskingAttend();

        /// <summary>
        /// depart载入完毕事件
        /// </summary>
        private event EventHandler _departmentLoadCompleted;

        /// <summary>
        /// vm加载完毕事件
        /// </summary>
        public event EventHandler LoadCompletedEvent;

        /// <summary>
        /// 竖直拖动栏，拖动偏移量改变事件
        /// </summary>
        public Action ScrollBarVerticalOffsetLayout;

        //当前部门id
        private int _curDepartId;
        private int curDepartId
        {
            get
            {
                return _curDepartId;
            }
            set
            {
                _curDepartId = value;
                if (value == AllDepartId) //-1代表全部部门
                {
                    IsOperateDepartBtnEnable = false;
                }
                else
                {
                    IsOperateDepartBtnEnable = true;
                }
            }
        }

        /// <summary>
        /// 总部门
        /// </summary>
        public TreeNode<UserDepartInfo> DepartAll = null;
       

        #endregion

        #region 与界面绑定的属性

        #region 命令属性

        /// <summary>
        /// 停止批量添加识别记录命令
        /// </summary>
        public DelegateCommand StopBatchAddRecordCommand
        {
            get;
            set;
        }
      
        /// <summary>
        /// 查询员工信息命令
        /// </summary>
        public DelegateCommand QueryPersonInfoCommand 
        {
            get; 
            set; 
        }
        
        /// <summary>
        /// 增加一个新员工命令
        /// </summary>
        public DelegateCommand AddPersonInfoCommand 
        { 
            get;
            set;
        }
        
        /// <summary>
        /// 批量修改人员属性命令
        /// </summary>
        public DelegateCommand BatchModifyCommand 
        {
            get; 
            set; 
        }

        /// <summary>
        /// 批量删除命令
        /// </summary>
        public DelegateCommand BatchDeleteCommand 
        {
            get; 
            set; 
        }
        
        /// <summary>
        /// 批量停用虹膜命令
        /// </summary>
        public DelegateCommand BatchStopIrisCommand 
        {
            get; 
            set; 
        }

        public DelegateCommand BatchAddRecordCommand
        {
            get;
            set;
        }

        /// <summary>
        /// 批量添加请假 by cty
        /// </summary>
        public DelegateCommand BatchAddAttendLeaveCommand
        {
            get;
            set;
        }
        #endregion


        #region 其他属性

        
        private Visibility _isBatchAddRecordVisible = Visibility.Collapsed;
        /// <summary>
        /// 表示批量添加识别记录是否正在进行
        /// </summary>
        public Visibility IsBatchAddRecordVisible
        {
            get { return _isBatchAddRecordVisible; }
            set
            {
                _isBatchAddRecordVisible = value;
                this.OnPropertyChanged(() => IsBatchAddRecordVisible);
            }
        }


        private double _batchAddRecordProgress = 0;
        /// <summary>
        /// 表示批量添加识别记录进度条的值，最大为100，最小为0
        /// </summary>
        public double BatchAddRecordProgress
        {
            get { return _batchAddRecordProgress; }
            set
            {
                _batchAddRecordProgress = value;
                this.OnPropertyChanged(() => BatchAddRecordProgress);
            }
        }

        private string _stopBatchAddRecordBtnContent;
        /// <summary>
        /// 停止批量添加识别记录的按钮的内容
        /// </summary>
        public string StopBatchAddRecordBtnContent
        {
            get { return _stopBatchAddRecordBtnContent; }
            set
            {
                _stopBatchAddRecordBtnContent = value;
                this.OnPropertyChanged(() => StopBatchAddRecordBtnContent);
            }
        }

        private bool _isStopBatchAddRecordBtnEnable;
        /// <summary>
        /// 停止批量添加识别记录的按钮的enable
        /// </summary>
        public bool IsStopBatchAddRecordBtnEnable
        {
            get { return _isStopBatchAddRecordBtnEnable; }
            set
            {
                _isStopBatchAddRecordBtnEnable = value;
                this.OnPropertyChanged(() => IsStopBatchAddRecordBtnEnable);
            }
        }


        //部门树数据结构
        private TreeNode<UserDepartInfo> _departTreeData;

        /// <summary>
        /// 部门树数据
        /// </summary>
        public BaseViewModelCollection<TreeNode<UserDepartInfo>> TreeData
        {
            get
            {
                if (_departTreeData == null)
                {
                    return null;
                }
                return _departTreeData.Children;
            }
            set
            {
                _departTreeData.Children = value;
                this.OnPropertyChanged<BaseViewModelCollection<TreeNode<UserDepartInfo>>>(() => this.TreeData);
            }
        }
     
        /// <summary>
        /// 全选按钮的绑定类
        /// </summary>
        public MarkObject MarkObj
        { 
            get; 
            set; 
        }
 

        private BaseViewModelCollection<UserDepartInfo> departs;

        /// <summary>
        /// 可供选择的部门列表
        /// </summary>
        public BaseViewModelCollection<UserDepartInfo> Departs
        {
            get 
            {
                return departs; 
            }
            set
            {
                departs = value;
                this.OnPropertyChanged(() => this.Departs);
            }
        }

        private int _irisRegistersSelectedIndex;

        /// <summary>
        /// 当前选择的虹膜注册
        /// </summary>
        public int IrisRegistersSelectedIndex
        {
            get 
            { 
                return _irisRegistersSelectedIndex; 
            }
            set
            {
                _irisRegistersSelectedIndex = value;
                this.OnPropertyChanged(() => this.IrisRegistersSelectedIndex);
            }
        }

        private List<string> _irisRegisters;
        /// <summary>
        /// 可供选择的虹膜注册列表
        /// </summary>
        public List<string> IrisRegisters
        {
            get
            { 
                return _irisRegisters;
            }
            set
            {
                _irisRegisters = value;
                this.OnPropertyChanged(() => this.IrisRegisters);
            }
        }

        private List<string> _irisStatus;
        /// <summary>
        /// 可供选择的虹膜状态
        /// </summary>
        public List<string> IrisStatus
        {
            get 
            {
                return _irisStatus; 
            }
            set
            {
                _irisStatus = value;
                this.OnPropertyChanged(() => this.IrisStatus);
            }
        }

        private int _irisStatusSelectedIndex;

        /// <summary>
        /// 当前选择的虹膜状态
        /// </summary>
        public int IrisStatusSelectedIndex
        {
            get
            { 
                return _irisStatusSelectedIndex; 
            }
            set
            {
                _irisStatusSelectedIndex = value;
                this.OnPropertyChanged(() => this.IrisStatusSelectedIndex);
            }
        }

        private string _personName;
        /// <summary>
        /// 人员名字
        /// </summary>
        public string PersonName
        {
            get 
            {
                return _personName; 
            }
            set
            {
                _personName = value;
                this.OnPropertyChanged(() => this.PersonName);
            }
        }

       
        private string _personWorkSn;
        /// <summary>
        /// 人员工号
        /// </summary>
        public string PersonWorkSn
        {
            get
            { 
                return _personWorkSn; 
            }
            set
            {
                _personWorkSn = value;
                this.OnPropertyChanged(() => this.PersonWorkSn);
            }
        }

        private bool _isBatchOperateBtnEnable;
        /// <summary>
        /// 批量操作按钮的enable属性
        /// </summary>
        public bool IsBatchOperateBtnEnable
        {
            get 
            { 
                return _isBatchOperateBtnEnable;
            }
            set
            {
                _isBatchOperateBtnEnable = value;
                this.OnPropertyChanged(() => this.IsBatchOperateBtnEnable);
            }
        }

       
        /// <summary>
        /// 添加人员操作按钮enable属性
        /// </summary>
        public bool IsAddPersonEnable
        {
            get 
            {
                foreach (var item in Departs)
                {
                    if (VmDepartMng.OperateDepartIdCollection.Contains(item.depart_id))
                    {
                        return true;
                    }
                }
                return false;
            }
           
        }

        private bool _isBatchStopIrisBtnEnable;
        /// <summary>
        /// 批量停用虹膜
        /// </summary>
        public bool IsBatchStopIrisBtnEnable
        {
            get 
            { 
                return _isBatchStopIrisBtnEnable;
            }
            set
            {
                _isBatchStopIrisBtnEnable = value;
                this.OnPropertyChanged(() => this.IsBatchStopIrisBtnEnable);
            }
        }

        private bool _isOperateDepartBtnEnable;
        /// <summary>
        /// 操作部门按钮的enable属性
        /// </summary>
        public bool IsOperateDepartBtnEnable
        {
            get 
            { 
                return _isOperateDepartBtnEnable;
            }
            set
            {
                _isOperateDepartBtnEnable = value;
                this.OnPropertyChanged(() => this.IsOperateDepartBtnEnable);
            }
        }

        private BaseViewModelCollection<UserPersonInfo> _userPersonInfos;
        /// <summary>
        /// 人员信息表
        /// </summary>
        public BaseViewModelCollection<UserPersonInfo> UserPersonInfos
        {
            get 
            { 
                return _userPersonInfos; 
            }
            set
            {
                _userPersonInfos = value;
                this.OnPropertyChanged(() => this.UserPersonInfos);
            }
        }

        private UserPersonInfo _selectPersonInfo = null;
        /// <summary>
        /// 当前选择的人员信息
        /// </summary>
        public UserPersonInfo SelectPersonInfo
        {
            get
            {
                return _selectPersonInfo;
            }
            set
            {
                if (_selectPersonInfo != value)
                {
                    _selectPersonInfo = value;
                }
            }
        }

      

        #endregion
      
        #endregion

        #region 构造函数

        public VmDepartAndPeopleMng()
        {
            curDepartId = AllDepartId;
            IsBatchOperateBtnEnable = false;
            IsBatchStopIrisBtnEnable = false;
            StopBatchAddRecordCommand = new DelegateCommand(new Action(StopBatchAddRecord));
            AddPersonInfoCommand = new DelegateCommand(new Action(AddPersonInfo));
            BatchDeleteCommand = new DelegateCommand(new Action(BatchDelete));
            BatchStopIrisCommand = new DelegateCommand(new Action(BatchStopIris));
            BatchModifyCommand = new DelegateCommand(new Action(BatchModify));
            BatchAddRecordCommand = new DelegateCommand(new Action(BatchAddRecord));
            BatchAddAttendLeaveCommand = new DelegateCommand(new Action(BatchAddAttendLeave));

            //更新权限集合列表
            VmDepartMng.UpdateOperateDepartIdCollection();
            
            #region 初始化操作 by cty
            QueryPersonInfoCommand = new DelegateCommand(new Action(QueryPersonInfo));// by cty

            this.IrisStatus = new List<string>();
            this.IrisStatus.Add("全部");
            this.IrisStatus.Add("停用");
            this.IrisStatus.Add("启用");
            this.IrisStatusSelectedIndex = 0;

            this.IrisRegisters = new List<string>();
            this.IrisRegisters.Add("全部");
            this.IrisRegisters.Add("已注册");
            this.IrisRegisters.Add("未注册");
            this.IrisRegisters.Add("注册单眼");
            this.IrisRegisters.Add("注册双眼");
            this.IrisRegisters.Add("仅注册左眼");
            this.IrisRegisters.Add("仅注册右眼");
            this.IrisRegistersSelectedIndex = 0;
            PersonWorkSn = "";
            PersonName = "";
            

            GetDepartsRia();
    

            _departmentLoadCompleted += (o, e) =>
                {
                    string departName = "全部";
                    string childDepartMode = "不包含";
                    string personName = "";
                    //界面初始化载入全部人员的信息
                    GetPersonInfoTableRia(AllDepartId, personName, personName, departName, departName, childDepartMode);
                };
            #endregion
        }

        #endregion

        #region 界面事件响应
        // 查询按钮
        private void QueryPersonInfo()
        {
            //通过ria向后台查询
            _serviceDomDbAccess = new DomainServiceIriskingAttend();
            GetPersonInfoTableRia(curDepartId, PersonName.Trim(), PersonWorkSn.Trim(),
                  IrisStatus[IrisStatusSelectedIndex].Trim(), IrisRegisters[IrisRegistersSelectedIndex].Trim(), 
                  "包含");
           
        }

        //增加新员工按钮
        private void AddPersonInfo()
        {
            if (VmLogin.GetIsMineApplication())
            {
                var childWnd_AddPerson = new ChildWnd_OperatePerson_Mine(null,
               ChildWndOptionMode.Add, PersonOperateCallback, curDepartId);
                childWnd_AddPerson.Show();
            }
            else
            {
                var childWnd_AddPerson = new ChildWnd_OperatePerson(null,
               ChildWndOptionMode.Add, PersonOperateCallback, curDepartId);
                childWnd_AddPerson.Show();
            }
        }

        //批量删除
        private void BatchDelete()
        {

            List<UserPersonInfo> delete_persons = new List<UserPersonInfo>();
            foreach (var item in UserPersonInfos)
            {
                if (item.isSelected)
                {
                    delete_persons.Add(item);
                  
                }
            }
            if (delete_persons.Count > 0)
            {
                MsgBoxWindow.MsgBox(
                    "请注意，您将进行如下操作\r\n批量删除人员信息！",
                    Dialog.MsgBoxWindow.MsgIcon.Warning,
                    Dialog.MsgBoxWindow.MsgBtns.OKCancel,
                    (e) =>
                    {
                        if (e == MsgBoxWindow.MsgResult.OK)
                        {
                            //通过ria向后台发送请求
                            BatchDeletePersonRia(delete_persons.ToArray());
                        }
                    });

            }
            else
            {
                MsgBoxWindow.MsgBox(
                       "请至少选择一个人员！", Dialog.MsgBoxWindow.MsgIcon.Information, Dialog.MsgBoxWindow.MsgBtns.OK);
            }

        }

        //批量停用虹膜
        private void BatchStopIris()
        {
            BaseViewModelCollection<UserPersonInfo> UserPersonInfos_StopIris = new BaseViewModelCollection<UserPersonInfo>();
            BaseViewModelCollection<PersonStopIrisInfo> PersonInfos_StopIris = new BaseViewModelCollection<PersonStopIrisInfo>();
            foreach (var item in UserPersonInfos)
            {
                if (item.isSelected)
                {
                    UserPersonInfos_StopIris.Add(item);
                }
            }
            if (UserPersonInfos_StopIris.Count > 0)
            {
                View.PeopleView.ChildWnd_BatchStopIris childW = new View.PeopleView.ChildWnd_BatchStopIris(UserPersonInfos_StopIris, true, PersonOperateCallback);
                childW.Show();

            }
            else
            {
                MsgBoxWindow.MsgBox(
                   "请至少选择一个人员！", 
                   Dialog.MsgBoxWindow.MsgIcon.Information,
                   Dialog.MsgBoxWindow.MsgBtns.OK);
        
            }
        }

        //批量修改人员属性
        private void BatchModify()
        {
            BaseViewModelCollection<UserPersonInfo> UserPersonInfos_Selected = new BaseViewModelCollection<UserPersonInfo>();
            foreach (var item in UserPersonInfos)
            {
                if (item.isSelected)
                {
                    UserPersonInfos_Selected.Add(item);
                }
            }
            if (UserPersonInfos_Selected.Count > 0)
            {
                if (VmLogin.GetIsMineApplication())
                {
                    var childW = new ChildWnd_BatchModify_Mine(UserPersonInfos_Selected, PersonOperateCallback);
                    childW.Show();
                }
                else
                {
                    var childW = new View.PeopleView.ChildWnd_BatchModify(UserPersonInfos_Selected, PersonOperateCallback);
                    childW.Show();
                }
              
            }
            else
            {
                MsgBoxWindow.MsgBox(
                    "请至少选择一个人员！", 
                    Dialog.MsgBoxWindow.MsgIcon.Information, 
                    Dialog.MsgBoxWindow.MsgBtns.OK);
              
            }

        }

        //批量多人添加同一条识别记录 
        private void BatchAddRecord()
        {

            //添加识别记录对话框
            AddRecog addRecog = new AddRecog();
            addRecog.btnSave.CommandParameter = addRecog;
            addRecog.Show();
            addRecog.btnSave.Command = new DelegateCommand<AddRecog>(DealPersonRecog);

        }

        /// <summary>
        /// 添加识别记录
        /// </summary>
        private void DealPersonRecog(AddRecog recogDialog)
        {
            UserPersonRecogLog recog = new UserPersonRecogLog();

            if (null != recogDialog.dateBegin.SelectedDate)
            {
                if (null != recogDialog.timeBegin.Value)
                {
                    recog.recog_time = recogDialog.dateBegin.SelectedDate.Value.Date + recogDialog.timeBegin.Value.Value.TimeOfDay;
                }
                else
                {
                    recog.recog_time = recogDialog.dateBegin.SelectedDate.Value.Date;
                }
            }
            else
            {
                MsgBoxWindow.MsgBox("时间为必填项！",
                                MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);
                return;
            }
            recog.at_position = "";
            if (recogDialog.cmbDev.SelectedIndex != 0)
            {
                recog.at_position = recogDialog.VmDevMng.SystemDeviceInfo[recogDialog.cmbDev.SelectedIndex - 1].place;
                recog.device_sn = recogDialog.VmDevMng.SystemDeviceInfo[recogDialog.cmbDev.SelectedIndex - 1].dev_sn;
            }

            Dictionary<int, string> DictDeviceType = PublicMethods.GetDevTypeDictionary(VmLogin.GetIsMineApplication());
            foreach (KeyValuePair<int, string> item in DictDeviceType)
            {
                if (item.Value == (string)(recogDialog.cmbDevType.SelectedItem))
                {
                    recog.dev_type_value = item.Key;
                }
            }

            if (recog.recog_time > DateTime.Now)
            {
                MsgBoxWindow.MsgBox("添加时间大于当前时间，是否确定添加！",
                                MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OKCancel,
                                (Result) =>
                                {
                                    if (Result == MsgBoxWindow.MsgResult.OK)
                                    {
                                        AddRecogInDB(recogDialog, recog);
                                    }
                                });
            }
            else
            {
                AddRecogInDB(recogDialog, recog);
            }

        }

        //操作描述
        StringBuilder descriptionBatchAddRecord = new StringBuilder();
        /// <summary>
        /// 添加识别记录到数据库
        /// </summary>
        /// <param name="recogDialog"></param>
        /// <param name="recog"></param>
        private void AddRecogInDB(AddRecog recogDialog, UserPersonRecogLog recog)
        {
            //首先关掉添加识别记录对话框
            recogDialog.DialogResult = true;
            //显示进度条
            IsBatchAddRecordVisible = Visibility.Visible;
            BatchAddRecordProgress = 0;
            StopBatchAddRecordBtnContent = "取消批量添加识别记录";
            IsStopBatchAddRecordBtnEnable = true;

            descriptionBatchAddRecord = new StringBuilder();

            //选择的人员
            List<int> personIds = new List<int>();
            foreach (var item in UserPersonInfos)
            {
                if (item.isSelected)
                {
                    personIds.Add(item.person_id);
                    descriptionBatchAddRecord.Append(string.Format("姓名：{0}，工号：{1}；", item.person_name, item.work_sn));
                }
            }

            descriptionBatchAddRecord.Append("\r\n");
            descriptionBatchAddRecord.Append(string.Format("识别时间：{0}；", recog.recog_time));

            //构建异步任务执行器
            if (_runnerTask != null)
            {
                _runnerTask.Stop = false;
                _runnerTask = null;
            }


            //估计每一次处理多少个人
            int days = (DateTime.Now - recog.recog_time).Days;
            int personCount = 20; //每次需要重构的人员个数
           
            _runnerTask = new AsyncActionRunner(GetBatchAddRecTasks(personIds.ToArray(), personCount, recog));
            //下面是运行完成后的处理
            _runnerTask.Completed += (obj, a) =>
            {
                
                IsBatchAddRecordVisible = Visibility.Collapsed;
                //如果取消了批量添加识别记录的操作，则不弹出对话框和不写入日志
                if (!_runnerTask.Stop)
                {
                    _runnerTask = null;
                    return;
                }
                _runnerTask = null;
                if (_asyncRuslt == 0)
                {
                    MsgBoxWindow.MsgBox("添加识别记录成功！",
                        MsgBoxWindow.MsgIcon.Succeed, MsgBoxWindow.MsgBtns.OK);
                    //操作员日志 
                    VmOperatorLog.InsertOperatorLog(1, "批量添加识别记录", descriptionBatchAddRecord.ToString(), null);
                   
                }
                else
                {
                    MsgBoxWindow.MsgBox("添加失败！",
                        MsgBoxWindow.MsgIcon.Error, MsgBoxWindow.MsgBtns.OK);
                    //操作员日志 
                    VmOperatorLog.InsertOperatorLog(0, "批量添加识别记录", descriptionBatchAddRecord.ToString(), null);
                }
            };

            //通知进度条改变
            _runnerTask.ProgressChanged += (obj,e) =>
            {
                BatchAddRecordProgress = e.Percent * 100;
            };

           
            //执行异步任务队列
            _runnerTask.Execute();


        }

        //很多人添加同一条请假 by cty
        private void BatchAddAttendLeave()
        {

            //批量添加请假对话框
            ContinueAddAttendLeaveDialog AddAttendLeaveDialog = new ContinueAddAttendLeaveDialog();
            AddAttendLeaveDialog.btnSave.CommandParameter = AddAttendLeaveDialog;
            AddAttendLeaveDialog.Show();
            AddAttendLeaveDialog.btnSave.Command = new DelegateCommand<ContinueAddAttendLeaveDialog>(ContinueAddAttendLeave);

        }
        private void ContinueAddAttendLeave(ContinueAddAttendLeaveDialog dailog)
        {
            bool tag = true;
            try
            {
                UserAttendForLeave userLeave = new UserAttendForLeave();
                if (tag)
                {
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
                    //选择的人员
                    List<UserAttendForLeave> UserLeaveList = new List<UserAttendForLeave>();
                    foreach (var item in UserPersonInfos)
                    {
                        if (item.isSelected)
                        {
                            UserAttendForLeave value = new UserAttendForLeave();

                            value.person_id = item.person_id;
                            value.leave_type_id = userLeave.leave_type_id;
                            value.leave_type_name = userLeave.leave_type_name;
                            value.leave_start_time = userLeave.leave_start_time;
                            value.leave_end_time = userLeave.leave_end_time;
                            value.is_leave_all_day=userLeave.is_leave_all_day;
                            value.memo = userLeave.memo;
                            value.operate_time = userLeave.operate_time;
                            value.operator_name = userLeave.operator_name;
                            value.actual_leave_days = userLeave.actual_leave_days;
                            value.modify_time = userLeave.modify_time;

                            UserLeaveList.Add(value);
                            descriptionBatchAddRecord.Append(string.Format("姓名：{0}，工号：{1}；", item.person_name, item.work_sn));
                        }
                    }
                    descriptionBatchAddRecord.Append("\r\n");
                    descriptionBatchAddRecord.Append(string.Format("\r\n请假时间：" + userLeave.leave_start_time.ToString() + "—" + userLeave.leave_end_time.ToString() + "\r\n请假类型：" + userLeave.leave_type_name + "；"));

                    Action<InvokeOperation<bool>> insetOrUpdateUserAttendForLeaveCallBack = CallBackHandleControl<bool>.OnInvokeErrorCallBack;
                    CallBackHandleControl<bool>.m_sendValue = (o) =>
                    {
                        if (o)
                        { 
                            //添加操作员日志 by cty
                           
                            VmOperatorLog.InsertOperatorLog(1, "添加人员请假", descriptionBatchAddRecord.ToString(), () =>
                            {
                                MsgBoxWindow.MsgBox("添加请假信息成功！",
                                                        MsgBoxWindow.MsgIcon.Succeed, MsgBoxWindow.MsgBtns.OK);
                                ServiceDomDbAcess.ReOpenSever();
                                //从新加载页面

                            });                        
                        }
                        else
                        {
                            //添加操作员日志 by cty
                            VmOperatorLog.InsertOperatorLog(0, "添加人员请假", descriptionBatchAddRecord.ToString(), () =>
                            {
                                MsgBoxWindow.MsgBox( "添加请假信息失败！",
                                                      MsgBoxWindow.MsgIcon.Error, MsgBoxWindow.MsgBtns.OK);
                            });
                        }

                    };

                    ServiceDomDbAcess.GetSever().IrisContinueInsertUserAttendForLeave(UserLeaveList, insetOrUpdateUserAttendForLeaveCallBack, new Object());
                    dailog.DialogResult = true;
                }
            }
            catch (Exception e)
            { 
            }
        }
        /// <summary>
        /// 停止批量添加识别记录,带取消提示框
        /// </summary>
        private void StopBatchAddRecord()
        {

            MsgBoxWindow.MsgBox(
                    "是否取消批量添加识别记录？",
                    Dialog.MsgBoxWindow.MsgIcon.Information,
                    Dialog.MsgBoxWindow.MsgBtns.OKCancel, (e) =>
                    {
                        if (e == MsgBoxWindow.MsgResult.OK)
                        {
                            CancelBatchAddRecord();
                        }
                    });
         
        }

        /// <summary>
        /// 停止批量添加识别记录,不带取消提示框
        /// </summary>
        public void CancelBatchAddRecord()
        {
            if (_runnerTask != null && _runnerTask.Stop)
            {
                _runnerTask.Stop = false;
                StopBatchAddRecordBtnContent = "已取消";
                IsStopBatchAddRecordBtnEnable = false;
                IsBatchAddRecordVisible = Visibility.Collapsed;

                //操作员日志 
                VmOperatorLog.InsertOperatorLog(1, "取消批量添加识别记录", descriptionBatchAddRecord.ToString(), null);

            }
        }
        #endregion

        private int _asyncRuslt;   //异步任务的操作返回信息
        /// <summary>
        /// 构造批量修改人员属性的异步任务队列 
        /// </summary>
        /// <returns></returns>
        private List<IAsyncAction> GetBatchAddRecTasks(int[] personIds, int countPerTask,
            UserPersonRecogLog recog)      
        {
            List<IAsyncAction> tasks = new List<IAsyncAction>();

            for (int i = 0; i < personIds.Length; i += countPerTask)
            {
                int[] curPersonIds;
                //构造一次任务的人数
                if (i + countPerTask - 1 < personIds.Length)
                {
                    curPersonIds = new int[countPerTask];
                    for (int j = 0; j < countPerTask; j++)
                    {
                        curPersonIds[j] = personIds[i + j];
                    }
                }
                else
                {
                    curPersonIds = new int[personIds.Length - i];
                    for (int j = 0; j < curPersonIds.Length; j++)
                    {
                        curPersonIds[j] = personIds[i + j];
                    }
                }


                ///创建新任务
                var taskTemp = new AsyncAction("BatchAddRecTask" + 1);

                //设置任务工作内容
                taskTemp.SetAction(() =>
                {

                    Action<InvokeOperation<int>> insertUserPersonRecogCallBack = CallBackHandleControl<int>.OnInvokeErrorCallBack;

                    CallBackHandleControl<int>.m_sendValue = (o) =>
                    {
                       
                        _asyncRuslt = o;
                        //表示当前任务完成了,执行下一个任务
                        taskTemp.OnCompleted();
                       
                    };
                    ServiceDomDbAcess.GetSever().IrisBatchInsertRecogLog(curPersonIds, recog, insertUserPersonRecogCallBack, new Object());

                 

                }, true);

                tasks.Add(taskTemp);

            }

            return tasks;
        }



        #region 回调函数
        
        /// <summary>
        /// 人员操作子窗口返回的回调函数
        /// </summary>
        /// <param name="DialogResult">子窗口是否点击确认关闭</param>
        private void PersonOperateCallback(bool? DialogResult)
        {
            if (DialogResult.HasValue && DialogResult.Value)
            {
                //重新查询数据库
                _serviceDomDbAccess = new DomainServiceIriskingAttend();
                GetPersonInfoTableRia(curDepartId, PersonName.Trim(), PersonWorkSn.Trim(),
                  IrisStatus[IrisStatusSelectedIndex].Trim(), IrisRegisters[IrisRegistersSelectedIndex].Trim(), "包含");
            }
        }

        /// <summary>
        /// 部门操作子窗口返回的回调函数
        /// </summary>
        /// <param name="DialogResult">子窗口是否点击确认关闭</param>
        private void DepartOperateCallback(bool? DialogResult)
        {
            if (DialogResult.HasValue && DialogResult.Value)
            {
                //重新查询数据库
                _serviceDomDbAccess = new DomainServiceIriskingAttend();
                GetDepartsRia();
               
            }
        }

        #endregion

        #region  通过ria连接后台，数据库相关操作
        /// <summary>
        /// 异步获取数据库中数据
        /// 获取部门名字
        /// </summary>
        private void GetDepartsRia()
        {
            try
            {
                WaitingDialog.ShowWaiting();

                EntityQuery<UserDepartInfo> list = _serviceDomDbAccess.GetDepartsInfoQuery();
                ///回调异常类
                Action<LoadOperation<UserDepartInfo>> actionCallBack =ErrorHandle<UserDepartInfo>.OnLoadErrorCallBack;
                ///异步事件
                LoadOperation<UserDepartInfo> lo = this._serviceDomDbAccess.Load(list, actionCallBack, null);

                lo.Completed += delegate
                {
                    //获取展开节点的id
                    HashSet<int> expandedIdCollection  = new HashSet<int>();
                    GetExpandedDepartNode(DepartAll, expandedIdCollection);

                    Departs = new BaseViewModelCollection<UserDepartInfo>();
                    //异步获取数据
                    foreach (UserDepartInfo ar in lo.Entities)
                    {
                        Departs.Add(ar);
                    }
                    //通知前台人员添加按钮的enable属性改变
                    this.OnPropertyChanged(() => this.IsAddPersonEnable);
                  
                    //存储为树形结构
                    _departTreeData = new TreeNode<UserDepartInfo>();
                    TreeData = new BaseViewModelCollection<TreeNode<UserDepartInfo>>();
                    
                    //添加总部门
                    DepartAll = new TreeNode<UserDepartInfo>();
                    DepartAll.NodeValue = new UserDepartInfo();
                    DepartAll.NodeValue.depart_id = AllDepartId; //
                    DepartAll.Level = 0;
                    DepartAll.IsChecked = true;
                    DepartAll.IsOpen = true;
                    DepartAll.GetNodeNameDelegate += () =>
                    {
                        return "全部";
                    };
                    DepartAll.Index = TreeData.Count;
                    TreeData.Add(DepartAll);

                    //创建部门信息树 
                    CreatDepartTree(-1, DepartAll.Children, 0, expandedIdCollection);
                    
                    //如果不是超级用户，则要进行权限限制
                    if (!VmLogin.GetIsSuperUser())
                    {
                        foreach (var item in DepartAll.Children)
                        {
                            //部门权限管理
                            ManagerDepartPermission(item);
                            //部门树可见性管理
                            SetTreeNodeVisibility(item);
                        }  
                    }


                    //重新布局departTree的竖直拖动栏
                    if (ScrollBarVerticalOffsetLayout != null)
                    {
                        App.Current.RootVisual.Dispatcher.BeginInvoke(ScrollBarVerticalOffsetLayout);
                    }
                    
                    if (_departmentLoadCompleted != null)
                    {
                        _departmentLoadCompleted(null, null);
                        _departmentLoadCompleted = null;
                    }
                    else
                    {
                        WaitingDialog.HideWaiting();
                    }

                    if (LoadCompletedEvent != null)
                    {
                        LoadCompletedEvent(this, null);
                    }

                

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
        /// 异步查询数据库中数据
        /// 查询人员信息表
        /// </summary>
        /// </summary>
        /// <param name="departID">部门id -1代表全部部门</param>
        /// <param name="person_Name">人员名称</param>
        /// <param name="person_WorkSn">工号</param>
        /// <param name="iris_Status">虹膜启用状态</param>
        /// <param name="iris_Register">虹膜注册状态</param>
        /// <param name="child_DepartMode">是否包含子部门</param>
        private void GetPersonInfoTableRia(int departID, string personName, string personWorkSn,
            string irisStatus, string irisRegister, string childDepartMode)
        {
            try
            {
                WaitingDialog.ShowWaiting();
                //ServiceDomDbAcess.ReOpenSever();
                EntityQuery<UserPersonInfo> list = _serviceDomDbAccess.GetPersonsInfoTableQuery(departID, personName,
                    personWorkSn, irisStatus, irisRegister, childDepartMode);
                ///回调异常类
                Action<LoadOperation<UserPersonInfo>> actionCallBack = new Action<LoadOperation<UserPersonInfo>>(ErrorHandle<UserPersonInfo>.OnLoadErrorCallBack);
                ///异步事件
                LoadOperation<UserPersonInfo> lo = this._serviceDomDbAccess.Load(list, actionCallBack, null);
                lo.Completed += delegate
                {
                  
                    UserPersonInfos = new BaseViewModelCollection<UserPersonInfo>();

                   // UserPersonInfosBinding.Clear();
                    //异步获取数据
                    foreach (UserPersonInfo ar in lo.Entities)
                    {
                        ar.isSelected = false;
                        if (ar.depart_id == -1 || VmDepartMng.OperateDepartIdCollection.Contains(ar.depart_id) )
                        {
                            ar.index = UserPersonInfos.Count + 1;
                            UserPersonInfos.Add(ar);
                        }
                        
                    }

                    this.IsBatchStopIrisBtnEnable = CheckMoreThanOneSelected();
                    this.IsBatchOperateBtnEnable = CheckIsAnyOneSelected();
                    this.MarkObj.Selected = CheckIsAllSelected();

                   
                 
                    


                    WaitingDialog.HideWaiting();
                    
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
        /// 批量删除人员
        /// </summary>
        /// <param name="personIds">人员id数组</param>
        private void BatchDeletePersonRia(UserPersonInfo[] persons)
        {
            //描述
            StringBuilder description = new StringBuilder();
            List<int> personIds = new List<int>();
            foreach (var item in persons)
            {
                description.Append(string.Format("姓名：{0}，工号：{1}；", item.person_name, item.work_sn));
                personIds.Add(item.person_id);
            }
            description.Append("\r\n");

            try
            {
                Action<InvokeOperation<OptionInfo>> callBack = CallBackHandleControl<OptionInfo>.OnInvokeErrorCallBack;
                CallBackHandleControl<OptionInfo>.m_sendValue = (op) =>
                {
                    VmOperatorLog.CompleteCallBack completeCallBack = () =>
                        {
                            QueryPersonInfo();
                        };

                    //异步获取数据
                    if (!op.isSuccess)
                    {
                        MsgBoxWindow.MsgBox(
                          op.option_info + "！",
                          Dialog.MsgBoxWindow.MsgIcon.Error, Dialog.MsgBoxWindow.MsgBtns.OK);
                        VmOperatorLog.InsertOperatorLog(0, "删除员工", description.ToString() + op.option_info, completeCallBack);
                    }
                    else
                    {
                        if (!op.isNotifySuccess)
                        {
                            MsgBoxWindow.MsgBox(
                             op.option_info + "！",
                             Dialog.MsgBoxWindow.MsgIcon.Warning, Dialog.MsgBoxWindow.MsgBtns.OK);
                            VmOperatorLog.InsertOperatorLog(1, "删除员工", description.ToString() + op.option_info, completeCallBack);
                        }
                        else
                        {
                            MsgBoxWindow.MsgBox(
                           string.Format("操作成功，共有{0}个人被删除！", persons.Length),
                           Dialog.MsgBoxWindow.MsgIcon.Succeed, Dialog.MsgBoxWindow.MsgBtns.OK);
                            VmOperatorLog.InsertOperatorLog(1, "删除员工", description.ToString() , completeCallBack);
                        }
                    }


                    WaitingDialog.HideWaiting();
                
                    
                };
                WaitingDialog.ShowWaiting();
                _serviceDomDbAccess.BatchDeletePerson(personIds.ToArray(), callBack, null);
            }
            catch (Exception e)
            {
                WaitingDialog.HideWaiting();
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }

        /// <summary>
        /// 批量删除指定部门
        /// </summary>
        /// <param name="IDs"></param>
        private void BatchDeleteDepartRia(UserDepartInfo[] infos)
        {
            List<string> IDs_str = new List<string>();
            StringBuilder description = new StringBuilder();
            foreach (var item in infos)
            {
                IDs_str.Add(PublicMethods.ToString(item.depart_id));
                description.Append(string.Format("部门名称：{0}；", item.depart_name));
            }

            description.Append("\r\n");
            
            try
            {
                WaitingDialog.ShowWaiting();
                _serviceDomDbAccess = new DomainServiceIriskingAttend();
                EntityQuery<OptionInfo> list = _serviceDomDbAccess.DeleteDepartQuery(IDs_str.ToArray());

                ///回调异常类
                Action<LoadOperation<OptionInfo>> actionCallBack = new Action<LoadOperation<OptionInfo>>(ErrorHandle<OptionInfo>.OnLoadErrorCallBack);
                ///异步事件
                LoadOperation<OptionInfo> lo = this._serviceDomDbAccess.Load(list, actionCallBack, null);
                lo.Completed += delegate
                {
                    foreach (OptionInfo item in lo.Entities)
                    {
                        if (!item.isSuccess)
                        {
                            VmOperatorLog.InsertOperatorLog(0, "删除部门", description.ToString() + item.option_info, null);
                            MsgBoxWindow.MsgBox(
                               item.option_info + "！",
                               Dialog.MsgBoxWindow.MsgIcon.Error, Dialog.MsgBoxWindow.MsgBtns.OK);
                        }
                        else
                        {
                            if (!item.isNotifySuccess)
                            {
                                VmOperatorLog.InsertOperatorLog(1, "删除部门", description.ToString() + item.option_info, null);
                                MsgBoxWindow.MsgBox(
                                     item.option_info + "！",
                                     Dialog.MsgBoxWindow.MsgIcon.Warning, Dialog.MsgBoxWindow.MsgBtns.OK);
                            }
                            else
                            {
                                VmOperatorLog.InsertOperatorLog(1, "删除部门", description.ToString() , null);
                                MsgBoxWindow.MsgBox(
                                   item.option_info + "！",
                                   Dialog.MsgBoxWindow.MsgIcon.Succeed, Dialog.MsgBoxWindow.MsgBtns.OK);
                            }
                        }
                        break;
                    }

                    //重新查询数据库
                    _serviceDomDbAccess = new DomainServiceIriskingAttend();
                    GetDepartsRia();
                    WaitingDialog.HideWaiting();
                };
            }
            catch (Exception e)
            {
                WaitingDialog.HideWaiting();
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }

        ///// <summary>
        ///// 添加部门权限
        ///// </summary>
        ///// <param name="departId">部门id</param>
        ///// <param name="addDepartPotenceRiaCompleted">异步回调</param>
        //private void AddDepartPotenceRia(int departId,  Action addDepartPotenceRiaCompleted = null)
        //{
        //    //异步回调函数，返回后台操作成功或者失败的标志
        //    Action<InvokeOperation<bool>> callBack = CallBackHandleControl<bool>.OnInvokeErrorCallBack;
        //    CallBackHandleControl<bool>.m_sendValue = (isSucceed) =>
        //    {
        //        if (isSucceed)
        //        {
        //            //更新部门权限列表
        //            VmLogin.UpdateOperatorDepartPotence(() =>
        //            {
        //                VmDepartMng.UpdateOperateDepartIdCollection();
                       
        //                if (addDepartPotenceRiaCompleted != null)
        //                {
        //                    addDepartPotenceRiaCompleted();
        //                }

        //            });
        //        }
        //        else
        //        {
        //            MsgBoxWindow.MsgBox(
        //                  "为新增部门添加权限失败!",
        //                  MsgBoxWindow.MsgIcon.Error,
        //                  MsgBoxWindow.MsgBtns.OK);
        //            WaitingDialog.HideWaiting();
        //        }

        //    };

        //    _serviceDomDbAccess = new DomainServiceIriskingAttend();
        //    _serviceDomDbAccess.AddDepartPotence(VmLogin.GetUserName(), departId, callBack, null);
        //}


        #endregion

        #region 私有功能函数


        /// <summary>
        /// 将父部门当做根节点
        /// 创建部门树
        /// 一般用递归方法调用
        /// </summary>
        /// <param name="PID">父部门id</param>
        /// <param name="tNode">根节点</param>
        private void CreatDepartTree(long parentDepartID, BaseViewModelCollection<TreeNode<UserDepartInfo>> treeNode,
            int level, HashSet<int> expandedIdCollection)
        {
            IEnumerable<UserDepartInfo> departsTmp = this.Departs.Where<UserDepartInfo>((info) =>
            {
                return info.parent_depart_id == parentDepartID;
            });

            if (departsTmp == null)
            {
                return;
            }
            level++;

            foreach (var item in departsTmp)
            {
                TreeNode<UserDepartInfo> node = new TreeNode<UserDepartInfo>();
                node.NodeValue = item;
                node.Level = level;
                node.GetNodeNameDelegate += () =>
                {
                    return ((UserDepartInfo)node.NodeValue).depart_name;
                };

                node.Children = new BaseViewModelCollection<TreeNode<UserDepartInfo>>();
                node.Index = treeNode.Count;
                //节点保持上一次的状态
                if (expandedIdCollection.Contains(node.NodeValue.depart_id))
                {
                    node.IsOpen = true;
                }


                treeNode.Add(node);
                CreatDepartTree(item.depart_id, node.Children, level, expandedIdCollection);
            }
        }

        /// <summary>
        /// 管理部门权限
        /// 规则如下：
        /// 1. 如果父部门有权限，则其子部门有权限
        /// 2. 如果父部门是总部门，则该部门权限由权限列表中的id给出
        /// </summary>
        /// <param name="treeNode">树节点</param>
        /// <param name="isParentNodeHasPermission">父节点是否有权限</param>
        private void ManagerDepartPermission(TreeNode<UserDepartInfo> treeNode)
        {
            //当前节点的权限,由部门权限列表决定
            bool curPermission = VmDepartMng.OperateDepartIdCollection.Contains(treeNode.NodeValue.depart_id);
            treeNode.IsEnable = curPermission;
            foreach (var item in treeNode.Children)
            {
                ManagerDepartPermission(item);
            }

        }

        /// <summary>
        /// 管理部门树的可见性
        /// 规则如下：
        /// 1. 如果该节点是叶子节点，则它的可见性由IsEnable属性决定
        /// 2. 否则，该节点的可见性 由 它的子节点是否包含可见节点来决定
        /// </summary>
        /// <param name="treeNode">树节点</param>
        /// <returns>包含几个可见子节点</returns>
        private int SetTreeNodeVisibility(TreeNode<UserDepartInfo> treeNode)
        {
            if (treeNode.Children == null || treeNode.Children.Count == 0)
            {
                treeNode.Visibility = treeNode.IsEnable ? Visibility.Visible : Visibility.Collapsed;
                return treeNode.IsEnable ? 1 : 0;
            }
            else
            {
               
                int count = 0;
                foreach (var item in treeNode.Children)
                {
                    count += SetTreeNodeVisibility(item);
                }
                //如果子部门中有可见的节点，则该部门可见
                if (count > 0)
                {
                    treeNode.Visibility = Visibility.Visible;
                }
                else
                {
                    treeNode.Visibility = Visibility.Collapsed;
                }
                return count;
            }

        }


        /// <summary>
        /// 获取展开的部门节点集合
        /// </summary>
        /// <returns></returns>
        private void GetExpandedDepartNode(TreeNode<UserDepartInfo> node ,HashSet<int> collection)
        {
            if (node == null)
            {
                return;
            }

            if (node.IsOpen)
            {
                collection.Add(node.NodeValue.depart_id);
            }
            foreach (var item in node.Children)
            {
                GetExpandedDepartNode(item, collection);
            }
            
        }



        /// <summary>
        /// 检查是否有多于一个数量的item被选中
        /// </summary>
        /// <returns></returns>
        private bool CheckMoreThanOneSelected()
        {
            int count = 0;
            foreach (var item in UserPersonInfos)
            {
                if (item.isSelected)
                {
                    count++;
                }
            }
            return count > 1 ? true : false;
        }

        /// <summary>
        /// 检查是否有Item被选中
        /// </summary>
        private bool CheckIsAnyOneSelected()
        {
            
            foreach (var item in UserPersonInfos)
            {
                if (item.isSelected)
                {
                    return true;
                }
            }
            return false;
          
            
        }

        /// <summary>
        /// 检查Item是否全部被选中
        /// </summary>
        private bool CheckIsAllSelected()
        {
            if (UserPersonInfos.Count == 0)
            {
                return false;
            }
            foreach (var item in UserPersonInfos)
            {
                if (!item.isSelected)
                {
                    return false;
                }
            }
            return true;
            
        }

        

        #endregion

        #region 扩展功能、给view层的接口

        /// <summary>
        /// 批量调离人员
        /// </summary>
        /// <param name="persons">待调离的人员</param>
        /// <param name="targetDepartId">目标部门id</param>
        public void BatchTransferPersonsRia(List<UserPersonInfo> persons,UserDepartInfo targetDepart)
        {
            if (persons.Count == 0)
            {
                return;
            }
            int targetDepartId =targetDepart.depart_id;

            ////调离成功，原人员列表重新排序
            //for (int i = persons[0].index - 1; i < UserPersonInfos.Count; i++)
            //{
            //    UserPersonInfos[i].index -= persons.Count;
            //}

            //获取描述
            StringBuilder description = new StringBuilder();
            foreach (var item in persons)
            {
                description.Append(string.Format("姓名：{0}，工号：{1}；", item.person_name,item.work_sn));
            }
            if (persons.Count == 1)
            {
                description.Append(string.Format("\r\n所属部门：{0}->{1}；\r\n", persons[0].depart_name, targetDepart.depart_name));
            }
            else
            {
                description.Append(string.Format("\r\n所属部门：->{1}；\r\n", targetDepart.depart_name));
            }
            
           
            try
            {
                //异步回调函数，返回后台操作成功或者失败的标志
                Action<InvokeOperation<OptionInfo>> errorCallBack = CallBackHandleControl<OptionInfo>.OnInvokeErrorCallBack;
                CallBackHandleControl<OptionInfo>.m_sendValue = (o) =>
                {
                    if (o.isSuccess)
                    {
                        VmOperatorLog.InsertOperatorLog(1, "调离人员", description.ToString() , () =>
                            {
                                //调离人员成功刷新当前部门的人员列表
                                _serviceDomDbAccess = new DomainServiceIriskingAttend();
                                GetPersonInfoTableRia(curDepartId, "", "", "全部", "全部", "包含");
                            });

                       
                        
                        //屏蔽掉操作成功框
                        //Dialog.MsgBoxWindow.MsgBox("操作成功", Dialog.MsgBoxWindow.MsgIcon.Succeed, Dialog.MsgBoxWindow.MsgBtns.OK);
                    }
                    else
                    {
                        Dialog.MsgBoxWindow.MsgBox(o.option_info + "！", Dialog.MsgBoxWindow.MsgIcon.Warning, Dialog.MsgBoxWindow.MsgBtns.OK);
                        WaitingDialog.HideWaiting();
                        VmOperatorLog.InsertOperatorLog(0, "调离人员", description.ToString() + o.option_info, null);
                    }
                  
                };

                List<int> personIds = new List<int>();
                foreach (var item in persons)
                {
                    personIds.Add(item.person_id);
                }
                WaitingDialog.ShowWaiting();
                _serviceDomDbAccess = new DomainServiceIriskingAttend();
                _serviceDomDbAccess.BatchModifyPersons(personIds.ToArray(), targetDepartId, -1, -1,
                    -1, -1, errorCallBack, null);

            }
            catch (Exception e)
            {
                WaitingDialog.HideWaiting();
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }

        /// <summary>
        /// 调离部门
        /// 使之成为目标部门的子部门
        /// </summary>
        /// <param name="depart">待调离的部门</param>
        /// <param name="targetDepartId">目标部门id</param>
        public void TransferDepartRia(TreeNode<UserDepartInfo> depart,UserDepartInfo targetDepart)
        {
            if (depart == null || depart.NodeValue == null )
            {
                return;
            }
            int targetDepartId = targetDepart.depart_id;
            string targetDepartIdStr = targetDepartId == -1 ? "null" : targetDepartId.ToString();

            //获取调离部门操作的描述
            string descrption = string.Format("部门名称：{0}；\r\n父部门：{1}->{2}；", depart.NodeValue.depart_name,
                depart.NodeValue.parent_depart_name, targetDepart.depart_name);

            try
            {
                //异步回调函数，返回后台操作成功或者失败的标志
                Action<InvokeOperation<OptionInfo>> errorCallBack = CallBackHandleControl<OptionInfo>.OnInvokeErrorCallBack;
                CallBackHandleControl<OptionInfo>.m_sendValue = (o) =>
                {
                    if (o.isSuccess && o.isNotifySuccess)
                    {
                        //写入操作员日志
                        VmOperatorLog.InsertOperatorLog(1, "调离部门", descrption , () =>
                            {
                                //刷新部门
                                _serviceDomDbAccess = new DomainServiceIriskingAttend();
                                GetDepartsRia();
                            });
                    
                    }
                    else
                    {
                        if (!o.isSuccess)
                        {
                            Dialog.MsgBoxWindow.MsgBox(o.option_info + "！",
                              Dialog.MsgBoxWindow.MsgIcon.Error,
                              Dialog.MsgBoxWindow.MsgBtns.OK);
                            //写入操作员日志
                            VmOperatorLog.InsertOperatorLog(0, "调离部门", descrption + o.option_info, null);
                        }
                        else if (!o.isNotifySuccess)
                        {
                            Dialog.MsgBoxWindow.MsgBox(o.option_info + "！",
                              Dialog.MsgBoxWindow.MsgIcon.Warning,
                              Dialog.MsgBoxWindow.MsgBtns.OK);
                            //写入操作员日志
                            VmOperatorLog.InsertOperatorLog(1, "调离部门", descrption + o.option_info, () =>
                            {
                                //刷新部门
                                _serviceDomDbAccess = new DomainServiceIriskingAttend();
                                GetDepartsRia();
                            });
                          
                        }
                        WaitingDialog.HideWaiting();
                    }
                   
                };


                WaitingDialog.ShowWaiting();
                _serviceDomDbAccess.ModifyDepart(depart.NodeValue.depart_id.ToString(),
                   PublicMethods.ToString(depart.NodeValue.depart_name),
                   PublicMethods.ToString(depart.NodeValue.depart_sn), targetDepartIdStr,
                   PublicMethods.ToString(depart.NodeValue.contact_phone),
                   PublicMethods.ToString(depart.NodeValue.memo), errorCallBack, null);
             

            }
            catch (Exception e)
            {
                WaitingDialog.HideWaiting();
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
           
        }

        /// <summary>
        /// 将待拖拽的人员从人员列表中移除
        /// </summary>
        /// <param name="readyToDragPersons">待拖拽的人员</param>
        public void RemoveDragPersons(List<UserPersonInfo> readyToDragPersons)
        {
            for (int i = UserPersonInfos.Count-1; i >=0 ; i--)
            {
                foreach (var dragItem in readyToDragPersons)
                {
                    if (dragItem.Equals(UserPersonInfos[i]))
                    {
                        UserPersonInfos.Remove(dragItem);
                    }
                }
            }
          
        }

     
        /// <summary>
        /// 将待拖拽部门从部门树中移除
        /// 将其visibility属性设为折叠的
        /// 并找到该部门的父部门，
        /// </summary>
        /// <param name="readyToDragDeparts"></param>
        public void RemoveDragDeparts(TreeNode<UserDepartInfo> readyToDragDeparts)
        {
            //_readyToDragDepartParent = FindParentNode<UserDepartInfo>(_departTreeData, readyToDragDeparts);
            readyToDragDeparts.Visibility = Visibility.Collapsed;
            if (ScrollBarVerticalOffsetLayout != null)
            {
                App.Current.RootVisual.Dispatcher.BeginInvoke(ScrollBarVerticalOffsetLayout);
            } 
        }
       
        //待拖拽部门的父部门
        //private TreeNode<UserDepartInfo> _readyToDragDepartParent = null;
        
        /// <summary>
        /// 寻找指定部门的父部门
        /// </summary>
        /// <param name="treeNode">部门树</param>
        /// <param name="removeValue">某部门</param>
        /// <returns></returns>
        private TreeNode<T> FindParentNode<T>(TreeNode<T> treeNode, TreeNode<T> removeValue)
        {
            TreeNode<T> res = null;
            if (treeNode != null && treeNode.Children != null && treeNode.Children.Count > 0)
            {
                foreach (var item in treeNode.Children)
                {
                    if (item.Equals(removeValue))
                    {
                        return item;
                    }
                    else
                    {
                        res = FindParentNode(item, removeValue);
                        if (res!=null)
                        {
                            return res;
                        }
                    }
                }
            }


            return res;
        }


        /// <summary>
        /// 拖拽失败 恢复拖拽人员
        /// </summary>
        /// <param name="readyToDragPersons">拖拽人员</param>
        public void RevertDragPersons(List<UserPersonInfo> readyToDragPersons)
        {

            foreach (var dragItem in readyToDragPersons)
            {
                if (dragItem.index < 0)
                {
                    continue;
                }
                else if (dragItem.index >= UserPersonInfos.Count )
                {
                    UserPersonInfos.Add(dragItem);
                }
                else
                {
                    UserPersonInfos.Insert(dragItem.index, dragItem);
                }
            }
       
        }

        /// <summary>
        /// 拖拽失败 恢复拖拽部门
        /// </summary>
        /// <param name="readyToDragDeparts">拖拽部门</param>
        public void RevertDragDeparts(TreeNode<UserDepartInfo> readyToDragDeparts)
        {
            if (readyToDragDeparts != null)
            {
                readyToDragDeparts.IsChecked = true;
                readyToDragDeparts.Visibility = Visibility.Visible;
                //_readyToDragDepartParent.Children.Insert(readyToDragDeparts.Index, readyToDragDeparts);
                //_readyToDragDepartParent = null;
                if (ScrollBarVerticalOffsetLayout != null)
                {
                    App.Current.RootVisual.Dispatcher.BeginInvoke(ScrollBarVerticalOffsetLayout);
                } 
            }
           
        }

        /// <summary>
        /// 选中全部人员或者取消选中
        /// </summary>
        /// <param name="IsChecked">是否全选标志</param>
        public void SelectAllPerson(bool? IsChecked)
        {
            if (IsChecked.Value)
            {
                foreach (var item in UserPersonInfos)
                {
                    item.isSelected = true;
                }
            }
            else
            {
                foreach (var item in UserPersonInfos)
                {
                    item.isSelected = false;
                }
            }

            this.IsBatchStopIrisBtnEnable = CheckMoreThanOneSelected();
            this.IsBatchOperateBtnEnable = CheckIsAnyOneSelected();
            this.MarkObj.Selected = CheckIsAllSelected();
        }

        /// <summary>
        /// 按需选择datagrid的Items
        /// </summary>
        /// <param name="sender">datagrid对象</param>
        public void SelectItems(UserPersonInfo personInfo)
        {
            personInfo.isSelected = !personInfo.isSelected;
            this.IsBatchStopIrisBtnEnable = CheckMoreThanOneSelected();
            this.IsBatchOperateBtnEnable = CheckIsAnyOneSelected();
            this.MarkObj.Selected = CheckIsAllSelected();
        }

        /// <summary>
        /// 查看人员信息 by cty
        /// </summary>
        /// <param name="personId">人员id</param>
        public void CheckPerson(UserPersonInfo personInfo)
        {
            if (VmLogin.GetIsMineApplication())
            {
                var childWnd_CheckPerson = new ChildWnd_OperatePerson_Mine(personInfo, ChildWndOptionMode.Check, PersonOperateCallback);
                childWnd_CheckPerson.Show();
            }
            else
            {
                var childWnd_CheckPerson = new ChildWnd_OperatePerson(personInfo, ChildWndOptionMode.Check, PersonOperateCallback);
                childWnd_CheckPerson.Show();
            }
        }

        /// <summary>
        /// 修改人员信息 by cty
        /// </summary>
        /// <param name="personId"></param>
        public void ModifyPerson(UserPersonInfo personInfo)
        {
            if (VmLogin.GetIsMineApplication())
            {
                var childWnd_CheckPerson = new ChildWnd_OperatePerson_Mine(personInfo, ChildWndOptionMode.Modify, PersonOperateCallback);
                childWnd_CheckPerson.Show();
            }
            else
            {
                var childWnd_CheckPerson = new ChildWnd_OperatePerson(personInfo, ChildWndOptionMode.Modify, PersonOperateCallback);
                childWnd_CheckPerson.Show();
            }
            
        }

        /// <summary>
        /// 删除人员信息 
        /// </summary>
        /// <param name="personId"></param>
        public void DeletePerson(UserPersonInfo info)
        {
            MsgBoxWindow.MsgBox(string.Format("请注意，您将进行如下操作:\r\n删除人员【{0}】！", info.person_name),
                MsgBoxWindow.MsgIcon.Warning, MsgBoxWindow.MsgBtns.OKCancel, (result) =>
                {
                    if (result == MsgBoxWindow.MsgResult.OK)
                    {
                        UserPersonInfo[] persons = new UserPersonInfo[1];
                        persons[0] = info;
                        BatchDeletePersonRia(persons);
                    }
                });
           
        }

        /// <summary>
        /// 查看识别记录 by cty || 优化代码 by wz
        /// </summary>
        /// <param name="personInfo"></param>
        public void Record(UserPersonInfo personInfo)
        {           
            var chidlWnd = new RecogInfo(personInfo);
            chidlWnd.Show();
        }

        /// <summary>
        /// 停用虹膜 by cty || 优化代码 by wz
        /// </summary>
        /// <param name="personInfo"></param>
        public void StopIris(UserPersonInfo personInfo)
        {
            BaseViewModelCollection<UserPersonInfo> UserPersonInfos_StopIris = new BaseViewModelCollection<UserPersonInfo>();
            if (personInfo != null)
            {
                UserPersonInfos_StopIris.Add(personInfo);
            }

            if (UserPersonInfos_StopIris.Count > 0)
            {
                View.PeopleView.ChildWnd_BatchStopIris childW = new View.PeopleView.ChildWnd_BatchStopIris(UserPersonInfos_StopIris, false, PersonOperateCallback);
                childW.Show();
            }
            else
            {
                MsgBoxWindow.MsgBox(
                          "请至少选择一个人员！",
                          Dialog.MsgBoxWindow.MsgIcon.Information, Dialog.MsgBoxWindow.MsgBtns.OK);
            }

        }

        /// <summary>
        /// 当前部门改变
        /// </summary>
        /// <param name="targetDepartName">改变后的部门id -1代表全部部门</param>
        public void ChangeCurDepart(int targetDepartId)
        {
            _serviceDomDbAccess = new DomainServiceIriskingAttend();
            GetPersonInfoTableRia(targetDepartId, "", "", "全部", "全部", "包含");
            curDepartId = targetDepartId;
        }

        /// <summary>
        /// 添加部门
        /// </summary>
        /// <param name="departInfo">部门信息</param>
        public void AddDepart(UserDepartInfo departInfo)
        {
            ChildWnd_OperateDepart cw = new ChildWnd_OperateDepart(departInfo, ChildWndOptionMode.Add, DepartOperateCallback);
            cw.Show();
        }

        /// <summary>
        /// 修改部门
        /// </summary>
        /// <param name="departInfo">部门信息</param>
        public void ModifyDepart(UserDepartInfo departInfo)
        {
            ChildWnd_OperateDepart cw = new ChildWnd_OperateDepart(departInfo, ChildWndOptionMode.Modify, DepartOperateCallback);
            cw.Show();
        }

        /// <summary>
        /// 删除部门
        /// </summary>
        /// <param name="departInfo">部门信息</param>
        public void DeleteDepart(UserDepartInfo departInfo)
        {
            MsgBoxWindow.MsgBox(string.Format("请注意，您将进行如下操作:\r\n删除部门【{0}】！", departInfo.depart_name),
              MsgBoxWindow.MsgIcon.Warning, MsgBoxWindow.MsgBtns.OKCancel, (result) =>
              {
                  if (result == MsgBoxWindow.MsgResult.OK)
                  {
                      UserDepartInfo[] infos = new UserDepartInfo[1];
                      infos[0] = departInfo;
                      BatchDeleteDepartRia(infos);
                  }
              });
        }

   
        #endregion

    }

 

}
