/*************************************************************************
** 文件名:   VmXlsOriginRecSumReport.cs
** 主要类:   VmXlsOriginRecSumReport
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   cty
** 日  期:   2013-5-13
** 修改人:   
** 日  期:
** 描  述:   用于报表显示的字段
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
using IriskingAttend.Web;
using System.ServiceModel.DomainServices.Client;
using Irisking.Web.DataModel;
using EDatabaseError;
using System.Collections.Generic;
using System.ComponentModel;
using IriskingAttend.Common;

namespace IriskingAttend.ViewModel
{
    public class XlsServiceDomDbAcess
    {
        #region 私有变量
        /// <summary>
        /// 域服务声明
        /// </summary>
        private DomainServiceIriskingAttend _serviceDomDbAccess = new DomainServiceIriskingAttend();

        #endregion

        #region 绑定属性

        /// <summary>
        /// 存放报表数据的集合
        /// </summary>
        public BaseViewModelCollection<XlsAttend> XlsAttend { get; set; }

        /// <summary>
        /// 前台界面绑定的数据集合
        /// </summary>
        public BaseViewModelCollection<XlsAttend> XlsAttendDataGridModel { get; set; }

        #endregion

        #region 事件

        /// <summary>
        /// 事件
        /// </summary>
        public Action<BaseViewModelCollection<XlsAttend>> ActionData = null;

        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public  XlsServiceDomDbAcess()
        {
            this.XlsAttend = new BaseViewModelCollection<XlsAttend>();
            XlsAttendDataGridModel = new BaseViewModelCollection<XlsAttend>();
        }
        #endregion

        #region 获取服务

        /// <summary>
        /// 获取域服务
        /// </summary>
        /// <returns></returns>
        public DomainServiceIriskingAttend GetSever()
        {
            return _serviceDomDbAccess;
        }

        /// <summary>
        /// 另起服务
        /// </summary>
        public void ReOpenSever()
        {
            _serviceDomDbAccess = new DomainServiceIriskingAttend();
        }

        #endregion

        #region 根据查询条件获取报表数据
        /// <summary>
        /// 根据查询条件获取报表数据
        /// </summary>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">截止时间</param>
        /// <param name="departIdLst">部门列表</param>
        /// <param name="ClassTypeId">班制</param>
        /// <param name="name">人员姓名</param>
        /// <param name="workSN">人员工号</param>
        /// <returns>数据集合</returns>
        public BaseViewModelCollection<XlsAttend> GetAttendBaoBiao(DateTime beginTime, DateTime endTime, int[] departIdLst, int ClassTypeId, string name, string workSN)
        {
            WaitingDialog.ShowWaiting();
            XlsAttend.Clear();
            EntityQuery<XlsUserAttendRec> list = _serviceDomDbAccess.MyGetXlsUserAttendRecQuery(beginTime, endTime, departIdLst, ClassTypeId, name, workSN);
            //回调异常类
            Action<LoadOperation<XlsUserAttendRec>> actionCallBack = new Action<LoadOperation<XlsUserAttendRec>>(ErrorHandle<XlsUserAttendRec>.OnLoadErrorCallBack);
            //异步事件
            LoadOperation<XlsUserAttendRec> lo = _serviceDomDbAccess.Load(list, actionCallBack, null);
            // actionCallBack, null
            lo.Completed += delegate
            {
                try
                {
                    //异步获取数据
                    foreach (XlsUserAttendRec ar in lo.Entities)
                    {
                        this.DoWithData(ar, beginTime, endTime);
                    }
                    int index = 0;
                    XlsAttendDataGridModel.Clear();
                    foreach (XlsAttend ar in XlsAttend)
                    {
                        index++;
                        ar.Index = index;
                        XlsAttendDataGridModel.Add(ar);
                    }
                }
                catch (Exception e)
                {
                    ChildWindow errorWin = new ErrorWindow("加载异常", e.Message);
                    errorWin.Show();
                }
                if (this.ActionData != null)
                {
                    this.ActionData(this.XlsAttend);
                }
                WaitingDialog.HideWaiting();
            };
            return XlsAttend;
        }

        #endregion

        #region 将查询出的数据处理成报表的样式，用于直接绑定

        /// <summary>
        /// 将查询出的数据处理成报表的样式，用于直接绑定
        /// </summary>
        /// <param name="ar"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        public void DoWithData(XlsUserAttendRec ar, DateTime beginTime, DateTime endTime)
        {
            foreach (XlsAttend xad in this.XlsAttend)
            { 
                if (xad.PersonId == ar.person_id)
                {
                    for (int i = 0; i < xad.AttendDays.Count; i++)
                    {
                        if (xad.AttendDays[i].ToShortDateString() == ar.attend_day.ToShortDateString())
                        {
                            xad.WorkTime[i] = ar.work_time;
                            if (ar.work_time % 60 > 9)
                            {
                                xad.WorktimeHour[i] = (ar.work_time / 60).ToString() + ":" + (ar.work_time % 60).ToString();
                            }
                            else
                            {
                                xad.WorktimeHour[i] = (ar.work_time / 60).ToString() + ":0" + (ar.work_time % 60).ToString();
                            }
                        }
                    }
                    xad.SumAttendTimes += ar.attend_times;
                    xad.SumWorktime += ar.work_time;
                    xad.AvgWorktime = xad.SumWorktime / xad.SumAttendTimes;
                    return;
                }
            }

            XlsAttend xd = new XlsAttend();
            xd.WorkTime = new List<int>();
            xd.WorktimeHour = new List<string>();
            xd.AttendDays = new List<DateTime>();

            xd.PersonId = ar.person_id;
            xd.WorkSn = ar.work_sn;
            xd.PersonName = ar.person_name;
            xd.DepartName = ar.depart_name;
            xd.ClassOrderName = ar.class_order_name;
            xd.ClassTypeName = ar.class_type_name;
            for (DateTime m_beginTime = beginTime; m_beginTime < endTime; m_beginTime = m_beginTime.AddDays(1))
            {
                xd.AttendDays.Add(m_beginTime);
                xd.WorkTime.Add(0);
                xd.WorktimeHour.Add("0");
            }
            for(int i=0;i<xd.AttendDays.Count;i++)
            {
                if(xd.AttendDays[i].ToShortDateString()==ar.attend_day.ToShortDateString())
                {
                    xd.WorkTime[i]=ar.work_time;
                    if (ar.work_time % 60 > 9)
                    {
                        xd.WorktimeHour[i] = (ar.work_time / 60).ToString() + ":" + (ar.work_time % 60).ToString();
                    }
                    else
                    {
                        xd.WorktimeHour[i] = (ar.work_time / 60).ToString() + ":0" + (ar.work_time % 60).ToString();
                    }
                }
            }
            
            xd.AttendTimes = ar.attend_times;
            xd.SumAttendTimes += ar.attend_times;
            xd.SumWorktime += ar.work_time;
            xd.AvgWorktime = xd.SumWorktime / xd.SumAttendTimes;
            this.XlsAttend.Add(xd);
        }

        #endregion
    }

    /// <summary>
    /// 原始记录汇总表集合
    /// </summary>
    public class XlsReport
    {
        /// <summary>
        /// 起始时间
        /// </summary>
        public DateTime XlsbeginTime { get; set; }

        /// <summary>
        /// 截止时间
        /// </summary>
        public DateTime XlsendTime { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        public int[] XlsdepartIdLst { get; set; }

        /// <summary>
        /// 考勤类型
        /// </summary>
        public int[] XlsattendTypeIdLst { get; set; }

        /// <summary>
        /// 班制
        /// </summary>
        public int XlsClassTypeName { get; set; }

        /// <summary>
        /// 考勤班次
        /// </summary>
        public string XlsClassOrderName { get; set; }

        /// <summary>
        /// 人员姓名
        /// </summary>
        public string Xlsname { get; set; }

        /// <summary>
        /// 工号
        /// </summary>
        public string XlsworkSN { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public XlsReport()
        {
            XlsbeginTime = DateTime.MinValue;
            XlsendTime = DateTime.Now;
            XlsdepartIdLst = null;
            XlsattendTypeIdLst = null;
            Xlsname = "";
            XlsworkSN = "";
            XlsClassTypeName = -1;
            XlsClassOrderName = "";

        }
    }

    /// <summary>
    /// 原始记录汇总表绑定集合 
    /// </summary>
    public class XlsAttend : INotifyPropertyChanged
    {
        /// <summary>
        /// 前台绑定序号
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 考勤id
        /// </summary>
        public int AttendRecordId { get; set; }

        /// <summary>
        /// personid
        /// </summary>
        public int PersonId { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string PersonName { get; set; }

        /// <summary>
        /// 工号
        /// </summary>
        public string WorkSn { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        public string DepartName { get; set; }

        /// <summary>
        /// 班次名称
        /// </summary>
        public string ClassOrderName { get; set; }

        /// <summary>
        /// 班制名称
        /// </summary>
        public string ClassTypeName { get; set; }

        /// <summary>
        /// 工时
        /// </summary>
        public List<int> WorkTime { get; set; }

        /// <summary>
        /// 工时 工时由分钟数转化成小时
        /// </summary>
        public List<string> WorktimeHour { get; set; }

        /// <summary>
        /// 工数
        /// </summary>
        public int AttendTimes { get; set; }

        /// <summary>
        /// 出勤次数
        /// </summary>
        public int SumAttendTimes { get; set; }

        /// <summary>
        /// 平均工时
        /// </summary>
        public int AvgWorktime { get; set; }

        /// <summary>
        /// 平均工时 平均工时由分钟数转化成小时
        /// </summary>
        public string AvgWorktimeShow
        {
            get
            {
                if (AvgWorktime % 60 > 9)
                {
                    return (AvgWorktime / 60).ToString() + ":" + (AvgWorktime % 60).ToString();
                }
                else
                {
                    return (AvgWorktime / 60).ToString() + ":0" + (AvgWorktime % 60).ToString();
                }
            }
        }

        /// <summary>
        /// 总工时
        /// </summary>
        public int SumWorktime { get; set; }

        /// <summary>
        /// 总工时 总工时由分钟数转化成小时
        /// </summary>
        public string SumWorktimeShow
        {
            get
            {
                if (SumWorktime % 60 > 9)
                {
                    return (SumWorktime / 60).ToString() + ":" + (SumWorktime % 60).ToString();
                }
                else
                {
                    return (SumWorktime / 60).ToString() + ":0" + (SumWorktime % 60).ToString();
                }
            }
        }
        
        /// <summary>
        /// 归属日
        /// </summary>
        public List<DateTime> AttendDays { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
