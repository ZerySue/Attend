/*************************************************************************
** 文件名:   ChildWnd_OperateDepart.cs
×× 主要类:   ChildWnd_OperateDepart
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   cty
** 日  期:   2013-6-14
** 修改人:   
** 日  期:
** 描  述:   ChildWnd_OperateDepart类,增删改部门页面
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
using IriskingAttend.ViewModel.PeopleViewModel;
using Irisking.Web.DataModel;
using IriskingAttend.BehaviorSelf;

namespace IriskingAttend.View.PeopleView
{
    /// <summary>
    /// 操作部门界面UI后台
    /// </summary>
    public partial class ChildWnd_OperateDepart : ChildWindow
    {
        #region 字段声明
        
        Action<bool?> callBack;

        #endregion

        #region 构造函数
       
        public ChildWnd_OperateDepart(UserDepartInfo dInfo, ChildWndOptionMode mode, Action<bool?> _callBack)
        {
            InitializeComponent();
            VmChildWndOperateDepart vm = new VmChildWndOperateDepart( dInfo, mode);
         
            vm.CloseEvent += new Action<bool>(vm_CloseEvent);
            vm.LoadCompletedEvent += new EventHandler(vm_LoadCompletedEvent);
            callBack = _callBack;
            
        }

        #endregion

        #region 界面事件
        
        //vm加载完毕事件
        void vm_LoadCompletedEvent(object sender, EventArgs e)
        {
            this.DataContext = sender;
        }

        //窗口关闭事件
        void vm_CloseEvent(bool obj)
        {
            DialogResult = obj;
            if (callBack != null)
            {
                callBack(this.DialogResult);
            }
        }

        #endregion


    }
}

