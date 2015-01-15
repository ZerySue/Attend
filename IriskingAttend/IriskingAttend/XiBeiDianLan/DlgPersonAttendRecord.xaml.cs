/*************************************************************************
** 文件名:   DlgPersonAttendRecord.cs
×× 主要类:   DlgPersonAttendRecord
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2014-5-30
** 修改人:   
** 日  期:   
** 描  述:   DlgPersonAttendRecord类，西北电缆厂人员考勤记录列表
**
** 版  本:   1.0.0
** 备  注:  命名及代码编写遵守C#编码规范
**
 * ***********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Navigation;
using IriskingAttend.Dialog;
using IriskingAttend.ViewModel;
using Irisking.Web.DataModel;
using System.IO.IsolatedStorage;
using System.IO;
using IriskingAttend.Common;
using ReportTemplate;
using System.Windows.Browser;

using Lite.ExcelLibrary.CompoundDocumentFormat;
using Lite.ExcelLibrary.BinaryFileFormat;
using Lite.ExcelLibrary.SpreadSheet;
using IriskingAttend.ApplicationType;
using IriskingAttend.Web;

namespace IriskingAttend.XiBeiDianLan
{
    public partial class DlgPersonAttendRecord : ChildWindow
    {
        #region 私有变量


        private VmXiBeiDianLanAttendCollect _vmAttendCollect = new VmXiBeiDianLanAttendCollect();
      
        #endregion

        #region 构造函数 初始化

        /// <summary>
        /// 构造函数
        /// </summary>
        public DlgPersonAttendRecord(DateTime beginTime, DateTime endTime, int personId, int attendType  )
        {
            InitializeComponent();

            dgAttendRecord.ItemsSource = _vmAttendCollect.AttendRecordModel;

            _vmAttendCollect.GetPersonAttendRecord(beginTime, endTime, personId, attendType);
        }
       
        #endregion
    }
}
