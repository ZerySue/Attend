/******************************************
* 模块名称：实体 depart_work_face
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
	/// 实体 depart_work_face
	/// </summary>
    
	public class depart_work_face
	{
        #region 构造函数
        /// <summary>
        /// 实体 depart_work_face
        /// </summary>
        public depart_work_face(){}
        #endregion

        #region 私有变量
        private Int32 _depart_work_face_id = Int32.MinValue;
        private Int32 _depart_id = Int32.MinValue;
        private string _work_face_sn = null;
        #endregion

        #region 公共属性
        /// <summary>
        /// depart_work_face_id(NOT NULL)
        /// </summary>
        public Int32 depart_work_face_id
        {
            set{ _depart_work_face_id=value;}
            get{return _depart_work_face_id;}
        }
        /// <summary>
        /// 主键 depart_id(NOT NULL)
        /// </summary>
        public Int32 depart_id
        {
            set{ _depart_id=value;}
            get{return _depart_id;}
        }
        /// <summary>
        /// work_face_sn(NOT NULL)
        /// </summary>
        public string work_face_sn
        {
            set{ _work_face_sn=value;}
            get{return _work_face_sn;}
        }
        #endregion
	}

    /// <summary>
    /// depart_work_face实体集
    /// </summary>
    
    public class depart_work_faceS : CollectionBase
    {
        #region 构造函数
        /// <summary>
        /// depart_work_face实体集
        /// </summary>
        public depart_work_faceS(){}
        #endregion

        #region 属性方法
        /// <summary>
        /// depart_work_face集合 增加方法
        /// </summary>
        public void Add(depart_work_face entity)
        {
            this.List.Add(entity);
        }
        /// <summary>
        /// depart_work_face集合 索引
        /// </summary>
        public depart_work_face this[int index]
        {
            get { return (depart_work_face)this.List[index]; }
            set { this.List[index] = value; }
        }
        #endregion
    }
}
