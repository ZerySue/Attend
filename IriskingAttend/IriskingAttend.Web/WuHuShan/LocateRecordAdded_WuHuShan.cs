/*************************************************************************
** 文件名:   LocateRecordAdded_WuHuShan.cs
×× 主要类:   LocateRecordAdded_WuHuShan
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2014-5-21
** 修改人:  
** 日  期:       
** 描  述:   五虎山单独定制域服务  根据虹膜记录添加的定位记录添加、删除、查询
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
using System.IO;
using System.Runtime.Serialization;

namespace IriskingAttend.Web
{
     // TODO: 创建包含应用程序逻辑的方法。

    public partial class DomainServiceIriskingAttend : DomainService
    {

        [Update]
        public void TestLocateRecordAdded_WuhuShan(LocateRecordAddedEntity t)
        {
        }

        /// <summary>
        /// 获取所有定位记录
        /// </summary>
        /// <returns>获取的定位记录列表</returns>
        public IEnumerable<LocateRecordAddedEntity> GetLocateRecord(DateTime beginTime, DateTime endTime, int[] departIds, string personName, string workSn)
        {
            string querySQL =string.Format( @"select whs_manual_locate.*, p.name, p.work_sn, p.depart_name  from whs_manual_locate 
                               left join person p on p.person_id = whs_manual_locate.pid
                               where fdate between '{0}' and '{1}'", beginTime,endTime);

            //部门过滤
            if (departIds != null && departIds.Length > 0)
            {
                StringBuilder sqlDepartWhere = new StringBuilder("and p.depart_id in (");
                foreach (var item in departIds)
                {
                    sqlDepartWhere.Append(item + ",");
                }
                sqlDepartWhere.Remove(sqlDepartWhere.Length - 1, 1);
                sqlDepartWhere.Append(")");
                querySQL = querySQL + sqlDepartWhere;
            }
            //人名过滤
            if (personName != null && personName != "")
            {
                querySQL = querySQL + String.Format("and p.name like '%{0}%'", personName);
            }

            //工号过滤
            if (workSn != null && workSn != "")
            {
                querySQL = querySQL + String.Format("and p.work_sn = '{0}'", workSn);
            }
            querySQL += " order by convert_to(p.name,  E'GBK'), ffirsttime";

            List<LocateRecordAddedEntity> recordList = new List<LocateRecordAddedEntity>();
            DataTable dt = DbAccess.POSTGRESQL.Select(querySQL, "whs_manual_locate");
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }

            try
            {
                int index = 1;
                foreach (DataRow ar in dt.Rows)
                {
                    LocateRecordAddedEntity recordInfo = new LocateRecordAddedEntity();
                    recordInfo.LocateRecordID = Int32.Parse(ar["whs_manual_id"].ToString());
                    recordInfo.Index = index++;
                    recordInfo.PersonName = ar["name"].ToString();
                    recordInfo.WorkSn = ar["work_sn"].ToString();
                    recordInfo.DepartName = ar["depart_name"].ToString();

                    recordInfo.AttendDay = DateTime.Parse(ar["fdate"].ToString());
                    recordInfo.LocateInWellTime = DateTime.Parse(ar["ffirsttime"].ToString());
                    recordInfo.LocateOutWellTime = DateTime.Parse(ar["flasttime"].ToString());
                    recordList.Add(recordInfo);
                }

                return recordList;
            }
            catch 
            {
                return null;
            }
        }
       
        /// <summary>
        /// 定位记录批量删除
        /// </summary>       
        [Invoke]
        public bool BatchDeleteLocateRecord(int[] rocordIds)
        {
            string deleteSql = "DELETE FROM whs_manual_locate WHERE whs_manual_id in ( ";

            deleteSql += rocordIds[0];
            for (int index = 1; index < rocordIds.Length; index++)
            {
                deleteSql += ("," + rocordIds[index]);
            }

            deleteSql += ")";

            if (DbAccess.POSTGRESQL.Delete(deleteSql))
            {
                return true;
            }
            return false;
        }
        
        [Invoke(HasSideEffects = true)]
        public bool BatchAddLocateRecord(int[] personIds, DateTime[] inLocateTimes, DateTime[] outLocateTimes, DateTime[] attendDays)
        {
            string sqlInsert = "";
            for(int index = 0; index < personIds.Count(); index++ )
            {
                string Values = string.Format("{0}\t", personIds[index]);               
                Values += string.Format("{0}\t", inLocateTimes[index]);
                Values += string.Format("{0}\t", outLocateTimes[index]);
                Values += string.Format("{0}\n", attendDays[index]);               
                sqlInsert += Values;
            }

            byte[] tm = System.Text.Encoding.UTF8.GetBytes(sqlInsert);
            MemoryStream ms = new MemoryStream(tm);

            string tableName = "whs_manual_locate (pid,ffirsttime,flasttime,fdate) ";

            if (DbAccess.POSTGRESQL.CopyIn(tableName, ms) > 0)
            {
                return true;
            }
            return false;   
        }
    }

    /// <summary>
    /// 五虎山矿根据虹膜记录添加的定位记录实体数据类
    /// </summary>
    [DataContract]
    public class LocateRecordAddedEntity
    {
         /// <summary>
        /// id
        /// </summary>
        [DataMember]
        [Key]
        public int LocateRecordID { get; set; }

        /// <summary>
        /// 序号
        /// </summary>        
        [DataMember]
        public int Index { get; set; }

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
        /// 部门名称
        /// </summary>        
        [DataMember]
        public string DepartName { get; set; }

        ///// <summary>
        ///// 职位
        ///// </summary>        
        //public string Principal { get; set; }

        /// <summary>
        /// 定位入井时间
        /// </summary>        
        [DataMember]
        public DateTime LocateInWellTime { get; set; }

        /// <summary>
        /// 定位升井时间
        /// </summary>        
        [DataMember]
        public DateTime LocateOutWellTime { get; set; }

        /// <summary>
        /// 归属日
        /// </summary>        
        [DataMember]
        public DateTime AttendDay { get; set; }

        /// <summary>
        /// 是否被选中
        /// </summary>
        [DataMember]
        public bool isSelected
        {
            get;
            set;
        }

        public LocateRecordAddedEntity()
        {
            DepartName = "";
            WorkSn = "";
            isSelected = false;
        }
    }
}