/*************************************************************************
** 文件名:   VmHuFengAttend.cs
** 主要类:   VmHuFengAttend
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2014-6-19
** 修改人:   
** 日  期:
** 描  述:   VmHuFengAttend，虎峰煤矿日报表、月报表vm
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

namespace IriskingAttend.HuFeng
{
    public class VmHuFengAttend : BaseViewModel
    {
        #region 绑定属性       

        private BaseViewModelCollection<HuFengDayAttendReport> _dayAttendModel;
        /// <summary>
        /// 前台界面绑定的数据集合
        /// </summary>
        public BaseViewModelCollection<HuFengDayAttendReport> DayAttendModel
        {
            get
            {
                return _dayAttendModel;
            }
            set
            {
                _dayAttendModel = value;
                OnPropertyChanged<BaseViewModelCollection<HuFengDayAttendReport>>(() => this.DayAttendModel);
            }
        }

        private BaseViewModelCollection<HuFengMonthAttendReport> _monthAttendModel;
        /// <summary>
        /// 前台界面绑定的数据集合
        /// </summary>
        public BaseViewModelCollection<HuFengMonthAttendReport> MonthAttendModel
        {
            get
            {
                return _monthAttendModel;
            }
            set
            {
                _monthAttendModel = value;
                OnPropertyChanged<BaseViewModelCollection<HuFengMonthAttendReport>>(() => this.MonthAttendModel);
            }
        }
        
        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public VmHuFengAttend()
        {
            DayAttendModel = new BaseViewModelCollection<HuFengDayAttendReport>();
            MonthAttendModel = new BaseViewModelCollection<HuFengMonthAttendReport>();       
        }

        #endregion

        #region 根据查询条件查询数据并对数据进行处理
       
        /// <summary>
        /// 根据查询条件查询数据
        /// </summary>        
        /// <returns></returns>
        public void GetDayAttendCollect(DateTime beginTime,string[] departNames, string[] classOrderNames, string[] principalNames, string personName, string workSn)
        {
            WaitingDialog.ShowWaiting("正在查询，请等待");

            ServiceDomDbAcess.ReOpenSever();
            EntityQuery<HuFengDayAttendReport> list = ServiceDomDbAcess.GetSever().GetHuFengDayAttendCollectQuery(beginTime, departNames, classOrderNames, principalNames, personName, workSn);
            //回调异常类
            Action<LoadOperation<HuFengDayAttendReport>> actionCallBack = ErrorHandle<HuFengDayAttendReport>.OnLoadErrorCallBack;
            //异步事件
            LoadOperation<HuFengDayAttendReport> loadOp = ServiceDomDbAcess.GetSever().Load(list, actionCallBack, null);

            loadOp.Completed += delegate
            {
                try
                {
                    DayAttendModel.Clear();

                    foreach (HuFengDayAttendReport item in loadOp.Entities)
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
        public void GetMonthAttendCollect(DateTime beginTime, DateTime endTime, string[] departNames, string personName, string workSn)
        {
            WaitingDialog.ShowWaiting("正在查询，请等待");

            ServiceDomDbAcess.ReOpenSever();
            EntityQuery<HuFengMonthAttendReport> list = ServiceDomDbAcess.GetSever().GetHuFengMonthAttendCollectQuery(beginTime, endTime, departNames, personName, workSn);
            //回调异常类
            Action<LoadOperation<HuFengMonthAttendReport>> actionCallBack = ErrorHandle<HuFengMonthAttendReport>.OnLoadErrorCallBack;
            //异步事件
            LoadOperation<HuFengMonthAttendReport> loadOp = ServiceDomDbAcess.GetSever().Load(list, actionCallBack, null);

            loadOp.Completed += delegate
            {
                try
                {
                    MonthAttendModel.Clear();

                    foreach (HuFengMonthAttendReport item in loadOp.Entities)
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
    }
}
