/******************************************
* 模块名称：实体 dev_disabled_person
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
	/// 实体 dev_disabled_person
	/// </summary>
	[Description("Primary:dev_sn,person_id")]
    
	public class dev_disabled_person
	{
        #region 构造函数
        /// <summary>
        /// 实体 dev_disabled_person
        /// </summary>
        public dev_disabled_person(){}
        #endregion

        #region 私有变量
        private string _dev_sn = null;
        private Int32 _person_id = Int32.MinValue;
        private string _create_time = null;
        #endregion

        #region 公共属性
        /// <summary>
        /// 主键 dev_sn(NOT NULL)
        /// </summary>
        public string dev_sn
        {
            set{ _dev_sn=value;}
            get{return _dev_sn;}
        }
        /// <summary>
        /// 主键 person_id(NOT NULL)
        /// </summary>
        public Int32 person_id
        {
            set{ _person_id=value;}
            get{return _person_id;}
        }
        /// <summary>
        /// create_time
        /// </summary>
        public string create_time
        {
            set{ _create_time=value;}
            get{return _create_time;}
        }
        #endregion
	}

    /// <summary>
    /// dev_disabled_person实体集
    /// </summary>
    
    public class dev_disabled_personS : CollectionBase
    {
        #region 构造函数
        /// <summary>
        /// dev_disabled_person实体集
        /// </summary>
        public dev_disabled_personS(){}
        #endregion

        #region 属性方法
        /// <summary>
        /// dev_disabled_person集合 增加方法
        /// </summary>
        public void Add(dev_disabled_person entity)
        {
            this.List.Add(entity);
        }
        /// <summary>
        /// dev_disabled_person集合 索引
        /// </summary>
        public dev_disabled_person this[int index]
        {
            get { return (dev_disabled_person)this.List[index]; }
            set { this.List[index] = value; }
        }
        #endregion
    }
}
