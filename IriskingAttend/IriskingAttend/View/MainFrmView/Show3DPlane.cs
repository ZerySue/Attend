using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace IriskingAttend
{
    public enum enumDirection
    {
        Up, Down, Left, Right
    };
    public class Show3DPlane
    {
        public Show3DPlane()
        {
            m_IsMoveOverIn = true;
            m_IsMoveOverOut = true;

            m_MoveSpeed = 2;

            sbTimeIn = new Storyboard();
            sbTimeOut = new Storyboard();
            SetTime(10, 10);
            sbTimeIn.Completed += new EventHandler(sbTimeIn_Completed);
            sbTimeOut.Completed += new EventHandler(sbTimeOut_Completed);
        }

        private int m_TimeSpeed1, m_TimeSpeed2, m_MoveSpeed, m_PlaneSpeed, m_InEnd, m_OutEnd;
        private double m_InLocalZ, m_OutLocalZ;
        private PlaneProjection m_pInPlane, m_pOutPlane;
        private Storyboard sbTimeIn, sbTimeOut;
        private enumDirection m_Direction;
        private bool m_IsMoveOverIn, m_IsMoveOverOut;

        //根据方向初始化两个planeProjection的相关值
        private void InitPlaneData()
        {
            m_pInPlane.RotationX = 0;
            m_pInPlane.RotationY = 0;
            m_pInPlane.RotationZ = 0;
            m_pInPlane.CenterOfRotationX = 0.5;
            m_pInPlane.CenterOfRotationY = 0.5;
            m_pInPlane.CenterOfRotationZ = 0;

            m_pOutPlane.RotationX = 0;
            m_pOutPlane.RotationY = 0;
            m_pOutPlane.RotationZ = 0;
            m_pOutPlane.CenterOfRotationX = 0.5;
            m_pOutPlane.CenterOfRotationY = 0.5;
            m_pOutPlane.CenterOfRotationZ = 0;

            m_pOutPlane.LocalOffsetZ = m_OutLocalZ;
            m_pOutPlane.GlobalOffsetZ = -m_OutLocalZ;
            m_pInPlane.LocalOffsetZ = m_InLocalZ;
            m_pInPlane.GlobalOffsetZ = -m_InLocalZ;
            switch (m_Direction)
            {
                case enumDirection.Up:
                    m_pInPlane.RotationX = -90;
                    m_InEnd = 0;

                    m_pOutPlane.RotationX = 0;
                    m_OutEnd = 90;

                    m_PlaneSpeed = m_MoveSpeed;
                    break;
                case enumDirection.Down:
                    m_pInPlane.RotationX = 90;
                    m_InEnd = 0;

                    m_pOutPlane.RotationX = 0;
                    m_OutEnd = -90;

                    m_PlaneSpeed = -m_MoveSpeed;
                    break;
                case enumDirection.Left:
                    m_pInPlane.RotationY = 90;
                    m_InEnd = 0;

                    m_pOutPlane.RotationY = 0;
                    m_OutEnd = -90;

                    m_PlaneSpeed = -m_MoveSpeed;

                    break;
                case enumDirection.Right:
                    m_pInPlane.RotationY = -90;
                    m_InEnd = 0;

                    m_pOutPlane.RotationY = 0;
                    m_OutEnd = 90;

                    m_PlaneSpeed = m_MoveSpeed;
                    break;
            }
        }
        public void Begin()
        {

            m_IsMoveOverIn = false;
            m_IsMoveOverOut = false;

            sbTimeIn.Begin();
            sbTimeOut.Begin();
        }
        //设置进入和离开对象
        public void SetInOutPlane(Grid gridIn, Grid gridOut, enumDirection eDirection)
        {
            m_pInPlane = new PlaneProjection();
            m_pOutPlane = new PlaneProjection();

            gridIn.Projection = m_pInPlane;
            gridOut.Projection = m_pOutPlane;
            if (eDirection == enumDirection.Left || eDirection == enumDirection.Right)
            {
                m_InLocalZ = gridIn.ActualWidth / 2;
                m_OutLocalZ = gridOut.ActualWidth / 2;
            }
            else
            {
                m_InLocalZ = gridIn.ActualHeight / 2;
                m_OutLocalZ = gridOut.ActualHeight / 2;
            }
            m_Direction = eDirection;
            InitPlaneData();
        }
        //重载
        public void SetInOutPlane(Control controlIn, Control controlOut, enumDirection eDirection)
        {
            m_pInPlane = new PlaneProjection();
            m_pOutPlane = new PlaneProjection();

            controlIn.Projection = m_pInPlane;
            controlOut.Projection = m_pOutPlane;

            if (eDirection == enumDirection.Left || eDirection == enumDirection.Right)
            {
                m_InLocalZ = controlIn.ActualWidth / 2;
                m_OutLocalZ = controlOut.ActualWidth / 2;
            }
            else
            {
                m_InLocalZ = controlIn.ActualHeight / 2;
                m_OutLocalZ = controlOut.ActualHeight / 2;
            }

            m_Direction = eDirection;

            InitPlaneData();
        }
        public bool MoveOver()
        {
            if (!m_IsMoveOverIn || !m_IsMoveOverOut)
                return false;
            else
                return true;

        }
        //设置进入和离开动画的速度
        public void SetTime(int timeSpeed1, int timeSpeed2)
        {
            m_TimeSpeed1 = timeSpeed1;
            m_TimeSpeed2 = timeSpeed2;
            sbTimeIn.Duration = new Duration(TimeSpan.FromMilliseconds(m_TimeSpeed1));
            sbTimeOut.Duration = new Duration(TimeSpan.FromMilliseconds(m_TimeSpeed2));
        }
        //离开对象的动画
        void sbTimeOut_Completed(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            if (m_Direction == enumDirection.Left || m_Direction == enumDirection.Right)
            {
                m_pOutPlane.RotationY += m_PlaneSpeed;
                if (m_pOutPlane.RotationY == m_OutEnd) m_IsMoveOverOut = true;
            }
            if (m_Direction == enumDirection.Up || m_Direction == enumDirection.Down)
            {
                m_pOutPlane.RotationX += m_PlaneSpeed;
                if (m_pOutPlane.RotationX == m_OutEnd) m_IsMoveOverOut = true;
            }

            if (!m_IsMoveOverOut)
                sbTimeOut.Begin();
            else
            {
                m_pOutPlane.LocalOffsetZ = 0;
                m_pOutPlane.GlobalOffsetZ = 0;
            }
        }
        //进入对象的动画
        void sbTimeIn_Completed(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            if (m_Direction == enumDirection.Left || m_Direction == enumDirection.Right)
            {
                m_pInPlane.RotationY += m_PlaneSpeed;
                if (m_pInPlane.RotationY == m_InEnd) m_IsMoveOverIn = true;
            }
            if (m_Direction == enumDirection.Up || m_Direction == enumDirection.Down)
            {
                m_pInPlane.RotationX += m_PlaneSpeed;
                if (m_pInPlane.RotationX == m_InEnd) m_IsMoveOverIn = true;
            }
            if (!m_IsMoveOverIn)
                sbTimeIn.Begin();
            else
            {
                m_pInPlane.LocalOffsetZ = 0;
                m_pInPlane.GlobalOffsetZ = 0;
            }
        }
    }
}