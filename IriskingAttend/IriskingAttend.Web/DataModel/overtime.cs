/******************************************
* 模块名称：实体 overtime
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
	/// 实体 overtime
	/// </summary>

    
	public class overtime
	{
        #region 构造函数
        /// <summary>
        /// 实体 overtime
        /// </summary>
        public overtime(){}
        #endregion

        #region 私有变量
        private Int32 _overtime_id = Int32.MinValue;
        private short _is_validated = short.MaxValue;
        private short _is_automated = short.MaxValue;
        private Int32 _hour_point = Int32.MinValue;
        private short _is_doubletime_validated = short.MaxValue;
        private short _is_doubletime_automated = short.MaxValue;
        private Int32 _doubletime_hour_point = Int32.MinValue;
        private string _memo = null;
        #endregion

        #region 公共属性
        /// <summary>
        /// 主键 overtime_id(NOT NULL)
        /// </summary>
        public Int32 overtime_id
        {
            set{ _overtime_id=value;}
            get{return _overtime_id;}
        }
        /// <summary>
        /// is_validated
        /// </summary>
        public short is_validated
        {
            set{ _is_validated=value;}
            get{return _is_validated;}
        }
        /// <summary>
        /// is_automated
        /// </summary>
        public short is_automated
        {
            set{ _is_automated=value;}
            get{return _is_automated;}
        }
        /// <summary>
        /// hour_point
        /// </summary>
        public Int32 hour_point
        {
            set{ _hour_point=value;}
            get{return _hour_point;}
        }
        /// <summary>
        /// is_doubletime_validated
        /// </summary>
        public short is_doubletime_validated
        {
            set{ _is_doubletime_validated=value;}
            get{return _is_doubletime_validated;}
        }
        /// <summary>
        /// is_doubletime_automated
        /// </summary>
        public short is_doubletime_automated
        {
            set{ _is_doubletime_automated=value;}
            get{return _is_doubletime_automated;}
        }
        /// <summary>
        /// doubletime_hour_point
        /// </summary>
        public Int32 doubletime_hour_point
        {
            set{ _doubletime_hour_point=value;}
            get{return _doubletime_hour_point;}
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
    /// overtime实体集
    /// </summary>
    
    public class overtimeS : CollectionBase
    {
        #region 构造函数
        /// <summary>
        /// overtime实体集
        /// </summary>
        public overtimeS(){}
        #endregion

        #region 属性方法
        /// <summary>
        /// overtime集合 增加方法
        /// </summary>
        public void Add(overtime entity)
        {
            this.List.Add(entity);
        }
        /// <summary>
        /// overtime集合 索引
        /// </summary>
        public overtime this[int index]
        {
            get { return (overtime)this.List[index]; }
            set { this.List[index] = value; }
        }
        #endregion
    }
}
