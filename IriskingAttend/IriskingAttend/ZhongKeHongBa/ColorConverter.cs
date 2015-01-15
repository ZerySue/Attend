using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;

namespace IriskingAttend.ZhongKeHongBa
{
    public class ColorConverter : IValueConverter
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

            if (colorIndex == WeekendDay)
            {
                return WeekendBrush;
            }
            else if (colorIndex == FestivalDay)
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
