/******************************************
* 模块名称：实体 depart
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
using System.ComponentModel.DataAnnotations;

namespace Irisking.Web.DataModel
{
	/// <summary>
	/// 实体 depart
	/// </summary>
    
	public class depart
	{
        #region 构造函数
        /// <summary>
        /// 实体 depart
        /// </summary>
        public depart(){}
        #endregion

        #region 私有变量
        private Int32 _depart_id = Int32.MinValue;
        private string _company_sn = null;
        private string _depart_sn = null;
        private string _depart_name = null;
        private Int32 _parent_depart_id = Int32.MinValue;
        private string _depart_auth = null;
        private string _depart_director = null;
        private Int32 _depart_function_id = Int32.MinValue;
        private Int32 _depart_work_place_id = Int32.MinValue;
        private string _contact_phone = null;
        private string _delete_time = null;
        private string _create_time = null;
        private string _update_time = null;
        private string _memo = null;
        #endregion

        #region 公共属性
        /// <summary>
        /// 主键 depart_id(NOT NULL)
        /// </summary>
        [Key]
        public Int32 depart_id
        {
            set{ _depart_id=value;}
            get{return _depart_id;}
        }
        /// <summary>
        /// company_sn
        /// </summary>
        public string company_sn
        {
            set{ _company_sn=value;}
            get{return _company_sn;}
        }
        /// <summary>
        /// 部门编号
        /// </summary>
        public string depart_sn
        {
            set{ _depart_sn=value;}
            get{return _depart_sn;}
        }
        /// <summary>
        /// 部门名称
        /// </summary>
        public string depart_name
        {
            set{ _depart_name=value;}
            get{return _depart_name;}
        }
        /// <summary>
        /// 父部门id
        /// </summary>
        public Int32 parent_depart_id
        {
            set{ _parent_depart_id=value;}
            get{return _parent_depart_id;}
        }
        /// <summary>
        /// depart_auth
        /// </summary>
        public string depart_auth
        {
            set{ _depart_auth=value;}
            get{return _depart_auth;}
        }
        /// <summary>
        /// depart_director
        /// </summary>
        public string depart_director
        {
            set{ _depart_director=value;}
            get{return _depart_director;}
        }

        /// <summary>
        /// depart_function_id
        /// </summary>
        public Int32 depart_function_id
        {
            set{ _depart_function_id=value;}
            get{return _depart_function_id;}
        }

        /// <summary>
        /// depart_work_place_id
        /// </summary>
        public Int32 depart_work_place_id
        {
            set{ _depart_work_place_id=value;}
            get{return _depart_work_place_id;}
        }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string contact_phone
        {
            set{ _contact_phone=value;}
            get{return _contact_phone;}
        }
        /// <summary>
        /// 删除时间
        /// </summary>
        public string delete_time
        {
            set{ _delete_time=value;}
            get{return _delete_time;}
        }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string create_time
        {
            set{ _create_time=value;}
            get{return _create_time;}
        }
        /// <summary>
        /// 更新时间
        /// </summary>
        public string update_time
        {
            set{ _update_time=value;}
            get{return _update_time;}
        }
        /// <summary>
        /// 备注
        /// </summary>
        public string memo
        {
            set{ _memo=value;}
            get{return _memo;}
        }
        #endregion
	}

    /// <summary>
    /// depart实体集
    /// </summary>
    
    public class departS : CollectionBase
    {
        #region 构造函数
        /// <summary>
        /// depart实体集
        /// </summary>
        public departS(){}
        #endregion

        #region 属性方法
        /// <summary>
        /// depart集合 增加方法
        /// </summary>
        public void Add(depart entity)
        {
            this.List.Add(entity);
        }
        /// <summary>
        /// depart集合 索引
        /// </summary>
        public depart this[int index]
        {
            get { return (depart)this.List[index]; }
            set { this.List[index] = value; }
        }
        #endregion
    }
}
