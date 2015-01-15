/*************************************************************************
** 文件名:   DomainServiceIriskingAttend_WorkType..cs
** 主要类:   DomainServiceIriskingAttend
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   wz
** 日  期:   2013-9-10
** 修改人:   
** 日  期:   
** 描  述:   工种管理的域服务
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
        /// 获取所有工种信息
        /// </summary>
        /// <returns></returns>
        public IEnumerable<WorkTypeInfo> GetWorkTypes()
        {
            List<WorkTypeInfo> query = new List<WorkTypeInfo>();
            string sql_all = "select * from work_type where delete_info is null order by convert_to(work_type_name,  E'GBK')";
            DataTable dt = DbAccess.POSTGRESQL.Select(sql_all, "work_type");

            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }

            foreach (DataRow ar in dt.Rows)
            {
                WorkTypeInfo workTypeInfo = new WorkTypeInfo();

                #region 数据填充
                workTypeInfo.work_type_id = -1;
                if (ar["work_type_id"] != DBNull.Value)
                {
                    workTypeInfo.work_type_id = Convert.ToInt32(ar["work_type_id"]);
                }
                workTypeInfo.work_type_name = "";
                if (ar["work_type_name"] != DBNull.Value)
                {
                    workTypeInfo.work_type_name = Convert.ToString(ar["work_type_name"]);
                }
                workTypeInfo.memo = "";
                if (ar["memo"] != DBNull.Value)
                {
                    workTypeInfo.memo = Convert.ToString(ar["memo"]);
                }

                #endregion

                query.Add(workTypeInfo);
            }

            return query;
        }

        #endregion

        #region 添加函数

        /// <summary>
        /// 添加新工种
        /// </summary>
        /// <param name="name">工种名称</param>
        /// <param name="memo">备注</param>
        /// <returns></returns>
        [Invoke]
        public OptionInfo AddWorkType(WorkTypeInfo workTypeInfo)
        {
            OptionInfo res = new OptionInfo();

            string sql = string.Format(@"INSERT INTO work_type(work_type_name, memo)
                    VALUES ('{0}', '{1}');", workTypeInfo.work_type_name, workTypeInfo.memo);

            //执行插入语句
            if (DbAccess.POSTGRESQL.ExecuteSql(sql) <= 0)
            {
                res.isSuccess = false;
                res.option_info = "执行sql失败 :" + sql;
                return res;
            }

            //通知考勤服务
            if (ServerComLib.NoticeTableChange(strIP, Port, new string[] { "work_type" }, 1) != 0)
            {
                res.isNotifySuccess = false;
                res.option_info = "添加成功，但通知后台失败，请检查后台！";
                return res;
            }
            res.option_info = "添加工种成功！";

            return res;
        }
      
        #endregion

        #region 修改函数

        /// <summary>
        /// 修改工种
        /// </summary>
        /// <param name="id">工种id</param>
        /// <param name="name">工种名称 </param>
        /// <param name="memo">工种备注 </param>
        /// <returns></returns>
        [Invoke]
        public OptionInfo ModifyWorkType(WorkTypeInfo workTypeInfo)
        {

            OptionInfo res = new OptionInfo();
            string sql = string.Format(@"UPDATE work_type
                Set work_type_name = '{0}' , memo= '{1}'
                WHERE work_type_id = {2};", workTypeInfo.work_type_name, workTypeInfo.memo,
                                          workTypeInfo.work_type_id);

            //执行sql语句
            if (DbAccess.POSTGRESQL.ExecuteSql(sql) <= 0)
            {
                res.isSuccess = false;
                res.option_info = "执行sql失败 :" + sql;
                return res;
            }

            //通知考勤服务
            if (ServerComLib.NoticeTableChange(strIP, Port, new string[] { "work_type" }, 1) != 0)
            {
                res.isNotifySuccess = false;
                res.option_info = "修改成功，但通知后台失败，请检查后台！";
                return res;
            }

            res.option_info = "修改工种成功！";
            return res;
        }

        #endregion

        #region 删除函数

        /// <summary>
        /// 删除工种
        /// </summary>
        /// <param name="ids">要删除的工种id数组</param>
        /// <returns></returns>
        [Invoke]
        public OptionInfo DeleteWorkType(int[] workTypeIds)
        {
            OptionInfo res = new OptionInfo();
            foreach (var item in workTypeIds)
            {
                string sql = string.Format(@"
                    UPDATE  work_type
                    SET delete_info = -1
                    WHERE work_type_id = {0};
                    UPDATE person_base
                    SET work_type_id = null
                    WHERE work_type_id = {0};", item);

                //执行sql语句
                if (DbAccess.POSTGRESQL.ExecuteSql(sql) <= 0)
                {
                    res.isSuccess = false;
                    res.option_info = "执行sql失败 :" + sql;
                    return res;
                }
            }


            //通知考勤服务
            if (ServerComLib.NoticeTableChange(strIP, Port, new string[] { "work_type" }, 1) != 0)
            {
                res.isNotifySuccess = false;
                res.option_info = "删除成功，但通知后台失败，请检查后台！";
                return res;
            }

            res.option_info = string.Format("删除{0}个工种成功！", workTypeIds.Length);
            return res;

        }

        #endregion

    }



}


