/*************************************************************************
** 文件名:   MainPage.cs
×× 主要类:   MainPage
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2013-6-14
** 修改人:   
** 日  期:
** 描  述:   MainPage类,主界面
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
using System.IO.IsolatedStorage;
using IriskingAttend.ViewModel.SystemViewModel;

namespace IriskingAttend
{
    public partial class MainPage : Page
    {
        #region 变量

        //权限vm
        private VmPrivilege vmPrivilege;

        #endregion

        #region 静态变量

        /// <summary>
        /// 当前操作员部门权力列表集合
        /// </summary>
        static public List<int> OperatorDepartIDList = new List<int>();

        /// <summary>
        /// 当前操作员权限列表集合
        /// </summary>
        static public List<int> OperatorPurviewIDList = new List<int>();

        #endregion        
    }
}
