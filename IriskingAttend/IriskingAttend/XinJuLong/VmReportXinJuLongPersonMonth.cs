/*************************************************************************
** 文件名:   VmReportXinJuLongPersonMonth.cs
** 主要类:   VmReportXinJuLongPersonMonth
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2014-3-20
** 修改人:   
** 日  期:
** 描  述:   VmReportXinJuLongPersonMonth，新巨龙月报表考勤查询
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
using IriskingAttend.Web;
using IriskingAttend.ViewModel;
using System.ComponentModel;
using System.Collections.Generic;
using System.ServiceModel.DomainServices.Client;
using IriskingAttend.Common;
using EDatabaseError;
using Irisking.Web.DataModel;

namespace IriskingAttend.XinJuLong
{    
    public class VmReportXinJuLongPersonMonth : BaseViewModel
    {
        #region 绑定属性

        private BaseViewModelCollection<ReportPersonMonth> _personMonthAttendModel;

        /// <summary>
        /// 前台界面绑定的数据集合
        /// </summary>
        public BaseViewModelCollection<ReportPersonMonth> PersonMonthAttendModel
        {
            get
            {
                return _personMonthAttendModel;
            }
            set
            {
                _personMonthAttendModel = value;
                OnPropertyChanged<BaseViewModelCollection<ReportPersonMonth>>(() => this.PersonMonthAttendModel);
            }
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public VmReportXinJuLongPersonMonth()
        {
            PersonMonthAttendModel = new BaseViewModelCollection<ReportPersonMonth>();           
        }

        #endregion

        #region 根据查询条件查询数据并对数据进行处理

        /// <summary>
        /// 异步获取不完整考勤查询
        /// </summary>
        public void GetReportPersonMonthAttend(DateTime beginTime, List<UserDepartInfo> departInfos, string personName, string workSn) 
        {
            try
            {
                List<int> departIdList = new List<int>();

                //获取选择的部门
                if (departInfos.Count > 0)
                {
                    foreach (UserDepartInfo ar in departInfos)
                    {
                        departIdList.Add(ar.depart_id);
                    }
                }

                //显示等待界面
                WaitingDialog.ShowWaiting();

                ServiceDomDbAcess.ReOpenSever();

                EntityQuery<ReportPersonMonth> lstQuery = ServiceDomDbAcess.GetSever().GetReportPersonMonthAttendQuery(beginTime, departIdList.ToArray(), personName, workSn);
                ///回调异常类
                Action<LoadOperation<ReportPersonMonth>> getPersonCallBack = new Action<LoadOperation<ReportPersonMonth>>(ErrorHandle<ReportPersonMonth>.OnLoadErrorCallBack);
                ///异步事件
                LoadOperation<ReportPersonMonth> lo = ServiceDomDbAcess.GetSever().Load(lstQuery, getPersonCallBack, null);

                lo.Completed += (o, e) =>
                {
                    PersonMonthAttendModel.Clear();
                  
                    foreach (var ar in lo.Entities)
                    {
                        PersonMonthAttendModel.Add(ar);                       
                    }

                    //隐藏等待界面
                    WaitingDialog.HideWaiting(); 
                };
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }
        #endregion
    }
}
