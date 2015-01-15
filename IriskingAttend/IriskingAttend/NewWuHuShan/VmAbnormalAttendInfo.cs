/*************************************************************************
** 文件名:   VmAbnormalAttendInfo.cs
×× 主要类:   VmAbnormalAttendInfo
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   wz
** 日  期:   2013-12-20
** 修改人: 
** 日  期:   
** 描  述:   VmAbnormalAttendInfo类,五虎山定制异常考勤查询界面vm层
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
using IriskingAttend.View.PeopleView;
using IriskingAttend.Dialog;
using System.Windows.Navigation;
using IriskingAttend.View;
using IriskingAttend.BehaviorSelf;
using System.Linq;
using System.Text;
using IriskingAttend.ViewModel.SystemViewModel;
using IriskingAttend.ViewModel;
using IriskingAttend.Web.WuHuShan;
using IriskingAttend.ViewModel.PeopleViewModel;
using System.Threading;
using System.Windows.Threading;
using IriskingAttend.AsyncControl;

namespace IriskingAttend.NewWuHuShan
{
    public class VmAbnormalAttendInfo : BaseViewModel
    {


        #region 字段声明

        private List<AttendRecordInfo_WuhuShan> _attendRecordInfosSource;

       

        private List<int> _attendStateFilterList; //过滤条件

        private ComboBox _cmbDepart;  //部门选择过滤项
         
        // 异步任务执行器
        private AsyncActionRunner _runnerTask;

        private int _asyncRusltAddRecord; //异步结果

        //回调函数声明
        public delegate void CompleteCallBack();

        #endregion

        #region    与界面绑定的属性

        /// <summary>
        /// 查询
        /// </summary>
        public DelegateCommand QueryCommand
        { 
            get; 
            set; 
        }

        /// <summary>
        /// 批量添加识别记录
        /// </summary>
        public DelegateCommand BatchAddRecordCommand
        {
            get;
            set;
        }

        /// <summary>
        /// 批量添加定位记录
        /// </summary>
        public DelegateCommand BatchAddLocateRecordCommand
        {
            get;
            set;
        }

        /// <summary>
        /// 取消异步任务
        /// </summary>
        public DelegateCommand StopAsyncTaskCommand
        {
            get;
            set;
        }

        /// <summary>
        /// 同步定位卡考勤
        /// </summary>
        public DelegateCommand SyncLocateAttendCommand
        {
            get;
            set;
        }

        private Visibility _isAsyncTaskVisible = Visibility.Collapsed;
        /// <summary>
        /// 表示异步任务是否正在进行
        /// </summary>
        public Visibility IsAsyncTaskVisible
        {
            get { return _isAsyncTaskVisible; }
            set
            {
                _isAsyncTaskVisible = value;
                this.OnPropertyChanged(() => IsAsyncTaskVisible);
            }
        }

        private double _asyncTaskProgress = 0;
        /// <summary>
        /// 表示异步任务进度条的值，最大为100，最小为0
        /// </summary>
        public double AsyncTaskProgress
        {
            get { return _asyncTaskProgress; }
            set
            {
                _asyncTaskProgress = value;
                this.OnPropertyChanged(() => AsyncTaskProgress);
            }
        }


        private BaseViewModelCollection<AttendRecordInfo_WuhuShan> _attendRecordInfos = new BaseViewModelCollection<AttendRecordInfo_WuhuShan>();
        /// <summary>
        /// 异常考勤列表
        /// </summary>
        public BaseViewModelCollection<AttendRecordInfo_WuhuShan> AttendRecordInfos
        {
            get 
            {
                return _attendRecordInfos; 
            }
            set
            {
                _attendRecordInfos = value;
                this.OnPropertyChanged(() => this.AttendRecordInfos);
            }
        }


        /// <summary>
        /// 全选按钮的绑定类
        /// </summary>
        public MarkObject MarkObj
        { 
            get; 
            set; 
        }

        private bool _isSyncLocateAttendInfoBtnEnable;
        /// <summary>
        /// 同步定位卡考勤按钮的Enable属性
        /// </summary>
        public bool IsSyncLocateAttendInfoBtnEnable
        {
            get { return _isSyncLocateAttendInfoBtnEnable; }
            set
            {
                _isSyncLocateAttendInfoBtnEnable = value;
                this.OnPropertyChanged(() => this.IsSyncLocateAttendInfoBtnEnable);
            }
        }

        private bool _isBatchOperateBtnEnable;
        /// <summary>
        /// 批量操作按钮的Enable属性
        /// </summary>
        public bool IsBatchOperateBtnEnable
        {
            get { return _isBatchOperateBtnEnable; }
            set
            {
                _isBatchOperateBtnEnable = value;
                this.OnPropertyChanged(() => this.IsBatchOperateBtnEnable);
            }
        }

        private DateTime? _beginTime;
        /// <summary>
        /// 查询开始时间
        /// </summary>
        public DateTime? BeginTime
        {
            get { return _beginTime; }
            set
            {
                _beginTime = value;
                this.OnPropertyChanged(() => this.BeginTime);
            }
        }

        private DateTime? _endTime;
        /// <summary>
        /// 查询结束时间
        /// </summary>
        public DateTime? EndTime
        {
            get { return _endTime; }
            set
            {
                _endTime = value;
                if (_endTime.HasValue)
                {
                    _endTime = Convert.ToDateTime(_endTime.Value.ToString("yyyy-MM-dd 23:59:59") );
                }
                this.OnPropertyChanged(() => this.EndTime);
            }
        }

     

        private string _personName="";
        /// <summary>
        /// 人员姓名
        /// </summary>
        public string PersonName
        {
            get { return _personName; }
            set
            {
                _personName = value;
                this.OnPropertyChanged(() => this.PersonName);
            }
        }

        private string _workSn="";
        /// <summary>
        /// 工号
        /// </summary>
        public string WorkSn
        {
            get { return _workSn; }
            set
            {
                _workSn = value;
                this.OnPropertyChanged(() => this.WorkSn);
            }
        }


        private List<int> _allPersonIds;
        /// <summary>
        /// 所有已注册人员的id列表 by cty
        /// </summary>
        public List<int> AllPersonIds
        {
            get { return _allPersonIds; }
            set
            {
                _allPersonIds = value;
                this.OnPropertyChanged(() => this.AllPersonIds);
            }
        }

        private Visibility _isAsyncTaskLocateVisible = Visibility.Collapsed;
        /// <summary>
        /// 表示异步定位数据任务是否正在进行 by cty
        /// </summary>
        public Visibility IsAsyncTaskLocateVisible
        {
            get { return _isAsyncTaskLocateVisible; }
            set
            {
                _isAsyncTaskLocateVisible = value;
                this.OnPropertyChanged(() => IsAsyncTaskLocateVisible);
            }
        }

        private double _asyncTaskLocateProgress = 0;
        /// <summary>
        /// 表示同步定位数据异步任务进度条的值，最大为100，最小为0 by cty
        /// </summary>
        public double AsyncTaskLocateProgress
        {
            get { return _asyncTaskLocateProgress; }
            set
            {
                _asyncTaskLocateProgress = value;
                this.OnPropertyChanged(() => AsyncTaskLocateProgress);
            }
        }

        /// <summary>
        /// 取消同步定位数据异步任务 by cty
        /// </summary>
        public DelegateCommand StopAsyncTaskLocateCommand
        {
            get;
            set;
        }

        #endregion

        #region 构造函数

        public VmAbnormalAttendInfo()
        {
            QueryCommand = new DelegateCommand(Query);
            BatchAddRecordCommand = new DelegateCommand(BatchAddRecord);
            BatchAddLocateRecordCommand = new DelegateCommand(BatchAddLocateRecord);
            SyncLocateAttendCommand = new DelegateCommand(SyncLocateAttend);
            StopAsyncTaskCommand = new DelegateCommand(StopAsyncTask);
            StopAsyncTaskLocateCommand = new DelegateCommand(StopAsyncTaskLocate);//by cty
           
            _attendStateFilterList = new List<int>();
            _attendStateFilterList.Add(0);
            _attendStateFilterList.Add(1);
            _attendStateFilterList.Add(2);
            _attendStateFilterList.Add(3);
            _attendStateFilterList.Add(4);

            IsSyncLocateAttendInfoBtnEnable = true;
            IsAsyncTaskVisible = Visibility.Collapsed;
            AllPersonIds=new List<int>();
        }

        #endregion

      

        #region 界面事件响应
        //检查日期
        private bool CheckTime()
        {
            if (BeginTime.HasValue && EndTime.HasValue)
            {
                return true;
            }
            else
            {
                MsgBoxWindow.MsgBox("请确保开始日期和结束日期不为空！",
                       MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);
                return false;
            }
        }

        //有无数据
        private bool CheckData()
        {
            if (AttendRecordInfos != null && AttendRecordInfos.Count>0)
            {
                return true;
            }
            else
            {
                MsgBoxWindow.MsgBox("请确保有考勤数据！",
                       MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);
                return false;
            }
        }

        //查询
        private void Query()
        {
            //检查时间是否为空
            if (!CheckTime())
            {
                return;
            }
            GetAbnormalAttendInfosRia();
        }

        //构建异步任务队列
        private List<IAsyncAction> GetBatchAddRecTasks(int[] personIds, DateTime[] recogTimes,
            int[] devTypes, string userName, int countPerTask)
        {
            List<IAsyncAction> tasks = new List<IAsyncAction>();

            for (int i = 0; i < personIds.Length; i += countPerTask)
            {
                int[] curPersonIds;
                DateTime[] curRecogTimes;
                int[] curDevTypes;
                //构造一次任务的人数
                if (i + countPerTask - 1 < personIds.Length)
                {
                    curPersonIds = new int[countPerTask];
                    curRecogTimes = new DateTime[countPerTask];
                    curDevTypes = new int[countPerTask];
                    for (int j = 0; j < countPerTask; j++)
                    {
                        curPersonIds[j] = personIds[i + j];
                        curRecogTimes[j] = recogTimes[i + j];
                        curDevTypes[j] = devTypes[i + j];
                    }
                }
                else
                {
                    curPersonIds = new int[personIds.Length - i];
                    curRecogTimes = new DateTime[personIds.Length - i];
                    curDevTypes = new int[personIds.Length - i];
                    for (int j = 0; j < curPersonIds.Length; j++)
                    {
                        curPersonIds[j] = personIds[i + j];
                        curRecogTimes[j] = recogTimes[i + j];
                        curDevTypes[j] = devTypes[i + j];
                    }
                }


                ///创建新任务
                var taskTemp = new AsyncAction("BatchAddRecTask" + i);

                //设置任务工作内容
                taskTemp.SetAction(() =>
                {

                    Action<InvokeOperation<int>> insertUserPersonRecogCallBack = CallBackHandleControl<int>.OnInvokeErrorCallBack;

                    CallBackHandleControl<int>.m_sendValue = (o) =>
                    {
                        if (o != 0)
                        {
                            _asyncRusltAddRecord = o;
                        }
                       
                        //表示当前任务完成了,执行下一个任务
                        taskTemp.OnCompleted();

                    };
                    ServiceDomDbAcess.GetSever().IrisBatchInsertRecogLog_WuHuShan(curPersonIds, curRecogTimes, curDevTypes, userName,
                        insertUserPersonRecogCallBack, new Object());
                }, true);

                tasks.Add(taskTemp);

            }

            return tasks;
        }

        /// <summary>
        /// 根据虹膜考勤记录添加定位卡记录
        /// </summary>
        private void BatchAddLocateRecord()
        {
            List<int> personIds = new List<int>();            
            List<DateTime> inLocateTimes = new List<DateTime>();
            List<DateTime> outLocateTimes = new List<DateTime>();
            List<DateTime> attendDays = new List<DateTime>();


            foreach (var item in AttendRecordInfos)
            {
                if (item.isSelected)
                {
                    if (item.in_well_time != null && item.out_well_time != null )
                    {
                        personIds.Add(item.person_id);

                        inLocateTimes.Add(item.in_well_time.Value);
                        outLocateTimes.Add(item.out_well_time.Value);
                        attendDays.Add(item.attend_day.Value.Date);
                    }                    
                }
            }

            if (personIds.Count == 0)
            {
                MsgBoxWindow.MsgBox("需要添加定位卡记录的人数为0！",
                      MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);
            }
            else
            {
                MsgBoxWindow.MsgBox("是否依照虹膜出入井记录添加定位卡出入井记录？",
                     MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OKCancel, (e) =>
                     {
                         if (e == IriskingAttend.Dialog.MsgBoxWindow.MsgResult.OK)
                         {
                             BatchAddLocateRecordRia(personIds, inLocateTimes, outLocateTimes, attendDays);
                         }
                     });

            }
        }

        /// <summary>
        /// ria方式通过后台增加定位记录
        /// </summary>       
        private void BatchAddLocateRecordRia(List<int> personIds, List<DateTime> inLocateTimes, List<DateTime> outLocateTimes, List<DateTime> attendDays)
        {
            try
            {
                Action<InvokeOperation<bool>> onInvokeErrCallBack = CallBackHandleControl<bool>.OnInvokeErrorCallBack;
                CallBackHandleControl<bool>.m_sendValue = (o) =>
                {
                    //隐藏等待界面
                    WaitingDialog.HideWaiting();

                    //异步获取数据                    
                    if (o)
                    {
                        SyncSelectedLocateRecord( false );
                        MsgBoxWindow.MsgBox("增加定位记录操作成功！", MsgBoxWindow.MsgIcon.Succeed, MsgBoxWindow.MsgBtns.OK);
                    }
                    else
                    {
                        MsgBoxWindow.MsgBox("增加定位记录操作失败！", MsgBoxWindow.MsgIcon.Error, MsgBoxWindow.MsgBtns.OK);
                    }

                };

                //显示等待界面
                WaitingDialog.ShowWaiting();

                //通过后台执行增加定位记录动作
                ServiceDomDbAcess.GetSever().BatchAddLocateRecord(personIds.ToArray(), inLocateTimes.ToArray(), outLocateTimes.ToArray(), attendDays.ToArray(), onInvokeErrCallBack, null);
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }


        //根据定位卡记录，添加虹膜考勤记录
        //添加人员为当前选择的人员
        private void BatchAddRecord()
        {

            List<int> personIds = new List<int>();
            List<int> devTypes = new List<int>();
            List<DateTime> recoTimes = new List<DateTime>();

            foreach (var item in AttendRecordInfos)
            {
                if (item.isSelected)
                {
                    if (item.in_well_time == null && item.in_locate_time != null)
                    {
                        personIds.Add(item.person_id);
                        devTypes.Add(1); //1代表入井
                        recoTimes.Add(item.in_locate_time.Value);
                    }
                    if (item.out_well_time == null && item.out_locate_time != null)
                    {
                        personIds.Add(item.person_id);
                        devTypes.Add(2); //2代表出井
                        recoTimes.Add(item.out_locate_time.Value);
                    }
                }
            }

            if (personIds.Count == 0)
            {
                MsgBoxWindow.MsgBox("需要添加识别记录的人数为0！",
                      MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);
            }
            else
            {
                MsgBoxWindow.MsgBox("是否依照定位卡出入井记录添加虹膜出入井记录？",
                     MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OKCancel, (e) =>
                     {
                         if (e == IriskingAttend.Dialog.MsgBoxWindow.MsgResult.OK)
                         {
                             StartBatchAddRecogTask(personIds, recoTimes, devTypes);
                         }
                     });
              
            }
        }

        private void StartBatchAddRecogTask(List<int> personIds,List<DateTime> recoTimes,List<int> devTypes)
        {
            //构建异步任务执行器
            if (_runnerTask != null)
            {
                _runnerTask.Stop = false;
                _runnerTask = null;
            }
            string logDescrip = "";
            int index = 1;
            foreach (var ar in this.AttendRecordInfos)
            {
                if (ar.isSelected )//&& ar.attend_record_id > 0)
                {
                    logDescrip += string.Format("{0}.姓名：{1}，工号：{2}，部门名称：{3}，入井定位时间：{4}，出井定位时间：{5}；\r\n",index++, ar.work_sn, ar.name, ar.depart_name, ar.in_locate_time, ar.out_locate_time);
                }
            }

            this.IsBatchOperateBtnEnable = false;
            this.IsSyncLocateAttendInfoBtnEnable = false;

            //开始添加识别记录前 的工作
            _asyncRusltAddRecord = 0;
            IsAsyncTaskVisible = Visibility.Visible;
            AsyncTaskProgress = 0;

            _runnerTask = new AsyncActionRunner(GetBatchAddRecTasks(personIds.ToArray(), recoTimes.ToArray(),
                     devTypes.ToArray(), VmLogin.GetUserName(), 20));

            //下面是运行完成后的处理
            _runnerTask.Completed += (obj, a) =>
            {
                this.IsBatchOperateBtnEnable = CheckIsAnyOneSelected();
                IsAsyncTaskVisible = Visibility.Collapsed;
                
                //如果取消了批量添加识别记录的操作，则不弹出对话框
                if (!_runnerTask.Stop)
                {
                    _runnerTask = null;
                    VmOperatorLog.InsertOperatorLog(1, "取消按定位时间批量添加识别记录", logDescrip, () =>
                        {
                            this.IsSyncLocateAttendInfoBtnEnable = true;
                            GetAbnormalAttendInfosRia();
                        });
                    return;
                }
                _runnerTask = null;
                if (_asyncRusltAddRecord == 0)
                {
                    //添加操作员日志
                    VmOperatorLog.InsertOperatorLog(1, "按定位时间批量添加识别记录成功", logDescrip, () =>
                    {
                        MsgBoxWindow.MsgBox("批量添加识别记录成功！", MsgBoxWindow.MsgIcon.Succeed, MsgBoxWindow.MsgBtns.OK, ( Result ) =>
                            {
                                this.IsSyncLocateAttendInfoBtnEnable = true;
                                GetAbnormalAttendInfosRia();
                            });                        
                    });
                }
                else
                {
                    //添加操作员日志
                    VmOperatorLog.InsertOperatorLog(0, "按定位时间批量添加识别记录失败", logDescrip, () =>
                    {
                        MsgBoxWindow.MsgBox("批量添加识别记录失败！", MsgBoxWindow.MsgIcon.Error, MsgBoxWindow.MsgBtns.OK, (Result) =>
                            {
                                this.IsSyncLocateAttendInfoBtnEnable = true;
                                GetAbnormalAttendInfosRia();
                            });                        
                    });                    
                }
            };

            //通知进度条改变
            _runnerTask.ProgressChanged += (obj, e) =>
            {
                AsyncTaskProgress = e.Percent * 100;
            };


            //执行异步任务队列
            _runnerTask.Execute();
        }

        /// <summary>
        /// 停止异步命令
        /// </summary>
        private void StopAsyncTask()
        {
            string  info = "是否取消批量添加识别记录？";
            MsgBoxWindow.MsgBox(
                    info,
                    Dialog.MsgBoxWindow.MsgIcon.Information,
                    Dialog.MsgBoxWindow.MsgBtns.OKCancel, (e) =>
                    {
                        if (e == MsgBoxWindow.MsgResult.OK)
                        {
                            CancelAsyncTask();
                        }
                    });

        }

        /// <summary>
        /// 停止异步命令,不带取消提示框
        /// </summary>
        public void CancelAsyncTask()
        {
            if (_runnerTask != null && _runnerTask.Stop)
            {
                _runnerTask.Stop = false;
                IsAsyncTaskVisible = Visibility.Collapsed;
                this.IsSyncLocateAttendInfoBtnEnable = true;
            }
        }

        private void SyncSelectedLocateRecord(bool bShowResultDialog)
        {
            List<int> personIds = new List<int>();

            foreach (var item in AttendRecordInfos)
            {
                if (item.isSelected && !personIds.Contains(item.person_id))
                {
                    personIds.Add(item.person_id);
                }
            }

            if (personIds.Count < 1)
            {
                GetAllPersonids(BeginTime.Value, EndTime.Value, bShowResultDialog, null);
            }
            else
            {
                SyncLocateAttendTask(personIds.ToArray(), BeginTime.Value, EndTime.Value, bShowResultDialog, null);
            }
        }

        //开始同步定位卡考勤
        private void SyncLocateAttend()
        {
            //检查时间是否为空
            if (!CheckTime())
            {
                return;
            }

            if (EndTime.Value - BeginTime.Value > TimeSpan.FromDays(31))
            {
                MsgBoxWindow.MsgBox("请确保开始日期和结束日期相差不超过31天！",
                      MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);
                return ;
            }

            

            MsgBoxWindow.MsgBox("是否同步定位卡考勤记录？",
                    MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OKCancel, (e) =>
                    {
                        if (e == IriskingAttend.Dialog.MsgBoxWindow.MsgResult.OK)
                        {
                            SyncSelectedLocateRecord( true );

                            ////同步按钮失效10秒
                            //IsSyncLocateAttendInfoBtnEnable = false;
                            //Thread th = new Thread(new ParameterizedThreadStart((o) =>
                            //{
                            //    //阻塞线程10秒
                            //    Thread.Sleep(10000);
                            //    ((Dispatcher)o).BeginInvoke(() =>
                            //    {
                            //        IsSyncLocateAttendInfoBtnEnable = true;
                            //    });
                            //}));

                            //th.Start(App.Current.RootVisual.Dispatcher);
                        }
                    });

        }

        #endregion

        #region wcf ria操作

       

        /// <summary>
        /// 获取异常考勤信息
        /// </summary>
        private void GetAbnormalAttendInfosRia()
        {
            List<int> departIds = new List<int>();
            foreach (var item in _cmbDepart.Items)
            {
                CheckBox chkBox = item as CheckBox;
                if (chkBox != null && chkBox.IsChecked.Value)
                {
                    departIds.Add((int)chkBox.Tag);
                }
            }

            if (departIds.Count == 0)
            {
                departIds = VmLogin.OperatorDepartIDList;
            }

            try
            {
                WaitingDialog.ShowWaiting();
                ServiceDomDbAcess.ReOpenSever();

                EntityQuery<AttendRecordInfo_WuhuShan> list = ServiceDomDbAcess.GetSever().GetAbnormalAttendRecordInfo_WuHuShanQuery(BeginTime.Value, EndTime.Value, departIds.ToArray(), PersonName, WorkSn);
                ///回调异常类
                Action<LoadOperation<AttendRecordInfo_WuhuShan>> actionCallBack = new Action<LoadOperation<AttendRecordInfo_WuhuShan>>(ErrorHandle<AttendRecordInfo_WuhuShan>.OnLoadErrorCallBack);
                ///异步事件
                LoadOperation<AttendRecordInfo_WuhuShan> lo = ServiceDomDbAcess.GetSever().Load(list, actionCallBack, null);
                lo.Completed += delegate
                {

                    AttendRecordInfos = new BaseViewModelCollection<AttendRecordInfo_WuhuShan>();
                    _attendRecordInfosSource = new List<AttendRecordInfo_WuhuShan>();
                    
                    //异步获取数据
                    foreach (AttendRecordInfo_WuhuShan ar in lo.Entities)
                    {
                        _attendRecordInfosSource.Add(ar);
                        if (_attendStateFilterList.Contains(ar.attend_state))
                        {
                            AttendRecordInfos.Add(ar);
                        }
                    }

                    this.MarkObj.Selected = CheckIsAllSelected();
                    this.IsBatchOperateBtnEnable = CheckIsAnyOneSelected();


                    WaitingDialog.HideWaiting();
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
        /// 开始同步定位卡考勤信息
        /// </summary>
        public void SyncLocateAttendInfoRia(int[] personIds, DateTime locateBeginTime, DateTime locateEndTime, CompleteCallBack completed)
        {
             WaitingDialog.ShowWaiting("正在同步！");

             Action<InvokeOperation<string>> errorCallBack = CallBackHandleControl<string>.OnInvokeErrorCallBack;
             CallBackHandleControl<string>.m_sendValue = (o) =>
             {
                 WaitingDialog.HideWaiting();
                 if (completed != null)
                 {
                     completed();
                 }               
             };

             ServiceDomDbAcess.GetSever().StartSyncLocateAttendInfo(locateBeginTime, locateEndTime, personIds, errorCallBack, null);
             
        }

        #endregion

        #region 同步定位考勤数据 by cty //////////////////////////////////////////

        /// <summary>
        /// 停止异步命令
        /// </summary>
        private void StopAsyncTaskLocate()
        {
            string info = "是否取消同步定位考勤数据？";
            MsgBoxWindow.MsgBox(
                    info,
                    Dialog.MsgBoxWindow.MsgIcon.Information,
                    Dialog.MsgBoxWindow.MsgBtns.OKCancel, (e) =>
                    {
                        if (e == MsgBoxWindow.MsgResult.OK)
                        {
                            CancelAsyncTaskLocate();
                        }
                    });

        }

        /// <summary>
        /// 停止异步命令,不带取消提示框
        /// </summary>
        public void CancelAsyncTaskLocate()
        {
            if (_runnerTask != null && _runnerTask.Stop)
            {
                _runnerTask.Stop = false;
                IsAsyncTaskLocateVisible = Visibility.Collapsed;
                this.IsSyncLocateAttendInfoBtnEnable = true;
                this.IsBatchOperateBtnEnable = true;
            }
        }

        /// <summary>
        /// 同步定位卡考勤数据 获得所有已注册人员的id by cty
        /// </summary>
        public void GetAllPersonids(DateTime locateBeginTime, DateTime locateEndTime, bool bShowResultDialog, CompleteCallBack completed)
        {
            WaitingDialog.ShowWaiting();
            ServiceDomDbAcess.ReOpenSever();

            EntityQuery<UserPersonInfo> list = ServiceDomDbAcess.GetSever().GetAllPersonIdsQuery();
            ///回调异常类
            Action<LoadOperation<UserPersonInfo>> actionCallBack = new Action<LoadOperation<UserPersonInfo>>(ErrorHandle<UserPersonInfo>.OnLoadErrorCallBack);
            ///异步事件
            LoadOperation<UserPersonInfo> lo = ServiceDomDbAcess.GetSever().Load(list, actionCallBack, null);
            lo.Completed += delegate
            {

                AttendRecordInfos = new BaseViewModelCollection<AttendRecordInfo_WuhuShan>();
                _attendRecordInfosSource = new List<AttendRecordInfo_WuhuShan>();
                AllPersonIds = new List<int>();
                //异步获取数据
                foreach (UserPersonInfo ar in lo.Entities)
                {
                    AllPersonIds.Add(ar.person_id);
                }
                WaitingDialog.HideWaiting();
                SyncLocateAttendTask(AllPersonIds.ToArray(), locateBeginTime, locateEndTime, bShowResultDialog, completed);

            };
        }

        //by cty
        private void SyncLocateAttendTask(int[] personIds, DateTime locateBeginTime, DateTime locateEndTime, bool bShowResultDialog, CompleteCallBack completed)
        {
            //构建异步任务执行器
            if (_runnerTask != null)
            {
                _runnerTask.Stop = false;
                _runnerTask = null;
            }

            this.IsSyncLocateAttendInfoBtnEnable = false;
            this.IsBatchOperateBtnEnable = false;

            //开始同步数据前 的工作
            _asyncRusltAddRecord = 0;
            IsAsyncTaskLocateVisible = Visibility.Visible;
            AsyncTaskLocateProgress = 0;

            _runnerTask = new AsyncActionRunner(GetSyncLocateAttendTasks(personIds.ToArray(), 20, locateBeginTime, locateEndTime));

            //下面是运行完成后的处理
            _runnerTask.Completed += (obj, a) =>
            {
                IsAsyncTaskLocateVisible = Visibility.Collapsed;

                //如果取消了批量添加识别记录的操作，则不弹出对话框
                if (!_runnerTask.Stop)
                {
                    _runnerTask = null;
                    VmOperatorLog.InsertOperatorLog(1, "取消同步定位考勤数据", "", () =>
                    {
                        if (completed != null)
                        {
                            completed();
                        }                      
                    });
                    return;
                }
                _runnerTask = null;
                if (_asyncRusltAddRecord == 0)
                {
                    //添加操作员日志
                    VmOperatorLog.InsertOperatorLog(1, "同步定位卡考勤记录", "", () =>
                    {
                        if (completed != null)
                        {
                            completed();
                        }     
                        if (bShowResultDialog)
                        {
                            MsgBoxWindow.MsgBox("同步定位卡考勤记录成功！", MsgBoxWindow.MsgIcon.Succeed, MsgBoxWindow.MsgBtns.OK);
                        }
                    });
                }
                else
                {
                    //添加操作员日志
                    VmOperatorLog.InsertOperatorLog(0, "同步定位卡考勤记录", "", () =>
                    {
                        if (completed != null)
                        {
                            completed();
                        }     
                        if (bShowResultDialog)
                        {
                            MsgBoxWindow.MsgBox("同步定位卡考勤记录失败！", MsgBoxWindow.MsgIcon.Error, MsgBoxWindow.MsgBtns.OK);
                        }
                    });
                }
                this.IsSyncLocateAttendInfoBtnEnable = true;
                this.IsBatchOperateBtnEnable = true;
            };

            //通知进度条改变
            _runnerTask.ProgressChanged += (obj, e) =>
            {
                AsyncTaskLocateProgress = e.Percent * 100;
            };


            //执行异步任务队列
            _runnerTask.Execute();
        }

        //构建异步任务队列 by cty 
        private List<IAsyncAction> GetSyncLocateAttendTasks(int[] personIds, int countPerTask, DateTime locateBeginTime, DateTime locateEndTime)
        {
            List<IAsyncAction> tasks = new List<IAsyncAction>();

            for (int i = 0; i < personIds.Length; i += countPerTask)
            {
                int[] curPersonIds;

                //构造一次任务的人数
                if (i + countPerTask - 1 < personIds.Length)
                {
                    curPersonIds = new int[countPerTask];
                    for (int j = 0; j < countPerTask; j++)
                    {
                        curPersonIds[j] = personIds[i + j];
                    }
                }
                else
                {
                    curPersonIds = new int[personIds.Length - i];
                    for (int j = 0; j < curPersonIds.Length; j++)
                    {
                        curPersonIds[j] = personIds[i + j];
                    }
                }


                ///创建新任务
                var taskTemp = new AsyncAction("BatchAddRecTask" + i);

                //设置任务工作内容
                taskTemp.SetAction(() =>
                {

                    Action<InvokeOperation<string>> errorCallBack = CallBackHandleControl<string>.OnInvokeErrorCallBack;
                    CallBackHandleControl<string>.m_sendValue = (o) =>
                    {
                        if (o.CompareTo("") == 0)
                        {

                                _asyncRusltAddRecord = 0;

                        }
                        taskTemp.OnCompleted();
                        //else
                        //{
                        //    IsSyncLocateAttendInfoBtnEnable = true;
                        //    Dialog.MsgBoxWindow.MsgBox("向后台发送请求失败！",
                        //        Dialog.MsgBoxWindow.MsgIcon.Error, Dialog.MsgBoxWindow.MsgBtns.OK);
                        //     VmOperatorLog.InsertOperatorLog(0, "同步定位卡考勤记录", o, null);
                        //     WaitingDialog.HideWaiting();
                        //}
               
                    };
                    ServiceDomDbAcess.GetSever().StartSyncLocateAttendInfo(locateBeginTime, locateEndTime, curPersonIds, errorCallBack, null);
                }, true);

                tasks.Add(taskTemp);

            }

            return tasks;
        }


        #endregion

        #region 给view层提供的接口函数

        /// <summary>
        /// 设置部门过滤的combobox
        /// </summary>
        /// <param name="comboBox"></param>
        public void SetDeparts(ComboBox comboBox)
        {
            _cmbDepart = comboBox;
        }

        /// <summary>
        /// 异常考勤过滤条件
        /// </summary>
        /// <param name="isShowIrisErr"></param>
        /// <param name="isShowLocateErr"></param>
        /// <param name="isShowBothErr"></param>
        /// <param name="isShowTimeErr"></param>
        public void FilterAttendInfo(bool isShowIrisErr, bool isShowLocateErr, bool isShowBothErr, bool isShowTimeErr)
        {
            _attendStateFilterList = new List<int>();
            if (isShowIrisErr)
            {
                _attendStateFilterList.Add(1);
            }
            if (isShowLocateErr)
            {
                _attendStateFilterList.Add(2);
            }
            if (isShowTimeErr)
            {
                _attendStateFilterList.Add(3);
            }
            if (isShowBothErr)
            {
                _attendStateFilterList.Add(4);
            }

            
            AttendRecordInfos = FiterAttendInfo(_attendRecordInfosSource);

        }

        /// <summary>
        /// 选中全部人员或者取消选中
        /// </summary>
        /// <param name="sender">checkBox对象</param>
        public void SelectAll(bool? IsChecked)
        {
            if (IsChecked.Value)
            {
                foreach (var item in AttendRecordInfos)
                {
                    item.isSelected = true;
                }
            }
            else
            {
                foreach (var item in AttendRecordInfos)
                {
                    item.isSelected = false;
                }
            }
            this.IsBatchOperateBtnEnable = CheckIsAnyOneSelected();
            this.MarkObj.Selected = CheckIsAllSelected();
        }

        /// <summary>
        /// 按需选择datagrid的Items
        /// </summary>
        /// <param name="sender">datagrid对象</param>
        public void SelectItems(AttendRecordInfo_WuhuShan info)
        {
            info.isSelected = !info.isSelected;
            this.IsBatchOperateBtnEnable = CheckIsAnyOneSelected();
            this.MarkObj.Selected = CheckIsAllSelected();
        }

      

        
        #endregion

        #region 私有功能函数

        //fiter by  err condition
        private BaseViewModelCollection<AttendRecordInfo_WuhuShan> FiterAttendInfo(List<AttendRecordInfo_WuhuShan> source)
        {
            if (source == null) 
            {
                return null;
            }
            BaseViewModelCollection<AttendRecordInfo_WuhuShan> res = new BaseViewModelCollection<AttendRecordInfo_WuhuShan>();
            foreach (var item in source)
            {
                if (_attendStateFilterList.Contains(item.attend_state))
                {
                    res.Add(item);
                }
            }
            return res;
        }

      
        
        /// <summary>
        /// 检查批量操作按钮的可见性
        /// </summary>
        private bool CheckIsAnyOneSelected()
        {

            foreach (var item in AttendRecordInfos)
            {
                if (item.isSelected)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 检查Item是否全部被选中
        /// </summary>
        private bool CheckIsAllSelected()
        {
            if (AttendRecordInfos.Count == 0)
            {
                return false;
            }
            foreach (var item in AttendRecordInfos)
            {
                if (!item.isSelected)
                {
                    return false;
                }
            }
            
            return true;

        }

        #endregion

    }

  

}
