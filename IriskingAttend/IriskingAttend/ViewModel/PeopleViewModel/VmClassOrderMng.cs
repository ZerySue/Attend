/*************************************************************************
** 文件名:   VmClassOrderMng.cs
×× 主要类:   VmClassOrderMng
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   cty
** 日  期:   2013-6-14
** 修改人:   wz 代码优化
** 日  期:   2013-7-25
** 描  述:   VmClassOrderMng类,班次信息显示
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
using IriskingAttend.BehaviorSelf;

namespace IriskingAttend.ViewModel.PeopleViewModel
{
    public class VmClassOrderMng : BaseViewModel
    {

        #region 字段声明
       
        /// <summary>
        /// 与服务声明
        /// </summary>
        private DomainServiceIriskingAttend serviceDomDbAccess = new DomainServiceIriskingAttend();

        //vm加载完毕事件
        public event EventHandler LoadCompletedEvent;

        #endregion

        #region    与界面绑定的属性

        /// <summary>
        /// 添加新班制命令
        /// </summary>
        public DelegateCommand AddClassOrderCommand { get; set; }
        
        /// <summary>
        /// 批量删除班制命令
        /// </summary>
        public DelegateCommand BatchDeleteCommand { get; set; }

        /// <summary>
        /// 全选按钮的绑定类
        /// </summary>
        public MarkObject MarkObj{ get; set; }

        /// <summary>
        /// 班制信息列表
        /// </summary>
        public BaseViewModelCollection<UserClassOrderInfo> ClassOrderInfos { get; set; }
     

        
        private bool isBatchOperateBtnEnable;
        /// <summary>
        /// 对勾选的item批量操作按钮enable属性
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


        #endregion

        #region 构造函数
        

        public VmClassOrderMng()
        {
            AddClassOrderCommand = new DelegateCommand(AddClassOrder);
            BatchDeleteCommand = new DelegateCommand(BatchDelete);
            GetClassOrdersInfo();
        }
        #endregion

        #region 回调函数
        

        //子窗口返回的回调函数
        private void ClassOrderOperate_callback(bool? DialogResult)
        {
            //if (DialogResult.HasValue && DialogResult.Value)
            {
                //重新查询数据库
                serviceDomDbAccess = new DomainServiceIriskingAttend();
                GetClassOrdersInfo();
            }        
        }

        #endregion

        #region 界面事件响应

        private void AddClassOrder()
        {
            var cw = new ChildWnd_OperateClassOrder(null, ChildWndOptionMode.Add, ClassOrderOperate_callback);
            cw.Show();
        }

        private void BatchDelete()
        {
            List<int> selectedIDs = new List<int>();
            foreach (var item in ClassOrderInfos)
            {
                if (item.isSelected)
                {
                    selectedIDs.Add(item.class_order_id);
                }
            }
            if (selectedIDs.Count > 0)
            {
                MsgBoxWindow.MsgBox(
                        "请注意，您将进行如下操作\r\n批量删除班制",
                        Dialog.MsgBoxWindow.MsgIcon.Warning,
                        Dialog.MsgBoxWindow.MsgBtns.OKCancel,
                        (e) =>
                        {
                            if (e == MsgBoxWindow.MsgResult.OK)
                            {
                                BatchDelete(selectedIDs.ToArray());
                            }
                        });
            }
            else
            {
                MsgBoxWindow.MsgBox(
                                "至少选择一个选项",
                                Dialog.MsgBoxWindow.MsgIcon.Information,
                                Dialog.MsgBoxWindow.MsgBtns.OK);
            }
        }

       

        #endregion

        #region wcf ria操作
    
        /// <summary>
        /// 获取所有班次信息
        /// </summary>
        private void GetClassOrdersInfo()
        {
            try
            {
                WaitingDialog.ShowWaiting();
                EntityQuery<UserClassOrderInfo> list = serviceDomDbAccess.GetClassOrderInfosQuery();
                ///回调异常类
                Action<LoadOperation<UserClassOrderInfo>> actionCallBack = ErrorHandle<UserClassOrderInfo>.OnLoadErrorCallBack;
                ///异步事件
                LoadOperation<UserClassOrderInfo> lo = this.serviceDomDbAccess.Load(list, actionCallBack, null);
                lo.Completed += delegate
                {
                    if (ClassOrderInfos == null)
                    {
                        ClassOrderInfos = new BaseViewModelCollection<UserClassOrderInfo>();
                    }
                    else
                    {
                        ClassOrderInfos.Clear();
                    }

                    //异步获取数据
                    foreach (UserClassOrderInfo ar in lo.Entities)
                    {
                        ClassOrderInfos.Add(ar);
                    }
                   
                    //延迟绑定
                    if (LoadCompletedEvent != null)
                    {
                        LoadCompletedEvent(this, null);
                    }
                    WaitingDialog.HideWaiting();
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
        /// 批量删除班次
        /// </summary>
        /// <param name="IDs">待删除的班次ID</param>
        private void BatchDelete(int[] IDs)
        {
            List<string> IDs_str = new List<string>();
            for (int i = 0; i < IDs.Length; i++)
            {
                IDs_str.Add(PublicMethods.ToString(IDs[i]));
            }

            

            try
            {
                WaitingDialog.ShowWaiting();
                serviceDomDbAccess = new DomainServiceIriskingAttend();
                EntityQuery<OptionInfo> list = serviceDomDbAccess.DeleteClassOrderQuery(IDs_str.ToArray());

                ///回调异常类
                Action<LoadOperation<OptionInfo>> actionCallBack = new Action<LoadOperation<OptionInfo>>(ErrorHandle<OptionInfo>.OnLoadErrorCallBack);
                ///异步事件
                LoadOperation<OptionInfo> lo = this.serviceDomDbAccess.Load(list, actionCallBack, null);
                lo.Completed += delegate
                {
                    foreach (OptionInfo item in lo.Entities)
                    {
                        if (!item.isSuccess)
                        {
                            MsgBoxWindow.MsgBox(
                                item.option_info,
                                Dialog.MsgBoxWindow.MsgIcon.Warning,
                                Dialog.MsgBoxWindow.MsgBtns.OK);
                            
                        }
                        else
                        {
                            MsgBoxWindow.MsgBox(
                                item.option_info,
                                Dialog.MsgBoxWindow.MsgIcon.Succeed,
                                Dialog.MsgBoxWindow.MsgBtns.OK);
                        }
                        serviceDomDbAccess = new DomainServiceIriskingAttend();
                        GetClassOrdersInfo();
                        break;
                    }
                    WaitingDialog.HideWaiting();

                };
            }
            catch (Exception e)
            {
                WaitingDialog.HideWaiting();
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        
        }

        #endregion


        #region 给view层提供的接口函数

        //选中全部或者取消选中
        public void SelectAll(bool? isChecked)
        {
            if (!isChecked.HasValue)
            {
                return;
            }

            if (isChecked.Value)
            {
                foreach (var item in ClassOrderInfos)
                {
                    item.isSelected = true;
                }
            }
            else
            {
                foreach (var item in ClassOrderInfos)
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
        public void SelectItems(UserClassOrderInfo classOrderInfo)
        {

            classOrderInfo.isSelected = !classOrderInfo.isSelected;

            this.IsBatchOperateBtnEnable = CheckIsAnyOneSelected();
            this.MarkObj.Selected = CheckIsAllSelected();
        }

        public void Check(object sender)
        {
            UserClassOrderInfo cInfo = ((HyperlinkButton)sender).DataContext as UserClassOrderInfo;
            var cw = new ChildWnd_OperateClassOrder(cInfo, ChildWndOptionMode.Check, ClassOrderOperate_callback);
            cw.Show();
         }

        public void Modify(object sender)
        {
            UserClassOrderInfo cInfo = ((HyperlinkButton)sender).DataContext as UserClassOrderInfo;
            var cw = new ChildWnd_OperateClassOrder(cInfo, ChildWndOptionMode.Modify, ClassOrderOperate_callback);
            cw.Show();
        }

        public void Delete(object sender)
        {
            UserClassOrderInfo cInfo = ((HyperlinkButton)sender).DataContext as UserClassOrderInfo;
            var cw = new ChildWnd_OperateClassOrder(cInfo, ChildWndOptionMode.Delete, ClassOrderOperate_callback);
            cw.Show();
        }

     
        
        #endregion

        #region  私有功能函数

        /// <summary>
        /// 检查Item是否全部被选中
        /// </summary>
        private bool CheckIsAllSelected()
        {
            
            foreach (var item in ClassOrderInfos)
            {
                if (!item.isSelected)
                {
                    return false;
                }
            }
            return true;

        }

        /// <summary>
        /// 检查是否有item被选中
        /// </summary>
        private bool CheckIsAnyOneSelected()
        {
           
            foreach (var item in ClassOrderInfos)
            {
                if (item.isSelected)
                {
                    return true;
                }
            }
            return false;
        }

       

        #endregion

    }



}
