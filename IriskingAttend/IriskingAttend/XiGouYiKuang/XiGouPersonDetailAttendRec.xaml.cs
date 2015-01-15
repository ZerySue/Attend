/*************************************************************************
** 文件名:   XiGouPersonDetailAttendRec.cs
** 主要类:   XiGouPersonDetailAttendRec
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   cty
** 日  期:   2014-4-14
** 修改人:   
** 日  期:
** 描  述:   XiGouPersonDetailAttendRec，西沟一矿领导带班考勤表个人明细前台cs
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
using IriskingAttend.Web;

namespace IriskingAttend.XiGouYiKuang
{
    public partial class XiGouPersonDetailAttendRec : ChildWindow
    {
        public XiGouPersonDetailAttendRec(XiGouLeaderAttend la,DateTime beginTime,DateTime endTime,BaseViewModelCollection<XiGouLeaderAttend> model)
        {
            InitializeComponent();
            lblTitle.Content = la.ShiftPrincipal + la.ShiftPersonName + " " + beginTime.ToShortDateString() + "—" + endTime.ToShortDateString() + " 的带班出勤情况";
            dgPersonAttend.ItemsSource = model;
        }
    }
}

