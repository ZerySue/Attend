/*************************************************************************
** 文件名:   DlgFestivalItemMng.cs
** 主要类:   DlgFestivalItemMng
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2013-12-12
** 修改人:   
** 日  期:
** 描  述:   DlgFestivalItemMng类,添加、修改节假日窗口
**
** 版  本:   1.0.0
** 备  注:  命名及代码编写遵守C#编码规范
**
*************************************************************************/
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
using IriskingAttend.ViewModel.AttendViewModel;
using Irisking.Web.DataModel;
using System.Windows.Data;

namespace IriskingAttend.Dialog
{
    public partial class DlgFestivalItemMng : ChildWindow
    {
        #region 私有变量声明：当前选中行信息、父窗口的回调函数

        //回调函数，父窗口传递
        private Action<bool?> _parentCallBack;

        //vm初始化
        VmFestivalItemMng _vmFesvalItemMng = new VmFestivalItemMng();

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="festivalInfoItem">欲进行操作的节假日信息</param>
        /// <param name="parentCallBack">子窗口关闭后调用的父窗口回调函数</param>
        public DlgFestivalItemMng(FestivalInfo festivalInfoItem, Action<bool?> parentCallBack)
        {            
            InitializeComponent();

            //vm数据绑定
            this.DataContext = _vmFesvalItemMng;

            //vm内包含的委托的初始化
            _vmFesvalItemMng.ChangeDlgResult = ModifyDlgResult;

            //子窗口关闭时的事件
            this.Closed += new EventHandler(DlgFestivalItemMng_Closed);

            //父窗口的回调函数
            this._parentCallBack = parentCallBack;

            _vmFesvalItemMng.Name = festivalInfoItem.name;
            _vmFesvalItemMng.BeginTime = festivalInfoItem.begin_time;
            _vmFesvalItemMng.EndTime = festivalInfoItem.end_time;
            _vmFesvalItemMng.Memo = festivalInfoItem.memo;

            if (festivalInfoItem.ShiftHolidayList != null && festivalInfoItem.ShiftHolidayList.Count() > 0)
            {
                foreach( DateTime item in festivalInfoItem.ShiftHolidayList)
                {
                    ShiftHoliday shiftDate = new ShiftHoliday();
                    shiftDate.ShiftDate = item;
                    _vmFesvalItemMng.ShiftDateList.Add(shiftDate);
                }
            }

            //节假日类型下拉框的数据源绑定
            this.dgShiftHoliday.ItemsSource = _vmFesvalItemMng.ShiftDateList;

            //当前节假日列表选中行绑定
            System.Windows.Data.Binding binding = new System.Windows.Data.Binding("SelectShiftHolidayItem") { Mode = BindingMode.TwoWay, };
            dgShiftHoliday.SetBinding(DataGrid.SelectedItemProperty, binding);

            //将父窗口中需要修改的节假日信息显示在此子窗口中，子窗口界面控件值初始化
            _vmFesvalItemMng.FesvalInfo = festivalInfoItem; 

        }

        #endregion

        #region 回调函数

        /// <summary>
        /// vm层委托的回调函数 
        /// </summary>
        /// <param name="dialogResult">是否需要关闭此子窗口</param>
        private void ModifyDlgResult(bool dialogResult)
        {
            this.DialogResult = dialogResult;
        }

        #endregion

        #region 事件响应

        /// <summary>
        /// 取消添加按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;           
        }

        /// <summary>
        /// 子窗口关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DlgFestivalItemMng_Closed(object sender, EventArgs e)
        {
            //子窗口关闭时，调用父窗口传递进来的回调函数
            if (_parentCallBack != null)
            {
                _parentCallBack(this.DialogResult);
            }
        }

        #endregion

        private void btnAddShiftHoliday_Click(object sender, RoutedEventArgs e)
        {
            if (dpShiftHoliday.SelectedDate == null )
            {
                return;
            }
            foreach (ShiftHoliday item in _vmFesvalItemMng.ShiftDateList)
            {
                if (item.ShiftDate == (DateTime)dpShiftHoliday.SelectedDate)
                {
                    MsgBoxWindow.MsgBox("此调休日期已在本节假日中存在，请重新输入！",
                                              MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);   
                    return;
                }                    
            }
            ShiftHoliday shiftDate = new ShiftHoliday();
            shiftDate.ShiftDate = (DateTime)dpShiftHoliday.SelectedDate;

            _vmFesvalItemMng.ShiftDateList.Add(shiftDate);           
        }

        private void hbtnDeleteShiftHoliday_Click(object sender, RoutedEventArgs e)
        {            
            _vmFesvalItemMng.ShiftDateList.Remove(_vmFesvalItemMng.SelectShiftHolidayItem);           
        }
    }
}
