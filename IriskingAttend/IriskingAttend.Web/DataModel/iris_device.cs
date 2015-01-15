/******************************************
* 模块名称：实体 iris_device
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
	/// 实体 iris_device
	/// </summary>

    
	public class iris_device
	{
        #region 构造函数
        /// <summary>
        /// 实体 iris_device
        /// </summary>
        public iris_device(){}
        #endregion

        #region 私有变量
        private string _dev_sn = null;
        private string _start_time = null;
        private Int32 _dev_type = Int32.MinValue;
        private string _place = null;
        #endregion

        #region 公共属性
        /// <summary>
        /// 主键 dev_sn(NOT NULL)
        /// </summary>
        public string dev_sn
        {
            set{ _dev_sn=value;}
            get{return _dev_sn;}
        }
        /// <summary>
        /// 主键 start_time(NOT NULL)
        /// </summary>
        public string start_time
        {
            set{ _start_time=value;}
            get{return _start_time;}
        }
        /// <summary>
        /// dev_type
        /// </summary>
        public Int32 dev_type
        {
            set{ _dev_type=value;}
            get{return _dev_type;}
        }
        /// <summary>
        /// place
        /// </summary>
        public string place
        {
            set{ _place=value;}
            get{return _place;}
        }
        #endregion
	}

    /// <summary>
    /// iris_device实体集
    /// </summary>
    
    public class iris_deviceS : CollectionBase
    {
        #region 构造函数
        /// <summary>
        /// iris_device实体集
        /// </summary>
        public iris_deviceS(){}
        #endregion

        #region 属性方法
        /// <summary>
        /// iris_device集合 增加方法
        /// </summary>
        public void Add(iris_device entity)
        {
            this.List.Add(entity);
        }
        /// <summary>
        /// iris_device集合 索引
        /// </summary>
        public iris_device this[int index]
        {
            get { return (iris_device)this.List[index]; }
            set { this.List[index] = value; }
        }
        #endregion
    }
}
