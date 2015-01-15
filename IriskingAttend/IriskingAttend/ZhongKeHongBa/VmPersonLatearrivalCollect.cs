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
using Irisking.Web.DataModel;
using IriskingAttend.Web;
using IriskingAttend.Common;
using System.ServiceModel.DomainServices.Client;
using EDatabaseError;
using System.Linq;

namespace IriskingAttend.ZhongKeHongBa
{
    public class VmPersonLatearrivalCollect:BaseViewModel
    {
        #region 绑定属性
        
        private BaseViewModelCollection<PersonLatearrivalInfo> _personLateArrivalModel;
        /// <summary>
        /// 前台界面绑定的数据集合
        /// </summary>
        public BaseViewModelCollection<PersonLatearrivalInfo> PersonLateArrivalModel
        {
            get
            {
                return _personLateArrivalModel;
            }
            set
            {
                _personLateArrivalModel = value;
                OnPropertyChanged<BaseViewModelCollection<PersonLatearrivalInfo>>(() => this.PersonLateArrivalModel);
            }
        }
      
        #endregion

         #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public VmPersonLatearrivalCollect()
        {
            PersonLateArrivalModel = new BaseViewModelCollection<PersonLatearrivalInfo>();           
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
        public void GetPersonLatearrivalCollect(DateTime beginTime, DateTime endTime, string[] departName, string[] personName)
        {
            WaitingDialog.ShowWaiting("正在查询，请等待");

            ServiceDomDbAcess.ReOpenSever();
            EntityQuery<PersonLatearrivalInfo> list = ServiceDomDbAcess.GetSever().GetPersonLatearrivalListQuery(beginTime, endTime, departName, personName);
            //回调异常类
            Action<LoadOperation<PersonLatearrivalInfo>> actionCallBack = ErrorHandle<PersonLatearrivalInfo>.OnLoadErrorCallBack;
            //异步事件
            LoadOperation<PersonLatearrivalInfo> loadOp = ServiceDomDbAcess.GetSever().Load(list, actionCallBack, null);

            loadOp.Completed += delegate
            {
                try
                {
                    PersonLateArrivalModel.Clear();

                    foreach (PersonLatearrivalInfo item in loadOp.Entities)
                    {
                        PersonLateArrivalModel.Add(item);
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
