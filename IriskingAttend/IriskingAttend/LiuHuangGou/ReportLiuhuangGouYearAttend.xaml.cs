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
using IriskingAttend.ZhongKeHongBa;
using System.Windows.Browser;

namespace IriskingAttend.LiuHuangGou
{
    public partial class ReportLiuhuangGouYearAttend : Page
    {
        #region 私有变量

        private VmLiuhuangGouAttendCollect _vmLiuhuangGouYearCollect = new VmLiuhuangGouAttendCollect();
      
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
        public ReportLiuhuangGouYearAttend()
        {
            InitializeComponent(); 

            dtpBegin.Text = DateTime.Now.AddMonths(-2).ToString("yyyy-MM-01");
            dtpEnd.Text = DateTime.Parse(DateTime.Now.AddMonths(-1).ToString("yyyy-MM-01")).AddDays(-1).ToString("yyyy-MM-dd");
            this.DataContext = _vmLiuhuangGouYearCollect;
            dgPersonYearAttend.ItemsSource = _vmLiuhuangGouYearCollect.PersonMonthAttendModel;

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

            LiuHuangGouQueryCondition qc = new LiuHuangGouQueryCondition();

            qc.BeginTime = this.dtpBegin.SelectedDate.Value.Date;
            if (dtpEnd.Text == null || dtpEnd.Text == "")
            {                
                qc.EndTime = System.DateTime.Now.Date;                
            }
            else
            {
                if (qc.BeginTime >= dtpEnd.SelectedDate.Value.Date.AddDays(1))
                {
                    MsgBoxWindow.MsgBox(
                                "请确定您的开始时间早于截止时间！",
                                Dialog.MsgBoxWindow.MsgIcon.Information,
                                Dialog.MsgBoxWindow.MsgBtns.OK);
                    return;
                }               
                else
                {
                    qc.EndTime = dtpEnd.SelectedDate.Value.Date;
                }
            }
            

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
                    departName.Add(item.depart_name);
                }
                qc.DepartNameList = departName.ToArray();
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

            _vmLiuhuangGouYearCollect.GetPersonAttendCollect(qc.BeginTime, qc.EndTime, qc.DepartNameList, qc.PersonNameList);

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
            if (_vmLiuhuangGouYearCollect.PersonMonthAttendModel.Count() < 1)
            {
                Dialog.MsgBoxWindow.MsgBox(
                            "请查询到数据后再导出Excel！",
                            Dialog.MsgBoxWindow.MsgIcon.Information,
                            Dialog.MsgBoxWindow.MsgBtns.OK);
                return;
            }

            DateTime beginTime = new DateTime();
            DateTime endTime = new DateTime();
            LiuHuangGouQueryCondition condition;
            if (_querySetting.Contains("attendConditon"))
            {
                _querySetting.TryGetValue<LiuHuangGouQueryCondition>("attendConditon", out condition);
                beginTime = condition.BeginTime;
                endTime = condition.EndTime;
            }
            else
            {
                WaitingDialog.HideWaiting();
                return;
            }

            var uri = new Uri(App.Current.Host.Source, string.Format("../LiuHuangGou/硫磺沟出勤统计表.aspx?beginTime={0}&endTime={1}", condition.BeginTime, condition.EndTime));
            HtmlPage.PopupWindow(uri, "_blank", new HtmlPopupWindowOptions());                          
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
