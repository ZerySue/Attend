/******************************************
* 模块名称：实体 operator_potence
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
	/// 实体 operator_potence
	/// </summary>

    
	public class operator_potence
	{
        #region 构造函数
        /// <summary>
        /// 实体 operator_potence
        /// </summary>
        public operator_potence(){}
        #endregion

        #region 私有变量
        private Int32 _operator_potence_id = Int32.MinValue;
        private Int32 _operator_id = Int32.MinValue;
        private Int32 _depart_id = Int32.MinValue;
        #endregion

        #region 公共属性
        /// <summary>
        /// 主键 operator_id
        /// </summary>
        [Key]
        public Int32 operator_potence_id
        {
            set { _operator_potence_id = value; }
            get { return _operator_potence_id; }
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
        /// depart_id(NOT NULL)
        /// </summary>
        public Int32 depart_id
        {
            set{ _depart_id=value;}
            get{return _depart_id;}
        }
        #endregion
	}

    /// <summary>
    /// operator_potence实体集
    /// </summary>
    
    public class operator_potenceS : CollectionBase
    {
        #region 构造函数
        /// <summary>
        /// operator_potence实体集
        /// </summary>
        public operator_potenceS(){}
        #endregion

        #region 属性方法
        /// <summary>
        /// operator_potence集合 增加方法
        /// </summary>
        public void Add(operator_potence entity)
        {
            this.List.Add(entity);
        }
        /// <summary>
        /// operator_potence集合 索引
        /// </summary>
        public operator_potence this[int index]
        {
            get { return (operator_potence)this.List[index]; }
            set { this.List[index] = value; }
        }
        #endregion
    }
}
