/*************************************************************************
** 文件名:   DepartAttendCollect.cs
×× 主要类:   DepartAttendCollect
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2013-11-21
** 修改人:   
** 日  期:   
** 描  述:   DepartAttendCollect类，部门出勤汇总表
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
using IriskingAttend.ApplicationType;
using System.ServiceModel.DomainServices.Client;
using EDatabaseError;

using System.Collections;
using IriskingAttend.Web.DataModel;
using System.Windows.Data;
using IriskingAttend.ExportExcel;
using System.Reflection;

namespace IriskingAttend.GuoDian
{
    public partial class GuoDianReport : Page
    {
        #region 私有变量
        DateTime beginTime;
        DateTime endTime;

        //报表中动态的日期列
        List<DataGridTemplateColumn> dynamicCols = new List<DataGridTemplateColumn>();
        DataGridTextColumn customCol = null;

        private int dynamicColsCount
        {
            get
            {
                return dynamicCols.Where(d => d.Visibility == Visibility.Visible).Count();
            }
        }


        #endregion

        #region 构造函数 初始化

        /// <summary>
        /// 构造函数
        /// </summary>
        public GuoDianReport()
        {
            InitializeComponent();

            dgReport.LoadingRow += (s,e) =>
                {
                    int index = e.Row.GetIndex();
                    var cell = dgReport.Columns[0].GetCellContent(e.Row) as TextBlock;
                    cell.Text = (index + 1).ToString();

                };
            dgReport.CanUserSortColumns = false;
            dgReport.MouseLeftButtonDown += new MouseButtonEventHandler(dataGrid_MouseLeftButtonDown);
            dgReport.MouseMove += new MouseEventHandler(dataGrid_MouseMove);
            dgReport.LayoutUpdated += new EventHandler(dataGrid_LayoutUpdated);
            dgReport.MouseLeftButtonUp += new MouseButtonEventHandler(dataGrid_MouseLeftButtonUp);


            GetCandidateDepartRia();

            this.Loaded += new RoutedEventHandler(GuoDianReport_Loaded);
        }
       


        //获取动态列
        void GuoDianReport_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var item in dgReport.Columns)
            {
                if (item is DataGridTemplateColumn)
                {
                    dynamicCols.Add((DataGridTemplateColumn)item);
                    item.Visibility = Visibility.Collapsed;
                    item.MinWidth = 62;
                }
                if (item.Header!=null && item.Header.ToString() == "部门")
                {
                    customCol = (DataGridTextColumn)item;
                }
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
            #region 判断查询时间
            if (dtpBegin.Text == null || dtpBegin.Text == "")
            {
                MsgBoxWindow.MsgBox(
                            "查询开始时间不能为空！",
                            Dialog.MsgBoxWindow.MsgIcon.Information,
                            Dialog.MsgBoxWindow.MsgBtns.OK);
                return;
            }
            beginTime = dtpBegin.SelectedDate.Value.Date;
            
            if (dtpEnd.Text == null || dtpEnd.Text == "")
            {
                endTime = beginTime.AddMonths(1).AddDays(-0.001);
            }
            else 
            {
                if (beginTime >= dtpEnd.SelectedDate.Value.Date.AddDays(1))
                {
                    MsgBoxWindow.MsgBox(
                                "请确定您的开始时间早于截止时间！",
                                Dialog.MsgBoxWindow.MsgIcon.Information,
                                Dialog.MsgBoxWindow.MsgBtns.OK);
                    return;
                }


                if (dtpEnd.SelectedDate.Value.Date >= beginTime.AddMonths(1))
                {
                    MsgBoxWindow.MsgBox(
                               "请确定您时间范围在一个月以内！",
                               Dialog.MsgBoxWindow.MsgIcon.Information,
                               Dialog.MsgBoxWindow.MsgBtns.OK);
                    return;
                }
                endTime = dtpEnd.SelectedDate.Value.Date.AddDays(0.99999);
            }
            #endregion

            //从数据库中检索
            if (comboBoxDataSource.SelectedIndex == 1)
            {
                List<int> departIds = this.departDropList.GetSelectedValues<int>();
                string personName = this.txtPersonName.Text.Trim();
                string workSn = this.txtWorkSn.Text.Trim();

                this.txtTitle.Text = string.Format("{0}单位{1}月考勤表", departDropList.GetSelectedContent(","), beginTime.Month);

                GetReportRia(personName, workSn, departIds);
            }
            else //从导入的excel中检索
            {
                GetReportByExcelRia();
            }
            
        }

        //报表数据源
        BaseViewModelCollection<GuoDianReportData> GuoDianReportDatas = new BaseViewModelCollection<GuoDianReportData>();
        
       
        //从原始数据库中查询报表
        private void GetReportRia(string personName, string workSn, List<int> departIds)
        {
            try
            {
                ServiceDomDbAcess.ReOpenSever();

                EntityQuery<GuoDianReportData> lstReport = ServiceDomDbAcess.GetSever()
                    .GetGuoDianReportQuery(beginTime, endTime, personName, workSn, departIds.ToArray());
                ///回调异常类
                Action<LoadOperation<GuoDianReportData>> CallBack = new Action<LoadOperation<GuoDianReportData>>(ErrorHandle<GuoDianReportData>.OnLoadErrorCallBack);
                ///异步事件
                LoadOperation<GuoDianReportData> lo = ServiceDomDbAcess.GetSever().Load(lstReport, CallBack, null);

                WaitingDialog.ShowWaiting();
                lo.Completed += (o, e) =>
                {
                    //先删除动态列
                    foreach (var item in dynamicCols)
                    {
                        item.Visibility = Visibility.Collapsed;
                    }
                    
                    //加载数据
                    GuoDianReportDatas.Clear();
                    foreach (var ar in lo.Entities)
                    {
                        GuoDianReportDatas.Add(ar);
                    }
                    dgReport.ItemsSource = GuoDianReportDatas;

                    if (GuoDianReportDatas.Count > 0)
                    {
                        
                        customCol.Header = GuoDianReportDatas[0].CustomColName;
                    }
                    
                    //日期
                    int index = 0;
                    for (DateTime currentDay = beginTime; currentDay < endTime; currentDay = currentDay.AddDays(1))
                    {
                        string header = string.Format("{0}月{1}日", currentDay.Month, currentDay.Day);
                        var templateColumn = dynamicCols[index];
                        templateColumn.Visibility = Visibility.Visible;
                        templateColumn.Header = header;
                        index++;
                    }
                    WaitingDialog.HideWaiting();

                };
            }
            catch (Exception e)
            {
                WaitingDialog.HideWaiting();
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
                
            }
        }

        //从导入的excel中查询报表
        private void GetReportByExcelRia()
        {
            try
            {
                ServiceDomDbAcess.ReOpenSever();

                EntityQuery<GuoDianReportData> lstReport = ServiceDomDbAcess.GetSever()
                    .GetGuoDianReportFromExcelQuery(beginTime, endTime);
                ///回调异常类
                Action<LoadOperation<GuoDianReportData>> CallBack = new Action<LoadOperation<GuoDianReportData>>(ErrorHandle<GuoDianReportData>.OnLoadErrorCallBack);
                ///异步事件
                LoadOperation<GuoDianReportData> lo = ServiceDomDbAcess.GetSever().Load(lstReport, CallBack, null);

                WaitingDialog.ShowWaiting();
                lo.Completed += (o, e) =>
                {
                    //先删除动态列
                    foreach (var item in dynamicCols)
                    {
                        item.Visibility = Visibility.Collapsed;
                    }

                    //加载数据
                    GuoDianReportDatas.Clear();
                    foreach (var ar in lo.Entities)
                    {
                        GuoDianReportDatas.Add(ar);
                    }
                    dgReport.ItemsSource = GuoDianReportDatas;



                    if (GuoDianReportDatas.Count > 0)
                    {
                        this.txtTitle.Text = GuoDianReportDatas[0].Title ;
                        customCol.Header = GuoDianReportDatas[0].CustomColName;

                    }

                    //日期
                    int index = 0;
                    for (DateTime currentDay = beginTime; currentDay < endTime; currentDay = currentDay.AddDays(1))
                    {
                        string header = string.Format("{0}月{1}日", currentDay.Month, currentDay.Day);
                        var templateColumn = dynamicCols[index];
                        templateColumn.Visibility = Visibility.Visible;
                        templateColumn.Header = header;
                        index++;
                    }
                    WaitingDialog.HideWaiting();

                };
            }
            catch (Exception e)
            {
                WaitingDialog.HideWaiting();
                ErrorWindow err = new ErrorWindow(e);
                err.Show();

            }
        }

        #region 获取待选部门

        /// <summary>
        /// 获取待选部门列表
        /// </summary>
        private void GetCandidateDepartRia()
        {
            try
            {
                ServiceDomDbAcess.ReOpenSever();

                EntityQuery<UserDepartInfo> lstDepart = ServiceDomDbAcess.GetSever().GetDepartsInfoQuery();
                ///回调异常类
                Action<LoadOperation<UserDepartInfo>> getDepartCallBack = new Action<LoadOperation<UserDepartInfo>>(ErrorHandle<UserDepartInfo>.OnLoadErrorCallBack);
                ///异步事件
                LoadOperation<UserDepartInfo> lo = ServiceDomDbAcess.GetSever().Load(lstDepart, getDepartCallBack, null);

                lo.Completed += (o, e) =>
                {
                    // depart_name 与 depart_id组成的键值对 
                    Dictionary<string,int> departList = new Dictionary<string,int>();
                   
                    
                    //增加部门权限 lo.Entities.OrderBy(a => a.depart_name);
                    foreach (var ar in lo.Entities)//.Where(a => VmLogin.OperatorDepartIDList.Contains(a.depart_id)).OrderBy(a => a.depart_name))
                    {
                        departList.Add(ar.depart_name,ar.depart_id);
                    }

                    departDropList.InitContent<int>(departList);
                    
                };
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }
   
        #endregion

        #endregion
        
        #region 导出excel

        /// <summary>
        /// 导出excel文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            if (this.GuoDianReportDatas.Count < 1)
            {
                Dialog.MsgBoxWindow.MsgBox(
                           "请查询到数据后再导出Excel！",
                           Dialog.MsgBoxWindow.MsgIcon.Information,
                           Dialog.MsgBoxWindow.MsgBtns.OK);

                WaitingDialog.HideWaiting();
                return;
            }

            #region 导出excel

            string space = "                                                             ";
            SaveFileDialog sDialog = new SaveFileDialog();
            sDialog.Filter = "Excel Files(*.xls)|*.xls";

            if (sDialog.ShowDialog() == false)
            {
                return;
            }

            string title = this.txtTitle.Text;
            
            Workbook workBook = new Workbook();
            Worksheet workSheet = new Worksheet(title);

            workSheet.Cells[0, 0] = new Cell(space + title + space);

            Int16 ColumnCount = -1;
            Int16 RowCount = 1;

            #region     生成表头

            workSheet.Cells[RowCount, ++ColumnCount] = new Cell("序号");
            workSheet.Cells.ColumnWidth[(byte)RowCount, (byte)ColumnCount] = 1000;

            workSheet.Cells[RowCount, ++ColumnCount] = new Cell("姓名");
            workSheet.Cells.ColumnWidth[(byte)RowCount, (byte)ColumnCount] = 1500;

            workSheet.Cells[RowCount, ++ColumnCount] = new Cell("工号");
            workSheet.Cells.ColumnWidth[(byte)RowCount, (byte)ColumnCount] = 1500;

            workSheet.Cells[RowCount, ++ColumnCount] = new Cell(customCol.Header.ToString());
            workSheet.Cells.ColumnWidth[(byte)RowCount, (byte)ColumnCount] = 1500;

            workSheet.Cells[RowCount, ++ColumnCount] = new Cell("出勤数");
            workSheet.Cells.ColumnWidth[(byte)RowCount, (byte)ColumnCount] = 1000;


            foreach (var item in dynamicCols)
            {
                if (item.Visibility == Visibility.Visible)
                {
                    workSheet.Cells[RowCount, ++ColumnCount] = new Cell(item.Header.ToString());
                    workSheet.Cells.ColumnWidth[(byte)RowCount, (byte)ColumnCount] = 4500;
                }
            }

            #endregion

            foreach (var data in GuoDianReportDatas)
            {
                ColumnCount = -1;
                RowCount++;

                workSheet.Cells[RowCount, ++ColumnCount] = new Cell((RowCount - 1).ToString());
                workSheet.Cells[RowCount, ++ColumnCount] = new Cell(data.PersonName);
                workSheet.Cells[RowCount, ++ColumnCount] = new Cell(data.WorkSn);
                workSheet.Cells[RowCount, ++ColumnCount] = new Cell(data.CustomCol);
                workSheet.Cells[RowCount, ++ColumnCount] = new Cell(data.VaildAttendCount.ToString());

                int index = 0;
                foreach (var item in dynamicCols)
                {
                    if (item.Visibility == Visibility.Visible)
                    {
                        workSheet.Cells[RowCount, ++ColumnCount] = new Cell(data.DailyContent.ElementAt(index));
                        index++;
                    }
                }
            }

            //add worksheet to workbook
            workBook.Worksheets.Add(workSheet);
            // get the selected file's stream
            try
            {
                using (Stream sFile = sDialog.OpenFile())
                {
                    workBook.Save(sFile);
                }
            }
            catch (Exception eee)
            {
                MessageBox.Show(eee.Message);
            }
            
            
            
            #endregion

        }

        #endregion

        #region 打印预览

        /// <summary>
        /// 预览打印数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            if (this.GuoDianReportDatas.Count < 1)
            {
                Dialog.MsgBoxWindow.MsgBox(
                           "请查询到数据后再进行打印预览！",
                           Dialog.MsgBoxWindow.MsgIcon.Information,
                           Dialog.MsgBoxWindow.MsgBtns.OK);

                WaitingDialog.HideWaiting();
                return;
            }
            WaitingDialog.ShowWaiting("正在打印预览数据，请稍后", 500, new Action(btnPrintEvent));
        }

        /// <summary>
        /// 预览打印数据
        /// </summary>
        void btnPrintEvent()
        {

            #region     生成表头数据
            List<HeaderNode> pageHeaderData = new List<HeaderNode>();

           
            pageHeaderData.Add(new HeaderNode("姓名", 11, 0, 1));
            pageHeaderData.Add(new HeaderNode("工号", 11, 0, 1));
            pageHeaderData.Add(new HeaderNode(customCol.Header.ToString(), 11, 0, 1));
            pageHeaderData.Add(new HeaderNode("出勤数", 11, 0, 1));

            for (int i = 0; i < dynamicColsCount; i++)
            {
                string header = dynamicCols[i].Header.ToString();
                pageHeaderData.Add(new HeaderNode(header, 11, 0, 1));
            }
            

            ReportHeader reportHeader = new ReportHeader(pageHeaderData);
            #endregion

            #region 表标题
            string time = beginTime.ToLongDateString() + " 到 " + endTime.AddDays(-1).ToLongDateString();

            ReportTitle reportTitle = new ReportTitle(this.txtTitle.Text, 20, time, 11, false);
            #endregion

            #region     生成报表页脚数据
            ReportFooter reportFooter = new ReportFooter("", 11);
            #endregion

            string[] bindingPropertyNames = new string[] { 
                "PersonName",
                "WorkSn",               
                "CustomCol",
                "VaildAttendCount",
                "DailyContent",
               };

            PrintControl printControl = new PrintControl();
            bool res = false;
            res = printControl.SetDataSource<GuoDianReportData>(reportTitle, reportFooter, reportHeader, GuoDianReportDatas, bindingPropertyNames, () =>
            {
                if (res)
                {
                    printControl.Preview_CurPage(null, null);
                }
            }, 9);

            WaitingDialog.HideWaiting();
        }


        #endregion 

        #region 排序

        private void dataGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MyDataGridSortInChinese.OrderData(sender, e, GuoDianReportDatas);
        }

        private void dataGrid_MouseMove(object sender, MouseEventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        private void dataGrid_LayoutUpdated(object sender, EventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        private void dataGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        #endregion


    }
}
