/*************************************************************************
** 文件名:   ChildWnd_OperateClassOrder.cs
×× 主要类:   ChildWnd_OperateClassOrder
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   cty
** 日  期:   2013-6-14
** 修改人:  lzc 
** 修改内容： 增加班次图示显示内容
** 日  期: 2013-8-7
** 描  述:   ChildWnd_OperateClassOrder类,增删改查班次页面
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
    public partial class ChildWnd_OperateClassOrder : ChildWindow
    {
        #region 字段声明
        
        Action<bool?> callBack;
        #endregion

        #region 构造函数

        public ChildWnd_OperateClassOrder(UserClassOrderInfo cInfo, ChildWndOptionMode mode, Action<bool?> _callBack,int preClassTypeId=0)
        {
            InitializeComponent();
           
            //考勤归属日by cty
            //
            Combobox_attend_day.Items.Add("【上班考勤起始时间】当天");
            Combobox_attend_day.Items.Add("【上班考勤起始时间】次日" );
            Combobox_attend_day.Items.Add("【上班考勤起始时间】第三日"); 
            //上班考勤开始时间
            Combobox_In_well_start_day.Items.Add("当日");
            //上班考勤截止时间
            Combobox_In_well_end_day.Items.Add("当日");
            Combobox_In_well_end_day.Items.Add("次日");
            Combobox_In_well_end_day.Items.Add("第三日");
            //下班考勤开始日期
            Combobox_Out_well_start_day.Items.Add("当日");
            Combobox_Out_well_start_day.Items.Add("次日");
            Combobox_Out_well_start_day.Items.Add("第三日");
            //下班考勤截止日期
            Combobox_Out_well_end_day.Items.Add("当日");
            Combobox_Out_well_end_day.Items.Add("次日");
            Combobox_Out_well_end_day.Items.Add("第三日");
            //最晚下班时间
            Combobox_Attend_work_time.Items.Add("当日");
            Combobox_Attend_work_time.Items.Add("次日");
            Combobox_Attend_work_time.Items.Add("第三日");
            //工数计算方式
            Combobox_Workcnt_method.Items.Add("标准");
            Combobox_Workcnt_method.Items.Add("分时间段记工");

            VmChildWndOperateClassOrder vm = new VmChildWndOperateClassOrder(cInfo, mode, preClassTypeId);
            vm.LoadCompletedEvent+=new EventHandler(vm_LoadCompletedEvent);
            vm.CloseEvent+=new Action<bool>(vm_CloseEvent);
            vm.LayoutDGClassOrder += ( sender) =>
                {
                    DG_ClassOrder.SelectedItem = sender;
                    DG_ClassOrder.ScrollIntoView(DG_ClassOrder.SelectedItem, null);
                };

            callBack = _callBack;
        }

        #endregion

        #region 界面事件响应函数


        //窗口关闭事件
        void vm_CloseEvent(bool obj)
        {
            DialogResult = obj;
            if (callBack != null)
            {
                callBack(this.DialogResult);
            }
        }

        //vm加载完毕事件
        void vm_LoadCompletedEvent(object sender, EventArgs e)
        {
            this.DataContext = sender;
        }


        //删除记工时间段和记工工数的事件 by cty
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext != null)
            {
                var obj = ((HyperlinkButton)sender).DataContext as WorkCntData;
                ((VmChildWndOperateClassOrder)this.DataContext).DelelteWorkCntData(obj);
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

        private void Combobox_Out_well_end_day_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //
        }
        private void Combobox_Out_well_Start_day_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //
        }
        private void Combobox_In_well_end_day_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //
            //tpOffEndTime.ValueChanged +=new RoutedPropertyChangedEventHandler<DateTime?>(tpOffEndTime_ValueChanged);
        }

        private void Combobox_In_well_start_day_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //
        }

      

       

    }
}

