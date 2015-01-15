/*************************************************************************
** 文件名:   DepartMonthReport.cs
** 主要类:   DepartMonthReport
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2013-1-12
** 修改人:   
** 日  期:
** 描  述:   周源山矿部门月统计表
**
** 版  本:   1.0.0
** 备  注:  命名及代码编写遵守C#编码规范
**
*************************************************************************/
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
using System.Windows.Browser;
using System.Windows.Data;

namespace IriskingAttend.ZhouYuanShan
{
    public class MinuteToHourConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return "";
            }
            int minutes = (int)value;

            return string.Format("{0}:{1}", minutes/60, (minutes%60).ToString("d2"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public partial class DepartMonthReport : Page
    {
        VmZhouYuanShanReport _vmZYSReport = new VmZhouYuanShanReport();

        public DepartMonthReport()
        {
            InitializeComponent();

            this.DataContext = _vmZYSReport;

            //部门月报表信息表数据源
            this.dgDepartMonthAttend.ItemsSource = _vmZYSReport.DepartMonthAttendZYS;

            _vmZYSReport.GetDepartMonthAttendRia();

            //日期
            this.txtDateDuration.Text = "";
            if (VmZhouYuanShanReport.QueryCondition.BeginTime.HasValue)
            {
                this.txtDateDuration.Text += VmZhouYuanShanReport.QueryCondition.BeginTime.Value.ToString("yyyy-MM-dd");
            }
            if (VmZhouYuanShanReport.QueryCondition.EndTime.HasValue)
            {
                this.txtDateDuration.Text += "到";
                this.txtDateDuration.Text += VmZhouYuanShanReport.QueryCondition.EndTime.Value.ToString("yyyy-MM-dd");
            }

            //注册行加载事件
            dgDepartMonthAttend.LoadingRow += new EventHandler<DataGridRowEventArgs>(dgDepartMonthAttend_LoadingRow);
        }

        #region 点击报表单元格出现详细信息相关

        /// <summary>
        /// 加载行事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dgDepartMonthAttend_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.MouseLeftButtonUp -= new MouseButtonEventHandler(Row_MouseLeftButtonUp);
            e.Row.MouseLeftButtonUp += new MouseButtonEventHandler(Row_MouseLeftButtonUp);
        }

        /// <summary>
        /// 点击行事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Row_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!PublicMethods.IsUserControlDoubleClicked(this.dgDepartMonthAttend))
            {
                return;
            }

            //获取当前点击的列index
            //2 = 部门名称
            int selectIndex = dgDepartMonthAttend.CurrentColumn.DisplayIndex;
            if (selectIndex == 2)
            {
                //获取当前点击的部门名称
                string departName = ((TextBlock)this.dgDepartMonthAttend.CurrentColumn.GetCellContent(this.dgDepartMonthAttend.SelectedItem)).Text;
                if (departName != null && departName.Length > 0)
                {
                    //客户要求报表查询页面以窗口的形式弹出
                    //周源山报表查询页面
                    ChildWndReportQuery reportQuery = ChildWndReportQuery.GetInstance();
                    reportQuery.InitDeparts(new string[] { departName });
                    reportQuery.Show();
                }
              
            }
        }

        #endregion

        /// <summary>
        /// 导出数据到Excel中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            if (_vmZYSReport.DepartMonthAttendZYS.Count() < 1)
            {
                Dialog.MsgBoxWindow.MsgBox(
                            "请查询到数据后再导出Excel！",
                            Dialog.MsgBoxWindow.MsgIcon.Information,
                            Dialog.MsgBoxWindow.MsgBtns.OK);
                return;
            }
            var uri = new Uri(App.Current.Host.Source, string.Format("../ZhouYuanShan/周源山煤矿部门月统计表.aspx?beginTime={0}&endTime={1}", VmZhouYuanShanReport.QueryCondition.BeginTime,VmZhouYuanShanReport.QueryCondition.EndTime));
            HtmlPage.PopupWindow(uri, "_blank", new HtmlPopupWindowOptions());
        }
    }
}
