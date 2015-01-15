/*************************************************************************
** 文件名:   MsgBoxWindow.cs
** 主要类:   MsgBoxWindow
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2013-7-12
** 修改人:   
** 日  期:
** 描  述:   MsgBoxWindow类, 自定义MessageBox
**
** 版  本:   1.0.0
** 备  注:  命名及代码编写遵守C#编码规范
**
*************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace IriskingAttend.Dialog
{
    public partial class MsgBoxWindow : ChildWindow
    {
        #region 按钮、图标及返回结果枚举 

        /// <summary>
        /// 按钮枚举
        /// </summary>
        public enum MsgBtns
        {
            OK = 0,
            Cancel = 1,
            OKCancel = 2,
            YesNo = 3,
            YesNoCancel = 4,
            OKCancelApply = 5,
            RetryCancel = 6,
            AbortRetryIgnore = 7
        }

        /// <summary>
        /// 图标枚举
        /// </summary>
        public enum MsgIcon
        {
            Information = 0,
            Error = 1,
            Warning = 2,
            Question = 3,
            Succeed = 4,
            None = 5
        }

        /// <summary>
        /// 返回结果枚举
        /// </summary>
        public enum MsgResult
        {
            OK = 0,
            Cancel = 1,
            Yes = 2,
            No = 3,
            Apply = 4,
            Retry = 5,
            Abort = 6,
            Ignore = 7,
            Close = 8
        }

        #endregion     
   
        #region 变量

        //返回结果：默认为关闭
        private MsgResult _result = MsgResult.Close;

        //回调函数变量
        private MsgBoxCloseCallBack _closeCallBack;

        //回调函数声明
        public delegate void MsgBoxCloseCallBack(MsgResult result);

        //变量声明：点击的按钮与返回结果之间的对应关系
        private Dictionary<string, MsgResult> _dictBtnAndMsgResult;

        //变量声明：图标枚举与图片名称之间的对应关系
        private Dictionary<MsgIcon, string> _dictIcon;
       
        #endregion

        #region 构造函数

        public MsgBoxWindow()
        {
            InitializeComponent();

            //字典初始化：点击的按钮与返回结果之间的对应关系
            _dictBtnAndMsgResult = GetDictBtnAndMsgResult();

            //字典初始化：图标枚举与图片名称之间的对应关系
            _dictIcon = GetDictIcon();

            //窗口关闭事件
            this.Closed += new EventHandler(MsgBoxWindow_Closed);
        }

        #endregion       

        #region 事件:按钮点击事件、窗口关闭事件

        /// <summary>
        /// 窗口关闭事件：回调事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MsgBoxWindow_Closed(object sender, EventArgs e)
        {
            if (_closeCallBack != null)
            {
                _closeCallBack( this._result );
            }
        }

        /// <summary>
        /// 点击不同的按钮返回不同的结果
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            //默认结果为关闭
            this._result = MsgResult.Close;

            //字典中若包含此按钮，则返回相应的结果
            if (_dictBtnAndMsgResult.ContainsKey(btn.Name))
            {
                this._result = _dictBtnAndMsgResult[btn.Name];
            }           
            base.Close();
        }

        #endregion

        #region MsgBox的不同显示方法

        /// <summary>
        /// 参数传递包含提示语、图标、按钮三个参数
        /// </summary>       
        /// <param name="msg">提示语</param>
        /// <param name="icon">图标</param>
        /// <param name="btns">按钮</param>
        public void Show(string msg, MsgIcon icon, MsgBtns btns)
        {
            Show(msg, icon, btns, null);
        }

        /// <summary>
        /// 参数传递包含提示语、图标、按钮及回调函数四个参数
        /// </summary>      
        /// <param name="msg">提示语</param>
        /// <param name="icon">图标</param>
        /// <param name="btns">按钮</param>
        /// <param name="closeCallBack">回调函数</param>
        public void Show(string msg, MsgIcon icon, MsgBtns btns,MsgBoxCloseCallBack closeCallBack)
        {
            //默认标题为“提示”
            string title = "提示";

            //错误图标为Error，标题为“错误”
            if (icon == MsgIcon.Error)
            {
                title = "错误";
            }
            //错误图标为Warning，标题为“警告”
            else if (icon == MsgIcon.Warning)
            {
                title = "警告";
            }

            Show(title, msg, icon, btns, closeCallBack);
        }

        /// <summary>
        /// 参数传递包含标题、提示语、图标、按钮四个参数，回调函数为空
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="msg">提示语</param>
        /// <param name="icon">图标</param>
        /// <param name="btns">按钮</param>
        public void Show(string title, string msg, MsgIcon icon, MsgBtns btns)
        {
            Show(title, msg, icon, btns, null);
        }

        /// <summary>
        /// 参数传递包含标题、提示语、图标、按钮及回调函数五个参数
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="msg">提示语</param>
        /// <param name="icon">图标</param>
        /// <param name="btns">按钮</param>
        /// <param name="closeCallBack">回调函数</param>
        public void Show(string title, string msg, MsgIcon icon, MsgBtns btns, MsgBoxCloseCallBack closeCallBack)
        {
            this.txtMsgBlock.Text = msg;
            this.Title = title;

            if (closeCallBack != null)
            {
                this._closeCallBack = closeCallBack;
            }

            //字典中若包含此图标枚举，则调用相应的图片
            if (_dictIcon.ContainsKey(icon))
            {
                this.Img_Icon.Source = LoadImage(_dictIcon[icon]); 
            }

            //创建按钮
            switch (btns)
            {
                case MsgBtns.OK:
                    {
                        CreateButtons(new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("OK", "确定") });
                    }
                    break;
                case MsgBtns.Cancel:
                    {
                        CreateButtons(new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("Cancel", "取消") });
                    }
                    break;
                case MsgBtns.OKCancel:
                    {
                        CreateButtons(new KeyValuePair<string, string>[] 
                                      { new KeyValuePair<string, string>("OK", "确定"),
                                        new KeyValuePair<string, string>("Cancel", "取消")});
                    }
                    break;
                case MsgBtns.YesNo:
                    {
                        CreateButtons(new KeyValuePair<string, string>[] 
                                      { new KeyValuePair<string, string>("Yes", "是"),
                                        new KeyValuePair<string, string>("No", "否")});
                    }
                    break;
                case MsgBtns.YesNoCancel:
                    {
                        CreateButtons(new KeyValuePair<string, string>[] 
                                      { new KeyValuePair<string, string>("Yes", "是"),
                                        new KeyValuePair<string, string>("No", "否"),
                                        new KeyValuePair<string, string>("Cancel", "取消")});
                    }
                    break;
                case MsgBtns.OKCancelApply:
                    {
                        CreateButtons(new KeyValuePair<string, string>[] 
                                      { new KeyValuePair<string, string>("OK", "确定"),
                                        new KeyValuePair<string, string>("Cancel", "取消"),
                                        new KeyValuePair<string, string>("Apply", "应用")});
                    }
                    break;
                case MsgBtns.RetryCancel:
                    {
                        CreateButtons(new KeyValuePair<string, string>[] 
                                      { new KeyValuePair<string, string>("Retry", "重试"),
                                        new KeyValuePair<string, string>("Cancel", "取消")});
                    }
                    break;
                case MsgBtns.AbortRetryIgnore:
                    {
                        CreateButtons(new KeyValuePair<string, string>[] 
                                      { new KeyValuePair<string, string>("Abort", "取消"),
                                        new KeyValuePair<string, string>("Retry", "重试"),
                                        new KeyValuePair<string, string>("Ignore", "忽略")});
                    }
                    break;
            }            
            base.Show();           
        }

        #endregion

        #region 辅助函数

        /// <summary>
        /// 点击的按钮与返回结果之间的对应关系
        /// </summary>
        /// <returns>关系字典</returns>
        private Dictionary<string, MsgResult> GetDictBtnAndMsgResult()
        {
            Dictionary<string, MsgResult> dictBtnAndMsgResult = new Dictionary<string, MsgResult>();
            
            dictBtnAndMsgResult.Add("OK", MsgResult.OK);
            dictBtnAndMsgResult.Add("Cancel", MsgResult.Cancel);
            dictBtnAndMsgResult.Add("Yes", MsgResult.Yes);
            dictBtnAndMsgResult.Add("No", MsgResult.No);
            dictBtnAndMsgResult.Add("Apply", MsgResult.Apply);
            dictBtnAndMsgResult.Add("Retry", MsgResult.Retry);
            dictBtnAndMsgResult.Add("Abort", MsgResult.Abort);
            dictBtnAndMsgResult.Add("Ignore", MsgResult.Ignore);

            return dictBtnAndMsgResult;
        }

        /// <summary>
        /// 图标枚举与图片名称之间的对应关系
        /// </summary>
        /// <returns>关系字典</returns>
        private Dictionary<MsgIcon, string> GetDictIcon()
        {
            Dictionary<MsgIcon, string> dictIcon = new Dictionary<MsgIcon, string>();

            dictIcon.Add(MsgIcon.Information, "Information.png");
            dictIcon.Add(MsgIcon.Error, "Error.png");
            dictIcon.Add(MsgIcon.Warning, "Warning.png");
            dictIcon.Add(MsgIcon.Question, "Question.png");
            dictIcon.Add(MsgIcon.Succeed, "Succeed.png");
            //dictIcon.Add(MsgIcon.None, "");
            return dictIcon;
        }

        /// <summary>
        /// 创建按钮
        /// </summary>
        /// <param name="btns">欲创建的按钮列表</param>
        private void CreateButtons(KeyValuePair<string, string>[] btns)
        {
            int margin = (int)(LayoutRoot.ColumnDefinitions[0].Width.Value + LayoutRoot.ColumnDefinitions[1].Width.Value);
            margin -= (btns.Count() * 80 - 4);

            margin = margin >> 1;

            for (int index = 0; index < btns.Count(); index++)
            {
                KeyValuePair<string, string> item = btns[index];
                Button btn = new Button()
                {
                    Name = item.Key,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(margin + 80 * index, 0, 0, 12),
                    Content = item.Value,
                    Width = 76,
                    Height = 25
                };
                btn.Click += new RoutedEventHandler(Button_Click);
                LayoutRoot.Children.Add(btn);
                LayoutRoot.HorizontalAlignment = HorizontalAlignment.Center;
                Grid.SetRow(btn, 1);
                Grid.SetColumnSpan(btn, 2);
            }
        }

        /// <summary>
        ///  加载不同的图片
        /// </summary>
        /// <param name="ImageName">图片名字</param>
        /// <returns>返回Image资源</returns>
        private BitmapImage LoadImage(string ImageName)
        {
            string imageUrl = string.Format("/IriskingAttend;component/images/{0}", ImageName);
            System.IO.Stream streamInfo = Application.GetResourceStream(new Uri(imageUrl, UriKind.Relative)).Stream;

            BitmapImage image = new BitmapImage();
            image.SetSource(streamInfo);           
            return image;
        }
        #endregion

        #region  静态函数，外部调用

        /// <summary>
        /// 对话框显示,无回调 
        /// </summary>        
        /// <param name="msg">对话框提示语</param>
        /// <param name="icon">对话框图标</param>
        /// <param name="btns">对话框按钮</param>        
        public static void MsgBox( string msg, MsgBoxWindow.MsgIcon icon, MsgBoxWindow.MsgBtns btns)
        {
            MsgBox( msg, icon, btns, null);
        }

        /// <summary>
        /// 对话框显示  
        /// </summary>      
        /// <param name="msg">对话框提示语</param>
        /// <param name="icon">对话框图标</param>
        /// <param name="btns">对话框按钮</param>
        /// <param name="callBack">回调函数</param>        
        public static void MsgBox(string msg, MsgBoxWindow.MsgIcon icon, MsgBoxWindow.MsgBtns btns, MsgBoxWindow.MsgBoxCloseCallBack callBack)
        {
            MsgBoxWindow msgBox = new MsgBoxWindow();
            msgBox.Show( msg, icon, btns, callBack);
        }

        /// <summary>
        /// 对话框显示,无回调 
        /// </summary>
        /// <param name="title">对话框标题</param>
        /// <param name="msg">对话框提示语</param>
        /// <param name="icon">对话框图标</param>
        /// <param name="btns">对话框按钮</param>        
        public static void MsgBox(string title, string msg, MsgBoxWindow.MsgIcon icon, MsgBoxWindow.MsgBtns btns)
        {
            MsgBox(title, msg, icon, btns, null);
        }

        /// <summary>
        /// 对话框显示  
        /// </summary>
        /// <param name="title">对话框标题</param>
        /// <param name="msg">对话框提示语</param>
        /// <param name="icon">对话框图标</param>
        /// <param name="btns">对话框按钮</param>
        /// <param name="callBack">回调函数</param>        
        public static void MsgBox(string title, string msg, MsgBoxWindow.MsgIcon icon, MsgBoxWindow.MsgBtns btns, MsgBoxWindow.MsgBoxCloseCallBack callBack)
        {
            MsgBoxWindow msgBox = new MsgBoxWindow();
            msgBox.Show(title, msg, icon, btns, callBack);
        }
        #endregion
    }
}


