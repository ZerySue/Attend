/*************************************************************************
** 文件名:   ColorConverterWuHuShan.cs
×× 主要类:   ColorConverterWuHuShan
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   wz
** 日  期:   2013-12-19
** 修改人:   
** 日  期:   
** 描  述:   ColorConverterWuHuShan类，五虎山异常考勤 考勤状态 内容转换
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

namespace IriskingAttend.NewWuHuShan
{
    public class AttendStateConverterWuHuShan : IValueConverter
    {
       
        public string Content1
        {
            get;
            set;
        }

        public string Content2
        {
            get;
            set;
        }

        public string Content3
        {
            get;
            set;
        }

        public string Content4
        {
            get;
            set;
        }

        public string DefaultContent
        {
            get;
            set;
        }

        /// 考勤状态
        /// 0 = 正常
        /// 1 = 未完成出入井虹膜， 但出入井定位信息都有
        /// 2 = 未完成出入井定位，但出入井虹膜都有
        /// 3 = 出入井虹膜和出入井定位都有，但时间限制不满足
        /// 4 = 出入井虹膜和出入井定位都残缺
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return DefaultContent;
            }

            int colorIndex = System.Convert.ToInt32(value);

            switch (colorIndex)
            {
                default:
                    return DefaultContent;
                case 0:
                    return DefaultContent;
                case 1:
                    return Content1;
                case 2:
                    return Content2;
                case 3:
                    return Content3;
                case 4:
                    return Content4;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
