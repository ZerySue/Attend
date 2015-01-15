/*************************************************************************
** 文件名:   VmDBBackup.cs
** 主要类:   VmDBBackup
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2013-6-14
** 修改人:   cty
** 日  期:   2013-8-1
*修改内容：  增加手动备份虹膜库功能，修改备份考勤库的方法，修改命名规则
** 描  述:   VmDBBackup,手动备份
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
using System.Text.RegularExpressions;
using System.Threading;

namespace IriskingAttend.ViewModel.SystemViewModel
{
    /// <summary>
    /// 数据库备份vm
    /// </summary>
    public class VmDBBackup : BaseViewModel
    {
        #region 私有变量

        /// <summary>
        /// 域服务声明
        /// </summary>
        private DomainServiceIriskingAttend _domSrvDbAccess = new DomainServiceIriskingAttend();

        #endregion

        #region 与页面绑定的属性

        private string _backupPathIrisApp = "";
        /// <summary>
        /// 考勤库备份目标地址
        /// </summary>
        public string BackupPathIrisApp
        {
            get { return _backupPathIrisApp; }
            set
            {
                _backupPathIrisApp = value;
                OnPropertyChanged<string>(() => this.BackupPathIrisApp);
            }
        }

        private string _backupPathIrisData = "";
        /// <summary>
        /// 虹膜库备份目标地址
        /// </summary>
        public string BackupPathIrisData
        {
            get { return _backupPathIrisData; }
            set
            {
                _backupPathIrisData = value;
                OnPropertyChanged<string>(() => this.BackupPathIrisData);
            }
        }

        /// <summary>
        /// 提交考勤库手动备份
        /// </summary>
        public DelegateCommand ManualBackupIrisAppCommand 
        {
            get; 
            set; 
        }

        /// <summary>
        /// 提交虹膜库手动备份
        /// </summary>
        public DelegateCommand ManualBackupIrisDataCommand
        {
            get;
            set;
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数，进行初始化
        /// </summary>
        public VmDBBackup()
        {
            BackupPathIrisApp = string.Format(@"C:\irisNewAttend-" + DateTime.Now.ToString("yyyyMMdd") + ".bak");
            BackupPathIrisData = string.Format(@"C:\irisData-" + DateTime.Now.ToString("yyyyMMdd") + ".bak");
            //初始化按钮绑定函数
            ManualBackupIrisAppCommand = new DelegateCommand(new Action(ManualBackupIrisApp));
            ManualBackupIrisDataCommand = new DelegateCommand(new Action(ManualBackupIrisData));
        }

        #endregion

        #region 控件绑定函数操作

        /// <summary>
        /// 与手动备份考勤库按钮相关联
        /// </summary>
        public void ManualBackupIrisApp()
        {
            //备份目标地址不能为空
            if (BackupPathIrisApp.Equals(""))
            {
                MsgBoxWindow.MsgBox( "备份输出地址不能为空！", MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);                
                return;
            }
            //socket发送请求给备份服务端，进行备份  by cty
            BackupParameter backupParameter = new BackupParameter();

            backupParameter.ManOrAuto = 0;
            backupParameter.DatabaseName = 0;
            backupParameter.PathIrisApp = BackupPathIrisApp;

            BackUpDatabase backUpDatabase = new BackUpDatabase();
            WaitingDialog.ShowWaiting("正在备份数据库，请耐心等待......");
            backUpDatabase.JudgeSuccess += new EventHandler(backUpDatabase_JudgeSuccess);
            backUpDatabase.BackUpOperation(backupParameter);           
        }

        /// <summary>
        /// cty
        /// 与手动备份虹膜库按钮相关联
        /// </summary>
        public void ManualBackupIrisData()
        {
            //备份目标地址不能为空
            if (BackupPathIrisData.Equals(""))
            {
                MsgBoxWindow.MsgBox("备份输出地址不能为空！", MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);
                return;
            }

            //socket发送请求给备份服务端，进行备份 by cty
            BackupParameter backupParameter = new BackupParameter();

            backupParameter.ManOrAuto = 0;
            backupParameter.DatabaseName = 1;
            backupParameter.PathIrisData = BackupPathIrisData;

            BackUpDatabase backUpDatabase = new BackUpDatabase();
            WaitingDialog.ShowWaiting("正在备份数据库，请耐心等待......");

            backUpDatabase.JudgeSuccess += new EventHandler(backUpDatabase_JudgeSuccess);
            backUpDatabase.BackUpOperation(backupParameter);
        }

        /// <summary>
        /// cty
        /// 备份的回调函数
        /// </summary>
        /// <param name="receiveStr">返回的字符串</param>
        /// <param name="e"></param>
        private void backUpDatabase_JudgeSuccess(object str, EventArgs e)
        {
            string[] receiveStr = str as String[];
            if (receiveStr[0] == "10")
            {
                MsgBoxWindow.MsgBox("备份数据库成功！", MsgBoxWindow.MsgIcon.Succeed, MsgBoxWindow.MsgBtns.OK);
            }
            else if (receiveStr[0] == "-10")
            {
                MsgBoxWindow.MsgBox("备份数据库失败！", MsgBoxWindow.MsgIcon.Warning, MsgBoxWindow.MsgBtns.OK);
            }
            else if (receiveStr[0] == "12")
            {
                MsgBoxWindow.MsgBox("备份路径中磁盘分区不存在，请重新填写！", MsgBoxWindow.MsgIcon.Warning, MsgBoxWindow.MsgBtns.OK);
            }
            else if (receiveStr[0] == "13")
            {
                MsgBoxWindow.MsgBox("备份路径格式不正确，请重新填写！", MsgBoxWindow.MsgIcon.Warning, MsgBoxWindow.MsgBtns.OK);
            }
            else if (receiveStr[0] == "error")
            {
                MsgBoxWindow.MsgBox(receiveStr[1], MsgBoxWindow.MsgIcon.Warning, MsgBoxWindow.MsgBtns.OK);
            }
            WaitingDialog.HideWaiting();
        }

        #endregion

    }
}
