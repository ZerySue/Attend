/*************************************************************************
** 文件名:   OperatorManage.cs
** 主要类:   OperatorManage
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2013-6-14
** 修改人:   
** 日  期:
** 描  述:   OperatorManage类,操作员管理界面
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
using System.Windows.Navigation;
using IriskingAttend.ViewModel.SystemViewModel;
using System.Windows.Data;
using IriskingAttend.ExportExcel;
using IriskingAttend.Common;
using IriskingAttend.Dialog;

namespace IriskingAttend.View.SystemView
{
    public partial class OperatorManage : Page
    {
        #region 私有变量初始化：vm变量

        //vm初始化
        private VmOperatorManage _vmOperatorMng = new VmOperatorManage();

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数，初始化
        /// </summary>
        public OperatorManage()
        {
            InitializeComponent();

            //从数据库中获得操作员信息
            _vmOperatorMng.GetOperatorInfoTableRia( null );

            //数据绑定
            this.DataContext = _vmOperatorMng;

            //操作员信息表数据源
            this.dgOperator.ItemsSource = _vmOperatorMng.OperatorInfoMng;

            //当前选中行绑定
            System.Windows.Data.Binding binding = new System.Windows.Data.Binding("SelectOperatorInfoItem") { Mode = BindingMode.TwoWay, };
            dgOperator.SetBinding(DataGrid.SelectedItemProperty, binding);

            //操作员信息列表序号
            this.dgOperator.LoadingRow += (a, e) =>
            {
                int index = e.Row.GetIndex();
                var cell = dgOperator.Columns[0].GetCellContent(e.Row) as TextBlock;
                cell.Text = (index + 1).ToString();
            };

            dgOperator.AddHandler(MouseLeftButtonUpEvent, new MouseButtonEventHandler(dgOperator_MouseLeftButtonUp), true);
        }

        #endregion

        #region 控件事件响应

        /// <summary>
        ///  修改操作员信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hbtnModifyOperatorInfo_Click(object sender, RoutedEventArgs e)
        {
            //显示修改操作员信息界面
            DlgModifyOperatorInfo dlgModifyOperatorInfo = new DlgModifyOperatorInfo(_vmOperatorMng.SelectOperatorInfoItem, OperatorItemMng_callback);
            dlgModifyOperatorInfo.Show();
        }

        /// <summary>
        /// 修改操作员密码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hbtnModifyOperatorPwd_Click(object sender, RoutedEventArgs e)
        {
            //显示修改操作员密码界面
            DlgModifyOperatorPwd dlgModifyOperatorPwd = new DlgModifyOperatorPwd(_vmOperatorMng.SelectOperatorInfoItem, OperatorItemMng_callback);
            dlgModifyOperatorPwd.Show();
        }

        /// <summary>
        /// 修改操作员权限
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hbtnModifyOperatorPurview_Click(object sender, RoutedEventArgs e)
        {
            if (VmLogin.IsSuperUser(_vmOperatorMng.SelectOperatorInfoItem.logname))
            {
                string info = string.Format("{0}超级用户拥有所有权限，不能修改权限！", _vmOperatorMng.SelectOperatorInfoItem.logname);
                MsgBoxWindow.MsgBox(info, MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);
                return;
            }

            DlgPrivilege dlgPrivilege = new DlgPrivilege(_vmOperatorMng.SelectOperatorInfoItem.logname, true);
            dlgPrivilege.Show();
        }

        /// <summary>
        /// 删除操作员
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hbtnDeleteOperator_Click(object sender, RoutedEventArgs e)
        {
            _vmOperatorMng.DeleteOperator();

            //操作员信息表数据源，排序后必须重新更新数据源
            this.dgOperator.ItemsSource = _vmOperatorMng.OperatorInfoMng;
        }

        /// <summary>
        ///  添加操作员
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddOperator_Click(object sender, RoutedEventArgs e)
        {
            //显示添加操作员界面
            DlgAddOperator dlgAddOperator = new DlgAddOperator(OperatorItemMng_callback);
            dlgAddOperator.Show();
        }

        /// <summary>
        /// 导出Excel按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            string space = "                         ";
            ExpExcel.ExportExcelFromDataGrid(this.dgOperator, 1, 4, (space + "操作员管理" + space), "操作员管理");
        }

        #endregion

        #region 回调函数

        /// <summary>
        /// 子窗口返回后，进行操作的回调函数
        /// 添加操作员、修改操作员信息后，全部操作员信息窗口重新刷新
        /// </summary>
        /// <param name="dialogResult"></param>
        private void OperatorItemMng_callback(bool? dialogResult)
        {
            if (dialogResult.HasValue && dialogResult.Value)
            {
                //重新查询数据库
                _vmOperatorMng.GetOperatorInfoTableRia( null );

                //操作员信息表数据源，排序后必须重新更新数据源
                this.dgOperator.ItemsSource = _vmOperatorMng.OperatorInfoMng;
            }
        }

        #endregion

        #region 操作员信息列表排序初始化及控件事件响应

        /// <summary>
        /// 操作员信息列表中，鼠标移动事件，显示排序的上下箭头
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgOperator_MouseMove(object sender, MouseEventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        /// <summary>
        /// 操作员信息列表中，外观刷新事件，显示排序的上下箭头
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgOperator_LayoutUpdated(object sender, EventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        /// <summary>
        /// 操作员信息列表左键点击事件发生时进行排序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgOperator_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MyDataGridSortInChinese.OrderData(sender, e, ((VmOperatorManage)this.DataContext).OperatorInfoMng);
        }

        private void dgOperator_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        #endregion
    }
}
