/*************************************************
*Author:zhang danhong
*Date:2017/04/28 18:29:02
*Des:系统运行参数类
************************************************/
using System;
using VTMC.Utils.Models;

namespace VTMC.Utils
{
    /// <summary>
    /// 系统运行参数类
    /// </summary>
    public static class AppSettings
    {
        #region Consts
        #endregion

        #region Fileds

        #endregion

        #region Property
        #region old property
        /// <summary>
        /// 主窗口
        /// </summary>
        public static System.Windows.Window mainForm { get; set; }

        /// <summary>
        /// 当前消息窗体
        /// </summary>
        public static System.Windows.Window messageform { get; set; }

        /// <summary>
        /// Windows系统版本
        /// </summary>
        public static double OSVersion { get; set; }
        /// <summary>
        /// 应用程序版本号
        /// </summary>
        public static string ProductVersion { get; set; }

        /// <summary>
        /// VTM页面地址
        /// </summary>
        public static string VTMURL { get; set; }


        /// <summary>
        /// 账户VT01/VT02
        /// </summary>
        public static string Account { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public static string Password { get; set; }


        /// <summary>
        /// 登录加密Key
        /// </summary>
        public static string LoginKey { get; set; }

        /// <summary>
        /// RSA加密Key
        /// </summary>
        public static string RSAPublickey { get; set; }

        /// <summary>
        /// 登录加密偏移量
        /// </summary>
        public static string LoginIV { get; set; }

        /// <summary>
        /// 打印完EJ
        /// </summary>
        public static string EJPrintedFolder { get; set; }

        /// <summary>
        /// Java服务器
        /// </summary>
        public static string JavaHost { get; set; }
        /// <summary>
        /// 上传服务器URL
        /// </summary>
        public static string FileUploadService { get; set; }

        /// <summary>
        /// EJ上传服务器URL
        /// </summary>
        public static string EJFileUploadService { get; set; }

        /// <summary>
        /// 登录URL
        /// </summary>
        public static string LoginURL { get; set; }

        /// <summary>
        /// EJAtmp数据获取URL
        /// </summary>
        public static string EJGetAtmpURL { get; set; }

        /// <summary>
        /// 服务器获得到的Token
        /// </summary>
        public static OAuthTokenModel OAuthToken { get; set; }

        /// <summary>
        /// EJ服务器获得到的Token
        /// </summary>
        public static OAuthTokenModel EJOAuthToken { get; set; }
        /// <summary>
        /// EJ 登录用户名
        /// </summary>
        public static string EJAccount { get; set; }
        /// <summary>
        /// VTMC公共文件夹
        /// </summary>
        public static string VTMCommonFolder { get; set; }

        /// <summary>
        /// MVTM Temp文件夹
        /// </summary>
        public static string MVTMTempFolder { get; set; }

        /// <summary>
        /// MVTMC ExceptionFile 文件夹
        /// </summary>
        public static string MVTMExceptionFileFolder { get; set; }
        /// <summary>
        /// MVTMC IDCardImgs 文件夹
        /// </summary>
        public static string MVTMIDCardImgsFolder { get; set; }

        /// <summary>
        /// MVTM 业务文件保存文件夹
        /// </summary>
        public static string MVTMBusinessFolder { get; set; }

        /// <summary>
        /// 保存当前用户前端版本信息文件路径
        /// </summary>
        public static string ConfigurationSaveFolder { get; set; }

        /// <summary>
        /// 设置系统语言
        /// </summary>
        public static string Language { get; set; }
        /// <summary>
        /// 设置系统字体
        /// </summary>
        public static string FamilyName { get; set; }

        /// <summary>
        /// 系统GuID
        /// </summary>
        public static string GuID
        {
            get
            {
                return Guid.NewGuid().ToString();
            }
        }

        /// <summary>
        /// HTTPS证书名
        /// </summary>
        public static string HTTPSCertFileName { get; set; }

        /// <summary>
        /// MainWindowTopMost置顶设置
        /// </summary>
        public static bool MainWindowTopMost { get; set; }

        /// <summary>
        /// 全局JSBridge
        /// </summary>
        public static object JSBridge { get; set; }

        /// <summary>
        /// 获取JSVersionService
        /// </summary>
        public static string GetJSVersionService { get; set; }

        #region Wosa设定
        /// <summary>
        /// Wosa设备运行参数模型
        /// </summary>
        public static WOSADeviceSettingModel WosaSetting { get; set; }
        #endregion

        /// <summary>
        /// 客户密码加密用Key
        /// </summary>
        public static string ConsumerPinKey { get; set; }

        /// <summary>
        /// EJ文件全路径
        /// </summary>
        public static string EJFilePath { get; set; }
        /// <summary>
        /// EJ文件缓存文件夹
        /// </summary>
        public static string EJFileBackDir { get; set; }
        #endregion

        /// <summary>
        /// VTMC公共文件夹
        /// </summary>
        public static string XXPCommonFolder { get; set; }
        public static string XXPDBFolder { get; set; }
        public static string XXPKeysFolder { get; set; }
        public static string XXPLogFolder { get; set; }
        public static string XXPTempFolder { get; set; }
        public static int XXPCommport { get; set; }
        public static int XXPTransFilePort { get; set; }
        //
        #endregion

        #region Public Function

        /// <summary>
        /// 设置自动旋转
        /// </summary>
        /// <param name="isEable"></param>
        public static void EnableAutoRotation(bool isEable)
        {
            if (AppSettings.OSVersion > 6.1)
            {
                SetRotationType(isEable);
            }
        }

        /// <summary>
        /// 设置是否可以自动旋转屏幕
        /// </summary>
        /// <param name="isEable"></param>
        private static void SetRotationType(bool isEable)
        {
            if (isEable)
            {
                Windows.Graphics.Display.DisplayInformation.AutoRotationPreferences = Windows.Graphics.Display.DisplayOrientations.None;
            }
            else
            {
                Windows.Graphics.Display.DisplayInformation.AutoRotationPreferences = Windows.Graphics.Display.DisplayOrientations.Landscape;
            }
        }
        #endregion

        #region Private Function

        #endregion
    }
}