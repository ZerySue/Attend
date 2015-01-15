/*************************************************************************
** 文件名:   XiBeiDianLanAttendCollect.cs
×× 主要类:   XiBeiDianLanAttendCollect
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2014-5-28
** 修改人:   
** 日  期:   
** 描  述:   XiBeiDianLanAttendCollect类，西北电缆厂考勤查询界面
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
using IriskingAttend.ExportExcel;

namespace IriskingAttend.XiBeiDianLan
{
    public partial class XiBeiDianLanAttendCollect : Page
    {
        #region 私有变量

        private VmXiBeiDianLanAttendCollect _vmOfficeAttendCollect = new VmXiBeiDianLanAttendCollect();

        /// <summary>
        /// 本地存储
        /// </summary>
        private IsolatedStorageSettings _querySetting = IsolatedStorageSettings.ApplicationSettings;

        /// <summary>
        /// 私有变量声明
        /// </summary>
        VmXlsFilter _vmFilter = new VmXlsFilter();

        private DateTime dateBegin = new DateTime();
        private DateTime dateEnd = new DateTime();

        #endregion

        #region 构造函数 初始化

        /// <summary>
        /// 构造函数
        /// </summary>
        public XiBeiDianLanAttendCollect()
        {
            InitializeComponent();

            //dtpBegin.Text = DateTime.Now.ToString("yyyy-MM-01");
            //dtpEnd.Text = Convert.ToDateTime(dtpBegin.Text).AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");


            this.DataContext = _vmOfficeAttendCollect;

            _vmFilter.GetDepartmentByPrivilege();

            //部门信息加载完成
            _vmFilter.DepartLoadCompleted += delegate
            {
            };

            //注册行加载事件
            dgAttendPersonList.LoadingRow += new EventHandler<DataGridRowEventArgs>(dgAttendPersonList_LoadingRow);
        }
        #endregion

        #region 点击报表单元格出现详细信息相关

        /// <summary>
        /// 加载行事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dgAttendPersonList_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.MouseLeftButtonUp -= new MouseButtonEventHandler(Row_MouseLeftButtonUp);
            e.Row.MouseLeftButtonUp += new MouseButtonEventHandler(Row_MouseLeftButtonUp);
        }

        /// <summary>
        /// 点击行事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Row_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //获取当前点击的列index            
            int selectIndex = dgAttendPersonList.CurrentColumn.DisplayIndex;

            OfficeAttend temp = (OfficeAttend)this.dgAttendPersonList.SelectedItem;

            if (selectIndex == 2 || selectIndex == 5 || selectIndex == 6 || selectIndex == 8 || selectIndex == 9)
            {
                DlgPersonAttendRecord dlgRecord = new DlgPersonAttendRecord(dateBegin, dateEnd, temp.PersonId, selectIndex);

                dlgRecord.lblDepart.Content = temp.DepartName;
                dlgRecord.lblPersonName.Content = temp.PersonName;
                dlgRecord.lblWorkSn.Content = temp.WorkSn;

                dlgRecord.Show();
            }
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
            dgAttendPersonList.ItemsSource = _vmOfficeAttendCollect.OfficeAttendModel;

            #region 判断查询时间
            if (dtpBegin.Text == null || dtpBegin.Text == "")
            {
                MsgBoxWindow.MsgBox(
                            "查询开始时间不能为空！",
                            Dialog.MsgBoxWindow.MsgIcon.Information,
                            Dialog.MsgBoxWindow.MsgBtns.OK);
                return;
            }
            dateBegin = dtpBegin.SelectedDate.Value.Date;
           

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
                List<string> departName = new List<string>();
                foreach (var item in VmLogin.OperatorDepartInfoList)
                {
                    departName.Add(item.depart_name);
                }
                qc.DepartNameList = departName.ToArray();
            }
            

            //获取选择的人员姓名
            if (txtPersonName.Text != "")
            {
                qc.PersonName = txtPersonName.Text;
            }

            //获取选择的人员工号
            if (txtWorkSn.Text != "")
            {
                qc.WorkSn = txtWorkSn.Text;
            }

            ///如果存在 先删除该键值对
            if (_querySetting.Contains("attendConditon"))
            {
                _querySetting.Remove("attendConditon");
            }
            _querySetting.Add("attendConditon", qc);
        }

        #region 选取部门

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
                    _vmOfficeAttendCollect.GetOfficeAttendCollect(condition.BeginTime, condition.EndTime, condition.DepartNameList, condition.PersonName, condition.WorkSn);
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
            if (_vmOfficeAttendCollect.OfficeAttendModel.Count() < 1)
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

            string space = "                              ";
            ExpExcel.ExportExcelFromDataGrid(this.dgAttendPersonList, 1, 13, (space + "西北电缆机关考勤" + space), "西北电缆机关考勤");

            //var uri = new Uri(App.Current.Host.Source, string.Format("../ShenShuoRailway/考勤统计表.aspx?beginTime={0}&endTime={1}", condition.BeginTime,condition.EndTime));
            //HtmlPage.PopupWindow(uri, "_blank", new HtmlPopupWindowOptions());
           
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
            if (_vmOfficeAttendCollect.OfficeAttendModel.Count() < 1)
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

            pageHeaderData.Add(new HeaderNode("序号", 9, 0, 1));
            pageHeaderData.Add(new HeaderNode("部门", 9, 0, 1));            
            pageHeaderData.Add(new HeaderNode("姓名", 9, 0, 1));
            pageHeaderData.Add(new HeaderNode("工号", 9, 0, 1));
            pageHeaderData.Add(new HeaderNode("班制", 9, 0, 1));
            
            pageHeaderData.Add(new HeaderNode("上午班出勤次数", 9, 0, 1));
            pageHeaderData.Add(new HeaderNode("下午班出勤次数", 9, 0, 1));
            pageHeaderData.Add(new HeaderNode("出勤天数", 9, 0, 1));
            pageHeaderData.Add(new HeaderNode("迟到次数", 9, 0, 1));
            pageHeaderData.Add(new HeaderNode("早退次数", 9, 0, 1));
            pageHeaderData.Add(new HeaderNode("休假天数", 9, 0, 1));
            pageHeaderData.Add(new HeaderNode("事假天数", 9, 0, 1));
            pageHeaderData.Add(new HeaderNode("备注", 9, 0, 1));

            ReportHeader reportHeader = new ReportHeader(pageHeaderData);
            #endregion

            #region 表标题
            string time = beginTime.ToLongDateString() + " 到 " + endTime.ToLongDateString();

            ReportTitle reportTitle = new ReportTitle("西北电缆机关考勤表", 20, time, 11, false, true);
            #endregion

            #region     生成报表页脚数据
            ReportFooter reportFooter = new ReportFooter(@"", 11);
            #endregion

            string[] bindingPropertyNames = new string[pageHeaderData.Count()];

            int index = 0;
            bindingPropertyNames[index++] = "Index";
            bindingPropertyNames[index++] = "DepartName";            
            bindingPropertyNames[index++] = "PersonName";
            bindingPropertyNames[index++] = "WorkSn";
            bindingPropertyNames[index++] = "ClassType";
            bindingPropertyNames[index++] = "MorningAttendNum";
            bindingPropertyNames[index++] = "AfternoonAttendNum";
            bindingPropertyNames[index++] = "DayAttendNum";
            bindingPropertyNames[index++] = "LateNum";
            bindingPropertyNames[index++] = "LeaveEarlyNum";
            bindingPropertyNames[index++] = "ShiftHolidayNum";
            bindingPropertyNames[index++] = "AskLeaveNum";
            bindingPropertyNames[index++] = "Note";

            PrintControl printControl = new PrintControl();

            bool res = false;
            res = printControl.SetDataSource<OfficeAttend>(reportTitle, reportFooter, reportHeader, _vmOfficeAttendCollect.OfficeAttendModel, bindingPropertyNames, () =>
            {
                if (res)
                {
                    printControl.Preview_CurPage(null, null);
                }
            }, 9);

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
        public string PersonName { get; set; }

        /// <summary>
        /// 工号
        /// </summary>
        public string WorkSn { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public QueryCondition()
        {
            BeginTime = DateTime.MinValue;
            EndTime = DateTime.Now;
            DepartNameList = null;
            PersonName = "";
            WorkSn = "";
        }
    }

    #endregion
}
