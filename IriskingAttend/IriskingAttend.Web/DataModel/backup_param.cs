/******************************************
* 模块名称：实体 backup_param
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

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.ServiceModel.DomainServices.Hosting;
using System.ServiceModel.DomainServices.Server;
using System.Runtime.Serialization;

namespace Irisking.Web.DataModel
{
	/// <summary>
	/// 实体 backup_param
	/// </summary>
    
	public class backup_param
	{
        #region 构造函数
        /// <summary>
        /// 实体 backup_param
        /// </summary>
        public backup_param(){}
        #endregion

        #region 私有变量
        private Int32 _is_integrity = Int32.MinValue;
        private Int32 _is_compress = Int32.MinValue;
        private string _backup_destination = null;
        private string _concrete_time = null;
        private Int32 _sub_period = Int32.MinValue;
        private Int32 _period = Int32.MinValue;
        #endregion

        #region 公共属性
        /// <summary>
        /// is_integrity(NOT NULL)
        /// </summary>
        [Key]
        public Int32 is_integrity
        {
            set{ _is_integrity=value;}
            get{return _is_integrity;}
        }
        /// <summary>
        /// is_compress(NOT NULL)
        /// </summary>
        public Int32 is_compress
        {
            set{ _is_compress=value;}
            get{return _is_compress;}
        }
        /// <summary>
        /// backup_destination
        /// </summary>
        public string backup_destination
        {
            set{ _backup_destination=value;}
            get{return _backup_destination;}
        }
        /// <summary>
        /// concrete_time
        /// </summary>
        public string concrete_time
        {
            set{ _concrete_time=value;}
            get{return _concrete_time;}
        }
        /// <summary>
        /// sub_period
        /// </summary>
        public Int32 sub_period
        {
            set{ _sub_period=value;}
            get{return _sub_period;}
        }
        /// <summary>
        /// period
        /// </summary>
        public Int32 period
        {
            set{ _period=value;}
            get{return _period;}
        }
        #endregion
	}

    /// <summary>
    /// backup_param实体集
    /// </summary>
    
    public class backup_paramS : CollectionBase
    {
        #region 构造函数
        /// <summary>
        /// backup_param实体集
        /// </summary>
        public backup_paramS(){}
        #endregion

        #region 属性方法
        /// <summary>
        /// backup_param集合 增加方法
        /// </summary>
        public void Add(backup_param entity)
        {
            this.List.Add(entity);
        }
        /// <summary>
        /// backup_param集合 索引
        /// </summary>
        public backup_param this[int index]
        {
            get { return (backup_param)this.List[index]; }
            set { this.List[index] = value; }
        }
        #endregion
    }
}
