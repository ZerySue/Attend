/*************************************************************************
** 文件名:   DepartDetailReport.cs
** 主要类:   DepartDetailReport
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   wz
** 日  期:   2013-1-9
** 修改人:   
** 日  期:
** 描  述:   周源山矿部门月统计表
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
using System.Windows.Navigation;
using IriskingAttend.Dialog;
using IriskingAttend.ViewModel;
using System.Windows.Browser;
using IriskingAttend.Web.ZhouYuanShan;
using System.IO.IsolatedStorage;
using Irisking.Web.DataModel;
using IriskingAttend.NewWuHuShan;

namespace IriskingAttend.XinJuLong
{
    public partial class ReportXinJuLongPersonMonth : Page
    {
        #region 私有变量

        /// <summary>
        /// 选中的部门
        /// </summary>
        private List<UserDepartInfo> _departSelect = new List<UserDepartInfo>();

        VmReportXinJuLongPersonMonth _vmReport = new VmReportXinJuLongPersonMonth();

        private DateTime beginTime = new DateTime();       

        #endregion
        

        public ReportXinJuLongPersonMonth()
        {
            InitializeComponent();

            this.DataContext = _vmReport;           
           
            //表数据源
            this.dgDepartDetail.ItemsSource = _vmReport.PersonMonthAttendModel;
            
        }

        #region 点击查询 获取查询条件

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQuery_Click(object sender, RoutedEventArgs e)
        {
            #region 判断查询时间
            if (dtpBegin.SelectedDate == null)
            {
                MsgBoxWindow.MsgBox(
                            "查询开始时间不能为空！",
                            Dialog.MsgBoxWindow.MsgIcon.Information,
                            Dialog.MsgBoxWindow.MsgBtns.OK);
                return;
            }
            beginTime = dtpBegin.SelectedDate.Value.Date;
            
            #endregion

            if (_departSelect.Count() > 0)
            {
                _vmReport.GetReportPersonMonthAttend(beginTime, _departSelect, txtPersonName.Text.Trim(), txtWorkSn.Text.Trim());
            }
            else
            {
                List<UserDepartInfo> depart = new List<UserDepartInfo>();

                VmXlsFilter vmXlsFilter = new VmXlsFilter();
                vmXlsFilter.GetDepartmentByPrivilege();

                //部门加载完成
                vmXlsFilter.DepartLoadCompleted += delegate
                {
                    if (vmXlsFilter.DepartInfoModel.Count > 0)
                    {
                        //去部门列表中“全部”一项
                        vmXlsFilter.DepartInfoModel.RemoveAt(0);

                        foreach (UserDepartInfo item in vmXlsFilter.DepartInfoModel)
                        {
                            depart.Add(item);
                        }

                        _vmReport.GetReportPersonMonthAttend(beginTime, depart, txtPersonName.Text.Trim(), txtWorkSn.Text.Trim());
                    }
                };
            }

        }

        #region 选取部门

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
        #endregion

        #endregion
        
        /// <summary>
        /// 导出数据到Excel中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            if (_vmReport.PersonMonthAttendModel.Count() < 1)
            {
                Dialog.MsgBoxWindow.MsgBox(
                            "请查询到数据后再导出Excel！",
                            Dialog.MsgBoxWindow.MsgIcon.Information,
                            Dialog.MsgBoxWindow.MsgBtns.OK);
                return;
            }

            var uri = new Uri(App.Current.Host.Source, string.Format("../XinJuLong/新巨龙月度考勤报表.aspx?beginTime={0}",beginTime));
            HtmlPage.PopupWindow(uri, "_blank", new HtmlPopupWindowOptions());
        }
    }
}
