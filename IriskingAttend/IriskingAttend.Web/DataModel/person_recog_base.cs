/******************************************
* 模块名称：实体 person_recog_base
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
	/// 实体 person_recog_base
	/// </summary>
    
	public class person_recog_base
	{

        #region 构造函数
        /// <summary>
        /// 实体 person_recog_base
        /// </summary>
        public person_recog_base(){}
        #endregion

        #region 私有变量
        private Int32 _person_recog_log_id = Int32.MinValue;
        private Int32 _person_id = Int32.MinValue;
        private Int32 _class_type_id = Int32.MinValue;
        private Int32 _feature_id = Int32.MinValue;
        private string _recog_time = null;
        private short _recog_type = short.MaxValue;
        private string _at_position = null;
        private string _device_sn = null;
        private Int32 _device_seq = Int32.MinValue;
        private short _dev_type = short.MaxValue;
        private string _op_time = null;
        private string _memo = null;
        private string _delete_time = null;
        #endregion

        #region 公共属性
        /// <summary>
        /// 主键 person_recog_log_id(NOT NULL)
        /// </summary>
        [Key]
        public Int32 person_recog_log_id
        {
            set{ _person_recog_log_id=value;}
            get{return _person_recog_log_id;}
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
        /// class_type_id
        /// </summary>
        public Int32 class_type_id
        {
            set{ _class_type_id=value;}
            get{return _class_type_id;}
        }
        /// <summary>
        /// feature_id
        /// </summary>
        public Int32 feature_id
        {
            set{ _feature_id=value;}
            get{return _feature_id;}
        }
        /// <summary>
        /// recog_time(NOT NULL)
        /// </summary>
        public string recog_time
        {
            set{ _recog_time=value;}
            get{return _recog_time;}
        }
        /// <summary>
        /// recog_type(NOT NULL)
        /// </summary>
        public short recog_type
        {
            set{ _recog_type=value;}
            get{return _recog_type;}
        }
        /// <summary>
        /// at_position
        /// </summary>
        public string at_position
        {
            set{ _at_position=value;}
            get{return _at_position;}
        }
        /// <summary>
        /// device_sn
        /// </summary>
        public string device_sn
        {
            set{ _device_sn=value;}
            get{return _device_sn;}
        }
        /// <summary>
        /// device_seq(NOT NULL)
        /// </summary>
        public Int32 device_seq
        {
            set{ _device_seq=value;}
            get{return _device_seq;}
        }
        /// <summary>
        /// dev_type(NOT NULL)
        /// </summary>
        public short dev_type
        {
            set{ _dev_type=value;}
            get{return _dev_type;}
        }
        /// <summary>
        /// op_time(NOT NULL)
        /// </summary>
        public string op_time
        {
            set{ _op_time=value;}
            get{return _op_time;}
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
        /// delete_time
        /// </summary>
        public string delete_time
        {
            set{ _delete_time=value;}
            get{return _delete_time;}
        }
        #endregion
	}

    /// <summary>
    /// person_recog_base实体集
    /// </summary>
    
    public class person_recog_baseS : CollectionBase
    {
        #region 构造函数
        /// <summary>
        /// person_recog_base实体集
        /// </summary>
        public person_recog_baseS(){}
        #endregion

        #region 属性方法
        /// <summary>
        /// person_recog_base集合 增加方法
        /// </summary>
        public void Add(person_recog_base entity)
        {
            this.List.Add(entity);
        }
        /// <summary>
        /// person_recog_base集合 索引
        /// </summary>
        public person_recog_base this[int index]
        {
            get { return (person_recog_base)this.List[index]; }
            set { this.List[index] = value; }
        }
        #endregion
    }
}
