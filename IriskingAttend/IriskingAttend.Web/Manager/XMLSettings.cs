/*************************************************************************
** 文件名:   XMLSettings.cs
×× 主要类:   XMLSettings
**  
** Copyright (c) 中科虹霸有限责任公司
** 创建人:   fjf
** 日  期:   2012-04-24
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

namespace Irisking.Manager
{
    class XMLSettings:IXMLSettings
    {
        #region 变量
        /// <summary>
        /// XML文档变量
        /// </summary>
        private XmlDocument docXmlSettings = null;
        /// <summary>
        /// XML文件名
        /// </summary>
        private string xmlFileName;
        #endregion

        public XMLSettings(string FileName)
        {            
            try
            {
                this.docXmlSettings = new XmlDocument();
                this.xmlFileName = FileName;

                if (!File.Exists(this.xmlFileName))
                {
                    XmlDeclaration xmlDec = this.docXmlSettings.CreateXmlDeclaration("1.0", "GB2312",null);
                    XmlElement docNode = this.docXmlSettings.CreateElement("Document");
                    this.docXmlSettings.AppendChild(xmlDec);
                    this.docXmlSettings.AppendChild(docNode);
                    this.docXmlSettings.Save(FileName);
                }
                
                this.docXmlSettings.Load(this.xmlFileName);
            }
            catch (Exception e)
            {
                throw new FileNotFoundException("错误分析 " + "文件 :" + this.xmlFileName + e.Message.ToString());
            }
        }

        #region 公共属性
        /// <summary>
        /// XML文件名
        /// </summary>
        public string FileName
        {
            get
            { 
                return xmlFileName; 
            }
            set 
            { 
                this.xmlFileName = value;
            }
        }
        
        #endregion

        #region 接口继承成员

        public override string ToString()
        {
            return this.xmlFileName;
        }

        public bool EncryptFile(bool bEncrypt)
        {
            return false;
        }
        #region 写入配置字符串
        /// <summary>
        /// 建立并写入XML文件
        /// </summary>
        /// <param name="Parent">父节点</param>
        /// <param name="Key">键</param>
        /// <param name="Value">键值</param>
        /// <returns>是否成功</returns>
        public bool WriteXMLString(string Parent, string Key, string Value)
        {

            try
            {
                //根节点
                XmlElement rootElement = this.docXmlSettings.DocumentElement;
                //选择根节点子集中匹配 XPath 表达式的第一个 XmlNode
                //XPath的具体定义可参看MSDN或XML书籍
                XmlNode xmlKeyNode = rootElement.SelectSingleNode("/Document/" +  Parent + "/" + Key);
                
                if (xmlKeyNode != null) 
                {
                    xmlKeyNode.InnerText = Value;
                    this.docXmlSettings.Save(this.xmlFileName);
                }
                else 
                {
                    XmlNode xmlNewNode;
                    XmlNode XmlParentNode = rootElement.SelectSingleNode("/Document" + "/" + Parent);

                    if (XmlParentNode == null)
                    {
                        XmlParentNode = docXmlSettings.DocumentElement; 
                        xmlNewNode = docXmlSettings.CreateElement(Parent);
                        XmlParentNode.AppendChild(xmlNewNode);
                    }

                    XmlParentNode = rootElement.SelectSingleNode("/Document" + "/" + Parent);
                    xmlNewNode = docXmlSettings.CreateElement(Key);
                    xmlNewNode.InnerText = Value;
                    XmlParentNode.AppendChild(xmlNewNode);

                    this.docXmlSettings.Save(this.xmlFileName);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool WriteXMLValue(string Parent, string Key, Int16 Value)
        {
            return WriteXMLString(Parent, Key, Value.ToString());
        }

        public bool WriteXMLValue(string Parent, string Key, Int32 Value)
        {
            return WriteXMLString(Parent, Key, Value.ToString());
        }

        public bool WriteXMLValue(string Parent, string Key, Int64 Value)
        {
            return WriteXMLString(Parent, Key, Value.ToString());
        }

        public bool WriteXMLValue(string Parent, string Key, float Value)
        {
            return WriteXMLString(Parent, Key, Value.ToString());
        }

        public bool WriteXMLValue(string Parent, string Key, decimal Value)
        {
            return WriteXMLString(Parent, Key, Value.ToString());
        }

        public bool WriteXMLValue(string Parent, string Key, bool Value)
        {
            return WriteXMLString(Parent, Key, Value.ToString());
        }

        public bool WriteXMLValue(string Parent, string Key, byte Value)
        {
            return WriteXMLString(Parent, Key, Value.ToString());
        }

        public bool WriteXMLValue(string Parent, string Key, UInt16 Value)
        {
            return WriteXMLString(Parent, Key, Value.ToString());
        }

        public bool WriteXMLValue(string Parent, string Key, UInt32 Value)
        {
            return WriteXMLString(Parent, Key, Value.ToString());
        }

        public bool WriteXMLValue(string Parent, string Key, UInt64 Value)
        {
            return WriteXMLString(Parent, Key, Value.ToString());
        }
        #endregion
        #region 读取并处理配置字符串
        /// <summary>
        /// 读取XML配置文件
        /// </summary>
        /// <param name="Parent">父节点</param>
        /// <param name="Key">键</param>
        /// <param name="Default">键值</param>
        /// <returns>返回配置字符串</returns>
        public string ReadXMLString(string Parent, string Key, string Default)
        {
            try
            {
                XmlElement rootElement = this.docXmlSettings.DocumentElement;
                XmlNode xmlKeyNode = rootElement.SelectSingleNode("/Document/" + Parent + "/" + Key);
                if (xmlKeyNode != null)  
                {
                    return xmlKeyNode.InnerText;
                }

                //返回默认值
                return Default;
            }
            catch
            {
                return Default;
            }
        }

        public Int16 ReadXMLint16(string Parent, string Key, Int16 Default)
        {
            try
            {
                return Int16.Parse(ReadXMLString(Parent, Key, Default.ToString()));
            }
            catch
            {
                return Default;
            }
        }

        public int ReadXMLint32(string Parent, string Key, int Default)
        {
            try
            {
                return int.Parse(ReadXMLString(Parent, Key, Default.ToString()));
            }
            catch
            {
                return Default;
            }
        }
        /// <summary>
        /// 重载读取格式字符串
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="Key"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        public int ReadXMLint32(string Parent, string Key, System.Globalization.NumberStyles format, int Default)
        {
            try
            {
                return int.Parse(ReadXMLString(Parent, Key, Default.ToString()),format);
            }
            catch
            {
                return Default;
            }
        }

        public Int64 ReadXMLint64(string Parent, string Key, Int64 Default)
        {
            try
            {
                return Int64.Parse(ReadXMLString(Parent, Key, Default.ToString()));
            }
            catch
            {
                return Default;
            }
        }

        public float ReadXMLfloat(string Parent, string Key, float Default)
        {
            try
            {
                return float.Parse(ReadXMLString(Parent, Key, Default.ToString()));
            }
            catch
            {
                return Default;
            }
        }

        public decimal ReadXMLdecimal(string Parent, string Key, decimal Default)
        {
            try
            {
                return decimal.Parse(ReadXMLString(Parent, Key, Default.ToString()));
            }
            catch
            {
                return Default;
            }
        }

        public bool ReadXMLbool(string Parent, string Key, bool Default)
        {
            try
            {
                return bool.Parse(ReadXMLString(Parent, Key, Default.ToString()));
            }
            catch
            {
                return Default;
            }
        }

        public byte ReadXMLbyte(string Parent, string Key, byte Default)
        {
            try
            {
                return byte.Parse(ReadXMLString(Parent, Key, Default.ToString()));
            }
            catch
            {
                return Default;
            }
        }

        public UInt16 ReadXMLuint16(string Parent, string Key, UInt16 Default)
        {
            try
            {
                return UInt16.Parse(ReadXMLString(Parent, Key, Default.ToString()));
            }
            catch
            {
                return Default;
            }
        }

        public UInt32 ReadXMLuint32(string Parent, string Key, System.Globalization.NumberStyles format, UInt32 Default)
        {
            try
            {
                string sTemp = ReadXMLString(Parent, Key, Default.ToString());
                return UInt32.Parse(sTemp,format);
            }
            catch
            {
                return Default;
            }
        }

        public UInt64 ReadXMLuint64(string Parent, string Key, UInt64 Default)
        {
            try
            {
                return UInt64.Parse(ReadXMLString(Parent, Key, Default.ToString()));
            }
            catch
            {
                return Default;
            }
        }
        #endregion
        #endregion
    }
}
