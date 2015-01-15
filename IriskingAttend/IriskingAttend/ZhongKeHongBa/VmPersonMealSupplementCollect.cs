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
    public class VmPersonMealSupplementCollect : BaseViewModel
    {
        #region 绑定属性
        private BaseViewModelCollection<UserPersonInfo> _personInfoModel;

        /// <summary>
        /// 前台界面绑定的数据集合
        /// </summary>
        public BaseViewModelCollection<UserPersonInfo> PersonInfoModel
        {
            get
            {
                return _personInfoModel;
            }
            set
            {
                _personInfoModel = value;
                OnPropertyChanged<BaseViewModelCollection<UserPersonInfo>>(() => this.PersonInfoModel);
            }
        }

        private BaseViewModelCollection<UserPersonInfo> _selectPersonInfoModel;

        /// <summary>
        /// 前台界面绑定的数据集合
        /// </summary>
        public BaseViewModelCollection<UserPersonInfo> SelectPersonInfoModel
        {
            get
            {
                return _selectPersonInfoModel;
            }
            set
            {
                _selectPersonInfoModel = value;
                OnPropertyChanged<BaseViewModelCollection<UserPersonInfo>>(() => this.SelectPersonInfoModel);
            }
        }

        private BaseViewModelCollection<PersonMealSuppleInfo> _PersonMealSuppleInfoModel;
        /// <summary>
        /// 前台界面绑定的数据集合
        /// </summary>
        public BaseViewModelCollection<PersonMealSuppleInfo> PersonMealSuppleInfoModel
        {
            get
            {
                return _PersonMealSuppleInfoModel;
            }
            set
            {
                _PersonMealSuppleInfoModel = value;
                OnPropertyChanged<BaseViewModelCollection<PersonMealSuppleInfo>>(() => this.PersonMealSuppleInfoModel);
            }
        }
        private BaseViewModelCollection<TotalAttend> _totalAttendModel;

        /// <summary>
        /// 前台界面绑定的数据集合
        /// </summary>
        public BaseViewModelCollection<TotalAttend> TotalAttendModel
        {
            get
            {
                return _totalAttendModel;
            }
            set
            {
                _totalAttendModel = value;
                OnPropertyChanged<BaseViewModelCollection<TotalAttend>>(() => this.TotalAttendModel);
            }
        }
        /// <summary>
        /// 人员加载完成
        /// </summary>
        public delegate void CompleteCallBack();
        #endregion

         #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public VmPersonMealSupplementCollect()
        {
            PersonMealSuppleInfoModel = new BaseViewModelCollection<PersonMealSuppleInfo>();
            PersonInfoModel = new BaseViewModelCollection<UserPersonInfo>();
            SelectPersonInfoModel = new BaseViewModelCollection<UserPersonInfo>();
            TotalAttendModel = new BaseViewModelCollection<TotalAttend>();
        }

        #endregion

        #region 根据查询条件查询数据并对数据进行处理

        /// <summary>
        /// 异步获取人员信息
        /// </summary>
        public void GetPersonInfo(string[] departName, CompleteCallBack completeCallBack)
        {
            try
            {
                EntityQuery<UserPersonInfo> lstPerson = ServiceDomDbAcess.GetSever().GetPersonInfoByDepartNameQuery(departName);
                ///回调异常类
                Action<LoadOperation<UserPersonInfo>> getPersonCallBack = new Action<LoadOperation<UserPersonInfo>>(ErrorHandle<UserPersonInfo>.OnLoadErrorCallBack);
                ///异步事件
                LoadOperation<UserPersonInfo> lo = ServiceDomDbAcess.GetSever().Load(lstPerson, getPersonCallBack, null);

                lo.Completed += (o, e) =>
                {
                    PersonInfoModel.Clear();

                    foreach (var ar in lo.Entities)
                    {
                        PersonInfoModel.Add(ar);
                    }

                    if (completeCallBack != null)
                    {
                        completeCallBack();
                    }
                };
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }
        public void SelectPersonByName()
        {
            SelectPersonInfoModel.Clear();
            var values = from u in PersonInfoModel
                         orderby u.person_name
                         select u;
            foreach (var item in values)
            {
                if (item.person_name == null || item.person_name == "")
                {
                    continue;
                }
                SelectPersonInfoModel.Add(item);
            }
        }

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
            EntityQuery<PersonMealSuppleInfo> list = ServiceDomDbAcess.GetSever().GetPersonMealSuppleListQuery(beginTime, endTime, departName, personName);
            //回调异常类
            Action<LoadOperation<PersonMealSuppleInfo>> actionCallBack = ErrorHandle<PersonMealSuppleInfo>.OnLoadErrorCallBack;
            //异步事件
            LoadOperation<PersonMealSuppleInfo> loadOp = ServiceDomDbAcess.GetSever().Load(list, actionCallBack, null);

            loadOp.Completed += delegate
            {
                try
                {
                    PersonMealSuppleInfoModel.Clear();

                    foreach (PersonMealSuppleInfo item in loadOp.Entities)
                    {
                        PersonMealSuppleInfoModel.Add(item);
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
        public void GetTotalAttendDetailList(DateTime beginTime, DateTime endTime, string[] departName, string[] personName, string[] workSn)
        {
            WaitingDialog.ShowWaiting("正在查询，请等待");

            ServiceDomDbAcess.ReOpenSever();
            EntityQuery<TotalAttend> list = ServiceDomDbAcess.GetSever().GetTotalAttendDetailListQuery(beginTime, endTime, departName, personName, workSn);
            //回调异常类
            Action<LoadOperation<TotalAttend>> actionCallBack = ErrorHandle<TotalAttend>.OnLoadErrorCallBack;
            //异步事件
            LoadOperation<TotalAttend> loadOp = ServiceDomDbAcess.GetSever().Load(list, actionCallBack, null);

            loadOp.Completed += delegate
            {
                try
                {
                    TotalAttendModel.Clear();

                    foreach (TotalAttend item in loadOp.Entities)
                    {
                        TotalAttendModel.Add(item);
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
