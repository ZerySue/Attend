/*************************************************************************
** 文件名:   AddAttendLeave.cs
×× 主要类:   AddAttendLeave
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   lzc
** 日  期:   2013-4-23
** 修改人:   
** 日  期:
** 描  述:   AddAttendLeave类,添加请假界面
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
    public partial class AddAttendLeaveDialog : ChildWindow
    {
        //部门数据层
        private VmDepartment _vmDepart = new VmDepartment();
        //请假数据层
        private VmLeaveType _vmLeaveType = new VmLeaveType();

        /// <summary>
        /// 修改构造
        /// </summary>
        /// <param name="leave"></param>
        public AddAttendLeaveDialog(UserAttendForLeave leave)
        {
            InitializeComponent();
            lbName.Content = leave.person_name;
            SetUiVisibility(true);
            _vmLeaveType.GetLeaveType(50);


            ///请假类型  按天请假或按小时请假
            if (leave.is_leave_all_day)
            {
                cmbType.SelectedIndex = 0;
                dateBegin.Text = leave.leave_start_time.Date.ToString("yyyy-MM-dd");
                dateEnd.Text = leave.leave_end_time.Date.ToString("yyyy-MM-dd");
            }
            else
            {
                dateBegin.Text = leave.leave_start_time.Date.ToString("yyyy-MM-dd");
                dateEnd.Text = leave.leave_end_time.Date.ToString("yyyy-MM-dd");
                timeBegin.Value = leave.leave_start_time;
                timeEnd.Value = leave.leave_end_time;
                cmbType.SelectedIndex = 1;
            }

            ///请假信息加载完成委托
            _vmLeaveType.LeaveTypeLoadCompleted += delegate
            {
                cmbLeaveType.ItemsSource = _vmLeaveType.LeaveTypeModel.Where(a => a.leave_type_id > 0);
                cmbLeaveType.DisplayMemberPath = "leave_type_name";
                if (_vmLeaveType.LeaveTypeModel.Count > 0)
                {
                    cmbLeaveType.SelectedItem = _vmLeaveType.LeaveTypeModel.Where(a => a.leave_type_name == leave.leave_type_name).First();
                }
            };

            //请假备注
            tbMemo.Text = leave.memo;
        }

        /// <summary>
        /// 设置界面显示效果  添加请假或请假信息更新界面
        /// </summary>
        /// <param name="tag"></param>
        private void SetUiVisibility(bool tag)
        {
            cmbType.Items.Add("按天请假");
            cmbType.Items.Add("按小时请假");
            cmbType.SelectedIndex = 0;
            timeBegin.Visibility = System.Windows.Visibility.Collapsed;
            timeEnd.Visibility = System.Windows.Visibility.Collapsed;

            if (tag)
            {
                ///查看或更新请假信息
                lbDepart.Visibility = System.Windows.Visibility.Collapsed;
                lbNameCmb.Visibility = System.Windows.Visibility.Collapsed;
                cmbDepart.Visibility = System.Windows.Visibility.Collapsed;
                cmbName.Visibility = System.Windows.Visibility.Collapsed;
                lbWorkSN.Visibility = System.Windows.Visibility.Collapsed;
                tbWorkSN.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                ///新添加请假信息
                spHand.Visibility = System.Windows.Visibility.Collapsed;
            }
        }
        /// <summary>
        /// 添加请假构造函数
        /// </summary>
        public AddAttendLeaveDialog()
        {
            try
            {
                InitializeComponent();

                ///改变选择部门事件委托
                cmbDepart.SelectionChanged += (oo, ee) =>
                    {
                        if (cmbDepart.SelectedItem != null)
                        {
                            _vmDepart.GetUserPersonSimple(((depart)cmbDepart.SelectedItem).depart_id);
                        }
                    };

                ///请假类型设置
                _vmLeaveType.GetLeaveType(50);
                _vmLeaveType.LeaveTypeLoadCompleted += (oo, ee) =>
                {
                    cmbLeaveType.ItemsSource = _vmLeaveType.LeaveTypeModel.Where(a => a.leave_type_id > 0);
                    cmbLeaveType.DisplayMemberPath = "leave_type_name";
                    if (_vmLeaveType.LeaveTypeModel.Count > 0)
                    {
                        cmbLeaveType.SelectedIndex = 0;
                        //适应神朔铁路需求，默认显示公出 by cty begin
                        int index=0;
                        foreach(var lt in _vmLeaveType.LeaveTypeModel )
                        {
                            if(lt.leave_type_name=="公出")
                            {
                                cmbLeaveType.SelectedIndex = index;
                            }
                            index++;
                        }
                        //by cty end
                    }
                    _vmDepart.GetDepartment();
                };

                ///通过工号选择人员信息加载完成事件
                _vmDepart.UserPersonSimpleForWorkSN.PropertyChanged += (oo,ee) =>
                    {
                        if (_vmDepart.UserPersonSimpleForWorkSN !=null && _vmDepart.UserPersonSimpleForWorkSN.depart_id > 0)
                        {
                            if (_vmDepart.DepartmentModel.Where(a => a.depart_id == _vmDepart.UserPersonSimpleForWorkSN.depart_id).Count() > 0)
                            {
                                cmbDepart.SelectedItem = _vmDepart.DepartmentModel.Where(a => a.depart_id == _vmDepart.UserPersonSimpleForWorkSN.depart_id).First();
                            }
                        }
                    };

                ///工号信息改变事件
                _vmDepart.WorkSNChanged += (oo, ee) =>
                {
                    if (_vmDepart.UserPersonSimpleForWorkSN != null && _vmDepart.UserPersonSimpleForWorkSN.depart_id > 0)
                    {
                        if (_vmDepart.DepartmentModel.Where(a => a.depart_id == _vmDepart.UserPersonSimpleForWorkSN.depart_id).Count() > 0)
                        {
                            if (cmbDepart.SelectedItem != _vmDepart.DepartmentModel.Where(a => a.depart_id == _vmDepart.UserPersonSimpleForWorkSN.depart_id).First())
                            {
                                cmbDepart.SelectedItem = _vmDepart.DepartmentModel.Where(a => a.depart_id == _vmDepart.UserPersonSimpleForWorkSN.depart_id).First();
                            }
                            else
                            {
                                cmbName.SelectedItem = _vmDepart.UserPersonSimpleForDepartModel.Where(a => a.person_id == _vmDepart.UserPersonSimpleForWorkSN.person_id).First();
                                //cmbName.SelectedItem = _vmDepart.UserPersonSimpleForDepartModel.Where(a => a.work_sn == tbWorkSN.ToString()).First();
                            }
                        }
                    }
                    else
                    {
                        MsgBoxWindow.MsgBox( "工号不存在，请输入正确的工号！",
                                              MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);        
                        tbWorkSN.Text = "";
                    }
                };

                ///部门信息加载完成事件委托
                _vmDepart.DepartmentLoadCompleted += (oo, ee) =>
                {
                    cmbDepart.ItemsSource = _vmDepart.DepartmentModel.Where(a=>a.depart_id>0);
 
                    cmbDepart.DisplayMemberPath = "depart_name";
                    if (_vmDepart.DepartmentModel.Where(a => a.depart_id > 0).Count() > 0)
                    {
                        cmbDepart.SelectedIndex = 0;
                    }
                   
                };

                SetUiVisibility(false);

                ///人员信息加载完成事件
                _vmDepart.UserPersonSimpleLoadForDepartCompleted += (oo, ee) =>
                {
                    if (_vmDepart.UserPersonSimpleForDepartModel.Count > 0)
                    {
                        cmbName.SelectedIndex = 0;
                    }

                    if (_vmDepart.UserPersonSimpleForWorkSN != null && _vmDepart.UserPersonSimpleForWorkSN.person_id > 0)
                    {
                        if (_vmDepart.UserPersonSimpleForDepartModel.Where(a => a.person_id == _vmDepart.UserPersonSimpleForWorkSN.person_id).Count() > 0)
                        {
                            cmbName.SelectedItem = _vmDepart.UserPersonSimpleForDepartModel.Where(a => a.person_id == _vmDepart.UserPersonSimpleForWorkSN.person_id).First();
                        }
                    }

                };
                cmbName.ItemsSource = _vmDepart.UserPersonSimpleForDepartModel;
                cmbName.DisplayMemberPath = "person_name";
            }
            catch (Exception err)
            {
                ErrorWindow errWin = new ErrorWindow(err);
                errWin.Show();
            }
        }

        /// <summary>
        /// 取消，关闭对话框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
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

        /// <summary>
        /// 通过工号自动匹配部门\姓名
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbWorkSN_LostFocus(object sender, RoutedEventArgs e)
        {
            if (tbWorkSN.Text != "")
            {
                _vmDepart.GetUserPersonSimple(tbWorkSN.Text.ToString());
            }
        }

        /// <summary>
        /// 填写姓名后自动匹配工号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cmbName.SelectedItem != null)
                {
                    tbWorkSN.Text = (cmbName.SelectedItem as UserPersonSimple).work_sn ?? "";
                }
                else
                {
                    tbWorkSN.Text = "";
                }
            }
            catch (Exception ee)
            {
                string str = ee.Message;
            }
        }

    }
}

