/*************************************************************************
** 文件名:   VmZhouYuanShanReport.cs
** 主要类:   VmZhouYuanShanReport
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2014-1-10
** 修改人:   
** 日  期:
** 描  述:   VmZhouYuanShanReport,周源山报表
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
using IriskingAttend.Web.ZhouYuanShan;
using IriskingAttend.ZhouYuanShan;


namespace IriskingAttend.ViewModel
{
    public class VmZhouYuanShanReport : BaseViewModel
    {
        public static ReportQueryCondition QueryCondition;

        #region   与页面绑定的属性
        /// <summary>
        /// 个人日报表信息
        /// </summary>
        private BaseViewModelCollection<PersonDayAttend> _personDayAttendZYS;
        public BaseViewModelCollection<PersonDayAttend> PersonDayAttendZYS
        {
            get
            {
                return _personDayAttendZYS; 
            }
            set
            {
                _personDayAttendZYS = value;
                OnPropertyChanged<BaseViewModelCollection<PersonDayAttend>>(() => this.PersonDayAttendZYS);
            }
        }

        /// <summary>
        /// 部门月报表信息
        /// </summary>
        private BaseViewModelCollection<DepartMonthAttend> _departMonthAttendZYS;
        public BaseViewModelCollection<DepartMonthAttend> DepartMonthAttendZYS
        {
            get
            {
                return _departMonthAttendZYS;
            }
            set
            {
                _departMonthAttendZYS = value;
                OnPropertyChanged<BaseViewModelCollection<DepartMonthAttend>>(() => this.DepartMonthAttendZYS);
            }
        }

        /// <summary>
        /// 个人月报表信息
        /// </summary>
        private BaseViewModelCollection<PersonAttendStatistics> _personAttendStatisticsZYS;
        public BaseViewModelCollection<PersonAttendStatistics> PersonAttendStatisticsZYS
        {
            get
            {
                return _personAttendStatisticsZYS;
            }
            set
            {
                _personAttendStatisticsZYS = value;
                OnPropertyChanged<BaseViewModelCollection<PersonAttendStatistics>>(() => this.PersonAttendStatisticsZYS);
            }
        }

        /// <summary>
        /// 部门详单信息
        /// </summary>
        private BaseViewModelCollection<PersonAttendStatistics> _departDetailZYS;
        public BaseViewModelCollection<PersonAttendStatistics> DepartDetailZYS
        {
            get
            {
                return _departDetailZYS;
            }
            set
            {
                _departDetailZYS = value;
                OnPropertyChanged<BaseViewModelCollection<PersonAttendStatistics>>(() => this.DepartDetailZYS);
            }
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数,初始化属性
        /// </summary>
        public VmZhouYuanShanReport()
        {
            //个人日报表信息初始化
            PersonDayAttendZYS = new BaseViewModelCollection<PersonDayAttend>();
            //部门月报表信息
            DepartMonthAttendZYS = new BaseViewModelCollection<DepartMonthAttend>();
            PersonAttendStatisticsZYS = new BaseViewModelCollection<PersonAttendStatistics>();
            DepartDetailZYS = new BaseViewModelCollection<PersonAttendStatistics>();
        }

        #endregion
        
        #region  通过ria连接后台，数据库相关操作

        public string GetDayPersonAttendSql()
        {

            string strSQL = string.Format(@"select ar.*, pricip.principal_name, p.work_type_id from attend_record_all as ar Left join person p on p.person_id=ar.person_id 
                            Left join principal pricip on pricip.principal_id = p.principal_id 
                            where p.delete_time is null and attend_day >= '{0}' and attend_day < '{1}'
                            and ar.leave_type_id < 50", ((DateTime)QueryCondition.BeginTime).ToString("yyyy-MM-dd 00:00:00"), ((DateTime)QueryCondition.BeginTime).ToString("yyyy-MM-dd 23:59:59"));

            //组装sql语句的where条件          
            string sql_where = "";
            if (QueryCondition.DepartIds != null && QueryCondition.DepartIds.Count() > 0)
            {
                sql_where = string.Format(" and ar.depart_id in ( ");
                for (int index = 0; index < QueryCondition.DepartIds.Count(); index++)
                {
                    sql_where += string.Format("{0}, ", QueryCondition.DepartIds[index]);
                }

                sql_where = sql_where.Remove(sql_where.LastIndexOf(","), 1);
                sql_where += ")";
            }
            else//如果没有部门条件，则查询结果为空（为权限考虑）
            {
                sql_where = "and ar.depart_id = -1";
            }
            
            if (QueryCondition.PersonIds != null && QueryCondition.PersonIds.Count() > 0)
            {
                sql_where += string.Format(" and ar.person_id in ( ");
                for (int index = 0; index < QueryCondition.PersonIds.Count(); index++)
                {
                    sql_where += string.Format(" {0}, ", QueryCondition.PersonIds[index]);
                }

                sql_where = sql_where.Remove(sql_where.LastIndexOf(","), 1);
                sql_where += ")";
            }
            else//如果没有人员条件，则查询结果为空（为权限考虑）
            {
                sql_where += "and ar.person_id = -1";
            }

            if (QueryCondition.ClassOrderIds != null && QueryCondition.ClassOrderIds.Count() > 0)
            {
                sql_where = string.Format(" and ar.class_order_id in ( ");
                for (int index = 0; index < QueryCondition.ClassOrderIds.Count(); index++)
                {
                    sql_where += string.Format("{0}, ", QueryCondition.ClassOrderIds[index]);
                }

                sql_where = sql_where.Remove(sql_where.LastIndexOf(","), 1);
                sql_where += ")";
            }           

            if (QueryCondition.WorkTypeIds != null && QueryCondition.WorkTypeIds.Count() > 0)
            {
                sql_where += string.Format(" and p.work_type_id in ( ");
                for (int index = 0; index < QueryCondition.WorkTypeIds.Count(); index++)
                {
                    sql_where += string.Format("{0}, ", QueryCondition.WorkTypeIds[index]);
                }

                sql_where = sql_where.Remove(sql_where.LastIndexOf(","), 1);
                sql_where += ")";
            }

            strSQL += sql_where;

            strSQL += " order by convert_to(ar.depart_name,  E'GBK'), convert_to(ar.name,  E'GBK'), convert_to(ar.work_sn,  E'GBK'), ar.attend_day,ar.in_well_time,ar.out_well_time";
            return strSQL;
        }

        /// <summary>
        /// 异步获取数据库中数据, 获取当日考勤数据
        /// </summary>
        public void GetDayPersonAttendRia()
        {
            try
            {
                //显示等待界面
                WaitingDialog.ShowWaiting();

                ServiceDomDbAcess.ReOpenSever();
                //获得个人日报表信息列表
                EntityQuery<PersonDayAttend> attendList = ServiceDomDbAcess.GetSever().GetDayPersonAttendZhouYuanShanQuery(GetDayPersonAttendSql());

                ///回调异常类
                Action<LoadOperation<PersonDayAttend>> loadDevCallBack = ErrorHandle<PersonDayAttend>.OnLoadErrorCallBack;

                ///异步事件
                LoadOperation<PersonDayAttend> loadOp = ServiceDomDbAcess.GetSever().Load(attendList, loadDevCallBack, null);
                loadOp.Completed += delegate
                {
                    //若DayPersonAttendZYS列表信息未分配内存，则进行内存分配
                    if (PersonDayAttendZYS == null)
                    {
                        PersonDayAttendZYS = new BaseViewModelCollection<PersonDayAttend>();
                    }
                    else
                    {
                        //将列表信息清空
                        PersonDayAttendZYS.Clear();
                    }

                    //异步获取数据，将获取到的信息添加到DayPersonAttendZYS中去
                    foreach (PersonDayAttend item in loadOp.Entities)
                    {
                        PersonDayAttendZYS.Add(item); 
                    }

                    //隐藏等待界面
                    WaitingDialog.HideWaiting();                    
                };
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
                WaitingDialog.HideWaiting();
            }
        }

        /// <summary>
        /// 异步获取数据库中数据, 获取部门月报表考勤数据
        /// </summary>
        public void GetDepartMonthAttendRia()
        {
            try
            {
                //显示等待界面
                WaitingDialog.ShowWaiting();

                ServiceDomDbAcess.ReOpenSever();
                //获得设备信息列表
                EntityQuery<DepartMonthAttend> attendList = ServiceDomDbAcess.GetSever().GetDepartMonthAttendZhouYuanShanQuery((DateTime)QueryCondition.BeginTime, (DateTime)QueryCondition.EndTime, QueryCondition.DepartIds.ToArray());

                ///回调异常类
                Action<LoadOperation<DepartMonthAttend>> loadDevCallBack = ErrorHandle<DepartMonthAttend>.OnLoadErrorCallBack;

                ///异步事件
                LoadOperation<DepartMonthAttend> loadOp = ServiceDomDbAcess.GetSever().Load(attendList, loadDevCallBack, null);
                loadOp.Completed += delegate
                {
                    //若DepartMonthAttendZYS列表信息未分配内存，则进行内存分配
                    if (DepartMonthAttendZYS == null)
                    {
                        DepartMonthAttendZYS = new BaseViewModelCollection<DepartMonthAttend>();
                    }
                    else
                    {
                        //将列表信息清空
                        DepartMonthAttendZYS.Clear();
                    }

                    //异步获取数据，将获取到的信息添加到DayPersonAttendZYS中去
                    foreach (DepartMonthAttend item in loadOp.Entities)
                    {
                        DepartMonthAttendZYS.Add(item);
                    }

                    //隐藏等待界面
                    WaitingDialog.HideWaiting();
                };
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
                WaitingDialog.HideWaiting();
            }
        }

        /// <summary>
        /// 异步获取数据库中数据, 获取个人月报表
        /// </summary>
        public void GetPersonAttendStatisticsRia()
        {
            try
            {
                //显示等待界面
                WaitingDialog.ShowWaiting();

                ServiceDomDbAcess.ReOpenSever();

                //获得个人月报表信息列表
                EntityQuery<PersonAttendStatistics> attendList = ServiceDomDbAcess.GetSever().GetPersonAttendStatisticsQuery((DateTime)QueryCondition.BeginTime, (DateTime)QueryCondition.EndTime,
                                          QueryCondition.DepartIds.ToArray(), QueryCondition.PersonIds.ToArray(), QueryCondition.WorkTypeIds.ToArray(), QueryCondition.ClassOrderIds.ToArray(), (int)QueryCondition.ShowElementType
                                          ,(int)QueryCondition.ReportType);

                ///回调异常类
                Action<LoadOperation<PersonAttendStatistics>> loadDevCallBack = ErrorHandle<PersonAttendStatistics>.OnLoadErrorCallBack;

                ///异步事件
                LoadOperation<PersonAttendStatistics> loadOp = ServiceDomDbAcess.GetSever().Load(attendList, loadDevCallBack, null);
                loadOp.Completed += delegate
                {
                    //若DepartMonthAttendZYS列表信息未分配内存，则进行内存分配
                    if (PersonAttendStatisticsZYS == null)
                    {
                        PersonAttendStatisticsZYS = new BaseViewModelCollection<PersonAttendStatistics>();
                    }
                    else
                    {
                        //将列表信息清空
                        PersonAttendStatisticsZYS.Clear();
                    }

                    //异步获取数据，将获取到的信息添加到DayPersonAttendZYS中去
                    foreach (PersonAttendStatistics item in loadOp.Entities)
                    {
                        PersonAttendStatisticsZYS.Add(item);
                    }

                    //隐藏等待界面
                    WaitingDialog.HideWaiting();
                };
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
                WaitingDialog.HideWaiting();
            }
        }

        /// <summary>
        /// 异步获取数据库中数据, 获取部门详单
        /// </summary>
        public void GetDepartDetailRia()
        {
            try
            {
                //显示等待界面
                WaitingDialog.ShowWaiting();

                ServiceDomDbAcess.ReOpenSever();

                //获得部门详单信息列表
                EntityQuery<PersonAttendStatistics> attendList = ServiceDomDbAcess.GetSever().GetPersonAttendStatisticsQuery((DateTime)QueryCondition.BeginTime, (DateTime)QueryCondition.EndTime,
                                          QueryCondition.DepartIds.ToArray(), QueryCondition.PersonIds.ToArray(), QueryCondition.WorkTypeIds.ToArray(), QueryCondition.ClassOrderIds.ToArray(), (int)QueryCondition.ShowElementType
                                           , (int)QueryCondition.ReportType);

                ///回调异常类
                Action<LoadOperation<PersonAttendStatistics>> loadDevCallBack = ErrorHandle<PersonAttendStatistics>.OnLoadErrorCallBack;

                ///异步事件
                LoadOperation<PersonAttendStatistics> loadOp = ServiceDomDbAcess.GetSever().Load(attendList, loadDevCallBack, null);
                loadOp.Completed += delegate
                {
                    //若DepartMonthAttendZYS列表信息未分配内存，则进行内存分配
                    if (DepartDetailZYS == null)
                    {
                        DepartDetailZYS = new BaseViewModelCollection<PersonAttendStatistics>();
                    }
                    else
                    {
                        //将列表信息清空
                        DepartDetailZYS.Clear();
                    }

                    //异步获取数据，将获取到的信息添加到DayPersonAttendZYS中去
                    foreach (PersonAttendStatistics item in loadOp.Entities)
                    {
                        DepartDetailZYS.Add(item);
                    }

                    //隐藏等待界面
                    WaitingDialog.HideWaiting();
                };
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
                WaitingDialog.HideWaiting();
            }
        }
        
        #endregion 

       
    }
}
