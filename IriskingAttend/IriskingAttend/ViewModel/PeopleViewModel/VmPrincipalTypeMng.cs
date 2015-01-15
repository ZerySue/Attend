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
    public class VmPrincipalTypeMng : BaseViewModel
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
        /// 添加新职务类型命令
        /// </summary>
        public DelegateCommand AddCommand
        {
            get;
            set;
        }

        /// <summary>
        /// 批量删除职务类型命令
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

        private BaseViewModelCollection<PrincipalTypeInfo> _principalTypeInfos = new BaseViewModelCollection<PrincipalTypeInfo>();

        /// <summary>
        /// 职务类型信息列表
        /// </summary>
        public BaseViewModelCollection<PrincipalTypeInfo> PrincipalTypeInfos
        {
            get { return _principalTypeInfos; }
            set
            {
                _principalTypeInfos = value;
                this.OnPropertyChanged(() => PrincipalTypeInfos);
            }
        }

        private PrincipalTypeInfo _selectedPrincipalType = null;

        /// <summary>
        /// 当前选择的职务类型
        /// </summary>
        public PrincipalTypeInfo SelectedPrincipalType
        {
            get { return _selectedPrincipalType; }
            set
            {
                _selectedPrincipalType = value;
                this.OnPropertyChanged(() => SelectedPrincipalType);
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

        public VmPrincipalTypeMng()
        {
            AddCommand = new DelegateCommand(AddPrincipalType);
            BatchDeleteCommand = new DelegateCommand(BatchDelete);

            WaitingDialog.ShowWaiting();
            //获取职务类型
            GetPricipalTypesRia((result) =>
                {
                    PrincipalTypeInfos = new BaseViewModelCollection<PrincipalTypeInfo>();
                    foreach (var item in result)
                    {
                        this.PrincipalTypeInfos.Add(item);
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
        private void AddPrincipalType()
        {
            ChildWndOperatePrincipalType cw = new ChildWndOperatePrincipalType(null,
                   ChildWndOptionMode.Add, (o) =>
                   {
                       if (o)
                       {
                           //重新查询
                           _serviceDomDbAccess = new DomainServiceIriskingAttend();
                           GetPricipalTypesRia((principals) =>
                           {
                               PrincipalTypeInfos = new BaseViewModelCollection<PrincipalTypeInfo>();
                               foreach (var item in principals)
                               {
                                   this.PrincipalTypeInfos.Add(item);
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
        public void ModifyCurPrincipalType()
        {
            if (SelectedPrincipalType != null)
            {
                ChildWndOperatePrincipalType cw = new ChildWndOperatePrincipalType(SelectedPrincipalType,
                    ChildWndOptionMode.Modify, (o) =>
                {
                    if (o)
                    {
                        //重新查询
                        _serviceDomDbAccess = new DomainServiceIriskingAttend();
                        GetPricipalTypesRia((principals) =>
                        {
                            PrincipalTypeInfos = new BaseViewModelCollection<PrincipalTypeInfo>();
                            foreach (var item in principals)
                            {
                                this.PrincipalTypeInfos.Add(item);
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
            MsgBoxWindow.MsgBox(string.Format("请注意，您将进行如下操作:\r\n批量删除职务类型！"),
              MsgBoxWindow.MsgIcon.Warning, MsgBoxWindow.MsgBtns.OKCancel, (result) =>
              {
                  if (result == MsgBoxWindow.MsgResult.OK)
                  {
                      List<PrincipalTypeInfo> toBeDelItems = new List<PrincipalTypeInfo>();
                      foreach (var item in PrincipalTypeInfos)
                      {
                          if (item.isSelected)
                          {
                              toBeDelItems.Add(item);
                          }
                      }
                      WaitingDialog.ShowWaiting();
                      BatchDeletePrincipalTypeRia(toBeDelItems.ToArray(), (o) =>
                          {
                              //重新查询
                              _serviceDomDbAccess = new DomainServiceIriskingAttend();
                              GetPricipalTypesRia((items) =>
                              {
                                  PrincipalTypeInfos = new BaseViewModelCollection<PrincipalTypeInfo>();
                                  foreach (var item in items)
                                  {
                                      this.PrincipalTypeInfos.Add(item);
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
        /// 删除当前职务类型
        /// </summary>
        public void DeleteCurPrincipalType()
        {
            if (SelectedPrincipalType != null)
            {
                MsgBoxWindow.MsgBox(string.Format("请注意，您将进行如下操作:\r\n删除职务类型【{0}】！", SelectedPrincipalType.principal_type_name),
                  MsgBoxWindow.MsgIcon.Warning, MsgBoxWindow.MsgBtns.OKCancel, (result) =>
                  {
                      if (result == MsgBoxWindow.MsgResult.OK)
                      {
                          WaitingDialog.ShowWaiting();
                          DeletePrincipalTypeRia(SelectedPrincipalType, (o) =>
                              {
                                  //重新查询
                                  _serviceDomDbAccess = new DomainServiceIriskingAttend();
                                  GetPricipalTypesRia((items) =>
                                  {
                                      PrincipalTypeInfos = new BaseViewModelCollection<PrincipalTypeInfo>();
                                      foreach (var item in items)
                                      {
                                          this.PrincipalTypeInfos.Add(item);
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
                foreach (var item in PrincipalTypeInfos)
                {
                    item.isSelected = true;
                }
            }
            else
            {
                foreach (var item in PrincipalTypeInfos)
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
        public void SelectItems(PrincipalTypeInfo info)
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

            foreach (var item in PrincipalTypeInfos)
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
            if (PrincipalTypeInfos.Count == 0)
            {
                return false;
            }
            foreach (var item in PrincipalTypeInfos)
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
        private void GetPricipalTypesRia(Action<IEnumerable<PrincipalTypeInfo>> riaOperateCallBack)
        {
            try
            {

                EntityQuery<PrincipalTypeInfo> list = _serviceDomDbAccess.GetPrincipalTypesQuery();
                ///回调异常类
                Action<LoadOperation<PrincipalTypeInfo>> actionCallBack = ErrorHandle<PrincipalTypeInfo>.OnLoadErrorCallBack;
                ///异步事件
                LoadOperation<PrincipalTypeInfo> lo = this._serviceDomDbAccess.Load(list, actionCallBack, null);
               
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
        private void BatchDeletePrincipalTypeRia(PrincipalTypeInfo[] principalTypes, Action<OptionInfo> riaOperateCallBack)
        {
            //获取操作描述
            StringBuilder description = new StringBuilder();

            List<int> Ids = new List<int>();
            foreach (var item in principalTypes)
            {
                Ids.Add(item.principal_type_id);
                description.Append(string.Format("职务类型名称：{0}；", item.principal_type_name));
            }
            description.Append("\r\n");
           

            try
            {
                _serviceDomDbAccess.DeletePrincipalType(Ids.ToArray(), CallBackHandleControl<OptionInfo>.OnInvokeErrorCallBack, null);
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
                        VmOperatorLog.InsertOperatorLog(1, "删除职务类型", description.ToString(), completeCallBack);
                    }
                    else if (!optionInfo.isNotifySuccess)
                    {
                        MsgBoxWindow.MsgBox(
                                optionInfo.option_info,
                                Dialog.MsgBoxWindow.MsgIcon.Warning,
                                Dialog.MsgBoxWindow.MsgBtns.OK);
                        VmOperatorLog.InsertOperatorLog(1, "删除职务类型", description.ToString() + optionInfo.option_info, completeCallBack);
                    }
                    else
                    {
                        MsgBoxWindow.MsgBox(
                             optionInfo.option_info,
                             Dialog.MsgBoxWindow.MsgIcon.Error,
                             Dialog.MsgBoxWindow.MsgBtns.OK);
                        VmOperatorLog.InsertOperatorLog(0, "删除职务类型", description.ToString() + optionInfo.option_info, completeCallBack);
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
        private void DeletePrincipalTypeRia(PrincipalTypeInfo info, Action<OptionInfo> riaOperateCallBack)
        {
            PrincipalTypeInfo[] toBeDelete = new PrincipalTypeInfo[1];
            toBeDelete[0] = info;
            BatchDeletePrincipalTypeRia(toBeDelete, riaOperateCallBack);
        }
      
      
        #endregion

    }

 

}
