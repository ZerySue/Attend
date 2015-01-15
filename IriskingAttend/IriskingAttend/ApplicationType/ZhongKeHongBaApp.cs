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
    public class ZhongKeHongBaApp : AbstractApp
    {
         private AbstractApp BaseApp;

         public ZhongKeHongBaApp(AbstractApp BaseApp)
        {
            this.BaseApp = BaseApp;
        }

        public override Dictionary<PrivilegeENUM, bool> GetDictPrivilegeListVisible()
        {
            Dictionary<PrivilegeENUM, bool> dictPrivilegeList = BaseApp.GetDictPrivilegeListVisible();

            dictPrivilegeList[PrivilegeENUM.ReportOriginRecord] = false;              //"原始记录汇总表"
            dictPrivilegeList[PrivilegeENUM.ReportOriginRecordExportExcel] = false;   //"导出Excel"
            dictPrivilegeList[PrivilegeENUM.ReportOriginRecordPrint] = false;         //"打印预览"

            dictPrivilegeList[PrivilegeENUM.ReportCustom] = false;
            dictPrivilegeList[PrivilegeENUM.Report] = false; 

            dictPrivilegeList[PrivilegeENUM.ZhongKeHongBa] = true; 
            dictPrivilegeList[PrivilegeENUM.ZhongKeHongBaPersonFullAttend] = true;          //"员工全勤奖统计表"
            dictPrivilegeList[PrivilegeENUM.ZhongKeHongBaLeakageAttendanceQuery] = true;    //中科虹霸"楼考勤报表"
            dictPrivilegeList[PrivilegeENUM.ZhongKeHongBaPersonMealSupplement] = true;      //中科虹霸餐补报表
            dictPrivilegeList[PrivilegeENUM.ZhongKeHongBaOriginRecSumList] = true;          //原始记录汇总表
            dictPrivilegeList[PrivilegeENUM.ZhongKeHongBaPersonLeaveList]= true;           //人员请假列表
            dictPrivilegeList[PrivilegeENUM.ZhongKeHongBaPersonLatearrivalList]= true;     //迟到早退人员列表
            dictPrivilegeList[PrivilegeENUM.ZhongKeHongBaPersonTimeProblemList] = true;     //工时不足8小时超过3次的人员列表

            return dictPrivilegeList;
        }
    }
}
