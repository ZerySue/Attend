/*************************************************************************
** 文件名:   BackupParameter.cs
×× 主要类:   BackupParameter
**  
** Copyright (c) 中科虹霸有限公司公司
** 创建人:   cty
** 日  期:   2013-07-23
** 修改人:   
** 日  期:
** 描  述:   接收请求的结构体
** 功  能:   接收请求的结构体：用于接收客户端发送的请求
** 版  本:   1.0.0
** 备  注:   命名及代码编写遵守C#编码规范
**
 * ***********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BackUpDatabaseServer
{
    public struct BackupParameter
    {
        /// <summary>
        /// 标志是手动备份还是自动备份，0-手动备份，1-自动备份，2-发送口令，返回设置的自动备份参数
        /// </summary>
        public int ManOrAuto;

        /// <summary>
        /// 要备份的数据库,0-考勤库，1-虹膜库 (只对手动备份有用)
        /// </summary>
        public int DatabaseName;

        /// <summary>
        /// 备份考勤库的路径
        /// </summary>
        public string PathIrisApp;

        /// <summary>
        /// 备份考勤库周期
        /// </summary>
        public int PeriodIrisApp;

        /// <summary>
        /// 备份考勤库日期
        /// </summary>
        public int SubPeriokIrisApp;

        /// <summary>
        /// 备份考勤库具体时间
        /// </summary>
        public DateTime ConcreteTimeIrisApp;

        /// <summary>
        /// 备份虹膜库的路径
        /// </summary>
        public string PathIrisData;

        /// <summary>
        /// 备份虹膜库周期
        /// </summary>
        public int PeriodIrisData;

        /// <summary>
        /// 备份虹膜库日期
        /// </summary>
        public int SubPeriokIrisData;

        /// <summary>
        /// 备份虹膜库具体时间
        /// </summary>
        public DateTime ConcreteTimeIrisData;
    }
}
