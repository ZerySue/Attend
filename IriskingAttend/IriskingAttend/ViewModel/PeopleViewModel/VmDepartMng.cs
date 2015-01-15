/*************************************************************************
** 文件名:   VmDepartMng.cs
×× 主要类:   VmDepartMng
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   cty
** 日  期:   2013-6-14
** 修改人:   wz 代码优化
** 日  期:   2013-7-24
** 描  述:   VmDepartMng类,部门信息显示
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
using IriskingAttend.Dialog;
using System.Windows.Navigation;
using IriskingAttend.View;
using IriskingAttend.BehaviorSelf;
using System.Linq;
using System.Text;
using IriskingAttend.ViewModel.SystemViewModel;

namespace IriskingAttend.ViewModel.PeopleViewModel
{
    public class VmDepartMng : BaseViewModel
    {

        #region 静态字段 部门权限
        static HashSet<int> _operateDepartIdCollection = null;
        
        /// <summary>
        /// 部门权限集合
        /// 以hashset的数据形式给出
        /// </summary>
        public static HashSet<int> OperateDepartIdCollection
        {
            get
            {
                if (_operateDepartIdCollection == null || _operateDepartIdCollection.Count == 0)
                {
                    _operateDepartIdCollection = new HashSet<int>();
                    foreach (var item in VmLogin.OperatorDepartIDList)
                    {
                        _operateDepartIdCollection.Add(item);
                    }
                }
                return _operateDepartIdCollection;
            }
        }

        /// <summary>
        /// 更新部门权限集合
        /// </summary>
        public static void UpdateOperateDepartIdCollection()
        {
            _operateDepartIdCollection = new HashSet<int>();
            foreach (var item in VmLogin.OperatorDepartIDList)
            {
                _operateDepartIdCollection.Add(item);
            }
        }
       

        #endregion

        #region 字段声明

        /// <summary>
        /// 与服务声明
        /// </summary>
        private DomainServiceIriskingAttend _serviceDomDbAccess = new DomainServiceIriskingAttend();


        //导航服务. 该对象绑定不了
        private NavigationService _navigationService;

        /// <summary>
        /// 部门信息加载完成
        /// </summary>
        public event EventHandler DepartInfoLoadCompleted;

        #endregion

        #region    与界面绑定的属性

        /// <summary>
        /// 添加新部门命令
        /// </summary>
        public DelegateCommand AddDepartCommand
        { 
            get; 
            set; 
        }
        
        /// <summary>
        /// 批量删除部门命令
        /// </summary>
        public DelegateCommand BatchDeleteCommand
        { 
            get;
            set; 
        }
        
        private BaseViewModelCollection<UserDepartInfo> departInfos;
        /// <summary>
        /// 部门信息列表
        /// </summary>
        public BaseViewModelCollection<UserDepartInfo>  DepartInfos
        {
            get 
            { 
                return departInfos; 
            }
            set
            {
                departInfos = value;
                this.OnPropertyChanged(()=>this.DepartInfos);
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


        private bool _isBatchOperateBtnEnable;
        /// <summary>
        /// 批量操作按钮的Enable属性
        /// </summary>
        public bool IsBatchOperateBtnEnable
        {
            get { return _isBatchOperateBtnEnable; }
            set
            {
                _isBatchOperateBtnEnable = value;
                this.OnPropertyChanged(() => this.IsBatchOperateBtnEnable);
            }
        }

        #endregion

        #region 构造函数

        public VmDepartMng(NavigationService _navigationService)
        {
            AddDepartCommand = new DelegateCommand(AddDepart);
            BatchDeleteCommand = new DelegateCommand(BatchDelete);
            this._navigationService = _navigationService;

            //事件初始化
            DepartInfoLoadCompleted += (a, e) => { };

            GetDepartsInfosRia();

            //更新权限集合列表
            UpdateOperateDepartIdCollection();
        }

        public VmDepartMng()
        {
            AddDepartCommand = new DelegateCommand(AddDepart);
            BatchDeleteCommand = new DelegateCommand(BatchDelete);

            //更新权限集合列表
            UpdateOperateDepartIdCollection();

            //事件初始化
            DepartInfoLoadCompleted += (a, e) => { };
        }

        #endregion

        #region 回调函数
        
        /// <summary>
        /// 子窗口返回的回调函数
        /// </summary>
        /// <param name="DialogResult"></param>
        private void DepartOperate_callback(bool? DialogResult)
        {
            if (DialogResult.HasValue && DialogResult.Value)
            {
                //重新查询数据库
                _serviceDomDbAccess = new DomainServiceIriskingAttend();
                GetDepartsInfosRia();
            }
        }

        #endregion

        #region 界面事件响应

        //增加部门
        private void AddDepart()
        {
            var cw = new ChildWnd_OperateDepart(null , ChildWndOptionMode.Add, DepartOperate_callback);
            cw.Show();
        }

        //删除部门
        private void BatchDelete()
        {
            List<UserDepartInfo> selectedDeparts = new List<UserDepartInfo>();
            foreach (var item in DepartInfos)
            {
                if (item.isSelected)
                {
                    selectedDeparts.Add(item);
                }
            }
            if (selectedDeparts.Count > 0)
            {
                MsgBoxWindow.MsgBox(
                         "请注意，您将进行如下操作\r\n批量删除部门！",
                         Dialog.MsgBoxWindow.MsgIcon.Warning,
                         Dialog.MsgBoxWindow.MsgBtns.OKCancel,
                         (e) =>
                         {
                             if (e == MsgBoxWindow.MsgResult.OK)
                             {
                                 BatchDeleteRia(selectedDeparts.ToArray());
                             }
                         });
            }
            else
            {
                MsgBoxWindow.MsgBox(
                        "至少选择一个选项！",
                        Dialog.MsgBoxWindow.MsgIcon.Information, Dialog.MsgBoxWindow.MsgBtns.OK);
            }
        }

       

        #endregion

        #region wcf ria操作

        /// <summary>
        /// 获取部门信息列表
        /// </summary>
        public void GetDepartsInfosRia()
        {
            try
            {
                WaitingDialog.ShowWaiting();
                ServiceDomDbAcess.ReOpenSever();
                EntityQuery<UserDepartInfo> list = ServiceDomDbAcess.GetSever().GetDepartsInfoQuery();
                ///回调异常类
                Action<LoadOperation<UserDepartInfo>> actionCallBack = new Action<LoadOperation<UserDepartInfo>>(ErrorHandle<UserDepartInfo>.OnLoadErrorCallBack);
                ///异步事件
                LoadOperation<UserDepartInfo> lo = ServiceDomDbAcess.GetSever().Load(list, actionCallBack, null);
                lo.Completed += delegate
                {
                   
                    DepartInfos = new BaseViewModelCollection<UserDepartInfo>();
                  

                    //异步获取数据
                    foreach (UserDepartInfo ar in lo.Entities)
                    {
                        //只有有权限的部门才能显示
                        if (VmDepartMng.OperateDepartIdCollection.Contains(ar.depart_id))
                        {
                            DepartInfos.Add(ar);
                        }
                    }
                    if (this.MarkObj != null)
                    {
                        this.MarkObj.Selected = CheckIsAllSelected();
                    }
                    this.IsBatchOperateBtnEnable = CheckIsAnyOneSelected();

                    //部门信息加载完成后，发送事件
                    DepartInfoLoadCompleted(this, new EventArgs());

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
        /// 批量删除指定部门
        /// </summary>
        /// <param name="IDs"></param>
        private void BatchDeleteRia(UserDepartInfo[] Infos)
        {
            //获取操作描述
            StringBuilder description = new StringBuilder();
            List<string> IDs_str = new List<string>();
            for (int i = 0; i < Infos.Length; i++)
            {
                IDs_str.Add(PublicMethods.ToString(Infos[i].depart_id));
                description.Append(string.Format("部门名称：{0}；", Infos[i].depart_name));
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
                        IriskingAttend.ViewModel.SystemViewModel.VmOperatorLog.CompleteCallBack completeCallBack =
                            () =>
                            {
                                //重新查询数据库                                
                                GetDepartsInfosRia();
                            };

                        if (!item.isSuccess)
                        {
                            MsgBoxWindow.MsgBox(
                               item.option_info+"！",
                               Dialog.MsgBoxWindow.MsgIcon.Error, Dialog.MsgBoxWindow.MsgBtns.OK);
                            //写入操作员日志
                            VmOperatorLog.InsertOperatorLog(0, "删除部门", description + item.option_info, completeCallBack);
                        }
                        else
                        {
                            if (!item.isNotifySuccess)
                            {
                                MsgBoxWindow.MsgBox(
                                     item.option_info + "！",
                                     Dialog.MsgBoxWindow.MsgIcon.Warning, Dialog.MsgBoxWindow.MsgBtns.OK);
                                //写入操作员日志
                                VmOperatorLog.InsertOperatorLog(1, "删除部门", description + item.option_info, completeCallBack);
                            }
                            else
                            {
                                MsgBoxWindow.MsgBox(
                                   item.option_info + "！",
                                   Dialog.MsgBoxWindow.MsgIcon.Succeed, Dialog.MsgBoxWindow.MsgBtns.OK);
                                //写入操作员日志
                                VmOperatorLog.InsertOperatorLog(1, "删除部门", description.ToString(), completeCallBack);
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

        #endregion

        #region 给view层提供的接口函数

        /// <summary>
        /// 选中全部人员或者取消选中
        /// </summary>
        /// <param name="sender">checkBox对象</param>
        public void SelectAll(bool? IsChecked)
        {
            if (IsChecked.Value)
            {
                foreach (var item in DepartInfos)
                {
                    item.isSelected = true;
                }
            }
            else
            {
                foreach (var item in DepartInfos)
                {
                    item.isSelected = false;
                }
            }
            this.IsBatchOperateBtnEnable = CheckIsAnyOneSelected();
            this.MarkObj.Selected = CheckIsAllSelected();
        }

        /// <summary>
        /// 按需选择datagrid的Items
        /// </summary>
        /// <param name="sender">datagrid对象</param>
        public void SelectItems(UserDepartInfo departInfo)
        {
            departInfo.isSelected = !departInfo.isSelected;
            this.IsBatchOperateBtnEnable = CheckIsAnyOneSelected();
            this.MarkObj.Selected = CheckIsAllSelected();
        }

        /// <summary>
        ///  查看部门
        /// </summary>
        /// <param name="departInfo">部门信息</param>
        public void Check(UserDepartInfo departInfo)
        {
            
            var cw = new ChildWnd_OperateDepart(departInfo, ChildWndOptionMode.Check, DepartOperate_callback);
            cw.Show();
         }

        /// <summary>
        ///  修改部门
        /// </summary>
        /// <param name="departInfo">部门信息</param>
        public void Modify(UserDepartInfo departInfo)
        {
           
            var cw = new ChildWnd_OperateDepart(departInfo, ChildWndOptionMode.Modify, DepartOperate_callback);
            cw.Show();
        }

        /// <summary>
        /// 删除部门
        /// </summary>
        /// <param name="departInfo">部门信息</param>
        public void Delete(UserDepartInfo departInfo)
        {
            MsgBoxWindow.MsgBox(string.Format("请注意，您将进行如下操作:\r\n删除部门【{0}】！", departInfo.depart_name),
                 MsgBoxWindow.MsgIcon.Warning, MsgBoxWindow.MsgBtns.OKCancel, (result) =>
                 {
                     if (result == MsgBoxWindow.MsgResult.OK)
                     {
                         UserDepartInfo[] departs = new UserDepartInfo[1];
                         departs[0] = departInfo;
                         BatchDeleteRia(departs);
                     }
                 });
        }

        /// <summary>
        /// 查看子部门
        /// </summary>
        /// <param name="sender"></param>
        public void CheckChild(UserDepartInfo departInfo)
        {

            IsolatedStorageSettings appSetting = IsolatedStorageSettings.ApplicationSettings;
            //DepartName
            if (!appSetting.Contains("ParentDepartInfo"))
            {
                appSetting.Add("ParentDepartInfo", departInfo);
            }
            else
            {
                appSetting["ParentDepartInfo"] = departInfo;
            }

            this._navigationService.Navigate(new Uri("/Page_childDepartMng", UriKind.RelativeOrAbsolute));
        }

        
        #endregion

        #region 私有功能函数

        /// <summary>
        /// 检查批量操作按钮的可见性
        /// </summary>
        private bool CheckIsAnyOneSelected()
        {
            
            foreach (var item in DepartInfos)
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
            if (DepartInfos.Count == 0)
            {
                return false;
            }
            foreach (var item in DepartInfos)
            {
                if (!item.isSelected)
                {
                    return false;
                }
            }
            return true;

        }

        #endregion

    }

  

}
