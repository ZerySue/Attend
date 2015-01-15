/******************************************
* 模块名称：实体 purview
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
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.ServiceModel.DomainServices.Hosting;
using System.ServiceModel.DomainServices.Server;
using System.Runtime.Serialization;

namespace Irisking.Web.DataModel
{
	/// <summary>
	/// 实体 purview
	/// </summary>
    
	public class purview
	{
        #region 构造函数
        /// <summary>
        /// 实体 purview
        /// </summary>
        public purview(){}
        #endregion

        #region 私有变量
        private Int32 _purview_id = Int32.MinValue;
        private Int32 _parent_purview_id = Int32.MinValue;
        private string _purview_name = null;
        private string _memo = null;        
        #endregion

        #region 公共属性
        /// <summary>
        /// 主键 purview_id(NOT NULL)
        /// </summary>
        [Key]
        public Int32 purview_id
        {
            set{ _purview_id=value;}
            get{return _purview_id;}
        }
        /// <summary>
        /// parent_purview_id
        /// </summary>
        public Int32 parent_purview_id
        {
            set{ _parent_purview_id=value;}
            get{return _parent_purview_id;}
        }
        /// <summary>
        /// purview_name
        /// </summary>
        public string purview_name
        {
            set{ _purview_name=value;}
            get{return _purview_name;}
        }
        /// <summary>
        /// memo
        /// </summary>
        public string memo
        {
            set { _memo = value; }
            get { return _memo; }
        }        
        #endregion
	}

    /// <summary>
    /// purview实体集
    /// </summary>
    
    public class purviewS : CollectionBase
    {
        #region 构造函数
        /// <summary>
        /// purview实体集
        /// </summary>
        public purviewS(){}
        #endregion

        #region 属性方法
        /// <summary>
        /// purview集合 增加方法
        /// </summary>
        public void Add(purview entity)
        {
            this.List.Add(entity);
        }
        /// <summary>
        /// purview集合 索引
        /// </summary>
        public purview this[int index]
        {
            get { return (purview)this.List[index]; }
            set { this.List[index] = value; }
        }
        #endregion
    }
}
