/*************************************************************************
** 文件名:   App.cs
** 主要类:   App
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2013-6-14
** 修改人:   
** 日  期:
** 描  述:   App,主程序入口
**
** 版  本:   1.0.0
** 备  注:  命名及代码编写遵守C#编码规范
**
*************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using IriskingAttend.ViewModel.SystemViewModel;
using IriskingAttend.ApplicationType;

namespace IriskingAttend
{
    public partial class App : Application
    {
        #region 变量

        //根网格
        private Grid rootGrid =new Grid();

        //应用程序内的主Frame
        private FrameNavigate MainFrame = new FrameNavigate();

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public App()
        {
            this.Startup += this.Application_Startup;
            this.Exit += this.Application_Exit;
            this.UnhandledException += this.Application_UnhandledException;
            InitializeComponent();
            this.RootVisual = rootGrid;
        }

        #endregion

        #region 事件响应

        /// <summary>
        /// 应用程序启动事件：load主页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            AppTypePublic.GetIsMineRia(() =>
                            {
                                AppTypePublic.GetAppTypeRia(() =>
                                {
                                    AppTypePublic.InitApp();
                                    rootGrid.Children.Add(MainFrame);  
                                });
                                
                            });                          
        }

        /// <summary>
        /// 应用程序退出事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Application_Exit(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 应用程序错误处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            // 如果应用程序是在调试器外运行的，则使用浏览器的
            // 异常机制报告该异常。在 IE 上，将在状态栏中用一个 
            // 黄色警报图标来显示该异常，而 Firefox 则会显示一个脚本错误。
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                // 注意: 这使应用程序可以在已引发异常但尚未处理该异常的情况下继续运行。 
                // 对于生产应用程序，此错误处理应替换为向网站报告错误并停止应用程序。
                e.Handled = true;
                Deployment.Current.Dispatcher.BeginInvoke(delegate { ReportErrorToDOM(e); });
            }
        }

        #endregion

        #region 辅助函数
        /// <summary>
        /// 错误处理时启动的事件
        /// </summary>
        /// <param name="e"></param>
        private void ReportErrorToDOM(ApplicationUnhandledExceptionEventArgs e)
        {
            try
            {
                string errorMsg = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;
                errorMsg = errorMsg.Replace('"', '\'').Replace("\r\n", @"\n");

                System.Windows.Browser.HtmlPage.Window.Eval("throw new Error(\"Unhandled Error in Silverlight Application " + errorMsg + "\");");
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 导航到指定页面
        /// </summary>
        /// <param name="url">指定页面uri</param>
        public void Navigation(Uri url)
        {
            MainFrame.ContentFrame.Source = url;
        }

        //public static void Navigation(Page newPage)
        //{
        //    //获取当前的Appliaction实例
        //    App currentApp = (App)Application.Current;
        //    //修改当前显示页面内容.   
        //    Page oldPage = currentApp.rootGrid.Children[0] as Page;
            
        //    currentApp.rootGrid.Children.Remove(oldPage);
        //    currentApp.rootGrid.Children.Clear();
        //    currentApp.rootGrid.Children.Add(newPage);
        //}

        #endregion

    }
}
