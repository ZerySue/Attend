﻿#pragma checksum "G:\IrisAttend\trunk\codes\CSharp\矿山考勤\03 Code\IriskingAttend\IriskingAttend\Dialog\LeaveTypeManage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "582C3D93270871E166EB2872D83155F7"
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
    
    
    public partial class LeaveTypeManage : System.Windows.Controls.ChildWindow {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.Label label1;
        
        internal System.Windows.Controls.Label label2;
        
        internal System.Windows.Controls.Label label5;
        
        internal System.Windows.Controls.TextBox txtLeaveTypeName;
        
        internal System.Windows.Controls.TextBox txtAttendSign;
        
        internal System.Windows.Controls.TextBox txtMemo;
        
        internal System.Windows.Controls.Button btnBatchAdd;
        
        internal System.Windows.Controls.Button btnOk;
        
        internal System.Windows.Controls.Button btnCancel;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/IriskingAttend;component/Dialog/LeaveTypeManage.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.label1 = ((System.Windows.Controls.Label)(this.FindName("label1")));
            this.label2 = ((System.Windows.Controls.Label)(this.FindName("label2")));
            this.label5 = ((System.Windows.Controls.Label)(this.FindName("label5")));
            this.txtLeaveTypeName = ((System.Windows.Controls.TextBox)(this.FindName("txtLeaveTypeName")));
            this.txtAttendSign = ((System.Windows.Controls.TextBox)(this.FindName("txtAttendSign")));
            this.txtMemo = ((System.Windows.Controls.TextBox)(this.FindName("txtMemo")));
            this.btnBatchAdd = ((System.Windows.Controls.Button)(this.FindName("btnBatchAdd")));
            this.btnOk = ((System.Windows.Controls.Button)(this.FindName("btnOk")));
            this.btnCancel = ((System.Windows.Controls.Button)(this.FindName("btnCancel")));
        }
    }
}
