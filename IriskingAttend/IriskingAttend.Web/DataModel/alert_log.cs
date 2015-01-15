/******************************************
* 模块名称：实体 alert_log
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
    //支持KEY
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.ServiceModel.DomainServices.Hosting;
using System.ServiceModel.DomainServices.Server;
using System.Runtime.Serialization;
	/// <summary>
	/// 实体 alert_log
	/// </summary> 
	public class alert_log
	{
        #region 构造函数
        /// <summary>
        /// 实体 alert_log
        /// </summary>
        public alert_log(){}
        #endregion

        #region 私有变量
        private Int32 _alert_log_id = Int32.MinValue;
        private string _person_sn = null;
        private string _name = null;
        private string _work_sn = null;
        private string _depart_name = null;
        private string _tele = null;
        private string _recog_time = null;
        private string _at_position = null;
        private string _logname = null;
        private string _ip = null;
        private string _operat_time = null;
        private string _memo = null;
        #endregion

        #region 公共属性
        /// <summary>
        /// 主键 alert_log_id(NOT NULL)
        /// </summary>
       [Key]
        public Int32 alert_log_id
        {
            set{ _alert_log_id=value;}
            get{return _alert_log_id;}
        }
        /// <summary>
        /// person_sn(NOT NULL)
        /// </summary>
        public string person_sn
        {
            set{ _person_sn=value;}
            get{return _person_sn;}
        }
        /// <summary>
        /// name
        /// </summary>
        public string name
        {
            set{ _name=value;}
            get{return _name;}
        }
        /// <summary>
        /// work_sn
        /// </summary>
        public string work_sn
        {
            set{ _work_sn=value;}
            get{return _work_sn;}
        }
        /// <summary>
        /// depart_name(NOT NULL)
        /// </summary>
        public string depart_name
        {
            set{ _depart_name=value;}
            get{return _depart_name;}
        }
        /// <summary>
        /// tele
        /// </summary>
        public string tele
        {
            set{ _tele=value;}
            get{return _tele;}
        }
        /// <summary>
        /// recog_time(NOT NULL)
        /// </summary>
        public string recog_time
        {
            set{ _recog_time=value;}
            get{return _recog_time;}
        }
        /// <summary>
        /// at_position(NOT NULL)
        /// </summary>
        public string at_position
        {
            set{ _at_position=value;}
            get{return _at_position;}
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
        /// ip(NOT NULL)
        /// </summary>
        public string ip
        {
            set{ _ip=value;}
            get{return _ip;}
        }
        /// <summary>
        /// operat_time(NOT NULL)
        /// </summary>
        public string operat_time
        {
            set{ _operat_time=value;}
            get{return _operat_time;}
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
    /// alert_log实体集
    /// </summary>
    
    public class alert_logS : CollectionBase
    {
        #region 构造函数
        /// <summary>
        /// alert_log实体集
        /// </summary>
        public alert_logS(){}
        #endregion

        #region 属性方法
        /// <summary>
        /// alert_log集合 增加方法
        /// </summary>
        public void Add(alert_log entity)
        {
            this.List.Add(entity);
        }
        /// <summary>
        /// alert_log集合 索引
        /// </summary>
        public alert_log this[int index]
        {
            get { return (alert_log)this.List[index]; }
            set { this.List[index] = value; }
        }
        #endregion
    }
}
