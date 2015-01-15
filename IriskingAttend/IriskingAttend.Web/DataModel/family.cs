/******************************************
* 模块名称：实体 family
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
	/// 实体 family
	/// </summary>
    
	public class family
	{
        #region 构造函数
        /// <summary>
        /// 实体 family
        /// </summary>
        public family(){}
        #endregion

        #region 私有变量
        private Int32 _family_id = Int32.MinValue;
        private Int32 _person_id = Int32.MinValue;
        private string _family_name = null;
        private string _relation = null;
        private string _family_phone = null;
        private string _family_address = null;
        private string _zipcode = null;
        private string _memo = null;
        #endregion

        #region 公共属性
        /// <summary>
        /// 主键 family_id(NOT NULL)
        /// </summary>
        public Int32 family_id
        {
            set{ _family_id=value;}
            get{return _family_id;}
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
        /// family_name(NOT NULL)
        /// </summary>
        public string family_name
        {
            set{ _family_name=value;}
            get{return _family_name;}
        }
        /// <summary>
        /// relation
        /// </summary>
        public string relation
        {
            set{ _relation=value;}
            get{return _relation;}
        }
        /// <summary>
        /// family_phone
        /// </summary>
        public string family_phone
        {
            set{ _family_phone=value;}
            get{return _family_phone;}
        }
        /// <summary>
        /// family_address
        /// </summary>
        public string family_address
        {
            set{ _family_address=value;}
            get{return _family_address;}
        }
        /// <summary>
        /// zipcode
        /// </summary>
        public string zipcode
        {
            set{ _zipcode=value;}
            get{return _zipcode;}
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
    /// family实体集
    /// </summary>
    
    public class familyS : CollectionBase
    {
        #region 构造函数
        /// <summary>
        /// family实体集
        /// </summary>
        public familyS(){}
        #endregion

        #region 属性方法
        /// <summary>
        /// family集合 增加方法
        /// </summary>
        public void Add(family entity)
        {
            this.List.Add(entity);
        }
        /// <summary>
        /// family集合 索引
        /// </summary>
        public family this[int index]
        {
            get { return (family)this.List[index]; }
            set { this.List[index] = value; }
        }
        #endregion
    }
}
