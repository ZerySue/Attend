﻿#pragma checksum "G:\IrisAttend\trunk\codes\CSharp\矿山考勤\03 Code\IriskingAttend\IriskingAttend\View\AttendView\InWellPerson.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "7A77210B315B959C6EBB8C9DB3384F3F"
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
    
    
    public partial class InWellPerson : System.Windows.Controls.Page {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.Label lab4;
        
        internal System.Windows.Controls.Label labInWellPersonCount;
        
        internal System.Windows.Controls.Label lab6;
        
        internal System.Windows.Controls.ComboBox cmbDepart;
        
        internal System.Windows.Controls.DataGrid dgInWellPerson;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/IriskingAttend;component/View/AttendView/InWellPerson.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.lab4 = ((System.Windows.Controls.Label)(this.FindName("lab4")));
            this.labInWellPersonCount = ((System.Windows.Controls.Label)(this.FindName("labInWellPersonCount")));
            this.lab6 = ((System.Windows.Controls.Label)(this.FindName("lab6")));
            this.cmbDepart = ((System.Windows.Controls.ComboBox)(this.FindName("cmbDepart")));
            this.dgInWellPerson = ((System.Windows.Controls.DataGrid)(this.FindName("dgInWellPerson")));
            this.btnExportExl = ((System.Windows.Controls.Button)(this.FindName("btnExportExl")));
        }
    }
}
