/*************************************************************************
** 文件名:   PageUnCompletedLunch.cs
×× 主要类:   PageUnCompletedLunch
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   wz
** 日  期:   2013-6-14
** 修改人:   
** 日  期:
** 描  述:   PageUnCompletedLunch类,增删改班制页面
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
using System.Windows.Navigation;
using IriskingAttend.Common;
using IriskingAttend.ViewModelMine.PeopleViewModel;
using Irisking.Web.DataModel;
using IriskingAttend.ExportExcel;
using IriskingAttend.View;
using IriskingAttend.Web.ZhouYuanShan;
using IriskingAttend.Dialog;
using System.IO.IsolatedStorage;
using IriskingAttend.ApplicationType;
using IriskingAttend.ViewModel;

namespace IriskingAttend.ZhouYuanShan.LunchSubsidy
{
    public partial class PageUnCompletedLunch : Page
    {
        public PageUnCompletedLunch()
        {
            InitializeComponent();

            //加载vm
            VmUnCompletedLunch vm = new VmUnCompletedLunch();
            vm.LoadCompletedEvent += new EventHandler((sender, e) =>
            {
                this.DataContext = sender;
            });

            //全选绑定初始化
            vm.MarkObjCompleted = this.Resources["MarkObjectCompleted"] as MarkObject;
            vm.MarkObjUnCompleted = this.Resources["MarkObjectUnCompleted"] as MarkObject;

            //添加序号 不能采用绑定的方式，绑定的话排序后序号会跟着变化
            this.dataGridUnCompletedLunch.LoadingRow += (a, e) =>
            {
                int index = e.Row.GetIndex();
                var cell = dataGridUnCompletedLunch.Columns[1].GetCellContent(e.Row) as TextBlock;
                cell.Text = (index + 1).ToString();
            };
            this.dataGridCompletedLunch.LoadingRow += (a, e) =>
            {
                int index = e.Row.GetIndex();
                var cell = dataGridCompletedLunch.Columns[1].GetCellContent(e.Row) as TextBlock;
                cell.Text = (index + 1).ToString();
            };

            //排序箭头显示
            dataGridUnCompletedLunch.AddHandler(MouseLeftButtonUpEvent, new MouseButtonEventHandler(dataGrid_MouseLeftButtonUp), true);
            dataGridCompletedLunch.AddHandler(MouseLeftButtonUpEvent, new MouseButtonEventHandler(dataGrid_MouseLeftButtonUp), true);

            //added by gqy at 2014-02-13 周源山需求
            if (!VmLogin.DictPrivilege.ContainsKey(AbstractApp.PrivilegeENUM.LunchManageQuery) || !VmLogin.DictPrivilege[AbstractApp.PrivilegeENUM.LunchManageQuery])
            {
                btnQuery.IsEnabled = false;
            }
        }

        // 当用户导航到此页面时执行。
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            IsolatedStorageSettings appSettings = IsolatedStorageSettings.ApplicationSettings;
            if (!appSettings.Contains("PageUnCompletedLunch_BeginTime"))
            {
                appSettings.Add("PageUnCompletedLunch_BeginTime", null);
            }
            if (!appSettings.Contains("PageUnCompletedLunch_EndTime"))
            {
                appSettings.Add("PageUnCompletedLunch_EndTime", null);
            }

            appSettings["PageUnCompletedLunch_BeginTime"] = this.dateBegin.SelectedDate;
            appSettings["PageUnCompletedLunch_EndTime"] = this.dateEnd.SelectedDate;
            base.OnNavigatingFrom(e);
        }


        #region 排序

        private void dataGrid_LayoutUpdated(object sender, EventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
            
        }

        private void dataGridUnCompletedLunch_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MyDataGridSortInChinese.OrderData(sender, e, ((VmUnCompletedLunch)this.DataContext).ReportRecordInfosOnDepart);
        }

        private void dataGridCompletedLunch_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MyDataGridSortInChinese.OrderData(sender, e, ((VmUnCompletedLunch)this.DataContext).CompletedReportInfosOnDepart);
        }

        private void dataGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }


        private void dataGrid_MouseMove(object sender, MouseEventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        #endregion

        #region 界面按钮响应

        //选择未完成班中餐item
        private void dataGridUnCompletedLunch_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.MouseLeftButtonUp -= new MouseButtonEventHandler(UnCompletedLunchRow_MouseLeftButtonUp);
            e.Row.MouseLeftButtonUp += new MouseButtonEventHandler(UnCompletedLunchRow_MouseLeftButtonUp);
        }

        void UnCompletedLunchRow_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            
            if (dataGridUnCompletedLunch.CurrentColumn.DisplayIndex == 0)
            {
                if (dataGridUnCompletedLunch.SelectedItem != null)
                {
                    var obj = dataGridUnCompletedLunch.SelectedItem as ReportRecordInfoOnDepart_ZhouYuanShan;
                    ((VmUnCompletedLunch)this.DataContext).SelectItemsUnCompleted(obj);
                }
            }
        }

        //选择已完成班中餐item
        private void dataGridCompletedLunch_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.MouseLeftButtonUp -= new MouseButtonEventHandler(CompletedLunchRow_MouseLeftButtonUp);
            e.Row.MouseLeftButtonUp += new MouseButtonEventHandler(CompletedLunchRow_MouseLeftButtonUp);
        }

        void CompletedLunchRow_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (dataGridCompletedLunch.CurrentColumn.DisplayIndex == 0)
            {
                if (dataGridCompletedLunch.SelectedItem != null)
                {
                    var obj = dataGridCompletedLunch.SelectedItem as ReportRecordInfoOnDepart_ZhouYuanShan;
                    ((VmUnCompletedLunch)this.DataContext).SelectItemsCompleted(obj);
                }
            }
        }

        //编辑未完成班中餐
        private void btnModify_Click(object sender, RoutedEventArgs e)
        {
            ((VmUnCompletedLunch)this.DataContext).ModifyUnCompletedReportRecord();
        }

        //从未完成班中餐生成已完成班中餐
        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            ((VmUnCompletedLunch)this.DataContext).ReportRecord();
        }

        //撤消已完成班中餐
        private void btnUndo_Click(object sender, RoutedEventArgs e)
        {
            ((VmUnCompletedLunch)this.DataContext).UnDoReportRecord();
        }

        //查看已完成班中餐
        private void btnShow_Click(object sender, RoutedEventArgs e)
        {
            ((VmUnCompletedLunch)this.DataContext).CheckCompletedReportRecord();
        }

        //全选未完成班中餐item
        private void chkSelectAllUnCompleted_Click(object sender, RoutedEventArgs e)
        {
            ((VmUnCompletedLunch)this.DataContext).SelectAllUnCompleted(((CheckBox)sender).IsChecked);
        }

        //全选已完成班中餐item
        private void chkSelectAllCompleted_Click(object sender, RoutedEventArgs e)
        {
            ((VmUnCompletedLunch)this.DataContext).SelectAllCompleted(((CheckBox)sender).IsChecked);
        }

        #endregion

        #region 日期控件设置

        /// <summary>
        /// 设置DataPicker 日期的选择范围
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dateBegin_CalendarClosed(object sender, RoutedEventArgs e)
        {
            if (null != dateBegin.SelectedDate && null != dateEnd.SelectedDate)
            {
                if (dateBegin.SelectedDate.Value > dateEnd.SelectedDate.Value)
                {
                    MsgBoxWindow.MsgBox("选择结束时间不能早于开始时间！",
                        MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);
                    dateBegin.SelectedDate = null;
                }
            }
        }

        /// <summary>
        /// 校验日期格式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dateBegin_DateValidationError(object sender, DatePickerDateValidationErrorEventArgs e)
        {
            MsgBoxWindow.MsgBox("日期格式不对！", MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);
        }

        /// <summary>
        /// 设置时间选择范围判定，起始时间应需要截止时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dateEnd_CalendarClosed(object sender, RoutedEventArgs e)
        {
            if (null != dateBegin.SelectedDate && null != dateEnd.SelectedDate)
            {
                if (dateBegin.SelectedDate.Value > dateEnd.SelectedDate.Value)
                {
                    MsgBoxWindow.MsgBox("选择结束时间不能早于开始时间！",
                        MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);
                    dateEnd.SelectedDate = null;
                }
            }
        }

        #endregion

       
    }

    #region 辅助类

    /// <summary>
    /// 绑定类
    /// </summary>
    public class VisibleObject : BaseViewModel
    {
        //是否可见
        private Visibility _editVisible;
        
        /// <summary>
        /// 选择状态
        /// </summary>
        public Visibility EditVisible
        {
            get
            {
                return _editVisible;
            }

            set
            {
                if (_editVisible == value)
                {
                    return;
                }
                _editVisible = value;                
                //软编码
                OnPropertyChanged(() => EditVisible);
            }
        }
        //是否可见
        private Visibility _createVisible;

        /// <summary>
        /// 选择状态
        /// </summary>
        public Visibility CreateVisible
        {
            get
            {
                return _createVisible;
            }

            set
            {
                if (_createVisible == value)
                {
                    return;
                }
                _createVisible = value;
                //软编码
                OnPropertyChanged(() => CreateVisible);
            }
        }

        //是否可见
        private Visibility _showVisible;

        /// <summary>
        /// 选择状态
        /// </summary>
        public Visibility ShowVisible
        {
            get
            {
                return _showVisible;
            }

            set
            {
                if (_showVisible == value)
                {
                    return;
                }
                _showVisible = value;
                //软编码
                OnPropertyChanged(() => ShowVisible);
            }
        }

        //是否可见
        private Visibility _undoVisible;

        /// <summary>
        /// 选择状态
        /// </summary>
        public Visibility UndoVisible
        {
            get
            {
                return _undoVisible;
            }

            set
            {
                if (_undoVisible == value)
                {
                    return;
                }
                _undoVisible = value;
                //软编码
                OnPropertyChanged(() => UndoVisible);
            }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public VisibleObject()
        {
            EditVisible = Visibility.Visible;
            if (!VmLogin.DictPrivilege.ContainsKey(AbstractApp.PrivilegeENUM.LunchManageEdit) || !VmLogin.DictPrivilege[AbstractApp.PrivilegeENUM.LunchManageEdit])
            {
                EditVisible = Visibility.Collapsed;
            }

            CreateVisible = Visibility.Visible;
            if (!VmLogin.DictPrivilege.ContainsKey(AbstractApp.PrivilegeENUM.LunchManageCreate) || !VmLogin.DictPrivilege[AbstractApp.PrivilegeENUM.LunchManageCreate])
            {
                CreateVisible = Visibility.Collapsed;
            }

            ShowVisible = Visibility.Visible;
            if (!VmLogin.DictPrivilege.ContainsKey(AbstractApp.PrivilegeENUM.LunchManageShow) || !VmLogin.DictPrivilege[AbstractApp.PrivilegeENUM.LunchManageShow])
            {
                ShowVisible = Visibility.Collapsed;
            }

            UndoVisible = Visibility.Visible;
            if (!VmLogin.DictPrivilege.ContainsKey(AbstractApp.PrivilegeENUM.LunchManageUndo) || !VmLogin.DictPrivilege[AbstractApp.PrivilegeENUM.LunchManageUndo])
            {
                UndoVisible = Visibility.Collapsed;
            }
        }
    }
    #endregion
}
