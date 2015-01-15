/*************************************************************************
** 文件名:   VmClassTypeAndClassOrderMng.cs
×× 主要类:   VmClassTypeAndClassOrderMng
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   cty
** 日  期:   2013-6-14
** 修改人:   wz  代码优化 szr 增加 记工时班次功能
** 日  期:   2013-7-25  2014-11-19
** 描  述:   班制班次管理界面
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
using System.Text;
using IriskingAttend.ViewModel.SystemViewModel;
using IriskingAttend.ApplicationType;

namespace IriskingAttend.ViewModel.PeopleViewModel
{
    public class VmClassTypeAndClassOrderMng : BaseViewModel
    {

        #region 字段声明
       
        /// <summary>
        /// 与服务声明
        /// </summary>
        private DomainServiceIriskingAttend _serviceDomDbAccess = new DomainServiceIriskingAttend();

        //vm加载完毕事件
        public event EventHandler LoadCompletedEvent;
         
        //当前班制Id
        private int _currentClassTypeId;

        #endregion

        #region    与界面绑定的属性

     
        /// <summary>
        /// 添加新班次命令
        /// </summary>
        public DelegateCommand AddClassOrderCommand 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// 添加新班制命令
        /// </summary>
        public DelegateCommand AddClassTypeCommand
        { 
            get; 
            set; 
        }

        /// <summary>
        /// 批量删除班次命令
        /// </summary>
        public DelegateCommand BatchDeleteClassOrderCommand 
        { 
            get;
            set; 
        }

        /// <summary>
        /// 批量删除班制命令
        /// </summary>
        public DelegateCommand BatchDeleteClassTypeCommand 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// 班次全选按钮
        /// </summary>
        public MarkObject ClassOrderAllSelect
        { 
            get; 
            set; 
        }
       
        /// <summary>
        /// 班制全选按钮
        /// </summary>
        public MarkObject ClassTypeAllSelect 
        { 
            get; 
            set;
        }

        private BaseViewModelCollection<UserClassOrderInfo> _classOrderInfos;
        /// <summary>
        /// 班次信息列表
        /// </summary>
        public BaseViewModelCollection<UserClassOrderInfo> ClassOrderInfos
        {
            get
            {
                return _classOrderInfos;
            }
            set
            {
                _classOrderInfos = value;
                OnPropertyChanged<BaseViewModelCollection<UserClassOrderInfo>>(() => this.ClassOrderInfos);
            }
        }



        private BaseViewModelCollection<UserClassTypeInfo> _classTypeInfos;
        /// <summary>
        /// 班制信息列表
        /// </summary>
        public BaseViewModelCollection<UserClassTypeInfo> ClassTypeInfos
        {
            get
            {
                return _classTypeInfos;
            }
            set
            {
                _classTypeInfos = value;
                OnPropertyChanged<BaseViewModelCollection<UserClassTypeInfo>>(() => this.ClassTypeInfos);
            }
        }

        private UserClassTypeInfo _selectedClassType;
        /// <summary>
        /// 当前班制
        /// </summary>
        public UserClassTypeInfo SelectedClassType
        {
            get
            {
                return _selectedClassType;
            }
            set
            {
                _selectedClassType = value;
                OnPropertyChanged(() => this.SelectedClassType);
                OnPropertyChanged(() => this.IsAddClassOrderBtnEnable);
                OnPropertyChanged(() => this.IsAddClassOrderJiGongShiBtnEnable);
                                             
            }
        }

        /// <summary>
        /// 添加班次按钮是否可操作
        /// </summary>
        public bool IsAddClassOrderBtnEnable
        {
            get
            {
                if (SelectedClassType == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            
        }


        
        private bool _isBatchOperateClassOrderBtnEnable;
        /// <summary>
        /// 对勾选的班次item批量操作按钮enable属性
        /// </summary>
        public bool IsBatchOperateClassOrderBtnEnable
        {
            get 
            {
                return _isBatchOperateClassOrderBtnEnable;
            }
            set
            {
                _isBatchOperateClassOrderBtnEnable = value;
                OnPropertyChanged<bool>(() => this.IsBatchOperateClassOrderBtnEnable);
            }
        }

        private bool _isBatchOperateClassTypeBtnEnable;

        /// <summary>
        /// 对勾选的班制item批量操作按钮enable属性
        /// </summary>
        public bool IsBatchOperateClassTypeBtnEnable
        {
            get { return _isBatchOperateClassTypeBtnEnable; }
            set
            {
                _isBatchOperateClassTypeBtnEnable = value;
                OnPropertyChanged<bool>(() => this.IsBatchOperateClassTypeBtnEnable);
            }
        }

        //vm加载完毕事件
        public event EventHandler ClassOrderLoadCompletedEvent;

        #endregion

        #region 构造函数
        /// <summary>
        /// init
        /// </summary>
        public VmClassTypeAndClassOrderMng()
        {
           
            AddClassOrderCommand = new DelegateCommand(AddClassOrder);
            AddClassTypeCommand = new DelegateCommand(AddClassType);
            BatchDeleteClassOrderCommand = new DelegateCommand(BatchDeleteClassOrder);
            BatchDeleteClassTypeCommand = new DelegateCommand(BatchDeleteClassType);

            AddClassOrderSignCommand = new DelegateCommand(AddClassOrderSign);
            BatchDeleteClassOrderSignCommand = new DelegateCommand(BatchDeleteClassOrderSign);

            AddClassOrderJiGongShiCommand = new DelegateCommand(AddClassOrderJiGongShi);//szr
            BatchDeleteClassOrderJiGongShiCommand= new DelegateCommand(BatchDeleteClassOrderJiGongShi);//szr

          
            ClassOrderSignInfos = new BaseViewModelCollection<UserClassOrderSignInfo>();

            ClassOrderJiGongShiInfos = new BaseViewModelCollection<UserClassOrderJiGongShiInfo>();

            //事件初始化
            ClassOrderLoadCompletedEvent += (a, e) => { };

            AppTypePublic.GetIsSupportClassOrderSignRia(() =>
                {
                    GetClassTypesInfo();
                });
        }

        #endregion

        #region 界面事件响应

        /// <summary>
        /// 添加班次
        /// </summary>
        private void AddClassOrder()
        {
           
            var cw = new ChildWnd_OperateClassOrder(null, ChildWndOptionMode.Add,ClassOrderOperate_callback,
                _currentClassTypeId);
            cw.Show();
        }

        /// <summary>
        /// 添加班制
        /// </summary>
        private void AddClassType()
        {
            var cw = new ChildWnd_OperateClassType(null, ChildWndOptionMode.Add, ClassTypeOperate_callback);
            cw.Show();
        }

        /// <summary>
        /// 批量删除班次
        /// </summary>
        private void BatchDeleteClassOrder()
        {
            List<UserClassOrderInfo> selectedClassOrders = new List<UserClassOrderInfo>();
            foreach (var item in ClassOrderInfos)
            {
                if (item.isSelected)
                {
                    selectedClassOrders.Add(item);
                }
            }
            if (selectedClassOrders.Count > 0)
            {
                MsgBoxWindow.MsgBox(
                        "请注意，您将进行如下操作\r\n批量删除班制！",
                        Dialog.MsgBoxWindow.MsgIcon.Warning,
                        Dialog.MsgBoxWindow.MsgBtns.OKCancel,
                        (e) =>
                        {
                            if (e == MsgBoxWindow.MsgResult.OK)
                            {
                                BatchDeleteClassOrderRia(selectedClassOrders.ToArray());
                            }
                        });
            }
            else
            {
                MsgBoxWindow.MsgBox(
                                "至少选择一个选项！",
                                Dialog.MsgBoxWindow.MsgIcon.Information,
                                Dialog.MsgBoxWindow.MsgBtns.OK);
            }

        }

        /// <summary>
        /// 批量删除班制
        /// </summary>
        private void BatchDeleteClassType()
        {
            List<UserClassTypeInfo> selectedClassTypes = new List<UserClassTypeInfo>();
            foreach (var item in ClassTypeInfos)
            {
                if (item.isSelected)
                {
                    selectedClassTypes.Add(item);
                }
            }
            if (selectedClassTypes.Count > 0)
            {
                MsgBoxWindow.MsgBox(
                        "请注意，您将进行如下操作\r\n批量删除班制！",
                        Dialog.MsgBoxWindow.MsgIcon.Warning,
                        Dialog.MsgBoxWindow.MsgBtns.OKCancel,
                        (e) =>
                        {
                            if (e == MsgBoxWindow.MsgResult.OK)
                            {
                                BatchDeleteClassTypeRia(selectedClassTypes.ToArray());
                            }
                        });
            }
            else
            {
                MsgBoxWindow.MsgBox(
                                "至少选择一个选项！",
                                Dialog.MsgBoxWindow.MsgIcon.Information,
                                Dialog.MsgBoxWindow.MsgBtns.OK);
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
                EntityQuery<UserClassTypeInfo> list = _serviceDomDbAccess.GetClassTypeInfosQuery();
                ///回调异常类
                Action<LoadOperation<UserClassTypeInfo>> actionCallBack = new Action<LoadOperation<UserClassTypeInfo>>(ErrorHandle<UserClassTypeInfo>.OnLoadErrorCallBack);
                ///异步事件
                LoadOperation<UserClassTypeInfo> lo = this._serviceDomDbAccess.Load(list, actionCallBack, null);
                lo.Completed += delegate
                {
                    
                    ClassTypeInfos = new BaseViewModelCollection<UserClassTypeInfo>();
                   

                    //异步获取数据
                    foreach (UserClassTypeInfo ar in lo.Entities)
                    {
                        ClassTypeInfos.Add(ar);
                    }

                    this.ClassTypeAllSelect.Selected = CheckIsAllClassTypeSelected();
                    this.IsBatchOperateClassTypeBtnEnable = CheckIsAnyClassTypeSelected();

                    if (LoadCompletedEvent != null)
                    {
                        LoadCompletedEvent(this, null);
                        LoadCompletedEvent = null;
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
        /// 获取所有班次信息
        /// </summary>
        private void GetAllClassOrdersInfo()
        {
            try
            {
                WaitingDialog.ShowWaiting();
                EntityQuery<UserClassOrderInfo> list = _serviceDomDbAccess.GetClassOrderInfosQuery();
                ///回调异常类
                Action<LoadOperation<UserClassOrderInfo>> actionCallBack = ErrorHandle<UserClassOrderInfo>.OnLoadErrorCallBack;
                ///异步事件
                LoadOperation<UserClassOrderInfo> lo = this._serviceDomDbAccess.Load(list, actionCallBack, null);
                lo.Completed += delegate
                {
                    
                    ClassOrderInfos = new BaseViewModelCollection<UserClassOrderInfo>();

                    //异步获取数据
                    foreach (UserClassOrderInfo ar in lo.Entities)
                    {
                        ClassOrderInfos.Add(ar);
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
        /// 获取对应班制的班次信息
        /// </summary>
        /// <param name="classTypeId">对应班制Id</param>
        private void GetClassOrderInfo(int classTypeId)
        {
            if (classTypeId < 0)
            {
                return;
            } 
            _currentClassTypeId = classTypeId;
            try
            {
                WaitingDialog.ShowWaiting();
                EntityQuery<UserClassOrderInfo> list = _serviceDomDbAccess.GetClassOrderInfosByClassTypeQuery(classTypeId);
                ///回调异常类
                Action<LoadOperation<UserClassOrderInfo>> actionCallBack = ErrorHandle<UserClassOrderInfo>.OnLoadErrorCallBack;
                ///异步事件
                LoadOperation<UserClassOrderInfo> lo = this._serviceDomDbAccess.Load(list, actionCallBack, null);
                lo.Completed += delegate
                {

                    ClassOrderInfos = new BaseViewModelCollection<UserClassOrderInfo>();

                    //异步获取数据
                    foreach (UserClassOrderInfo ar in lo.Entities)
                    {
                        ClassOrderInfos.Add(ar);
                    }

                    this.IsBatchOperateClassOrderBtnEnable = CheckIsAnyClassOrderSelected();
                    this.ClassOrderAllSelect.Selected =  this.CheckIsAllClassOrderSelected();
                   
                    ClassOrderLoadCompletedEvent(this, new EventArgs());

                    WaitingDialog.HideWaiting();
                };
            }
            catch (Exception e)
            {
                WaitingDialog.HideWaiting();
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
                _currentClassTypeId = -1;
            }
        }

        /// <summary>
        /// 批量删除班次
        /// </summary>
        /// <param name="IDs">待删除的班次ID</param>
        private void BatchDeleteClassOrderRia(UserClassOrderInfo[] classOrders)
        {
            //获取操作描述
            StringBuilder description = new StringBuilder(string.Format("所属班制：{0}；\r\n班次名称：",classOrders[0].class_type_name)); 

            List<string> Ids_str = new List<string>();
            for (int i = 0; i < classOrders.Length; i++)
            {
                Ids_str.Add(PublicMethods.ToString(classOrders[i].class_order_id));
                description.Append(classOrders[i].class_order_name + "，");
            }
            description.Remove(description.Length - 1, 1);
            description.Append("；\r\n");

            try
            {
                WaitingDialog.ShowWaiting();
                _serviceDomDbAccess = new DomainServiceIriskingAttend();
                EntityQuery<OptionInfo> list = _serviceDomDbAccess.DeleteClassOrderQuery(Ids_str.ToArray());

                ///回调异常类
                Action<LoadOperation<OptionInfo>> actionCallBack = new Action<LoadOperation<OptionInfo>>(ErrorHandle<OptionInfo>.OnLoadErrorCallBack);
                ///异步事件
                LoadOperation<OptionInfo> lo = this._serviceDomDbAccess.Load(list, actionCallBack, null);
                lo.Completed += delegate
                {
                    VmOperatorLog.CompleteCallBack completeCallBack = ()=>
                        {
                             _serviceDomDbAccess = new DomainServiceIriskingAttend();
                            GetClassOrderInfo(_currentClassTypeId);
                        };

                    foreach (OptionInfo item in lo.Entities)
                    {
                        if (!item.isSuccess)
                        {
                            MsgBoxWindow.MsgBox("提示",
                                item.option_info + "！",
                                Dialog.MsgBoxWindow.MsgIcon.Error,
                                Dialog.MsgBoxWindow.MsgBtns.OK);
                            //写入操作员日志 
                            VmOperatorLog.InsertOperatorLog(0, "删除班次", description.ToString() + item.option_info, completeCallBack);
                        }
                        else
                        {
                            if (!item.isNotifySuccess)
                            {
                                MsgBoxWindow.MsgBox("提示",
                                 item.option_info + "！",
                                 Dialog.MsgBoxWindow.MsgIcon.Warning,
                                 Dialog.MsgBoxWindow.MsgBtns.OK);
                                //写入操作员日志 
                                VmOperatorLog.InsertOperatorLog(1, "删除班次", description.ToString() + item.option_info, completeCallBack);
                            }
                            else
                            {
                                MsgBoxWindow.MsgBox("提示",
                                 item.option_info + "！",
                                 Dialog.MsgBoxWindow.MsgIcon.Succeed,
                                 Dialog.MsgBoxWindow.MsgBtns.OK);
                                //写入操作员日志 
                                VmOperatorLog.InsertOperatorLog(1, "删除班次", description.ToString() , completeCallBack);
                            }
                     
                        }
                       
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

        /// <summary>
        /// 批量删除班制
        /// </summary>
        /// <param name="IDs">待删除的班制ID</param>
        private void BatchDeleteClassTypeRia(UserClassTypeInfo[] classTypes)
        {
            //获取操作描述
            StringBuilder description = new StringBuilder("班制名称：");
            List<string> Ids_str = new List<string>();
            for (int i = 0; i < classTypes.Length; i++)
            {
                Ids_str.Add(PublicMethods.ToString(classTypes[i].class_type_id));
                description.Append(classTypes[i].class_type_name + "，");
            }
            description.Remove(description.Length - 1, 1);
            description.Append("；\r\n");
            try
            {
                _serviceDomDbAccess = new DomainServiceIriskingAttend();
                EntityQuery<OptionInfo> list = _serviceDomDbAccess.DeleteClassTypeQuery(Ids_str.ToArray());

                ///回调异常类
                Action<LoadOperation<OptionInfo>> actionCallBack = new Action<LoadOperation<OptionInfo>>(ErrorHandle<OptionInfo>.OnLoadErrorCallBack);
                ///异步事件
                LoadOperation<OptionInfo> lo = this._serviceDomDbAccess.Load(list, actionCallBack, null);
                lo.Completed += delegate
                {
                    VmOperatorLog.CompleteCallBack completeCallBack = () =>
                    {
                        //重新查询数据库
                        _serviceDomDbAccess = new DomainServiceIriskingAttend();
                        GetClassTypesInfo();
                    };
                    foreach (OptionInfo item in lo.Entities)
                    {
                        if (!item.isSuccess)
                        {
                            MsgBoxWindow.MsgBox(
                              item.option_info + "！",
                              Dialog.MsgBoxWindow.MsgIcon.Error,
                              Dialog.MsgBoxWindow.MsgBtns.OK);
                            //写入操作员日志 
                            VmOperatorLog.InsertOperatorLog(0, "删除班制", description.ToString() + item.option_info, completeCallBack);
                        }
                        else
                        {
                            if (!item.isNotifySuccess)
                            {
                                MsgBoxWindow.MsgBox(
                                  item.option_info + "！",
                                  Dialog.MsgBoxWindow.MsgIcon.Warning,
                                  Dialog.MsgBoxWindow.MsgBtns.OK);
                                //写入操作员日志 
                                VmOperatorLog.InsertOperatorLog(1, "删除班制", description.ToString() + item.option_info, completeCallBack);
                            }
                            else
                            {
                                MsgBoxWindow.MsgBox(
                                  item.option_info + "！",
                                  Dialog.MsgBoxWindow.MsgIcon.Succeed,
                                  Dialog.MsgBoxWindow.MsgBtns.OK);
                                //写入操作员日志 
                                VmOperatorLog.InsertOperatorLog(1, "删除班制", description.ToString(), completeCallBack);
                            }
                            
                        }

                        break;
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

        #endregion

        #region 给view层提供的接口函数

        /// <summary>
        /// 选中全部或者取消选中ClassOrder
        /// </summary>
        /// <param name="isChecked">是否全选标志</param>
        public void SelectAllClassOrder(bool? isChecked)
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
            this.IsBatchOperateClassOrderBtnEnable = CheckIsAnyClassOrderSelected();
            this.ClassOrderAllSelect.Selected = CheckIsAllClassOrderSelected();
        }

        /// <summary>
        /// 点击第一列选择ClassOrder
        /// </summary>
        /// <param name="sender">UserClassOrderInfo对象</param>
        public void SelectOneClassOrder(UserClassOrderInfo classOrderInfo)
        {
            classOrderInfo.isSelected = !classOrderInfo.isSelected;

            this.IsBatchOperateClassOrderBtnEnable = CheckIsAnyClassOrderSelected();
            this.ClassOrderAllSelect.Selected = CheckIsAllClassOrderSelected();
        }

        /// <summary>
        /// 选中全部或者取消选中ClassType
        /// </summary>
        /// <param name="isChecked">是否全选标志</param>
        public void SelectAllClassType(bool? isChecked)
        {
            if (!isChecked.HasValue)
            {
                return;
            }

            if (isChecked.Value)
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
            this.IsBatchOperateClassTypeBtnEnable = CheckIsAnyClassTypeSelected();
            this.ClassTypeAllSelect.Selected = CheckIsAllClassTypeSelected();
        }

        /// <summary>
        /// 点击第一列选择ClassType
        /// </summary>
        /// <param name="sender">UserClassTypeInfo对象</param>
        public void SelectOneClassType(UserClassTypeInfo classTypeInfo)
        {

            classTypeInfo.isSelected = !classTypeInfo.isSelected;

            this.IsBatchOperateClassTypeBtnEnable = CheckIsAnyClassTypeSelected();
            this.ClassTypeAllSelect.Selected = CheckIsAllClassTypeSelected();
        }

        /// <summary>
        /// 当前班制改变事件
        /// </summary>
        /// <param name="classTypeInfo">当前班制信息</param>
        public void CurrentClassTypeChanged(UserClassTypeInfo classTypeInfo)
        {
            _serviceDomDbAccess = new DomainServiceIriskingAttend();
            ClassOrderLoadCompletedEvent = null;
            ClassOrderLoadCompletedEvent += (a, e) => { };

            GetClassOrderInfo(classTypeInfo.class_type_id);

            ClassOrderLoadCompletedEvent += (o, e) =>
            {
                if (AppTypePublic.GetIsSupportClassOrderSign()==1)
                {
                    GetClassOrderSignInfo(classTypeInfo.class_type_id);
                }
                if (AppTypePublic.GetIsSupportClassOrderSign() == 2)//szr
                {
                    GetClassOrderJiGongShiInfo(classTypeInfo.class_type_id);
                }
            };
        }

        /// <summary>
        /// 修改班次
        /// </summary>
        /// <param name="info">班次信息</param>
        public void ModifyClassOrder(UserClassOrderInfo info)
        {            
            var cw = new ChildWnd_OperateClassOrder(info, ChildWndOptionMode.Modify, ClassOrderOperate_callback);
            cw.Show();
        }

        /// <summary>
        /// 删除班次
        /// </summary>
        /// <param name="info">班次信息</param>
        public void DeleteClassOrder(UserClassOrderInfo info)
        {
             MsgBoxWindow.MsgBox(string.Format("请注意，您将进行如下操作:\r\n删除班次【{0}】！",info.class_order_name),
                 MsgBoxWindow.MsgIcon.Warning,MsgBoxWindow.MsgBtns.OKCancel,(result)=>
                     {
                         if (result == MsgBoxWindow.MsgResult.OK)
                         {
                             UserClassOrderInfo[] classOrders = new UserClassOrderInfo[1];
                             classOrders[0] = info;
                             BatchDeleteClassOrderRia(classOrders);
                         }
                     });
        }

        /// <summary>
        /// 修改班制
        /// </summary>
        /// <param name="info">班制信息</param>
        public void ModifyClassType(UserClassTypeInfo info)
        {
            var cw = new ChildWnd_OperateClassType(info, ChildWndOptionMode.Modify, ClassTypeOperate_callback);
            cw.Show();
        }

        /// <summary>
        /// 删除班制
        /// </summary>
        /// <param name="info">班制信息</param>
        public void DeleteClassType(UserClassTypeInfo info)
        {
            MsgBoxWindow.MsgBox(string.Format("请注意，您将进行如下操作:\r\n删除班制【{0}】！", info.class_type_name),
                  MsgBoxWindow.MsgIcon.Warning, MsgBoxWindow.MsgBtns.OKCancel, (result) =>
                  {
                      if (result == MsgBoxWindow.MsgResult.OK)
                      {
                          UserClassTypeInfo[] classTypes = new UserClassTypeInfo[1];
                          classTypes[0] = info;
                          BatchDeleteClassTypeRia(classTypes);
                      }
                  });
        }
     
        
        #endregion

        #region  私有功能函数

        /// <summary>
        /// 检查ClassOrder的Item是否全部被选中
        /// </summary>
        private bool CheckIsAllClassOrderSelected()
        {
            if (ClassOrderInfos.Count == 0)
            {
                return false;
            }


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
        /// 检查ClassOrder是否有item被选中
        /// </summary>
        private bool CheckIsAnyClassTypeSelected()
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
        /// 检查ClassOrder的Item是否全部被选中
        /// </summary>
        private bool CheckIsAllClassTypeSelected()
        {
            if (ClassTypeInfos.Count == 0)
            {
                return false;
            }
            foreach (var item in ClassTypeInfos)
            {
                if (!item.isSelected)
                {
                    return false;
                }
            }
            return true;

        }

        /// <summary>
        /// 检查ClassOrder是否有item被选中
        /// </summary>
        private bool CheckIsAnyClassOrderSelected()
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

        #region 回调函数

        /// <summary>
        /// 对单个班次操作的回调函数
        /// </summary>
        /// <param name="dialogResult">子窗口是否确认关闭的标志</param>
        private void ClassOrderOperate_callback(bool? dialogResult)
        {
            if (dialogResult.HasValue && dialogResult.Value)
            {
                //重新查询数据库
                _serviceDomDbAccess = new DomainServiceIriskingAttend();
                GetClassOrderInfo(_currentClassTypeId);
            }
        }

        /// <summary>
        /// 对单个班制操作的回调函数
        /// </summary>
        /// <param name="dialogResult">子窗口是否确认关闭的标志</param>
        private void ClassTypeOperate_callback(bool? dialogResult)
        {
            if (dialogResult.HasValue && dialogResult.Value)
            {
                //重新查询数据库
                _serviceDomDbAccess = new DomainServiceIriskingAttend();
                GetClassTypesInfo();
            }
        }

        #endregion

        #region 签到班班次相关操作  add by gqy 2014-09-25

        private BaseViewModelCollection<UserClassOrderSignInfo> _classOrderSignInfos;
        /// <summary>
        /// 班次信息列表
        /// </summary>
        public BaseViewModelCollection<UserClassOrderSignInfo> ClassOrderSignInfos
        {
            get
            {
                return _classOrderSignInfos;
            }
            set
            {
                _classOrderSignInfos = value;
                OnPropertyChanged<BaseViewModelCollection<UserClassOrderSignInfo>>(() => this.ClassOrderSignInfos);
            }
        }

        private bool _isBatchOperateClassOrderSignBtnEnable;
        /// <summary>
        /// 对勾选的签到班班次item批量操作按钮enable属性
        /// </summary>
        public bool IsBatchOperateClassOrderSignBtnEnable
        {
            get
            {
                return _isBatchOperateClassOrderSignBtnEnable;
            }
            set
            {
                _isBatchOperateClassOrderSignBtnEnable = value;
                OnPropertyChanged<bool>(() => this.IsBatchOperateClassOrderSignBtnEnable);
            }
        }

        /// <summary>
        /// 签到班班次全选按钮
        /// </summary>
        public MarkObject ClassOrderSignAllSelect
        {
            get;
            set;
        }

        /// <summary>
        /// 添加新签到班班次命令
        /// </summary>
        public DelegateCommand AddClassOrderSignCommand
        {
            get;
            set;
        }       

        /// <summary>
        /// 批量删除签到班班次命令
        /// </summary>
        public DelegateCommand BatchDeleteClassOrderSignCommand
        {
            get;
            set;
        }

        /// <summary>
        /// 检查ClassOrderSign是否有item被选中
        /// </summary>
        private bool CheckIsAnyClassOrderSignSelected()
        {

            foreach (var item in ClassOrderSignInfos)
            {
                if (item.isSelected)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 检查ClassOrderSign的Item是否全部被选中
        /// </summary>
        private bool CheckIsAllClassOrderSignSelected()
        {
            if (ClassOrderSignInfos.Count == 0)
            {
                return false;
            }


            foreach (var item in ClassOrderSignInfos)
            {
                if (!item.isSelected)
                {
                    return false;
                }
            }
            return true;

        }

        /// <summary>
        /// 获取对应班制的班次信息
        /// </summary>
        /// <param name="classTypeId">对应班制Id</param>
        private void GetClassOrderSignInfo(int classTypeId)
        {
            if (classTypeId < 0)
            {
                return;
            }
            _currentClassTypeId = classTypeId;
            try
            {
                WaitingDialog.ShowWaiting();

                ServiceDomDbAcess.ReOpenSever();

                EntityQuery<UserClassOrderSignInfo> list = ServiceDomDbAcess.GetSever().GetClassOrderSignInfosByClassTypeQuery(classTypeId);
                ///回调异常类
                Action<LoadOperation<UserClassOrderSignInfo>> actionCallBack = ErrorHandle<UserClassOrderSignInfo>.OnLoadErrorCallBack;
                ///异步事件
                LoadOperation<UserClassOrderSignInfo> lo = ServiceDomDbAcess.GetSever().Load(list, actionCallBack, null);
                lo.Completed += delegate
                {

                    ClassOrderSignInfos = new BaseViewModelCollection<UserClassOrderSignInfo>();

                    //异步获取数据
                    foreach (UserClassOrderSignInfo ar in lo.Entities)
                    {
                        ClassOrderSignInfos.Add(ar);
                    }
                    //是否有问题 待验证  szr 签到班方法 控制 班次中按钮？？？？
                    this.IsBatchOperateClassOrderBtnEnable = CheckIsAnyClassOrderSignSelected();
                    this.ClassOrderSignAllSelect.Selected = this.CheckIsAllClassOrderSignSelected();

                    WaitingDialog.HideWaiting();
                };
            }
            catch (Exception e)
            {
                WaitingDialog.HideWaiting();
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
                _currentClassTypeId = -1;
            }
        }

        /// <summary>
        /// 选中全部或者取消选中ClassOrderSign
        /// </summary>
        /// <param name="isChecked">是否全选标志</param>
        public void SelectAllClassOrderSign(bool? isChecked)
        {
            if (!isChecked.HasValue)
            {
                return;
            }

            if (isChecked.Value)
            {
                foreach (var item in ClassOrderSignInfos)
                {
                    item.isSelected = true;
                }
            }
            else
            {
                foreach (var item in ClassOrderSignInfos)
                {
                    item.isSelected = false;
                }
            }
            this.IsBatchOperateClassOrderSignBtnEnable = CheckIsAnyClassOrderSignSelected();
            this.ClassOrderSignAllSelect.Selected = CheckIsAllClassOrderSignSelected();
        }

        /// <summary>
        /// 点击第一列选择ClassOrderSign
        /// </summary>
        /// <param name="sender">UserClassOrderSignInfo对象</param>
        public void SelectOneClassOrderSign(UserClassOrderSignInfo classOrderInfoSign)
        {
            classOrderInfoSign.isSelected = !classOrderInfoSign.isSelected;

            this.IsBatchOperateClassOrderSignBtnEnable = CheckIsAnyClassOrderSignSelected();
            this.ClassOrderSignAllSelect.Selected = CheckIsAllClassOrderSignSelected();
        }

        /// <summary>
        /// 修改签到班班次
        /// </summary>
        /// <param name="info">签到班班次信息</param>
        public void ModifyClassOrderSign(UserClassOrderSignInfo info)
        {
            ChildWnd_OperateClassOrderSign modifyClassOrderSign = new ChildWnd_OperateClassOrderSign(info, ChildWndOptionMode.Modify, ClassOrderOperate_callback);
            modifyClassOrderSign.VmOperateClassOrderSign.ClassTypeInfos = this.ClassTypeInfos;
            modifyClassOrderSign.VmOperateClassOrderSign.SelectedClassType = this.SelectedClassType;  
            modifyClassOrderSign.Show();
        }

        /// <summary>
        /// 添加签到班班次
        /// </summary>
        private void AddClassOrderSign()
        {
            ChildWnd_OperateClassOrderSign addClassOrderSign = new ChildWnd_OperateClassOrderSign(null, ChildWndOptionMode.Add, ClassOrderOperate_callback);
            addClassOrderSign.VmOperateClassOrderSign.ClassTypeInfos = this.ClassTypeInfos;
            addClassOrderSign.VmOperateClassOrderSign.SelectedClassType = this.SelectedClassType;   
            addClassOrderSign.Show();
        }

        /// <summary>
        /// 删除签到班班次
        /// </summary>
        /// <param name="info">签到班班次信息</param>
        public void DeleteClassOrderSign(UserClassOrderSignInfo info)
        {
            MsgBoxWindow.MsgBox(string.Format("请注意，您将进行如下操作:\r\n删除签到班班次【{0}】！", info.class_order_name),
                MsgBoxWindow.MsgIcon.Warning, MsgBoxWindow.MsgBtns.OKCancel, (result) =>
                {
                    if (result == MsgBoxWindow.MsgResult.OK)
                    {
                        UserClassOrderSignInfo[] classOrders = new UserClassOrderSignInfo[1];
                        classOrders[0] = info;
                        BatchDeleteClassOrderSignRia(classOrders);
                    }
                });
        }

        /// <summary>
        /// 批量删除班次
        /// </summary>
        private void BatchDeleteClassOrderSign()
        {
            List<UserClassOrderSignInfo> selectedClassOrderSigns = new List<UserClassOrderSignInfo>();
            foreach (var item in ClassOrderSignInfos)
            {
                if (item.isSelected)
                {
                    selectedClassOrderSigns.Add(item);
                }
            }
            if (selectedClassOrderSigns.Count > 0)
            {
                MsgBoxWindow.MsgBox(
                        "请注意，您将进行如下操作\r\n批量删除签到班班次！",
                        Dialog.MsgBoxWindow.MsgIcon.Warning,
                        Dialog.MsgBoxWindow.MsgBtns.OKCancel,
                        (e) =>
                        {
                            if (e == MsgBoxWindow.MsgResult.OK)
                            {
                                BatchDeleteClassOrderSignRia(selectedClassOrderSigns.ToArray());
                            }
                        });
            }
            else
            {
                MsgBoxWindow.MsgBox(
                                "至少选择一个选项！",
                                Dialog.MsgBoxWindow.MsgIcon.Information,
                                Dialog.MsgBoxWindow.MsgBtns.OK);
            }

        }

        /// <summary>
        /// 批量删除签到班班次
        /// </summary>
        /// <param name="IDs">待删除的签到班班次ID</param>
        private void BatchDeleteClassOrderSignRia(UserClassOrderSignInfo[] classOrders)
        {
            //获取操作描述
            StringBuilder description = new StringBuilder(string.Format("所属班制：{0}；\r\n班次名称：", classOrders[0].class_type_name));

            List<string> Ids_str = new List<string>();
            for (int i = 0; i < classOrders.Length; i++)
            {
                Ids_str.Add(PublicMethods.ToString(classOrders[i].class_order_id));
                description.Append(classOrders[i].class_order_name + "，");
            }
            description.Remove(description.Length - 1, 1);
            description.Append("；\r\n");

            try
            {
                WaitingDialog.ShowWaiting();
                _serviceDomDbAccess = new DomainServiceIriskingAttend();
                EntityQuery<OptionInfo> list = _serviceDomDbAccess.DeleteClassOrderSignQuery(Ids_str.ToArray());

                ///回调异常类
                Action<LoadOperation<OptionInfo>> actionCallBack = new Action<LoadOperation<OptionInfo>>(ErrorHandle<OptionInfo>.OnLoadErrorCallBack);
                ///异步事件
                LoadOperation<OptionInfo> lo = this._serviceDomDbAccess.Load(list, actionCallBack, null);
                lo.Completed += delegate
                {
                    VmOperatorLog.CompleteCallBack completeCallBack = () =>
                    {
                        _serviceDomDbAccess = new DomainServiceIriskingAttend();
                        GetClassOrderSignInfo(_currentClassTypeId);
                    };

                    foreach (OptionInfo item in lo.Entities)
                    {
                        if (!item.isSuccess)
                        {
                            MsgBoxWindow.MsgBox("提示",
                                item.option_info + "！",
                                Dialog.MsgBoxWindow.MsgIcon.Error,
                                Dialog.MsgBoxWindow.MsgBtns.OK);
                            //写入操作员日志 
                            VmOperatorLog.InsertOperatorLog(0, "删除签到班班次", description.ToString() + item.option_info, completeCallBack);
                        }
                        else
                        {
                            if (!item.isNotifySuccess)
                            {
                                MsgBoxWindow.MsgBox("提示",
                                 item.option_info + "！",
                                 Dialog.MsgBoxWindow.MsgIcon.Warning,
                                 Dialog.MsgBoxWindow.MsgBtns.OK);
                                //写入操作员日志 
                                VmOperatorLog.InsertOperatorLog(1, "删除签到班班次", description.ToString() + item.option_info, completeCallBack);
                            }
                            else
                            {
                                MsgBoxWindow.MsgBox("提示",
                                 item.option_info + "！",
                                 Dialog.MsgBoxWindow.MsgIcon.Succeed,
                                 Dialog.MsgBoxWindow.MsgBtns.OK);
                                //写入操作员日志 
                                VmOperatorLog.InsertOperatorLog(1, "删除签到班班次", description.ToString(), completeCallBack);
                            }

                        }

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



        #region 记工时班次相关操作  add by szr 2014-11-14

        private BaseViewModelCollection<UserClassOrderJiGongShiInfo> _classOrderJiGongShiInfos;
        /// <summary>
        /// 班次信息列表
        /// </summary>
        public BaseViewModelCollection<UserClassOrderJiGongShiInfo> ClassOrderJiGongShiInfos
        {
            get
            {
                return _classOrderJiGongShiInfos;
            }
            set
            {
                _classOrderJiGongShiInfos = value;
                OnPropertyChanged<BaseViewModelCollection<UserClassOrderJiGongShiInfo>>(() => this.ClassOrderJiGongShiInfos);
            }
        }

        /// <summary>
        /// 添加班次按钮是否可操作
        /// </summary>
        public bool IsAddClassOrderJiGongShiBtnEnable
        {
            get
            {
                if (SelectedClassType == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

        }

        private bool _isBatchOperateClassOrderJiGongShiBtnEnable;
        /// <summary>
        /// 对勾选的记工时班次item批量操作按钮enable属性
        /// </summary>
        public bool IsBatchOperateClassOrderJiGongShiBtnEnable
        {
            get
            {
                return _isBatchOperateClassOrderJiGongShiBtnEnable;
            }
            set
            {
                _isBatchOperateClassOrderJiGongShiBtnEnable = value;
                OnPropertyChanged<bool>(() => this.IsBatchOperateClassOrderJiGongShiBtnEnable);
            }
        }

        /// <summary>
        /// 记工时班次全选按钮
        /// </summary>
        public MarkObject ClassOrderJiGongShiAllSelect
        {
            get;
            set;
        }

        /// <summary>
        /// 添加新签到班班次命令
        /// </summary>
        public DelegateCommand AddClassOrderJiGongShiCommand
        {
            get;
            set;
        }

        /// <summary>
        /// 批量删除签到班班次命令
        /// </summary>
        public DelegateCommand BatchDeleteClassOrderJiGongShiCommand
        {
            get;
            set;
        }

        /// <summary>
        /// 检查ClassOrderSign是否有item被选中
        /// </summary>
        private bool CheckIsAnyClassOrderJiGongShiSelected()
        {

            foreach (var item in ClassOrderJiGongShiInfos)
            {
                if (item.isSelected)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 检查ClassOrderJiGongShi的Item是否全部被选中
        /// </summary>
        private bool CheckIsAllClassOrderJiGongShiSelected()
        {
            if (ClassOrderJiGongShiInfos.Count == 0)
            {
                return false;
            }


            foreach (var item in ClassOrderJiGongShiInfos)
            {
                if (!item.isSelected)
                {
                    return false;
                }
            }
            return true;

        }

        /// <summary>
        /// 获取对应班制的记工时班次信息
        /// </summary>
        /// <param name="classTypeId">对应班制Id</param>
        private void GetClassOrderJiGongShiInfo(int classTypeId)
        {
            if (classTypeId < 0)
            {
                return;
            }
            _currentClassTypeId = classTypeId;
            try
            {
                WaitingDialog.ShowWaiting();

                ServiceDomDbAcess.ReOpenSever();

                EntityQuery<UserClassOrderJiGongShiInfo> list = ServiceDomDbAcess.GetSever().GetClassOrderJiGongShiInfosByClassTypeQuery(classTypeId);//==//
                ///回调异常类
                Action<LoadOperation<UserClassOrderJiGongShiInfo>> actionCallBack = ErrorHandle<UserClassOrderJiGongShiInfo>.OnLoadErrorCallBack;
                ///异步事件
                LoadOperation<UserClassOrderJiGongShiInfo> lo = ServiceDomDbAcess.GetSever().Load(list, actionCallBack, null);
                lo.Completed += delegate
                {

                    ClassOrderJiGongShiInfos = new BaseViewModelCollection<UserClassOrderJiGongShiInfo>();

                    //异步获取数据
                    foreach (UserClassOrderJiGongShiInfo ar in lo.Entities)
                    {
                        ClassOrderJiGongShiInfos.Add(ar);
                    }

                    this.IsBatchOperateClassOrderJiGongShiBtnEnable = CheckIsAnyClassOrderJiGongShiSelected();
                    this.ClassOrderJiGongShiAllSelect.Selected = this.CheckIsAllClassOrderJiGongShiSelected();

                    WaitingDialog.HideWaiting();
                };
            }
            catch (Exception e)
            {
                WaitingDialog.HideWaiting();
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
                _currentClassTypeId = -1;
            }
        }

       
        /// 选中全部或者取消选中ClassOrderJiGongShi szr 2014-11-14
        /// </summary> 
        /// <param name="isChecked">是否全选标志</param>
        public void SelectAllClassOrderJiGongShi(bool? isChecked)
        {
            if (!isChecked.HasValue)
            {
                return;
            }

            if (isChecked.Value)
            {
                foreach (var item in ClassOrderJiGongShiInfos)
                {
                    item.isSelected = true;
                }
            }
            else
            {
                foreach (var item in ClassOrderJiGongShiInfos)
                {
                    item.isSelected = false;
                }
            }
            this.IsBatchOperateClassOrderJiGongShiBtnEnable = CheckIsAnyClassOrderJiGongShiSelected();
            this.ClassOrderJiGongShiAllSelect.Selected = CheckIsAnyClassOrderJiGongShiSelected();
        }



        /// <summary>
        /// 点击第一列选择ClassOrder
        /// </summary>
        /// <param name="sender">UserClassOrderSignInfo对象</param>
        public void SelectOneClassOrderJiGongShi(UserClassOrderJiGongShiInfo classOrderInfoJiGongShi)
        {          
            classOrderInfoJiGongShi.isSelected = !classOrderInfoJiGongShi.isSelected;

            this.IsBatchOperateClassOrderJiGongShiBtnEnable = CheckIsAnyClassOrderJiGongShiSelected();
            this.ClassOrderJiGongShiAllSelect.Selected = CheckIsAllClassOrderJiGongShiSelected();
        }

        /// <summary>
        /// 修改记工时班次
        /// </summary>
        /// <param name="info">记工时班次信息</param>
        public void ModifyClassOrderJiGongShi(UserClassOrderJiGongShiInfo info)
        {
            ChildWnd_OperateClassOrderJiGongShi  modifyClassOrderJiGongShi = new ChildWnd_OperateClassOrderJiGongShi(info, ChildWndOptionMode.Modify, ClassOrderOperate_callback);
            modifyClassOrderJiGongShi.VmOperateClassOrderJiGongShi.ClassTypeInfos = this.ClassTypeInfos;
            modifyClassOrderJiGongShi.VmOperateClassOrderJiGongShi.SelectedClassType = this.SelectedClassType;
            modifyClassOrderJiGongShi.Show();
        }

        /// <summary>
        /// 添加记工时班次
        /// </summary>
        private void AddClassOrderJiGongShi()
        {
            ChildWnd_OperateClassOrderJiGongShi addClassOrderJiGongShi = new ChildWnd_OperateClassOrderJiGongShi(null, ChildWndOptionMode.Add, ClassOrderOperate_callback, _currentClassTypeId);
            addClassOrderJiGongShi.VmOperateClassOrderJiGongShi.ClassTypeInfos = this.ClassTypeInfos;
            addClassOrderJiGongShi.Show();
        }

        /// <summary>
        /// 删除记工时班次
        /// </summary>
        /// <param name="info">记工时班次信息</param>
        public void DeleteClassOrderJiGongShi(UserClassOrderJiGongShiInfo info)
        {
            MsgBoxWindow.MsgBox(string.Format("请注意，您将进行如下操作:\r\n删除签到班班次【{0}】！", info.class_order_name),
                MsgBoxWindow.MsgIcon.Warning, MsgBoxWindow.MsgBtns.OKCancel, (result) =>
                {
                    if (result == MsgBoxWindow.MsgResult.OK)
                    {
                        UserClassOrderJiGongShiInfo[] classOrders = new UserClassOrderJiGongShiInfo[1];
                        classOrders[0] = info;
                        BatchDeleteClassOrderJiGongShiRia(classOrders);
                    }
                });
        }

        /// <summary>
        /// 批量删除记工时班次
        /// </summary>
        private void BatchDeleteClassOrderJiGongShi()
        {
            List<UserClassOrderJiGongShiInfo> selectedClassJiGongShiSigns = new List<UserClassOrderJiGongShiInfo>();
            foreach (var item in ClassOrderJiGongShiInfos)
            {
                if (item.isSelected)
                {
                    selectedClassJiGongShiSigns.Add(item);
                }
            }
            if (selectedClassJiGongShiSigns.Count > 0)
            {
                MsgBoxWindow.MsgBox(
                        "请注意，您将进行如下操作\r\n批量删除记工时班次！",
                        Dialog.MsgBoxWindow.MsgIcon.Warning,
                        Dialog.MsgBoxWindow.MsgBtns.OKCancel,
                        (e) =>
                        {
                            if (e == MsgBoxWindow.MsgResult.OK)
                            {
                                BatchDeleteClassOrderJiGongShiRia(selectedClassJiGongShiSigns.ToArray());
                            }
                        });
            }
            else
            {
                MsgBoxWindow.MsgBox(
                                "至少选择一个选项！",
                                Dialog.MsgBoxWindow.MsgIcon.Information,
                                Dialog.MsgBoxWindow.MsgBtns.OK);
            }

        }
        
        /// <summary>
        /// 批量删除记工时班次
        /// </summary>
        /// <param name="IDs">待删除的签到班班次ID</param>
        private void BatchDeleteClassOrderJiGongShiRia(UserClassOrderJiGongShiInfo[] classOrders)
        {
            //获取操作描述
            StringBuilder description = new StringBuilder(string.Format("所属班制：{0}；\r\n班次名称：{1}",classOrders[0].class_type_name,classOrders[0].class_order_name));

            List<string> Ids_str = new List<string>();
            for (int i = 0; i < classOrders.Length; i++)
            {
                Ids_str.Add(PublicMethods.ToString(classOrders[i].class_order_id));
                description.Append(classOrders[i].class_order_name + "，");
            }
            description.Remove(description.Length - 1, 1);
            description.Append("；\r\n");

            try
            {
                //WaitingDialog.ShowWaiting();
                _serviceDomDbAccess = new DomainServiceIriskingAttend();
                EntityQuery<OptionInfo> list = _serviceDomDbAccess.DeleteClassOrderJiGongShiQuery(Ids_str.ToArray());

                ///回调异常类
                Action<LoadOperation<OptionInfo>> actionCallBack = new Action<LoadOperation<OptionInfo>>(ErrorHandle<OptionInfo>.OnLoadErrorCallBack);
                ///异步事件
                LoadOperation<OptionInfo> lo = this._serviceDomDbAccess.Load(list, actionCallBack, null);
                lo.Completed += delegate
                {
                    VmOperatorLog.CompleteCallBack completeCallBack = () =>
                    {
                        _serviceDomDbAccess = new DomainServiceIriskingAttend();
                        GetClassOrderJiGongShiInfo(_currentClassTypeId);
                    };

                    foreach (OptionInfo item in lo.Entities)
                    {
                        if (!item.isSuccess)
                        {
                            MsgBoxWindow.MsgBox("提示",
                                item.option_info + "！",
                                Dialog.MsgBoxWindow.MsgIcon.Error,
                                Dialog.MsgBoxWindow.MsgBtns.OK);
                            //写入操作员日志 
                            VmOperatorLog.InsertOperatorLog(0, "删除记工时班次", description.ToString() + item.option_info, completeCallBack);
                        }
                        else
                        {
                            if (!item.isNotifySuccess)
                            {
                                MsgBoxWindow.MsgBox("提示",
                                 item.option_info + "！",
                                 Dialog.MsgBoxWindow.MsgIcon.Warning,
                                 Dialog.MsgBoxWindow.MsgBtns.OK);
                                //写入操作员日志 
                                VmOperatorLog.InsertOperatorLog(1, "删除记工时班次", description.ToString() + item.option_info, completeCallBack);
                            }
                            else
                            {
                                MsgBoxWindow.MsgBox("提示",
                                 item.option_info + "！",
                                 Dialog.MsgBoxWindow.MsgIcon.Succeed,
                                 Dialog.MsgBoxWindow.MsgBtns.OK);
                                //写入操作员日志 
                                VmOperatorLog.InsertOperatorLog(1, "删除记工时班次", description.ToString(), completeCallBack);
                            }

                        }

                        break;
                    }
                    //WaitingDialog.HideWaiting();

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



    }



}
