/*************************************************************************
** 文件名:   IkReport.cs
×× 主要类:   IkReport
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2014-4-15
** 修改人:   
** 日  期:
** 描  述:   IkReport类,空界面
**
** 版  本:   1.0.0
** 备  注:  命名及代码编写遵守C#编码规范
**
 * ***********************************************************************/
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
using System.Windows.Navigation;

namespace IriskingAttend
{
    public partial class IkReport : Page
    {
        #region 构造函数

        public IkReport()
        {
            InitializeComponent();

            this.LayoutRoot.Children.Add( new IkReportWCFRia.PageIkReportClient() );
        }

        #endregion
    }
}
