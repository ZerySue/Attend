/*************************************************************************
** 文件名:   PersonAttendCollect.cs
×× 主要类:   PersonAttendCollect
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2013-12-3
** 修改人:   
** 日  期:   
** 描  述:   PersonAttendCollect类，个人出勤汇总表
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
using System.IO;
using IriskingAttend.Common;
using ReportTemplate;
using System.Windows.Browser;
using IriskingAttend.NewWuHuShan;

using Lite.ExcelLibrary.CompoundDocumentFormat;
using Lite.ExcelLibrary.BinaryFileFormat;
using Lite.ExcelLibrary.SpreadSheet;
using IriskingAttend.Web;
using IriskingAttend.ApplicationType;

namespace IriskingAttend.ShenShuoRailway
{
    public partial class PersonAttendCollect : Page
    {
        #region 私有变量

        private VmPersonAttendCollect _vmPersonAttendCollect = new VmPersonAttendCollect();

        /// <summary>
        /// 私有变量声明
        /// </summary>
        private VmXlsFilter _vmFilter = new VmXlsFilter();

        /// <summary>
        /// 本地存储
        /// </summary>
        private IsolatedStorageSettings _querySetting = IsolatedStorageSettings.ApplicationSettings;

        #endregion

        #region 构造函数 初始化

        /// <summary>
        /// 构造函数
        /// </summary>
        public PersonAttendCollect()
        {
            InitializeComponent();

            //打印预览和导出Excel的权限            
            if (!VmLogin.DictPrivilege[AbstractApp.PrivilegeENUM.ReportPersonAttendCollectExportExcel])
            {
                this.btnExportExcel.Visibility = Visibility.Collapsed;
            }

            if (!VmLogin.DictPrivilege[AbstractApp.PrivilegeENUM.ReportPersonAttendCollectPrint])
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

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQuery_Click(object sender, RoutedEventArgs e)
        {
            this.dgDepartAttend.ItemsSource = _vmPersonAttendCollect.PersonAttendModel;

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

            SetCondition(dateBegin, dateEnd);

            GetAttend();

        }

        /// <summary>
        /// 考勤查询
        /// </summary>
        private void GetAttend()
        {
            if (_querySetting.Contains("attendConditon"))
            {
                try
                {
                    QueryCondition condition;
                    _querySetting.TryGetValue<QueryCondition>("attendConditon", out condition);
                    _vmPersonAttendCollect.GetPersonAttendCollect(condition.BeginTime, condition.EndTime, condition.DepartNameList, condition.PersonNameList, condition.WorkSnList);
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
            if (_vmPersonAttendCollect.PersonAttendModel.Count() < 1)
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

            var uri = new Uri(App.Current.Host.Source, string.Format("../ShenShuoRailway/个人出勤表.aspx?beginTime={0}&endTime={1}", condition.BeginTime, condition.EndTime));
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
            if (_vmPersonAttendCollect.PersonAttendModel.Count() < 1)
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

            pageHeaderData.Add(new HeaderNode("日期", 11, 0, 1));
            pageHeaderData.Add(new HeaderNode("部门", 11, 0, 1));
            pageHeaderData.Add(new HeaderNode("姓名", 11, 0, 1));
            pageHeaderData.Add(new HeaderNode("上午上班时间", 11, 0, 1));
            pageHeaderData.Add(new HeaderNode("上午下班时间", 11, 0, 1));
            pageHeaderData.Add(new HeaderNode("下午上班时间", 11, 0, 1));
            pageHeaderData.Add(new HeaderNode("下午下班时间", 11, 0, 1));
            pageHeaderData.Add(new HeaderNode("备注", 11, 0, 1));

            ReportHeader reportHeader = new ReportHeader(pageHeaderData);
            #endregion

            #region 表标题
            string time = beginTime.ToLongDateString() + " 到 " + endTime.ToLongDateString();

            ReportTitle reportTitle = new ReportTitle("个人出勤表", 20, time, 11, false,true);
            #endregion

            #region     生成报表页脚数据
            ReportFooter reportFooter = new ReportFooter("", 11);
            #endregion

            string[] bindingPropertyNames = new string[] 
            { 
                "AttendDay",
                "DepartName",
                "PersonName",
                "MoringInWellTime",
                "MoringOutWellTime",
                "AfternoonInWellTime",
                "NightAttendTimes",
                "AfternoonOutWellTime",
                "Note",
            };

            PrintControl printControl = new PrintControl();
            bool res = false;
            res = printControl.SetDataSource<PersonAttend>(reportTitle, reportFooter, reportHeader, _vmPersonAttendCollect.PersonAttendModel, bindingPropertyNames, () =>
            {
                if (res)
                {
                    printControl.Preview_CurPage(null, null);
                }
            }, 9);
            
            WaitingDialog.HideWaiting();
        }

        #endregion

        #region 选取部门、职务、工种

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
                    departNameList.Add( item.depart_name);
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
        private void dtpBegin_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            dtpEnd.Text = dtpBegin.Text;
        }
    }
}
