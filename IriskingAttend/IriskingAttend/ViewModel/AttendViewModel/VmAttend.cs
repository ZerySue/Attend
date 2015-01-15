/*************************************************************************
** 文件名:   VmAttend.cs
** 主要类:   VmAttend
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   lzc
** 日  期:   2013-4-11
** 修改人:   gqy
** 日  期:   2013-9-12
** 描  述:   VmAttend，主要是考勤查询
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
using IriskingAttend.Web;
using Irisking.Web.DataModel;
using System.IO.IsolatedStorage;
using System.ServiceModel.DomainServices.Client;
using EDatabaseError;
using Microsoft.Practices.Prism.Commands;
using IriskingAttend.Dialog;
using System.Windows.Controls.Primitives;
using IriskingAttend.Common;

namespace IriskingAttend.ViewModel
{

    public class VmAttend : BaseViewModel
    {
        #region 私有变量

        //本地独立存储，用来传参
        private IsolatedStorageSettings _querySetting = IsolatedStorageSettings.ApplicationSettings;

        //当前选择的考勤数据
        private UserAttendRec _selectAttendRecordItem = null;

        #endregion

        #region 绑定数据源

        /// <summary>
        /// 考勤数据
        /// </summary>
        public BaseViewModelCollection<UserAttendRec> AttendRecModel { get; set; }

        /// <summary>
        /// 考勤详细信息
        /// </summary>
        public BaseViewModelCollection<UserAttendRecDetail> AttendRecDetailModel { get; set; }

        /// <summary>
        /// 人员考勤记录绑定
        /// </summary>
        public UserAttendRec SelectAttendRec
        {
            get
            {
                return _selectAttendRecordItem;
            }
            set
            {
                if (value != _selectAttendRecordItem)
                {
                    _selectAttendRecordItem = value;
                    OnPropertyChanged(() => SelectAttendRec);
                }
            }
        }

        #endregion

        #region 事件

        /// <summary>
        /// 考勤信息加载完成
        /// </summary>
        public event EventHandler AttendRecLoadCompleted;

        /// <summary>
        /// 考勤详细信息加载完成
        /// </summary>
        public event EventHandler AttendDetialRecLoadCompleted;

        #endregion

        #region 构造函数
        public VmAttend()
        {
            ///重建服务
            ServiceDomDbAcess.ReOpenSever();
            AttendDetialRecLoadCompleted +=(o,e)=>{};
            //初始化
            AttendRecModel = new BaseViewModelCollection<UserAttendRec>();
           // AttendRecSignModel = new BaseViewModelCollection<UserAttendRec>();
            AttendRecDetailModel = new BaseViewModelCollection<UserAttendRecDetail>();
           // AttendRecSignDetailModel = new BaseViewModelCollection<UserAttendRecDetail>();
            AttendRecLoadCompleted += (a, e) => {};
        }
        #endregion

        #region 公共属性
        /// <summary>
        /// 考勤查询的开始时间
        /// </summary>
        public DateTime BeginDate{get;set;}

        /// <summary>
        /// 考勤查询截止时间
        /// </summary>
        public DateTime EndDate{get;set;}

        #endregion

        #region  Command绑定

        /// <summary>
        /// 显示考勤详细信息对话框委托
        /// </summary>
        protected DelegateCommand _showAttendRecDetailCmd = null;

        /// <summary>
        /// 显示考勤详细信息对话框委托
        /// </summary>
        protected DelegateCommand _showZKHBAttendRecDetailCmd = null;
        
        /// <summary>
        /// 显示详细信息对话框绑定Command
        /// </summary>
        public ICommand ShowAttendRecDetailCommand
        {
            get
            {
                if (null == _showAttendRecDetailCmd)
                {
                    _showAttendRecDetailCmd = new DelegateCommand(ShowAttendRecDetail);
                }
                return _showAttendRecDetailCmd;
            }
        }

        /// <summary>
        /// 显示中科红霸详细信息对话框绑定Command
        /// </summary>
        public ICommand ShowZKHBAttendRecDetailCommand
        {
            get
            {
                if (null == _showZKHBAttendRecDetailCmd)
                {
                    _showZKHBAttendRecDetailCmd = new DelegateCommand(ShowZKHBAttendRecDetail);
                }
                return _showZKHBAttendRecDetailCmd;
            }
        }

        #endregion

        #region Command绑定相应函数

        /// <summary>
        /// 显示考勤详细信息
        /// </summary>
        public void ShowZKHBAttendRecDetail()
        {
            try
            {
                GetAttendRecDetail();
                AttendRecordDetail detailDialog = new AttendRecordDetail(this);

                detailDialog.Closed += (o, e) =>
                {
                    if (detailDialog.isChanged)
                    {
                        GetZKHBAttend();
                    }
                };
                detailDialog.Show();
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }

        /// <summary>
        /// 显示考勤详细信息
        /// </summary>
        public void ShowAttendRecDetail()
        {
            try
            {
                GetAttendRecDetail();
                AttendRecordDetail detailDialog = new AttendRecordDetail(this);

                detailDialog.Closed += (o, e) =>
                    {
                        if (detailDialog.isChanged)
                        {
                            GetAttend();
                        }
                    };
                detailDialog.Show();
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }

        /// <summary>
        /// 重数据库中获取详细考勤记录
        /// </summary>
        public  void GetAttendRecDetail()
        {
            ////通过本地存储获取查询条件
            //if (_querySetting.Contains("attendConditon"))
            //{
            //    AttendQueryCondition condition;
            //    _querySetting.TryGetValue<AttendQueryCondition>("attendConditon", out condition);

            //    EntityQuery<UserAttendRecDetail> lstAttendDetail = ServiceDomDbAcess.GetSever().IrisGetAttendDetailQuery(condition.BeginTime,
            //        condition.EndTime, condition.DevTypeIdLst, condition.WorkTime, SelectAttendRec.person_id);

            //    //回调异常类
            //    Action<LoadOperation<UserAttendRecDetail>> getAttendDetailCallBack = new Action<LoadOperation<UserAttendRecDetail>>
            //        (ErrorHandle<UserAttendRecDetail>.OnLoadErrorCallBack);
            //    //异步事件
            //    LoadOperation<UserAttendRecDetail> lo = ServiceDomDbAcess.GetSever().Load(lstAttendDetail, getAttendDetailCallBack, null);

            //    lo.Completed += (o,arg) =>
            //    {
            //        WaitingDialog.HideWaiting();
            //        AttendRecDetailModel.Clear();
            //        try
            //        {
            //            //异步获取数据
            //            foreach (UserAttendRecDetail ar in lo.Entities.OrderBy(a=>a.attend_day))
            //            {
            //                AttendRecDetailModel.Add(ar);
            //            }

            //            AttendDetialRecLoadCompleted(null, new EventArgs());
            //        }
            //        catch (Exception e)
            //        {
            //            ChildWindow errorWin = new ErrorWindow("加载异常", e.Message);
            //            errorWin.Show();
            //        }
            //    };

            //    WaitingDialog.ShowWaiting("加载考勤详细信息，请等待...");
            //}
        }

        /// <summary>
        /// 考勤查询
        /// </summary>
        public void GetZKHBAttend()
        {
            ////通过本地存储获取查询条件
            //if (_querySetting.Contains("attendConditon"))
            //{ 
            //    try
            //    {
            //        AttendQueryCondition condition;
            //        _querySetting.TryGetValue<AttendQueryCondition>("attendConditon", out condition);

            //        BeginDate = condition.BeginTime;
            //        EndDate = condition.EndTime;
            //        EntityQuery<UserAttendRec> lstAttendRec;
            //        if (condition.DepartNameLst != null)
            //        {
            //            lstAttendRec = ServiceDomDbAcess.GetSever().IrisGetZKHBAttendRecQuery(condition.BeginTime,condition.EndTime, condition.DepartNameLst, 
            //                condition.DevTypeIdLst, condition.Name, condition.WorkSN, condition.PrincipalIdList, condition.WorkTypeIdList, condition.WorkTime);
            //        }
            //        else
            //        {
            //            lstAttendRec = ServiceDomDbAcess.GetSever().IrisGetZKHBAttendRecQuery(condition.BeginTime, condition.EndTime, condition.DepartNameLst, 
            //                condition.DevTypeIdLst, condition.Name, condition.WorkSN, condition.PrincipalIdList, condition.WorkTypeIdList, condition.WorkTime);
            //        }

            //        ///回调异常类
            //        Action<LoadOperation<UserAttendRec>> getAttendRecCallBack = new Action<LoadOperation<UserAttendRec>>
            //            (ErrorHandle<UserAttendRec>.OnLoadErrorCallBack);
            //        ///异步事件
            //        LoadOperation<UserAttendRec> lo = ServiceDomDbAcess.GetSever().Load(lstAttendRec, getAttendRecCallBack, null);

            //        lo.Completed += (o, e) =>
            //        {
            //            WaitingDialog.HideWaiting();
            //            AttendRecModel.Clear();
            //            try
            //            {
            //                //异步获取数据
            //                foreach (UserAttendRec ar in lo.Entities)
            //                {
            //                    AttendRecModel.Add(ar);
            //                }
            //                AttendRecLoadCompleted(this, new EventArgs());
            //            }
            //            catch (Exception err)
            //            {
            //                ErrorWindow errorWin = new ErrorWindow(err);
            //                errorWin.Show();
            //            }
            //        };
            //        WaitingDialog.ShowWaiting("加载考勤数据，请等待...");
            //    }
            //    catch (Exception e)
            //    {
            //        ErrorWindow err = new ErrorWindow(e);
            //        err.Show();
            //    }

            //}
            //else
            //{
            //    MsgBoxWindow.MsgBox("查询条件不准确！",
            //                            MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);
            //}
        }

        /// <summary>
        /// 考勤查询
        /// </summary>
        public void GetAttend()
        {
            //通过本地存储获取查询条件
            if (_querySetting.Contains("attendConditon"))
            {
                try
                {
                    AttendQueryCondition condition;
                    _querySetting.TryGetValue<AttendQueryCondition>("attendConditon", out condition);

                    BeginDate = condition.BeginTime;
                    EndDate = condition.EndTime;
                    EntityQuery<UserAttendRecDetail> lstAttendRec;
                    //if (condition.DepartIdLst != null)
                    //{
                    //    lstAttendRec = ServiceDomDbAcess.GetSever().IrisGetAttendRecQuery(condition.BeginTime, condition.EndTime, condition.DepartIdLst,
                    //        condition.DevTypeIdLst, condition.Name, condition.WorkSN, condition.PrincipalIdList, condition.WorkTypeIdList, condition.WorkTime);
                    //}
                    //else
                    //{
                    lstAttendRec = ServiceDomDbAcess.GetSever().IrisGetAttendRecQuery(condition.BeginTime, condition.EndTime,
                            condition.Name, condition.WorkSN);
                    //}

                    ///回调异常类
                    Action<LoadOperation<UserAttendRecDetail>> getAttendRecCallBack = new Action<LoadOperation<UserAttendRecDetail>>
                        (ErrorHandle<UserAttendRecDetail>.OnLoadErrorCallBack);
                    ///异步事件
                    LoadOperation<UserAttendRecDetail> lo = ServiceDomDbAcess.GetSever().Load(lstAttendRec, getAttendRecCallBack, null);

                    lo.Completed += (o, e) =>
                    {
                        WaitingDialog.HideWaiting();
                        AttendRecDetailModel.Clear();
                        try
                        {
                            //异步获取数据
                            foreach (UserAttendRecDetail ar in lo.Entities)
                            {
                                AttendRecDetailModel.Add(ar);
                            }
                            AttendRecLoadCompleted(this, new EventArgs());
                        }
                        catch (Exception err)
                        {
                            ErrorWindow errorWin = new ErrorWindow(err);
                            errorWin.Show();
                        }
                    };
                    WaitingDialog.ShowWaiting("加载考勤数据，请等待...");
                }
                catch (Exception e)
                {
                    ErrorWindow err = new ErrorWindow(e);
                    err.Show();
                }

            }
            else
            {
                MsgBoxWindow.MsgBox("查询条件不准确！",
                                        MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);
            }
        }

        #endregion

        #region 签到班        

        /// <summary>
        /// 考勤数据
        /// </summary>
        public BaseViewModelCollection<UserAttendRec> AttendRecSignModel { get; set; }

        /// <summary>
        /// 考勤详细信息
        /// </summary>
        public BaseViewModelCollection<UserAttendRecDetail> AttendRecSignDetailModel { get; set; }

        //当前选择的考勤数据
        private UserAttendRec _selectAttendRecordSignItem = null;
        /// <summary>
        /// 人员考勤记录绑定
        /// </summary>
        public UserAttendRec SelectAttendRecSign
        {
            get
            {
                return _selectAttendRecordSignItem;
            }
            set
            {
                if (value != _selectAttendRecordSignItem)
                {
                    _selectAttendRecordSignItem = value;
                    OnPropertyChanged(() => SelectAttendRecSign);
                }
            }
        }

        #region  Command绑定

        /// <summary>
        /// 显示考勤详细信息对话框委托
        /// </summary>
        protected DelegateCommand _showAttendRecSignDetailCmd = null;

        /// <summary>
        /// 显示详细信息对话框绑定Command
        /// </summary>
        public ICommand ShowAttendRecSignDetailCommand
        {
            get
            {
                if (null == _showAttendRecSignDetailCmd)
                {
                    _showAttendRecSignDetailCmd = new DelegateCommand(ShowAttendRecSignDetail);
                }
                return _showAttendRecSignDetailCmd;
            }
        }
        #endregion

        /// <summary>
        /// 显示考勤详细信息
        /// </summary>
        public void ShowAttendRecSignDetail()
        {
            //try
            //{
            //    GetAttendRecSignDetail();
            //    AttendRecordSignDetail detailDialog = new AttendRecordSignDetail(this);

            //    detailDialog.Closed += (o, e) =>
            //    {
            //        if (detailDialog.isChanged)
            //        {
            //            GetAttend();
            //        }
            //    };
            //    detailDialog.Show();
            //}
            //catch (Exception e)
            //{
            //    ErrorWindow err = new ErrorWindow(e);
            //    err.Show();
            //}
        }

        /// <summary>
        /// 从数据库中获取详细考勤记录
        /// </summary>
        public void GetAttendRecSignDetail()
        {
            ////通过本地存储获取查询条件
            //if (_querySetting.Contains("attendConditon"))
            //{
            //    AttendQueryCondition condition;
            //    _querySetting.TryGetValue<AttendQueryCondition>("attendConditon", out condition);

            //    EntityQuery<UserAttendRecDetail> lstAttendDetail = ServiceDomDbAcess.GetSever().IrisGetAttendSignDetailQuery(condition.BeginTime,
            //        condition.EndTime, condition.DevTypeIdLst, condition.WorkTime, SelectAttendRecSign.person_id);

            //    //回调异常类
            //    Action<LoadOperation<UserAttendRecDetail>> getAttendDetailCallBack = new Action<LoadOperation<UserAttendRecDetail>>
            //        (ErrorHandle<UserAttendRecDetail>.OnLoadErrorCallBack);
            //    //异步事件
            //    LoadOperation<UserAttendRecDetail> lo = ServiceDomDbAcess.GetSever().Load(lstAttendDetail, getAttendDetailCallBack, null);

            //    lo.Completed += (o, arg) =>
            //    {
            //        WaitingDialog.HideWaiting();
            //        AttendRecSignDetailModel.Clear();
            //        try
            //        {
            //            //异步获取数据
            //            foreach (UserAttendRecDetail ar in lo.Entities.OrderBy(a => a.attend_day))
            //            {
            //                AttendRecSignDetailModel.Add(ar);
            //            }

            //            AttendDetialRecLoadCompleted(null, new EventArgs());
            //        }
            //        catch (Exception e)
            //        {
            //            ChildWindow errorWin = new ErrorWindow("加载异常", e.Message);
            //            errorWin.Show();
            //        }
            //    };

            //    WaitingDialog.ShowWaiting("加载考勤详细信息，请等待...");
            //}
        }

        /// <summary>
        /// 考勤查询
        /// </summary>
        public void GetAttendRecSign()
        {
            ////通过本地存储获取查询条件
            //if (_querySetting.Contains("attendConditon"))
            //{
            //    try
            //    {
            //        AttendQueryCondition condition;
            //        _querySetting.TryGetValue<AttendQueryCondition>("attendConditon", out condition);

            //        BeginDate = condition.BeginTime;
            //        EndDate = condition.EndTime;
            //        EntityQuery<UserAttendRec> lstAttendRec;
            //        //if (condition.DepartIdLst != null)
            //        //{
            //        //    lstAttendRec = ServiceDomDbAcess.GetSever().IrisGetAttendRecSignQuery(condition.BeginTime, condition.EndTime, condition.DepartIdLst,
            //        //        condition.DevTypeIdLst, condition.Name, condition.WorkSN, condition.PrincipalIdList, condition.WorkTypeIdList, condition.WorkTime);
            //        //}
            //        //else
            //        //{
            //            lstAttendRec = ServiceDomDbAcess.GetSever().IrisGetAttendRecSignQuery(condition.BeginTime, condition.EndTime, VmLogin.OperatorDepartIDList,
            //                condition.Name, condition.WorkSN,);
            //        //}

            //        ///回调异常类
            //        Action<LoadOperation<UserAttendRec>> getAttendRecCallBack = new Action<LoadOperation<UserAttendRec>>
            //            (ErrorHandle<UserAttendRec>.OnLoadErrorCallBack);
            //        ///异步事件
            //        LoadOperation<UserAttendRec> lo = ServiceDomDbAcess.GetSever().Load(lstAttendRec, getAttendRecCallBack, null);

            //        lo.Completed += (o, e) =>
            //        {
            //            WaitingDialog.HideWaiting();
            //            AttendRecSignModel.Clear();
            //            try
            //            {
            //                //异步获取数据
            //                foreach (UserAttendRec ar in lo.Entities)
            //                {
            //                    AttendRecSignModel.Add(ar);
            //                }                           
            //            }
            //            catch (Exception err)
            //            {
            //                ErrorWindow errorWin = new ErrorWindow(err);
            //                errorWin.Show();
            //            }
            //        };
            //        WaitingDialog.ShowWaiting("加载考勤数据，请等待...");
            //    }
            //    catch (Exception e)
            //    {
            //        ErrorWindow err = new ErrorWindow(e);
            //        err.Show();
            //    }

            //}
            //else
            //{
            //    MsgBoxWindow.MsgBox("查询条件不准确！",
            //                            MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);
            //}
        }
        #endregion
    }

    #region 辅助类

    /// <summary>
    /// 考勤查询条件定义
    /// </summary>
    public class AttendQueryCondition
    {
        /// <summary>
        /// 起始时间
        /// </summary>
        public DateTime BeginTime { get; set; }

        /// <summary>
        /// 截止时间
        /// </summary>
        public DateTime EndTime { get; set; }

        /// </summary>
        ///部门ID
        /// </summary>
        public int[] DepartIdLst { get; set; }

        /// </summary>
        ///部门名称
        /// </summary>
        public string[] DepartNameLst { get; set; }

        /// </summary>
        ///在岗类型
        /// </summary>
        public int[] DevTypeIdLst { get; set; }

        /// </summary>
        /// 职务类型  add by gqy
        /// </summary>
        public int[] PrincipalIdList { get; set; }

        /// </summary>
        /// 工种类型  add by gqy
        /// </summary>
        public int[] WorkTypeIdList { get; set; }

        /// </summary>
        /// 工作时长  add by gqy
        /// </summary>
        public int WorkTime { get; set; }

        /// </summary>
        ///人员姓名
        /// </summary>
        public string Name { get; set; }

        /// </summary>
        ///工号
        /// </summary>
        public string WorkSN { get; set; }

        /// <summary>
        /// 构造函数 初始化默认值
        /// </summary>
        public AttendQueryCondition()
        {
            BeginTime = DateTime.MinValue;
            EndTime = DateTime.MaxValue;
            DepartIdLst = null;            
            DevTypeIdLst = null;            
            Name = "";
            WorkSN = "";
            DepartNameLst = null;
            //add by gqy
            PrincipalIdList = null;
            WorkTypeIdList = null;
            WorkTime = 0;
        }
    }

    #endregion
}
