/*************************************************************************
** 文件名:   AttendDepartInWellDetail.cs
×× 主要类:   AttendDepartInWellDetail
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2014-9-9
** 修改人:   
** 日  期:   
** 描  述:   AttendDepartInWellDetail类，部门井下出勤详细信息表
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

namespace IriskingAttend.XinJuLong
{
    public partial class DlgDepartInWellDetail : ChildWindow
    {
        private VmDepartInWellCollect _vmDepartInWellCollect = new VmDepartInWellCollect();

        #region 构造函数 初始化       

        /// <summary>
        /// 构造函数
        /// </summary>
        public DlgDepartInWellDetail(VmDepartInWellCollect vmDepartInWellCollect)
        {
            InitializeComponent();

            this._vmDepartInWellCollect = vmDepartInWellCollect;

            dgDepartAttend.ItemsSource = _vmDepartInWellCollect.DepartInWellDetailModel;                 
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
            if (this._vmDepartInWellCollect.DepartInWellCollectModel.Count() < 1)
            {
                Dialog.MsgBoxWindow.MsgBox(
                           "请查询到数据后再导出Excel！",
                           Dialog.MsgBoxWindow.MsgIcon.Information,
                           Dialog.MsgBoxWindow.MsgBtns.OK);

                WaitingDialog.HideWaiting();
                return;
            }
            string space = "                                                                       ";
            ExpExcel.ExportExcelFromDataGrid(this.dgDepartAttend, 1, 9, (space + "员工下井人员信息" + space), "员工下井人员信息");
        }

        #endregion       
    }
}
