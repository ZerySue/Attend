﻿#pragma checksum "G:\IrisAttend\trunk\codes\CSharp\矿山考勤\03 Code\IriskingAttend\IriskingAttend\CustomCursor\MovableMouse.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "E41C611D632CEDE833613858248F1DC2"
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


namespace CustomCursor {
    
    
    public partial class MovableMouse : System.Windows.Controls.UserControl {
        
        internal System.Windows.Media.Animation.Storyboard storyboard;
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Media.TranslateTransform trans;
        
        internal System.Windows.Controls.Image image;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/IriskingAttend;component/CustomCursor/MovableMouse.xaml", System.UriKind.Relative));
            this.storyboard = ((System.Windows.Media.Animation.Storyboard)(this.FindName("storyboard")));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.trans = ((System.Windows.Media.TranslateTransform)(this.FindName("trans")));
            this.image = ((System.Windows.Controls.Image)(this.FindName("image")));
        }
    }
}

