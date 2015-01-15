/*************************************************************************
** 文件名:   XMLSettings.cs
×× 主要类:   XMLSettings
**  
** Copyright (c) 中科虹霸有限公司公司
** 创建人:   cty
** 日  期:   2013-07-23
** 修改人:   
** 日  期:
** 描  述:   管理XML配置文件
** 功  能:   管理类：主要用于管理XML配置文件
** 版  本:   1.0.0
** 备  注:   命名及代码编写遵守C#编码规范
**
 * ***********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace BackUpDatabaseServer
{
    class XMLSettings
    {
        #region 私有变量

        /// <summary>
        /// XML文档变量
        /// </summary>
        private XmlDocument _docXmlSettings = null;

        /// <summary>
        /// XML文件名
        /// </summary>
        private string _xmlFileName;

        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="fileName">读取的配置文件的名称</param>
        public XMLSettings(string fileName)
        {
            try
            {
                this._docXmlSettings = new XmlDocument();
                //获取程序的基目录
                string path = System.AppDomain.CurrentDomain.BaseDirectory;
                this._xmlFileName = Path.Combine(path,fileName);

                if (!File.Exists(this._xmlFileName))
                {
                    Console.WriteLine(DateTime.Now.ToString() + " 未找到配置文件:" + _xmlFileName);
                }
                Console.WriteLine(DateTime.Now.ToString() + " 找到配置文件:" + _xmlFileName);
                this._docXmlSettings.Load(_xmlFileName);
            }
            catch (Exception e)
            {
                Console.WriteLine(DateTime.Now.ToString() + " 错误分析文件 :" + this._xmlFileName + e.Message.ToString());
            }
        }
        #endregion

        #region 读取并处理配置字符串
        /// <summary>
        /// 读取XML配置文件
        /// </summary>
        /// <param name="parent">父节点</param>
        /// <param name="key">键</param>
        /// <param name="Default">键值</param>
        /// <returns>返回配置字符串</returns>
        public string ReadXMLString(string parent, string key, string readStr)
        {
            try
            {
                XmlElement rootElement = this._docXmlSettings.DocumentElement;
                XmlNode xmlKeyNode = this._docXmlSettings.SelectSingleNode("/backupdatabase/" + parent + "/" + key);

                if (xmlKeyNode != null)
                {
                    return xmlKeyNode.InnerText;
                }

                //返回默认值
                return null;
            }
            catch
            {
                Console.WriteLine(DateTime.Now.ToString() + " 读取XML文件失败！");
                return null;
            }
        }
        #endregion

        #region 写入配置字符串

        /// <summary>
        /// 建立并写入XML文件
        /// </summary>
        /// <param name="parent">父节点</param>
        /// <param name="key">键</param>
        /// <param name="value">键值</param>
        /// <returns>是否成功</returns>
        public bool WriteXMLString(string parent, string key,string value) 
        {
            try
            {
                //根节点
                XmlElement rootElement = this._docXmlSettings.DocumentElement;

                //选择根节点子集中匹配 XPath 表达式的第一个 XmlNode
                //XPath的具体定义可参看MSDN或XML书籍
                XmlNode xmlKeyNode = rootElement.SelectSingleNode("/backupdatabase/" + parent + "/" + key);

                //读取出考勤库的数据库名字
                string irisAppDatabaseName = ReadXMLString("backup", "irisAppDatabaseName", "");
                XmlNode xmlKeyNodeAppName = rootElement.SelectSingleNode("/backupdatabase/" + "backup" + "/" + "irisAppDatabaseName");

                //读取出虹膜库的数据库名字
                string irisDataDatabaseName = ReadXMLString("backup", "irisDataDatabaseName", "");
                XmlNode xmlKeyNodeDataName = rootElement.SelectSingleNode("/backupdatabase/" + "backup" + "/" + "irisDataDatabaseName");

                if (xmlKeyNode != null)
                {
                    xmlKeyNode.InnerText = value;
                    xmlKeyNodeAppName.InnerText = irisAppDatabaseName;
                    xmlKeyNodeDataName.InnerText = irisDataDatabaseName;

                    this._docXmlSettings.Save(this._xmlFileName);
                }
                return true;
            }
            catch
            {
                Console.WriteLine(DateTime.Now.ToString() + " 写入XML文件失败！");
                return false;
            }
        }
        #endregion
    }
}
