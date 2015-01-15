/*************************************************************************
** 文件名:   FestivalMng.cs
** 主要类:   FestivalMng
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2013-12-12
** 修改人:   
** 日  期:
** 描  述:   FestivalMng类,节假日管理界面
**
** 版  本:   1.0.0
** 备  注:  命名及代码编写遵守C#编码规范
**
*************************************************************************/
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
using IriskingAttend.ViewModel.AttendViewModel;
using System.Windows.Data;
using IriskingAttend.ExportExcel;
using IriskingAttend.Common;
using IriskingAttend.Dialog;
using Irisking.Web.DataModel;

namespace IriskingAttend.View.AttendView
{
    public partial class FestivalMng : Page
    {
        #region 私有变量初始化：vm变量，排序字典集合

        //vm变量初始化
        private VmFestivalManage _vmFestivalMng = new VmFestivalManage();

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数， 初始化
        /// </summary>
        public FestivalMng()
        {
            InitializeComponent();
            
            //通过后台从数据库中获得节假日信息
            _vmFestivalMng.GetFestivalInfoTableRia();

            //vm数据绑定
            this.DataContext = _vmFestivalMng;

            //节假日信息表数据源
            this.dgFestival.ItemsSource = _vmFestivalMng.SystemFestivalInfo;

            //当前节假日列表选中行绑定
            System.Windows.Data.Binding binding = new System.Windows.Data.Binding("SelectFestivalInfoItem") { Mode = BindingMode.TwoWay, };
            dgFestival.SetBinding(DataGrid.SelectedItemProperty, binding);

            //全选按钮绑定
            _vmFestivalMng.MarkObj = this.Resources["MarkObject"] as MarkObject;

            //节假日列表序号
            this.dgFestival.LoadingRow += (a, e) =>
            {
                int index = e.Row.GetIndex();
                var cell = dgFestival.Columns[1].GetCellContent(e.Row) as TextBlock;
                cell.Text = (index + 1).ToString();
            };

            dgFestival.AddHandler(MouseLeftButtonUpEvent, new MouseButtonEventHandler(dgFestival_MouseLeftButtonUp), true);
        }

        #endregion

        #region 控件事件响应

        /// <summary>
        /// 全选节假日操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkSelectAll_Click(object sender, RoutedEventArgs e)
        {            
           _vmFestivalMng.SelectAllFestival(((CheckBox)sender).IsChecked.Value);           
        }

        /// <summary>
        /// 注册鼠标事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgFestival_LoadingRow(object sender, DataGridRowEventArgs e)
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
            if ( dgFestival.CurrentColumn == null || dgFestival.CurrentColumn.DisplayIndex != 0 )
            {
                return;
            }
            //且当前选中行的不为空
            if (dgFestival.SelectedItem == null)
            {
                return;
            }

            //更改当前选中行的节假日选中状态
            FestivalInfo selectDevInfo = this.dgFestival.SelectedItem as FestivalInfo;
            _vmFestivalMng.ChangeFestivalCheckedState(selectDevInfo);
        }

        /// <summary>
        /// 修改节假日信息  
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hbtnModifyFestival_Click(object sender, RoutedEventArgs e)
        {
            //修改节假日信息界面
            DlgFestivalItemMng dlgModifyFestival = new DlgFestivalItemMng(_vmFestivalMng.SelectFestivalInfoItem, FestivalOperate_callback);  

            //更改对话框标题
            dlgModifyFestival.Title = "修改节假日";

            //增加节假日与修改节假日界面用的是同一个，将确定按钮绑定到修改节假日命令
            System.Windows.Data.Binding binding = new System.Windows.Data.Binding("ModifyFestivalCommand") { Mode = BindingMode.TwoWay, };
            dlgModifyFestival.btnOK.SetBinding(Button.CommandProperty, binding);

            //显示修改节假日对话框
            dlgModifyFestival.Show();  
        }

        /// <summary>
        /// 删除某个节假日，只删除某一行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hbtnDeleteFestival_Click(object sender, RoutedEventArgs e)
        {
            _vmFestivalMng.DeleteFestival();

            //节假日信息表数据源，排序后必须重新更新数据源
            this.dgFestival.ItemsSource = _vmFestivalMng.SystemFestivalInfo;
        }

        /// <summary>
        ///  批量删除节假日按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBatchDeleteFestival_Click(object sender, RoutedEventArgs e)
        {
            _vmFestivalMng.BatchDeleteFestival();

            //节假日信息表数据源，排序后必须重新更新数据源
            this.dgFestival.ItemsSource = _vmFestivalMng.SystemFestivalInfo;
        }

        /// <summary>
        ///  添加节假日按钮事件响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddFestival_Click(object sender, RoutedEventArgs e)
        {
            //添加节假日默认显示的节假日信息
            FestivalInfo festivalInfoItem = new FestivalInfo();
            //festivalInfoItem.dev_sn = "";
            //festivalInfoItem.dev_type = -1;
            //festivalInfoItem.place = "";

            DlgFestivalItemMng dlgAddFestival = new DlgFestivalItemMng(festivalInfoItem, FestivalOperate_callback);

            //增加节假日与修改节假日界面用的是同一个，将确定按钮绑定到增加节假日命令
            System.Windows.Data.Binding binding = new System.Windows.Data.Binding("AddFestivalCommand") { Mode = BindingMode.TwoWay, };
            dlgAddFestival.btnOK.SetBinding(Button.CommandProperty, binding);

            //显示添加节假日对话框
            dlgAddFestival.Show();               
        }        

        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            string space = "                                                  ";
            ExpExcel.ExportExcelFromDataGrid(this.dgFestival, 2, 7, (space + "节假日管理" + space), "节假日管理");
        }

        #endregion

        #region 回调函数

        /// <summary>
        /// 当前窗口的子窗口返回后，调用的回调函数
        /// </summary>
        /// <param name="dialogResult">是否需要刷新当前datagrid列表</param>
        private void FestivalOperate_callback(bool? dialogResult)
        {
            if (dialogResult.HasValue && dialogResult.Value)
            {
                //重新查询数据库
                _vmFestivalMng.GetFestivalInfoTableRia();

                //节假日信息表数据源，排序后必须重新更新数据源
                this.dgFestival.ItemsSource = _vmFestivalMng.SystemFestivalInfo;
            }
        }

        #endregion

        #region 节假日列表排序初始化及控件事件响应 

        /// <summary>
        /// 节假日列表中，鼠标移动事件，显示排序的上下箭头
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgFestival_MouseMove(object sender, MouseEventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        /// <summary>
        /// 节假日列表中，外观刷新事件，显示排序的上下箭头
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgFestival_LayoutUpdated(object sender, EventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        /// <summary>
        /// 节假日列表左键点击事件发生时进行排序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgFestival_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MyDataGridSortInChinese.OrderData(sender, e, ((VmFestivalManage)this.DataContext).SystemFestivalInfo);
        }

        private void dgFestival_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        #endregion  
    }
}
