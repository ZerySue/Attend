/*************************************************************************
** 文件名:   DomainServiceIriskingAttend_ShenShuo.cs.cs
** 主要类:   DomainServiceIriskingAttend_ShenShuo.cs
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2013-12-3
** 修改人:   
** 日  期:
** 描  述:   用于神朔铁路报表查询的域服务
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
        public const int NormalDay = 0;
        public const int ShiftDay = 1;
        public const int WeekendDay = 2;
        public const int FestivalDay = 3;

        #region 神朔铁路个人记录表

        /// <summary>        
        /// 获取人员信息表
        /// </summary>
        /// <param name="depart_Name">查询条件 部门名称</param>       
        /// <returns>人员信息表</returns>
        [Query(HasSideEffects = true)]
        public IEnumerable<UserPersonInfo> GetPersonInfoByDepartName(string[] depart_Name)
        {
            //组装sql语句的where条件          
            string sql_where = "";
            if (depart_Name != null && depart_Name.Count() > 0 )
            {
                sql_where = string.Format(" and depart_name in ( ");
                for (int index = 0; index < depart_Name.Count(); index++)
                {
                    sql_where += string.Format(" '{0}', ", depart_Name[index]);
                }

                sql_where = sql_where.Remove(sql_where.LastIndexOf(","), 1);
                sql_where += ")";
            }

            string sql_all = string.Format(@"SELECT person_id, name, work_sn from person where person.delete_time is null {0}", sql_where);

            List<UserPersonInfo> query = new List<UserPersonInfo>();
            DataTable dt = DbAccess.POSTGRESQL.Select(sql_all, "person");
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }

            foreach (DataRow ar in dt.Rows)
            {
                UserPersonInfo userPersonInfo = new UserPersonInfo();

                //如果可为空需要判断   if(ar["attend_record_id"].ToString() != "")
                //或者 if(ar["attend_record_id"] != DBNull.Value)               
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
                query.Add(userPersonInfo);               
            }

            return query;
        }

        class InOutTimeList
        {
            /// <summary>
            /// 上午上班时间
            /// </summary>            
            public string[] MoringInWellTime;

            /// <summary>
            /// 上午下班时间
            /// </summary>            
            public string[] MoringOutWellTime;

            /// <summary>
            /// 下午上班时间
            /// </summary>            
            public string[] AfternoonInWellTime;

            /// <summary>
            /// 下午下班时间
            /// </summary>            
            public string[] AfternoonOutWellTime;

            public string[] MorningAttendSignal;
            public string[] AfternoonAttendSignal;
        }


        private void InitInOutTimeList(InOutTimeList[] inOutTime, int dayLength )
        {
            for (int index = 0; index < inOutTime.Count(); index++)
            {
                inOutTime[index] = new InOutTimeList();
                inOutTime[index].MoringInWellTime = new string[dayLength];
                inOutTime[index].MoringOutWellTime = new string[dayLength];
                inOutTime[index].AfternoonInWellTime = new string[dayLength];
                inOutTime[index].AfternoonOutWellTime = new string[dayLength];
                inOutTime[index].MorningAttendSignal = new string[dayLength];
                inOutTime[index].AfternoonAttendSignal = new string[dayLength];
            }
        }

        public static List<PersonAttend> PersonAttendList = new List<PersonAttend>();

        [Query(HasSideEffects = true)]
        public IEnumerable<PersonAttend> GetPersonAttendDetailList(DateTime beginTime, DateTime endTime, string[] departName, string[] personName, string[] workSn)
        {
            PersonAttendList = new List<PersonAttend>();

            List<UserPersonInfo> personList = (List<UserPersonInfo>)GetPersonInfo(departName, personName, workSn);
            List<TotalAttend> attendList = new List<TotalAttend>();   
            InitTotalAttendList(attendList, personList, beginTime, endTime);

            InOutTimeList[] inOutTime = new InOutTimeList[attendList.Count()];
            InitInOutTimeList(inOutTime, (endTime - beginTime).Days);

            //获得考勤数据
            DataTable dt = GetTotalAttendTable(beginTime, endTime, departName, personName, workSn);
            CaculateAttendDetail(attendList, inOutTime, dt);
            SetFestivalSignal(attendList, beginTime, endTime);

            Dictionary<string, string> attendSignal = GetAttendSignal();
            PersonAttend tempAttend = null;
            int dayOffset = 0;
            int keyIndex = 1;
            for (int index = 0; index < attendList.Count(); index++)
            {
                for (DateTime tempTime = beginTime; tempTime < endTime; tempTime = tempTime.AddDays(1))
                {
                    tempAttend = null;
                    tempAttend = new PersonAttend();
                    dayOffset = (tempTime - beginTime).Days;
                    tempAttend.Index = keyIndex++;
                    tempAttend.AttendDay = tempTime.ToString("yyyy-MM-dd");
                    tempAttend.DepartName = attendList[index].DepartName;
                    tempAttend.PersonName = attendList[index].PersonName;
                    tempAttend.DayType = attendList[index].DayType[dayOffset];
                    tempAttend.MoringInWellTime = "";
                    tempAttend.MoringOutWellTime = "";
                    tempAttend.AfternoonInWellTime = "";
                    tempAttend.AfternoonOutWellTime = "";
                    tempAttend.Note = "";

                    if (inOutTime[index].MorningAttendSignal[dayOffset] != null)
                    {
                        if (attendSignal.ContainsKey(inOutTime[index].MorningAttendSignal[dayOffset]) &&
                            inOutTime[index].MorningAttendSignal[dayOffset] != "加班" &&
                            inOutTime[index].MorningAttendSignal[dayOffset] != "出差")
                        {
                            //有正常上班的显示上、下班时间
                            tempAttend.MoringInWellTime = inOutTime[index].MoringInWellTime[dayOffset];
                            tempAttend.MoringOutWellTime = inOutTime[index].MoringOutWellTime[dayOffset];
                        }
                        else
                        {
                            //请假类型不在默认列表里的病假、事假等，显示请假类型简称，不显示时间
                            tempAttend.MoringInWellTime = inOutTime[index].MorningAttendSignal[dayOffset];
                            tempAttend.MoringOutWellTime = inOutTime[index].MorningAttendSignal[dayOffset];
                        }
                    }

                    if (inOutTime[index].AfternoonAttendSignal[dayOffset] != null)
                    {
                        if (attendSignal.ContainsKey(inOutTime[index].AfternoonAttendSignal[dayOffset]) &&
                            inOutTime[index].AfternoonAttendSignal[dayOffset] != "加班" &&
                            inOutTime[index].AfternoonAttendSignal[dayOffset] != "出差")
                        {
                            //有正常上班的显示上、下班时间
                            tempAttend.AfternoonInWellTime = inOutTime[index].AfternoonInWellTime[dayOffset];
                            tempAttend.AfternoonOutWellTime = inOutTime[index].AfternoonOutWellTime[dayOffset];
                        }
                        else
                        {
                            //请假类型不在默认列表里的病假、事假等，显示请假类型简称，不显示时间
                            tempAttend.AfternoonInWellTime = inOutTime[index].AfternoonAttendSignal[dayOffset];
                            tempAttend.AfternoonOutWellTime = inOutTime[index].AfternoonAttendSignal[dayOffset];
                        }
                    }
                    PersonAttendList.Add(tempAttend);
                }
            }
            return PersonAttendList;
        }

        #endregion

        #region 神朔铁路总的考勤统计表

        /// <summary>        
        /// 获取人员信息表
        /// </summary>
        /// <param name="departName">查询条件 部门名称</param>    
        /// <param name="personName">查询条件 人员名称</param>    
        /// <param name="workSn">查询条件 工号</param>    
        /// <returns>人员信息表</returns>
        [Query(HasSideEffects = true)]
        public IEnumerable<UserPersonInfo> GetPersonInfo(string[] departName, string[] personName, string[] workSn)
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

            if (workSn != null && workSn.Count() > 0)
            {
                sql_where += string.Format(" and work_sn in ( ");
                for (int index = 0; index < workSn.Count(); index++)
                {
                    sql_where += string.Format(" '{0}', ", workSn[index]);
                }

                sql_where = sql_where.Remove(sql_where.LastIndexOf(","), 1);
                sql_where += ")";
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

        /// <summary>
        /// 根据条件查询考勤统计表
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="departName"></param>
        /// <param name="personName"></param>
        /// <param name="workSn"></param>
        /// <returns></returns>
        private DataTable GetTotalAttendTable(DateTime beginTime, DateTime endTime, string[] departName, string[] personName, string[] workSn)
        {
            string strSQL = string.Format(@"select *, EXTRACT(EPOCH FROM date(attend_day)-date('{0}')::timestamp)/86400 day_offset from( SELECT p.name, p.work_sn, p.depart_id, p.depart_name, p.delete_time, 
                                                ar.person_id, ar.in_well_time, 
                                                ar.out_well_time, ar.work_time, 
                                                ar.is_valid, ar.leave_type_id, ar.class_order_id, 
                                                ar.attend_day, 
                                                ar.work_cnt, 
                                                ar.in_leave_type_id, ar.out_leave_type_id, co.class_order_name, 
                                                lt.leave_type_name AS leave_type_name, lt.attend_sign as attend_sign,
                                                inlt.leave_type_name AS in_leave_type_name, 
                                                outlt.leave_type_name AS out_leave_type_name,
                                                ptype.principal_type_order, pricip.principal_name
                                                FROM attend_record_normal ar
                                       LEFT JOIN person p ON ar.person_id = p.person_id
                                       LEFT JOIN class_order_normal co ON ar.class_order_id = co.class_order_id
                                       Left join leave_type lt on lt.leave_type_id = ar.leave_type_id 
                                       LEFT JOIN leave_type inlt ON ar.in_leave_type_id = inlt.leave_type_id
                                       LEFT JOIN leave_type outlt ON ar.out_leave_type_id = outlt.leave_type_id 
                                       Left join principal pricip on pricip.principal_id = p.principal_id 
                                       Left join principal_type ptype on ptype.principal_type_id = pricip.principal_type_id
                                       where p.delete_time is null and attend_day >= '{0}' and attend_day < '{1}'

                                UNION ALL 
                                     SELECT p.name, p.work_sn, p.depart_id, p.depart_name,p.delete_time,  
                                        al.person_id, case WHEN  al.leave_start_time > '{0}' then al.leave_start_time 
                                        else '{0}' end in_well_time, 
                                        case WHEN  al.leave_end_time < '{1}' then al.leave_end_time 
                                        else '{1}' end out_well_time,  0 AS work_time,
                                        0 AS is_valid, al.leave_type_id, NULL::integer AS class_order_id, 
                                        case WHEN  al.leave_start_time > '{0}' then al.leave_start_time 
                                        else '{0}' end attend_day, 
                                        0 AS work_cnt,  
                                        NULL::integer AS in_leave_type_id, 
                                        NULL::integer AS out_leave_type_id, 
                                        NULL::character varying AS class_order_name,            
                                        lt.leave_type_name AS leave_type_name, lt.attend_sign as attend_sign,            
                                        NULL::character varying AS in_leave_type_name, 
                                        NULL::character varying AS out_leave_type_name, 
                                        ptype.principal_type_order, pricip.principal_name
                                       FROM attend_for_leave al
                                      LEFT JOIN person p ON al.person_id = p.person_id
                                      Left join leave_type lt on lt.leave_type_id = al.leave_type_id 
                                      Left join principal pricip on pricip.principal_id = p.principal_id 
                                      Left join principal_type ptype on ptype.principal_type_id = pricip.principal_type_id
                                      where al.leave_start_time <= '{1}' and al.leave_end_time >= '{0}' ) as ar where delete_time is null", 
                                       beginTime.ToString("yyyy-MM-dd HH:mm:ss"), endTime.ToString("yyyy-MM-dd HH:mm:ss"));

            //组装sql语句的where条件          
            string sql_where = "";
            if (departName != null && departName.Count() > 0)
            {
                sql_where = string.Format(" and ar.depart_name in ( ");
                for (int index = 0; index < departName.Count(); index++)
                {
                    sql_where += string.Format(" '{0}', ", departName[index]);
                }

                sql_where = sql_where.Remove(sql_where.LastIndexOf(","), 1);
                sql_where += ")";
            }
            if (personName != null && personName.Count() > 0)
            {
                sql_where += string.Format(" and ar.name in ( ");
                for (int index = 0; index < personName.Count(); index++)
                {
                    sql_where += string.Format(" '{0}', ", personName[index]);
                }

                sql_where = sql_where.Remove(sql_where.LastIndexOf(","), 1);
                sql_where += ")";
            }

            if (workSn != null && workSn.Count() > 0)
            {
                sql_where += string.Format(" and ar.work_sn in ( ");
                for (int index = 0; index < workSn.Count(); index++)
                {
                    sql_where += string.Format(" '{0}', ", workSn[index]);
                }

                sql_where = sql_where.Remove(sql_where.LastIndexOf(","), 1);
                sql_where += ")";
            }

            strSQL += sql_where;

            strSQL += " order by convert_to(depart_name,  E'GBK'), principal_type_order, convert_to(principal_name,  E'GBK'), convert_to(name,  E'GBK'), convert_to(work_sn,  E'GBK'), attend_day,in_well_time,out_well_time";

            DataTable dt = DbAccess.POSTGRESQL.Select(strSQL, "attend_record");
            return dt;
        }

        public bool SetLeaveTypeNameByPriority(ref string origSignal, string newSignal)
        {
            if (origSignal == null)
            {
                origSignal = newSignal;
                return true;
            }

            //正常优先级最高
            if (origSignal == "正常" || origSignal == "早到")
            {
                return false;                
            }

            if (newSignal == "正常" || newSignal == "早到")
            {
                origSignal = newSignal;
                return true;
            }

            if (origSignal != null && origSignal.Contains("差"))
            {
                return false;
            }

            if (origSignal == "旷工")
            {
                origSignal = newSignal;
                return true;
            }

            if (IsContainAttendType(origSignal, false) && !IsContainAttendType(newSignal, false))
            {
                origSignal = newSignal;
                return true;
            }   
            
            return false;
        }

        public void SetSignalByPriority(ref string origSignal, string newSignal)
        {
            if (origSignal == null)
            {
                origSignal = newSignal;
                return;
            }

            //正常标志“+”优先级最高
            if (origSignal == "+")
            {
                return;
            }

            if (newSignal == "+")
            {
                origSignal = newSignal;
                return;
            }

            if (origSignal == "差")
            {
                return;
            }

            if (origSignal == "-")
            {
                origSignal = newSignal;
                return;
            }

            if (IsContainAttendType(origSignal, true) && !IsContainAttendType(newSignal, true))
            {
                origSignal = newSignal;
            }            
        }

        private bool IsContainAttendType(string signal, bool bValue)
        {
            Dictionary<string, string> attendSignal = new Dictionary<string, string>();

            attendSignal.Add("正常", "+");
            attendSignal.Add("早到", "+");
            attendSignal.Add("未签", "]");
            attendSignal.Add("异常", "[");
            attendSignal.Add("迟到", ">");
            attendSignal.Add("早退", "<");

            foreach (KeyValuePair<string, string> item in attendSignal)
            {
                if (bValue)
                {
                    if (signal.Contains(item.Value))
                    {
                        return true;
                    }
                }
                else
                {
                    if (signal.Contains(item.Key))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 考勤符号对照表
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetAttendSignal()
        {
            Dictionary<string, string> attendSignal = new Dictionary<string, string>();

            attendSignal.Add("正常", "+");
            attendSignal.Add("早到", "+");
            attendSignal.Add("未签", "]");
            attendSignal.Add("异常", "[");
            attendSignal.Add("迟到", ">");
            attendSignal.Add("早退", "<");

            attendSignal.Add("旷工", "-");
            attendSignal.Add("加班", "加");
            attendSignal.Add("出差", "差");
            return attendSignal;
        }

        /// <summary>
        /// 设置考勤符号
        /// </summary>
        /// <param name="decideTime"></param>
        /// <param name="attendtype"></param>
        /// <param name="dayOffset"></param>
        /// <param name="signal"></param>
        private void SetAttendSignal(DateTime decideTime, bool bIsInWellTime, TotalAttend attendtype, int dayOffset, string signal, InOutTimeList inOutTimeList, string leaveTypeName)
        {
            Dictionary<string, string> attendSignal = GetAttendSignal();
            string standardTime = "12:30:00";
            if (!bIsInWellTime)
            {
                standardTime = "12:30:01";
            }

            string str = "";
            if (decideTime.ToString("HH:mm:ss").CompareTo(standardTime) < 0)
            {
                str = "SELECT  class_order_name, out_well_start_time FROM class_order_normal where class_order_name like '%上午%'";
            }
            else
            {
                str = "SELECT  class_order_name, out_well_start_time FROM class_order_normal where class_order_name like '%下午%'";
            }

            if (signal == attendSignal["异常"])
            {
                DataTable dt = DbAccess.POSTGRESQL.Select(str, "attend_record");
                int outWellStartTime = 0;

                if (dt != null && dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["out_well_start_time"] != DBNull.Value)
                    {
                        outWellStartTime = Convert.ToInt16(dt.Rows[0]["out_well_start_time"]);
                        str = string.Format("{0}:{1}:00", (outWellStartTime / 60).ToString("d2"), (outWellStartTime % 60).ToString("d2"));

                        if (decideTime.ToString("HH:mm:ss").CompareTo(str) < 0)
                        {
                            signal += attendSignal["早退"];
                        }
                    }
                }
            }
           
            if (decideTime.ToString("HH:mm:ss").CompareTo(standardTime) < 0)
            {                 
                SetSignalByPriority(ref attendtype.MoringSignal[dayOffset], signal);
                if (inOutTimeList != null)
                {
                    //是否需要设置leavetype，设置后需要更新时间
                    if (SetLeaveTypeNameByPriority(ref inOutTimeList.MorningAttendSignal[dayOffset], leaveTypeName))
                    {
                        if (!bIsInWellTime)
                        {
                            inOutTimeList.MoringOutWellTime[dayOffset] = decideTime.ToString("HH:mm:ss");
                        }
                        else
                        {
                            inOutTimeList.MoringInWellTime[dayOffset] = decideTime.ToString("HH:mm:ss");                           
                        }
                    }    
                }
            }
            else
            {
                SetSignalByPriority(ref attendtype.AfternoonSignal[dayOffset], signal);
                 
                if (inOutTimeList != null)
                {
                    //是否需要设置leavetype，设置后需要更新时间
                    if (SetLeaveTypeNameByPriority(ref inOutTimeList.AfternoonAttendSignal[dayOffset], leaveTypeName))
                    {
                        if (!bIsInWellTime)
                        {
                            inOutTimeList.AfternoonOutWellTime[dayOffset] = decideTime.ToString("HH:mm:ss");
                        }
                        else
                        {
                            inOutTimeList.AfternoonInWellTime[dayOffset] = decideTime.ToString("HH:mm:ss");
                        }
                    }                        
                }
            }
        }

        /// <summary>
        /// 初始化考勤统计信息
        /// </summary>
        /// <param name="totalAttendList"></param>
        /// <param name="personList"></param>
        /// <param name="dayCount"></param>
        private void InitTotalAttendList(List<TotalAttend> totalAttendList, List<UserPersonInfo> personList, DateTime beginTime, DateTime endTime)
        {
            Dictionary<string, string> attendSignal = GetAttendSignal();
            TotalAttend tempAttend = new TotalAttend();
            int personIndex = 1;
            if (personList == null || personList.Count <= 0)
            {
                return;
            }

            int dayCount = (endTime - beginTime).Days;
            int nowDayOffset = 0;
            if (System.DateTime.Now >= beginTime)
            {
                nowDayOffset = (System.DateTime.Now - beginTime).Days + 1;
            }
            foreach (UserPersonInfo item in personList)
            {
                tempAttend = null;
                tempAttend = new TotalAttend();
                tempAttend.Index = personIndex++;
                tempAttend.DepartName = item.depart_name;
                tempAttend.PersonName = item.person_name;
                tempAttend.WorkSn = item.work_sn;
                tempAttend.PersonId = item.person_id;
                tempAttend.ClassType = "上午\r\n下午";

                tempAttend.DayType = new int[dayCount];
                tempAttend.MoringSignal = new string[dayCount];
                tempAttend.AfternoonSignal = new string[dayCount];
                tempAttend.DisplaySignal = new string[dayCount];

                int forntDay = dayCount > nowDayOffset ? nowDayOffset : dayCount;
                int dayIndex = 0;
                for (dayIndex = 0; dayIndex < forntDay; dayIndex++)
                {
                    tempAttend.MoringSignal[dayIndex] = attendSignal["旷工"];
                    tempAttend.AfternoonSignal[dayIndex] = attendSignal["旷工"];
                    tempAttend.DisplaySignal[dayIndex] = "";
                    tempAttend.DayType[dayIndex] = NormalDay;
                }
                if (dayCount > nowDayOffset)
                {
                    for (; dayIndex < dayCount; dayIndex++)
                    {
                        tempAttend.MoringSignal[dayIndex] = "";
                        tempAttend.AfternoonSignal[dayIndex] = "";
                        tempAttend.DisplaySignal[dayIndex] = "";
                        tempAttend.DayType[dayIndex] = NormalDay;
                    }
                }

                totalAttendList.Add(tempAttend);
            }
        }

        /// <summary>
        /// 设置考勤统计信息中的节日及周末符号
        /// </summary>
        /// <param name="totalAttendList"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        private void SetFestivalSignal(List<TotalAttend> totalAttendList, DateTime beginTime, DateTime endTime)
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

            Dictionary<string, string> attendSignal = GetAttendSignal(); 
            int dayType = NormalDay;
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
                    //if (temp.DayOfWeek == DayOfWeek.Friday)
                    //{
                    //    for (int personIndex = 0; personIndex < totalAttendList.Count(); personIndex++)
                    //    {
                    //        if (totalAttendList[personIndex].AfternoonSignal[(temp - beginTime).Days] == attendSignal["旷工"])
                    //        {
                    //            totalAttendList[personIndex].AfternoonSignal[(temp - beginTime).Days] = "";
                    //        }
                    //    }
                    //}
                    continue;
                }

                for (int personIndex = 0; personIndex < totalAttendList.Count(); personIndex++)
                {
                    totalAttendList[personIndex].DayType[(temp - beginTime).Days] = dayType;

                    if (totalAttendList[personIndex].MoringSignal[(temp - beginTime).Days] == attendSignal["旷工"] ||
                        (totalAttendList[personIndex].MoringSignal[(temp - beginTime).Days] == attendSignal["出差"] && dayType != FestivalDay) ||
                        !IsContainNormalLeaveType(totalAttendList[personIndex].MoringSignal[(temp - beginTime).Days]))
                    {
                        totalAttendList[personIndex].MoringSignal[(temp - beginTime).Days] = "";
                    }

                    if (totalAttendList[personIndex].AfternoonSignal[(temp - beginTime).Days] == attendSignal["旷工"] ||
                        (totalAttendList[personIndex].AfternoonSignal[(temp - beginTime).Days] == attendSignal["出差"] && dayType != FestivalDay) ||
                        !IsContainNormalLeaveType(totalAttendList[personIndex].AfternoonSignal[(temp - beginTime).Days]))
                    {
                        totalAttendList[personIndex].AfternoonSignal[(temp - beginTime).Days] = "";
                    }
                }
            }
        }

        private string CalculateSignal( string classSignal, string inSignal, string outSignal )
        {
            Dictionary<string, string> attendSignal = GetAttendSignal(); 

            string signal = "";
            if (classSignal == "正常" || classSignal == "早到")
            {
                signal = attendSignal[classSignal];
                return signal;
            }

            if (inSignal != "正常" && inSignal != "早到")
            {
                if (inSignal == "未签")
                {
                    inSignal = "异常";
                }
                signal += attendSignal[inSignal];
            }
            if (outSignal != "正常" && outSignal != "加班")
            {
                signal += attendSignal[outSignal];
            }
            return signal;
        }

        /// <summary>
        /// 计算考勤统计表中每人每天的详细出勤符号
        /// </summary>
        /// <param name="totalAttendList"></param>
        /// <param name="dt"></param>
        private void CaculateAttendDetail(List<TotalAttend> totalAttendList, InOutTimeList[] inOutTime, DataTable dt)
        {
            if (null == dt || dt.Rows.Count <=0 )
            {
                return;
            }

            Dictionary<string, string> attendSignal = GetAttendSignal(); 
          
            int personIndex = 0;
            int dayOffset = 0;
            string classSignal = "";
            string inSignal = "";
            string outSignal = "";
            string leaveTypeName = "";

            for (int attendIndex = 0; attendIndex < dt.Rows.Count; attendIndex++)
            {
                while (personIndex < totalAttendList.Count() && totalAttendList[personIndex].PersonId != Convert.ToInt32(dt.Rows[attendIndex]["person_id"]))
                {
                    personIndex++;
                }

                if (personIndex >= totalAttendList.Count())
                {
                    return;
                }

                dayOffset = Convert.ToInt32(dt.Rows[attendIndex]["day_offset"]);
                classSignal = "";
                inSignal = "";
                outSignal = "";

                try
                {
                    if (Convert.ToInt32(dt.Rows[attendIndex]["leave_type_id"]) < 50 && attendSignal.ContainsKey(dt.Rows[attendIndex]["leave_type_name"].ToString()))
                    {
                        classSignal = dt.Rows[attendIndex]["leave_type_name"].ToString();
                        inSignal = dt.Rows[attendIndex]["in_leave_type_name"].ToString();
                        outSignal = dt.Rows[attendIndex]["out_leave_type_name"].ToString();

                        classSignal = CalculateSignal(classSignal, inSignal, outSignal);

                        leaveTypeName = dt.Rows[attendIndex]["leave_type_name"].ToString();    
                    }
                    else
                    {
                        classSignal = dt.Rows[attendIndex]["attend_sign"].ToString();
                        leaveTypeName = dt.Rows[attendIndex]["attend_sign"].ToString();  
                    }              
                }
                catch
                {                    
                }

                DateTime? inWellTime = null;
                DateTime? outWellTime = null;

                if (dt.Rows[attendIndex]["out_well_time"] != null && dt.Rows[attendIndex]["out_well_time"].ToString() != "")
                {
                    outWellTime = Convert.ToDateTime(dt.Rows[attendIndex]["out_well_time"]);
                }

                if (dt.Rows[attendIndex]["in_well_time"] != null && dt.Rows[attendIndex]["in_well_time"].ToString() != "")
                {
                    inWellTime = Convert.ToDateTime(dt.Rows[attendIndex]["in_well_time"]);
                }

                string classOrderName = "";
                if (dt.Rows[attendIndex]["class_order_name"] != null && dt.Rows[attendIndex]["class_order_name"].ToString() != "")
                {
                    classOrderName = dt.Rows[attendIndex]["class_order_name"].ToString();
                }
                if (classOrderName != "")
                {
                    if (classSignal == "")
                    {
                        continue;
                    }
                    if (classOrderName.Contains("上午"))
                    {
                        SetSignalByPriority(ref totalAttendList[personIndex].MoringSignal[dayOffset], classSignal);
                       
                        if (inOutTime != null)
                        {
                            //是否需要设置leavetype，设置后需要更新时间
                            if (SetLeaveTypeNameByPriority(ref inOutTime[personIndex].MorningAttendSignal[dayOffset], leaveTypeName))
                            {
                                if (inWellTime != null)
                                {
                                    inOutTime[personIndex].MoringInWellTime[dayOffset] = ((DateTime)inWellTime).ToString("HH:mm:ss");
                                }
                                if (outWellTime != null)
                                {
                                    inOutTime[personIndex].MoringOutWellTime[dayOffset] = ((DateTime)outWellTime).ToString("HH:mm:ss");
                                }
                            }
                        }
                    }
                    else if (classOrderName.Contains("下午"))
                    {
                        SetSignalByPriority(ref totalAttendList[personIndex].AfternoonSignal[dayOffset], classSignal);
                        
                        if (inOutTime != null)
                        {
                            //是否需要设置leavetype，设置后需要更新时间
                            if (SetLeaveTypeNameByPriority(ref inOutTime[personIndex].AfternoonAttendSignal[dayOffset], leaveTypeName))
                            {
                                if (inWellTime != null)
                                {
                                    inOutTime[personIndex].AfternoonInWellTime[dayOffset] = ((DateTime)inWellTime).ToString("HH:mm:ss");
                                }
                                if (outWellTime != null)
                                {
                                    inOutTime[personIndex].AfternoonOutWellTime[dayOffset] = ((DateTime)outWellTime).ToString("HH:mm:ss");
                                }
                            }
                        }
                    }
                    continue;
                }

                InOutTimeList tempInOutTime = null;
                if (inOutTime != null)
                {
                    tempInOutTime = inOutTime[personIndex];
                }
                if ((inWellTime != null) && (outWellTime != null))
                {
                    if (classSignal == "")
                    {
                        continue;
                    }
                    for (DateTime temp = (DateTime)inWellTime; temp < outWellTime; temp = ((DateTime)temp).AddDays(1))
                    {
                        int day = dayOffset + (temp - (DateTime)inWellTime).Days;
                        if (day >= totalAttendList[personIndex].DayType.Count())
                        {
                            break;
                        }
                        if (temp.ToString("yyyy-MM-dd").Equals(((DateTime)inWellTime).ToString("yyyy-MM-dd")))
                        {
                            SetAttendSignal((DateTime)inWellTime, true, totalAttendList[personIndex], day, classSignal, tempInOutTime, leaveTypeName);
                        }
                        else
                        {
                            if (inOutTime != null)
                            {
                                SetLeaveTypeNameByPriority(ref inOutTime[personIndex].MorningAttendSignal[day], leaveTypeName);
                            }
                            SetSignalByPriority(ref totalAttendList[personIndex].MoringSignal[day], classSignal);
                        }

                        if (temp.ToString("yyyy-MM-dd").Equals(((DateTime)outWellTime).ToString("yyyy-MM-dd")))
                        {
                            SetAttendSignal((DateTime)outWellTime, false, totalAttendList[personIndex], day, classSignal, tempInOutTime, leaveTypeName);
                        }
                        else
                        {
                            if (inOutTime != null)
                            {
                                SetLeaveTypeNameByPriority(ref inOutTime[personIndex].AfternoonAttendSignal[day], leaveTypeName);
                            }
                            SetSignalByPriority(ref totalAttendList[personIndex].AfternoonSignal[day], classSignal);
                        }
                    }
                }
                else if (inWellTime != null)
                {
                    if (classSignal == "")
                    {
                        classSignal = attendSignal["异常"];
                    }
                    SetAttendSignal((DateTime)inWellTime, true, totalAttendList[personIndex], dayOffset, classSignal, tempInOutTime, leaveTypeName);
                }
                else if (outWellTime != null)
                {
                    if (classSignal == "")
                    {
                        classSignal = attendSignal["未签"];
                    }
                    SetAttendSignal((DateTime)outWellTime, false, totalAttendList[personIndex], dayOffset, classSignal, tempInOutTime, leaveTypeName);
                }
            }
        }

        private bool IsContainNormalLeaveType(string signal)
        {
            Dictionary<string, string> attendSignal = GetAttendSignal();
            
            foreach (KeyValuePair<string, string> item in attendSignal)
            {
                if (signal.Contains(item.Value))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 计算考勤统计表中的出勤综合状况
        /// </summary>
        /// <param name="personAttend"></param>
        /// <param name="signal"></param>
        private void CalculateTotalAttend( TotalAttend personAttend, string signal )
        {
            Dictionary<string, string> attendSignal = GetAttendSignal();

            bool bValue = IsContainNormalLeaveType(signal);

            if ( bValue )
            {
                if (signal.Contains(attendSignal["旷工"]))
                {
                    personAttend.AbsentNum += 0.5f;
                }
                //if (signal.Contains(attendSignal["加班"]))
                //{
                //    personAttend.ExtraNum += 0.5f;
                //}
                if (signal.Contains(attendSignal["出差"]))
                {
                    personAttend.BusinessNum += 0.5f;
                }
                if (signal.Contains(attendSignal["迟到"]))
                {
                    personAttend.LateNum += 1f;
                }
                if (signal.Contains(attendSignal["早退"]))
                {
                    personAttend.LeaveEarlyNum += 1f;
                }
            }
            else if (signal != "")
            {
                personAttend.AskLeaveNum += 0.5f;
            }
        }

        public static List<TotalAttend> TotalAttendList = new List<TotalAttend>();
        public static string[] DepartName = new string[0];
        
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
        public IEnumerable<TotalAttend> GetTotalAttendDetailList(DateTime beginTime, DateTime endTime, string[] departName, string[] personName, string[] workSn)
        {
            TotalAttendList = new List<TotalAttend>();
            
            if (departName == null || departName.Count() == 0)
            {
                List<UserDepartInfo> departList = (List<UserDepartInfo>)GetDepartInfo();
                DepartName = new string[departList.Count];

                for (int departIndex = 0; departIndex < departList.Count; departIndex++)
                {
                    DepartName[departIndex] = departList[departIndex].depart_name;
                }
            }
            else
            {
                DepartName = departName;
            }

            List<UserPersonInfo> personList = (List<UserPersonInfo>)GetPersonInfo(departName, personName, workSn);
            InitTotalAttendList(TotalAttendList, personList, beginTime, endTime);

            if (endTime.Date > System.DateTime.Now.AddDays(1).Date )
            {
                endTime = System.DateTime.Now.AddDays(1).Date;
            }

            //获得考勤数据
            DataTable dt = GetTotalAttendTable(beginTime, endTime, departName, personName, workSn);
            CaculateAttendDetail(TotalAttendList, null, dt);

            SetFestivalSignal(TotalAttendList, beginTime, endTime);

            Dictionary<string, string> attendSignal = GetAttendSignal();
            int dayOffset = 0;
            for (int personIndex = 0; personIndex < TotalAttendList.Count(); personIndex++)
            {
                TotalAttendList[personIndex].SupposeNum = 0;
                TotalAttendList[personIndex].ActualNum = 0;
                TotalAttendList[personIndex].AbsentNum = 0;
                TotalAttendList[personIndex].LateNum = 0;
                TotalAttendList[personIndex].LeaveEarlyNum = 0;
                TotalAttendList[personIndex].ExtraNum = 0;
                TotalAttendList[personIndex].AskLeaveNum = 0;
                TotalAttendList[personIndex].BusinessNum = 0;

                for (DateTime temp = beginTime; temp < endTime; temp = temp.AddDays(1))
                {
                    dayOffset = (temp - beginTime).Days;
                    TotalAttendList[personIndex].DisplaySignal[dayOffset] =
                       TotalAttendList[personIndex].MoringSignal[dayOffset] + "\r\n"
                       + TotalAttendList[personIndex].AfternoonSignal[dayOffset];

                    if (TotalAttendList[personIndex].MoringSignal[dayOffset].Contains(attendSignal["加班"]))
                    {
                        TotalAttendList[personIndex].ExtraNum += 0.5f;
                    }
                    if (TotalAttendList[personIndex].AfternoonSignal[dayOffset].Contains(attendSignal["加班"]))
                    {
                        TotalAttendList[personIndex].ExtraNum += 0.5f;
                    }

                    if (TotalAttendList[personIndex].DayType[dayOffset] < WeekendDay)
                    {
                        CalculateTotalAttend(TotalAttendList[personIndex], TotalAttendList[personIndex].MoringSignal[dayOffset]);
                        CalculateTotalAttend(TotalAttendList[personIndex], TotalAttendList[personIndex].AfternoonSignal[dayOffset]);

                        TotalAttendList[personIndex].SupposeNum += 1f;
                    }
                }
                TotalAttendList[personIndex].ActualNum = TotalAttendList[personIndex].SupposeNum +
                                                        TotalAttendList[personIndex].ExtraNum -
                                                        TotalAttendList[personIndex].AskLeaveNum -
                                                        TotalAttendList[personIndex].AbsentNum;
            }
            return TotalAttendList;
        }
        #endregion

        /// <summary>
        /// 父部门按名称排序，父部门内子部门按名称排序 by cty 2014-3-27
        /// </summary>
        /// <returns></returns>
        public List<depart> GetCustomOrderbyDepartsInfo()
        {
            List<depart> departAll = new List<depart>();
            string sql = string.Format(@"select * from depart where delete_time is null order by depart_name");
            
            DataTable dt = DbAccess.POSTGRESQL.Select(sql, "depart");
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }

            foreach (DataRow ar in dt.Rows)
            {
                depart d = new depart();

                if (ar["depart_id"] != DBNull.Value)
                {
                    d.depart_id = Convert.ToInt32(ar["depart_id"]);
                }
                if (ar["company_sn"] != DBNull.Value)
                {
                    d.company_sn = Convert.ToString(ar["company_sn"]).Trim();
                }

                if (ar["depart_sn"] != DBNull.Value)
                {
                    d.depart_sn = Convert.ToString(ar["depart_sn"]).Trim();
                }

                if (ar["depart_name"] != DBNull.Value)
                {
                    d.depart_name = Convert.ToString(ar["depart_name"]).Trim();
                }
                if (ar["parent_depart_id"] != DBNull.Value)
                {
                    d.parent_depart_id = Convert.ToInt32(ar["parent_depart_id"]);
                }

                departAll.Add(d);
            }
            var parentDepart=from pd in departAll where pd.parent_depart_id == Int32.MinValue select pd;
            var childDepart = from cd in departAll where cd.parent_depart_id != Int32.MinValue select cd;
            List<depart> query = new List<depart>();
            foreach (depart d in parentDepart)
            {
                query.Add(d);
                foreach (depart cd in childDepart)
                {
                    if(cd.parent_depart_id == d.depart_id)
                    {
                        query.Add(cd);
                    }
                }
            }
            return query;
        }
    }

    #region 神朔铁路个人考勤查询界面实体数据类

    /// <summary>
    /// 神朔铁路个人考勤查询界面实体数据类
    /// </summary>
    [DataContract]
    public class PersonAttend
    {
        /// <summary>
        /// 序号
        /// </summary>
        [DataMember]
        [Key]
        public int Index { get; set; }

        [DataMember]
        public int DayType { get; set; }

        /// <summary>
        /// 归属日
        /// </summary>
        [DataMember]        
        public string AttendDay { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        [DataMember]
        public string DepartName { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [DataMember]
        public string PersonName { get; set; }

        /// <summary>
        /// 上午上班时间
        /// </summary>
        [DataMember]
        public string MoringInWellTime { get; set; }

        /// <summary>
        /// 上午下班时间
        /// </summary>
        [DataMember]
        public string MoringOutWellTime { get; set; }

        /// <summary>
        /// 下午上班时间
        /// </summary>
        [DataMember]
        public string AfternoonInWellTime { get; set; }

        /// <summary>
        /// 下午下班时间
        /// </summary>
        [DataMember]
        public string AfternoonOutWellTime { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [DataMember]
        public string Note { get; set; }

    }
   
    /// <summary>
    /// 神朔铁路总的考勤统计实体数据类
    /// </summary>
    [DataContract]
    public class TotalAttend
    {
        /// <summary>
        /// 序号
        /// </summary>
        [DataMember]
        [Key]
        public int Index { get; set; }

        /// <summary>
        /// 姓名id
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
        /// 此天的类型， 0为上班日，1为周末，2为节假日，3为周末的调休上班日
        /// </summary>   
        [DataMember]
        public int[] DayType { get; set; }

        /// <summary>
        /// 上午
        /// </summary> 
        [DataMember]
        public string[] MoringSignal { get; set; }

        /// <summary>
        /// 下午
        /// </summary> 
        [DataMember]
        public string[] AfternoonSignal { get; set; }

        /// <summary>
        /// 前台显示 = 上午 + “\r\n” + 下午
        /// </summary> 
        [DataMember]
        public string[] DisplaySignal { get; set; }

        /// <summary>
        /// 应到
        /// </summary>   
        [DataMember]
        public float SupposeNum { get; set; }

        /// <summary>
        /// 实到
        /// </summary>   
        [DataMember]
        public float ActualNum { get; set; }

        /// <summary>
        /// 旷工
        /// </summary>   
        [DataMember]
        public float AbsentNum { get; set; }

        /// <summary>
        /// 迟到
        /// </summary>     
        [DataMember]
        public float LateNum { get; set; }

        /// <summary>
        /// 早退
        /// </summary>    
        [DataMember]
        public float LeaveEarlyNum { get; set; }

        /// <summary>
        /// 加班
        /// </summary>    
        [DataMember]
        public float ExtraNum { get; set; }

        /// <summary>
        /// 请假
        /// </summary>     
        [DataMember]
        public float AskLeaveNum { get; set; }

        /// <summary>
        /// 公出
        /// </summary>      
        [DataMember]
        public float BusinessNum { get; set; }  
    }
    
    #endregion

}