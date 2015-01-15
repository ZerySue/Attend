/******************************************
* 模块名称：实体 principal_type
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
	/// <summary>
	/// 实体 principal_type
	/// </summary>
    
	public class principal_type
	{
        #region 构造函数
        /// <summary>
        /// 实体 principal_type
        /// </summary>
        public principal_type(){}
        #endregion

        #region 私有变量
        private Int32 _principal_type_id = Int32.MinValue;
        private string _principal_type_name = null;
        private Int32 _principal_type_order = Int32.MinValue;
        private Int32 _min_attend_time = Int32.MinValue;
        private Int32 _is_inout_well = Int32.MinValue;
        private Int32 _min_times_per_month = Int32.MinValue;
        private Int32 _principal_type_class = Int32.MinValue;
        private Int32 _delete_info = Int32.MinValue;
        private string _memo = null;
        private string _create_time = null;
        private string _update_time = null;
        #endregion

        #region 公共属性
        /// <summary>
        /// 主键 principal_type_id(NOT NULL)
        /// </summary>
        public Int32 principal_type_id
        {
            set{ _principal_type_id=value;}
            get{return _principal_type_id;}
        }
        /// <summary>
        /// principal_type_name(NOT NULL)
        /// </summary>
        public string principal_type_name
        {
            set{ _principal_type_name=value;}
            get{return _principal_type_name;}
        }
        /// <summary>
        /// principal_type_order
        /// </summary>
        public Int32 principal_type_order
        {
            set{ _principal_type_order=value;}
            get{return _principal_type_order;}
        }
        /// <summary>
        /// min_attend_time
        /// </summary>
        public Int32 min_attend_time
        {
            set{ _min_attend_time=value;}
            get{return _min_attend_time;}
        }
        /// <summary>
        /// is_inout_well
        /// </summary>
        public Int32 is_inout_well
        {
            set{ _is_inout_well=value;}
            get{return _is_inout_well;}
        }
        /// <summary>
        /// min_times_per_month
        /// </summary>
        public Int32 min_times_per_month
        {
            set{ _min_times_per_month=value;}
            get{return _min_times_per_month;}
        }
        /// <summary>
        /// principal_type_class
        /// </summary>
        public Int32 principal_type_class
        {
            set{ _principal_type_class=value;}
            get{return _principal_type_class;}
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
        #endregion
	}

    /// <summary>
    /// principal_type实体集
    /// </summary>
    
    public class principal_typeS : CollectionBase
    {
        #region 构造函数
        /// <summary>
        /// principal_type实体集
        /// </summary>
        public principal_typeS(){}
        #endregion

        #region 属性方法
        /// <summary>
        /// principal_type集合 增加方法
        /// </summary>
        public void Add(principal_type entity)
        {
            this.List.Add(entity);
        }
        /// <summary>
        /// principal_type集合 索引
        /// </summary>
        public principal_type this[int index]
        {
            get { return (principal_type)this.List[index]; }
            set { this.List[index] = value; }
        }
        #endregion
    }
}
