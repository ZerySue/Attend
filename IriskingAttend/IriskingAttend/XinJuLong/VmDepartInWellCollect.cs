/*************************************************************************
** 文件名:   VmDepartInWellCollect.cs
** 主要类:   VmDepartInWellCollect
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2014-3-18
** 修改人:   
** 日  期:
** 描  述:   VmDepartInWellCollect，新巨龙当前各单位井下出勤统计
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
    public class VmDepartInWellCollect : BaseViewModel
    {
        #region 绑定属性

        private BaseViewModelCollection<AttendDepartInWellQuery> _departInWellCollectModel;

        /// <summary>
        /// 前台界面绑定的数据集合
        /// </summary>
        public BaseViewModelCollection<AttendDepartInWellQuery> DepartInWellCollectModel
        {
            get
            {
                return _departInWellCollectModel;
            }
            set
            {
                _departInWellCollectModel = value;
                OnPropertyChanged<BaseViewModelCollection<AttendDepartInWellQuery>>(() => this.DepartInWellCollectModel);
            }
        }

        private BaseViewModelCollection<AttendDepartInWellDetail> _departInWellDetailModel;

        /// <summary>
        /// 前台界面绑定的数据集合
        /// </summary>
        public BaseViewModelCollection<AttendDepartInWellDetail> DepartInWellDetailModel
        {
            get
            {
                return _departInWellDetailModel;
            }
            set
            {
                _departInWellDetailModel = value;
                OnPropertyChanged<BaseViewModelCollection<AttendDepartInWellDetail>>(() => this.DepartInWellDetailModel);
            }
        }
        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public VmDepartInWellCollect()
        {
            DepartInWellCollectModel = new BaseViewModelCollection<AttendDepartInWellQuery>();
            DepartInWellDetailModel = new BaseViewModelCollection<AttendDepartInWellDetail>();
        }

        #endregion

        #region 根据查询条件查询数据并对数据进行处理

        /// <summary>
        /// 异步获取人员信息
        /// </summary>
        public void GetAttendDepartInWellCollect(DateTime beginTime, DateTime endTime, List<UserDepartInfo> departInfos)
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

                EntityQuery<AttendDepartInWellQuery> lstQuery = ServiceDomDbAcess.GetSever().GetAttendDepartInWellCollectQuery(beginTime, endTime, departIdList.ToArray());
                ///回调异常类
                Action<LoadOperation<AttendDepartInWellQuery>> getPersonCallBack = new Action<LoadOperation<AttendDepartInWellQuery>>(ErrorHandle<AttendDepartInWellQuery>.OnLoadErrorCallBack);
                ///异步事件
                LoadOperation<AttendDepartInWellQuery> lo = ServiceDomDbAcess.GetSever().Load(lstQuery, getPersonCallBack, null);

                lo.Completed += (o, e) =>
                {
                    DepartInWellCollectModel.Clear();

                    AttendDepartInWellQuery totalDepartCollect = new AttendDepartInWellQuery();
                    totalDepartCollect.DepartId = -1;
                    totalDepartCollect.DepartName = "合计";
                  
                    foreach (var ar in lo.Entities)
                    {
                        DepartInWellCollectModel.Add(ar);
                        totalDepartCollect.NightInWellCollect += ar.NightInWellCollect;
                        totalDepartCollect.MiddleInWellCollect += ar.MiddleInWellCollect;
                        totalDepartCollect.MoringInWellCollect += ar.MoringInWellCollect;
                        totalDepartCollect.OneInWellCollect += ar.OneInWellCollect;
                        totalDepartCollect.TwoInWellCollect += ar.TwoInWellCollect;
                        totalDepartCollect.ThreeInWellCollect += ar.ThreeInWellCollect;
                        totalDepartCollect.FourInWellCollect += ar.FourInWellCollect;
                    }

                    DepartInWellCollectModel.Add(totalDepartCollect);

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

        /// <summary>
        /// 异步获取下井人员详细信息
        /// </summary>
        public void GetAttendDepartInWellDetail(DateTime beginTime, DateTime endTime, int departId, string attendSign)
        {
            try
            {               
                //显示等待界面
                WaitingDialog.ShowWaiting();

                ServiceDomDbAcess.ReOpenSever();

                EntityQuery<AttendDepartInWellDetail> lstQuery = ServiceDomDbAcess.GetSever().GetAttendDepartInWellDetailQuery(beginTime, endTime, departId, attendSign);
                ///回调异常类
                Action<LoadOperation<AttendDepartInWellDetail>> getPersonCallBack = new Action<LoadOperation<AttendDepartInWellDetail>>(ErrorHandle<AttendDepartInWellDetail>.OnLoadErrorCallBack);
                ///异步事件
                LoadOperation<AttendDepartInWellDetail> lo = ServiceDomDbAcess.GetSever().Load(lstQuery, getPersonCallBack, null);

                lo.Completed += (o, e) =>
                {
                    DepartInWellDetailModel.Clear();

                    AttendDepartInWellDetail totalDepartCollect = new AttendDepartInWellDetail();                   

                    foreach (var ar in lo.Entities)
                    {
                        DepartInWellDetailModel.Add(ar);
                       
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
