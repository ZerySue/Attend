/******************************************
* 模块名称：实体 person_enroll_info
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
	/// 实体 person_enroll_info
	/// </summary>
    
	public class person_enroll_info
	{
        #region 构造函数
        /// <summary>
        /// 实体 person_enroll_info
        /// </summary>
        public person_enroll_info(){}
        #endregion

        #region 私有变量
        private Int32 _person_id = Int32.MinValue;
        private Int32 _eye_flag = Int32.MinValue;
        private string _enroll_time = null;
        #endregion

        #region 公共属性
        /// <summary>
        /// person_id(NOT NULL)
        /// </summary>
        public Int32 person_id
        {
            set{ _person_id=value;}
            get{return _person_id;}
        }
        /// <summary>
        /// eye_flag(NOT NULL)
        /// </summary>
        public Int32 eye_flag
        {
            set{ _eye_flag=value;}
            get{return _eye_flag;}
        }
        /// <summary>
        /// enroll_time(NOT NULL)
        /// </summary>
        public string enroll_time
        {
            set{ _enroll_time=value;}
            get{return _enroll_time;}
        }
        #endregion
	}

    /// <summary>
    /// person_enroll_info实体集
    /// </summary>
    
    public class person_enroll_infoS : CollectionBase
    {
        #region 构造函数
        /// <summary>
        /// person_enroll_info实体集
        /// </summary>
        public person_enroll_infoS(){}
        #endregion

        #region 属性方法
        /// <summary>
        /// person_enroll_info集合 增加方法
        /// </summary>
        public void Add(person_enroll_info entity)
        {
            this.List.Add(entity);
        }
        /// <summary>
        /// person_enroll_info集合 索引
        /// </summary>
        public person_enroll_info this[int index]
        {
            get { return (person_enroll_info)this.List[index]; }
            set { this.List[index] = value; }
        }
        #endregion
    }
}
