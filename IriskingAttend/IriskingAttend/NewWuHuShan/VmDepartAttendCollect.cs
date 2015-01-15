/*************************************************************************
** 文件名:   VmDepartAttendCollect.cs
** 主要类:   VmDepartAttendCollect
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2013-11-22
** 修改人:   
** 日  期:
** 描  述:   VmDepartAttendCollect，五虎山部门汇总报表
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
using Irisking.Web.DataModel;

namespace IriskingAttend
{
    #region 数据绑定类

    /// <summary>
    /// 数据绑定的实体类
    /// </summary>
    public class DepartAttend : BaseViewModel
    {
        private string _index;
        /// <summary>
        /// 前台绑定序号
        /// </summary>
        public string Index
        {
            get
            {
                return _index;
            }
            set
            {
                _index = value;
                OnPropertyChanged<string>(() => this.Index);
            }
        }

        private string _departName;
        /// <summary>
        /// 部门名称
        /// </summary>
        public string DepartName
         {
            get
            {
                return _departName;
            }
            set
            {
                _departName = value;
                OnPropertyChanged<string>(() => this.DepartName);
            }
        }

        private int _moringAttendTime;
        /// <summary>
        /// 早班工数
        /// </summary>       
        public int MoringAttendTime
        {
            get
            {
                return _moringAttendTime;
            }
            set
            {
                _moringAttendTime = value;
                OnPropertyChanged<int>(() => this.MoringAttendTime);
            }
        }

        private int _middleAttendTime;
        /// <summary>
        /// 中班工数
        /// </summary>       
        public int MiddleAttendTime
        {
            get
            {
                return _middleAttendTime;
            }
            set
            {
                _middleAttendTime = value;
                OnPropertyChanged<int>(() => this.MiddleAttendTime);
            }
        }

        private int _nightAttendTime;
        /// <summary>
        /// 夜班工数
        /// </summary>       
        public int NightAttendTime
        {
            get
            {
                return _nightAttendTime;
            }
            set
            {
                _nightAttendTime = value;
                OnPropertyChanged<int>(() => this.NightAttendTime);  
            }
        }

        private int _sumAttendTime;
        /// <summary>
        /// 总工数
        /// </summary>
        public int SumAttendTime
        {
            get
            {
                return _sumAttendTime;
            }
            set
            {
                _sumAttendTime = value;
                OnPropertyChanged<int>(() => this.SumAttendTime);
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public DepartAttend()
        {
            NightAttendTime = 0;
            MoringAttendTime = 0;
            MiddleAttendTime = 0;
            SumAttendTime = 0;
        }

    }
    #endregion
    public class VmDepartAttendCollect : BaseViewModel
    {
        #region  常量

        public const int ClassType_None = 0;
        public const int ClassType_Middle = 1;
        public const int ClassType_Morning = 2;
        public const int ClassType_Nignt = 3;

        #endregion

        #region 绑定属性

        private BaseViewModelCollection<DepartAttend> _departAttendModel;

        /// <summary>
        /// 前台界面绑定的数据集合
        /// </summary>
        public BaseViewModelCollection<DepartAttend> DepartAttendModel
        {
            get 
            {
                return _departAttendModel;
            }
            set
            {
                _departAttendModel = value;
                OnPropertyChanged<BaseViewModelCollection<DepartAttend>>(() => this.DepartAttendModel);               
            }
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public VmDepartAttendCollect()
        {
            DepartAttendModel = new BaseViewModelCollection<DepartAttend>();            
        }

        #endregion

        #region 根据查询条件查询数据并对数据进行处理

        private int GetClassType( string classOrderName )
        {
            if (classOrderName.Contains("夜班"))
            {
                return ClassType_Nignt;
            }

            if (classOrderName.Contains("早班"))
            {
                return ClassType_Morning;
            }
            if (classOrderName.Contains("中班"))
            {
                return ClassType_Middle;
            }
            return ClassType_None;
        }

        private void AddPersonAttend(int departIndex, string classOrderName, DepartAttend total)
        {
            int classOrder = GetClassType( classOrderName );

            if (classOrder == ClassType_Nignt)
            {
                DepartAttendModel[departIndex].NightAttendTime++;
                total.NightAttendTime++;

                DepartAttendModel[departIndex].SumAttendTime++;
                total.SumAttendTime++;
            }

            if (classOrder == ClassType_Morning)
            {
                DepartAttendModel[departIndex].MoringAttendTime++;
                total.MoringAttendTime++;

                DepartAttendModel[departIndex].SumAttendTime++;
                total.SumAttendTime++;
            }

            if (classOrder == ClassType_Middle)
            {
                DepartAttendModel[departIndex].MiddleAttendTime++;
                total.MiddleAttendTime++;

                DepartAttendModel[departIndex].SumAttendTime++;
                total.SumAttendTime++;
            }
        }
        
        /// <summary>
        /// 根据查询条件查询数据
        /// </summary>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">截止时间</param>
        /// <param name="departIdLst">部门列表</param>        
        /// <returns></returns>
        public void GetDepartAttendCollect(DateTime beginTime, DateTime endTime, List<UserDepartInfo> departList)
        {    
            //departList.Sort("depart_name", false);
            //departList.Sort();  

            List<int> departIdList = new List<int>();

            //获取选择的部门
            if (departList.Count > 0)
            {
                foreach (UserDepartInfo ar in departList)
                {
                    departIdList.Add( ar.depart_id );
                }
            }
            int[] departIds = departIdList.ToArray();
            if (departIdList.Count == 0)
            {
                departIds = null; 
            }
            WaitingDialog.ShowWaiting("正在查询，请等待");

            ServiceDomDbAcess.ReOpenSever();

            EntityQuery<XlsAttendWuHuShanPersonList> list = ServiceDomDbAcess.GetSever().GetXlsWuHuShanPersonListQuery(beginTime, endTime, departIds, "", "", null, null, 0);
            //回调异常类
            Action<LoadOperation<XlsAttendWuHuShanPersonList>> actionCallBack = new Action<LoadOperation<XlsAttendWuHuShanPersonList>>(ErrorHandle<XlsAttendWuHuShanPersonList>.OnLoadErrorCallBack);
            //异步事件
            LoadOperation<XlsAttendWuHuShanPersonList> loadOp = ServiceDomDbAcess.GetSever().Load(list, actionCallBack, null);
            
            loadOp.Completed += delegate
            {
                try
                {
                    int index = 0;

                    DepartAttendModel.Clear();

                    foreach (UserDepartInfo item in departList)
                    {
                        index++;
                        DepartAttend temp = null;
                        temp = new DepartAttend();
                        temp.DepartName = item.depart_name;
                        temp.Index = index.ToString();
                        DepartAttendModel.Add(temp);
                    }

                    index++;
                    DepartAttend total = new DepartAttend();
                    total.DepartName = "总合计";
                    total.Index = index.ToString();

                    index = 0;


                    Dictionary<int, Dictionary<string, int>> personAttend = new Dictionary<int, Dictionary<string, int>>();

                    foreach (XlsAttendWuHuShanPersonList ar in loadOp.Entities)
                    {   
                        for (; index < DepartAttendModel.Count; )
                        {
                            if (ar.DepartName != DepartAttendModel[index].DepartName)
                            {
                                index++;
                            }
                            else
                            {
                                break;
                            }
                        }

                        if (index >= DepartAttendModel.Count)
                        {
                            break;
                        }

                        string attendDayStr = ar.AttendDay.ToString("yyyy-MM-dd");
                        int classOrder = GetClassType(ar.ClassOrderName);

                        //列表中还无此人员id
                        if (!personAttend.ContainsKey(ar.PersonId) )
                        {
                            Dictionary<string, int> dateClassType = new Dictionary<string, int>();
                            dateClassType.Add(attendDayStr, classOrder);
                            personAttend.Add( ar.PersonId, dateClassType );

                            AddPersonAttend( index, ar.ClassOrderName, total );
                            continue;
                        }

                        if (!personAttend[ar.PersonId].ContainsKey(attendDayStr))
                        {
                            personAttend[ar.PersonId].Add(attendDayStr, classOrder);

                            AddPersonAttend(index, ar.ClassOrderName, total);
                            continue;
                        }

                        if (personAttend[ar.PersonId][attendDayStr] >= classOrder)
                        {
                            continue;
                        }

                        int origClassOrder = personAttend[ar.PersonId][attendDayStr];

                        if (origClassOrder == ClassType_Nignt)
                        {
                            DepartAttendModel[index].NightAttendTime--;
                            total.NightAttendTime--;
                        }

                        if (origClassOrder == ClassType_Morning)
                        {
                            DepartAttendModel[index].MoringAttendTime--;
                            total.MoringAttendTime--;
                        }

                        if (origClassOrder == ClassType_Middle)
                        {
                            DepartAttendModel[index].MiddleAttendTime--;
                            total.MiddleAttendTime--;
                        }

                        if (classOrder == ClassType_Nignt)
                        {
                            DepartAttendModel[index].NightAttendTime++;
                            total.NightAttendTime++;
                        }

                        if (classOrder == ClassType_Morning)
                        {
                            DepartAttendModel[index].MoringAttendTime++;
                            total.MoringAttendTime++;
                        }

                        if (classOrder == ClassType_Middle)
                        {
                            DepartAttendModel[index].MiddleAttendTime++;
                            total.MiddleAttendTime++;
                        }          
                    }

                    DepartAttendModel.Add(total);

                    WaitingDialog.HideWaiting();
                }
                catch (Exception e)
                {
                    ChildWindow errorWin = new ErrorWindow("加载异常", e.Message);
                    errorWin.Show();
                    WaitingDialog.HideWaiting();
                }               
            };            
        }
        
        #endregion
    }
}
