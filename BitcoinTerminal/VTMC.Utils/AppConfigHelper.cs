/*************************************************
*Author:zhang danhong
*Date:2016/12/6 18:29:02
*Des:AppConfig工具类
************************************************/
using System;
using System.Configuration;

namespace VTMC.Utils
{
    /// <summary>
    /// AppConfig工具类
    /// </summary>
    public static class AppConfigHelper
    {
        /// <summary>
        /// 获取配置设定内容
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetConfigValByKey(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }


        /// <summary>
        /// 修改AppSettings中配置
        /// </summary>
        /// <param name="key">key值</param>
        /// <param name="value">相应值</param>
        public static bool SetConfigValue(string key, string value)
        {
            try
            {
                LogHelper.WriteMethodLog(true);
                LogHelper.WriteMethodInfoLog(key, value);
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                if (config.AppSettings.Settings[key] != null)
                {
                    config.AppSettings.Settings[key].Value = value;
                }
                else
                {
                    config.AppSettings.Settings.Add(key, value);
                }
                config.AppSettings.SectionInformation.ForceSave = true;
                config.Save(ConfigurationSaveMode.Modified);
                //config.SaveAs(configFilePath, ConfigurationSaveMode.Modified, true);
                ConfigurationManager.RefreshSection("appSettings");
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorCodeLog(ex.Message, ex);
                return false;
            }
            finally
            {
                LogHelper.WriteMethodLog(false);
            }
        }
    }
}