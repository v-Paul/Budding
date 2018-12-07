/*************************************************
*Author:Paul Wang
*Date:6/1/2017 11:28:53 AM
*Des:  
************************************************/
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace VTMC.Utils
{
    /// <summary>
    /// 异常处理工具类
    /// </summary>
    public class ExceptionManager
    {
        #region MVTM Exception Handler
        /// <summary>
        /// 取得异常编码
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static string MVTMExceptionHandler(Exception exception)
        {
            try
            {
                throw exception;
            }
            catch (MVTMException userEx)
            {
                return JsonHelper.Serializer<ResultEntity>(userEx.RE);
            }
            catch (Exception)
            {
                ResultEntity re = new ResultEntity();
                re.hResult = ConstHelper.CNT_ERROR;
                re.errorCode = ErrorCodes.E21_999;
                re.message = "未知错误";
                return JsonHelper.Serializer<ResultEntity>(re);
            }
            finally
            {
                LogHelper.WriteMethodLog(false);
            }
        }
        #endregion
    }

    #region MVTM Exception
    /// <summary>
    /// 系统自定义异常错误
    /// </summary>
    [Serializable]
    public class MVTMException : Exception, ISerializable
    {
        /// <summary>
        /// 返回给前端的强类型结构
        /// </summary>
        public ResultEntity RE { get; internal set; }

        /// <summary>
        /// 系统自定义异常错误 构造函数
        /// </summary>
        /// <param name="errorCode"></param>
        public MVTMException(string errorCode)
        {
            RE = new ResultEntity();
            RE.hResult = ConstHelper.CNT_ERROR;
            RE.errorCode = errorCode;
            RE.message = ResourceHelper.GetResourceInfo(errorCode);
            //RE.data = data;
        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }

    #endregion

    #region Json entity for frontside
    /// <summary>
    /// 返回给前端的Json结构
    /// </summary>
    /// <example>
    ///     ResultEntity的Json序列化结构:
    ///     <code>
    ///     {
    ///         "hResult":"WFS_SUCCESS/WFS_ERROR",
    ///         "data":"错误附加数据/信息",
    ///         "errorCode": "错误Code",
    ///         "message":"错误消息"
    ///         "Code": "当前命令的命令码",
    ///         "Name":"当前模块的名称"
    ///     }
    ///     </code>
    ///     errorCode:参见<see cref="VTMC.Utils.ErrorCodes"/>
    /// </example>
    [Serializable]
    public class ResultEntity
    {
        /// <summary>
        /// 执行结果
        /// WFS_SUCCESS:成功
        /// WFS_ERROR:失败
        /// </summary>
        public string hResult { get; set; }
        /// <summary>
        /// 错误Code
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string errorCode { get; set; }
        /// <summary>
        /// 错误消息
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string message { get; set; }
        /// <summary>
        /// 错误附加数据/信息
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string data { get; set; }

        /// <summary>
        /// 当前命令的命令码
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Code { get; set; }
        /// <summary>
        /// 当前模块的名称
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
    }
    #endregion
}
