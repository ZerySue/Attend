﻿#pragma checksum "G:\IrisAttend\trunk\codes\CSharp\矿山考勤\03 Code\IriskingAttend\IriskingAttend\View\SystemView\BackupConfig.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "CE0FE2A5019B3AF7FFFA83A7986ADEB8"
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


namespace IriskingAttend.View.SystemView {
    
    
    public partial class BackupConfig : System.Windows.Controls.Page {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.TextBox txtMannualBackupIrisApp;
        
        internal System.Windows.Controls.Label labIrisApp;
        
        internal System.Windows.Controls.Button btnSubmitIrisApp;
        
        internal System.Windows.Controls.TextBox txtMannualBackupIrisData;
        
        internal System.Windows.Controls.Label labIrisData;
        
        internal System.Windows.Controls.Button btnSubmitIrisData;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/IriskingAttend;component/View/SystemView/BackupConfig.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.txtMannualBackupIrisApp = ((System.Windows.Controls.TextBox)(this.FindName("txtMannualBackupIrisApp")));
            this.labIrisApp = ((System.Windows.Controls.Label)(this.FindName("labIrisApp")));
            this.btnSubmitIrisApp = ((System.Windows.Controls.Button)(this.FindName("btnSubmitIrisApp")));
            this.txtMannualBackupIrisData = ((System.Windows.Controls.TextBox)(this.FindName("txtMannualBackupIrisData")));
            this.labIrisData = ((System.Windows.Controls.Label)(this.FindName("labIrisData")));
            this.btnSubmitIrisData = ((System.Windows.Controls.Button)(this.FindName("btnSubmitIrisData")));
        }
    }
}

