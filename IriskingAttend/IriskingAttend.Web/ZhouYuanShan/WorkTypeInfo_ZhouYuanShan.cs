/*************************************************************************
** 文件名:   WorkTypeInfo_ZhouYuanShan.cs
×× 主要类:   WorkTypeInfo_ZhouYuanShan
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   wz
** 日  期:   2013-12-30
** 修改人:  
** 日  期:       
** 描  述:   周源山的数据类
**
** 版  本:   1.0.0
** 备  注:  命名及代码编写遵守C#编码规范
**
 * ***********************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
//using System.Data.Objects.DataClasses;
using System.Linq;
using System.ServiceModel.DomainServices.Hosting;
using System.ServiceModel.DomainServices.Server;
using System.Runtime.Serialization;
using System.Drawing;
using System.Net.Sockets;

namespace IriskingAttend.Web.ZhouYuanShan
{


    /// <summary>
    /// wz
    /// 与部门结合的工种信息
    /// </summary>
    [DataContract]
    public class WorkTypeInfo_ZhouYuanShan
    {
        /// <summary>
        /// 考勤详细信息ID
        /// </summary>
        [DataMember]
        [Key]
        public int work_type_id { get; set; }

        /// <summary>
        /// 属于部门ID
        /// </summary>
        [DataMember]
        [Key]
        public int depart_id { get; set; }

        /// <summary>
        /// 工种名称
        /// </summary>
        [DataMember]
        public string work_type_name { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [DataMember]
        public string memo { get; set; }


     

    }

    
    /// <summary>
    /// 班中餐模块
    /// 需要未完成的上报记录
    /// 以部门为单位
    /// </summary>
    [DataContract]
    public class ReportRecordInfoOnDepart_ZhouYuanShan
    {
    
        /// <summary>
        /// 属于部门ID
        /// </summary>
        [DataMember]
        [Key]
        public int depart_id { get; set; }

        /// <summary>
        /// 属于部门
        /// </summary>
        [DataMember]
        public string depart_name { get; set; }

        /// <summary>
        /// 考勤日期
        /// </summary>
        [DataMember]
        [Key]
        public DateTime attend_day { get; set; }

        /// <summary>
        /// 班次id
        /// </summary>
        [DataMember]
        [Key]
        public int class_order_id { get; set; }

        /// <summary>
        /// 班次名称
        /// </summary>
        [DataMember]
        public string class_order_name { get; set; }
        

        /// <summary>
        /// 班次简称
        /// </summary>
        [DataMember]
        public string attend_sign { get; set; }

        /// <summary>
        /// 考勤人数
        /// </summary>
        [DataMember]
        public int attend_person_count { get; set; }

        /// <summary>
        /// 考勤人员姓名
        /// </summary>
        [DataMember]
        public string[] attend_person_names { get; set; }

        /// <summary>
        /// 考勤人员id
        /// </summary>
        [DataMember]
        public int[] attend_person_ids { get; set; }

        /// <summary>
        /// 考勤记录id
        /// </summary>
        [DataMember]
        public int[] attend_record_ids { get; set; }

        /// <summary>
        /// 上报人数
        /// </summary>
        [DataMember]
        public int reported_count { get; set; }

        /// <summary>
        /// 差异人员姓名
        /// </summary>
        [DataMember]
        public string[] diff_person_names { get; set; }

        /// <summary>
        /// 差异人员id
        /// </summary>
        [DataMember]
        public int[] diff_person_ids { get; set; }

        /// <summary>
        /// 已完成的班中餐记录id
        /// </summary>
        [DataMember]
        public int[] report_record_ids { get; set; }

        /// <summary>
        /// 状态('未完成'或者'已完成')
        /// </summary>
        [DataMember]
        public string state { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public bool isSelected { get; set; }

    }

    /// <summary>
    /// 以部门分组的班中餐记录信息
    /// </summary>
    [DataContract]
    public class LunchRecordInfoOnDepart
    {
        /// <summary>
        /// 属于部门ID
        /// </summary>
        [DataMember]
        [Key]
        public int depart_id { get; set; }

        /// <summary>
        /// 属于部门
        /// </summary>
        [DataMember]
        public string depart_name { get; set; }

        /// <summary>
        /// 开始日期
        /// </summary>
        [DataMember]
        [Key]
        public DateTime start_day { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>
        [DataMember]
        [Key]
        public DateTime end_day { get; set; }

        /// <summary>
        /// 考勤人次
        /// </summary>
        [DataMember]
        public int attend_person_count { get; set; }

        /// <summary>
        /// 差异人次
        /// </summary>
        [DataMember]
        public int diff_person_count { get; set; }

        /// <summary>
        /// 上报人次
        /// </summary>
        [DataMember]
        public int reported_count { get; set; }

        /// <summary>
        /// 差异人员姓名
        /// </summary>
        [DataMember]
        public string[] diff_person_names { get; set; }

       

        /// <summary>
        /// 差异人员id
        /// </summary>
        [DataMember]
        public int[] diff_person_ids { get; set; }

        /// <summary>
        /// 每个人的差异次数
        /// </summary>
        [DataMember]
        public int[] diff_count_per_person { get; set; }


        /// <summary>
        /// 考勤人员姓名
        /// </summary>
        [DataMember]
        public string[] attend_person_names { get; set; }

        /// <summary>
        /// 考勤人员id
        /// </summary>
        [DataMember]
        public int[] attend_person_ids { get; set; }

        /// <summary>
        /// 已完成的班中餐记录id
        /// </summary>
        [DataMember]
        public int[] report_record_ids { get; set; }

        /// <summary>
        /// 是否选择
        /// </summary>
        [DataMember]
        public bool isSelected { get; set; }
    }

    /// <summary>
    /// 个人班中餐记录信息详情
    /// </summary>
    [DataContract]
    public class LunchRecordInfoOnPerson
    {
        /// <summary>
        /// 人员id
        /// </summary>
        [Key]
        [DataMember]
        public int person_id { get; set; }

        /// <summary>
        /// 上报部门ID
        /// </summary>
        [DataMember]
        [Key]
        public int report_depart_id { get; set; }

        /// <summary>
        /// 日期
        /// </summary>
        [DataMember]
        [Key]
        public DateTime report_day { get; set; }


        /// <summary>
        /// 工号 
        /// </summary>
        [DataMember]
        public string work_sn { get; set; }

        /// <summary>
        /// 姓名 
        /// </summary>
        [DataMember]
        public string name { get; set; }
          
        /// <summary>
        /// 属于部门ID
        /// </summary>
        [DataMember]
        public int depart_id { get; set; }

        /// <summary>
        /// 属于部门
        /// </summary>
        [DataMember]
        public string depart_name { get; set; }

      

        /// <summary>
        /// 上报部门
        /// </summary>
        [DataMember]
        public string report_depart_name { get; set; }

      

        /// <summary>
        /// 差异次数（目前只能是1）
        /// </summary>
        [DataMember]
        public int diff_count { get; set; }

        /// <summary>
        /// 班次简称
        /// </summary>
        [DataMember]
        public string attend_sign { get; set; }

        /// <summary>
        /// 班次
        /// </summary>
        [DataMember]
        public string class_order_name { get; set; }

      

        /// <summary>
        /// 班次id
        /// </summary>
        [DataMember]
        public int report_class_order_id { get; set; }

        /// <summary>
        /// 是否选择
        /// </summary>
        [DataMember]
        public bool isSelected { get; set; }
    }


   

}