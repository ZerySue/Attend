/*************************************************************************
** 文件名:   PrintControl.cs
×× 主要类:   PrintControl
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   fjf
** 日  期:   2013-01-17
** 修改人:   
** 日  期:
** 描  述:   打印控制类
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

using System.Collections.Generic;
using ReportTemplate;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using IriskingAttend.ViewModel;
using IriskingAttend.Common;

namespace IriskingAttend
{

    /// <summary>
    /// 打印接口类，需要提供4个数据
    /// 表头，表内容，表标题，表页脚,绑定数据属性名数组
    /// </summary>
    public class PrintControl
    {
        /// <summary>
        /// 报表打印管理
        /// </summary>
        ReportManager reportManager;


        public PrintControl()
        {
            reportManager = new ReportManager();
        }

        
        /// <summary>
        /// 设置打印数据源
        /// </summary>
        /// <typeparam name="T">表内容数据源类型</typeparam>
        /// <param name="_ReportTitle">表标题</param>
        /// <param name="_ReportFooter">表页脚</param>
        /// <param name="_ReportHeader">表头</param>
        /// <param name="dataSource">表内容</param>
        /// <param name="_PropertiesName">要绑定的属性</param>
        /// <param name="fontsize">表内容字体大小</param>
        /// <returns></returns>
        public bool SetDataSource<T>(ReportTitle reportTitle, 
            ReportFooter reportFooter, ReportHeader reportHeader,
            BaseViewModelCollection<T> dataSource, 
            string[] propertiesName,Action callBack,
            int fontsize = 11)
        {
            
            string opInfo;
            //根据载入的表头数据和报表内容、报表标题、报表页脚创建表
            try
            {
                bool res = reportManager.CreateReport(reportTitle, reportFooter, reportHeader, dataSource, propertiesName, out opInfo, fontsize, callBack);
                if (!res)
                {
                    Dialog.MsgBoxWindow.MsgBox(opInfo, Dialog.MsgBoxWindow.MsgIcon.Error, Dialog.MsgBoxWindow.MsgBtns.OK);
                } 
                return res;
            }
            catch(Exception e)
            {
                WaitingDialog.HideWaiting();
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
            return false;

        }

        /// <summary>
        /// 打印当前页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Print_CurPage(object sender, RoutedEventArgs e)
        {
            reportManager.Print(sender, e);
        }

        /// <summary>
        /// 预览当前打印页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Preview_CurPage(object sender, RoutedEventArgs e)
        {
            reportManager.Preview(sender, e);
        }





    }
}
