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

namespace IriskingAttend
{
    public partial class DepartAttendCollect : Page
    {
        #region 私有变量

        /// <summary>
        /// 选中的部门
        /// </summary>
        private List<UserDepartInfo> _departSelect = new List<UserDepartInfo>();

        private VmDepartAttendCollect _vmDepartAttendCollect = new VmDepartAttendCollect();

        private DateTime beginTime = new DateTime();
        private DateTime endTime = new DateTime();
      
        #endregion

        #region 构造函数 初始化

        /// <summary>
        /// 构造函数
        /// </summary>
        public DepartAttendCollect()
        {
            InitializeComponent();

            dgDepartAttend.ItemsSource = _vmDepartAttendCollect.DepartAttendModel;

            //打印预览和导出Excel的权限            
            if (!VmLogin.DictPrivilege[AbstractApp.PrivilegeENUM.ReportDepartAttendCollectExportExcel])
            {
                this.btnExportExcel.Visibility = Visibility.Collapsed;
            }

            if (!VmLogin.DictPrivilege[AbstractApp.PrivilegeENUM.ReportDepartAttendCollectPrint])
            {
                this.btnPrint.Visibility = Visibility.Collapsed;
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
                endTime = DateTime.Now;
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
                else
                {
                    endTime = dtpEnd.SelectedDate.Value.Date.AddDays(1);
                }
            }
            #endregion

            if (_departSelect.Count() > 0)
            {
                _vmDepartAttendCollect.GetDepartAttendCollect(beginTime, endTime, _departSelect);
            }
            else
            {
                List<UserDepartInfo> depart = new List<UserDepartInfo>();

                VmXlsFilter vmXlsFilter = new VmXlsFilter();

                vmXlsFilter.GetDepartmentByPrivilege();

                //部门加载完成
                vmXlsFilter.DepartLoadCompleted += delegate
                {
                    if (vmXlsFilter.DepartInfoModel.Count > 0)
                    {
                        //去部门列表中“全部”一项
                        vmXlsFilter.DepartInfoModel.RemoveAt(0);

                        foreach (UserDepartInfo item in vmXlsFilter.DepartInfoModel)
                        {
                            depart.Add(item);
                        }

                        _vmDepartAttendCollect.GetDepartAttendCollect(beginTime, endTime, depart);
                    }
                };
            }
            
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

        #endregion
       

        #region 导出excel

        /// <summary>
        /// 导出excel文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            if (this._vmDepartAttendCollect.DepartAttendModel.Count() < 1)
            {
                Dialog.MsgBoxWindow.MsgBox(
                           "请查询到数据后再导出Excel！",
                           Dialog.MsgBoxWindow.MsgIcon.Information,
                           Dialog.MsgBoxWindow.MsgBtns.OK);

                WaitingDialog.HideWaiting();
                return;
            }

            string space = "                              ";           

            SaveFileDialog _sDialog = new SaveFileDialog();
            _sDialog.Filter = "Excel Files(*.xls)|*.xls";

            if (_sDialog.ShowDialog() == false)
            {
                return;
            }

            string time = beginTime.ToLongDateString() + " 到 " + endTime.AddDays(-1).ToLongDateString();
            string title = "部门出勤汇总表";
        

            Workbook workBook = new Workbook();
            Worksheet workSheet = new Worksheet(title);

            workSheet.Cells[0, 0] = new Cell(space + title + "    " + time + space);

            Int16 ColumnCount = -1;
            Int16 RowCount = 1;

            #region     生成表头

            workSheet.Cells[RowCount, ++ColumnCount] = new Cell("序号");
            workSheet.Cells[RowCount, ++ColumnCount] = new Cell("部门");
            workSheet.Cells[RowCount, ++ColumnCount] = new Cell("夜班");
            workSheet.Cells[RowCount, ++ColumnCount] = new Cell("早班");
            workSheet.Cells[RowCount, ++ColumnCount] = new Cell("中班");
            workSheet.Cells[RowCount, ++ColumnCount] = new Cell("合计");
           
            #endregion

            workSheet.Cells.ColumnWidth[0, 5] = 5000;       
            foreach (DepartAttend data in _vmDepartAttendCollect.DepartAttendModel)
            {
                ColumnCount = 0;
                RowCount++;

                workSheet.Cells[RowCount, ColumnCount++] = new Cell(data.Index);
                workSheet.Cells[RowCount, ColumnCount++] = new Cell(data.DepartName);
                workSheet.Cells[RowCount, ColumnCount++] = new Cell(data.NightAttendTime);
                workSheet.Cells[RowCount, ColumnCount++] = new Cell(data.MoringAttendTime);
                workSheet.Cells[RowCount, ColumnCount++] = new Cell(data.MiddleAttendTime);
                workSheet.Cells[RowCount, ColumnCount++] = new Cell(data.SumAttendTime);              
            }

            //add worksheet to workbook
            workBook.Worksheets.Add(workSheet);
            // get the selected file's stream
            Stream sFile = _sDialog.OpenFile();
            workBook.Save(sFile);
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
            WaitingDialog.ShowWaiting("正在打印预览数据，请稍后", 500, new Action(btnPrintEvent));
        }

        /// <summary>
        /// 预览打印数据
        /// </summary>
        void btnPrintEvent()
        {
            if (this._vmDepartAttendCollect.DepartAttendModel.Count() < 1)
            {
                Dialog.MsgBoxWindow.MsgBox(
                           "请查询到数据后再进行打印预览！",
                           Dialog.MsgBoxWindow.MsgIcon.Information,
                           Dialog.MsgBoxWindow.MsgBtns.OK);

                WaitingDialog.HideWaiting();
                return;
            }

            #region     生成表头数据
            List<HeaderNode> pageHeaderData = new List<HeaderNode>();

            pageHeaderData.Add(new HeaderNode("序号", 11, 0, 1));
            pageHeaderData.Add(new HeaderNode("部门", 11, 0, 1));
            pageHeaderData.Add(new HeaderNode("夜班", 11, 0, 1));
            pageHeaderData.Add(new HeaderNode("早班", 11, 0, 1));
            pageHeaderData.Add(new HeaderNode("中班", 11, 0, 1));
            pageHeaderData.Add(new HeaderNode("合计", 11, 0, 1));

            ReportHeader reportHeader = new ReportHeader(pageHeaderData);
            #endregion

            #region 表标题
            string time = beginTime.ToLongDateString() + " 到 " + endTime.AddDays(-1).ToLongDateString();
            string leftTitle = DateTime.Now.ToString("打印时间：yyyy-MM-dd HH:mm:ss");
            ReportTitle reportTitle = new ReportTitle("部门考勤汇总表", 20, leftTitle,11, time, 11, false);
            #endregion

            #region     生成报表页脚数据
            ReportFooter reportFooter = new ReportFooter("", 11);
            #endregion

            string[] bindingPropertyNames = new string[] { 
                "Index",
                "DepartName",               
                "NightAttendTime",
                "MoringAttendTime",
                "MiddleAttendTime",
                "SumAttendTime",
               };

            PrintControl printControl = new PrintControl();
            bool res = false;
            res = printControl.SetDataSource<DepartAttend>(reportTitle, reportFooter, reportHeader, _vmDepartAttendCollect.DepartAttendModel, bindingPropertyNames, () =>
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
