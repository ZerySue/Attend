/******************************************
* 模块名称：实体 work_cnt_policy
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
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.ServiceModel.DomainServices.Hosting;
using System.ServiceModel.DomainServices.Server;
using System.Runtime.Serialization;

namespace Irisking.Web.DataModel
{
	/// <summary>
	/// 实体 work_cnt_policy
	/// </summary>
	
    
	public class work_cnt_policy
	{
        #region 构造函数
        /// <summary>
        /// 实体 work_cnt_policy
        /// </summary>
        public work_cnt_policy(){}
        #endregion

        #region 私有变量
        private Int32 _lt = Int32.MinValue;
        private Int32 _gt = Int32.MinValue;
        private Int32 _accuracy = Int32.MinValue;
        private Int32 _min_work_cnt = Int32.MinValue;
        private Int32 _max_work_cnt = Int32.MinValue;
        #endregion

        #region 公共属性
        /// <summary>
        /// lt
        /// </summary>
        [Key]
        public Int32 lt
        {
            set{ _lt=value;}
            get{return _lt;}
        }
        /// <summary>
        /// gt
        /// </summary>
        public Int32 gt
        {
            set{ _gt=value;}
            get{return _gt;}
        }
        /// <summary>
        /// accuracy
        /// </summary>
        public Int32 accuracy
        {
            set{ _accuracy=value;}
            get{return _accuracy;}
        }
        /// <summary>
        /// min_work_cnt
        /// </summary>
        public Int32 min_work_cnt
        {
            set{ _min_work_cnt=value;}
            get{return _min_work_cnt;}
        }
        /// <summary>
        /// max_work_cnt
        /// </summary>
        public Int32 max_work_cnt
        {
            set{ _max_work_cnt=value;}
            get{return _max_work_cnt;}
        }
        #endregion
	}

    /// <summary>
    /// work_cnt_policy实体集
    /// </summary>
    
    public class work_cnt_policyS : CollectionBase
    {
        #region 构造函数
        /// <summary>
        /// work_cnt_policy实体集
        /// </summary>
        public work_cnt_policyS(){}
        #endregion

        #region 属性方法
        /// <summary>
        /// work_cnt_policy集合 增加方法
        /// </summary>
        public void Add(work_cnt_policy entity)
        {
            this.List.Add(entity);
        }
        /// <summary>
        /// work_cnt_policy集合 索引
        /// </summary>
        public work_cnt_policy this[int index]
        {
            get { return (work_cnt_policy)this.List[index]; }
            set { this.List[index] = value; }
        }
        #endregion
    }
}
