/*************************************************************************
** 文件名:   AttendLeaveManage.cs
** 主要类:   AttendLeaveManage
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   cty
** 日  期:   2013-10-28
** 修改人:   
** 日  期:
** 描  述:   AttendLeaveManage，考勤类型管理页面
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
using System.Windows.Navigation;
using IriskingAttend.ViewModel;
using Irisking.Web.DataModel;
using IriskingAttend.ExportExcel;
using IriskingAttend.Dialog;
using IriskingAttend.Common;

namespace IriskingAttend.View.AttendView
{
    public partial class AttendLeaveManage : Page
    {
        #region 私有变量

        //请假数据层
        private VmLeaveType _vmLeaveType = new VmLeaveType();

        #endregion

        #region 构造函数 初始化

        /// <summary>
        /// 构造函数
        /// </summary>
        public AttendLeaveManage()
        {
            InitializeComponent();

            //全选绑定初始化
            _vmLeaveType.MarkObj = this.Resources["MarkObject"] as MarkObject;

            _vmLeaveType.GetLeaveType(50);

            this.DataContext = _vmLeaveType;

            dgLeaveType.ItemsSource = _vmLeaveType.LeaveTypeModel;

            dgLeaveType.LoadingRow += (a, e) =>
            {
                int index = e.Row.GetIndex();
                var cell = dgLeaveType.Columns[1].GetCellContent(e.Row) as TextBlock;
                cell.Text = (index + 1).ToString();
            };
        }

        // 当用户导航到此页面时执行。
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        #endregion

        /// <summary>
        /// 修改操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnModify_Click(object sender, RoutedEventArgs e)
        {
            ((VmLeaveType)this.DataContext).ModifyLeaveType();//((HyperlinkButton)sender).DataContext as LeaveType);
        }

        /// <summary>
        /// 删除操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        { 
            ((VmLeaveType)this.DataContext).DeleteLeaveType();
        }

        #region 导出excel

        /// <summary>
        /// 导出excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportExl_Click(object sender, RoutedEventArgs e)
        {
            string space = "                         ";
            ExpExcel.ExportExcelFromDataGrid(this.dgLeaveType, 2, 5, (space + "请假类型管理" + space), "请假类型管理");
        }

        #endregion

        #region 复选框选取操作

        /// <summary>
        /// 复选框选取操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgLeaveType_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.MouseLeftButtonUp -= new MouseButtonEventHandler(Row_MouseLeftButtonUp);
            e.Row.MouseLeftButtonUp += new MouseButtonEventHandler(Row_MouseLeftButtonUp);
        }

        private void Row_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (dgLeaveType.CurrentColumn.DisplayIndex == 0)
            {
                if (dgLeaveType.SelectedItem != null)
                {
                    var obj = dgLeaveType.SelectedItem as LeaveType;
                    ((VmLeaveType)this.DataContext).SelectItems(obj);
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
            VmLeaveType vm = this.DataContext as VmLeaveType;
            if (vm != null)
            {
                vm.SelectAll(((CheckBox)sender).IsChecked);
            }
        }

        #endregion

        #region 排序

        private void dgLeaveType_LayoutUpdated(object sender, EventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        private void dgLeaveType_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MyDataGridSortInChinese.OrderData(sender, e, ((VmLeaveType)this.DataContext).LeaveTypeModel);
        }

        private void dgLeaveType_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        private void dgLeaveType_MouseMove(object sender, MouseEventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        #endregion

    }
}
