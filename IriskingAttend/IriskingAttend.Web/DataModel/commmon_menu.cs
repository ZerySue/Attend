/******************************************
* 模块名称：实体 commmon_menu
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
	/// 实体 commmon_menu
	/// </summary>

    
	public class commmon_menu
	{
        #region 构造函数
        /// <summary>
        /// 实体 commmon_menu
        /// </summary>
        public commmon_menu(){}
        #endregion

        #region 私有变量
        private Int32 _commmon_menu_id = Int32.MinValue;
        private Int32 _operator_id = Int32.MinValue;
        private string _menu_name = null;
        private string _menu_link = null;
        #endregion

        #region 公共属性
        /// <summary>
        /// 主键 commmon_menu_id(NOT NULL)
        /// </summary>
        public Int32 commmon_menu_id
        {
            set{ _commmon_menu_id=value;}
            get{return _commmon_menu_id;}
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
        /// menu_name
        /// </summary>
        public string menu_name
        {
            set{ _menu_name=value;}
            get{return _menu_name;}
        }
        /// <summary>
        /// menu_link
        /// </summary>
        public string menu_link
        {
            set{ _menu_link=value;}
            get{return _menu_link;}
        }
        #endregion
	}

    /// <summary>
    /// commmon_menu实体集
    /// </summary>
    
    public class commmon_menuS : CollectionBase
    {
        #region 构造函数
        /// <summary>
        /// commmon_menu实体集
        /// </summary>
        public commmon_menuS(){}
        #endregion

        #region 属性方法
        /// <summary>
        /// commmon_menu集合 增加方法
        /// </summary>
        public void Add(commmon_menu entity)
        {
            this.List.Add(entity);
        }
        /// <summary>
        /// commmon_menu集合 索引
        /// </summary>
        public commmon_menu this[int index]
        {
            get { return (commmon_menu)this.List[index]; }
            set { this.List[index] = value; }
        }
        #endregion
    }
}
