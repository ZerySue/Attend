/*************************************************************************
** 文件名:   DomainServiceIriskingAttend_HuFeng.cs
×× 主要类:   DomainServiceIriskingAttend_HuFeng
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2014-6-19
** 修改人:  
** 日  期:       
** 描  述:   虎峰煤矿
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
        /// <summary>
        /// 获取日考勤统计表
        /// </summary>
        /// <returns>获取的机关考勤统计表记录列表</returns>
        [Query(HasSideEffects = true)]
        public IEnumerable<HuFengDayAttendReport> GetHuFengDayAttendCollect(DateTime beginTime, string[] departNames, string[] classOrderNames, string[] principalNames, string personName, string workSn)
        {
            string querySQL = string.Format(
             @"select p.person_id, p.work_sn, p.name, p.depart_name,  pr.principal_name, 
                ar.class_order_name,ar.in_well_time, ar.out_well_time, ar.work_time from attend_record_all ar 
                left join person p on ar.person_id = p.person_id 
                left join principal pr on pr.principal_id = p.principal_id 
                where ar.work_time >= 300 and ar.work_time <= 600 and ar.leave_type_id < 50 and to_char(ar.attend_day,'yyyy-MM-dd') = '{0}' ", beginTime.ToString("yyyy-MM-dd"));

            string sqlWhere = "";

            //部门过滤
            if (departNames != null && departNames.Length > 0)
            {
                sqlWhere += @" and p.depart_name in (";
                foreach (var item in departNames)
                {
                    sqlWhere += string.Format( @"'{0}',", item );
                }
                sqlWhere = sqlWhere.Remove(sqlWhere.LastIndexOf(","), 1);
                sqlWhere += ")";               
            }
            //班制过滤
            if (classOrderNames != null && classOrderNames.Length > 0)
            {
                sqlWhere += @" and ar.class_order_name in (";
                foreach (var item in classOrderNames)
                {
                    sqlWhere += string.Format( @"'{0}',", item );
                }
                sqlWhere = sqlWhere.Remove(sqlWhere.LastIndexOf(","), 1);
                sqlWhere += ")";               
            }
            //职务过滤
            if (principalNames != null && principalNames.Length > 0)
            {
                sqlWhere += @" and pr.principal_name in (";
                foreach (var item in principalNames)
                {
                    sqlWhere += string.Format( @"'{0}',", item );
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
            sqlWhere += " order by convert_to(p.depart_name,  E'GBK'),convert_to(p.name,  E'GBK')";

            querySQL += sqlWhere;

            List<HuFengDayAttendReport> recordList = new List<HuFengDayAttendReport>();
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
                    HuFengDayAttendReport recordInfo = new HuFengDayAttendReport();                    
                    recordInfo.Index = index++;
                    recordInfo.PersonId = Int32.Parse(ar["person_id"].ToString());
                    recordInfo.PersonName = ar["name"].ToString();
                    recordInfo.WorkSn = ar["work_sn"].ToString();
                    recordInfo.DepartName = ar["depart_name"].ToString();
                    recordInfo.ClassOrderName = ar["class_order_name"].ToString();
                    recordInfo.PrincipalName = ar["principal_name"].ToString();
                    recordInfo.InWellTime = ar["in_well_time"].ToString();
                    recordInfo.OutWellTime = ar["out_well_time"].ToString();
                    int workTime = Int32.Parse(ar["work_time"].ToString());
                    recordInfo.WorkTime = string.Format("{0:d2}:{1:d2}", workTime/60, workTime%60);
                    recordList.Add(recordInfo);
                }

                return recordList;
            }
            catch
            {
                return null;
            }
        }
        [Query(HasSideEffects = true)]
        public IEnumerable<HuFengMonthAttendReport> GetHuFengMonthAttendCollect(DateTime beginTime, DateTime endTime, string[] departNames, string personName, string workSn)
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
             @"select person_id, max(work_sn) work_sn, name, depart_name,
                array_agg(attend_day) as attend_days, array_agg(class_order_name) as class_order_names, array_agg(work_time) as work_times,  
                sum(work_time) totalWorkTime, count(attend_day) attendCount 
                from (select p.person_id, p.work_sn, p.name, p.depart_name, ar.class_order_name, ar.work_time, 
                ar.attend_day from attend_record_all ar 
                left join person p on ar.person_id = p.person_id                 
                where ar.work_time >= 300 and ar.work_time <= 600 and ar.leave_type_id < 50 and ar.attend_day between '{0}' and '{1}' {2}) dayAttend 
                group by person_id, name, depart_name order by convert_to(depart_name,  E'GBK'),convert_to(name,  E'GBK')  ",
                beginTime.ToString("yyyy-MM-dd 00:00:00"), endTime.ToString("yyyy-MM-dd 23:59:59"), sqlWhere);

            List<HuFengMonthAttendReport> recordList = new List<HuFengMonthAttendReport>();
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
                    HuFengMonthAttendReport recordInfo = new HuFengMonthAttendReport();
                    recordInfo.Index = index++;
                    recordInfo.PersonId = Int32.Parse(ar["person_id"].ToString());
                    recordInfo.PersonName = ar["name"].ToString();
                    recordInfo.WorkSn = ar["work_sn"].ToString();
                    recordInfo.DepartName = ar["depart_name"].ToString();
                    recordInfo.ClassOrderNames = (string[])ar["class_order_names"];
                    NpgsqlTimeStamp[] AttendDays = (NpgsqlTimeStamp[])ar["attend_days"];
                    recordInfo.WorkTimes = (int[])ar["work_times"];
                    recordInfo.DisplayInfo = new string[(endTime - beginTime).Days+1];
                    for (int count = 0; count < recordInfo.DisplayInfo.Count(); count++)
                    {
                        recordInfo.DisplayInfo[count] = "";
                    }
                    
                    for (int count = 0; count < AttendDays.Count(); count++)
                    {
                        int dayIndex = ((DateTime)AttendDays[count] - beginTime).Days;
                        recordInfo.DisplayInfo[dayIndex] =
                            recordInfo.ClassOrderNames[count] + string.Format("{0:d2}:{1:d2}", recordInfo.WorkTimes[count] / 60, recordInfo.WorkTimes[count] % 60);
                    }

                    int workTime = Int32.Parse(ar["totalWorkTime"].ToString());
                    recordInfo.TotalWorkTime = string.Format("{0:d2}:{1:d2}", workTime / 60, workTime % 60);
                    recordInfo.AttendCount = Int32.Parse(ar["attendCount"].ToString());

                    recordList.Add(recordInfo);
                }

                return recordList;
            }
            catch
            {
                return null;
            }
        }
    }

    /// <summary>
    /// 虎峰煤矿日考勤报表
    /// </summary>
    [DataContract]
    public class HuFengDayAttendReport
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
        /// 班次名称
        /// </summary>        
        [DataMember]
        public string ClassOrderName { get; set; }

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
        /// 总工作时间
        /// </summary>   
        [DataMember]
        public string WorkTime { get; set; }

        public HuFengDayAttendReport()
        {
            
        }
    }

    /// <summary>
    /// 虎峰煤矿月考勤报表
    /// </summary>
    [DataContract]
    public class HuFengMonthAttendReport
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
        /// 显示的班次名称和时间
        /// </summary>        
        [DataMember]
        public string[] DisplayInfo { get; set; }

        /// <summary>
        /// 班次名称
        /// </summary>        
        [DataMember]
        public string[] ClassOrderNames { get; set; }

        /// <summary>
        /// 出勤时间
        /// </summary>        
        [DataMember]
        public int[] WorkTimes { get; set; }      
       
        /// <summary>
        /// 出勤次数
        /// </summary>   
        [DataMember]
        public int AttendCount { get; set; }

        /// <summary>
        /// 总工作时间
        /// </summary>   
        [DataMember]
        public string TotalWorkTime { get; set; }

        public HuFengMonthAttendReport()
        {
            DisplayInfo = new string[0];
            ClassOrderNames = new string[0];
            WorkTimes = new int[0];           
        }
    }
}