/******************************************
* 模块名称：实体 attend_multiple
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
	/// 实体 attend_multiple
	/// </summary>

    
	public class attend_multiple
	{
        #region 构造函数
        /// <summary>
        /// 实体 attend_multiple
        /// </summary>
        public attend_multiple(){}
        #endregion

        #region 私有变量
        private Int32 _attend_multiple_id = Int32.MinValue;
        private Int32 _person_id = Int32.MinValue;
        private Int32 _attend_record_id = Int32.MinValue;
        private Int32 _multiple_type = Int32.MinValue;
        private Int32 _multiple = Int32.MinValue;
        private string _multiple_date = null;
        private string _memo = null;
        #endregion

        #region 公共属性
        /// <summary>
        /// 主键 attend_multiple_id(NOT NULL)
        /// </summary>
        public Int32 attend_multiple_id
        {
            set{ _attend_multiple_id=value;}
            get{return _attend_multiple_id;}
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
        /// attend_record_id
        /// </summary>
        public Int32 attend_record_id
        {
            set{ _attend_record_id=value;}
            get{return _attend_record_id;}
        }
        /// <summary>
        /// multiple_type
        /// </summary>
        public Int32 multiple_type
        {
            set{ _multiple_type=value;}
            get{return _multiple_type;}
        }
        /// <summary>
        /// multiple
        /// </summary>
        public Int32 multiple
        {
            set{ _multiple=value;}
            get{return _multiple;}
        }
        /// <summary>
        /// multiple_date
        /// </summary>
        public string multiple_date
        {
            set{ _multiple_date=value;}
            get{return _multiple_date;}
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
    /// attend_multiple实体集
    /// </summary>
    
    public class attend_multipleS : CollectionBase
    {
        #region 构造函数
        /// <summary>
        /// attend_multiple实体集
        /// </summary>
        public attend_multipleS(){}
        #endregion

        #region 属性方法
        /// <summary>
        /// attend_multiple集合 增加方法
        /// </summary>
        public void Add(attend_multiple entity)
        {
            this.List.Add(entity);
        }
        /// <summary>
        /// attend_multiple集合 索引
        /// </summary>
        public attend_multiple this[int index]
        {
            get { return (attend_multiple)this.List[index]; }
            set { this.List[index] = value; }
        }
        #endregion
    }
}
