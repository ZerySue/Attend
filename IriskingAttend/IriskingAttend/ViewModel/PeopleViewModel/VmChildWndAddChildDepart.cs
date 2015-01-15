/*************************************************************************
** 文件名:   VmChildWndAddChildDepart.cs
×× 主要类:   VmChildWndAddChildDepart
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   cty
** 日  期:   2013-6-14
** 修改人:   wz 代码优化
** 日  期:   2013-7-19
** 描  述:   VmChildWndAddChildDepart类,添加子部门
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
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using IriskingAttend.Web;
using Microsoft.Practices.Prism.Commands;
using Irisking.Web.DataModel;
using System.ServiceModel.DomainServices.Client;
using EDatabaseError;
using System.Collections.Generic;
using IriskingAttend.Common;
using IriskingAttend.Dialog;
using IriskingAttend.View;
using IriskingAttend.ViewModel.SystemViewModel;
using System.Text;

namespace IriskingAttend.ViewModel.PeopleViewModel
{
    /// <summary>
    /// 添加子部门viewModel
    /// </summary>
    public class VmChildWndAddChildDepart : BaseViewModel
    {

        #region 字段声明

        /// <summary>
        /// 与服务声明
        /// </summary>
        private DomainServiceIriskingAttend _serviceDomDbAccess = new DomainServiceIriskingAttend();

        /// <summary>
        /// vm加载完毕事件
        /// </summary>
        public event EventHandler LoadCompletedEvent;

        /// <summary>
        ///  窗口关闭事件
        /// </summary>
        public event Action<bool> CloseEvent;
        
        /// <summary>
        /// 管理的父部门信息
        /// </summary>
        private UserDepartInfo _parentDepart;

        #endregion

        #region 与页面绑定的属性
       
        //命令
        public DelegateCommand OkBtnCmd { get; set; }
        public DelegateCommand CancelBtnCmd { get; set; }

        private BaseViewModelCollection<UserDepartInfo> departsInfo;

        /// <summary>
        /// 支持的子部门列表
        /// </summary>
        public BaseViewModelCollection<UserDepartInfo> DepartsInfo
        {
            get { return departsInfo; }
            set
            {
                departsInfo = value;
                this.NotifyPropertyChanged("DepartsInfo");
            }
        }

        /// <summary>
        /// 全选按钮的绑定类
        /// </summary>
        public MarkObject MarkObj { get; set; }


       

        #endregion

        #region 构造函数
        
        public VmChildWndAddChildDepart( UserDepartInfo parentDepart)
        {
            _parentDepart = parentDepart;
            OkBtnCmd = new DelegateCommand(new Action(OkBtnClicked));
            CancelBtnCmd = new DelegateCommand(new Action(CancelBtnClicked));
            GetSupportableDeparts();
        }

        #endregion

        #region 与界面绑定的事件


        private void OkBtnClicked()
        {
            AddChildDepart();

        }

        private void CancelBtnClicked()
        {
            //this.DialogResult = false;
            if (CloseEvent != null)
            {
                CloseEvent( false);
            }
           
        }


        #endregion

        #region wcf ria 操作
        
        /// <summary>
        /// 获取可选的部门名称列表
        /// </summary>
        private void GetSupportableDeparts()
        {
            try
            {
                EntityQuery<UserDepartInfo> list = _serviceDomDbAccess.GetSupportableChildDepartQuery(_parentDepart.depart_id, _parentDepart.parent_depart_id);
                ///回调异常类
                Action<LoadOperation<UserDepartInfo>> actionCallBack = new Action<LoadOperation<UserDepartInfo>>(ErrorHandle<UserDepartInfo>.OnLoadErrorCallBack);
                ///异步事件
                LoadOperation<UserDepartInfo> lo = this._serviceDomDbAccess.Load(list, actionCallBack, null);
                lo.Completed += delegate
                {
                    DepartsInfo = new BaseViewModelCollection<UserDepartInfo>();

                    //异步获取数据
                    foreach (UserDepartInfo ar in lo.Entities)
                    {
                        //只有有权限的部门才能显示
                        if (VmDepartMng.OperateDepartIdCollection.Contains(ar.depart_id))
                        {
                            DepartsInfo.Add(ar);
                        }
                    }
                    if (LoadCompletedEvent != null)
                    {
                        LoadCompletedEvent(this, null);
                    }
                   
                };
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
                WaitingDialog.HideWaiting();
            }
        }

        /// <summary>
        /// 增加子部门操作
        /// </summary>
        private void AddChildDepart()
        {
            string parentDepartID = PublicMethods.ToString(_parentDepart.depart_id);
            List<string> chidlDepartIDs = new List<string>();
            foreach (var item in DepartsInfo)
	        {
                if (item.isSelected)
                {
                    chidlDepartIDs.Add(PublicMethods.ToString(item.depart_id));
                }
            }
            if (chidlDepartIDs.Count < 1)
            {
                MsgBoxWindow.MsgBox(
                          "至少选择一个子部门！",
                          MsgBoxWindow.MsgIcon.Information,
                          MsgBoxWindow.MsgBtns.OK);
                return;
            }
            //获取操作描述
            string description = GetDescription();

            try 
            {
                WaitingDialog.ShowWaiting();
                _serviceDomDbAccess = new DomainServiceIriskingAttend();
                EntityQuery<OptionInfo> list = _serviceDomDbAccess.AddChildDepartQuery(parentDepartID, chidlDepartIDs.ToArray());

                ///回调异常类
                Action<LoadOperation<OptionInfo>> actionCallBack = new Action<LoadOperation<OptionInfo>>(ErrorHandle<OptionInfo>.OnLoadErrorCallBack);
                ///异步事件
                LoadOperation<OptionInfo> lo = this._serviceDomDbAccess.Load(list, actionCallBack, null);
                lo.Completed += delegate
                {
                    IriskingAttend.ViewModel.SystemViewModel.VmOperatorLog.CompleteCallBack closeAction = () =>
                        {
                            if (CloseEvent != null)
                            {
                                CloseEvent(true);
                            }
                        };
                    

                    foreach (OptionInfo item in lo.Entities)
                    {
                        if (!item.isSuccess)
                        {
                            MsgBoxWindow.MsgBox(
                              item.option_info + "！",
                              MsgBoxWindow.MsgIcon.Error,
                              MsgBoxWindow.MsgBtns.OK);
                            VmOperatorLog.InsertOperatorLog(0,"添加子部门", description + item.option_info, closeAction);
                        }
                        else
                        {
                            if (!item.isNotifySuccess)
                            {
                                MsgBoxWindow.MsgBox(
                                 item.option_info + "！",
                                 MsgBoxWindow.MsgIcon.Warning,
                                 MsgBoxWindow.MsgBtns.OK);
                                VmOperatorLog.InsertOperatorLog(1, "添加子部门", description + item.option_info, closeAction);
                            }
                            else
                            {
                                MsgBoxWindow.MsgBox(
                                 item.option_info + "！",
                                 MsgBoxWindow.MsgIcon.Succeed,
                                 MsgBoxWindow.MsgBtns.OK);
                                VmOperatorLog.InsertOperatorLog(1, "添加子部门", description , closeAction);
                            }
                        }

                        break;
                    }
                    
                
                };
                WaitingDialog.HideWaiting();
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
                WaitingDialog.HideWaiting();
            }
        }
       
        #endregion

        #region 给view层提供的接口函数
        /// <summary>
        /// 选中全部人员或者取消选中
        /// <param name="isChecked">是否全选</param>
        /// </summary>
        public void SelectAll(bool? isChecked)
        {
            if (!isChecked.HasValue)
            {
                return;
            }
            if (isChecked.Value)
            {
                foreach (var item in DepartsInfo)
                {
                    item.isSelected = true;
                }
            }
            else
            {
                foreach (var item in DepartsInfo)
                {
                    item.isSelected = false;
                }
            }
        }
        
        /// <summary>
        /// 按需选择Item
        /// </summary>
        /// <param name="userDepartInfos">被选择的对象</param>
        public void SelectItems(UserDepartInfo userDepartInfo)
        {
            userDepartInfo.isSelected = !userDepartInfo.isSelected;
            
            this.MarkObj.Selected = CheckIsAllSelected();
        }
 
        #endregion

        #region 私有功能函数

        /// <summary>
        /// 检查是否有Item被选中
        /// </summary>
        private bool CheckIsAnyOneSelected()
        {
            foreach (var item in DepartsInfo)
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
            foreach (var item in DepartsInfo)
            {
                if (!item.isSelected)
                {
                    return false;
                }
            }
            return true;
        }

        //获取添加子部门操作的描述
        private string GetDescription()
        {
            int index =0;
            StringBuilder description = new StringBuilder(string.Format("父部门名称：{0}\r\n子部门名称：", _parentDepart.depart_name));
            foreach (var item in DepartsInfo)
            {
                if (item.isSelected)
                {
                    description.Append(item.depart_name + "；");
                    index++;
                }
            }
            description.Append("\r\n");
           

            return index == 0 ? "" : description.ToString();
        }
      
        #endregion
    }
}
