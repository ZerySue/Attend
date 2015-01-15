/*************************************************************************
** 文件名:   ChildWnd_AddChildDepart.cs
×× 主要类:   ChildWnd_AddChildDepart
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   cty
** 日  期:   2013-6-14
** 修改人:   
** 日  期:
** 描  述:   ChildWnd_AddChildDepart类,添加子部门页面
**
** 版  本:   1.0.0
** 备  注:  命名及代码编写遵守C#编码规范
**
 * ***********************************************************************/

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
using IriskingAttend.ViewModel.PeopleViewModel;
using Irisking.Web.DataModel;
using IriskingAttend.Common;
using System.Windows.Data;


namespace IriskingAttend.View.PeopleView
{
    /// <summary>
    /// 添加子部门窗口UI后台代码
    /// </summary>
    public partial class ChildWnd_AddChildDepart : ChildWindow
    {
        Action<bool?> Callback;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="callback">窗口关闭的回调函数句柄</param>
        /// <param name="dInfo">部门信息</param>
        public ChildWnd_AddChildDepart(Action<bool?> callback,UserDepartInfo dInfo)
        {
            InitializeComponent();
            var vm = new VmChildWndAddChildDepart( dInfo);
            vm.LoadCompletedEvent += new EventHandler(vm_LoadCompletedEvent);
            vm.CloseEvent += new Action<bool>(vm_CloseEvent);

            Callback = callback;

        }

        /// <summary>
        /// 窗口关闭事件
        /// </summary>
        /// <param name="obj"></param>
        void vm_CloseEvent(bool obj)
        {
            this.DialogResult = obj;
            if (Callback != null) Callback(this.DialogResult);
        }

        /// <summary>
        /// viewModel加载完毕回调函数
        /// </summary>
        /// <param name="sender">viewModel实体</param>
        /// <param name="e"></param>
        void vm_LoadCompletedEvent(object sender, EventArgs e)
        {
            this.DataContext = sender;
            ((VmChildWndAddChildDepart)sender).MarkObj = this.Resources["MarkObject"] as MarkObject;
        }

        //全选
        private void chkSelectAll_Click(object sender, RoutedEventArgs e)
        {
            VmChildWndAddChildDepart vm = this.DataContext as VmChildWndAddChildDepart;
            if (vm != null)
            {
                vm.SelectAll(((CheckBox)sender).IsChecked);
            }
        }

        //注册鼠标事件
        private void DG_SelectedPerson_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.MouseLeftButtonUp -= new MouseButtonEventHandler(Row_MouseLeftButtonUp);
            e.Row.MouseLeftButtonUp += new MouseButtonEventHandler(Row_MouseLeftButtonUp);
        }

        //点击行 选择item操作
        void Row_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.DataContext != null)
            {
                if (SelectedDeparts.CurrentColumn.DisplayIndex == 0)
                {
                    if (SelectedDeparts.SelectedItem != null)
                    {
                        var obj = SelectedDeparts.SelectedItem as UserDepartInfo;
                        ((VmChildWndAddChildDepart)this.DataContext).SelectItems(obj);
                    }
                }  
            }
        }

        #region 排序 by cty



        private void DG_SelectedPerson_MouseMove(object sender, MouseEventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        private void DG_SelectedPerson_LayoutUpdated(object sender, EventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }


        private void DG_SelectedPerson_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MyDataGridSortInChinese.OrderData(sender, e, ((VmChildWndAddChildDepart)this.DataContext).DepartsInfo);
        }

        #endregion

    }
}

