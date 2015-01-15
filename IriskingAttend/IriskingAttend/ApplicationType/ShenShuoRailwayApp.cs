/*************************************************************************
** 文件名:   ShenShuoRailwayApp.cs
** 主要类:   ShenShuoRailwayApp
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2013-12-11
** 修改人:   
** 日  期:
** 描  述:   ShenShuoRailwayApp，神朔铁路应用程序类型
**
** 版  本:   1.0.0
** 备  注:  命名及代码编写遵守C#编码规范
**
 * ***********************************************************************/
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace IriskingAttend.ApplicationType
{
    public class ShenShuoRailwayApp : AbstractApp
    {
        private AbstractApp BaseApp;

        public ShenShuoRailwayApp( AbstractApp BaseApp )
        {
            this.BaseApp = BaseApp;
        }

        public override Dictionary<PrivilegeENUM, bool> GetDictPrivilegeListVisible()
        {
            Dictionary<PrivilegeENUM, bool> dictPrivilegeList = BaseApp.GetDictPrivilegeListVisible();

            dictPrivilegeList[PrivilegeENUM.ReportOriginRecord] = false;              //"原始记录汇总表"
            dictPrivilegeList[PrivilegeENUM.ReportOriginRecordExportExcel] = false;   //"导出Excel"
            dictPrivilegeList[PrivilegeENUM.ReportOriginRecordPrint] = false;         //"打印预览"
            dictPrivilegeList[PrivilegeENUM.ReportCustom] = false;  //自定义报表

            dictPrivilegeList[PrivilegeENUM.ReportPersonAttendCollect] = true;              //"个人出勤表"
            dictPrivilegeList[PrivilegeENUM.ReportPersonAttendCollectExportExcel] = true;   //"导出Excel"
            dictPrivilegeList[PrivilegeENUM.ReportPersonAttendCollectPrint] = true;         //"打印预览"  

            dictPrivilegeList[PrivilegeENUM.ReportShenShuoAttendCollect] = true;              //"考勤统计表"
            dictPrivilegeList[PrivilegeENUM.ReportShenShuoAttendCollectExportExcel] = true;   //"导出Excel"
            dictPrivilegeList[PrivilegeENUM.ReportShenShuoAttendCollectPrint] = true;         //"打印预览"   

            return dictPrivilegeList;
        }
    }
}
