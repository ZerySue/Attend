/*************************************************************************
** 文件名:   AbstractApp.cs
** 主要类:   AbstractApp
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2013-12-11
** 修改人:   
** 日  期:
** 描  述:   AbstractApp，应用程序类型抽象类
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
using System.Windows.Navigation;
using System.Windows.Media.Imaging;

namespace IriskingAttend.ApplicationType
{
    public abstract class AbstractApp
    {
        #region 辅助枚举 权限类型

        /// <summary>
        ///  权限类型枚举结构
        /// </summary>
        public enum PrivilegeENUM
        {
            Attend = 10,                    //"考勤管理"
            AttendOnduty = 10001,           //"当前在岗人员"
            AttendQuery = 10002,            //"考勤查询"
            AttendIdentifyRecord = 10003,   //"识别记录管理"
            AttendAbsense = 10004,          //"人员请假"
            AttendRestructure = 10005,      //"考勤重构"
            AttendLeaveManage = 10006,      //"请假类型管理"
            AttendFestivalMng = 10007,      //节假日管理

            //五虎山
            AbnormalAttendInfo = 10008,     //异常考勤查询
            IrisAttendQuery=10009,          //"虹膜考勤查询" 
          
            //西沟一矿
            LeaderScheduling = 10010,       //领导排班考勤

            Person = 11,                        //"人员管理"
            PersonDepartAndPeople = 11001,      //"部门人员管理" 
            PersonClassTypeAndOrder = 11002,    //"班制班次管理"
            PersonDepart = 11003,               //"部门管理"
            PersonPrincipal = 11004,            //"职务管理"
            PersonWorkType = 11005,             //"工种管理"
            PersonPrincipalType = 11006,        //"职务类型管理"

            //新巨龙 部门人员管理权限
            PersonQuery = 11007,
            PersonAddDepart = 11008,
            PersonDeleteDepart = 11009,
            PersonModifyDepart = 11010,
            PersonBatchAddRecord = 11011,
            PersonBatchModifyPerson = 11012,
            PersonBatchStopIris = 11013,
            PersonBatchDeletePerson = 11014,
            PersonAddPerson = 11015,
            PersonModifyPerson = 11016,            
            PersonDeletePerson = 11017,
            PersonIdentifyRecord = 11018,
            PersonStopIris = 11019,
            PersonExportExcel = 11020,


            Report = 12,                                //"报表打印"
            ReportOriginRecord = 12001,                 //"原始记录汇总表"
            ReportOriginRecordExportExcel = 12002,      //"导出Excel"
            ReportOriginRecordPrint = 12003,            //"打印预览"

            ReportDepartAttendCollect = 12004,            //五虎山"部门出勤汇总表"
            ReportDepartAttendCollectExportExcel = 12005, //"导出Excel"
            ReportDepartAttendCollectPrint = 12006,       //"打印预览"

            ReportPersonAttendCollect = 12007,            //神朔铁路"个人出勤表"
            ReportPersonAttendCollectExportExcel = 12008, //"导出Excel"
            ReportPersonAttendCollectPrint = 12009,       //"打印预览"

            ReportShenShuoAttendCollect = 12010,            //神朔铁路"考勤统计表"
            ReportShenShuoAttendCollectExportExcel = 12011, //"导出Excel"
            ReportShenShuoAttendCollectPrint = 12012,       //"打印预览"

            ReportZhouYuanShan = 12013,            //周源山报表       
            ReportCustom = 12014,                  //用户自定义报表 
            
            //西北电缆厂机关考勤
            ReportXiBeiDianLanAttend = 12015,

            //国电
            ReportGuoDian=12016,

            //虎峰煤矿
            ReportHuFengDayAttend = 12017,
            ReportHuFengMonthAttend = 12018,

            //西沟一矿
            ReportXiGouDayAttend = 12019,
            ReportXiGouMonthAttend = 12020,

            //林州铸锻
            ReportZhuDuanMonthAttend = 12021,

            //硫磺沟
            ReportLiuHuangGouMonthAttend = 12022,
            ReportLiuHuangGouMonthAttendUnderRule = 12023,
            ReportLiuHuangGouYearAttend = 12024,

            //西沟一矿，后增加两张报表
            ReportXiGouInWellPerson = 12025,
            ReportXiGouInWellPersonDetail = 12026,

            System = 13,                    //"系统管理"
            SystemParameter = 13001,        //"系统参数设置"
            SystemOperator  = 13002,        // "操作员管理"
            SystemDevice = 13003,           //"设备管理"
            SystemDBBackup = 13004,         //"数据库手动备份"   
            SystemOperatorLog = 13005,       //"操作员日志"   
            SystemOperatorLogQuery = 13006,       //操作员日志查询 
            SystemOperatorLogDelete = 13007,       //操作员日志删除 

            Lunch = 14,                    //"班中餐"
            LunchManage = 14001,           //"班中餐管理"
            LunchQuery = 14002,            //"班中餐查询"
            LunchManageQuery = 14003,          //"班中餐管理"中的查询
            LunchManageEdit = 14004,           //"班中餐管理"中的未完成班中餐编辑
            LunchManageCreate = 14005,         //"班中餐管理"中的未完成班中餐生成
            LunchManageShow = 14006,           //"班中餐管理"中的已完成班中餐查看
            LunchManageUndo = 14007,           //"班中餐管理"中的已完成班中餐撤销

            //新巨龙
            XinJuLong = 15,
            XinJuLongDepartInWellCollect = 15001,  //当天部门下井人数统计
            XinJuLongInCompleteQuery = 15002,      //不完整考勤查询
            XinJuLongPersonInWellCollect = 15003,  //当天个人下井汇总           
            XinJuLongReportPersonMonth = 15004,    //新巨龙个人月报表

            ZhongKeHongBa=16,
            ZhongKeHongBaPersonFullAttend = 16001,          //中科虹霸"全勤奖统计表"
            ZhongKeHongBaLeakageAttendanceQuery=16002,      //中科虹霸"楼考勤报表"
            ZhongKeHongBaPersonMealSupplement = 16003,      //中科虹霸餐补报表
            ZhongKeHongBaOriginRecSumList = 16004,          //原始记录汇总表
            ZhongKeHongBaPersonLeaveList = 16005,           //人员请假列表
            ZhongKeHongBaPersonLatearrivalList = 16006,     //迟到早退人员列表
            ZhongKeHongBaPersonTimeProblemList = 16007,     //工时不足8小时超过3次的人员列表
        }

        #endregion

        protected BitmapImage LogoImage = null;
        protected string LogoText = "";

        /// <summary>
        /// 获得logo图片
        /// </summary>
        /// <returns></returns>
        public BitmapImage GetLogoImage()
        {
            return LogoImage;
        }

        /// <summary>
        /// 获得logo名称
        /// </summary>
        /// <returns></returns>
        public string GetLogoText()
        {
            return LogoText;
        }

        /// <summary>
        /// 获得在设置权限、修改权限时的可见列表
        /// </summary>
        /// <returns></returns>
        public virtual Dictionary<PrivilegeENUM, bool> GetDictPrivilegeListVisible()
        {
            Dictionary<PrivilegeENUM, bool> dictPrivilegeList = new Dictionary<PrivilegeENUM, bool>();

            //公共模块：考勤管理
            dictPrivilegeList.Add(PrivilegeENUM.Attend, true);           //"考勤管理"
            dictPrivilegeList.Add(PrivilegeENUM.AttendOnduty, true);        //"当前在岗人员"
            dictPrivilegeList.Add(PrivilegeENUM.AttendQuery, true);         //"考勤查询"
            dictPrivilegeList.Add(PrivilegeENUM.AttendIdentifyRecord, true);//"识别记录管理"            
            dictPrivilegeList.Add(PrivilegeENUM.AttendRestructure, true);   //"考勤重构"            
            dictPrivilegeList.Add(PrivilegeENUM.AttendAbsense, true);       //"人员请假"
            dictPrivilegeList.Add(PrivilegeENUM.AttendLeaveManage, true);   //"请假类型管理"  
            dictPrivilegeList.Add(PrivilegeENUM.AttendFestivalMng, true);   //"节假日管理"             

            //公共模块：人员管理
            dictPrivilegeList.Add(PrivilegeENUM.Person, true);                  //"人员管理"
            dictPrivilegeList.Add(PrivilegeENUM.PersonDepartAndPeople, true);   //"部门人员管理" 
            dictPrivilegeList.Add(PrivilegeENUM.PersonClassTypeAndOrder, true); //"班制班次管理"
            dictPrivilegeList.Add(PrivilegeENUM.PersonDepart, true);            //"部门管理"
            dictPrivilegeList.Add(PrivilegeENUM.PersonPrincipal, true);         //"职务管理"
            dictPrivilegeList.Add(PrivilegeENUM.PersonPrincipalType, true);     //"职务类型管理"
            dictPrivilegeList.Add(PrivilegeENUM.PersonWorkType, true);          //"工种管理"

            //公共模块：系统管理
            dictPrivilegeList.Add(PrivilegeENUM.System, true);              //"系统管理"
            dictPrivilegeList.Add(PrivilegeENUM.SystemParameter, true);     //"系统参数设置"
            dictPrivilegeList.Add(PrivilegeENUM.SystemDevice, true);        //"设备管理"
            dictPrivilegeList.Add(PrivilegeENUM.SystemDBBackup, true);      //"数据库手动备份" 
            dictPrivilegeList.Add(PrivilegeENUM.SystemOperatorLog, true);   //"操作员日志"

            //公共模块：报表打印
            dictPrivilegeList.Add(PrivilegeENUM.Report, true);                          //"报表打印"
            dictPrivilegeList.Add(PrivilegeENUM.ReportOriginRecord, true);              //"原始记录汇总表"
            dictPrivilegeList.Add(PrivilegeENUM.ReportOriginRecordExportExcel, true);   //"导出Excel"
            dictPrivilegeList.Add(PrivilegeENUM.ReportOriginRecordPrint, true);         //"打印预览"
            dictPrivilegeList.Add(PrivilegeENUM.ReportCustom, false);                   //自定义报表
            
            //五虎山部门出勤汇总表
            dictPrivilegeList.Add(PrivilegeENUM.ReportDepartAttendCollect, false);              //"部门出勤汇总表"
            dictPrivilegeList.Add(PrivilegeENUM.ReportDepartAttendCollectExportExcel, false);   //"导出Excel"
            dictPrivilegeList.Add(PrivilegeENUM.ReportDepartAttendCollectPrint, false);         //"打印预览" 
            //五虎山异常考勤查询模块
            dictPrivilegeList.Add(PrivilegeENUM.AbnormalAttendInfo, false);   //"异常考勤查询" 
            dictPrivilegeList.Add(PrivilegeENUM.IrisAttendQuery, false);      //"虹膜考勤查询"
            dictPrivilegeList.Add(PrivilegeENUM.SystemOperatorLogQuery, false);  //操作员日志查询权限
            dictPrivilegeList.Add(PrivilegeENUM.SystemOperatorLogDelete, false);  //操作员日志删除权限

            //神朔铁路报表
            dictPrivilegeList.Add(PrivilegeENUM.ReportPersonAttendCollect, false);              //"个人出勤表"
            dictPrivilegeList.Add(PrivilegeENUM.ReportPersonAttendCollectExportExcel, false);   //"导出Excel"
            dictPrivilegeList.Add(PrivilegeENUM.ReportPersonAttendCollectPrint, false);         //"打印预览"   
            dictPrivilegeList.Add(PrivilegeENUM.ReportShenShuoAttendCollect, false);              //"考勤统计表"
            dictPrivilegeList.Add(PrivilegeENUM.ReportShenShuoAttendCollectExportExcel, false);   //"导出Excel"
            dictPrivilegeList.Add(PrivilegeENUM.ReportShenShuoAttendCollectPrint, false);         //"打印预览"   

            //北京中科虹霸报表
            dictPrivilegeList.Add(PrivilegeENUM.ZhongKeHongBa, false);
            dictPrivilegeList.Add(PrivilegeENUM.ZhongKeHongBaPersonFullAttend, false);              //"个人出勤表"
            dictPrivilegeList.Add(PrivilegeENUM.ZhongKeHongBaLeakageAttendanceQuery, false);    //中科虹霸"楼考勤报表"
            dictPrivilegeList.Add(PrivilegeENUM.ZhongKeHongBaPersonMealSupplement, false);      //中科虹霸餐补报表
            dictPrivilegeList.Add(PrivilegeENUM.ZhongKeHongBaOriginRecSumList, false);          //原始记录汇总表
            dictPrivilegeList.Add(PrivilegeENUM.ZhongKeHongBaPersonLeaveList, false);           //人员请假列表
            dictPrivilegeList.Add(PrivilegeENUM.ZhongKeHongBaPersonLatearrivalList, false);     //迟到早退人员列表
            dictPrivilegeList.Add(PrivilegeENUM.ZhongKeHongBaPersonTimeProblemList, false);     //工时不足8小时超过3次的人员列表

            //周源山报表及周源山模块
            dictPrivilegeList.Add(PrivilegeENUM.ReportZhouYuanShan, false); //"打印预览"   
            dictPrivilegeList.Add(PrivilegeENUM.Lunch, false);              //"班中餐"  
            dictPrivilegeList.Add(PrivilegeENUM.LunchManage, false);        //"班中餐管理"
            dictPrivilegeList.Add(PrivilegeENUM.LunchQuery, false);         //"班中餐查询"

            //西北电缆厂机关考勤
            dictPrivilegeList.Add(PrivilegeENUM.ReportXiBeiDianLanAttend, false); 

            dictPrivilegeList.Add(PrivilegeENUM.LunchManageQuery, false);       //"班中餐管理"中的查询
            dictPrivilegeList.Add(PrivilegeENUM.LunchManageEdit, false);        //"班中餐管理"中未完成班中餐编辑
            dictPrivilegeList.Add(PrivilegeENUM.LunchManageCreate, false);      //"班中餐管理"中未完成班中餐生成
            dictPrivilegeList.Add(PrivilegeENUM.LunchManageShow, false);        //"班中餐管理"中已完成班中餐查看
            dictPrivilegeList.Add(PrivilegeENUM.LunchManageUndo, false);        //"班中餐管理"中已完成班中餐撤销

            //新巨龙模块及报表
            dictPrivilegeList.Add(PrivilegeENUM.XinJuLong, false);            
            dictPrivilegeList.Add(PrivilegeENUM.XinJuLongDepartInWellCollect, false);   //当天部门下井人数统计  
            dictPrivilegeList.Add(PrivilegeENUM.XinJuLongPersonInWellCollect, false);   //当天个人下井汇总
            dictPrivilegeList.Add(PrivilegeENUM.XinJuLongInCompleteQuery, false);       //不完整考勤查询
            dictPrivilegeList.Add(PrivilegeENUM.XinJuLongReportPersonMonth, false);  //新巨龙个人月报表

            dictPrivilegeList.Add(PrivilegeENUM.PersonQuery, false);  
            dictPrivilegeList.Add(PrivilegeENUM.PersonAddDepart, false); 
            dictPrivilegeList.Add(PrivilegeENUM.PersonDeleteDepart, false); 
            dictPrivilegeList.Add(PrivilegeENUM.PersonModifyDepart, false); 
            dictPrivilegeList.Add(PrivilegeENUM.PersonBatchAddRecord, false); 
            dictPrivilegeList.Add(PrivilegeENUM.PersonBatchModifyPerson, false); 
            dictPrivilegeList.Add(PrivilegeENUM.PersonBatchStopIris, false); 
            dictPrivilegeList.Add(PrivilegeENUM.PersonBatchDeletePerson, false); 
            dictPrivilegeList.Add(PrivilegeENUM.PersonAddPerson, false); 
            dictPrivilegeList.Add(PrivilegeENUM.PersonModifyPerson, false); 
            dictPrivilegeList.Add(PrivilegeENUM.PersonDeletePerson, false); 
            dictPrivilegeList.Add(PrivilegeENUM.PersonIdentifyRecord, false); 
            dictPrivilegeList.Add(PrivilegeENUM.PersonStopIris, false); 
            dictPrivilegeList.Add(PrivilegeENUM.PersonExportExcel, false); 

            //西沟一矿领导排班考勤
            dictPrivilegeList.Add(PrivilegeENUM.LeaderScheduling, false);

            //西沟一矿报表
            dictPrivilegeList.Add(PrivilegeENUM.ReportXiGouDayAttend, false);
            dictPrivilegeList.Add(PrivilegeENUM.ReportXiGouMonthAttend, false);  
            dictPrivilegeList.Add(PrivilegeENUM.ReportXiGouInWellPerson, false);  
            dictPrivilegeList.Add(PrivilegeENUM.ReportXiGouInWellPersonDetail, false);  

            //国电报表
            dictPrivilegeList.Add(PrivilegeENUM.ReportGuoDian, false);

            //虎峰煤矿报表
            dictPrivilegeList.Add(PrivilegeENUM.ReportHuFengDayAttend, false);
            dictPrivilegeList.Add(PrivilegeENUM.ReportHuFengMonthAttend, false);

            //林州铸锻
            dictPrivilegeList.Add(PrivilegeENUM.ReportZhuDuanMonthAttend, false);

            //硫磺沟
            dictPrivilegeList.Add(PrivilegeENUM.ReportLiuHuangGouMonthAttend, false);
            dictPrivilegeList.Add(PrivilegeENUM.ReportLiuHuangGouMonthAttendUnderRule, false);
            dictPrivilegeList.Add(PrivilegeENUM.ReportLiuHuangGouYearAttend, false); 
           
            return dictPrivilegeList;
        }

        public virtual Dictionary<PrivilegeENUM, string> GetUriMapper()
        {
            Dictionary<PrivilegeENUM, string> dictUriList = new Dictionary<PrivilegeENUM, string>();  

            //考勤管理 非矿山界面
            dictUriList.Add(PrivilegeENUM.AttendQuery, "/View/AttendView/AttendRecord.xaml");
            dictUriList.Add(PrivilegeENUM.AttendOnduty, "/View/AttendView/InWellPerson.xaml");
            dictUriList.Add(PrivilegeENUM.AttendIdentifyRecord, "/View/AttendView/RecogManage.xaml");
            dictUriList.Add(PrivilegeENUM.AttendAbsense, "/View/AttendView/AttendLeave.xaml");
            dictUriList.Add(PrivilegeENUM.AttendLeaveManage, "/View/AttendView/AttendLeaveManage.xaml");
            dictUriList.Add(PrivilegeENUM.AttendRestructure, "/View/AttendView/RebuildAttend.xaml");
            dictUriList.Add(PrivilegeENUM.AttendFestivalMng, "/View/AttendView/FestivalMng.xaml");               

            //人员管理 非矿山界面    
            dictUriList.Add(PrivilegeENUM.PersonDepart, "/View/PeopleView/Page_departMng.xaml");
            dictUriList.Add(PrivilegeENUM.PersonDepartAndPeople, "/View/PeopleView/PageDepartAndPeopleMng.xaml");
            dictUriList.Add(PrivilegeENUM.PersonClassTypeAndOrder, "/View/PeopleView/Page_ClassTypeAndClassOrderMng.xaml");
            dictUriList.Add(PrivilegeENUM.PersonPrincipal, "/View/PeopleView/PagePrincipalMng.xaml");
            dictUriList.Add(PrivilegeENUM.PersonPrincipalType, "/View/PeopleView/PagePrincipalTypeMng.xaml");
            dictUriList.Add(PrivilegeENUM.PersonWorkType, "/ViewMine/PeopleView/PageWorkTypeMng.xaml");

            //系统管理  
            dictUriList.Add(PrivilegeENUM.SystemDevice, "/View/SystemView/DeviceManage.xaml");
            dictUriList.Add(PrivilegeENUM.SystemOperator, "/View/SystemView/OperatorManage.xaml");
            dictUriList.Add(PrivilegeENUM.SystemParameter, "/View/SystemView/ParameterConfig.xaml");
            dictUriList.Add(PrivilegeENUM.SystemDBBackup, "/View/SystemView/BackupConfig.xaml");
            dictUriList.Add(PrivilegeENUM.SystemOperatorLog, "/View/SystemView/OperatorLog.xaml");           

            //dictUriList.Add("/{pageName}", "/View/MainFrmView/{pageName}.xaml");

            //报表管理 通用页面--原始记录汇总表
            dictUriList.Add(PrivilegeENUM.ReportOriginRecord, "/View/ReportView/Xls_OriginRecSumReport.xaml");                            
                            
            //报表管理 五虎山           
            dictUriList.Add(PrivilegeENUM.ReportDepartAttendCollect, "/NewWuHuShan/DepartAttendCollect.xaml");
            //五虎山异常考勤查询模块
            dictUriList.Add(PrivilegeENUM.AbnormalAttendInfo, "/NewWuHuShan/AbnormalAttendInfo.xaml");
            dictUriList.Add(PrivilegeENUM.IrisAttendQuery, "/NewWuHuShan/IrisAttendQuery.xaml");

            //报表管理 神朔铁路
            dictUriList.Add(PrivilegeENUM.ReportPersonAttendCollect, "/ShenShuoRailway/PersonAttendCollect.xaml");
            dictUriList.Add(PrivilegeENUM.ReportShenShuoAttendCollect, "/ShenShuoRailway/ShenShuoAttendCollect.xaml");

            //报表管理 中科虹霸
            dictUriList.Add(PrivilegeENUM.ZhongKeHongBaPersonFullAttend, "/ZhongKeHongBa/PersonFullAttend.xaml");
            dictUriList.Add(PrivilegeENUM.ZhongKeHongBaLeakageAttendanceQuery, "/ZhongKeHongBa/PersonFullAttend.xaml");
            dictUriList.Add(PrivilegeENUM.ZhongKeHongBaPersonMealSupplement, "/ZhongKeHongBa/PersonFullAttend.xaml");
            dictUriList.Add(PrivilegeENUM.ZhongKeHongBaOriginRecSumList, "/ZhongKeHongBa/PersonFullAttend.xaml");
            dictUriList.Add(PrivilegeENUM.ZhongKeHongBaPersonLeaveList, "/ZhongKeHongBa/PersonFullAttend.xaml");
            dictUriList.Add(PrivilegeENUM.ZhongKeHongBaPersonLatearrivalList, "/ZhongKeHongBa/PersonFullAttend.xaml");
            dictUriList.Add(PrivilegeENUM.ZhongKeHongBaPersonTimeProblemList, "/ZhongKeHongBa/PersonFullAttend.xaml");
            //dictUriList.Add(PrivilegeENUM.ReportShenShuoAttendCollect, "/ShenShuoRailway/ShenShuoAttendCollect.xaml");
            //UriMapping a = new UriMapping();
            //a.Uri = new System.Uri("/AttendRecord");
            //a.MappedUri = new System.Uri("/View/AttendView/AttendRecord.xaml");
                            
            //<!-- 考勤管理 矿山页面-->
            //<uriMapper:UriMapping Uri="/InWellPersonMine" MappedUri="/ViewMine/AttendViewMine/InWellPersonMine.xaml"/>
            //<uriMapper:UriMapping Uri="/AttendRecordMine" MappedUri="/ViewMine/AttendViewMine/AttendRecordMine.xaml"/>
            //<uriMapper:UriMapping Uri="/RecogManageMine" MappedUri="/ViewMine/AttendViewMine/RecogManageMine.xaml"/>
            return dictUriList;
                                           
        }

        public virtual void SetAppLogo()
        {
        }
    }
}
