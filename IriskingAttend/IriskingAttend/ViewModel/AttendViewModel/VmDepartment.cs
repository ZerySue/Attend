/*************************************************************************
** 文件名:   VmDepartment.cs
** 主要类:   VmDepartment
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   lzc
** 日  期:   2013-4-11
** 修改人:   
** 日  期:
** 描  述:   VmDepartment，主要是部门管理
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
using Irisking.Web.DataModel;
using IriskingAttend.Web;
using System.ServiceModel.DomainServices.Client;
using EDatabaseError;
using IriskingAttend.Common;
using System.Collections.Generic;

namespace IriskingAttend.ViewModel
{
    public class VmDepartment : BaseViewModel
    {
        #region 绑定数据

        ///<summary>
        /// 绑定数据源
        /// </summary>
        public BaseViewModelCollection<depart> DepartmentModel { get; set; }


        /// <summary>
        /// 通过用户权限部门过滤人员简单信息
        /// </summary>
        public BaseViewModelCollection<UserPersonSimple> UserPersonSimpleForDepartModelBase { get; set; }


        /// <summary>
        /// 通过部门过滤人员简单信息
        /// </summary>
        public BaseViewModelCollection<UserPersonSimple> UserPersonSimpleForDepartModel { get; set; }

        /// <summary>
        /// 人员简单信息实体
        /// </summary>
        public IEnumerable<UserPersonSimple> UserPersonSimpleModelEntities { get; set; }


        /// <summary>
        /// 通过工号获取简单人员信息
        /// </summary>
        public UserPersonSimple UserPersonSimpleForWorkSN { get; set; }

        /// <summary>
        /// 全选控件绑定
        /// </summary>
        public bool CheckAll
        {
            get { return _checkAll; }
            set
            {
                _checkAll = value;
                OnPropertyChanged(() => CheckAll);
            }
        }

        #endregion

        #region 私有变量

        ///// <summary>
        ///// 部门树形结构
        ///// </summary>
        //private TreeNode m_DepartTreeData;

        /// <summary>
        /// 部门树形结构
        /// </summary>
        //private TreeNode _departTreeData;



        /// <summary>
        /// 全选绑定
        /// </summary>
        private bool _checkAll = true;

        #endregion

        #region 事件

        /// <summary>
        /// 部门加载完成事件
        /// </summary>
        public event EventHandler DepartmentLoadCompleted;

        /// <summary>
        /// 简单人员信息加载完成
        /// </summary>
        public event EventHandler UserPersonSimpleLoadCompleted;

        /// <summary>
        /// 简单人员信息加载完成
        /// </summary>
        public event EventHandler UserPersonSimpleLoadForDepartCompleted;

        /// <summary>
        /// 通过工号查出人员完成
        /// </summary>
        public event EventHandler WorkSNChanged;

        #endregion

        #region 构造函数
        public VmDepartment()
        {
            init();
        }

        private void init()
        {
            //另起服务
            ServiceDomDbAcess.ReOpenSever();

            //初始化
            DepartmentModel = new BaseViewModelCollection<depart>();
            UserPersonSimpleForWorkSN = new UserPersonSimple();
            //UserPersonSimpleModel = new BaseViewModelCollection<UserPersonSimple>();
            UserPersonSimpleForDepartModelBase = new BaseViewModelCollection<UserPersonSimple>();
            UserPersonSimpleForDepartModel = new BaseViewModelCollection<UserPersonSimple>();
            ///事件初始化
            UserPersonSimpleLoadCompleted += (a,e)=>{ };
            WorkSNChanged += (a, e) =>{ };
            DepartmentLoadCompleted += (a, e) => { };
            UserPersonSimpleLoadForDepartCompleted += (a, e) => { };
        }
        #endregion

        #region 函数

        /// <summary>
        /// 异步获取部门信息
        /// </summary>
        public void GetDepartment()
        {
            try
            {
                EntityQuery<depart> lstDepart = ServiceDomDbAcess.GetSever().IrisGetDepartQuery();

                ///回调异常类
                Action<LoadOperation<depart>> getDepartCallBack = new Action<LoadOperation<depart>>(ErrorHandle<depart>.OnLoadErrorCallBack);
                ///异步事件
                LoadOperation<depart> lo = ServiceDomDbAcess.GetSever().Load(lstDepart, getDepartCallBack, null);

                lo.Completed += (o, e) =>
                {
                    DepartmentModel.Clear();
                    depart departAll = new depart { depart_name = "全部", depart_id = 0 };
                    DepartmentModel.Add(departAll);
                    //增加部门权限 lo.Entities.OrderBy(a => a.depart_name);
                    foreach (var ar in lo.Entities.Where(a=> VmLogin.OperatorDepartIDList.Contains(a.depart_id )).OrderBy(a => a.depart_name))
                    {
                        DepartmentModel.Add(ar);
                    }

                    DepartmentLoadCompleted(this, new EventArgs());
                };
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }

        /// <summary>
        /// 获取人员简单信息
        /// </summary>
        public void GetUserPersonSimple()
        {
            try
            {
                if (UserPersonSimpleModelEntities == null || UserPersonSimpleModelEntities.Count() < 1)
                {
                    EntityQuery<UserPersonSimple> lstUserPersonSimple = ServiceDomDbAcess.GetSever().IrisGetUserPersonSimpleForOperatorDepartQuery(VmLogin.OperatorDepartIDList);

                    ///回调异常类
                    Action<LoadOperation<UserPersonSimple>> getUserPersonSimpleCallBack = new Action<LoadOperation<UserPersonSimple>>
                        (ErrorHandle<UserPersonSimple>.OnLoadErrorCallBack);
                    ///异步事件
                    LoadOperation<UserPersonSimple> lo = ServiceDomDbAcess.GetSever().Load(lstUserPersonSimple, getUserPersonSimpleCallBack, null);

                    lo.Completed += (o, e) =>
                    {
                        WaitingDialog.HideWaiting();
                        try
                        {
                            UserPersonSimpleForDepartModel.Clear();
                            UserPersonSimpleForDepartModelBase.Clear();
                            foreach (var ar in lo.Entities)
                            {
                                UserPersonSimpleForDepartModelBase.Add(ar);
                                UserPersonSimpleForDepartModel.Add(ar);
                            }
                            
                            UserPersonSimpleLoadCompleted(this, new EventArgs());
                        }
                        catch (Exception ee)
                        {
                            ErrorWindow err = new ErrorWindow(ee);
                            err.Show();
                        }
                    };
                    WaitingDialog.ShowWaiting("加载人员...");
                }
                else
                {
                    UserPersonSimpleForDepartModelBase.Clear();
                    foreach (var ar in ServiceDomDbAcess.GetSever().UserPersonSimples)
                    {
                        UserPersonSimpleForDepartModelBase.Add(ar);
                    }
                    UserPersonSimpleLoadCompleted(this, new EventArgs());
                }
            }
            catch (Exception e)
            {
                WaitingDialog.HideWaiting();
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }

        /// <summary>
        /// 获取人员简单信息 通过部门过滤人员信息
        /// </summary>
        public void GetUserPersonSimple(int depart_id)
        {
            //没有具体部门时返回
            if (depart_id < 1)
            {
                return;
            }

            try
            {
                //如果RIA中存在人员信息则直接获取，不存在时异步获取
                if (ServiceDomDbAcess.GetSever().UserPersonSimples.Count< 1)
                {
                    GetUserPersonSimple();
                    UserPersonSimpleLoadCompleted += (a,e) =>
                        {
                            UserPersonSimpleForDepartModel.Clear();
                            var values = from u in UserPersonSimpleForDepartModelBase
                                                where u.depart_id == depart_id
                                                orderby u.person_name
                                                select u;
                            foreach (var ar in values)//UserPersonSimpleForDepartModelBase.Where(p => p.depart_id == depart_id))
                            {
                                UserPersonSimpleForDepartModel.Add(ar);
                            }
                            UserPersonSimpleLoadForDepartCompleted(this, new EventArgs());
                        };

                    //EntityQuery<UserPersonSimple> lstUserPersonSimpleForDepart = ServiceDomDbAcess.GetSever().IrisGetUserPersonSimpleForDepartQuery(depart_id);

                    /////回调异常类
                    //Action<LoadOperation<UserPersonSimple>> getUserPersonSimpleForDepartCallBack = new Action<LoadOperation<UserPersonSimple>>
                    //    (ErrorHandle<UserPersonSimple>.OnLoadErrorCallBack);
                    /////异步事件
                    //LoadOperation<UserPersonSimple> lo = ServiceDomDbAcess.GetSever().Load(lstUserPersonSimpleForDepart, getUserPersonSimpleForDepartCallBack, null);

                    //lo.Completed += (o, e) =>
                    //{
                    //    UserPersonSimpleForDepartModel.Clear();
                    //    foreach (var ar in lo.Entities.OrderBy(a => a.person_name))
                    //    {
                    //        UserPersonSimpleForDepartModel.Add(ar);
                    //    }
                    //    //WaitingDialog.HideWaiting();
                    //    UserPersonSimpleLoadCompleted(this, new EventArgs());
                    //};
                    //// WaitingDialog.ShowWaiting("加载人员...");
                }
                else
                {
                    //直接获取
                    UserPersonSimpleForDepartModel.Clear();
                    var values = from u in ServiceDomDbAcess.GetSever().UserPersonSimples.Where(a => a.depart_id == depart_id)
                                 orderby u.person_name
                                 select u;
                    foreach (var ar in values)
                    {
                        UserPersonSimpleForDepartModel.Add(ar);
                    }
                    UserPersonSimpleLoadForDepartCompleted(this, new EventArgs());
                }
            }
            catch (Exception err)
            {
                ErrorWindow errWin = new ErrorWindow(err);
                errWin.Show();
            }
        }
        /// <summary>
        /// 获取人员简单信息 通过部门过滤人员信息
        /// </summary>
        public void GetUserPersonSimple(string work_sn)
        {
            try
            {
                //work_sn 不存在时 返回
                if (work_sn == null || work_sn == "")
                {
                    return;
                }

                //如果RIA中存在人员信息则直接获取，不存在时异步获取
                if (ServiceDomDbAcess.GetSever().UserPersonSimples.Where(a => a.work_sn == work_sn).Count() < 1)
                {
                    Action<InvokeOperation<UserPersonSimple>> getUserPersonSimpleForWorkSNCallBack = CallBackHandleControl<UserPersonSimple>.OnInvokeErrorCallBack;

                    CallBackHandleControl<UserPersonSimple>.m_sendValue = (result) =>
                     {
                         UserPersonSimpleForWorkSN = result;
                         WorkSNChanged(null, null);
                     };
                    ///异步获取
                    ServiceDomDbAcess.GetSever().IrisGetUserPersonSimpleForWorkSN(work_sn, getUserPersonSimpleForWorkSNCallBack, null);
                }
                else
                {
                    //从本地直接获取
                    foreach (var ar in ServiceDomDbAcess.GetSever().UserPersonSimples.Where(a => a.work_sn == work_sn))
                    {
                        UserPersonSimpleForWorkSN = ar;
                        WorkSNChanged(null, null);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }

        /// <summary>
        /// 通过部门Id查询人员基本信息
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        public void GetUserPersonSimple(object o, EventArgs e)
        {
            var cmbBox = o as ComboBox;
            if (cmbBox.SelectedIndex > -1)
            {
                GetUserPersonSimple(((depart)cmbBox.SelectedItem).depart_id);
            }
        }


        /// <summary>
        /// 通过部门Id 人员姓名和人员工号查询人员基本信息
        /// </summary>
        /// <param name="name"></param>
        /// <param name="departId"></param>
        /// <param name="workSn"></param>
        public void GetUserPersonSimple(string name, int departId, string workSn)
        {
            UserPersonSimpleForDepartModel.Clear();
            var values = from u in UserPersonSimpleForDepartModelBase
                         orderby u.depart_name,u.person_name
                         select u;
            foreach (var ar in values.Where(a => ((name != "" && name != null) ? a.person_name.Contains(name) : true)
                && ((workSn != "" && workSn != null) ? a.work_sn == workSn : true)
                && ((departId != 0) ? a.depart_id == departId : true)))
            {
                UserPersonSimpleForDepartModel.Add(ar);
            }
        }

        /// <summary>
        /// 通过部门ID和人员名字获取人员基本信息
        /// </summary>
        /// <param name="name"></param>
        /// <param name="departId"></param>
        public void GetUserPersonSimpleForNameAndDepart(string name, int departId)
        {
            try
            {
                //如果无人员名字 则返回
                if (name == null || name == "")
                {
                    return;
                }

                //如果有部门则增加按部门查询条件 否则无部门过滤条件
                if (departId < 1)
                {
                    UserPersonSimpleForDepartModelBase.Clear();
                    foreach (var ar in ServiceDomDbAcess.GetSever().UserPersonSimples.Where(a => a.person_name.Contains(name)))
                    {
                        UserPersonSimpleForDepartModelBase.Add(ar);
                    }
                }
                else
                {
                    UserPersonSimpleForDepartModelBase.Clear();
                    foreach (var ar in ServiceDomDbAcess.GetSever().UserPersonSimples.Where(a => a.person_name.Contains(name)
                        && a.depart_id == departId))
                    {
                        UserPersonSimpleForDepartModelBase.Add(ar);
                    }
                }
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }

        ///// <summary>
        ///// 对子部门部门名称前增减空格
        ///// </summary>
        ///// <param name="PID">父部门Id</param>
        ///// <param name="tNode">子部门</param>
        ///// <param name="level">层</param>
        //private void CreatDepartTree(int PID, BaseViewModelCollection<TreeNode> tNode, int level)
        //{
        //    //DepartmentModel.Where(a => a.parent_depart_id < 1);
        //    //if (PID < 1)
        //    //{
        //    try
        //    {
        //        foreach (depart ar in DepartmentModel.Where(a => (a.depart_id > 0)
        //            && PID < 0 ? a.parent_depart_id < 0 : a.parent_depart_id == PID))
        //        {
        //            TreeNode pNode = new TreeNode();
        //            pNode.NodeVale = ar;
        //            pNode.Children = new BaseViewModelCollection<TreeNode>();

        //            CreatDepartTree(ar.depart_id, pNode.Children, level + 1);
        //            tNode.Add(pNode);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        ErrorWindow err = new ErrorWindow(e);
        //        err.Show();
        //    }
        //}

        #endregion
    }

    #region 辅助类
    /// <summary>
    /// 部门树形结构定义
    ///// </summary>
    //public class TreeNode : Entity // BaseViewModel
    //{
    //    //伸缩资源图标
    //    public static Uri[] ImgSources = new Uri[]{
    //        new Uri("/IriskingAttend;component/Images/bullet-minus.png", UriKind.Relative) ,     // 收缩
    //        new Uri("/IriskingAttend;component/Images/bullet-plus.png", UriKind.Relative),      // 展开
    //        new Uri("/IriskingAttend;component/Images/directly.png", UriKind.Relative) ,     // 直属
    //        new Uri("/IriskingAttend;component/Images/indirectly.png", UriKind.Relative),      // 非直属
    //    };
    //    public depart NodeVale { get; set; }
    //    public String DepartName
    //    {
    //        get
    //        {
    //            return NodeVale.depart_name;
    //        }
    //    }
    //    public BaseViewModelCollection<TreeNode> Children { get; set; }
    //}

    #endregion
}
