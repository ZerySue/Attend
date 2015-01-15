/*************************************************************************
** 文件名:   LeaveTypeManage.cs
** 主要类:   LeaveTypeManage
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   cty
** 日  期:   2013-10-31
** 修改人:   
** 日  期:   
 *修改内容： 
** 描  述:   LeaveTypeManage，增加、修改考勤类型的页面
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
using IriskingAttend.BehaviorSelf;
using IriskingAttend.ViewModel.AttendViewModel;

namespace IriskingAttend.Dialog
{
    public partial class LeaveTypeManage : ChildWindow
    {
        Action<bool?> _callBack;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="leaveTypeInfo"></param>
        /// <param name="mode"></param>
        /// <param name="_callBack"></param>
        public LeaveTypeManage(LeaveType leaveTypeInfo, ChildWndOptionMode mode, Action<bool?> callBack)
        {        
            InitializeComponent();
            _callBack = callBack;
            ////是否记工
            //cmbIsSchedule.Items.Add("否");
            //cmbIsSchedule.Items.Add("是");

            ////考勤类别
            //cmbIsNormalAttend.Items.Add("不计入考勤");
            //cmbIsNormalAttend.Items.Add("计入考勤");
            //cmbIsNormalAttend.Items.Add("有条件计入考勤");

            VmLeaveTypeManagecs vm = new VmLeaveTypeManagecs(leaveTypeInfo, mode);
            vm.CloseEvent += new Action<bool>(vm_CloseEvent);
            this.DataContext = vm;       
        }

        /// <summary>
        /// 窗口关闭事件
        /// </summary>
        /// <param name="obj"></param>
        private void vm_CloseEvent(bool obj)
        {
            DialogResult = obj;
            if (_callBack != null)
            {
                _callBack(this.DialogResult);
            }
        }
    }
}

