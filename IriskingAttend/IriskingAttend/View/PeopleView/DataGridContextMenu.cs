/*************************************************************************
** 文件名:   DGContextMenu.cs
** 主要类:   DGContextMenu
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   wz
** 日  期:   2013-4-11
** 修改人:   
** 日  期:
** 描  述:   dataGird右键点击弹出菜单
**
** 版  本:   1.0.0
** 备  注:  命名及代码编写遵守C#编码规范
**
 * ***********************************************************************/

using System;
using System.Net;
using System.Windows;

using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using IriskingAttend.ViewModel.PeopleViewModel;
using Irisking.Web.DataModel;

namespace IriskingAttend.View.PeopleView
{
    //Derives from the abstract class ContextMenu that provides the boilerplate code for displaying a popup window. 
    /// <summary>
    /// 右键菜单类
    /// </summary>
    public class DGContextMenu : IriskingAttend.Dialog.RightBtnMenu.ContextMenu
    {
        DataGrid dataGrid;
        Action<PersonOptionMode, int> callBack;
        int choosed_personId = -1;

        public DGContextMenu(DataGrid dg, Action<PersonOptionMode, int> _callBack)
        {
            callBack = _callBack;
            this.dataGrid = dg;
            if ((UserPersonInfo)this.dataGrid.SelectedItem != null)
            {
                choosed_personId = (int)((UserPersonInfo)this.dataGrid.SelectedItem).person_id;
            }
            
        }

        //Construct the context menu and return the parent FrameworkElement. 

        /// <summary>
        /// 构造菜单内容
        /// </summary>
        /// <returns></returns>
        protected override FrameworkElement GetContent()
        {
            Border border = new Border() { BorderBrush = new SolidColorBrush(Color.FromArgb(255, 167,171,176)), BorderThickness = new Thickness(1), Background = new SolidColorBrush(Colors.White) };
            border.Effect = new DropShadowEffect() { BlurRadius = 3, Color = Color.FromArgb(255, 230, 227, 236) };

            Grid grid = new Grid() { Margin = new Thickness(1) };
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(25) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(105) });

            grid.Children.Add(new Rectangle() { Fill = new SolidColorBrush(Color.FromArgb(255, 233, 238, 238)) });
            grid.Children.Add(new Rectangle() { Fill = new SolidColorBrush(Color.FromArgb(255, 226, 228, 231)), HorizontalAlignment = HorizontalAlignment.Right, Width = 1 });

            AddItme(grid, "查看", PersonOptionMode.Check);
            AddItme(grid, "修改", PersonOptionMode.Modify);
            AddItme(grid, "删除", PersonOptionMode.Delete);
            AddItme(grid, "记录", PersonOptionMode.Record);
            AddItme(grid, "停用虹膜", PersonOptionMode.StopIris);
           


            border.Child = grid;

            return border;
        }

        /// <summary>
        /// 往grid里添加按钮
        /// </summary>
        /// <param name="grid">要添加的目标</param>
        /// <param name="name">添加按钮的名字</param>
        /// <param name="option">按钮的功能</param>
        void AddItme(Grid grid, string name, PersonOptionMode option)
        {
            //计算grid里现有的按钮数目
            int index = 0;
            foreach (var item in grid.Children)
            {
                if (item.GetType().Equals(typeof(Button)))
                {
                    index++;
                }
            }

            Button addButton = new Button() { Height = 22, Margin = new Thickness(0, index*24, 0, 0), HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Top, HorizontalContentAlignment = HorizontalAlignment.Left };
            addButton.Style = Application.Current.Resources["ContextMenuButton"] as Style;
            addButton.Click += MouseLeftButtonUp;
            addButton.Tag = option;
            Grid.SetColumnSpan(addButton, 2);

            StackPanel sp_modify = new StackPanel() { Orientation = Orientation.Horizontal };

            Image modifyImage = new Image() { HorizontalAlignment = HorizontalAlignment.Left, Width = 16, Height = 16, Margin = new Thickness(1, 0, 0, 0) };
            modifyImage.Source = new BitmapImage(new Uri("/IriskingAttend;component/images/funImage.png", UriKind.RelativeOrAbsolute));
            sp_modify.Children.Add(modifyImage);

            TextBlock modifyText = new TextBlock() { Text = name, HorizontalAlignment = HorizontalAlignment.Left, Margin = new Thickness(16, 0, 0, 0) };
            sp_modify.Children.Add(modifyText);

            addButton.Content = sp_modify;

            grid.Children.Add(addButton);
        }


        #region 点击菜单事件
        
        //点击菜单事件
        void MouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
            callBack((PersonOptionMode)((Button)sender).Tag, choosed_personId);
            Close();
        }
     
        #endregion
    }
}
