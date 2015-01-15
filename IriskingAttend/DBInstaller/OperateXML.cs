/*************************************************************************
** 文件名:   OperateXML.cs
×× 主要类:   OperateXML
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   
** 日  期:   
** 修改人:   lzc
** 日  期:   2013-8-12
** 描  述:   OperateXML类，XML文件读写类
**
** 版  本:   1.0.0
** 备  注:  命名及代码编写遵守C#编码规范
**
 * ***********************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Collections.Specialized;
using System.Xml;


    public class OperateXML
    {
        /// <summary>
        /// XML文档
        /// </summary>
        private static XmlDocument _xmlDoc = new XmlDocument();



        /// <summary>
        /// 获取对应XML节点的值
        /// </summary>
        /// <param name="xml">Xml文件路径</param>
        /// <param name="nodename">节点名</param>
        /// <returns></returns>
        public static string GetXmlNodeValue(string xmlPaht, string nodeName)
        {
            string NodeValue = string.Empty;
            try
            {
                _xmlDoc.Load(xmlPaht);
                XmlNodeList items = _xmlDoc.DocumentElement.ChildNodes;

                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i].Name == nodeName)
                    {
                        NodeValue = items[i].InnerText;
                        break;
                    }
                }
            }
            catch
            {
                MessageBox.Show("XML文件格式错误");
            }            
            return NodeValue;
        }

        /// <summary>
        /// 更新站点编号
        /// </summary>
        /// <param name="xmlPath">文件路径</param>
        /// <param name="nodeName">节点名字</param>
        /// <param name="nodeValue">节点值</param>
        /// <returns></returns>
        public static void UpdateXMLNode(string xmlPath, string nodeName, string nodeValue)
        {
            try
            {
                _xmlDoc.Load(xmlPath);
                XmlNodeList items = _xmlDoc.DocumentElement.ChildNodes;
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i].Name == nodeName)
                    {
                        items[i].InnerText = nodeValue;
                        break;
                    }
                }
                if (System.IO.File.Exists(xmlPath))
                {
                    _xmlDoc.Save(xmlPath);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }


        /// <summary>
        ///	  更新报表模版编号
        /// </summary>
        /// <param name="id">模版编号</param>
        /// <param name="nvc"></param>
        public static void UpdateModelConfig(string id, string fileName, NameValueCollection nvc)
        {
            XmlNode node = GetNode(id, fileName);
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                string name = node.ChildNodes[i].Attributes["name"].Value;
                node.ChildNodes[i].InnerText = nvc[name];
            }
            if (System.IO.File.Exists(fileName))
            {
                _xmlDoc.Save(fileName);
            }
            
        }

        /// <summary>
        ///	 得到模块的节点
        /// </summary>
        /// <param name="id">桌面模块的ID</param>
        /// <returns>XmlNode</returns>
        private static XmlNode GetNode(string id, string fileName)
        {
            NameValueCollection nvc = new NameValueCollection();
            if (System.IO.File.Exists(Application.StartupPath+ @"\ReportModel.xml"))
            {
                _xmlDoc.Load(Application.StartupPath + @"\ReportModel.xml");
            }
            else
            {
                throw new Exception("配置文件不存在！");
            }
            XmlNodeList list = _xmlDoc.SelectNodes("root");
            XmlNode xmlNode = null;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Attributes["id"].Value.Equals(id))
                {
                    xmlNode = list[i];
                }
            }
            return xmlNode;

        }
    }
