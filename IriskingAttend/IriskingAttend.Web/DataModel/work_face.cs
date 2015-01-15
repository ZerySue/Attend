/******************************************
* 模块名称：实体 work_face
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
	/// 实体 work_face
	/// </summary>
    
	public class work_face
	{
        #region 构造函数
        /// <summary>
        /// 实体 work_face
        /// </summary>
        public work_face(){}
        #endregion

        #region 私有变量
        private Int32 _work_face_id = Int32.MinValue;
        private Int32 _mine_area_id = Int32.MinValue;
        private string _serial_num = null;
        private string _work_face_name = null;
        private string _at_position = null;
        private string _brief = null;
        private Int32 _work_face_type_id = Int32.MinValue;
        private string _establish_time = null;
        private string _discard_time = null;
        private string _memo = null;
        private string _create_time = null;
        private string _update_time = null;
        private short _delete_info = short.MaxValue;
        #endregion

        #region 公共属性
        /// <summary>
        /// 主键 work_face_id(NOT NULL)
        /// </summary>
        public Int32 work_face_id
        {
            set{ _work_face_id=value;}
            get{return _work_face_id;}
        }
        /// <summary>
        /// mine_area_id
        /// </summary>
        public Int32 mine_area_id
        {
            set{ _mine_area_id=value;}
            get{return _mine_area_id;}
        }
        /// <summary>
        /// serial_num(NOT NULL)
        /// </summary>
        public string serial_num
        {
            set{ _serial_num=value;}
            get{return _serial_num;}
        }
        /// <summary>
        /// work_face_name(NOT NULL)
        /// </summary>
        public string work_face_name
        {
            set{ _work_face_name=value;}
            get{return _work_face_name;}
        }
        /// <summary>
        /// at_position
        /// </summary>
        public string at_position
        {
            set{ _at_position=value;}
            get{return _at_position;}
        }
        /// <summary>
        /// brief
        /// </summary>
        public string brief
        {
            set{ _brief=value;}
            get{return _brief;}
        }
        /// <summary>
        /// work_face_type_id
        /// </summary>
        public Int32 work_face_type_id
        {
            set{ _work_face_type_id=value;}
            get{return _work_face_type_id;}
        }
        /// <summary>
        /// establish_time
        /// </summary>
        public string establish_time
        {
            set{ _establish_time=value;}
            get{return _establish_time;}
        }
        /// <summary>
        /// discard_time
        /// </summary>
        public string discard_time
        {
            set{ _discard_time=value;}
            get{return _discard_time;}
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
        /// delete_info
        /// </summary>
        public short delete_info
        {
            set{ _delete_info=value;}
            get{return _delete_info;}
        }
        #endregion
	}

    /// <summary>
    /// work_face实体集
    /// </summary>
    
    public class work_faceS : CollectionBase
    {
        #region 构造函数
        /// <summary>
        /// work_face实体集
        /// </summary>
        public work_faceS(){}
        #endregion

        #region 属性方法
        /// <summary>
        /// work_face集合 增加方法
        /// </summary>
        public void Add(work_face entity)
        {
            this.List.Add(entity);
        }
        /// <summary>
        /// work_face集合 索引
        /// </summary>
        public work_face this[int index]
        {
            get { return (work_face)this.List[index]; }
            set { this.List[index] = value; }
        }
        #endregion
    }
}
