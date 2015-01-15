/*************************************************************************
** 文件名:   AttendDepartInWellCollect.cs
×× 主要类:   AttendDepartInWellCollect
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2014-3-18
** 修改人:   
** 日  期:   
** 描  述:   AttendDepartInWellCollect类，部门井下出勤汇总表
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
    public partial class AttendDepartInWellCollect : Page
    {
        #region 私有变量

        /// <summary>
        /// 选中的部门
        /// </summary>
        private List<UserDepartInfo> _departSelect = new List<UserDepartInfo>();

        private VmDepartInWellCollect _vmDepartInWellCollect = new VmDepartInWellCollect();

        private DateTime beginTime = new DateTime();
        private DateTime endTime = new DateTime();       
      
        #endregion

        #region 构造函数 初始化

        /// <summary>
        /// 构造函数
        /// </summary>
        public AttendDepartInWellCollect()
        {
            InitializeComponent();

            dgDepartAttend.ItemsSource = _vmDepartInWellCollect.DepartInWellCollectModel;

            //注册行加载事件
            dgDepartAttend.LoadingRow += new EventHandler<DataGridRowEventArgs>(dgDepartAttend_LoadingRow);

            timeEnd.Value = Convert.ToDateTime("00:00:00");
            timeBegin.Value = Convert.ToDateTime("00:00:00"); 
        }
       
        #endregion

        #region 点击报表单元格出现详细信息相关

        /// <summary>
        /// 加载行事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dgDepartAttend_LoadingRow(object sender, DataGridRowEventArgs e)
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
            int selectIndex = dgDepartAttend.CurrentColumn.DisplayIndex;

             AttendDepartInWellQuery temp = (AttendDepartInWellQuery)this.dgDepartAttend.SelectedItem;
             if (temp.DepartId == -1)
             {
                 return;
             }

            string attendSign = "";

            if (selectIndex == 2)
            {
                attendSign = "夜";
            }
            else if (selectIndex == 3)
            {
                attendSign = "早";
            }
            else if (selectIndex == 4)
            {
                attendSign = "中";
            }
            else if (selectIndex == 5)
            {
                attendSign = "一";
            }
            else if (selectIndex == 6)
            {
                attendSign = "二";
            }
            else if (selectIndex == 7)
            {
                attendSign = "三";
            }
            else if (selectIndex == 8)
            {
                attendSign = "四";
            }
            else
            {
                return;
            }

            _vmDepartInWellCollect.GetAttendDepartInWellDetail( beginTime, endTime, temp.DepartId, attendSign);

            DlgDepartInWellDetail detail = new DlgDepartInWellDetail(_vmDepartInWellCollect);

            detail.Show();

            
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
                endTime = DateTime.Now.Date;
            }
            else
            {                
                endTime = dtpEnd.SelectedDate.Value.Date;
            }

            if (timeEnd.Value != null)
            {
                endTime += timeEnd.Value.Value.TimeOfDay;
            }
            #endregion

            if (_departSelect.Count() > 0)
            {
                _vmDepartInWellCollect.GetAttendDepartInWellCollect(beginTime, endTime, _departSelect);
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

                        _vmDepartInWellCollect.GetAttendDepartInWellCollect(beginTime, endTime, depart);
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
            if (this._vmDepartInWellCollect.DepartInWellCollectModel.Count() < 1)
            {
                Dialog.MsgBoxWindow.MsgBox(
                           "请查询到数据后再导出Excel！",
                           Dialog.MsgBoxWindow.MsgIcon.Information,
                           Dialog.MsgBoxWindow.MsgBtns.OK);

                WaitingDialog.HideWaiting();
                return;
            }

            string space = "                                                                                ";
            ExpExcel.ExportExcelFromDataGrid(this.dgDepartAttend, 1, 9, (space + "当天下井人数" + space), "当天下井人数");     
        }

        #endregion
    }
}
