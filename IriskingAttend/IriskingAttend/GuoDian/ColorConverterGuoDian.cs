/*************************************************************************
** 文件名:   ColorConverterWuHuShan.cs
×× 主要类:   ColorConverterWuHuShan
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   wz
** 日  期:   2013-12-19
** 修改人:   
** 日  期:   
** 描  述:   ColorConverterWuHuShan类，五虎山异常考勤 考勤状态 颜色转换
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
    public class ColorConverterGuoDian : IValueConverter
    {



        /// <summary>
        /// 每日考勤状态
        ///正常为0
        ///缺上下班为1
        ///缺上班为2
        ///缺下班为3
        ///时长不够为4
        ///超时为5
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return new SolidColorBrush(Colors.Black);
            }
            int colorIndex = 0;
            try
            {
                colorIndex = (int)value;
            }
            catch (Exception)
            {
                return new SolidColorBrush(Colors.Black);
            }
            

            switch (colorIndex)
            {
                case 0:
                    return new SolidColorBrush(Colors.Black);
                case 1:
                    return new SolidColorBrush(Colors.Black);
                case 2:
                    return new SolidColorBrush(Colors.Black);
                case 3:
                    return new SolidColorBrush(Colors.Black);
                case 4:
                    return new SolidColorBrush(Colors.Black);
                case 5:
                    return new SolidColorBrush(Colors.Purple);
            }
            return new SolidColorBrush(Colors.Black);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
