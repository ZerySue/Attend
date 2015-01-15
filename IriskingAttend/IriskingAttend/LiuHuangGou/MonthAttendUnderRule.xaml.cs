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
using IriskingAttend.NewWuHuShan;
using System.IO.IsolatedStorage;
using IriskingAttend.Common;
using ReportTemplate;
using IriskingAttend.Web;
using Irisking.Web.DataModel;
using IriskingAttend.ExportExcel;
using System.IO;
using Lite.ExcelLibrary.SpreadSheet;
using IriskingAttend.ZhongKeHongBa;
using System.Windows.Browser;

namespace IriskingAttend.LiuHuangGou
{
    public partial class MonthAttendUnderRule : Page
    {
        #region 私有变量

        private VmLiuhuangGouAttendCollect _vmMonthAttendCollect = new VmLiuhuangGouAttendCollect();
      
        /// <summary>
        /// 私有变量声明
        /// </summary>
        private VmXlsFilter _vmFilter = new VmXlsFilter();
    
        /// <summary>
        /// 本地存储
        /// </summary>
        private IsolatedStorageSettings _querySetting = IsolatedStorageSettings.ApplicationSettings;       

        private VmPersonFullAttendCollect _vmPersonFullAttendCollect = new VmPersonFullAttendCollect(); 

        #endregion

        #region 构造函数
        public MonthAttendUnderRule()
        {
            InitializeComponent(); 
 
            dtpBegin.Text = DateTime.Now.AddMonths(-2).ToString("yyyy-MM-01");
            dtpEnd.Text = DateTime.Parse(DateTime.Now.AddMonths(-1).ToString("yyyy-MM-01")).AddDays(-1).ToString("yyyy-MM-dd");
            this.DataContext = _vmMonthAttendCollect;
            dgMonthAttend.ItemsSource = _vmMonthAttendCollect.MonthAttendUnderRuleModel;

            _vmFilter.GetDepartmentByPrivilege();

            //部门信息加载完成
            _vmFilter.DepartLoadCompleted += delegate
            {
            };   
        }
        #endregion

        #region 点击查询 
       
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQuery_Click(object sender, RoutedEventArgs e)
        {
            if (dtpBegin.Text == null || dtpBegin.Text == "")
            {
                MsgBoxWindow.MsgBox(
                            "查询开始时间不能为空！",
                            Dialog.MsgBoxWindow.MsgIcon.Information,
                            Dialog.MsgBoxWindow.MsgBtns.OK);
                return;
            }

            int ruleDataNum = 0;

            if (txtRuleDataNum.Text != "")
            {
                if (!int.TryParse(txtRuleDataNum.Text, out ruleDataNum))
                {
                    MsgBoxWindow.MsgBox(
                            "制度工作日只能为数字！",
                            Dialog.MsgBoxWindow.MsgIcon.Information,
                            Dialog.MsgBoxWindow.MsgBtns.OK);
                    return;
                }                
            }

            LiuHuangGouQueryCondition qc = new LiuHuangGouQueryCondition();

            qc.BeginTime = this.dtpBegin.SelectedDate.Value.Date;

            if (dtpEnd.Text == null || dtpEnd.Text == "")
            {
                qc.EndTime = this.dtpBegin.SelectedDate.Value.AddMonths(1).AddDays(-1);
                if (qc.EndTime > System.DateTime.Now)
                {
                    qc.EndTime = System.DateTime.Now.Date;
                }
            }
            else
            {
                if (qc.BeginTime >= dtpEnd.SelectedDate.Value.Date.AddDays(1))
                {
                    MsgBoxWindow.MsgBox(
                                "请确定您的开始时间早于截止时间！",
                                Dialog.MsgBoxWindow.MsgIcon.Information,
                                Dialog.MsgBoxWindow.MsgBtns.OK);
                    return;
                }
                else if (qc.BeginTime.AddMonths(1) < dtpEnd.SelectedDate.Value.Date.AddDays(1))
                {
                    MsgBoxWindow.MsgBox(
                                 "请确定您的查询跨度时间为一个月！",
                                  Dialog.MsgBoxWindow.MsgIcon.Information,
                                  Dialog.MsgBoxWindow.MsgBtns.OK);
                    return;
                }
                else
                {
                    qc.EndTime = dtpEnd.SelectedDate.Value.Date;
                }
            }
            

            qc.DepartNameList = null;
            //获取选择的部门
            if (txtDepart.Text != "")
            {
                qc.DepartNameList = txtDepart.Text.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            }
            else
            {
                List<string> departName = new List<string>();
                foreach (var item in VmLogin.OperatorDepartInfoList)
                {
                    departName.Add(item.depart_name);
                }
                qc.DepartNameList = departName.ToArray();
            }

            qc.PersonNameList = null;
            //获取选择的人员姓名
            if (txtPersonName.Text != "")
            {
                qc.PersonNameList = txtPersonName.Text.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            }

            ///如果存在 先删除该键值对
            if (_querySetting.Contains("attendConditon"))
            {
                _querySetting.Remove("attendConditon");
            }
            _querySetting.Add("attendConditon", qc);


            int col = 28;
            for (DateTime mindate = qc.BeginTime; mindate <= qc.EndTime; mindate = mindate.AddDays(1))
            {
                dgMonthAttend.Columns[col].Header = mindate.Day.ToString("d2");
                dgMonthAttend.Columns[col++].Visibility = System.Windows.Visibility.Visible;
            }
            for (int i = col; i < 59; i++)
            {
                dgMonthAttend.Columns[i].Visibility = System.Windows.Visibility.Collapsed;
            }

            if (ruleDataNum >= ((qc.EndTime - qc.BeginTime).Days + 1))
            {
                ruleDataNum = 0;
            }

            _vmMonthAttendCollect.GetMonthAttendUnderRuleCollect(qc.BeginTime, qc.EndTime, qc.DepartNameList, qc.PersonNameList, ruleDataNum);

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
            if (_vmMonthAttendCollect.MonthAttendUnderRuleModel.Count() < 1)
            {
                Dialog.MsgBoxWindow.MsgBox(
                            "请查询到数据后再导出Excel！",
                            Dialog.MsgBoxWindow.MsgIcon.Information,
                            Dialog.MsgBoxWindow.MsgBtns.OK);
                return;
            }

            DateTime beginTime = new DateTime();
            DateTime endTime = new DateTime();
            LiuHuangGouQueryCondition condition;
            if (_querySetting.Contains("attendConditon"))
            {
                _querySetting.TryGetValue<LiuHuangGouQueryCondition>("attendConditon", out condition);
                beginTime = condition.BeginTime;
                endTime = condition.EndTime;
            }
            else
            {
                WaitingDialog.HideWaiting();
                return;
            }

            var uri = new Uri(App.Current.Host.Source, string.Format("../LiuHuangGou/月考勤报表.aspx?beginTime={0}&endTime={1}", condition.BeginTime, condition.EndTime));
            HtmlPage.PopupWindow(uri, "_blank", new HtmlPopupWindowOptions());
        }

        #endregion

        #region 打印预览

        ///// <summary>
        ///// 打印
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void btnPrint_Click(object sender, RoutedEventArgs e)
        //{
        //    WaitingDialog.ShowWaiting("正在打印预览数据，请稍后", 500, new Action(btnPrintEvent));
        //}

        ///// <summary>
        ///// 预览打印数据
        ///// </summary>
        //void btnPrintEvent()
        //{
        //    if (_vmMonthAttendCollect.MonthAttendUnderRuleModel.Count() < 1)
        //    {
        //        Dialog.MsgBoxWindow.MsgBox(
        //                   "请查询到数据后再进行打印预览！",
        //                   Dialog.MsgBoxWindow.MsgIcon.Information,
        //                   Dialog.MsgBoxWindow.MsgBtns.OK);

        //        WaitingDialog.HideWaiting();
        //        return;
        //    }
        //    DateTime beginTime = new DateTime();
        //    DateTime endTime = new DateTime();
        //    if (_querySetting.Contains("attendConditon"))
        //    {
        //        LiuHuangGouQueryCondition condition;
        //        _querySetting.TryGetValue<LiuHuangGouQueryCondition>("attendConditon", out condition);
        //        beginTime = condition.BeginTime;
        //        endTime = condition.EndTime.AddDays(-1);
        //    }
        //    else
        //    {
        //        WaitingDialog.HideWaiting();
        //        return;
        //    }

        //    #region     生成表头数据


        //    List<HeaderNode> pageHeaderData = new List<HeaderNode>();

        //    pageHeaderData.Add(new HeaderNode("序号", 11, 0, 1));
        //    pageHeaderData.Add(new HeaderNode("内部关联号", 11, 0, 1));
        //    pageHeaderData.Add(new HeaderNode("单位", 11, 0, 1));
        //    pageHeaderData.Add(new HeaderNode("姓名", 11, 0, 1));
        //    pageHeaderData.Add(new HeaderNode("出勤工数", 11, 0, 1));
        //    pageHeaderData.Add(new HeaderNode("出勤总时长", 11, 0, 1));
        //    pageHeaderData.Add(new HeaderNode("下井总时长", 11, 0, 1));
        //    pageHeaderData.Add(new HeaderNode("加班工数", 11, 0, 1));
        //    pageHeaderData.Add(new HeaderNode("大班值班工数", 11, 0, 1));
        //    pageHeaderData.Add(new HeaderNode("大班入井工数", 11, 0, 1));
        //    pageHeaderData.Add(new HeaderNode("小班入井工数", 11, 0, 1));
        //    pageHeaderData.Add(new HeaderNode("地面大班工数", 11, 0, 1));
        //    pageHeaderData.Add(new HeaderNode("地面早班工数", 11, 0, 1));
        //    pageHeaderData.Add(new HeaderNode("地面中班工数", 11, 0, 1));
        //    pageHeaderData.Add(new HeaderNode("地面夜班工数", 11, 0, 1));
        //    pageHeaderData.Add(new HeaderNode("井下早班工数", 11, 0, 1));
        //    pageHeaderData.Add(new HeaderNode("井下中班工数", 11, 0, 1));
        //    pageHeaderData.Add(new HeaderNode("井下夜班工数", 11, 0, 1));
        //    pageHeaderData.Add(new HeaderNode("大班公出工数", 11, 0, 1));
        //    pageHeaderData.Add(new HeaderNode("婚丧探天数", 11, 0, 1));
        //    pageHeaderData.Add(new HeaderNode("年休天数", 11, 0, 1));
        //    pageHeaderData.Add(new HeaderNode("病假天数", 11, 0, 1));
        //    pageHeaderData.Add(new HeaderNode("伤假天数", 11, 0, 1));
        //    pageHeaderData.Add(new HeaderNode("事假天数", 11, 0, 1));
        //    pageHeaderData.Add(new HeaderNode("旷工天数", 11, 0, 1));
        //    pageHeaderData.Add(new HeaderNode("产假天数", 11, 0, 1));
        //    pageHeaderData.Add(new HeaderNode("育儿假天数", 11, 0, 1));
        //    pageHeaderData.Add(new HeaderNode("待岗学习天数", 11, 0, 1));
        //    for (DateTime mindate = beginTime; mindate <= endTime; mindate = mindate.AddDays(1))
        //    {
        //        pageHeaderData.Add(new HeaderNode(mindate.Day.ToString("d2"), 9, 0, 1));
        //    }
            

        //    ReportHeader reportHeader = new ReportHeader(pageHeaderData);
        //    #endregion

        //    #region 表标题
        //    string time = beginTime.ToLongDateString() + " 到 " + endTime.ToLongDateString();

        //    ReportTitle reportTitle = new ReportTitle("原始记录汇总表", 20, time, 11, false);
        //    #endregion

        //    #region     生成报表页脚数据
        //    ReportFooter reportFooter = new ReportFooter("", 11);
        //    #endregion

        //    string[] bindingPropertyNames = new string[] 
        //    { 
        //        "Index",
        //        "WorkSn",
        //        "DepartName",
        //        "PersonName",
        //        "InOutTimes",
        //        "WorkTotalTimes",
        //        "WellTotalTimes",
        //        "OverTimes",
        //        "BigDutyTimes",
        //        "BigInWellTimes",
        //        "SmallInWellTimes",
        //        "GroupBigTimes",
        //        "GroupMorningTimes",
        //        "GroupMiddleTimes",
        //        "GroupNightTimes",
        //        "InWellMorningTimes",
        //        "InWellMiddleTimes",
        //        "InWellNightTimes",
        //        "BigClassOutTimes",
        //        "WeddingsTimes",
        //        "AnnualLeaveTimes",
        //        "SickLeaveTimes",
        //        "FakeInjuryTimes",
        //        "LeaveTimes",
        //        "AbsenteeismTimes",
        //        "MaternityLeaveTimes",
        //        "ParentalLeaveTimes",
        //        "StudyDays",
        //        "ClassOrder",                
        //    };

        //    PrintControl printControl = new PrintControl();
        //    bool res = false;
        //    res = printControl.SetDataSource<PersonMonthAttend>(reportTitle, reportFooter, reportHeader, _vmMonthAttendCollect.MonthAttendUnderRuleModel, bindingPropertyNames, () =>
        //    {
        //        if (res)
        //        {
        //            printControl.Preview_CurPage(null, null);
        //        }
        //    }, 9);
        //    WaitingDialog.HideWaiting();
        //}

        #endregion    

        #region 选取部门、姓名

        private void btnSelectDepart_Click(object sender, RoutedEventArgs e)
        {
            List<UserDepartInfo> departList = new List<UserDepartInfo>();
            string[] selectDepart = new string[0];

            //获取选择的部门
            if (txtDepart.Text != "")
            {
                selectDepart = txtDepart.Text.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            }
            if (_vmFilter.DepartInfoModel.Count > 0)
            {
                foreach (UserDepartInfo ud in _vmFilter.DepartInfoModel)
                {
                    if (selectDepart.Contains(ud.depart_name))
                    {
                        departList.Add(ud);
                    }
                }
            }
            SelectDepart sd = new SelectDepart(departList, ChildWnd_SelectDepart_CallBack);
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
            _vmPersonFullAttendCollect.GetPersonInfo(txtDepart.Text.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries), () =>
            {
                _vmPersonFullAttendCollect.SelectPersonByName();

                string[] personList = new string[0];

                //获取选择的部门
                if (txtPersonName.Text != "")
                {
                    personList = txtPersonName.Text.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                }
                if (_vmPersonFullAttendCollect.SelectPersonInfoModel.Count > 0)
                {
                    foreach (UserPersonInfo personInfo in _vmPersonFullAttendCollect.SelectPersonInfoModel)
                    {
                        personInfo.isSelected = false;
                        if (personList.Contains(personInfo.person_name))
                        {
                            personInfo.isSelected = true;
                        }
                    }
                }

                SelectPersonName selectPerson = new SelectPersonName(_vmPersonFullAttendCollect.SelectPersonInfoModel, ChildWnd_SelectName_CallBack);
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
        #endregion         

    }   
}
