/*************************************************************************
** 文件名:   DomainServiceIriskingAttend_Principal.cs
** 主要类:   DomainServiceIriskingAttend
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   wz
** 日  期:   2013-9-10
** 修改人:   
** 日  期:   
** 描  述:   职务管理的域服务
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
        /// 获取所有职务信息
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PrincipalInfo> GetPrincipals()
        {
            List<PrincipalInfo> query = new List<PrincipalInfo>();
            string sql_all = @"select principal_id,principal_name,p.principal_type_id,principal_type_name, principal_type_order, p.memo
                from principal as p
                left join principal_type as pt on pt.principal_type_id = p.principal_type_id
                where p.delete_info is null
                order by convert_to(p.principal_name,  E'GBK');";
            DataTable dt = DbAccess.POSTGRESQL.Select(sql_all, "principal");
           
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }

            foreach (DataRow ar in dt.Rows)
            {
                PrincipalInfo principalInfo = new PrincipalInfo();

                #region 数据填充
                principalInfo.principal_id = -1;
                if (ar["principal_id"] != DBNull.Value)
                {
                    principalInfo.principal_id = Convert.ToInt32(ar["principal_id"]);
                }
                principalInfo.principal_name = "";
                if (ar["principal_name"] != DBNull.Value)
                {
                    principalInfo.principal_name = Convert.ToString(ar["principal_name"]);
                }
                principalInfo.memo = "";
                if (ar["memo"] != DBNull.Value)
                {
                    principalInfo.memo = Convert.ToString(ar["memo"]);
                }
                principalInfo.principal_type_id = 0;
                if (ar["principal_type_id"] != DBNull.Value)
                {
                    principalInfo.principal_type_id = Convert.ToInt32(ar["principal_type_id"]);
                }
                principalInfo.principal_type_name = "";
                if (ar["principal_type_name"] != DBNull.Value)
                {
                    principalInfo.principal_type_name = Convert.ToString(ar["principal_type_name"]);
                }
                principalInfo.principal_type_order = 0;
                if (ar["principal_type_order"] != DBNull.Value)
                {
                    principalInfo.principal_type_order = Convert.ToInt32(ar["principal_type_order"]);
                }
                #endregion

                query.Add(principalInfo);

            }
            return query;
        }

        /// <summary>
        /// 获取职务类型
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PrincipalTypeInfo> GetPrincipalTypes()
        {
            List<PrincipalTypeInfo> query = new List<PrincipalTypeInfo>();
            string sql_all = @"select * from principal_type where delete_info is null order by principal_type_order desc";
            DataTable dt = DbAccess.POSTGRESQL.Select(sql_all, "principal");

            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }

            foreach (DataRow ar in dt.Rows)
            {
                PrincipalTypeInfo temp = new PrincipalTypeInfo();

                #region 数据填充
                temp.principal_type_id = 0;
                if (ar["principal_type_id"] != DBNull.Value)
                {
                    temp.principal_type_id = Convert.ToInt32(ar["principal_type_id"]);
                }
                temp.principal_type_name = "";
                if (ar["principal_type_name"] != DBNull.Value)
                {
                    temp.principal_type_name = Convert.ToString(ar["principal_type_name"]);
                }
                temp.memo = "";
                if (ar["memo"] != DBNull.Value)
                {
                    temp.memo = Convert.ToString(ar["memo"]);
                }
                temp.principal_type_order = 0;
                if (ar["principal_type_order"] != DBNull.Value)
                {
                    temp.principal_type_order = Convert.ToInt32(ar["principal_type_order"]);
                }
                #endregion

                query.Add(temp);

            }
            return query;
        }


        #endregion

        #region 添加函数

        /// <summary>
        /// 添加新职务
        /// </summary>
        /// <returns></returns>
        [Invoke]
        public OptionInfo AddPrincipal(PrincipalInfo principalInfo)
        {
            OptionInfo res = new OptionInfo();

            string sql = string.Format(@"INSERT INTO principal(principal_name, memo,principal_type_id)
                    VALUES ('{0}', '{1}', {2});", principalInfo.principal_name, principalInfo.memo,
                                                  principalInfo.principal_type_id);

            //执行插入语句
            if (DbAccess.POSTGRESQL.ExecuteSql(sql) <= 0)
            {
                res.isSuccess = false;
                res.option_info = "执行sql失败 :" + sql;
                return res;
            }

            //通知考勤服务
            if (ServerComLib.NoticeTableChange(strIP, Port, new string[] { "principal" }, 1) != 0)
            {
                res.isNotifySuccess = false;
                res.option_info = "添加成功，但通知后台失败，请检查后台！";
                return res;
            }
            res.option_info = "添加职务成功！";

            return res;
        }

        /// <summary>
        /// 添加新职务类型
        /// </summary>
        /// <returns></returns>
        [Invoke]
        public OptionInfo AddPrincipalType(PrincipalTypeInfo principalTypeInfo)
        {
            OptionInfo res = new OptionInfo();

            string sql = string.Format(@"INSERT INTO principal_type (principal_type_name, memo, principal_type_order)
                    VALUES ('{0}', '{1}', {2});", principalTypeInfo.principal_type_name, principalTypeInfo.memo,
                                           principalTypeInfo.principal_type_order);

            //执行插入语句
            if (DbAccess.POSTGRESQL.ExecuteSql(sql) <= 0)
            {
                res.isSuccess = false;
                res.option_info = "执行sql失败 :" + sql;
                return res;
            }

            //通知考勤服务
            if (ServerComLib.NoticeTableChange(strIP, Port, new string[] { "principal" }, 1) != 0)
            {
                res.isNotifySuccess = false;
                res.option_info = "添加成功，但通知后台失败，请检查后台！";
                return res;
            }
            res.option_info = "添加职务类型成功！";

            return res;
        }
      
        #endregion

        #region 修改函数

        /// <summary>
        /// 修改职务
        /// </summary>
        /// <returns></returns>
        [Invoke]
        public OptionInfo ModifyPrincipal(PrincipalInfo principalInfo)
        {

            OptionInfo res = new OptionInfo();
            string sql;
            if (principalInfo.principal_type_id > 0)
            {
                sql = string.Format(@"UPDATE principal
                Set principal_name = '{0}' , memo= '{1}', principal_type_id = {2}
                WHERE principal_id = {3};", principalInfo.principal_name
                        , principalInfo.memo, principalInfo.principal_type_id, principalInfo.principal_id);

            }
            else
            {
                sql = string.Format(@"UPDATE principal
                Set principal_name = '{0}' , memo= '{1}', principal_type_id = null
                WHERE principal_id = {2};", principalInfo.principal_name
                     , principalInfo.memo,  principalInfo.principal_id);

            }

            //执行sql语句
            if (DbAccess.POSTGRESQL.ExecuteSql(sql) <= 0)
            {
                res.isSuccess = false;
                res.option_info = "执行sql失败 :" + sql;
                return res;
            }

            //通知考勤服务
            if (ServerComLib.NoticeTableChange(strIP, Port, new string[] { "principal" }, 1) != 0)
            {
                res.isNotifySuccess = false;
                res.option_info = "修改成功，但通知后台失败，请检查后台！";
                return res;
            }
            
            res.option_info = "修改职务成功！";
            return res;
        }

        /// <summary>
        /// 修改职务类型
        /// </summary>
        /// <returns></returns>
        [Invoke]
        public OptionInfo ModifyPrincipalType(PrincipalTypeInfo info)
        {

            OptionInfo res = new OptionInfo();
            string sql = string.Format(@"UPDATE principal_type
                Set principal_type_name = '{0}' ,principal_type_order={1},  memo= '{2}'
                WHERE principal_type_id = {3};", info.principal_type_name, 
                                               info.principal_type_order,
                                          info.memo, info.principal_type_id);

            //执行sql语句
            if (DbAccess.POSTGRESQL.ExecuteSql(sql) <= 0)
            {
                res.isSuccess = false;
                res.option_info = "执行sql失败 :" + sql;
                return res;
            }

            //通知考勤服务
            if (ServerComLib.NoticeTableChange(strIP, Port, new string[] { "principal" }, 1) != 0)
            {
                res.isNotifySuccess = false;
                res.option_info = "修改成功，但通知后台失败，请检查后台！";
                return res;
            }

            res.option_info = "修改职务类型成功！";
            return res;
        }
    
        #endregion

        #region 删除函数

        /// <summary>
        /// 删除职务
        /// </summary>
        /// <param name="ids">要删除的职务id数组</param>
        /// <returns></returns>
        [Invoke]
        public OptionInfo DeletePrincipal(int[] principalIds)
        {
            OptionInfo res = new OptionInfo();
            foreach (var item in principalIds)
            {
                string sql = string.Format(@"UPDATE  principal
                    SET delete_info = -1
                    WHERE principal_id = {0};
                    UPDATE person_base
                    SET principal_id = null
                    WHERE principal_id = {0};
                ", item);
                //执行sql语句
                if (DbAccess.POSTGRESQL.ExecuteSql(sql) <= 0)
                {
                    res.isSuccess = false;
                    res.option_info = "执行sql失败 :" + sql;
                    return res;
                }
            }
            

            //通知考勤服务
            if (ServerComLib.NoticeTableChange(strIP, Port, new string[] { "principal" }, 1) != 0)
            {
                res.isNotifySuccess = false;
                res.option_info = "删除成功，但通知后台失败，请检查后台！";
                return res;
            }

            res.option_info = string.Format("删除{0}个职务成功！", principalIds.Length);
            return res;

        }

        /// <summary>
        /// 删除职务类型
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [Invoke]
        public OptionInfo DeletePrincipalType(int[] ids)
        {
            OptionInfo res = new OptionInfo();
            if (ids.Length == 0) return res;
            StringBuilder sb = new StringBuilder();
            foreach (var item in ids)
            {
                sb.Append(item + ",");
            }
            sb.Remove(sb.Length - 1, 1);

            string sql = string.Format(@"UPDATE  principal_type
                            SET delete_info = -1
                            WHERE principal_type_id in ({0});
                            UPDATE principal
                            SET principal_type_id = null
                            WHERE principal_type_id in ({0});
                            ", sb.ToString());

           
            
            //执行sql语句
            if (DbAccess.POSTGRESQL.ExecuteSql(sql) <= 0)
            {
                res.isSuccess = false;
                res.option_info = "执行sql失败 :" + sql;
                return res;
            }
            res.option_info = string.Format("删除{0}个职务类型成功！", ids.Length);
            return res;
        }

        #endregion



    }



}


