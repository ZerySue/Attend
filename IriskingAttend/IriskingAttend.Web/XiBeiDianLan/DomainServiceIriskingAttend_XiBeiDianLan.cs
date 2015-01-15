/*************************************************************************
** 文件名:   DomainServiceIriskingAttend_XiBeiDianLan.cs
×× 主要类:   DomainServiceIriskingAttend_XiBeiDianLan
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2014-5-28
** 修改人:  
** 日  期:       
** 描  述:   西北电缆厂
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

namespace IriskingAttend.Web
{
    // TODO: 创建包含应用程序逻辑的方法。
    public partial class DomainServiceIriskingAttend : DomainService
    {
        /// <summary>
        /// 获取机关考勤统计表
        /// </summary>
        /// <returns>获取的机关考勤统计表记录列表</returns>
        public IEnumerable<OfficeAttend> GetOfficeAttend(DateTime beginTime, DateTime endTime, string[] departNames, string personName, string workSn)
        {
            string querySQL = string.Format(
             @"select * from ( select dayAttend.person_id, max(p.name ) as name, max(p.work_Sn ) as work_Sn,max(p.depart_name ) as depart_name,
                 max(dayAttend.class_type_name) class_type_name, 
                 sum( Moring ) Moring, sum( Afternoon ) Afternoon, sum( dayWorkCount) dayWorkCount, sum( Late) Late, 
                 sum( LeaveEarly) LeaveEarly, sum(AskLeave) AskLeave, sum(Shift) Shift from
                 ( select person_id, attend_day, max(class_type_name) class_type_name,max( Moring ) Moring, max( Afternoon ) Afternoon, 
                     ( max( Moring ) + max( Afternoon ) ) as dayWorkCount,   -----一天的工数
                      sum( Late) Late, sum( LeaveEarly) LeaveEarly, sum(AskLeave) AskLeave, sum(Shift) Shift from 
                      ( SELECT person_id, ct.class_type_name, to_date(to_char(attend_day,'yyyy-MM-dd'),'yyyy-MM-dd') as attend_day,lt.leave_type_name,
                          CASE WHEN (in_well_time IS NOT NULL AND out_well_time IS NOT NULL AND co.class_order_name like '%上午%') THEN 1 ELSE 0 END Moring, -----上午班次数
                          CASE WHEN (in_well_time IS NOT NULL AND out_well_time IS NOT NULL AND co.class_order_name like '%下午%')  THEN 1 ELSE 0 END Afternoon,-----下午班次数
                          CASE WHEN (ar.in_leave_type_name like '迟到')  THEN 1 ELSE 0 END Late,                                      -----迟到
                          CASE WHEN (ar.out_leave_type_name like '早退')  THEN 1 ELSE 0 END LeaveEarly,                                -----早退
                          CASE WHEN (lt.leave_type_name like '事假')  THEN (EXTRACT(EPOCH FROM date(ar.out_well_time)-date(ar.in_well_time)::timestamp)/86400+1) ELSE 0 END AskLeave,                                  -----事假
                          CASE WHEN (lt.leave_type_name like '休假')  THEN (EXTRACT(EPOCH FROM date(ar.out_well_time)-date(ar.in_well_time)::timestamp)/86400+1) ELSE 0 END Shift                                      -----休假
                       FROM attend_record_all ar LEFT JOIN class_order_normal co ON ar.class_order_id=co.class_order_id
                       LEFT JOIN leave_type lt on ar.leave_type_id = lt.leave_type_id  
                       left join class_type ct on co.class_type_id = ct.class_type_id
                       where attend_day between '{0}' and '{1}') classAttend 
                       group by person_id, attend_day ) dayAttend 
                left join person p on dayAttend.person_id = p.person_id group by dayAttend.person_id ) totalAttend ", beginTime, endTime);

            string sqlWhere = "";

            //部门过滤
            if (departNames != null && departNames.Length > 0)
            {
                StringBuilder sqlDepartWhere = new StringBuilder("where depart_name in (");
                foreach (var item in departNames)
                {
                    sqlDepartWhere.Append(@"'" + item + @"'"+ ",");
                }
                sqlDepartWhere.Remove(sqlDepartWhere.Length - 1, 1);
                sqlDepartWhere.Append(")");
                sqlWhere = sqlWhere + sqlDepartWhere;
            }
            //人名过滤
            if (personName != null && personName != "")
            {
                if (sqlWhere != "")
                {
                    sqlWhere += " and ";
                }
                else
                {
                    sqlWhere = " where ";
                }
                sqlWhere = sqlWhere + String.Format(" name like '%{0}%'", personName);
            }

            //工号过滤
            if (workSn != null && workSn != "")
            {
                if (sqlWhere != "")
                {
                    sqlWhere += " and ";
                }
                else
                {
                    sqlWhere = " where ";
                }
                sqlWhere = sqlWhere + String.Format(" work_sn = '{0}'", workSn);
            }
            sqlWhere += " order by convert_to(depart_name,  E'GBK'),convert_to(name,  E'GBK')";

            querySQL += sqlWhere;

            List<OfficeAttend> recordList = new List<OfficeAttend>();
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
                    OfficeAttend recordInfo = new OfficeAttend();                    
                    recordInfo.Index = index++;
                    recordInfo.PersonId = Int32.Parse(ar["person_id"].ToString());
                    recordInfo.PersonName = ar["name"].ToString();
                    recordInfo.WorkSn = ar["work_sn"].ToString();
                    recordInfo.DepartName = ar["depart_name"].ToString();
                    recordInfo.ClassType = ar["class_type_name"].ToString();
                    recordInfo.MorningAttendNum = Int32.Parse(ar["Moring"].ToString());
                    recordInfo.AfternoonAttendNum = Int32.Parse(ar["Afternoon"].ToString());
                    recordInfo.DayAttendNum = float.Parse(ar["dayWorkCount"].ToString()) /2;
                    recordInfo.LateNum = Int32.Parse(ar["Late"].ToString());
                    recordInfo.LeaveEarlyNum = Int32.Parse(ar["LeaveEarly"].ToString());
                    recordInfo.AskLeaveNum = Int32.Parse(ar["AskLeave"].ToString());
                    recordInfo.ShiftHolidayNum = Int32.Parse(ar["Shift"].ToString());
                    
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
        public IEnumerable<PersonAttendRecord> GetPersonAttendRecord(DateTime beginTime, DateTime endTime, int personId, int attendType )
        {
            //组装sql语句的where条件          
            string sql_where = "";
            sql_where = String.Format(" and person_id = {0}", personId);

            if (attendType == 5)
            {
                sql_where += " and class_order_name like '%上午%' and in_well_time IS NOT NULL AND out_well_time IS NOT NULL ";
            }
            else if (attendType == 6)
            {
                sql_where += " and class_order_name like '%下午%' and in_well_time IS NOT NULL AND out_well_time IS NOT NULL";
            }
            else if (attendType == 8)
            {
                sql_where += " and in_leave_type_name like '%迟到%'";
            }
            else if (attendType == 9)
            {
                sql_where += " and out_leave_type_name like '%早退%'";
            }

            string sql_all = string.Format(@"SELECT *, lt.leave_type_name
                              FROM attend_record_all ar
                              LEFT JOIN leave_type lt on ar.leave_type_id = lt.leave_type_id                              
                              WHERE  attend_day between '{0}' and '{1}' {2} and ar.leave_type_id < 50 
                              ORDER BY attend_day",
                              beginTime, endTime, sql_where);

            List<PersonAttendRecord> query = new List<PersonAttendRecord>();
            DataTable dt = DbAccess.POSTGRESQL.Select(sql_all, "attend_record_base");
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }

            int count = 1;
            for (int index = 0; index < dt.Rows.Count; index++)
            {
                PersonAttendRecord record = new PersonAttendRecord();

                //如果可为空需要判断
                record.RecordId = Convert.ToInt32(dt.Rows[index]["attend_record_id"]);

                if (DBNull.Value != dt.Rows[index]["attend_day"])
                {
                    record.AttendDay = DateTime.Parse(dt.Rows[index]["attend_day"].ToString()).ToString("yyyy-MM-dd");
                }

                if (DBNull.Value != dt.Rows[index]["dev_group"])
                {
                    record.DevGroup = (int.Parse(dt.Rows[index]["dev_group"].ToString()) == 3) ? "出入井" : "上下班";
                }
               
                if (DBNull.Value != dt.Rows[index]["in_well_time"])
                {
                    record.InWellTime = dt.Rows[index]["in_well_time"].ToString().Trim();
                }

                if (DBNull.Value != dt.Rows[index]["out_well_time"])
                {
                    record.OutWellTime = dt.Rows[index]["out_well_time"].ToString().Trim();
                }

                record.ClassOrderName = dt.Rows[index]["class_order_name"].ToString();                
                record.InLeaveTypeName= dt.Rows[index]["in_leave_type_name"].ToString();

                record.OutLeaveTypeName = dt.Rows[index]["out_leave_type_name"].ToString();
               
                record.LeaveTypeName = dt.Rows[index]["leave_type_name"].ToString();
              
                if (DBNull.Value != dt.Rows[index]["work_cnt"])
                {
                    //工数数据库中以10的倍数存储，取出应除以10
                    record.WorkCnt = Int32.Parse(dt.Rows[index]["work_cnt"].ToString()) / 10.0;
                }
                if (DBNull.Value != dt.Rows[index]["work_time"])
                {
                    int temp = int.Parse(dt.Rows[index]["work_time"].ToString());
                    //化成分钟数，如果分钟数为一位，则在其前补零
                    if ((temp % 60) > 9)
                    {
                        record.WorkTime = (temp / 60).ToString() + ":" + (temp % 60).ToString();
                    }
                    else
                    {
                        record.WorkTime = (temp / 60).ToString() + ":0" + (temp % 60).ToString();
                    }
                }
                record.Index = count++.ToString();

                query.Add(record);

            }

            return query;
        }
    }

    /// <summary>
    /// 西北电缆厂
    /// </summary>
    [DataContract]
    public class OfficeAttend
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
        /// 班制
        /// </summary>        
        [DataMember]
        public string ClassType { get; set; }

        /// <summary>
        /// 上午班出勤次数
        /// </summary>   
        [DataMember]
        public int MorningAttendNum { get; set; }

        /// <summary>
        /// 下午班出勤次数
        /// </summary>   
        [DataMember]
        public int AfternoonAttendNum { get; set; }

        /// <summary>
        /// 出勤天数
        /// </summary>   
        [DataMember]
        public float DayAttendNum { get; set; }

        /// <summary>
        /// 迟到
        /// </summary>     
        [DataMember]
        public int LateNum { get; set; }

        /// <summary>
        /// 早退
        /// </summary>    
        [DataMember]
        public int LeaveEarlyNum { get; set; }

        /// <summary>
        /// 休假
        /// </summary>    
        [DataMember]
        public int ShiftHolidayNum { get; set; }

        /// <summary>
        /// 事假天数
        /// </summary>     
        [DataMember]
        public int AskLeaveNum { get; set; }

        /// <summary>
        /// 备注
        /// </summary>      
        [DataMember]
        public string Note { get; set; }

        public OfficeAttend()
        {
            Note = "";
        }
    }

    /// <summary>
    /// 考勤记录实体数据类
    /// </summary>
    [DataContract]
    public class PersonAttendRecord
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

        /// <summary>
        /// 考勤类型
        /// </summary>
        [DataMember]
        public string LeaveTypeName { get; set; }

        /// <summary>
        /// 入井考勤类型
        /// </summary>
        [DataMember]
        public string InLeaveTypeName { get; set; }

        /// <summary>
        /// 出井考勤类型
        /// </summary>
        [DataMember]
        public string OutLeaveTypeName { get; set; }

        /// <summary>
        /// 考勤时间
        /// </summary>
        [DataMember]
        public string AttendDay { get; set; }

        /// <summary>
        /// 工  时
        /// </summary>
        [DataMember]
        public string WorkTime { get; set; }

        /// <summary>
        /// 记录类型
        /// </summary>
        [DataMember]
        public string DevGroup { get; set; }

        /// <summary>
        /// 工  数
        /// </summary>
        [DataMember]
        public double WorkCnt { get; set; }

        /// <summary>
        /// 班 次
        /// </summary>
        [DataMember]
        public string ClassOrderName { get; set; }

    }
}