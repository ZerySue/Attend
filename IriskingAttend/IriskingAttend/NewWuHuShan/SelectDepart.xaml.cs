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
using Irisking.Web.DataModel;
using IriskingAttend.Common;
using IriskingAttend.View;

namespace IriskingAttend.NewWuHuShan
{
    public partial class SelectDepart : ChildWindow
    {
        #region 变量声明

        /// <summary>
        /// 私有变量声明
        /// </summary>
        VmXlsFilter _vm = new VmXlsFilter();

        /// <summary>
        /// 回调函数
        /// </summary>
        private Action<BaseViewModelCollection<UserDepartInfo>> _callback;

        #endregion

        public SelectDepart(List<UserDepartInfo> departSelect, Action<BaseViewModelCollection<UserDepartInfo>> callback)
        {
            InitializeComponent();

            _vm.GetDepartmentByPrivilege();           

            this.DataContext = _vm;

            //全选绑定初始化
            _vm.MarkObj = this.Resources["MarkObject"] as MarkObject;

            //部门加载完成
            _vm.DepartLoadCompleted += delegate
            {
                if (_vm.DepartInfoModel.Count > 0)
                {
                    //去部门列表中“全部”一项
                    _vm.DepartInfoModel.RemoveAt(0);

                    //再次进入该页面时，记住上次选中的部门
                    foreach (UserDepartInfo ar in departSelect)
                    {
                        foreach (UserDepartInfo ud in _vm.DepartInfoModel)
                        {
                            if (ar.depart_id == ud.depart_id)
                            {
                                ud.isSelected = true;
                            }
                        }
                    }
                    var values = from u in _vm.DepartInfoModel orderby u.depart_name select u;
                    dgSelectDepart.ItemsSource = values;

                    //造一条假数据，用于运行vm层的控制全选按钮是否选中的代码
                    UserDepartInfo departAll = new UserDepartInfo { depart_id = -1, depart_name = "全部" };
                    ((VmXlsFilter)this.DataContext).SelectItemDepart(departAll);
                }
            };
            dgSelectDepart.ItemsSource = _vm.DepartInfoModel;

            _callback = callback;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            _callback(_vm.DepartInfoModel);
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        #region 复选框选取操作
        /// <summary>
        /// 复选框选取操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgSelectDepart_LoadingRow(object sender, DataGridRowEventArgs e)
        {

            dgSelectDepart.MouseLeftButtonUp -= new MouseButtonEventHandler(Row_MouseLeftButtonUp);
            dgSelectDepart.MouseLeftButtonUp += new MouseButtonEventHandler(Row_MouseLeftButtonUp);
        }

        private void Row_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (dgSelectDepart.CurrentColumn.DisplayIndex == 0)
            {
                if (dgSelectDepart.SelectedItem != null)
                {
                    var obj = dgSelectDepart.SelectedItem as UserDepartInfo;
                    ((VmXlsFilter)this.DataContext).SelectItemDepart(obj);
                }
            }
        }

        #endregion

        #region 全选操作

        /// <summary>
        /// 全选操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkSelectAll_Click(object sender, RoutedEventArgs e)
        {
            VmXlsFilter vm = this.DataContext as VmXlsFilter;
            if (vm != null)
            {
                vm.SelectAllDepart(((CheckBox)sender).IsChecked);
            }
        }

        #endregion

        #region 排序
        private void dgSelectDepart_LayoutUpdated(object sender, EventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        private void dgSelectDepart_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MyDataGridSortInChinese.OrderData(sender, e, ((VmXlsFilter)this.DataContext).DepartInfoModel);
        }

        private void dgSelectDepart_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        private void dgSelectDepart_MouseMove(object sender, MouseEventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }
        #endregion 
    }
}

