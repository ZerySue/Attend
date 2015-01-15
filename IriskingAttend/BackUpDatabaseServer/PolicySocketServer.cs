/*************************************************************************
** 文件名:   PolicySocketServer.cs
×× 主要类:   PolicySocketServer
**  
** Copyright (c) 中科虹霸有限公司公司
** 创建人:   cty
** 日  期:   2013-07-23
** 修改人:   
** 日  期:
** 描  述:   发送策略文件服务类
** 功  能:   发送策略文件服务类：服务器端的发送策略文件服务的代码类
** 版  本:   1.0.0
** 备  注:   命名及代码编写遵守C#编码规范
**
 * ***********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;

namespace BackUpDatabaseServer
{
    /// <summary>
    /// 服务器端的发送策略文件服务的代码类
    /// </summary>
    class PolicySocketServer
    {
        #region 私有变量声明

        /// <summary>
        /// 侦听连接
        /// </summary>
        private TcpListener _listener = null;

        /// <summary>
        /// TCP连接
        /// </summary>
        private TcpClient _client = null;

        /// <summary>
        /// 通知一个或多个正在等待的线程已发生事件
        /// </summary>
        private static ManualResetEvent _tcpClientConnected = new ManualResetEvent(false);

        /// <summary>
        /// 客户端发送的请求指令
        /// </summary>
        private const string _policyRequestString = "<policy-file-request/>";

        /// <summary>
        /// 接收字符串的长度
        /// </summary>
        private int _receivedLength = 0;

        /// <summary>
        /// 存放读取的配置文件
        /// </summary>
        private byte[] _policy = null;

        /// <summary>
        /// 接收请求的长度
        /// </summary>
        private byte[] _receiveBuffer = null;

        /// <summary>
        /// 读取配置文件的初始化
        /// </summary>
        private XMLSettings _xmlSet = new XMLSettings("backupdatabase.xml");

        /// <summary>
        /// 服务器端的IP地址
        /// </summary>
        private IPAddress _ipAddress;

        #endregion

        #region 获取IP地址，获取策略文件
        /// <summary>
        /// 获取策略文件
        /// </summary>
        private void InitializeData()
        {
            //读取配置文件获取IP地址
            string BackUpServerIP = _xmlSet.ReadXMLString("backup", "BackUpServerIP", "");

            Console.WriteLine(DateTime.Now.ToString() + " BackUpServerIP：" + BackUpServerIP);

            _ipAddress = IPAddress.Parse(BackUpServerIP);

            //获取策略文件
            string path = System.AppDomain.CurrentDomain.BaseDirectory;
            string policyFile = Path.Combine(path, "clientaccesspolicy.cfg");

            if (!File.Exists(policyFile))
            {
                Console.WriteLine(DateTime.Now.ToString() + " 未找到策略文件：" + policyFile);
            }
            else
            {
                Console.WriteLine(DateTime.Now.ToString() + " 找到策略文件：" + policyFile);
            }
            using (FileStream fs = new FileStream(policyFile, FileMode.Open))
            {
                _policy = new byte[fs.Length];

                //从流中读取字节块并将该数据写入给定缓冲区中
                fs.Read(_policy, 0, _policy.Length);
            }
            _receiveBuffer = new byte[_policyRequestString.Length];
        }
        #endregion

        #region 启动服务开始侦听连接
        /// <summary>
        /// 启动服务开始侦听连接
        /// </summary>
        public void StartSocketServer()
        {
            InitializeData();
            try
            {
                //_Listener = new TcpListener(IPAddress.Any, 943);
                _listener = new TcpListener(_ipAddress, 943);
                _listener.Start();
                Console.WriteLine(DateTime.Now.ToString() + " 监听943端口的服务启动成功...");
                while (true)
                {
                    _tcpClientConnected.Reset();
                    _listener.BeginAcceptTcpClient(new AsyncCallback(OnBeginAccept), null);
                    _tcpClientConnected.WaitOne();
                }
            }
            catch (Exception)
            {
                Console.WriteLine(DateTime.Now.ToString() + " 监听943端口的服务启动失败！");
            }
        }
        /// <summary>
        /// 异步接收传入的连接，并异步接收数据
        /// </summary>
        /// <param name="ar"></param>
        private void OnBeginAccept(IAsyncResult ar)
        {
            _client = _listener.EndAcceptTcpClient(ar);
            _client.Client.BeginReceive(_receiveBuffer, 0, _policyRequestString.Length, SocketFlags.None,
                new AsyncCallback(OnReceiveComplete), null);
        }

        /// <summary>
        /// 阻塞侦听，并对接收的请求进行判断，
        /// </summary>
        /// <param name="ar"></param>
        private void OnReceiveComplete(IAsyncResult ar)
        {
            try
            {
                _receivedLength += _client.Client.EndReceive(ar);
                if (_receivedLength < _policyRequestString.Length)
                {
                    _client.Client.BeginReceive(_receiveBuffer, _receivedLength,
                        _policyRequestString.Length - _receivedLength,
                        SocketFlags.None, new AsyncCallback(OnReceiveComplete), null);
                    return;
                }
                string request = System.Text.Encoding.UTF8.GetString(_receiveBuffer, 0, _receivedLength);
                if (StringComparer.InvariantCultureIgnoreCase.Compare(request, _policyRequestString) != 0)
                {
                    _client.Client.Close();
                    return;
                }
                _client.Client.BeginSend(_policy, 0, _policy.Length, SocketFlags.None,
                    new AsyncCallback(OnSendComplete), null);
            }
            catch (Exception)
            {
                _client.Client.Close();
            }
            _receivedLength = 0;
            _tcpClientConnected.Set(); 
        }

        /// <summary>
        /// 返回消息的回调函数
        /// </summary>
        /// <param name="ar"></param>
        private void OnSendComplete(IAsyncResult ar)
        {
            try
            {
                _client.Client.EndSendFile(ar);
            }
            catch (Exception)
            {
            }
            finally
            {
                _client.Client.Close();
            }
        }
        #endregion
    }
}
