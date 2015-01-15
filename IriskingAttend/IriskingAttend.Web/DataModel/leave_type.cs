/******************************************
* 模块名称：实体 leave_type
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
	/// 实体 leave_type
	/// </summary>
	
    
	public class leave_type
	{
        #region 构造函数
        /// <summary>
        /// 实体 leave_type
        /// </summary>
        public leave_type(){}
        #endregion

        #region 私有变量
        private Int32 _leave_type_id = Int32.MinValue;
        private string _leave_type_name = null;
        private string _attend_sign = null;
        private short _leave_type_class = short.MaxValue;
        private short _is_normal_attend = short.MaxValue;
        private short _delete_info = short.MaxValue;
        private Int32 _is_schedule = Int32.MinValue;
        private string _create_time = null;
        private string _update_time = null;
        private string _memo = null;
        private Int32 _system_defined = Int32.MinValue;
        private Int32 _priority = Int32.MinValue;
        #endregion

        #region 公共属性
        /// <summary>
        /// leave_type_id(NOT NULL)
        /// </summary>
        [Key]
        public Int32 leave_type_id
        {
            set{ _leave_type_id=value;}
            get{return _leave_type_id;}
        }
        /// <summary>
        /// leave_type_name(NOT NULL)
        /// </summary>
        public string leave_type_name
        {
            set{ _leave_type_name=value;}
            get{return _leave_type_name;}
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
        /// leave_type_class
        /// </summary>
        public short leave_type_class
        {
            set{ _leave_type_class=value;}
            get{return _leave_type_class;}
        }
        /// <summary>
        /// is_normal_attend
        /// </summary>
        public short is_normal_attend
        {
            set{ _is_normal_attend=value;}
            get{return _is_normal_attend;}
        }
        /// <summary>
        /// delete_info
        /// </summary>
        public short delete_info
        {
            set{ _delete_info=value;}
            get{return _delete_info;}
        }
        /// <summary>
        /// is_schedule(NOT NULL)
        /// </summary>
        public Int32 is_schedule
        {
            set{ _is_schedule=value;}
            get{return _is_schedule;}
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
        /// memo
        /// </summary>
        public string memo
        {
            set{ _memo=value;}
            get{return _memo;}
        }
        /// <summary>
        /// system_defined
        /// </summary>
        public Int32 system_defined
        {
            set{ _system_defined=value;}
            get{return _system_defined;}
        }
        /// <summary>
        /// priority(NOT NULL)
        /// </summary>
        public Int32 priority
        {
            set{ _priority=value;}
            get{return _priority;}
        }
        #endregion
	}

    /// <summary>
    /// leave_type实体集
    /// </summary>
    
    public class leave_typeS : CollectionBase
    {
        #region 构造函数
        /// <summary>
        /// leave_type实体集
        /// </summary>
        public leave_typeS(){}
        #endregion

        #region 属性方法
        /// <summary>
        /// leave_type集合 增加方法
        /// </summary>
        public void Add(leave_type entity)
        {
            this.List.Add(entity);
        }
        /// <summary>
        /// leave_type集合 索引
        /// </summary>
        public leave_type this[int index]
        {
            get { return (leave_type)this.List[index]; }
            set { this.List[index] = value; }
        }
        #endregion
    }
}
