/*************************************************************************
** 文件名:   ParameterConfig.cs
** 主要类:   ParameterConfig
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2013-6-14
** 修改人:   cty
** 日  期:   2013-8-4
 *修改内容：增加自动备份虹膜库的功能，自动备份部分的代码修改，代码优化
** 描  述:   ParameterConfig类,系统参数配置界面
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
using System.IO;
using IriskingAttend.Dialog;
using System.Windows.Data;
using System.Text.RegularExpressions;


namespace IriskingAttend.View.SystemView
{    
    public partial class ParameterConfig : Page
    {
        #region 变量初始化

        //vm变量
        private VmSystemParameter _vmParam = new VmSystemParameter();

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数，进行初始化
        /// </summary>
        public ParameterConfig()
        {            
            InitializeComponent();

            //判断是否为非矿，若为非矿，则超时时间设置部分不显示
            if (!VmLogin.GetIsMineApplication())
            {
                lblOverTime.Visibility = Visibility.Collapsed;
                txtOverTimeHour.Visibility = Visibility.Collapsed;
                lblOverTimeHour.Visibility = Visibility.Collapsed;
                txtOverTimeMinute.Visibility = Visibility.Collapsed;
                lblOverTimeMinute.Visibility = Visibility.Collapsed;
            }

            //此label涉及到换行，所以在代码中实现，而不是在界面中直接加载
            this.labNoteIrisApp.Content = "注意：此地址为服务端备份文件夹地址," + Environment.NewLine + "若输入错误,可能会发生不可预估的错误";
            this.labNoteIrisData.Content = "注意：此地址为服务端备份文件夹地址," + Environment.NewLine + "若输入错误,可能会发生不可预估的错误";
            
            //数据绑定
            this.DataContext = _vmParam;

            //记工策略精确度下拉框的选中项绑定
            Binding binding = new Binding("SelectAccuracy") 
            { 
                Mode = BindingMode.TwoWay 
            };
            cmbAccuracy.SetBinding(ComboBox.SelectedItemProperty, binding);

            //记工策略精确度下拉框的数据源绑定
            this.cmbAccuracy.ItemsSource = _vmParam.DictWorkCntPolicyAccuracy;
            cmbAccuracy.DisplayMemberPath = "Value";

            //记工策略精确度下拉框默认显示为第一个
            cmbAccuracy.SelectedIndex = 0;   
   
            //ria方式获得系统参数信息
            _vmParam.GetSystemParams();           
        }

        #endregion

        #region 事件响应

        /// <summary>
        /// 取消按钮，实际是刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            _vmParam.GetSystemParams(); 
        }
       
        /// <summary>
        /// 自动备份参数设置有变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbBackupCycle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //按月备份
            if (cmbBackupCycle.SelectedIndex == 0)
            {
                this.cmbBackupDate.Items.Clear();
                //将1-31添加到日期列表，按最大日期计算
                for (int index = 1; index < 32; index++)
                {
                    this.cmbBackupDate.Items.Add(index.ToString());
                }
                //默认选中每月1日
                this.cmbBackupDate.SelectedIndex = 0;
                return;
            }

            //按星期备份
            if (cmbBackupCycle.SelectedIndex == 1)
            {
                //将星期一到星期日添加到日期列表
                this.cmbBackupDate.Items.Clear();
                this.cmbBackupDate.Items.Add("星期一");
                this.cmbBackupDate.Items.Add("星期二");
                this.cmbBackupDate.Items.Add("星期三");
                this.cmbBackupDate.Items.Add("星期四");
                this.cmbBackupDate.Items.Add("星期五");
                this.cmbBackupDate.Items.Add("星期六");
                this.cmbBackupDate.Items.Add("星期日");

                //默认选中星期一
                this.cmbBackupDate.SelectedIndex = 0;
                return;
            }

            //按天备份
            if (cmbBackupCycle.SelectedIndex == 2)
            {
                this.cmbBackupDate.Items.Clear();
            }
        }

        /// <summary>
        /// 自动备份虹膜库参数设置有变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbBackupCycleIrisData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //按月备份
            if (cmbBackupCycleIrisData.SelectedIndex == 0)
            {
                this.cmbBackupDateIrisData.Items.Clear();
                //将1-31添加到日期列表，按最大日期计算
                for (int index = 1; index < 32; index++)
                {
                    this.cmbBackupDateIrisData.Items.Add(index.ToString());
                }
                //默认选中每月1日
                this.cmbBackupDateIrisData.SelectedIndex = 0;
                return;
            }

            //按星期备份
            if (cmbBackupCycleIrisData.SelectedIndex == 1)
            {
                //将星期一到星期日添加到日期列表
                this.cmbBackupDateIrisData.Items.Clear();
                this.cmbBackupDateIrisData.Items.Add("星期一");
                this.cmbBackupDateIrisData.Items.Add("星期二");
                this.cmbBackupDateIrisData.Items.Add("星期三");
                this.cmbBackupDateIrisData.Items.Add("星期四");
                this.cmbBackupDateIrisData.Items.Add("星期五");
                this.cmbBackupDateIrisData.Items.Add("星期六");
                this.cmbBackupDateIrisData.Items.Add("星期日");

                //默认选中星期一
                this.cmbBackupDateIrisData.SelectedIndex = 0;
                return;
            }

            //按天备份
            if (cmbBackupCycleIrisData.SelectedIndex == 2)
            {
                this.cmbBackupDateIrisData.Items.Clear();
            }
        }


        /// <summary>
        /// 提交系统参数设置。在提交之前，先判断系统参数输入是否合法，然后调用vm进行修改到数据库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            //超时时间只能为数字 by cty
            Regex reggx = new Regex(@"(^[1-9]\d*|0$)");
            if (!(reggx.IsMatch(txtOverTimeHour.Text.Trim()) && reggx.IsMatch(txtOverTimeMinute.Text.Trim())))
            {
                MsgBoxWindow.MsgBox("超时时间只能为数字！", MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);
                return;
            }
            if (int.Parse(txtOverTimeHour.Text.Trim()) > 500)//+ int.Parse(txtOverTimeMinute.Text.Trim())) > 32767)
            {
                MsgBoxWindow.MsgBox("请确定超时时间-小时数不大于500！", MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);
                return;
            }
            if (int.Parse(txtOverTimeMinute.Text.Trim()) > 59)
            {
                MsgBoxWindow.MsgBox("请确定超时时间-分钟数不大于59！", MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);
                return;
            }
            //重复识别间隔不能为空
            if (txtDupTime.Text.Equals(""))
            {
                MsgBoxWindow.MsgBox( "重复识别间隔不能为空！", MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);
                return;
            }

            //重复识别间隔只能为数字
            if (!PublicMethods.IsNumberString(txtDupTime.Text))
            {
                MsgBoxWindow.MsgBox( "重复识别间隔只能为数字！", MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);
                return;
            }

            try
            {

                //重复识别间隔在0与3000之间
                string dupTime = txtDupTime.Text.TrimStart('0');

                if ((dupTime == "") || (dupTime.Length >= 5) || (Int16.Parse(dupTime) > 3000) || (Int16.Parse(dupTime) <= 0))
                {
                    MsgBoxWindow.MsgBox("重复识别间隔不能小于等于0且不能大于3000！", MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);
                    return;
                }
            }
            catch (Exception ex)
            {
                ErrorWindow err = new ErrorWindow(ex);
                err.Show();
                return;
            }
            
            //调用vm，将修改后的参数提交修改到数据库
            _vmParam.SubmitSystemParam();
        }

        #endregion
    }
}
