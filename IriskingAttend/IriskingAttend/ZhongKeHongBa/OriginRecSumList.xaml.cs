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
using IriskingAttend.NewWuHuShan;
using System.IO.IsolatedStorage;
using IriskingAttend.Common;
using ReportTemplate;
using IriskingAttend.Web;
using Irisking.Web.DataModel;
using IriskingAttend.ExportExcel;
using System.IO;
using Lite.ExcelLibrary.SpreadSheet;

namespace IriskingAttend.ZhongKeHongBa
{
    public partial class OriginRecSumList : Page
    {
        #region 私有变量

        private VmPersonOriginRecCollect _vmPersonOriginRecCollect = new VmPersonOriginRecCollect();
      
        /// <summary>
        /// 私有变量声明
        /// </summary>
        private VmXlsFilter _vmFilter = new VmXlsFilter();
    
        /// <summary>
        /// 本地存储
        /// </summary>
        private IsolatedStorageSettings _querySetting = IsolatedStorageSettings.ApplicationSettings;       

        private VmPersonFullAttendCollect _vmPersonFullAttendCollect = new VmPersonFullAttendCollect(); 

        #endregion

        #region 构造函数
        public OriginRecSumList()
        {
            InitializeComponent(); 

            dtpBegin.Text = DateTime.Now.AddMonths(-1).ToString("yyyy-MM-01");
            this.DataContext = _vmPersonOriginRecCollect;
            dgPersonOriginList.ItemsSource = _vmPersonOriginRecCollect.PersonOriginInfoModel;

            _vmFilter.GetDepartmentByPrivilege();

            //部门信息加载完成
            _vmFilter.DepartLoadCompleted += delegate
            {
            };   
        }
        #endregion

        #region 点击查询 
       
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQuery_Click(object sender, RoutedEventArgs e)
        {
            if (dtpBegin.Text == null || dtpBegin.Text == "")
            {
                MsgBoxWindow.MsgBox(
                            "查询开始时间不能为空！",
                            Dialog.MsgBoxWindow.MsgIcon.Information,
                            Dialog.MsgBoxWindow.MsgBtns.OK);
                return;
            }

            ZhongKeHongBaQueryCondition qc = new ZhongKeHongBaQueryCondition();

            qc.BeginTime = this.dtpBegin.SelectedDate.Value.Date;
            qc.EndTime = this.dtpBegin.SelectedDate.Value.AddMonths(1);

            qc.DepartNameList = null;
            //获取选择的部门
            if (txtDepart.Text != "")
            {
                qc.DepartNameList = txtDepart.Text.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            }

            qc.PersonNameList = null;
            //获取选择的人员姓名
            if (txtPersonName.Text != "")
            {
                qc.PersonNameList = txtPersonName.Text.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            }

            ///如果存在 先删除该键值对
            if (_querySetting.Contains("attendConditon"))
            {
                _querySetting.Remove("attendConditon");
            }
            _querySetting.Add("attendConditon", qc);


            int col = 3;
            for (DateTime mindate = qc.BeginTime; mindate < qc.EndTime; mindate = mindate.AddDays(1))
            {
                dgPersonOriginList.Columns[col].Header = mindate.Day.ToString("d2");
                dgPersonOriginList.Columns[col++].Visibility = System.Windows.Visibility.Visible;
            }
            for (int i = col; i < 34; i++)
            {
                dgPersonOriginList.Columns[i].Visibility = System.Windows.Visibility.Collapsed;
            }

            _vmPersonOriginRecCollect.GetPersonAttendCollect(qc.BeginTime, qc.EndTime, qc.DepartNameList, qc.PersonNameList);

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
            if (_vmPersonOriginRecCollect.PersonOriginInfoModel.Count() < 1)
            {
                Dialog.MsgBoxWindow.MsgBox(
                            "请查询到数据后再导出Excel！",
                            Dialog.MsgBoxWindow.MsgIcon.Information,
                            Dialog.MsgBoxWindow.MsgBtns.OK);
                return;
            }

            DateTime beginTime = new DateTime();
            DateTime endTime = new DateTime();
            if (_querySetting.Contains("attendConditon"))
            {
                ZhongKeHongBaQueryCondition condition;
                _querySetting.TryGetValue<ZhongKeHongBaQueryCondition>("attendConditon", out condition);
                beginTime = condition.BeginTime;
                endTime = condition.EndTime.AddDays(-1);
            }
            else
            {
                WaitingDialog.HideWaiting();
                return;
            }

            SaveFileDialog sDialog = new SaveFileDialog();
            sDialog.Filter = "Excel Files(*.xls)|*.xls";

            if (sDialog.ShowDialog() == false)
            {
                return;
            }

            string title = "原始记录汇总表";
            string space = "                                                                                                    ";

            Workbook workBook = new Workbook();
            Worksheet workSheet = new Worksheet(title);

            workSheet.Cells[0, 0] = new Cell(space + title + space);

            Int16 ColumnCount = -1;
            Int16 RowCount = 1;

            #region     生成表头

            workSheet.Cells[RowCount, ++ColumnCount] = new Cell("序号");
            workSheet.Cells[RowCount, ++ColumnCount] = new Cell("部门");
            workSheet.Cells[RowCount, ++ColumnCount] = new Cell("姓名");

            for (DateTime mindate = beginTime; mindate <= endTime; mindate = mindate.AddDays(1))
            {
                workSheet.Cells[RowCount, ++ColumnCount] = new Cell(mindate.Day.ToString("D2"));
                workSheet.Cells.ColumnWidth[(byte)RowCount, (byte)ColumnCount] = 1500;
            }
            workSheet.Cells[RowCount, ++ColumnCount] = new Cell("总工时");
            workSheet.Cells[RowCount, ++ColumnCount] = new Cell("出勤次数");
            workSheet.Cells[RowCount, ++ColumnCount] = new Cell("平均工时");

            #endregion

            foreach (PersonOriginInfo data in _vmPersonOriginRecCollect.PersonOriginInfoModel)
            {
                ColumnCount = 0;
                RowCount++;

                workSheet.Cells[RowCount, ColumnCount++] = new Cell(data.Index);
                workSheet.Cells[RowCount, ColumnCount++] = new Cell(data.DepartName);
                workSheet.Cells[RowCount, ColumnCount++] = new Cell(data.PersonName);

                foreach (string count in data.WorkTimes)
                {
                    workSheet.Cells[RowCount, ColumnCount++] = new Cell(count);
                }

                workSheet.Cells[RowCount, ColumnCount++] = new Cell(data.CountTimes);
                workSheet.Cells[RowCount, ColumnCount++] = new Cell(data.InOutTimes.ToString());
                workSheet.Cells[RowCount, ColumnCount++] = new Cell(data.PrvTimes);
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
            if (_vmPersonOriginRecCollect.PersonOriginInfoModel.Count() < 1)
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
                ZhongKeHongBaQueryCondition condition;
                _querySetting.TryGetValue<ZhongKeHongBaQueryCondition>("attendConditon", out condition);
                beginTime = condition.BeginTime;
                endTime = condition.EndTime.AddDays(-1);
            }
            else
            {
                WaitingDialog.HideWaiting();
                return;
            }

            #region     生成表头数据


            List<HeaderNode> pageHeaderData = new List<HeaderNode>();

            pageHeaderData.Add(new HeaderNode("序号", 11, 0, 1));
            pageHeaderData.Add(new HeaderNode("部门", 11, 0, 1));
            pageHeaderData.Add(new HeaderNode("姓名", 11, 0, 1));
            for (DateTime mindate = beginTime; mindate <= endTime; mindate = mindate.AddDays(1))
            {
                pageHeaderData.Add(new HeaderNode(mindate.Day.ToString("d2"), 9, 0, 1));
            }
            pageHeaderData.Add(new HeaderNode("总工时", 11, 0, 1));
            pageHeaderData.Add(new HeaderNode("出勤次数", 11, 0, 1));
            pageHeaderData.Add(new HeaderNode("平均工时", 11, 0, 1));

            ReportHeader reportHeader = new ReportHeader(pageHeaderData);
            #endregion

            #region 表标题
            string time = beginTime.ToLongDateString() + " 到 " + endTime.ToLongDateString();

            ReportTitle reportTitle = new ReportTitle("原始记录汇总表", 20, time, 11, false);
            #endregion

            #region     生成报表页脚数据
            ReportFooter reportFooter = new ReportFooter("", 11);
            #endregion

            string[] bindingPropertyNames = new string[] 
            { 
                "Index",
                "DepartName",
                "PersonName",
                "WorkTimes",
                "CountTimes",
                "InOutTimes",
                "PrvTimes",
            };

            PrintControl printControl = new PrintControl();
            bool res = false;
            res = printControl.SetDataSource<PersonOriginInfo>(reportTitle, reportFooter, reportHeader, _vmPersonOriginRecCollect.PersonOriginInfoModel, bindingPropertyNames, () =>
            {
                if (res)
                {
                    printControl.Preview_CurPage(null, null);
                }
            }, 9);
            WaitingDialog.HideWaiting();
        }

        #endregion    

        #region 选取部门、姓名

        private void btnSelectDepart_Click(object sender, RoutedEventArgs e)
        {
            List<UserDepartInfo> departList = new List<UserDepartInfo>();
            string[] selectDepart = new string[0];

            //获取选择的部门
            if (txtDepart.Text != "")
            {
                selectDepart = txtDepart.Text.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            }
            if (_vmFilter.DepartInfoModel.Count > 0)
            {
                foreach (UserDepartInfo ud in _vmFilter.DepartInfoModel)
                {
                    if (selectDepart.Contains(ud.depart_name))
                    {
                        departList.Add(ud);
                    }
                }
            }
            SelectDepart sd = new SelectDepart(departList, ChildWnd_SelectDepart_CallBack);
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
            _vmPersonFullAttendCollect.GetPersonInfo(txtDepart.Text.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries), () =>
            {
                _vmPersonFullAttendCollect.SelectPersonByName();

                string[] personList = new string[0];

                //获取选择的部门
                if (txtPersonName.Text != "")
                {
                    personList = txtPersonName.Text.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                }
                if (_vmPersonFullAttendCollect.SelectPersonInfoModel.Count > 0)
                {
                    foreach (UserPersonInfo personInfo in _vmPersonFullAttendCollect.SelectPersonInfoModel)
                    {
                        personInfo.isSelected = false;
                        if (personList.Contains(personInfo.person_name))
                        {
                            personInfo.isSelected = true;
                        }
                    }
                }

                SelectPersonName selectPerson = new SelectPersonName(_vmPersonFullAttendCollect.SelectPersonInfoModel, ChildWnd_SelectName_CallBack);
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
        #endregion         
    }
}
