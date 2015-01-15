/*************************************************************************
** 文件名:   VmInWellPerson.cs
** 主要类:   VmInWellPerson
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   lzc
** 日  期:   2013-4-11
** 修改人:   
** 日  期:
** 描  述:   VmInWellPerson，主要是当前井下人员管理
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
using IriskingAttend.Common;
using System.Collections.Generic;
using IriskingAttend.Dialog;
using GalaSoft.MvvmLight.Command;
using MvvmLightCommand.SL4.TriggerActions;
using IriskingAttend.View;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using IriskingAttend.ViewModel.SystemViewModel;
using IriskingAttend.AsyncControl;

namespace IriskingAttend.ViewModel.SafeManager
{
    public class VmInWellPerson : BaseViewModel
    {
        #region  私有数据
        //选择当前超时人员
        private UserInWellPerson _selectOverInWellPerson = null;
        //是否选择
        private MarkObject _selectAll = new MarkObject();

        //界面按钮是否有效
        private bool _isEnable = false;

        //是否为全选事件
        private bool tag = true;

        /// <summary>
        /// 数据基源 井下超时人员
        /// </summary>
        private BaseViewModelCollection<UserInWellPerson> _inWellPersonOverModelBase { get; set; }

        /// <summary>
        /// 数据基源--所有井下人员
        /// </summary>
        private BaseViewModelCollection<UserInWellPerson> _inWellPersonModelBase { get; set; }

        //设置查询条件
        private string _currentDepartName = "全部";
        private DevGroup _currentDevGroup = DevGroup.InOutWell;

        /// <summary>
        /// 每次批量添加、删除的记录条数，根据经验可以修改
        /// </summary>
        private const int _recordNumber = 20;

        private string _logDescrip = "";

        /// <summary>
        /// 异步工作列队
        /// </summary>
        private AsyncActionRunner _runnerTask = null;

        private int _asyncRuslt;   //异步任务的操作返回信息

        #endregion

        #region 数据源

        /// <summary>
        /// 绑定数据源 当前井下人员
        /// </summary>
        public BaseViewModelCollection<UserInWellPerson> InWellPersonModel { get; set; }


        /// <summary>
        /// 绑定数据源 井下超时人员
        /// </summary>
        public BaseViewModelCollection<UserInWellPerson> InWellPersonOverModel { get; set; }


        /// <summary>
        /// 是否全选
        /// </summary>
        public MarkObject SelectAll
        {
            get
            {
                return _selectAll;
            }
            set
            {
                if (value != _selectAll)
                {
                    _selectAll = value;
                    OnPropertyChanged(() => SelectAll);
                }
            }
        }

        /// <summary>
        /// 界面按钮是否有效
        /// </summary>
        public bool IsEnable
        {
            get
            {
                return _isEnable;
            }
            set
            {
                if(value != _isEnable)
                {
                    _isEnable = value;
                    OnPropertyChanged(() => IsEnable);
                }
            }
        }

        /// <summary>
        /// 绑定当前选择的井下超时人员信息
        /// </summary>
        public UserInWellPerson SelectOverInWellPerson
        {
            get
            {
                return _selectOverInWellPerson;
            }
            set
            {
                if (value != _selectOverInWellPerson)
                {

                    _selectOverInWellPerson = value;
                    OnPropertyChanged(() => SelectOverInWellPerson);
                }
            }
        }

        /// <summary>
        /// 进度条数据绑定
        /// </summary>
        public BarBindObject BarBind { get; set; }

        private string _labNoteContent;
        /// <summary>
        ///  取消批量添加、删除说明label绑定
        /// </summary>
        public string LabNoteContent  
        {
            get
            {
                return _labNoteContent;
            }
            set
            {
                if (value != _labNoteContent)
                {
                    _labNoteContent = value;
                    OnPropertyChanged(() => LabNoteContent);
                }
            }
        }

        private string _txtCancelText;
        /// <summary>
        /// 取消批量添加、删除按钮text绑定
        /// </summary>
        public string TxtCancelText
        {
            get
            {
                return _txtCancelText;
            }
            set
            {
                if (value != _txtCancelText)
                {
                    _txtCancelText = value;
                    OnPropertyChanged(() => TxtCancelText);
                }
            }
        }
        #endregion

        #region 构造函数

        public VmInWellPerson()
        {
            //初始化
            InWellPersonModel = new BaseViewModelCollection<UserInWellPerson>();
            InWellPersonOverModel = new BaseViewModelCollection<UserInWellPerson>();
            _inWellPersonModelBase = new BaseViewModelCollection<UserInWellPerson>();
            _inWellPersonOverModelBase = new BaseViewModelCollection<UserInWellPerson>();
            BarBind = new BarBindObject();            
        }

        #endregion


        #region Command绑定
       
        //显示超时人员
        private DelegateCommand _showOverInWellDialogCmd = null;
        //批量删除
        private DelegateCommand _batchDeleteSelectedCmd = null;
        //批量补加
        private DelegateCommand _batchAddRecogSelectedCmd = null;

        private DelegateCommand _cancelBatchCmd = null;

        /// <summary>
        /// 显示批量处理井下超时人员命令
        /// </summary>
        public DelegateCommand ShowOverInWellDialogCmd
        {
            get
            {
                if (_showOverInWellDialogCmd == null)
                {
                    //_showOverInWellDialogCmd = new DelegateCommand(ShowOverInWellDialog);
                }
                return _showOverInWellDialogCmd;
            }
        }

        /// <summary>
        /// //批量删除命令
        /// </summary>
        public DelegateCommand BatchDeleteSelectedCmd
        {
            get
            {
                if (_batchDeleteSelectedCmd == null)
                {
                    _batchDeleteSelectedCmd = new DelegateCommand(BatchDeleteSelected);
                }
                return _batchDeleteSelectedCmd;
            }
        }

        /// <summary>
        /// //批量删除命令
        /// </summary>
        public DelegateCommand BatchAddRecogSelectedCmd
        {
            get
            {
                if (_batchAddRecogSelectedCmd == null)
                {
                    _batchAddRecogSelectedCmd = new DelegateCommand(BatchAddRecogSelected);
                }
                return _batchAddRecogSelectedCmd;
            }
        }

        /// <summary>
        /// //批量删除命令
        /// </summary>
        public DelegateCommand CancelBatchCmd
        {
            get
            {
                _cancelBatchCmd = new DelegateCommand(StopBatchRecogAction);                
                return _cancelBatchCmd;
            }           
        }

        #endregion

        #region 函数

        /// <summary>
        /// 异步获取当前在岗人员信息
        /// </summary>
        public void GetInWellPerson()
        {
            EntityQuery<UserInWellPerson> lstInWellPerson = ServiceDomDbAcess.GetSever().IrisGetInWellPersonForDepartQuery(VmLogin.OperatorDepartIDList);

            ///回调异常类
            Action<LoadOperation<UserInWellPerson>> getInWellPersonCallBack = new Action<LoadOperation<UserInWellPerson>>(ErrorHandle<UserInWellPerson>.OnLoadErrorCallBack);
            ///异步事件
            LoadOperation<UserInWellPerson> lo = ServiceDomDbAcess.GetSever().Load(lstInWellPerson, getInWellPersonCallBack, null);

            lo.Completed += (o, e) =>
                {
                    WaitingDialog.HideWaiting();
                    _inWellPersonModelBase.Clear();
                    _inWellPersonOverModelBase.Clear();
                    try
                    {
                        //异步获取数据
                        foreach (UserInWellPerson ar in lo.Entities.OrderByDescending(a => a.work_time, new SpecialComparer()))
                        {
                           _inWellPersonModelBase.Add(ar);
                            if (ar.work_state != "正常")
                            {
                                //添加超时人员
                                _inWellPersonOverModelBase.Add(ar);
                            }
                        } 
                        SelectionChanged(_currentDepartName, _currentDevGroup);
                    }
                    catch (Exception err)
                    {
                        ChildWindow errorWin = new ErrorWindow("加载异常", err.Message);
                        errorWin.Show();
                    }
                };
            WaitingDialog.ShowWaiting();
        }

        /// <summary>
        /// 全选事件
        /// </summary>
        /// <param name="o"></param>
        /// <param name="arg"></param>
        private void SelectAll_PropertyChanged(object o, EventArgs arg)
        {

            if (SelectAll.Selected)
            {
                foreach (var ar in InWellPersonOverModel)
                {
                    ar.is_select = true;
                }
                IsEnable = true;
                tag = true;
            }
            else
            {
                if (tag)
                {
                    foreach (var ar in InWellPersonOverModel)
                    {
                        ar.is_select = false;
                    }
                    IsEnable = false;
                }
                else
                {
                    tag = true;
                }
            }

        }

        /// <summary>
        /// 处理（伪）单击checkbox事件
        /// </summary>
        /// <param name="ei"></param>
        public void OverInWellMouseLeftButtonUpArgs(  UserInWellPerson personInfo)
        {
            try
            {
                personInfo.is_select = !personInfo.is_select;
                tag = false;

                if (personInfo.is_select &&
                 InWellPersonOverModel.Where(a => a.is_select == false).Count() == 0)
                {
                    SelectAll.Selected = true;
                }
                else
                {
                    SelectAll.Selected = false;
                }

                if (InWellPersonOverModel.Where(a => a.is_select == true).Count() == 0)
                {
                    IsEnable = false;
                }
                else
                {
                    IsEnable = true;
                }

            }
            catch (Exception err)
            {
                ErrorWindow errWin = new ErrorWindow(err);
                errWin.Show();
            }
        }

        /// <summary>
        /// 显示批量处理井下超时人员信息界面
        /// </summary>
        public void ShowOverInWellDialog()
        {
            OverInWellDialog overInWellDialog = new OverInWellDialog();
            SelectAll = overInWellDialog.Resources["MarkObject"] as MarkObject;
            overInWellDialog.DataContext = this;
            SelectAll.SelectedChanged += SelectAll_PropertyChanged;
            overInWellDialog.Show();
        }

        /// <summary>
        /// 选择的部门改变触发函数
        /// </summary>
        /// <param name="departName">部门名称</param>
        public void SelectionChanged(string departName, DevGroup devGroup)
        {
            _currentDepartName = departName;
            _currentDevGroup = devGroup;

            InWellPersonModel.Clear();
            InWellPersonOverModel.Clear();
            if (departName != "全部")
            {
                SelectionDevGroupChanged(departName,devGroup);
            }
            else
            {
                SelectionDevGroupChanged(devGroup);
            }
        }

        //排序模式
        public enum DevGroup
        {
            InWell ,
            OutWell ,
            InOutWell
        }

        /// <summary>
        /// 选择的部门改变触发函数
        /// </summary>
        /// <param name="departName">部门名称</param>
        public void SelectionDevGroupChanged(string departName,DevGroup devGroup)
        {
            if (DevGroup.InWell == devGroup)
            {
                foreach (var item in _inWellPersonModelBase.Where(a => a.dev_group_int == 3 && a.depart_name == departName))
                {
                    InWellPersonModel.Add(item);
                }

                foreach (var item in _inWellPersonOverModelBase.Where(a => a.dev_group_int == 3 && a.depart_name == departName))
                {
                    InWellPersonOverModel.Add(item);
                }
            }
            else if (DevGroup.OutWell == devGroup)
            {
                foreach (var item in _inWellPersonModelBase.Where(a => a.dev_group_int == 6 && a.depart_name == departName))
                {
                    InWellPersonModel.Add(item);
                }

                foreach (var item in _inWellPersonOverModelBase.Where(a => a.dev_group_int == 6 && a.depart_name == departName))
                {
                    InWellPersonOverModel.Add(item);
                }
            }
            else
            {
                foreach (var ar in _inWellPersonModelBase.Where(a=> a.depart_name == departName))
                {
                    InWellPersonModel.Add(ar);
                }

                foreach (var ar in _inWellPersonOverModelBase.Where(a => a.depart_name == departName))
                {
                    InWellPersonOverModel.Add(ar);
                }
            }
        }

        /// <summary>
        /// 选择的部门改变触发函数
        /// </summary>
        /// <param name="departName">部门名称</param>
        public void SelectionDevGroupChanged( DevGroup devGroup)
        {
            if (DevGroup.InWell == devGroup)
            {
                foreach (var item in _inWellPersonModelBase.Where(a => a.dev_group_int == 3))
                {
                    InWellPersonModel.Add(item);
                }
               
                foreach (var item in _inWellPersonOverModelBase.Where(a => a.dev_group_int == 3))
                {
                    InWellPersonOverModel.Add(item);
                }
            }
            else if (DevGroup.OutWell == devGroup)
            {
                foreach (var item in _inWellPersonModelBase.Where(a => a.dev_group_int == 6))
                {
                    InWellPersonModel.Add(item);
                }

                foreach (var item in _inWellPersonOverModelBase.Where(a => a.dev_group_int == 6))
                {
                    InWellPersonOverModel.Add(item);
                }
            }
            else
            {
                foreach (var ar in _inWellPersonModelBase)
                {
                    InWellPersonModel.Add(ar);
                }

                foreach (var ar in _inWellPersonOverModelBase)
                {
                    InWellPersonOverModel.Add(ar);
                }
            }
        }
        /// <summary>
        /// 批量删除当前井下超时人员信息
        /// </summary>
        private void DeleteSelected()
        {
            List<int> overInWellIds = new List<int>();
            _logDescrip = SetBatchInWellIds(overInWellIds);

            //绑定数据的状态
            BarBind.BarVisibility = Visibility.Visible;
            BarBind.BarMaximun = Math.Ceiling(overInWellIds.Count() / _recordNumber);
            BarBind.BarValue = 0;

            TxtCancelText = "取消批量删除识别记录";
            LabNoteContent = "批量删除识别记录中：";

            IsEnable = false;

            if (_runnerTask != null)
            {
                _runnerTask.Stop = false;
                _runnerTask = null;
            }

            _runnerTask = new AsyncActionRunner(SetBatchDeleteRecogTask(overInWellIds.ToArray()));
            //下面是运行完成后的处理
            _runnerTask.Completed += (obj, a) =>
            {
                BarBind.BarVisibility = Visibility.Collapsed;
                SelectAll_PropertyChanged(null, null);

                if (_runnerTask == null)
                {
                    return;
                }

                //如果取消了批量添加识别记录的操作，则不弹出对话框和不写入日志
                if (!_runnerTask.Stop)
                {
                    GetInWellPerson();
                    _runnerTask = null;
                    return;
                }
                
                _runnerTask.Stop = false;
                _runnerTask = null;                

                if (_asyncRuslt == 0)
                {
                    //添加操作员日志
                    VmOperatorLog.InsertOperatorLog(1, "批量删除超时人员识别记录", _logDescrip, () =>
                    {
                        //删除成功！处理函数
                        MsgBoxWindow.MsgBox("删除成功！",
                                                MsgBoxWindow.MsgIcon.Succeed, MsgBoxWindow.MsgBtns.OK);
                        GetInWellPerson();
                    });

                }
                else
                {
                    //添加操作员日志
                    VmOperatorLog.InsertOperatorLog(0, "批量删除超时人员识别记录", _logDescrip, () =>
                    {
                        //删除失败！处理函数
                        MsgBoxWindow.MsgBox("删除失败！",
                                                MsgBoxWindow.MsgIcon.Error, MsgBoxWindow.MsgBtns.OK);
                        GetInWellPerson();
                    });
                }
            };

            //执行异步通信
            _runnerTask.Execute();              
        }

        /// <summary>
        /// 设置任务链表
        /// </summary>
        /// <param name="idsSum"> 需要重构人员ID数组</param>
        /// <param name="personCount">需要重构人员个数</param>
        /// <returns>任务链表</returns>
        private List<IAsyncAction> SetBatchDeleteRecogTask(int[] personIds)
        {
            int personSum = personIds.Count();
            int personCount = _recordNumber;

            List<IAsyncAction> lstBatchDeleteRecogTask = new List<IAsyncAction>();

            ///设置需要重构的人员ID
            for (int totalIndex = 0; totalIndex < personSum; totalIndex += personCount)
            {
                int[] ids;                
                if (totalIndex + personCount < personSum)
                {
                    ids = new int[personCount];                   
                    for (int index = 0; index < personCount; index++)
                    {
                        ids[index] = personIds[totalIndex + index];                       
                    }
                }
                else
                {
                    ids = new int[personSum - totalIndex];                    
                    for (int index = 0; index < personSum - totalIndex; index++)
                    {
                        ids[index] = personIds[totalIndex + index];                        
                    }
                }

                ///创建新任务
                var taskTemp = new AsyncAction("ManagerDeleteRecog" + totalIndex);

                ///任务回调
                taskTemp.SetAction(() =>
                {
                    BarBind.BarValue++;
                    Action<InvokeOperation<int>> deleteRecogCallBack = CallBackHandleControl<int>.OnInvokeErrorCallBack;

                    CallBackHandleControl<int>.m_sendValue = (o) =>
                    {
                        _asyncRuslt = o;
                        //表示当前任务完成了,执行下一个任务
                        taskTemp.OnCompleted();
                    };

                    ServiceDomDbAcess.GetSever().IrisDeleteUserPersonRecogLogsForIDS(ids, deleteRecogCallBack, null);

                }, true);

                ///添加任务
                lstBatchDeleteRecogTask.Add(taskTemp);
            }
            return lstBatchDeleteRecogTask;
        }
        /// <summary>
        /// 确认是否删除
        /// </summary>
        private void BatchDeleteSelected()
        {
            MsgBoxWindow.MsgBox("是否批量删除当前选择的识别记录！",
                                          MsgBoxWindow.MsgIcon.Warning, MsgBoxWindow.MsgBtns.OKCancel, (result) =>
                {
                    if (result == MsgBoxWindow.MsgResult.OK)
                    {
                        DeleteSelected();
                    }
                });
        }


        /// <summary>
        /// 批量增加数据 是当前井下超时人员记录生成一条正常的考勤记录
        /// </summary>
        private void BatchAddRecogSelected()
        {
            List<int> overInWellIds = new List<int>();
            _logDescrip = SetBatchInWellIds(overInWellIds);
            
            int[] personIds = new int[InWellPersonOverModel.Where(a => a.is_select).Count()];
            DateTime[] dts = new DateTime[InWellPersonOverModel.Where(a => a.is_select).Count()];
            int[] devTypes = new int[InWellPersonOverModel.Where(a => a.is_select).Count()];//int index =0;index < InWellPersonOverModel.Where(a => a.is_select).Count();index ++)//
            int index=0;
            foreach (var ar in InWellPersonOverModel.Where(a => a.is_select))
            {
                personIds[index] = ar.person_id;
                dts[index] = ar.in_time;
                devTypes[index] = ar.dev_group_int - 2;//预留
                index++;
            }

            //绑定数据的状态
            BarBind.BarVisibility = Visibility.Visible;
            BarBind.BarMaximun = Math.Ceiling(personIds.Count() / _recordNumber);
            BarBind.BarValue = 0;
            
            TxtCancelText = "取消批量添加识别记录";
            LabNoteContent = "批量添加识别记录中：";

            IsEnable = false;

            if (_runnerTask != null)
            {
                _runnerTask.Stop = false;
                _runnerTask = null;
            }

            _runnerTask = new AsyncActionRunner(SetBatchAddRecogTask(personIds, dts, devTypes));
            //下面是运行完成后的处理
            _runnerTask.Completed += (obj, a) =>
            {                
                SelectAll_PropertyChanged(null, null);
                BarBind.BarVisibility = Visibility.Collapsed;

                if (_runnerTask == null)
                {
                    return;
                }

                //如果取消了批量添加识别记录的操作，则不弹出对话框和不写入日志
                if (!_runnerTask.Stop)
                {
                    GetInWellPerson();
                    _runnerTask = null;
                    return;
                }
                
                _runnerTask.Stop = false;
                _runnerTask = null;                

                if (_asyncRuslt == 0)
                {
                    //添加操作员日志
                    VmOperatorLog.InsertOperatorLog(1, "批量添加超时人员识别记录", _logDescrip, () =>
                    {
                        //添加成功！处理函数
                        MsgBoxWindow.MsgBox("添加成功！",
                                                MsgBoxWindow.MsgIcon.Succeed, MsgBoxWindow.MsgBtns.OK);
                        GetInWellPerson();
                    });

                }
                else
                {
                    //添加操作员日志
                    VmOperatorLog.InsertOperatorLog(0, "批量添加超时人员识别记录", _logDescrip, () =>
                    {
                        //添加失败！处理函数
                        MsgBoxWindow.MsgBox("添加失败！",
                                                MsgBoxWindow.MsgIcon.Error, MsgBoxWindow.MsgBtns.OK);
                        GetInWellPerson();
                    });
                }
                
            };

            //执行异步通信
            _runnerTask.Execute();                    
        }

        /// <summary>
        /// 设置任务链表
        /// </summary>
        /// <param name="idsSum"> 需要重构人员ID数组</param>
        /// <param name="personCount">需要重构人员个数</param>
        /// <returns>任务链表</returns>
        private List<IAsyncAction> SetBatchAddRecogTask(int[] personIds, DateTime[] rocordDts, int[] devTypes)
        {
            int personSum = personIds.Count();
            int personCount = _recordNumber;

            List<IAsyncAction> lstBatchAddRecogTask = new List<IAsyncAction>();

            ///设置需要重构的人员ID
            for (int totalIndex = 0; totalIndex < personSum; totalIndex += personCount)
            {
                int[] ids;
                DateTime[] dts;
                int[] types;
                if (totalIndex + personCount < personSum)
                {
                    ids = new int[personCount];
                    dts = new DateTime[personCount];
                    types = new int[personCount];
                    for (int index = 0; index < personCount; index++)
                    {
                        ids[index] = personIds[totalIndex + index];
                        dts[index] = rocordDts[totalIndex + index];
                        types[index] = devTypes[totalIndex + index];
                    }
                }
                else
                {
                    ids = new int[personSum - totalIndex];
                    dts = new DateTime[personSum - totalIndex];
                    types = new int[personSum - totalIndex];
                    for (int index = 0; index < personSum - totalIndex; index++)
                    {
                        ids[index] = personIds[totalIndex + index];
                        dts[index] = rocordDts[totalIndex + index];
                        types[index] = devTypes[totalIndex + index];
                    }
                }

                ///创建新任务
                var taskTemp = new AsyncAction("ManagerAddRecog" + totalIndex);

                ///任务回调
                taskTemp.SetAction(() =>
                {
                    BarBind.BarValue++;
                    Action<InvokeOperation<int>> addRecogCallBack = CallBackHandleControl<int>.OnInvokeErrorCallBack;

                    CallBackHandleControl<int>.m_sendValue = (o) =>
                    {
                        _asyncRuslt = o;
                        //表示当前任务完成了,执行下一个任务
                        taskTemp.OnCompleted();
                    };
                    string remarks = string.Format("操作员{0}添加", VmLogin.GetUserName());
                    ServiceDomDbAcess.GetSever().IrisInsertUserPersonRecogLogForOverInWell(ids, dts, types, remarks, addRecogCallBack, null);
                    
                }, true);

                ///添加任务
                lstBatchAddRecogTask.Add(taskTemp);
            }
            return lstBatchAddRecogTask;
        }

        /// <summary>
        /// 停止批量添加、删除识别记录任务
        /// </summary>
        public void StopBatchRecogAction()
        {
            MsgBoxWindow.MsgBox("是否" + TxtCancelText + "?",
                    MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK,
                    (isOK) =>
                    {
                        if (isOK == MsgBoxWindow.MsgResult.OK)
                        {
                            CancelBatchRecog();
                        }
                    });
        }

        /// <summary>
        /// 停止批量添加、删除识别记录
        /// </summary>
        public void CancelBatchRecog()
        {
            if (_runnerTask != null && _runnerTask.Stop)
            {
                _runnerTask.Stop = false;

                BarBind.BarVisibility = Visibility.Collapsed;

                string log ="";
                log = TxtCancelText.Insert( 6, "超时人员");

                //添加操作员日志
                VmOperatorLog.InsertOperatorLog(1, log, _logDescrip, null);
            }
        }

        /// <summary>
        /// 筛选多选的记录信息
        /// </summary>
        /// <param name="lstIds"></param>
        private string SetBatchInWellIds(List<int> lstIds)
        {
            string logDescrip = "";
            int index = 1;
            foreach (var ar in this.InWellPersonOverModel)
            {
                if (ar.is_select && ar.in_recog_id > 0)
                {
                    lstIds.Add(ar.in_recog_id);
                    logDescrip += string.Format("{0}.姓名：{1}，工号：{2}，部门名称：{3}，上岗时间：{4}，工作时间：{5}；\r\n",index++, ar.work_sn, ar.person_name, ar.depart_name, ar.in_time, ar.work_time);
                }
            }
            return logDescrip;
        }
        #endregion

    }
}
