/******************************************
* 模块名称：实体 festal_attendance
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
	/// 实体 festal_attendance
	/// </summary>
    
	public class festal_attendance
	{
        #region 构造函数
        /// <summary>
        /// 实体 festal_attendance
        /// </summary>
        public festal_attendance(){}
        #endregion

        #region 私有变量
        private Int32 _festal_attendance_id = Int32.MinValue;
        private Int32 _festival_id = Int32.MinValue;
        private Int32 _is_multipled = Int32.MinValue;
        private Int32 _multiple = Int32.MinValue;
        private string _memo = null;
        #endregion

        #region 公共属性
        /// <summary>
        /// 主键 festal_attendance_id(NOT NULL)
        /// </summary>
        public Int32 festal_attendance_id
        {
            set{ _festal_attendance_id=value;}
            get{return _festal_attendance_id;}
        }
        /// <summary>
        /// festival_id
        /// </summary>
        public Int32 festival_id
        {
            set{ _festival_id=value;}
            get{return _festival_id;}
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
        /// multiple
        /// </summary>
        public Int32 multiple
        {
            set{ _multiple=value;}
            get{return _multiple;}
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
    /// festal_attendance实体集
    /// </summary>
    
    public class festal_attendanceS : CollectionBase
    {
        #region 构造函数
        /// <summary>
        /// festal_attendance实体集
        /// </summary>
        public festal_attendanceS(){}
        #endregion

        #region 属性方法
        /// <summary>
        /// festal_attendance集合 增加方法
        /// </summary>
        public void Add(festal_attendance entity)
        {
            this.List.Add(entity);
        }
        /// <summary>
        /// festal_attendance集合 索引
        /// </summary>
        public festal_attendance this[int index]
        {
            get { return (festal_attendance)this.List[index]; }
            set { this.List[index] = value; }
        }
        #endregion
    }
}
