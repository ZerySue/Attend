/*************************************************************************
** 文件名:   DomainServiceIriskingAttend_LiuHuangGou.cs
** 主要类:   DomainServiceIriskingAttend_LiuHuangGou
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   yht 
** 日  期:   2014-10-11
** 修改人:   
** 日  期:
** 描  述:   用于报表查询的域服务
**
** 版  本:   1.0.0
** 备  注:  命名及代码编写遵守C#编码规范
**
 * ***********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text;
using Irisking.DataBaseAccess;
using Irisking.Web.DataModel;

namespace IriskingAttend.Web
{
    public partial class DomainServiceIriskingAttend
    {
        /// <summary>
        /// 报表查询结果集合
        /// </summary>
        public static List<PersonMonthAttend> PersonMonthAttendList = new List<PersonMonthAttend>();

        #region 生成考勤报表
        /// <summary>
        /// 生成考勤报表
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="departName"></param>
        /// <param name="personName"></param>
        /// <returns></returns>
        public IEnumerable<PersonMonthAttend> GetPersonMonthAttendList(DateTime beginTime, DateTime endTime, string[] departName, string[] personName)
        {
            PersonMonthAttendList = new List<PersonMonthAttend>();
            System.Diagnostics.Trace.WriteLine("开始执行：" + DateTime.Now.ToLongTimeString());
            //获取人员信息
            List<UserPersonInfo> personList = (List<UserPersonInfo>)GetPersonInfo(departName, personName, null);
            //初始化数据
            InitPersonMonthAttendList(PersonMonthAttendList, personList, beginTime, endTime);
            System.Diagnostics.Trace.WriteLine("获取人员信息：" + DateTime.Now.ToLongTimeString());

            #region 获取节假日
            string querySql = string.Format(@"select * from festival where begin_time >= '{0}' and begin_time < '{1}'", beginTime, endTime);
            List<FestivalInfo> festivalList = (List<FestivalInfo>)GetFestivalBySql(querySql);
            List<DateTime> festivalDate = new List<DateTime>();
            if (festivalList != null)
            {
                foreach (FestivalInfo item in festivalList)
                {
                    for (DateTime temp = Convert.ToDateTime(item.begin_time); temp <= Convert.ToDateTime(item.end_time); temp = temp.AddDays(1))
                    {
                        festivalDate.Add(temp);
                    }
                }
            }
            #endregion

            #region 拼接查询条件
            string sql_where = "";
            if (departName != null && departName.Count() > 0)
            {
                sql_where = string.Format(" and depart_name in ( ");
                for (int index = 0; index < departName.Count(); index++)
                {
                    sql_where += string.Format(" '{0}', ", departName[index]);
                }

                sql_where = sql_where.Remove(sql_where.LastIndexOf(","), 1);
                sql_where += ")";
            }

            if (personName != null && personName.Count() > 0)
            {
                sql_where += string.Format(" and name in ( ");
                for (int index = 0; index < personName.Count(); index++)
                {
                    sql_where += string.Format(" '{0}', ", personName[index]);
                }

                sql_where = sql_where.Remove(sql_where.LastIndexOf(","), 1);
                sql_where += ")";
            }

            if (beginTime != null && endTime != null)
            {
                sql_where += string.Format(" and attend_day >=");
                sql_where += string.Format(" '{0}'", beginTime);
                sql_where += string.Format(" and attend_day <=");
                sql_where += string.Format(" '{0}'", endTime.AddDays(1));//此处加一天是为方便计算大班值班的第二天的考勤

            }
            #endregion
           
            //签到班查询
            string sql_all = string.Format(@"select a.person_id,p.name, work_time, is_valid, a.class_order_id,c.attend_sign,c.class_order_name,in_leave_type_id,to_char(attend_day,'yyyy-MM-DD') attend_day, out_leave_type_id,array_to_string(recog_sign_time,',','*') recog_sign_time,
case when p.class_type_id_on_ground is not null then p.class_type_id_on_ground else p.class_type_id end class_type_id,
 EXTRACT(EPOCH FROM date_trunc('day', in_well_time) - date_trunc('day', attend_day))/(60) + date_part('H', in_well_time)*60 + date_part('m', in_well_time) in_h,
EXTRACT(EPOCH FROM date_trunc('day', out_well_time) - date_trunc('day', attend_day))/(60) + date_part('H', out_well_time)*60 + date_part('m', out_well_time) out_h,
EXTRACT(EPOCH FROM date_trunc('day',a.attend_day)-'{1}')/(24*3600) as day_offset
  from attend_record_sign a left join person p on p.person_id = a.person_id left join class_order_base c on a.class_order_id=c.class_order_id where p.delete_time is null {0} 
        order by convert_to(depart_name, 'GBK'),convert_to(name, 'GBK'), attend_day ", sql_where, beginTime);

            DataTable dt = DbAccess.POSTGRESQL.Select(sql_all, "attend_record_sign");
            System.Diagnostics.Trace.WriteLine("签到班查询：" + DateTime.Now.ToLongTimeString());
            //获取定位数据
            List<MonthCheckRecord> MonthCheckRecordList = GetMonthCheckRecordList(PersonMonthAttendList, beginTime, endTime.AddDays(1));
            System.Diagnostics.Trace.WriteLine("获取定位数据：" + DateTime.Now.ToLongTimeString());
            if (null != dt && dt.Rows.Count > 0)
            {
                PersonMonthAttendDetail(PersonMonthAttendList, dt, beginTime, endTime, false, festivalDate, MonthCheckRecordList);
                System.Diagnostics.Trace.WriteLine("统计签到班考勤：" + DateTime.Now.ToLongTimeString());
            }

            //三八制查询
            sql_all = string.Format(@"select a.person_id,p.name, work_time, is_valid, a.class_order_id,c.attend_sign,c.class_order_name,in_leave_type_id,to_char(attend_day,'yyyy-MM-DD') attend_day, out_leave_type_id,
case when p.class_type_id_on_ground is not null then p.class_type_id_on_ground else p.class_type_id end class_type_id,
 EXTRACT(EPOCH FROM date_trunc('day', in_well_time) - date_trunc('day', attend_day))/(60) + date_part('H', in_well_time)*60 + date_part('m', in_well_time) in_h,
EXTRACT(EPOCH FROM date_trunc('day', out_well_time) - date_trunc('day', attend_day))/(60) + date_part('H', out_well_time)*60 + date_part('m', out_well_time) out_h,
EXTRACT(EPOCH FROM date_trunc('day',a.attend_day)-'{1}')/(24*3600) as day_offset
  from attend_record_normal a left join person p on p.person_id = a.person_id left join class_order_base c on a.class_order_id=c.class_order_id where p.delete_time is null {0} 
        order by convert_to(depart_name, 'GBK'),convert_to(name, 'GBK'), attend_day ", sql_where, beginTime);
            dt = DbAccess.POSTGRESQL.Select(sql_all, "attend_record_sign");
            System.Diagnostics.Trace.WriteLine("三八制查询：" + DateTime.Now.ToLongTimeString());
            if (null != dt && dt.Rows.Count > 0)
            {
                PersonMonthAttendDetail(PersonMonthAttendList, dt, beginTime, endTime, true, festivalDate, MonthCheckRecordList);
                System.Diagnostics.Trace.WriteLine("统计三八制考勤：" + DateTime.Now.ToLongTimeString());
            }

            GetLeaveAttendDetail(PersonMonthAttendList, beginTime, endTime, festivalDate,departName,personName);
            System.Diagnostics.Trace.WriteLine("统计请假：" + DateTime.Now.ToLongTimeString());
            GetAbsenteeismDays(PersonMonthAttendList, beginTime, endTime, festivalDate);
            System.Diagnostics.Trace.WriteLine("统计旷工：" + DateTime.Now.ToLongTimeString());
            return PersonMonthAttendList;
        }

        #endregion

        #region 初始化数据
        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <param name="PersonMonthAttendList"></param>
        /// <param name="personList"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        private void InitPersonMonthAttendList(List<PersonMonthAttend> PersonMonthAttendList, List<UserPersonInfo> personList, DateTime beginTime, DateTime endTime)
        {
            PersonMonthAttend tempAttend = new PersonMonthAttend();
            int personIndex = 1;
            if (personList == null || personList.Count <= 0)
            {
                return;
            }

            int dayCount = (endTime - beginTime).Days+1;

            foreach (UserPersonInfo item in personList)
            {
                tempAttend = null;
                tempAttend = new PersonMonthAttend();
                tempAttend.Index = personIndex++;
                tempAttend.DepartName = item.depart_name;
                tempAttend.PersonName = item.person_name;
                tempAttend.WorkSn = item.work_sn.Trim();
                tempAttend.PersonId = item.person_id;
                tempAttend.Remark = item.work_type_name;
                if (dayCount < 31)
                {
                    tempAttend.ClassOrder = new string[31];
                    tempAttend.GroupTimes = new string[31];
                }
                else
                {
                    tempAttend.ClassOrder = new string[dayCount];
                    tempAttend.GroupTimes = new string[dayCount];
                }

                int dayIndex = 0;
                for (dayIndex = 0; dayIndex < dayCount; dayIndex++)
                {
                    tempAttend.ClassOrder[dayIndex] = string.Empty;
                }
                if (dayCount < 31)
                {
                    for (dayIndex = dayCount; dayIndex < 31; dayIndex++)
                    {
                        tempAttend.ClassOrder[dayIndex] = "*";
                    }
                }
                PersonMonthAttendList.Add(tempAttend);
            }
        }

        #endregion

        #region 查询考勤上下班时间

        /// <summary>
        /// 查询考勤上下班时间
        /// </summary>
        /// <returns></returns>
        public List<ClassOrderTimes> GetClassTimesList()
        {
            List<ClassOrderTimes> list = new List<ClassOrderTimes>();
            //查询正常班次
            string sql = "SELECT attend_sign,in_well_start_time, in_well_end_time,out_well_start_time, out_well_end_time FROM class_order_normal where attend_sign!=''";
            DataTable dt = DbAccess.POSTGRESQL.Select(sql, "class_order_normal");
            if (null != dt && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new ClassOrderTimes()
                    {
                        ClassOrderName = dr["attend_sign"].ToString(),
                        InWellEndTime = int.Parse(dr["in_well_end_time"].ToString()),
                        InWellStartTime = int.Parse(dr["in_well_start_time"].ToString()),
                        OutWellEndTime = int.Parse(dr["out_well_end_time"].ToString()),
                        OutWellStartTime = int.Parse(dr["out_well_start_time"].ToString())
                    });
                }
            }
            //查询签到班
            sql = "";
            sql += "SELECT attend_sign, section_begin_mins, section_end_mins FROM \"class_order_sign.section\" a ";
            sql += "inner join class_order_sign c on a.class_order_id=c.class_order_id and c.attend_sign='大' order by section_begin_mins limit 1 offset 2";
            dt = DbAccess.POSTGRESQL.Select(sql, "class_order_sign");
            if (null != dt && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new ClassOrderTimes()
                    {
                        ClassOrderName = dr["attend_sign"].ToString(),
                        InWellEndTime = int.Parse(dr["section_end_mins"].ToString()),
                        InWellStartTime = int.Parse(dr["section_begin_mins"].ToString())
                    });
                }
            }
            return list;
        }
        #endregion

        #region 统计考勤数据
        /// <summary>
        /// 统计考勤数据
        /// </summary>
        /// <param name="personMonthAttendList"></param>
        /// <param name="dt"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="isNormal"></param>
        /// <param name="festivalDate"></param>
        /// <param name="shiftDate"></param>
        private void PersonMonthAttendDetail(List<PersonMonthAttend> personMonthAttendList, DataTable dt, DateTime beginTime, DateTime endTime, bool isNormal, List<DateTime> festivalDate, List<MonthCheckRecord> monthCheckRecordList)
        {
            int personIndex = 0;
            int dayOffset = 0;
            string attendSign = string.Empty;//班次
            DateTime attendDay = DateTime.Now;//考勤归属日
            string leveTypeName = string.Empty;//请假类型

            MonthCheckRecord record = null;//定位数据
            List<ClassOrderTimes> classTimesList = GetClassTimesList();//地面与虹膜上下班时间
            bool overClass = true;//是否加班
            for (int attendIndex = 0; attendIndex < dt.Rows.Count; attendIndex++)
            {
                while (personIndex < personMonthAttendList.Count() && personMonthAttendList[personIndex].PersonId != Convert.ToInt32(dt.Rows[attendIndex]["person_id"]))
                {
                    personIndex++;
                }
                dayOffset = Convert.ToInt32(dt.Rows[attendIndex]["day_offset"]);
                if (personIndex >= personMonthAttendList.Count())
                {
                    return;
                }
                if (dayOffset >= personMonthAttendList[personIndex].ClassOrder.Length)
                {
                    continue;
                }
                try
                {
                    int worktimes = 0;
                    int inouttimes = 0;//出勤天数
                    attendSign = dt.Rows[attendIndex]["attend_sign"].ToString();
                    worktimes = Convert.ToInt32(dt.Rows[attendIndex]["work_time"]);//虹膜考勤时间
                    attendDay = DateTime.Parse(dt.Rows[attendIndex]["attend_day"].ToString());//考勤归属日
                    int isValid = Convert.ToInt32(dt.Rows[attendIndex]["is_valid"]);//是否正常考勤
                    personMonthAttendList[personIndex].Remark = dt.Rows[attendIndex]["class_order_name"].ToString();
                    //是否加班
                    if (!festivalDate.Contains(beginTime.AddDays(dayOffset)))
                    {
                        overClass = false;
                    }
                    else
                    {
                        overClass = true;
                    }
                    if (worktimes > 0)
                    {
                        int inH = Convert.ToInt32(dt.Rows[attendIndex]["in_h"]);//上班时间，下班时间
                        int outH = Convert.ToInt32(dt.Rows[attendIndex]["out_h"]);
                        record = monthCheckRecordList.Where(a => a.PepoleExNumber == personMonthAttendList[personIndex].WorkSn && a.DownWellDay == attendDay.ToString("yyyyMMdd")).OrderByDescending(a => a.WellTime).FirstOrDefault();//定位数据

                        #region 地面大班
                        personMonthAttendList[personIndex].GroupTimes[dayOffset] = worktimes.ToString();//只要有工作时间就不是旷工
                        if (!isNormal)
                        {
                            //考勤签到
                            string[] signTimes = dt.Rows[attendIndex]["recog_sign_time"].ToString().Split(new char[] { ',' });
                            //大班值班，*代表没有签到记录
                            if (signTimes[3] != "*" && signTimes[0] != "*")
                            {
                                DateTime nextDay = DateTime.Parse(dt.Rows[attendIndex + 1]["attend_day"].ToString());//获取第二天
                                string[] NextSignTimes = dt.Rows[attendIndex + 1]["recog_sign_time"].ToString().Split(new char[] { ',' }); ;//第二天签到时间
                                //有下井和无下井的请客
                                if (((null != record && record.WellTime > 0) || (null == record && signTimes[1] != "*"))
                                    && (nextDay - attendDay).Days == 1 && !(signTimes[2] == "*" && signTimes[3] == "*") && NextSignTimes[0] != "*")
                                {
                                    personMonthAttendList[personIndex].ClassOrder[dayOffset] = "大值";
                                    if (string.IsNullOrEmpty(personMonthAttendList[personIndex].BigDutyTimes))
                                    {
                                        personMonthAttendList[personIndex].BigDutyTimes = "1";
                                    }
                                    else
                                    {
                                        personMonthAttendList[personIndex].BigDutyTimes = (int.Parse(personMonthAttendList[personIndex].BigDutyTimes) + 1).ToString();
                                    }
                                    inouttimes = 1;
                                }
                            }
                            //地面大班工数
                            else
                            {
                                if (null == record || record.UpWellTime > outH || record.UpWellTime == 0 || record.DownWellTime == 0 || record.WellTime == 0)
                                {
                                    record = null;
                                }
                                if (((null == record && signTimes[1] != "*") || (null != record && record.WellTime > 0 && record.WellTime < 120))
                                 && signTimes[2] != "*" && signTimes[0] != "*"
                                )
                                {
                                    personMonthAttendList[personIndex].ClassOrder[dayOffset] = "大";

                                    if (string.IsNullOrEmpty(personMonthAttendList[personIndex].GroupBigTimes))
                                    {
                                        personMonthAttendList[personIndex].GroupBigTimes = "1";
                                    }
                                    else
                                    {
                                        personMonthAttendList[personIndex].GroupBigTimes = (int.Parse(personMonthAttendList[personIndex].GroupBigTimes) + 1).ToString();
                                    }
                                    inouttimes = 1;

                                }
                                //大班入井工数
                                else if (null != record && record.WellTime >= 120 && record.WellTime < 480 && signTimes[2] != "*" && signTimes[0] != "*")
                                {
                                    personMonthAttendList[personIndex].ClassOrder[dayOffset] = "大1";

                                    if (string.IsNullOrEmpty(personMonthAttendList[personIndex].BigInWellTimes))
                                    {
                                        personMonthAttendList[personIndex].BigInWellTimes = "1";
                                    }
                                    else
                                    {
                                        personMonthAttendList[personIndex].BigInWellTimes = (int.Parse(personMonthAttendList[personIndex].BigInWellTimes) + 1).ToString();
                                    }
                                    inouttimes = 1;
                                }
                                //小班入井工数
                                else if (null != record && record.WellTime >= 480 && signTimes[2] != "*" && signTimes[0] != "*")
                                {

                                    personMonthAttendList[personIndex].ClassOrder[dayOffset] = "早1";

                                    if (string.IsNullOrEmpty(personMonthAttendList[personIndex].SmallInWellTimes))
                                    {
                                        personMonthAttendList[personIndex].SmallInWellTimes = "1";
                                    }
                                    else
                                    {
                                        personMonthAttendList[personIndex].SmallInWellTimes = (int.Parse(personMonthAttendList[personIndex].SmallInWellTimes) + 1).ToString();
                                    }

                                    inouttimes = 1;
                                }
                                //}
                            }

                            //请假
                        }
                        #endregion

                        #region 三八班制
                        else
                        {
                            int inLeaveType = Convert.ToInt32(dt.Rows[attendIndex]["in_leave_type_id"]);//上班类型
                            int outLeaveType = Convert.ToInt32(dt.Rows[attendIndex]["out_leave_type_id"]);//下班类型
                            if (isValid == 0 && inLeaveType == 1)
                            {

                                ClassOrderTimes bigClassOrderTimes = classTimesList.Where(a => a.ClassOrderName.Trim() == "大").FirstOrDefault();//大班第三次签到时间
                                ClassOrderTimes wellclassOrderTimes = classTimesList.Where(a => a.ClassOrderName.Trim() == attendSign.Trim()).FirstOrDefault();
                                switch (attendSign)
                                {
                                    case "早":
                                        personMonthAttendList[personIndex].ClassOrder[dayOffset] = attendSign;
                                        if (string.IsNullOrEmpty(personMonthAttendList[personIndex].GroupMorningTimes))
                                        {
                                            personMonthAttendList[personIndex].GroupMorningTimes = "1";
                                        }
                                        else
                                        {
                                            personMonthAttendList[personIndex].GroupMorningTimes = (int.Parse(personMonthAttendList[personIndex].GroupMorningTimes) + 1).ToString();
                                        }
                                        inouttimes = 1;
                                        break;
                                    case "中":
                                        personMonthAttendList[personIndex].ClassOrder[dayOffset] = attendSign;
                                        if (string.IsNullOrEmpty(personMonthAttendList[personIndex].GroupMiddleTimes))
                                        {
                                            personMonthAttendList[personIndex].GroupMiddleTimes = "1";
                                        }
                                        else
                                        {
                                            personMonthAttendList[personIndex].GroupMiddleTimes = (int.Parse(personMonthAttendList[personIndex].GroupMiddleTimes) + 1).ToString();
                                        }
                                        inouttimes = 1;
                                        break;
                                    case "夜":
                                        personMonthAttendList[personIndex].ClassOrder[dayOffset] = attendSign;
                                        if (string.IsNullOrEmpty(personMonthAttendList[personIndex].GroupNightTimes))
                                        {
                                            personMonthAttendList[personIndex].GroupNightTimes = "1";
                                        }
                                        else
                                        {
                                            personMonthAttendList[personIndex].GroupNightTimes = (int.Parse(personMonthAttendList[personIndex].GroupNightTimes) + 1).ToString();
                                        }
                                        inouttimes = 1;
                                        break;
                                    case "早1":

                                        if (null != record
                                            && !(record.DownWellTime < wellclassOrderTimes.InWellStartTime && record.UpWellTime < wellclassOrderTimes.InWellStartTime)
                                            && !(record.DownWellTime > wellclassOrderTimes.OutWellEndTime && record.UpWellTime > wellclassOrderTimes.OutWellEndTime) && (outLeaveType == 1 || outLeaveType == 6))
                                        {
                                            inouttimes = 1;
                                            if (record.WellTime >= 480)
                                            {
                                                personMonthAttendList[personIndex].ClassOrder[dayOffset] = attendSign;
                                                if (string.IsNullOrEmpty(personMonthAttendList[personIndex].InWellMorningTimes))
                                                {
                                                    personMonthAttendList[personIndex].InWellMorningTimes = "1";
                                                }
                                                else
                                                {
                                                    personMonthAttendList[personIndex].InWellMorningTimes = (int.Parse(personMonthAttendList[personIndex].InWellMorningTimes) + 1).ToString();
                                                }
                                            }
                                            else if (record.WellTime >= 120 && record.WellTime < 480)
                                            {
                                                personMonthAttendList[personIndex].ClassOrder[dayOffset] = "大1";

                                                if (string.IsNullOrEmpty(personMonthAttendList[personIndex].BigInWellTimes))
                                                {
                                                    personMonthAttendList[personIndex].BigInWellTimes = "1";
                                                }
                                                else
                                                {
                                                    personMonthAttendList[personIndex].BigInWellTimes = (int.Parse(personMonthAttendList[personIndex].BigInWellTimes) + 1).ToString();
                                                }
                                            }
                                            else if (record.WellTime >= 0 && record.WellTime < 120)
                                            {
                                                if (outH > bigClassOrderTimes.InWellStartTime)//如果是在18:20之后
                                                {
                                                    personMonthAttendList[personIndex].ClassOrder[dayOffset] = "大";

                                                    if (string.IsNullOrEmpty(personMonthAttendList[personIndex].GroupBigTimes))
                                                    {
                                                        personMonthAttendList[personIndex].GroupBigTimes = "1";
                                                    }
                                                    else
                                                    {
                                                        personMonthAttendList[personIndex].GroupBigTimes = (int.Parse(personMonthAttendList[personIndex].GroupBigTimes) + 1).ToString();
                                                    }
                                                }
                                                else
                                                {
                                                    personMonthAttendList[personIndex].ClassOrder[dayOffset] = "早";
                                                    if (string.IsNullOrEmpty(personMonthAttendList[personIndex].GroupMorningTimes))
                                                    {
                                                        personMonthAttendList[personIndex].GroupMorningTimes = "1";
                                                    }
                                                    else
                                                    {
                                                        personMonthAttendList[personIndex].GroupMorningTimes = (int.Parse(personMonthAttendList[personIndex].GroupMorningTimes) + 1).ToString();
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    case "中1":
                                        if (null != record && record.WellTime >= 480 && (outLeaveType == 1 || outLeaveType == 6))
                                        {
                                            personMonthAttendList[personIndex].ClassOrder[dayOffset] = attendSign;
                                            if (string.IsNullOrEmpty(personMonthAttendList[personIndex].InWellMiddleTimes))
                                            {
                                                personMonthAttendList[personIndex].InWellMiddleTimes = "1";
                                            }
                                            else
                                            {
                                                personMonthAttendList[personIndex].InWellMiddleTimes = (int.Parse(personMonthAttendList[personIndex].InWellMiddleTimes) + 1).ToString();
                                            }
                                            inouttimes = 1;
                                        }
                                        break;
                                    case "夜1":
                                        if (null != record && record.WellTime >= 480 && (outLeaveType == 1 || outLeaveType == 6))
                                        {
                                            personMonthAttendList[personIndex].ClassOrder[dayOffset] = attendSign;
                                            if (string.IsNullOrEmpty(personMonthAttendList[personIndex].InWellNightTimes))
                                            {
                                                personMonthAttendList[personIndex].InWellNightTimes = "1";
                                            }
                                            else
                                            {
                                                personMonthAttendList[personIndex].InWellNightTimes = (int.Parse(personMonthAttendList[personIndex].InWellNightTimes) + 1).ToString();
                                            }
                                            inouttimes = 1;
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                        #endregion

                        }
                    }
                    else
                    {
                        #region 工作时间为0的大值

                        if (!isNormal)
                        {
                            //考勤签到
                            string[] signTimes = dt.Rows[attendIndex]["recog_sign_time"].ToString().Split(new char[] { ',' });
                            //大班值班，*代表没有签到记录
                            if (signTimes[3] != "*" && signTimes[0] != "*")
                            {
                                record = monthCheckRecordList.Where(a => a.PepoleExNumber == personMonthAttendList[personIndex].WorkSn && a.DownWellDay == attendDay.ToString("yyyyMMdd")).OrderByDescending(a => a.WellTime).FirstOrDefault();//定位数据
                                DateTime nextDay = DateTime.Parse(dt.Rows[attendIndex + 1]["attend_day"].ToString());//获取第二天
                                string[] NextSignTimes = dt.Rows[attendIndex + 1]["recog_sign_time"].ToString().Split(new char[] { ',' }); ;//第二天签到时间
                                //有下井和无下井的情况
                                if (((null != record && record.WellTime > 0) || (null == record && signTimes[1] != "*"))
                                    && (nextDay - attendDay).Days == 1 && !(signTimes[2] == "*" && signTimes[3] == "*") && NextSignTimes[0] != "*")
                                {
                                    personMonthAttendList[personIndex].ClassOrder[dayOffset] = "大值";
                                    if (string.IsNullOrEmpty(personMonthAttendList[personIndex].BigDutyTimes))
                                    {
                                        personMonthAttendList[personIndex].BigDutyTimes = "1";
                                    }
                                    else
                                    {
                                        personMonthAttendList[personIndex].BigDutyTimes = (int.Parse(personMonthAttendList[personIndex].BigDutyTimes) + 1).ToString();
                                    }
                                    inouttimes = 1;
                                    if (signTimes[2] != "*")
                                    {
                                        worktimes = (DateTime.Parse(signTimes[2]) - DateTime.Parse(signTimes[0])).Hours * 60 + (DateTime.Parse(signTimes[2]) - DateTime.Parse(signTimes[0])).Minutes;
                                    }
                                    else
                                    {
                                        worktimes = (DateTime.Parse(signTimes[3]) - DateTime.Parse(signTimes[0])).Hours * 60 + (DateTime.Parse(signTimes[3]) - DateTime.Parse(signTimes[0])).Minutes;
                                    }
                                    personMonthAttendList[personIndex].GroupTimes[dayOffset] = worktimes.ToString();//只要有工作时间就不是旷工
                                }
                            }

                        }
                        #endregion
                    }

                    #region 统计加班与工时
                    if (inouttimes > 0)
                    {
                        //计算正常工时
                        if (!overClass)
                        {
                            personMonthAttendList[personIndex].InOutTimes += inouttimes;
                        }
                        else
                        {
                            //计算加班
                            if (personMonthAttendList[personIndex].OverTimes == "")
                            {
                                personMonthAttendList[personIndex].OverTimes = inouttimes.ToString();

                            }
                            else
                            {
                                personMonthAttendList[personIndex].OverTimes = (Convert.ToInt32(personMonthAttendList[personIndex].OverTimes) + inouttimes).ToString();
                            }
                        }
                        if (null != record)
                        {
                            //计算井下时长
                            if (personMonthAttendList[personIndex].WellTotalTimes == "")
                            {
                                personMonthAttendList[personIndex].WellTotalTimes = record.WellTime.ToString();
                            }
                            else
                            {
                                personMonthAttendList[personIndex].WellTotalTimes = (Convert.ToInt32(personMonthAttendList[personIndex].WellTotalTimes) + record.WellTime).ToString();
                            }
                        }
                        //计算出勤时长
                        if (personMonthAttendList[personIndex].WorkTotalTimes == "")
                        {
                            personMonthAttendList[personIndex].WorkTotalTimes = worktimes.ToString();
                        }
                        else
                        {
                            personMonthAttendList[personIndex].WorkTotalTimes = (Convert.ToInt32(personMonthAttendList[personIndex].WorkTotalTimes) + worktimes).ToString();
                        }
                    }

                    #endregion

                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(ex.Message);
                }
            }
        }
        #endregion

        #region 查询请假数据
        /// <summary>
        /// 查询请假数据
        /// </summary>
        /// <param name="PersonMonthAttendList"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        public void GetLeaveAttendDetail(List<PersonMonthAttend> PersonMonthAttendList, DateTime beginTime, DateTime endTime, List<DateTime> festivalDate, string[] departName, string[] personName)
        {
            #region 拼接sql
            StringBuilder strSql = new StringBuilder();
            string sql_where = "";
            if (departName != null && departName.Count() > 0)
            {
                sql_where = string.Format(" and depart_name in ( ");
                for (int index = 0; index < departName.Count(); index++)
                {
                    sql_where += string.Format(" '{0}', ", departName[index]);
                }

                sql_where = sql_where.Remove(sql_where.LastIndexOf(","), 1);
                sql_where += ")";
            }

            if (personName != null && personName.Count() > 0)
            {
                sql_where += string.Format(" and name in ( ");
                for (int index = 0; index < personName.Count(); index++)
                {
                    sql_where += string.Format(" '{0}', ", personName[index]);
                }

                sql_where = sql_where.Remove(sql_where.LastIndexOf(","), 1);
                sql_where += ")";
            }

            strSql.Append("SELECT  p.person_id, leave_start_time,leave_end_time,t.attend_sign leave_type_name,");
            strSql.AppendFormat("EXTRACT(EPOCH FROM date_trunc('day',leave_start_time)-'{0}')/(24*3600) as day_offset,", beginTime);
            strSql.AppendFormat("EXTRACT(EPOCH FROM date_trunc('day',leave_end_time)-leave_start_time)/(24*3600)+1 as days ", endTime);
            strSql.Append("FROM attend_for_leave a left join leave_type t on a.leave_type_id=t.leave_type_id left join person p on p.person_id = a.person_id ");
            strSql.AppendFormat("where ((leave_start_time >= '{0}' and leave_start_time <= '{1}') or (leave_end_time >= '{0}' and leave_end_time <= '{1}')) {2} order by convert_to(depart_name, 'GBK'),convert_to(name, 'GBK') ,day_offset", beginTime, endTime, sql_where);
            #endregion

            DataTable dt = DbAccess.POSTGRESQL.Select(strSql.ToString(), "attend_for_leave");
            if (null != dt && dt.Rows.Count > 0)
            {
                int personIndex = 0;
                int dayOffset = 0;
                DateTime leaveStartTime = DateTime.Now;//考勤归属日
                string leveTypeName = string.Empty;//请假类型

                for (int attendIndex = 0; attendIndex < dt.Rows.Count; attendIndex++)
                {
                    while (personIndex < PersonMonthAttendList.Count() && PersonMonthAttendList[personIndex].PersonId != Convert.ToInt32(dt.Rows[attendIndex]["person_id"]))
                    {
                        personIndex++;
                    }
                    if (personIndex >= PersonMonthAttendList.Count())
                    {
                        return;
                    }
                    leaveStartTime = DateTime.Parse(dt.Rows[attendIndex]["leave_start_time"].ToString());
                    leveTypeName = dt.Rows[attendIndex]["leave_type_name"].ToString();
                    int days = int.Parse(dt.Rows[attendIndex]["days"].ToString());//请假天数
                    int realDays = 0;//实际天数，减去周末与节假日
                    dayOffset = int.Parse(dt.Rows[attendIndex]["day_offset"].ToString());
                    for (int i = 0; i < days; i++)
                    {
                        if (!festivalDate.Contains(beginTime.AddDays(dayOffset + i)))
                        {
                            if (beginTime.AddDays(dayOffset + i) <= endTime && beginTime.AddDays(dayOffset + i)>=beginTime)
                            {
                                if ((dayOffset + i)<31)
                                {
                                    if (string.IsNullOrEmpty(PersonMonthAttendList[personIndex].ClassOrder[dayOffset + i]))
                                    {
                                        PersonMonthAttendList[personIndex].ClassOrder[dayOffset + i] = leveTypeName;
                                        realDays++;
                                    }
                                }
                            }
                        }
                    }
                    #region 匹配请假类型
                    switch (leveTypeName)
                    {
                        case "病":
                            if (string.IsNullOrEmpty(PersonMonthAttendList[personIndex].SickLeaveTimes))
                            {
                                PersonMonthAttendList[personIndex].SickLeaveTimes = realDays.ToString();
                            }
                            else
                            {
                                PersonMonthAttendList[personIndex].SickLeaveTimes = (int.Parse(PersonMonthAttendList[personIndex].SickLeaveTimes) + realDays).ToString();
                            }
                            break;
                        case "公":
                            if (string.IsNullOrEmpty(PersonMonthAttendList[personIndex].BigClassOutTimes))
                            {
                                PersonMonthAttendList[personIndex].BigClassOutTimes = realDays.ToString();
                            }
                            else
                            {
                                PersonMonthAttendList[personIndex].BigClassOutTimes = (int.Parse(PersonMonthAttendList[personIndex].BigClassOutTimes) + realDays).ToString();
                            }
                            break;
                        case "婚":
                        case "丧":
                        case "探":
                            if (string.IsNullOrEmpty(PersonMonthAttendList[personIndex].WeddingsTimes))
                            {
                                PersonMonthAttendList[personIndex].WeddingsTimes = realDays.ToString();
                            }
                            else
                            {
                                PersonMonthAttendList[personIndex].WeddingsTimes = (int.Parse(PersonMonthAttendList[personIndex].WeddingsTimes) + realDays).ToString();
                            }
                            break;
                        case "工":
                            if (string.IsNullOrEmpty(PersonMonthAttendList[personIndex].FakeInjuryTimes))
                            {
                                PersonMonthAttendList[personIndex].FakeInjuryTimes = realDays.ToString();
                            }
                            else
                            {
                                PersonMonthAttendList[personIndex].FakeInjuryTimes = (int.Parse(PersonMonthAttendList[personIndex].FakeInjuryTimes) + realDays).ToString();
                            }
                            break;
                        case "事":
                            if (string.IsNullOrEmpty(PersonMonthAttendList[personIndex].LeaveTimes))
                            {
                                PersonMonthAttendList[personIndex].LeaveTimes = realDays.ToString();
                            }
                            else
                            {
                                PersonMonthAttendList[personIndex].LeaveTimes = (int.Parse(PersonMonthAttendList[personIndex].LeaveTimes) + realDays).ToString();
                            }
                            break;
                        case "产":
                            if (string.IsNullOrEmpty(PersonMonthAttendList[personIndex].MaternityLeaveTimes))
                            {
                                PersonMonthAttendList[personIndex].MaternityLeaveTimes = realDays.ToString();
                            }
                            else
                            {
                                PersonMonthAttendList[personIndex].MaternityLeaveTimes = (int.Parse(PersonMonthAttendList[personIndex].MaternityLeaveTimes) + realDays).ToString();
                            }
                            break;
                        case "育":
                            if (string.IsNullOrEmpty(PersonMonthAttendList[personIndex].ParentalLeaveTimes))
                            {
                                PersonMonthAttendList[personIndex].ParentalLeaveTimes = realDays.ToString();
                            }
                            else
                            {
                                PersonMonthAttendList[personIndex].ParentalLeaveTimes = (int.Parse(PersonMonthAttendList[personIndex].ParentalLeaveTimes) + realDays).ToString();
                            }
                            break;
                        case "年":
                            if (string.IsNullOrEmpty(PersonMonthAttendList[personIndex].AnnualLeaveTimes))
                            {
                                PersonMonthAttendList[personIndex].AnnualLeaveTimes = realDays.ToString();
                            }
                            else
                            {
                                PersonMonthAttendList[personIndex].AnnualLeaveTimes = (int.Parse(PersonMonthAttendList[personIndex].AnnualLeaveTimes) + realDays).ToString();
                            }
                            break;
                        case "学":
                            if (string.IsNullOrEmpty(PersonMonthAttendList[personIndex].StudyDays))
                            {
                                PersonMonthAttendList[personIndex].StudyDays = realDays.ToString();
                            }
                            else
                            {
                                PersonMonthAttendList[personIndex].StudyDays = (int.Parse(PersonMonthAttendList[personIndex].StudyDays) + realDays).ToString();
                            }
                            break;
                        default:
                            break;
                    }
                    #endregion
                }
            }
        }

        #endregion

        #region 计算旷工
        /// <summary>
        /// 计算旷工
        /// </summary>
        /// <param name="personMonthAttendList"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="festivalDate"></param>
        /// <param name="shiftDate"></param>
        public void GetAbsenteeismDays(List<PersonMonthAttend> personMonthAttendList, DateTime beginTime, DateTime endTime, List<DateTime> festivalDate)
        {
            int days = 0;
            for (int i = 0; i < personMonthAttendList.Count; i++)
            {
                days = 0;
                for (int j = 0; j < personMonthAttendList[i].ClassOrder.Length; j++)
                {
                    if (personMonthAttendList[i].ClassOrder[j] == "*")
                    {
                        personMonthAttendList[i].ClassOrder[j] = string.Empty;
                        continue;
                    }
                    //排除周末与节假日
                    if (!festivalDate.Contains(beginTime.AddDays(j)))
                    {
                        if (string.IsNullOrEmpty(personMonthAttendList[i].ClassOrder[j]) && string.IsNullOrEmpty(personMonthAttendList[i].GroupTimes[j]))
                        {
                            personMonthAttendList[i].ClassOrder[j] = "旷";
                            days++;
                        }
                    }
                    if (days > 0)
                    {
                        personMonthAttendList[i].AbsenteeismTimes = days.ToString();
                    }
                }

                if (!string.IsNullOrEmpty(personMonthAttendList[i].WellTotalTimes))
                {
                    int minutes = int.Parse(personMonthAttendList[i].WellTotalTimes) % 60;
                    if (minutes > 0)
                    {
                        personMonthAttendList[i].WellTotalTimes = int.Parse(personMonthAttendList[i].WellTotalTimes) / 60 + ":" + (minutes > 10 ? minutes.ToString() : "0" + minutes.ToString());
                    }
                    else
                    {
                        personMonthAttendList[i].WellTotalTimes = (int.Parse(personMonthAttendList[i].WellTotalTimes) / 60).ToString();
                    }

                }

                if (!string.IsNullOrEmpty(personMonthAttendList[i].WorkTotalTimes))
                {
                    int minutes = int.Parse(personMonthAttendList[i].WorkTotalTimes) % 60;
                    if (minutes > 0)
                    {
                        personMonthAttendList[i].WorkTotalTimes = int.Parse(personMonthAttendList[i].WorkTotalTimes) / 60 + ":" + (minutes > 10 ? minutes.ToString() : "0" + minutes.ToString());
                    }
                    else
                    {
                        personMonthAttendList[i].WorkTotalTimes = (int.Parse(personMonthAttendList[i].WorkTotalTimes) / 60).ToString();
                    }
                }
            }
        }

        #endregion

        #region 查询井下定位数据
        /// <summary>
        /// 查询井下定位数据
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public List<MonthCheckRecord> GetMonthCheckRecordList(List<PersonMonthAttend> personMonthAttendList, DateTime beginTime, DateTime endTime)
        {
            List<MonthCheckRecord> list = new List<MonthCheckRecord>();
            if (personMonthAttendList.Count > 0)
            {
                #region 拼接SQL语句
                StringBuilder strWorkSn = new StringBuilder();
                foreach (PersonMonthAttend item in personMonthAttendList)
                {
                    if (!string.IsNullOrEmpty(item.WorkSn))
                    {
                        strWorkSn.Append(item.WorkSn + ",");
                    }

                }
                strWorkSn.Append("-1");
                StringBuilder strSql = new StringBuilder();
                strSql.AppendFormat("declare @Temp_Array varchar(max) set @Temp_Array='{0}' ", strWorkSn);
                strSql.Append("declare @Temp_Variable varchar(max) ");
                strSql.Append("declare @Temp_Table table (Item varchar(max)) ");
                strSql.Append("while(LEN(@Temp_Array) > 0) ");
                strSql.Append("begin if(CHARINDEX(',',@Temp_Array) = 0) ");
                strSql.Append("begin ");
                strSql.Append("set @Temp_Variable = @Temp_Array ");
                strSql.Append("set @Temp_Array = ''");
                strSql.Append("end ");
                strSql.Append("else ");
                strSql.Append("begin ");
                strSql.Append("set @Temp_Variable = LEFT(@Temp_Array,CHARINDEX(',',@Temp_Array)-1) ");
                strSql.Append("set @Temp_Array = RIGHT(@Temp_Array,LEN(@Temp_Array)-LEN(@Temp_Variable)-1) ");
                strSql.Append("end ");
                strSql.Append("insert into @Temp_Table(Item) values(@Temp_Variable) ");
                strSql.Append("end ");
                strSql.Append("select LTRIM(RTRIM(p.PepoleExNumber)) PepoleExNumber,p.PepoleName,CONVERT(char(8), DownWellTime,112) DownWellDay");
                strSql.Append(",ISNULL(DATEPART(HH,DownWellTime)*60+DATEPART(MI,DownWellTime),-1) DownWellTime");
                strSql.Append(",ISNULL(DATEPART(HH,UpWellTime)*60+DATEPART(MI,UpWellTime),-1)+ISNULL(datediff(DAY,DownWellTime,UpWellTime),-1)*1440 UpWellTime ");
                strSql.Append(",ISNULL(datediff(MI,DownWellTime,UpWellTime),-1) WellTime from (");

                int months = endTime.AddDays(-1).Month - beginTime.Month;
                string tableName = string.Empty;
                string strCon = System.Web.Configuration.WebConfigurationManager.AppSettings["SqlServerConnectionStr"].ToString();
                SqlControl sqlControl = new SqlControl(strCon);
                sqlControl.Open();
                StringBuilder strTab = new StringBuilder();
                for (int i = 0; i <= months; i++)
                {
                    tableName = string.Format("MonthCheckRecord{0}{1}", beginTime.AddMonths(i).Year, beginTime.AddMonths(i).Month > 9 ? beginTime.AddMonths(i).Month.ToString() : "0" + beginTime.AddMonths(i).Month.ToString());
                    if (CheckTableName(tableName, sqlControl))
                    {
                        strTab.AppendFormat(" SELECT PepoleNumber,DownWellTime,UpWellTime FROM {0} union", tableName);
                    }

                }
                #endregion

                if (!string.IsNullOrEmpty(strTab.ToString()))
                {
                    strSql.Append(strTab.ToString().Substring(0, strTab.ToString().Length - 5) + ") a ");
                    strSql.Append("inner join StaffInformation p on a.PepoleNumber =p.PepoleNumber ");
                    strSql.Append("where DownWellTime is not null and UpWellTime is not null  and LTRIM(RTRIM(p.PepoleExNumber)) in (select Item from @Temp_Table) order by PepoleExNumber desc ");
                    System.Diagnostics.Trace.WriteLine(strSql.ToString());
                    DataTable dt = sqlControl.SelectDT(strSql.ToString(), "MonthCheckRecord");

                    if (null != dt && dt.Rows.Count > 0)
                    {

                        foreach (DataRow dr in dt.Rows)
                        {
                            list.Add(new MonthCheckRecord()
                            {
                                DownWellDay = dr["DownWellDay"].ToString(),
                                PepoleExNumber = dr["PepoleExNumber"].ToString(),
                                DownWellTime = int.Parse(dr["DownWellTime"].ToString()),
                                UpWellTime = int.Parse(dr["UpWellTime"].ToString()),
                                WellTime = int.Parse(dr["WellTime"].ToString())
                            });
                        }
                    }
                }
                sqlControl.CloseConnect();
            }
            return list;
        }

        #endregion

        #region 判断表是否存在
        /// <summary>
        /// 判断表是否存在
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="sqlControl"></param>
        /// <returns></returns>
        public bool CheckTableName(string TableName, SqlControl sqlControl)
        {
            string sql = string.Format(@"SELECT  COUNT(*)  FROM [sys].[tables] where [name]='{0}' ", TableName);
            DataTable dt = sqlControl.SelectDT(sql, "tables");

            return dt.Rows[0][0].ToString() == "1";
        }

        #endregion

        #region 按制度工作日计算月考勤
        /// <summary>
        /// 按制度工作日计算月考勤
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="departName"></param>
        /// <param name="personName"></param>
        /// <param name="ruleDataNum"></param>
        /// <returns></returns>
        public IEnumerable<PersonMonthAttend> GetMonthAttendUnderRuleCollect(DateTime beginTime, DateTime endTime, string[] departName, string[] personName, int ruleDataNum)
        {
            PersonMonthAttendList = (List<PersonMonthAttend>)GetPersonMonthAttendList(beginTime, endTime, departName, personName);
            if (ruleDataNum <= 0)
            {
                return PersonMonthAttendList;
            }

            for (int attendIndex = 0; attendIndex < PersonMonthAttendList.Count(); attendIndex++)
            {
                if (PersonMonthAttendList[attendIndex].InOutTimes <= ruleDataNum)
                {
                    continue;
                }
                /*地面大班工数(大)GroupBigTimes>地面早班工数（早）GroupMorningTimes>地面中班工数（中）GroupMiddleTimes>地面夜班工数（夜）GroupNightTimes
                >大班入井工数（大1）BigInWellTimes>井下早班工数（早1）InWellMorningTimes>井下中班工数（中1）InWellMiddleTimes>井下夜班工数（夜1）InWellNightTimes*/
                int overDays =(int) (PersonMonthAttendList[attendIndex].InOutTimes - ruleDataNum);
                PersonMonthAttendList[attendIndex].InOutTimes = ruleDataNum;
                int totalSubDays = overDays;
                //每个班次要减去的工数
                int[] classSubDays = new int[8];
                //每个班次的工数
                int[] classDays = new int[8];
                classDays[0] = Convert.ToInt32(PersonMonthAttendList[attendIndex].InWellNightTimes);
                classDays[1] = Convert.ToInt32(PersonMonthAttendList[attendIndex].InWellMiddleTimes);
                classDays[2] = Convert.ToInt32(PersonMonthAttendList[attendIndex].InWellMorningTimes);
                classDays[3] = Convert.ToInt32(PersonMonthAttendList[attendIndex].BigInWellTimes);
                classDays[4] = Convert.ToInt32(PersonMonthAttendList[attendIndex].GroupNightTimes);
                classDays[5] = Convert.ToInt32(PersonMonthAttendList[attendIndex].GroupMiddleTimes);
                classDays[6] = Convert.ToInt32(PersonMonthAttendList[attendIndex].GroupMorningTimes);
                classDays[7] = Convert.ToInt32(PersonMonthAttendList[attendIndex].GroupBigTimes);

                //班次简称
                string[] classSign = new string[8] { "夜1", "中1", "早1", "大1", "夜", "中", "早", "大" };

                //计算每个班次应该减去的工数
                for (int index = 0; index < 8; index++)
                {
                    if (totalSubDays <= 0)
                    {
                        break;
                    }

                    if (classDays[index] >= totalSubDays)
                    {
                        classSubDays[index] = totalSubDays;
                    }
                    else
                    {
                        classSubDays[index] = classDays[index];
                    }
                    totalSubDays -= classSubDays[index];
                    classDays[index] -= classSubDays[index];
                }

                PersonMonthAttendList[attendIndex].InWellNightTimes = classDays[0].ToString();
                PersonMonthAttendList[attendIndex].InWellMiddleTimes = classDays[1].ToString();
                PersonMonthAttendList[attendIndex].InWellMorningTimes = classDays[2].ToString();
                PersonMonthAttendList[attendIndex].BigInWellTimes = classDays[3].ToString();
                PersonMonthAttendList[attendIndex].GroupNightTimes = classDays[4].ToString();
                PersonMonthAttendList[attendIndex].GroupMiddleTimes = classDays[5].ToString();
                PersonMonthAttendList[attendIndex].GroupMorningTimes = classDays[6].ToString();
                PersonMonthAttendList[attendIndex].GroupBigTimes = classDays[7].ToString();

                //将31天中应该减去的班次置为空
                for (int dayIndex = 0; dayIndex < PersonMonthAttendList[attendIndex].ClassOrder.Count(); dayIndex++)
                {
                    if (PersonMonthAttendList[attendIndex].ClassOrder[dayIndex] == null || PersonMonthAttendList[attendIndex].ClassOrder[dayIndex] == "")
                    {
                        continue;
                    }

                    for (int classIndex = 0; classIndex < 8; classIndex++)
                    {
                        if (classSubDays[classIndex] != 0 && PersonMonthAttendList[attendIndex].ClassOrder[dayIndex].CompareTo(classSign[classIndex]) == 0)
                        {
                            PersonMonthAttendList[attendIndex].ClassOrder[dayIndex] = "";
                            classSubDays[classIndex]--;
                        }
                    }
                }
            }

            return PersonMonthAttendList;
        }

        #endregion
    }

    #region 硫磺沟月考勤报表数据类

    /// <summary>
    /// 硫磺沟月考勤报表
    /// </summary>
    [DataContract]
    public class PersonMonthAttend
    {
        /// <summary>
        /// 序号
        /// </summary>
        [DataMember]
        [Key]
        public int Index { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        [DataMember]
        public string DepartName { get; set; }

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
        /// 内部关联号
        /// </summary>
        [DataMember]
        public string WorkSn { get; set; }

        /// <summary>
        /// 出勤工数
        /// </summary>
        [DataMember]
        public float InOutTimes { get; set; }

        /// <summary>
        /// 加班天数
        /// </summary>
        [DataMember]
        public string OverTimes { get; set; }

        /// <summary>
        /// 大班入井工数
        /// </summary>
        [DataMember]
        public string BigInWellTimes { get; set; }

        /// <summary>
        /// 大班公出工数
        /// </summary>
        [DataMember]
        public string BigClassOutTimes { get; set; }

        /// <summary>
        /// 大班值班工数
        /// </summary>
        [DataMember]
        public string BigDutyTimes { get; set; }

        /// <summary>
        /// 小班入井工数
        /// </summary>
        [DataMember]
        public string SmallInWellTimes { get; set; }

        /// <summary>
        /// 地面大班工数
        /// </summary>
        [DataMember]
        public string GroupBigTimes { get; set; }

        /// <summary>
        /// 地面早班工数
        /// </summary>
        [DataMember]
        public string GroupMorningTimes { get; set; }

        /// <summary>
        /// 虹膜考勤时间
        /// </summary>
        [DataMember]
        public string[] GroupTimes { get; set; }

        /// <summary>
        /// 地面中班工数
        /// </summary>
        [DataMember]
        public string GroupMiddleTimes { get; set; }

        /// <summary>
        /// 地面夜班工数
        /// </summary>
        [DataMember]
        public string GroupNightTimes { get; set; }

        /// <summary>
        /// 井下早班工数
        /// </summary>
        [DataMember]
        public string InWellMorningTimes { get; set; }

        /// <summary>
        /// 井下中班工数
        /// </summary>
        [DataMember]
        public string InWellMiddleTimes { get; set; }

        /// <summary>
        /// 井下夜班工数
        /// </summary>
        [DataMember]
        public string InWellNightTimes { get; set; }

        /// <summary>
        /// 婚丧探天数
        /// </summary>
        [DataMember]
        public string WeddingsTimes { get; set; }

        /// <summary>
        /// 年休天数
        /// </summary>
        [DataMember]
        public string AnnualLeaveTimes { get; set; }

        /// <summary>
        /// 待岗学习天数
        /// </summary>
        [DataMember]
        public string StudyDays { get; set; }

        /// <summary>
        /// 病假天数
        /// </summary>
        [DataMember]
        public string SickLeaveTimes { get; set; }

        /// <summary>
        /// 伤假天数
        /// </summary>
        [DataMember]
        public string FakeInjuryTimes { get; set; }

        /// <summary>
        /// 事假天数
        /// </summary>
        [DataMember]
        public string LeaveTimes { get; set; }

        /// <summary>
        /// 旷工天数
        /// </summary>
        [DataMember]
        public string AbsenteeismTimes { get; set; }

        /// <summary>
        /// 产假天数
        /// </summary>
        [DataMember]
        public string MaternityLeaveTimes { get; set; }

        /// <summary>
        /// 育儿假天数
        /// </summary>
        [DataMember]
        public string ParentalLeaveTimes { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [DataMember]
        public string Remark { get; set; }

        /// <summary>
        /// 出勤明细
        /// </summary> 
        [DataMember]
        public string[] ClassOrder { get; set; }

        /// <summary>
        /// 出勤总时长
        /// </summary> 
        [DataMember]
        public string WorkTotalTimes { get; set; }

        /// <summary>
        /// 下井总时长
        /// </summary> 
        [DataMember]
        public string WellTotalTimes { get; set; }
    }
    /// <summary>
    /// 人员井下定位数据
    /// </summary>
    [DataContract]
    public class MonthCheckRecord
    {
        /// <summary>
        /// 工号
        /// </summary> 
        [DataMember]
        public string PepoleExNumber { get; set; }
        /// <summary>
        /// 考勤日期
        /// </summary> 
        [DataMember]
        public string DownWellDay { get; set; }
        /// <summary>
        /// 下井时间
        /// </summary> 
        [DataMember]
        public int DownWellTime { get; set; }
        /// <summary>
        /// 上井时间
        /// </summary> 
        [DataMember]
        public int UpWellTime { get; set; }
        /// <summary>
        /// 井下时间
        /// </summary> 
        [DataMember]
        public int WellTime { get; set; }

    }

    /// <summary>
    /// 正常班次上下班时间
    /// </summary>
    public class ClassOrderTimes
    {
        /// <summary>
        /// 班次名称
        /// </summary>
        public string ClassOrderName { get; set; }
        /// <summary>
        /// 上班开始时间
        /// </summary>
        public int InWellStartTime { get; set; }
        /// <summary>
        /// 上班结束时间
        /// </summary>
        public int InWellEndTime { get; set; }
        /// <summary>
        /// 下班开始时间
        /// </summary>
        public int OutWellStartTime { get; set; }
        /// <summary>
        /// 下班结束时间
        /// </summary>
        public int OutWellEndTime { get; set; }
    }

    #endregion
}