/*************************************************************************
** 文件名:   ServiceDomDbAcess.cs
×× 主要类:   ServiceDomDbAcess
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   lzc
** 日  期:   2013-4-23
** 修改人:   
** 日  期:
** 描  述:   ServiceDomDbAcess类，提供与服务类
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

namespace IriskingAttend.ViewModel
{
    /// <summary>
    ///  服务类
    /// </summary>
    public static class ServiceDomDbAcess
    {
        /// <summary>
        /// 服务声明
        /// </summary>
        private static DomainServiceIriskingAttend _serviceDomDbAccess = new DomainServiceIriskingAttend();

        static ServiceDomDbAcess()
        { 
        }

        /// <summary>
        /// 获取域服务
        /// </summary>
        /// <returns></returns>
        public static DomainServiceIriskingAttend GetSever()
        {
            return _serviceDomDbAccess;
        }
        /// <summary>
        /// 另起服务
        /// </summary>
        public static void ReOpenSever()
        {
            _serviceDomDbAccess = new DomainServiceIriskingAttend();
        }
    }
}
