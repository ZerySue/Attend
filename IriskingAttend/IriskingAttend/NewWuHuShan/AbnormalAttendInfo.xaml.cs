/*************************************************************************
** 文件名:   AbnormalAttendInfo.cs
×× 主要类:   AbnormalAttendInfo
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   wz
** 日  期:   2013-12-20
** 修改人:   
** 日  期:
** 描  述:   AbnormalAttendInfo类,五虎山与定位卡结合的异常考勤查询界面
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
using IriskingAttend.ExportExcel;
using IriskingAttend.Common;
using Irisking.Web.DataModel;
using IriskingAttend.View;
using IriskingAttend.Web.WuHuShan;
using System.Windows.Data;
using ReportTemplate;
using System.ServiceModel.DomainServices.Client;
using EDatabaseError;
using IriskingAttend.ViewModel.PeopleViewModel;


namespace IriskingAttend.NewWuHuShan
{
    public partial class AbnormalAttendInfo : Page
    {
      
        #region  构造函数
        public AbnormalAttendInfo()
        {
            InitializeComponent();

            //viewModel层初始化
            VmAbnormalAttendInfo vm = new VmAbnormalAttendInfo();

            this.DataContext = vm;
           
            vm.MarkObj = this.Resources["MarkObject"] as MarkObject;
            vm.SetDeparts(this.cmbDepart);

            ///序号
            dgAttendLeave.LoadingRow += (a, e) =>
            {
                int index = e.Row.GetIndex();
                var cell = dgAttendLeave.Columns[1].GetCellContent(e.Row) as TextBlock;  //序号在第一列
                cell.Text = (index + 1).ToString();

              

                e.Row.MouseLeftButtonUp -= new MouseButtonEventHandler(Row_MouseLeftButtonUp);
                e.Row.MouseLeftButtonUp += new MouseButtonEventHandler(Row_MouseLeftButtonUp);
            };

            dgAttendLeave.UnloadingRow += (a, e) =>
                {
                    if (((DataGrid)a).ItemsSource != null)
                    {
                        e.Row.DataContext = null;
                    }
                };

            dgAttendLeave.AddHandler(MouseLeftButtonUpEvent, new MouseButtonEventHandler(dgAttendLeave_MouseLeftButtonUp), true);

            this.cmbDepart.Items.Clear();
            ComboBoxItem allItem = new ComboBoxItem();
            allItem.Content = "全部部门";
            allItem.AddHandler(ComboBoxItem.MouseLeftButtonDownEvent, new MouseButtonEventHandler(allItem_MouseLeftButtonDown), true);
            this.cmbDepart.Items.Add(allItem);
            
            VmDepartMng vmDepart = new VmDepartMng();

            vmDepart.GetDepartsInfosRia();

            vmDepart.DepartInfoLoadCompleted += (o, e) =>
            {
                foreach (UserDepartInfo ar in vmDepart.DepartInfos)
                {
                    CheckBox chkBox = new CheckBox();
                    chkBox.Content = ar.depart_name;
                    chkBox.Tag = ar.depart_id;
                    cmbDepart.Items.Add(chkBox);
                }
            };
            
        }
      
        #endregion


        /// <summary>
        /// 离开该页面
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            //如果正在批量添加识别记录则停止该操作
            ((VmAbnormalAttendInfo)this.DataContext).CancelAsyncTask();
            base.OnNavigatedFrom(e);

           
            object a =  cmbIrisErr.Content;
            
        }

        #region 响应事件函数
      

        //行点击事件
        void Row_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (dgAttendLeave.CurrentColumn == null)
            {
                return;
            }

        

            if (dgAttendLeave.CurrentColumn.DisplayIndex == 0)
            {
                if (dgAttendLeave.SelectedItem != null)
                {
                    var obj = dgAttendLeave.SelectedItem as AttendRecordInfo_WuhuShan;
                    ((VmAbnormalAttendInfo)this.DataContext).SelectItems(obj);
                }
            }
        }

        /// <summary>
        /// 排序箭头设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgAttendLeave_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
            //需要重绘
            dgAttendLeave.UpdateLayout();
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgAttendLeave_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                MyDataGridSortInChinese.OrderData(sender, e, ((VmAbnormalAttendInfo)dgAttendLeave.DataContext).AttendRecordInfos);
            }
            catch (Exception ex)
            {
                ErrorWindow errWin = new ErrorWindow(ex);
                errWin.Show();
            }
        }

        /// <summary>
        /// 排序箭头设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgAttendLeave_MouseMove(object sender, MouseEventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        /// <summary>
        /// 排序箭头设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgAttendLeave_LayoutUpdated(object sender, EventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }


        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportExl_Click(object sender, RoutedEventArgs e)
        {
            string space = "                                                                                         ";
            ExpExcel.ExportExcelFromDataGrid(dgAttendLeave, 2, 13, (space + "异常考勤" + space), "异常考勤");
        }
        #endregion

        #region 日期控件设置

        /// <summary>
        /// 设置DataPicker 日期的选择范围
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dateBegin_CalendarClosed(object sender, RoutedEventArgs e)
        {
            if (null != dateBegin.SelectedDate && null != dateEnd.SelectedDate)
            {
                if (dateBegin.SelectedDate.Value > dateEnd.SelectedDate.Value)
                {
                    MsgBoxWindow.MsgBox("选择结束时间不能早于开始时间！",
                        MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);
                    dateBegin.SelectedDate = null;

                    //btnQuery.IsEnabled = false;
                }
            }
        }

        /// <summary>
        /// 校验日期格式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dateBegin_DateValidationError(object sender, DatePickerDateValidationErrorEventArgs e)
        {
            MsgBoxWindow.MsgBox("日期格式不对！", MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);
        }

        /// <summary>
        /// 设置时间选择范围判定，起始时间应需要截止时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dateEnd_CalendarClosed(object sender, RoutedEventArgs e)
        {
            if (null != dateBegin.SelectedDate && null != dateEnd.SelectedDate)
            {
                if (dateBegin.SelectedDate.Value > dateEnd.SelectedDate.Value)
                {
                    MsgBoxWindow.MsgBox("选择结束时间不能早于开始时间！",
                        MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);
                    dateEnd.SelectedDate = null;
                }
            }
        }
        #endregion

        private void chkSelectAll_Click(object sender, RoutedEventArgs e)
        {
            ((VmAbnormalAttendInfo)this.DataContext).SelectAll(((CheckBox)sender).IsChecked);
        }

        //异常过滤
        private void cmbIrisErr_Click(object sender, RoutedEventArgs e)
        {
            ((VmAbnormalAttendInfo)this.DataContext).FilterAttendInfo(this.cmbIrisErr.IsChecked.Value,
                this.cmbLocateErr.IsChecked.Value, this.cmbIrisAndLocateErr.IsChecked.Value,
                this.cmbTimeErr.IsChecked.Value);
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
            var source = ((VmAbnormalAttendInfo)this.DataContext).AttendRecordInfos;
            var beginTime =((VmAbnormalAttendInfo)this.DataContext).BeginTime.Value;
            var endTime = ((VmAbnormalAttendInfo)this.DataContext).EndTime.Value;

            if (source.Count() < 1)
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

            pageHeaderData.Add(new HeaderNode("姓名", 11, 0, 1));
            pageHeaderData.Add(new HeaderNode("工号", 11, 0, 1));
            pageHeaderData.Add(new HeaderNode("部门", 11, 0, 1));
            pageHeaderData.Add(new HeaderNode("职务", 11, 0, 1));
            pageHeaderData.Add(new HeaderNode("虹膜入井时间", 11, 0, 1));
            pageHeaderData.Add(new HeaderNode("虹膜出井时间", 11, 0, 1));
            pageHeaderData.Add(new HeaderNode("工作时长", 11, 0, 1));
            pageHeaderData.Add(new HeaderNode("定位入井时间", 11, 0, 1));
            pageHeaderData.Add(new HeaderNode("定位出井时间", 11, 0, 1));
            pageHeaderData.Add(new HeaderNode("工作时长", 11, 0, 1));
            pageHeaderData.Add(new HeaderNode("考勤状态", 11, 0, 1));
           

            ReportHeader reportHeader = new ReportHeader(pageHeaderData);
            #endregion

            #region 表标题
            string time = beginTime.ToLongDateString() + " 到 " + endTime.ToLongDateString();

            ReportTitle reportTitle = new ReportTitle("异常考勤记录表", 20, time, 11, false);
            #endregion

            #region     生成报表页脚数据
            //考勤状态
            // 0 = 正常
            // 1 = 未完成出入井虹膜， 但出入井定位信息都有
            // 2 = 未完成出入井定位，但出入井虹膜都有
            // 3 = 出入井虹膜和出入井定位都有，但时间限制不满足
            // 4 = 出入井虹膜和出入井定位都残缺
            ReportFooter reportFooter = new ReportFooter(
                    "", 11);
            #endregion

            string[] bindingPropertyNames = new string[] { 
                "name",
                "work_sn",
                "depart_name",
                "principal_name",
                "in_well_time",
                "out_well_time",
                "iris_work_time",
                "in_locate_time",
                "out_locate_time",
                "locate_work_time",
                "attend_state",
               };

            PrintControl printControl = new PrintControl();
            bool res = false;
            res = printControl.SetDataSource<AttendRecordInfo_WuhuShan>(reportTitle, reportFooter, reportHeader, source, bindingPropertyNames, ()=>
            {
                if (res)
                {
                    printControl.Preview_CurPage(null, null);
                }
            },9);
           
            WaitingDialog.HideWaiting();
        }

        //显示全部部门对应的内容
        void allItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //更新checkbox属性
            foreach (var item in this.cmbDepart.Items)
            {
                if (item is CheckBox)
                {
                    ((CheckBox)item).IsChecked = false;
                }
            }
        }

        /// <summary>
        /// 调用查询定位记录页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QueryLocateRecord_Click(object sender, RoutedEventArgs e)
        {
            LocateRecordAdded dlg = new LocateRecordAdded();
            dlg.Show();
        }
    }
}
