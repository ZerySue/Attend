/*************************************************************************
** 文件名:   DomainServiceIriskingAttend_WuHuShan.cs
×× 主要类:   DomainServiceIriskingAttend_WuHuShan
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   wz
** 日  期:   2014-06-17
** 修改人:  
** 日  期:       
** 描  述:   国电单独定制域服务
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
using IriskingAttend.Web.DataModel;

namespace IriskingAttend.Web
{
     // TODO: 创建包含应用程序逻辑的方法。

    public partial class DomainServiceIriskingAttend : DomainService
    {

        /// <summary>
        /// 获取国电报表,数据来源为原始数据库识别记录表
        /// </summary>
        /// <returns></returns>
        [Invoke(HasSideEffects = true)]
        [Query]
        public IEnumerable<GuoDianReportData> GetGuoDianReport(DateTime beginTime,DateTime endTime,
            string personName,string workSn, int[] departIds)
        {
            string whereSql = "";
            if (!string.IsNullOrWhiteSpace(personName))
            {
                personName = personName.Replace('\'', '\"');
                whereSql += string.Format(" and pb.name like '%{0}%'",personName);
                
            }
            if (!string.IsNullOrWhiteSpace(workSn))
            {
                workSn = workSn.Replace('\'', '\"');
                whereSql += string.Format( " and pb.work_sn = '{0}' ",workSn);
            }
            if ((departIds != null && departIds.Length > 0))
            {
                string departStr = " (";
                foreach (var item in departIds)
                {
                    departStr += item + ",";
                }
                departStr = departStr.Remove(departStr.Length - 1, 1);
                departStr += ") ";

                whereSql += " and pb.depart_id in " + departStr;
            }

            string strSQL = string.Format(@"select pb.person_id, pb.name, pb.work_sn, pb.depart_name,  s.places ,s.recog_time_arr

                    from

                    person as pb

                    left join  

                     (select person_id,array_agg(id.place) as places ,array_agg(prb.recog_time) as recog_time_arr  

	                                 from person_recog_base as prb

	                                 left join iris_device as id on id.dev_sn = prb.device_sn
	 
	                                 where recog_time between '{0}' and '{1}'  and prb.delete_time is null

	                                 group by  prb.person_id ) as s on pb.person_id =  s.person_id

                    where pb.delete_time is null {2}
                
                    order by pb.depart_name,pb.name", beginTime, endTime, whereSql);


            DataTable dt = DbAccess.POSTGRESQL.Select(strSQL, "");


            List<GuoDianReportData> guoDianReportDatas = new List<GuoDianReportData>();
            if (dt != null && dt.Rows.Count >= 1)
            {
                int index = 0;
                foreach (DataRow ar in dt.Rows)
                {

                    GuoDianReportData item = new GuoDianReportData();

                    if (ar["person_id"] != DBNull.Value)
                    {
                        item.PersonID = Convert.ToInt32(ar["person_id"]);
                    }
                    if (ar["name"] != DBNull.Value)
                    {
                        item.PersonName = Convert.ToString(ar["name"]);
                    }
                    if (ar["work_sn"] != DBNull.Value)
                    {
                        item.WorkSn = Convert.ToString(ar["work_sn"]);
                    }
                    if (ar["depart_name"] != DBNull.Value)
                    {
                        item.CustomCol = Convert.ToString(ar["depart_name"]);
                    }
                    NpgsqlTypes.NpgsqlTimeStamp[] recogTimes = null;
                    if (ar["recog_time_arr"] != DBNull.Value)
                    {
                        recogTimes = (NpgsqlTypes.NpgsqlTimeStamp[])(ar["recog_time_arr"]);
                    }
                    string[] places = null;
                    if (ar["places"] != DBNull.Value)
                    {
                        places = (string[])(ar["places"]);
                    }

                    for (DateTime currentDay = beginTime; currentDay < endTime; currentDay = currentDay.AddDays(1))
                    {
                        int DailyState = 0;
                        //寻找当天的识别记录
                        List<RecogTimePlaceInfo> curDateTimes = GetCurrentDayRecog(recogTimes, places, currentDay);
                        //按时间排序
                        var orderdedCurDateTimes = curDateTimes.OrderBy(d => d.RecogTime);

                        //第一个为入井
                        string inWellDescription = "    ";
                        //第二个为出井
                        string outWellDescription = "    ";

                        if (orderdedCurDateTimes.Count() > 0)
                        {
                            var recogStart = orderdedCurDateTimes.ElementAt(0);
                            inWellDescription = string.Format("{0}({1})", recogStart.RecogTime.ToString("HH:mm"), recogStart.Place);
                            
                            if (orderdedCurDateTimes.Count() > 1)
                            {
                                var recogEnd = orderdedCurDateTimes.Last();
                                outWellDescription = string.Format("{0}({1})", recogEnd.RecogTime.ToString("HH:mm"), recogEnd.Place);

                                //上班时间超过8小时 算一个有效考勤
                                if (recogStart.RecogTime.AddHours(8) <= recogEnd.RecogTime)
                                {
                                    item.VaildAttendCount++;
                                    DailyState = 0; //正常考勤
                                }
                                else
                                {
                                    DailyState = 4; //时长不够
                                }
                            }
                            else
                            {
                                DailyState = 3;  //缺下班
                            }
                        }
                        else
                        {
                            DailyState = 1; //缺上班
                        }

                        item.DailyState.Add(DailyState);
                        if (string.IsNullOrWhiteSpace(inWellDescription) && string.IsNullOrWhiteSpace(outWellDescription))
                        {
                            item.DailyContent.Add("");
                        }
                        else
                        {
                            item.DailyContent.Add(inWellDescription + "|" + outWellDescription);
                        }
                    }

                    item.Index = index++;
                    guoDianReportDatas.Add(item);
                }

            }
            return guoDianReportDatas;
        }

        /// <summary>
        /// 寻找当天的识别记录
        /// </summary>
        /// <param name="source"></param>
        /// <param name="currentDay"></param>
        /// <returns></returns>
        List<RecogTimePlaceInfo> GetCurrentDayRecog(NpgsqlTypes.NpgsqlTimeStamp[] source,string[] palces, DateTime currentDay)
        {
            List<RecogTimePlaceInfo> res = new List<RecogTimePlaceInfo>();

            if (source == null || palces == null)
            {
                return res;
            }

            for (int i = 0; i < source.Length; i++)
			{
                if (source[i].Month == currentDay.Month && source[i].Day == currentDay.Day)
                {
                    RecogTimePlaceInfo info = new RecogTimePlaceInfo();
                    info.Place = palces[i];
                    info.RecogTime = DateTime.Parse(source[i].ToString());
                    res.Add(info);
                }
			}
            
            return res;
        }

        /// <summary>
        /// 获取国电报表，人员来源为导入的Excel数据，存在guo_dian_excel_data表中
        /// guo_dian_excel_info 存放表名称和自定义列名称
        /// </summary>
        /// <returns></returns>
        [Query]
        public IEnumerable<GuoDianReportData> GetGuoDianReportFromExcel(DateTime beginTime,DateTime endTime)
        {
            string Title = "";
            string customColName = "";
            string sqlExcelInfo = "select title,custom_col_name from guo_dian_excel_info limit 1";
            DataTable dtInfo = DbAccess.POSTGRESQL.Select(sqlExcelInfo, "");
            if (dtInfo != null && dtInfo.Rows.Count >= 1)
            {
                foreach (DataRow ar in dtInfo.Rows)
                {
                    if (ar["title"] != DBNull.Value)
                    {
                        Title = Convert.ToString(ar["title"]);
                    }
                    if (ar["custom_col_name"] != DBNull.Value)
                    {
                        customColName = Convert.ToString(ar["custom_col_name"]);
                    }
                }
            }

            string sqlDataInfo = string.Format(@"SELECT  gd.person_id, gd.person_name as name, gd.work_sn , gd.custom_col , s.places ,s.recog_time_arr

            FROM guo_dian_excel_data as gd
	  
            left join  (select person_id,array_agg(id.place) as places ,array_agg(prb.recog_time) as recog_time_arr  

	                 from person_recog_base as prb

	                 left join iris_device as id on id.dev_sn = prb.device_sn
 
	                 where recog_time between '{0}' and '{1}'  and prb.delete_time is null

	                 group by  prb.person_id ) as s on gd.person_id =  s.person_id

            left join person as pb on gd.person_id = pb.person_id

              where  pb.delete_time is  null

              Order by guo_dian_id
                ", beginTime, endTime);

            DataTable dt = DbAccess.POSTGRESQL.Select(sqlDataInfo, "");

            List<GuoDianReportData> guoDianReportDatas = new List<GuoDianReportData>();
            if (dt != null && dt.Rows.Count >= 1)
            {
                int index = 0;
                foreach (DataRow ar in dt.Rows)
                {
                    GuoDianReportData item = new GuoDianReportData();
                    item.Title = Title;
                    item.CustomColName = customColName;

                    if (ar["person_id"] != DBNull.Value)
                    {
                        item.PersonID = Convert.ToInt32(ar["person_id"]);
                    }
                    if (ar["name"] != DBNull.Value)
                    {
                        item.PersonName = Convert.ToString(ar["name"]);
                    }
                    if (ar["work_sn"] != DBNull.Value)
                    {
                        item.WorkSn = Convert.ToString(ar["work_sn"]);
                    }
                    if (ar["custom_col"] != DBNull.Value)
                    {
                        item.CustomCol = Convert.ToString(ar["custom_col"]);
                    }
                    NpgsqlTypes.NpgsqlTimeStamp[] recogTimes = null;
                    if (ar["recog_time_arr"] != DBNull.Value)
                    {
                        recogTimes = (NpgsqlTypes.NpgsqlTimeStamp[])(ar["recog_time_arr"]);
                    }
                    string[] places = null;
                    if (ar["places"] != DBNull.Value)
                    {
                        places = (string[])(ar["places"]);
                    }

                    for (DateTime currentDay = beginTime; currentDay < endTime; currentDay = currentDay.AddDays(1))
                    {
                        int DailyState = 0;
                        //寻找当天的识别记录
                        List<RecogTimePlaceInfo> curDateTimes = GetCurrentDayRecog(recogTimes, places, currentDay);
                        //按时间排序
                        var orderdedCurDateTimes = curDateTimes.OrderBy(d => d.RecogTime);

                        //第一个为入井
                        string inWellDescription = "              ";
                        //第二个为出井
                        string outWellDescription = "              ";

                        if (orderdedCurDateTimes.Count() > 0)
                        {
                            var recogStart = orderdedCurDateTimes.ElementAt(0);
                            inWellDescription = string.Format("{0}({1})", recogStart.RecogTime.ToString("HH:mm"), recogStart.Place);

                            if (orderdedCurDateTimes.Count() > 1)
                            {
                                var recogEnd = orderdedCurDateTimes.Last();
                                outWellDescription = string.Format("{0}({1})", recogEnd.RecogTime.ToString("HH:mm"), recogEnd.Place);

                                //上班时间超过8小时 算一个有效考勤
                                if (recogStart.RecogTime.AddHours(8) <= recogEnd.RecogTime)
                                {
                                    item.VaildAttendCount++;
                                    DailyState = 0; //正常考勤
                                }
                                else
                                {
                                    DailyState = 4; //时长不够
                                }
                            }
                            else
                            {
                                DailyState = 3;  //缺下班
                            }
                        }
                        else
                        {
                            DailyState = 1; //缺上班
                        }

                        item.DailyState.Add(DailyState);
                        if (string.IsNullOrWhiteSpace(inWellDescription) && string.IsNullOrWhiteSpace(outWellDescription))
                        {
                            item.DailyContent.Add("");
                        }
                        else
                        {
                            item.DailyContent.Add(inWellDescription + "|" + outWellDescription);
                        }
                    }
                    item.Index = index++;
                    guoDianReportDatas.Add(item);
                }
            }
            return guoDianReportDatas;
        }

    }

   
}