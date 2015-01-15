/*************************************************************************
** 文件名:   DlgDeviceItemMng.cs
** 主要类:   DlgDeviceItemMng
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2013-7-22
** 修改人:   
** 日  期:
** 描  述:   DlgDeviceItemMng类,添加、修改设备窗口
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
using IriskingAttend.ViewModel.SystemViewModel;
using Irisking.Web.DataModel;
using System.Windows.Data;

namespace IriskingAttend.Dialog
{
    public partial class DlgDeviceItemMng : ChildWindow
    {
        #region 私有变量声明：当前选中行信息、父窗口的回调函数

        //当前操作设备信息
        private DeviceInfo _deviceInfoItem;

        //回调函数，父窗口传递
        private Action<bool?> _parentCallBack;

        //vm初始化
        VmDeviceItemMng _vmDevItemMng;

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="deviceInfoItem">欲进行操作的设备信息</param>
        /// <param name="parentCallBack">子窗口关闭后调用的父窗口回调函数</param>
        public DlgDeviceItemMng(DeviceInfo deviceInfoItem, Action<bool?> parentCallBack)
        {            
            InitializeComponent();

            _vmDevItemMng = new VmDeviceItemMng();

            //vm数据绑定
            this.DataContext = _vmDevItemMng;

            //vm内包含的委托的初始化
            _vmDevItemMng.ChangeDlgResult = ModifyDlgResult;

            //子窗口关闭时的事件
            this.Closed += new EventHandler(DlgDeviceItemMng_Closed);

            //父窗口的回调函数
            this._parentCallBack = parentCallBack;

            //子窗口中需要显示的设备信息
            this._deviceInfoItem = deviceInfoItem;

            //设备类型list初始化
            listStackPanelDevtype.InitContent(deviceInfoItem);
            _vmDevItemMng.InitDevTypes(listStackPanelDevtype);

            ////设备类型下拉框的选中项绑定
            //Binding binding = new Binding("SelectDevType") { Mode = BindingMode.TwoWay };
            //cmbDevtype.SetBinding(ComboBox.SelectedItemProperty, binding);

            ////设备类型下拉框的数据源绑定
            //this.cmbDevtype.ItemsSource = vmDevItemMng.DictDeviceType;
            //cmbDevtype.DisplayMemberPath = "Value";

            ////设备类型下拉框显示的设备类型
            //if (this._deviceInfoItem.dev_type < 0)
            //{
            //    //添加设备时，默认显示设备类型为下标0的设备类型
            //    cmbDevtype.SelectedIndex = 0;
            //}
            //else
            //{
            //    //修改设备时，显示设备修改前的设备类型
            //    if (vmDevItemMng.DictDeviceType.ContainsKey(this._deviceInfoItem.dev_type))
            //    {
            //        vmDevItemMng.SelectDevType = new KeyValuePair<int, string>(this._deviceInfoItem.dev_type, vmDevItemMng.DictDeviceType[this._deviceInfoItem.dev_type]);
            //    }
            //    else
            //    {
            //        //若设备类型下拉框中不包含设备修改前的设备类型，显示为空。例如：非矿的不包含矿山的入井、出井、出入井等。
            //        cmbDevtype.SelectedIndex = -1;
            //    }
            //}

            //将父窗口中需要修改的设备信息显示在此子窗口中，子窗口界面控件值初始化
            _vmDevItemMng.DevInfo = deviceInfoItem; 

        }

        public DlgDeviceItemMng(DeviceInfo[] deviceInfos, Action<bool?> parentCallBack):this((DeviceInfo)null, parentCallBack)
        {
            _vmDevItemMng.BatchModifyDevInfos = deviceInfos;
        }

        #endregion

        #region 回调函数

        /// <summary>
        /// vm层委托的回调函数 
        /// </summary>
        /// <param name="dialogResult">是否需要关闭此子窗口</param>
        private void ModifyDlgResult(bool dialogResult)
        {
            this.DialogResult = dialogResult;
        }

        #endregion

        #region 事件响应

        /// <summary>
        /// 取消添加按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;           
        }

        /// <summary>
        /// 子窗口关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DlgDeviceItemMng_Closed(object sender, EventArgs e)
        {
            //子窗口关闭时，调用父窗口传递进来的回调函数
            if (_parentCallBack != null)
            {
                _parentCallBack(this.DialogResult);
            }
        }

        #endregion
    }
}
