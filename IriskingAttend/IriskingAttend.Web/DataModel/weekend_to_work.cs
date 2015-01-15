/******************************************
* 模块名称：实体 weekend_to_work
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
	/// 实体 weekend_to_work
	/// </summary>
	[Description("Primary:weekend_to_work_id")]
    
	public class weekend_to_work
	{
        #region 构造函数
        /// <summary>
        /// 实体 weekend_to_work
        /// </summary>
        public weekend_to_work(){}
        #endregion

        #region 私有变量
        private Int32 _weekend_to_work_id = Int32.MinValue;
        private string _name = null;
        private Int32 _festival_id = Int32.MinValue;
        private string _begin_time = null;
        private string _end_time = null;
        private string _memo = null;
        #endregion

        #region 公共属性
        /// <summary>
        /// 主键 weekend_to_work_id(NOT NULL)
        /// </summary>
        public Int32 weekend_to_work_id
        {
            set{ _weekend_to_work_id=value;}
            get{return _weekend_to_work_id;}
        }
        /// <summary>
        /// name
        /// </summary>
        public string name
        {
            set{ _name=value;}
            get{return _name;}
        }
        /// <summary>
        /// festival_id(NOT NULL)
        /// </summary>
        public Int32 festival_id
        {
            set{ _festival_id=value;}
            get{return _festival_id;}
        }
        /// <summary>
        /// begin_time(NOT NULL)
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
    /// weekend_to_work实体集
    /// </summary>
    
    public class weekend_to_workS : CollectionBase
    {
        #region 构造函数
        /// <summary>
        /// weekend_to_work实体集
        /// </summary>
        public weekend_to_workS(){}
        #endregion

        #region 属性方法
        /// <summary>
        /// weekend_to_work集合 增加方法
        /// </summary>
        public void Add(weekend_to_work entity)
        {
            this.List.Add(entity);
        }
        /// <summary>
        /// weekend_to_work集合 索引
        /// </summary>
        public weekend_to_work this[int index]
        {
            get { return (weekend_to_work)this.List[index]; }
            set { this.List[index] = value; }
        }
        #endregion
    }
}
