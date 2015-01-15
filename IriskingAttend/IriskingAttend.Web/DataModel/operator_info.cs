/******************************************
* 模块名称：实体 operator_info
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
	/// 实体 operator_info
	/// </summary>
    
	public class operator_info
	{
        #region 构造函数
        /// <summary>
        /// 实体 operator_info
        /// </summary>
        public operator_info(){}
        #endregion

        #region 私有变量
        private Int32 _operator_id = Int32.MinValue;
        private string _logname = null;
        private string _realityname = null;
        private string _password = null;
        private Int32 _operator_type = Int32.MinValue;
        #endregion

        #region 公共属性
        /// <summary>
        /// 主键 operator_id(NOT NULL)
        /// </summary>
        [Key]
        public Int32 operator_id
        {
            set{ _operator_id=value;}
            get{return _operator_id;}
        }
        /// <summary>
        /// logname(NOT NULL)
        /// </summary>
        public string logname
        {
            set{ _logname=value;}
            get{return _logname;}
        }
        /// <summary>
        /// realityname
        /// </summary>
        public string realityname
        {
            set{ _realityname=value;}
            get{return _realityname;}
        }
        /// <summary>
        /// password
        /// </summary>
        public string password
        {
            set{ _password=value;}
            get{return _password;}
        }
        /// <summary>
        /// operator_type(NOT NULL)
        /// </summary>
        public Int32 operator_type
        {
            set{ _operator_type=value;}
            get{return _operator_type;}
        }
        #endregion
	}

    /// <summary>
    /// operator_info实体集
    /// </summary>
    
    public class operator_infoS : CollectionBase
    {
        #region 构造函数
        /// <summary>
        /// operator_info实体集
        /// </summary>
        public operator_infoS(){}
        #endregion

        #region 属性方法
        /// <summary>
        /// operator_info集合 增加方法
        /// </summary>
        public void Add(operator_info entity)
        {
            this.List.Add(entity);
        }
        /// <summary>
        /// operator_info集合 索引
        /// </summary>
        public operator_info this[int index]
        {
            get { return (operator_info)this.List[index]; }
            set { this.List[index] = value; }
        }
        #endregion
    }
}
