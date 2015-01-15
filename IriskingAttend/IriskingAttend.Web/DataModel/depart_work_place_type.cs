/******************************************
* 模块名称：实体 depart_work_place_type
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
	/// 实体 depart_work_place_type
	/// </summary>
	[Description("Primary:depart_work_place_id")]
    
	public class depart_work_place_type
	{
        #region 构造函数
        /// <summary>
        /// 实体 depart_work_place_type
        /// </summary>
        public depart_work_place_type(){}
        #endregion

        #region 私有变量
        private Int32 _depart_work_place_id = Int32.MinValue;
        private string _depart_work_place_name = null;
        private Int32 _depart_work_place_class = Int32.MinValue;
        private Int32 _delete_info = Int32.MinValue;
        private string _create_time = null;
        private string _update_time = null;
        private string _memo = null;
        #endregion

        #region 公共属性
        /// <summary>
        /// 主键 depart_work_place_id(NOT NULL)
        /// </summary>
        public Int32 depart_work_place_id
        {
            set{ _depart_work_place_id=value;}
            get{return _depart_work_place_id;}
        }
        /// <summary>
        /// depart_work_place_name(NOT NULL)
        /// </summary>
        public string depart_work_place_name
        {
            set{ _depart_work_place_name=value;}
            get{return _depart_work_place_name;}
        }
        /// <summary>
        /// depart_work_place_class
        /// </summary>
        public Int32 depart_work_place_class
        {
            set{ _depart_work_place_class=value;}
            get{return _depart_work_place_class;}
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
    /// depart_work_place_type实体集
    /// </summary>
    
    public class depart_work_place_typeS : CollectionBase
    {
        #region 构造函数
        /// <summary>
        /// depart_work_place_type实体集
        /// </summary>
        public depart_work_place_typeS(){}
        #endregion

        #region 属性方法
        /// <summary>
        /// depart_work_place_type集合 增加方法
        /// </summary>
        public void Add(depart_work_place_type entity)
        {
            this.List.Add(entity);
        }
        /// <summary>
        /// depart_work_place_type集合 索引
        /// </summary>
        public depart_work_place_type this[int index]
        {
            get { return (depart_work_place_type)this.List[index]; }
            set { this.List[index] = value; }
        }
        #endregion
    }
}
