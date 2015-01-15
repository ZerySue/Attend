/*************************************************************************
** 文件名:   VmXiGouAttendReport.cs
** 主要类:   VmXiGouAttendReport
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2014-6-19
** 修改人:   
** 日  期:
** 描  述:   VmXiGouAttendReport，西沟一矿日报表、月报表vm
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
using IriskingAttend.ViewModel;
using IriskingAttend.Web;
using Irisking.Web.DataModel;
using System.ServiceModel.DomainServices.Client;
using System.Linq;
using IriskingAttend.Common;
using EDatabaseError;

namespace IriskingAttend.XiGouYiKuang
{
    public class VmXiGouAttendReport : BaseViewModel
    {
        #region 绑定属性       

        private BaseViewModelCollection<XiGouDayAttendReport> _dayAttendModel;
        /// <summary>
        /// 前台界面绑定的数据集合
        /// </summary>
        public BaseViewModelCollection<XiGouDayAttendReport> DayAttendModel
        {
            get
            {
                return _dayAttendModel;
            }
            set
            {
                _dayAttendModel = value;
                OnPropertyChanged<BaseViewModelCollection<XiGouDayAttendReport>>(() => this.DayAttendModel);
            }
        }

        private BaseViewModelCollection<XiGouMonthAttendReport> _monthAttendModel;
        /// <summary>
        /// 前台界面绑定的数据集合
        /// </summary>
        public BaseViewModelCollection<XiGouMonthAttendReport> MonthAttendModel
        {
            get
            {
                return _monthAttendModel;
            }
            set
            {
                _monthAttendModel = value;
                OnPropertyChanged<BaseViewModelCollection<XiGouMonthAttendReport>>(() => this.MonthAttendModel);
            }
        }

        private BaseViewModelCollection<XiGouInWellPersonDetailReport> _inWellPersonDetailModel;
        /// <summary>
        /// 前台界面绑定的数据集合
        /// </summary>
        public BaseViewModelCollection<XiGouInWellPersonDetailReport> InWellPersonDetailModel
        {
            get
            {
                return _inWellPersonDetailModel;
            }
            set
            {
                _inWellPersonDetailModel = value;
                OnPropertyChanged<BaseViewModelCollection<XiGouInWellPersonDetailReport>>(() => this.InWellPersonDetailModel);
            }
        }
        
        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public VmXiGouAttendReport()
        {
            DayAttendModel = new BaseViewModelCollection<XiGouDayAttendReport>();
            MonthAttendModel = new BaseViewModelCollection<XiGouMonthAttendReport>();
            InWellPersonDetailModel = new BaseViewModelCollection<XiGouInWellPersonDetailReport>(); 
        }

        #endregion

        #region 根据查询条件查询数据并对数据进行处理
       
        /// <summary>
        /// 根据查询条件查询数据
        /// </summary>        
        /// <returns></returns>
        public void GetDayAttendCollect(DateTime beginTime,string[] departNames, string[] classTypeNames, string personName, string workSn)
        {
            WaitingDialog.ShowWaiting("正在查询，请等待");

            ServiceDomDbAcess.ReOpenSever();
            EntityQuery<XiGouDayAttendReport> list = ServiceDomDbAcess.GetSever().GetXiGouDayAttendCollectQuery(beginTime, departNames, classTypeNames, personName, workSn);
            //回调异常类
            Action<LoadOperation<XiGouDayAttendReport>> actionCallBack = ErrorHandle<XiGouDayAttendReport>.OnLoadErrorCallBack;
            //异步事件
            LoadOperation<XiGouDayAttendReport> loadOp = ServiceDomDbAcess.GetSever().Load(list, actionCallBack, null);

            loadOp.Completed += delegate
            {
                try
                {
                    DayAttendModel.Clear();

                    foreach (XiGouDayAttendReport item in loadOp.Entities)
                    {
                        DayAttendModel.Add(item);
                    }

                    WaitingDialog.HideWaiting();
                }
                catch (Exception e)
                {
                    ChildWindow errorWin = new ErrorWindow("加载异常", e.Message);
                    errorWin.Show();
                    WaitingDialog.HideWaiting();
                }
            };
        }


        /// <summary>
        /// 根据查询条件查询数据
        /// </summary>        
        /// <returns></returns>
        public void GetMonthAttendCollect(DateTime beginTime, DateTime endTime, string[] departNames, string[] classTypeNames, string personName, string workSn)
        {
            WaitingDialog.ShowWaiting("正在查询，请等待");

            ServiceDomDbAcess.ReOpenSever();
            EntityQuery<XiGouMonthAttendReport> list = ServiceDomDbAcess.GetSever().GetXiGouMonthAttendCollectQuery(beginTime, endTime, departNames, classTypeNames, personName, workSn);
            //回调异常类
            Action<LoadOperation<XiGouMonthAttendReport>> actionCallBack = ErrorHandle<XiGouMonthAttendReport>.OnLoadErrorCallBack;
            //异步事件
            LoadOperation<XiGouMonthAttendReport> loadOp = ServiceDomDbAcess.GetSever().Load(list, actionCallBack, null);

            loadOp.Completed += delegate
            {
                try
                {
                    MonthAttendModel.Clear();

                    foreach (XiGouMonthAttendReport item in loadOp.Entities)
                    {
                        MonthAttendModel.Add(item);
                    }

                    WaitingDialog.HideWaiting();
                }
                catch (Exception e)
                {
                    ChildWindow errorWin = new ErrorWindow("加载异常", e.Message);
                    errorWin.Show();
                    WaitingDialog.HideWaiting();
                }
            };
        }       
        #endregion

        #region  西沟一矿人员明细表
        /// <summary>
        /// 根据查询条件查询数据
        /// </summary>        
        /// <returns></returns>
        public void GetXiGouInWellPersonDetail(DateTime beginTime, DateTime endTime, string personName)
        {
            WaitingDialog.ShowWaiting("正在查询，请等待");

            ServiceDomDbAcess.ReOpenSever();
            EntityQuery<XiGouInWellPersonDetailReport> list = ServiceDomDbAcess.GetSever().GetXiGouInWellPersonDetailQuery(beginTime, endTime, personName);
            //回调异常类
            Action<LoadOperation<XiGouInWellPersonDetailReport>> actionCallBack = ErrorHandle<XiGouInWellPersonDetailReport>.OnLoadErrorCallBack;
            //异步事件
            LoadOperation<XiGouInWellPersonDetailReport> loadOp = ServiceDomDbAcess.GetSever().Load(list, actionCallBack, null);

            loadOp.Completed += delegate
            {
                try
                {
                    InWellPersonDetailModel.Clear();

                    foreach (XiGouInWellPersonDetailReport item in loadOp.Entities)
                    {
                        InWellPersonDetailModel.Add(item);
                    }

                    WaitingDialog.HideWaiting();
                }
                catch (Exception e)
                {
                    ChildWindow errorWin = new ErrorWindow("加载异常", e.Message);
                    errorWin.Show();
                    WaitingDialog.HideWaiting();
                }
            };
        }       
        #endregion
    }
}
