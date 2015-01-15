/*************************************************************************
** 文件名:   HuFengDayAttend.cs
** 主要类:   HuFengDayAttend
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2014-6-19
** 修改人:   
** 日  期:
** 描  述:   HuFengDayAttend，虎峰煤矿日报表
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
using IriskingAttend.NewWuHuShan;

using Lite.ExcelLibrary.CompoundDocumentFormat;
using Lite.ExcelLibrary.BinaryFileFormat;
using Lite.ExcelLibrary.SpreadSheet;
using IriskingAttend.Web;
using IriskingAttend.ApplicationType;
using IriskingAttend.ExportExcel;
using IriskingAttend.ViewModel.PeopleViewModel;

namespace IriskingAttend.HuFeng
{
    public partial class HuFengDayAttend : Page
    {
        #region 私有变量

        private VmHuFengAttend _vmAttend = new VmHuFengAttend(); 
    
        /// <summary>
        /// 本地存储
        /// </summary>
        private IsolatedStorageSettings _querySetting = IsolatedStorageSettings.ApplicationSettings;

        #endregion

        #region 构造函数 初始化

        /// <summary>
        /// 构造函数
        /// </summary>
        public HuFengDayAttend()
        {
            InitializeComponent();
            
            this.DataContext = _vmAttend;
            dgAttendPersonList.ItemsSource = _vmAttend.DayAttendModel;
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

            HuFengDayQueryCondition qc = new HuFengDayQueryCondition();

            qc.BeginTime = this.dtpBegin.SelectedDate.Value.Date;           

            qc.DepartNameList = null;
            //获取选择的部门
            if (txtDepart.Text != "")
            {
                qc.DepartNameList = txtDepart.Text.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            }
            else
            {
                List<string> departName = new List<string>();
                foreach (var item in VmLogin.OperatorDepartInfoList)
                {
                    departName.Add( item.depart_name );
                }
                qc.DepartNameList = departName.ToArray();
            }

            qc.ClassOrderList = null;
            //获取选择的班制
            if (txtClassOrder.Text != "")
            {
                qc.ClassOrderList = txtClassOrder.Text.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            }

            qc.PrincipalList = null;
            //获取选择的班制
            if (txtPrincipal.Text != "")
            {
                qc.PrincipalList = txtPrincipal.Text.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            }

            qc.PersonName = "";
            //获取人员姓名
            if (txtPersonName.Text != "")
            {
                qc.PersonName = txtPersonName.Text.Trim();
            }

            qc.WorkSn = "";
            //获取人员工号
            if (txtWorkSn.Text != "")
            {
                qc.WorkSn = txtWorkSn.Text.Trim();
            }
            

            ///如果存在 先删除该键值对
            if (_querySetting.Contains("attendConditon"))
            {
                _querySetting.Remove("attendConditon");
            }
            _querySetting.Add("attendConditon", qc);

            _vmAttend.GetDayAttendCollect(qc.BeginTime, qc.DepartNameList, qc.ClassOrderList,qc.PrincipalList,qc.PersonName,qc.WorkSn);
             
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
            if (_vmAttend.DayAttendModel.Count() < 1)
            {
                Dialog.MsgBoxWindow.MsgBox(
                            "请查询到数据后再导出Excel！",
                            Dialog.MsgBoxWindow.MsgIcon.Information,
                            Dialog.MsgBoxWindow.MsgBtns.OK);
                return;
            }

            string space = "                                                                           ";
            ExpExcel.ExportExcelFromDataGrid(this.dgAttendPersonList, 1, 9, (space + "虎峰煤矿日考勤查询" + space), "虎峰煤矿日考勤查询");
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
            if (_vmAttend.DayAttendModel.Count() < 1)
            {
                Dialog.MsgBoxWindow.MsgBox(
                           "请查询到数据后再进行打印预览！",
                           Dialog.MsgBoxWindow.MsgIcon.Information,
                           Dialog.MsgBoxWindow.MsgBtns.OK);

                WaitingDialog.HideWaiting();
                return;
            }
            DateTime beginTime = new DateTime();
           
            if (_querySetting.Contains("attendConditon"))
            {
                HuFengDayQueryCondition condition;
                _querySetting.TryGetValue<HuFengDayQueryCondition>("attendConditon", out condition);
                beginTime = condition.BeginTime;               
            }
            else
            {
                WaitingDialog.HideWaiting();
                return;
            }

            #region     生成表头数据
            List<HeaderNode> pageHeaderData = new List<HeaderNode>();

            pageHeaderData.Add(new HeaderNode("序号", 11, 0, 1));
            pageHeaderData.Add(new HeaderNode("工号", 11, 0, 1));
            pageHeaderData.Add(new HeaderNode("姓名", 11, 0, 1));
            pageHeaderData.Add(new HeaderNode("部门", 11, 0, 1));
            pageHeaderData.Add(new HeaderNode("职务", 11, 0, 1));
            pageHeaderData.Add(new HeaderNode("班次", 11, 0, 1));
            pageHeaderData.Add(new HeaderNode("入井时间", 11, 0, 1));
            pageHeaderData.Add(new HeaderNode("出井时间", 11, 0, 1));
            pageHeaderData.Add(new HeaderNode("总工作时间", 11, 0, 1));
            

            ReportHeader reportHeader = new ReportHeader(pageHeaderData);
            #endregion

            #region 表标题
            string time = beginTime.ToLongDateString();

            ReportTitle reportTitle = new ReportTitle("虎峰煤矿日考勤查询", 20, time, 11, false);
            #endregion

            #region     生成报表页脚数据
            ReportFooter reportFooter = new ReportFooter("", 11);
            #endregion

            string[] bindingPropertyNames = new string[] 
            { 
                "Index",
                "WorkSn",
                "PersonName", 
                "DepartName",            
                "PrincipalName", 
                "ClassOrderName", 
                "InWellTime", 
                "OutWellTime", 
                "WorkTime", 
                               
            };

            PrintControl printControl = new PrintControl();
            bool res = false;
            res = printControl.SetDataSource<HuFengDayAttendReport>(reportTitle, reportFooter, reportHeader, _vmAttend.DayAttendModel, bindingPropertyNames, () =>
            {
                if (res)
                {
                    printControl.Preview_CurPage(null, null);
                }
            }, 9);
            WaitingDialog.HideWaiting();
        }

        #endregion

        #region 选取参数条件

        private void btnSelectDepart_Click(object sender, RoutedEventArgs e)
        {            
            VmXlsFilter vmFilter = new VmXlsFilter();
            vmFilter.GetDepartmentByPrivilege();
            vmFilter.DepartLoadCompleted += delegate
            {
                string[] departList = new string[0];
                //获取选择的部门
                if (txtDepart.Text != "")
                {
                    departList = txtDepart.Text.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                }

                vmFilter.DepartInfoModel.RemoveAt(0);

                VmSelectInfoFilter vmInfoFilter = new VmSelectInfoFilter();
                foreach (UserDepartInfo departInfo in vmFilter.DepartInfoModel)
                {
                    SelectInfoFilter infoFilter = new SelectInfoFilter();
                    infoFilter.InfoName = departInfo.depart_name;

                    if (departList.Contains(departInfo.depart_name))
                    {
                        infoFilter.Checked = true;
                    }
                    vmInfoFilter.InfoFilterModel.Add(infoFilter);
                }

                SelectInfo selectInfo = new SelectInfo(vmInfoFilter, txtDepart, ChildWnd_SelectInfo_CallBack);
                selectInfo.Title = "选择部门";
                selectInfo.Show();
            };           
        }

        private void btnSelectClassOrder_Click(object sender, RoutedEventArgs e)
        {
            VmClassOrderMng vmClassOrder = new VmClassOrderMng();
            vmClassOrder.LoadCompletedEvent += (classOrderSender, ergs) =>
            {
                string[] classOrderList = new string[0];
                //获取选择的班次
                if ( txtClassOrder.Text != "")
                {
                    classOrderList = txtClassOrder.Text.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                }

                VmSelectInfoFilter vmInfoFilter = new VmSelectInfoFilter();

                foreach( UserClassOrderInfo classOrderInfo in vmClassOrder.ClassOrderInfos)
                {
                    SelectInfoFilter infoFilter = new SelectInfoFilter();
                    infoFilter.InfoName = classOrderInfo.class_order_name;

                    if (classOrderList.Contains(classOrderInfo.class_order_name))
                    {
                        infoFilter.Checked = true;
                    }
                    vmInfoFilter.InfoFilterModel.Add( infoFilter );
                }                

                SelectInfo selectInfo = new SelectInfo(vmInfoFilter, txtClassOrder,ChildWnd_SelectInfo_CallBack);
                selectInfo.Title = "选择班次";
                selectInfo.Show();
            };
        }

        private void btnSelectPrincipal_Click(object sender, RoutedEventArgs e)
        {
            VmPrincipalMng vmPrincipal = new VmPrincipalMng();
            vmPrincipal.LoadCompletedEvent += (principalSender, ergs) =>
            {
                string[] principalList = new string[0];
                //获取选择的班次
                if (txtClassOrder.Text != "")
                {
                    principalList = txtClassOrder.Text.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                }

                VmSelectInfoFilter vmInfoFilter = new VmSelectInfoFilter();

                foreach (PrincipalInfo principalInfo in vmPrincipal.PrincipalInfos)
                {
                    SelectInfoFilter infoFilter = new SelectInfoFilter();
                    infoFilter.InfoName = principalInfo.principal_name;

                    if (principalList.Contains(principalInfo.principal_name))
                    {
                        infoFilter.Checked = true;
                    }
                    vmInfoFilter.InfoFilterModel.Add(infoFilter);
                }

                SelectInfo selectInfo = new SelectInfo(vmInfoFilter, txtPrincipal, ChildWnd_SelectInfo_CallBack);
                selectInfo.Title = "选择职务";
                selectInfo.Show();
            };
        }

        /// <summary>
        /// 回调函数，得到选取的信息
        /// </summary>
        /// <param name="principalSelect"></param>
        private void ChildWnd_SelectInfo_CallBack( BaseViewModelCollection<SelectInfoFilter> infoSelect, TextBox txtShow)
        {
            string txtShowName = "";
            foreach (SelectInfoFilter ar in infoSelect)
            {
                if (ar.Checked)
                {
                    txtShowName += ar.InfoName + ",";
                }
            }
            if (txtShowName != "")
            {
                txtShow.Text = txtShowName.Remove(txtShowName.LastIndexOf(","), 1);
            }
            else
            {
                txtShow.Text = txtShowName;
            }
        }
        #endregion        
    }

    #region 选择条件

    /// <summary>
    /// 报表查询条件
    /// </summary>
    public class HuFengDayQueryCondition
    {
        /// <summary>
        /// 起始时间
        /// </summary>
        public DateTime BeginTime { get; set; }

        /// <summary>
        /// 终止时间
        /// </summary>
        public DateTime EndTime { get; set; }        

        /// <summary>
        /// 部门名称
        /// </summary>
        public string[] DepartNameList { get; set; }

        /// <summary>
        /// 班次名称
        /// </summary>
        public string[] ClassOrderList { get; set; }

        /// <summary>
        /// 职务名称
        /// </summary>
        public string[] PrincipalList { get; set; }

        /// <summary>
        /// 人员姓名
        /// </summary>
        public string PersonName { get; set; }

        /// <summary>
        /// 工号
        /// </summary>
        public string WorkSn { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public HuFengDayQueryCondition()
        {
            BeginTime = DateTime.MinValue;
            //EndTime = DateTime.Now;
            DepartNameList = null;
            ClassOrderList = null;
            PrincipalList = null;
        }
    }

    #endregion

}
