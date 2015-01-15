/*************************************************************************
** 文件名:   DeviceManage.cs
** 主要类:   DeviceManage
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2013-6-14
** 修改人:   
** 日  期:
** 描  述:   DeviceManage类,设备管理界面
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
using System.Windows.Data;
using IriskingAttend.ExportExcel;
using IriskingAttend.Common;
using Irisking.Web.DataModel;
using IriskingAttend.View;
using IriskingAttend.YangMei;


namespace IriskingAttend.YangMei
{
    public partial class DeviceManage : Page
    {
        #region 私有变量初始化：vm变量，排序字典集合

        //vm变量初始化
        private VmDeviceManage _vmDeviceMng = new VmDeviceManage();

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数， 初始化
        /// </summary>
        public DeviceManage()
        {
            InitializeComponent();
            
            //通过后台从数据库中获得设备信息
            _vmDeviceMng.GetDeviceInfoTableRia();

            //vm数据绑定
            this.DataContext = _vmDeviceMng;

            //设备信息表数据源
            this.dgDevice.ItemsSource = _vmDeviceMng.SystemDeviceInfo;

            //当前设备列表选中行绑定
            System.Windows.Data.Binding binding = new System.Windows.Data.Binding("SelectDeviceInfoItem") { Mode = BindingMode.TwoWay, };
            dgDevice.SetBinding(DataGrid.SelectedItemProperty, binding);

            //全选按钮绑定
            _vmDeviceMng.MarkObj = this.Resources["MarkObject"] as MarkObject;

            //设备列表序号
            this.dgDevice.LoadingRow += (a, e) =>
            {
                int index = e.Row.GetIndex();
                var cell = dgDevice.Columns[1].GetCellContent(e.Row) as TextBlock;
                cell.Text = (index + 1).ToString();
            };

            dgDevice.AddHandler(MouseLeftButtonUpEvent, new MouseButtonEventHandler(dgDevice_MouseLeftButtonUp), true);

            _vmDeviceMng.DeviceLoadCompleted += (o, e) =>
                {
                    _vmDeviceMng.UpdateCheckAllState();
                };

            ////设备详细信息加载完成后加载设备信息
            //_vmDeviceMng.DeviceLoadCompleted += (o, e) =>
            //    {
            //        foreach (object item in dgDevice.ItemsSource)
            //        {
            //            var cell = dgDevice.Columns[4].GetCellContent(item) as StackPanel;
            //            ListBox lbDevType = cell.FindName("lbDevType") as ListBox;
            //            lbDevType.Dispatcher.BeginInvoke(new EventHandler((listBox, ee) => ListBox_Loaded(listBox, (RoutedEventArgs)ee)), new object[] { lbDevType, null });
            //        }
            //       // dgDevice.Dispatcher.BeginInvoke(new EventHandler((listBox, ee) => dgDevice_LayoutUpdated(listBox, (RoutedEventArgs)ee)), new object[] { dgDevice, null });   
            //    };
        }

        #endregion

        #region 控件事件响应

        /// <summary>
        /// 全选设备操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkSelectAll_Click(object sender, RoutedEventArgs e)
        {            
           _vmDeviceMng.SelectAllDevice(((CheckBox)sender).IsChecked.Value);    
        }

        /// <summary>
        /// 注册鼠标事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgDevice_LoadingRow(object sender, DataGridRowEventArgs e)
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
            if ( dgDevice.CurrentColumn == null || dgDevice.CurrentColumn.DisplayIndex != 0 )
            {
                return;
            }
            //且当前选中行的不为空
            if (dgDevice.SelectedItem == null)
            {
                return;
            }

            //更改当前选中行的设备选中状态
            DeviceInfo selectDevInfo = this.dgDevice.SelectedItem as DeviceInfo;
            _vmDeviceMng.ChangeDeviceCheckedState(selectDevInfo);
        }

        /// <summary>
        /// 修改设备信息  
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hbtnModifyDevice_Click(object sender, RoutedEventArgs e)
        {  
            //修改设备信息界面
            DlgDeviceItemMng dlgModifyDevice = new DlgDeviceItemMng(_vmDeviceMng.SelectDeviceInfoItem, DeviceOperate_callback);

            //增加设备与修改设备界面用的是同一个，隐藏批量添加按钮。
            dlgModifyDevice.btnBatchAdd.Visibility = Visibility.Collapsed;

            //修改设备信息时，设备ID不可以修改
            dlgModifyDevice.txtDevsn.IsReadOnly = true;

            //更改对话框标题
            dlgModifyDevice.Title = "修改设备";

            //增加设备与修改设备界面用的是同一个，将确定按钮绑定到修改设备命令
            System.Windows.Data.Binding binding = new System.Windows.Data.Binding("ModifyDeviceCommand") { Mode = BindingMode.TwoWay, };
            dlgModifyDevice.btnOK.SetBinding(Button.CommandProperty, binding);           

            //显示修改设备对话框
            dlgModifyDevice.Show();  
        }

        /// <summary>
        /// 删除某个设备，只删除某一行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hbtnDeleteDevice_Click(object sender, RoutedEventArgs e)
        {
            _vmDeviceMng.DeleteDevice();

            //设备信息表数据源，排序后必须重新更新数据源
            this.dgDevice.ItemsSource = _vmDeviceMng.SystemDeviceInfo;
        }

        /// <summary>
        ///  批量删除设备按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBatchDeleteDevice_Click(object sender, RoutedEventArgs e)
        {
            _vmDeviceMng.BatchDeleteDevice();

            //设备信息表数据源，排序后必须重新更新数据源
            this.dgDevice.ItemsSource = _vmDeviceMng.SystemDeviceInfo;
        }

        /// <summary>
        /// 批量修改设备用途
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBatchModifyDevice_Click(object sender, RoutedEventArgs e)
        {
            //添加设备默认显示的设备信息
            List<DeviceInfo> devInfos = new List<DeviceInfo>();

            //将选中的设备添加到列表中           
            foreach (DeviceInfo dev in  _vmDeviceMng.SystemDeviceInfo)
            {
                if (dev.isSelected)
                {
                    devInfos.Add(dev);
                }
            }

            //修改设备信息界面
            DlgDeviceItemMng dlgModifyDevice = new DlgDeviceItemMng(devInfos.ToArray(), DeviceOperate_callback);

            //增加设备与修改设备界面用的是同一个，隐藏批量添加按钮。
            dlgModifyDevice.btnBatchAdd.Visibility = Visibility.Collapsed;

            //修改设备信息时，设备ID不可以修改
            dlgModifyDevice.txtDevsn.Visibility = Visibility.Collapsed;

            dlgModifyDevice.txtPlace.Visibility = Visibility.Collapsed;

            dlgModifyDevice.labDevSn.Visibility = Visibility.Collapsed;

            dlgModifyDevice.labDevPlace.Visibility = Visibility.Collapsed;

            dlgModifyDevice.labDevSnNote.Visibility = Visibility.Collapsed;

            dlgModifyDevice.txtDevFunc.Visibility = Visibility.Collapsed;

            dlgModifyDevice.labDevFuntion.Visibility = Visibility.Collapsed;

            //更改对话框标题
            dlgModifyDevice.Title = "批量修改设备用途";

            //增加设备与修改设备界面用的是同一个，将确定按钮绑定到修改设备命令
            System.Windows.Data.Binding binding = new System.Windows.Data.Binding("BatchModifyDeviceCommand") { Mode = BindingMode.TwoWay, };
            dlgModifyDevice.btnOK.SetBinding(Button.CommandProperty, binding);

            //显示修改设备对话框
            dlgModifyDevice.Show();  
        }

        /// <summary>
        ///  添加设备按钮事件响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddDevice_Click(object sender, RoutedEventArgs e)
        {
            //添加设备默认显示的设备信息
            DeviceInfo deviceInfoItem = new DeviceInfo();
            deviceInfoItem.dev_sn = "";
            deviceInfoItem.dev_type = -1;
            deviceInfoItem.place = "";

            DlgDeviceItemMng dlgAddDevice = new DlgDeviceItemMng(deviceInfoItem, DeviceOperate_callback);

            //增加设备与修改设备界面用的是同一个，将确定按钮绑定到增加设备命令
            System.Windows.Data.Binding binding = new System.Windows.Data.Binding("AddDeviceCommand") { Mode = BindingMode.TwoWay, };
            dlgAddDevice.btnOK.SetBinding(Button.CommandProperty, binding);

            //显示添加设备对话框
            dlgAddDevice.Show();               
        }        

        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            string space = "                                            ";

            ExpExcel.ExportExcelFromDataGrid(this.dgDevice, 2, 6, (space + " 设备管理" + space), "设备管理");
        }

        #endregion

        #region 回调函数

        /// <summary>
        /// 当前窗口的子窗口返回后，调用的回调函数
        /// </summary>
        /// <param name="dialogResult">是否需要刷新当前datagrid列表</param>
        private void DeviceOperate_callback(bool? dialogResult)
        {
            if (dialogResult.HasValue && dialogResult.Value)
            {
                //重新查询数据库
                _vmDeviceMng.GetDeviceInfoTableRia();                

                //设备信息表数据源，排序后必须重新更新数据源
                this.dgDevice.ItemsSource = _vmDeviceMng.SystemDeviceInfo;
            }
        }

        #endregion

        #region 设备列表排序初始化及控件事件响应 

        /// <summary>
        /// 设备列表中，鼠标移动事件，显示排序的上下箭头
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgDevice_MouseMove(object sender, MouseEventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        /// <summary>
        /// 设备列表中，外观刷新事件，显示排序的上下箭头
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgDevice_LayoutUpdated(object sender, EventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        /// <summary>
        /// 设备列表左键点击事件发生时进行排序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgDevice_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MyDataGridSortInChinese.OrderData(sender, e, ((VmDeviceManage)this.DataContext).SystemDeviceInfo);
        }

        private void dgDevice_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        #endregion          

        /// <summary>
        /// 手动绑定ListBox的数据源,
        /// 如果在xaml上绑定，则ListBox只会显示一行数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void ListBox_Loaded(object sender, RoutedEventArgs e)
        //{
        //    ListBox listBox = sender as ListBox;

        //    listBox.ItemsSource = null;
        //    Binding binding = new Binding("dev_type_List");
        //    binding.Mode = BindingMode.OneWay;
        //    binding.Source = listBox.DataContext;
        //    listBox.SetBinding(ListBox.ItemsSourceProperty, binding);
        //}
    }
}
