/*************************************************************************
** 文件名:   PersonDayReport.cs
** 主要类:   PersonDayReport
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2013-1-9
** 修改人:   
** 日  期:
** 描  述:   周源山矿当日考勤报表
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

namespace IriskingAttend.ZhouYuanShan
{
    public partial class PersonDayReport : Page
    {
        VmZhouYuanShanReport _vmZYSReport = new VmZhouYuanShanReport();

        public PersonDayReport()
        {
            InitializeComponent();

            this.DataContext = _vmZYSReport;

            //设备信息表数据源
            this.dgDayPersonAttend.ItemsSource = _vmZYSReport.PersonDayAttendZYS;

            _vmZYSReport.GetDayPersonAttendRia();

            //日期
            this.txtDateDuration.Text = "";
            if (VmZhouYuanShanReport.QueryCondition.BeginTime.HasValue)
            {
                this.txtDateDuration.Text += VmZhouYuanShanReport.QueryCondition.BeginTime.Value.ToString("yyyy-MM-dd");
            }
            //注册行加载事件
            dgDayPersonAttend.LoadingRow += new EventHandler<DataGridRowEventArgs>(dgDayPersonAttend_LoadingRow);
        }

        #region 点击报表单元格出现详细信息相关

        /// <summary>
        /// 加载行事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dgDayPersonAttend_LoadingRow(object sender, DataGridRowEventArgs e)
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
            if (!PublicMethods.IsUserControlDoubleClicked(this.dgDayPersonAttend))
            {
                return;
            }

            //获取当前点击的列index
            //3  = 姓名；1 = 部门名称
            int selectIndex = dgDayPersonAttend.CurrentColumn.DisplayIndex;
            if (selectIndex == 3)
            {
                //获取当前点击的姓名
                string name = ((TextBlock)this.dgDayPersonAttend.CurrentColumn.GetCellContent(this.dgDayPersonAttend.SelectedItem)).Text;

                if (name != null && name.Length > 0)
                {
                    //客户要求报表查询页面以窗口的形式弹出
                    //周源山报表查询页面
                    ChildWndReportQuery reportQuery = ChildWndReportQuery.GetInstance();
                    reportQuery.InitPersons(new string[] { name });
                    reportQuery.Show();
                }               
            }
            else if (selectIndex == 1)
            {
                //获取当前点击的部门名称
                string departName = ((TextBlock)this.dgDayPersonAttend.CurrentColumn.GetCellContent(this.dgDayPersonAttend.SelectedItem)).Text;

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
            if (_vmZYSReport.PersonDayAttendZYS.Count() < 1)
            {
                Dialog.MsgBoxWindow.MsgBox(
                            "请查询到数据后再导出Excel！",
                            Dialog.MsgBoxWindow.MsgIcon.Information,
                            Dialog.MsgBoxWindow.MsgBtns.OK);
                return;
            }
            var uri = new Uri(App.Current.Host.Source, string.Format("../ZhouYuanShan/周源山矿当日考勤报表.aspx?beginTime={0}", VmZhouYuanShanReport.QueryCondition.BeginTime));
            HtmlPage.PopupWindow(uri, "_blank", new HtmlPopupWindowOptions());
        }
    }
}
