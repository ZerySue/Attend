/******************************************
* 模块名称：实体 person_base
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
	/// 实体 person_base
	/// </summary>  
	public class person_base
	{
        #region 构造函数
        /// <summary>
        /// 实体 person_base
        /// </summary>
        public person_base(){}
        #endregion

        #region 私有变量
        private Int32 _person_id = Int32.MinValue;
        private string _work_sn = null;
        private string _name = null;
        private Int32 _sex = Int32.MinValue;
        private string _blood_type = null;
        private string _birthdate = null;
        private string _workday = null;
        private string _id_card = null;
        private string _phone = null;
        private string _address = null;
        private string _zipcode = null;
        private string _email = null;
        private Int32 _depart_id = Int32.MinValue;
        private Int32 _principal_id = Int32.MinValue;
        private Int32 _work_type_id = Int32.MinValue;
        private Int32 _person_work_place_id = Int32.MinValue;
        private Int32 _lunch_subsidy_type_id = Int32.MinValue;
        private Int32 _class_type_id = Int32.MinValue;
        private Int32 _class_type_id_on_ground = Int32.MinValue;
        private Int32 _technical_id = Int32.MinValue;
        private Int32 _employee_type_id = Int32.MinValue;
        private Int32 _locate_card_valid = Int32.MinValue;
        private Int32 _status = Int32.MinValue;
        private Int32 _min_times_per_month = Int32.MinValue;
        private string _delete_time = null;
        private string _create_time = null;
        private string _update_time = null;
        private string _memo = null;
        private Int32 _attr = Int32.MinValue;
        private string _owner_key = null;
        #endregion

        #region 公共属性
        /// <summary>
        /// 主键 person_id(NOT NULL)
        /// </summary>
        public Int32 person_id
        {
            set{ _person_id=value;}
            get{return _person_id;}
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
        /// name
        /// </summary>
        public string name
        {
            set{ _name=value;}
            get{return _name;}
        }
        /// <summary>
        /// sex
        /// </summary>
        public Int32 sex
        {
            set{ _sex=value;}
            get{return _sex;}
        }
        /// <summary>
        /// blood_type
        /// </summary>
        public string blood_type
        {
            set{ _blood_type=value;}
            get{return _blood_type;}
        }
        /// <summary>
        /// birthdate
        /// </summary>
        public string birthdate
        {
            set{ _birthdate=value;}
            get{return _birthdate;}
        }
        /// <summary>
        /// workday
        /// </summary>
        public string workday
        {
            set{ _workday=value;}
            get{return _workday;}
        }
        /// <summary>
        /// id_card
        /// </summary>
        public string id_card
        {
            set{ _id_card=value;}
            get{return _id_card;}
        }
        /// <summary>
        /// phone
        /// </summary>
        public string phone
        {
            set{ _phone=value;}
            get{return _phone;}
        }
        /// <summary>
        /// address
        /// </summary>
        public string address
        {
            set{ _address=value;}
            get{return _address;}
        }
        /// <summary>
        /// zipcode
        /// </summary>
        public string zipcode
        {
            set{ _zipcode=value;}
            get{return _zipcode;}
        }
        /// <summary>
        /// email
        /// </summary>
        public string email
        {
            set{ _email=value;}
            get{return _email;}
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
        /// principal_id
        /// </summary>
        public Int32 principal_id
        {
            set{ _principal_id=value;}
            get{return _principal_id;}
        }
        /// <summary>
        /// work_type_id
        /// </summary>
        public Int32 work_type_id
        {
            set{ _work_type_id=value;}
            get{return _work_type_id;}
        }
        /// <summary>
        /// person_work_place_id
        /// </summary>
        public Int32 person_work_place_id
        {
            set{ _person_work_place_id=value;}
            get{return _person_work_place_id;}
        }
        /// <summary>
        /// lunch_subsidy_type_id
        /// </summary>
        public Int32 lunch_subsidy_type_id
        {
            set{ _lunch_subsidy_type_id=value;}
            get{return _lunch_subsidy_type_id;}
        }
        /// <summary>
        /// class_type_id
        /// </summary>
        public Int32 class_type_id
        {
            set{ _class_type_id=value;}
            get{return _class_type_id;}
        }
        /// <summary>
        /// class_type_id_on_ground
        /// </summary>
        public Int32 class_type_id_on_ground
        {
            set{ _class_type_id_on_ground=value;}
            get{return _class_type_id_on_ground;}
        }
        /// <summary>
        /// technical_id
        /// </summary>
        public Int32 technical_id
        {
            set{ _technical_id=value;}
            get{return _technical_id;}
        }
        /// <summary>
        /// employee_type_id
        /// </summary>
        public Int32 employee_type_id
        {
            set{ _employee_type_id=value;}
            get{return _employee_type_id;}
        }
        /// <summary>
        /// locate_card_valid
        /// </summary>
        public Int32 locate_card_valid
        {
            set{ _locate_card_valid=value;}
            get{return _locate_card_valid;}
        }
        /// <summary>
        /// status(NOT NULL)
        /// </summary>
        public Int32 status
        {
            set{ _status=value;}
            get{return _status;}
        }
        /// <summary>
        /// min_times_per_month
        /// </summary>
        public Int32 min_times_per_month
        {
            set{ _min_times_per_month=value;}
            get{return _min_times_per_month;}
        }
        /// <summary>
        /// delete_time
        /// </summary>
        public string delete_time
        {
            set{ _delete_time=value;}
            get{return _delete_time;}
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
        /// <summary>
        /// attr
        /// </summary>
        public Int32 attr
        {
            set{ _attr=value;}
            get{return _attr;}
        }
        /// <summary>
        /// owner_key
        /// </summary>
        public string owner_key
        {
            set{ _owner_key=value;}
            get{return _owner_key;}
        }
        #endregion
	}

    /// <summary>
    /// person_base实体集
    /// </summary>
    
    public class person_baseS : CollectionBase
    {
        #region 构造函数
        /// <summary>
        /// person_base实体集
        /// </summary>
        public person_baseS(){}
        #endregion

        #region 属性方法
        /// <summary>
        /// person_base集合 增加方法
        /// </summary>
        public void Add(person_base entity)
        {
            this.List.Add(entity);
        }
        /// <summary>
        /// person_base集合 索引
        /// </summary>
        public person_base this[int index]
        {
            get { return (person_base)this.List[index]; }
            set { this.List[index] = value; }
        }
        #endregion
    }
}
