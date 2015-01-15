/*************************************************************************
** 文件名:   PageWorkTypeMng.cs
×× 主要类:   PageWorkTypeMng
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   wz
** 日  期:   2013-6-14
** 修改人:   
** 日  期:
** 描  述:   PageWorkTypeMng类,增删改班制页面
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
using IriskingAttend.Common;
using IriskingAttend.ViewModelMine.PeopleViewModel;
using Irisking.Web.DataModel;
using IriskingAttend.ExportExcel;
using IriskingAttend.View;

namespace IriskingAttend.ViewMine.PeopleView
{
    public partial class PageWorkTypeMng : Page
    {
        public PageWorkTypeMng()
        {
            InitializeComponent();


            //加载vm
            VmWorkTypeMng vm = new VmWorkTypeMng();
            vm.LoadCompletedEvent += new EventHandler((sender, e) =>
            {
                this.DataContext = sender;
            });

            //全选绑定初始化
            vm.MarkObj = this.Resources["MarkObject"] as MarkObject;

            //添加序号 不能采用绑定的方式，绑定的话排序后序号会跟着变化
            this.dataGridWorkType.LoadingRow += (a, e) =>
            {
                int index = e.Row.GetIndex();
                var cell = dataGridWorkType.Columns[1].GetCellContent(e.Row) as TextBlock;
                cell.Text = (index + 1).ToString();
            };

            dataGridWorkType.AddHandler(MouseLeftButtonUpEvent, new MouseButtonEventHandler(dataGridPrincipal_MouseLeftButtonUp), true);

        }

        // 当用户导航到此页面时执行。
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            
        }

        #region 排序

        private void dataGridPrincipal_LayoutUpdated(object sender, EventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
            
        }

        private void dataGridPrincipal_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MyDataGridSortInChinese.OrderData(sender, e, ((VmWorkTypeMng)this.DataContext).WorkTypeInfos);
           
        }

        private void dataGridPrincipal_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }


        private void dataGridPrincipal_MouseMove(object sender, MouseEventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        #endregion

        #region 界面按钮响应
        

        private void dataGridPrincipal_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.MouseLeftButtonUp -= new MouseButtonEventHandler(Row_MouseLeftButtonUp);
            e.Row.MouseLeftButtonUp += new MouseButtonEventHandler(Row_MouseLeftButtonUp);
        }

        void Row_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (dataGridWorkType.CurrentColumn.DisplayIndex == 0)
            {
                if (dataGridWorkType.SelectedItem != null)
                {
                    var obj = dataGridWorkType.SelectedItem as WorkTypeInfo;
                    ((VmWorkTypeMng)this.DataContext).SelectItems(obj);
                }
            }
        }

        private void btnModify_Click(object sender, RoutedEventArgs e)
        {
            ((VmWorkTypeMng)this.DataContext).ModifyWorkType();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            ((VmWorkTypeMng)this.DataContext).DeleteCurWorkType();
        }

        private void ExportExl_Click(object sender, RoutedEventArgs e)
        {
            string space = "                         ";
            ExpExcel.ExportExcelFromDataGrid(this.dataGridWorkType,2,4, (space + "工种管理" + space), "工种管理");
        }

        private void chkSelectAll_Click(object sender, RoutedEventArgs e)
        {
            ((VmWorkTypeMng)this.DataContext).SelectAll(((CheckBox)sender).IsChecked);
        }

        #endregion

       
    }
}
