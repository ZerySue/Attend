/*************************************************************************
** 文件名:   AttendRecord.cs
×× 主要类:   AttendRecord
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   lzc
** 日  期:   2013-4-8
** 修改人:   cty
** 日  期:   2013-10-30
 *修改内容：将
** 描  述:   AttendRecord类，考勤查询结果
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
using System.Windows.Data;
using System.Windows.Controls.Primitives;
using IriskingAttend.Common;
using Irisking.Web.DataModel;
using System.IO.IsolatedStorage;
using IriskingAttend.ExportExcel;
using IriskingAttend.Dialog;

namespace IriskingAttend.View
{
    public partial class AttendRecord : Page
    {
        #region 私有变量

        /// <summary>
        /// 考勤VM数据
        /// </summary>
        private VmAttend _vmAttend = new VmAttend();
        /// <summary>
        /// 部门人员VM数据
        /// </summary>
        private VmDepartment _vmDepart = new VmDepartment();       

        //本地独立存储，用来传参
        private IsolatedStorageSettings _querySetting = IsolatedStorageSettings.ApplicationSettings;

        #endregion

        #region  构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public AttendRecord()
        {
            InitializeComponent();

            SetInit();
        }

        /// <summary>
        /// 设置初始化界面
        /// </summary>
        private void SetInit()
        {
            try
            {
                //部门绑定
                _vmDepart.GetDepartment();
                _vmDepart.DepartmentLoadCompleted += (o, e) =>
                {
                    combDepart.ItemsSource = _vmDepart.DepartmentModel;
                    combDepart.DisplayMemberPath = "depart_name";

                    if (combDepart.Items.Count > 0)
                    {
                        combDepart.SelectedIndex = 0;
                    }                    
                };

                //考勤记录绑定
                dgAttendRecAll.DataContext = _vmAttend;
                dgAttendRecAll.ItemsSource = _vmAttend.AttendRecModel;
                dgAttendRecAll.MouseLeftButtonDown += new MouseButtonEventHandler(dgAttendRecAll_MouseLeftButtonDown);
                dgAttendRecAll.AddHandler(MouseLeftButtonUpEvent, new MouseButtonEventHandler(dgAttendRecAll_MouseLeftButtonUp), true);

                ///绑定选择考勤记录
                System.Windows.Data.Binding binding = new System.Windows.Data.Binding("SelectAttendRec") { Mode = BindingMode.TwoWay, };
                dgAttendRecAll.SetBinding(DataGrid.SelectedItemProperty, binding);
                //设置序号
                dgAttendRecAll.LoadingRow += (a, e) =>
                {
                    int index = e.Row.GetIndex();
                    var cell = dgAttendRecAll.Columns[0].GetCellContent(e.Row) as TextBlock;
                    cell.Text = (index + 1).ToString();
                };
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }

        }

        #endregion

        #region 响应事件函数

        /// <summary>
        /// 进入界面
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (App.Current.Resources.Contains("AttendRecordDateBegin"))
            {
                dateBegin.Text = App.Current.Resources["AttendRecordDateBegin"].ToString();
            }

            if (App.Current.Resources.Contains("AttendRecordDateEnd"))
            {
                dateEnd.Text = App.Current.Resources["AttendRecordDateEnd"].ToString();
            }
        }

        /// <summary>
        /// 页面离开
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (!App.Current.Resources.Contains("AttendRecordDateBegin"))
            {
                if (dateBegin.Text != null)
                {
                    App.Current.Resources.Add("AttendRecordDateBegin", dateBegin.Text);
                }
            }
            else
            {
                try
                {
                    App.Current.Resources.Remove("AttendRecordDateBegin");

                    if (!App.Current.Resources.Contains("AttendRecordDateBegin"))
                    {
                        App.Current.Resources.Add("AttendRecordDateBegin", dateBegin.Text);
                    }
                }
                catch (Exception ee)
                {
                    string err = ee.Message;
                }
            }

            if (!App.Current.Resources.Contains("AttendRecordDateEnd"))
            {
                if (dateEnd.Text != null)
                {
                    App.Current.Resources.Add("AttendRecordDateEnd", dateEnd.Text);
                }
            }
            else
            {
                App.Current.Resources.Remove("AttendRecordDateEnd");
                App.Current.Resources.Add("AttendRecordDateEnd", dateEnd.Text);
            }
        }


        /// <summary>
        /// 显示详细信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnShowDetails_Click(object sender, RoutedEventArgs e)
        {
            _vmAttend.ShowAttendRecDetail();
        }

        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportExl_Click(object sender, RoutedEventArgs e)
        {
            string space = "                                                                 ";
            ExpExcel.ExportExcelFromDataGrid(dgAttendRecAll, 1, 8, (space + "考勤查询" + space), "考勤查询");
        }

        #endregion

        //#region 排序

        //private void dgAttendRecAll_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        //{
        //    MyDataGridSortInChinese.SetColumnSortState();
        //    dgAttendRecAll.UpdateLayout();
        //}

        ///// <summary>
        ///// 鼠标左键点击DataGrid头针对对应列进行排序
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //public void dgAttendRecAll_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    MyDataGridSortInChinese.OrderData(sender, e, _vmAttend.AttendRecModel);
        //}

        ///// <summary>
        ///// 显示排序箭头
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void dgAttendRecAll_MouseMove(object sender, MouseEventArgs e)
        //{
        //    MyDataGridSortInChinese.SetColumnSortState();
        //}
        ///// <summary>
        ///// 显示排序箭头
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void dgAttendRecAll_LayoutUpdated(object sender, EventArgs e)
        //{
        //    MyDataGridSortInChinese.SetColumnSortState();
        //}
        //#endregion

        
        #region 排序

        private string _dir = "asc";
        private string _sortFiled = "";
        private DataGridColumnHeader _sortHeader;
        private DataGridColumnHeader _lastSortHeader;

        /// <summary>
        /// 排序箭头显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgAttendRecAll_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SetColumnSortState();
            dgAttendRecAll.UpdateLayout();
        }

        /// <summary>
        /// 鼠标左键点击DataGrid头针对对应列进行排序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void dgAttendRecAll_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataGrid dg = sender as DataGrid;
            if (_vmAttend.AttendRecModel == null || dg == null) //非空判断 by wz
            {
                return;
            }

            try
            {

                //获取界面元素
                var uiElement = from element in VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(null), (UIElement)null)
                                where element is DataGridColumnHeader
                                select element;
                int test = uiElement.Count();

                if (uiElement.Count() > 0)
                {
                    //鼠标点击的ColumnHeader 
                    DataGridColumnHeader header = (DataGridColumnHeader)uiElement.First();

                    if (header.Content == null)
                    {
                        return;
                    }
                    //要排序的字段 
                    string newSort = header.Content.ToString();

                    foreach (var col in dg.Columns.Where((columnItem) =>    //修改by wz 判断列头为空的情况
                    {
                        if (columnItem.Header == null)
                        {
                            return false;
                        }
                        return columnItem.Header.ToString() == newSort;
                    }))
                    {
                        //如果有绑定数据
                        if (col.ClipboardContentBinding != null &&
                            col.ClipboardContentBinding.Path != null &&
                            !col.ClipboardContentBinding.Path.Path.Equals(""))
                        {
                            _sortFiled = col.ClipboardContentBinding.Path.Path;
                            break;
                        }
                        return;
                    }

                    //判断升降序
                    if (_dir == "des")
                    {
                        _dir = "asc";
                    }
                    else
                    {
                        _dir = "des";
                    }

                    _lastSortHeader = _sortHeader;
                    _sortHeader = header;

                    //特殊排序自己实现
                    if (_sortFiled == "sum_work_time")
                    {
                        SortData(_vmAttend.AttendRecModel, "sum_work_time");
                    }
                    else if (_sortFiled == "avg_work_time")
                    {
                        SortData(_vmAttend.AttendRecModel, "avg_work_time");
                    }
                    else
                    {
                        MyDataGridSortInChinese.OrderData(sender, e, _vmAttend.AttendRecModel);
                    }

                }
                SetColumnSortState();
            }
            catch (Exception err)
            {
                ErrorWindow errWin = new ErrorWindow(err);
                errWin.Show();
            }
        }

        /// <summary>
        /// 调用公共排序
        /// </summary>
        /// <param name="data"></param>
        private void SortData(BaseViewModelCollection<UserAttendRec> data, string sortName)
        {
            if (_dir == "des")//
            {
                IOrderedEnumerable<UserAttendRec> sortedObject = data.OrderByDescending(u => u.GetType().GetProperties().Where(p =>
                p.Name == sortName).Single().GetValue(u, null).ToString(), new SpecialComparer());
                UserAttendRec[] sortedData = sortedObject.ToArray();  //执行这一步之后，排序的linq表达式才有效   
                data.Clear();
                foreach (var item in sortedData)
                {
                    data.Add(item);
                }
            }
            else
            {
                IOrderedEnumerable<UserAttendRec> sortedObject = data.OrderBy(u => u.GetType().GetProperties().Where(p =>
                p.Name == sortName).Single().GetValue(u, null).ToString(), new SpecialComparer());
                UserAttendRec[] sortedData = sortedObject.ToArray();  //执行这一步之后排序的linq表达式才有效
                data.Clear();
                foreach (var item in sortedData)
                {
                    data.Add(item);
                }
            }
        }

        /// <summary>
        /// 显示排序的上下箭头 
        /// </summary>
        private void SetColumnSortState()
        {
            if (_sortHeader == null)
            {
                return;
            }
            if (_dir == "asc")
            {
                VisualStateManager.GoToState(_sortHeader, "SortAscending", false);
            }
            else
            {
                VisualStateManager.GoToState(_sortHeader, "SortDescending", false);
            }

            if (_lastSortHeader != null && _lastSortHeader != _sortHeader)
            {
                VisualStateManager.GoToState(_lastSortHeader, "Unsorted", false);
            }
        }

        /// <summary>
        /// 显示排序箭头
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgAttendRecAll_MouseMove(object sender, MouseEventArgs e)
        {
            SetColumnSortState();
        }

        /// <summary>
        /// 显示排序箭头
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgAttendRecAll_LayoutUpdated(object sender, EventArgs e)
        {
            SetColumnSortState();
        }

        #endregion


        #region 查询条件

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQuery_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dateBegin.SelectedDate == null)
                {
                    MsgBoxWindow.MsgBox("请选择查询时间！",
                                         MsgBoxWindow.MsgIcon.Information,
                                         MsgBoxWindow.MsgBtns.OK);
                    return;
                }

                //设置查询条件
                SetCondition();

                _vmAttend.GetAttend();
                ///重新绑定  防止排序更新
                dgAttendRecAll.ItemsSource = _vmAttend.AttendRecModel;
                ///记住页面信息
                //NavigationCacheMode = System.Windows.Navigation.NavigationCacheMode.Required;
            }
            catch (Exception er)
            {
                ErrorWindow err = new ErrorWindow(er);
                err.Show();
            }
        }

        /// <summary>
        ///  设置查询条件
        /// </summary>
        private void SetCondition()
        {
            try
            {
                AttendQueryCondition qc = new AttendQueryCondition();
                //起始时间
                if (dateBegin.SelectedDate != null)
                {
                    qc.BeginTime = dateBegin.SelectedDate.Value.Date;
                }

                //结束时间
                if (dateEnd.SelectedDate != null)
                {
                    qc.EndTime = dateEnd.SelectedDate.Value.Date.AddDays(1);

                }

                qc.Name = txtBoxName.Text;
                qc.WorkSN = tbWorkSN.Text;

                //选择部门
                if (combDepart.SelectedIndex > 0)
                {
                    qc.DepartIdLst = new int[1];
                    qc.DepartIdLst[0] = (combDepart.SelectedItem as depart).depart_id;
                }

                ///如果存在 先删除该键值对
                if (_querySetting.Contains("attendConditon"))
                {
                    _querySetting.Remove("attendConditon");
                }
                _querySetting.Add("attendConditon", qc);
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
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
                    //btnQuery.IsEnabled = false;
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
}
