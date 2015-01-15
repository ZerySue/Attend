/*************************************************************************
** 文件名:   CustomClass.cs
** 主要类:   UserAttendRec
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   lzc
** 日  期:   2013-4-12
** 修改人:   gqy  szr 增加记工时班次对象
** 日  期:   2013-4-19  2014-11-19
** 描  述:    定义实体类
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
    using System.Drawing;
    using System.Net.Sockets;

    /// <summary>
    /// lzc
    /// 考勤查询结构
    /// </summary>
    [DataContract]
    public class UserAttendRec
    {
        /// <summary>
        /// 考勤ID
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
        /// 姓名
        /// </summary>
        [DataMember]
        public string person_name { get; set; }

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
        /// 班次名称
        /// </summary>
        [DataMember]
        public string class_order_name { get; set; }
        
        /// <summary>
        /// 工作日总工作时间
        /// </summary>
        [DataMember]
        public string sum_work_time { get; set; }

        /// <summary>
        /// 加班总工作时间
        /// </summary>
        [DataMember]
        public string sum_over_time { get; set; }

        /// <summary>
        /// 平均作时间
        /// </summary>
        [DataMember]
        public string avg_work_time { get; set; }

        /// <summary>
        /// 工数
        /// </summary>
        [DataMember]
        public double sum_work_cnt { get; set; }

        /// <summary>
        /// 工数
        /// </summary>
        [DataMember]
        public int sum_count { get; set; }

        /// <summary>
        /// 出勤次数
        /// </summary>
        [DataMember]
        public float sum_times { get; set; }

        /// <summary>
        /// 工种  add by gqy
        /// </summary>
        [DataMember]
        public string work_type_name { get; set; }

        /// <summary>
        /// 职务  add by gqy
        /// </summary>
        [DataMember]
        public string principal_name { get; set; }


     
    }

    /// <summary>
    /// lzc
    /// 考勤详细信息
    /// </summary>
    [DataContract]
    public class UserAttendRecDetail
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
        /// 姓名
        /// </summary>
        [DataMember]
        public string person_name { get; set; }

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
        /// 班次名称
        /// </summary>
        [DataMember]
        public string class_order_name { get; set; }

        ////工数
        //[DataMember]
        //public int work_cnt { get; set; }

        /// <summary>
        /// 入井时间
        /// </summary>
        [DataMember]
        public DateTime? in_well_time { get; set; }
          /// <summary>
        /// 入井时间
        /// </summary>
        [DataMember]
        public string in_well_time_str { get; set; }

        /// <summary>
        /// 出井时间
        /// </summary>
        [DataMember]
        public string out_well_time_str { get; set; }

        /// <summary>
        /// 出井时间
        /// </summary>
        [DataMember]
        public DateTime? out_well_time { get; set; }
        /// <summary>
        /// 工作时间
        /// </summary>
        [DataMember]
        public string work_time { get; set; }

        /// <summary>
        /// 工作时间 页面显示
        /// </summary>
        [DataMember]
        public string work_time_str { get; set; }

        /// <summary>
        /// 出入井次数
        /// </summary>
        [DataMember]
        public int in_out_times { get; set; }

        /// <summary>
        /// 是否有效
        /// </summary>
        [DataMember]
        public int is_valid { get; set; }

        /// <summary>
        /// 入井状态
        /// </summary>
        [DataMember]
        public string in_leave_type_name { get; set; }

        /// <summary>
        /// 出井状态
        /// </summary>
        [DataMember]
        public string out_leave_type_name { get; set; }


        /// <summary>
        /// 考勤次数
        /// </summary>
        [DataMember]
        public int attend_times { get; set; }

        /// <summary>
        /// 考勤归属日
        /// </summary>
        [DataMember]
        public string attend_day { get; set; }

        /// <summary>
        /// 考勤分组
        /// </summary>
        [DataMember]
        public string dev_group { get; set; }

        /// <summary>
        /// 考勤类型
        /// </summary>
        [DataMember]
        public string leave_type_name { get; set; }

        /// <summary>
        /// 工数
        /// </summary>
        [DataMember]
        public double work_cnt { get; set; }

        /// <summary>
        /// 显示颜色设置
        /// </summary>
        [DataMember]
        public string leave_type_name_color { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [DataMember]
        public string memo { get; set; }

        /// <summary>
        /// 是否为周末或者节假日
        /// </summary>
        [DataMember]
        public int DayType { get; set; }

        /// <summary>
        /// 签到时间
        /// </summary>
        [DataMember]
        public string recog_sign_time { get; set; }
        /// <summary>
        /// 稽核路径
        /// </summary>
        [DataMember]
        public string attend_path { get; set; }

    }

    /// <summary>
    /// lzc
    /// 人员简单信息
    /// </summary>
    [DataContract]
    public class UserPersonSimple
    {
        /// <summary>
        /// 人员ID
        /// </summary>
        [DataMember]
        [Key]
        public int person_id { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [DataMember]
        public string person_name { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        [DataMember]
        public string depart_name { get; set; }

        /// <summary>
        /// 部门Id
        /// </summary>
        [DataMember]
        public int depart_id { get; set; }

        /// <summary>
        /// 工号
        /// </summary>
        [DataMember]
         public string work_sn { get; set; }

        /// <summary>
        /// 是否被选择
        /// </summary>
        [DataMember]
        public bool is_select { get; set; }

    }


    /// <summary>
    /// lzc
    /// 当前井下人员简单信息
    /// </summary>
    [DataContract]
    public class UserInWellPerson
    {
        /// <summary>
        /// 井下人员识别信息ID
        /// </summary>
        [DataMember]
        [Key]
        public int in_out_id { get; set; }

        /// <summary>
        /// 人员ID
        /// </summary>
        [DataMember]
        public int person_id { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [DataMember]
        public string person_name { get; set; }

        /// <summary>
        /// 部门Id
        /// </summary>
        [DataMember]
        public string depart_name { get; set; }

        /// <summary>
        /// 工号
        /// </summary>
        [DataMember]
        public string work_sn { get; set; }

        /// <summary>
        /// 班次名称
        /// </summary>
        [DataMember]
        public string class_order_name { get; set; }

        /// <summary>
        /// 在岗类型
        /// </summary>
        [DataMember]
        public string dev_group { get; set; }

        /// <summary>
        /// 在岗类型
        /// </summary>
        [DataMember]
        public int dev_group_int { get; set; }

        /// <summary>
        /// 工作状态
        /// </summary>
        [DataMember]
        public string  work_state{ get; set; }

        /// <summary>
        /// 入井时间
        /// </summary>
        [DataMember]
        public DateTime in_time { get; set; }

        /// <summary>
        /// 识别记录ID
        /// </summary>
        [DataMember]
        public int in_recog_id { get; set; }

        /// <summary>
        /// 工作时间
        /// </summary>
        [DataMember]
        public string work_time { get; set; }

        /// <summary>
        /// 是否被选择
        /// </summary>
        [DataMember]
        public bool is_select { get; set; }

    }

    /// <summary>
    /// 所有人员识别记录
    /// </summary>
    [DataContract]
    public class UserAllPersonRecogLog
    {
        /// <summary>
        /// 人员ID
        /// </summary>
        [Key]
        [DataMember]
        public int person_id { get; set; }

        /// <summary>
        /// 人员名字
        /// </summary>
        [DataMember]
        public string person_name { get; set; }


        /// <summary>
        /// 部门名称
        /// </summary>
        [DataMember]
        public string depart_name { get; set; }

        /// <summary>
        /// 工号
        /// </summary>
        [DataMember]
        public string work_sn { get; set; }

        /// <summary>
        /// 记录条数
        /// </summary>
        [DataMember]
        public int record_count{ get; set; }

    }

    /// <summary>
    /// 人员识别记录，手动补加记录特别处理
    /// </summary>
    [DataContract]
    public class UserPersonRecogLog
    {
        /// <summary>
        /// 识别记录ID
        /// </summary>
        [DataMember]
        [Key]
        public int person_recog_id { get; set; }

        /// <summary>
        /// 是否被选中  add by gqy
        /// </summary>
        [DataMember]
        public bool isSelected { get; set; }

        /// <summary>
        /// 人员名字
        /// </summary>
        [DataMember]
        public int person_id { get; set; }

        /// <summary>
        /// 人员名字
        /// </summary>
        [DataMember]
        public string person_name { get; set; }


        /// <summary>
        /// 部门名称
        /// </summary>
        [DataMember]
        public string depart_name { get; set; }

        /// <summary>
        /// 工号
        /// </summary>
        [DataMember]
        public string work_sn { get; set; }

        /// <summary>
        /// 识别时间
        /// </summary>
        [DataMember]
        public DateTime recog_time { get; set; }


        /// <summary>
        /// 识别类型
        /// </summary>
        [DataMember]
        public string recog_type { get; set; }


        /// <summary>
        /// 记录类型颜色设置 --如果人工补加记录以灰色加以区分
        /// </summary>
        [DataMember]
        public string recog_type_color { get; set; }

        /// <summary>
        /// 识别地点
        /// </summary>
        [DataMember]
        public string at_position { get; set; }


        /// <summary>
        /// 设备编号
        /// </summary>
        [DataMember]
        public string device_sn { get; set; }

        /// <summary>
        /// 记录类型
        /// </summary>
        [DataMember]
        public string dev_type { get; set; }

        /// <summary>
        /// 记录类型
        /// </summary>
        [DataMember]
        public int dev_type_value { get; set; }


        /// <summary>
        /// 备注
        /// </summary>
        [DataMember]
        public string memo { get; set; }


        /// <summary>
        /// 记录标记
        /// </summary>
        [DataMember]
        public string proc_mark { get; set; }

        /// <summary>
        /// 班次名称
        /// </summary>
        [DataMember]
        public string class_type_name { get; set; }


        /// <summary>
        /// 识别记录状态
        /// </summary>
        [DataMember]
        public string attend_state { get; set; }

        /// <summary>
        /// 显示颜色设置
        /// </summary>
        [DataMember]
        public string attend_state_color { get; set; }

        /// <summary>
        /// 备注：识别记录添加的操作员名称  add by gqy
        /// </summary>
        [DataMember]
        public string operator_name { get; set; }

        /// <summary>
        /// 是否节假日  add by yuhaitao
        /// </summary>
        [DataMember]
        public int DayType { get; set; }

    }

    /// <summary>
    /// wz
    /// 人员查询之后的显示结果
    /// 用于提供人员信息管理界面的数据支持
    /// </summary>
    [DataContract]
    public class UserPersonInfo
    {
        /// <summary>
        /// 人员ID
        /// </summary>
        [DataMember]
        [Key]
        public int person_id { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        [DataMember]
        public int index { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [DataMember]
        public string person_name { get; set; }

        /// <summary>
        /// 工号
        /// </summary>
        [DataMember]
        public string work_sn { get; set; }

        /// <summary>
        /// 虹膜注册
        /// </summary>
        [DataMember]
        public string iris_register { get; set; }
        
        //虹膜状态
        [DataMember]
        public string iris_status { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        [DataMember]
        public string depart_name { get; set; }

        /// <summary>
        /// 部门id
        /// </summary>
        [DataMember]
        public int depart_id { get; set; }

        /// <summary>
        /// 上级部门
        /// </summary>
        [DataMember]
        public string parent_depart_name { get; set; }

        /// <summary>
        /// 部门编号
        /// </summary>
        [DataMember]
        public string depart_sn { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        [DataMember]
        public string sex { get; set; }

        /// <summary>
        /// 班次类别名称 地面
        /// </summary>
        [DataMember]
        public string class_type_name_on_ground { get; set; }

         /// <summary>
        /// 班次类别名称 井下
         /// </summary>
        [DataMember]
        public string class_type_name { get; set; }

        /// <summary>
        /// 班次类别id 地面 
        /// </summary>
        [DataMember]
        public int class_type_id_on_ground { get; set; }

        /// <summary>
        /// 班制id 井下
        /// </summary>
        [DataMember]
        public int class_type_id { get; set; }

        /// <summary>
        /// 是否选中
        /// </summary>
        [DataMember]
        public bool isSelected { get; set; }

        /// <summary>
        /// 血型
        /// </summary>
        [DataMember]
        public string blood_type { get; set; }

        /// <summary>
        /// 出生日期
        /// </summary>
        [DataMember]
        public DateTime birthdate { get; set; }

        /// <summary>
        /// 参加工作日期
        /// </summary>
        [DataMember]
        public DateTime workday { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        [DataMember]
        public string phone { get; set; }

        /// <summary>
        /// 邮政编码
        /// </summary>
        [DataMember]
        public string zipcode { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        [DataMember]
        public string address { get; set; }

        /// <summary>
        /// 身份证
        /// </summary>
        [DataMember]
        public string id_card { get; set; }

        /// <summary>
        /// 电子邮件
        /// </summary>
        [DataMember]
        public string email { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [DataMember]
        public string memo { get; set; }

        /// <summary>
        /// 图像二进制数据
        /// </summary>
        [DataMember]
        public byte[] image { get; set; }

        /// <summary>
        /// 图像类型
        /// </summary>
        [DataMember]
        public string img_type { get; set; }

        /// <summary>
        /// 工种
        /// </summary>
        [DataMember]
        public int work_type_id { get; set; }

        /// <summary>
        /// 职务
        /// </summary>
        [DataMember]
        public int principal_id { get; set; }

        /// <summary>
        /// 职务名称
        /// </summary>
        [DataMember]
        public string principal_name { get; set; }

        /// <summary>
        /// 工种名称
        /// </summary>
        [DataMember]
        public string work_type_name { get; set; }


        public UserPersonInfo()
        {
            memo = "";
            address = "";
            image = null;
            img_type = "";
            person_id = -1;
            email = "";
            id_card = "";
            zipcode = "";
            phone = "";
            blood_type = "";
            class_type_name_on_ground = "";
            sex = "";
            depart_sn = "";
            parent_depart_name = "";
            depart_name = "";
            iris_status = "";
            iris_register = "";
            work_sn = "";
            person_name = "";
            index = -1;
            birthdate = DateTime.MinValue;
            workday = DateTime.MinValue;
            depart_id = -1;
            class_type_id_on_ground = -1;
        }

    
        
    }

    /// <summary>
    /// wcf ria 前台调用后台
    /// 异步操作的返回信息
    /// by wz
    /// </summary>
    [DataContract]
    public class OptionInfo
    {
        /// <summary>
        /// 操作码id
        /// </summary>
        [DataMember]
        [Key]
        public int option_info_id { get; set; }

        /// <summary>
        /// 操作信息
        /// </summary>
        [DataMember]
        public string option_info { get; set; }

        /// <summary>
        /// 成功标志
        /// </summary>
        [DataMember]
        public bool isSuccess { get; set; }

        /// <summary>
        /// 通知后台服务器成功标志
        /// </summary>
        [DataMember]
        public bool isNotifySuccess { get; set; }


        /// <summary>
        /// 用于保存id
        /// </summary>
        [DataMember]
        public int tag { get; set; }

        public OptionInfo()
        {
            isNotifySuccess = true;
            isSuccess = true;
        }
    }

    /// <summary>
    /// wz
    /// 部门信息
    /// </summary>
    [DataContract]
    public class UserDepartInfo
    {

        public UserDepartInfo()
        {
            depart_id = -1;
            isSelected = false;
            depart_name = "";
            depart_sn = "";
            parent_depart_id = -1;
            parent_depart_name = "";
            index = -1;
        }

        /// <summary>
        /// 部门id
        /// </summary>
        [Key]
        [DataMember]
        public Int32 depart_id
        {
            set;
            get;
        }
        /// <summary>
        /// company_sn
        /// </summary>
        [DataMember]
        public string company_sn
        {
            set;
            get;
        }
        /// <summary>
        /// 部门编号
        /// </summary>
        [DataMember]
        public string depart_sn
        {
            set;
            get;
        }
        /// <summary>
        /// 部门名称
        /// </summary>
        [DataMember]
        public string depart_name
        {
            set;
            get;
        }
        /// <summary>
        /// 父部门id
        /// </summary>
        [DataMember]
        public Int32 parent_depart_id
        {
            set;
            get;
        }

        /// <summary>
        /// 父部门名称
        /// </summary>
        [DataMember]
        public string parent_depart_name
        {
            get;
            set;
        }

        /// <summary>
        /// depart_auth
        /// </summary>
        [DataMember]
        public string depart_auth
        {
            set;
            get;
        }
        /// <summary>
        /// depart_director
        /// </summary>
        [DataMember]
        public string depart_director
        {
            set;
            get;
        }
        /// <summary>
        /// depart_function_id
        /// </summary>
        [DataMember]
        public Int32 depart_function_id
        {
            set;
            get;
        }
        /// <summary>
        /// depart_work_place_id
        /// </summary>
        [DataMember]
        public Int32 depart_work_place_id
        {
            set;
            get;
        }
        /// <summary>
        /// contact_phone
        /// </summary>
        [DataMember]
        public string contact_phone
        {
            set;
            get;
        }
        /// <summary>
        /// delete_time
        /// </summary>
        [DataMember]
        public string delete_time
        {
            set;
            get;
        }
        /// <summary>
        /// create_time
        /// </summary>
        [DataMember]
        public string create_time
        {
            set;
            get;
        }
        /// <summary>
        /// update_time
        /// </summary>
        [DataMember]
        public string update_time
        {
            set;
            get;
        }
        /// <summary>
        /// memo
        /// </summary>
        [DataMember]
        public string memo
        {
            set;
            get;
        }

        /// <summary>
        /// 序号
        /// </summary>
        [DataMember]
        public int index
        {
            set;
            get;
        }

        /// <summary>
        /// 是否选中
        /// </summary>
        [DataMember]
        public bool isSelected
        {
            set;
            get;
        }



    }

    /// <summary>
    /// wz
    /// 班制信息
    /// </summary>
    [DataContract]
    public class UserClassTypeInfo
    {
        public UserClassTypeInfo()
        {
            class_type_id = -1;
            class_type_name = "";
            index = -1;
            memo = "";
            isSelected = false;
        }

        /// <summary>
        /// 主键 班制id(NOT NULL)
        /// </summary>
        [Key]
        [DataMember]
        public Int32 class_type_id
        {
            get;
            set;
        }
        /// <summary>
        /// 班制名称(NOT NULL)
        /// </summary>
        [DataMember]
        public string class_type_name
        {
            set;
            get;
        }
        /// <summary>
        /// class_type
        /// </summary>
        [DataMember]
        public short class_type
        {
            set;
            get;
        }

        /// <summary>
        /// delete_info
        /// </summary>
        [DataMember]
        public short delete_info
        {
            set;
            get;
        }

        /// <summary>
        /// 备注
        /// </summary>
        [DataMember]
        public string memo
        {
            set;
            get;
        }

        /// <summary>
        /// 序号
        /// </summary>
        [DataMember]
        public int index
        {
            set;
            get;
        }

        /// <summary>
        /// 是否选中
        /// </summary>
        [DataMember]
        public bool isSelected
        {
            set;
            get;
        }
    }

    /// <summary>
    /// wz
    /// 班次信息
    /// </summary>
    [DataContract]
    public class UserClassOrderInfo
    {

        public UserClassOrderInfo()
        {
            this.index = -1;
            this.isSelected = false;
            this.avail_time_linear = -1;
            this.class_order_id = -1;
            this.work_cnt_linear = -1;
            this.is_count_workcnt_by_timeduration = false;
            is_count_workcnt_by_timeduration_str = " [否]";
            this.avail_time_linear_str = "无效";
            this.work_cnt_linear_str = "无效";
            this.in_well_end_time = -1;
            this.out_well_end_time = -1;
            this.out_well_start_time = -1;
            this.in_well_start_time = -1;
        }

        /// <summary>
        /// 序号
        /// </summary>
        [Key]
        [DataMember]
        public int index
        {
            set;
            get;
        }

        /// <summary>
        /// 是否选中
        /// </summary>
        [DataMember]
        public bool isSelected
        {
            set;
            get;
        }

        /// <summary>
        /// 是否按时间段记工
        /// </summary>
        [DataMemberAttribute]
        public bool is_count_workcnt_by_timeduration
        {
            get;
            set;
        }

        /// <summary>
        /// 是否按时间段记工 显示内容
        /// </summary>
        [DataMember]
        public string is_count_workcnt_by_timeduration_str
        {
            get;
            set;
        }


        #region class_order.timeduration

        /// <summary>
        /// 按时间段记工时长
        /// </summary>
         [DataMemberAttribute]
        public short[] avail_time_timeduration
        {
            get;
            set;
        }

        /// <summary>
         /// 按时间段记工时长 显示内容
        /// </summary>
         [DataMemberAttribute]
         public string[] avail_time_timeduration_str
         {
             get;
             set;
         }

        
        /// <summary>
         /// 按时间段记工工数
        /// </summary>
        [DataMemberAttribute]
        public Int32[] work_cnt_timeduration
        {
            get;
            set;
        }

        /// <summary>
        /// 按时间段记工工数
        /// </summary>
        [DataMemberAttribute]
        public string[] work_cnt_timeduration_str
        {
            get;
            set;
        }

        #endregion


        #region class_order.standard 表中属性

        /// <summary>
        /// 线性记工时长
        /// </summary>
        [DataMemberAttribute]
        public short avail_time_linear
        {
            get;
            set;
        }

        /// <summary>
        /// 线性记工时长
        /// </summary>
        [DataMemberAttribute]
        public string avail_time_linear_str
        {
            get;
            set;
        }

        /// <summary>
        /// 线性记工工数
        /// </summary>
        [DataMemberAttribute]
        public Int32 work_cnt_linear
        {
            get;
            set;
        }

        /// <summary>
        /// 线性记工工数
        /// </summary>
        [DataMemberAttribute]
        public string work_cnt_linear_str
        {
            get;
            set;
        }

        #endregion

        #region class_order_base表中原有属性
        /// <summary>
        /// 主键 班次id(NOT NULL)
        /// </summary>
        [Key]
        [DataMemberAttribute]
        public Int32 class_order_id
        {
            set;
            get;
        }
        /// <summary>
        /// 班次名称(NOT NULL)
        /// </summary>
        [DataMemberAttribute]
        public string class_order_name
        {
            set;
            get;
        }
        /// <summary>
        /// 入井开始时间
        /// </summary>
        [DataMemberAttribute]
        public short in_well_start_time
        {
            set;
            get;
        }

        /// <summary>
        /// 入井开始时间
        /// </summary>
        [DataMemberAttribute]
        public string in_well_start_time_str
        {
            set;
            get;
        }

        /// <summary>
        /// 入井结束时间
        /// </summary>
        [DataMemberAttribute]
        public short in_well_end_time
        {
            set;
            get;
        }

        /// <summary>
        /// 入井结束时间
        /// </summary>
        [DataMemberAttribute]
        public string in_well_end_time_str
        {
            set;
            get;
        }

        /// <summary>
        /// 出井开始时间
        /// </summary>
        [DataMemberAttribute]
        public short out_well_start_time
        {
            set;
            get;
        }

        /// <summary>
        /// 出井开始时间
        /// </summary>
        [DataMemberAttribute]
        public string out_well_start_time_str
        {
            set;
            get;
        }

        /// <summary>
        /// 出井结束时间
        /// </summary>
        [DataMemberAttribute]
        public short out_well_end_time
        {
            set;
            get;
        }

        /// <summary>
        /// 出井结束时间
        /// </summary>
        [DataMemberAttribute]
        public string out_well_end_time_str
        {
            set;
            get;
        }

        /// <summary>
        /// 班制id(NOT NULL)
        /// </summary>
        [DataMemberAttribute]
        public Int32 class_type_id
        {
            set;
            get;
        }

        /// <summary>
        /// 班制名称
        /// </summary>
        [DataMemberAttribute]
        public string class_type_name
        {
            set;
            get;
        }

        
        /// <summary>
        /// 简称
        /// </summary>
        [DataMemberAttribute]
        public string attend_sign
        {
            set;
            get;
        }

        /// <summary>
        /// 考勤偏移时间
        /// </summary>
        [DataMemberAttribute]
        public short attend_off_minutes
        {
            set;
            get;
        }

        /// <summary>
        /// 考勤偏移时间
        /// </summary>
        [DataMemberAttribute]
        public string attend_off_minutes_str
        {
            set;
            get;
        }

        /// <summary>
        /// 最大在岗时长(NOT NULL)
        /// </summary>
        [DataMemberAttribute]
        public short attend_max_minutes
        {
            set;
            get;
        }

        /// <summary>
        /// 最大在岗时长
        /// </summary>
        [DataMemberAttribute]
        public string attend_max_minutes_str
        {
            set;
            get;
        }

        /// <summary>
        /// 最大在岗时长是否有效(NOT NULL)
        /// </summary>
        [DataMemberAttribute]
        public short max_minutes_valid
        {
            set;
            get;
        }

   

        /// <summary>
        /// 最晚下班时间(NOT NULL)
        /// </summary>
        [DataMemberAttribute]
        public short attend_latest_worktime
        {
            set;
            get;
        }

        /// <summary>
        /// 最晚下班时间
        /// </summary>
        [DataMemberAttribute]
        public string attend_latest_worktime_str
        {
            set;
            get;
        }

        /// <summary>
        /// 最晚下班时间是否有效(NOT NULL)
        /// </summary>
        [DataMemberAttribute]
        public short latest_worktime_valid
        {
            set;
            get;
        }
        /// <summary>
        /// 备注
        /// </summary>
        [DataMemberAttribute]
        public string memo
        {
            set;
            get;
        }
      
        #endregion
    }

    /// <summary>
    /// gqy
    /// 签到班班次信息
    /// </summary>
    [DataContract]
    public class UserClassOrderSignInfo
    {

        public UserClassOrderSignInfo()
        {          
            this.isSelected = false;           
            this.class_order_id = -1;    
        }       

        #region class_order_base表中原有属性
        /// <summary>
        /// 主键 班次id(NOT NULL)
        /// </summary>
        [DataMember]
        [Key]        
        public Int32 class_order_id
        {
            set;
            get;
        }
        /// <summary>
        /// 班次名称(NOT NULL)
        /// </summary>
        [DataMember]
        public string class_order_name
        {
            set;
            get;
        }
        
        /// <summary>
        /// 班制id(NOT NULL)
        /// </summary>
        [DataMember]
        public Int32 class_type_id
        {
            set;
            get;
        }

        /// <summary>
        /// 班制名称
        /// </summary>
        [DataMember]
        public string class_type_name
        {
            set;
            get;
        }

        /// <summary>
        /// 简称
        /// </summary>
        [DataMember]
        public string attend_sign
        {
            set;
            get;
        }

        /// <summary>
        /// 是否连班
        /// </summary>
        [DataMember]
        public Int32 lian_ban
        {
            set;
            get;
        }
       
        /// <summary>
        /// 备注
        /// </summary>
        [DataMember]
        public string memo
        {
            set;
            get;
        }

        #endregion

        /// <summary>
        /// 是否选中
        /// </summary>
        [DataMember]
        public bool isSelected
        {
            set;
            get;
        }  

        /// <summary>
        /// 签到开始时间
        /// </summary>
        [DataMember]
        public int[] section_begin_mins
        {
            set;
            get;
        }

        /// <summary>
        /// 签到结束时间
        /// </summary>
        [DataMember]
        public int[] section_end_mins
        {
            set;
            get;
        }

        /// <summary>
        /// 此次签到是否计入计算
        /// </summary>
        [DataMember]
        public int[] in_calcs
        {
            set;
            get;
        }

        /// <summary>
        /// 签到时间段显示
        /// </summary>
        [DataMember]
        public string section_time_str
        {
            set;
            get;
        }

        /// <summary>
        /// 记工时长
        /// </summary>
        [DataMember]
        public short min_work_time
        {
            get;
            set;
        }

        /// <summary>
        /// 按时间段记工时长 显示内容
        /// </summary>
        [DataMember]
        public string min_work_time_str
        {
            get;
            set;
        }

        /// <summary>
        /// 记工工数
        /// </summary>
        [DataMember]
        public Int32 work_cnt
        {
            get;
            set;
        }

        /// <summary>
        /// 记工工数
        /// </summary>
        [DataMember]
        public string work_cnt_str
        {
            get;
            set;
        }    

    }
    /// <summary>
    /// szr
    /// 记工时班次结构
    /// </summary>
    [DataContract]
    public class UserClassOrderJiGongShiInfo
    {

        public UserClassOrderJiGongShiInfo()
        {          
            this.isSelected = false;
            this.in_start_time = 0;
            this.in_end_time = 24;
            this.attend_max_minutes = 0;
            this.max_minutes_valid = 0;
            this.max_minutes_validStr = "无效";
            this.attend_latest_worktime = 0;
            this.latest_worktime_valid = 0;
            this.latest_worktime_validStr = "无效";
            this.work_cnt = 10;
            this.avail_time = 1;
            this.in_well_end_time = -1;
          
            this.in_well_start_time = -1;

        } 
        /// <summary>
        /// 是否选中
        /// </summary>
        [DataMember]
        public bool isSelected
        {
            set;
            get;
        }
        /// <summary>
        /// 班次名称
        /// </summary>
        [DataMember]
        public string class_order_name
        {
            set;
            get;
        }
        /// <summary>
        /// 主键 班次id(NOT NULL)
        /// </summary>
        [DataMember]
        [Key]
        public Int32 class_order_id
        {
            set;
            get;
        }

        #region class_order_base属性
        /// <summary>
        /// 班制id(NOT NULL)
        /// </summary>
        [DataMember]
        public Int32 class_type_id
        {
            set;
            get;
        }
        /// <summary>
        /// 班制名称
        /// </summary>
        [DataMember]
        public string class_type_name
        {
            set;
            get;
        }
        /// <summary>
        /// 班次简称
        /// </summary>
        [DataMember]
        public string attend_sign
        {
            set;
            get;
        }
        /// <summary>
        /// 所属单位
        /// </summary>
        [DataMember]
        public string depart_name
        {
            set;
            get;
        }
        /// <summary>
        /// 创建时间
        /// </summary>
        [DataMember]
        public DateTime create_time
        {
            set;
            get;
        }


        /// <summary>
        /// 更新时间
        /// </summary>
        [DataMember]
        public DateTime update_time
        {
            set;
            get;
        }


        /// <summary>
        /// 入井开始时间
        /// </summary>
        [DataMemberAttribute]
        public short in_well_start_time
        {
            set;
            get;
        }

        /// <summary>
        /// 入井开始时间
        /// </summary>
        [DataMemberAttribute]
        public string in_well_start_time_str
        {
            set;
            get;
        }

        /// <summary>
        /// 入井结束时间
        /// </summary>
        [DataMemberAttribute]
        public short in_well_end_time
        {
            set;
            get;
        }

        /// <summary>
        /// 入井结束时间
        /// </summary>
        [DataMemberAttribute]
        public string in_well_end_time_str
        {
            set;
            get;
        }

        /// <summary>
        /// 备注
        /// </summary>
        [DataMember]
        public string memo
        {
            set;
            get;
        }
        #endregion
        /// <summary>
        /// in_start_time 默认值0
        /// </summary>
        [DataMember]
        public Int32 in_start_time
        {
            set;
            get;
        }

        /// <summary>
        /// in_end_time 默认值24
        /// </summary>
        [DataMember]
        public Int32 in_end_time
        {
            set;
            get;
        }

        /// <summary>
        /// attend_off_minutes 考勤归属日
        /// </summary>
        [DataMember]
        public Int32 attend_off_minutes
        {
            set;
            get;
        }

        /// <summary>
        /// attend_off_minutes 考勤归属日
        /// </summary>
        [DataMember]
        public string attend_off_minutesStr
        {
            set;
            get;
        }
        /// <summary>
        /// attend_max_minutes 最长在岗时长
        /// </summary>
        [DataMember]
        public Int32 attend_max_minutes
        {
            set;
            get;
        }
        /// <summary>
        /// attend_max_minutes 最长在岗时长
        /// </summary>
        [DataMember]
        public string attend_max_minutesStr
        {
            set;
            get;
        }
        /// <summary>
        /// max_minutes_valid  最大时长是否有效 1有效0无效
        /// </summary>
        [DataMember]
        public Int32 max_minutes_valid
        {
            set;
            get;
        }

        /// <summary>
        /// max_minutes_valid  最大时长是否有效 1有效0无效
        /// </summary>
        [DataMember]
        public string max_minutes_validStr 
        {
            set;
            get;
        }
        /// <summary>
        /// attend_latest_worktime 班次最晚下班时间点
        /// </summary>
        [DataMember]
        public Int32 attend_latest_worktime
        {
            set;
            get;
        }
      
        /// <summary>
        /// attend_latest_worktime 班次最晚下班时间点
        /// </summary>
        [DataMember]
        public string attend_latest_worktimeStr
        {
            set;
            get;
        }

        /// <summary>
        /// latest_worktime_valid  最晚下班时间是否有效1有效0无效
        /// </summary>
        [DataMember]
        public Int32 latest_worktime_valid
        {
            set;
            get;
        }

         /// <summary>
        /// latest_worktime_valid  最晚下班时间是否有效1有效0无效
        /// </summary>
        [DataMember]
        public string latest_worktime_validStr
        {
            set;
            get;
        }

        /// <summary>
        ///work_cnt  工数 单位0.1
        /// </summary>
        [DataMember]
        public double work_cnt 
        {
            set;
            get;
        }

        /// <summary>
        ///work_cnt  工数 单位0.1
        /// </summary>
        [DataMember]
        public double work_cntStr
        {
            set;
            get;
        }
         /// <summary>
        ///单位工作时长 分钟为单位
        /// </summary>
        [DataMember]
        public Int32 avail_time 
        {
            set;
            get;
        }

        /// <summary>
        ///单位工作时长 分钟为单位
        /// </summary>
        [DataMember]
        public string avail_timeStr
        {
            set;
            get;
        }

    }
        
    /// <summary>
    /// gqy
    /// 操作员日志结构
    /// </summary>
    [DataContract]
    public class UserOperationLog
    {
        [DataMember]
        [Key]
        public Int32 user_operation_log_id
        {
            set;
            get;
        }

        /// <summary>
        /// user_id
        /// </summary>
        [DataMember]
        public Int32 user_id
        {
            set;
            get;
        }
        /// <summary>
        /// user_name
        /// </summary>
        [DataMember]
        public string user_name
        {
            set;
            get;
        }
        /// <summary>
        /// user_ip
        /// </summary>
        [DataMember]
        public string user_ip
        {
            set;
            get;
        }
        /// <summary>
        /// operation_time(NOT NULL)
        /// </summary>
        [DataMember]
        public DateTime operation_time
        {
            set;
            get;
        }
        /// <summary>
        /// content
        /// </summary>
        [DataMember]
        public string content
        {
            set;
            get;
        }
        /// <summary>
        /// result
        /// </summary>
        [DataMember]
        public short result
        {
            set;
            get;
        }
        /// <summary>
        /// resultStr
        /// </summary>
        [DataMember]
        public string resultStr
        {
            set;
            get;
        }     
        /// <summary>
        /// description
        /// </summary>
        [DataMember]
        public string description
        {
            set;
            get;
        }        
    }

    /// <summary>
    /// gqy
    /// 设备查询结构
    /// </summary>
    [DataContract]
    public class DeviceInfo
    {
        /// <summary>
        /// 设备编号
        /// </summary>
        [DataMember]        
        [Key]        
        public string dev_sn 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// 起始启用时间
        /// </summary>
        [DataMember]
        [Key]
        public string start_time 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// 设备类型
        /// </summary>
        [DataMember]
        [Key]
        public int dev_type 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// 设备类型的字符形式,包括开始启用时间 by wz
        /// </summary>
        [DataMember]
        public List<string> dev_type_List
        {
            get;
            set;
        }

      
        /// <summary>
        /// 设备类型的字符形式，用于界面显示
        /// </summary>
        [DataMember]
        public string dev_type_strings
        {
            get;
            set;
        }

        /// <summary>
        /// 设备类型的字符形式
        /// </summary>
        [DataMember]
        public string dev_type_string 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// 设备放置地方
        /// </summary>
        [DataMember]
        public string place 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// 设备类型
        /// </summary>
        [DataMember]
        public string dev_function
        {
            get;
            set;
        }

        /// <summary>
        /// 是否被选中
        /// </summary>
        [DataMember]
        public bool isSelected 
        { 
            get; 
            set; 
        }
    }

    /// <summary>
    /// gqy
    /// 节假日查询结构
    /// </summary>
    [DataContract]
    public class FestivalInfo
    {
        /// <summary>
        /// 节假日id号
        /// </summary>
        [DataMember]
        [Key]
        public Int32 festival_id
        {
            get;
            set;
        }

        /// <summary>
        /// 节假日名称
        /// </summary>
        [DataMember]
        public string name
        {
            get;
            set;
        }

        /// <summary>
        /// 节假日起始时间
        /// </summary>
        [DataMember]
        public string begin_time
        {
            get;
            set;
        }

        /// <summary>
        /// 节假日结束时间
        /// </summary>
        [DataMember]
        public string end_time
        {
            get;
            set;
        }

        /// <summary>
        /// 节假日备注
        /// </summary>
        [DataMember]
        public string memo
        {
            get;
            set;
        }

        /// <summary>
        /// 节假日调休日期
        /// </summary>
        [DataMember]
        public string ShiftHoliday
        {
            get;
            set;
        }

        /// <summary>
        /// 节假日调休日期
        /// </summary>
        [DataMember]
        public List<DateTime> ShiftHolidayList
        {
            get;
            set;
        }

        /// <summary>
        /// 是否被选中
        /// </summary>
        [DataMember]
        public bool isSelected
        {
            get;
            set;
        }

        public FestivalInfo()
        {
            festival_id = 0;
            begin_time = "";
            end_time = "";
            ShiftHolidayList = new List<DateTime>();
            ShiftHoliday = "";
            isSelected = false;
        }
    
    }

    /// <summary>
    /// cty
    /// 停用虹膜列表信息
    /// </summary>
    [DataContract]
    public class PersonStopIrisInfo
    {
        /// <summary>
        /// 停用虹膜表主键
        /// </summary>
        [DataMember]
        [Key]
        public int person_disable_info_id { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        [DataMember]
        public int index { get; set; }

        /// <summary>
        /// 人员id
        /// </summary>
        [DataMember]
        public int person_id { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [DataMember]
        public string person_name { get; set; }

        /// <summary>
        /// 工号
        /// </summary>
        [DataMember]
        public string work_sn { get; set; }

        /// <summary>
        /// 停用虹膜开始日期
        /// </summary>
        [DataMember]
        public string begin_time { get; set; }

        /// <summary>
        /// 停用虹膜结束日期
        /// </summary>
        [DataMember]
        public string end_time { get; set; }

        /// <summary>
        /// 超出虹膜的操作
        /// </summary>
        [DataMember]
        public string  policy { get; set; }

        /// <summary>
        /// 是否被选中
        /// </summary>
        [DataMember]
        public bool isSelected { get; set; }

        public PersonStopIrisInfo()
        {
            person_disable_info_id = -1;

            person_id = -1;
            person_name = "";
            work_sn = "";
            policy = "";
            
        }

    }

    /// <summary>
    /// cty
    /// 人员姓名 id
    /// </summary>
    [DataContract]
    public class XlsPersoninfo
    {
        /// <summary>
        /// 人员id
        /// </summary>
        [DataMember]
        [Key]
        public int XlsPersonId { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [DataMember]
        public string XlsPersonName { get; set; }
    }

    /// <summary>
    /// cty
    /// 原始记录汇总表查询结果
    /// </summary>
    [DataContract]
    public class XlsUserAttendRec
    {
        /// <summary>
        /// 考勤id
        /// </summary>
        [DataMember]
        [Key]
        public int attend_record_id { get; set; }

        /// <summary>
        /// 人员id
        /// </summary>
        [DataMember]
        public int person_id { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [DataMember]
        public string person_name { get; set; }

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
        /// 班次名称
        /// </summary>
        [DataMember]
        public string class_order_name { get; set; }

        /// <summary>
        /// 班制名称
        /// </summary>
        [DataMember]
        public string class_type_name { get; set; }

        /// <summary>
        /// 考勤数据
        /// </summary>
        [DataMember]
        public int work_time { get; set; }

        /// <summary>
        /// 工数
        /// </summary>
        [DataMember]
        public int attend_times { get; set; }

        /// <summary>
        /// 归属日
        /// </summary>
        [DataMember]
        public DateTime attend_day { get; set; }

        public XlsUserAttendRec()
        {

        }


    }

    /// <summary>
    /// cty
    /// 传递备份参数的结构体
    /// </summary>
    [DataContract]
    public class BackupParameter1
    {
        public BackupParameter1()
        {
            ManOrAuto = 3;
        }

        /// <summary>
        /// 标志是手动备份还是自动备份，0-手动备份，1-自动备份，2-发送口令，返回设置的自动备份参数
        /// </summary>
        [DataMember]
        [Key]
        public int ManOrAuto { get; set; }

        /// <summary>
        /// 要备份的数据库,0-考勤库，1-虹膜库 (只对手动备份有用)
        /// </summary>
        [DataMember]
        public int DatabaseName { get; set; }

        /// <summary>
        /// 备份考勤库的路径
        /// </summary>
        [DataMember]
        public string PathIrisApp { get; set; }

        /// <summary>
        /// 备份考勤库周期
        /// </summary>
        [DataMember]
        public int PeriodIrisApp { get; set; }

        /// <summary>
        /// 备份考勤库日期
        /// </summary>
        [DataMember]
        public int SubPeriokIrisApp { get; set; }

        /// <summary>
        /// 备份考勤库具体时间
        /// </summary>
        [DataMember]
        public DateTime ConcreteTimeIrisApp { get; set; }

        /// <summary>
        /// 备份虹膜库的路径
        /// </summary>
        [DataMember]
        public string PathIrisData { get; set; }

        /// <summary>
        /// 备份虹膜库周期
        /// </summary>
        [DataMember]
        public int PeriodIrisData { get; set; }

        /// <summary>
        /// 备份虹膜库日期
        /// </summary>
        [DataMember]
        public int SubPeriokIrisData { get; set; }

        /// <summary>
        /// 备虹膜库具体时间
        /// </summary>
        [DataMember]
        public DateTime ConcreteTimeIrisData { get; set; }

    }
    /// <summary>
    /// cty
    /// 工种
    /// </summary>
    [DataContract]
    public class WorkTypeInfo
    {
        /// <summary>
        /// work_type_id
        /// </summary>
        [DataMember]
        [Key]
        public int work_type_id { get; set; }

        /// <summary>
        /// 工种名称
        /// </summary>
        [DataMember]
        public string work_type_name { get; set; }

        /// <summary>
        /// 是否选中
        /// </summary>
        [DataMember]
        public bool isSelected { get; set; }

        /// <summary>
        /// 备注说明
        /// </summary>
        [DataMember]
        public string memo { get; set; }

        public WorkTypeInfo()
        {
            work_type_id = -1;
            work_type_name = "";
            memo = "";
        }

    }

    /// <summary>
    /// cty
    /// 职务
    /// </summary>
    [DataContract]
    public class PrincipalInfo
    {
        /// <summary>
        /// principal_id
        /// </summary>
        [DataMember]
        [Key]
        public int principal_id { get; set; }

        /// <summary>
        /// 职务名称
        /// </summary>
        [DataMember]
        public string principal_name { get; set; }

        /// <summary>
        /// 职务类型的id
        /// </summary>
        [DataMember]
        public int principal_type_id { get; set; }

        /// <summary>
        /// 职务类型的名称
        /// </summary>
        [DataMember]
        public string principal_type_name { get; set; }

        /// <summary>
        /// 计入考勤的类型
        /// </summary>
        [DataMember]
        public string principal_class { get; set; }

        /// <summary>
        /// 职务类型的顺序
        /// </summary>
        [DataMember]
        public int principal_type_order { get; set; }

        /// <summary>
        /// 备注说明
        /// </summary>
        [DataMember]
        public string memo { get; set; }

        /// <summary>
        /// 是否选中
        /// </summary>
        [DataMember]
        public bool isSelected { get; set; }

        public PrincipalInfo()
        {
            principal_id = -1;
            principal_name = "";
            principal_type_id = -1;
            principal_type_name = "";
            principal_class = "0";
            memo = "";
            ;
        }

    }

    /// <summary>
    /// wz
    /// 职务类型
    /// </summary>
    [DataContract]
    public class PrincipalTypeInfo
    {
        /// <summary>
        /// 主键
        /// </summary>
        [DataMember]
        [Key]
        public int principal_type_id { get; set; }

        /// <summary>
        /// 职务类型名称
        /// </summary>
        [DataMember]
        public string principal_type_name { get; set; }

        /// <summary>
        /// 职务类型的顺序
        /// </summary>
        [DataMember]
        public int principal_type_order { get; set; }



        /// <summary>
        /// 备注说明
        /// </summary>
        [DataMember]
        public string memo { get; set; }

        /// <summary>
        /// 是否选中
        /// </summary>
        [DataMember]
        public bool isSelected { get; set; }

        public PrincipalTypeInfo()
        {
            principal_type_name = "";
            memo = "";
        }

    }

    /// <summary>
    /// cty
    /// 考勤类型
    /// </summary>
    [DataContract]
    public class LeaveType
    {
        /// <summary>
        /// leave_type_id(NOT NULL)
        /// </summary>
        [DataMember]
        [Key]
        public Int32 leave_type_id { get; set; }

        /// <summary>
        /// 类型名称
        /// </summary>
        [DataMember]
        public string leave_type_name { get; set; }

        /// <summary>
        /// 考勤符号
        /// </summary>
        [DataMember]
        public string attend_sign { get; set; }

        /// <summary>
        /// leave_type_class
        /// </summary>
        [DataMember]
        public short leave_type_class { get; set; }
  
        /// <summary>
        /// 考勤类型
        /// </summary>
        [DataMember]
        public short is_normal_attend { get; set; }

        /// <summary>
        /// 考勤类型str
        /// </summary>
        [DataMember]
        public string is_normal_attendStr { get; set; }
  
        /// <summary>
        /// delete_info
        /// </summary>
        [DataMember]
        public short delete_info { get; set; }
   
        /// <summary>
        /// 是否记工
        /// </summary>
        [DataMember]
        public Int32 is_schedule { get; set; }

        /// <summary>
        /// 是否记工str
        /// </summary>
        [DataMember]
        public string is_scheduleStr { get; set; }
   
        /// <summary>
        /// 创建时间
        /// </summary>
        [DataMember]
        public string create_time { get; set; }
   
        /// <summary>
        /// 更新时间
        /// </summary>
        [DataMember]
        public string update_time { get; set; }
   
        /// <summary>
        /// 备注
        /// </summary>
        [DataMember]
        public string memo { get; set; }
    
        /// <summary>
        /// system_defined
        /// </summary>
        [DataMember]
        public Int32 system_defined { get; set; }
    
        /// <summary>
        /// priority(NOT NULL)
        /// </summary>
        [DataMember]
        public Int32 priority { get; set; }

        /// <summary>
        /// 是否选中
        /// </summary>
        [DataMember]
        public bool isSelected { get; set; }

        public LeaveType()
        {
            isSelected = false;
            leave_type_class = 0;
            priority = 0;
        }
   
    }

}