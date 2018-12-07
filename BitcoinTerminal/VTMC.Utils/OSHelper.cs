/*************************************************
*Author:fan danpeng
*Date:20/12/2017 8:17:35 PM
*Des:  
************************************************/
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Management;

namespace VTMC.Utils
{
    public class OSHelper
    {
        /// <summary>
        /// 获取系统位数
        /// </summary>
        /// <returns></returns>
        public static int GetOSBit()
        {
            bool is64Bit;
            int a = 0;
            is64Bit = Environment.Is64BitOperatingSystem;
            if (is64Bit) { a = 64; }
            else { a = 32; }

            return a;
        }

        /// <summary>
        /// 获取操作系统版本
        /// </summary>
        /// <returns></returns>
        public static string GetOsVersion()
        {
            ////判断 
            //string iniPath = Environment.GetFolderPath(Environment.SpecialFolder.System);
            ////获取系统信息                
            //System.OperatingSystem osInfo = System.Environment.OSVersion;
            ////获取操作系统ID                
            //System.PlatformID platformID = osInfo.Platform;
            ////获取主版本号                
            //int versionMajor = osInfo.Version.Major;
            ////获取副版本号                
            //int versionMinor = osInfo.Version.Minor;
            //string osInfor = platformID.GetHashCode().ToString() + versionMajor.ToString() + versionMinor.ToString();
            ////return osInfor;
            ////return 
            ////(OsVer)
            //   int a = (OsVer.Parse(OsVerosInfor));

            //return "";

            string str = "Unknown";

            string hdId = string.Empty;
            ManagementClass hardDisk = new ManagementClass("Win32_OperatingSystem");
            ManagementObjectCollection hardDiskC = hardDisk.GetInstances();
            foreach (ManagementObject m in hardDiskC)
            {
                str = m["Name"].ToString().Split('|')[0].Replace("Microsoft", "");
                break;
            }

            return str;


        }

        /// <summary>
        /// 获取计算机型号
        /// </summary>
        /// <returns></returns>
        public static string GetComputerProduct()
        {
            string str = string.Empty;
            string hdId = string.Empty;
            ManagementClass hardDisk = new ManagementClass("Win32_ComputerSystemProduct");
            ManagementObjectCollection hardDiskC = hardDisk.GetInstances();
            foreach (ManagementObject m in hardDiskC)
            {
                str = m["Version"].ToString(); break;
            }

            return str;
        }

        public static int GetOSVersionMajorNum()
        {
            //判断 
            string iniPath = Environment.GetFolderPath(Environment.SpecialFolder.System);
            //获取系统信息                
            System.OperatingSystem osInfo = System.Environment.OSVersion;
            //获取操作系统ID                
            System.PlatformID platformID = osInfo.Platform;
            //获取主版本号                
            int versionMajor = osInfo.Version.Major;

            return versionMajor;

        }

    }

}
