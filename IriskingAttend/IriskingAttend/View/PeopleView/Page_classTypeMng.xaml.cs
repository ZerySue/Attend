/*************************************************************************
** 文件名:   Page_classTypeMng.cs
×× 主要类:   Page_classTypeMng
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   cty
** 日  期:   2013-6-14
** 修改人:   
** 日  期:
** 描  述:   Page_classTypeMng类,班制显示页面
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
    public partial class Page_classTypeMng : Page
    {
        public Page_classTypeMng()
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
            var vm = new VmClassTypeMng();
            this.DataContext = vm;
            vm.MarkObj = this.Resources["MarkObject"] as MarkObject;
        }

        //删除
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as VmClassTypeMng;
            if (vm != null)
            {
                UserClassTypeInfo classTypeInfo = ((HyperlinkButton)sender).DataContext as UserClassTypeInfo;
                vm.Delete(classTypeInfo);
            }
        }

        //修改
        private void btnModify_Click(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as VmClassTypeMng;
            if (vm != null)
            {
                UserClassTypeInfo classTypeInfo = ((HyperlinkButton)sender).DataContext as UserClassTypeInfo;
                vm.Modify(classTypeInfo);
            }
        }

        //查询
        private void btnShowDetails_Click(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as VmClassTypeMng;
            if (vm != null)
            {
                UserClassTypeInfo classTypeInfo = ((HyperlinkButton)sender).DataContext as UserClassTypeInfo;
                vm.Check(classTypeInfo);
            }
        }

        //全选或者全取消
        private void chkSelectAll_Click(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as VmClassTypeMng;
            if (vm != null)
            {
                vm.SelectAll(((CheckBox)sender).IsChecked);
            }
        }


        //注册鼠标点击事件
        private void dataGrid1_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.MouseLeftButtonUp -= new MouseButtonEventHandler(Row_MouseLeftButtonUp);
            e.Row.MouseLeftButtonUp += new MouseButtonEventHandler(Row_MouseLeftButtonUp);
        }

        //按需选择item
        void Row_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (dataGrid1 == null || dataGrid1.CurrentColumn == null)
            {
                return;
            }

            if (dataGrid1.CurrentColumn.DisplayIndex == 0)
            {
                if (dataGrid1.SelectedItem != null)
                {
                    var obj = dataGrid1.SelectedItem as UserClassTypeInfo;
                    var vm = this.DataContext as VmClassTypeMng;
                    if (vm != null)
                    {
                        vm.SelectItems(obj);
                    }
                }
            }

            
        }

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
            MyDataGridSortInChinese.OrderData(sender, e, ((VmClassTypeMng)this.DataContext).ClassTypeInfos);
        }

        #endregion

        private void ExportExl_Click(object sender, RoutedEventArgs e)
        {
            string space = "                              ";
            ExpExcel.ExportExcelFromDataGrid(this.dataGrid1, 2, 4, (space + "班制管理" + space), "班制管理");
        }

        

    }
}
