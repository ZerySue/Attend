/******************************************
* 模块名称：实体 system_param
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
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.ServiceModel.DomainServices.Hosting;
using System.ServiceModel.DomainServices.Server;
using System.Runtime.Serialization;

namespace Irisking.Web.DataModel
{
	/// <summary>
	/// 实体 system_param
	/// </summary>
    
	public class system_param
	{
        #region 构造函数
        /// <summary>
        /// 实体 system_param
        /// </summary>
        public system_param(){}
        #endregion

        #region 私有变量
        private Int32 _auto_backup_type = Int32.MinValue;
        private string _auto_backup_time = null;
        private Int32 _week_auto_backup = Int32.MinValue;
        private Int32 _day_auto_backup = Int32.MinValue;
        private string _auto_backup_path = null;
        private string _styles_sn = null;
        private string _device = null;
        private short _over_time = short.MaxValue;
        private short _alert_time = short.MaxValue;
        private short _accident_time = short.MaxValue;
        private short _is_register = short.MaxValue;
        private string _class_type_sn = null;
        private short _dup_time = short.MaxValue;
        private short _in_dup_recog = short.MaxValue;
        private short _out_dup_recog = short.MaxValue;
        private string _finance_begin_cal = null;
        private short _previous_night = short.MaxValue;
        private short _tertian_type = short.MaxValue;
        private short _scan_sound_time = short.MaxValue;
        private short _phonate_time = short.MaxValue;
        private Int32 _sound_config = Int32.MinValue;
        private Int32 _is_multipled = Int32.MinValue;
        private short _true_accident_time = short.MaxValue;
        private short _auto_attend_time = short.MaxValue;
        private short _attend_max_early = short.MaxValue;
        private short _attend_max_minutes = short.MaxValue;
        #endregion

        #region 公共属性
        /// <summary>
        /// auto_backup_type
        /// </summary>
        [Key]
        public Int32 auto_backup_type
        {
            set{ _auto_backup_type=value;}
            get{return _auto_backup_type;}
        }
        /// <summary>
        /// auto_backup_time
        /// </summary>
        public string auto_backup_time
        {
            set{ _auto_backup_time=value;}
            get{return _auto_backup_time;}
        }
        /// <summary>
        /// week_auto_backup
        /// </summary>
        public Int32 week_auto_backup
        {
            set{ _week_auto_backup=value;}
            get{return _week_auto_backup;}
        }
        /// <summary>
        /// day_auto_backup
        /// </summary>
        public Int32 day_auto_backup
        {
            set{ _day_auto_backup=value;}
            get{return _day_auto_backup;}
        }
        /// <summary>
        /// auto_backup_path
        /// </summary>
        public string auto_backup_path
        {
            set{ _auto_backup_path=value;}
            get{return _auto_backup_path;}
        }
        /// <summary>
        /// styles_sn
        /// </summary>
        public string styles_sn
        {
            set{ _styles_sn=value;}
            get{return _styles_sn;}
        }
        /// <summary>
        /// device
        /// </summary>
        public string device
        {
            set{ _device=value;}
            get{return _device;}
        }
        /// <summary>
        /// over_time
        /// </summary>
        public short over_time
        {
            set{ _over_time=value;}
            get{return _over_time;}
        }
        /// <summary>
        /// alert_time
        /// </summary>
        public short alert_time
        {
            set{ _alert_time=value;}
            get{return _alert_time;}
        }
        /// <summary>
        /// accident_time
        /// </summary>
        public short accident_time
        {
            set{ _accident_time=value;}
            get{return _accident_time;}
        }
        /// <summary>
        /// is_register
        /// </summary>
        public short is_register
        {
            set{ _is_register=value;}
            get{return _is_register;}
        }
        /// <summary>
        /// class_type_sn
        /// </summary>
        public string class_type_sn
        {
            set{ _class_type_sn=value;}
            get{return _class_type_sn;}
        }
        /// <summary>
        /// dup_time
        /// </summary>
        public short dup_time
        {
            set{ _dup_time=value;}
            get{return _dup_time;}
        }
        /// <summary>
        /// in_dup_recog
        /// </summary>
        public short in_dup_recog
        {
            set{ _in_dup_recog=value;}
            get{return _in_dup_recog;}
        }
        /// <summary>
        /// out_dup_recog
        /// </summary>
        public short out_dup_recog
        {
            set{ _out_dup_recog=value;}
            get{return _out_dup_recog;}
        }
        /// <summary>
        /// finance_begin_cal
        /// </summary>
        public string finance_begin_cal
        {
            set{ _finance_begin_cal=value;}
            get{return _finance_begin_cal;}
        }
        /// <summary>
        /// previous_night
        /// </summary>
        public short previous_night
        {
            set{ _previous_night=value;}
            get{return _previous_night;}
        }
        /// <summary>
        /// tertian_type(NOT NULL)
        /// </summary>
        public short tertian_type
        {
            set{ _tertian_type=value;}
            get{return _tertian_type;}
        }
        /// <summary>
        /// scan_sound_time
        /// </summary>
        public short scan_sound_time
        {
            set{ _scan_sound_time=value;}
            get{return _scan_sound_time;}
        }
        /// <summary>
        /// phonate_time
        /// </summary>
        public short phonate_time
        {
            set{ _phonate_time=value;}
            get{return _phonate_time;}
        }
        /// <summary>
        /// sound_config
        /// </summary>
        public Int32 sound_config
        {
            set{ _sound_config=value;}
            get{return _sound_config;}
        }
        /// <summary>
        /// is_multipled
        /// </summary>
        public Int32 is_multipled
        {
            set{ _is_multipled=value;}
            get{return _is_multipled;}
        }
        /// <summary>
        /// true_accident_time
        /// </summary>
        public short true_accident_time
        {
            set{ _true_accident_time=value;}
            get{return _true_accident_time;}
        }
        /// <summary>
        /// auto_attend_time
        /// </summary>
        public short auto_attend_time
        {
            set{ _auto_attend_time=value;}
            get{return _auto_attend_time;}
        }
        /// <summary>
        /// attend_max_early
        /// </summary>
        public short attend_max_early
        {
            set{ _attend_max_early=value;}
            get{return _attend_max_early;}
        }
        /// <summary>
        /// attend_max_minutes
        /// </summary>
        public short attend_max_minutes
        {
            set{ _attend_max_minutes=value;}
            get{return _attend_max_minutes;}
        }
        #endregion
	}

    /// <summary>
    /// system_param实体集
    /// </summary>
    
    public class system_paramS : CollectionBase
    {
        #region 构造函数
        /// <summary>
        /// system_param实体集
        /// </summary>
        public system_paramS(){}
        #endregion

        #region 属性方法
        /// <summary>
        /// system_param集合 增加方法
        /// </summary>
        public void Add(system_param entity)
        {
            this.List.Add(entity);
        }
        /// <summary>
        /// system_param集合 索引
        /// </summary>
        public system_param this[int index]
        {
            get { return (system_param)this.List[index]; }
            set { this.List[index] = value; }
        }
        #endregion
    }
}
