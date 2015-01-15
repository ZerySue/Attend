/*************************************************************************
** 文件名:   DomainServiceIriskingAttend_ZhouYuanShan.cs
×× 主要类:   DomainServiceIriskingAttend_ZhouYuanShan
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   wz
** 日  期:   2013-12-17
** 修改人:  
** 日  期:       
** 描  述:   周源山单独定制域服务
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
using IriskingAttend.Web.ZhouYuanShan;

namespace IriskingAttend.Web
{
     // TODO: 创建包含应用程序逻辑的方法。
    
    public partial class DomainServiceIriskingAttend : DomainService
    {

        [Update]
        public void Test_ZhouYuanShan(WorkTypeInfo_ZhouYuanShan t)
        {

        }

        [Update]
        public void Test_ZhouYuanShan(ReportRecordInfoOnDepart_ZhouYuanShan t)
        {

        }

        [Update]
        public void Test_ZhouYuanShan(LunchRecordInfoOnDepart t)
        {

        }


        [Update]
        public void Test_ZhouYuanShan(LunchRecordInfoOnPerson t)
        {

        }


        #region 报表查询页面
       
      
        /// <summary>
        /// 获取与部门结合的工种信息
        /// </summary>
        /// <returns></returns>
        public IEnumerable<WorkTypeInfo_ZhouYuanShan> GetWorkTypeInfo_ZhouYuanShan(int[] departIds)
        {
            List<WorkTypeInfo_ZhouYuanShan> query = new List<WorkTypeInfo_ZhouYuanShan>();

            StringBuilder sbDepart = new StringBuilder();
            if (departIds == null || departIds.Length == 0)
            {
                sbDepart.Append(" pb.depart_id = -1");
            }
            else
            {
                sbDepart.Append(" pb.depart_id in (");
                foreach (var item in departIds)
                {
                    sbDepart.Append(item);
                    sbDepart.Append(',');
                }
                sbDepart.Remove(sbDepart.Length - 1, 1);
                sbDepart.Append(")");
            }
           
            string sql = string.Format(@"select wt.work_type_id, wt.work_type_name,pb.depart_id from person_base as pb

                left join work_type as wt on wt.work_type_id = pb.work_type_id

                where wt.work_type_id is not null and pb.depart_id is not null
                and {0}

                group by depart_id,wt.work_type_id,wt.work_type_name

                order by convert_to(wt.work_type_name,  E'GBK')",sbDepart.ToString());

            DataTable dt = DbAccess.POSTGRESQL.Select(sql, "");

            if (dt.Rows == null)
            {
                return query;
            }
            foreach (DataRow ar in dt.Rows)
            {

                WorkTypeInfo_ZhouYuanShan item = new WorkTypeInfo_ZhouYuanShan();

                #region     数据填充
                if (ar["depart_id"] != DBNull.Value)
                {
                    item.depart_id = Convert.ToInt32(ar["depart_id"]);
                }
                if (ar["work_type_id"] != DBNull.Value)
                {
                    item.work_type_id = Convert.ToInt32(ar["work_type_id"]);
                }
                if (ar["work_type_name"] != DBNull.Value)
                {
                    item.work_type_name = Convert.ToString(ar["work_type_name"]);
                }
                #endregion

                query.Add(item);
            }
            return query;
        }

        /// <summary>
        /// 获取与部门结合的人员信息
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UserPersonInfo> GetPersonInfo_ZhouYuanShan(int[] departIds)
        {
            List<UserPersonInfo> query = new List<UserPersonInfo>();

            StringBuilder sbDepart = new StringBuilder();
            if (departIds == null || departIds.Length == 0)
            {
                sbDepart.Append(" person_base.depart_id = -1");
            }
            else
            {
                sbDepart.Append(" person_base.depart_id in (");
                foreach (var item in departIds)
                {
                    sbDepart.Append(item);
                    sbDepart.Append(',');
                }
                sbDepart.Remove(sbDepart.Length - 1, 1);
                sbDepart.Append(")");
            }

            string sql = string.Format(@"SELECT person_id, work_sn, name, depart_id
                    FROM person_base 
                    where {0}
                    order by  convert_to(name,  E'GBK');",sbDepart.ToString());

            DataTable dt = DbAccess.POSTGRESQL.Select(sql, "");

            if (dt.Rows == null)
            {
                return query;
            }
            foreach (DataRow ar in dt.Rows)
            {
                UserPersonInfo item = new UserPersonInfo();

                #region     数据填充
                if (ar["depart_id"] != DBNull.Value)
                {
                    item.depart_id = Convert.ToInt32(ar["depart_id"]);
                }
                if (ar["person_id"] != DBNull.Value)
                {
                    item.person_id = Convert.ToInt32(ar["person_id"]);
                }
                if (ar["work_sn"] != DBNull.Value)
                {
                    item.work_sn = Convert.ToString(ar["work_sn"]);
                }
                if (ar["name"] != DBNull.Value)
                {
                    item.person_name = Convert.ToString(ar["name"]);
                }
                #endregion

                query.Add(item);
            }
            return query;
        }
        #endregion

        #region 班中餐模块

        /// <summary>
        /// 查询某时间段的真实考勤记录（按照部门、时间、班次分组）,用于上报班中餐记录
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ReportRecordInfoOnDepart_ZhouYuanShan> GetUnCompletedReportRecordInfoOnDepart(DateTime beginTime,DateTime endTime,int[] departIds)
        {
            List<ReportRecordInfoOnDepart_ZhouYuanShan> query = new List<ReportRecordInfoOnDepart_ZhouYuanShan>();


            string sql = string.Format(
                    @"SELECT * FROM get_uncompleted_report_record_Info('{0}','{1}') as 
                        (depart_name VARCHAR,attend_day date,class_order_name VARCHAR,attend_sign VARCHAR,
                         attend_person_count bigint, attend_person_names VARCHAR[], attend_person_ids INTEGER[],
                         attend_record_ids INTEGER[], class_order_id INTEGER,
                         depart_id INTEGER);
                    ", beginTime,endTime);

            DataTable dt = DbAccess.POSTGRESQL.Select(sql, "");

            if (dt == null || dt.Rows == null)
            {
                return query;
            }
            foreach (DataRow ar in dt.Rows)
            {
                ReportRecordInfoOnDepart_ZhouYuanShan item = new ReportRecordInfoOnDepart_ZhouYuanShan();

                #region     数据填充
                if (ar["attend_day"] != DBNull.Value)
                {
                    item.attend_day = Convert.ToDateTime(ar["attend_day"]);
                }
                if (ar["attend_person_count"] != DBNull.Value)
                {
                    item.attend_person_count = Convert.ToInt32(ar["attend_person_count"]);
                }
                if (ar["attend_person_names"] != DBNull.Value)
                {
                    item.attend_person_names = (string[])(ar["attend_person_names"]);
                    item.reported_count = item.attend_person_names.Length;
                }
                if (ar["attend_person_ids"] != DBNull.Value)
                {
                    item.attend_person_ids = (int[])(ar["attend_person_ids"]);
                }
                if (ar["attend_record_ids"] != DBNull.Value)
                {
                    item.attend_record_ids = (int[])(ar["attend_record_ids"]);
                }
                if (ar["attend_sign"] != DBNull.Value)
                {
                    item.attend_sign = Convert.ToString(ar["attend_sign"]);
                }
                if (ar["class_order_name"] != DBNull.Value)
                {
                    item.class_order_name = Convert.ToString(ar["class_order_name"]);
                }
                if (ar["class_order_id"] != DBNull.Value)
                {
                    item.class_order_id = Convert.ToInt32(ar["class_order_id"]);
                }
                if (ar["depart_id"] != DBNull.Value)
                {
                    item.depart_id = Convert.ToInt32(ar["depart_id"]);
                }
                if (ar["depart_name"] != DBNull.Value)
                {
                    item.depart_name = Convert.ToString(ar["depart_name"]);
                }
                item.diff_person_names = null;
                item.state = "未完成";
                
                #endregion

                bool isAdd = true;
                //部门过滤
                if (departIds != null && departIds.Length > 0)
                {
                    //部门过滤
                    if (!departIds.Contains(item.depart_id))
                    {
                        isAdd = false;
                    }
                }
                else
                {
                    isAdd = false;
                }

                if (isAdd)
                {
                    query.Add(item);
                }
            }
            return query;
        }

        /// <summary>
        /// 查询某时间段的已完成班中餐记录
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ReportRecordInfoOnDepart_ZhouYuanShan> GetCompletedReportRecordInfoOnDepart(DateTime beginTime, DateTime endTime,int[] departIds)
        {
            List<ReportRecordInfoOnDepart_ZhouYuanShan> query = new List<ReportRecordInfoOnDepart_ZhouYuanShan>();


            string sql = string.Format(
                    @"select report_record_ids,depart_id,depart_name, report_day, report_class_order_id,class_order_name,attend_sign,
                    attend_person_ids,attend_person_names,diff_person_ids,diff_person_names

                     from completed_lunch_record
                     where report_day between '{0}' and '{1}'
                     order by convert_to(depart_name,  E'GBK'),report_day,convert_to(class_order_name,  E'GBK')
                    ", beginTime, endTime);

            DataTable dt = DbAccess.POSTGRESQL.Select(sql, "");

            if (dt == null || dt.Rows == null)
            {
                return query;
            }
            foreach (DataRow ar in dt.Rows)
            {
                ReportRecordInfoOnDepart_ZhouYuanShan item = new ReportRecordInfoOnDepart_ZhouYuanShan();

                #region     数据填充
                if (ar["depart_name"] != DBNull.Value)
                {
                    item.depart_name = Convert.ToString(ar["depart_name"]);
                }
                if (ar["depart_id"] != DBNull.Value)
                {
                    item.depart_id = Convert.ToInt32(ar["depart_id"]);
                }
                if (ar["report_day"] != DBNull.Value)
                {
                    item.attend_day = Convert.ToDateTime(ar["report_day"]);
                }
                if (ar["report_class_order_id"] != DBNull.Value)
                {
                    item.class_order_id = Convert.ToInt32(ar["report_class_order_id"]);
                }
                if (ar["attend_sign"] != DBNull.Value)
                {
                    item.attend_sign = Convert.ToString(ar["attend_sign"]);
                }
                if (ar["class_order_name"] != DBNull.Value)
                {
                    item.class_order_name = Convert.ToString(ar["class_order_name"]);
                }
                if (ar["attend_person_ids"] != DBNull.Value)
                {
                    item.attend_person_ids = StringArrayToIntArray((string[])(ar["attend_person_ids"]));
                }
                if (ar["attend_person_names"] != DBNull.Value)
                {
                    item.attend_person_names = (string[])(ar["attend_person_names"]);
                }
                if (ar["diff_person_ids"] != DBNull.Value)
                {
                    item.diff_person_ids = StringArrayToIntArray((string[])(ar["diff_person_ids"]));
                }
                if (ar["diff_person_names"] != DBNull.Value)
                {
                    item.diff_person_names = (string[])(ar["diff_person_names"]);
                }
                if (ar["report_record_ids"] != DBNull.Value)
                {
                    item.report_record_ids = (int[])(ar["report_record_ids"]);
                }
                
                
                item.attend_person_count = item.attend_person_names.Length;
                item.reported_count = item.attend_person_names.Length + item.diff_person_names.Length;

                item.state = "已完成";

                #endregion

                bool isAdd = true;
                //部门过滤
                if (departIds != null && departIds.Length > 0)
                {
                    //部门过滤
                    if (!departIds.Contains(item.depart_id))
                    {
                        isAdd = false;
                    }
                }
                else
                {
                    isAdd = false;
                }

                if (isAdd)
                {
                    query.Add(item);
                }
                
            }
            return query;
        }


        /// <summary>
        /// 生成已完成班中餐记录
        /// </summary>
        /// <param name="infos"></param>
        /// <returns></returns>
        [Invoke(HasSideEffects=true)]
        public OptionInfo CreateReportRecord(ReportRecordInfoOnDepart_ZhouYuanShan[] infos)
        {
            OptionInfo optionInfo = new OptionInfo();

            //开启事务过程
            NpgsqlTransaction trans = DbAccess.POSTGRESQL.BeginTransaction();
            bool isSuccess = true;

            foreach (var item in infos)
            {
                if (item.attend_person_ids != null)
                {
                    for (int i = 0; i < item.attend_person_ids.Length; i++)
                    {
                        //提交真实的班中餐记录
                        string sql = string.Format(@"INSERT INTO report_record(
                            person_id, report_day, report_class_order_id, 
                            attend_record_id, state,depart_id)
                            VALUES ({0}, '{1}', {2}, 
                                {3}, '已完成',{4});",
                                item.attend_person_ids[i],
                                item.attend_day,
                                item.class_order_id,
                                item.attend_record_ids[i],
                                item.depart_id);
                        if (DbAccess.POSTGRESQL.ExecuteSql(sql) <= 0)
                        {
                            isSuccess = false;
                            break;
                        }
                    }
                }

                if (item.diff_person_ids != null)
                {
                    for (int i = 0; i < item.diff_person_ids.Length; i++)
                    {
                        //提交虚拟的班中餐记录(attend_record_id为空)
                        string sql = string.Format(@"INSERT INTO report_record(
                            person_id, report_day, report_class_order_id, 
                            attend_record_id, state,depart_id)
                            VALUES ({0}, '{1}', {2}, 
                                null, '已完成',{3});",
                                item.diff_person_ids[i],
                                item.attend_day,
                                item.class_order_id,
                                item.depart_id);
                        if (DbAccess.POSTGRESQL.ExecuteSql(sql) <= 0)
                        {
                            isSuccess = false;
                            break;
                        }
                    }
                }
                if (!isSuccess)
                {
                    break;
                }
                
            }

            //提交事务
            if (isSuccess)
            {
                optionInfo.isSuccess = true;
                trans.Commit();
            }
            else
            {
                optionInfo.isSuccess = false;
                optionInfo.option_info = "生成班中餐记录失败！";
                trans.Rollback();
            }
            trans.Dispose();
            trans = null;

            return optionInfo;
        }

        /// <summary>
        /// 撤消某条已完成的班中餐记录
        /// </summary>
        /// <param name="infos"></param>
        /// <returns></returns>
        [Invoke(HasSideEffects = true)]
        public OptionInfo UndoReportRecord(ReportRecordInfoOnDepart_ZhouYuanShan[] infos)
        {
            OptionInfo optionInfo = new OptionInfo();
            if (infos == null || infos.Length == 0 )
            {
                optionInfo.isSuccess = false;
                optionInfo.option_info = "待撤消记录为空！";
                return optionInfo;
            }

            
            
            StringBuilder sql = new StringBuilder(@"DELETE FROM report_record
                WHERE report_record_id in (");

            foreach (var info in infos)
            {
                foreach (var item in info.report_record_ids)
                {
                    sql.Append(item + ",");
                }
            }

           
            sql.Remove(sql.Length - 1, 1);
            sql.Append(");");
            if (DbAccess.POSTGRESQL.ExecuteSql(sql.ToString()) <= 0)
            {
                optionInfo.isSuccess = false;
                optionInfo.option_info = "撤消记录失败！";
            }

            optionInfo.isSuccess = true;
            return optionInfo;
        }

        //将string数组装换为Int数组
        private int[] StringArrayToIntArray(string[] array)
        {
            if (array == null) return null;
            int[] res = new int[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                try
                {
                    res[i] = int.Parse(array[i]);
                }
                catch (System.Exception ex)
                {
                	
                }
            }
            return res;
        }

        #endregion

        #region 查询班中餐模块

        /// <summary>
        /// 查询班中餐记录
        /// 以部门分组
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public IEnumerable<LunchRecordInfoOnDepart> GetLunchRecordOnDepart(DateTime beginTime, DateTime endTime,int[] departIds)
        {
            List<LunchRecordInfoOnDepart> query = new List<LunchRecordInfoOnDepart>();
            string sql = string.Format(
                    @"SELECT * FROM get_lunch_record_Info_on_depart('{0}','{1}') as 
                    (depart_name VARCHAR,depart_id INTEGER,report_record_ids INTEGER[],attend_person_ids TEXT[],
                     attend_person_names TEXT[], diff_person_ids TEXT[], diff_person_names TEXT[])
                     order by convert_to(depart_name,  E'GBK');
                    ", beginTime, endTime);

            DataTable dt = DbAccess.POSTGRESQL.Select(sql, "");

            if (dt == null || dt.Rows == null)
            {
                return query;
            }
            foreach (DataRow ar in dt.Rows)
            {
                LunchRecordInfoOnDepart item = new LunchRecordInfoOnDepart();

                #region     数据填充
                if (ar["depart_name"] != DBNull.Value)
                {
                    item.depart_name = Convert.ToString(ar["depart_name"]);
                }
                if (ar["depart_id"] != DBNull.Value)
                {
                    item.depart_id = Convert.ToInt32(ar["depart_id"]);
                }
                if (ar["report_record_ids"] != DBNull.Value)
                {
                    item.report_record_ids = (int[])(ar["report_record_ids"]);
                }
                if (ar["attend_person_ids"] != DBNull.Value)
                {
                    item.attend_person_ids = StringArrayToIntArray((string[])(ar["attend_person_ids"]));
                }
                if (ar["attend_person_names"] != DBNull.Value)
                {
                    item.attend_person_names = (string[])(ar["attend_person_names"]);
                }
                if (ar["diff_person_ids"] != DBNull.Value)
                {
                    item.diff_person_ids = StringArrayToIntArray((string[])(ar["diff_person_ids"]));
                }
                if (ar["diff_person_names"] != DBNull.Value)
                {
                    item.diff_person_names = (string[])(ar["diff_person_names"]);
                }
                
                item.attend_person_count = item.attend_person_names.Length;
                item.reported_count = item.attend_person_names.Length + item.diff_person_names.Length;
                item.diff_person_count = item.diff_person_names.Length; 
                

                //将差异人次改为差异人员，并计算每人差异的次数
                List<string> diff_person_names = new List<string>();
                List<int> diff_count_per_person = new List<int>();
                foreach (var name in item.diff_person_names)
                {
                    int index = diff_person_names.IndexOf(name);
                    if (index >= 0)
                    {
                        diff_count_per_person[index]++;
                    }
                    else
                    {
                        diff_person_names.Add(name);
                        diff_count_per_person.Add(1);
                    }
                }
                item.diff_person_names = diff_person_names.ToArray();
                item.diff_count_per_person = diff_count_per_person.ToArray();

                #endregion

                

                //如果设置的待选部门，则启用部门id过滤
                if (departIds != null && departIds.Length > 0)
                {
                    if (departIds.Contains(item.depart_id))
                    {
                        query.Add(item);
                    }
                }
                else
                {
                    query.Add(item);
                }
            }
            return query;
        }

        /// <summary>
        /// 查询班中餐记录详情
        /// 不分组
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public IEnumerable<LunchRecordInfoOnPerson> GetLunchRecordOnPerson(DateTime beginTime, DateTime endTime,int[] personIds,int[] departIds)
        {
            List<LunchRecordInfoOnPerson> query = new List<LunchRecordInfoOnPerson>();
          
            string sql = string.Format(
                    @" select work_sn,name,report_depart_name,depart_name,report_day,attend_sign,
              class_order_name,person_id,report_class_order_id,report_depart_id,depart_id from lunch_record_on_person
                    where report_day between '{0}' and '{1}'
                    order by convert_to(report_depart_name,  E'GBK'),report_day,convert_to(name,  E'GBK');
                    ", beginTime, endTime);

            DataTable dt = DbAccess.POSTGRESQL.Select(sql, "");

            if (dt == null || dt.Rows == null)
            {
                return query;
            }
            foreach (DataRow ar in dt.Rows)
            {
                LunchRecordInfoOnPerson item = new LunchRecordInfoOnPerson();

                #region     数据填充
                if (ar["work_sn"] != DBNull.Value)
                {
                    item.work_sn = Convert.ToString(ar["work_sn"]);
                }
                if (ar["name"] != DBNull.Value)
                {
                    item.name = Convert.ToString(ar["name"]);
                }
                if (ar["report_depart_name"] != DBNull.Value)
                {
                    item.report_depart_name = Convert.ToString(ar["report_depart_name"]);
                }
                if (ar["depart_name"] != DBNull.Value)
                {
                    item.depart_name = Convert.ToString(ar["depart_name"]);
                }
                
                if (ar["report_day"] != DBNull.Value)
                {
                    item.report_day = Convert.ToDateTime(ar["report_day"]);
                }
                if (ar["attend_sign"] != DBNull.Value)
                {
                    item.attend_sign = Convert.ToString(ar["attend_sign"]);
                }
                if (ar["class_order_name"] != DBNull.Value)
                {
                    item.class_order_name = Convert.ToString(ar["class_order_name"]);
                }
                if (ar["person_id"] != DBNull.Value)
                {
                    item.person_id = Convert.ToInt32(ar["person_id"]);
                }
                if (ar["report_class_order_id"] != DBNull.Value)
                {
                    item.report_class_order_id = Convert.ToInt32(ar["report_class_order_id"]);
                }
                if (ar["report_depart_id"] != DBNull.Value)
                {
                    item.report_depart_id = Convert.ToInt32(ar["report_depart_id"]);
                }
                if (ar["depart_id"] != DBNull.Value)
                {
                    item.depart_id = Convert.ToInt32(ar["depart_id"]);
                }
                item.diff_count = 1;

                
                #endregion

                bool isAdd = true;
                //如果设置了待选人员，则启用人员id过滤
                if (personIds != null && personIds.Length > 0)
                {
                    if (!personIds.Contains(item.person_id))
                    {
                        isAdd = false;
                    }
                }
                
                //如果设置了待选部门，则部门人员id过滤
                if (departIds != null && departIds.Length > 0)
                {
                    if (!departIds.Contains(item.report_depart_id))
                    {
                        isAdd = false;
                    }
                }
                if (isAdd)
                {
                    query.Add(item);
                }
               
            }
            return query;
        }


        /// <summary>
        /// 查询某时间段的
        /// 班次班中餐记录
        /// 其实就是已完成班中餐记录
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ReportRecordInfoOnDepart_ZhouYuanShan> GetLunchRecordOnClassOrder(DateTime beginTime, DateTime endTime,int[] classOrderIDs, int[] departIds)
        {
            List<ReportRecordInfoOnDepart_ZhouYuanShan> query = new List<ReportRecordInfoOnDepart_ZhouYuanShan>();


            string sql = string.Format(
                    @"select report_record_ids,depart_id,depart_name, report_day, report_class_order_id,class_order_name,attend_sign,
                    attend_person_ids,attend_person_names,diff_person_ids,diff_person_names

                     from completed_lunch_record
                     where report_day between '{0}' and '{1}'
                     order by convert_to(depart_name,  E'GBK'),report_day,convert_to(class_order_name,  E'GBK')
                    ", beginTime, endTime);

            DataTable dt = DbAccess.POSTGRESQL.Select(sql, "");

            if (dt == null || dt.Rows == null)
            {
                return query;
            }
            foreach (DataRow ar in dt.Rows)
            {
                ReportRecordInfoOnDepart_ZhouYuanShan item = new ReportRecordInfoOnDepart_ZhouYuanShan();

                #region     数据填充
                if (ar["depart_name"] != DBNull.Value)
                {
                    item.depart_name = Convert.ToString(ar["depart_name"]);
                }
                if (ar["depart_id"] != DBNull.Value)
                {
                    item.depart_id = Convert.ToInt32(ar["depart_id"]);
                }
                if (ar["report_day"] != DBNull.Value)
                {
                    item.attend_day = Convert.ToDateTime(ar["report_day"]);
                }
                if (ar["report_class_order_id"] != DBNull.Value)
                {
                    item.class_order_id = Convert.ToInt32(ar["report_class_order_id"]);
                }
                if (ar["attend_sign"] != DBNull.Value)
                {
                    item.attend_sign = Convert.ToString(ar["attend_sign"]);
                }
                if (ar["class_order_name"] != DBNull.Value)
                {
                    item.class_order_name = Convert.ToString(ar["class_order_name"]);
                }
                if (ar["attend_person_ids"] != DBNull.Value)
                {
                    item.attend_person_ids = StringArrayToIntArray((string[])(ar["attend_person_ids"]));
                }
                if (ar["attend_person_names"] != DBNull.Value)
                {
                    item.attend_person_names = (string[])(ar["attend_person_names"]);
                }
                if (ar["diff_person_ids"] != DBNull.Value)
                {
                    item.diff_person_ids = StringArrayToIntArray((string[])(ar["diff_person_ids"]));
                }
                if (ar["diff_person_names"] != DBNull.Value)
                {
                    item.diff_person_names = (string[])(ar["diff_person_names"]);
                }
                if (ar["report_record_ids"] != DBNull.Value)
                {
                    item.report_record_ids = (int[])(ar["report_record_ids"]);
                }


                item.attend_person_count = item.attend_person_names.Length;
                item.reported_count = item.attend_person_names.Length + item.diff_person_names.Length;

                item.state = "已完成";

                #endregion

                bool isAdd = true;
                //部门过滤
                if (departIds != null && departIds.Length > 0)
                {
                    //部门过滤
                    if (!departIds.Contains(item.depart_id))
                    {
                        isAdd = false;
                    }
                }
                else
                {
                    isAdd = false;
                }
                //班次过滤
                if (classOrderIDs != null && classOrderIDs.Length > 0)
                {
                    //班次过滤
                    if (!classOrderIDs.Contains(item.class_order_id))
                    {
                        isAdd = false;
                    }
                }
                else
                {
                    isAdd = false;
                }

                if (isAdd)
                {
                    query.Add(item);
                }

            }
            return query;
        }


        #endregion

    }
}