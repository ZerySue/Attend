/*************************************************************************
** 文件名:   BackUpDataBase.cs
×× 主要类:   BackUpDataBase
**  
** Copyright (c) 北京中科虹霸科技有限公司
** 创建人:   cty
** 日  期:   2013-8-13
** 修改人:   
** 日  期:   
 *修改内容： 
** 描  述:   数据库备份的操作类，进行socket通讯传递备份参数
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
using System.Net.Sockets;
using System.ServiceModel.DomainServices.Client;
using EDatabaseError;
using System.Text;
using IriskingAttend.Dialog;
using IriskingAttend.Web;
using System.IO;
using System.Xml.Serialization;

namespace IriskingAttend
{
    #region 传递备份参数的结构体

    /// <summary>
    /// 传递备份参数的结构体
    /// </summary>
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
        /// 备虹膜库具体时间
        /// </summary>
        public DateTime ConcreteTimeIrisData;
    }

    #endregion

    public class BackUpDatabase
    {

        #region 变量

        /// <summary>
        /// 域服务声明 
        /// </summary>
        private DomainServiceIriskingAttend _domSrvDbAccess = new DomainServiceIriskingAttend();

        /// <summary>
        /// socket声明
        /// </summary>
        private Socket _socket;

        /// <summary>
        /// 备份数据库的IP地址
        /// </summary>
        private string _backupServerIP;

        /// <summary>
        /// 备份数据库的端口号
        /// </summary>
        private int _backupServerPort;

        #endregion

        /// <summary>
        /// 取得备份数据库服务端返回值的事件
        /// </summary>
        public event EventHandler JudgeSuccess;

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public BackUpDatabase()
        {
 
        }

        #endregion

        /// <summary>
        /// 备份函数
        /// </summary>
        /// <param name="backupPara">结构体</param>
        public void BackUpOperation(BackupParameter backupPara)
        {
            #region 获取备份数据库的IP地址
            try
            {
                Action<InvokeOperation<string>> onInvokeErrCallBack = CallBackHandleControl<string>.OnInvokeErrorCallBack;
                CallBackHandleControl<string>.m_sendValue = (ip) =>
                {
                    //将应用程序类型赋给变量，整个程序都将利用此变量来获得应用程序类型。
                    _backupServerIP = ip;
                    #region 获取备份数据库的端口号
                    try
                    {
                        Action<InvokeOperation<string>> onInvokeErrCallBackPort = CallBackHandleControl<string>.OnInvokeErrorCallBack;
                        CallBackHandleControl<string>.m_sendValue = (port) =>
                        {
                            _backupServerPort = Convert.ToInt32(port);
                            #region 监听
                            //将应用程序类型赋给变量，整个程序都将利用此变量来获得应用程序类型。
                            //_backupServerPort = Convert.ToInt32(port);
                            DnsEndPoint hostEntry = new DnsEndPoint(_backupServerIP, _backupServerPort);
                            //创建一个Socket对象
                            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                            //创建Socket异步事件参数
                            SocketAsyncEventArgs socketEventArg = new SocketAsyncEventArgs();

                            MemoryStream ms = new MemoryStream();
                            XmlSerializer xml = new XmlSerializer(typeof(BackupParameter));

                            //将对象序列化为流
                            xml.Serialize(ms, backupPara);

                            //将消息转化为发送的byte[]格式
                            byte[] buffer = ms.ToArray();

                            //注册Socket完成事件
                            socketEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(SocketEventArg_Completed);
                            //设置Socket异步事件远程终结点
                            socketEventArg.RemoteEndPoint = hostEntry;
                            //将定义好的Socket对象赋值给Socket异步事件参数的运行实例属性
                            socketEventArg.UserToken = buffer;
                            try
                            {
                                _socket.ConnectAsync(socketEventArg);
                            }
                            catch (SocketException ex)
                            {
                                throw new SocketException((int)ex.ErrorCode);
                            }
                            #endregion
                        };

                        //调用后台,获得应用程序类型
                        _domSrvDbAccess.GetBackupServerPort(onInvokeErrCallBackPort, null);
                    }
                    catch (Exception e)
                    {
                        ErrorWindow err = new ErrorWindow(e);
                        err.Show();
                        return;
                    }
                    #endregion

                };

                //调用后台,获得应用程序类型
                _domSrvDbAccess.GetBackupServerIP(onInvokeErrCallBack, null);
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
                return;
            }
            #endregion
        }

        /// <summary>
        /// 向服务器发送数据，并接受服务器回复的消息。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SocketEventArg_Completed(object sender, SocketAsyncEventArgs e)
        {
            //检查是否发送出错
            if (e.SocketError != SocketError.Success)
            {
                if (e.SocketError == SocketError.ConnectionAborted)
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() => JudgeSuccess(SplitString("error,连接超时....请重试！"), new EventArgs()));
                }
                else if (e.SocketError == SocketError.ConnectionRefused)
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() => JudgeSuccess(SplitString("error,远程主机正在主动拒绝连接!"), new EventArgs()));
                }
                else
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() => JudgeSuccess(SplitString("error,读取参数失败，请检查服务端是否正常开启！"), new EventArgs()));
                }
                return;
            }
            //如果连接上，则发送数据
            if (e.LastOperation == SocketAsyncOperation.Connect)
            {
                byte[] userbytes = (byte[])e.UserToken;
                e.SetBuffer(userbytes, 0, userbytes.Length);
                _socket.SendAsync(e);

            }//如果已发送数据，则开始接收服务器回复的消息
            else if (e.LastOperation == SocketAsyncOperation.Send)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {

                });
                byte[] userbytes = new byte[1024];
                e.SetBuffer(userbytes, 0, userbytes.Length);
                _socket.ReceiveAsync(e);
            }
            //接收服务器返回的数据
            else if (e.LastOperation == SocketAsyncOperation.Receive)
            {
                string recevieStr = Encoding.UTF8.GetString(e.Buffer, 0, e.Buffer.Length).Replace("\0", "");
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    JudgeSuccess(SplitString(recevieStr), new EventArgs());
                });
                _socket.Close();
            }
        }

        /// <summary>
        /// 按逗号拆分字符串，并返回拆分后的第一个字符串
        /// </summary>
        /// <param name="receiveStr">要拆分的字符串</param>
        /// <returns></returns>
        private string[] SplitString(string receiveStr)
        {
            string[] strarr = receiveStr.Split(',');
            return strarr;
        }

    }
}
