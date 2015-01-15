/*************************************************************************
** 文件名:   ReportQueryCondition.cs
** 主要类:   ReportQueryCondition
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   wz
** 日  期:   2013-12-30
** 修改人:   
** 日  期:
** 描  述:   周源山报表查询条件
**
** 版  本:   1.0.0
** 备  注:  命名及代码编写遵守C#编码规范
**
*************************************************************************/
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

namespace IriskingAttend.ZhouYuanShan
{
    /// <summary>
    /// 报表查询条件
    /// </summary>
    public class ReportQueryCondition
    {
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? BeginTime
        {
            get;
            set;
        }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime
        {
            get;
            set;
        }

        /// <summary>
        /// 选择的部门
        /// </summary>
        public List<int> DepartIds
        {
            get;
            set;
        }

        /// <summary>
        /// 选择的人员
        /// </summary>
        public List<int> PersonIds
        {
            get;
            set;
        }

        /// <summary>
        /// 选择的工种
        /// </summary>
        public List<int> WorkTypeIds
        {
            get;
            set;
        }

        /// <summary>
        /// 班次
        /// </summary>
        public List<int> ClassOrderIds
        {
            get;
            set;
        }

        /// <summary>
        /// 报表显示内容
        /// </summary>
        public ShowElementType ShowElementType
        {
            get;
            set;
        }

        /// <summary>
        /// 报表类型
        /// </summary>
        public ReportType ReportType
        {
            get;
            set;
        }

    }

    /// <summary>
    /// 班次类型（早班，中班，晚班及其组合）
    /// </summary>
    public enum ClassOrder
    {
        None = 0,
        Morning = 1,
        Noon =2,
        Night =4,
        MorningAndNoon = 3,
        MorningAndNight = 5,
        NoonAndNight = 6,
        All = 7,
    }

    /// <summary>
    /// 报表显示选择（班次，时长，时间及其组合）
    /// </summary>
    public enum ShowElementType
    {
        None = 0,
        ClassOrder =1,
        Duration =2,
        Time =4,
        ClassOrderAndDuration =3,
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
        MonthlyReportOnPerson = 1,
        DailyReportOnPerson = 2,
        DetailReportOnDepart = 3,
        MonthlyReportOnDepart = 4,
    }

}
