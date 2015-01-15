/******************************************
* 模块名称：实体 employee_type
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
	/// 实体 employee_type
	/// </summary>
    
	public class employee_type
	{
        #region 构造函数
        /// <summary>
        /// 实体 employee_type
        /// </summary>
        public employee_type(){}
        #endregion

        #region 私有变量
        private Int32 _employee_type_id = Int32.MinValue;
        private string _employee_type_name = null;
        private short _employee_type_class = short.MaxValue;
        private short _delete_info = short.MaxValue;
        private string _create_time = null;
        private string _update_time = null;
        private string _memo = null;
        #endregion

        #region 公共属性
        /// <summary>
        /// 主键 employee_type_id(NOT NULL)
        /// </summary>
        public Int32 employee_type_id
        {
            set{ _employee_type_id=value;}
            get{return _employee_type_id;}
        }
        /// <summary>
        /// employee_type_name(NOT NULL)
        /// </summary>
        public string employee_type_name
        {
            set{ _employee_type_name=value;}
            get{return _employee_type_name;}
        }
        /// <summary>
        /// employee_type_class
        /// </summary>
        public short employee_type_class
        {
            set{ _employee_type_class=value;}
            get{return _employee_type_class;}
        }
        /// <summary>
        /// delete_info
        /// </summary>
        public short delete_info
        {
            set{ _delete_info=value;}
            get{return _delete_info;}
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
    /// employee_type实体集
    /// </summary>
    
    public class employee_typeS : CollectionBase
    {
        #region 构造函数
        /// <summary>
        /// employee_type实体集
        /// </summary>
        public employee_typeS(){}
        #endregion

        #region 属性方法
        /// <summary>
        /// employee_type集合 增加方法
        /// </summary>
        public void Add(employee_type entity)
        {
            this.List.Add(entity);
        }
        /// <summary>
        /// employee_type集合 索引
        /// </summary>
        public employee_type this[int index]
        {
            get { return (employee_type)this.List[index]; }
            set { this.List[index] = value; }
        }
        #endregion
    }
}
