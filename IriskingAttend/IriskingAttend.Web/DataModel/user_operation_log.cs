/******************************************
* 模块名称：实体 user_operation_log
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
	/// 实体 user_operation_log
	/// </summary>
	
    
	public class user_operation_log
	{
        #region 构造函数
        /// <summary>
        /// 实体 user_operation_log
        /// </summary>
        public user_operation_log(){}
        #endregion

        #region 私有变量
        private Int32 _user_operation_log_id = Int32.MinValue;
        private Int32 _user_id = Int32.MinValue;
        private string _user_name = null;
        private string _user_ip = null;
        private string _operation_time = null;
        private string _content = null;
        private short _result = short.MaxValue;
        private string _description = null;
        #endregion

        #region 公共属性
        /// <summary>
        /// user_operation_log_id(NOT NULL)
        /// </summary>
        public Int32 user_operation_log_id
        {
            set{ _user_operation_log_id=value;}
            get{return _user_operation_log_id;}
        }
        /// <summary>
        /// user_id
        /// </summary>
        public Int32 user_id
        {
            set{ _user_id=value;}
            get{return _user_id;}
        }
        /// <summary>
        /// user_name
        /// </summary>
        public string user_name
        {
            set{ _user_name=value;}
            get{return _user_name;}
        }
        /// <summary>
        /// user_ip
        /// </summary>
        public string user_ip
        {
            set{ _user_ip=value;}
            get{return _user_ip;}
        }
        /// <summary>
        /// operation_time(NOT NULL)
        /// </summary>
        public string operation_time
        {
            set{ _operation_time=value;}
            get{return _operation_time;}
        }
        /// <summary>
        /// content
        /// </summary>
        public string content
        {
            set{ _content=value;}
            get{return _content;}
        }
        /// <summary>
        /// result
        /// </summary>
        public short result
        {
            set{ _result=value;}
            get{return _result;}
        }
        /// <summary>
        /// description
        /// </summary>
        public string description
        {
            set{ _description=value;}
            get{return _description;}
        }
        #endregion
	}

    /// <summary>
    /// user_operation_log实体集
    /// </summary>
    
    public class user_operation_logS : CollectionBase
    {
        #region 构造函数
        /// <summary>
        /// user_operation_log实体集
        /// </summary>
        public user_operation_logS(){}
        #endregion

        #region 属性方法
        /// <summary>
        /// user_operation_log集合 增加方法
        /// </summary>
        public void Add(user_operation_log entity)
        {
            this.List.Add(entity);
        }
        /// <summary>
        /// user_operation_log集合 索引
        /// </summary>
        public user_operation_log this[int index]
        {
            get { return (user_operation_log)this.List[index]; }
            set { this.List[index] = value; }
        }
        #endregion
    }
}
