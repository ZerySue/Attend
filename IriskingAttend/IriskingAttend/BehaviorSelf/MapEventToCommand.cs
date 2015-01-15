/*************************************************************************
** 文件名:   MapEventToCommand.cs
×× 主要类:   MapEventToCommandBase
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   lzc
** 日  期:   2013-5-3
** 修改人:   
** 日  期:
** 描  述:   MapEventToCommandBase类，该类用来自定义事件转换为Command
**
** 版  本:   1.0.0
** 备  注:  命名及代码编写遵守C#编码规范
**
 * ***********************************************************************/
using System;
using System.Windows;
using System.Windows.Interactivity;

using GalaSoft.MvvmLight.Command;
using System.Windows.Input;

namespace MvvmLightCommand.SL4.TriggerActions
{
    #region   注册事件到Command  根据需要可以在这里继续添加

    /// <summary>
    /// 注册EventArgs事件
    /// </summary>
    public class MapEventToCommand : MapEventToCommandBase<EventArgs>{}

    /// <summary>
    /// 注册RoutedEventArgs事件
    /// </summary>
    public class MapRoutedEventToCommand : MapEventToCommandBase<RoutedEventArgs>{}

    /// <summary>
    /// 注册MouseButtonEventArgs事件
    /// </summary>
    public class MouseButtonEventToCommand : MapEventToCommandBase<MouseButtonEventArgs>{ }

    /// <summary>
    /// 注册MouseEventArgs事件
    /// </summary>
    public class MouseEventToCommand : MapEventToCommandBase<MouseEventArgs>  {}

    #endregion

    /// <summary>
    /// 事件转换为Command基类
    /// </summary>
    /// <typeparam name="TEventArgsType"></typeparam>
    public abstract class MapEventToCommandBase<TEventArgsType> : TriggerAction<FrameworkElement>
        where TEventArgsType : EventArgs
    {
        /// <summary>
        /// Command属性注册
        /// </summary>
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), 
            typeof(MapEventToCommandBase<TEventArgsType>), new PropertyMetadata(null, OnCommandPropertyChanged));
       
        /// <summary>
        /// Command参数属性注册
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), 
            typeof(MapEventToCommandBase<TEventArgsType>), new PropertyMetadata(null, OnCommandParameterPropertyChanged));

        /// <summary>
        /// 传参改变时触发事件
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnCommandParameterPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var invokeCommand = d as MapEventToCommand;
            if (invokeCommand != null)
            {
                invokeCommand.SetValue(CommandParameterProperty, e.NewValue);
            }
        }

        /// <summary>
        /// 属性改变时触发事件
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var invokeCommand = d as MapEventToCommand;
            if (invokeCommand != null)
            {
                invokeCommand.SetValue(CommandProperty, e.NewValue);
            }
        }

        /// <summary>
        /// 委托
        /// </summary>
        /// <param name="parameter"></param>
        protected override void Invoke(object parameter)
        {
            if (this.Command == null)
            {
                return;
            }

            var eventInfo = new EventInformation<TEventArgsType>
            {
                EventArgs = parameter as TEventArgsType,
                Sender = this.AssociatedObject,
                CommandArgument = GetValue(CommandParameterProperty)
            };

            if (Command.CanExecute(eventInfo))
            {
                Command.Execute(eventInfo);
            }
        }
        /// <summary>
        /// Command 命令 获取Command
        /// </summary>
        public ICommand Command
        {
            get
            {
                return (ICommand)base.GetValue(CommandProperty);
            }
            set
            {
                base.SetValue(CommandProperty, value);
            }
        }

        /// <summary>
        /// command 传递参数
        /// </summary>
        public object CommandParameter
        {
            get
            {
                return base.GetValue(CommandParameterProperty);
            }
            set
            {
                base.SetValue(CommandParameterProperty, value);
            }
        }
    }

}