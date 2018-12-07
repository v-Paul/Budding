/*************************************************
*Author:zhang danhong
*Date:2016/12/6 18:29:02
*Des:资源信息工具类
************************************************/
using System;
using System.Reflection;
using System.Resources;

namespace VTMC.Utils
{
    /// <summary>
    /// 系统运行参数类
    /// </summary>
    public static class ResourceHelper
    {
        #region Consts

        #endregion

        #region Fileds

        #endregion

        #region Property

        #endregion

        #region Public Function
        /// <summary>
        /// 获取资源文件内容
        /// </summary>
        /// <param name="resID"></param>
        /// <returns></returns>
        public static string GetResourceInfo(string resID)
        {
            try
            {
                ResourceManager resourceManager = new ResourceManager(Resource.Resource.ResourceManager.BaseName, Assembly.GetExecutingAssembly());
                return resourceManager.GetString(resID).Replace("\\r", Environment.NewLine);
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorInfoLog("Resource is not exist.", ex);
                return "";
            }
        }
        #endregion

        #region Private Function

        #endregion
    }
}