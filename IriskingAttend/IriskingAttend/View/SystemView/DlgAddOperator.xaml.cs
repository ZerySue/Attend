/*************************************************************************
** 文件名:   DlgAddOperator.cs
×× 主要类:   DlgAddOperator
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2013-6-14
** 修改人:   
** 日  期:
** 描  述:   DlgAddOperator类,添加操作员窗口
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
    public partial class DlgAddOperator : ChildWindow
    {
        #region 私有变量声明：父窗口的回调函数

        //回调函数，父窗口传递
        private Action<bool?> _parentCallBack;

        //vm声明
        private VmOperatorItemMng _vmAddOperator;

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数，增加操作员子窗口
        /// </summary>
        /// <param name="parentCallBack">子窗口关闭后调用的父窗口的回调函数</param>
        public DlgAddOperator(Action<bool?> parentCallBack)
        {            
            InitializeComponent();

            _vmAddOperator = new VmOperatorItemMng();

            //vm数据绑定
            this.DataContext = _vmAddOperator;

            //vm内包含的委托的初始化
            _vmAddOperator.ChangeDlgResult = ModifyDlgResult;

            //子窗口关闭时的事件
            this.Closed += new EventHandler(DlgAddOperator_Closed);

            //父窗口的回调函数
            this._parentCallBack = parentCallBack;
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
        /// 取消添加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;            
        }

        /// <summary>
        /// 子窗口关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DlgAddOperator_Closed(object sender, EventArgs e)
        {
            if (this.DialogResult.HasValue && this.DialogResult.Value)
            {
                DlgPrivilege dlgPrivilege = new DlgPrivilege(_vmAddOperator.OperatInfo.logname, false );

                dlgPrivilege.Show((Result) =>
                {
                    //子窗口关闭时，调用父窗口传递进来的回调函数
                    if (_parentCallBack != null)
                    {
                        _parentCallBack(this.DialogResult);
                    }
                });
            }
            else
            {
                //子窗口关闭时，调用父窗口传递进来的回调函数
                if (_parentCallBack != null)
                {
                    _parentCallBack(this.DialogResult);
                }
            }            
        }
        #endregion
        
    }
}
