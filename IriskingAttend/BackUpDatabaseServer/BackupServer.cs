/*************************************************************************
** 文件名:   BackupServer.cs
×× 主要类:   BackupServer
**  
** Copyright (c) 中科虹霸有限公司公司
** 创建人:   cty
** 日  期:   2013-07-23
** 修改人:   
** 日  期:
** 描  述:   数据库备份服务类
** 功  能:   数据库备份服务类：服务器端的监听客户端发送请求服务的代码类
** 版  本:   1.0.0
** 备  注:   命名及代码编写遵守C#编码规范
**
 * ***********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;
using System.Timers;
using System.IO;
using System.Xml.Serialization;

namespace BackUpDatabaseServer
{
    class BackupServer
    {
        #region 私有变量声明
        /// <summary>
        /// 服务器端的IP地址
        /// </summary>
        private IPAddress _ipAddress;

        /// <summary>
        /// 监听的IP地址和端口号
        /// </summary>
        private IPEndPoint _localEndPoint;

        /// <summary>
        /// 声明一个socket
        /// </summary>
        private Socket _listener;

        /// <summary>
        /// 返回客户端的消息类
        /// </summary>
        private SendMessage _sendMessage =new SendMessage();

        /// <summary>
        /// 通知一个或多个正在等待的线程已发生事件 控制主线程等待
        /// </summary>
        private static AutoResetEvent _autoResetEvent = new AutoResetEvent(false);

        /// <summary>
        /// 监听服务线程
        /// </summary>
        private Thread _threadWatch;

        /// <summary>
        /// 自动备份中考勤库定时器的间隔
        /// </summary>
        private int _intervalIrisApp = 30000;

        /// <summary>
        /// 自动备份中虹膜库定时器的间隔
        /// </summary>
        private int _intervalIrisData = 30000;

        /// <summary>
        /// 定义考勤库的定时器
        /// </summary>
        private System.Timers.Timer _backUpIrisAppTimer = new System.Timers.Timer();

        /// <summary>
        /// 定义虹膜库的定时器
        /// </summary>
        private System.Timers.Timer _backUpIrisDataTimer = new System.Timers.Timer();

        #endregion

        #region Main函数
        
        static void Main(string[] args)
        {
            BackupServer backupServer = new BackupServer();
            Console.WriteLine(DateTime.Now.ToString() + " -------------------------------------------------");           

            #region Start The Policy Server 开启监听验证策略文件的服务

            PolicySocketServer _StartPolicyServer = new PolicySocketServer();
            Thread thread = new Thread(new ThreadStart(_StartPolicyServer.StartSocketServer));
            thread.IsBackground = true;
            thread.Start();         

            #endregion
           
            backupServer.StartTimerServer();
            backupServer.StartListenServer();

            //主线程无限期等待
            _autoResetEvent.WaitOne(-1);
  
        }
        #endregion

        #region 启动定时器服务，实现自动备份功能
        /// <summary>
        /// 定义计时器
        /// </summary>
        private void StartTimerServer()
        {
            //考勤库的定时器
            _backUpIrisAppTimer.Interval = _intervalIrisApp;
            _backUpIrisAppTimer.Enabled = true;
            _backUpIrisAppTimer.Elapsed += new ElapsedEventHandler(BackUpIrisAppTimer_Tick);

            Console.WriteLine(DateTime.Now.ToString() + " 启动备份考勤库定时器...");

            //虹膜库的定时器 
            _backUpIrisDataTimer.Interval = _intervalIrisData;
            _backUpIrisDataTimer.Enabled = true;
            _backUpIrisDataTimer.Elapsed += new ElapsedEventHandler(BackUpIrisDataTimer_Tick);

            Console.WriteLine(DateTime.Now.ToString() + " 启动备份虹膜库定时器...");
        }

        /// <summary>
        /// 备份考勤库的定时器事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackUpIrisAppTimer_Tick(object sender, EventArgs e)
        {
            Console.WriteLine(DateTime.Now.ToString() + " 备份考勤库的定时器事件");
            if (JudgeBackupTime("irisAppPeriod", "irisAppSubPeriod", "irisAppConcreteTime"))
            {
                //读取配置文件中的备份路径及备份的数据库名称  
                string irisAppPath = GetXML().ReadXMLString("backup", "irisAppPath", "");
                string irisAppDatabaseName = GetXML().ReadXMLString("backup", "irisAppDatabaseName", "");

                try
                {
                    //判断备份的目录文件夹是否存在，若不存在，就创建
                    if (!Directory.Exists(irisAppPath))
                    {
                        Directory.CreateDirectory(irisAppPath);
                        Console.WriteLine(DateTime.Now.ToString() + " 创建备份目录：" + irisAppPath);
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(DateTime.Now.ToString() + " 考勤定时器创建目录失败：" + ex.Message);
                    return;
                }

                Console.WriteLine(DateTime.Now.ToString() + " 执行考勤库的定时器事件,备份地址及数据库名字：irisAppPath =" + irisAppPath + ",irisAppDatabaseName = " + irisAppDatabaseName);

                StartBackUp(irisAppPath + "/" + irisAppDatabaseName + "-" + DateTime.Now.ToString("yyyyMMdd") + ".bak", irisAppDatabaseName);
            }        
        }

        /// <summary>
        /// 备份虹膜库的定时器事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackUpIrisDataTimer_Tick(object sender, EventArgs e)
        {
            Console.WriteLine(DateTime.Now.ToString() + " 备份虹膜库的定时器事件");
            if (JudgeBackupTime("irisDataPeriod", "irisDataSubPeriod", "irisDataConcreteTime"))
            {
                //读取配置文件中的备份路径及备份的数据库名称
                string irisDataPath = GetXML().ReadXMLString("backup", "irisDataPath", "");
                string irisDataDatabaseName = GetXML().ReadXMLString("backup", "irisDataDatabaseName", "");

                try
                {
                    //判断备份的目录文件夹是否存在，若不存在，就创建
                    if (!Directory.Exists(irisDataPath))
                    {
                        Directory.CreateDirectory(irisDataPath);
                        Console.WriteLine(DateTime.Now.ToString() + " 创建备份目录：" + irisDataPath);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(DateTime.Now.ToString() + " 虹膜定时器创建目录失败：" + ex.Message);
                    return;
                }
               
                Console.WriteLine(DateTime.Now.ToString() + " 执行虹膜库的定时器事件,备份地址及数据库名字：irisDataPath =" + irisDataPath + ",irisDataDatabaseName = " + irisDataDatabaseName);
               
                StartBackUp(irisDataPath + "/" + irisDataDatabaseName + "-" + DateTime.Now.ToString("yyyyMMdd") + ".bak", irisDataDatabaseName);
            } 
        }

        /// <summary>
        /// 判断是否到了要备份的时间
        /// </summary>
        /// <param name="period">备份的周期</param>
        /// <param name="subPeriod">备份的日期</param>
        /// <param name="concreteTime">备份的具体时间</param>
        /// <returns>是否备份</returns>
        private bool JudgeBackupTime(string period,string subPeriod,string concreteTime)
        {
            int Period = int.Parse(GetXML().ReadXMLString("backup", period, ""));
            int SubPeriod = int.Parse(GetXML().ReadXMLString("backup", subPeriod, ""));
            DateTime ConcreteTime = DateTime.Parse(GetXML().ReadXMLString("backup", concreteTime, ""));

            DateTime dateTime = DateTime.Now;

            //Console.WriteLine(DateTime.Now.ToString() + " 备份数据库的时间：\n Period = " + Period.ToString() + "\n SubPeriod = " + SubPeriod.ToString() + "\n ConcreteTime = " + ConcreteTime.ToString() + ".");
    
            if (Period == 0)//备份周期为月
            {                
                int nowDay = dateTime.Day;
                if (nowDay != SubPeriod || ConcreteTime.Hour != dateTime.Hour || ConcreteTime.Minute != dateTime.Minute)
                {
                    return false;
                }
                return true;
            }
            else if (Period == 1)//备份周期为周
            {
                int nowDayOfWeek = Convert.ToInt32(dateTime.DayOfWeek);
                if (nowDayOfWeek != (SubPeriod % 7) || ConcreteTime.Hour != dateTime.Hour || ConcreteTime.Minute != dateTime.Minute)
                {
                    return false;
                }
                return true;
            }
            else if (Period == 2)//备份周期为天
            {
                if (ConcreteTime.Hour != dateTime.Hour || ConcreteTime.Minute != dateTime.Minute)
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        #endregion

        #region 启动监听服务,开启监听端口线程,循环接收客户端的消息
        /// <summary>
        /// 启动监听服务,开启监听端口线程,循环接收客户端的消息
        /// </summary>
        private void StartListenServer()
        {
            Console.WriteLine(DateTime.Now.ToString() + " 开始启动监听服务");
            //创建Socket
            _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //获取主机信息
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                    
            //读取配置文件IP和端口号
            string BackUpServerIP = GetXML().ReadXMLString("backup", "BackUpServerIP", "");
            string BackUpServerPort = GetXML().ReadXMLString("backup", "BackUpServerPort", "").ToString();

            _ipAddress = IPAddress.Parse(BackUpServerIP);
            //把IP和端口转换化为IPEndPoint实例,端口号取4530  
            _localEndPoint = new IPEndPoint(_ipAddress, int.Parse(BackUpServerPort));
            try
            {
                //绑定指定的终结点
                _listener.Bind(_localEndPoint);
                //开始监听
                _listener.Listen(10);
                //一直循环接收客户端的消息，开启监听端口线程
                ThreadStart threadwatchStart = new ThreadStart(WatchConnecting);

                _threadWatch = new Thread(threadwatchStart);
                _threadWatch.IsBackground = true;
                _threadWatch.Start();

                Console.WriteLine(DateTime.Now.ToString() + " 监听IP地址: " + BackUpServerIP );
                Console.WriteLine(DateTime.Now.ToString() + " 监听端口号: " + BackUpServerPort );
                Console.WriteLine(DateTime.Now.ToString() + " 监听服务启动成功... \n");
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToString() + " 服务启动失败！监听IP地址: " + BackUpServerIP + "监听端口号: " + BackUpServerPort + ", " + ex.Message);
                _autoResetEvent.Set();
            }      
        }

        /// <summary>
        /// 监听
        /// </summary>
        private void WatchConnecting()
        {
            _listener.BeginAccept(AcceptCallBack, _listener);
        }

        /// <summary>
        ///接收请求的回调
        /// </summary>
        /// <param name="asyresult"></param>
        private void AcceptCallBack(IAsyncResult asyresult)
        {
            Socket listener = (Socket)asyresult.AsyncState;
            Socket socket = listener.EndAccept(asyresult);
            var state = new StateObject();

            state.Socket = socket;
            socket.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, ReciverCallBack, state);

            listener.BeginAccept(AcceptCallBack, listener);
        }

        /// <summary>
        /// 收到请求信息的回调
        /// </summary>
        /// <param name="asyResult"></param>
        private void ReciverCallBack(IAsyncResult asyResult)
        {
            JudgeReceiveText(asyResult);
        }
        #endregion

        #region 对接收到的请求进行判断函数
        /// <summary>
        /// 对接收到的请求进行判断
        /// </summary>
        /// <param name="receive">接收到的请求</param>
        private void JudgeReceiveText(IAsyncResult receive)
        {
            try
            {
                StateObject state = (StateObject)receive.AsyncState;
                Socket socket = state.Socket;
                int read = socket.EndReceive(receive);

                MemoryStream ms = new MemoryStream(state.Buffer);
                XmlSerializer xml = new XmlSerializer(typeof(BackupParameter));
                BackupParameter? backupPara = (BackupParameter)xml.Deserialize(ms);

                if (backupPara != null)
                {
                    //控制台界面输出
                    Console.WriteLine(DateTime.Now.ToString() + " 接收到客户端发送来的备份请求！\n "
                        + backupPara.Value.ManOrAuto + ","
                        + backupPara.Value.DatabaseName + ","
                        + backupPara.Value.PathIrisApp + ","
                        + backupPara.Value.PeriodIrisApp + ","
                        + backupPara.Value.SubPeriokIrisApp + ","
                        + backupPara.Value.ConcreteTimeIrisApp + ","
                        + backupPara.Value.PathIrisData + ","
                        + backupPara.Value.PeriodIrisData + ","
                        + backupPara.Value.SubPeriokIrisData + ","
                        + backupPara.Value.ConcreteTimeIrisData);

                    #region 判断是否是手动备份，0-手动备份，1-自动备份,2-返回设置的自动备份参数
                    //判断是否是手动备份，0-手动备份，1-自动备份,2-返回设置的自动备份参数
                    if (backupPara.Value.ManOrAuto == 0)
                    {
                        if (backupPara.Value.DatabaseName == 0)
                        {
                            //手动备份考勤库
                            if (JudgePath(socket, 0, backupPara.Value.PathIrisApp))
                            {
                                string irisApp_DatabaseName = GetXML().ReadXMLString("backup", "irisAppDatabaseName", "");
                                if (StartBackUp(backupPara.Value.PathIrisApp, irisApp_DatabaseName) == 0)
                                {
                                    Send(socket, _sendMessage.MannualBackupTrue);
                                }
                                else
                                {
                                    Send(socket, _sendMessage.MannualBackupFalse);
                                }
                            }
                        }
                        else
                        {
                            //手动备份虹膜库
                            if (JudgePath(socket, 0, backupPara.Value.PathIrisData))
                            {
                                string irisData_DatabaseName = GetXML().ReadXMLString("backup", "irisDataDatabaseName", "");
                                if (StartBackUp(backupPara.Value.PathIrisData, irisData_DatabaseName) == 0)
                                {
                                    Send(socket, _sendMessage.MannualBackupTrue);
                                }
                                else
                                {
                                    Send(socket, _sendMessage.MannualBackupFalse);
                                }
                            }
                        }
                    }
                    #endregion

                    //自动备份
                    else if (backupPara.Value.ManOrAuto == 1)
                    {
                        if (JudgePath(socket, 1, backupPara.Value.PathIrisApp) && JudgePath(socket, 1, backupPara.Value.PathIrisData))
                        {
                            //写入考勤库自动备份的配置文件
                            GetXML().WriteXMLString("backup", "irisAppPath", backupPara.Value.PathIrisApp);
                            GetXML().WriteXMLString("backup", "irisAppPeriod", backupPara.Value.PeriodIrisApp.ToString());
                            GetXML().WriteXMLString("backup", "irisAppSubPeriod", backupPara.Value.SubPeriokIrisApp.ToString());
                            GetXML().WriteXMLString("backup", "irisAppConcreteTime", backupPara.Value.ConcreteTimeIrisApp.ToString());

                            //写入虹膜自动备份的配置文件
                            GetXML().WriteXMLString("backup", "irisDataPath", backupPara.Value.PathIrisData);
                            GetXML().WriteXMLString("backup", "irisDataPeriod", backupPara.Value.PeriodIrisData.ToString());
                            GetXML().WriteXMLString("backup", "irisDataSubPeriod", backupPara.Value.SubPeriokIrisData.ToString());
                            GetXML().WriteXMLString("backup", "irisDataConcreteTime", backupPara.Value.ConcreteTimeIrisData.ToString());

                            Send(socket, _sendMessage.AutoBackupParameterSetTrue);
                        }
                    }
                    //返回配置文件的备份信息给客户端
                    else
                    {
                        string irisAppPeriod = GetXML().ReadXMLString("backup", "irisAppPeriod", "");
                        string irisAppSubPeriod = GetXML().ReadXMLString("backup", "irisAppSubPeriod", "");
                        string irisAppConcreteTime = GetXML().ReadXMLString("backup", "irisAppConcreteTime", "");
                        string irisAppPath = GetXML().ReadXMLString("backup", "irisAppPath", "");

                        string irisDataPeriod = GetXML().ReadXMLString("backup", "irisDataPeriod", "");
                        string irisDataSubPeriod = GetXML().ReadXMLString("backup", "irisDataSubPeriod", "");
                        string irisDataConcreteTime = GetXML().ReadXMLString("backup", "irisDataConcreteTime", "");
                        string irisDataPath = GetXML().ReadXMLString("backup", "irisDataPath", "");

                        _sendMessage.SendAutoBackupParameter = irisAppPeriod + "," + irisAppSubPeriod + "," + irisAppConcreteTime + "," + irisAppPath + "," + irisDataPeriod + "," + irisDataSubPeriod + "," + irisDataConcreteTime + "," + irisDataPath;
                        Send(socket, _sendMessage.SendAutoBackupParameter);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(DateTime.Now.ToString() + " 错误：" + e.Message);
            }
              
        }

        /// <summary>
        /// 判断备份的地址是否合法正确
        /// </summary>
        /// <param name="socket">socket句柄</param>
        /// <param name="mark">标记是手动备份路径还是自动备份路径， 0-手动，1-自动</param>
        /// <param name="path">备份路径</param>
        private bool JudgePath(Socket socket, int mark, string path)
        {
            //手动备份
            if (mark == 0)
            {
                //获取路径的根目录信息
                string pathRoot = Path.GetPathRoot(path);

                //获取路径字符串的扩展名
                string pathExtension = Path.GetExtension(path);
                if (!Directory.Exists(pathRoot))
                {
                    Send(socket, _sendMessage.DiskNotExist);
                    return false;
                }

                if (pathExtension.ToLower() != ".bak" && pathExtension.ToLower() != ".txt")
                {
                    Send(socket, _sendMessage.PathFormat);
                    return false;
                }

                //返回指定路径字符串的目录信息
                string pathGetDirectoryName = Path.GetDirectoryName(path);
                if (!Directory.Exists(pathGetDirectoryName))
                {
                    try
                    {
                        Directory.CreateDirectory(pathGetDirectoryName);
                        return true;
                    }
                    catch (Exception e)
                    {
                        Send(socket, _sendMessage.PathFormat);
                        return false;
                    }
                }
            }       
            //自动备份
            else if (mark == 1)
            {
                //获取路径的根目录信息
                string pathRoot = Path.GetPathRoot(path);
                if (!Directory.Exists(pathRoot))
                {
                    Send(socket, _sendMessage.DiskNotExist);
                    return false;
                }
                if (!Directory.Exists(path))
                {
                    try
                    {
                        Directory.CreateDirectory(path);
                        return true;
                    }
                    catch (Exception e)
                    {
                        Send(socket, _sendMessage.PathFormat);
                        return false;
                    }
                }
            }
            return true;
        }

        #endregion

        #region 发送数据，返回消息给客户端，0-手动备份成功，1-自动备份设置参数成功，2-返回配置文件中自动备份参数

        /// <summary>
        /// 发送数据，返回消息给客户端，0-手动备份成功，1-自动备份设置参数成功，2-返回配置文件中自动备份参数
        /// </summary>
        /// <param name="handler">socket句柄</param>
        /// <param name="data">返回值</param>
        private void Send(Socket handler, String data)
        {
            Console.WriteLine(DateTime.Now.ToString() + " 准备发送消息给客户端," + data + "发送成功！");
            byte[] byteData = Encoding.UTF8.GetBytes(data);
            handler.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallBack), handler);
        }

        /// <summary>
        /// 返回消息的回调函数
        /// </summary>
        /// <param name="asyResult"></param>
        private void SendCallBack(IAsyncResult asyResult)
        {
            try
            {
                Socket handler = (Socket)asyResult.AsyncState;
                int byteSent = handler.EndSend(asyResult);
                if (byteSent > 0)
                {
                    Console.WriteLine(DateTime.Now.ToString() + " 发送成功！");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("发送消息给客户端发送失败！ " + ex.Data.ToString());
            }
        }

        #endregion

        #region 调用cmd执行dos命令进行备份函数
        /// <summary>
        /// 调用cmd执行dos命令进行备份
        /// </summary>
        /// <param name="backUpPath">备份路径</param>
        /// <param name="backUpDatabase">备份的数据库名字</param>
        private int StartBackUp(string backUpPath,string backUpDatabase)
        {
            //读取配置文件信息，获取IP地址，端口号，及数据库密码
            string DBHost = GetXML().ReadXMLString("backup", "DBHost", "");
            string DBPort = GetXML().ReadXMLString("backup", "DBPort", "");
            string DBPassword = GetXML().ReadXMLString("backup", "DBPassword", "");

            Console.WriteLine(DateTime.Now.ToString() + " 开始调用cmd窗口，执行备份操作......");
            Process process = null;
            process = new Process();

            process.StartInfo.FileName = "pg_dump.exe";
            string backupCmd= string.Format("-h {0} -p {1} -U postgres -f \"{2}\" \"{3}\"", DBHost, DBPort, backUpPath, backUpDatabase);
            Console.WriteLine(DateTime.Now.ToString() + " 备份命令：" + backupCmd);

            process.StartInfo.Arguments = backupCmd;

            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.EnvironmentVariables.Add("PGPASSWORD", DBPassword);

            process.ErrorDataReceived += new DataReceivedEventHandler(process_ErrorDataReceived);

            try
            {
                process.Start();
                process.BeginErrorReadLine();

                process.WaitForExit();

                int isSuccess = process.ExitCode;

                Console.WriteLine(DateTime.Now.ToString() + string.Format(" pg_dump exit with {0}", process.ExitCode));

                process.Close();

                Console.WriteLine(DateTime.Now.ToString() + " 关闭启用pg_dump的进程");

                return isSuccess;
            }
            catch (Exception e)
            {
                Console.WriteLine(DateTime.Now.ToString() + " 备份进程启动失败：" + e.Message);
                return 1;
            }
        }

        private static void process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {      
            if (e.Data != null && e.Data.Length != 0)
            {  
                Console.WriteLine(DateTime.Now.ToString() + " " + e.Data);              
            }
        }

        #endregion

        #region 获取配置文件

        /// <summary>
        /// 获取xml文件
        /// </summary>
        /// <returns></returns>
        public XMLSettings GetXML()
        {
            XMLSettings xmlSet = new XMLSettings("backupdatabase.xml");
            return xmlSet;
        }

        #endregion

    }

    /// <summary>
    /// 接收发送请求控制的代码类
    /// </summary>
    public class StateObject
    {
        /// <summary>
        /// socket记录连接
        /// </summary>
        public Socket Socket;

        /// <summary>
        /// 可变字符的字符串
        /// </summary>
        public StringBuilder StringBuilder = new StringBuilder();

        /// <summary>
        /// 定义长度
        /// </summary>
        public const int BufferSize = 1024;

        /// <summary>
        /// 定义数组
        /// </summary>
        public byte[] Buffer = new byte[BufferSize];
    }

    /// <summary>
    /// 服务端发送返回消息的类
    /// </summary>
    public class SendMessage
    {
        /// <summary>
        /// 手动备份成功
        /// </summary>
        public string MannualBackupTrue = "10";

        /// <summary>
        /// 手动备份失败
        /// </summary>
        public string MannualBackupFalse = "-10";

        /// <summary>
        /// 自动备份参数设置成功
        /// </summary>
        public string AutoBackupParameterSetTrue = "11";

        /// <summary>
        /// 自动备份参数设置失败
        /// </summary>
        public string AutoBackupParameterSetFalse = "-11";

        /// <summary>
        /// 备份路径磁盘不存在
        /// </summary>
        public string DiskNotExist = "12";

        /// <summary>
        /// 备份路径格式不正确
        /// </summary>
        public string PathFormat = "13";

        /// <summary>
        /// 返回备份参数
        /// </summary>
        public string SendAutoBackupParameter;
    }
}
