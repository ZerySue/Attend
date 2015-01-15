/*************************************************************************
** 文件名:   LogMef.cs
×× 主要类:   LogMef
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   fjf
** 日  期:   2012-11-13
** 修改人:   
** 日  期:
** 描  述:   公用日志类
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
using System.ComponentModel.Composition;

namespace IriskingAttend.Other
{
    public class LogMef
    {
        private string m_msgLog = "这是一个测试";

        /// <summary>
        /// 导出属性
        /// </summary>
        [Export]
        public string MessageLog
        {
            get
            {
                return this.m_msgLog;
            }
            set
            {
                if (null != value)
                {
                    this.m_msgLog = value;
                    this.WriteLog();
                }
            }

        }
        /// <summary>
        /// 写入日志
        /// </summary>
        /// <returns></returns>
        private int WriteLog()
        {
            return 0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Export(typeof(Func<int,int>))]
        public int GetMax(int id)
        {
            return 100;
        }
    }
}
