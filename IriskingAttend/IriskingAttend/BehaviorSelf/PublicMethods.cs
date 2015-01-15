/*************************************************************************
** 文件名:   PublicMethods.cs
** 主要类:   PublicFunction
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2013-7-9
** 修改人:   wz
** 日  期:   2013-8-2
** 描  述:   PublicMethods类,公共函数
**
** 版  本:   1.0.0
** 备  注:  命名及代码编写遵守C#编码规范
**
*************************************************************************/
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
using System.Text.RegularExpressions;
using IriskingAttend.Dialog;
using System.Collections.Generic;
using System.Windows.Data;
using System.Linq;
using Irisking.Web.DataModel;
using System.Text;

namespace IriskingAttend
{
    public class PublicMethods
    {
        /// <summary>
        /// 判断输入的键是否为数字 by wz
        /// </summary>
        /// <param name="e">键盘枚举值</param>
        /// <returns></returns>
        public static bool IsKeyNumber(Key e)
        {

            if (e != Key.NumPad0 && e != Key.NumPad1 &&
                e != Key.NumPad2 && e != Key.NumPad3 &&
                e != Key.NumPad4 && e != Key.NumPad5 &&
                e != Key.NumPad6 && e != Key.NumPad7 &&
                e != Key.NumPad8 && e != Key.NumPad9 &&
                e != Key.D0 && e != Key.D1 &&
                e != Key.D2 && e != Key.D3 &&
                e != Key.D4 && e != Key.D5 &&
                e != Key.D6 && e != Key.D7 &&
                e != Key.D8 && e != Key.D9)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 判断输入的键是否为“.” by wz
        /// </summary>
        /// <param name="e">键盘事件参数</param>
        /// <returns></returns>
        public static  bool IsKeyDecimall(KeyEventArgs e)
        {
             if ((e.Key == Key.Unknown && e.PlatformKeyCode == 190) ||   //190代表中键盘中的"."键位
                e.Key == Key.Decimal)                                   //Key.Decimal代表小键盘中的"."键位
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 判断路径是否合法 by gqy
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool CheckPath(string path)
        {
            string pattern = @"^[a-zA-Z]:(((\\(?! )[^/:*?<>\""|\\]+)+\\?)|(\\)?)\s*$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(path);
        }

        /// <summary>
        /// 输入是否合法判断 by gqy
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static bool IsInputValid( string Input)
        {
            Regex regExp = new Regex("[~!@#$%^&*()=+[\\]{}''\";:/?.,><`|！·￥…—（）\\-、；：。，》《]");
            return !regExp.IsMatch(Input);
        }

        /// <summary>
        /// 判断一个字符串是否是数字格式
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNumberString(string str)
        {
            if (str == null || str == "")
            {
                return false;
            }
            Regex objNotNumberPattern = new Regex("[^0-9.-]");
            Regex objTwoDotPattern = new Regex("[0-9]*[.][0-9]*[.][0-9]*");
            Regex objTwoMinusPattern = new Regex("[0-9]*[-][0-9]*[-][0-9]*");
            String strValidRealPattern = "^([-]|[.]|[-.]|[0-9])[0-9]*[.]*[0-9]+$";
            String strValidIntegerPattern = "^([-]|[0-9])[0-9]*$";
            Regex objNumberPattern = new Regex("(" + strValidRealPattern + ")|(" + strValidIntegerPattern + ")");

            return (!objNotNumberPattern.IsMatch(str) && !objTwoDotPattern.IsMatch(str) &&
                    !objTwoMinusPattern.IsMatch(str) && objNumberPattern.IsMatch(str));
        }

        /// <summary>
        /// 获得设备类型数字指示与设备类型字符之间的对应关系  by gqy
        /// </summary>
        /// <param name="isMine">是否为矿山</param>
        /// <returns>设备类型数字指示与设备类型字符之间的字典对应关系</returns>
        public static Dictionary<int, string> GetDevTypeDictionary( bool isMine )
        {
            Dictionary<int, string> dictDevType = new Dictionary<int, string>();     

            //往字典内添加数字指示与设备类型字符之间的对应关系，矿山的有六种，非矿的有三种
            if (isMine)
            {
                dictDevType.Add(1, "入井");
                dictDevType.Add(2, "出井");
                dictDevType.Add(3, "出入井");
            }
            dictDevType.Add(4, "上班");
            dictDevType.Add(5, "下班");
            dictDevType.Add(6, "上下班");    

            return dictDevType;
        }
        
        /// <summary>
        /// 为了方便后台sql语句的组装，提供一个字符串转换函数 
        /// by wz
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static string ToString(object o)
        {
            if (o == null) return "null";
            if (o.GetType().Equals(typeof(string)))
            {
                return "'" + o.ToString().Trim() + "'";
            }
            else if (o.GetType().Equals(typeof(int)) || o.GetType().Equals(typeof(long)))
            {
                if ((int)o == -1)
                {
                    return "null";
                }
                else
                {
                    return o.ToString();
                }

            }
            else if (o.GetType().Equals(typeof(DateTime)))
            {
                if ((DateTime)o == DateTime.MinValue)
                {
                    return "null";
                }
                else
                {
                    return "'" + o.ToString() + "'";
                }
            }
            return o.ToString();
        }

        /// <summary>
        /// 手动绑定整个树  by wz
        /// 需要在树初始化完成之后调用该函数（建议用BeginInvoke函数来调用该函数)
        /// 每个节点的4个属性：
        /// IsExpandedProperty，IsSelectedProperty，ForegroundProperty，VisibilityProperty
        /// </summary>
        /// <param name="node"></param>
        public static void BindingAllTree(TreeView node)
        {
            if (node == null)
            {
                return;
            }

            //激活树,用于遍历函数GetDescendantContainers
            //遍历所有树节点Containers
            //如果不激活，则只能遍历最上层节点
            node.ExpandAll();
            node.UpdateLayout();
            node.CollapseAll();


            IEnumerable<TreeViewItem> _treeChildren = TreeViewExtensions.GetDescendantContainers((TreeView)node);
            int cout = _treeChildren.Count();
            foreach (TreeViewItem item in _treeChildren)
            {
                //手动绑定
                //绑定isExpended属性
                Binding bingding = new Binding();
                bingding.Path = new System.Windows.PropertyPath("IsOpen");
                bingding.Source = item.DataContext;
                bingding.Mode = BindingMode.TwoWay;
                item.SetBinding(TreeViewItem.IsExpandedProperty, bingding);

                //绑定isSelected属性
                bingding = new Binding();
                bingding.Path = new System.Windows.PropertyPath("IsChecked");
                bingding.Source = item.DataContext;
                bingding.Mode = BindingMode.TwoWay;
                item.SetBinding(TreeViewItem.IsSelectedProperty, bingding);

                //绑定ForegroundProperty属性
                bingding = new Binding();
                bingding.Path = new System.Windows.PropertyPath("Foreground");
                bingding.Source = item.DataContext;
                item.SetBinding(TreeViewItem.ForegroundProperty, bingding);

                //绑定Visibility属性
                bingding = new Binding();
                bingding.Path = new System.Windows.PropertyPath("Visibility");
                bingding.Source = item.DataContext;
                item.SetBinding(TreeViewItem.VisibilityProperty, bingding);
            }
        }
        /// <summary>
        /// 按照树形结构排列部门信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static void OrderDepartByTreeStructure(List<UserDepartInfo> input, List<UserDepartInfo> output, int parentDepartId, string tabChars,int level)
        {
            //寻找当前深度的部门
            IEnumerable<UserDepartInfo> departsTmp = input.Where<UserDepartInfo>((info) =>
            {
                if (info.parent_depart_id == parentDepartId)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            });

            if (departsTmp == null)
            {
                return;
            }

            StringBuilder sbTabSpace = new StringBuilder();
            for (int i = 0; i < level; i++)
            {
                sbTabSpace.Append(tabChars);
            }
            
            foreach (var item in departsTmp)
            {
                item.depart_name = sbTabSpace.ToString() +item.depart_name;
                output.Add(item);
                int nextParentDepartId = item.depart_id;
                OrderDepartByTreeStructure(input, output, nextParentDepartId, tabChars,level+1);
            }
        }

        /// <summary>
        /// 按照树形结构排列部门信息
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <param name="parentDepartId"></param>
        public static void OrderDepartByTree(List<UserDepartInfo> input, List<UserDepartInfo> output, int parentDepartId,string tabChars)
        {
            tabChars = tabChars == null ? "" : tabChars;
            OrderDepartByTreeStructure(input, output, parentDepartId, tabChars, 0);
        }

        /// <summary>
        /// 按照权限过滤部门
        /// </summary>
        /// <param name="input"></param>
        /// <param name="departIds"></param>
        /// <returns></returns>
        public static List<UserDepartInfo> FiterDepartById(List<UserDepartInfo> input, List<int> departIds)
        {
            List<UserDepartInfo> res = new List<UserDepartInfo>();
            foreach (var item in input)
            {
                if (departIds.Contains(item.depart_id))
                {
                    res.Add(item);
                }
            }

            return res;
        }

        /// <summary>
        /// 添加双击事件  gqy
        /// </summary>
        /// <typeparam name="TControl"></typeparam>
        /// <param name="sender"></param>
        /// <returns></returns>
        public static bool IsUserControlDoubleClicked<TControl>(TControl sender)
            where TControl : Control
        {
            TimeSpan t = DateTime.Now.TimeOfDay;
            if (sender.Tag == null)
            {
                sender.Tag = t;
                return false;
            }

            TimeSpan oldT = (TimeSpan)sender.Tag;
            sender.Tag = t;

            if ((t - oldT) >= TimeSpan.FromMilliseconds(300))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 从原字符串中去掉前置的指定字符
        /// </summary>
        /// <param name="source"></param>
        /// <param name="toBeMovedChar"></param>
        /// <returns></returns>
        public static string RemoveAtStart(string source, char toBeMovedChar)
        {
            StringBuilder res = new StringBuilder();
            bool isAtStart = true;
            foreach (var item in source)
            {
                if (isAtStart)
                {
                    if (item == toBeMovedChar)
                    {
                        continue;
                    }
                    else
                    {
                        res.Append(item);
                        isAtStart = false;
                    }
                }
                else
                {
                    res.Append(item);
                }
            }
            return res.ToString();
        }
    }
}
