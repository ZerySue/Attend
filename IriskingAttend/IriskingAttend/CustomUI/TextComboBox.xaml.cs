/*************************************************************************
** 文件名:   TextComboBox.cs
** 主要类:   TextComboBox
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   wz
** 日  期:   2014-2-12
** 修改人:   
** 日  期:
** 描  述:   textBox与ComboBox的组合控件，用于某些多选情况。
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
using System.Text;

namespace IriskingAttend.CustomUI
{
    public partial class TextComboBox : UserControl
    {
        public TextComboBox()
        {
            InitializeComponent();

        }

        /// <summary>
        /// 初始化一般的操作对象
        /// 在InitComBoBoxItemSelectTrigger之前调用
        /// </summary>
        public void InitCommonOperation()
        {
            //对于多选项，首先要有选择全部和取消选择两个操作
            ComboBoxItem selectAll = new ComboBoxItem();
            selectAll.Content = "选择全部";
            selectAll.AddHandler(ComboBoxItem.MouseLeftButtonDownEvent,
                new MouseButtonEventHandler(comboBox_SelectAllEvent), true);

            ComboBoxItem selectNone = new ComboBoxItem();
            selectNone.Content = "取消选择";
            selectNone.AddHandler(ComboBoxItem.MouseLeftButtonDownEvent,
                new MouseButtonEventHandler(comboBox_SelectNoneEvent), true);

            comboBox.Items.Add(selectAll);
            comboBox.Items.Add(selectNone);
        }

        /// <summary>
        /// 初始监听comBoBox选择改变事件(待选择的item包括checkbox)
        /// 在载入item之后执行
        /// 在InitCommonOperation之后调用
        /// </summary>
        public void InitComBoBoxItemSelectTrigger()
        {
            foreach (var item in comboBox.Items)
            {
                CheckBox checkBox = item as CheckBox;
                if (checkBox != null)
                {
                    checkBox.Click += comboBox_SelectedEvent;
                }
                ComboBoxItem comboBoxItem = item as ComboBoxItem;
                if (comboBoxItem != null)
                {
                    comboBoxItem.AddHandler(ComboBoxItem.MouseLeftButtonDownEvent, new MouseButtonEventHandler(comboBox_SelectedEvent), true);
                }
            }
        }

        //combox选择项改变事件
        private void comboBox_SelectedEvent(object sender, RoutedEventArgs e)
        {
            StringBuilder content = new StringBuilder();
            int index = 0;
            foreach (var item in comboBox.Items)
            {
                if (item is CheckBox && ((CheckBox)item).IsChecked.Value)
                {
                    content.Append(((CheckBox)item).Content);
                    content.Append(",");
                    index++;
                }
            }
            if (index > 0)
            {
                content.Remove(content.Length - 1, 1);
            }
            textBox.Text = content.ToString();
        }

        //combox选择全部点击事件
        private void comboBox_SelectAllEvent(object sender, RoutedEventArgs e)
        {
            foreach (var item in comboBox.Items)
            {
                if (item is CheckBox)
                {
                    ((CheckBox)item).IsChecked = true;
                }
            }
        }

        //combox取消选择点击事件
        private void comboBox_SelectNoneEvent(object sender, RoutedEventArgs e)
        {
            foreach (var item in comboBox.Items)
            {
                if (item is CheckBox)
                {
                    ((CheckBox)item).IsChecked = false;
                }
            }
        }

        /// <summary>
        /// 手动选择全部
        /// </summary>
        public void SelectAll()
        {
            comboBox_SelectAllEvent(null, null);
            comboBox_SelectedEvent(null, null);
        }

        /// <summary>
        /// 手动取消选择
        /// </summary>
        public void SelectNone()
        {
            comboBox_SelectAllEvent(null, null);
            comboBox_SelectedEvent(null, null);
        }

    }
}
