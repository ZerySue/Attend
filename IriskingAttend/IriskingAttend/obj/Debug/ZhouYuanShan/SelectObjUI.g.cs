﻿#pragma checksum "G:\IrisAttend\trunk\codes\CSharp\矿山考勤\03 Code\IriskingAttend\IriskingAttend\ZhouYuanShan\SelectObjUI.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "22CB989329A8199B13CA0E7A9B461439"
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


namespace IriskingAttend.ZhouYuanShan {
    
    
    public partial class SelectObjUI : System.Windows.Controls.UserControl {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.ListBox candidateList;
        
        internal System.Windows.Controls.Label label1;
        
        internal System.Windows.Controls.ListBox selectedList;
        
        internal System.Windows.Controls.Label label2;
        
        internal System.Windows.Controls.StackPanel stackPanelDepart;
        
        internal System.Windows.Controls.ComboBox cmbDepart;
        
        internal System.Windows.Controls.Button addButton;
        
        internal System.Windows.Controls.Button removeBtn;
        
        internal System.Windows.Controls.Button addAllBtn;
        
        internal System.Windows.Controls.Button removeAllBtn;
        
        internal System.Windows.Controls.TextBox txtLike;
        
        internal System.Windows.Controls.Button btnQuery;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/IriskingAttend;component/ZhouYuanShan/SelectObjUI.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.candidateList = ((System.Windows.Controls.ListBox)(this.FindName("candidateList")));
            this.label1 = ((System.Windows.Controls.Label)(this.FindName("label1")));
            this.selectedList = ((System.Windows.Controls.ListBox)(this.FindName("selectedList")));
            this.label2 = ((System.Windows.Controls.Label)(this.FindName("label2")));
            this.stackPanelDepart = ((System.Windows.Controls.StackPanel)(this.FindName("stackPanelDepart")));
            this.cmbDepart = ((System.Windows.Controls.ComboBox)(this.FindName("cmbDepart")));
            this.addButton = ((System.Windows.Controls.Button)(this.FindName("addButton")));
            this.removeBtn = ((System.Windows.Controls.Button)(this.FindName("removeBtn")));
            this.addAllBtn = ((System.Windows.Controls.Button)(this.FindName("addAllBtn")));
            this.removeAllBtn = ((System.Windows.Controls.Button)(this.FindName("removeAllBtn")));
            this.txtLike = ((System.Windows.Controls.TextBox)(this.FindName("txtLike")));
            this.btnQuery = ((System.Windows.Controls.Button)(this.FindName("btnQuery")));
        }
    }
}

