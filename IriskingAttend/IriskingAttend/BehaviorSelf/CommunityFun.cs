/*************************************************************************
** 文件名:   CommunityFun.cs
×× 主要类:   WaitingDialog
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   lzc
** 日  期:   2013-4-8
** 修改人:   
** 日  期:   
 *修改内容： 
** 描  述:   WaitingDialog类，显示关闭等待对话框
**
** 版  本:   1.0.0
** 备  注:  命名及代码编写遵守C#编码规范
**
 * ***********************************************************************/

using System;
using System.Windows;
using System.Windows.Controls;
using IriskingAttend.Dialogs;
using IriskingAttend.ViewModel;
using IriskingAttend.Web;
using System.Windows.Media.Imaging;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Text;
using IriskingAttend.Dialog;
using System.ServiceModel.DomainServices.Client;
using EDatabaseError;
using System.Reflection;
using System.Text.RegularExpressions;
namespace IriskingAttend.Common
{
    //#region 等待对话框
    ///// <summary>
    ///// 该类功能为弹出和关闭加载页面对话框
    ///// </summary>
    //public class WaitingDialog
    //{
    //    /// <summary>
    //    /// 等待对话框
    //    /// </summary>
    //    private static DialogWaiting _waiting;

    //    /// <summary>
    //    /// 静态构造函数
    //    /// </summary>
    //    static WaitingDialog()
    //    {
    //        if (_waiting == null)
    //        {
    //            _waiting = new DialogWaiting();
    //        }
    //    }

    //    /// <summary>
    //    /// 显示等待信息 by wz
    //    /// 并且在显示等待消息成功后invoke回调函数
    //    /// </summary>
    //    /// <param name="message">要显示的消息</param>
    //    /// <param name="milliSeconds">执行回调函数的等待时间</param>
    //    /// <param name="callBack">回调函数委托</param>
    //    public static void ShowWaiting(string message,int milliSeconds,Action callBack)
    //    {
    //        try
    //        {
    //            if (_waiting != null)
    //            {
    //                _waiting.Visibility = Visibility.Visible;
    //                _waiting.SetLoadingText(message);
    //                _waiting.Show();
    //            }
    //            else
    //            {
    //                _waiting = new DialogWaiting();
    //                ShowWaiting(message);
    //            }
    //        }
    //        catch (Exception e)
    //        {
    //            string str = e.Message.ToString();
    //        }

    //        Action threadAction = ()=>
    //            {
    //                Thread.Sleep(milliSeconds);
    //                if (callBack != null)
    //                {
    //                    _waiting.Dispatcher.BeginInvoke(callBack); 
    //                }

    //            };

    //        Thread th = new Thread(new ThreadStart(threadAction));
    //        th.Start();

    //    }
      
    //    /// <summary>
    //    /// 显示等待信息
    //    /// </summary>
    //    /// <param name="message">显示等待提示语</param>
    //    public static void ShowWaiting(string message)
    //    {
    //        try
    //        {
    //            if (_waiting != null)
    //            {

    //                _waiting.Visibility = Visibility.Visible;
    //                _waiting.SetLoadingText(message);
    //                _waiting.Show();

    //            }
    //            else
    //            {
    //                _waiting = new DialogWaiting();
    //                ShowWaiting(message);
    //            }
    //        }
    //        catch (Exception e)
    //        {
    //            string str = e.Message.ToString();
    //        }
    //    }

      
    //    /// <summary>
    //    /// 显示等待信息-默认提示语
    //    /// </summary>
    //    public static void ShowWaiting()
    //    {
    //        try
    //        {
    //            if (_waiting != null)
    //            {   
    //                _waiting.SetLoadingText();
    //                _waiting.Visibility = Visibility.Visible;
    //                _waiting.Show();
    //            }
    //            else
    //            {
    //                _waiting = new DialogWaiting();
    //                ShowWaiting();
    //            }
    //        }
    //        catch (Exception e)
    //        {
    //            string str = e.Message.ToString();
    //        }
    //    }

    //    /// <summary>
    //    /// 隐藏等待信息
    //    /// </summary>
    //    public static void HideWaiting()
    //    {
    //        if (_waiting != null)
    //        {
    //            _waiting.Close();
    //        }
    //    }
    //}
    //#endregion

    #region  数学函数 
    /// <summary>
    /// 定义数学函数类--以后如果有需要使用的数学函数可以向改类中进行增加
    /// 创建者：lzc
    /// </summary>
    public static class mathFun
    {

        /// <summary>
        /// 将字符串转换成Color类型
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Color ReturnColorFromString(string color)
        {
            color = color.Substring(2, color.Length - 2);
            string alpha = color.Substring(0, 2);
            string red = color.Substring(2, 2);
            string green = color.Substring(4, 2);
            string blue = color.Substring(6, 2);
            try
            {
                byte alphaByte = Convert.ToByte(alpha, 16);
                byte redByte = Convert.ToByte(red, 16);
                byte greenByte = Convert.ToByte(green, 16);
                byte blueByte = Convert.ToByte(blue, 16);
                return Color.FromArgb(alphaByte, redByte, greenByte, blueByte);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            return Colors.Black;
        }

        /// <summary>
        /// 将IP转换为Int形式
        /// </summary>
        /// <param name="ip">IP字符创</param>
        /// <returns>转换后int形式的IP</returns>
        public static int ipToInt(string ip)
        {
            //返回结果
            int result = 0;
            //中间变量
            int temp = 0;
            //字符串索引
            int index = 0;
  
            //转换计算
            while (index < ip.Length)
            {
                if ('.' == ip[index])
                {
                    result += temp;
                    result = result << 8;
                    temp = 0;
                }
                else
                {
                    temp = temp * 10 + ip[index] - '0';
                }
                index++;
            }

            return result += temp;
        }
     
    }

    #endregion

    #region 排序函数

    /// <summary>
    /// 自定义时间字符比较  string类型为例如  "15653:14" 样式的字符串比较
    /// 应用时间排序（井下时间）
    /// </summary>
    public class SpecialComparer : IComparer<string>
    {
        public int Compare(string d1, string d2)
        {
            double fractional1, fractional2;

            try
            {
                fractional1 = double.Parse(d1.Replace(':', '.'));
            }
            catch (DivideByZeroException)
            {
                fractional1 = 0;
            }
            try
            {
                fractional2 = double.Parse(d2.Replace(':', '.'));
            }
            catch (DivideByZeroException)
            {
                fractional2 = 0;
            }

            if (fractional1 == fractional2)
            {
                return 0;
            }
            else if (fractional1 > fractional2)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }
    }

    /// <summary>
    /// DataGridSort按汉语拼音排序
    /// </summary>
    public class MyDataGridSortInChinese
    {
        //排序模式
        enum SortMode
        {
            Ascending,
            Descending
        }

        //排序模式
        private static SortMode _sortMode;
        //排序字段
        private static string _sortFiled;
        //排序DataGrid 头
        private static DataGridColumnHeader _sortHeader;
        private static DataGridColumnHeader _lastSortHeader;

        /// <summary>
        /// 初始化
        /// </summary>
        public MyDataGridSortInChinese()
        {
            _sortMode = SortMode.Ascending;
            _sortFiled = "";
            _sortHeader = new DataGridColumnHeader();
        }

        
        ///// <summary>
        ///// 排序
        ///// </summary>
        ///// <typeparam name="T">需要排序的类</typeparam>
        ///// <param name="sender">需要排序的DataGrid</param>
        ///// <param name="e">事件</param>
        ///// <param name="data">需要排序的数据源</param>
        //public static void OrderDataForSpecial<T>(object sender, MouseButtonEventArgs e, BaseViewModelCollection<T> data, IComparer<string> comparer)
        //{
        //    IOrderedEnumerable<T> sortedObject = data.OrderByDescending(u => u.GetType().GetProperties().Where(p => 
        //        p.Name == _sortFiled).Single().GetValue(u, null).GetType());
        //}

        /// <summary>
        /// 排序
        /// </summary>
        /// <typeparam name="T">需要排序的类</typeparam>
        /// <param name="sender">需要排序的DataGrid</param>
        /// <param name="e">事件</param>
        /// <param name="data">需要排序的数据源</param>
        public static void OrderData<T>(object sender, MouseButtonEventArgs e, BaseViewModelCollection<T> data)
        {    
            DataGrid dg = sender as DataGrid;
            if (data == null || dg == null) //非空判断 by wz
            {
                return;
            }

            try
            {
                //获取界面元素
                var uiElement = from element in VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(null), (System.Windows.UIElement)null)
                         where element is DataGridColumnHeader
                         select element;

               

                if (uiElement.Count() > 0)
                {
                    //鼠标点击的ColumnHeader 
                    DataGridColumnHeader header = (DataGridColumnHeader)uiElement.First();
                    
                    if (header.Content == null)
                    {
                        return;
                    }
                    //要排序的字段 
                    string newSort = header.Content.ToString();

                    foreach (var col in dg.Columns.Where( (columnItem)=>    //修改by wz 判断列头为空的情况
                    {
                        if (columnItem.Header == null)
                        {
                            return false;
                        }
                        return  columnItem.Header.ToString() == newSort;
                    }))

                    {
                        //如果有绑定数据
                        if (col.ClipboardContentBinding != null &&
                            col.ClipboardContentBinding.Path != null &&
                            !col.ClipboardContentBinding.Path.Path.Equals(""))
                        {
                                _sortFiled = col.ClipboardContentBinding.Path.Path;
                                break;
                        }
                        return;
                    }

                    //判断升降序
                    if (_sortMode == SortMode.Descending)
                    {
                        _sortMode = SortMode.Ascending;
                    }
                    else
                    {
                        _sortMode = SortMode.Descending;
                    }

                    _lastSortHeader = _sortHeader;
                    _sortHeader = header;
                  
                    //判断 该元素是否能够作为排序
                    if (data.Count > 0)
                    {
                        PropertyInfo sortElementInfo = data[0].GetType().GetProperties().SingleOrDefault(p => p.Name == _sortFiled);
                        object sortElement = sortElementInfo.GetValue(data[0], null);
                        if (!(sortElement is IComparable))
                        {
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }

                    //进行排序
                    if (_sortMode == SortMode.Descending)//
                    {
                        
                        IOrderedEnumerable<T> sortedObject = data.OrderByDescending(u => u.GetType().GetProperties().Where(p =>
                            p.Name == _sortFiled).Single().GetValue(u, null));
                        T[] sortedData = sortedObject.ToArray();  //执行这一步之后，排序的linq表达式才有效   
                        data.Clear();
                        foreach (var item in sortedData)
                        {
                            data.Add(item);
                        }
                    }
                    else //进行降序排序
                    {
                        IOrderedEnumerable<T> sortedObject = data.OrderBy(u => u.GetType().GetProperties().Where(p =>
                            p.Name == _sortFiled).Single().GetValue(u, null));
                        T[] sortedData = sortedObject.ToArray();  //执行这一步之后，排序的linq表达式才有效
                        data.Clear();
                        foreach (var item in sortedData)
                        {
                            data.Add(item);
                        }
                    }
                  

                }

                
            }
            catch (Exception ee)
            {
                ErrorWindow err = new ErrorWindow(ee);
                err.Show();
            }
        }


     
        /// <summary>
        /// 显示排序的上下箭头 
        /// </summary>
        public static  void SetColumnSortState()
        {
            if (_sortHeader == null)
            {
                return;
            }
            object a = VisualStateManager.GetVisualStateGroups(_sortHeader);
            object b= VisualStateManager.GetCustomVisualStateManager(_sortHeader);
           

            if (_sortMode == SortMode.Ascending)
            {
               
                VisualStateManager.GoToState(_sortHeader, "SortAscending", false);
            }
            else
            {
                VisualStateManager.GoToState(_sortHeader, "SortDescending", false);
            }

            if (_lastSortHeader != null && _lastSortHeader != _sortHeader)
            {
                VisualStateManager.GoToState(_lastSortHeader, "Unsorted", false);
                _lastSortHeader.UpdateLayout();
            }

            _sortHeader.UpdateLayout();
           
        }
    } 
    #endregion


}