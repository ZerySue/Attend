﻿#pragma checksum "G:\IrisAttend\trunk\codes\CSharp\矿山考勤\03 Code\IriskingAttend\IriskingAttend\View\PeopleView\ChildWndOperatePrincipal.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "DFF8B036A310A80CC31C54BB06CE3236"
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


namespace IriskingAttend.ViewMine.PeopleView {
    
    
    public partial class ChildWndOperatePrincipal : System.Windows.Controls.ChildWindow {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.TextBox txtName;
        
        internal System.Windows.Controls.ComboBox cmbPrincipalType;
        
        internal System.Windows.Controls.TextBox txtMemo;
        
        internal System.Windows.Controls.Button ContinueButton;
        
        internal System.Windows.Controls.Button OKButton;
        
        internal System.Windows.Controls.TextBlock OkBtnContent;
        
        internal System.Windows.Controls.Button CancelButton;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/IriskingAttend;component/View/PeopleView/ChildWndOperatePrincipal.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.txtName = ((System.Windows.Controls.TextBox)(this.FindName("txtName")));
            this.cmbPrincipalType = ((System.Windows.Controls.ComboBox)(this.FindName("cmbPrincipalType")));
            this.txtMemo = ((System.Windows.Controls.TextBox)(this.FindName("txtMemo")));
            this.ContinueButton = ((System.Windows.Controls.Button)(this.FindName("ContinueButton")));
            this.OKButton = ((System.Windows.Controls.Button)(this.FindName("OKButton")));
            this.OkBtnContent = ((System.Windows.Controls.TextBlock)(this.FindName("OkBtnContent")));
            this.CancelButton = ((System.Windows.Controls.Button)(this.FindName("CancelButton")));
        }
    }
}

