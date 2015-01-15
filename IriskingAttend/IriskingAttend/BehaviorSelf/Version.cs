/*************************************************************************
** 文件名:   Version.cs
** 主要类:   Version
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2013-10-29
** 修改人:  
** 日  期:   
** 描  述:   Version类,版本号
**
** 版  本:   1.0.0
** 备  注:  命名及代码编写遵守C#编码规范
**
*************************************************************************/
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
using System.Text.RegularExpressions;
using IriskingAttend.Dialog;
using System.Collections.Generic;
using System.Windows.Data;
using System.Linq;

namespace IriskingAttend
{
    public class Version
    {
        /// <summary>
        /// svn 版本号
        /// </summary>
        public static string SvnVersion = "3851";

        /// <summary>
        /// 大版本号名字
        /// </summary>
        public static string FirstVersion = "虹膜考勤系统 V4.0";

        /// <summary>
        /// 获得项目的总版本号
        /// </summary>
        /// <returns></returns>
        public static string GetProjectVersion()
        {
            return (FirstVersion +"."+ SvnVersion);
        }

        /// <summary>
        /// 获得大版本号，在界面显示时不需要显示svn版本号
        /// </summary>
        /// <returns></returns>
        public static string GetFirstVersion()
        {
            return FirstVersion;
        }
    } 
}
