/******************************************
* 模块名称：实体 person_alert
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
	/// 实体 person_alert
	/// </summary>
    
	public class person_alert
	{
        #region 构造函数
        /// <summary>
        /// 实体 person_alert
        /// </summary>
        public person_alert(){}
        #endregion

        #region 私有变量
        private Int32 _person_alert_id = Int32.MinValue;
        private Int32 _person_id = Int32.MinValue;
        private string _person_sn = null;
        private string _name = null;
        private string _work_sn = null;
        private string _depart_sn = null;
        private string _tele = null;
        private short _person_type = short.MaxValue;
        private string _recog_time = null;
        private short _recog_type = short.MaxValue;
        private Int32 _class_order_id = Int32.MinValue;
        private string _at_position = null;
        private string _con_work_date = null;
        private short _cont_work_time = short.MaxValue;
        #endregion

        #region 公共属性
        /// <summary>
        /// 主键 person_alert_id(NOT NULL)
        /// </summary>
        public Int32 person_alert_id
        {
            set{ _person_alert_id=value;}
            get{return _person_alert_id;}
        }
        /// <summary>
        /// person_id(NOT NULL)
        /// </summary>
        public Int32 person_id
        {
            set{ _person_id=value;}
            get{return _person_id;}
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
        /// depart_sn(NOT NULL)
        /// </summary>
        public string depart_sn
        {
            set{ _depart_sn=value;}
            get{return _depart_sn;}
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
        /// person_type(NOT NULL)
        /// </summary>
        public short person_type
        {
            set{ _person_type=value;}
            get{return _person_type;}
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
        /// recog_type(NOT NULL)
        /// </summary>
        public short recog_type
        {
            set{ _recog_type=value;}
            get{return _recog_type;}
        }
        /// <summary>
        /// class_order_id
        /// </summary>
        public Int32 class_order_id
        {
            set{ _class_order_id=value;}
            get{return _class_order_id;}
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
        /// con_work_date
        /// </summary>
        public string con_work_date
        {
            set{ _con_work_date=value;}
            get{return _con_work_date;}
        }
        /// <summary>
        /// cont_work_time
        /// </summary>
        public short cont_work_time
        {
            set{ _cont_work_time=value;}
            get{return _cont_work_time;}
        }
        #endregion
	}

    /// <summary>
    /// person_alert实体集
    /// </summary>
    
    public class person_alertS : CollectionBase
    {
        #region 构造函数
        /// <summary>
        /// person_alert实体集
        /// </summary>
        public person_alertS(){}
        #endregion

        #region 属性方法
        /// <summary>
        /// person_alert集合 增加方法
        /// </summary>
        public void Add(person_alert entity)
        {
            this.List.Add(entity);
        }
        /// <summary>
        /// person_alert集合 索引
        /// </summary>
        public person_alert this[int index]
        {
            get { return (person_alert)this.List[index]; }
            set { this.List[index] = value; }
        }
        #endregion
    }
}
