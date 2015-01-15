/*************************************************************************
** 文件名:   DomainServiceIriskingAttend_Xls.cs
** 主要类:   DomainServiceIriskingAttend_Xls
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   cty
** 日  期:   2014-4-10
** 修改人:   
** 日  期:
** 描  述:   用于报表查询的域服务
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
    using System.IO;

    public partial class DomainServiceIriskingAttend
    {
        #region 获取西沟一矿带班领导出勤数据

        /// <summary>
        /// 获取西沟一矿带班领导出勤数据
        /// </summary>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">截止时间</param>
        /// <param name="departIdLst">部门列表</param>
        /// <returns></returns>
        [Query(HasSideEffects = true)]
        public IEnumerable<XiGouLeaderAttend> GetXiGouLeaderAttendRec(DateTime beginTime, DateTime endTime, int[] departIdLst, int person_id)
        {
            List<XiGouLeaderAttend> query = new List<XiGouLeaderAttend>();
            query.Clear();
            query = GetXiGouLeaderList(beginTime, endTime);
            if (query == null || query.Count() == 0)
            {
                return null;
            }
            //超时时间
            int overtime = GetOverTime();

            string sql = string.Format(@"select ar.attend_record_id,p.person_id,p.name,p.work_sn,d.depart_name,d.depart_id,pri.principal_name,ar.in_well_time,ar.out_well_time,ar.work_time,ar.is_valid,ar.attend_day,con.class_order_name,ct.class_type_name from attend_record_normal as ar 
                                        left join person as p on ar.person_id=p.person_id
                                        left join depart as d on p.depart_id=d.depart_id
                                        left join principal as pri on p.principal_id=pri.principal_id
                                        left join class_order_normal as con on ar.class_order_id=con.class_order_id
                                        left join class_type as ct on con.class_type_id=ct.class_type_id where ar.attend_day >= '{0}' and ar.attend_day < '{1}'", beginTime, endTime);

            #region 选择条件是否选择了部门

            if (null != departIdLst && departIdLst.Length > 0)
            {
                sql += " and d.depart_id in ( ";
                for (int i = 0; i < departIdLst.Length - 1; i++)
                {
                    sql += departIdLst[i] + ",";
                }
                sql += departIdLst[departIdLst.Length - 1] + ")";
            }
            #endregion

            if (person_id != -1)
            {
                sql += " and p.person_id = " + person_id;
            }
            DataTable dt = DbAccess.POSTGRESQL.Select(sql, "attend_record_person");
            if (null != dt && dt.Rows.Count > 0)
            {
                foreach (DataRow ar in dt.Rows)
                {
                    foreach (var la in query)
                    {
                        if (CompareTreat(ar, la))
                        {
                            if (ar["attend_record_id"] != DBNull.Value)
                            {
                                la.AttendRecordId = Convert.ToInt32(ar["attend_record_id"]);
                            }
                            if (ar["work_time"] != DBNull.Value)
                            {
                                la.WorkTimeMinutes = Convert.ToInt32(ar["work_time"]);

                            }
                            if (ar["in_well_time"] != DBNull.Value)
                            {
                                DateTime inwelltime = Convert.ToDateTime(ar["in_well_time"]);
                                la.InWellTime = inwelltime.ToString("HH:mm");
                            }
                            if (ar["out_well_time"] != DBNull.Value)
                            {
                                DateTime outwelltime = Convert.ToDateTime(ar["out_well_time"]);
                                la.OutWellTime = outwelltime.ToString("HH:mm");
                            }
                            if (ar["out_well_time"] != DBNull.Value && ar["in_well_time"] != DBNull.Value)
                            {
                                la.WorkTime = (Convert.ToDateTime(ar["out_well_time"]) - Convert.ToDateTime(ar["in_well_time"])).ToString(@"hh\:mm");
                            }
                            if (ar["is_valid"] != DBNull.Value)
                            {
                                if (Convert.ToInt32(ar["is_valid"]) == -1 || la.WorkTimeMinutes > overtime)
                                {
                                    if (la.WorkTimeMinutes != 0)
                                    {
                                        la.color = "red";
                                    }
                                }
                            }
                            continue;
                        }
                    }
                }
            }


            if (null != departIdLst && departIdLst.Length > 0)
            {
                var value = from q in query where departIdLst.Contains(q.DepartId) orderby q.AttendDay, q.ShiftPersonName, q.DepartName select q;
                return value;
            }
            else if (person_id != -1)
            {
                var value2 = from q in query where q.ShiftPersonId == person_id orderby q.AttendDay, q.ShiftPersonName, q.DepartName select q;
                return value2;
            }
            var value3 = from q in query orderby q.AttendDay, q.ShiftPersonName, q.DepartName select q;
            return value3;
        }


        public bool CompareTreat(DataRow ar, XiGouLeaderAttend la)
        {
            bool match = false;
            if (Convert.ToInt32(ar["person_id"]) == la.ShiftPersonId && Convert.ToDateTime(ar["attend_day"].ToString()).ToShortDateString() == la.AttendDay.ToShortDateString())
            {
                if (ar["class_order_name"] != DBNull.Value)
                {
                    if (ar["class_order_name"].ToString() == la.ClassOrderName)
                    {
                        match = true;
                    }
                }
            }
            return match;
        }
        #endregion

        #region 获取带班人员列表
        /// <summary>
        /// 获取带班人员列表
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public List<XiGouLeaderAttend> GetXiGouLeaderList(DateTime beginTime, DateTime endTime)
        {
            List<XiGouLeaderAttend> leaderList = new List<XiGouLeaderAttend>();
            leaderList.Clear();
            string sqlperson = string.Format(@"select ls.date_id,
                                               p_todayleader1.person_id as todayleader1_person_id, p_todayleader1.name as todayleader1_name,p_todayleader1.depart_id as todayleader1_depart_id,p_todayleader1.depart_name as todayleader1_depart_name,p_todayleader1.work_sn as todayledaer1_word_sn,pri_todayleader1.principal_name as todayleader1_principal_name,
                                               p_todayleader2.person_id as todayleader2_person_id, p_todayleader2.name as todayleader2_name,p_todayleader2.depart_id as todayleader2_depart_id,p_todayleader2.depart_name as todayleader2_depart_name,p_todayleader2.work_sn as todayledaer2_word_sn,  pri_todayleader2.principal_name as todayleader2_principal_name,
                                               p_morn.person_id as morn_person_id, p_morn.name as morn_name,p_morn.depart_id as morn_depart_id,p_morn.depart_name as morn_depart_name,p_morn.work_sn as morn_word_sn, pri_morn.principal_name as morn_principal_name,
                                                 (select ct.class_type_name as morn_class_type_name from class_order_normal as con left join class_type as ct on con.class_type_id=ct.class_type_id where con.attend_sign like '%早%'),
                                                 (select class_order_name as morn_class_order_name from class_order_normal where attend_sign like '%早%'),
                                               p_mid.person_id as mid_person_id, p_mid.name as mid_name,p_mid.depart_id as mid_depart_id,p_mid.depart_name as mid_depart_name,p_mid.work_sn as mid_word_sn, pri_mid.principal_name as mid_principal_name,
                                                 (select ct.class_type_name as mid_class_type_name from class_order_normal as con left join class_type as ct on con.class_type_id=ct.class_type_id where con.attend_sign like '%中%'),
                                                 (select class_order_name as mid_class_order_name from class_order_normal where attend_sign like '%中%'),
                                               p_night.person_id as night_person_id, p_night.name as night_name,p_night.depart_id as night_depart_id,p_night.depart_name as night_depart_name,p_night.work_sn as night_word_sn,pri_night.principal_name as night_principal_name,
                                                 (select ct.class_type_name as night_class_type_name from class_order_normal as con left join class_type as ct on con.class_type_id=ct.class_type_id where con.attend_sign like '%夜%'),
                                                 (select class_order_name as night_class_order_name from class_order_normal where attend_sign like '%夜%')
                                               from xigou_leader_schedule as ls 
	                                            LEFT JOIN person as p_todayleader1 on ls.todayleader1=p_todayleader1.person_id	
	                                            left join principal as pri_todayleader1 on p_todayleader1.principal_id=pri_todayleader1.principal_id
	
	                                            left join person as p_todayleader2 on ls.todayleader2=p_todayleader2.person_id
	                                            left join principal as pri_todayleader2 on p_todayleader2.principal_id=pri_todayleader2.principal_id
	
	                                            left join person as p_morn on ls.morningleader=p_morn.person_id
	                                            left join principal as pri_morn on p_morn.principal_id=pri_morn.principal_id
	
	                                            left join person as p_mid on ls.midleader=p_mid.person_id
	                                            left join principal as pri_mid on p_mid.principal_id=pri_mid.principal_id
	
	                                            left join person as p_night on ls.nightleader=p_night.person_id
	                                            left join principal as pri_night on p_night.principal_id=pri_night.principal_id 
                                                where ls.date_id >='{0}' and ls.date_id < '{1}'", beginTime, endTime);

            DataTable dt = DbAccess.POSTGRESQL.Select(sqlperson, "attend_record_person");
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }
            int index = 1;
            foreach (DataRow ar in dt.Rows)
            {
                #region 获取早班带班领导信息
                XiGouLeaderAttend laMorn = new XiGouLeaderAttend();

                laMorn.index = index;
                if (ar["date_id"] != DBNull.Value)
                {
                    laMorn.AttendDay = Convert.ToDateTime(ar["date_id"]);
                    laMorn.AttendDayStr = Convert.ToDateTime(ar["date_id"]).ToShortDateString();
                }
                if (ar["morn_person_id"] != DBNull.Value)
                {
                    laMorn.ShiftPersonId = Convert.ToInt32(ar["morn_person_id"]);
                }
                laMorn.ShiftPersonName = ar["morn_name"].ToString();
                if (ar["morn_depart_id"] != DBNull.Value)
                {
                    laMorn.DepartId = Convert.ToInt32(ar["morn_depart_id"]);
                }
                if (ar["morn_depart_name"] != DBNull.Value)
                {
                    laMorn.DepartName = ar["morn_depart_name"].ToString();
                }
                if (ar["morn_word_sn"] != DBNull.Value)
                {
                    laMorn.ShiftWorkSn = ar["morn_word_sn"].ToString();
                }
                if (ar["morn_principal_name"] != DBNull.Value)
                {
                    laMorn.ShiftPrincipal = ar["morn_principal_name"].ToString();
                }
                if (ar["morn_class_order_name"] != DBNull.Value)
                {
                    laMorn.ClassOrderName = ar["morn_class_order_name"].ToString();
                }
                if (ar["morn_class_type_name"] != DBNull.Value)
                {
                    laMorn.ClassTypeName = ar["morn_class_type_name"].ToString();
                }
                #region 获取值班领导信息
                if (ar["todayleader1_person_id"] != DBNull.Value)
                {
                    laMorn.OnDutyPersonId = Convert.ToInt32(ar["todayleader1_person_id"]);
                }
                if (ar["todayleader1_name"] != DBNull.Value)
                {
                    laMorn.OnDutyPersonName = ar["todayleader1_name"].ToString();
                }
                if (ar["todayledaer1_word_sn"] != DBNull.Value)
                {
                    laMorn.OnDutyWorkSn = ar["todayledaer1_word_sn"].ToString();
                }

                if (ar["todayleader2_person_id"] != DBNull.Value)
                {
                    laMorn.OnDutyPersonId2 = Convert.ToInt32(ar["todayleader2_person_id"]);
                }
                if (ar["todayleader2_name"] != DBNull.Value)
                {
                    laMorn.OnDutyPersonName2 = ar["todayleader2_name"].ToString();
                }
                if (ar["todayledaer2_word_sn"] != DBNull.Value)
                {
                    laMorn.OnDutyWorkSn2 = ar["todayledaer2_word_sn"].ToString();
                }
                #endregion
                leaderList.Add(laMorn);
                index++;
                #endregion

                #region 获取中班带班领导信息
                XiGouLeaderAttend laMid = new XiGouLeaderAttend();

                laMid.index = index;
                if (ar["date_id"] != DBNull.Value)
                {
                    laMid.AttendDay = Convert.ToDateTime(ar["date_id"]);
                    laMid.AttendDayStr = Convert.ToDateTime(ar["date_id"]).ToShortDateString();
                }
                if (ar["mid_person_id"] != DBNull.Value)
                {
                    laMid.ShiftPersonId = Convert.ToInt32(ar["mid_person_id"]);
                }
                laMid.ShiftPersonName = ar["mid_name"].ToString();
                if (ar["mid_depart_id"] != DBNull.Value)
                {
                    laMid.DepartId = Convert.ToInt32(ar["mid_depart_id"]);
                }
                if (ar["mid_depart_name"] != DBNull.Value)
                {
                    laMid.DepartName = ar["mid_depart_name"].ToString();
                }
                if (ar["mid_word_sn"] != DBNull.Value)
                {
                    laMid.ShiftWorkSn = ar["mid_word_sn"].ToString();
                }
                if (ar["mid_principal_name"] != DBNull.Value)
                {
                    laMid.ShiftPrincipal = ar["mid_principal_name"].ToString();
                }
                if (ar["mid_class_order_name"] != DBNull.Value)
                {
                    laMid.ClassOrderName = ar["mid_class_order_name"].ToString();
                }
                if (ar["mid_class_type_name"] != DBNull.Value)
                {
                    laMid.ClassTypeName = ar["mid_class_type_name"].ToString();
                }
                #region 获取值班领导信息
                if (ar["todayleader1_person_id"] != DBNull.Value)
                {
                    laMid.OnDutyPersonId = Convert.ToInt32(ar["todayleader1_person_id"]);
                }
                if (ar["todayleader1_name"] != DBNull.Value)
                {
                    laMid.OnDutyPersonName = ar["todayleader1_name"].ToString();
                }
                if (ar["todayledaer1_word_sn"] != DBNull.Value)
                {
                    laMid.OnDutyWorkSn = ar["todayledaer1_word_sn"].ToString();
                }

                if (ar["todayleader2_person_id"] != DBNull.Value)
                {
                    laMid.OnDutyPersonId2 = Convert.ToInt32(ar["todayleader2_person_id"]);
                }
                if (ar["todayleader2_name"] != DBNull.Value)
                {
                    laMid.OnDutyPersonName2 = ar["todayleader2_name"].ToString();
                }
                if (ar["todayledaer2_word_sn"] != DBNull.Value)
                {
                    laMid.OnDutyWorkSn2 = ar["todayledaer2_word_sn"].ToString();
                }
                #endregion

                leaderList.Add(laMid);
                index++;
                #endregion

                #region 获取夜班带班领导信息
                XiGouLeaderAttend laNight = new XiGouLeaderAttend();

                laNight.index = index;
                if (ar["date_id"] != DBNull.Value)
                {
                    laNight.AttendDay = Convert.ToDateTime(ar["date_id"]);
                    laNight.AttendDayStr = Convert.ToDateTime(ar["date_id"]).ToShortDateString();
                }
                if (ar["night_person_id"] != DBNull.Value)
                {
                    laNight.ShiftPersonId = Convert.ToInt32(ar["night_person_id"]);
                }
                laNight.ShiftPersonName = ar["night_name"].ToString();
                if (ar["night_depart_id"] != DBNull.Value)
                {
                    laNight.DepartId = Convert.ToInt32(ar["night_depart_id"]);
                }
                if (ar["night_depart_name"] != DBNull.Value)
                {
                    laNight.DepartName = ar["night_depart_name"].ToString();
                }
                if (ar["night_word_sn"] != DBNull.Value)
                {
                    laNight.ShiftWorkSn = ar["night_word_sn"].ToString();
                }
                if (ar["night_principal_name"] != DBNull.Value)
                {
                    laNight.ShiftPrincipal = ar["night_principal_name"].ToString();
                }
                if (ar["night_class_order_name"] != DBNull.Value)
                {
                    laNight.ClassOrderName = ar["night_class_order_name"].ToString();
                }
                if (ar["night_class_type_name"] != DBNull.Value)
                {
                    laNight.ClassTypeName = ar["night_class_type_name"].ToString();
                }
                #region 获取值班领导信息
                if (ar["todayleader1_person_id"] != DBNull.Value)
                {
                    laNight.OnDutyPersonId = Convert.ToInt32(ar["todayleader1_person_id"]);
                }
                if (ar["todayleader1_name"] != DBNull.Value)
                {
                    laNight.OnDutyPersonName = ar["todayleader1_name"].ToString();
                }
                if (ar["todayledaer1_word_sn"] != DBNull.Value)
                {
                    laNight.OnDutyWorkSn = ar["todayledaer1_word_sn"].ToString();
                }

                if (ar["todayleader2_person_id"] != DBNull.Value)
                {
                    laNight.OnDutyPersonId2 = Convert.ToInt32(ar["todayleader2_person_id"]);
                }
                if (ar["todayleader2_name"] != DBNull.Value)
                {
                    laNight.OnDutyPersonName2 = ar["todayleader2_name"].ToString();
                }
                if (ar["todayledaer2_word_sn"] != DBNull.Value)
                {
                    laNight.OnDutyWorkSn2 = ar["todayledaer2_word_sn"].ToString();
                }
                #endregion

                leaderList.Add(laNight);
                index++;
                #endregion
            }


            return leaderList;
        }
        #endregion

        #region 获取超时时间
        /// <summary>
        /// 获取超时时间
        /// </summary>
        /// <returns></returns>
        public int GetOverTime()
        {
            int OverTime = 0;
            string sqlOverTime = string.Format(@"select * from system_param");

            DataTable dt = DbAccess.POSTGRESQL.Select(sqlOverTime, "system_param");
            if (null == dt || dt.Rows.Count < 1)
            {
                return OverTime;
            }
            OverTime = int.Parse(dt.Rows[0]["over_time"].ToString());

            return OverTime;
        }
        #endregion

        #region 获取西沟一矿带班领导出勤数据LeaderSchedule

        #region 获取西沟一矿带班领导出勤数据

        /// <summary>
        /// 获取西沟一矿带班领导出勤数据
        /// </summary>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">截止时间</param>
        /// <param name="departIdLst">部门列表</param>
        /// <returns></returns>
        [Query(HasSideEffects = true)]
        public IEnumerable<XiGouLeaderSchedule> GetXiGouLeaderScheduleRec(DateTime beginTime, DateTime endTime, int[] departIdLst)
        {
            List<XiGouLeaderSchedule> query = new List<XiGouLeaderSchedule>();
            query.Clear();
            query = GetXiGouLeaderScheduleList(beginTime, endTime);
            if (query == null || query.Count() == 0)
            {
                return null;
            }
            GetInOutWellList(beginTime, endTime, query);
            //超时时间
            int overtime = GetOverTime();

            string sql = string.Format(@"select ar.attend_record_id,p.person_id,p.name,p.work_sn,d.depart_name,d.depart_id,pri.principal_name,ar.in_well_time,ar.out_well_time,ar.work_time,ar.is_valid,ar.attend_day,con.class_order_name,ct.class_type_name from attend_record_normal as ar 
                                        left join person as p on ar.person_id=p.person_id
                                        left join depart as d on p.depart_id=d.depart_id
                                        left join principal as pri on p.principal_id=pri.principal_id
                                        left join class_order_normal as con on ar.class_order_id=con.class_order_id
                                        left join class_type as ct on con.class_type_id=ct.class_type_id where ar.attend_day >= '{0}' and ar.attend_day < '{1}'", beginTime, endTime);
            #region 选择条件是否选择了部门

            if (null != departIdLst && departIdLst.Length > 0)
            {
                sql += " and d.depart_id in ( ";
                for (int i = 0; i < departIdLst.Length - 1; i++)
                {
                    sql += departIdLst[i] + ",";
                }
                sql += departIdLst[departIdLst.Length - 1] + ")";
            }
            #endregion

            DataTable dt = DbAccess.POSTGRESQL.Select(sql, "attend_record_person");
            if (null != dt && dt.Rows.Count > 0)
            {
                foreach (DataRow ar in dt.Rows)
                {
                    foreach (var la in query)
                    {
                        if (Convert.ToDateTime(ar["attend_day"].ToString()).ToShortDateString() == la.AttendDay.ToShortDateString())
                        {
                            if (ar["person_id"] != DBNull.Value)
                            {
                                #region 如果是早班的人员
                                if (Convert.ToInt32(ar["person_id"]) == la.MornPersonId)
                                {
                                    la.MornInWellTime = "";
                                    la.MornOutWellTime = "";
                                    la.MornWorkTimeMinutes = 0;
                                    la.MornColor = "Red";
                                    if (ar["class_order_name"] != DBNull.Value && ar["class_order_name"].ToString() == la.MornClassOrderName)
                                    {
                                        la.MornColor = "Black";
                                        if (ar["in_well_time"] != DBNull.Value)
                                        {
                                            la.MornInWellTime = Convert.ToDateTime(ar["in_well_time"]).ToString("HH:mm");
                                        }
                                        if (ar["out_well_time"] != DBNull.Value)
                                        {
                                            la.MornOutWellTime = Convert.ToDateTime(ar["out_well_time"]).ToString("HH:mm");
                                        }
                                    }
                                    else
                                    {
                                        if (ar["in_well_time"] != DBNull.Value)
                                        {
                                            la.MornColor = "Red";
                                            la.MornInWellTime = Convert.ToDateTime(ar["in_well_time"]).ToString("HH:mm");
                                        }
                                        if (ar["out_well_time"] != DBNull.Value)
                                        {
                                            la.MornColor = "Red";
                                            la.MornOutWellTime = Convert.ToDateTime(ar["out_well_time"]).ToString("HH:mm");
                                        }
                                    }
                                    if (ar["work_time"] != DBNull.Value)
                                    {
                                        la.MornWorkTimeMinutes = Convert.ToInt32(ar["work_time"]);

                                    }
                                    if (ar["is_valid"] != DBNull.Value)
                                    {
                                        if (Convert.ToInt32(ar["is_valid"]) == -1 || la.MornWorkTimeMinutes > overtime)
                                        {
                                            if (la.MornWorkTimeMinutes != 0)
                                            {
                                                la.MornColor = "Red";
                                            }
                                        }
                                    }
                                }
                                #endregion
                                #region 如果是中班的人员
                                else if (Convert.ToInt32(ar["person_id"]) == la.MidPersonId)
                                {
                                    la.MidInWellTime = "";
                                    la.MidOutWellTime = "";
                                    la.MidWorkTimeMinutes = 0;
                                    la.MidColor = "Red";
                                    if (ar["class_order_name"] != DBNull.Value && ar["class_order_name"].ToString() == la.MidClassOrderName)
                                    {
                                        la.MidColor = "Black";
                                        if (ar["in_well_time"] != DBNull.Value)
                                        {
                                            la.MidInWellTime = Convert.ToDateTime(ar["in_well_time"]).ToString("HH:mm");
                                        }
                                        if (ar["out_well_time"] != DBNull.Value)
                                        {
                                            la.MidOutWellTime = Convert.ToDateTime(ar["out_well_time"]).ToString("HH:mm");
                                        }
                                    }
                                    else
                                    {
                                        if (ar["in_well_time"] != DBNull.Value)
                                        {
                                            la.MidColor = "Red";
                                            la.MidInWellTime = Convert.ToDateTime(ar["in_well_time"]).ToString("HH:mm");
                                        }
                                        if (ar["out_well_time"] != DBNull.Value)
                                        {
                                            la.MidColor = "Red";
                                            la.MidOutWellTime = Convert.ToDateTime(ar["out_well_time"]).ToString("HH:mm");
                                        }
                                    }
                                    if (ar["work_time"] != DBNull.Value)
                                    {
                                        la.MidWorkTimeMinutes = Convert.ToInt32(ar["work_time"]);

                                    }
                                    if (ar["is_valid"] != DBNull.Value)
                                    {
                                        if (Convert.ToInt32(ar["is_valid"]) == -1 || la.MidWorkTimeMinutes > overtime)
                                        {
                                            if (la.MidWorkTimeMinutes != 0)
                                            {
                                                la.MidColor = "Red";
                                            }
                                        }
                                    }
                                }
                                #endregion
                                #region 如果是夜班的人员
                                if (Convert.ToInt32(ar["person_id"]) == la.NightPersonId)
                                {
                                    la.NightInWellTime = "";
                                    la.NightOutWellTime = "";
                                    la.NightWorkTimeMinutes = 0;
                                    la.NightColor = "Red";
                                    if (ar["class_order_name"] != DBNull.Value && ar["class_order_name"].ToString() == la.NightClassOrderName)
                                    {
                                        la.NightColor = "Black";
                                        if (ar["in_well_time"] != DBNull.Value)
                                        {
                                            la.NightInWellTime = Convert.ToDateTime(ar["in_well_time"]).ToString("HH:mm");
                                        }
                                        if (ar["out_well_time"] != DBNull.Value)
                                        {
                                            la.NightOutWellTime = Convert.ToDateTime(ar["out_well_time"]).ToString("HH:mm");
                                        }
                                    }
                                    else
                                    {
                                        if (ar["in_well_time"] != DBNull.Value)
                                        {
                                            la.NightColor = "Red";
                                            la.NightInWellTime = Convert.ToDateTime(ar["in_well_time"]).ToString("HH:mm");
                                        }
                                        if (ar["out_well_time"] != DBNull.Value)
                                        {
                                            la.NightColor = "Red";
                                            la.NightOutWellTime = Convert.ToDateTime(ar["out_well_time"]).ToString("HH:mm");
                                        }
                                    }
                                    if (ar["work_time"] != DBNull.Value)
                                    {
                                        la.NightWorkTimeMinutes = Convert.ToInt32(ar["work_time"]);

                                    }
                                    if (ar["is_valid"] != DBNull.Value)
                                    {
                                        if (Convert.ToInt32(ar["is_valid"]) == -1 || la.NightWorkTimeMinutes > overtime)
                                        {
                                            if (la.NightWorkTimeMinutes != 0)
                                            {
                                                la.NightColor = "Red";
                                            }
                                        }
                                    }
                                }
                                #endregion
                            }
                        }
                    }
                }
            }
            return query;
        }
        #endregion

        /// <summary>
        /// 获取当前在岗记录
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="query"></param>
        public void GetInOutWellList(DateTime beginTime, DateTime endTime, List<XiGouLeaderSchedule> query)
        {
            string sql = string.Format(@"Select * from in_out_well where attend_day >= '{0}' and attend_day < '{1}'", beginTime, endTime);
            DataTable dt = DbAccess.POSTGRESQL.Select(sql, "in_out_well");
            if (!(null == dt || dt.Rows.Count < 1))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    foreach (var ar in query)
                    {
                        if (Convert.ToDateTime(dr["attend_day"]).ToShortDateString() == ar.AttendDayStr)
                        {
                            if (dr["person_id"] != DBNull.Value)
                            {
                                if (Convert.ToInt32(dr["person_id"]) == ar.MornPersonId)
                                {
                                    if (dr["class_order_name"].ToString() == ar.MornClassOrderName)
                                    {
                                        ar.MornColor = "Black";
                                    }
                                    ar.MornInWellTime = Convert.ToDateTime(dr["in_time"]).ToString("HH:mm");
                                }
                                else if (Convert.ToInt32(dr["person_id"]) == ar.MidPersonId)
                                {
                                    if (dr["class_order_name"].ToString() == ar.MidClassOrderName)
                                    {
                                        ar.MidColor = "Black";
                                    }
                                    ar.MidInWellTime = Convert.ToDateTime(dr["in_time"]).ToString("HH:mm");
                                }
                                else if (Convert.ToInt32(dr["person_id"]) == ar.NightPersonId)
                                {
                                    if (dr["class_order_name"].ToString() == ar.NightClassOrderName)
                                    {
                                        ar.NightColor = "Black";
                                    }
                                    ar.NightInWellTime = Convert.ToDateTime(dr["in_time"]).ToString("HH:mm");
                                }

                            }
                        }
                    }
                }
            }
        }

        #region 获取带班人员列表LeaderSchedule
        /// <summary>
        /// 获取带班人员列表
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public List<XiGouLeaderSchedule> GetXiGouLeaderScheduleList(DateTime beginTime, DateTime endTime)
        {
            List<XiGouLeaderSchedule> leaderList = new List<XiGouLeaderSchedule>();
            leaderList.Clear();
            string sqlperson = string.Format(@"select ls.date_id,
                                               p_todayleader1.person_id as todayleader1_person_id, p_todayleader1.name as todayleader1_name,p_todayleader1.depart_id as todayleader1_depart_id,p_todayleader1.depart_name as todayleader1_depart_name,p_todayleader1.work_sn as todayledaer1_word_sn,pri_todayleader1.principal_name as todayleader1_principal_name,
                                               p_todayleader2.person_id as todayleader2_person_id, p_todayleader2.name as todayleader2_name,p_todayleader2.depart_id as todayleader2_depart_id,p_todayleader2.depart_name as todayleader2_depart_name,p_todayleader2.work_sn as todayledaer2_word_sn,  pri_todayleader2.principal_name as todayleader2_principal_name,
                                               p_morn.person_id as morn_person_id, p_morn.name as morn_name,p_morn.depart_id as morn_depart_id,p_morn.depart_name as morn_depart_name,p_morn.work_sn as morn_word_sn, pri_morn.principal_name as morn_principal_name,
                                                 (select ct.class_type_name as morn_class_type_name from class_order_normal as con left join class_type as ct on con.class_type_id=ct.class_type_id where con.attend_sign like '%早%'),
                                                 (select class_order_name as morn_class_order_name from class_order_normal where attend_sign like '%早%'),
                                               p_mid.person_id as mid_person_id, p_mid.name as mid_name,p_mid.depart_id as mid_depart_id,p_mid.depart_name as mid_depart_name,p_mid.work_sn as mid_word_sn, pri_mid.principal_name as mid_principal_name,
                                                 (select ct.class_type_name as mid_class_type_name from class_order_normal as con left join class_type as ct on con.class_type_id=ct.class_type_id where con.attend_sign like '%中%'),
                                                 (select class_order_name as mid_class_order_name from class_order_normal where attend_sign like '%中%'),
                                               p_night.person_id as night_person_id, p_night.name as night_name,p_night.depart_id as night_depart_id,p_night.depart_name as night_depart_name,p_night.work_sn as night_word_sn,pri_night.principal_name as night_principal_name,
                                                 (select ct.class_type_name as night_class_type_name from class_order_normal as con left join class_type as ct on con.class_type_id=ct.class_type_id where con.attend_sign like '%夜%'),
                                                 (select class_order_name as night_class_order_name from class_order_normal where attend_sign like '%夜%')
                                               from xigou_leader_schedule as ls 
	                                            LEFT JOIN person as p_todayleader1 on ls.todayleader1=p_todayleader1.person_id	
	                                            left join principal as pri_todayleader1 on p_todayleader1.principal_id=pri_todayleader1.principal_id
	
	                                            left join person as p_todayleader2 on ls.todayleader2=p_todayleader2.person_id
	                                            left join principal as pri_todayleader2 on p_todayleader2.principal_id=pri_todayleader2.principal_id
	
	                                            left join person as p_morn on ls.morningleader=p_morn.person_id
	                                            left join principal as pri_morn on p_morn.principal_id=pri_morn.principal_id
	
	                                            left join person as p_mid on ls.midleader=p_mid.person_id
	                                            left join principal as pri_mid on p_mid.principal_id=pri_mid.principal_id
	
	                                            left join person as p_night on ls.nightleader=p_night.person_id
	                                            left join principal as pri_night on p_night.principal_id=pri_night.principal_id 
                                                where ls.date_id >='{0}' and ls.date_id < '{1}'", beginTime, endTime);

            DataTable dt = DbAccess.POSTGRESQL.Select(sqlperson, "attend_record_person");
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }
            int index = 1;
            foreach (DataRow ar in dt.Rows)
            {
                #region 获取带班领导信息
                XiGouLeaderSchedule ls = new XiGouLeaderSchedule();

                ls.index = index;
                if (ar["date_id"] != DBNull.Value)
                {
                    ls.AttendDay = Convert.ToDateTime(ar["date_id"]);
                    ls.AttendDayStr = Convert.ToDateTime(ar["date_id"]).ToShortDateString();
                }
                if (ar["morn_person_id"] != DBNull.Value)
                {
                    ls.MornPersonId = Convert.ToInt32(ar["morn_person_id"]);
                }
                ls.MornPersonName = ar["morn_name"].ToString();
                if (ar["morn_depart_id"] != DBNull.Value)
                {
                    ls.MornDepartId = Convert.ToInt32(ar["morn_depart_id"]);
                }
                if (ar["morn_depart_name"] != DBNull.Value)
                {
                    ls.MornDepartName = ar["morn_depart_name"].ToString();
                }
                if (ar["morn_class_order_name"] != DBNull.Value)
                {
                    ls.MornClassOrderName = ar["morn_class_order_name"].ToString();
                }
                if (ar["morn_class_type_name"] != DBNull.Value)
                {
                    ls.MornClassTypeName = ar["morn_class_type_name"].ToString();
                }
                if (ar["mid_person_id"] != DBNull.Value)
                {
                    ls.MidPersonId = Convert.ToInt32(ar["mid_person_id"]);
                }
                ls.MidPersonName = ar["mid_name"].ToString();
                if (ar["mid_depart_id"] != DBNull.Value)
                {
                    ls.MidDepartId = Convert.ToInt32(ar["mid_depart_id"]);
                }
                if (ar["mid_depart_name"] != DBNull.Value)
                {
                    ls.MidDepartName = ar["mid_depart_name"].ToString();
                }
                if (ar["mid_class_order_name"] != DBNull.Value)
                {
                    ls.MidClassOrderName = ar["mid_class_order_name"].ToString();
                }
                if (ar["mid_class_type_name"] != DBNull.Value)
                {
                    ls.MidClassTypeName = ar["mid_class_type_name"].ToString();
                }

                if (ar["night_person_id"] != DBNull.Value)
                {
                    ls.NightPersonId = Convert.ToInt32(ar["night_person_id"]);
                }
                ls.NightPersonName = ar["night_name"].ToString();
                if (ar["night_depart_id"] != DBNull.Value)
                {
                    ls.NightDepartId = Convert.ToInt32(ar["night_depart_id"]);
                }
                if (ar["night_depart_name"] != DBNull.Value)
                {
                    ls.NightDepartName = ar["night_depart_name"].ToString();
                }
                if (ar["night_class_order_name"] != DBNull.Value)
                {
                    ls.NightClassOrderName = ar["night_class_order_name"].ToString();
                }
                if (ar["night_class_type_name"] != DBNull.Value)
                {
                    ls.NightClassTypeName = ar["night_class_type_name"].ToString();
                }
                #endregion

                #region 获取值班领导信息
                if (ar["todayleader1_person_id"] != DBNull.Value)
                {
                    ls.OnDutyPersonId = Convert.ToInt32(ar["todayleader1_person_id"]);
                }
                if (ar["todayleader1_name"] != DBNull.Value)
                {
                    ls.OnDutyPersonName = ar["todayleader1_name"].ToString();
                }
                if (ar["todayleader2_person_id"] != DBNull.Value)
                {
                    ls.OnDutyPersonId2 = Convert.ToInt32(ar["todayleader2_person_id"]);
                }
                if (ar["todayleader2_name"] != DBNull.Value)
                {
                    ls.OnDutyPersonName2 = ar["todayleader2_name"].ToString();
                }
                #endregion
                leaderList.Add(ls);
                index++;
            }
            return leaderList;
        }
        #endregion

        #endregion

        #region 获取西沟一矿带班领导出勤明细PersonLeaderSchedule
        /// <summary>
        /// 获取西沟一矿带班领导出勤数据LeaderSchedule
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="departIdLst"></param>
        /// <param name="name"></param>
        /// <param name="personId"></param>
        /// <returns></returns>
        [Query(HasSideEffects = true)]
        public IEnumerable<XiGouLeaderAttend> GetXiGouPersonLeaderScheuleRec(DateTime beginTime, DateTime endTime, int[] departIdLst, string name, int personId)
        {
            List<XiGouLeaderAttend> laList = new List<XiGouLeaderAttend>();
            List<XiGouLeaderSchedule> lsList = new List<XiGouLeaderSchedule>();

            lsList = GetXiGouLeaderScheduleRec(beginTime, endTime, departIdLst).ToList();
            int index = 1;
            foreach (XiGouLeaderSchedule ls in lsList)
            {
                if (ls.MornPersonId != 0)
                {
                    XiGouLeaderAttend laMorn = new XiGouLeaderAttend();
                    laMorn.index = index;
                    laMorn.DepartName = ls.MornDepartName;
                    laMorn.DepartId = ls.MornDepartId;
                    laMorn.ShiftPersonId = ls.MornPersonId;
                    laMorn.ShiftPersonName = ls.MornPersonName;
                    laMorn.AttendDayStr = ls.AttendDayStr;
                    laMorn.AttendDay = ls.AttendDay;
                    laMorn.ClassTypeName = ls.MornClassTypeName;
                    laMorn.ClassOrderName = ls.MornClassOrderName;
                    laMorn.InWellTime = ls.MornInWellTime;
                    laMorn.OutWellTime = ls.MornOutWellTime;
                    if (laMorn.InWellTime != null && laMorn.InWellTime != "" && laMorn.OutWellTime != null && laMorn.OutWellTime != "")
                    {
                        //laMorn.WorkTime = (Convert.ToDateTime(laMorn.OutWellTime) - Convert.ToDateTime(laMorn.InWellTime)).ToString(@"hh\:mm");
                        DateTime s = new DateTime(1970, 1, 1);
                        laMorn.WorkTime = (s.AddMinutes(ls.MornWorkTimeMinutes) - s).ToString(@"hh\:mm");
                    }
                    laMorn.color = ls.MornColor;
                    laMorn.OnDutyPersonName = ls.OnDutyPersonName;
                    laMorn.OnDutyPersonName2 = ls.OnDutyPersonName2;
                    laList.Add(laMorn);
                    index++;

                }
                if (ls.MidPersonId != 0)
                {
                    XiGouLeaderAttend laMid = new XiGouLeaderAttend();
                    laMid.index = index;
                    laMid.DepartName = ls.MidDepartName;
                    laMid.DepartId = ls.MidDepartId;
                    laMid.ShiftPersonId = ls.MidPersonId;
                    laMid.ShiftPersonName = ls.MidPersonName;
                    laMid.AttendDayStr = ls.AttendDayStr;
                    laMid.AttendDay = ls.AttendDay;
                    laMid.ClassTypeName = ls.MidClassTypeName;
                    laMid.ClassOrderName = ls.MidClassOrderName;
                    laMid.InWellTime = ls.MidInWellTime;
                    laMid.OutWellTime = ls.MidOutWellTime;
                    if (laMid.InWellTime != null && laMid.OutWellTime != null && laMid.InWellTime != "" && laMid.OutWellTime != "")
                    {
                        //laMid.WorkTime = (Convert.ToDateTime(laMid.OutWellTime) - Convert.ToDateTime(laMid.InWellTime)).ToString(@"hh\:mm");
                        DateTime s = new DateTime(1970, 1, 1);
                        laMid.WorkTime = (s.AddMinutes(ls.MidWorkTimeMinutes) - s).ToString(@"hh\:mm");
                    }
                    laMid.color = ls.MidColor;
                    laMid.OnDutyPersonName = ls.OnDutyPersonName;
                    laMid.OnDutyPersonName2 = ls.OnDutyPersonName2;
                    laList.Add(laMid);
                    index++;
                }

                if (ls.NightPersonId != 0)
                {
                    XiGouLeaderAttend laNight = new XiGouLeaderAttend();
                    laNight.index = index;
                    laNight.DepartName = ls.NightDepartName;
                    laNight.DepartId = ls.NightDepartId;
                    laNight.ShiftPersonId = ls.NightPersonId;
                    laNight.ShiftPersonName = ls.NightPersonName;
                    laNight.AttendDayStr = ls.AttendDayStr;
                    laNight.AttendDay = ls.AttendDay;
                    laNight.ClassTypeName = ls.NightClassTypeName;
                    laNight.ClassOrderName = ls.NightClassOrderName;
                    laNight.InWellTime = ls.NightInWellTime;
                    laNight.OutWellTime = ls.NightOutWellTime;
                    if (laNight.InWellTime != null && laNight.OutWellTime != null && laNight.InWellTime != "" && laNight.OutWellTime != "" && ls.NightWorkTimeMinutes != 0)
                    {
                        DateTime s = new DateTime(1970, 1, 1);
                        laNight.WorkTime = (s.AddMinutes(ls.NightWorkTimeMinutes) - s).ToString(@"hh\:mm");

                        // laNight.WorkTime = (Convert.ToDateTime(laNight.OutWellTime) - Convert.ToDateTime(laNight.InWellTime)).ToString(@"hh\:mm");
                    }
                    laNight.color = ls.NightColor;
                    laNight.OnDutyPersonName = ls.OnDutyPersonName;
                    laNight.OnDutyPersonName2 = ls.OnDutyPersonName2;
                    laList.Add(laNight);
                    index++;
                }
            }
            if (personId != -1)
            {
                var valuePersonId = from q in laList where q.ShiftPersonId == personId orderby q.AttendDay, q.ShiftPersonName, q.DepartName select q;
                return valuePersonId;
            }
            else
            {
                var valueDepartOrName = from q in laList where (null != departIdLst && departIdLst.Length > 0) ? departIdLst.Contains(q.DepartId) : true && (name != null && name != "") ? name == q.ShiftPersonName : true orderby q.AttendDay, q.ShiftPersonName, q.DepartName select q;
                return valueDepartOrName;
            }
        }

        #endregion

        #region 获取西沟一矿领导排班数据

        /// <summary>
        /// 获取西沟一矿领导信息
        /// </summary>
        /// <returns></returns>
        [Query(HasSideEffects = true)]
        public IEnumerable<UserPersonInfo> GetLeaderPersonInfoList()
        {
            string condition = " and ptype.principal_type_name like ('领导')";//('一般工人')";
            return GetUserPersonInfoBySql(condition, "");
        }

        /// <summary>
        /// 获取西沟一矿领导排班数据
        /// </summary>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">截止时间</param>
        /// <returns></returns>
        [Query(HasSideEffects = true)]
        public IEnumerable<XiGouLeaderScheduling> GetXiGouLeaderSchedulingList(DateTime beginTime, DateTime endTime)
        {

            List<XiGouLeaderScheduling> query = new List<XiGouLeaderScheduling>();

            string[] weekDay = new string[] { "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" };
            for (int index = 0; index < (endTime - beginTime).Days; index++)
            {
                XiGouLeaderScheduling temp = null;
                temp = new XiGouLeaderScheduling();
                temp.DateId = beginTime.AddDays(index).ToString("yyyy-MM-dd");
                temp.WeekDay = weekDay[Convert.ToInt16(beginTime.AddDays(index).DayOfWeek)];

                query.Add(temp);
            }

            string sql = string.Format(@"select * from xigou_leader_schedule where date_id >= '{0}' and date_id < '{1}' order by date_id", beginTime, endTime);

            DataTable dt = DbAccess.POSTGRESQL.Select(sql, "xigou_leader_schedule");
            if (null == dt || dt.Rows.Count < 1)
            {
                return query;
            }
            foreach (DataRow ar in dt.Rows)
            {
                #region     数据填充

                DateTime attendDay = new DateTime();

                if (!DateTime.TryParse(ar["date_id"].ToString(), out attendDay))
                {
                    continue;
                }

                int dayOffset = (attendDay - beginTime).Days;
                if (ar["todayleader1"] != DBNull.Value)
                {
                    query[dayOffset].TodayLeaderId1 = Convert.ToInt32(ar["todayleader1"]);
                }

                if (ar["todayleader2"] != DBNull.Value)
                {
                    query[dayOffset].TodayLeaderId2 = Convert.ToInt32(ar["todayleader2"]);
                }

                if (ar["morningleader"] != DBNull.Value)
                {
                    query[dayOffset].MorningLeaderId = Convert.ToInt32(ar["morningleader"]);
                }

                if (ar["midleader"] != DBNull.Value)
                {
                    query[dayOffset].MidLeaderId = Convert.ToInt32(ar["midleader"]);
                }

                if (ar["nightleader"] != DBNull.Value)
                {
                    query[dayOffset].NigntLeaderId = Convert.ToInt32(ar["nightleader"]);
                }

                #endregion
            }

            return query;
        }

        /// <summary>
        ///  不能删除，否则无法更新
        /// </summary>
        /// <param name="o"></param>
        [Update]
        public void TestLeaderScheduling(XiGouLeaderScheduling o)
        {
        }

        [Invoke(HasSideEffects = true)]
        public bool SetLeaderScheduling(XiGouLeaderScheduling[] leaderSchedulingList, DateTime beginTime, DateTime endTime)
        {
            string sql = string.Format(@"Delete from xigou_leader_schedule where date_id >= '{0}' and date_id < '{1}'", beginTime, endTime);

            if (!DbAccess.POSTGRESQL.Delete(sql))
            {
                return false;
            }

            string sqlInsert = "";
            foreach (XiGouLeaderScheduling item in leaderSchedulingList)
            {
                bool bInsert = false;

                string Values = string.Format("{0}\t", item.DateId);

                if (item.TodayLeaderId1 != -1)
                {
                    bInsert = true;
                    Values += string.Format("{0}\t", item.TodayLeaderId1);
                }
                else
                {
                    Values += "\\N\t";
                }
                if (item.TodayLeaderId2 != -1)
                {
                    bInsert = true;
                    Values += string.Format("{0}\t", item.TodayLeaderId2);
                }
                else
                {
                    Values += "\\N\t";
                }
                if (item.MorningLeaderId != -1)
                {
                    bInsert = true;
                    Values += string.Format("{0}\t", item.MorningLeaderId);
                }
                else
                {
                    Values += "\\N\t";
                }
                if (item.MidLeaderId != -1)
                {
                    bInsert = true;
                    Values += string.Format("{0}\t", item.MidLeaderId);
                }
                else
                {
                    Values += "\\N\t";
                }

                if (item.NigntLeaderId != -1)
                {
                    bInsert = true;
                    Values += string.Format("{0}\n", item.NigntLeaderId);
                }
                else
                {
                    Values += "\\N\n";
                }
                if (!bInsert)
                {
                    continue;
                }

                sqlInsert += Values;
            }

            byte[] tm = System.Text.Encoding.UTF8.GetBytes(sqlInsert);
            MemoryStream ms = new MemoryStream(tm);

            if (DbAccess.POSTGRESQL.CopyIn("xigou_leader_schedule", ms) > 0)
            {
                return true;
            }
            return false;
        }

        #endregion

        #region 查询井下统计数据

        /// <summary>
        /// 查询井下统计数据
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        [Query(HasSideEffects = true)]
        public IEnumerable<XiGouInWellPerson> GetXiGouInWellList(DateTime beginTime, DateTime endTime)
        {
            List<XiGouInWellPerson> list = new List<XiGouInWellPerson>();
            list.Add(new XiGouInWellPerson() { BeginTime = beginTime.ToString("yyyy-MM-dd HH:mm:ss"), EndTime = endTime.ToString("yyyy-MM-dd HH:mm:ss") });
            string sql = string.Format(@"SELECT count(*) FROM (SELECT DISTINCT person_id FROM (SELECT person_id FROM attend_record_normal WHERE in_well_time >= '{0}' 
                                        AND in_well_time<= '{1}' UNION SELECT person_id FROM in_out_normal WHERE in_time >= '{0}' AND in_time<= '{1}') a)b",
                                        beginTime.ToString("yyyy-MM-dd HH:mm:ss"), endTime.ToString("yyyy-MM-dd HH:mm:ss"));
            DataTable dt = DbAccess.POSTGRESQL.Select(sql, "in_out_normal");

            if (null!=dt)
            {
                list[0].InWellNum = int.Parse(dt.Rows[0][0].ToString());
            }

            sql = string.Format("SELECT count(*) FROM (SELECT DISTINCT person_id FROM attend_record_normal WHERE out_well_time >= '{0}' AND out_well_time<= '{1}')a", beginTime.ToString("yyyy-MM-dd HH:mm:ss"), endTime.ToString("yyyy-MM-dd HH:mm:ss"));
            dt = DbAccess.POSTGRESQL.Select(sql, "attend_record_normal");

            if (null != dt)
            {
                list[0].OutWellNum = int.Parse(dt.Rows[0][0].ToString());
            }
            sql = string.Format(@"SELECT count(*) FROM (SELECT DISTINCT person_id FROM (SELECT person_id FROM attend_record_normal WHERE (in_well_time >= '{0}' 
                                AND in_well_time<= '{1}') OR (out_well_time >= '{0}' AND out_well_time<= '{1}') OR (in_well_time <='{0}' AND out_well_time>='{1}') UNION SELECT person_id FROM in_out_normal WHERE  in_time<= '{1}') a)b", beginTime.ToString("yyyy-MM-dd HH:mm:ss"), endTime.ToString("yyyy-MM-dd HH:mm:ss"));
            dt = DbAccess.POSTGRESQL.Select(sql, "attend_record_normal");
            if (null != dt)
            {
                list[0].DownWellNum = int.Parse(dt.Rows[0][0].ToString());
            }
            return list;
        }
        #endregion
    }
    #region 西沟一矿带班领导考勤实体数据类LeaderAttend
    /// <summary>
    /// 西沟一矿带班领导考勤实体数据类
    /// </summary>
    [DataContract]
    public class XiGouLeaderAttend
    {
        public XiGouLeaderAttend()
        {
            color = "Black";
            WorkTime = "";
            WorkTimeMinutes = -1;
        }

        /// <summary>
        /// 序号，主键
        /// </summary>
        [DataMember]
        [Key]
        public int index { get; set; }

        /// <summary>
        /// 考勤id
        /// </summary>
        [DataMember]
        public int AttendRecordId { get; set; }

        /// <summary>
        /// 带班领导personid
        /// </summary>
        [DataMember]
        public int ShiftPersonId { get; set; }

        /// <summary>
        /// 部门id
        /// </summary>
        [DataMember]
        public int DepartId { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        [DataMember]
        public string DepartName { get; set; }

        /// <summary>
        /// 带班人员姓名
        /// </summary>
        [DataMember]
        public string ShiftPersonName { get; set; }

        /// <summary>
        /// 带班人员工号
        /// </summary>
        [DataMember]
        public string ShiftWorkSn { get; set; }

        /// <summary>
        /// 带班人员工种
        /// </summary>
        [DataMember]
        public string ShiftWorkType { get; set; }

        /// <summary>
        /// 带班人员职务
        /// </summary>
        [DataMember]
        public string ShiftPrincipal { get; set; }

        /// <summary>
        /// 班次名称
        /// </summary>
        [DataMember]
        public string ClassOrderName { get; set; }

        /// <summary>
        /// 班制名称
        /// </summary>
        [DataMember]
        public string ClassTypeName { get; set; }

        /// <summary>
        /// 工时分钟数
        /// </summary>
        [DataMember]
        public int WorkTimeMinutes { get; set; }

        /// <summary>
        /// 工时
        /// </summary>
        [DataMember]
        public string WorkTime { get; set; }

        /// <summary>
        /// 颜色
        /// </summary>
        [DataMember]
        public string color { get; set; }

        /// <summary>
        /// 工数
        /// </summary>
        [DataMember]
        public int AttendTimes { get; set; }

        /// <summary>
        /// 归属日
        /// </summary>
        [DataMember]
        public DateTime AttendDay { get; set; }

        /// <summary>
        /// 归属日string
        /// </summary>
        [DataMember]
        public string AttendDayStr { get; set; }

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

        /// <summary>
        /// 值班领导personid
        /// </summary>
        [DataMember]
        public int OnDutyPersonId { get; set; }

        /// <summary>
        /// 值班人员姓名
        /// </summary>
        [DataMember]
        public string OnDutyPersonName { get; set; }

        /// <summary>
        /// 值班人员工号
        /// </summary>
        [DataMember]
        public string OnDutyWorkSn { get; set; }

        /// <summary>
        /// 值班领导personid2
        /// </summary>
        [DataMember]
        public int OnDutyPersonId2 { get; set; }

        /// <summary>
        /// 值班人员姓名2
        /// </summary>
        [DataMember]
        public string OnDutyPersonName2 { get; set; }

        /// <summary>
        /// 值班人员工号2
        /// </summary>
        [DataMember]
        public string OnDutyWorkSn2 { get; set; }

    }

    #endregion

    #region 西沟一矿带班领导考勤实体数据类LeaderSchedule
    /// <summary>
    /// 西沟一矿带班领导考勤实体数据类LeaderSchedule
    /// </summary>
    [DataContract]
    public class XiGouLeaderSchedule
    {
        public XiGouLeaderSchedule()
        {
            MornColor = "Red";
            MidColor = "Red";
            NightColor = "Red";
            CompanyName = "西沟一矿";
        }

        /// <summary>
        /// 序号，主键
        /// </summary>
        [DataMember]
        [Key]
        public int index { get; set; }

        #region 早班人员信息

        /// <summary>
        /// 带班领导personid
        /// </summary>
        [DataMember]
        public int MornPersonId { get; set; }

        /// <summary>
        /// 部门id
        /// </summary>
        [DataMember]
        public int MornDepartId { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        [DataMember]
        public string MornDepartName { get; set; }

        /// <summary>
        /// 带班人员姓名
        /// </summary>
        [DataMember]
        public string MornPersonName { get; set; }

        /// <summary>
        /// 带班人员工号
        /// </summary>
        [DataMember]
        public string MornWorkSn { get; set; }

        /// <summary>
        /// 班次名称
        /// </summary>
        [DataMember]
        public string MornClassOrderName { get; set; }

        /// <summary>
        /// 班制名称
        /// </summary>
        [DataMember]
        public string MornClassTypeName { get; set; }

        /// <summary>
        /// 上班时间
        /// </summary>
        [DataMember]
        public string MornInWellTime { get; set; }

        /// <summary>
        /// 下班时间
        /// </summary>
        [DataMember]
        public string MornOutWellTime { get; set; }

        /// <summary>
        /// 工时分钟数
        /// </summary>
        [DataMember]
        public int MornWorkTimeMinutes { get; set; }

        /// <summary>
        /// 颜色
        /// </summary>
        [DataMember]
        public string MornColor { get; set; }

        #endregion

        #region 中班人员信息

        /// <summary>
        /// 带班领导personid
        /// </summary>
        [DataMember]
        public int MidPersonId { get; set; }

        /// <summary>
        /// 部门id
        /// </summary>
        [DataMember]
        public int MidDepartId { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        [DataMember]
        public string MidDepartName { get; set; }

        /// <summary>
        /// 带班人员姓名
        /// </summary>
        [DataMember]
        public string MidPersonName { get; set; }

        /// <summary>
        /// 带班人员工号
        /// </summary>
        [DataMember]
        public string MidWorkSn { get; set; }

        /// <summary>
        /// 班次名称
        /// </summary>
        [DataMember]
        public string MidClassOrderName { get; set; }

        /// <summary>
        /// 班制名称
        /// </summary>
        [DataMember]
        public string MidClassTypeName { get; set; }

        /// <summary>
        /// 上班时间
        /// </summary>
        [DataMember]
        public string MidInWellTime { get; set; }

        /// <summary>
        /// 下班时间
        /// </summary>
        [DataMember]
        public string MidOutWellTime { get; set; }

        /// <summary>
        /// 工时分钟数
        /// </summary>
        [DataMember]
        public int MidWorkTimeMinutes { get; set; }

        /// <summary>
        /// 颜色
        /// </summary>
        [DataMember]
        public string MidColor { get; set; }

        #endregion

        #region 夜班人员信息

        /// <summary>
        /// 带班领导personid
        /// </summary>
        [DataMember]
        public int NightPersonId { get; set; }

        /// <summary>
        /// 部门id
        /// </summary>
        [DataMember]
        public int NightDepartId { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        [DataMember]
        public string NightDepartName { get; set; }

        /// <summary>
        /// 带班人员姓名
        /// </summary>
        [DataMember]
        public string NightPersonName { get; set; }

        /// <summary>
        /// 带班人员工号
        /// </summary>
        [DataMember]
        public string NightWorkSn { get; set; }

        /// <summary>
        /// 班次名称
        /// </summary>
        [DataMember]
        public string NightClassOrderName { get; set; }

        /// <summary>
        /// 班制名称
        /// </summary>
        [DataMember]
        public string NightClassTypeName { get; set; }

        /// <summary>
        /// 上班时间
        /// </summary>
        [DataMember]
        public string NightInWellTime { get; set; }

        /// <summary>
        /// 下班时间
        /// </summary>
        [DataMember]
        public string NightOutWellTime { get; set; }

        /// <summary>
        /// 工时分钟数
        /// </summary>
        [DataMember]
        public int NightWorkTimeMinutes { get; set; }

        /// <summary>
        /// 颜色
        /// </summary>
        [DataMember]
        public string NightColor { get; set; }

        #endregion

        #region 带班人员公共信息

        /// <summary>
        /// 单位名称
        /// </summary>
        [DataMember]
        public string CompanyName { get; set; }

        /// <summary>
        /// 归属日
        /// </summary>
        [DataMember]
        public DateTime AttendDay { get; set; }

        /// <summary>
        /// 归属日string
        /// </summary>
        [DataMember]
        public string AttendDayStr { get; set; }

        /// <summary>
        /// 值班领导personid
        /// </summary>
        [DataMember]
        public int OnDutyPersonId { get; set; }

        /// <summary>
        /// 值班人员姓名
        /// </summary>
        [DataMember]
        public string OnDutyPersonName { get; set; }

        /// <summary>
        /// 值班领导personid2
        /// </summary>
        [DataMember]
        public int OnDutyPersonId2 { get; set; }

        /// <summary>
        /// 值班人员姓名2
        /// </summary>
        [DataMember]
        public string OnDutyPersonName2 { get; set; }

        #endregion

    }

    #endregion

    #region  西沟一矿领导排班实体数据类
    /// <summary>
    /// 西沟一矿领导排班实体数据类
    /// </summary>
    [DataContract]
    public class XiGouLeaderScheduling
    {
        /// <summary>
        /// 日期
        /// </summary>
        [DataMember]
        [Key]
        public string DateId { get; set; }

        /// <summary>
        /// 星期
        /// </summary>
        [DataMember]
        public string WeekDay { get; set; }

        /// <summary>
        /// 当天值班领导1personid
        /// </summary>
        [DataMember]
        public int TodayLeaderId1 { get; set; }

        /// <summary>
        /// 当天值班领导2personid
        /// </summary>
        [DataMember]
        public int TodayLeaderId2 { get; set; }

        /// <summary>
        /// 早班值班领导personid
        /// </summary>
        [DataMember]
        public int MorningLeaderId { get; set; }

        /// <summary>
        /// 中班值班领导personid
        /// </summary>
        [DataMember]
        public int MidLeaderId { get; set; }

        /// <summary>
        /// 晚班值班领导personid
        /// </summary>
        [DataMember]
        public int NigntLeaderId { get; set; }

        public XiGouLeaderScheduling()
        {
            TodayLeaderId1 = -1;
            TodayLeaderId2 = -1;
            MorningLeaderId = -1;
            MidLeaderId = -1;
            NigntLeaderId = -1;
        }

    }
    
    #endregion

    #region 西沟一矿井下人员统计实体类

    /// <summary>
    /// 西沟一矿井下人员统计
    /// </summary>
    [DataContract]
    public class XiGouInWellPerson
    {
        /// <summary>
        /// 开始时间
        /// </summary>
        [DataMember]
        [Key]
        public string BeginTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        [DataMember]
        public string EndTime { get; set; }
        /// <summary>
        /// 入井数
        /// </summary>
        [DataMember]
        public int InWellNum { get; set; }
        /// <summary>
        /// 井下数
        /// </summary>
        [DataMember]
        public int DownWellNum { get; set; }
        /// <summary>
        /// 升井数
        /// </summary>
        [DataMember]
        public int OutWellNum { get; set; }
    }
    #endregion

}