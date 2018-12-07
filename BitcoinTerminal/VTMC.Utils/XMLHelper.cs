/*************************************************
*Author:Zhang danhong
*Date:2017/04/13 18:48:37
*Des:XML工具类
************************************************/
using System.IO;
using System.Xml.Serialization;

namespace VTMC.Utils
{
    /// <summary>
    /// Xml数据工具类
    /// </summary>
    public static class XMLHelper
    {
        #region 反序列化
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="xml">XML字符串</param>
        /// <returns></returns>
        public static T Deserialize<T>(string xml)
        {
            LogHelper.WriteMethodLog(true);
            try
            {
                using (StringReader sr = new StringReader(xml))
                {
                    XmlSerializer xmldes = new XmlSerializer(typeof(T));
                    return (T)xmldes.Deserialize(sr);
                }
            }
            finally
            {
                LogHelper.WriteMethodLog(false);
            }
        }
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="type"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static T DeserializeFromXmlFile<T>(string xmlpath)
        {
            LogHelper.WriteMethodLog(true);
            try
            {
                object result = null;
                if (File.Exists(xmlpath))
                {
                    using (StreamReader reader = new StreamReader(xmlpath))
                    {
                        XmlSerializer xs = new XmlSerializer(typeof(T));
                        result = xs.Deserialize(reader);
                    }
                }
                return (T)result;
            }
            finally
            {
                LogHelper.WriteMethodLog(false);
            }
        }
        #endregion

        #region 序列化
        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static string Serializer<T>(T obj)
        {
            LogHelper.WriteMethodLog(true);
            try
            {
                MemoryStream Stream = new MemoryStream();
                XmlSerializer xml = new XmlSerializer(typeof(T));
                //序列化对象
                xml.Serialize(Stream, obj);

                Stream.Position = 0;
                StreamReader sr = new StreamReader(Stream);
                string str = sr.ReadToEnd();

                sr.Dispose();

                return str;
            }
            finally
            {
                LogHelper.WriteMethodLog(false);
            }
        }
        /// <summary>
        /// 序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlFilePath"></param>
        /// <param name="obj"></param>
        public static void SerializeToXmlFile<T>(string xmlFilePath, T obj)
        {
            LogHelper.WriteMethodLog(true);
            try
            {
                using (StreamWriter sw = new StreamWriter(xmlFilePath))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(T));
                    xs.Serialize(sw, obj);
                }
            }
            finally
            {
                LogHelper.WriteMethodLog(false);
            }
        }
        #endregion
    }
}
