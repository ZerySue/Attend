/*************************************************************************
** 文件名:   WuHuShanApp.cs
** 主要类:   WuHuShanApp
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2013-12-11
** 修改人:   
** 日  期:
** 描  述:   WuHuShanApp，五虎山应用程序类型
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
using System.Windows.Media.Imaging;

namespace IriskingAttend.ApplicationType
{
    public class WuHuShanApp : AbstractApp
    {
        private AbstractApp BaseApp;

        public WuHuShanApp(AbstractApp BaseApp)
        {
            this.BaseApp = BaseApp;
        }

        public override Dictionary<PrivilegeENUM, bool> GetDictPrivilegeListVisible()
        {
            Dictionary<PrivilegeENUM, bool> dictPrivilegeList = BaseApp.GetDictPrivilegeListVisible();

            dictPrivilegeList[PrivilegeENUM.ReportCustom] = false;  //自定义报表

            dictPrivilegeList[PrivilegeENUM.ReportDepartAttendCollect] = true;              //"部门出勤汇总表"
            dictPrivilegeList[PrivilegeENUM.ReportDepartAttendCollectExportExcel] = true;   //"导出Excel"
            dictPrivilegeList[PrivilegeENUM.ReportDepartAttendCollectPrint] = true;         //"打印预览"   
            dictPrivilegeList[PrivilegeENUM.AbnormalAttendInfo] = true;   //"异常考勤查询" 
            dictPrivilegeList[PrivilegeENUM.IrisAttendQuery] = true;      //"虹膜考勤查询" 

            dictPrivilegeList[PrivilegeENUM.SystemOperatorLogQuery] = true;  //操作员日志查询权限
            dictPrivilegeList[PrivilegeENUM.SystemOperatorLogDelete] = true;  //操作员日志删除权限
            return dictPrivilegeList;
        }

        public override void SetAppLogo()
        {
            try
            {
                string imageUrl = "/IriskingAttend;component/NewWuHuShan/Images/wuhushanlog.png";
                System.IO.Stream streamInfo = Application.GetResourceStream(new Uri(imageUrl, UriKind.Relative)).Stream;

                LogoImage = new BitmapImage();
                LogoImage.SetSource(streamInfo);
                LogoText = "五虎山矿业有限责任公司";                
            }
            catch
            {
            }
        }
    }
}
