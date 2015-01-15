/******************************************
* 模块名称：实体 operator_purview
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
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.ServiceModel.DomainServices.Hosting;
using System.ServiceModel.DomainServices.Server;
using System.Runtime.Serialization;

namespace Irisking.Web.DataModel
{
	/// <summary>
	/// 实体 operator_purview
	/// </summary>

    
	public class operator_purview
	{
        #region 构造函数
        /// <summary>
        /// 实体 operator_purview
        /// </summary>
        public operator_purview(){}
        #endregion

        #region 私有变量
        private Int32 _oper_purv_id = Int32.MinValue;
        private Int32 _operator_id = Int32.MinValue;
        private Int32 _purview_id = Int32.MinValue;
        #endregion

        #region 公共属性
        /// <summary>
        /// 主键 oper_purv_id(NOT NULL)
        /// </summary>
        [Key]
        public Int32 oper_purv_id
        {
            set{ _oper_purv_id=value;}
            get{return _oper_purv_id;}
        }
        /// <summary>
        /// operator_id
        /// </summary>
        public Int32 operator_id
        {
            set{ _operator_id=value;}
            get{return _operator_id;}
        }
        /// <summary>
        /// purview_id
        /// </summary>
        public Int32 purview_id
        {
            set{ _purview_id=value;}
            get{return _purview_id;}
        }
        #endregion
	}

    /// <summary>
    /// operator_purview实体集
    /// </summary>
    
    public class operator_purviewS : CollectionBase
    {
        #region 构造函数
        /// <summary>
        /// operator_purview实体集
        /// </summary>
        public operator_purviewS(){}
        #endregion

        #region 属性方法
        /// <summary>
        /// operator_purview集合 增加方法
        /// </summary>
        public void Add(operator_purview entity)
        {
            this.List.Add(entity);
        }
        /// <summary>
        /// operator_purview集合 索引
        /// </summary>
        public operator_purview this[int index]
        {
            get { return (operator_purview)this.List[index]; }
            set { this.List[index] = value; }
        }
        #endregion
    }
}
