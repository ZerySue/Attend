﻿#pragma checksum "G:\IrisAttend\trunk\codes\CSharp\矿山考勤\03 Code\IriskingAttend\IriskingAttend\View\SystemView\DeviceManage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "1F0B15B884ED6DCCD7B3D304573D2182"
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
    
    
    public partial class DeviceManage : System.Windows.Controls.Page {
        
        internal System.Windows.Controls.DataGrid dgDevice;
        
        internal System.Windows.Controls.Button btnBatchModifyDevice;
        
        internal System.Windows.Controls.Button btnBatchDeleteDevice;
        
        internal System.Windows.Controls.Button btnAddDevice;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/IriskingAttend;component/View/SystemView/DeviceManage.xaml", System.UriKind.Relative));
            this.dgDevice = ((System.Windows.Controls.DataGrid)(this.FindName("dgDevice")));
            this.btnBatchModifyDevice = ((System.Windows.Controls.Button)(this.FindName("btnBatchModifyDevice")));
            this.btnBatchDeleteDevice = ((System.Windows.Controls.Button)(this.FindName("btnBatchDeleteDevice")));
            this.btnAddDevice = ((System.Windows.Controls.Button)(this.FindName("btnAddDevice")));
            this.btnExportExcel = ((System.Windows.Controls.Button)(this.FindName("btnExportExcel")));
        }
    }
}

