﻿#pragma checksum "G:\IrisAttend\trunk\codes\CSharp\矿山考勤\03 Code\IriskingAttend\IriskingAttend\ZhouYuanShan\ChildWndReportQuery.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "34A9A5B80DBC84DA4B891D9BA81FDBE7"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.225
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using IriskingAttend.CustomUI;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace IriskingAttend.ZhouYuanShan {
    
    
    public partial class ChildWndReportQuery : System.Windows.Controls.ChildWindow {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.Button CancelButton;
        
        internal System.Windows.Controls.Button OKButton;
        
        internal System.Windows.Controls.DatePicker dateBegin;
        
        internal System.Windows.Controls.DatePicker dateEnd;
        
        internal System.Windows.Controls.HyperlinkButton hBtnSelectObj;
        
        internal System.Windows.Controls.TextBox textSelectObj;
        
        internal System.Windows.Controls.CheckBox checkBoxClassOrder;
        
        internal System.Windows.Controls.CheckBox checkBoxDuration;
        
        internal System.Windows.Controls.CheckBox checkBoxTime;
        
        internal System.Windows.Controls.RadioButton radioBtnMonthlyReportOnPerson;
        
        internal System.Windows.Controls.RadioButton radioBtnDailyReportOnPerson;
        
        internal System.Windows.Controls.RadioButton radioButtonDetailReportOnDepart;
        
        internal System.Windows.Controls.RadioButton radioButtonMonthlyReportOnDepart;
        
        internal IriskingAttend.CustomUI.TextComboBox textCmbClassOrder;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/IriskingAttend;component/ZhouYuanShan/ChildWndReportQuery.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.CancelButton = ((System.Windows.Controls.Button)(this.FindName("CancelButton")));
            this.OKButton = ((System.Windows.Controls.Button)(this.FindName("OKButton")));
            this.dateBegin = ((System.Windows.Controls.DatePicker)(this.FindName("dateBegin")));
            this.dateEnd = ((System.Windows.Controls.DatePicker)(this.FindName("dateEnd")));
            this.hBtnSelectObj = ((System.Windows.Controls.HyperlinkButton)(this.FindName("hBtnSelectObj")));
            this.textSelectObj = ((System.Windows.Controls.TextBox)(this.FindName("textSelectObj")));
            this.checkBoxClassOrder = ((System.Windows.Controls.CheckBox)(this.FindName("checkBoxClassOrder")));
            this.checkBoxDuration = ((System.Windows.Controls.CheckBox)(this.FindName("checkBoxDuration")));
            this.checkBoxTime = ((System.Windows.Controls.CheckBox)(this.FindName("checkBoxTime")));
            this.radioBtnMonthlyReportOnPerson = ((System.Windows.Controls.RadioButton)(this.FindName("radioBtnMonthlyReportOnPerson")));
            this.radioBtnDailyReportOnPerson = ((System.Windows.Controls.RadioButton)(this.FindName("radioBtnDailyReportOnPerson")));
            this.radioButtonDetailReportOnDepart = ((System.Windows.Controls.RadioButton)(this.FindName("radioButtonDetailReportOnDepart")));
            this.radioButtonMonthlyReportOnDepart = ((System.Windows.Controls.RadioButton)(this.FindName("radioButtonMonthlyReportOnDepart")));
            this.textCmbClassOrder = ((IriskingAttend.CustomUI.TextComboBox)(this.FindName("textCmbClassOrder")));
        }
    }
}

