/******************************************
* 模块名称：实体 attend_param
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
	/// 实体 attend_param
	/// </summary>
	
    
	public class attend_param
	{
        #region 构造函数
        /// <summary>
        /// 实体 attend_param
        /// </summary>
        public attend_param(){}
        #endregion

        #region 私有变量
        private Int32 _auto_attend = Int32.MinValue;
        private Int32 _auto_attend_time = Int32.MinValue;
        private Int32 _min_hard = Int32.MinValue;
        private Int32 _add_attend_time = Int32.MinValue;
        private Int32 _scan_time = Int32.MinValue;
        private Int32 _begin_time = Int32.MinValue;
        private Int32 _end_time = Int32.MinValue;
        private Int32 _scan_check_time = Int32.MinValue;
        private Int32 _auto_check = Int32.MinValue;
        #endregion

        #region 公共属性
        /// <summary>
        /// auto_attend
        /// </summary>
        public Int32 auto_attend
        {
            set{ _auto_attend=value;}
            get{return _auto_attend;}
        }
        /// <summary>
        /// auto_attend_time
        /// </summary>
        public Int32 auto_attend_time
        {
            set{ _auto_attend_time=value;}
            get{return _auto_attend_time;}
        }
        /// <summary>
        /// min_hard
        /// </summary>
        public Int32 min_hard
        {
            set{ _min_hard=value;}
            get{return _min_hard;}
        }
        /// <summary>
        /// add_attend_time
        /// </summary>
        public Int32 add_attend_time
        {
            set{ _add_attend_time=value;}
            get{return _add_attend_time;}
        }
        /// <summary>
        /// scan_time
        /// </summary>
        public Int32 scan_time
        {
            set{ _scan_time=value;}
            get{return _scan_time;}
        }
        /// <summary>
        /// begin_time
        /// </summary>
        public Int32 begin_time
        {
            set{ _begin_time=value;}
            get{return _begin_time;}
        }
        /// <summary>
        /// end_time
        /// </summary>
        public Int32 end_time
        {
            set{ _end_time=value;}
            get{return _end_time;}
        }
        /// <summary>
        /// scan_check_time
        /// </summary>
        public Int32 scan_check_time
        {
            set{ _scan_check_time=value;}
            get{return _scan_check_time;}
        }
        /// <summary>
        /// auto_check
        /// </summary>
        public Int32 auto_check
        {
            set{ _auto_check=value;}
            get{return _auto_check;}
        }
        #endregion
	}

    /// <summary>
    /// attend_param实体集
    /// </summary>
    
    public class attend_paramS : CollectionBase
    {
        #region 构造函数
        /// <summary>
        /// attend_param实体集
        /// </summary>
        public attend_paramS(){}
        #endregion

        #region 属性方法
        /// <summary>
        /// attend_param集合 增加方法
        /// </summary>
        public void Add(attend_param entity)
        {
            this.List.Add(entity);
        }
        /// <summary>
        /// attend_param集合 索引
        /// </summary>
        public attend_param this[int index]
        {
            get { return (attend_param)this.List[index]; }
            set { this.List[index] = value; }
        }
        #endregion
    }
}
