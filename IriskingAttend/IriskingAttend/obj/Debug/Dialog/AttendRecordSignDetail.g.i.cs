﻿#pragma checksum "G:\IrisAttend\trunk\codes\CSharp\矿山考勤\03 Code\IriskingAttend\IriskingAttend\Dialog\AttendRecordSignDetail.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "6CFA021DA1AEF0A7FC72E290DB1F5569"
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


namespace IriskingAttend.Dialog {
    
    
    public partial class AttendRecordSignDetail : System.Windows.Controls.ChildWindow {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.Label lbName;
        
        internal System.Windows.Controls.Label lbWorkSN;
        
        internal System.Windows.Controls.DataGrid dgAttendRec;
        
        internal System.Windows.Controls.GridSplitter gridSplitter;
        
        internal System.Windows.Controls.DataGrid dgRecog;
        
        internal System.Windows.Controls.Button CancelButton;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/IriskingAttend;component/Dialog/AttendRecordSignDetail.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.lbName = ((System.Windows.Controls.Label)(this.FindName("lbName")));
            this.lbWorkSN = ((System.Windows.Controls.Label)(this.FindName("lbWorkSN")));
            this.dgAttendRec = ((System.Windows.Controls.DataGrid)(this.FindName("dgAttendRec")));
            this.gridSplitter = ((System.Windows.Controls.GridSplitter)(this.FindName("gridSplitter")));
            this.dgRecog = ((System.Windows.Controls.DataGrid)(this.FindName("dgRecog")));
            this.CancelButton = ((System.Windows.Controls.Button)(this.FindName("CancelButton")));
        }
    }
}

