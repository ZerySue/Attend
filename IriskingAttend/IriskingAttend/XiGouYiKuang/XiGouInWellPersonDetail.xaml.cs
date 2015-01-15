/*************************************************************************
** 文件名:   XiGouInWellPersonDetail.cs
×× 主要类:   XiGouInWellPersonDetail
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2014-10-29
** 修改人:   
** 日  期:   
** 描  述:   XiGouInWellPersonDetail类，西沟一矿井下人员明细表
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
    public partial class XiGouInWellPersonDetail : Page
    {
        #region 私有变量      

        private VmXiGouAttendReport _vmXiGouAttendReport = new VmXiGouAttendReport();

        private DateTime beginTime = new DateTime();
        private DateTime endTime = new DateTime();       
      
        #endregion

        #region 构造函数 初始化

        /// <summary>
        /// 构造函数
        /// </summary>
        public XiGouInWellPersonDetail()
        {
            InitializeComponent();

            //vm数据绑定
            this.DataContext = _vmXiGouAttendReport;

            dgPersonDetailAttend.ItemsSource = _vmXiGouAttendReport.InWellPersonDetailModel;

            //报表序号
            this.dgPersonDetailAttend.LoadingRow += (a, e) =>
            {
                int index = e.Row.GetIndex();
                var cell = dgPersonDetailAttend.Columns[0].GetCellContent(e.Row) as TextBlock;
                cell.Text = (index + 1).ToString();
            };    
        
            dtpBegin.Text = DateTime.Now.ToString("yyyy-MM-dd 00:00:00");
            timeBegin.Value = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 00:00:00"));
            dtpEnd.Text = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            timeEnd.Value = DateTime.Now;
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
                            "查询时间不能为空！",
                            Dialog.MsgBoxWindow.MsgIcon.Information,
                            Dialog.MsgBoxWindow.MsgBtns.OK);
                return;
            }
            beginTime = dtpBegin.SelectedDate.Value.Date;

            if (timeBegin.Value != null)
            {
                beginTime += timeBegin.Value.Value.TimeOfDay;
            }

            if (dtpEnd.Text == null || dtpEnd.Text == "")
            {
                endTime = DateTime.Now;
            }
            else
            {                
                endTime = dtpEnd.SelectedDate.Value.Date;
                if (timeEnd.Value != null)
                {
                    endTime += timeEnd.Value.Value.TimeOfDay;
                }
            }
            
            #endregion
            
            _vmXiGouAttendReport.GetXiGouInWellPersonDetail(beginTime, endTime, txtName.Text.Trim());          
            
        }        

        #endregion

        #region 人员明细表排序初始化及控件事件响应        

        /// <summary>
        /// 人员明细表列表中，鼠标移动事件，显示排序的上下箭头
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgPersonDetailAttend_MouseMove(object sender, MouseEventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        /// <summary>
        /// 人员明细表列表中，外观刷新事件，显示排序的上下箭头
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgPersonDetailAttend_LayoutUpdated(object sender, EventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        /// <summary>
        /// 人员明细表列表左键点击事件发生时进行排序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgPersonDetailAttend_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MyDataGridSortInChinese.OrderData(sender, e, ((VmXiGouAttendReport)this.DataContext).InWellPersonDetailModel);
        }

        private void dgPersonDetailAttend_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
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
            if (this._vmXiGouAttendReport.InWellPersonDetailModel.Count() < 1)
            {
                Dialog.MsgBoxWindow.MsgBox(
                           "请查询到数据后再导出Excel！",
                           Dialog.MsgBoxWindow.MsgIcon.Information,
                           Dialog.MsgBoxWindow.MsgBtns.OK);

                WaitingDialog.HideWaiting();
                return;
            }

            string space = "                                             ";
            ExpExcel.ExportExcelFromDataGrid(this.dgPersonDetailAttend, 1, 7, (space +"     "+ "西沟一矿人员明细表" + space + "导出日期："+ DateTime.Now.ToString("yyyy年MM月dd日")), "西沟一矿人员明细表");     
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
        //    if (_vmXiGouAttendReport.InWellPersonDetailModel.Count() < 1)
        //    {
        //        Dialog.MsgBoxWindow.MsgBox(
        //                   "请查询到数据后再进行打印预览！",
        //                   Dialog.MsgBoxWindow.MsgIcon.Information,
        //                   Dialog.MsgBoxWindow.MsgBtns.OK);

        //        WaitingDialog.HideWaiting();
        //        return;
        //    }            

        //    #region     生成表头数据
        //    List<HeaderNode> pageHeaderData = new List<HeaderNode>();

        //    pageHeaderData.Add(new HeaderNode("序号", 11, 0, 1));            
        //    pageHeaderData.Add(new HeaderNode("姓名", 11, 0, 1));
        //    pageHeaderData.Add(new HeaderNode("职务", 11, 0, 1));
        //    pageHeaderData.Add(new HeaderNode("入井时间", 11, 0, 1));
        //    pageHeaderData.Add(new HeaderNode("升井时间", 11, 0, 1));
        //    pageHeaderData.Add(new HeaderNode("工作时长", 11, 0, 1));
        //    pageHeaderData.Add(new HeaderNode("班次", 11, 0, 1));           

        //    ReportHeader reportHeader = new ReportHeader(pageHeaderData);
        //    #endregion

        //    #region 表标题           
        //    string time = beginTime.ToString("yyyy-MM-dd HH:mm:ss") + " 到 " + endTime.ToString("yyyy-MM-dd HH:mm:ss");

        //    ReportTitle reportTitle = new ReportTitle("西沟一矿人员明细表", 20, time, 11, null, 11, false);
        //    #endregion

        //    #region     生成报表页脚数据
        //    ReportFooter reportFooter = new ReportFooter("", 11);
        //    #endregion

        //    string[] bindingPropertyNames = new string[] 
        //    { 
        //        "Index",
        //        "PersonName",                 
        //        "PrincipalName",   
        //        "InWellTime",
        //        "OutWellTime",
        //        "WorkTime", 
        //        "ClassOrderName",                                      
        //    };

        //    PrintControl printControl = new PrintControl();
        //    bool res = false;
        //    res = printControl.SetDataSource<XiGouInWellPersonDetailReport>(reportTitle, reportFooter, reportHeader, _vmXiGouAttendReport.InWellPersonDetailModel, bindingPropertyNames, () =>
        //    {
        //        if (res)
        //        {
        //            printControl.Preview_CurPage(null, null);
        //        }
        //    }, 9);
        //    WaitingDialog.HideWaiting();
        //}

        #endregion
    }
}
