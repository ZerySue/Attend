/*************************************************************************
** 文件名:   VmInCompleteQuery.cs
** 主要类:   VmInCompleteQuery
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2014-3-20
** 修改人:   
** 日  期:
** 描  述:   VmInCompleteQuery，新巨龙不完整考勤查询
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
    public class VmInCompleteQuery : BaseViewModel
    {
        #region 绑定属性

        private BaseViewModelCollection<AttendInComplete> _inCompleteCollectModel;

        /// <summary>
        /// 前台界面绑定的数据集合
        /// </summary>
        public BaseViewModelCollection<AttendInComplete> InCompleteCollectModel
        {
            get
            {
                return _inCompleteCollectModel;
            }
            set
            {
                _inCompleteCollectModel = value;
                OnPropertyChanged<BaseViewModelCollection<AttendInComplete>>(() => this.InCompleteCollectModel);
            }
        }

        private BaseViewModelCollection<InCompleteRecord> _inCompleteRecordModel;

        /// <summary>
        /// 前台界面绑定的数据集合
        /// </summary>
        public BaseViewModelCollection<InCompleteRecord> InCompleteRecordModel
        {
            get
            {
                return _inCompleteRecordModel;
            }
            set
            {
                _inCompleteRecordModel = value;
                OnPropertyChanged<BaseViewModelCollection<InCompleteRecord>>(() => this.InCompleteRecordModel);
            }
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public VmInCompleteQuery()
        {
            InCompleteCollectModel = new BaseViewModelCollection<AttendInComplete>();
            InCompleteRecordModel = new BaseViewModelCollection<InCompleteRecord>();
        }

        #endregion

        #region 根据查询条件查询数据并对数据进行处理

        /// <summary>
        /// 异步获取不完整考勤查询
        /// </summary>
        public void GetInCompleteQuery(DateTime beginTime, DateTime endTime, List<UserDepartInfo> departInfos, string personName, string workSn) 
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

                EntityQuery<AttendInComplete> lstQuery = ServiceDomDbAcess.GetSever().GetInCompleteCollectQuery(beginTime, endTime, departIdList.ToArray(), personName, workSn);
                ///回调异常类
                Action<LoadOperation<AttendInComplete>> getPersonCallBack = new Action<LoadOperation<AttendInComplete>>(ErrorHandle<AttendInComplete>.OnLoadErrorCallBack);
                ///异步事件
                LoadOperation<AttendInComplete> lo = ServiceDomDbAcess.GetSever().Load(lstQuery, getPersonCallBack, null);

                lo.Completed += (o, e) =>
                {
                    InCompleteCollectModel.Clear();
                  
                    foreach (var ar in lo.Entities)
                    {
                        InCompleteCollectModel.Add(ar);                       
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

        /// <summary>
        /// 异步获取不完整考勤记录
        /// </summary>
        public void GetInCompleteRecord(DateTime beginTime, DateTime endTime, int personId)
        {
            try
            {      
                //显示等待界面
                WaitingDialog.ShowWaiting();

                ServiceDomDbAcess.ReOpenSever();

                EntityQuery<InCompleteRecord> lstQuery = ServiceDomDbAcess.GetSever().GetInCompleteRecordQuery(beginTime, endTime, personId);
                ///回调异常类
                Action<LoadOperation<InCompleteRecord>> getPersonCallBack = new Action<LoadOperation<InCompleteRecord>>(ErrorHandle<InCompleteRecord>.OnLoadErrorCallBack);
                ///异步事件
                LoadOperation<InCompleteRecord> lo = ServiceDomDbAcess.GetSever().Load(lstQuery, getPersonCallBack, null);

                lo.Completed += (o, e) =>
                {
                    InCompleteRecordModel.Clear();

                    foreach (var ar in lo.Entities)
                    {
                        InCompleteRecordModel.Add(ar);
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
