﻿#pragma checksum "G:\IrisAttend\trunk\codes\CSharp\矿山考勤\03 Code\IriskingAttend\IriskingAttend\View\PeopleView\PageDepartAndPeopleMng.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "CF6928075BCA2769BA6BEFAA1798A2B5"
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


namespace IriskingAttend.View.PeopleView {
    
    
    public partial class PageDepartAndPeopleMng : System.Windows.Controls.Page {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.TreeView departTree;
        
        internal System.Windows.Controls.Button btnAddDepart;
        
        internal System.Windows.Controls.Button btnDelDepart;
        
        internal System.Windows.Controls.Button btnModifyDepart;
        
        internal System.Windows.Controls.GridSplitter gridSplitter;
        
        internal System.Windows.Controls.TextBlock textBlock2;
        
        internal System.Windows.Controls.ComboBox comboBox2;
        
        internal System.Windows.Controls.TextBlock textBlock3;
        
        internal System.Windows.Controls.ComboBox comboBox3;
        
        internal System.Windows.Controls.TextBox name;
        
        internal System.Windows.Controls.TextBox work_sn;
        
        internal System.Windows.Controls.Button btnQuery;
        
        internal System.Windows.Controls.Button btnBatchAddAttendLeave;
        
        internal System.Windows.Controls.Button btnBatchAddRecord;
        
        internal System.Windows.Controls.Button btnBatchModifyPerson;
        
        internal System.Windows.Controls.Button btnBatchStopIris;
        
        internal System.Windows.Controls.Button btnBatchDeletePerson;
        
        internal System.Windows.Controls.Button btnAddPerson;
        
        internal System.Windows.Controls.Button btnExportExl;
        
        internal System.Windows.Controls.DataGrid dataGridPerson;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/IriskingAttend;component/View/PeopleView/PageDepartAndPeopleMng.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.departTree = ((System.Windows.Controls.TreeView)(this.FindName("departTree")));
            this.btnAddDepart = ((System.Windows.Controls.Button)(this.FindName("btnAddDepart")));
            this.btnDelDepart = ((System.Windows.Controls.Button)(this.FindName("btnDelDepart")));
            this.btnModifyDepart = ((System.Windows.Controls.Button)(this.FindName("btnModifyDepart")));
            this.gridSplitter = ((System.Windows.Controls.GridSplitter)(this.FindName("gridSplitter")));
            this.textBlock2 = ((System.Windows.Controls.TextBlock)(this.FindName("textBlock2")));
            this.comboBox2 = ((System.Windows.Controls.ComboBox)(this.FindName("comboBox2")));
            this.textBlock3 = ((System.Windows.Controls.TextBlock)(this.FindName("textBlock3")));
            this.comboBox3 = ((System.Windows.Controls.ComboBox)(this.FindName("comboBox3")));
            this.name = ((System.Windows.Controls.TextBox)(this.FindName("name")));
            this.work_sn = ((System.Windows.Controls.TextBox)(this.FindName("work_sn")));
            this.btnQuery = ((System.Windows.Controls.Button)(this.FindName("btnQuery")));
            this.btnBatchAddAttendLeave = ((System.Windows.Controls.Button)(this.FindName("btnBatchAddAttendLeave")));
            this.btnBatchAddRecord = ((System.Windows.Controls.Button)(this.FindName("btnBatchAddRecord")));
            this.btnBatchModifyPerson = ((System.Windows.Controls.Button)(this.FindName("btnBatchModifyPerson")));
            this.btnBatchStopIris = ((System.Windows.Controls.Button)(this.FindName("btnBatchStopIris")));
            this.btnBatchDeletePerson = ((System.Windows.Controls.Button)(this.FindName("btnBatchDeletePerson")));
            this.btnAddPerson = ((System.Windows.Controls.Button)(this.FindName("btnAddPerson")));
            this.btnExportExl = ((System.Windows.Controls.Button)(this.FindName("btnExportExl")));
            this.dataGridPerson = ((System.Windows.Controls.DataGrid)(this.FindName("dataGridPerson")));
        }
    }
}

