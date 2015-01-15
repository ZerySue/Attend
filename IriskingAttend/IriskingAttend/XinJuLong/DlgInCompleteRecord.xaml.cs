/*************************************************************************
** 文件名:   DlgInCompleteRecord.cs
×× 主要类:   DlgInCompleteRecord
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2014-3-20
** 修改人:   
** 日  期:   
** 描  述:   DlgInCompleteRecord类，新巨龙人员不完整考勤记录列表
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

namespace IriskingAttend.XinJuLong
{
    public partial class DlgInCompleteRecord : ChildWindow
    {
        #region 私有变量

       
        private VmInCompleteQuery _vmInCompleteQuery = new VmInCompleteQuery();
      
        #endregion

        #region 构造函数 初始化

        /// <summary>
        /// 构造函数
        /// </summary>
        public DlgInCompleteRecord(DateTime beginTime, DateTime endTime, int personId)
        {
            InitializeComponent();

            dgInCompleteRecord.ItemsSource = _vmInCompleteQuery.InCompleteRecordModel;

            _vmInCompleteQuery.GetInCompleteRecord( beginTime, endTime, personId);
        }
       
        #endregion
    }
}
