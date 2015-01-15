using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Navigation;
using System.Collections.ObjectModel;

namespace IriskingAttend
{
    public partial class FrmMain : Page
    {
        private ContentControl contentCur = new ContentControl();
        private ContentControl contentNext = new ContentControl();
        private Show3DPlane c3d = new Show3DPlane();        

        public FrmMain()
        {
            InitializeComponent();  
        }

        // 当用户导航到此页面时执行。
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void LayoutRoot_Loaded(object sender, RoutedEventArgs e)
        {
            PlaneProjection pp = new PlaneProjection();
            pp.RotationY = 90;
            rightFuntionControl.Projection = pp;            
            contentCur = leftFuntionControl;
            contentNext = rightFuntionControl;           
        }        

        private void C3dRetrun()
        {
            if (c3d.MoveOver())
            {
                c3d.SetInOutPlane(contentNext, contentCur, enumDirection.Right);
                contentCur = contentNext;
                if (contentCur == leftFuntionControl)
                {
                    contentNext = rightFuntionControl;
                }
                if (contentCur == rightFuntionControl)
                {
                    contentNext = leftFuntionControl;
                }                
                c3d.Begin();
            }
        }

        public void ConvertToFunctionPage( Uri url)
        {
            if (contentNext == rightFuntionControl)
            {
                rightContentFrame.Source = url;
            }
            if (contentNext == leftFuntionControl)
            {
                leftContentFrame.Source = url;
            }

            C3dRetrun(); 
        }     

    }
}
