/******************************************
* 模块名称：实体 work_face_type
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
	/// 实体 work_face_type
	/// </summary>
	
    
	public class work_face_type
	{
        #region 构造函数
        /// <summary>
        /// 实体 work_face_type
        /// </summary>
        public work_face_type(){}
        #endregion

        #region 私有变量
        private Int32 _work_face_type_id = Int32.MinValue;
        private string _work_face_type_name = null;
        private short _work_face_type_class = short.MaxValue;
        private string _memo = null;
        #endregion

        #region 公共属性
        /// <summary>
        /// work_face_type_id(NOT NULL)
        /// </summary>
        public Int32 work_face_type_id
        {
            set{ _work_face_type_id=value;}
            get{return _work_face_type_id;}
        }
        /// <summary>
        /// work_face_type_name(NOT NULL)
        /// </summary>
        public string work_face_type_name
        {
            set{ _work_face_type_name=value;}
            get{return _work_face_type_name;}
        }
        /// <summary>
        /// work_face_type_class
        /// </summary>
        public short work_face_type_class
        {
            set{ _work_face_type_class=value;}
            get{return _work_face_type_class;}
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
    /// work_face_type实体集
    /// </summary>
    
    public class work_face_typeS : CollectionBase
    {
        #region 构造函数
        /// <summary>
        /// work_face_type实体集
        /// </summary>
        public work_face_typeS(){}
        #endregion

        #region 属性方法
        /// <summary>
        /// work_face_type集合 增加方法
        /// </summary>
        public void Add(work_face_type entity)
        {
            this.List.Add(entity);
        }
        /// <summary>
        /// work_face_type集合 索引
        /// </summary>
        public work_face_type this[int index]
        {
            get { return (work_face_type)this.List[index]; }
            set { this.List[index] = value; }
        }
        #endregion
    }
}
