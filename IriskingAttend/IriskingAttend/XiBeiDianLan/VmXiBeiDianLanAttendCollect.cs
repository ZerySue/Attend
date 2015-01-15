/*************************************************************************
** 文件名:   VmXiBeiDianLanAttendCollect.cs
** 主要类:   VmXiBeiDianLanAttendCollect
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2014-5-28
** 修改人:   
** 日  期:
** 描  述:   VmXiBeiDianLanAttendCollect，西北电缆厂出勤汇总表
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

namespace IriskingAttend.XiBeiDianLan
{    
    public class VmXiBeiDianLanAttendCollect : BaseViewModel
    {
        #region 绑定属性

        private BaseViewModelCollection<OfficeAttend> _officeAttendModel;

        /// <summary>
        /// 前台界面绑定的数据集合
        /// </summary>
        public BaseViewModelCollection<OfficeAttend> OfficeAttendModel
        {
            get
            {
                return _officeAttendModel;
            }
            set
            {
                _officeAttendModel = value;
                OnPropertyChanged<BaseViewModelCollection<OfficeAttend>>(() => this.OfficeAttendModel);
            }
        }

        private BaseViewModelCollection<PersonAttendRecord> _attendRecordModel;

        /// <summary>
        /// 前台界面绑定的数据集合
        /// </summary>
        public BaseViewModelCollection<PersonAttendRecord> AttendRecordModel
        {
            get
            {
                return _attendRecordModel;
            }
            set
            {
                _attendRecordModel = value;
                OnPropertyChanged<BaseViewModelCollection<PersonAttendRecord>>(() => this.AttendRecordModel);
            }
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public VmXiBeiDianLanAttendCollect()
        {
            OfficeAttendModel = new BaseViewModelCollection<OfficeAttend>();
            AttendRecordModel = new BaseViewModelCollection<PersonAttendRecord>();
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
        public void GetOfficeAttendCollect(DateTime beginTime, DateTime endTime, string[] departName, string personName, string workSn)
        {                
            WaitingDialog.ShowWaiting("正在查询，请等待");

            ServiceDomDbAcess.ReOpenSever();
            EntityQuery<OfficeAttend> list = ServiceDomDbAcess.GetSever().GetOfficeAttendQuery(beginTime, endTime, departName, personName, workSn);
            //回调异常类
            Action<LoadOperation<OfficeAttend>> actionCallBack = ErrorHandle<OfficeAttend>.OnLoadErrorCallBack;
            //异步事件
            LoadOperation<OfficeAttend> loadOp = ServiceDomDbAcess.GetSever().Load(list, actionCallBack, null);
            
            loadOp.Completed += delegate
            {
                try
                {
                    OfficeAttendModel.Clear();

                    foreach (OfficeAttend item in loadOp.Entities)
                    {
                        OfficeAttendModel.Add( item );
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
        /// 异步获取考勤记录
        /// </summary>
        public void GetPersonAttendRecord(DateTime beginTime, DateTime endTime, int personId, int attendType )
        {
            try
            {
                //显示等待界面
                WaitingDialog.ShowWaiting();

                ServiceDomDbAcess.ReOpenSever();

                EntityQuery<PersonAttendRecord> lstQuery = ServiceDomDbAcess.GetSever().GetPersonAttendRecordQuery(beginTime, endTime, personId, attendType);
                ///回调异常类
                Action<LoadOperation<PersonAttendRecord>> getPersonCallBack = new Action<LoadOperation<PersonAttendRecord>>(ErrorHandle<PersonAttendRecord>.OnLoadErrorCallBack);
                ///异步事件
                LoadOperation<PersonAttendRecord> lo = ServiceDomDbAcess.GetSever().Load(lstQuery, getPersonCallBack, null);

                lo.Completed += (o, e) =>
                {
                    AttendRecordModel.Clear();

                    foreach (var ar in lo.Entities)
                    {
                        AttendRecordModel.Add(ar);
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
