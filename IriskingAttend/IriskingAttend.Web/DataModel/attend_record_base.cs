/******************************************
* 模块名称：实体 attend_record_base
* 当前版本：1.0
* 开发人员：IK1208
* 生成时间：2013-04-07
* 版本历史：此代码由 VB/C#.Net实体代码生成工具(EntitysCodeGenerate 4.4) 自动生成。
* 
******************************************/
using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Irisking.Web.DataModel
{
    //支持KEY
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.ServiceModel.DomainServices.Hosting;
    using System.ServiceModel.DomainServices.Server;
    using System.Runtime.Serialization;
	/// <summary>
	/// 实体 attend_record_base
	/// </summary>
    
	public class attend_record_base
	{
        #region 构造函数
        /// <summary>
        /// 实体 attend_record_base
        /// </summary>
        public attend_record_base(){}
        #endregion

        #region 私有变量
        private Int32 _attend_record_id = Int32.MinValue;
        private Int32 _person_id = Int32.MinValue;
        private Int32 _in_id = Int32.MinValue;
        private string _in_well_time = null;
        private Int32 _in_leave_type_id = Int32.MinValue;
        private Int32 _out_id = Int32.MinValue;
        private string _out_well_time = null;
        private Int32 _out_leave_type_id = Int32.MinValue;
        private Int32 _work_time = Int32.MinValue;
        private Int32 _in_out_times = Int32.MinValue;
        private Int32 _modify_type = Int32.MinValue;
        private Int32 _is_valid = Int32.MinValue;
        private Int32 _leave_type_id = Int32.MinValue;
        private Int32 _class_order_id = Int32.MinValue;
        private Int32 _attend_times = Int32.MinValue;
        private string _attend_day = null;
        private string _memo = null;
        private Int32 _attend_type = Int32.MinValue;
        private short _dev_group = short.MaxValue;
        private Int32 _work_cnt = Int32.MinValue;
        private Int32 _locate_rec_count = Int32.MinValue;
        #endregion

        #region 公共属性
        /// <summary>
        /// 主键 attend_record_id(NOT NULL)
        /// </summary>
        [Key]
        public Int32 attend_record_id
        {
            set{ _attend_record_id=value;}
            get{return _attend_record_id;}
        }
        /// <summary>
        /// person_id
        /// </summary>
        public Int32 person_id
        {
            set{ _person_id=value;}
            get{return _person_id;}
        }
        /// <summary>
        /// in_id
        /// </summary>
        public Int32 in_id
        {
            set{ _in_id=value;}
            get{return _in_id;}
        }
        /// <summary>
        /// in_well_time
        /// </summary>
        public string in_well_time
        {
            set{ _in_well_time=value;}
            get{return _in_well_time;}
        }
        /// <summary>
        /// in_leave_type_id
        /// </summary>
        public Int32 in_leave_type_id
        {
            set{ _in_leave_type_id=value;}
            get{return _in_leave_type_id;}
        }
        /// <summary>
        /// out_id
        /// </summary>
        public Int32 out_id
        {
            set{ _out_id=value;}
            get{return _out_id;}
        }
        /// <summary>
        /// out_well_time
        /// </summary>
        public string out_well_time
        {
            set{ _out_well_time=value;}
            get{return _out_well_time;}
        }
        /// <summary>
        /// out_leave_type_id
        /// </summary>
        public Int32 out_leave_type_id
        {
            set{ _out_leave_type_id=value;}
            get{return _out_leave_type_id;}
        }
        /// <summary>
        /// work_time
        /// </summary>
        public Int32 work_time
        {
            set{ _work_time=value;}
            get{return _work_time;}
        }
        /// <summary>
        /// in_out_times
        /// </summary>
        public Int32 in_out_times
        {
            set{ _in_out_times=value;}
            get{return _in_out_times;}
        }
        /// <summary>
        /// modify_type
        /// </summary>
        public Int32 modify_type
        {
            set{ _modify_type=value;}
            get{return _modify_type;}
        }
        /// <summary>
        /// is_valid
        /// </summary>
        public Int32 is_valid
        {
            set{ _is_valid=value;}
            get{return _is_valid;}
        }
        /// <summary>
        /// leave_type_id
        /// </summary>
        public Int32 leave_type_id
        {
            set{ _leave_type_id=value;}
            get{return _leave_type_id;}
        }
        /// <summary>
        /// class_order_id
        /// </summary>
        public Int32 class_order_id
        {
            set{ _class_order_id=value;}
            get{return _class_order_id;}
        }
        /// <summary>
        /// attend_times
        /// </summary>
        public Int32 attend_times
        {
            set{ _attend_times=value;}
            get{return _attend_times;}
        }
        /// <summary>
        /// attend_day
        /// </summary>
        public string attend_day
        {
            set{ _attend_day=value;}
            get{return _attend_day;}
        }
        /// <summary>
        /// memo
        /// </summary>
        public string memo
        {
            set{ _memo=value;}
            get{return _memo;}
        }
        /// <summary>
        /// attend_type
        /// </summary>
        public Int32 attend_type
        {
            set{ _attend_type=value;}
            get{return _attend_type;}
        }
        /// <summary>
        /// dev_group
        /// </summary>
        public short dev_group
        {
            set{ _dev_group=value;}
            get{return _dev_group;}
        }
        /// <summary>
        /// work_cnt(NOT NULL)
        /// </summary>
        public Int32 work_cnt
        {
            set{ _work_cnt=value;}
            get{return _work_cnt;}
        }
        /// <summary>
        /// locate_rec_count(NOT NULL)
        /// </summary>
        public Int32 locate_rec_count
        {
            set{ _locate_rec_count=value;}
            get{return _locate_rec_count;}
        }
        #endregion
	}

    /// <summary>
    /// attend_record_base实体集
    /// </summary>
    
    public class attend_record_baseS : CollectionBase
    {
        #region 构造函数
        /// <summary>
        /// attend_record_base实体集
        /// </summary>
        public attend_record_baseS(){}
        #endregion

        #region 属性方法
        /// <summary>
        /// attend_record_base集合 增加方法
        /// </summary>
        public void Add(attend_record_base entity)
        {
            this.List.Add(entity);
        }
        /// <summary>
        /// attend_record_base集合 索引
        /// </summary>
        public attend_record_base this[int index]
        {
            get { return (attend_record_base)this.List[index]; }
            set { this.List[index] = value; }
        }
        #endregion
    }
}
