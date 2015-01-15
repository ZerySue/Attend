/*************************************************************************
** 文件名:   AddRecog.cs
×× 主要类:   AddRecog
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   yht
** 日  期:   2014-9-29
** 修改人:   yht
** 日  期:   2014-9-29
** 描  述:   AddRecog类，添加识别记录界面
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
using IriskingAttend.ViewModel.AttendViewModel;
using IriskingAttend.ViewModel.SystemViewModel;

namespace IriskingAttend.ZhongKeHongBa
{
    public partial class AddRecog : ChildWindow
    {
        /// <summary>
        /// 区分矿山与非矿山--按照设备类型进行区分
        /// </summary>
        public VmDeviceManage VmDevMng = new VmDeviceManage();

        //private string _operator_name;

        //public string Operator_name
        //{
        //    get { return _operator_name; }
        //    set { _operator_name = value; }
        //}

        /// <summary>
        /// 人员信息
        /// </summary>
        private UserPersonSimple _person;

        /// <summary>
        /// 获取人员信息
        /// </summary>
        public UserPersonSimple Person
        {
            get
            {
                return _person;
            }
        }

        /// <summary>
        /// 重构1，根据简单人员信息创建添加识别记录界面
        /// </summary>
        /// <param name="person"></param>
        public AddRecog(UserPersonSimple person)
        {
            InitializeComponent();
            _person = person;
            this.lbName.Content = person.person_name;
            
            InitCmbDevice();
        }

        /// <summary>
        /// 重构，根据人员姓名，人员Id设置添加识别记录
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        public AddRecog(string name,int id)
        {
            InitializeComponent();
            _person.person_name = name;
            _person.person_id = id;
            this.lbName.Content = name;
            ///设置界面显示矿与非矿绑定
            InitCmbDevice();
        }

        /// <summary>
        /// 重构2，多个人员添加同一条识别记录
        /// by wz
        /// </summary>
        /// <param name="persons"></param>
        public AddRecog()
        {
            InitializeComponent();
            this.lbName.Content = "...";
            ///设置界面显示矿与非矿绑定
            InitCmbDevice();
        }

        /// <summary>
        /// 设置绑定
        /// </summary>
        private void InitCmbDevice()
        {
            VmDevMng.GetDeviceInfoTableRia();

            //设备详细信息加载完成后加载设备信息
            VmDevMng.DeviceLoadCompleted += (o, e) =>
            {
                cmbDev.Items.Add("无");

                foreach (DeviceInfo dev in VmDevMng.SystemDeviceInfo)
                {
                    string devInfo = "";
                    if (dev.place != null)
                    {
                        devInfo += dev.place;
                        devInfo += " ";
                    }

                    devInfo += dev.dev_sn;
                    cmbDev.Items.Add(devInfo);
                }

                cmbDev.SelectedIndex = 0;

            };

            Dictionary<int, string> DictDeviceType = PublicMethods.GetDevTypeDictionary(VmLogin.GetIsMineApplication());

            foreach( KeyValuePair<int, string> devType in DictDeviceType )
            {
                cmbDevType.Items.Add(devType.Value);
            }
            cmbDevType.SelectedIndex = 0;
        }
        /// <summary>
        /// 取消  关闭对话框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void cmbDev_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbDevType.Items.Clear();

            Dictionary<int, string> DictDeviceType = PublicMethods.GetDevTypeDictionary(VmLogin.GetIsMineApplication());

            if (cmbDev.SelectedIndex == 0)
            {   
                foreach (KeyValuePair<int, string> devType in DictDeviceType)
                {
                    cmbDevType.Items.Add(devType.Value);
                }
                cmbDevType.SelectedIndex = 0;
                return;
            }

            List<string> dev_type_List = (List<string>)VmDevMng.SystemDeviceInfo[cmbDev.SelectedIndex - 1].dev_type_List;

            foreach (string devTypeString in dev_type_List)
            {
                if (devTypeString.Contains("上下班"))
                {
                    if (!cmbDevType.Items.Contains("上班"))
                    {
                        cmbDevType.Items.Add("上班");
                    }

                    if (!cmbDevType.Items.Contains("下班"))
                    {
                        cmbDevType.Items.Add("下班");
                    }

                    if (!cmbDevType.Items.Contains("上下班"))
                    {
                        cmbDevType.Items.Add("上下班");
                    }
                }
                else if (devTypeString.Contains("出入井"))
                {
                    if (!cmbDevType.Items.Contains("入井"))
                    {
                        cmbDevType.Items.Add("入井");
                    }

                    if (!cmbDevType.Items.Contains("出井"))
                    {
                        cmbDevType.Items.Add("出井");
                    }

                    if (!cmbDevType.Items.Contains("出入井"))
                    {
                        cmbDevType.Items.Add("出入井");
                    }
                }
                else
                {
                    string devType = devTypeString.Split(' ').First().Trim();

                    if (!cmbDevType.Items.Contains(devType))
                    {
                        cmbDevType.Items.Add(devType);
                    }
                }
            }
            cmbDevType.SelectedIndex = 0;
        }
    }
}

