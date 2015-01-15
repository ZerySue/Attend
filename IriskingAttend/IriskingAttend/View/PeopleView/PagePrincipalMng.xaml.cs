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
using IriskingAttend.ViewModelMine.PeopleViewModel;
using IriskingAttend.View;
using Irisking.Web.DataModel;
using IriskingAttend.Common;
using IriskingAttend.ExportExcel;
using IriskingAttend.ViewModel.PeopleViewModel;


namespace IriskingAttend.View.PeopleView
{
    public partial class PagePrincipalMng : Page
    {
        public PagePrincipalMng()
        {
            InitializeComponent();

            //加载vm
            VmPrincipalMng vm = new VmPrincipalMng();
            vm.LoadCompletedEvent += new EventHandler((sender,e) =>
            {
                this.DataContext = sender;
            });

            //全选绑定初始化
            vm.MarkObj = this.Resources["MarkObject"] as MarkObject;

            //添加序号 不能采用绑定的方式，绑定的话排序后序号会跟着变化by cty
            this.dataGridPrincipal.LoadingRow += (a, e) =>
            {
                int index = e.Row.GetIndex();
                var cell = dataGridPrincipal.Columns[1].GetCellContent(e.Row) as TextBlock;
                cell.Text = (index + 1).ToString();
            };

            dataGridPrincipal.AddHandler(MouseLeftButtonUpEvent, new MouseButtonEventHandler(dataGridPrincipal_MouseLeftButtonUp), true);

        }

       

        // 当用户导航到此页面时执行。
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        #region 界面按钮响应

        //订阅行点击事件
        private void dataGridPrincipal_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.MouseLeftButtonUp -= new MouseButtonEventHandler(Row_MouseLeftButtonUp);
            e.Row.MouseLeftButtonUp += new MouseButtonEventHandler(Row_MouseLeftButtonUp);
        }

        //datagrid每一行的鼠标点击事件
        void Row_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (dataGridPrincipal.CurrentColumn.DisplayIndex == 0)
            {
                if (dataGridPrincipal.SelectedItem != null)
                {
                    var obj = dataGridPrincipal.SelectedItem as PrincipalInfo;
                    ((VmPrincipalMng)this.DataContext).SelectItems(obj);
                }
            }
        }

        private void btnModify_Click(object sender, RoutedEventArgs e)
        {
            ((VmPrincipalMng)this.DataContext).ModifyCurPrincipal();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            ((VmPrincipalMng)this.DataContext).DeleteCurPrincipal();
        }

        private void chkSelectAll_Click(object sender, RoutedEventArgs e)
        {
            ((VmPrincipalMng)this.DataContext).SelectAll(((CheckBox)sender).IsChecked);
        }

        #endregion

        #region 排序

        private void dataGridPrincipal_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MyDataGridSortInChinese.OrderData(sender, e, ((VmPrincipalMng)this.DataContext).PrincipalInfos);
        }

        private void dataGridPrincipal_MouseMove(object sender, MouseEventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        private void dataGridPrincipal_LayoutUpdated(object sender, EventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        private void dataGridPrincipal_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        #endregion


      

        private void ExportExl_Click(object sender, RoutedEventArgs e)
        {
            string space = "                                   ";
            ExpExcel.ExportExcelFromDataGrid(this.dataGridPrincipal, 2,5,(space + "职务管理" + space), "职务管理");
        }

      

    }
}
