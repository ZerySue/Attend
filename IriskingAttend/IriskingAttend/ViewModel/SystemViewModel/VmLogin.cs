/*************************************************************************
** 文件名:   Vmlogin.cs
** 主要类:   Vmlogin
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2013-6-14
** 修改人:   
** 日  期:
** 描  述:   Vmlogin,登录
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
using IriskingAttend.ViewModel.SystemViewModel;
using IriskingAttend.ApplicationType;
using IriskingAttend.NewWuHuShan;
using IriskingAttend.BehaviorSelf;

namespace IriskingAttend
{
    public partial class VmLogin : BaseViewModel
    {
        #region 变量 
      
        /// <summary>
        ///  域服务声明
        /// </summary>       
        private DomainServiceIriskingAttend _domSrvDbAccess = new DomainServiceIriskingAttend();

        #endregion
        
        #region  与页面绑定的属性
        
        /// <summary>
        ///  密码   
        /// </summary>     
        private string _inputPwd;
        public string InputPwd
        {
            get { return _inputPwd; }
            set
            {
                _inputPwd = value;
                OnPropertyChanged<string>(() => this.InputPwd);  
            }
        }

        /// <summary>
        /// 用户名
        /// </summary>
        private string _inputUserName;
        public string InputUserName
        {
            get { return _inputUserName; }
            set
            {
                _inputUserName = value;
                OnPropertyChanged<string>(() => this.InputUserName); 
            }
        }

        #endregion        

        #region 构造函数，初始化

        /// <summary>
        /// 构造函数，清空密码和用户名
        /// </summary>
        public VmLogin()
        {
            //全局变量，登录的用户名
            _userName = "";
            _userID = -1;

            //清空密码和用户名输入框
            InputPwd = "";
            InputUserName = "";
           
        }

        #endregion       
        
        #region  控件绑定函数操作

        /// <summary>
        /// 提交按钮，验证输入的用户名和密码是否合法
        /// </summary>
        /// <param name="inputUserName">输入的用户名</param>
        /// <param name="inputPwd">输入的密码</param>
        public void SubmitLogin(string inputUserName, string inputPwd, bool? isRemember)
        {
            //用户名不能为空
            if (inputUserName.Equals(""))
            {
                MsgBoxWindow.MsgBox( "用户名不能为空！", MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);                
                return;
            }

            //密码不能为空
            if (inputPwd.Equals(""))
            {
                MsgBoxWindow.MsgBox( "密码不能为空！", MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);                
                return;
            }

            //验证输入的用户名是否存在，且是否与输入的密码匹配
            CheckIsPwdOKRia(inputUserName, inputPwd, isRemember);
        }

        #endregion

        #region  通过ria连接后台，数据库相关操作

        /// <summary>
        /// ria方式调用后台获得用户密码
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        private void CheckIsPwdOKRia(string userName, string password, bool? isRemember)
        {
            try
            {
                Action<InvokeOperation<Int32>> onInvokeErrCallBack = CallBackHandleControl<Int32>.OnInvokeErrorCallBack;
                CallBackHandleControl<Int32>.m_sendValue = (o) => 
                {
                    //返回值为-2，则数据库中不存在此用户名
                    if (-2 == o)                    
                    {
                        WaitingDialog.HideWaiting();
                        MsgBoxWindow.MsgBox( "无此用户，请重试！", MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);
                    }
                    //返回值为-1，证明数据库中存在此用户名，但密码错误
                    else if (-1 == o)
                    {
                        WaitingDialog.HideWaiting();
                        MsgBoxWindow.MsgBox("密码输入错误，请重试！", MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);                                        
                    }
                    //其它返回值，证明数据库中存在此用户名，密码正确
                    else
                    {
                        //记录登录的用户名
                        VmLogin._userName = userName;
                        VmLogin._userID = o;

                        //登录成功后，获得此操作员的权限
                        _vmPrivilege = null;
                        _vmPrivilege = new VmPrivilege(VmLogin.GetUserName(), () =>
                        {
                            WaitingDialog.HideWaiting();
                            ReLoadPrivilege();

                            if (AppTypePublic.GetCustomAppType().GetType().ToString().CompareTo("IriskingAttend.ApplicationType.WuHuShanApp") == 0)
                            {
                                VmAbnormalAttendInfo vmAbnormal = new VmAbnormalAttendInfo();
                                vmAbnormal.SyncLocateAttendInfoRia(new int[0], DateTime.Now.AddDays(-3).Date, DateTime.Now, () =>                                
                                {
                                    //登录成功且获得应用程序类型后，进入主界面
                                    App currentApp = (App)Application.Current;
                                    Uri url = new Uri("/MainPage", UriKind.Relative);
                                    currentApp.Navigation(url);
                                });
                            }
                            else
                            {
                                //登录成功且获得应用程序类型后，进入主界面
                                App currentApp = (App)Application.Current;
                                Uri url = new Uri("/MainPage", UriKind.Relative);
                                currentApp.Navigation(url);
                            }
                        });                            
                    }
                    //记住用户名密码 add by yuhaitao
                    if (isRemember.Value)
                    {
                        SLCookieOperate.SetCookie("UserName", userName);
                        SLCookieOperate.SetCookie("Password", password);
                    }
                    
                    userName = "";
                    password = "";
                };
                WaitingDialog.ShowWaiting();

                //调用后台，获得用户名与密码是否合法
                _domSrvDbAccess.IsPasswordOk(userName, password, onInvokeErrCallBack, null);  
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }

        /// <summary>
        /// 异步获取人员信息
        /// </summary>
        public void GetPersonInfoByDepartPotence(Action<IEnumerable<UserPersonInfo>> completeCallBack)
        {
            try
            {
                ServiceDomDbAcess.ReOpenSever();

                string[] departName = new string[VmLogin.OperatorDepartInfoList.Count];
                int index=0;
                foreach (UserDepartInfo item in VmLogin.OperatorDepartInfoList)
                {
                    departName[index++] = item.depart_name;
                }

                List<UserPersonInfo> PersonInfoModel = new List<UserPersonInfo>();

                EntityQuery<UserPersonInfo> lstPerson = ServiceDomDbAcess.GetSever().GetPersonInfoByDepartNameQuery(departName);
                ///回调异常类
                Action<LoadOperation<UserPersonInfo>> getPersonCallBack = new Action<LoadOperation<UserPersonInfo>>(ErrorHandle<UserPersonInfo>.OnLoadErrorCallBack);
                ///异步事件
                LoadOperation<UserPersonInfo> lo = ServiceDomDbAcess.GetSever().Load(lstPerson, getPersonCallBack, null);

                lo.Completed += (o, e) =>
                {
                    PersonInfoModel.Clear();

                    foreach (var ar in lo.Entities)
                    {
                        PersonInfoModel.Add(ar);
                    }

                    if (completeCallBack != null)
                    {
                        completeCallBack(PersonInfoModel);
                    }
                };
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
