/*************************************************************************
** 文件名:   OverInWellDialog.cs
×× 主要类:   OverInWellDialog
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   lzc
** 日  期:   2013-9-1
** 修改人:   
** 日  期:
** 描  述:   OverInWellDialog类,井下超时界面
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
using IriskingAttend.Common;
using IriskingAttend.ViewModel.SafeManager;
using System.Windows.Controls.Primitives;
using Irisking.Web.DataModel;
using IriskingAttend.ViewModel;

namespace IriskingAttend.Dialog
{
    public partial class OverInWellDialog : ChildWindow
    {
        public OverInWellDialog()
        {
            InitializeComponent();
            dgOverInMinePerson.LoadingRow += (a, e) =>
            {
                int index = e.Row.GetIndex();
                var cell = dgOverInMinePerson.Columns[1].GetCellContent(e.Row) as TextBlock;
                cell.Text = (index + 1).ToString();
            };
            dgOverInMinePerson.AddHandler(MouseLeftButtonUpEvent, 
                new MouseButtonEventHandler(dgOverInMinePerson_MouseLeftButtonUp), true);

            //窗口关闭事件
            this.Closed += new EventHandler(MsgBoxWindow_Closed);
            
        }

        /// <summary>
        /// 窗口关闭事件：回调事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MsgBoxWindow_Closed(object sender, EventArgs e)
        {
            ((VmInWellPerson)this.DataContext).CancelBatchRecog();
        }

        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        #region 列表排序初始化及控件事件响应

        /// <summary>
        /// 鼠标移动事件，显示排序的上下箭头
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgOverInMinePerson_MouseMove(object sender, MouseEventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        /// <summary>
        /// 外观刷新事件，显示排序的上下箭头
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgOverInMinePerson_LayoutUpdated(object sender, EventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        /// <summary>
        ///左键点击事件发生时进行排序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgOverInMinePerson_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MyDataGridSortInChinese.OrderData(sender, e, ((VmInWellPerson)this.DataContext).InWellPersonOverModel);
        }

        private void dgOverInMinePerson_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        #endregion 

        /// <summary>
        /// 注册鼠标事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgOverInMinePerson_LoadingRow(object sender, DataGridRowEventArgs e)
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
            if (dgOverInMinePerson.CurrentColumn == null || dgOverInMinePerson.CurrentColumn.DisplayIndex != 0)
            {
                return;
            }
            //且当前选中行的不为空
            if (dgOverInMinePerson.SelectedItem == null)
            {
                return;
            }

            //更改当前选中行的选中状态
            UserInWellPerson selectPersonInfo = this.dgOverInMinePerson.SelectedItem as UserInWellPerson;
            ((VmInWellPerson)this.DataContext).OverInWellMouseLeftButtonUpArgs(selectPersonInfo);
        }
        
    }
}

