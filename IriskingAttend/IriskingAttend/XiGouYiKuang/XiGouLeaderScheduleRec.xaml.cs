/*************************************************************************
** 文件名:   XiGouLeaderScheduleRec.cs
** 主要类:   XiGouLeaderScheduleRec
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   cty
** 日  期:   2014-4-18
** 修改人:   
** 日  期:
** 描  述:   XiGouLeaderScheduleRec，西沟一矿领导带班考勤表前台cs
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
using System.Windows.Navigation;
using IriskingAttend.Dialog;
using IriskingAttend.NewWuHuShan;
using Irisking.Web.DataModel;
using IriskingAttend.ViewModel;
using System.IO.IsolatedStorage;
using IriskingAttend.Web;

namespace IriskingAttend.XiGouYiKuang
{
    public partial class XiGouLeaderScheduleRec : Page
    {
        /// <summary>
        /// VM层
        /// </summary>
        VmXiGouYiKuang _vm = new VmXiGouYiKuang();

        /// <summary>
        /// 本地存储
        /// </summary>
        private IsolatedStorageSettings _querySetting = IsolatedStorageSettings.ApplicationSettings;

        /// <summary>
        /// 选中的部门
        /// </summary>
        private List<UserDepartInfo> _departSelect = new List<UserDepartInfo>();

        public XiGouLeaderScheduleRec()
        {
            InitializeComponent();
            dtpBegin.Text = DateTime.Now.ToShortDateString();
            _vm.GetXiGouLeaderSchedule(dtpBegin.SelectedDate.Value,dtpBegin.SelectedDate.Value.AddDays(1),null);
            dgLeaderSchedule.ItemsSource = _vm.LeaderScheduleList;
        }

        // 当用户导航到此页面时执行。
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void btnQuery_Click(object sender, RoutedEventArgs e)
        {
            
            #region 判断查询时间
            if (dtpBegin.Text == null || dtpBegin.Text == "")
            {
                MsgBoxWindow.MsgBox(
                            "查询时间不能为空！",
                            Dialog.MsgBoxWindow.MsgIcon.Information,
                            Dialog.MsgBoxWindow.MsgBtns.OK);
                return;
            }
            #endregion
            DateTime dataBegin = dtpBegin.SelectedDate.Value.Date;
            DateTime dataEnd = dataBegin.AddDays(1);
            SetCondition(dataBegin, dataEnd);
            GetAttend();
        }
        /// <summary>
        /// 获取查询条件
        /// </summary>
        /// <param name="dataBegin"></param>
        /// <param name="dataEnd"></param>
        private void SetCondition(DateTime dataBegin, DateTime dataEnd)
        {
            XlsQueryCondition qc = new XlsQueryCondition();
            qc.XlsBeginTime = dataBegin;
            qc.XlsEndTime = dataEnd;

            //获取选择的部门
            if (_departSelect.Count > 0)
            {
                qc.XlsDepartIdLst = new int[_departSelect.Count];
                int num = 0;
                foreach (UserDepartInfo ar in _departSelect)
                {
                    qc.XlsDepartIdLst[num++] = ar.depart_id;
                }
            }

            ///如果存在 先删除该键值对
            if (_querySetting.Contains("attendConditon"))
            {
                _querySetting.Remove("attendConditon");
            }
            _querySetting.Add("attendConditon", qc);
        }     
        private void btnSelectDepart_Click(object sender, RoutedEventArgs e)
        {
            SelectDepart sd = new SelectDepart(_departSelect, ChildWnd_SelectDepart_CallBack);
            sd.Show();
        }
        /// <summary>
        /// 回调函数，得到选取的部门信息
        /// </summary>
        /// <param name="principalSelect"></param>
        private void ChildWnd_SelectDepart_CallBack(BaseViewModelCollection<UserDepartInfo> departSelect)
        {
            string txtShowDepartName = "";
            _departSelect.Clear();
            foreach (UserDepartInfo ar in departSelect)
            {
                if (ar.isSelected)
                {
                    _departSelect.Add(ar);
                    txtShowDepartName += ar.depart_name + ",";
                }
            }
            if (txtShowDepartName != "")
            {
                txtDepart.Text = txtShowDepartName.Remove(txtShowDepartName.LastIndexOf(","), 1);
            }
            else
            {
                txtDepart.Text = txtShowDepartName;
            }
        }


        #region 查询数据

        /// <summary>
        /// 考勤查询
        /// </summary>
        void GetAttend()
        {
            if (_querySetting.Contains("attendConditon"))
            {
                try
                {
                    XlsQueryCondition condition;
                    _querySetting.TryGetValue<XlsQueryCondition>("attendConditon", out condition);
                    _vm.GetXiGouLeaderSchedule(condition.XlsBeginTime, condition.XlsEndTime,null);
                }
                catch (Exception e)
                {
                    ErrorWindow err = new ErrorWindow(e);
                    err.Show();
                }
            }
            else
            {
                ChildWindow errorWin = new ErrorWindow("非法请求", "查询条件不准确！");
                errorWin.Show();
            }
        }

        #endregion        

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            XiGouLeaderSchedule obj = dgLeaderSchedule.SelectedItem as XiGouLeaderSchedule;
            int personId = 0;
            string name = ((HyperlinkButton)sender).Content.ToString();
            switch (((HyperlinkButton)sender).Name)
            {
                case "hlbtnNightPersonName": personId = obj.NightPersonId;
                    name = obj.NightPersonName;
                    break;
                case "hlbtnMorningPersonName": personId = obj.MornPersonId;
                    name = obj.MornPersonName;
                    break;
                case "hlbtnMidPersonName": personId = obj.MidPersonId;
                    name = obj.MidPersonName;
                    break;
            }
            DateTime beginTime = new DateTime(dtpBegin.SelectedDate.Value.Year, dtpBegin.SelectedDate.Value.Month, 1);
            DateTime endTime = beginTime.AddMonths(1).AddDays(-1);
            _vm.GetXiGouPersonLeaderSchedule(beginTime, endTime, null, "", personId);

            XiGouPersonDetailScheduleRec sd = new XiGouPersonDetailScheduleRec(name, beginTime, endTime, _vm.PersonLeaderScheduleList);
            sd.Show();
        }
    }
}
