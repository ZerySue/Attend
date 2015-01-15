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
using IriskingAttend.ExportExcel;
using IriskingAttend.Common;
using IriskingAttend.ViewModel;
using Lite.ExcelLibrary.SpreadSheet;
using System.Threading;
using IriskingAttend.ApplicationType;

namespace IriskingAttend.View.PeopleView
{
    public partial class PageClassTypeAndClassOrderMng : Page
    {

        public PageClassTypeAndClassOrderMng()
        {
            InitializeComponent();

            var vm = new VmClassTypeAndClassOrderMng();
            vm.ClassOrderAllSelect = this.Resources["SelectAllClassOrder"] as MarkObject;
            vm.ClassTypeAllSelect = this.Resources["SelectAllClasType"] as MarkObject;
            vm.ClassOrderSignAllSelect = this.Resources["SelectAllClassOrderSign"] as MarkObject;
            vm.ClassOrderJiGongShiAllSelect = this.Resources["SelectAllClassOrderJiGongShi"] as MarkObject;//szr
            
            vm.LoadCompletedEvent += (sender, e) =>
            {
                this.DataContext = sender;

                //初始化班次列表的内容，为第一个班制的班次
                this.dataGridClassType.SelectedIndex = 0;
                UserClassTypeInfo userClassTypeInfo = dataGridClassType.SelectedItem as UserClassTypeInfo;
                if (userClassTypeInfo != null)
                {
                    ((VmClassTypeAndClassOrderMng)this.DataContext).CurrentClassTypeChanged(userClassTypeInfo);
                }
                // modify by  szr
                int isSupport = AppTypePublic.GetIsSupportClassOrderSign();

                if (isSupport != 1) //不支持 签到班班次
                {
                    this.dgClassOrderSign.Visibility = Visibility.Collapsed;
                    txtCurrentClassOrderSign.Visibility = Visibility.Collapsed;
                    spClassOrderSignBtn.Visibility = Visibility.Collapsed;
                }
                if (isSupport != 2) //不支持 记工时班次
                {
                    this.dgClassOrderJiGongShi.Visibility = Visibility.Collapsed;
                    spClassOrderJiGongShiBtn.Visibility = Visibility.Collapsed;
                    txtCurrentClassOrderJiGongShi.Visibility = Visibility.Collapsed;

                }
            };

            #region    注册事件


            //注册班次列表的行加载事件
            this.dataGridClassOrder.LoadingRow += (sender, e) =>
            {
                MouseButtonEventHandler h = null;
                //注册班次列表的鼠标左键点击事件，先注销再添加
                e.Row.MouseLeftButtonUp -= h = delegate(object o, MouseButtonEventArgs ergs)
                {
                    if (this.dataGridClassOrder.CurrentColumn == null)
                    {
                        return;
                    }
                    if (dataGridClassOrder.CurrentColumn.DisplayIndex == 0)
                    {
                        if (dataGridClassOrder.SelectedItem != null)
                        {
                            var obj = dataGridClassOrder.SelectedItem as UserClassOrderInfo;
                            var vmodel = this.DataContext as VmClassTypeAndClassOrderMng;
                            if (vmodel != null)
                            {
                                vmodel.SelectOneClassOrder(obj);                                
                            }
                        }
                    }
                };
               
                
                e.Row.MouseLeftButtonUp += h;
                //序号设置
                int index = e.Row.GetIndex();
                var cell = dataGridClassOrder.Columns[1].GetCellContent(e.Row) as TextBlock;
                cell.Text = (index + 1).ToString();
            };

            //注册班制列表的行加载事件
            this.dataGridClassType.LoadingRow += (sender, e) =>
            {
                MouseButtonEventHandler h = null;
                //注册班制列表的鼠标左键点击事件，先注销再添加
                e.Row.MouseLeftButtonUp -= h = delegate(object o, MouseButtonEventArgs ergs)
                {
                    if (this.dataGridClassType.CurrentColumn == null)
                    {
                        return;
                    }
                    if (dataGridClassType.CurrentColumn.DisplayIndex == 0)
                    {
                        if (dataGridClassType.SelectedItem != null)
                        {
                            var obj = dataGridClassType.SelectedItem as UserClassTypeInfo;
                            var vmodel = this.DataContext as VmClassTypeAndClassOrderMng;
                            if (vmodel != null)
                            {
                                vmodel.SelectOneClassType(obj);
                            }
                        }
                    }
                };
                e.Row.MouseLeftButtonUp += h;
                //序号设置
                int index = e.Row.GetIndex();
                var cell = dataGridClassType.Columns[1].GetCellContent(e.Row) as TextBlock;
                cell.Text = (index + 1).ToString();
                
            };

            //注册签到班班次列表的行加载事件
            this.dgClassOrderSign.LoadingRow += (sender, e) =>
            {
                MouseButtonEventHandler h = null;
                //注册班次列表的鼠标左键点击事件，先注销再添加
                e.Row.MouseLeftButtonUp -= h = delegate(object o, MouseButtonEventArgs ergs)
                {
                    if (this.dgClassOrderSign.CurrentColumn == null)
                    {
                        return;
                    }
                    if (dgClassOrderSign.CurrentColumn.DisplayIndex == 0)
                    {
                        if (dgClassOrderSign.SelectedItem != null)
                        {
                            var obj = dgClassOrderSign.SelectedItem as UserClassOrderSignInfo;
                            var vmodel = this.DataContext as VmClassTypeAndClassOrderMng;
                            if (vmodel != null)
                            {
                                vmodel.SelectOneClassOrderSign(obj);
                            }
                        }
                    }
                };


                e.Row.MouseLeftButtonUp += h;
                //序号设置
                int index = e.Row.GetIndex();
                var cell = dgClassOrderSign.Columns[1].GetCellContent(e.Row) as TextBlock;
                cell.Text = (index + 1).ToString();
            };

            //注册记工时班次列表的行加载事件 szr
            this.dgClassOrderJiGongShi.LoadingRow += (sender, e) =>
            {
                MouseButtonEventHandler h = null;
                //注册班次列表的鼠标左键点击事件，先注销再添加
                e.Row.MouseLeftButtonUp -= h = delegate(object o, MouseButtonEventArgs ergs)
                { 
                   
                    if (this.dgClassOrderJiGongShi.CurrentColumn == null)
                    {
                        return;
                    }
                    if (dgClassOrderJiGongShi.CurrentColumn.DisplayIndex == 0)
                    {
                        if (dgClassOrderJiGongShi.SelectedItem != null)
                        {
                            var obj = dgClassOrderJiGongShi.SelectedItem as UserClassOrderJiGongShiInfo;
                            var vmodel = this.DataContext as VmClassTypeAndClassOrderMng;
                            if (vmodel != null)
                            {
                                vmodel.SelectOneClassOrderJiGongShi(obj);
                            }
                        }
                    }
                };


                e.Row.MouseLeftButtonUp += h;
                //序号设置
                int index = e.Row.GetIndex();
                var cell = dgClassOrderJiGongShi.Columns[1].GetCellContent(e.Row) as TextBlock;
                cell.Text = (index + 1).ToString();
            };


            dataGridClassType.AddHandler(MouseLeftButtonUpEvent, new MouseButtonEventHandler(dataGridClassType_MouseLeftButtonUp), true);
            dataGridClassOrder.AddHandler(MouseLeftButtonUpEvent, new MouseButtonEventHandler(dataGridClassOrder_MouseLeftButtonUp), true);
            dgClassOrderSign.AddHandler(MouseLeftButtonUpEvent, new MouseButtonEventHandler(dgClassOrderSign_MouseLeftButtonUp), true);
            dgClassOrderJiGongShi.AddHandler(MouseLeftButtonUpEvent, new MouseButtonEventHandler(dgClassOrderJiGongShi_MouseLeftButtonUp), true);
            #endregion

        }
       

        //全选班制
        private void ChkSelectAllClassType_Click(object sender, RoutedEventArgs e)
        {
            ((VmClassTypeAndClassOrderMng)this.DataContext).SelectAllClassType(((CheckBox)sender).IsChecked);
        }

        //全选班次
        private void ChkSelectAllClassOrder_Click(object sender, RoutedEventArgs e)
        {
            ((VmClassTypeAndClassOrderMng)this.DataContext).SelectAllClassOrder(((CheckBox)sender).IsChecked);
        }

        //全选签到班班次
        private void ChkSelectAllClassOrderSign_Click(object sender, RoutedEventArgs e)
        {
            ((VmClassTypeAndClassOrderMng)this.DataContext).SelectAllClassOrderSign(((CheckBox)sender).IsChecked);
        }


        /// <summary>
        /// 当前班制改变，获取对应的班次信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridClassType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //点击第一列无反应
            if (sender == null || ((DataGrid)sender).CurrentColumn == null || ((DataGrid)sender).CurrentColumn.DisplayIndex == 0)
            {
                return;
            }
            UserClassTypeInfo userClassTypeInfo = ((DataGrid)sender).SelectedItem as UserClassTypeInfo;
            if (userClassTypeInfo != null)
            {
                ((VmClassTypeAndClassOrderMng)this.DataContext).CurrentClassTypeChanged(userClassTypeInfo);
            }

         
        }
       
        //修改班制
        private void btnModifyClassType_Click(object sender, RoutedEventArgs e)
        {
            var info = (UserClassTypeInfo)((HyperlinkButton)sender).DataContext;
            ((VmClassTypeAndClassOrderMng)this.DataContext).ModifyClassType(info);
        }

        //删除班制
        private void btnDeleteClassType_Click(object sender, RoutedEventArgs e)
        {
            var info = (UserClassTypeInfo)((HyperlinkButton)sender).DataContext;
            ((VmClassTypeAndClassOrderMng)this.DataContext).DeleteClassType(info);
        }

        //修改班次
        private void btnModifyClassOrder_Click(object sender, RoutedEventArgs e)
        {
            var info = (UserClassOrderInfo)((HyperlinkButton)sender).DataContext;
            ((VmClassTypeAndClassOrderMng)this.DataContext).ModifyClassOrder(info);
        }

        //删除班次
        private void btnDeleteClassOrder_Click(object sender, RoutedEventArgs e)
        {
            var info = (UserClassOrderInfo)((HyperlinkButton)sender).DataContext;
            ((VmClassTypeAndClassOrderMng)this.DataContext).DeleteClassOrder(info);
        }

        //修改签到班班次
        private void btnModifyClassOrderSign_Click(object sender, RoutedEventArgs e)
        {
            var info = (UserClassOrderSignInfo)((HyperlinkButton)sender).DataContext;
            ((VmClassTypeAndClassOrderMng)this.DataContext).ModifyClassOrderSign(info);
        }

        //删除签到班班次
        private void btnDeleteClassOrderSign_Click(object sender, RoutedEventArgs e)
        {
            var info = (UserClassOrderSignInfo)((HyperlinkButton)sender).DataContext;
            ((VmClassTypeAndClassOrderMng)this.DataContext).DeleteClassOrderSign(info);
        }

        //导出班制
        private void ExportClassTypeExl_Click(object sender, RoutedEventArgs e)
        {
            string space = "                         ";
            ExpExcel.ExportExcelFromDataGrid(this.dataGridClassType, 2, 4, (space + "班制信息列表" + space), "班制信息列表",4,6000);
        }

        //导出班次
        private void ExportClassOrderExl_Click(object sender, RoutedEventArgs e)
        {
            string space = "                                                                                             ";
            ExpExcel.ExportExcelFromDataGrid(this.dataGridClassOrder, 2, 12, (space + "当前班次信息列表" + space), "当前班次信息列表");
        }

        #region 排序 

        //班制排序箭头显示
        private void dataGridClassType_MouseMove(object sender, MouseEventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        //班制排序
        private void dataGridClassType_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MyDataGridSortInChinese.OrderData(sender, e,
                ((VmClassTypeAndClassOrderMng)this.DataContext).ClassTypeInfos);
        }

        //班制排序箭头显示
        private void dataGridClassType_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        //班制排序箭头显示
        private void dataGridClassType_LayoutUpdated(object sender, EventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        //班次排序箭头显示
        private void dataGridClassOrder_LayoutUpdated(object sender, EventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        //班次排序
        private void dataGridClassOrder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MyDataGridSortInChinese.OrderData(sender, e,
               ((VmClassTypeAndClassOrderMng)this.DataContext).ClassOrderInfos);
        }

        //班次排序箭头显示
        private void dataGridClassOrder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        //班次排序箭头显示
        private void dataGridClassOrder_MouseMove(object sender, MouseEventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        //签到班班次排序箭头显示
        private void dgClassOrderSign_MouseMove(object sender, MouseEventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        //签到班班次排序
        private void dgClassOrderSign_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MyDataGridSortInChinese.OrderData(sender, e,
                ((VmClassTypeAndClassOrderMng)this.DataContext).ClassOrderSignInfos);
        }

        //签到班班次排序箭头显示
        private void dgClassOrderSign_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        //签到班班次排序箭头显示
        private void dgClassOrderSign_LayoutUpdated(object sender, EventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        #endregion
        /// <summary>
        /// 全选 记工时班次  add by szr 2014-11-14
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChkSelectAllClassOrderJiGongShi_Click(object sender, RoutedEventArgs e)
        {
            ((VmClassTypeAndClassOrderMng)this.DataContext).SelectAllClassOrderJiGongShi(((CheckBox)sender).IsChecked);
       
        }
        //记工时班次排序 szr
        private void dgClassOrderJiGongShi_LayoutUpdated(object sender, EventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }
        //记工时班次排序 箭头显示 szr
        private void dgClassOrderJiGongShi_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MyDataGridSortInChinese.OrderData(sender, e,
               ((VmClassTypeAndClassOrderMng)this.DataContext).ClassOrderJiGongShiInfos);
        }


        //签到班班次排序箭头显示
        private void dgClassOrderJiGongShi_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }
        //记工时班次排序 箭头显示 szr
        private void dgClassOrderJiGongShi_MouseMove(object sender, MouseEventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }
        //修改记工时班次
        private void btnModifyClassOrderJiGongShi_Click(object sender, RoutedEventArgs e)
        {
            var info = (UserClassOrderJiGongShiInfo)((HyperlinkButton)sender).DataContext;
            ((VmClassTypeAndClassOrderMng)this.DataContext).ModifyClassOrderJiGongShi(info);
        }
        //删除记工时班次
        private void btnDeleteClassOrderJiGongShi_Click(object sender, RoutedEventArgs e)
        {

            var info = (UserClassOrderJiGongShiInfo)((HyperlinkButton)sender).DataContext;
            ((VmClassTypeAndClassOrderMng)this.DataContext).DeleteClassOrderJiGongShi(info);

        }
        //导出Excel 记工时班次
        private void ExportClassOrderExlJiGongShi_Click(object sender, RoutedEventArgs e)
        {
            string space = "                                                                                   ";
            ExpExcel.ExportExcelFromDataGrid(this.dgClassOrderJiGongShi, 2, 12, (space + "当前记工时班次信息列表" + space), "当前记工时班次信息列表");
       
        }


  

    }
}
