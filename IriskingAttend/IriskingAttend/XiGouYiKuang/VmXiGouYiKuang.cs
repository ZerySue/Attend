/*************************************************************************
** 文件名:   VmXiGouYiKuang.cs
** 主要类:   VmXiGouYiKuang
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   cty
** 日  期:   2014-4-10
** 修改人:   
** 日  期:
** 描  述:   VmXiGouYiKuang，西沟一矿领导带班考勤表
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
using IriskingAttend.ViewModel;
using System.ComponentModel;
using System.Collections.Generic;
using System.ServiceModel.DomainServices.Client;
using IriskingAttend.Common;
using EDatabaseError;
using System.Linq;
using IriskingAttend.Web;

namespace IriskingAttend.XiGouYiKuang
{
    public class VmXiGouYiKuang:BaseViewModel
    {
        #region 绑定属性
        private BaseViewModelCollection<XiGouLeaderAttend> _leaderAttendList;
        /// <summary>
        /// 前台界面绑定的数据集合
        /// </summary>
        public BaseViewModelCollection<XiGouLeaderAttend> LeaderAttendList
        {
            get
            {
                return _leaderAttendList;
            }
            set
            {
                _leaderAttendList = value;
                OnPropertyChanged<BaseViewModelCollection<XiGouLeaderAttend>>(() => this.LeaderAttendList);
            }
        }

        private BaseViewModelCollection<XiGouLeaderAttend> _PersonleaderAttend;
        /// <summary>
        /// 前台界面绑定的个人出勤明细
        /// </summary>
        public BaseViewModelCollection<XiGouLeaderAttend> PersonLeaderAttend
        {
            get
            {
                return _PersonleaderAttend;
            }
            set
            {
                _PersonleaderAttend = value;
                OnPropertyChanged<BaseViewModelCollection<XiGouLeaderAttend>>(() => this.PersonLeaderAttend);
            }
        }

        private BaseViewModelCollection<XiGouLeaderSchedule> _leaderScheduleList;
        /// <summary>
        /// 前台界面绑定的数据集合
        /// </summary>
        public BaseViewModelCollection<XiGouLeaderSchedule> LeaderScheduleList
        {
            get
            {
                return _leaderScheduleList;
            }
            set
            {
                _leaderScheduleList = value;
                OnPropertyChanged<BaseViewModelCollection<XiGouLeaderSchedule>>(() => this.LeaderScheduleList);
            }
        }

        private BaseViewModelCollection<XiGouLeaderAttend> _PersonleaderScheduleList;
        /// <summary>
        /// 前台界面绑定的数据集合
        /// </summary>
        public BaseViewModelCollection<XiGouLeaderAttend> PersonLeaderScheduleList
        {
            get
            {
                return _PersonleaderScheduleList;
            }
            set
            {
                _PersonleaderScheduleList = value;
                OnPropertyChanged<BaseViewModelCollection<XiGouLeaderAttend>>(() => this.PersonLeaderScheduleList);
            }
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public VmXiGouYiKuang()
        {
            LeaderAttendList = new BaseViewModelCollection<XiGouLeaderAttend>();
            PersonLeaderAttend = new BaseViewModelCollection<XiGouLeaderAttend>();
            LeaderScheduleList = new BaseViewModelCollection<XiGouLeaderSchedule>();
            PersonLeaderScheduleList = new BaseViewModelCollection<XiGouLeaderAttend>();
        }

        #endregion

        #region 获取带班领导考勤信息

        /// <summary>
        /// 获取带班领导考勤信息
        /// </summary>
        /// <param name="beginTime">查询开始时间</param>
        /// <param name="endTime">查询截止时间</param>
        /// <param name="departIdLst">查询部门列表</param>
        /// <returns></returns>
        public BaseViewModelCollection<XiGouLeaderAttend> GetXiGouLeaderAttendList(DateTime beginTime,DateTime endTime, int[] departIdLst)
        {
            WaitingDialog.ShowWaiting("正在查询，请等待...");
            ServiceDomDbAcess.ReOpenSever();
            EntityQuery<XiGouLeaderAttend> list = ServiceDomDbAcess.GetSever().GetXiGouLeaderAttendRecQuery(beginTime, endTime, departIdLst, -1);
            //回调异常类
            Action<LoadOperation<XiGouLeaderAttend>> actionCallBack = ErrorHandle<XiGouLeaderAttend>.OnLoadErrorCallBack;
            //异步事件
            LoadOperation<XiGouLeaderAttend> loadop = ServiceDomDbAcess.GetSever().Load(list, actionCallBack, null);
            loadop.Completed+=delegate
            {
                try
                {
                    LeaderAttendList.Clear();

                    foreach (XiGouLeaderAttend item in loadop.Entities)
                    {
                        LeaderAttendList.Add(item);
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

            return LeaderAttendList;
        }

        #endregion 

        #region 获取带班领导个人考勤信息

        /// <summary>
        /// 获取带班领导个人当月考勤信息
        /// </summary>
        /// <param name="beginTime">查询开始时间</param>
        /// <param name="endTime">查询截止时间</param>
        /// <param name="personId">查询的人员id</param>
        /// <returns></returns>
        public BaseViewModelCollection<XiGouLeaderAttend> GetXiGouPersonLeaderAttend(DateTime beginTime, DateTime endTime, int personId)
        {
            WaitingDialog.ShowWaiting("正在查询，请等待...");
            ServiceDomDbAcess.ReOpenSever();
            EntityQuery<XiGouLeaderAttend> list = ServiceDomDbAcess.GetSever().GetXiGouLeaderAttendRecQuery(beginTime, endTime, null, personId);
            //回调异常类
            Action<LoadOperation<XiGouLeaderAttend>> actionCallBack = ErrorHandle<XiGouLeaderAttend>.OnLoadErrorCallBack;
            //异步事件
            LoadOperation<XiGouLeaderAttend> loadop = ServiceDomDbAcess.GetSever().Load(list, actionCallBack, null);
            loadop.Completed += delegate
            {
                try
                {
                    PersonLeaderAttend.Clear();

                    foreach (XiGouLeaderAttend item in loadop.Entities)
                    {
                        PersonLeaderAttend.Add(item);
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

            return PersonLeaderAttend;
        }

        #endregion


        #region LeaderSchedule查询结果

        public BaseViewModelCollection<XiGouLeaderSchedule> GetXiGouLeaderSchedule(DateTime beginTime, DateTime endTime, int[] departIdLst)
        {
            WaitingDialog.ShowWaiting("正在查询，请等待...");
            ServiceDomDbAcess.ReOpenSever();
            EntityQuery<XiGouLeaderSchedule> list = ServiceDomDbAcess.GetSever().GetXiGouLeaderScheduleRecQuery(beginTime, endTime, departIdLst);
            //回调异常类
            Action<LoadOperation<XiGouLeaderSchedule>> actionCallBack = ErrorHandle<XiGouLeaderSchedule>.OnLoadErrorCallBack;
            //异步事件
            LoadOperation<XiGouLeaderSchedule> loadop = ServiceDomDbAcess.GetSever().Load(list, actionCallBack, null);
            loadop.Completed += delegate
            {
                try
                {
                    LeaderScheduleList.Clear();

                    foreach (XiGouLeaderSchedule item in loadop.Entities)
                    {
                        LeaderScheduleList.Add(item);
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

            return LeaderScheduleList;
        }
        #endregion


        #region 获取带班领导出勤班次明细PersonDetailSchedule

        /// <summary>
        /// 获取带班领导出勤班次明细
        /// </summary>
        /// <param name="beginTime">查询开始时间</param>
        /// <param name="endTime">查询截止时间</param>
        /// <param name="personId">查询的人员id</param>
        /// <returns></returns>
        public BaseViewModelCollection<XiGouLeaderAttend> GetXiGouPersonLeaderSchedule(DateTime beginTime, DateTime endTime,int[]departIdList,string name, int personId)
        {
            WaitingDialog.ShowWaiting("正在查询，请等待...");
            ServiceDomDbAcess.ReOpenSever();
            EntityQuery<XiGouLeaderAttend> list = ServiceDomDbAcess.GetSever().GetXiGouPersonLeaderScheuleRecQuery(beginTime, endTime, departIdList, name, personId);
            //回调异常类
            Action<LoadOperation<XiGouLeaderAttend>> actionCallBack = ErrorHandle<XiGouLeaderAttend>.OnLoadErrorCallBack;
            //异步事件
            LoadOperation<XiGouLeaderAttend> loadop = ServiceDomDbAcess.GetSever().Load(list, actionCallBack, null);
            loadop.Completed += delegate
            {
                try
                {
                    PersonLeaderScheduleList.Clear();

                    foreach (XiGouLeaderAttend item in loadop.Entities)
                    {
                        PersonLeaderScheduleList.Add(item);
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

            return PersonLeaderAttend;
        }

        #endregion
    }
}
