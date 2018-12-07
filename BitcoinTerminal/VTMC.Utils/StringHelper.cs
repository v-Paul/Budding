/*************************************************
*Author:Zhang danhong
*Date:2017/04/11 18:48:37
*Des:字符串操作类
************************************************/
using System;

namespace VTMC.Utils
{
    /// <summary>
    /// 字符串操作类
    /// </summary>
    public static class StringHelper
    {
        /// <summary>
        /// 对字符串按照固定长度整形
        /// 在有换行处理的地方需要注意使用
        /// </summary>
        /// <param name="data">字符串</param>
        /// <param name="len">每行长度</param>
        /// <returns></returns>
        public static string CutString(string data, int len)
        {
            int rowCount = 1;
            string result = string.Empty;
            int tmpLen = 0;

            foreach (var chr in data)
            {
                tmpLen = GetStringLen(result + chr);
                if (tmpLen == len * rowCount)
                {
                    result += chr;
                    rowCount++;
                }
                else if (tmpLen > len * rowCount)
                {
                    result += " ";
                    result += chr;
                    rowCount++;
                }
                else
                {
                    result += chr;
                }
            }

            return result;
        }

        /// <summary>
        /// 取得字符串字节数
        /// </summary>
        /// <param name="data">字符串</param>
        /// <returns></returns>
        public static int GetStringLen(string data)
        {
            byte[] sarr = System.Text.Encoding.Default.GetBytes(data);
            return sarr.Length;
        }
    }
}
