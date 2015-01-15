/*************************************************************************
** 文件名:   Page_childDepartMng.cs
×× 主要类:   Page_childDepartMng
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   cty
** 日  期:   2013-6-14
** 修改人:   
** 日  期:
** 描  述:   Page_childDepartMng类,子部门管理页面
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
using IriskingAttend.ViewModel.PeopleViewModel;
using Irisking.Web.DataModel;
using IriskingAttend.Common;

namespace IriskingAttend.View.PeopleView
{
    public partial class Page_childDepartMng : Page
    {
        public Page_childDepartMng()
        {
            InitializeComponent();
          

        }

        // 当用户导航到此页面时执行。
        protected override void OnNavigatedTo(NavigationEventArgs eArgs)
        {
            var vm = new VmChildDepartMng(this.NavigationService);
            vm.LoadCompletedEvent += new EventHandler(vm_LoadCompletedEvent);
            //添加序号 不能采用绑定的方式，绑定的话排序后序号会跟着变化by cty
            this.dataGrid1.LoadingRow += (a, e) =>
            {
                int index = e.Row.GetIndex();
                var cell = dataGrid1.Columns[0].GetCellContent(e.Row) as TextBlock;
                cell.Text = (index + 1).ToString();
            };

            dataGrid1.AddHandler(MouseLeftButtonUpEvent, new MouseButtonEventHandler(dataGrid1_MouseLeftButtonUp), true);
        }


        /// <summary>
        /// viewmodel加载完毕事件
        /// </summary>
        /// <param name="sender">viewModel实例</param>
        /// <param name="e"></param>
        void vm_LoadCompletedEvent(object sender, EventArgs e)
        {
            this.DataContext = sender;
        }

        /// <summary>
        /// 删除按钮点击函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            VmChildDepartMng vm = this.DataContext as VmChildDepartMng;
            var departInfo = ((HyperlinkButton)sender).DataContext as UserDepartInfo;
            if (vm != null && departInfo != null)
            {
                vm.Delete(departInfo);
            }
        }

        #region 排序 by cty



        private void dataGrid1_MouseMove(object sender, MouseEventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        private void dataGrid1_LayoutUpdated(object sender, EventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }


        private void dataGrid1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MyDataGridSortInChinese.OrderData(sender, e, ((VmChildDepartMng)this.DataContext).ChildDepartInfos);
        }

        private void dataGrid1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        #endregion

    }
}
