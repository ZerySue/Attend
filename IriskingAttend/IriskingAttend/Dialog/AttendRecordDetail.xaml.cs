/*************************************************************************
** 文件名:   AttendRecordDetail.cs
×× 主要类:   AttendRecordDetail
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   lzc
** 日  期:   2013-4-8
** 修改人:   
** 日  期:
** 描  述:   AttendRecordDetail类，考勤记录详细查询界面
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
using IriskingAttend.ViewModel;
using Irisking.Web.DataModel;
using IriskingAttend.Common;
using IriskingAttend.ApplicationType;

namespace IriskingAttend.Dialog
{
    public partial class AttendRecordDetail : ChildWindow
    {
        #region 私有变量

        /// <summary>
        /// 考勤数据模型
        /// </summary>
        private VmAttend _vmAttend = null;

        /// <summary>
        /// 识别记录数据模型
        /// </summary>
        private VmRecogInfo _vmRecogInfo = new VmRecogInfo();

        /// <summary>
        /// 设置GetPersonRecogByAttend函数只执行一次
        /// </summary>
        private bool _tag = true;

        #endregion

        #region 共有变量
        /// <summary>
        /// 是否对识别记录进行修改过
        /// </summary>
        public bool isChanged = false;
        #endregion

        #region  构造函数
        public AttendRecordDetail(VmAttend vmAttend)
        {
            InitializeComponent();

            _vmAttend = vmAttend;
            SetPageBind();
            SetPageSequence();
        }

        /// <summary>
        /// 设置绑定
        /// </summary>
        private void SetPageBind()
        {
            ///取消人工补加列
            dgRecog.Columns.Where(a => a.Header.ToString() == "识别类型").First().Visibility = System.Windows.Visibility.Collapsed;  
 
            dgAttendRec.ItemsSource = _vmAttend.AttendRecDetailModel;
            //考勤详细信息加载完成，之后加载识别记录信息
            _vmAttend.AttendDetialRecLoadCompleted += (o, e) =>
            {
                if (_tag)
                {
                    _vmRecogInfo.GetPersonRecogByAttend(_vmAttend.SelectAttendRec.person_id);
                    _tag = false;
                }
            };

            ///设置识别记录绑定
            dgRecog.DataContext = _vmRecogInfo;
            dgRecog.ItemsSource = _vmRecogInfo.RecogInfoModel;
            _vmRecogInfo.RecogUpadateCompleted += (o, e) =>
            {
                _vmAttend.GetAttendRecDetail();
                //识别记录改变
                isChanged = true;
            };
            lbName.Content = _vmAttend.SelectAttendRec.person_name;
            lbWorkSN.Content = _vmAttend.SelectAttendRec.work_sn;
        }

        /// <summary>
        /// 设置序列号
        /// </summary>
        private void SetPageSequence()
        {

            dgAttendRec.LoadingRow += (a, e) =>
            {
                int index = e.Row.GetIndex();
                ///通过转换e.Row.DataContext 获取当前行显示颜色
                SolidColorBrush brush = new SolidColorBrush(
                    mathFun.ReturnColorFromString(((UserAttendRecDetail)e.Row.DataContext).leave_type_name_color));
                var cells = dgAttendRec.Columns[1].GetCellContent(e.Row) as TextBlock;
                //当前行显示颜色
                cells.Foreground = brush;
                if (((UserAttendRecDetail)e.Row.DataContext).DayType == 1 && AppTypePublic.GetCustomAppType().GetType().ToString().CompareTo("IriskingAttend.ApplicationType.ZhongKeHongBaApp") == 0)
                {
                    e.Row.Background = new SolidColorBrush(mathFun.ReturnColorFromString("0xFFF5F5DC"));
                }
                else
                {
                    e.Row.Background = new SolidColorBrush(mathFun.ReturnColorFromString("0xFFFFFFFF"));
                }
                
                var cell = dgAttendRec.Columns[0].GetCellContent(e.Row) as TextBlock;
                cell.Text = (index + 1).ToString();
            };

            dgRecog.LoadingRow += (a, e) =>
            {
                int index = e.Row.GetIndex();
                ///通过转换e.Row.DataContext 获取当前行显示颜色
                SolidColorBrush brush = new SolidColorBrush(
                    mathFun.ReturnColorFromString(((UserPersonRecogLog)e.Row.DataContext).recog_type_color));

                //当前行显示颜色
                e.Row.Foreground = brush;
                if (((UserPersonRecogLog)e.Row.DataContext).DayType == 1 && AppTypePublic.GetCustomAppType().GetType().ToString().CompareTo("IriskingAttend.ApplicationType.ZhongKeHongBaApp") == 0)
                {
                    e.Row.Background = new SolidColorBrush(mathFun.ReturnColorFromString("0xFFF5F5DC"));
                }
                else
                {
                    e.Row.Background = new SolidColorBrush(mathFun.ReturnColorFromString("0xFFFFFFFF"));
                }
                var cell = dgRecog.Columns[0].GetCellContent(e.Row) as TextBlock;
                cell.Text = (index + 1).ToString();
            };

            //是否显示识别类型
            _vmRecogInfo.IsShowTypeCompleted += (a, e) =>
            {
                if (_vmRecogInfo.IsShowRecogType == 0)
                {
                    ///取消人工补加列
                    dgRecog.Columns.Where(col => col.Header.ToString() == "识别类型").First().Visibility = System.Windows.Visibility.Visible;
                }
            };

            //added by gqy at 2014-02-13 周源山需求
            if (!VmLogin.DictPrivilege[AbstractApp.PrivilegeENUM.AttendIdentifyRecord])
            {
                dgRecog.Columns.Where(col => col.Header.ToString() == "操  作").First().Visibility = Visibility.Collapsed;
            }
            //add by yht 2014-09-29 中科红霸考勤显示备注
            //if (AppTypePublic.GetCustomAppType().GetType().ToString().CompareTo("IriskingAttend.ApplicationType.ZhongKeHongBaApp") == 0)
            //{
            //    dgRecog.Columns.Where(col => col.Header.ToString() == "备注").First().Visibility = Visibility.Visible;
            //}
        }
        #endregion

        #region 响应事件函数

        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        /// <summary>
        /// 重构识别记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRebuild_Click(object sender, RoutedEventArgs e)
        {
            _vmRecogInfo.RebuildPersonRecog((UserPersonRecogLog)(dgRecog.SelectedItem));
        }

        /// <summary>
        /// 删除识别记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDel_Click(object sender, RoutedEventArgs e)
        {
            _vmRecogInfo.DelPersonRecog((UserPersonRecogLog)(dgRecog.SelectedItem));
        }

        /// <summary>
        /// 添加识别记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddRecorg_Click(object sender, RoutedEventArgs e)
        {
            //中科红霸 yht 2014-10-08
            if (AppTypePublic.GetCustomAppType().GetType().ToString().CompareTo("IriskingAttend.ApplicationType.ZhongKeHongBaApp") == 0)
            {
                _vmRecogInfo.ShowZKHBAddRecogDialog((UserPersonRecogLog)(dgRecog.SelectedItem));
            }
            else
            {
                _vmRecogInfo.ShowAddRecogDialog((UserPersonRecogLog)(dgRecog.SelectedItem));
            }
        }
        #endregion
    }
}

