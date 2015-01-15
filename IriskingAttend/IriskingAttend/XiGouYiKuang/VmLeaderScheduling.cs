/*************************************************************************
** 文件名:   VmLeaderScheduling.cs
** 主要类:   VmLeaderScheduling
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2014-4-11
** 修改人:   
** 日  期:
** 描  述:   VmLeaderScheduling,西沟一矿领导排班
**
** 版  本:   1.0.0
** 备  注:  命名及代码编写遵守C#编码规范
**
*************************************************************************/
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
using Irisking.Web.DataModel;
using IriskingAttend.Dialog;

namespace IriskingAttend.XiGouYiKuang
{
    public class VmLeaderScheduling:BaseViewModel
    {
        private BaseViewModelCollection<XiGouLeaderScheduling> _leaderSchedulingList;

        /// <summary>
        /// 前台界面绑定的数据集合
        /// </summary>
        public BaseViewModelCollection<XiGouLeaderScheduling> LeaderSchedulingList
        {
            get
            {
                return _leaderSchedulingList;
            }
            set
            {
                _leaderSchedulingList = value;
                OnPropertyChanged<BaseViewModelCollection<XiGouLeaderScheduling>>(() => this.LeaderSchedulingList);
            }
        }

        private BaseViewModelCollection<UserPersonInfo> _leaderInfoList;

        /// <summary>
        /// 前台界面绑定的数据集合
        /// </summary>
        public BaseViewModelCollection<UserPersonInfo> LeaderInfoList
        {
            get
            {
                return _leaderInfoList;
            }
            set
            {
                _leaderInfoList = value;
                OnPropertyChanged<BaseViewModelCollection<UserPersonInfo>>(() => this.LeaderInfoList);
            }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public VmLeaderScheduling()
        {
            LeaderSchedulingList = new BaseViewModelCollection<XiGouLeaderScheduling>();
            LeaderInfoList = new BaseViewModelCollection<UserPersonInfo>();            
        }

        public BaseViewModelCollection<XiGouLeaderScheduling> GetLeaderSchedulingList(DateTime beginTime, DateTime endTime)
        {
            try
            {
                ServiceDomDbAcess.ReOpenSever();

                //显示等待界面
                WaitingDialog.ShowWaiting();

                //获得设备信息列表
                EntityQuery<XiGouLeaderScheduling> leaderList = ServiceDomDbAcess.GetSever().GetXiGouLeaderSchedulingListQuery(beginTime, endTime);

                ///回调异常类
                Action<LoadOperation<XiGouLeaderScheduling>> loadDevCallBack = ErrorHandle<XiGouLeaderScheduling>.OnLoadErrorCallBack;

                ///异步事件
                LoadOperation<XiGouLeaderScheduling> loadOp = ServiceDomDbAcess.GetSever().Load(leaderList, loadDevCallBack, null);
                loadOp.Completed += delegate
                {
                    //若列表信息未分配内存，则进行内存分配
                    if (LeaderSchedulingList == null)
                    {
                        LeaderSchedulingList = new BaseViewModelCollection<XiGouLeaderScheduling>();
                    }
                    else
                    {
                        //将列表信息清空
                        LeaderSchedulingList.Clear();
                    }

                    //异步获取数据，将获取到的信息添加到LeaderSchedulingList中去
                    foreach (XiGouLeaderScheduling item in loadOp.Entities)
                    {
                        LeaderSchedulingList.Add(item);
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
            return LeaderSchedulingList;
        }

        public BaseViewModelCollection<UserPersonInfo> GetLeaderInfoList()
        {
            try
            {
                ServiceDomDbAcess.ReOpenSever();

                //显示等待界面
                WaitingDialog.ShowWaiting();

                //获得设备信息列表
                EntityQuery<UserPersonInfo> leaderList = ServiceDomDbAcess.GetSever().GetLeaderPersonInfoListQuery();

                ///回调异常类
                Action<LoadOperation<UserPersonInfo>> loadDevCallBack = ErrorHandle<UserPersonInfo>.OnLoadErrorCallBack;

                ///异步事件
                LoadOperation<UserPersonInfo> loadOp = ServiceDomDbAcess.GetSever().Load(leaderList, loadDevCallBack, null);
                loadOp.Completed += delegate
                {
                    //若列表信息未分配内存，则进行内存分配
                    if (LeaderInfoList == null)
                    {
                        LeaderInfoList = new BaseViewModelCollection<UserPersonInfo>();
                    }
                    else
                    {
                        //将列表信息清空
                        LeaderInfoList.Clear();
                    }

                    //异步获取数据，将获取到的信息添加到LeaderSchedulingList中去
                    foreach (UserPersonInfo item in loadOp.Entities)
                    {
                        LeaderInfoList.Add(item);
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
            return LeaderInfoList;
        }

        /// <summary>
        /// 设置领导排班
        /// </summary>        
        public void SetLeaderScheduling(DateTime beginTime, DateTime endTime)
        {
            try
            {
                Action<InvokeOperation<bool>> onInvokeErrCallBack = CallBackHandleControl<bool>.OnInvokeErrorCallBack;
                CallBackHandleControl<bool>.m_sendValue = (o) =>
                {
                    //隐藏等待界面
                    WaitingDialog.HideWaiting();

                    //异步获取数据                    
                    if (o)
                    {
                         MsgBoxWindow.MsgBox("设置领导排班考勤操作成功！", MsgBoxWindow.MsgIcon.Succeed, MsgBoxWindow.MsgBtns.OK);
                    }
                    else
                    {
                        MsgBoxWindow.MsgBox("设置领导排班考勤操作失败！", MsgBoxWindow.MsgIcon.Error, MsgBoxWindow.MsgBtns.OK);
                    }

                };

                //显示等待界面
                WaitingDialog.ShowWaiting();

                //通过后台执行设置领导排班考勤动作
                ServiceDomDbAcess.GetSever().SetLeaderScheduling(LeaderSchedulingList.ToArray(), beginTime, endTime, onInvokeErrCallBack, null);
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }

    }
}
