/*************************************************************************
** 文件名:   ColorConverterWuHuShan.cs
×× 主要类:   ColorConverterWuHuShan
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   wz
** 日  期:   2013-12-19
** 修改人:   
** 日  期:   
** 描  述:   VisibilityConverterGuoDian类，国电
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
using System.Windows.Data;

namespace IriskingAttend.GuoDian
{
    public class TextToVisibilityConverterGuoDian : IValueConverter
    {



        /// <summary>
        ///  字符串不为空则显示
        ///  否则则隐藏
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return Visibility.Collapsed;
            }
            string str = null;
            try
            {
                str = (string)value;
            }
            catch (Exception)
            {
                return Visibility.Collapsed;
            }

            if (!string.IsNullOrWhiteSpace(str))
            {
                return Visibility.Visible;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
