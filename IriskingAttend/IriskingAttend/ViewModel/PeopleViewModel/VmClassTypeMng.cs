/*************************************************************************
** 文件名:   VmClassTypeMng.cs
×× 主要类:   VmClassTypeMng
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   cty
** 日  期:   2013-6-14
** 修改人:   wz 代码优化
** 日  期:   2013-7-24
** 描  述:   VmClassTypeMng类,班制信息显示
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
    public class VmClassTypeMng : BaseViewModel
    {

        #region 字段声明
       
        /// <summary>
        /// 与服务声明
        /// </summary>
        private DomainServiceIriskingAttend serviceDomDbAccess = new DomainServiceIriskingAttend();

        #endregion

        #region    与界面绑定的属性

        /// <summary>
        /// 添加新班制命令
        /// </summary>
        public DelegateCommand AddClassTypeCommand { get; set; }
        
        /// <summary>
        /// 批量删除班制命令
        /// </summary>
        public DelegateCommand BatchDeleteCommand { get; set; }

        /// <summary>
        /// 全选按钮的绑定类
        /// </summary>
        public MarkObject MarkObj{ get; set; }


        private BaseViewModelCollection<UserClassTypeInfo> classTypeInfos;
        
        /// <summary>
        /// 班制信息列表
        /// </summary>
        public BaseViewModelCollection<UserClassTypeInfo> ClassTypeInfos
        {
            get { return classTypeInfos; }
            set
            {
                classTypeInfos = value;
                this.NotifyPropertyChanged("ClassTypeInfos");
            }
        }

       
        private bool isBatchOperateBtnEnable;
        /// <summary>
        /// 批量操作按钮的enable属性
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

        public VmClassTypeMng( )
        {
            AddClassTypeCommand = new DelegateCommand(AddClassType);
            BatchDeleteCommand = new DelegateCommand(BatchDelete);

            GetClassTypesInfo();
        }
        #endregion

        #region 回调函数

        //子窗口返回的回调函数
        private void ClassTypeOperate_callback(bool? DialogResult)
        {
           
            //重新查询数据库
            serviceDomDbAccess = new DomainServiceIriskingAttend();
            GetClassTypesInfo();
            
        }
        #endregion

        #region 界面事件响应

        private void AddClassType()
        {
            var cw = new ChildWnd_OperateClassType(null, ChildWndOptionMode.Add , ClassTypeOperate_callback);
            cw.Show();
        }

        private void BatchDelete()
        {
            List<int> selectedIDs = new List<int>();
            foreach (var item in ClassTypeInfos)
            {
                if (item.isSelected)
                {
                    selectedIDs.Add(item.class_type_id);
                }
            }
            if (selectedIDs.Count > 0)
            {
                MsgBoxWindow.MsgBox(
                    "请注意，您将进行如下操作\r\n批量删除班制",
                    Dialog.MsgBoxWindow.MsgIcon.Warning, Dialog.MsgBoxWindow.MsgBtns.OKCancel,
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
                            Dialog.MsgBoxWindow.MsgIcon.Information, Dialog.MsgBoxWindow.MsgBtns.OK);
            }
            
        }

       

        #endregion

        #region wcf ria操作
        
        /// <summary>
        /// 获取所有班制信息
        /// </summary>
        private void GetClassTypesInfo()
        {
            try
            {
                WaitingDialog.ShowWaiting();
                EntityQuery<UserClassTypeInfo> list = serviceDomDbAccess.GetClassTypeInfosQuery();
                ///回调异常类
                Action<LoadOperation<UserClassTypeInfo>> actionCallBack = new Action<LoadOperation<UserClassTypeInfo>>(ErrorHandle<UserClassTypeInfo>.OnLoadErrorCallBack);
                ///异步事件
                LoadOperation<UserClassTypeInfo> lo = this.serviceDomDbAccess.Load(list, actionCallBack, null);
                lo.Completed += delegate
                {
                    if (ClassTypeInfos == null)
                    {
                        ClassTypeInfos = new BaseViewModelCollection<UserClassTypeInfo>();
                    }
                    else
                    {
                        ClassTypeInfos.Clear();
                    }

                    //异步获取数据
                    foreach (UserClassTypeInfo ar in lo.Entities)
                    {
                        ClassTypeInfos.Add(ar);
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
        /// 批量删除班制
        /// </summary>
        /// <param name="IDs"></param>
        private void BatchDelete(int[] IDs)
        {
            List<string> IDs_str = new List<string>();
            for (int i = 0; i < IDs.Length; i++)
            {
                IDs_str.Add(PublicMethods.ToString(IDs[i]));
            }
            try
            {
                serviceDomDbAccess = new DomainServiceIriskingAttend();
                EntityQuery<OptionInfo> list = serviceDomDbAccess.DeleteClassTypeQuery(IDs_str.ToArray());

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

                        break;
                    }

                    //重新查询数据库
                    serviceDomDbAccess = new DomainServiceIriskingAttend();
                    GetClassTypesInfo();
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

        /// <summary>
        /// 选中全部或者取消选中
        /// </summary>
        /// <param name="sender"></param>
        public void SelectAll(bool? IsChecked)
        {
            if (!IsChecked.HasValue)
            {
                return;
            }
            if (IsChecked.Value)
            {
                foreach (var item in ClassTypeInfos)
                {
                    item.isSelected = true;
                }
            }
            else
            {
                foreach (var item in ClassTypeInfos)
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
        public void SelectItems(UserClassTypeInfo classTypeInfo)
        {
            classTypeInfo.isSelected = !classTypeInfo.isSelected;

            this.IsBatchOperateBtnEnable = CheckIsAnyOneSelected();
            this.MarkObj.Selected = CheckIsAllSelected();
        }
     
        public void Check(UserClassTypeInfo classTypeInfo)
        {
            var cw = new ChildWnd_OperateClassType(classTypeInfo, ChildWndOptionMode.Check, ClassTypeOperate_callback);
            cw.Show();
         }

        public void Modify(UserClassTypeInfo classTypeInfo)
        {

            var cw = new ChildWnd_OperateClassType(classTypeInfo, ChildWndOptionMode.Modify, ClassTypeOperate_callback);
            cw.Show();
        }

        public void Delete(UserClassTypeInfo classTypeInfo)
        {

            var cw = new ChildWnd_OperateClassType(classTypeInfo, ChildWndOptionMode.Delete, ClassTypeOperate_callback);
            cw.Show();
        }

     
        
        #endregion

        #region 私有功能函数
       
        /// <summary>
        /// 检查是否有item被选中
        /// </summary>
        private bool CheckIsAnyOneSelected()
        {
           
            foreach (var item in ClassTypeInfos)
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
            foreach (var item in ClassTypeInfos)
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
