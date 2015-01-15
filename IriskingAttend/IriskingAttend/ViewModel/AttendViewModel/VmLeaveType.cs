/*************************************************************************
** 文件名:   VmLeaveType.cs
** 主要类:   VmLeaveType
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   lzc
** 日  期:   2013-4-11
** 修改人:   cty
** 日  期:   2013-10-29
 *修改内容： 增加考勤类型的增删改查功能
** 描  述:   VmLeaveType，主要是考勤类型
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
using IriskingAttend.Web;
using Irisking.Web.DataModel;
using System.ServiceModel.DomainServices.Client;
using EDatabaseError;
using Microsoft.Practices.Prism.Commands;
using IriskingAttend.View;
using IriskingAttend.Dialog;
using IriskingAttend.Common;
using System.Collections.Generic;
using IriskingAttend.BehaviorSelf;
using IriskingAttend.ViewModel.SystemViewModel;

namespace IriskingAttend.ViewModel
{
    public class VmLeaveType : BaseViewModel
    {
        #region 绑定数据

        /// <summary>
        /// 绑定数据源
        /// </summary>
        public BaseViewModelCollection<LeaveType> LeaveTypeModel { get; set; }

        private bool _isBatchBtnEnable = false;
        /// <summary>
        /// cty
        /// 批量操作按钮enable属性
        /// </summary>
        public bool IsBatchBtnEnable
        {
            get 
            {
                return _isBatchBtnEnable; 
            }
            set
            {
                _isBatchBtnEnable = value;
                this.OnPropertyChanged(() => IsBatchBtnEnable);
            }
        }


        private LeaveType _selectedLeaveType = null;
        /// <summary>
        /// 当前选择的考勤类型
        /// </summary>
        public LeaveType SelectedLeaveType
        {
            get { return _selectedLeaveType; }
            set
            {
                _selectedLeaveType = value;
                this.OnPropertyChanged(() => SelectedLeaveType);
            }
        }

        /// <summary>
        /// cty
        /// 全选按钮的绑定类
        /// </summary>
        public MarkObject MarkObj
        {
            get;
            set;
        }

        #endregion


        #region 与页面绑定的命令(cty)

        /// <summary>
        /// cty
        /// 添加命令
        /// </summary>
        public DelegateCommand AddCommand
        {
            get;
            set;
        }

        /// <summary>
        /// cty
        /// 批量删除命令
        /// </summary>
        public DelegateCommand BatchDeleteCommand
        {
            get;
            set;
        }

        #endregion

        #region 事件

        /// <summary>
        /// 请假类型加载完成
        /// </summary>
        public event EventHandler LeaveTypeLoadCompleted;

        #endregion

        #region 构造函数

        public VmLeaveType()
        {
            AddCommand = new DelegateCommand(AddLeaveType);
            BatchDeleteCommand = new DelegateCommand(BatchDelete);
            MarkObj = new MarkObject();
            LeaveTypeModel = new BaseViewModelCollection<LeaveType>();
            ServiceDomDbAcess.ReOpenSever();
            LeaveTypeLoadCompleted += (a,e)=>{};
        }

        #endregion

        #region 函数

        /// <summary>
        /// index 选择请假类型、还是考勤类型1-50为考勤默认类型 50以外为请假类型，用户增加的考勤类型
        /// </summary>
        /// <param name="index"></param>
        public void GetLeaveType(int index)
        {
            try
            {
                EntityQuery<LeaveType> lstLeaveType = ServiceDomDbAcess.GetSever().IrisGetLeaveTypeQuery(0);

                ///回调异常类
                Action<LoadOperation<LeaveType>> getLeaveTypeCallBack = new Action<LoadOperation<LeaveType>>
                    (ErrorHandle<LeaveType>.OnLoadErrorCallBack);
                ///异步事件
                LoadOperation<LeaveType> lo = ServiceDomDbAcess.GetSever().Load(lstLeaveType, getLeaveTypeCallBack, null);

                lo.Completed += (o, e) =>
                {
                    LeaveTypeModel.Clear();
                    //LeaveType leave_typeAll = new LeaveType { leave_type_name = "全部", leave_type_id = 0 };//by cty 注释掉
                    //LeaveTypeModel.Add(leave_typeAll);//by cty 注释掉
                    foreach (var ar in lo.Entities.Where(a=>a.leave_type_id >= index))
                    {
                        LeaveTypeModel.Add(ar);
                    }

                    #region 重新排序

                    IOrderedEnumerable<LeaveType> sortedObject = LeaveTypeModel.OrderBy(a =>a.leave_type_name);
                    LeaveType[] sortedData = sortedObject.ToArray();  //执行这一步之后排序的linq表达式才有效

                    LeaveTypeModel.Clear();
                    foreach (var item in sortedData)
                    {
                        LeaveTypeModel.Add(item);
                    }

                    #endregion
                    LeaveTypeLoadCompleted(this,new EventArgs());
                };
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }

        #endregion

        #region 回调函数

        /// <summary>
        /// cty
        /// 回调函数
        /// </summary>
        /// <param name="DialogResult"></param>
        private void LeaveTypeManage_callback(bool? DialogResult)
        {
            GetLeaveType(50);
        }

        #endregion

        #region 添加修改考勤类型

        /// <summary>
        /// cty
        /// 修改考勤类型
        /// </summary>
        public void ModifyLeaveType()
        {
            if (SelectedLeaveType != null)
            {
                LeaveTypeManage ltm = new LeaveTypeManage(SelectedLeaveType, ChildWndOptionMode.Modify, LeaveTypeManage_callback);
                ltm.Show();
            }
        }

        /// <summary>
        /// cty
        /// 添加考勤类型
        /// </summary>
        public void AddLeaveType()
        {
            LeaveTypeManage ltm = new LeaveTypeManage(null, ChildWndOptionMode.Add, LeaveTypeManage_callback);
            ltm.Show();
        }

        #endregion

        #region 删除考勤类型

        /// <summary>
        /// cty
        /// 删除考勤类型
        /// </summary>
        public void DeleteLeaveType()
        {
            if (SelectedLeaveType != null)
            {
                if (SelectedLeaveType.leave_type_id < 50)
                {
                    MsgBoxWindow.MsgBox("该项为系统预设请假类型，不允许删除！",
                                        MsgBoxWindow.MsgIcon.Information,
                                        MsgBoxWindow.MsgBtns.OK);
                    return;
                }
                List<int> ids = new List<int>();
                ids.Add(SelectedLeaveType.leave_type_id);

                MsgBoxWindow.MsgBox("您确定要删除该请假类型吗？", MsgBoxWindow.MsgIcon.Question, MsgBoxWindow.MsgBtns.OKCancel, (result) =>
                {
                    if (result == MsgBoxWindow.MsgResult.OK)
                    {
                        WaitingDialog.ShowWaiting();
                        Action<InvokeOperation<bool>> onInvokeErrorCallBack = CallBackHandleControl<bool>.OnInvokeErrorCallBack;
                        CallBackHandleControl<bool>.m_sendValue = (o) =>
                        {
                            if (o)
                            {
                                //添加操作员日志
                                VmOperatorLog.InsertOperatorLog(1, "删除请假类型", "请假类型名称：" + SelectedLeaveType.leave_type_name, () =>
                                {
                                    MsgBoxWindow.MsgBox(
                                        "删除请假类型成功！",
                                        MsgBoxWindow.MsgIcon.Succeed,
                                        MsgBoxWindow.MsgBtns.OK);
                                    GetLeaveType(50);
                                }); 
                                
                            }
                            else
                            {
                                  //添加操作员日志
                                VmOperatorLog.InsertOperatorLog(0, "删除请假类型", "请假类型名称：" + SelectedLeaveType.leave_type_name, () =>
                                {
                                    MsgBoxWindow.MsgBox(
                                           "删除请假类型失败！",
                                           MsgBoxWindow.MsgIcon.Error,
                                           MsgBoxWindow.MsgBtns.OK);
                                });
                            }

                            WaitingDialog.HideWaiting();
                        };
                        ServiceDomDbAcess.GetSever().IrisDeleteLeaveType(ids.ToArray(), onInvokeErrorCallBack, null);
                    }
                });
            }
        }

        #endregion

        #region 批量删除

        /// <summary>
        /// cty
        /// 批量删除考勤类型
        /// </summary>
        private void BatchDelete()
        {
            MsgBoxWindow.MsgBox(string.Format("请注意，您将进行如下操作:\r\n批量删除请假类型！"),
              MsgBoxWindow.MsgIcon.Warning, MsgBoxWindow.MsgBtns.OKCancel, (result) =>
              {
                  if (result == MsgBoxWindow.MsgResult.OK)
                  {
                      //用来记录批量删除的考勤类型名称，写入操作员日志
                      string batchDelStr = "";

                      List<int> ids = new List<int>();
                      foreach (var ar in LeaveTypeModel)
                      {
                          if (ar.isSelected && ar.leave_type_id >= 50)
                          {
                              batchDelStr += ar.leave_type_name + "，";
                              ids.Add(ar.leave_type_id);
                          }
                      }
                      batchDelStr = batchDelStr.TrimEnd('，') + "；";

                      WaitingDialog.ShowWaiting();
                      Action<InvokeOperation<bool>> onInvokeErrorCallBack = CallBackHandleControl<bool>.OnInvokeErrorCallBack;
                      CallBackHandleControl<bool>.m_sendValue = (o) =>
                      {
                          if (o)
                          {
                              //添加操作员日志
                              VmOperatorLog.InsertOperatorLog(1, "批量删除请假类型", "请假类型名称：" + batchDelStr, () =>
                              {
                                  MsgBoxWindow.MsgBox(
                                         "批量删除请假类型成功！",
                                         MsgBoxWindow.MsgIcon.Succeed,
                                         MsgBoxWindow.MsgBtns.OK);
                                  IsBatchBtnEnable = false;
                                  GetLeaveType(50);
                              });
                          }
                          else
                          {
                              //添加操作员日志
                              VmOperatorLog.InsertOperatorLog(0, "批量删除请假类型", "请假类型名称：" + batchDelStr, () =>
                              {
                                  MsgBoxWindow.MsgBox(
                                         "批量删除请假类型失败！",
                                         MsgBoxWindow.MsgIcon.Error,
                                         MsgBoxWindow.MsgBtns.OK);
                              });
                          }

                          WaitingDialog.HideWaiting();
                      };
                      ServiceDomDbAcess.GetSever().IrisDeleteLeaveType(ids.ToArray(), onInvokeErrorCallBack, null);
                  }
              });
        }

        #endregion

        #region 全选操作

        /// <summary>
        /// cty
        /// 考勤类型的全选操作
        /// </summary>
        /// <param name="isChecked"></param>
        public void SelectAll(bool? isChecked)
        {
            if (!isChecked.HasValue)
            {
                return;
            }
            if (isChecked.Value)
            {
                foreach (var item in LeaveTypeModel)
                {
                    item.isSelected = true;
                }
            }
            else
            {
                foreach (var item in LeaveTypeModel)
                {
                    item.isSelected = false;
                }
            }
            this.IsBatchBtnEnable = CheckIsAnyOneSelected();
            this.MarkObj.Selected = CheckIsAllSelected();
        }

        #endregion

        #region 复选框的选取操作

        /// <summary>
        /// cty
        /// 按需选择datagrid的Items
        /// </summary>
        /// <param name="sender">datagrid对象</param>
        public void SelectItems(LeaveType info)
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
            foreach (var item in LeaveTypeModel)
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
            if (LeaveTypeModel.Count == 0)
            {
                return false;
            }
            foreach (var item in LeaveTypeModel)
            {
                if (!item.isSelected)
                {
                    return false;
                }
            }
            return true;
        }

        #endregion

    }
}


