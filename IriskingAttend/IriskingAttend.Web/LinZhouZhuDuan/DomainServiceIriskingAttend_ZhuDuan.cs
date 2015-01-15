/*************************************************************************
** 文件名:   DomainServiceIriskingAttend_ZhuDuan.cs.cs
** 主要类:   DomainServiceIriskingAttend_ZhuDuan.cs
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2014-7-22
** 修改人:   
** 日  期:
** 描  述:   用于林州铸锻报表查询的域服务
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

    public partial class DomainServiceIriskingAttend
    {       
        /// <summary>
        /// 获得总的考勤统计信息
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="departName"></param>
        /// <param name="personName"></param>
        /// <param name="workSn"></param>
        /// <returns></returns>
        [Query(HasSideEffects = true)]
        public IEnumerable<ZhuDuanMonthAttendReport> GetZhuDuanMonthAttendList(DateTime beginTime, DateTime endTime, string[] departNames, string[] personNames, string[] workSns)
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
            if (personNames != null && personNames.Length > 0)
            {
                sqlWhere += @" and p.name in (";
                foreach (var item in personNames)
                {
                    sqlWhere += string.Format(@"'{0}',", item);
                }
                sqlWhere = sqlWhere.Remove(sqlWhere.LastIndexOf(","), 1);
                sqlWhere += ")";               
            }

            //工号过滤
            if (workSns != null && workSns.Length > 0)
            {
                sqlWhere += @" and p.work_sn in (";
                foreach (var item in workSns)
                {
                    sqlWhere += string.Format(@"'{0}',", item);
                }
                sqlWhere = sqlWhere.Remove(sqlWhere.LastIndexOf(","), 1);
                sqlWhere += ")";  
            }

            string querySQL = string.Format(
             @"select person_id, max(work_sn) work_sn, name, depart_name,
                day_offset, array_agg(class_order_name) as class_order_names               
                from (select p.person_id, p.work_sn, p.name, p.depart_name, co.class_order_name, 
                        case when co.class_order_name like '%夜%' then 
			                    ( case when (EXTRACT( EPOCH FROM date_trunc('day', in_well_time) - date_trunc('day', attend_day))/(60) + 
                                        date_part('H', in_well_time)*60 + date_part('m', in_well_time)) between in_well_start_time and in_well_end_time 
                                        then 1 else 0 end) 
                        else ( case when (EXTRACT(EPOCH FROM date_trunc('day', in_well_time) - date_trunc('day', attend_day))/(60) + date_part('H', in_well_time)*60 + date_part('m', in_well_time) 
                                    between co.in_well_start_time and co.in_well_end_time
                                    and EXTRACT(EPOCH FROM date_trunc('day', out_well_time) - date_trunc('day', attend_day))/(60) + date_part('H', out_well_time)*60 + date_part('m', out_well_time) 
                                    between co.out_well_start_time and co.out_well_end_time) then 1 else 0 end )  
                        end isvalid,                
                EXTRACT(EPOCH FROM date(attend_day)-date('{0}')::timestamp)/86400 day_offset from attend_record_base ar 
                left join person p on ar.person_id = p.person_id 
                left join class_order_normal co on ar.class_order_id = co.class_order_id                
                where ar.attend_day between '{0}' and '{1}' and in_well_time is not null and out_well_time is not null 
                    and ( co.class_order_name like '%夜%' or co.class_order_name like '%上午%' or co.class_order_name like '%下午%'){2}) dayAttend where isvalid = 1
                group by person_id, depart_name, name, day_offset order by convert_to(depart_name,  E'GBK'),convert_to(name,  E'GBK'),day_offset ",
                beginTime.ToString("yyyy-MM-dd 00:00:00"), endTime.ToString("yyyy-MM-dd 23:59:59"), sqlWhere);

            List<ZhuDuanMonthAttendReport> recordList = new List<ZhuDuanMonthAttendReport>();
            DataTable dt = DbAccess.POSTGRESQL.Select(querySQL, "attend_record_all");
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }

            try
            {
                int index = 1;
                int personId = -1;
                int dayCount = (endTime - beginTime).Days + 1;

                ZhuDuanMonthAttendReport totalRecord = new ZhuDuanMonthAttendReport();
                totalRecord.PersonId = -1;
                totalRecord.AttendCount = 0;
                totalRecord.DepartName = "合计";
                totalRecord.DisplaySignal = new string[dayCount];
                for (int dayIndex = 0; dayIndex < dayCount; dayIndex++)
                {
                    totalRecord.DisplaySignal[dayIndex] = "";
                }

                ZhuDuanMonthAttendReport recordInfo = null;
                
                foreach (DataRow ar in dt.Rows)
                {
                    if (personId != Int32.Parse(ar["person_id"].ToString()))
                    {
                        if (recordInfo != null)
                        {
                            recordList.Add(recordInfo);
                        }

                        recordInfo = new ZhuDuanMonthAttendReport();
                        recordInfo.DisplaySignal = new string[dayCount];
                        for (int dayIndex = 0; dayIndex < dayCount; dayIndex++)
                        {
                            recordInfo.DisplaySignal[dayIndex] = "";
                        }

                        recordInfo.Index = index++;
                        recordInfo.PersonId = Int32.Parse(ar["person_id"].ToString());
                        recordInfo.PersonName = ar["name"].ToString();
                        recordInfo.WorkSn = ar["work_sn"].ToString();
                        recordInfo.DepartName = ar["depart_name"].ToString();
                        recordInfo.ClassType = "上午/下午";
                        recordInfo.AttendCount = 0;
                        personId = recordInfo.PersonId;
                    }
                    int dayOffset = Int32.Parse(ar["day_offset"].ToString());
                    string[] classOrderNames;
                    string yeBan = "   ";
                    string shangWuBan = "   ";
                    string xiaWuBan = "   ";
                    if (ar["class_order_names"] != null && ar["class_order_names"].ToString() != "")
                    {                       
                        classOrderNames = (string[])ar["class_order_names"];

                        foreach (string item in classOrderNames)
                        {
                            if (item.Contains("上午"))
                            {
                                shangWuBan = item;
                            }
                            else if (item.Contains("下午"))
                            {
                                xiaWuBan = item;
                            }
                            else if (item.Contains("夜"))
                            {
                                yeBan = item;
                            }
                        }
                    }

                    if (yeBan != "   ")
                    {
                        totalRecord.AttendCount += 1;
                        recordInfo.AttendCount += 1;
                        recordInfo.DisplaySignal[dayOffset] = "1";
                    }
                    else if (shangWuBan != "   " || xiaWuBan != "   ") 
                    {
                        if (shangWuBan != "   ")
                        {
                            totalRecord.AttendCount += 0.5f;
                            recordInfo.AttendCount += 0.5f;
                            shangWuBan = "0.5";
                        }
                        if (xiaWuBan != "   ")
                        {
                            totalRecord.AttendCount += 0.5f;
                            recordInfo.AttendCount += 0.5f;
                            xiaWuBan = "0.5";
                        }
                        recordInfo.DisplaySignal[dayOffset] = string.Format("{0}|{1}", shangWuBan, xiaWuBan);
                    }                   
                }
                totalRecord.Index = index;
                recordList.Add(recordInfo);
                recordList.Add( totalRecord );

                return recordList;
            }
            catch
            {
                return null;
            }
        }       
    }

    /// <summary>
    /// 林州铸锻月考勤统计实体数据类
    /// </summary>
    [DataContract]
    public class ZhuDuanMonthAttendReport
    {
        /// <summary>
        /// 姓名id
        /// </summary>
        [DataMember]
        [Key]
        public int PersonId { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        [DataMember]        
        public int Index { get; set; }       

        /// <summary>
        /// 姓名
        /// </summary>
        [DataMember]
        public string PersonName { get; set; }

        /// <summary>
        /// 工号
        /// </summary>
        [DataMember]
        public string WorkSn { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        [DataMember]
        public string DepartName { get; set; }

        /// <summary>
        /// 前台显示 = 上午 + “\r\n” + 下午
        /// </summary>
        [DataMember]
        public string ClassType { get; set; } 

        /// <summary>
        /// 从begintime到endtime的显示
        /// </summary> 
        [DataMember]
        public string[] DisplaySignal { get; set; }
        
        /// <summary>
        /// 考勤总次数
        /// </summary>      
        [DataMember]
        public float AttendCount { get; set; }
       
    }  

}