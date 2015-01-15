/*************************************************************************
** 文件名:   VmChildWndOperateClassType.cs
×× 主要类:   VmChildWndOperateClassType
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   cty
** 日  期:   2013-6-14
** 修改人:   wz 代码优化
** 日  期:   2013-7-24
** 描  述:   VmChildWndOperateClassType类,单个班制管理
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
using System.Windows.Media.Imaging;
using System.IO;
using IriskingAttend.Dialog;
using IriskingAttend.BehaviorSelf;
using IriskingAttend.ViewModel.SystemViewModel;
using System.Text;

namespace IriskingAttend.ViewModel.PeopleViewModel
{
    /// <summary>
    /// 单班制管理界面的viewModel
    /// </summary>
    public class VmChildWndOperateClassType : BaseViewModel
    {

        #region 字段声明
       
        /// <summary>
        /// 与服务声明
        /// </summary>
        private DomainServiceIriskingAttend serviceDomDbAccess = new DomainServiceIriskingAttend();

        /// <summary>
        /// vm加载完毕事件
        /// </summary>
        public event EventHandler LoadCompletedEvent;

        /// <summary>
        /// 窗口关闭事件
        /// </summary>
        public event Action<bool> CloseEvent;

        //关联的子窗口的模式
        private ChildWndOptionMode operateClassTypeMode;

        //当前班制数据源
        private UserClassTypeInfo classTypeInfo;

        //是否进行了继续添加操作
        private bool _isContinueAddExcuted = false;

        #endregion

        #region 与页面绑定的属性
        //命令
        public DelegateCommand OkBtnCmd { get; set; }
        public DelegateCommand CancelBtnCmd { get; set; }
        public DelegateCommand ContinueBtnCmd { get; set; }

        
        private string okBtnContent;
        /// <summary>
        /// okBtn 内容
        /// </summary>
        public string OkBtnContent
        {
            get { return okBtnContent; }
            set
            {
                okBtnContent = value;
                this.NotifyPropertyChanged("OkBtnContent");
            }
        }

        private string title;
        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                this.NotifyPropertyChanged("Title");
            }
        }

        
        private string classTypeName;
        /// <summary>
        /// 班制名称
        /// </summary>
        public string ClassTypeName
        {
            get { return classTypeName; }
            set
            {
                classTypeName = value;
                this.NotifyPropertyChanged("ClassTypeName");
            }
        }

        private string memo;
        /// <summary>
        /// 备注
        /// </summary>
        public string Memo
        {
            get { return memo; }
            set
            {
                memo = value;
                this.NotifyPropertyChanged("Memo");
            }
        }

        private bool isReadOnly;

        public bool IsReadOnly
        {
            get { return isReadOnly; }
            set
            {
                isReadOnly = value;
                this.NotifyPropertyChanged("IsReadOnly");
            }
        }

        
        private double opacityOfContinueAdd;
        /// <summary>
        /// 继续添加btn的透明度
        /// </summary>
        public double OpacityOfContinueAdd
        {
            get { return opacityOfContinueAdd; }
            set
            {
                opacityOfContinueAdd = value;
                this.NotifyPropertyChanged("OpacityOfContinueAdd");
            }
        }

        #endregion

        #region 构造函数
        
        public VmChildWndOperateClassType(UserClassTypeInfo cInfo, ChildWndOptionMode mode)
        {
            classTypeInfo = cInfo;
            operateClassTypeMode = mode;
            GetContent(mode);
            if (mode != ChildWndOptionMode.Add)
            {
                ClassTypeName = cInfo.class_type_name;
                Memo = cInfo.memo;
            }

            OkBtnCmd = new DelegateCommand(new Action(OkBtnClicked));
            CancelBtnCmd = new DelegateCommand(new Action(CancelBtnClicked));
            ContinueBtnCmd = new DelegateCommand(new Action(ContinueBtnClicked));

            if (LoadCompletedEvent != null)
            {
                LoadCompletedEvent(this, null);
            }

        }

        #endregion

        #region 私有功能函数
        /// <summary>
        /// 根据操作模式确定窗体其中的内容
        /// </summary>
        /// <param name="mode"></param>
        private void GetContent(ChildWndOptionMode mode)
        {
            OpacityOfContinueAdd = 0;
            switch (mode)
            {
                case ChildWndOptionMode.Modify:
                    OkBtnContent = "修改";
                    IsReadOnly = false;
                    Title = "修改班制信息";
                    break;
                case ChildWndOptionMode.Delete:
                    OkBtnContent = "删除";
                    IsReadOnly = true;
                    Title = "删除班制信息";
                    break;
                case ChildWndOptionMode.Check:
                    OkBtnContent = "确定";
                    IsReadOnly = true;
                    Title = "查看班制信息";
                    break;
                case ChildWndOptionMode.Add:
                    OkBtnContent = "添加";
                    OpacityOfContinueAdd = 1;
                    IsReadOnly = false;
                    Title = "添加班制信息";
                    break;
            }

        
        }

        /// <summary>
        /// 获取修改班制操作的描述 
        /// </summary>
        /// <returns></returns>
        private string GetModfiyDescription(out bool isModify)
        {
            StringBuilder description = new StringBuilder(string.Format("班制名称：{0}；\r\n", classTypeInfo.class_type_name));
           
            isModify = false;
            if (classTypeInfo.class_type_name != ClassTypeName)
            {
                isModify = true;
                description.Append(string.Format("名称：{0}=>{1}，", classTypeInfo.class_type_name, ClassTypeName));
            }
            if (classTypeInfo.memo != Memo)
            {
                isModify = true;
                description.Append(string.Format("备注：{0}=>{1}，", classTypeInfo.memo, Memo));
            }
            return description.ToString();
        }
        
        #endregion

        #region 界面命令响应
        private void OkBtnClicked()
        {
            switch (operateClassTypeMode)
            {
                case ChildWndOptionMode.Modify:
                    ModifyClassType();
                    break;
                case ChildWndOptionMode.Delete:
                    DeleteClassType();
                    break;
                case ChildWndOptionMode.Check:
                    if (CloseEvent != null)
                    {
                        CloseEvent(false);
                    }
                    break;
                case ChildWndOptionMode.Add:
                    AddClassType(false);
                    break;
            }
        

           
        }

        private void CancelBtnClicked()
        {
            if (CloseEvent != null)
            {
                CloseEvent(_isContinueAddExcuted);
            }
        }

        private void ContinueBtnClicked()
        {
            if (operateClassTypeMode == ChildWndOptionMode.Add)
            {
                AddClassType(true);
            }
        }

        #endregion

        #region wcf ria 操作
       
        /// <summary>
        /// 添加新班制
        /// </summary>
        /// <param name="isCoutinue">是否继续添加</param>
        private void AddClassType(bool isCoutinue)
        {
            if (isCoutinue)
            {
                _isContinueAddExcuted = true;
            }

            if (ClassTypeName == null || ClassTypeName.Equals(""))
            {
                MsgBoxWindow.MsgBox(
                             "班制名称不能为空！",
                             MsgBoxWindow.MsgIcon.Information,
                             MsgBoxWindow.MsgBtns.OK);
                return;
            }

            string class_type_name = PublicMethods.ToString(ClassTypeName);
            string memo = PublicMethods.ToString(Memo);
            //获取操作描述
            string description = "班制名称：" + ClassTypeName + "；\r\n";

            try
            {
                serviceDomDbAccess = new DomainServiceIriskingAttend();
                EntityQuery<OptionInfo> list = serviceDomDbAccess.AddClassTypeQuery(class_type_name, memo);
                WaitingDialog.ShowWaiting();
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
                             item.option_info + "！",
                             MsgBoxWindow.MsgIcon.Error,
                             MsgBoxWindow.MsgBtns.OK);
                            //写入操作员日志
                            VmOperatorLog.InsertOperatorLog(0, "添加班制", description + item.option_info, null);
                        }
                        else
                        {
                            if (!item.isNotifySuccess)
                            {
                                MsgBoxWindow.MsgBox(
                                   item.option_info + "！",
                                   MsgBoxWindow.MsgIcon.Warning,
                                   MsgBoxWindow.MsgBtns.OK);
                                //写入操作员日志
                                VmOperatorLog.InsertOperatorLog(1, "添加班制", description + item.option_info, () =>
                                    {
                                        if (!isCoutinue)
                                        {
                                            if (CloseEvent != null)
                                            {
                                                CloseEvent(true);
                                            }
                                        }
                                    });
                            }
                            else
                            {
                                MsgBoxWindow.MsgBox(
                                 item.option_info + "！",
                                 MsgBoxWindow.MsgIcon.Succeed,
                                 MsgBoxWindow.MsgBtns.OK);
                                //写入操作员日志
                                VmOperatorLog.InsertOperatorLog(1, "添加班制", description , () =>
                                    {
                                        if (!isCoutinue)
                                        {
                                            if (CloseEvent != null)
                                            {
                                                CloseEvent(true);
                                            }
                                        }
                                    });
                            }
                           
                            this.ClassTypeName = "";
                            this.Memo = "";

                        }
                      
                       
                        WaitingDialog.HideWaiting();
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

        /// <summary>
        /// 修改当前班制信息
        /// </summary>
        private void ModifyClassType()
        {
            if (ClassTypeName == null || ClassTypeName.Equals(""))
            {
                MsgBoxWindow.MsgBox(
                            "班制名称不能为空！",
                            MsgBoxWindow.MsgIcon.Information,
                            MsgBoxWindow.MsgBtns.OK);
                return;
            }
            string class_type_name = PublicMethods.ToString(ClassTypeName);
            string class_type_id = PublicMethods.ToString(classTypeInfo.class_type_id);
            string memo = PublicMethods.ToString(Memo);


            //获取操作描述
            bool isModify = false;
            string description = GetModfiyDescription(out isModify);
            if (!isModify)
            {
                if (CloseEvent != null)
                {
                    CloseEvent(true);
                }
                return;
            }


            try
            {
                serviceDomDbAccess = new DomainServiceIriskingAttend();
                EntityQuery<OptionInfo> list = serviceDomDbAccess.ModifyClassTypeQuery(class_type_id,
                    class_type_name, memo);
                WaitingDialog.ShowWaiting();

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
                                item.option_info + "！",
                                MsgBoxWindow.MsgIcon.Warning,
                                MsgBoxWindow.MsgBtns.OK);
                            //操作员日志
                            VmOperatorLog.InsertOperatorLog(0, "修改班制", description + item.option_info, null);
                        }
                        else
                        {
                            VmOperatorLog.CompleteCallBack completeCallBack = () =>
                                {
                                    if (CloseEvent != null)
                                    {
                                        CloseEvent(true);
                                    }
                                };
                            if (!item.isNotifySuccess)
                            {
                                MsgBoxWindow.MsgBox(
                                  item.option_info + "！",
                                  MsgBoxWindow.MsgIcon.Warning,
                                  MsgBoxWindow.MsgBtns.OK);
                                //操作员日志
                                VmOperatorLog.InsertOperatorLog(1, "修改班制", description + item.option_info, completeCallBack);   
                            }
                            else
                            {
                                MsgBoxWindow.MsgBox(
                                  item.option_info + "！",
                                  MsgBoxWindow.MsgIcon.Succeed,
                                  MsgBoxWindow.MsgBtns.OK);
                                //操作员日志
                                VmOperatorLog.InsertOperatorLog(1, "修改班制", description, completeCallBack);
                            }
                        }
                        WaitingDialog.HideWaiting();
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

        /// <summary>
        /// 删除当前班制
        /// </summary>
        private void DeleteClassType()
        {
            string class_type_id = PublicMethods.ToString(classTypeInfo.class_type_id);
            string[] class_type_ids = new string[1];
            class_type_ids[0] = class_type_id;
            try
            {
                serviceDomDbAccess = new DomainServiceIriskingAttend();
                EntityQuery<OptionInfo> list = serviceDomDbAccess.DeleteClassTypeQuery(class_type_ids);
                WaitingDialog.ShowWaiting();
                ///回调异常类
                Action<LoadOperation<OptionInfo>> onLoadErrorCallBack = new Action<LoadOperation<OptionInfo>>(ErrorHandle<OptionInfo>.OnLoadErrorCallBack);
                ///异步事件
                LoadOperation<OptionInfo> lo = this.serviceDomDbAccess.Load(list, onLoadErrorCallBack, null);
                lo.Completed += delegate
                {
                    foreach (OptionInfo optionInfo in lo.Entities)
                    {
                        if (!optionInfo.isSuccess)
                        {
                            MsgBoxWindow.MsgBox(
                               optionInfo.option_info + "！",
                               MsgBoxWindow.MsgIcon.Error,
                               MsgBoxWindow.MsgBtns.OK);
                        }
                        else
                        {
                            if (!optionInfo.isNotifySuccess)
                            {
                                MsgBoxWindow.MsgBox(
                                  optionInfo.option_info + "！",
                                  MsgBoxWindow.MsgIcon.Warning,
                                  MsgBoxWindow.MsgBtns.OK);
                            }
                            else
                            {
                                MsgBoxWindow.MsgBox(
                                  optionInfo.option_info + "！",
                                  MsgBoxWindow.MsgIcon.Succeed,
                                  MsgBoxWindow.MsgBtns.OK);
                            }
                            if (CloseEvent != null)
                            {
                                CloseEvent(true);
                            }
                        }
                      
                        WaitingDialog.HideWaiting();
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

    }
}
