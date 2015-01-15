/*************************************************************************
** 文件名:   VmloginPublic.cs
** 主要类:   Vmlogin
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2013-8-19
** 修改人:   
** 日  期:
** 描  述:   Vmlogin,登录
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
using IriskingAttend.Dialog;
using IriskingAttend.ViewModel;
using IriskingAttend.ViewModel.SystemViewModel;
using System.Linq;
using IriskingAttend.ApplicationType;

namespace IriskingAttend
{
    public partial class VmLogin : BaseViewModel
    {
        #region 变量   

        /// <summary>
        /// 权限vm
        /// </summary>
        private static VmPrivilege _vmPrivilege;

        /// <summary>
        /// 登录用户名
        /// </summary>
        private static string _userName = "";

        /// <summary>
        /// 登录用户ID
        /// </summary>
        private static int _userID = 0;

        /// <summary>
        /// 当前操作员部门权力列表集合
        /// </summary>
        private static List<int> _operatorDepartIDList = new List<int>();

        /// <summary>
        /// 当前操作员部门权力信息列表集合
        /// </summary>
        private static List<UserDepartInfo> _operatorDepartInfoList = new List<UserDepartInfo>();

        /// <summary>
        /// 当前操作员权限列表集合
        /// </summary>
        private static List<int> _operatorPurviewIDList = new List<int>();

        /// <summary>
        /// 权限树
        /// </summary>
        private static BaseViewModelCollection<TreeNode<purview>> _purviewTreeData;

        //回调函数声明
        public delegate void CompleteCallBack();

        #endregion

        #region  全局公共函数、变量

        /// <summary>
        /// 当前操作员部门权力列表集合
        /// </summary>
        public static List<int> OperatorDepartIDList
        {
            get
            {
                return _operatorDepartIDList;
            }
        }

        /// <summary>
        /// 当前操作员部门权力信息列表集合
        /// </summary>
        public static List<UserDepartInfo> OperatorDepartInfoList
        {
            get
            {
                return _operatorDepartInfoList;
            }
        }

        /// <summary>
        /// 当前操作员权限列表集合
        /// </summary>
        public static List<int> OperatorPurviewIDList
        {
            get
            {
                return _operatorPurviewIDList;
            }
        }

        /// <summary>
        /// 权限树
        /// </summary>
        public static BaseViewModelCollection<TreeNode<purview>> PurviewTreeData
        {
            get
            {
                return _purviewTreeData;
            }
        }

        /// <summary>
        /// 权限是否可见的值对应表
        /// </summary>
        public static Dictionary<AbstractApp.PrivilegeENUM, bool> DictPrivilege = new Dictionary<AbstractApp.PrivilegeENUM, bool>();
        

        /// <summary>
        /// 获得当前登录用户是否为超级用户
        /// </summary>
        /// <returns>true：超级用户 false：非超级用户</returns>
        public static bool GetIsSuperUser()
        {
            return ( GetUserName() == "admin" );
        }

        /// <summary>
        /// 获得操作员是否为超级用户
        /// </summary>
        /// <param name="operatorName">操作员名字</param>
        /// <returns>true：超级用户 false：非超级用户</returns>
        public static bool IsSuperUser( string operatorName )
        {
            return (operatorName == "admin");
        }

        /// <summary>
        /// 获得登录的用户名
        /// </summary>
        /// <returns>登录用户名</returns>
        public static string GetUserName()
        {
            return _userName;
        }

        /// <summary>
        /// 获得登录的用户id
        /// </summary>
        /// <returns>登录id</returns>
        public static int GetUserID()
        {
            return _userID;
        }
        /// <summary>
        /// 获得是否是矿山应用程序
        /// </summary>
        /// <returns>false：非矿山  true：矿山</returns>
        public static bool GetIsMineApplication()
        {
            return AppTypePublic.GetIsMineApplication();
        }

        #endregion    

        #region 私有辅助函数

        /// <summary>
        /// 获得所有权限是否可见
        /// </summary>       
        private static void GetPrivilegeVisualable()
        {
            DictPrivilege.Clear();
            GetNodeVisualable(VmLogin.PurviewTreeData.ElementAt(0));
        }

        /// <summary>
        /// 获得权限是否可见
        /// </summary>
        /// <param name="nodeChildren">权限树节点</param>
        private static void GetNodeVisualable(TreeNode<purview> nodeChildren)
        {
            if( nodeChildren.Children.Count <= 0 )
            {
                return;
            }
            foreach (var item in nodeChildren.Children)
            {
                bool value = item.IsChecked || item.IsOpen;
                DictPrivilege.Add((AbstractApp.PrivilegeENUM)item.NodeValue.purview_id, value);              

                GetNodeVisualable(item);
            }           
        }
        
        /// <summary>
        /// 设置超级用户所有权限可见
        /// </summary>
        /// <param name="nodeChildren"></param>
        private static void SetSuperUserVisualable(TreeNode<purview> nodeChildren)
        {
            if (nodeChildren.Children.Count <= 0)
            {
                return;
            }
            foreach (var item in nodeChildren.Children)            
            {
                DictPrivilege.Add((AbstractApp.PrivilegeENUM)item.NodeValue.purview_id, true);
                SetSuperUserVisualable(item);
            }
        }

        /// <summary>
        /// 私有辅助函数：将获得的权限重新赋给公共接口
        /// </summary>
        private static void ReLoadPrivilege()
        {
            _operatorDepartIDList = _vmPrivilege.OperatorDepartIDList;
            _operatorPurviewIDList = _vmPrivilege.OperatorPurviewIDList;
            _purviewTreeData = _vmPrivilege.PurviewTreeData;

            _operatorDepartInfoList.Clear();
            foreach (UserDepartInfo item in _vmPrivilege.DepartInfo)
            {                
                if (_operatorDepartIDList.Contains(item.depart_id))
                {
                    _operatorDepartInfoList.Add(item);
                }
            }
            GetPrivilegeVisualable();

            if (VmLogin.GetIsSuperUser())
            {
                _operatorDepartIDList.Clear();
                _operatorDepartInfoList.Clear();
                foreach (UserDepartInfo item in _vmPrivilege.DepartInfo)
                {
                    _operatorDepartIDList.Add(item.depart_id);
                    _operatorDepartInfoList.Add(item);
                }

                _operatorPurviewIDList.Clear();
                foreach (purview item in _vmPrivilege.Purview)
                {
                    _operatorPurviewIDList.Add(item.purview_id);
                }

                DictPrivilege.Clear();
                SetSuperUserVisualable(VmLogin.PurviewTreeData.ElementAt(0));
            }
        }
        #endregion

        #region ria方式调用后台        

        /// <summary>
        /// 更新当前操作员的部门权力
        /// </summary>
        public static void UpdateOperatorDepartPotence(CompleteCallBack completeCallBack)
        {
            try
            {
                _vmPrivilege = new VmPrivilege(VmLogin.GetUserName(), () =>
                {
                    ReLoadPrivilege();

                    if (completeCallBack != null)
                    {
                        completeCallBack();
                    }
                });
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
