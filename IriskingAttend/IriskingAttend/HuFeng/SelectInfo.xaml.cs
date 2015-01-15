/*************************************************************************
** 文件名:   SelectInfo.cs
** 主要类:   SelectInfo
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2014-6-19
** 修改人:   
** 日  期:
** 描  述:   SelectInfo，查询条件多选类
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
using IriskingAttend.ViewModel;
using Irisking.Web.DataModel;
using IriskingAttend.Common;
using IriskingAttend.View;

namespace IriskingAttend
{
    public partial class SelectInfo : ChildWindow
    {
        #region 变量声明

        /// <summary>
        /// 私有变量声明
        /// </summary>
        VmSelectInfoFilter _vmFilter = new VmSelectInfoFilter();

        /// <summary>
        /// 回调函数
        /// </summary>
        private Action<BaseViewModelCollection<SelectInfoFilter>, TextBox> _callback;

        private TextBox _txtShow;

        #endregion

        public SelectInfo(VmSelectInfoFilter vmFilter, TextBox txtShow, Action<BaseViewModelCollection<SelectInfoFilter>, TextBox> callback)
        {
            InitializeComponent();

            this._vmFilter = vmFilter;

            this.DataContext = _vmFilter;

            //全选绑定初始化
            _vmFilter.MarkObj = this.Resources["MarkObject"] as MarkObject;
            
            dgSelectInfo.ItemsSource = _vmFilter.InfoFilterModel;

            this._callback = callback;

            this._txtShow = txtShow;

            ////列表序号
            //this.dgSelectInfo.LoadingRow += (a, e) =>
            //{
            //    int index = e.Row.GetIndex();
            //    var cell = dgSelectInfo.Columns[1].GetCellContent(e.Row) as TextBlock;
            //    cell.Text = (index + 1).ToString();
            //};
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            _callback(_vmFilter.InfoFilterModel, _txtShow);
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        #region 复选框选取操作
        /// <summary>
        /// 复选框选取操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgSelectInfo_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            dgSelectInfo.MouseLeftButtonUp -= new MouseButtonEventHandler(Row_MouseLeftButtonUp);
            dgSelectInfo.MouseLeftButtonUp += new MouseButtonEventHandler(Row_MouseLeftButtonUp);
        }

        private void Row_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (dgSelectInfo.CurrentColumn.DisplayIndex == 0)
            {
                if (dgSelectInfo.SelectedItem != null)
                {
                    var obj = dgSelectInfo.SelectedItem as SelectInfoFilter;
                    ((VmSelectInfoFilter)this.DataContext).SelectItemInfoFilter(obj);
                }
            }
        }

        #endregion

        #region 全选操作

        /// <summary>
        /// 全选操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkSelectAll_Click(object sender, RoutedEventArgs e)
        {
            VmSelectInfoFilter vm = this.DataContext as VmSelectInfoFilter;
            if (vm != null)
            {
                vm.SelectAllInfoFilter(((CheckBox)sender).IsChecked);
            }
        }

        #endregion

        #region 排序
        private void dgSelectInfo_LayoutUpdated(object sender, EventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        private void dgSelectInfo_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MyDataGridSortInChinese.OrderData(sender, e, ((VmSelectInfoFilter)this.DataContext).InfoFilterModel);
        }

        private void dgSelectInfo_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        private void dgSelectInfo_MouseMove(object sender, MouseEventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }
        #endregion 
    }
}

