/*************************************************************************
** 文件名:   SLCookieOperate.cs
** 主要类:   SLCookieOperate
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   yuhaitao
** 日  期:   2014-8-19
** 修改人:   wz
** 日  期:   2014-8-19
** 描  述:   SLCookieOperate类,操作Cookies
**
** 版  本:   1.0.0
** 备  注:  命名及代码编写遵守C#编码规范
**
*************************************************************************/
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
using System.Windows.Browser;

namespace IriskingAttend.BehaviorSelf
{
    public class SLCookieOperate
    {
        /// <summary>
        /// 设置持久时间长的Cookie
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        public  static void SetCookie(string key, string value)

        {
            string oldCookie = HtmlPage.Document.GetProperty("cookie") as String;
            DateTime expiration = DateTime.UtcNow + TimeSpan.FromDays(2000);
            string cookie = String.Format("{0}={1};expires={2}", key, value, expiration.ToString("R"));
            HtmlPage.Document.SetProperty("cookie", cookie);
        }

        #region 读取一个已经存在的Cookie
        /// <summary>
        /// 读取一个已经存在的Cookie
        /// </summary>
        /// <param name="key">cookie key</param>
        /// <returns></returns>
        public static string GetCookie(string key)
        {
            string[] cookies = HtmlPage.Document.Cookies.Split(';');
            key += '=';
            foreach (string cookie in cookies)
            {
                string cookieStr = cookie.Trim();
                if (cookieStr.StartsWith(key, StringComparison.OrdinalIgnoreCase))
                {
                    string[] vals = cookieStr.Split('=');
                    if (vals.Length >= 2)
                    {
                        return vals[1];
                    }
                    return string.Empty;
                }
            }
            return null;

        }
        #endregion

        #region 删除特定的Cookie(清空它的Value值，过期值设置为-1天)
        /// <summary>
        /// 删除特定的Cookie(清空它的Value值，过期值设置为-1天)
        /// </summary>
        /// <param name="key"></param>
        public static void DeleteCookie(string key)
        {
            string oldCookie = HtmlPage.Document.GetProperty("cookie") as String;
            DateTime expiration = DateTime.UtcNow - TimeSpan.FromDays(1);
            string cookie = String.Format("{0}=;expires={1}", key, expiration.ToString("R"));
            HtmlPage.Document.SetProperty("cookie", cookie);
        }
        #endregion

        #region 判定指定的key-value对是否在cookie中存在
        /// <summary>
        /// 判定指定的key-value对是否在cookie中存在
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool Exists(String key, String value)
        {
            return HtmlPage.Document.Cookies.Contains(String.Format("{0}={1}", key, value));
        }
        #endregion

        #region 获取当前cookie内容
        /// <summary>
        /// 获取当前cookie内容
        /// </summary>
        /// <returns></returns>
        public static string getCookieContent()
        {
            return HtmlPage.Document.GetProperty("cookie") as String;
        }

        #endregion

    }
}
