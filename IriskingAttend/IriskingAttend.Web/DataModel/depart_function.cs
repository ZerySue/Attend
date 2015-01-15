/******************************************
* 模块名称：实体 depart_function
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
	/// 实体 depart_function
	/// </summary>
    
	public class depart_function
	{
        #region 构造函数
        /// <summary>
        /// 实体 depart_function
        /// </summary>
        public depart_function(){}
        #endregion

        #region 私有变量
        private Int32 _depart_function_id = Int32.MinValue;
        private string _depart_function_name = null;
        private short _depart_function_class = short.MaxValue;
        private string _memo = null;
        private string _create_time = null;
        private string _update_time = null;
        #endregion

        #region 公共属性
        /// <summary>
        /// 主键 depart_function_id(NOT NULL)
        /// </summary>
        public Int32 depart_function_id
        {
            set{ _depart_function_id=value;}
            get{return _depart_function_id;}
        }
        /// <summary>
        /// depart_function_name(NOT NULL)
        /// </summary>
        public string depart_function_name
        {
            set{ _depart_function_name=value;}
            get{return _depart_function_name;}
        }
        /// <summary>
        /// depart_function_class
        /// </summary>
        public short depart_function_class
        {
            set{ _depart_function_class=value;}
            get{return _depart_function_class;}
        }
        /// <summary>
        /// memo
        /// </summary>
        public string memo
        {
            set{ _memo=value;}
            get{return _memo;}
        }
        /// <summary>
        /// create_time
        /// </summary>
        public string create_time
        {
            set{ _create_time=value;}
            get{return _create_time;}
        }
        /// <summary>
        /// update_time
        /// </summary>
        public string update_time
        {
            set{ _update_time=value;}
            get{return _update_time;}
        }
        #endregion
	}

    /// <summary>
    /// depart_function实体集
    /// </summary>
    
    public class depart_functionS : CollectionBase
    {
        #region 构造函数
        /// <summary>
        /// depart_function实体集
        /// </summary>
        public depart_functionS(){}
        #endregion

        #region 属性方法
        /// <summary>
        /// depart_function集合 增加方法
        /// </summary>
        public void Add(depart_function entity)
        {
            this.List.Add(entity);
        }
        /// <summary>
        /// depart_function集合 索引
        /// </summary>
        public depart_function this[int index]
        {
            get { return (depart_function)this.List[index]; }
            set { this.List[index] = value; }
        }
        #endregion
    }
}
