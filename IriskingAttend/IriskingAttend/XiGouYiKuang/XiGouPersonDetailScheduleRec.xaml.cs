/*************************************************************************
** 文件名:   XiGouPersonDetailScheduleRec.cs
** 主要类:   XiGouPersonDetailScheduleRec
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   cty
** 日  期:   2014-4-19
** 修改人:   
** 日  期:
** 描  述:   XiGouPersonDetailScheduleRec，西沟一矿领导带班考勤表前台cs
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
using IriskingAttend.Web;
using IriskingAttend.ViewModel;
using IriskingAttend.NewWuHuShan;
using Irisking.Web.DataModel;
using IriskingAttend.Dialog;

namespace IriskingAttend.XiGouYiKuang
{
    public partial class XiGouPersonDetailScheduleRec : ChildWindow
    {
        //私有变量
        VmXiGouYiKuang _vm = new VmXiGouYiKuang();
        /// <summary>
        /// 选中的部门
        /// </summary>
        private List<UserDepartInfo> _departSelect = new List<UserDepartInfo>();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="model"></param>
        public XiGouPersonDetailScheduleRec(string name, DateTime beginTime, DateTime endTime, BaseViewModelCollection<XiGouLeaderAttend> model)
        {         
            InitializeComponent();
            _vm.PersonLeaderScheduleList = model;

            dtpBegin.Text = beginTime.ToShortDateString();
            dtpEnd.Text = endTime.ToShortDateString();
            txtName.Text = name;

            dgPersonSchedule.ItemsSource = _vm.PersonLeaderScheduleList;
        }

        //protected override Size ArrangeOverride(Size finalSize)
        //{
        //    Grid grid = Utils.FindChild<Grid>(this, "LayoutRoot");

        //    if (grid != null)
        //    {
        //        grid.Arrange(new Rect(0, 0, grid.DesiredSize.Width, grid.DesiredSize.Height));
        //        return finalSize;
        //    }

        //    return base.ArrangeOverride(finalSize);
        //}


      
        #region 选取部门

        private void btnSelectDepart_Click(object sender, RoutedEventArgs e)
        {
            SelectDepart sd = new SelectDepart(_departSelect, ChildWnd_SelectDepart_CallBack);
            sd.HorizontalAlignment = HorizontalAlignment.Left;
            sd.VerticalAlignment = VerticalAlignment.Top;
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

        /// <summary>
        /// 查询按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQuery_Click(object sender, RoutedEventArgs e)
        {
            #region 判断查询时间
            if (dtpBegin.Text == null || dtpBegin.Text == "")
            {
                MsgBoxWindow.MsgBox(
                            "查询开始时间不能为空！",
                            Dialog.MsgBoxWindow.MsgIcon.Information,
                            Dialog.MsgBoxWindow.MsgBtns.OK);
                return;
            }
            DateTime dataBegin = dtpBegin.SelectedDate.Value.Date;
            DateTime dataEnd;

            if (dtpEnd.Text == null || dtpEnd.Text == "")
            {
                dataEnd = dataBegin.AddMonths(1);
            }
            else
            {
                if (dataBegin >= dtpEnd.SelectedDate.Value.Date.AddDays(1))
                {
                    MsgBoxWindow.MsgBox(
                                "请确定您的开始时间早于截止时间！",
                                Dialog.MsgBoxWindow.MsgIcon.Information,
                                Dialog.MsgBoxWindow.MsgBtns.OK);
                    return;
                }
                else if (dataBegin.AddMonths(1) < dtpEnd.SelectedDate.Value.Date.AddDays(1))
                {
                    MsgBoxWindow.MsgBox(
                                 "请确定您的查询跨度时间为一个月！",
                                  Dialog.MsgBoxWindow.MsgIcon.Information,
                                  Dialog.MsgBoxWindow.MsgBtns.OK);
                    return;
                }
                else
                {
                    dataEnd = dtpEnd.SelectedDate.Value.Date.AddDays(1);
                }
            }
            #endregion
            //获取选择的部门
            List<int> departId = new List<int>();
            if (_departSelect.Count > 0)
            {
                foreach (UserDepartInfo ar in _departSelect)
                {
                    departId.Add(ar.depart_id);
                }
            }
            _vm.GetXiGouPersonLeaderSchedule(dataBegin, dataEnd, departId.ToArray(), txtName.Text.Trim(), -1);
        }
    }
}

