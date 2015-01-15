/*************************************************************************
** 文件名:   BloodConvertor.cs
×× 主要类:   BloodConvertor
**  
** Copyright (c) 北京中科虹霸科技有限公司
** 创建人:   wz
** 日  期:   2013-8-13
** 修改人:   
** 日  期:   
 *修改内容： 
** 描  述:   血型索引转换
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
using System.Collections.Generic;
using System.Collections;

namespace IriskingAttend.BehaviorSelf
{

    public interface IrisConverter
    {
        string IndexToString(int Index);
        int StingToIndex(string name);
    }

    //血型转换器
    public class BloodConverter : IrisConverter
    {
        private static Dictionary<int,string> _dic = new Dictionary<int,string>();

        static BloodConverter()
        {
            _dic.Add(0, "不详");
            _dic.Add(1, "a");
            _dic.Add(2, "b");
            _dic.Add(3, "ab");
            _dic.Add(4, "o");
            _dic.Add(5, "其他");
        }
        /// <summary>
        ///  血型按：不详，a,b,ab,o,其他排列
        ///  与前台界面对应
        /// </summary>
        /// <param name="Index"></param>
        /// <returns></returns>
        public string IndexToString(int Index)
        {
            return _dic[Index];
        }

        /// <summary>
        /// 血型按：不详，a,b,ab,o,其他排列
        /// 与前台界面对应
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int StingToIndex(string name)
        {
            if (name == null)
            {
                return 0;
            }
            foreach (var item in _dic)
            {
                if (item.Value == name.Trim())
                {
                    return item.Key;
                }
            }
            return 0;
        }


    }

    /// <summary>
    /// 性别转换器： 0=男，1=女
    /// </summary>
    public class SexConverTer:IrisConverter
    {
        public int StingToIndex(string name)
        {
            if (name == "男")
            {
                return 0;
            }
            return 1;
        }

        public string IndexToString(int index)
        {
            if (index == 0)
            {
                return "男";
            }
            return "女";
        }
    }

}
