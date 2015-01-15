/*************************************************************************
** 文件名:   DbManager.cs
×× 主要类:   DbManager
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   fjf
** 日  期:   2013-05-29
** 修改人:   fjf
** 日  期:    
 * 修改内容： 
** 描  述:   POSTGRESQL数据库备份
 *            
** 版  本:   1.0.0
** 备  注:  命名及代码编写遵守C#编码规范
**
 * ***********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Threading;
using Irisking.Manager;

namespace IriskingAttend.Web.Manager
{
    /// <summary>
    /// 备份数据库参数数据结构体
    /// </summary>
    public struct BackupParamTime
    {
        /// <summary>
        /// 0: 只备份数据；其它: 完全备份
        /// </summary>
        public int iIntegrity;
        /// <summary>
        /// 0: 正常输出；其它: 压缩
        /// </summary>
        public int iCompress;
        /// <summary>
        /// 备份周期：1-月备份， 2-周备份, 3-每天备份
        /// </summary>
        public int iPeriod;
        /// <summary>
        /// 在周期内的哪一天：月备份：月的哪一天(1-31)--周备份：周几(1-7)--每天备份：此字段将被忽略
        /// </summary>
        public int iSubPeriod;
        /// <summary>
        /// 备份目录
        /// </summary>
        public string sDestPath;
        /// <summary>
        /// 备份时间
        /// </summary>
        public string sBackTime;
    }
    public class DbManager
    {
        #region 变量和属性
        private AutoResetEvent m_cEvent = new AutoResetEvent(false);   //线程事件
        //备份是否压缩和地址
        private string sPath = null;
        private bool bIsCompress = false;

        public string Path
        {
            get { return this.sPath; }
        }
        public bool Compress
        {
            get { return this.bIsCompress; }
        }
        #endregion
        public DbManager()
        {
            this.Init();
        }
        private void Init()
        {
            this.CreateSync();
        }
        /// <summary>
        /// 创建同步线程
        /// </summary>
        /// <returns></returns>
        private bool CreateSync()
        {
            //返回参数
            bool bRet = false;

            //启动新线程
            try
            {
                Thread t = new Thread(new ParameterizedThreadStart(this.DoWorkSync));
                t.IsBackground = true;
                t.Start(this);

                bRet = true;
            }
            catch
            {
                bRet = false;
            }

            return bRet;
        }
        /// <summary>
        /// 线程函数
        /// </summary>
        /// <param name="o"></param>
        public void DoWorkSync(Object o)
        {
            while (this.m_cEvent.WaitOne())
            {
                DbManager db = o as DbManager;
                //获得当前数据库更新列表            
                try
                {
                    this.StartBackup(db.Path,db.Compress);
                }
                catch (Exception e)
                {
                    //nothing
                    //LogManage.ErrLog = "线程异常错误";
                }
            }
        }
        #region 备份数据库
        /// <summary>
        /// 备份当前数据库
        /// </summary>
        /// <param name="backupPath">指定的备份路径</param>
        /// <returns></returns>
        public int BackupDBManager(string backupPath, bool isCompress)
        {
            int iRet = -1;
            //判断是否正在备份
            if (ProcessControl.FindActiveServices("cmd"))
            {
                iRet = 3 ;
                return iRet;
            }
            //设置当前属性
            this.bIsCompress = isCompress;
            this.sPath = backupPath;

            //启动线程
            if (this.m_cEvent.Set())
            {
                iRet = 1;
            }

            return iRet;
        }
        private int StartBackup(string backupPath, bool isCompress)
        {
            int iRet = -1;

            //构建执行的命令
            StringBuilder sbcommand = new StringBuilder();

            //true压缩
            if (isCompress)
            {
                sbcommand.AppendFormat("pg_dump.exe -h localhost -p 5432 -U postgres  -F c -b -v -f {0}  irisApp", @backupPath);
            }
            else
            {
                sbcommand.AppendFormat("pg_dump.exe -h localhost -p 5432 -U postgres  -b -v -f {0}  irisApp", @backupPath);
            }
            String command = sbcommand.ToString();
            //获取pg_dump.exe所在路径
            String appDirecroty = @"C:\";
            ProcessControl.StartCmd(appDirecroty, command);

            return iRet;
        }
        /// <summary>
        /// 自动备份数据库
        /// </summary>
        /// <param name="bt"></param>
        /// <returns></returns>
        public int BackupDBAuto(BackupParamTime bt)
        {
            int iRet = -1;
            return iRet;
        }
        #endregion
    }
}