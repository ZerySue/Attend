/*************************************************************************
** 文件名:   CursorEx.cs
×× 主要类:   CursorEx
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   wz
** 日  期:   2013-8-13
** 修改人:   
** 日  期:
** 描  述:   CursorEx类，自定义鼠标图案
**
** 版  本:   1.0.0
** 备  注:  命名及代码编写遵守C#编码规范
**
 * ***********************************************************************/
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
using System.Windows.Controls.Primitives;
using System.Linq;
using System.Diagnostics;
using System.Windows.Threading;
using IriskingAttend;

namespace CustomCursor
{
    /// <summary>
    /// 自定义鼠标类
    /// </summary>
    public class CursorEx
    {
        private static Popup _cursorPopup;                    //用于显示鼠标图案的Popup
        private static DispatcherTimer _mouseLeaveTimer;      //鼠标离开计时器
        private static object _syncRoot = new object();       //锁
        private static GeneralTransform _generalTransform;    //移动矩阵
        private static Point _mousePoint;                     //当前鼠标位置
        private static UIElement _popupChild;                 //Popup的子元素
        private static FrameworkElement _shownElement;        //显示的UI
        private static FrameworkElement _capturingElement;    //获得鼠标输入的UI

        private static Popup CursorPopup
        {
            get
            {
                if (_cursorPopup == null)
                {
                    lock (_syncRoot)
                    {
                        if (_cursorPopup == null)
                        {
                            _cursorPopup = new Popup();
                            _cursorPopup.IsHitTestVisible = false;
                            _cursorPopup.IsOpen = true;
                        }
                    }
                }
                return _cursorPopup;
            }
        }

        /// <summary>
        /// MouseLeave启动后
        /// </summary>
        private static DispatcherTimer MouseLeaveTimer
        {
            get
            {
                if (_mouseLeaveTimer == null)
                {
                    lock (_syncRoot)
                    {
                        if (_mouseLeaveTimer == null)
                        {
                            _mouseLeaveTimer = new DispatcherTimer();
                            _mouseLeaveTimer.Interval = TimeSpan.FromMilliseconds(10);
                            _mouseLeaveTimer.Tick += new EventHandler(OnMouseLeaveTimerTick);

                        }
                    }
                }
                return _mouseLeaveTimer;
            }
        }

        #region dependency 属性

        #region 自定义鼠标
        public static UIElement GetCustomCursor(DependencyObject obj) 
        {
            return (UIElement)obj.GetValue(CustomCursorProperty); 
        }

        public static void SetCustomCursor(DependencyObject obj, UIElement value) 
        {
            obj.SetValue(CustomCursorProperty, value); 
        }

        public static readonly DependencyProperty CustomCursorProperty =
            DependencyProperty.RegisterAttached("CustomCursor", typeof(FrameworkElement), typeof(CursorEx), new PropertyMetadata(OnCursorChanged));
        #endregion

        #region 是否使用默认鼠标
        
        public static bool GetUseOriginalCursor(DependencyObject obj)
        {
            return (bool)obj.GetValue(UseOriginalCursorProperty);
        }

        public static void SetUseOriginalCursor(DependencyObject obj, bool value)
        {
            obj.SetValue(UseOriginalCursorProperty, value);
        }

        public static readonly DependencyProperty UseOriginalCursorProperty =
            DependencyProperty.RegisterAttached("UseOriginalCursor", typeof(bool), typeof(CursorEx), new PropertyMetadata(OnUseOriginalCursorChanged));
        #endregion

        #region 本来的鼠标

        private static Cursor GetOriginalCursor(DependencyObject obj)
        {
            return (Cursor)obj.GetValue(OriginalCursorProperty);
        }

        private static void SetOriginalCursor(DependencyObject obj, Cursor value)
        {
            obj.SetValue(OriginalCursorProperty, value);
        }

        public static readonly DependencyProperty OriginalCursorProperty =
            DependencyProperty.RegisterAttached("OriginalCursor", typeof(Cursor), typeof(CursorEx), null);
        #endregion

        #endregion

        #region 响应事件

        private static void OnUseOriginalCursorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            FrameworkElement element = obj as FrameworkElement;
            if (element != null)
            {
                SetCusorToUIElement(element);
            }
               
        }


        private static void OnCursorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            FrameworkElement element = obj as FrameworkElement;
            if (element != null)
            {
                SetCusorToUIElement(element);
            }
                
        }

        private static void OnMouseMove(object sender, MouseEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            _generalTransform = element.TransformToVisual(App.Current.RootVisual);
            OnMouseMove(element, _generalTransform.Transform(e.GetPosition(element)));
        }


        private static void OnMouseLeave(object sender, MouseEventArgs e)
        {
            var popup = CursorPopup;
            FrameworkElement element = sender as FrameworkElement;
            var child = GetCustomCursor(element);
            if (child != null)
            {
                child.Visibility = Visibility.Collapsed;
            }

            if (_capturingElement == element)
            {
                MouseLeaveTimer.Start();
            }
            else
            {
                _shownElement = null;
            }
        }

        private static void OnMouseLeaveTimerTick(object sender, EventArgs e)
        {
            if (_capturingElement == null || CheckIsCapturing(_capturingElement) == false)
            {
                MouseLeaveTimer.Stop();
                _shownElement = null;
                UpdateCurrentChild();
            }
        }
        #endregion

        #region 私有功能函数
       
        private static void SetCusorToUIElement(FrameworkElement element)
        {
            var customCurosr = GetCustomCursor(element);
            var userOriginalCursor = GetUseOriginalCursor(element);
            if (customCurosr != null || userOriginalCursor)
            {
                if (customCurosr != null)
                {
                    customCurosr.IsHitTestVisible = false;
                    if (_shownElement == element && CursorPopup.Child != null)
                    {
                        customCurosr.Visibility = CursorPopup.Child.Visibility;
                        CursorPopup.Child = customCurosr;
                    }
                }
                if (userOriginalCursor == false)
                {
                    element.Cursor = Cursors.None;
                }
                    
                DetachEvent(element);
                AttachEvent(element);
                if (_mousePoint != null && VisualTreeHelper.FindElementsInHostCoordinates(_mousePoint, element).Contains(element))
                {
                    //OnMouseMove(element, _mousePoint);
                }
                   
            }
            else
            {
                SetIsHandeld(element, false);
                element.Cursor = GetOriginalCursor(element);
                UpdateCurrentChild();
            }

            if (GetOriginalCursor(element) == null && element.Cursor != Cursors.None)
            {
                SetOriginalCursor(element, element.Cursor);
            }
               
        }

        private static void AttachEvent(FrameworkElement element)
        {
            element.MouseLeave += new MouseEventHandler(OnMouseLeave);
            element.MouseMove += new MouseEventHandler(OnMouseMove);
        }

        private static void DetachEvent(FrameworkElement element)
        {
            element.MouseLeave -= new MouseEventHandler(OnMouseLeave);
            element.MouseMove -= new MouseEventHandler(OnMouseMove);
        }

        private static bool CheckIsHandled(FrameworkElement element)
        {
            if (_shownElement == element)
            {
                return false;
            }
              
            FrameworkElement parent = _shownElement;
            while (parent != null && parent != element)
            {
                parent = VisualTreeHelper.GetParent(parent) as FrameworkElement;
            }

            if (parent != null)
            {
                return true;
            }
            else
            {
                return false;
            }
              
        }

        private static void SetIsHandeld(FrameworkElement element, bool value)
        {
            if (value)
            {
                _shownElement = element;
            }
            else
            {
                if (_shownElement == element)
                    _shownElement = null;
                DetachEvent(element);
            }
        }

        private static void OnMouseMove(FrameworkElement element, Point mousePoint)
        {
            _mousePoint = mousePoint;
            if (element == null || CheckIsHandled(element))
            {
                return;
            }
              
            SetIsHandeld(element, true);
            _popupChild = GetCustomCursor(element);

            if (_popupChild != CursorPopup.Child)
            {
                CursorPopup.Child = _popupChild;
            }

            if (_popupChild == null)
            {
                return;
            }

            if (_popupChild.Visibility != Visibility.Visible)
            {
                _popupChild.Visibility = Visibility.Visible;
            }
                
            CursorPopup.HorizontalOffset = _mousePoint.X;
            CursorPopup.VerticalOffset = _mousePoint.Y;

            if (CheckIsCapturing(element))
            {
                _capturingElement = element;
            }
            else if (_capturingElement == element)
            {
                _capturingElement = null;
            }
               
            GetToppestElement(_mousePoint);
        }

        private static void UpdateCurrentChild()
        {
            if (_mousePoint == null)
            {
                return;
            }
                
            var pointElement = GetToppestElement(_mousePoint);
            if (pointElement == null)
            {
                CursorPopup.Child = null;
            }
            else
            {
                OnMouseMove(pointElement, _mousePoint);
            }
               
        }

        private static FrameworkElement GetToppestElement(Point point)
        {
            var elements = VisualTreeHelper.FindElementsInHostCoordinates(point, App.Current.RootVisual);
            return elements.Where(es => (es is FrameworkElement)
                && (GetCustomCursor(es as FrameworkElement) != null || GetUseOriginalCursor(es as FrameworkElement) == true))
                .ElementAtOrDefault(0) as FrameworkElement;
        }

        private static bool CheckIsCapturing(FrameworkElement element)
        {
            bool isRootCapturingMouse = App.Current.RootVisual.CaptureMouse();
            App.Current.RootVisual.ReleaseMouseCapture();
            if (isRootCapturingMouse)
            {
                return false;
            }  
            else
            {
                return element.CaptureMouse(); 
            }
        }

        #endregion
    }


}
