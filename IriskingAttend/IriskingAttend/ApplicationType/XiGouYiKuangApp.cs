/*************************************************************************
** 文件名:   XiGouYiKuangApp.cs
** 主要类:   XiGouYiKuangApp
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2014-4-14
** 修改人:   
** 日  期:
** 描  述:   XiGouYiKuangApp，西沟一矿应用程序类型
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
    public class XiGouYiKuangApp : AbstractApp
    {
        private AbstractApp BaseApp;

        public XiGouYiKuangApp(AbstractApp BaseApp)
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

            dictPrivilegeList[PrivilegeENUM.LeaderScheduling] = true;              //"领导排班考勤" 
            dictPrivilegeList[PrivilegeENUM.ReportXiGouDayAttend] = true;  //西沟一矿日报表
            dictPrivilegeList[PrivilegeENUM.ReportXiGouMonthAttend] = true;  //西沟一矿月报表 
            dictPrivilegeList[PrivilegeENUM.ReportXiGouInWellPerson] = true;  //西沟一矿人员汇总表 
            dictPrivilegeList[PrivilegeENUM.ReportXiGouInWellPersonDetail] = true;  //西沟一矿人员明细表 
          
            return dictPrivilegeList;
        }
    }
}
