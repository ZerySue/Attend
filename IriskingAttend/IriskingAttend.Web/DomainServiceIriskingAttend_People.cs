/*************************************************************************
** 文件名:   DomainServiceIriskingAttend_People.cs
** 主要类:   DomainServiceIriskingAttend
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   wz
** 日  期:   2013-4-9
** 修改人:   wz 代码优化 szr 增加 记工时班次 增删改功能
** 日  期:   2013-7-24  2014-11-19
** 描  述:   人员信息管理的域服务
**
** 版  本:   1.0.0
** 备  注:  命名及代码编写遵守C#编码规范
**
 * ***********************************************************************/
using IriskingAttend.Web.Manager;


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
    using System.Threading;
    using System.Text;
    using System.Windows.Forms;
    using ServerCommunicationLib;
    using IriskingAttend.Web.WuHuShan;


    // TODO: 创建包含应用程序逻辑的方法。
    public partial class DomainServiceIriskingAttend
    {

        #region 查询函数
        
        /// <summary>
        /// 检验停用虹膜人员列表 by蔡天雨
        /// </summary>
        /// <param name="persoonId"></param>
        /// <returns></returns>
        [Query(HasSideEffects = true)]
        public IEnumerable<PersonStopIrisInfo> GetPersonStopIrisTable(int[] persoonId)
        {
            string sql = @"select pdi.person_disable_info_id,pb.work_sn,pb.name,pdi.begin_time,pdi.end_time,pdi.policy,pb.person_id from person_disable_info as pdi
            left join person_base as pb on pb.person_id=pdi.person_id where pb.person_id in ( ";
            //DELETE FROM person_disable_info WHERE person_id = xxx;
            sql += persoonId[0];
            for (int i = 1; i < persoonId.Length; i++)
            {
                sql += "," + persoonId[i];
            }
            

            sql += ")";
            //            string sql = string.Format(@"select pdi.person_disable_info_id,pb.work_sn,pb.name,pdi.begin_time,pdi.end_time,pdi.policy,pb.person_id from person_disable_info as pdi
            //            left join person_base as pb on pb.person_id=pdi.person_id where pb.person_id={0}", persoonId);
            //UserPersonInfo UserPersonInfo = new UserPersonInfo();
            DataTable dt = DbAccess.POSTGRESQL.Select(sql, "person_stop_info");
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }
            List<PersonStopIrisInfo> query = new List<PersonStopIrisInfo>();

            foreach (DataRow ar in dt.Rows)
            {
                PersonStopIrisInfo personStopIrisInfo = new PersonStopIrisInfo();
                // personStopIrisInfo.Clear();
                #region     数据填充
                if (ar["person_disable_info_id"] != DBNull.Value)
                {
                    personStopIrisInfo.person_disable_info_id = Convert.ToInt32(ar["person_disable_info_id"]);
                }
                if (ar["work_sn"] != DBNull.Value)
                {
                    personStopIrisInfo.work_sn = (ar["work_sn"].ToString());
                }
                if (ar["name"] != DBNull.Value)
                {
                    personStopIrisInfo.person_name = ar["name"].ToString();
                }
                if (ar["begin_time"] != DBNull.Value)
                {
                    personStopIrisInfo.begin_time = Convert.ToDateTime(ar["begin_time"]).ToString();
                    //personStopIrisInfo.begin_time = Convert.ToDateTime(personStopIrisInfo.begin_time.ToString("yyyy-MM-dd"));
                }
                if (ar["end_time"] != DBNull.Value)
                {
                    personStopIrisInfo.end_time = Convert.ToDateTime(ar["end_time"]).ToString();
                }
                if (ar["policy"] != DBNull.Value)
                {
                    switch (Convert.ToInt32(ar["policy"]))
                    {
                        case 0: personStopIrisInfo.policy = "自动变为启用";
                            break;
                        case 1: personStopIrisInfo.policy = "删除该用户虹膜信息";
                            break;
                        case 2: personStopIrisInfo.policy = "删除该用户人员信息";
                            break;
                    }
                }
                if (ar["person_id"] != DBNull.Value)
                {
                    personStopIrisInfo.person_id = Convert.ToInt32(ar["person_id"]);
                }
                personStopIrisInfo.index = query.Count + 1;
                query.Add(personStopIrisInfo);
                #endregion
            }

            return query;
        }


        /// <summary>
        /// 获取停用虹膜人员列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PersonStopIrisInfo> GetPersonStopIrisInfo()
        {
            string sql = string.Format(@"select pdi.person_disable_info_id,pb.work_sn,pb.name,pdi.begin_time,pdi.end_time,pdi.policy,pb.person_id from person_disable_info as pdi
            left join person_base as pb on pb.person_id=pdi.person_id");


            //UserPersonInfo UserPersonInfo = new UserPersonInfo();
            DataTable dt = DbAccess.POSTGRESQL.Select(sql, "user_person_info");
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }


            List<PersonStopIrisInfo> query = new List<PersonStopIrisInfo>();
            foreach (DataRow ar in dt.Rows)
            {
                PersonStopIrisInfo personStopIrisInfo = new PersonStopIrisInfo();
                #region     数据填充
                if (ar["person_disable_info_id"] != DBNull.Value)
                {
                    personStopIrisInfo.person_disable_info_id = Convert.ToInt32(ar["person_disable_info_id"]);
                }
                if (ar["work_sn"] != DBNull.Value)
                {
                    personStopIrisInfo.work_sn = ar["work_sn"].ToString();
                }
                if (ar["name"] != DBNull.Value)
                {
                    personStopIrisInfo.person_name = ar["name"].ToString();
                }
                if (ar["begin_time"] != DBNull.Value)
                {
                    personStopIrisInfo.begin_time = Convert.ToDateTime(ar["begin_time"]).ToShortDateString().ToString();
                }
                if (ar["end_time"] != DBNull.Value)
                {
                    personStopIrisInfo.end_time = Convert.ToDateTime(ar["end_time"]).ToShortDateString().ToString();
                }
                if (ar["policy"] != DBNull.Value)
                {
                    switch (Convert.ToInt32(ar["policy"]))
                    {
                        case 0: personStopIrisInfo.policy = "自动变为启用";
                            break;
                        case 1: personStopIrisInfo.policy = "删除该用户虹膜信息";
                            break;
                        case 2: personStopIrisInfo.policy = "删除该用户人员信息";
                            break;
                    }
                }
                if (ar["person_id"] != DBNull.Value)
                {
                    personStopIrisInfo.person_id = Convert.ToInt32(ar["person_id"]);
                }
                query.Add(personStopIrisInfo);
                #endregion
            }

            return query;
        }

        /// <summary>
        /// 获取班次信息
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UserClassOrderInfo> GetClassOrderInfos()
        {
            List<UserClassOrderInfo> query = new List<UserClassOrderInfo>();
            string sql = string.Format(@"SELECT  cob.class_order_id, cos.avail_time, cos.work_cnt, class_order_name, in_well_start_time, in_well_end_time, 
                       out_well_start_time, out_well_end_time, cob.class_type_id, attend_sign, 
                       attend_off_minutes,  cob.memo,  attend_max_minutes, 
                       max_minutes_valid, attend_latest_worktime,
                       latest_worktime_valid, ct.class_type_name
                       FROM class_order_normal as cob
                       left join " + "\"class_order.standard\"" + @" as cos on cob.class_order_id = cos.class_order_id
                       left join class_type as ct on ct.class_type_id = cob.class_type_id;");
            DataTable dt = DbAccess.POSTGRESQL.Select(sql, "class_order_info");
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }


            foreach (DataRow ar in dt.Rows)
            {
                UserClassOrderInfo userClassOrderInfo = new UserClassOrderInfo();
                //如果可为空需要判断   if(ar["attend_record_id"].ToString() != "")
                //或者 if(ar["attend_record_id"] != DBNull.Value)
                #region         数据填充
                if (ar["latest_worktime_valid"] != DBNull.Value)
                {
                    userClassOrderInfo.latest_worktime_valid = Convert.ToInt16(ar["latest_worktime_valid"]);
                }
                if (ar["max_minutes_valid"] != DBNull.Value)
                {
                    userClassOrderInfo.max_minutes_valid = Convert.ToInt16(ar["max_minutes_valid"]);
                }

                if (ar["attend_latest_worktime"] != DBNull.Value)
                {
                    userClassOrderInfo.attend_latest_worktime = Convert.ToInt16(ar["attend_latest_worktime"]);
                    if (userClassOrderInfo.latest_worktime_valid > 0)
                    {
                        userClassOrderInfo.attend_latest_worktime_str = Helper.MinutesToDate(userClassOrderInfo.attend_latest_worktime);
                    }
                    else
                    {
                        userClassOrderInfo.attend_latest_worktime_str = "无效";
                    }
                }
                if (ar["attend_max_minutes"] != DBNull.Value)
                {
                    userClassOrderInfo.attend_max_minutes = Convert.ToInt16(ar["attend_max_minutes"]);
                    if (userClassOrderInfo.max_minutes_valid > 0)
                    {
                        userClassOrderInfo.attend_max_minutes_str = Helper.MinutesToTimeLenth(userClassOrderInfo.attend_max_minutes);
                    }
                    else
                    {
                        userClassOrderInfo.attend_max_minutes_str = "无效";
                    }
                }
                if (ar["attend_off_minutes"] != DBNull.Value)
                {
                    userClassOrderInfo.attend_off_minutes = Convert.ToInt16(ar["attend_off_minutes"]);
                    userClassOrderInfo.attend_off_minutes_str = Helper.GetAttendBelongDay(userClassOrderInfo.attend_off_minutes);
                }
                if (ar["attend_sign"] != DBNull.Value)
                {
                    userClassOrderInfo.attend_sign = Convert.ToString(ar["attend_sign"]);
                }
                if (ar["avail_time"] != DBNull.Value)
                {
                    userClassOrderInfo.avail_time_linear = Convert.ToInt16(ar["avail_time"]);
                    userClassOrderInfo.avail_time_linear_str = Helper.MinutesToDate(userClassOrderInfo.avail_time_linear, false);
                }
                if (ar["class_order_id"] != DBNull.Value)
                {
                    userClassOrderInfo.class_order_id = Convert.ToInt32(ar["class_order_id"]);
                }
                if (ar["class_order_name"] != DBNull.Value)
                {
                    userClassOrderInfo.class_order_name = Convert.ToString(ar["class_order_name"]);
                }
                if (ar["class_type_id"] != DBNull.Value)
                {
                    userClassOrderInfo.class_type_id = Convert.ToInt32(ar["class_type_id"]);
                }
                if (ar["class_type_name"] != DBNull.Value)
                {
                    userClassOrderInfo.class_type_name = Convert.ToString(ar["class_type_name"]);
                }
                if (ar["in_well_end_time"] != DBNull.Value)
                {
                    userClassOrderInfo.in_well_end_time = Convert.ToInt16(ar["in_well_end_time"]);
                    userClassOrderInfo.in_well_end_time_str = Helper.MinutesToDate(userClassOrderInfo.in_well_end_time);
                }
                if (ar["in_well_start_time"] != DBNull.Value)
                {
                    userClassOrderInfo.in_well_start_time = Convert.ToInt16(ar["in_well_start_time"]);
                    userClassOrderInfo.in_well_start_time_str = Helper.MinutesToDate(userClassOrderInfo.in_well_start_time);
                }

                if (ar["memo"] != DBNull.Value)
                {
                    userClassOrderInfo.memo = Convert.ToString(ar["memo"]);
                }
                if (ar["out_well_end_time"] != DBNull.Value)
                {
                    userClassOrderInfo.out_well_end_time = Convert.ToInt16(ar["out_well_end_time"]);
                    userClassOrderInfo.out_well_end_time_str = Helper.MinutesToDate(userClassOrderInfo.out_well_end_time);
                }
                if (ar["out_well_start_time"] != DBNull.Value)
                {
                    userClassOrderInfo.out_well_start_time = Convert.ToInt16(ar["out_well_start_time"]);
                    userClassOrderInfo.out_well_start_time_str = Helper.MinutesToDate(userClassOrderInfo.out_well_start_time);
                }
                if (ar["work_cnt"] != DBNull.Value)
                {
                    userClassOrderInfo.work_cnt_linear = Convert.ToInt32(ar["work_cnt"]);
                    userClassOrderInfo.work_cnt_linear_str = Helper.GetWorkCnt(userClassOrderInfo.work_cnt_linear);
                }
                #endregion

                //如果是按时间段记工
                if (userClassOrderInfo.work_cnt_linear == -1 &&
                    userClassOrderInfo.avail_time_linear == -1 &&
                    userClassOrderInfo.class_order_id != -1)
                {
                    //重新查询在class_order.standard表里
                    string sql_select = string.Format(@"SELECT class_order_id, avail_time, work_cnt
                        FROM " + "\"class_order.timeduration\"" +
                          "where class_order_id = {0};", userClassOrderInfo.class_order_id);
                    DataTable dt_timeduration = DbAccess.POSTGRESQL.Select(sql_select, "");
                    if (dt_timeduration != null && dt_timeduration.Rows.Count > 0)
                    {
                        userClassOrderInfo.is_count_workcnt_by_timeduration = true;
                        userClassOrderInfo.is_count_workcnt_by_timeduration_str = " [是]";
                        userClassOrderInfo.avail_time_timeduration = new short[dt_timeduration.Rows.Count];
                        userClassOrderInfo.work_cnt_timeduration = new int[dt_timeduration.Rows.Count];
                        userClassOrderInfo.avail_time_timeduration_str = new string[dt_timeduration.Rows.Count];
                        userClassOrderInfo.work_cnt_timeduration_str = new string[dt_timeduration.Rows.Count];

                        for (int i = 0; i < dt_timeduration.Rows.Count; i++)
                        {
                            if (dt_timeduration.Rows[i]["avail_time"] != DBNull.Value)
                            {
                                userClassOrderInfo.avail_time_timeduration[i] = Convert.ToInt16(dt_timeduration.Rows[i]["avail_time"]);
                                userClassOrderInfo.avail_time_timeduration_str[i] = Helper.MinutesToDate(userClassOrderInfo.avail_time_timeduration[i], false);

                            }
                            if (dt_timeduration.Rows[i]["work_cnt"] != DBNull.Value)
                            {
                                userClassOrderInfo.work_cnt_timeduration[i] = Convert.ToInt32(dt_timeduration.Rows[i]["work_cnt"]);
                                userClassOrderInfo.work_cnt_timeduration_str[i] = Helper.GetWorkCnt(userClassOrderInfo.work_cnt_timeduration[i]);
                            }
                        }
                    }
                }

                userClassOrderInfo.index = query.Count + 1;

                query.Add(userClassOrderInfo);
            }

            return query;
        }

        /// <summary>
        /// 获取班次信息
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UserClassOrderInfo> GetClassOrderInfosByClassType(int classTypeId)
        {
            List<UserClassOrderInfo> query = new List<UserClassOrderInfo>();
            string sql = string.Format(@"SELECT  cob.class_order_id, cos.avail_time, cos.work_cnt, class_order_name, in_well_start_time, in_well_end_time, 
                       out_well_start_time, out_well_end_time, cob.class_type_id, attend_sign, 
                       attend_off_minutes,  cob.memo,  attend_max_minutes, 
                       max_minutes_valid, attend_latest_worktime,
                       latest_worktime_valid, ct.class_type_name
                       FROM class_order_normal as cob
                       left join " + "\"class_order.standard\"" + @" as cos on cob.class_order_id = cos.class_order_id
                       left join class_type as ct on ct.class_type_id = cob.class_type_id
                       where cob.class_type_id = " + classTypeId +";");

            

            DataTable dt = DbAccess.POSTGRESQL.Select(sql, "class_order_info");
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }


            foreach (DataRow ar in dt.Rows)
            {
                UserClassOrderInfo userClassOrderInfo = new UserClassOrderInfo();
                //如果可为空需要判断   if(ar["attend_record_id"].ToString() != "")
                //或者 if(ar["attend_record_id"] != DBNull.Value)
                #region         数据填充
                if (ar["latest_worktime_valid"] != DBNull.Value)
                {
                    userClassOrderInfo.latest_worktime_valid = Convert.ToInt16(ar["latest_worktime_valid"]);
                }
                if (ar["max_minutes_valid"] != DBNull.Value)
                {
                    userClassOrderInfo.max_minutes_valid = Convert.ToInt16(ar["max_minutes_valid"]);
                }

                if (ar["attend_latest_worktime"] != DBNull.Value)
                {
                    userClassOrderInfo.attend_latest_worktime = Convert.ToInt16(ar["attend_latest_worktime"]);
                    if (userClassOrderInfo.latest_worktime_valid > 0)
                    {
                        userClassOrderInfo.attend_latest_worktime_str = Helper.MinutesToDate(userClassOrderInfo.attend_latest_worktime);
                    }
                    else
                    {
                        userClassOrderInfo.attend_latest_worktime_str = "无效";
                    }
                }
                if (ar["attend_max_minutes"] != DBNull.Value)
                {
                    userClassOrderInfo.attend_max_minutes = Convert.ToInt16(ar["attend_max_minutes"]);
                    if (userClassOrderInfo.max_minutes_valid > 0)
                    {
                        userClassOrderInfo.attend_max_minutes_str = Helper.MinutesToTimeLenth(userClassOrderInfo.attend_max_minutes);
                    }
                    else
                    {
                        userClassOrderInfo.attend_max_minutes_str = "无效";
                    }
                }
                if (ar["attend_off_minutes"] != DBNull.Value)
                {
                    userClassOrderInfo.attend_off_minutes = Convert.ToInt16(ar["attend_off_minutes"]);
                    userClassOrderInfo.attend_off_minutes_str = Helper.GetAttendBelongDay(userClassOrderInfo.attend_off_minutes);
                }
                if (ar["attend_sign"] != DBNull.Value)
                {
                    userClassOrderInfo.attend_sign = Convert.ToString(ar["attend_sign"]);
                }
                if (ar["avail_time"] != DBNull.Value)
                {
                    userClassOrderInfo.avail_time_linear = Convert.ToInt16(ar["avail_time"]);
                    userClassOrderInfo.avail_time_linear_str = Helper.MinutesToDate(userClassOrderInfo.avail_time_linear, false);
                }
                if (ar["class_order_id"] != DBNull.Value)
                {
                    userClassOrderInfo.class_order_id = Convert.ToInt32(ar["class_order_id"]);
                }
                if (ar["class_order_name"] != DBNull.Value)
                {
                    userClassOrderInfo.class_order_name = Convert.ToString(ar["class_order_name"]);
                }
                if (ar["class_type_id"] != DBNull.Value)
                {
                    userClassOrderInfo.class_type_id = Convert.ToInt32(ar["class_type_id"]);
                }
                if (ar["class_type_name"] != DBNull.Value)
                {
                    userClassOrderInfo.class_type_name = Convert.ToString(ar["class_type_name"]);
                }
                if (ar["in_well_end_time"] != DBNull.Value)
                {
                    userClassOrderInfo.in_well_end_time = Convert.ToInt16(ar["in_well_end_time"]);
                    userClassOrderInfo.in_well_end_time_str = Helper.MinutesToDate(userClassOrderInfo.in_well_end_time);
                }
                if (ar["in_well_start_time"] != DBNull.Value)
                {
                    userClassOrderInfo.in_well_start_time = Convert.ToInt16(ar["in_well_start_time"]);
                    userClassOrderInfo.in_well_start_time_str = Helper.MinutesToDate(userClassOrderInfo.in_well_start_time);
                }

                if (ar["memo"] != DBNull.Value)
                {
                    userClassOrderInfo.memo = Convert.ToString(ar["memo"]);
                }
                if (ar["out_well_end_time"] != DBNull.Value)
                {
                    userClassOrderInfo.out_well_end_time = Convert.ToInt16(ar["out_well_end_time"]);
                    userClassOrderInfo.out_well_end_time_str = Helper.MinutesToDate(userClassOrderInfo.out_well_end_time);
                }
                if (ar["out_well_start_time"] != DBNull.Value)
                {
                    userClassOrderInfo.out_well_start_time = Convert.ToInt16(ar["out_well_start_time"]);
                    userClassOrderInfo.out_well_start_time_str = Helper.MinutesToDate(userClassOrderInfo.out_well_start_time);
                }
                if (ar["work_cnt"] != DBNull.Value)
                {
                    userClassOrderInfo.work_cnt_linear = Convert.ToInt32(ar["work_cnt"]);
                    userClassOrderInfo.work_cnt_linear_str = Helper.GetWorkCnt(userClassOrderInfo.work_cnt_linear);
                }
                #endregion

                //如果是按时间段记工
                if (userClassOrderInfo.work_cnt_linear == -1 &&
                    userClassOrderInfo.avail_time_linear == -1 &&
                    userClassOrderInfo.class_order_id != -1)
                {
                    //重新查询在class_order.standard表里
                    string sql_select = string.Format(@"SELECT class_order_id, avail_time, work_cnt
                        FROM " + "\"class_order.timeduration\"" +
                          "where class_order_id = {0};", userClassOrderInfo.class_order_id);
                    DataTable dt_timeduration = DbAccess.POSTGRESQL.Select(sql_select, "");
                    if (dt_timeduration != null && dt_timeduration.Rows.Count > 0)
                    {
                        userClassOrderInfo.is_count_workcnt_by_timeduration = true;
                        userClassOrderInfo.is_count_workcnt_by_timeduration_str = " [是]";
                        userClassOrderInfo.avail_time_timeduration = new short[dt_timeduration.Rows.Count];
                        userClassOrderInfo.work_cnt_timeduration = new int[dt_timeduration.Rows.Count];
                        userClassOrderInfo.avail_time_timeduration_str = new string[dt_timeduration.Rows.Count];
                        userClassOrderInfo.work_cnt_timeduration_str = new string[dt_timeduration.Rows.Count];

                        for (int i = 0; i < dt_timeduration.Rows.Count; i++)
                        {
                            if (dt_timeduration.Rows[i]["avail_time"] != DBNull.Value)
                            {
                                userClassOrderInfo.avail_time_timeduration[i] = Convert.ToInt16(dt_timeduration.Rows[i]["avail_time"]);
                                userClassOrderInfo.avail_time_timeduration_str[i] = Helper.MinutesToDate(userClassOrderInfo.avail_time_timeduration[i], false);

                            }
                            if (dt_timeduration.Rows[i]["work_cnt"] != DBNull.Value)
                            {
                                userClassOrderInfo.work_cnt_timeduration[i] = Convert.ToInt32(dt_timeduration.Rows[i]["work_cnt"]);
                                userClassOrderInfo.work_cnt_timeduration_str[i] = Helper.GetWorkCnt(userClassOrderInfo.work_cnt_timeduration[i]);
                            }
                        }
                    }
                }

                userClassOrderInfo.index = query.Count + 1;

                query.Add(userClassOrderInfo);
            }

            return query;
        }

        /// <summary>
        /// 获取班制信息
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UserClassTypeInfo> GetClassTypeInfos()
        {
            List<UserClassTypeInfo> query = new List<UserClassTypeInfo>();
            DataTable dt = DbAccess.POSTGRESQL.Select("SELECT * FROM class_type where class_type_id not in (select distinct class_type_id from class_order_normal ) and delete_info is null order by convert_to(class_type_name,  E'GBK');", "user_class_type_info");
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }

            foreach (DataRow ar in dt.Rows)
            {
                UserClassTypeInfo userClassTypeInfo = new UserClassTypeInfo();
                //如果可为空需要判断   if(ar["attend_record_id"].ToString() != "")
                //或者 if(ar["attend_record_id"] != DBNull.Value)
                #region         数据填充
                if (ar["class_type_id"] != DBNull.Value)
                {
                    userClassTypeInfo.class_type_id = Convert.ToInt32(ar["class_type_id"]);
                }
                if (ar["class_type_name"] != DBNull.Value)
                {
                    userClassTypeInfo.class_type_name = Convert.ToString(ar["class_type_name"]);
                }
                if (ar["class_type"] != DBNull.Value)
                {
                    userClassTypeInfo.class_type = Convert.ToInt16(ar["class_type"]);
                }
                if (ar["delete_info"] != DBNull.Value)
                {
                    userClassTypeInfo.delete_info = Convert.ToInt16(ar["delete_info"]);
                }
                if (ar["memo"] != DBNull.Value)
                {
                    userClassTypeInfo.memo = Convert.ToString(ar["memo"]);
                }

                #endregion
                userClassTypeInfo.index = query.Count + 1;

                query.Add(userClassTypeInfo);
            }

            return query;
        }


        /// <summary>
        /// wz
        /// 获取人员信息表
        /// </summary>
        /// <param name="depart_Name">查询条件 部门id -1代表查找全部部门</param>
        /// <param name="person_Name">查询条件 人员名字</param>
        /// <param name="person_WorkSn">查询条件 人员工号</param>
        /// <param name="irisStatus">查询条件 虹膜状态</param>
        /// <param name="irisRegister">查询条件 虹膜注册信息</param>
        /// <param name="childDepartMode">查询条件 是否递归查询子部门</param>
        /// <returns>人员信息表</returns>
        public IEnumerable<UserPersonInfo> GetPersonsInfoTable(int depart_id, string person_Name, string person_WorkSn,
            string irisStatus, string irisRegister, string childDepartMode)
        {
            //组装sql语句的where条件
            //人员姓名
            string sql_where_personName = "";
            if (person_Name != null && !person_Name.Equals("") && !person_Name.Equals("全部"))
            {
                sql_where_personName = string.Format(" and pb.name LIKE '%{0}%' ", person_Name);
            }
            //人员工号
            string sql_where_personWorkSn = "";
            if (person_WorkSn != null && !person_WorkSn.Equals("") && !person_WorkSn.Equals("全部"))
            {
                sql_where_personWorkSn = string.Format(" and pb.work_sn = '{0}' ", person_WorkSn);
            }
            //虹膜状态
            string sql_where_irisStatus = "";
            if (irisStatus != null && !irisStatus.Equals("") && !irisStatus.Equals("全部"))
            {
                string time_now = System.DateTime.Now.ToString();
                if (irisStatus.Equals("启用"))
                {
                    sql_where_irisStatus = string.Format(@" and (pdi.person_disable_info_id is null or 
                        pdi.begin_time > '{0}' or pdi.end_time < '{1}')  ", time_now, time_now);
                }
                else
                {
                    sql_where_irisStatus = string.Format(@" and (pdi.person_disable_info_id is not null and pdi.begin_time < '{0}' and
                            (pdi.end_time > '{1}' or pdi.end_time is null) ) ", time_now, time_now);
                }
            }

            string sql_where = sql_where = string.Format("{0}{1}{2}", sql_where_irisStatus, sql_where_personName, sql_where_personWorkSn);

            //组装sql语句的with条件
            string sql_conditon = Helper.GetConditionSql(depart_id, childDepartMode, irisRegister);

            //如果是全部查询条件则无部门的人 也要能查出来   
            string depart_nameSql =" and pb.depart_id = depart_enhence.depart_id ";
            string groupBySql = null;
            
            if (depart_id == -1 )
            {
                depart_nameSql = "";
                groupBySql = @"group by depart_enhence.depart_name, pb.name, pb.person_id, pb.work_sn,  pdi.person_disable_info_id, 
                    depart_enhence.parent_depart_name, depart_enhence.depart_sn,depart_enhence.depart_id,
                    pb.sex, tec.technical_name,
                    ct_on_ground.class_type_name, ct.class_type_name ,pdi.begin_time, pdi.end_time ,ptype.principal_type_order, pcp.principal_name";
            }
            else
            {
                groupBySql = @"group by pb.name, depart_enhence.depart_name, pb.person_id, pb.work_sn,  pdi.person_disable_info_id, 
                    depart_enhence.parent_depart_name, depart_enhence.depart_sn,depart_enhence.depart_id,
                    pb.sex, tec.technical_name,
                    ct_on_ground.class_type_name, ct.class_type_name ,pdi.begin_time, pdi.end_time,ptype.principal_type_order,pcp.principal_name";
            }

            if (GetApplicationType().CompareTo("ShenShuoRailwayApp") == 0)
            {
                //modify by gqy 2014-1-25  添加按职位高低及职位名称排序
                groupBySql += " order by convert_to(depart_enhence.depart_name,  E'GBK'), ptype.principal_type_order, convert_to(pcp.principal_name,  E'GBK'), convert_to(pb.name,  E'GBK')";
            }
            else
            {
                groupBySql += " order by convert_to(depart_enhence.depart_name,  E'GBK'), convert_to(pb.name,  E'GBK')";
            }

            string sql_all = string.Format(@"
                {0}
                SELECT pb.person_id, pb.work_sn, pb.name, count(pei.eye_flag) as iris_info,
                pdi.person_disable_info_id, pdi.begin_time, pdi.end_time, depart_enhence.depart_name,
                depart_enhence.parent_depart_name, depart_enhence.depart_sn, depart_enhence.depart_id,
                pb.sex, tec.technical_name, ct_on_ground.class_type_name as class_type_name_on_ground, ct.class_type_name,
                pcp.principal_name,ptype.principal_type_order                

                from person_base as pb 

                left join person_enroll_info as pei on pb.person_id = pei.person_id
                left join technical as tec on pb.technical_id = tec.technical_id
                left join class_type as ct_on_ground on pb.class_type_id_on_ground = ct_on_ground.class_type_id
                left join class_type as ct on pb.class_type_id = ct.class_type_id
                left join person_disable_info as pdi on pb.person_id = pdi.person_id
                left join depart_enhence on pb.depart_id = depart_enhence.depart_id 
                left join principal as pcp on pb.principal_id = pcp.principal_id
                left join principal_type ptype on ptype.principal_type_id = pcp.principal_type_id
                left join work_type as wt on pb.work_type_id = wt.work_type_id
                ,pei_condition
               
                where pb.delete_time is null and pei_condition.person_id = pb.person_id 
                {1}
                {2}
                {3}",
                sql_conditon, depart_nameSql, sql_where, groupBySql);

            List<UserPersonInfo> query = new List<UserPersonInfo>();
            DataTable dt = DbAccess.POSTGRESQL.Select(sql_all, "user_persons_info");
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }

            foreach (DataRow ar in dt.Rows)
            {
                UserPersonInfo userPersonInfo = new UserPersonInfo();
                //如果可为空需要判断   if(ar["attend_record_id"].ToString() != "")
                //或者 if(ar["attend_record_id"] != DBNull.Value)
                #region         数据填充
                userPersonInfo.person_id = -1;
                if (ar["person_id"] != DBNull.Value)
                {
                    userPersonInfo.person_id = Convert.ToInt32(ar["person_id"]);
    
                }
                userPersonInfo.work_sn = "";
                if (ar["work_sn"] != DBNull.Value)
                {
                    userPersonInfo.work_sn = Convert.ToString(ar["work_sn"]).Trim();
                }
                userPersonInfo.person_name = "";
                if (ar["name"] != DBNull.Value)
                {
                    userPersonInfo.person_name = Convert.ToString(ar["name"]).Trim();
                }
                userPersonInfo.iris_register = "";
                if (ar["iris_info"] != DBNull.Value)
                {
                    int eye_flag_count = Convert.ToInt32(ar["iris_info"]);
                    if (eye_flag_count > 0)
                    {
                        userPersonInfo.iris_register = "已注册";
                    }
                    else
                    {
                        userPersonInfo.iris_register = "未注册";
                    }
                }
                userPersonInfo.iris_status = "";
                if (ar["person_disable_info_id"] != DBNull.Value)
                {
                    DateTime begin_time = DateTime.Parse(ar["begin_time"].ToString());
                    DateTime end_time = DateTime.MaxValue;
                    if (ar["end_time"] != DBNull.Value)
                    {
                        end_time = DateTime.Parse(ar["end_time"].ToString());
                    }
                    int beginComp = begin_time.CompareTo(System.DateTime.Now);
                    int endComp = end_time.CompareTo(System.DateTime.Now);
                    if (beginComp < 0 && endComp > 0)
                    {
                        userPersonInfo.iris_status = "未启用";
                    }
                    else
                    {
                        userPersonInfo.iris_status = "启用";
                    }
                }
                else
                {
                    userPersonInfo.iris_status = "启用";
                }
                userPersonInfo.depart_name = "";
                if (ar["depart_name"] != DBNull.Value)
                {
                    userPersonInfo.depart_name = Convert.ToString(ar["depart_name"]).Trim();
                }
                userPersonInfo.parent_depart_name = "";
                if (ar["parent_depart_name"] != DBNull.Value)
                {
                    userPersonInfo.parent_depart_name = Convert.ToString(ar["parent_depart_name"]).Trim();
                }
                userPersonInfo.depart_sn = "";
                if (ar["depart_id"] != DBNull.Value)
                {
                    userPersonInfo.depart_id = Convert.ToInt32(ar["depart_id"]);
                }
                if (ar["depart_sn"] != DBNull.Value)
                {
                    userPersonInfo.depart_sn = Convert.ToString(ar["depart_sn"]).Trim();
                }
                userPersonInfo.sex = "";
                if (ar["sex"] != DBNull.Value)
                {
                    int sex = Convert.ToInt32(ar["sex"]);
                    if (sex == 0)
                    {
                        userPersonInfo.sex = "男";
                    }
                    else if (sex == 1)
                    {
                        userPersonInfo.sex = "女";
                    }
                }
                userPersonInfo.class_type_name_on_ground = "";
                if (ar["class_type_name_on_ground"] != DBNull.Value)
                {
                    userPersonInfo.class_type_name_on_ground = Convert.ToString(ar["class_type_name_on_ground"]).Trim();
                }
                userPersonInfo.class_type_name = "";
                if (ar["class_type_name"] != DBNull.Value)
                {
                    userPersonInfo.class_type_name = Convert.ToString(ar["class_type_name"]).Trim();
                }
                userPersonInfo.principal_name = "";
                if (ar["principal_name"] != DBNull.Value)
                {
                    userPersonInfo.principal_name = ar["principal_name"].ToString();
                }

                userPersonInfo.index = query.Count + 1;
                #endregion
                //该if else 语句用来处理当一个人有多条停用虹膜记录的时候，在人员列表中也只显示一条该人员的信息by cty
                if (query.Where(a => a.person_id == userPersonInfo.person_id).Count() > 0)
                {
                    foreach (UserPersonInfo upi in query)
                    {
                        if (upi.person_id == userPersonInfo.person_id)
                        {
                            DateTime begin_time = DateTime.Parse(ar["begin_time"].ToString());
                            DateTime end_time = DateTime.MaxValue;
                            if (ar["end_time"] != DBNull.Value)
                            {
                                end_time = DateTime.Parse(ar["end_time"].ToString());
                            }
                            int beginComp = begin_time.CompareTo(System.DateTime.Now);
                            int endComp = end_time.CompareTo(System.DateTime.Now);
                            if (beginComp < 0 && endComp > 0)
                            {
                                upi.iris_status = "未启用";
                            }
                        }
                    }
                }
                else
                {
                    query.Add(userPersonInfo);
                }
            }
            
            return query;
        }

        /// <summary>
        /// 获取当前人员的详细信息
        /// </summary>
        /// <param name="personID"></param>
        /// <returns></returns>
        public UserPersonInfo GetPersonDetailInfo(int personID)
        {
            string sql_all = string.Format(@"SELECT  pb.person_id, pb.work_sn, pb.name, count(pei.eye_flag) as iris_info, pdi.person_disable_info_id,
                d.depart_name, d.depart_id, pb.sex, ct.class_type_id,  ct.class_type_name, 
                ct_on_ground.class_type_id as class_type_id_on_ground, ct_on_ground.class_type_name as class_type_name_on_ground,
                pb.blood_type, pb.birthdate, pb.workday, pb.phone, 
                pb.zipcode, pb.id_card, pb.address, pb.email, pb.memo, pi.image, pi.img_type,
                pcp.principal_id, pcp.principal_name,
                wt.work_type_id, wt.work_type_name
                
                from person_base as pb 
                left join person_enroll_info as pei on pb.person_id = pei.person_id
                left join class_type as ct_on_ground on pb.class_type_id_on_ground = ct_on_ground.class_type_id
                left join class_type as ct on pb.class_type_id = ct.class_type_id
                left join person_disable_info as pdi on pb.person_id = pdi.person_id
                left join depart as d on d.depart_id = pb.depart_id
                left join person_image as pi on pb.person_id = pi.person_id
                left join principal as pcp on pcp.principal_id = pb.principal_id
                left join work_type as wt on wt.work_type_id = pb.work_type_id

                where pb.delete_time is null and pb.person_id = {0}

                group by pb.person_id, pb.work_sn, pb.name,  pdi.person_disable_info_id,
                d.depart_name, pb.sex,  pb.blood_type, pb.birthdate, pb.workday, pb.phone, 
                pb.zipcode, pb.id_card, pb.address, pb.email, pb.memo, pi.image, pi.img_type,
                d.depart_id,  ct_on_ground.class_type_name, ct_on_ground.class_type_id,
                ct.class_type_name, ct.class_type_id, 
                pcp.principal_id, wt.work_type_id,
                pcp.principal_name, wt.work_type_name;", personID);


            DataTable dt = DbAccess.POSTGRESQL.Select(sql_all, "user_person_info");
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }
            UserPersonInfo userPersonInfo = new UserPersonInfo();
            foreach (DataRow ar in dt.Rows)
            {
                #region     数据填充
                if (ar["person_id"] != DBNull.Value)
                {
                    userPersonInfo.person_id = Convert.ToInt32(ar["person_id"]);
                }
                if (ar["depart_name"] != DBNull.Value)
                {
                    userPersonInfo.depart_name = Convert.ToString(ar["depart_name"]);
                }
                if (ar["name"] != DBNull.Value)
                {
                    userPersonInfo.person_name = Convert.ToString(ar["name"]);
                }
                if (ar["iris_info"] != DBNull.Value)
                {
                    int eye_flag_count = Convert.ToInt32(ar["iris_info"]);
                    if (eye_flag_count > 0)
                    {
                        userPersonInfo.iris_register = "已注册";
                    }
                    else
                    {
                        userPersonInfo.iris_register = "未注册";
                    }
                }
                if (ar["work_sn"] != DBNull.Value)
                {
                    userPersonInfo.work_sn = Convert.ToString(ar["work_sn"]);
                }
                if (ar["sex"] != DBNull.Value)
                {
                    int sex = Convert.ToInt32(ar["sex"]);
                    if (sex == 0)
                    {
                        userPersonInfo.sex = "男";
                    }
                    else if (sex == 1)
                    {
                        userPersonInfo.sex = "女";
                    }
                }
                if (ar["blood_type"] != DBNull.Value)
                {
                    userPersonInfo.blood_type = Convert.ToString(ar["blood_type"]);
                }
                if (ar["birthdate"] != DBNull.Value)
                {
                    try
                    {
                        userPersonInfo.birthdate = Convert.ToDateTime(ar["birthdate"]);
                    }
                    catch
                    {

                    }

                }
                if (ar["workday"] != DBNull.Value)
                {
                    try
                    {
                        userPersonInfo.workday = Convert.ToDateTime(ar["workday"]);
                    }
                    catch
                    {

                    }
                }
                if (ar["class_type_name_on_ground"] != DBNull.Value)
                {
                    userPersonInfo.class_type_name_on_ground = Convert.ToString(ar["class_type_name_on_ground"]);
                }
                if (ar["class_type_id_on_ground"] != DBNull.Value)
                {
                    userPersonInfo.class_type_id_on_ground = Convert.ToInt32(ar["class_type_id_on_ground"]);
                }
                if (ar["class_type_name"] != DBNull.Value)
                {
                    userPersonInfo.class_type_name = Convert.ToString(ar["class_type_name"]);
                }
                if (ar["class_type_id"] != DBNull.Value)
                {
                    userPersonInfo.class_type_id = Convert.ToInt32(ar["class_type_id"]);
                }

                if (ar["phone"] != DBNull.Value)
                {
                    userPersonInfo.phone = Convert.ToString(ar["phone"]);
                }
                if (ar["zipcode"] != DBNull.Value)
                {
                    userPersonInfo.zipcode = Convert.ToString(ar["zipcode"]);
                }
                if (ar["id_card"] != DBNull.Value)
                {
                    userPersonInfo.id_card = Convert.ToString(ar["id_card"]);
                }
                if (ar["address"] != DBNull.Value)
                {
                    userPersonInfo.address = Convert.ToString(ar["address"]);
                }
                if (ar["email"] != DBNull.Value)
                {
                    userPersonInfo.email = Convert.ToString(ar["email"]);
                }
                if (ar["memo"] != DBNull.Value)
                {
                    userPersonInfo.memo = Convert.ToString(ar["memo"]);
                }
                if (ar["image"] != DBNull.Value)
                {
                    userPersonInfo.image = (byte[])ar["image"];
                }
                if (ar["img_type"] != DBNull.Value)
                {
                    userPersonInfo.img_type = Convert.ToString(ar["img_type"]);
                }
                if (ar["depart_id"] != DBNull.Value)
                {
                    userPersonInfo.depart_id = Convert.ToInt32(ar["depart_id"]);
                }
                if (ar["memo"] != DBNull.Value)
                {
                    userPersonInfo.memo = Convert.ToString(ar["memo"]);
                }
                if (ar["principal_id"] != DBNull.Value)
                {
                    userPersonInfo.principal_id = Convert.ToInt32(ar["principal_id"]);
                }
                if (ar["work_type_id"] != DBNull.Value)
                {
                    userPersonInfo.work_type_id = Convert.ToInt32(ar["work_type_id"]);
                }
                if (ar["work_type_name"] != DBNull.Value)
                {
                    userPersonInfo.work_type_name = Convert.ToString(ar["work_type_name"]);
                }
                if (ar["principal_name"] != DBNull.Value)
                {
                    userPersonInfo.principal_name = Convert.ToString(ar["principal_name"]);
                }
               
                #endregion
            }
            return userPersonInfo;
        }

        /// <summary>
        /// 判断新增加的时间段与与已存在的时间段是否冲突 by cty
        /// </summary>
        /// <param name="personIDs"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public bool JudgeTimeIsExist(int[] personIDs, DateTime startTime, DateTime endTime)
        {
            for (int i = 0; i < personIDs.Length; i++)
            {
                string sql_select = string.Format(@" Select * from person_disable_info where person_id = {0} and ( begin_time <= '{1}'
                          and begin_time >= '{2}' or end_time >= '{3}' and end_time <= '{4}');", personIDs[i], endTime, startTime, startTime, endTime);
                DataTable dt = DbAccess.POSTGRESQL.Select(sql_select, "person_disable_info_Is_Exist");
                if (null == dt || dt.Rows.Count < 1)
                {
                    return false;
                }
            }
            return true;
        }

        #endregion

        #region 删除函数
        
        /// <summary>
        /// 人员批量删除
        /// </summary>
        /// <param name="personIDs"></param>
        [Invoke]
        public OptionInfo BatchDeletePerson(int[] person_ids)
        {
            string deleteTime = System.DateTime.Now.ToString();
            string sql_delete = @"UPDATE person_base SET delete_time = '"+deleteTime+"' WHERE person_id in ( ";
            sql_delete += person_ids[0];
            OptionInfo optionInfo = new OptionInfo();
            for (int i = 1; i < person_ids.Length; i++)
            {
                sql_delete += "," + person_ids[i];
            }
            sql_delete += ")";
            if (DbAccess.POSTGRESQL.Update(sql_delete))
            {
                //通知后台已经删除了人
                DataRefChangeData dt = new DataRefChangeData();
                dt.type = 1;
                dt.typeTable = 0;
                dt.len = person_ids.Length;
                if (ServerComLib.DataRefChange(strIP, Port, ref dt, person_ids) == 0)
                {
                    optionInfo.isSuccess = true;
                }
                else
                {
                    optionInfo.isNotifySuccess = false;
                    //int err = 0;
                    optionInfo.option_info = "删除成功，但通知后台失败，请检查后台"; 
                        //ServerComLib.GetLastError(ref err) + "错误代码【" + err + "】";
                }
            }
            else
            {
                optionInfo.isSuccess = false;
                optionInfo.option_info = "执行sql失败";
            }


            return optionInfo;
           
        }


        /// <summary>
        /// 删除指定personIDs 的虹膜停用记录
        /// 批处理
        /// </summary>
        /// <param name="perosnIDs"></param>
        [Invoke]
        public OptionInfo DelAllStopIrisRecords(int[] perosnIDs)
        {
            OptionInfo optionInfo = new OptionInfo();
            string sql = @"DELETE FROM person_disable_info WHERE person_id in ( ";
            //DELETE FROM person_disable_info WHERE person_id = xxx;
            sql += perosnIDs[0];
            for (int i = 1; i < perosnIDs.Length; i++)
            {
                sql += "," + perosnIDs[i];
            }

            sql += ")";
            if (DbAccess.POSTGRESQL.Delete(sql))
            {
                //往后台发情期 告之删除了所有停用虹膜的记录
                if (ServerComLib.ControlIrisDataEnable(strIP, Port)==0)
                {
                    optionInfo.isSuccess = true;
                }
                else
                {
                    optionInfo.isNotifySuccess = false;
                    optionInfo.option_info = "删除成功，但通知后台失败，请检查后台";
                }
            }
            else
            {
                optionInfo.isSuccess = false;
                optionInfo.option_info = "执行sql失败";
            }

            return optionInfo;

        }

        /// <summary>
        /// 删除班制
        /// </summary>
        /// <param name="class_type_ids"></param>
        /// <returns></returns>
        public OptionInfo DeleteClassType(string[] class_type_ids)
        {

            OptionInfo res = new OptionInfo();
            for (int i = 0; i < class_type_ids.Length; i++)
            {
                string sql_select = string.Format(@"SELECT class_order_id
                  FROM class_order_jigongshi
                  where class_type_id = {0} and delete_time is null", class_type_ids[i]);
                DataTable dt = DbAccess.POSTGRESQL.Select(sql_select, "");
                if (dt != null && dt.Rows.Count > 0 && dt.Rows[0].ItemArray.Length > 0)
                {
                    res.isSuccess = false;
                    res.option_info = "该班制还存在没有删除的班次信息，请删除该班制中所有的班次信息再试";
                    return res;
                }
                string str_Del = string.Format(@"update class_type
                    set delete_info = -1 where class_type_id={0} ;
                    update person_base
                    set class_type_id = null
                    where class_type_id = {0};
                    update person_base
                    set class_type_id_on_ground = null
                    where class_type_id_on_ground = {0};
                    ", class_type_ids[i]);

                if (DbAccess.POSTGRESQL.ExecuteSql(str_Del) < 1)
                {
                    res.isSuccess = false;
                    res.option_info = "执行Sql语句出错： " + str_Del;
                    return res;
                }

            }

            //通知后台已经删除了班制 by cty
            DataRefChangeData addclasstypedt = new DataRefChangeData();
            addclasstypedt.type = 1;
            addclasstypedt.typeTable = 2;
            addclasstypedt.len = 1;
            int[] personKeyID = new int[class_type_ids.Length];
            for (int i = 0; i < class_type_ids.Length; i++)
            {
                personKeyID[i] = Convert.ToInt32(class_type_ids[i]);
            }

            if (ServerComLib.DataRefChange(strIP, Port, ref addclasstypedt, personKeyID) != 0)
            {
                res.isNotifySuccess = false;
                res.option_info = "删除成功，但通知后台失败，请检查后台";
                return res;
            }

            res.isSuccess = true;
            res.option_info = "删除班制成功";
            return res;
        }

        //删除班次
        public OptionInfo DeleteClassOrder(string[] class_order_ids)
        {
            OptionInfo res = new OptionInfo();
            for (int i = 0; i < class_order_ids.Length; i++)
            {
                string class_order_id = class_order_ids[i];
                if (class_order_id.Equals("null"))
                {
                    res.isSuccess = false;
                    res.option_info = "该classclass_order_id不存在";
                    return res;
                }

                //先删除classorderstandard/classorder_timeduration表里的该class_order_id的信息
                string sql_delete_classOrderStandard = string.Format("DELETE FROM \"class_order.standard\" WHERE class_order_id = {0};", class_order_id);
                if (DbAccess.POSTGRESQL.ExecuteSql(sql_delete_classOrderStandard) < 0)
                {
                    res.isSuccess = false;
                    res.option_info = "执行Sql语句出错： " + sql_delete_classOrderStandard;
                    return res;
                }

                string sql_delete_classOrderTimeduration = string.Format("DELETE FROM \"class_order.timeduration\" WHERE class_order_id = {0};", class_order_id);
                if (DbAccess.POSTGRESQL.ExecuteSql(sql_delete_classOrderTimeduration) < 0)
                {
                    res.isSuccess = false;
                    res.option_info = "执行Sql语句出错： " + sql_delete_classOrderTimeduration;
                    return res;
                }

                string sql_delete_classOrderBase = string.Format("DELETE FROM class_order_normal WHERE class_order_id = {0};", class_order_id);
                if (DbAccess.POSTGRESQL.ExecuteSql(sql_delete_classOrderBase) < 0)
                {
                    res.isSuccess = false;
                    res.option_info = "执行Sql语句出错： " + sql_delete_classOrderBase;
                    return res;
                }
            }

            ////通知后台服务已经新删除了班次信息 by cty
            //if (ServerComLib.NoticeTableChange(strIP, Port,
            //    new string[] { "class_order.timeduration", "class_order.standard", "class_order_normal" }, 
            //    3) != 0)
            //{
            //    res.isNotifySuccess = false;
            //    res.option_info = "删除成功，但通知后台失败，请检查后台";
            //    return res;
            //}

            res.isSuccess = true;
            res.option_info = "删除班次成功";
            return res;
        }

        #endregion

        #region 复合函数

        /// <summary>
        /// 设置指定personIDs 的虹膜停用记录
        /// 批处理
        /// </summary>
        /// <param name="personIDs"></param>
        /// <param name="startTiem"></param>
        /// <param name="endTime"></param>
        [Invoke]
        public OptionInfo SetStopIrisRecords(int[] personIDs, List<string> startTime, List<string> endTime, List<int> policy, bool IsBatch)
        {
            OptionInfo optionInfo = new OptionInfo();
            for (int i = 0; i < personIDs.Length; i++)
            {
                if (!IsBatch)//如果是批量停用虹膜的话，就不删除原有的停用虹膜记录，否则就将原来的记录删除掉 by cty
                {
                    string sql_delete = string.Format(@"DELETE FROM person_disable_info WHERE person_id ={0}", personIDs[i]);
                    if (!DbAccess.POSTGRESQL.Delete(sql_delete))
                    {
                        optionInfo.isSuccess = false;
                        optionInfo.option_info = "执行sql失败";
                        return optionInfo;
                    }
                }
                for (int j = 0; j < startTime.Count(); j++)
                {
                    string endTime_str = endTime[j] == null ? "null" : ("'" + endTime[j].ToString() + "'");
                    string startTime_str = "'" + startTime[j].ToString() + "'";
                    int policy_int = policy[j];
                    string sql_insert = string.Format(@"INSERT INTO person_disable_info(
                         person_id, begin_time, end_time, policy)
                         VALUES ({0}, {1}, {2}, {3});",
                         personIDs[i], startTime_str, endTime_str, policy_int);
                    if (DbAccess.POSTGRESQL.ExecuteSql(sql_insert) <= 0)
                    {
                        optionInfo.isSuccess = false;
                        optionInfo.option_info = "执行sql失败";
                        return optionInfo;
                    }
                }
            }
            //往后台发请求 告之停用虹膜了
            if (ServerComLib.ControlIrisDataEnable(strIP, Port) != 0)
            {
                optionInfo.isNotifySuccess = false;
                optionInfo.option_info = "设置成功，但通知后台失败，请检查后台";
            }
            else
            {
                optionInfo.isSuccess = true;
            }
            return optionInfo;

        }

        #endregion

        #region 修改函数和新增函数


        //添加班次
        public OptionInfo AddClassOrder(string class_order_name, string attend_sign,
            string class_type_id, string attend_off_minutes, string in_well_start_time,
            string in_well_end_time, string out_well_start_time, string out_well_end_time,
            string attend_latest_worktime, string attend_max_minutes, bool Is_workcnt_method_standard,
            string[] avail_times, string[] work_cnts, string avail_time_standard, string work_cnt_standard, string memo)
        {

            OptionInfo res = new OptionInfo();
            if (!Is_workcnt_method_standard)
            {
                if (avail_times.Length < 1 || work_cnts.Length != avail_times.Length)
                {
                    res.isSuccess = false;
                    res.option_info = "记工时长和记工工数数目不一致";
                    return res;
                }
            }

            string sql_select = string.Format(@"SELECT class_order_id
                  FROM class_order_normal
                  where class_order_name = {0};", class_order_name);
            DataTable dt = DbAccess.POSTGRESQL.Select(sql_select, "");
            if (dt != null && dt.Rows.Count > 0 && dt.Rows[0].ItemArray.Length > 0)
            {
                res.isSuccess = false;
                res.option_info = "您输入的【班次名称】已被占用，请重新输入";
                
                return res;
            }


            string max_minutes_valid = "1";
            string latest_worktime_valid = "1";
            if (attend_max_minutes.Equals("null"))
            {
                max_minutes_valid = "0";
                attend_max_minutes = "0";
            }
            if (attend_latest_worktime.Equals("null"))
            {
                latest_worktime_valid = "0";
                attend_latest_worktime = "0";
            }
            string create_time = "'" + DateTime.Now.ToString() + "'";
            string attend_sign_str = attend_sign == null ? "null" : ("'" + attend_sign + "'");
            string memo_str = memo == null ? "null" : ("'" + memo + "'");
            string str_intsert = string.Format(@"INSERT INTO class_order_normal(
                class_order_name, in_well_start_time, in_well_end_time, 
                out_well_start_time, out_well_end_time, class_type_id, attend_sign, 
                attend_off_minutes, memo, attend_max_minutes, max_minutes_valid, 
                attend_latest_worktime, latest_worktime_valid,create_time,update_time)
                VALUES ({0},{1},{2},{3},
                        {4},{5},{6},{7},
                        {8},{9},{10},{11},
                        {12},{13},{14});", class_order_name, in_well_start_time, in_well_end_time,
                               out_well_start_time, out_well_end_time, class_type_id,
                               attend_sign_str, attend_off_minutes, memo_str, attend_max_minutes,
                               max_minutes_valid, attend_latest_worktime, latest_worktime_valid, create_time, create_time);

            if (DbAccess.POSTGRESQL.ExecuteSql(str_intsert) < 1)
            {
                res.isSuccess = false;
                res.option_info = "执行insertSql语句出错： " + str_intsert;
                return res;
            }

            sql_select = string.Format(@"SELECT class_order_id
                  FROM class_order_normal
                  where class_order_name = {0};", class_order_name);
            dt = DbAccess.POSTGRESQL.Select(sql_select, "");
            int class_order_id = -1;
            if (dt != null && dt.Rows.Count > 0 && dt.Rows[0].ItemArray.Length > 0)
            {
                class_order_id = (int)dt.Rows[0]["class_order_id"];
            }
            if (class_order_id == -1)
            {
                res.isSuccess = false;
                res.option_info = "班次名称为 " + class_order_name + "的 class_order_id不存在";
                return res;
            }

            if (Is_workcnt_method_standard)
            {
                string sql_insert_classOrderStandard = string.Format("INSERT INTO \"class_order.standard\" " +
                    @"(class_order_id, avail_time, work_cnt)
                     VALUES ({0}, {1}, {2});", class_order_id, avail_time_standard, work_cnt_standard);
                if (DbAccess.POSTGRESQL.ExecuteSql(sql_insert_classOrderStandard) < 1)
                {
                    res.isSuccess = false;
                    res.option_info = "执行Sql语句出错： " + sql_insert_classOrderStandard;
                    return res;
                }
            }
            else
            {
                for (int i = 0; i < avail_times.Length; i++)
                {
                    string sql_insert_classOrderTimeDuration = string.Format("INSERT INTO \"class_order.timeduration\"" +
                       @"(class_order_id, avail_time, work_cnt)
                        VALUES ({0}, {1}, {2});", class_order_id, avail_times[i], work_cnts[i]);
                    if (DbAccess.POSTGRESQL.ExecuteSql(sql_insert_classOrderTimeDuration) < 1)
                    {
                        res.isSuccess = false;
                        res.option_info = "执行Sql语句出错： " + sql_insert_classOrderTimeDuration;
                        return res;
                    }
                }
            }


            res.isSuccess = true;
            res.option_info = "添加班次成功";
            return res;
        }

        //修改班次
        public OptionInfo ModifyClassOrder(string class_order_id, string class_order_name, string attend_sign,
            string class_type_id, string attend_off_minutes, string in_well_start_time,
            string in_well_end_time, string out_well_start_time, string out_well_end_time,
            string attend_latest_worktime, string attend_max_minutes, bool Is_workcnt_method_standard,
            string[] avail_times, string[] work_cnts, string avail_time_standard, string work_cnt_standard, string memo)
        {
            OptionInfo res = new OptionInfo();
            if (!Is_workcnt_method_standard)
            {
                if (avail_times.Length < 1 || work_cnts.Length != avail_times.Length)
                {
                    res.isSuccess = false;
                    res.option_info = "记工时长和记工工数数目不一致";
                    return res;
                }
            }
            
            //查看重复名字的班次
            string sql_select = string.Format(@"SELECT class_order_id
                  FROM class_order_normal
                  where class_order_name = {0} and class_order_id != {1};", class_order_name, class_order_id);
            DataTable dt = DbAccess.POSTGRESQL.Select(sql_select, "");
            if (dt != null && dt.Rows.Count > 0 && dt.Rows[0].ItemArray.Length > 0)
            {
                res.isSuccess = false;
                res.option_info = "您输入的【班次名称】已被占用，请重新输入";
                return res;
            }



            string max_minutes_valid = "1";
            string latest_worktime_valid = "1";
            if (attend_max_minutes.Equals("null"))
            {
                max_minutes_valid = "0";
                attend_max_minutes = "0";
            }
            if (attend_latest_worktime.Equals("null"))
            {
                latest_worktime_valid = "0";
                attend_latest_worktime = "0";
            }
            string update_time = "'" + DateTime.Now.ToString() + "'";
            string attend_sign_str = attend_sign == null ? "null" : ("'" + attend_sign + "'");
            string memo_str = memo == null ? "null" : ("'" + memo + "'");
            string sql_update = string.Format(@"UPDATE class_order_normal
                   SET class_order_name={0}, in_well_start_time={1}, in_well_end_time={2}, 
                       out_well_start_time={3}, out_well_end_time={4}, class_type_id={5}, 
                       attend_sign={6}, attend_off_minutes={7}, memo={8}, 
                       attend_max_minutes={9}, max_minutes_valid={10}, 
                       attend_latest_worktime={11}, latest_worktime_valid={12},update_time={13}
                 WHERE class_order_id = {14};", class_order_name, in_well_start_time, in_well_end_time,
                                    out_well_start_time, out_well_end_time, class_type_id,
                                    attend_sign_str, attend_off_minutes, memo_str, attend_max_minutes,
                                    max_minutes_valid, attend_latest_worktime, latest_worktime_valid, update_time, class_order_id);

            if (DbAccess.POSTGRESQL.ExecuteSql(sql_update) < 1)
            {
                res.isSuccess = false;
                res.option_info = "执行Sql语句出错： " + sql_update;
                return res;
            }
            //删除classorderstandard表里的该class_order_id的信息
            string sql_delete_classOrderStandard = string.Format("DELETE FROM \"class_order.standard\" WHERE class_order_id = {0};", class_order_id);
            if (DbAccess.POSTGRESQL.ExecuteSql(sql_delete_classOrderStandard) < 0)
            {
                res.isSuccess = false;
                res.option_info = "执行Sql语句出错： " + sql_delete_classOrderStandard;
                return res;
            }
            //删除classorder_timeduration表里的该class_order_id的信息
            string sql_delete_classOrderTimeduration = string.Format("DELETE FROM \"class_order.timeduration\" WHERE class_order_id = {0};", class_order_id);
            if (DbAccess.POSTGRESQL.ExecuteSql(sql_delete_classOrderTimeduration) < 0)
            {
                res.isSuccess = false;
                res.option_info = "执行Sql语句出错： " + sql_delete_classOrderTimeduration;
                return res;
            }

            //再插入一行
            if (Is_workcnt_method_standard)
            {
                string sql_insert_classOrderStandard = string.Format("INSERT INTO \"class_order.standard\" " +
                    @"(class_order_id, avail_time, work_cnt)
                     VALUES ({0}, {1}, {2});", class_order_id, avail_time_standard, work_cnt_standard);
                if (DbAccess.POSTGRESQL.ExecuteSql(sql_insert_classOrderStandard) < 1)
                {
                    res.isSuccess = false;
                    res.option_info = "执行Sql语句出错： " + sql_insert_classOrderStandard;
                    return res;
                }
            }
            else
            {
                for (int i = 0; i < avail_times.Length; i++)
                {
                    string sql_insert_classOrderTimeDuration = string.Format("INSERT INTO \"class_order.timeduration\"" +
                       @"(class_order_id, avail_time, work_cnt)
                        VALUES ({0}, {1}, {2});", class_order_id, avail_times[i], work_cnts[i]);
                    if (DbAccess.POSTGRESQL.ExecuteSql(sql_insert_classOrderTimeDuration) < 1)
                    {
                        res.isSuccess = false;
                        res.option_info = "执行Sql语句出错： " + sql_insert_classOrderTimeDuration;
                        return res;
                    }
                }
            }
            ////通知后台服务已经新修改了班次信息 by cty
            //if (ServerComLib.NoticeTableChange(strIP, Port,
            //    new string[] { "class_order.timeduration", "class_order.standard", "class_order_normal" },
            //    3) != 0)
            //{
            //    res.isNotifySuccess = false;
            //    res.option_info = "修改成功，但通知后台失败，请检查后台";
            //    return res;
            //}

            res.isSuccess = true;
            res.option_info = "修改班次成功";

            return res;
        }



//        /// <summary>
//        /// 修改人员信息 
//        /// 应用于非矿模式
//        /// </summary>
//        /// <param name="pInfo"></param>
//        /// <returns></returns>
//        [Query(HasSideEffects = true)]
//        public IEnumerable<OptionInfo> UpdatePersonInfo(int person_id, string depart_id, string class_type_id,
//            string worksn, string name,
//            string sex, string blood_type,
//            string birthdate, string workday,
//            string id_card, string phone,
//            string address, string zipcode,
//            string email, string memo,
//            byte[] imgdata, string imgType)
//        {

//            OptionInfo res = new OptionInfo();
//            List<OptionInfo> query = new List<OptionInfo>();
//            query.Add(res);
//            //更新图片data
//            if (imgdata == null)
//            {
//                imgdata = new byte[0];
//            }
//            //if (imgdata != null && imgdata.Length > 0)
//            {
//                string sql_selectImg = string.Format("SELECT person_image_id FROM person_image where person_id = {0};", person_id);
//                DataTable dt = DbAccess.POSTGRESQL.Select(sql_selectImg, "img_id");
//                if (dt != null && dt.Rows.Count > 0 && dt.Rows[0].ItemArray.Length > 0)
//                {
//                    string sqp_updateImg = "UPDATE person_image Set image=:a, img_type='" + imgType +
//                        "' WHERE person_id = " + person_id.ToString() + ";";
//                    if (DbAccess.POSTGRESQL.ExecuteSql(sqp_updateImg, "a", imgdata, "image") <= 0)
//                    {
//                        res.isSuccess = false;
//                        res.option_info = "执行img更新语句出错： " + sqp_updateImg;
//                        return query;
//                    }
//                }
//                else
//                {
//                    //UPDATE person_image image=:a, img_type='InitPng.png' WHERE person_id = 42;
//                    string sql_insertImg = string.Format(@"insert into person_image(person_id,image,img_type)
//                                    values({0},:a,'{1}')", person_id, imgType);
//                    if (DbAccess.POSTGRESQL.ExecuteSql(sql_insertImg, "a", imgdata, "image") <= 0)
//                    {
//                        res.isSuccess = false;
//                        res.option_info = "执行img插入语句出错" + sql_insertImg;
//                        return query;
//                    }
//                }
//            }



//            //  SELECT person_image_id
//            //FROM person_image
//            //where person_id = xxx;


//            //           UPDATE person_base
//            //  SET work_sn=3, name='wz12121', class_type_id=2, sex=0, blood_type='a', birthdate=null, 
//            //      workday=null, id_card='431128198905070070', phone=15801246981, address=null, zipcode=null, email=null, 
//            //      depart_id=11, memo='wwwww'
//            //WHERE person_id = 135759;
//            //更新人员信息 
//            string sql_update = string.Format(@"UPDATE person_base Set
//                   work_sn = {0}, name = {1}, class_type_id_on_ground = {2}, sex={3}, blood_type={4}, birthdate = {5}, 
//                   workday = {6}, id_card = {7}, phone = {8}, address= {9}, zipcode={10}, email={11}, 
//                   depart_id={12}, memo={13} 
//                   where person_id ={14};", worksn, name, class_type_id, sex, blood_type, birthdate,
//                                             workday, id_card, phone, address, zipcode, email,
//                                             depart_id, memo, person_id.ToString());

//            if (DbAccess.POSTGRESQL.ExecuteSql(sql_update) < 1)
//            {
//                res.isSuccess = false;
//                res.option_info = "执行updateSql语句出错： " + sql_update;
//            }
           
//            //通知后台已经修改了人员
//            DataRefChangeData updatepersondt = new DataRefChangeData();
//            updatepersondt.type = 2;
//            updatepersondt.typeTable = 0;
//            updatepersondt.len = 1;
//            int[] personKeyID = { person_id };

//            if (ServerComLib.DataRefChange(strIP, Port, ref updatepersondt, personKeyID) != 0)
//            {
//                res.isNotifySuccess = false;
//                res.option_info = "修改成功，但通知后台失败，请检查后台";
//            }
//            else
//            {
//                res.isSuccess = true;
//                res.option_info = "更新人员成功";
//            }
//            //ServerComLib.NoticeTableChange(strIP, Port, new string[] { "person_base" }, 1);
//            return query;
//        }

//        /// <summary>
//        /// 添加新员工 
//        /// 应用于非矿模式
//        /// </summary>
//        /// <returns></returns>
//        [Query(HasSideEffects = true)]
//        public IEnumerable<OptionInfo> AddPerson(string depart_id, string class_type_id,
//            string worksn, string name,
//            string sex, string blood_type,
//            string birthdate, string workday,
//            string id_card, string phone,
//            string address, string zipcode,
//            string email, string memo, byte[] imgdata, string imgType)
//        {
//            OptionInfo res = new OptionInfo();
//            List<OptionInfo> query = new List<OptionInfo>();
//            query.Add(res);
//            //        INSERT INTO person_base(
//            //        work_sn, "name", sex, blood_type, birthdate,
//            //        workday, id_card, phone, address, zipcode,
//            //        email, depart_id, class_type_id,  memo)
//            //VALUES (?, ?, ?, ?, ?,
//            //        ?, ?, ?, ?, ?, 
//            //        ?, ?, ?, ?, );
//            string str_intsert = string.Format(@"INSERT INTO person_base(
//                    work_sn, name, sex, blood_type, birthdate,
//                    workday, id_card, phone, address, zipcode,
//                    email, depart_id, class_type_id_on_ground,  memo)
//                VALUES ({0}, {1}, {2}, {3}, {4},
//                    {5}, {6}, {7}, {8}, {9}, 
//                    {10}, {11}, {12}, {13} );",
//                        worksn, name, sex, blood_type, birthdate,
//                        workday, id_card, phone, address, zipcode,
//                        email, depart_id, class_type_id, memo);

//            if (DbAccess.POSTGRESQL.ExecuteSql(str_intsert) < 1)
//            {
//                res.isSuccess = false;
//                res.option_info = "执行insertSql语句出错： " + str_intsert;
//                return query;
//            }
//            int person_id = -1;
//            string sql_select = string.Format(@"SELECT person_id
//                      FROM person_base
//                      where name = {0} ", name);
//            DataTable dt = DbAccess.POSTGRESQL.Select(sql_select, "");
//            if (dt != null && dt.Rows.Count > 0 && dt.Rows[0].ItemArray.Length > 0)
//            {
//                person_id = (int)dt.Rows[0]["person_id"];
//                for (int i = 0; i < dt.Rows.Count; i++)
//                {
//                    if ((int)dt.Rows[i]["person_id"] > person_id)
//                    {
//                        person_id = (int)dt.Rows[i]["person_id"];
//                    }
//                }
//            }
//            else
//            {
//                res.isSuccess = false;
//                res.option_info = "执行Sql,结构person_id不存在: " + sql_select;
//                return query;
//            }

//            //更新图片data
//            if (imgdata != null && imgdata.Length > 0)
//            {
//                string sql_selectImg = string.Format("SELECT person_image_id FROM person_image where person_id = {0};", person_id);
//                dt = DbAccess.POSTGRESQL.Select(sql_selectImg, "img_id");
//                if (dt != null && dt.Rows.Count > 0 && dt.Rows[0].ItemArray.Length > 0)
//                {
//                    string sqp_updateImg = "UPDATE person_image Set image=:a, img_type='" + imgType +
//                        "' WHERE person_id = " + person_id.ToString() + ";";
//                    if (DbAccess.POSTGRESQL.ExecuteSql(sqp_updateImg, "a", imgdata, "image") <= 0)
//                    {
//                        res.isSuccess = false;
//                        res.option_info = "执行img更新语句出错： " + sqp_updateImg;
//                        return query;
//                    }
//                }
//                else
//                {
//                    //UPDATE person_image image=:a, img_type='InitPng.png' WHERE person_id = 42;
//                    string sql_insertImg = string.Format(@"insert into person_image(person_id,image,img_type)
//                                    values({0},:a,'{1}')", person_id, imgType);
//                    if (DbAccess.POSTGRESQL.ExecuteSql(sql_insertImg, "a", imgdata, "image") <= 0)
//                    {
//                        res.isSuccess = false;
//                        res.option_info = "执行img插入语句出错" + sql_insertImg;
//                        return query;
//                    }
//                }
//            }
//            //通知后台已经添加了人员
//            DataRefChangeData addpersondt = new DataRefChangeData();
//            addpersondt.type = 0;
//            addpersondt.typeTable = 0;
//            addpersondt.len = 1;
//            int[] personKeyID = { person_id };
//            if (ServerComLib.DataRefChange(strIP, Port, ref addpersondt, personKeyID) != 0)
//            {
//                res.isNotifySuccess = false;
//                res.option_info = "添加成功，但通知后台失败，请检查后台";
//            }
//            else
//            {
//                res.isSuccess = true;
//                res.option_info = "添加人员成功";
//            }

//            return query;
//        }

        /// <summary>
        /// 修改人员信息
        /// 应用于矿山模式
        /// </summary>
        /// <param name="pInfo"></param>
        /// <returns></returns>
        [Query(HasSideEffects = true)]
        public IEnumerable<OptionInfo> UpdatePersonInfoOnMine(int person_id, string depart_id,
            string class_type_id_on_ground, string class_type_id,
            string worksn, string name,
            string sex, string blood_type,
            string birthdate, string workday,
            string id_card, string phone,
            string address, string zipcode,
            string email, string memo,
            byte[] imgdata, string imgType,
            string principal_id,string work_type_id)
        {
            OptionInfo res = new OptionInfo();
            List<OptionInfo> query = new List<OptionInfo>();
            query.Add(res);
            //更新图片data
            if (imgdata == null)
            {
                imgdata = new byte[0];
            }
            //if (imgdata != null && imgdata.Length > 0)
            {
                string sql_selectImg = string.Format("SELECT person_image_id FROM person_image where person_id = {0};", person_id);
                DataTable dt = DbAccess.POSTGRESQL.Select(sql_selectImg, "img_id");
                if (dt != null && dt.Rows.Count > 0 && dt.Rows[0].ItemArray.Length > 0)
                {
                    string sqp_updateImg = "UPDATE person_image Set image=:a, img_type='" + imgType +
                        "' WHERE person_id = " + person_id.ToString() + ";";
                    if (DbAccess.POSTGRESQL.ExecuteSql(sqp_updateImg, "a", imgdata, "image") <= 0)
                    {
                        res.isSuccess = false;
                        res.option_info = "执行img更新语句出错： " + sqp_updateImg;
                        return query;
                    }
                }
                else
                {
                    //UPDATE person_image image=:a, img_type='InitPng.png' WHERE person_id = 42;
                    string sql_insertImg = string.Format(@"insert into person_image(person_id,image,img_type)
                                    values({0},:a,'{1}')", person_id, imgType);
                    if (DbAccess.POSTGRESQL.ExecuteSql(sql_insertImg, "a", imgdata, "image") <= 0)
                    {
                        res.isSuccess = false;
                        res.option_info = "执行img插入语句出错" + sql_insertImg;
                        return query;
                    }
                }
            }


            //更新人员信息 
            string sql_update = string.Format(@"UPDATE person_base Set
                   work_sn = {0}, name = {1}, class_type_id_on_ground = {2}, class_type_id = {3},
                   sex={4}, blood_type={5}, birthdate = {6}, 
                   workday = {7}, id_card = {8}, phone = {9}, address= {10}, zipcode={11}, email={12}, 
                   depart_id={13}, memo={14} , principal_id={15} , work_type_id={16},update_time='{18}'
                   where person_id ={17};", worksn, name, class_type_id_on_ground, class_type_id,
                                            sex, blood_type, birthdate,
                                            workday, id_card, phone, address, zipcode, email,
                                            depart_id, memo, principal_id,
                                            work_type_id, person_id.ToString(), DateTime.Now.ToString());

            if (DbAccess.POSTGRESQL.ExecuteSql(sql_update) < 1)
            {
                res.isSuccess = false;
                res.option_info = "执行updateSql语句出错： " + sql_update;
                return query;
            }
           
            //通知后台已经修改了人员
            DataRefChangeData updatepersondt = new DataRefChangeData();
            updatepersondt.type = 2;
            updatepersondt.typeTable = 0;
            updatepersondt.len = 1;
            int[] personKeyID = { person_id };
            if (ServerComLib.DataRefChange(strIP, Port, ref updatepersondt, personKeyID) != 0)
            {
                res.isNotifySuccess = false;
                res.option_info = "修改成功，但通知后台失败，请检查后台！";
            }
            else
            {
                res.isSuccess = true;
                res.option_info = "更新人员成功！";
            }
            //ServerComLib.NoticeTableChange(strIP, Port, new string[] { "person_base" }, 1);
            return query;
        }

        /// <summary>
        /// 添加新员工
        /// 应用于矿山模式
        /// </summary>
        /// <returns></returns>
        [Query(HasSideEffects = true)]
        public IEnumerable<OptionInfo> AddPersonOnMine(string depart_id, string class_type_id_on_ground, string class_type_id,
            string worksn, string name,
            string sex, string blood_type,
            string birthdate, string workday,
            string id_card, string phone,
            string address, string zipcode,
            string email, string memo, byte[] imgdata, string imgType,
            string principal_id, string work_type_id)
        {
            OptionInfo res = new OptionInfo();
            List<OptionInfo> query = new List<OptionInfo>();
            query.Add(res);

            string str_intsert = string.Format(@"INSERT INTO person_base(
                    work_sn, name, sex, blood_type, birthdate,
                    workday, id_card, phone, address, zipcode,
                    email, depart_id, class_type_id_on_ground, class_type_id, memo,
                    principal_id, work_type_id, create_time, update_time)
                VALUES ({0}, {1}, {2}, {3}, {4},
                    {5}, {6}, {7}, {8}, {9}, 
                    {10}, {11}, {12}, {13}, {14},
                    {15}, {16},'{17}','{17}');",
                        worksn, name, sex, blood_type, birthdate,
                        workday, id_card, phone, address, zipcode,
                        email, depart_id, class_type_id_on_ground,
                        class_type_id, memo,
                        principal_id, work_type_id, DateTime.Now.ToString());

            if (DbAccess.POSTGRESQL.ExecuteSql(str_intsert) < 1)
            {
                res.isSuccess = false;
                res.option_info = "执行insertSql语句出错： " + str_intsert;
                return query;
            }
            int person_id = -1;
            string sql_select = string.Format(@"SELECT person_id
                      FROM person_base
                      where name = {0} ", name);
            DataTable dt = DbAccess.POSTGRESQL.Select(sql_select, "");
            if (dt != null && dt.Rows.Count > 0 && dt.Rows[0].ItemArray.Length > 0)
            {
                person_id = (int)dt.Rows[0]["person_id"];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if ((int)dt.Rows[i]["person_id"] > person_id)
                    {
                        person_id = (int)dt.Rows[i]["person_id"];
                    }
                }
            }
            else
            {
                res.isSuccess = false;
                res.option_info = "执行Sql,结构person_id不存在: " + sql_select;
                return query;
            }

            //更新图片data
            if (imgdata != null && imgdata.Length > 0)
            {
                string sql_selectImg = string.Format("SELECT person_image_id FROM person_image where person_id = {0};", person_id);
                dt = DbAccess.POSTGRESQL.Select(sql_selectImg, "img_id");
                if (dt != null && dt.Rows.Count > 0 && dt.Rows[0].ItemArray.Length > 0)
                {
                    string sqp_updateImg = "UPDATE person_image Set image=:a, img_type='" + imgType +
                        "' WHERE person_id = " + person_id.ToString() + ";";
                    if (DbAccess.POSTGRESQL.ExecuteSql(sqp_updateImg, "a", imgdata, "image") <= 0)
                    {
                        res.isSuccess = false;
                        res.option_info = "执行img更新语句出错： " + sqp_updateImg;
                        return query;
                    }
                }
                else
                {
                    //UPDATE person_image image=:a, img_type='InitPng.png' WHERE person_id = 42;
                    string sql_insertImg = string.Format(@"insert into person_image(person_id,image,img_type)
                                    values({0},:a,'{1}')", person_id, imgType);
                    if (DbAccess.POSTGRESQL.ExecuteSql(sql_insertImg, "a", imgdata, "image") <= 0)
                    {
                        res.isSuccess = false;
                        res.option_info = "执行img插入语句出错" + sql_insertImg;
                        return query;
                    }
                }
            }
            //通知后台已经添加了人员
            DataRefChangeData addpersondt = new DataRefChangeData();
            addpersondt.type = 0;
            addpersondt.typeTable = 0;
            addpersondt.len = 1;
            int[] personKeyID = { person_id };
            if (ServerComLib.DataRefChange(strIP, Port, ref addpersondt, personKeyID) != 0)
            {
                res.isNotifySuccess = false;
                res.option_info = "添加成功，但通知后台失败，请检查后台！";
            }
            else
            {
                res.isSuccess = true;
                res.option_info = "添加人员成功！";
            }

            return query;
        }

        /// <summary>
        /// 批量修改人员属性
        /// 目标属性 -1代表保存原状
        /// </summary>
        /// <param name="personIDs">要修改的人员id数组</param>
        /// <param name="depart_id">目标部门</param>
        /// <param name="class_type_id_on_ground">目标地面班制 -1</param>
        /// <param name="class_type_id_on_mine">目标井下班制</param>
        /// <returns></returns>
        [Invoke]
        public OptionInfo BatchModifyPersons(int[] personIDs, int depart_id, int class_type_id_on_ground,int class_type_id_on_mine,
            int principalId,int workTypeId)
        {
            
            OptionInfo optionInfo = new OptionInfo();
            //对于depart_id 和 class_type_id 为 -1的情况 维持原状不改变
            if (depart_id <= 0 && class_type_id_on_ground <= 0 && class_type_id_on_mine<=0 &&
                principalId <= 0 && workTypeId<=0)
            {
                optionInfo.isSuccess = true;
                return optionInfo;
            }

            StringBuilder SetSql = new StringBuilder("SET ");
            if (depart_id > 0)
            {
                SetSql.Append("depart_id = " + depart_id + " ,");
            }
            if (class_type_id_on_ground > 0)
            {
                SetSql.Append("class_type_id_on_ground = " + class_type_id_on_ground + " ,");
            }
            if (class_type_id_on_mine > 0)
            {
                SetSql.Append("class_type_id = " + class_type_id_on_mine + " ,");
            }
            if (principalId > 0)
            {
                SetSql.Append("principal_id = " + principalId + " ,");
            }
            if (workTypeId > 0)
            {
                SetSql.Append("work_type_id=" + workTypeId + " ,");
            }

            SetSql.Append(@"update_time='" + DateTime.Now.ToString() + @"',");

            if (SetSql[SetSql.Length - 1] == ',')
            {
                SetSql.Remove(SetSql.Length - 1, 1);
            }

            StringBuilder personIdsForSql = new StringBuilder("(");

            for (int i = 0; i < personIDs.Length-1; i++)
            {
                personIdsForSql.Append(personIDs[i] + ",");
            }
            personIdsForSql.Append(personIDs[personIDs.Length - 1] + ")");

            string sqlUpdate = string.Format(@"UPDATE person_base
                {0}
                WHERE person_id in {1};", SetSql.ToString(), personIdsForSql);
            if (DbAccess.POSTGRESQL.ExecuteSql(sqlUpdate) < 1)
            {
                optionInfo.isSuccess = false;
                optionInfo.option_info = "执行sql失败";
                return optionInfo;
            }
            

            //通知后台已经修改人员属性
            DataRefChangeData addpersondt = new DataRefChangeData();
            addpersondt.type = 2;
            addpersondt.typeTable = 0;
            addpersondt.len = personIDs.Length;
            int[] personKeyID = personIDs;
            if (ServerComLib.DataRefChange(strIP, Port, ref addpersondt, personKeyID) != 0)
            {
                optionInfo.isNotifySuccess = false;
                optionInfo.option_info = "修改成功，但通知后台失败，请检查后台";
            }
            else
            {
                optionInfo.isSuccess = true;
                optionInfo.option_info = "批量修改人员属性成功";
            }

           
            return optionInfo;
        }


        /// <summary>
        /// 添加新班制
        /// </summary>
        /// <param name="class_type_name"></param>
        /// <param name="memo"></param>
        /// <returns></returns>
        public OptionInfo AddClassType(string class_type_name, string memo)
        {
            DateTime JudgeDate = DateTime.Now; //by cty
            OptionInfo res = new OptionInfo();

            string str_select = string.Format(@"SELECT *
              FROM class_type
              where class_type_name = {0};", class_type_name);
            DataTable dt = DbAccess.POSTGRESQL.Select(str_select, "ClassType");
            if (dt != null && dt.Rows.Count > 0 && dt.Rows[0].ItemArray.Length > 0)
            {
                res.isSuccess = false;
                res.option_info = "您输入的【班制名称】已被占用，请重新输入";
                return res;
            }


            string str_intsert = string.Format(@"INSERT INTO class_type(
                class_type_name, memo,create_time)
                VALUES ({0}, {1},'{2}');", class_type_name, memo, DateTime.Now);

            if (DbAccess.POSTGRESQL.ExecuteSql(str_intsert) < 1)
            {
                res.isSuccess = false;
                res.option_info = "执行insertSql语句出错： " + str_intsert;
                return res;
            }
            else
            {
                res.isSuccess = true;
                res.option_info = "添加班制成功";
            }

            //取得class_type_id，用于通知后台
            int class_type_id = -1;
            string sql_select = string.Format(@"SELECT class_type_id
                      FROM class_type
                      where class_type_name = {0} and  create_time>= '{1}'", class_type_name, JudgeDate);
            DataTable m_dt = DbAccess.POSTGRESQL.Select(sql_select, "");
            if (m_dt != null && m_dt.Rows.Count > 0 && m_dt.Rows[0].ItemArray.Length > 0)
            {
                class_type_id = (int)m_dt.Rows[0]["class_type_id"];
                for (int i = 0; i < m_dt.Rows.Count; i++)
                {
                    if ((int)m_dt.Rows[i]["class_type_id"] > class_type_id)
                    {
                        class_type_id = (int)m_dt.Rows[i]["class_type_id"];
                    }
                }
            }
            else
            {
                res.isSuccess = false;
                res.option_info = "执行Sql,结构person_id不存在: " + sql_select;
                return res;
            }
            //通知后台服务已经新删除了班次信息 
            if (ServerComLib.NoticeTableChange(strIP, Port,
                new string[] { "class_type"  },
                1) != 0)
            {
                res.isNotifySuccess = false;
                res.option_info = "删除成功，但通知后台失败，请检查后台";
                return res;
            }
            res.isNotifySuccess = true;
            res.isSuccess = true;
            res.option_info = "添加班制成功";
            
            return res;
        }

        /// <summary>
        /// 修改班制信息
        /// </summary>
        /// <param name="class_type_id"></param>
        /// <param name="class_type_name"></param>
        /// <param name="memo"></param>
        /// <returns></returns>
        public OptionInfo ModifyClassType(string class_type_id, string class_type_name, string memo)
        {
            OptionInfo res = new OptionInfo();
            string str_select = string.Format(@"SELECT *
              FROM class_type
              where class_type_name = {0} and class_type_id != {1};", class_type_name, class_type_id);
            DataTable dt = DbAccess.POSTGRESQL.Select(str_select, "ClassType");
            if (dt != null && dt.Rows.Count > 0 && dt.Rows[0].ItemArray.Length > 0)
            {
                res.isSuccess = false;
                res.option_info = "您输入的【班制名称】已被占用，请重新输入";
                return res;
            }

            string str_update = string.Format(@"UPDATE class_type
               SET class_type_name = {0}, memo = {1}
             WHERE class_type_id = {2};", class_type_name, memo, class_type_id);

            if (DbAccess.POSTGRESQL.ExecuteSql(str_update) < 1)
            {
                res.isSuccess = false;
                res.option_info = "执行Sql语句出错： " + str_update;
                return res;
            }
        

            //通知后台已经修改了班制 by cty
            DataRefChangeData addclasstypedt = new DataRefChangeData();
            addclasstypedt.type = 2;
            addclasstypedt.typeTable = 2;
            addclasstypedt.len = 1;
            int[] personKeyID = { Convert.ToInt32(class_type_id) };
            if (ServerComLib.DataRefChange(strIP, Port, ref addclasstypedt, personKeyID) != 0)
            {
                res.isNotifySuccess = false;
                res.option_info = "修改成功，但通知后台失败，请检查后台";
            }
            else
            {
                res.isSuccess = true;
                res.option_info = "修改班制成功";
            }
            return res;
        }
     
        #endregion

        #region 允许用户更新实体类的辅助函数
       
        /// <summary>
        /// 允许用户更新
        /// UserPersonInfo类型的变量
        /// </summary>
        /// <param name="t"></param>
        [Update]
        public void TestUserPersonInfo(UserPersonInfo t)
        {

        }

        [Update]
        public void TestUserClassOrderInfo(UserClassOrderInfo t)
        {

        }


        [Update]
        public void TestUserUserDepartInfo(UserDepartInfo t)
        {

        }

        [Update]
        public void TestUserClassTypeInfo(UserClassTypeInfo t)
        {

        }

        [Update]
        public void TestPersonStopIrisInfo(PersonStopIrisInfo t)
        {

        }

        [Update]
        public void TestPersonStopIrisInfo(PrincipalInfo t)
        {

        }

        [Update]
        public void TestPersonStopIrisInfo(WorkTypeInfo t)
        {

        }

        [Update]
        public void TestPersonStopIrisInfo(PrincipalTypeInfo t)
        {

        }

    

        #endregion

        #region 记工时班次操作 szr 2014-11-14
        //szr 2014-11-14
        /// <summary>
        /// 删除记工时班次
        /// </summary>
        /// <param name="class_order_ids"></param>
        /// <returns></returns>
        public OptionInfo DeleteClassOrderJiGongShi(string[] class_order_ids)
        {
            OptionInfo res = new OptionInfo();
            for (int i = 0; i < class_order_ids.Length; i++)
            {
                string class_order_id = class_order_ids[i];
                if (class_order_id.Equals("null"))
                {
                    res.isSuccess = false;
                    res.option_info = "该class class_order_id不存在";
                    return res;
                }

                //先删除class_order_jigongshi_extend 表里的该class_order_id的信息
                string sql_delete_classOrderJiGongShi_Extend = string.Format("DELETE FROM \"class_order_jigongshi_extend\" WHERE class_order_id = {0};", class_order_id);
                if (DbAccess.POSTGRESQL.ExecuteSql(sql_delete_classOrderJiGongShi_Extend) < 0)
                {
                    //res.isSuccess = false;
                    //res.option_info = "执行Sql语句出错： " + sql_delete_classOrderJiGongShi_Extend;
                    //return res;
                }

                //删除class_order_jigongshi信息
                string sql_delete_classOrderJiGongShi = string.Format("update class_order_jigongshi set delete_time = '{0}' where class_order_id={1};", DateTime.Now, class_order_id);
                if (DbAccess.POSTGRESQL.ExecuteSql(sql_delete_classOrderJiGongShi) < 0)
                {
                    res.isSuccess = false;
                    res.option_info = "执行Sql语句出错： " + sql_delete_classOrderJiGongShi;
                    return res;
                }

            }
            //通知后台服务已经新删除了班次信息 
            if (ServerComLib.NoticeTableChange(strIP, Port,
                new string[] { "class_order_jigongshi_extend", "class_order_jigongshi" },
                2) != 0)
            {
                res.isNotifySuccess = false;
                res.option_info = "删除成功，但通知后台失败，请检查后台";
                return res;
            }
            res.isSuccess = true;
            res.option_info = "删除记工时班次成功";
            return res;
        }

        /// <summary>
        ///  不能删除，否则无法更新
        /// </summary>
        /// <param name="o"></param>
        [Update]
        public void TestEidtor(UserClassOrderJiGongShiInfo o)
        {

        }


        /// <summary>
        /// 添加记工时班次
        /// </summary>       AddClassOrderSignJiGongShi
        /// <returns></returns>
        [Invoke(HasSideEffects = true)]
        public OptionInfo AddClassOrderJiGongShi(UserClassOrderJiGongShiInfo userClassOrderJiGongShi)
        {

            OptionInfo res = new OptionInfo();

            string sql_select = string.Format(@"SELECT class_order_id
                  FROM class_order_jigongshi
                  where class_order_name = {0};", userClassOrderJiGongShi.class_order_name);
            DataTable dt = DbAccess.POSTGRESQL.Select(sql_select, "");
            if (dt != null && dt.Rows.Count > 0 && dt.Rows[0].ItemArray.Length > 0)
            {
                res.isSuccess = false;
                res.option_info = "您输入的【记工时班次名称】已被占用，请重新输入";

                return res;
            }
            
            string str_insert = string.Format(@"INSERT INTO class_order_jigongshi (
                 class_order_name, class_type_id, attend_sign,memo, 
                create_time,in_start_time,in_end_time,attend_off_minutes,attend_max_minutes,max_minutes_valid,
                 attend_latest_worktime,latest_worktime_valid, work_cnt,avail_time)
                VALUES ({0},{1},'{2}','{3}','{4}',{5},{6},{7},{8},{9},{10},{11},{12},{13}
                        );", userClassOrderJiGongShi.class_order_name, userClassOrderJiGongShi.class_type_id,
                               userClassOrderJiGongShi.attend_sign, userClassOrderJiGongShi.memo, userClassOrderJiGongShi.create_time,
                               userClassOrderJiGongShi.in_start_time,
            userClassOrderJiGongShi.in_end_time, userClassOrderJiGongShi.attend_off_minutes, userClassOrderJiGongShi.attend_max_minutes,
            userClassOrderJiGongShi.max_minutes_valid, userClassOrderJiGongShi.attend_latest_worktime, userClassOrderJiGongShi.latest_worktime_valid,
            userClassOrderJiGongShi.work_cnt, userClassOrderJiGongShi.avail_time);
            
            if (DbAccess.POSTGRESQL.ExecuteSql(str_insert) < 1)
            {
                res.isSuccess = false;
                res.option_info = "执行insertSql语句出错： " + str_insert;
                return res;
            }

            //查询刚刚添加的classOrderID
            DataTable dtClassOrderID = DbAccess.POSTGRESQL.Select(sql_select, "");
            string classOrderID = Convert.ToString(dtClassOrderID.Rows[0][0]); 

            string sql_insert_classOrderExtend = string.Format(@"INSERT INTO class_order_jigongshi_extend (class_order_id, order_set, dev_function)
                    VALUES ({0}, {1}, '{2}');", classOrderID, -1, "井口设备");
            if (DbAccess.POSTGRESQL.ExecuteSql(sql_insert_classOrderExtend) < 1)
            {
                res.isSuccess = false;
                res.option_info = "执行Sql语句出错： " + sql_insert_classOrderExtend;
                return res;
            }

            //通知后台服务已经新删除了班次信息 by cty
            if (ServerComLib.NoticeTableChange(strIP, Port,
                new string[] { "class_order_jigongshi", "class_order_jigongshi_extend"},
                2) != 0)
            {
                res.isNotifySuccess = false;
                res.option_info = "删除成功，但通知后台失败，请检查后台";
                return res;
            }
            res.isSuccess = true;
            res.option_info = "添加记工时班次成功";
            return res;
        }

        /// <summary>
        /// 获得记工时班次
        /// </summary>
        /// <param name="classTypeId"></param>
        /// <returns></returns>
        public IEnumerable<UserClassOrderJiGongShiInfo> GetClassOrderJiGongShiInfosByClassType(int classTypeId)
        {
            
            List<UserClassOrderJiGongShiInfo> query = new List<UserClassOrderJiGongShiInfo>();
            string sql = string.Format(@"select in_start_time,in_end_time,attend_off_minutes,attend_max_minutes,
        max_minutes_valid,attend_latest_worktime,latest_worktime_valid,work_cnt,avail_time,class_order_id,
       class_order_name,attend_sign,jigongshi.memo,jigongshi.create_time,jigongshi.update_time,jigongshi.class_type_id, cltype.class_type_name from class_order_jigongshi as jigongshi,class_type as cltype where jigongshi.class_type_id={0} and cltype.class_type_id={1} and delete_time is null " ,
                                                                                                                                                                                                                                                                           classTypeId, classTypeId);

            DataTable dt = DbAccess.POSTGRESQL.Select(sql, "class_order_jigongshi");
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }

            foreach (DataRow ar in dt.Rows)
            {
                UserClassOrderJiGongShiInfo userClassOrderJiGongShiInfo = new UserClassOrderJiGongShiInfo();

                #region         数据填充

                if (ar["class_order_id"] != DBNull.Value)
                {
                    userClassOrderJiGongShiInfo.class_order_id = Convert.ToInt32(ar["class_order_id"]);
                }
                if (ar["class_order_name"] != DBNull.Value)
                {
                    userClassOrderJiGongShiInfo.class_order_name = Convert.ToString(ar["class_order_name"]);
                }
                if (ar["class_type_id"] != DBNull.Value)
                {
                    userClassOrderJiGongShiInfo.class_type_id = Convert.ToInt32(ar["class_type_id"]);
                }
                if (ar["attend_sign"] != null)
                {
                    userClassOrderJiGongShiInfo.attend_sign = Convert.ToString(ar["attend_sign"]);
                }

                if (ar["create_time"] != null)
                {
                    userClassOrderJiGongShiInfo.create_time = Convert.ToDateTime(ar["create_time"]);
                }
                if (ar["update_time"] != null)
                {
                    userClassOrderJiGongShiInfo.update_time = Convert.ToDateTime(ar["update_time"]);
                }
                if (ar["memo"] !=null)
                {
                    userClassOrderJiGongShiInfo.memo = Convert.ToString(ar["memo"]);
                }

               

               
                  userClassOrderJiGongShiInfo.work_cnt = Convert.ToInt32(ar["work_cnt"]);
                  userClassOrderJiGongShiInfo.work_cntStr = Convert.ToInt32(ar["work_cnt"]) / 10f;
               
             
                if (ar["attend_off_minutes"] != DBNull.Value)
                {
                    userClassOrderJiGongShiInfo.attend_off_minutesStr = Helper.GetAttendBelongDay(Convert.ToInt32(ar["attend_off_minutes"]));
                   userClassOrderJiGongShiInfo.attend_off_minutes =Convert.ToInt32(ar["attend_off_minutes"]);
                }
                if (ar["in_start_time"] != DBNull.Value)
                {
                    userClassOrderJiGongShiInfo.in_start_time = Convert.ToInt32(ar["in_start_time"]);
                    userClassOrderJiGongShiInfo.in_well_start_time_str = Helper.MinutesToDate(Convert.ToInt32(ar["in_start_time"]), true);
                }
                if (ar["in_end_time"] != DBNull.Value)
                {
                    userClassOrderJiGongShiInfo.in_end_time = Convert.ToInt32(ar["in_end_time"]);
                    userClassOrderJiGongShiInfo.in_well_end_time_str = Helper.MinutesToDate(Convert.ToInt32(ar["in_end_time"]), true);
                }

                if (ar["attend_max_minutes"] != DBNull.Value)
                {
                    if (Convert.ToInt32(ar["max_minutes_valid"]) > 0)
                    {

                        userClassOrderJiGongShiInfo.attend_max_minutesStr = Helper.MinutesToTimeLenth(Convert.ToInt32(ar["attend_max_minutes"])); 
                        userClassOrderJiGongShiInfo.max_minutes_valid = 1;
                    }
                    else
                    {
                        userClassOrderJiGongShiInfo.attend_max_minutesStr = "无效";
                    }
                    userClassOrderJiGongShiInfo.attend_max_minutes = Convert.ToInt32(ar["attend_max_minutes"]);
                }

                if (ar["attend_latest_worktime"] != DBNull.Value)
                {
                    if (Convert.ToInt32(ar["latest_worktime_valid"]) > 0)
                    {
                        userClassOrderJiGongShiInfo.attend_latest_worktimeStr = Helper.MinutesToDate(Convert.ToInt32(ar["attend_latest_worktime"]), true);
                        userClassOrderJiGongShiInfo.latest_worktime_valid = 1;
                   
                    }
                    else
                    {
                        userClassOrderJiGongShiInfo.attend_latest_worktimeStr = "无效";
                    }
                    userClassOrderJiGongShiInfo.attend_latest_worktime = Convert.ToInt32(ar["attend_latest_worktime"]);
                }

               
                if (ar["avail_time"] != DBNull.Value)
                {
                    userClassOrderJiGongShiInfo.avail_timeStr = Helper.MinutesToDate(Convert.ToInt32(ar["avail_time"]), false);
                    userClassOrderJiGongShiInfo.avail_time = Convert.ToInt32(ar["avail_time"]);
                }
                if (ar["class_type_name"] != DBNull.Value)
                {
                    userClassOrderJiGongShiInfo.class_type_name = ar["class_type_name"].ToString();
 
                }
                #endregion

                query.Add(userClassOrderJiGongShiInfo);
            }

            return query;
        }

        /// <summary>
        /// 修改记工时班次
        /// </summary>       
        /// <returns></returns>
       [Invoke(HasSideEffects = true)]
        public OptionInfo ModifyClassOrderJiGongShi(UserClassOrderJiGongShiInfo userClassOrderJiGongShi)
        {

            OptionInfo res = new OptionInfo();

            string sql_select = string.Format(@"SELECT class_order_id
                  FROM class_order_jigongshi
                  where class_order_name = '{0}' and class_order_id != {1};", userClassOrderJiGongShi.class_order_name, userClassOrderJiGongShi.class_order_id);
            DataTable dt = DbAccess.POSTGRESQL.Select(sql_select, "");
            if (dt != null && dt.Rows.Count > 0 && dt.Rows[0].ItemArray.Length > 0)
            {
                res.isSuccess = false;
                res.option_info = "您输入的【记工时班次名称】已被占用，请重新输入";

                return res;
            }

            string str_update = string.Format(@"UPDATE class_order_jigongshi SET
                class_order_name = '{0}', class_type_id ={1}, attend_sign='{2}', 
                attend_off_minutes = {3}, work_cnt={4}, update_time='{5}',attend_max_minutes={6},
                max_minutes_valid={7},attend_latest_worktime={8},latest_worktime_valid ={9}, 
                avail_time={10},in_start_time={11},in_end_time={12} WHERE class_order_id = {13};",
                userClassOrderJiGongShi.class_order_name, userClassOrderJiGongShi.class_type_id,
                userClassOrderJiGongShi.attend_sign,userClassOrderJiGongShi.attend_off_minutes,
                userClassOrderJiGongShi.work_cnt,userClassOrderJiGongShi.update_time,
                userClassOrderJiGongShi.attend_max_minutes,
                userClassOrderJiGongShi.max_minutes_valid, userClassOrderJiGongShi.attend_latest_worktime,
                userClassOrderJiGongShi.latest_worktime_valid,userClassOrderJiGongShi.avail_time,
                userClassOrderJiGongShi.in_start_time,userClassOrderJiGongShi.in_end_time,
                userClassOrderJiGongShi.class_order_id);

            if (DbAccess.POSTGRESQL.ExecuteSql(str_update) < 1)
            {
                res.isSuccess = false;
                res.option_info = "执行updateSql语句出错： " + str_update;
                return res;
            }

            //通知后台服务已经新删除了班次信息 by cty
            if (ServerComLib.NoticeTableChange(strIP, Port,
                new string[] { "class_order_jigongshi" },
                1) != 0)
            {
                res.isNotifySuccess = false;
                res.option_info = "修改成功，但通知后台失败，请检查后台";
                return res;
            }

            res.isSuccess = true;
            res.option_info = "修改记工时班次成功";
            return res;
        }
        

        #endregion

    }

  


}


