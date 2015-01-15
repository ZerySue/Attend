/*************************************************************************
** 文件名:   DlgModifyOperatorInfo.cs
** 主要类:   DlgModifyOperatorInfo
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2013-6-14
** 修改人:   
** 日  期:
** 描  述:   DlgModifyOperatorInfo类,修改操作员信息窗口
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
using Irisking.Web.DataModel;


namespace IriskingAttend.Dialog
{
    public partial class DlgModifyOperatorInfo : ChildWindow
    {
        #region 私有变量声明

        //当前选中行信息
        private operator_info _selectOperatorInfoItem;

        //回调函数，父窗口传递
        private Action<bool?> _parentCallBack;

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数，修改操作员信息子窗口
        /// </summary>
        /// <param name="selectOperatorInfoItem">选中的被修改操作员的信息</param>
        /// <param name="parentCallBack">子窗口关闭后调用的父窗口的回调函数</param>
        public DlgModifyOperatorInfo(operator_info selectOperatorInfoItem, Action<bool?> parentCallBack)
        {            
            InitializeComponent();

            //vm初始化
            VmOperatorItemMng vmModifyOperatorInfo = new VmOperatorItemMng();

            //vm数据绑定
            this.DataContext = vmModifyOperatorInfo;

            //vm内包含的委托的初始化
            vmModifyOperatorInfo.ChangeDlgResult = ModifyDlgResult;

            //子窗口关闭时的事件
            this.Closed += new EventHandler(DlgModifyOperatorInfo_Closed);

            //父窗口的回调函数
            this._parentCallBack = parentCallBack;
            this._selectOperatorInfoItem = selectOperatorInfoItem;

            //将父窗口中需要修改的操作员信息显示在此子窗口中,子窗口界面控件值初始化 
            vmModifyOperatorInfo.OldUserName = selectOperatorInfoItem.logname;
            vmModifyOperatorInfo.OperatInfo.logname = selectOperatorInfoItem.logname;
            vmModifyOperatorInfo.OperatInfo.realityname = selectOperatorInfoItem.realityname;           
 
            if(VmLogin.IsSuperUser(selectOperatorInfoItem.logname))
            {
                this.txtOperatorName.IsReadOnly = true;               
            }
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
        ///  取消修改
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
        private void DlgModifyOperatorInfo_Closed(object sender, EventArgs e)
        {
            //子窗口关闭时，调用父窗口传递进来的回调函数
            if (_parentCallBack != null)
            {
                _parentCallBack(this.DialogResult);
            }
        }

        #endregion
    }
}
