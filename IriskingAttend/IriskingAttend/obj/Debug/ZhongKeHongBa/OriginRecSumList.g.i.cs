﻿#pragma checksum "G:\IrisAttend\trunk\codes\CSharp\矿山考勤\03 Code\IriskingAttend\IriskingAttend\ZhongKeHongBa\OriginRecSumList.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "E7B6FE24AEBDB9626809AB4AC7EE5676"
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


namespace IriskingAttend.ZhongKeHongBa {
    
    
    public partial class OriginRecSumList : System.Windows.Controls.Page {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.DatePicker dtpBegin;
        
        internal System.Windows.Controls.TextBox txtDepart;
        
        internal System.Windows.Controls.Button btnSelectDepart;
        
        internal System.Windows.Controls.TextBox txtPersonName;
        
        internal System.Windows.Controls.Button btnSelectName;
        
        internal System.Windows.Controls.Button btnQuery;
        
        internal System.Windows.Controls.DataGrid dgPersonOriginList;
        
        internal System.Windows.Controls.Button btnPrint;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/IriskingAttend;component/ZhongKeHongBa/OriginRecSumList.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.dtpBegin = ((System.Windows.Controls.DatePicker)(this.FindName("dtpBegin")));
            this.txtDepart = ((System.Windows.Controls.TextBox)(this.FindName("txtDepart")));
            this.btnSelectDepart = ((System.Windows.Controls.Button)(this.FindName("btnSelectDepart")));
            this.txtPersonName = ((System.Windows.Controls.TextBox)(this.FindName("txtPersonName")));
            this.btnSelectName = ((System.Windows.Controls.Button)(this.FindName("btnSelectName")));
            this.btnQuery = ((System.Windows.Controls.Button)(this.FindName("btnQuery")));
            this.dgPersonOriginList = ((System.Windows.Controls.DataGrid)(this.FindName("dgPersonOriginList")));
            this.btnPrint = ((System.Windows.Controls.Button)(this.FindName("btnPrint")));
            this.btnExportExcel = ((System.Windows.Controls.Button)(this.FindName("btnExportExcel")));
        }
    }
}

