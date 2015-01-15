/*************************************************************************
** 文件名:   VmXlsWuHuShan.cs
** 主要类:   VmXlsWuHuShan
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   cty
** 日  期:   2013-8-30
** 修改人:   
** 日  期:
** 描  述:   VmXlsWuHuShan，五虎山报表
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
using IriskingAttend.ViewModel;
using System.ComponentModel;
using System.Collections.Generic;
using System.ServiceModel.DomainServices.Client;
using IriskingAttend.Common;
using EDatabaseError;

namespace IriskingAttend.NewWuHuShan
{
    public class VmXlsWuHuShan
    {
        #region 私有变量声明

        /// <summary>
        /// 域服务声明
        /// </summary>
        private DomainServiceIriskingAttend _serviceDomDbAccess = new DomainServiceIriskingAttend();

        #endregion

        #region 绑定属性

        /// <summary>
        /// 五虎山虹膜考勤查询 数据集合
        /// </summary>
        public BaseViewModelCollection<XlsAttendWuHuShan> IrisAttendQueryModel
        {
            get;
            set;
        }

        /// <summary>
        /// 五虎山虹膜考勤查询 数据绑定集合
        /// </summary>
        public BaseViewModelCollection<XlsAttendWuHuShan> IrisAttendQueryModelShow
        {
            get;
            set;
        }

        /// <summary>
        /// 虹膜考勤查询界面绑定的详细信息数据集合
        /// </summary>
        public BaseViewModelCollection<XlsAttendWuHuShan> IrisShowDetailsModel
        {
            get;
            set;
        }


        /// <summary>
        /// 数据集合
        /// </summary>
        public BaseViewModelCollection<XlsAttendWuHuShan> XlsAttendDataGridWuHuShanModel
        {
            get;
            set;
        }

        /// <summary>
        /// 前台界面绑定的数据集合
        /// </summary>
        public BaseViewModelCollection<XlsAttendWuHuShan> XlsAttendDataGridWuHuShanModelShow
        {
            get;
            set;
        }

        /// <summary>
        /// 详细信息界面绑定的数据集合
        /// </summary>
        public BaseViewModelCollection<XlsAttendWuHuShan> ShowDetailsModel
        {
            get;
            set;
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public VmXlsWuHuShan()
        {
            IrisAttendQueryModel = new BaseViewModelCollection<XlsAttendWuHuShan>();
            IrisAttendQueryModelShow = new BaseViewModelCollection<XlsAttendWuHuShan>();
            IrisShowDetailsModel = new BaseViewModelCollection<XlsAttendWuHuShan>();

            XlsAttendDataGridWuHuShanModel = new BaseViewModelCollection<XlsAttendWuHuShan>();
            XlsAttendDataGridWuHuShanModelShow = new BaseViewModelCollection<XlsAttendWuHuShan>();
            ShowDetailsModel = new BaseViewModelCollection<XlsAttendWuHuShan>();
        }

        #endregion

        #region 根据查询条件查询数据并对数据进行处理 （虹膜考勤查询界面）

        /// <summary>
        /// 根据查询条件查询数据
        /// </summary>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">截止时间</param>
        /// <param name="departIdLst">部门列表</param>
        /// <param name="name">人员姓名</param>
        /// <param name="workSn">人员工号</param>
        /// <param name="principalIdList">职务列表</param>
        /// <param name="workTypeIdList">工种列表</param>
        /// <param name="workTime">工作时长</param>
        /// <returns></returns>
        public BaseViewModelCollection<XlsAttendWuHuShan> GetIrisAttendQuery(DateTime beginTime, DateTime endTime, int[] departIdLst, string name, string workSn, int[] principalIdList, int[] workTypeIdList, int workTime)
        {
            WaitingDialog.ShowWaiting();
            EntityQuery<XlsAttendWuHuShanPersonList> list = _serviceDomDbAccess.GetIrisAttendQueryQuery(beginTime, endTime, departIdLst, name, workSn, principalIdList, workTypeIdList, workTime);
            //回调异常类
            Action<LoadOperation<XlsAttendWuHuShanPersonList>> actionCallBack = new Action<LoadOperation<XlsAttendWuHuShanPersonList>>(ErrorHandle<XlsAttendWuHuShanPersonList>.OnLoadErrorCallBack);
            //异步事件
            LoadOperation<XlsAttendWuHuShanPersonList> lo = _serviceDomDbAccess.Load(list, actionCallBack, null);

            lo.Completed += delegate
            {
                try
                {
                    IrisAttendQueryModel.Clear();
                    //异步获取数据
                    foreach (XlsAttendWuHuShanPersonList ar in lo.Entities)
                    {
                        //对查询出的数据进行处理，处理成前台可绑定的集合
                        this.DoWithIrisAttendData(ar, beginTime, endTime);
                    }

                    IrisAttendQueryModelShow.Clear();
                    int index = 0;
                    XlsAttendWuHuShan total = new XlsAttendWuHuShan();
                    total.AtteinTime = new List<string>();
                    for (DateTime m_beginTime = beginTime; m_beginTime < endTime; m_beginTime = m_beginTime.AddDays(1))
                    {
                        total.AtteinTime.Add("");
                    }

                    total.DepartName = "总合计";
                    foreach (XlsAttendWuHuShan ar in IrisAttendQueryModel)
                    {
                        index++;
                        ar.Index = index;
                        total.MiddleAttendTime += ar.MiddleAttendTime;
                        total.MoringAttendTime += ar.MoringAttendTime;
                        total.NightAttendTime += ar.NightAttendTime;
                        total.SumAttendTimes += ar.SumAttendTimes;
                        IrisAttendQueryModelShow.Add(ar);

                    }
                    if (IrisAttendQueryModelShow.Count > 0)
                    {
                        total.Index = ++index;
                        total.MiddleAttendTimes = total.MiddleAttendTime.ToString() == "0" ? "" : total.MiddleAttendTime.ToString();
                        total.MoringAttendTimes = total.MoringAttendTime.ToString() == "0" ? "" : total.MoringAttendTime.ToString();
                        total.NightAttendTimes = total.NightAttendTime.ToString() == "0" ? "" : total.NightAttendTime.ToString();
                        IrisAttendQueryModelShow.Add(total);
                    }
                }
                catch (Exception e)
                {
                    ChildWindow errorWin = new ErrorWindow("加载异常", e.Message);
                    errorWin.Show();
                }
                WaitingDialog.HideWaiting();
            };
            return IrisAttendQueryModel;
        }

        /// <summary>
        /// 对查询出的数据进行处理，处理成前台可绑定的集合
        /// </summary>
        /// <param name="ar"></param>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">截止时间</param>
        private void DoWithIrisAttendData(XlsAttendWuHuShanPersonList ar, DateTime beginTime, DateTime endTime)
        {
            foreach (XlsAttendWuHuShan xa in IrisAttendQueryModel)
            {
                if (xa.PersonId == ar.PersonId)
                {
                    for (int i = 0; i < xa.AttendDays.Count; i++)
                    {
                        if (xa.AttendDays[i].ToShortDateString() == ar.AttendDay.ToShortDateString())
                        {
                            if (xa.AtteinTime[i] != "1")
                            {
                                xa.AtteinTime[i] = "1";
                                xa.IrisWorkTimeList[i] = ar.IrisWorkTime;                            
                                xa.InWellTimeList[i] = ar.InWellTime;
                                xa.OutWellTimeList[i] = ar.OutWellTime;
                                                          
                                xa.ClassOrderNameList[i] = ar.ClassOrderName;

                                if ((ar.ClassOrderName).Contains("早班"))
                                {
                                    xa.MoringAttendTime++;
                                    xa.MoringAttendTimes = xa.MoringAttendTime.ToString();
                                }
                                else if ((ar.ClassOrderName).Contains("中班"))
                                {
                                    xa.MiddleAttendTime++;
                                    xa.MiddleAttendTimes = xa.MiddleAttendTime.ToString();
                                }
                                else if ((ar.ClassOrderName).Contains("夜班"))
                                {
                                    xa.NightAttendTime++;
                                    xa.NightAttendTimes = xa.NightAttendTime.ToString();
                                }
                                xa.SumAttendTimes++;
                            }
                            else //一天只统计一个工按夜班、早班、中班优先级来算，
                            {
                                if ((ar.ClassOrderName).Contains("夜班"))
                                {
                                    if (xa.ClassOrderNameList[i].Contains("早班"))
                                    {
                                        xa.IrisWorkTimeList[i] = ar.IrisWorkTime;
                                        xa.InWellTimeList[i] = ar.InWellTime;
                                        xa.OutWellTimeList[i] = ar.OutWellTime;

                                        xa.ClassOrderNameList[i] = ar.ClassOrderName;
                                        xa.NightAttendTime++;
                                        xa.NightAttendTimes = xa.NightAttendTime.ToString();
                                        xa.MoringAttendTime--;
                                        xa.MoringAttendTimes = xa.MoringAttendTime.ToString();
                                    }
                                    else if ((xa.ClassOrderNameList[i]).Contains("中班"))
                                    {
                                        xa.IrisWorkTimeList[i] = ar.IrisWorkTime;
                                        xa.InWellTimeList[i] = ar.InWellTime;
                                        xa.OutWellTimeList[i] = ar.OutWellTime;


                                        xa.ClassOrderNameList[i] = ar.ClassOrderName;
                                        xa.NightAttendTime++;
                                        xa.NightAttendTimes = xa.NightAttendTime.ToString();
                                        xa.MiddleAttendTime--;
                                        xa.MiddleAttendTimes = xa.MiddleAttendTime.ToString();
                                    }
                                }
                                else if ((ar.ClassOrderName).Contains("早班"))
                                {
                                    if ((xa.ClassOrderNameList[i]).Contains("中班"))
                                    {
                                        xa.IrisWorkTimeList[i] = ar.IrisWorkTime;
                                        xa.InWellTimeList[i] = ar.InWellTime;
                                        xa.OutWellTimeList[i] = ar.OutWellTime;

                                        xa.ClassOrderNameList[i] = ar.ClassOrderName;
                                        xa.MoringAttendTime++;
                                        xa.MoringAttendTimes = xa.MoringAttendTime.ToString();
                                        xa.MiddleAttendTime--;
                                        xa.MiddleAttendTimes = xa.MiddleAttendTime.ToString();
                                    }
                                }
                            }
                        }
                    }
                    
                    return;
                }
            }

            XlsAttendWuHuShan xd = new XlsAttendWuHuShan();
            xd.IrisWorkTimeList = new List<TimeSpan?>();
            xd.LocateWorkTimeList = new List<TimeSpan?>();
            xd.AttendDays = new List<DateTime>();
            xd.AtteinTime = new List<string>();
            xd.InWellTimeList = new List<DateTime?>();
            xd.OutWellTimeList = new List<DateTime?>();
            xd.InLocateTimeList = new List<DateTime?>();
            xd.OutLocateTimeList = new List<DateTime?>();
            xd.ClassOrderNameList = new List<string>();

            xd.PersonId = ar.PersonId;
            xd.WorkSn = ar.WorkSn;
            xd.WorkType = ar.WorkType;
            xd.PrincipalName = ar.Principal;
            xd.PersonName = ar.PersonName;
            xd.DepartName = ar.DepartName;
            xd.ClassOrderName = ar.ClassOrderName;
            xd.ClassTypeName = ar.ClassTypeName;

            for (DateTime m_beginTime = beginTime; m_beginTime < endTime; m_beginTime = m_beginTime.AddDays(1))
            {
                xd.AttendDays.Add(m_beginTime);
                xd.IrisWorkTimeList.Add(TimeSpan.FromHours(0));
                xd.LocateWorkTimeList.Add(TimeSpan.FromHours(0));
                xd.AtteinTime.Add("");
                xd.InWellTimeList.Add(DateTime.MinValue);
                xd.OutWellTimeList.Add(DateTime.MinValue);
                xd.InLocateTimeList.Add(DateTime.MinValue);
                xd.OutLocateTimeList.Add(DateTime.MinValue);
                xd.ClassOrderNameList.Add("");
            }
            for (int i = 0; i < xd.AttendDays.Count; i++)
            {
                if (xd.AttendDays[i].ToShortDateString() == ar.AttendDay.ToShortDateString())
                {
                    if(xd.AtteinTime[i]!="1")
                    {
                        xd.AtteinTime[i] = "1";
                        xd.AttendTimes = 1;

                        xd.IrisWorkTimeList[i] = ar.IrisWorkTime;
                        xd.InWellTimeList[i] = ar.InWellTime;
                        xd.OutWellTimeList[i] = ar.OutWellTime;
                        xd.ClassOrderNameList[i] = ar.ClassOrderName;

                        if ((ar.ClassOrderName).Contains("早班"))
                        {
                            xd.MoringAttendTime++;
                            xd.MoringAttendTimes = xd.MoringAttendTime.ToString();
                        }
                        else if ((ar.ClassOrderName).Contains("中班"))
                        {
                            xd.MiddleAttendTime++;
                            xd.MiddleAttendTimes = xd.MiddleAttendTime.ToString();
                        }
                        else if ((ar.ClassOrderName).Contains("夜班"))
                        {
                            xd.NightAttendTime++;
                            xd.NightAttendTimes = xd.NightAttendTime.ToString();
                        }
                    }                 
                }
            }
            xd.SumAttendTimes += xd.AttendTimes;

            this.IrisAttendQueryModel.Add(xd);
        }

        #endregion

        #region 获取显示详细信息的数据集合

        /// <summary>
        /// 获取显示详细信息的数据集合
        /// </summary>
        /// <param name="personId">人员的personID</param>
        /// <param name="id">链表中的索引</param>
        /// <returns></returns>
        public BaseViewModelCollection<XlsAttendWuHuShan> IrisAttendQueryShowDetail(int personId, int id)
        {
            IrisShowDetailsModel.Clear();
            foreach (XlsAttendWuHuShan ar in IrisAttendQueryModelShow)
            {
                if (ar.PersonId == personId)
                {
                    XlsAttendWuHuShan xa = new XlsAttendWuHuShan();

                    xa.AttendDay = ar.AttendDays[id];
                    xa.WorkSn = ar.WorkSn;
                    xa.PersonName = ar.PersonName;
                    xa.DepartName = ar.DepartName;
                    xa.WorkType = ar.WorkType;
                    xa.PrincipalName = ar.PrincipalName;
                    xa.ClassTypeName = ar.ClassTypeName;
                    xa.ClassOrderName = ar.ClassOrderNameList[id];
                    xa.InWellTime = ar.InWellTimeList[id];
                    xa.OutWellTime = ar.OutWellTimeList[id];
                    xa.IrisWorkTimeStr = ar.IrisWorkTimeList[id].Value.ToString();

                    IrisShowDetailsModel.Add(xa);
                }
            }

            return IrisShowDetailsModel;
        }

        #endregion

        #region 根据查询条件查询数据并对数据进行处理

        /// <summary>
        /// 根据查询条件查询数据
        /// </summary>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">截止时间</param>
        /// <param name="departIdLst">部门列表</param>
        /// <param name="name">人员姓名</param>
        /// <param name="workSn">人员工号</param>
        /// <param name="principalIdList">职务列表</param>
        /// <param name="workTypeIdList">工种列表</param>
        /// <param name="workTime">工作时长</param>
        /// <returns></returns>
        public BaseViewModelCollection<XlsAttendWuHuShan> GetXlsWuHuShanPersonList(DateTime beginTime, DateTime endTime, int[] departIdLst, string name, string workSn, int[] principalIdList, int[] workTypeIdList, int workTime)
        {
            WaitingDialog.ShowWaiting();
            //2014.2.24 wz 添加，刷新personList数据
            _serviceDomDbAccess = new DomainServiceIriskingAttend();
            

            EntityQuery<XlsAttendWuHuShanPersonList> list = _serviceDomDbAccess.GetXlsWuHuShanPersonListQuery(beginTime, endTime, departIdLst,name,workSn, principalIdList, workTypeIdList, workTime);
            //回调异常类
            Action<LoadOperation<XlsAttendWuHuShanPersonList>> actionCallBack = new Action<LoadOperation<XlsAttendWuHuShanPersonList>>(ErrorHandle<XlsAttendWuHuShanPersonList>.OnLoadErrorCallBack);
            //异步事件
            LoadOperation<XlsAttendWuHuShanPersonList> lo = _serviceDomDbAccess.Load(list, actionCallBack, null);

            lo.Completed += delegate
            {
                try
                {
                    XlsAttendDataGridWuHuShanModel.Clear();
                    //异步获取数据
                    foreach (XlsAttendWuHuShanPersonList ar in lo.Entities)
                    {
                        //对查询出的数据进行处理，处理成前台可绑定的集合
                        this.DoWithData(ar, beginTime, endTime);
                    }

                    XlsAttendDataGridWuHuShanModelShow.Clear();
                    int index = 0;
                    XlsAttendWuHuShan total = new XlsAttendWuHuShan();
                    total.AtteinTime = new List<string>();
                    for (DateTime m_beginTime = beginTime; m_beginTime < endTime; m_beginTime = m_beginTime.AddDays(1))
                    {
                        total.AtteinTime.Add("");
                    }

                    total.DepartName = "总合计";
                    foreach (XlsAttendWuHuShan ar in XlsAttendDataGridWuHuShanModel)
                    {
                        index++;
                        ar.Index = index;
                        total.MiddleAttendTime += ar.MiddleAttendTime;               
                        total.MoringAttendTime += ar.MoringAttendTime;                     
                        total.NightAttendTime += ar.NightAttendTime;                       
                        total.SumAttendTimes += ar.SumAttendTimes;
                        XlsAttendDataGridWuHuShanModelShow.Add(ar);
                        
                    }
                    if (XlsAttendDataGridWuHuShanModelShow.Count > 0)
                    {
                        total.Index = ++index;
                        total.MiddleAttendTimes = total.MiddleAttendTime.ToString() == "0" ? "" : total.MiddleAttendTime.ToString();
                        total.MoringAttendTimes = total.MoringAttendTime.ToString() == "0" ? "" : total.MoringAttendTime.ToString();
                        total.NightAttendTimes = total.NightAttendTime.ToString() == "0" ? "" : total.NightAttendTime.ToString();
                        XlsAttendDataGridWuHuShanModelShow.Add(total);
                    }
                }
                catch (Exception e)
                {
                    ChildWindow errorWin = new ErrorWindow("加载异常", e.Message);
                    errorWin.Show();
                }
                WaitingDialog.HideWaiting();
            };
            return XlsAttendDataGridWuHuShanModel;
        }

        /// <summary>
        /// 对查询出的数据进行处理，处理成前台可绑定的集合
        /// </summary>
        /// <param name="ar"></param>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">截止时间</param>
        private void DoWithData(XlsAttendWuHuShanPersonList ar,DateTime beginTime,DateTime endTime)
        { 
            foreach (XlsAttendWuHuShan xa in XlsAttendDataGridWuHuShanModel)
            {
                if (xa.PersonId == ar.PersonId)
                {
                    for (int i = 0; i < xa.AttendDays.Count; i++)
                    {
                        if (xa.AttendDays[i].ToShortDateString() == ar.AttendDay.ToShortDateString())
                        {
                            if (xa.AtteinTime[i] != "1")
                            {
                                xa.AtteinTime[i] = "1";

                                xa.IrisWorkTimeList[i] = ar.IrisWorkTime;
                                xa.LocateWorkTimeList[i] = ar.LocateWorkTime;
                                xa.InWellTimeList[i] = ar.InWellTime;
                                xa.OutWellTimeList[i] = ar.OutWellTime;
                                xa.InLocateTimeList[i] = ar.InLocateTime;
                                xa.OutLocateTimeList[i] = ar.OutLocateTime;
                                
                                xa.ClassOrderNameList[i] = ar.ClassOrderName;

                                if ((ar.ClassOrderName).Contains("早班"))
                                {
                                    xa.MoringAttendTime++;
                                    xa.MoringAttendTimes = xa.MoringAttendTime.ToString();
                                }
                                else if ((ar.ClassOrderName).Contains("中班"))
                                {
                                    xa.MiddleAttendTime++;
                                    xa.MiddleAttendTimes = xa.MiddleAttendTime.ToString();
                                }
                                else if ((ar.ClassOrderName).Contains("夜班"))
                                {
                                    xa.NightAttendTime++;
                                    xa.NightAttendTimes = xa.NightAttendTime.ToString();
                                }
                                xa.SumAttendTimes++;
                            }
                            else //一天只统计一个工按夜班、早班、中班优先级来算，
                            {
                                if ((ar.ClassOrderName).Contains("夜班"))
                                {

                                    if (xa.ClassOrderNameList[i].Contains("早班"))
                                    {
                                        xa.IrisWorkTimeList[i] = ar.IrisWorkTime;
                                        xa.LocateWorkTimeList[i] = ar.LocateWorkTime;
                                        xa.InWellTimeList[i] = ar.InWellTime;
                                        xa.OutWellTimeList[i] = ar.OutWellTime;
                                        xa.InLocateTimeList[i] = ar.InLocateTime;
                                        xa.OutLocateTimeList[i] = ar.OutLocateTime;

                                        xa.ClassOrderNameList[i] = ar.ClassOrderName;
                                        xa.NightAttendTime++;
                                        xa.NightAttendTimes = xa.NightAttendTime.ToString();
                                        xa.MoringAttendTime--;
                                        xa.MoringAttendTimes = xa.MoringAttendTime.ToString();
                                    }
                                    else if ((xa.ClassOrderNameList[i]).Contains("中班"))
                                    {
                                        xa.IrisWorkTimeList[i] = ar.IrisWorkTime;
                                        xa.LocateWorkTimeList[i] = ar.LocateWorkTime;
                                        xa.InWellTimeList[i] = ar.InWellTime;
                                        xa.OutWellTimeList[i] = ar.OutWellTime;
                                        xa.InLocateTimeList[i] = ar.InLocateTime;
                                        xa.OutLocateTimeList[i] = ar.OutLocateTime;
                                        xa.ClassOrderNameList[i] = ar.ClassOrderName;
                                        xa.NightAttendTime++;
                                        xa.NightAttendTimes = xa.NightAttendTime.ToString();
                                        xa.MiddleAttendTime--;
                                        xa.MiddleAttendTimes = xa.MiddleAttendTime.ToString();
                                    }
                                }
                                else if ((ar.ClassOrderName).Contains("早班"))
                                {
                                    if ((xa.ClassOrderNameList[i]).Contains("中班"))
                                    {
                                        xa.IrisWorkTimeList[i] = ar.IrisWorkTime;
                                        xa.LocateWorkTimeList[i] = ar.LocateWorkTime;
                                        xa.InLocateTimeList[i] = ar.InLocateTime;
                                        xa.OutLocateTimeList[i] = ar.OutLocateTime;
                                        xa.InWellTimeList[i] = ar.InWellTime;
                                        xa.OutWellTimeList[i] = ar.OutWellTime;
                                        xa.ClassOrderNameList[i] = ar.ClassOrderName;
                                        xa.MoringAttendTime++;
                                        xa.MoringAttendTimes = xa.MoringAttendTime.ToString();
                                        xa.MiddleAttendTime--;
                                        xa.MiddleAttendTimes = xa.MiddleAttendTime.ToString();
                                    }
                                }
                            }
                        }
                    }
                    
                    return;
                }
            }

            XlsAttendWuHuShan xd = new XlsAttendWuHuShan();
            xd.IrisWorkTimeList = new List<TimeSpan?>();
            xd.LocateWorkTimeList = new List<TimeSpan?>();
            xd.AttendDays = new List<DateTime>();
            xd.AtteinTime = new List<string>();
            xd.InWellTimeList = new List<DateTime?>();
            xd.OutWellTimeList = new List<DateTime?>();
            xd.InLocateTimeList = new List<DateTime?>();
            xd.OutLocateTimeList = new List<DateTime?>();
            xd.ClassOrderNameList = new List<string>();

            xd.PersonId = ar.PersonId;
            xd.WorkSn = ar.WorkSn;
            xd.WorkType = ar.WorkType;
            xd.PrincipalName = ar.Principal;
            xd.PersonName = ar.PersonName;
            xd.DepartName = ar.DepartName;
            xd.ClassOrderName = ar.ClassOrderName;
            xd.ClassTypeName = ar.ClassTypeName;

            for (DateTime m_beginTime = beginTime; m_beginTime < endTime; m_beginTime = m_beginTime.AddDays(1))
            {
                xd.AttendDays.Add(m_beginTime);
                xd.IrisWorkTimeList.Add(TimeSpan.FromHours(0));
                xd.LocateWorkTimeList.Add(TimeSpan.FromHours(0));
                xd.AtteinTime.Add("");
                xd.InWellTimeList.Add(DateTime.MinValue);
                xd.OutWellTimeList.Add(DateTime.MinValue);
                xd.InLocateTimeList.Add(DateTime.MinValue);
                xd.OutLocateTimeList.Add(DateTime.MinValue);
                xd.ClassOrderNameList.Add("");
            }
            for (int i = 0; i < xd.AttendDays.Count; i++)
            {
                if (xd.AttendDays[i].ToShortDateString() == ar.AttendDay.ToShortDateString())
                {
                    if(xd.AtteinTime[i]!="1")
                    {
                        xd.AtteinTime[i] = "1";
                        xd.AttendTimes = 1;

                        xd.IrisWorkTimeList[i] = ar.IrisWorkTime;
                        xd.LocateWorkTimeList[i] = ar.LocateWorkTime;
                        xd.InLocateTimeList[i] = ar.InLocateTime;
                        xd.OutLocateTimeList[i] = ar.OutLocateTime;
                        xd.InWellTimeList[i] = ar.InWellTime;
                        xd.OutWellTimeList[i] = ar.OutWellTime;
                        xd.ClassOrderNameList[i] = ar.ClassOrderName;

                        if ((ar.ClassOrderName).Contains("早班"))
                        {
                            xd.MoringAttendTime++;
                            xd.MoringAttendTimes = xd.MoringAttendTime.ToString();
                        }
                        else if ((ar.ClassOrderName).Contains("中班"))
                        {
                            xd.MiddleAttendTime++;
                            xd.MiddleAttendTimes = xd.MiddleAttendTime.ToString();
                        }
                        else if ((ar.ClassOrderName).Contains("夜班"))
                        {
                            xd.NightAttendTime++;
                            xd.NightAttendTimes = xd.NightAttendTime.ToString();
                        }
                    }
                   
                }
            }
            xd.SumAttendTimes += xd.AttendTimes;

            this.XlsAttendDataGridWuHuShanModel.Add(xd);
        }

        #endregion

        #region 获取显示详细信息的数据集合

        /// <summary>
        /// 获取显示详细信息的数据集合
        /// </summary>
        /// <param name="personId">人员的personID</param>
        /// <param name="id">链表中的索引</param>
        /// <returns></returns>
        public BaseViewModelCollection<XlsAttendWuHuShan> ShowDetail(int personId, int id)
        {
            ShowDetailsModel.Clear();
            foreach(XlsAttendWuHuShan ar in XlsAttendDataGridWuHuShanModel)
            {
                if (ar.PersonId == personId)
                {
                    XlsAttendWuHuShan xa = new XlsAttendWuHuShan();

                    xa.AttendDay = ar.AttendDays[id];
                    xa.WorkSn = ar.WorkSn;
                    xa.PersonName = ar.PersonName;
                    xa.DepartName = ar.DepartName;
                    xa.WorkType = ar.WorkType;
                    xa.PrincipalName = ar.PrincipalName;
                    xa.ClassTypeName = ar.ClassTypeName;
                    xa.ClassOrderName=ar.ClassOrderNameList[id];
                    xa.InWellTime = ar.InWellTimeList[id];
                    xa.OutWellTime = ar.OutWellTimeList[id];
                    xa.InLocateTime = ar.InLocateTimeList[id];
                    xa.OutLocateTime = ar.OutLocateTimeList[id];
                    xa.IrisWorkTimeStr = ar.IrisWorkTimeList[id].Value.ToString();
                    xa.LocateWorkTimeStr = ar.LocateWorkTimeList[id].Value.ToString();
                  
                    ShowDetailsModel.Add(xa);
                }
            }
            
            return ShowDetailsModel;
        }

        #endregion
    }

    #region 数据绑定类

    /// <summary>
    /// 数据绑定的实体类
    /// </summary>
    public class XlsAttendWuHuShan : INotifyPropertyChanged
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
        /// 部门名称
        /// </summary>
        public string DepartName { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string PersonName { get; set; }

        /// <summary>
        /// 工号
        /// </summary>
        public string WorkSn { get; set; }

        /// <summary>
        /// 工种
        /// </summary>
        public string WorkType { get; set; }

        /// <summary>
        /// 职务
        /// </summary>
        public string PrincipalName { get; set; }

        /// <summary>
        /// 班次名称
        /// </summary>
        public string ClassOrderName { get; set; }

        /// <summary>
        /// 班制名称
        /// </summary>
        public string ClassTypeName { get; set; }       

        /// <summary>
        /// 工数
        /// </summary>
        public int AttendTimes { get; set; }

        /// <summary>
        /// 总工数
        /// </summary>
        public int SumAttendTimes { get; set; }

        /// <summary>
        /// 工数连表
        /// </summary>
        public List<string> AtteinTime { get; set; }

        /// <summary>
        /// 夜班工数
        /// </summary>
        public string NightAttendTimes { get;set;}
        public int NightAttendTime { get; set; }

        /// <summary>
        /// 早班工数
        /// </summary>
        public string MoringAttendTimes { get; set; }
        public int MoringAttendTime { get; set; }

        /// <summary>
        /// 中班工数
        /// </summary>
        public string MiddleAttendTimes { get;set;}
        public int MiddleAttendTime { get; set; }

        /// <summary>
        /// 归属日
        /// </summary>
        public DateTime? AttendDay { get; set; }

        /// <summary>
        /// 归属日链表
        /// </summary>
        public List<DateTime> AttendDays { get; set; }

        #region 详细信息绑定使用的属性
        
        /// <summary>
        /// 工作时长
        /// </summary>
        public int WorkTime { get; set; }

        /// <summary>
        /// 工作时长 **小时**分钟
        /// </summary>
        public string IrisWorkTimeStr { get; set; }

        /// <summary>
        /// 工作时长 **小时**分钟
        /// </summary>
        public string LocateWorkTimeStr { get; set; }

        /// <summary>
        /// 上班时间
        /// </summary>
        public DateTime? InWellTime { get; set; }

        /// <summary>
        /// 下班时间
        /// </summary>
        public DateTime? OutWellTime { get; set; }

        /// <summary>
        /// 上班时间
        /// </summary>
        public DateTime? InLocateTime { get; set; }

        /// <summary>
        /// 下班时间
        /// </summary>
        public DateTime? OutLocateTime { get; set; }

        /// <summary>
        /// 虹膜工时链表
        /// </summary>
        public List<TimeSpan?> IrisWorkTimeList { get; set; }

        /// <summary>
        /// 定位工时链表
        /// </summary>
        public List<TimeSpan?> LocateWorkTimeList { get; set; }

        /// <summary>
        /// 上班时间链表
        /// </summary>
        public List<DateTime?> InWellTimeList { get; set; }

        /// <summary>
        /// 下班时间链表
        /// </summary>
        public List<DateTime?> OutWellTimeList { get; set; }

        /// <summary>
        /// 上班时间链表
        /// </summary>
        public List<DateTime?> InLocateTimeList { get; set; }

        /// <summary>
        /// 下班时间链表
        /// </summary>
        public List<DateTime?> OutLocateTimeList { get; set; }

        /// <summary>
        /// 班次链表
        /// </summary>
        public List<string> ClassOrderNameList { get; set; }
        #endregion

        /// <summary>
        /// 属性更改时的事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 构造函数
        /// </summary>
        public XlsAttendWuHuShan()
        {
            NightAttendTime = 0;
            MoringAttendTime = 0;
            MiddleAttendTime = 0;

            NightAttendTimes = "";
            MoringAttendTimes = "";
            MiddleAttendTimes = "";

            SumAttendTimes = 0;
        }

    }

    #endregion
}
