/*************************************************************************
** 文件名:   InWellPerson.cs
×× 主要类:   InWellPerson
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   lzc
** 日  期:   2013-4-18
** 修改人:   
** 日  期:
** 描  述:   InWellPerson类，当前井下人员
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
using IriskingAttend.ViewModel.SafeManager;
using System.Windows.Data;
using Irisking.Web.DataModel;
using System.Windows.Controls.Primitives;
using IriskingAttend.ViewModel;
using IriskingAttend.Common;
using IriskingAttend.ExportExcel;
using IriskingAttend.ApplicationType;
using IriskingAttend.Dialog;
using System.IO.IsolatedStorage;

namespace IriskingAttend.ViewMine
{
    public partial class InWellPersonMine : Page
    {
        #region 私有变量
        /// <summary>
        /// 当前在岗人员VM数据
        /// </summary>
        private VmInWellPerson _vmInWellPerson = new VmInWellPerson();
        /// <summary>
        /// 部门VM数据
        /// </summary>
        private VmDepartment _vmDepart = new VmDepartment();

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数  设置页面绑定
        /// </summary>
        public InWellPersonMine()
        {
            InitializeComponent();
            SetPageBind();            

            ///设置序列号
            dgInWellPerson.LoadingRow += (a, e) =>
            {
                int index = e.Row.GetIndex();
                var cell = dgInWellPerson.Columns[0].GetCellContent(e.Row) as TextBlock;
                cell.Text = (index + 1).ToString();

                e.Row.MouseLeftButtonUp -= new MouseButtonEventHandler(Row_MouseLeftButtonUp);
                e.Row.MouseLeftButtonUp += new MouseButtonEventHandler(Row_MouseLeftButtonUp);
            };
            dgInWellPerson.AddHandler(MouseLeftButtonUpEvent, new MouseButtonEventHandler(dgInWellPerson_MouseLeftButtonUp), true);
           // _vmInWellPerson.ShowOverInWellDialog();
        }

        /// <summary>
        /// 点击行事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Row_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (AppTypePublic.GetCustomAppType().GetType().ToString().CompareTo("IriskingAttend.ApplicationType.ZhouYuanShanApp") != 0)
            {
                return;
            }

            if (!PublicMethods.IsUserControlDoubleClicked(this.dgInWellPerson))
            {
                return;
            }

            try
            {

                UserInWellPerson userPerson = (UserInWellPerson)dgInWellPerson.SelectedItem;

                DateTime beginTime = DateTime.Parse( userPerson.in_time.ToString("yyyy-MM-01 00:00:00") );
                DateTime endTime = beginTime.AddMonths(1);

                VmAttend vmAttend = new VmAttend();
                vmAttend.SelectAttendRec = new UserAttendRec();
                vmAttend.SelectAttendRec.person_id = userPerson.person_id;
                vmAttend.SelectAttendRec.person_name = userPerson.person_name;
                vmAttend.SelectAttendRec.work_sn = userPerson.work_sn;

                IsolatedStorageSettings querySetting = IsolatedStorageSettings.ApplicationSettings;

                AttendQueryCondition qc = new AttendQueryCondition();               
                qc.BeginTime = beginTime;             
                qc.EndTime = endTime;   
                qc.Name = userPerson.person_name;
                qc.WorkSN = userPerson.work_sn;             
                

                ///如果存在 先删除该键值对
                if (querySetting.Contains("attendConditon"))
                {
                    querySetting.Remove("attendConditon");
                }
                querySetting.Add("attendConditon", qc);
                vmAttend.ShowAttendRecDetail();

                //vmAttend.GetAttendRecDetail(beginTime, endTime, null, 0, userPerson.person_id);
                //AttendRecordDetail detailDialog = new AttendRecordDetail(vmAttend);

                ////detailDialog.Closed += (o, e) =>
                ////{
                ////    if (detailDialog.isChanged)
                ////    {
                ////        GetAttend();
                ////    }
                ////};
                //detailDialog.Show();
            }
            catch (Exception ex)
            {
                ErrorWindow err = new ErrorWindow(ex);
                err.Show();
            }            
        }

        /// <summary>
        /// 设置绑定
        /// </summary>
        private void SetPageBind()
        {
            _vmDepart.GetDepartment();
            _vmDepart.DepartmentLoadCompleted += (o, e) =>
            {
                cmbDepart.SelectedIndex = 0;
                _vmInWellPerson.GetInWellPerson();
            };
            cmbDepart.ItemsSource = _vmDepart.DepartmentModel;
            cmbDepart.DisplayMemberPath = "depart_name";
            dgInWellPerson.ItemsSource = _vmInWellPerson.InWellPersonModel;

            Binding binding = new Binding("InWellPersonModel.Count") { Mode = BindingMode.TwoWay };
            binding.Source = _vmInWellPerson;
            labInWellPersonCount.SetBinding(Label.ContentProperty, binding);

            Binding bindingOverCount = new Binding("InWellPersonOverModel.Count") { Mode = BindingMode.TwoWay };
            bindingOverCount.Source = _vmInWellPerson;
            hbtnOverInWell.SetBinding(HyperlinkButton.ContentProperty, bindingOverCount);
        }
        #endregion

        #region 响应函数
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        /// <summary>
        /// 按部门筛选
        /// </summary>
        /// <param name="sender">cmbDepart</param>
        /// <param name="e">SelectionChanged</param>
        private void cmbDepart_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            SelectChanged();
            //人员过滤绑定
            //RadioButton_Click(null, null);
        }

        private void SelectChanged()
        {
            if (cmbDepart.SelectedIndex > 0)
            {
                _vmInWellPerson.SelectionChanged((cmbDepart.SelectedItem as depart).depart_name, GetDevGroup());
            }
            else
            {
                _vmInWellPerson.SelectionChanged("全部", GetDevGroup());
            }
        }
        /// <summary>
        /// 井下人员过滤
        /// </summary>
        /// <param name="sender">RadioButton</param>
        /// <param name="e">Click</param>
        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            if(rbtnInWell.IsChecked??false)
            {
                labPerson.Content = "当前井下人总数：";
                labOverPerson.Content = "当前井下超时人数：";
                SelectChanged();
               
            }
            else if (rbtnOutWell.IsChecked ?? false)
            {
                labPerson.Content = "当前地面人总数：";
                labOverPerson.Content = "当前地面超时人数：";
                SelectChanged();
              
            }
            else
            {
                labPerson.Content = "当前在岗总人数：";
                labOverPerson.Content = "当前在岗超时人数：";
                SelectChanged();
            }    
        }

        private VmInWellPerson.DevGroup GetDevGroup()
        {
            if (rbtnInWell.IsChecked ?? false)
            {
                return VmInWellPerson.DevGroup.InWell;
            }
            else if (rbtnOutWell.IsChecked ?? false)
            {
                return VmInWellPerson.DevGroup.OutWell;
            }
            else
            {
               return VmInWellPerson.DevGroup.InOutWell;
            }  
        }
        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="sender">btnExportExl</param>
        /// <param name="e">Click</param>
        private void btnExportExl_Click(object sender, RoutedEventArgs e)
        {
            string space = "                                                                 ";
            ExpExcel.ExportExcelFromDataGrid(dgInWellPerson, 1, 8, (space + "当前在岗人员" + space), "当前在岗人员");
        }
        #endregion

        #region 排序
        //排序模式
        private string _dir = "asc";
        //排序字段
        private string _sortFiled;
        //排序DataGrid 头
        private DataGridColumnHeader _sortHeader;
        private DataGridColumnHeader _lastSortHeader;

        /// <summary>
        /// 通过鼠标左键抬起事件响应箭头显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgInWellPerson_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SetColumnSortState();
            dgInWellPerson.UpdateLayout();
        }
        /// <summary>
        /// 通过鼠标左键事件响应排序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgInWellPerson_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataGrid dg = sender as DataGrid;
            if (_vmInWellPerson.InWellPersonModel == null || dg == null) //非空判断 by wz
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
                    if (_sortFiled == "work_time")
                    {
                        SortData(_vmInWellPerson.InWellPersonModel);
                    }
                    else
                    {
                        MyDataGridSortInChinese.OrderData(sender, e, _vmInWellPerson.InWellPersonModel);
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
        private void SortData(BaseViewModelCollection<UserInWellPerson> data)
        {
            if (_dir == "des")//
            {
                IOrderedEnumerable<UserInWellPerson> sortedObject = data.OrderByDescending(u => u.work_time, new SpecialComparer());
                UserInWellPerson[] sortedData = sortedObject.ToArray();  //执行这一步之后，排序的linq表达式才有效   
                data.Clear();
                foreach (var item in sortedData)
                {
                    data.Add(item);
                }
            }
            else
            {
                IOrderedEnumerable<UserInWellPerson> sortedObject = data.OrderBy(u => u.work_time, new SpecialComparer());
                UserInWellPerson[] sortedData = sortedObject.ToArray();  //执行这一步之后排序的linq表达式才有效
                data.Clear();
                foreach (var item in sortedData)
                {
                    data.Add(item);
                }
            }
        }

        /// <summary>
        /// 显示排序的上下箭头 --待测
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
        /// 设置排序箭头显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgInWellPerson_MouseMove(object sender, MouseEventArgs e)
        {
            SetColumnSortState();
        }

        /// <summary>
        /// 设置排序箭头显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgInWellPerson_LayoutUpdated(object sender, EventArgs e)
        {
            SetColumnSortState();
        }

        #endregion 

        /// <summary>
        /// 显示井下超时人员信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hbtnOverInWell_Click(object sender, RoutedEventArgs e)
        {
            _vmInWellPerson.ShowOverInWellDialog();
        }
      
    }
}
