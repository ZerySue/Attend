/*************************************************************************
** 文件名:   ChildWnd_OperatePerson.cs
×× 主要类:   ChildWnd_OperatePerson
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   cty
** 日  期:   2013-6-14
** 修改人:   wz 代码优化
** 日  期:
** 描  述:   ChildWnd_OperatePerson类,增删改查人员信息页面
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
using IriskingAttend.ViewModelMine.PeopleViewModel;
using IriskingAttend.BehaviorSelf;
using Irisking.Web.DataModel;

namespace IriskingAttend.ViewMine.PeopleView
{
    /// <summary>
    /// 人员管理页面的子窗口
    /// </summary>
    public partial class ChildWnd_OperatePerson_Mine : ChildWindow
    {
        Action<bool?> callBack;

        public ChildWnd_OperatePerson_Mine(UserPersonInfo personInfo, ChildWndOptionMode opMode, Action<bool?> _callBack,int targetDepartId =0)
        {
            InitializeComponent();
            var vmModel = new VmChildWndOperatePerson_Mine(opMode, personInfo, targetDepartId);
            vmModel.LoadCompletedEvent += new EventHandler(vmModel_LoadCompletedEvent);
            vmModel.CloseEvent += new Action<bool>(vmModel_CloseEvent);
            callBack = _callBack;
           
            
        }

        //窗口关闭事件
        private void vmModel_CloseEvent(bool obj)
        {
            DialogResult = obj;
            if (callBack != null)
            {
                callBack(this.DialogResult);
            }
        }

        //vm加载完毕事件
        private  void vmModel_LoadCompletedEvent(object sender, EventArgs e)
        {
            this.DataContext = sender;
        }


    }
}

