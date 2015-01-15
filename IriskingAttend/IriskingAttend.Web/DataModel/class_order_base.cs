/******************************************
* 模块名称：实体 class_order_base
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
	/// 实体 class_order_base
	/// </summary>
	

	public class class_order_base
	{
        #region 构造函数
        /// <summary>
        /// 实体 class_order_base
        /// </summary>
        public class_order_base(){}
        #endregion

        #region 私有变量
        private Int32 _class_order_id = Int32.MinValue;
        private string _class_order_name = null;
        private short _in_well_start_time = short.MaxValue;
        private short _in_well_end_time = short.MaxValue;
        private short _out_well_start_time = short.MaxValue;
        private short _out_well_end_time = short.MaxValue;
        private Int32 _class_type_id = Int32.MinValue;
        private string _attend_sign = null;
        private short _attend_off_minutes = short.MaxValue;
        private short _class_order_class = short.MaxValue;
        private short _attend_max_minutes = short.MaxValue;
        private short _max_minutes_valid = short.MaxValue;
        private short _attend_latest_worktime = short.MaxValue;
        private short _latest_worktime_valid = short.MaxValue;
        private string _memo = null;
        private string _create_time = null;
        private string _update_time = null;
        private short _delete_info = short.MaxValue;
        #endregion

        #region 公共属性
        /// <summary>
        /// 主键 class_order_id(NOT NULL)
        /// </summary>
        [Key]
        public Int32 class_order_id
        {
            set{ _class_order_id=value;}
            get{return _class_order_id;}
        }
        /// <summary>
        /// class_order_name(NOT NULL)
        /// </summary>
        public string class_order_name
        {
            set{ _class_order_name=value;}
            get{return _class_order_name;}
        }
        /// <summary>
        /// in_well_start_time
        /// </summary>
        public short in_well_start_time
        {
            set{ _in_well_start_time=value;}
            get{return _in_well_start_time;}
        }
        /// <summary>
        /// in_well_end_time
        /// </summary>
        public short in_well_end_time
        {
            set{ _in_well_end_time=value;}
            get{return _in_well_end_time;}
        }
        /// <summary>
        /// out_well_start_time
        /// </summary>
        public short out_well_start_time
        {
            set{ _out_well_start_time=value;}
            get{return _out_well_start_time;}
        }
        /// <summary>
        /// out_well_end_time
        /// </summary>
        public short out_well_end_time
        {
            set{ _out_well_end_time=value;}
            get{return _out_well_end_time;}
        }
        /// <summary>
        /// class_type_id(NOT NULL)
        /// </summary>
        public Int32 class_type_id
        {
            set{ _class_type_id=value;}
            get{return _class_type_id;}
        }
        /// <summary>
        /// attend_sign
        /// </summary>
        public string attend_sign
        {
            set{ _attend_sign=value;}
            get{return _attend_sign;}
        }
        /// <summary>
        /// attend_off_minutes
        /// </summary>
        public short attend_off_minutes
        {
            set{ _attend_off_minutes=value;}
            get{return _attend_off_minutes;}
        }
        /// <summary>
        /// class_order_class
        /// </summary>
        public short class_order_class
        {
            set{ _class_order_class=value;}
            get{return _class_order_class;}
        }
        /// <summary>
        /// attend_max_minutes(NOT NULL)
        /// </summary>
        public short attend_max_minutes
        {
            set{ _attend_max_minutes=value;}
            get{return _attend_max_minutes;}
        }
        /// <summary>
        /// max_minutes_valid(NOT NULL)
        /// </summary>
        public short max_minutes_valid
        {
            set{ _max_minutes_valid=value;}
            get{return _max_minutes_valid;}
        }
        /// <summary>
        /// attend_latest_worktime(NOT NULL)
        /// </summary>
        public short attend_latest_worktime
        {
            set{ _attend_latest_worktime=value;}
            get{return _attend_latest_worktime;}
        }
        /// <summary>
        /// latest_worktime_valid(NOT NULL)
        /// </summary>
        public short latest_worktime_valid
        {
            set{ _latest_worktime_valid=value;}
            get{return _latest_worktime_valid;}
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
        /// create_time
        /// </summary>
        public string create_time
        {
            set{ _create_time=value;}
            get{return _create_time;}
        }
        /// <summary>
        /// update_time
        /// </summary>
        public string update_time
        {
            set{ _update_time=value;}
            get{return _update_time;}
        }
        /// <summary>
        /// delete_info
        /// </summary>
        public short delete_info
        {
            set{ _delete_info=value;}
            get{return _delete_info;}
        }
        #endregion
	}

    /// <summary>
    /// class_order_base实体集
    /// </summary>
    
    public class class_order_baseS : CollectionBase
    {
        #region 构造函数
        /// <summary>
        /// class_order_base实体集
        /// </summary>
        public class_order_baseS(){}
        #endregion

        #region 属性方法
        /// <summary>
        /// class_order_base集合 增加方法
        /// </summary>
        public void Add(class_order_base entity)
        {
            this.List.Add(entity);
        }
        /// <summary>
        /// class_order_base集合 索引
        /// </summary>
        public class_order_base this[int index]
        {
            get { return (class_order_base)this.List[index]; }
            set { this.List[index] = value; }
        }
        #endregion
    }
}
