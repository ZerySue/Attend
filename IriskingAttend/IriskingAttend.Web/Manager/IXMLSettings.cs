/*************************************************************************
** 文件名:   IXMLSettings.cs
×× 主要类:   IXMLSettings
**  
** Copyright (c) 中科虹霸有限责任公司
** 创建人:   fjf
** 日  期:   2012-04-24
** 修改人:   
** 日  期:
** 描  述:   管理XML配置接口
** 功  能:   XML配置文件管理的接口
** 版  本:   1.0.0
** 备  注:   命名及代码编写遵守C#编码规范
**
 * ***********************************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace Irisking.Manager
{
    public interface IXMLSettings    
    { 
        /// <summary>
        /// 显示字符串
        /// </summary>
        /// <returns></returns>
        string ToString(); 
        /// <summary>
        /// 是否加密（暂不使用）
        /// </summary>
        /// <param name="bEncrypt"></param>
        /// <returns></returns>
        bool EncryptFile(bool bEncrypt); 
        
        /// <summary>
        /// 读取XML文件
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="Key"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        string ReadXMLString(string Parent, string Key, string Default);
        /// <summary>
        /// 读取XML文件并返回Int16
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="Key"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        Int16 ReadXMLint16 (string Parent, string Key, Int16 Default);
        /// <summary>
        /// 读取XML文件并返回Int32
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="Key"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        Int32 ReadXMLint32(string Parent, string Key, Int32 Default);
        /// <summary>
        /// 读取XML文件并返回Int64
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="Key"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        Int64 ReadXMLint64(string Parent, string Key, Int64 Default);
        /// <summary>
        /// 读取XML文件并返回float
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="Key"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        float ReadXMLfloat(string Parent, string Key, float Default);

        /// <summary>
        /// 读取XML文件并返回decimal
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="Key"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        decimal ReadXMLdecimal(string Parent, string Key, decimal Default);
        /// <summary>
        /// 读取XML文件并返回bool
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="Key"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        bool ReadXMLbool(string Parent, string Key, bool Default);
        /// <summary>
        /// 读取XML文件并返回byte
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="Key"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        byte ReadXMLbyte (string Parent, string Key, byte Default);
        /// <summary>
        /// 读取XML文件并返回UInt16
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="Key"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        UInt16 ReadXMLuint16(string Parent, string Key, UInt16 Default);
        /// <summary>
        /// 读取XML文件并返回UInt32
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="Key"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        UInt32 ReadXMLuint32(string Parent, string Key, System.Globalization.NumberStyles format, UInt32 Default);

        /// <summary>
        /// 读取XML文件并返回UInt64
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="Key"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        UInt64 ReadXMLuint64(string Parent, string Key, UInt64 Default); 
        /// <summary>
        /// 写入字符串
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        bool WriteXMLString (string Parent, string Key, string Value);
        /// <summary>
        /// 写入Int16数据
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        bool WriteXMLValue (string Parent, string Key, Int16 Value);
        /// <summary>
        /// 写入Int32数据
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        bool WriteXMLValue (string Parent, string Key, Int32 Value);
        /// <summary>
        /// 写入Int64数据
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        bool WriteXMLValue (string Parent, string Key, Int64 Value);

        /// <summary>
        /// 写入float数据
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        bool WriteXMLValue (string Parent, string Key, float Value);
        /// <summary>
        /// 写入decimal数据
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        bool WriteXMLValue(string Parent, string Key, decimal Value);
        /// <summary>
        /// 写入bool值
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        bool WriteXMLValue (string Parent, string Key, bool Value);
        /// <summary>
        /// 写入byte值
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        bool WriteXMLValue (string Parent, string Key, byte Value);
        /// <summary>
        /// 写入UInt16数据
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        bool WriteXMLValue (string Parent, string Key, UInt16 Value);

        /// <summary>
        /// 写入UInt32数据
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        bool WriteXMLValue (string Parent, string Key, UInt32 Value);
        /// <summary>
        /// 写入UInt64数据
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        bool WriteXMLValue (string Parent, string Key, UInt64 Value);

    }
}
