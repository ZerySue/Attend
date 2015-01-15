/*************************************************************************
** 文件名:   Samples.cs
** 主要类:   Samples
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   lzc
** 日  期:   2013-4-7
** 修改人:   
** 日  期:
** 描  述:   例程，如何访问数据库
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
using System.ServiceModel.DomainServices.Client;
using Irisking.Web.DataModel;
using EDatabaseError;

namespace IriskingAttend.ViewModel.AttendViewModel
{
    public class Samples
    {
        /// <summary>
        /// 与服务声明
        /// </summary>
        private DomainServiceIriskingAttend m_serviceDomDbAccess = new DomainServiceIriskingAttend();
        /// <summary>
        /// 绑定数据源
        /// </summary>
        public BaseViewModelCollection<attend_record_base> ModelTest { get; set; }

        public Samples()
        {
            ///初始化
            ModelTest = new BaseViewModelCollection<attend_record_base>();
            GetMyTest();
        }

        /// <summary>
        /// 异步获取数据库中数据
        /// </summary>
        private void GetMyTest()
        {
            EntityQuery<attend_record_base> list = m_serviceDomDbAccess.IrisTestQuery(" select * from attend_record_base");
            ///回调异常类
            Action<LoadOperation<attend_record_base>> actionCallBack = new Action<LoadOperation<attend_record_base>>(ErrorHandle<attend_record_base>.OnLoadErrorCallBack);
            ///异步事件
            LoadOperation<attend_record_base> lo = this.m_serviceDomDbAccess.Load(list, actionCallBack, null);

            lo.Completed += delegate
            {
                this.ModelTest.Clear();
                try
                {
                    //异步获取数据
                    foreach (attend_record_base ar in lo.Entities)
                    {
                        this.ModelTest.Add(ar);
                    }
                }
                catch (Exception e)
                {
                    string s = e.ToString();
                }

            };
        }
    }
}
