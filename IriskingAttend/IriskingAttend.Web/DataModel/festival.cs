/******************************************
* 模块名称：实体 festival
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
	/// 实体 festival
	/// </summary>
    
	public class festival
	{
        #region 构造函数
        /// <summary>
        /// 实体 festival
        /// </summary>
        public festival(){}
        #endregion

        #region 私有变量
        private Int32 _festival_id = Int32.MinValue;
        private string _name = null;
        private string _begin_time = null;
        private string _end_time = null;
        private string _memo = null;
        #endregion

        #region 公共属性
        /// <summary>
        /// 主键 festival_id(NOT NULL)
        /// </summary>
        public Int32 festival_id
        {
            set{ _festival_id=value;}
            get{return _festival_id;}
        }
        /// <summary>
        /// name(NOT NULL)
        /// </summary>
        public string name
        {
            set{ _name=value;}
            get{return _name;}
        }
        /// <summary>
        /// begin_time
        /// </summary>
        public string begin_time
        {
            set{ _begin_time=value;}
            get{return _begin_time;}
        }
        /// <summary>
        /// end_time
        /// </summary>
        public string end_time
        {
            set{ _end_time=value;}
            get{return _end_time;}
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
    /// festival实体集
    /// </summary>
    
    public class festivalS : CollectionBase
    {
        #region 构造函数
        /// <summary>
        /// festival实体集
        /// </summary>
        public festivalS(){}
        #endregion

        #region 属性方法
        /// <summary>
        /// festival集合 增加方法
        /// </summary>
        public void Add(festival entity)
        {
            this.List.Add(entity);
        }
        /// <summary>
        /// festival集合 索引
        /// </summary>
        public festival this[int index]
        {
            get { return (festival)this.List[index]; }
            set { this.List[index] = value; }
        }
        #endregion
    }
}
