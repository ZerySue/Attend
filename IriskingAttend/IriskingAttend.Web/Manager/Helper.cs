/*************************************************************************
** 文件名:   Helper.cs
×× 主要类:   Helper
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   wz
** 日  期:   2013-05-29
** 修改人:   wz
** 日  期:    
 * 修改内容： 
** 描  述:   在获取数据库数据或者填写sql函数时，提供一些帮助函数
 *            
** 版  本:   1.0.0
** 备  注:  命名及代码编写遵守C#编码规范
**
 * ***********************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IriskingAttend.Web.Manager
{
    public class Helper
    {
        /// <summary>
        /// 人员信息列表查询时
        /// 组装with语句的功能函数
        /// </summary>
        /// <param name="departId"> -1 代表查找全部部门</param>
        /// <param name="childDepartMode"></param>
        /// <param name="irisRegister"></param>
        /// <returns></returns>
        public static string GetConditionSql(int departId, string childDepartMode, string irisRegister)
        {
            string QuerySqlConditon_IrisEnroll = null;
            string QuerySqlConditon_Depart = null;
            if (departId == -1)
            {
                QuerySqlConditon_Depart = @"select depart_id, parent_depart_id,depart_name, depart_sn from depart";
            }
            else
            {
                switch (childDepartMode.Trim())
                {
                    case "包含":
                        QuerySqlConditon_Depart = string.Format(@"select root.depart_id, root.parent_depart_id, root.depart_name, root.depart_sn
	                            from depart as root where root.depart_id = {0}
	                            union all
	                            select sub.depart_id, sub.parent_depart_id, sub.depart_name, sub.depart_sn
	                            from depart sub, depart_childs as super where sub.parent_depart_id = super.depart_id"
                                 , departId);
                        break;
                    case "不包含":
                        QuerySqlConditon_Depart = string.Format(@"select depart_id, parent_depart_id,depart_name, depart_sn
                                    from depart
                                    where depart_id = {0} ", departId);
                        break;
                    default:
                        return null;
                }
            }
            switch (irisRegister.Trim())
            {
                case "全部":
                    QuerySqlConditon_IrisEnroll = @"select person_id from person_base group by person_id";
                    break;
                case "已注册":
                    QuerySqlConditon_IrisEnroll = @"SELECT person_id 
	                    FROM  person_enroll_info 
	                    group by person_id";
                    break;
                case "未注册":
                    QuerySqlConditon_IrisEnroll = @"select pb.person_id
                        from person_base as pb
                        where  
                        pb.person_id not in (  
                          SELECT person_id 
                          FROM  person_enroll_info 
                          group by person_id
                         ) 
                        group by pb.person_id";
                    break;
                case "注册单眼":
                    QuerySqlConditon_IrisEnroll = @"select person_id from 
                        (select pei.person_id,count(pei.eye_flag) as eye_flag_count
                        from person_enroll_info as pei
                        group by pei.person_id) as pei
                        where pei.eye_flag_count = 1";
                    break;
                case "注册双眼":
                    QuerySqlConditon_IrisEnroll = @"select person_id from 
                    (select pei.person_id,count(pei.eye_flag) as eye_flag_count
                    from person_enroll_info as pei
                    group by pei.person_id) as pei
                    where pei.eye_flag_count = 2";
                    break;
                case "仅注册左眼":
                    QuerySqlConditon_IrisEnroll = @"select pei.person_id from
                    (
	                    select person_id from 
	                    (
		                    select pei.person_id, count(pei.eye_flag) as eye_flag_count
		                    from person_enroll_info as pei
		                    group by pei.person_id
	                    ) as pei
	                    where pei.eye_flag_count = 1
                    ) as pei, person_enroll_info
                    where pei.person_id = person_enroll_info.person_id and person_enroll_info.eye_flag =1";
                    break;
                case "仅注册右眼":
                    QuerySqlConditon_IrisEnroll = @"select pei.person_id from
                    (
	                    select person_id from 
	                    (
		                    select pei.person_id, count(pei.eye_flag) as eye_flag_count
		                    from person_enroll_info as pei
		                    group by pei.person_id
	                    ) as pei
	                    where pei.eye_flag_count = 1
                    ) as pei, person_enroll_info
                    where pei.person_id = person_enroll_info.person_id and person_enroll_info.eye_flag =2";
                    break;
                default:

                    return null;
            }
            string res = string.Format(@"with RECURSIVE 
                depart_childs(depart_id,parent_depart_id,depart_name, depart_sn) as
                ({0}),
                pei_condition (person_id) as 
                ({1}),
                depart_enhence as
		        ( select d1.depart_id as depart_id, d1.depart_name as depart_name,
		          d2.depart_name as parent_depart_name, d1.depart_sn AS depart_sn
		          from  depart_childs as d1  left join  depart as d2 on d1.parent_depart_id = d2.depart_id )",
                QuerySqlConditon_Depart, QuerySqlConditon_IrisEnroll);

            return res;
        }


        /// <summary>
        /// 计算分钟数对应的日期表示式字符串
        /// 如：
        /// [60 -> 当日 01:00]
        /// [1440 ->次日 00:00]
        /// [121 -> 当日 02:01]
        /// 如果isAdd为false，则
        /// [60 ->  01:00]
        /// </summary>
        /// <param name="minutes"></param>
        /// <param name="isAdd"></param>
        /// <returns></returns>
        public static string MinutesToDate(int minutes,bool isAdd = true)
        {
            if (minutes <0) return null;
            string day = "";
            string time ="";
            int hour ;
            int minute ;
            if (minutes < 1440)
            {
                 day = "当日";
                 hour = minutes / 60;
                 minute = minutes - hour * 60;
                
            }
            else if (minutes < 2880)
            {
                minutes = minutes - 1440;
                day = "次日";
                 hour = minutes / 60;
                 minute = minutes - hour * 60;
               
            }
            else if (minutes < 4320)
            {
                minutes = minutes - 2880;
                day = "第三日";
                hour = minutes / 60;
                minute = minutes - hour * 60;

            }
            else
            {
                day = "第" + (minutes / 1440 + 1).ToString() + "日";
                minutes = minutes % 1440;
                hour = minutes / 60;
                minute = minutes - hour * 60;

            }
            time = hour.ToString().PadLeft(2, '0') + ":" + minute.ToString().PadLeft(2, '0');
            if (isAdd)
            {
                return day + " " + time;
            }
            else
            {
                return time;
            }
          
        }

        /// <summary>
        /// 计算分钟数对应的(小时+分钟式)字符串表示
        /// </summary>
        /// <param name="minutes"></param>
        /// <returns></returns>
        public static string MinutesToTimeLenth(int minutes)
        {
            int hour = minutes / 60;
            int minute = minutes - hour * 60;
            return hour + "小时" + minute + "分钟";
        }

        /// <summary>
        /// 根据考勤偏移分钟数计算考勤归属日
        /// </summary>
        /// <param name="offMinute"></param>
        /// <returns></returns>
        public static string GetAttendBelongDay(int offMinute)
        {
            if (offMinute <0)
            {
                return "未知";
            }
            if (offMinute < 1440)
            {
                return "当天";
            }
            else if (offMinute < 2880)
            {
                return "次日";
            }
            else if (offMinute < 4320)
            {
                return "第三日";
            }
            else
            {
                return "第" + (offMinute/1440 + 1).ToString() + "日";
            }
        }

        /// <summary>
        /// 计算浮点类型工数
        /// </summary>
        /// <param name="workcnt"></param>
        /// <returns></returns>
        public static string GetWorkCnt(int workcnt)
        {
            return ((float)workcnt / 10).ToString("0.0");
        }

        /// <summary>
        /// 为了方便后台sql语句的组装，提供一个字符串转换函数 
        /// by wz
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static string ToString(object o)
        {
            if (o == null) return "null";
            if (o.GetType().Equals(typeof(string)))
            {
                return "'" + o.ToString() + "'";
            }
            else if (o.GetType().Equals(typeof(int)) || o.GetType().Equals(typeof(long)))
            {
                if ((int)o == -1)
                {
                    return "null";
                }
                else
                {
                    return o.ToString();
                }

            }
            else if (o.GetType().Equals(typeof(DateTime)))
            {
                if ((DateTime)o == DateTime.MinValue)
                {
                    return "null";
                }
                else
                {
                    return "'" + o.ToString() + "'";
                }
            }
            return o.ToString();
        }

    }
}