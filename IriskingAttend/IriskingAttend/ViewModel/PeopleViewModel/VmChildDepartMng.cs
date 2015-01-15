/*************************************************************************
** 文件名:   VmChildDepartMng.cs
×× 主要类:   VmChildDepartMng
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   cty
** 日  期:   2013-6-14
** 修改人:   wz 代码优化
** 日  期:   2013-7-19
** 描  述:   VmChildDepartMng类,子部门管理
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
using System.Linq;
using System.Text;
using IriskingAttend.ViewModel.SystemViewModel;

namespace IriskingAttend.ViewModel.PeopleViewModel
{
    /// <summary>
    /// 子部门管理ViewModel
    /// </summary>
    public class VmChildDepartMng : BaseViewModel
    {
        
        #region  字段声明
        
        /// <summary>
        /// 与服务声明
        /// </summary>
        private DomainServiceIriskingAttend serviceDomDbAccess = new DomainServiceIriskingAttend();

        /// <summary>
        /// 与之关联的导航服务
        /// </summary>
        private NavigationService navigationService;

        /// <summary>
        /// 加载完毕事件
        /// </summary>
        public event EventHandler LoadCompletedEvent;

        /// <summary>
        /// 对应的父部门信息
        /// </summary>
        private UserDepartInfo parentDepart;

        #endregion

        #region    与界面绑定的属性

        /// <summary>
        /// 增加一个新子部门命令
        /// </summary>
        public DelegateCommand AddChildDepartCommand { get; set; }
        
        /// <summary>
        /// 返回按钮命令
        /// </summary>
        public DelegateCommand BackBtnCommand { get; set; }

        private BaseViewModelCollection<UserDepartInfo> childDepartInfos;

        /// <summary>
        /// 子部门信息列表
        /// </summary>
        public BaseViewModelCollection<UserDepartInfo> ChildDepartInfos
        {
            get { return childDepartInfos; }
            set
            {
                childDepartInfos = value;
                this.NotifyPropertyChanged("ChildDepartInfos");
            }
        }
        
        private string departName;

        /// <summary>
        /// 父部门名称
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
       
        private string departSn;

        /// <summary>
        ///  父部门编号
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

        #endregion

        #region 构造函数
        
        public VmChildDepartMng(NavigationService _NavigationService )
        {
            AddChildDepartCommand = new DelegateCommand(AddChildDepart);
            BackBtnCommand  = new DelegateCommand(BackBtnClick);

            this.navigationService = _NavigationService;
            IsolatedStorageSettings appSetting = IsolatedStorageSettings.ApplicationSettings;
            if (appSetting.Contains("ParentDepartInfo"))
            {
                parentDepart = appSetting["ParentDepartInfo"] as UserDepartInfo;
            }
            DepartName = parentDepart.depart_name;
            DepartSn = parentDepart.depart_sn;

            VmDepartMng.UpdateOperateDepartIdCollection();

            GetChildDepartsInfo();
        }

        #endregion

        #region 该页面下子窗口的回调函数
       
        /// <summary>
        /// 添加子部门回调函数
        /// </summary>
        /// <param name="res">子窗口关闭的标志</param>
        private void ChildWnd_AddChildDepart_CallBack(bool? res)
        {
            if (res.HasValue && res.Value)
            {
                //重新查询子部门
                serviceDomDbAccess = new DomainServiceIriskingAttend();
                GetChildDepartsInfo();
            }
        }

        #endregion

        #region 界面事件响应

        /// <summary>
        /// 添加子部门操作
        /// </summary>
        private void  AddChildDepart()
        {
            //Action<bool?> callb = new Action<bool?>(ChildWnd_AddChildDepart_CallBack);
            var cw = new ChildWnd_AddChildDepart(ChildWnd_AddChildDepart_CallBack, parentDepart);
            cw.Show();
        }

        /// <summary>
        /// 返回操作 
        /// </summary>
        private void BackBtnClick()
        {
            this.navigationService.Navigate(new Uri("/Page_departMng", UriKind.RelativeOrAbsolute));
        }

        #endregion

        #region wcf ria操作
        
        /// <summary>
        /// 获取子部门信息
        /// </summary>
        private void GetChildDepartsInfo()
        {
            try
            {
                string pID = PublicMethods.ToString(parentDepart.depart_id);
                EntityQuery<UserDepartInfo> list = serviceDomDbAccess.GetChildDepartQuery(pID);
                ///回调异常类
                Action<LoadOperation<UserDepartInfo>> actionCallBack = new Action<LoadOperation<UserDepartInfo>>(ErrorHandle<UserDepartInfo>.OnLoadErrorCallBack);
                ///异步事件
                LoadOperation<UserDepartInfo> lo = this.serviceDomDbAccess.Load(list, actionCallBack, null);
                lo.Completed += delegate
                {
                    ChildDepartInfos = new BaseViewModelCollection<UserDepartInfo>();

                    //异步获取数据
                    foreach (UserDepartInfo ar in lo.Entities)
                    {
                        ChildDepartInfos.Add(ar);
                    }

                 
                  
                    if (LoadCompletedEvent != null)
                    {
                        LoadCompletedEvent(this, null);
                    }
                        
                };
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
                
            }
        
        }
       
        /// <summary>
        /// 删除子部门
        /// </summary>
        /// <param name="child"></param>
        private void DeleteChildDepart(UserDepartInfo child)
        {
            string parentDepartID = PublicMethods.ToString(parentDepart.depart_id);
            string childDepartID = PublicMethods.ToString(child.depart_id);
            string[] strs = new string[1];
            strs[0] = childDepartID;

            try
            {
                //获取操作描述
                string description = GetDeleteChildDepartDescription(parentDepart,child);

                serviceDomDbAccess = new DomainServiceIriskingAttend();
                EntityQuery<OptionInfo> list = serviceDomDbAccess.DeleteChildDepartQuery(parentDepartID, strs);

                ///回调异常类
                Action<LoadOperation<OptionInfo>> actionCallBack = new Action<LoadOperation<OptionInfo>>(ErrorHandle<OptionInfo>.OnLoadErrorCallBack);
                ///异步事件
                LoadOperation<OptionInfo> lo = this.serviceDomDbAccess.Load(list, actionCallBack, null);
                lo.Completed += delegate
                {
                    
                    IriskingAttend.ViewModel.SystemViewModel.VmOperatorLog.CompleteCallBack completeCallBack
                        = ()=>
                            {
                                //重新查询数据库
                                this.serviceDomDbAccess = new DomainServiceIriskingAttend();
                                GetChildDepartsInfo();
                            };

                    foreach (OptionInfo item in lo.Entities)
                    {
                        if (!item.isSuccess)
                        {
                            MsgBoxWindow.MsgBox(
                              item.option_info + "！",
                              MsgBoxWindow.MsgIcon.Error,
                              MsgBoxWindow.MsgBtns.OK);
                            VmOperatorLog.InsertOperatorLog(0, "删除子部门", description + item.option_info, completeCallBack);
                        }
                        else
                        {
                            if (!item.isNotifySuccess)
                            {
                                MsgBoxWindow.MsgBox(
                                     item.option_info + "！",
                                     MsgBoxWindow.MsgIcon.Warning,
                                     MsgBoxWindow.MsgBtns.OK);
                                VmOperatorLog.InsertOperatorLog(1, "删除子部门", description + item.option_info, completeCallBack);
                            }
                            else
                            {
                                MsgBoxWindow.MsgBox(
                                     item.option_info + "！",
                                     MsgBoxWindow.MsgIcon.Succeed,
                                     MsgBoxWindow.MsgBtns.OK);
                                VmOperatorLog.InsertOperatorLog(1, "删除子部门", description , completeCallBack);
                            }
                           
                        }

                        break;
                    }
                 
                };
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }

        #endregion

        #region 给view层提供的接口函数

        /// <summary>
        /// 删除子部门 
        /// </summary>
        /// <param name="sender">当前部门信息</param>
        public void Delete(UserDepartInfo departInfo)
        {

            MsgBoxWindow.MsgBox(
                "请注意，将进行如下操作：\n\r删除子部门！",
                MsgBoxWindow.MsgIcon.Warning,
                MsgBoxWindow.MsgBtns.OKCancel,
                (e) =>
                {
                    if (e == MsgBoxWindow.MsgResult.OK)
                    {
                        DeleteChildDepart(departInfo);
                    }
                });

        }

        
        #endregion

        #region 私有功能函数

        /// <summary>
        /// 获取删除子部门关系操作的描述
        /// </summary>
        /// <returns></returns>
        private string GetDeleteChildDepartDescription(UserDepartInfo parent,UserDepartInfo child)
        {
            string description = string.Format("父部门名称：{0}；\r\n子部门名称：{1}\r\n",
                parent.depart_name, child.depart_name);
                
            return description;
        }

      
        #endregion

    }

   


}
