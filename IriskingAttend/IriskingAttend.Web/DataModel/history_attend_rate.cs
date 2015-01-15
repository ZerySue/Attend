/******************************************
* 模块名称：实体 history_attend_rate
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
	/// 实体 history_attend_rate
	/// </summary>
    
	public class history_attend_rate
	{
        #region 构造函数
        /// <summary>
        /// 实体 history_attend_rate
        /// </summary>
        public history_attend_rate(){}
        #endregion

        #region 私有变量
        private Int32 _history_attend_id = Int32.MinValue;
        private Int32 _depart_id = Int32.MinValue;
        private string _depart_name = null;
        private string _history_date = null;
        private Int32 _percent = Int32.MinValue;
        private Int32 _fact_times = Int32.MinValue;
        private Int32 _ougt_times = Int32.MinValue;
        private string _memo = null;
        #endregion

        #region 公共属性
        /// <summary>
        /// 主键 history_attend_id(NOT NULL)
        /// </summary>
        [Key]
        public Int32 history_attend_id
        {
            set{ _history_attend_id=value;}
            get{return _history_attend_id;}
        }
        /// <summary>
        /// depart_id
        /// </summary>
        public Int32 depart_id
        {
            set{ _depart_id=value;}
            get{return _depart_id;}
        }
        /// <summary>
        /// depart_name
        /// </summary>
        public string depart_name
        {
            set{ _depart_name=value;}
            get{return _depart_name;}
        }
        /// <summary>
        /// history_date(NOT NULL)
        /// </summary>
        public string history_date
        {
            set{ _history_date=value;}
            get{return _history_date;}
        }
        /// <summary>
        /// percent(NOT NULL)
        /// </summary>
        public Int32 percent
        {
            set{ _percent=value;}
            get{return _percent;}
        }
        /// <summary>
        /// fact_times(NOT NULL)
        /// </summary>
        public Int32 fact_times
        {
            set{ _fact_times=value;}
            get{return _fact_times;}
        }
        /// <summary>
        /// ougt_times(NOT NULL)
        /// </summary>
        public Int32 ougt_times
        {
            set{ _ougt_times=value;}
            get{return _ougt_times;}
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
    /// history_attend_rate实体集
    /// </summary>
    
    public class history_attend_rateS : CollectionBase
    {
        #region 构造函数
        /// <summary>
        /// history_attend_rate实体集
        /// </summary>
        public history_attend_rateS(){}
        #endregion

        #region 属性方法
        /// <summary>
        /// history_attend_rate集合 增加方法
        /// </summary>
        public void Add(history_attend_rate entity)
        {
            this.List.Add(entity);
        }
        /// <summary>
        /// history_attend_rate集合 索引
        /// </summary>
        public history_attend_rate this[int index]
        {
            get { return (history_attend_rate)this.List[index]; }
            set { this.List[index] = value; }
        }
        #endregion
    }
}
