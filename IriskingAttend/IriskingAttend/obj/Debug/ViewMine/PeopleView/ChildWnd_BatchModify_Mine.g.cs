﻿#pragma checksum "G:\IrisAttend\trunk\codes\CSharp\矿山考勤\03 Code\IriskingAttend\IriskingAttend\ViewMine\PeopleView\ChildWnd_BatchModify_Mine.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "23ED47C2F1E8E45393B2E87DECD9D72B"
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
    
    
    public partial class ChildWnd_BatchModify_Mine : System.Windows.Controls.ChildWindow {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.DataGrid dgSelectedPerson;
        
        internal System.Windows.Controls.ComboBox comb_TargetDepart;
        
        internal System.Windows.Controls.ComboBox comb_TargetClassTypeOnGround;
        
        internal System.Windows.Controls.ComboBox comb_TargetClassTypeOnMine;
        
        internal System.Windows.Controls.ComboBox combTargetPrincipal;
        
        internal System.Windows.Controls.ComboBox combTargetWorkType;
        
        internal System.Windows.Controls.Button CancelButton;
        
        internal System.Windows.Controls.Button OKButton;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/IriskingAttend;component/ViewMine/PeopleView/ChildWnd_BatchModify_Mine.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.dgSelectedPerson = ((System.Windows.Controls.DataGrid)(this.FindName("dgSelectedPerson")));
            this.comb_TargetDepart = ((System.Windows.Controls.ComboBox)(this.FindName("comb_TargetDepart")));
            this.comb_TargetClassTypeOnGround = ((System.Windows.Controls.ComboBox)(this.FindName("comb_TargetClassTypeOnGround")));
            this.comb_TargetClassTypeOnMine = ((System.Windows.Controls.ComboBox)(this.FindName("comb_TargetClassTypeOnMine")));
            this.combTargetPrincipal = ((System.Windows.Controls.ComboBox)(this.FindName("combTargetPrincipal")));
            this.combTargetWorkType = ((System.Windows.Controls.ComboBox)(this.FindName("combTargetWorkType")));
            this.CancelButton = ((System.Windows.Controls.Button)(this.FindName("CancelButton")));
            this.OKButton = ((System.Windows.Controls.Button)(this.FindName("OKButton")));
        }
    }
}
