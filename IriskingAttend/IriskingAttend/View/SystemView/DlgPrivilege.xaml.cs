/*************************************************************************
** 文件名:   DlgPrivilege.cs
** 主要类:   DlgPrivilege
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2013-8-6
** 修改人:   
** 日  期:
** 描  述:   DlgPrivilege类,修改操作员权限窗口
**
** 版  本:   1.0.0
** 备  注:  命名及代码编写遵守C#编码规范
**
*************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using IriskingAttend.ViewModel.SystemViewModel;
using Irisking.Web.DataModel;
using System.Windows.Data;

namespace IriskingAttend.Dialog
{
    public partial class DlgPrivilege : ChildWindow
    {
        #region 变量

        /// <summary>
        /// vm 变量
        /// </summary>
        private VmPrivilege _vmPrivilege; 

        /// <summary>
        /// 操作员名称
        /// </summary>
        private string _operatorName = "";

        /// <summary>
        /// 回调函数变量
        /// </summary>
        private CloseCallBack _closeCallBack;

        /// <summary>
        /// 回调函数声明
        /// </summary>
        /// <param name="dialogResult"></param>
        public delegate void CloseCallBack( bool? dialogResult);

        #endregion

        #region 构造函数        

        /// <summary>
        /// 构造函数，修改权限时使用
        /// </summary>
        /// <param name="operatorName">操作员名称</param>
        /// <param name="modify">是否为修改权限</param>
        public DlgPrivilege(string operatorName, bool modify)
        {
            InitializeComponent();

            if (modify)
            {
                _vmPrivilege = new VmPrivilege(operatorName);
            }
            else
            {
                _vmPrivilege = new VmPrivilege();
            }

            //数据绑定
            this.DataContext = _vmPrivilege;

            _operatorName = operatorName;

            //窗口关闭事件
            this.Closed += new EventHandler(DlgPrivilege_Closed);

            //vm加载完毕事件
            _vmPrivilege.LoadCompletedEvent += (sender, ergs) =>
            {    
                //解决初始化时 部门树的 scrollbar遮挡问题
                this.Dispatcher.BeginInvoke(() =>
                {   
                    //手动绑定树节点
                    this.BindingAllTree(treeViewDepart);

                    //手动绑定树节点
                    this.BindingAllTree(treeViewPurview);
                });
            };
        }

        #endregion

        #region 辅助函数
        /// <summary>
        /// 手动绑定整个树  
        /// 在树初始化完成之后调用该函数     
        /// </summary>
        /// <param name="node"></param>
        private void BindingAllTree(TreeView node)
        {
            if (node == null)
            {
                return;
            }

            //激活树,用于遍历函数GetDescendantContainers
            //遍历所有树节点Containers
            //如果不激活，则只能遍历最上层节点
            node.ExpandAll();
            node.UpdateLayout();
            node.CollapseAll();


            IEnumerable<TreeViewItem> _treeChildren = TreeViewExtensions.GetDescendantContainers((TreeView)node);
            int cout = _treeChildren.Count();
            foreach (TreeViewItem item in _treeChildren)
            {
                //手动绑定
                //绑定isExpended属性
                Binding bingding = new Binding();
                bingding.Path = new System.Windows.PropertyPath("IsOpen");
                bingding.Source = item.DataContext;
                bingding.Mode = BindingMode.TwoWay;
                item.SetBinding(TreeViewItem.IsExpandedProperty, bingding);               
            }
        }
        #endregion

        #region 事件响应

        /// <summary>
        /// 窗口关闭事件：回调事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DlgPrivilege_Closed(object sender, EventArgs e)
        {
            if (_closeCallBack != null)
            {
                _closeCallBack(this.DialogResult);
            }
        }

        /// <summary>
        /// 部门权力复选框点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkDepartSelected_Click(object sender, RoutedEventArgs e)
        {
            _vmPrivilege.ChangeDepartTreeCheckStatus((TreeNode<UserDepartInfo>)this.treeViewDepart.SelectedItem);
        }

        /// <summary>
        /// 权限复选框点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkPurviewSelected_Click(object sender, RoutedEventArgs e)
        {
            _vmPrivilege.ChangePurviewTreeCheckStatus((TreeNode<purview>)this.treeViewPurview.SelectedItem);
        }

        /// <summary>
        /// 确定按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (_vmPrivilege.AddPrivilege(_operatorName))
            {
                this.DialogResult = true;
            }
        }

        /// <summary>
        /// 取消按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        /// <summary>
        /// 对话框显示函数重载
        /// </summary>
        /// <param name="closeCallBack">回调函数</param>
        public void Show(CloseCallBack closeCallBack)
        {
            this._closeCallBack = closeCallBack;
            base.Show();
        }

        #endregion
    }
}

