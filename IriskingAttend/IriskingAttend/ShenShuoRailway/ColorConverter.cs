/*************************************************************************
** 文件名:   ColorConverter.cs
×× 主要类:   ColorConverter
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2013-12-19
** 修改人:   
** 日  期:   
** 描  述:   ColorConverter类，颜色转换
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

namespace IriskingAttend.ShenShuoRailway
{
    public class ColorConverter: IValueConverter
    {
        public const int NormalDay = 0;
        public const int ShiftDay = 1;
        public const int WeekendDay = 2;
        public const int FestivalDay = 3;

        public Brush WeekendBrush
        {
            get;
            set;
        }

        public Brush FestivalBrush
        {
            get;
            set;
        }

        public Brush DefaultBrush
        {
            get;
            set;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return DefaultBrush;
            }
            int colorIndex = (int)value;

            if (colorIndex == WeekendDay )
            {
                return WeekendBrush;
            }
            else if( colorIndex == FestivalDay )
            {
                return FestivalBrush;
            }
            else
            {
                return DefaultBrush;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
