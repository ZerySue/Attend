/*************************************************************************
** 文件名:   DomainServiceIriskingAttend_System.cs
** 主要类:   DomainServiceIriskingAttend
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2013-6-14
** 修改人:   
** 日  期:
** 描  述:   DomainServiceIriskingAttend类,后台操作数据库
**
** 版  本:   1.0.0
** 备  注:  命名及代码编写遵守C#编码规范
**
*************************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.ServiceModel.DomainServices.Hosting;
using System.ServiceModel.DomainServices.Server;
using Irisking.Web.DataModel;
using System.Data;
using Irisking.DataBaseAccess;
using System.Security.Cryptography;
using ServerCommunicationLib;
using IriskingAttend.Web.Manager;
using System.IO; 

using System.Threading;
using System.Windows.Forms;
using System.Web;

namespace IriskingAttend.Web
{
    // TODO: 创建包含应用程序逻辑的方法。    
    public partial class DomainServiceIriskingAttend : DomainService
    {
        #region 设备管理
        /// <summary>
        /// 增加设备
        /// </summary>
        /// <param name="device">增加的设备信息</param>
        /// <returns>0X01：添加成功且通知后台成功 0X00：添加失败 0XFF:添加成功,通知后台失败</returns>
        [Invoke]
        public byte AddDevice(string deviceSn,string place,string[] startTimes,int[] devTypes)
        {
            byte res = 0X00;
            for (int i = 0; i < startTimes.Length; i++)
            {
                string insertSQL = string.Format(@" INSERT INTO iris_device(dev_sn, dev_type, place, start_time)
                        VALUES ('{0}',{1},'{2}','{3}')",
                    deviceSn, devTypes[i], place, startTimes[i]);
                if (DbAccess.POSTGRESQL.Insert(insertSQL))  //成功插入数据库
                {
                    if (ServerComLib.NoticeTableChange(strIP, Port, new string[] { "iris_device" }, 1) != 0)
                    {
                        res = 0XFF;
                    }
                    else //成功通知后台
                    {
                        res = 0X01;
                    }                    
                }
            }
            return res;
        }


      
        /// <summary>
        /// 修改设备
        /// </summary>
        /// <param name="device">修改的设备信息</param>
        /// <returns>0X01：修改成功且通知后台成功 0X00：修改失败 0XFF:修改成功,通知后台失败</returns>
        [Invoke]
        public byte ModifyDevice(string deviceSn, string place, string[] startTimes, int[] devTypes)
        {
            byte res = 0X00;
            
            //先删除该设备所有的信息，再往数据库添加修改后的信息
            if (!DbAccess.POSTGRESQL.Delete(string.Format("DELETE FROM iris_device WHERE dev_sn = '{0}'", deviceSn)))            
            {
                return res;
            }

            for (int i = 0; i < startTimes.Length; i++)
            {                
                string insertSQL = string.Format(@" INSERT INTO iris_device(dev_sn, dev_type, place, start_time)
                        VALUES ('{0}',{1},'{2}','{3}')",
                   deviceSn, devTypes[i], place, startTimes[i]);

                DbAccess.POSTGRESQL.Update(insertSQL);                             
            }
           
            if (ServerComLib.NoticeTableChange(strIP, Port, new string[] { "iris_device" }, 1) != 0)
            {
                res = 0XFF;
            }
            else
            {
                res = 0X01;
            }
           
            return res;
        }
        #region 阳煤集团调用设备增加、修改功能 szr

        /// <summary>
        ///  修改设备阳煤集团调用该方法 by szr
        /// </summary>
        /// <param name="device">修改的设备信息</param>
        /// <returns>0X01：修改成功且通知后台成功 0X00：修改失败 0XFF:修改成功,通知后台失败</returns>
        [Invoke]
        public byte ModifyDeviceYangMei(string deviceSn, string place,string[] startTimes, int[] devTypes,string devFunction)
        {
            byte res = 0X00;

            //先删除该设备所有的信息，再往数据库添加修改后的信息
            if (!DbAccess.POSTGRESQL.Delete(string.Format("DELETE FROM iris_device WHERE dev_sn = '{0}'", deviceSn)))
            {
                return res;
            }

            for (int i = 0; i < startTimes.Length; i++)
            {
                string insertSQL = string.Format(@" INSERT INTO iris_device(dev_sn, dev_type, place, start_time,dev_function)
                        VALUES ('{0}',{1},'{2}','{3}','{4}')",
                   deviceSn, devTypes[i], place, startTimes[i], devFunction);

                DbAccess.POSTGRESQL.Update(insertSQL);
            }

            if (ServerComLib.NoticeTableChange(strIP, Port, new string[] { "iris_device" }, 1) != 0)
            {
                res = 0XFF;
            }
            else
            {
                res = 0X01;
            }

            return res;
        }

        /// <summary>
        /// 增加设备
        /// </summary>
        /// <param name="device">增加的设备信息</param>
        /// <returns>0X01：添加成功且通知后台成功 0X00：添加失败 0XFF:添加成功,通知后台失败</returns>
        [Invoke]
        public byte AddDeviceYangMei(string deviceSn, string place, string[] startTimes, int[] devTypes,string devFunction)
        {
            byte res = 0X00;
            for (int i = 0; i < startTimes.Length; i++)
            {
                string insertSQL = string.Format(@" INSERT INTO iris_device(dev_sn, dev_type, place, start_time,dev_function)
                        VALUES ('{0}',{1},'{2}','{3}','{4}')",
                    deviceSn, devTypes[i], place, startTimes[i], devFunction);
                if (DbAccess.POSTGRESQL.Insert(insertSQL))  //成功插入数据库
                {
                    if (ServerComLib.NoticeTableChange(strIP, Port, new string[] { "iris_device" }, 1) != 0)
                    {
                        res = 0XFF;
                    }
                    else //成功通知后台
                    {
                        res = 0X01;
                    }  
                }
            }
            return res;
        }
       
        #endregion

        [Invoke]
        public byte BatchModifyDevice(DeviceInfo[] devInfos, string[] startTimes, int[] devTypes)
        {
            byte res = 0X00;
            bool bInform = false;

            foreach (DeviceInfo devItem in devInfos)
            {
                //先删除该设备所有的信息，再往数据库添加修改后的信息
                if (DbAccess.POSTGRESQL.Delete(string.Format("DELETE FROM iris_device WHERE dev_sn = '{0}'", devItem.dev_sn)))
                {
                    bInform = true;
                }
                else
                {
                    continue;
                }

                for (int i = 0; i < startTimes.Length; i++)
                {
                    string insertSQL = string.Format(@" INSERT INTO iris_device(dev_sn, dev_type, place, start_time)
                        VALUES ('{0}',{1},'{2}','{3}')",
                       devItem.dev_sn, devTypes[i], devItem.place, startTimes[i]);

                    DbAccess.POSTGRESQL.Update(insertSQL);    
                }
            }

            if (bInform)
            {
                if (ServerComLib.NoticeTableChange(strIP, Port, new string[] { "iris_device" }, 1) != 0)
                {
                    res = 0XFF;
                }
                else
                {
                    res = 0X01;
                }
            }                   

            return res;
        }

        /// <summary>
        /// 获取所有设备
        /// </summary>
        /// <returns>获取的设备列表</returns>
        public IEnumerable<DeviceInfo> GetAllDevice()
        {
            string querySQL = @"select *, row_number() over() as row from iris_device order by convert_to(place,  E'GBK'),convert_to(dev_sn,  E'GBK'), row";
            List<DeviceInfo> devList = new List<DeviceInfo>();
            DataTable dt = DbAccess.POSTGRESQL.Select(querySQL, "iris_device");
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }

            try
            {

                if (dt.Columns.Count > 5)//通过列数判断设备表是否增加设备类型字段 也可以通过查询列名判断是否含有设备类型字段dev_function szr
                {

                    foreach (DataRow ar in dt.Rows)
                    {
                        DeviceInfo devInfo = new DeviceInfo();
                        devInfo.dev_sn = ar["dev_sn"].ToString();
                        devInfo.place = ar["place"].ToString();
                        devInfo.start_time = ar["start_time"].ToString();
                        devInfo.dev_function = ar["dev_function"].ToString();
                        if (ar["dev_type"] != DBNull.Value)
                        {
                            devInfo.dev_type = Int32.Parse(ar["dev_type"].ToString());
                        }
                        devList.Add(devInfo);
                    }
                }
                else
                {
                    foreach (DataRow ar in dt.Rows)
                    {
                        DeviceInfo devInfo = new DeviceInfo();
                        devInfo.dev_sn = ar["dev_sn"].ToString();
                        devInfo.place = ar["place"].ToString();
                        devInfo.start_time = ar["start_time"].ToString();
                        if (ar["dev_type"] != DBNull.Value)
                        {
                            devInfo.dev_type = Int32.Parse(ar["dev_type"].ToString());
                        }
                        devList.Add(devInfo);
                    }
                }


                return devList;
            }
            catch (Exception e)
            {
                MessageBox.Show( e.ToString() );
                return null;
            }            
        }

        /// <summary>
        ///  不能删除，否则无法更新
        /// </summary>
        /// <param name="o"></param>
        [Update]
        public void TestEidtor(DeviceInfo o)
        {
        }

        
        /// <summary>
        /// 设备批量删除
        /// </summary>
        /// <param name="deviceIds">删除的设备列表ID集合</param>
        /// <returns>true：删除成功 false：删除失败</returns>
        [Invoke]
        public byte BatchDeleteDevice(string[] deviceIds)
        {
            string deleteSql = "DELETE FROM iris_device WHERE dev_sn in ( \'";

            deleteSql += deviceIds[0];
            for (int index = 1; index < deviceIds.Length; index++)
            {
                deleteSql += ("\',\'" + deviceIds[index]);
            }

            deleteSql += "\')";

            if (DbAccess.POSTGRESQL.Delete(deleteSql))
            {
                if (ServerComLib.NoticeTableChange(strIP, Port, new string[] { "iris_device" }, 1) != 0)
                {
                    return 0XFF;
                }
                return 0X01;                    
            }
            return 0X00;           
        }

        /// <summary>
        /// 查询是否存在此设备
        /// </summary>        
        /// <param name="devSn">查询的设备的sn</param>
        /// <returns>true：存在 false：不存在</returns>
        [Invoke]
        public bool IsDeviceExist(string devSn)
        {
            string querySQL = string.Format(@"select * from iris_device where dev_sn = '{0}'", devSn );
            DataTable dt = DbAccess.POSTGRESQL.Select(querySQL, "iris_device");
            if (null == dt || dt.Rows.Count < 1)
            {
                return false;
            }
            return true;
        }
        #endregion

        #region 操作员密码及MD5码的计算

        /// <summary>
        /// 对比旧密码的MD5与输入的旧密码MD5是否相符，若相符，则将旧密码修改为新密码
        /// </summary>
        /// <param name="oldPwdMD5">原密码MD5</param>
        /// <param name="inputPwd">输入的旧密码</param>
        /// <param name="userName">用户名</param>
        /// <param name="newPwd">新密码</param>
        /// <returns>
        /// 0X00：旧密码的MD5与输入的旧密码MD5不相符
        /// 0X01：将旧密码修改为新密码修改成功
        /// 0XFF：将旧密码修改为新密码修改失败
        /// </returns>
        [Invoke]
        public byte CompOldMD5AndModifyPwd(string oldPwdMD5, string inputPwd, string userName, string newPwd)
        {
            string inputPwdMD5 = GetMD5String(inputPwd);
            if (!oldPwdMD5.Equals(inputPwdMD5))
            {
                return 0X00;
            }
            return ModifyPassword(userName, newPwd);
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="newPwd">新密码</param>
        /// <returns>
        /// 0X01：将旧密码修改为新密码修改成功
        /// 0XFF：将旧密码修改为新密码修改失败
        /// </returns>
        [Invoke]
        public byte ModifyPassword( string userName, string newPwd)
        {
            string updateSQL = string.Format(@" UPDATE operator_info SET password='{0}' WHERE logname = '{1}'",
            GetMD5String(newPwd), userName);

            if (DbAccess.POSTGRESQL.Update(updateSQL))
            {
                return 0X01;
            }
            return 0XFF;
        }

         /// <summary>
         /// 获取密码
         /// </summary>
         /// <param name="userName">用户名</param>
         /// <returns>获取到的密码</returns>
        [Invoke]
        public string GetPassword(string userName )
        {
            string querySQL = string.Format(@" select password from operator_info WHERE logname = '{0}'", userName);            
            DataTable dt = DbAccess.POSTGRESQL.Select(querySQL, "operator_info");
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }            
            return dt.Rows[0]["password"].ToString();
         }

        /// <summary>
        /// 密码是否正确
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>-2：不存在此用户  -1：密码错误  其它：密码正确，返回用户名ID</returns>
        [Invoke]
        public int IsPasswordOk(string userName, string password)
        {
            string querySQL = string.Format(@" select operator_id, password from operator_info WHERE logname = '{0}'", userName);
            DataTable dt = DbAccess.POSTGRESQL.Select(querySQL, "operator_info");
            if (null == dt || dt.Rows.Count < 1)
            {
                return -2;
            }
            string md5 = GetMD5String(password);
            if (dt.Rows[0]["password"].ToString() == md5)
            {
                try
                {
                    return Int32.Parse(dt.Rows[0]["operator_id"].ToString());
                }
                catch
                {
                    return -2;
                }
            }
            return -1;
        }

        /// <summary>
        /// 获取字符串的MD5码
        /// </summary>
        /// <param name="inputStr">输入的字符串</param>
        /// <returns>字符串的MD5码</returns>
        public string GetMD5String( string inputStr )
        {
            byte[] result = System.Text.Encoding.ASCII.GetBytes(inputStr.Trim());    
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(result);

            //将获得的MD5码用‘ ’代替‘-’并转换为小写
            string convertMd5 = BitConverter.ToString(output).Replace("-", "");
            convertMd5 = convertMd5.ToLower();
            return convertMd5;
        }

        #endregion

        #region 操作员管理
        /// <summary>
        /// 获取所有操作员信息列表
        /// </summary>
        /// <returns>所有操作员信息列表</returns>
        public IEnumerable<operator_info> GetAllOperator()
        {
            string querySQL = @"select * from operator_info";
            List<operator_info> opList = new List<operator_info>();
            DataTable dt = DbAccess.POSTGRESQL.Select(querySQL, "operator_info");
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }

            try
            {
                foreach (DataRow ar in dt.Rows)
                {
                    operator_info opInfo = new operator_info();                    
                    opInfo.operator_id = Int32.Parse(ar["operator_id"].ToString());
                    opInfo.logname = ar["logname"].ToString();
                    opInfo.realityname = ar["realityname"].ToString();
                    opInfo.password = ar["password"].ToString();

                    if (ar["operator_type"] != DBNull.Value)
                    {
                        opInfo.operator_type = Int32.Parse(ar["operator_type"].ToString());                       
                    }
                    opList.Add(opInfo);
                }

                return opList;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                return null;
            }          
            
        }

        /// <summary>
        /// 修改操作员信息
        /// </summary>
        /// <param name="oldLogName">操作员原始的登录名</param>
        /// <param name="opInfo">修改后的操作员信息</param>
        /// <returns>true：修改成功 false：修改失败</returns>
        [Invoke]
        public bool ModifyOperatorInfo(string oldLogName, operator_info opInfo)
        {
            string updateSQL = string.Format(@" UPDATE operator_info SET logname='{0}', realityname='{1}', operator_type={2} WHERE logname = '{3}'",
             opInfo.logname, opInfo.realityname, opInfo.operator_type, oldLogName);

            if (DbAccess.POSTGRESQL.Update(updateSQL))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 修改操作员密码
        /// </summary>
        /// <param name="logName">操作员的登录名</param>
        /// <param name="inputPwd">修改后的操作员密码</param>
        /// <returns>true：修改成功 false：修改失败</returns>
        [Invoke]
        public bool ModifyOperatorPassword(string logName, string inputPwd)
        {
            string inputPwdMD5 = GetMD5String(inputPwd);
           
            string updateSQL = string.Format(@" UPDATE operator_info SET password='{0}' WHERE logname = '{1}'",
            inputPwdMD5, logName);

            if (DbAccess.POSTGRESQL.Update(updateSQL))
            {
                return true;
            }            
            return false;
        }

        /// <summary>
        /// 删除操作员信息
        /// </summary>
        /// <param name="logName">操作员的登录名</param>        
        /// <returns>true：删除成功 false：删除失败</returns>
        [Invoke]
        public bool DeleteOperator(string logName)
        {
            string deleteSQL = string.Format(@" DELETE FROM operator_info WHERE logname = '{0}'", logName);

            if (DbAccess.POSTGRESQL.Delete(deleteSQL))
            {
                return true;
            }
            return false;
        }
   
         /// <summary>
         /// 增加操作员
         /// </summary>
         /// <param name="opInfo">增加的操作员信息</param>
        /// <returns>true：删除成功 false：删除失败</returns>
        [Invoke]
        public bool AddOperator(operator_info opInfo)
        {
            opInfo.password = GetMD5String(opInfo.password);
            string insertSQL = string.Format(@" INSERT INTO operator_info(logname,realityname, password, operator_type)VALUES ('{0}','{1}','{2}',{3})",
            opInfo.logname, opInfo.realityname, opInfo.password, opInfo.operator_type );

            if (DbAccess.POSTGRESQL.Insert(insertSQL))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 查询是否存在此操作员
        /// </summary>
        /// <param name="logName">操作员登录名</param>
        /// <returns>true：存在 false：不存在</returns>
        [Invoke]
        public bool IsOperatorExist( string logName)
        {
            string querySQL = string.Format(@"select * from operator_info where logname = '{0}'", logName);
            DataTable dt = DbAccess.POSTGRESQL.Select(querySQL, "operator_info");
            if (null == dt || dt.Rows.Count < 1)
            {
                return false;
            }           
            return true;
        }
        #endregion

        #region 系统参数管理
        /// <summary>
        /// 获取所有系统参数 修改 by cty time:2013-9-9
        /// </summary>
        /// <returns>系统参数</returns>
        public IEnumerable<system_param> GetSystemParam()
        {
            string querySQL = @"select * from system_param";
            List<system_param> systemParamList = new List<system_param>();
            DataTable dt = DbAccess.POSTGRESQL.Select(querySQL, "system_param");
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }

            try
            {
                system_param param = new system_param();
                param.dup_time = Int16.Parse(dt.Rows[0]["dup_time"].ToString());
                param.in_dup_recog = Int16.Parse(dt.Rows[0]["in_dup_recog"].ToString());
                param.out_dup_recog = Int16.Parse(dt.Rows[0]["out_dup_recog"].ToString());
                param.over_time = Int16.Parse(dt.Rows[0]["over_time"].ToString()); // by cty

                systemParamList.Add(param);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                return null;
            }
            return systemParamList;
        }

        /// <summary>
        /// 修改系统参数 修改 by cty time:2013-9-9
        /// </summary>
        /// <param name="dup_time">重复识别间隔</param>
        /// <param name="in_dup_recog">上班重复判别</param>
        /// <param name="out_dup_recog">下班重复判别</param>
        /// <returns>0X01：修改成功且通知后台成功 0X00：修改失败 0XFF:修改成功,通知后台失败</returns>
        [Invoke]
        public byte ModifySystemParam(int over_time, Int16 dup_time, Int16 in_dup_recog, Int16 out_dup_recog)
        {
            string updateSQL = string.Format(@" UPDATE system_param SET dup_time={0}, in_dup_recog={1}, out_dup_recog={2},over_time={3}",
            dup_time, in_dup_recog, out_dup_recog, over_time);

            if (DbAccess.POSTGRESQL.Update(updateSQL))
            {
                if (ServerComLib.NoticeTableChange(strIP, Port, new string[] { "system_param" }, 1) != 0)
                {
                    return 0XFF;
                }
                return 0X01;
            }
            return 0X00;
        }        

        /// <summary>
        /// 获取记工策略
        /// </summary>
        /// <returns>记工策略</returns>
        public IEnumerable<work_cnt_policy> GetWorkCntPolicy()
        {
            string querySQL = @"select * from work_cnt_policy";
            List<work_cnt_policy> policyList = new List<work_cnt_policy>();
            DataTable dt = DbAccess.POSTGRESQL.Select(querySQL, "work_cnt_policy");
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }

            try
            {
                work_cnt_policy policy = new work_cnt_policy();
                policy.lt = Int32.Parse(dt.Rows[0]["lt"].ToString());
                policy.gt = Int32.Parse(dt.Rows[0]["gt"].ToString());
                policy.accuracy = Int32.Parse(dt.Rows[0]["accuracy"].ToString());
                policyList.Add(policy);
            }           
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                return null;
            }
            return policyList;
        }

        /// <summary>
        /// 修改记工策略
        /// </summary>
        /// <param name="lt">当实际工数小于【班次】中规定工数时，工数取值</param>
        /// <param name="gt">当实际工数大于【班次】中规定工数时，工数取值</param>
        /// <param name="accuracy">当工数存在小数部分时，对小数部分的处理意见</param>
        /// <returns>0X01：修改成功且通知后台成功 0X00：修改失败 0XFF:修改成功,通知后台失败</returns>
        [Invoke]
        public byte ModifyWorkCntPolicy(Int32 lt, Int32 gt, Int32 accuracy)
        {
            string updateSQL = string.Format(@" UPDATE work_cnt_policy SET lt={0}, gt={1}, accuracy={2}",
            lt, gt, accuracy);

            if (DbAccess.POSTGRESQL.Update(updateSQL))
            {
                if (ServerComLib.NoticeTableChange(strIP, Port, new string[] { "work_cnt_policy" }, 1) != 0)
                {
                    return 0XFF;
                }
                return 0X01;
            }
            return 0X00;
        }

        /// <summary>
        /// 获取数据库备份参数
        /// </summary>
        /// <returns>数据库备份参数</returns>
        public IEnumerable<backup_param> GetDBBackupParam()
        {
            string querySQL = @"select * from backup_param";
            List<backup_param> backupParamList = new List<backup_param>();
            DataTable dt = DbAccess.POSTGRESQL.Select(querySQL, "system_param");
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }

            try
            {
                backup_param backupparam = new backup_param();
                backupparam.backup_destination = dt.Rows[0]["backup_destination"].ToString();
                backupparam.concrete_time = dt.Rows[0]["concrete_time"].ToString();
                backupparam.period = Int32.Parse(dt.Rows[0]["period"].ToString());
                backupparam.sub_period = Int32.Parse(dt.Rows[0]["sub_period"].ToString());
                backupParamList.Add(backupparam);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                return null;
            }
            return backupParamList;
        }

        /// <summary>
        /// 修改自动备份数据库参数
        /// </summary>
        /// <param name="period">备份周期</param>
        /// <param name="sub_period">备份日期</param>
        /// <param name="concrete_time">具体时间</param>
        /// <param name="backup_destination">备份输出地址</param>
        /// <returns>0X01：修改成功且通知后台成功 0X00：修改失败 0XFF:修改成功,通知后台失败</returns>
        [Invoke]
        public byte ModifyAutoDBBackup(Int32 period, Int32 sub_period, string concrete_time, string backup_destination)
        {            
            string updateSQL = string.Format(@" UPDATE backup_param SET period={0}, sub_period={1}, concrete_time='{2}', backup_destination='{3}'",
            period, sub_period, concrete_time, backup_destination);

            //PostgreSQL 8.4时需要这句话
            //updateSQL = updateSQL.Replace("\\", "\\\\");           
            if (DbAccess.POSTGRESQL.Update(updateSQL))
            { 
                if (ServerComLib.NoticeTableChange(strIP, Port, new string[] { "backup_param" }, 1) != 0)
                {
                    return 0XFF;
                }
                return 0X01;
            }
            return 0X00;
        }
        
        /// <summary>
        /// 启动手动备份
        /// </summary>
        /// <param name="backupDestination">备份输出地址</param>
        /// <returns>
        /// -2：备份输出路径不合法
        /// 1： 备份成功
        /// 3： 有另一进程正在进行数据库备份
        /// 其它：缺少数据库备份工具
        /// </returns>
        [Invoke]
        public int StartMannualDBBackup( string backupDestination)
        {
            if (false == File.Exists(backupDestination))
            {
                try
                {
                    FileStream backup = File.Create(backupDestination);
                    backup.Close(); 
                }
                catch (Exception e)
                {
                    string str = e.ToString();
                    return -2;
                }  
            }  
            
            DbManager mgr = new DbManager();
            int ret = mgr.BackupDBManager(backupDestination, false);

            if (ret != 1)
            {
                return ret;
            }

            while (ProcessControl.FindActiveServices("pg_dump") == true)
            {
                Thread.Sleep(1000);
            }
            return ret;
        }

        /// <summary>
        /// 关闭数据库
        /// </summary>
        /// <returns>关闭是否成功</returns>
        [Invoke]
        public bool CloseDataBase()
        {
            return DbAccess.ClosePsqlDb();            
        }
        #endregion

        #region 权限管理
        /// <summary>
        /// 获取指定操作员的权限
        /// </summary>
        /// <returns>指定操作员的权限</returns>
        public IEnumerable<operator_purview> GetOperatorPurview(string operatorName)
        {
            try
            {
                //读取操作员ID
                string querySQL = string.Format(@"select operator_id from operator_info where logname ='{0}' ", operatorName);
                DataTable dt = DbAccess.POSTGRESQL.Select(querySQL, "operator_info");
                if (null == dt || dt.Rows.Count < 1)
                {
                    return null;
                }

                int operatorID = Int32.Parse(dt.Rows[0]["operator_id"].ToString());

                querySQL = string.Format(@"select * from operator_purview where operator_id = {0}", operatorID);

                dt = DbAccess.POSTGRESQL.Select(querySQL, "operator_purview");
                if (null == dt || dt.Rows.Count < 1)
                {
                    return null;
                }

                List<operator_purview> operatorPurviewList = new List<operator_purview>();                
                foreach (DataRow ar in dt.Rows)
                {
                    operator_purview operatorPurview = new operator_purview();
                    operatorPurview.oper_purv_id = Int16.Parse(ar["oper_purv_id"].ToString());
                    operatorPurview.operator_id = Int16.Parse(ar["operator_id"].ToString());
                    operatorPurview.purview_id = Int16.Parse(ar["purview_id"].ToString());                    
                    operatorPurviewList.Add(operatorPurview);                    
                }

                return operatorPurviewList;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                return null;
            }
        }

        /// <summary>
        /// 获取指定操作员的部门权力
        /// </summary>
        /// <returns>指定操作员的权限</returns>
        public IEnumerable<operator_potence> GetOperatorDepartPotence(string operatorName)
        {
            try
            {
                //读取操作员ID
                string querySQL = string.Format(@"select operator_id from operator_info where logname ='{0}' ", operatorName);
                DataTable dt = DbAccess.POSTGRESQL.Select(querySQL, "operator_info");
                if (null == dt || dt.Rows.Count < 1)
                {
                    return null;
                }

                int operatorID = Int32.Parse(dt.Rows[0]["operator_id"].ToString());

                querySQL = string.Format(@"select * from operator_potence where operator_id = {0}", operatorID);

                dt = DbAccess.POSTGRESQL.Select(querySQL, "operator_potence");
                if (null == dt || dt.Rows.Count < 1)
                {
                    return null;
                }

                List<operator_potence> operatorPotenceList = new List<operator_potence>();
                foreach (DataRow ar in dt.Rows)
                {
                    operator_potence operatorPotence = new operator_potence();
                    operatorPotence.operator_potence_id = Int16.Parse(ar["operator_potence_id"].ToString());
                    operatorPotence.operator_id = Int16.Parse(ar["operator_id"].ToString());
                    operatorPotence.depart_id = Int16.Parse(ar["depart_id"].ToString());
                    operatorPotenceList.Add(operatorPotence);
                }

                return operatorPotenceList;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                return null;
            }
        }

        /// <summary>
        /// 获取权限表
        /// </summary>
        /// <returns>权限列表</returns>
        public IEnumerable<purview> GetPurview()
        {
            string querySQL = @"select * from purview order by purview_id asc";
            List<purview> purviewList = new List<purview>();
            DataTable dt = DbAccess.POSTGRESQL.Select(querySQL, "purview");
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }

            try
            {
                foreach (DataRow ar in dt.Rows)
                {
                    purview pur = new purview();
                    pur.purview_id = Int16.Parse(ar["purview_id"].ToString());
                    if (ar["parent_purview_id"] != null && ar["parent_purview_id"].ToString() != "")
                    {
                        pur.parent_purview_id = Int16.Parse(ar["parent_purview_id"].ToString());
                    }
                    else
                    {
                        pur.parent_purview_id = -1;
                    }
                    pur.purview_name = ar["purview_name"].ToString();
                    purviewList.Add(pur);

                    //删除此字段
                    /*if (ar["new_attend"] != null && ar["new_attend"].ToString() != "")
                    {
                        purviewList.Add(pur);
                    }*/
                }

                return purviewList;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                return null;
            }            
        }

        /// <summary>
        /// 设置操作员的部门权力与权限列表
        /// </summary>
        /// <param name="operatorName">操作员名称</param>
        /// <param name="departIDList">部门权力列表</param>
        /// <param name="purviewIDList">权限ID列表</param>
        /// <returns>设置是否成功</returns>
        public bool SetOperDepartPotenceAndPurvew(string operatorName, List<int> departIDList, List<int> purviewIDList)
        {
            try
            {
                //读取操作员ID
                string querySQL = string.Format(@"select operator_id from operator_info where logname ='{0}' ", operatorName);
                DataTable dt = DbAccess.POSTGRESQL.Select(querySQL, "operator_info");
                if (null == dt || dt.Rows.Count < 1)
                {
                    return false;
                }

                int operatorID = Int32.Parse(dt.Rows[0]["operator_id"].ToString());

                //删除部门权力表中此操作员的部门权力
                string deleteSQL = string.Format(@" DELETE from operator_potence where operator_id = {0}", operatorID);
                if (!DbAccess.POSTGRESQL.Delete(deleteSQL))
                {
                    return false;
                }

                //读取部门权力表的最大ID
                querySQL = @"select max( operator_potence_id ) as maxID from operator_potence";
                dt = DbAccess.POSTGRESQL.Select(querySQL, "operator_potence");
                if (null == dt || dt.Rows.Count < 1)
                {
                    return false;
                }                
                
                int maxID = 0;
                if (dt.Rows[0]["maxID"] != null && dt.Rows[0]["maxID"].ToString() != "")
                {
                    maxID = Int32.Parse(dt.Rows[0]["maxID"].ToString());
                }

                //将部门权力表重新写入
                foreach (int departId in departIDList)
                {
                    string insertSQL = string.Format(@" INSERT INTO operator_potence(operator_potence_id, operator_id , depart_id)
                                                    VALUES ('{0}',{1},'{2}')", ++maxID, operatorID, departId);

                    if (!DbAccess.POSTGRESQL.Insert(insertSQL))
                    {
                        return false;
                    }
                }

                //删除权限表中此操作员的所有权限
                deleteSQL = string.Format(@" DELETE from operator_purview where operator_id = {0}", operatorID);
                if (!DbAccess.POSTGRESQL.Delete(deleteSQL))
                {
                    return false;
                }

                //读取权限表的最大ID
                querySQL = @"select max( oper_purv_id ) as maxID from operator_purview";
                dt = DbAccess.POSTGRESQL.Select(querySQL, "operator_purview");
                if (null == dt || dt.Rows.Count < 1)
                {
                    return false;
                }
                
                maxID = 0;
                if (dt.Rows[0]["maxID"] != null && dt.Rows[0]["maxID"].ToString() != "")
                {
                    maxID = Int32.Parse(dt.Rows[0]["maxID"].ToString());
                }

                //将权限表重新写入
                foreach (int purviewId in purviewIDList)
                {
                    string insertSQL = string.Format(@" INSERT INTO operator_purview(oper_purv_id,  operator_id, purview_id)
                                                    VALUES ('{0}',{1},'{2}')", ++maxID, operatorID, purviewId);

                    if (!DbAccess.POSTGRESQL.Insert(insertSQL))
                    {
                        return false;
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                return false;
            }    
        }

        /// <summary>
        /// 增加操作员部门权力
        /// </summary>
        /// <param name="operatorName">操作员姓名</param>
        /// <param name="departId">部门ID</param>
        /// <returns>增加是否成功</returns>
        public bool AddDepartPotence(string operatorName, int departId)
        {
            try
            {
                //读取操作员ID
                string querySQL = string.Format(@"select operator_id from operator_info where logname ='{0}' ", operatorName);
                DataTable dt = DbAccess.POSTGRESQL.Select(querySQL, "operator_info");
                if (null == dt || dt.Rows.Count < 1)
                {
                    return false;
                }

                int operatorID = Int32.Parse(dt.Rows[0]["operator_id"].ToString());

                //读取部门权力表的最大ID
                querySQL = @"select max( operator_potence_id ) as maxID from operator_potence";
                dt = DbAccess.POSTGRESQL.Select(querySQL, "operator_potence");
                if (null == dt || dt.Rows.Count < 1)
                {
                    return false;
                }

                int maxID = 0;
                if (dt.Rows[0]["maxID"] != null && dt.Rows[0]["maxID"].ToString() != "")
                {
                    maxID = Int32.Parse(dt.Rows[0]["maxID"].ToString());
                }
                
                string insertSQL = string.Format(@" INSERT INTO operator_potence(operator_potence_id, operator_id , depart_id)
                                                VALUES ('{0}',{1},'{2}')", ++maxID, operatorID, departId);

                if (!DbAccess.POSTGRESQL.Insert(insertSQL))
                {
                    return false;
                }               
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                return false;
            }
        }

        #endregion

        #region 操作员日志

        /// <summary>
        ///  不能删除，否则无法更新
        /// </summary>
        /// <param name="o"></param>
        [Update]
        public void UserOperationLogEidtor(UserOperationLog o)
        {
        }

        /// <summary>        
        /// 获取客户端的IP地址
        /// </summary>
        /// <returns></returns>
        [Invoke]
        public string GetClientIP()
        {    
             string uip = "";
             if (HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null)
             {
                 uip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
             }
             else
             {
                 uip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString();
             }
             if (uip == "::1")
             {
                 uip = "127.0.0.1";
             }
             return uip;   
        }

        /// <summary>
        /// 按最大记录数获取操作员日志
        /// </summary>
        /// <returns>获取的操作员日志列表</returns>
        public IEnumerable<UserOperationLog> GetUserOperationLogByRecordCount(int maxRecordCount)
        {
            string querySQL = @"select * from user_operation_log where delete_time is null order by operation_time desc";

            if (maxRecordCount >= 0)
            {
                querySQL += string.Format(" limit {0}", maxRecordCount);
            }

            return GetUserOperationLogByQuerySql(querySQL);
        }

        /// <summary>
        /// 按起始时间、终止时间及操作员查询获取操作员日志
        /// </summary>
        /// <returns>获取的操作员日志列表</returns>
        public IEnumerable<UserOperationLog> GetUserOperationLog( string startTime, string endTime, string logName )
        {
            string querySQL = @"select * from user_operation_log";

            string sqlWhere = " where delete_time is null";

            if (startTime != null && startTime != "")
            {
                sqlWhere += string.Format(" and operation_time >= '{0}:00:00:00'", startTime);
            }

            if (endTime != null && endTime != "")
            {
                sqlWhere += string.Format(" and operation_time <= '{0}:23:59:59'", endTime);
            }

            if (logName != null && logName != "")
            {
                sqlWhere += string.Format(" and user_name='{0}'", logName);
            }
            querySQL += sqlWhere;
            querySQL += " order by operation_time desc";

            return GetUserOperationLogByQuerySql(querySQL);
        }

        /// <summary>
        /// 根据参数条件获取操作员日志
        /// </summary>
        /// <returns>获取的操作员日志列表</returns>
        private IEnumerable<UserOperationLog> GetUserOperationLogByQuerySql(string querySQL)
        {            
            List<UserOperationLog> logList = new List<UserOperationLog>();
            DataTable dt = DbAccess.POSTGRESQL.Select(querySQL, "user_operation_log");
            if (null == dt || dt.Rows.Count < 1)
            {
                return null;
            }

            try
            {
                foreach (DataRow ar in dt.Rows)
                {
                    UserOperationLog log = new UserOperationLog();
                    log.user_operation_log_id = Int32.Parse(ar["user_operation_log_id"].ToString());
                    log.user_id = Int32.Parse(ar["user_id"].ToString());
                    log.user_name = ar["user_name"].ToString();
                    log.user_ip = ar["user_ip"].ToString();
                    log.operation_time = DateTime.Parse(ar["operation_time"].ToString());
                    log.content = ar["content"].ToString();
                    log.result = Int16.Parse(ar["result"].ToString());
                    log.resultStr = "失败";
                    if (log.result == 1)
                    {
                        log.resultStr = "成功";
                    }
                    log.description = ar["description"].ToString();

                    logList.Add(log);
                }

                return logList;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                return null;
            }
        }

        /// <summary>
        /// 增加操作员日志
        /// </summary>
        /// <param name="opInfo">增加的操作员日志信息</param>
        /// <returns>true：删除成功 false：删除失败</returns>
        [Invoke]
        public bool AddOperatorLog(UserOperationLog opLogInfo)
        {
            opLogInfo.user_ip = GetClientIP();
            string insertSQL = string.Format(@" INSERT INTO user_operation_log(user_id,user_name, user_ip, operation_time,result,content,description)VALUES ({0},'{1}','{2}','{3}',{4},'{5}','{6}')",
            opLogInfo.user_id, opLogInfo.user_name, opLogInfo.user_ip, opLogInfo.operation_time, opLogInfo.result,opLogInfo.content,opLogInfo.description);

            
            if (DbAccess.POSTGRESQL.Insert(insertSQL))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 添加五虎山项目中只有admin用户可见可操作的‘清除无用记录’的功能 by cty 2014-3-11
        /// </summary>
        /// <param name="opLogInfo"></param>
        /// <returns></returns>
        [Invoke]
        public bool DeleteOperatorLog(UserOperationLog opLogInfo)
        {
            DateTime dt = DateTime.Now;
            string deleteSQL = string.Format(@"UPDATE user_operation_log SET delete_time='{0}' WHERE user_operation_log_id={1}",dt,opLogInfo.user_operation_log_id);
            if (DbAccess.POSTGRESQL.Update(deleteSQL))
            {
                return true;
            }
            else 
            {
                return false;
            }
            
        }
        #endregion
    }
}


