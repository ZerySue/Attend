/******************************************
* 模块名称：实体 base_info
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
	/// 实体 base_info
	/// </summary>    
	public class base_info
	{
        #region 构造函数
        /// <summary>
        /// 实体 base_info
        /// </summary>
        public base_info(){}
        #endregion

        #region 私有变量
        private string _name = null;
        private string _val = null;
        #endregion

        #region 公共属性
        /// <summary>
        /// name(NOT NULL)
        /// </summary>
        public string name
        {
            set{ _name=value;}
            get{return _name;}
        }
        /// <summary>
        /// val
        /// </summary>
        public string val
        {
            set{ _val=value;}
            get{return _val;}
        }
        #endregion
	}

    /// <summary>
    /// base_info实体集
    /// </summary>
    
    public class base_infoS : CollectionBase
    {
        #region 构造函数
        /// <summary>
        /// base_info实体集
        /// </summary>
        public base_infoS(){}
        #endregion

        #region 属性方法
        /// <summary>
        /// base_info集合 增加方法
        /// </summary>
        public void Add(base_info entity)
        {
            this.List.Add(entity);
        }
        /// <summary>
        /// base_info集合 索引
        /// </summary>
        public base_info this[int index]
        {
            get { return (base_info)this.List[index]; }
            set { this.List[index] = value; }
        }
        #endregion
    }
}
