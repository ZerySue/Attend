﻿#pragma checksum "G:\IrisAttend\trunk\codes\CSharp\矿山考勤\03 Code\IriskingAttend\IriskingAttend\ViewMine\PeopleView\ChildWnd_OperatePerson_Mine.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "7D461090D3AFABFA454785AE93B94AA3"
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
    
    
    public partial class ChildWnd_OperatePerson_Mine : System.Windows.Controls.ChildWindow {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.ComboBox Comb_DepartName;
        
        internal System.Windows.Controls.Image img;
        
        internal System.Windows.Controls.Button btn_chooseImg;
        
        internal System.Windows.Controls.Button btn_cancelChoosedImg;
        
        internal System.Windows.Controls.Image imageNone;
        
        internal System.Windows.Controls.Image imageSelect;
        
        internal System.Windows.Controls.Button OKContinueAdd;
        
        internal System.Windows.Controls.Button OKButton;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/IriskingAttend;component/ViewMine/PeopleView/ChildWnd_OperatePerson_Mine.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.Comb_DepartName = ((System.Windows.Controls.ComboBox)(this.FindName("Comb_DepartName")));
            this.img = ((System.Windows.Controls.Image)(this.FindName("img")));
            this.btn_chooseImg = ((System.Windows.Controls.Button)(this.FindName("btn_chooseImg")));
            this.btn_cancelChoosedImg = ((System.Windows.Controls.Button)(this.FindName("btn_cancelChoosedImg")));
            this.imageNone = ((System.Windows.Controls.Image)(this.FindName("imageNone")));
            this.imageSelect = ((System.Windows.Controls.Image)(this.FindName("imageSelect")));
            this.OKContinueAdd = ((System.Windows.Controls.Button)(this.FindName("OKContinueAdd")));
            this.OKButton = ((System.Windows.Controls.Button)(this.FindName("OKButton")));
            this.CancelButton = ((System.Windows.Controls.Button)(this.FindName("CancelButton")));
        }
    }
}

