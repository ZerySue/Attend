/******************************************
* 模块名称：实体 person_disable_info
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
	/// 实体 person_disable_info
	/// </summary>
    
	public class person_disable_info
	{
        #region 构造函数
        /// <summary>
        /// 实体 person_disable_info
        /// </summary>
        public person_disable_info(){}
        #endregion

        #region 私有变量
        private Int32 _person_disable_info_id = Int32.MinValue;
        private Int32 _person_id = Int32.MinValue;
        private string _begin_time = null;
        private string _end_time = null;
        private Int32 _policy = Int32.MinValue;
        private Int32 _rec_state = Int32.MinValue;
        #endregion

        #region 公共属性
        /// <summary>
        /// 主键 person_disable_info_id(NOT NULL)
        /// </summary>
        public Int32 person_disable_info_id
        {
            set{ _person_disable_info_id=value;}
            get{return _person_disable_info_id;}
        }
        /// <summary>
        /// person_id(NOT NULL)
        /// </summary>
        public Int32 person_id
        {
            set{ _person_id=value;}
            get{return _person_id;}
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
        /// policy
        /// </summary>
        public Int32 policy
        {
            set{ _policy=value;}
            get{return _policy;}
        }
        /// <summary>
        /// rec_state
        /// </summary>
        public Int32 rec_state
        {
            set{ _rec_state=value;}
            get{return _rec_state;}
        }
        #endregion
	}

    /// <summary>
    /// person_disable_info实体集
    /// </summary>
    
    public class person_disable_infoS : CollectionBase
    {
        #region 构造函数
        /// <summary>
        /// person_disable_info实体集
        /// </summary>
        public person_disable_infoS(){}
        #endregion

        #region 属性方法
        /// <summary>
        /// person_disable_info集合 增加方法
        /// </summary>
        public void Add(person_disable_info entity)
        {
            this.List.Add(entity);
        }
        /// <summary>
        /// person_disable_info集合 索引
        /// </summary>
        public person_disable_info this[int index]
        {
            get { return (person_disable_info)this.List[index]; }
            set { this.List[index] = value; }
        }
        #endregion
    }
}
