/******************************************
* 模块名称：实体 lunch_subsidy_type
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
	/// 实体 lunch_subsidy_type
	/// </summary>

    
	public class lunch_subsidy_type
	{
        #region 构造函数
        /// <summary>
        /// 实体 lunch_subsidy_type
        /// </summary>
        public lunch_subsidy_type(){}
        #endregion

        #region 私有变量
        private Int32 _lunch_subsidy_type_id = Int32.MinValue;
        private string _lunch_subsidy_type_name = null;
        private Int32 _lunch_subsidy_money = Int32.MinValue;
        private Int32 _lunch_subsidy_class = Int32.MinValue;
        private Int32 _delete_info = Int32.MinValue;
        private string _create_time = null;
        private string _update_time = null;
        private string _memo = null;
        #endregion

        #region 公共属性
        /// <summary>
        /// 主键 lunch_subsidy_type_id(NOT NULL)
        /// </summary>
        public Int32 lunch_subsidy_type_id
        {
            set{ _lunch_subsidy_type_id=value;}
            get{return _lunch_subsidy_type_id;}
        }
        /// <summary>
        /// lunch_subsidy_type_name(NOT NULL)
        /// </summary>
        public string lunch_subsidy_type_name
        {
            set{ _lunch_subsidy_type_name=value;}
            get{return _lunch_subsidy_type_name;}
        }
        /// <summary>
        /// lunch_subsidy_money
        /// </summary>
        public Int32 lunch_subsidy_money
        {
            set{ _lunch_subsidy_money=value;}
            get{return _lunch_subsidy_money;}
        }
        /// <summary>
        /// lunch_subsidy_class
        /// </summary>
        public Int32 lunch_subsidy_class
        {
            set{ _lunch_subsidy_class=value;}
            get{return _lunch_subsidy_class;}
        }
        /// <summary>
        /// delete_info
        /// </summary>
        public Int32 delete_info
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
    /// lunch_subsidy_type实体集
    /// </summary>
    
    public class lunch_subsidy_typeS : CollectionBase
    {
        #region 构造函数
        /// <summary>
        /// lunch_subsidy_type实体集
        /// </summary>
        public lunch_subsidy_typeS(){}
        #endregion

        #region 属性方法
        /// <summary>
        /// lunch_subsidy_type集合 增加方法
        /// </summary>
        public void Add(lunch_subsidy_type entity)
        {
            this.List.Add(entity);
        }
        /// <summary>
        /// lunch_subsidy_type集合 索引
        /// </summary>
        public lunch_subsidy_type this[int index]
        {
            get { return (lunch_subsidy_type)this.List[index]; }
            set { this.List[index] = value; }
        }
        #endregion
    }
}
