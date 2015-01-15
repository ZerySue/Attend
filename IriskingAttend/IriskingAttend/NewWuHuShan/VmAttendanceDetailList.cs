/*************************************************************************
** 文件名:   VmAttendanceDetailList.cs
** 主要类:   VmAttendanceDetailList
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   cty
** 日  期:   2013-8-30
** 修改人:   
** 日  期:
** 描  述:   VmAttendanceDetailList，五虎山人员出入井记录表的vm层
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
using System.ComponentModel;
using IriskingAttend.Web;
using IriskingAttend.ViewModel;
using IriskingAttend.Common;
using System.ServiceModel.DomainServices.Client;
using EDatabaseError;
using System.Linq;

namespace IriskingAttend.NewWuHuShan
{
    public class VmAttendanceDetailList
    {
        #region 私有变量声明

        /// <summary>
        /// 域服务声明
        /// </summary>
        private DomainServiceIriskingAttend _serviceDomDbAccess = new DomainServiceIriskingAttend();

        #endregion

        #region 绑定属性

        /// <summary>
        /// 数据集合
        /// </summary>
        public BaseViewModelCollection<XlsAttendanceDetailList> XlsAttendancdDetailListModel
        {
            get;
            set;
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public VmAttendanceDetailList()
        {
            XlsAttendancdDetailListModel=new BaseViewModelCollection<XlsAttendanceDetailList>();
        }

        #endregion

        #region 根据查询条件查询数据

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
        /// <param name="workTimeMore">工作时长大于</param>
        /// <param name="workTimeEqual">工作时长等于</param>
        /// <param name="workTimeLess">工作时长小于</param>
        /// <returns></returns>
        public BaseViewModelCollection<XlsAttendanceDetailList> GetXlsWuHuShanAttendanceDetailList(DateTime beginTime, DateTime endTime, int[] departIdLst, string name, string workSn, int[] principalIdList, int[] workTypeIdList, int workTimeMore, int workTimeEqual, int workTimeLess)
        {
            WaitingDialog.ShowWaiting();
            _serviceDomDbAccess = new DomainServiceIriskingAttend();
            XlsAttendancdDetailListModel.Clear();

            EntityQuery<XlsAttendWuHuShanPersonList> list = _serviceDomDbAccess.GetXlsWuHuShanAttendanceDetailListQuery(beginTime, endTime, departIdLst, name, workSn, principalIdList, workTypeIdList, workTimeMore,workTimeEqual,workTimeLess);
            //回调异常类
            Action<LoadOperation<XlsAttendWuHuShanPersonList>> actionCallBack = new Action<LoadOperation<XlsAttendWuHuShanPersonList>>(ErrorHandle<XlsAttendWuHuShanPersonList>.OnLoadErrorCallBack);
            //异步事件
            LoadOperation<XlsAttendWuHuShanPersonList> lo = _serviceDomDbAccess.Load(list, actionCallBack, null);

            lo.Completed += delegate
            {
                try
                {
                    int num=1;
                    //异步获取数据
                    foreach (XlsAttendWuHuShanPersonList ar in lo.Entities)
                    {
                        XlsAttendanceDetailList adl = new XlsAttendanceDetailList();

                        if (ar.WorkTime != 0)
                        {
                            adl.WorkTimeInt = ar.WorkTime;
                            if (ar.WorkTime % 60 > 9)
                            {
                                adl.WorkTime = (ar.WorkTime / 60).ToString() + "小时" + (ar.WorkTime % 60).ToString() + "分钟";
                            }
                            else
                            {
                                adl.WorkTime = (ar.WorkTime / 60).ToString() + "小时" + (ar.WorkTime % 60).ToString() + "分钟";
                            }
                        }
                        adl.Index = num++;
                        adl.AttendRecordId = ar.AttendRecordId;
                        adl.PersonId = ar.PersonId;
                        adl.DepartName = ar.DepartName;
                        adl.PersonName = ar.PersonName;
                        adl.WorkSn = ar.WorkSn;
                        adl.WorkType = ar.WorkType;
                        adl.PrincipalName = ar.Principal;
                        adl.InWellTime = ar.InWellTime;
                        adl.OutWellTime = ar.OutWellTime;

                        XlsAttendancdDetailListModel.Add(adl);                     
                    }

                    #region 重新排序
                   
                    IOrderedEnumerable<XlsAttendanceDetailList> sortedObject = XlsAttendancdDetailListModel.OrderBy(a => a.AttendDay).OrderBy(a => a.InWellTime).OrderBy(a => a.PersonName).OrderBy(a => a.DepartName);
                    XlsAttendanceDetailList[] sortedData = sortedObject.ToArray();  //执行这一步之后排序的linq表达式才有效

                    XlsAttendancdDetailListModel.Clear();
                    foreach (var item in sortedData)
                    {
                        XlsAttendancdDetailListModel.Add(item);
                    }

                    #endregion
                }
                catch (Exception e)
                {
                    ChildWindow errorWin = new ErrorWindow("加载异常", e.Message);
                    errorWin.Show();
                }
                WaitingDialog.HideWaiting();
            };
          
            return XlsAttendancdDetailListModel;
        }

        #endregion
    }

    #region 数据绑定类

    /// <summary>
    /// 数据绑定的实体类
    /// </summary>
    public class XlsAttendanceDetailList : INotifyPropertyChanged
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
        /// 归属日
        /// </summary>
        public DateTime AttendDay { get; set; }

        /// <summary>
        /// 工作时长 int型，用于排序
        /// </summary>
        public int WorkTimeInt { get; set; }

        /// <summary>
        /// 工作时长
        /// </summary>
        public string WorkTime { get; set; }

        /// <summary>
        /// 上班时间
        /// </summary>
        public DateTime? InWellTime { get; set; }

        /// <summary>
        /// 下班时间
        /// </summary>
        public DateTime? OutWellTime { get; set; }

        /// <summary>
        /// 属性更改时的事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

    }

    #endregion
}
