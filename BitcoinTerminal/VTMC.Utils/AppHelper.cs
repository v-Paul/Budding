using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace VTMC.Utils
{
    /// <summary>
    /// 系统辅助类
    /// </summary>
    public static class AppHelper
    {
        /// <summary>
        /// 获取版本号
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentVersionNumberStr()
        {
            return System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetEntryAssembly().Location).ProductVersion;
        }

        /// <summary>
        /// 获取IP地址
        /// </summary>
        /// <returns></returns>
        public static List<string> GetIpAddress()
        {
            string hostName = Dns.GetHostName();   //获取本机名
            IPHostEntry localhost = Dns.GetHostByName(hostName);    //方法已过期，可以获取IPv4的地址
                                                                    //IPHostEntry localhost = Dns.GetHostEntry(hostName);   //获取IPv6地址
            List<string> lsIp = new List<string>();
            localhost.AddressList.ToList().ForEach(x =>
            {
                lsIp.Add(x.ToString());
            });
            return lsIp;
        }

        /// <summary>
        /// 获取IP地址(多个以逗号分割)
        /// </summary>
        /// <returns></returns>
        public static string GetIpAddressString()
        {
            string hostName = Dns.GetHostName();   //获取本机名
            IPHostEntry localhost = Dns.GetHostByName(hostName);
            string ipStr = string.Empty;
            foreach (var item in localhost.AddressList)
            {
                ipStr += item.ToString() + ",";
            }
            if (!string.IsNullOrEmpty(ipStr))
            {
                ipStr = ipStr.Substring(0, ipStr.Length - 1);
            }
            return ipStr;
        }

        /// <summary>
        /// 获取主机名
        /// </summary>
        /// <returns></returns>
        public static string GetHostName()
        {
            string hostName = Dns.GetHostName();   //获取本机名
            return hostName;
        }
    }
}
