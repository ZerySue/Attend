/*************************************************************************
** 文件名:   VmPrivilege.cs
** 主要类:   VmPrivilege.cs
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2013-8-8
** 修改人:   
** 日  期:   
** 描  述:   VmPrivilege类,权限设置
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
using IriskingAttend.ApplicationType;

namespace IriskingAttend.ViewModel.SystemViewModel
{
    public class VmPrivilege : BaseViewModel
    {
        #region 变量
        
        /// <summary>
        /// 域服务声明
        /// </summary>
        private DomainServiceIriskingAttend _domSrvDbAccess = new DomainServiceIriskingAttend();

        /// <summary>
        /// 选中的部门ID列表集合
        /// </summary>
        private List<int> _selectDepartIDList = new List<int>();

        /// <summary>
        /// 选中的权限ID列表集合
        /// </summary>
        private List<int> _selectPurviewIDList = new List<int>();

        /// <summary>
        /// 回调函数变量
        /// </summary>
        private GetPrivilegeCallBack _getPrivilegeCallBack;

        /// <summary>
        /// 回调函数声明
        /// </summary>
        public delegate void GetPrivilegeCallBack();

        /// <summary>
        /// vm加载完毕事件
        /// </summary>
        public event EventHandler LoadCompletedEvent;
        
        #endregion

        #region 属性

        /// <summary>
        /// 部门权力树
        /// </summary>
        private TreeNode<UserDepartInfo> _departPotenceTreeData = null;        
        public BaseViewModelCollection<TreeNode<UserDepartInfo>> DepartPotenceTreeData
        {
            get
            {
                if (_departPotenceTreeData == null)
                {
                    return null;
                }
                return _departPotenceTreeData.Children;
            }
            set
            {
                _departPotenceTreeData.Children = value;
                this.OnPropertyChanged<BaseViewModelCollection<TreeNode<UserDepartInfo>>>(() => this.DepartPotenceTreeData);
            }
        }        

        /// <summary>
        /// 权限树
        /// </summary>
        private TreeNode<purview> _purviewTreeData = null;        
        public BaseViewModelCollection<TreeNode<purview>> PurviewTreeData
        {
            get
            {
                if (_purviewTreeData == null)
                {
                    return null;
                }
                return _purviewTreeData.Children;
            }
            set
            {
                _purviewTreeData.Children = value;
                this.OnPropertyChanged<BaseViewModelCollection<TreeNode<purview>>>(() => this.PurviewTreeData);
            }
        }

        /// <summary>
        /// 全部部门列表
        /// </summary>
        private BaseViewModelCollection<UserDepartInfo> _departInfo;
        public BaseViewModelCollection<UserDepartInfo> DepartInfo
        {
            get
            {
                return _departInfo;
            }
            set
            {
                _departInfo = value;
                this.OnPropertyChanged<BaseViewModelCollection<UserDepartInfo>>(() => this.DepartInfo);
            }
        }

        /// <summary>
        /// 全部权限列表
        /// </summary>
        private BaseViewModelCollection<purview> _purview = null;        
        public BaseViewModelCollection<purview> Purview
        {
            get 
            { 
                return _purview; 
            }
            set
            {
                _purview = value;
                this.OnPropertyChanged<BaseViewModelCollection<purview>>(() => this.Purview); 
            }
        }

        /// <summary>
        /// 当前操作员部门权力列表集合
        /// </summary>
        private List<int> _operatorDepartIDList = new List<int>();
        public List<int> OperatorDepartIDList
        {
            get
            {
                return _operatorDepartIDList;
            }
        }

        /// <summary>
        /// 当前操作员权限列表集合
        /// </summary>
        private List<int> _operatorPurviewIDList = new List<int>();
        public List<int> OperatorPurviewIDList
        {
            get
            {
                return _operatorPurviewIDList;
            }
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 添加操作员时设定操作员权限用此构造函数。只需获得部门权力树与权限树即可。
        /// </summary>
        public VmPrivilege()
        {
            GetDepartPotenceRia();             
        }

        /// <summary>
        /// 修改操作员权限时用此构造函数。不光要获得部门权力树与权限树，还要获得当前修改的操作员部门权力及权限。
        /// </summary>
        /// <param name="operatorName">当前修改的操作员名称</param>
        public VmPrivilege(string operatorName)
        {
            GetOperatorPurviewRia(operatorName);
        }

        /// <summary>
        /// 获得部门权力树与权限树及当前修改的操作员部门权力及权限，获取完毕后，调用回调函数
        /// </summary>
        /// <param name="operatorName">当前修改的操作员名称</param>
        public VmPrivilege(string operatorName, GetPrivilegeCallBack getPrivilegeCallBack )
        {
            this._getPrivilegeCallBack = getPrivilegeCallBack;
            GetOperatorPurviewRia(operatorName);
        }

        #endregion

        #region 公共函数，外部调用接口

        /// <summary>
        /// 添加操作员部门权力与权限
        /// </summary>
        /// <param name="operatorName">操作员名称</param>
        public bool AddPrivilege(string operatorName)
        {
            //获得选中的部门权力ID列表
            _selectDepartIDList.Clear();
            foreach (var item in DepartPotenceTreeData.ElementAt(0).Children)
            {
                GetDepartNodeCheckList(item);
            }

            //获得选中的权限ID列表
            _selectPurviewIDList.Clear();
            foreach (var item in PurviewTreeData.ElementAt(0).Children)
            {
                GetPurviewNodeCheckList(item);
            }

            if (_selectPurviewIDList.Count <= 0)
            {
                MsgBoxWindow.MsgBox("权限不能为空！", MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);
                return false;
            }

            SetOperDepartPotenceAndPurvewRia(operatorName, _selectDepartIDList, _selectPurviewIDList);
            return true;
        }

        #endregion

        #region view层调用公共函数

        /// <summary>
        /// 改变部门权力树的选中状态
        /// </summary>        
        /// <param name="nodeDepart">部门节点</param>
        public void ChangeDepartTreeCheckStatus(TreeNode<UserDepartInfo> nodeDepart)
        {
            //先将子节点的状态改为与父节点一致
            ChildValueAsParentValue(nodeDepart);

            //然后再改变父节点的状态
            SetParentCheckState(DepartPotenceTreeData.ElementAt(0));
        }

        /// <summary>
        /// 改变权限树的选中状态
        /// </summary>
        /// <param name="nodePurview">权限节点</param>
        public void ChangePurviewTreeCheckStatus(TreeNode<purview> nodePurview)
        {
            //先将子节点的状态改为与父节点一致
            ChildValueAsParentValue(nodePurview);

            //然后再改变父节点的状态
            SetParentCheckState(PurviewTreeData.ElementAt(0));
        }

        #endregion

        #region  通过ria连接后台，数据库相关操作

        /// <summary>
        /// ria方式获得当前操作员的权限
        /// </summary>
        /// <param name="operatorName">操作员名称</param>
        private void GetOperatorPurviewRia(string operatorName)
        {
            try
            {
                WaitingDialog.ShowWaiting();

                EntityQuery<operator_purview> operatorPurviewList = _domSrvDbAccess.GetOperatorPurviewQuery(operatorName);
                ///回调异常类
                Action<LoadOperation<operator_purview>> actionCallBack = ErrorHandle<operator_purview>.OnLoadErrorCallBack;
                ///异步事件
                LoadOperation<operator_purview> lo = this._domSrvDbAccess.Load(operatorPurviewList, actionCallBack, null);

                lo.Completed += delegate
                {
                    _operatorPurviewIDList.Clear();
                    //异步获取数据
                    foreach (operator_purview ar in lo.Entities)
                    {
                        _operatorPurviewIDList.Add(ar.purview_id);
                    }                   

                    GetOperatorDepartPotenceRia(operatorName);
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
        /// ria方式获得当前操作员的部门权力
        /// </summary>
        /// <param name="operatorName">操作员名称</param>
        private void GetOperatorDepartPotenceRia(string operatorName)
        {
            try
            {
                WaitingDialog.ShowWaiting();

                EntityQuery<operator_potence> list = _domSrvDbAccess.GetOperatorDepartPotenceQuery(operatorName);
                ///回调异常类
                Action<LoadOperation<operator_potence>> actionCallBack = ErrorHandle<operator_potence>.OnLoadErrorCallBack;
                ///异步事件
                LoadOperation<operator_potence> lo = this._domSrvDbAccess.Load(list, actionCallBack, null);

                lo.Completed += delegate
                {
                    _operatorDepartIDList.Clear();
                    //异步获取数据
                    foreach (operator_potence ar in lo.Entities)
                    {
                        _operatorDepartIDList.Add(ar.depart_id);
                    }                   

                    GetDepartPotenceRia();
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
        /// 获取部门权力列表        
        /// </summary>
        private void GetDepartPotenceRia()
        {
            try
            {
                WaitingDialog.ShowWaiting();

                EntityQuery<UserDepartInfo> list = _domSrvDbAccess.GetDepartsInfoQuery();
                ///回调异常类
                Action<LoadOperation<UserDepartInfo>> actionCallBack =ErrorHandle<UserDepartInfo>.OnLoadErrorCallBack;
                ///异步事件
                LoadOperation<UserDepartInfo> lo = this._domSrvDbAccess.Load(list, actionCallBack, null);

                lo.Completed += delegate
                {

                    DepartInfo = new BaseViewModelCollection<UserDepartInfo>();
                    //异步获取数据
                    foreach (UserDepartInfo ar in lo.Entities)
                    {
                        DepartInfo.Add(ar);                
                    }

                    _departPotenceTreeData = new TreeNode<UserDepartInfo>();

                    //存储为树形结构                    
                    DepartPotenceTreeData = new BaseViewModelCollection<TreeNode<UserDepartInfo>>();
                    
                    //添加总部门
                    TreeNode<UserDepartInfo> departAll = new TreeNode<UserDepartInfo>();
                    departAll.Children = new BaseViewModelCollection<TreeNode<UserDepartInfo>>();
                    departAll.NodeValue = null;

                    //总部门默认为展开
                    departAll.IsOpen = true;  
                    departAll.GetNodeNameDelegate += () =>
                    {
                        return "全部";
                    };

                    DepartPotenceTreeData.Add(departAll);
                    CreateDepartTree(-1, departAll.Children);
                    ChildTrueAsParentTrueDepartPotence(DepartPotenceTreeData.ElementAt(0));
                    SetParentCheckAndOpenState(DepartPotenceTreeData.ElementAt(0));

                    GetPurviewRia();
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
        /// 获取权限列表        
        /// </summary>
        private void GetPurviewRia()
        {
            try
            {
                WaitingDialog.ShowWaiting();

                EntityQuery<purview> list = _domSrvDbAccess.GetPurviewQuery();
                ///回调异常类
                Action<LoadOperation<purview>> actionCallBack = ErrorHandle<purview>.OnLoadErrorCallBack;
                ///异步事件
                LoadOperation<purview> lo = this._domSrvDbAccess.Load(list, actionCallBack, null);

                lo.Completed += delegate
                {
					WaitingDialog.HideWaiting();

                    Dictionary<AbstractApp.PrivilegeENUM, bool> _dictPrivilegeList = AppTypePublic.GetCustomAppType().GetDictPrivilegeListVisible();
                    Purview = new BaseViewModelCollection<purview>();
                    //异步获取数据
                    foreach (purview ar in lo.Entities)
                    {
                        if (_dictPrivilegeList.ContainsKey((AbstractApp.PrivilegeENUM)ar.purview_id) && _dictPrivilegeList[(AbstractApp.PrivilegeENUM)ar.purview_id])
                        {
                            Purview.Add(ar);
                        }
                    }

                    _purviewTreeData = new TreeNode<purview>();
                    //存储为树形结构                    
                    PurviewTreeData = new BaseViewModelCollection<TreeNode<purview>>();
                    
                    //添加总部门
                    TreeNode<purview> purviewAll = new TreeNode<purview>();
                    purviewAll.Children = new BaseViewModelCollection<TreeNode<purview>>();
                    purviewAll.NodeValue = null;

                    //总权限默认为展开
                    purviewAll.IsOpen = true;  
                    purviewAll.GetNodeNameDelegate += () =>
                    {
                        return "全部";
                    };

                    PurviewTreeData.Add(purviewAll);
                    CreatePurviewTree(-1, purviewAll.Children);
                    ChildTrueAsParentTruePurview(PurviewTreeData.ElementAt(0));
                    SetParentCheckAndOpenState(PurviewTreeData.ElementAt(0));

                    if (this._getPrivilegeCallBack != null)
                    {
                        this._getPrivilegeCallBack();
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
        /// 设置当前操作员的部门权力与权限，将其写入数据库
        /// </summary>
        /// <param name="operatorName">当前操作员名称</param>
        /// <param name="departIDList">部门权力列表</param>
        /// <param name="purviewIDList">权限列表</param>
        public void SetOperDepartPotenceAndPurvewRia(string operatorName, List<int> departIDList, List<int> purviewIDList)
        {
            try
            {
                Action<InvokeOperation<bool>> onInvokeErrCallBack = CallBackHandleControl<bool>.OnInvokeErrorCallBack;
                CallBackHandleControl<bool>.m_sendValue = (o) =>
                {
                    WaitingDialog.HideWaiting();

                    //异步获取数据                    
                    if (o)
                    {
                        MsgBoxWindow.MsgBox("设置操作员权限成功！", MsgBoxWindow.MsgIcon.Succeed, MsgBoxWindow.MsgBtns.OK);
                    }
                    else
                    {
                        MsgBoxWindow.MsgBox("设置操作员权限失败，请重试！", MsgBoxWindow.MsgIcon.Error, MsgBoxWindow.MsgBtns.OK);
                    }                    
                };
                WaitingDialog.ShowWaiting();

                //将当前操作员的部门权力与权限写入数据库
                _domSrvDbAccess.SetOperDepartPotenceAndPurvew(operatorName, departIDList, purviewIDList, onInvokeErrCallBack, null);
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }

        #endregion

        #region 私有功能函数

        /// <summary>
        /// 将父部门当做根节点，创建部门树。
        /// 用递归方法调用
        /// </summary>
        /// <param name="parentDepartID">父部门id</param>
        /// <param name="treeNode">根节点</param>
        private void CreateDepartTree(long parentDepartID, BaseViewModelCollection<TreeNode<UserDepartInfo>> treeNode)
        {
            IEnumerable<UserDepartInfo> departsTmp = this.DepartInfo.Where<UserDepartInfo>((info) =>
            {
                return info.parent_depart_id == parentDepartID;
            });

            if (departsTmp == null)
            {
                return;
            }

            foreach (UserDepartInfo item in departsTmp)
            {
                TreeNode<UserDepartInfo> node = new TreeNode<UserDepartInfo>();
                node.NodeValue = item;

                //若当前部门权力表里包含此部门，则将此部门的选中状态设为选中
                if (_operatorDepartIDList.Contains(item.depart_id))
                {
                    node.IsChecked = true;
                }

                node.GetNodeNameDelegate += () =>
                {
                    return ((UserDepartInfo)node.NodeValue).depart_name;
                };

                node.Children = new BaseViewModelCollection<TreeNode<UserDepartInfo>>();
                treeNode.Add(node);
                CreateDepartTree(item.depart_id, node.Children);
            }
        }

        /// <summary>
        /// 将父权限当做根节点，创建权限树
        /// 用递归方法调用
        /// </summary>
        /// <param name="parentPurviewID">父权限id</param>
        /// <param name="treeNode">根节点</param>
        private void CreatePurviewTree(long parentPurviewID, BaseViewModelCollection<TreeNode<purview>> treeNode)
        {
            IEnumerable<purview> purviewTmp = this.Purview.Where<purview>((info) =>
            {
                return info.parent_purview_id == parentPurviewID;
            });

            if (purviewTmp == null)
            {
                return;
            }

            foreach (purview item in purviewTmp)
            {
                TreeNode<purview> node = new TreeNode<purview>();
                node.NodeValue = item;
                //若当前操作员权限表里包含此权限，则将此权限的选中状态设为选中
                if (  _operatorPurviewIDList.Contains(item.purview_id))
                {
                    node.IsChecked = true;
                }
                node.GetNodeNameDelegate += () =>
                {
                    return ((purview)node.NodeValue).purview_name;
                };

                node.Children = new BaseViewModelCollection<TreeNode<purview>>();
                treeNode.Add(node);
                CreatePurviewTree(item.purview_id, node.Children);
            }
        }

        /// <summary>
        /// 当树的父节点为选中状态时，将子节点也设为选中状态，适合在初始化树的
        /// 状态时使用。当此操作员有此父权限（权力）时，必定拥有此父权限的所有子权限（权力）。
        /// 但是，当树的父节点为未选中状态时，不能将子节点设为未选中状态。
        /// </summary>        
        /// <param name="nodeChild">子节点</param>
        private void ChildTrueAsParentTruePurview(TreeNode<purview> nodeChild)
        {
            //递归退出出口
            if (nodeChild.Children.Count == 0)
            {
                return;
            }

            foreach (var item in nodeChild.Children)
            {
                //当此节点被选中时，此子节点的子节点也设为选中状态
                if (nodeChild.IsChecked)
                {
                    item.IsChecked = nodeChild.IsChecked;
                    if (!_operatorPurviewIDList.Contains(item.NodeValue.purview_id))
                    {
                        _operatorPurviewIDList.Add(item.NodeValue.purview_id);
                    }
                }

                ChildTrueAsParentTruePurview(item);
            }
        }

        /// <summary>
        /// 当树的父节点为选中状态时，将子节点也设为选中状态，适合在初始化树的
        /// 状态时使用。当此操作员有此父权限（权力）时，必定拥有此父权限的所有子权限（权力）。
        /// 但是，当树的父节点为未选中状态时，不能将子节点设为未选中状态。
        /// </summary>        
        /// <param name="nodeChild">子节点</param>
        private void ChildTrueAsParentTrueDepartPotence(TreeNode<UserDepartInfo> nodeChild)
        {
            //递归退出出口
            if (nodeChild.Children.Count == 0)
            {
                return;
            }

            foreach (var item in nodeChild.Children)
            {
                //当此节点被选中时，此子节点的子节点也设为选中状态
                if (nodeChild.IsChecked)
                {
                    item.IsChecked = nodeChild.IsChecked;
                    if (!_operatorDepartIDList.Contains(item.NodeValue.depart_id))
                    {
                        _operatorDepartIDList.Add(item.NodeValue.depart_id);
                    }
                }

                ChildTrueAsParentTrueDepartPotence(item);
            }
        }

        /// <summary>
        /// 递归改变父节点树的选中与展开状态
        /// 当父节点的所有子节点为选中状态时，父节点才为选中状态。
        /// 当父节点的所有子节点的选中状态相同时（都为选中或者未选中），父节点处于未展开状态，否则将父节点展开。
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="nodeChild">子节点</param>
        /// <returns>修改后的父节点树的选中状态</returns>
        private bool SetParentCheckAndOpenState<T>(TreeNode<T> nodeChild)
        {
            //递归退出出口
            if (nodeChild.Children.Count == 0)
            {
                return nodeChild.IsChecked;
            }

            //是否此父节点的所有的子节点都为选中状态，只要有一个子节点处于未选中状态，则此就为false
            bool childAllChecked = true;

            //是否此父节点所有的子节点都为未选中状态，只要有一个子节点处于选中状态，则此就为true
            bool childAllUnChecked = false;

            bool childAllUnOpened = true;

            foreach (var item in nodeChild.Children)
            {
                //只要有一个子节点处于未选中，则父节点就处于未选中状态
                if (false == SetParentCheckAndOpenState(item))
                {
                    childAllChecked = false;
                }
                //有一个子节点处于选中状态时
                else
                {
                    childAllUnChecked = true;
                }

                if (item.IsOpen)
                {
                    childAllUnOpened = false;
                }
            }

            //当所有子节点都处于选中状态时，父节点处于选中状态
            //nodeChild.IsChecked = childAllChecked;

            if (childAllUnOpened)
            {
                //当所有子节点并未都处于选中状态或未选中状态时，父节点处于展开状态
                if (!nodeChild.IsChecked && childAllUnChecked)
                {
                    nodeChild.IsOpen = true;
                }
            }            
            else
            {
                nodeChild.IsOpen = true;
            }
            
            return childAllChecked;
        }

        /// <summary>
        /// 递归改变父节点树的状态
        /// 当父节点的所有子节点为选中状态时，父节点才为选中状态。
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="nodeChild">子节点</param>
        /// <returns>修改后的父节点树的选中状态</returns>
        private bool SetParentCheckState<T>(TreeNode<T> nodeChild)
        {
            //递归退出出口
            if (nodeChild.Children.Count == 0)
            {
                return nodeChild.IsChecked;
            }

            //是否此父节点的所有的子节点都为选中状态，只要有一个子节点处于未选中状态，则此就为false
            bool childAllChecked = true;
            foreach (var item in nodeChild.Children)
            {
                //只要有一个子节点处于未选中，则父节点就处于未选中状态
                if (false == SetParentCheckState(item))
                {
                    childAllChecked = false;

                    //不能退出，否则不能遍历所有子节点
                    //break;
                }
            }
            if (!childAllChecked)
            {
                nodeChild.IsChecked = childAllChecked;
            }

            return childAllChecked;
        }

        /// <summary>
        /// 当父节点的状态发生改变时，递归改变子节点的状态。
        /// 父节点选中，所有子节点被选中；父节点未选中，所有子节点未选中
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="nodeChild">欲改变的子节点树</param>
        private void ChildValueAsParentValue<T>(TreeNode<T> nodeChild)
        {
            //递归退出出口
            if (nodeChild.Children.Count == 0)
            {
                return;
            }

            //将所有子节点的状态改为与父节点一致
            foreach (var item in nodeChild.Children)
            {
                item.IsChecked = nodeChild.IsChecked;
                ChildValueAsParentValue(item);
            }
        }

        /// <summary>
        /// 获得部门权力树子节点的设置状态
        /// </summary>
        /// <param name="nodeDepart">子部门节点</param>
        private void GetDepartNodeCheckList(TreeNode<UserDepartInfo> nodeDepart)
        {
            if( nodeDepart.IsChecked )
            {
                _selectDepartIDList.Add( nodeDepart.NodeValue.depart_id );
                return;
            }

            if (nodeDepart.Children.Count == 0)
            {
                return;
            }

            foreach (var item in nodeDepart.Children)
            {
                GetDepartNodeCheckList(item);
            }
        }

        /// <summary>
        /// 获得权限树子节点的设置状态
        /// </summary>
        /// <param name="nodePurview">子权限节点</param>
        private void GetPurviewNodeCheckList(TreeNode<purview> nodePurview)
        {
            if (nodePurview.IsChecked)
            {
                _selectPurviewIDList.Add(nodePurview.NodeValue.purview_id);
                return;
            }

            if (nodePurview.Children.Count == 0)
            {
                return;
            }

            foreach (var item in nodePurview.Children)
            {
                GetPurviewNodeCheckList(item);
            }
        }

        #endregion

    }

}
