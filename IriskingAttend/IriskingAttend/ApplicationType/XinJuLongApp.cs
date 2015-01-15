/*************************************************************************
** 文件名:   XinJuLongApp.cs
** 主要类:   XinJuLongApp
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2014-3-18
** 修改人:   
** 日  期:
** 描  述:   XinJuLongApp，新巨龙应用程序类型
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
    public class XinJuLongApp : AbstractApp
    {
        private AbstractApp BaseApp;

        public XinJuLongApp(AbstractApp BaseApp)
        {
            this.BaseApp = BaseApp;
        }

        public override Dictionary<PrivilegeENUM, bool> GetDictPrivilegeListVisible()
        {
            Dictionary<PrivilegeENUM, bool> dictPrivilegeList = BaseApp.GetDictPrivilegeListVisible();

            dictPrivilegeList[PrivilegeENUM.Report] = false;                          //报表打印一大块不显示
            dictPrivilegeList[PrivilegeENUM.ReportOriginRecord] = false;              //"原始记录汇总表"
            dictPrivilegeList[PrivilegeENUM.ReportOriginRecordExportExcel] = false;   //"导出Excel"
            dictPrivilegeList[PrivilegeENUM.ReportOriginRecordPrint] = false;         //"打印预览"
            dictPrivilegeList[PrivilegeENUM.ReportCustom] = false;  //自定义报表

            //新巨龙模块及报表
            dictPrivilegeList[PrivilegeENUM.XinJuLong] = true; 
            dictPrivilegeList[PrivilegeENUM.XinJuLongDepartInWellCollect] = true;   //当天部门下井人数统计  
            dictPrivilegeList[PrivilegeENUM.XinJuLongPersonInWellCollect] = true;   //当天个人下井汇总
            dictPrivilegeList[PrivilegeENUM.XinJuLongInCompleteQuery] = true;       //不完整考勤查询
            dictPrivilegeList[PrivilegeENUM.XinJuLongReportPersonMonth] = true;  //新巨龙个人月报表

            dictPrivilegeList[PrivilegeENUM.SystemOperatorLogQuery] = true;  //操作员日志查询权限
            dictPrivilegeList[PrivilegeENUM.SystemOperatorLogDelete] = true;  //操作员日志删除权限
            
            dictPrivilegeList[PrivilegeENUM.PersonQuery] = true; 
            dictPrivilegeList[PrivilegeENUM.PersonAddDepart] = true; 
            dictPrivilegeList[PrivilegeENUM.PersonDeleteDepart] = true; 
            dictPrivilegeList[PrivilegeENUM.PersonModifyDepart] = true; 
            dictPrivilegeList[PrivilegeENUM.PersonBatchAddRecord] = true; 
            dictPrivilegeList[PrivilegeENUM.PersonBatchModifyPerson] = true; 
            dictPrivilegeList[PrivilegeENUM.PersonBatchStopIris] = true; 
            dictPrivilegeList[PrivilegeENUM.PersonBatchDeletePerson] = true; 
            dictPrivilegeList[PrivilegeENUM.PersonAddPerson] = true; 
            dictPrivilegeList[PrivilegeENUM.PersonModifyPerson] = true; 
            dictPrivilegeList[PrivilegeENUM.PersonDeletePerson] = true; 
            dictPrivilegeList[PrivilegeENUM.PersonIdentifyRecord] = true; 
            dictPrivilegeList[PrivilegeENUM.PersonStopIris] = true; 
            dictPrivilegeList[PrivilegeENUM.PersonExportExcel] = true;  

            return dictPrivilegeList;
        }
    }
}
