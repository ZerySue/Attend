/******************************************
* 模块名称：实体 person_recog_proc_mark
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
	/// 实体 person_recog_proc_mark
	/// </summary>
    
	public class person_recog_proc_mark
	{
        #region 构造函数
        /// <summary>
        /// 实体 person_recog_proc_mark
        /// </summary>
        public person_recog_proc_mark(){}
        #endregion

        #region 私有变量
        private Int32 _person_recog_log_id = Int32.MinValue;
        private Int32 _in_1_out_2 = Int32.MinValue;
        private Int32 _attend_state = Int32.MinValue;
        #endregion

        #region 公共属性
        /// <summary>
        /// 主键 person_recog_log_id(NOT NULL)
        /// </summary>
        [Key]
        public Int32 person_recog_log_id
        {
            set{ _person_recog_log_id=value;}
            get{return _person_recog_log_id;}
        }
        /// <summary>
        /// in_1_out_2
        /// </summary>
        public Int32 in_1_out_2
        {
            set{ _in_1_out_2=value;}
            get{return _in_1_out_2;}
        }
        /// <summary>
        /// attend_state
        /// </summary>
        public Int32 attend_state
        {
            set{ _attend_state=value;}
            get{return _attend_state;}
        }
        #endregion
	}

    /// <summary>
    /// person_recog_proc_mark实体集
    /// </summary>
    
    public class person_recog_proc_markS : CollectionBase
    {
        #region 构造函数
        /// <summary>
        /// person_recog_proc_mark实体集
        /// </summary>
        public person_recog_proc_markS(){}
        #endregion

        #region 属性方法
        /// <summary>
        /// person_recog_proc_mark集合 增加方法
        /// </summary>
        public void Add(person_recog_proc_mark entity)
        {
            this.List.Add(entity);
        }
        /// <summary>
        /// person_recog_proc_mark集合 索引
        /// </summary>
        public person_recog_proc_mark this[int index]
        {
            get { return (person_recog_proc_mark)this.List[index]; }
            set { this.List[index] = value; }
        }
        #endregion
    }
}
