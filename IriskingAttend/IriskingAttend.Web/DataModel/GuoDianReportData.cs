using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace IriskingAttend.Web.DataModel
{
    [DataContract]
    public class GuoDianReportData
    {
        public GuoDianReportData()
        {
          
            DailyContent = new List<string>();
            DailyState = new List<int>();
            VaildAttendCount = 0;
            CustomColName = "部门";
        }

        [Key]
        [DataMember]
        public int Index { get; set; }
        
        [DataMember]
        public int PersonID { get; set; }

        
        [DataMember]
        public string PersonName { get; set; }

        [DataMember]
        public string WorkSn { get; set; }

        //自定义列，内容根据excel提供的内容决定
        //如果无excel，则该列是部门属性
        [DataMember]
        public string CustomCol { get; set; }

        //有效考勤次数
        [DataMember]
        public int VaildAttendCount { get; set; }

        /// <summary>
        /// 每日考勤状态
        ///正常为0
        ///缺上下班为1
        ///缺上班为2
        ///缺下班为3
        ///时长不够为4
        ///超时为5
        /// </summary>
        [DataMember]
        public List<int> DailyState { get; set; }

        //每日显示内容
        //入井-出井-地点
        [DataMember]
        public List<string> DailyContent { get; set; }

        //自定义列名称
        [DataMember]
        public string CustomColName { get; set; }

        //表名称
        [DataMember]
        public string Title { get; set; }
       

    }

    public class RecogTimePlaceInfo
    {
        public DateTime RecogTime { get; set; }

        public string Place { get; set; }

        public RecogTimePlaceInfo()
        {
            Place = "";
        }
    }
}