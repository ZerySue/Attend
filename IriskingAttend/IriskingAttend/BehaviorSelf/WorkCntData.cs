/*************************************************************************
** 文件名:   WorkCntData.cs
×× 主要类:   WorkCntData
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   wz
** 日  期:   2013-7-23
** 修改人:   
** 日  期:
** 描  述:   用于显示分时段的记工时间和记工工数
**
** 版  本:   1.0.0
** 备  注:  命名及代码编写遵守C#编码规范
**
 * ***********************************************************************/
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ServiceModel.DomainServices.Client;
namespace IriskingAttend
{

    /// <summary>
    /// 用于显示分时段的记工时间和记工工数
    /// </summary>
    public class WorkCntData : Entity
    {

        #region 属性
       
        private DateTime _availTime;
        /// <summary>
        /// 记工时间
        /// </summary>
        public DateTime AvailTime
        {
            get
            {
                return _availTime;
            }
            set
            {
                this.RaiseDataMemberChanging("AvailTime");
                this.ValidateProperty("AvailTime", value);
                this._availTime = value;
                this.RaiseDataMemberChanged("AvailTime");
            }
        }

        private double _workCnt;
        /// <summary>
        ///  记工工数
        /// </summary>
        public double WorkCnt
        {
            get
            {
                return _workCnt;
            }
            set
            {
                this.RaiseDataMemberChanging("WorkCnt");
                this.ValidateProperty("WorkCnt", value);
                this._workCnt = value;
                this.RaiseDataMemberChanged("WorkCnt");
            }
        }

      

        #endregion


        public WorkCntData()
        {
            WorkCnt = 1;
        }

    }

}
