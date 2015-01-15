/*************************************************************************
** 文件名:   ChildWndReportQuery.cs
** 主要类:   ChildWndReportQuery
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   wz
** 日  期:   2013-12-30
** 修改人:   
** 日  期:
** 描  述:   周源山报表查询窗口
**
** 版  本:   1.0.0
** 备  注:  命名及代码编写遵守C#编码规范
**
*************************************************************************/
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
using IriskingAttend.ViewModel;
using System.ServiceModel.DomainServices.Client;
using EDatabaseError;
using IriskingAttend.Common;
using IriskingAttend.CustomUI;
using System.Text;
using System.Diagnostics;

namespace IriskingAttend.ZhouYuanShan
{
    public partial class ChildWndReportQuery : ChildWindow
    {
        #region 单例模式

        private static ChildWndReportQuery _childWndReportQuery;

        public static ChildWndReportQuery GetInstance()
        {
            if (_childWndReportQuery == null)
            {
                _childWndReportQuery = new ChildWndReportQuery(MainPage.ZhouYuanShanQueryReportAction);
                _childWndReportQuery.InitBaseInfos();
            }
            //保留除选择对象以外的其他筛选条件
            //换言之，清空选择对象
            _childWndReportQuery.ClearSelectedObj();
            
            return _childWndReportQuery;
        }

        /// <summary>
        /// 销毁单例对象，在退出登录的时候调用
        /// </summary>
        public static void DestroyInstance()
        {
            if (_childWndReportQuery != null)
            {
                _childWndReportQuery = null;
                System.GC.Collect();
            }
        }

        #endregion


        //选择对象对话框
        private ChildWndSelectObj _childWndSelectObj;

        //回调函数
        private Action<ReportQueryCondition, ChildWindow> _callBack;

        private ChildWndReportQuery(Action<ReportQueryCondition, ChildWindow> callBack)
        {
            _callBack = callBack;
           
            InitializeComponent();

            //默认显示元素为：班次、时间、时长
            this.checkBoxClassOrder.IsChecked = true;
            this.checkBoxDuration.IsChecked = true;
            this.checkBoxTime.IsChecked = true;
           

        }

        //确认
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
           
            if (_callBack != null)
            {
                ReportQueryCondition reportQueryCondition = new ReportQueryCondition();

                try
                {
                    reportQueryCondition.BeginTime = Convert.ToDateTime(this.dateBegin.Text);
                    reportQueryCondition.EndTime = Convert.ToDateTime(this.dateEnd.Text);
                }
                catch (System.Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                reportQueryCondition.ClassOrderIds = GetSelectedClassOrder();
                reportQueryCondition.ShowElementType = GetSelectedShowElement();
                reportQueryCondition.DepartIds = _childWndSelectObj.GetSelectedDepartIds();
                reportQueryCondition.PersonIds = _childWndSelectObj.GetSelectedPersonIds();
                reportQueryCondition.WorkTypeIds = _childWndSelectObj.GetSelectedWorkTypeIds();
                reportQueryCondition.ReportType = GetSelectedReport();

                _callBack(reportQueryCondition,this);
            }
            else
            {
                this.DialogResult = true;
            }
            
        }

        //取消
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        /// <summary>
        /// 选择查询条件(部门，工种，人员)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hBtnSelectObj_Click(object sender, RoutedEventArgs e)
        {
            var TempWnd = _childWndSelectObj.Clone();
            TempWnd.Show();
            TempWnd.Closed += (s, ee) =>
            {
                if (TempWnd.DialogResult.Value)
                {
                    this.textSelectObj.Text = TempWnd.GetSelectObjDescription();
                    _childWndSelectObj = TempWnd;
                }
            };
           
        }

        /// <summary>
        /// 清空选择对象
        /// </summary>
        public void ClearSelectedObj()
        {
            textSelectObj.Text = "";
            if (_childWndSelectObj != null)
            {
                _childWndSelectObj.ClearAllSelectedValue();
            }
        }

        #region 私有功能函数


        //获取选择的班次
        private List<int> GetSelectedClassOrder()
        {
            List<int> classOrderIDs = new List<int>();
            int checkBoxCount = 0;
            foreach (var item in textCmbClassOrder.comboBox.Items)
            {
                if (item is CheckBox)
                {
                    if (((CheckBox)item).IsChecked.Value)
                    {
                        classOrderIDs.Add((int)((CheckBox)item).Tag);
                    }
                    checkBoxCount++;
                }
            }
            //如果是全选，则查询条件为空数组,不作班次过滤，目的为了查询出不完整考勤记录
            if (classOrderIDs.Count == checkBoxCount)
            {
                return new List<int>();
            }

            return classOrderIDs;
        }

        //获取选择的显示元素
        private ShowElementType GetSelectedShowElement()
        {
            int res = 0;
            if (this.checkBoxClassOrder.IsChecked.Value)
            {
                res += (int)ShowElementType.ClassOrder;
            }
            if (this.checkBoxDuration.IsChecked.Value)
            {
                res += (int)ShowElementType.Duration;
            }
            if (this.checkBoxTime.IsChecked.Value)
            {
                res += (int)ShowElementType.Time;
            }
            return (ShowElementType)res;
        }

        //获取选择的报表
        private ReportType GetSelectedReport()
        {
            if (this.radioBtnDailyReportOnPerson.IsChecked.Value)
            {
                return ReportType.DailyReportOnPerson;
            }
            if (this.radioBtnMonthlyReportOnPerson.IsChecked.Value)
            {
                return ReportType.MonthlyReportOnPerson;
            }
            if (this.radioButtonDetailReportOnDepart.IsChecked.Value)
            {
                return ReportType.DetailReportOnDepart;
            }
            if (this.radioButtonMonthlyReportOnDepart.IsChecked.Value)
            {
                return ReportType.MonthlyReportOnDepart;
            }
            return ReportType.None;
        }

        #endregion

        #region 初始化函数

        /// <summary>
        /// 初始化基本信息
        /// </summary>
        private void InitBaseInfos()
        {
            //获取班次
            GetClassOrderInfoRia((result) =>
            {
                textCmbClassOrder.InitCommonOperation();
                foreach (var item in result)
                {
                    CheckBox chkBox = new CheckBox();
                    chkBox.Content = item.class_order_name + "(" + item.attend_sign + ")";
                    chkBox.Tag = item.class_order_id;
                    this.textCmbClassOrder.comboBox.Items.Add(chkBox);
                }
                //初始化多选控件
                textCmbClassOrder.InitComBoBoxItemSelectTrigger();
                textCmbClassOrder.SelectAll();

                //选择对象控件（人员，工种，部门）
                _childWndSelectObj = new ChildWndSelectObj();

                _childWndSelectObj.Init(true, true, true);
                
            });
        }

        /// <summary>
        /// 获取班次信息
        /// </summary>
        private void GetClassOrderInfoRia(Action<List<UserClassOrderInfo>> callBack)
        {
            WaitingDialog.ShowWaiting();
            ServiceDomDbAcess.ReOpenSever();
            

            EntityQuery<UserClassOrderInfo> list = ServiceDomDbAcess.GetSever().GetClassOrderInfosQuery();
            ///回调异常类
            Action<LoadOperation<UserClassOrderInfo>> actionCallBack = ErrorHandle<UserClassOrderInfo>.OnLoadErrorCallBack;
            ///异步事件
            LoadOperation<UserClassOrderInfo> lo = ServiceDomDbAcess.GetSever().Load(list, actionCallBack, null);

            lo.Completed += delegate
            {
                List<UserClassOrderInfo> res = new List<UserClassOrderInfo>();
                //异步获取数据
                foreach (UserClassOrderInfo ar in lo.Entities)
                {
                    res.Add(ar);
                }

                if (callBack != null)
                {
                    callBack(res);
                }
                WaitingDialog.HideWaiting();
            };
        }

        /// <summary>
        /// 设置默认选择的人员
        /// 单例第一次GetInstance时，需要向后台查询信息，
        /// _childWndSelectObj还未初始化,不能紧接着使用此函数
        /// </summary>
        public void InitPersons(string[] personNames)
        {
            _childWndSelectObj.SetSelectedPersons(personNames);
            int index =0;
            StringBuilder sb = new StringBuilder();
            foreach (var item in personNames)
            {
                sb.Append(item);
                sb.Append(',');
                index++;
            }
            if (index > 0)
            {
                sb.Remove(sb.Length - 1, 1);
            }
            textSelectObj.Text += sb.ToString();
        }

        /// <summary>
        /// 设置默认选择的部门
        /// 单例第一次GetInstance时，需要向后台查询信息，
        /// _childWndSelectObj还未初始化,不能紧接着使用此函数
        /// </summary>
        public void InitDeparts(string[] departNames)
        {
            _childWndSelectObj.SetSelectedDeparts(departNames);
            int index = 0;
            StringBuilder sb = new StringBuilder();
            foreach (var item in departNames)
            {
                sb.Append(item);
                sb.Append(',');
                index++;
            }
            if (index > 0)
            {
                sb.Remove(sb.Length - 1, 1);
            }
            textSelectObj.Text += sb.ToString();
        }

        #endregion

    }
}

