/*************************************************************************
** 文件名:   LogManage.cs
×× 主要类:   LogManage
**  
** Copyright (c) 中科虹霸有限公司公司
** 创建人:   cty
** 日  期:   2013-07-23
** 修改人:   
** 日  期:
** 描  述:   日志类
** 功  能:   日志类：主要用于记录日志信息
** 版  本:   1.0.0
** 备  注:   命名及代码编写遵守C#编码规范
**
 * ***********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BackUpDatabaseServer
{
    class LogManage
    {
        #region 私有变量声明
        /// <summary>
        /// 日志存储路径
        /// </summary>
        private string _toFileNmae;
        #endregion

        #region  构造函数
        public LogManage()
        {
            string str = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            this._toFileNmae = str;
        }
        #endregion

        #region 创建日志，写日志函数
        /// <summary>
        /// 创建日志，写日志函数
        /// </summary>
        /// <param name="message">日志信息</param>
        /// <param name="title">日志标题</param>
        public void WriteLog(string strLog)
        {
            string path = this._toFileNmae + "Log\\";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string time = String.Format("{0:yyyyMMdd}.txt", DateTime.Now);
            string filename = path + "BackUpServerLog-" + time;
            string cont = "";
            FileInfo fileInf = new FileInfo(filename);
            if (File.Exists(filename))//如何文件存在 则在文件后面累加
            {
                FileStream myFss = new FileStream(filename, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                StreamReader r = new StreamReader(myFss);
                cont = r.ReadToEnd();
                r.Close();
                myFss.Close();
            }

            #region 生成文件日志
            FileStream myFs = new FileStream(filename, FileMode.Create,FileAccess.ReadWrite,FileShare.ReadWrite);
            StreamWriter n = new StreamWriter(myFs);
            n.WriteLine(cont);         
            n.WriteLine("时间：" + DateTime.Now.ToString());
            n.WriteLine("信息：" + strLog);
            n.Close();
            myFs.Close();
            #endregion
        }
        #endregion
    }
}
