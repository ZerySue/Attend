/*************************************************************************
** 文件名:   AttendanceDetailList.cs
×× 主要类:   AttendanceDetailList
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   cty
** 日  期:   2013-8-30
** 修改人:   
** 日  期:   
** 描  述:   AttendanceDetailList类，五虎山出勤明细表
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
using IriskingAttend.Dialog;
using Irisking.Web.DataModel;
using System.IO.IsolatedStorage;
using Lite.ExcelLibrary.SpreadSheet;
using System.IO;
using IriskingAttend.Common;
using ReportTemplate;
using IriskingAttend.ExportExcel;
using System.Globalization;
using System.Windows.Controls.Primitives;
using IriskingAttend.ViewModel;

namespace IriskingAttend.NewWuHuShan
{
    public partial class AttendanceDetailList : ChildWindow
    {
        #region 变量声明

        /// <summary>
        /// 五虎山人员出入井记录表的VM层
        /// </summary>
        private VmAttendanceDetailList _vmXls = new VmAttendanceDetailList();

        /// <summary>
        /// 部门、职务、工种vm数据
        /// </summary>
        private VmXlsFilter _vmXlsFilter = new VmXlsFilter();

        /// <summary>
        /// 本地存储
        /// </summary>
        private IsolatedStorageSettings _querySetting = IsolatedStorageSettings.ApplicationSettings;

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public AttendanceDetailList()
        {
            InitializeComponent();
            this.DataContext = _vmXls;
            SetInt();
        }

        #endregion

        #region 界面初始化函数

        /// <summary>
        /// 初始化绑定 部门，职务，工种
        /// </summary>
        private void SetInt()
        {
            ///序号
            dgAttendanceDetailList.LoadingRow += (a, e) =>
            {
                int index = e.Row.GetIndex();
                var cell = dgAttendanceDetailList.Columns[0].GetCellContent(e.Row) as TextBlock;
                cell.Text = (index + 1).ToString();
            };

            //加载部门
            _vmXlsFilter.GetDepartmentByPrivilege();
            _vmXlsFilter.DepartLoadCompleted += delegate
            {
                if (cmbDepart.Items.Count > 0)
                {
                    cmbDepart.SelectedIndex = 0;
                }
                _vmXlsFilter.GetWorkType();
            };
            this.cmbDepart.ItemsSource = _vmXlsFilter.DepartInfoModel;
            this.cmbDepart.DisplayMemberPath = "depart_name";

            //加载工种
            _vmXlsFilter.WorkTypeLoadCompleted += delegate
            {
                if (_vmXlsFilter.WorkTypeModel.Count > 0)
                {
                    _vmXlsFilter.WorkTypeModel.RemoveAt(0);
                    this.lstWorkType.SelectedIndex = -1;
                }
                _vmXlsFilter.GetPrincipalInfo();
            };
            this.lstWorkType.ItemsSource = _vmXlsFilter.WorkTypeModel;
            this.lstWorkType.DisplayMemberPath = "work_type_name";

            //加载职务
            _vmXlsFilter.PrincipalInfoLoadCompleted += delegate
            {
                if (_vmXlsFilter.PrincipalInfoModel.Count > 0)
                {
                    _vmXlsFilter.PrincipalInfoModel.RemoveAt(0);
                    this.lstPrincipalName.SelectedIndex = -1;
                }
            };
            this.lstPrincipalName.ItemsSource = _vmXlsFilter.PrincipalInfoModel;
            this.lstPrincipalName.DisplayMemberPath = "principal_name";

        }

        #endregion

        #region  按钮查询事件

        /// <summary>
        /// 按钮查询事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQuery_Click(object sender, RoutedEventArgs e)
        {
            #region 判断查询时间
            if (txtdateBegin.Text == null || txtdateBegin.Text == "")
            {
                MsgBoxWindow.MsgBox(
                            "查询开始时间不能为空！",
                            Dialog.MsgBoxWindow.MsgIcon.Information,
                            Dialog.MsgBoxWindow.MsgBtns.OK);
                return;
            }
            if (txtdateEnd.Text == null || txtdateEnd.Text == "")
            {
                MsgBoxWindow.MsgBox(
                            "查询截止时间不能为空！",
                            Dialog.MsgBoxWindow.MsgIcon.Information,
                            Dialog.MsgBoxWindow.MsgBtns.OK);
                return;
            }
           
            DateTime dataBegin ;
            DateTime dataEnd ;

            try
            {
                 dataBegin = DateTime.Parse(txtdateBegin.Text);
                 dataEnd = DateTime.Parse(txtdateEnd.Text);
            }
            catch
            {
                 MsgBoxWindow.MsgBox(
                            "您输入的时间格式不正确，请重新输入！",
                            Dialog.MsgBoxWindow.MsgIcon.Information,
                            Dialog.MsgBoxWindow.MsgBtns.OK);
                return;
            }

            if (dataBegin >= dataEnd)//dtpEnd.SelectedDate.Value.Date.AddDays(1))
            {
                MsgBoxWindow.MsgBox(
                            "请确定您的开始时间早于截止时间！",
                            Dialog.MsgBoxWindow.MsgIcon.Information,
                            Dialog.MsgBoxWindow.MsgBtns.OK);
                return;
            }
            
            #endregion

            if (ConvertToInt(txtWorkTimeMore.Text.Trim()) == -2)
            {
                Dialog.MsgBoxWindow.MsgBox(
                                "请确定您输入的”工作时长大于“为数字！",
                                Dialog.MsgBoxWindow.MsgIcon.Information,
                                Dialog.MsgBoxWindow.MsgBtns.OK);
                return;
            }
            if (ConvertToInt(txtWorkTimeEqual.Text.Trim()) == -2)
            {
                Dialog.MsgBoxWindow.MsgBox(
                                "请确定您输入的”工作时长等于“为数字！",
                                Dialog.MsgBoxWindow.MsgIcon.Information,
                                Dialog.MsgBoxWindow.MsgBtns.OK);
                return;
            }
            if (ConvertToInt(txtWorkTimeLess.Text.Trim()) == -2)
            {
                Dialog.MsgBoxWindow.MsgBox(
                                "请确定您输入的”工作时长小于“为数字！",
                                Dialog.MsgBoxWindow.MsgIcon.Information,
                                Dialog.MsgBoxWindow.MsgBtns.OK);
                return;
            }

            SetCondition(dataBegin, dataEnd);

            GetAttend();
        }

        /// <summary>
        /// 获取查询条件
        /// </summary>
        /// <param name="dataBegin"></param>
        /// <param name="dataEnd"></param>
        private void SetCondition(DateTime dataBegin, DateTime dataEnd)
        {
            dgAttendanceDetailList.ItemsSource = _vmXls.XlsAttendancdDetailListModel;

            XlsQueryCondition qc = new XlsQueryCondition();
            qc.XlsBeginTime = dataBegin;
            qc.XlsEndTime = dataEnd;
            qc.XlsName = txtPersonName.Text.Trim();
            qc.XlsWorkSN = txtWorkSn.Text.Trim();
           
            qc.XlsWorkTimeMore = ConvertToInt(txtWorkTimeMore.Text.Trim());
            qc.XlsWorkTimeEqual = ConvertToInt(txtWorkTimeEqual.Text.Trim());
            qc.XlsWorkTimeLess = ConvertToInt(txtWorkTimeLess.Text.Trim());

            //获取选择的部门
            if (cmbDepart.SelectedIndex > 0)
            {
                qc.XlsDepartIdLst = new int[1];
                qc.XlsDepartIdLst[0] = (cmbDepart.SelectedItem as UserDepartInfo).depart_id;

            }

            //获取选择的职务
            if (lstPrincipalName.SelectedItems.Count > 0)
            {
                qc.XlsPrincipalId = new int[lstPrincipalName.SelectedItems.Count];
                for (int i = 0; i < lstPrincipalName.SelectedItems.Count; i++)
                {
                    qc.XlsPrincipalId[i] = (lstPrincipalName.SelectedItems[i] as PrincipalInfo).principal_id;
                }                  
            }

            //获取选择的工种
            if (lstWorkType.SelectedItems.Count > 0)
            {
                qc.XlsWorkTypeId = new int[lstWorkType.SelectedItems.Count];
                for (int i = 0; i < lstWorkType.SelectedItems.Count; i++)
                {
                    qc.XlsWorkTypeId[i] = (lstWorkType.SelectedItems[i] as WorkTypeInfo).work_type_id;
                }                   
            }

            ///如果存在 先删除该键值对
            if (_querySetting.Contains("attendConditon"))
            {
                _querySetting.Remove("attendConditon");
            }
            _querySetting.Add("attendConditon", qc);
        }

        #region 将DatePicker控件和textbox控件结合显示年月日时分秒

        /// <summary>
        /// 开始日期的选择改变时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dtpBegin_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            txtdateBegin.Text = dtpBegin.Text + " " + "00:00:00";
        }

        /// <summary>
        /// 截止日期的选择改变时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dtpEnd_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            txtdateEnd.Text = dtpEnd.Text + " " + "23:59:59";
        }

        #endregion

        #region 将工作时长文本框输入的信息转换为整数

        /// <summary>
        /// 将工作时长文本框输入的信息转换为整数
        /// </summary>
        /// <param name="txtWorkTime"></param>
        /// <returns>-1 表示该文本框输入为空，不判断该项；-2为输入的格式不正确</returns>
        public int ConvertToInt(string txtWorkTime)
        { 
            if (txtWorkTime != "" && txtWorkTime != null)
            {
                try
                {
                    return (int.Parse(txtWorkTime));
                }
                catch
                {
                    try
                    {
                        string[] strarr = txtWorkTime.Split(':');
                        return int.Parse(strarr[0]) * 60+(int.Parse(strarr[1]));
                    }
                    catch
                    { 
                        return -2;
                    }
                }
            }
            else
            {
                return -1;
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
                    XlsQueryCondition condition;
                    _querySetting.TryGetValue<XlsQueryCondition>("attendConditon", out condition);
                    _vmXls.GetXlsWuHuShanAttendanceDetailList(condition.XlsBeginTime, condition.XlsEndTime, condition.XlsDepartIdLst, condition.XlsName, condition.XlsWorkSN, condition.XlsPrincipalId, condition.XlsWorkTypeId, condition.XlsWorkTimeMore,condition.XlsWorkTimeEqual,condition.XlsWorkTimeLess);
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

        #region 工作时长三个文本框控制只允许填一个

        /// <summary>
        /// 工作时长三个文本框控制只允许填一个
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_GotFocus(object sender, RoutedEventArgs e)
        {
            //三个文本框至少有一个不为空时
            if ((txtWorkTimeMore.Text.Trim() != null && txtWorkTimeMore.Text.Trim() != "") || (txtWorkTimeEqual.Text.Trim() != null && txtWorkTimeEqual.Text.Trim() != "") || (txtWorkTimeLess.Text.Trim() != null && txtWorkTimeLess.Text.Trim() != ""))
            {
                txtWorkTimeMore.IsReadOnly = true;
                txtWorkTimeEqual.IsReadOnly = true;
                txtWorkTimeLess.IsReadOnly = true;

                if ((sender as TextBox).Text != null && (sender as TextBox).Text != "")
                {
                    (sender as TextBox).IsReadOnly = false;
                }
            }
            else//三个文本框都为空
            {
                txtWorkTimeMore.IsReadOnly = true;
                txtWorkTimeEqual.IsReadOnly = true;
                txtWorkTimeLess.IsReadOnly = true;
                (sender as TextBox).IsReadOnly = false;
            }
        }

        #endregion
       
        #region 重置查询条件按钮

        /// <summary>
        /// 重置按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            dtpBegin.SelectedDate = null;
            dtpEnd.SelectedDate = null;

            txtdateBegin.Text = "";
            txtdateEnd.Text = "";

            cmbDepart.SelectedIndex = 0;

            txtPersonName.Text = "";
            txtWorkSn.Text = "";

            txtWorkTimeMore.Text= "";
            txtWorkTimeEqual.Text = "";
            txtWorkTimeLess.Text = "";
            txtWorkTimeMore.IsReadOnly = false;
            txtWorkTimeEqual.IsReadOnly = false;
            txtWorkTimeLess.IsReadOnly = false;

            lstPrincipalName.SelectedIndex = -1;
            lstWorkType.SelectedIndex = -1;
        }

        #endregion

        #region 打印预览

        /// <summary>
        /// 打印预览
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
            if (_vmXls.XlsAttendancdDetailListModel.Count() < 1)
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
            int[] departId = new int[0];
            if (_querySetting.Contains("attendConditon"))
            {
                XlsQueryCondition condition;
                _querySetting.TryGetValue<XlsQueryCondition>("attendConditon", out condition);
                beginTime = condition.XlsBeginTime;
                endTime = condition.XlsEndTime.AddDays(-1);
                departId = condition.XlsDepartIdLst;
            }
            else
            {
                WaitingDialog.HideWaiting();
                return;
            }

            #region     生成表头数据
            List<HeaderNode> pageHeaderData = new List<HeaderNode>();

            pageHeaderData.Add(new HeaderNode("序号", 11, 0, 1));           
            pageHeaderData.Add(new HeaderNode("姓名", 11, 0, 1));
            pageHeaderData.Add(new HeaderNode("工号", 11, 0, 1));
            pageHeaderData.Add(new HeaderNode("部门", 11, 0, 1));
            pageHeaderData.Add(new HeaderNode("职务", 11, 0, 1));
            pageHeaderData.Add(new HeaderNode("入井时间", 11, 0, 1));
            pageHeaderData.Add(new HeaderNode("出井时间", 11, 0, 1));
            pageHeaderData.Add(new HeaderNode("下井工作时间", 11, 0, 1));


            ReportHeader reportHeader = new ReportHeader(pageHeaderData);
            #endregion

            #region 表标题
            string time = beginTime.ToLongDateString() + " 到 " + endTime.ToLongDateString();

            string leftTitle = "";
            //int departIndex = 0;
            //foreach (UserDepartInfo item in _vmXlsFilter.DepartInfoModel)
            //{                
            //    if (departId == null)
            //    {
            //        if (departIndex != 0 )
            //        {
            //            leftTitle += string.Format("{0};", item.depart_name);
            //        }
            //    }
            //    else if (departId.Contains(item.depart_id))
            //    {
            //        leftTitle += string.Format("{0};", item.depart_name);
            //    }
            //    departIndex++;
            //}

            //leftTitle = leftTitle.Remove(leftTitle.LastIndexOf(";"), 1);
            leftTitle += DateTime.Now.ToString("打印时间：yyyy-MM-dd HH:mm:ss");

            ReportTitle reportTitle = new ReportTitle("五虎山矿", 20, leftTitle, 11, time, 11, false);
            #endregion

            #region     生成报表页脚数据
            ReportFooter reportFooter = new ReportFooter("", 11);
            #endregion

            string[] bindingPropertyNames = new string[] 
            { 
                "Index",              
                "PersonName",
                "WorkSn",
                "DepartName",
                "PrincipalName",
                "InWellTime",
                "OutWellTime",
                "WorkTime",
            };

            PrintControl printControl = new PrintControl();
            bool res = false;
            res = printControl.SetDataSource<XlsAttendanceDetailList>(reportTitle, reportFooter, reportHeader, _vmXls.XlsAttendancdDetailListModel, bindingPropertyNames, () =>
            {
                if (res)
                {
                    printControl.Preview_CurPage(null, null);
                }
            }, 9);
           
            WaitingDialog.HideWaiting();
        }

        #endregion

        #region 导出excel

        /// <summary>
        /// 导出excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {  
            if (_vmXls.XlsAttendancdDetailListModel.Count() < 1)
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
                XlsQueryCondition condition;
                _querySetting.TryGetValue<XlsQueryCondition>("attendConditon", out condition);
                beginTime = condition.XlsBeginTime;
                endTime = condition.XlsEndTime.AddDays(-1);
            }
            else
            {
                return;
            }
            
            string time = beginTime.ToLongDateString() + " 到 " + endTime.ToLongDateString();
            string space = "                                                                           ";

            ExpExcel.ExportExcelFromDataGrid(dgAttendanceDetailList, 1, 8, (space + time + space), "");   
        }

        #endregion

        #region 排序
        //排序模式
        private string _dir = "asc";
        //排序字段
        private string _sortFiled;
        //排序DataGrid 头
        private DataGridColumnHeader _sortHeader;
        private DataGridColumnHeader _lastSortHeader;

        //显示排序箭头
        private void dgAttendanceDetailList_LayoutUpdated(object sender, EventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        //进行排序
        private void dgAttendanceDetailList_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataGrid dg = sender as DataGrid;
            if (_vmXls.XlsAttendancdDetailListModel == null || dg == null) //非空判断 
            {
                return;
            }

            try
            {

                //获取界面元素
                var uiElement = from element in VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(null), (System.Windows.UIElement)null)
                                where element is DataGridColumnHeader
                                select element;
                int test = uiElement.Count();

                if (uiElement.Count() > 0)
                {
                    //鼠标点击的ColumnHeader 
                    DataGridColumnHeader header = (DataGridColumnHeader)uiElement.First();

                    if (header.Content == null)
                    {
                        return;
                    }
                    //要排序的字段 
                    string newSort = header.Content.ToString();

                    foreach (var col in dg.Columns.Where((columnItem) =>    //判断列头为空的情况
                    {
                        if (columnItem.Header == null)
                        {
                            return false;
                        }
                        return columnItem.Header.ToString() == newSort;
                    }))
                    {
                        //如果有绑定数据
                        if (col.ClipboardContentBinding != null &&
                            col.ClipboardContentBinding.Path != null &&
                            !col.ClipboardContentBinding.Path.Path.Equals(""))
                        {
                            _sortFiled = col.ClipboardContentBinding.Path.Path;
                            break;
                        }
                        return;
                    }

                    //判断升降序
                    if (_dir == "des")
                    {
                        _dir = "asc";
                    }
                    else
                    {
                        _dir = "des";
                    }

                    _lastSortHeader = _sortHeader;
                    _sortHeader = header;

                    //特殊排序自己实现
                    if (_sortFiled == "WorkTime")
                    {
                        SortData(_vmXls.XlsAttendancdDetailListModel);
                    }
                    else
                    {
                        MyDataGridSortInChinese.OrderData(sender, e, ((VmAttendanceDetailList)this.DataContext).XlsAttendancdDetailListModel);
                    }

                }
                MyDataGridSortInChinese.SetColumnSortState();
            }
            catch (Exception err)
            {
                ErrorWindow errWin = new ErrorWindow(err);
                errWin.Show();
            }
        }

        /// <summary>
        /// 调用公共排序
        /// </summary>
        /// <param name="data"></param>
        private void SortData(BaseViewModelCollection<XlsAttendanceDetailList> data)
        {
            if (_dir == "des")//
            {
                IOrderedEnumerable<XlsAttendanceDetailList> sortedObject = data.OrderByDescending(u => u.WorkTimeInt);
                XlsAttendanceDetailList[] sortedData = sortedObject.ToArray();  //执行这一步之后，排序的linq表达式才有效   
                data.Clear();
                foreach (var item in sortedData)
                {
                    data.Add(item);
                }
            }
            else
            {
                IOrderedEnumerable<XlsAttendanceDetailList> sortedObject = data.OrderBy(u => u.WorkTimeInt);
                XlsAttendanceDetailList[] sortedData = sortedObject.ToArray();  //执行这一步之后排序的linq表达式才有效
                data.Clear();
                foreach (var item in sortedData)
                {
                    data.Add(item);
                }
            }
        }

        //显示排序箭头
        private void dgAttendanceDetailList_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        //显示排序箭头
        private void dgAttendanceDetailList_MouseMove(object sender, MouseEventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }
        #endregion
       
    }
}

