/*************************************************************************
** 文件名:   DlgLogDetail.cs
×× 主要类:   DlgLogDetail
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2013-11-5
** 修改人:   
** 日  期:
** 描  述:   DlgLogDetail类,操作员日志详细信息
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
using IriskingAttend.ViewModel.SystemViewModel;
using Irisking.Web.DataModel;


namespace IriskingAttend.Dialog
{
    public partial class DlgLogDetail : ChildWindow
    {
        #region 构造函数

        /// <summary>
        /// 构造函数，显示操作员日志详细信息窗口
        /// </summary>      
        public DlgLogDetail()
        {            
            InitializeComponent();           
        }
        #endregion        

        #region 事件响应

        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;            
        }
        
        #endregion        
    }
}
