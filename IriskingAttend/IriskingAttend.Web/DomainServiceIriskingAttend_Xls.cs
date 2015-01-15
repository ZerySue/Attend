/*************************************************************************
** 文件名:   DomainServiceIriskingAttend_Xls.cs
** 主要类:   DomainServiceIriskingAttend_Xls
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   cty
** 日  期:   2013-5-13
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

    public partial class DomainServiceIriskingAttend
    {
        //用于主键的设置
        private static int iCount = 1;

        #region 原始记录汇总表

        /// <summary>
        /// 获取原始记录汇总表的数据
        /// </summary>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">截止时间</param>
        /// <param name="departIdLst">部门列表</param>
        /// <param name="ClassTypeId">班制</param>
        /// <param name="name">姓名</param>
        /// <param name="workSn">工号</param>
        /// <returns></returns>
        public IEnumerable<XlsUserAttendRec> MyGetXlsUserAttendRec(DateTime beginTime, DateTime endTime, int[] departIdLst, int ClassTypeId, string name, string workSn)
        {

            List<XlsUserAttendRec> query = new List<XlsUserAttendRec>();
            query.Clear();

            #region 组合sql语句

            string strSQL =string.Format( @"select attend_times ,work_time ,name, person_id,work_sn,depart_name,class_order_name,
                    attend_times,attend_day,class_type_name,class_type_id from attend_record_new where attend_day >= '{0}' 
                    and attend_day < '{1}'", beginTime, endTime );

            if (ClassTypeId != -1 && ClassTypeId != 0)
            {
                strSQL += " and class_type_id=" + ClassTypeId;
            }
          
            if (null != departIdLst)
            {
                strSQL += " and depart_id in ( ";
                for (int i = 0; i < departIdLst.Length - 1; i++)
                {
                    strSQL += departIdLst[i] + ",";
                }
                strSQL += departIdLst[departIdLst.Length - 1] + ")";
            }

            if ("" != name)
                strSQL += " and name like \'%" + name + "%\'";
            if ("" != workSn)
                strSQL += " and work_sn = \'" + workSn + "\'";

            strSQL += " order by depart_name,work_sn ";

            #endregion

            DataTable dt = DbAccess.POSTGRESQL.Select(strSQL, "attend_record_base");
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }
            
            foreach (DataRow ar in dt.Rows)
            {
                XlsUserAttendRec attend = new XlsUserAttendRec();

                //数据填充
                attend.attend_record_id = iCount++;
               
                if (DBNull.Value != ar["person_id"])
                {
                    attend.person_id = Convert.ToInt32(ar["person_id"]);
                }
                attend.person_name = ar["name"].ToString();
                if (DBNull.Value != ar["work_time"])
                {
                    attend.work_time = Convert.ToInt32(ar["work_time"]);
                }
                if (DBNull.Value != ar["attend_times"])
                {
                    attend.attend_times = Convert.ToInt32(ar["attend_times"]);
                }
                if (DBNull.Value != ar["attend_day"])
                {
                    attend.attend_day = Convert.ToDateTime(ar["attend_day"]);  
                }
                
                attend.work_sn = ar["work_sn"].ToString();
                attend.depart_name = ar["depart_name"].ToString();
                attend.class_order_name = ar["class_order_name"].ToString();
                attend.class_type_name = ar["class_type_name"].ToString();

                query.Add(attend);
            }

            return query;
        }

        #endregion

        #region 获取部门信息

        /// <summary>
        /// 获取所有部门
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UserDepartInfo> GetDepartInfo()
        {
            string strSQL = "select * from depart where delete_time is null order by convert_to(depart_name,  E'GBK')";
            List<UserDepartInfo> query = new List<UserDepartInfo>();
            DataTable dt = DbAccess.POSTGRESQL.Select(strSQL, "depart");
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }
            foreach (DataRow ar in dt.Rows)
            {
                UserDepartInfo department = new UserDepartInfo();
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

        #endregion

        #region 获得工种信息
        /// <summary>
        /// 获得工种信息
        /// </summary>
        /// <returns></returns>
        public IEnumerable<WorkTypeInfo> GetWorkTypeInfo()
        {
            List<WorkTypeInfo> query = new List<WorkTypeInfo>();
            query.Clear();

            string strSQL = string.Format("select work_type_id,work_type_name,memo from work_type order by work_type_name");
            DataTable dt = DbAccess.POSTGRESQL.Select(strSQL, "work_type_info");

            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }

            foreach (DataRow ar in dt.Rows)
            {
                WorkTypeInfo workTypeInfo = new WorkTypeInfo();
                workTypeInfo.work_type_id = Convert.ToInt32(ar["work_type_id"]);
                workTypeInfo.work_type_name = ar["work_type_name"].ToString();
                workTypeInfo.memo = ar["memo"].ToString();
                query.Add(workTypeInfo);
            }
            return query;
        }
        #endregion

        #region 获取职务信息
        /// <summary>
        /// 获得职务信息
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PrincipalInfo> GetPrincipalInfo()
        {
            List<PrincipalInfo> query = new List<PrincipalInfo>();
            query.Clear();

            string strSql = string.Format("select pri.principal_id,pri.principal_name,pri.principal_type_id,pt.principal_type_name,pri.principal_class,pri.memo from principal pri left join principal_type pt on pt.principal_type_id=pri.principal_type_id order by pri.principal_name");
            DataTable dt = DbAccess.POSTGRESQL.Select(strSql,"PrincipalInfo");

            if (dt == null || dt.Rows.Count < 1)
            {
                return null;
            }
            foreach (DataRow ar in dt.Rows)
            {
                PrincipalInfo principalInfo = new PrincipalInfo();
                principalInfo.principal_id = Convert.ToInt32(ar["principal_id"]);
                principalInfo.principal_name = ar["principal_name"].ToString();
                //principalInfo.principal_type_id = Convert.ToInt32(ar["principal_type_id"]);
                principalInfo.principal_type_name = ar["principal_type_name"].ToString();
                principalInfo.principal_class = ar["principal_class"].ToString();
                principalInfo.memo = ar["memo"].ToString();
                query.Add(principalInfo);
            }
            return query;
        }
        #endregion

        #region 五虎山虹膜考勤查询界面

        /// <summary>
        /// 五虎山单独查询界面获取数据
        /// </summary>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">截止时间</param>
        /// <param name="departIdLst">部门列表</param>
        /// <param name="name">人员姓名</param>
        /// <param name="workSn">人员工号</param>
        /// <param name="principalIdList">职务列表</param>
        /// <param name="workTypeIdList">工种列表</param>
        /// <param name="workTime">工作时长</param>
        /// <returns></returns>
        public IEnumerable<XlsAttendWuHuShanPersonList> GetIrisAttendQuery(DateTime beginTime, DateTime endTime, int[] departIdLst, string name, string workSn, int[] principalIdList, int[] workTypeIdList, int workTime)
        {
            List<XlsAttendWuHuShanPersonList> query = new List<XlsAttendWuHuShanPersonList>();
            query.Clear();

            string strSQL = string.Format(@"select p.name, p.work_sn, p.depart_id, d.depart_name, d.depart_sn, 
    arn.attend_record_id, arn.person_id, arn.in_well_time,
    arn.out_well_time, arn.work_time, arn.in_out_times,arn.class_order_id, arn.attend_times, (arn.out_well_time - arn.in_well_time) as iris_work_time, 
    arn.attend_day,arn.attend_type, arn.dev_group, arn.work_cnt, arn.in_leave_type_id, arn.out_leave_type_id, 
    arn.create_time, c.class_order_name,ct.class_type_name,pri.principal_name,wt.work_type_name  from attend_record_normal as arn
                      left join person as p on arn.person_id=p.person_id
                      left join class_order_normal as c on  c.class_order_id = arn.class_order_id
                      left join class_type as ct on c.class_type_id = ct.class_type_id
                      left join depart as d on p.depart_id = d.depart_id
                      left join principal as pri on p.principal_id = pri.principal_id
                      left join work_type as wt on wt.work_type_id = p.work_type_id

                      where arn.attend_day between '{0}' and '{1}'
                      and (arn.out_well_time - arn.in_well_time) is not null

                      ", beginTime, endTime);

            #region 选择条件是否选择了部门

            if (null != departIdLst && departIdLst.Length > 0)
            {

                strSQL += " and d.depart_id in ( ";
                for (int i = 0; i < departIdLst.Length - 1; i++)
                {
                    strSQL += departIdLst[i] + ",";
                }
                strSQL += departIdLst[departIdLst.Length - 1] + ")";
            }
            #endregion

            #region 选择条件是否选择了职务

            if (null != principalIdList)
            {
                strSQL += " and pri.principal_id in ( ";
                for (int i = 0; i < principalIdList.Length - 1; i++)
                {
                    strSQL += principalIdList[i] + ",";
                }
                strSQL += principalIdList[principalIdList.Length - 1] + ")";

            }

            #endregion

            #region 选择条件是否选择了工种

            if (null != workTypeIdList)
            {

                strSQL += " and wt.work_type_id in ( ";
                for (int i = 0; i < workTypeIdList.Length - 1; i++)
                {
                    strSQL += workTypeIdList[i] + ",";
                }
                strSQL += workTypeIdList[workTypeIdList.Length - 1] + ")";

            }

            #endregion

            if ("" != name)
                strSQL += " and p.name like \'%" + name + "%\'";
            if ("" != workSn)
                strSQL += " and p.work_sn = \'" + workSn + "\'";
            if (workTime != 0)
            {
                strSQL += " and (arn.out_well_time - arn.in_well_time) >= \'" + workTime + " min\'";
            }
            strSQL += " order by d.depart_name,p.name, p.work_sn";
            
            DataTable dt = DbAccess.POSTGRESQL.Select(strSQL, "attend_record_base");
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }

            foreach (DataRow ar in dt.Rows)
            {
                XlsAttendWuHuShanPersonList whsPersonList = new XlsAttendWuHuShanPersonList();

                whsPersonList.AttendRecordId = Convert.ToInt32(ar["attend_record_id"]);
                whsPersonList.PersonId = Convert.ToInt32(ar["person_id"]);
                whsPersonList.DepartName = ar["depart_name"].ToString();
                whsPersonList.PersonName = ar["name"].ToString();
                whsPersonList.WorkSn = ar["work_sn"].ToString();
                whsPersonList.WorkType = ar["work_type_name"].ToString();
                whsPersonList.Principal = ar["principal_name"].ToString();
                whsPersonList.ClassOrderName = ar["class_order_name"].ToString();
                whsPersonList.ClassTypeName = ar["class_type_name"].ToString();
                if (ar["iris_work_time"] != DBNull.Value)
                {
                    whsPersonList.IrisWorkTime = (TimeSpan)(ar["iris_work_time"]);
                }
                if (ar["attend_day"] != DBNull.Value)
                {
                    whsPersonList.AttendDay = Convert.ToDateTime(ar["attend_day"]);
                }
                if (ar["in_well_time"] != DBNull.Value)
                {
                    whsPersonList.InWellTime = Convert.ToDateTime(ar["in_well_time"]);
                }
                if (ar["out_well_time"] != DBNull.Value)
                {
                    whsPersonList.OutWellTime = Convert.ToDateTime(ar["out_well_time"]);
                }

                whsPersonList.AttendTimes = 1;

                query.Add(whsPersonList);
            }
            return query;
        }
        #endregion

        #region 五虎山单独查询界面

        /// <summary>
        /// 五虎山单独查询界面获取数据
        /// </summary>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">截止时间</param>
        /// <param name="departIdLst">部门列表</param>
        /// <param name="name">人员姓名</param>
        /// <param name="workSn">人员工号</param>
        /// <param name="principalIdList">职务列表</param>
        /// <param name="workTypeIdList">工种列表</param>
        /// <param name="workTime">工作时长</param>
        /// <returns></returns>
        public IEnumerable<XlsAttendWuHuShanPersonList> GetXlsWuHuShanPersonList(DateTime beginTime, DateTime endTime, int[] departIdLst, string name, string workSn, int[] principalIdList, int[] workTypeIdList, int workTime)
        {
            List<XlsAttendWuHuShanPersonList> query = new List<XlsAttendWuHuShanPersonList>();
            query.Clear();

           
            
            string strSQL = string.Format(@"select a.person_id, a.attend_record_id, a.whs_locate_id, a.attend_day, pb.work_sn, pb.name, d.depart_name,d.depart_id,
                       wt.work_type_name, p.principal_name,ct.class_type_name,c.class_order_name,
                       arn.in_well_time,arn.out_well_time,  (arn.out_well_time - arn.in_well_time) as iris_work_time, 
                       a.in_locate_time, a.out_locate_time, (a.out_locate_time - a.in_locate_time) as locate_work_time,
	                   (arn.in_well_time - a.in_locate_time), (arn.out_well_time - a.out_locate_time)
                      from whs_in_well_locate as a

                      left join attend_record_normal as arn on  -------2014.2.24修改，定位和虹膜考勤配对条件为:出入井时间相等且人员相同
                                                               (
                                                                arn.person_id = a.person_id
                                                                and ( a.in_well_time = arn.in_well_time or (a.in_well_time is null and arn.in_well_time is null ) )
                                                                and ( a.out_well_time = arn.out_well_time or (a.out_well_time is null and arn.out_well_time is null ) ))

                      left join class_order_normal as c on  c.class_order_id = arn.class_order_id
                      left join class_type as ct on c.class_type_id = ct.class_type_id
                      left join person_base as pb on pb.person_id = a.person_id
                      left join depart as d on pb.depart_id = d.depart_id
                      left join principal as p on pb.principal_id = p.principal_id
                      left join work_type as wt on wt.work_type_id = pb.work_type_id
  
                       
                      where a.attend_day between '{0}' and '{1}'
                      and (arn.out_well_time - arn.in_well_time) is not null
                      

                      ", beginTime, endTime.AddDays(1));

         
        
            #region 选择条件是否选择了部门
            
            if (null != departIdLst && departIdLst.Length>0)
            {
                
                strSQL += " and d.depart_id in ( ";
                for (int i = 0; i < departIdLst.Length - 1; i++)
                {
                    strSQL += departIdLst[i] + ",";
                }
                strSQL += departIdLst[departIdLst.Length - 1] + ")";
            }
            #endregion 

            #region 选择条件是否选择了职务
           
            if (null != principalIdList)
            {
                strSQL += " and p.principal_id in ( ";
                for (int i = 0; i < principalIdList.Length - 1; i++)
                {
                    strSQL += principalIdList[i] + ",";
                }
                strSQL += principalIdList[principalIdList.Length - 1] + ")";
              
            }
       
            #endregion

            #region 选择条件是否选择了工种
            
            if (null != workTypeIdList)
            {

                strSQL += " and wt.work_type_id in ( ";
                for (int i = 0; i < workTypeIdList.Length - 1; i++)
                {
                    strSQL += workTypeIdList[i] + ",";
                }
                strSQL += workTypeIdList[workTypeIdList.Length - 1] + ")";
                
            }
         
            #endregion

            if ("" != name)
                strSQL += " and pb.name like \'%" + name + "%\'";
            if ("" != workSn)
                strSQL += " and pb.work_sn = \'" + workSn + "\'";
            if (workTime != 0)
            {
                strSQL += " and (arn.out_well_time - arn.in_well_time) >= \'" + workTime + " min\'";
                strSQL += " and (a.out_locate_time - a.in_locate_time) >= \'" + workTime + " min\'";
            }
            strSQL += " order by convert_to(d.depart_name,  E'GBK'),convert_to(pb.name,  E'GBK')";
             
            DataTable dt = DbAccess.POSTGRESQL.Select(strSQL, "attend_record_base");

            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }

            foreach (DataRow ar in dt.Rows)
            {
                XlsAttendWuHuShanPersonList whsPersonList = new XlsAttendWuHuShanPersonList();

                whsPersonList.AttendRecordId = Convert.ToInt32(ar["whs_locate_id"]);
                whsPersonList.PersonId = Convert.ToInt32(ar["person_id"]);
                whsPersonList.DepartName=ar["depart_name"].ToString();
                whsPersonList.PersonName=ar["name"].ToString();
                whsPersonList.WorkSn=ar["work_sn"].ToString();
                whsPersonList.WorkType=ar["work_type_name"].ToString();
                whsPersonList.Principal = ar["principal_name"].ToString();
                whsPersonList.ClassOrderName = ar["class_order_name"].ToString();
                whsPersonList.ClassTypeName=ar["class_type_name"].ToString();
                if (ar["iris_work_time"] != DBNull.Value)
                {
                    whsPersonList.IrisWorkTime = (TimeSpan)(ar["iris_work_time"]);
                }
                if (ar["locate_work_time"] != DBNull.Value)
                {
                    whsPersonList.LocateWorkTime = (TimeSpan)(ar["locate_work_time"]);
                }
                if (ar["attend_day"] != DBNull.Value)
                {
                    whsPersonList.AttendDay = Convert.ToDateTime(ar["attend_day"]);
                }
                if (ar["in_well_time"] != DBNull.Value)
                {
                    whsPersonList.InWellTime = Convert.ToDateTime(ar["in_well_time"]);
                }
                if (ar["out_well_time"] != DBNull.Value)
                {
                    whsPersonList.OutWellTime = Convert.ToDateTime(ar["out_well_time"]);

                    if (whsPersonList.ClassOrderName.Contains("夜"))
                    {
                        if (whsPersonList.OutWellTime.Value.Day != whsPersonList.AttendDay.Day)
                        {
                            whsPersonList.AttendDay = whsPersonList.AttendDay.AddDays(-1);
                        }
                    }
                }
                if (ar["in_locate_time"] != DBNull.Value)
                {
                    whsPersonList.InLocateTime = Convert.ToDateTime(ar["in_locate_time"]);
                }
                if (ar["out_locate_time"] != DBNull.Value)
                {
                    whsPersonList.OutLocateTime = Convert.ToDateTime(ar["out_locate_time"]);
                }
                
                whsPersonList.AttendTimes = 1;

                if (whsPersonList.AttendDay >= beginTime && whsPersonList.AttendDay < endTime)
                {
                    query.Add(whsPersonList);
                }
            }
            return query;
        }
        #endregion

        #region 五虎山出入井记录表

        /// <summary>
        /// 五虎山人员出入井记录表获取数据
        /// </summary>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">截止时间</param>
        /// <param name="departIdLst">部门列表</param>
        /// <param name="name">人员姓名</param>
        /// <param name="workSn">人员工号</param>
        /// <param name="principalIdList">职务列表</param>
        /// <param name="workTypeIdList">工种列表</param>
        /// <param name="workTimeMore">工作时长大于</param>
        /// <param name="workTimeEqual">工作时长等于</param>
        /// <param name="workTimeLess">工作时长小于</param>
        /// <returns></returns>
        public IEnumerable<XlsAttendWuHuShanPersonList> GetXlsWuHuShanAttendanceDetailList(DateTime beginTime, DateTime endTime, int[] departIdLst, string name, string workSn, int[] principalIdList, int[] workTypeIdList, int workTimeMore,int workTimeEqual,int workTimeLess)
        {
            //判断是否需求进行当前在岗人员信息的查询
            //bool isSelectInOutWell = true;

            List<XlsAttendWuHuShanPersonList> query = new List<XlsAttendWuHuShanPersonList>();
            query.Clear();
            string strSQL = string.Format(@"select ar.*,wt.work_type_name,pri.principal_name,ct.class_type_name,p.delete_time
                                            from attend_record as ar
                                            Left join person p on p.person_id=ar.person_id 
                                            Left join work_type wt on p.work_type_id=wt.work_type_id
                                            Left join principal pri on p.principal_id=pri.principal_id
                                            LEFT JOIN class_type ct ON ct.class_type_id = p.class_type_id
                                            where ar.leave_type_id < 50 and p.delete_time is null and (in_well_time >= '");

            strSQL += beginTime + "\' or out_well_time >= '" + beginTime + "\' ) and attend_day < \'" + endTime + "\'";
            #region 选择条件是否选择了部门
            if (null != departIdLst)
            {
                bool isall = true;
                for (int i = 0; i < departIdLst.Length; i++)
                {
                    if (departIdLst[i] == 0)
                    {
                        isall = false;
                    }
                }
                if (isall)
                {
                    strSQL += " and ar.depart_id in ( ";
                    for (int i = 0; i < departIdLst.Length - 1; i++)
                    {
                        strSQL += departIdLst[i] + ",";
                    }
                    strSQL += departIdLst[departIdLst.Length - 1] + ")";
                }
            }
            #endregion

            #region 选择条件是否选择了职务
            if (null != principalIdList)
            {
                //bool isall = true;
                //for (int i = 0; i < principalIdList.Length; i++)
                //{
                //    if (principalIdList[i] == 0)
                //    {
                //        isall = false;
                //    }
                //}
                //if (isall)
                //{
                    strSQL += " and pri.principal_id in ( ";
                    for (int i = 0; i < principalIdList.Length - 1; i++)
                    {
                        strSQL += principalIdList[i] + ",";
                    }
                    strSQL += principalIdList[principalIdList.Length - 1] + ")";
                //}
            }
            #endregion

            #region 选择条件是否选择了工种
            if (null != workTypeIdList)
            {
                //bool isall = true;
                //for (int i = 0; i < workTypeIdList.Length; i++)
                //{
                //    if (workTypeIdList[i] == 0)
                //    {
                //        isall = false;
                //    }
                //}
                //if (isall)
                //{
                    strSQL += " and wt.work_type_id in ( ";
                    for (int i = 0; i < workTypeIdList.Length - 1; i++)
                    {
                        strSQL += workTypeIdList[i] + ",";
                    }
                    strSQL += workTypeIdList[workTypeIdList.Length - 1] + ")";
               // }
            }
            #endregion

            if ("" != name)
                strSQL += " and ar.name like \'%" + name + "%\'";
            if ("" != workSn)
                strSQL += " and ar.work_sn = \'" + workSn + "\'";
            if (workTimeMore != -1)
            {
                //isSelectInOutWell = false;
                strSQL += " and (ar.work_time > \'" + workTimeMore + "\'" + " or ar.work_time = \'" + workTimeEqual + "\'" + "or ar.work_time < \'" + workTimeLess + "\')";
            }
            else if (workTimeEqual != -1)
            {
                //isSelectInOutWell = false;
                strSQL += " and (ar.work_time = \'" + workTimeEqual + "\'" + "or ar.work_time < \'" + workTimeLess + "\')";
            }
            else if (workTimeLess != -1)
            {
                //isSelectInOutWell = false;
                strSQL += " and ar.work_time < \'" + workTimeLess + "\'";
            }
            strSQL += " order by ar.depart_name,ar.name,ar.in_well_time,ar.out_well_time";

            DataTable dt = DbAccess.POSTGRESQL.Select(strSQL, "attend_record_base");

            if (null != dt && dt.Rows.Count > 0)
            {
                foreach (DataRow ar in dt.Rows)
                {
                    XlsAttendWuHuShanPersonList whsPersonList = new XlsAttendWuHuShanPersonList();

                    whsPersonList.AttendRecordId = Convert.ToInt32(ar["attend_record_id"]);
                    whsPersonList.PersonId = Convert.ToInt32(ar["person_id"]);
                    whsPersonList.DepartName = ar["depart_name"].ToString();
                    whsPersonList.PersonName = ar["name"].ToString();
                    whsPersonList.WorkSn = ar["work_sn"].ToString();
                    whsPersonList.WorkType = ar["work_type_name"].ToString();
                    whsPersonList.Principal = ar["principal_name"].ToString();
                    whsPersonList.ClassOrderName = ar["class_order_name"].ToString();
                    whsPersonList.ClassTypeName = ar["class_type_name"].ToString();
                    whsPersonList.WorkTime = Convert.ToInt32(ar["work_time"]);
                    whsPersonList.AttendTimes = 1;
                    whsPersonList.AttendDay = Convert.ToDateTime(ar["attend_day"]);
                    if ("" != ar["in_well_time"].ToString())
                    {
                        whsPersonList.InWellTime = Convert.ToDateTime(ar["in_well_time"]);
                    }
                    if ("" != ar["out_well_time"].ToString())
                    {
                        whsPersonList.OutWellTime = Convert.ToDateTime(ar["out_well_time"]);
                    }
                    

                    query.Add(whsPersonList);
                }
            }

           #region 查询当前在岗人员中符合条件的数据
//            if (isSelectInOutWell)
//            {
//                string strSql = string.Format(@"select iow.*,wt.work_type_name,pri.principal_name,ct.class_type_name
//                                            from in_out_well as iow
//                                            Left join person p on p.person_id=iow.person_id
//                                            Left join work_type wt on p.work_type_id=wt.work_type_id
//                                            Left join principal pri on p.principal_id=pri.principal_id
//                                            Left join class_type ct ON ct.class_type_id = p.class_type_id
//                                            where (in_time >= '");

//                strSql += beginTime + "\' ) and attend_day < \'" + endTime + "\'";
//                #region 选择条件是否选择了部门
//                if (null != departIdLst)
//                {
//                    bool isall = true;
//                    for (int i = 0; i < departIdLst.Length; i++)
//                    {
//                        if (departIdLst[i] == 0)
//                        {
//                            isall = false;
//                        }
//                    }
//                    if (isall)
//                    {
//                        strSql += " and iow.depart_id in ( ";
//                        for (int i = 0; i < departIdLst.Length - 1; i++)
//                        {
//                            strSql += departIdLst[i] + ",";
//                        }
//                        strSql += departIdLst[departIdLst.Length - 1] + ")";
//                    }
//                }
//                #endregion

//                #region 选择条件是否选择了职务
//                if (null != principalIdList)
//                {
//                    strSql += " and pri.principal_id in ( ";
//                    for (int i = 0; i < principalIdList.Length - 1; i++)
//                    {
//                        strSql += principalIdList[i] + ",";
//                    }
//                    strSql += principalIdList[principalIdList.Length - 1] + ")";
//                }
//                #endregion

//                #region 选择条件是否选择了工种
//                if (null != workTypeIdList)
//                {
//                    strSql += " and wt.work_type_id in ( ";
//                    for (int i = 0; i < workTypeIdList.Length - 1; i++)
//                    {
//                        strSql += workTypeIdList[i] + ",";
//                    }
//                    strSql += workTypeIdList[workTypeIdList.Length - 1] + ")";
//                }
//                #endregion

//                if ("" != name)
//                    strSql += " and iow.name like \'%" + name + "%\'";
//                if ("" != workSn)
//                    strSql += " and iow.work_sn = \'" + workSn + "\'";
//                strSql += " order by iow.depart_name,iow.name,iow.in_time";

//                DataTable dt2 = DbAccess.POSTGRESQL.Select(strSql, "in_out_well");

//                if (null == dt2 || dt2.Rows.Count < 1 )
//                {
//                    return query;
//                }
//                int id = 1;
//                if (query.Count > 0)
//                {
//                    id = query.OrderBy(a => a.AttendRecordId).Select(a => a.AttendRecordId).Last() + 1;
//                }
//                foreach (DataRow ar in dt2.Rows)
//                {
//                    XlsAttendWuHuShanPersonList whsPersonList = new XlsAttendWuHuShanPersonList();

//                    whsPersonList.AttendRecordId = id++;
//                    whsPersonList.PersonId = Convert.ToInt32(ar["person_id"]);
//                    whsPersonList.DepartName = ar["depart_name"].ToString();
//                    whsPersonList.PersonName = ar["name"].ToString();
//                    whsPersonList.WorkSn = ar["work_sn"].ToString();
//                    whsPersonList.WorkType = ar["work_type_name"].ToString();
//                    whsPersonList.Principal = ar["principal_name"].ToString();
//                    whsPersonList.ClassOrderName = ar["class_order_name"].ToString();
//                    whsPersonList.ClassTypeName = ar["class_type_name"].ToString();                  
//                    whsPersonList.AttendDay = Convert.ToDateTime(ar["attend_day"]);
//                    whsPersonList.InWellTime = Convert.ToDateTime(ar["in_time"]);
                    
//                    query.Add(whsPersonList);
//                }
//            }
           #endregion

            return query;
        }

        #endregion

    }
    #region 五虎山独立查询界面实体数据类

    /// <summary>
    /// 五虎山单独查询界面数据类
    /// </summary>
    [DataContract]
    public class XlsAttendWuHuShanPersonList 
    {
        /// <summary>
        /// 考勤id
        /// </summary>
        [DataMember]
        public int AttendRecordId { get; set; }

        /// <summary>
        /// personid
        /// </summary>
        [DataMember]
        [Key]
        public int PersonId { get; set; }

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
        /// 工号
        /// </summary>
        [DataMember]
        public string WorkSn { get; set; }

        /// <summary>
        /// 工种
        /// </summary>
        [DataMember]
        public string WorkType { get; set; }

        /// <summary>
        /// 职务
        /// </summary>
        [DataMember]
        public string Principal { get; set; }

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
        /// 工时
        /// </summary>
        [DataMember]
        public int WorkTime { get; set; }

        /// <summary>
        /// 工数
        /// </summary>
        [DataMember]
        public int AttendTimes { get; set; }

        /// <summary>
        /// 归属日
        /// </summary>
        [DataMember]
        [Key]
        public DateTime AttendDay { get; set; }

        /// <summary>
        /// 上班时间
        /// </summary>
        [DataMember]
        public DateTime? InWellTime { get; set; }

        /// <summary>
        /// 下班时间
        /// </summary>
        [DataMember]
        public DateTime? OutWellTime { get; set; }

        /// <summary>
        /// 定位卡工时
        /// </summary>
        [DataMember]
        public TimeSpan? IrisWorkTime { get; set; }

        /// <summary>
        /// 定位卡入井时间
        /// </summary>
        [DataMember]
        public DateTime? InLocateTime { get; set; }

        /// <summary>
        /// 定位卡出井时间
        /// </summary>
        [DataMember]
        public DateTime? OutLocateTime { get; set; }

        /// <summary>
        /// 定位卡工时
        /// </summary>
        [DataMember]
        public TimeSpan? LocateWorkTime { get; set; }

    }
    
    #endregion

}