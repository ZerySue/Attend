﻿#pragma checksum "G:\IrisAttend\trunk\codes\CSharp\矿山考勤\03 Code\IriskingAttend\IriskingAttend\View\AttendView\AttendRecord.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "E897CA4DB070C0E6AB7EF34CA4D68A91"
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


namespace IriskingAttend.View {
    
    
    public partial class AttendRecord : System.Windows.Controls.Page {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.Grid gSelectRegion;
        
        internal System.Windows.Controls.StackPanel spDateTime;
        
        internal System.Windows.Controls.StackPanel spDateTimeBegin;
        
        internal System.Windows.Controls.DatePicker dateBegin;
        
        internal System.Windows.Controls.Label label1;
        
        internal System.Windows.Controls.StackPanel spDateTimeEnd;
        
        internal System.Windows.Controls.DatePicker dateEnd;
        
        internal System.Windows.Controls.Label lbDepart;
        
        internal System.Windows.Controls.ComboBox combDepart;
        
        internal System.Windows.Controls.Label lblName;
        
        internal System.Windows.Controls.TextBox txtBoxName;
        
        internal System.Windows.Controls.Label lblWorkSn;
        
        internal System.Windows.Controls.TextBox tbWorkSN;
        
        internal System.Windows.Controls.Button btnQuery;
        
        internal System.Windows.Controls.DataGrid dgAttendRecAll;
        
        internal System.Windows.Controls.Button btnExportExl;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/IriskingAttend;component/View/AttendView/AttendRecord.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.gSelectRegion = ((System.Windows.Controls.Grid)(this.FindName("gSelectRegion")));
            this.spDateTime = ((System.Windows.Controls.StackPanel)(this.FindName("spDateTime")));
            this.spDateTimeBegin = ((System.Windows.Controls.StackPanel)(this.FindName("spDateTimeBegin")));
            this.dateBegin = ((System.Windows.Controls.DatePicker)(this.FindName("dateBegin")));
            this.label1 = ((System.Windows.Controls.Label)(this.FindName("label1")));
            this.spDateTimeEnd = ((System.Windows.Controls.StackPanel)(this.FindName("spDateTimeEnd")));
            this.dateEnd = ((System.Windows.Controls.DatePicker)(this.FindName("dateEnd")));
            this.lbDepart = ((System.Windows.Controls.Label)(this.FindName("lbDepart")));
            this.combDepart = ((System.Windows.Controls.ComboBox)(this.FindName("combDepart")));
            this.lblName = ((System.Windows.Controls.Label)(this.FindName("lblName")));
            this.txtBoxName = ((System.Windows.Controls.TextBox)(this.FindName("txtBoxName")));
            this.lblWorkSn = ((System.Windows.Controls.Label)(this.FindName("lblWorkSn")));
            this.tbWorkSN = ((System.Windows.Controls.TextBox)(this.FindName("tbWorkSN")));
            this.btnQuery = ((System.Windows.Controls.Button)(this.FindName("btnQuery")));
            this.dgAttendRecAll = ((System.Windows.Controls.DataGrid)(this.FindName("dgAttendRecAll")));
            this.btnExportExl = ((System.Windows.Controls.Button)(this.FindName("btnExportExl")));
        }
    }
}

