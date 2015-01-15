/*************************************************************************
** 文件名:   ListStackPanelItem.cs
** 主要类:   ListStackPanelItem
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   wz
** 日  期:   2013-12-30
** 修改人:   
** 日  期:
** 描  述:   ListStackPanelItem类,用于显示每一个分时间段设备用途
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
using Irisking.Web.DataModel;

namespace IriskingAttend.YangMei
{
    public partial class ListStackPanelItem : UserControl
    {
        /// <summary>
        /// 添加项事件
        /// </summary>
        public event EventHandler AddItemEvent;
        
        /// <summary>
        /// 删除项事件
        /// </summary>
        public event EventHandler DeleteItemEvent;
        
        /// <summary>
        /// 设备用途字典
        /// </summary>
        Dictionary<int, string> _dictionaryDevType;

        public ListStackPanelItem()
        {
            InitializeComponent();
            this.addBtn.Click += new RoutedEventHandler(addBtn_Click);
            this.deleteBtn.Click += new RoutedEventHandler(deleteBtn_Click);
        }

        private void deleteBtn_Click(object sender, RoutedEventArgs e)
        {
            DeleteItemEvent(this, null);
        }

        private void addBtn_Click(object sender, RoutedEventArgs e)
        {
            AddItemEvent(this, null);
        }

        /// <summary>
        /// 初始化内容
        /// </summary>
        /// <param name="devTypeName"></param>
        /// <param name="startTime"></param>
        /// <param name="isAdd"></param>
        public void InitContent(string devTypeName,string startTime,bool isAdd)
        {
            _dictionaryDevType = PublicMethods.GetDevTypeDictionary(VmLogin.GetIsMineApplication());
            this.cmbDevType.ItemsSource = _dictionaryDevType;
            this.cmbDevType.DisplayMemberPath = "Value";
            this.cmbDevType.SelectedValue = _dictionaryDevType.FirstOrDefault((info) =>
                {
                    return info.Value == devTypeName;
                });
            if (this.cmbDevType.SelectedValue == null)
            {
                this.cmbDevType.SelectedIndex = 0;
            }

            this.timePicker.Value = Convert.ToDateTime(startTime);

            if (isAdd)
            {
                this.addBtn.Visibility = Visibility.Visible;
                this.deleteBtn.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.addBtn.Visibility = Visibility.Collapsed;
                this.deleteBtn.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// 获取内容
        /// </summary>
        /// <returns></returns>
        public DeviceInfo GetContent()
        {
            DeviceInfo deviceInfo = new DeviceInfo();
            deviceInfo.dev_type = ((KeyValuePair<int, string>)this.cmbDevType.SelectedValue).Key;
            deviceInfo.start_time = this.timePicker.Value.Value.ToString("HH:mm:ss");
            return deviceInfo;
        }

        /// <summary>
        /// 设置按钮可见性
        /// </summary>
        /// <param name="isAdd"></param>
        public void SetBtnVisibility(bool isAdd)
        {
            if (isAdd)
            {
                this.addBtn.Visibility = Visibility.Visible;
                this.deleteBtn.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.addBtn.Visibility = Visibility.Collapsed;
                this.deleteBtn.Visibility = Visibility.Visible;
            }
        }
    }
}
