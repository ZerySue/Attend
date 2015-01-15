/*************************************************************************
** 文件名:   VmQueryLunchRecord.cs
×× 主要类:   VmQueryLunchRecord
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   wz
** 日  期:   2013-9-12
** 修改人:   
** 日  期:   
** 描  述:   VmQueryLunchRecord.cs类,查询班中餐记录viewmodel
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
using IriskingAttend.Web.ZhouYuanShan;
using IriskingAttend.CustomUI;

namespace IriskingAttend.ZhouYuanShan.LunchSubsidy
{
    public class VmQueryLunchRecord : BaseViewModel
    {

        #region 字段声明

        private ChildWndSelectObj _childWndSelectObj;

        private TextComboBox _textCmbClassOrder;

        #endregion

        #region 与页面绑定的命令

        public DelegateCommand QueryCmd
        {
            get;
            set;
        }

        public DelegateCommand SelectObjCmd
        {
            get;
            set;
        }

        #endregion

        #region   与页面绑定的属性


        #region 部门班中餐
       
        private BaseViewModelCollection<LunchRecordInfoOnDepart> _lunchRecordInfoOnDepart = new BaseViewModelCollection<LunchRecordInfoOnDepart>();

        /// <summary>
        /// 班中餐记录按部门分组
        /// </summary>
        public BaseViewModelCollection<LunchRecordInfoOnDepart> LunchRecordInfoOnDepart
        {
            get { return _lunchRecordInfoOnDepart; }
            set
            {
                _lunchRecordInfoOnDepart = value;
                this.OnPropertyChanged(() => LunchRecordInfoOnDepart);
            }
        }

        private LunchRecordInfoOnDepart _selectedLunchRecordInfoOnDepart = null;

        /// <summary>
        /// 选择的部门班中餐记录
        /// </summary>
        public LunchRecordInfoOnDepart SelectedLunchRecordInfoOnDepart
        {
            get { return _selectedLunchRecordInfoOnDepart; }
            set
            {
                _selectedLunchRecordInfoOnDepart = value;
                this.OnPropertyChanged(() => SelectedLunchRecordInfoOnDepart);
            }
        }

        #endregion

        #region 个人班中餐记录
        

        private BaseViewModelCollection<LunchRecordInfoOnPerson> _lunchRecordInfoOnPerson = new BaseViewModelCollection<LunchRecordInfoOnPerson>();

        /// <summary>
        /// 班中餐记录详情
        /// </summary>
        public BaseViewModelCollection<LunchRecordInfoOnPerson> LunchRecordInfoOnPerson
        {
            get { return _lunchRecordInfoOnPerson; }
            set
            {
                _lunchRecordInfoOnPerson = value;
                this.OnPropertyChanged(() => LunchRecordInfoOnPerson);
            }
        }

        private LunchRecordInfoOnPerson _selectedLunchRecordInfoOnPerson = null;

        /// <summary>
        /// 选择的班中餐记录详情
        /// </summary>
        public LunchRecordInfoOnPerson SelectedLunchRecordInfoOnPerson
        {
            get { return _selectedLunchRecordInfoOnPerson; }
            set
            {
                _selectedLunchRecordInfoOnPerson = value;
                this.OnPropertyChanged(() => SelectedLunchRecordInfoOnPerson);
            }
        }

        #endregion

        #region 班次班中餐记录(其实和已完成班中餐一样的)


        private BaseViewModelCollection<ReportRecordInfoOnDepart_ZhouYuanShan> _lunchRecordInfoOnClassOrder = new BaseViewModelCollection<ReportRecordInfoOnDepart_ZhouYuanShan>();

        /// <summary>
        /// 班次班中餐记录
        /// </summary>
        public BaseViewModelCollection<ReportRecordInfoOnDepart_ZhouYuanShan> LunchRecordInfoOnClassOrder
        {
            get { return _lunchRecordInfoOnClassOrder; }
            set
            {
                _lunchRecordInfoOnClassOrder = value;
                this.OnPropertyChanged(() => LunchRecordInfoOnClassOrder);
            }
        }

        private ReportRecordInfoOnDepart_ZhouYuanShan _selectedLunchRecordInfoOnClassOrder = null;

        /// <summary>
        /// 选择的班次班中餐记录
        /// </summary>
        public ReportRecordInfoOnDepart_ZhouYuanShan SelectedLunchRecordInfoOnClassOrder
        {
            get { return _selectedLunchRecordInfoOnClassOrder; }
            set
            {
                _selectedLunchRecordInfoOnClassOrder = value;
                this.OnPropertyChanged(() => SelectedLunchRecordInfoOnClassOrder);
            }
        }

        #endregion

        /// <summary>
        /// 全选按钮的绑定类已完成
        /// </summary>
        public MarkObject MarkObjLunchRecordInfoOnDepart
        {
            get;
            set;
        }

          /// <summary>
        /// 全选按钮的绑定类未完成
        /// </summary>
        public MarkObject MarkObjLunchRecordInfoOnPerson
        { 
            get; 
            set; 
        }

        /// 全选按钮的绑定类未完成
        /// </summary>
        public MarkObject MarkObjLunchRecordInfoOnClassOrder
        {
            get;
            set;
        }

        private DateTime _beginTime;
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime BeginTime
        {
            get
            {
                return _beginTime;
            }
            set
            {
                _beginTime = value;
                OnPropertyChanged(() => BeginTime);
            }
        }

        private DateTime _endTime;
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime
        {
            get
            {
                return _endTime;
            }
            set
            {
                _endTime = value;
                OnPropertyChanged(() => BeginTime);
            }
        }

        private string _textSelectedObj;
        /// <summary>
        /// 当前选择的对象描述
        /// </summary>
        public string TextSelectedObj
        {
            get
            {
                return _textSelectedObj;
            }
            set
            {
                _textSelectedObj = value;
                OnPropertyChanged(() => TextSelectedObj);
            }
        }

        private string _textSelectedClassOrder;
        /// <summary>
        /// 当前选择的班次描述
        /// </summary>
        public string TextSelectedClassOrder
        {
            get
            {
                return _textSelectedClassOrder;
            }
            set
            {
                _textSelectedClassOrder = value;
                OnPropertyChanged(() => TextSelectedClassOrder);
            }
        }
        

        /// <summary>
        /// tabControl当前的选择序号
        /// 0 = 个人班中餐
        /// 1 = 班次班中餐
        /// 2 = 部门班中餐
        /// </summary>
        public int TabSelectedIndex
        {
            get;
            set;
        }



        #endregion

        #region 构造函数

        public VmQueryLunchRecord(TextComboBox textCmbClassOrder)
        {
            IsolatedStorageSettings appSettings = IsolatedStorageSettings.ApplicationSettings;
            if (appSettings.Contains("PageQueryLunchRecord_BeginTime") &&
                appSettings["PageQueryLunchRecord_BeginTime"] != null)
            {
                BeginTime = (DateTime)appSettings["PageQueryLunchRecord_BeginTime"];
            }
            else
            {
                BeginTime = DateTime.Now.Date.AddDays(-1);

            }
            if (appSettings.Contains("PageQueryLunchRecord_EndTime") &&
                appSettings["PageQueryLunchRecord_EndTime"] != null)
            {
                EndTime = (DateTime)appSettings["PageQueryLunchRecord_EndTime"];
            }
            else
            {
                EndTime = DateTime.Now;
            }

            
            QueryCmd = new DelegateCommand(Query);
            SelectObjCmd = new DelegateCommand(SelectObj);

            //获取班次
            GetClassOrderInfoRia((result) =>
            {
                textCmbClassOrder.InitCommonOperation();
                foreach (var item in result)
                {
                    CheckBox chkBox = new CheckBox();
                    chkBox.Content = item.class_order_name + "(" + item.attend_sign + ")";
                    chkBox.Tag = item.class_order_id;
                   
                    chkBox.IsChecked = true;   //默认班次全部选择
                    textCmbClassOrder.comboBox.Items.Add(chkBox);
                }
                //初始化多选控件
                textCmbClassOrder.InitComBoBoxItemSelectTrigger();
                textCmbClassOrder.SelectAll();


                _textCmbClassOrder = textCmbClassOrder;

                //初始化选择部门和人员窗口
                _childWndSelectObj = new ChildWndSelectObj();
                _childWndSelectObj.Init(false, true, true);

            });
          
        }

       
        #endregion

        #region 界面的事件响应

       
    

        //选择人员和部门
        private void SelectObj()
        {
            var TempWnd = _childWndSelectObj.Clone();
            TempWnd.Show();
            TempWnd.Closed += (s, e) =>
                {
                    if (TempWnd.DialogResult.Value)
                    {
                        TextSelectedObj = TempWnd.GetSelectObjDescription();
                        _childWndSelectObj = TempWnd;
                    }
                };
        }

        //查询班中餐列表
        private void Query()
        {
            List<int> classOrderIds = GetSelectedClassOrder();
            List<int> personIds = _childWndSelectObj.GetSelectedPersonIds();
            List<int> departIds = _childWndSelectObj.GetSelectedDepartIds();
            

            //查询以部门分组的班中餐信息
            if (TabSelectedIndex == 2)
            {
                WaitingDialog.ShowWaiting();
                
                GetLunchRecordOnDepartRia(departIds.ToArray(),(result) =>
                {
                    LunchRecordInfoOnDepart = new BaseViewModelCollection<LunchRecordInfoOnDepart>();
                    foreach (var item in result)
                    {
                        this.LunchRecordInfoOnDepart.Add(item);
                    }
                    
                    WaitingDialog.HideWaiting();
                });
            }
            else if (TabSelectedIndex == 1)  //班次班中餐
            {
                WaitingDialog.ShowWaiting();

                GetLunchRecordOnClassOrderRia(classOrderIds.ToArray() ,departIds.ToArray(), (result) =>
                {
                    LunchRecordInfoOnClassOrder = new BaseViewModelCollection<ReportRecordInfoOnDepart_ZhouYuanShan>();
                    foreach (var item in result)
                    {
                        this.LunchRecordInfoOnClassOrder.Add(item);
                    }

                    WaitingDialog.HideWaiting();
                });

            }
            else if (TabSelectedIndex == 0) //个人班中餐
            {
                WaitingDialog.ShowWaiting();

                GetLunchRecordOnPersonRia(personIds.ToArray(),departIds.ToArray(),(result) =>
                {
                    LunchRecordInfoOnPerson = new BaseViewModelCollection<LunchRecordInfoOnPerson>();
                    foreach (var item in result)
                    {
                        this.LunchRecordInfoOnPerson.Add(item);
                    }

                    WaitingDialog.HideWaiting();
                });
            }
           
        }

        /// <summary>
        /// 选中全部item或者取消选中
        /// </summary>
        /// <param name="sender">checkBox对象</param>
        public void SelectAllOnDepart(bool? IsChecked)
        {
            if (IsChecked.Value)
            {
                foreach (var item in LunchRecordInfoOnDepart)
                {
                    item.isSelected = true;
                }
            }
            else
            {
                foreach (var item in LunchRecordInfoOnDepart)
                {
                    item.isSelected = false;
                }
            }

            //this.IsBatchBtnEnable = CheckIsAnyOneSelected();
            this.MarkObjLunchRecordInfoOnDepart.Selected = CheckIsAllSelectedOnDepart();
        }

        /// <summary>
        /// 选中全部item或者取消选中
        /// </summary>
        /// <param name="sender">checkBox对象</param>
        public void SelectAllOnPerson(bool? IsChecked)
        {
            if (IsChecked.Value)
            {
                foreach (var item in LunchRecordInfoOnPerson)
                {
                    item.isSelected = true;
                }
            }
            else
            {
                foreach (var item in LunchRecordInfoOnPerson)
                {
                    item.isSelected = false;
                }
            }

            //this.IsBatchBtnEnable = CheckIsAnyOneSelected();
            this.MarkObjLunchRecordInfoOnPerson.Selected = CheckIsAllSelectedOnPerson();
        }

        /// <summary>
        /// 选中全部item或者取消选中
        /// </summary>
        /// <param name="sender">checkBox对象</param>
        public void SelectAllOnClassOrder(bool? IsChecked)
        {
            if (IsChecked.Value)
            {
                foreach (var item in LunchRecordInfoOnClassOrder)
                {
                    item.isSelected = true;
                }
            }
            else
            {
                foreach (var item in LunchRecordInfoOnClassOrder)
                {
                    item.isSelected = false;
                }
            }

            //this.IsBatchBtnEnable = CheckIsAnyOneSelected();
            this.MarkObjLunchRecordInfoOnClassOrder.Selected = CheckIsAllSelectedOnClassOrder();
        }

       
        /// <summary>
        /// 按需选择datagrid的Items
        /// </summary>
        /// <param name="sender">datagrid对象</param>
        public void SelectItemsOnDepart(LunchRecordInfoOnDepart info)
        {
            info.isSelected = !info.isSelected;
            //this.IsBatchBtnEnable = CheckIsAnyOneSelected();
            this.MarkObjLunchRecordInfoOnDepart.Selected = CheckIsAllSelectedOnDepart();
        }


        /// <summary>
        /// 按需选择datagrid的Items
        /// </summary>
        /// <param name="sender">datagrid对象</param>
        public void SelectItemsOnPerson(LunchRecordInfoOnPerson info)
        {
            info.isSelected = !info.isSelected;
            //this.IsBatchBtnEnable = CheckIsAnyOneSelected();
            this.MarkObjLunchRecordInfoOnPerson.Selected = CheckIsAllSelectedOnPerson();
        }


        /// <summary>
        /// 按需选择datagrid的Items
        /// </summary>
        /// <param name="sender">datagrid对象</param>
        public void SelectItemsOnClassOrder(ReportRecordInfoOnDepart_ZhouYuanShan info)
        {
            info.isSelected = !info.isSelected;
            //this.IsBatchBtnEnable = CheckIsAnyOneSelected();
            this.MarkObjLunchRecordInfoOnClassOrder.Selected = CheckIsAllSelectedOnClassOrder();
        }

      

        #endregion

        #region 私有功能函数

        //获取选择的班次
        private List<int> GetSelectedClassOrder()
        {
            List<int> classOrderIDs = new List<int>();

            foreach (var item in _textCmbClassOrder.comboBox.Items)
            {
                if (item is CheckBox && ((CheckBox)item).IsChecked.Value)
                {
                    classOrderIDs.Add((int)((CheckBox)item).Tag);
                }
            }

            return classOrderIDs;
        }

        /// <summary>
        /// 检查Item是否全部被选中
        /// </summary>
        private bool CheckIsAllSelectedOnDepart()
        {
            if (LunchRecordInfoOnDepart.Count == 0)
            {
                return false;
            }
            foreach (var item in LunchRecordInfoOnDepart)
            {
                if (!item.isSelected)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 检查Item是否全部被选中
        /// </summary>
        private bool CheckIsAllSelectedOnPerson()
        {
            if (LunchRecordInfoOnPerson.Count == 0)
            {
                return false;
            }
            foreach (var item in LunchRecordInfoOnPerson)
            {
                if (!item.isSelected)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 检查Item是否全部被选中
        /// </summary>
        private bool CheckIsAllSelectedOnClassOrder()
        {
            if (LunchRecordInfoOnClassOrder.Count == 0)
            {
                return false;
            }
            foreach (var item in LunchRecordInfoOnClassOrder)
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
        /// 获取班次信息
        /// </summary>
        private void GetClassOrderInfoRia(Action<List<UserClassOrderInfo>> callBack)
        {
            ServiceDomDbAcess.ReOpenSever();

            EntityQuery<UserClassOrderInfo> list = ServiceDomDbAcess.GetSever().GetClassOrderInfosQuery();
            ///回调异常类
            Action<LoadOperation<UserClassOrderInfo>> actionCallBack = ErrorHandle<UserClassOrderInfo>.OnLoadErrorCallBack;
            ///异步事件
            LoadOperation<UserClassOrderInfo> lo = ServiceDomDbAcess.GetSever().Load(list, actionCallBack, null);

            lo.Completed += delegate
            {
                List<UserClassOrderInfo> res = new List<UserClassOrderInfo>();
                //异步获取数据
                foreach (UserClassOrderInfo ar in lo.Entities)
                {
                    res.Add(ar);
                }

                if (callBack != null)
                {
                    callBack(res);
                }
            };
        }

        
        /// <summary>
        /// 向后台发送命令，获取以部门分组的班中餐记录
        /// </summary>
        /// <param name="riaOperateCallBack">异步操作完成后的回调函数</param>
        private void GetLunchRecordOnDepartRia(int[] departIds, Action<IEnumerable<LunchRecordInfoOnDepart>> riaOperateCallBack)
        {
            try
            {
                ServiceDomDbAcess.ReOpenSever();
                EntityQuery<LunchRecordInfoOnDepart> list = ServiceDomDbAcess.GetSever().GetLunchRecordOnDepartQuery(BeginTime, EndTime, departIds);
                ///回调异常类
                Action<LoadOperation<LunchRecordInfoOnDepart>> actionCallBack = ErrorHandle<LunchRecordInfoOnDepart>.OnLoadErrorCallBack;
                ///异步事件
                LoadOperation<LunchRecordInfoOnDepart> lo = ServiceDomDbAcess.GetSever().Load(list, actionCallBack, null);

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
        /// 向后台发送命令，获取班中餐记录详情
        /// </summary>
        /// <param name="riaOperateCallBack">异步操作完成后的回调函数</param>
        private void GetLunchRecordOnPersonRia(int[] personIds,int[] departIds, Action<IEnumerable<LunchRecordInfoOnPerson>> riaOperateCallBack)
        {
            try
            {
                ServiceDomDbAcess.ReOpenSever();
                EntityQuery<LunchRecordInfoOnPerson> list = ServiceDomDbAcess.GetSever().GetLunchRecordOnPersonQuery(BeginTime, EndTime, personIds, departIds);
                ///回调异常类
                Action<LoadOperation<LunchRecordInfoOnPerson>> actionCallBack = ErrorHandle<LunchRecordInfoOnPerson>.OnLoadErrorCallBack;
                ///异步事件
                LoadOperation<LunchRecordInfoOnPerson> lo = ServiceDomDbAcess.GetSever().Load(list, actionCallBack, null);

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
        /// 向后台发送命令，获取班次班中餐记录
        /// </summary>
        /// <param name="riaOperateCallBack">异步操作完成后的回调函数</param>
        private void GetLunchRecordOnClassOrderRia(int[] classOrderIds,int[] departIds,Action<IEnumerable<ReportRecordInfoOnDepart_ZhouYuanShan>> riaOperateCallBack)
        {
            try
            {
                ServiceDomDbAcess.ReOpenSever();
                EntityQuery<ReportRecordInfoOnDepart_ZhouYuanShan> list = ServiceDomDbAcess.GetSever().GetLunchRecordOnClassOrderQuery(BeginTime, EndTime, classOrderIds, departIds);
                ///回调异常类
                Action<LoadOperation<ReportRecordInfoOnDepart_ZhouYuanShan>> actionCallBack = ErrorHandle<ReportRecordInfoOnDepart_ZhouYuanShan>.OnLoadErrorCallBack;
                ///异步事件
                LoadOperation<ReportRecordInfoOnDepart_ZhouYuanShan> lo = ServiceDomDbAcess.GetSever().Load(list, actionCallBack, null);

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


      
        #endregion

     
    }

 

}
