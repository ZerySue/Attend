/*************************************************************************
** 文件名:   VmChildWndOperateClassOrder.cs
×× 主要类:   VmChildWndOperateClassOrder
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   cty
** 日  期:   2013-6-14
** 修改人:   wz 代码优化
** 日  期:   2013-7-24
** 描  述:   VmChildWndOperateClassOrder类,班次管理
**
** 修改人:   lzc 
** 日  期:   2013-8-8
** 描  述:   班次图示
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
using System.Windows.Media.Imaging;
using System.IO;

using System.ServiceModel.Channels;
using GalaSoft.MvvmLight.Command;
using MvvmLightCommand.SL4.TriggerActions;
using IriskingAttend.Dialog;
using IriskingAttend.BehaviorSelf;
using System.Text;
using IriskingAttend.ViewModel.SystemViewModel;

namespace IriskingAttend.ViewModel.PeopleViewModel
{
    /// <summary>
    /// 班次管理ViewModel
    /// </summary>
    public class VmChildWndOperateClassOrder : BaseViewModel
    {

        #region 字段声明


        /// <summary>
        /// 与服务声明
        /// </summary>
        private DomainServiceIriskingAttend _serviceDomDbAccess = new DomainServiceIriskingAttend();

        /// <summary>
        /// vm加载完毕事件
        /// </summary>
        public event EventHandler LoadCompletedEvent;

        /// <summary>
        /// 窗口关闭事件
        /// </summary>
        public event Action<bool> CloseEvent;

        /// <summary>
        /// 重布局班次信息的Datagrid事件
        /// </summary>
        public event Action<object> LayoutDGClassOrder;

        /// <summary>
        /// 子窗口的操作模式
        /// </summary>
        private ChildWndOptionMode _operateClassOrderMode;

        /// <summary>
        /// 班次信息
        /// </summary>
        private UserClassOrderInfo _classOrderInfo;

        //分时段记工变量
        //private List<short> _availTimes;  //记工工时
        //private List<int> _workCnts;      //记工工数

        private int _preClassTypeId = 0;  //该班次预计所属的部门

        //是否进行了继续添加操作
        private bool _isContinueAddExcuted = false;

        #endregion

        #region 与页面绑定的属性
        //命令
        public DelegateCommand OkBtnCmd { get; set; }
        public DelegateCommand CancelBtnCmd { get; set; }
        public DelegateCommand ContinueBtnCmd { get; set; }
        public DelegateCommand AddWorkCntCmd { get; set; }


        /// <summary>
        /// 分时段记工时长和记工工数
        /// </summary>
        public BaseViewModelCollection<WorkCntData> WorkCntDatas { get; set; }
 

        private Visibility _standardWorkCntsVisibility;

        /// <summary>
        /// 标准记工可见性 by cty
        /// </summary>
        public Visibility StandardWorkCntsVisibility
        {
            get { return _standardWorkCntsVisibility; }
            set
            {
                _standardWorkCntsVisibility = value;
                this.OnPropertyChanged<Visibility>(()=>this.StandardWorkCntsVisibility);
            }
        }


        private Visibility _addWorkCntVisibility;
        /// <summary>
        /// 添加工数按钮可见性
        /// </summary>
        public Visibility AddWorkCntVisibility
        {
            get { return _addWorkCntVisibility; }
            set
            {
                _addWorkCntVisibility = value;
                this.OnPropertyChanged<Visibility>(() => this.AddWorkCntVisibility);
            }
        }


        private string _okBtnContent;
        /// <summary>
        /// okBtn 内容
        /// </summary>
        public string OkBtnContent
        {
            get { return _okBtnContent; }
            set
            {
                _okBtnContent = value;
                this.OnPropertyChanged<string>(() => this.OkBtnContent);
            }
        }


        private string _cancelBtnContent;
        /// <summary>
        /// 取消按钮的内容 by cty
        /// </summary>
        public string CancelBtnContent
        {
            get { return _cancelBtnContent; }
            set
            {
                _cancelBtnContent = value;
                this.OnPropertyChanged<string>(() => this.CancelBtnContent);
            }
        }


        private string _title;
        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                this.OnPropertyChanged<string>(() => this.Title);
            }
        }

        private string _classOrderName;
        /// <summary>
        /// 班次名称
        /// </summary>
        public string ClassOrderName
        {
            get { return _classOrderName; }
            set
            {
                _classOrderName = value;
                this.OnPropertyChanged<string>(() => this.ClassOrderName);
            }
        }


        private string _attendSign;
        /// <summary>
        /// 班次简称
        /// </summary>
        public string AttendSign
        {
            get { return _attendSign; }
            set
            {
                _attendSign = value;
                this.OnPropertyChanged<string>(() => this.AttendSign);
            }
        }

        private List<string> _classTypeNames;
        /// <summary>
        /// 班制列表
        /// </summary>
        public List<string> ClassTypeNames
        {
            get { return _classTypeNames; }
            set
            {
                _classTypeNames = value;
                this.OnPropertyChanged<List<string>>(() => this.ClassTypeNames);
            }
        }

        private List<int> _classTypeIds;


        private int _classTypeNamesSelectedIndex;
        /// <summary>
        /// 班制列表选择Index
        /// </summary>
        public int ClassTypeNamesSelectedIndex
        {
            get { return _classTypeNamesSelectedIndex; }
            set
            {
                _classTypeNamesSelectedIndex = value;
                this.OnPropertyChanged<int>(() => this.ClassTypeNamesSelectedIndex);
            }
        }

        private int _attendOffMinutesSelectedIndex;
        /// <summary>
        /// 考勤归属日
        /// </summary>
        public int AttendOffMinutesSelectedIndex
        {
            get { return _attendOffMinutesSelectedIndex; }
            set
            {
                _attendOffMinutesSelectedIndex = value;
                this.OnPropertyChanged<int>(() => this.AttendOffMinutesSelectedIndex);
            }
        }

        private object _inWellStartTime;
        /// <summary>
        /// 上班考勤开始时间
        /// </summary>
        public object InWellStartTime
        {
            get { return _inWellStartTime; }
            set
            {
                _inWellStartTime = value;
                this.NotifyPropertyChanged("InWellStartTime");
                //图示-lzc
                OnStartTimeChart.Updata(DateTimeToMinute(_inWellStartTime, InWellStartTimeDayIndex),
                    InWellStartTimeDayIndex, "上班开始时间：" + CalStringByDayIndex(InWellStartTimeDayIndex)
                    + CalTimeByMinutes(DateTimeToMinute(_inWellStartTime)), DateTimeToMinute(_inWellStartTime));
                OnPropertyChanged(() => OnStartTimeChart);
            }
        }

        private int _inWellStartTimeDayIndex;

        /// <summary>
        /// 上班考勤开始时间归属日
        /// </summary>
        public int InWellStartTimeDayIndex
        {
            get { return _inWellStartTimeDayIndex; }
            set
            {
                _inWellStartTimeDayIndex = value;
                OnPropertyChanged(() => InWellStartTimeDayIndex);
                //图示-lzc
                OnStartTimeChart.Updata(DateTimeToMinute(_inWellStartTime, InWellStartTimeDayIndex),
                InWellStartTimeDayIndex, "上班开始时间：" + CalStringByDayIndex(InWellStartTimeDayIndex)
                 + CalTimeByMinutes(DateTimeToMinute(_inWellStartTime)), DateTimeToMinute(_inWellStartTime));
                OnPropertyChanged(() => OnStartTimeChart);
            }
        }


        private object _inWellEndTime;
        /// <summary>
        /// 上班考勤结束时间
        /// </summary>
        public object InWellEndTime
        {
            get { return _inWellEndTime; }
            set
            {
                _inWellEndTime = value;
                this.NotifyPropertyChanged("InWellEndTime");
                //图示-lzc
                OnEndTimeChart.Updata(DateTimeToMinute(_inWellEndTime, _inWellEndTimeDayIndex),
                _inWellEndTimeDayIndex, "上班结束时间：" + CalStringByDayIndex(_inWellEndTimeDayIndex)
                 + CalTimeByMinutes(DateTimeToMinute(_inWellEndTime)), DateTimeToMinute(_inWellEndTime));
                OnPropertyChanged(() => OnEndTimeChart);
            }
        }

        private int _inWellEndTimeDayIndex;
        /// <summary>
        /// 上班考勤结束时间归属日
        /// </summary>
        public int InWellEndTimeDayIndex
        {
            get { return _inWellEndTimeDayIndex; }
            set
            {
                _inWellEndTimeDayIndex = value;
                OnPropertyChanged(() => InWellEndTimeDayIndex);
                //图示-lzc
                OnEndTimeChart.Updata(DateTimeToMinute(_inWellEndTime, _inWellEndTimeDayIndex),
                _inWellEndTimeDayIndex, "上班结束时间：" + CalStringByDayIndex(_inWellEndTimeDayIndex)
                 + CalTimeByMinutes(DateTimeToMinute(_inWellEndTime)), DateTimeToMinute(_inWellEndTime));
                OnPropertyChanged(() => OnEndTimeChart);
            }
        }


        private object _outWellStartTime;
        /// <summary>
        /// 下班考勤开始时间
        /// </summary>
        public object OutWellStartTime
        {
            get { return _outWellStartTime; }
            set
            {
                _outWellStartTime = value;
                OnPropertyChanged(() => OutWellStartTime);

                //图示-lzc
                OffStartTimeChart.Updata(DateTimeToMinute(_outWellStartTime, _outWellStartTimeDayIndex),
                _outWellStartTimeDayIndex, "下班开始时间：" + CalStringByDayIndex(_outWellStartTimeDayIndex)
                 + CalTimeByMinutes(DateTimeToMinute(_outWellStartTime)), DateTimeToMinute(_outWellStartTime));
                OnPropertyChanged(() => OffStartTimeChart);
            }
        }

        private int _outWellStartTimeDayIndex;
        /// <summary>
        /// 下班考勤开始时间归属日
        /// </summary>
        public int OutWellStartTimeDayIndex
        {
            get { return _outWellStartTimeDayIndex; }
            set
            {
                _outWellStartTimeDayIndex = value;
                OnPropertyChanged(() => OutWellStartTimeDayIndex);

                //图示-lzc
                OffStartTimeChart.Updata(DateTimeToMinute(_outWellStartTime, _outWellStartTimeDayIndex),
                _outWellStartTimeDayIndex, "下班开始时间：" + CalStringByDayIndex(_outWellStartTimeDayIndex)
                 + CalTimeByMinutes(DateTimeToMinute(_outWellStartTime)), DateTimeToMinute(_outWellStartTime));
                OnPropertyChanged(() => OffStartTimeChart);
            }
        }

        private object _outWellEndTime;
        /// <summary>
        /// 下班考勤结束时间
        /// </summary>
        public object OutWellEndTime
        {
            get { return _outWellEndTime; }
            set
            {
                _outWellEndTime = value;
                OnPropertyChanged(() => OutWellEndTime);

                //图示-lzc
                OffEndTimeChart.Updata(DateTimeToMinute(_outWellEndTime, _outWellEndTimeDayIndex),
                _outWellEndTimeDayIndex, "下班结束时间：" + CalStringByDayIndex(_outWellEndTimeDayIndex)
                 + CalTimeByMinutes(DateTimeToMinute(_outWellEndTime)), DateTimeToMinute(_outWellEndTime));
                OnPropertyChanged(() => OffEndTimeChart);
            }
        }

        private int _outWellEndTimeDayIndex;
        /// <summary>
        /// 下班考勤结束时间归属日
        /// </summary>
        public int OutWellEndTimeDayIndex
        {
            get { return _outWellEndTimeDayIndex; }
            set
            {
                _outWellEndTimeDayIndex = value;
                OnPropertyChanged(() => OutWellEndTimeDayIndex);

                //图示-lzc
                OffEndTimeChart.Updata(DateTimeToMinute(_outWellEndTime, _outWellEndTimeDayIndex),
                _outWellEndTimeDayIndex, "下班结束时间：" + CalStringByDayIndex(_outWellEndTimeDayIndex)
                 + CalTimeByMinutes(DateTimeToMinute(_outWellEndTime)), DateTimeToMinute(_outWellEndTime));
                OnPropertyChanged(() => OffEndTimeChart);
            }
        }

        #region lzc增加班次图示绑定

        private ClassOrderChart _onStartTimeChart ;
        /// <summary>
        /// 上班开始时间
        /// </summary>
        public ClassOrderChart OnStartTimeChart
        {
            get
            {
                if (_onStartTimeChart == null)
                    _onStartTimeChart = new ClassOrderChart();
                return _onStartTimeChart;
            }

            set
            {
                if(_onStartTimeChart != value)
                    _onStartTimeChart = value;
                OnPropertyChanged(() => OnStartTimeChart);
            }
        }

        private ClassOrderChart _onEndTimeChart;
        /// <summary>
        /// 上班结束时间
        /// </summary>
        public ClassOrderChart OnEndTimeChart
        {
            get
            {
                if (_onEndTimeChart == null)
                    _onEndTimeChart = new ClassOrderChart();
                return _onEndTimeChart;
            }

            set
            {
                if (_onEndTimeChart != value)
                    _onEndTimeChart = value;
                OnPropertyChanged(() => OnEndTimeChart);
            }
        }

        private ClassOrderChart _offStartTimeChart;
        /// <summary>
        /// 下班开始时间
        /// </summary>
        public ClassOrderChart OffStartTimeChart
        {
            get
            {
                if (_offStartTimeChart == null)
                    _offStartTimeChart = new ClassOrderChart();
                return _offStartTimeChart;
            }

            set
            {
                if (_offStartTimeChart != value)
                    _offStartTimeChart = value;
                OnPropertyChanged(() => OffStartTimeChart);
            }
        }

        private ClassOrderChart _offEndTimeChart;
        /// <summary>
        /// 下班结束时间
        /// </summary>
        public ClassOrderChart OffEndTimeChart
        {
            get
            {
                if (_offEndTimeChart == null)
                    _offEndTimeChart = new ClassOrderChart();
                return _offEndTimeChart;
            }

            set
            {
                if (_offEndTimeChart != value)
                    _offEndTimeChart = value;
                OnPropertyChanged(() => OffEndTimeChart);
            }
        }

        #endregion



        #region 最晚下班时间

        private bool _attendLatestWorktimeValid;
        /// <summary>
        /// 最晚下班时间是否有效
        /// </summary>
        public bool AttendLatestWorktimeValid
        {
            get { return _attendLatestWorktimeValid; }
            set
            {
                _attendLatestWorktimeValid = value;
                if (value)
                {
                    AttendLatestWorktimeVisibility = Visibility.Visible;
                }
                else
                {
                    AttendLatestWorktimeVisibility = Visibility.Collapsed;
                }
                OnPropertyChanged(() => AttendLatestWorktimeValid);
            }
        }

        private Visibility _attendLatestWorktimeVisibility;
        /// <summary>
        /// 最晚下班时可见性
        /// </summary>
        public Visibility AttendLatestWorktimeVisibility
        {
            get { return _attendLatestWorktimeVisibility; }
            set
            {
                _attendLatestWorktimeVisibility = value;
                OnPropertyChanged(() => AttendLatestWorktimeVisibility);
            }
        }

        private object _attendLatestWorktime;
        /// <summary>
        /// 最晚下班时间
        /// </summary>
        public object AttendLatestWorktime
        {
            get { return _attendLatestWorktime; }
            set
            {
                _attendLatestWorktime = value;
                OnPropertyChanged(() => AttendLatestWorktime);
            }
        }

        private int _attendLatestWorktimeDayIndex;

        /// <summary>
        /// 最晚下班时间归属日
        /// </summary>
        public int AttendLatestWorktimeDayIndex
        {
            get { return _attendLatestWorktimeDayIndex; }
            set
            {
                _attendLatestWorktimeDayIndex = value;
                OnPropertyChanged(() => AttendLatestWorktimeDayIndex);
            }
        }

        #endregion

        #region 最大在岗时长
        private bool _attendMaxMinutesValid;
        /// <summary>
        /// 最大在岗时长是否有效
        /// </summary>
        public bool AttendMaxMinutesValid
        {
            get { return _attendMaxMinutesValid; }
            set
            {
                _attendMaxMinutesValid = value;
                if (value)
                {
                    AttendMaxMinutesVisibility = Visibility.Visible;
                }
                else
                {
                    AttendMaxMinutes = ""; //最大在岗时长无效时
                    AttendMaxMinutesVisibility = Visibility.Collapsed;
                }
                OnPropertyChanged(() => AttendMaxMinutesValid);
            }
        }

        private Visibility _attendMaxMinutesVisibility;

        /// <summary>
        /// 最大在岗时长可见性
        /// </summary>
        public Visibility AttendMaxMinutesVisibility
        {
            get { return _attendMaxMinutesVisibility; }
            set
            {
                _attendMaxMinutesVisibility = value;
                OnPropertyChanged(() => AttendMaxMinutesVisibility);
            }
        }

        private string _attendMaxMinutes;
        /// <summary>
        /// 最大在岗时长
        /// </summary>
        public string AttendMaxMinutes
        {
            get { return _attendMaxMinutes; }
            set
            {
                _attendMaxMinutes = value;
                OnPropertyChanged(() => AttendMaxMinutes);
            }
        }



        #endregion

        #region 计算工数

        private int _workCntMethodIndex;
        /// <summary>
        /// 哪种记工方式， 0 标准，1 分时段记工
        /// </summary>
        public int WorkCntMethodIndex
        {
            get { return _workCntMethodIndex; }
            set
            {
                if (value == 0)
                {
                    IsWorkCntMethodStandard = true;//如果是选择的是标准记工，则添加按钮不可见by cty
                }
                else
                {
                    IsWorkCntMethodStandard = false;//如果是选择的是分时间段记工，则添加按钮可见by cty
                }
                CheckVisibility();
                _workCntMethodIndex = value;
                OnPropertyChanged(() => WorkCntMethodIndex);
            }
        }

        private bool _isWorkCntMethodStandard;

        public bool IsWorkCntMethodStandard
        {
            get { return _isWorkCntMethodStandard; }
            set
            {
                _isWorkCntMethodStandard = value;
                OnPropertyChanged(() => IsWorkCntMethodStandard);
            }
        }

        private DateTime _availTime;
        /// <summary>
        /// 标准记工时间长度
        /// </summary>
        public DateTime AvailTime
        {
            get { return _availTime; }
            set
            {
                _availTime = value;
                OnPropertyChanged(() => AvailTime);
            }
        }


 


        private double _workCnt;
        /// <summary>
        /// 标准记工工数
        /// </summary>
        public double WorkCnt
        {
            get { return _workCnt; }
            set
            {
                _workCnt = value;
                OnPropertyChanged(() => WorkCnt);
            }
        }


     
        #endregion

        private string _memo;

        public string Memo
        {
            get { return _memo; }
            set
            {
                _memo = value;
                OnPropertyChanged(() => Memo);
            }
        }

        private bool _isReadOnly;

        public bool IsReadOnly
        {
            get { return _isReadOnly; }
            set
            {
                IsEditable = !value;
                _isReadOnly = value;
                OnPropertyChanged(() => IsReadOnly);
            }
        }

        private Visibility _continueBtnVisibility;
        /// <summary>
        /// 是否显示继续添加按钮
        /// </summary>
        public Visibility ContinueBtnVisibility
        {
            get { return _continueBtnVisibility; }
            set
            {
                _continueBtnVisibility = value;
                OnPropertyChanged(() => ContinueBtnVisibility);
            }
        }

        private bool _isEditable;

        public bool IsEditable
        {
            get { return _isEditable; }
            set
            {
                _isEditable = value;
                OnPropertyChanged(() => IsEditable);
            }
        }

        private Visibility _okButtonVisibility;
        /// <summary>
        /// ok按钮可见性
        /// </summary>
        public Visibility OkButtonVisibility
        {
            get { return _okButtonVisibility; }
            set
            {
                _okButtonVisibility = value;
                OnPropertyChanged(() => OkButtonVisibility);
            }
        }


        #endregion

        #region 构造函数

        public VmChildWndOperateClassOrder(UserClassOrderInfo cInfo, ChildWndOptionMode mode, int preClassTypeId = 0)
        {
            _preClassTypeId = preClassTypeId;
            _operateClassOrderMode = mode;
            WorkCntDatas = new BaseViewModelCollection<WorkCntData>();
            //_availTimes = new List<short>();
            //_workCnts = new List<int>();
            _classOrderInfo = cInfo;
            this.AttendLatestWorktimeValid = false;
            this.AttendMaxMinutesValid = false;
            WorkCntMethodIndex = 0; //默认为标准记工方式

            GetContent(mode);     //获取显示内容
            GetClassTypeNames();  //获得班制名称


          

            //GetWorkCntContent();

            OkBtnCmd = new DelegateCommand(new Action(OkBtnClicked));
            CancelBtnCmd = new DelegateCommand(new Action(CancelBtnClicked));
            ContinueBtnCmd = new DelegateCommand(new Action(ContinueBtnClicked));
            AddWorkCntCmd = new DelegateCommand(new Action(AddWorkCntClicked));
            CheckVisibility();
        }

        #endregion

        #region 私有功能函数

        /// <summary>
        /// 检查上班开始时间、上班结束时间
        /// 下班开始时间和下班结束时间的合法性
        /// </summary>
        /// <param name="inWellStartTime"></param>
        /// <param name="inWellEndTime"></param>
        /// <param name="outWellStartTime"></param>
        /// <param name="outWellEndTime"></param>
        /// <returns></returns>
        private bool CheckTimeReasonable(int inWellStartTime,int inWellEndTime,
            int outWellStartTime, int outWellEndTime)
        {
            if (inWellStartTime < inWellEndTime &&
                inWellEndTime < outWellStartTime &&
                outWellStartTime < outWellEndTime)
            {
                return true;
            }
            return false;

        }


        /// <summary>
        /// 检查记工工时和工数相关按钮可见性
        /// </summary>
        private void CheckVisibility()
        {
            if (this.IsWorkCntMethodStandard)
            {
                AddWorkCntVisibility = Visibility.Collapsed;
                StandardWorkCntsVisibility = Visibility.Visible;
            }
            else
            {
                AddWorkCntVisibility = Visibility.Visible;
                StandardWorkCntsVisibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 将整型的分钟数装换为整型的日期索引
        /// 0为当天 1为次日 3为第三日
        /// </summary>
        /// <param name="minutes"></param>
        /// <returns></returns>
        private int CalDayByMinutes(int minutes)
        {
            int res = minutes / 1440;
            if (res > 3)
            {
                return -1;
            }
                
            return res;
        }

        /// <summary>
        /// 将索引转换为String类型
        /// 0为当天 1为次日 3为第三日
        /// </summary>
        /// <param name="minutes"></param>
        /// <returns></returns>
        private string  CalStringByDayIndex(int dayIndex)
        {
            switch (dayIndex)
            {
                case 0:
                    return "当天:";
                case 1:
                    return "第二天:";
                case 2:
                    return "第三天:";
                default :
                    return "";
            }
        }

        /// <summary>
        /// 计算分钟数对应的日期表示式字符串
        /// 如：
        /// [60 -> 当日 01:00]
        /// [1440 ->次日 00:00]
        /// [121 -> 当日 02:01]
        /// 如果isAdd为false，则
        /// [60 ->  01:00]
        /// </summary>
        /// <param name="minutes"></param>
        /// <param name="isAdd"></param>
        /// <returns></returns>
        private string MinutesToDate(int minutes, bool isAdd = true)
        {
            if (minutes < 0) return null;
            string day = "";
            string time = "";
            int hour;
            int minute;
            if (minutes < 1440)
            {
                day = "当日";
                hour = minutes / 60;
                minute = minutes - hour * 60;

            }
            else if (minutes < 2880)
            {
                minutes = minutes - 1440;
                day = "次日";
                hour = minutes / 60;
                minute = minutes - hour * 60;

            }
            else if (minutes < 4320)
            {
                minutes = minutes - 2880;
                day = "第三日";
                hour = minutes / 60;
                minute = minutes - hour * 60;

            }
            else
            {
                day = "第" + (minutes / 1440 + 1).ToString() + "日";
                minutes = minutes % 1440;
                hour = minutes / 60;
                minute = minutes - hour * 60;

            }
            time = hour.ToString().PadLeft(2, '0') + ":" + minute.ToString().PadLeft(2, '0');
            if (isAdd)
            {
                return day + " " + time;
            }
            else
            {
                return time;
            }

        }

        /// <summary>
        /// 将整型的分钟数装换为字符串类型的时间
        /// 例如 60 -> 01:00
        ///      1440 ->00:00
        /// </summary>
        /// <param name="minutes">整型的分钟数</param>
        /// <returns>字符串类型的时间</returns>
        private string CalTimeByMinutes(int minutes)
        {
            minutes = minutes % 1440;
            int hour = minutes / 60;
            int minute = minutes - hour * 60;
            return hour.ToString().PadLeft(2, '0') + ":" + minute.ToString().PadLeft(2, '0');
        }

        /// <summary>
        /// 整型的分钟数装换为DateTime类型
        /// </summary>
        /// <param name="minutes"></param>
        /// <returns></returns>
        private object MinutesToDateTime(int minutes)
        {
            if (minutes < 0)
            {
                return null;
            }
            DateTime res = DateTime.Parse(CalTimeByMinutes(minutes));
            return res;
        }

        /// <summary>
        /// 将时间表示从DateTime装换到整型的分钟数
        /// </summary>
        /// <param name="dateTime_t"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        private int DateTimeToMinute(object dateTime_t, int day = 0)
        {
            if (dateTime_t == null)
            {
                return -1;
            }
            if (!dateTime_t.GetType().Equals(typeof(DateTime)))
            {
                return -1;
            }

            DateTime dateTime = (DateTime)dateTime_t;
            if (dateTime != null)
            {
                return dateTime.Hour * 60 + dateTime.Minute + day * 1440;
            }
            return -1;
        }

        /// <summary>
        /// 将字符串转换成分钟 by cty
        /// 如： 01:00 -> 60
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private int StingToMinutes(string str)
        {
            try
            {
                int h1 = Convert.ToInt16(str.Substring(0, 2));
                int h2 = Convert.ToInt16(str.Substring(3, 2));
                return (h1 * 60 + h2);
            }
            catch (System.Exception ex)
            {
                WaitingDialog.HideWaiting();
                ErrorWindow err = new ErrorWindow(ex);
                err.Show();
            }
            return 0;
            
        }

        /// <summary>
        /// 计算考勤归属日偏移时间
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        private int GetAttendOffMinutes(int day)
        {
            return day * 1440 + 1;
        }

        /// <summary>
        /// 判断上班开始时间是否晚于上班截止时间，下班开始时间是否晚于下班截止时间
        /// </summary>
        /// <param name="In_start_time">上班开始时间</param>
        /// <param name="In_end_time">上班截止时间</param>
        /// <param name="Out_start_time">下班开始时间</param>
        /// <param name="Out_end_time">下班截止时间 </param>
        /// <returns></returns>
        private bool CheckInTimeAndOutTimeIsReasonable(int inStartTime, int inEndTime, int outStartTime, int outEndTime)
        {
            if (inEndTime>=0 && inStartTime > inEndTime)
            {
                MsgBoxWindow.MsgBox(
                            "请确定您的【上班考勤起始时间】早于【上班考勤结束时间】！",
                            MsgBoxWindow.MsgIcon.Information,
                            MsgBoxWindow.MsgBtns.OK);
                return false;
            }
            else if (outEndTime >= 0 && outStartTime > outEndTime)
            {
                MsgBoxWindow.MsgBox(
                        "请确定您的【下班考勤起始时间】早于【下班考勤结束时间】！",
                        MsgBoxWindow.MsgIcon.Information,
                        MsgBoxWindow.MsgBtns.OK);
                return false;
            }
            else 
            {
                int inTime = inStartTime >= 0 ? inStartTime : inEndTime;
                int outTime = outStartTime >= 0 ? outStartTime : outEndTime;
                if (outTime >= 0 && inTime > outTime)
                {
                      MsgBoxWindow.MsgBox(
                           "请确定您的【上班时间】早于【下班时间】！",
                           MsgBoxWindow.MsgIcon.Information,
                           MsgBoxWindow.MsgBtns.OK);
                     return false;
                }
                
            }

            return true;

            
        }

        /// <summary>
        /// 根据操作模式确定窗体其中的内容
        /// </summary>
        /// <param name="mode"></param>
        private void GetContent(ChildWndOptionMode mode)
        {
            ContinueBtnVisibility = Visibility.Collapsed;
            switch (mode)
            {
                case ChildWndOptionMode.Modify:
                    IsReadOnly = false;
                    OkBtnContent = "修改";
                    CancelBtnContent = "取消";
                    Title = "修改班次信息";
                    break;
                case ChildWndOptionMode.Delete:
                    IsReadOnly = true;
                    OkBtnContent = "删除";
                    CancelBtnContent = "取消";
                    Title = "删除班次信息";
                    break;
                case ChildWndOptionMode.Check:
                    IsReadOnly = true;
                    OkBtnContent = "确定";
                    CancelBtnContent = "关闭";
                    Title = "查看班次信息";
                    OkButtonVisibility = Visibility.Collapsed;
                    break;
                case ChildWndOptionMode.Add:
                    ContinueBtnVisibility = Visibility.Visible;
                    IsReadOnly = false;
                    OkBtnContent = "添加";
                    CancelBtnContent = "取消";
                    Title = "添加班次信息";
                    try
                    {
                        WorkCntDatas.Add(new WorkCntData()
                        {
                            AvailTime = DateTime.Parse("01:00"),
                            WorkCnt = 1
                        });
                        this.WorkCnt = 1.0;
                        this.AvailTime = Convert.ToDateTime(CalTimeByMinutes(60));
                    }
                    catch (Exception e)
                    {
                        ErrorWindow err = new ErrorWindow(e);
                        err.Show();
                    }
                  
                    break;

            }


            #region     非add模式添加内容

            if (mode != ChildWndOptionMode.Add)
            {
                this.Memo = _classOrderInfo.memo;
                this.ClassOrderName = _classOrderInfo.class_order_name;
                this.AttendSign = _classOrderInfo.attend_sign;
                this.AttendOffMinutesSelectedIndex = CalDayByMinutes(_classOrderInfo.attend_off_minutes);
                
                //上班开始和结束时间
                this.InWellStartTimeDayIndex = CalDayByMinutes(_classOrderInfo.in_well_start_time);
                this.InWellStartTime = MinutesToDateTime(_classOrderInfo.in_well_start_time);
                this.InWellEndTimeDayIndex = CalDayByMinutes(_classOrderInfo.in_well_end_time);
                this.InWellEndTime = MinutesToDateTime(_classOrderInfo.in_well_end_time);
                
                //下班开始和结束时间
                this.OutWellEndTimeDayIndex = CalDayByMinutes(_classOrderInfo.out_well_end_time);
                this.OutWellEndTime = MinutesToDateTime(_classOrderInfo.out_well_end_time);
                this.OutWellStartTimeDayIndex = CalDayByMinutes(_classOrderInfo.out_well_start_time);
                this.OutWellStartTime = MinutesToDateTime(_classOrderInfo.out_well_start_time);
               
                //最晚下班时间
                if (_classOrderInfo.latest_worktime_valid > 0)
                {
                    this.AttendLatestWorktimeValid = true;
                    this.AttendLatestWorktimeDayIndex = CalDayByMinutes(_classOrderInfo.attend_latest_worktime);
                    this.AttendLatestWorktime = MinutesToDateTime(_classOrderInfo.attend_latest_worktime);
                }
                else
                {
                    this.AttendLatestWorktimeValid = false;
                }

                //最大在岗时长
                if (_classOrderInfo.max_minutes_valid > 0)
                {
                    this.AttendMaxMinutesValid = true;
                    this.AttendMaxMinutes = _classOrderInfo.attend_max_minutes.ToString();
                }
                else
                {
                    this.AttendMaxMinutesValid = false;
                }

                //分时段记工
                if (_classOrderInfo.is_count_workcnt_by_timeduration)  //by  cty
                {
                    this.WorkCntMethodIndex = 1;
                    this.WorkCnt = 1.0;

                    this.AvailTime = Convert.ToDateTime(CalTimeByMinutes(60));
                    for (int i = 0; i < _classOrderInfo.avail_time_timeduration_str.Length; i++)
                    {
                        this.WorkCntDatas.Add(new WorkCntData()
                            {
                                AvailTime = (DateTime)MinutesToDateTime(_classOrderInfo.avail_time_timeduration[i]),
                                WorkCnt = _classOrderInfo.work_cnt_timeduration[i] / 10.0,
                            });
                        //this._availTimes.Add(_classOrderInfo.avail_time_timeduration[i]);
                        //this._workCnts.Add(_classOrderInfo.work_cnt_timeduration[i]);
                    }
                }
                else//标准记工方式
                {
                    this.WorkCntMethodIndex = 0;
                    // by cty
                    if ( _classOrderInfo.avail_time_linear != -1 && _classOrderInfo.work_cnt_linear != -1)
                    {
                        this.WorkCnt = _classOrderInfo.work_cnt_linear / 10f;
                        this.AvailTime = Convert.ToDateTime(CalTimeByMinutes(_classOrderInfo.avail_time_linear));
                        WorkCntDatas.Add(new WorkCntData()
                        {
                            AvailTime = DateTime.Parse("01:00"),
                            WorkCnt = 1
                        });
                    }
                }
            }
            #endregion
        }

        /// <summary>
        /// 刷新窗体中部分内容 
        /// </summary>
        private void FreshContent()
        {
            this.ClassOrderName = "";
            this.AttendSign = "";
            this.InWellEndTimeDayIndex = 0;
            this.OutWellEndTimeDayIndex = 0;
            this.InWellStartTimeDayIndex = 0;
            this.OutWellStartTimeDayIndex = 0;
            this.InWellStartTime = DateTime.MinValue;
            this.InWellEndTime = null;
            this.OutWellEndTime = null;
            this.OutWellStartTime = null;
            this.AttendLatestWorktimeValid = false;
            this.AttendMaxMinutesValid = false;
            this.AttendLatestWorktimeDayIndex = 0;
            this.AttendLatestWorktime = null;
            this.AttendMaxMinutes = "";
            this.IsWorkCntMethodStandard = true;
            this.WorkCntMethodIndex = 0;
            this.WorkCntDatas.Clear();
            WorkCntDatas.Add(new WorkCntData()
            {
                AvailTime = DateTime.Parse("01:00"),
                WorkCnt = 1
            });
            this.Memo = "";
           // GetWorkCntContent();
            CheckVisibility();

        }

        /// <summary>
        /// 获取修改操作描述
        /// </summary>
        /// <returns></returns>
        private string GetModfiyDescription(out bool isModify)
        {
            isModify = false;
            StringBuilder description = new StringBuilder("班次名称:" + _classOrderInfo.class_order_name +"；\r\n");
            if (_classOrderInfo.class_order_name != ClassOrderName)
            {
                isModify = true;
                description.Append(string.Format("班次名称:{0}->{1}，", _classOrderInfo.class_order_name, ClassOrderName));
            }
            if (_classOrderInfo.attend_sign != AttendSign)
            {
                isModify = true;
                description.Append(string.Format("班次简称:{0}->{1}，", _classOrderInfo.attend_sign, AttendSign));
            }
            if (_classOrderInfo.class_type_name != this.ClassTypeNames[ClassTypeNamesSelectedIndex])
            {
                isModify = true;
                description.Append(string.Format("所属班制:{0}->{1}，",
                    _classOrderInfo.class_type_name,
                    this.ClassTypeNames[ClassTypeNamesSelectedIndex]));
            }
            int AttendDay = CalDayByMinutes(_classOrderInfo.attend_off_minutes);
            if (AttendDay != AttendOffMinutesSelectedIndex)
            {
                isModify = true;
                description.Append(string.Format("考勤归属日:[上班考勤起始时间]{0}->[上班考勤起始时间]{1}，", CalStringByDayIndex(AttendDay), CalStringByDayIndex(AttendOffMinutesSelectedIndex)));
            }
            // string in_well_start_time_t = (DateTimeToMinute(InWellStartTime, InWellStartTimeDayIndex)).ToString();
            int in_well_start_time_minutes = DateTimeToMinute(InWellStartTime, InWellStartTimeDayIndex);
            if (_classOrderInfo.in_well_start_time != in_well_start_time_minutes)
            {
                isModify = true;
                description.Append(string.Format("上班开始时间:{0}->{1}，", _classOrderInfo.in_well_start_time_str, MinutesToDate(in_well_start_time_minutes)));
            }
            int in_well_end_time_minutes = DateTimeToMinute(InWellEndTime, InWellEndTimeDayIndex);
            if (_classOrderInfo.in_well_end_time != in_well_end_time_minutes)
            {
                isModify = true;
                description.Append(string.Format("上班结束时间:{0}->{1}，", _classOrderInfo.in_well_end_time_str, MinutesToDate(in_well_end_time_minutes)));
            }
            int out_well_start_time_minutes = DateTimeToMinute(OutWellStartTime, OutWellStartTimeDayIndex);
            if (_classOrderInfo.out_well_start_time != out_well_start_time_minutes)
            {
                isModify = true;
                description.Append(string.Format("下班开始时间:{0}->{1}，", _classOrderInfo.out_well_end_time_str, MinutesToDate(out_well_start_time_minutes)));
            }
            int out_well_end_time_minutes = DateTimeToMinute(OutWellEndTime, OutWellEndTimeDayIndex);
            if (_classOrderInfo.out_well_end_time != out_well_end_time_minutes)
            {
                isModify = true;
                description.Append(string.Format("下班结束时间:{0}->{1}，", _classOrderInfo.out_well_end_time_str, MinutesToDate(out_well_end_time_minutes)));
            }

            //最晚下班时间(0是无效值)
            int attendLatestWorkTimeMinutes = DateTimeToMinute(AttendLatestWorktime, AttendLatestWorktimeDayIndex);
            if (attendLatestWorkTimeMinutes == -1) attendLatestWorkTimeMinutes = 0;
            if (_classOrderInfo.attend_latest_worktime != attendLatestWorkTimeMinutes)
            {
                isModify = true;
                description.Append(string.Format("最晚下班时间:{0}->{1}，", _classOrderInfo.attend_latest_worktime_str, MinutesToDate(attendLatestWorkTimeMinutes)));
            }

            //最大在岗时长(0是无效值)
            int attendMaxMinutes = 0;
            string attendMaxMinutesStr = "无效";
            if (int.TryParse(AttendMaxMinutes, out attendMaxMinutes))
            {
                attendMaxMinutesStr = string.Format("{0}分钟", attendMaxMinutes);
            }
            else
            {
                attendMaxMinutes = 0;
            }
            if (_classOrderInfo.attend_max_minutes != attendMaxMinutes )
            {
                isModify = true;
                string lastAttendMaxMinutesStr = _classOrderInfo.attend_max_minutes > 0 ? 
                    string.Format("{0}分钟", _classOrderInfo.attend_max_minutes) : "无效";
                description.Append(string.Format("最大在岗时长:{0}->{1}，",
                    lastAttendMaxMinutesStr,
                    attendMaxMinutesStr));
            }

           
            //标准记工方式WorkCnt
            int workCntInt = (int)(WorkCnt * 10);
            if (_classOrderInfo.work_cnt_linear != workCntInt)
            {
                isModify = true;
                description.Append(string.Format("标准记工工数:{0}->{1}，", _classOrderInfo.work_cnt_linear_str, WorkCnt));
            }
            //标准记工时长
            int availTimeMinutes = DateTimeToMinute(AvailTime, 0);
            if (_classOrderInfo.avail_time_linear != availTimeMinutes)
            {
                isModify = true;
                description.Append(string.Format("标准记工时长:{0}->{1}，", _classOrderInfo.avail_time_linear_str, MinutesToDate(availTimeMinutes, false)));
            }

            //记工方式是否改变（标准记工和分时段记工）
            if (_classOrderInfo.is_count_workcnt_by_timeduration != !IsWorkCntMethodStandard)
            {
                isModify = true;
                if (IsWorkCntMethodStandard)
                {
                    description.Append("记工方式：分时段记工->标准记工，");
                }
                else
                {
                    description.Append("记工方式：标准记工->分时段记工，");
                } 
            }

            description.Remove(description.Length - 1, 1);
            description.Append("；\r\n");
        
           
           
            return description.ToString();
        }

     
        #endregion

        #region 界面命令响应
        private void OkBtnClicked()
        {
            switch (_operateClassOrderMode)
            {
                case ChildWndOptionMode.Modify:
                    ModifyClassOrder();
                    break;
                case ChildWndOptionMode.Delete:
                    DeleteClassOrder();
                    break;
                case ChildWndOptionMode.Check:
                    if (CloseEvent != null)
                    {
                        CloseEvent(false);
                    }
                    break;
                case ChildWndOptionMode.Add:
                    AddClassOrder(false);
                    break;
            }
        }

        private void CancelBtnClicked()
        {
            if (CloseEvent != null)
            {
                CloseEvent(_isContinueAddExcuted);
            }
        }

        private void ContinueBtnClicked()
        {
            if (_operateClassOrderMode == ChildWndOptionMode.Add)
            {
                AddClassOrder(true);
            }
        }


        //添加工数按钮响应事件
        private void AddWorkCntClicked() 
        {
            WorkCntDatas.Add(new WorkCntData()
                {
                    AvailTime = DateTime.Parse("01:00"),
                    WorkCnt = 1,
                });
            CheckVisibility();
            //重布局datagrid
            App.Current.RootVisual.Dispatcher.BeginInvoke(LayoutDGClassOrder, WorkCntDatas[WorkCntDatas.Count - 1]);
        }

        #endregion

        #region 功能扩展，给view层的接口


        //选中单行删除分时段记工的 记工时长和记工工数 by cty
        public void DelelteWorkCntData(WorkCntData workCntData)
        {
            WorkCntDatas.Remove(workCntData);
            CheckVisibility();
        }

        #endregion

        #region wcf ria 操作
        
        /// <summary>
        /// 获取班制名称列表
        /// </summary>
        private void GetClassTypeNames()
        {
            try
            {
                EntityQuery<UserClassTypeInfo> list = _serviceDomDbAccess.GetClassTypeInfosQuery();
                ///回调异常类
                Action<LoadOperation<UserClassTypeInfo>> actionCallBack = ErrorHandle<UserClassTypeInfo>.OnLoadErrorCallBack;
                ///异步事件
                LoadOperation<UserClassTypeInfo> lo = this._serviceDomDbAccess.Load(list, actionCallBack, null);
                lo.Completed += delegate
                {
                    ClassTypeNames = new List<string>();
                    _classTypeIds = new List<int>();

                    //异步获取数据
                    foreach (UserClassTypeInfo ar in lo.Entities)
                    {
                        ClassTypeNames.Add(ar.class_type_name);
                        _classTypeIds.Add(ar.class_type_id);
                    }
                    if (_classOrderInfo != null)
                    {
                        this.ClassTypeNamesSelectedIndex =
                             this.ClassTypeNames.IndexOf(_classOrderInfo.class_type_name);
                    }
                    else
                    {
                        this.ClassTypeNamesSelectedIndex = this._classTypeIds.IndexOf(_preClassTypeId);
                        ClassTypeNamesSelectedIndex = ClassTypeNamesSelectedIndex == -1 ? 0 : ClassTypeNamesSelectedIndex;
                    }

                    if (ClassTypeNames.Count == 0)
                    {
                        ClassTypeNamesSelectedIndex = -1;
                    }

                    if (LoadCompletedEvent != null)
                    {
                        LoadCompletedEvent(this, null);
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
        /// 添加班次
        /// </summary>
        /// <param name="isContinue"></param>
        private void AddClassOrder(bool isContinue)
        {
            //获取操作描述
            string description;
            description = string.Format("班次名称：{0}，所属班制：{1}；\r\n", ClassOrderName, ClassTypeNames[ClassTypeNamesSelectedIndex]);

            if (isContinue)
            {
                _isContinueAddExcuted = true;
            }

            if (ClassOrderName == null || ClassOrderName.Equals(""))
            {
                MsgBoxWindow.MsgBox(
                              "班次名称不能为空！",
                              MsgBoxWindow.MsgIcon.Information,
                              MsgBoxWindow.MsgBtns.OK);
                return;
            }

            if (InWellStartTime == null)
            {
                MsgBoxWindow.MsgBox(
                             "上班开始时间不能为空！",
                             MsgBoxWindow.MsgIcon.Information,
                             MsgBoxWindow.MsgBtns.OK);
                return;
            }


            string class_order_name = PublicMethods.ToString(ClassOrderName);
            string attend_sign = AttendSign;
            string attend_off_minutes = PublicMethods.ToString(GetAttendOffMinutes(AttendOffMinutesSelectedIndex));
            string class_type_id = PublicMethods.ToString(_classTypeIds[ClassTypeNamesSelectedIndex]);
            string in_well_start_time_t = (DateTimeToMinute(InWellStartTime, InWellStartTimeDayIndex)).ToString();
            string in_well_end_time_t = (DateTimeToMinute(InWellEndTime, InWellEndTimeDayIndex)).ToString();
            string out_well_start_time_t = (DateTimeToMinute(OutWellStartTime, OutWellStartTimeDayIndex)).ToString();
            string out_well_end_time_t = (DateTimeToMinute(OutWellEndTime, OutWellEndTimeDayIndex)).ToString();

            //检查时间的合法性
            bool isTimeReasonable = CheckInTimeAndOutTimeIsReasonable(DateTimeToMinute(InWellStartTime, InWellStartTimeDayIndex),
                DateTimeToMinute(InWellEndTime, InWellEndTimeDayIndex),
                DateTimeToMinute(OutWellStartTime, OutWellStartTimeDayIndex),
                DateTimeToMinute(OutWellEndTime, OutWellEndTimeDayIndex));
            if (!isTimeReasonable)
            {
                return;
            }
          
            string attend_latest_worktime_t = "null";
            if (AttendLatestWorktimeValid)
            {
                attend_latest_worktime_t = PublicMethods.ToString(DateTimeToMinute(this.AttendLatestWorktime, AttendLatestWorktimeDayIndex));
            }
            int maxTime_t = -1;
            string attend_max_miutes_t = "null";
            if (this.AttendMaxMinutesValid)
            {
                bool resTry = int.TryParse(this.AttendMaxMinutes, out maxTime_t);
                if (!resTry)
                {
                    MsgBoxWindow.MsgBox(
                        "最大在岗时长应该为数字！",
                        MsgBoxWindow.MsgIcon.Information,
                        MsgBoxWindow.MsgBtns.OK);
                    return;
                }
                attend_max_miutes_t = PublicMethods.ToString(maxTime_t);
            }

            if (!IsWorkCntMethodStandard && WorkCntDatas.Count < 1)
            {
                MsgBoxWindow.MsgBox(
                        "记工时长和记工工数至少要有一项存在！",
                        MsgBoxWindow.MsgIcon.Information,
                        MsgBoxWindow.MsgBtns.OK);
                return;
            }

            string[] Avail_times_str = new string[WorkCntDatas.Count];
            string[] Work_cnts_str = new string[WorkCntDatas.Count];
            int i = 0;
            foreach (WorkCntData tmp in WorkCntDatas)
            {
                Avail_times_str[i] = (DateTimeToMinute(tmp.AvailTime, 0)).ToString();
                Work_cnts_str[i] = ((int)(tmp.WorkCnt*10f)).ToString();
                i++;
            }
            string Avail_time_Stanard = "";
            string workCntStandard = "";
            if (IsWorkCntMethodStandard)//当是标准记工时，判断记工时长，记工工数是否为空，格式是否正确 by cty
            {

                workCntStandard = (this.WorkCnt * 10).ToString();
                if (AvailTime == null)
                {
                    MsgBoxWindow.MsgBox(
                      "标准记工时间不能为空！",
                      MsgBoxWindow.MsgIcon.Information,
                      MsgBoxWindow.MsgBtns.OK);
                    return;
                }
                Avail_time_Stanard = (DateTimeToMinute(AvailTime, 0)).ToString();
            }

            string memo_t = Memo;

            try
            {
                WaitingDialog.ShowWaiting();
                _serviceDomDbAccess = new DomainServiceIriskingAttend();
                EntityQuery<OptionInfo> list = _serviceDomDbAccess.AddClassOrderQuery(class_order_name, attend_sign,
                    class_type_id, attend_off_minutes, in_well_start_time_t, in_well_end_time_t,
                    out_well_start_time_t, out_well_end_time_t, attend_latest_worktime_t,
                    attend_max_miutes_t, IsWorkCntMethodStandard, Avail_times_str, Work_cnts_str, Avail_time_Stanard, workCntStandard, memo_t);

                ///回调异常类
                Action<LoadOperation<OptionInfo>> actionCallBack = new Action<LoadOperation<OptionInfo>>(ErrorHandle<OptionInfo>.OnLoadErrorCallBack);
                ///异步事件
                LoadOperation<OptionInfo> lo = this._serviceDomDbAccess.Load(list, actionCallBack, null);
                lo.Completed += delegate
                {
                    foreach (OptionInfo item in lo.Entities)
                    {
                        VmOperatorLog.CompleteCallBack completeCallBack = () =>
                            {
                                if (!isContinue)
                                {
                                    if (CloseEvent != null)
                                    {
                                        CloseEvent(true);
                                    }
                                }
                            };
                        if (!item.isSuccess)
                        {
                            MsgBoxWindow.MsgBox(
                              item.option_info + "！",
                              MsgBoxWindow.MsgIcon.Error,
                              MsgBoxWindow.MsgBtns.OK);
                            VmOperatorLog.InsertOperatorLog(0, "添加班次", description + item.option_info, null);
                            break;
                        }
                        if (!item.isNotifySuccess)
                        {
                            MsgBoxWindow.MsgBox(
                              item.option_info + "！",
                              MsgBoxWindow.MsgIcon.Warning,
                              MsgBoxWindow.MsgBtns.OK);
                            VmOperatorLog.InsertOperatorLog(1, "添加班次", description + item.option_info, completeCallBack);
                        }
                        else
                        {
                            MsgBoxWindow.MsgBox(
                           item.option_info + "！",
                           MsgBoxWindow.MsgIcon.Succeed,
                           MsgBoxWindow.MsgBtns.OK);
                            VmOperatorLog.InsertOperatorLog(1, "添加班次", description , completeCallBack);
                        }
                        FreshContent();
                        break;
                    }
                   
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
        /// 修改班次
        /// </summary>
        private void ModifyClassOrder()
        {
            

            if (ClassOrderName == null || ClassOrderName.Equals(""))
            {
                MsgBoxWindow.MsgBox(
                              "班次名称不能为空！",
                              MsgBoxWindow.MsgIcon.Information,
                              MsgBoxWindow.MsgBtns.OK);
                return;
            }

            if (InWellStartTime == null)
            {
                MsgBoxWindow.MsgBox(
                             "上班开始时间不能为空！",
                             MsgBoxWindow.MsgIcon.Information,
                             MsgBoxWindow.MsgBtns.OK);
                return;
            }

            string class_order_name = PublicMethods.ToString(ClassOrderName);
            string attend_sign = AttendSign;
            string attend_off_minutes = PublicMethods.ToString(GetAttendOffMinutes(AttendOffMinutesSelectedIndex));
            string class_type_id = PublicMethods.ToString(_classTypeIds[ClassTypeNamesSelectedIndex]);
            string in_well_start_time_t = (DateTimeToMinute(InWellStartTime, InWellStartTimeDayIndex)).ToString();
            string in_well_end_time_t = (DateTimeToMinute(InWellEndTime, InWellEndTimeDayIndex)).ToString();
            string out_well_start_time_t = (DateTimeToMinute(OutWellStartTime, OutWellStartTimeDayIndex)).ToString();
            string out_well_end_time_t = (DateTimeToMinute(OutWellEndTime, OutWellEndTimeDayIndex)).ToString();

            //检查时间的合法性
            bool isTimeReasonable = CheckInTimeAndOutTimeIsReasonable(DateTimeToMinute(InWellStartTime, InWellStartTimeDayIndex),
                DateTimeToMinute(InWellEndTime, InWellEndTimeDayIndex),
                DateTimeToMinute(OutWellStartTime, OutWellStartTimeDayIndex),
                DateTimeToMinute(OutWellEndTime, OutWellEndTimeDayIndex));
            if (!isTimeReasonable)
            {
                return;
            }

            string attend_latest_worktime_t = "null";
            if (AttendLatestWorktimeValid)
            {
                attend_latest_worktime_t = PublicMethods.ToString(DateTimeToMinute(this.AttendLatestWorktime, AttendLatestWorktimeDayIndex));
            }
            int maxTime_t = -1;
            string attend_max_miutes_t = "null";
            if (this.AttendMaxMinutesValid)
            {
                bool resTry = int.TryParse(this.AttendMaxMinutes, out maxTime_t);
                if (!resTry)
                {
                    MsgBoxWindow.MsgBox(
                              "最大在岗时长应该为数字！",
                              MsgBoxWindow.MsgIcon.Information,
                              MsgBoxWindow.MsgBtns.OK);
                    return;
                }
                attend_max_miutes_t = PublicMethods.ToString(maxTime_t);
            }

            if (!IsWorkCntMethodStandard && WorkCntDatas.Count < 1)
            {
                MsgBoxWindow.MsgBox(
                              "记工时长和记工工数至少要有一项存在！",
                              MsgBoxWindow.MsgIcon.Information,
                              MsgBoxWindow.MsgBtns.OK);
                return;
            }

            string[] Avail_times_str = new string[WorkCntDatas.Count];
            string[] Work_cnts_str = new string[WorkCntDatas.Count];
            int i = 0;
            foreach (WorkCntData tmp in WorkCntDatas)
            {
                Avail_times_str[i] = (DateTimeToMinute(tmp.AvailTime, 0)).ToString();
                Work_cnts_str[i] = ((int)(tmp.WorkCnt * 10)).ToString();
                i++;
            }
            string Avail_time_Stanard = "";
            string workCntStandard = "";
            if (IsWorkCntMethodStandard)//当是标准记工时，判断记工时长，记工工数是否为空，格式是否正确 by cty
            {

                workCntStandard = (this.WorkCnt * 10).ToString();
                if (AvailTime == null)
                {
                    MsgBoxWindow.MsgBox(
                              "标准记工时间不能为空！",
                              MsgBoxWindow.MsgIcon.Information,
                              MsgBoxWindow.MsgBtns.OK);
                    return;
                }
                Avail_time_Stanard = (DateTimeToMinute(AvailTime, 0)).ToString();
            }
            string memo_t = Memo;
            string class_order_id = PublicMethods.ToString(_classOrderInfo.class_order_id);

            //获取操作描述
            bool isModify = false;
            string description = GetModfiyDescription(out isModify);
            if (!isModify)
            {
                if (CloseEvent != null)
                {
                    CloseEvent(true);
                }
                return;
            }


            try
            {
                WaitingDialog.ShowWaiting();
                _serviceDomDbAccess = new DomainServiceIriskingAttend();
                EntityQuery<OptionInfo> list = _serviceDomDbAccess.ModifyClassOrderQuery(class_order_id, class_order_name, attend_sign,
                    class_type_id, attend_off_minutes, in_well_start_time_t, in_well_end_time_t,
                    out_well_start_time_t, out_well_end_time_t, attend_latest_worktime_t,
                    attend_max_miutes_t, IsWorkCntMethodStandard, Avail_times_str, Work_cnts_str, Avail_time_Stanard, workCntStandard, memo_t);

                ///回调异常类
                Action<LoadOperation<OptionInfo>> actionCallBack = new Action<LoadOperation<OptionInfo>>(ErrorHandle<OptionInfo>.OnLoadErrorCallBack);
                ///异步事件
                LoadOperation<OptionInfo> lo = this._serviceDomDbAccess.Load(list, actionCallBack, null);
                lo.Completed += delegate
                {
                    foreach (OptionInfo item in lo.Entities)
                    {
                        VmOperatorLog.CompleteCallBack CompleteCallBack = () =>
                            {
                                if (CloseEvent != null)
                                {
                                    CloseEvent(true);
                                }
                            };
                        if (!item.isSuccess)
                        {
                            MsgBoxWindow.MsgBox(
                              item.option_info + "！",
                              MsgBoxWindow.MsgIcon.Error,
                              MsgBoxWindow.MsgBtns.OK);
                            VmOperatorLog.InsertOperatorLog(0, "修改班次", description + item.option_info, CompleteCallBack);
                            break;
                        }
                        else
                        {
                            if (!item.isNotifySuccess)
                            {
                                MsgBoxWindow.MsgBox(
                                 item.option_info + "！",
                                 MsgBoxWindow.MsgIcon.Warning,
                                 MsgBoxWindow.MsgBtns.OK);
                                VmOperatorLog.InsertOperatorLog(1, "修改班次", description + item.option_info, CompleteCallBack);
                            }
                            else
                            {
                                MsgBoxWindow.MsgBox(
                                 item.option_info + "！",
                                 MsgBoxWindow.MsgIcon.Succeed,
                                 MsgBoxWindow.MsgBtns.OK);
                                VmOperatorLog.InsertOperatorLog(1, "修改班次", description , CompleteCallBack);
                            }
                        }
                      
                        break;
                    }
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
        /// 删除班次
        /// </summary>
        private void DeleteClassOrder()
        {
            string class_order_id = PublicMethods.ToString(_classOrderInfo.class_order_id);
            string[] class_order_ids = new string[1] { class_order_id };

            try
            {
                WaitingDialog.ShowWaiting();
                _serviceDomDbAccess = new DomainServiceIriskingAttend();
                EntityQuery<OptionInfo> list = _serviceDomDbAccess.DeleteClassOrderQuery(class_order_ids);

                ///回调异常类
                Action<LoadOperation<OptionInfo>> actionCallBack = new Action<LoadOperation<OptionInfo>>(ErrorHandle<OptionInfo>.OnLoadErrorCallBack);
                ///异步事件
                LoadOperation<OptionInfo> lo = this._serviceDomDbAccess.Load(list, actionCallBack, null);
                lo.Completed += delegate
                {
                    foreach (OptionInfo item in lo.Entities)
                    {
                        if (!item.isSuccess)
                        {
                            MsgBoxWindow.MsgBox(
                              item.option_info + "！",
                              MsgBoxWindow.MsgIcon.Error,
                              MsgBoxWindow.MsgBtns.OK);
                            break;
                        }
                        else
                        {
                            if (!item.isNotifySuccess)
                            {
                                MsgBoxWindow.MsgBox(
                                 item.option_info + "！",
                                 MsgBoxWindow.MsgIcon.Warning,
                                 MsgBoxWindow.MsgBtns.OK);
                            }
                            else
                            {
                                MsgBoxWindow.MsgBox(
                                 item.option_info + "！",
                                 MsgBoxWindow.MsgIcon.Warning,
                                 MsgBoxWindow.MsgBtns.OK);
                            }
                        }
                        if (CloseEvent != null)
                        {
                            CloseEvent(true);
                        }
                        break;
                    }
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


        #endregion

    }

    /// <summary>
    /// 班次图示类
    /// </summary>
    public class ClassOrderChart : Entity
    {
        
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public ClassOrderChart()
        {
            _left = 0;
            _dayIndex = 0;
            _labContext = "";
            _minute = -1;
        }

        private  Visibility _isVisibility = Visibility.Visible;

        /// <summary>
        /// 设置是否显示该图示
        /// </summary>
        public Visibility IsVisibility
        {
            get
            {
                if (_minute == -1)
                {
                    _isVisibility = Visibility.Collapsed;
                }
                else
                {
                    _isVisibility = Visibility.Visible;
                }
                return _isVisibility;
            }

        }

        /// <summary>
        /// 通过字段值构造函数
        /// </summary>
        /// <param name="left"></param>
        /// <param name="dayIndex"></param>
        /// <param name="time"></param>
        /// <param name="lanContext"></param>
        public ClassOrderChart(int left,int dayIndex,string time,string lanContext,int minute)
        {
            _left = left;
            _dayIndex = dayIndex;
            _labContext = lanContext;
            _minute = minute;
        }

        /// <summary>
        /// 更新字段值
        /// </summary>
        /// <param name="left"></param>
        /// <param name="dayIndex"></param>
        /// <param name="time"></param>
        /// <param name="lanContext"></param>
        public void Updata(int left, int dayIndex, string lanContext, int minute)
        {
            _left = left/7;
            _dayIndex = dayIndex;
            _labContext = lanContext;
            _minute = minute;          
        }
        //时间线与左边边距
        private const int _margin = 3;
        //时间线坐标
        private int _left = 0;
        //日期
        private int _dayIndex =  0;
        //显示文本
        private string _labContext = "";
        //分钟数
        private int _minute  =  -1;

        /// <summary>
        /// 文本框位置
        /// </summary>
        public string LabMargin
        {
            get 
            { 
                return string.Format("{0},0,0,0", X); 
            }   
        }

        /// <summary>
        /// 位置X坐标
        /// </summary>
        public int X
        {
            get 
            {
                return _left + _margin;
            }
        }

        /// <summary>
        /// 图示显示颜色
        /// </summary>
        public SolidColorBrush Brush
        {
            get 
            {
                string color = "0xff007F00";//绿色
                if (_dayIndex ==1)
                {
                    color = "0xff00007F";//褐色
                }
                else if (_dayIndex == 2)
                {
                    color = "0xff7F0000";//红色
                }     
                return new SolidColorBrush(mathFun.ReturnColorFromString(color));
            }
        }

        /// <summary>
        /// lab显示文字
        /// </summary>
        public string LabContext
        {
            get
            {
                return _labContext;
            }
        }
    }
    
}
