/*************************************************************************
** 文件名:   ZhuDuanAttendCollect.cs
×× 主要类:   ZhuDuanAttendCollect
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2014-7-18
** 修改人:   
** 日  期:   
** 描  述:   ZhuDuanAttendCollect类，林州铸锻考勤查询界面
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
using Lite.ExcelLibrary.SpreadSheet;
using System.IO;
using IriskingAttend.Common;
using ReportTemplate;
using System.Windows.Browser;
using IriskingAttend.NewWuHuShan;
using IriskingAttend.Web;
using IriskingAttend.ApplicationType;
using IriskingAttend.ShenShuoRailway;

namespace IriskingAttend.LinZhouZhuDuan
{
    public partial class ZhuDuanMonthAttend : Page
    {
        #region 私有变量

        private VmZhuDuanMonthAttend _vmZhuDuanMonthAttend = new VmZhuDuanMonthAttend();

        /// <summary>
        /// 报表DataGrid起始插入日期列--报表定制
        /// </summary>
        private const int _colInsert = 6;

        /// <summary>
        /// 报表DataGrid最大日期列--报表定制
        /// </summary>
        private const int _colMax = 37;

        /// <summary>
        /// 本地存储
        /// </summary>
        private IsolatedStorageSettings _querySetting = IsolatedStorageSettings.ApplicationSettings;

        /// <summary>
        /// 私有变量声明
        /// </summary>
        VmXlsFilter _vmFilter = new VmXlsFilter();

        #endregion

        #region 构造函数 初始化

        /// <summary>
        /// 构造函数
        /// </summary>
        public ZhuDuanMonthAttend()
        {
            InitializeComponent();                 

            this.DataContext = _vmZhuDuanMonthAttend;

            _vmFilter.GetDepartmentByPrivilege();

            //部门信息加载完成
            _vmFilter.DepartLoadCompleted += delegate
            {
            };
        }
        #endregion

        #region 点击查询 获取查询条件

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQuery_Click(object sender, RoutedEventArgs e)
        {
            dgAttendPersonList.ItemsSource = _vmZhuDuanMonthAttend.ZhuDuanMonthAttendModel;

            #region 判断查询时间
            if (dtpBegin.Text == null || dtpBegin.Text == "")
            {
                MsgBoxWindow.MsgBox(
                            "查询开始时间不能为空！",
                            Dialog.MsgBoxWindow.MsgIcon.Information,
                            Dialog.MsgBoxWindow.MsgBtns.OK);
                return;
            }
            DateTime dateBegin = dtpBegin.SelectedDate.Value.Date;
            DateTime dateEnd;

            if (dtpEnd.Text == null || dtpEnd.Text == "")
            {
                dateEnd = dateBegin.AddMonths(1).AddDays(-1);
                if (dateEnd > System.DateTime.Now)
                {
                    dateEnd = System.DateTime.Now.Date;
                }
            }
            else 
            {
                if (dateBegin.Date > dtpEnd.SelectedDate.Value.Date)
                {
                    MsgBoxWindow.MsgBox(
                                "请确定您的开始时间早于截止时间！",
                                Dialog.MsgBoxWindow.MsgIcon.Information,
                                Dialog.MsgBoxWindow.MsgBtns.OK);
                    return;
                }
                else if (dateBegin.AddMonths(1) < dtpEnd.SelectedDate.Value.Date.AddDays(1))
                {
                    MsgBoxWindow.MsgBox(
                                 "请确定您的查询跨度时间为一个月！",
                                  Dialog.MsgBoxWindow.MsgIcon.Information,
                                  Dialog.MsgBoxWindow.MsgBtns.OK);
                    return;
                }
                else
                {
                    dateEnd = dtpEnd.SelectedDate.Value.Date;
                }
            }
            #endregion            

            SetdgAttendPersonListHeader(dateBegin, dateEnd);

            SetCondition(dateBegin, dateEnd);

            GetAttend();
        }

        /// <summary>
        /// 设置datagrid的Header
        /// </summary>
        /// <param name="dateBegin">查询开始时间</param>
        /// <param name="dateEnd">查询截止时间</param>
        private void SetdgAttendPersonListHeader(DateTime dateBegin, DateTime dateEnd)
        {
            int col = _colInsert;
            for (dateBegin = (DateTime)dtpBegin.SelectedDate; dateBegin <= dateEnd; dateBegin = dateBegin.AddDays(1))
            {
                dgAttendPersonList.Columns[col].Header = dateBegin.Day.ToString("d2");               
                dgAttendPersonList.Columns[col++].Visibility = System.Windows.Visibility.Visible;
            }
            for (int i = col; i < _colMax; i++)
            {
                dgAttendPersonList.Columns[i].Visibility = System.Windows.Visibility.Collapsed;
            }     
        }

        /// <summary>
        /// 获取查询条件
        /// </summary>
        /// <param name="dateBegin"></param>
        /// <param name="dateEnd"></param>
        private void SetCondition(DateTime dateBegin, DateTime dateEnd)
        {
            QueryCondition qc = new QueryCondition();
            qc.BeginTime = dateBegin;
            qc.EndTime = dateEnd;           

            //获取选择的部门
            if (txtDepart.Text != "")
            {
                qc.DepartNameList = txtDepart.Text.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            }
            else
            {
                List<string> departList = new List<string>();

                if (_vmFilter.DepartInfoModel.Count > 0)
                {
                    foreach (UserDepartInfo ud in _vmFilter.DepartInfoModel)
                    {
                        departList.Add(ud.depart_name);
                    }
                }
                qc.DepartNameList = departList.ToArray();
            }

            //获取选择的人员姓名
            if (txtPersonName.Text != "")
            {
                qc.PersonNameList = txtPersonName.Text.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            }

            //获取选择的人员工号
            if (txtWorkSn.Text != "")
            {
                qc.WorkSnList = txtWorkSn.Text.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            }

            ///如果存在 先删除该键值对
            if (_querySetting.Contains("attendConditon"))
            {
                _querySetting.Remove("attendConditon");
            }
            _querySetting.Add("attendConditon", qc);
        }

        #region 选取部门、姓名、工号

        private void btnSelectDepart_Click(object sender, RoutedEventArgs e)
        {
            List<UserDepartInfo> departList = new List<UserDepartInfo>();
            string[] selectDepart = new string[0];

            //获取选择的部门
            if (txtDepart.Text != "")
            {
                selectDepart = txtDepart.Text.Split( new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            }
            if (_vmFilter.DepartInfoModel.Count > 0)
            {
                foreach (UserDepartInfo ud in _vmFilter.DepartInfoModel)
                {
                    if (selectDepart.Contains( ud.depart_name ) )
                    {
                        departList.Add( ud );
                    }
                }
            }
            SelectDepartPrivilege sd = new SelectDepartPrivilege(departList, ChildWnd_SelectDepart_CallBack);
            sd.Show();
        }

        /// <summary>
        /// 回调函数，得到选取的部门信息
        /// </summary>
        /// <param name="principalSelect"></param>
        private void ChildWnd_SelectDepart_CallBack(BaseViewModelCollection<UserDepartInfo> departSelect)
        {
            string txtShowDepartName = "";            
            foreach (UserDepartInfo ar in departSelect)
            {
                if (ar.isSelected)
                {
                    txtShowDepartName += ar.depart_name + ",";
                }
            }
            if (txtShowDepartName != "")
            {
                txtDepart.Text = txtShowDepartName.Remove(txtShowDepartName.LastIndexOf(","), 1);
            }
            else
            {
                txtDepart.Text = txtShowDepartName;
            }
        }

        private void btnSelectName_Click(object sender, RoutedEventArgs e)
        {
            //添加部门权限部分
            string[] departName = new string[0];
            if (txtDepart.Text != "")
            {
                departName = txtDepart.Text.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            }
            else
            {
                List<string> departNameList = new List<string>();
                foreach (var item in VmLogin.OperatorDepartInfoList)
                {
                    departNameList.Add(item.depart_name);
                }
                departName = departNameList.ToArray();
            }

            //获得人员信息
            _vmZhuDuanMonthAttend.GetPersonInfo(departName, () =>
            {
                _vmZhuDuanMonthAttend.SelectPersonByName();
                
                string[] personList = new string[0];

                //获取选择的部门
                if (txtPersonName.Text != "")
                {
                    personList = txtPersonName.Text.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                }
                if (_vmZhuDuanMonthAttend.SelectPersonInfoModel.Count > 0)
                {
                    foreach (UserPersonInfo personInfo in _vmZhuDuanMonthAttend.SelectPersonInfoModel)
                    {
                        personInfo.isSelected = false;
                        if (personList.Contains(personInfo.person_name))
                        {
                            personInfo.isSelected = true;
                        }
                    }
                }

                SelectPerson selectPerson = new SelectPerson(_vmZhuDuanMonthAttend.SelectPersonInfoModel, ChildWnd_SelectName_CallBack);
                selectPerson.Show();
            });            
        }

        /// <summary>
        /// 回调函数，得到选取的姓名信息
        /// </summary>
        /// <param name="principalSelect"></param>
        private void ChildWnd_SelectName_CallBack(BaseViewModelCollection<UserPersonInfo> personSelect)
        {
            string txtShowPersonName = "";
            foreach (UserPersonInfo ar in personSelect)
            {
                if (ar.isSelected)
                {
                    txtShowPersonName += ar.person_name + ",";
                }
            }
            if (txtShowPersonName != "")
            {
                txtPersonName.Text = txtShowPersonName.Remove(txtShowPersonName.LastIndexOf(","), 1);
            }
            else
            {
                txtPersonName.Text = txtShowPersonName;
            }
        }

        private void btnSelectWorkSn_Click(object sender, RoutedEventArgs e)
        {
            //添加部门权限部分
            string[] departName = new string[0];
            if (txtDepart.Text != "")
            {
                departName = txtDepart.Text.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            }
            else
            {
                List<string> departNameList = new List<string>();
                foreach (var item in VmLogin.OperatorDepartInfoList)
                {
                    departNameList.Add(item.depart_name);
                }
                departName = departNameList.ToArray();
            }

            //获得人员信息
            _vmZhuDuanMonthAttend.GetPersonInfo(departName, () =>
            {
                _vmZhuDuanMonthAttend.SelectPersonByWorkSn();

                string[] personList = new string[0];
                if (txtWorkSn.Text != "")
                {
                    personList = txtWorkSn.Text.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                }
                if (_vmZhuDuanMonthAttend.SelectPersonInfoModel.Count > 0)
                {
                    foreach (UserPersonInfo personInfo in _vmZhuDuanMonthAttend.SelectPersonInfoModel)
                    {
                        personInfo.isSelected = false;
                        if (personList.Contains(personInfo.work_sn))
                        {
                            personInfo.isSelected = true;
                        }
                    }
                }
                SelectPerson selectPerson = new SelectPerson(_vmZhuDuanMonthAttend.SelectPersonInfoModel, ChildWnd_SelectWorkSn_CallBack);
                selectPerson.Show();
            });            
        }

        /// <summary>
        /// 回调函数，得到选取的人员工号信息
        /// </summary>
        /// <param name="principalSelect"></param>
        private void ChildWnd_SelectWorkSn_CallBack(BaseViewModelCollection<UserPersonInfo> personSelect)
        {
            string txtShowWorkSn = "";
            foreach (UserPersonInfo ar in personSelect)
            {
                if (ar.isSelected)
                {
                    txtShowWorkSn += ar.work_sn + ",";
                }
            }
            if (txtShowWorkSn != "")
            {
                txtWorkSn.Text = txtShowWorkSn.Remove(txtShowWorkSn.LastIndexOf(","), 1);
            }
            else
            {
                txtWorkSn.Text = txtShowWorkSn;
            }
        }

        #endregion

        #endregion

        #region 查询数据

        /// <summary>
        /// 考勤查询
        /// </summary>
        void GetAttend()
        {
            if (_querySetting.Contains("attendConditon"))
            {
                try
                {
                    QueryCondition condition;
                    _querySetting.TryGetValue<QueryCondition>("attendConditon", out condition);
                    _vmZhuDuanMonthAttend.GetZhuDuanMonthAttendList(condition.BeginTime, condition.EndTime, condition.DepartNameList, condition.PersonNameList, condition.WorkSnList);
                }
                catch (Exception e)
                {
                    ErrorWindow err = new ErrorWindow(e);
                    err.Show();
                }
            }
            else
            {
                ChildWindow errorWin = new ErrorWindow("非法请求", "查询条件不准确！");
                errorWin.Show();
            }
        }

        #endregion        

        #region 导出excel

        /// <summary>
        /// 导出excel文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            if (_vmZhuDuanMonthAttend.ZhuDuanMonthAttendModel.Count() < 1)
            {
                Dialog.MsgBoxWindow.MsgBox(
                            "请查询到数据后再导出Excel！",
                            Dialog.MsgBoxWindow.MsgIcon.Information,
                            Dialog.MsgBoxWindow.MsgBtns.OK);
                return;
            }
            DateTime beginTime = new DateTime();
            DateTime endTime = new DateTime();
            QueryCondition condition;
            if (_querySetting.Contains("attendConditon"))
            {
                _querySetting.TryGetValue<QueryCondition>("attendConditon", out condition);
                beginTime = condition.BeginTime;
                endTime = condition.EndTime;
            }
            else
            {
                return;
            }

            SaveFileDialog sDialog = new SaveFileDialog();
            sDialog.Filter = "Excel Files(*.xls)|*.xls";

            if (sDialog.ShowDialog() == false)
            {
                return;
            }

            string title = "林州铸锻考勤月报表";
            string space = "                                                                                                    ";

            Workbook workBook = new Workbook();
            Worksheet workSheet = new Worksheet(title);

            workSheet.Cells[0, 0] = new Cell(space + title + space);

            Int16 ColumnCount = -1;
            Int16 RowCount = 1;

            #region     生成表头

            workSheet.Cells[RowCount, ++ColumnCount] = new Cell("序号");
            workSheet.Cells.ColumnWidth[(byte)RowCount, (byte)ColumnCount] = 2500;
            workSheet.Cells[RowCount, ++ColumnCount] = new Cell("部门");
            workSheet.Cells[RowCount, ++ColumnCount] = new Cell("工号");
            workSheet.Cells[RowCount, ++ColumnCount] = new Cell("姓名");
            workSheet.Cells[RowCount, ++ColumnCount] = new Cell("考勤");
            workSheet.Cells[RowCount, ++ColumnCount] = new Cell("时间");      

            for (DateTime mindate = beginTime; mindate <= endTime; mindate = mindate.AddDays(1))
            {
                workSheet.Cells[RowCount, ++ColumnCount] = new Cell(mindate.Day.ToString("D2"));
                
            }          

            #endregion

            foreach (ZhuDuanMonthAttendReport data in _vmZhuDuanMonthAttend.ZhuDuanMonthAttendModel)
            {
                ColumnCount = 0;
                RowCount++;

                workSheet.Cells[RowCount, ColumnCount++] = new Cell(data.Index);
                workSheet.Cells[RowCount, ColumnCount++] = new Cell(data.DepartName);
                workSheet.Cells[RowCount, ColumnCount++] = new Cell(data.WorkSn);
                workSheet.Cells[RowCount, ColumnCount++] = new Cell(data.PersonName);
                workSheet.Cells[RowCount, ColumnCount++] = new Cell(data.AttendCount.ToString());
                workSheet.Cells[RowCount, ColumnCount++] = new Cell(data.ClassType);

                foreach (string count in data.DisplaySignal)
                {
                    workSheet.Cells[RowCount, ColumnCount++] = new Cell(count);
                }              
            }

            //add worksheet to workbook
            workBook.Worksheets.Add(workSheet);
            // get the selected file's stream
            Stream sFile = sDialog.OpenFile();
            workBook.Save(sFile);
           
        }

        #endregion

        #region 打印预览

        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            WaitingDialog.ShowWaiting("正在打印预览数据，请稍后", 500, new Action(btnPrintEvent)); 
        }

        /// <summary>
        /// 预览打印数据
        /// </summary>
        void btnPrintEvent()
        {
            if (_vmZhuDuanMonthAttend.ZhuDuanMonthAttendModel.Count() < 1)
            {
                Dialog.MsgBoxWindow.MsgBox(
                           "请查询到数据后再进行打印预览！",
                           Dialog.MsgBoxWindow.MsgIcon.Information,
                           Dialog.MsgBoxWindow.MsgBtns.OK);

                WaitingDialog.HideWaiting();
                return;
            }
            DateTime beginTime = new DateTime();
            DateTime endTime = new DateTime();
            if (_querySetting.Contains("attendConditon"))
            {
                QueryCondition condition;
                _querySetting.TryGetValue<QueryCondition>("attendConditon", out condition);
                beginTime = condition.BeginTime;
                endTime = condition.EndTime;
            }
            else
            {
                WaitingDialog.HideWaiting();
                return;
            }

            #region     生成表头数据
            List<HeaderNode> pageHeaderData = new List<HeaderNode>();

            pageHeaderData.Add(new HeaderNode("序号", 11, 0, 1));
            pageHeaderData.Add(new HeaderNode("部门", 9, 0, 1));
            pageHeaderData.Add(new HeaderNode("工号", 9, 0, 1));
            pageHeaderData.Add(new HeaderNode("姓名", 9, 0, 1));
            pageHeaderData.Add(new HeaderNode("考勤", 9, 0, 1));
            pageHeaderData.Add(new HeaderNode("时间", 9, 0, 1));
            for (DateTime mindate = beginTime; mindate <= endTime; mindate = mindate.AddDays(1))
            {
                pageHeaderData.Add(new HeaderNode(mindate.Day.ToString("d2"), 9, 0, 1));
            }

            ReportHeader reportHeader = new ReportHeader(pageHeaderData);
            #endregion

            #region 表标题
            string time = beginTime.ToLongDateString() + " 到 " + endTime.ToLongDateString();

            ReportTitle reportTitle = new ReportTitle("林州铸锻考勤月报表", 20, time, 11, false, true);
            #endregion

            #region     生成报表页脚数据
            ReportFooter reportFooter = new ReportFooter("", 11);
            #endregion

            string[] bindingPropertyNames = new string[pageHeaderData.Count()];

            int index = 0;
            bindingPropertyNames[index++] = "Index";
            bindingPropertyNames[index++] = "DepartName";
            bindingPropertyNames[index++] = "WorkSn";
            bindingPropertyNames[index++] = "PersonName";
            bindingPropertyNames[index++] = "AttendCount";
            bindingPropertyNames[index++] = "ClassType";            
            bindingPropertyNames[index++] = "DisplaySignal";            
           
            PrintControl printControl = new PrintControl();

            bool res = false;
            res = printControl.SetDataSource<ZhuDuanMonthAttendReport>(reportTitle, reportFooter, reportHeader, _vmZhuDuanMonthAttend.ZhuDuanMonthAttendModel, bindingPropertyNames, () =>
            {
                if (res)
                {
                    printControl.Preview_CurPage(null, null);
                }
            }, 9);

            WaitingDialog.HideWaiting();
        }

        #endregion              
    }
    #region 选择条件

    /// <summary>
    /// 报表查询条件
    /// </summary>
    public class QueryCondition
    {
        /// <summary>
        /// 起始时间
        /// </summary>
        public DateTime BeginTime { get; set; }

        /// <summary>
        /// 截止时间
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        public string[] DepartNameList { get; set; }

        /// <summary>
        /// 人员姓名
        /// </summary>
        public string[] PersonNameList { get; set; }

        /// <summary>
        /// 工号
        /// </summary>
        public string[] WorkSnList { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public QueryCondition()
        {
            BeginTime = DateTime.MinValue;
            EndTime = DateTime.Now;
            DepartNameList = null;
            PersonNameList = null;
            WorkSnList = null;
        }
    }

    #endregion
}
