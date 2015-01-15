/*************************************************************************
** 文件名:   AppTypePublic.cs
** 主要类:   AppTypePublic
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2013-12-11
** 修改人:   szr
** 日  期:   2014-11-14
** 描  述:   AppTypePublic，应用程序类型公共类
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
using System.ServiceModel.DomainServices.Client;
using EDatabaseError;
using IriskingAttend.ViewModel;
using System.Reflection;
using System.Diagnostics;

namespace IriskingAttend.ApplicationType
{
    public class AppTypePublic
    {
        /// <summary>
        /// 是否是矿山考勤： false：非矿山  true：矿山
        /// </summary>
        private static bool _isMine = false;

        /// <summary>
        /// 获得是否是矿山应用程序
        /// </summary>
        /// <returns>false：非矿山  true：矿山</returns>
        public static bool GetIsMineApplication()
        {
            return _isMine;
        }

        /// <summary>
        /// 具体的应用类型
        /// </summary>
        private static string _appType = "";
        
        //回调函数声明
        public delegate void CompleteCallBack();

        private static AbstractApp IsMineApp;
        private static AbstractApp CustomApp;

        /// <summary>
        /// 是否支持签到班、记工时班次 0 都不支持  1为支持 签到班  2支持 记工时班次 szr
        /// </summary>
        private static int _isSupportClassOrderSign =0;

        /// <summary>
        /// 获得是否支持签到班
        /// </summary>
        /// <returns>true为支持  false为不支持</returns>
        public static int GetIsSupportClassOrderSign()
        {

            return _isSupportClassOrderSign;
        }

        /// <summary>
        /// 是否为矿山应用类型
        /// </summary>
        /// <returns></returns>
        public static AbstractApp GetIsMineAppType()
        {
            return IsMineApp;
        }

        /// <summary>
        /// 具体的应用类型
        /// </summary>
        /// <returns></returns>
        public static AbstractApp GetCustomAppType()
        {
            return CustomApp;
        }

        #region ria方式调用后台

        /// <summary>
        /// ria方式调用后台获得应用程序类型
        /// </summary>
        public static void GetIsMineRia(CompleteCallBack completeCallBack)
        {
            try
            {
                Action<InvokeOperation<bool>> onInvokeErrCallBack = CallBackHandleControl<bool>.OnInvokeErrorCallBack;
                CallBackHandleControl<bool>.m_sendValue = (o) =>
                {
                    //将应用程序类型赋给变量，整个程序都将利用此变量来获得应用程序类型。
                    _isMine = o;

                    if (completeCallBack != null)
                    {
                        completeCallBack();
                    }
                };

                //调用后台,获得应用程序类型
                ServiceDomDbAcess.GetSever().GetIsMineApp(onInvokeErrCallBack, null);
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }

        /// <summary>
        /// ria方式调用后台获得应用程序类型
        /// </summary>
        public static void GetAppTypeRia(CompleteCallBack completeCallBack)
        {
            try
            {
                Action<InvokeOperation<string>> onInvokeErrCallBack = CallBackHandleControl<string>.OnInvokeErrorCallBack;
                CallBackHandleControl<string>.m_sendValue = (o) =>
                {
                    //将应用程序类型赋给变量，整个程序都将利用此变量来获得应用程序类型。
                    _appType = o;

                    if (completeCallBack != null)
                    {
                        completeCallBack();
                    }
                };

                //调用后台,获得应用程序类型
                ServiceDomDbAcess.GetSever().GetApplicationType(onInvokeErrCallBack, null);
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }

        /// <summary>
        /// ria方式调用后台，获得是否支持签到班
        /// </summary>
        public static void GetIsSupportClassOrderSignRia(CompleteCallBack completeCallBack)
        {
            try
            {
                Action<InvokeOperation<int>> onInvokeErrCallBack = CallBackHandleControl<int>.OnInvokeErrorCallBack;
                CallBackHandleControl<int>.m_sendValue = (o) =>
                {
                    //将是否支持签到班赋给变量，整个程序都将利用此变量来获得是否支持签到班。
                    _isSupportClassOrderSign = o;

                    if (completeCallBack != null)
                    {
                        completeCallBack();
                    }
                };

                //调用后台,获得是否支持签到班
               ServiceDomDbAcess.GetSever().GetIsSupportClassOrderSign(onInvokeErrCallBack, null);
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }

        public static void InitApp()
        {
            if (GetIsMineApplication())
            {
                IsMineApp = new StandardMineApp();
            }
            else
            {
                IsMineApp = new StandardNotMineApp();
            }

            if (_appType == "")
            {
                CustomApp = IsMineApp;
            }
            else
            {
                object obj = null;
                try
                {
                    _appType = "IriskingAttend.ApplicationType." + _appType;
                    Type objType = Type.GetType(_appType, true);
                    obj = Activator.CreateInstance(objType, IsMineApp);
                    CustomApp = (AbstractApp)obj;
                }
                catch( Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    CustomApp = IsMineApp;
                }                
            }

        }

        #endregion
    }
}
