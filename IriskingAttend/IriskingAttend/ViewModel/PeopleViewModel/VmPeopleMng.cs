///*************************************************************************
//** 文件名:   VmPeopleMng.cs
//×× 主要类:   VmPeopleMng
//**  
//** Copyright (c) 中科虹霸有限公司
//** 创建人:   cty
//** 日  期:   2013-6-14
//** 修改人:   wz 代码优化
//** 日  期:   2013-7-24
//** 描  述:   VmPeopleMng类,人员信息的显示及根据查询条件显示
//**
//** 版  本:   1.0.0
//** 备  注:  命名及代码编写遵守C#编码规范
//**
// * ***********************************************************************/
//using System;
//using System.Net;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Documents;
//using System.Windows.Ink;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Linq;
//using System.Windows.Media.Animation;
//using System.Windows.Shapes;
//using Microsoft.Practices.Prism.Commands;
//using System.Collections.Generic;
//using System.ServiceModel.DomainServices.Client;
//using Irisking.Web.DataModel;
//using EDatabaseError;
//using IriskingAttend.Web;
//using IriskingAttend.Common;
//using System.IO.IsolatedStorage;
//using System.ComponentModel.Composition;
//using System.ComponentModel.Composition.Hosting;
//using System.Reflection;
//using IriskingAttend.View.PeopleView;
//using IriskingAttend.Dialog;
//using GalaSoft.MvvmLight.Command;
//using MvvmLightCommand.SL4.TriggerActions;
//using System.Windows.Controls.Primitives;
//using System.ComponentModel;
//using IriskingAttend.View;
//using IriskingAttend.BehaviorSelf;



//namespace IriskingAttend.ViewModel.PeopleViewModel
//{
//    public class VmPeopleMng : BaseViewModel
//    {

//        #region 字段声明
        
//        /// <summary>
//        /// 与服务声明
//        /// </summary>
//        private DomainServiceIriskingAttend serviceDomDbAccess = new DomainServiceIriskingAttend();

//        /// <summary>
//        /// depart载入完毕事件
//        /// </summary>
//        private event EventHandler departmentLoadCompleted;

//        #endregion

//        #region 与界面绑定的属性

//        #region 命令属性
      
//        /// <summary>
//        /// 查询员工信息命令
//        /// </summary>
//        public DelegateCommand QueryPersonInfoCommand { get; set; }
        
//        /// <summary>
//        /// 增加一个新员工命令
//        /// </summary>
//        public DelegateCommand AddPersonInfoCommand { get; set; }
        
//        /// <summary>
//        /// 批量修改人员属性命令
//        /// </summary>
//        public DelegateCommand BatchModifyCommand { get; set; }

//        /// <summary>
//        /// 批量删除命令
//        /// </summary>
//        public DelegateCommand BatchDeleteCommand { get; set; }
        
//        /// <summary>
//        /// 批量停用虹膜命令
//        /// </summary>
//        public DelegateCommand BatchStopIrisCommand { get; set; }

//        #endregion

//        #region  Command 排序绑定
//        ////排序命令
//        //public RelayCommand<EventInformation<MouseButtonEventArgs>> SortBehaviorCommand
//        //{
//        //    get
//        //    {
//        //        //MvvmLightCommand.SL4.TriggerActions
//        //        if (mouseButtonDownCommand == null)
//        //        {
//        //            mouseButtonDownCommand = new RelayCommand<EventInformation<MouseButtonEventArgs>>(SortAttendLeave);
//        //        }
//        //        return mouseButtonDownCommand;
//        //    }

//        //}
//        //private RelayCommand<EventInformation<MouseButtonEventArgs>> mouseButtonDownCommand = null;

//        //public RelayCommand<EventInformation<MouseButtonEventArgs>> SortBehaviorUpCommand
//        //{
//        //    get
//        //    {
//        //        if (mouseButtonUpCommand == null)
//        //        {
//        //            mouseButtonUpCommand = new RelayCommand<EventInformation<MouseButtonEventArgs>>(SortAttendLeaveUp);
//        //        }
//        //        return mouseButtonUpCommand;
//        //    }

//        //}
//        //private RelayCommand<EventInformation<MouseButtonEventArgs>> mouseButtonUpCommand = null;
      
     

//        //public RelayCommand<EventInformation<MouseEventArgs>> MouseMoveBehaviorCommand
//        //{
//        //    get
//        //    {
//        //        //MvvmLightCommand.SL4.TriggerActions
//        //        if (mouseMoveEventCommand == null)
//        //        {
//        //            mouseMoveEventCommand = new RelayCommand<EventInformation<MouseEventArgs>>(MouseMoveAttendLeave);
//        //        }
//        //        return mouseMoveEventCommand;
//        //    }

//        //}
//        //private RelayCommand<EventInformation<MouseEventArgs>> mouseMoveEventCommand = null;
       
//        //public RelayCommand<EventInformation<EventArgs>> LayoutUpdateCommand
//        //{
//        //    get
//        //    {
//        //        //MvvmLightCommand.SL4.TriggerActions
//        //        if (layoutUpdateEventCommand == null)
//        //        {
//        //            layoutUpdateEventCommand = new RelayCommand<EventInformation<EventArgs>>(LayoutUpadtaAttendLeave);
//        //        }
//        //        return layoutUpdateEventCommand;
//        //    }

//        //}
//        //private RelayCommand<EventInformation<EventArgs>> layoutUpdateEventCommand = null;
//        #endregion

//        #region 其他属性
     
//        /// <summary>
//        /// 全选按钮的绑定类
//        /// </summary>
//        public MarkObject MarkObj{ get; set; }
 

//        private int departs_SelectedIndex;
        
//        /// <summary>
//        /// 当前选择的部门Index
//        /// </summary>
//        public int Departs_SelectedIndex
//        {
//            get { return departs_SelectedIndex; }
//            set
//            {
//                departs_SelectedIndex = value;
//                this.NotifyPropertyChanged("Departs_SelectedIndex");
//            }
//        }

//        private List<string> departs;

//        /// <summary>
//        /// 可供选择的部门列表
//        /// </summary>
//        public List<string> Departs
//        {
//            get { return departs; }
//            set
//            {
//                departs = value;
//                this.NotifyPropertyChanged("Departs");
//            }
//        }

//        private int irisRegisters_SelectedIndex;

//        /// <summary>
//        /// 当前选择的虹膜注册
//        /// </summary>
//        public int IrisRegisters_SelectedIndex
//        {
//            get { return irisRegisters_SelectedIndex; }
//            set
//            {
//                irisRegisters_SelectedIndex = value;
//                this.NotifyPropertyChanged("IrisRegisters_SelectedIndex");
//            }
//        }

       
//        private List<string> irisRegisters;
//        /// <summary>
//        /// 可供选择的虹膜注册列表
//        /// </summary>
//        public List<string> IrisRegisters
//        {
//            get { return irisRegisters; }
//            set
//            {
//                irisRegisters = value;
//                this.NotifyPropertyChanged("IrisRegisters");
//            }
//        }

//        private List<string> irisStatus;
//        /// <summary>
//        /// 可供选择的虹膜状态
//        /// </summary>
//        public List<string> IrisStatus
//        {
//            get { return irisStatus; }
//            set
//            {
//                irisStatus = value;
//                this.NotifyPropertyChanged("IrisStatus");
//            }
//        }

//        private int irisStatus_SelectedIndex;

//        /// <summary>
//        /// 当前选择的虹膜状态
//        /// </summary>
//        public int IrisStatus_SelectedIndex
//        {
//            get { return irisStatus_SelectedIndex; }
//            set
//            {
//                irisStatus_SelectedIndex = value;
//                this.NotifyPropertyChanged("IrisStatus_SelectedIndex");
//            }
//        }

//        private List<string> childDepartMode;
//        /// <summary>
//        /// 可供选择的子部门操作列表
//        /// </summary>
//        public List<string> ChildDepartMode
//        {
//            get { return childDepartMode; }
//            set
//            {
//                childDepartMode = value;
//                this.NotifyPropertyChanged("ChildDepartMode");
//            }
//        }

     
//        private int childDepartMode_SelectedIndex;
//        /// <summary>
//        /// 当前选择的子部门操作方式
//        /// </summary>
//        public int ChildDepartMode_SelectedIndex
//        {
//            get { return childDepartMode_SelectedIndex; }
//            set
//            {
//                childDepartMode_SelectedIndex = value;
//                this.NotifyPropertyChanged("ChildDepartMode_SelectedIndex");
//            }
//        }

      
//        private string personName;
//        /// <summary>
//        /// 人员名字
//        /// </summary>
//        public string PersonName
//        {
//            get { return personName; }
//            set
//            {
//                personName = value;
//                this.NotifyPropertyChanged("PersonName");
//            }
//        }

       
//        private string personWorkSn;
//        /// <summary>
//        /// 人员工号
//        /// </summary>
//        public string PersonWorkSn
//        {
//            get { return personWorkSn; }
//            set
//            {
//                personWorkSn = value;
//                this.NotifyPropertyChanged("PersonWorkSn");
//            }
//        }

//        private bool isBatchOperateBtnEnable;
//        /// <summary>
//        /// 批量操作按钮的enable属性
//        /// </summary>
//        public bool IsBatchOperateBtnEnable
//        {
//            get { return isBatchOperateBtnEnable; }
//            set
//            {
//                isBatchOperateBtnEnable = value;
//                this.NotifyPropertyChanged("IsBatchOperateBtnEnable");
//            }
//        }

//        private BaseViewModelCollection<UserPersonInfo> userPersonInfos;
//        /// <summary>
//        /// 人员信息表
//        /// </summary>
//        public BaseViewModelCollection<UserPersonInfo> UserPersonInfos
//        {
//            get { return userPersonInfos; }
//            set
//            {
//                userPersonInfos = value;
//                this.NotifyPropertyChanged("UserPersonInfos");
//            }
//        }

//        private UserPersonInfo selectPersonInfo = null;
//        /// <summary>
//        /// 当前选择的人员信息
//        /// </summary>
//        public UserPersonInfo SelectPersonInfo
//        {
//            get
//            {
//                return selectPersonInfo;
//            }
//            set
//            {
//                if (selectPersonInfo != value)
//                {
//                    selectPersonInfo = value;
//                }
//            }
//        }

      

//        #endregion
      
//        #endregion

//        #region 构造函数

//        public VmPeopleMng()
//        {
//            IsBatchOperateBtnEnable = false;
//            AddPersonInfoCommand = new DelegateCommand(new Action(AddPersonInfo));
//            BatchDeleteCommand = new DelegateCommand(new Action(BatchDelete));
//            BatchStopIrisCommand = new DelegateCommand(new Action(BatchStopIris));
//            BatchModifyCommand = new DelegateCommand(new Action(BatchModify));
            
//            #region 初始化操作 by cty
//            QueryPersonInfoCommand = new DelegateCommand(new Action(QueryPersonInfo));// by cty
//            this.ChildDepartMode = new List<string>();
//            this.ChildDepartMode.Add("不包含");
//            this.ChildDepartMode.Add("包含");
//            this.ChildDepartMode_SelectedIndex = 0;

//            this.IrisStatus = new List<string>();
//            this.IrisStatus.Add("全部");
//            this.IrisStatus.Add("停用");
//            this.IrisStatus.Add("启用");
//            this.IrisStatus_SelectedIndex = 0;

//            this.IrisRegisters = new List<string>();
//            this.IrisRegisters.Add("全部");
//            this.IrisRegisters.Add("已注册");
//            this.IrisRegisters.Add("未注册");
//            this.IrisRegisters.Add("注册单眼");
//            this.IrisRegisters.Add("注册双眼");
//            this.IrisRegisters.Add("仅注册左眼");
//            this.IrisRegisters.Add("仅注册右眼");
//            this.IrisRegisters_SelectedIndex = 0;
//            PersonWorkSn = "";
//            PersonName = "";
//            Departs = new List<string>();

//            GetDeparts();
//            Departs_SelectedIndex = -1;
         

//            departmentLoadCompleted += (o, e) =>
//                {
//                    string a = "全部";
//                    string b = "不包含";
//                    string c = "";
//                    GetPersonInfoTable(0, c, c, a, a, b);
//                };
//            #endregion
//        }

//        #endregion

//        #region 界面事件响应
        

//        // 查询按钮
//        private void QueryPersonInfo()
//        {
//            //通过ria向后台查询
//            serviceDomDbAccess = new DomainServiceIriskingAttend();
//            GetPersonInfoTable(Departs[Departs_SelectedIndex].Trim(), PersonName.Trim(), PersonWorkSn.Trim(),
//                  IrisStatus[IrisStatus_SelectedIndex].Trim(), IrisRegisters[IrisRegisters_SelectedIndex].Trim(), 
//                  ChildDepartMode[ChildDepartMode_SelectedIndex].Trim());
           
//        }

//        //增加新员工按钮
//        private void AddPersonInfo()
//        {
//            var childWnd_AddPerson = new ChildWnd_OperatePerson(-1, ChildWndOptionMode.Add, PersonOperate_callback);
//            childWnd_AddPerson.Show();
//        }

//        //批量删除
//        private void BatchDelete()
//        {
//            List<int> delete_personIds = new List<int>();
//            foreach (var item in UserPersonInfos)
//            {
//                if (item.isSelected)
//                {
//                    delete_personIds.Add((int)item.person_id);
//                }
//            }
//            if (delete_personIds.Count > 0)
//            {
//                MsgBoxWindow.MsgBox(
//                    "请注意，您将进行如下操作\r\n批量删除人员信息！",
//                    Dialog.MsgBoxWindow.MsgIcon.Warning,
//                    Dialog.MsgBoxWindow.MsgBtns.OKCancel,
//                    (e) =>
//                    {
//                        if (e == MsgBoxWindow.MsgResult.OK)
//                        {
//                            //通过ria向后台发送请求
//                            BatchDelete(delete_personIds.ToArray());
//                        }
//                    });

//            }
//            else
//            {
//                MsgBoxWindow.MsgBox(
//                       "请至少选择一个人员！", Dialog.MsgBoxWindow.MsgIcon.Information, Dialog.MsgBoxWindow.MsgBtns.OK);
//            }

//        }

//        //批量停用虹膜
//        private void BatchStopIris()
//        {
//            BaseViewModelCollection<UserPersonInfo> UserPersonInfos_StopIris = new BaseViewModelCollection<UserPersonInfo>();
//            BaseViewModelCollection<PersonStopIrisInfo> PersonInfos_StopIris = new BaseViewModelCollection<PersonStopIrisInfo>();
//            foreach (var item in UserPersonInfos)
//            {
//                if (item.isSelected)
//                {
//                    UserPersonInfos_StopIris.Add(item);
//                }
//            }
//            if (UserPersonInfos_StopIris.Count > 0)
//            {
//                View.PeopleView.ChildWnd_BatchStopIris childW = new View.PeopleView.ChildWnd_BatchStopIris(UserPersonInfos_StopIris, true, PersonOperate_callback);
//                childW.Show();

//            }
//            else
//            {
//                MsgBoxWindow.MsgBox(
//                   "请至少选择一个人员！", Dialog.MsgBoxWindow.MsgIcon.Information, Dialog.MsgBoxWindow.MsgBtns.OK);
        
//            }
//        }

//        //批量修改人员属性
//        private void BatchModify()
//        {
//            BaseViewModelCollection<UserPersonInfo> UserPersonInfos_Selected = new BaseViewModelCollection<UserPersonInfo>();
//            foreach (var item in UserPersonInfos)
//            {
//                if (item.isSelected)
//                {
//                    UserPersonInfos_Selected.Add(item);
//                }
//            }
//            if (UserPersonInfos_Selected.Count > 0)
//            {
//                View.PeopleView.ChildWnd_BatchModify childW = new View.PeopleView.ChildWnd_BatchModify(UserPersonInfos_Selected, PersonOperate_callback);
//                childW.Show();
//            }
//            else
//            {
//                MsgBoxWindow.MsgBox(
//                    "请至少选择一个人员！", Dialog.MsgBoxWindow.MsgIcon.Information, Dialog.MsgBoxWindow.MsgBtns.OK);
              
//            }

//        }

//        #endregion

//        #region 回调函数
        
//        /// <summary>
//        /// 子窗口返回的回调函数
//        /// </summary>
//        /// <param name="DialogResult">子窗口是否点击确认关闭</param>
//        private void PersonOperate_callback(bool? DialogResult)
//        {
//            if (DialogResult.HasValue && DialogResult.Value)
//            {
//                //重新查询数据库
//                serviceDomDbAccess = new DomainServiceIriskingAttend();
//                GetPersonInfoTable(Departs[Departs_SelectedIndex].Trim(), PersonName.Trim(), PersonWorkSn.Trim(),
//                  IrisStatus[IrisStatus_SelectedIndex].Trim(), IrisRegisters[IrisRegisters_SelectedIndex].Trim(), ChildDepartMode[ChildDepartMode_SelectedIndex].Trim());
//            }
//        }
//        #endregion

//        #region  通过ria连接后台，数据库相关操作

//        /// <summary>
//        /// 异步获取数据库中数据
//        /// 获取部门名字
//        /// by cty
//        /// </summary>
//        private void GetDeparts()
//        {
//            try
//            {
//                WaitingDialog.ShowWaiting();

//                EntityQuery<UserDepartInfo> list = serviceDomDbAccess.GetDepartsInfoQuery();
//                ///回调异常类
//                Action<LoadOperation<UserDepartInfo>> actionCallBack = ErrorHandle<UserDepartInfo>.OnLoadErrorCallBack;
//                ///异步事件
//                LoadOperation<UserDepartInfo> lo = this.serviceDomDbAccess.Load(list, actionCallBack, null);

//                lo.Completed += delegate
//                {
//                    Departs.Clear();
//                    Departs.Add("全部");
//                    bool hasValue = false;
//                    //异步获取数据
//                    foreach (UserDepartInfo ar in lo.Entities)
//                    {
//                        Departs.Add(ar.depart_name);
//                        hasValue = true;
//                    }
//                    if (hasValue)
//                    {
//                        this.Departs_SelectedIndex = 0;
//                    }
//                    departmentLoadCompleted(null, null);

//                };
//            }
//            catch (Exception e)
//            {
//                WaitingDialog.HideWaiting();
//                ErrorWindow err = new ErrorWindow(e);
//                err.Show();
//            }
//        }

        
//        /// <summary>
//        /// 异步查询数据库中数据
//        /// 查询人员信息表
//        /// </summary>
//        /// </summary>
//        /// <param name="depart_Id">部门id 0代表全部部门</param>
//        /// <param name="person_Name">人员名称</param>
//        /// <param name="person_WorkSn">工号</param>
//        /// <param name="iris_Status">虹膜启用状态</param>
//        /// <param name="iris_Register">虹膜注册状态</param>
//        /// <param name="child_DepartMode">是否包含子部门</param>
//        private void GetPersonInfoTable(int depart_Id, string person_Name, string person_WorkSn,
//            string iris_Status, string iris_Register, string child_DepartMode)
//        {
//            try
//            {
//                WaitingDialog.ShowWaiting();
//                //ServiceDomDbAcess.ReOpenSever();
//                EntityQuery<UserPersonInfo> list = serviceDomDbAccess.GetPersonsInfoTableQuery(depart_Id, person_Name,
//                    person_WorkSn, iris_Status, iris_Register, child_DepartMode);
//                ///回调异常类
//                Action<LoadOperation<UserPersonInfo>> actionCallBack = new Action<LoadOperation<UserPersonInfo>>(ErrorHandle<UserPersonInfo>.OnLoadErrorCallBack);
//                ///异步事件
//                LoadOperation<UserPersonInfo> lo = this.serviceDomDbAccess.Load(list, actionCallBack, null);
//                lo.Completed += delegate
//                {
                  
//                    UserPersonInfos = new BaseViewModelCollection<UserPersonInfo>();

//                   // UserPersonInfosBinding.Clear();
//                    //异步获取数据
//                    foreach (UserPersonInfo ar in lo.Entities)
//                    {
//                        ///lzc
//                        ar.isSelected = false;

//                        UserPersonInfos.Add(ar);
//                      //  UserPersonInfosBinding.Add(ar);
//                    }

//                    this.IsBatchOperateBtnEnable = CheckIsAnyOneSelected();
//                    this.MarkObj.Selected = CheckIsAllSelected();

//                    WaitingDialog.HideWaiting();
                    
//                };
//            }
//            catch (Exception e)
//            {
//                WaitingDialog.HideWaiting();
//                ErrorWindow err = new ErrorWindow(e);
//                err.Show();
//            }
//        }

//        /// <summary>
//        /// 批量删除人员
//        /// </summary>
//        /// <param name="personIds">人员id数组</param>
//        private void BatchDelete(int[] personIds)
//        {
//            try
//            {
//                Action<InvokeOperation<OptionInfo>> callBack = CallBackHandleControl<OptionInfo>.OnInvokeErrorCallBack;
//                CallBackHandleControl<OptionInfo>.m_sendValue = (op) =>
//                {

//                    //异步获取数据
//                    if (op.isSuccess)
//                    {
//                        MsgBoxWindow.MsgBox(
//                             string.Format("操作成功，共有{0}个人被删除！", personIds.Length),
//                             Dialog.MsgBoxWindow.MsgIcon.Succeed, Dialog.MsgBoxWindow.MsgBtns.OK);
//                    }
//                    else
//                    {
//                        MsgBoxWindow.MsgBox(
//                           op.option_info +"！",
//                           Dialog.MsgBoxWindow.MsgIcon.Warning, Dialog.MsgBoxWindow.MsgBtns.OK);
//                    }


//                    WaitingDialog.HideWaiting();
                
//                    QueryPersonInfo();
//                };
//                WaitingDialog.ShowWaiting();
//                serviceDomDbAccess.BatchDeletePerson(personIds, callBack, null);
//            }
//            catch (Exception e)
//            {
//                WaitingDialog.HideWaiting();
//                ErrorWindow err = new ErrorWindow(e);
//                err.Show();
//            }
//        }

//        #endregion

//        #region 私有功能函数
       
//        /// <summary>
//        /// 检查是否有Item被选中
//        /// </summary>
//        private bool CheckIsAnyOneSelected()
//        {
            
//            foreach (var item in UserPersonInfos)
//            {
//                if (item.isSelected)
//                {
//                    return true;
//                }
//            }
//            return false;
          
            
//        }

//        /// <summary>
//        /// 检查Item是否全部被选中
//        /// </summary>
//        private bool CheckIsAllSelected()
//        {
            
//            foreach (var item in UserPersonInfos)
//            {
//                if (!item.isSelected)
//                {
//                    return false;
//                }
//            }
//            return true;
            
//        }
//        #endregion

//        #region 扩展功能、给view层的接口
//        //选中全部人员或者取消选中
//        public void SelectAllPerson(bool? IsChecked)
//        {
//            if (IsChecked.Value)
//            {
//                foreach (var item in UserPersonInfos)
//                {
//                    item.isSelected = true;
//                }
//            }
//            else
//            {
//                foreach (var item in UserPersonInfos)
//                {
//                    item.isSelected = false;
//                }
//            }

//            this.IsBatchOperateBtnEnable = CheckIsAnyOneSelected();
//            this.MarkObj.Selected = CheckIsAllSelected();
//        }

//        /// <summary>
//        /// 按需选择datagrid的Items
//        /// </summary>
//        /// <param name="sender">datagrid对象</param>
//        public void SelectItems(UserPersonInfo personInfo)
//        {

//            personInfo.isSelected = !personInfo.isSelected;

//            this.IsBatchOperateBtnEnable = CheckIsAnyOneSelected();
//            this.MarkObj.Selected = CheckIsAllSelected();
//        }

      
    
      
//        //查看人员信息 by cty
//        public void Check(int personId)
//        {
           
//            var childWnd_CheckPerson = new ChildWnd_OperatePerson(personId, ChildWndOptionMode.Check, PersonOperate_callback);
//            childWnd_CheckPerson.Show();
//        }

//        //修改人员信息 by cty
//        public void Modify(int personId)
//        {
            
//            var childWnd_CheckPerson = new ChildWnd_OperatePerson(personId, ChildWndOptionMode.Modify, PersonOperate_callback);
//            childWnd_CheckPerson.Show();
//        }

//        //删除人员信息 by cty
//        public void Delete(int personId)
//        {
            
//            var childWnd_CheckPerson = new ChildWnd_OperatePerson(personId, ChildWndOptionMode.Delete, PersonOperate_callback);
//            childWnd_CheckPerson.Show();
//        }

//        //查看识别记录 by cty || 优化代码 by wz
//        public void Record(UserPersonInfo personInfo)
//        {
           
//            var chidlWnd = new RecogInfo(personInfo);
//            chidlWnd.Show();
//        }

//        //停用虹膜 by cty || 优化代码 by wz
//        public void StopIris(UserPersonInfo personInfo)
//        {
//            BaseViewModelCollection<UserPersonInfo> UserPersonInfos_StopIris = new BaseViewModelCollection<UserPersonInfo>();
//            if (personInfo != null)
//            {
//                UserPersonInfos_StopIris.Add(personInfo);
//            }

//            if (UserPersonInfos_StopIris.Count > 0)
//            {
//                View.PeopleView.ChildWnd_BatchStopIris childW = new View.PeopleView.ChildWnd_BatchStopIris(UserPersonInfos_StopIris, false, PersonOperate_callback);
//                childW.Show();
//            }
//            else
//            {
//                MsgBoxWindow.MsgBox(
//                          "请至少选择一个人员！",
//                          Dialog.MsgBoxWindow.MsgIcon.Information, Dialog.MsgBoxWindow.MsgBtns.OK);
//            }

//        }

   
//        #endregion

//        #region 排序相关函数
//        //private string dir = "asc";
//        //private string sortFiled = "";
//        //private DataGridColumnHeader sortHeader;
//        ///// <summary>
//        ///// DataGrid 字段汉语拼音排序
//        ///// </summary>
//        ///// <param name="ei"></param>
//        //private void SortAttendLeave(EventInformation<MouseButtonEventArgs> ei)
//        //{
//        //    try
//        //    {
//        //        EventInformation<MouseButtonEventArgs> eventInfo = ei as EventInformation<MouseButtonEventArgs>;

//        //        System.Windows.Controls.DataGrid sender = eventInfo.Sender as System.Windows.Controls.DataGrid;
//        //        MouseButtonEventArgs e = ei.EventArgs;
//        //        var u = from element in VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(null), null)
//        //                where element is DataGridColumnHeader
//        //                select element;
//        //        if (u.Count() == 1)
//        //        {
//        //            //鼠标点击的ColumnHeader 
//        //            DataGridColumnHeader header = (DataGridColumnHeader)u.Single();
//        //            //要排序的字段 
//        //            if (header == null || header.Content == null)
//        //            {
//        //                return;
//        //            }
//        //            string _newSort = header.Content.ToString();
//        //            if (dir == "desc")
//        //            {
//        //                dir = "asc";
//        //            }
//        //            else
//        //            {
//        //                dir = "desc";
//        //            }
//        //            sortFiled = _newSort;
//        //            sortHeader = header;
//        //            SortData(sender, dir, sortFiled);
//        //            e.Handled = true;
//        //        }
//        //        else
//        //        {
//        //            e.Handled = false;
//        //        }
//        //        setColumnSortState();
//        //    }
//        //    catch (Exception e)
//        //    {
//        //        WaitingDialog.HideWaiting();
//        //        ErrorWindow err = new ErrorWindow(e);
//        //        err.Show();
//        //    }
//        //}
        
//        //排序事件
//        //private void SortData(Object sender, string sortType, string sortFiled)
//        //{
//        //    DataGrid dataGrid1 = sender as DataGrid;
//        //    // dDetailList = ChangeResult(DetailList);                           
//        //    UserPersonInfosBinding.Clear();
//        //    if (sortType == "desc")
//        //    {
//        //        switch (sortFiled)
//        //        {
//        //            case "人员工号":
//        //                {
//        //                    foreach (var ar in UserPersonInfos.OrderByDescending(u => u.work_sn))
//        //                    {
//        //                        UserPersonInfosBinding.Add(ar);
//        //                    }
//        //                    break;
//        //                }
//        //            case "人员姓名":
//        //                {
//        //                    foreach (var ar in UserPersonInfos.OrderByDescending(u => u.person_name))
//        //                    {
//        //                        UserPersonInfosBinding.Add(ar);
//        //                    }
//        //                    break;
//        //                }
//        //            case "虹膜":
//        //                {
//        //                    foreach (var ar in UserPersonInfos.OrderByDescending(u => u.iris_register))
//        //                    {
//        //                        UserPersonInfosBinding.Add(ar);
//        //                    }
//        //                    break;
//        //                }
//        //            case "虹膜状态":
//        //                {
//        //                    foreach (var ar in UserPersonInfos.OrderByDescending(u => u.iris_status))
//        //                    {
//        //                        UserPersonInfosBinding.Add(ar);
//        //                    }
//        //                    break;
//        //                }
//        //            case "部门":
//        //                {
//        //                    foreach (var ar in UserPersonInfos.OrderByDescending(u => u.depart_name))
//        //                    {
//        //                        UserPersonInfosBinding.Add(ar);
//        //                    }
//        //                    break;
//        //                }
//        //            case "上级部门":
//        //                {
//        //                    foreach (var ar in UserPersonInfos.OrderByDescending(u => u.parent_depart_name))
//        //                    {
//        //                        UserPersonInfosBinding.Add(ar);
//        //                    }
//        //                    break;
//        //                }
//        //            case "部门编号":
//        //                {
//        //                    foreach (var ar in UserPersonInfos.OrderByDescending(u => u.depart_sn))
//        //                    {
//        //                        UserPersonInfosBinding.Add(ar);
//        //                    }
//        //                    break;
//        //                }
//        //            case "性别":
//        //                {
//        //                    foreach (var ar in UserPersonInfos.OrderByDescending(u => u.sex))
//        //                    {
//        //                        UserPersonInfosBinding.Add(ar);
//        //                    }
//        //                    break;
//        //                }
//        //            case "所在班制":
//        //                {
//        //                    foreach (var ar in UserPersonInfos.OrderByDescending(u => u.class_type_name_on_ground))
//        //                    {
//        //                        UserPersonInfosBinding.Add(ar);
//        //                    }
//        //                    break;
//        //                }
//        //            default: 
//        //                {
//        //                    foreach (var ar in UserPersonInfos)
//        //                    {
//        //                        UserPersonInfosBinding.Add(ar);
//        //                    }
//        //                    break;
//        //                }

//        //        }
//        //    }
//        //    else
//        //    {
//        //        switch (sortFiled)
//        //        {
//        //            case "人员工号":
//        //                {
//        //                    foreach (var ar in UserPersonInfos.OrderBy(u => u.work_sn))
//        //                    {
//        //                        UserPersonInfosBinding.Add(ar);
//        //                    }
//        //                    break;
//        //                }
//        //            case "人员姓名":
//        //                {
//        //                    foreach (var ar in UserPersonInfos.OrderBy(u => u.person_name))
//        //                    {
//        //                        UserPersonInfosBinding.Add(ar);
//        //                    }
//        //                    break;
//        //                }
//        //            case "虹膜":
//        //                {
//        //                    foreach (var ar in UserPersonInfos.OrderBy(u => u.iris_register))
//        //                    {
//        //                        UserPersonInfosBinding.Add(ar);
//        //                    }
//        //                    break;
//        //                }
//        //            case "虹膜状态":
//        //                {
//        //                    foreach (var ar in UserPersonInfos.OrderBy(u => u.iris_status))
//        //                    {
//        //                        UserPersonInfosBinding.Add(ar);
//        //                    }
//        //                    break;
//        //                }
//        //            case "部门":
//        //                {
//        //                    foreach (var ar in UserPersonInfos.OrderBy(u => u.depart_name))
//        //                    {
//        //                        UserPersonInfosBinding.Add(ar);
//        //                    }
//        //                    break;
//        //                }
//        //            case "上级部门":
//        //                {
//        //                    foreach (var ar in UserPersonInfos.OrderBy(u => u.parent_depart_name))
//        //                    {
//        //                        UserPersonInfosBinding.Add(ar);
//        //                    }
//        //                    break;
//        //                }
//        //            case "部门编号":
//        //                {
//        //                    foreach (var ar in UserPersonInfos.OrderBy(u => u.depart_sn))
//        //                    {
//        //                        UserPersonInfosBinding.Add(ar);
//        //                    }
//        //                    break;
//        //                }
//        //            case "性别":
//        //                {
//        //                    foreach (var ar in UserPersonInfos.OrderBy(u => u.sex))
//        //                    {
//        //                        UserPersonInfosBinding.Add(ar);
//        //                    }
//        //                    break;
//        //                }
//        //            case "所在班制":
//        //                {
//        //                    foreach (var ar in UserPersonInfos.OrderBy(u => u.class_type_name_on_ground))
//        //                    {
//        //                        UserPersonInfosBinding.Add(ar);
//        //                    }
//        //                    break;
//        //                }
//        //            default:
//        //                {
//        //                    foreach (var ar in UserPersonInfos)
//        //                    {
//        //                        UserPersonInfosBinding.Add(ar);
//        //                    }
//        //                    break;
//        //                }
//        //        }
//        //    }
//        //}

//        //private void MouseMoveAttendLeave(EventInformation<MouseEventArgs> ei)
//        //{
//        //    setColumnSortState();
//        //}
//        //private void LayoutUpadtaAttendLeave(EventInformation<EventArgs> ei)
//        //{
//        //    setColumnSortState();
//        //}
//        //private void SortAttendLeaveUp(EventInformation<MouseButtonEventArgs> ei)
//        //{
//        //    setColumnSortState();
//        //}
//        ///// <summary>
//        ///// 显示排序的上下箭头 
//        ///// </summary>
//        //private void setColumnSortState()
//        //{
//        //    if (sortHeader == null)
//        //        return;
//        //    if (dir == "asc")
//        //        VisualStateManager.GoToState(sortHeader, "SortAscending", false);
//        //    else
//        //        VisualStateManager.GoToState(sortHeader, "SortDescending", false);
//        //}
//        #endregion

//    }

 

//}
