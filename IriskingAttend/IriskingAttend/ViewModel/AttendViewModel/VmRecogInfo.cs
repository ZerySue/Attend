/*************************************************************************
** 文件名:   VmRecogInfo.cs
** 主要类:   VmRecogInfo
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   lzc
** 日  期:   2013-4-16
** 修改人:   gqy
** 日  期:   2013-9-10
** 描  述:   VmRecogInfo，识别记录管理
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
using IriskingAttend.View;
using IriskingAttend.Dialog;
using System.Collections.Generic;
using IriskingAttend.ViewModel.AttendViewModel;
using System.IO.IsolatedStorage;
using IriskingAttend.ViewModel.SystemViewModel;

namespace IriskingAttend.ViewModel
{
    public class VmRecogInfo :BaseViewModel
    {
        #region 绑定数据

        /// <summary>
        /// 绑定数据源，识别记录汇总信息
        /// </summary>
        public BaseViewModelCollection<UserPersonRecogLog> RecogInfoModel 
        {
            get;
            set;
        }

        /// <summary>
        /// 识别记录详细信息
        /// </summary>
        public BaseViewModelCollection<UserAllPersonRecogLog> AllRecogInfoModel 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// 是否显示识别类型
        /// </summary>
        public int IsShowRecogType 
        {
            get
            {
                return _isShowRecogType;
            }

            set
            {
                if (_isShowRecogType == value)
                    return;
                _isShowRecogType = value;
                OnPropertyChanged(() => IsShowRecogType);

            }
        }

        /// <summary>
        /// 全选按钮绑定  add by gqy
        /// </summary>
        public MarkObject MarkObj
        {
            get;
            set;
        }

        /// <summary>
        /// 选中识别记录信息  add by gqy
        /// </summary>
        private UserPersonRecogLog _selectPersonRecogItem;
        public UserPersonRecogLog SelectPersonRecogItem
        {
            get
            {
                return _selectPersonRecogItem;
            }
            set
            {
                _selectPersonRecogItem = value;
                OnPropertyChanged<UserPersonRecogLog>(() => this.SelectPersonRecogItem);
            }
        }

        /// <summary>
        /// 批量删除识别记录按钮的enable属性  add by gqy
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

        #region 私有变量

        //本地独立存储，用来传参
        private IsolatedStorageSettings _querySetting = IsolatedStorageSettings.ApplicationSettings;

        //部门类
        private VmDepartment _depart = new VmDepartment();

        //识别记录查询条件
        private RecogCondition _recogCondition = new RecogCondition();

        //是否为识别记录更新
        private bool _isUpRecog = false;

        //是否显示识别类型
        private int _isShowRecogType = 1;

        #endregion

        #region 事件

        /// <summary>
        /// 考勤记录加载完成
        /// </summary>
        public event EventHandler AttendRecgLoadCompleted;

        /// <summary>
        /// 识别记录更新完成
        /// </summary>
        public event EventHandler RecogUpadateCompleted;


        /// <summary>
        /// 从配置文件获取是否显示识别类型完成
        /// </summary>
        public event EventHandler IsShowTypeCompleted;

        #endregion

        #region 构造函数

        public VmRecogInfo()
        {
            ServiceDomDbAcess.ReOpenSever();
            RecogInfoModel = new BaseViewModelCollection<UserPersonRecogLog>();
            AllRecogInfoModel = new BaseViewModelCollection<UserAllPersonRecogLog>();

            AttendRecgLoadCompleted += (o, e) =>{};
            RecogUpadateCompleted += (o, e) => { };
            IsShowTypeCompleted += (o, e) => { };

            //获取是否显示识别类型列
            GetIsShowRecogType();
        }

        #endregion

        #region Command 委托变量

        /// <summary>
        /// 添加识别记录委托
        /// </summary>
        private DelegateCommand<AddRecog> _addRecogCmd = null;

        /// <summary>
        /// 添加识别记录委托
        /// </summary>
        private DelegateCommand<IriskingAttend.ZhongKeHongBa.AddRecog> _addZKHBRecogCmd = null;

        /// <summary>
        /// 删除识别记录委托
        /// </summary>
        private DelegateCommand _delRecogCmd = null;

        /// <summary>
        /// 重构识别记录委托
        /// </summary>
        private DelegateCommand _rebuildRecogCmd = null;

        #endregion

        #region ICommand 绑定

        /// <summary>
        /// 添加识别记录命令
        /// </summary>
        public ICommand AddRecogCommand
        {
            get
            {
                if (null == _addRecogCmd)
                {
                    _addRecogCmd = new DelegateCommand<AddRecog>(DealPersonRecog);
                }
                return _addRecogCmd;
            }
        }

        /// <summary>
        /// 添加中科红霸识别记录命令
        /// </summary>
        public ICommand AddZKHBRecogCommand
        {
            get
            {
                if (null == _addZKHBRecogCmd)
                {
                    _addZKHBRecogCmd = new DelegateCommand<IriskingAttend.ZhongKeHongBa.AddRecog>(ZKHBDealPersonRecog);
                }
                return _addZKHBRecogCmd;
            }
        }

        /// <summary>
        /// 删除识别记录
        /// </summary>
        public ICommand DelRecogCommand
        {
            get
            {
                if (null == _delRecogCmd)
                {
                    //_delRecogCmd = new DelegateCommand(DelPersonRecog);
                }
                return _delRecogCmd;
            }
        }

        /// <summary>
        /// 重构命令
        /// </summary>
        public ICommand RebuildRecogCommand
        {
            get
            {
                if (null == _rebuildRecogCmd)
                {
                    //_rebuildRecogCmd = new DelegateCommand(RebuildPersonRecog);
                }
                return _rebuildRecogCmd;
            }
        }
        #endregion

        #region Icommand绑定的函数


        /// <summary>
        /// 添加识别记录
        /// </summary>
        public void DealPersonRecog(AddRecog recogDialog)
        {
            try
            {
                UserPersonRecogLog recog = new UserPersonRecogLog();
                recog.person_id = recogDialog.Person.person_id;
                recog.work_sn = recogDialog.Person.work_sn;
                //gqy  2013-11-6
                recog.person_name = recogDialog.Person.person_name;
                //recog.at_position = recogDialog.tbPosition.Text.ToString();
                //recog.operator_name = recogDialog.tbMemo.Text.Trim(); //yht-2014-09-028
                if (null != recogDialog.dateBegin.SelectedDate)
                {
                    if (null != recogDialog.timeBegin.Value)
                    {
                        recog.recog_time = recogDialog.dateBegin.SelectedDate.Value.Date + recogDialog.timeBegin.Value.Value.TimeOfDay;
                    }
                    else
                    {
                        recog.recog_time = recogDialog.dateBegin.SelectedDate.Value.Date;
                    }
                }
                else
                {
                    MsgBoxWindow.MsgBox( "时间为必填项！",
                                   MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);
                    return;
                }
                recog.at_position = "";
                if (recogDialog.cmbDev.SelectedIndex != 0)
                {
                    recog.at_position = recogDialog.VmDevMng.SystemDeviceInfo[recogDialog.cmbDev.SelectedIndex-1].place;
                    recog.device_sn = recogDialog.VmDevMng.SystemDeviceInfo[recogDialog.cmbDev.SelectedIndex - 1].dev_sn;
                }

                Dictionary<int,string> DictDeviceType = PublicMethods.GetDevTypeDictionary(VmLogin.GetIsMineApplication());
                foreach( KeyValuePair<int,string > item in DictDeviceType )
                {
                    if(item.Value == (string)(recogDialog.cmbDevType.SelectedItem))
                    {
                        recog.dev_type_value = item.Key;
                    }
                }               

                if (recog.recog_time > DateTime.Now)
                {
                    MsgBoxWindow.MsgBox( "添加时间大于当前时间，是否确定添加！",
                                   MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OKCancel,
                                   (Result) =>
                                   {
                                       if (Result == MsgBoxWindow.MsgResult.OK)
                                       {
                                           AddRecogInDB(recogDialog, recog);
                                       }
                                   });
                }
                else
                {
                    AddRecogInDB(recogDialog, recog);
                }


            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }

        #region 添加中科红霸识别记录

        /// <summary>
        /// 添加中科红霸识别记录
        /// </summary>
        public void ZKHBDealPersonRecog(IriskingAttend.ZhongKeHongBa.AddRecog recogDialog)
        {
            try
            {
                UserPersonRecogLog recog = new UserPersonRecogLog();
                recog.person_id = recogDialog.Person.person_id;
                recog.work_sn = recogDialog.Person.work_sn;
                //gqy  2013-11-6
                recog.person_name = recogDialog.Person.person_name;
                //recog.at_position = recogDialog.tbPosition.Text.ToString();
                recog.operator_name = recogDialog.tbMemo.Text.Trim(); //yht-2014-09-028
                if (null != recogDialog.dateBegin.SelectedDate)
                {
                    if (null != recogDialog.timeBegin.Value)
                    {
                        recog.recog_time = recogDialog.dateBegin.SelectedDate.Value.Date + recogDialog.timeBegin.Value.Value.TimeOfDay;
                    }
                    else
                    {
                        recog.recog_time = recogDialog.dateBegin.SelectedDate.Value.Date;
                    }
                }
                else
                {
                    MsgBoxWindow.MsgBox("时间为必填项！",
                                   MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);
                    return;
                }
                recog.at_position = "";
                if (recogDialog.cmbDev.SelectedIndex != 0)
                {
                    recog.at_position = recogDialog.VmDevMng.SystemDeviceInfo[recogDialog.cmbDev.SelectedIndex - 1].place;
                    recog.device_sn = recogDialog.VmDevMng.SystemDeviceInfo[recogDialog.cmbDev.SelectedIndex - 1].dev_sn;
                }

                Dictionary<int, string> DictDeviceType = PublicMethods.GetDevTypeDictionary(VmLogin.GetIsMineApplication());
                foreach (KeyValuePair<int, string> item in DictDeviceType)
                {
                    if (item.Value == (string)(recogDialog.cmbDevType.SelectedItem))
                    {
                        recog.dev_type_value = item.Key;
                    }
                }

                if (recog.recog_time > DateTime.Now)
                {
                    MsgBoxWindow.MsgBox("添加时间大于当前时间，是否确定添加！",
                                   MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OKCancel,
                                   (Result) =>
                                   {
                                       if (Result == MsgBoxWindow.MsgResult.OK)
                                       {
                                           AddZKHBRecogInDB(recogDialog, recog);
                                       }
                                   });
                }
                else
                {
                    AddZKHBRecogInDB(recogDialog, recog);
                }


            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }

        /// <summary>
        /// 添加中科红霸识别记录到数据库 add yht 2014-09-29
        /// </summary>
        /// <param name="recogDialog"></param>
        /// <param name="recog"></param>
        private void AddZKHBRecogInDB(IriskingAttend.ZhongKeHongBa.AddRecog recogDialog, UserPersonRecogLog recog)
        {
            Action<InvokeOperation<int>> insertUserPersonRecogCallBack = CallBackHandleControl<int>.OnInvokeErrorCallBack;

            CallBackHandleControl<int>.m_sendValue = (o) =>
            {
                WaitingDialog.HideWaiting();

                string logDescrip = string.Format("姓名：{0}，工号：{1}，识别时间：{2}； ", recog.person_name, recog.work_sn, recog.recog_time);
                if (o == 0)
                {
                    recogDialog.DialogResult = true;
                    ServiceDomDbAcess.ReOpenSever();
                    _isUpRecog = true;

                    VmOperatorLog.InsertOperatorLog(1, "添加识别记录", logDescrip, () =>
                    {
                        GetPersonRecog();

                        MsgBoxWindow.MsgBox("添加成功！",
                            MsgBoxWindow.MsgIcon.Succeed, MsgBoxWindow.MsgBtns.OK);
                    });

                    return;
                }
                else
                {
                    VmOperatorLog.InsertOperatorLog(0, "添加识别记录", logDescrip, () =>
                    {
                        MsgBoxWindow.MsgBox("添加失败！",
                            MsgBoxWindow.MsgIcon.Error, MsgBoxWindow.MsgBtns.OK);
                    });
                }
            };
            if (!string.IsNullOrEmpty(recog.operator_name))
            {
                recog.operator_name = string.Format("{0},by {1}", recog.operator_name, VmLogin.GetUserName());
            }
            else
            {
                recog.operator_name = string.Format("by {0}",VmLogin.GetUserName());
            }
            ServiceDomDbAcess.GetSever().IrisInsertUserPersonRecogLog(recog, insertUserPersonRecogCallBack, new Object());
            WaitingDialog.ShowWaiting("添加识别记录中...");
        }

        #endregion

        /// <summary>
        /// 添加识别记录到数据库
        /// </summary>
        /// <param name="recogDialog"></param>
        /// <param name="recog"></param>
        private void AddRecogInDB(AddRecog recogDialog, UserPersonRecogLog recog)
        {
            Action<InvokeOperation<int>> insertUserPersonRecogCallBack = CallBackHandleControl<int>.OnInvokeErrorCallBack;

            CallBackHandleControl<int>.m_sendValue = (o) =>
            {
                WaitingDialog.HideWaiting();

                string logDescrip = string.Format("姓名：{0}，工号：{1}，识别时间：{2}； ", recog.person_name, recog.work_sn, recog.recog_time);
                if (o == 0)
                {
                    recogDialog.DialogResult = true;
                    ServiceDomDbAcess.ReOpenSever();
                    _isUpRecog = true;

                    VmOperatorLog.InsertOperatorLog(1, "添加识别记录", logDescrip, () =>
                        {
                            GetPersonRecog();

                            MsgBoxWindow.MsgBox("添加成功！",
                                MsgBoxWindow.MsgIcon.Succeed, MsgBoxWindow.MsgBtns.OK);
                        });                    

                    return;
                }
                else
                {
                    VmOperatorLog.InsertOperatorLog(0, "添加识别记录", logDescrip, () =>
                        {
                            MsgBoxWindow.MsgBox("添加失败！",
                                MsgBoxWindow.MsgIcon.Error, MsgBoxWindow.MsgBtns.OK);
                        });
                }
            };
            recog.operator_name = string.Format("操作员{0}添加", VmLogin.GetUserName()); 
            ServiceDomDbAcess.GetSever().IrisInsertUserPersonRecogLog(recog, insertUserPersonRecogCallBack, new Object());
            WaitingDialog.ShowWaiting("添加识别记录中...");
        }

        /// <summary>
        /// 显示添加识别记录对话框  通过识别记录
        /// </summary>
        /// <param name="person"></param>
        public void ShowAddRecogDialog(UserPersonRecogLog recog)
        {
            UserPersonSimple personTemp = new UserPersonSimple();
            personTemp.person_id = recog.person_id;
            personTemp.work_sn = recog.work_sn;
            personTemp.person_name = recog.person_name;
            //显示添加识别记录对话框
            ShowAddRecogDialog(personTemp, recog.recog_time);
        }

        /// <summary>
        /// 显示中科红霸添加窗口
        /// </summary>
        /// <param name="recog"></param>
        public void ShowZKHBAddRecogDialog(UserPersonRecogLog recog)
        {
            try
            {
                UserPersonSimple personTemp = new UserPersonSimple();
                personTemp.person_id = recog.person_id;
                personTemp.work_sn = recog.work_sn;
                personTemp.person_name = recog.person_name;

                IriskingAttend.ZhongKeHongBa.AddRecog addRecog = new IriskingAttend.ZhongKeHongBa.AddRecog(personTemp);
                addRecog.btnSave.Command = AddZKHBRecogCommand;
                addRecog.btnSave.CommandParameter = addRecog;
                if (recog.recog_time != null)
                {
                    addRecog.dateBegin.Text = ((DateTime)recog.recog_time).ToString("yyyy-M-d");
                }
                addRecog.Show();
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }

        /// <summary>
        /// 显示中科红霸添加窗口 通过人员信息
        /// </summary>
        /// <param name="recog"></param>
        public void ShowZKHBAddRecogDialog(UserPersonSimple personTemp, DateTime? recogTime)
        {
            try
            {
                IriskingAttend.ZhongKeHongBa.AddRecog addRecog = new IriskingAttend.ZhongKeHongBa.AddRecog(personTemp);
                addRecog.btnSave.Command = AddZKHBRecogCommand;
                addRecog.btnSave.CommandParameter = addRecog;
                if (recogTime != null)
                {
                    addRecog.dateBegin.Text = recogTime.Value.ToString("yyyy-M-d");
                }
                addRecog.Show();
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }

        /// <summary>
        /// 显示添加识别记录对话框 通过人员信息
        /// </summary>
        /// <param name="person"></param>
        public void ShowAddRecogDialog(UserPersonSimple person)
        {
            ShowAddRecogDialog( person, null );
        }

        /// <summary>
        /// 显示添加识别记录对话框 通过人员信息  add by gqy
        /// </summary>
        /// <param name="person"></param>
        public void ShowAddRecogDialog(UserPersonSimple person, DateTime? recogTime )
        {
            try
            {
                AddRecog addRecog = new AddRecog(person);
                addRecog.btnSave.Command = AddRecogCommand;
                addRecog.btnSave.CommandParameter = addRecog;
                if (recogTime != null)
                {
                    addRecog.dateBegin.Text = ((DateTime)recogTime).ToString("yyyy-M-d");
                }
                addRecog.Show();
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }

        /// <summary>
        /// 删除识别记录
        /// </summary>
        public void DelPersonRecog(UserPersonRecogLog recog)
        {
            if (recog != null)
            {

                MsgBoxWindow.MsgBox( "请注意，您将进行如下操作：\r\n 删除 \"" +
                        recog.person_name + "\" 在 " + recog.recog_time + " 时的识别记录！",
                        MsgBoxWindow.MsgIcon.Warning, MsgBoxWindow.MsgBtns.OKCancel,
                        (e) =>
                        {
                            if (e == MsgBoxWindow.MsgResult.OK)
                            {
                                try
                                {
                                    Action<InvokeOperation<int>> deleteUserPersonRecogCallBack = CallBackHandleControl<int>.OnInvokeErrorCallBack;
                                    CallBackHandleControl<int>.m_sendValue = (o) =>
                                    {
                                        WaitingDialog.HideWaiting();
                                        string logDescrip = string.Format("姓名：{0}，工号：{1}，识别时间：{2}；", recog.person_name, recog.work_sn, recog.recog_time);
                                        if (o == 0)
                                        {
                                            VmOperatorLog.InsertOperatorLog(1, "删除识别记录", logDescrip, () =>
                                              {
                                                  MsgBoxWindow.MsgBox("删除成功！",
                                                                        MsgBoxWindow.MsgIcon.Succeed, MsgBoxWindow.MsgBtns.OK);

                                                  ServiceDomDbAcess.ReOpenSever();
                                                  _isUpRecog = true;
                                                  GetPersonRecog();
                                              });
                                        }
                                        else
                                        {
                                            VmOperatorLog.InsertOperatorLog(0, "删除识别记录", logDescrip, () =>
                                              {
                                                  MsgBoxWindow.MsgBox("删除失败！",
                                                                        MsgBoxWindow.MsgIcon.Error, MsgBoxWindow.MsgBtns.OK);
                                              });
                                        }
                                        //GetAttendLeave();
                                    };
                                    ServiceDomDbAcess.GetSever().IrisDeleteUserPersonRecogLog(recog, deleteUserPersonRecogCallBack, new Object());
                                    ServiceDomDbAcess.ReOpenSever();
                                    WaitingDialog.ShowWaiting("删除识别记录,请等待...");
                                }
                                catch (Exception er)
                                {
                                    ErrorWindow err = new ErrorWindow(er);
                                    err.Show();
                                }
                            }
                        });
            }
        }

        /// <summary>
        /// 重构识别记录
        /// </summary>
        public void RebuildPersonRecog(UserPersonRecogLog recog)
        {
            try
            {
                if (recog != null)
                {
                    Action<InvokeOperation<int>> rebuildAttendCallBack = CallBackHandleControl<int>.OnInvokeErrorCallBack;
                    CallBackHandleControl<int>.m_sendValue = (o) =>
                    {
                        WaitingDialog.HideWaiting();

                        string logDescrip = string.Format("姓名：{0}，工号：{1}，识别时间：{2}；", recog.person_name, recog.work_sn, recog.recog_time);
                        if (o == 0)
                        {
                            VmOperatorLog.InsertOperatorLog(1, "重构识别记录", logDescrip, () =>
                           {
                               MsgBoxWindow.MsgBox("重构成功！",
                                                     MsgBoxWindow.MsgIcon.Succeed, MsgBoxWindow.MsgBtns.OK);

                               ServiceDomDbAcess.ReOpenSever();
                               _isUpRecog = true;
                               GetPersonRecog();

                               //2013-11-5 gqy
                               if (MarkObj != null)
                               {
                                   MarkObj.Selected = false;
                               }
                           });
                        }
                        else
                        {
                            VmOperatorLog.InsertOperatorLog(0, "重构识别记录", logDescrip, () =>
                            {
                                MsgBoxWindow.MsgBox( "重构失败！",
                                                  MsgBoxWindow.MsgIcon.Error, MsgBoxWindow.MsgBtns.OK);
                            });
                        }
                    };
                    int[] ids = { recog.person_id };//_bindingRecog.person_id _bindingRecog.recog_time
                    ServiceDomDbAcess.GetSever().IrisRebuildAttend(recog.recog_time, ids, rebuildAttendCallBack, new Object());
                    WaitingDialog.ShowWaiting("正在重构，请等待...");
                }
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }
        #endregion

        #region 函数

        /// <summary>
        /// 批量删除识别记录   add by gqy
        /// </summary>
        public void BatchDeletePersonRecog()
        {
            //对话框提示信息
            string strInfo = "请注意，您将进行如下操作:\r\n批量删除识别记录！";
            MsgBoxWindow.MsgBox(strInfo, MsgBoxWindow.MsgIcon.Warning, MsgBoxWindow.MsgBtns.OKCancel, (result) =>
            {
                //确定删除
                if (result == MsgBoxWindow.MsgResult.OK)
                {
                    string logDescrip = "";
                    //将选中的识别记录添加到删除列表中
                    List<int> RecogList = new List<int>();
                    foreach (UserPersonRecogLog recog in RecogInfoModel)
                    {
                        if (recog.isSelected)
                        {
                            RecogList.Add(recog.person_recog_id);
                            logDescrip += string.Format("姓名：{0}，工号：{1}， 识别时间：{2}；", recog.person_name, recog.work_sn, recog.recog_time);
                        }
                    }

                    try
                    {
                        Action<InvokeOperation<int>> deleteUserPersonRecogCallBack = CallBackHandleControl<int>.OnInvokeErrorCallBack;
                        CallBackHandleControl<int>.m_sendValue = (o) =>
                        {
                            WaitingDialog.HideWaiting();
                            if (o == 0)
                            {
                                VmOperatorLog.InsertOperatorLog(1, "批量删除识别记录", logDescrip, () =>
                                {
                                    MsgBoxWindow.MsgBox("删除成功！",
                                                            MsgBoxWindow.MsgIcon.Succeed, MsgBoxWindow.MsgBtns.OK);

                                    ServiceDomDbAcess.ReOpenSever();
                                    _isUpRecog = true;
                                    GetPersonRecog();
                                    MarkObj.Selected = false;
                                });
                            }
                            else
                            {
                                VmOperatorLog.InsertOperatorLog(0, "批量删除识别记录", logDescrip, () =>
                                {
                                    MsgBoxWindow.MsgBox("删除失败！",
                                                           MsgBoxWindow.MsgIcon.Error, MsgBoxWindow.MsgBtns.OK);
                                });
                            }
                        };
                        ServiceDomDbAcess.GetSever().IrisDeleteUserPersonRecogLogsForIDS(RecogList.ToArray(), deleteUserPersonRecogCallBack, new Object());
                        ServiceDomDbAcess.ReOpenSever();
                        WaitingDialog.ShowWaiting("删除识别记录,请等待...");
                    }
                    catch (Exception er)
                    {
                        ErrorWindow err = new ErrorWindow(er);
                        err.Show();
                    }
                }                
            });
        }

        /// <summary>
        /// 选中全部识别记录或者取消选中  add by gqy
        /// </summary>
        /// <param name="isChecked">true: 选中， false：未选中</param>
        public void SelectAllPersonRecog(bool isChecked)
        {
            //将全部识别记录置为全选或全不选状态
            foreach (UserPersonRecogLog item in RecogInfoModel)
            {               
                item.isSelected = isChecked;
            }

            //更新批量删除按钮的可用状态
            IsBatchDeleteEnabled = CheckIsBatchDeleteEnabled();
        }

        /// <summary>
        /// 确定批量删除按钮的可用状态  add by gqy
        /// </summary>
        /// <returns>true: 可用状态， false：不可用状态</returns>
        private bool CheckIsBatchDeleteEnabled()
        {
            //遍历所有设备信息
            foreach (UserPersonRecogLog item in RecogInfoModel)
            {
                //只要有一台设备处于选中状态，批量删除按钮就可用。
                if (item.isSelected)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 确定全选按钮的选中状态  add by gqy
        /// </summary>
        /// <returns>选中状态</returns>
        private bool CheckIsAllDevSelected()
        {
            //默认为选中
            bool checkAll = true;

            //遍历所有识别记录
            foreach (UserPersonRecogLog item in RecogInfoModel)
            {
                //只要有一条识别记录未选中，全选按钮就处于未选中状态
                if (!item.isSelected)
                {
                    checkAll = false;
                    break;
                }
            }
            return checkAll;
        }

        /// <summary>
        /// 更改当前选中记录的选中状态  add by gqy
        /// </summary>
        /// <param name="selectPersonRecogItem">选中记录的信息源</param>
        public void ChangePersonRecogCheckedState(UserPersonRecogLog selectPersonRecogItem)
        {
            //反选选中设备状态  
            selectPersonRecogItem.isSelected = !selectPersonRecogItem.isSelected;

            //更新全选按钮的选中状态
            MarkObj.Selected = CheckIsAllDevSelected();

            //更新批量删除按钮的可用状态
            IsBatchDeleteEnabled = CheckIsBatchDeleteEnabled();
        }
        

        /// <summary>
        /// 获取是否显示识别记录类型列--主要应用于测试要求
        /// </summary>
        public void GetIsShowRecogType()
        {
            Action<InvokeOperation<int>> rebuildCallBack = CallBackHandleControl<int>.OnInvokeErrorCallBack;

            CallBackHandleControl<int>.m_sendValue = (result) =>
            {
                WaitingDialog.HideWaiting();
                IsShowRecogType = result;
                IsShowTypeCompleted(null, null);
            };

            ///获取配置信息
            ServiceDomDbAcess.GetSever().GetIsShowRecogType(rebuildCallBack, new Object());
            WaitingDialog.ShowWaiting();
        }

        /// <summary>
        /// 按时间和person_id查询识别记录
        /// </summary>
        /// <param name="beginTime">起始时间</param>
        /// <param name="endTime">截止时间</param>
        /// <param name="personId">人员ID</param>
        public void GetPersonRecog(DateTime beginTime, DateTime endTime, int personId)
        {
                _recogCondition.PersongId = personId;
                if (beginTime != null)
                {
                    _recogCondition.BeginDate = beginTime;
                }
                else
                {
                    _recogCondition.BeginDate = DateTime.MinValue;
                }

                if (endTime != null)
                {
                    _recogCondition.EndDate = endTime;
                }
                else
                {
                    _recogCondition.EndDate = DateTime.MaxValue;
                }
                try
                {
                    WaitingDialog.ShowWaiting("加载识别记录...");
                    EntityQuery<UserPersonRecogLog> list = ServiceDomDbAcess.GetSever().IrisGetPersonRecogQuery(beginTime, endTime, personId);

                    ///回调异常类
                    Action<LoadOperation<UserPersonRecogLog>> actionCallBack = new Action<LoadOperation<UserPersonRecogLog>>
                        (ErrorHandle<UserPersonRecogLog>.OnLoadErrorCallBack);
                    ///异步事件
                    LoadOperation<UserPersonRecogLog> lo = ServiceDomDbAcess.GetSever().Load(list, actionCallBack, null);

                    lo.Completed += (o, e) =>
                    {
                        RecogInfoModel.Clear();
                        try
                        {
                            //异步获取数据
                            foreach (UserPersonRecogLog ar in lo.Entities.OrderBy(a => a.recog_time))
                            {
                                RecogInfoModel.Add(ar);
                            }
                            WaitingDialog.HideWaiting();
                            AttendRecgLoadCompleted(null, null);
                            if (_isUpRecog)
                            {
                                RecogUpadateCompleted(null, null);
                                _isUpRecog = false;
                            }
                        }
                        catch (Exception err)
                        {
                            WaitingDialog.HideWaiting();
                            ChildWindow errorWin = new ErrorWindow("加载异常", err.Message);
                            errorWin.Show();
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
        /// 按时间和person_id查询识别记录
        /// </summary>
        /// <param name="personId">人员ID</param>
        public void GetPersonRecogByAttend(int personId)
        {

            if (_querySetting.Contains("attendConditon"))
            {
                AttendQueryCondition condition;
                _querySetting.TryGetValue<AttendQueryCondition>("attendConditon", out condition);
                _recogCondition.PersongId = personId;
                _recogCondition.BeginDate = condition.BeginTime;
                _recogCondition.EndDate = condition.EndTime;

                int[] devType = null;
                if (condition.DevTypeIdLst != null && condition.DevTypeIdLst.Count() > 0)
                {
                    devType = new int[3];
                    devType[0] = condition.DevTypeIdLst[0] - 2;
                    devType[1] = condition.DevTypeIdLst[0] - 1;
                    devType[2] = condition.DevTypeIdLst[0];
                }
                try
                {
                    WaitingDialog.ShowWaiting("加载识别记录...");
                    EntityQuery<UserPersonRecogLog> list = ServiceDomDbAcess.GetSever().IrisGetPersonRecogByDevTypeQuery(condition.BeginTime, condition.EndTime, devType, personId);

                    ///回调异常类
                    Action<LoadOperation<UserPersonRecogLog>> actionCallBack = new Action<LoadOperation<UserPersonRecogLog>>
                        (ErrorHandle<UserPersonRecogLog>.OnLoadErrorCallBack);
                    ///异步事件
                    LoadOperation<UserPersonRecogLog> lo = ServiceDomDbAcess.GetSever().Load(list, actionCallBack, null);

                    lo.Completed += (o, e) =>
                    {
                        RecogInfoModel.Clear();
                        try
                        {
                            //异步获取数据
                            foreach (UserPersonRecogLog ar in lo.Entities.OrderBy(a => a.recog_time))
                            {
                                RecogInfoModel.Add(ar);
                            }
                            WaitingDialog.HideWaiting();
                            AttendRecgLoadCompleted(null, null);
                            if (_isUpRecog)
                            {
                                RecogUpadateCompleted(null, null);
                                _isUpRecog = false;
                            }
                        }
                        catch (Exception err)
                        {
                            WaitingDialog.HideWaiting();
                            ChildWindow errorWin = new ErrorWindow("加载异常", err.Message);
                            errorWin.Show();
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
        }

        /// <summary>
        /// 按时间等查询人员识别记录
        /// 识别管理界面
        /// </summary>
        public void GetAllPersonRecog(RecogCondition recogContion)
        {
            if (_recogCondition != recogContion)
            {
                _recogCondition = recogContion;
            }

            try
            {
                EntityQuery<UserAllPersonRecogLog> lstAllPersonRecog = ServiceDomDbAcess.GetSever().IrisGetAllPersonRecogQuery(recogContion.BeginDate,
                    recogContion.EndDate, recogContion.DepartName, recogContion.PersonName, recogContion.WorkSn);

                ///回调异常类
                Action<LoadOperation<UserAllPersonRecogLog>> getAllPersonRecogCallBack = new Action<LoadOperation<UserAllPersonRecogLog>>
                    (ErrorHandle<UserAllPersonRecogLog>.OnLoadErrorCallBack);
                ///异步事件
                LoadOperation<UserAllPersonRecogLog> lo = ServiceDomDbAcess.GetSever().Load(lstAllPersonRecog, getAllPersonRecogCallBack, null);

                lo.Completed += (o, e) =>
                {
                    WaitingDialog.HideWaiting();

                    AllRecogInfoModel.Clear();
                    try
                    {
                        //异步获取数据
                        foreach (UserAllPersonRecogLog ar in lo.Entities)
                        {
                            AllRecogInfoModel.Add(ar);
                        }

                        //加载完成触发事件 
                        AttendRecgLoadCompleted(null, null);
                    }
                    catch (Exception err)
                    {
                        ChildWindow errorWin = new ErrorWindow("加载异常", err.Message);
                        errorWin.Show();
                    }
                };
                WaitingDialog.ShowWaiting("加载识别记录...");
            }
            catch (Exception e)
            {
                WaitingDialog.HideWaiting();
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }

        /// <summary>
        /// 按person_id查询识别记录   重构1
        /// </summary>
        /// <param name="personId">人员Id</param>
        public void GetPersonRecog(int personId)
        {
            GetPersonRecog(DateTime.MinValue, DateTime.MaxValue, personId);
        }

        /// <summary>
        /// 重构、添加识别，删除识别记录完成后更新识别记录
        /// </summary>
        private void GetPersonRecog()
        {
            if (_recogCondition.PersongId > 0)
            {
                GetPersonRecog(_recogCondition.BeginDate, _recogCondition.EndDate, _recogCondition.PersongId);
            }
            else
            {
                //GetPersonRecog(m_RecogContion);
            }
        }

        /// <summary>
        /// 重构、添加识别，删除识别记录完成后更新识别记录
        /// </summary>
        public void GetPersonRecog(RecogCondition recogCondition, int personID)
        {
            GetPersonRecog(recogCondition.BeginDate, recogCondition.EndDate, personID);
        }

        //public void GetIsShow
        #endregion
    }

    #region  辅助类
    /// <summary>
    /// 识别记录查询条件
    /// </summary>
    public class RecogCondition
    {
        /// <summary>
        /// 初始化构造函数
        /// </summary>
        public RecogCondition()
        {
            Clear();
        }

        /// <summary>
        /// 人员ID
        /// </summary>
        public int PersongId { get; set; }
        /// <summary>
        /// 起始时间
        /// </summary>
        public DateTime BeginDate { get; set; }
        
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 人员名字
        /// </summary>
        public string PersonName { get; set; }
        /// <summary>
        /// 部门名字
        /// </summary>
        public string DepartName { get; set; }

        /// <summary>
        /// 工号
        /// </summary>
        public string WorkSn { get; set; }

        /// <summary>
        /// 清空
        /// </summary>
        public void Clear()
        {
            PersongId = -1;
            BeginDate = DateTime.MinValue;
            EndDate = DateTime.MaxValue;
        }
    }
    /// <summary>
    /// 人员信息绑定类
    /// </summary>
    public class PersonRecogSimple : Entity
    {
        /// <summary>
        /// 人员ID
        /// </summary>
        public int PersonId { get; set; }

        /// <summary>
        /// 班次类型ID
        /// </summary>
        public int ClassTypeId { get; set; }

        /// <summary>
        /// 识别日期
        /// </summary>
        public DateTime RecogData { get; set; }

        /// <summary>
        /// 识别时间
        /// </summary>
        public DateTime RecogTime { get; set; }

        /// <summary>
        /// 识别类型
        /// </summary>
        public int RecogType { get; set; }

        /// <summary>
        /// 设备类型
        /// </summary>
        public int DevType { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 识别地点
        /// </summary>
        public string AtPosition { get; set; }

        /// <summary>
        /// 构造函数初始化
        /// </summary>
        public PersonRecogSimple()
        {
            RecogType = 0;//手动补加
            DevType = 0;
        }
    }


    #endregion
}
