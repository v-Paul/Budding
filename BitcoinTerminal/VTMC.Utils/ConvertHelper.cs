/*************************************************
*Author:Zhang danhong
*Date:2017/04/11 18:48:37
*Des:类型转换工具类
************************************************/
using System;
using System.Text;

namespace VTMC.Utils
{
    /// <summary>
    /// 类型转换操作类
    /// </summary>
    public static class ConvertHelper
    {
        /// <summary>
        /// 判断字符是否为数值型
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool IsInt(string val)
        {
            int refVal = 0;
            return int.TryParse(val, out refVal);
        }

        /// <summary>
        /// 转换成数值型
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static int ToInt(string val)
        {
            int refVal = 0;

            if (int.TryParse(val, out refVal))
            {
                return refVal;
            }
            else
            {
                return 0;
            }
        }
        /// <summary>
        /// 转换成数值型
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static Int16 ToInt16(string val)
        {
            Int16 refVal = 0;

            if (Int16.TryParse(val, out refVal))
            {
                return refVal;
            }
            else
            {
                return 0;
            }
        }
        /// <summary>
        /// 转换成数值型
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static UInt32 ToInt32(string val)
        {
            UInt32 refVal = 0;

            if (UInt32.TryParse(val, out refVal))
            {
                return refVal;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 转换成日期型
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(string val)
        {
            DateTime refVal = DateTime.Now;
            DateTime.TryParse(val, out refVal);
            return refVal;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        /// <param name="defDate"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(string val, DateTime defDate)
        {
            DateTime.TryParse(val, out defDate);
            return defDate;
        }


        /// <summary>
        /// 日期转标准ISO8601格式
        /// </summary>
        /// <returns></returns>
        public static string ToDateTimeISOString(string dtStr)
        {
            DateTime dt;

            if (DateTime.TryParse(dtStr, out dt))
            {
                return dt.ToString("yyyy-MM-ddTHH:mm:ss.fffzz00", System.Threading.Thread.CurrentThread.CurrentUICulture); // 得到日期字符串
            }
            else
            {
                return string.Empty;
            }
        }


        /// <summary>
        /// 转换成布尔型
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool ToBool(string val)
        {
            bool refVal = false;

            if (IsInt(val))
            {
                return ToBool(ToInt(val));
            }
            else
            {
                bool.TryParse(val, out refVal);
                return refVal;
            }
        }
        /// <summary>
        /// 转换成布尔型
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool ToBool(int val)
        {
            if(val ==1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 转换Double型
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static double ToDouble(string val)
        {
            double refVal = 0.0;

            if (double.TryParse(val, out refVal))
            {
                return refVal;
            }
            else
            {
                return 0.0;
            }
        }

        /// <summary>
        /// 转换float型
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static float ToFloat(string val)
        {
            float refVal = 0.0f;

            if (float.TryParse(val, out refVal))
            {
                return refVal;
            }
            else
            {
                return 0.0f;
            }
        }

        /// <summary>
        /// Bytes转十六进制字符串
        /// </summary>
        public static string BytesToHex(byte[] data)
        {
            StringBuilder sb = new StringBuilder(data.Length * 2);
            foreach (byte b in data)
            {
                sb.AppendFormat("{0:X2}", b);
            }
            return sb.ToString();
        }
    }
}
