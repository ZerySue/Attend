/*************************************************************************
** 文件名:   PersonAttendStatisticsReport.cs
** 主要类:   PersonAttendStatisticsReport
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   wz
** 日  期:   2013-1-9
** 修改人:   
** 日  期:
** 描  述:   周源山矿个人月报表
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
using IriskingAttend.Web.ZhouYuanShan;
using System.IO.IsolatedStorage;

namespace IriskingAttend.ZhouYuanShan
{
    public partial class PersonAttendStatisticsReport : Page
    {
        VmZhouYuanShanReport _vmZYSReport = new VmZhouYuanShanReport();

        public PersonAttendStatisticsReport()
        {
            InitializeComponent();

            this.DataContext = _vmZYSReport;

          
            //表数据源
            this.dgDayPersonAttend.ItemsSource = _vmZYSReport.PersonAttendStatisticsZYS;

            //日期改动(开始时间变为当月第一天，结束时间变为当月最后一天)
            if (VmZhouYuanShanReport.QueryCondition.BeginTime.HasValue)
            {
                DateTime beginTime =VmZhouYuanShanReport.QueryCondition.BeginTime.Value;
                VmZhouYuanShanReport.QueryCondition.BeginTime = new DateTime(beginTime.Year, beginTime.Month, 1);
            }
            if (VmZhouYuanShanReport.QueryCondition.EndTime.HasValue)
            {
                DateTime endTime = VmZhouYuanShanReport.QueryCondition.EndTime.Value;
                VmZhouYuanShanReport.QueryCondition.EndTime = new DateTime(endTime.Year, endTime.Month, 1).AddMonths(1).AddSeconds(-1);
            }
            else
            {
                DateTime endTime = VmZhouYuanShanReport.QueryCondition.BeginTime.Value;
                VmZhouYuanShanReport.QueryCondition.EndTime = new DateTime(endTime.Year, endTime.Month, 1).AddMonths(1).AddSeconds(-1);
            }

            //日期
            this.textDateDuration.Text = "";
            if (VmZhouYuanShanReport.QueryCondition.BeginTime.HasValue)
            {
                this.textDateDuration.Text += VmZhouYuanShanReport.QueryCondition.BeginTime.Value.ToString("yyyy-MM-dd");
            }
            if (VmZhouYuanShanReport.QueryCondition.EndTime.HasValue)
            {
                this.textDateDuration.Text += "到";
                this.textDateDuration.Text += VmZhouYuanShanReport.QueryCondition.EndTime.Value.ToString("yyyy-MM-dd");
            }

            _vmZYSReport.GetPersonAttendStatisticsRia();

            //注册行加载事件
            dgDayPersonAttend.LoadingRow += new EventHandler<DataGridRowEventArgs>(dgDepartDetail_LoadingRow);
        }


        #region 点击报表单元格出现详细信息相关

        /// <summary>
        /// 加载行事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dgDepartDetail_LoadingRow(object sender, DataGridRowEventArgs e)
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
            //2  = 姓名；3 = 部门名称
            int selectIndex = dgDayPersonAttend.CurrentColumn.DisplayIndex;
            if (selectIndex == 2)
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
            else if (selectIndex == 3)
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
            else if (selectIndex >= 18)
            {
                PersonAttendStatistics temp = (PersonAttendStatistics)this.dgDayPersonAttend.SelectedItem;

                if (temp.work_sn.CompareTo("小计") == 0 || temp.work_sn.CompareTo("合计") == 0)
                {
                    return;
                }
                VmAttend vmAttend = new VmAttend();
                vmAttend.SelectAttendRec = new Irisking.Web.DataModel.UserAttendRec();
                vmAttend.SelectAttendRec.person_id = temp.person_id;
                vmAttend.SelectAttendRec.person_name = temp.name;
                vmAttend.SelectAttendRec.work_sn = temp.work_sn;

                IsolatedStorageSettings querySetting = IsolatedStorageSettings.ApplicationSettings;

                AttendQueryCondition condition = new AttendQueryCondition();
                //通过本地存储获取查询条件
                if (querySetting.Contains("attendConditon"))
                {
                    querySetting.Remove("attendConditon");
                }
                condition.BeginTime = (DateTime)VmZhouYuanShanReport.QueryCondition.BeginTime;
                condition.EndTime = (DateTime)VmZhouYuanShanReport.QueryCondition.EndTime;
                condition.DepartIdLst = new int[] { temp.depart_id };
                condition.Name = temp.name;
                condition.WorkSN = temp.work_sn;               

                querySetting.Add("attendConditon", condition);

                vmAttend.ShowAttendRecDetail();
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
            if (_vmZYSReport.PersonAttendStatisticsZYS.Count() < 1)
            {
                Dialog.MsgBoxWindow.MsgBox(
                            "请查询到数据后再导出Excel！",
                            Dialog.MsgBoxWindow.MsgIcon.Information,
                            Dialog.MsgBoxWindow.MsgBtns.OK);
                return;
            }


            var uri = new Uri(App.Current.Host.Source, string.Format("../ZhouYuanShan/周源山煤矿个人考勤月报表.aspx?beginTime={0}&endTime={1}",
                VmZhouYuanShanReport.QueryCondition.BeginTime, VmZhouYuanShanReport.QueryCondition.EndTime));
            HtmlPage.PopupWindow(uri, "_blank", new HtmlPopupWindowOptions());
        }
    }
}
