/*************************************************************************
** 文件名:   XiGouLeaderAttendRec.cs
** 主要类:   XiGouLeaderAttendRec
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   cty
** 日  期:   2014-4-10
** 修改人:   
** 日  期:
** 描  述:   XiGouLeaderAttendRec，西沟一矿领导带班考勤表前台cs
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
using IriskingAttend.NewWuHuShan;
using IriskingAttend.ViewModel;
using Irisking.Web.DataModel;
using IriskingAttend.Dialog;
using System.IO.IsolatedStorage;
using IriskingAttend.Web;
using IriskingAttend.ExportExcel;
using Lite.ExcelLibrary.SpreadSheet;
using System.IO;
using IriskingAttend.Common;
using ReportTemplate;

namespace IriskingAttend.XiGouYiKuang
{
    public partial class XiGouLeaderAttendRec : Page
    {
        #region 私有变量

        /// <summary>
        /// VM层
        /// </summary>
        VmXiGouYiKuang _vm = new VmXiGouYiKuang();

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

        #endregion

        #region 构造函数 初始化

        /// <summary>
        /// 构造函数
        /// </summary>
        public XiGouLeaderAttendRec()
        {
            InitializeComponent();
            
            //添加序号
            this.dgLeaderAttend.LoadingRow += (a, ex) =>
            {
                int index = ex.Row.GetIndex();
                var cell = dgLeaderAttend.Columns[0].GetCellContent(ex.Row) as TextBlock;
                cell.Text = (index + 1).ToString();
            };
        }

        // 当用户导航到此页面时执行。
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        #endregion

        #region 点击查询 获取数据

        private void btnQuery_Click(object sender, RoutedEventArgs e)
        {
            dgLeaderAttend.ItemsSource = _vm.LeaderAttendList;
         
            #region 判断查询时间
            if (dtpBegin.Text == null || dtpBegin.Text == "")
            {
                MsgBoxWindow.MsgBox(
                            "查询时间不能为空！",
                            Dialog.MsgBoxWindow.MsgIcon.Information,
                            Dialog.MsgBoxWindow.MsgBtns.OK);
                return;
            }
            #endregion
            DateTime dataBegin = dtpBegin.SelectedDate.Value.Date;
            DateTime dataEnd = dataBegin.AddDays(1);
            SetCondition(dataBegin,dataEnd);
            GetAttend();
        }

        /// <summary>
        /// 获取查询条件
        /// </summary>
        /// <param name="dataBegin"></param>
        /// <param name="dataEnd"></param>
        private void SetCondition(DateTime dataBegin,DateTime dataEnd)
        {
            XlsQueryCondition qc = new XlsQueryCondition();
            qc.XlsBeginTime = dataBegin;
            qc.XlsEndTime = dataEnd;

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
                    _vm.GetXiGouLeaderAttendList(condition.XlsBeginTime, condition.XlsEndTime, condition.XlsDepartIdLst);
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

        #endregion

        #region 获取点击当前单元格的信息，并获取其详细信息的集合

        private void dgLeaderAttend_LoadingRow(object sender, DataGridRowEventArgs e)
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
            int selectIndex = dgLeaderAttend.CurrentColumn.DisplayIndex;
            string sColumnValue = ((TextBlock)this.dgLeaderAttend.Columns[selectIndex].GetCellContent(this.dgLeaderAttend.SelectedItem)).Text.Trim();

            //判断点击的是显示工数的单元格，而不是姓名、工号等
            if (sColumnValue != "" && selectIndex ==2)
            {
                XiGouLeaderAttend obj = dgLeaderAttend.SelectedItem as XiGouLeaderAttend;

                int personId = obj.ShiftPersonId;
                DateTime beginTime = new DateTime(dtpBegin.SelectedDate.Value.Year,dtpBegin.SelectedDate.Value.Month,1);
                DateTime endTime = beginTime.AddMonths(1).AddDays(-1);
                _vm.GetXiGouPersonLeaderAttend(beginTime, endTime, personId);

                XiGouPersonDetailAttendRec sd = new XiGouPersonDetailAttendRec(obj, beginTime, endTime, _vm.PersonLeaderAttend);
                sd.Show();

            }
        }

        #endregion

        #region 打印预览
        /// <summary>
        /// 打印预览
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
            if (_vm.LeaderAttendList.Count() < 1)
            {
                Dialog.MsgBoxWindow.MsgBox(
                           "请查询到数据后再进行打印预览！",
                           Dialog.MsgBoxWindow.MsgIcon.Information,
                           Dialog.MsgBoxWindow.MsgBtns.OK);

                WaitingDialog.HideWaiting();
                return;
            }
            DateTime beginTime = new DateTime();
            if (_querySetting.Contains("attendConditon"))
            {
                XlsQueryCondition condition;
                _querySetting.TryGetValue<XlsQueryCondition>("attendConditon", out condition);
                beginTime = condition.XlsBeginTime;
            }
            else
            {
                WaitingDialog.HideWaiting();
                return;
            }

         
            WaitingDialog.HideWaiting();
        }

        #endregion

        #region 导出excel

        /// <summary>
        /// 导出excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            if (this._vm.LeaderAttendList.Count() < 1)
            {
                Dialog.MsgBoxWindow.MsgBox(
                           "请查询到数据后再导出Excel！",
                           Dialog.MsgBoxWindow.MsgIcon.Information,
                           Dialog.MsgBoxWindow.MsgBtns.OK);

                return;
            }

            DateTime beginTime = new DateTime();
            if (_querySetting.Contains("attendConditon"))
            {
                XlsQueryCondition condition;
                _querySetting.TryGetValue<XlsQueryCondition>("attendConditon", out condition);
                beginTime = condition.XlsBeginTime;
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

            string time = beginTime.ToLongDateString();
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
            workSheet.Cells[RowCount, ++ColumnCount] = new Cell("职务");
            workSheet.Cells[RowCount, ++ColumnCount] = new Cell("班制");
            workSheet.Cells[RowCount, ++ColumnCount] = new Cell("班次");
            workSheet.Cells[RowCount, ++ColumnCount] = new Cell("日期");
            workSheet.Cells[RowCount, ++ColumnCount] = new Cell("入井时间");
            workSheet.Cells[RowCount, ++ColumnCount] = new Cell("出井时间");
            workSheet.Cells[RowCount, ++ColumnCount] = new Cell("出入井时长");
            workSheet.Cells[RowCount, ++ColumnCount] = new Cell("值班领导1");
            workSheet.Cells[RowCount, ++ColumnCount] = new Cell("值班领导2");
            #endregion
            int index = 1;
            foreach (XiGouLeaderAttend data in _vm.LeaderAttendList)
            {
                ColumnCount = 0;
                RowCount++;

                workSheet.Cells[RowCount, ColumnCount++] = new Cell(index++);
                workSheet.Cells[RowCount, ColumnCount++] = new Cell(data.DepartName);
                workSheet.Cells[RowCount, ColumnCount++] = new Cell(data.ShiftPersonName);
                workSheet.Cells[RowCount, ColumnCount++] = new Cell(data.ShiftWorkSn);
                workSheet.Cells[RowCount, ColumnCount++] = new Cell(data.ShiftPrincipal);
                workSheet.Cells[RowCount, ColumnCount++] = new Cell(data.ClassTypeName);
                workSheet.Cells[RowCount, ColumnCount++] = new Cell(data.ClassOrderName);
                workSheet.Cells[RowCount, ColumnCount++] = new Cell(data.AttendDayStr);
                workSheet.Cells[RowCount, ColumnCount++] = new Cell(data.InWellTime);
                workSheet.Cells[RowCount, ColumnCount++] = new Cell(data.OutWellTime);           
                workSheet.Cells[RowCount, ColumnCount++] = new Cell(data.WorkTime);
                workSheet.Cells[RowCount, ColumnCount++] = new Cell(data.OnDutyPersonName);
                workSheet.Cells[RowCount, ColumnCount++] = new Cell(data.OnDutyPersonName2);
            }

            workBook.Worksheets.Add(workSheet);
            Stream sFile = _sDialog.OpenFile();
            workBook.Save(sFile);
        }

        #endregion

    }
}
