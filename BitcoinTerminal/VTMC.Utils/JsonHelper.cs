/*************************************************
*Author:zhang danhong
*Date:2017/04/27 18:29:02
*Des:Json工具类
************************************************/
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace VTMC.Utils
{
    /// <summary>
    /// Json数据工具类
    /// </summary>
    public static class JsonHelper
    {
        /// <summary>
        /// 将对象序列化为JSON格式
        /// </summary>
        /// <param name="o">对象</param>
        /// <returns>json字符串</returns>
        public static string Serializer<T>(T o)
        {
            try
            {
                return JsonConvert.SerializeObject(o);
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorCodeLog(ErrorCodes.E22_220, ex);
                return ExceptionManager.MVTMExceptionHandler(new MVTMException(ErrorCodes.E22_220));
            }
        }

        /// <summary>
        /// 反序列化JSON到给定的匿名对象.
        /// </summary>
        /// <typeparam name="T">匿名对象类型</typeparam>
        /// <param name="json">json字符串</param>
        /// <param name="anonymousTypeObject">匿名对象</param>
        /// <returns>匿名对象</returns>
        public static T Deserialize<T>(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorCodeLog(ErrorCodes.E22_221, ex);
                throw new MVTMException(ErrorCodes.E22_221);
            }
        }

        /// <summary>
        /// 判断Json数据单个Key是否存在
        /// </summary>
        /// <param name="json"></param>
        /// <param name="itemKey"></param>
        /// <returns></returns>
        public static bool CheckJsonItemKey(string json, string itemKey)
        {
            LogHelper.WriteMethodLog(true);
            try
            {
                var jObject = JObject.Parse(json);

                if(jObject.Property(itemKey) == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            finally
            {
                LogHelper.WriteMethodLog(false);
            }
        }

        /// <summary>
        /// 获取Json数据单个字段值
        /// </summary>
        /// <param name="json"></param>
        /// <param name="itemKey"></param>
        /// <returns></returns>
        public static string GetJsonItemValue(string json, string itemKey)
        {
            LogHelper.WriteMethodLog(true);
            try
            {
                var jObject = JObject.Parse(json);

                return jObject[itemKey]?.ToString();
            }
            finally
            {
                LogHelper.WriteMethodLog(false);
            }
        }

        /// <summary>
        /// 追加指定字段到Json字符串
        /// </summary>
        /// <param name="json"></param>
        /// <param name="itemKey"></param>
        /// <param name="itemValue"></param>
        /// <returns></returns>
        public static string AddJsonItemValue(string json, string itemKey, string itemValue)
        {
            var jObject = JObject.Parse(json);

            jObject[itemKey] = itemValue;
            return jObject?.ToString();
        }

        /// <summary>
        /// 获取指定Json数组值
        /// </summary>
        /// <param name="json">数组json</param>
        /// <param name="intemKey"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string JsonAarrayIndexValue(string json,string itemKey, int index)
        {
            JArray jar = JArray.Parse(json);
            JObject j = JObject.Parse(jar[index].ToString());
            return j[itemKey]?.ToString();
        }
    }
}