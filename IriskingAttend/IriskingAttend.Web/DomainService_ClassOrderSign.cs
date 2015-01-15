/*************************************************************************
** 文件名:   DomainServiceIriskingAttend.cs
×× 主要类:   DomainServiceIriskingAttend
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2014-9-26
** 修改人:  
** 日  期:   
** 描  述:   DomainServiceIriskingAttend类,后台数据库操作  添加修改删除签到班班次
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
using IriskingAttend.Web.Manager;

namespace IriskingAttend.Web
{
    public partial class DomainServiceIriskingAttend
    {
        #region 查询、修改、删除、增加签到班班次
        /// <summary>
        /// 允许用户更新实体类的辅助函数
        /// </summary>
        /// <param name="t"></param>
        [Update]
        public void TestUserClassOrderSignInfo(UserClassOrderSignInfo t)
        {
        }

        /// <summary>
        /// 获得签到班班次
        /// </summary>
        /// <param name="classTypeId"></param>
        /// <returns></returns>
        public IEnumerable<UserClassOrderSignInfo> GetClassOrderSignInfosByClassType(int classTypeId)
        {
            List<UserClassOrderSignInfo> query = new List<UserClassOrderSignInfo>();
            string sql = string.Format(@"
select cosign.class_order_id, max(cosign.class_order_name) class_order_name,max(cosign.class_type_id) class_type_id, max(cosign.attend_sign) attend_sign,
max(cosign.min_work_time) min_work_time, max(cosign.work_time_valid) work_time_valid, max(cosign.work_cnt) work_cnt, max(cosign.lian_ban) lian_ban,
max(cosign.memo) memo, max(ct.class_type_name) class_type_name, array_agg(cosec.section_begin_mins) as section_begin_mins,
array_agg(cosec.section_end_mins ) as section_end_mins, array_agg(cosec.in_calc ) as in_calcs from class_order_sign cosign 
	left join class_type ct on cosign.class_type_id = ct.class_type_id 
	left join (select * from ""class_order_sign.section"" order by section_begin_mins ) as cosec on cosign.class_order_id = cosec.class_order_id
	where cosign.class_type_id = {0} group by cosign.class_order_id ", classTypeId );


            DataTable dt = DbAccess.POSTGRESQL.Select(sql, "class_order_sign");
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }

            foreach (DataRow ar in dt.Rows)
            {
                UserClassOrderSignInfo userClassOrderSignInfo = new UserClassOrderSignInfo();
                
                #region         数据填充

                if (ar["class_order_id"] != DBNull.Value)
                {
                    userClassOrderSignInfo.class_order_id = Convert.ToInt32(ar["class_order_id"]);
                }
                if (ar["class_order_name"] != DBNull.Value)
                {
                    userClassOrderSignInfo.class_order_name = Convert.ToString(ar["class_order_name"]);
                }
                if (ar["class_type_id"] != DBNull.Value)
                {
                    userClassOrderSignInfo.class_type_id = Convert.ToInt32(ar["class_type_id"]);
                }
                if (ar["class_type_name"] != DBNull.Value)
                {
                    userClassOrderSignInfo.class_type_name = Convert.ToString(ar["class_type_name"]);
                }

                if (ar["attend_sign"] != DBNull.Value)
                {
                    userClassOrderSignInfo.attend_sign = Convert.ToString(ar["attend_sign"]);
                }

                if (ar["memo"] != DBNull.Value)
                {
                    userClassOrderSignInfo.memo = Convert.ToString(ar["memo"]);
                }

                if (ar["min_work_time"] != DBNull.Value)
                {
                    userClassOrderSignInfo.min_work_time = Convert.ToInt16(ar["min_work_time"]);
                    userClassOrderSignInfo.min_work_time_str = string.Format("{0}:{1}", userClassOrderSignInfo.min_work_time / 60, (userClassOrderSignInfo.min_work_time % 60).ToString("d2"));
                }
                
                if (ar["work_cnt"] != DBNull.Value)
                {
                    userClassOrderSignInfo.work_cnt = Convert.ToInt32(ar["work_cnt"]);
                    userClassOrderSignInfo.work_cnt_str = Helper.GetWorkCnt(userClassOrderSignInfo.work_cnt);
                }

                if (ar["section_begin_mins"] != DBNull.Value)
                {
                    userClassOrderSignInfo.section_begin_mins = (int[])ar["section_begin_mins"];                    
                }

                if (ar["section_end_mins"] != DBNull.Value)
                {
                    userClassOrderSignInfo.section_end_mins = (int[])ar["section_end_mins"];
                }

                if (ar["in_calcs"] != DBNull.Value)
                {
                    userClassOrderSignInfo.in_calcs = (int[])ar["in_calcs"];
                }
                #endregion   
           
                userClassOrderSignInfo.section_time_str = "";

                if (userClassOrderSignInfo.section_begin_mins!= null)
                {
                    StringBuilder tempStr = new StringBuilder("");
                    for (int secIndex = 0; secIndex < userClassOrderSignInfo.section_begin_mins.Count(); secIndex++)
                    {
                        if (secIndex != 0)
                        {
                            tempStr.Append("\r\n");
                        }
                        
                        //if (userClassOrderSignInfo.section_begin_mins[secIndex] != null)
                        {
                            tempStr.Append(Helper.MinutesToDate(userClassOrderSignInfo.section_begin_mins[secIndex],true));
                            tempStr.Append("->");
                        }

                        //if (userClassOrderSignInfo.section_end_mins[secIndex] != null)
                        {
                            tempStr.Append(Helper.MinutesToDate(userClassOrderSignInfo.section_end_mins[secIndex], true));                            
                        }

                        if ( userClassOrderSignInfo.in_calcs[secIndex] == 1)
                        {
                            tempStr.Append("(*)");
                        }
                    }
                    userClassOrderSignInfo.section_time_str = tempStr.ToString();
                }

                query.Add(userClassOrderSignInfo);
            }

            return query;
        }

        /// <summary>
        /// 删除签到班班次
        /// </summary>
        /// <param name="class_order_ids"></param>
        /// <returns></returns>
        public OptionInfo DeleteClassOrderSign(string[] class_order_ids)
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

                //先删除class_order_sign.section表里的该class_order_id的信息
                string sql_delete_classOrderStandard = string.Format("DELETE FROM \"class_order_sign.section\" WHERE class_order_id = {0};", class_order_id);
                if (DbAccess.POSTGRESQL.ExecuteSql(sql_delete_classOrderStandard) < 0)
                {
                    res.isSuccess = false;
                    res.option_info = "执行Sql语句出错： " + sql_delete_classOrderStandard;
                    return res;
                }


                string sql_delete_classOrderBase = string.Format("DELETE FROM class_order_sign WHERE class_order_id = {0};", class_order_id);
                if (DbAccess.POSTGRESQL.ExecuteSql(sql_delete_classOrderBase) < 0)
                {
                    res.isSuccess = false;
                    res.option_info = "执行Sql语句出错： " + sql_delete_classOrderBase;
                    return res;
                }
            }

            //通知后台服务已经新删除了班次信息 
            if (ServerComLib.NoticeTableChange(strIP, Port,
                new string[] { "class_order_sign.section", "class_order_sign" },
                2) != 0)
            {
                res.isNotifySuccess = false;
                res.option_info = "删除成功，但通知后台失败，请检查后台";
                return res;
            }

            res.isSuccess = true;
            res.option_info = "删除签到班班次成功";
            return res;
        }

        /// <summary>
        /// 添加签到班班次
        /// </summary>       
        /// <returns></returns>
        [Invoke(HasSideEffects = true)]
        public OptionInfo AddClassOrderSign(UserClassOrderSignInfo userClassOrerSign)
        {

            OptionInfo res = new OptionInfo();            

            string sql_select = string.Format(@"SELECT class_order_id
                  FROM class_order_sign
                  where class_order_name = '{0}';", userClassOrerSign.class_order_name);
            DataTable dt = DbAccess.POSTGRESQL.Select(sql_select, "");
            if (dt != null && dt.Rows.Count > 0 && dt.Rows[0].ItemArray.Length > 0)
            {
                res.isSuccess = false;
                res.option_info = "您输入的【签到班班次名称】已被占用，请重新输入";

                return res;
            }

            string str_insert = string.Format(@"INSERT INTO class_order_sign(
                class_order_name, class_type_id, attend_sign, 
                min_work_time, work_cnt, lian_ban)
                VALUES ('{0}',{1},'{2}',{3},
                        {4},{5});", userClassOrerSign.class_order_name,userClassOrerSign.class_type_id, userClassOrerSign.attend_sign,
                                      userClassOrerSign.min_work_time,userClassOrerSign.work_cnt,userClassOrerSign.lian_ban);

            if (DbAccess.POSTGRESQL.ExecuteSql(str_insert) < 1)
            {
                res.isSuccess = false;
                res.option_info = "执行insertSql语句出错： " + str_insert;
                return res;
            }

            sql_select = string.Format(@"SELECT class_order_id
                  FROM class_order_sign
                  where class_order_name = '{0}';", userClassOrerSign.class_order_name);
            dt = DbAccess.POSTGRESQL.Select(sql_select, "");
            int class_order_id = -1;
            if (dt != null && dt.Rows.Count > 0 && dt.Rows[0].ItemArray.Length > 0)
            {
                class_order_id = (int)dt.Rows[0]["class_order_id"];
            }
            if (class_order_id == -1)
            {
                res.isSuccess = false;
                res.option_info = "签到班班次名称为 " + userClassOrerSign.class_order_name + "的 class_order_id不存在";
                return res;
            }

            for (int i = 0; i < userClassOrerSign.section_begin_mins.Length; i++)
            {
                string sql_insert_classOrderTimeDuration = string.Format("INSERT INTO \"class_order_sign.section\"" +
                    @"(class_order_id, section_begin_mins, section_end_mins,in_calc)
                    VALUES ({0}, {1}, {2},{3});", class_order_id, userClassOrerSign.section_begin_mins[i],
                                                userClassOrerSign.section_end_mins[i], userClassOrerSign.in_calcs[i]);
                if (DbAccess.POSTGRESQL.ExecuteSql(sql_insert_classOrderTimeDuration) < 1)
                {
                    res.isSuccess = false;
                    res.option_info = "执行Sql语句出错： " + sql_insert_classOrderTimeDuration;
                    return res;
                }
            }

            //通知后台服务已经新添加了班次信息
            if (ServerComLib.NoticeTableChange(strIP, Port,
                new string[] { "class_order_sign.section", "class_order_sign" },
                2) != 0)
            {
                res.isNotifySuccess = false;
                res.option_info = "添加签到班班次成功，但通知后台失败，请检查后台";
                return res;
            }

            res.isSuccess = true;
            res.option_info = "添加签到班班次成功";
            return res;
        }

        /// <summary>
        /// 修改签到班班次
        /// </summary>       
        /// <returns></returns>
        [Invoke(HasSideEffects = true)]
        public OptionInfo ModifyClassOrderSign(UserClassOrderSignInfo userClassOrerSign)
        {

            OptionInfo res = new OptionInfo();

            string sql_select = string.Format(@"SELECT class_order_id
                  FROM class_order_sign
                  where class_order_name = '{0}' and class_order_id != {1};", userClassOrerSign.class_order_name, userClassOrerSign.class_order_id);
            DataTable dt = DbAccess.POSTGRESQL.Select(sql_select, "");
            if (dt != null && dt.Rows.Count > 0 && dt.Rows[0].ItemArray.Length > 0)
            {
                res.isSuccess = false;
                res.option_info = "您输入的【签到班班次名称】已被占用，请重新输入";

                return res;
            }

            string str_insert = string.Format(@"UPDATE class_order_sign SET
                class_order_name = '{0}', class_type_id ={1}, attend_sign='{2}', 
                min_work_time = {3}, work_cnt={4}, lian_ban={5} WHERE class_order_id = {6};", 
                userClassOrerSign.class_order_name, userClassOrerSign.class_type_id, userClassOrerSign.attend_sign,
                userClassOrerSign.min_work_time, userClassOrerSign.work_cnt, userClassOrerSign.lian_ban, userClassOrerSign.class_order_id);

            if (DbAccess.POSTGRESQL.ExecuteSql(str_insert) < 1)
            {
                res.isSuccess = false;
                res.option_info = "执行insertSql语句出错： " + str_insert;
                return res;
            }

            //删除class_order_sign.section表里的该class_order_id的信息
            string sql_delete_classOrderStandard = string.Format("DELETE FROM \"class_order_sign.section\" WHERE class_order_id = {0};", userClassOrerSign.class_order_id);
            if (DbAccess.POSTGRESQL.ExecuteSql(sql_delete_classOrderStandard) < 0)
            {
                res.isSuccess = false;
                res.option_info = "执行Sql语句出错： " + sql_delete_classOrderStandard;
                return res;
            }

            for (int i = 0; i < userClassOrerSign.section_begin_mins.Length; i++)
            {
                string sql_insert_classOrderTimeDuration = string.Format("INSERT INTO \"class_order_sign.section\"" +
                    @"(class_order_id, section_begin_mins, section_end_mins,in_calc)
                    VALUES ({0}, {1}, {2},{3});", userClassOrerSign.class_order_id, userClassOrerSign.section_begin_mins[i],
                                                userClassOrerSign.section_end_mins[i], userClassOrerSign.in_calcs[i]);
                if (DbAccess.POSTGRESQL.ExecuteSql(sql_insert_classOrderTimeDuration) < 1)
                {
                    res.isSuccess = false;
                    res.option_info = "执行Sql语句出错： " + sql_insert_classOrderTimeDuration;
                    return res;
                }
            }

            //通知后台服务已经新添加了班次信息
            if (ServerComLib.NoticeTableChange(strIP, Port,
                new string[] { "class_order_sign.section", "class_order_sign" },
                2) != 0)
            {
                res.isNotifySuccess = false;
                res.option_info = "修改签到班班次成功，但通知后台失败，请检查后台";
                return res;
            }

            res.isSuccess = true;
            res.option_info = "修改签到班班次成功";
            return res;
        }
        #endregion

        #region 签到班考勤查询
        /// <summary>
        /// 考勤记录查询
        /// </summary>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">截止时间</param>
        /// <param name="departName">部门名称</param>
        /// <param name="leaveType">考勤类型</param>
        /// /// <param name="devTypeIdLst">设备类型</param>
        /// <param name="name">人员名字</param>
        /// <param name="workSN">人员工号</param>
        /// <returns></returns>
        public IEnumerable<UserAttendRec> IrisGetAttendRecSign(DateTime beginTime, DateTime endTime, List<int> departIdLst, List<int> devTypeIdLst,
            string name, string workSN, List<int> principalIdList, List<int> workTypeIdList, int workTime)
        {

            List<UserAttendRec> query = new List<UserAttendRec>();
            if (endTime < beginTime)
                return query;

            string strWhere = "attend_day >= \'";
            strWhere += beginTime + "\' and attend_day < \'" + endTime + "\'";

            if (devTypeIdLst != null && devTypeIdLst.Count > 0)
            {
                strWhere += " and dev_group in ( ";
                for (int i = 0; i < devTypeIdLst.Count - 1; i++)
                {
                    strWhere += devTypeIdLst[i] + ",";
                }
                strWhere += devTypeIdLst[devTypeIdLst.Count - 1] + ")";
            }

            if (null != departIdLst && departIdLst.Count > 0)
            {
                strWhere += " and depart_id in ( ";
                for (int i = 0; i < departIdLst.Count - 1; i++)
                {
                    strWhere += departIdLst[i] + ",";
                }
                strWhere += departIdLst[departIdLst.Count - 1] + ")";
            }

            if ("" != name)
            {
                strWhere += " and name like \'%" + name + "%\'";
            }
            if ("" != workSN)
            {
                strWhere += " and work_sn = \'" + workSN + "\'";
            }
            // 工作时长 
            if (workTime > 0)
            {
                strWhere += " and work_time >=" + workTime;
            }

            //add by gqy 20131030 去掉请假人员
            strWhere += "and leave_type_id < 50 ";

            //add by gqy 20131030 去除已删除人员 
            string totalWhere = "where c.delete_time is null";

            #region 选择条件是否选择了职务 

            if (null != principalIdList)
            {
                totalWhere += " and c.principal_id in ( ";
                for (int i = 0; i < principalIdList.Count - 1; i++)
                {
                    totalWhere += principalIdList[i] + ",";
                }
                totalWhere += principalIdList[principalIdList.Count - 1] + ")";
            }
            #endregion

            #region 选择条件是否选择了工种
            if (null != workTypeIdList)
            {
                totalWhere += " and c.work_type_id in ( ";
                for (int i = 0; i < workTypeIdList.Count - 1; i++)
                {
                    totalWhere += workTypeIdList[i] + ",";
                }
                totalWhere += workTypeIdList[workTypeIdList.Count - 1] + ")";
            }
            #endregion

            string strSQL = string.Format(@" select a.person_id,a.name,a.work_sn,a.work_cnt_sum,a.work_time_sum,a.depart_name,
                 b.count,a.attend_times_sum,c.work_type_id, c.work_type_name, c.principal_id, c.principal_name from 
                (select Count(name) as attend_times_sum, sum(work_cnt) as work_cnt_sum, sum(work_time) as work_time_sum , name, person_id,work_sn,depart_name from attend_record_sign_all 
                where {0} group by name,person_id,work_sn,depart_name)as a 
                left join (select Count(work_time),person_id from attend_record_sign_all where work_time >0 and {1} group by name,person_id )as b 
                            on a.person_id = b.person_id
                left join (select p.person_id as person_id, p.delete_time as delete_time, wt.work_type_id as work_type_id, wt.work_type_name as work_type_name, pri.principal_id as principal_id, 
                            pri.principal_name as principal_name from person p Left join work_type wt on p.work_type_id=wt.work_type_id 
                            Left join principal pri on p.principal_id=pri.principal_id) as c 
                          on a.person_id = c.person_id {2}", strWhere, strWhere, totalWhere);

            DataTable dt = DbAccess.PSQLPOOL.SelectDT(strSQL, "attend_record_base");
           
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }

            foreach (DataRow ar in dt.Rows)
            {
                UserAttendRec attend = new UserAttendRec();
                attend.attend_record_id = keyId++;

                //如果可为空需要判断   if(ar["attend_record_id"].ToString() != "")
                if (DBNull.Value != ar["person_id"])
                {
                    attend.person_id = int.Parse(ar["person_id"].ToString());
                }
                attend.person_name = ar["name"].ToString();
                int temp = 0;
                if (DBNull.Value != ar["work_time_sum"])
                {
                    temp = int.Parse(ar["work_time_sum"].ToString());
                    //化成分钟数，如果分钟数为一位，则在其前补零
                    if ((temp % 60) > 9)
                    {
                        attend.sum_work_time = (temp / 60).ToString() + ":" + (temp % 60).ToString();
                    }
                    else
                    {
                        attend.sum_work_time = (temp / 60).ToString() + ":0" + (temp % 60).ToString();
                    }
                }
                if (DBNull.Value != ar["attend_times_sum"])
                {
                    attend.sum_count = int.Parse(ar["attend_times_sum"].ToString());
                }
                attend.work_sn = ar["work_sn"].ToString();
                attend.depart_name = ar["depart_name"].ToString();

                attend.work_type_name = ar["work_type_name"].ToString();
                attend.principal_name = ar["principal_name"].ToString();


                if (DBNull.Value != ar["work_cnt_sum"])
                {
                    attend.sum_work_cnt = int.Parse(ar["work_cnt_sum"].ToString()) / 10.0;
                }


                if (DBNull.Value != ar["count"])
                {
                    int count = Int32.Parse(ar["count"].ToString());
                    //化成分钟数，如果分钟数为一位，则在其前补零
                    if ((temp / count % 60) > 9)
                    {
                        attend.avg_work_time = (temp / count / 60).ToString() + ":" + (temp / count % 60).ToString();
                    }
                    else
                    {
                        attend.avg_work_time = (temp / count / 60).ToString() + ":0" + (temp / count % 60).ToString();
                    }
                }
                else
                {
                    attend.avg_work_time = "0:0";
                }
                query.Add(attend);
            }
            return query;
        }

        /// <summary>
        /// 查询考勤详细信息
        /// </summary>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">截止时间</param>
        /// <param name="personId">人员ID</param>
        /// <returns>考勤记录</returns>
        [Query(HasSideEffects = true)]
        public IEnumerable<UserAttendRecDetail> IrisGetAttendSignDetail(DateTime beginTime, DateTime endTime,
             int[] devTypeIdLst, int workTime, int personId)
        {             

            List<UserAttendRecDetail> query = new List<UserAttendRecDetail>();
//            string strSQL = @"select attend_record_id, attend_day, attend_times, dev_group, in_out_times,in_well_time,is_valid,
//            class_order_name,depart_name,in_leave_type_name,out_leave_type_name,a.memo,name,work_sn,leave_type_name,out_well_time,
//            person_id,work_cnt,work_time,recog_sign_time from attend_record_sign_all as a left join leave_type as b on a.leave_type_id = b.leave_type_id ";

            string strSQL = @"select unnest(recog_sign_time) as signTime,attend_record_id, attend_day, attend_times, dev_group, in_out_times,in_well_time,is_valid,
            class_order_name,depart_name,in_leave_type_name,out_leave_type_name,a.memo,name,work_sn,leave_type_name,out_well_time,
            person_id,work_cnt,work_time from attend_record_sign_all as a left join leave_type as b on a.leave_type_id = b.leave_type_id "; 

            if (endTime < beginTime)
                return query;

            List<DateTime> listDate = GetHoliday(beginTime, endTime);
            strSQL += "where attend_day >= \'" + beginTime + "\' and attend_day < \'" + endTime + "\'";

            if (devTypeIdLst != null)
            {
                strSQL += " and a.dev_group in ( ";
                for (int i = 0; i < devTypeIdLst.Length - 1; i++)
                {
                    strSQL += devTypeIdLst[i] + ",";
                }
                strSQL += devTypeIdLst[devTypeIdLst.Length - 1] + ")";
            }

            strSQL += " and person_id = " + personId;
            //add by gqy 20131223 去掉请假人员
            strSQL += " and a.leave_type_id < 50 ";

            // 工作时长  ---add by gqy
            if (workTime > 0)
            {
                strSQL += " and work_time >=" + workTime;
            }

            DataTable dt = DbAccess.POSTGRESQL.Select(strSQL, "attend_record_sign");
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }

            UserAttendRecDetail attend = null;
            int attendRecordId = -1;
            foreach (DataRow ar in dt.Rows)
            {                
                //如果可为空需要判断   if(ar["attend_record_id"].ToString() != "")
                if (DBNull.Value != ar["attend_record_id"])
                {
                    attendRecordId = int.Parse(ar["attend_record_id"].ToString());
                }

                if (attend != null && attend.attend_record_id == attendRecordId)
                {
                    if (DBNull.Value != ar["signTime"])
                    {
                        attend.recog_sign_time += (ar["signTime"].ToString() + " | ");
                    }
                    else
                    {
                        attend.recog_sign_time += "                       | ";
                    }
                    continue;
                }
                if (attend != null)
                {
                    query.Add(attend);
                }
                attend = null;
                attend = new UserAttendRecDetail();
                attend.attend_record_id = attendRecordId;
                attend.recog_sign_time = "";
                if (DBNull.Value != ar["signTime"])
                {
                    attend.recog_sign_time += (ar["signTime"].ToString() + " | ");
                }
                else
                {
                    attend.recog_sign_time += "                           | ";
                }
                
                if (DBNull.Value != ar["attend_day"])
                {
                    attend.attend_day = DateTime.Parse(ar["attend_day"].ToString()).ToString("yyyy-MM-dd");
                    //判断是否为节假日 add by yuhaitao
                    if (listDate.Contains(DateTime.Parse(attend.attend_day)))
                    {
                        attend.DayType = 1;
                    }
                }
                if (DBNull.Value != ar["attend_times"])
                {
                    attend.attend_times = int.Parse(ar["attend_times"].ToString());
                }

                if (DBNull.Value != ar["dev_group"])
                {
                    attend.dev_group = (int.Parse(ar["dev_group"].ToString()) == 3) ? "出入井" : "上下班";
                }
                if (DBNull.Value != ar["in_out_times"])
                {
                    attend.in_out_times = int.Parse(ar["in_out_times"].ToString());
                }
                if (DBNull.Value != ar["in_well_time"])
                {
                    attend.in_well_time = DateTime.Parse(ar["in_well_time"].ToString());
                }
                if (DBNull.Value != ar["is_valid"])
                {
                    attend.is_valid = int.Parse(ar["is_valid"].ToString());
                }

                attend.class_order_name = ar["class_order_name"].ToString();
                attend.depart_name = ar["depart_name"].ToString();
                attend.in_leave_type_name = ar["in_leave_type_name"].ToString();
                attend.out_leave_type_name = ar["out_leave_type_name"].ToString();

                attend.memo = ar["memo"].ToString();
                attend.person_name = ar["name"].ToString();
                attend.work_sn = ar["work_sn"].ToString();
                attend.leave_type_name = ar["leave_type_name"].ToString();
                if (attend.leave_type_name == "正常")
                {
                    attend.leave_type_name_color = "0xFF000000";
                }
                else
                {
                    attend.leave_type_name_color = "0xFFFF0000";
                }

                if (DBNull.Value != ar["out_well_time"])
                {
                    attend.out_well_time = DateTime.Parse(ar["out_well_time"].ToString());
                }
                if (DBNull.Value != ar["person_id"])
                {
                    attend.person_id = int.Parse(ar["person_id"].ToString());
                }
                if (DBNull.Value != ar["work_cnt"])
                {
                    //工数数据库中以10的倍数存储，取出应除以10
                    attend.work_cnt = Int32.Parse(ar["work_cnt"].ToString()) / 10.0;
                }
                if (DBNull.Value != ar["work_time"])
                {
                    int temp = int.Parse(ar["work_time"].ToString());
                    //化成分钟数，如果分钟数为一位，则在其前补零
                    if ((temp % 60) > 9)
                    {
                        attend.work_time = (temp / 60).ToString() + ":" + (temp % 60).ToString();
                    }
                    else
                    {
                        attend.work_time = (temp / 60).ToString() + ":0" + (temp % 60).ToString();
                    }
                }                   
            }

            if (attend != null)
            {
                query.Add(attend);
            }
            return query;
        }
        #endregion
    }
}