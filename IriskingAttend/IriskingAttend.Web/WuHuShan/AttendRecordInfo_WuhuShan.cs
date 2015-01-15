/*************************************************************************
** 文件名:   AttendRecordInfo_WuhuShan.cs
×× 主要类:   AttendRecordInfo_WuhuShan
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   wz
** 日  期:   2013-12-17
** 修改人:  
** 日  期:       
** 描  述:   五虎山的数据类
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

namespace IriskingAttend.Web.WuHuShan
{


    /// <summary>
    /// wz
    /// 虹膜和定位卡结合的考勤信息
    /// </summary>
    [DataContract]
    public class AttendRecordInfo_WuhuShan
    {
        /// <summary>
        /// 考勤详细信息ID
        /// </summary>
        [DataMember]
        [Key]
        public int attend_record_id { get; set; }

        /// <summary>
        /// 人员ID
        /// </summary>
        [DataMember]
        public int person_id { get; set; }

        /// <summary>
        /// 五虎山定位卡记录id
        /// </summary>
        [DataMember]
        [Key]
        public int whs_locate_id { get; set; }

        /// <summary>
        /// 考勤日期
        /// </summary>
        [DataMember]
        public DateTime? attend_day { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [DataMember]
        public string name { get; set; }

        /// <summary>
        /// 工号
        /// </summary>
        [DataMember]
        public string work_sn { get; set; }


        /// <summary>
        /// 部门名称
        /// </summary>
        [DataMember]
        public string depart_name { get; set; }

        /// <summary>
        /// 工种
        /// </summary>
        [DataMember]
        public string work_type_name { get; set; }

        /// <summary>
        /// 职务
        /// </summary>
        [DataMember]
        public string principal_name { get; set; }

        /// <summary>
        /// 班次类别名称
        /// </summary>
        [DataMember]
        public string class_type_name { get; set; }

        /// <summary>
        /// 班次名称
        /// </summary>
        [DataMember]
        public string class_order_name { get; set; }

     
        /// <summary>
        /// 入井时间
        /// </summary>
        [DataMember]
        public DateTime? in_well_time { get; set; }

        /// <summary>
        /// 出井时间
        /// </summary>
        [DataMember]
        public DateTime? out_well_time { get; set; }

        /// <summary>
        /// 工作时间
        /// </summary>
        [DataMember]
        public TimeSpan? iris_work_time { get; set; }

        /// <summary>
        /// 定位卡入井时间
        /// </summary>
        [DataMember]
        public DateTime? in_locate_time { get; set; }

        /// <summary>
        /// 定位卡出井时间
        /// </summary>
        [DataMember]
        public DateTime? out_locate_time { get; set; }

        /// <summary>
        /// 定位卡工作时间
        /// </summary>
        [DataMember]
        public TimeSpan? locate_work_time { get; set; }
   

        /// <summary>
        /// 选择与否
        /// </summary>
        [DataMember]
        public bool isSelected { get; set; }

        /// <summary>
        /// 可见性 0 = 不可见
        /// 1= 可见
        /// </summary>
        [DataMember]
        public int Visibility1 { get; set; }

        /// <summary>
        /// 考勤状态
        /// 0 = 正常
        /// 1 = 未完成出入井虹膜， 但出入井定位信息都有
        /// 2 = 未完成出入井定位，但出入井虹膜都有
        /// 3 = 出入井虹膜和出入井定位都有，但时间限制不满足
        /// 4 = 出入井虹膜和出入井定位都残缺
        /// </summary>
        [DataMember]
        public int attend_state { get; set; }

    }
}