/*************************************************************************
** 文件名:   ArgumentExceptionGuard.cs
×× 主要类:   ArgumentExceptionGuard
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   lzc
** 日  期:   2013-4-8
** 修改人:   
** 日  期:
** 描  述:   ArgumentExceptionGuard类，用来支持多参传值
**
** 版  本:   1.0.0
** 备  注:  命名及代码编写遵守C#编码规范
**
 * ***********************************************************************/

using System;
using System.Diagnostics;
using System.Globalization;

namespace IriskingAttend.BehaviorSelf
{
    /// <summary>
    /// 校验参数是否为null 或者 为空
    /// </summary>
    internal static class ArgumentExceptionGuard
    {
        private const string _parameterCannotBeNullOrEmpty = "参数'{0}'不能为null or empty";

        ///判断参数不为空
        //[DebuggerStepThrough]
        public static void ArgumentNotNull<T>(T value, string parameterName) where T : class
        {
            if (value == null)
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        /// <summary>
        /// 判断value是否为null或空
        /// </summary>
        /// <param name="value">属性值</param>
        /// <param name="parameterName">属性名</param>
        public static void ArgumentNotNullOrWhiteSpace(string value, string parameterName)
        {
            ArgumentNotNullOrWhiteSpace(value, parameterName, string.Format(CultureInfo.CurrentCulture,
                    _parameterCannotBeNullOrEmpty, parameterName));
        }

        /// <summary>
        /// 判断value是否为null或空
        /// </summary>
        /// <param name="value">属性值</param>
        /// <param name="parameterName">属性名</param>
        /// <param name="message">信息</param>
        public static void ArgumentNotNullOrWhiteSpace(string value, string parameterName, string message)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(message, parameterName);
            }
        }

        /// <summary>
        /// 是否抛异常
        /// </summary>
        /// <param name="throwException">是否异常</param>
        /// <param name="parameterName">属性名</param>
        /// <param name="message">信息</param>
        public static void ArgumentValue(bool throwException, string parameterName, string message)
        {
            if (throwException)
            {
                throw new ArgumentException(message, parameterName);
            }
        }

        /// <summary>
        /// 判断数据格式
        /// </summary>
        /// <param name="throwException"> 是否异常</param>
        /// <param name="parameterName">属性名</param>
        /// <param name="messageFormat">格式</param>
        /// <param name="messageArgs">信息</param>
        public static void ArgumentValue(bool throwException, string parameterName, string messageFormat, params object[] messageArgs)
        {
            ArgumentValue(throwException, parameterName, string.Format(CultureInfo.CurrentCulture, messageFormat, messageArgs));
        }
    }
}
