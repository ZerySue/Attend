/*************************************************************************
** 文件名:   LocateRecordAdded.cs
×× 主要类:   LocateRecordAdded
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2014-5-21
** 修改人:   
** 日  期:   
** 描  述:   LocateRecordAdded类，五虎山根据虹膜记录添加的定位记录查询子窗口
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
using IriskingAttend.ViewModel;
using System.Windows.Data;
using IriskingAttend.View;
using IriskingAttend.Common;
using IriskingAttend.Web;
using IriskingAttend.Dialog;
using IriskingAttend.ViewModel.PeopleViewModel;
using Irisking.Web.DataModel;

namespace IriskingAttend.NewWuHuShan
{
    public partial class LocateRecordAdded : ChildWindow
    {
        VmLocateRecordAdded _vmLocateRecord = new VmLocateRecordAdded();

        public LocateRecordAdded()
        {
            InitializeComponent();

            this.DataContext = _vmLocateRecord;

            //定位记录表数据源
            this.dgLocateRecord.ItemsSource = _vmLocateRecord.LocateRecordModel;

            //当前定位记录列表选中行绑定
            System.Windows.Data.Binding binding = new System.Windows.Data.Binding("SelectLocateRecordItem") { Mode = BindingMode.TwoWay, };
            dgLocateRecord.SetBinding(DataGrid.SelectedItemProperty, binding);

            //全选按钮绑定
            _vmLocateRecord.MarkObj = this.Resources["MarkObject"] as MarkObject;

            //定位记录列表序号
            this.dgLocateRecord.LoadingRow += (a, e) =>
            {
                int index = e.Row.GetIndex();
                var cell = dgLocateRecord.Columns[1].GetCellContent(e.Row) as TextBlock;
                cell.Text = (index + 1).ToString();
            };

            dgLocateRecord.AddHandler(MouseLeftButtonUpEvent, new MouseButtonEventHandler(dgLocateRecord_MouseLeftButtonUp), true);

            _vmLocateRecord.LocateRecordLoadCompleted += (o, e) =>
            {
                _vmLocateRecord.UpdateCheckAllState();
            };

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
            if (dtpEnd.Text == null || dtpEnd.Text == "")
            {
                MsgBoxWindow.MsgBox(
                            "查询截止时间不能为空！",
                            Dialog.MsgBoxWindow.MsgIcon.Information,
                            Dialog.MsgBoxWindow.MsgBtns.OK);
                return;
            }
            #endregion

            List<int> departIds = new List<int>();
            foreach (var item in cmbDepart.Items)
            {
                CheckBox chkBox = item as CheckBox;
                if (chkBox != null && chkBox.IsChecked.Value)
                {
                    departIds.Add((int)chkBox.Tag);
                }
            }
            _vmLocateRecord.GetLocateRecordCollect(DateTime.Parse(dtpBegin.Text), DateTime.Parse(dtpEnd.Text), departIds.ToArray(), txtPersonName.Text, txtWorkSN.Text);
        }

        #region 控件事件响应

        /// <summary>
        /// 全选定位记录操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkSelectAll_Click(object sender, RoutedEventArgs e)
        {
            _vmLocateRecord.SelectAllLocateRecord(((CheckBox)sender).IsChecked.Value);
        }

        /// <summary>
        /// 注册鼠标事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgLocateRecord_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.MouseLeftButtonUp -= new MouseButtonEventHandler(Row_MouseLeftButtonUp);
            e.Row.MouseLeftButtonUp += new MouseButtonEventHandler(Row_MouseLeftButtonUp);
        }

        /// <summary>
        /// 点击行 选择item操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Row_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //选择的是第一列
            if (dgLocateRecord.CurrentColumn == null || dgLocateRecord.CurrentColumn.DisplayIndex != 0)
            {
                return;
            }
            //且当前选中行的不为空
            if (dgLocateRecord.SelectedItem == null)
            {
                return;
            }

            //更改当前选中行的定位记录选中状态
            LocateRecordAddedEntity selectRecordInfo = this.dgLocateRecord.SelectedItem as LocateRecordAddedEntity;
            _vmLocateRecord.ChangeLocateRecordCheckedState(selectRecordInfo);
        }
        #endregion

        #region 定位记录列表排序初始化及控件事件响应

        /// <summary>
        /// 定位记录列表中，鼠标移动事件，显示排序的上下箭头
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgLocateRecord_MouseMove(object sender, MouseEventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        /// <summary>
        /// 定位记录列表中，外观刷新事件，显示排序的上下箭头
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgLocateRecord_LayoutUpdated(object sender, EventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        /// <summary>
        /// 定位记录列表左键点击事件发生时进行排序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgLocateRecord_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MyDataGridSortInChinese.OrderData(sender, e, ((VmLocateRecordAdded)this.DataContext).LocateRecordModel);
        }

        private void dgLocateRecord_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        #endregion          

        private void btnBatchDeleteLocateRecord_Click(object sender, RoutedEventArgs e)
        {
            _vmLocateRecord.BatchDeleteLocateRecord();

            //定位记录信息表数据源，排序后必须重新更新数据源
            this.dgLocateRecord.ItemsSource = _vmLocateRecord.LocateRecordModel;
        }
    }
}

