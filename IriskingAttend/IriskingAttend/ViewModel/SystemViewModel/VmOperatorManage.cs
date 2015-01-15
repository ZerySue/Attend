/*************************************************************************
** 文件名:   VmOperatorManage.cs
** 主要类:   VmOperatorManage
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2013-6-14
** 修改人:   
** 日  期:
** 描  述:   VmOperatorManage,操作员管理
**
** 版  本:   1.0.0
** 备  注:  命名及代码编写遵守C#编码规范
**
*************************************************************************/
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
using IriskingAttend.View.SystemView;
using IriskingAttend.Dialog;

namespace IriskingAttend.ViewModel.SystemViewModel
{
    public class VmOperatorManage : BaseViewModel
    {
        #region 私有变量：域服务声明
       
        /// <summary>
        /// 域服务声明 
        /// </summary>      
        private DomainServiceIriskingAttend _domSrvDbAccess = new DomainServiceIriskingAttend();

        #endregion

        #region 公共变量：回调函数声明

        //回调函数声明
        public delegate void CompleteCallBack();

        #endregion

        #region 与页面绑定的属性

        /// <summary>
        /// 操作员信息表
        /// </summary>
        private BaseViewModelCollection<operator_info> _operatorInfoMng;
        public BaseViewModelCollection<operator_info> OperatorInfoMng
        {
            get { return _operatorInfoMng; }
            set
            {
                _operatorInfoMng = value;
                OnPropertyChanged<BaseViewModelCollection<operator_info>>(() => this.OperatorInfoMng); 
            }
        }

        /// <summary>
        /// 选中的操作员信息
        /// </summary>
        private operator_info _selectOperatorInfoItem;
        public operator_info SelectOperatorInfoItem
        {
            get
            {
                return _selectOperatorInfoItem;
            }
            set
            {
                _selectOperatorInfoItem = value;
                OnPropertyChanged<operator_info>(() => this.SelectOperatorInfoItem); 
            }
        }  
 
        #endregion

        #region  构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public VmOperatorManage()
        {
            //绑定的变量属性初始化
            OperatorInfoMng = new BaseViewModelCollection<operator_info>();
            SelectOperatorInfoItem = new operator_info();
        }

        #endregion

        #region  控件绑定函数操作
        
        /// <summary>
        /// 删除操作员
        /// </summary>
        public void DeleteOperator()
        {
            if (VmLogin.IsSuperUser(this.SelectOperatorInfoItem.logname))
            {
                string info = string.Format( "{0}超级用户不能删除",this.SelectOperatorInfoItem.logname );
                MsgBoxWindow.MsgBox(info, MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);
                return;
            }

            string str = string.Format( "请注意，您将进行如下操作：\r\n删除操作员\"{0}\"的信息！", this.SelectOperatorInfoItem.logname);
            MsgBoxWindow.MsgBox( str, MsgBoxWindow.MsgIcon.Warning, MsgBoxWindow.MsgBtns.OKCancel, (Result) =>
            {
                if (Result == MsgBoxWindow.MsgResult.OK)
                {
                   DeleteOperatorRia();
                }
            });
        }

        #endregion

        #region  通过ria连接后台，数据库相关操作

        /// <summary>
        /// 异步获取数据库中数据，获取操作员信息表
        /// </summary>
        public void GetOperatorInfoTableRia(CompleteCallBack completeCallBack) 
        {
            try
            {
                _domSrvDbAccess = new DomainServiceIriskingAttend();

                //显示等待界面
                WaitingDialog.ShowWaiting();

                //获得操作员信息列表
                EntityQuery<operator_info> operatorList = _domSrvDbAccess.GetAllOperatorQuery();

                ///回调异常类
                Action<LoadOperation<operator_info>> loadOperatorCallBack = ErrorHandle<operator_info>.OnLoadErrorCallBack;
                ///异步事件
                LoadOperation<operator_info> loadOp = this._domSrvDbAccess.Load(operatorList, loadOperatorCallBack, null);
                loadOp.Completed += delegate
                {
                    //若OperatorInfoMng操作员列表信息未分配内存，则进行内存分配
                    if (OperatorInfoMng == null)
                    {
                        OperatorInfoMng = new BaseViewModelCollection<operator_info>();
                    }
                    else
                    {
                        //将操作员列表信息清空
                        OperatorInfoMng.Clear();
                    }

                    //异步获取数据，将获取到的操作员信息添加到OperatorInfoMng中去
                    foreach (operator_info opInfo in loadOp.Entities)
                    {
                        OperatorInfoMng.Add(opInfo);
                    }

                    if (completeCallBack != null)
                    {
                        completeCallBack();
                    }

                    //隐藏等待界面
                    WaitingDialog.HideWaiting();                    
                };
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }

        /// <summary>
        /// ria方式调用后台删除操作员
        /// </summary>
        private void DeleteOperatorRia()
        {
            try
            {
                Action<InvokeOperation<bool>> onInvokeErrCallBack = CallBackHandleControl<bool>.OnInvokeErrorCallBack;
                CallBackHandleControl<bool>.m_sendValue = (o) =>
                {
                    //隐藏等待界面
                    WaitingDialog.HideWaiting();                     

                    //异步获取数据
                    if (o)
                    {
                        MsgBoxWindow.MsgBox( string.Format("操作成功，操作员\"{0}\"信息被删除！", SelectOperatorInfoItem.logname), MsgBoxWindow.MsgIcon.Succeed, MsgBoxWindow.MsgBtns.OK);

                        //重新查询数据库,刷新操作员列表
                        GetOperatorInfoTableRia( null );
                    }
                    else
                    {
                        MsgBoxWindow.MsgBox( "删除操作员失败，请重试！", MsgBoxWindow.MsgIcon.Error, MsgBoxWindow.MsgBtns.OK);
                    }                    
                    
                };

                //显示等待界面
                WaitingDialog.ShowWaiting();

                //通过后台执行删除操作员动作
                _domSrvDbAccess.DeleteOperator(SelectOperatorInfoItem.logname, onInvokeErrCallBack, null);
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }
        #endregion        
    }
}
