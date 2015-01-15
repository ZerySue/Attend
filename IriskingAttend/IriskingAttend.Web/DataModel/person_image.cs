/******************************************
* 模块名称：实体 person_image
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
	/// 实体 person_image
	/// </summary>
    
	public class person_image
	{
        #region 构造函数
        /// <summary>
        /// 实体 person_image
        /// </summary>
        public person_image(){}
        #endregion

        #region 私有变量
        private Int32 _person_image_id = Int32.MinValue;
        private Int32 _person_id = Int32.MinValue;
        private string _image = null;
        private string _img_type = null;
        #endregion

        #region 公共属性
        /// <summary>
        /// 主键 person_image_id(NOT NULL)
        /// </summary>
        public Int32 person_image_id
        {
            set{ _person_image_id=value;}
            get{return _person_image_id;}
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
        /// image(NOT NULL)
        /// </summary>
        public string image
        {
            set{ _image=value;}
            get{return _image;}
        }
        /// <summary>
        /// img_type
        /// </summary>
        public string img_type
        {
            set{ _img_type=value;}
            get{return _img_type;}
        }
        #endregion
	}

    /// <summary>
    /// person_image实体集
    /// </summary>
    
    public class person_imageS : CollectionBase
    {
        #region 构造函数
        /// <summary>
        /// person_image实体集
        /// </summary>
        public person_imageS(){}
        #endregion

        #region 属性方法
        /// <summary>
        /// person_image集合 增加方法
        /// </summary>
        public void Add(person_image entity)
        {
            this.List.Add(entity);
        }
        /// <summary>
        /// person_image集合 索引
        /// </summary>
        public person_image this[int index]
        {
            get { return (person_image)this.List[index]; }
            set { this.List[index] = value; }
        }
        #endregion
    }
}
