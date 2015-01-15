/*************************************************************************
** 文件名:   Login.cs
** 主要类:   Login
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2013-6-14
** 修改人:   
** 日  期:
** 描  述:   Login类,登录界面
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
using System.Windows.Browser;
using System.IO;
using System.Windows.Media.Imaging;
using IriskingAttend.ApplicationType;
using IriskingAttend.ZhouYuanShan;
using IriskingAttend.BehaviorSelf;

namespace IriskingAttend
{
	public partial class Login : Page
    {
        #region 变量初始化

        //vm变量初始化
        private VmLogin _vmLogin = new VmLogin();

        #endregion

        #region 构造函数

        public Login()
		{
			InitializeComponent();

            txtAppName.Text = Version.GetFirstVersion();
			
            //页面启动事件初始化
            this.Loaded += new RoutedEventHandler(login_Loaded);  
          
            //vm赋值
            this.DataContext = _vmLogin;

            AppTypePublic.GetCustomAppType().SetAppLogo();
            imageLogo.Source = AppTypePublic.GetCustomAppType().GetLogoImage();
            txtLogo.Text = AppTypePublic.GetCustomAppType().GetLogoText();
		}

        #endregion

        #region 事件响应

        /// <summary>
        /// 重置按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReset_Click(object sender, RoutedEventArgs e)
		{
            //清空用户名和密码输入框
            txtUserName.Text = "";
            txtPwd.Password = "";
		}       

        /// <summary>
        /// 页面启动事件：将焦点定位到用户名上
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void login_Loaded(object sender, RoutedEventArgs e)
        {
            //silverlight插件聚焦,先把焦点放到silverlight中来
            HtmlPage.Plugin.Focus();

            //再把焦点聚焦到用户名输入框
            this.txtUserName.Focus();
            //从cookie取出用户名密码 add by yuhaitao
            if (!string.IsNullOrEmpty(SLCookieOperate.GetCookie("UserName")))
            {
                this.txtUserName.Text = SLCookieOperate.GetCookie("UserName");
                chkRemember.IsChecked = true;
            }
            if (!string.IsNullOrEmpty(SLCookieOperate.GetCookie("Password")))
            {
                txtPwd.Password = SLCookieOperate.GetCookie("Password");
            }
        }

        /// <summary>
        /// 登录按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            _vmLogin.SubmitLogin(this.txtUserName.Text, this.txtPwd.Password, chkRemember.IsChecked);
        }

        /// <summary>
        /// 填写用户信息完毕后，回车键的响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            //判断按钮是否为回车键
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                _vmLogin.SubmitLogin(this.txtUserName.Text, this.txtPwd.Password, chkRemember.IsChecked);                
            }
            else
            {
                e.Handled = false;
            }
        }

        #endregion        

        private void chkRemember_Click(object sender, RoutedEventArgs e)
        {
            //清空 cookie的用户名密码 add by yuhaitao
            if (chkRemember.IsChecked==false)
            {
                SLCookieOperate.DeleteCookie("UserName");
                SLCookieOperate.DeleteCookie("Password");
                //清空用户名和密码输入框
                txtUserName.Text = "";
                txtPwd.Password = "";
            }
        }
    }
}
