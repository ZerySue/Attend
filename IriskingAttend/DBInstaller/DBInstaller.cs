/*************************************************************************
** 文件名:   DBInstaller.cs
×× 主要类:   DBInstaller
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   
** 日  期:   
** 修改人:   lzc
** 日  期:   2013-8-12
** 修改内容： 增加IIS的安装、web.congif文件的配置，卸载相关配置、卸载、站点的启用和删除、创建开始菜单快捷方式等
** 描  述:   DBInstaller类，新考勤安装包程序
**
** 版  本:   1.0.0
** 备  注:  命名及代码编写遵守C#编码规范
**
 * ***********************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Reflection;
using System.Text;
using System.Xml;
using System.DirectoryServices;
using System.Runtime;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.ServiceProcess;
using Microsoft.Win32;
using System.Threading;

namespace DBInstaller
{
    [RunInstaller(true)]
    public partial class DBInstaller : Installer
    {
        #region 私有变量

        //数据库路径
        private string _dbconfigPath = string.Empty;
        //w3svc位置
        private string _entPath = String.Format("IIS://{0}/w3svc", "localhost");

        //站点端口号
        private string _newSiteNum = string.Empty;

        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public DBInstaller()
        {

            InitializeComponent();
        }

        #endregion

        #region 重载函数

        /// <summary>
        /// 增加自定义安装
        /// </summary>
        /// <param name="savedState"></param>
        protected override void OnBeforeInstall(IDictionary savedState)
        {
            base.OnBeforeInstall(savedState);
            string webconfigpath = Path.Combine(this.Context.Parameters["installdir"].ToString(), "Web.config");
            //string configPath  =Path.Combine( Directory.GetCurrentDirectory(),"Install.config");
            //默认安装为矿单位安装----如果非矿 则IsMine = "false";
            //string IsMine = "false"; //将矿与非矿的区别直接在配置文件中改变 --by gqy 2014-1-17
            //#if RELEASE 
            //            MessageBox.Show("RELEASENEWATTEND  IsMine = TRUE" , "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //            IsMine = "True"; 
            //#else
            //            IsMine = "False";
            //#endif

            try
            {
                //设置web.config文件 安装参数
                string webcofnigstring = File.ReadAllText(webconfigpath).Replace("#constring#",
                    GetConnectionString(Context.Parameters["dbname"].ToString()));
                webcofnigstring = webcofnigstring.Replace("#serverport#", Context.Parameters["serverport"].ToString());
                webcofnigstring = webcofnigstring.Replace("#serverip#", Context.Parameters["serverip"].ToString());
                //webcofnigstring = webcofnigstring.Replace("#ismine#", IsMine);  //将矿与非矿的区别直接在配置文件中改变 --by gqy 2014-1-17
                File.WriteAllText(webconfigpath, webcofnigstring);

                //为卸载存储相关信息
                WriteMessageForUnInstall();
            }
            catch (Exception e)
            {
                MessageBox.Show("File.ReadAllText" + e.Message, "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            //xp或server2003需要自行安装IIS
            if (!IsXPOr2003())
            {
                //如果不存在IIS 则安装IIS
                if (string.Empty == GetIISVerstion())
                {
                    IISInstall(this.Context.Parameters["installdir"].ToString(),
                        this.Context.Parameters["installdir"].ToString() + "iis.txt");
                    //asp.net 注册IIS 
                    AspnetRegIIS();
                }
            }
        }

        /// <summary>
        /// 增加自定义卸载安装
        /// </summary>
        /// <param name="savedState"></param>
        protected override void OnBeforeUninstall(IDictionary savedState)
        {
            //卸载之前先关闭备份数据库进程
            KillProcess("BackUpDatabaseServer");
        }

        /// <summary>
        /// 安装
        /// </summary>
        /// <param name="stateSaver"></param>
        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);

            if (IsExistSiteName(Context.Parameters["sitename"].ToString()))
            {
                //如果存在站点则删除该站点
                DeleteWebSite(Context.Parameters["sitename"].ToString());
                //MessageBox.Show("站点名称重复。", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //this.Rollback(stateSaver);
                // return;
            }

            if (IsExistSitePort())
            {
                MessageBox.Show("站点端口号重复。", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Rollback(stateSaver);
                return;
            }

            try
            {
                //创建快捷方式
                CreateDeskTopShortcut();
                CreateProgramsShortcut();

               
                Thread.Sleep(2000);
                string s = Context.Parameters["installdir"].ToString().Replace("\\\\", "");

                NewWebSiteInfo siteInfo = new NewWebSiteInfo(string.Empty, Context.Parameters["siteport"].ToString(), "",
                    Context.Parameters["sitename"].ToString(), s);

                CreateNewWebSite(siteInfo);
                Thread.Sleep(2000);
                StartWebSiteByNum(siteInfo.BindString);

                 //MessageBox.Show("数据库备份开始！\n", "出错！");
                 Register reg = new Register();
                 reg.SetStartWithWindow("BackUpDatabaseServer", Path.Combine(s,"数据库备份\\BackUpDatabaseServer.exe"));
                 //MessageBox.Show("数据库备份结束！\n", "出错！");

                 UpdateDataBase(this.Context.Parameters["installdir"].ToString());

            }
            catch (Exception ex)
            {
                MessageBox.Show("创建站点失败！\n" + ex.Message, "出错！");
                CreateVirWebSite("Message", Context.Parameters["installdir"].ToString() + "Message");
                CreateVirWebSite("MoreUpload", Context.Parameters["installdir"].ToString() + "MoreUpload");
                this.Rollback(stateSaver);
            }
        }


        /// <summary>
        /// 卸载 
        /// </summary>
        /// <param name="savedState"></param>
        public override void Uninstall(IDictionary savedState)
        {
            _dbconfigPath = Path.Combine(this.Context.Parameters["installdir"].ToString(), "dbconfig.xml");
            string siteName = OperateXML.GetXmlNodeValue(_dbconfigPath, "SiteName");
            //删除安装文件前先删除站点，因删除站点时需读取安装文件的配置信息
            DeleteWebSite(siteName);

            //删除快捷方式
            DeleteDeskTopShortcut();

            //删除开始菜单中快捷方式
            DeleteProgramsShortcut();

            Register reg = new Register();
            reg.DeleteStartWithWindow("BackUpDatabaseServer");

            //卸载
            base.Uninstall(savedState);

        }

        #endregion

        #region 函数

        /// <summary>
        /// 获取数据库登录连接字符串
        /// </summary>
        /// <param name="databasename">数据库名称</param>
        /// <returns></returns>
        private string GetConnectionString(string databasename)
        {

            string connStr = "server=" + Context.Parameters["server"].ToString() + ";database=" +
                (string.IsNullOrEmpty(databasename) ? "master" : databasename)
                + ";uid=" + Context.Parameters["user"].ToString()
                + ";pwd=" + Context.Parameters["pwd"].ToString();

            //将连接串写入XML文件，供卸载操作时读取
            if (!string.IsNullOrEmpty(databasename))
            {
                _dbconfigPath = Path.Combine(this.Context.Parameters["installdir"].ToString(), "dbconfig.xml");
                OperateXML.UpdateXMLNode(_dbconfigPath, "DbName", databasename);
            }

            _dbconfigPath = Path.Combine(this.Context.Parameters["installdir"].ToString(), "dbconfig.xml");
            OperateXML.UpdateXMLNode(_dbconfigPath, "ConnString", connStr);
            return connStr;
        }

        /// <summary>
        /// 将配置信息写入文件 供卸载时使用
        /// </summary>
        private void WriteMessageForUnInstall()
        {
            //站点名称
            _dbconfigPath = Path.Combine(this.Context.Parameters["installdir"].ToString(), "dbconfig.xml");
            OperateXML.UpdateXMLNode(_dbconfigPath, "SiteName", Context.Parameters["sitename"].ToString());
            OperateXML.UpdateXMLNode(_dbconfigPath, "ShortcutName", Context.Parameters["shortcutname"].ToString());

        }

        /// <summary>
        /// 注册Aspnet_regiis
        /// </summary>
        private void AspnetRegIIS()
        {
            string fileName = Environment.GetEnvironmentVariable("windir") + @"\Microsoft.NET\Framework\v4.0.30319\aspnet_regiis.exe";
            ProcessStartInfo startInfo = new ProcessStartInfo(fileName, "-i");
            Process process = new Process();
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
        }

        /// <summary>
        /// 判断是否安装了SQL SERVER
        /// </summary>
        /// <returns></returns>
        private bool ExistSqlServerService()
        {
            bool Exist = false;
            ServiceController[] service = ServiceController.GetServices();
            for (int i = 0; i < service.Length; i++)
            {
                if (service[i].ServiceName.Length > 5 && service[i].ServiceName.Substring(0, 5) == "MSSQL")
                {
                    Exist = true;
                    break;
                }
            }
            return Exist;
        }

        /// <summary>
        /// 判断系统是否为xp或者为server2003
        /// </summary>
        /// <returns></returns>
        private bool IsXPOr2003()
        {
            string osVerstion = string.Empty;
            OperatingSystem os = Environment.OSVersion;
            //Windows   200、   Windows   XP、  Windows   2003 
            if (os.Platform == PlatformID.Win32NT && os.Version.Major == 5)  //xp 不安装IIS，进行下提示，会让界面更友好
                return true;
            return false;
            //系统版本获取
            //switch (os.Platform)
            //{
            //    case PlatformID.Win32Windows:
            //        switch (os.Version.Minor)
            //        {
            //            case 0:
            //                osVerstion = "Windows   95 ";
            //                break;
            //            case 10:
            //                if (os.Version.Revision.ToString() == "2222A ")
            //                    osVerstion = "Windows   98   第二版 ";
            //                else
            //                    osVerstion = "Windows   98 ";
            //                break;
            //            case 90:
            //                osVerstion = "Windows   Me ";
            //                break;
            //        }
            //        break;
            //    case PlatformID.Win32NT:
            //        switch (os.Version.Major)
            //        {
            //            case 3:
            //                osVerstion = "Windows   NT   3.51 ";
            //                break;
            //            case 4:
            //                osVerstion = "Windows   NT   4.0 ";
            //                break;
            //            case 5:
            //                switch (os.Version.Minor)
            //                {
            //                    case 0:
            //                        osVerstion = "Windows   200 ";
            //                        break;
            //                    case 1:
            //                        osVerstion = "Windows   XP ";
            //                        break;
            //                    case 2:
            //                        osVerstion = "Windows   2003 ";
            //                        break;
            //                }
            //                break;
            //            case 6:
            //                switch (os.Version.Minor)
            //                {
            //                    case 0:
            //                        osVerstion = "Windows  Vista ";
            //                        break;
            //                    case 1:
            //                        osVerstion = "Windows   7 ";
            //                        break;
            //                }
            //                break;
            //        }
            //        break;
            //}

        }
        /// <summary>
        /// 检测IIS及版本号
        /// </summary>
        /// <returns></returns>
        public string GetIISVerstion()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\INetStp");

            if (key == null)
                return string.Empty;
            else
                return Convert.ToString(key.GetValue("MajorVersion")) + "." + Convert.ToString(key.GetValue("MinorVersion"));

        }

        /// <summary>
        /// 采用批处理方式运行数据库升级
        /// </summary>
        private void UpdateDataBase(string installPath)
        {
            try
            {
                //采用批处理方式安装IIS7.5
                Process process = new Process();
                process.StartInfo.FileName = Path.Combine(installPath, "NewAttend.Upgrade.exe");

                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardError = true;//开启出错返回信息
                process.StartInfo.RedirectStandardOutput = true;//开启输出返回信息

                process.Start();
                string strOUT = process.StandardOutput.ReadToEnd();//用于捕捉返回信息。
                string strERR = process.StandardError.ReadToEnd();
                process.WaitForExit();
                process.Dispose();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// 安装IIS服务
        /// </summary>
        /// <param name="installPath"></param>
        /// <param name="iisTxt"></param>
        private void IISInstall(string installPath, string iisTxt)
        {
            //获取注册表中系统安装路径键值 -- 为xp\server2003上安装做准备
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Setup", true);

            if (key == null)
            {
                return;
            }

            //安装路径
            string sourcePath = Convert.ToString(key.GetValue("SourcePath"));
            string servicePackSourcePath = Convert.ToString(key.GetValue("ServicePackSourcePath"));

            //设置安装目录值
            key.SetValue("ServicePackSourcePath", installPath);
            key.SetValue("SourcePath", installPath);

            try
            {
                //采用批处理方式安装IIS7.5
                Process process = new Process();
                process.StartInfo.FileName = Path.Combine(installPath, "iis7x_setup.bat");

                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardError = true;//开启出错返回信息
                process.StartInfo.RedirectStandardOutput = true;//开启输出返回信息

                process.Start();
                string strOUT = process.StandardOutput.ReadToEnd();//用于捕捉返回信息。
                string strERR = process.StandardError.ReadToEnd();
                process.WaitForExit();
                process.Dispose();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                //将系统路径置回
                key.SetValue("ServicePackSourcePath", servicePackSourcePath);
                key.SetValue("SourcePath", sourcePath);
            }
        }

        /// <summary>
        /// 未有处理远程服务器
        /// </summary>
        /// <param name="entPath">名字</param>
        /// <returns></returns>
        public DirectoryEntry GetDirectoryEntry(string entPath)
        {
            DirectoryEntry ent = new DirectoryEntry(entPath);
            return ent;
        }

        /// <summary>
        /// Web站点类
        /// </summary>
        public class NewWebSiteInfo
        {
            private string _hostIP;   // 主机IP
            private string _portNum;   // 网站端口号
            private string _descOfWebSite; // 网站表示。一般为网站的网站名。例如"www.dns.com.cn"
            private string _commentOfWebSite;// 网站注释。一般也为网站的网站名。
            private string _webPath;   // 网站的主目录。例如"e:\ mp"

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="hostIP">主机IP</param>
            /// <param name="portNum">网站端口号</param>
            /// <param name="descOfWebSite">网站名</param>
            /// <param name="commentOfWebSite">网站注释</param>
            /// <param name="webPath">网站的主目录</param>
            public NewWebSiteInfo(string hostIP, string portNum, string descOfWebSite, string commentOfWebSite, string webPath)
            {
                _hostIP = hostIP;
                _portNum = portNum;
                _descOfWebSite = descOfWebSite;
                _commentOfWebSite = commentOfWebSite;
                _webPath = webPath;
            }

            /// <summary>
            /// 绑定字符串 主机IP 端口号 网站名
            /// </summary>
            public string BindString
            {
                get
                {
                    return String.Format("{0}:{1}:{2}", _hostIP, _portNum, _descOfWebSite); //网站标识（IP,端口，主机头值）
                }
            }

            /// <summary>
            /// 端口号
            /// </summary>
            public string PortNum
            {
                get
                {
                    return _portNum;
                }
            }

            /// <summary>
            /// 网站注释。一般也为网站的网站名
            /// </summary>
            public string CommentOfWebSite
            {
                get
                {
                    return _commentOfWebSite;
                }
            }

            /// <summary>
            /// 网站的主目录。例如"e:\ mp"
            /// </summary>
            public string WebPath
            {
                get
                {
                    return _webPath;
                }
            }
        }


        /// <summary>
        /// 站点是否重复
        /// </summary>
        /// <param name="bindStr"></param>
        /// <returns></returns>
        private bool EnsureNewSiteEnavaible(string bindStr)
        {
            DirectoryEntry ent = GetDirectoryEntry(_entPath);

            foreach (DirectoryEntry child in ent.Children)
            {
                if (child.SchemaClassName == "IIsWebServer")
                {

                    if (child.Properties["ServerBindings"].Value != null)
                    {
                        if (child.Properties["ServerBindings"].Value.ToString() == bindStr)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 端口号是否重复
        /// </summary>
        /// <returns></returns>
        private bool IsExistSitePort()
        {
            bool exist = false;
            DirectoryEntry ent = GetDirectoryEntry(_entPath);

            try
            {
                foreach (DirectoryEntry child in ent.Children)
                {
                    if (child.SchemaClassName == "IIsWebServer")
                    {
                        if (child.Properties["ServerBindings"].Value != null && child.Properties["ServerBindings"].Value.ToString().Split(':').Length > 1)
                        {
                            if (child.Properties["ServerBindings"].Value.ToString().Split(':')[1] == Context.Parameters["siteport"].ToString())
                            {
                                exist = true;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            return exist;
        }

        /// <summary>
        /// 站点名称是否存在
        /// </summary>
        /// <returns></returns>
        private bool IsExistSiteName(string sitename)
        {
            bool exist = false;
            try
            {
                using (DirectoryEntry root = new DirectoryEntry(_entPath))
                {
                    foreach (DirectoryEntry Child in root.Children)
                    {
                        if (Child.SchemaClassName == "IIsWebServer")
                        {
                            string WName = Child.Properties["ServerComment"].Value.ToString();
                            if (sitename == WName)
                            {
                                exist = true;
                                break;
                            }
                        }
                    }
                    root.Close();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return exist;
        }


        /// <summary>
        ///  获取网站系统里面可以使用的最小的ID。
        ///  这是因为每个网站都需要有一个唯一的编号，而且这个编号越小越好。
        /// </summary>
        /// <returns>最小的id</returns>
        public string GetNewWebSiteID()
        {
            ArrayList list = new ArrayList();
            string tmpStr;
            DirectoryEntry ent = GetDirectoryEntry(_entPath);

            foreach (DirectoryEntry child in ent.Children)
            {
                if (child.SchemaClassName == "IIsWebServer")
                {
                    tmpStr = child.Name.ToString();
                    list.Add(Convert.ToInt32(tmpStr));
                }
            }
            list.Sort();
            int i = 1;
            foreach (int j in list)
            {
                if (i == j)
                {
                    i++;
                }
            }
            return i.ToString();
        }

        /// <summary>
        /// 创建网站
        /// </summary>
        /// <param name="siteInfo"></param>
        public void CreateNewWebSite(NewWebSiteInfo siteInfo)
        {
            try
            {
                //MessageBox.Show("创建站点CreateNewWebSite开始", "信息提示", MessageBoxButtons.OK);
                if (!EnsureNewSiteEnavaible(siteInfo.BindString))
                {
                    throw new Exception("该网站已存在" + Environment.NewLine + siteInfo.BindString);
                }

                DirectoryEntry rootEntry = GetDirectoryEntry(_entPath);

                _newSiteNum = GetNewWebSiteID();

                DirectoryEntry newSiteEntry = rootEntry.Children.Add(_newSiteNum, "IIsWebServer");

                newSiteEntry.CommitChanges();

                newSiteEntry.Properties["ServerBindings"].Value = siteInfo.BindString;
                newSiteEntry.Properties["ServerComment"].Value = siteInfo.CommentOfWebSite;
                newSiteEntry.Properties["AppPoolId"].Value = "ASP.NET v4.0";
                newSiteEntry.CommitChanges();
                DirectoryEntry vdEntry = newSiteEntry.Children.Add("root", "IIsWebVirtualDir");
                vdEntry.CommitChanges();
                vdEntry.Properties["Path"].Value = siteInfo.WebPath;
                vdEntry.Invoke("AppCreate", true);//创建应用程序

                vdEntry.Properties["AccessRead"][0] = true; //设置读取权限
                vdEntry.Properties["DefaultDoc"][0] = Context.Parameters["startpage"].ToString();//设置默认文档
                vdEntry.Properties["AppFriendlyName"][0] = "Toncent"; //应用程序名称
                vdEntry.Properties["AccessScript"][0] = true;//执行权限
                vdEntry.Properties["AuthFlags"][0] = 1;//0表示不允许匿名访问,1表示就可以3为基本身份验证，7为windows继承身份验证

                List<string> appPools = GetApplicationPools();

                if (appPools.Contains("ASP.NET v4.0"))
                {                    
                    if (!IsXPOr2003())
                    {                        
                        ///IIS 7.0以上版本这样写
                        vdEntry.Properties["AppPoolId"].Value = "ASP.NET v4.0";
                    }
                    else
                    {                        
                        //xp 或server2003需要配置
                        vdEntry.Properties["AppPoolId"].Value = "DefaultAppPool";
                    }
                }
                else
                {                    
                    vdEntry.Properties["AppPoolId"].Value = "DefaultAppPool";
                }

                vdEntry.CommitChanges();

                //启动aspnet_regiis.exe程序 
                string fileName = Environment.GetEnvironmentVariable("windir") + @"\Microsoft.NET\Framework\v4.0.30319\aspnet_regiis.exe";
                ProcessStartInfo startInfo = new ProcessStartInfo(fileName);

                //处理目录路径 
                string path = vdEntry.Path.ToUpper();
                int index = path.IndexOf("W3SVC");
                path = path.Remove(0, index);

                //启动ASPnet_iis.exe程序,刷新脚本映射 
                startInfo.Arguments = "-s " + path;
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.UseShellExecute = false;
                startInfo.CreateNoWindow = true;
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardError = true;

                Process process = new Process();
                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();
                string errors = process.StandardError.ReadToEnd();

                if (errors != string.Empty)
                {
                    throw new Exception(errors);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// 启动站点
        /// </summary>
        /// <param name="siteName"></param>
        public void StartWebSite(string websiteName)
        {
            string siteName = Context.Parameters["sitename"].ToString();
            string siteEntPath = String.Format("IIS://{0}/w3svc/{1}", "localhost", siteName);
            DirectoryEntry siteEntry = GetDirectoryEntry(siteEntPath);
            siteEntry.Invoke("Start", new object[] { });
        }

        public List<string> GetApplicationPools()
        {
            //if ((SiteInfo.ServerType != WebServerTypes.IIS6) && (SiteInfo.ServerType != WebServerTypes.IIS7)) return null;
            DirectoryEntry directoryEntry = GetDirectoryEntry("IIS://LOCALHOST/W3SVC/AppPools");
            if (directoryEntry == null) return null;
            List<string> list = new List<string>();
            foreach (DirectoryEntry entry2 in directoryEntry.Children)
            {
                PropertyCollection properties = entry2.Properties;               
                list.Add(entry2.Name);
            }
            return list;
        }    

        /// <summary>
        /// 在主机上创建虚拟目录
        /// </summary>
        /// <param name="_virtualDirName">虚拟目录名称</param>
        /// <param name="_physicalPath">虚拟目录物理路径</param>
        public void CreateVirWebSite(string _virtualDirName, string _physicalPath)
        {

            try
            {
                String constIISWebSiteRoot = "IIS://localhost/W3SVC/" + _newSiteNum + "/ROOT";  //获得IIS版本号
                string virtualDirName = _virtualDirName;
                string physicalPath = _physicalPath;

                if (String.IsNullOrEmpty(physicalPath))
                {
                    throw new NullReferenceException("创建虚拟目录的物理路径不能为空");
                }

                DirectoryEntry root = new DirectoryEntry(constIISWebSiteRoot);
                DirectoryEntry tbEntry = root.Children.Add(virtualDirName, "IIsWebVirtualDir");

                tbEntry.Properties["Path"][0] = physicalPath;  //虚拟目录物理路径  
                tbEntry.Invoke("AppCreate", true);
                tbEntry.Properties["AccessRead"][0] = true;   //设置读取权限  
                tbEntry.Properties["ContentIndexed"][0] = true;

                tbEntry.Properties["AppFriendlyName"][0] = virtualDirName; //虚拟目录名称
                tbEntry.Properties["AccessScript"][0] = true; //执行权限  
                tbEntry.Properties["AppIsolated"][0] = "1";
                tbEntry.Properties["DontLog"][0] = true;  // 是否记录日志                  

                tbEntry.Properties["AuthFlags"][0] = 1;// 设置目录的安全性，0表示不允许匿名访问，1为允许，3为基本身份验证，7为windows继承身份验证  
                tbEntry.CommitChanges();
                root.CommitChanges();//确认更改  

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }



        /// <summary>
        /// 解压缩
        /// </summary>
        /// <param name="rarName">压缩包的名称</param>
        /// <returns></returns>
        public void unRAR(string rarName)
        {
            string the_Info;
            ProcessStartInfo the_StartInfo;
            Process the_Process;
            try
            {
                RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\WinRAR.exe");//动态读取WinRAR的安装路径
                the_Info = " X  " + Context.Parameters["installdir"].ToString() + rarName + " " + Context.Parameters["installdir"].ToString();
                the_StartInfo = new ProcessStartInfo();
                the_StartInfo.FileName = key.GetValue("").ToString(); //@"C:\Program Files\WinRAR\WinRAR.exe";  //GetValue参数值为空时读取默认值
                the_StartInfo.Arguments = the_Info;

                the_StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                the_Process = new Process();
                the_Process.StartInfo = the_StartInfo;
                the_Process.Start();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// 读取COM组件并写入批处理文件
        /// </summary>
        private void WriteBat()
        {
            string path = Path.Combine(this.Context.Parameters["installdir"].ToString(), "Com");
            //动态读取系统system32的路径
            string sysPath = Environment.GetFolderPath(Environment.SpecialFolder.System);
            DirectoryInfo Dir = new DirectoryInfo(path);
            if (Dir.Exists == true)
            {
                FileInfo[] fileArr = Dir.GetFiles();
                StreamWriter sw = new StreamWriter(path + @"\regcom.bat");
                foreach (FileInfo file in fileArr)
                {
                    File.Copy(path + @"\" + file.Name, sysPath + @"\" + file.Name, true);
                    sw.WriteLine(@"regsvr32.exe /s  %WINDIR%\system32\" + file.Name);
                }
                sw.Close();
            }
        }

        /// <summary>
        /// 注册COM组件
        /// </summary>
        private void RegisterCom()
        {
            string path = Path.Combine(this.Context.Parameters["installdir"].ToString(), "Com");
            Process rarPro = new Process();
            rarPro.StartInfo.FileName = path + @"\regcom.bat";
            rarPro.Start();
            rarPro.WaitForExit();
            rarPro.Dispose();
            File.Delete(path + @"\regcom.bat");
        }

        /// <summary>
        /// 关闭进程
        /// </summary>
        /// <param name="name"></param>
        private void KillProcess(string name)
        {
            Process[] p = Process.GetProcessesByName(name);

            for (int index = 0; index < p.Length; index++)
            {
                p[index].Kill();
            }
        }

        /// <summary>
        /// 卸载项目时一并删除IIS下的WEB站点
        /// </summary>
        private void DeleteWebSite(string siteName)
        {
            if (!IsExistSiteName(siteName))
            {
                return;
            }
            //关闭 w3wp.exe 进程 防止站点不让删除
            KillProcess("w3wp");

            using (DirectoryEntry root = new DirectoryEntry(_entPath))
            {
                foreach (DirectoryEntry Child in root.Children)
                {
                    if (Child.SchemaClassName == "IIsWebServer")
                    {
                        string WName = Child.Properties["ServerComment"].Value.ToString();

                        if (siteName == WName)
                        {
                            Child.DeleteTree();
                            root.CommitChanges();
                            break;
                        }
                    }
                }
                root.Close();
            }
        }

        /// <summary>
        /// 删除桌面快捷方式
        /// </summary>
        private void DeleteDeskTopShortcut()
        {
            _dbconfigPath = Path.Combine(this.Context.Parameters["installdir"].ToString(), "dbconfig.xml");
            string siteName = OperateXML.GetXmlNodeValue(_dbconfigPath, "ShortcutName");

            //删除快捷方式
            string deskPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

            deskPath = deskPath + @"\" + siteName + ".url ";

            if (File.Exists(deskPath))
            {
                File.Delete(deskPath);
            }
        }


        /// <summary>
        /// 创建桌面快捷方式
        /// </summary>
        private void CreateDeskTopShortcut()
        {
            try
            {
                //获取桌面路径
                string DeskPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                string IconPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                StreamWriter deskShortName = new StreamWriter(File.Open(DeskPath + @"\" + Context.Parameters["shortcutname"].ToString() + ".url ", FileMode.Create, FileAccess.Write));
                Thread.Sleep(1000);
                deskShortName.WriteLine("[InternetShortcut] ");
                deskShortName.WriteLine("URL=http://localhost:" + Context.Parameters["siteport"].ToString() + "/" + Context.Parameters["startpage"].ToString());
                deskShortName.WriteLine("Modified=00B21CE31E06C30199 ");

                deskShortName.Flush();
                deskShortName.Close();

                //string ProgramsPath = Environment.GetFolderPath(Environment.SpecialFolder.Programs);

                //StreamWriter programsShortName = new StreamWriter(File.Open(ProgramsPath + @"\" + Context.Parameters["shortcutname"].ToString() + ".url ", FileMode.Create, FileAccess.Write));
                //programsShortName.WriteLine("[InternetShortcut] ");
                //programsShortName.WriteLine("URL=http://localhost:" + Context.Parameters["siteport"].ToString() + "/" + Context.Parameters["startpage"].ToString());
                //programsShortName.WriteLine("Modified=00B21CE31E06C30199 ");

                //programsShortName.Flush();
                //programsShortName.Close();

            }
            catch (Exception e)
            {
                MessageBox.Show("创建桌面快捷方式异常。" + e.Message, "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// 打开注册表路径
        /// </summary>
        /// <param name="root"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        private RegistryKey OpenRegistryPath(RegistryKey root, string s)
        {
            s = s.Remove(0, 1) + @"/";
            while (s.IndexOf(@"/") != -1)
            {
                root = root.OpenSubKey(s.Substring(0, s.IndexOf(@"/")));
                s = s.Remove(0, s.IndexOf(@"/") + 1);
            }
            return root;
        }

        /// <summary>
        /// 删除开始菜单快捷方式
        /// </summary>
        private void DeleteProgramsShortcut()
        {
            _dbconfigPath = Path.Combine(this.Context.Parameters["installdir"].ToString(), "dbconfig.xml");
            string siteName = OperateXML.GetXmlNodeValue(_dbconfigPath, "ShortcutName");

            //SHGetSpecialFolderPath(IntPtr.Zero, _pathBuilder, CSIDL_PROGRAMS, false);
            //string programsPath = _pathBuilder.ToString();
            //MessageBox.Show(programsPath, "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //RegistryKey folders;
            //folders = OpenRegistryPath(Registry.CurrentUser, @"/software/microsoft/windows/currentversion/explorer/shell folders");

            //删除快捷方式   folders.GetValue("Programs").ToString();

            string programsPath = Environment.GetFolderPath(Environment.SpecialFolder.Programs);
            programsPath = programsPath + @"\虹膜考勤系统-浏览器\" + siteName + ".url ";

            if (File.Exists(programsPath))
            {
                File.Delete(programsPath);
            }
        }

        /// <summary>
        /// 创建开始菜单快捷方式
        /// </summary>
        private void CreateProgramsShortcut()
        {
            try
            {
                //System.Environment.
                string programsPath = Environment.GetFolderPath(Environment.SpecialFolder.Programs);
                StreamWriter programsShortName = new StreamWriter(File.Open(programsPath + @"\虹膜考勤系统-浏览器\" + Context.Parameters["shortcutname"].ToString() + ".url ", FileMode.Create, FileAccess.Write));
                programsShortName.WriteLine("[InternetShortcut] ");
                programsShortName.WriteLine("URL=http://localhost:" + Context.Parameters["siteport"].ToString() + "/" + Context.Parameters["startpage"].ToString());
                programsShortName.WriteLine("Modified=00B21CE31E06C30199 ");

                programsShortName.Flush();
                programsShortName.Close();

            }
            catch (Exception e)
            {
                MessageBox.Show("创建开始菜单快捷方式异常。" + e.Message, "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        /// <summary>
        /// 获取资源文件中的脚本
        /// </summary>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        string GetResource(string resourceName)
        {
            Assembly ass = Assembly.GetExecutingAssembly();
            Stream stream = ass.GetManifestResourceStream(ass.GetName().Name + "." + resourceName);
            using (StreamReader reader = new StreamReader(stream, System.Text.Encoding.Default))
            {
                return reader.ReadToEnd();
            }
        }


        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="sql"></param>
        void ExecuteSQL(SqlConnection connection, string sql)
        {
            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.ExecuteNonQuery();
        }
        #endregion

        #region 添加启动项

        //private void Form1_Load(object sender, EventArgs e)
        //{
        //    //检测程序是否设为开机启动
        //    Register rg = new Register();
        //    rg.SetStartWithWindow();
        //}

        /// <summary>
        /// 设置启动项类
        /// </summary>
        class Register
        {
            /// <summary>
            /// 启动项名称
            /// </summary>
            private string _name = string.Empty;

            /// <summary>
            /// 启动项路径
            /// </summary>
            private string _path = string.Empty;

            /// <summary>
            /// 开机启动注册表检查
            /// </summary>
            public void SetStartWithWindow(string name, string path)
            {
                if (name != "" && path != "")
                {
                    _name = name;
                    _path =path;
                }
                else
                {
                    return;
                }
                RegistryKey hklm = Registry.LocalMachine;
                RegistryKey run = hklm.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                //未设置开机启动
                if (run.GetValue(_name) == null)
                {
                    SetRegister();
                }
            }


            /// <summary>
            /// 卸载开机启动注册表
            /// </summary>
            public void DeleteStartWithWindow(string name)
            {
                if (name != "")
                {
                    _name = name;
                }
                else
                {
                    return;
                }

                RegistryKey hklm = Registry.LocalMachine;
                RegistryKey run = hklm.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                //未设置开机启动
                if (run.GetValue(_name) != null)
                {
                    DeleteRegister();
                }
            }

            /// <summary>
            /// 将程序的开机启动写入注册表
            /// </summary>
            private void SetRegister()
            {
                //string starupPath = Application.ExecutablePath;
                //class Micosoft.Win32.RegistryKey. 表示Window注册表中项级节点,此类是注册表装.
                RegistryKey loca = Registry.LocalMachine;
                RegistryKey run = loca.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");

                try
                {
                    run.SetValue(_name, _path);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message.ToString(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    loca.Close();
                    run.Close();
                }
            }

            /// <summary>
            /// 将程序的开机启动从注册表中删除
            /// </summary>
            private void DeleteRegister()
            {
                //string starupPath = Application.ExecutablePath;
                //class Micosoft.Win32.RegistryKey. 表示Window注册表中项级节点,此类是注册表装.

                RegistryKey hklm = Registry.LocalMachine;
                RegistryKey run = hklm.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                try
                {
                    run.DeleteValue(_name);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message.ToString(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    hklm.Close();
                    run.Close();
                }
            }

        }

        #endregion

        #region 获取一个网站编号//一个输入参数为站点描述

        /// <summary>
        /// 输入参数为 站点的描述名 默认是站点描述为 "默认网站"
        /// 2013-03-20 fjf 新增网站序号函数
        /// <exception cref="NotFoundWebSiteException">表示没有找到网站</exception>
        public string GetWebSiteNum(string siteName)
        {
            Regex regex = new Regex(siteName);
            string tmpStr;
            //第二个参数为HostName
            //详见：http://blog.csdn.net/jxufewbt/article/details/849764
            string entPath = String.Format("IIS://{0}/w3svc", "localhost");
            DirectoryEntry ent = GetDirectoryEntry(entPath);
            foreach (DirectoryEntry child in ent.Children)
            {
                if (child.SchemaClassName == "IIsWebServer")
                {
                    if (child.Properties["ServerBindings"].Value != null)
                    {
                        tmpStr = child.Properties["ServerBindings"].Value.ToString();
                        if (regex.Match(tmpStr).Success)
                        {
                            return child.Name;
                        }
                    }
                    if (child.Properties["ServerComment"].Value != null)
                    {
                        tmpStr = child.Properties["ServerComment"].Value.ToString();
                        if (regex.Match(tmpStr).Success)
                        {
                            return child.Name;
                        }
                    }
                }
            }
            throw new Exception("没有找到我们想要的站点" + siteName);
        }
        #endregion

        #region Start和Stop网站的方法 2013-03-20 fjf

        /// <summary>
        /// 启动网站的新版本 2013-03-20 fjf 
        /// </summary>
        /// <param name="siteName"></param>
        public void StartWebSiteByNum(string siteName)
        {
            try
            {
                Thread.Sleep(3000);
                string siteNum = GetWebSiteNum(siteName);
                string siteEntPath = String.Format("IIS://{0}/w3svc/{1}", "localhost", siteNum);
                DirectoryEntry siteEntry = GetDirectoryEntry(siteEntPath);
                siteEntry.Invoke("Start", new object[] { });
            }
            catch (Exception e)
            {
                MessageBox.Show("CreateNewWebSite（）异常 " + e.Message, "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// 停止网站站点
        /// </summary>
        /// <param name="siteName"></param>
        public void StopWebSite(string siteName)
        {
            string siteNum = GetWebSiteNum(siteName);
            string siteEntPath = String.Format("IIS://{0}/w3svc/{1}", "localhost", siteNum);
            DirectoryEntry siteEntry = GetDirectoryEntry(siteEntPath);
            siteEntry.Invoke("Stop", new object[] { });
        }

        #endregion

        #region 两种分配线程池的方法

        /// <summary>
        /// 分配应用程序池 一种方法
        /// </summary>
        /// <param name="newvdir"></param>
        /// <param name="AppPoolName"></param>
        public void AssignAppPool(DirectoryEntry newvdir, string AppPoolName)
        {
            object[] param = { 0, AppPoolName, true };
            newvdir.Invoke("Appcreate3", param);
        }

        /// <summary>
        /// 建立程序池后关联相应应用程序及虚拟目录 第二种
        /// </summary>
        public static void SetAppToPool(string appname)
        {
            //获取目录
            DirectoryEntry getdir = new DirectoryEntry("IIS://localhost/W3SVC");
            foreach (DirectoryEntry getentity in getdir.Children)
            {
                if (getentity.SchemaClassName.Equals("IIsWebServer"))
                {
                    //设置应用程序程序池 先获得应用程序 在设定应用程序程序池 
                    //第一次测试根目录
                    foreach (DirectoryEntry getchild in getentity.Children)
                    {
                        if (getchild.SchemaClassName.Equals("IIsWebVirtualDir"))
                        {
                            //找到指定的虚拟目录.
                            foreach (DirectoryEntry getsite in getchild.Children)
                            {
                                if (getsite.Name.Equals(appname))
                                {
                                    //【测试成功通过】 
                                    getsite.Properties["AppPoolId"].Value = "ASP.NET v4.0";
                                    getsite.CommitChanges();
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion

    }
}
