/*************************************************************************
** 文件名:   RebuildRecog.cs
** 主要类:   RebuildRecog
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   lzc
** 日  期:   2013-5-2
** 修改人:   cty
** 日  期:   2013-11-5
 *修改内容：增加操作员日志
** 描  述:   RebuildRecog，主要是 重构函数
**
** 版  本:   1.0.0
** 备  注:  命名及代码编写遵守C#编码规范
**
 * ***********************************************************************/

using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ServiceModel.DomainServices.Client;
using Irisking.Web.DataModel;
using EDatabaseError;
using Microsoft.Practices.Prism.Commands;
using IriskingAttend.View.AttendView;
using IriskingAttend.Common;
using IriskingAttend.AsyncControl;
using System.Collections.Generic;
using IriskingAttend.Dialog;
using IriskingAttend.ViewModel.SystemViewModel;

namespace IriskingAttend.ViewModel
{
    public class RebuildRecog : BaseViewModel
    {
        #region 私有变量

        /// <summary>
        /// 重构时统计人数
        /// </summary>
        private int _personSum;

        //要重构的人员姓名，用于写入操作员日志 by cty
        string _rebulidPersonname = "";

        /// <summary>
        /// 异步工作列队
        /// </summary>
        private AsyncActionRunner _runnerTask = null;

        /// <summary>
        /// 每次重构的记录条数，根据经验可以修改
        /// </summary>
        private const int _recordNumber = 2000;       

        #endregion

        #region 绑定数据

        /// <summary>
        /// 查询条件中选择的部门，仅用于添加操作员日志时用  by cty
        /// </summary>
        public string DepartName = "";

        /// <summary>
        /// 需要重构的人员
        /// </summary>
        public BaseViewModelCollection<UserPersonSimple> UserPersonSimpleForRebuildModel { get; set; }

        /// <summary>
        /// 进度条数据绑定
        /// </summary>
        public BarBindObject BarBind { get; set; }

        /// <summary>
        /// button控件是否可用绑定
        /// </summary>
        public ButtonIsEnable BtnBind { get; set; }

        /// <summary>
        /// 重构起始时间
        /// </summary>
        public DateTime RebuitDateTime { get; set; }

        #endregion

        #region 构造函数

        public RebuildRecog()
        {
            ///重构时间--初始化为前一个月
            RebuitDateTime = Convert.ToDateTime( DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd 00:00:00") );

            BarBind = new BarBindObject();
            BtnBind = new ButtonIsEnable();
            UserPersonSimpleForRebuildModel = new BaseViewModelCollection<UserPersonSimple>();
        }

        #endregion

        #region Command 委托变量

        /// <summary>
        /// 重构识别记录委托
        /// </summary>
        private DelegateCommand<object> _rebuildRecogCmd = null;

        /// <summary>
        /// 停止重构识别记录委托
        /// </summary>
        private DelegateCommand     _stopRebuildRecogCmd = null;

        #endregion

        #region ICommand 绑定

        /// <summary>
        /// 重构绑定命令 传入参数为 BaseViewModelCollection<UserPersonSimple>类型
        /// </summary>
        public ICommand RebuildRecogCommand
        {
            get
            {
                if (null == _rebuildRecogCmd)
                {
                    _rebuildRecogCmd = new DelegateCommand<object>(RebuildPersonRecog);
                }
                return _rebuildRecogCmd;
            }
        }

        /// <summary>
        /// 取消重构命令
        /// </summary>
        public ICommand StopRebuildRecogCommand
        {
            get
            {
                if (null == _stopRebuildRecogCmd)
                {
                    _stopRebuildRecogCmd = new DelegateCommand(StopRebuildPersonRecog);
                }
                return _stopRebuildRecogCmd;
            }
        }

        #endregion

        #region  重构

        /// <summary>
        /// 重构识别记录
        /// </summary>
        public void RebuildPersonRecog(object dataGrid)
        {
            _rebulidPersonname = "";
            try
            {
                DataGrid dgPerson = dataGrid as DataGrid;
                ///先清空重构人员列表
                UserPersonSimpleForRebuildModel.Clear();

                ///添加重构人员列表
                foreach (var ar in dgPerson.ItemsSource)
                {
                    if (((UserPersonSimple)ar).is_select)
                    {
                        UserPersonSimpleForRebuildModel.Add((UserPersonSimple)ar);
                    }
                }

                if (UserPersonSimpleForRebuildModel.Count > 0)
                {
                    ///分批次重构人员信息
                    RebuildBatch();
                }
                else
                {
                    MsgBoxWindow.MsgBox( "请选择需要重构的人员！",
                                         MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);
                    return;
                }
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }

        /// <summary>
        /// 分批对人员进行重构
        /// </summary>
        private void RebuildBatch()
        {
            int[] idsSum;
            ///需要重构人员ID数组
            idsSum = new int[UserPersonSimpleForRebuildModel.Count];
            int iSum = 0;
            foreach (var ar in UserPersonSimpleForRebuildModel)
            {
                idsSum[iSum++] = ((UserPersonSimple)ar).person_id;
                _rebulidPersonname += "姓名："+((UserPersonSimple)ar).person_name + "，工号：" + ((UserPersonSimple)ar).work_sn+"；";
            }

            _personSum = UserPersonSimpleForRebuildModel.Count;
            int days = (DateTime.Now - RebuitDateTime).Days;

            if (days > 0)
            {
                ///每次需要重构的人员个数
                int personCount = _recordNumber / days;

                ///绑定数据的状态
                BarBind.BarVisibility = Visibility.Visible;
                BarBind.BarMaximun = Math.Ceiling(_personSum / personCount);
                BarBind.BarValue = 0;
                BtnBind.RebuiltIsEnable = false;

                if (_runnerTask != null)
                {
                    _runnerTask.Stop = false;
                    _runnerTask = null;
                }

                _runnerTask = new AsyncActionRunner(SetTask(idsSum, personCount));
                //下面是运行完成后的处理
                _runnerTask.Completed += (obj, a) =>
                {
                    if ((obj as AsyncActionRunner).Reason == 0)
                    {
                         //添加操作员日志 by cty
                        VmOperatorLog.InsertOperatorLog(1, "考勤重构", "重构开始时间：" + RebuitDateTime.ToString() + "\r\n选择部门：" + DepartName + "\r\n重构人员：" + _rebulidPersonname, () =>
                        {
                            MsgBoxWindow.MsgBox("重构完成！",
                                             MsgBoxWindow.MsgIcon.Succeed, MsgBoxWindow.MsgBtns.OK);

                            BarBind.BarVisibility = Visibility.Collapsed;
                            BtnBind.RebuiltIsEnable = true;
                            BtnBind.StoppbuiltIsEnable = false;
                        });
                    }
                };

                //执行异步通信
                _runnerTask.Execute();
                BtnBind.RebuiltIsEnable = false;
                BtnBind.StoppbuiltIsEnable = true;
            }
            else
            {
                //days = 0;
            }
        }

        /// <summary>
        /// 设置任务链表
        /// </summary>
        /// <param name="idsSum"> 需要重构人员ID数组</param>
        /// <param name="personCount">需要重构人员个数</param>
        /// <returns>任务链表</returns>
        private List<IAsyncAction> SetTask(int[] idsSum, int personCount)
        {
            List<IAsyncAction> lstRebuildPersonBatchTask = new List<IAsyncAction>();

            ///设置需要重构的人员ID
            for (int count = 0; count < _personSum; count += personCount)
            {
                int[] ids;
                if (count + personCount < _personSum)
                {
                    ids = new int[personCount];
                    for (int pCount = 0; pCount < personCount; pCount++)
                    {
                        ids[pCount] = idsSum[count + pCount];
                    }
                }
                else
                {
                    ids = new int[_personSum - count];
                    for (int pCount = 0; pCount < _personSum - count; pCount++)
                    {
                        ids[pCount] = idsSum[count + pCount];
                    }
                }

                ///创建新任务
                var taskTemp = new AsyncAction("ManagerAttend" + count);

                ///任务回调
                taskTemp.SetAction(() =>
                {
                    BarBind.BarValue++;
                    Action<InvokeOperation<int>> rebuildCallBack = CallBackHandleControl<int>.OnInvokeErrorCallBack;

                    CallBackHandleControl<int>.m_sendValue = (result) =>
                    {
                        if (result == 0)
                        {
                        }
                        else
                        {
                            BarBind.BarVisibility = Visibility.Collapsed;
                            if (_runnerTask != null)
                            {
                                _runnerTask.Stop = false;
                                _runnerTask = null;
                            }

                            BtnBind.RebuiltIsEnable = true;
                            BtnBind.StoppbuiltIsEnable = false;
                             //添加操作员日志 by cty
                            VmOperatorLog.InsertOperatorLog(0, "考勤重构", "重构开始时间：" + RebuitDateTime.ToString() + "\r\n选择部门：" + DepartName + "\r\n重构人员：" + _rebulidPersonname, () =>
                            {
                                MsgBoxWindow.MsgBox("重构失败！",
                                           MsgBoxWindow.MsgIcon.Error, MsgBoxWindow.MsgBtns.OK);
                            });
                          
                        }
                        taskTemp.OnCompleted();
                    };

                    ///执行重构
                    ServiceDomDbAcess.GetSever().IrisRebuildAttend(RebuitDateTime, ids, rebuildCallBack, new Object());
                }, true);

                ///添加任务
                lstRebuildPersonBatchTask.Add(taskTemp);
            }
            return lstRebuildPersonBatchTask;
        }

        /// <summary>
        /// 停止重构任务
        /// </summary>
        public void StopRebuildPersonRecog()
        {
            if (_runnerTask != null)
            {
                _runnerTask.Stop = false;
                BtnBind.RebuiltIsEnable = true;
                BtnBind.StoppbuiltIsEnable = false;
                BarBind.BarVisibility = Visibility.Collapsed;
                _runnerTask = null;

                //添加操作员日志 by cty
                VmOperatorLog.InsertOperatorLog(1, "停止考勤重构", "重构开始时间：" + RebuitDateTime.ToString() + "\r\n选择部门：" + DepartName + "\r\n重构人员：" + _rebulidPersonname, () =>
                {
      
                });
            }
        }

        #endregion
    }

    #region 辅助函数

    /// <summary>
    /// 绑定全选CheckBox 类
    /// </summary>
    public class BarBindObject : Entity
    {
        /// <summary>
        /// 进度条是否显示
        /// </summary>
        private Visibility _barVisbility = Visibility.Collapsed;

        /// <summary>
        /// 进度条是否显示
        /// </summary>
        public Visibility BarVisibility
        {
            get
            {
                return _barVisbility;
            }
            set
            {
                RaiseDataMemberChanging("BarVisibility");
                ValidateProperty("BarVisibility", value);
                _barVisbility = value;
                RaiseDataMemberChanged("BarVisibility");
            }
        }

        private double _barMaximun = 100.0;

        /// <summary>
        /// 进度条最大值
        /// </summary>
        public double BarMaximun
        {
            get
            {
                return _barMaximun;
            }
            set
            {
                RaiseDataMemberChanging("BarMaximun");
                ValidateProperty("BarMaximun", value);
                _barMaximun = value;
                RaiseDataMemberChanged("BarMaximun");
            }
        }

        private double _barValue = 0.0;
        /// <summary>
        /// 进度条进度值 
        /// </summary>
        public double BarValue
        {
            get
            {
                return _barValue;
            }
            set
            {
                RaiseDataMemberChanging("BarValue");
                ValidateProperty("BarValue", value);
                _barValue = value;
                RaiseDataMemberChanged("BarValue");
            }
        }

        
        public BarBindObject()
        {
            _barVisbility = Visibility.Collapsed;
            _barMaximun = 100.0;
            _barValue = 0.0;
        }
    }

    /// <summary>
    /// 重构界面按钮IsEnable绑定
    /// </summary>
    public class ButtonIsEnable : Entity
    {
        //重构按钮
        private bool _rebuiltIsEnable = false;
        //停止重构按钮
        private bool _stoppbuiltIsEnable = false;
        //查询按钮
        private bool _queryIsEnable = true;

        /// <summary>
        /// 构造函数 设置默认值
        /// </summary>
        public ButtonIsEnable()
        {
            _rebuiltIsEnable = false;
            _stoppbuiltIsEnable = false;
            _queryIsEnable = true;

        }

        /// <summary>
        /// 查询按钮置灰
        /// </summary>
        public bool QueryIsEnable
        {
            get
            {
                return _queryIsEnable;
            }
            set
            {
                RaiseDataMemberChanging("QueryIsEnable");
                ValidateProperty("QueryIsEnable", value);
                _queryIsEnable = value;
                RaiseDataMemberChanged("QueryIsEnable");
            }
        }

        /// <summary>
        /// 重构按钮置灰
        /// </summary>
        public bool RebuiltIsEnable
        {
            get
            {
                return _rebuiltIsEnable;
            }
            set
            {
                RaiseDataMemberChanging("RebuiltIsEnable");
                ValidateProperty("RebuiltIsEnable", value);
                _rebuiltIsEnable = value;
                RaiseDataMemberChanged("RebuiltIsEnable");
            }
        }

        /// <summary>
        /// 停止重构置灰
        /// </summary>
        public bool StoppbuiltIsEnable
        {

            get
            {
                return _stoppbuiltIsEnable;
            }
            set
            {
                RaiseDataMemberChanging("StoppbuiltIsEnable");
                ValidateProperty("StoppbuiltIsEnable", value);
                _stoppbuiltIsEnable = value;
                RaiseDataMemberChanged("StoppbuiltIsEnable");
            }
        }
    }

    #endregion
}
