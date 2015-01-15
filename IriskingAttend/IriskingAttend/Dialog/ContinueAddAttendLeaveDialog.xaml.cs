/*************************************************************************
** 文件名:   ContinueAddAttendLeaveDialog.cs
×× 主要类:   ContinueAddAttendLeaveDialog
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   cty
** 日  期:   2014-3-30
** 修改人:   
** 日  期:
** 描  述:   ContinueAddAttendLeaveDialog类,批量添加请假界面
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

namespace IriskingAttend.Dialog
{
    public partial class ContinueAddAttendLeaveDialog : ChildWindow
    {
        //部门数据层
        private VmDepartment _vmDepart = new VmDepartment();
        //请假数据层
        private VmLeaveType _vmLeaveType = new VmLeaveType();

        public ContinueAddAttendLeaveDialog()
        {
            InitializeComponent();
            ///请假类型设置
            _vmLeaveType.GetLeaveType(50);
            _vmLeaveType.LeaveTypeLoadCompleted += (oo, ee) =>
            {
                cmbLeaveType.ItemsSource = _vmLeaveType.LeaveTypeModel.Where(a => a.leave_type_id > 0);
                cmbLeaveType.DisplayMemberPath = "leave_type_name";
                if (_vmLeaveType.LeaveTypeModel.Count > 0)
                {
                    cmbLeaveType.SelectedIndex = 0;
                    int index = 0;
                    foreach (var lt in _vmLeaveType.LeaveTypeModel)
                    {
                        if (lt.leave_type_name == "公出")
                        {
                            cmbLeaveType.SelectedIndex = index;
                        }
                        index++;
                    }
                }
            };

            SetUiVisibility();
        }
        /// <summary>
        /// 设置界面显示效果  添加请假或请假信息更新界面
        /// </summary>
        /// <param name="tag"></param>
        private void SetUiVisibility()
        {
            cmbType.Items.Add("按天请假");
            cmbType.Items.Add("按小时请假");
            cmbType.SelectedIndex = 0;
            timeBegin.Visibility = System.Windows.Visibility.Collapsed;
            timeEnd.Visibility = System.Windows.Visibility.Collapsed;

            //spHand.Visibility = System.Windows.Visibility.Collapsed;
            
        }

        /// <summary>
        /// 选择时间 按天请假或按小时请假  其时间控件显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //comboBox1.SelectedIndex

            if ((sender as ComboBox).SelectedIndex == 0)
            {
                if (timeBegin != null && timeBegin.Visibility == System.Windows.Visibility.Visible)
                {
                    timeBegin.Visibility = System.Windows.Visibility.Collapsed;
                    timeEnd.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
            else
            {
                if (timeBegin != null && timeBegin.Visibility == System.Windows.Visibility.Collapsed)
                {
                    timeBegin.Visibility = System.Windows.Visibility.Visible;
                    timeEnd.Visibility = System.Windows.Visibility.Visible;
                }
            }
        }
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}

