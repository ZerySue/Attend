/*************************************************************************
** 文件名:   Xls_OriginRecSumReport.cs
×× 主要类:   Xls_OriginRecSumReport
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   lzc
** 日  期:   2013-5-30
** 修改人:   gqy
** 日  期:   2013-08-20
** 描  述:   Xls_OriginRecSumReport类，原始记录汇总表
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
using IriskingAttend.ViewModel.PeopleViewModel;
using IriskingAttend.ViewModel;
using System.IO.IsolatedStorage;
using Irisking.Web.DataModel;
using System.ServiceModel.DomainServices.Client;
using EDatabaseError;
using ReportTemplate;

using Lite.ExcelLibrary.CompoundDocumentFormat;
using Lite.ExcelLibrary.BinaryFileFormat;
using Lite.ExcelLibrary.SpreadSheet;

using System.Collections.ObjectModel;
using System.Collections;
using System.Reflection;
using System.IO;
using IriskingAttend.Common;
using System.Threading;
using IriskingAttend.ApplicationType;

namespace IriskingAttend
{
    public partial class Xls_OriginRecSumReport : Page
    {
        #region  私有变量
        /// <summary>
        /// XlsServiceDomDbAcess
        /// </summary>
        private XlsServiceDomDbAcess _xls = new XlsServiceDomDbAcess();

        /// <summary>
        /// 班次类型vm数据MODE
        /// </summary>
        private VmClassType _vmClassType = new VmClassType();

        /// <summary>
        /// 部门vm数据MODE
        /// </summary>
        private VmDepartment _vmDepartment = new VmDepartment();

        /// <summary>
        /// 文件存储对话框
        /// </summary>
        private SaveFileDialog _sDialog;

        /// <summary>
        /// 是否为第一次加载
        /// </summary>
        private bool _isOnceLoad = true;

        /// <summary>
        /// 报表DataGrid起始插入日期列--报表定制
        /// </summary>
        private const int _colInsert = 5;

        /// <summary>
        /// 报表DataGrid最大日期列--报表定制
        /// </summary>
        private const int _colMax = 36;

        /// <summary>
        /// 本地存储
        /// </summary>
        private IsolatedStorageSettings querySetting = IsolatedStorageSettings.ApplicationSettings;

        #endregion

        #region 构造函数 初始化
        public Xls_OriginRecSumReport()
        {
            InitializeComponent();
            SetInit();

            //added begin by gqy at 2013-08-20 打印预览和导出Excel的权限            
            if (!VmLogin.DictPrivilege[AbstractApp.PrivilegeENUM.ReportOriginRecordExportExcel])
            {
                this.btnExportExcel.Visibility = Visibility.Collapsed;
            }

            if (!VmLogin.DictPrivilege[AbstractApp.PrivilegeENUM.ReportOriginRecordPrint])
            {
                this.btnPrint.Visibility = Visibility.Collapsed;
            }           
            //added end by gqy at 2013-08-20
        }

        // 当用户导航到此页面时执行。
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }


        /// <summary>
        /// 初始化绑定
        /// </summary>
        private void SetInit()
        {
            _vmDepartment.GetDepartment();
            _vmDepartment.DepartmentLoadCompleted += delegate
            {
                if (listBoxDepart.Items.Count > 0)
                {
                    listBoxDepart.SelectedIndex = 0;
                }
                _vmClassType.GetClassType();
            };
            this.listBoxDepart.ItemsSource = _vmDepartment.DepartmentModel;
            this.listBoxDepart.DisplayMemberPath = "depart_name";
            this.comboBoxClassTypeName.ItemsSource = _vmClassType.classTypeModel;

            _vmClassType.classTypeModel.CollectionChanged += (oo, ee) =>
                {
                    if (_vmClassType.classTypeModel.Count > 0)
                    {
                        this.comboBoxClassTypeName.SelectedIndex = 0;
                    }
                };
            this.comboBoxClassTypeName.DisplayMemberPath = "class_type_name";

            
            this.dgXlsAttend.LoadingRow += (a, e) =>
            {
                try
                {
                    int index = e.Row.GetIndex();
                    if (_isOnceLoad)
                    {
                        _isOnceLoad = false;
                        int col = _colInsert;
                        foreach (var ar in _xls.XlsAttendDataGridModel[index].AttendDays)
                        {
                            if (ar.Day > 9)
                            {
                                dgXlsAttend.Columns[col].Header = ar.Day.ToString();
                            }
                            else
                            {
                                dgXlsAttend.Columns[col].Header = "0" + ar.Day.ToString();
                            }
                            dgXlsAttend.Columns[col++].Visibility = System.Windows.Visibility.Visible;
                        }
                        for (int i = col; i < _colMax; i++)
                        {
                            dgXlsAttend.Columns[i].Visibility = System.Windows.Visibility.Collapsed;
                        }
                    }

                    //object pb = dgXlsAttend.Columns[0].GetCellContent(e.Row);
                    //var cell = dgXlsAttend.Columns[0].GetCellContent(e.Row) as TextBlock;
                   // cell.Text = (index + 1).ToString();
                    int col2 = _colInsert;
                    foreach (var ar in _xls.XlsAttendDataGridModel[index].WorktimeHour)
                    {
                        var cell2 = dgXlsAttend.Columns[col2++].GetCellContent(e.Row) as TextBlock;
                        cell2.Text = ar;
                    }
                }
                catch (Exception eee)
                {
                    string str = eee.Message;
                }
            };
        }

        #endregion

        #region  事件 函数
        /// <summary>
        /// 重置条件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnResert_Click(object sender, RoutedEventArgs e)
        {
            this.dateBegin.Text = "";
            this.listBoxDepart.SelectedValue = null;
            this.comboBoxClassTypeName.SelectedValue = null;
            this.txtPersonName.Text = "";
            this.txtWorkSn.Text = "";
        }

        /// <summary>
        /// 设置查询条件
        /// </summary>
        private void SetCondition()
        {
            XlsReport qc = new XlsReport();

            qc.XlsbeginTime = this.dateBegin.SelectedDate.Value.Date;
            qc.XlsendTime = this.dateBegin.SelectedDate.Value.AddMonths(1);
            qc.Xlsname = txtPersonName.Text.Trim();
            qc.XlsworkSN = txtWorkSn.Text.Trim();
            ///暂支持单部门查询
            //if (listBoxDepart.SelectedItems.Count > 0)
            //{
            //    qc.XlsdepartIdLst = new int[listBoxDepart.SelectedItems.Count];
            //    for (int i = 0; i < listBoxDepart.SelectedItems.Count; i++)
            //    {
            //        qc.XlsdepartIdLst[i] = (listBoxDepart.SelectedItems[i] as depart).depart_id;
            //    }
            //}
            if (listBoxDepart.SelectedIndex > 0)
            {
                qc.XlsdepartIdLst = new int[1];
                qc.XlsdepartIdLst[0] = (listBoxDepart.SelectedItem as depart).depart_id;

            }
            else
            {
                int index = 0;
                qc.XlsdepartIdLst=new int[_vmDepartment.DepartmentModel.Count()];
                foreach (depart d in _vmDepartment.DepartmentModel)
                {
                    qc.XlsdepartIdLst[index++] = d.depart_id;
                }
            }

            if (comboBoxClassTypeName.SelectedValue != null)
            {
                qc.XlsClassTypeName = (comboBoxClassTypeName.SelectedItem as UserClassTypeInfo).class_type_id;
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
            if (querySetting.Contains("attendConditon"))
            {
                try
                {
                    XlsReport condition;
                    querySetting.TryGetValue<XlsReport>("attendConditon", out condition);
                    _xls.GetAttendBaoBiao(condition.XlsbeginTime, condition.XlsendTime, condition.XlsdepartIdLst, condition.XlsClassTypeName, condition.Xlsname, condition.XlsworkSN);
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
        /// 查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQuery_Click(object sender, RoutedEventArgs e)
        {
            ///绑定数据源
            dgXlsAttend.ItemsSource = _xls.XlsAttendDataGridModel;
            if (dateBegin.Text == null || dateBegin.Text == "")
            {
                Dialog.MsgBoxWindow.MsgBox(
                             "查询时间不能为空！",
                             Dialog.MsgBoxWindow.MsgIcon.Information,
                             Dialog.MsgBoxWindow.MsgBtns.OK);  
            }
            else
            {
                _isOnceLoad = true;
                SetCondition();
                GetAttend();
            }
        }

        /// <summary>
        /// 导出数据到Excel中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            if (_xls.XlsAttend.Count() < 1)
            {
                Dialog.MsgBoxWindow.MsgBox(
                            "请查询到数据后再导出Excel！",
                            Dialog.MsgBoxWindow.MsgIcon.Information,
                            Dialog.MsgBoxWindow.MsgBtns.OK);  
                return;
            }
            DateTime beginTime = new DateTime();
            DateTime endTime = new DateTime();
            if (querySetting.Contains("attendConditon"))
            {
                XlsReport condition;
                querySetting.TryGetValue<XlsReport>("attendConditon", out condition);
                beginTime = condition.XlsbeginTime;
                endTime = condition.XlsendTime;
            }
            else
            {
                return;
            }
            if (dateBegin.Text == null || dateBegin.Text == "")
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

            string time = beginTime.ToLongDateString() + " 到 " + beginTime.AddMonths(+1).AddDays(-1).ToLongDateString();
            string title = "原始记录汇总表";
            string space = "                                                                           ";

            Workbook workBook = new Workbook();
            Worksheet workSheet = new Worksheet(title);

            workSheet.Cells[0, 0] = new Cell(space + title + "    " + time + space);

            Int16 ColumnCount = -1;
            Int16 RowCount = 1;

            #region     生成表头

            workSheet.Cells[RowCount, ++ColumnCount] = new Cell("序号");
            workSheet.Cells[RowCount, ++ColumnCount] = new Cell("工号");
            workSheet.Cells[RowCount, ++ColumnCount] = new Cell("姓名");
            workSheet.Cells[RowCount, ++ColumnCount] = new Cell("部门名称");
            workSheet.Cells[RowCount, ++ColumnCount] = new Cell("班制");
            for (DateTime mindate = beginTime; mindate < endTime; mindate = mindate.AddDays(1))
            {
                workSheet.Cells[RowCount, ++ColumnCount] = new Cell(mindate.Day.ToString("D2"));
                workSheet.Cells.ColumnWidth[(byte)RowCount, (byte)ColumnCount] = 1500;
            }
            workSheet.Cells[RowCount, ++ColumnCount] = new Cell("出勤次数");
            workSheet.Cells[RowCount, ++ColumnCount] = new Cell("总工时");
            workSheet.Cells[RowCount, ++ColumnCount] = new Cell("平均工时");
            #endregion

            foreach (XlsAttend data in _xls.XlsAttend)
            {
                ColumnCount = 0;
                RowCount++;

                workSheet.Cells[RowCount, ColumnCount++] = new Cell(data.Index);
                workSheet.Cells[RowCount, ColumnCount++] = new Cell(data.WorkSn);
                workSheet.Cells[RowCount, ColumnCount++] = new Cell(data.PersonName);
                workSheet.Cells[RowCount, ColumnCount++] = new Cell(data.DepartName);
                workSheet.Cells[RowCount, ColumnCount++] = new Cell(data.ClassTypeName);

                foreach (string hour in data.WorktimeHour)
                {
                    workSheet.Cells[RowCount, ColumnCount++] = new Cell(hour);
                }

                workSheet.Cells[RowCount, ColumnCount++] = new Cell(data.SumAttendTimes);
                workSheet.Cells[RowCount, ColumnCount++] = new Cell(data.SumWorktimeShow);
                workSheet.Cells[RowCount, ColumnCount++] = new Cell(data.AvgWorktimeShow);
            }

            //add worksheet to workbook
            workBook.Worksheets.Add(workSheet);
            // get the selected file's stream
            Stream sFile = _sDialog.OpenFile();
            workBook.Save(sFile);
        }
        
        /// <summary>
        /// 预览打印数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            WaitingDialog.ShowWaiting("正在打印预览数据，请稍后！", 500, new Action(btnPrintEvent));     
        }

        /// <summary>
        /// 预览打印数据
        /// </summary>
        void btnPrintEvent()
        {
            if (_xls.XlsAttend.Count() < 1)
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
            if (querySetting.Contains("attendConditon"))
            {
                XlsReport condition;
                querySetting.TryGetValue<XlsReport>("attendConditon", out condition);
                beginTime = condition.XlsbeginTime;
                endTime = condition.XlsendTime;
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
            pageHeaderData.Add(new HeaderNode("部门名称", 11, 0, 1));
            pageHeaderData.Add(new HeaderNode("班制", 11, 0, 1));
            for (DateTime mindate = beginTime; mindate < endTime; mindate = mindate.AddDays(1))
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
            pageHeaderData.Add(new HeaderNode("出勤次数", 11, 0, 1));
            pageHeaderData.Add(new HeaderNode("总工时", 11, 0, 1));
            pageHeaderData.Add(new HeaderNode("平均工时", 11, 0, 1));


            ReportHeader reportHeader = new ReportHeader(pageHeaderData);
            #endregion

            #region 表标题
            string time = beginTime.ToLongDateString() + " 到 " + endTime.AddDays(-1).ToLongDateString();

            ReportTitle reportTitle = new ReportTitle("原始记录汇总表", 20, time, 11, false);
            #endregion

            #region     生成报表页脚数据
            ReportFooter reportFooter = new ReportFooter("", 11);
            #endregion

            string[] bindingPropertyNames = new string[] { 
                "Index",
                "WorkSn",
                "PersonName",
                "DepartName",
                "ClassTypeName",
                "WorktimeHour",
                "SumAttendTimes",
                "SumWorktimeShow",
                "AvgWorktimeShow",
               };

            PrintControl printControl = new PrintControl();
            bool res= false;
            res = printControl.SetDataSource<XlsAttend>(reportTitle, reportFooter, reportHeader, _xls.XlsAttend, bindingPropertyNames,()=>
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
