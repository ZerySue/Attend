/******************************************
* 模块名称：实体 tbl_recog_to_leave_type
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
	/// 实体 tbl_recog_to_leave_type
	/// </summary>
    
	public class tbl_recog_to_leave_type
	{
        #region 构造函数
        /// <summary>
        /// 实体 tbl_recog_to_leave_type
        /// </summary>
        public tbl_recog_to_leave_type(){}
        #endregion

        #region 私有变量
        private Int32 _tbl_recog_to_leave_type_id = Int32.MinValue;
        private Int32 _leave_type_id = Int32.MinValue;
        private Int32 _in_well_leave_state = Int32.MinValue;
        private Int32 _out_well_leave_state = Int32.MinValue;
        #endregion

        #region 公共属性
        /// <summary>
        /// tbl_recog_to_leave_type_id(NOT NULL)
        /// </summary>
        [Key]
        public Int32 tbl_recog_to_leave_type_id
        {
            set{ _tbl_recog_to_leave_type_id=value;}
            get{return _tbl_recog_to_leave_type_id;}
        }
        /// <summary>
        /// 主键 leave_type_id
        /// </summary>
        public Int32 leave_type_id
        {
            set{ _leave_type_id=value;}
            get{return _leave_type_id;}
        }
        /// <summary>
        /// in_well_leave_state(NOT NULL)
        /// </summary>
        public Int32 in_well_leave_state
        {
            set{ _in_well_leave_state=value;}
            get{return _in_well_leave_state;}
        }
        /// <summary>
        /// out_well_leave_state(NOT NULL)
        /// </summary>
        public Int32 out_well_leave_state
        {
            set{ _out_well_leave_state=value;}
            get{return _out_well_leave_state;}
        }
        #endregion
	}

    /// <summary>
    /// tbl_recog_to_leave_type实体集
    /// </summary>
    
    public class tbl_recog_to_leave_typeS : CollectionBase
    {
        #region 构造函数
        /// <summary>
        /// tbl_recog_to_leave_type实体集
        /// </summary>
        public tbl_recog_to_leave_typeS(){}
        #endregion

        #region 属性方法
        /// <summary>
        /// tbl_recog_to_leave_type集合 增加方法
        /// </summary>
        public void Add(tbl_recog_to_leave_type entity)
        {
            this.List.Add(entity);
        }
        /// <summary>
        /// tbl_recog_to_leave_type集合 索引
        /// </summary>
        public tbl_recog_to_leave_type this[int index]
        {
            get { return (tbl_recog_to_leave_type)this.List[index]; }
            set { this.List[index] = value; }
        }
        #endregion
    }
}
