/*************************************************************************
** 文件名:   Page_peopleMng.cs
×× 主要类:   Page_peopleMng
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   cty
** 日  期:   2013-6-14
** 修改人:   wz 代码优化
** 日  期:
** 描  述:   Page_peopleMng类，人员信息查询页面
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
using IriskingAttend.ViewModelMine.PeopleViewModel;
using Irisking.Web.DataModel;
using IriskingAttend.Common;
using IriskingAttend.ExportExcel;
using System.Windows.Controls.Primitives;
using System.Threading;
using System.Windows.Threading;
using IriskingAttend.View;

namespace IriskingAttend.ViewMine.PeopleView
{
    /// <summary>
    /// 人员信息管理界面
    /// </summary>
    public partial class Page_peopleMng_Mine : Page
    {
       

        public Page_peopleMng_Mine()
        {
            InitializeComponent();

            var vmPeopleMng = new VmPeopleMng_Mine();
            
            try
            {
                this.DataContext = vmPeopleMng;
                vmPeopleMng.MarkObj = this.Resources["MarkObject"] as MarkObject;
                //添加序号 不能采用绑定的方式，绑定的话排序后序号会跟着变化by cty
                this.dataGridPerson.LoadingRow += (a, e) =>
                {
                    int index = e.Row.GetIndex();
                    var cell = dataGridPerson.Columns[1].GetCellContent(e.Row) as TextBlock;
                    cell.Text = (index + 1).ToString();
                   
                };
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }

        // 当用户导航到此页面时执行。
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
           
        }

        //全选人员操作
        private void chkSelectAll_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext != null)
            {
                ((VmPeopleMng_Mine)this.DataContext).SelectAllPerson(sender);
            }
        }

        //注册鼠标事件
        private void dataGridPerson_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.MouseLeftButtonUp -= new MouseButtonEventHandler(Row_MouseLeftButtonUp);
            e.Row.MouseLeftButtonUp += new MouseButtonEventHandler(Row_MouseLeftButtonUp);
        }
        //点击行 选择item操作
        void Row_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.DataContext != null)
            {
                ((VmPeopleMng_Mine)this.DataContext).SelectItems(this.dataGridPerson);
            }
        }

        //查看按钮 by cty
        private void btnCheck_personInfo_Click(object sender, RoutedEventArgs e)
        {
            ((VmPeopleMng_Mine)this.DataContext).Check(sender);
        }

        //修改按钮 by cty
        private void btnModify_personInfo_Click(object sender, RoutedEventArgs e)
        {
            ((VmPeopleMng_Mine)this.DataContext).Modify(sender);
        }

        //删除按钮 by cty
        private void btnDelete_personInfo_Click(object sender, RoutedEventArgs e)
        {
            ((VmPeopleMng_Mine)this.DataContext).Delete(sender);
        }

        //查看识别记录按钮 by cty
        private void btnRecord_personInfo_Click(object sender, RoutedEventArgs e)
        {
            ((VmPeopleMng_Mine)this.DataContext).Record(sender);
        }

        //停用虹膜按钮 by cty
        private void btnStopIris_personInfo_Click(object sender, RoutedEventArgs e)
        {
            ((VmPeopleMng_Mine)this.DataContext).StopIris(sender);
        }

        //导出excel
        private void ExportExl_Click(object sender, RoutedEventArgs e)
        {
            string space = "                                                  ";
            ExpExcel.ExportExcelFromDataGrid(this.dataGridPerson, 2, 11, (space + "人员信息管理" + space), "人员信息管理", 9, 3000);       
        }

        #region 排序



        private void dataGridPerson_LayoutUpdated(object sender, EventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        private void dataGridPerson_MouseMove(object sender, MouseEventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        private void dataGridPerson_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MyDataGridSortInChinese.OrderData(sender, e, ((VmPeopleMng_Mine)this.DataContext).UserPersonInfos);
        }
        #endregion

 

    

    }
}
