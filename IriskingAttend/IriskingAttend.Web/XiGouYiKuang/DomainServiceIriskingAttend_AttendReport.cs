/*************************************************************************
** 文件名:   DomainServiceIriskingAttend_AttendReport.cs
×× 主要类:   DomainServiceIriskingAttend_AttendReport
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2014-7-2
** 修改人:  
** 日  期:       
** 描  述:   西沟一矿
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
using System.IO;
using System.Runtime.Serialization;
using NpgsqlTypes;

namespace IriskingAttend.Web
{
    // TODO: 创建包含应用程序逻辑的方法。
    public partial class DomainServiceIriskingAttend : DomainService
    {

        public static List<XiGouDayAttendReport> XiGouDayAttendReportList = new List<XiGouDayAttendReport>();
        /// <summary>
        /// 获取日考勤统计表
        /// </summary>
        /// <returns>获取的机关考勤统计表记录列表</returns>
        [Query(HasSideEffects = true)]
        public IEnumerable<XiGouDayAttendReport> GetXiGouDayAttendCollect(DateTime beginTime, string[] departNames, string[] classTypeNames, string personName, string workSn)
        {
            string sqlWhere = "";

            //部门过滤
            if (departNames != null && departNames.Length > 0)
            {
                sqlWhere += @" and p.depart_name in (";
                foreach (var item in departNames)
                {
                    sqlWhere += string.Format(@"'{0}',", item);
                }
                sqlWhere = sqlWhere.Remove(sqlWhere.LastIndexOf(","), 1);
                sqlWhere += ")";
            }
            //班制过滤
            if (classTypeNames != null && classTypeNames.Length > 0)
            {
                sqlWhere += @" and p.class_type_name in (";
                foreach (var item in classTypeNames)
                {
                    sqlWhere += string.Format(@"'{0}',", item);
                }
                sqlWhere = sqlWhere.Remove(sqlWhere.LastIndexOf(","), 1);
                sqlWhere += ")";
            }
            //人名过滤
            if (personName != null && personName != "")
            {
                sqlWhere += String.Format(" and p.name like '%{0}%'", personName);
            }

            //工号过滤
            if (workSn != null && workSn != "")
            {
                sqlWhere += String.Format(" and p.work_sn = '{0}'", workSn);
            }                    

            string querySQL = string.Format(
             @"select p.person_id, p.work_sn, p.name, p.depart_name,  pr.principal_name, wt.work_type_name,  
attend.work_times, class_order_names,in_well_times,out_well_times,festival,weekends,normal,
leave.shijias, bingjias, wuxinjias, hunjias,chanjias, sangjias,nianxius,youxinjias from person p
left join ( select person_id, sum(work_time) work_times, array_agg(class_order_name) as class_order_names,array_agg(in_well_time) as in_well_times,
		array_agg(out_well_time) as out_well_times,
		case when max(weekend) = 2 then sum(work_cnt) else 0 end festival,
		case when max(weekend) = 1 then sum(work_cnt) else 0 end weekends,
		case when max(weekend) = 0 then sum(work_cnt) else 0 end normal 
		from ( select person_id, work_time, in_well_time, out_well_time,weekend,class_order_name,attend_day,		                    
		       case when class_order_name like '%上午%' or class_order_name like '%下午%' then workcnt/2 else workcnt end work_cnt 
			   from( select ar.attend_day,ar.person_id, ar.class_order_name, ar.work_time,
				 to_char(ar.in_well_time,'yyyy-MM-dd HH24:MI:ss') in_well_time, to_char(ar.out_well_time,'yyyy-MM-dd HH24:MI:ss') out_well_time, 
				 case when (SELECT count(*) FROM festival f WHERE f.begin_time <= ar.attend_day AND ar.attend_day <= f.end_time) >0 then 2 
				      when (EXTRACT(DOW from attend_day::timestamp )>0 and EXTRACT(DOW from attend_day::timestamp )<6) then 0 else 1 end weekend,   
				 case when ar.work_cnt = 10 then 1 when classStan.avail_time is null then 0  when classStan.avail_time = 0 then 1  
				      when ar.work_time >=classStan.avail_time then 1 else ar.work_time*1.0/classStan.avail_time end workcnt 
				 from attend_record_all ar left join ""class_order.standard"" classStan on ar.class_order_id = classStan.class_order_id 
				 where ar.leave_type_id < 50 and to_char(ar.attend_day,'yyyy-MM-dd') = '{0}' 
				)att
		    ) atte group by person_id,to_char(attend_day,'yyyy-MM-dd')
	  )attend on attend.person_id = p.person_id
left join ( select person_id, sum(shijia) shijias, sum(bingjia) bingjias, sum(wuxinjia) wuxinjias, sum(hunjia) hunjias, 
              sum(chanjia) chanjias, sum(sangjia) sangjias, sum(nianxiu) nianxius,sum(youxinjia) youxinjias 
              from( select person_id, 
		      case when leave_type_name like '事假' then (case when is_leave_all_day = 0 then leavetime else (date_part('day', (leaveEnd-leaveStart)) +1) end ) else 0 end shijia,
		      case when leave_type_name like '病假' then (case when is_leave_all_day = 0 then leavetime else (date_part('day', (leaveEnd-leaveStart)) +1) end ) else 0 end bingjia,
		      case when leave_type_name like '其它无薪假' then (case when is_leave_all_day = 0 then leavetime else (date_part('day', (leaveEnd-leaveStart)) +1) end ) else 0 end wuxinjia,
		      case when leave_type_name like '婚假' then (case when is_leave_all_day = 0 then leavetime else (date_part('day', (leaveEnd-leaveStart)) +1) end ) else 0 end hunjia,
		      case when leave_type_name like '产假' then (case when is_leave_all_day = 0 then leavetime else (date_part('day', (leaveEnd-leaveStart)) +1) end ) else 0 end chanjia,
		      case when leave_type_name like '丧假' then (case when is_leave_all_day = 0 then leavetime else (date_part('day', (leaveEnd-leaveStart)) +1) end ) else 0 end sangjia,
		      case when leave_type_name like '调休/年休' then (case when is_leave_all_day = 0 then leavetime else (date_part('day', (leaveEnd-leaveStart)) +1) end ) else 0 end nianxiu,
		      case when leave_type_name like '其它有薪假' then (case when is_leave_all_day = 0 then leavetime else (date_part('day', (leaveEnd-leaveStart)) +1) end ) else 0 end youxinjia
	           from( SELECT person_id, leave_type_name,leave_start_time,leave_end_time, is_leave_all_day, case when date_part('H', leave_end_time-leave_start_time) > 4 then 1 else 0.5 end  leavetime, 
				case when date_trunc('day',al.leave_start_time ) > date_trunc('day', date('{0}')) then al.leave_start_time else '{0}' end leaveStart,
				case when date_trunc('day',al.leave_end_time ) < date_trunc('day', date('{0}')) then al.leave_end_time else '{0}' end leaveEnd
			FROM attend_for_leave al left join leave_type lt on al.leave_type_id = lt.leave_type_id  
			WHERE date_trunc('day',al.leave_start_time ) <= '{0}' AND '{0}' <=  date_trunc('day',al.leave_end_time)
                   ) lea
              ) leav group by person_id
          ) leave on leave.person_id = p.person_id
left join principal pr on pr.principal_id = p.principal_id 
left join work_type wt on wt.work_type_id = p.work_type_id where p.delete_time is null {1} order by convert_to(depart_name,  E'GBK'),convert_to(name,  E'GBK')", 
            beginTime.ToString("yyyy-MM-dd"), sqlWhere);            

            XiGouDayAttendReportList = new List<XiGouDayAttendReport>(); 
            DataTable dt = DbAccess.POSTGRESQL.Select(querySQL, "attend_record_all");
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }

            try
            {
                int index = 1;
                foreach (DataRow ar in dt.Rows)
                {
                    XiGouDayAttendReport recordInfo = new XiGouDayAttendReport();                    
                    recordInfo.Index = index++;
                    recordInfo.PersonId = Int32.Parse(ar["person_id"].ToString());
                    recordInfo.PersonName = ar["name"].ToString();
                    recordInfo.WorkSn = ar["work_sn"].ToString();
                    recordInfo.DepartName = ar["depart_name"].ToString();                    
                    recordInfo.PrincipalName = ar["principal_name"].ToString();
                    recordInfo.WorkType = ar["work_type_name"].ToString();

                    string[] personClassOrderNames = null;
                    string[] inWellTimes = null;
                    string[] outWellTimes = null;
                    if (ar["class_order_names"] != null && ar["class_order_names"].ToString() != "")
                    {                       
                        personClassOrderNames = (string[])ar["class_order_names"];                       
                    }
                    if (ar["in_well_times"] != null && ar["in_well_times"].ToString() != "")
                    {                        
                        inWellTimes = (string[])ar["in_well_times"];                        
                    }
                    if (ar["out_well_times"] != null && ar["out_well_times"].ToString() != "")
                    {                       
                        outWellTimes = (string[])ar["out_well_times"];                        
                    }
                    if (personClassOrderNames != null && personClassOrderNames.Count() > 0)
                    {
                        if (personClassOrderNames[0].Contains("上午") || personClassOrderNames[0].Contains("下午"))
                        {                           
                            for (int classIndex = 0; classIndex < personClassOrderNames.Count(); classIndex++)
                            {
                                if (personClassOrderNames[classIndex].Contains("上午"))
                                {
                                    if (inWellTimes[classIndex] != null && inWellTimes[classIndex].CompareTo("NULL") != 0)
                                    {
                                        recordInfo.MorningInWellTime = inWellTimes[classIndex];
                                    }
                                    if (outWellTimes[classIndex] != null && outWellTimes[classIndex].CompareTo("NULL") != 0)
                                    {
                                        recordInfo.MorningOutWellTime = outWellTimes[classIndex];
                                    }
                                }
                                if (personClassOrderNames[classIndex].Contains("下午"))
                                {
                                    if (inWellTimes[classIndex] != null && inWellTimes[classIndex].CompareTo("NULL") != 0)
                                    {
                                        recordInfo.AfternoonInWellTime = inWellTimes[classIndex];
                                    }
                                    if (outWellTimes[classIndex] != null && outWellTimes[classIndex].CompareTo("NULL") != 0)
                                    {
                                        recordInfo.AfternoonOutWellTime = outWellTimes[classIndex];
                                    }
                                }
                            }
                        }
                        else
                        {                            
                            if (inWellTimes[0] != null && inWellTimes[0].CompareTo("NULL") != 0)
                            {
                                recordInfo.MorningInWellTime = inWellTimes[0];
                            }
                            if (outWellTimes[0] != null && outWellTimes[0].CompareTo("NULL") != 0)
                            {
                                recordInfo.AfternoonOutWellTime = outWellTimes[0];
                            }
                        }                        
                    }                  
                   
                    recordInfo.LeaveTypeName[0] = ar["shijias"].ToString();                   
                    recordInfo.LeaveTypeName[1] = ar["bingjias"].ToString();                    
                    recordInfo.LeaveTypeName[2] = ar["wuxinjias"].ToString();                    
                    recordInfo.LeaveTypeName[3] = ar["hunjias"].ToString();                    
                    recordInfo.LeaveTypeName[4] = ar["chanjias"].ToString();                   
                    recordInfo.LeaveTypeName[5] = ar["sangjias"].ToString();                  
                    recordInfo.LeaveTypeName[6] = ar["nianxius"].ToString();                   
                    recordInfo.LeaveTypeName[7] = ar["youxinjias"].ToString();                   

                    if (ar["normal"].ToString() != "")
                    {
                        recordInfo.WeekendType[0] = string.Format("{0:f2}", (float)Double.Parse(ar["normal"].ToString()));
                    }

                    if (ar["weekends"].ToString() != "")
                    {
                        recordInfo.WeekendType[1] = string.Format("{0:f2}", (float)Double.Parse(ar["weekends"].ToString()));
                    }

                    if (ar["festival"].ToString() != "")
                    {
                        recordInfo.WeekendType[2] = string.Format("{0:f2}", (float)Double.Parse(ar["festival"].ToString()));
                    }

                    if (ar["work_times"].ToString() != "")
                    {
                        int workTime = Int32.Parse(ar["work_times"].ToString());
                        recordInfo.WorkTime = string.Format("{0:f2}", (float)workTime / 60);
                    }

                    XiGouDayAttendReportList.Add(recordInfo);
                }

                return XiGouDayAttendReportList;
            }
            catch
            {
                return null;
            }
        }
        public static List<XiGouMonthAttendReport> XiGouMonthAttendReportList = new List<XiGouMonthAttendReport>();

        [Query(HasSideEffects = true)]
        public IEnumerable<XiGouMonthAttendReport> GetXiGouMonthAttendCollect(DateTime beginTime, DateTime endTime, string[] departNames, string[] classTypeNames, string personName, string workSn)
        {
            string sqlWhere = "";

            //部门过滤
            if (departNames != null && departNames.Length > 0)
            {
                sqlWhere += @" and p.depart_name in (";
                foreach (var item in departNames)
                {
                    sqlWhere += string.Format(@"'{0}',", item);
                }
                sqlWhere = sqlWhere.Remove(sqlWhere.LastIndexOf(","), 1);
                sqlWhere += ")";
            }

            //班制过滤
            if (classTypeNames != null && classTypeNames.Length > 0)
            {
                sqlWhere += @" and p.class_type_name in (";
                foreach (var item in classTypeNames)
                {
                    sqlWhere += string.Format(@"'{0}',", item);
                }
                sqlWhere = sqlWhere.Remove(sqlWhere.LastIndexOf(","), 1);
                sqlWhere += ")";
            }
         
            //人名过滤
            if (personName != null && personName != "")
            {
                sqlWhere += String.Format(" and p.name like '%{0}%'", personName);
            }

            //工号过滤
            if (workSn != null && workSn != "")
            {
                sqlWhere += String.Format(" and p.work_sn = '{0}'", workSn);
            }          

            string querySQL = string.Format(
             @"select p.person_id, p.work_sn, p.name, p.depart_name,  pr.principal_name, wt.work_type_name,  
                attend.work_times,festival,weekends,normal,
                leave.shijias, bingjias, wuxinjias, hunjias,chanjias, sangjias,nianxius,youxinjias,jias
                from person p
                left join ( select person_id,sum(work_times) work_times, sum(festival) festival, sum(weekends) weekends, sum(normal) normal
		                from( select person_id, sum(work_time) work_times, 
			                case when max(weekend) = 2 then sum(work_cnt) else 0 end festival,
			                case when max(weekend) = 1 then sum(work_cnt) else 0 end weekends,
			                case when max(weekend) = 0 then sum(work_cnt) else 0 end normal 
			                from ( select person_id, work_time, in_well_time, out_well_time,weekend,class_order_name,attend_day,		                    
			                       case when class_order_name like '%上午%' or class_order_name like '%下午%' then workcnt/2 else workcnt  end work_cnt 
				                   from( select ar.attend_day,ar.person_id, ar.class_order_name, ar.work_time,
					                 to_char(ar.in_well_time,'yyyy-MM-dd HH24:MI:ss') in_well_time, to_char(ar.out_well_time,'yyyy-MM-dd HH24:MI:ss') out_well_time, 
					                 case when (SELECT count(*) FROM festival f WHERE f.begin_time <= ar.attend_day AND ar.attend_day <= f.end_time) >0 then 2 
					                      when (EXTRACT(DOW from attend_day::timestamp )>0 and EXTRACT(DOW from attend_day::timestamp )<6) then 0 else 1 end weekend,   
					                 case when ar.work_cnt = 10 then 1 when classStan.avail_time is null then 0  when classStan.avail_time = 0 then 1  
					                      when ar.work_time >=classStan.avail_time then 1 else ar.work_time*1.0/classStan.avail_time end workcnt 
					                 from attend_record_all ar left join ""class_order.standard"" classStan on ar.class_order_id = classStan.class_order_id 
					                 where ar.leave_type_id < 50 and ar.attend_day between '{0}' and '{1}'
					                )att
			                    ) atte group by person_id,to_char(attend_day,'yyyy-MM-dd')
		                     ) atten group by person_id
	                  )attend on attend.person_id = p.person_id
                left join (  select person_id, sum(shijia) shijias, sum(bingjia) bingjias, sum(wuxinjia) wuxinjias, sum(hunjia) hunjias, 
		                sum(chanjia) chanjias, sum(sangjia) sangjias, sum(nianxiu) nianxius,sum(youxinjia) youxinjias, sum(jia) jias 
		                from( select person_id, 
		                      case when is_leave_all_day = 0 then leavetime else (date_part('day', (leaveEnd-leaveStart)) +1) end jia,
		                      case when leave_type_name like '事假' then (case when is_leave_all_day = 0 then leavetime else (date_part('day', (leaveEnd-leaveStart)) +1) end ) else 0 end shijia,
		                      case when leave_type_name like '病假' then (case when is_leave_all_day = 0 then leavetime else (date_part('day', (leaveEnd-leaveStart)) +1) end ) else 0 end bingjia,
		                      case when leave_type_name like '其它无薪假' then (case when is_leave_all_day = 0 then leavetime else (date_part('day', (leaveEnd-leaveStart)) +1) end ) else 0 end wuxinjia,
		                      case when leave_type_name like '婚假' then (case when is_leave_all_day = 0 then leavetime else (date_part('day', (leaveEnd-leaveStart)) +1) end ) else 0 end hunjia,
		                      case when leave_type_name like '产假' then (case when is_leave_all_day = 0 then leavetime else (date_part('day', (leaveEnd-leaveStart)) +1) end ) else 0 end chanjia,
		                      case when leave_type_name like '丧假' then (case when is_leave_all_day = 0 then leavetime else (date_part('day', (leaveEnd-leaveStart)) +1) end ) else 0 end sangjia,
		                      case when leave_type_name like '调休/年休' then (case when is_leave_all_day = 0 then leavetime else (date_part('day', (leaveEnd-leaveStart)) +1) end ) else 0 end nianxiu,
		                      case when leave_type_name like '其它有薪假' then (case when is_leave_all_day = 0 then leavetime else (date_part('day', (leaveEnd-leaveStart)) +1) end ) else 0 end youxinjia
		                      from( SELECT person_id, leave_type_name,leave_start_time,leave_end_time, is_leave_all_day, case when date_part('H', leave_end_time-leave_start_time) > 4 then 1 else 0.5 end  leavetime, 
				                case when date_trunc('day',al.leave_start_time ) > date_trunc('day', date('{0}')) then al.leave_start_time else '{0}' end leaveStart,
				                case when date_trunc('day',al.leave_end_time ) < date_trunc('day', date('{1}')) then al.leave_end_time else '{1}' end leaveEnd
			                    FROM attend_for_leave al left join leave_type lt on al.leave_type_id = lt.leave_type_id  
			                    WHERE date_trunc('day',al.leave_start_time ) <= '{1}' AND '{0}' <=  date_trunc('day',al.leave_end_time)
		                       ) lea   
		                    )leav group by person_id           
                          ) leave on leave.person_id = p.person_id
                left join principal pr on pr.principal_id = p.principal_id 
                left join work_type wt on wt.work_type_id = p.work_type_id where p.delete_time is null {2} order by convert_to(depart_name,  E'GBK'),convert_to(name,  E'GBK') ",
                     beginTime.ToString("yyyy-MM-dd 00:00:00"), endTime.ToString("yyyy-MM-dd 23:59:59"), sqlWhere);

            XiGouMonthAttendReportList = new List<XiGouMonthAttendReport>();
            DataTable dt = DbAccess.POSTGRESQL.Select(querySQL, "attend_record_all");
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }

            try
            {
                int index = 1;
                foreach (DataRow ar in dt.Rows)
                {
                    XiGouMonthAttendReport recordInfo = new XiGouMonthAttendReport();                 
                    recordInfo.Index = index++;
                    recordInfo.PersonId = Int32.Parse(ar["person_id"].ToString());
                    recordInfo.PersonName = ar["name"].ToString();
                    recordInfo.WorkSn = ar["work_sn"].ToString();
                    recordInfo.DepartName = ar["depart_name"].ToString();
                    recordInfo.PrincipalName = ar["principal_name"].ToString();
                    recordInfo.WorkType = ar["work_type_name"].ToString();

                    recordInfo.LeaveTypeName[0] = ar["shijias"].ToString();
                    recordInfo.LeaveTypeName[1] = ar["bingjias"].ToString();
                    recordInfo.LeaveTypeName[2] = ar["wuxinjias"].ToString();
                    recordInfo.LeaveTypeName[3] = ar["hunjias"].ToString();
                    recordInfo.LeaveTypeName[4] = ar["chanjias"].ToString();
                    recordInfo.LeaveTypeName[5] = ar["sangjias"].ToString();
                    recordInfo.LeaveTypeName[6] = ar["nianxius"].ToString();
                    recordInfo.LeaveTypeName[7] = ar["youxinjias"].ToString();
                    recordInfo.SumLeave = ar["jias"].ToString();

                    if (ar["normal"].ToString() != "")
                    {
                        recordInfo.WeekendType[0] = string.Format("{0:f2}", (float)Double.Parse(ar["normal"].ToString()));
                    }

                    if (ar["weekends"].ToString() != "")
                    {
                        recordInfo.WeekendType[1] = string.Format("{0:f2}", (float)Double.Parse(ar["weekends"].ToString()));
                    }

                    if (ar["festival"].ToString() != "")
                    {
                        recordInfo.WeekendType[2] = string.Format("{0:f2}", (float)Double.Parse(ar["festival"].ToString()));
                    }

                    if (ar["work_times"].ToString() != "")
                    {
                        int workTime = Int32.Parse(ar["work_times"].ToString());
                        recordInfo.WorkTime = string.Format("{0:f2}", (float)workTime / 60);
                    }

                    XiGouMonthAttendReportList.Add(recordInfo);
                }

                return XiGouMonthAttendReportList;
            }
            catch
            {
                return null;
            }
        }

        [Query(HasSideEffects = true)]
        public IEnumerable<XiGouInWellPersonDetailReport> GetXiGouInWellPersonDetail(DateTime beginTime, DateTime endTime, string personName)
        {
            string sqlWhere = "";
            
            //人名过滤
            if (personName != null && personName != "")
            {
                sqlWhere += String.Format(" and name like '%{0}%'", personName);
            }           

            string querySQL = string.Format(
             @"select * from (
                    select ar.person_id, pb.name, prin.principal_name, in_well_time,out_well_time,work_time,co.class_order_name, attend_day
                    from attend_record_normal ar left join person_base pb on ar.person_id = pb.person_id
                    left join principal prin on pb.principal_id = prin.principal_id
                    left join class_order_normal co on ar.class_order_id = co.class_order_id

                    union

                    select person_id, name, principal_name, in_time, out_time, work_time, co.class_order_name,attend_day from
                        (select inOut.person_id, pb.name,prin.principal_name, in_time, out_time, 
                        (EXTRACT(EPOCH FROM date_trunc('day',now()-date(in_time)))/60+ date_part('H', now()-date(in_time))*60 + date_part('m', now()-date(in_time)) )work_time,unnest(inOut.candidate_class_order) class_order_id,attend_day
                        from in_out_normal inOut left join person_base pb on inOut.person_id = pb.person_id
                        left join principal prin on pb.principal_id = prin.principal_id) a
                    left join class_order_normal co on  a.class_order_id= co.class_order_id ) b

            where attend_day >= '{0}' and attend_day <= '{1}' {2}
            order by attend_day,in_well_time  ",
                     beginTime, endTime, sqlWhere);

            List<XiGouInWellPersonDetailReport> inWellPersonDetailReport = new List<XiGouInWellPersonDetailReport>();
            DataTable dt = DbAccess.POSTGRESQL.Select(querySQL, "attend_record_all");
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }

            try
            {
                int index = 1;
                foreach (DataRow ar in dt.Rows)
                {
                    XiGouInWellPersonDetailReport recordInfo = new XiGouInWellPersonDetailReport();
                    recordInfo.Index = index++;
                    recordInfo.PersonId = Int32.Parse(ar["person_id"].ToString());
                    recordInfo.PersonName = ar["name"].ToString(); 
                    recordInfo.PrincipalName = ar["principal_name"].ToString();

                    recordInfo.InWellTime = "";
                    if (ar["in_well_time"].ToString() != "")
                    {
                        recordInfo.InWellTime = DateTime.Parse(ar["in_well_time"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    recordInfo.OutWellTime = "";
                    if (ar["out_well_time"].ToString() != "")
                    {
                        recordInfo.OutWellTime = DateTime.Parse(ar["out_well_time"].ToString()).ToString("yyyy-MM-dd HH:mm:ss"); 
                    }
                    recordInfo.ClassOrderName = ar["class_order_name"].ToString();

                    if (ar["work_time"].ToString() != "")
                    {
                        int workTime = (int)(double.Parse(ar["work_time"].ToString()));
                        recordInfo.WorkTime = string.Format("{0}:{1:d2}", workTime / 60, workTime % 60);
                    }

                    inWellPersonDetailReport.Add(recordInfo);
                }

                return inWellPersonDetailReport;
            }
            catch
            {
                return null;
            }
        }
    }

    /// <summary>
    /// 西沟一矿日考勤报表
    /// </summary>
    [DataContract]
    public class XiGouDayAttendReport
    {
        /// <summary>
        /// 序号
        /// </summary>        
        [DataMember]
        [Key]
        public int Index { get; set; }

        /// <summary>
        /// 人员id
        /// </summary>
        [DataMember]
        public int PersonId { get; set; }

        /// <summary>
        /// 工号
        /// </summary>        
        [DataMember]
        public string WorkSn { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [DataMember]
        public string PersonName { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>        
        [DataMember]
        public string DepartName { get; set; }

        /// <summary>
        /// 职务
        /// </summary>        
        [DataMember]
        public string PrincipalName { get; set; }

        /// <summary>
        /// 工种
        /// </summary>        
        [DataMember]
        public string WorkType { get; set; }  

        /// <summary>
        /// 上午入井时间
        /// </summary>   
        [DataMember]
        public string MorningInWellTime { get; set; }

        /// <summary>
        /// 上午出井时间
        /// </summary>   
        [DataMember]
        public string MorningOutWellTime { get; set; }

        /// <summary>
        /// 下午入井时间
        /// </summary>   
        [DataMember]
        public string AfternoonInWellTime { get; set; }

        /// <summary>
        /// 下午出井时间
        /// </summary>   
        [DataMember]
        public string AfternoonOutWellTime { get; set; }

        /// <summary>
        /// 总工作时间
        /// </summary>   
        [DataMember]
        public string WorkTime { get; set; }

        /// <summary>
        /// 假期类型
        /// </summary>   
        [DataMember]
        public string[] LeaveTypeName { get; set; }

        /// <summary>
        /// 上班类型： 平时上班、周末加班、节假日加班
        /// </summary>   
        [DataMember]
        public string[] WeekendType { get; set; }       

        public XiGouDayAttendReport()
        {
            LeaveTypeName = new string[8] { "", "", "", "", "", "", "", "" };
            WeekendType = new string[3] { "", "", "" };
        }
    }

    /// <summary>
    /// 西沟一矿月考勤报表
    /// </summary>
    [DataContract]
    public class XiGouMonthAttendReport
    {
        /// <summary>
        /// 序号
        /// </summary>        
        [DataMember]
        [Key]
        public int Index { get; set; }

        /// <summary>
        /// 人员id
        /// </summary>
        [DataMember]
        public int PersonId { get; set; }

        /// <summary>
        /// 工号
        /// </summary>        
        [DataMember]
        public string WorkSn { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [DataMember]
        public string PersonName { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>        
        [DataMember]
        public string DepartName { get; set; }

        /// <summary>
        /// 职务
        /// </summary>        
        [DataMember]
        public string PrincipalName { get; set; }

        /// <summary>
        /// 工种
        /// </summary>        
        [DataMember]
        public string WorkType { get; set; }  

        /// <summary>
        /// 上午入井时间
        /// </summary>   
        [DataMember]
        public string MorningInWellTime { get; set; }

        /// <summary>
        /// 上午出井时间
        /// </summary>   
        [DataMember]
        public string MorningOutWellTime { get; set; }

        /// <summary>
        /// 下午入井时间
        /// </summary>   
        [DataMember]
        public string AfternoonInWellTime { get; set; }

        /// <summary>
        /// 下午出井时间
        /// </summary>   
        [DataMember]
        public string AfternoonOutWellTime { get; set; }

        /// <summary>
        /// 总工作时间
        /// </summary>   
        [DataMember]
        public string WorkTime { get; set; }

        /// <summary>
        /// 假期类型
        /// </summary>   
        [DataMember]
        public string[] LeaveTypeName { get; set; }

        /// <summary>
        /// 上班类型： 平时上班、周末加班、节假日加班
        /// </summary>   
        [DataMember]
        public string[] WeekendType { get; set; }

        /// <summary>
        /// 假期总和
        /// </summary>   
        [DataMember]
        public string SumLeave { get; set; }

        public XiGouMonthAttendReport()
        {
            LeaveTypeName = new string[8] { "", "", "", "", "", "", "", "" };
            WeekendType = new string[3] { "", "", "" };
        }
    }

    /// <summary>
    /// 西沟一矿人员明细表
    /// </summary>
    [DataContract]
    public class XiGouInWellPersonDetailReport
    {
        /// <summary>
        /// 序号
        /// </summary>        
        [DataMember]
        [Key]
        public int Index { get; set; }

        /// <summary>
        /// 人员id
        /// </summary>
        [DataMember]
        public int PersonId { get; set; }        

        /// <summary>
        /// 姓名
        /// </summary>
        [DataMember]
        public string PersonName { get; set; }        

        /// <summary>
        /// 职务
        /// </summary>        
        [DataMember]
        public string PrincipalName { get; set; }      

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

        /// <summary>
        /// 工作时间
        /// </summary>   
        [DataMember]
        public string WorkTime { get; set; }       

        /// <summary>
        /// 班次
        /// </summary>   
        [DataMember]
        public string ClassOrderName { get; set; }

        public XiGouInWellPersonDetailReport()
        {           
        }
    }
}