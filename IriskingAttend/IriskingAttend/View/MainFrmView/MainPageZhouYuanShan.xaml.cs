/*************************************************************************
** 文件名:   MainPageZhouYuanShan.cs
×× 主要类:   MainPage
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2014-1-9
** 修改人:   
** 日  期:
** 描  述:   MainPage类,周源山矿报表的主界面部分
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
using System.IO.IsolatedStorage;
using IriskingAttend.ViewModel.SystemViewModel;
using Silverlight.OutlookBar;
using IriskingAttend.Dialog;
using IriskingAttend.ApplicationType;
using IriskingAttend.ZhouYuanShan;
using IriskingAttend.ViewModel;

namespace IriskingAttend
{
    public partial class MainPage : Page
    {
        /// <summary>
        /// 周源山报表查询页面回调函数
        /// </summary>
        public static Action<ReportQueryCondition, ChildWindow> ZhouYuanShanQueryReportAction;
        
        
        public void LoadQueryPage()
        {
            ZhouYuanShanQueryReportAction = new Action<ReportQueryCondition, ChildWindow>(QueryReport);

            //客户要求报表查询页面以窗口的形式弹出
            //周源山报表查询页面
            ChildWndReportQuery reportQuery = ChildWndReportQuery.GetInstance();
            reportQuery.Show();
        }

        /// <summary>
        /// 根据条件查询报表
        /// </summary>
        /// <param name="reportQueryCondition"></param>
        private void QueryReport(ReportQueryCondition reportQueryCondition, ChildWindow wnd)
        {
            if (reportQueryCondition.BeginTime == null)
            {
                MsgBoxWindow.MsgBox( "开始时间不能为空！", MsgBoxWindow.MsgIcon.Information,MsgBoxWindow.MsgBtns.OK);
                return;
            }

            //if (reportQueryCondition.EndTime == null)
            //{
            //    reportQueryCondition.EndTime = System.DateTime.Now;
            //}

            if (reportQueryCondition.EndTime.HasValue)
            {
                if (((DateTime)reportQueryCondition.BeginTime - (DateTime)reportQueryCondition.EndTime).Days > 0)
                {
                    MsgBoxWindow.MsgBox(
                                "请确定您的开始时间早于截止时间！",
                                Dialog.MsgBoxWindow.MsgIcon.Information,
                                Dialog.MsgBoxWindow.MsgBtns.OK);
                    return;
                }
            }
           
            
            wnd.DialogResult = true;
            VmZhouYuanShanReport.QueryCondition = reportQueryCondition;
            
            switch (reportQueryCondition.ReportType)
            {
                case ReportType.MonthlyReportOnPerson:
                    {
                        if (this.ContentFrame.CurrentSource.OriginalString != "/PersonAttendStatisticsReport")
                        {
                            ConvertToFunctionPage(new Uri("/PersonAttendStatisticsReport", UriKind.Relative));
                        }
                        else
                        {
                            this.ContentFrame.Refresh();
                        }
                    }
                    break;
                case ReportType.DailyReportOnPerson:
                    {
                        if (this.ContentFrame.CurrentSource.OriginalString != "/PersonDayReport")
                        {
                            ConvertToFunctionPage(new Uri("/PersonDayReport", UriKind.Relative));
                        }
                        else
                        {
                            this.ContentFrame.Refresh();
                        }
                    }
                    break;
                case ReportType.DetailReportOnDepart:
                    {
                        if (this.ContentFrame.CurrentSource.OriginalString != "/DepartDetailReport")
                        {
                            ConvertToFunctionPage(new Uri("/DepartDetailReport", UriKind.Relative));
                        }
                        else
                        {
                            this.ContentFrame.Refresh();
                        }
                    }
                    break;
                case ReportType.MonthlyReportOnDepart:
                    {
                        if (reportQueryCondition.EndTime == null)
                        {
                            reportQueryCondition.EndTime = System.DateTime.Now;
                        }
                        if (this.ContentFrame.CurrentSource.OriginalString != "/DepartMonthReport")
                        {
                            ConvertToFunctionPage(new Uri("/DepartMonthReport", UriKind.Relative));
                        }
                        else
                        {
                            this.ContentFrame.Refresh();
                        }
                    }
                    break;
                default:
                    {
                    }
                    break;

            }
        }
    }       
}
