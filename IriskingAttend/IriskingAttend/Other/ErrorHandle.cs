/*************************************************************************
** 文件名:   ErrorHandle.cs
×× 主要类:   ErrorHandle
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   fjf
** 日  期:   2013-01-17
** 修改人:   fjf 
** 日  期:   2013-04-25
** 描  述:   错误控制类
**           增加了invoke的普通回调处理
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
using System.ServiceModel.DomainServices.Client;
using System.Linq;
using System.Collections.Generic;
using IriskingAttend.Common;
//using Example.Portal.Common;

namespace EDatabaseError
{
    //传说中，IE自带RIA的错误调试工具。http://www.cnblogs.com/chenxizhang/archive/2011/10/22/2221009.html
    public  class ErrorHandle<TEntity> 
        where TEntity : global::System.ServiceModel.DomainServices.Client.Entity
    {
        public ErrorHandle()
        { } 

        /// <summary>
        /// 加载调用错误回调函数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        public static void OnLoadErrorCallBack(LoadOperation<TEntity> e)
        {
            if (e.HasError)
            {
                MessageBox.Show("加载错误" + e.Error.ToString());
                e.MarkErrorAsHandled();
                WaitingDialog.HideWaiting();
            }
            if (e.IsComplete)
            {
                //其下：业务逻辑代码
            }
        }
        /// <summary>
        /// 提交保存数据库错误回调函数
        /// </summary>
        /// <param name="so"></param>
        public static void OnSubmitErrorCallBack(SubmitOperation so)
        {
            if (so.HasError)
            {
                MessageBox.Show("提交错误" + so.Error.ToString());
                so.MarkErrorAsHandled();
                WaitingDialog.HideWaiting();
            }
            if (so.IsComplete)
            {
                //其下：业务逻辑代码
            }
        }
        /// <summary>
        /// INVOKE调用错误回调函数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="io"></param>
        public static void OnInvokeErrorCallBack<T>(InvokeOperation<T> io)
        {
            if (io.HasError)
            {
                MessageBox.Show("调用错误" + io.Error.Message.ToString());
                WaitingDialog.HideWaiting();
            }
            else
            {
                //其它逻辑
            }
        }
    }
    /// <summary>
    /// 回调函数处理类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CallBackHandleControl<T>
    {
        #region 数据处理委托
        public static Action<T> m_sendValue = null;   //INVOKE
        public static Action m_snedSubmit   = null;   //SUBMIT
        #endregion
        /// <summary>
        /// INVOKE回调函数
        /// </summary>
        /// <param name="io"></param>
        public static void OnInvokeErrorCallBack(InvokeOperation<T> io)
        {
            if (io.HasError)
            {
                MessageBox.Show("调用错误" + io.Error.Message.ToString());
                WaitingDialog.HideWaiting();
            }
            else if (io.IsComplete)
            {
                //其它逻辑         
                if (null != m_sendValue)
                {
                    m_sendValue(io.Value);
                }
            }
        }
        /// <summary>
        /// 提交保存数据库错误回调函数
        /// </summary>
        /// <param name="so"></param>
        public static void OnSubmitErrorCallBack(SubmitOperation so)
        {
            if (so.HasError)
            {
                MessageBox.Show("提交错误" + so.Error.ToString());
                so.MarkErrorAsHandled();
                WaitingDialog.HideWaiting();
            }
            if (so.IsComplete)
            {
                //其下：业务逻辑代码
                if (null != m_snedSubmit)
                {
                    m_snedSubmit();
                }
            }
        }
    }
}
