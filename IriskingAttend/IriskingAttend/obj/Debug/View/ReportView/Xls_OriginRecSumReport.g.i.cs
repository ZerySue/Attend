﻿#pragma checksum "G:\IrisAttend\trunk\codes\CSharp\矿山考勤\03 Code\IriskingAttend\IriskingAttend\View\ReportView\Xls_OriginRecSumReport.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "25E991829D65D6232F48EA39907646A0"
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


namespace IriskingAttend {
    
    
    public partial class Xls_OriginRecSumReport : System.Windows.Controls.Page {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.Label label1;
        
        internal System.Windows.Controls.DatePicker dateBegin;
        
        internal System.Windows.Controls.Label lbMust;
        
        internal System.Windows.Controls.Label label2;
        
        internal System.Windows.Controls.ComboBox listBoxDepart;
        
        internal System.Windows.Controls.Label label3;
        
        internal System.Windows.Controls.ComboBox comboBoxClassTypeName;
        
        internal System.Windows.Controls.Label label4;
        
        internal System.Windows.Controls.TextBox txtPersonName;
        
        internal System.Windows.Controls.Label label5;
        
        internal System.Windows.Controls.TextBox txtWorkSn;
        
        internal System.Windows.Controls.Button btnQuery;
        
        internal System.Windows.Controls.DataGrid dgXlsAttend;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/IriskingAttend;component/View/ReportView/Xls_OriginRecSumReport.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.label1 = ((System.Windows.Controls.Label)(this.FindName("label1")));
            this.dateBegin = ((System.Windows.Controls.DatePicker)(this.FindName("dateBegin")));
            this.lbMust = ((System.Windows.Controls.Label)(this.FindName("lbMust")));
            this.label2 = ((System.Windows.Controls.Label)(this.FindName("label2")));
            this.listBoxDepart = ((System.Windows.Controls.ComboBox)(this.FindName("listBoxDepart")));
            this.label3 = ((System.Windows.Controls.Label)(this.FindName("label3")));
            this.comboBoxClassTypeName = ((System.Windows.Controls.ComboBox)(this.FindName("comboBoxClassTypeName")));
            this.label4 = ((System.Windows.Controls.Label)(this.FindName("label4")));
            this.txtPersonName = ((System.Windows.Controls.TextBox)(this.FindName("txtPersonName")));
            this.label5 = ((System.Windows.Controls.Label)(this.FindName("label5")));
            this.txtWorkSn = ((System.Windows.Controls.TextBox)(this.FindName("txtWorkSn")));
            this.btnQuery = ((System.Windows.Controls.Button)(this.FindName("btnQuery")));
            this.dgXlsAttend = ((System.Windows.Controls.DataGrid)(this.FindName("dgXlsAttend")));
            this.btnPrint = ((System.Windows.Controls.Button)(this.FindName("btnPrint")));
            this.btnExportExcel = ((System.Windows.Controls.Button)(this.FindName("btnExportExcel")));
        }
    }
}

