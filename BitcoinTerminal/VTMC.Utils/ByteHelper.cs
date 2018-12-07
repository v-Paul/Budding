/*************************************************
*Author:zhang danhong
*Date:2017/04/27 18:29:02
*Des:Byte工具类
************************************************/
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace VTMC.Utils
{
    /// <summary>
    /// Byte数据工具类
    /// </summary>
    public static class ByteHelper
    {
        /// <summary>
        /// 将一个object对象序列化，返回一个byte[]    
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] ObjectToBytes<T>(T obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                return ms.GetBuffer();
            }
        }

        /// <summary>
        /// 将Byte转换为结构体类型
        /// </summary>
        /// <param name="bytData"></param>
        /// <returns></returns>
        public static T BytesToObject<T>(byte[] bytData)
        {
            using (MemoryStream ms = new MemoryStream(bytData))
            {
                IFormatter formatter = new BinaryFormatter();
                return (T)formatter.Deserialize(ms);
            }
        }
    }
}