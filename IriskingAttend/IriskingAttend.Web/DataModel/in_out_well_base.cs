/******************************************
* 模块名称：实体 in_out_well_base
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
	/// 实体 in_out_well_base
	/// </summary>
    
	public class in_out_well_base
	{
        #region 构造函数
        /// <summary>
        /// 实体 in_out_well_base
        /// </summary>
        public in_out_well_base(){}
        #endregion

        #region 私有变量
        private Int32 _in_out_id = Int32.MinValue;
        private Int32 _dev_group = Int32.MinValue;
        private Int32 _person_id = Int32.MinValue;
        private string _candidate_class_order = null;
        private Int32 _in_recog_id = Int32.MinValue;
        private string _in_time = null;
        private Int32 _in_leave_type_id = Int32.MinValue;
        private Int32 _out_recog_id = Int32.MinValue;
        private string _out_time = null;
        private Int32 _out_leave_type_id = Int32.MinValue;
        private Int32 _leave_type_id = Int32.MinValue;
        private string _leave_type_name = null;
        private string _zero_day = null;
        private string _attend_day = null;
        private Int32 _locate_rec_count = Int32.MinValue;
        private string _locate_time = null;
        #endregion

        #region 公共属性
        /// <summary>
        /// 主键 in_out_id(NOT NULL)
        /// </summary>
        [Key]
        public Int32 in_out_id
        {
            set{ _in_out_id=value;}
            get{return _in_out_id;}
        }
        /// <summary>
        /// dev_group
        /// </summary>
        public Int32 dev_group
        {
            set{ _dev_group=value;}
            get{return _dev_group;}
        }
        /// <summary>
        /// person_id(NOT NULL)
        /// </summary>
        public Int32 person_id
        {
            set{ _person_id=value;}
            get{return _person_id;}
        }
        /// <summary>
        /// candidate_class_order
        /// </summary>
        public string candidate_class_order
        {
            set{ _candidate_class_order=value;}
            get{return _candidate_class_order;}
        }
        /// <summary>
        /// in_recog_id
        /// </summary>
        public Int32 in_recog_id
        {
            set{ _in_recog_id=value;}
            get{return _in_recog_id;}
        }
        /// <summary>
        /// in_time
        /// </summary>
        public string in_time
        {
            set{ _in_time=value;}
            get{return _in_time;}
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
        /// out_recog_id
        /// </summary>
        public Int32 out_recog_id
        {
            set{ _out_recog_id=value;}
            get{return _out_recog_id;}
        }
        /// <summary>
        /// out_time
        /// </summary>
        public string out_time
        {
            set{ _out_time=value;}
            get{return _out_time;}
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
        /// leave_type_id
        /// </summary>
        public Int32 leave_type_id
        {
            set{ _leave_type_id=value;}
            get{return _leave_type_id;}
        }
        /// <summary>
        /// leave_type_name
        /// </summary>
        public string leave_type_name
        {
            set{ _leave_type_name=value;}
            get{return _leave_type_name;}
        }
        /// <summary>
        /// zero_day
        /// </summary>
        public string zero_day
        {
            set{ _zero_day=value;}
            get{return _zero_day;}
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
        /// locate_rec_count(NOT NULL)
        /// </summary>
        public Int32 locate_rec_count
        {
            set{ _locate_rec_count=value;}
            get{return _locate_rec_count;}
        }
        /// <summary>
        /// locate_time
        /// </summary>
        public string locate_time
        {
            set{ _locate_time=value;}
            get{return _locate_time;}
        }
        #endregion
	}

    /// <summary>
    /// in_out_well_base实体集
    /// </summary>
    
    public class in_out_well_baseS : CollectionBase
    {
        #region 构造函数
        /// <summary>
        /// in_out_well_base实体集
        /// </summary>
        public in_out_well_baseS(){}
        #endregion

        #region 属性方法
        /// <summary>
        /// in_out_well_base集合 增加方法
        /// </summary>
        public void Add(in_out_well_base entity)
        {
            this.List.Add(entity);
        }
        /// <summary>
        /// in_out_well_base集合 索引
        /// </summary>
        public in_out_well_base this[int index]
        {
            get { return (in_out_well_base)this.List[index]; }
            set { this.List[index] = value; }
        }
        #endregion
    }
}
