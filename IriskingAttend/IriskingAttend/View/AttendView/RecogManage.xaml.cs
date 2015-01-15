
/*************************************************************************
** 文件名:   RecogManage.cs
×× 主要类:   RecogManage
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   lzc
** 日  期:   2013-4-8
** 修改人:   gqy
** 日  期:   2013-10-21   添加识别记录时显示默认时间
** 描  述:   RecogManage类 识别记录查询结果
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
using System.Windows.Data;
using IriskingAttend.Common;
using IriskingAttend.ExportExcel;
using System.Windows.Controls.Primitives;
using IriskingAttend.Dialog;
using IriskingAttend.ApplicationType;

namespace IriskingAttend.View
{
    public partial class RecogManage : Page
    {
        #region 私有变量
        /// <summary>
        /// 部门VM数据
        /// </summary>
        private VmDepartment _vmDepart = new VmDepartment();
        /// <summary>
        /// 识别记录VM数据
        /// </summary>
        private VmRecogInfo _vmRecogInfo = new VmRecogInfo();

        /// <summary>
        /// 查询条件
        /// </summary>
        private RecogCondition _recogContion = new RecogCondition();
        #endregion

        #region  构造函数

        /// <summary>
        /// 构造函数  初始化绑定数据，并设置识别记录列 识别类型 不显示
        /// </summary>
        public RecogManage()
        {

            InitializeComponent();
            Init();

            ///取消人工补加列
            dgRecord.Columns.Where(a => a.Header.ToString() == "识别类型").First().Visibility = System.Windows.Visibility.Collapsed;

        }

        /// <summary>
        /// 数据绑定初始化
        /// </summary>
        private void Init()
        {
            try
            {
                ///资源绑定
                dgRecord.ItemsSource = _vmRecogInfo.RecogInfoModel;
                dgPersonDetail.ItemsSource = _vmDepart.UserPersonSimpleForDepartModel;

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

                //添加序列号
                dgPersonDetail.LoadingRow += (a, e) =>
                {
                    int index = e.Row.GetIndex();
                    var cell = dgPersonDetail.Columns[0].GetCellContent(e.Row) as TextBlock;
                    cell.Text = (index + 1).ToString();
                };
                dgPersonDetail.AddHandler(MouseLeftButtonUpEvent, new MouseButtonEventHandler(dgRecord_MouseLeftButtonUp), true);
                dgRecord.LoadingRow += (a, e) =>
                {
                    int index = e.Row.GetIndex();

                    ///通过转换e.Row.DataContext 获取当前行显示颜色
                    SolidColorBrush brush = new SolidColorBrush(
                        mathFun.ReturnColorFromString(((UserPersonRecogLog)e.Row.DataContext).recog_type_color));

                    //当前行显示颜色
                    e.Row.Foreground = brush;
                    //序号
                    var cell = dgRecord.Columns[0].GetCellContent(e.Row) as TextBlock;
                    cell.Text = (index + 1).ToString();
                };
                dgRecord.AddHandler(MouseLeftButtonUpEvent, new MouseButtonEventHandler(dgRecord_MouseLeftButtonUp), true);

                //是否显示识别类型
                _vmRecogInfo.IsShowTypeCompleted += (a, e) =>
                   {
                       if (_vmRecogInfo.IsShowRecogType == 0)
                       {
                           ///取消人工补加列
                           dgRecord.Columns.Where(col => col.Header.ToString() == "识别类型").First().Visibility = System.Windows.Visibility.Visible;
                       }
                   };
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }

        #endregion

        #region  响应函数

        /// <summary>
        /// 进入界面
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (App.Current.Resources.Contains("RecogManageDateBegin"))
            {
                dateBegin.Text = App.Current.Resources["RecogManageDateBegin"].ToString();
            }

            if (App.Current.Resources.Contains("RecogManageDateEnd"))
            {
                dateEnd.Text = App.Current.Resources["RecogManageDateEnd"].ToString();
            }
        }

        /// <summary>
        /// 页面离开
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (!App.Current.Resources.Contains("RecogManageDateBegin"))
            {
                if (dateBegin.Text != null)
                {
                    App.Current.Resources.Add("RecogManageDateBegin", dateBegin.Text);
                }
            }
            else
            {
                try
                {
                    App.Current.Resources.Remove("RecogManageDateBegin");

                    if (!App.Current.Resources.Contains("RecogManageDateBegin"))
                    {
                        App.Current.Resources.Add("RecogManageDateBegin", dateBegin.Text);
                    }
                }
                catch (Exception ee)
                {
                    string err = ee.Message;
                }
            }

            if (!App.Current.Resources.Contains("RecogManageDateEnd"))
            {
                if (dateEnd.Text != null)
                {
                    App.Current.Resources.Add("RecogManageDateEnd", dateEnd.Text);
                }
            }
            else
            {
                App.Current.Resources.Remove("RecogManageDateEnd");
                App.Current.Resources.Add("RecogManageDateEnd", dateEnd.Text);
            }
        }

        /// <summary>
        /// 重构数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRebuildRecorg_Click(object sender, RoutedEventArgs e)
        {
            _vmRecogInfo.RebuildPersonRecog((UserPersonRecogLog)dgRecord.SelectedItem);
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelRecog_Click(object sender, RoutedEventArgs e)
        {
            _vmRecogInfo.DelPersonRecog((UserPersonRecogLog)dgRecord.SelectedItem);
        }


        /// <summary>
        /// 时间过滤条件
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
        /// 时间过滤条件
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
        /// <summary>
        /// 查询满足条件的识别记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQuery_Click(object sender, RoutedEventArgs e)
        {
            if (dateBegin.SelectedDate != null)
            {
                _recogContion.BeginDate = dateBegin.SelectedDate.Value;
            }
            else
            {
                MsgBoxWindow.MsgBox("请选择识别记录开始时间！",
                                    MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);
                return;
            }

            if (dateEnd.SelectedDate != null)
            {
                _recogContion.EndDate = dateEnd.SelectedDate.Value.AddDays(1);
            }

            _recogContion.PersonName = txtName.Text.ToString();
            _recogContion.WorkSn = txtWorkSn.Text.ToString();
            int departID = 0;

            ///如果增加全部查询  应把combDepart.SelectedIndex > 0
            if (combDepart.SelectedIndex > -1)
            {
                _recogContion.DepartName = (combDepart.SelectedItem as depart).depart_name;
                departID = (combDepart.SelectedItem as depart).depart_id;
            }
            _vmDepart.GetUserPersonSimple();
            _vmDepart.UserPersonSimpleLoadCompleted += (obj,arg) =>
                {
                    _vmDepart.GetUserPersonSimple(txtName.Text.ToString(), departID, txtWorkSn.Text.ToString());
                };
            
            dgPersonDetail.ItemsSource = _vmDepart.UserPersonSimpleForDepartModel;
            NavigationCacheMode = System.Windows.Navigation.NavigationCacheMode.Required;
            //重新查询时  清空识别记录内容
            _vmRecogInfo.RecogInfoModel.Clear();
        }


        /// <summary>
        /// 将当前人员识别记录导出Excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportExl_Click(object sender, RoutedEventArgs e)
        {
            string space = "                                                  ";
            ExpExcel.ExportExcelFromDataGrid(dgRecord, 1, 7, (space + "识别记录管理" + space), "识别记录管理");
        }

        /// <summary>
        /// 查看识别记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hbLookRecorg_Click(object sender, RoutedEventArgs e)
        {

            _vmRecogInfo.GetPersonRecog(_recogContion, ((dgPersonDetail.SelectedItem) as UserPersonSimple).person_id);
            ///重新绑定防止排序更新
            dgRecord.ItemsSource = _vmRecogInfo.RecogInfoModel;
        }
        /// <summary>
        /// 添加识别记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddRecord_Click(object sender, RoutedEventArgs e)
        {
            if (dgPersonDetail.SelectedItem != null)
            {
                //_vmRecogInfo.ShowAddRecogDialog((UserPersonSimple)dgPersonDetail.SelectedItem);
                //modify by gqy 20131021
                DateTime? recogTime = null;
                if (dgRecord.SelectedItem != null)
                {
                    recogTime = ((UserPersonRecogLog)dgRecord.SelectedItem).recog_time;
                }
                if (AppTypePublic.GetCustomAppType().GetType().ToString().CompareTo("IriskingAttend.ApplicationType.ZhongKeHongBaApp") == 0)
                {
                    _vmRecogInfo.ShowZKHBAddRecogDialog((UserPersonSimple)(dgPersonDetail.SelectedItem), recogTime);
                }
                else
                {
                    _vmRecogInfo.ShowAddRecogDialog((UserPersonSimple)dgPersonDetail.SelectedItem, recogTime);
                }  
            }
            else
            {
                MsgBoxWindow.MsgBox("请选择要添加识别记录的人！",
                                    MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);
            }

        }

        /// <summary>
        /// 根据选择的人员 查询人员的识别记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgPersonDetail_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgPersonDetail.SelectedIndex > -1)
            {
                _vmRecogInfo.GetPersonRecog(_recogContion, ((dgPersonDetail.SelectedItem) as UserPersonSimple).person_id);
                ///重新绑定防止排序更新
                dgRecord.ItemsSource = _vmRecogInfo.RecogInfoModel;
            }
        }
        #endregion

        #region 排序

        /// <summary>
        /// 同构鼠标左键事件响应排序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgRecord_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
            dgPersonDetail.UpdateLayout();
            dgRecord.UpdateLayout();
        }

        /// <summary>
        /// 同构鼠标左键事件响应排序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgRecord_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var unit = from element in VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(null), (System.Windows.UIElement)null)
                           where element is DataGridColumnHeader
                           select element;
                DataGridColumnHeader header;
                if (unit.Count() > 0)
                {
                    header = (DataGridColumnHeader)unit.First();//.Single();
                }
                else
                {
                    return;
                }
                if (header.Content == null)
                {
                    return;
                }

                //要排序的字段 
                string _newSort = header.Content.ToString();
                if ((sender as DataGrid) == dgRecord)
                {
                    ///对识别记录进行排序
                    MyDataGridSortInChinese.OrderData(sender, e, _vmRecogInfo.RecogInfoModel);
                }
                else
                {
                    ///对人员进行排序
                    MyDataGridSortInChinese.OrderData(sender, e, _vmDepart.UserPersonSimpleForDepartModel);
                }
            }
            catch (Exception er)
            {
                ErrorWindow err = new ErrorWindow(er);
                err.Show();
            }
        }


        /// <summary>
        /// 排序箭头显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgRecord_MouseMove(object sender, MouseEventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        /// <summary>
        /// 排序箭头显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgRecord_LayoutUpdated(object sender, EventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }
        #endregion

    }
}
