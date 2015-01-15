/*************************************************************************
** 文件名:   VmModifyCurPwd.cs
** 主要类:   VmModifyCurPwd
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2013-6-14
** 修改人:   
** 日  期:
** 描  述:   VmModifyCurPwd,修改当前操作员密码
**
** 版  本:   1.0.0
** 备  注:  命名及代码编写遵守C#编码规范
**
*************************************************************************/
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
using Microsoft.Practices.Prism.Commands;
using System.Collections.Generic;
using System.ServiceModel.DomainServices.Client;
using Irisking.Web.DataModel;
using EDatabaseError;
using IriskingAttend.Web;
using IriskingAttend.Common;
using System.IO.IsolatedStorage;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using IriskingAttend.Dialog;
using IriskingAttend.ViewModel;

namespace IriskingAttend
{
    public class VmModifyCurPwd : BaseViewModel
    {        
        #region 私有变量
        
        /// <summary>
        /// 域服务声明  
        /// </summary>      
        private DomainServiceIriskingAttend _domSrvDbAccess = new DomainServiceIriskingAttend();

        /// <summary>
        /// 旧密码的MD5格式
        /// </summary>
        private string _oldPwdMD5 = "";

        /// <summary>
        /// 当前登录的用户名
        /// </summary>
        private string _curUserName = "";

        #endregion

        #region 委托定义及变量声明

        /// <summary>
        /// 委托定义：vm层程序执行完后需要回调view层，进行界面刷新，决定是否要关闭view
        /// </summary>
        /// <param name="dialogResult"></param>
        public delegate void ChangeDialogResult(bool dialogResult);

        /// <summary>
        /// 委托变量声明
        /// </summary>
        public ChangeDialogResult ChangeDlgResult;

        #endregion     

        #region  与页面绑定的属性 

        /// <summary>
        /// 提交修改密码命令
        /// </summary>
        public DelegateCommand SubmitModifyPwdCommand 
        { 
            get; 
            set; 
        } 

        /// <summary>
        /// 旧密码
        /// </summary>
        private string _oldPassword;
        public string OldPassword
        {
            get { return _oldPassword; }
            set
            {
                _oldPassword = value;
                OnPropertyChanged<string>(() => this.OldPassword); 
            }
        }

        /// <summary>
        /// 新密码
        /// </summary>
        private string _newPassword;
        public string NewPassword
        {
            get { return _newPassword; }
            set
            {
                _newPassword = value;
                OnPropertyChanged<string>(() => this.NewPassword); 
            }
        }

        /// <summary>
        /// 重复新密码
        /// </summary>
        private string _repeatNewPassword;
        public string RepeatNewPassword
        {
            get { return _repeatNewPassword; }
            set
            {
                _repeatNewPassword = value;
                OnPropertyChanged<string>(() => this.RepeatNewPassword); 
            }
        }

        #endregion 

        #region  构造函数

        /// <summary>
        /// 构造函数，清空新旧密码及用户输入框
        /// </summary>       
        /// <param name="userName">登录的用户名</param>
        public VmModifyCurPwd(string userName)
        {
            SubmitModifyPwdCommand = new DelegateCommand(new Action(SubmitModifyPwd));
            OldPassword = "";
            NewPassword = "";
            RepeatNewPassword = "";
            this._curUserName = userName;
        }

        #endregion

        #region  控件绑定函数操作

        /// <summary>
        /// 提交修改密码按钮
        /// </summary>
        private void SubmitModifyPwd()
        {
            //旧密码不能为空
            if (OldPassword.Equals(""))
            {
                MsgBoxWindow.MsgBox( "旧密码不能为空！", MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);                
                return;
            }

            //新密码不能为空
            if (NewPassword.Equals(""))
            {
                MsgBoxWindow.MsgBox( "新密码不能为空！", MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);                
                return;
            }

            //重复新密码不能为空
            if (RepeatNewPassword.Equals(""))
            {
                MsgBoxWindow.MsgBox( "重复密码不能为空！", MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);                
                return;
            }

            //两次新密码必须输入一致
            if (!NewPassword.Equals(RepeatNewPassword))
            {                
                MsgBoxWindow.MsgBox( "两次密码输入不一致，请重新输入！", MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);

                //两次密码输入不一致，则清空两个密码输入框，进行重新输入
                NewPassword = "";
                RepeatNewPassword = "";
                return;
            }

            //获得旧密码，验证输入的旧密码是否正确，若正确，将当前用户的密码修改为新密码
            GetAndModifyPwdRia();
        }

        #endregion

        #region  通过ria连接后台，数据库相关操作

        /// <summary>
        /// 获得旧密码，验证输入的旧密码是否正确，若正确，将当前用户的密码修改为新密码
        /// </summary>
        private void GetAndModifyPwdRia()
        {
            try
            {
                Action<InvokeOperation<string>> onInvokeErrCallBack = CallBackHandleControl<string>.OnInvokeErrorCallBack;
                CallBackHandleControl<string>.m_sendValue = (o) => 
                {
                    WaitingDialog.HideWaiting();

                    //获得旧密码成功
                    if (null != o)
                    {
                        //记录旧密码的MD5格式
                        _oldPwdMD5 = o;

                        //对比旧密码的MD5与输入的旧密码MD5是否相符，若相符，则将旧密码修改为新密码
                        CompOldMD5AndModifyPwdRia(); 
                    }
                    else
                    {
                        MsgBoxWindow.MsgBox( "密码修改失败，请重试！", MsgBoxWindow.MsgIcon.Error, MsgBoxWindow.MsgBtns.OK);
                    }                    
                };
                WaitingDialog.ShowWaiting();

                //先获得当前用户密码的MD5格式
                _domSrvDbAccess.GetPassword(_curUserName, onInvokeErrCallBack, null);  
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }           
        }         
        
        /// <summary>
        /// ria方式调用后台修改密码
        /// 先对比旧密码的MD5与输入的旧密码MD5是否相符，若相符，则将旧密码修改为新密码
        /// </summary>
        private void CompOldMD5AndModifyPwdRia()
        {
            try
            {
                Action<InvokeOperation<byte>> onInvokeErrCallBack = CallBackHandleControl<byte>.OnInvokeErrorCallBack;
                CallBackHandleControl<byte>.m_sendValue = (o) =>
                {
                    WaitingDialog.HideWaiting();

                    //密码修改成功
                    if (0X01 == o)
                    {
                        MsgBoxWindow.MsgBox( "密码修改成功！", MsgBoxWindow.MsgIcon.Succeed, MsgBoxWindow.MsgBtns.OK);

                        //调用回调函数，关闭修改密码页面
                        if (null != ChangeDlgResult)
                        {
                            ChangeDlgResult(true);
                        }                        
                    }
                    //旧密码的MD5与输入的旧密码MD5不相符
                    else if (0X00 == o)
                    {                        
                        MsgBoxWindow.MsgBox( "旧密码错误，请重新输入！", MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);

                        //清空旧密码及新密码输入框，重新输入
                        OldPassword = "";
                        NewPassword = "";
                        RepeatNewPassword = "";
                    }
                    //密码修改失败
                    else
                    {
                        MsgBoxWindow.MsgBox( "密码修改失败，请重试！", MsgBoxWindow.MsgIcon.Error, MsgBoxWindow.MsgBtns.OK);

                        ////清空旧密码及新密码输入框，重新输入
                        OldPassword = "";
                        NewPassword = "";
                        RepeatNewPassword = "";
                    }                    
                };

                WaitingDialog.ShowWaiting();

                //对比旧密码的MD5与输入的旧密码MD5是否相符，若相符，则将旧密码修改为新密码
                _domSrvDbAccess.CompOldMD5AndModifyPwd(_oldPwdMD5, OldPassword, _curUserName, NewPassword, onInvokeErrCallBack, null);  
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }

        #endregion
    }

}
