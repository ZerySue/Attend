/*************************************************************************
** 文件名:   VmXlsFilter.cs
** 主要类:   VmXlsFilter
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   cty
** 日  期:   2013-8-30
** 修改人:   
** 日  期:
** 描  述:   VmXlsFilter，报表查询条件
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
using IriskingAttend.ViewModel;
using Irisking.Web.DataModel;
using IriskingAttend.Web;
using System.ServiceModel.DomainServices.Client;
using EDatabaseError;
using IriskingAttend.View;

namespace IriskingAttend.NewWuHuShan
{
    public class VmXlsFilter:BaseViewModel
    {
        #region 变量
        /// <summary>
        /// 域服务声明
        /// </summary>
        private DomainServiceIriskingAttend _serviceDomDbAccess = new DomainServiceIriskingAttend();

        /// <summary>
        /// 全选按钮的绑定类
        /// </summary>
        public MarkObject MarkObj
        {
            get;
            set;
        }

        #endregion

        #region 与界面绑定属性

        /// <summary>
        /// 部门
        /// </summary>
        public BaseViewModelCollection<UserDepartInfo> DepartInfoModel
        {
            get;
            set;
        }

        /// <summary>
        /// 工种
        /// </summary>
        public BaseViewModelCollection<WorkTypeInfo> WorkTypeModel
        { 
            get;
            set; 
        }

        /// <summary>
        /// 职务
        /// </summary>
        public BaseViewModelCollection<PrincipalInfo> PrincipalInfoModel
        {
            get;
            set;
        }

        #endregion

        #region 部门、职务、工种的加载完成

        /// <summary>
        /// 部门加载完成
        /// </summary>
        public event EventHandler DepartLoadCompleted;

        /// <summary>
        /// 工种加载完成
        /// </summary>
        public event EventHandler WorkTypeLoadCompleted;

        /// <summary>
        /// 职务加载完成
        /// </summary>
        public event EventHandler PrincipalInfoLoadCompleted;

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public VmXlsFilter()
        {
            MarkObj = new MarkObject();
            DepartInfoModel = new BaseViewModelCollection<UserDepartInfo>();
            WorkTypeModel = new BaseViewModelCollection<WorkTypeInfo>();
            PrincipalInfoModel = new BaseViewModelCollection<PrincipalInfo>();
            DepartLoadCompleted += (a, e) => { };
            WorkTypeLoadCompleted += (a, e) => { };
            PrincipalInfoLoadCompleted += (a, e) => { };
        }

        #endregion

        #region 获取部门信息        

        /// <summary>
        /// 异步获取部门信息
        /// </summary>
        public void GetDepartmentByPrivilege()
        {
            try
            {
                ServiceDomDbAcess.ReOpenSever();

                EntityQuery<UserDepartInfo> lstDepart = ServiceDomDbAcess.GetSever().GetDepartInfoQuery();
                ///回调异常类
                Action<LoadOperation<UserDepartInfo>> getDepartCallBack = new Action<LoadOperation<UserDepartInfo>>(ErrorHandle<UserDepartInfo>.OnLoadErrorCallBack);
                ///异步事件
                LoadOperation<UserDepartInfo> lo = ServiceDomDbAcess.GetSever().Load(lstDepart, getDepartCallBack, null);

                lo.Completed += (o, e) =>
                {
                    DepartInfoModel.Clear();
                    UserDepartInfo departAll = new UserDepartInfo { depart_name = "全部", depart_id = 0 };
                    DepartInfoModel.Add(departAll);
                    //增加部门权限 lo.Entities.OrderBy(a => a.depart_name);

                    if (VmLogin.GetUserID() <= 0)
                    {
                        foreach (var ar in lo.Entities)
                        {
                            ar.isSelected = false;
                            DepartInfoModel.Add(ar);
                        }
                    }
                    else
                    {
                        foreach (var ar in lo.Entities.Where(a => VmLogin.OperatorDepartIDList.Contains(a.depart_id)).OrderBy(a => a.depart_name))
                        {
                            ar.isSelected = false;
                            DepartInfoModel.Add(ar);
                        }
                    }

                    DepartLoadCompleted(this, new EventArgs());
                };
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }

        #endregion

        #region 获取工种信息

        /// <summary>
        /// 异步获取工种信息
        /// </summary>
        public void GetWorkType()
        {
            try
            {
                ServiceDomDbAcess.ReOpenSever();

                EntityQuery<WorkTypeInfo> list = ServiceDomDbAcess.GetSever().GetWorkTypeInfoQuery();
                //回调异常类
                Action<LoadOperation<WorkTypeInfo>> actionCallBack = new Action<LoadOperation<WorkTypeInfo>>(ErrorHandle<WorkTypeInfo>.OnLoadErrorCallBack);
                //异步事件
                LoadOperation<WorkTypeInfo> lo = ServiceDomDbAcess.GetSever().Load(list, actionCallBack, null);

                lo.Completed += delegate
                {
                    WorkTypeModel.Clear();
                    WorkTypeInfo worktypeAll = new WorkTypeInfo { work_type_id = 0, work_type_name = "全部" };
                    WorkTypeModel.Add(worktypeAll);
                    foreach (var ar in lo.Entities)
                    {
                        ar.isSelected = false;
                        WorkTypeModel.Add(ar);
                    }
                    WorkTypeLoadCompleted(this, new EventArgs());
                };
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }

        #endregion

        #region 获取职务信息

        /// <summary>
        /// 异步获取职务信息
        /// </summary>
        public void GetPrincipalInfo()
        {
            try
            {
                ServiceDomDbAcess.ReOpenSever();

                EntityQuery<PrincipalInfo> list = ServiceDomDbAcess.GetSever().GetPrincipalInfoQuery();
                //回调异常类
                Action<LoadOperation<PrincipalInfo>> actionCallBack = new Action<LoadOperation<PrincipalInfo>>(ErrorHandle<PrincipalInfo>.OnLoadErrorCallBack);
                //异步事件
                LoadOperation<PrincipalInfo> lo = ServiceDomDbAcess.GetSever().Load(list, actionCallBack, null);

                lo.Completed += delegate
                {
                    PrincipalInfoModel.Clear();
                    PrincipalInfo worktypeAll = new PrincipalInfo { principal_id = 0, principal_name = "全部" };
                    PrincipalInfoModel.Add(worktypeAll);
                    foreach (var ar in lo.Entities)
                    {
                        ar.isSelected = false;
                        PrincipalInfoModel.Add(ar);
                    }
                    PrincipalInfoLoadCompleted(this, new EventArgs());
                };
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }

        #endregion

        #region 部门的选中与全选操作

        /// <summary>
        /// 部门全选操作
        /// </summary>
        /// <param name="isChecked"></param>
        public void SelectAllDepart(bool? isChecked)
        {
            if (!isChecked.HasValue)
            {
                return;
            }
            if (isChecked.Value)
            {
                foreach (var item in DepartInfoModel)
                {
                    item.isSelected = true;
                }
            }
            else
            {
                foreach (var item in DepartInfoModel)
                {
                    item.isSelected = false;
                }
            }
        }

        /// <summary>
        /// 按需选择datagrid的Items(职务)
        /// </summary>
        /// <param name="sender">datagrid对象</param>
        public void SelectItemDepart(UserDepartInfo info)
        {
            info.isSelected = !info.isSelected;
            this.MarkObj.Selected = CheckDepartIsAllSelected();
        }

        /// <summary>
        /// 检查Item（部门）是否全部被选中
        /// </summary>
        private bool CheckDepartIsAllSelected()
        {
            if (DepartInfoModel.Count == 0)
            {
                return false;
            }
            foreach (var item in DepartInfoModel)
            {
                if (!item.isSelected)
                {
                    return false;
                }
            }
            return true;

        }

        #endregion

        #region 职务的选中与全选操作

        /// <summary>
        /// 职务全选操作
        /// </summary>
        /// <param name="isChecked"></param>
        public void SelectAllPrincipal(bool? isChecked)
        {
            if (!isChecked.HasValue)
            {
                return;
            }
            if (isChecked.Value)
            {
                foreach (var item in PrincipalInfoModel)
                {
                    item.isSelected = true;
                }
            }
            else
            {
                foreach (var item in PrincipalInfoModel)
                {
                    item.isSelected = false;
                }
            }
        }

        /// <summary>
        /// 按需选择datagrid的Items(职务)
        /// </summary>
        /// <param name="sender">datagrid对象</param>
        public void SelectItemPrincipal(PrincipalInfo info)
        {
            info.isSelected = !info.isSelected;
            this.MarkObj.Selected = CheckPrincipalIsAllSelected();
        }

        /// <summary>
        /// 检查Item（职务）是否全部被选中
        /// </summary>
        private bool CheckPrincipalIsAllSelected()
        {
            if (PrincipalInfoModel.Count == 0)
            {
                return false;
            }
            foreach (var item in PrincipalInfoModel)
            {
                if (!item.isSelected)
                {
                    return false;
                }
            }
            return true;

        }

        #endregion

        #region 工种的选中与全选操作

        /// <summary>
        /// 工种全选操作
        /// </summary>
        /// <param name="isChecked"></param>
        public void SelectAllWorkType(bool? isChecked)
        {
            if (!isChecked.HasValue)
            {
                return;
            }
            if (isChecked.Value)
            {
                foreach (var item in WorkTypeModel)
                {
                    item.isSelected = true;
                }
            }
            else
            {
                foreach (var item in WorkTypeModel)
                {
                    item.isSelected = false;
                }
            }
        }

        /// <summary>
        /// 按需选择datagrid的Items(工种)
        /// </summary>
        /// <param name="sender">datagrid对象</param>
        public void SelectItemWorkType(WorkTypeInfo info)
        {
            info.isSelected = !info.isSelected;
            this.MarkObj.Selected = CheckWorkTypeIsAllSelected();
        }

        /// <summary>
        /// 检查Item（工种）是否全部被选中
        /// </summary>
        private bool CheckWorkTypeIsAllSelected()
        {
            if (WorkTypeModel.Count == 0)
            {
                return false;
            }
            foreach (var item in WorkTypeModel)
            {
                if (!item.isSelected)
                {
                    return false;
                }
            }
            return true;

        }

        #endregion
    }

    #region 选择条件

    /// <summary>
    /// 报表查询条件
    /// </summary>
    public class XlsQueryCondition
    {
        /// <summary>
        /// 起始时间
        /// </summary>
        public DateTime XlsBeginTime { get; set; }

        /// <summary>
        /// 截止时间
        /// </summary>
        public DateTime XlsEndTime { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        public int[] XlsDepartIdLst { get; set; }

        /// <summary>
        /// 考勤类型
        /// </summary>
        public int[] XlsAttendTypeIdLst { get; set; }

        /// <summary>
        /// 班制
        /// </summary>
        public int XlsClassTypeName { get; set; }

        /// <summary>
        /// 考勤班次
        /// </summary>
        public string XlsClassOrderName { get; set; }

        /// <summary>
        /// 人员姓名
        /// </summary>
        public string XlsName { get; set; }

        /// <summary>
        /// 工号
        /// </summary>
        public string XlsWorkSN { get; set; }

        /// <summary>
        /// 职务
        /// </summary>
        public int[] XlsPrincipalId { get; set; }

        /// <summary>
        /// 工种
        /// </summary>
        public int[] XlsWorkTypeId { get; set; }

        /// <summary>
        /// 工作时长
        /// </summary>
        public int XlsWorkTime { get; set; }

        /// <summary>
        /// 工作时长大于
        /// </summary>
        public int XlsWorkTimeMore { get; set; }

        /// <summary>
        /// 工作时长等于
        /// </summary>
        public int XlsWorkTimeEqual { get; set; }

        /// <summary>
        /// 工作时长小于
        /// </summary>
        public int XlsWorkTimeLess { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public XlsQueryCondition()
        {
            XlsBeginTime = DateTime.MinValue;
            XlsEndTime = DateTime.Now;
            XlsDepartIdLst = null;
            XlsAttendTypeIdLst = null;
            XlsName = "";
            XlsWorkSN = "";
            XlsClassTypeName = -1;
            XlsClassOrderName = "";
            XlsPrincipalId = null;
            XlsWorkTypeId = null;
            XlsWorkTime = 0;
        }
    }

    #endregion

}
