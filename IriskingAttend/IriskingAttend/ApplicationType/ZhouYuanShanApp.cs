/*************************************************************************
** 文件名:   ZhouYuanShanApp.cs
** 主要类:   ZhouYuanShanApp
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2014-1-9
** 修改人:   
** 日  期:
** 描  述:   ZhouYuanShanApp，周源山应用程序类型
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
    public class ZhouYuanShanApp : AbstractApp
    {
        private AbstractApp BaseApp;

        public ZhouYuanShanApp(AbstractApp BaseApp)
        {
            this.BaseApp = BaseApp;
        }

        public override Dictionary<PrivilegeENUM, bool> GetDictPrivilegeListVisible()
        {
            Dictionary<PrivilegeENUM, bool> dictPrivilegeList = BaseApp.GetDictPrivilegeListVisible();

            dictPrivilegeList[PrivilegeENUM.ReportOriginRecord] = false;              //"原始记录汇总表"
            dictPrivilegeList[PrivilegeENUM.ReportOriginRecordExportExcel] = false;   //"导出Excel"
            dictPrivilegeList[PrivilegeENUM.ReportOriginRecordPrint] = false;         //"打印预览"
            dictPrivilegeList[PrivilegeENUM.ReportCustom] = false;  //自定义报表

            //周源山报表及周源山模块
            dictPrivilegeList[PrivilegeENUM.ReportZhouYuanShan] = true; //"打印预览"   
            dictPrivilegeList[PrivilegeENUM.Lunch] = true;              //"班中餐"  
            dictPrivilegeList[PrivilegeENUM.LunchManage] = true;        //"班中餐管理"
            dictPrivilegeList[PrivilegeENUM.LunchQuery] = true;         //"班中餐查询"

            dictPrivilegeList[PrivilegeENUM.LunchManageQuery] = true;       //"班中餐管理"中的查询
            dictPrivilegeList[PrivilegeENUM.LunchManageEdit] = true;        //"班中餐管理"中未完成班中餐编辑
            dictPrivilegeList[PrivilegeENUM.LunchManageCreate] = true;      //"班中餐管理"中未完成班中餐生成
            dictPrivilegeList[PrivilegeENUM.LunchManageShow] = true;        //"班中餐管理"中已完成班中餐查看
            dictPrivilegeList[PrivilegeENUM.LunchManageUndo] = true;        //"班中餐管理"中已完成班中餐撤销

            return dictPrivilegeList;
        }
    }
}
