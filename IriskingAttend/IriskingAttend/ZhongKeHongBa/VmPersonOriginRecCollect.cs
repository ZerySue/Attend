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

namespace IriskingAttend.ZhongKeHongBa
{
    public class VmPersonOriginRecCollect:BaseViewModel
    {
        #region 绑定属性

        private BaseViewModelCollection<PersonOriginInfo> _PersonOriginInfoModel;
        /// <summary>
        /// 前台界面绑定的数据集合
        /// </summary>
        public BaseViewModelCollection<PersonOriginInfo> PersonOriginInfoModel
        {
            get
            {
                return _PersonOriginInfoModel;
            }
            set
            {
                _PersonOriginInfoModel = value;
                OnPropertyChanged<BaseViewModelCollection<PersonOriginInfo>>(() => this.PersonOriginInfoModel);
            }
        }
      
        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public VmPersonOriginRecCollect()
        {
            PersonOriginInfoModel = new BaseViewModelCollection<PersonOriginInfo>();            
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
            EntityQuery<PersonOriginInfo> list = ServiceDomDbAcess.GetSever().GetPersonOriginRecListQuery(beginTime, endTime, departName, personName);
            //回调异常类
            Action<LoadOperation<PersonOriginInfo>> actionCallBack = ErrorHandle<PersonOriginInfo>.OnLoadErrorCallBack;
            //异步事件
            LoadOperation<PersonOriginInfo> loadOp = ServiceDomDbAcess.GetSever().Load(list, actionCallBack, null);

            loadOp.Completed += delegate
            {
                try
                {
                    PersonOriginInfoModel.Clear();

                    foreach (PersonOriginInfo item in loadOp.Entities)
                    {
                        PersonOriginInfoModel.Add(item);
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
