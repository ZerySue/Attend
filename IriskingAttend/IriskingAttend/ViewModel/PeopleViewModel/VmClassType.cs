/*************************************************************************
** 文件名:   VmClassType.cs
** 主要类:   VmClassType
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   cty
** 日  期:   2013-5-15
** 修改人:   
** 日  期:
** 描  述:   VmClassOrder，主要是班制管理用于支持报表
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
using Irisking.Web.DataModel;
using System.ServiceModel.DomainServices.Client;
using EDatabaseError;
using IriskingAttend.Common;

namespace IriskingAttend.ViewModel.PeopleViewModel
{
    /// <summary>
    /// VmClassOrder，主要是班制管理用于支持报表
    /// </summary>
    public class VmClassType
    {

        #region 与界面绑定属性
        
        public BaseViewModelCollection<UserClassTypeInfo> classTypeModel { get; set; }

        #endregion

        //vm加载完毕事件
        public event EventHandler LoadCompletedEvent;

        #region 构造函数
        public VmClassType()
        {
            classTypeModel = new BaseViewModelCollection<UserClassTypeInfo>();
        }
        #endregion

        #region wcf ria操作
        /// <summary>
        /// 异步获取班制信息
        /// </summary>
        public void GetClassType()
        {
            try
            {
                EntityQuery<UserClassTypeInfo> list = ServiceDomDbAcess.GetSever().GetClassTypeInfosQuery();

                ///回调异常类
                Action<LoadOperation<UserClassTypeInfo>> actionCallBack = new Action<LoadOperation<UserClassTypeInfo>>(ErrorHandle<UserClassTypeInfo>.OnLoadErrorCallBack);
                ///异步事件
                LoadOperation<UserClassTypeInfo> lo = ServiceDomDbAcess.GetSever().Load(list, actionCallBack, null);

                lo.Completed += delegate
                {
                    classTypeModel.Clear();
                    UserClassTypeInfo classtypeAll = new UserClassTypeInfo { class_type_id = 0, class_type_name = "全部班制" };
                    classTypeModel.Add(classtypeAll);
                    foreach (var ar in lo.Entities)
                    {
                        classTypeModel.Add(ar); 
                    }
                    //延迟绑定
                    if (LoadCompletedEvent != null)
                    {
                        LoadCompletedEvent(this, null);
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

        #endregion
        
    }
}
