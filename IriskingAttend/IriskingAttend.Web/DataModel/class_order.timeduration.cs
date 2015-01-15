/******************************************
* 模块名称：实体 class_order.timeduration
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

//支持KEY
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.ServiceModel.DomainServices.Hosting;
using System.ServiceModel.DomainServices.Server;
using System.Runtime.Serialization;

namespace Irisking.Web.DataModel
{
	/// <summary>
	/// 实体 class_order.timeduration
	/// </summary>
	
    
	public class class_order_timeduration
	{
        #region 构造函数
        /// <summary>
        /// 实体 class_order.timeduration
        /// </summary>
        public class_order_timeduration(){}
        #endregion

        #region 私有变量
        private Int32 _class_order_id = Int32.MinValue;
        private short _avail_time = short.MaxValue;
        private Int32 _work_cnt = Int32.MinValue;
        #endregion

        #region 公共属性
        /// <summary>
        /// 主键 class_order_id(NOT NULL)
        /// </summary>
        [Key]
        public Int32 class_order_id
        {
            set{ _class_order_id=value;}
            get{return _class_order_id;}
        }
        /// <summary>
        /// avail_time
        /// </summary>
        public short avail_time
        {
            set{ _avail_time=value;}
            get{return _avail_time;}
        }
        /// <summary>
        /// work_cnt
        /// </summary>
        public Int32 work_cnt
        {
            set{ _work_cnt=value;}
            get{return _work_cnt;}
        }
        #endregion
	}

    /// <summary>
    /// class_order.timeduration实体集
    /// </summary>
    
    public class class_order_timedurationS : CollectionBase
    {
        #region 构造函数
        /// <summary>
        /// class_order.timeduration实体集
        /// </summary>
        public class_order_timedurationS(){}
        #endregion

        #region 属性方法
        /// <summary>
        /// class_order.timeduration集合 增加方法
        /// </summary>
        public void Add(class_order_timeduration entity)
        {
            this.List.Add(entity);
        }
        /// <summary>
        /// class_order.timeduration集合 索引
        /// </summary>
        public class_order_timeduration this[int index]
        {
            get { return (class_order_timeduration)this.List[index]; }
            set { this.List[index] = value; }
        }
        #endregion
    }
}
