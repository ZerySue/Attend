/*************************************************************************
** 文件名:   VmXiGouInWellPerson.cs
** 主要类:   VmXiGouInWellPerson
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   yht
** 日  期:   2014-10-29
** 修改人:   
** 日  期:
** 描  述:   VmXiGouAttendReport，虎峰煤矿日报表、月报表vm
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
using Irisking.Web.DataModel;
using IriskingAttend.ViewModel;
using IriskingAttend.Web;
using IriskingAttend.Common;
using System.ServiceModel.DomainServices.Client;
using EDatabaseError;

namespace IriskingAttend.XiGouYiKuang
{
    public class VmXiGouInWellPerson : BaseViewModel
    {
        /// <summary>
        /// 绑定数据源 当前井下人员
        /// </summary>
        private BaseViewModelCollection<XiGouInWellPerson> xiGouInWellPersonModel;

        public BaseViewModelCollection<XiGouInWellPerson> XiGouInWellPersonModel
        {
            get
            {
                return xiGouInWellPersonModel;
            }
            set
            {
                xiGouInWellPersonModel = value;
                OnPropertyChanged<BaseViewModelCollection<XiGouInWellPerson>>(() => this.XiGouInWellPersonModel);
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public VmXiGouInWellPerson()
        {
            XiGouInWellPersonModel = new BaseViewModelCollection<XiGouInWellPerson>();
        }

        #region 井下人员统计

        /// <summary>
        /// 井下人员统计
        /// </summary>
        /// <param name="beginTime">查询开始时间</param>
        /// <param name="endTime">查询截止时间</param>
        /// <returns></returns>
        public BaseViewModelCollection<XiGouInWellPerson> GetXiGouInWellList(DateTime beginTime, DateTime endTime)
        {
            WaitingDialog.ShowWaiting("正在查询，请等待...");
            ServiceDomDbAcess.ReOpenSever();
            EntityQuery<XiGouInWellPerson> list = ServiceDomDbAcess.GetSever().GetXiGouInWellListQuery(beginTime, endTime);
            //回调异常类
            Action<LoadOperation<XiGouInWellPerson>> actionCallBack = ErrorHandle<XiGouInWellPerson>.OnLoadErrorCallBack;
            //异步事件
            LoadOperation<XiGouInWellPerson> loadop = ServiceDomDbAcess.GetSever().Load(list, actionCallBack, null);
            loadop.Completed += delegate
            {
                try
                {
                    XiGouInWellPersonModel.Clear();

                    foreach (XiGouInWellPerson item in loadop.Entities)
                    {
                        XiGouInWellPersonModel.Add(item);
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

            return XiGouInWellPersonModel;
        }

        #endregion 

    }
}
