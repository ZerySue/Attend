/*************************************************************************
** 文件名:   VmChildWndOperateClassOrderSign.cs
×× 主要类:   VmChildWndOperateClassOrderSign
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2014-9-26
** 修改人:  
** 日  期:   
** 描  述:   VmChildWndOperateClassOrderSign类,签到班班次管理
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
    /// 签到班班次管理ViewModel
    /// </summary>
    public class VmChildWndOperateClassOrderSign : BaseViewModel
    {

        #region 字段声明        

        /// <summary>
        /// vm加载完毕事件
        /// </summary>
        public event EventHandler LoadCompletedEvent;

        /// <summary>
        /// 窗口关闭事件
        /// </summary>
        public event Action<bool> CloseEvent;
     

        /// <summary>
        /// 子窗口的操作模式
        /// </summary>
        private ChildWndOptionMode _operateClassOrderMode;

        /// <summary>
        /// 签到班班次信息
        /// </summary>
        private UserClassOrderSignInfo _classOrderSignInfo;    

        //是否进行了继续添加操作
        private bool _isContinueAddExcuted = false;

        #endregion

        #region 与页面绑定的属性
        //命令
        public DelegateCommand OkBtnCmd { get; set; }
        public DelegateCommand CancelBtnCmd { get; set; }
        public DelegateCommand ContinueBtnCmd { get; set; }      

        private BaseViewModelCollection<UserClassTypeInfo> _classTypeInfos;
        /// <summary>
        /// 班制信息列表
        /// </summary>
        public BaseViewModelCollection<UserClassTypeInfo> ClassTypeInfos
        {
            get
            {
                return _classTypeInfos;
            }
            set
            {
                _classTypeInfos = value;
                OnPropertyChanged<BaseViewModelCollection<UserClassTypeInfo>>(() => this.ClassTypeInfos);
            }
        }

        private UserClassTypeInfo _selectedClassType;
        /// <summary>
        /// 当前班制
        /// </summary>
        public UserClassTypeInfo SelectedClassType
        {
            get
            {
                return _selectedClassType;
            }
            set
            {
                _selectedClassType = value;
                OnPropertyChanged(() => this.SelectedClassType);                
            }
        }

        /// <summary>
        /// 签到班时间段
        /// </summary>
        public BaseViewModelCollection<ClassOrderSignSection> SectionDatas { get; set; }         

        private string _classOrderSignName;
        /// <summary>
        /// 签到班班次名称
        /// </summary>
        public string ClassOrderSignName
        {
            get { return _classOrderSignName; }
            set
            {
                _classOrderSignName = value;
                this.OnPropertyChanged<string>(() => this.ClassOrderSignName);
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

        private DateTime _minWorkTime;
        /// <summary>
        /// 记工时间长度
        /// </summary>
        public DateTime MinWorkTime
        {
            get { return _minWorkTime; }
            set
            {
                _minWorkTime = value;
                OnPropertyChanged(() => MinWorkTime);
            }
        }

        private double _workCnt;
        /// <summary>
        /// 记工工数
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

        private int _selectLianBanIndex;
        /// <summary>
        /// 是否连班的选择index
        /// </summary>
        public int SelectLianBanIndex
        {
            get { return _selectLianBanIndex; }
            set
            {
                _selectLianBanIndex = value;
                OnPropertyChanged(() => SelectLianBanIndex);
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
        #endregion

        #region 构造函数

        public VmChildWndOperateClassOrderSign(UserClassOrderSignInfo cInfo, ChildWndOptionMode mode)
        {           
            _operateClassOrderMode = mode;
            SectionDatas = new BaseViewModelCollection<ClassOrderSignSection>();          
            _classOrderSignInfo = cInfo;

            if (_classOrderSignInfo != null)
            {
                for (int index = 0; index < _classOrderSignInfo.section_begin_mins.Length; index++)
                {
                    ClassOrderSignSection temp = new ClassOrderSignSection();
                    temp.SectionBeginMin = _classOrderSignInfo.section_begin_mins[index];
                    temp.SectionBeginMinStr = MinutesToDate(temp.SectionBeginMin, true);
                    temp.SectionEndMin = _classOrderSignInfo.section_end_mins[index];
                    temp.SectionEndMinStr = MinutesToDate(temp.SectionEndMin, true);
                    temp.InCalc = _classOrderSignInfo.in_calcs[index];
                    if (temp.InCalc == 0)
                    {
                        temp.InCalcStr = "否";
                    }

                    SectionDatas.Add(temp);
                }
            }
            

            GetContent(mode);     //获取显示内容                    

            OkBtnCmd = new DelegateCommand(new Action(OkBtnClicked));
            CancelBtnCmd = new DelegateCommand(new Action(CancelBtnClicked));
            ContinueBtnCmd = new DelegateCommand(new Action(ContinueBtnClicked));                  
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
        public string MinutesToDate(int minutes, bool isAdd = true)
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
        public int DateTimeToMinute(object dateTime_t, int day = 0)
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
                    {
                        OkBtnContent = "修改";
                        Title = "修改签到班班次信息";
                    }
                    break;
                case ChildWndOptionMode.Add:
                    {
                        ContinueBtnVisibility = Visibility.Visible;
                        OkBtnContent = "添加";
                        Title = "添加签到班班次信息";
                        try
                        {
                            this.WorkCnt = 1.0;
                            this.MinWorkTime = Convert.ToDateTime(CalTimeByMinutes(60));
                        }
                        catch (Exception e)
                        {
                            ErrorWindow err = new ErrorWindow(e);
                            err.Show();
                        }
                    }
                    break;
            }
           
        }      
     
        #endregion

        #region 界面命令响应
        private void OkBtnClicked()
        {
            switch (_operateClassOrderMode)
            {
                case ChildWndOptionMode.Modify:
                    {
                        ModifyClassOrder();
                    }
                    break;

                case ChildWndOptionMode.Add:
                    {
                        AddClassOrder(false);
                    }
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

        #endregion      

        #region wcf ria 操作 

        /// <summary>
        /// 添加班次
        /// </summary>
        /// <param name="isContinue"></param>
        private void AddClassOrder(bool isContinue)
        {
            //获取操作描述
            string description;
            description = string.Format("班次名称：{0}，所属班制：{1}；\r\n", ClassOrderSignName, SelectedClassType.class_type_name);
            
            _isContinueAddExcuted = isContinue;           

            if (ClassOrderSignName == null || ClassOrderSignName.Equals(""))
            {
                MsgBoxWindow.MsgBox(
                              "班次名称不能为空！",
                              MsgBoxWindow.MsgIcon.Information,
                              MsgBoxWindow.MsgBtns.OK);
                return;
            }

            if (SectionDatas.Count <= 0 )
            {
                MsgBoxWindow.MsgBox(
                             "签到班时间段不能为空！",
                             MsgBoxWindow.MsgIcon.Information,
                             MsgBoxWindow.MsgBtns.OK);
                return;
            }

            bool bCritical = false;

            for (int index = 0; index < SectionDatas.Count; index++)
            {
                if (SectionDatas[index].InCalc == 1)
                {
                    bCritical = true;
                    break;
                }
            }

            if (bCritical == false)
            {
                MsgBoxWindow.MsgBox(
                             "签到班时间段至少要包含一个关键时间段！",
                             MsgBoxWindow.MsgIcon.Information,
                             MsgBoxWindow.MsgBtns.OK);
                return;
            }

            UserClassOrderSignInfo addItem = new UserClassOrderSignInfo();
            addItem.class_order_name = ClassOrderSignName;
            addItem.attend_sign = AttendSign;
            addItem.class_type_id = SelectedClassType.class_type_id;
            addItem.class_type_name = SelectedClassType.class_type_name;
            addItem.lian_ban = SelectLianBanIndex;

            addItem.min_work_time = (short)DateTimeToMinute(MinWorkTime);
            addItem.work_cnt = (int)(WorkCnt * 10);

            addItem.section_begin_mins = new int[SectionDatas.Count];
            addItem.section_end_mins = new int[SectionDatas.Count];
            addItem.in_calcs = new int[SectionDatas.Count];

            for( int sectionIndex = 0; sectionIndex < SectionDatas.Count; sectionIndex ++ )
            {
                addItem.section_begin_mins[sectionIndex] = SectionDatas[sectionIndex].SectionBeginMin;
                addItem.section_end_mins[sectionIndex] = SectionDatas[sectionIndex].SectionEndMin;
                addItem.in_calcs[sectionIndex] = SectionDatas[sectionIndex].InCalc;
            }

            try
            {             
                WaitingDialog.ShowWaiting();
                ServiceDomDbAcess.ReOpenSever();
                ServiceDomDbAcess.GetSever().AddClassOrderSign(addItem, CallBackHandleControl<OptionInfo>.OnInvokeErrorCallBack, null);

                CallBackHandleControl<OptionInfo>.m_sendValue = (option) =>
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
                        if (!option.isSuccess)
                        {
                            MsgBoxWindow.MsgBox(
                              option.option_info + "！",
                              MsgBoxWindow.MsgIcon.Error,
                              MsgBoxWindow.MsgBtns.OK);
                            VmOperatorLog.InsertOperatorLog(0, "添加签到班班次", description + option.option_info, null);
                            
                        }
                        else if (!option.isNotifySuccess)
                        {
                            MsgBoxWindow.MsgBox(
                              option.option_info + "！",
                              MsgBoxWindow.MsgIcon.Warning,
                              MsgBoxWindow.MsgBtns.OK);
                            VmOperatorLog.InsertOperatorLog(1, "添加签到班班次", description + option.option_info, completeCallBack);
                        }
                        else
                        {
                            MsgBoxWindow.MsgBox(
                           option.option_info + "！",
                           MsgBoxWindow.MsgIcon.Succeed,
                           MsgBoxWindow.MsgBtns.OK);
                            VmOperatorLog.InsertOperatorLog(1, "添加签到班班次", description, completeCallBack);
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
            //获取操作描述
            string description;
            description = string.Format("班次名称：{0}，所属班制：{1}；\r\n", ClassOrderSignName, SelectedClassType.class_type_name);           

            if (ClassOrderSignName == null || ClassOrderSignName.Equals(""))
            {
                MsgBoxWindow.MsgBox(
                              "班次名称不能为空！",
                              MsgBoxWindow.MsgIcon.Information,
                              MsgBoxWindow.MsgBtns.OK);
                return;
            }

            if (SectionDatas.Count <= 0)
            {
                MsgBoxWindow.MsgBox(
                             "签到班时间段不能为空！",
                             MsgBoxWindow.MsgIcon.Information,
                             MsgBoxWindow.MsgBtns.OK);
                return;
            }

            bool bCritical = false;

            for (int index = 0; index < SectionDatas.Count; index++)
            {
                if (SectionDatas[index].InCalc == 1)
                {
                    bCritical = true;
                    break;
                }
            }

            if (bCritical == false)
            {
                MsgBoxWindow.MsgBox(
                             "签到班时间段至少要包含一个关键时间段！",
                             MsgBoxWindow.MsgIcon.Information,
                             MsgBoxWindow.MsgBtns.OK);
                return;
            }

            UserClassOrderSignInfo addItem = new UserClassOrderSignInfo();
            addItem.class_order_id = _classOrderSignInfo.class_order_id;
            addItem.class_order_name = ClassOrderSignName;
            addItem.attend_sign = AttendSign;
            addItem.class_type_id = SelectedClassType.class_type_id;
            addItem.class_type_name = SelectedClassType.class_type_name;
            addItem.lian_ban = SelectLianBanIndex;

            addItem.min_work_time = (short)DateTimeToMinute(MinWorkTime);
            addItem.work_cnt = (int)(WorkCnt * 10);

            addItem.section_begin_mins = new int[SectionDatas.Count];
            addItem.section_end_mins = new int[SectionDatas.Count];
            addItem.in_calcs = new int[SectionDatas.Count];

            for (int sectionIndex = 0; sectionIndex < SectionDatas.Count; sectionIndex++)
            {
                addItem.section_begin_mins[sectionIndex] = SectionDatas[sectionIndex].SectionBeginMin;
                addItem.section_end_mins[sectionIndex] = SectionDatas[sectionIndex].SectionEndMin;
                addItem.in_calcs[sectionIndex] = SectionDatas[sectionIndex].InCalc;
            }

            try
            {
                WaitingDialog.ShowWaiting();
                ServiceDomDbAcess.ReOpenSever();
                ServiceDomDbAcess.GetSever().ModifyClassOrderSign(addItem, CallBackHandleControl<OptionInfo>.OnInvokeErrorCallBack, null);

                CallBackHandleControl<OptionInfo>.m_sendValue = (option) =>
                {                    
                   
                    VmOperatorLog.CompleteCallBack CompleteCallBack = () =>
                        {
                            if (CloseEvent != null)
                            {
                                CloseEvent(true);
                            }
                        };
                    if (!option.isSuccess)
                    {
                        MsgBoxWindow.MsgBox(
                            option.option_info + "！",
                            MsgBoxWindow.MsgIcon.Error,
                            MsgBoxWindow.MsgBtns.OK);
                        VmOperatorLog.InsertOperatorLog(0, "修改签到班班次", description + option.option_info, CompleteCallBack);                       
                    }
                    else
                    {
                        if (!option.isNotifySuccess)
                        {
                            MsgBoxWindow.MsgBox(
                                option.option_info + "！",
                                MsgBoxWindow.MsgIcon.Warning,
                                MsgBoxWindow.MsgBtns.OK);
                            VmOperatorLog.InsertOperatorLog(1, "修改签到班班次", description + option.option_info, CompleteCallBack);
                        }
                        else
                        {
                            MsgBoxWindow.MsgBox(
                                option.option_info + "！",
                                MsgBoxWindow.MsgIcon.Succeed,
                                MsgBoxWindow.MsgBtns.OK);
                            VmOperatorLog.InsertOperatorLog(1, "修改签到班班次", description, CompleteCallBack);
                        }
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
    /// 数据绑定的实体类:签到班班次的时间段
    /// </summary>
    public class ClassOrderSignSection : BaseViewModel
    {
        private int _sectionBeginMin;
        /// <summary>
        /// 起始时间
        /// </summary>
        public int SectionBeginMin
        {
            get
            {
                return _sectionBeginMin;
            }
            set
            {
                _sectionBeginMin = value;
                OnPropertyChanged<int>(() => this.SectionBeginMin);
            }
        }

        private string _sectionBeginMinStr;
        /// <summary>
        /// 起始时间字符形式
        /// </summary>
        public string SectionBeginMinStr
        {
            get
            {
                return _sectionBeginMinStr;
            }
            set
            {
                _sectionBeginMinStr = value;
                OnPropertyChanged<string>(() => this.SectionBeginMinStr);
            }
        }

        private int _sectionEndMin;
        /// <summary>
        /// 结束时间
        /// </summary>
        public int SectionEndMin
        {
            get
            {
                return _sectionEndMin;
            }
            set
            {
                _sectionEndMin = value;
                OnPropertyChanged<int>(() => this.SectionEndMin);
            }
        }

        private string _sectionEndMinStr;
        /// <summary>
        /// 结束时间字符形式
        /// </summary>
        public string SectionEndMinStr
        {
            get
            {
                return _sectionEndMinStr;
            }
            set
            {
                _sectionEndMinStr = value;
                OnPropertyChanged<string>(() => this.SectionEndMinStr);
            }
        }

        private int _inCalc;
        /// <summary>
        /// 是否是关键时间段
        /// </summary>
        public int InCalc
        {
            get
            {
                return _inCalc;
            }
            set
            {
                _inCalc = value;
                OnPropertyChanged<int>(() => this.InCalc);
            }
        }     


        private string _inCalcStr;
        /// <summary>
        /// 是否是关键时间段字符形式
        /// </summary>
        public string InCalcStr
        {
            get
            {
                return _inCalcStr;
            }
            set
            {
                _inCalcStr = value;
                OnPropertyChanged<string>(() => this.InCalcStr);
            }
        }       


        /// <summary>
        /// 构造函数
        /// </summary>
        public ClassOrderSignSection()
        {
            InCalc = 1;
            InCalcStr = "是";
        }

    }
}
