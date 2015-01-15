/*************************************************************************
** 文件名:   SelectWorkType.cs
×× 主要类:   SelectWorkType
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   cty
** 日  期:   2013-10-30
** 修改人:   
** 日  期:   
** 描  述:   SelectWorkType类，五虎山PersonList界面查询条件中选择工种的页面
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
using Irisking.Web.DataModel;
using IriskingAttend.View;
using IriskingAttend.Common;

namespace IriskingAttend.NewWuHuShan
{
    public partial class SelectWorkType : ChildWindow
    {
        #region 变量声明

        /// <summary>
        /// 私有变量声明
        /// </summary>
        VmXlsFilter _vm = new VmXlsFilter();

        /// <summary>
        /// 回调函数
        /// </summary>
        private Action<BaseViewModelCollection<WorkTypeInfo>> _callback;

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="callback"></param>
        public SelectWorkType(List<WorkTypeInfo> workTypeSelect,Action<BaseViewModelCollection<WorkTypeInfo>> callback)
        {
            InitializeComponent();

            _vm.GetWorkType();

            this.DataContext = _vm;

            //全选绑定初始化
            _vm.MarkObj = this.Resources["MarkObject"] as MarkObject;

            //工种加载完成
            _vm.WorkTypeLoadCompleted += delegate
            {
                if (_vm.WorkTypeModel.Count > 0)
                {
                    //去工种务列表中“全部”一项
                    _vm.WorkTypeModel.RemoveAt(0);

                    //再次进入该页面时，记住上次选中的工种
                    foreach (WorkTypeInfo ar in workTypeSelect)
                    {
                        foreach (WorkTypeInfo wt in _vm.WorkTypeModel)
                        {
                            if (ar.work_type_id == wt.work_type_id)
                            {
                                wt.isSelected = true;
                            }
                        }
                    }

                    dgSelectWorkType.ItemsSource = _vm.WorkTypeModel;

                    //造一条假数据，用于运行vm层的控制全选按钮是否选中的代码
                    WorkTypeInfo worktypeAll = new WorkTypeInfo { work_type_id = 0, work_type_name = "全部" };
                    ((VmXlsFilter)this.DataContext).SelectItemWorkType(worktypeAll);
                }
            };
            dgSelectWorkType.ItemsSource = _vm.WorkTypeModel;
         
            _callback = callback;
        }

        #endregion

        #region 确定操作

        /// <summary>
        /// 确定按钮操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            _callback(_vm.WorkTypeModel);
            this.DialogResult = true;
        }

        #endregion

        #region 取消操作

        /// <summary>
        /// 取消按钮操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        #endregion

        #region 复选框选取操作
        /// <summary>
        /// 复选框选取操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgSelectWorkType_LoadingRow(object sender, DataGridRowEventArgs e)
        {

            dgSelectWorkType.MouseLeftButtonUp -= new MouseButtonEventHandler(Row_MouseLeftButtonUp);
            dgSelectWorkType.MouseLeftButtonUp += new MouseButtonEventHandler(Row_MouseLeftButtonUp);
        }

        private void Row_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (dgSelectWorkType.CurrentColumn.DisplayIndex == 0)
            {
                if (dgSelectWorkType.SelectedItem != null)
                {
                    var obj = dgSelectWorkType.SelectedItem as WorkTypeInfo;
                    ((VmXlsFilter)this.DataContext).SelectItemWorkType(obj);
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
                vm.SelectAllWorkType(((CheckBox)sender).IsChecked);
            }
        }

        #endregion

        #region 排序
        private void dgSelectWorkType_LayoutUpdated(object sender, EventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        private void dgSelectWorkType_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MyDataGridSortInChinese.OrderData(sender, e, ((VmXlsFilter)this.DataContext).WorkTypeModel);
        }

        private void dgSelectWorkType_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        private void dgSelectWorkType_MouseMove(object sender, MouseEventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }
        #endregion 
    }
}

