/*************************************************************************
** 文件名:   EventInformation.cs
×× 主要类:   EventInformation
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   lzc
** 日  期:   2013-5-3
** 修改人:   
** 日  期:
** 描  述:   EventInformation类，自定义事件类（即将多个参数合并，sender eventArgs等）
**
** 版  本:   1.0.0
** 备  注:  命名及代码编写遵守C#编码规范
**
 * ***********************************************************************/

using System;
using System.Windows;

namespace MvvmLightCommand.SL4.TriggerActions
{
    public class EventInformation<TEventArgsType>
    {
        /// <summary>
        /// 事件发送者
        /// </summary>
        public object Sender { get; set; }

        /// <summary>
        /// 响应事件
        /// </summary>
        public TEventArgsType EventArgs { get; set; }

        /// <summary>
        /// 参数
        /// </summary>
        public object CommandArgument { get; set; }
    }

}
