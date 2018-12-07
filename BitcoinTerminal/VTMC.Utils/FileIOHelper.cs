/*************************************************
*Author:Zhang danhong
*Date:2017/04/11 18:48:37
*Des:文件工具类
************************************************/
using System;
using System.Collections.Generic;
using System.IO;

namespace VTMC.Utils
{
    /// <summary>
    /// 文件操作类
    /// </summary>
    public static class FileIOHelper
    {
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="info"></param>
        public static void DeleteFile(string path)
        {
            LogHelper.WriteMethodLog(true);
            if (File.Exists(path)) File.Delete(path);
            LogHelper.WriteMethodLog(false);
        }
        /// <summary>
        /// 删除文件夹
        /// </summary>
        /// <param name="path"></param>
        public static void DeleteDir(string path)
        {
            LogHelper.WriteMethodLog(true);
            if (Directory.Exists(path)) Directory.Delete(path, true);
            LogHelper.WriteMethodLog(false);
        }

        /// <summary>
        /// 写入数据到Text文件中
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="info">写入内容</param>
        public static void WriteToText(string path,string info)
        {
            LogHelper.WriteMethodLog(true);
            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.Write(info);
                sw.Flush();
            }
            LogHelper.WriteMethodLog(false);
        }

        /// <summary>
        /// 读取Text文件内容
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns>文件内容</returns>
        public static string ReadFromText(string path)
        {
            LogHelper.WriteMethodLog(true);
            try
            {
                string result = string.Empty;
                using (StreamReader sr = new StreamReader(path))
                {
                    result = sr.ReadToEnd();
                }

                return result;
            }
            finally
            {
                LogHelper.WriteMethodLog(false);
            }
        }

        /// <summary>
        /// 文件转为byte[]
        /// </summary>
        /// <param name="fileName">文件</param>
        /// <returns></returns>
        public static byte[] FileContent(string fileName)
        {
            LogHelper.WriteMethodLog(true);
            FileStream fs = null;
            try
            {
                fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                byte[] buffur = new byte[fs.Length];
                fs.Read(buffur, 0, (int)fs.Length);
                return buffur;
            }
            finally
            {
                LogHelper.WriteMethodLog(false);
                if (fs != null)
                {
                    fs.Close();
                }
            }
        }

        /// <summary>
        /// byte[]转为文件
        /// </summary>
        /// <param name="pReadByte">byte[]</param>
        /// <param name="fileName">文件</param>
        /// <returns></returns>
        public static bool WriteFile(byte[] pReadByte, string fileName)
        {
            LogHelper.WriteMethodLog(true);
            FileStream fs = null;
            BinaryWriter bw = null;
            try
            {
                fs = new FileStream(fileName, FileMode.OpenOrCreate);
                bw = new BinaryWriter(fs);
                bw.Write(pReadByte, 0, pReadByte.Length);
                return true;
            }
            finally
            {
                LogHelper.WriteMethodLog(false);
                if (fs != null || bw != null)
                {
                    bw.Dispose();
                }
            }
        }

        /// <summary>
        /// 获取文件大小(单位：KB)
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static double GetFileSize(string filePath)
        {
            //判断判断文件是否存在
            if (File.Exists(filePath) == false) return 0;

            //以获取其大小
            FileInfo fileInfo = new FileInfo(filePath);
            return fileInfo.Length / 1024.0;
        }

        /// <summary>
        /// 删除文件修改日期是否小于day天
        /// </summary>
        /// <param name="dir">目录地址</param>
        /// <param name="day">删除小于day天</param>
        /// <returns></returns>
        public static void DeleteOldFileInFolder(Dictionary<string, double> dic)
        {
            foreach (KeyValuePair<string, double> keyValuePair in dic)
            {
                if (Directory.Exists(keyValuePair.Key))
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(keyValuePair.Key);

                    //获取文件夹下所有的文件
                    foreach (System.IO.FileInfo fileInfo in dirInfo.GetFiles())
                    {
                        //判断文件修改日期是否小于day天以前，是则删除
                        if (fileInfo.LastWriteTime < DateTime.Now.AddDays(keyValuePair.Value))
                        {
                            fileInfo.Delete();
                            //FileIOHelper.DeleteFile(fileInfo.FullName);
                        }
                    }
                }
            }
        }
    }
}
