/*************************************************************************
** 文件名:   DomainServiceIriskingAttend.cs
×× 主要类:   DomainServiceIriskingAttend
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   fjf
** 日  期:   2013-4-23
** 修改人:   lzc  gqy       cty
** 日  期:        2013-9-10 2013-11-2
** 描  述:   DomainServiceIriskingAttend类,后台数据库操作  添加修改删除考勤类型函数
**
** 版  本:   1.0.0
** 备  注:  命名及代码编写遵守C#编码规范
**
 * ***********************************************************************/
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


    // TODO: 创建包含应用程序逻辑的方法。
    [EnableClientAccess()]
    public partial class DomainServiceIriskingAttend : DomainService
    {

        #region  静态变量
        static string strIP = System.Web.Configuration.WebConfigurationManager.AppSettings["ServerIP"].ToString();
        static ushort Port = UInt16.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["ServerPort"].ToString());

        static int keyId = 0;
        #endregion        

        #region  从配置文件获得配置信息

        /// <summary>
        /// 获取软件应用环境--分为矿山、非矿山
        /// </summary>
        /// <returns></returns>
        [Invoke]
        public bool GetIsMineApp()
        {
            bool result;
            if (bool.TryParse(System.Web.Configuration.WebConfigurationManager.AppSettings["IsMine"].ToString(), out result))
                return result;
           
            return false;
        }

        /// <summary>
        /// 获得具体的软件应用环境
        /// </summary>
        /// <returns></returns>
        [Invoke]
        public string GetApplicationType()
        {
            try
            {
                string appType = System.Web.Configuration.WebConfigurationManager.AppSettings["AppType"].ToString();
                return appType;
            }
            catch( Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return "";
            }
        }
        
        /// <summary>
        /// 获取是否显示识别记录类型列
        /// </summary>
        /// <returns></returns>
        [Invoke]
        public int GetIsShowRecogType()
        {
            int result;
            if (Int32.TryParse(System.Web.Configuration.WebConfigurationManager.AppSettings["IsShowRecogType"].ToString(), out result))
            {
                return result;
            }
            return 1;
        }

        /// <summary>
        /// cty
        /// 获取备份数据库服务端的IP地址
        /// </summary>
        /// <returns></returns>
        [Invoke]
        public string GetBackupServerIP()
        {
            string ip = "127.0.0.1";
            ip = HttpContext.Current.Request.ServerVariables.Get("LOCAL_ADDR").ToString();
            if (ip == "::1")
            {
                ip = "127.0.0.1";
            }
            return ip;
            //return System.Web.Configuration.WebConfigurationManager.AppSettings["BackupServerIP"].ToString();
        }

        /// <summary>
        /// cty
        /// 获取备份数据库服务端的端口号
        /// </summary>
        /// <returns></returns>
        [Invoke]
        public string GetBackupServerPort()
        {
            return System.Web.Configuration.WebConfigurationManager.AppSettings["BackupServerPort"].ToString();
        }

        /// <summary>
        /// 获取是否支持签到班,记工时班次 0为都不支持  1为不支持签到班2支持记工时
        /// </summary>
        /// <returns></returns>
        [Invoke]
        public int GetIsSupportClassOrderSign()
        {
            int result=0;
            if (int.TryParse(System.Web.Configuration.WebConfigurationManager.AppSettings["SupportClassOrderSign"].ToString(), out result))
                return result;

            return 0;
        }

        #endregion

        #region

        /// <summary>
        /// 以获取考勤记录为例子，根据需要编写自己的函数
        /// </summary>
        /// <param name="SQLstr">SQL语句</param>
        /// <returns>考勤记录</returns>
        public IEnumerable<attend_record_base> IrisTest(string SQLstr)
        {
            List<attend_record_base> query = new List<attend_record_base>();
            DataTable dt = DbAccess.POSTGRESQL.Select(SQLstr, "attend_record_base");
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }
            foreach (DataRow ar in dt.Rows)
            {
                attend_record_base attend = new attend_record_base();
                //如果可为空需要判断   if(ar["attend_record_id"].ToString() != "")
                attend.attend_record_id = int.Parse(ar["attend_record_id"].ToString());
                query.Add(attend);
            }
            return query;
        }

        /// <summary>
        /// 获取人员总数
        /// </summary>
        /// <returns></returns>
        [Invoke]
        public int IrisGetPersonSum()
        {
            string strSQL = "select count(*) as count from person_base ";

            DataTable dt = DbAccess.POSTGRESQL.Select(strSQL, "person");
            if (null == dt || dt.Rows.Count < 1)
            {
                return -1;
            }
            return int.Parse(dt.Rows[0]["count"].ToString());
        }

        /// <summary>
        /// 查询考勤详细信息
        /// </summary>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">截止时间</param>
        /// <param name="personId">人员ID</param>
        /// <returns>考勤记录</returns>
        public IEnumerable<UserAttendRecDetail> IrisGetAttendDetail(DateTime beginTime, DateTime endTime,
             int[] devTypeIdLst, int workTime, int personId )
        {

            List<UserAttendRecDetail> query = new List<UserAttendRecDetail>();
            string strSQL = "select * from attend_record_all as a left join leave_type as b on a.leave_type_id = b.leave_type_id ";
            //strSQL += " where a.attend_day >= \'" + beginTime + "\' and a.attend_day <= \'" + endTime + "\' ";
            //strSQL += " and a.person_id = " + personId;

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

            DataTable dt = DbAccess.POSTGRESQL.Select(strSQL, "attend_record_base");
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }
            foreach (DataRow ar in dt.Rows)
            {
                UserAttendRecDetail attend = new UserAttendRecDetail();
                //如果可为空需要判断   if(ar["attend_record_id"].ToString() != "")
                if (DBNull.Value != ar["attend_record_id"])
                {
                    attend.attend_record_id = int.Parse(ar["attend_record_id"].ToString());
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


                query.Add(attend);
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
        private List<DateTime> GetHoliday(DateTime beginTime, DateTime endTime)
        {
            if (endTime.Year >= 9999)
            {
                string sql = "select max(attend_day) from attend_record_all";
                DataTable dt = DbAccess.POSTGRESQL.Select(sql, "attend_record_all");
                if (null == dt || dt.Rows.Count < 1)
                {
                    return new List<DateTime>();
                }
                endTime = DateTime.Parse(dt.Rows[0][0].ToString());
            }
            List<DateTime> listDate = new List<DateTime>();
            string querySql = string.Format(@"select * from festival where begin_time >= '{0}' and begin_time < '{1}'", beginTime, endTime);
            List<FestivalInfo> festivalList = (List<FestivalInfo>)GetFestivalBySql(querySql);
            if (festivalList != null)
            {
                foreach (FestivalInfo item in festivalList)
                {
                    for (DateTime temp = Convert.ToDateTime(item.begin_time); temp <= Convert.ToDateTime(item.end_time); temp = temp.AddDays(1))
                    {
                        listDate.Add(temp);
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
                    listDate.Add(temp);
                }
            }
            return listDate;
        }
        #endregion

        /// <summary>
        /// 获取所有部门
        /// </summary>
        /// <returns></returns>
        public IEnumerable<depart> IrisGetDepart()
        {
            string strSQL = "select * from depart where delete_time is null";
            List<depart> query = new List<depart>();
            DataTable dt = DbAccess.POSTGRESQL.Select(strSQL, "depart");
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }
            foreach (DataRow ar in dt.Rows)
            {
                depart department = new depart();
                //如果可为空需要判断   if(ar["attend_record_id"].ToString() != "")
                department.depart_id = int.Parse(ar["depart_id"].ToString());
                department.depart_name = ar["depart_name"].ToString();
                if (DBNull.Value != ar["parent_depart_id"])
                {
                    department.parent_depart_id = Int32.Parse(ar["parent_depart_id"].ToString());
                }
                query.Add(department);
            }
            return query;
        }

        /// <summary>
        /// 更新函数，如果不存在，则UserPersonSimple类在前台不允许修改
        /// </summary>
        /// <param name="person"></param>
        [Update]
        public void IrisUpdateUserPersonSimple(UserPersonSimple person)
        {
        }

        /// <summary>
        /// 获取所人员简单信息
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UserPersonSimple> IrisGetUserPersonSimple()
        {
            string strSQL = "select person_id,work_sn, name, person_base.depart_id as depart_id, depart.depart_name as depart_name ";
            strSQL += " from person_base LEFT JOIN  depart on depart.depart_id = person_base.depart_id  where person_base.delete_time is null";
            List<UserPersonSimple> query = new List<UserPersonSimple>();
            DataTable dt = DbAccess.POSTGRESQL.Select(strSQL, "person");
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }
            foreach (DataRow ar in dt.Rows)
            {
                UserPersonSimple personSimple = new UserPersonSimple();
                //如果可为空需要判断   if(ar["attend_record_id"].ToString() != "")
                if (DBNull.Value != ar["person_id"])
                {
                    personSimple.person_id = int.Parse(ar["person_id"].ToString());
                }
                personSimple.person_name = ar["name"].ToString();
                if (DBNull.Value != ar["depart_id"])
                {
                    personSimple.depart_id = int.Parse(ar["depart_id"].ToString());
                }

                personSimple.is_select = true;
                personSimple.work_sn = ar["work_sn"].ToString();
                personSimple.depart_name = ar["depart_name"].ToString();
                query.Add(personSimple);
            }
            return query;
        }


        /// <summary>
        /// 获取所人员简单信息
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UserPersonSimple> IrisGetUserPersonSimpleForOperatorDepart(List<int> lstDepartID)
        {
            string strSQL = "select person_id,work_sn, name, person_base.depart_id as depart_id, depart.depart_name as depart_name ";
            strSQL += " from person_base LEFT JOIN  depart on depart.depart_id = person_base.depart_id  where person_base.delete_time is null";
            List<UserPersonSimple> query = new List<UserPersonSimple>();


            if (lstDepartID != null && lstDepartID.Count > 0)
            {
                strSQL += " and person_base.depart_id in ( ";
                for (int i = 0; i < lstDepartID.Count - 1; i++)
                {
                    strSQL += lstDepartID[i] + ",";
                }
                strSQL += lstDepartID[lstDepartID.Count - 1] + ")";
            }

            strSQL += "order by depart.depart_name,name";
            DataTable dt = DbAccess.POSTGRESQL.Select(strSQL, "person");
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }
            foreach (DataRow ar in dt.Rows)
            {
                UserPersonSimple personSimple = new UserPersonSimple();
                //如果可为空需要判断   if(ar["attend_record_id"].ToString() != "")
                if (DBNull.Value != ar["person_id"])
                {
                    personSimple.person_id = int.Parse(ar["person_id"].ToString());
                }
                personSimple.person_name = ar["name"].ToString();
                if (DBNull.Value != ar["depart_id"])
                {
                    personSimple.depart_id = int.Parse(ar["depart_id"].ToString());
                }

                personSimple.is_select = true;
                personSimple.work_sn = ar["work_sn"].ToString();
                personSimple.depart_name = ar["depart_name"].ToString();
                query.Add(personSimple);
            }
            return query;
        }


        /// <summary>
        /// 获取所人员简单信息 通过工号、人员名字、部门ID
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UserPersonSimple> IrisGetNeedUserPersonSimple(string workSN, string personName, int departID)
        {
            string strSQL = "select person_id,work_sn, name, person_base.depart_id as depart_id, depart.depart_name as depart_name ";
            strSQL += " from person_base LEFT JOIN  depart on depart.depart_id = person_base.depart_id  where person_base.delete_time is null";

            if (workSN != null && workSN != "")
            {
                strSQL += " and person_base.work_sn = \'" + workSN + "\'";
            }
            if (personName != null && personName != "")
            {
                strSQL += " and person_base.name like \'" + personName + "\'";
            }
            if (departID > 0)
            {
                strSQL += " and depart.depart_id = " + departID;
            }
            strSQL += " Order by name";
            List<UserPersonSimple> query = new List<UserPersonSimple>();
            DataTable dt = DbAccess.POSTGRESQL.Select(strSQL, "person");
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }
            foreach (DataRow ar in dt.Rows)
            {
                UserPersonSimple personSimple = new UserPersonSimple();
                //如果可为空需要判断   if(ar["attend_record_id"].ToString() != "")
                if (DBNull.Value != ar["person_id"])
                {
                    personSimple.person_id = int.Parse(ar["person_id"].ToString());
                }
                personSimple.person_name = ar["name"].ToString();
                if (DBNull.Value != ar["depart_id"])
                {
                    personSimple.depart_id = int.Parse(ar["depart_id"].ToString());
                }

                personSimple.is_select = true;
                personSimple.work_sn = ar["work_sn"].ToString();
                personSimple.depart_name = ar["depart_name"].ToString();
                query.Add(personSimple);
            }
            return query;
        }

        /// <summary>
        /// 通过工号获取所人员简单信息
        /// </summary>
        /// <returns></returns>
        [Invoke]
        public UserPersonSimple IrisGetUserPersonSimpleForWorkSN(string work_sn)
        {
            string strSQL = "select person_id,work_sn, name, person_base.depart_id as depart_id, depart.depart_name as depart_name ";
            strSQL += " from person_base LEFT JOIN  depart on depart.depart_id = person_base.depart_id  where person_base.delete_time is null";
            strSQL += " and work_sn = \'" + work_sn + "\'";
            UserPersonSimple personSimple = new UserPersonSimple();
            DataTable dt = DbAccess.POSTGRESQL.Select(strSQL, "person");
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }
            foreach (DataRow ar in dt.Rows)
            {
                //如果可为空需要判断   if(ar["attend_record_id"].ToString() != "")
                if (DBNull.Value != ar["person_id"])
                {
                    personSimple.person_id = int.Parse(ar["person_id"].ToString());
                }
                personSimple.person_name = ar["name"].ToString();
                if (DBNull.Value != ar["depart_id"])
                {
                    personSimple.depart_id = int.Parse(ar["depart_id"].ToString());
                }
                personSimple.work_sn = ar["work_sn"].ToString();
                personSimple.is_select = true;
                personSimple.depart_name = ar["depart_name"].ToString();
                break;
            }
            return personSimple;
        }

        /// <summary>
        /// 获取所人员简单信息
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UserPersonSimple> IrisGetUserPersonSimpleForDepart(int depart_id)
        {
            string strSQL = "select person_id,work_sn, name, person_base.depart_id as depart_id, depart.depart_name as depart_name ";
            strSQL += " from person_base LEFT JOIN  depart on depart.depart_id = person_base.depart_id  where person_base.delete_time is null ";
            strSQL += " and person_base.depart_id = " + depart_id;

            List<UserPersonSimple> query = new List<UserPersonSimple>();
            DataTable dt = DbAccess.POSTGRESQL.Select(strSQL, "person");
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }

            foreach (DataRow ar in dt.Rows)
            {
                UserPersonSimple personSimple = new UserPersonSimple();
                //如果可为空需要判断   if(ar["attend_record_id"].ToString() != "")
                if (DBNull.Value != ar["person_id"])
                {
                    personSimple.person_id = int.Parse(ar["person_id"].ToString());
                }
                personSimple.person_name = ar["name"].ToString();
                if (DBNull.Value != ar["depart_id"])
                {
                    personSimple.depart_id = int.Parse(ar["depart_id"].ToString());
                }
                personSimple.work_sn = ar["work_sn"].ToString();
                personSimple.depart_name = ar["depart_name"].ToString();
                query.Add(personSimple);
            }
            return query;
        }

        /// <summary>
        /// 获取所有考勤类型信息
        /// cty 修改（2013-10-28）
        /// </summary>
        /// <param name="id">0-50为系统所有，50以后为用户添加</param>
        /// <returns></returns>
        public IEnumerable<LeaveType> IrisGetLeaveType(int id)
        {
            string strSQL = "select * from leave_type where leave_type_id > " + id;
            List<LeaveType> query = new List<LeaveType>();
            DataTable dt = DbAccess.POSTGRESQL.Select(strSQL, "leave_type");
            if (null == dt || dt.Rows.Count < 1)
            {
                return query;
            }
            foreach (DataRow ar in dt.Rows)
            {
                LeaveType leaveType = new LeaveType();
                //如果可为空需要判断   if(ar["attend_record_id"].ToString() != "")
                leaveType.leave_type_id = int.Parse(ar["leave_type_id"].ToString());
                leaveType.leave_type_name = ar["leave_type_name"].ToString();
                leaveType.attend_sign = ar["attend_sign"].ToString();//cty
                leaveType.is_normal_attend = short.Parse(ar["is_normal_attend"].ToString());//cty
                switch (leaveType.is_normal_attend)
                {
                    case 0:
                        leaveType.is_normal_attendStr = "不计入出勤";
                        break;
                    case 1:
                        leaveType.is_normal_attendStr = "计入出勤";
                        break;
                    case 2:
                        leaveType.is_normal_attendStr = "有条件计入出勤";
                        break;
                }
                leaveType.is_schedule = int.Parse(ar["is_schedule"].ToString());//cty
                switch (leaveType.is_schedule)
                {
                    case 0:
                        leaveType.is_scheduleStr = "否";
                        break;
                    case 1:
                        leaveType.is_scheduleStr = "是";
                        break;
                }
                leaveType.memo = ar["memo"].ToString();//cty
                query.Add(leaveType);
            }
            return query;
        }

        /// <summary>
        /// cty
        /// 允许操作
        /// </summary>
        /// <param name="t"></param>
        [Update]
        public void TestIrisGetLeaveType(LeaveType t)
        {

        }

        /// <summary>
        /// 考勤记录查询
        /// </summary>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">截止时间</param>
        /// <param name="name">人员名字</param>
        /// <param name="workSN">人员工号</param>
        /// <returns></returns>
        public IEnumerable<UserAttendRecDetail> IrisGetAttendRec(DateTime beginTime, DateTime endTime,
            string name, string workSN)
        {

            List<UserAttendRecDetail> query = new List<UserAttendRecDetail>();
            if (endTime < beginTime)
                return query;

            string strWhere = "attend_day >= \'";
            strWhere += beginTime + "\' and attend_day < \'" + endTime + "\'";            
            
            
            if ("" != name)
            {
                strWhere += "and a.person_id in (select distinct person_id from person_base  where name like '%" + name + "%')";
                //strWhere += " and name like \'%" + name + "%\'";
            }
           
            if ("" != workSN)
            {
                strWhere += "and a.person_id in (select distinct person_id from person_base  where work_sn like '%"+workSN+"%') ";
                //strWhere += " and work_sn = \'" + workSN + "\'";
            }
          

            //add by gqy 20131030 去掉请假人员
            strWhere += "and a.leave_type_id < 50  order by name,a.attend_day,a.in_well_time";
           
            

            string strSQL="select a.attend_record_id,a.person_id,a.in_id,a.in_well_time,a.out_id,"+
           " a.out_well_time,a.leave_type_id,lt.leave_type_name,a.create_time," +
            " a.work_time,a.class_order_id,a.attend_day,a.work_cnt,a.recog_id,"+
            " p.name,p.work_sn,clo.class_order_name"+
          " from  attend_record_jigongshi as a left join leave_type as b"+
           " on a.leave_type_id = b.leave_type_id left join person_base as p on p.person_id=a.person_id "+
           "left join leave_type as lt on lt.leave_type_id=a.leave_type_id " +
          " left join class_order_base as clo on clo.class_order_id=a.class_order_id where "+strWhere;
            DataTable dt = DbAccess.PSQLPOOL.SelectDT(strSQL, "attend_record_jigongshi");
           
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }
           
            foreach (DataRow ar in dt.Rows)
            {
                UserAttendRecDetail attend = new UserAttendRecDetail();
                //查询稽核路径
                int[] _listRecogIds = new int[] { };
                string recogPath = "";
                object aa = (object)ar["recog_id"];
                #region 组装 识别路径
                if (aa != null)//转化成数组
                {
                    _listRecogIds = (int[])aa;

                    string strSqlFirst = "select at_position  from person_recog_base where person_recog_log_id="
                        + _listRecogIds[0];

                    DataTable dt_person_recog_first = DbAccess.PSQLPOOL.SelectDT(strSqlFirst, "");
                    if (dt_person_recog_first != null && dt_person_recog_first.Rows.Count > 0)
                    {
                        recogPath += (dt_person_recog_first.Rows[0]["at_position"].ToString() == "" ? "无" :

dt_person_recog_first.Rows[0]["at_position"].ToString());

                        for (int i = 1; i < _listRecogIds.Length; i++)
                        {
                            strSqlFirst = "select at_position from person_recog_base where person_recog_log_id=" + _listRecogIds[i];

                            DataTable dtofrecog = DbAccess.PSQLPOOL.SelectDT(strSqlFirst, "");

                            if (dtofrecog != null && dtofrecog.Rows.Count > 0)
                            {
                                recogPath += ">" + (dtofrecog.Rows[0]["at_position"].ToString() == "" ? "无" :

    dtofrecog.Rows[0]["at_position"].ToString());
                            }
                        }

                    }
                }
                #endregion
                attend.attend_record_id = keyId++;
                attend.attend_path=recogPath;
                //如果可为空需要判断   if(ar["attend_record_id"].ToString() != "")
                if (DBNull.Value != ar["person_id"])
                {
                    attend.person_id = int.Parse(ar["person_id"].ToString());
                }
                attend.person_name = ar["name"].ToString();
                attend.work_sn = ar["work_sn"].ToString();
                if (DBNull.Value != ar["leave_type_name"])
                {
                    attend.leave_type_name = ar["leave_type_name"].ToString();
                   // attend.leave_type_id = Convert.ToInt32(ar["leave_type_name"]);
                }
                if (ar["in_well_time"] != DBNull.Value)
                {
                    attend.in_well_time = Convert.ToDateTime(ar["in_well_time"]);

                    attend.in_well_time_str = Convert.ToDateTime(ar["in_well_time"]).ToString("yyyy-MM-dd HH:mm:ss");
                }
                if (ar["out_well_time"] != DBNull.Value)
                {
                    attend.out_well_time_str = Convert.ToDateTime(ar["out_well_time"]).ToString("yyyy-MM-dd HH:mm:ss");// Convert.ToDateTime(ar["out_well_time"]);

                    attend.out_well_time = Convert.ToDateTime(ar["out_well_time"]);// Convert.ToDateTime(ar["out_well_time"]);
                }
                if (ar["attend_day"] != DBNull.Value)
                {
                    attend.attend_day = Convert.ToDateTime(ar["attend_day"]).ToString("yyyy-MM-dd HH:mm:ss");
                }
                //稽核路径
               // attend.attend_path=

                if (DBNull.Value != ar["work_cnt"])
                {
                    attend.work_cnt = int.Parse(ar["work_cnt"].ToString()) / 10.0;
                }
               
                if (DBNull.Value != ar["work_time"])
                {
                    attend.work_time = ar["work_time"].ToString();

                    attend.work_time_str = IriskingAttend.Web.Manager.Helper.MinutesToDate(Convert.ToInt32(ar["work_time"]),false);
                }
                attend.class_order_name = ar["class_order_name"].ToString();
               
                query.Add(attend);
            }
            return query;
        }

        /// <summary>
        /// 更新函数，如果不存在，则UserPersonRecogLog类在前台不允许修改
        /// add by  gqy
        /// </summary>
        /// <param name="o"></param>
        [Update]
        public void IrisUpdateUserPersonRecogLog(UserPersonRecogLog o)
        {
        }

        /// <summary>
        /// 获取人员识别记录
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="person_id"></param>
        /// <returns></returns>
        public IEnumerable<UserPersonRecogLog> IrisGetPersonRecog(DateTime beginTime, DateTime endTime, int person_id)
        {
            List<UserPersonRecogLog> query = new List<UserPersonRecogLog>();

            string strSQL = "select name,work_sn, depart_name,person_recog_log_id,person_id,class_type_id,class_type_name,recog_time,"
                          + "recog_type,at_position,device_sn,dev_type,memo,attend_state, operator_name from person_recog_log  where ";

            if (beginTime > endTime)
                return query;

            strSQL += " person_id = " + person_id;
            strSQL += " and recog_time >= \'" + beginTime + "\' ";
            strSQL += " and recog_time < \'" + endTime + "\' ";
            strSQL += " and (delete_time is null or attend_state != 0)";

            DataTable dt = DbAccess.POSTGRESQL.Select(strSQL, "person_recog_log");
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }

            int IsShowRecogType = GetIsShowRecogType();

            foreach (DataRow ar in dt.Rows)
            {
                UserPersonRecogLog recorg = new UserPersonRecogLog();

                recorg.at_position = ar["at_position"].ToString();
                recorg.depart_name = ar["depart_name"].ToString();
                if (DBNull.Value != ar["dev_type"])
                {
                    recorg.dev_type_value = int.Parse(ar["dev_type"].ToString());
                    switch (recorg.dev_type_value)
                    {
                        case 1:
                            recorg.dev_type = "入井";
                            break;
                        case 2:
                            recorg.dev_type = "出井";
                            break;
                        case 3:
                            recorg.dev_type = "出入井";
                            break;
                        case 4:
                            recorg.dev_type = "上班";
                            break;
                        case 5:
                            recorg.dev_type = "下班";
                            break;
                        case 6:
                            recorg.dev_type = "上下班";
                            break;
                        default:
                            break;
                    }
                }
                recorg.class_type_name = ar["class_type_name"].ToString();
                recorg.device_sn = ar["device_sn"].ToString();
                recorg.memo = ar["memo"].ToString();
                recorg.person_name = ar["name"].ToString();
                if (DBNull.Value != ar["person_recog_log_id"])
                {
                    recorg.person_recog_id = int.Parse(ar["person_recog_log_id"].ToString());
                }
                if (DBNull.Value != ar["person_id"])
                {
                    recorg.person_id = int.Parse(ar["person_id"].ToString());
                }


                if (DBNull.Value != ar["recog_type"])
                {
                    switch (int.Parse(ar["recog_type"].ToString()))
                    {
                        case 0:
                            recorg.recog_type = "人工补加";

                            //若设置此值为2，则人工补加的识别记录与虹膜识别的识别记录底色颜色显示相同
                            if (IsShowRecogType == 2)
                            {
                                recorg.recog_type_color = "0xFF000000";
                            }
                            else
                            {
                                recorg.recog_type_color = "0xFF505050";
                            }
                            break;
                        case 1:
                            recorg.recog_type = "虹膜识别";
                            recorg.recog_type_color = "0xFF000000";
                            break;
                        case 2:
                            recorg.recog_type = "人脸识别";
                            recorg.recog_type_color = "0xFF000000";
                            break;
                        default:
                            break;
                    }
                }
                if (DBNull.Value != ar["attend_state"])
                {
                    recorg.attend_state_color = "Red";
                    switch (int.Parse(ar["attend_state"].ToString()))
                    {
                        case 0:
                            recorg.attend_state = "已删除数据";
                            break;
                        case 1:
                            recorg.attend_state = "未考勤";
                            break;
                        case 2:
                            recorg.attend_state = "已考勤";
                            recorg.attend_state_color = "#FFF1F1F1";
                            break;
                        case 3:
                            recorg.attend_state = "重复数据";
                            break;
                        case 4:
                            recorg.attend_state = "不完整数据";
                            break;
                        default:
                            recorg.attend_state = "未考勤";
                            break;
                    }
                }
                recorg.recog_time = DateTime.Parse(ar["recog_time"].ToString());
                recorg.work_sn = ar["work_sn"].ToString();
                recorg.operator_name = ar["operator_name"].ToString();
                query.Add(recorg);
            }


            return query;
        }


        /// <summary>
        /// 获取人员识别记录
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="devTypeIdLst">设备类型</param>
        /// <param name="person_id"></param>
        /// <returns></returns>
        public IEnumerable<UserPersonRecogLog> IrisGetPersonRecogByDevType(DateTime beginTime, DateTime endTime, int[] devTypeIdLst, int person_id)
        {
            List<UserPersonRecogLog> query = new List<UserPersonRecogLog>();

            string strSQL = "select name,work_sn, depart_name,person_recog_log_id,person_id,class_type_id,class_type_name,recog_time,"
                          + "recog_type,at_position,device_sn,dev_type,memo,operator_name,attend_state from person_recog_log  where ";

            if (beginTime > endTime)
                return query;
            List<DateTime> listDate = GetHoliday(beginTime, endTime);
            strSQL += " person_id = " + person_id;
            strSQL += " and recog_time >= \'" + beginTime + "\' ";
            strSQL += " and recog_time < \'" + endTime + "\' ";
            strSQL += " and (delete_time is null or attend_state != 0)";

            if (devTypeIdLst != null)
            {
                strSQL += " and dev_type in ( ";
                for (int i = 0; i < devTypeIdLst.Length - 1; i++)
                {
                    strSQL += devTypeIdLst[i] + ",";
                }
                strSQL += devTypeIdLst[devTypeIdLst.Length - 1] + ")";
            }

            DataTable dt = DbAccess.POSTGRESQL.Select(strSQL, "UserPersonRecogLog");
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }
            foreach (DataRow ar in dt.Rows)
            {
                UserPersonRecogLog recorg = new UserPersonRecogLog();

                recorg.at_position = ar["at_position"].ToString();
                recorg.depart_name = ar["depart_name"].ToString();
                if (DBNull.Value != ar["dev_type"])
                {
                    recorg.dev_type_value = int.Parse(ar["dev_type"].ToString());
                    switch (recorg.dev_type_value)
                    {
                        case 1:
                            recorg.dev_type = "入井";
                            break;
                        case 2:
                            recorg.dev_type = "出井";
                            break;
                        case 3:
                            recorg.dev_type = "出入井";
                            break;
                        case 4:
                            recorg.dev_type = "上班";
                            break;
                        case 5:
                            recorg.dev_type = "下班";
                            break;
                        case 6:
                            recorg.dev_type = "上下班";
                            break;
                        default:
                            break;
                    }
                }
                recorg.class_type_name = ar["class_type_name"].ToString();
                recorg.device_sn = ar["device_sn"].ToString();
                recorg.memo = ar["memo"].ToString();
                recorg.person_name = ar["name"].ToString();
                if (DBNull.Value != ar["person_recog_log_id"])
                {
                    recorg.person_recog_id = int.Parse(ar["person_recog_log_id"].ToString());
                }
                if (DBNull.Value != ar["person_id"])
                {
                    recorg.person_id = int.Parse(ar["person_id"].ToString());
                }


                if (DBNull.Value != ar["recog_type"])
                {
                    switch (int.Parse(ar["recog_type"].ToString()))
                    {
                        case 0:
                            recorg.recog_type = "人工补加";
                            recorg.recog_type_color = "0xFF505050";
                            break;
                        case 1:
                            recorg.recog_type = "虹膜识别";
                            recorg.recog_type_color = "0xFF000000";
                            break;
                        case 2:
                            recorg.recog_type = "人脸识别";
                            recorg.recog_type_color = "0xFF000000";
                            break;
                        default:
                            break;
                    }
                }
                if (DBNull.Value != ar["attend_state"])
                {
                    recorg.attend_state_color = "Red";
                    switch (int.Parse(ar["attend_state"].ToString()))
                    {
                        case 0:
                            recorg.attend_state = "已删除数据";
                            break;
                        case 1:
                            recorg.attend_state = "未考勤";
                            break;
                        case 2:
                            recorg.attend_state = "已考勤";
                            recorg.attend_state_color = "#FFF1F1F1";
                            break;
                        case 3:
                            recorg.attend_state = "重复数据";
                            break;
                        case 4:
                            recorg.attend_state = "不完整数据";
                            break;
                        default:
                            recorg.attend_state = "未考勤";
                            break;
                    }
                }
                recorg.recog_time = DateTime.Parse(ar["recog_time"].ToString());
                if (listDate.Contains(DateTime.Parse(recorg.recog_time.ToString("yyyy-MM-dd"))))
                {
                    recorg.DayType = 1;
                }
                recorg.work_sn = ar["work_sn"].ToString();
                recorg.operator_name = ar["operator_name"].ToString();//add by yht 2014-09-30
                query.Add(recorg);
            }


            return query;
        }

        /// <summary>
        /// 获取人员识别记录 
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="person_id"></param>
        /// <returns></returns>
        public IEnumerable<UserAllPersonRecogLog> IrisGetAllPersonRecog(DateTime beginTime, DateTime endTime,
                                                                      string depart_name, string person_name, string work_sn)
        {
            List<UserAllPersonRecogLog> query = new List<UserAllPersonRecogLog>();

            if (beginTime > endTime)
            {
                return query;
            }

            string strSQL = "select tbPersonID.person_id as person_id ,tbPersonID.count as count, name, depart_name,work_sn from" +
                            " (select person_id, count(*) as count from person_recog_log  where 1=1 ";
            if (person_name != "")
            {
                strSQL += " name like\'%" + person_name + "%\' and";
            }
            if (depart_name != null && depart_name != "")
            {
                strSQL += " depart_name = \'" + depart_name + "\' and ";
            }
            if (work_sn != "")
            {
                strSQL += " work_sn = \'" + work_sn + "\' and ";
            }

            strSQL += " and recog_time >= \'" + beginTime + "\' ";
            strSQL += " and recog_time < \'" + endTime + "\' ";
            strSQL += " and (delete_time is null or attend_state != 0)";
            strSQL += " group by person_id ) as tbPersonID,person where tbPersonID.person_id = person.person_id " +
                      " order by depart_name,name";


            DataTable dt = DbAccess.POSTGRESQL.Select(strSQL, "UserPersonRecogLog");
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }
            foreach (DataRow ar in dt.Rows)
            {
                UserAllPersonRecogLog recorg = new UserAllPersonRecogLog();

                if (DBNull.Value != ar["person_id"])
                {
                    recorg.person_id = int.Parse(ar["person_id"].ToString());
                }
                else
                {
                    continue;
                }

                if (DBNull.Value != ar["count"])
                {
                    recorg.record_count = int.Parse(ar["count"].ToString());

                }
                recorg.depart_name = ar["depart_name"].ToString();
                recorg.person_name = ar["name"].ToString();
                recorg.work_sn = ar["work_sn"].ToString();

                query.Add(recorg);
            }
            return query;
        }


        /// <summary>
        /// 获取当前井下人员
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UserInWellPerson> IrisGetInWellPerson()
        {
            List<UserInWellPerson> query = new List<UserInWellPerson>();
            int overTime = 0;
            int alertTime = 0;
            int accidentTime = 0;
            string strSQLSysParam = " select over_time,alert_time,accident_time from system_param";
            DataTable dtSysParam = DbAccess.POSTGRESQL.Select(strSQLSysParam, "SystemParam");
            if (null != dtSysParam && dtSysParam.Rows.Count > 0)
            {
                if (DBNull.Value != dtSysParam.Rows[0]["over_time"])
                {
                    overTime = int.Parse(dtSysParam.Rows[0]["over_time"].ToString());
                }
                if (DBNull.Value != dtSysParam.Rows[0]["alert_time"])
                {
                    alertTime = int.Parse(dtSysParam.Rows[0]["alert_time"].ToString());
                }
                if (DBNull.Value != dtSysParam.Rows[0]["accident_time"])
                {
                    accidentTime = int.Parse(dtSysParam.Rows[0]["accident_time"].ToString());
                }
            }

            string strSQL = "select in_out_id,work_sn, name,depart_name,class_order_name,dev_group,in_time from in_out_well where 1=1 ";
            strSQL += " and out_time is null and in_time is not null ";

            DataTable dt = DbAccess.POSTGRESQL.Select(strSQL, "UserInWellPerson");
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }
            foreach (DataRow ar in dt.Rows)
            {
                UserInWellPerson inWellPerson = new UserInWellPerson();


                if (DBNull.Value != ar["in_out_id"])
                {
                    inWellPerson.in_out_id = int.Parse(ar["in_out_id"].ToString());
                }
                inWellPerson.class_order_name = ar["class_order_name"].ToString();
                inWellPerson.depart_name = ar["depart_name"].ToString();

                if (DBNull.Value != ar["dev_group"])
                {
                    switch (int.Parse(ar["dev_group"].ToString()))
                    {
                        case 3:
                            inWellPerson.dev_group = "出入井";
                            break;
                        case 6:
                            inWellPerson.dev_group = "上下班";
                            break;
                        default:
                            break;
                    }
                }
                if (DBNull.Value != ar["in_time"])
                {
                    inWellPerson.in_time = DateTime.Parse(ar["in_time"].ToString());
                }
                inWellPerson.person_name = ar["name"].ToString();
                inWellPerson.work_sn = ar["work_sn"].ToString();

                //化成分钟数，如果分钟数为一位，则在其前补零
                if (Math.Abs((DateTime.Now - inWellPerson.in_time).Minutes) > 9)
                {
                    inWellPerson.work_time = Math.Truncate((DateTime.Now - inWellPerson.in_time).TotalHours).ToString("f0") + ":"
                        + Math.Abs((DateTime.Now - inWellPerson.in_time).Minutes).ToString();
                }
                else
                {
                    inWellPerson.work_time = Math.Truncate((DateTime.Now - inWellPerson.in_time).TotalHours).ToString("f0") + ":0"
                        + Math.Abs((DateTime.Now - inWellPerson.in_time).Minutes).ToString();
                }
                int work_time = (int)(DateTime.Now - inWellPerson.in_time).TotalMinutes;
                if (work_time < overTime)
                {
                    inWellPerson.work_state = "正常";
                }
                else if (work_time < alertTime)
                {
                    inWellPerson.work_state = "超时";
                }
                else if (work_time < accidentTime)
                {
                    inWellPerson.work_state = "嫌疑";
                }
                else
                {
                    inWellPerson.work_state = "事故";
                }
                query.Add(inWellPerson);
            }

            return query;
        }

        [Update]
        public void IrisUpdateInWellPersonForDepart(UserInWellPerson inWellPerson)
        {
        }
        /// <summary>
        /// 获取当前井下人员
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UserInWellPerson> IrisGetInWellPersonForDepart(List<int> lstDepartID)
        {
            List<UserInWellPerson> query = new List<UserInWellPerson>();
            int overTime = 0;
            int alertTime = 0;
            int accidentTime = 0;
            string strSQLSysParam = " select over_time,alert_time,accident_time from system_param";
            DataTable dtSysParam = DbAccess.POSTGRESQL.Select(strSQLSysParam, "SystemParam");
            if (null != dtSysParam && dtSysParam.Rows.Count > 0)
            {
                if (DBNull.Value != dtSysParam.Rows[0]["over_time"])
                {
                    overTime = int.Parse(dtSysParam.Rows[0]["over_time"].ToString());
                }
                if (DBNull.Value != dtSysParam.Rows[0]["alert_time"])
                {
                    alertTime = int.Parse(dtSysParam.Rows[0]["alert_time"].ToString());
                }
                if (DBNull.Value != dtSysParam.Rows[0]["accident_time"])
                {
                    accidentTime = int.Parse(dtSysParam.Rows[0]["accident_time"].ToString());
                }
            }

            string strSQL = "select in_out_id,work_sn,person_id, name,depart_name,class_order_name,dev_group,in_time,in_recog_id from in_out_well where 1=1 ";

            if (lstDepartID != null && lstDepartID.Count > 0)
            {
                strSQL += " and depart_id in ( ";
                for (int i = 0; i < lstDepartID.Count - 1; i++)
                {
                    strSQL += lstDepartID[i] + ",";
                }

                strSQL += lstDepartID[lstDepartID.Count - 1] + ")";
            }

            strSQL += " and out_time is null and in_time is not null ";

            DataTable dt = DbAccess.POSTGRESQL.Select(strSQL, "UserInWellPerson");
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }
            foreach (DataRow ar in dt.Rows)
            {
                UserInWellPerson inWellPerson = new UserInWellPerson();

                if (DBNull.Value != ar["in_out_id"])
                {
                    inWellPerson.in_out_id = int.Parse(ar["in_out_id"].ToString());
                }

                if (DBNull.Value != ar["person_id"])
                {
                    inWellPerson.person_id = int.Parse(ar["person_id"].ToString());
                }

                inWellPerson.class_order_name = ar["class_order_name"].ToString();
                inWellPerson.depart_name = ar["depart_name"].ToString();

                if (DBNull.Value != ar["dev_group"])
                {
                    switch (int.Parse(ar["dev_group"].ToString()))
                    {
                        case 3:
                            inWellPerson.dev_group_int = 3;
                            inWellPerson.dev_group = "出入井";
                            break;
                        case 6:
                            inWellPerson.dev_group_int = 6;
                            inWellPerson.dev_group = "上下班";
                            break;
                        default:
                            break;
                    }
                }
                if (DBNull.Value != ar["in_time"])
                {
                    inWellPerson.in_time = DateTime.Parse(ar["in_time"].ToString());
                }
                inWellPerson.person_name = ar["name"].ToString();
                inWellPerson.work_sn = ar["work_sn"].ToString();

                //double d = 0.9f;
                //string str = d.ToString("f0");
                //string str4 = Math.Truncate(d).ToString();
                //double time1 = (DateTime.Now - DateTime.Parse("2013-09-09 16:53")).TotalHours;
                //string str2 = time1.ToString("f0");
                //string str3 = time1.ToString("f1");


                //化成分钟数，如果分钟数为一位，则在其前补零
                if (Math.Abs((DateTime.Now - inWellPerson.in_time).Minutes) > 9)
                {
                    inWellPerson.work_time = Math.Truncate((DateTime.Now - inWellPerson.in_time).TotalHours).ToString("f0") + ":"
                        + Math.Abs((DateTime.Now - inWellPerson.in_time).Minutes).ToString();
                }
                else
                {
                    inWellPerson.work_time = Math.Truncate((DateTime.Now - inWellPerson.in_time).TotalHours).ToString("f0") + ":0"
                        + Math.Abs((DateTime.Now - inWellPerson.in_time).Minutes).ToString();
                }
                int work_time = (int)(DateTime.Now - inWellPerson.in_time).TotalMinutes;
                if (work_time < overTime)
                {
                    inWellPerson.work_state = "正常";
                }
                else if (work_time < alertTime)
                {
                    inWellPerson.work_state = "超时";
                }
                else if (work_time < accidentTime)
                {
                    inWellPerson.work_state = "嫌疑";
                }
                else
                {
                    inWellPerson.work_state = "事故";
                }

                if (ar["in_recog_id"].ToString() != "")
                {
                    inWellPerson.in_recog_id = int.Parse(ar["in_recog_id"].ToString());
                }

                query.Add(inWellPerson);
            }

            return query;
        }

        /// <summary>
        /// 获取请假信息
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UserAttendForLeave> IrisGetAttendForLeave()
        {
            List<UserAttendForLeave> query = new List<UserAttendForLeave>();

            string strSQL = "SELECT attend_for_leave_id, ar.person_id,name,work_sn,depart_name, leave_start_time, leave_end_time, ";
            strSQL += " is_leave_all_day, ar.leave_type_id,leave_type_name, ar.memo, operate_time, operator_name, ";
            strSQL += " actual_leave_days, modify_time  FROM attend_for_leave ar";
            strSQL += " LEFT JOIN person p ON ar.person_id = p.person_id";
            strSQL += " LEFT JOIN leave_type inlt ON ar.leave_type_id = inlt.leave_type_id   order by person_id";

            DataTable dt = DbAccess.POSTGRESQL.Select(strSQL, "UserInWellPerson");
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }
            foreach (DataRow ar in dt.Rows)
            {
                UserAttendForLeave attendLeave = new UserAttendForLeave();

                attendLeave.attend_for_leave_id = int.Parse(ar["attend_for_leave_id"].ToString());
                if (DBNull.Value != ar["person_id"])
                {
                    attendLeave.person_id = int.Parse(ar["person_id"].ToString());
                }
                if (DBNull.Value != ar["leave_start_time"])
                {
                    attendLeave.leave_start_time = DateTime.Parse(ar["leave_start_time"].ToString());
                }
                if (DBNull.Value != ar["leave_end_time"])
                {
                    attendLeave.leave_end_time = DateTime.Parse(ar["leave_end_time"].ToString());
                }

                if (DBNull.Value != ar["is_leave_all_day"])
                {
                    attendLeave.is_leave_all_day = (int.Parse(ar["is_leave_all_day"].ToString())) == 1 ? true : false;
                }
                if (DBNull.Value != ar["leave_type_id"])
                {
                    attendLeave.leave_type_id = int.Parse(ar["leave_type_id"].ToString());
                }

                attendLeave.memo = ar["memo"].ToString();
                if (DBNull.Value != ar["actual_leave_days"])
                {
                    attendLeave.actual_leave_days = Double.Parse(ar["actual_leave_days"].ToString());
                }

                attendLeave.work_sn = ar["work_sn"].ToString();
                attendLeave.depart_name = ar["depart_name"].ToString();
                attendLeave.person_name = ar["name"].ToString();
                attendLeave.leave_type_name = ar["leave_type_name"].ToString();
                query.Add(attendLeave);
            }

            return query;
        }

        /// <summary>
        /// 通过部门获取请假信息
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UserAttendForLeave> IrisGetAttendLeaveForDepart(List<int> lstDepartID)
        {
            List<UserAttendForLeave> query = new List<UserAttendForLeave>();

            string strSQL = "SELECT attend_for_leave_id, ar.person_id,name,work_sn, p.depart_id, depart_name, leave_start_time, leave_end_time, ";
            strSQL += " is_leave_all_day, ar.leave_type_id,leave_type_name, ar.memo, operate_time, operator_name, ";
            strSQL += " actual_leave_days, modify_time  FROM attend_for_leave ar";
            strSQL += " LEFT JOIN person p ON ar.person_id = p.person_id";
            strSQL += " LEFT JOIN leave_type inlt ON ar.leave_type_id = inlt.leave_type_id where 1=1";


            if (null != lstDepartID && lstDepartID.Count > 0)
            {
                strSQL += " and p.depart_id in ( ";
                for (int i = 0; i < lstDepartID.Count - 1; i++)
                {
                    strSQL += lstDepartID[i] + ",";
                }
                strSQL += lstDepartID[lstDepartID.Count - 1] + ")";
            }

            strSQL += " order by convert_to(depart_name,  E'GBK'), convert_to(name,  E'GBK'), leave_start_time ";
            DataTable dt = DbAccess.POSTGRESQL.Select(strSQL, "UserInWellPerson");
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }
            foreach (DataRow ar in dt.Rows)
            {
                UserAttendForLeave attendLeave = new UserAttendForLeave();

                attendLeave.attend_for_leave_id = int.Parse(ar["attend_for_leave_id"].ToString());
                if (DBNull.Value != ar["person_id"])
                {
                    attendLeave.person_id = int.Parse(ar["person_id"].ToString());
                }
                if (DBNull.Value != ar["leave_start_time"])
                {
                    attendLeave.leave_start_time = DateTime.Parse(ar["leave_start_time"].ToString());
                }
                if (DBNull.Value != ar["leave_end_time"])
                {
                    attendLeave.leave_end_time = DateTime.Parse(ar["leave_end_time"].ToString());
                }

                if (DBNull.Value != ar["is_leave_all_day"])
                {
                    attendLeave.is_leave_all_day = (int.Parse(ar["is_leave_all_day"].ToString())) == 1 ? true : false;
                }
                if (DBNull.Value != ar["leave_type_id"])
                {
                    attendLeave.leave_type_id = int.Parse(ar["leave_type_id"].ToString());
                }

                attendLeave.memo = ar["memo"].ToString();
                if (DBNull.Value != ar["actual_leave_days"])
                {
                    attendLeave.actual_leave_days = Double.Parse(ar["actual_leave_days"].ToString());
                }

                attendLeave.work_sn = ar["work_sn"].ToString();
                attendLeave.depart_id = int.Parse(ar["depart_id"].ToString());
                attendLeave.depart_name = ar["depart_name"].ToString();
                attendLeave.person_name = ar["name"].ToString();
                attendLeave.leave_type_name = ar["leave_type_name"].ToString();
                query.Add(attendLeave);
            }            
            return query;
        }

        /// <summary>
        /// 获取所有请假信息基类
        /// </summary>
        /// <returns></returns>
        public IEnumerable<attend_for_leave> IrisGetAttendForLeaveBase()
        {
            List<attend_for_leave> query = new List<attend_for_leave>();

            string strSQL = @"SELECT attend_for_leave_id, person_id, leave_start_time, leave_end_time, ";
            strSQL += " is_leave_all_day, leave_type_id, memo, operate_time, operator_name, ";
            strSQL += " actual_leave_days, modify_time  FROM attend_for_leave order by person_id";

            DataTable dt = DbAccess.POSTGRESQL.Select(strSQL, "attend_for_leave");
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }
            foreach (DataRow ar in dt.Rows)
            {
                attend_for_leave inWellPerson = new attend_for_leave();

                inWellPerson.attend_for_leave_id = int.Parse(ar["attend_for_leave_id"].ToString());
                if (DBNull.Value != ar["person_id"])
                {
                    inWellPerson.person_id = int.Parse(ar["person_id"].ToString());
                }
                if (DBNull.Value != ar["leave_start_time"])
                {
                    inWellPerson.leave_start_time = DateTime.Parse(ar["leave_start_time"].ToString());
                }
                if (DBNull.Value != ar["leave_end_time"])
                {
                    inWellPerson.leave_end_time = DateTime.Parse(ar["leave_end_time"].ToString());
                }

                if (DBNull.Value != ar["is_leave_all_day"])
                {
                    inWellPerson.is_leave_all_day = int.Parse(ar["is_leave_all_day"].ToString());
                }
                if (DBNull.Value != ar["leave_type_id"])
                {
                    inWellPerson.leave_type_id = int.Parse(ar["leave_type_id"].ToString());
                }
                inWellPerson.memo = ar["memo"].ToString();
                if (DBNull.Value != ar["actual_leave_days"])
                {
                    inWellPerson.actual_leave_days = Double.Parse(ar["actual_leave_days"].ToString());
                }
                //inWellPerson.
                query.Add(inWellPerson);
            }

            return query;
        }


        #endregion

        #region  添加函数
        /// <summary>
        /// 采用自定义函数添加和修改请假
        /// </summary>
        /// <param name="attendForLeave"></param>
        /// <returns></returns>
        [Invoke]
        public bool IrisInsetOrUpdateUserAttendForLeave(UserAttendForLeave attendForLeave)
        {
            if (attendForLeave.attend_for_leave_id > 0)
            {
                string updateSQL = @"UPDATE attend_for_leave SET ";
                updateSQL += "person_id=" + attendForLeave.person_id;
                updateSQL += ", leave_start_time=\'" + attendForLeave.leave_start_time + "\'";
                updateSQL += ", leave_end_time=\'" + attendForLeave.leave_end_time + "\'";
                updateSQL += ", is_leave_all_day= " + (attendForLeave.is_leave_all_day ? 1 : 0);
                updateSQL += ", leave_type_id=" + attendForLeave.leave_type_id;
                updateSQL += ", memo=\'" + attendForLeave.memo + "\'";
                updateSQL += ", operate_time= \'" + attendForLeave.operate_time + "\'";
                updateSQL += ", operator_name=\'" + attendForLeave.operator_name + "\'";
                updateSQL += ", actual_leave_days= " + attendForLeave.actual_leave_days;
                updateSQL += ", modify_time=\'" + attendForLeave.modify_time + "\'";
                updateSQL += " WHERE attend_for_leave_id=" + attendForLeave.attend_for_leave_id;
                return DbAccess.POSTGRESQL.Update(updateSQL);
            }
            else
            {
                string insertSQL = @" INSERT INTO attend_for_leave(";
                insertSQL += "  person_id, leave_start_time, leave_end_time, ";
                insertSQL += "  is_leave_all_day, leave_type_id, memo, operate_time, operator_name, ";
                insertSQL += "  actual_leave_days, modify_time) VALUES (";
                insertSQL += attendForLeave.person_id + ",\'" + attendForLeave.leave_start_time + "\',\'" + attendForLeave.leave_end_time + "\',";
                insertSQL += (attendForLeave.is_leave_all_day ? 1 : 0) + "," + attendForLeave.leave_type_id + ",\'" + attendForLeave.memo + "\',\'"
                    + attendForLeave.operate_time + "\',\'";
                insertSQL += attendForLeave.operator_name + "\'," + attendForLeave.actual_leave_days + ",\'" + attendForLeave.modify_time + "\'";
                insertSQL += ")";

                return DbAccess.POSTGRESQL.Insert(insertSQL);
            }
        }

        /// <summary>
        /// 批量添加请假 by cty 2014-3-31
        /// </summary>
        /// <param name="attendForLeave"></param>
        /// <returns></returns>
        [Invoke]
        public bool IrisContinueInsertUserAttendForLeave(List<UserAttendForLeave> attendForLeave)
        {
            bool isTrue=false;
            foreach (UserAttendForLeave ar in attendForLeave)
            {
                string insertSQL = @" INSERT INTO attend_for_leave(";
                insertSQL += "  person_id, leave_start_time, leave_end_time, ";
                insertSQL += "  is_leave_all_day, leave_type_id, memo, operate_time, operator_name, ";
                insertSQL += "  actual_leave_days, modify_time) VALUES (";
                insertSQL += ar.person_id + ",\'" + ar.leave_start_time + "\',\'" + ar.leave_end_time + "\',";
                insertSQL += (ar.is_leave_all_day ? 1 : 0) + "," + ar.leave_type_id + ",\'" + ar.memo + "\',\'"
                    + ar.operate_time + "\',\'";
                insertSQL += ar.operator_name + "\'," + ar.actual_leave_days + ",\'" + ar.modify_time + "\'";
                insertSQL += ")";


                isTrue= DbAccess.POSTGRESQL.Insert(insertSQL);
            }
            return isTrue;
            
        }
        /// <summary>
        /// 添加考勤记录  包括添加请假等信息--submitChanged
        /// </summary>
        /// <param name="attendRec"></param>
        public void InsertUserAttendForLeave(UserAttendForLeave attendForLeave)
        {
            if (attendForLeave.attend_for_leave_id > 0)
            {
                string updateSQL = @"UPDATE attend_for_leave SET ";
                updateSQL += "person_id=" + attendForLeave.person_id;
                updateSQL += ", leave_start_time=\'" + attendForLeave.leave_start_time + "\'";
                updateSQL += ", leave_end_time=\'" + attendForLeave.leave_end_time + "\'";
                updateSQL += ", is_leave_all_day= " + (attendForLeave.is_leave_all_day ? 1 : 0);
                updateSQL += ", leave_type_id=" + attendForLeave.leave_type_id;
                updateSQL += ", memo=\'" + attendForLeave.memo + "\'";
                updateSQL += ", operate_time= \'" + attendForLeave.operate_time + "\'";
                updateSQL += ", operator_name=\'" + attendForLeave.operator_name + "\'";
                updateSQL += ", actual_leave_days= " + attendForLeave.actual_leave_days;
                updateSQL += ", modify_time=\'" + attendForLeave.modify_time + "\'";
                updateSQL += " WHERE attend_for_leave_id=" + attendForLeave.attend_for_leave_id;
                DbAccess.POSTGRESQL.Update(updateSQL);
            }
            else
            {
                string insertSQL = @" INSERT INTO attend_for_leave(";
                insertSQL += " person_id, leave_start_time, leave_end_time, ";
                insertSQL += " is_leave_all_day, leave_type_id, memo, operate_time, operator_name, ";
                insertSQL += " actual_leave_days, modify_time, create_time) VALUES (";
                insertSQL += attendForLeave.person_id + ",\'" + attendForLeave.leave_start_time + "\',\'" + attendForLeave.leave_end_time + "\',";
                insertSQL += (attendForLeave.is_leave_all_day ? 1 : 0) + "," + attendForLeave.leave_type_id + ",\'" + attendForLeave.memo + "\',\'"
                           + attendForLeave.operate_time + "\',\'";
                insertSQL += attendForLeave.operator_name + "\'," + attendForLeave.actual_leave_days + ",\'" + attendForLeave.modify_time + "\',";
                insertSQL += "\'" + DateTime.Now + "\' )";

                DbAccess.POSTGRESQL.Insert(insertSQL);
            }
        }

        /// <summary>
        /// 添加考勤记录  包括添加请假等信息--submitChanged
        /// </summary>
        /// <param name="attendRec"></param>
        public void InsertAttendRecord(attend_record_base attendRec)
        {
            string insertSQL = @" INSERT INTO attend_record_base(";
            insertSQL += " person_id,in_well_time, out_well_time, ";
            insertSQL += " work_time, leave_type_id, ";
            insertSQL += " class_order_id, attend_times, attend_day, memo, attend_type, ";
            insertSQL += " dev_group, work_cnt )VALUES (";
            insertSQL += attendRec.person_id + ",\'" + attendRec.in_well_time + "\',\'"
                       + attendRec.out_well_time + "\',";
            insertSQL += attendRec.work_time + "," + attendRec.leave_type_id + ","
                       + attendRec.class_order_id + "," + attendRec.attend_times + ",\'";
            insertSQL += attendRec.attend_day + "\',\'" + attendRec.memo + "\'," + attendRec.attend_type + ",";
            insertSQL += attendRec.dev_group + "," + attendRec.work_cnt + " )";

            DbAccess.POSTGRESQL.Insert(insertSQL);
        }

        /// <summary>
        /// 添加识别记录
        /// </summary>
        /// <param name="recog"></param>
        [Invoke]
        public int IrisInsertUserPersonRecogLog(UserPersonRecogLog recog)
        {
            try
            {
                IdentifyData data = new IdentifyData();
                data.class_type_id = 0;
                data.class_type_name = "";
                data.depart_id = 0;
                data.depart_name = "";
                data.id = 0;
                data.peson_name = "";
                data.work_sn = "";
                data.recog_type = 0;
                data.person_id = recog.person_id;
                data.recog_time = recog.recog_time.ToString();
                data.at_position = recog.at_position;
                data.dev_sn = recog.device_sn ?? "";
                data.dev_type = recog.dev_type_value;
                data.operator_name = recog.operator_name;

                IdentifyData[] datas = new IdentifyData[1];
                datas[0] = data;
                //data.memo = recog.memo;
                return ServerComLib.IdentifyRecAdd(strIP, Port, datas);
            }
            catch (Exception e)
            {
                string str = e.Message;
                return -10;
            }
        }

        /// <summary>
        /// 为多个人员批量添加统一时间的识别记录
        /// by wz 2013.11.1
        /// 0 成功 非0失败
        /// </summary>
        /// <param name="personIds"></param>
        /// <param name="recogTime"></param>
        /// <param name="position"></param>
        /// <param name="deviceSn"></param>
        /// <param name="devType"></param>
        /// <param name="operatorName"></param>
        /// <returns></returns>
        [Invoke]
        public int IrisBatchInsertRecogLog(int[] personIds, UserPersonRecogLog recog)
        {
            //DateTime startTime = DateTime.Now;
            //TimeSpan spendTime;
            try
            {
                
                List<IdentifyData> identifyDatas = new List<IdentifyData>(); 
                foreach (var personId in personIds)
                {
                    IdentifyData data = new IdentifyData();
                    data.class_type_id = 0;
                    data.class_type_name = "";
                    data.depart_id = 0;
                    data.depart_name = "";
                    data.id = 0;
                    data.peson_name = "";
                    data.work_sn = "";
                    data.recog_type = 0;
                    data.person_id = personId;
                    data.recog_time = recog.recog_time.ToString();
                    data.at_position = recog.at_position;
                    data.dev_sn = recog.device_sn;
                    data.dev_type = recog.dev_type_value;
                    data.operator_name = recog.operator_name;
                    identifyDatas.Add(data);
                }

                int res = ServerComLib.IdentifyRecAdd(strIP, Port, identifyDatas.ToArray());

                return res;
            }
            catch (Exception e)
            {
                string str = e.Message;
               // spendTime = DateTime.Now - startTime;
                return -10;
            }

           
        }


        /// <summary>
        /// 批量添加井下超时人员识别记录 0成功！ 1失败
        /// </summary>
        /// <param name="recog"></param>
        [Invoke]
        public int IrisInsertUserPersonRecogLogForOverInWell(int[] personID,DateTime[] dt,int[] devType, string remarks)
        {
            if (personID.Count()<1 || personID.Count() != dt.Count() || personID.Count()!=devType.Count())
            {
                return 0;
            }

            try
            {

                int overTime = 30;
                string strSQLSysParam = " select over_time,alert_time,accident_time from system_param";
                DataTable dtSysParam = DbAccess.POSTGRESQL.Select(strSQLSysParam, "SystemParam");
                if (null != dtSysParam && dtSysParam.Rows.Count > 0)
                {
                    if (DBNull.Value != dtSysParam.Rows[0]["over_time"])
                    {
                        overTime = int.Parse(dtSysParam.Rows[0]["over_time"].ToString());
                    }
                }

                List<IdentifyData> identifyDatas = new List<IdentifyData>(); 

                for (int index = 0; index < personID.Count();index++ )
                {
                    IdentifyData data = new IdentifyData();
                    data.class_type_id = 0;
                    data.class_type_name = "";
                    data.depart_id = 0;
                    data.depart_name = "";
                    data.id = 0;
                    data.peson_name = "";
                    data.work_sn = "";
                    data.recog_type = 0;//0手动补加
                    data.person_id = personID[index];
                    data.recog_time = dt[index].AddMinutes(overTime).ToString();
                    data.at_position = "";
                    data.dev_sn =  "";
                    data.operator_name = remarks;
                    //1入井2出井3出入井4上班5下班6上下班
                    if (devType[index] > 6 || devType[index] < 1)
                    {
                        data.dev_type = 2;//出井
                    }
                    else if (devType[index] != 3 && devType[index] != 6)
                    {
                        data.dev_type = devType[index] + 1;  
                    }
                    else
                    {
                        data.dev_type = devType[index];
                    }
                    
                    identifyDatas.Add( data );
                    
                    //ServerComLib.IdentifyRecAdd(strIP, Port, ref data);
                }

                int tag = ServerComLib.IdentifyRecAdd(strIP, Port, identifyDatas.ToArray());
                if (tag != 0)
                {
                    string str = IrisGetError(tag);
                    return 1;
                }

                //List<UserPersonRecogLog> query = new List<UserPersonRecogLog>();

                //string strSQL = "select name,work_sn, depart_name,person_recog_log_id,person_id,class_type_id,class_type_name,recog_time,"
                //              + "recog_type,at_position,device_sn,dev_type,memo,attend_state from person_recog_log  where 1=1";

                //strSQL += " and person_recog_log_id in(" + recogIds[0];
                //for (int icount = 1; icount < recogIds.Count(); icount++)
                //{
                //    strSQL += " ," + recogIds[icount];
                //}
                //strSQL += ") ";
                //strSQL += " and (delete_time is null or attend_state != 0)";

                //DataTable dt = DbAccess.POSTGRESQL.Select(strSQL, "UserPersonRecogLog");
                //if (null == dt || dt.Rows.Count < 1)
                //{
                //    return 0;
                //}
                //foreach (DataRow ar in dt.Rows)
                //{
                //    IdentifyData data = new IdentifyData();
                //    data.class_type_id = int.Parse(ar["class_type_id"].ToString());
                //    data.class_type_name = ar["class_type_name"].ToString();
                //    data.depart_id = 0;
                //    data.depart_name = "";
                //    data.id = 0;
                //    data.peson_name = ar["name"].ToString();
                //    data.work_sn = ar["work_sn"].ToString();
                //    data.recog_type = 0;
                //    data.person_id = int.Parse(ar["person_id"].ToString());
                //    data.recog_time = (DateTime.Parse(ar["recog_time"].ToString()).AddHours(9)).ToString();
                //    data.at_position = "";
                //    data.dev_sn =  "";
                //    data.dev_type = int.Parse(ar["dev_type"].ToString()) != 3 
                //        && int.Parse(ar["dev_type"].ToString()) != 6 
                //        ? int.Parse(ar["dev_type"].ToString()) + 1 
                //        : int.Parse(ar["dev_type"].ToString());

                //    if (ServerComLib.IdentifyRecAdd(strIP, Port, ref data) != 0)
                //        return 1;
                //}
            }
            catch (Exception e)
            {
                string str = e.Message;
                return -10;
            }
            return 0;
        }

        /// <summary>
        /// 添加识别记录--submitChanged
        /// </summary>
        /// <param name="recog"></param>
        public void InsertUserPersonRecogLog(UserPersonRecogLog recog)
        {
            try
            {
                IdentifyData data = new IdentifyData();

                data.person_id = recog.person_id;
                data.recog_time = recog.recog_time.ToString();
                data.at_position = recog.at_position;
                data.dev_sn = recog.device_sn;
                data.dev_type = recog.dev_type_value;
                // data.memo = recog.memo;

                IdentifyData[] datas = new IdentifyData[1];
                datas[0] = data;

                int abc = ServerComLib.IdentifyRecAdd(strIP, Port, datas);
            }
            catch (Exception e)
            {
                string str = e.Message;
            }
        }

        /// <summary>
        /// cty
        /// 添加考勤类型
        /// </summary>
        /// <param name="attendRec"></param>
        [Invoke]
        public string InsertOrUpdateLeaveType(LeaveType leaveType)
        {
            string attend_sign = leaveType.attend_sign == null ? "null" : ("'" + leaveType.attend_sign + "'");
            string memo = leaveType.memo == null ? "null" : ("'" + leaveType.memo + "'");

            //添加考勤类型
            if (leaveType.leave_type_id == -1)
            {
                //string selectSql = @" Select * from leave_type where leave_type_name =" + " \'" + leaveType.leave_type_name + "\'";
                // modify by gqy 请假类型在50以上的不重复
                string selectSql = string.Format(@" Select * from leave_type where leave_type_name = '{0}' and leave_type_id > 50", leaveType.leave_type_name);
                DataTable dt = DbAccess.POSTGRESQL.Select(selectSql, "");
                if (dt != null && dt.Rows.Count > 0 && dt.Rows[0].ItemArray.Length > 0)
                {
                    return "LeaveTypeHasExist";
                }

                string insertSQL = @" INSERT INTO leave_type(";
                insertSQL += " leave_type_name,attend_sign, leave_type_class,is_normal_attend,delete_info,is_schedule,create_time,update_time,memo ,system_defined,priority )VALUES (\'";
                insertSQL += leaveType.leave_type_name + "\',";
                insertSQL += attend_sign + ",";
                insertSQL += "0" + ",";
                insertSQL += leaveType.is_normal_attend + ",";
                insertSQL += "null" + ",";
                insertSQL += leaveType.is_schedule + ",\'";
                insertSQL += DateTime.Now.ToString() + "\',\'";
                insertSQL += DateTime.Now.ToString() + "\',";
                insertSQL += memo + ",";
                insertSQL += "null" + ",";
                insertSQL += "0" + ")";
                string insql = insertSQL;
                if (DbAccess.POSTGRESQL.Insert(insertSQL))
                {
                    return "InsertSuccess";
                }
                else
                {
                    return "InsertError";
                }
            }
            else
            {
                string updateSQL = @"UPDATE leave_type SET ";
                updateSQL += "leave_type_name=\'" + leaveType.leave_type_name+"\'";
                updateSQL += ", attend_sign=" + attend_sign;
                updateSQL += ", is_normal_attend=" + leaveType.is_normal_attend;
                updateSQL += ", is_schedule= " + leaveType.is_schedule;
                updateSQL += ", memo=" + memo;
                updateSQL += " WHERE leave_type_id=" + leaveType.leave_type_id;
                if (DbAccess.POSTGRESQL.Update(updateSQL))
                {
                    return "UpdateSuccess";
                }
                else
                {
                    return "UpdateError";
                }
            }
        }

        #endregion

        #region 更新数据库
        //       UPDATE attend_for_leave
        //  SET attend_for_leave_id=?, person_id=?, leave_start_time=?, leave_end_time=?, 
        //      is_leave_all_day=?, leave_type_id=?, memo=?, operate_time=?, 
        //      operator_name=?, actual_leave_days=?, modify_time=?, create_time=?
        //WHERE <condition>;
        /// <summary>
        /// 更新考勤记录  包括添加请假等信息
        /// </summary>
        /// <param name="attendRec"></param>
        /// <summary>
        /// 添加考勤记录  包括添加请假等信息
        /// </summary>
        /// <param name="attendRec"></param>
        [Invoke]
        public bool IrisUpdateUserAttendForLeave(UserAttendForLeave attendForLeave)
        {
            string updateSQL = @"UPDATE attend_for_leave SET ";
            updateSQL += "person_id=" + attendForLeave.person_id;
            updateSQL += ", leave_start_time=\'" + attendForLeave.leave_start_time + "\'";
            updateSQL += ", leave_end_time=\'" + attendForLeave.leave_end_time + "\'";
            updateSQL += ", is_leave_all_day= " + (attendForLeave.is_leave_all_day ? 1 : 0);
            updateSQL += ", leave_type_id=" + attendForLeave.leave_type_id;
            updateSQL += ", memo=\'" + attendForLeave.memo + "\'";
            updateSQL += ", operate_time= \'" + attendForLeave.operate_time + "\'";
            updateSQL += ", operator_name=\'" + attendForLeave.operator_name + "\'";
            updateSQL += ", actual_leave_days= " + attendForLeave.actual_leave_days;
            updateSQL += ", modify_time=\'" + attendForLeave.modify_time + "\'";
            updateSQL += " WHERE attend_for_leave_id=" + attendForLeave.attend_for_leave_id;
            return DbAccess.POSTGRESQL.Update(updateSQL);
        }


        #endregion

        #region 删除数据

        /// <summary>
        /// 删除请假，自定义函数
        /// </summary>
        /// <param name="attend_for_leave_id"></param>
        /// <returns></returns>
        [Invoke]
        public bool IrisDeleteUserAttendForLeave(int attend_for_leave_id)
        {
            string delSQL = "delete from attend_for_leave where attend_for_leave_id = " + attend_for_leave_id;
            return DbAccess.POSTGRESQL.Delete(delSQL);
        }

        /// <summary>
        /// 删除请假--submitChanged
        /// </summary>
        /// <param name="attendForLeave"></param>
        public void DeleteUserAttendForLeave(UserAttendForLeave attendForLeave)
        {
            string delSQL = "delete from attend_for_leave where attend_for_leave_id = "
                + attendForLeave.attend_for_leave_id;
            DbAccess.POSTGRESQL.Delete(delSQL);
        }


        /// <summary>
        /// 删除识别记录
        /// </summary>
        /// <param name="recog"></param>
        [Invoke]
        public int IrisDeleteUserPersonRecogLog(UserPersonRecogLog recog)
        {
            int[] recog_ids = { recog.person_recog_id };
            return ServerComLib.IdentifyRecDel(strIP, Port, recog_ids, 1);
        }

        /// <summary>
        /// 删除多条识别记录
        /// </summary>
        /// <param name="recog"></param>
        /// <returns></returns>
        [Invoke]
        public int IrisDeleteUserPersonRecogLogsForIDS(int[] recogIds)
        {
            return ServerComLib.IdentifyRecDel(strIP, Port, recogIds, recogIds.Count());
        }

        /// <summary>
        /// cty
        /// 删除考勤类型
        /// </summary>
        /// <param name="leaveTypeIds"></param>
        /// <returns></returns>
        public bool IrisDeleteLeaveType(int[] leaveTypeIds)
        {
            string sql = @"DELETE FROM leave_type WHERE leave_type_id in ( ";

            sql += leaveTypeIds[0];
            for (int i = 1; i < leaveTypeIds.Length; i++)
            {
                sql += "," + leaveTypeIds[i];
            }

            sql += ")";

            return DbAccess.POSTGRESQL.Delete(sql);
 
        }

        #endregion

        #region 重构考勤记录

        [Invoke]
        public int IrisRebuildAttend(DateTime beginDate, int[] person_ids)
        {
            if (person_ids == null)
            {
                string strSQL = "select person_id from person_base ";

                DataTable dt = DbAccess.POSTGRESQL.Select(strSQL, "person");
                if (null == dt || dt.Rows.Count < 1)
                {
                    return -1;
                }
                int[] ids = new int[dt.Rows.Count];
                int i = 0;
                foreach (DataRow ar in dt.Rows)
                {
                    ids[i++] = int.Parse(ar["person_id"].ToString());
                }

                int result = ServerComLib.Rebuild(strIP, Port, ids, ids.Count(),
                    beginDate.ToString("yyyy-MM-dd HH:mm:ss"));
                string err = ServerComLib.GetLastError(ref result);
                return result;

            }
            else
            {
                //string str = beginDate.ToString("yyyy-MM-dd HH:mm:ss");
                int result1 = ServerComLib.Rebuild(strIP, Port, person_ids, person_ids.Count(),
                    beginDate.ToString("yyyy-MM-dd HH:mm:ss"));
                string err = ServerComLib.GetLastError(ref result1);
                return result1;
            }
        }

        /// <summary>
        /// offset 从0开始
        /// </summary>
        /// <param name="beginDate"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [Invoke]
        public int IrisRebuildAttendPart(DateTime beginDate, int offset, int count)
        {
            string strSQL = "select person_id from person_base LIMIT " + count + " OFFSET " + offset;

            DataTable dt = DbAccess.POSTGRESQL.Select(strSQL, "person");
            if (null == dt || dt.Rows.Count < 1)
            {
                return -1;
            }
            int[] ids = new int[dt.Rows.Count];

            {
                int index = 0;
                foreach (DataRow ar in dt.Rows)
                {
                    ids[index++] = int.Parse(ar["person_id"].ToString());
                }
            }

            int result = ServerComLib.Rebuild(strIP, Port, ids, ids.Count(), beginDate.ToString("yyyy-MM-dd HH:mm:ss"));
            return result;
        }

        #endregion

        #region 与后台交互

        /// <summary>
        /// 错误原因
        /// </summary>
        /// <param name="errorId"></param>
        /// <returns></returns>
        [Invoke]
        public string IrisGetError(int errorId)
        {
            string errStr = ServerComLib.GetLastError(ref errorId);
            return errStr;
        }

        #endregion

        #region 节假日管理 add by gqy
        /// <summary>
        /// 增加节假日
        /// </summary>
        /// <param name="festival">增加的节假日信息</param>
        /// <returns>0X01：添加成功且通知后台成功 0X00：添加失败 0XFF:添加成功,通知后台失败</returns>
        [Invoke]
        public byte AddFestival(FestivalInfo fesval)
        {
            string strSQL = string.Format(@" INSERT INTO festival(name, begin_time, end_time, memo)VALUES ('{0}','{1}','{2}','{3}')",
            fesval.name, fesval.begin_time, fesval.end_time, fesval.memo);

            if (!DbAccess.POSTGRESQL.Insert(strSQL))
            {
                return 0X00;
            }

            if (fesval.ShiftHolidayList == null || fesval.ShiftHolidayList.Count == 0)
            {
                if (ServerComLib.NoticeTableChange(strIP, Port, new string[] { "festival" }, 1) != 0)
                {
                    return 0XFF;
                }
                return 0X01;
            }

            strSQL = string.Format(@"select festival_id from festival where name = '{0}'", fesval.name);
            DataTable dt = DbAccess.POSTGRESQL.Select(strSQL, "festival");
            if (null == dt || dt.Rows.Count < 1)
            {
                return 0X00;
            }

            int fesvalID = Convert.ToInt32(dt.Rows[0]["festival_id"].ToString());
            strSQL = string.Format(@"Delete from weekend_to_work where festival_id = {0}", fesvalID);
            if (!DbAccess.POSTGRESQL.Delete(strSQL))
            {
                return 0X00;
            }

            for (int index = 0; index < fesval.ShiftHolidayList.Count(); index++)
            {
                strSQL = string.Format(@"Insert INTO weekend_to_work (festival_id, name, begin_time, end_time)VALUES ('{0}','{1}','{2}','{3}')",
                    fesvalID, fesval.name, fesval.ShiftHolidayList[index], fesval.ShiftHolidayList[index].AddDays(1));
                if (!DbAccess.POSTGRESQL.Insert(strSQL))
                {
                    return 0X00;
                }
            }
            if (ServerComLib.NoticeTableChange(strIP, Port, new string[] { "festival", "weekend_to_work" }, 1) != 0)
            {
                return 0XFF;
            }
            return 0X01;
            
        }

        /// <summary>
        /// 修改节假日
        /// </summary>
        /// <param name="festival">修改的节假日信息</param>
        /// <returns>0X01：修改成功且通知后台成功 0X00：修改失败 0XFF:修改成功,通知后台失败</returns>
        [Invoke]
        public byte ModifyFestival(FestivalInfo fesval)
        {
            string strSQL = string.Format(@" UPDATE festival SET name='{0}', begin_time='{1}', end_time='{2}', memo='{3}' where festival_id = {4}",
            fesval.name, fesval.begin_time, fesval.end_time, fesval.memo, fesval.festival_id);

            if (!DbAccess.POSTGRESQL.Insert(strSQL))
            {
                return 0X00;
            }

            strSQL = string.Format(@"Delete from weekend_to_work where festival_id = {0}", fesval.festival_id);
            if (!DbAccess.POSTGRESQL.Delete(strSQL))
            {
                return 0X00;
            }
           
            for (int index = 0; index < fesval.ShiftHolidayList.Count(); index++)
            {
                strSQL = string.Format(@"Insert INTO weekend_to_work (festival_id, name, begin_time, end_time)VALUES ('{0}','{1}','{2}','{3}')",
                    fesval.festival_id, fesval.name, fesval.ShiftHolidayList[index], fesval.ShiftHolidayList[index].AddDays(1));
                if (!DbAccess.POSTGRESQL.Insert(strSQL))
                {
                    return 0X00;
                }
            }
            if (ServerComLib.NoticeTableChange(strIP, Port, new string[] { "festival", "weekend_to_work" }, 1) != 0)
            {
                return 0XFF;
            }
            return 0X01;
        }

        /// <summary>
        /// 获取所有节假日
        /// </summary>
        /// <returns>获取的节假日列表</returns>
        public IEnumerable<FestivalInfo> GetAllFestival()
        {
            string querySql = @"select * from festival";
            return GetFestivalBySql(querySql); 
        }

        public IEnumerable<FestivalInfo> GetFestivalBySql( string querySql )
        {
            List<FestivalInfo> fesvalList = new List<FestivalInfo>();
            DataTable dt = DbAccess.POSTGRESQL.Select(querySql, "festival");
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }

            try
            {
                foreach (DataRow ar in dt.Rows)
                {
                    FestivalInfo fesvalInfo = new FestivalInfo();
                    fesvalInfo.festival_id = Convert.ToInt32(ar["festival_id"].ToString());
                    fesvalInfo.name = ar["name"].ToString();
                    fesvalInfo.begin_time = Convert.ToDateTime(ar["begin_time"]).ToString("yyyy-MM-dd");
                    fesvalInfo.end_time = Convert.ToDateTime(ar["end_time"]).AddDays(-1).ToString("yyyy-MM-dd");
                    fesvalInfo.memo = ar["memo"].ToString();
                    fesvalInfo.ShiftHolidayList = new List<DateTime>();
                    querySql = string.Format(@"select * from weekend_to_work where festival_id = {0}", fesvalInfo.festival_id);

                    DataTable workDt = DbAccess.POSTGRESQL.Select(querySql, "weekend_to_work");
                    if (null != workDt || workDt.Rows.Count >= 1)
                    {
                        foreach (DataRow item in workDt.Rows)
                        {
                            fesvalInfo.ShiftHolidayList.Add(Convert.ToDateTime(item["begin_time"]));
                            if (fesvalInfo.ShiftHoliday != null && fesvalInfo.ShiftHoliday != "")
                            {
                                fesvalInfo.ShiftHoliday += "\r\n";
                            }
                            fesvalInfo.ShiftHoliday += Convert.ToDateTime(item["begin_time"]).ToString("yyyy-MM-dd");
                        }
                    }
                    fesvalList.Add(fesvalInfo);
                }

                return fesvalList;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                return null;
            }
        }

        /// <summary>
        ///  不能删除，否则无法更新
        /// </summary>
        /// <param name="o"></param>
        [Update]
        public void UpdateFestivalEidtor(FestivalInfo o)
        {
        }

        /// <summary>
        /// 节假日批量删除
        /// </summary>
        /// <param name="festivalIds">删除的节假日列表ID集合</param>
        /// <returns>true：删除成功 false：删除失败</returns>
        [Invoke]
        public byte BatchDeleteFestival(int[] festivalIds)
        {
            string deleteFestivalSql = "DELETE FROM festival WHERE festival_id in ( ";
            string deleteWeekendSql = "DELETE FROM weekend_to_work WHERE festival_id in ( ";

            string whereSql = festivalIds[0].ToString();
            for (int index = 1; index < festivalIds.Length; index++)
            {
                whereSql += ("," + festivalIds[index]);
            }
            whereSql += ")";

            deleteFestivalSql += whereSql;
            deleteWeekendSql += whereSql;

            if (DbAccess.POSTGRESQL.Delete(deleteWeekendSql + ";" + deleteFestivalSql))
            {
                if (ServerComLib.NoticeTableChange(strIP, Port, new string[] { "festival" }, 1) != 0)
                {
                    return 0XFF;
                }
                return 0X01;
            }
            return 0X00;
        }

        /// <summary>
        /// 查询是否存在此节假日
        /// </summary>        
        /// <param name="devSn">查询的节假日的sn</param>
        /// <returns>true：存在 false：不存在</returns>
        [Invoke]
        public bool IsFestivalExist(FestivalInfo fesval)
        {
            string querySQL = string.Format(@"select * from festival where ");

            if (fesval.festival_id != 0)
            {
                querySQL += string.Format(@" festival_id != {0} and ", fesval.festival_id ); 
            }
            querySQL += string.Format(@"(name = '{0}' or (begin_time >= '{1}' and begin_time < '{2}') or (end_time > '{1}' and end_time <= '{2}') )",
                    fesval.name, fesval.begin_time, fesval.end_time);
            DataTable dt = DbAccess.POSTGRESQL.Select(querySQL, "festival");
            if (null != dt && dt.Rows.Count > 0)
            {
                return true;
            }

            if (fesval.ShiftHolidayList == null || fesval.ShiftHolidayList.Count <= 0)
            {
                return false;
            }
            querySQL = string.Format(@"select * from weekend_to_work where ");

            if (fesval.festival_id != 0)
            {
                querySQL += string.Format(@" festival_id != {0} and ", fesval.festival_id);
            }

            querySQL += " begin_time in (";

            foreach (DateTime item in fesval.ShiftHolidayList)
            {
                querySQL += "\'" + item.ToString( "yyyy-MM-dd 00:00:00") + "\',";
            }

            querySQL = querySQL.Remove(querySQL.Length - 1);

            querySQL += ")";

            dt = DbAccess.POSTGRESQL.Select(querySQL, "weekend_to_work");
            if (null != dt && dt.Rows.Count > 0)
            {
                return true;
            }
            return false;
        }
        #endregion


    }
}


