﻿#pragma checksum "G:\IrisAttend\trunk\codes\CSharp\矿山考勤\03 Code\IriskingAttend\IriskingAttend\View\SystemView\DlgLogDetail.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "2A6C71BBBCE90F5D09CF790C4898BAE8"
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
    
    
    public partial class DlgLogDetail : System.Windows.Controls.ChildWindow {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.TextBox txtOperator;
        
        internal System.Windows.Controls.TextBox txtOperateTime;
        
        internal System.Windows.Controls.TextBox txtOperateContent;
        
        internal System.Windows.Controls.TextBox txtOperateIP;
        
        internal System.Windows.Controls.TextBox txtOperateResult;
        
        internal System.Windows.Controls.TextBox txtOperateDescrip;
        
        internal System.Windows.Controls.Button btnOk;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/IriskingAttend;component/View/SystemView/DlgLogDetail.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.txtOperator = ((System.Windows.Controls.TextBox)(this.FindName("txtOperator")));
            this.txtOperateTime = ((System.Windows.Controls.TextBox)(this.FindName("txtOperateTime")));
            this.txtOperateContent = ((System.Windows.Controls.TextBox)(this.FindName("txtOperateContent")));
            this.txtOperateIP = ((System.Windows.Controls.TextBox)(this.FindName("txtOperateIP")));
            this.txtOperateResult = ((System.Windows.Controls.TextBox)(this.FindName("txtOperateResult")));
            this.txtOperateDescrip = ((System.Windows.Controls.TextBox)(this.FindName("txtOperateDescrip")));
            this.btnOk = ((System.Windows.Controls.Button)(this.FindName("btnOk")));
        }
    }
}
