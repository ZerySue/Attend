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
using IriskingAttend.Common;
using System.ServiceModel.DomainServices.Client;
using EDatabaseError;
using System.Linq;

namespace IriskingAttend.LiuHuangGou
{
    public class VmLiuhuangGouAttendCollect : BaseViewModel
    {
        #region 绑定属性

        private BaseViewModelCollection<PersonMonthAttend> _personMonthAttendModel;
        /// <summary>
        /// 前台界面绑定的数据集合
        /// </summary>
        public BaseViewModelCollection<PersonMonthAttend> PersonMonthAttendModel
        {
            get
            {
                return _personMonthAttendModel;
            }
            set
            {
                _personMonthAttendModel = value;
                OnPropertyChanged<BaseViewModelCollection<PersonMonthAttend>>(() => this.PersonMonthAttendModel);
            }
        }

        private BaseViewModelCollection<PersonMonthAttend> _monthAttendUnderRuleModel;
        /// <summary>
        /// 前台界面绑定的数据集合
        /// </summary>
        public BaseViewModelCollection<PersonMonthAttend> MonthAttendUnderRuleModel
        {
            get
            {
                return _monthAttendUnderRuleModel;
            }
            set
            {
                _monthAttendUnderRuleModel = value;
                OnPropertyChanged<BaseViewModelCollection<PersonMonthAttend>>(() => this.MonthAttendUnderRuleModel);
            }
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public VmLiuhuangGouAttendCollect()
        {
            PersonMonthAttendModel = new BaseViewModelCollection<PersonMonthAttend>();
            MonthAttendUnderRuleModel = new BaseViewModelCollection<PersonMonthAttend>();
        }
        #endregion

        #region 根据查询条件查询数据并对数据进行处理

        /// <summary>
        /// 根据查询条件查询数据
        /// </summary>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">截止时间</param>
        /// <param name="departName">部门名称</param>   
        /// <param name="personName">人员名称</param>  
        /// <returns></returns>
        public void GetPersonAttendCollect(DateTime beginTime, DateTime endTime, string[] departName, string[] personName)
        {
            WaitingDialog.ShowWaiting("正在查询，请等待");

            ServiceDomDbAcess.ReOpenSever();
            EntityQuery<PersonMonthAttend> list = ServiceDomDbAcess.GetSever().GetPersonMonthAttendListQuery(beginTime, endTime, departName, personName);
            //回调异常类
            Action<LoadOperation<PersonMonthAttend>> actionCallBack = ErrorHandle<PersonMonthAttend>.OnLoadErrorCallBack;
            //异步事件
            LoadOperation<PersonMonthAttend> loadOp = ServiceDomDbAcess.GetSever().Load(list, actionCallBack, null);

            loadOp.Completed += delegate
            {
                try
                {
                    PersonMonthAttendModel.Clear();
                    foreach (PersonMonthAttend item in loadOp.Entities)
                    {
                        PersonMonthAttendModel.Add(item);
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
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">截止时间</param>
        /// <param name="departName">部门名称</param>   
        /// <param name="personName">人员名称</param>  
        /// <returns></returns>
        public void GetMonthAttendUnderRuleCollect(DateTime beginTime, DateTime endTime, string[] departName, string[] personName, int ruleDataNum)
        {
            WaitingDialog.ShowWaiting("正在查询，请等待");

            ServiceDomDbAcess.ReOpenSever();
            EntityQuery<PersonMonthAttend> list = ServiceDomDbAcess.GetSever().GetMonthAttendUnderRuleCollectQuery(beginTime, endTime, departName, personName, ruleDataNum);
            //回调异常类
            Action<LoadOperation<PersonMonthAttend>> actionCallBack = ErrorHandle<PersonMonthAttend>.OnLoadErrorCallBack;
            //异步事件
            LoadOperation<PersonMonthAttend> loadOp = ServiceDomDbAcess.GetSever().Load(list, actionCallBack, null);

            loadOp.Completed += delegate
            {
                try
                {
                    MonthAttendUnderRuleModel.Clear();
                    foreach (PersonMonthAttend item in loadOp.Entities)
                    {
                        MonthAttendUnderRuleModel.Add(item);
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
