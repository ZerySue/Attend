/*************************************************************************
** 文件名:   SelectPrincipal.cs
×× 主要类:   SelectPrincipal
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   cty
** 日  期:   2013-10-29
** 修改人:   
** 日  期:   
** 描  述:   SelectPrincipal类，五虎山PersonList界面查询条件中选择职务的页面
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
using Irisking.Web.DataModel;
using IriskingAttend.ViewModel;
using IriskingAttend.View;
using IriskingAttend.Common;

namespace IriskingAttend.NewWuHuShan
{
    public partial class SelectPrincipal : ChildWindow
    {
        #region 变量声明

        /// <summary>
        /// 私有变量声明
        /// </summary>
        VmXlsFilter _vm = new VmXlsFilter();

        /// <summary>
        /// 回调函数
        /// </summary>
        private Action<BaseViewModelCollection<PrincipalInfo>> _callback;

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public SelectPrincipal(List<PrincipalInfo> principalSelect,Action<BaseViewModelCollection<PrincipalInfo>> callback)
        {
            InitializeComponent();

            //每次进入该页面时确保是重新查询的数据
            _vm = new VmXlsFilter();

            _vm.GetPrincipalInfo();

            this.DataContext = _vm;

            //全选绑定初始化
            _vm.MarkObj = this.Resources["MarkObject"] as MarkObject;

            //职务加载完成
            _vm.PrincipalInfoLoadCompleted += delegate
            {
                if (_vm.PrincipalInfoModel.Count > 0)
                {
                    //去掉职务列表中“全部”一项
                    _vm.PrincipalInfoModel.RemoveAt(0);
                    
                    //再次进入该页面时，记住上次选中的职务
                    foreach (PrincipalInfo ar in principalSelect)
                    {
                        foreach (PrincipalInfo pr in _vm.PrincipalInfoModel)
                        {
                            if (ar.principal_id == pr.principal_id)
                            {
                                pr.isSelected = true;
                            }
                        }
                    }
                    
                    dgSelectPrincipal.ItemsSource = _vm.PrincipalInfoModel;

                    //造一条假数据，用于运行vm层的控制全选按钮是否选中的代码
                    PrincipalInfo principalAll = new PrincipalInfo { principal_id = 0, principal_name = "全部" };
                    ((VmXlsFilter)this.DataContext).SelectItemPrincipal(principalAll);
                }
            };

            _callback = callback;
        }

        #endregion

        #region 确定按钮操作

        /// <summary>
        /// 确定按钮操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {

            _callback(_vm.PrincipalInfoModel);
            this.DialogResult = true;

        }

        #endregion

        #region 取消按钮操作

        /// <summary>
        /// 取消操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            //foreach (PrincipalInfo pr in _vm.PrincipalInfoModel)
            //{
            //    pr.isSelected = false;
            //}
            this.DialogResult = false;
        }

        #endregion

        #region 复选框选取操作

        /// <summary>
        /// 复选框选取操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgSelectPrincipal_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            dgSelectPrincipal.MouseLeftButtonUp -= new MouseButtonEventHandler(Row_MouseLeftButtonUp);
            dgSelectPrincipal.MouseLeftButtonUp += new MouseButtonEventHandler(Row_MouseLeftButtonUp);
        }

        private void Row_MouseLeftButtonUp(object sender,MouseButtonEventArgs e)
        {
            if (dgSelectPrincipal.CurrentColumn.DisplayIndex == 0)
            {
                if (dgSelectPrincipal.SelectedItem != null)
                {
                    var obj = dgSelectPrincipal.SelectedItem as PrincipalInfo;
                    ((VmXlsFilter)this.DataContext).SelectItemPrincipal(obj);
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
                vm.SelectAllPrincipal(((CheckBox)sender).IsChecked);
            }
        }

        #endregion

        #region 排序

        private void dgSelectPrincipal_LayoutUpdated(object sender, EventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        private void dgSelectPrincipal_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MyDataGridSortInChinese.OrderData(sender, e, ((VmXlsFilter)this.DataContext).PrincipalInfoModel);
        }

        private void dgSelectPrincipal_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        private void dgSelectPrincipal_MouseMove(object sender, MouseEventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        #endregion

    }
}

