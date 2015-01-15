/*************************************************************************
** 文件名:   ChildWnd_OperateClassOrderSign.cs
×× 主要类:   ChildWnd_OperateClassOrderSign
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2014-9-26
** 修改人: 
** 修改内容：
** 日  期: 
** 描  述:   ChildWnd_OperateClassOrderSign类,增删改查签到班班次页面
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
using IriskingAttend.ViewModel.PeopleViewModel;
using Irisking.Web.DataModel;
using System.Windows.Controls.Primitives;
using System.Windows.Browser;
using IriskingAttend.BehaviorSelf;
using System.Text.RegularExpressions;
using IriskingAttend.Dialog;

namespace IriskingAttend.View.PeopleView
{
    /// <summary>
    /// 操作班次界面UI后台
    /// </summary>
    public partial class ChildWnd_OperateClassOrderSign : ChildWindow
    {
        #region 字段声明
        
        private Action<bool?> _callBack;

        public VmChildWndOperateClassOrderSign VmOperateClassOrderSign;
        #endregion

        #region 构造函数

        public ChildWnd_OperateClassOrderSign(UserClassOrderSignInfo cInfo, ChildWndOptionMode mode, Action<bool?> callBack)
        {
            InitializeComponent();

            //签到时间段起始日期
            cmbBeginDay.Items.Add("当日");
            cmbBeginDay.Items.Add("次日");
            cmbBeginDay.Items.Add("第三日");
            //签到时间段结束日期
            cmbEndDay.Items.Add("当日");
            cmbEndDay.Items.Add("次日");
            cmbEndDay.Items.Add("第三日");

            cmbLianBan.Items.Add("否");
            cmbLianBan.Items.Add("是");

            VmOperateClassOrderSign = new VmChildWndOperateClassOrderSign(cInfo, mode);
            this.DataContext = VmOperateClassOrderSign;
            if (cInfo != null)
            {
                VmOperateClassOrderSign.ClassOrderSignName = cInfo.class_order_name;
                VmOperateClassOrderSign.AttendSign = cInfo.attend_sign;
                VmOperateClassOrderSign.MinWorkTime = DateTime.Parse(cInfo.min_work_time_str);
                VmOperateClassOrderSign.WorkCnt = Double.Parse(cInfo.work_cnt_str);                
            }
            VmOperateClassOrderSign.CloseEvent += new Action<bool>(vm_CloseEvent);    

            _callBack = callBack;
        }

        #endregion

        #region 界面事件响应函数


        //窗口关闭事件
        void vm_CloseEvent(bool obj)
        {
            DialogResult = obj;
            if (_callBack != null)
            {
                _callBack(this.DialogResult);
            }
        }

        /// <summary>
        /// 记工工数控件输入时，只能输入数字键和“.”
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtWorkCntstandard_KeyDown(object sender, KeyEventArgs e)
        {
            if (!PublicMethods.IsKeyNumber(e.Key) && e.Key != Key.Tab && !PublicMethods.IsKeyDecimall(e))
            {
                e.Handled = true;
            }

            if (PublicMethods.IsKeyDecimall(e))
            {
                //限制只能输入整数或者小数
                Regex regDecimals = new Regex(@"^.*\..*$");
                if (regDecimals.IsMatch(((TextBox)sender).Text) || ((TextBox)sender).Text.TrimStart().Length == 0)
                {
                    e.Handled = true;
                }
            }
         
        }

        #endregion      

        private void AddSectionData_Click(object sender, RoutedEventArgs e)
        {
            if (tpStartTime.GetSelectedValue() == null || cmbBeginDay.SelectedIndex == -1 ||
                tpEndTime.GetSelectedValue() == null || cmbEndDay.SelectedIndex == -1)
            {
                MsgBoxWindow.MsgBox(
                              "起始时间和终止时间都不能为空！",
                              MsgBoxWindow.MsgIcon.Information,
                              MsgBoxWindow.MsgBtns.OK);
                return;
            }
            ClassOrderSignSection temp = new ClassOrderSignSection();
            temp.SectionBeginMin = VmOperateClassOrderSign.DateTimeToMinute(tpStartTime.GetSelectedValue(), cmbBeginDay.SelectedIndex);
            temp.SectionBeginMinStr = VmOperateClassOrderSign.MinutesToDate(temp.SectionBeginMin, true);
            temp.SectionEndMin =  VmOperateClassOrderSign.DateTimeToMinute(tpEndTime.GetSelectedValue(), cmbEndDay.SelectedIndex);
            temp.SectionEndMinStr = VmOperateClassOrderSign.MinutesToDate(temp.SectionEndMin, true);
            temp.InCalc = chkCriticalSection.IsChecked == true ? 1 : 0;
            if (temp.InCalc == 0)
            {
                temp.InCalcStr = "否";
            }
            VmOperateClassOrderSign.SectionDatas.Add(temp);
            VmOperateClassOrderSign.SectionDatas.OrderBy(a=>a.SectionBeginMin);
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var info = (ClassOrderSignSection)((HyperlinkButton)sender).DataContext;
            VmOperateClassOrderSign.SectionDatas.Remove(info); 
        }
       

    }
}

