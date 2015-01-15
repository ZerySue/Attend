/*************************************************************************
** 文件名:   DomainServiceIriskingAttend_WuHuShan.cs
×× 主要类:   DomainServiceIriskingAttend_WuHuShan
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   wz
** 日  期:   2013-12-17
** 修改人:  
** 日  期:       
** 描  述:   五虎山单独定制域服务
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
using IriskingAttend.Web.WuHuShan;

namespace IriskingAttend.Web
{
     // TODO: 创建包含应用程序逻辑的方法。

    public partial class DomainServiceIriskingAttend : DomainService
    {

        [Update]
        public void TestAttendRecordInfo_WuhuShan(AttendRecordInfo_WuhuShan t)
        {

        }


        #region 从定位卡同步人员相关


        //定位系统人员信息
        internal class TrackPersonInfo
        {
            public string WorkSn { get; set; }
            public string Name { get; set; }
            public string DepartName { get; set; }
            public string PrincipalName { get; set; }
            public string WorkTypeName { get; set; }
        }

        //虹膜系统考勤信息
        internal class IrisPersonInfo
        {
            public int PersonID { get; set; }
            public object DepartID { get; set; }
            public object WorkTypeID { get; set; }
            public object PrincipalID { get; set; }
            public string WorkSn { get; set; }
            public string Name { get; set; }
            public string DepartName { get; set; }
            public string PrincipalName { get; set; }
            public string WorkTypeName { get; set; }
        }

        private static SqlControl _sqlControl;
        private static DataBase _postgreSqlControl;


        private static NpgsqlTransaction _trans;

        private static Dictionary<string, int> _departs = new Dictionary<string, int>();
        private static Dictionary<string, int> _principals = new Dictionary<string, int>();
        private static Dictionary<string, int> _work_types = new Dictionary<string, int>();
        private static DataTable _dtTrack = null;
        private static DataTable _dtIrisPerson = null;

        private static object _syncObj = new object();
        private static bool _isRunSync = false;
        private static OptionInfo _syncResult;
        private static int _curIrisRow = 0;
        private static bool _isCancel;
        private static List<int> _addPersonIds;
        private static List<int> _modifyPersonIds;

        /// <summary>
        /// 处理同步任务
        /// </summary>
        /// <param name="startRow"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [Invoke]
        public OptionInfo SyncPersonInfo(int startRow, int deltaRow, int count)
        {

            if (startRow + deltaRow > _dtTrack.Rows.Count)
            {
                deltaRow = _dtTrack.Rows.Count - startRow;
            }


            if (_syncResult.isSuccess || !_isCancel)
            {
                bool isErr = false;

                for (int j = startRow; j < startRow + deltaRow; j++)
                {
                    DataRow drTrack = _dtTrack.Rows[j];
                    TrackPersonInfo trackPersonInfo = new TrackPersonInfo();
                    trackPersonInfo.DepartName = drTrack["FDeptName"].ToString().Trim();
                    trackPersonInfo.Name = drTrack["FEmpName"].ToString().Trim();
                    trackPersonInfo.WorkSn = drTrack["WorkSn"].ToString().Trim();
                    trackPersonInfo.WorkTypeName = drTrack["FWorkTypeName"].ToString().Trim();
                    trackPersonInfo.PrincipalName = drTrack["FZhiWu"].ToString().Trim();


                    bool isFind = false;
                    for (int i = _curIrisRow; i < _dtIrisPerson.Rows.Count; i++)
                    {
                        string curPName = _dtIrisPerson.Rows[i]["work_sn"].ToString().Trim();

                        if (trackPersonInfo.WorkSn.CompareTo(curPName) == 0)
                        {
                            isFind = true;
                            _curIrisRow = i;
                            break;
                        }
                        if (trackPersonInfo.WorkSn.CompareTo(curPName) < 0)
                        {
                            _curIrisRow = i;
                            break;
                        }
                    }

                    //如果找到了匹配的人员信息
                    if (isFind)
                    {
                        //处理这一条 人员信息， 新的覆盖旧的
                        IrisPersonInfo irisPersonInfo = new IrisPersonInfo();
                        irisPersonInfo.PersonID = (int)_dtIrisPerson.Rows[_curIrisRow]["person_id"];
                        irisPersonInfo.DepartID = _dtIrisPerson.Rows[_curIrisRow]["depart_id"];
                        irisPersonInfo.WorkTypeID = _dtIrisPerson.Rows[_curIrisRow]["work_type_id"];
                        irisPersonInfo.PrincipalID = _dtIrisPerson.Rows[_curIrisRow]["principal_id"];
                        irisPersonInfo.Name = _dtIrisPerson.Rows[_curIrisRow]["name"].ToString();
                        irisPersonInfo.PrincipalName = _dtIrisPerson.Rows[_curIrisRow]["principal_name"].ToString();
                        irisPersonInfo.WorkSn = _dtIrisPerson.Rows[_curIrisRow]["work_sn"].ToString();
                        irisPersonInfo.WorkTypeName = _dtIrisPerson.Rows[_curIrisRow]["work_type_name"].ToString();
                        irisPersonInfo.DepartName = _dtIrisPerson.Rows[_curIrisRow]["depart_name"].ToString();

                        if (irisPersonInfo.PrincipalID == DBNull.Value)
                        {
                            irisPersonInfo.PrincipalID = "null";
                        }
                        if (irisPersonInfo.WorkTypeID == DBNull.Value)
                        {
                            irisPersonInfo.WorkTypeID = "null";
                        }
                        if (irisPersonInfo.DepartID == DBNull.Value)
                        {
                            irisPersonInfo.DepartID = "null";
                        }


                        isErr = !ModifyOneRow(trackPersonInfo, irisPersonInfo);
                        if (isErr)
                        {
                            break;
                        }

                    }
                    else
                    {
                        //添加一条新的人员信息
                        isErr = !InsertOneRow(trackPersonInfo);
                        if (isErr) break;
                    }
                }

                if (isErr && _syncResult.isSuccess)
                {
                    _syncResult.isSuccess = false;
                    //_syncResult.option_info = _postgreSqlControl.GetLastErr();
                }

            }


            //最后一个任务完成清理工作
            if (count <= startRow + deltaRow)
            {
                EndSyncPersonInfo(_isCancel);
                if (_isCancel)
                {
                    _syncResult.tag = -1;
                }
            }


            return _syncResult;

        }

        /// <summary>
        /// 开始同步人员信息
        /// </summary>
        /// <returns></returns>
        [Invoke]
        public int StartSyncPerson()
        {
            lock (_syncObj)
            {
                if (!_isRunSync)
                {
                    _isRunSync = true;
                }
                else
                {
                    return -99;
                }
            }

            _addPersonIds = new List<int>();
            _modifyPersonIds = new List<int>();
            _isCancel = false;
            _curIrisRow = 0;
            _syncResult = new OptionInfo();
            _syncResult.isSuccess = true;
            //获取待同步人员信息
            string strCon = System.Web.Configuration.WebConfigurationManager.AppSettings["SqlServerConnectionStr"].ToString();
            _sqlControl = new SqlControl(strCon);
            if (!_sqlControl.Open(strCon))
            {
                _isRunSync = false;
                return -10;

            }
            _dtTrack = _sqlControl.SelectDT(@"Select a.FID as FEmpID,
                                                    a.FName as FEmpName,
                                                    a.FCode as WorkSn,
                                                    t.FFullName as FDeptName,
                                                    a.FZhiWu,
                                                    w.FName as FWorkTypeName 
                                                    From t_Employee a 
                                                    Left Outer Join Base_Dept t On a.FParentID=t.FID 
                                                    Left Outer Join Base_gz_WorkType w On a.FWorkTypeID=w.FID 
                                                    where a.FCode <> ''
                                                    order by WorkSn;", "");
            _sqlControl.CloseConnect();

            //获取同步目标人员信息
            strCon = System.Web.Configuration.WebConfigurationManager.AppSettings["PostgresConnectionStr"].ToString();
            _postgreSqlControl = new DataBase();
            if (!_postgreSqlControl.Open(strCon))
            {
                _isRunSync = false;
                return -11;
            }

            _dtIrisPerson = _postgreSqlControl.Select(@"SELECT a.person_id, a.depart_name, a.work_sn,
                                            a.name, 
                                           a.depart_id, a.principal_id, a.work_type_id,
                                           p.principal_name, w.work_type_name
                                          FROM person as a
                                          left join principal as p on p.principal_id = a.principal_id
                                          left join work_type as w on w.work_type_id = a.work_type_id
                                          where delete_time is null and a.work_sn <>''
                                          order by work_sn;
                                        ", "");

            //获取部门，职务，工种信息
            _departs = new Dictionary<string, int>();
            DataTable dt = _postgreSqlControl.Select(@"SELECT depart_name,depart_id from depart", "");
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["depart_name"] != DBNull.Value)
                {
                    _departs.Add(dr["depart_name"].ToString(), Convert.ToInt32(dr["depart_id"]));
                }
            }

            _principals = new Dictionary<string, int>();
            dt = _postgreSqlControl.Select(@"SELECT principal_name,principal_id from principal", "");
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["principal_name"] != DBNull.Value)
                {
                    _principals.Add(dr["principal_name"].ToString(), Convert.ToInt32(dr["principal_id"]));
                }
            }

            _work_types = new Dictionary<string, int>();
            dt = _postgreSqlControl.Select(@"SELECT work_type_name,work_type_id from work_type", "");
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["work_type_name"] != DBNull.Value)
                {
                    _work_types.Add(dr["work_type_name"].ToString(), Convert.ToInt32(dr["work_type_id"]));
                }
            }


            //开启事务
            _trans = _postgreSqlControl.BeginTransaction();

            int res = 0;
            res = _dtTrack == null ? -1 : _dtTrack.Rows.Count;

            return res;
        }

        /// <summary>
        /// 取消人员同步
        /// </summary>
        [Invoke]
        public void CancelSyncPerson()
        {
            _isCancel = true;
        }

        /// <summary>
        /// 结束同步人员信息
        /// </summary>   
        private void EndSyncPersonInfo(bool isCancel)
        {
            lock (_syncObj)
            {
                if (_isRunSync)
                {
                    _isRunSync = false; //标志位置为false
                }
                else
                {
                    return;
                }
            }
            _departs.Clear();
            _principals.Clear();
            _work_types.Clear();
            _addPersonIds.Clear();
            _modifyPersonIds.Clear();
            _dtIrisPerson.Dispose();
            _dtTrack.Dispose();
            if (_trans != null)
            {
                if (_syncResult.isSuccess && !isCancel)
                {
                    //数据库提交
                    _trans.Commit();
                    //通知后台已经修改人员属性
                    DataRefChangeData addpersondt = new DataRefChangeData();
                    addpersondt.type = 2;
                    addpersondt.typeTable = 0;
                    addpersondt.len = _modifyPersonIds.Count;
                    int[] personKeyID = _modifyPersonIds.ToArray();
                    ServerComLib.DataRefChange(strIP, Port, ref addpersondt, personKeyID);
                    //通知后台已经添加了人员
                    DataRefChangeData addpersondt1 = new DataRefChangeData();
                    addpersondt.type = 0;
                    addpersondt.typeTable = 0;
                    addpersondt.len = _addPersonIds.Count;
                    int[] personKeyID1 = _addPersonIds.ToArray();
                    ServerComLib.DataRefChange(strIP, Port, ref addpersondt1, personKeyID1);
                }
                else
                {
                    _trans.Rollback();
                }
                _trans.Dispose();
                _trans = null;
            }

            _postgreSqlControl.CloseDataBase();
        }

        //修改一行人员信息
        private bool ModifyOneRow(TrackPersonInfo trackPersonInfo, IrisPersonInfo irisPersonInfo)
        {
            //部门
            if (trackPersonInfo.DepartName != irisPersonInfo.DepartName)
            {
                if (_departs.ContainsKey(trackPersonInfo.DepartName))
                {
                    irisPersonInfo.DepartID = _departs[trackPersonInfo.DepartName];
                }
                else
                {
                    //部门如果为空做判断
                    if (trackPersonInfo.DepartName.Trim() == "")
                    {
                        irisPersonInfo.DepartID = "null";
                    }
                    else
                    {
                        //数据库depart表中 新增一个部门
                        int departId = AddNewDepart(trackPersonInfo.DepartName);
                        if (departId < 0) return false;
                        irisPersonInfo.DepartID = departId;
                    }

                }
            }
            //职务
            if (trackPersonInfo.PrincipalName != irisPersonInfo.PrincipalName)
            {
                if (_principals.ContainsKey(trackPersonInfo.PrincipalName))
                {
                    irisPersonInfo.PrincipalID = _principals[trackPersonInfo.PrincipalName];
                }
                else
                {
                    if (trackPersonInfo.PrincipalName.Trim() == "")
                    {
                        irisPersonInfo.PrincipalID = "null";
                    }
                    else
                    {
                        //数据库principal表中 新增一个item
                        int id = AddNewPricipal(trackPersonInfo.PrincipalName);
                        if (id < 0) return false;
                        irisPersonInfo.PrincipalID = id;
                    }

                }
            }

            //工种
            if (trackPersonInfo.WorkTypeName != irisPersonInfo.WorkTypeName)
            {
                if (_work_types.ContainsKey(trackPersonInfo.WorkTypeName))
                {
                    irisPersonInfo.WorkTypeID = _work_types[trackPersonInfo.WorkTypeName];
                }
                else
                {
                    if (trackPersonInfo.WorkTypeName.Trim() == "")
                    {
                        irisPersonInfo.WorkTypeID = "null";
                    }
                    else
                    {
                        //数据库work_type表中 新增一个item
                        int id = AddNewWorkType(trackPersonInfo.WorkTypeName);
                        if (id < 0) return false;
                        irisPersonInfo.WorkTypeID = id;
                    }

                }
            }

            irisPersonInfo.Name = trackPersonInfo.Name;
            string sql = string.Format(@"UPDATE person_base
                           SET name='{0}',depart_id={1}, principal_id={2}, work_type_id={3}
                         WHERE person_id = {4};", irisPersonInfo.Name, irisPersonInfo.DepartID,
                                                irisPersonInfo.PrincipalID, irisPersonInfo.WorkTypeID,
                                                irisPersonInfo.PersonID);

            if (_postgreSqlControl.ExecuteSql(sql) > 0)
            {
                _modifyPersonIds.Add(irisPersonInfo.PersonID);
                return true;
            }


            return false;
        }

        //插入一行人员信息
        private bool InsertOneRow(TrackPersonInfo trackPersonInfo)
        {
            IrisPersonInfo irisPersonInfo = new IrisPersonInfo();
            irisPersonInfo.Name = trackPersonInfo.Name;
            irisPersonInfo.WorkSn = trackPersonInfo.WorkSn;
            if (_departs.ContainsKey(trackPersonInfo.DepartName))
            {
                irisPersonInfo.DepartID = _departs[trackPersonInfo.DepartName];
            }
            else
            {
                //数据库depart表中 新增一个部门
                int departId = AddNewDepart(trackPersonInfo.DepartName);
                if (departId < 0) return false;
                irisPersonInfo.DepartID = departId;
            }

            //职务
            if (_principals.ContainsKey(trackPersonInfo.PrincipalName))
            {
                irisPersonInfo.PrincipalID = _principals[trackPersonInfo.PrincipalName];
            }
            else
            {
                //数据库principal表中 新增一个item
                int id = AddNewPricipal(trackPersonInfo.PrincipalName);
                if (id < 0) return false;
                irisPersonInfo.PrincipalID = id;
            }


            //工种
            if (_work_types.ContainsKey(trackPersonInfo.WorkTypeName))
            {
                irisPersonInfo.WorkTypeID = _work_types[trackPersonInfo.WorkTypeName];
            }
            else
            {
                //数据库work_type表中 新增一个item
                int id = AddNewWorkType(trackPersonInfo.WorkTypeName);
                if (id < 0) return false;
                irisPersonInfo.WorkTypeID = id;
            }

            string sql = string.Format(@"INSERT INTO person_base(
                    work_sn, name, 
                    depart_id, principal_id, 
                     work_type_id)
                     VALUES ( '{0}', '{1}', 
                         {2}, {3}, {4});", irisPersonInfo.WorkSn, irisPersonInfo.Name,
                                             irisPersonInfo.DepartID, irisPersonInfo.PrincipalID,
                                             irisPersonInfo.WorkTypeID);

            if (_postgreSqlControl.ExecuteSql(sql) > 0)
            {
                sql = "select currval('person_id') ";
                DataTable dt = _postgreSqlControl.Select(sql, "");
                int pid = Convert.ToInt32(dt.Rows[0][0]);
                _addPersonIds.Add(pid);
                return true;
            }

            return false;
        }

        //增加新部门
        private int AddNewDepart(string departName)
        {

            string sql = string.Format(@"insert into depart(depart_name) Values('{0}');", departName);
            if (_postgreSqlControl.ExecuteSql(sql) <= 0)
            {
                return -1;
            }
            sql = "select currval('depart_id') ";
            DataTable dt = _postgreSqlControl.Select(sql, "");
            int res = Convert.ToInt32(dt.Rows[0][0]);
            _departs.Add(departName, res);
            return res;
        }

        //增加新职务
        private int AddNewPricipal(string name)
        {

            string sql = string.Format(@"insert into principal(principal_name) Values('{0}');", name);
            if (_postgreSqlControl.ExecuteSql(sql) <= 0)
            {
                return -1;
            }
            sql = "select currval('principal_id') ";
            DataTable dt = _postgreSqlControl.Select(sql, "");
            int res = Convert.ToInt32(dt.Rows[0][0]);
            _principals.Add(name, res);
            return res;
        }

        //增加新工种
        private int AddNewWorkType(string name)
        {
            string sql = string.Format(@"insert into work_type(work_type_name) Values('{0}');", name);
            if (_postgreSqlControl.ExecuteSql(sql) <= 0)
            {
                return -1;
            }
            sql = "select currval('work_type_id') ";
            DataTable dt = _postgreSqlControl.Select(sql, "");
            int res = Convert.ToInt32(dt.Rows[0][0]);
            _work_types.Add(name, res);
            return res;
        }

        #endregion

        #region 同步定位考勤记录相关

        /// <summary>
        /// 开始同步定位卡考勤信息
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="personIds"></param>
        /// <returns></returns>
        [Invoke(HasSideEffects = true)]
        public string StartSyncLocateAttendInfo(DateTime beginTime, DateTime endTime, int[] personIds)
        {
            string errorStr = "";
            string ServerIP = System.Web.Configuration.WebConfigurationManager.AppSettings["ServerIP"].ToString();
            string ServerPort = System.Web.Configuration.WebConfigurationManager.AppSettings["ServerPort"].ToString();
            ushort port = ushort.Parse(ServerPort);

            //将数据库里，客户参数表中，inSyncLocate字段置为0，表示开始同步定位卡考勤信息
            string sql = @"UPDATE customer_param
                           SET param_value=0
                           WHERE customer_name= 'wuhushan' and 
                           param_name= 'inSyncLocate';";

            string strCon = System.Web.Configuration.WebConfigurationManager.AppSettings["PostgresConnectionStr"].ToString();
            _postgreSqlControl = new DataBase();
            if (!_postgreSqlControl.Open(strCon))
            {
                errorStr = string.Format("错误代码为{0}：打开数据库失败！", -11);
                return errorStr;
            }

            if (_postgreSqlControl.ExecuteSql(sql) <= 0)
            {
                errorStr = string.Format("错误代码为{0}：SQL语句执行失败！", -1);
                return errorStr;
            }
            _postgreSqlControl.CloseDataBase();

            int errorCode = ServerComLib.RefreshLocalData(ServerIP, port, beginTime.ToString(), endTime.ToString(), personIds);
            if (errorCode != 0)
            {
                int error = errorCode;
                errorStr = ServerComLib.GetLastError(ref error);
                errorStr = string.Format("错误代码为{0}({1})：{2}！", errorCode, error, errorStr);
            }
            return errorStr;
        }

        /// <summary>
        /// 获取定位卡考勤同步状态
        /// 0 = 准备开始同步
        /// 1 = 正在同步中
        /// 2 = 同步结束
        /// -1 = 未获取准确数据
        /// </summary>
        /// <returns></returns>
        [Invoke]
        public int GetSyncLocateState()
        {
            string strCon = System.Web.Configuration.WebConfigurationManager.AppSettings["PostgresConnectionStr"].ToString();
            _postgreSqlControl = new DataBase();
            if (!_postgreSqlControl.Open(strCon))
            {
                return -11;
            }

            string sql = @"Select param_value from customer_param
                           WHERE customer_name= 'wuhushan' and 
                           param_name= 'inSyncLocate';";
            DataTable dt = _postgreSqlControl.Select(sql, "");
            _postgreSqlControl.CloseDataBase();
            if (dt.Rows == null)
            {
                return -1;
            }
            else
            {
                object obj = dt.Rows[0][0];
                int res = Convert.ToInt32(obj);
                return res;
            }

        }

        #endregion


       
        /// <summary>
        /// 获取与定位卡结合的异常考勤信息
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="departName"></param>
        /// <param name="personName"></param>
        /// <param name="workSn"></param>
        /// <returns></returns>
        public IEnumerable<AttendRecordInfo_WuhuShan> GetAbnormalAttendRecordInfo_WuHuShan(DateTime beginTime, DateTime endTime,
            int[] departIds, string personName, string workSn)
        {
            List<AttendRecordInfo_WuhuShan> query = new List<AttendRecordInfo_WuhuShan>();
            
            //有定位考勤记录,但出入的时间不完整
            //新增1：有定位 无虹膜记录的 并且 已经注册的人
            //新增2：过滤掉当天有匹配记录的 异常记录
            string sqlLocate = string.Format(@"select a.person_id, a.attend_record_id,a.whs_locate_id ,a.attend_day, pb.work_sn, pb.name, d.depart_name, wt.work_type_name, p.principal_name,ct.class_type_name,c.class_order_name,
                    arn.in_well_time,arn.out_well_time,  (arn.out_well_time - arn.in_well_time) as iris_work_time, 
                    a.in_locate_time, a.out_locate_time, (a.out_locate_time - a.in_locate_time) as locate_work_time
	                ,(arn.in_well_time - a.in_locate_time), (arn.out_well_time - a.out_locate_time)
                 from  whs_in_well_locate as a

                  left join attend_record_normal as arn on ( a.person_id = arn.person_id                 -------2014.2.24修改，定位和虹膜考勤配对条件为:出入井时间相等且人员相同
                                                             and ( a.in_well_time = arn.in_well_time or (a.in_well_time is null and arn.in_well_time is null ) )
                                                             and ( a.out_well_time = arn.out_well_time or (a.out_well_time is null and arn.out_well_time is null ) ))
	           
                  left join class_order_normal as c on  c.class_order_id = arn.class_order_id
                  left join class_type as ct on c.class_type_id = ct.class_type_id
                  left join person_base as pb on pb.person_id = a.person_id
                  left join depart as d on pb.depart_id = d.depart_id
                  left join principal as p on pb.principal_id = p.principal_id
                  left join work_type as wt on wt.work_type_id = pb.work_type_id
                  left join (select distinct person_id from  person_enroll_info)  as pei on pei.person_id = pb.person_id  ---过滤掉没有注册虹膜的人员

		          left join (select person_id ,attend_day,locate_day from whs_in_well_locate where attend_day is not null) as matchT   ---过滤当天已有匹配记录的异常记录
			        on a.person_id = matchT.person_id and to_char(a.locate_day, 'yyyy-MM-DD') = to_char(matchT.locate_day, 'yyyy-MM-DD')
                  
                  where a.locate_day between '{0}' and '{1}'
                   and pei.person_id is not null  ---过滤掉没有注册虹膜的人员
                   and matchT.person_id is  null   ---过滤当天已有匹配记录的异常记录
                   and 
	                 ((arn.in_well_time - a.in_locate_time) is null            ----残缺记录
	                or (arn.out_well_time - a.out_locate_time) is null )
                   ", beginTime, endTime);

            //部门过滤
            if (departIds != null && departIds.Length > 0)
            {
                StringBuilder sqlDepartWhere = new StringBuilder("and d.depart_id in (");
                foreach (var item in departIds)
                {
                    sqlDepartWhere.Append(item + ",");
                }
                sqlDepartWhere.Remove(sqlDepartWhere.Length - 1, 1);
                sqlDepartWhere.Append(")");
                sqlLocate = sqlLocate + sqlDepartWhere;
            }
            //人名过滤
            if (personName != null && personName != "")
            {
                sqlLocate = sqlLocate + String.Format("and pb.name like '%{0}%'", personName);
            }

            //工号过滤
            if (workSn != null && workSn != "")
            {
                sqlLocate = sqlLocate + String.Format("and pb.work_sn = '{0}'", workSn);
            }
            sqlLocate += " order by convert_to(pb.name,  E'GBK'),a.attend_day";

            DataTable dt = DbAccess.POSTGRESQL.Select(sqlLocate, "");

            if (dt != null && dt.Rows.Count >= 1)
            {
                foreach (DataRow ar in dt.Rows)
                {

                    AttendRecordInfo_WuhuShan item = new AttendRecordInfo_WuhuShan();

                    #region     数据填充
                    if (ar["attend_day"] != DBNull.Value)
                    {
                        item.attend_day = Convert.ToDateTime(ar["attend_day"]);
                    }
                    if (ar["attend_record_id"] != DBNull.Value)
                    {
                        item.attend_record_id = Convert.ToInt32(ar["attend_record_id"]);
                    }
                    if (ar["class_order_name"] != DBNull.Value)
                    {
                        item.class_order_name = Convert.ToString(ar["class_order_name"]);
                    }
                    if (ar["class_type_name"] != DBNull.Value)
                    {
                        item.class_type_name = Convert.ToString(ar["class_type_name"]);
                    }
                    if (ar["depart_name"] != DBNull.Value)
                    {
                        item.depart_name = Convert.ToString(ar["depart_name"]);
                    }
                    if (ar["in_locate_time"] != DBNull.Value)
                    {
                        item.in_locate_time = Convert.ToDateTime(ar["in_locate_time"]);
                    }
                    if (ar["in_well_time"] != DBNull.Value)
                    {
                        item.in_well_time = Convert.ToDateTime(ar["in_well_time"]);
                    }
                    if (ar["iris_work_time"] != DBNull.Value)
                    {
                        item.iris_work_time = (TimeSpan)(ar["iris_work_time"]);
                    }
                    if (ar["locate_work_time"] != DBNull.Value)
                    {
                        item.locate_work_time = (TimeSpan)(ar["locate_work_time"]);
                    }
                    if (ar["name"] != DBNull.Value)
                    {
                        item.name = Convert.ToString(ar["name"]);
                    }
                    if (ar["out_locate_time"] != DBNull.Value)
                    {
                        item.out_locate_time = Convert.ToDateTime(ar["out_locate_time"]);
                    }
                    if (ar["out_well_time"] != DBNull.Value)
                    {
                        item.out_well_time = Convert.ToDateTime(ar["out_well_time"]);
                    }
                    if (ar["person_id"] != DBNull.Value)
                    {
                        item.person_id = Convert.ToInt32(ar["person_id"]);
                    }
                    if (ar["principal_name"] != DBNull.Value)
                    {
                        item.principal_name = Convert.ToString(ar["principal_name"]);
                    }
                    if (ar["whs_locate_id"] != DBNull.Value)
                    {
                        item.whs_locate_id = Convert.ToInt32(ar["whs_locate_id"]);
                    }
                    if (ar["work_sn"] != DBNull.Value)
                    {
                        item.work_sn = Convert.ToString(ar["work_sn"]);
                    }
                    if (ar["work_type_name"] != DBNull.Value)
                    {
                        item.work_type_name = Convert.ToString(ar["work_type_name"]);
                    }
                    item.attend_state = CalAttendState(item);

                    #endregion

                    query.Add(item);
                }

            }

            //无定位卡考勤记录,有虹膜考勤记录
            //新增2：过滤掉当天有匹配记录的 异常记录
            string sqlNoneLocate = string.Format(@"SELECT a.person_id, a.attend_record_id, null as whs_locate_id,a.attend_day, pb.work_sn, pb.name, 
               d.depart_name, wt.work_type_name, p.principal_name,ct.class_type_name,c.class_order_name,
               a.in_well_time, a.out_well_time, (a.out_well_time - a.in_well_time) as iris_work_time ,
               null as in_locate_time ,null as out_locate_time, null as locate_work_time 

              FROM attend_record_normal as a
  
              left join class_order_normal as c on  c.class_order_id = a.class_order_id
              left join class_type as ct on c.class_type_id = ct.class_type_id
              left join person_base as pb on pb.person_id = a.person_id
              left join depart as d on pb.depart_id = d.depart_id
              left join principal as p on pb.principal_id = p.principal_id
              left join work_type as wt on wt.work_type_id = pb.work_type_id
              LEFT JOIN whs_in_well_locate AS whs ON  -------2014.2.24修改，定位和虹膜考勤配对条件为:出入井时间相等且人员相同
              (
                    whs.person_id = a.person_id
                    and ( whs.in_well_time = a.in_well_time or (a.in_well_time is null and whs.in_well_time is null ) )
                    and ( whs.out_well_time = a.out_well_time or (a.out_well_time is null and whs.out_well_time is null ) )
              )                 
              left join (select person_id ,attend_day,locate_day from whs_in_well_locate where attend_day is not null) as matchT   ---过滤当天已有匹配记录的异常记录
			        on a.person_id = matchT.person_id and to_char(a.attend_day, 'yyyy-MM-DD') = to_char(matchT.locate_day, 'yyyy-MM-DD')                      

	          WHERE 
	          whs.attend_record_id IS NULL AND 
               matchT.person_id is null and  ---过滤当天已有匹配记录的异常记录
              a.attend_day between '{0}' and '{1}' ", beginTime, endTime);

            //部门过滤
            if (departIds != null && departIds.Length > 0)
            {
                StringBuilder sqlDepartWhere = new StringBuilder("and d.depart_id in (");
                foreach (var item in departIds)
                {
                    sqlDepartWhere.Append(item + ",");
                }
                sqlDepartWhere.Remove(sqlDepartWhere.Length - 1, 1);
                sqlDepartWhere.Append(")");
                sqlNoneLocate = sqlNoneLocate + sqlDepartWhere;
            }
            //人名过滤
            if (personName != null && personName != "")
            {
                sqlNoneLocate = sqlNoneLocate + String.Format("and pb.name like '%{0}%'", personName);
            }

            //工号过滤
            if (workSn != null && workSn != "")
            {
                sqlNoneLocate = sqlNoneLocate + String.Format("and pb.work_sn = '{0}'", workSn);
            }
            sqlNoneLocate += " order by convert_to(pb.name,  E'GBK'),a.attend_day";

            dt = DbAccess.POSTGRESQL.Select(sqlNoneLocate, "");

            if (dt == null || dt.Rows.Count < 1)
            {
                return query;
            }
            foreach (DataRow ar in dt.Rows)
            {
                AttendRecordInfo_WuhuShan item = new AttendRecordInfo_WuhuShan();

                #region     数据填充
                if (ar["attend_day"] != DBNull.Value)
                {
                    item.attend_day = Convert.ToDateTime(ar["attend_day"]);
                }
                if (ar["attend_record_id"] != DBNull.Value)
                {
                    item.attend_record_id = Convert.ToInt32(ar["attend_record_id"]);
                }
                if (ar["class_order_name"] != DBNull.Value)
                {
                    item.class_order_name = Convert.ToString(ar["class_order_name"]);
                }
                if (ar["class_type_name"] != DBNull.Value)
                {
                    item.class_type_name = Convert.ToString(ar["class_type_name"]);
                }
                if (ar["depart_name"] != DBNull.Value)
                {
                    item.depart_name = Convert.ToString(ar["depart_name"]);
                }
                if (ar["in_locate_time"] != DBNull.Value)
                {
                    item.in_locate_time = Convert.ToDateTime(ar["in_locate_time"]);
                }
                if (ar["in_well_time"] != DBNull.Value)
                {
                    item.in_well_time = Convert.ToDateTime(ar["in_well_time"]);
                }
                if (ar["iris_work_time"] != DBNull.Value)
                {
                    item.iris_work_time = (TimeSpan)(ar["iris_work_time"]);
                }
                if (ar["locate_work_time"] != DBNull.Value)
                {
                    item.locate_work_time = (TimeSpan)(ar["locate_work_time"]);
                }
                if (ar["name"] != DBNull.Value)
                {
                    item.name = Convert.ToString(ar["name"]);
                }
                if (ar["out_locate_time"] != DBNull.Value)
                {
                    item.out_locate_time = Convert.ToDateTime(ar["out_locate_time"]);
                }
                if (ar["out_well_time"] != DBNull.Value)
                {
                    item.out_well_time = Convert.ToDateTime(ar["out_well_time"]);
                }
                if (ar["person_id"] != DBNull.Value)
                {
                    item.person_id = Convert.ToInt32(ar["person_id"]);
                }
                if (ar["principal_name"] != DBNull.Value)
                {
                    item.principal_name = Convert.ToString(ar["principal_name"]);
                }
                if (ar["whs_locate_id"] != DBNull.Value)
                {
                    item.whs_locate_id = Convert.ToInt32(ar["whs_locate_id"]);
                }
                if (ar["work_sn"] != DBNull.Value)
                {
                    item.work_sn = Convert.ToString(ar["work_sn"]);
                }
                if (ar["work_type_name"] != DBNull.Value)
                {
                    item.work_type_name = Convert.ToString(ar["work_type_name"]);
                }
                item.attend_state = CalAttendState(item);

                #endregion

                query.Add(item);
            }


            return query;
        }

        /// <summary>
        /// 添加多条识别记录
        /// </summary>
        /// <returns></returns>
        [Invoke(HasSideEffects = true)]
        public int IrisBatchInsertRecogLog_WuHuShan(int[] personIds, DateTime[] recogTimes, int[] devTypes, string operatorName)
        {
            //DateTime startTime = DateTime.Now;
            //TimeSpan spendTime;
            try
            {

                List<IdentifyData> identifyDatas = new List<IdentifyData>();
                for (int i = 0; i < personIds.Length; i++)
                {
                    IdentifyData data = new IdentifyData();
                    data.class_type_id = 0;
                    data.class_type_name = "";
                    data.depart_id = 0;
                    data.depart_name = "";
                    data.id = 0;
                    data.peson_name = "";
                    data.work_sn = "";
                    data.recog_type = 0; //手动补加为0
                    data.person_id = personIds[i];
                    data.recog_time = recogTimes[i].ToString();
                    data.at_position = "";
                    data.dev_sn = "";
                    data.dev_type = devTypes[i];
                    data.operator_name = operatorName;
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
        /// 计算考勤状态
        /// 0 = 正常
        /// 1 = 未完成出入井虹膜， 但出入井定位信息都有
        /// 2 = 未完成出入井定位，但出入井虹膜都有
        /// 3 = 出入井虹膜和出入井定位都有，但时间限制不满足
        /// 4 = 出入井虹膜和定位卡 考勤都残缺
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private int CalAttendState(AttendRecordInfo_WuhuShan info)
        {
            if ((info.iris_work_time == null) &&
                (info.locate_work_time != null))
            {
                return 1;  //缺少虹膜考勤
            }
            if ((info.iris_work_time != null) &&
               (info.locate_work_time == null))
            {
                return 2; //缺少定位卡考勤
            }
            if ((info.iris_work_time != null) &&
              (info.locate_work_time != null))
            {
                return 0;
            }
            else
            {
                return 4; //出入井虹膜和定位卡 考勤都残缺
            }


        }



        /// <summary>
        /// 获取所有已注册人员的id
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UserPersonInfo> GetAllPersonIds()
        {
            List<UserPersonInfo> allPersonIds = new List<UserPersonInfo>();
            string strSQL = string.Format(@"select pei.person_id from person_enroll_info as pei left join person_base as p on p.person_id=pei.person_id group by pei.person_id order by pei.person_id");

            DataTable dt = DbAccess.POSTGRESQL.Select(strSQL, "");

            if (dt != null && dt.Rows.Count >= 1)
            {
                foreach (DataRow ar in dt.Rows)
                {

                    UserPersonInfo item = new UserPersonInfo();

                    if (ar["person_id"] != DBNull.Value)
                    {
                        item.person_id = Convert.ToInt32(ar["person_id"]);
                    }
                    allPersonIds.Add(item);
                }

            }
            return allPersonIds;
        }
    }
}