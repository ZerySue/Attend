/*************************************************************************
** 文件名:   Query.cs
×× 主要类:   Query
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   lzc
** 日  期:   2013-4-8
** 修改人:   
** 日  期:
** 描  述:   Query类，查询界面
**
** 版  本:   1.0.0
** 备  注:  命名及代码编写遵守C#编码规范
**
 * ***********************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Navigation;
using System.Diagnostics;
using System.Threading;
using System.Globalization;
using System.IO.IsolatedStorage;
using IriskingAttend.Web;
using IriskingAttend.ViewModel;
using Irisking.Web.DataModel;
using System.ServiceModel.DomainServices.Client;
using EDatabaseError;
using ReportTemplate;

namespace IriskingAttend.View.ReportView
{
    public partial class ReportQuery : Page
    {
        BaseViewModelCollection<UserAttendRec> AttendRecModel;
        private VmDepartment m_Depart = new VmDepartment();
        private VmLeaveType m_LeaveType = new VmLeaveType();
        private string m_Url = "/AttendRecord";
        /// <summary>
        /// 构造函数
        /// </summary>
        public ReportQuery()
        {
            InitializeComponent();
            SetInit();
           
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ReportQuery(string urlStr)
        {
            InitializeComponent();
            m_Url = urlStr;
            SetInit();
        }

        private void SetInit()
        {
            this.btnQuery.IsEnabled = false;
            m_Depart.GetDepartment();
            m_Depart.DepartmentLoadCompleted += delegate
            {
                m_LeaveType.GetLeaveType(0);
            };
            this.listBoxDepartment.ItemsSource = m_Depart.DepartmentModel;
            this.listBoxDepartment.DisplayMemberPath = "depart_name";

            this.listBoxAttendType.ItemsSource = m_LeaveType.LeaveTypeModel;
            this.listBoxAttendType.DisplayMemberPath = "leave_type_name";
        }
        /// <summary>
        /// 点击<查询>按钮后，跳转的页面
        /// </summary>
        private string _nextPage = string.Empty;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // 样例：/Query?Action=/SafeManager/InWellPersonQuery&NoDateTime&NoMine
            //string strTip = string.Empty;
            foreach (var item in this.NavigationContext.QueryString)
            {
                //strTip = string.Format("{0} = {1}\n", item.Key, item.Value);
                if (item.Key == "Action")
                {
                    _nextPage = item.Value;
                }
                else if (item.Key == "Title")
                {
                    lblTitle.Content = "  " + item.Value;
                    if (item.Value == "领导考勤")
                    {
                        //vmPerson.SelectPrincipalData(2);
                    }
                    else if (item.Value == "关键岗位考勤")
                    {
                        // vmPerson.SelectPrincipalData(1);
                    }//
                    else if (item.Value == "个人考勤")
                    {
                        //0为普通
                        // vmPerson.SelectPrincipalData(-1);
                    }

                }
                else if (string.IsNullOrEmpty(item.Value))
                {
                    //if (dictShower.ContainsKey(item.Key))
                    //    dictShower[item.Key]();
                }
            }
        }


        private IsolatedStorageSettings querySetting = IsolatedStorageSettings.ApplicationSettings;//本地独立存储，用来传参
        //    /// <summary>
        //    /// 查询
        //    /// </summary>
        //    /// <param name="sender"></param>
        //    /// <param name="e"></param>
        private void btnQuery_Click(object sender, RoutedEventArgs e)
        {
            SetCondition();
            GetAttend();
        }

        private void SetCondition()
        {
            AttendQueryCondition qc = new AttendQueryCondition();
            if (this.dateBegin.SelectedDate != null)
            {
                qc.BeginTime = this.dateBegin.SelectedDate.Value.Date;
                if (this.timeBegin.Value != null)
                    qc.BeginTime += this.timeBegin.Value.Value.TimeOfDay;
            }

            if (this.dateEnd.SelectedDate != null)
            {
                qc.EndTime = this.dateEnd.SelectedDate.Value.Date;
                if (this.timeEnd.Value != null)
                    qc.EndTime += this.timeEnd.Value.Value.TimeOfDay;
            }
            //qc.nextPageTitle = lblTitle.Content;
            qc.Name = txtBoxName.Text;
            qc.WorkSN = txtBoxWorkSn.Text;
            if (listBoxDepartment.SelectedItems.Count > 0)
            {
                qc.DepartIdLst = new int[listBoxDepartment.SelectedItems.Count];
                for (int i = 0; i < listBoxDepartment.SelectedItems.Count; i++)
                {
                    qc.DepartIdLst[i] = (listBoxDepartment.SelectedItems[i] as depart).depart_id;
                }
            }

            if (listBoxAttendType.SelectedItems.Count > 0)
            {
                qc.AttendTypeIdLst = new int[listBoxAttendType.SelectedItems.Count];
                for (int i = 0; i < listBoxAttendType.SelectedItems.Count; i++)
                {
                    qc.AttendTypeIdLst[i] = (listBoxAttendType.SelectedItems[i] as leave_type).leave_type_id;
                }
            }

            ///如果存在 先删除该键值对
            if (querySetting.Contains("attendConditon"))
            {
                querySetting.Remove("attendConditon");
            }
            querySetting.Add("attendConditon", qc);
        }

        /// <summary>
        /// 考勤查询
        /// </summary>
        void GetAttend()
        {
            ///MEF方式实现条件传值
            if (querySetting.Contains("attendConditon"))
            {
                try
                {
                    AttendQueryCondition condition;
                    querySetting.TryGetValue<AttendQueryCondition>("attendConditon", out condition);

                    EntityQuery<UserAttendRec> list = ServiceDomDbAcess.GetSever().IrisGetAttendRecQuery(condition.BeginTime, condition.EndTime, condition.DepartIdLst,
                        condition.AttendTypeIdLst,condition.DevTypeIdLst, condition.Name, condition.WorkSN);

                    ///回调异常类
                    Action<LoadOperation<UserAttendRec>> actionCallBack = ErrorHandle<UserAttendRec>.OnLoadErrorCallBack;
                    ///异步事件
                    LoadOperation<UserAttendRec> lo = ServiceDomDbAcess.GetSever().Load(list, actionCallBack, null);

                    lo.Completed += delegate
                    {
                        this.AttendRecModel = new BaseViewModelCollection<UserAttendRec>();
                        try
                        {
                            //异步获取数据
                            foreach (UserAttendRec ar in lo.Entities)
                            {
                                this.AttendRecModel.Add(ar);
                            }

                            
                        }
                        catch (Exception e)
                        {
                            ChildWindow errorWin = new ErrorWindow("加载异常", e.Message);
                            errorWin.Show();
                        }
                        GenerateData(AttendRecModel, condition.BeginTime, condition.EndTime);
                    };
                }
                catch (Exception e)
                {
                    ErrorWindow err = new ErrorWindow(e);
                    err.Show();
                }

            }
            else
            {
                ChildWindow errorWin = new ErrorWindow("非法请求", "查询条件不准确！");
                errorWin.Show();
            }
        }

        /// <summary>
        /// 产生报表并预览
        /// </summary>
        /// <param name="dataCollection"></param>
        void GenerateData(BaseViewModelCollection<UserAttendRec> dataCollection,DateTime beginTime,DateTime endTime)
        {
            #region     生成表头测试数据
            List<HeaderNode> PageHeaderData = new List<HeaderNode>();

            PageHeaderData.Add(new HeaderNode("职工工号", 14, 0));
            PageHeaderData.Add(new HeaderNode("职工姓名", 14, 0));
            PageHeaderData.Add(new HeaderNode("部门名称", 14, 0));
            PageHeaderData.Add(new HeaderNode("出勤次数", 14, 0));
            PageHeaderData.Add(new HeaderNode("总工时", 14, 0));
            PageHeaderData.Add(new HeaderNode("平均工时", 14, 0));
         

            ReportHeader _ReportHeader = new ReportHeader(PageHeaderData);
            #endregion


            #region     生成报表标题测试数据
            string time = beginTime.ToLongDateString() + " 到 " + endTime.ToShortDateString();

            ReportTitle reportTitle = new ReportTitle("考勤汇总表", 20, time, 11, false);
            #endregion

            #region     生成报表页脚测试数据
            ReportFooter reportFooter = new ReportFooter("", 11);
            #endregion

            string[] bindingPropertyNames = new string[] { 
                "work_sn",
                "person_name",
                "depart_name",
                "sum_count",
                "sum_work_time",
                "avg_work_time",
               };

            PrintControl printControl = new PrintControl();
     

            if (printControl.SetDataSource<UserAttendRec>(reportTitle, reportFooter, _ReportHeader, dataCollection, bindingPropertyNames))
            {
                printControl.Preview_CurPage(null, null);
            }
           
           
            
        }

        /// <summary>
        /// 设置DataPicker 日期的选择范围
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dateBegin_CalendarClosed(object sender, RoutedEventArgs e)
        {
            if (dateBegin.SelectedDate != null)
            {
                this.dateEnd.BlackoutDates.Clear();
                this.dateEnd.SelectedDate = null;
                int frontYear = dateBegin.SelectedDate.Value.AddDays(-1).Year;
                int frontMonth = dateBegin.SelectedDate.Value.AddDays(-1).Month;
                int frontDay = dateBegin.SelectedDate.Value.AddDays(-1).Day;
                int backYear = dateBegin.SelectedDate.Value.AddMonths(1).Year;
                int backMonth = dateBegin.SelectedDate.Value.AddMonths(1).Month;
                int backDay = dateBegin.SelectedDate.Value.AddMonths(1).Day;

                this.dateEnd.BlackoutDates.Add(new CalendarDateRange(new DateTime(0001, 1, 1), new DateTime
                (frontYear, frontMonth, frontDay)));
                this.dateEnd.BlackoutDates.Add(new CalendarDateRange(new DateTime
                (backYear, backMonth, backDay), new DateTime(2100, 1, 1)));
                dateEnd.DisplayDate = dateBegin.SelectedDate.Value;

                //暂时先写一个控件，如需要写两个
                this.btnQuery.IsEnabled = true;
            }
        }

        /// <summary>
        /// 选择所有部门
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkSelectAll_Click(object sender, RoutedEventArgs e)
        {
            //vmPerson.SelectAllDepart(sender as CheckBox);
        }

        /// <summary>
        /// 校验日期格式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dateBegin_DateValidationError(object sender, DatePickerDateValidationErrorEventArgs e)
        {
            MessageBox.Show("日期格式不对！", "提示", MessageBoxButton.OK);
            //this.btnQuery.IsEnabled = false;
        }

        /// <summary>
        /// 防止用户选择日期后 再将日期删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dateBegin_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back || e.Key == Key.Delete)
            {
                this.btnQuery.IsEnabled = false;
            }
            else
            {
                this.btnQuery.IsEnabled = true;
            }
        }

        private void dateEnd_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (null != dateBegin.SelectedDate)
            {
                this.btnQuery.IsEnabled = true;
            }
            else
            {
                this.btnQuery.IsEnabled = false;
            }
        }

    }
}
