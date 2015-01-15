
/*************************************************************************
** 文件名:   RebuildAttend.cs
×× 主要类:   RebuildAttend
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   lzc
** 日  期:   2013-4-28
** 修改人:   
** 日  期:
** 描  述:   RebuildAttend类,重构界面
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
using IriskingAttend.ViewModel;
using Irisking.Web.DataModel;
using System.ServiceModel.DomainServices.Client;
using System.ComponentModel;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using IriskingAttend.Dialog;
using IriskingAttend.Common;
using GalaSoft.MvvmLight.Command;
using MvvmLightCommand.SL4.TriggerActions;

namespace IriskingAttend.View.AttendView
{
    public partial class RebuildAttend : Page
    {
        #region 私有变量
        //部门vm类
        private VmDepartment _vmDepart = new VmDepartment();
        /// <summary>
        /// 重构vm类
        /// </summary>
        private RebuildRecog _vmRebuildRecog = new RebuildRecog();

        /// <summary>
        /// 是否全选
        /// </summary>
        public MarkObject AllSelect { get; private set; }

        #endregion

        #region 构造函数

        public RebuildAttend()
        {
            InitializeComponent();
            //初始化设置
            SetCheck();
            SetPageBind();
        }

        /// <summary>
        ///设置页面绑定
        /// </summary>
        private void SetPageBind()
        {
            ///设置进度条绑定
            Binding bindingValue = new Binding("BarBind.BarValue") 
            { Mode = BindingMode.TwoWay, Source = _vmRebuildRecog };
            Binding bindingMaximum = new Binding("BarBind.BarMaximun") 
            { Mode = BindingMode.TwoWay, Source = _vmRebuildRecog };
            Binding bindingVisibility = new Binding("BarBind.BarVisibility") 
            { Mode = BindingMode.TwoWay, Source = _vmRebuildRecog };

            //设置进度条绑定
            pBarRebuild.SetBinding(ProgressBar.MaximumProperty, bindingMaximum);
            pBarRebuild.SetBinding(ProgressBar.ValueProperty, bindingValue);
            pBarRebuild.SetBinding(ProgressBar.VisibilityProperty, bindingVisibility);

            //设置时间控件绑定
            Binding beginDate = new Binding("RebuitDateTime") 
            { Mode = BindingMode.TwoWay, Source = _vmRebuildRecog };
            datePickerDate.SetBinding(DatePicker.SelectedDateProperty, beginDate);
            timeBegin.SetBinding(TimePicker.ValueProperty, beginDate);


            ///设置button是否可用绑定
            Binding bindingBtnRebuitIsEnable = new Binding("BtnBind.RebuiltIsEnable") 
            { Mode = BindingMode.TwoWay, Source = _vmRebuildRecog };
            Binding bindingBtaStopIsEnable = new Binding("BtnBind.StoppbuiltIsEnable") 
            { Mode = BindingMode.TwoWay, Source = _vmRebuildRecog };

            //button绑定
            btnRebuild.SetBinding(Button.IsEnabledProperty, bindingBtnRebuitIsEnable);
            btnStopRebuild.SetBinding(Button.IsEnabledProperty, bindingBtaStopIsEnable);

            btnRebuild.Command = _vmRebuildRecog.RebuildRecogCommand;
            btnRebuild.CommandParameter = dgRebuildPerson;

            //部门绑定
            _vmDepart.GetDepartment();
            _vmDepart.DepartmentLoadCompleted += (o, e) =>
            {
                ///绑定部门信息
                cmbDepart.ItemsSource = _vmDepart.DepartmentModel;
                cmbDepart.DisplayMemberPath = "depart_name";
                if (_vmDepart.DepartmentModel.Count() > 0)
                {
                    cmbDepart.SelectedIndex = 0;
                }
                _vmDepart.GetUserPersonSimple();
            };
          
            ///绑定人员数据
            dgRebuildPerson.ItemsSource = _vmDepart.UserPersonSimpleForDepartModel;
            _vmDepart.UserPersonSimpleForDepartModelBase.CollectionChanged += (a, e) =>
            {
                if (_vmDepart.UserPersonSimpleForDepartModel.Count() > 0)
                {
                    _vmDepart.UserPersonSimpleForDepartModel.Last().is_select = true;
                    //btnRebuild.IsEnabled = true;
                    _vmRebuildRecog.BtnBind.RebuiltIsEnable = true;
                }
                AllSelect.Selected = true;
            };

            dgRebuildPerson.AddHandler(MouseLeftButtonUpEvent, new MouseButtonEventHandler(dgRebuildPerson_MouseLeftButtonUp), true);
        }

        /// <summary>
        /// 设置复选事件
        /// </summary>
        private void SetCheck()
        {
            AllSelect = Resources["MarkObject"] as MarkObject;
        }
        #endregion

        #region 响应函数

        /// <summary>
        /// 进入界面
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            AllSelect.Selected = true;
            if (App.Current.Resources.Contains("RebuildDateBegin"))
            {
                datePickerDate.Text = App.Current.Resources["RebuildDateBegin"].ToString();
            }
        }

        /// <summary>
        /// 页面离开
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (pBarRebuild.Visibility != System.Windows.Visibility.Collapsed)
            {
                _vmRebuildRecog.StopRebuildPersonRecog();
            }
            //基类函数

            if (!App.Current.Resources.Contains("RebuildDateBegin"))
            {
                if (datePickerDate.Text != null)
                {
                    App.Current.Resources.Add("RebuildDateBegin", datePickerDate.Text);
                }
            }
            else
            {
                try
                {
                    App.Current.Resources.Remove("RebuildDateBegin");

                    if (!App.Current.Resources.Contains("RebuildDateBegin"))
                    {
                        App.Current.Resources.Add("RebuildDateBegin", datePickerDate.Text);
                    }
                }
                catch (Exception ee)
                {
                    string err = ee.Message;
                }
            }

            base.OnNavigatedFrom(e);
        }
        /// <summary>
        /// 停止重构
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStopRebuild_Click(object sender, RoutedEventArgs e)
        {

            MsgBoxWindow.MsgBox( "是否取消重构！",
                    MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK,
                    (isOK) =>
                    {
                        if (isOK == MsgBoxWindow.MsgResult.OK)
                        {
                            pBarRebuild.Visibility = System.Windows.Visibility.Collapsed;
                            _vmRebuildRecog.BtnBind.RebuiltIsEnable = true;
                            _vmRebuildRecog.BtnBind.StoppbuiltIsEnable = false;
                            _vmRebuildRecog.StopRebuildPersonRecog();
                        }
                    });
        }

        /// <summary>
        /// 是否全选，全部选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkSelectAll_Click(object sender, RoutedEventArgs e)
        {
            bool tag = (sender as CheckBox).IsChecked ?? false;
            if (tag)
            {
                _vmRebuildRecog.BtnBind.RebuiltIsEnable = true;
            }
            else
            {
                _vmRebuildRecog.BtnBind.RebuiltIsEnable = false;
            }

            foreach (var ar in dgRebuildPerson.ItemsSource)
            {
                ((UserPersonSimple)ar).is_select = tag;
            }
        }

        /// <summary>
        ///  查询需要重构的人员
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQuery_Click(object sender, RoutedEventArgs e)
        {

            if (cmbDepart.SelectedIndex >= 0)
            {
                _vmDepart.GetUserPersonSimple(tbName.Text.ToString(), ((cmbDepart.SelectedItem) as depart).depart_id, tbWorkSN.Text.ToString());
            }

            dgRebuildPerson.ItemsSource = _vmDepart.UserPersonSimpleForDepartModel;
            //NavigationCacheMode = System.Windows.Navigation.NavigationCacheMode.Required;
        }

        /// <summary>
        /// 设置checkBox全选框状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox)
            {
                if ((sender as CheckBox).IsChecked ?? false)
                {
                    //防止事件先于绑定前执行
                    if (dgRebuildPerson.SelectedItem != null && !(dgRebuildPerson.SelectedItem as UserPersonSimple).is_select)
                    {
                        (dgRebuildPerson.SelectedItem as UserPersonSimple).is_select = true;
                    }

                    ///设置全选
                    if (_vmDepart.UserPersonSimpleForDepartModel.Where(a => a.is_select == false).Count() == 0)
                    {
                        AllSelect.Selected = true;
                    }
                    _vmRebuildRecog.BtnBind.RebuiltIsEnable = true;
                }
                else
                {
                    if (dgRebuildPerson.SelectedItem != null && (dgRebuildPerson.SelectedItem as UserPersonSimple).is_select)
                    {
                        (dgRebuildPerson.SelectedItem as UserPersonSimple).is_select = false;
                    }

                    ///重构按钮为非选状态
                    if (_vmDepart.UserPersonSimpleForDepartModel.Where(a => a.is_select == true).Count() == 0)
                    {
                        _vmRebuildRecog.BtnBind.RebuiltIsEnable = false;
                    }
                    AllSelect.Selected = false;

                }
            }
        }
       
        /// <summary>
        /// 为dgRebuildPerson的每一行增加MouseLeftButtonUp事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgRebuildPerson_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.MouseLeftButtonUp -= new MouseButtonEventHandler(Row_MouseLeftButtonUp);
            e.Row.MouseLeftButtonUp += new MouseButtonEventHandler(Row_MouseLeftButtonUp);
        }

        /// <summary>
        /// 绑定MouseLeftButtonUp事件响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Row_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //设置checkBox的选择状态和全选状态 以及重构按钮是否置灰
            if (dgRebuildPerson.CurrentColumn != null && dgRebuildPerson.CurrentColumn.DisplayIndex == 0 
                && dgRebuildPerson.SelectedItem != null)
            {
                
                (dgRebuildPerson.SelectedItem as UserPersonSimple).is_select = !(dgRebuildPerson.SelectedItem as UserPersonSimple).is_select;
                if ((dgRebuildPerson.SelectedItem as UserPersonSimple).is_select && 
                    _vmDepart.UserPersonSimpleForDepartModel.Where(a => a.is_select == false).Count() == 0)
                {
                    AllSelect.Selected = true;
                    _vmRebuildRecog.BtnBind.RebuiltIsEnable = true;
                }
                else
                {
                    if (_vmDepart.UserPersonSimpleForDepartModel.Where(a => a.is_select == true).Count() == 0)
                    {
                        _vmRebuildRecog.BtnBind.RebuiltIsEnable = false;
                    }
                    else
                    {
                        _vmRebuildRecog.BtnBind.RebuiltIsEnable = true;
                    }

                    AllSelect.Selected = false;
                }
            }
        }  

        #endregion

        #region 排序

        /// <summary>
        /// 排序箭头显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgRebuildPerson_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
            dgRebuildPerson.UpdateLayout();
        }

        /// <summary>
        /// 鼠标左键点击DataGrid头针对对应列进行排序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgRebuildPerson_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MyDataGridSortInChinese.OrderData(sender, e, _vmDepart.UserPersonSimpleForDepartModel);
        }

        /// <summary>
        /// 设置排序箭头显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgRebuildPerson_MouseMove(object sender, MouseEventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }
        /// <summary>
        /// 设置排序箭头显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgRebuildPerson_LayoutUpdated(object sender, EventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        #endregion

        private void cmbDepart_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string departName = ((cmbDepart.SelectedItem) as depart).depart_name;
            _vmRebuildRecog.DepartName = departName;
        }
    } 
}


namespace IriskingAttend.View
{ 
    #region  辅助类
    /// <summary>
    /// 绑定全选CheckBox 类
    /// </summary>
    public class MarkObject : BaseViewModel
    {
        //是否被选择
        private bool _selected;

        /// <summary>
        /// Selected被改变事件
        /// </summary>
        public event EventHandler SelectedChanged = null;
        /// <summary>
        /// 选择状态
        /// </summary>
        public bool Selected
        {
            get 
            { 
                return _selected;
            }

            set
            {
                if (_selected == value)
                {
                    return;
                }
                _selected = value;
                SelectedChanged(null, null);
                //软编码
                OnPropertyChanged(() => Selected);
            }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public MarkObject()
        {
            _selected = false;
            SelectedChanged += (o, e) =>
                {
                };
        }
    }

    #endregion
}