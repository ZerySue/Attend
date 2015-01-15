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
using System.Text;
using IriskingAttend.Common;

namespace IriskingAttend.ZhouYuanShan.LunchSubsidy
{
    public class ColorToBrushConverter : IValueConverter
    {
       

        
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string valueStr = value as string;
            //0xff000000格式的颜色
            //0xARGB
            Color c = mathFun.ReturnColorFromString(valueStr);
            SolidColorBrush res = new SolidColorBrush(c);
            return res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
