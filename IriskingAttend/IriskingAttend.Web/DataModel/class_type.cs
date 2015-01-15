/******************************************
* 模块名称：实体 class_type
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

//支持KEY
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.ServiceModel.DomainServices.Hosting;
using System.ServiceModel.DomainServices.Server;
using System.Runtime.Serialization;

namespace Irisking.Web.DataModel
{
	/// <summary>
	/// 实体 class_type
	/// </summary>
    
	public class class_type
	{
        #region 构造函数
        /// <summary>
        /// 实体 class_type
        /// </summary>
        public class_type(){}
        #endregion

        #region 私有变量
        private Int32 _class_type_id = Int32.MinValue;
        private string _class_type_name = null;
        private short _class_type = short.MaxValue;
        private short _delete_info = short.MaxValue;
        private string _create_time = null;
        private string _update_time = null;
        private string _memo = null;
        #endregion

        #region 公共属性
        /// <summary>
        /// 主键 class_type_id(NOT NULL)
        /// </summary>
        [Key]
        public Int32 class_type_id
        {
            set{ _class_type_id=value;}
            get{return _class_type_id;}
        }
        /// <summary>
        /// class_type_name(NOT NULL)
        /// </summary>
        public string class_type_name
        {
            set{ _class_type_name=value;}
            get{return _class_type_name;}
        }
        /// <summary>
        /// class_type
        /// </summary>
        public short classtype
        {
            set{ _class_type=value;}
            get{return _class_type;}
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
        #endregion
	}

    /// <summary>
    /// class_type实体集
    /// </summary>
    
    public class class_typeS : CollectionBase
    {
        #region 构造函数
        /// <summary>
        /// class_type实体集
        /// </summary>
        public class_typeS(){}
        #endregion

        #region 属性方法
        /// <summary>
        /// class_type集合 增加方法
        /// </summary>
        public void Add(class_type entity)
        {
            this.List.Add(entity);
        }
        /// <summary>
        /// class_type集合 索引
        /// </summary>
        public class_type this[int index]
        {
            get { return (class_type)this.List[index]; }
            set { this.List[index] = value; }
        }
        #endregion
    }
}
