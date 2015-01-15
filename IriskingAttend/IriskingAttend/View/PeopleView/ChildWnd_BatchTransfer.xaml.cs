/*************************************************************************
** 文件名:   ChildWnd_BatchTransfer.cs
×× 主要类:   ChildWnd_BatchTransfer
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   cty
** 日  期:   2013-6-14
** 修改人:   
** 日  期:
** 描  述:   ChildWnd_BatchTransfer类,批量调离人员页面
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
using IriskingAttend.ViewModel;
using Irisking.Web.DataModel;
using System.ServiceModel.DomainServices.Client;
using IriskingAttend.Web;
using EDatabaseError;
using IriskingAttend.Common;
using IriskingAttend.ViewModel.PeopleViewModel;

namespace IriskingAttend.View.PeopleView
{
    public partial class ChildWnd_BatchTransfer : ChildWindow
    {

        /// <summary>
        /// 与服务声明
        /// </summary>
        private DomainServiceIriskingAttend m_serviceDomDbAccess = new DomainServiceIriskingAttend();

        //被选中的人员
        BaseViewModelCollection<UserPersonInfo> UserPersonInfos_Selected;
        
        //调离目标部门列表
        List<string> departNames;
        List<int> departIDs;

        //窗口关闭回调函数
        Action<bool?> callback;

        public ChildWnd_BatchTransfer(BaseViewModelCollection<UserPersonInfo> _UserPersonInfos_Selected,Action<bool?> _callback)
        {
            callback = _callback;
            UserPersonInfos_Selected = _UserPersonInfos_Selected;
            InitializeComponent();
            //向后台查询部门信息
            GetDepartNames_ria();

            DG_SelectedPerson.ItemsSource = UserPersonInfos_Selected;
            this.Closed += new EventHandler(ChildWnd_BatchTransfer_Closed);
            init();
        }

        void ChildWnd_BatchTransfer_Closed(object sender, EventArgs e)
        {
            if (callback != null) callback(this.DialogResult);
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            BatchTransfer_ria();
            
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        //获取部门名称
        void GetDepartNames_ria()
        {
            try
            {
                EntityQuery<depart> list = m_serviceDomDbAccess.GetDepartsNameQuery();
                ///回调异常类
                Action<LoadOperation<depart>> actionCallBack = new Action<LoadOperation<depart>>(ErrorHandle<depart>.OnLoadErrorCallBack);
                ///异步事件
                LoadOperation<depart> lo = this.m_serviceDomDbAccess.Load(list, actionCallBack, null);
                lo.Completed += delegate
                {
                    departNames = new List<string>();
                    departIDs = new List<int>();
                    bool hasValue = false;

                    //异步获取数据
                    foreach (depart ar in lo.Entities)
                    {
                        departNames.Add(ar.depart_name);
                        departIDs.Add(ar.depart_id);
                        hasValue = true;
                    }
                    if (hasValue)
                    {
                        this.comb_TargetDepart.ItemsSource = departNames;
                        this.comb_TargetDepart.SelectedIndex = 0;
                    }
                };
            }
            catch (Exception e)
            {

                string s = e.ToString();
            }
        }

        //批量调离 
        void BatchTransfer_ria()
        {
            List<int> selectedPersonIds = new List<int>();
            foreach (var item in UserPersonInfos_Selected)
            {
                if (item.isSelected)
                {
                    selectedPersonIds.Add(item.person_id);
                }
            }
            string departId = "null";
            if (departIDs.Count > comb_TargetDepart.SelectedIndex && comb_TargetDepart.SelectedIndex >= 0)
            {
                departId = departIDs[comb_TargetDepart.SelectedIndex].ToString();
            }

            m_serviceDomDbAccess.BatchTransferPersonsQuery(selectedPersonIds.ToArray(), departId);

            try
            {
                EntityQuery<OptionInfo> list = m_serviceDomDbAccess.BatchTransferPersonsQuery(selectedPersonIds.ToArray(), departId);
                ///回调异常类
                Action<LoadOperation<OptionInfo>> actionCallBack = new Action<LoadOperation<OptionInfo>>(ErrorHandle<OptionInfo>.OnLoadErrorCallBack);
                ///异步事件
                LoadOperation<OptionInfo> lo = this.m_serviceDomDbAccess.Load(list, actionCallBack, null);
                lo.Completed += delegate
                {
              
                    //异步获取数据
                    foreach (OptionInfo ar in lo.Entities)
                    {
                        if (ar.isSuccess)
                        {
                            PublicMethods.MsgBox("操作结果", ar.option_info, Dialog.MsgBoxWindow.MsgIcon.Succeed, Dialog.MsgBoxWindow.MsgBtns.OK);
                        }
                        else
                        {
                            PublicMethods.MsgBox("操作结果", ar.option_info, Dialog.MsgBoxWindow.MsgIcon.Error, Dialog.MsgBoxWindow.MsgBtns.OK);
                        }
                        break;
                    }
                    this.DialogResult = true;
                };
            }
            catch (Exception e)
            {

                string s = e.ToString();
            }
        }



        #region 排序 by cty
        private Dictionary<string, string> dictOrderName = new Dictionary<string, string>();
        private void init()
        {
            if (dictOrderName == null)
                dictOrderName = new Dictionary<string, string>();
            dictOrderName["人员工号"] = "work_sn";
            dictOrderName["人员姓名"] = "person_name";
            dictOrderName["部门"] = "depart_name";
   

        }


        private void DG_SelectedPerson_MouseMove(object sender, MouseEventArgs e)
        {
            MyDataGridSortInChinese.setColumnSortState();
        }

        private void DG_SelectedPerson_LayoutUpdated(object sender, EventArgs e)
        {
            MyDataGridSortInChinese.setColumnSortState();
        }


        private void DG_SelectedPerson_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MyDataGridSortInChinese.OrderData(sender, e, UserPersonInfos_Selected, dictOrderName);
        }

        #endregion

    }
}

