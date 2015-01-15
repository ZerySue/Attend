/*************************************************************************
** 文件名:   DlgModifyCurPwd.cs
×× 主要类:   DlgModifyCurPwd
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2013-6-14
** 修改人:   
** 日  期:
** 描  述:   DlgModifyCurPwd类,修改当前操作员密码窗口
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

namespace IriskingAttend
{
    public partial class DlgModifyCurPwd : ChildWindow
    {
        #region 构造函数

        /// <summary>
        /// 修改当前用户密码，构造函数，初始化。
        /// </summary>
        public DlgModifyCurPwd()
        {            
            InitializeComponent();

            //vm初始化
            VmModifyCurPwd vmModifyPwd = new VmModifyCurPwd(VmLogin.GetUserName());

            //vm内包含的委托的初始化
            vmModifyPwd.ChangeDlgResult = ModifyDlgResult;

            //vm 赋值
            this.DataContext = vmModifyPwd;            
        }

        #endregion

        #region 回调函数

        /// <summary>
        /// vm层委托的回调函数 
        /// </summary>
        /// <param name="dialogResult">是否需要关闭此子窗口</param>
        private void ModifyDlgResult(bool dialogResult)
        {
            this.DialogResult = dialogResult;
        }

        #endregion

        #region 事件响应

        /// <summary>
        /// 取消修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        #endregion
    }
}
