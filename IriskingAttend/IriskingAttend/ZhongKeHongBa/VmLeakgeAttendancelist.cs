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

namespace IriskingAttend.ZhongKeHongBa
{
    public class VmLeakgeAttendancelist:BaseViewModel
    {
        #region 绑定属性
      
        private BaseViewModelCollection<LeakageAttendance> _LeakageAttendanceModel;

        /// <summary>
        /// 前台界面绑定的数据集合
        /// </summary>
        public BaseViewModelCollection<LeakageAttendance> LeakageAttendanceModel
        {
            get
            {
                return _LeakageAttendanceModel;
            }
            set
            {
                _LeakageAttendanceModel = value;
                OnPropertyChanged<BaseViewModelCollection<LeakageAttendance>>(() => this.LeakageAttendanceModel);
            }
        }
      
        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public VmLeakgeAttendancelist()
        {
            LeakageAttendanceModel = new BaseViewModelCollection<LeakageAttendance>();                   
        }

        #endregion

        #region 根据查询条件查询数据并对数据进行处理
        
        /// <summary>
        /// 异步获取漏考勤查询
        /// </summary>
        public void GetLeakageListQuery(DateTime beginTime, DateTime endTime, string[] departName, string[] personName)
        {
            WaitingDialog.ShowWaiting("正在查询，请等待");

            ServiceDomDbAcess.ReOpenSever();

           EntityQuery<LeakageAttendance> list = ServiceDomDbAcess.GetSever().GetLeakageAttendanceCollectQuery(beginTime, endTime, departName, personName);
           //回调异常类
           Action<LoadOperation<LeakageAttendance>> actionCallBack = ErrorHandle<LeakageAttendance>.OnLoadErrorCallBack;
           //异步事件
           LoadOperation<LeakageAttendance> loadOp = ServiceDomDbAcess.GetSever().Load(list, actionCallBack, null);
           loadOp.Completed += delegate
           {
               try
               {
                   LeakageAttendanceModel.Clear();

                   foreach (LeakageAttendance item in loadOp.Entities)
                   {
                       LeakageAttendanceModel.Add(item);
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
