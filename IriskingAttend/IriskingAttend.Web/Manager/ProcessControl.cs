/*************************************************************************
** 文件名:   ProcessControl.cs
×× 主要类:   ProcessControl
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   fjf
** 日  期:   2012-05-29
** 修改人:   fjf
** 日  期:    
 * 修改内容： 
** 描  述:   进程管理类
 *            
** 版  本:   1.0.0
** 备  注:  命名及代码编写遵守C#编码规范
**
 * ***********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;

using System.IO; //StreamWriter

namespace IriskingAttend.Web.Manager
{
    public class ProcessControl
    {
        /// <summary>
        /// 启动PSQL数据库的备份
        /// </summary>
        /// <param name="workingDirectory"></param>
        /// <param name="command"></param>
        public static void StartCmd(String workingDirectory, String command)
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            //p.StartInfo.WorkingDirectory = workingDirectory;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            p.StandardInput.WriteLine(command);
            p.StandardInput.WriteLine("exit");
            p.Close();
        }

        /// <summary>
        /// 启动执行应用程序
        /// </summary>
        /// <param name="serName"></param>
        /// <returns></returns>
        public static bool StartIrisServers(string serName)
        {
            bool bRet = false;
            Process pRet = null;
            try
            {
                pRet = Process.Start(serName);
                bRet = true;
            }
            catch (Exception e)
            {
                bRet = false;
            }

            return bRet;
        }
        /// <summary>
        /// 查找当前的任务进程
        /// </summary>
        /// <param name="serName"></param>
        /// <returns></returns>
        public static bool FindActiveServices(string serName)
        {
            bool bRet = false;

            try
            {
                System.Diagnostics.Process[] ShowID = System.Diagnostics.Process.GetProcesses();
                foreach (Process p in ShowID)
                {
                    //LogManage.WriteLog(p.ProcessName);
                    if (p.ProcessName == serName)
                    {
                        return true;
                    }
                }

                bRet = false;
            }
            catch
            {
                bRet = false;
            }

            return bRet;
        }
    }
}