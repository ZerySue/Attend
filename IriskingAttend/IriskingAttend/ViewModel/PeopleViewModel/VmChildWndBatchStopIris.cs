/*************************************************************************
** 文件名:   VmChildWndBatchStopIris.cs
×× 主要类:   VmChildWndBatchStopIris
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   cty
** 日  期:   2013-6-14
** 修改人:   wz 代码优化
** 日  期:   2013-7-19
** 描  述:   VmChildWndBatchStopIris类,停用虹膜
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
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Practices.Prism.Commands;
using System.Collections.Generic;
using System.ServiceModel.DomainServices.Client;
using Irisking.Web.DataModel;
using EDatabaseError;
using IriskingAttend.Web;
using IriskingAttend.Common;
using System.IO.IsolatedStorage;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using IriskingAttend.View.PeopleView;
using IriskingAttend.Dialog;
using System.Windows.Navigation;
using IriskingAttend.View;
using System.Text;
using IriskingAttend.ViewModel.SystemViewModel;

namespace IriskingAttend.ViewModel.PeopleViewModel
{
    /// <summary>
    /// 停用虹膜界面ViewModel
    /// </summary>
    public class VmChildWndBatchStopIris : BaseViewModel
    {

        #region 字段声明

        /// <summary>
        /// 与服务声明
        /// </summary>
        private DomainServiceIriskingAttend serviceDomDbAccess = new DomainServiceIriskingAttend();
        
        private List<string> beginTime; //停用虹膜开始时间
        private List<string> endTime;   //停用虹膜结束时间
        private List<int> policy;       //停用虹膜策略
        private bool isBatch;         //是否批量操作
        BaseViewModelCollection<UserPersonInfo> persons; //被选择的人员
        List<int> personIDs;

        public event EventHandler CloseEvent;

        #endregion

        #region    与界面绑定的属性
      
        /// <summary>
        /// //提交命令
        /// </summary>
        public DelegateCommand OkBtnCmd { get; set; }
        
        /// <summary>
        /// //批量删除命令
        /// </summary>
        public DelegateCommand BatchDeleteSelectedCmd { get; set; }
        
        /// <summary>
        /// 删除所有停用虹膜命令
        /// </summary>
        public DelegateCommand DelAllStopIrisRecordsCmd { get; set; }
        
        /// <summary>
        /// 取消命令
        /// </summary>
        public DelegateCommand CancelBtnCmd { get; set; }

        /// <summary>
        /// 全选按钮的绑定类
        /// </summary>
        public MarkObject MarkObj { get; set; }
     
        private Visibility delAllStopIrisVisibility;

        /// <summary>
        /// 删除所有停用虹膜记录按钮可见性
        /// </summary>
        public Visibility DelAllStopIrisVisibility
        {
            get { return delAllStopIrisVisibility; }
            set
            {
                delAllStopIrisVisibility = value;
                this.NotifyPropertyChanged("DelAllStopIrisVisibility");
            }
        }

        
        private bool isBatchOperateBtnEnable;

        /// <summary>
        /// 对勾选的item批量操作按钮enable属性  by wz
        /// </summary>
        public bool IsBatchOperateBtnEnable
        {
            get { return isBatchOperateBtnEnable; }
            set
            {
                isBatchOperateBtnEnable = value;
                this.NotifyPropertyChanged("IsBatchOperateBtnEnable");
            }
        }

        
        private bool okBtnIsEnabled;

        /// <summary>
        /// 提交按钮是否可以点击
        /// </summary>
        public bool OkBtnIsEnabled
        {
            get { return okBtnIsEnabled; }
            set
            {
                okBtnIsEnabled = value;
                this.NotifyPropertyChanged("OkBtnIsEnabled");
            }
        }

        /// <summary>
        /// 多个人员的停用虹膜信息表
        /// </summary>
        public BaseViewModelCollection<PersonStopIrisInfo> PersonStopIrisInfoBatch
        {
            get;
            set;
        }

        private BaseViewModelCollection<PersonStopIrisInfo> personStopIrisInfo;

        /// <summary>
        /// 人员停用虹膜信息表
        /// </summary>
        public BaseViewModelCollection<PersonStopIrisInfo> PersonStopIrisInfo
        {
            get { return personStopIrisInfo; }
            set
            {
                personStopIrisInfo = value;
                this.NotifyPropertyChanged("PersonStopIrisInfo");
            }
        }

        #endregion

        #region 构造函数
        
        public VmChildWndBatchStopIris(BaseViewModelCollection<UserPersonInfo> persons,bool _IsBatch)
        {
            isBatch = _IsBatch;
            this.persons = persons;
            OkBtnCmd = new DelegateCommand(new Action(AddStopIrisRecords));//提交
            BatchDeleteSelectedCmd = new DelegateCommand(new Action(BatchDeleteSelectedClicked));//批量删除
            DelAllStopIrisRecordsCmd = new DelegateCommand(new Action(DelAllStopIrisRecords));//删除所有停用虹膜记录
            PersonStopIrisInfo = new BaseViewModelCollection<PersonStopIrisInfo>();
            PersonStopIrisInfoBatch = new BaseViewModelCollection<PersonStopIrisInfo>();
          
            //当时批量停用虹膜时 by cty
            if (isBatch)
            {
                DelAllStopIrisVisibility = Visibility.Visible;
            }
            //操作单个人的停用虹膜时 by cty
            else
            {
                DelAllStopIrisVisibility = Visibility.Collapsed;   
            }

            personIDs = new List<int>();
            foreach (var item in persons)
            {
                personIDs.Add(item.person_id);
            }

            GetPersonStopIrisTable(personIDs.ToArray(), isBatch);
        }

        #endregion

        #region WCF Ria操作
        
        /// <summary>
        /// 加载停用虹膜的人员信息及停用虹膜信息 by cty
        /// </summary>
        /// <param name="personid"></param>
        /// <param name="isbatch"></param>
        private void GetPersonStopIrisTable(int[] personid, bool isbatch)
        {
            //if (personid == null)
            //    return;
            ServiceDomDbAcess.ReOpenSever();
            EntityQuery<PersonStopIrisInfo> list = ServiceDomDbAcess.GetSever().GetPersonStopIrisTableQuery(personid);
            ///回调异常类
            Action<LoadOperation<PersonStopIrisInfo>> actionCallBack = new Action<LoadOperation<PersonStopIrisInfo>>(ErrorHandle<PersonStopIrisInfo>.OnLoadErrorCallBack);
            ///异步事件
            LoadOperation<PersonStopIrisInfo> lo = ServiceDomDbAcess.GetSever().Load(list, actionCallBack, null);
            lo.Completed += delegate
            {
                //PersonStopInfos.Clear();
                foreach (var item in lo.Entities)/////////////
                {
                    
                    for (int i = 0; i < personid.Length; i++)
                    {
                        if (item.person_id == personid[i])
                        {
                            if (isbatch)
                            {
                                PersonStopIrisInfoBatch.Add(item);
                            }
                            else
                            {
                                PersonStopIrisInfo.Add(item);
                            }

                        }
                    }
                }
            };
        }

        #endregion
       
        #region 给view层的接口函数
        
        /// <summary>
        /// 选中全部人员或者取消选中
        /// </summary>
        /// <param name="sender">datagrid对象</param>
        /// <param name="personids"></param>
        public void SelectAllPerson(bool? isChecked)
        {
            if (!isChecked.HasValue)
            {
                return;
            }
            if (isChecked.Value)
            {
                foreach (var item in PersonStopIrisInfo)
                {
                    item.isSelected = true;
                }
            }
            else
            {
                foreach (var item in PersonStopIrisInfo)
                {
                    item.isSelected = false;
                }
            }
           
            this.IsBatchOperateBtnEnable = CheckIsAnyOneSelected();
            this.MarkObj.Selected = CheckIsAllSelected();
        }

        /// <summary>
        /// 按需选择datagrid的Items
        /// </summary>
        /// <param name="sender">datagrid对象</param>
        public void SelectItems(PersonStopIrisInfo stopIrisInfo)
        {


            if (stopIrisInfo != null)
            {
                stopIrisInfo.isSelected = !stopIrisInfo.isSelected;
            }

            this.IsBatchOperateBtnEnable = CheckIsAnyOneSelected();
            this.MarkObj.Selected = CheckIsAllSelected();
        }

        /// <summary>
        /// 添加停用虹膜记录函数 by cty
        /// </summary>
        /// <param name="beginTime">停用虹膜记录开始时间</param>
        /// <param name="endTime">停用虹膜记录结束时间</param>
        /// <param name="policy">策略</param>
        public void PersonStopIrisAddClicked(DateTime beginTime, DateTime endTime, int policy)
        {
            foreach (PersonStopIrisInfo item in PersonStopIrisInfoBatch)
            {
                bool a = (Convert.ToDateTime(item.begin_time) < endTime && Convert.ToDateTime(item.end_time) > beginTime && item.begin_time != null);
                bool b = (item.end_time == null && Convert.ToDateTime(item.begin_time) < endTime);
                if (a || b)
                {
                    MsgBoxWindow.MsgBox(
                              "您新增加的时间段与与已存在的时间段有冲突，请重设！",
                              MsgBoxWindow.MsgIcon.Information,
                              MsgBoxWindow.MsgBtns.OK);
                    return;
                }
            }

            foreach (PersonStopIrisInfo item in PersonStopIrisInfo)
            {
                bool a = (Convert.ToDateTime(item.begin_time) < endTime && Convert.ToDateTime(item.end_time) > beginTime && item.begin_time != null);
                bool b = (item.end_time == null && Convert.ToDateTime(item.begin_time) < endTime);
                if (a || b)
                {
                    MsgBoxWindow.MsgBox(
                              "您新增加的时间段与与已存在的时间段有冲突，请重设！",
                              MsgBoxWindow.MsgIcon.Information,
                              MsgBoxWindow.MsgBtns.OK);
                    return;
                }
            }



            PersonStopIrisInfo dg_psii = new PersonStopIrisInfo();
            dg_psii.begin_time = beginTime.ToString();
            if (endTime == DateTime.MaxValue)
            {
                dg_psii.end_time = null;
            }
            else
            {
                dg_psii.end_time = endTime.ToString();
            }
            dg_psii.policy = IntToString(policy);
            dg_psii.isSelected = false;
            PersonStopIrisInfo.Add(dg_psii);
            OkBtnIsEnabled = true;

        }

        #endregion

        #region 事件响应函数

        /// <summary>
        /// 批量删除命令 by cty
        /// </summary>
        private void BatchDeleteSelectedClicked()
        {
            bool IsHaveSelected = true;
            BaseViewModelCollection<PersonStopIrisInfo> tmp = new BaseViewModelCollection<PersonStopIrisInfo>();
            foreach (var item in PersonStopIrisInfo)
            {
                tmp.Add(item);
            }
            foreach (var item in tmp)
            {
                if (item.isSelected)
                {
                    PersonStopIrisInfo.Remove(item);
                    OkBtnIsEnabled = true;
                    IsHaveSelected = false;
                }
            }
            if (IsHaveSelected)
            {
                MsgBoxWindow.MsgBox(
                            "请至少选中一个要删除的记录！",
                            MsgBoxWindow.MsgIcon.Information,
                            MsgBoxWindow.MsgBtns.OK);
            }
            this.IsBatchOperateBtnEnable = CheckIsAnyOneSelected();
            this.MarkObj.Selected = CheckIsAllSelected();
        }
        
        /// <summary>
        /// 删除所有停用虹膜的记录 by cty
        /// </summary>
        private void DelAllStopIrisRecords()
        {
            //获取描述
            StringBuilder description = new StringBuilder("");
            foreach (var item in persons)
            {
                description.Append(string.Format("姓名：{0}，工号：{1}；", item.person_name, item.work_sn));
            }
            description.Append("\r\n");

            WaitingDialog.ShowWaiting();
            Action<InvokeOperation<OptionInfo>> onInvokeErrorCallBack = CallBackHandleControl<OptionInfo>.OnInvokeErrorCallBack;
            CallBackHandleControl<OptionInfo>.m_sendValue = (result) =>
            {


                if (result.isSuccess && result.isNotifySuccess)
                {
                    PersonStopIrisInfo.Clear();
                    MsgBoxWindow.MsgBox(
                           "删除当前人员所有的停止虹膜日期记录\r\n操作成功！",
                           MsgBoxWindow.MsgIcon.Succeed,
                           MsgBoxWindow.MsgBtns.OK);

                    //清空临时列表中的记录 bug 3066 by wz
                    PersonStopIrisInfoBatch.Clear();

                    //操作员日志
                    VmOperatorLog.InsertOperatorLog(1, "删除所有的停用虹膜记录", description.ToString() , null);
                }
                else
                {
                    if (!result.isSuccess)
                    {
                        MsgBoxWindow.MsgBox(
                         result.option_info + "！",
                         MsgBoxWindow.MsgIcon.Error,
                         MsgBoxWindow.MsgBtns.OK);
                        //操作员日志
                        VmOperatorLog.InsertOperatorLog(0, "删除所有的停用虹膜记录", description.ToString() + result.option_info, null);
               
                    }
                    else if (!result.isNotifySuccess)
                    {
                        MsgBoxWindow.MsgBox(
                         result.option_info + "！",
                         MsgBoxWindow.MsgIcon.Warning,
                         MsgBoxWindow.MsgBtns.OK);
                        //操作员日志
                        VmOperatorLog.InsertOperatorLog(1, "删除所有的停用虹膜记录", description.ToString() + result.option_info, null);
               
                    }
                }

                WaitingDialog.HideWaiting();
            };
            serviceDomDbAccess.DelAllStopIrisRecords(personIDs.ToArray(), onInvokeErrorCallBack, null);

        }

        /// <summary>
        /// 提交命令 okBtn响应函数
        /// </summary>
        private void AddStopIrisRecords()
        {
            //获取描述
            StringBuilder description = new StringBuilder();
           
            foreach (var item in persons)
            {
                description.Append(string.Format("姓名：{0}，工号：{1}；",item.person_name,item.work_sn));
            }
            description.Append("\r\n");
            
            string desripTitle = isBatch ? 
                "添加停用虹膜记录" : "设置停用虹膜记录";

            if (PersonStopIrisInfo.Count == 0)
            {
                description.Append("删除该人员的停用虹膜记录；\r\n");
            }
            else
            {
                description.Append("详细停用虹膜记录如下：\r\n");
            }

            beginTime = new List<string>();
            endTime = new List<string>();
            policy = new List<int>();
            foreach (PersonStopIrisInfo item in PersonStopIrisInfo)
            {
                beginTime.Add(item.begin_time);
                endTime.Add(item.end_time);
                policy.Add(StringToInt(item.policy));
                description.Append(string.Format("{0}.开始时间:{1}，结束时间:{2}，策略:{3}；\r\n",
                    policy.Count, item.begin_time, item.end_time, item.policy));
            }

            if (personIDs.ToArray().Length > 0)
            {
                WaitingDialog.ShowWaiting();
                Action<InvokeOperation<OptionInfo>> callBack1 = CallBackHandleControl<OptionInfo>.OnInvokeErrorCallBack;
                CallBackHandleControl<OptionInfo>.m_sendValue = (optionInfo) =>
                {
                    VmOperatorLog.CompleteCallBack CompleteCallBack = () =>
                        {
                            if (CloseEvent != null)
                            {
                                this.CloseEvent(null, null);
                            }
                        };
                    if (optionInfo.isSuccess && optionInfo.isNotifySuccess)
                    {
                        MsgBoxWindow.MsgBox(
                            "设置当前人员的停止虹膜日期记录\r\n操作成功！",
                            MsgBoxWindow.MsgIcon.Succeed,
                            MsgBoxWindow.MsgBtns.OK);
                        OkBtnIsEnabled = false;
                        if (!isBatch || PersonStopIrisInfo.Count != 0) //批量操作且无停用虹膜记录时，对数据库无操作，不写日志
                        {
                            VmOperatorLog.InsertOperatorLog(1, desripTitle, description.ToString(), CompleteCallBack);
                        }
                       
                    }
                    else
                    {
                        if (!optionInfo.isSuccess)
                        {
                            MsgBoxWindow.MsgBox(
                               optionInfo.option_info + "！",
                               MsgBoxWindow.MsgIcon.Error,
                               MsgBoxWindow.MsgBtns.OK);
                            if (!isBatch || PersonStopIrisInfo.Count != 0)  //批量操作且无停用虹膜记录时，对数据库无操作，不写日志
                            {
                                VmOperatorLog.InsertOperatorLog(0, desripTitle, description.ToString() + optionInfo.option_info, CompleteCallBack);
                            }
                           
                        }
                        else
                        {
                            MsgBoxWindow.MsgBox(
                               optionInfo.option_info + "！",
                               MsgBoxWindow.MsgIcon.Warning,
                               MsgBoxWindow.MsgBtns.OK);
                            if (!isBatch || PersonStopIrisInfo.Count != 0) //批量操作且无停用虹膜记录时，对数据库无操作，不写日志
                            {
                                VmOperatorLog.InsertOperatorLog(1, desripTitle, description.ToString() + optionInfo.option_info, CompleteCallBack);
                            }
                            
                        }
                       
                    }
                    WaitingDialog.HideWaiting();
                };
                serviceDomDbAccess.SetStopIrisRecords(personIDs.ToArray(), beginTime, endTime, policy, isBatch, callBack1, null);
            }

            else
            {
                MsgBoxWindow.MsgBox(
                         "请选择要添加停用虹膜的人员！",
                         MsgBoxWindow.MsgIcon.Information,
                         MsgBoxWindow.MsgBtns.OK);
            }
        }
       
        #endregion

        #region 私有功能函数
       
        /// <summary>
        /// 检查是否有item被选中
        /// </summary>
        private bool CheckIsAnyOneSelected()
        {
            foreach (var item in PersonStopIrisInfo)
            {
                if (item.isSelected)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 检查Item是否全部被选中
        /// </summary>
        private bool CheckIsAllSelected()
        {
            if (PersonStopIrisInfo == null || PersonStopIrisInfo.Count == 0)
            {
                return false;
            }

            foreach (var item in PersonStopIrisInfo)
            {
                if (!item.isSelected)
                {
                    return false;

                }
            }
            return true;

        }

        private string IntToString(int policy)
        {
            string policy_str = "";
            switch (policy)
            {
                case 0: 
                    policy_str = "自动变为启用";
                    break;
                case 1: 
                    policy_str = "删除该用户虹膜信息";
                    break;
                case 2: 
                    policy_str = "删除该用户人员信息";
                    break;
            }
            return policy_str;
        }

        private int StringToInt(string policy_str)
        {
            int policy = 0;
            switch (policy_str)
            {
                case "自动变为启用": policy = 0;
                    break;
                case "删除该用户虹膜信息": policy = 1;
                    break;
                case "删除该用户人员信息": policy = 2;
                    break;
            }
            return policy;
        }

        #endregion
    }

 
}
