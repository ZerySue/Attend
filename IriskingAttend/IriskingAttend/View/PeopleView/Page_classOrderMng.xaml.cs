/*************************************************************************
** 文件名:   Page_classOrderMng.cs
×× 主要类:   Page_classOrderMng
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   cty
** 日  期:   2013-6-14
** 修改人:   
** 日  期:
** 描  述:   Page_classOrderMng类,班次显示页面
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
using IriskingAttend.ViewModel.PeopleViewModel;
using IriskingAttend.Common;
using IriskingAttend.ExportExcel;
using Irisking.Web.DataModel;

namespace IriskingAttend.View.PeopleView
{
    public partial class Page_classOrderMng : Page
    {
        public Page_classOrderMng()
        {
            InitializeComponent();
            //添加序号 不能采用绑定的方式，绑定的话排序后序号会跟着变化by cty
            this.dataGrid1.LoadingRow += (a, e) =>
            {
                int index = e.Row.GetIndex();
                var cell = dataGrid1.Columns[1].GetCellContent(e.Row) as TextBlock;
                cell.Text = (index + 1).ToString();
            };
        }

        // 当用户导航到此页面时执行。
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var vm = new VmClassOrderMng();
            vm.MarkObj = this.Resources["MarkObject"] as MarkObject;
            vm.LoadCompletedEvent += new EventHandler(vm_LoadCompletedEvent);
           
        }

        //viewModel加载完毕事件
        void vm_LoadCompletedEvent(object sender, EventArgs e)
        {
            this.DataContext = sender;

        }

        #region 界面按钮点击响应事件
        
        //删除
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as VmClassOrderMng;
            if (vm != null)
            {
                vm.Delete(sender);
            }
        }

        //修改
        private void btnModify_Click(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as VmClassOrderMng;
            if (vm != null)
            {
                vm.Modify(sender);
            }
        }

        //查询
        private void btnShowDetails_Click(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as VmClassOrderMng;
            if (vm != null)
            {
                vm.Check(sender);
            }
        }
        
        //全选或者全取消
        private void chkSelectAll_Click(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as VmClassOrderMng;
            if (vm != null)
            {
                vm.SelectAll(((CheckBox)sender).IsChecked);
            }
        }

        //导出excel
        private void ExportExl_Click(object sender, RoutedEventArgs e)
        {
            string space = "                                                                    ";
            ExpExcel.ExportExcelFromDataGrid(this.dataGrid1, 2, 12, (space + "班次管理" + space), "班次管理");
        }

        #endregion

        #region  按需选择Item

        //注册dataGrid行鼠标点击事件
        private void dataGrid1_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.MouseLeftButtonUp -= new MouseButtonEventHandler(Row_MouseLeftButtonUp);
            e.Row.MouseLeftButtonUp += new MouseButtonEventHandler(Row_MouseLeftButtonUp);
        }
        //点击行 按需选择item操作
        void Row_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.DataContext != null)
            {

                if (dataGrid1.CurrentColumn.DisplayIndex == 0)
                {
                    if (dataGrid1.SelectedItem != null)
                    {
                        var obj = dataGrid1.SelectedItem as UserClassOrderInfo;
                        ((VmClassOrderMng)this.DataContext).SelectItems(obj);
                    }
                }
               
            }
        }

        #endregion

        #region 排序 by cty


        private void dataGrid1_MouseMove(object sender, MouseEventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        private void dataGrid1_LayoutUpdated(object sender, EventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        private void dataGrid1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MyDataGridSortInChinese.OrderData(sender, e, ((VmClassOrderMng)this.DataContext).ClassOrderInfos);
        }

        #endregion

        

     
    }
}
