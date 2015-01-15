/*************************************************************************
** 文件名:   Page_departMng.cs
×× 主要类:   Page_departMng
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   cty
** 日  期:   2013-6-14
** 修改人:   
** 日  期:
** 描  述:   Page_departMng类，部门显示页面
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
using System.ServiceModel.DomainServices.Client;
using System.Windows.Data;
using Irisking.Web.DataModel;



namespace IriskingAttend.View.PeopleView
{
    public partial class PageDepartMng : Page
    {
        public PageDepartMng()
        {
            InitializeComponent();
            //添加序号 不能采用绑定的方式，绑定的话排序后序号会跟着变化by cty
            this.dataGridDepart.LoadingRow += (a, e) =>
            {
                int index = e.Row.GetIndex();
                var cell = dataGridDepart.Columns[1].GetCellContent(e.Row) as TextBlock;
                cell.Text = (index + 1).ToString();
            };

            dataGridDepart.AddHandler(MouseLeftButtonUpEvent, new MouseButtonEventHandler(dataGridDepart_MouseLeftButtonUp), true);

        }

        // 当用户导航到此页面时执行。
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            VmDepartMng vm = new VmDepartMng(this.NavigationService);
            this.DataContext = vm;
            vm.MarkObj = this.Resources["MarkObject"] as MarkObject;
        }

        //订阅每一行的鼠标点击事件
        private void dataGridDepart_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.MouseLeftButtonUp -= new MouseButtonEventHandler(Row_MouseLeftButtonUp);
            e.Row.MouseLeftButtonUp += new MouseButtonEventHandler(Row_MouseLeftButtonUp);
        }

        //datagrid每一行的鼠标点击事件
        void Row_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (dataGridDepart.CurrentColumn.DisplayIndex == 0)
            {
                if (dataGridDepart.SelectedItem != null)
                {
                    var obj = dataGridDepart.SelectedItem as UserDepartInfo;
                    ((VmDepartMng)this.DataContext).SelectItems(obj);
                }
            }
        }

        //全选datagird的Item
        private void chkSelectAll_Click(object sender, RoutedEventArgs e)
        {             
            ((VmDepartMng)this.DataContext).SelectAll(((CheckBox)sender).IsChecked);
        }

        //查看部门信息
        private void btnShowDetails_Click(object sender, RoutedEventArgs e)
        {
            UserDepartInfo departInfo = ((HyperlinkButton)sender).DataContext as UserDepartInfo;
            ((VmDepartMng)this.DataContext).Check(departInfo);
        }

        //修改部门
        private void btnModify_Click(object sender, RoutedEventArgs e)
        {
            UserDepartInfo departInfo = ((HyperlinkButton)sender).DataContext as UserDepartInfo;
            ((VmDepartMng)this.DataContext).Modify(departInfo);
        }

        //删除部门
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            UserDepartInfo departInfo = ((HyperlinkButton)sender).DataContext as UserDepartInfo;
            ((VmDepartMng)this.DataContext).Delete(departInfo);
        }

        //查看子部门
        private void btnChildDepart_Click(object sender, RoutedEventArgs e)
        {
            UserDepartInfo dInfo = ((HyperlinkButton)sender).DataContext as UserDepartInfo;
            ((VmDepartMng)this.DataContext).CheckChild(dInfo);
        }

        #region 排序 by cty

        //进行排序
        private void dataGridDepart_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MyDataGridSortInChinese.OrderData(sender, e, ((VmDepartMng)this.DataContext).DepartInfos);
        }

        //显示排序箭头
        private void dataGridDepart_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        //显示排序箭头
        private void dataGridDepart_MouseMove(object sender, MouseEventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        //显示排序箭头
        private void dataGridDepart_LayoutUpdated(object sender, EventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }


        #endregion

        //导出excel
        private void ExportExl_Click(object sender, RoutedEventArgs e)
        {
            string space = "                                                  ";
            ExpExcel.ExportExcelFromDataGrid(this.dataGridDepart, 2, 7, (space + "部门管理" + space), "部门管理");
        }

    
      
    }
}
