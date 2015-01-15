/*************************************************************************
** 文件名:   OperatorLog.cs
** 主要类:   OperatorLog
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2013-10-30
** 修改人:   cty
** 日  期:   2014-3-11
 * 修改内容：添加五虎山项目中只有admin用户可见可操作的‘清除无用记录’的功能
** 描  述:   OperatorLog类,操作员日志界面
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
using Irisking.Web.DataModel;
using IriskingAttend.ApplicationType;

namespace IriskingAttend.View.SystemView
{
    public partial class OperatorLog : Page
    {
        #region 私有变量初始化：vm变量

        //vm初始化
        private VmOperatorLog _vmOperatorLog = new VmOperatorLog();

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数，初始化
        /// </summary>
        public OperatorLog()
        {
            InitializeComponent();

            //查询权限默认可见           
            if (VmLogin.DictPrivilege.ContainsKey(AbstractApp.PrivilegeENUM.SystemOperatorLogQuery) && !VmLogin.DictPrivilege[AbstractApp.PrivilegeENUM.SystemOperatorLogQuery])
            {
                this.btnQuery.Visibility = Visibility.Collapsed;
            }

            //删除操作员日志的权限默认不可见
            dgOperatorLog.Columns[7].Visibility = Visibility.Collapsed;
            if (VmLogin.DictPrivilege.ContainsKey(AbstractApp.PrivilegeENUM.SystemOperatorLogDelete) && VmLogin.DictPrivilege[AbstractApp.PrivilegeENUM.SystemOperatorLogDelete])
            {
                dgOperatorLog.Columns[7].Visibility = Visibility.Visible;
            }

            this.combOperator.Items.Add("全部");

            VmOperatorManage vmOperatorMng = new VmOperatorManage();

            vmOperatorMng.GetOperatorInfoTableRia(() =>
            {
                
                foreach( operator_info item in vmOperatorMng.OperatorInfoMng)
                {
                    this.combOperator.Items.Add(item.logname);
                }
                this.combOperator.SelectedIndex = 0;

                //从数据库中获得操作员日志
                _vmOperatorLog.GetOperatorLogTableRia( 500 );
            });           

            //数据绑定
            this.DataContext = _vmOperatorLog;

            //操作员日志表数据源
            this.dgOperatorLog.ItemsSource = _vmOperatorLog.OperatorLogMng;

            //当前选中行绑定
            System.Windows.Data.Binding binding = new System.Windows.Data.Binding("SelectOperatorLogItem") { Mode = BindingMode.TwoWay, };
            dgOperatorLog.SetBinding(DataGrid.SelectedItemProperty, binding);

            //操作员日志列表序号
            this.dgOperatorLog.LoadingRow += (a, e) =>
            {
                int index = e.Row.GetIndex();
                var cell = dgOperatorLog.Columns[0].GetCellContent(e.Row) as TextBlock;
                cell.Text = (index + 1).ToString();
            };

            dgOperatorLog.AddHandler(MouseLeftButtonUpEvent, new MouseButtonEventHandler(dgOperatorLog_MouseLeftButtonUp), true);
        }

        #endregion

        #region 控件事件响应
        /// <summary>
        /// 操作员日志查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQuery_Click(object sender, RoutedEventArgs e)
        {
            labNote.Visibility = Visibility.Collapsed;

            string userName = "";

            if (combOperator.SelectedIndex != 0)
            {
                userName = combOperator.SelectedItem.ToString();
            }
            _vmOperatorLog.GetOperatorLogTableRia(dpBegin.Text, dpEnd.Text, userName);
        }

        /// <summary>
        /// 详细信息查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hbtnViewOperatorLog_Click(object sender, RoutedEventArgs e)
        {
            DlgLogDetail dlgLogDetail = new DlgLogDetail();

            dlgLogDetail.txtOperator.Text = _vmOperatorLog.SelectOperatorLogItem.user_name;
            dlgLogDetail.txtOperateTime.Text = _vmOperatorLog.SelectOperatorLogItem.operation_time.ToString("yyyy-MM-dd HH:mm:ss");
            dlgLogDetail.txtOperateIP.Text = _vmOperatorLog.SelectOperatorLogItem.user_ip;
            dlgLogDetail.txtOperateResult.Text = _vmOperatorLog.SelectOperatorLogItem.resultStr;
            dlgLogDetail.txtOperateContent.Text = _vmOperatorLog.SelectOperatorLogItem.content;
            dlgLogDetail.txtOperateDescrip.Text = _vmOperatorLog.SelectOperatorLogItem.description;
            
            dlgLogDetail.Show();
        }

        /// <summary>
        /// 清除无用记录(五虎山admin用户才有该功能) by cty 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            UserOperationLog UserOperationLog = (((HyperlinkButton)sender).DataContext as UserOperationLog);
            ((VmOperatorLog)this.DataContext).DeleteUserOperationLog(UserOperationLog);
        }

        #endregion        

        #region 操作员信息列表排序初始化及控件事件响应

        /// <summary>
        /// 操作员信息列表中，鼠标移动事件，显示排序的上下箭头
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgOperatorLog_MouseMove(object sender, MouseEventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        /// <summary>
        /// 操作员信息列表中，外观刷新事件，显示排序的上下箭头
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgOperatorLog_LayoutUpdated(object sender, EventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        /// <summary>
        /// 操作员信息列表左键点击事件发生时进行排序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgOperatorLog_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MyDataGridSortInChinese.OrderData(sender, e, ((VmOperatorLog)this.DataContext).OperatorLogMng);
        }

        private void dgOperatorLog_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        #endregion           


    }
}
