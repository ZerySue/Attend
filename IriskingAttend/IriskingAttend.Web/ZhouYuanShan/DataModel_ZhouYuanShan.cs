/*************************************************************************
** 文件名:   DataModel_ZhouYuanShan.cs
×× 主要类:   DataModel_ZhouYuanShan
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   wz
** 日  期:   2013-12-30
** 修改人:  
** 日  期:       
** 描  述:   周源山的数据类
**
** 版  本:   1.0.0
** 备  注:  命名及代码编写遵守C#编码规范
**
 * ***********************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
//using System.Data.Objects.DataClasses;
using System.Linq;
using System.ServiceModel.DomainServices.Hosting;
using System.ServiceModel.DomainServices.Server;
using System.Runtime.Serialization;
using System.Drawing;
using System.Net.Sockets;

namespace IriskingAttend.Web.ZhouYuanShan
{

    /// <summary>
    /// 报表显示选择（班次，时长，时间及其组合）
    /// </summary>
    [DataContract]
    public enum ShowElementType
    {
        None = 0,
        ClassOrder = 1,
        Duration = 2,
        Time = 4,
        ClassOrderAndDuration = 3,
        ClassOrderAndTime = 5,
        DurationAndTime = 6,
        All = 7,
    }

    /// <summary>
    /// 报表类型
    /// </summary>
    public enum ReportType
    {
        None = 0,
        MonthlyReportOnPerson=1,
        DailyReportOnPerson=2,
        DetailReportOnDepart=3,
        MonthlyReportOnDepart=4,
    }

    #region 周源山矿当日考勤报表

    /// <summary>
    /// 周源山矿当日考勤报表实体数据类
    /// </summary>
    
    public class PersonDayAttend
    {
        /// <summary>
        /// 序号
        /// </summary>
        [Key]
        public int Index { get; set; }

        /// <summary>
        /// 归属日
        /// </summary>        
        public string AttendDay { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>        
        public string DepartName { get; set; }

        /// <summary>
        /// 工号
        /// </summary>        
        public string WorkSn { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string PersonName { get; set; }

        /// <summary>
        /// 职位
        /// </summary>        
        public string Principal { get; set; }

        /// <summary>
        /// 入井时间
        /// </summary>        
        public string InWellTime { get; set; }

        /// <summary>
        /// 升井时间
        /// </summary>        
        public string OutWellTime { get; set; }

        /// <summary>
        /// 工时
        /// </summary>        
        public int WorkTime { get; set; }

        /// <summary>
        /// 记工
        /// </summary>        
        public float WorkCnt { get; set; }

        public PersonDayAttend()
        {
            DepartName = "";
            WorkSn = "";
            Principal = "";
            InWellTime = "";
            OutWellTime = "";
        }
    }

    /// <summary>
    /// 周源山矿部门月考勤报表实体数据类
    /// </summary>
    [DataContract]
    public class DepartMonthAttend
    {
        /// <summary>
        /// 序号
        /// </summary>
        [DataMember]
        [Key]
        public int KeyIndex { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        [DataMember]
        public string Index { get; set; }

        /// <summary>
        /// 归属日
        /// </summary>
        [DataMember]        
        public string AttendMonth { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        [DataMember]        
        public string DepartName { get; set; }

        /// <summary>
        /// 总工作时长
        /// </summary>
        [DataMember]
        public int TotalWorkTime { get; set; }

        /// <summary>
        /// 平均工作时长
        /// </summary>
        [DataMember]
        public int AvgWorkTime { get; set; }

        /// <summary>
        /// 总次数
        /// </summary>
        [DataMember]
        public int TotalTimes { get; set; }

        /// <summary>
        /// 有效次数
        /// </summary>
        [DataMember]
        public int ValidTimes { get; set; }

        /// <summary>
        /// 无效次数
        /// </summary>
        [DataMember]
        public int InvalidTimes { get; set; }

        /// <summary>
        /// 小于工时的次数
        /// </summary>
        [DataMember]
        public int LessAvailTimes { get; set; }

        /// <summary>
        /// 早班次数
        /// </summary>
        [DataMember]
        public int ZaoTimes { get; set; }

        /// <summary>
        /// 中班次数
        /// </summary>
        [DataMember]
        public int ZhongTimes { get; set; }

        /// <summary>
        /// 晚班次数
        /// </summary>
        [DataMember]
        public int WanTimes { get; set; }

        /// <summary>
        /// 0-2次数
        /// </summary>
        [DataMember]
        public int Sum0To2 { get; set; }

        /// <summary>
        /// 2-4次数
        /// </summary>
        [DataMember]
        public int Sum2To4 { get; set; }

        /// <summary>
        /// 4-8次数
        /// </summary>
        [DataMember]
        public int Sum4To8 { get; set; }

        /// <summary>
        /// 8-12次数
        /// </summary>
        [DataMember]
        public int Sum8To12 { get; set; }

        /// <summary>
        /// >12次数
        /// </summary>
        [DataMember]
        public int Sum12Up { get; set; }

        /// <summary>
        /// 每日班次的计数
        /// </summary>
        [DataMember]
        public int[] classOrderCount { get; set; }

        public DepartMonthAttend()
        {
            DepartName = "";
            TotalWorkTime = 0;
            AvgWorkTime = 0;
            TotalTimes = 0;
            ValidTimes = 0;
            InvalidTimes = 0;
            LessAvailTimes = 0;
            ZaoTimes = 0;
            ZhongTimes = 0;
            WanTimes = 0;
            Sum0To2 = 0;
            Sum2To4 = 0;
            Sum4To8 = 0;
            Sum8To12 = 0;
            Sum12Up = 0;

            classOrderCount = new int[31];
        }
    }

     /// <summary>
    /// 周源山矿个人考勤月报表实体数据类
    /// </summary>
    [DataContract]
    public class PersonAttendStatistics
    {
        /// <summary>
        /// 月份
        /// </summary>
        [Key]
        [DataMember]
        public string month { get; set; }

        /// <summary>
        /// 部门
        /// </summary>
        [DataMember]
        public string depart_name { get; set; }

        /// <summary>
        /// 部门id
        /// </summary>
        [DataMember]
        public int depart_id { get; set; }

        /// <summary>
        /// id
        /// </summary>
        [Key]
        [DataMember]
        public int person_id { get; set; }

        /// <summary>
        /// 人员名称
        /// </summary>
        [DataMember]
        public string name { get; set; }

        /// <summary>
        /// 工号
        /// </summary>
        [DataMember]
        public string work_sn { get; set; }

        /// <summary>
        /// 工种id
        /// </summary>
        [DataMember]
        public int work_type_id { get; set; }

         /// <summary>
        /// 工种名称
        /// </summary>
        [DataMember]
        public string work_type_name { get; set; }        

        /// <summary>
        /// 总工作时长
        /// </summary>
        [DataMember]
        public int total_work_time { get; set; }

        /// <summary>
        /// 平均工作时长
        /// </summary>
        [DataMember]
        public int avg_work_time { get; set; }

        /// <summary>
        /// 总次数
        /// </summary>
        [DataMember]
        public int total_times { get; set; }

        /// <summary>
        /// 有效次数
        /// </summary>
        [DataMember]
        public int valid_times { get; set; }

        /// <summary>
        /// 无效次数
        /// </summary>
        [DataMember]
        public int invalid_times { get; set; }

        /// <summary>
        /// 工时小于规定记工时长的出勤次数
        /// </summary>
        [DataMember]
        public int less_avail_times { get; set; }

        /// <summary>
        /// 早班次数
        /// </summary>
        [DataMember]
        public int zao_times { get; set; }

        /// <summary>
        /// 中班次数
        /// </summary>
        [DataMember]
        public int zhong_times { get; set; }

        /// <summary>
        /// 晚班次数
        /// </summary>
        [DataMember]
        public int wan_times { get; set; }

        /// <summary>
        /// 0-2次数
        /// </summary>
        [DataMember]
        public int sum_0_2 { get; set; }

        /// <summary>
        /// 2-4次数
        /// </summary>
        [DataMember]
        public int sum_2_4 { get; set; }

        /// <summary>
        /// 4-8次数
        /// </summary>
        [DataMember]
        public int sum_4_8 { get; set; }

        /// <summary>
        /// 8-12次数
        /// </summary>
        [DataMember]
        public int sum_8_12 { get; set; }

        /// <summary>
        /// >12次数
        /// </summary>
        [DataMember]
        public int sum_12 { get; set; }

        /// <summary>
        /// 每日内容描述
        /// </summary>
        [DataMember]
        public string daily_content_description { get; set; }
        
        /// <summary>
        /// 班次简称
        /// </summary>
        [DataMember]
        public string[] attend_signs { get; set; }

        /// <summary>
        /// 工作时长
        /// </summary>
        [DataMember]
        public string[] work_times { get; set; }

        /// <summary>
        /// 时间描述
        /// </summary>
        [DataMember]
        public string[] time_descriptions { get; set; }

        /// <summary>
        /// 每日显示内容
        /// </summary>
        [DataMember]
        public string[] display_content { get; set; }

      

        /// <summary>
        /// 每日显示内容的颜色
        /// </summary>
        [DataMember]
        public string[] display_content_color { get; set; }

        /// <summary>
        /// 每日班次的计数
        /// </summary>
        [DataMember]
        public int[] classOrderCount { get; set; }
    

        

        public PersonAttendStatistics()
        {
            depart_name = "";
            name = "";
            work_sn = "";
            work_type_name = "";
            daily_content_description = "";
            month = "";
            display_content = new string[31];
            attend_signs = new string[31];
            work_times = new string[31];
            time_descriptions = new string[31];
            display_content_color = new string[31];
            classOrderCount = new int[31];

            for (int i = 0; i < 31; i++)
            {
                display_content[i] = "";
                attend_signs[i] = "";
                work_times[i] = "";
                time_descriptions[i] = "";
                display_content_color[i] = "0xff000000";
                classOrderCount[i] = 0;
            }

            //content_1 = "";
            //content_2 = "";
            //content_3 = "";
            //content_4 = "";
            //content_5 = "";
            //content_6 = "";
            //content_7 = "";
            //content_8 = "";
            //content_9 = "";
            //content_10 = "";
            //content_11 = "";
            //content_12 = "";
            //content_13 = "";
            //content_14 = "";
            //content_15 = "";
            //content_16 = ""; 
            //content_17 = "";
            //content_18 = "";
            //content_19 = "";
            //content_20 = "";
            //content_21 = "";
            //content_22 = "";
            //content_23 = "";
            //content_24 = "";
            //content_25 = "";
            //content_26 = "";
            //content_27 = "";
            //content_28 = "";
            //content_29 = "";
            //content_30 = "";
            //content_31 = "";
        }

        
    }
    


    #endregion
}