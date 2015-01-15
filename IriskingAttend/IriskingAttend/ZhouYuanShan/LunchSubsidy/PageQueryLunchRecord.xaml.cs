/*************************************************************************
** 文件名:   PageUnCompletedLunch.cs
×× 主要类:   PageUnCompletedLunch
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   wz
** 日  期:   2013-6-14
** 修改人:   
** 日  期:
** 描  述:   PageUnCompletedLunch类,增删改班制页面
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
using IriskingAttend.Web.ZhouYuanShan;
using IriskingAttend.Dialog;
using System.IO.IsolatedStorage;

namespace IriskingAttend.ZhouYuanShan.LunchSubsidy
{
    public partial class PageQueryLunchRecord : Page
    {
        public PageQueryLunchRecord()
        {
            InitializeComponent();

            //加载vm
            VmQueryLunchRecord vm = new VmQueryLunchRecord(textCmbClassOrder);
           
            this.DataContext = vm;
           

            //全选绑定初始化
            vm.MarkObjLunchRecordInfoOnPerson = this.Resources["MarkObjectPersonLunchRecord"] as MarkObject;
            vm.MarkObjLunchRecordInfoOnClassOrder = this.Resources["MarkObjectClassOrderLunchRecord"] as MarkObject;
            vm.MarkObjLunchRecordInfoOnDepart = this.Resources["MarkObjectDepartLunchRecord"] as MarkObject;

            //添加序号 不能采用绑定的方式，绑定的话排序后序号会跟着变化
            this.dgPersonLunchRecord.LoadingRow += (a, e) =>
            {
                int index = e.Row.GetIndex();
                var cell = dgPersonLunchRecord.Columns[1].GetCellContent(e.Row) as TextBlock;
                cell.Text = (index + 1).ToString();
            };
            this.dgClassOrderLunchRecord.LoadingRow += (a, e) =>
            {
                int index = e.Row.GetIndex();
                var cell = dgClassOrderLunchRecord.Columns[1].GetCellContent(e.Row) as TextBlock;
                cell.Text = (index + 1).ToString();
            };
            this.dgDepartLunchRecord.LoadingRow += (a, e) =>
            {
                int index = e.Row.GetIndex();
                var cell = dgDepartLunchRecord.Columns[1].GetCellContent(e.Row) as TextBlock;
                cell.Text = (index + 1).ToString();
            };
            

            //排序箭头显示
            dgPersonLunchRecord.AddHandler(MouseLeftButtonUpEvent, new MouseButtonEventHandler(dataGrid_MouseLeftButtonUp), true);
            dgClassOrderLunchRecord.AddHandler(MouseLeftButtonUpEvent, new MouseButtonEventHandler(dataGrid_MouseLeftButtonUp), true);
            dgDepartLunchRecord.AddHandler(MouseLeftButtonUpEvent, new MouseButtonEventHandler(dataGrid_MouseLeftButtonUp), true);
        }

        // 当用户导航到此页面时执行。
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            IsolatedStorageSettings appSettings = IsolatedStorageSettings.ApplicationSettings;
            if (!appSettings.Contains("PageQueryLunchRecord_BeginTime"))
            {
                appSettings.Add("PageQueryLunchRecord_BeginTime", null);
            }
            if (!appSettings.Contains("PageQueryLunchRecord_EndTime"))
            {
                appSettings.Add("PageQueryLunchRecord_EndTime", null);
            }

            appSettings["PageQueryLunchRecord_BeginTime"] = this.dateBegin.SelectedDate;
            appSettings["PageQueryLunchRecord_EndTime"] = this.dateEnd.SelectedDate;
            base.OnNavigatingFrom(e);
        }


        #region 排序 

        private void dataGrid_LayoutUpdated(object sender, EventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
            
        }

        //个人
        private void dgPersonLunchRecord_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MyDataGridSortInChinese.OrderData(sender, e, ((VmQueryLunchRecord)this.DataContext).LunchRecordInfoOnPerson);
        }

        //班次
        private void dgClassOrderLunchRecord_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MyDataGridSortInChinese.OrderData(sender, e, ((VmQueryLunchRecord)this.DataContext).LunchRecordInfoOnClassOrder);
        }

        //部门
        private void dgDepartLunchRecord_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MyDataGridSortInChinese.OrderData(sender, e, ((VmQueryLunchRecord)this.DataContext).LunchRecordInfoOnDepart);
        }

        private void dataGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }


        private void dataGrid_MouseMove(object sender, MouseEventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        #endregion

        #region 界面按钮响应

        #region 单选
        

        //个人
        private void dgPersonLunchRecord_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.MouseLeftButtonUp -= new MouseButtonEventHandler(dgPersonLunchRecord_MouseLeftButtonUp);
            e.Row.MouseLeftButtonUp += new MouseButtonEventHandler(dgPersonLunchRecord_MouseLeftButtonUp);
        }

        void dgPersonLunchRecord_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            
            if (dgPersonLunchRecord.CurrentColumn.DisplayIndex == 0)
            {
                if (dgPersonLunchRecord.SelectedItem != null)
                {
                    var obj = dgPersonLunchRecord.SelectedItem as LunchRecordInfoOnPerson;
                    ((VmQueryLunchRecord)this.DataContext).SelectItemsOnPerson(obj);
                }
            }
        }

        //班次
        private void dgClassOrderLunchRecord_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.MouseLeftButtonUp -= new MouseButtonEventHandler(dgClassOrderLunchRecord_MouseLeftButtonUp);
            e.Row.MouseLeftButtonUp += new MouseButtonEventHandler(dgClassOrderLunchRecord_MouseLeftButtonUp);
        }

        void dgClassOrderLunchRecord_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

            if (dgClassOrderLunchRecord.CurrentColumn.DisplayIndex == 0)
            {
                if (dgClassOrderLunchRecord.SelectedItem != null)
                {
                    var obj = dgClassOrderLunchRecord.SelectedItem as ReportRecordInfoOnDepart_ZhouYuanShan;
                    ((VmQueryLunchRecord)this.DataContext).SelectItemsOnClassOrder(obj);
                }
            }
        }

        //部门
        private void dgDepartLunchRecord_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.MouseLeftButtonUp -= new MouseButtonEventHandler(dgDepartLunchRecord_MouseLeftButtonUp);
            e.Row.MouseLeftButtonUp += new MouseButtonEventHandler(dgDepartLunchRecord_MouseLeftButtonUp);
        }

        void dgDepartLunchRecord_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

            if (dgDepartLunchRecord.CurrentColumn.DisplayIndex == 0)
            {
                if (dgDepartLunchRecord.SelectedItem != null)
                {
                    var obj = dgDepartLunchRecord.SelectedItem as LunchRecordInfoOnDepart;
                    ((VmQueryLunchRecord)this.DataContext).SelectItemsOnDepart(obj);
                }
            }
        }

        #endregion

        #region 全选

        //个人
        private void chkSelectAllPersonLunchRecord_Click(object sender, RoutedEventArgs e)
        {
            ((VmQueryLunchRecord)this.DataContext).SelectAllOnPerson(((CheckBox)sender).IsChecked);
        }

        //班次
        private void chkSelectAllClassOrderLunchRecord_Click(object sender, RoutedEventArgs e)
        {
            ((VmQueryLunchRecord)this.DataContext).SelectAllOnClassOrder(((CheckBox)sender).IsChecked);
        }

        //部门
        private void chkSelectAllDepartLunchRecord_Click(object sender, RoutedEventArgs e)
        {
            ((VmQueryLunchRecord)this.DataContext).SelectAllOnDepart(((CheckBox)sender).IsChecked);
        }
        #endregion

        #region 导出excel

        //个人
        private void ExportExlPersonLunchRecord_Click(object sender, RoutedEventArgs e)
        {
            if (!dateBegin.SelectedDate.HasValue || !dateEnd.SelectedDate.HasValue)
            {
                return;
            }
            string title = "个人班中餐记录(" + dateBegin.SelectedDate.Value.Date + "到" + dateBegin.SelectedDate.Value.Date + ")";
            string space = "                         ";
            ExpExcel.ExportExcelFromDataGrid(this.dgPersonLunchRecord, 2, 8, (space + title + space), "个人班中餐记录");
        }

        //班次
        private void ExportExlClassOrderLunchRecord_Click(object sender, RoutedEventArgs e)
        {
            if (!dateBegin.SelectedDate.HasValue || !dateEnd.SelectedDate.HasValue)
            {
                return;
            }
            string title = "班次班中餐记录汇总(" + dateBegin.SelectedDate.Value.Date + "到" + dateBegin.SelectedDate.Value.Date + ")";
            string space = "                         ";
            ExpExcel.ExportExcelFromDataGrid(this.dgClassOrderLunchRecord, 2, 8, (space + title + space), "班次班中餐记录汇总");
        }

        //部门
        private void ExportExlDepartLunchRecord_Click(object sender, RoutedEventArgs e)
        {
            if (!dateBegin.SelectedDate.HasValue || !dateEnd.SelectedDate.HasValue)
            {
                return;
            }
            string title = "部门班中餐记录汇总(" + dateBegin.SelectedDate.Value.Date + "到" + dateBegin.SelectedDate.Value.Date + ")";
            string space = "                         ";
            ExpExcel.ExportExcelFromDataGrid(this.dgDepartLunchRecord, 2, 7, (space + title + space), "部门班中餐记录汇总");
        }


        #endregion

        #endregion

        #region 日期控件设置

        /// <summary>
        /// 设置DataPicker 日期的选择范围
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dateBegin_CalendarClosed(object sender, RoutedEventArgs e)
        {
            if (null != dateBegin.SelectedDate && null != dateEnd.SelectedDate)
            {
                if (dateBegin.SelectedDate.Value > dateEnd.SelectedDate.Value)
                {
                    MsgBoxWindow.MsgBox("选择结束时间不能早于开始时间！",
                        MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);
                    dateBegin.SelectedDate = null;
                }
            }
        }

        /// <summary>
        /// 校验日期格式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dateBegin_DateValidationError(object sender, DatePickerDateValidationErrorEventArgs e)
        {
            MsgBoxWindow.MsgBox("日期格式不对！", MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);
        }

        /// <summary>
        /// 设置时间选择范围判定，起始时间应需要截止时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dateEnd_CalendarClosed(object sender, RoutedEventArgs e)
        {
            if (null != dateBegin.SelectedDate && null != dateEnd.SelectedDate)
            {
                if (dateBegin.SelectedDate.Value > dateEnd.SelectedDate.Value)
                {
                    MsgBoxWindow.MsgBox("选择结束时间不能早于开始时间！",
                        MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);
                    dateEnd.SelectedDate = null;
                }
            }
        }

        #endregion


        


       
    }
}
