﻿#pragma checksum "G:\IrisAttend\trunk\codes\CSharp\矿山考勤\03 Code\IriskingAttend\IriskingAttend\XinJuLong\AttendDepartInWellCollect.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "9525C38BC9DBD269A22F084C8CE95FC1"
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


namespace IriskingAttend.XinJuLong {
    
    
    public partial class AttendDepartInWellCollect : System.Windows.Controls.Page {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.DatePicker dtpBegin;
        
        internal System.Windows.Controls.TimePicker timeBegin;
        
        internal System.Windows.Controls.DatePicker dtpEnd;
        
        internal System.Windows.Controls.TimePicker timeEnd;
        
        internal System.Windows.Controls.TextBox txtDepart;
        
        internal System.Windows.Controls.Button btnSelectDepart;
        
        internal System.Windows.Controls.Button btnQuery;
        
        internal System.Windows.Controls.DataGrid dgDepartAttend;
        
        internal System.Windows.Controls.Button btnExportExcel;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/IriskingAttend;component/XinJuLong/AttendDepartInWellCollect.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.dtpBegin = ((System.Windows.Controls.DatePicker)(this.FindName("dtpBegin")));
            this.timeBegin = ((System.Windows.Controls.TimePicker)(this.FindName("timeBegin")));
            this.dtpEnd = ((System.Windows.Controls.DatePicker)(this.FindName("dtpEnd")));
            this.timeEnd = ((System.Windows.Controls.TimePicker)(this.FindName("timeEnd")));
            this.txtDepart = ((System.Windows.Controls.TextBox)(this.FindName("txtDepart")));
            this.btnSelectDepart = ((System.Windows.Controls.Button)(this.FindName("btnSelectDepart")));
            this.btnQuery = ((System.Windows.Controls.Button)(this.FindName("btnQuery")));
            this.dgDepartAttend = ((System.Windows.Controls.DataGrid)(this.FindName("dgDepartAttend")));
            this.btnExportExcel = ((System.Windows.Controls.Button)(this.FindName("btnExportExcel")));
        }
    }
}

