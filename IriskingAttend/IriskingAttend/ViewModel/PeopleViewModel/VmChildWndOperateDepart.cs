/*************************************************************************
** 文件名:   VmChildWndOperateDepart.cs
×× 主要类:   VmChildWndOperateDepart
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   cty
** 日  期:   2013-6-14
** 修改人:   wz  代码优化
** 日  期:   2013-7-24
** 描  述:   VmChildWndOperateDepart类,部门管理（增删改查）
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
using IriskingAttend.Dialog;
using IriskingAttend.BehaviorSelf;
using System.Linq;
using IriskingAttend.ViewModel.SystemViewModel;
using System.Text;

namespace IriskingAttend.ViewModel.PeopleViewModel
{
    public class VmChildWndOperateDepart : BaseViewModel
    {
        #region 字段声明
       
        /// <summary>
        /// 与服务声明
        /// </summary>
        private DomainServiceIriskingAttend serviceDomDbAccess = new DomainServiceIriskingAttend();

        /// <summary>
        /// vm加载完毕事件
        /// </summary>
        public event EventHandler LoadCompletedEvent;

        /// <summary>
        /// 窗口关闭事件
        /// </summary>
        public event Action<bool> CloseEvent;

        //关联的子窗口的模式
        private ChildWndOptionMode operateDepartMode;

        //当前部门信息
        private UserDepartInfo _departInfo;

        //是否进行了继续添加操作
        private bool _isContinueAddExcuted = false;


        private HashSet<string> departNamesCollection = new HashSet<string>();
        private HashSet<string> departSnCollection = new HashSet<string>();

        #endregion

        #region 与页面绑定的属性
        //命令
        public DelegateCommand ContinueAddBtnCmd { get; set; }//bug2731 新增加一个命令 by蔡天雨
        public DelegateCommand OkBtnCmd { get; set; }
        public DelegateCommand CancelBtnCmd { get; set; }

        
        private string okBtnContent;
        /// <summary>
        /// okBtn 内容
        /// </summary>
        public string OkBtnContent
        {
            get { return okBtnContent; }
            set
            {
                okBtnContent = value;
                this.NotifyPropertyChanged("OkBtnContent");
            }
        }

        private Visibility continueAddBtnVisibility;
        /// <summary>
        /// 继续添加按钮可见性
        /// </summary>
        public Visibility ContinueAddBtnVisibility
        {
            get { return continueAddBtnVisibility; }
            set
            {
                continueAddBtnVisibility = value;
                this.NotifyPropertyChanged("ContinueAddBtnVisibility");
            }
        }

        private Visibility okBtnVisibility;
        /// <summary>
        /// okBtn可见性
        /// </summary>
        public Visibility OkBtnVisibility
        {
            get { return okBtnVisibility; }
            set
            {
                okBtnVisibility = value;
                this.NotifyPropertyChanged("OkBtnVisibility");
            }
        }
        

        private string cancelBtnContent;
        /// <summary>
        /// CancelBtn 内容 by cty
        /// </summary>
        public string CancelBtnContent
        {
            get { return cancelBtnContent; }
            set
            {
                cancelBtnContent = value;
                this.NotifyPropertyChanged("CancelBtnContent");
            }
        }
        
        private string title;
        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                this.NotifyPropertyChanged("Title");
            }
        }

        
        private BaseViewModelCollection<UserDepartInfo> _parentDeparts;
        /// <summary>
        /// 父部门列表
        /// </summary>
        public BaseViewModelCollection<UserDepartInfo> ParentDeparts
        {
            get { return _parentDeparts; }
            set
            {
                _parentDeparts = value;
                this.NotifyPropertyChanged("ParentDeparts");
            }
        }


        private UserDepartInfo _selectedParentDepart;
        /// <summary>
        /// 上级部门selectedIndex
        /// </summary>
        public UserDepartInfo SelectedParentDepart
        {
            get { return _selectedParentDepart; }
            set
            {
                _selectedParentDepart = value;
                this.NotifyPropertyChanged("SelectedParentDepart");
            }
        }

        
        private string departName ="";
        /// <summary>
        /// 部门名称
        /// </summary>
        public string DepartName
        {
            get { return departName; }
            set
            {
                departName = value;
                this.NotifyPropertyChanged("DepartName");
            }
        }

        private string departSn = "";
        /// <summary>
        /// 部门编号
        /// </summary>
        public string DepartSn
        {
            get { return departSn; }
            set
            {
                departSn = value;
                this.NotifyPropertyChanged("DepartSn");    
            }
        }


        
        private string departPhone;
        /// <summary>
        /// 部门电话
        /// </summary>
        public string DepartPhone
        {
            get { return departPhone; }
            set
            {
                departPhone = value;
                this.NotifyPropertyChanged("DepartPhone");
            }
        }

        private string departMemo;
        /// <summary>
        /// 部门备注
        /// </summary>
        public string DepartMemo
        {
            get { return departMemo; }
            set
            {
                departMemo = value;
                this.NotifyPropertyChanged("DepartMemo");
            }
        }

        private bool isReadOnly;

        public bool IsReadOnly
        {
            get { return isReadOnly; }
            set
            {
                IsEditable = !value;
                isReadOnly = value;
                this.NotifyPropertyChanged("IsReadOnly");
            }
        }

        
        private bool isEditable;
        /// <summary>
        /// 是否可操作
        /// </summary>
        public bool IsEditable
        {
            get { return isEditable; }
            set
            {
                isEditable = value;
                this.NotifyPropertyChanged("IsEditable");
            }
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dInfo">部门信息</param>
        /// <param name="mode">操作方式</param>
        public VmChildWndOperateDepart(UserDepartInfo dInfo, ChildWndOptionMode mode)
        {
            _departInfo = dInfo;
            //如果没有父部门则默认父部门名称为无
            if (_departInfo == null)
            {
                _departInfo = new UserDepartInfo();
                _departInfo.depart_id = -1;
            }
            _departInfo.parent_depart_name = _departInfo.parent_depart_name == "" ? "无" : _departInfo.parent_depart_name;
            
            operateDepartMode = mode;
            
            GetContent(mode);
            GetDeparts();

            VmDepartMng.UpdateOperateDepartIdCollection();
          
            OkBtnCmd = new DelegateCommand(new Action(OkBtnClicked));
            ContinueAddBtnCmd = new DelegateCommand(new Action(ContinueAddBtnClicked));
            CancelBtnCmd = new DelegateCommand(new Action(CancelBtnClicked));
        }

        #endregion

        #region 私有功能函数
      

        /// <summary>
        /// 根据操作模式确定窗体其中的内容
        /// </summary>
        /// <param name="mode"></param>
        private void GetContent(ChildWndOptionMode mode)
        {
            ContinueAddBtnVisibility = Visibility.Collapsed;
            OkBtnVisibility = Visibility.Visible;
            switch (mode)
            {
                case ChildWndOptionMode.Modify:
                    IsReadOnly = false;
                    OkBtnContent = "修改";
                    CancelBtnContent = "取消";
                    Title = "修改部门信息";
                    break;
                case ChildWndOptionMode.Delete:
                    IsReadOnly = true;
                    OkBtnContent = "删除";
                    CancelBtnContent = "取消";
                    Title = "删除部门信息";
                    break;
                case ChildWndOptionMode.Check:
                    IsReadOnly = true;
                    OkBtnContent = "确定";
                    CancelBtnContent = "关闭";
                    Title = "查看部门信息";
                    OkBtnVisibility = Visibility.Collapsed;
                    break;
                case ChildWndOptionMode.Add:
                    IsReadOnly = false;
                    OkBtnContent = "添加";
                    CancelBtnContent = "取消";
                    Title = "添加部门信息";
                    ContinueAddBtnVisibility = Visibility.Visible;
                    break;

            }

            if (mode != ChildWndOptionMode.Add)
            {
                this.DepartMemo = _departInfo.memo;
                this.DepartName = _departInfo.depart_name;
                this.DepartPhone = _departInfo.contact_phone;
                this.DepartSn = _departInfo.depart_sn;
            }

        }

        /// <summary>
        /// 获取修改部门操作的描述
        /// </summary>
        /// <returns></returns>
        private string GetDiffrentDescription(out bool IsModify)
        {
            IsModify = false;
            StringBuilder decription = new StringBuilder(string.Format("部门名称：{0}；\r\n", _departInfo.depart_name));
            if (_departInfo.depart_name != DepartName)
            {
                IsModify = true;
                decription.Append(string.Format("名称：{0}->{1}，", _departInfo.depart_name, DepartName));
            }
            if (_departInfo.depart_sn != DepartSn)
            {
                IsModify = true;
                decription.Append(string.Format("编号：{0}->{1}，", _departInfo.depart_sn, DepartSn));
            }
            if (_departInfo.contact_phone != DepartPhone)
            {
                IsModify = true;
                decription.Append(string.Format("电话：{0}->{1}，", _departInfo.contact_phone, DepartPhone));
            }
            if (_departInfo.memo != DepartMemo)
            {
                IsModify = true;
                decription.Append(string.Format("备注：{0}->{1}，", _departInfo.memo, DepartMemo));
            }
            if (_departInfo.parent_depart_name != SelectedParentDepart.depart_name)
            {
                IsModify = true;
                decription.Append(string.Format("上级部门：{0}->{1}，", _departInfo.parent_depart_name,
                    SelectedParentDepart.depart_name));
            }
            decription.Remove(decription.Length - 1, 1);
            decription.Append("；\r\n");

            return decription.ToString();
        }

        /// <summary>
        /// 刷新界面内容
        /// </summary>
        private void Refrensh()
        {
            this.DepartName = "";
            this.DepartSn = "";
            this.DepartPhone = "";
            this.DepartMemo = "";
        }

        #endregion

        #region 界面命令响应

        //继续添加按钮的命令响应 by蔡天雨
        private void ContinueAddBtnClicked()
        {
            switch (operateDepartMode)
            {
                case ChildWndOptionMode.Modify:
                    if (DepartName == null || DepartName.Equals(""))
                    {
                        MsgBoxWindow.MsgBox(
                               "部门名称不能为空！",
                               MsgBoxWindow.MsgIcon.Information,
                               MsgBoxWindow.MsgBtns.OK);
                    }
                    else
                    {
                        ModifyDepart();
                    }
                    break;
                case ChildWndOptionMode.Delete:
                    DeleteDepart();
                    break;
                case ChildWndOptionMode.Check:
                    if (CloseEvent != null)
                    {
                        CloseEvent(false);
                    }
                    break;
                case ChildWndOptionMode.Add:
                    if (DepartName == null || DepartName.Equals(""))
                    {
                        MsgBoxWindow.MsgBox(
                               "部门名称不能为空！",
                               MsgBoxWindow.MsgIcon.Information,
                               MsgBoxWindow.MsgBtns.OK);
                    }
                    else 
                    {
                        AddDepart(true);
                    }
                break;
            }
        }

        private void OkBtnClicked()
        {
            switch (operateDepartMode)
            {
                case ChildWndOptionMode.Modify:
                    if (DepartName == null || DepartName.Equals(""))
                    {
                        MsgBoxWindow.MsgBox(
                               "部门名称不能为空！",
                               MsgBoxWindow.MsgIcon.Information,
                               MsgBoxWindow.MsgBtns.OK);
                    }
                    else
                    {
                        ModifyDepart();
                    }
                    break;
                case ChildWndOptionMode.Delete:
                    DeleteDepart();
                    break;
                case ChildWndOptionMode.Check:
                    if (CloseEvent != null)
                    {
                        CloseEvent(false);
                    }
                    break;
                case ChildWndOptionMode.Add:
                    if (DepartName == null || DepartName.Equals(""))
                    {
                        MsgBoxWindow.MsgBox(
                               "部门名称不能为空！",
                               MsgBoxWindow.MsgIcon.Information,
                               MsgBoxWindow.MsgBtns.OK);
                    }
                    else
                    {
                        AddDepart(false);
                    }
                    break;
            }
        }

        private void CancelBtnClicked()
        {
            if (CloseEvent != null)
            {
                CloseEvent(_isContinueAddExcuted);
            }
            
        }
        #endregion

        #region wcf ria 操作
        /// <summary>
        /// 获取部门名称列表
        /// </summary>
        private void GetDeparts(Action riaCallBack = null)
        {
            try
            {
                EntityQuery<UserDepartInfo> list = serviceDomDbAccess.GetDepartsInfoQuery();
                ///回调异常类
                Action<LoadOperation<UserDepartInfo>> actionCallBack = ErrorHandle<UserDepartInfo>.OnLoadErrorCallBack;
                ///异步事件
                LoadOperation<UserDepartInfo> lo = this.serviceDomDbAccess.Load(list, actionCallBack, null);
                lo.Completed += delegate
                {
                    ParentDeparts = new BaseViewModelCollection<UserDepartInfo>();
                    var noneDepart = new  UserDepartInfo();
                    noneDepart.depart_id = -1;
                    noneDepart.depart_name = "无";
                    ParentDeparts.Add(noneDepart);

                    UserDepartInfo curDepart = null;
                    //异步获取数据
                    foreach (UserDepartInfo ar in lo.Entities)
                    {
                        //只有有权限的部门 或者 在修改模式下该部门的父部门  才能显示
                        if (VmDepartMng.OperateDepartIdCollection.Contains(ar.depart_id) ||
                            (_departInfo!=null && ar.depart_id == _departInfo.parent_depart_id &&
                             operateDepartMode == ChildWndOptionMode.Modify))
                        {
                            ParentDeparts.Add(ar);
                            if (_departInfo != null && ar.depart_id == _departInfo.depart_id)
                            {
                                curDepart = ar;
                            }
                        }
                        if (ar.depart_sn != "")
                        {
                            departSnCollection.Add(ar.depart_sn);
                        }
                        if (ar.depart_name != "")
                        {
                            departNamesCollection.Add(ar.depart_name);
                        }
                       
                    }
                    //如果是修改模式，判断重复的部门名称和编号集合中要去掉当前部门
                    if (operateDepartMode == ChildWndOptionMode.Modify)
                    {
                        departNamesCollection.Remove(_departInfo.depart_name);
                        departSnCollection.Remove(_departInfo.depart_sn);
                    }

                    //如果是修改模式，父部门列表中要移除当前部门
                    if (operateDepartMode == ChildWndOptionMode.Modify)
                    {
                        ParentDeparts.Remove(curDepart);
                    }


                    //设置当前选择的父部门
                    if (_departInfo != null)
                    {
                        if (operateDepartMode == ChildWndOptionMode.Add)
                        {
                            foreach (var item in ParentDeparts)
	                        {
                                if (item.depart_id == _departInfo.depart_id)
                                {
                                     this.SelectedParentDepart = item;
                                     break;
                                }
                            }
                        }
                        else
                        {
                            foreach (var item in ParentDeparts)
                            {
                                if (item.depart_id == _departInfo.parent_depart_id)
                                {
                                    this.SelectedParentDepart = item;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        this.SelectedParentDepart = ParentDeparts[0];
                    }

                    if (LoadCompletedEvent != null)
                    {
                        LoadCompletedEvent(this, null);
                        LoadCompletedEvent = null;
                         
                    }

                    if (riaCallBack != null)
                    {
                        riaCallBack();
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

        //添加新部门
        private void AddDepart(bool isContinueAdd)
        {
            DepartSn = DepartSn.Trim();        //移除前置和后置空格
            DepartName = DepartName.Trim();    //移除前置和后置空格
            if (departSnCollection.Contains(DepartSn))
            {
                MsgBoxWindow.MsgBox(
                            "已存在相同的部门编号，请重设！",
                            MsgBoxWindow.MsgIcon.Information,
                            MsgBoxWindow.MsgBtns.OK);
                return;
            }

            if (departNamesCollection.Contains(DepartName))
            {
                MsgBoxWindow.MsgBox(
                            "已存在相同的部门名称，请重设！",
                            MsgBoxWindow.MsgIcon.Information,
                            MsgBoxWindow.MsgBtns.OK);
                return;
            }

            if (isContinueAdd)
            {
                _isContinueAddExcuted = true;
            }

            string depart_sn = PublicMethods.ToString(DepartSn);
            string depart_name = PublicMethods.ToString(DepartName);
            int index = SelectedParentDepart == null ? -1 : SelectedParentDepart.depart_id;
            string parent_depart_id = PublicMethods.ToString(index);
            string phone = PublicMethods.ToString(DepartPhone);
            string memo = PublicMethods.ToString(this.DepartMemo);


       

            try
            {
                WaitingDialog.ShowWaiting();//弹出进度条 by cty
                serviceDomDbAccess = new DomainServiceIriskingAttend();
                EntityQuery<OptionInfo> list = serviceDomDbAccess.AddDepartQuery(
                    depart_name, depart_sn, parent_depart_id, phone, memo);

                ///回调异常类
                Action<LoadOperation<OptionInfo>> actionCallBack = new Action<LoadOperation<OptionInfo>>(ErrorHandle<OptionInfo>.OnLoadErrorCallBack);
                ///异步事件
                LoadOperation<OptionInfo> lo = this.serviceDomDbAccess.Load(list, actionCallBack, null);
                lo.Completed += delegate
                {
                    
                    foreach (OptionInfo item in lo.Entities)
                    {
                        if (!item.isSuccess)
                        {
                            MsgBoxWindow.MsgBox(
                             item.option_info + "！",
                             MsgBoxWindow.MsgIcon.Error,
                             MsgBoxWindow.MsgBtns.OK);
                            WaitingDialog.HideWaiting();

                            //写入操作员日志
                            VmOperatorLog.InsertOperatorLog(0, "添加部门", string.Format("部门名称：{0} ；\r\n", DepartName)+item.option_info , null);
                        }
                        else
                        {
                            if (!item.isNotifySuccess)
                            {
                               
                                MsgBoxWindow.MsgBox(
                                item.option_info + "！",
                                MsgBoxWindow.MsgIcon.Warning,
                                MsgBoxWindow.MsgBtns.OK, (o) =>
                                {
                                  
                                    Action callBack = null;
                                    if (isContinueAdd)
                                    {
                                        callBack = () =>
                                        {
                                            GetDeparts(() =>
                                            {
                                                //写入操作员日志
                                                VmOperatorLog.InsertOperatorLog(1, "添加部门", string.Format("部门名称：{0} ；\r\n", DepartName) + item.option_info, null);
                                                Refrensh();
                                            });
                                        };
                                    }
                                    else
                                    {
                                        callBack = () =>
                                            {
                                                //写入操作员日志
                                                VmOperatorLog.InsertOperatorLog(1, "添加部门", string.Format("部门名称：{0} ；\r\n", DepartName) + item.option_info, null);
                                                Refrensh();
                                            };
                                    }
                                    if (item.tag > 0)
                                    {
                                        //添加新部门的权限
                                        AddDepartPotenceRia(item.tag,DepartName, isContinueAdd, callBack);
                                    }
                                    else
                                    {
                                        GetDeparts(() =>
                                            {
                                                //写入操作员日志
                                                VmOperatorLog.InsertOperatorLog(1, "添加部门", string.Format("部门名称：{0} ；\r\n", DepartName) + item.option_info, null);
                                                Refrensh();
                                            });
                                    }
                                   
                                });
                            }
                            else
                            {
                                Action callBack = null;
                                if (isContinueAdd)
                                {
                                    callBack = () =>
                                    {
                                        GetDeparts(() =>
                                            {
                                                //写入操作员日志
                                                VmOperatorLog.InsertOperatorLog(1, "添加部门", string.Format("部门名称：{0} ；\r\n", DepartName) , null);
                                                Refrensh();
                                            });
                                    };
                                }
                                else
                                {
                                    callBack = () =>
                                        {
                                            //写入操作员日志
                                            VmOperatorLog.InsertOperatorLog(1, "添加部门", string.Format("部门名称：{0} ；\r\n", DepartName), null);
                                            Refrensh();
                                        };
                                }
                                //添加新部门的权限
                                AddDepartPotenceRia(item.tag, DepartName,isContinueAdd, callBack);
                            }
                        }
                        break;
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

        //修改当前部门信息
        private void ModifyDepart()
        {
            DepartSn = DepartSn.Trim();        //移除前置和后置空格
            DepartName = DepartName.Trim();    //移除前置和后置空格
            if (departSnCollection.Contains(DepartSn))
            {
                MsgBoxWindow.MsgBox(
                            "已存在相同的部门编号，请重设",
                            MsgBoxWindow.MsgIcon.Information,
                            MsgBoxWindow.MsgBtns.OK);
                return;
            }

            if (departNamesCollection.Contains(DepartName))
            {
                MsgBoxWindow.MsgBox(
                            "已存在相同的部门名称，请重设",
                            MsgBoxWindow.MsgIcon.Information,
                            MsgBoxWindow.MsgBtns.OK);
                return;
            }


            string depart_sn = PublicMethods.ToString(DepartSn);
            string depart_name = PublicMethods.ToString(DepartName);
            string parent_depart_id;
            //bug2790 修改部门 by蔡天雨//////////////////
            if (this.SelectedParentDepart == null )
            {
                parent_depart_id = PublicMethods.ToString(-1);
            }
            else 
            {
                parent_depart_id = PublicMethods.ToString(SelectedParentDepart.depart_id);
            }
            string phone = PublicMethods.ToString(DepartPhone);
            string memo = PublicMethods.ToString(this.DepartMemo);
            string dID = PublicMethods.ToString(_departInfo.depart_id);

            bool isModify;
            string diff = GetDiffrentDescription(out isModify); //获取修改操作描述 
            if (!isModify) //如果没做任何修改操作 则返回
            {
                if (CloseEvent != null)
                {
                    CloseEvent(true);
                }
                return;
            }

            try
            {
                //异步回调函数，返回后台操作成功或者失败的标志
                Action<InvokeOperation<OptionInfo>> errorCallBack = CallBackHandleControl<OptionInfo>.OnInvokeErrorCallBack;
                CallBackHandleControl<OptionInfo>.m_sendValue = (o) =>
                {
                   
                    if (!o.isSuccess)
                    {
                        MsgBoxWindow.MsgBox(
                          o.option_info + "！",
                          MsgBoxWindow.MsgIcon.Error,
                          MsgBoxWindow.MsgBtns.OK);
                        WaitingDialog.HideWaiting();
                        //写入操作员日志
                        VmOperatorLog.InsertOperatorLog(0, "修改部门", diff + o.option_info, null);

                    }
                    else
                    {
                        if (!o.isNotifySuccess)
                        {
                            MsgBoxWindow.MsgBox(
                             o.option_info + "！",
                             MsgBoxWindow.MsgIcon.Warning,
                             MsgBoxWindow.MsgBtns.OK);
                            //写入操作员日志
                            VmOperatorLog.InsertOperatorLog(1, "修改部门", diff + o.option_info, () =>
                                {
                                    if (CloseEvent != null)
                                    {
                                        CloseEvent(true);
                                    }
                                });
                        }
                        else
                        {
                            MsgBoxWindow.MsgBox(
                             o.option_info + "！",
                             MsgBoxWindow.MsgIcon.Succeed,
                             MsgBoxWindow.MsgBtns.OK);
                            //写入操作员日志
                            VmOperatorLog.InsertOperatorLog(1, "修改部门", diff , () =>
                                {
                                    if (CloseEvent != null)
                                    {
                                        CloseEvent(true);
                                    }
                                });
                        }
                        
                    }

                   
                    WaitingDialog.HideWaiting();
                };


                WaitingDialog.ShowWaiting();
                serviceDomDbAccess.ModifyDepart(dID,
                    depart_name, depart_sn, parent_depart_id, phone, memo, errorCallBack, null);

                
            }
            catch (Exception e)
            {
                WaitingDialog.HideWaiting();
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }

        //删除当前部门
        private void DeleteDepart()
        {
            string dID = PublicMethods.ToString(_departInfo.depart_id);
            string[] dIDs = new string[1];
            dIDs[0] = dID;
            try
            {
                WaitingDialog.ShowWaiting();
                serviceDomDbAccess = new DomainServiceIriskingAttend();
                EntityQuery<OptionInfo> list = serviceDomDbAccess.DeleteDepartQuery(dIDs);

                ///回调异常类
                Action<LoadOperation<OptionInfo>> actionCallBack = new Action<LoadOperation<OptionInfo>>(ErrorHandle<OptionInfo>.OnLoadErrorCallBack);
                ///异步事件
                LoadOperation<OptionInfo> lo = this.serviceDomDbAccess.Load(list, actionCallBack, null);
                lo.Completed += delegate
                {
                    foreach (OptionInfo item in lo.Entities)
                    {
                        if (!item.isSuccess)
                        {
                            MsgBoxWindow.MsgBox(
                              item.option_info + "！",
                              MsgBoxWindow.MsgIcon.Error,
                              MsgBoxWindow.MsgBtns.OK);
                            WaitingDialog.HideWaiting();
                            //写入操作员日志
                            VmOperatorLog.InsertOperatorLog(0, "删除部门", string.Format("部门名称：{0} ；", DepartName) + item.option_info, null);
                     
                        }
                        else
                        {
                            if (!item.isNotifySuccess)
                            {
                                MsgBoxWindow.MsgBox(
                                    item.option_info + "！",
                                    MsgBoxWindow.MsgIcon.Warning,
                                    MsgBoxWindow.MsgBtns.OK);
                                //写入操作员日志
                                VmOperatorLog.InsertOperatorLog(1, "删除部门", string.Format("部门名称：{0} ；", DepartName) + item.option_info, null);
                     
                            }
                            else
                            {
                                MsgBoxWindow.MsgBox(
                                    item.option_info + "！",
                                    MsgBoxWindow.MsgIcon.Succeed,
                                    MsgBoxWindow.MsgBtns.OK);
                                //写入操作员日志
                                VmOperatorLog.InsertOperatorLog(1, "删除部门", string.Format("部门名称：{0} ；", DepartName) + item.option_info, null);
                     
                            }
                            if (CloseEvent != null)
                            {
                                CloseEvent(true);
                            }
                        }

                        break;
                    }
                   
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
        /// 添加部门权限
        /// </summary>
        /// <param name="departId"></param>
        private void AddDepartPotenceRia(int departId,string departName,bool isContinueAdd, Action addDepartPotenceRiaCompleted = null)
        {
            //异步回调函数，返回后台操作成功或者失败的标志
            Action<InvokeOperation<bool>> callBack = CallBackHandleControl<bool>.OnInvokeErrorCallBack;
            CallBackHandleControl<bool>.m_sendValue = (isSucceed) =>
                {
                    if (isSucceed)
                    {
                       
                        //更新部门权限列表
                        VmLogin.UpdateOperatorDepartPotence(() =>
                            {
                                VmDepartMng.UpdateOperateDepartIdCollection();
                                if (!isContinueAdd && CloseEvent != null)
                                {
                                    CloseEvent(true);
                                }
                                WaitingDialog.HideWaiting();
                                if (addDepartPotenceRiaCompleted != null)
                                {
                                    addDepartPotenceRiaCompleted();
                                }

                                MsgBoxWindow.MsgBox("添加部门成功！",
                                  MsgBoxWindow.MsgIcon.Succeed,
                                  MsgBoxWindow.MsgBtns.OK);
                            });
                    }
                    else
                    {
                       
                        MsgBoxWindow.MsgBox(
                              "为新增部门添加权限失败！",
                              MsgBoxWindow.MsgIcon.Error,
                              MsgBoxWindow.MsgBtns.OK, (o) =>
                              {
                                  if (!isContinueAdd && CloseEvent != null)
                                  {
                                      CloseEvent(false);
                                  }
                                  WaitingDialog.HideWaiting();
                                  if (addDepartPotenceRiaCompleted != null)
                                  {
                                      addDepartPotenceRiaCompleted();
                                  }
                              });
                     
                    }
                    
                   
                };

            serviceDomDbAccess = new DomainServiceIriskingAttend();
            serviceDomDbAccess.AddDepartPotence(VmLogin.GetUserName(), departId, callBack, null);
        }

        #endregion

    }

  
}
