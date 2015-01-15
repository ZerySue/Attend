/******************************************
* 模块名称：实体 technical
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
	/// 实体 technical
	/// </summary>
    
	public class technical
	{
        #region 构造函数
        /// <summary>
        /// 实体 technical
        /// </summary>
        public technical(){}
        #endregion

        #region 私有变量
        private Int32 _technical_id = Int32.MinValue;
        private string _technical_name = null;
        private short _technical_class = short.MaxValue;
        private short _delete_info = short.MaxValue;
        private string _create_time = null;
        private string _update_time = null;
        private string _memo = null;
        #endregion

        #region 公共属性
        /// <summary>
        /// 主键 technical_id(NOT NULL)
        /// </summary>
        public Int32 technical_id
        {
            set{ _technical_id=value;}
            get{return _technical_id;}
        }
        /// <summary>
        /// technical_name(NOT NULL)
        /// </summary>
        public string technical_name
        {
            set{ _technical_name=value;}
            get{return _technical_name;}
        }
        /// <summary>
        /// technical_class
        /// </summary>
        public short technical_class
        {
            set{ _technical_class=value;}
            get{return _technical_class;}
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
    /// technical实体集
    /// </summary>
    
    public class technicalS : CollectionBase
    {
        #region 构造函数
        /// <summary>
        /// technical实体集
        /// </summary>
        public technicalS(){}
        #endregion

        #region 属性方法
        /// <summary>
        /// technical集合 增加方法
        /// </summary>
        public void Add(technical entity)
        {
            this.List.Add(entity);
        }
        /// <summary>
        /// technical集合 索引
        /// </summary>
        public technical this[int index]
        {
            get { return (technical)this.List[index]; }
            set { this.List[index] = value; }
        }
        #endregion
    }
}
