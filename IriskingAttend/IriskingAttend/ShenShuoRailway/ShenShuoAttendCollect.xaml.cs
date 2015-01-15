/*************************************************************************
** 文件名:   ShenShuoAttendCollect.cs
×× 主要类:   ShenShuoAttendCollect
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2013-12-15
** 修改人:   
** 日  期:   
** 描  述:   ShenShuoAttendCollect类，神朔铁路考勤查询界面
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
using IriskingAttend.Dialog;
using IriskingAttend.ViewModel;
using Irisking.Web.DataModel;
using System.IO.IsolatedStorage;
using Lite.ExcelLibrary.SpreadSheet;
using System.IO;
using IriskingAttend.Common;
using ReportTemplate;
using System.Windows.Browser;
using IriskingAttend.NewWuHuShan;
using IriskingAttend.Web;
using IriskingAttend.ApplicationType;

namespace IriskingAttend.ShenShuoRailway
{
    public partial class ShenShuoAttendCollect : Page
    {
        #region 私有变量

        private VmPersonAttendCollect _vmPersonAttendCollect = new VmPersonAttendCollect();

        /// <summary>
        /// 报表DataGrid起始插入日期列--报表定制
        /// </summary>
        private const int _colInsert = 4;

        /// <summary>
        /// 报表DataGrid最大日期列--报表定制
        /// </summary>
        private const int _colMax = 35;

        /// <summary>
        /// 本地存储
        /// </summary>
        private IsolatedStorageSettings _querySetting = IsolatedStorageSettings.ApplicationSettings;

        /// <summary>
        /// 私有变量声明
        /// </summary>
        VmXlsFilter _vmFilter = new VmXlsFilter();

        #endregion

        #region 构造函数 初始化

        /// <summary>
        /// 构造函数
        /// </summary>
        public ShenShuoAttendCollect()
        {
            InitializeComponent();

            dtpBegin.Text = DateTime.Now.ToString("yyyy-MM-01");
            dtpEnd.Text = Convert.ToDateTime(dtpBegin.Text).AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");

            //打印预览和导出Excel的权限            
            if (!VmLogin.DictPrivilege[AbstractApp.PrivilegeENUM.ReportShenShuoAttendCollectExportExcel])
            {
                this.btnExportExcel.Visibility = Visibility.Collapsed;
            }

            if (!VmLogin.DictPrivilege[AbstractApp.PrivilegeENUM.ReportShenShuoAttendCollectPrint])
            {
                this.btnPrint.Visibility = Visibility.Collapsed;
            }

            this.DataContext = _vmPersonAttendCollect;

            _vmFilter.GetDepartmentByPrivilege();

            //部门信息加载完成
            _vmFilter.DepartLoadCompleted += delegate
            {
            };
        }
        #endregion

        #region 点击查询 获取查询条件

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQuery_Click(object sender, RoutedEventArgs e)
        {
            dgAttendPersonList.ItemsSource = _vmPersonAttendCollect.TotalAttendModel;

            #region 判断查询时间
            if (dtpBegin.Text == null || dtpBegin.Text == "")
            {
                MsgBoxWindow.MsgBox(
                            "查询开始时间不能为空！",
                            Dialog.MsgBoxWindow.MsgIcon.Information,
                            Dialog.MsgBoxWindow.MsgBtns.OK);
                return;
            }
            DateTime dateBegin = dtpBegin.SelectedDate.Value.Date;
            DateTime dateEnd;

            if (dtpEnd.Text == null || dtpEnd.Text == "")
            {
                dateEnd = dateBegin.AddMonths(1);
                if (dateEnd > System.DateTime.Now)
                {
                    dateEnd = System.DateTime.Now.Date.AddDays(1);
                }
            }
            else 
            {
                if (dateBegin >= dtpEnd.SelectedDate.Value.Date.AddDays(1))
                {
                    MsgBoxWindow.MsgBox(
                                "请确定您的开始时间早于截止时间！",
                                Dialog.MsgBoxWindow.MsgIcon.Information,
                                Dialog.MsgBoxWindow.MsgBtns.OK);
                    return;
                }
                else if (dateBegin.AddMonths(1) < dtpEnd.SelectedDate.Value.Date.AddDays(1))
                {
                    MsgBoxWindow.MsgBox(
                                 "请确定您的查询跨度时间为一个月！",
                                  Dialog.MsgBoxWindow.MsgIcon.Information,
                                  Dialog.MsgBoxWindow.MsgBtns.OK);
                    return;
                }
                else
                {
                    dateEnd = dtpEnd.SelectedDate.Value.Date.AddDays(1);
                }
            }
            #endregion            

            SetdgAttendPersonListHeader(dateBegin, dateEnd);

            SetCondition(dateBegin, dateEnd);

            GetAttend();
        }

        /// <summary>
        /// 设置datagrid的Header
        /// </summary>
        /// <param name="dateBegin">查询开始时间</param>
        /// <param name="dateEnd">查询截止时间</param>
        private void SetdgAttendPersonListHeader(DateTime dateBegin, DateTime dateEnd)
        {
            int col = _colInsert;
            for (dateBegin = (DateTime)dtpBegin.SelectedDate; dateBegin < dateEnd; dateBegin = dateBegin.AddDays(1))
            {
                dgAttendPersonList.Columns[col].Header = dateBegin.Day.ToString("d2");               
                dgAttendPersonList.Columns[col++].Visibility = System.Windows.Visibility.Visible;
            }
            for (int i = col; i < _colMax; i++)
            {
                dgAttendPersonList.Columns[i].Visibility = System.Windows.Visibility.Collapsed;
            }     
        }

        /// <summary>
        /// 获取查询条件
        /// </summary>
        /// <param name="dateBegin"></param>
        /// <param name="dateEnd"></param>
        private void SetCondition(DateTime dateBegin, DateTime dateEnd)
        {
            QueryCondition qc = new QueryCondition();
            qc.BeginTime = dateBegin;
            qc.EndTime = dateEnd;           

            //获取选择的部门
            if (txtDepart.Text != "")
            {
                qc.DepartNameList = txtDepart.Text.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            }
            else
            {
                List<string> departList = new List<string>();

                if (_vmFilter.DepartInfoModel.Count > 0)
                {
                    foreach (UserDepartInfo ud in _vmFilter.DepartInfoModel)
                    {                       
                       departList.Add(ud.depart_name);                      
                    }
                }               
                qc.DepartNameList = departList.ToArray();
            }

            //获取选择的人员姓名
            if (txtPersonName.Text != "")
            {
                qc.PersonNameList = txtPersonName.Text.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            }

            //获取选择的人员工号
            if (txtWorkSn.Text != "")
            {
                qc.WorkSnList = txtWorkSn.Text.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            }

            ///如果存在 先删除该键值对
            if (_querySetting.Contains("attendConditon"))
            {
                _querySetting.Remove("attendConditon");
            }
            _querySetting.Add("attendConditon", qc);
        }

        #region 选取部门、职务、工种

        private void btnSelectDepart_Click(object sender, RoutedEventArgs e)
        {
            List<UserDepartInfo> departList = new List<UserDepartInfo>();
            string[] selectDepart = new string[0];

            //获取选择的部门
            if (txtDepart.Text != "")
            {
                selectDepart = txtDepart.Text.Split( new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            }
            if (_vmFilter.DepartInfoModel.Count > 0)
            {
                foreach (UserDepartInfo ud in _vmFilter.DepartInfoModel)
                {
                    if (selectDepart.Contains( ud.depart_name ) )
                    {
                        departList.Add( ud );
                    }
                }
            }
            SelectDepartPrivilege sd = new SelectDepartPrivilege(departList, ChildWnd_SelectDepart_CallBack);
            sd.Show();
        }

        /// <summary>
        /// 回调函数，得到选取的部门信息
        /// </summary>
        /// <param name="principalSelect"></param>
        private void ChildWnd_SelectDepart_CallBack(BaseViewModelCollection<UserDepartInfo> departSelect)
        {
            string txtShowDepartName = "";            
            foreach (UserDepartInfo ar in departSelect)
            {
                if (ar.isSelected)
                {
                    txtShowDepartName += ar.depart_name + ",";
                }
            }
            if (txtShowDepartName != "")
            {
                txtDepart.Text = txtShowDepartName.Remove(txtShowDepartName.LastIndexOf(","), 1);
            }
            else
            {
                txtDepart.Text = txtShowDepartName;
            }
        }

        private void btnSelectName_Click(object sender, RoutedEventArgs e)
        {
            //添加部门权限部分
            string[] departName = new string[0];
            if (txtDepart.Text != "")
            {
                departName = txtDepart.Text.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            }
            else
            {
                List<string> departNameList = new List<string>();
                foreach (var item in VmLogin.OperatorDepartInfoList)
                {
                    departNameList.Add(item.depart_name);
                }
                departName = departNameList.ToArray();
            }

            //获得人员信息
            _vmPersonAttendCollect.GetPersonInfo(departName, () =>
            {
                _vmPersonAttendCollect.SelectPersonByName();
                
                string[] personList = new string[0];

                //获取选择的部门
                if (txtPersonName.Text != "")
                {
                    personList = txtPersonName.Text.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                }
                if (_vmPersonAttendCollect.SelectPersonInfoModel.Count > 0)
                {
                    foreach (UserPersonInfo personInfo in _vmPersonAttendCollect.SelectPersonInfoModel)
                    {
                        personInfo.isSelected = false;
                        if (personList.Contains(personInfo.person_name))
                        {
                            personInfo.isSelected = true;
                        }
                    }
                }

                SelectPerson selectPerson = new SelectPerson(_vmPersonAttendCollect.SelectPersonInfoModel, ChildWnd_SelectName_CallBack);
                selectPerson.Show();
            });            
        }

        /// <summary>
        /// 回调函数，得到选取的姓名信息
        /// </summary>
        /// <param name="principalSelect"></param>
        private void ChildWnd_SelectName_CallBack(BaseViewModelCollection<UserPersonInfo> personSelect)
        {
            string txtShowPersonName = "";
            foreach (UserPersonInfo ar in personSelect)
            {
                if (ar.isSelected)
                {
                    txtShowPersonName += ar.person_name + ",";
                }
            }
            if (txtShowPersonName != "")
            {
                txtPersonName.Text = txtShowPersonName.Remove(txtShowPersonName.LastIndexOf(","), 1);
            }
            else
            {
                txtPersonName.Text = txtShowPersonName;
            }
        }

        private void btnSelectWorkSn_Click(object sender, RoutedEventArgs e)
        {
            //添加部门权限部分
            string[] departName = new string[0];
            if (txtDepart.Text != "")
            {
                departName = txtDepart.Text.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            }
            else
            {
                List<string> departNameList = new List<string>();
                foreach (var item in VmLogin.OperatorDepartInfoList)
                {
                    departNameList.Add(item.depart_name);
                }
                departName = departNameList.ToArray();
            }

            //获得人员信息
            _vmPersonAttendCollect.GetPersonInfo(departName, () =>
            {
                _vmPersonAttendCollect.SelectPersonByWorkSn();

                string[] personList = new string[0];
                if (txtWorkSn.Text != "")
                {
                    personList = txtWorkSn.Text.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                }
                if (_vmPersonAttendCollect.SelectPersonInfoModel.Count > 0)
                {
                    foreach (UserPersonInfo personInfo in _vmPersonAttendCollect.SelectPersonInfoModel)
                    {
                        personInfo.isSelected = false;
                        if (personList.Contains(personInfo.work_sn))
                        {
                            personInfo.isSelected = true;
                        }
                    }
                }
                SelectPerson selectPerson = new SelectPerson(_vmPersonAttendCollect.SelectPersonInfoModel, ChildWnd_SelectWorkSn_CallBack);
                selectPerson.Show();
            });            
        }

        /// <summary>
        /// 回调函数，得到选取的人员工号信息
        /// </summary>
        /// <param name="principalSelect"></param>
        private void ChildWnd_SelectWorkSn_CallBack(BaseViewModelCollection<UserPersonInfo> personSelect)
        {
            string txtShowWorkSn = "";
            foreach (UserPersonInfo ar in personSelect)
            {
                if (ar.isSelected)
                {
                    txtShowWorkSn += ar.work_sn + ",";
                }
            }
            if (txtShowWorkSn != "")
            {
                txtWorkSn.Text = txtShowWorkSn.Remove(txtShowWorkSn.LastIndexOf(","), 1);
            }
            else
            {
                txtWorkSn.Text = txtShowWorkSn;
            }
        }

        #endregion

        #endregion

        #region 查询数据

        /// <summary>
        /// 考勤查询
        /// </summary>
        void GetAttend()
        {
            if (_querySetting.Contains("attendConditon"))
            {
                try
                {
                    QueryCondition condition;
                    _querySetting.TryGetValue<QueryCondition>("attendConditon", out condition);
                    _vmPersonAttendCollect.GetTotalAttendDetailList(condition.BeginTime, condition.EndTime, condition.DepartNameList, condition.PersonNameList, condition.WorkSnList);
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

        #endregion        

        #region 导出excel

        /// <summary>
        /// 导出excel文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            if (_vmPersonAttendCollect.TotalAttendModel.Count() < 1)
            {
                Dialog.MsgBoxWindow.MsgBox(
                            "请查询到数据后再导出Excel！",
                            Dialog.MsgBoxWindow.MsgIcon.Information,
                            Dialog.MsgBoxWindow.MsgBtns.OK);
                return;
            }

            QueryCondition condition;
            if (_querySetting.Contains("attendConditon"))
            {
                _querySetting.TryGetValue<QueryCondition>("attendConditon", out condition);
            }
            else
            {
                return;
            }

            var uri = new Uri(App.Current.Host.Source, string.Format("../ShenShuoRailway/考勤统计表.aspx?beginTime={0}&endTime={1}", condition.BeginTime,condition.EndTime));
            HtmlPage.PopupWindow(uri, "_blank", new HtmlPopupWindowOptions());
           
        }

        #endregion

        #region 打印预览

        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            WaitingDialog.ShowWaiting("正在打印预览数据，请稍后", 500, new Action(btnPrintEvent)); 
        }

        /// <summary>
        /// 预览打印数据
        /// </summary>
        void btnPrintEvent()
        {
            if (_vmPersonAttendCollect.TotalAttendModel.Count() < 1)
            {
                Dialog.MsgBoxWindow.MsgBox(
                           "请查询到数据后再进行打印预览！",
                           Dialog.MsgBoxWindow.MsgIcon.Information,
                           Dialog.MsgBoxWindow.MsgBtns.OK);

                WaitingDialog.HideWaiting();
                return;
            }
            DateTime beginTime = new DateTime();
            DateTime endTime = new DateTime();
            if (_querySetting.Contains("attendConditon"))
            {
                QueryCondition condition;
                _querySetting.TryGetValue<QueryCondition>("attendConditon", out condition);
                beginTime = condition.BeginTime;
                endTime = condition.EndTime.AddDays(-1);
            }
            else
            {
                WaitingDialog.HideWaiting();
                return;
            }

            #region     生成表头数据
            List<HeaderNode> pageHeaderData = new List<HeaderNode>();

            //pageHeaderData.Add(new HeaderNode("序号", 11, 0, 1));
            pageHeaderData.Add(new HeaderNode("部门", 9, 0, 1));
            //pageHeaderData.Add(new HeaderNode("工号", 9, 0, 1));
            pageHeaderData.Add(new HeaderNode("姓名", 9, 0, 1));            
            pageHeaderData.Add(new HeaderNode("班次", 9, 0, 1));
            for (DateTime mindate = beginTime; mindate <= endTime; mindate = mindate.AddDays(1))
            {
                pageHeaderData.Add(new HeaderNode(mindate.Day.ToString("d2"), 9, 0, 1));
            }
            pageHeaderData.Add(new HeaderNode("应到", 9, 0, 1));
            pageHeaderData.Add(new HeaderNode("实到", 9, 0, 1));
            pageHeaderData.Add(new HeaderNode("未签", 9, 0, 1));
            pageHeaderData.Add(new HeaderNode("迟到", 9, 0, 1));
            pageHeaderData.Add(new HeaderNode("早退", 9, 0, 1));
            pageHeaderData.Add(new HeaderNode("加班", 9, 0, 1));
            pageHeaderData.Add(new HeaderNode("请假", 9, 0, 1));
            pageHeaderData.Add(new HeaderNode("公出", 9, 0, 1));

            ReportHeader reportHeader = new ReportHeader(pageHeaderData);
            #endregion

            #region 表标题
            string time = beginTime.ToLongDateString() + " 到 " + endTime.ToLongDateString();

            ReportTitle reportTitle = new ReportTitle("考勤统计表", 20, time, 11, false,true);
            #endregion

            #region     生成报表页脚数据
            ReportFooter reportFooter = new ReportFooter(@"备注：正常='+',未签到='[',未签退=']',旷工='-',迟到='>',早退='&lt;',加班='加',公出='差',产假='产',护理假='护',病假='病',事假='事',探亲假='探',年休假='年',助勤='助',调休='调',婚假='婚'", 11);
            //ReportFooter reportFooter = new ReportFooter(@"备注：正常=+,未签到=[,未签退=],旷工=-,迟到='>',早退='<',加班=加,公出=差,产假=产 护理假=护，病假=病,事假=事,探亲假=探,年休假=年,助勤=助,调休=调,婚假=婚", 11);
            #endregion

            string[] bindingPropertyNames = new string[pageHeaderData.Count()];

            int index = 0;
            //bindingPropertyNames[index++] = "Index";
            bindingPropertyNames[index++] = "DepartName";
            //bindingPropertyNames[index++] = "WorkSn";
            bindingPropertyNames[index++] = "PersonName";
            bindingPropertyNames[index++] = "ClassType";
            bindingPropertyNames[index++] = "DisplaySignal";
            bindingPropertyNames[index++] = "SupposeNum";
            bindingPropertyNames[index++] = "ActualNum";
            bindingPropertyNames[index++] = "AbsentNum";
            bindingPropertyNames[index++] = "LateNum";
            bindingPropertyNames[index++] = "LeaveEarlyNum";
            bindingPropertyNames[index++] = "ExtraNum";
            bindingPropertyNames[index++] = "AskLeaveNum";
            bindingPropertyNames[index++] = "BusinessNum";

            PrintControl printControl = new PrintControl();

            bool res = false;
            res =  printControl.SetDataSource<TotalAttend>(reportTitle, reportFooter, reportHeader, _vmPersonAttendCollect.TotalAttendModel, bindingPropertyNames,()=>
            {
                if (res)
                {
                    printControl.Preview_CurPage(null, null);
                }
            },9);

           

            WaitingDialog.HideWaiting();
        }

        #endregion              
    }
    #region 选择条件

    /// <summary>
    /// 报表查询条件
    /// </summary>
    public class QueryCondition
    {
        /// <summary>
        /// 起始时间
        /// </summary>
        public DateTime BeginTime { get; set; }

        /// <summary>
        /// 截止时间
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        public string[] DepartNameList { get; set; }

        /// <summary>
        /// 人员姓名
        /// </summary>
        public string[] PersonNameList { get; set; }

        /// <summary>
        /// 工号
        /// </summary>
        public string[] WorkSnList { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public QueryCondition()
        {
            BeginTime = DateTime.MinValue;
            EndTime = DateTime.Now;
            DepartNameList = null;
            PersonNameList = null;
            WorkSnList = null;
        }
    }

    #endregion
}
