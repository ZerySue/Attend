﻿#pragma checksum "G:\IrisAttend\trunk\codes\CSharp\矿山考勤\03 Code\IriskingAttend\IriskingAttend\XiGouYiKuang\XiGouPersonDetailScheduleRec.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "FD97DB882515B33F29ECD0579FE81C9E"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.225
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

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


namespace IriskingAttend.XiGouYiKuang {
    
    
    public partial class XiGouPersonDetailScheduleRec : System.Windows.Controls.ChildWindow {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.DatePicker dtpBegin;
        
        internal System.Windows.Controls.DatePicker dtpEnd;
        
        internal System.Windows.Controls.TextBox txtName;
        
        internal System.Windows.Controls.TextBox txtDepart;
        
        internal System.Windows.Controls.Button btnSelectDepart;
        
        internal System.Windows.Controls.Button btnQuery;
        
        internal System.Windows.Controls.DataGrid dgPersonSchedule;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/IriskingAttend;component/XiGouYiKuang/XiGouPersonDetailScheduleRec.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.dtpBegin = ((System.Windows.Controls.DatePicker)(this.FindName("dtpBegin")));
            this.dtpEnd = ((System.Windows.Controls.DatePicker)(this.FindName("dtpEnd")));
            this.txtName = ((System.Windows.Controls.TextBox)(this.FindName("txtName")));
            this.txtDepart = ((System.Windows.Controls.TextBox)(this.FindName("txtDepart")));
            this.btnSelectDepart = ((System.Windows.Controls.Button)(this.FindName("btnSelectDepart")));
            this.btnQuery = ((System.Windows.Controls.Button)(this.FindName("btnQuery")));
            this.dgPersonSchedule = ((System.Windows.Controls.DataGrid)(this.FindName("dgPersonSchedule")));
        }
    }
}

