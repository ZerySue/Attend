/*************************************************************************
** 文件名:   VmSystemParameter.cs
** 主要类:   VmSystemParameter
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2013-6-14
** 修改人:   cty
** 日  期:   2013-8-4
 *修改内容： 修改自动备份部分的参数设置功能
** 描  述:   VmSystemParameter,系统参数配置
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


namespace IriskingAttend.ViewModel.SystemViewModel
{
    public class VmSystemParameter : BaseViewModel
    {
        #region 变量
        
        /// <summary>
        /// 域服务声明    
        /// </summary>
        private DomainServiceIriskingAttend _domSrvDbAccess = new DomainServiceIriskingAttend();

        /// <summary>
        /// 通知后台是否成功  true：成功  false：失败。 默认为成功。
        /// </summary>
        private bool _notifyServerSucceed = true;

        #endregion  

        #region 与页面绑定的属性

        private system_param _systemParam;
        /// <summary>
        /// 系统参数
        /// </summary>
        public system_param SystemParam
        {
            get
            { 
                return _systemParam; 
            }
            set
            {
                _systemParam = value;
                OnPropertyChanged<system_param>(() => this.SystemParam); 
            }
        }

        private int _overTimeHour;
        /// <summary>
        /// 超时时间小时部分 cty
        /// </summary>
        public int OverTimeHour
        {
            get 
            {
                return _overTimeHour;
            }
            set
            {
                _overTimeHour = value;
                OnPropertyChanged<int>(() => this.OverTimeHour);
            }
        }

        private int _overTimeMinute;
        /// <summary>
        /// 超时时间分钟部分 cty
        /// </summary>
        public int OverTimeMinute
        {
            get
            {
                return _overTimeMinute;
            }
            set
            {
                _overTimeMinute = value;
                OnPropertyChanged<int>(() => this.OverTimeMinute);
            }
        }

        private work_cnt_policy _workCntPolicy;
        /// <summary>
        /// 记工策略
        /// </summary>
        public work_cnt_policy WorkCntPolicy
        {
            get 
            { 
                return _workCntPolicy; 
            }
            set
            {
                _workCntPolicy = value;
                OnPropertyChanged<work_cnt_policy>(() => this.WorkCntPolicy); 
            }
        }

        #region 与页面绑定的备份数据库的属性 by cty

        private int _periodIirsApp;
        /// <summary>
        /// 考勤库的备份周期
        /// </summary>
        public int PeriodIirsApp
        {
            get
            {
                return _periodIirsApp;
            }
            set
            {
                _periodIirsApp = value;
                OnPropertyChanged<int>(()=>this.PeriodIirsApp);
            }
        }

        private int _subPeriodIirsApp;
        /// <summary>
        /// 考勤库的备份日期
        /// </summary>
        public int SubPeriodIirsApp
        {
            get
            {
                return _subPeriodIirsApp;
            }
            set
            {
                _subPeriodIirsApp = value;
                OnPropertyChanged<int>(()=>this.SubPeriodIirsApp);
            }
        }

        private DateTime _concreteTimeIirsApp;
        /// <summary>
        /// 考勤库的备份具体时间
        /// </summary>
        public DateTime ConcreteTimeIirsApp
        {
            get
            {
                return _concreteTimeIirsApp;
            }
            set
            {
                _concreteTimeIirsApp = value;
                OnPropertyChanged<DateTime>(() => this.ConcreteTimeIirsApp);
            }
        }

        private string _pathIirsApp;
        /// <summary>
        /// 考勤库的备份地址
        /// </summary>
        public string PathIirsApp
        {
            get
            {
                return _pathIirsApp;
            }
            set
            {
                _pathIirsApp = value;
                OnPropertyChanged<string>(() => this.PathIirsApp);
            }
        }

        private int _periodIirsData;
        /// <summary>
        /// 虹膜库的备份周期
        /// </summary>
        public int PeriodIirsData
        {
            get
            {
                return _periodIirsData;
            }
            set
            {
                _periodIirsData = value;
                OnPropertyChanged<int>(() => this.PeriodIirsData);
            }
        }

        private int _subPeriodIirsData;
        /// <summary>
        /// 虹膜库的备份日期
        /// </summary>
        public int SubPeriodIirsData
        {
            get
            {
                return _subPeriodIirsData;
            }
            set
            {
                _subPeriodIirsData = value;
                OnPropertyChanged<int>(() => this.SubPeriodIirsData);
            }
        }

        private DateTime _concreteTimeIirsData;
        /// <summary>
        /// 虹膜库的备份具体时间
        /// </summary>
        public DateTime ConcreteTimeIirsData
        {
            get
            {
                return _concreteTimeIirsData;
            }
            set
            {
                _concreteTimeIirsData = value;
                OnPropertyChanged<DateTime>(() => this.ConcreteTimeIirsData);
            }
        }

        private string _pathIirsData;
        /// <summary>
        /// 虹膜库的备份地址
        /// </summary>
        public string PathIirsData
        {
            get
            {
                return _pathIirsData;
            }
            set
            {
                _pathIirsData = value;
                OnPropertyChanged<string>(() => this.PathIirsData);
            }
        }
        #endregion

        private Dictionary<int, string> _dictWorkCntPolicyAccuracy;
        /// <summary>
        /// 用工策略参数精确度绑定：数字指示与用工策略参数（当工数存在小数部分时，对小数部分的处理意见）字符之间的对应关系
        /// </summary>
        public Dictionary<int, string> DictWorkCntPolicyAccuracy
        {
            get
            {
                return _dictWorkCntPolicyAccuracy;
            }
            set
            {
                _dictWorkCntPolicyAccuracy = value;
                OnPropertyChanged<Dictionary<int, string>>(() => this.DictWorkCntPolicyAccuracy);
            }
        }

        private KeyValuePair<int, string> _selectAccuracy;
        /// <summary>
        /// 当前选中的用工策略参数精确度绑定
        /// </summary>
        public KeyValuePair<int, string> SelectAccuracy
        {
            get
            {
                return _selectAccuracy;
            }
            set
            {
                _selectAccuracy = value;
                OnPropertyChanged<KeyValuePair<int, string>>(() => this.SelectAccuracy);
            }
        }
     
        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public VmSystemParameter()
        {
            //初始化与页面绑定的变量属性
            SystemParam = new system_param();
            WorkCntPolicy = new work_cnt_policy();

            DictWorkCntPolicyAccuracy = GetWorkCntPolicyAccuracyDictionary();
            SelectAccuracy = new KeyValuePair<int, string>(0, "");
        }

        #endregion

        #region  辅助函数

        /// <summary>
        /// 获得数字指示与用工策略参数（当工数存在小数部分时，对小数部分的处理意见）字符之间的对应关系
        /// </summary>
        /// <returns>数字指示与用工策略参数（当工数存在小数部分时，对小数部分的处理意见）字符之间的字典对应关系</returns>
        private Dictionary<int, string> GetWorkCntPolicyAccuracyDictionary()
        {
            Dictionary<int, string> DictWorkCntPolicyAccuracy = new Dictionary<int, string>();

            ///往字典内添加数字指示与用工策略参数（当工数存在小数部分时，对小数部分的处理意见）
            ///字符之间的对应关系            
            DictWorkCntPolicyAccuracy.Add(0, "不保留，直接丢弃小数部分");
            DictWorkCntPolicyAccuracy.Add(5, "不保留，四舍五入");
            DictWorkCntPolicyAccuracy.Add(9, "不保留，进一法");
            DictWorkCntPolicyAccuracy.Add(15, "保留，设为半工，即0.5个工");

            return DictWorkCntPolicyAccuracy;
        }

        #endregion

        #region  控件绑定函数操作

        /// <summary>
        /// 提交系统参数修改到后台进行修改
        /// </summary>
        public void SubmitSystemParam()
        {
            //默认通知后台成功
            _notifyServerSucceed = true;

            //ria方式连接后台，将修改后的参数修改到数据库
            SystemParameterSet();
        }

        #endregion

        #region  通过ria连接后台，数据库相关操作

        /// <summary>
        /// 异步获取数据库中数据，获取参数设置表
        /// </summary>
        public void GetSystemParams()
        {
            try
            {
                _domSrvDbAccess = new DomainServiceIriskingAttend();

                //显示等待界面
                WaitingDialog.ShowWaiting();

                //获得系统参数信息
                EntityQuery<system_param> paramList = _domSrvDbAccess.GetSystemParamQuery();

                ///回调异常类
                Action<LoadOperation<system_param>> loadParamCallBack = new Action<LoadOperation<system_param>>(ErrorHandle<system_param>.OnLoadErrorCallBack);
                ///异步事件
                LoadOperation<system_param> loadParam = this._domSrvDbAccess.Load(paramList, loadParamCallBack, null);
                loadParam.Completed += delegate
                {
                    //隐藏等待界面
                    WaitingDialog.HideWaiting();

                    //异步获取数据，将获取到的系统参数信息赋值到绑定变量SystemParam中去
                    foreach (system_param param in loadParam.Entities)
                    {
                        SystemParam.dup_time = param.dup_time;
                        SystemParam.in_dup_recog = param.in_dup_recog;
                        SystemParam.out_dup_recog = param.out_dup_recog;
                        OverTimeHour = param.over_time / 60;  //by cty
                        OverTimeMinute = param.over_time % 60;//by cty
                    }

                    //获取记工策略参数信息
                    GetWorkCntPolicy();           
                };
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }

        /// <summary>
        /// 异步获取数据库中数据，获取记工策略表
        /// </summary>
        private void GetWorkCntPolicy()
        {
            try
            {
                _domSrvDbAccess = new DomainServiceIriskingAttend();

                //显示等待界面
                WaitingDialog.ShowWaiting();

                //获得记工策略参数信息
                EntityQuery<work_cnt_policy> workCntPolicyList = _domSrvDbAccess.GetWorkCntPolicyQuery();

                ///回调异常类
                Action<LoadOperation<work_cnt_policy>> loadPolicyCallBack = new Action<LoadOperation<work_cnt_policy>>(ErrorHandle<work_cnt_policy>.OnLoadErrorCallBack);
                ///异步事件
                LoadOperation<work_cnt_policy> loadPolicy = this._domSrvDbAccess.Load(workCntPolicyList, loadPolicyCallBack, null);
                loadPolicy.Completed += delegate
                {
                    //隐藏等待界面
                    WaitingDialog.HideWaiting();

                    //异步获取数据,将获取到的记工策略参数信息赋值到绑定变量WorkCntPolicy中去
                    foreach (work_cnt_policy policy in loadPolicy.Entities)
                    {
                        WorkCntPolicy.lt = policy.lt;
                        WorkCntPolicy.gt = policy.gt;
                        WorkCntPolicy.accuracy = policy.accuracy;

                        //记工策略精确度下拉框显示的记工策略精确度            
                        if (this.DictWorkCntPolicyAccuracy.ContainsKey(this.WorkCntPolicy.accuracy))
                        {
                            this.SelectAccuracy = new KeyValuePair<int, string>(WorkCntPolicy.accuracy, this.DictWorkCntPolicyAccuracy[WorkCntPolicy.accuracy]);
                        }
                    }

                    //获取自动备份参数信息
                    GetBackupParameter();
                };
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }

        /// <summary>
        /// cty
        /// 异步获取数据库中数据，获取数据库备份参数表
        /// </summary>
        public void GetBackupParameter()
        {
            BackupParameter backupParameter = new BackupParameter();

            backupParameter.ManOrAuto = 2;

            BackUpDatabase backUpDatabase = new BackUpDatabase();
            backUpDatabase.JudgeSuccess += new EventHandler(backUpDatabase_JudgeSuccess);
            backUpDatabase.BackUpOperation(backupParameter);
        }

        /// <summary>
        /// 提交系统参数修改设置
        /// </summary>
        private void SystemParameterSet()
        {
            try
            {
                Action<InvokeOperation<byte>> onInvokeErrCallBack = CallBackHandleControl<byte>.OnInvokeErrorCallBack;
                CallBackHandleControl<byte>.m_sendValue = (o) =>
                {
                    WaitingDialog.HideWaiting();

                    //异步获取数据，成功后，继续提交其它参数
                    if (0 != o )
                    {
                        //提交记工策略参数修改设置
                        WorkCntPolicySet();

                        if (0XFF == o)
                        {
                            _notifyServerSucceed = false;
                        }
                    }
                    else
                    {
                        MsgBoxWindow.MsgBox( "参数设置失败！", MsgBoxWindow.MsgIcon.Error, MsgBoxWindow.MsgBtns.OK);                       
                    }
                };

                WaitingDialog.ShowWaiting("正在设置系统参数，请等待。。。");

                //通过后台提交系统参数修改设置 //by cty
                _domSrvDbAccess.ModifySystemParam(OverTimeHour*60+OverTimeMinute,SystemParam.dup_time, SystemParam.in_dup_recog, SystemParam.out_dup_recog, onInvokeErrCallBack, null);
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }

        /// <summary>
        /// 提交记工策略参数修改设置
        /// </summary>
        private void WorkCntPolicySet()
        {    
            try
            {
                Action<InvokeOperation<byte>> onInvokeErrCallBack = CallBackHandleControl<byte>.OnInvokeErrorCallBack;
                CallBackHandleControl<byte>.m_sendValue = (o) =>
                {
                    WaitingDialog.HideWaiting();

                    //异步获取数据,成功后，继续提交其它参数
                    if (0X00 != o)
                    {
                        //提交数据库备份参数修改设置
                        AutoBackupParameterSet();

                        if (0XFF == o)
                        {
                            _notifyServerSucceed = false;
                        }
                    }
                    else
                    {
                        MsgBoxWindow.MsgBox( "参数设置失败！", MsgBoxWindow.MsgIcon.Error, MsgBoxWindow.MsgBtns.OK);
                    }
                };

                WaitingDialog.ShowWaiting("正在设置标准记工策略，请等待。。。");

                //通过后台提交记工策略参数修改设置
                _domSrvDbAccess.ModifyWorkCntPolicy(WorkCntPolicy.lt, WorkCntPolicy.gt, this.SelectAccuracy.Key, onInvokeErrCallBack, null);
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }

        /// <summary>
        /// cty
        /// 提交数据库备份参数修改设置
        /// </summary>
        private void AutoBackupParameterSet()
        {
            //初始化要传递的结构体
            BackupParameter backupParameter = new BackupParameter();

            //给要传递的结构体赋值
            backupParameter.ManOrAuto = 1;
            backupParameter.PathIrisApp = PathIirsApp;
            backupParameter.PeriodIrisApp = PeriodIirsApp;
            backupParameter.SubPeriokIrisApp = SubPeriodIirsApp + 1;
            backupParameter.ConcreteTimeIrisApp = ConcreteTimeIirsApp;

            backupParameter.PathIrisData = PathIirsData;
            backupParameter.PeriodIrisData = PeriodIirsData;
            backupParameter.SubPeriokIrisData = SubPeriodIirsData + 1;
            backupParameter.ConcreteTimeIrisData = ConcreteTimeIirsData;

            BackUpDatabase backUpDatabase = new BackUpDatabase();
            backUpDatabase.JudgeSuccess += new EventHandler(backUpDatabase_JudgeSuccess);
            backUpDatabase.BackUpOperation(backupParameter);         
        }
        
        /// <summary>
        /// cty
        /// 备份的回调函数
        /// </summary>
        /// <param name="str">返回的字符串</param>
        /// <param name="e"></param>
        private void backUpDatabase_JudgeSuccess(object str, EventArgs e)
        { 
            string[] receiveStr = str as String[];
            if (receiveStr[0] == "12")
            {
                MsgBoxWindow.MsgBox("备份路径中磁盘分区不存在，请重新填写！", MsgBoxWindow.MsgIcon.Warning, MsgBoxWindow.MsgBtns.OK);
                _notifyServerSucceed = false;
            }
            else if (receiveStr[0] == "13")
            {
                MsgBoxWindow.MsgBox("备份路径格式不正确，请重新填写！", MsgBoxWindow.MsgIcon.Warning, MsgBoxWindow.MsgBtns.OK);
                _notifyServerSucceed = false;
            }
            else if (receiveStr[0] == "11")
            {
                if (_notifyServerSucceed)
                {
                    MsgBoxWindow.MsgBox("参数设置成功！", MsgBoxWindow.MsgIcon.Succeed, MsgBoxWindow.MsgBtns.OK);
                }
                else
                {
                    MsgBoxWindow.MsgBox("参数设置成功，但通知后台失败，请检查后台！", MsgBoxWindow.MsgIcon.Warning, MsgBoxWindow.MsgBtns.OK);
                }
            }
            else if (receiveStr[0] == "error")
            {
                MsgBoxWindow.MsgBox(receiveStr[1], MsgBoxWindow.MsgIcon.Warning, MsgBoxWindow.MsgBtns.OK);
            }
            else
            {
                //将从配置文件返回的自动备份参数显示的界面上
                PeriodIirsApp = int.Parse(receiveStr[0]);
                SubPeriodIirsApp = int.Parse(receiveStr[1]) - 1;
                ConcreteTimeIirsApp = DateTime.Parse(receiveStr[2]);
                PathIirsApp = receiveStr[3];

                PeriodIirsData = int.Parse(receiveStr[4]);
                SubPeriodIirsData = int.Parse(receiveStr[5]) - 1;
                ConcreteTimeIirsData = DateTime.Parse(receiveStr[6]);
                PathIirsData = receiveStr[7];
            }
        }

        #endregion
    }
}
