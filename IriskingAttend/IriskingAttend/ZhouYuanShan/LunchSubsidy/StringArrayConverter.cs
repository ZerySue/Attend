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

namespace IriskingAttend.ZhouYuanShan.LunchSubsidy
{
    public class StringArrayConverter : IValueConverter
    {
        private int _maxLenth = 5;
        /// <summary>
        /// 字符串显示的最大长度
        /// </summary>
        public int MaxLenth
        {
            get
            {
                return _maxLenth;
            }
            set
            {
                _maxLenth = value;
            }
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
                return "空";
            }
            Array array = value as Array;
            if (array == null)
            {
                return "非数组";
            }

            if (array.Length == 0)
            {
                return "空";
            }
            StringBuilder sb = new StringBuilder();
            bool isBreak = false;
            foreach (var item in array)
            {
                if (sb.Length + item.ToString().Length <= MaxLenth)
                {
                    sb.Append(item);
                    sb.Append("、");
                }
                else
                {
                    isBreak = true;
                    break;
                }
            }

            if (sb.Length > 0)
            {
                sb.Remove(sb.Length - 1, 1);
            }
            

            if (isBreak)
            {
                sb.Append("…");
            }
            return sb.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
