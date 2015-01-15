/*************************************************************************
** 文件名:   VmMainFrm.cs
×× 主要类:   VmMainFrm
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2013-6-14
** 修改人:   
** 日  期:
** 描  述:   VmMainFrm,主界面
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
    public class VmMainFrm : BaseViewModel
    {
        /// <summary>
        /// 与服务声明
        /// </summary>
        private DomainServiceIriskingAttend m_serviceDomDbAccess = new DomainServiceIriskingAttend();

        public VmMainFrm()
        {       
        }
        /// <summary>
        /// 关闭数据库
        /// </summary>
        public void CloseDB()
        {
            try
            {
                Action<InvokeOperation<bool>> callBack = CallBackHandleControl<bool>.OnInvokeErrorCallBack;
                CallBackHandleControl<bool>.m_sendValue = (o) =>
                {
                };
                m_serviceDomDbAccess.CloseDataBase( callBack, null);
            }
            catch (Exception e)
            {
                string s = e.ToString();
            } 
        }
    }

}
