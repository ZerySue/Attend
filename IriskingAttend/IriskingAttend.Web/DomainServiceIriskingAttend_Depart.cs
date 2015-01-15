/*************************************************************************
** 文件名:   DomainServiceIriskingAttend_Depart.cs
** 主要类:   DomainServiceIriskingAttend
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   wz
** 日  期:   2013-4-9
** 修改人:   wz 代码优化
** 日  期:   2013-7-24
** 描  述:   部门管理的域服务
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
    using System.Threading;
    using System.Text;
    using System.Windows.Forms;
    using ServerCommunicationLib;


    // TODO: 创建包含应用程序逻辑的方法。
    public partial class DomainServiceIriskingAttend
    {

        #region 查询函数
        /// <summary>
        /// 查询子部门
        /// </summary>
        /// <param name="parentDepartID"></param>
        /// <returns></returns>
        public IEnumerable<UserDepartInfo> GetChildDepart(string parentDepartID)
        {
            List<UserDepartInfo> query = new List<UserDepartInfo>();
            DataTable dt = DbAccess.POSTGRESQL.Select(string.Format(@"SELECT depart_id, depart_sn, depart_name
                  FROM depart 
                  where parent_depart_id = {0} and delete_time is null ;", parentDepartID), "childdepart");
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }

            foreach (DataRow ar in dt.Rows)
            {
                UserDepartInfo UserDepartInfo = new UserDepartInfo();
                //如果可为空需要判断   if(ar["attend_record_id"].ToString() != "")
                //或者 if(ar["attend_record_id"] != DBNull.Value)
                #region         数据填充
                if (ar["depart_id"] != DBNull.Value)
                {
                    UserDepartInfo.depart_id = Convert.ToInt32(ar["depart_id"]);
                }
                if (ar["depart_name"] != DBNull.Value)
                {
                    UserDepartInfo.depart_name = Convert.ToString(ar["depart_name"]);
                }
                if (ar["depart_sn"] != DBNull.Value)
                {
                    UserDepartInfo.depart_sn = Convert.ToString(ar["depart_sn"]);
                }



                #endregion
                UserDepartInfo.index = query.Count + 1;
                query.Add(UserDepartInfo);
            }
            return query;
        }

        /// <summary>
        /// 查询可选的子部门
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UserDepartInfo> GetSupportableChildDepart(int depart_ID, int parent_depart_ID)
        {
            List<UserDepartInfo> query = new List<UserDepartInfo>();
            string select_childdepaet = string.Format(@"SELECT depart_id, depart_sn, depart_name
                      FROM depart 
                      where delete_time is null and depart_id != {0} and depart_id != {1} and 
                      (parent_depart_id != {2} or parent_depart_id is null) order by convert_to(depart_name,  E'GBK')",
                      depart_ID, parent_depart_ID, depart_ID);
            DataTable dt = DbAccess.POSTGRESQL.Select(select_childdepaet, "SupportableChildDepart");
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }

            foreach (DataRow ar in dt.Rows)
            {
                UserDepartInfo UserDepartInfo = new UserDepartInfo();
                //如果可为空需要判断   if(ar["attend_record_id"].ToString() != "")
                //或者 if(ar["attend_record_id"] != DBNull.Value)
                #region         数据填充
                if (ar["depart_id"] != DBNull.Value)
                {
                    UserDepartInfo.depart_id = Convert.ToInt32(ar["depart_id"]);
                }
                if (ar["depart_name"] != DBNull.Value)
                {
                    UserDepartInfo.depart_name = Convert.ToString(ar["depart_name"]);
                }
                if (ar["depart_sn"] != DBNull.Value)
                {
                    UserDepartInfo.depart_sn = Convert.ToString(ar["depart_sn"]);
                }
                #endregion
                UserDepartInfo.index = query.Count + 1;
                query.Add(UserDepartInfo);
            }
            return query;
        }

        
        /// <summary>
        /// 获取部门
        /// wz
        /// </summary>
        /// <returns>班次类型信息</returns>
        public IEnumerable<UserDepartInfo> GetDepartsInfo()
        {
            List<UserDepartInfo> query = new List<UserDepartInfo>();
            string select_departs = string.Format(@"SELECT d1.depart_id, d1.company_sn, d1.depart_sn, d1.depart_name, 
                   d1.parent_depart_id, d2.depart_name as parent_depart_name, d1.depart_auth, d1.depart_director, 
                   d1.depart_function_id, d1.depart_work_place_id, 
                   d1.contact_phone, d1.delete_time, d1.create_time, d1.update_time, d1.memo
                  FROM depart as d1 
                  left join depart as d2 on d1.parent_depart_id = d2.depart_id
                  where d1.delete_time is null order by convert_to(d1.depart_name,  E'GBK');");
            DataTable dt = DbAccess.POSTGRESQL.Select(select_departs, "depart");

            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }

            foreach (DataRow ar in dt.Rows)
            {
                UserDepartInfo UserDepartInfo = new UserDepartInfo();
                //如果可为空需要判断   if(ar["attend_record_id"].ToString() != "")
                //或者 if(ar["attend_record_id"] != DBNull.Value)
                #region         数据填充
                if (ar["depart_id"] != DBNull.Value)
                {
                    UserDepartInfo.depart_id = Convert.ToInt32(ar["depart_id"]);
                }
                if (ar["depart_name"] != DBNull.Value)
                {
                    UserDepartInfo.depart_name = Convert.ToString(ar["depart_name"]);
                }
                if (ar["depart_sn"] != DBNull.Value)
                {
                    UserDepartInfo.depart_sn = Convert.ToString(ar["depart_sn"]);
                }
                if (ar["parent_depart_name"] != DBNull.Value)
                {
                    UserDepartInfo.parent_depart_name = Convert.ToString(ar["parent_depart_name"]);
                }
                if (ar["parent_depart_id"] != DBNull.Value)
                {
                    UserDepartInfo.parent_depart_id = Convert.ToInt32(ar["parent_depart_id"]);
                }
                if (ar["contact_phone"] != DBNull.Value)
                {
                    UserDepartInfo.contact_phone = Convert.ToString(ar["contact_phone"]);
                }
                if (ar["memo"] != DBNull.Value)
                {
                    UserDepartInfo.memo = Convert.ToString(ar["memo"]);
                }

                #endregion
                UserDepartInfo.index = query.Count + 1;
                query.Add(UserDepartInfo);
            }
            return query;
        }
        #endregion

        #region 添加函数
        /// <summary>
        /// 添加新部门
        /// </summary>
        /// <param name="departName"></param>
        /// <param name="departSn"></param>
        /// <param name="parentDepartId"></param>
        /// <param name="phone"></param>
        /// <param name="memo"></param>
        /// <returns></returns>
        public OptionInfo AddDepart(string departName,
            string departSn, string parentDepartId,
            string phone, string memo)
        {
            DateTime JudgeDate = DateTime.Now;//用于取得新添加部门depart_id，用日期和名称联合去判断 By cty
            OptionInfo res = new OptionInfo();
       
            string sql_insert = string.Format(@"INSERT INTO depart(
                depart_sn, depart_name, parent_depart_id, 
                contact_phone,  memo,create_time)
                VALUES ({0}, {1}, {2}, {3}, {4},'{5}');", departSn, departName, parentDepartId, phone, memo, DateTime.Now);
            if (DbAccess.POSTGRESQL.ExecuteSql(sql_insert) <= 0)
            {
                res.isSuccess = false;
                res.option_info = "执行sql失败： " + sql_insert;
                return res;
            }
          

            //取得depart_id，用于通知后台 by cty
            int depart_id = -1;
            string sql_select = string.Format(@"SELECT depart_id
                      FROM depart
                      where depart_name = {0} and  create_time>= '{1}'", departName, JudgeDate);
            DataTable m_dt = DbAccess.POSTGRESQL.Select(sql_select, "");
            if (m_dt != null && m_dt.Rows.Count > 0 && m_dt.Rows[0].ItemArray.Length > 0)
            {
                depart_id = (int)m_dt.Rows[0]["depart_id"];
                res.tag = depart_id;
                for (int i = 0; i < m_dt.Rows.Count; i++)
                {
                    if ((int)m_dt.Rows[i]["depart_id"] > depart_id)
                    {
                        depart_id = (int)m_dt.Rows[i]["depart_id"];
                        res.tag = depart_id;
                    }
                }
            }
            else
            {
                res.isSuccess = false;
                res.option_info = "执行Sql,结构depart_id不存在: " + sql_select;
                return res;
            }
            //通知后台已经添加了部门 by cty
            DataRefChangeData addclasstypedt = new DataRefChangeData();
            addclasstypedt.type = 0;
            addclasstypedt.typeTable = 1;
            addclasstypedt.len = 1;
            int[] personKeyID = { Convert.ToInt32(depart_id) };
            if (ServerComLib.DataRefChange(strIP, Port, ref addclasstypedt, personKeyID)!=0)
            {
                res.isNotifySuccess = false;
                res.option_info = "添加成功，但通知后台失败，请检查后台！";
            }
            else
            {
                res.isSuccess = true;
                res.option_info = "添加部门成功";
            }
            return res;
        }
        #endregion

        #region 修改函数

        /// <summary>
        /// 删除父部门和子部门的关系
        /// 父部门和子部门都是已存在的部门
        /// </summary>
        /// <param name="parentDepartId"></param>
        /// <param name="childDepartIDs"></param>
        /// <returns></returns>
        public OptionInfo DeleteChildDepart(string parentDepartId, string[] childDepartIDs)
        {
            OptionInfo res = new OptionInfo();
            for (int i = 0; i < childDepartIDs.Length; i++)
            {
                string sqlAdd = string.Format(@"UPDATE depart
                  SET parent_depart_id = null
                 WHERE depart_id = {0} and parent_depart_id = {1};", childDepartIDs[i], parentDepartId);
                if (DbAccess.POSTGRESQL.ExecuteSql(sqlAdd) <= 0)
                {
                    res.isSuccess = false;
                    res.option_info = "执行sql失败: " + sqlAdd;
                    return res;
                }
            }
            res.isSuccess = true;
            res.option_info = "删除子部门成功";

            return res;
        }

        /// <summary>
        /// 往父部门里添加子部门
        /// 父部门和子部门都是已存在的部门
        /// </summary>
        /// <param name="parentDepartId"></param>
        /// <param name="childDepartIDs"></param>
        /// <returns></returns>
        public OptionInfo AddChildDepart(string parentDepartId, string[] childDepartIDs)
        {
            OptionInfo res = new OptionInfo();
            for (int i = 0; i < childDepartIDs.Length; i++)
            {
                string sqlAdd = string.Format(@"UPDATE depart
                  SET parent_depart_id = '{0}'
                 WHERE depart_id = {1};", parentDepartId, childDepartIDs[i]);
                if (DbAccess.POSTGRESQL.ExecuteSql(sqlAdd) <= 0)
                {
                    res.isSuccess = false;
                    res.option_info = "执行sql失败: " + sqlAdd;
                    return res;
                }
            }

            res.isSuccess = true;
            res.option_info = "添加子部门成功";

            return res;
        }
        /// <summary>
        /// 修改部门信息
        /// </summary>
        /// <param name="departId"></param>
        /// <param name="departName"></param>
        /// <param name="departSn"></param>
        /// <param name="parentDepartId"></param>
        /// <param name="phone"></param>
        /// <param name="memo"></param>
        /// <returns></returns>
        [Invoke]
        public OptionInfo ModifyDepart(string departId, string departName,
            string departSn, string parentDepartId,
            string phone, string memo)
        {
            OptionInfo res = new OptionInfo();
            string sql_update = string.Format(@"UPDATE depart
                SET depart_sn={0}, depart_name={1}, parent_depart_id={2}, 
                contact_phone={3},memo={4}
                WHERE depart_id={5};", departSn, departName, parentDepartId, phone, memo, departId);
            if (DbAccess.POSTGRESQL.ExecuteSql(sql_update) <= 0)
            {
                res.isSuccess = false;
                res.option_info = "执行sql失败: " + sql_update;
                return res;
            }


            //通知后台已经添加了部门 by cty
            DataRefChangeData addclasstypedt = new DataRefChangeData();
            addclasstypedt.type = 2;
            addclasstypedt.typeTable = 1;
            addclasstypedt.len = 1;
            int[] personKeyID = { Convert.ToInt32(departId) };
            if (ServerComLib.DataRefChange(strIP, Port, ref addclasstypedt, personKeyID) != 0)
            {
                res.isNotifySuccess = false;
                res.option_info = "修改成功，但通知后台失败，请检查后台";
            }
            else
            {
                res.isSuccess = true;
                res.option_info = "修改部门成功";
            }

            return res;
        }
        #endregion

        #region 删除函数


        /// <summary>
        /// 删除部门
        /// </summary>
        /// <param name="departIds"></param>
        /// <returns></returns>
        public OptionInfo DeleteDepart(string[] departIds)
        {
            //bug2838 by cty
            OptionInfo res = new OptionInfo();
            for (int j = 0; j < departIds.Length; j++)// 确定部门下是否有人员信息 by cty
            {
                string sqlD = string.Format(@"select * from person_base WHERE depart_id = {0} and delete_time is null ;", departIds[j]);
                DataTable dt = DbAccess.POSTGRESQL.Select(sqlD, "a");
                if (dt.Rows.Count > 0) //如果在人员表中查询到有部门 表示该部门下有人员，则不能删除该部门，该处提示 by cty
                {
                    res.isSuccess = false;
                    res.option_info = "您要删除的部门中还存在人员信息，请确定删除相应信息之后再删除此部门信息！";
                    return res;
                }
            }

            for (int j = 0; j < departIds.Length; j++)// 确定部门是否是父部门并且子部门是否已被删除（删除时打标记） by cty
            {
                string sqlD = string.Format(@"select * from depart WHERE parent_depart_id = {0} and delete_time is null;", departIds[j]);
                DataTable dt = DbAccess.POSTGRESQL.Select(sqlD, "b");
                if (dt.Rows.Count > 0)  //检查该部门是否是父部门 by cty
                {
                    res.isSuccess = false;
                    res.option_info = "您要删除的部门中还存在没有删除的子部门，请确定删除相应信息之后再删除此部门信息！";
                    return res;
                }
            }

            // OptionInfo res = new OptionInfo();
            for (int i = 0; i < departIds.Length; i++)
            {
                string sqlD = string.Format(@"UPDATE depart
                  SET delete_time = '{0}'
                 WHERE depart_id = {1};", System.DateTime.Now, departIds[i]);
                if (DbAccess.POSTGRESQL.ExecuteSql(sqlD) <= 0)
                {
                    res.isSuccess = false;
                    res.option_info = "执行sql失败: " + sqlD;
                    return res;
                }
            }

            res.isSuccess = true;
            res.option_info = "删除部门成功";

            //通知后台已经删除了班制 by cty
            DataRefChangeData addclasstypedt = new DataRefChangeData();
            addclasstypedt.type = 1;
            addclasstypedt.typeTable = 1;
            addclasstypedt.len = 1;
            int[] personKeyID = new int[departIds.Length];
            for (int i = 0; i < departIds.Length; i++)
            {
                personKeyID[i] = Convert.ToInt32(departIds[i]);
            }
            if (ServerComLib.DataRefChange(strIP, Port, ref addclasstypedt, personKeyID)!=0)
            {
                res.isNotifySuccess = false;
                res.option_info = "删除成功，但通知后台失败，请检查后台！";
            }
            else
            {
                res.isSuccess = true;
                res.option_info = "删除部门成功";
            }
            return res;
        }
        #endregion

    }



}


