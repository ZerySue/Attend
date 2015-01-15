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

namespace IriskingAttend.ZhongKeHongBa
{
    public class VmPersonFullAttendCollect : BaseViewModel
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

        private BaseViewModelCollection<PersonFullAttendInfo> _personAttendModel;
        /// <summary>
        /// 前台界面绑定的数据集合
        /// </summary>
        public BaseViewModelCollection<PersonFullAttendInfo> PersonAttendModel
        {
            get
            {
                return _personAttendModel;
            }
            set
            {
                _personAttendModel = value;
                OnPropertyChanged<BaseViewModelCollection<PersonFullAttendInfo>>(() => this.PersonAttendModel);
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
        public VmPersonFullAttendCollect()
        {
            PersonAttendModel = new BaseViewModelCollection<PersonFullAttendInfo>();
            PersonInfoModel = new BaseViewModelCollection<UserPersonInfo>();
            SelectPersonInfoModel = new BaseViewModelCollection<UserPersonInfo>();          
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
            EntityQuery<PersonFullAttendInfo> list = ServiceDomDbAcess.GetSever().GetFullPersonListQuery(beginTime, endTime, departName, personName);
            //回调异常类
            Action<LoadOperation<PersonFullAttendInfo>> actionCallBack = ErrorHandle<PersonFullAttendInfo>.OnLoadErrorCallBack;
            //异步事件
            LoadOperation<PersonFullAttendInfo> loadOp = ServiceDomDbAcess.GetSever().Load(list, actionCallBack, null);

            loadOp.Completed += delegate
            {
                try
                {
                    PersonAttendModel.Clear();

                    foreach (PersonFullAttendInfo item in loadOp.Entities)
                    {
                        PersonAttendModel.Add(item);
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
