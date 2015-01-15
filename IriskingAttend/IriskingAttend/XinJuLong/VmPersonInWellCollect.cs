/*************************************************************************
** 文件名:   VmPersonInWellCollect.cs
** 主要类:   VmPersonInWellCollect
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2014-3-19
** 修改人:   
** 日  期:
** 描  述:   VmPersonInWellCollect，新巨龙个人井下出勤统计
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
    public class VmPersonInWellCollect : BaseViewModel
    {
        #region 绑定属性

        private BaseViewModelCollection<AttendPersonInWellQuery> _personInWellCollectModel;

        /// <summary>
        /// 前台界面绑定的数据集合
        /// </summary>
        public BaseViewModelCollection<AttendPersonInWellQuery> PersonInWellCollectModel
        {
            get
            {
                return _personInWellCollectModel;
            }
            set
            {
                _personInWellCollectModel = value;
                OnPropertyChanged<BaseViewModelCollection<AttendPersonInWellQuery>>(() => this.PersonInWellCollectModel);
            }
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public VmPersonInWellCollect()
        {
            PersonInWellCollectModel = new BaseViewModelCollection<AttendPersonInWellQuery>();
        }

        #endregion

        #region 根据查询条件查询数据并对数据进行处理

        /// <summary>
        /// 异步获取人员信息
        /// </summary>
        public void GetAttendPersonInWellCollect(DateTime beginTime, DateTime endTime, List<UserDepartInfo> departInfos, string personName, string workSn, int workTime) 
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

                EntityQuery<AttendPersonInWellQuery> lstQuery = ServiceDomDbAcess.GetSever().GetAttendPersonInWellCollectQuery(beginTime, endTime, departIdList.ToArray(), personName,workSn,workTime);
                ///回调异常类
                Action<LoadOperation<AttendPersonInWellQuery>> getPersonCallBack = new Action<LoadOperation<AttendPersonInWellQuery>>(ErrorHandle<AttendPersonInWellQuery>.OnLoadErrorCallBack);
                ///异步事件
                LoadOperation<AttendPersonInWellQuery> lo = ServiceDomDbAcess.GetSever().Load(lstQuery, getPersonCallBack, null);

                lo.Completed += (o, e) =>
                {
                    PersonInWellCollectModel.Clear();
                  
                    foreach (var ar in lo.Entities)
                    {
                        PersonInWellCollectModel.Add(ar);                       
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
