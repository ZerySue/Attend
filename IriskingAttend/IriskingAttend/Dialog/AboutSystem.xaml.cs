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
using System.Windows.Browser;

namespace IriskingAttend.Dialog
{
    public partial class AboutSystem : ChildWindow
    {
        public AboutSystem()
        {
            InitializeComponent();
            lblVersion.Content = Version.GetProjectVersion(); 
        }

        /// <summary>
        /// 进入公司网站
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hbtnCompanyWebsites_Click(object sender, RoutedEventArgs e)
        {
            HtmlWindow html = HtmlPage.Window;
            html.Navigate(new Uri("http://www.irisking.com/", UriKind.Absolute), "_blank");
        }

    }
}

