/*************************************************************************
** 文件名:   attend_for_leave.cs
** 主要类:   attend_for_leave
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   lzc
** 日  期:   2013-4-23
** 修改人:   
** 日  期:
** 描  述:    定义 请假 实体类 
**
** 版  本:   1.0.0
** 备  注:  命名及代码编写遵守C#编码规范
**
 * ***********************************************************************/

namespace Irisking.Web.DataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.ServiceModel.DomainServices.Hosting;
    using System.ServiceModel.DomainServices.Server;
    using System.Runtime.Serialization;

    /// <summary>
    /// 添加、更新需要
    /// </summary>
    [DataContract]
    public class attend_for_leave
    {
        [DataMember]
        [Key]
        public int attend_for_leave_id {get;set;}

        /// <summary>
        /// 人员ID
        /// </summary>
        [DataMember]
        public int person_id { get; set; }

        /// <summary>
        /// 请假起始日期
        /// </summary>
        [DataMember]
        public DateTime leave_start_time { get; set; }

        /// <summary>
        /// 请假结束日期，如果是一天，则两个日期一样
        /// </summary>
        [DataMember]
        public DateTime leave_end_time { get; set; }

        /// <summary>
        /// 是否是全天请假，==1，是全天请假
        /// </summary>
        [DataMember]
        public int is_leave_all_day { get; set; }

        /// <summary>
        /// 请假类型，与leave_type表中的ID对应
        /// </summary>
        [DataMember]
        public int leave_type_id { get; set; }

        /// <summary>
        /// 请假原因，可为空字符串
        /// </summary>
        [DataMember]
        public string memo { get; set; }

        /// <summary>
        /// 请假操作日期
        /// </summary>
        [DataMember]
        public DateTime operate_time { get; set; }

        /// <summary>
        /// 操作员姓名
        /// </summary>
        [DataMember]
        public string operator_name { get; set; }

        /// <summary>
        /// 实际休假天数
        /// </summary>
        [DataMember]
        public double actual_leave_days { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        [DataMember]
        public DateTime modify_time { get; set; }
    }

    /// <summary>
    /// 查询需要字段  与人员信息、请假类型相关联
    /// </summary>
    [DataContract]
    public class UserAttendForLeave
    {
        [DataMember]
        [Key]
        public int attend_for_leave_id { get; set; }

        /// <summary>
        /// 人员ID
        /// </summary>
        [DataMember]
        public int person_id { get; set; }

        /// <summary>
        /// 人员名字
        /// </summary>
        [DataMember]
        public string person_name { get; set; }

        /// <summary>
        /// 人员工号
        /// </summary>
        [DataMember]
        public string work_sn { get; set; }

        /// <summary>
        /// 部门ID
        /// </summary>
        [DataMember]
        public int depart_id { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        [DataMember]
        public string depart_name { get; set; }

        /// <summary>
        /// 请假起始日期
        /// </summary>
        [DataMember]
        public DateTime leave_start_time { get; set; }

        /// <summary>
        /// 请假结束日期，如果是一天，则两个日期一样
        /// </summary>
        [DataMember]
        public DateTime leave_end_time { get; set; }

        /// <summary>
        /// 是否是全天请假，==1，是全天请假
        /// </summary>
        [DataMember]
        public bool is_leave_all_day { get; set; }

        /// <summary>
        /// 请假类型，与leave_type表中的ID对应
        /// </summary>
        [DataMember]
        public int leave_type_id { get; set; }

        /// <summary>
        /// 请假类型，与leave_type表中的ID对应
        /// </summary>
        [DataMember]
        public string leave_type_name { get; set; }

        /// <summary>
        /// 请假原因，可为空字符串
        /// </summary>
        [DataMember]
        public string memo { get; set; }

        /// <summary>
        /// 请假操作日期
        /// </summary>
        [DataMember]
        public DateTime operate_time { get; set; }

        /// <summary>
        /// 操作员姓名
        /// </summary>
        [DataMember]
        public string operator_name { get; set; }

        /// <summary>
        /// 实际休假天数
        /// </summary>
        [DataMember]
        public double actual_leave_days { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        [DataMember]
        public DateTime modify_time { get; set; }
    }
}