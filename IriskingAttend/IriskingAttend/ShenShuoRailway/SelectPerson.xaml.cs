/*************************************************************************
** 文件名:   SelectPerson.cs
×× 主要类:   SelectPerson
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2013-12-16
** 修改人:   
** 日  期:   
** 描  述:   SelectPerson类，选择人员
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
using IriskingAttend.Common;
using IriskingAttend.View;

namespace IriskingAttend.ShenShuoRailway
{
    public partial class SelectPerson : ChildWindow
    {
        #region 变量声明

        /// <summary>
        /// 回调函数
        /// </summary>
        private Action<BaseViewModelCollection<UserPersonInfo>> _callback;

        /// <summary>
        /// 全选按钮的绑定类
        /// </summary>
        private MarkObject MarkObj
        {
            get;
            set;
        }

        #endregion

        public SelectPerson(BaseViewModelCollection<UserPersonInfo> selectPerson, Action<BaseViewModelCollection<UserPersonInfo>> callback)
        {
            InitializeComponent();
            
            dgPersonInfo.ItemsSource = selectPerson;

            _callback = callback;

            MarkObj = new MarkObject();
            MarkObj = this.Resources["MarkObject"] as MarkObject;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            _callback((BaseViewModelCollection<UserPersonInfo>)dgPersonInfo.ItemsSource);
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
        private void dgPersonInfo_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            dgPersonInfo.MouseLeftButtonUp -= new MouseButtonEventHandler(Row_MouseLeftButtonUp);
            dgPersonInfo.MouseLeftButtonUp += new MouseButtonEventHandler(Row_MouseLeftButtonUp);
        }

        private void Row_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (dgPersonInfo.CurrentColumn.DisplayIndex == 0)
            {
                if (dgPersonInfo.SelectedItem != null)
                {
                    var obj = dgPersonInfo.SelectedItem as UserPersonInfo;
                    obj.isSelected = !obj.isSelected;
                    this.MarkObj.Selected = CheckIsAllSelected();
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
             SelectAll(((CheckBox)sender).IsChecked);           
        }

        #region 人员的选中与全选操作

        /// <summary>
        /// 部门全选操作
        /// </summary>
        /// <param name="isChecked"></param>
        public void SelectAll(bool? isChecked)
        {
            if (!isChecked.HasValue)
            {
                return;
            }
            if (isChecked.Value)
            {
                foreach (var item in (BaseViewModelCollection<UserPersonInfo>)dgPersonInfo.ItemsSource)
                {
                    item.isSelected = true;
                }
            }
            else
            {
                foreach (var item in (BaseViewModelCollection<UserPersonInfo>)dgPersonInfo.ItemsSource)
                {
                    item.isSelected = false;
                }
            }
        }

        /// <summary>
        /// 检查Item是否全部被选中
        /// </summary>
        private bool CheckIsAllSelected()
        {
            if (((BaseViewModelCollection<UserPersonInfo>)dgPersonInfo.ItemsSource).Count == 0)
            {
                return false;
            }
            foreach (var item in (BaseViewModelCollection<UserPersonInfo>)dgPersonInfo.ItemsSource)
            {
                if (!item.isSelected)
                {
                    return false;
                }
            }
            return true;

        }

        #endregion

        #endregion

        #region 排序
        private void dgPersonInfo_LayoutUpdated(object sender, EventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        private void dgPersonInfo_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MyDataGridSortInChinese.OrderData(sender, e, (BaseViewModelCollection<UserPersonInfo>)dgPersonInfo.ItemsSource);
        }

        private void dgPersonInfo_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        private void dgPersonInfo_MouseMove(object sender, MouseEventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }
        #endregion 
    }
}

