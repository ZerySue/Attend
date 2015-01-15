/*************************************************************************
** 文件名:   LeaderScheduling.cs
×× 主要类:   LeaderScheduling
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2014-4-10
** 修改人:   
** 日  期:   
** 描  述:   LeaderScheduling类，西沟一矿领导排班及值班查询表
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
using IriskingAttend.Web;
using IriskingAttend.ExportExcel;

namespace IriskingAttend.XiGouYiKuang
{
    public partial class LeaderScheduling : Page
    {
        #region 私有变量       

        private DateTime beginTime = new DateTime();
        private DateTime endTime = new DateTime();

        private VmLeaderScheduling _vmleaderSchedule = new VmLeaderScheduling();

        #endregion

        #region 构造函数 初始化

        /// <summary>
        /// 构造函数
        /// </summary>
        public LeaderScheduling()
        {
            InitializeComponent();

            _vmleaderSchedule.GetLeaderInfoList();

            dgLeaderSchdule.ItemsSource = _vmleaderSchedule.LeaderSchedulingList;

            dgLeaderSchdule.LoadingRow += (o, e) =>
                {
                    for (int index = 2; index < 7; index++)
                    {
                        var cell = dgLeaderSchdule.Columns[index].GetCellContent(e.Row) as StackPanel;
                        ComboBox cmbLeader = cell.FindName("cmbLeader") as ComboBox;
                        cmbLeader.ItemsSource = _vmleaderSchedule.LeaderInfoList;
                        cmbLeader.DisplayMemberPath = "person_name";
                        cmbLeader.SelectedValuePath = "person_id";
                    }                   
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

            _vmleaderSchedule.GetLeaderSchedulingList(beginTime, endTime);
                   
        }

        #endregion


        #region 按钮响应事件

        /// <summary>
        /// 导出excel文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            if (this._vmleaderSchedule.LeaderSchedulingList.Count() < 1)
            {
                Dialog.MsgBoxWindow.MsgBox(
                           "请查询到数据后再导出Excel！",
                           Dialog.MsgBoxWindow.MsgIcon.Information,
                           Dialog.MsgBoxWindow.MsgBtns.OK);

                WaitingDialog.HideWaiting();
                return;
            }
           
           
            SaveFileDialog  sDialog = new SaveFileDialog();
            sDialog.Filter = "Excel Files(*.xls)|*.xls";

            if (sDialog.ShowDialog() == false)
            {
                return;
            }     
           
            string title = "领导值班表";
            string space = "                                             ";

            Workbook workBook = new Workbook();
            Worksheet workSheet = new Worksheet(title);

            workSheet.Cells[0, 0] = new Cell(space + title + space);

            Int16 ColumnCount = -1;
            Int16 RowCount = 1;

            #region     生成表头

            workSheet.Cells[RowCount, ++ColumnCount] = new Cell("日期");
            workSheet.Cells[RowCount, ++ColumnCount] = new Cell("星期");
            workSheet.Cells[RowCount, ++ColumnCount] = new Cell("当天值班领导1");
            workSheet.Cells[RowCount, ++ColumnCount] = new Cell("当天值班领导2");
            workSheet.Cells[RowCount, ++ColumnCount] = new Cell("早班带班领导");
            workSheet.Cells[RowCount, ++ColumnCount] = new Cell("中班带班领导");
            workSheet.Cells[RowCount, ++ColumnCount] = new Cell("夜班带班领导");

            workSheet.Cells.ColumnWidth[0, 6] = 4000;
            #endregion

            foreach (XiGouLeaderScheduling data in _vmleaderSchedule.LeaderSchedulingList)
            {
                ColumnCount = 0;
                RowCount++;

                workSheet.Cells[RowCount, ColumnCount++] = new Cell(data.DateId);
                workSheet.Cells[RowCount, ColumnCount++] = new Cell(data.WeekDay);
                if (data.TodayLeaderId1 != -1)
                {
                    workSheet.Cells[RowCount, ColumnCount++] = new Cell(_vmleaderSchedule.LeaderInfoList.Where(person => person.person_id == data.TodayLeaderId1).First().person_name);
                }
                else
                {
                    workSheet.Cells[RowCount, ColumnCount++] = new Cell("");
                }

                if (data.TodayLeaderId2 != -1)
                {
                    workSheet.Cells[RowCount, ColumnCount++] = new Cell(_vmleaderSchedule.LeaderInfoList.Where(person => person.person_id == data.TodayLeaderId2).First().person_name);
                }
                else
                {
                    workSheet.Cells[RowCount, ColumnCount++] = new Cell("");
                }
                if (data.MorningLeaderId != -1)
                {
                    workSheet.Cells[RowCount, ColumnCount++] = new Cell(_vmleaderSchedule.LeaderInfoList.Where(person => person.person_id == data.MorningLeaderId).First().person_name);
                }
                else
                {
                    workSheet.Cells[RowCount, ColumnCount++] = new Cell("");
                }
                if (data.MidLeaderId != -1)
                {
                    workSheet.Cells[RowCount, ColumnCount++] = new Cell(_vmleaderSchedule.LeaderInfoList.Where(person => person.person_id == data.MidLeaderId).First().person_name);
                }
                else
                {
                    workSheet.Cells[RowCount, ColumnCount++] = new Cell("");
                }
                if (data.NigntLeaderId != -1)
                {
                    workSheet.Cells[RowCount, ColumnCount++] = new Cell(_vmleaderSchedule.LeaderInfoList.Where(person => person.person_id == data.NigntLeaderId).First().person_name);
                }
                else
                {
                    workSheet.Cells[RowCount, ColumnCount++] = new Cell("");
                }
            }

            //add worksheet to workbook
            workBook.Worksheets.Add(workSheet);
            // get the selected file's stream
            Stream sFile = sDialog.OpenFile();
            workBook.Save(sFile);
        }

        private void btnSetLeaderSchedule_Click(object sender, RoutedEventArgs e)
        {
            if (this._vmleaderSchedule.LeaderSchedulingList.Count() < 1)
            {
                Dialog.MsgBoxWindow.MsgBox(
                           "请查询到数据后再进行设置！",
                           Dialog.MsgBoxWindow.MsgIcon.Information,
                           Dialog.MsgBoxWindow.MsgBtns.OK);

                WaitingDialog.HideWaiting();
                return;
            }
            this._vmleaderSchedule.SetLeaderScheduling( beginTime,endTime);
        }

        #endregion       

       
    }
}
