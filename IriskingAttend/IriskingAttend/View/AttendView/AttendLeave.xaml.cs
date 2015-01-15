/*************************************************************************
** 文件名:   AttendLeave.cs
×× 主要类:   AttendLeave
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   lzc
** 日  期:   2013-4-23
** 修改人:   
** 日  期:
** 描  述:   AttendLeave类,请假界面
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
using IriskingAttend.Dialog;
using IriskingAttend.ViewModel;
using IriskingAttend.ExportExcel;
using IriskingAttend.Common;
using Irisking.Web.DataModel;

namespace IriskingAttend.View
{
    public partial class AttendLeave : Page
    {
       // private VmDepartment _vmDepart = new VmDepartment();
        #region  构造函数
        public AttendLeave()
        {
            InitializeComponent();

            ///序号
            dgAttendLeave.LoadingRow += (a, e) =>
            {
                int index = e.Row.GetIndex();
                var cell = dgAttendLeave.Columns[0].GetCellContent(e.Row) as TextBlock;
                cell.Text = (index + 1).ToString();
            };
            dgAttendLeave.AddHandler(MouseLeftButtonUpEvent, new MouseButtonEventHandler(dgAttendLeave_MouseLeftButtonUp), true);
            // //设置部门的源
            //cmbDepart.ItemsSource = _vmDepart.DepartmentModel;
            //cmbDepart.DisplayMemberPath = "depart_name";
            //_vmDepart.GetDepartment();

        }
        #endregion

        #region 响应事件函数
        /// <summary>
        /// 进入界面
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (App.Current.Resources.Contains("AttendLeaveDateBegin"))
            {
                dateBegin.Text = App.Current.Resources["AttendLeaveDateBegin"].ToString();
            }

            if (App.Current.Resources.Contains("AttendLeaveDateEnd"))
            {
                dateEnd.Text = App.Current.Resources["AttendLeaveDateEnd"].ToString();
            }
        }

        /// <summary>
        /// 页面离开
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (!App.Current.Resources.Contains("AttendLeaveDateBegin"))
            {
                if (dateBegin.Text != null)
                {
                    App.Current.Resources.Add("AttendLeaveDateBegin", dateBegin.Text);
                }
            }
            else
            {
                try
                {
                    App.Current.Resources.Remove("AttendLeaveDateBegin");

                    if (!App.Current.Resources.Contains("AttendLeaveDateBegin"))
                    {
                        App.Current.Resources.Add("AttendLeaveDateBegin", dateBegin.Text);
                    }
                }
                catch (Exception ee)
                {
                    string err = ee.Message;
                }
            }

            if (!App.Current.Resources.Contains("AttendLeaveDateEnd"))
            {
                if (dateEnd.Text != null)
                {
                    App.Current.Resources.Add("AttendLeaveDateEnd", dateEnd.Text);
                }
            }
            else
            {
                App.Current.Resources.Remove("AttendLeaveDateEnd");
                App.Current.Resources.Add("AttendLeaveDateEnd", dateEnd.Text);
            }
        }

        /// <summary>
        /// 排序箭头设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgAttendLeave_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
            //需要重绘
            dgAttendLeave.UpdateLayout();
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgAttendLeave_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                MyDataGridSortInChinese.OrderData(sender, e, ((VmAttendLeave)dgAttendLeave.DataContext).AttendLeaveModel);
            }
            catch (Exception ex)
            {
                ErrorWindow errWin = new ErrorWindow(ex);
                errWin.Show();
            }
        }

        /// <summary>
        /// 排序箭头设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgAttendLeave_MouseMove(object sender, MouseEventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        /// <summary>
        /// 排序箭头设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgAttendLeave_LayoutUpdated(object sender, EventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }


        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportExl_Click(object sender, RoutedEventArgs e)
        {
            string space = "                                                                 ";
            ExpExcel.ExportExcelFromDataGrid(dgAttendLeave, 1, 8, (space + "人员请假" + space), "人员请假");
        }
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
                    //btnQuery.IsEnabled = false;
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
