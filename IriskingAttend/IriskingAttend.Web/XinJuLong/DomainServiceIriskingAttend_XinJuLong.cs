/*************************************************************************
** 文件名:   DomainServiceIriskingAttend_XinJuLong.cs
** 主要类:   DomainServiceIriskingAttend_XinJuLong.cs
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2014-3-18
** 修改人:   
** 日  期:
** 描  述:   用于新巨龙查询的域服务
**
** 版  本:   1.0.0
** 备  注:  命名及代码编写遵守C#编码规范
**
 * ***********************************************************************/
using System.Web;
using System.Linq;

namespace IriskingAttend.Web
{
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
    using System.Runtime.Serialization;
    using NpgsqlTypes;

    public partial class DomainServiceIriskingAttend
    {
        [Query(HasSideEffects = true)]
        public IEnumerable<AttendDepartInWellDetail> GetAttendDepartInWellDetail(DateTime beginTime, DateTime endTime, int departId, string attendSign)
        {
            //组装sql语句的where条件          
            string sql_where = string.Format(" and depart_id = {0} and attend_sign like '{1}'", departId, attendSign);

            string sql_all = string.Format(@"SELECT ar.person_id, name, work_sn, depart_id, depart_name,in_well_time,out_well_time,work_time,
	                            inRecog.device_sn as in_device_sn, outRecog.device_sn as out_device_sn,
		                    co.class_order_name, co.attend_sign                                                                           
		                    FROM attend_record_all ar LEFT JOIN class_order_normal co ON ar.class_order_id=co.class_order_id 
		                    left join person_recog_base inRecog on ar.in_id = inRecog.person_recog_log_id
		                    left join person_recog_base outRecog on ar.out_id = outRecog.person_recog_log_id
		                    WHERE  leave_type_id < 50 and attend_day between '{0}' and '{1}' {2}  
			          UNION ALL
	                  select inOut.person_id, name, work_sn, depart_id, depart_name,in_time as in_well_time,NULL::timestamp without time zone AS out_well_time,0 as work_time,
	                            inRecog.device_sn as in_device_sn,NULL::character varying AS out_device_sn,  
		                    co.class_order_name, co.attend_sign   
		                    from in_out_well inOut LEFT JOIN class_order_normal co ON inOut.class_order_id=co.class_order_id 
		                    left join person_recog_base inRecog on inOut.in_out_id = inRecog.person_recog_log_id 
		                    where attend_day between '{0}' and '{1}' {2} ",
                beginTime, endTime, sql_where);

            List<AttendDepartInWellDetail> query = new List<AttendDepartInWellDetail>();
            DataTable dt = DbAccess.POSTGRESQL.Select(sql_all, "attend_record_base");
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }

            int count = 1;
            for (int index = 0; index < dt.Rows.Count; index++)
            {
                AttendDepartInWellDetail departInWell = new AttendDepartInWellDetail();

                //如果可为空需要判断                
                departInWell.DepartName = dt.Rows[index]["depart_name"].ToString().Trim();
                departInWell.Index = count++;
                departInWell.Name = dt.Rows[index]["name"].ToString().Trim();
                departInWell.WorkSn = dt.Rows[index]["work_sn"].ToString().Trim();
                int workTime = Int32.Parse(dt.Rows[index]["work_time"].ToString());
                departInWell.WorkTime = string.Format("{0}:{1}", workTime / 60, (workTime % 60).ToString("d2")); 
                departInWell.InDeviceSn = dt.Rows[index]["in_device_sn"].ToString().Trim();
                departInWell.OutDeviceSn = dt.Rows[index]["out_device_sn"].ToString().Trim();
                if (dt.Rows[index]["in_well_time"] != null && dt.Rows[index]["in_well_time"].ToString() != "")
                {
                    departInWell.InWellTime = DateTime.Parse( dt.Rows[index]["in_well_time"].ToString() );
                }

                if (dt.Rows[index]["out_well_time"] != null && dt.Rows[index]["out_well_time"].ToString() != "")
                {
                    departInWell.OutWellTime = DateTime.Parse(dt.Rows[index]["out_well_time"].ToString());
                }              

                query.Add(departInWell);
            }

            return query;
        }

        [Query(HasSideEffects = true)]
        public IEnumerable<AttendDepartInWellQuery> GetAttendDepartInWellCollect( DateTime beginTime, DateTime endTime, int[] departIds)
        {
            //组装sql语句的where条件          
            string sql_where = "";
            if (departIds != null && departIds.Count() > 0)
            {
                sql_where = string.Format(" and depart_id in ( ");
                for (int index = 0; index < departIds.Count(); index++)
                {
                    sql_where += string.Format(" {0}, ", departIds[index]);
                }

                sql_where = sql_where.Remove(sql_where.LastIndexOf(","), 1);
                sql_where += ")";
            }

            string sql_all = string.Format(@"SELECT class_order_id, depart_id, max(depart_name) depart_name , max(class_order_name) class_order_name, 
                max(attend_sign) attend_sign, count(class_order_id) total_times                
	            FROM( SELECT ar.class_order_id, ar.person_id, depart_id, depart_name, co.class_order_name, co.attend_sign                                                                           
		                    FROM attend_record_all ar LEFT JOIN class_order_normal co ON ar.class_order_id=co.class_order_id 		                    
		                    WHERE  leave_type_id < 50 and attend_day between '{0}' and '{1}' {2}  
			          UNION ALL
	                  select inOut.class_order_id, inOut.person_id, depart_id, depart_name, co.class_order_name, co.attend_sign   
		                    from in_out_well inOut LEFT JOIN class_order_normal co ON inOut.class_order_id=co.class_order_id 		                    
		                    where attend_day between '{0}' and '{1}' {2} ) as person_count 
                where class_order_id is not null group by class_order_id,depart_id order by depart_name,class_order_id",
                beginTime, endTime, sql_where);

            List<AttendDepartInWellQuery> query = new List<AttendDepartInWellQuery>();
            DataTable dt = DbAccess.POSTGRESQL.Select(sql_all, "attend_record_base");
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }

            int  count = 1;
            for( int index = 0; index < dt.Rows.Count; )
            {
                AttendDepartInWellQuery departInWell = new AttendDepartInWellQuery();

                //如果可为空需要判断
                departInWell.DepartId = Convert.ToInt32(dt.Rows[index]["depart_id"]);
                departInWell.DepartName = dt.Rows[index]["depart_name"].ToString().Trim();
                departInWell.Index = count++.ToString();

                do
                {
                    string attendSign = dt.Rows[index]["attend_sign"].ToString().Trim();                                   

                    if (attendSign.Contains("早"))
                    {
                        departInWell.MoringInWellCollect = Convert.ToInt32(dt.Rows[index]["total_times"]);
                    }
                    else if (attendSign.Contains("中"))
                    {
                        departInWell.MiddleInWellCollect = Convert.ToInt32(dt.Rows[index]["total_times"]);
                    }
                    else if (attendSign.Contains("夜"))
                    {
                        departInWell.NightInWellCollect = Convert.ToInt32(dt.Rows[index]["total_times"]);
                    }
                    else if (attendSign.Contains("一"))
                    {
                        departInWell.OneInWellCollect = Convert.ToInt32(dt.Rows[index]["total_times"]);
                    }
                    else if (attendSign.Contains("二"))
                    {
                        departInWell.TwoInWellCollect = Convert.ToInt32(dt.Rows[index]["total_times"]);
                    }
                    else if (attendSign.Contains("三"))
                    {
                        departInWell.ThreeInWellCollect = Convert.ToInt32(dt.Rows[index]["total_times"]);
                    }
                    else if (attendSign.Contains("四"))
                    {
                        departInWell.FourInWellCollect = Convert.ToInt32(dt.Rows[index]["total_times"]);
                    }

                } while (++index < dt.Rows.Count && Convert.ToInt32(dt.Rows[index]["depart_id"]) == departInWell.DepartId);              
                
                query.Add(departInWell);
            }

            return query;
        }

        [Query(HasSideEffects = true)]
        public IEnumerable<AttendPersonInWellQuery> GetAttendPersonInWellCollect(DateTime beginTime, DateTime endTime, int[] departIds, string personName, string workSn, int workTime )
        {
            //组装sql语句的where条件          
            string sql_where = "";
            if (departIds != null && departIds.Count() > 0)
            {
                sql_where = string.Format(" and d.depart_id in ( ");
                for (int index = 0; index < departIds.Count(); index++)
                {
                    sql_where += string.Format(" {0}, ", departIds[index]);
                }

                sql_where = sql_where.Remove(sql_where.LastIndexOf(","), 1);
                sql_where += ")";
            }

            //人名过滤
            if (personName != null && personName != "")
            {
                sql_where = sql_where + String.Format("and p.name like '%{0}%'", personName);
            }

            //工号过滤
            if (workSn != null && workSn != "")
            {
                sql_where = sql_where + String.Format("and p.work_sn = '{0}'", workSn);
            }

            sql_where += String.Format(" and ar.work_time >= {0}", workTime);
            sql_where += " and in_well_time is not null and out_well_time is not null";

            string sql_all = string.Format(@"SELECT ar.person_id, max(p.name) person_name,  max(p.work_sn) work_sn,
                               max(d.depart_id) depart_id, max(d.depart_name) depart_name, count(ar.person_id) total_times                                                                         
                              FROM attend_record_base ar                   
			                    left join person_base p on  ar.person_id=p.person_id
			                    left join depart d on p.depart_id=d.depart_id
			                    WHERE  attend_day between '{0}' and '{1}' {2}	    
			                    GROUP BY ar.person_id
			                    ORDER BY depart_name, person_name",
                               beginTime.ToString("yyyy-MM-dd 00:00:00"), endTime.ToString("yyyy-MM-dd 23:59:59"), sql_where);

            List<AttendPersonInWellQuery> query = new List<AttendPersonInWellQuery>();
            DataTable dt = DbAccess.POSTGRESQL.Select(sql_all, "attend_record_base");
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }

            int  count = 1;
            for (int index = 0; index < dt.Rows.Count; index++)
            {
                AttendPersonInWellQuery personInWell = new AttendPersonInWellQuery();

                //如果可为空需要判断
                personInWell.PersonId = Convert.ToInt32(dt.Rows[index]["person_id"]);
                personInWell.DepartName = dt.Rows[index]["depart_name"].ToString().Trim();
                personInWell.PersonName = dt.Rows[index]["person_name"].ToString().Trim();
                personInWell.WorkSn = dt.Rows[index]["work_sn"].ToString().Trim();
                personInWell.InWellCollect = Convert.ToInt32(dt.Rows[index]["total_times"]);
                personInWell.Index = count++.ToString();

                query.Add(personInWell);

            }

            return query;
        }

        [Query(HasSideEffects = true)]
        public IEnumerable<AttendInComplete> GetInCompleteCollect(DateTime beginTime, DateTime endTime, int[] departIds, string personName, string workSn)
        {
            //组装sql语句的where条件          
            string sql_where = "";
            if (departIds != null && departIds.Count() > 0)
            {
                sql_where = string.Format(" and d.depart_id in ( ");
                for (int index = 0; index < departIds.Count(); index++)
                {
                    sql_where += string.Format(" {0}, ", departIds[index]);
                }

                sql_where = sql_where.Remove(sql_where.LastIndexOf(","), 1);
                sql_where += ")";
            }

            //人名过滤
            if (personName != null && personName != "")
            {
                sql_where = sql_where + String.Format("and p.name like '%{0}%'", personName);
            }

            //工号过滤
            if (workSn != null && workSn != "")
            {
                sql_where = sql_where + String.Format("and p.work_sn = '{0}'", workSn);
            }

            sql_where += " and ( ( in_well_time is null and out_well_time is not null ) or (in_well_time is not null and out_well_time is null))";

            string sql_all = string.Format(@"SELECT ar.person_id, max(p.name) person_name,  max(p.work_sn) work_sn,
                               max(d.depart_id) depart_id, max(d.depart_name) depart_name, count(ar.person_id) total_times                                                                         
                              FROM attend_record_base ar                   
			                    left join person_base p on  ar.person_id=p.person_id
			                    left join depart d on p.depart_id=d.depart_id
			                    WHERE  attend_day between '{0}' and '{1}' {2}	    
			                    GROUP BY ar.person_id
			                    ORDER BY depart_name, person_name",
                               beginTime.ToString("yyyy-MM-dd 00:00:00"), endTime.ToString("yyyy-MM-dd 23:59:59"), sql_where);

            List<AttendInComplete> query = new List<AttendInComplete>();
            DataTable dt = DbAccess.POSTGRESQL.Select(sql_all, "attend_record_base");
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }

            int count = 1;
            for (int index = 0; index < dt.Rows.Count; index++)
            {
                AttendInComplete personInWell = new AttendInComplete();

                //如果可为空需要判断
                personInWell.PersonId = Convert.ToInt32(dt.Rows[index]["person_id"]);
                personInWell.DepartName = dt.Rows[index]["depart_name"].ToString().Trim();
                personInWell.PersonName = dt.Rows[index]["person_name"].ToString().Trim();
                personInWell.WorkSn = dt.Rows[index]["work_sn"].ToString().Trim();
                personInWell.InCompleteCollect = Convert.ToInt32(dt.Rows[index]["total_times"]);
                personInWell.Index = count++.ToString();

                query.Add(personInWell);

            }

            return query;
        }

        [Query(HasSideEffects = true)]
        public IEnumerable<InCompleteRecord> GetInCompleteRecord(DateTime beginTime, DateTime endTime, int personId)
        {
            //组装sql语句的where条件          
            string sql_where = "";
            sql_where = String.Format(" and person_id = {0}", personId);
           

            sql_where += " and ( ( in_well_time is null and out_well_time is not null ) or (in_well_time is not null and out_well_time is null))";

            string sql_all = string.Format(@"SELECT attend_record_id, person_id, in_well_time, out_well_time
                              FROM attend_record_base WHERE  attend_day between '{0}' and '{1}' {2} ORDER BY attend_day",
                              beginTime.ToString("yyyy-MM-dd 00:00:00"), endTime.ToString("yyyy-MM-dd 23:59:59"), sql_where);

            List<InCompleteRecord> query = new List<InCompleteRecord>();
            DataTable dt = DbAccess.POSTGRESQL.Select(sql_all, "attend_record_base");
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }

            int count = 1;
            for (int index = 0; index < dt.Rows.Count; index++)
            {
                InCompleteRecord record = new InCompleteRecord();

                //如果可为空需要判断
                record.RecordId = Convert.ToInt32(dt.Rows[index]["attend_record_id"]);
                record.InWellTime = dt.Rows[index]["in_well_time"].ToString().Trim();
                record.OutWellTime = dt.Rows[index]["out_well_time"].ToString().Trim();                
                record.Index = count++.ToString();

                query.Add(record);

            }

            return query;
        }

        public static List<ReportPersonMonth> ReportPersonMonthAttendList = new List<ReportPersonMonth>();

        [Query(HasSideEffects = true)]
        public IEnumerable<ReportPersonMonth> GetReportPersonMonthAttend(DateTime beginTime, int[] departIds, string personName, string workSn)
        {
            ReportPersonMonthAttendList = new List<ReportPersonMonth>();

            //组装sql语句的where条件          
            string sql_where = "";
            if (departIds != null && departIds.Count() > 0)
            {
                sql_where = string.Format(" d.depart_id in ( ");
                for (int index = 0; index < departIds.Count(); index++)
                {
                    sql_where += string.Format(" {0}, ", departIds[index]);
                }

                sql_where = sql_where.Remove(sql_where.LastIndexOf(","), 1);
                sql_where += ")";
            }

            //人名过滤
            if (personName != null && personName != "")
            {
                if (sql_where != "")
                {
                    sql_where = sql_where + " and " ;
                }
                sql_where = sql_where + String.Format(" p.name like '%{0}%'", personName);
            }

            //工号过滤
            if (workSn != null && workSn != "")
            {
                if (sql_where != "")
                {
                    sql_where = sql_where + " and ";
                }
                sql_where = sql_where + String.Format(" p.work_sn = '{0}'", workSn);
            }

            if (sql_where != "")
            {
                sql_where = " where " + sql_where;
            }

            beginTime = Convert.ToDateTime( beginTime.ToString("yyyy-MM-01 00:00:00") );

            string sql = string.Format(
             @"SELECT d.depart_name,d.depart_id, p.name, p.work_sn, p.person_id,Result.attend_days,Result.attend_signs, Result.work_times from person p left join 
	                (  SELECT  max(person_id) person_id, 
                                    array_agg(attend_day) as attend_days,      --------考勤日期分组
                                    array_agg(attend_sign) as attend_signs,    --------班次简称分组
                                    array_agg(work_time) as work_times         --------工作时间分组 
			                FROM (
				                -- 先将基本的数据计算出来
				                SELECT person_id, to_date(to_char(attend_day,'yyyy-MM-dd'),'yyyy-MM-dd') as attend_day, work_time, 
				                    CASE WHEN (in_well_time IS NULL OR out_well_time IS NULL) THEN '未' ELSE co.attend_sign END attend_sign				                    
				                FROM attend_record_base ar LEFT JOIN class_order_base co ON ar.class_order_id=co.class_order_id WHERE  attend_day between '{0}' and '{1}'
			                  ) Attend  GROUP BY person_id
	                ) Result on  Result.person_id=p.person_id
	                left join depart d on p.depart_id=d.depart_id 
	                {2} order by depart_name, name ", beginTime.ToString("yyyy-MM-dd 00:00:00"), beginTime.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd 23:59:59"), sql_where);
            

            List<ReportPersonMonth> query = new List<ReportPersonMonth>();
            DataTable dt = DbAccess.POSTGRESQL.Select(sql, "attend_record_base");
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }

            int count = 1;
            foreach ( DataRow ar in dt.Rows)
            {
                ReportPersonMonth item = new ReportPersonMonth();
                DateTime[] attend_days = new DateTime[0];
                string[] attend_signs = null;                
                int[] work_times = null;
                item.Index = count++.ToString();

                #region     数据填充
                if (ar["depart_name"] != DBNull.Value)
                {
                    item.depart_name = Convert.ToString(ar["depart_name"]);
                }
                if (ar["depart_id"] != DBNull.Value)
                {
                    item.depart_id = Convert.ToInt32(ar["depart_id"]);
                }
                if (ar["name"] != DBNull.Value)
                {
                    item.name = (string)(ar["name"]);
                }
                if (ar["person_id"] != DBNull.Value)
                {
                    item.person_id = Convert.ToInt32(ar["person_id"]);
                }
                if (ar["work_sn"] != DBNull.Value)
                {
                    item.work_sn = (string)(ar["work_sn"]);
                }

                if (ar["attend_days"] != DBNull.Value)
                {
                    NpgsqlDate[] temp = (NpgsqlDate[])(ar["attend_days"]);
                    attend_days = new DateTime[temp.Length];
                    for (int index = 0; index < temp.Length; index++)
                    {
                        attend_days[index] = (DateTime)temp[index];
                    }
                }
                if (ar["attend_signs"] != DBNull.Value)
                {
                    attend_signs = (string[])(ar["attend_signs"]);
                }
                if (ar["work_times"] != DBNull.Value)
                {
                    work_times = (int[])(ar["work_times"]);
                }
                #endregion

                #region 构造每日内容
                
                item.daily_content_description = "班次\r\n时长";

                //构造每日内容
                for (int i = 0; i < attend_days.Length; i++)
                {
                    int dayIndex = attend_days[i].Day - 1;

                    if (attend_signs[i].CompareTo("未") == 0)
                    {
                        if (item.attend_signs[dayIndex].Length <= 0)
                        {
                            item.attend_signs[dayIndex] = attend_signs[i];
                        }
                    }
                    else
                    {
                        if (item.attend_signs[dayIndex].Length <= 0 ||
                            item.attend_signs[dayIndex].CompareTo("未") == 0 ||
                            Convert.ToInt32(item.work_times[dayIndex]) < work_times[i])
                        {
                            item.attend_signs[dayIndex] = attend_signs[i];
                            item.work_times[dayIndex] = work_times[i].ToString();
                        }
                    }                   
                }

                for (int i = 0; i < item.display_content.Length; i++)
                {
                    if (item.attend_signs[i].Length > 0)
                    {
                        if (item.attend_signs[i] == "未")
                        {
                            item.invalid_times++;
                        }
                        else
                        {
                            item.valid_times++;
                            if (item.attend_signs[i] == "早")
                            {
                                item.zao_times++;
                            }
                            else if (item.attend_signs[i] == "中")
                            {
                                item.zhong_times++;
                            }
                            else if (item.attend_signs[i] == "夜")
                            {
                                item.wan_times++;
                            }
                            else if (item.attend_signs[i] == "一")
                            {
                                item.one_times++;
                            }
                            else if (item.attend_signs[i] == "二")
                            {
                                item.two_times++;
                            }
                            else if (item.attend_signs[i] == "三")
                            {
                                item.three_times++;
                            }
                            else if (item.attend_signs[i] == "四")
                            {
                                item.four_times++;
                            }
                        }
                    }      
                   
                    if (item.work_times[i].Length > 0)
                    {
                        int workTime = Convert.ToInt32(item.work_times[i]);
                        item.work_times[i] = string.Format("{0}:{1:d2}", workTime/60, workTime%60);

                        if (workTime < 3 * 60)
                        {
                            item.sum_0_3++;
                        }
                        else if (workTime < 4 * 60)
                        {
                            item.sum_3_4++;
                        }
                        else if (workTime < 6 * 60)
                        {
                            item.sum_4_6++;
                        }
                        else if (workTime < 8 * 60)
                        {
                            item.sum_6_8++;
                        }
                        else 
                        {
                            item.sum_8++;
                        }
                    }
                    item.display_content[i] = item.attend_signs[i] + "\r\n" + item.work_times[i];
                }

                #endregion

                query.Add(item);

            }

            ReportPersonMonthAttendList = query;

            return query;
        }
    }

    #region 新巨龙当前单位当前班次下井人数明细

    /// <summary>
    /// 新巨龙当前单位当前班次下井人数明细实体数据类
    /// </summary>
    [DataContract]
    public class AttendDepartInWellDetail
    {
        /// <summary>
        /// 序号
        /// </summary>
        [DataMember]
        [Key]
        public int Index { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        [DataMember]
        public string DepartName { get; set; }

        /// <summary>
        /// 人员姓名
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// 人员工号
        /// </summary>
        [DataMember]
        public string WorkSn { get; set; }

        /// <summary>
        /// 人员下井时间
        /// </summary>
        [DataMember]
        public DateTime InWellTime { get; set; }

        /// <summary>
        /// 人员下井地点
        /// </summary>
        [DataMember]
        public string InDeviceSn { get; set; }

        /// <summary>
        /// 人员出井时间
        /// </summary>
        [DataMember]
        public DateTime OutWellTime { get; set; }

        /// <summary>
        /// 人员出井地点集合
        /// </summary>
        [DataMember]
        public string OutDeviceSn { get; set; }

        /// <summary>
        /// 人员下井时长
        /// </summary>
        [DataMember]
        public string WorkTime { get; set; }

        public AttendDepartInWellDetail()
        {
            WorkSn = "";
            InDeviceSn = "";
            OutDeviceSn = "";
            WorkTime = "";
        }

    }

    #endregion

    #region 新巨龙当前各单位下井人数统计实体数据类

    /// <summary>
    /// 新巨龙当前各单位下井人数实体数据类
    /// </summary>
    [DataContract]
    public class AttendDepartInWellQuery
    {
        /// <summary>
        /// 部门Id
        /// </summary>
        [DataMember]
        [Key]
        public int DepartId { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        [DataMember]       
        public string Index { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        [DataMember]        
        public string DepartName { get; set; }       

        /// <summary>
        /// 早班在井人数
        /// </summary>
        [DataMember]
        public int MoringInWellCollect { get; set; }

        /// <summary>
        /// 中班在井人数
        /// </summary>
        [DataMember]
        public int MiddleInWellCollect { get; set; }

        /// <summary>
        /// 晚班在井人数
        /// </summary>
        [DataMember]
        public int NightInWellCollect { get; set; }

        /// <summary>
        /// 一班在井人数
        /// </summary>
        [DataMember]
        public int OneInWellCollect { get; set; }

        /// <summary>
        /// 二班在井人数
        /// </summary>
        [DataMember]
        public int TwoInWellCollect { get; set; }

        /// <summary>
        /// 三班在井人数
        /// </summary>
        [DataMember]
        public int ThreeInWellCollect { get; set; }

        /// <summary>
        /// 四班在井人数
        /// </summary>
        [DataMember]
        public int FourInWellCollect { get; set; }

        public AttendDepartInWellQuery()
        {
            
        }

    }   
    
    #endregion

    #region 新巨龙个人下井统计实体数据类

    /// <summary>
    /// 新巨龙个人下井统计实体数据类
    /// </summary>
    [DataContract]
    public class AttendPersonInWellQuery
    {
        /// <summary>
        /// 人员Id
        /// </summary>
        [DataMember]
        [Key]
        public int PersonId { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        [DataMember]
        public string Index { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        [DataMember]
        public string DepartName { get; set; }

        /// <summary>
        /// 人员姓名
        /// </summary>
        [DataMember]
        public string PersonName { get; set; }

        /// <summary>
        /// 人员工号
        /// </summary>
        [DataMember]
        public string WorkSn { get; set; }

        /// <summary>
        /// 下井总数
        /// </summary>
        [DataMember]
        public int InWellCollect { get; set; }

    }

    #endregion

    #region 新巨龙不完整考勤统计实体数据类

    /// <summary>
    /// 新巨龙不完整考勤统计实体数据类
    /// </summary>
    [DataContract]
    public class AttendInComplete
    {
        /// <summary>
        /// 人员Id
        /// </summary>
        [DataMember]
        [Key]
        public int PersonId { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        [DataMember]
        public string Index { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        [DataMember]
        public string DepartName { get; set; }

        /// <summary>
        /// 人员姓名
        /// </summary>
        [DataMember]
        public string PersonName { get; set; }

        /// <summary>
        /// 人员工号
        /// </summary>
        [DataMember]
        public string WorkSn { get; set; }

        /// <summary>
        /// 不完整考勤总数
        /// </summary>
        [DataMember]
        public int InCompleteCollect { get; set; }

    }

    /// <summary>
    /// 新巨龙不完整考勤记录实体数据类
    /// </summary>
    [DataContract]
    public class InCompleteRecord
    {
        /// <summary>
        /// 记录Id
        /// </summary>
        [DataMember]
        [Key]
        public int RecordId { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        [DataMember]
        public string Index { get; set; }        

        /// <summary>
        /// 入井时间
        /// </summary>
        [DataMember]
        public string InWellTime { get; set; }

        /// <summary>
        /// 出井时间
        /// </summary>
        [DataMember]
        public string OutWellTime { get; set; }

    }
    #endregion

    /// <summary>
    /// 新巨龙矿个人考勤月报表实体数据类
    /// </summary>
    [DataContract]
    public class ReportPersonMonth
    {
        /// <summary>
        /// 序号
        /// </summary>
        [DataMember]
        public string Index { get; set; }

        /// <summary>
        /// 部门
        /// </summary>
        [DataMember]
        public string depart_name { get; set; }

        /// <summary>
        /// 部门id
        /// </summary>
        [DataMember]
        [Key]
        public int depart_id { get; set; }

        /// <summary>
        /// id
        /// </summary>        
        [DataMember]
        [Key]
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
        /// 有效下井次数
        /// </summary>
        [DataMember]
        public int valid_times { get; set; }

        /// <summary>
        /// 无效次数
        /// </summary>
        [DataMember]
        public int invalid_times { get; set; }

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
        /// 一班次数
        /// </summary>
        [DataMember]
        public int one_times { get; set; }

        /// <summary>
        /// 二班次数
        /// </summary>
        [DataMember]
        public int two_times { get; set; }

        /// <summary>
        /// 三班次数
        /// </summary>
        [DataMember]
        public int three_times { get; set; }

        /// <summary>
        /// 四班次数
        /// </summary>
        [DataMember]
        public int four_times { get; set; }


        /// <summary>
        /// 0-3次数
        /// </summary>
        [DataMember]
        public int sum_0_3 { get; set; }

        /// <summary>
        /// 3-4次数
        /// </summary>
        [DataMember]
        public int sum_3_4 { get; set; }

        /// <summary>
        /// 4-6次数
        /// </summary>
        [DataMember]
        public int sum_4_6 { get; set; }

        /// <summary>
        /// 6-8次数
        /// </summary>
        [DataMember]
        public int sum_6_8 { get; set; }

        /// <summary>
        /// >=8次数
        /// </summary>
        [DataMember]
        public int sum_8 { get; set; }

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
        /// 时间描述
        /// </summary>
        [DataMember]
        public string[] work_times { get; set; }

        /// <summary>
        /// 每日显示内容
        /// </summary>
        [DataMember]
        public string[] display_content { get; set; }

        public ReportPersonMonth()
        {
            daily_content_description = "";

            display_content = new string[31];
            work_times = new string[31];
            attend_signs = new string[31];

            for (int i = 0; i < 31; i++)
            {
                display_content[i] = "";
                work_times[i] = "";
                attend_signs[i] = "";
            }
        }


    }

}