/*************************************************************************
** 文件名:   VmDevType.cs
×× 主要类:   VmDevType
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   lzc
** 日  期:   2013-7-22
** 修改人:   
** 日  期:
** 描  述:   VmDevType类，按照设备类型区分软件应用环境--矿山、非矿山
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
using System.ServiceModel.DomainServices.Client;
using EDatabaseError;
using System.Collections.Generic;

namespace IriskingAttend.ViewModel.AttendViewModel
{
    public class VmDevType
    {
        #region 绑定数据
        /// <summary>
        /// 设备类型
        /// </summary>
        //public List<string> DevType;

        /// <summary>
        /// 键值对枚举类型DevTypeENUM 对应与名称（需要绑定到界面显示）
        /// </summary>
        public Dictionary<DevTypeENUM, string> DevType = new Dictionary<DevTypeENUM, string>();

        /// <summary>
        /// 包含“全部”、“出入井”、“上下班”--查询界面绑定
        /// </summary>
        public Dictionary<DevTypeENUM, string> DevTypeAndAll = new Dictionary<DevTypeENUM, string>();

        #endregion

        #region 构造函数
        public VmDevType()
        {
            GetIsMine();
        }
        #endregion

        #region 获取当前设备类型类表

        private void GetIsMine()
        {
            ///通过VmLogin的静态函数获取是否为矿山单位
            if (VmLogin.GetIsMineApplication())
            {
                ///矿山配置
                DevType.Add(DevTypeENUM.INWELL, "入井");
                DevType.Add(DevTypeENUM.OUTWELL, "出井");
                DevType.Add(DevTypeENUM.INOUTWELL, "出入井");
                DevType.Add(DevTypeENUM.ONDUTY, "上班");
                DevType.Add(DevTypeENUM.OFFDUTY, "下班");
                DevType.Add(DevTypeENUM.ONOFFDUTY, "上下班");

                ///查询界面显示绑定
                DevTypeAndAll.Add(DevTypeENUM.NULL, "全部");
                DevTypeAndAll.Add(DevTypeENUM.INOUTWELL, "出入井");
                DevTypeAndAll.Add(DevTypeENUM.ONOFFDUTY, "上下班");
            }
            else
            {
                ///非矿山配置
                DevType.Add(DevTypeENUM.ONDUTY, "上班");
                DevType.Add(DevTypeENUM.OFFDUTY, "下班");
                DevType.Add(DevTypeENUM.ONOFFDUTY, "上下班");
            }
        }
        #endregion
        #region 函数
        /// <summary>
        /// 将枚举类型值转换为Int值
        /// </summary>
        /// <param name="devType"></param>
        /// <returns></returns>
        public static int DevTypeENUMToInt(DevTypeENUM devType)
        {
            return (int)devType;
        }

        /// <summary>
        /// 将键值对的键值转换为int值
        /// </summary>
        /// <param name="o">键值对</param>
        /// <returns></returns>
        public static int KeyValuePairToInt(object o)
        {
            if (o is KeyValuePair<DevTypeENUM, string>)
            {
                return DevTypeENUMToInt(((KeyValuePair<DevTypeENUM, string>)(o)).Key);
            }
            return -1;
        }

        /// <summary>
        /// 将枚举类型值转换为Int值
        /// </summary>
        /// <param name="devType"></param>
        /// <returns></returns>
        public static int[] DevTypeENUMToIntArray(int devType)
        {
            int[] devArray = new int[3];
            devArray[0] = devType - 2;
            devArray[1] = devType - 1;
            devArray[2] = devType;

            return devArray;
        }
        #endregion
    }

    #region 辅助枚举 设备类型
    /// <summary>
    ///  设备类型枚举结构
    /// </summary>
    public enum DevTypeENUM
    {
        NULL = 0,   //全部
        INWELL = 1,//入井
        OUTWELL,//出井
        INOUTWELL,//出入井
        ONDUTY,//上班
        OFFDUTY,//下班
        ONOFFDUTY//上下班
    }

    #endregion
}
