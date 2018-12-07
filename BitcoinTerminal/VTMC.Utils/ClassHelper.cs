/*************************************************
*Author:zhangdanhong
*Date:2016/12/6 18:48:37
*Des:Class对象处理
************************************************/

using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace VTMC.Utils
{
    /// <summary>
    /// Class对象处理
    /// </summary>
    public static class ClassHelper
    {
        /// <summary>
        /// 拷贝模型(深度拷贝)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="RealObject"></param>
        /// <returns></returns>
        public static T Copy<T>(T RealObject)
        {
            //using (Stream objectStream = new MemoryStream())
            //{
            //    //利用 System.Runtime.Serialization序列化与反序列化完成引用对象的复制 
            //    IFormatter formatter = new BinaryFormatter();
            //    formatter.Serialize(objectStream, RealObject);
            //    objectStream.Seek(0, SeekOrigin.Begin);
            //    return (T)formatter.Deserialize(objectStream);
            //}

            string data = JsonHelper.Serializer<T>(RealObject);

            return JsonHelper.Deserialize<T>(data);
        }
    }
}
