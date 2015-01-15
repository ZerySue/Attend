/*************************************************************************
** 文件名:   AttendRecord.cs
×× 主要类:   AttendRecord
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   lzc
** 日  期:   2013-4-8
** 修改人:   gqy
** 日  期:   2013-9-11
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
using IriskingAttend.ViewModel.AttendViewModel;
using IriskingAttend.NewWuHuShan;
using IriskingAttend.ApplicationType;

namespace IriskingAttend.ViewMine
{
    public partial class AttendRecordMine : Page
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

        /// <summary>
        /// 绑定矿山查询列表
        /// </summary>
        private VmDevType _vmDevType = new VmDevType();

        /// <summary>
        /// 职务、工种vm数据  add by gqy
        /// </summary>
        private VmXlsFilter _vmXlsFilter = new VmXlsFilter();

        #endregion

        #region  构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public AttendRecordMine()
        {
            InitializeComponent();
            try
            {
                SetInit();
                dgAttendRecAll.LoadingRow += (a, e) =>
                {
                    int index = e.Row.GetIndex();
                    var cell = dgAttendRecAll.Columns[0].GetCellContent(e.Row) as TextBlock;
                    cell.Text = (index + 1).ToString();
                };

                dgAttendRecSignAll.LoadingRow += (a, e) =>
                {
                    int index = e.Row.GetIndex();
                    var cell = dgAttendRecSignAll.Columns[0].GetCellContent(e.Row) as TextBlock;
                    cell.Text = (index + 1).ToString();
                };
               
                dgAttendRecAll.AddHandler(MouseLeftButtonUpEvent, new MouseButtonEventHandler(dgAttendRecAll_MouseLeftButtonUp), true);
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }

        /// <summary>
        /// 设置初始化界面
        /// </summary>
        private void SetInit()
        {
            AppTypePublic.GetIsSupportClassOrderSignRia(() =>
            {
                if (AppTypePublic.GetIsSupportClassOrderSign()!=1)
                {
                    this.txtAttendRecordSign.Visibility = Visibility.Collapsed;
                    this.dgAttendRecSignAll.Visibility = Visibility.Collapsed;
                    this.btnExportExlSign.Visibility = Visibility.Collapsed;
                }

                //if (AppTypePublic.GetIsSupportClassOrderSign() != 2)//不支持记工时班次
                //{
                //    this.txtAttendRecordJiGongshi.Visibility = Visibility.Collapsed;
                //    this.dgAttendRecSignAll.Visibility = Visibility.Collapsed;
                //    this.btnExportExlSign.Visibility = Visibility.Collapsed;
                //}
                /////获取部门信息
                //_vmDepart.GetDepartment();

                //if (AppTypePublic.GetIsSupportClassOrderSign()==1)
                //{
                //    _vmAttend.AttendRecLoadCompleted += new EventHandler(Attend_AttendRecLoadCompleted);
                //}
            });
            
            /////部门信息加载完成事件
            //_vmDepart.DepartmentLoadCompleted += (o, e) =>
            //{
            //    combDepart.ItemsSource = _vmDepart.DepartmentModel;
            //    combDepart.DisplayMemberPath = "depart_name";

            //    if (combDepart.Items.Count > 0)
            //    {
            //        combDepart.SelectedIndex = 0;
            //    }
                
            //    _vmXlsFilter.GetWorkType();
            //};

            ////add begin by gqy
            //_vmXlsFilter.WorkTypeLoadCompleted += delegate
            //{
            //    if (_vmXlsFilter.WorkTypeModel.Count > 0)
            //    {
            //        this.cmbWorkType.SelectedIndex = 0;
            //    }
            //    _vmXlsFilter.GetPrincipalInfo();
            //};
            //this.cmbWorkType.ItemsSource = _vmXlsFilter.WorkTypeModel;
            //this.cmbWorkType.DisplayMemberPath = "work_type_name";

            //_vmXlsFilter.PrincipalInfoLoadCompleted += delegate
            //{
            //    if (_vmXlsFilter.PrincipalInfoModel.Count > 0)
            //    {
            //        this.cmbPrincipal.SelectedIndex = 0;
            //    }                
            //};
            //this.cmbPrincipal.ItemsSource = _vmXlsFilter.PrincipalInfoModel;
            //this.cmbPrincipal.DisplayMemberPath = "principal_name";
            ////add end by gqy

            ///设置绑定信息上下文
            dgAttendRecAll.DataContext = _vmAttend;
            dgAttendRecAll.ItemsSource = _vmAttend.AttendRecModel;            
            dgAttendRecAll.MouseLeftButtonDown += new MouseButtonEventHandler(dgAttendRecAll_MouseLeftButtonDown);

            ///设置绑定
            System.Windows.Data.Binding binding = new System.Windows.Data.Binding("SelectAttendRec") 
            { Mode = BindingMode.TwoWay, };
            dgAttendRecAll.SetBinding(DataGrid.SelectedItemProperty, binding);            

            ///设备类型源
            combDevype.ItemsSource = _vmDevType.DevTypeAndAll;
            combDevype.DisplayMemberPath = "Value";
            if (combDevype.Items.Count > 0)
            {
                combDevype.SelectedIndex = 0;
            }

        //    ///签到班
        //    dgAttendRecSignAll.DataContext = _vmAttend;
        //    dgAttendRecSignAll.ItemsSource = _vmAttend.AttendRecSignModel;

        //    ///设置绑定
        //    System.Windows.Data.Binding bindingSign = new System.Windows.Data.Binding("SelectAttendRecSign") { Mode = BindingMode.TwoWay, };
        //    dgAttendRecSignAll.SetBinding(DataGrid.SelectedItemProperty, bindingSign);

        //    dgAttendRecSignAll.MouseLeftButtonDown += new MouseButtonEventHandler(dgAttendRecSignAll_MouseLeftButtonDown);
        //
        }

        #endregion

        #region 响应事件函数
        private void Attend_AttendRecLoadCompleted(object o, EventArgs e)
        {
            _vmAttend.GetAttendRecSign();
        }
        /// <summary>
        /// 进入界面
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (App.Current.Resources.Contains("AttendRecordMineDateBegin"))
            {
                dateBegin.Text = App.Current.Resources["AttendRecordMineDateBegin"].ToString();
            }

            if (App.Current.Resources.Contains("AttendRecordMineDateEnd"))
            {
                dateEnd.Text = App.Current.Resources["AttendRecordMineDateEnd"].ToString();
            }
        }

        /// <summary>
        /// 页面离开
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (!App.Current.Resources.Contains("AttendRecordMineDateBegin"))
            {
                if (dateBegin.Text != null)
                {
                    App.Current.Resources.Add("AttendRecordMineDateBegin", dateBegin.Text);
                }
            }
            else
            {
                try
                {
                    App.Current.Resources.Remove("AttendRecordMineDateBegin");

                    if (!App.Current.Resources.Contains("AttendRecordMineDateBegin"))
                    {
                        App.Current.Resources.Add("AttendRecordMineDateBegin", dateBegin.Text);
                    }
                }
                catch (Exception ee)
                {
                    string err = ee.Message;
                }
            }

            if (!App.Current.Resources.Contains("AttendRecordMineDateEnd"))
            {
                if (dateEnd.Text != null)
                {
                    App.Current.Resources.Add("AttendRecordMineDateEnd", dateEnd.Text);
                }
            }
            else
            {
                App.Current.Resources.Remove("AttendRecordMineDateEnd");
                App.Current.Resources.Add("AttendRecordMineDateEnd", dateEnd.Text);
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
            string space = "                                                                             ";
            ExpExcel.ExportExcelFromDataGrid(dgAttendRecAll, 1, 10, (space + "稽核记录查询" + space), "稽核记录查询");
        }
        #endregion

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
                var uiElement = from element in VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(null), (System.Windows.UIElement)null)
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
               // IOrderedEnumerable<UserAttendRec> sortedObject = data.OrderBy(u => sortName, new SpecialComparer());
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

        #region 签到班排序及事件响应
        /// <summary>
        /// 排序箭头显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgAttendRecSignAll_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SetColumnSortState();
            dgAttendRecSignAll.UpdateLayout();
        }

        public void dgAttendRecSignAll_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataGrid dg = sender as DataGrid;
            if (_vmAttend.AttendRecSignModel == null || dg == null) //非空判断 by wz
            {
                return;
            }

            try
            {

                //获取界面元素
                var uiElement = from element in VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(null), (System.Windows.UIElement)null)
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
                        SortData(_vmAttend.AttendRecSignModel, "sum_work_time");
                    }
                    else if (_sortFiled == "avg_work_time")
                    {
                        SortData(_vmAttend.AttendRecSignModel, "avg_work_time");
                    }
                    else
                    {
                        MyDataGridSortInChinese.OrderData(sender, e, _vmAttend.AttendRecSignModel);
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
        /// 显示排序箭头
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgAttendRecSignAll_MouseMove(object sender, MouseEventArgs e)
        {
            SetColumnSortState();
        }

        /// <summary>
        /// 显示排序箭头
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgAttendRecSignAll_LayoutUpdated(object sender, EventArgs e)
        {
            SetColumnSortState();
        }

        /// <summary>
        /// 显示详细信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnShowDetailsSign_Click(object sender, RoutedEventArgs e)
        {
            _vmAttend.ShowAttendRecSignDetail();
        }
        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportExlSign_Click(object sender, RoutedEventArgs e)
        {
            //string space = "                                                                                     ";
           // ExpExcel.ExportExcelFromDataGrid(dgAttendRecAll, 1, 10, (space + "考勤查询" + space), "考勤查询");
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
                    MsgBoxWindow.MsgBox( "请选择查询时间！",
                            MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);
                    return;
                }

                //设置查询条件
                if (!SetCondition())
                {
                    return;
                }
                ///查询考勤信息
                _vmAttend.GetAttend();

                ///重新绑定  防止排序更新
                dgAttendRecAll.ItemsSource = _vmAttend.AttendRecDetailModel;
               // dgAttendRecSignAll.ItemsSource = _vmAttend.AttendRecSignModel;
                
                //NavigationCacheMode = System.Windows.Navigation.NavigationCacheMode.Required;
            }
            catch (Exception er)
            {
                ErrorWindow err = new ErrorWindow(er);
                err.Show();
            }
        }
        
        //本地独立存储，用来传参
        private IsolatedStorageSettings querySetting = IsolatedStorageSettings.ApplicationSettings;

        /// <summary>
        ///  设置查询条件
        /// </summary>
        private bool SetCondition()
        {
            try
            {
                AttendQueryCondition qc = new AttendQueryCondition();
                if (dateBegin.SelectedDate != null)
                {
                    qc.BeginTime = dateBegin.SelectedDate.Value.Date;
                }

                if (dateEnd.SelectedDate != null)
                {
                    qc.EndTime = dateEnd.SelectedDate.Value.Date.AddDays(1);
                }

                qc.Name = txtBoxName.Text;
                qc.WorkSN = tbWorkSN.Text;

                if (combDepart.SelectedIndex > 0)
                {
                    qc.DepartIdLst = new int[1];
                    qc.DepartIdLst[0] = (combDepart.SelectedItem as depart).depart_id;
                }

                if (combDevype.SelectedIndex > 0)
                {
                    qc.DevTypeIdLst = new int[1];
                    //出入井1*3 =3，上下班2*3 =6
                    qc.DevTypeIdLst[0] = 3 * combDevype.SelectedIndex;// (int)((KeyValuePair<DevTypeENUM, string>)combAttendType.SelectedItem).Key;
                }

                // add begin by gqy
                if (txtWorkTime.Text.Trim() == "" || txtWorkTime.Text.Trim() == null)
                {
                    qc.WorkTime = 0;
                }
                else
                {
                    try
                    {
                        qc.WorkTime = int.Parse(txtWorkTime.Text.Trim());
                    }
                    catch
                    {
                        Dialog.MsgBoxWindow.MsgBox(
                             "请确定您输入的工作时长为数字！",
                             Dialog.MsgBoxWindow.MsgIcon.Information,
                             Dialog.MsgBoxWindow.MsgBtns.OK);
                        return false;
                    }
                }
                
                if (cmbPrincipal.SelectedIndex > 0)
                {
                    qc.PrincipalIdList = new int[1];
                    qc.PrincipalIdList[0] = (cmbPrincipal.SelectedItem as PrincipalInfo).principal_id;
                }

                if (cmbWorkType.SelectedIndex > 0)
                {
                    qc.WorkTypeIdList = new int[1];
                    qc.WorkTypeIdList[0] = (cmbWorkType.SelectedItem as WorkTypeInfo).work_type_id;
                }
                //add end by gqy

                ///如果存在 先删除该键值对
                if (querySetting.Contains("attendConditon"))
                {
                    querySetting.Remove("attendConditon");
                }
                querySetting.Add("attendConditon", qc);
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
                return false;
            }

            return true;
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
            MsgBoxWindow.MsgBox( "日期格式不对！", MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);
        }

        /// <summary>
        /// 选择结束时间不能大于开始时间校验
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
