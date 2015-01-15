/*************************************************************************
** 文件名:   ChildWndSelectObj.cs
** 主要类:   ChildWndSelectObj
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   wz
** 日  期:   2013-12-30
** 修改人:   
** 日  期:
** 描  述:   周源山选择对象窗口
**
** 版  本:   1.0.0
** 备  注:  命名及代码编写遵守C#编码规范
**
*************************************************************************/
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
using EDatabaseError;
using Irisking.Web.DataModel;
using System.ServiceModel.DomainServices.Client;
using IriskingAttend.Web.ZhouYuanShan;
using IriskingAttend.Common;
using System.Text;

namespace IriskingAttend.ZhouYuanShan
{
    public partial class ChildWndSelectObj : ChildWindow
    {

        #region 属性和字段
       
        private List<UserDepartInfo> _departs;               //原始部门信息
        private List<WorkTypeInfo_ZhouYuanShan> _workTypes;  //原始工种信息
        private List<UserPersonInfo> _persons;               //原始人员信息

        private SelectDepartUI _selectDepart;       //选择部门控件
        private SelectWorkTypeUI _selectWorkType;   //选择工种控件
        private SelectPersonUI _selectPerson;       //选择人员控件

        private bool _isSelectWorkType;            //是否选择工种作为条件
        private bool _isSelectDepart;              //是否选择部门作为条件
        private bool _isSelectPerson;              //是否选择人员作为条件


        private int _curTabControlIndex = -1;
        public int CurTabControlIndex
        {
            get{
                return _curTabControlIndex;
            }
        }
        #endregion

        public ChildWndSelectObj()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 获取选择描述
        /// </summary>
        /// <returns></returns>
        public string GetSelectObjDescription()
        {
            
            //获取当前选择的对象描述
            int index = 0;
            StringBuilder content = new StringBuilder();

            //部门
            if (_selectDepart != null)
            {
                foreach (var item in _selectDepart.selectedList.Items)
                {
                    content.Append(((UserDepartInfo)item).depart_name);
                    content.Append(',');
                    index++;
                }
            }

            //人员
            if (_selectPerson != null)
            {
                foreach (var item in _selectPerson.selectedList.Items)
                {
                    content.Append(((UserPersonInfo)item).person_name);
                    content.Append(',');
                    index++;
                }
            }
            
            //工种
            if (_selectWorkType != null)
            {
                foreach (var item in _selectWorkType.selectedList.Items)
                {
                    content.Append(((WorkTypeInfo_ZhouYuanShan)item).work_type_name);
                    content.Append(',');
                    index++;
                }
            }
            
            if (index > 0)
            {
                content.Remove(content.Length - 1, 1);
            }

            return content.ToString();
        }

        /// <summary>
        /// 初始化函数,WCF
        /// </summary>
        /// <param name="isSelectWorkType"></param>
        /// <param name="isSelectDepart"></param>
        /// <param name="isSelectPerson"></param>
        public void Init(bool isSelectWorkType, bool isSelectDepart, bool isSelectPerson)
        {
            _isSelectWorkType = isSelectWorkType;
            _isSelectPerson = isSelectPerson;
            _isSelectDepart = isSelectDepart;
            WaitingDialog.ShowWaiting();

            GetDepartRia((departs) =>
                {
                    //部门按树形结构排序
                    _departs = new List<UserDepartInfo>();
                    PublicMethods.OrderDepartByTree(departs, _departs, -1,"-");
                    _departs = PublicMethods.FiterDepartById(_departs, VmLogin.OperatorDepartIDList);

                    GetWorkTypeRia((workTypes) =>
                    {
                        _workTypes = workTypes;
                        GetPersonsRia((persons) =>
                        {
                            _persons = persons;
                            InitUI(isSelectWorkType, isSelectDepart, isSelectPerson);
                            WaitingDialog.HideWaiting();
                        });
                    });
                });
            
          
        }

        /// <summary>
        /// 根据条件决定是否初始化选择某个对象
        /// </summary>
        /// <param name="isSelectWorkType"></param>
        /// <param name="isSelectDepart"></param>
        /// <param name="isSelectPerson"></param>
        public void InitUI(bool isSelectWorkType, bool isSelectDepart, bool isSelectPerson)
        {
            if (isSelectDepart)
            {
                _selectDepart = new SelectDepartUI();
                _selectDepart.LoadDeparts(_departs);
                _selectDepart.LoadContent(_departs);
                _selectDepart.SelectedEvent += (tabControlIndex) =>
                    {
                        _curTabControlIndex = tabControlIndex;
                        //保留单一选择对象操作
                        if (_selectPerson != null)
                        {
                            _selectPerson.ClearAllSelectedValue();
                        }
                        if (_selectWorkType != null)
                        {
                            _selectWorkType.ClearAllSelectedValue();
                        }
                    };
                
                TabItem tabItemDepart = new TabItem();
                tabItemDepart.Content = _selectDepart;
                tabItemDepart.Header = " 部 门 ";

                _selectDepart.Tag = this.tabControl.Items.Count;
                this.tabControl.Items.Add(tabItemDepart);
            }

            if (isSelectWorkType)
            {
                _selectWorkType = new SelectWorkTypeUI();
                _selectWorkType.LoadDeparts(_departs);
                _selectWorkType.LoadContent(_workTypes);
                _selectWorkType.SelectedEvent += (tabControlIndex) =>
                {
                    _curTabControlIndex = tabControlIndex;
                    //保留单一选择对象操作
                    if (_selectPerson != null)
                    {
                        _selectPerson.ClearAllSelectedValue();
                    }
                    if (_selectDepart != null)
                    {
                        _selectDepart.ClearAllSelectedValue();
                    }
                };

                TabItem tabItemWorkType = new TabItem();
                tabItemWorkType.Content = _selectWorkType;
                tabItemWorkType.Header = " 工 种 ";
                _selectWorkType.Tag = this.tabControl.Items.Count;
                this.tabControl.Items.Add(tabItemWorkType);
            }

            if (isSelectPerson)
            {
                _selectPerson = new SelectPersonUI();
                _selectPerson.LoadDeparts(_departs);
                _selectPerson.LoadContent(_persons);
                _selectPerson.SelectedEvent += (tabControlIndex) =>
                {
                    _curTabControlIndex = tabControlIndex;
                    //保留单一选择对象操作
                    if (_selectDepart != null)
                    {
                        _selectDepart.ClearAllSelectedValue();
                    }
                    if (_selectWorkType != null)
                    {
                        _selectWorkType.ClearAllSelectedValue();
                    }
                };

                TabItem tabItemPerson = new TabItem();
                tabItemPerson.Content = _selectPerson;
                tabItemPerson.Header = " 人 员 ";

                _selectPerson.Tag = this.tabControl.Items.Count;
                this.tabControl.Items.Add(tabItemPerson);
            }
           
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        #region 公有函数

        /// <summary>
        /// 克隆选择对象窗口对象
        /// </summary>
        /// <returns></returns>
        public ChildWndSelectObj Clone()
        {
            ChildWndSelectObj res = new ChildWndSelectObj();
            res._departs = this._departs;
            res._workTypes = this._workTypes;
            res._persons = this._persons;
            res._isSelectDepart = this._isSelectDepart;
            res._isSelectPerson = this._isSelectPerson;
            res._isSelectWorkType = this._isSelectWorkType;

            if (_isSelectDepart)
            {
                TabItem tabItemDepart = new TabItem();
                //克隆UI对象
                res._selectDepart =this._selectDepart.Clone();
                tabItemDepart.Content = res._selectDepart;
                tabItemDepart.Header = " 部 门 ";
                res._selectDepart.Tag = res.tabControl.Items.Count;
                res.tabControl.Items.Add(tabItemDepart);
                res._selectDepart.SelectedEvent += (tabControlIndex) =>
                {
                    res._curTabControlIndex = tabControlIndex;
                    //保留单一选择对象操作
                    if (res._selectPerson != null)
                    {
                        res._selectPerson.ClearAllSelectedValue();
                    }
                    if (res._selectWorkType != null)
                    {
                        res._selectWorkType.ClearAllSelectedValue();
                    }
                };
            }

            if (_isSelectWorkType)
            {
                TabItem tabItemWorkType = new TabItem();
                //克隆UI对象
                res._selectWorkType = this._selectWorkType.Clone();
                tabItemWorkType.Content = res._selectWorkType;
                tabItemWorkType.Header = " 工 种 ";
                res._selectWorkType.Tag = res.tabControl.Items.Count;
                res.tabControl.Items.Add(tabItemWorkType);
                res._selectWorkType.SelectedEvent += (tabControlIndex) =>
                {
                    res._curTabControlIndex = tabControlIndex;
                    //保留单一选择对象操作
                    if (res._selectPerson != null)
                    {
                        res._selectPerson.ClearAllSelectedValue();
                    }
                    if (res._selectDepart != null)
                    {
                        res._selectDepart.ClearAllSelectedValue();
                    }
                };
            }

            if (_isSelectPerson)
            {
                TabItem tabItemPerson = new TabItem();
                //克隆UI对象
                res._selectPerson = this._selectPerson.Clone();
                tabItemPerson.Content = res._selectPerson;
                tabItemPerson.Header = " 人 员 ";
                res._selectPerson.Tag = res.tabControl.Items.Count;
                res.tabControl.Items.Add(tabItemPerson);
                res._selectPerson.SelectedEvent += (tabControlIndex) =>
                {
                    res._curTabControlIndex = tabControlIndex;
                    //保留单一选择对象操作
                    if (res._selectWorkType != null)
                    {
                        res._selectWorkType.ClearAllSelectedValue();
                    }
                    if (res._selectDepart != null)
                    {
                        res._selectDepart.ClearAllSelectedValue();
                    }
                };
            }
            //设置当前的tabControl选择对象
            if (this.CurTabControlIndex != -1)
            {
                res._curTabControlIndex = this.CurTabControlIndex;
                res.tabControl.SelectedIndex = this.CurTabControlIndex;
            }
            return res;
        }

        /// <summary>
        /// 清空所以已选择的对象
        /// </summary>
        public void ClearAllSelectedValue()
        {
            if (_selectDepart != null)
            {
                _selectDepart.ClearAllSelectedValue();
            }
            if (_selectPerson != null)
            {
                _selectPerson.ClearAllSelectedValue();
            }
            if (_selectWorkType != null)
            {
                _selectWorkType.ClearAllSelectedValue();
            }
        }

        /// <summary>
        /// 设置已选择的部门
        /// </summary>
        public void SetSelectedDeparts(string[] names)
        {
            _selectDepart.SetSelectedDeparts(names);
        }

        /// <summary>
        /// 设置已选择的人员
        /// </summary>
        public void SetSelectedPersons(string[] names)
        {
            _selectPerson.SetSelectedPersons(names);
        }

        /// <summary>
        /// 获取选择的部门id
        /// </summary>
        /// <returns></returns>
        public List<int> GetSelectedDepartIds()
        {
            List<int> res = new List<int>();
            if (_selectDepart == null)
            {
                return res;
            }
            //如果无选择项，则查询所有待选项里的条件
            if (_selectDepart.selectedList.Items.Count > 0)
            {
                foreach (var item in _selectDepart.selectedList.Items)
                {
                    res.Add(((UserDepartInfo)item).depart_id);
                }
            }
            else
            {
                foreach (var item in _selectDepart.candidateList.Items)
                {
                    res.Add(((UserDepartInfo)item).depart_id);
                }
            }

            return res;
        }
        
        /// <summary>
        /// 获取选择的工种id
        /// </summary>
        /// <returns></returns>
        public List<int> GetSelectedWorkTypeIds()
        {
            List<int> res = new List<int>();
            if (_selectWorkType == null)
            {
                return res;
            }
            //如果无选择项，则返回空，因为和部门不同，有的人员就是没有工种
            foreach (var item in _selectWorkType.selectedList.Items)
            {
                res.Add(((WorkTypeInfo_ZhouYuanShan)item).work_type_id);
            }
            return res;
        }

        /// <summary>
        /// 获取选择的人员id
        /// </summary>
        /// <returns></returns>
        public List<int> GetSelectedPersonIds()
        {
            List<int> res = new List<int>();
            if (_selectPerson == null)
            {
                return res;
            }
            //如果无选择项，则查询所有待选项里的条件
            if (_selectPerson.selectedList.Items.Count > 0)
            {
                foreach (var item in _selectPerson.selectedList.Items)
                {
                    res.Add(((UserPersonInfo)item).person_id);
                }
            }
            else
            {
                foreach (var item in _selectPerson.candidateList.Items)
                {
                    res.Add(((UserPersonInfo)item).person_id);
                }
            }
            
            return res;
        }

        #endregion

        #region 域服务交互

         //<summary>
         //获取部门信息
         //</summary>
        private void GetDepartRia(Action<List<UserDepartInfo>> callBack)
        {
            ServiceDomDbAcess.ReOpenSever();

            EntityQuery<UserDepartInfo> list = ServiceDomDbAcess.GetSever().GetDepartsInfoQuery();
            //回调异常类
            Action<LoadOperation<UserDepartInfo>> actionCallBack = ErrorHandle<UserDepartInfo>.OnLoadErrorCallBack;
            //异步事件
            LoadOperation<UserDepartInfo> lo = ServiceDomDbAcess.GetSever().Load(list, actionCallBack, null);

            lo.Completed += delegate
            {
                List<UserDepartInfo> departs = new List<UserDepartInfo>();
                //异步获取数据
                foreach (UserDepartInfo ar in lo.Entities)
                {
                    departs.Add(ar);
                }

                if (callBack != null)
                {
                    callBack(departs);
                }
            };
        }

        /// <summary>
        /// 获取与部门相关的工种信息
        /// </summary>
        private void GetWorkTypeRia(Action<List<WorkTypeInfo_ZhouYuanShan>> callBack)
        {
            ServiceDomDbAcess.ReOpenSever();

            EntityQuery<WorkTypeInfo_ZhouYuanShan> list = ServiceDomDbAcess.GetSever().GetWorkTypeInfo_ZhouYuanShanQuery(VmLogin.OperatorDepartIDList.ToArray());
            ///回调异常类
            Action<LoadOperation<WorkTypeInfo_ZhouYuanShan>> actionCallBack = ErrorHandle<WorkTypeInfo_ZhouYuanShan>.OnLoadErrorCallBack;
            ///异步事件
            LoadOperation<WorkTypeInfo_ZhouYuanShan> lo = ServiceDomDbAcess.GetSever().Load(list, actionCallBack, null);

            lo.Completed += delegate
            {
                List<WorkTypeInfo_ZhouYuanShan> workTypes = new List<WorkTypeInfo_ZhouYuanShan>();
                //异步获取数据
                foreach (WorkTypeInfo_ZhouYuanShan ar in lo.Entities)
                {

                    workTypes.Add(ar);
                }

                if (callBack != null)
                {
                    callBack(workTypes);
                }
            };
        }

        /// <summary>
        /// 获取与部门相关的人员信息
        /// </summary>
        private void GetPersonsRia(Action<List<UserPersonInfo>> callBack)
        {
            ServiceDomDbAcess.ReOpenSever();

            EntityQuery<UserPersonInfo> list = ServiceDomDbAcess.GetSever().GetPersonInfo_ZhouYuanShanQuery(VmLogin.OperatorDepartIDList.ToArray());
            ///回调异常类
            Action<LoadOperation<UserPersonInfo>> actionCallBack = ErrorHandle<UserPersonInfo>.OnLoadErrorCallBack;
            ///异步事件
            LoadOperation<UserPersonInfo> lo = ServiceDomDbAcess.GetSever().Load(list, actionCallBack, null);

            lo.Completed += delegate
            {
                List<UserPersonInfo> persons = new List<UserPersonInfo>();
                //异步获取数据
                foreach (UserPersonInfo ar in lo.Entities)
                {
                    persons.Add(ar);
                }
                if (callBack != null)
                {
                    callBack(persons);
                }
            };
        }

        #endregion

    }

    /// <summary>
    /// 选择对象类型
    /// </summary>
    public enum SelectObjType
    {
        None = 0,
        Person = 1,
        Depart = 2,
        WorkType = 3,
    }

}

