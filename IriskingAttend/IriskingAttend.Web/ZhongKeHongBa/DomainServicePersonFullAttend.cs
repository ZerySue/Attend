/*************************************************************************
** 文件名:   PersonFullAttend.cs
×× 主要类:   PersonFullAttend
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   ty
** 日  期:   2014-4-22
** 修改人:   
** 日  期:    
** 描  述:   DomainServiceIriskingAttend类,后台数据库操作  添加修改删除考勤类型函数
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
    using System.Drawing;
    using System.Web;
    using System.Windows.Forms;
    using System.Diagnostics;
    using System.Runtime.Serialization;
    using System.Text;


    // TODO: 创建包含应用程序逻辑的方法。
    public partial class DomainServiceIriskingAttend
    {
        #region 获取一段时间工作日的天数 除了周六日 法定节假日 还要算上调休的时间 
        [Query(HasSideEffects = true)]
        public IEnumerable<ZKHBMonthList> GetDayList(DateTime beginTime, DateTime endTime)
        {
            //组装sql语句的where条件          
            string sql_where = "";
            if (beginTime != null && endTime != null)
            {
                sql_where += string.Format("  (begin_time, end_time) OVERLAPS( ");
                sql_where += string.Format(" '{0}'", beginTime);
                sql_where += string.Format(" :: timestamp,");
                sql_where += string.Format(" '{0}'", endTime);
                sql_where += string.Format("::timestamp)");
            }

            string sql_where_work = "";
            if (beginTime != null && endTime != null)
            {
                sql_where_work += string.Format("  (begin_time, end_time) OVERLAPS( ");
                sql_where_work += string.Format(" '{0}'", beginTime);
                sql_where_work += string.Format(" :: timestamp,");
                sql_where_work += string.Format(" '{0}'", endTime);
                sql_where_work += string.Format("::timestamp)");
            }
             string sql_all = string.Format(@"SELECT begin_time, end_time from festival where {0}",sql_where);
             string sql_work = string.Format(@"SELECT begin_time, end_time from weekend_to_work where {0}", sql_where_work);
             List<ZKHBMonthList> query = new List<ZKHBMonthList>();
             query.Clear();
             DataTable dt = DbAccess.POSTGRESQL.Select(sql_all, "festival");
             DataTable dt_work = DbAccess.POSTGRESQL.Select(sql_work, "weekend_to_work");
            
             TimeSpan ts1 = endTime.Subtract(beginTime);//TimeSpan得到dt1和dt2的时间间隔
             int countday = ts1.Days;//获取两个日期间的总天数 
            //循环用来扣除总天数中的双休日
             for (int i = 0; i < countday; i++)
             {
                 DateTime tempdt = beginTime.Date.AddDays(i);
                 //tempdt.DayOfWeek 
                 ZKHBMonthList ZKHBMonthList = new ZKHBMonthList();
                 ZKHBMonthList.Index = i;
                 switch (tempdt.DayOfWeek)
                 {
                     case DayOfWeek.Monday:
                          ZKHBMonthList.Flag = 1;
                         break;
                     case DayOfWeek.Tuesday:
                         ZKHBMonthList.Flag = 2;
                         break;
                     case DayOfWeek.Wednesday:
                          ZKHBMonthList.Flag = 3;
                         break;
                     case DayOfWeek.Thursday:
                         ZKHBMonthList.Flag = 4;
                         break;
                     case DayOfWeek.Friday:
                         ZKHBMonthList.Flag = 5;
                         break;
                     case DayOfWeek.Saturday:
                         ZKHBMonthList.Flag = 6;
                         break;
                     case DayOfWeek.Sunday:
                         ZKHBMonthList.Flag = 0;
                         break;

                 }

                 if (null != dt && dt.Rows.Count > 0)
                 {
                     foreach (DataRow ar in dt.Rows)//节假日
                     {
                         DateTime begintime = Convert.ToDateTime(ar["begin_time"]);
                         DateTime endtime = Convert.ToDateTime(ar["end_time"]);
                         if (tempdt >= begintime && tempdt < endtime)
                         {
                             ZKHBMonthList.Flag = 7;
                         }
                     }
                 }
                 if (null != dt_work && dt_work.Rows.Count > 0)
                 {
                     foreach (DataRow ar in dt_work.Rows)//调休应该上班的
                     {
                         DateTime begintime = Convert.ToDateTime(ar["begin_time"]);
                         DateTime endtime = Convert.ToDateTime(ar["end_time"]);
                         if (tempdt >= begintime && tempdt < endtime)
                         {
                             ZKHBMonthList.Flag = 1;
                         }
                     }
                 }
                query.Add(ZKHBMonthList);
             }
             return query;
        }
        #endregion

        #region 获取连个时间段之间相差天数（出去周六日）

        private int WorkDays(DateTime beginTime, DateTime endTime)
        {
            TimeSpan ts1 =  endTime.Subtract(beginTime);//TimeSpan得到dt1和dt2的时间间隔
            int countday = ts1.Days;//获取两个日期间的总天数 
            int weekdays = 0;//工作日
            //循环用来扣除总天数中的双休日
            for (int i = 0; i < countday; i++)
            {
                DateTime tempdt = beginTime.Date.AddDays(i);
                if (tempdt.DayOfWeek != System.DayOfWeek.Saturday && tempdt.DayOfWeek != System.DayOfWeek.Sunday)
                {
                    weekdays++;
                }
            }
            return weekdays;
        }

        #endregion

        #region 中科虹霸全勤人员统计表
        /// <summary>        
        /// 获取人员信息表
        /// </summary>
        /// <param name="departName">查询条件 部门名称</param>    
        /// <param name="personName">查询条件 人员名称</param>    
        /// <returns>人员信息表</returns>
        [Query(HasSideEffects = true)]
        public IEnumerable<PersonFullAttendInfo> GetFullPersonList(DateTime beginTime, DateTime endTime, string[] departName, string[] personName)
        {
            //int monthworkday = WorkDays(beginTime, endTime);
            int monthworkday = 0;
            List<ZKHBMonthList> ZKHBMonthListquery = (List<ZKHBMonthList>)GetDayList(beginTime, endTime);
            if (ZKHBMonthListquery != null && ZKHBMonthListquery.Count() > 0)
            {
                foreach (ZKHBMonthList item in ZKHBMonthListquery)
                {
                    if (item.Flag > 0 && item.Flag < 6)
                    {
                        monthworkday += 1;
                    }
                }
            }
            //组装sql语句的where条件          
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
            if(beginTime != null && endTime != null)
            {
                sql_where += string.Format(" and attend_day >=");
                sql_where += string.Format(" '{0}'", beginTime);
                sql_where += string.Format(" and attend_day <");
                sql_where += string.Format(" '{0}'", endTime);

            }
            string sql_all = string.Format(@"select * from (
select count(*) count, person_id, name, depart_name, work_sn from (
select * from (
select (SELECT count(*) FROM festival f WHERE f.begin_time <= a.attend_day AND a.attend_day <= f.end_time) weekend,
(select count(*) from weekend_to_work w where w.begin_time <= a.attend_day AND a.attend_day <= w.end_time) works,
a.person_id, name, depart_name, work_sn,attend_day 
from attend_record_base a 
left join person p on p.person_id = a.person_id
                where p.delete_time is null and is_valid >=0 {0} 
               )T1 
 where (((EXTRACT(DOW from attend_day::timestamp )>0 and EXTRACT(DOW from attend_day::timestamp )<6) and weekend = 0) 
 or ((EXTRACT(DOW from attend_day::timestamp )=0 or EXTRACT(DOW from attend_day::timestamp )=6) and works =1))
 )T2 group by person_id, name, depart_name, work_sn
 ) T3 where count>={1}
                order by convert_to(depart_name, 'GBK'),convert_to(name, 'GBK') ", sql_where, monthworkday);

            List<PersonFullAttendInfo> query = new List<PersonFullAttendInfo>();

            query.Clear();
            int zkCount = 1;
            DataTable dt = DbAccess.POSTGRESQL.Select(sql_all, "attend_record_base");
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }

            foreach (DataRow ar in dt.Rows)
            {
                PersonFullAttendInfo PersonFullAttendInfo = new PersonFullAttendInfo();

                PersonFullAttendInfo.Index = zkCount++;
                //如果可为空需要判断          
                PersonFullAttendInfo.PersonId = -1;
                if (ar["person_id"] != DBNull.Value)
                {
                    PersonFullAttendInfo.PersonId = Convert.ToInt32(ar["person_id"]);

                }
                PersonFullAttendInfo.PersonName = "";
                if (ar["name"] != DBNull.Value)
                {
                    PersonFullAttendInfo.PersonName = Convert.ToString(ar["name"]).Trim();
                }

               
                PersonFullAttendInfo.DepartName = "";
                if (ar["depart_name"] != DBNull.Value)
                {
                    PersonFullAttendInfo.DepartName = Convert.ToString(ar["depart_name"]).Trim();
                }
             
                query.Add(PersonFullAttendInfo);
            }

            return query;
        }
        #endregion

        #region 中科虹霸餐补
       
        /// <summary>        
        /// 获取人员餐补列表
        /// </summary>
        /// <param name="departName">查询条件 部门名称</param>    
        /// <param name="personName">查询条件 人员名称</param>    
        /// <returns>人员信息表</returns>
        [Query(HasSideEffects = true)]
        public IEnumerable<PersonMealSuppleInfo> GetPersonMealSuppleList(DateTime beginTime, DateTime endTime, string[] departName, string[] personName)
        {
            List<PersonMealSuppleInfo> PersonMealSuppleList = new List<PersonMealSuppleInfo>();
           
            List<UserPersonInfo> personList = (List<UserPersonInfo>)GetPersonInfo(departName, personName,null);
            InitPersonMealSuppleList(PersonMealSuppleList, personList, beginTime, endTime);//初始化数据
            //组装sql语句的where条件          
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
                sql_where += string.Format(" '{0}'", endTime);

            }
            string sql_all = string.Format(@"select * from (
select (SELECT count(*) FROM festival f WHERE f.begin_time <= a.attend_day AND a.attend_day <= f.end_time) weekend,
(select count(*) from weekend_to_work w where w.begin_time <= a.attend_day AND a.attend_day <= w.end_time) works,
a.person_id, name, depart_name, work_sn,attend_day ,out_well_time, work_time,
EXTRACT(EPOCH FROM date_trunc('day', in_well_time) - date_trunc('day', attend_day))/(60) + date_part('H', in_well_time)*60 + date_part('m', in_well_time) in_h,
EXTRACT(EPOCH FROM date_trunc('day', out_well_time) - date_trunc('day', attend_day))/(60) + date_part('H', out_well_time)*60 + date_part('m', out_well_time) out_h,
EXTRACT(EPOCH FROM date_trunc('day',a.attend_day)-'{0}')/(24*3600) as day_offset, EXTRACT(DOW from attend_day::timestamp ) dayorwork
from attend_record_base a 
left join person p on p.person_id= a.person_id where p.delete_time is null {1} 
        )T1
 where (((EXTRACT(DOW from attend_day::timestamp )>0 and EXTRACT(DOW from attend_day::timestamp )<6) and ((weekend = 0 and out_h >=1260) or(weekend =1 and work_time>240))) 
 or ((EXTRACT(DOW from attend_day::timestamp )=0 or EXTRACT(DOW from attend_day::timestamp )=6) and works =0 and work_time >240)
 or ((EXTRACT(DOW from attend_day::timestamp )=0 or EXTRACT(DOW from attend_day::timestamp )=6) and works =1 and out_h >=1260))
        order by convert_to(depart_name, 'GBK'), convert_to(name, 'GBK'), attend_day ", beginTime, sql_where);
          
            DataTable dt = DbAccess.POSTGRESQL.Select(sql_all, "attend_record_base");
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }
            PersonMealAttendDetail(PersonMealSuppleList, dt);
            int dayOffset = 0;
            for (int personIndex = 0; personIndex < PersonMealSuppleList.Count(); personIndex++)
            {
                //PersonMealSuppleList
                PersonMealSuppleList[personIndex].CountTimes = 0;
                PersonMealSuppleList[personIndex].CountMoney = 0;
                for (DateTime temp = beginTime; temp < endTime; temp = temp.AddDays(1))
                {
                    dayOffset = (temp - beginTime).Days;
                    int count = 0;
                    if (PersonMealSuppleList[personIndex].MealSuppTimes[dayOffset] != "")
                    {
                        count = Convert.ToInt32(PersonMealSuppleList[personIndex].MealSuppTimes[dayOffset]);
                    }
                   
                    PersonMealSuppleList[personIndex].CountTimes += count;
                    PersonMealSuppleList[personIndex].CountMoney += count * 20;
                }
            }
            

            return PersonMealSuppleList;
        }

        /// <summary>
        /// 计算考勤统计表中每人每天的详细
        /// </summary>
        /// <param name="totalAttendList"></param>
        /// <param name="dt"></param>
        private void PersonMealAttendDetail(List<PersonMealSuppleInfo> PersonMealSuppleList, DataTable dt)
        {
            if (null == dt || dt.Rows.Count <= 0)
            {
                return;
            }

            int personIndex = 0;
            int dayOffset = 0;
            for (int attendIndex = 0; attendIndex < dt.Rows.Count; attendIndex++)
            {
                while (personIndex < PersonMealSuppleList.Count() && PersonMealSuppleList[personIndex].PersonId != Convert.ToInt32(dt.Rows[attendIndex]["person_id"]))
                {
                    personIndex++;
                }

                if (personIndex >= PersonMealSuppleList.Count())
                {
                    return;
                }
                dayOffset = Convert.ToInt32(dt.Rows[attendIndex]["day_offset"]);
                try
                {
                    int counttimes = 0;
                    int worktimes = 0;
                    if (Convert.ToInt32(dt.Rows[attendIndex]["dayorwork"]) == 0 || Convert.ToInt32(dt.Rows[attendIndex]["dayorwork"]) == 6)
                    {
                        if ((Convert.ToInt32(dt.Rows[attendIndex]["weekend"]) == 1) || (Convert.ToInt32(dt.Rows[attendIndex]["weekend"]) == 0 && Convert.ToInt32(dt.Rows[attendIndex]["works"]) == 0))
                        {
                            //当时间为周六日时判断是否是节假日 或者非节假日 但是也不是调休 这个时候一天有4个小时会有一个补助 周末节假日不减一个小时了
                            //if (Convert.ToInt32(dt.Rows[attendIndex]["in_h"]) < 780)
                            //{
                            //    worktimes = Convert.ToInt32(dt.Rows[attendIndex]["work_time"]) - 60;
                            //    counttimes = worktimes / (240);//在这里工作时长大于4小时会有一个
                            //}
                            //else
                            {
                                worktimes = Convert.ToInt32(dt.Rows[attendIndex]["work_time"]);
                                counttimes = worktimes / (240);//在这里工作时长大于4小时会有一个
                            }
                        }
                        else
                        {
                            if (Convert.ToInt32(dt.Rows[attendIndex]["weekend"]) == 0 && Convert.ToInt32(dt.Rows[attendIndex]["works"]) == 1)
                            {
                                //这里的在每天晚上的21:00之后会有一个补助
                                if (Convert.ToInt32(dt.Rows[attendIndex]["out_h"]) >= 1260)
                                {
                                    counttimes = 1;
                                }
                            }
                        }
                    }
                    else
                    {
                        if ((Convert.ToInt32(dt.Rows[attendIndex]["weekend"]) == 1))
                        {
                            //当时间为周六日时判断是否是节假日 或者非节假日 但是也不是调休 这个时候一天有4个小时会有一个补助  周末节假日不减一个小时了
                            //if (Convert.ToInt32(dt.Rows[attendIndex]["in_h"]) < 780)
                            //{
                            //    worktimes = Convert.ToInt32(dt.Rows[attendIndex]["work_time"]) - 60;
                            //    counttimes = worktimes / (240);//在这里工作时长大于4小时会有一个
                            //}
                            //else
                            {
                                worktimes = Convert.ToInt32(dt.Rows[attendIndex]["work_time"]);
                                counttimes = worktimes / (240);//在这里工作时长大于4小时会有一个
                            }
                        }
                        else
                        {
                            if (Convert.ToInt32(dt.Rows[attendIndex]["out_h"]) >= 1260)
                            {
                                counttimes = 1;
                            }
                        }
                    }
                    //MealSuppTimes
                    if (counttimes > 2)
                    {
                        counttimes = 2;
                    }
                    PersonMealSuppleList[personIndex].MealSuppTimes[dayOffset] = counttimes.ToString();
                }
                catch
                {
                }
            }
        }  
       
        /// <summary>
        /// 初始化餐补统计信息
        /// </summary>
        /// <param name="totalAttendList"></param>
        /// <param name="personList"></param>
        /// <param name="dayCount"></param>
        private void InitPersonMealSuppleList(List<PersonMealSuppleInfo> PersonMealSuppleInfoList, List<UserPersonInfo> personList, DateTime beginTime, DateTime endTime)
        {
            PersonMealSuppleInfo tempAttend = new PersonMealSuppleInfo();
            int personIndex = 1;
            if (personList == null || personList.Count <= 0)
            {
                return;
            }
            #region 计算节假日与周末
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

            List<DateTime> shiftDate = new List<DateTime>();
            querySql = string.Format(@"select * from weekend_to_work where begin_time >= '{0}' and begin_time < '{1}'", beginTime, endTime);
            DataTable workDt = DbAccess.POSTGRESQL.Select(querySql, "weekend_to_work");
            if (null != workDt || workDt.Rows.Count >= 1)
            {
                foreach (DataRow item in workDt.Rows)
                {
                    shiftDate.Add(Convert.ToDateTime(item["begin_time"]));
                }
            }
            #endregion  
            int dayCount = (endTime - beginTime).Days;
            
            foreach (UserPersonInfo item in personList)
            {
                tempAttend = null;
                tempAttend = new PersonMealSuppleInfo();
                tempAttend.Index = personIndex++;
                tempAttend.DepartName = item.depart_name;
                tempAttend.PersonName = item.person_name;
                tempAttend.WorkSn = item.work_sn;
                tempAttend.PersonId = item.person_id;
                tempAttend.CountTimes = 0;
                tempAttend.CountMoney = 0;
              
                tempAttend.MealSuppTimes = new string[dayCount];
                tempAttend.AttendDay = new DateTime[dayCount];
                tempAttend.DayType = new int[dayCount];
                for (int dayIndex = 0; dayIndex < dayCount; dayIndex++)
                {
                    tempAttend.MealSuppTimes[dayIndex] = "";
                    tempAttend.AttendDay[dayIndex] = beginTime.AddDays( dayIndex );
                    //判断是否是周末 --add yuhaitao
                    if (shiftDate.Contains(beginTime.AddDays(dayIndex)))
                    {
                        tempAttend.DayType[dayIndex] = ShiftDay;
                    }
                    else if (festivalDate.Contains(beginTime.AddDays(dayIndex)))
                    {
                        tempAttend.DayType[dayIndex] = FestivalDay;
                    }
                    else if (beginTime.AddDays(dayIndex).DayOfWeek == DayOfWeek.Saturday || beginTime.AddDays(dayIndex).DayOfWeek == DayOfWeek.Sunday)
                    {
                        tempAttend.DayType[dayIndex] = WeekendDay;
                    }
                }               

                PersonMealSuppleInfoList.Add(tempAttend);
            }
        }      
        #endregion     

        #region 原始记录汇总表
        /// <summary>        
        /// 获取人员信息表
        /// </summary>
        /// <param name="departName">查询条件 部门名称</param>    
        /// <param name="personName">查询条件 人员名称</param>    
        /// <param name="workSn">查询条件 工号</param>    
        /// <returns>人员信息表</returns>
        [Query(HasSideEffects = true)]
        public IEnumerable<UserPersonInfo> GetPersonListInfo(string[] departName, string[] personName)
        {
            //组装sql语句的where条件          
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


            string sql_all = string.Format(@"SELECT person_id, depart_name, name, work_sn, ptype.principal_type_order, pricip.principal_name from person 
                Left join principal pricip on pricip.principal_id = person.principal_id Left join principal_type ptype on ptype.principal_type_id = pricip.principal_type_id
                where person.delete_time is null {0} 
                order by convert_to(depart_name,  'GBK'), ptype.principal_type_order, convert_to(pricip.principal_name,  E'GBK'), convert_to(name,  E'GBK'), convert_to(work_sn,  E'GBK')", sql_where);

            List<UserPersonInfo> query = new List<UserPersonInfo>();
            DataTable dt = DbAccess.POSTGRESQL.Select(sql_all, "person");
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }

            foreach (DataRow ar in dt.Rows)
            {
                UserPersonInfo userPersonInfo = new UserPersonInfo();

                //如果可为空需要判断          
                userPersonInfo.person_id = -1;
                if (ar["person_id"] != DBNull.Value)
                {
                    userPersonInfo.person_id = Convert.ToInt32(ar["person_id"]);

                }
                userPersonInfo.person_name = "";
                if (ar["name"] != DBNull.Value)
                {
                    userPersonInfo.person_name = Convert.ToString(ar["name"]).Trim();
                }

                userPersonInfo.work_sn = "";
                if (ar["work_sn"] != DBNull.Value)
                {
                    userPersonInfo.work_sn = Convert.ToString(ar["work_sn"]).Trim();
                }

                userPersonInfo.depart_name = "";
                if (ar["depart_name"] != DBNull.Value)
                {
                    userPersonInfo.depart_name = Convert.ToString(ar["depart_name"]).Trim();
                }
                query.Add(userPersonInfo);
            }

            return query;
        }
       
         /// <summary>        
        /// 获取人员原始记录汇总表
        /// </summary>
        /// <param name="departName">查询条件 部门名称</param>    
        /// <param name="personName">查询条件 人员名称</param>    
        /// <returns>人员信息表</returns>
        [Query(HasSideEffects = true)]
        public IEnumerable<PersonOriginInfo> GetPersonOriginRecList(DateTime beginTime, DateTime endTime, string[] departName, string[] personName)
        {
            List<PersonOriginInfo> PersonOriginInfoList = new List<PersonOriginInfo>();
            
            List<UserPersonInfo> personList = (List<UserPersonInfo>)GetPersonInfo(departName, personName, null);
            InitPersonOriginRecList(PersonOriginInfoList, personList, beginTime, endTime);//初始化数据
            //组装sql语句的where条件          
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
                sql_where += string.Format(" '{0}'", endTime);

            }
            string sql_all = string.Format(@"select a.person_id, work_time, is_valid, a.class_order_id,in_leave_type_id, out_leave_type_id,
case when p.class_type_id_on_ground is not null then p.class_type_id_on_ground else p.class_type_id end class_type_id,
 EXTRACT(EPOCH FROM date_trunc('day', in_well_time) - date_trunc('day', attend_day))/(60) + date_part('H', in_well_time)*60 + date_part('m', in_well_time) in_h,
EXTRACT(EPOCH FROM date_trunc('day', out_well_time) - date_trunc('day', attend_day))/(60) + date_part('H', out_well_time)*60 + date_part('m', out_well_time) out_h,
EXTRACT(EPOCH FROM date_trunc('day',a.attend_day)-'{1}')/(24*3600) as day_offset
  from attend_record_base a left join person p on p.person_id = a.person_id where p.delete_time is null {0} 
        order by convert_to(depart_name, 'GBK'),convert_to(name, 'GBK'), attend_day ", sql_where, beginTime);
            
            DataTable dt = DbAccess.POSTGRESQL.Select(sql_all, "attend_record_base");
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }

            PersonOriginDetail(PersonOriginInfoList, dt,beginTime, endTime);
            for (int personIndex = 0; personIndex < PersonOriginInfoList.Count(); personIndex++)
            {                
                PersonOriginInfoList[personIndex].PrvTimes = "";
                if (PersonOriginInfoList[personIndex].InOutTimes > 0)
                {
                    PersonOriginInfoList[personIndex].PrvTimes = Math.Round(((string.IsNullOrWhiteSpace(PersonOriginInfoList[personIndex].CountTimes) ? 0 : Convert.ToInt32(PersonOriginInfoList[personIndex].CountTimes))/PersonOriginInfoList[personIndex].InOutTimes), 0).ToString();
                }

                int tempInt;
                for (int dayIndex = 0; dayIndex < PersonOriginInfoList[personIndex].WorkTimes.Count(); dayIndex++ )
                {
                    if (PersonOriginInfoList[personIndex].WorkTimes[dayIndex] != "")
                    {
                        tempInt = Convert.ToInt32(PersonOriginInfoList[personIndex].WorkTimes[dayIndex]);
                        PersonOriginInfoList[personIndex].WorkTimes[dayIndex] = string.Format("{0}:{1:d2}", tempInt / 60, tempInt % 60);
                    }
                }
                if (PersonOriginInfoList[personIndex].CountTimes != "")
                {
                    tempInt = Convert.ToInt32(PersonOriginInfoList[personIndex].CountTimes);
                    PersonOriginInfoList[personIndex].CountTimes = string.Format("{0}:{1:d2}", tempInt / 60, tempInt % 60);
                }
                if (!string.IsNullOrWhiteSpace(PersonOriginInfoList[personIndex].OverTimes))
                {
                    tempInt = Convert.ToInt32(PersonOriginInfoList[personIndex].OverTimes);
                    PersonOriginInfoList[personIndex].OverTimes = string.Format("{0}:{1:d2}", tempInt / 60, tempInt % 60);
                }
                if (PersonOriginInfoList[personIndex].PrvTimes != "")
                {
                    tempInt = Convert.ToInt32(PersonOriginInfoList[personIndex].PrvTimes);
                    PersonOriginInfoList[personIndex].PrvTimes = string.Format("{0}:{1:d2}", tempInt / 60, tempInt % 60);
                }
                if (PersonOriginInfoList[personIndex].InOutTimes > 0)
                {
                    PersonOriginInfoList[personIndex].InOutTimes = (float)Math.Round(PersonOriginInfoList[personIndex].InOutTimes,2,MidpointRounding.ToEven);
                }
               
            }
            SetFestivalSignal(PersonOriginInfoList, beginTime, endTime);
            return PersonOriginInfoList;
        }

        #region 设置原始记录统计信息中的节日
 
        /// <summary>
        /// 设置原始记录统计信息中的节日
        /// </summary>
        /// <param name="totalAttendList"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        private void SetFestivalSignal(List<PersonOriginInfo> PersonOriginInfoList, DateTime beginTime, DateTime endTime)
        {
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

            List<DateTime> shiftDate = new List<DateTime>();
            querySql = string.Format(@"select * from weekend_to_work where begin_time >= '{0}' and begin_time < '{1}'", beginTime, endTime);
            DataTable workDt = DbAccess.POSTGRESQL.Select(querySql, "weekend_to_work");
            if (null != workDt || workDt.Rows.Count >= 1)
            {
                foreach (DataRow item in workDt.Rows)
                {
                    shiftDate.Add(Convert.ToDateTime(item["begin_time"]));
                }
            }

            //设置考勤统计信息中的节日及周末符号 add-yuhaitao
            int dayType;
            for (DateTime temp = beginTime; temp < endTime; temp = temp.AddDays(1))
            {
                if (shiftDate.Contains(temp))
                {
                    dayType = ShiftDay;
                    continue;
                }
                else if (festivalDate.Contains(temp))
                {
                    dayType = FestivalDay;
                }
                else if (temp.DayOfWeek == DayOfWeek.Saturday || temp.DayOfWeek == DayOfWeek.Sunday)
                {
                    dayType = WeekendDay;
                }
                else
                {
                    continue;
                }
                for (int personIndex = 0; personIndex < PersonOriginInfoList.Count(); personIndex++)
                {
                    PersonOriginInfoList[personIndex].DayType[(temp - beginTime).Days] = dayType;
                   
                }
                
            }
        }
        #endregion

        #region 初始化原始记录汇总表
        private void InitPersonOriginRecList(List<PersonOriginInfo> PersonOriginInfoList, List<UserPersonInfo> personList, DateTime beginTime, DateTime endTime)
        {
            PersonOriginInfo tempAttend = new PersonOriginInfo();
            int personIndex = 1;
            if (personList == null || personList.Count <= 0)
            {
                return;
            }

            int dayCount = (endTime - beginTime).Days;
           
            foreach (UserPersonInfo item in personList)
            {
                tempAttend = null;
                tempAttend = new PersonOriginInfo();
                tempAttend.Index = personIndex++;
                tempAttend.DepartName = item.depart_name;
                tempAttend.PersonName = item.person_name;
                tempAttend.WorkSn = item.work_sn;
                tempAttend.PersonId = item.person_id;
                tempAttend.CountTimes = "";
                tempAttend.InOutTimes = 0;
                tempAttend.PrvTimes = "";

                tempAttend.WorkTimes = new string[dayCount];
                tempAttend.DayType = new int[dayCount];
                int dayIndex = 0;
                for (dayIndex = 0; dayIndex < dayCount; dayIndex++)
                {
                    tempAttend.WorkTimes[dayIndex] = "";
                }               

                PersonOriginInfoList.Add(tempAttend);
            }
        }
        #endregion

        #region 计算人员每天的考勤详情工作的时间 等
        /// <summary>
        /// 计算考勤统计表中每人每天的工作时长 是否计算为请假 计算工时等
        /// </summary>
        /// <param name="PersonOriginInfoList"></param>
        /// <param name="dt"></param>
        private void PersonOriginDetail(List<PersonOriginInfo> PersonOriginInfoList, DataTable dt, DateTime beginTime, DateTime endTime)
        {
            if (null == dt || dt.Rows.Count <= 0)
            {
                return;
            }
            //获取节假日与周末日期 add-yuhaitao
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

            List<DateTime> shiftDate = new List<DateTime>();
            querySql = string.Format(@"select * from weekend_to_work where begin_time >= '{0}' and begin_time < '{1}'", beginTime, endTime);
            DataTable workDt = DbAccess.POSTGRESQL.Select(querySql, "weekend_to_work");
            if (null != workDt || workDt.Rows.Count >= 1)
            {
                foreach (DataRow item in workDt.Rows)
                {
                    shiftDate.Add(Convert.ToDateTime(item["begin_time"]));
                }
            }

            Dictionary<string, string> attendSignal = GetAttendSignal();
        

            int personIndex = 0;
            int dayOffset = 0;
            for (int attendIndex = 0; attendIndex < dt.Rows.Count; attendIndex++)
            {
                while (personIndex < PersonOriginInfoList.Count() && PersonOriginInfoList[personIndex].PersonId != Convert.ToInt32(dt.Rows[attendIndex]["person_id"]))
                {
                    personIndex++;
                }

                if (personIndex >= PersonOriginInfoList.Count())
                {
                    return;
                }
                dayOffset = Convert.ToInt32(dt.Rows[attendIndex]["day_offset"]);
                try
                {
                    //int counttimes = 0;
                    int worktimes = 0;
                    float inouttimes = 1;                    
                    worktimes = Convert.ToInt32(dt.Rows[attendIndex]["work_time"]);
                    int workTimeSub = 0;
                    if (worktimes > 0)
                    {
                        int inH = Convert.ToInt32(dt.Rows[attendIndex]["in_h"]);
                        int outH = Convert.ToInt32(dt.Rows[attendIndex]["out_h"]);

                        if (inH < 0) //跨天
                        {
                            inH += 1440;
                            outH += 1440;
                        }

                        if (inH <= 720)
                        {
                            if (outH >= 780)
                            {
                                workTimeSub = 60;
                            }
                            else if (outH >= 720)
                            {
                                workTimeSub = outH - 720;
                            }
                        }
                        else if (inH <= 780)
                        {
                            if (outH >= 780)
                            {
                                workTimeSub = 780 - inH;
                            }
                            else if (outH >= 720)
                            {
                                workTimeSub = 0;
                            }
                        }

                        worktimes -= workTimeSub;

                        int maxLeaveTime = 1080; //走的最晚时间
                        int minComeTime = 540; //来的最早时间
                        bool bAskLeave = false;

                        if (Convert.ToInt32(dt.Rows[attendIndex]["in_leave_type_id"]) == 2 || Convert.ToInt32(dt.Rows[attendIndex]["out_leave_type_id"]) == 3)//迟到或者早退
                        {
                            if (Convert.ToInt32(dt.Rows[attendIndex]["class_type_id"]) == 7)//这里来判断是弹性工作制(7)或者是固定工作(8) 
                            {
                                maxLeaveTime = 1110; //18:30     
                                minComeTime = 510; //8:30

                                if (inH > 600 || outH < 1020)
                                {
                                    bAskLeave = true;
                                }
                            }
                            if (Convert.ToInt32(dt.Rows[attendIndex]["class_type_id"]) == 8)//这里来判断是弹性工作制(7)或者是固定工作(8) 
                            {
                                maxLeaveTime = 1080; //18:00     
                                minComeTime = 540; //9:00

                                if (inH > 570 || outH < 1050)
                                {
                                    bAskLeave = true;
                                }
                            }                            

                            //早上早于来的最早时间
                            if (inH < minComeTime )
                            {
                                inH = minComeTime;
                            }
                            //或者下午晚于走的最晚时间
                            if (outH > maxLeaveTime)
                            {
                                outH = maxLeaveTime;
                            }

                            if (bAskLeave)
                            {
                                inouttimes = (float)((outH - inH - workTimeSub) / 60.0 / 8);

                                if (inouttimes > 1)
                                {
                                    inouttimes = 1;
                                }
                                if (inouttimes < 0)
                                {
                                    inouttimes = 0;
                                }
                            }
                        }
                    }
                    else
                    {
                        inouttimes = 0;
                    }
                    
                    if ( PersonOriginInfoList[personIndex].WorkTimes[dayOffset] == "" || Convert.ToInt32(PersonOriginInfoList[personIndex].WorkTimes[dayOffset]) < worktimes)
                    {
                        PersonOriginInfoList[personIndex].WorkTimes[dayOffset] = worktimes.ToString();

                        //计算正常工时
                        if ((!festivalDate.Contains(beginTime.AddDays(dayOffset)) && beginTime.AddDays(dayOffset).DayOfWeek != DayOfWeek.Saturday && beginTime.AddDays(dayOffset).DayOfWeek != DayOfWeek.Sunday) || shiftDate.Contains(beginTime.AddDays(dayOffset)))
                        {
                            if (PersonOriginInfoList[personIndex].CountTimes == "")
                            {
                                PersonOriginInfoList[personIndex].CountTimes = worktimes.ToString();
                            }
                            else
                            {
                                PersonOriginInfoList[personIndex].CountTimes = (Convert.ToInt32(PersonOriginInfoList[personIndex].CountTimes) + worktimes).ToString();
                            }
                            if (worktimes != 0)
                            {
                                PersonOriginInfoList[personIndex].InOutTimes += inouttimes;
                            }
                        }
                        else
                        {
                            if (PersonOriginInfoList[personIndex].OverTimes == "")
                            {
                                PersonOriginInfoList[personIndex].OverTimes = worktimes.ToString();
                            }
                            else
                            {
                                PersonOriginInfoList[personIndex].OverTimes = (Convert.ToInt32(PersonOriginInfoList[personIndex].OverTimes) + worktimes).ToString();
                            }
                        }
                    }
                }
                catch
                {
                }
            }
        }
        #endregion

       #endregion

        #region 漏考勤报表
        [Query(HasSideEffects = true)]
        public IEnumerable<LeakageAttendance> GetLeakageAttendanceCollect(DateTime beginTime, DateTime endTime, string[] departName, string[] personName)
        {
            //获取节假日与周末
            string strDate = GetHoliday(beginTime, endTime,"not");

            //组装sql语句的where条件          
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

            sql_where += " and ( in_well_time is null or out_well_time is null)";
            sql_where += strDate;
            string sql_all = string.Format(@"SELECT a.person_id, attend_day, in_well_time, out_well_time, name, depart_id, depart_name from attend_record_base a left join person p on p.person_id = a.person_id 
                                            where delete_time is null and  attend_day >= '{0}' and attend_day < '{1}' {2} ORDER BY convert_to(depart_name,  'GBK'), name, attend_day",
                                            beginTime, endTime, sql_where);

            List<LeakageAttendance> query = new List<LeakageAttendance>();
            DataTable dt = DbAccess.POSTGRESQL.Select(sql_all, "attend_record_base");
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }

            int count = 1;
            for (int flag = 0; flag < dt.Rows.Count; flag++)
            {
                LeakageAttendance personInWell = new LeakageAttendance();

                //如果可为空需要判断
                personInWell.PersonId = Convert.ToInt32(dt.Rows[flag]["person_id"]);
                personInWell.DepartName = dt.Rows[flag]["depart_name"].ToString().Trim();
                personInWell.PersonName = dt.Rows[flag]["name"].ToString().Trim();
                personInWell.AttendDay = DateTime.Parse(dt.Rows[flag]["attend_day"].ToString()).ToString("yyyy-MM-dd").Trim();
                personInWell.InWellTime = dt.Rows[flag]["in_well_time"].ToString().Trim();
                personInWell.OutWellTime = dt.Rows[flag]["out_well_time"].ToString().Trim();
                personInWell.Index = count++;

                query.Add(personInWell);

            }

            return query;
        }

        #region 计算节假日与周末
        /// <summary>
        /// 根据开始日期与结束日期获取节假日(包括周末与法定假日) add by yuhaitao
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        private string GetHoliday(DateTime beginTime, DateTime endTime,string where)
        {
            if (endTime.Year>=9999)
            {
                string sql = "select max(attend_day) from attend_record_all";
                DataTable dt = DbAccess.POSTGRESQL.Select(sql, "attend_record_all");
                if (null == dt || dt.Rows.Count < 1)
                {
                    return string.Empty;
                }
                endTime = DateTime.Parse(dt.Rows[0][0].ToString());
            }
            string querySql = string.Format(@"select * from festival where begin_time >= '{0}' and begin_time < '{1}'", beginTime, endTime);
            List<FestivalInfo> festivalList = (List<FestivalInfo>)GetFestivalBySql(querySql);
            StringBuilder strDate = new StringBuilder();
            strDate.AppendFormat(" and  date_trunc('day', attend_day) {0} in (",where);
            if (festivalList != null)
            {
                foreach (FestivalInfo item in festivalList)
                {
                    for (DateTime temp = Convert.ToDateTime(item.begin_time); temp <= Convert.ToDateTime(item.end_time); temp = temp.AddDays(1))
                    {
                        strDate.AppendFormat("'{0}',", temp.ToString("yyyy-MM-dd"));
                    }
                }
            }

            List<DateTime> shiftDate = new List<DateTime>();
            querySql = string.Format(@"select * from weekend_to_work where begin_time >= '{0}' and begin_time < '{1}'", beginTime, endTime);
            DataTable workDt = DbAccess.POSTGRESQL.Select(querySql, "weekend_to_work");
            if (null != workDt || workDt.Rows.Count >= 1)
            {
                foreach (DataRow item in workDt.Rows)
                {
                    shiftDate.Add(Convert.ToDateTime(item["begin_time"]));
                }
            }
            for (DateTime temp = beginTime; temp <= endTime; temp = temp.AddDays(1))
            {
                if ((temp.DayOfWeek == DayOfWeek.Saturday || temp.DayOfWeek == DayOfWeek.Sunday) && !shiftDate.Contains(temp))
                {
                    strDate.AppendFormat("'{0}',", temp.ToString("yyyy-MM-dd"));
                }
            }
            return strDate.ToString().TrimEnd(new char[] { ',' }) + ")"; 
        }
        #endregion
        #endregion

        #region 请假列表（计算迟到早退计为请假的 不包含已交请假单的）PersonLeaveListInfo
        public static List<PersonLeaveListInfo> PersonLeaveList = new List<PersonLeaveListInfo>();
        /// <summary>        
        /// 获取人员原始记录汇总表
        /// </summary>
        /// <param name="departName">查询条件 部门名称</param>    
        /// <param name="personName">查询条件 人员名称</param>    
        /// <returns>人员信息表</returns>
        [Query(HasSideEffects = true)]
        public IEnumerable<PersonLeaveListInfo> GetPersonLeaveList(DateTime beginTime, DateTime endTime, string[] departName, string[] personName)
        {
            PersonLeaveList = new List<PersonLeaveListInfo>();
            //组装sql语句的where条件          
            string sql_where = "";
            if (departName != null && departName.Count() > 0)
            {
                sql_where = string.Format(" and depart_name in ( ");
                for (int ind = 0; ind < departName.Count(); ind++)
                {
                    sql_where += string.Format(" '{0}', ", departName[ind]);
                }

                sql_where = sql_where.Remove(sql_where.LastIndexOf(","), 1);
                sql_where += ")";
            }

            if (personName != null && personName.Count() > 0)
            {
                sql_where += string.Format(" and name in ( ");
                for (int indx = 0; indx < personName.Count(); indx++)
                {
                    sql_where += string.Format(" '{0}', ", personName[indx]);
                }

                sql_where = sql_where.Remove(sql_where.LastIndexOf(","), 1);
                sql_where += ")";
            }
            if (beginTime != null && endTime != null)
            {
                sql_where += string.Format(" and attend_day >=");
                sql_where += string.Format(" '{0}'", beginTime);
                sql_where += string.Format(" and attend_day <=");
                sql_where += string.Format(" '{0}'", endTime);

            }
            string sql_all = string.Format(@"select *, case when ((class_type_id =7 and in_h > 600 ) or (class_type_id = 8 and in_h > 570 )) then 1 else 0 end late, 
case when (class_type_id =7 and out_h <1020) or (class_type_id = 8 and out_h <1050) then 1 else 0 end leaveEarly from
(select a.person_id,depart_name,name, p.work_sn, in_well_time,in_leave_type_id, out_well_time , out_leave_type_id, is_valid, a.class_order_id , 
case when p.class_type_id_on_ground is not null then p.class_type_id_on_ground else p.class_type_id end class_type_id,
(SELECT count(*) FROM attend_for_leave at WHERE date_trunc('day',at.leave_start_time ) <= date_trunc('day',a.attend_day ) AND date_trunc('day',a.attend_day ) <=  date_trunc('day',at.leave_end_time) and at.person_id = a.person_id) leave , attend_day,
EXTRACT(EPOCH FROM date_trunc('day', in_well_time) - date_trunc('day', attend_day))/(60) + date_part('H', in_well_time)*60 + date_part('m', in_well_time) in_h,
EXTRACT(EPOCH FROM date_trunc('day', out_well_time) - date_trunc('day', attend_day))/(60) + date_part('H', out_well_time)*60 + date_part('m', out_well_time) out_h,
(SELECT count(*) FROM festival f WHERE f.begin_time <= a.attend_day AND a.attend_day <= f.end_time) weekend,
(select count(*) from weekend_to_work w where w.begin_time <= a.attend_day AND a.attend_day <= w.end_time) works
from attend_record_base  a 
left join person p on p.person_id = a.person_id
where delete_time is null and (in_leave_type_id = 2 or out_leave_type_id = 3) {0}
)T1 where leave  = 0 and ((class_type_id =7 and (in_h > 600 or out_h <1020)) or (class_type_id = 8 and ( in_h >570 or out_h <1050)))
and ((((EXTRACT(DOW from attend_day::timestamp )>0 and EXTRACT(DOW from attend_day::timestamp )<6) and weekend = 0) 
 or ((EXTRACT(DOW from attend_day::timestamp )=0 or EXTRACT(DOW from attend_day::timestamp )=6) and works =1))) 
order by convert_to(depart_name, 'GBK'),convert_to(name, 'GBK'), attend_day ", sql_where);

            List<PersonLeaveListInfo> queryleave = new List<PersonLeaveListInfo>();

            queryleave.Clear();
            int zkCount = 1;
            DataTable dt = DbAccess.POSTGRESQL.Select(sql_all, "attend_record_base");
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }

            foreach (DataRow ar in dt.Rows)
            {
                PersonLeaveListInfo PersonLeaveLists = new PersonLeaveListInfo();

                PersonLeaveLists.Index = zkCount++;
                //如果可为空需要判断          
                PersonLeaveLists.PersonId = -1;
                if (ar["person_id"] != DBNull.Value)
                {
                    PersonLeaveLists.PersonId = Convert.ToInt32(ar["person_id"]);

                }
                PersonLeaveLists.PersonName = "";
                if (ar["name"] != DBNull.Value)
                {
                    PersonLeaveLists.PersonName = Convert.ToString(ar["name"]).Trim();
                }

                PersonLeaveLists.DepartName = "";
                if (ar["depart_name"] != DBNull.Value)
                {
                    PersonLeaveLists.DepartName = Convert.ToString(ar["depart_name"]).Trim();
                }

                PersonLeaveLists.AttendDay = "";
                if (ar["attend_day"] != DBNull.Value)
                {
                    PersonLeaveLists.AttendDay = Convert.ToDateTime(ar["attend_day"]).ToString("yyyy-MM-dd");
                }

                PersonLeaveLists.InWellTime = "";
                if (ar["in_well_time"] != DBNull.Value)
                {
                    PersonLeaveLists.InWellTime = Convert.ToString(ar["in_well_time"]).Trim();
                }

                PersonLeaveLists.OutWellTime = "";
                if (ar["out_well_time"] != DBNull.Value)
                {
                    PersonLeaveLists.OutWellTime = Convert.ToString(ar["out_well_time"]).Trim();
                }

                int inH = 0; 
                int outH = 0; 

                if (ar["in_h"] != null && ar["in_h"].ToString() != "")
                {
                    inH = Convert.ToInt32(ar["in_h"]);
                }

                if (ar["out_h"] != null && ar["out_h"].ToString() != "")
                {
                    outH = Convert.ToInt32(ar["out_h"]);
                }
                if (inH < 0) //跨天
                {
                    inH += 1440;                   
                    outH += 1440;                   
                }                        

                int maxComeTime = 600;  //来的最晚时间
                int minLeaveTime = 1020; //走的最早时间     
                int maxLeaveTime = 1110; //走的最晚时间
                int lateTime = 0;                       

                if (Convert.ToInt32(ar["in_leave_type_id"]) == 2 || Convert.ToInt32(ar["out_leave_type_id"]) == 3)//迟到或者早退
                {
                    if (Convert.ToInt32(ar["class_type_id"]) == 7)//这里来判断是弹性工作制(7)或者是固定工作(8) 
                    {
                        maxComeTime = 600; //10:00      
                        minLeaveTime = 1020; //17:00
                        maxLeaveTime = 1110; //18:30
                    }
                    if (Convert.ToInt32(ar["class_type_id"]) == 8)//这里来判断是弹性工作制(7)或者是固定工作(8) 
                    {
                        maxComeTime = 570; //9:30      
                        minLeaveTime = 1050; //17:30   
                        maxLeaveTime = 1080; //18:00
                    }
                   
                    //早上大于来的最晚时间
                    if (inH > maxComeTime && inH < maxLeaveTime)
                    {
                        if (inH > 720 && inH < 780)
                        {
                            inH = 720;
                        }
                        if (inH >= 780)
                        {
                            inH -= 60;
                        }
                        int tempLate = inH - (maxComeTime - 30);
                        lateTime = tempLate / 60;
                        if (tempLate % 60 != 0)
                        {
                            lateTime += 1;
                        }
                    }                        
                   
                    if (inH >= maxLeaveTime)
                    {
                        lateTime = 8;
                    }
                   
                    //或者下午早于走的最早时间
                    if (outH < minLeaveTime && outH != 0)
                    {
                        if (outH > 720 && outH < 780)
                        {
                            outH = 780;
                        }
                        if (outH <= 720)
                        {
                            outH += 60;
                        }
                        int tempLate = (maxLeaveTime - outH);
                        lateTime += tempLate / 60;
                        if (tempLate % 60 != 0)
                        {
                            lateTime += 1;
                        }
                    }
                   
                }

                if (lateTime > 8)
                {
                    lateTime = 8;
                }

                PersonLeaveLists.LeaveTimes = lateTime;
                queryleave.Add(PersonLeaveLists);
            }


            return queryleave;
        }
        #endregion

        #region 迟到早退报表
        
        /// <summary>        
        /// 迟到早退报表
        /// </summary>
        /// <param name="departName">查询条件 部门名称</param>    
        /// <param name="personName">查询条件 人员名称</param>    
        /// <returns>人员信息表</returns>
        [Query(HasSideEffects = true)]
        public IEnumerable<PersonLatearrivalInfo> GetPersonLatearrivalList(DateTime beginTime, DateTime endTime, string[] departName, string[] personName)
        {
            List<PersonLatearrivalInfo> PersonLatearrivalList = new List<PersonLatearrivalInfo>();           
            
            //组装sql语句的where条件          
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
                sql_where += string.Format(" '{0}'", endTime);

            }
            string sql_all = string.Format(@"select * , 
case when ((class_type_id =7 and (in_h > 570 and in_h <= 600 )) or (class_type_id = 8 and (in_h > 540 and in_h <= 570 ))) then 1 else 0 end late, 
case when (class_type_id =7 and (out_h <1050 and out_h >=1020 )) or (class_type_id = 8 and (out_h <1080 and out_h >=1050 )) then 1 else 0 end leaveEarly from
(select a.person_id,depart_name,name, p.work_sn, in_well_time,in_leave_type_id, out_well_time , out_leave_type_id, is_valid, a.class_order_id , 
case when p.class_type_id_on_ground is not null then p.class_type_id_on_ground else p.class_type_id end class_type_id,
(SELECT count(*) FROM attend_for_leave at WHERE date_trunc('day',at.leave_start_time ) <= date_trunc('day',a.attend_day ) AND date_trunc('day',a.attend_day ) <=  date_trunc('day',at.leave_end_time) and at.person_id = a.person_id) leave , attend_day,
EXTRACT(EPOCH FROM date_trunc('day', in_well_time) - date_trunc('day', attend_day))/(60) + date_part('H', in_well_time)*60 + date_part('m', in_well_time) in_h,
EXTRACT(EPOCH FROM date_trunc('day', out_well_time) - date_trunc('day', attend_day))/(60) + date_part('H', out_well_time)*60 + date_part('m', out_well_time) out_h,
EXTRACT(EPOCH FROM date_trunc('day',a.attend_day)-'{0}')/(24*3600) as day_offset,
(SELECT count(*) FROM festival f WHERE f.begin_time <= a.attend_day AND a.attend_day <= f.end_time) weekend,
(select count(*) from weekend_to_work w where w.begin_time <= a.attend_day AND a.attend_day <= w.end_time) works
from attend_record_base  a 
left join person p on p.person_id = a.person_id
where delete_time is null and (in_leave_type_id = 2 or out_leave_type_id = 3) {1}
)T1 where leave  = 0 and ((class_type_id =7 and ((in_h > 570 and in_h <= 600 )or (out_h <1050 and out_h >=1020))) or (class_type_id = 8 and ((in_h > 540 and in_h <= 570 )or (out_h <1080 and out_h >=1050)))) 
and ((((EXTRACT(DOW from attend_day::timestamp )>0 and EXTRACT(DOW from attend_day::timestamp )<6) and weekend = 0) 
 or ((EXTRACT(DOW from attend_day::timestamp )=0 or EXTRACT(DOW from attend_day::timestamp )=6) and works =1)))
order by convert_to(depart_name, 'GBK'),convert_to(name, 'GBK'), attend_day ", beginTime, sql_where);
            
            DataTable dt = DbAccess.POSTGRESQL.Select(sql_all, "attend_record_base");
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }

            int dayCount = (endTime - beginTime).Days;

            PersonLatearrivalDetail(PersonLatearrivalList, dt, dayCount, beginTime, endTime);
           
            return PersonLatearrivalList;
        }

        private void PersonLatearrivalDetail(List<PersonLatearrivalInfo> PersonLatearrivalList, DataTable dt, int dayCount, DateTime beginTime, DateTime endTime)
        {
            if (null == dt || dt.Rows.Count <= 0)
            {
                return;
            }

            #region 计算节假日与周末 
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

            List<DateTime> shiftDate = new List<DateTime>();
            querySql = string.Format(@"select * from weekend_to_work where begin_time >= '{0}' and begin_time < '{1}'", beginTime, endTime);
            DataTable workDt = DbAccess.POSTGRESQL.Select(querySql, "weekend_to_work");
            if (null != workDt || workDt.Rows.Count >= 1)
            {
                foreach (DataRow item in workDt.Rows)
                {
                    shiftDate.Add(Convert.ToDateTime(item["begin_time"]));
                }
            }
            #endregion  

            int personId = 0;
            int dayOffset = 0;
            int personIndex = -1;
            for (int attendIndex = 0; attendIndex < dt.Rows.Count; attendIndex++)
            {
                if (personId != Convert.ToInt32(dt.Rows[attendIndex]["person_id"]))
                {
                    personIndex++;
                    PersonLatearrivalInfo temp = new PersonLatearrivalInfo();

                    temp.PersonId = -1;
                    if (dt.Rows[attendIndex]["person_id"] != DBNull.Value)
                    {
                        temp.PersonId = Convert.ToInt32(dt.Rows[attendIndex]["person_id"]);
                    }
                    temp.PersonName = "";
                    if (dt.Rows[attendIndex]["name"] != DBNull.Value)
                    {
                        temp.PersonName = Convert.ToString(dt.Rows[attendIndex]["name"]).Trim();
                    }
                    temp.DepartName = "";
                    if (dt.Rows[attendIndex]["depart_name"] != DBNull.Value)
                    {
                        temp.DepartName = Convert.ToString(dt.Rows[attendIndex]["depart_name"]).Trim();
                    }
                    temp.Index = personIndex + 1;                   
                    temp.CountTimes = 0;
                    temp.LateArrivalTimes = new string[dayCount];
                    temp.DayType = new int[dayCount];
                   
                    for (int dayIndex = 0; dayIndex < dayCount; dayIndex++)
                    {
                        temp.LateArrivalTimes[dayIndex] = "";
                        //判断是否是周末 --add yuhaitao
                        if (shiftDate.Contains(beginTime.AddDays(dayIndex)))
                        {
                            temp.DayType[dayIndex] = ShiftDay;
                        }
                        else if (festivalDate.Contains(beginTime.AddDays(dayIndex)))
                        {
                            temp.DayType[dayIndex] = FestivalDay;
                        }
                        else if (beginTime.AddDays(dayIndex).DayOfWeek == DayOfWeek.Saturday || beginTime.AddDays(dayIndex).DayOfWeek == DayOfWeek.Sunday)
                        {
                            temp.DayType[dayIndex] = WeekendDay;
                        }
                    }
                    PersonLatearrivalList.Add(temp);

                    personId = temp.PersonId;
                }
              
                dayOffset = Convert.ToInt32(dt.Rows[attendIndex]["day_offset"]);
                try
                {
                    int counttimes = 0;
                    //
                    if (Convert.ToInt32(dt.Rows[attendIndex]["late"]) == 1)//迟到或者早退
                    {
                        counttimes += 1;
                        PersonLatearrivalList[personIndex].LateArrivalTimes[dayOffset] = "迟";
                        PersonLatearrivalList[personIndex].LateTimes += 1;
                    }
                    if (Convert.ToInt32(dt.Rows[attendIndex]["leaveEarly"]) == 1)
                    {
                        counttimes += 1;
                        PersonLatearrivalList[personIndex].LateArrivalTimes[dayOffset] = "退";
                        PersonLatearrivalList[personIndex].EarlyTimes += 1;
                    }
                    if (counttimes==2)
                    {
                        PersonLatearrivalList[personIndex].LateArrivalTimes[dayOffset] = counttimes.ToString();
                    }
                    PersonLatearrivalList[personIndex].CountTimes += counttimes;
                }
                catch
                {
                }
            }
        }

        #endregion

        #region 工时不足8小时报表 
               
        /// <summary>        
        /// 工时不足8小时报表
        /// </summary>
        /// <param name="departName">查询条件 部门名称</param>    
        /// <param name="personName">查询条件 人员名称</param>    
        /// <returns>人员信息表</returns>
        [Query(HasSideEffects = true)]
        public IEnumerable<PersonLatearrivalInfo> GetPersonTimeProblemList(DateTime beginTime, DateTime endTime, string[] departName, string[] personName)
        {
            List<PersonLatearrivalInfo> PersonTimeProblemList = new List<PersonLatearrivalInfo>();  
           
            //组装sql语句的where条件          
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
                sql_where += string.Format(" '{0}'", endTime);

            }
            string sql_all = string.Format(@"select * from
(select a.person_id,depart_name,name, in_well_time,out_well_time , is_valid,a.class_order_id , 
case when p.class_type_id_on_ground is not null then p.class_type_id_on_ground else p.class_type_id end class_type_id,
(SELECT count(*) FROM attend_for_leave at WHERE date_trunc('day',at.leave_start_time ) <= date_trunc('day',a.attend_day ) AND date_trunc('day',a.attend_day ) <=  date_trunc('day',at.leave_end_time) and at.person_id = a.person_id) leave , attend_day,
EXTRACT(EPOCH FROM date_trunc('day', in_well_time) - date_trunc('day', attend_day))/(60) + date_part('H', in_well_time)*60 + date_part('m', in_well_time) in_h,
EXTRACT(EPOCH FROM date_trunc('day', out_well_time) - date_trunc('day', attend_day))/(60) + date_part('H', out_well_time)*60 + date_part('m', out_well_time) out_h,
EXTRACT(EPOCH FROM date_trunc('day',a.attend_day)-'{0}')/(24*3600) as day_offset,
(SELECT count(*) FROM festival f WHERE f.begin_time <= a.attend_day AND a.attend_day <= f.end_time) weekend,
(select count(*) from weekend_to_work w where w.begin_time <= a.attend_day AND a.attend_day <= w.end_time) works
from attend_record_base  a 
left join person p on p.person_id = a.person_id
where delete_time is null and is_valid = -1 and in_well_time is not null and out_well_time is not null {1}
)T1 where leave  = 0 and ((class_type_id =7 and (in_h <= 600 )and (out_h >=1020)) or (class_type_id = 8 and (in_h <= 570 )and (out_h >=1050)))
and ((((EXTRACT(DOW from attend_day::timestamp )>0 and EXTRACT(DOW from attend_day::timestamp )<6) and weekend = 0) 
 or ((EXTRACT(DOW from attend_day::timestamp )=0 or EXTRACT(DOW from attend_day::timestamp )=6) and works =1)))  
order by convert_to(depart_name, 'GBK'),convert_to(name, 'GBK'), attend_day", beginTime, sql_where);

           
            DataTable dt = DbAccess.POSTGRESQL.Select(sql_all, "attend_record_base");
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }
            int dayCount = (endTime - beginTime).Days;
            PersonTimeProblemDetail(PersonTimeProblemList, dt, dayCount, beginTime, endTime);

            return PersonTimeProblemList;
        }


        private void PersonTimeProblemDetail(List<PersonLatearrivalInfo> PersonTimeProblemList, DataTable dt, int dayCount, DateTime beginTime, DateTime endTime)
        {
            if (null == dt || dt.Rows.Count <= 0)
            {
                return;
            }

            #region 计算节假日与周末
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

            List<DateTime> shiftDate = new List<DateTime>();
            querySql = string.Format(@"select * from weekend_to_work where begin_time >= '{0}' and begin_time < '{1}'", beginTime, endTime);
            DataTable workDt = DbAccess.POSTGRESQL.Select(querySql, "weekend_to_work");
            if (null != workDt || workDt.Rows.Count >= 1)
            {
                foreach (DataRow item in workDt.Rows)
                {
                    shiftDate.Add(Convert.ToDateTime(item["begin_time"]));
                }
            }
            #endregion  

            int personId = 0;
            int dayOffset = 0;
            int personIndex = -1;
            for (int attendIndex = 0; attendIndex < dt.Rows.Count; attendIndex++)
            {
                if (personId != Convert.ToInt32(dt.Rows[attendIndex]["person_id"]))
                {
                    personIndex++;
                    PersonLatearrivalInfo temp = new PersonLatearrivalInfo();

                    temp.PersonId = -1;
                    if (dt.Rows[attendIndex]["person_id"] != DBNull.Value)
                    {
                        temp.PersonId = Convert.ToInt32(dt.Rows[attendIndex]["person_id"]);
                    }
                    temp.PersonName = "";
                    if (dt.Rows[attendIndex]["name"] != DBNull.Value)
                    {
                        temp.PersonName = Convert.ToString(dt.Rows[attendIndex]["name"]).Trim();
                    }
                    temp.DepartName = "";
                    if (dt.Rows[attendIndex]["depart_name"] != DBNull.Value)
                    {
                        temp.DepartName = Convert.ToString(dt.Rows[attendIndex]["depart_name"]).Trim();
                    }
                    temp.Index = personIndex + 1;
                    temp.CountTimes = 0;
                    temp.LateArrivalTimes = new string[dayCount];
                    temp.DayType = new int[dayCount];
                    for (int dayIndex = 0; dayIndex < dayCount; dayIndex++)
                    {
                        temp.LateArrivalTimes[dayIndex] = "";
                        //判断是否是周末 --add yuhaitao
                        if (shiftDate.Contains(beginTime.AddDays(dayIndex)))
                        {
                            temp.DayType[dayIndex] = ShiftDay;
                        }
                        else if (festivalDate.Contains(beginTime.AddDays(dayIndex)))
                        {
                            temp.DayType[dayIndex] = FestivalDay;
                        }
                        else if (beginTime.AddDays(dayIndex).DayOfWeek == DayOfWeek.Saturday || beginTime.AddDays(dayIndex).DayOfWeek == DayOfWeek.Sunday)
                        {
                            temp.DayType[dayIndex] = WeekendDay;
                        }
                    }
                    PersonTimeProblemList.Add(temp);

                    personId = temp.PersonId;
                }

                dayOffset = Convert.ToInt32(dt.Rows[attendIndex]["day_offset"]);
                try
                {                   
                    PersonTimeProblemList[personIndex].LateArrivalTimes[dayOffset] = "1";
                    PersonTimeProblemList[personIndex].CountTimes += 1;
                }
                catch
                {
                }
            }
        }

        #endregion

        #region 中科虹霸考勤记录查询
        /// <summary>
        /// 中科虹霸考勤记录查询
        /// </summary>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">截止时间</param>
        /// <param name="departName">部门名称</param>
        /// <param name="leaveType">考勤类型</param>
        /// <param name="devTypeIdLst">设备类型</param>
        /// <param name="name">人员名字</param>
        /// <param name="workSN">人员工号</param>
        /// <returns></returns>
        public IEnumerable<UserAttendRec> IrisGetZKHBAttendRec(DateTime beginTime, DateTime endTime, string[] departName, List<int> devTypeIdLst,
            string name, string workSN, List<int> principalIdList, List<int> workTypeIdList, int workTime)
        {
            #region 初始化数据
            List<UserAttendRec> PersonOriginInfoList = new List<UserAttendRec>();

            List<UserPersonInfo> personList = (List<UserPersonInfo>)GetZKHBPersonInfo(departName, name, workSN);
            
            UserAttendRec tempAttend = new UserAttendRec();
            if (personList == null || personList.Count <= 0)
            {
                return null;
            }

            int dayCount = (endTime - beginTime).Days;
           
            foreach (UserPersonInfo item in personList)
            {
                tempAttend = null;
                tempAttend = new UserAttendRec();
                tempAttend.attend_record_id = keyId++;
                tempAttend.depart_name = item.depart_name;
                tempAttend.person_name = item.person_name;
                tempAttend.work_sn = item.work_sn;
                tempAttend.person_id = item.person_id;
                tempAttend.sum_work_time = "";
                tempAttend.sum_count = 0;
                tempAttend.avg_work_time = "";
                PersonOriginInfoList.Add(tempAttend);
            }
            	#endregion

            //组装sql语句的where条件          
            string sql_where = "";
            if (departName != null && departName.Count() > 0)
            {
                sql_where = string.Format(" and depart_name ='{0}' ",departName[0]);
            }
            if (!string.IsNullOrEmpty(name))
            {
                sql_where += string.Format(" and name like '%{0}%' ", name);
            }
            if (beginTime != null && endTime != null)
            {
                sql_where += string.Format(" and attend_day >=");
                sql_where += string.Format(" '{0}'", beginTime);
                sql_where += string.Format(" and attend_day <=");
                sql_where += string.Format(" '{0}'", endTime);

            }
            if (!string.IsNullOrEmpty(workSN))
            {
                sql_where += string.Format(" and p.work_sn like '%{0}%'", workSN);
            }
            string sql_all = string.Format(@"select a.person_id, work_time, is_valid, a.class_order_id,in_leave_type_id, out_leave_type_id,p.work_sn,
case when p.class_type_id_on_ground is not null then p.class_type_id_on_ground else p.class_type_id end class_type_id,
 EXTRACT(EPOCH FROM date_trunc('day', in_well_time) - date_trunc('day', attend_day))/(60) + date_part('H', in_well_time)*60 + date_part('m', in_well_time) in_h,
EXTRACT(EPOCH FROM date_trunc('day', out_well_time) - date_trunc('day', attend_day))/(60) + date_part('H', out_well_time)*60 + date_part('m', out_well_time) out_h,
EXTRACT(EPOCH FROM date_trunc('day',a.attend_day)-'{1}')/(24*3600) as day_offset
  from attend_record_base a left join person p on p.person_id = a.person_id where p.delete_time is null {0} 
        order by convert_to(depart_name, 'GBK'),convert_to(name, 'GBK'), attend_day ", sql_where, beginTime);

            DataTable dt = DbAccess.POSTGRESQL.Select(sql_all, "attend_record_base");
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }

            ZKHBAttendRecDetail(PersonOriginInfoList, dt, beginTime, endTime);
            for (int personIndex = 0; personIndex < PersonOriginInfoList.Count(); personIndex++)
            {
                PersonOriginInfoList[personIndex].avg_work_time = "";
                if (PersonOriginInfoList[personIndex].sum_times > 0)
                {
                    PersonOriginInfoList[personIndex].avg_work_time = Math.Round(((string.IsNullOrWhiteSpace(PersonOriginInfoList[personIndex].sum_work_time) ? 0 : Convert.ToInt32(PersonOriginInfoList[personIndex].sum_work_time)) / PersonOriginInfoList[personIndex].sum_times), 0).ToString();
                }

                int tempInt;
                if (!string.IsNullOrWhiteSpace(PersonOriginInfoList[personIndex].sum_work_time))
                {
                    tempInt = Convert.ToInt32(PersonOriginInfoList[personIndex].sum_work_time);
                    PersonOriginInfoList[personIndex].sum_work_time = string.Format("{0}:{1:d2}", tempInt / 60, tempInt % 60);
                }
                else
                {
                    PersonOriginInfoList[personIndex].sum_work_time = "0";
                }
                if (!string.IsNullOrWhiteSpace(PersonOriginInfoList[personIndex].sum_over_time))
                {
                    tempInt = Convert.ToInt32(PersonOriginInfoList[personIndex].sum_over_time);
                    PersonOriginInfoList[personIndex].sum_over_time = string.Format("{0}:{1:d2}", tempInt / 60, tempInt % 60);
                }
                else
                {
                    PersonOriginInfoList[personIndex].sum_over_time = "0";
                }
                if (!string.IsNullOrWhiteSpace(PersonOriginInfoList[personIndex].avg_work_time))
                {
                    tempInt = Convert.ToInt32(PersonOriginInfoList[personIndex].avg_work_time);
                    PersonOriginInfoList[personIndex].avg_work_time = string.Format("{0}:{1:d2}", tempInt / 60, tempInt % 60);
                }
                else
                {
                    PersonOriginInfoList[personIndex].avg_work_time = "0";
                }
                if (PersonOriginInfoList[personIndex].sum_times > 0)
                {
                    PersonOriginInfoList[personIndex].sum_times = (float)Math.Round(PersonOriginInfoList[personIndex].sum_times, 2, MidpointRounding.ToEven);
                }

            }

            return PersonOriginInfoList;
            
        }
        #region 计算人员每天的考勤详情工作的时间 等
        /// <summary>
        /// 计算考勤统计表中每人每天的工作时长 是否计算为请假 计算工时等
        /// </summary>
        /// <param name="PersonOriginInfoList"></param>
        /// <param name="dt"></param>
        private void ZKHBAttendRecDetail(List<UserAttendRec> PersonOriginInfoList, DataTable dt, DateTime beginTime, DateTime endTime)
        {
            if (null == dt || dt.Rows.Count <= 0)
            {
                return;
            }
            //获取节假日与周末日期 add-yuhaitao
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

            List<DateTime> shiftDate = new List<DateTime>();
            querySql = string.Format(@"select * from weekend_to_work where begin_time >= '{0}' and begin_time < '{1}'", beginTime, endTime);
            DataTable workDt = DbAccess.POSTGRESQL.Select(querySql, "weekend_to_work");
            if (null != workDt || workDt.Rows.Count >= 1)
            {
                foreach (DataRow item in workDt.Rows)
                {
                    shiftDate.Add(Convert.ToDateTime(item["begin_time"]));
                }
            }

            Dictionary<string, string> attendSignal = GetAttendSignal();


            int personIndex = 0;
            int dayOffset = 0;
            for (int attendIndex = 0; attendIndex < dt.Rows.Count; attendIndex++)
            {
                while (personIndex < PersonOriginInfoList.Count() && PersonOriginInfoList[personIndex].person_id != Convert.ToInt32(dt.Rows[attendIndex]["person_id"]))
                {
                    personIndex++;
                }

                if (personIndex >= PersonOriginInfoList.Count())
                {
                    return;
                }
                dayOffset = Convert.ToInt32(dt.Rows[attendIndex]["day_offset"]);
                try
                {
                    //int counttimes = 0;
                    int worktimes = 0;
                    float inouttimes = 1;
                    worktimes = Convert.ToInt32(dt.Rows[attendIndex]["work_time"]);//工作时间
                    int workTimeSub = 0;//午休时间
                    if (worktimes > 0)
                    {
                        int inH = Convert.ToInt32(dt.Rows[attendIndex]["in_h"]);
                        int outH = Convert.ToInt32(dt.Rows[attendIndex]["out_h"]);

                        if (inH < 0) //跨天
                        {
                            inH += 1440;
                            outH += 1440;
                        }

                        #region 计算午休时间
                        if (inH <= 720)//12点之前到
                        {
                            if (outH >= 780)
                            {
                                workTimeSub = 60;//13点后离开
                            }
                            else if (outH >= 720)
                            {
                                workTimeSub = outH - 720;//12到13点离开
                            }
                        }
                        else if (inH <= 780)//12到13点到
                        {
                            if (outH >= 780)
                            {
                                workTimeSub = 780 - inH;//13点后离开
                            }
                            else if (outH >= 720)
                            {
                                workTimeSub = 0;//12到13点离开
                            }
                        }
                        #endregion

                        worktimes -= workTimeSub;//减去午休时间

                        int maxLeaveTime = 1080; //18 走的最晚时间
                        int minComeTime = 540; //9 来的最早时间
                        bool bAskLeave = false;//迟到早退

                        #region 计算弹性工作时间
                        if (Convert.ToInt32(dt.Rows[attendIndex]["in_leave_type_id"]) == 2 || Convert.ToInt32(dt.Rows[attendIndex]["out_leave_type_id"]) == 3)//迟到或者早退
                        {
                            if (Convert.ToInt32(dt.Rows[attendIndex]["class_type_id"]) == 7)//这里来判断是弹性工作制(7)或者是固定工作(8) 
                            {
                                maxLeaveTime = 1110; //18:30     
                                minComeTime = 510; //8:30

                                if (inH > 600 || outH < 1020) //1小时弹性
                                {
                                    bAskLeave = true;
                                }
                            }
                            if (Convert.ToInt32(dt.Rows[attendIndex]["class_type_id"]) == 8)//这里来判断是弹性工作制(7)或者是固定工作(8) 
                            {
                                maxLeaveTime = 1080; //18:00     
                                minComeTime = 540; //9:00

                                if (inH > 570 || outH < 1050)//半小时弹性
                                {
                                    bAskLeave = true;
                                }
                            }

                            //早上早于来的最早时间
                            if (inH < minComeTime)
                            {
                                inH = minComeTime;
                            }
                            //或者下午晚于走的最晚时间
                            if (outH > maxLeaveTime)
                            {
                                outH = maxLeaveTime;
                            }

                            if (bAskLeave)
                            {
                                inouttimes = (float)((outH - inH - workTimeSub) / 60.0 / 8);

                                if (inouttimes > 1)
                                {
                                    inouttimes = 1;
                                }
                                if (inouttimes < 0)
                                {
                                    inouttimes = 0;
                                }
                            }
                        }
                        #endregion

                    }
                    else
                    {
                        inouttimes = 0;
                    }

                    //if (PersonOriginInfoList[personIndex].WorkTimes[dayOffset] == "" || Convert.ToInt32(PersonOriginInfoList[personIndex].WorkTimes[dayOffset]) < worktimes)
                    //{
                        //PersonOriginInfoList[personIndex].WorkTimes[dayOffset] = worktimes.ToString();

                        //计算正常工时
                        if ((!festivalDate.Contains(beginTime.AddDays(dayOffset)) && beginTime.AddDays(dayOffset).DayOfWeek != DayOfWeek.Saturday && beginTime.AddDays(dayOffset).DayOfWeek != DayOfWeek.Sunday) || shiftDate.Contains(beginTime.AddDays(dayOffset)))
                        {
                            if (PersonOriginInfoList[personIndex].sum_work_time == "")
                            {
                                PersonOriginInfoList[personIndex].sum_work_time = worktimes.ToString();
                            }
                            else
                            {
                                PersonOriginInfoList[personIndex].sum_work_time = (Convert.ToInt32(PersonOriginInfoList[personIndex].sum_work_time) + worktimes).ToString();
                            }
                            if (worktimes != 0)
                            {
                                PersonOriginInfoList[personIndex].sum_times += inouttimes;
                            }
                        }
                        else
                        {
                            if (PersonOriginInfoList[personIndex].sum_over_time == "")
                            {
                                PersonOriginInfoList[personIndex].sum_over_time = worktimes.ToString();
                            }
                            else
                            {
                                PersonOriginInfoList[personIndex].sum_over_time = (Convert.ToInt32(PersonOriginInfoList[personIndex].sum_over_time) + worktimes).ToString();
                            }
                        }
                    //}
                }
                catch
                {
                }
            }
        }
        #endregion

        /// <summary>        
        /// 获取人员信息表
        /// </summary>
        /// <param name="departName">查询条件 部门名称</param>    
        /// <param name="personName">查询条件 人员名称</param>    
        /// <param name="workSn">查询条件 工号</param>    
        /// <returns>人员信息表</returns>
        [Query(HasSideEffects = true)]
        public IEnumerable<UserPersonInfo> GetZKHBPersonInfo(string[] departName, string personName, string workSn)
        {
            //组装sql语句的where条件          
            string sql_where = "";
            if (departName != null && departName.Count() > 0)
            {
                sql_where = string.Format(" and depart_name ='{0}' ", departName[0]);
            }

            if (!string.IsNullOrEmpty(personName))
            {
                sql_where += string.Format(" and name like '%{0}%' ", personName);
            }
            if (!string.IsNullOrEmpty(workSn))
            {
                sql_where += string.Format(" and work_sn like '%{0}%'", workSn);
            }

            string sql_all = string.Format(@"SELECT person_id, depart_name, name, work_sn, ptype.principal_type_order, pricip.principal_name from person 
                Left join principal pricip on pricip.principal_id = person.principal_id Left join principal_type ptype on ptype.principal_type_id = pricip.principal_type_id
                where person.delete_time is null {0} 
                order by convert_to(depart_name,  E'GBK'), ptype.principal_type_order, convert_to(pricip.principal_name,  E'GBK'), convert_to(name,  E'GBK'), convert_to(work_sn,  E'GBK')", sql_where);

            List<UserPersonInfo> query = new List<UserPersonInfo>();
            DataTable dt = DbAccess.POSTGRESQL.Select(sql_all, "person");
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }

            foreach (DataRow ar in dt.Rows)
            {
                UserPersonInfo userPersonInfo = new UserPersonInfo();

                //如果可为空需要判断          
                userPersonInfo.person_id = -1;
                if (ar["person_id"] != DBNull.Value)
                {
                    userPersonInfo.person_id = Convert.ToInt32(ar["person_id"]);

                }
                userPersonInfo.person_name = "";
                if (ar["name"] != DBNull.Value)
                {
                    userPersonInfo.person_name = Convert.ToString(ar["name"]).Trim();
                }

                userPersonInfo.work_sn = "";
                if (ar["work_sn"] != DBNull.Value)
                {
                    userPersonInfo.work_sn = Convert.ToString(ar["work_sn"]).Trim();
                }

                userPersonInfo.depart_name = "";
                if (ar["depart_name"] != DBNull.Value)
                {
                    userPersonInfo.depart_name = Convert.ToString(ar["depart_name"]).Trim();
                }
                query.Add(userPersonInfo);
            }

            return query;
        }

        #endregion
    }
  
      

    #region 中科虹霸整月数据

    /// <summary>
    /// 中科虹霸
    /// </summary>
    [DataContract]
    public class ZKHBMonthList
    {
       /// <summary>
        /// 序号
        /// </summary>
        [DataMember]
        [Key]
        public int Index { get; set; }  
        /// <summary>
        /// 日期
        /// </summary>
        [DataMember]
        public int DateId { get; set; }

        /// <summary>
        /// 标记是周末还是上班 0/6是周末 1-5是周一至周五 7法定节假日
        /// </summary>
        [DataMember]
        public int Flag { get; set; }
    }
    #endregion 

    #region 中科虹霸餐补报表界面实体数据类

    /// <summary>
    /// 中科虹霸餐补查询界面实体数据类
    /// </summary>
    [DataContract]
    public class PersonMealSuppleInfo
    {
        /// <summary>
        /// 序号
        /// </summary>
        [DataMember]
        [Key]
        public int Index { get; set; }

        /// <summary>
        /// 归属日
        /// </summary>
        [DataMember]
        public DateTime[] AttendDay { get; set; }

        /// <summary>
        /// 部门名称
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
        /// 工号
        /// </summary>
        [DataMember]
        public string WorkSn { get; set; }
        /// <summary>
        /// 加班补助总次数
        /// </summary>
        [DataMember]
        public int CountTimes { get; set; }
        /// <summary>
        /// 加班补助金额
        /// </summary>
        [DataMember]
        public int CountMoney { get; set; }
          /// <summary>
        /// 前台显示 餐补的次数
        /// </summary> 
        [DataMember]
        public string[] MealSuppTimes { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [DataMember]
        public string Note { get; set; }

        /// <summary>
        /// 此天的类型， 0为上班日，1为周末，2为节假日，3为周末的调休上班日
        /// </summary>   
        [DataMember]
        public int[] DayType { get; set; }

    }
    #endregion
    
    #region 中科虹霸全勤奖报表界面实体数据类

    /// <summary>
    /// 中科虹霸全勤奖查询界面实体数据类
    /// </summary>
    [DataContract]
    public class PersonFullAttendInfo
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
        /// 人员id
        /// </summary>
        [DataMember]
        public int PersonId { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [DataMember]
        public string PersonName { get; set; }

        public PersonFullAttendInfo()
        {
            DepartName = "";
            PersonName = "";
        }

    }
    #endregion

    #region 中科虹霸原始记录汇总界面实体数据类

    /// <summary>
    /// 中科虹霸餐补查询界面实体数据类
    /// </summary>
    [DataContract]
    public class PersonOriginInfo
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
        /// 工作工时
        /// </summary>
        [DataMember]
        public string CountTimes { get; set; }

        /// <summary>
        /// 加班工时
        /// </summary>
        [DataMember]
        public string OverTimes { get; set; }

        /// <summary>
        /// 出勤次数
        /// </summary>
        [DataMember]
        public float InOutTimes { get; set; }

        /// <summary>
        /// 平均工时
        /// </summary>
        [DataMember]
        public string PrvTimes { get; set; }

        /// <summary>
        /// 前台显示 每天工作时间
        /// </summary> 
        [DataMember]
        public string[] WorkTimes { get; set; }

        /// <summary>
        /// 此天的类型， 0为上班日，1为周末，2为节假日，3为周末的调休上班日
        /// </summary>   
        [DataMember]
        public int[] DayType { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [DataMember]
        public string Note { get; set; }

    }
    #endregion

    #region 中科虹霸漏考勤统计实体数据类
    /// <summary>
    /// 中科虹霸漏考勤统计实体数据类
    /// </summary>
    [DataContract]
    public class LeakageAttendance
    {
        /// <summary>
        /// 序号
        /// </summary>
        [DataMember]
        [Key]
        public int Index { get; set; }
        /// <summary>
        /// 人员Id
        /// </summary>
        [DataMember]
        public int PersonId { get; set; }


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
        /// 日期
        /// </summary>
        [DataMember]
        public string AttendDay { get; set; }


        /// <summary>
        /// 上班时间
        /// </summary>
        [DataMember]
        public string InWellTime { get; set; }

        /// <summary>
        /// 下班时间
        /// </summary>
        [DataMember]
        public string OutWellTime { get; set; }

    }
    #endregion

    #region 中科虹霸请假报表实体数据类
    /// <summary>
    /// 中科虹霸请假报表实体数据类
    /// </summary>
    [DataContract]
    public class PersonLeaveListInfo
    {
        /// <summary>
        /// 序号
        /// </summary>
        [DataMember]
        [Key]
        public int Index { get; set; }
        /// <summary>
        /// 人员Id
        /// </summary>
        [DataMember]
        public int PersonId { get; set; }

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
        /// 日期
        /// </summary>
        [DataMember]
        public string AttendDay { get; set; }

        /// <summary>
        /// 上班时间
        /// </summary>
        [DataMember]
        public string InWellTime { get; set; }

        /// <summary>
        /// 下班时间
        /// </summary>
        [DataMember]
        public string OutWellTime { get; set; }

        ///<summary>
        ///计请假时间
        ///</summary>
        [DataMember]
        public int LeaveTimes { get; set; }

    }
    #endregion

    #region 中科虹霸迟到早退报表界面实体数据类

    /// <summary>
    /// 中科虹霸迟到早退报表界面实体数据类
    /// </summary>
    [DataContract]
    public class PersonLatearrivalInfo
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
        /// 总工时
        /// </summary>
        [DataMember]
        public int CountTimes { get; set; }

        /// <summary>
        /// 前台显示 迟到早退
        /// </summary> 
        [DataMember]
        public string[] LateArrivalTimes { get; set; }

        /// <summary>
        /// 此天的类型， 0为上班日，1为周末，2为节假日，3为周末的调休上班日
        /// </summary>   
        [DataMember]
        public int[] DayType { get; set; }

        /// <summary>
        /// 迟到次数
        /// </summary>
        [DataMember]
        public int LateTimes { get; set; }

        /// <summary>
        /// 早退次数
        /// </summary>
        [DataMember]
        public int EarlyTimes { get; set; }

    }
    #endregion

}