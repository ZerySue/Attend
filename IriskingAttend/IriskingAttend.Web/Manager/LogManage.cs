///*************************************************************************
//** 文件名:   LogManage.cs
//×× 主要类:   LogManage
//**  
//** Copyright (c) 中科虹霸有限公司公司
//** 创建人:   fjf
//** 日  期:   2012-04-19
//** 修改人:   
//** 日  期:
//** 描  述:   日志类
//** 功  能:   日志类：主要用于记录日志信息
//** 版  本:   1.0.0
//** 备  注:   命名及代码编写遵守C#编码规范
//**
// * ***********************************************************************/

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.IO;
//using System.Windows.Forms;
//using System.Reflection;
//using System.Resources;
//using System.Drawing;

//namespace Irisking.Manager
//{
//    class LogManage
//    {
        
//        #region  构造函数
//        public LogManage()
//        {
//        }
//        #endregion

//        #region 变量
//        private static string m_strFileName;
//        #endregion


//        #region 属性
//        /// <summary>
//        /// 日志文件名
//        /// </summary>
//        public static string FileName
//        {
//            set
//            {
//                m_strFileName = value;
//            }
//        }
//        /// <summary>
//        /// 日志内容
//        /// </summary>
//        public static string Log
//        {
           
//            set 
//            {
//                WriteLog(value);
//            }
//        }
//        /// <summary>
//        /// 日志文件所有记录
//        /// </summary>
//        public static string[] AllLog
//        {
//            get
//            {
//                return ReadLog();
//            }
//        }
//        public static string ErrLog
//        {
//            //get
//            //{ 
//            //}
//            set
//            {
//                WriteErrLog(value);     
//            }
//        }
//        #endregion

//        #region 函数
//        /// <summary>
//        /// 写日志
//        /// </summary>
//        /// <param name="strLog">日志内容</param>
//        public static void WriteLog(string strLog)
//        {
//            string sFileName = String.Format("{0:yyyyMMdd}Log.txt", DateTime.Now);
//            using (StreamWriter sw = File.AppendText(sFileName))
//            {
//                string strTime = DateTime.Now.ToString().Trim();
//                sw.WriteLine(strTime+ ":"+strLog);
//                sw.WriteLine("------------------------------------------");
//                sw.Close();
//            }
//        }

//        /// <summary>
//        /// 写入指定路径日志
//        /// </summary>
//        /// <param name="strLog">日志内容</param>
//        /// <param name="sFilePath">日志路径</param>
//        public static void WriteLog(string strLog, string sFilePath)
//        {
//            using (StreamWriter sw = File.AppendText(sFilePath))
//            {
//                string strTime = DateTime.Now.ToString().Trim();
//                sw.WriteLine(strTime + ":" + strLog);                
//                sw.WriteLine("------------------------------------------");
//                sw.Close();
//            }
//        }

//        /// <summary>
//        /// 写错误日志
//        /// </summary>
//        /// <param name="strErrorLog"></param>
//        private static void WriteErrLog(string strErrorLog)
//        {
//            string sFileName = String.Format("ErrorLog{0:yyyyMMdd}.txt", DateTime.Now);
//            using (StreamWriter sw = File.AppendText(sFileName))
//            {
//                sw.WriteLine(strErrorLog);
//                sw.WriteLine("--------------------------------");
//                // Close the writer and underlying file.
//                sw.Close();
//            }
//        }

//        /// <summary>
//        /// 写指定路径的错误日志
//        /// </summary>
//        /// <param name="strErrorLog"></param>
//        /// <param name="sFilePath"></param>
//        private static void WriteErrLog(string strErrorLog, string sFilePath)
//        {
//            using (StreamWriter sw = File.AppendText(sFilePath))
//            {
//                sw.WriteLine(strErrorLog);
//                sw.WriteLine("--------------------------------");
//                // Close the writer and underlying file.
//                sw.Close();
//            }
//        }   
     
//        /// <summary>
//        /// 读所有文本行
//        /// </summary>
//        /// <param name="sFileName"></param>
//        /// <returns></returns>
//        private static string[] ReadLog()
//        {
//            string[] ReadText = File.ReadAllLines(m_strFileName,Encoding.Default);
//            return ReadText;
//        }

//        /// <summary>
//        /// 清空日志内容
//        /// </summary>
//        public static bool Clear()
//        {
//            if (m_strFileName == "" || m_strFileName == null)
//            {
//                MessageBox.Show("没有打开的文件！");
//                return false;
//            }
//            else
//            {
//                if (MessageBox.Show("确实要清空日志内容吗？", "询问", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
//                {
//                    File.WriteAllText(m_strFileName, "", Encoding.Default);
//                    return true;
//                }
//                else
//                {
//                    return false;
//                }
//            }
//        }
//        #endregion

//        #region 读资源文件
//        /// <summary>
//        /// 暂未测试成功
//        /// </summary>
//        /// <returns></returns>
//        public static Image GetResourceImage()
//        {
//            Assembly asm = Assembly.GetEntryAssembly();
//            //MyNamespace是你程序的命名空间，test是指test.resources。
//            ResourceManager rm = new ResourceManager("CustomsClearanceXinJiang.CustomsClearanceXinJiang" ,asm);
//            return (Image)rm.GetObject("281");
//            //pictureBox1.Image = (Image)rm.GetObject("progress");  //读图片
//            //textBox1.Text = rm.GetString("loading");  //读文字
//        }
//        #endregion

//        #region IP转换

//        ///<summary>
//        ///这个函数的目的把十六进制的数字转换成IP地址， 以字符串的形式返回
//        ///</summary>
//        public static string GetIpAddress(UInt32 IpAddress)
//        {
//            string ip = "";                             //保存结果的字符串

//            //取出末尾的数字
//            UInt32 value = 0x000000FF;
//            UInt32 Result = IpAddress & value;

//            //移位运算
//            IpAddress = IpAddress >> 8;
//            value = 0x0000FF;
//            UInt32 Result2 = IpAddress & value;

//            //移位运算取出下一个数值
//            IpAddress = IpAddress >> 8;
//            value = 0x00FF;
//            UInt32 Result3 = IpAddress & value;

//            //移位运算取出最后一个数字来
//            IpAddress = IpAddress >> 8;

//            //对IP进行赋值
//            ip += IpAddress;
//            ip += ".";
//            ip += Result3;
//            ip += ".";
//            ip += Result2;
//            ip += ".";
//            ip += Result;

//            return ip;
//        }
//        /// <summary>
//        /// 获得数字形式IP地址 
//        /// </summary>
//        /// <param name="sIp"></param>
//        /// <returns></returns>
//        public static UInt32 GetIpAddress(string sIp)
//        {
//            UInt32 iIP = 0;
//            string[] sAddr = sIp.Split('.');
//            try
//            {
//                UInt32 ip1 = UInt32.Parse(sAddr[0]);
//                UInt32 ip2 = UInt32.Parse(sAddr[1]);
//                UInt32 ip3 = UInt32.Parse(sAddr[2]);
//                UInt32 ip4 = UInt32.Parse(sAddr[3]);
//                iIP = (ip1 << 24) + (ip2 << 16) + (ip3 << 8) + ip4;
//            }
//            catch (Exception)
//            {
//                MessageBox.Show("IP地址转换错误", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//            }

//            return iIP;
//        }

//        #endregion

//    }
//}
