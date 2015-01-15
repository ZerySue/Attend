/*************************************************************************
** 文件名:   VmFestivalManage.cs
** 主要类:   VmFestivalManage
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2013-12-12
** 修改人:   
** 日  期:
** 描  述:   VmFestivalManage,节假日管理
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
using IriskingAttend.View.AttendView;
using IriskingAttend.Dialog;
using System.ComponentModel;
using IriskingAttend.View;


namespace IriskingAttend.ViewModel.AttendViewModel
{
    public class VmFestivalManage : BaseViewModel
    {
        #region 私有变量：域服务声明

        /// <summary>
        /// 域服务声明
        /// </summary>
        private DomainServiceIriskingAttend _domSrvDbAccess = new DomainServiceIriskingAttend();

        #endregion

        #region   与页面绑定的属性

        /// <summary>
        /// 全选按钮绑定
        /// </summary>
        public MarkObject MarkObj 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// 节假日信息表
        /// </summary>
        private BaseViewModelCollection<FestivalInfo> _systemFestivalInfo;
        public BaseViewModelCollection<FestivalInfo> SystemFestivalInfo
        {
            get
            { 
                return _systemFestivalInfo; 
            }
            set
            {
                _systemFestivalInfo = value;
                OnPropertyChanged<BaseViewModelCollection<FestivalInfo>>(() => this.SystemFestivalInfo);
            }
        }

        /// <summary>
        /// 选中节假日信息
        /// </summary>
        private FestivalInfo _selectFestivalInfoItem;
        public FestivalInfo SelectFestivalInfoItem
        {
            get
            {
                return _selectFestivalInfoItem;
            }
            set
            {                
                _selectFestivalInfoItem = value;
                OnPropertyChanged<FestivalInfo>(() => this.SelectFestivalInfoItem);       
            }
        }

        /// <summary>
        /// 批量删除节假日按钮的enable属性
        /// </summary>
        private bool _isBatchDeleteEnabled;
        public bool IsBatchDeleteEnabled
        {
            get 
            { 
                return _isBatchDeleteEnabled; 
            }
            set
            {
                _isBatchDeleteEnabled = value;
                OnPropertyChanged<bool>(() => this.IsBatchDeleteEnabled);  
            }
        }

        #endregion

        #region 事件

        /// <summary>
        /// 节假日信息加载完成
        /// </summary>
        public event EventHandler FestivalLoadCompleted;

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数,初始化属性
        /// </summary>
        public VmFestivalManage()
        {
            //节假日信息表初始化
            SystemFestivalInfo = new BaseViewModelCollection<FestivalInfo>();

            //选中节假日信息初始化
            SelectFestivalInfoItem = new FestivalInfo();

            //批量删除按钮处于不可用状态
            IsBatchDeleteEnabled = false;

            //事件初始化
            FestivalLoadCompleted += (a, e) => { };
        }

        #endregion

        #region  控件绑定函数操作
      
        /// <summary>
        /// 批量删除节假日
        /// </summary>
        public void BatchDeleteFestival()
        {
            //对话框提示信息
            string strInfo = "请注意，您将进行如下操作：\r\n批量删除节假日信息！";
            MsgBoxWindow.MsgBox( strInfo, MsgBoxWindow.MsgIcon.Warning, MsgBoxWindow.MsgBtns.OKCancel, (result) =>　
            {
                //确定删除
                if (result == MsgBoxWindow.MsgResult.OK)
                {
                    //将选中的节假日添加到删除列表中
                    List<int> festivalIDs = new List<int>();
                    foreach (FestivalInfo fesval in SystemFestivalInfo)
                    {
                        if (fesval.isSelected)
                        {
                            festivalIDs.Add(fesval.festival_id);
                        }
                    }

                    //通过ria向后台发送请求
                    DeleteFestivalRia(festivalIDs.ToArray());
                }
            });             
        }

        /// <summary>
        /// 删除节假日
        /// </summary>
        public void DeleteFestival()
        {
            string strInfo = string.Format("请注意，您将进行如下操作：\r\n删除节假日\"{0}\"的信息！", this.SelectFestivalInfoItem.name);
            MsgBoxWindow.MsgBox( strInfo, MsgBoxWindow.MsgIcon.Warning, MsgBoxWindow.MsgBtns.OKCancel, (result) =>　
            {
                //确定删除
                if (result == MsgBoxWindow.MsgResult.OK)
                {
                    //将欲删除的节假日ID添加到删除列表中
                    List<int> festivalIDs = new List<int>();
                    festivalIDs.Add(this.SelectFestivalInfoItem.festival_id);

                    //通过ria向后台发送请求
                    DeleteFestivalRia(festivalIDs.ToArray());
                }
            });  
        }

        /// <summary>
        /// 选中全部节假日或者取消选中
        /// </summary>
        /// <param name="isChecked">true: 选中， false：未选中</param>
        public void SelectAllFestival(bool isChecked)
        {
            //将全部节假日置为全选或全部选状态
            foreach (var item in SystemFestivalInfo)
            {
                item.isSelected = isChecked;
            }

            //更新批量删除按钮的可用状态
            IsBatchDeleteEnabled = CheckIsBatchDeleteEnabled();
        }

        /// <summary>
        /// 确定批量删除按钮的可用状态
        /// </summary>
        /// <returns>true: 可用状态， false：不可用状态</returns>
        private bool CheckIsBatchDeleteEnabled()
        {
            //遍历所有节假日信息
            foreach (FestivalInfo dev in SystemFestivalInfo)
            {
                //只要有一台节假日处于选中状态，批量删除按钮就可用。
                if (dev.isSelected)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 确定全选按钮的选中状态
        /// </summary>
        /// <returns>选中状态</returns>
        private bool CheckIsAllDevSelected()
        {
            //默认为选中
            bool checkAll = true;

            //遍历所有节假日信息
            foreach (FestivalInfo dev in SystemFestivalInfo)
            {
                //只要有一条节假日信息未选中，全选按钮就处于未选中状态
                if (!dev.isSelected)
                {
                    checkAll = false;
                    break;
                }
            }
            return checkAll;
        }
        
        /// <summary>
        /// 更改当前选中节假日的选中状态
        /// </summary>
        /// <param name="selectDevInfo">选中节假日的节假日信息源</param>
        public void ChangeFestivalCheckedState(FestivalInfo selectDevInfo )
        {   
            //反选选中节假日状态  
            selectDevInfo.isSelected = !selectDevInfo.isSelected;

            //更新全选按钮的选中状态
            MarkObj.Selected = CheckIsAllDevSelected();            

            //更新批量删除按钮的可用状态
            IsBatchDeleteEnabled = CheckIsBatchDeleteEnabled();
        }
        
        #endregion

        #region  通过ria连接后台，数据库相关操作

        /// <summary>
        /// ria方式通过后台删除节假日
        /// </summary>
        /// <param name="festivalIds">欲删除的节假日ID字符串数组</param>
        private void DeleteFestivalRia(int[] festivalIds)
        {
            try
            {
                Action<InvokeOperation<byte>> onInvokeErrCallBack = CallBackHandleControl<byte>.OnInvokeErrorCallBack;
                CallBackHandleControl<byte>.m_sendValue = (o) =>
                {
                    //隐藏等待界面
                    WaitingDialog.HideWaiting();                   

                    //异步获取数据                    
                    if (0X00 != o)
                    {
                        if (0XFF == o)
                        {
                            MsgBoxWindow.MsgBox("删除节假日操作成功，但通知后台失败，请检查后台！", MsgBoxWindow.MsgIcon.Warning, MsgBoxWindow.MsgBtns.OK);
                        }
                        else
                        {
                            MsgBoxWindow.MsgBox("删除节假日操作成功！", MsgBoxWindow.MsgIcon.Succeed, MsgBoxWindow.MsgBtns.OK);                            
                        }
                        //重新查询数据库,刷新节假日列表                       
                        GetFestivalInfoTableRia();
                    }
                    else
                    {
                        MsgBoxWindow.MsgBox("删除节假日操作失败！", MsgBoxWindow.MsgIcon.Error, MsgBoxWindow.MsgBtns.OK);
                    }
                   
                };

                //显示等待界面
                WaitingDialog.ShowWaiting();

                //通过后台执行删除节假日动作
                _domSrvDbAccess.BatchDeleteFestival(festivalIds, onInvokeErrCallBack, null);
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }

        /// <summary>
        /// 异步获取数据库中数据, 获取节假日信息表
        /// </summary>
        public void GetFestivalInfoTableRia()
        {
            try
            {
                _domSrvDbAccess = new DomainServiceIriskingAttend();

                //显示等待界面
                WaitingDialog.ShowWaiting();

                //获得节假日信息列表
                EntityQuery<FestivalInfo> devList = _domSrvDbAccess.GetAllFestivalQuery();

                ///回调异常类
                Action<LoadOperation<FestivalInfo>> loadDevCallBack = ErrorHandle<FestivalInfo>.OnLoadErrorCallBack;

                ///异步事件
                LoadOperation<FestivalInfo> loadOp = this._domSrvDbAccess.Load(devList, loadDevCallBack, null);
                loadOp.Completed += delegate
                {
                    //若SystemFestivalInfo节假日列表信息未分配内存，则进行内存分配
                    if (SystemFestivalInfo == null)
                    {
                        SystemFestivalInfo = new BaseViewModelCollection<FestivalInfo>();
                    }
                    else
                    {
                        //将节假日列表信息清空
                        SystemFestivalInfo.Clear();
                    }

                    //异步获取数据，将获取到的节假日信息添加到SystemFestivalInfo中去
                    foreach (FestivalInfo fesval in loadOp.Entities)
                    {
                        SystemFestivalInfo.Add(fesval);                        
                    }

                    //节假日信息加载完成后，发送事件
                    FestivalLoadCompleted(this, new EventArgs());

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

        #endregion 
    }
}
