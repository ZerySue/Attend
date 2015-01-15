/*************************************************************************
** 文件名:   DomainServiceIriskingAttend_Public.cs
** 主要类:   DomainServiceIriskingAttend
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2014-3-23
** 修改人:   
** 日  期:
** 描  述:   DomainServiceIriskingAttend类,后台操作数据库
**
** 版  本:   1.0.0
** 备  注:  命名及代码编写遵守C#编码规范
**
*************************************************************************/
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
using System.Security.Cryptography;
using ServerCommunicationLib;
using IriskingAttend.Web.Manager;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Web;

namespace IriskingAttend.Web
{
    // TODO: 创建包含应用程序逻辑的方法。    
    public partial class DomainServiceIriskingAttend : DomainService
    {
        public IEnumerable<UserPersonInfo> GetUserPersonInfoBySql(string condition, string orderBy)
        {
            List<UserPersonInfo> query = new List<UserPersonInfo>();

            if( orderBy == null || orderBy == "" )
            {
                orderBy = @"convert_to(depart_name,  E'GBK'), convert_to(name,  E'GBK')";
            }

            string sql = string.Format(@"SELECT person_id, work_sn, name, p.depart_id, depart_name, 
                    pri.principal_name, ptype.principal_type_name,ptype.principal_type_order
                    FROM person_base p LEFT JOIN  depart d on d.depart_id = p.depart_id  
                    left join principal pri on pri.principal_id = p.principal_id
                    left join principal_type ptype on ptype.principal_type_id = pri.principal_type_id 
                    where p.delete_time is null
                    {0} order by {1}", condition, orderBy);   

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
                if (ar["depart_name"] != DBNull.Value)
                {
                    item.depart_name = Convert.ToString(ar["depart_name"]);
                }

                if (ar["principal_name"] != DBNull.Value)
                {
                    item.principal_name = Convert.ToString(ar["principal_name"]);
                }
                #endregion

                query.Add(item);
            }
            return query;
        }
    }
}


