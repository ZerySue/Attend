/*************************************************************************
** 文件名:   VmLocateRecordAdded.cs
** 主要类:   VmLocateRecordAdded
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2014-5-21
** 修改人:   
** 日  期:
** 描  述:   VmLocateRecordAdded，五虎山根据虹膜记录添加的定位记录
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
using IriskingAttend.ViewModel;
using System.ComponentModel;
using System.Collections.Generic;
using System.ServiceModel.DomainServices.Client;
using IriskingAttend.Common;
using EDatabaseError;
using Irisking.Web.DataModel;
using IriskingAttend.View;
using IriskingAttend.Dialog;

namespace IriskingAttend.NewWuHuShan
{
    public class VmLocateRecordAdded : BaseViewModel
    {
        #region 绑定属性

        /// <summary>
        /// 全选按钮绑定
        /// </summary>
        public MarkObject MarkObj
        {
            get;
            set;
        }

        private BaseViewModelCollection<LocateRecordAddedEntity> _locateRecordModel;

        /// <summary>
        /// 前台界面绑定的数据集合
        /// </summary>
        public BaseViewModelCollection<LocateRecordAddedEntity> LocateRecordModel
        {
            get 
            {
                return _locateRecordModel;
            }
            set
            {
                _locateRecordModel = value;
                OnPropertyChanged<BaseViewModelCollection<LocateRecordAddedEntity>>(() => this.LocateRecordModel);               
            }
        }

        /// <summary>
        /// 选中定位记录信息
        /// </summary>
        private LocateRecordAddedEntity _selectLocateRecordItem;
        public LocateRecordAddedEntity SelectLocateRecordItem
        {
            get
            {
                return _selectLocateRecordItem;
            }
            set
            {
                _selectLocateRecordItem = value;
                OnPropertyChanged<LocateRecordAddedEntity>(() => this.SelectLocateRecordItem);
            }
        }

        /// <summary>
        /// 批量删除定位记录按钮的enable属性
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
        /// 定位记录信息加载完成
        /// </summary>
        public event EventHandler LocateRecordLoadCompleted;

        #endregion

        private DateTime _beginTime;
        private DateTime _endTime;
        private int[] _departIds;
        private string _persnName;
        private string _workSn;

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public VmLocateRecordAdded()
        {
            LocateRecordModel = new BaseViewModelCollection<LocateRecordAddedEntity>();

            //选中定位记录信息初始化
            SelectLocateRecordItem = new LocateRecordAddedEntity();

            //批量删除按钮处于不可用状态
            IsBatchDeleteEnabled = false;

            //事件初始化
            LocateRecordLoadCompleted += (a, e) => { };
        }

        #endregion

        #region 根据查询条件查询数据并对数据进行处理

        /// <summary>
        /// 批量删除定位记录
        /// </summary>
        public void BatchDeleteLocateRecord()
        {
            //对话框提示信息
            string strInfo = "请注意，您将进行如下操作：\r\n批量删除定位记录信息！";
            MsgBoxWindow.MsgBox(strInfo, MsgBoxWindow.MsgIcon.Warning, MsgBoxWindow.MsgBtns.OKCancel, (result) =>
            {
                //确定删除
                if (result == MsgBoxWindow.MsgResult.OK)
                {
                    //将选中的定位记录添加到删除列表中
                    List<int> recordIDs = new List<int>();
                    foreach (LocateRecordAddedEntity record in LocateRecordModel)
                    {
                        if (record.isSelected)
                        {
                            recordIDs.Add(record.LocateRecordID);
                        }
                    }

                    //通过ria向后台发送请求
                    DeleteLocateRecordRia(recordIDs.ToArray());
                }
            });
        }

        /// <summary>
        /// ria方式通过后台删除定位记录
        /// </summary>
        /// <param name="deviceIds">欲删除的定位记录ID字符串数组</param>
        private void DeleteLocateRecordRia(int[] recordIDs)
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
                        
                        MsgBoxWindow.MsgBox("删除定位记录操作成功！", MsgBoxWindow.MsgIcon.Succeed, MsgBoxWindow.MsgBtns.OK);
                        
                        //重新查询数据库,刷新定位记录列表                       
                        GetLocateRecordCollect(_beginTime, _endTime, _departIds, _persnName, _workSn);
                    }
                    else
                    {
                        MsgBoxWindow.MsgBox("删除定位记录操作失败！", MsgBoxWindow.MsgIcon.Error, MsgBoxWindow.MsgBtns.OK);
                    }

                };

                //显示等待界面
                WaitingDialog.ShowWaiting();

                //通过后台执行删除定位记录动作
                ServiceDomDbAcess.GetSever().BatchDeleteLocateRecord(recordIDs, onInvokeErrCallBack, null);
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }

        /// <summary>
        /// 根据查询条件查询数据
        /// </summary>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">截止时间</param>         
        /// <returns></returns>
        public void GetLocateRecordCollect(DateTime beginTime, DateTime endTime, int[] departIds, string personName, string workSn)
        {
            _beginTime = beginTime;
            _endTime = endTime;
            _departIds = departIds;
            _persnName = personName;
            _workSn = workSn;

            WaitingDialog.ShowWaiting("正在查询，请等待");

            ServiceDomDbAcess.ReOpenSever();

            EntityQuery<LocateRecordAddedEntity> list = ServiceDomDbAcess.GetSever().GetLocateRecordQuery(beginTime, endTime, departIds,personName, workSn);
            //回调异常类
            Action<LoadOperation<LocateRecordAddedEntity>> actionCallBack = new Action<LoadOperation<LocateRecordAddedEntity>>(ErrorHandle<LocateRecordAddedEntity>.OnLoadErrorCallBack);
            //异步事件
            LoadOperation<LocateRecordAddedEntity> loadOp = ServiceDomDbAcess.GetSever().Load(list, actionCallBack, null);
            
            loadOp.Completed += delegate
            {
                try
                {
                    LocateRecordModel.Clear();

                    foreach (LocateRecordAddedEntity ar in loadOp.Entities)
                    {
                        LocateRecordModel.Add(ar);
                    }

                    //定位记录信息加载完成后，发送事件
                    LocateRecordLoadCompleted(this, new EventArgs());
                    WaitingDialog.HideWaiting();
                }
                catch (Exception e)
                {
                    ChildWindow errorWin = new ErrorWindow("加载异常", e.Message);
                    errorWin.Show();
                    WaitingDialog.HideWaiting();
                }               
            };            
        }
        
        #endregion

        #region  全选按钮
        /// <summary>
        /// 选中全部定位记录或者取消选中
        /// </summary>
        /// <param name="isChecked">true: 选中， false：未选中</param>
        public void SelectAllLocateRecord(bool isChecked)
        {
            //将全部定位记录置为全选或全部选状态
            foreach (var item in LocateRecordModel)
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
            //遍历所有定位记录信息
            foreach (LocateRecordAddedEntity dev in LocateRecordModel)
            {
                //只要有一条定位记录处于选中状态，批量删除按钮就可用。
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
        private bool CheckIsAllRecordSelected()
        {
            //默认为选中
            bool checkAll = true;

            //遍历所有定位记录信息
            foreach (LocateRecordAddedEntity dev in LocateRecordModel)
            {
                //只要有一条定位记录未选中，全选按钮就处于未选中状态
                if (!dev.isSelected)
                {
                    checkAll = false;
                    break;
                }
            }
            return checkAll;
        }
        
        /// <summary>
        /// 更改当前选中定位记录的选中状态
        /// </summary>
        /// <param name="selectRecordInfo">选中定位记录的定位记录信息源</param>
        public void ChangeLocateRecordCheckedState(LocateRecordAddedEntity selectRecordInfo)
        {   
            //反选选中定位记录状态  
            selectRecordInfo.isSelected = !selectRecordInfo.isSelected;

            UpdateCheckAllState();
        }

        /// <summary>
        /// 更新全选按钮的状态
        /// </summary>
        public void UpdateCheckAllState()
        {
            //更新全选按钮的选中状态
            MarkObj.Selected = CheckIsAllRecordSelected();

            //更新批量删除按钮的可用状态
            IsBatchDeleteEnabled = CheckIsBatchDeleteEnabled();
        }
        
        #endregion

    }
}
