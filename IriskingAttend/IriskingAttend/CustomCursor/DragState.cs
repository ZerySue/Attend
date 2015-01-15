/*************************************************************************
** 文件名:   DragState.cs
×× 主要类:   DragState
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   wz
** 日  期:   2013-8-13
** 修改人:   
** 日  期:
** 描  述:   DragState类 鼠标拖拽状态枚举类
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

namespace IriskingAttend.CustomCursor
{
    /// <summary>
    /// 拖拽状态类
    /// </summary>
    public class DragState
    {
        /// <summary>
        /// 拖拽的物体
        /// </summary>
        public DragObject DragObject = DragObject.None;

        private DragPossition _dragPossition = DragPossition.NoDrag;
        /// <summary>
        /// 拖拽状态
        /// </summary>
        public DragPossition DragPossition
        {
            get
            {
                return _dragPossition;
            }
            set
            {
                _dragPossition = value;
            }
        }
        
        /// <summary>
        /// 拖拽开始的位置
        /// </summary>
        public Point DragStartPos;

        /// <summary>
        /// 当前拖拽位置
        /// </summary>
        public Point DragCurPos;
    }

    /// <summary>
    /// 拖拽的物体
    /// </summary>
    public enum DragObject
    {
        None,
        Person,
        Depart,
    }

    /// <summary>
    /// 拖拽的状态
    /// </summary>
    public enum DragPossition
    {
        Draging,
        ReadyToDrag,
        AfterDrag,
        NoDrag,
    }


}
