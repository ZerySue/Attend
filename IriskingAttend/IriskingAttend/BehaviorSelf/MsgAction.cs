/*************************************************************************
** 文件名:   MsgAction.cs
×× 主要类:   MsgAction
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   fjf
** 日  期:   2012-11-14
** 修改人:   
** 日  期:
** 描  述:   Action类，用来支持BLEND中的动作事件
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
using System.Windows.Interactivity;

namespace IriskingAttend.ViewModel
{
    public class MsgAction:TriggerAction<DependencyObject>
    {
        public static Action sa = null;
        public MsgAction()
        { 
        }
        protected override void Invoke(object parameter)
        {
            MessageBox.Show("这是一个测试");
        }
      
    }
}
