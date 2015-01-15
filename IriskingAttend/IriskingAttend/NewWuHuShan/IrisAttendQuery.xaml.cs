/*************************************************************************
** 文件名:   IrisAttendQuery.cs
×× 主要类:   IrisAttendQuery
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   cty
** 日  期:   2014-2-24
** 修改人:   
** 日  期:   
** 描  述:   IrisAttendQuery类，五虎山虹膜工数查询
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
using System.IO.IsolatedStorage;
using Irisking.Web.DataModel;
using IriskingAttend.Dialog;
using IriskingAttend.ViewModel;
using IriskingAttend.Common;
using ReportTemplate;
using Lite.ExcelLibrary.SpreadSheet;
using System.IO;

namespace IriskingAttend.NewWuHuShan
{
    public partial class IrisAttendQuery : Page
    {   
        #region 私有变量
        /// <summary>
        /// 五虎山独立查询界面的VM层
        /// </summary>
        private VmXlsWuHuShan _vmXls = new VmXlsWuHuShan();

        /// <summary>
        /// 职务、工种vm数据
        /// </summary>
        private VmXlsFilter _vmXlsFilter = new VmXlsFilter();

        /// <summary>
        /// 报表DataGrid起始插入日期列--报表定制
        /// </summary>
        private const int _colInsert = 5;

        /// <summary>
        /// 报表DataGrid最大日期列--报表定制
        /// </summary>
        private const int _colMax = 36;

        /// <summary>
        /// 文件存储对话框
        /// </summary>
        private SaveFileDialog _sDialog;

        /// <summary>
        /// 本地存储
        /// </summary>
        private IsolatedStorageSettings _querySetting = IsolatedStorageSettings.ApplicationSettings;

        /// <summary>
        /// 选中的部门
        /// </summary>
        private List<UserDepartInfo> _departSelect = new List<UserDepartInfo>();

        /// <summary>
        /// 选中的职务
        /// </summary>
        private List<PrincipalInfo> _principalSelect = new List<PrincipalInfo>();

        /// <summary>
        /// 选中的工种
        /// </summary>
        private List<WorkTypeInfo> _workTypeSelect = new List<WorkTypeInfo>();

        #endregion

        public IrisAttendQuery()
        {
            InitializeComponent();
            SetInt();
        }

        // 当用户导航到此页面时执行。
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        /// <summary>
        /// 初始化绑定 部门，职务，工种
        /// </summary>
        private void SetInt()
        {
            this.dgIrisAttendQuery.LoadingRow += (a, e) =>
            {
                try
                {
                    int index = e.Row.GetIndex();
                    int col2 = _colInsert;
                    foreach (var ar in _vmXls.IrisAttendQueryModelShow[index].AtteinTime)
                    {
                        var cell2 = dgIrisAttendQuery.Columns[col2++].GetCellContent(e.Row) as TextBlock;
                        cell2.Text = ar.ToString();
                    }
                }
                catch (Exception eee)
                {
                    string str = eee.Message;
                }
            };
        }

        #region 点击查询 获取查询条件

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQuery_Click(object sender, RoutedEventArgs e)
        {
            dgIrisAttendQuery.ItemsSource = _vmXls.IrisAttendQueryModelShow;

            #region 判断查询时间
            if (dtpBegin.Text == null || dtpBegin.Text == "")
            {
                MsgBoxWindow.MsgBox(
                            "查询开始时间不能为空！",
                            Dialog.MsgBoxWindow.MsgIcon.Information,
                            Dialog.MsgBoxWindow.MsgBtns.OK);
                return;
            }
            DateTime dataBegin = dtpBegin.SelectedDate.Value.Date;
            DateTime dataEnd;

            if (dtpEnd.Text == null || dtpEnd.Text == "")
            {
                dataEnd = dataBegin.AddMonths(1);
            }
            else
            {
                if (dataBegin >= dtpEnd.SelectedDate.Value.Date.AddDays(1))
                {
                    MsgBoxWindow.MsgBox(
                                "请确定您的开始时间早于截止时间！",
                                Dialog.MsgBoxWindow.MsgIcon.Information,
                                Dialog.MsgBoxWindow.MsgBtns.OK);
                    return;
                }
                else if (dataBegin.AddMonths(1) < dtpEnd.SelectedDate.Value.Date.AddDays(1))
                {
                    MsgBoxWindow.MsgBox(
                                 "请确定您的查询跨度时间为一个月！",
                                  Dialog.MsgBoxWindow.MsgIcon.Information,
                                  Dialog.MsgBoxWindow.MsgBtns.OK);
                    return;
                }
                else
                {
                    dataEnd = dtpEnd.SelectedDate.Value.Date.AddDays(1);
                }
            }
            #endregion

            int workTime = 0;

            if (txtWorkTime.Text.Trim() != "" && txtWorkTime.Text.Trim() != null)
            {
                DateTime date;
                try
                {
                    workTime = int.Parse(txtWorkTime.Text.Trim());
                }
                catch
                {
                    try
                    {
                        date = Convert.ToDateTime(txtWorkTime.Text.Trim());
                        workTime = date.Hour * 60 + date.Minute;
                    }
                    catch
                    {
                        Dialog.MsgBoxWindow.MsgBox(
                                "请确定您输入的工作时长为数字！",
                                Dialog.MsgBoxWindow.MsgIcon.Information,
                                Dialog.MsgBoxWindow.MsgBtns.OK);
                        return;
                    }
                }
            }

            SetdgIrisAttendQueryHeader(dataBegin, dataEnd);

            SetCondition(dataBegin, dataEnd, workTime);

            GetAttend();
        }
        /// <summary>
        /// 设置datagrid的Header
        /// </summary>
        /// <param name="dataBegin">查询开始时间</param>
        /// <param name="dataEnd">查询截止时间</param>
        private void SetdgIrisAttendQueryHeader(DateTime dataBegin, DateTime dataEnd)
        {
            int col = _colInsert;
            for (dataBegin = dtpBegin.SelectedDate.Value.Date; dataBegin < dataEnd; dataBegin = dataBegin.AddDays(1))
            {
                if (dataBegin.Day > 9)
                {
                    dgIrisAttendQuery.Columns[col].Header = dataBegin.Day.ToString();
                }
                else
                {
                    dgIrisAttendQuery.Columns[col].Header = "0" + dataBegin.Day.ToString();
                }
                dgIrisAttendQuery.Columns[col++].Visibility = System.Windows.Visibility.Visible;
            }
            for (int i = col; i < _colMax; i++)
            {
                dgIrisAttendQuery.Columns[i].Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 获取查询条件
        /// </summary>
        /// <param name="dataBegin"></param>
        /// <param name="dataEnd"></param>
        private void SetCondition(DateTime dataBegin, DateTime dataEnd, int workTime)
        {
            XlsQueryCondition qc = new XlsQueryCondition();
            qc.XlsBeginTime = dataBegin;
            qc.XlsEndTime = dataEnd;
            qc.XlsName = txtPersonName.Text.Trim();
            qc.XlsWorkSN = txtWorkSn.Text.Trim();
            qc.XlsWorkTime = workTime;

            //获取选择的部门
            if (_departSelect.Count > 0)
            {
                qc.XlsDepartIdLst = new int[_departSelect.Count];
                int num = 0;
                foreach (UserDepartInfo ar in _departSelect)
                {
                    qc.XlsDepartIdLst[num++] = ar.depart_id;
                }
            }
            else
            {
                qc.XlsDepartIdLst = new int[VmLogin.OperatorDepartInfoList.Count];
                int num = 0;
                foreach (UserDepartInfo ar in VmLogin.OperatorDepartInfoList)
                {
                    qc.XlsDepartIdLst[num++] = ar.depart_id;
                }                
            }

            //获取选择的职务
            if (_principalSelect.Count > 0)
            {
                qc.XlsPrincipalId = new int[_principalSelect.Count];
                int num = 0;
                foreach (PrincipalInfo ar in _principalSelect)
                {
                    qc.XlsPrincipalId[num++] = ar.principal_id;
                }
            }

            //获取选择的工种
            if (_workTypeSelect.Count > 0)
            {
                qc.XlsWorkTypeId = new int[_workTypeSelect.Count];
                int num = 0;
                foreach (WorkTypeInfo ar in _workTypeSelect)
                {
                    qc.XlsWorkTypeId[num++] = ar.work_type_id;
                }
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
            SelectDepart sd = new SelectDepart(_departSelect, ChildWnd_SelectDepart_CallBack);
            sd.Show();
        }

        /// <summary>
        /// 回调函数，得到选取的部门信息
        /// </summary>
        /// <param name="principalSelect"></param>
        private void ChildWnd_SelectDepart_CallBack(BaseViewModelCollection<UserDepartInfo> departSelect)
        {
            string txtShowDepartName = "";
            _departSelect.Clear();
            foreach (UserDepartInfo ar in departSelect)
            {
                if (ar.isSelected)
                {
                    _departSelect.Add(ar);
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

        /// <summary>
        /// 显示选择职务的子窗口，选取职务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectPrincipal_Click(object sender, RoutedEventArgs e)
        {
            SelectPrincipal sp = new SelectPrincipal(_principalSelect, ChildWnd_SelectPrincipal_CallBack);
            sp.Show();
        }

        /// <summary>
        /// 回调函数，得到选取的职务信息
        /// </summary>
        /// <param name="principalSelect"></param>
        private void ChildWnd_SelectPrincipal_CallBack(BaseViewModelCollection<PrincipalInfo> principalSelect)
        {
            string txtShowPrincipalName = "";
            _principalSelect.Clear();
            foreach (PrincipalInfo ar in principalSelect)
            {
                if (ar.isSelected)
                {
                    _principalSelect.Add(ar);
                    txtShowPrincipalName += ar.principal_name + ",";
                }
            }
            if (txtShowPrincipalName != "")
            {
                txtPrincipal.Text = txtShowPrincipalName.Remove(txtShowPrincipalName.LastIndexOf(","), 1);
            }
            else
            {
                txtPrincipal.Text = txtShowPrincipalName;
            }
        }

        /// <summary>
        /// 显示选择工种的子窗口，选取工种
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectWorkType_Click(object sender, RoutedEventArgs e)
        {
            SelectWorkType sp = new SelectWorkType(_workTypeSelect, ChildWnd_SelectWorkType_CallBack);
            sp.Show();
        }

        /// <summary>
        /// 回调函数，得到选取的职务信息
        /// </summary>
        /// <param name="principalSelect"></param>
        private void ChildWnd_SelectWorkType_CallBack(BaseViewModelCollection<WorkTypeInfo> workTypeSelect)
        {
            string txtShowWorkTypeName = "";
            _workTypeSelect.Clear();
            foreach (WorkTypeInfo ar in workTypeSelect)
            {
                if (ar.isSelected)
                {
                    _workTypeSelect.Add(ar);
                    txtShowWorkTypeName += ar.work_type_name + ",";
                }
            }
            if (txtShowWorkTypeName != "")
            {
                txtWorkType.Text = txtShowWorkTypeName.Remove(txtShowWorkTypeName.LastIndexOf(","), 1);
            }
            else
            {
                txtWorkType.Text = txtShowWorkTypeName;
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
                    XlsQueryCondition condition;
                    _querySetting.TryGetValue<XlsQueryCondition>("attendConditon", out condition);
                    _vmXls.GetIrisAttendQuery(condition.XlsBeginTime, condition.XlsEndTime, condition.XlsDepartIdLst, condition.XlsName, condition.XlsWorkSN, condition.XlsPrincipalId, condition.XlsWorkTypeId, condition.XlsWorkTime);
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

        #region 获取点击当前单元格的信息，并获取其详细信息的集合
        /// <summary>
        /// 点击行事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgAttendPersonList_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.MouseLeftButtonUp -= new MouseButtonEventHandler(Row_MouseLeftButtonUp);
            e.Row.MouseLeftButtonUp += new MouseButtonEventHandler(Row_MouseLeftButtonUp);
        }

        /// <summary>
        /// 获取当前点击的单元格信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Row_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            int selectIndex = dgIrisAttendQuery.CurrentColumn.DisplayIndex;
            string sColumnValue = ((TextBlock)this.dgIrisAttendQuery.Columns[selectIndex].GetCellContent(this.dgIrisAttendQuery.SelectedItem)).Text.Trim();

            //判断点击的是显示工数的单元格，而不是姓名、工号等
            if (sColumnValue != "" && selectIndex >= 5 && selectIndex <= 35)
            {
                var obj = dgIrisAttendQuery.SelectedItem as XlsAttendWuHuShan;

                int personId = obj.PersonId;

                _vmXls.IrisAttendQueryShowDetail(personId, selectIndex - 5);

                IrisAttendQueryDetails sd = new IrisAttendQueryDetails(_vmXls.IrisShowDetailsModel);
                sd.Show();

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
            if (_vmXls.IrisAttendQueryModelShow.Count() < 1)
            {
                Dialog.MsgBoxWindow.MsgBox(
                            "请查询到数据后再导出Excel！",
                            Dialog.MsgBoxWindow.MsgIcon.Information,
                            Dialog.MsgBoxWindow.MsgBtns.OK);
                return;
            }
            DateTime beginTime = new DateTime();
            DateTime endTime = new DateTime();
            if (_querySetting.Contains("attendConditon"))
            {
                XlsQueryCondition condition;
                _querySetting.TryGetValue<XlsQueryCondition>("attendConditon", out condition);
                beginTime = condition.XlsBeginTime;
                endTime = condition.XlsEndTime.AddDays(-1);
            }
            else
            {
                return;
            }
            if (dtpBegin.Text == null || dtpBegin.Text == "")
            {
                Dialog.MsgBoxWindow.MsgBox(
                           "查询时间不能为空！",
                           Dialog.MsgBoxWindow.MsgIcon.Information,
                           Dialog.MsgBoxWindow.MsgBtns.OK);
            }
            else
            {
                _sDialog = new SaveFileDialog();
                _sDialog.Filter = "Excel Files(*.xls)|*.xls";

                if (_sDialog.ShowDialog() == false)
                {
                    return;
                }
            }

            string time = beginTime.ToLongDateString() + " 到 " + endTime.ToLongDateString();
            string title = "";
            string space = "                                                                           ";

            Workbook workBook = new Workbook();
            Worksheet workSheet = new Worksheet(title);

            workSheet.Cells[0, 0] = new Cell(space + title + "    " + time + space);

            Int16 ColumnCount = -1;
            Int16 RowCount = 1;

            #region     生成表头

            workSheet.Cells[RowCount, ++ColumnCount] = new Cell("序号");
            workSheet.Cells[RowCount, ++ColumnCount] = new Cell("部门");
            workSheet.Cells[RowCount, ++ColumnCount] = new Cell("姓名");
            workSheet.Cells[RowCount, ++ColumnCount] = new Cell("工号");
            workSheet.Cells[RowCount, ++ColumnCount] = new Cell("工种");
            for (DateTime mindate = beginTime; mindate <= endTime; mindate = mindate.AddDays(1))
            {
                workSheet.Cells[RowCount, ++ColumnCount] = new Cell(mindate.Day.ToString("D2"));
                workSheet.Cells.ColumnWidth[(byte)RowCount, (byte)ColumnCount] = 1500;
            }
            workSheet.Cells[RowCount, ++ColumnCount] = new Cell("夜班");
            workSheet.Cells[RowCount, ++ColumnCount] = new Cell("早班");
            workSheet.Cells[RowCount, ++ColumnCount] = new Cell("中班");
            workSheet.Cells[RowCount, ++ColumnCount] = new Cell("合计");
            #endregion

            foreach (XlsAttendWuHuShan data in _vmXls.IrisAttendQueryModelShow)
            {
                ColumnCount = 0;
                RowCount++;

                workSheet.Cells[RowCount, ColumnCount++] = new Cell(data.Index);
                workSheet.Cells[RowCount, ColumnCount++] = new Cell(data.DepartName);
                workSheet.Cells[RowCount, ColumnCount++] = new Cell(data.PersonName);
                workSheet.Cells[RowCount, ColumnCount++] = new Cell(data.WorkSn);
                workSheet.Cells[RowCount, ColumnCount++] = new Cell(data.WorkType);

                foreach (string attendTime in data.AtteinTime)
                {
                    workSheet.Cells[RowCount, ColumnCount++] = new Cell(attendTime);
                }

                workSheet.Cells[RowCount, ColumnCount++] = new Cell(data.NightAttendTimes);
                workSheet.Cells[RowCount, ColumnCount++] = new Cell(data.MoringAttendTimes);
                workSheet.Cells[RowCount, ColumnCount++] = new Cell(data.MiddleAttendTimes);
                workSheet.Cells[RowCount, ColumnCount++] = new Cell(data.SumAttendTimes);
            }

            workBook.Worksheets.Add(workSheet);
            Stream sFile = _sDialog.OpenFile();
            workBook.Save(sFile);
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
            if (_vmXls.IrisAttendQueryModelShow.Count() < 1)
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
                XlsQueryCondition condition;
                _querySetting.TryGetValue<XlsQueryCondition>("attendConditon", out condition);
                beginTime = condition.XlsBeginTime;
                endTime = condition.XlsEndTime.AddDays(-1);
            }
            else
            {
                WaitingDialog.HideWaiting();
                return;
            }

            #region     生成表头数据
            List<HeaderNode> pageHeaderData = new List<HeaderNode>();

            pageHeaderData.Add(new HeaderNode("序号", 11, 0, 1));
            pageHeaderData.Add(new HeaderNode("部门", 11, 0, 1));
            pageHeaderData.Add(new HeaderNode("姓名", 11, 0, 1));
            pageHeaderData.Add(new HeaderNode("工号", 11, 0, 1));
            pageHeaderData.Add(new HeaderNode("工种", 11, 0, 1));
            for (DateTime mindate = beginTime; mindate <= endTime; mindate = mindate.AddDays(1))
            {
                if (mindate.Day < 10)
                {
                    pageHeaderData.Add(new HeaderNode("0" + mindate.Day.ToString(), 11, 0, 1));
                }
                else
                {
                    pageHeaderData.Add(new HeaderNode(mindate.Day.ToString(), 11, 0, 1));
                }
            }
            pageHeaderData.Add(new HeaderNode("夜班", 11, 0, 1));
            pageHeaderData.Add(new HeaderNode("早班", 11, 0, 1));
            pageHeaderData.Add(new HeaderNode("中班", 11, 0, 1));
            pageHeaderData.Add(new HeaderNode("合计", 11, 0, 1));

            ReportHeader reportHeader = new ReportHeader(pageHeaderData);
            #endregion

            #region 表标题
            string time = beginTime.ToLongDateString() + " 到 " + endTime.ToLongDateString();

            ReportTitle reportTitle = new ReportTitle("", 20, time, 11, false);
            #endregion

            #region     生成报表页脚数据
            ReportFooter reportFooter = new ReportFooter("", 11);
            #endregion

            string[] bindingPropertyNames = new string[] 
            { 
                "Index",
                "DepartName",
                "PersonName",
                "WorkSn",
                "WorkType",
                "AtteinTime",
                "NightAttendTimes",
                "MoringAttendTimes",
                "MiddleAttendTimes",
                "SumAttendTimes",
            };

            PrintControl printControl = new PrintControl();

            bool res = false;
            res = printControl.SetDataSource<XlsAttendWuHuShan>(reportTitle, reportFooter, reportHeader, _vmXls.IrisAttendQueryModelShow, bindingPropertyNames, () =>
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
}
