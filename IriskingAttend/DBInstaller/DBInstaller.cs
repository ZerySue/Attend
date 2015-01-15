/*************************************************************************
** �ļ���:   DBInstaller.cs
���� ��Ҫ��:   DBInstaller
**  
** Copyright (c) �пƺ�����޹�˾
** ������:   
** ��  ��:   
** �޸���:   lzc
** ��  ��:   2013-8-12
** �޸����ݣ� ����IIS�İ�װ��web.congif�ļ������ã�ж��������á�ж�ء�վ������ú�ɾ����������ʼ�˵���ݷ�ʽ��
** ��  ��:   DBInstaller�࣬�¿��ڰ�װ������
**
** ��  ��:   1.0.0
** ��  ע:  �����������д����C#����淶
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
        #region ˽�б���

        //���ݿ�·��
        private string _dbconfigPath = string.Empty;
        //w3svcλ��
        private string _entPath = String.Format("IIS://{0}/w3svc", "localhost");

        //վ��˿ں�
        private string _newSiteNum = string.Empty;

        #endregion

        #region ���캯��
        /// <summary>
        /// ���캯��
        /// </summary>
        public DBInstaller()
        {

            InitializeComponent();
        }

        #endregion

        #region ���غ���

        /// <summary>
        /// �����Զ��尲װ
        /// </summary>
        /// <param name="savedState"></param>
        protected override void OnBeforeInstall(IDictionary savedState)
        {
            base.OnBeforeInstall(savedState);
            string webconfigpath = Path.Combine(this.Context.Parameters["installdir"].ToString(), "Web.config");
            //string configPath  =Path.Combine( Directory.GetCurrentDirectory(),"Install.config");
            //Ĭ�ϰ�װΪ��λ��װ----����ǿ� ��IsMine = "false";
            //string IsMine = "false"; //������ǿ������ֱ���������ļ��иı� --by gqy 2014-1-17
            //#if RELEASE 
            //            MessageBox.Show("RELEASENEWATTEND  IsMine = TRUE" , "��Ϣ��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //            IsMine = "True"; 
            //#else
            //            IsMine = "False";
            //#endif

            try
            {
                //����web.config�ļ� ��װ����
                string webcofnigstring = File.ReadAllText(webconfigpath).Replace("#constring#",
                    GetConnectionString(Context.Parameters["dbname"].ToString()));
                webcofnigstring = webcofnigstring.Replace("#serverport#", Context.Parameters["serverport"].ToString());
                webcofnigstring = webcofnigstring.Replace("#serverip#", Context.Parameters["serverip"].ToString());
                //webcofnigstring = webcofnigstring.Replace("#ismine#", IsMine);  //������ǿ������ֱ���������ļ��иı� --by gqy 2014-1-17
                File.WriteAllText(webconfigpath, webcofnigstring);

                //Ϊж�ش洢�����Ϣ
                WriteMessageForUnInstall();
            }
            catch (Exception e)
            {
                MessageBox.Show("File.ReadAllText" + e.Message, "��Ϣ��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            //xp��server2003��Ҫ���а�װIIS
            if (!IsXPOr2003())
            {
                //���������IIS ��װIIS
                if (string.Empty == GetIISVerstion())
                {
                    IISInstall(this.Context.Parameters["installdir"].ToString(),
                        this.Context.Parameters["installdir"].ToString() + "iis.txt");
                    //asp.net ע��IIS 
                    AspnetRegIIS();
                }
            }
        }

        /// <summary>
        /// �����Զ���ж�ذ�װ
        /// </summary>
        /// <param name="savedState"></param>
        protected override void OnBeforeUninstall(IDictionary savedState)
        {
            //ж��֮ǰ�ȹرձ������ݿ����
            KillProcess("BackUpDatabaseServer");
        }

        /// <summary>
        /// ��װ
        /// </summary>
        /// <param name="stateSaver"></param>
        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);

            if (IsExistSiteName(Context.Parameters["sitename"].ToString()))
            {
                //�������վ����ɾ����վ��
                DeleteWebSite(Context.Parameters["sitename"].ToString());
                //MessageBox.Show("վ�������ظ���", "��Ϣ��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //this.Rollback(stateSaver);
                // return;
            }

            if (IsExistSitePort())
            {
                MessageBox.Show("վ��˿ں��ظ���", "��Ϣ��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Rollback(stateSaver);
                return;
            }

            try
            {
                //������ݷ�ʽ
                CreateDeskTopShortcut();
                CreateProgramsShortcut();

               
                Thread.Sleep(2000);
                string s = Context.Parameters["installdir"].ToString().Replace("\\\\", "");

                NewWebSiteInfo siteInfo = new NewWebSiteInfo(string.Empty, Context.Parameters["siteport"].ToString(), "",
                    Context.Parameters["sitename"].ToString(), s);

                CreateNewWebSite(siteInfo);
                Thread.Sleep(2000);
                StartWebSiteByNum(siteInfo.BindString);

                 //MessageBox.Show("���ݿⱸ�ݿ�ʼ��\n", "����");
                 Register reg = new Register();
                 reg.SetStartWithWindow("BackUpDatabaseServer", Path.Combine(s,"���ݿⱸ��\\BackUpDatabaseServer.exe"));
                 //MessageBox.Show("���ݿⱸ�ݽ�����\n", "����");

                 UpdateDataBase(this.Context.Parameters["installdir"].ToString());

            }
            catch (Exception ex)
            {
                MessageBox.Show("����վ��ʧ�ܣ�\n" + ex.Message, "����");
                CreateVirWebSite("Message", Context.Parameters["installdir"].ToString() + "Message");
                CreateVirWebSite("MoreUpload", Context.Parameters["installdir"].ToString() + "MoreUpload");
                this.Rollback(stateSaver);
            }
        }


        /// <summary>
        /// ж�� 
        /// </summary>
        /// <param name="savedState"></param>
        public override void Uninstall(IDictionary savedState)
        {
            _dbconfigPath = Path.Combine(this.Context.Parameters["installdir"].ToString(), "dbconfig.xml");
            string siteName = OperateXML.GetXmlNodeValue(_dbconfigPath, "SiteName");
            //ɾ����װ�ļ�ǰ��ɾ��վ�㣬��ɾ��վ��ʱ���ȡ��װ�ļ���������Ϣ
            DeleteWebSite(siteName);

            //ɾ����ݷ�ʽ
            DeleteDeskTopShortcut();

            //ɾ����ʼ�˵��п�ݷ�ʽ
            DeleteProgramsShortcut();

            Register reg = new Register();
            reg.DeleteStartWithWindow("BackUpDatabaseServer");

            //ж��
            base.Uninstall(savedState);

        }

        #endregion

        #region ����

        /// <summary>
        /// ��ȡ���ݿ��¼�����ַ���
        /// </summary>
        /// <param name="databasename">���ݿ�����</param>
        /// <returns></returns>
        private string GetConnectionString(string databasename)
        {

            string connStr = "server=" + Context.Parameters["server"].ToString() + ";database=" +
                (string.IsNullOrEmpty(databasename) ? "master" : databasename)
                + ";uid=" + Context.Parameters["user"].ToString()
                + ";pwd=" + Context.Parameters["pwd"].ToString();

            //�����Ӵ�д��XML�ļ�����ж�ز���ʱ��ȡ
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
        /// ��������Ϣд���ļ� ��ж��ʱʹ��
        /// </summary>
        private void WriteMessageForUnInstall()
        {
            //վ������
            _dbconfigPath = Path.Combine(this.Context.Parameters["installdir"].ToString(), "dbconfig.xml");
            OperateXML.UpdateXMLNode(_dbconfigPath, "SiteName", Context.Parameters["sitename"].ToString());
            OperateXML.UpdateXMLNode(_dbconfigPath, "ShortcutName", Context.Parameters["shortcutname"].ToString());

        }

        /// <summary>
        /// ע��Aspnet_regiis
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
        /// �ж��Ƿ�װ��SQL SERVER
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
        /// �ж�ϵͳ�Ƿ�Ϊxp����Ϊserver2003
        /// </summary>
        /// <returns></returns>
        private bool IsXPOr2003()
        {
            string osVerstion = string.Empty;
            OperatingSystem os = Environment.OSVersion;
            //Windows   200��   Windows   XP��  Windows   2003 
            if (os.Platform == PlatformID.Win32NT && os.Version.Major == 5)  //xp ����װIIS����������ʾ�����ý�����Ѻ�
                return true;
            return false;
            //ϵͳ�汾��ȡ
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
            //                    osVerstion = "Windows   98   �ڶ��� ";
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
        /// ���IIS���汾��
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
        /// ����������ʽ�������ݿ�����
        /// </summary>
        private void UpdateDataBase(string installPath)
        {
            try
            {
                //����������ʽ��װIIS7.5
                Process process = new Process();
                process.StartInfo.FileName = Path.Combine(installPath, "NewAttend.Upgrade.exe");

                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardError = true;//������������Ϣ
                process.StartInfo.RedirectStandardOutput = true;//�������������Ϣ

                process.Start();
                string strOUT = process.StandardOutput.ReadToEnd();//���ڲ�׽������Ϣ��
                string strERR = process.StandardError.ReadToEnd();
                process.WaitForExit();
                process.Dispose();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "��Ϣ��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// ��װIIS����
        /// </summary>
        /// <param name="installPath"></param>
        /// <param name="iisTxt"></param>
        private void IISInstall(string installPath, string iisTxt)
        {
            //��ȡע�����ϵͳ��װ·����ֵ -- Ϊxp\server2003�ϰ�װ��׼��
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Setup", true);

            if (key == null)
            {
                return;
            }

            //��װ·��
            string sourcePath = Convert.ToString(key.GetValue("SourcePath"));
            string servicePackSourcePath = Convert.ToString(key.GetValue("ServicePackSourcePath"));

            //���ð�װĿ¼ֵ
            key.SetValue("ServicePackSourcePath", installPath);
            key.SetValue("SourcePath", installPath);

            try
            {
                //����������ʽ��װIIS7.5
                Process process = new Process();
                process.StartInfo.FileName = Path.Combine(installPath, "iis7x_setup.bat");

                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardError = true;//������������Ϣ
                process.StartInfo.RedirectStandardOutput = true;//�������������Ϣ

                process.Start();
                string strOUT = process.StandardOutput.ReadToEnd();//���ڲ�׽������Ϣ��
                string strERR = process.StandardError.ReadToEnd();
                process.WaitForExit();
                process.Dispose();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "��Ϣ��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                //��ϵͳ·���û�
                key.SetValue("ServicePackSourcePath", servicePackSourcePath);
                key.SetValue("SourcePath", sourcePath);
            }
        }

        /// <summary>
        /// δ�д���Զ�̷�����
        /// </summary>
        /// <param name="entPath">����</param>
        /// <returns></returns>
        public DirectoryEntry GetDirectoryEntry(string entPath)
        {
            DirectoryEntry ent = new DirectoryEntry(entPath);
            return ent;
        }

        /// <summary>
        /// Webվ����
        /// </summary>
        public class NewWebSiteInfo
        {
            private string _hostIP;   // ����IP
            private string _portNum;   // ��վ�˿ں�
            private string _descOfWebSite; // ��վ��ʾ��һ��Ϊ��վ����վ��������"www.dns.com.cn"
            private string _commentOfWebSite;// ��վע�͡�һ��ҲΪ��վ����վ����
            private string _webPath;   // ��վ����Ŀ¼������"e:\ mp"

            /// <summary>
            /// ���캯��
            /// </summary>
            /// <param name="hostIP">����IP</param>
            /// <param name="portNum">��վ�˿ں�</param>
            /// <param name="descOfWebSite">��վ��</param>
            /// <param name="commentOfWebSite">��վע��</param>
            /// <param name="webPath">��վ����Ŀ¼</param>
            public NewWebSiteInfo(string hostIP, string portNum, string descOfWebSite, string commentOfWebSite, string webPath)
            {
                _hostIP = hostIP;
                _portNum = portNum;
                _descOfWebSite = descOfWebSite;
                _commentOfWebSite = commentOfWebSite;
                _webPath = webPath;
            }

            /// <summary>
            /// ���ַ��� ����IP �˿ں� ��վ��
            /// </summary>
            public string BindString
            {
                get
                {
                    return String.Format("{0}:{1}:{2}", _hostIP, _portNum, _descOfWebSite); //��վ��ʶ��IP,�˿ڣ�����ͷֵ��
                }
            }

            /// <summary>
            /// �˿ں�
            /// </summary>
            public string PortNum
            {
                get
                {
                    return _portNum;
                }
            }

            /// <summary>
            /// ��վע�͡�һ��ҲΪ��վ����վ��
            /// </summary>
            public string CommentOfWebSite
            {
                get
                {
                    return _commentOfWebSite;
                }
            }

            /// <summary>
            /// ��վ����Ŀ¼������"e:\ mp"
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
        /// վ���Ƿ��ظ�
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
        /// �˿ں��Ƿ��ظ�
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
                MessageBox.Show(e.Message, "�쳣", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            return exist;
        }

        /// <summary>
        /// վ�������Ƿ����
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
                MessageBox.Show(e.Message, "�쳣", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return exist;
        }


        /// <summary>
        ///  ��ȡ��վϵͳ�������ʹ�õ���С��ID��
        ///  ������Ϊÿ����վ����Ҫ��һ��Ψһ�ı�ţ�����������ԽСԽ�á�
        /// </summary>
        /// <returns>��С��id</returns>
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
        /// ������վ
        /// </summary>
        /// <param name="siteInfo"></param>
        public void CreateNewWebSite(NewWebSiteInfo siteInfo)
        {
            try
            {
                //MessageBox.Show("����վ��CreateNewWebSite��ʼ", "��Ϣ��ʾ", MessageBoxButtons.OK);
                if (!EnsureNewSiteEnavaible(siteInfo.BindString))
                {
                    throw new Exception("����վ�Ѵ���" + Environment.NewLine + siteInfo.BindString);
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
                vdEntry.Invoke("AppCreate", true);//����Ӧ�ó���

                vdEntry.Properties["AccessRead"][0] = true; //���ö�ȡȨ��
                vdEntry.Properties["DefaultDoc"][0] = Context.Parameters["startpage"].ToString();//����Ĭ���ĵ�
                vdEntry.Properties["AppFriendlyName"][0] = "Toncent"; //Ӧ�ó�������
                vdEntry.Properties["AccessScript"][0] = true;//ִ��Ȩ��
                vdEntry.Properties["AuthFlags"][0] = 1;//0��ʾ��������������,1��ʾ�Ϳ���3Ϊ���������֤��7Ϊwindows�̳������֤

                List<string> appPools = GetApplicationPools();

                if (appPools.Contains("ASP.NET v4.0"))
                {                    
                    if (!IsXPOr2003())
                    {                        
                        ///IIS 7.0���ϰ汾����д
                        vdEntry.Properties["AppPoolId"].Value = "ASP.NET v4.0";
                    }
                    else
                    {                        
                        //xp ��server2003��Ҫ����
                        vdEntry.Properties["AppPoolId"].Value = "DefaultAppPool";
                    }
                }
                else
                {                    
                    vdEntry.Properties["AppPoolId"].Value = "DefaultAppPool";
                }

                vdEntry.CommitChanges();

                //����aspnet_regiis.exe���� 
                string fileName = Environment.GetEnvironmentVariable("windir") + @"\Microsoft.NET\Framework\v4.0.30319\aspnet_regiis.exe";
                ProcessStartInfo startInfo = new ProcessStartInfo(fileName);

                //����Ŀ¼·�� 
                string path = vdEntry.Path.ToUpper();
                int index = path.IndexOf("W3SVC");
                path = path.Remove(0, index);

                //����ASPnet_iis.exe����,ˢ�½ű�ӳ�� 
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
                MessageBox.Show(e.Message, "��Ϣ��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// ����վ��
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
        /// �������ϴ�������Ŀ¼
        /// </summary>
        /// <param name="_virtualDirName">����Ŀ¼����</param>
        /// <param name="_physicalPath">����Ŀ¼����·��</param>
        public void CreateVirWebSite(string _virtualDirName, string _physicalPath)
        {

            try
            {
                String constIISWebSiteRoot = "IIS://localhost/W3SVC/" + _newSiteNum + "/ROOT";  //���IIS�汾��
                string virtualDirName = _virtualDirName;
                string physicalPath = _physicalPath;

                if (String.IsNullOrEmpty(physicalPath))
                {
                    throw new NullReferenceException("��������Ŀ¼������·������Ϊ��");
                }

                DirectoryEntry root = new DirectoryEntry(constIISWebSiteRoot);
                DirectoryEntry tbEntry = root.Children.Add(virtualDirName, "IIsWebVirtualDir");

                tbEntry.Properties["Path"][0] = physicalPath;  //����Ŀ¼����·��  
                tbEntry.Invoke("AppCreate", true);
                tbEntry.Properties["AccessRead"][0] = true;   //���ö�ȡȨ��  
                tbEntry.Properties["ContentIndexed"][0] = true;

                tbEntry.Properties["AppFriendlyName"][0] = virtualDirName; //����Ŀ¼����
                tbEntry.Properties["AccessScript"][0] = true; //ִ��Ȩ��  
                tbEntry.Properties["AppIsolated"][0] = "1";
                tbEntry.Properties["DontLog"][0] = true;  // �Ƿ��¼��־                  

                tbEntry.Properties["AuthFlags"][0] = 1;// ����Ŀ¼�İ�ȫ�ԣ�0��ʾ�������������ʣ�1Ϊ����3Ϊ���������֤��7Ϊwindows�̳������֤  
                tbEntry.CommitChanges();
                root.CommitChanges();//ȷ�ϸ���  

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "��Ϣ��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }



        /// <summary>
        /// ��ѹ��
        /// </summary>
        /// <param name="rarName">ѹ����������</param>
        /// <returns></returns>
        public void unRAR(string rarName)
        {
            string the_Info;
            ProcessStartInfo the_StartInfo;
            Process the_Process;
            try
            {
                RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\WinRAR.exe");//��̬��ȡWinRAR�İ�װ·��
                the_Info = " X  " + Context.Parameters["installdir"].ToString() + rarName + " " + Context.Parameters["installdir"].ToString();
                the_StartInfo = new ProcessStartInfo();
                the_StartInfo.FileName = key.GetValue("").ToString(); //@"C:\Program Files\WinRAR\WinRAR.exe";  //GetValue����ֵΪ��ʱ��ȡĬ��ֵ
                the_StartInfo.Arguments = the_Info;

                the_StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                the_Process = new Process();
                the_Process.StartInfo = the_StartInfo;
                the_Process.Start();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "��Ϣ��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// ��ȡCOM�����д���������ļ�
        /// </summary>
        private void WriteBat()
        {
            string path = Path.Combine(this.Context.Parameters["installdir"].ToString(), "Com");
            //��̬��ȡϵͳsystem32��·��
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
        /// ע��COM���
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
        /// �رս���
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
        /// ж����Ŀʱһ��ɾ��IIS�µ�WEBվ��
        /// </summary>
        private void DeleteWebSite(string siteName)
        {
            if (!IsExistSiteName(siteName))
            {
                return;
            }
            //�ر� w3wp.exe ���� ��ֹվ�㲻��ɾ��
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
        /// ɾ�������ݷ�ʽ
        /// </summary>
        private void DeleteDeskTopShortcut()
        {
            _dbconfigPath = Path.Combine(this.Context.Parameters["installdir"].ToString(), "dbconfig.xml");
            string siteName = OperateXML.GetXmlNodeValue(_dbconfigPath, "ShortcutName");

            //ɾ����ݷ�ʽ
            string deskPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

            deskPath = deskPath + @"\" + siteName + ".url ";

            if (File.Exists(deskPath))
            {
                File.Delete(deskPath);
            }
        }


        /// <summary>
        /// ���������ݷ�ʽ
        /// </summary>
        private void CreateDeskTopShortcut()
        {
            try
            {
                //��ȡ����·��
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
                MessageBox.Show("���������ݷ�ʽ�쳣��" + e.Message, "��Ϣ��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// ��ע���·��
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
        /// ɾ����ʼ�˵���ݷ�ʽ
        /// </summary>
        private void DeleteProgramsShortcut()
        {
            _dbconfigPath = Path.Combine(this.Context.Parameters["installdir"].ToString(), "dbconfig.xml");
            string siteName = OperateXML.GetXmlNodeValue(_dbconfigPath, "ShortcutName");

            //SHGetSpecialFolderPath(IntPtr.Zero, _pathBuilder, CSIDL_PROGRAMS, false);
            //string programsPath = _pathBuilder.ToString();
            //MessageBox.Show(programsPath, "��Ϣ��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //RegistryKey folders;
            //folders = OpenRegistryPath(Registry.CurrentUser, @"/software/microsoft/windows/currentversion/explorer/shell folders");

            //ɾ����ݷ�ʽ   folders.GetValue("Programs").ToString();

            string programsPath = Environment.GetFolderPath(Environment.SpecialFolder.Programs);
            programsPath = programsPath + @"\��Ĥ����ϵͳ-�����\" + siteName + ".url ";

            if (File.Exists(programsPath))
            {
                File.Delete(programsPath);
            }
        }

        /// <summary>
        /// ������ʼ�˵���ݷ�ʽ
        /// </summary>
        private void CreateProgramsShortcut()
        {
            try
            {
                //System.Environment.
                string programsPath = Environment.GetFolderPath(Environment.SpecialFolder.Programs);
                StreamWriter programsShortName = new StreamWriter(File.Open(programsPath + @"\��Ĥ����ϵͳ-�����\" + Context.Parameters["shortcutname"].ToString() + ".url ", FileMode.Create, FileAccess.Write));
                programsShortName.WriteLine("[InternetShortcut] ");
                programsShortName.WriteLine("URL=http://localhost:" + Context.Parameters["siteport"].ToString() + "/" + Context.Parameters["startpage"].ToString());
                programsShortName.WriteLine("Modified=00B21CE31E06C30199 ");

                programsShortName.Flush();
                programsShortName.Close();

            }
            catch (Exception e)
            {
                MessageBox.Show("������ʼ�˵���ݷ�ʽ�쳣��" + e.Message, "��Ϣ��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        /// <summary>
        /// ��ȡ��Դ�ļ��еĽű�
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
        /// ִ��sql���
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="sql"></param>
        void ExecuteSQL(SqlConnection connection, string sql)
        {
            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.ExecuteNonQuery();
        }
        #endregion

        #region ���������

        //private void Form1_Load(object sender, EventArgs e)
        //{
        //    //�������Ƿ���Ϊ��������
        //    Register rg = new Register();
        //    rg.SetStartWithWindow();
        //}

        /// <summary>
        /// ������������
        /// </summary>
        class Register
        {
            /// <summary>
            /// ����������
            /// </summary>
            private string _name = string.Empty;

            /// <summary>
            /// ������·��
            /// </summary>
            private string _path = string.Empty;

            /// <summary>
            /// ��������ע�����
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
                //δ���ÿ�������
                if (run.GetValue(_name) == null)
                {
                    SetRegister();
                }
            }


            /// <summary>
            /// ж�ؿ�������ע���
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
                //δ���ÿ�������
                if (run.GetValue(_name) != null)
                {
                    DeleteRegister();
                }
            }

            /// <summary>
            /// ������Ŀ�������д��ע���
            /// </summary>
            private void SetRegister()
            {
                //string starupPath = Application.ExecutablePath;
                //class Micosoft.Win32.RegistryKey. ��ʾWindowע�������ڵ�,������ע���װ.
                RegistryKey loca = Registry.LocalMachine;
                RegistryKey run = loca.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");

                try
                {
                    run.SetValue(_name, _path);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message.ToString(), "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    loca.Close();
                    run.Close();
                }
            }

            /// <summary>
            /// ������Ŀ���������ע�����ɾ��
            /// </summary>
            private void DeleteRegister()
            {
                //string starupPath = Application.ExecutablePath;
                //class Micosoft.Win32.RegistryKey. ��ʾWindowע�������ڵ�,������ע���װ.

                RegistryKey hklm = Registry.LocalMachine;
                RegistryKey run = hklm.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                try
                {
                    run.DeleteValue(_name);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message.ToString(), "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    hklm.Close();
                    run.Close();
                }
            }

        }

        #endregion

        #region ��ȡһ����վ���//һ���������Ϊվ������

        /// <summary>
        /// �������Ϊ վ��������� Ĭ����վ������Ϊ "Ĭ����վ"
        /// 2013-03-20 fjf ������վ��ź���
        /// <exception cref="NotFoundWebSiteException">��ʾû���ҵ���վ</exception>
        public string GetWebSiteNum(string siteName)
        {
            Regex regex = new Regex(siteName);
            string tmpStr;
            //�ڶ�������ΪHostName
            //�����http://blog.csdn.net/jxufewbt/article/details/849764
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
            throw new Exception("û���ҵ�������Ҫ��վ��" + siteName);
        }
        #endregion

        #region Start��Stop��վ�ķ��� 2013-03-20 fjf

        /// <summary>
        /// ������վ���°汾 2013-03-20 fjf 
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
                MessageBox.Show("CreateNewWebSite�����쳣 " + e.Message, "��Ϣ��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// ֹͣ��վվ��
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

        #region ���ַ����̳߳صķ���

        /// <summary>
        /// ����Ӧ�ó���� һ�ַ���
        /// </summary>
        /// <param name="newvdir"></param>
        /// <param name="AppPoolName"></param>
        public void AssignAppPool(DirectoryEntry newvdir, string AppPoolName)
        {
            object[] param = { 0, AppPoolName, true };
            newvdir.Invoke("Appcreate3", param);
        }

        /// <summary>
        /// ��������غ������ӦӦ�ó�������Ŀ¼ �ڶ���
        /// </summary>
        public static void SetAppToPool(string appname)
        {
            //��ȡĿ¼
            DirectoryEntry getdir = new DirectoryEntry("IIS://localhost/W3SVC");
            foreach (DirectoryEntry getentity in getdir.Children)
            {
                if (getentity.SchemaClassName.Equals("IIsWebServer"))
                {
                    //����Ӧ�ó������� �Ȼ��Ӧ�ó��� ���趨Ӧ�ó������� 
                    //��һ�β��Ը�Ŀ¼
                    foreach (DirectoryEntry getchild in getentity.Children)
                    {
                        if (getchild.SchemaClassName.Equals("IIsWebVirtualDir"))
                        {
                            //�ҵ�ָ��������Ŀ¼.
                            foreach (DirectoryEntry getsite in getchild.Children)
                            {
                                if (getsite.Name.Equals(appname))
                                {
                                    //�����Գɹ�ͨ���� 
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
