/*************************************************************************
** 文件名:   StandardNotMineApp.cs
** 主要类:   StandardNotMineApp
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2013-12-11
** 修改人:   
** 日  期:
** 描  述:   StandardNotMineApp，标准非矿应用程序类型
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

namespace IriskingAttend.ApplicationType
{
    public class StandardNotMineApp:AbstractApp
    {
        public override Dictionary<PrivilegeENUM, bool> GetDictPrivilegeListVisible()
        {
            Dictionary<PrivilegeENUM, bool> dictPrivilegeList = base.GetDictPrivilegeListVisible();

            dictPrivilegeList[PrivilegeENUM.PersonWorkType] = false;           //"工种管理"
            dictPrivilegeList[PrivilegeENUM.ReportCustom] = true;
            return dictPrivilegeList;
        }
    }
}
