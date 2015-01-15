/*************************************************************************
** 文件名:   MovableMouse.cs
** 主要类:   MovableMouse
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   wz
** 日  期:   2013-8-13
** 修改人:   
** 日  期:   
** 描  述:   MovableMouse类,自定义的动画,
 *           一般与自定义鼠标图案相结合使用，用于代替原始鼠标图案
**
** 版  本:   1.0.0
** 备  注:  命名及代码编写遵守C#编码规范
**
*************************************************************************/
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Reflection;
using IriskingAttend.CustomCursor;
using IriskingAttend;

namespace CustomCursor
{
	public partial class MovableMouse : UserControl
    {
        #region 私有字段

        Action<MovableMouse> _callback;             //控件消失后回调函数


        #endregion

        #region 构造函数

        /// <summary>
        /// 创建自定义可拖拽鼠标图案
        /// </summary>
        /// <param name="_uri">显示图片资源的路径</param>
        /// <param name="width">显示宽度</param>
        /// <param name="callback">回调函数</param>
        public MovableMouse(string _uri, double width = 60, Action<MovableMouse> callback = null)
		{
            Init(width, callback);
            var uri = new Uri(_uri, UriKind.Relative);
            image.Source = new System.Windows.Media.Imaging.BitmapImage(uri);
          
		}

        /// <summary>
        /// 创建自定义可拖拽鼠标图案
        /// </summary>
        /// <param name="_showImage">待显示图片控件</param>
        /// <param name="width">显示宽度</param>
        /// <param name="callback">回调函数</param>
        public MovableMouse(UIElement _showImage, Action<MovableMouse> callback = null)
        {
            Init(null, callback);
            int index = LayoutRoot.Children.IndexOf(image);
            LayoutRoot.Children.Insert(index, _showImage);
            LayoutRoot.Children.Remove(image);

        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="width">显示宽度</param>
        /// <param name="callback">回调函数</param>
        private void Init(double? width , Action<MovableMouse> callback)
        {

            InitializeComponent();
            if (width.HasValue)
            {
                this.Width = width.Value;
            }
           
            _callback = callback;

            //添加动画结束事件
            storyboard.Completed += (o, agrs) =>
            {
                //释放鼠标设备
                this.ReleaseMouseCapture();
                if (_callback != null)
                {
                    _callback(this);
                }
            };

        }

        #endregion

        #region 公有函数
      
        /// <summary>
        /// 使之成为鼠标图标
        /// </summary>
        /// <param name="e">鼠标点击事件参数</param>
        public void StartMove(Point curPos, Point targetMovePoint, double seconds)
        {
           
            this.trans.X = curPos.X;
            this.trans.Y = curPos.Y;
            this.Cursor = Cursors.None;

            //this.CaptureMouse();
              //保证有3个动画效果:x坐标，y坐标，透明度
            if (storyboard.Children.Count > 2)
            {
                try
                {
                    if (storyboard.GetCurrentTime().Milliseconds == 0)
                    {
                        ((DoubleAnimationUsingKeyFrames)storyboard.Children[0]).KeyFrames[0].Value = targetMovePoint.X;
                        ((DoubleAnimationUsingKeyFrames)storyboard.Children[1]).KeyFrames[0].Value = targetMovePoint.Y;
                        ((DoubleAnimationUsingKeyFrames)storyboard.Children[0]).KeyFrames[0].KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(seconds));
                        ((DoubleAnimationUsingKeyFrames)storyboard.Children[1]).KeyFrames[0].KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(seconds));
                        ((DoubleAnimationUsingKeyFrames)storyboard.Children[2]).KeyFrames[0].KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(seconds));
                        
                        storyboard.Begin();
                    }
                }
                catch (System.Exception ex)
                {
                    ErrorWindow errWin = new ErrorWindow(ex);
                    errWin.Show();
                }
            }
            else //如果不播放动画直接回调完成函数
            {
                if (_callback != null)
                {
                    _callback(this);
                }
            }
        }

        #endregion

    }
}