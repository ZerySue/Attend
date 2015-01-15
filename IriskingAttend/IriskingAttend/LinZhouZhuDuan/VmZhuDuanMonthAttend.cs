/*************************************************************************
** 文件名:   VmZhuDuanAttendCollect.cs
** 主要类:   VmZhuDuanAttendCollect
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2014-7-18
** 修改人:   
** 日  期:
** 描  述:   VmZhuDuanAttendCollect，林州铸锻出勤汇总表
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
using System.Linq;

namespace IriskingAttend.LinZhouZhuDuan
{
    public class VmZhuDuanMonthAttend : BaseViewModel
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

        private BaseViewModelCollection<ZhuDuanMonthAttendReport> _zhuDuanMonthAttendModel;

        /// <summary>
        /// 前台界面绑定的数据集合
        /// </summary>
        public BaseViewModelCollection<ZhuDuanMonthAttendReport> ZhuDuanMonthAttendModel
        {
            get
            {
                return _zhuDuanMonthAttendModel;
            }
            set
            {
                _zhuDuanMonthAttendModel = value;
                OnPropertyChanged<BaseViewModelCollection<ZhuDuanMonthAttendReport>>(() => this.ZhuDuanMonthAttendModel);
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
        public VmZhuDuanMonthAttend()
        {            
            PersonInfoModel = new BaseViewModelCollection<UserPersonInfo>();
            SelectPersonInfoModel = new BaseViewModelCollection<UserPersonInfo>();
            ZhuDuanMonthAttendModel = new BaseViewModelCollection<ZhuDuanMonthAttendReport>();
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

        public void SelectPersonByWorkSn()
        {
            SelectPersonInfoModel.Clear();
            var values = from u in PersonInfoModel
                         orderby u.work_sn
                         select u;
            foreach (var item in values)
            {
                if (item.work_sn == null || item.work_sn == "")
                {
                    continue;
                }
                SelectPersonInfoModel.Add(item);
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
        public void GetZhuDuanMonthAttendList(DateTime beginTime, DateTime endTime, string[] departName, string[] personName, string[] workSn)
        {
            WaitingDialog.ShowWaiting("正在查询，请等待");

            ServiceDomDbAcess.ReOpenSever();
            EntityQuery<ZhuDuanMonthAttendReport> list = ServiceDomDbAcess.GetSever().GetZhuDuanMonthAttendListQuery(beginTime, endTime, departName, personName, workSn);
            //回调异常类
            Action<LoadOperation<ZhuDuanMonthAttendReport>> actionCallBack = ErrorHandle<ZhuDuanMonthAttendReport>.OnLoadErrorCallBack;
            //异步事件
            LoadOperation<ZhuDuanMonthAttendReport> loadOp = ServiceDomDbAcess.GetSever().Load(list, actionCallBack, null);

            loadOp.Completed += delegate
            {
                try
                {
                    ZhuDuanMonthAttendModel.Clear();

                    foreach (ZhuDuanMonthAttendReport item in loadOp.Entities)
                    {
                        ZhuDuanMonthAttendModel.Add(item);
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
