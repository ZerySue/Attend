﻿#pragma checksum "G:\IrisAttend\trunk\codes\CSharp\矿山考勤\03 Code\IriskingAttend\IriskingAttend\View\PeopleView\Page_ClassTypeAndClassOrderMng.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "658F983FB933C1286B752CC69A7FC719"
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
    
    
    public partial class PageClassTypeAndClassOrderMng : System.Windows.Controls.Page {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.Grid gridClassType;
        
        internal System.Windows.Controls.DataGrid dataGridClassType;
        
        internal System.Windows.Controls.TextBlock label1;
        
        internal System.Windows.Controls.Button ExportClassTypeExl;
        
        internal System.Windows.Controls.GridSplitter gridSplitter;
        
        internal System.Windows.Controls.Grid gridClassOrder;
        
        internal System.Windows.Controls.DataGrid dataGridClassOrder;
        
        internal System.Windows.Controls.Button ExportClassOrderExl;
        
        internal System.Windows.Controls.DataGrid dgClassOrderSign;
        
        internal System.Windows.Controls.TextBlock txtCurrentClassOrderSign;
        
        internal System.Windows.Controls.StackPanel spClassOrderSignBtn;
        
        internal System.Windows.Controls.Button ExportClassOrderExlSign;
        
        internal System.Windows.Controls.DataGrid dgClassOrderJiGongShi;
        
        internal System.Windows.Controls.StackPanel spClassOrderJiGongShiBtn;
        
        internal System.Windows.Controls.Button ExportClassOrderExlJiGongShi;
        
        internal System.Windows.Controls.TextBlock txtCurrentClassOrderJiGongShi;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/IriskingAttend;component/View/PeopleView/Page_ClassTypeAndClassOrderMng.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.gridClassType = ((System.Windows.Controls.Grid)(this.FindName("gridClassType")));
            this.dataGridClassType = ((System.Windows.Controls.DataGrid)(this.FindName("dataGridClassType")));
            this.label1 = ((System.Windows.Controls.TextBlock)(this.FindName("label1")));
            this.ExportClassTypeExl = ((System.Windows.Controls.Button)(this.FindName("ExportClassTypeExl")));
            this.gridSplitter = ((System.Windows.Controls.GridSplitter)(this.FindName("gridSplitter")));
            this.gridClassOrder = ((System.Windows.Controls.Grid)(this.FindName("gridClassOrder")));
            this.dataGridClassOrder = ((System.Windows.Controls.DataGrid)(this.FindName("dataGridClassOrder")));
            this.ExportClassOrderExl = ((System.Windows.Controls.Button)(this.FindName("ExportClassOrderExl")));
            this.dgClassOrderSign = ((System.Windows.Controls.DataGrid)(this.FindName("dgClassOrderSign")));
            this.txtCurrentClassOrderSign = ((System.Windows.Controls.TextBlock)(this.FindName("txtCurrentClassOrderSign")));
            this.spClassOrderSignBtn = ((System.Windows.Controls.StackPanel)(this.FindName("spClassOrderSignBtn")));
            this.ExportClassOrderExlSign = ((System.Windows.Controls.Button)(this.FindName("ExportClassOrderExlSign")));
            this.dgClassOrderJiGongShi = ((System.Windows.Controls.DataGrid)(this.FindName("dgClassOrderJiGongShi")));
            this.spClassOrderJiGongShiBtn = ((System.Windows.Controls.StackPanel)(this.FindName("spClassOrderJiGongShiBtn")));
            this.ExportClassOrderExlJiGongShi = ((System.Windows.Controls.Button)(this.FindName("ExportClassOrderExlJiGongShi")));
            this.txtCurrentClassOrderJiGongShi = ((System.Windows.Controls.TextBlock)(this.FindName("txtCurrentClassOrderJiGongShi")));
        }
    }
}
