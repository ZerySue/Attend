/*************************************************************************
** 文件名:   VmPrincipalMng.cs
×× 主要类:   VmPrincipalMng
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   wz
** 日  期:   2013-9-12
** 修改人:   
** 日  期:   
** 描  述:   VmPrincipalMng.cs类,职务信息管理viewmodel
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
using System.Linq;
using System.Windows.Input;
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
using System.Windows.Media.Imaging;
using System.IO;
using System.Text.RegularExpressions;
using IriskingAttend.Dialog;
using IriskingAttend.BehaviorSelf;
using IriskingAttend.ViewModel;
using IriskingAttend.ViewModel.PeopleViewModel;
using IriskingAttend.View;
using IriskingAttend.ViewMine.PeopleView;
using System.Text;
using IriskingAttend.ViewModel.SystemViewModel;

namespace IriskingAttend.ViewModel.PeopleViewModel
{
    public class VmPrincipalMng : BaseViewModel
    {

        #region 字段声明
        
        /// <summary>
        /// 与服务声明
        /// </summary>
        private DomainServiceIriskingAttend _serviceDomDbAccess = new DomainServiceIriskingAttend();

        /// <summary>
        /// viewModel加载完毕事件
        /// </summary>
        public event EventHandler LoadCompletedEvent;

        

        #endregion

        #region 与页面绑定的命令

        /// <summary>
        /// 添加新职务命令
        /// </summary>
        public DelegateCommand AddCommand
        {
            get;
            set;
        }

        /// <summary>
        /// 批量删除职务命令
        /// </summary>
        public DelegateCommand BatchDeleteCommand
        {
            get;
            set;
        }

        #endregion

        #region   与页面绑定的属性

      
        private bool _isBatchBtnEnable = false;
        /// <summary>
        /// 批量操作按钮enable属性
        /// </summary>
        public bool IsBatchBtnEnable
        {
            get { return _isBatchBtnEnable; }
            set
            {
                _isBatchBtnEnable = value;
                this.OnPropertyChanged(()=>IsBatchBtnEnable);
            }
        }

        private BaseViewModelCollection<PrincipalInfo> _principalInfos = new BaseViewModelCollection<PrincipalInfo>();

        /// <summary>
        /// 职务信息列表
        /// </summary>
        public BaseViewModelCollection<PrincipalInfo> PrincipalInfos
        {
            get { return _principalInfos; }
            set
            {
                _principalInfos = value;
                this.OnPropertyChanged(()=>PrincipalInfos);
            }
        }

        private PrincipalInfo _selectedPrincipal = null;

        /// <summary>
        /// 当前选择的职务
        /// </summary>
        public PrincipalInfo SelectedPrincipal
        {
            get { return _selectedPrincipal; }
            set
            {
                _selectedPrincipal = value;
                this.OnPropertyChanged(() => SelectedPrincipal);
            }
        }

          /// <summary>
        /// 全选按钮的绑定类
        /// </summary>
        public MarkObject MarkObj
        { 
            get; 
            set; 
        }
      

        #endregion

        #region 构造函数

        public VmPrincipalMng()
        {
            AddCommand = new DelegateCommand(AddPrincipal);
            BatchDeleteCommand = new DelegateCommand(BatchDelete);

            WaitingDialog.ShowWaiting();
            //获取职务
            GetPricipalsRia((result) =>
                {
                    PrincipalInfos = new BaseViewModelCollection<PrincipalInfo>();
                    foreach (var item in result)
                    {
                        this.PrincipalInfos.Add(item);
                    }
                    if (LoadCompletedEvent != null)
                    {
                        LoadCompletedEvent(this, null);
                    }
                    WaitingDialog.HideWaiting();
                    
                });


        }
        #endregion

        #region 界面的事件响应

        //添加
        private void AddPrincipal()
        {
            ChildWndOperatePrincipal cw = new ChildWndOperatePrincipal(null,
                   ChildWndOptionMode.Add, (o) =>
                   {
                       if (o)
                       {
                           //重新查询
                           _serviceDomDbAccess = new DomainServiceIriskingAttend();
                           GetPricipalsRia((principals) =>
                           {
                               PrincipalInfos = new BaseViewModelCollection<PrincipalInfo>();
                               foreach (var item in principals)
                               {
                                   this.PrincipalInfos.Add(item);
                               }
                               WaitingDialog.HideWaiting();
                           });
                       }

                   });
            cw.Show();
        }

        /// <summary>
        /// 修改当前职务
        /// </summary>
        public void ModifyCurPrincipal()
        {
            if (SelectedPrincipal != null)
            {
                ChildWndOperatePrincipal cw = new ChildWndOperatePrincipal(SelectedPrincipal,
                    ChildWndOptionMode.Modify, (o) =>
                {
                    if (o)
                    {
                        //重新查询
                        _serviceDomDbAccess = new DomainServiceIriskingAttend();
                        GetPricipalsRia((principals) =>
                        {
                            PrincipalInfos = new BaseViewModelCollection<PrincipalInfo>();
                            foreach (var item in principals)
                            {
                                this.PrincipalInfos.Add(item);
                            }
                            WaitingDialog.HideWaiting();
                        });
                    }

                });
                cw.Show();
            }
        }

        //批量删除
        private void BatchDelete()
        {
            MsgBoxWindow.MsgBox(string.Format("请注意，您将进行如下操作:\r\n批量删除职务！"),
              MsgBoxWindow.MsgIcon.Warning, MsgBoxWindow.MsgBtns.OKCancel, (result) =>
              {
                  if (result == MsgBoxWindow.MsgResult.OK)
                  {
                      List<PrincipalInfo> toBeDelPrincipals = new List<PrincipalInfo>();
                      foreach (var item in PrincipalInfos)
                      {
                          if (item.isSelected)
                          {
                              toBeDelPrincipals.Add(item);
                          }
                      }
                      WaitingDialog.ShowWaiting();
                      BatchDeletePrincipalRia(toBeDelPrincipals.ToArray(), (o) =>
                          {
                              //重新查询
                              _serviceDomDbAccess = new DomainServiceIriskingAttend();
                              GetPricipalsRia((principals) =>
                              {
                                  PrincipalInfos = new BaseViewModelCollection<PrincipalInfo>();
                                  foreach (var item in principals)
                                  {
                                      this.PrincipalInfos.Add(item);
                                  }
                                  WaitingDialog.HideWaiting();
                              });
                              this.IsBatchBtnEnable = false;
                              this.MarkObj.Selected = false;
                          });
                  }
              });

        }

        /// <summary>
        /// 删除当前职务
        /// </summary>
        public void DeleteCurPrincipal()
        {
            if (SelectedPrincipal != null)
            {
                MsgBoxWindow.MsgBox(string.Format("请注意，您将进行如下操作:\r\n删除职务【{0}】！", SelectedPrincipal.principal_name),
                  MsgBoxWindow.MsgIcon.Warning, MsgBoxWindow.MsgBtns.OKCancel, (result) =>
                  {
                      if (result == MsgBoxWindow.MsgResult.OK)
                      {
                          WaitingDialog.ShowWaiting();
                          DeletePrincipalRia(SelectedPrincipal, (o) =>
                              {
                                  //重新查询
                                  _serviceDomDbAccess = new DomainServiceIriskingAttend();
                                  GetPricipalsRia((principals) =>
                                  {
                                      PrincipalInfos = new BaseViewModelCollection<PrincipalInfo>();
                                      foreach (var item in principals)
                                      {
                                          this.PrincipalInfos.Add(item);
                                      }
                                      WaitingDialog.HideWaiting();
                                  });
                              });
                      }
                  });
           
            }
        }

        /// <summary>
        /// 选中全部item或者取消选中
        /// </summary>
        /// <param name="sender">checkBox对象</param>
        public void SelectAll(bool? IsChecked)
        {
            if (IsChecked.Value)
            {
                foreach (var item in PrincipalInfos)
                {
                    item.isSelected = true;
                }
            }
            else
            {
                foreach (var item in PrincipalInfos)
                {
                    item.isSelected = false;
                }
            }

            this.IsBatchBtnEnable = CheckIsAnyOneSelected();
            this.MarkObj.Selected = CheckIsAllSelected();
        }

        /// <summary>
        /// 按需选择datagrid的Items
        /// </summary>
        /// <param name="sender">datagrid对象</param>
        public void SelectItems(PrincipalInfo info)
        {
            info.isSelected = !info.isSelected;
            this.IsBatchBtnEnable = CheckIsAnyOneSelected();
            this.MarkObj.Selected = CheckIsAllSelected();
        }

        #endregion

        #region 私有功能函数


        /// <summary>
        /// 检查批量操作按钮的可见性
        /// </summary>
        private bool CheckIsAnyOneSelected()
        {

            foreach (var item in PrincipalInfos)
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
            if (PrincipalInfos.Count == 0)
            {
                return false;
            }
            foreach (var item in PrincipalInfos)
            {
                if (!item.isSelected)
                {
                    return false;
                }
            }
            return true;

        }



     
     

    
       
        #endregion

        #region ria连接后台操作
       
        
        /// <summary>
        /// 向后台发送命令，获取职务列表
        /// </summary>
        /// <param name="riaOperateCallBack">异步操作完成后的回调函数</param>
        private void GetPricipalsRia(Action<IEnumerable<PrincipalInfo>> riaOperateCallBack)
        {
            try
            {

                EntityQuery<PrincipalInfo> list = _serviceDomDbAccess.GetPrincipalsQuery();
                ///回调异常类
                Action<LoadOperation<PrincipalInfo>> actionCallBack = ErrorHandle<PrincipalInfo>.OnLoadErrorCallBack;
                ///异步事件
                LoadOperation<PrincipalInfo> lo = this._serviceDomDbAccess.Load(list, actionCallBack, null);
               
                lo.Completed += delegate
                {
                    riaOperateCallBack(lo.Entities);
                };
            }
            catch (Exception e)
            {
                WaitingDialog.HideWaiting();
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }

        /// <summary>
        /// 向后台发送命令，批量删除职务
        /// </summary>
        /// <param name="ids"></param>
        private void BatchDeletePrincipalRia(PrincipalInfo[] principals, Action<OptionInfo> riaOperateCallBack)
        {
            //获取操作描述
            StringBuilder description = new StringBuilder();

            List<int> principalIds = new List<int>();
            foreach (var item in principals)
            {
                principalIds.Add(item.principal_id);
                description.Append(string.Format("职务名称：{0}；", item.principal_name));
            }
            description.Append("\r\n");
           

            try
            {
                _serviceDomDbAccess.DeletePrincipal(principalIds.ToArray(), CallBackHandleControl<OptionInfo>.OnInvokeErrorCallBack, null);
                CallBackHandleControl<OptionInfo>.m_sendValue = (OptionInfo optionInfo) =>
                {
                     VmOperatorLog.CompleteCallBack completeCallBack = () =>
                     {

                        if (riaOperateCallBack != null)
                        {
                            riaOperateCallBack(optionInfo);
                        }
                     };

                    if (optionInfo.isNotifySuccess && optionInfo.isSuccess)
                    {
                        MsgBoxWindow.MsgBox(
                                 optionInfo.option_info,
                                 Dialog.MsgBoxWindow.MsgIcon.Succeed,
                                 Dialog.MsgBoxWindow.MsgBtns.OK);
                        VmOperatorLog.InsertOperatorLog(1, "删除职务", description.ToString() , completeCallBack);
                    }
                    else if (!optionInfo.isNotifySuccess)
                    {
                        MsgBoxWindow.MsgBox(
                                optionInfo.option_info,
                                Dialog.MsgBoxWindow.MsgIcon.Warning,
                                Dialog.MsgBoxWindow.MsgBtns.OK);
                        VmOperatorLog.InsertOperatorLog(1, "删除职务", description.ToString() + optionInfo.option_info, completeCallBack);
                    }
                    else
                    {
                        MsgBoxWindow.MsgBox(
                             optionInfo.option_info,
                             Dialog.MsgBoxWindow.MsgIcon.Error,
                             Dialog.MsgBoxWindow.MsgBtns.OK);
                        VmOperatorLog.InsertOperatorLog(0, "删除职务", description.ToString() + optionInfo.option_info, completeCallBack);
                    }

                   
                };
            }
            catch (Exception e)
            {
                WaitingDialog.HideWaiting();
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
           


        }

        /// <summary>
        /// 向后台发送命令，删除单个职务
        /// </summary>
        /// <param name="principalInfo"></param>
        private void DeletePrincipalRia(PrincipalInfo principalInfo, Action<OptionInfo> riaOperateCallBack)
        {
            PrincipalInfo[] toBeDelete = new PrincipalInfo[1];
            toBeDelete[0] = principalInfo;
            BatchDeletePrincipalRia(toBeDelete, riaOperateCallBack);
        }
      
      
        #endregion

    }

 

}
