/******************************************
* 模块名称：实体 principal
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
using System.ComponentModel.DataAnnotations;

namespace Irisking.Web.DataModel
{
	/// <summary>
	/// 实体 principal
	/// </summary>
    
	public class principal
	{
        #region 构造函数
        /// <summary>
        /// 实体 principal
        /// </summary>
        public principal(){}
        #endregion

        #region 私有变量
        private Int32 _principal_id = Int32.MinValue;
        private string _principal_name = null;
        private Int32 _principal_type_id = Int32.MinValue;
        private Int32 _principal_class = Int32.MinValue;
        private Int32 _delete_info = Int32.MinValue;
        private string _create_time = null;
        private string _update_time = null;
        private string _memo = null;
        #endregion

        #region 公共属性
        /// <summary>
        /// 主键 principal_id(NOT NULL)
        /// </summary>
        [Key]
        public Int32 principal_id
        {
            set{ _principal_id=value;}
            get{return _principal_id;}
        }
        /// <summary>
        /// principal_name(NOT NULL)
        /// </summary>
        public string principal_name
        {
            set{ _principal_name=value;}
            get{return _principal_name;}
        }
        /// <summary>
        /// principal_type_id
        /// </summary>
        public Int32 principal_type_id
        {
            set{ _principal_type_id=value;}
            get{return _principal_type_id;}
        }
        /// <summary>
        /// principal_class
        /// </summary>
        public Int32 principal_class
        {
            set{ _principal_class=value;}
            get{return _principal_class;}
        }
        /// <summary>
        /// delete_info
        /// </summary>
        public Int32 delete_info
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
    /// principal实体集
    /// </summary>
    
    public class principalS : CollectionBase
    {
        #region 构造函数
        /// <summary>
        /// principal实体集
        /// </summary>
        public principalS(){}
        #endregion

        #region 属性方法
        /// <summary>
        /// principal集合 增加方法
        /// </summary>
        public void Add(principal entity)
        {
            this.List.Add(entity);
        }
        /// <summary>
        /// principal集合 索引
        /// </summary>
        public principal this[int index]
        {
            get { return (principal)this.List[index]; }
            set { this.List[index] = value; }
        }
        #endregion
    }
}
