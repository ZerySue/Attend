/*************************************************************************
** 文件名:   VmOperatorItemMng.cs
** 主要类:   VmOperatorItemMng
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2013-6-14
** 修改人:   
** 日  期:
** 描  述:   VmOperatorItemMng,操作员添加、修改
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
using IriskingAttend.View.SystemView;
using IriskingAttend.Dialog;

namespace IriskingAttend.ViewModel.SystemViewModel
{
    public class VmOperatorItemMng : BaseViewModel
    {
        #region 变量
        
        /// <summary>
        /// 域服务声明
        /// </summary>        
        private DomainServiceIriskingAttend _domSrvDbAccess = new DomainServiceIriskingAttend();

        /// <summary>
        /// 旧的用户名 
        /// </summary>       
        public string OldUserName;
        
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

        #region 与页面绑定的属性

        /// <summary>
        /// 增加一名新操作员命令
        /// </summary>
        public DelegateCommand AddOperatorCommand 
        { 
            get; 
            set; 
        }        

        /// <summary>
        /// 修改操作员信息命令
        /// </summary>
        public DelegateCommand ModifyOperatorInfoCommand 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// 修改操作员密码命令
        /// </summary>
        public DelegateCommand ModifyOperatorPasswordCommand 
        { 
            get; 
            set; 
        }   
      
        /// <summary>
        /// 旧的用户名
        /// </summary>
        private operator_info _operatInfo;
        public operator_info OperatInfo
        {
            get { return _operatInfo; }
            set
            {
                _operatInfo = value;
                OnPropertyChanged<operator_info>(() => this.OperatInfo); 
            }
        }

        /// <summary>
        /// 输入密码
        /// </summary>
        private string _inputPassword;
        public string InputPassword
        {
            get { return _inputPassword; }
            set
            {
                _inputPassword = value;
                OnPropertyChanged<string>(() => this.InputPassword); 
            }
        }

        /// <summary>
        /// 输入的重复密码
        /// </summary>
        private string _inputPasswordRepeat;
        public string InputPasswordRepeat
        {
            get { return _inputPasswordRepeat; }
            set
            {
                _inputPasswordRepeat = value;
                OnPropertyChanged<string>(() => this.InputPasswordRepeat); 
            }
        }       
        #endregion   
     
        #region 构造函数，初始化

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_ChildWindow"></param>
        public VmOperatorItemMng()        
        {
            //命令绑定属性初始化
            AddOperatorCommand = new DelegateCommand(new Action(AddOperator));            
            ModifyOperatorInfoCommand = new DelegateCommand(new Action(ModifyOperatorInfo));
            ModifyOperatorPasswordCommand = new DelegateCommand(new Action(ModifyOperatorPassword));

            //变量属性初始化
            OperatInfo = new operator_info();

            //与子窗口绑定的变量值初始化
            InputPassword = "";
            InputPasswordRepeat = "";
            OperatInfo.logname = "";            
            OperatInfo.realityname = "";
        }

        #endregion

        #region  控件绑定函数操作

        /// <summary>
        /// 添加操作员
        /// </summary>
        private void AddOperator()
        {
            //用户名与密码都不能为空
            if (OperatInfo.logname.Equals("") || InputPassword.Equals("") || InputPasswordRepeat.Equals(""))
            {
                MsgBoxWindow.MsgBox( "用户名与密码都不能为空！", MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);                
                return;              
            }

            //两次密码输入必须一致
            if (!InputPassword.Equals(InputPasswordRepeat) )
            {
                MsgBoxWindow.MsgBox( "两次密码输入不一致，请重新输入！", MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);                
                return;                 
            }

            //校验输入的操作员中是否包含非法字符
            if (!PublicMethods.IsInputValid(OperatInfo.logname) || !PublicMethods.IsInputValid(InputPassword) || !PublicMethods.IsInputValid(OperatInfo.realityname)) 
            {
                MsgBoxWindow.MsgBox( "您输入的操作员信息中包含非法字符，请重新输入！", MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);               
                return;
            }

            //添加操作员时，先检查数据库中有没有相同名称的操作员，然后再添加。操作员名称不能重复
            CheckAndAddOperatorRia();
        }
        /// <summary>
        /// 修改操作员信息
        /// </summary>
        private void ModifyOperatorInfo()
        {
            //用户名不能为空
            if (OperatInfo.logname.Equals(""))
            {
                MsgBoxWindow.MsgBox( "用户名不能为空！", MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);               
                return;
            }

            //校验输入的操作员中是否包含非法字符
            if (!PublicMethods.IsInputValid(OperatInfo.logname) || !PublicMethods.IsInputValid(OperatInfo.realityname)) 
            {
                MsgBoxWindow.MsgBox( "您输入的操作员信息中包含非法字符，请重新输入！", MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);                
                return;
            }

            //修改操作员之前，先在数据库里检查是否有修改后新操作员的名称，操作员名称不可重复
            CheckAndModifyOperatorRia();
        }

        /// <summary>
        /// 修改操作员密码
        /// </summary>
        private void ModifyOperatorPassword()
        {
            //密码输入不能为空
            if (InputPassword.Equals("") || InputPasswordRepeat.Equals(""))
            {
                MsgBoxWindow.MsgBox( "密码输入不能为空！", MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);                
                return;
            }

            //两次密码输入必须一致
            if (!InputPassword.Equals(InputPasswordRepeat))
            {
                MsgBoxWindow.MsgBox( "两次密码输入不一致！请重新输入！", MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);                
                return;
            }
            
            //校验输入的操作员中是否包含非法字符
            if (!PublicMethods.IsInputValid(InputPassword)) 
            {
                MsgBoxWindow.MsgBox( "您输入的操作员密码中包含非法字符，请重新输入！", MsgBoxWindow.MsgIcon.Warning, MsgBoxWindow.MsgBtns.OK);                
                return;
            }

            //ria方式调用后台修改操作员密码
            ModifyOperatorPasswordRia();
        }

        #endregion

        #region  通过ria连接后台，数据库相关操作

        /// <summary>
        /// 在添加操作员之前，先在数据库里检查是否有此新操作员的名称，操作员名称不可重复
        /// </summary>
        private void CheckAndAddOperatorRia()
        {
            try
            {
                Action<InvokeOperation<bool>> onInvokeErrCallBack = CallBackHandleControl<bool>.OnInvokeErrorCallBack;
                CallBackHandleControl<bool>.m_sendValue = (o) =>
                {
                    WaitingDialog.HideWaiting();                    

                    //异步获取数据,返回为tue时，此操作员名称在数据库中已存在，需要重新输入                   
                    if (o)
                    {
                        MsgBoxWindow.MsgBox( "此用户名已存在，请重新输入！", MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);                                              
                    }
                    else
                    {
                        //此操作员名称在数据库中不存在，可直接加入数据库列表
                        AddOperatorRia();                        
                    }                    
                };

                WaitingDialog.ShowWaiting();

                //检查欲添加的操作员名称是否存在
                _domSrvDbAccess.IsOperatorExist(OperatInfo.logname, onInvokeErrCallBack, null);
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }

        /// <summary>
        /// 修改操作员之前，先在数据库里检查是否有修改后新操作员的名称，操作员名称不可重复
        /// </summary>
        private void CheckAndModifyOperatorRia()
        {
            ///修改操作员信息时，操作员名称未改变，可直接调用后台修改操作员信息，
            ///否则需先检查数据库中是否存在修改后的新操作员名称           
            if (OldUserName == OperatInfo.logname)
            {
                ModifyOperatorInfoRia();
                return;
            }

            try
            {
                Action<InvokeOperation<bool>> onInvokeErrCallBack = CallBackHandleControl<bool>.OnInvokeErrorCallBack;
                CallBackHandleControl<bool>.m_sendValue = (o) =>
                {
                    WaitingDialog.HideWaiting();                   

                    //异步获取数据,返回为tue时，修改后的操作员名称在数据库中已存在，需要重新输入                       
                    if (o)
                    {
                        MsgBoxWindow.MsgBox("此用户名已存在，请重新输入！", MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);
                    }
                    else
                    {
                        //此操作员名称在数据库中不存在，可调用后台修改此操作员信息
                        ModifyOperatorInfoRia();
                    }                    
                };
                WaitingDialog.ShowWaiting();

                //检查修改后的新操作员名称是否存在
                _domSrvDbAccess.IsOperatorExist(OperatInfo.logname, onInvokeErrCallBack, null);
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }

        /// <summary>
        /// ria方式调用后台添加操作员
        /// </summary>
        private void AddOperatorRia()
        {
            try
            {
                //组织欲添加的操作员信息
                operator_info tempOperator = new operator_info();
                tempOperator.logname = OperatInfo.logname;
                tempOperator.realityname = OperatInfo.realityname;
                tempOperator.password = InputPassword;
                //1 为系统管理员， 2为受限管理员。 本系统中只有admin一个系统管理员。兼容矿山考勤。
                tempOperator.operator_type = 2;  

                Action<InvokeOperation<bool>> onInvokeErrCallBack = CallBackHandleControl<bool>.OnInvokeErrorCallBack;
                CallBackHandleControl<bool>.m_sendValue = (o) =>
                {
                    WaitingDialog.HideWaiting();                  

                    //异步获取数据，返回为true，添加操作员成功              
                    if (o)
                    {
                        MsgBoxWindow.MsgBox( "添加操作员成功！", MsgBoxWindow.MsgIcon.Succeed, MsgBoxWindow.MsgBtns.OK);

                        //调用回调函数，关闭添加操作员页面
                        if (null != ChangeDlgResult)
                        {                           
                            ChangeDlgResult(true);
                        }                                           
                    }
                    else
                    {
                        //添加操作员失败
                        MsgBoxWindow.MsgBox( "添加操作员失败！", MsgBoxWindow.MsgIcon.Error, MsgBoxWindow.MsgBtns.OK);                        
                    }
                    
                };

                WaitingDialog.ShowWaiting();

                //调用后台添加操作员
                _domSrvDbAccess.AddOperator(tempOperator, onInvokeErrCallBack, null);
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }

        /// <summary>
        /// ria方式调用后台修改操作员信息
        /// </summary>
        private void ModifyOperatorInfoRia()
        {
            try
            {
                //组织修改后的操作员信息
                operator_info tempOperator = new operator_info();
                tempOperator.logname = OperatInfo.logname;
                tempOperator.realityname = OperatInfo.realityname;
                tempOperator.operator_type = 2;

                Action<InvokeOperation<bool>> onInvokeErrCallBack = CallBackHandleControl<bool>.OnInvokeErrorCallBack;
                CallBackHandleControl<bool>.m_sendValue = (o) =>
                {
                    WaitingDialog.HideWaiting();                    

                    //异步获取数据，返回为true，修改操作员信息成功                    
                    if (o)
                    {
                        MsgBoxWindow.MsgBox( "修改操作员信息成功！", MsgBoxWindow.MsgIcon.Succeed, MsgBoxWindow.MsgBtns.OK);

                        //调用回调函数，关闭修改操作员信息页面
                        if (null != ChangeDlgResult)
                        {
                            ChangeDlgResult(true);
                        }                                       
                    }
                    else
                    {
                        MsgBoxWindow.MsgBox( "修改操作员信息失败！", MsgBoxWindow.MsgIcon.Error, MsgBoxWindow.MsgBtns.OK);
                    }
                   
                };

                WaitingDialog.ShowWaiting();

                //调用后台修改操作员信息
                _domSrvDbAccess.ModifyOperatorInfo(OldUserName, tempOperator, onInvokeErrCallBack, null);
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }

        /// <summary>
        /// ria方式调用后台修改操作员密码
        /// </summary>
        private void ModifyOperatorPasswordRia()
        {
            try
            {
                Action<InvokeOperation<bool>> onInvokeErrCallBack = CallBackHandleControl<bool>.OnInvokeErrorCallBack;
                CallBackHandleControl<bool>.m_sendValue = (o) =>
                {
                    WaitingDialog.HideWaiting();                   

                    //异步获取数据，返回为true，修改操作员密码成功                     
                    if (o)
                    {
                        MsgBoxWindow.MsgBox( "修改操作员密码成功！", MsgBoxWindow.MsgIcon.Succeed, MsgBoxWindow.MsgBtns.OK);

                        //调用回调函数，关闭修改操作员密码页面
                        if (null != ChangeDlgResult)
                        {
                            ChangeDlgResult(true);
                        }                        
                    }
                    else
                    {
                        MsgBoxWindow.MsgBox( "修改操作员密码失败！", MsgBoxWindow.MsgIcon.Error, MsgBoxWindow.MsgBtns.OK);
                    }                    
                };

                WaitingDialog.ShowWaiting();

                //调用后台修改操作员密码
                _domSrvDbAccess.ModifyOperatorPassword(OperatInfo.logname, InputPassword, onInvokeErrCallBack, null);
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
