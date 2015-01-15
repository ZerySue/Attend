/*************************************************************************
** 文件名:   ListStackPanel.cs
** 主要类:   ListStackPanel
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   wz
** 日  期:   2013-12-30
** 修改人:   
** 日  期:
** 描  述:   ListStackPanel类,用于显示多个分时间段的设备用途
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

namespace IriskingAttend.View.PeopleView
{
    public partial class ListStackPanel : UserControl
    {
        

        public ListStackPanel()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 初始化内容
        /// </summary>
        /// <param name="deviceInfo"></param>
        public void InitContent(DeviceInfo deviceInfo)
        {
            bool isAdd = true;
            if (deviceInfo == null || deviceInfo.dev_type_List == null)
            {
                ListStackPanelItem listStackPanelItem = new ListStackPanelItem();
                listStackPanelItem.InitContent("", "00:00:00", true);
                listStackPanelItem.DeleteItemEvent += new EventHandler(listStackPanelItem_DeleteItemEvent);
                listStackPanelItem.AddItemEvent += new EventHandler(listStackPanelItem_AddItemEvent);
                this.LayoutRoot.Children.Add(listStackPanelItem);
                return;
            }
            foreach (var item in deviceInfo.dev_type_List)
            {
                string[] strs = item.Trim().Split(' ');
                ListStackPanelItem listStackPanelItem = new ListStackPanelItem();
                listStackPanelItem.InitContent(strs[0].Trim(), strs[2].Trim(), isAdd);
                listStackPanelItem.DeleteItemEvent += new EventHandler(listStackPanelItem_DeleteItemEvent);
                listStackPanelItem.AddItemEvent += new EventHandler(listStackPanelItem_AddItemEvent);
                this.LayoutRoot.Children.Add(listStackPanelItem);
                if (isAdd)
                {
                    isAdd = false;
                }
            }

        }

        private void listStackPanelItem_AddItemEvent(object sender, EventArgs e)
        {
            ListStackPanelItem listStackPanelItem = new ListStackPanelItem();
            listStackPanelItem.InitContent("", "00:00:00", false);
            listStackPanelItem.DeleteItemEvent += new EventHandler(listStackPanelItem_DeleteItemEvent);
            listStackPanelItem.AddItemEvent += new EventHandler(listStackPanelItem_AddItemEvent);
            this.LayoutRoot.Children.Add(listStackPanelItem);
        }

        private void listStackPanelItem_DeleteItemEvent(object sender, EventArgs e)
        {
            this.LayoutRoot.Children.Remove((UIElement)sender);
        }

        /// <summary>
        /// 获取设备表
        /// </summary>
        /// <returns></returns>
        public List<DeviceInfo> GetContent(string devSn,string place)
        {
            List<DeviceInfo> deviceInfos = new List<DeviceInfo>();
            foreach (var item in this.LayoutRoot.Children)
            {
                ListStackPanelItem listStackPanelItem = item as ListStackPanelItem;
                DeviceInfo deviceInfo = listStackPanelItem.GetContent();
                deviceInfo.dev_sn = devSn;
                deviceInfo.place = place;
                deviceInfos.Add(deviceInfo);
            }
            return deviceInfos;
        }
    }
}
