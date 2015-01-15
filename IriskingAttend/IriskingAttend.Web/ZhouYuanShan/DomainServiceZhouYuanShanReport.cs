/*************************************************************************
** 文件名:   DomainServiceIriskingAttend_ZhouYuanShan.cs
×× 主要类:   DomainServiceIriskingAttend_ZhouYuanShan
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2014-1-12
** 修改人:  
** 日  期:       
** 描  述:   周源山单独定制域服务
**
** 版  本:   1.0.0
** 备  注:  命名及代码编写遵守C#编码规范
**
 * ***********************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.ServiceModel.DomainServices.Hosting;
using System.ServiceModel.DomainServices.Server;
using Irisking.Web.DataModel;
using System.Data;
using Irisking.DataBaseAccess;
using ServerCommunicationLib;
using System.Drawing;
using System.Web;
using Npgsql;
using System.Text;
using IriskingAttend.Web.ZhouYuanShan;
using NpgsqlTypes;
using System.Reflection;

namespace IriskingAttend.Web
{
     // TODO: 创建包含应用程序逻辑的方法。
    public partial class DomainServiceIriskingAttend : DomainService
    {
        #region 周源山个人日报表查询页面

        public static List<PersonDayAttend> DayPersonAttendList = new List<PersonDayAttend>();
        
        /// <summary>
        /// 周源山个人日报表查询页面
        /// </summary>
        /// <returns></returns>
        [Query(HasSideEffects = true)]
        public IEnumerable<PersonDayAttend> GetDayPersonAttendZhouYuanShan( string sqlQuery)
        {

            int over_time = 0;
            string paramSQL = @"select over_time from system_param";
            DataTable paramDt = DbAccess.POSTGRESQL.Select(paramSQL, "system_param");
            if (null != paramDt && paramDt.Rows.Count >= 1 )
            {
                over_time = Int16.Parse(paramDt.Rows[0]["over_time"].ToString());
            }

            DayPersonAttendList = new List<PersonDayAttend>();

            DataTable dt = DbAccess.POSTGRESQL.Select(sqlQuery, "");

            if (dt.Rows == null)
            {
                return DayPersonAttendList;
            }
            int index = 1;
            PersonDayAttend totalItem = new PersonDayAttend();
            totalItem.AttendDay = "合计";
            totalItem.WorkCnt = 0f;
            totalItem.WorkTime = 0;
            foreach (DataRow ar in dt.Rows)
            {
                PersonDayAttend item = new PersonDayAttend();

                item.Index = index++;

                #region     数据填充
                if (ar["attend_day"] != DBNull.Value)
                {
                    item.AttendDay = DateTime.Parse(ar["attend_day"].ToString()).ToString("yyyy-MM-dd");
                }
                if (ar["depart_name"] != DBNull.Value)
                {
                    item.DepartName = ar["depart_name"].ToString();
                }
                if (ar["work_sn"] != DBNull.Value)
                {
                    item.WorkSn = ar["work_sn"].ToString();
                }
                if (ar["name"] != DBNull.Value)
                {
                    item.PersonName = ar["name"].ToString();
                }
                if (ar["principal_name"] != DBNull.Value)
                {
                    item.Principal = ar["principal_name"].ToString();
                }
                if (ar["in_well_time"] != DBNull.Value)
                {
                    item.InWellTime = ar["in_well_time"].ToString();
                }
                if (ar["out_well_time"] != DBNull.Value)
                {
                    item.OutWellTime = ar["out_well_time"].ToString();
                }
                if (ar["work_time"] != DBNull.Value)
                {
                    item.WorkTime = int.Parse( ar["work_time"].ToString());
                    totalItem.WorkTime += item.WorkTime;
                }
                if (ar["work_cnt"] != DBNull.Value)
                {
                    if (Int32.Parse( ar["work_time"].ToString()) > over_time)
                    {
                        item.WorkCnt = 0;
                    }
                    else
                    {
                        item.WorkCnt = float.Parse(ar["work_cnt"].ToString()) / 10;
                    }
                    totalItem.WorkCnt += item.WorkCnt;
                }
                #endregion

                DayPersonAttendList.Add(item);
            }
            //totalItem.WorkTime = (float)Math.Round(totalItem.WorkTime, 1);
            DayPersonAttendList.Add( totalItem);
            return DayPersonAttendList;
        }

        #endregion

        #region 周源山部门月报表查询页面

        public static List<DepartMonthAttend> DepartMonthAttendList = new List<DepartMonthAttend>();

        /// <summary>
        /// 周源山部门月报表查询页面
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DepartMonthAttend> GetDepartMonthAttendZhouYuanShan(DateTime beginTime, DateTime endTime, int[] departIds)
        {
            DepartMonthAttendList = new List<DepartMonthAttend>();

            int monthCount = (endTime.Year - beginTime.Year) * 12 + endTime.Month - beginTime.Month + 1;
            List<DepartMonthAttend>[] monthAttend = new List<DepartMonthAttend>[monthCount];

            string monthBeginTime = "";
            string monthEndTime = "";
            string sqlQuery = "";

            string departWhere = "";
            if (departIds != null && departIds.Count() > 0)
            {
                departWhere = string.Format(" d.depart_id in ( ");
                for (int index = 0; index < departIds.Count(); index++)
                {
                    departWhere += string.Format("{0}, ", departIds[index]);
                }

                departWhere = departWhere.Remove(departWhere.LastIndexOf(","), 1);
                departWhere += ") and ";
            }
            else //如果没有部门条件，则查询结果为空（为权限考虑）
            {
                departWhere = string.Format(" d.depart_id = -1 and ");
            }

            DepartMonthAttend totalItem = new DepartMonthAttend();
            totalItem.Index = "合计";
            int totalIndex = 1;
            for (int index = 0; index < monthCount; index++)
            {
                monthAttend[index] = new List<DepartMonthAttend>();

                if (index == 0)
                {
                    monthBeginTime = beginTime.ToString("yyyy-MM-dd 00:00:00");
                }
                else
                {
                    monthBeginTime = beginTime.AddMonths(index).ToString("yyyy-MM-01 00:00:00");
                }

                if (index == (monthCount - 1))
                {
                    monthEndTime = endTime.ToString("yyyy-MM-dd 23:59:59");
                }
                else
                {
                    monthEndTime = DateTime.Parse(beginTime.AddMonths(index+1).ToString("yyyy-MM-01 00:00:00")).AddDays(-1).ToString("yyyy-MM-dd 23:59:59");
                }

                sqlQuery = string.Format(@"select depart_name, SUM(total_work_time) total_work_time,
                SUM(less_avail_times) less_avail_times,
                CASE WHEN (SUM(valid_times)+SUM(less_avail_times))=0 THEN 0 ELSE SUM(total_work_time)/(SUM(valid_times)+SUM(less_avail_times)) END avg_work_time,
                sum( total_times ) total_times,
                SUM(valid_times) valid_times, SUM(invalid_times) invalid_times,
                SUM(zao_times) zao_times, SUM(zhong_times) zhong_times, SUM(wan_times) wan_times,
                SUM(sum_0_2) sum_0_2, SUM(sum_2_4) sum_2_4, SUM(sum_4_8) sum_4_8, SUM(sum_8_12) sum_8_12, SUM(sum__12) sum__12,
                array_agg(day_offset) as day_offsets,      --------考勤日期分组
                array_agg(valid_times) as day_valid_times    --------班次简称分组
                from
                    (SELECT d.depart_name, 
                        SUM(S.total_work_time) total_work_time,
                        SUM(less_avail_times) less_avail_times,
                        CASE WHEN (SUM(valid_times)+SUM(less_avail_times))=0 THEN 0 ELSE SUM(S.total_work_time)/(SUM(valid_times)+SUM(less_avail_times)) END avg_work_time,
                        count(*) total_times,
                        SUM(S.valid_times) valid_times, SUM(S.invalid_times) invalid_times,
                        SUM(S.zao_times) zao_times, SUM(S.zhong_times) zhong_times, SUM(S.wan_times) wan_times,
                        SUM(S.t_0_2) sum_0_2, SUM(S.t_2_4) sum_2_4, SUM(S.t_4_8) sum_4_8, SUM(S.t_8_12) sum_8_12, SUM(S.t__12) sum__12,
                        max(day_offset) as day_offset  
                            FROM (
                                -- 将基本数据分类
                                SELECT *, CASE WHEN NOT gt_over_time THEN work_time ELSE 0 END total_work_time,		--总时长
                                    CASE WHEN gt_avail_time AND NOT gt_over_time THEN 1 ELSE 0 END valid_times, --有效次数
                                    CASE WHEN NOT incomplete AND NOT gt_avail_time THEN 1 ELSE 0 END less_avail_times, --工时小于规定记工时长的出勤次数
                                    CASE WHEN incomplete OR NOT gt_avail_time OR gt_over_time THEN 1 ELSE 0 END invalid_times, --无效次数
                                    CASE WHEN attend_sign='早' AND gt_avail_time AND NOT gt_over_time THEN 1 ELSE 0 END zao_times, --早班次数
                                    CASE WHEN attend_sign='中' AND gt_avail_time AND NOT gt_over_time THEN 1 ELSE 0 END zhong_times, --中班次数
                                    CASE WHEN attend_sign='晚' AND gt_avail_time AND NOT gt_over_time THEN 1 ELSE 0 END wan_times --晚班次数
                                FROM (
                                    -- 先将基本的数据计算出来
                                    SELECT person_id, ar.class_order_id, co.class_order_name, co.attend_sign, attend_day, in_well_time, out_well_time, work_time, ar.work_cnt, co.avail_time,
                                        EXTRACT(EPOCH FROM date_trunc('day',attend_day)-'{1}')/(24*3600) as day_offset,
                                        (work_time >= co.avail_time) gt_avail_time,
                                        (work_time > (SELECT over_time FROM system_param LIMIT 1)) gt_over_time,
                                        (in_well_time IS NULL OR out_well_time IS NULL) incomplete,
                                        CASE WHEN (in_well_time IS NOT NULL AND out_well_time IS NOT NULL AND work_time <= 2*60) THEN 1 ELSE 0 END t_0_2,
                                        CASE WHEN (2*60 < work_time AND work_time <= 4*60)  THEN 1 ELSE 0 END t_2_4,
                                        CASE WHEN (4*60 < work_time AND work_time <= 8*60)  THEN 1 ELSE 0 END t_4_8,
                                        CASE WHEN (8*60 < work_time AND work_time <= 12*60)  THEN 1 ELSE 0 END t_8_12,
                                        CASE WHEN (12*60 < work_time)  THEN 1 ELSE 0 END t__12
                                FROM attend_record_base ar LEFT JOIN 
                                       (SELECT con.*, costd.avail_time, costd.work_cnt FROM class_order_normal con, ""class_order.standard"" costd WHERE con.class_order_id=costd.class_order_id) co
                                    ON ar.class_order_id=co.class_order_id
                                ) SALL
                            ) S, person_base p, depart d
                       WHERE {0} S.person_id=p.person_id AND p.depart_id=d.depart_id AND attend_day >= '{1}' AND attend_day <= '{2}'
                       GROUP BY d.depart_name,day_offset
                       ORDER BY depart_name,day_offset
                )Sdepart group by depart_name order by depart_name;", departWhere, monthBeginTime, monthEndTime);

                DataTable dt = DbAccess.POSTGRESQL.Select(sqlQuery, "");

                if (dt != null && dt.Rows != null && dt.Rows.Count > 0)
                {
                    int departIndex = 1;
                    DepartMonthAttend departTotalItem = new DepartMonthAttend();
                    departTotalItem.Index = "小计";
                    departTotalItem.AttendMonth = beginTime.AddMonths(index).ToString("MM月");
                    
                    foreach (DataRow ar in dt.Rows)
                    {
                        DepartMonthAttend item = new DepartMonthAttend();

                        item.KeyIndex = totalIndex++;
                        item.Index = departIndex++.ToString();
                       
                        #region     数据填充

                        item.AttendMonth = departTotalItem.AttendMonth;

                        if (ar["depart_name"] != DBNull.Value)
                        {
                            item.DepartName = ar["depart_name"].ToString();
                        }
                        
                        if (ar["total_work_time"] != DBNull.Value)
                        {
                            item.TotalWorkTime = Int32.Parse(ar["total_work_time"].ToString());
                            departTotalItem.TotalWorkTime += item.TotalWorkTime;
                        }
                        if (ar["avg_work_time"] != DBNull.Value)
                        {
                            item.AvgWorkTime = (Int32)(Double.Parse(ar["avg_work_time"].ToString()));
                        }
                        if (ar["total_times"] != DBNull.Value)
                        {
                            item.TotalTimes = Int32.Parse(ar["total_times"].ToString());
                            departTotalItem.TotalTimes += item.TotalTimes;
                        }
                        if (ar["valid_times"] != DBNull.Value)
                        {
                            item.ValidTimes = Int32.Parse(ar["valid_times"].ToString());
                            departTotalItem.ValidTimes += item.ValidTimes;
                        }
                        if (ar["invalid_times"] != DBNull.Value)
                        {
                            item.InvalidTimes = Int32.Parse(ar["invalid_times"].ToString());
                            departTotalItem.InvalidTimes += item.InvalidTimes;
                        }
                        if (ar["less_avail_times"] != DBNull.Value)
                        {
                            item.LessAvailTimes = Int32.Parse(ar["less_avail_times"].ToString());
                            departTotalItem.LessAvailTimes += item.LessAvailTimes;
                        }
                        if (ar["zao_times"] != DBNull.Value)
                        {
                            item.ZaoTimes = Int32.Parse(ar["zao_times"].ToString());
                            departTotalItem.ZaoTimes += item.ZaoTimes;
                        }
                        if (ar["zhong_times"] != DBNull.Value)
                        {
                            item.ZhongTimes = Int32.Parse(ar["zhong_times"].ToString());
                            departTotalItem.ZhongTimes += item.ZhongTimes;
                        }
                        if (ar["wan_times"] != DBNull.Value)
                        {
                            item.WanTimes = Int32.Parse(ar["wan_times"].ToString());
                            departTotalItem.WanTimes += item.WanTimes;
                        }
                        if (ar["sum_0_2"] != DBNull.Value)
                        {
                            item.Sum0To2 = Int32.Parse(ar["sum_0_2"].ToString());
                            departTotalItem.Sum0To2 += item.Sum0To2;
                        }
                        if (ar["sum_2_4"] != DBNull.Value)
                        {
                            item.Sum2To4 = Int32.Parse(ar["sum_2_4"].ToString());
                            departTotalItem.Sum2To4 += item.Sum2To4;
                        }
                        if (ar["sum_4_8"] != DBNull.Value)
                        {
                            item.Sum4To8 = Int32.Parse(ar["sum_4_8"].ToString());
                            departTotalItem.Sum4To8 += item.Sum4To8;
                        }
                        if (ar["sum_8_12"] != DBNull.Value)
                        {
                            item.Sum8To12 = Int32.Parse(ar["sum_8_12"].ToString());
                            departTotalItem.Sum8To12 += item.Sum8To12;
                        }
                        if (ar["sum__12"] != DBNull.Value)
                        {
                            item.Sum12Up = Int32.Parse(ar["sum__12"].ToString());
                            departTotalItem.Sum12Up += item.Sum12Up;
                        }

                        double[] dayOffset = (double[])ar["day_offsets"];
                        Int64[] day_valid_times = (Int64[])ar["day_valid_times"];

                        if (dayOffset != null && dayOffset.Count() > 0)
                        {
                            for (int dayIndex = 0; dayIndex < dayOffset.Count(); dayIndex++)
                            {
                                item.classOrderCount[(int)dayOffset[dayIndex]] = (int)day_valid_times[dayIndex];
                                departTotalItem.classOrderCount[(int)dayOffset[dayIndex]] += (int)day_valid_times[dayIndex];
                            }
                        }
                        #endregion

                        monthAttend[index].Add(item);
                    }
                    departTotalItem.KeyIndex = totalIndex++;
                    if (departTotalItem.LessAvailTimes + departTotalItem.ValidTimes > 0)
                    {
                        departTotalItem.AvgWorkTime = departTotalItem.TotalWorkTime / (departTotalItem.LessAvailTimes + departTotalItem.ValidTimes);
                    }
                    monthAttend[index].Add(departTotalItem);

                    totalItem.TotalWorkTime += departTotalItem.TotalWorkTime;                   
                    totalItem.TotalTimes += departTotalItem.TotalTimes;
                    totalItem.ValidTimes += departTotalItem.ValidTimes;
                    totalItem.InvalidTimes += departTotalItem.InvalidTimes;
                    totalItem.LessAvailTimes += departTotalItem.LessAvailTimes;
                    totalItem.ZaoTimes += departTotalItem.ZaoTimes;
                    totalItem.ZhongTimes += departTotalItem.ZhongTimes;
                    totalItem.WanTimes += departTotalItem.WanTimes;
                    totalItem.Sum0To2 += departTotalItem.Sum0To2;
                    totalItem.Sum2To4 += departTotalItem.Sum2To4;
                    totalItem.Sum4To8 += departTotalItem.Sum4To8;
                    totalItem.Sum8To12 += departTotalItem.Sum8To12;
                    totalItem.Sum12Up += departTotalItem.Sum12Up;

                    for (int dayIndex = 0; dayIndex < departTotalItem.classOrderCount.Count(); dayIndex++)
                    {
                        totalItem.classOrderCount[dayIndex] += departTotalItem.classOrderCount[dayIndex];
                    }
                }
            }

            totalItem.KeyIndex = totalIndex++;
            if (totalItem.LessAvailTimes + totalItem.ValidTimes > 0)
            {
                totalItem.AvgWorkTime = totalItem.TotalWorkTime / (totalItem.LessAvailTimes + totalItem.ValidTimes);
            }

            totalIndex= 1;
            if (monthCount == 1)
            {
                if (monthAttend[0].Count() >= 1)
                {
                    for (int tempIndex = 0; tempIndex < monthAttend[0].Count() - 1; tempIndex++)
                    {
                        monthAttend[0][tempIndex].Index = totalIndex++.ToString();
                        DepartMonthAttendList.Add(monthAttend[0][tempIndex]);
                    }
                    DepartMonthAttendList.Add(totalItem);
                }
            }
            else if (departIds != null && departIds.Count() == 1)
            {
                for (int tempIndex = 0; tempIndex < monthAttend.Count(); tempIndex++)
                {
                    if (monthAttend[tempIndex].Count() >= 1)
                    {
                        monthAttend[tempIndex][0].Index = totalIndex++.ToString();
                        DepartMonthAttendList.Add(monthAttend[tempIndex][0]);
                    }
                }
                DepartMonthAttendList.Add(totalItem);
            }
            else
            {
                for (int i = 0; i < monthAttend.Count(); i++)
                {
                    if (monthAttend[i].Count() >= 1)
                    {
                        int j = 0;
                        for ( j = 0; j < monthAttend[i].Count()-1; j++)
                        {
                            monthAttend[i][j].Index = totalIndex++.ToString();
                            DepartMonthAttendList.Add(monthAttend[i][j]);
                        }
                        
                        DepartMonthAttendList.Add(monthAttend[i][j]);                       
                    }
                }
                DepartMonthAttendList.Add(totalItem);
            }

            return DepartMonthAttendList;
        }

        #endregion

        
        #region 周源山个人月报表 和 部门详单报表

        public static List<PersonAttendStatistics> PersonAttendStatisticsList = new List<PersonAttendStatistics>();
        public static List<PersonAttendStatistics> DepartDetailList = new List<PersonAttendStatistics>();
        public static int ConditionShowElementType = 0;
        /// <summary>
        /// 查询个人考勤月报表数据(部门详单)
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="departIds"></param>
        /// <param name="personIds"></param>
        /// <param name="workTypeIds"></param>
        /// <returns></returns>
        [Query(HasSideEffects=true)]
        public IEnumerable<PersonAttendStatistics> GetPersonAttendStatistics(DateTime beginTime, DateTime endTime, 
            int[] departIds, int[] personIds, int[] workTypeIds, int[] classOrderIds, int _showElementType,int _reportType)
        {
            ShowElementType showElementType = (ShowElementType)_showElementType;
            List<PersonAttendStatistics> query  = new List<PersonAttendStatistics>();

            //如果时间跨度大于一个月，则每个月单独查询数据
            int monthCount = endTime.Month - beginTime.Month ;
            monthCount = monthCount < 0 ? monthCount + 12 : monthCount;
            monthCount++;
            DateTime[] beginTimes = new DateTime[monthCount];
            DateTime[] endTimes = new DateTime[monthCount];
            for (int i = 0; i < monthCount; i++)
            {
                beginTimes[i] = beginTime.AddMonths(i);
                endTimes[i] = endTime.AddMonths(-(monthCount - i - 1));
            }
            

            for (int timeIndex = 0; timeIndex < monthCount; timeIndex++)
            {
                string sql = string.Format(
                    @"SELECT * FROM get_person_attend_statistics('{0}','{1}') as 
                    (depart_name VARCHAR,depart_id integer ,name VARCHAR,person_id integer, work_sn text, work_type_id integer,work_type_name text,                     
                     total_work_time bigint,avg_work_time bigint,
                     total_times bigint,valid_times bigint,invalid_times bigint, less_avail_times bigint,
                     zao_times bigint,zhong_times bigint,wan_times bigint,
                     sum_0_2 bigint,sum_2_4 bigint,sum_4_8 bigint,sum_8_12 bigint,sum_12 bigint,
                     class_order_ids integer[],attend_days date[],attend_signs VARCHAR[], work_times integer[],attend_time_descriptions TEXT[],invalid_flags integer[],
                     invalid_details integer[])
                    ", beginTimes[timeIndex], endTimes[timeIndex]);

                //组装sql语句的where条件          
                string sql_where = "";
                if (departIds != null && departIds.Count() > 0)
                {
                    sql_where = string.Format(" depart_id in ( ");
                    for (int index = 0; index < departIds.Count(); index++)
                    {
                        sql_where += string.Format("{0}, ", departIds[index]);
                    }

                    sql_where = sql_where.Remove(sql_where.LastIndexOf(","), 1);
                    sql_where += ")";
                }
                else//如果没有部门条件，则查询结果为空（为权限考虑）
                {
                    sql_where = "depart_id = -1";
                }

                if (personIds != null && personIds.Count() > 0)
                {
                    sql_where += string.Format(" and person_id in ( ");
                    for (int index = 0; index < personIds.Count(); index++)
                    {
                        sql_where += string.Format(" {0}, ", personIds[index]);
                    }

                    sql_where = sql_where.Remove(sql_where.LastIndexOf(","), 1);
                    sql_where += ")";
                }
                else//如果没有人员条件，则查询结果为空（为权限考虑）
                {
                    sql_where += "and person_id = -1";
                }

                if (workTypeIds != null && workTypeIds.Count() > 0)
                {
                    sql_where += string.Format(" and work_type_id in ( ");
                    for (int index = 0; index < workTypeIds.Count(); index++)
                    {
                        sql_where += string.Format("{0}, ", workTypeIds[index]);
                    }

                    sql_where = sql_where.Remove(sql_where.LastIndexOf(","), 1);
                    sql_where += ")";
                }

                if (sql_where.CompareTo("") != 0)
                {
                    sql += ( " where " + sql_where);
                }

                DataTable dt = DbAccess.POSTGRESQL.Select(sql, "");

                if (dt == null || dt.Rows == null)
                {
                    return query;
                }

                int lastDepartStartIndex = 0;
                for(int rowIndex = 0; rowIndex < dt.Rows.Count; rowIndex++ )
                {
                    DataRow ar = dt.Rows[rowIndex];
                    PersonAttendStatistics item = new PersonAttendStatistics();

                    int[] class_order_ids = null;                    
                    DateTime[] attend_days = new DateTime[0];
                    string[] attend_signs = null;
                    string[] attend_time_descriptions = null;
                    int[] work_times = null;
                    int[] invalid_flags = null;
                    int[] invalid_details = null;  

                    #region     数据填充
                    if (ar["depart_name"] != DBNull.Value)
                    {
                        item.depart_name = Convert.ToString(ar["depart_name"]);
                    }
                    if (ar["depart_id"] != DBNull.Value)
                    {
                        item.depart_id = Convert.ToInt32(ar["depart_id"]);
                    }
                    if (ar["class_order_ids"] != DBNull.Value)
                    {
                        class_order_ids = (int[])ar["class_order_ids"];
                    }                    
                    if (ar["attend_days"] != DBNull.Value)
                    {
                        NpgsqlDate[] temp = (NpgsqlDate[])(ar["attend_days"]);
                        attend_days = new DateTime[temp.Length];
                        for (int index = 0; index < temp.Length; index++)
                        {
                            attend_days[index] = (DateTime)temp[index];
                        }
                        invalid_flags = (int[])ar["invalid_flags"];
                        invalid_details = (int[])ar["invalid_details"];                        
                    }
                    if (ar["attend_signs"] != DBNull.Value)
                    {
                        attend_signs = (string[])(ar["attend_signs"]);
                    }
                    if (ar["attend_time_descriptions"] != DBNull.Value)
                    {
                        attend_time_descriptions = (string[])(ar["attend_time_descriptions"]);
                    }
                    if (ar["avg_work_time"] != DBNull.Value)
                    {
                        item.avg_work_time = Convert.ToInt32(ar["avg_work_time"]);
                    }
                    if (ar["invalid_times"] != DBNull.Value)
                    {
                        item.invalid_times = Convert.ToInt32(ar["invalid_times"]);
                    }
                    if (ar["less_avail_times"] != DBNull.Value)
                    {
                        item.less_avail_times = Convert.ToInt32(ar["less_avail_times"]);
                    }
                    if (ar["name"] != DBNull.Value)
                    {
                        item.name = (string)(ar["name"]);
                    }
                    if (ar["person_id"] != DBNull.Value)
                    {
                        item.person_id = Convert.ToInt32(ar["person_id"]);
                    }
                    if (ar["sum_0_2"] != DBNull.Value)
                    {
                        item.sum_0_2 = Convert.ToInt32(ar["sum_0_2"]);
                    }
                    if (ar["sum_12"] != DBNull.Value)
                    {
                        item.sum_12 = Convert.ToInt32(ar["sum_12"]);
                    }
                    if (ar["sum_2_4"] != DBNull.Value)
                    {
                        item.sum_2_4 = Convert.ToInt32(ar["sum_2_4"]);
                    }
                    if (ar["sum_4_8"] != DBNull.Value)
                    {
                        item.sum_4_8 = Convert.ToInt32(ar["sum_4_8"]);
                    }
                    if (ar["sum_8_12"] != DBNull.Value)
                    {
                        item.sum_8_12 = Convert.ToInt32(ar["sum_8_12"]);
                    }
                    if (ar["total_times"] != DBNull.Value)
                    {
                        item.total_times = Convert.ToInt32(ar["total_times"]);
                    }
                    if (ar["total_work_time"] != DBNull.Value)
                    {
                        item.total_work_time = Convert.ToInt32(ar["total_work_time"]);
                    }
                    if (ar["valid_times"] != DBNull.Value)
                    {
                        item.valid_times = Convert.ToInt32(ar["valid_times"]);
                    }
                    if (ar["wan_times"] != DBNull.Value)
                    {
                        item.wan_times = Convert.ToInt32(ar["wan_times"]);
                    }
                    if (ar["work_sn"] != DBNull.Value)
                    {
                        item.work_sn = (string)(ar["work_sn"]);
                    }
                    if (ar["work_times"] != DBNull.Value)
                    {
                        work_times = (int[])(ar["work_times"]);
                    }
                    if (ar["work_type_id"] != DBNull.Value)
                    {
                        item.work_type_id = Convert.ToInt32(ar["work_type_id"]);
                    }
                    if (ar["work_type_name"] != DBNull.Value)
                    {
                        item.work_type_name = (string)(ar["work_type_name"]);
                    }
                   
                    if (ar["zao_times"] != DBNull.Value)
                    {
                        item.zao_times = Convert.ToInt32(ar["zao_times"]);
                    }
                    if (ar["zhong_times"] != DBNull.Value)
                    {
                        item.zhong_times = Convert.ToInt32(ar["zhong_times"]);
                    }

                    #endregion

                    #region 构造每日内容

                    bool bfirst = true;
                    item.daily_content_description = "";
                    if (((int)showElementType & (int)ShowElementType.ClassOrder) != 0X00)
                    {
                        item.daily_content_description = "班次";
                        bfirst = false;
                    }

                    if (((int)showElementType & (int)ShowElementType.Duration) != 0X00)
                    {
                        if (!bfirst)
                        {
                            item.daily_content_description += "\r\n";                            
                        }

                        item.daily_content_description += "时长";
                        bfirst = false;
                    }

                    if (((int)showElementType & (int)ShowElementType.Time) != 0X00)
                    {
                        if (!bfirst)
                        {
                            item.daily_content_description += "\r\n";                           
                        }
                        item.daily_content_description += "时间";
                    }

                    item.month = attend_days[0].Month + "月";

                    
                    for (int i = 0; i < item.attend_signs.Count(); i++)
                    {
                        item.attend_signs[i] = "";
                        item.work_times[i] = "";
                        item.time_descriptions[i] = "";
                    }

                    //如果每日的班次都不在classOrderIds条件中，说明此人这个月的内容为空，就不用添加了
                    bool bAdd = false;

                    //构造每日内容
                    for (int i = 0; i < attend_days.Length; i++)
                    {
                        int dayIndex = attend_days[i].Day - 1;
                        bool bClassOrderId = true;

                        //如果设置了班次过滤条件，则启用班次id过滤
                        if (classOrderIds != null && classOrderIds.Length > 0)
                        {
                            if (!classOrderIds.Contains(class_order_ids[i]))
                            {
                                bClassOrderId = false;
                            }
                        }

                        if (bClassOrderId)
                        {
                            bAdd = true;

                            if (((int)showElementType & (int)ShowElementType.ClassOrder) != 0X00)
                            {
                                if (item.attend_signs[dayIndex].Length > 0)
                                {
                                    item.attend_signs[dayIndex] += ",";
                                }
                                item.attend_signs[dayIndex] += (attend_signs[i]);                               
                            }

                            if (((int)showElementType & (int)ShowElementType.Duration) != 0X00)
                            {
                                if (item.work_times[dayIndex].Length > 0)
                                {
                                    item.work_times[dayIndex] += ",";
                                }
                                item.work_times[dayIndex] += string.Format("{0}:{1}", work_times[i]/60, (work_times[i]%60).ToString("d2"));
                            }

                            if (((int)showElementType & (int)ShowElementType.Time) != 0X00)
                            {
                                if (item.time_descriptions[dayIndex].Length > 0)
                                {
                                    item.time_descriptions[dayIndex] += ",";
                                }
                                item.time_descriptions[dayIndex] += (attend_time_descriptions[i]);
                            }
                        }

                        //如果该天是无效考勤,用不同颜色表示
                        if (invalid_flags[i] == 1)
                        {
                            //不完整考勤用黄色，时间不够用绿色。超时用红色， 
                            //1：不完整记录标志描述 2：时长有效记录标志描述 3：超时记录标志描述 0：有效 
                            if (invalid_details[i] == 1)
                            {
                                item.display_content_color[dayIndex] = "0xffffa500";//Color.Orange "0xffffff00";//Color.Yellow.ToArgb().ToString();
                            }
                            else if (invalid_details[i] == 2)
                            {
                                item.display_content_color[dayIndex] = "0xff008000";//Color.Green.ToArgb().ToString();
                            }
                            else if (invalid_details[i] == 3)
                            {
                                item.display_content_color[dayIndex] = "0xffff0000";//Color.Red.ToArgb().ToString(); 
                            }
                        }
                        else
                        {                            
                            item.classOrderCount[dayIndex]++;                            
                        }
                    }

                    for (int i = 0; i < item.display_content.Count(); i++)
                    {
                        bfirst = true;
                        item.display_content[i] = "";
                        
                        if (((int)showElementType & (int)ShowElementType.ClassOrder) != 0X00)
                        {
                            item.display_content[i] = item.attend_signs[i];
                            bfirst = false;
                        }

                        if (((int)showElementType & (int)ShowElementType.Duration) != 0X00)
                        {
                            if (!bfirst)
                            {
                                item.display_content[i] += "\r\n";
                            }

                            item.display_content[i] += item.work_times[i];
                            bfirst = false;
                        }

                        if (((int)showElementType & (int)ShowElementType.Time) != 0X00)
                        {
                            if (!bfirst)
                            {
                                item.display_content[i] += "\r\n";
                            }
                            item.display_content[i] += item.time_descriptions[i];
                        }
                    }

                    #endregion

                    if ( bAdd )
                    {
                        query.Add(item);                        
                    }

                    #region 部门小计
                    
                    if ((ReportType)_reportType == ReportType.DetailReportOnDepart)
                    {
                        int nextDepartID = -1;
                        if (rowIndex + 1 < dt.Rows.Count)
                        {
                            if (dt.Rows[rowIndex + 1]["depart_id"] != DBNull.Value)
                            {
                                nextDepartID = Convert.ToInt32(dt.Rows[rowIndex + 1]["depart_id"]);
                            }
                        }
                        //如果 换部门了 则添加一行部门小计
                        if (nextDepartID != item.depart_id)
                        {
                            PersonAttendStatistics departSumItem = new PersonAttendStatistics();
                            for (int i = lastDepartStartIndex; i < query.Count; i++)
                            {
                                departSumItem.sum_0_2 += query[i].sum_0_2;
                                departSumItem.sum_12 += query[i].sum_12;
                                departSumItem.sum_2_4 += query[i].sum_2_4;
                                departSumItem.sum_4_8 += query[i].sum_4_8;
                                departSumItem.sum_8_12 += query[i].sum_8_12;
                                departSumItem.total_times += query[i].total_times;
                                departSumItem.total_work_time += query[i].total_work_time;
                                departSumItem.avg_work_time += query[i].avg_work_time;
                                departSumItem.valid_times += query[i].valid_times;
                                departSumItem.invalid_times += query[i].invalid_times;
                                departSumItem.less_avail_times += query[i].less_avail_times;
                                departSumItem.wan_times += query[i].wan_times;
                                departSumItem.zao_times += query[i].zao_times;
                                departSumItem.zhong_times += query[i].zhong_times;

                                for (int classOrderIndex = 0; classOrderIndex < query[i].classOrderCount.Length; classOrderIndex++)
                                {
                                    departSumItem.classOrderCount[classOrderIndex] += query[i].classOrderCount[classOrderIndex];
                                }
                            }

                            for (int classOrderIndex = 0; classOrderIndex < departSumItem.classOrderCount.Length; classOrderIndex++)
                            {
                                departSumItem.display_content[classOrderIndex] = departSumItem.classOrderCount[classOrderIndex].ToString();
                            }

                            if ((departSumItem.valid_times + departSumItem.less_avail_times) == 0)
                            {
                                departSumItem.avg_work_time = 0;
                            }
                            else
                            {
                                departSumItem.avg_work_time = departSumItem.total_work_time / (departSumItem.valid_times + departSumItem.less_avail_times);
                            }

                            departSumItem.work_sn = "小计";
                            departSumItem.depart_name = item.depart_name;

                            //key特征属性赋值
                            departSumItem.month = item.month;
                            departSumItem.person_id = -query.Count;

                            query.Add(departSumItem);
                            lastDepartStartIndex = query.Count;
                        }
                    }

                    #endregion
                }
            }
            PersonAttendStatistics sumItem = new PersonAttendStatistics();
            //合计
            if (query.Count > 0)
            {
                foreach (var item in query)
                {
                    if (item.work_sn.CompareTo("小计") != 0)
                    {
                        sumItem.sum_0_2 += item.sum_0_2;
                        sumItem.sum_12 += item.sum_12;
                        sumItem.sum_2_4 += item.sum_2_4;
                        sumItem.sum_4_8 += item.sum_4_8;
                        sumItem.sum_8_12 += item.sum_8_12;
                        sumItem.total_times += item.total_times;
                        sumItem.total_work_time += item.total_work_time;
                        sumItem.avg_work_time += item.avg_work_time;
                        sumItem.valid_times += item.valid_times;
                        sumItem.invalid_times += item.invalid_times;
                        sumItem.less_avail_times += item.less_avail_times;
                        sumItem.wan_times += item.wan_times;
                        sumItem.zao_times += item.zao_times;
                        sumItem.zhong_times += item.zhong_times;

                        for (int classOrderIndex = 0; classOrderIndex < item.classOrderCount.Length; classOrderIndex++)
                        {
                            sumItem.classOrderCount[classOrderIndex] += item.classOrderCount[classOrderIndex];
                        }
                    }
                }

                for (int classOrderIndex = 0; classOrderIndex < sumItem.classOrderCount.Length; classOrderIndex++)
                {
                    sumItem.display_content[classOrderIndex] = sumItem.classOrderCount[classOrderIndex].ToString();
                }

                if ((sumItem.valid_times + sumItem.less_avail_times) == 0)
                {
                    sumItem.avg_work_time = 0;
                }
                else
                {
                    sumItem.avg_work_time = sumItem.total_work_time / (sumItem.valid_times + sumItem.less_avail_times);
                }
                
                sumItem.work_sn  = "合计";

                //key特征属性赋值
                sumItem.person_id = -(query.Count + 1);

                query.Add(sumItem);
            }
            

            ReportType reportType = (ReportType)_reportType;
            switch (reportType)
            {
                case ReportType.MonthlyReportOnPerson:
                    PersonAttendStatisticsList = query;
                    sumItem.month = "合计";
                    break;
                case ReportType.DetailReportOnDepart:
                    sumItem.work_sn = "合计";
                    DepartDetailList = query;
                    break;
            }

            ConditionShowElementType = _showElementType;

            return query;
        }
        
        #endregion
        

    }
}