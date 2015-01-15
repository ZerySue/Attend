/*************************************************************************
** 文件名:   ChildWnd_BatchStopIris.cs
×× 主要类:   ChildWnd_BatchStopIris
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   cty
** 日  期:   2013-6-14
** 修改人:   
** 日  期:
** 描  述:   ChildWnd_BatchStopIris类,批量停用虹膜页面
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
using Irisking.Web.DataModel;
using IriskingAttend.ViewModel;
using IriskingAttend.Web;
using IriskingAttend.ViewModel.PeopleViewModel;

namespace IriskingAttend.View.PeopleView
{
    /// <summary>
    /// 停用虹膜窗口UI后台
    /// </summary>
    public partial class ChildWnd_BatchStopIris : ChildWindow
    {

        #region 字段声明
       
        //被选中的人员
        BaseViewModelCollection<UserPersonInfo> selectedUserPersons;
        //回调函数
        Action<bool?> callback;

        public List<int> personIDs = new List<int>();

        #endregion

        #region 构造函数

        public ChildWnd_BatchStopIris(BaseViewModelCollection<UserPersonInfo> _selectedUserPersons_, bool IsBatch, Action<bool?> _callback)//BaseViewModelCollection<PersonStopIrisInfo> _PersonStopIrisInfo_Selected
        {
            InitializeComponent();
            selectedUserPersons = _selectedUserPersons_;
            this.dgSelectedPerson.ItemsSource = selectedUserPersons;
            callback = _callback;

          
            VmChildWndBatchStopIris vm = new VmChildWndBatchStopIris(selectedUserPersons, IsBatch);
            this.DataContext = vm;
            vm.MarkObj = this.Resources["MarkObject"] as MarkObject;

            this.dateBegin.DisplayDateStart = System.DateTime.Now;
            this.dateEnd.IsEnabled = false;

            //使当前被选择的人员不能被操作
            this.dgSelectedPerson.LoadingRow += new EventHandler<DataGridRowEventArgs>((sender, e) =>
            {
                e.Row.IsHitTestVisible = false;
            });
            this.Dispatcher.BeginInvoke(() =>
                {
                    dgSelectedPerson.CurrentColumn = dgSelectedPerson.Columns[dgSelectedPerson.Columns.Count - 1];

                });

            //关闭窗口事件
            vm.CloseEvent += new EventHandler((o,e) =>
            {
                this.DialogResult = true;
            });

        }
     
        #endregion

        #region 界面事件响应函数
        

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            if (callback != null) callback(this.DialogResult);
        }

        //开始时间选择改变
        private void dateBegin_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            
            this.dateEnd.DisplayDateStart = this.dateBegin.SelectedDate;
            this.dateEnd.IsEnabled = true;
            if (this.dateEnd.SelectedDate.HasValue)
            {
                int res = this.dateEnd.SelectedDate.Value.CompareTo(this.dateBegin.SelectedDate.Value);
                if (res < 0)
                {
                    this.dateEnd.SelectedDate = null;
                    this.dateEnd.DisplayDateStart = this.dateBegin.SelectedDate;
                }
            }
        }

        //全选人员操作
        private void chkSelectAll_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext != null)
            {
                ((VmChildWndBatchStopIris)this.DataContext).SelectAllPerson(((CheckBox)sender).IsChecked);
            }
        }

        //添加按钮 by cty
        private void Btn_Add_Click(object sender, RoutedEventArgs e)
        {
            if (!this.dateBegin.SelectedDate.HasValue)
            {
                Dialog.MsgBoxWindow.MsgBox("开始日期不能为空！", Dialog.MsgBoxWindow.MsgIcon.Information, Dialog.MsgBoxWindow.MsgBtns.OK);
                return;
            }
            DateTime beginTime = dateBegin.SelectedDate.HasValue ? dateBegin.SelectedDate.Value : DateTime.MaxValue;
            DateTime endTime = dateEnd.SelectedDate.HasValue ? dateEnd.SelectedDate.Value : DateTime.MaxValue;
            if (endTime != DateTime.MaxValue)
            {
                endTime = endTime.AddHours(23.99999);
            }
            int policy = comb_option.SelectedIndex;
            ((VmChildWndBatchStopIris)this.DataContext).PersonStopIrisAddClicked(beginTime,endTime,policy);
        }

        #region 按需选择datagrid的行

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DG_PersonStopIrisData_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.MouseLeftButtonUp -= new MouseButtonEventHandler(Row_MouseLeftButtonUp);
            e.Row.MouseLeftButtonUp += new MouseButtonEventHandler(Row_MouseLeftButtonUp);
        }

        void Row_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.DataContext != null)
            {
                if (DG_PersonStopIrisData.CurrentColumn.DisplayIndex == 0)
                {
                    var info = DG_PersonStopIrisData.SelectedItem as PersonStopIrisInfo;
                    ((VmChildWndBatchStopIris)this.DataContext).SelectItems(info);
                }
               
            }
        }

        

        #endregion

        #endregion
    }
}

