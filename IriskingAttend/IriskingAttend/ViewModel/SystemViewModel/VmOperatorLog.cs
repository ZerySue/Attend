/*************************************************************************
** 文件名:   VmOperatorLog.cs
** 主要类:   VmOperatorLog
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2013-6-14
** 修改人:   cty
** 日  期:   2014-3-11
 * 修改内容：添加五虎山项目中只有admin用户可见可操作的‘清除无用记录’的功能
** 描  述:   VmOperatorLog,操作员管理
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
using Microsoft.Practices.Prism.Commands;
using System.Collections.Generic;
using System.ServiceModel.DomainServices.Client;
using Irisking.Web.DataModel;
using EDatabaseError;
using IriskingAttend.Web;
using IriskingAttend.Common;
using System.IO.IsolatedStorage;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using IriskingAttend.View.SystemView;
using IriskingAttend.Dialog;

namespace IriskingAttend.ViewModel.SystemViewModel
{
    public class VmOperatorLog : BaseViewModel
    {
        #region 公共变量：回调函数声明

        //回调函数声明
        public delegate void CompleteCallBack();

        #endregion

        #region 与页面绑定的属性

        /// <summary>
        /// 操作员日志信息表
        /// </summary>
        private BaseViewModelCollection<UserOperationLog> _operatorLogMng;
        public BaseViewModelCollection<UserOperationLog> OperatorLogMng
        {
            get { return _operatorLogMng; }
            set
            {
                _operatorLogMng = value;
                OnPropertyChanged<BaseViewModelCollection<UserOperationLog>>(() => this.OperatorLogMng); 
            }
        }

        /// <summary>
        /// 选中的操作员日志信息
        /// </summary>
        private UserOperationLog _selectOperatorLogItem;
        public UserOperationLog SelectOperatorLogItem
        {
            get
            {
                return _selectOperatorLogItem;
            }
            set
            {
                _selectOperatorLogItem = value;
                OnPropertyChanged<UserOperationLog>(() => this.SelectOperatorLogItem); 
            }
        }

        #endregion

        #region  构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public VmOperatorLog()
        {
            //绑定的变量属性初始化
            OperatorLogMng = new BaseViewModelCollection<UserOperationLog>();
            SelectOperatorLogItem = new UserOperationLog();
        }

        #endregion   
     
        #region 函数

        /// <summary>
        /// 插入数据库操作员日志
        /// </summary>
        /// <param name="result">成功还是失败：0：失败；1：成功</param>
        /// <param name="content">内容：例如：用户登录，增加新员工等</param>
        /// <param name="description">对结果的描述：例如：登录成功，输入用户名错误等</param>
        public static void InsertOperatorLog(short result, string content, string description, CompleteCallBack completeCallBack)
        {
            UserOperationLog log = new UserOperationLog();
            log.user_id = VmLogin.GetUserID();
            log.user_name = VmLogin.GetUserName();
            log.user_ip = Application.Current.Host.InitParams["ClientIP"];
            log.operation_time = DateTime.Now;
            log.result = result;
            log.content = content;
            log.description = description;
            InsertOperatorLogRia(log, completeCallBack);
        }
    
        #endregion

        #region  通过ria连接后台，数据库相关操作

        /// <summary>
        /// 异步获取数据库中数据，获取操作员日志表
        /// </summary>
        public void GetOperatorLogTableRia( int maxRecordCount)
        {
            try
            {                
                //显示等待界面
                WaitingDialog.ShowWaiting();

                //获得操作员日志列表
                EntityQuery<UserOperationLog> logList = ServiceDomDbAcess.GetSever().GetUserOperationLogByRecordCountQuery(maxRecordCount);

                ///回调异常类
                Action<LoadOperation<UserOperationLog>> loadOperatorCallBack = ErrorHandle<UserOperationLog>.OnLoadErrorCallBack;
                ///异步事件
                LoadOperation<UserOperationLog> loadOp = ServiceDomDbAcess.GetSever().Load(logList, loadOperatorCallBack, null);
                loadOp.Completed += delegate
                {
                    //若OperatorLogMng操作员日志表未分配内存，则进行内存分配
                    if (OperatorLogMng == null)
                    {
                        OperatorLogMng = new BaseViewModelCollection<UserOperationLog>();
                    }
                    else
                    {
                        //将操作员日志信息清空
                        OperatorLogMng.Clear();
                    }

                    //异步获取数据，将获取到的操作员日志添加到OperatorLogMng中去
                    foreach (UserOperationLog opInfo in loadOp.Entities)
                    {
                        OperatorLogMng.Add(opInfo);
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
        /// 异步获取数据库中数据，获取操作员日志表
        /// </summary>
        public void GetOperatorLogTableRia(string startTime, string endTime, string logName )
        {
            try
            {                
                if (startTime != "" && endTime != "")
                {
                    DateTime dataBegin;
                    DateTime dataEnd;

                    try
                    {
                        dataBegin = DateTime.Parse(startTime);
                        dataEnd = DateTime.Parse(endTime);
                    }
                    catch
                    {
                        MsgBoxWindow.MsgBox(
                                   "您输入的时间格式不正确，请重新输入！",
                                   Dialog.MsgBoxWindow.MsgIcon.Information,
                                   Dialog.MsgBoxWindow.MsgBtns.OK);
                        return;
                    }

                    if (dataBegin > dataEnd)
                    {
                        MsgBoxWindow.MsgBox(
                                    "请确定开始时间早于截止时间！",
                                    Dialog.MsgBoxWindow.MsgIcon.Information,
                                    Dialog.MsgBoxWindow.MsgBtns.OK);
                        return;
                    }
                }
                //显示等待界面
                WaitingDialog.ShowWaiting();

                //获得操作员日志列表
                EntityQuery<UserOperationLog> logList = ServiceDomDbAcess.GetSever().GetUserOperationLogQuery(startTime, endTime, logName);

                ///回调异常类
                Action<LoadOperation<UserOperationLog>> loadOperatorCallBack = ErrorHandle<UserOperationLog>.OnLoadErrorCallBack;
                ///异步事件
                LoadOperation<UserOperationLog> loadOp = ServiceDomDbAcess.GetSever().Load(logList, loadOperatorCallBack, null);
                loadOp.Completed += delegate
                {
                    //若OperatorLogMng操作员日志表未分配内存，则进行内存分配
                    if (OperatorLogMng == null)
                    {
                        OperatorLogMng = new BaseViewModelCollection<UserOperationLog>();
                    }
                    else
                    {
                        //将操作员日志信息清空
                        OperatorLogMng.Clear();
                    }

                    //异步获取数据，将获取到的操作员日志添加到OperatorLogMng中去
                    foreach (UserOperationLog opInfo in loadOp.Entities)
                    {
                        OperatorLogMng.Add(opInfo);
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
        /// ria方式调用后台插入操作员日志信息
        /// </summary>
        private static void InsertOperatorLogRia(UserOperationLog log, CompleteCallBack completeCallBack)
        {
            try
            {
                Action<InvokeOperation<bool>> onInvokeErrCallBack = CallBackHandleControl<bool>.OnInvokeErrorCallBack;
                CallBackHandleControl<bool>.m_sendValue = (o) =>
                {
                    if (completeCallBack != null)
                    {
                        completeCallBack();
                    }
                };
               
                //通过后台执行插入操作员日志动作
                ServiceDomDbAcess.GetSever().AddOperatorLog(log, onInvokeErrCallBack, null);
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }

        /// <summary>
        /// 清除无用记录(五虎山admin用户才有该功能) by cty 2014-3-11 采用打标记的方式删除
        /// </summary>
        /// <param name="deleteUserOperationLog"></param>
        public void DeleteUserOperationLog(UserOperationLog deleteUserOperationLog)
        {
            MsgBoxWindow.MsgBox("您确定要清除该条操作员记录吗？", MsgBoxWindow.MsgIcon.Question, MsgBoxWindow.MsgBtns.OKCancel, (result) => 
            {
                if (result == MsgBoxWindow.MsgResult.OK)
                {
                    WaitingDialog.ShowWaiting();  
                    try
                    {
                        Action<InvokeOperation<bool>> onInvokeErrCallBack = CallBackHandleControl<bool>.OnInvokeErrorCallBack;
                        CallBackHandleControl<bool>.m_sendValue = (o) =>
                        {
                            if (o)
                            {
                                MsgBoxWindow.MsgBox("清除无用记录成功！", MsgBoxWindow.MsgIcon.Succeed, MsgBoxWindow.MsgBtns.OK);
                                OperatorLogMng.Remove(deleteUserOperationLog);
                                //GetOperatorLogTableRia(500);
                            }
                            else
                            {
                                MsgBoxWindow.MsgBox("清除无用记录失败！", MsgBoxWindow.MsgIcon.Warning, MsgBoxWindow.MsgBtns.OK);
                            }
                        };

                        //通过后台执行插入操作员日志动作
                        ServiceDomDbAcess.GetSever().DeleteOperatorLog(deleteUserOperationLog, onInvokeErrCallBack, null);
                    }
                    catch (Exception e)
                    {
                        ErrorWindow err = new ErrorWindow(e);
                        err.Show();
                    }
                    WaitingDialog.HideWaiting();
                }
            });
        }
        #endregion        
    }
}
