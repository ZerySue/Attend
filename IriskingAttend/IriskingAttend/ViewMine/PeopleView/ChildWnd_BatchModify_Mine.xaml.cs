/*************************************************************************
** 文件名:   ChildWnd_BatchModify.cs
×× 主要类:   ChildWnd_BatchModify
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   cty
** 日  期:   2013-6-14
** 修改人:   wz 代码优化
** 日  期:   2013-7-23
** 描  述:   ChildWnd_BatchModify类,批量修改人员属性页面
**
** 版  本:   1.0.0
** 备  注:  命名及代码编写遵守C#编码规范
**
 * ***********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using IriskingAttend.ViewModel;
using Irisking.Web.DataModel;
using System.ServiceModel.DomainServices.Client;
using IriskingAttend.Web;
using EDatabaseError;
using IriskingAttend.Common;
using IriskingAttend.ViewModelMine.PeopleViewModel;
using System.Collections;
using System.Text;
using IriskingAttend.ViewModel.SystemViewModel;
using IriskingAttend.AsyncControl;

namespace IriskingAttend.ViewMine.PeopleView
{
    /// <summary>
    /// 批量修改人员属性UI后台
    /// </summary>
    public partial class ChildWnd_BatchModify_Mine : ChildWindow
    {
        #region 字段声明

        /// <summary>
        /// 异步任务执行器
        /// </summary>
        private AsyncActionRunner _runnerTask;

        /// <summary>
        /// 与服务声明
        /// </summary>
        private DomainServiceIriskingAttend _serviceDomDbAccess = new DomainServiceIriskingAttend();

        /// <summary>
        /// 被选中的人员
        /// </summary>
        private BaseViewModelCollection<UserPersonInfo> _selectedUserPersonInfos;
        
        //调离目标部门列表
        private List<string> _departNames;
        private List<int> _departIDs;

        //调离班制列表
        private List<string> classTypeNames;
        private List<int> classTypeIDs;

        //调离职务列表
        private List<PrincipalInfo> principalInfos = new List<PrincipalInfo>();

        //调离工种列表
        private List<WorkTypeInfo> workTypeInfos = new List<WorkTypeInfo>();

      

        /// <summary>
        /// 窗口关闭回调函数
        /// </summary>
        private Action<bool?> callback;

        #endregion

        #region 构造函数
       
        /// <summary>
        /// 批量修改人员属性构造函数
        /// </summary>
        /// <param name="_UserPersonInfos_Selected">被选中人员集合</param>
        /// <param name="_callback">回调函数</param>
        public ChildWnd_BatchModify_Mine(BaseViewModelCollection<UserPersonInfo> _UserPersonInfos_Selected, Action<bool?> _callback)
        {
            callback = _callback;
            _selectedUserPersonInfos = _UserPersonInfos_Selected;
            InitializeComponent();
            //向后台查询部门信息
            GetDepartNames((departs) =>
                {
                    _departNames = new List<string>();
                    _departIDs = new List<int>();
                    _departNames.Add("保存原状");
                    _departIDs.Add(-1);

                    //异步获取数据
                    foreach (UserDepartInfo ar in departs)
                    {
                        _departNames.Add(ar.depart_name);
                        _departIDs.Add(ar.depart_id);

                    }
                    if (_departNames.Count > 0)
                    {
                        this.comb_TargetDepart.ItemsSource = _departNames;
                        this.comb_TargetDepart.SelectedIndex = 0;
                    }

                    //查询班制信息
                    GetClassTypeNamesRia((classTypes) =>
                        {
                            classTypeNames = new List<string>();
                            classTypeIDs = new List<int>();
                            classTypeNames.Add("保存原状");
                            classTypeIDs.Add(-1);

                            //异步获取数据
                            foreach (UserClassTypeInfo ar in classTypes)
                            {
                                classTypeNames.Add(ar.class_type_name);
                                classTypeIDs.Add(ar.class_type_id);
                            }
                            if (classTypeNames.Count > 0)
                            {
                                this.comb_TargetClassTypeOnGround.ItemsSource = classTypeNames;
                                this.comb_TargetClassTypeOnGround.SelectedIndex = 0;
                                this.comb_TargetClassTypeOnMine.ItemsSource = classTypeNames;
                                this.comb_TargetClassTypeOnMine.SelectedIndex = 0;
                            }

                            GetPricipalsRia((principals) =>
                                {
                                    principalInfos.Clear();
                                    principalInfos.Add(new PrincipalInfo()
                                        {
                                            principal_id = 0,
                                            principal_name = "保留原状",
                                        });
                                    foreach (var item in principals)
	                                {
                                        this.principalInfos.Add(item);
	                                }

                                    this.combTargetPrincipal.ItemsSource = principalInfos;
                                    this.combTargetPrincipal.DisplayMemberPath = "principal_name";
                                    this.combTargetPrincipal.SelectedIndex = 0;

                                    GetWorkTypesRia((workTypes) =>
                                        {
                                            workTypeInfos.Clear();
                                            workTypeInfos.Add(new WorkTypeInfo()
                                                {
                                                    work_type_id=0,
                                                    work_type_name = "保留原状",
                                                });
                                            foreach (var item in workTypes)
                                            {
                                                workTypeInfos.Add(item);
                                            }
                                            this.combTargetWorkType.ItemsSource = workTypeInfos;
                                            this.combTargetWorkType.DisplayMemberPath = "work_type_name";
                                            this.combTargetWorkType.SelectedIndex = 0;

                                        });
                                   
                                });
                        });
                });




            dgSelectedPerson.ItemsSource = _selectedUserPersonInfos;
            this.Closed += new EventHandler(ChildWnd_BatchTransfer_Closed);

            //使当前被选择的人员不能被操作
            this.dgSelectedPerson.LoadingRow += new EventHandler<DataGridRowEventArgs>((sender, e) =>
            {
                e.Row.IsHitTestVisible = false;
            });
            this.Dispatcher.BeginInvoke(() =>
            {
                dgSelectedPerson.CurrentColumn = dgSelectedPerson.Columns[dgSelectedPerson.Columns.Count - 1];

            });

        }

        #endregion

        #region 界面事件响应
       
        void ChildWnd_BatchTransfer_Closed(object sender, EventArgs e)
        {
            if (callback != null) callback(this.DialogResult);
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            BatchModifyRia();
            
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        #endregion

        #region Ria操作

        /// <summary>
        /// 获取部门名称
        /// </summary>
        private void GetDepartNames(Action<IEnumerable> riaCallBack)
        {
            try
            {
                EntityQuery<UserDepartInfo> list = _serviceDomDbAccess.GetDepartsInfoQuery();
                ///回调异常类
                Action<LoadOperation<UserDepartInfo>> actionCallBack = ErrorHandle<UserDepartInfo>.OnLoadErrorCallBack;
                ///异步事件
                LoadOperation<UserDepartInfo> lo = this._serviceDomDbAccess.Load(list, actionCallBack, null);
                lo.Completed += delegate
                {
                    if (riaCallBack != null)
                    {
                        riaCallBack(lo.Entities);
                    }
                   
                };
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }

        /// <summary>
        /// 获取班制名称
        /// </summary>
        private void GetClassTypeNamesRia(Action<IEnumerable> riaCallBack)
        {
            try
            {
                EntityQuery<UserClassTypeInfo> list = _serviceDomDbAccess.GetClassTypeInfosQuery();
                ///回调异常类
                Action<LoadOperation<UserClassTypeInfo>> actionCallBack = new Action<LoadOperation<UserClassTypeInfo>>(ErrorHandle<UserClassTypeInfo>.OnLoadErrorCallBack);
                ///异步事件
                LoadOperation<UserClassTypeInfo> lo = this._serviceDomDbAccess.Load(list, actionCallBack, null);
                lo.Completed += (s,e)=>
                {
                    if (riaCallBack != null)
                    {
                        riaCallBack(lo.Entities);
                    }
                   
                };
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }

        /// <summary>
        /// 向后台发送命令，获取职务列表
        /// </summary>
        /// <param name="riaOperateCallBack">异步操作完成后的回调函数</param>
        private void GetPricipalsRia(Action<IEnumerable<PrincipalInfo>> riaOperateCallBack)
        {
            try
            {

                EntityQuery<PrincipalInfo> list = _serviceDomDbAccess.GetPrincipalsQuery();
                ///回调异常类
                Action<LoadOperation<PrincipalInfo>> actionCallBack = ErrorHandle<PrincipalInfo>.OnLoadErrorCallBack;
                ///异步事件
                LoadOperation<PrincipalInfo> lo = this._serviceDomDbAccess.Load(list, actionCallBack, null);

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
        /// 向后台发送命令，获取工种列表
        /// </summary>
        /// <param name="riaOperateCallBack">异步操作完成后的回调函数</param>
        private void GetWorkTypesRia(Action<IEnumerable<WorkTypeInfo>> riaOperateCallBack)
        {
            try
            {

                EntityQuery<WorkTypeInfo> list = _serviceDomDbAccess.GetWorkTypesQuery();
                ///回调异常类
                Action<LoadOperation<WorkTypeInfo>> actionCallBack = ErrorHandle<WorkTypeInfo>.OnLoadErrorCallBack;
                ///异步事件
                LoadOperation<WorkTypeInfo> lo = this._serviceDomDbAccess.Load(list, actionCallBack, null);

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
        /// 批量修改人员属性
        /// </summary>
        private void BatchModifyRia()
        {
            #region 获取描述和组装数据
           
            //获取描述
            StringBuilder description = new StringBuilder();

            List<int> selectedPersonIds = new List<int>();
            foreach (var item in _selectedUserPersonInfos)
            {
                 selectedPersonIds.Add(item.person_id);
                 description.Append(string.Format("姓名：{0}，工号：{1}；",item.person_name,item.work_sn));
            }
            
            description.Append("\r\n");

            int departId=-1 ;
            if (_departIDs.Count > comb_TargetDepart.SelectedIndex && comb_TargetDepart.SelectedIndex >= 0)
            {
                departId = _departIDs[comb_TargetDepart.SelectedIndex];
            }
            
           

            int classTypeIdOnMine=-1 ;
            if (classTypeIDs.Count > comb_TargetClassTypeOnMine.SelectedIndex && comb_TargetClassTypeOnMine.SelectedIndex >= 0)
            {
                classTypeIdOnMine = classTypeIDs[comb_TargetClassTypeOnMine.SelectedIndex];
            }

            int classTypeIdOnGround = -1;
            if (classTypeIDs.Count > comb_TargetClassTypeOnGround.SelectedIndex && comb_TargetClassTypeOnGround.SelectedIndex >= 0)
            {
                classTypeIdOnGround = classTypeIDs[comb_TargetClassTypeOnGround.SelectedIndex];
            }

            int principalId = ((PrincipalInfo)combTargetPrincipal.SelectedItem).principal_id;
            int workTypeId = ((WorkTypeInfo)combTargetWorkType.SelectedItem).work_type_id;

            //获取描述
            bool isModify = false;
            if (departId >0)
            {
                description.Append(string.Format("所属部门：->{0}，", _departNames[comb_TargetDepart.SelectedIndex]));
                isModify = true;
            }
            if (classTypeIdOnMine > 0)
            {
                description.Append(string.Format("井下班制：->{0}，", classTypeNames[comb_TargetClassTypeOnMine.SelectedIndex]));
                isModify = true;
            }
            if (classTypeIdOnGround > 0)
            {
                description.Append(string.Format("地面班制：->{0}，", classTypeNames[comb_TargetClassTypeOnGround.SelectedIndex]));
                isModify = true;
            }
            if (principalId > 0)
            {
                description.Append(string.Format("职务：->{0}，", ((PrincipalInfo)combTargetPrincipal.SelectedItem).principal_name));
                isModify = true;
            }
            if (workTypeId > 0)
            {
                description.Append(string.Format("工种：->{0}，", ((WorkTypeInfo)combTargetWorkType.SelectedItem).work_type_name));
                isModify = true;
            }
            if (isModify)
            {
                description.Remove(description.Length - 1, 1);
                description.Append("；\r\n");
            }
            else
            {
                this.DialogResult = true;
                return;
            }
            
             #endregion

            //操作员日志回调函数
            VmOperatorLog.CompleteCallBack completeCallBack = () =>
            {
                this.DialogResult = true;
            };
            
            //构建异步任务执行器
            if (_runnerTask != null)
            {
                _runnerTask.Stop = false;
                _runnerTask = null;
            }

            _runnerTask = new AsyncActionRunner(GetBatchModifyPersonTasks(selectedPersonIds.ToArray(), 5000,
                departId, classTypeIdOnGround, classTypeIdOnMine, principalId, workTypeId));
            //下面是运行完成后的处理
            _runnerTask.Completed += (obj, a) =>
            {
                WaitingDialog.HideWaiting();
                if ((obj as AsyncActionRunner).Reason == 0)
                {
                    if (_asyncRusltOptionInfo.isSuccess && _asyncRusltOptionInfo.isNotifySuccess)
                    {
                        Dialog.MsgBoxWindow.MsgBox("操作成功！", Dialog.MsgBoxWindow.MsgIcon.Succeed,
                            Dialog.MsgBoxWindow.MsgBtns.OK);
                        VmOperatorLog.InsertOperatorLog(1, "批量修改人员属性", description.ToString(), completeCallBack);
                    }
                    else
                    {
                        if (!_asyncRusltOptionInfo.isNotifySuccess)
                        {
                            Dialog.MsgBoxWindow.MsgBox(_asyncRusltOptionInfo.option_info + "！",
                                Dialog.MsgBoxWindow.MsgIcon.Warning, Dialog.MsgBoxWindow.MsgBtns.OK);
                            VmOperatorLog.InsertOperatorLog(1, "批量修改人员属性", description.ToString() + _asyncRusltOptionInfo.option_info, completeCallBack);
                        }
                        else
                        {
                            Dialog.MsgBoxWindow.MsgBox(_asyncRusltOptionInfo.option_info + "！",
                                Dialog.MsgBoxWindow.MsgIcon.Error, Dialog.MsgBoxWindow.MsgBtns.OK);
                            VmOperatorLog.InsertOperatorLog(0, "批量修改人员属性", description.ToString() + _asyncRusltOptionInfo.option_info, completeCallBack);
                        }

                    }
                }
            };

            WaitingDialog.ShowWaiting();
            //执行异步任务队列
            _runnerTask.Execute();

           

        }

        #endregion


        private OptionInfo _asyncRusltOptionInfo;   //异步任务的操作返回信息
        /// <summary>
        /// 构造批量修改人员属性的异步任务队列 
        /// </summary>
        /// <param name="personIds"></param>
        /// <param name="countPerTask"></param>
        /// <param name="departId"></param>
        /// <param name="classTypeIdOnGround"></param>
        /// <param name="classTypeIdOnMine"></param>
        /// <param name="principalId"></param>
        /// <param name="workTypeId"></param>
        /// <returns></returns>
        private List<IAsyncAction> GetBatchModifyPersonTasks(int[] personIds,int countPerTask,
            int departId, int classTypeIdOnGround, int classTypeIdOnMine, int principalId, int workTypeId)
        {
            List<IAsyncAction> tasks = new List<IAsyncAction>();

            for (int i = 0; i < personIds.Length; i+= countPerTask )
            {
                int[] curPersonIds;
                //构造一次任务的人数
                if (i + countPerTask -1 < personIds.Length)
                {
                    curPersonIds = new int[countPerTask];
                    for (int j = 0; j < countPerTask; j++)
                    {
                        curPersonIds[j] = personIds[i + j];
                    }
                }
                else
                {
                    curPersonIds = new int[personIds.Length - i];
                    for (int j = 0; j < curPersonIds.Length; j++)
                    {
                        curPersonIds[j] = personIds[i + j];
                    }
                }


                ///创建新任务
                var taskTemp = new AsyncAction("BatchModifyPersonTask" + 1);

                //设置任务工作内容
                taskTemp.SetAction(() =>
                {

                    //异步回调函数，返回后台操作成功或者失败的标志
                    Action<InvokeOperation<OptionInfo>> callBack = CallBackHandleControl<OptionInfo>.OnInvokeErrorCallBack;
                    CallBackHandleControl<OptionInfo>.m_sendValue = (o) =>
                    {
                        _asyncRusltOptionInfo = o;
                        //表示当前任务完成了,执行下一个任务
                        taskTemp.OnCompleted();

                    };

                    _serviceDomDbAccess.BatchModifyPersons(personIds, departId,
                        classTypeIdOnGround, classTypeIdOnMine, principalId, workTypeId, callBack, null);

                }, true);

                tasks.Add(taskTemp);

            }


            return tasks;
        }


        #region 排序 by cty



        private void dgSelectedPerson_MouseMove(object sender, MouseEventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        private void dgSelectedPerson_LayoutUpdated(object sender, EventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }


        private void dgSelectedPerson_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MyDataGridSortInChinese.OrderData(sender, e, _selectedUserPersonInfos);
        }

        #endregion

    }
}

