/*************************************************************************
** 文件名:   InWellPerson.cs
×× 主要类:   InWellPerson
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   yht
** 日  期:   2014-10-29
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
using IriskingAttend.Dialog;
using System.IO.IsolatedStorage;

namespace IriskingAttend.XiGouYiKuang
{
    public partial class XiGouInWellPersonReport : Page
    {
        #region 私有变量
        /// <summary>
        /// 当前井下人员VM数据
        /// </summary>
        private VmXiGouInWellPerson _vmInWellPerson = new VmXiGouInWellPerson();
        /// <summary>
        /// 本地存储
        /// </summary>
        private IsolatedStorageSettings _querySetting = IsolatedStorageSettings.ApplicationSettings;  
        /// <summary>
        /// 当前部门VM数据
        /// </summary>
        private VmDepartment _vmDepart = new VmDepartment();

        #endregion

        #region 构造函数
        public XiGouInWellPersonReport()
        {
            InitializeComponent();
            dtpBegin.Text=DateTime.Now.ToString("yyyy-MM-dd 00:00:00");
            timeBegin.Value =DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 00:00:00"));
            dtpEnd.Text = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            timeEnd.Value = DateTime.Now;
            dgInWellPerson.AddHandler(MouseLeftButtonUpEvent, new MouseButtonEventHandler(dgInWellPerson_MouseLeftButtonUp), true);
            dgInWellPerson.ItemsSource = _vmInWellPerson.XiGouInWellPersonModel;
        }

        
        #endregion

        #region 响应函数

        /// <summary>
        /// 按部门筛选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbDepart_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (cmbDepart.SelectedIndex > 0)
            //{
            //    _vmInWellPerson.SelectionChanged((cmbDepart.SelectedItem as depart).depart_name,VmInWellPerson.DevGroup.InOutWell);
            //}
            //else
            //{
            //    _vmInWellPerson.SelectionChanged("全部", VmInWellPerson.DevGroup.InOutWell);
            //}

            //dgInWellPerson.ItemsSource = _vmInWellPerson.InWellPersonModel;
            //labInWellPersonCount.Content = _vmInWellPerson.InWellPersonModel.Count;
        }

        /// <summary>
        /// 井下人员过滤
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void RadioButton_Click(object sender, RoutedEventArgs e)
        //{
        //    dgInWellPerson.ItemsSource = _vmInWellPerson.InWellPersonModel.Where(a => a.work_state == (sender as RadioButton).Content.ToString());
        //}


        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportExl_Click(object sender, RoutedEventArgs e)
        {
            string space = "                                             ";
            ExpExcel.ExportExcelFromDataGrid(dgInWellPerson, 0, 5, (space + "西沟一矿井下人员统计汇总表" + space.Substring(0, space.Length-15) + "导出日期:"+DateTime.Now.ToString("yyyy年MM月dd日")), "西沟一矿井下人员统计汇总表");
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
            if (_vmInWellPerson.XiGouInWellPersonModel == null || dg == null) //非空判断 by wz
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
                        //SortData(_vmInWellPerson.InWellPersonModel);
                    }
                    else
                    {
                        MyDataGridSortInChinese.OrderData(sender, e, _vmInWellPerson.XiGouInWellPersonModel);
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

        #region 查询

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQuery_Click(object sender, RoutedEventArgs e)
        {
            if (dtpBegin.Text == null || dtpBegin.Text == "")
            {
                MsgBoxWindow.MsgBox(
                            "查询开始时间不能为空！",
                            Dialog.MsgBoxWindow.MsgIcon.Information,
                            Dialog.MsgBoxWindow.MsgBtns.OK);
                return;
            }
            if (dtpEnd.Text == null || dtpEnd.Text == "")
            {
                MsgBoxWindow.MsgBox(
                            "查询结束时间不能为空！",
                            Dialog.MsgBoxWindow.MsgIcon.Information,
                            Dialog.MsgBoxWindow.MsgBtns.OK);
                return;
            }

            XiGouQueryCondition qc = new XiGouQueryCondition();

            qc.BeginTime = this.dtpBegin.SelectedDate.Value.Date;
            if (timeBegin.Value!=null)
            {
                qc.BeginTime += timeBegin.Value.Value.TimeOfDay;
            }
            if (dtpEnd.Text == null || dtpEnd.Text == "")
            {
                qc.EndTime = DateTime.Now.Date;
            }
            else
            {
                qc.EndTime = this.dtpEnd.SelectedDate.Value.Date;
            }

            if (timeEnd.Value!=null)
            {
                qc.EndTime += timeEnd.Value.Value.TimeOfDay;
            }
            if (qc.EndTime < qc.BeginTime)
            {
                MsgBoxWindow.MsgBox(
                            "结束时间不能大于开始时间！",
                            Dialog.MsgBoxWindow.MsgIcon.Information,
                            Dialog.MsgBoxWindow.MsgBtns.OK);
                return;
            }

            ///如果存在 先删除该键值对
            if (_querySetting.Contains("attendConditon"))
            {
                _querySetting.Remove("attendConditon");
            }
            _querySetting.Add("attendConditon", qc);
            _vmInWellPerson.GetXiGouInWellList(qc.BeginTime, qc.EndTime);
        }
        #endregion
    }

    #region 报表查询条件

    /// <summary>
    /// 报表查询条件
    /// </summary>
    public class XiGouQueryCondition
    {
        /// <summary>
        /// 起始时间
        /// </summary>
        public DateTime BeginTime { get; set; }

        /// <summary>
        /// 截止时间
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public XiGouQueryCondition()
        {
            BeginTime = DateTime.Now;
            EndTime = DateTime.Now;
        }
    }
    #endregion

}
