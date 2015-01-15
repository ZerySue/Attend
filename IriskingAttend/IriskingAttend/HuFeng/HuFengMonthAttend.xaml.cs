/*************************************************************************
** 文件名:   HuFengMonthAttend.cs
** 主要类:   HuFengMonthAttend
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2014-6-20
** 修改人:   
** 日  期:
** 描  述:   HuFengMonthAttend，虎峰煤矿月报表
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
using IriskingAttend.NewWuHuShan;
using System.IO.IsolatedStorage;
using IriskingAttend.Common;
using ReportTemplate;
using IriskingAttend.Web;
using Irisking.Web.DataModel;
using IriskingAttend.ExportExcel;
using System.IO;
using Lite.ExcelLibrary.SpreadSheet;

namespace IriskingAttend.HuFeng
{
    public partial class HuFengMonthAttend : Page
    {
        #region 私有变量

        private VmHuFengAttend _vmAttend = new VmHuFengAttend();
    
        /// <summary>
        /// 本地存储
        /// </summary>
        private IsolatedStorageSettings _querySetting = IsolatedStorageSettings.ApplicationSettings; 

        #endregion

        #region 构造函数
        public HuFengMonthAttend()
        {
            InitializeComponent(); 
            
            this.DataContext = _vmAttend;
            dgMonthAttend.ItemsSource = _vmAttend.MonthAttendModel;
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

            HuFengDayQueryCondition qc = new HuFengDayQueryCondition();

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

            qc.ClassOrderList = null;           

            qc.PersonName = "";
            //获取人员姓名
            if (txtPersonName.Text != "")
            {
                qc.PersonName = txtPersonName.Text.Trim();
            }

            qc.WorkSn = "";
            //获取人员工号
            if (txtWorkSn.Text != "")
            {
                qc.WorkSn = txtWorkSn.Text.Trim();
            }

            ///如果存在 先删除该键值对
            if (_querySetting.Contains("attendConditon"))
            {
                _querySetting.Remove("attendConditon");
            }
            _querySetting.Add("attendConditon", qc);

            int col = 4;
            for (DateTime mindate = qc.BeginTime; mindate <= qc.EndTime; mindate = mindate.AddDays(1))
            {
                dgMonthAttend.Columns[col].Header = mindate.Day.ToString("d2");
                dgMonthAttend.Columns[col++].Visibility = System.Windows.Visibility.Visible;
            }
            for (int i = col; i < 35; i++)
            {
                dgMonthAttend.Columns[i].Visibility = System.Windows.Visibility.Collapsed;
            }

            _vmAttend.GetMonthAttendCollect(qc.BeginTime, qc.EndTime, qc.DepartNameList, qc.PersonName, qc.WorkSn);

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
            if (_vmAttend.MonthAttendModel.Count() < 1)
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
                HuFengDayQueryCondition condition;
                _querySetting.TryGetValue<HuFengDayQueryCondition>("attendConditon", out condition);
                beginTime = condition.BeginTime;
                endTime = condition.EndTime;
            }
            else
            {
                WaitingDialog.HideWaiting();
                return;
            }

            SaveFileDialog sDialog = new SaveFileDialog();
            sDialog.Filter = "Excel Files(*.xls)|*.xls";

            if (sDialog.ShowDialog() == false)
            {
                return;
            }

            string title = "虎峰煤矿月考勤查询";
            string space = "                                                                                                    ";

            Workbook workBook = new Workbook();
            Worksheet workSheet = new Worksheet(title);

            workSheet.Cells[0, 0] = new Cell(space + title + space);

            Int16 ColumnCount = -1;
            Int16 RowCount = 1;

            #region     生成表头

            workSheet.Cells[RowCount, ++ColumnCount] = new Cell("序号");
            workSheet.Cells[RowCount, ++ColumnCount] = new Cell("工号");
            workSheet.Cells[RowCount, ++ColumnCount] = new Cell("姓名");
            workSheet.Cells[RowCount, ++ColumnCount] = new Cell("部门");            

            for (DateTime mindate = beginTime; mindate <= endTime; mindate = mindate.AddDays(1))
            {
                workSheet.Cells[RowCount, ++ColumnCount] = new Cell(mindate.Day.ToString("D2"));
                workSheet.Cells.ColumnWidth[(byte)RowCount, (byte)ColumnCount] = 2500;
            }            
            workSheet.Cells[RowCount, ++ColumnCount] = new Cell("出勤次数");
            workSheet.Cells[RowCount, ++ColumnCount] = new Cell("总工时");

            #endregion

            foreach (HuFengMonthAttendReport data in _vmAttend.MonthAttendModel)
            {
                ColumnCount = 0;
                RowCount++;

                workSheet.Cells[RowCount, ColumnCount++] = new Cell(data.Index);
                workSheet.Cells[RowCount, ColumnCount++] = new Cell(data.WorkSn);
                workSheet.Cells[RowCount, ColumnCount++] = new Cell(data.PersonName);
                workSheet.Cells[RowCount, ColumnCount++] = new Cell(data.DepartName);

                foreach (string count in data.DisplayInfo)
                {
                    workSheet.Cells[RowCount, ColumnCount++] = new Cell(count);
                }

                workSheet.Cells[RowCount, ColumnCount++] = new Cell(data.AttendCount);
                workSheet.Cells[RowCount, ColumnCount++] = new Cell(data.TotalWorkTime);              
            }

            //add worksheet to workbook
            workBook.Worksheets.Add(workSheet);
            // get the selected file's stream
            Stream sFile = sDialog.OpenFile();
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
            if (_vmAttend.MonthAttendModel.Count() < 1)
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
                HuFengDayQueryCondition condition;
                _querySetting.TryGetValue<HuFengDayQueryCondition>("attendConditon", out condition);
                beginTime = condition.BeginTime;
                endTime = condition.EndTime;
            }
            else
            {
                WaitingDialog.HideWaiting();
                return;
            }

            #region     生成表头数据


            List<HeaderNode> pageHeaderData = new List<HeaderNode>();

            pageHeaderData.Add(new HeaderNode("序号", 11, 0, 1));
            pageHeaderData.Add(new HeaderNode("工号", 11, 0, 1));
            pageHeaderData.Add(new HeaderNode("姓名", 11, 0, 1));
            pageHeaderData.Add(new HeaderNode("部门", 11, 0, 1));
            
            for (DateTime mindate = beginTime; mindate <= endTime; mindate = mindate.AddDays(1))
            {
                pageHeaderData.Add(new HeaderNode(mindate.Day.ToString("d2"), 9, 0, 1));
            }
            
            pageHeaderData.Add(new HeaderNode("出勤次数", 11, 0, 1));
            pageHeaderData.Add(new HeaderNode("总工时", 11, 0, 1));

            ReportHeader reportHeader = new ReportHeader(pageHeaderData);
            #endregion

            #region 表标题
            string time = beginTime.ToLongDateString() + " 到 " + endTime.ToLongDateString();

            ReportTitle reportTitle = new ReportTitle("虎峰煤矿月考勤查询", 20, time, 11, false);
            #endregion

            #region     生成报表页脚数据
            ReportFooter reportFooter = new ReportFooter("", 11);
            #endregion

            string[] bindingPropertyNames = new string[] 
            { 
                "Index",
                "WorkSn",
                "PersonName",
                "DepartName",                
                "DisplayInfo",
                "AttendCount",
                "TotalWorkTime",               
            };

            PrintControl printControl = new PrintControl();
            bool res = false;
            res = printControl.SetDataSource<HuFengMonthAttendReport>(reportTitle, reportFooter, reportHeader, _vmAttend.MonthAttendModel, bindingPropertyNames, () =>
            {
                if (res)
                {
                    printControl.Preview_CurPage(null, null);
                }
            }, 9);
            WaitingDialog.HideWaiting();
        }

        #endregion    

        #region 选取部门

        private void btnSelectDepart_Click(object sender, RoutedEventArgs e)
        {
            VmXlsFilter vmFilter = new VmXlsFilter();
            vmFilter.GetDepartmentByPrivilege();
            vmFilter.DepartLoadCompleted += delegate
            {
                string[] departList = new string[0];
                //获取选择的部门
                if (txtDepart.Text != "")
                {
                    departList = txtDepart.Text.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                }

                vmFilter.DepartInfoModel.RemoveAt(0);

                VmSelectInfoFilter vmInfoFilter = new VmSelectInfoFilter();
                foreach (UserDepartInfo departInfo in vmFilter.DepartInfoModel)
                {
                    SelectInfoFilter infoFilter = new SelectInfoFilter();
                    infoFilter.InfoName = departInfo.depart_name;

                    if (departList.Contains(departInfo.depart_name))
                    {
                        infoFilter.Checked = true;
                    }
                    vmInfoFilter.InfoFilterModel.Add(infoFilter);
                }

                SelectInfo selectInfo = new SelectInfo(vmInfoFilter, txtDepart, ChildWnd_SelectInfo_CallBack);
                selectInfo.Title = "选择部门";
                selectInfo.Show();
            };           
        }

        /// <summary>
        /// 回调函数，得到选取的信息
        /// </summary>
        /// <param name="principalSelect"></param>
        private void ChildWnd_SelectInfo_CallBack(BaseViewModelCollection<SelectInfoFilter> infoSelect, TextBox txtShow)
        {
            string txtShowName = "";
            foreach (SelectInfoFilter ar in infoSelect)
            {
                if (ar.Checked)
                {
                    txtShowName += ar.InfoName + ",";
                }
            }
            if (txtShowName != "")
            {
                txtShow.Text = txtShowName.Remove(txtShowName.LastIndexOf(","), 1);
            }
            else
            {
                txtShow.Text = txtShowName;
            }
        }
        #endregion         
    }
}
