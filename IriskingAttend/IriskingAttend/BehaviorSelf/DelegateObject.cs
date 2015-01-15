/*************************************************************************
** 文件名:   DelegateObject.cs
×× 主要类:   DelegateObject
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   lzc
** 日  期:   2013-4-8
** 修改人:   
** 日  期:
** 描  述:   DelegateObject类，用来支持多参传值
**
** 版  本:   1.0.0
** 备  注:  命名及代码编写遵守C#编码规范
**
 * ***********************************************************************/

using System.ComponentModel;
using System.Windows.Markup;
using System.Windows;
using System;

namespace IriskingAttend.BehaviorSelf
{
    /// <summary>
    /// 通过键值对绑定多个参数
    /// </summary>
    [ContentProperty("Value")]
    public class DelegateObject : DependencyObject, IEquatable<DelegateObject>
    {
        public DelegateObject() { }

        public DelegateObject(string key, Object value)
        {
            this.Key = key;
            this.Value = value;
        }

        private const string _key_set_notNull = "绑定参数的key不能为空";

        #region DependencyProperty

        /// <summary>
        /// 键属性
        /// </summary>
        public static readonly DependencyProperty KeyProperty = DependencyProperty.Register("Key", 
                                                    typeof(string), typeof(DelegateObject), 
                                                    new PropertyMetadata(null, KeyPropertyChanged));
        /// <summary>
        /// 值属性
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value",
                                                     typeof(Object), typeof(DelegateObject),
                                                     new PropertyMetadata(null));

        /// <summary>
        ///  键值属性改变
        /// </summary>
        /// <param name="o">send</param>
        /// <param name="e">args</param>
        private static void KeyPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Convert.ToString(e.NewValue)) 
                && string.IsNullOrEmpty(Convert.ToString(e.NewValue)))
            {
                throw new Exception(_key_set_notNull);
            }
        }

        [Category("公共属性")]
        [Description("不区分大小写")]
        public string Key
        {
            get { return Convert.ToString(GetValue(KeyProperty)); }
            set { SetValue(KeyProperty, value); }
        }

        [Category("公共属性")]
        public Object Value
        {
            get { return GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        #endregion

        /// <summary>
        /// 格式转换
        /// </summary>
        /// <returns>按键值对形式返回字符串</returns>
        public override string ToString()
        {
            return string.Format("({0},{1})", this.Key, this.Value);
        }

        #region IEquatable接口实现

        /// <summary>
        /// 判断是否相等
        /// </summary>
        /// <param name="other">要比较的值</param>
        /// <returns>是否相等</returns>
        public bool Equals(DelegateObject other)
        {
            if (other == null)
            {
                return false;
            }
            return string.Equals(Key, other.Key, StringComparison.InvariantCultureIgnoreCase) && Value == other.Value;
        }

        #endregion
    }
}