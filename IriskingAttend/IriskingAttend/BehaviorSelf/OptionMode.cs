/*************************************************************************
** 文件名:   ChildWndOptionMode.cs
×× 主要类:   ChildWndOptionMode
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   wz
** 日  期:   2013-7-23
** 修改人:   
** 日  期:
** 描  述:   子窗口的操作模式枚举类
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

namespace IriskingAttend.BehaviorSelf
{
    /// <summary>
    /// 子窗口操作模式枚举类
    /// </summary>
    public enum ChildWndOptionMode
    {
        /// <summary>
        /// 修改模式
        /// </summary>
        Modify,
        /// <summary>
        /// 删除模式
        /// </summary>
        Delete,
        /// <summary>
        /// 查看模式
        /// </summary>
        Check,
        /// <summary>
        /// 添加模式
        /// </summary>
        Add,
        /// <summary>
        /// 查看记录模式
        /// </summary>
        Record,
        /// <summary>
        /// 停用虹膜模式
        /// </summary>
        StopIris
    }

    /// <summary>
    /// 血型枚举类型
    /// </summary>
    public enum BloodTypeEnum
    {
        A,
        B,
        AB,
        O,
        Unkown,
        Other
    }


    public class BloodType
    {
        private  BloodTypeEnum _bloodType;


        public BloodType(BloodTypeEnum _bloodTypeEnum)
        {
            _bloodType = _bloodTypeEnum;
        }

        public BloodType(string _bloodTypeEnum)
        {
            switch (_bloodTypeEnum)
            {
                case "A":
                    _bloodType = BloodTypeEnum.A;
                    break;
                case "B":
                    _bloodType = BloodTypeEnum.B;
                    break;
                case "AB":
                    _bloodType = BloodTypeEnum.AB;
                    break;
                case "O":
                    _bloodType = BloodTypeEnum.O;
                    break;
                case "其他":
                    _bloodType = BloodTypeEnum.Other;
                    break;
                default :
                    _bloodType = BloodTypeEnum.Unkown;
                    break;
            }
        }

        public override string ToString()
        {
            switch (_bloodType)
            {
                case BloodTypeEnum.A:
                    return "A";
                case BloodTypeEnum.B:
                    return "B";
                case BloodTypeEnum.AB:
                    return "AB";
                case BloodTypeEnum.O:
                    return "O";
                case BloodTypeEnum.Other:
                    return "其他";
                default:
                    return "未知";
            }
        }

    }


}
