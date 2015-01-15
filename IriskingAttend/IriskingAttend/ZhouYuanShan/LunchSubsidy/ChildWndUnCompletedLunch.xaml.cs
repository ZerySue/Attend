/*************************************************************************
** 文件名:   ChildWndUnCompletedLunch.cs
×× 主要类:   ChildWndUnCompletedLunch
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   wz
** 日  期:   2013-12-19
** 修改人:   
** 日  期:   
** 描  述:   ChildWndUnCompletedLunch类，未完成班中餐子界面
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
using IriskingAttend.Web.ZhouYuanShan;
using System.Text;

namespace IriskingAttend.ZhouYuanShan.LunchSubsidy
{
    public partial class ChildWndUnCompletedLunch : ChildWindow
    {
        private SelectUncompletedPersonUI _selectPerson;
        private ReportRecordInfoOnDepart_ZhouYuanShan _reportRecordInfo;
        private List<ReportRecordInfoOnDepart_ZhouYuanShan> _compareRecordInfo;

        public ChildWndUnCompletedLunch(List<ReportRecordInfoOnDepart_ZhouYuanShan> compareRecordInfo)
        {
            InitializeComponent();
            this._compareRecordInfo = compareRecordInfo;
        }

        /// <summary>
        /// 初始化班中餐记录的基本信息
        /// </summary>
        /// <param name="reportRecordInfo"></param>
        public void InitBaseUI(ReportRecordInfoOnDepart_ZhouYuanShan reportRecordInfo)
        {
            
            _reportRecordInfo = reportRecordInfo;
            //日期
            textAttendDay.Text = _reportRecordInfo.attend_day.ToString("yyyy-MM-dd");
            //班次
            textClassOrder.Text = _reportRecordInfo.attend_sign;
            textClassOrderName.Text = _reportRecordInfo.class_order_name;
            //部门
            textDepart.Text = _reportRecordInfo.depart_name;
            //当班考勤人员
            if (_reportRecordInfo.attend_person_names != null)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var item in _reportRecordInfo.attend_person_names)
                {
                    sb.Append(item + "、");
                }
                if (sb.Length > 0)
                {
                    sb.Remove(sb.Length - 1, 1);
                }
                textAttendPersons.Text = sb.ToString();
            }
        }

        /// <summary>
        /// 选择差异人员UI
        /// 未完成班中餐子页面
        /// 在InitBaseUI之后调用
        /// </summary>
        /// <param name="departs"></param>
        /// <param name="persons"></param>
        public void InitUnCompletedUI(List<UserDepartInfo> departs, List<UserPersonInfo> persons)
        {
            this.OKButton.Visibility = Visibility.Visible;
            //差异人员
            _selectPerson = new SelectUncompletedPersonUI(_compareRecordInfo);
            _selectPerson.LoadDeparts(departs);
            _selectPerson.LoadContent(persons);
            _selectPerson.SelectDepart(_reportRecordInfo.depart_id);
            //过滤当前在岗人员
            _selectPerson.SetUnSelectedPersons( _reportRecordInfo.attend_person_ids);
            //选择当前已经存在的差异人员
            _selectPerson.SetSelectedPersons(_reportRecordInfo.diff_person_ids);

            gridSelectDiffPerson.Children.Add(_selectPerson);
            txtDiffPersonDiscrp.Text = "选择差异人员";

            //TabItem tabItemPerson = new TabItem();
            //tabItemPerson.Content = _selectPerson;
            //tabItemPerson.Header = " 选择差异人员";

            //this.tabControl.Items.Add(tabItemPerson);
        }

        /// <summary>
        /// 已完成班中餐子页面
        /// 查看差异人员UI
        /// 在InitBaseUI之后调用
        /// </summary>
        public void InitCompletedUI()
        {
            this.OKButton.Visibility = Visibility.Collapsed;

            //差异人员
            CompletedDiffPersonUI diffPersonUI = new CompletedDiffPersonUI();
            diffPersonUI.Init(_reportRecordInfo.diff_person_names);

            gridSelectDiffPerson.Children.Add(diffPersonUI);
            txtDiffPersonDiscrp.Text = "查看差异人员";
            //TabItem tabItemPerson = new TabItem();
            //tabItemPerson.Content = diffPersonUI;
            //tabItemPerson.Header = " 查看差异人员";
            //this.tabControl.Items.Add(tabItemPerson);
        }


        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            //修改差异人员
            List<string> diff_person_names = new List<string>();
            List<int> diff_person_ids = new List<int>();
            foreach (var item in _selectPerson.selectedList.Items)
            {
                diff_person_names.Add(((UserPersonInfo)item).person_name);
                diff_person_ids.Add(((UserPersonInfo)item).person_id);
            }
            _reportRecordInfo.diff_person_names = diff_person_names.ToArray();
            _reportRecordInfo.diff_person_ids = diff_person_ids.ToArray();
            _reportRecordInfo.reported_count = _reportRecordInfo.attend_person_count +  diff_person_names.Count;
            

            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}

