﻿#pragma checksum "G:\IrisAttend\trunk\codes\CSharp\矿山考勤\03 Code\IriskingAttend\IriskingAttend\ZhouYuanShan\LunchSubsidy\PageUnCompletedLunch.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "D0FEC42E35C6856A25A915FACA08F509"
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


namespace IriskingAttend.ZhouYuanShan.LunchSubsidy {
    
    
    public partial class PageUnCompletedLunch : System.Windows.Controls.Page {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.DatePicker dateBegin;
        
        internal System.Windows.Controls.DatePicker dateEnd;
        
        internal System.Windows.Controls.Button btnQuery;
        
        internal System.Windows.Controls.DataGrid dataGridUnCompletedLunch;
        
        internal System.Windows.Controls.DataGrid dataGridCompletedLunch;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/IriskingAttend;component/ZhouYuanShan/LunchSubsidy/PageUnCompletedLunch.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.dateBegin = ((System.Windows.Controls.DatePicker)(this.FindName("dateBegin")));
            this.dateEnd = ((System.Windows.Controls.DatePicker)(this.FindName("dateEnd")));
            this.btnQuery = ((System.Windows.Controls.Button)(this.FindName("btnQuery")));
            this.dataGridUnCompletedLunch = ((System.Windows.Controls.DataGrid)(this.FindName("dataGridUnCompletedLunch")));
            this.dataGridCompletedLunch = ((System.Windows.Controls.DataGrid)(this.FindName("dataGridCompletedLunch")));
        }
    }
}

