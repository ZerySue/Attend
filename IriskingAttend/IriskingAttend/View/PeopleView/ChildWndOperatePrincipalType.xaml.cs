/*************************************************************************
** 文件名:   ChildWnd_OperateClassType.cs
×× 主要类:   ChildWnd_OperateClassType
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   cty
** 日  期:   2013-6-14
** 修改人:   
** 日  期:
** 描  述:   ChildWnd_OperateClassType类,增删改班制页面
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
using IriskingAttend.ViewModel.PeopleViewModel;
using Irisking.Web.DataModel;
using IriskingAttend.BehaviorSelf;
using IriskingAttend.Web;
using EDatabaseError;
using IriskingAttend.Common;
using IriskingAttend.Dialog;
using IriskingAttend.ViewModel.SystemViewModel;
using System.Text;
using System.ServiceModel.DomainServices.Client;

namespace IriskingAttend.ViewMine.PeopleView
{
    /// <summary>
    /// 操作班制界面UI后台
    /// </summary>
    public partial class ChildWndOperatePrincipalType : ChildWindow
    {

        #region 字段声明

        private Action<bool> _callBack;                        //子窗口回调函数
        private ChildWndOptionMode _mode;                      //操作模式 添加或者修改
        private PrincipalTypeInfo _curPrincipalType = null;        //当前的职务信息
        private bool _isContinueAdd = false;                   //是否执行了继续添加操作
        
        /// <summary>
        /// 与服务声明
        /// </summary>
        private DomainServiceIriskingAttend _serviceDomDbAccess = new DomainServiceIriskingAttend();

        #endregion

        #region 构造函数

        public ChildWndOperatePrincipalType(PrincipalTypeInfo pInfo, ChildWndOptionMode mode, Action<bool> callBack)
        {
            InitializeComponent();
            _callBack = callBack;
            _mode = mode;
            _curPrincipalType = pInfo;
            SetContent();
           
        }

        #endregion

        #region 私有功能函数
       
        /// <summary>
        /// 产生界面内容
        /// </summary>
        private void SetContent()
        {
            if (_mode == ChildWndOptionMode.Add)
            {
                _curPrincipalType = new PrincipalTypeInfo();
                _curPrincipalType.principal_type_name = "";
                _curPrincipalType.memo = "";
                OkBtnContent.Text = "添加";
                ContinueButton.Visibility = Visibility.Visible;
                this.Title = "添加职务类型";
            }
            else
            {
                this.Title ="修改职务类型";
                OkBtnContent.Text = "修改";
                ContinueButton.Visibility = Visibility.Collapsed;
            }


            this.txtName.Text = _curPrincipalType.principal_type_name;
            this.txtMemo.Text = _curPrincipalType.memo;
            this.txtPrincipalTypeOrder.Text = _curPrincipalType.principal_type_order.ToString();
        }

        /// <summary>
        /// 由界面内容产生实体对象
        /// </summary>
        /// <returns></returns>
        private PrincipalTypeInfo GetContent()
        {
            PrincipalTypeInfo newInfo = new PrincipalTypeInfo();
            newInfo.principal_type_name = this.txtName.Text;
            newInfo.memo = this.txtMemo.Text;
            newInfo.principal_type_id = _curPrincipalType.principal_type_id;
            try
            {
                newInfo.principal_type_order = int.Parse(this.txtPrincipalTypeOrder.Text.Trim());
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("职务类型顺序应为整数类型，请重设！{0}", e.Message));
                return null;
            }
            

            return newInfo;
        }
      

        #endregion

        #region 事件响应函数

        //窗口关闭事件
        private void ChildWindow_Closed(object sender, EventArgs e)
        {
            if (_callBack != null)
            {
                _callBack(this.DialogResult.Value || _isContinueAdd);
            }
        }

        //继续添加
        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.txtName.Text == null || this.txtName.Text == "")
            {
                MsgBoxWindow.MsgBox(
                    "职务类型名称不能为空！",
                    Dialog.MsgBoxWindow.MsgIcon.Information,
                    Dialog.MsgBoxWindow.MsgBtns.OK);
                return;
            }


            WaitingDialog.ShowWaiting();
            var info = GetContent();
            if (info == null) return;

            AddRia(info, (op) =>
                {
                    SetContent();
                    _isContinueAdd = true;
                    WaitingDialog.HideWaiting();
                });
        }

        //添加或者 修改
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.txtName.Text == null || this.txtName.Text == "")
            {
                MsgBoxWindow.MsgBox(
                    "职务类型名称不能为空！",
                    Dialog.MsgBoxWindow.MsgIcon.Information,
                    Dialog.MsgBoxWindow.MsgBtns.OK);
                return;
            }
            var info = GetContent();
            if (info == null) return;

            if (_mode == ChildWndOptionMode.Add)
            {
                WaitingDialog.ShowWaiting();

                AddRia(info, (op) =>
                    {
                        WaitingDialog.HideWaiting();
                        this.DialogResult = true;
                    });
            }
            else if (_mode == ChildWndOptionMode.Modify)
            {
                WaitingDialog.ShowWaiting();
                ModifyRia(info, (op) =>
                    {
                        WaitingDialog.HideWaiting();
                        this.DialogResult = true;
                    });
            }
        }

        //取消
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        #endregion

        #region ria 操作

        /// <summary>
        /// 添加职务类型
        /// </summary>
        /// <param name="info"></param>
        /// <param name="riaOperateCallBack"></param>
        private void AddRia(PrincipalTypeInfo info, Action<OptionInfo> riaOperateCallBack)
        {
            //获取操作描述
            String description = "职务类型名称：" + info.principal_type_name + "；\r\n";
           
            try
            {
                _serviceDomDbAccess.AddPrincipalType(info, CallBackHandleControl<OptionInfo>.OnInvokeErrorCallBack, null);
                CallBackHandleControl<OptionInfo>.m_sendValue = (optionInfo) =>
                {
                    VmOperatorLog.CompleteCallBack completeCallBack = () =>
                        {
                            if (riaOperateCallBack != null)
                            {
                                riaOperateCallBack(optionInfo);
                            }
                        };
                    if (optionInfo.isNotifySuccess && optionInfo.isSuccess)
                    {
                        MsgBoxWindow.MsgBox(
                                 optionInfo.option_info,
                                 Dialog.MsgBoxWindow.MsgIcon.Succeed,
                                 Dialog.MsgBoxWindow.MsgBtns.OK);
                        VmOperatorLog.InsertOperatorLog(1, "添加职务类型", description, completeCallBack);
                    }
                    else if (!optionInfo.isNotifySuccess)
                    {
                        MsgBoxWindow.MsgBox(
                                optionInfo.option_info,
                                Dialog.MsgBoxWindow.MsgIcon.Warning,
                                Dialog.MsgBoxWindow.MsgBtns.OK);
                        VmOperatorLog.InsertOperatorLog(1, "添加职务类型", description + optionInfo.option_info, completeCallBack);
                    }
                    else
                    {
                        MsgBoxWindow.MsgBox(
                             optionInfo.option_info,
                             Dialog.MsgBoxWindow.MsgIcon.Error,
                             Dialog.MsgBoxWindow.MsgBtns.OK);
                        VmOperatorLog.InsertOperatorLog(0, "添加职务", description + optionInfo.option_info, completeCallBack);
                    }

                   
                };
            }
            catch (Exception e)
            {
                WaitingDialog.HideWaiting();
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
           
        }

        /// <summary>
        /// 修改职务
        /// </summary>
        /// <param name="info"></param>
        /// <param name="riaOperateCallBack"></param>
        private void ModifyRia(PrincipalTypeInfo info, Action<OptionInfo> riaOperateCallBack)
        {
            StringBuilder description = new StringBuilder(string.Format("职务类型名称：{0}；\r\n", _curPrincipalType.principal_type_name));
            bool isModify = false;
            if (info.principal_type_name != _curPrincipalType.principal_type_name)
            {
                isModify = true;
                description.Append(string.Format("职务类型名称:{0}->{1}，", _curPrincipalType.principal_type_name, info.principal_type_name));
            }
            if (info.memo != _curPrincipalType.memo)
            {
                isModify = true;
                description.Append(string.Format("备注:{0}->{1}，", _curPrincipalType.memo, info.memo));
            }
            if (info.principal_type_order != _curPrincipalType.principal_type_order)
            {
                isModify = true;
                description.Append(string.Format("顺序:{0}->{1}，", _curPrincipalType.principal_type_order, info.principal_type_order));
            }
            if (isModify)
            {
                description.Remove(description.Length - 1, 1);
                description.Append("；\r\n");
            }
            else
            {
                this.DialogResult = true;
                return;
            }

            try
            {
                _serviceDomDbAccess.ModifyPrincipalType(info, CallBackHandleControl<OptionInfo>.OnInvokeErrorCallBack, null);
                CallBackHandleControl<OptionInfo>.m_sendValue = (optionInfo) =>
                {
                    VmOperatorLog.CompleteCallBack completeCallBack = () =>
                        {
                            if (riaOperateCallBack != null)
                            {
                                riaOperateCallBack(optionInfo);
                            }
                        };

                    if (optionInfo.isNotifySuccess && optionInfo.isSuccess)
                    {
                        MsgBoxWindow.MsgBox(
                                 optionInfo.option_info,
                                 Dialog.MsgBoxWindow.MsgIcon.Succeed,
                                 Dialog.MsgBoxWindow.MsgBtns.OK);
                        VmOperatorLog.InsertOperatorLog(1, "修改职务类型", description.ToString(), completeCallBack);
                    }
                    else if (!optionInfo.isNotifySuccess)
                    {
                        MsgBoxWindow.MsgBox(
                                optionInfo.option_info,
                                Dialog.MsgBoxWindow.MsgIcon.Warning,
                                Dialog.MsgBoxWindow.MsgBtns.OK);
                        VmOperatorLog.InsertOperatorLog(1, "修改职务类型", description.ToString() + optionInfo.option_info, completeCallBack);
                    }
                    else
                    {
                        MsgBoxWindow.MsgBox(
                             optionInfo.option_info,
                             Dialog.MsgBoxWindow.MsgIcon.Error,
                             Dialog.MsgBoxWindow.MsgBtns.OK);
                        VmOperatorLog.InsertOperatorLog(0, "修改职务类型", description.ToString() + optionInfo.option_info, completeCallBack);
                    }

                };
            }
            catch (Exception e)
            {
                WaitingDialog.HideWaiting();
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }

        }

    

        #endregion

    }
}

