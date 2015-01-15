/*************************************************************************
** 文件名:   RecogInfo.cs
×× 主要类:   RecogInfo
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   lzc
** 日  期:   2013-4-8
** 修改人:   
** 日  期:
** 描  述:   RecogInfo类，识别记录查询界面
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
using IriskingAttend.ViewModel;
using Irisking.Web.DataModel;
using System.Windows.Data;
using IriskingAttend.Common;
using System.Windows.Controls.Primitives;
using IriskingAttend.ApplicationType;

namespace IriskingAttend.Dialog
{
    public partial class RecogInfo : ChildWindow
    {
        #region 私有变量
        /// <summary>
        /// 识别记录数据模型
        /// </summary>
        private VmRecogInfo _vmRecogInfo = new VmRecogInfo();

        /// <summary>
        /// 人员信息
        /// </summary>
        private UserPersonSimple _person = new UserPersonSimple();
        #endregion

        #region 构造

        /// <summary>
        /// 构造函数1
        /// </summary>
        /// <param name="personID">人员ID</param>
        /// <param name="name">人员姓名</param>
        /// <param name="workSN">人员工号</param>
        public RecogInfo(int personID, string name, string workSN)
        {
            InitializeComponent();

            _vmRecogInfo.GetPersonRecog(personID);
            _person.person_id = personID;
            _person.person_name = name;
            _person.work_sn = workSN;

            lbName.Content = name;
            lbWorkSN.Content = workSN;
            init();

            //added by gqy at 2014-02-13 周源山需求
            if (!VmLogin.DictPrivilege[AbstractApp.PrivilegeENUM.AttendIdentifyRecord])
            {
                dgRecog.Columns.Where(col => col.Header.ToString() == "操  作").First().Visibility = Visibility.Collapsed;
                btnAddRecord.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 构造函数重构1
        /// 查询时间段内 识别记录
        /// </summary>
        /// <param name="personID">人员ID</param>
        /// <param name="name">人员姓名</param>
        /// <param name="workSN">人员工号</param>
        /// <param name="beginTime">起始时间</param>
        /// <param name="endTime">结束时间</param>
        public RecogInfo(int personID, string name, string workSN, DateTime beginTime, DateTime endTime)
        {
            InitializeComponent();
            _vmRecogInfo.GetPersonRecog(beginTime, endTime, personID);

            lbName.Content = name;
            _person.person_id = personID;
            _person.person_name = name;
            _person.work_sn = workSN;

            lbWorkSN.Content = workSN;
            init();

            //added by gqy at 2014-02-13 周源山需求
            if (!VmLogin.DictPrivilege[AbstractApp.PrivilegeENUM.AttendIdentifyRecord])
            {
                dgRecog.Columns.Where(col => col.Header.ToString() == "操  作").First().Visibility = Visibility.Collapsed;
                btnAddRecord.Visibility = Visibility.Collapsed;
            }
        }



        /// <summary>
        /// 构造函数重构2
        /// 通过人员查询识别记录 
        /// </summary>
        /// <param name="person">人员简单信息</param>
        public RecogInfo(UserPersonInfo person)
        {
            InitializeComponent();

            _vmRecogInfo.GetPersonRecog(person.person_id);
            Binding binding = new Binding("BindingRecog") { Mode = BindingMode.TwoWay };
            binding.Source = _vmRecogInfo;
            dgRecog.SetBinding(DataGrid.SelectedItemProperty, binding);

            _person.person_id = person.person_id;
            _person.person_name = person.person_name;
            _person.work_sn = person.work_sn;
            lbName.Content = person.person_name;
            lbWorkSN.Content = person.work_sn;

            init();

            //added by gqy at 2014-02-13 周源山需求
            if (!VmLogin.DictPrivilege[AbstractApp.PrivilegeENUM.AttendIdentifyRecord])
            {
                dgRecog.Columns.Where(col => col.Header.ToString() == "操  作").First().Visibility = Visibility.Collapsed;
                btnAddRecord.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 设置初始化
        /// </summary>
        private void init()
        {
            ///取消人工补加列
            dgRecog.Columns.Where(a => a.Header.ToString() == "识别类型").First().Visibility = System.Windows.Visibility.Collapsed;

            dgRecog.LoadingRow += (a, e) =>
            {
                int index = e.Row.GetIndex();

                ///通过转换e.Row.DataContext 获取当前行显示颜色
                SolidColorBrush brush = new SolidColorBrush(
                    mathFun.ReturnColorFromString(((UserPersonRecogLog)e.Row.DataContext).recog_type_color));
                //当前行显示颜色
                e.Row.Foreground = brush;

                //设置序号
                var cell = dgRecog.Columns[0].GetCellContent(e.Row) as TextBlock;
                cell.Text = (index + 1).ToString();
            };

            dgRecog.AddHandler(MouseLeftButtonUpEvent, new MouseButtonEventHandler(dgRecog_MouseLeftButtonUp), true);

            _vmRecogInfo.AttendRecgLoadCompleted += (a, e) =>
            {
                ///资源绑定
                dgRecog.ItemsSource = _vmRecogInfo.RecogInfoModel;
            };

            //是否显示识别类型
            _vmRecogInfo.IsShowTypeCompleted += (a, e) =>
            {
                if (_vmRecogInfo.IsShowRecogType == 0)
                {
                    ///取消人工补加列
                    dgRecog.Columns.Where(col => col.Header.ToString() == "识别类型").First().Visibility = System.Windows.Visibility.Visible;
                }
            };
        }
        #endregion

        /// <summary>
        /// 取消  关闭对话框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        /// <summary>
        /// 超链接 显示添加识别记录对话框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            DateTime? recogTime = null;
            if (dgRecog.SelectedItem != null)
            {
                recogTime = ((UserPersonRecogLog)dgRecog.SelectedItem).recog_time;
            }
            _vmRecogInfo.ShowAddRecogDialog(_person, recogTime);
        }

        /// <summary>
        /// 重构识别记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRebuild_Click(object sender, RoutedEventArgs e)
        {
            _vmRecogInfo.RebuildPersonRecog((UserPersonRecogLog)dgRecog.SelectedItem);
        }

        /// <summary>
        /// 删除识别记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDel_Click(object sender, RoutedEventArgs e)
        {
            _vmRecogInfo.DelPersonRecog((UserPersonRecogLog)dgRecog.SelectedItem);
        }



        #region 排序


        /// <summary>
        /// 同构鼠标左键事件响应排序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgRecog_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
            dgRecog.UpdateLayout();
        }

        /// <summary>
        /// 鼠标左键点击DataGrid头针对对应列进行排序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgRecog_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                MyDataGridSortInChinese.OrderData(sender, e, _vmRecogInfo.RecogInfoModel);
            }
            catch (Exception er)
            {
                ErrorWindow err = new ErrorWindow(er);
                err.Show();
            }
        }

        /// <summary>
        /// 鼠标移动时 显示排序的上下箭头 
        ///// </summary>
        private void dgRecog_MouseMove(object sender, MouseEventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        /// <summary>
        /// 界面更新 显示排序的上下箭头 
        ///// </summary>
        private void dgRecog_LayoutUpdated(object sender, EventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        #endregion

    }
}

