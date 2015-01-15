/*************************************************************************
** �ļ���:   IXMLSettings.cs
���� ��Ҫ��:   IXMLSettings
**  
** Copyright (c) �пƺ���������ι�˾
** ������:   fjf
** ��  ��:   2012-04-24
** �޸���:   
** ��  ��:
** ��  ��:   ����XML���ýӿ�
** ��  ��:   XML�����ļ�����Ľӿ�
** ��  ��:   1.0.0
** ��  ע:   �����������д����C#����淶
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
        /// ��ʾ�ַ���
        /// </summary>
        /// <returns></returns>
        string ToString(); 
        /// <summary>
        /// �Ƿ���ܣ��ݲ�ʹ�ã�
        /// </summary>
        /// <param name="bEncrypt"></param>
        /// <returns></returns>
        bool EncryptFile(bool bEncrypt); 
        
        /// <summary>
        /// ��ȡXML�ļ�
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="Key"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        string ReadXMLString(string Parent, string Key, string Default);
        /// <summary>
        /// ��ȡXML�ļ�������Int16
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="Key"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        Int16 ReadXMLint16 (string Parent, string Key, Int16 Default);
        /// <summary>
        /// ��ȡXML�ļ�������Int32
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="Key"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        Int32 ReadXMLint32(string Parent, string Key, Int32 Default);
        /// <summary>
        /// ��ȡXML�ļ�������Int64
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="Key"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        Int64 ReadXMLint64(string Parent, string Key, Int64 Default);
        /// <summary>
        /// ��ȡXML�ļ�������float
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="Key"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        float ReadXMLfloat(string Parent, string Key, float Default);

        /// <summary>
        /// ��ȡXML�ļ�������decimal
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="Key"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        decimal ReadXMLdecimal(string Parent, string Key, decimal Default);
        /// <summary>
        /// ��ȡXML�ļ�������bool
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="Key"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        bool ReadXMLbool(string Parent, string Key, bool Default);
        /// <summary>
        /// ��ȡXML�ļ�������byte
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="Key"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        byte ReadXMLbyte (string Parent, string Key, byte Default);
        /// <summary>
        /// ��ȡXML�ļ�������UInt16
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="Key"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        UInt16 ReadXMLuint16(string Parent, string Key, UInt16 Default);
        /// <summary>
        /// ��ȡXML�ļ�������UInt32
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="Key"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        UInt32 ReadXMLuint32(string Parent, string Key, System.Globalization.NumberStyles format, UInt32 Default);

        /// <summary>
        /// ��ȡXML�ļ�������UInt64
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="Key"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        UInt64 ReadXMLuint64(string Parent, string Key, UInt64 Default); 
        /// <summary>
        /// д���ַ���
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        bool WriteXMLString (string Parent, string Key, string Value);
        /// <summary>
        /// д��Int16����
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        bool WriteXMLValue (string Parent, string Key, Int16 Value);
        /// <summary>
        /// д��Int32����
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        bool WriteXMLValue (string Parent, string Key, Int32 Value);
        /// <summary>
        /// д��Int64����
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        bool WriteXMLValue (string Parent, string Key, Int64 Value);

        /// <summary>
        /// д��float����
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        bool WriteXMLValue (string Parent, string Key, float Value);
        /// <summary>
        /// д��decimal����
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        bool WriteXMLValue(string Parent, string Key, decimal Value);
        /// <summary>
        /// д��boolֵ
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        bool WriteXMLValue (string Parent, string Key, bool Value);
        /// <summary>
        /// д��byteֵ
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        bool WriteXMLValue (string Parent, string Key, byte Value);
        /// <summary>
        /// д��UInt16����
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        bool WriteXMLValue (string Parent, string Key, UInt16 Value);

        /// <summary>
        /// д��UInt32����
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        bool WriteXMLValue (string Parent, string Key, UInt32 Value);
        /// <summary>
        /// д��UInt64����
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        bool WriteXMLValue (string Parent, string Key, UInt64 Value);

    }
}
