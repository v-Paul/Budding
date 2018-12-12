using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
namespace VTMC.Utils
{
    /// <summary>
    /// 压缩解压缩辅助类
    /// </summary>
    public static class ZipHelper
    {
        ///// <summary>
        ///// 将一组文件压缩至一个Zip文件中
        ///// </summary>
        ///// <param name="files">文件列表</param>
        ///// <param name="zipedFileName">目标Zip文件</param>
        ///// <param name="compressionLevel">压缩级别</param>
        //public static void ZipFiles(IEnumerable<string> files, string zipedFileName, int compressionLevel = 9)
        //{
        //    if (File.Exists(zipedFileName)) File.Delete(zipedFileName);
        //    if (Path.GetExtension(zipedFileName) != ".zip") zipedFileName = zipedFileName + ".zip";
        //    using (var zipOutputStream = new ZipOutputStream(File.Create(zipedFileName)))
        //    {
        //        zipOutputStream.SetLevel(compressionLevel);
        //        Crc32 crc = new Crc32();
        //        files.ToList().ForEach(file => ZipFile(file, zipOutputStream, crc));
        //    }
        //}

        ///// <summary>
        ///// 将一个文件夹内的内容压缩至一个ZIP文件中
        ///// </summary>
        ///// <param name="folderName"></param>
        ///// <param name="zipedFileName"></param>
        ///// <param name="compressionLevel"></param>
        //public static void ZipFolder(string folderName, string zipedFileName, int compressionLevel = 9)
        //{
        //    if (File.Exists(zipedFileName)) File.Delete(zipedFileName);
        //    if (Path.GetExtension(zipedFileName) != ".zip") zipedFileName = zipedFileName + ".zip";
        //    using (var zipOutputStream = new ZipOutputStream(File.Create(zipedFileName)))
        //    {
        //        zipOutputStream.SetLevel(compressionLevel);
        //        ZipDirectory(folderName, zipOutputStream);
        //    }
        //}


        ///// <summary>
        ///// 递归压缩指定文件夹
        ///// </summary>
        ///// <param name="folderName">文件夹路径</param>
        ///// <param name="zipOutputStream">ZIP输出流</param>
        ///// <param name="parentFolder">父级目录</param>
        //private static void ZipDirectory(string folderName, ZipOutputStream zipOutputStream)
        //{
        //    var crc = new Crc32();

        //    //压缩当前文件夹内的文件
        //    var files = Directory.GetFiles(folderName);
        //    var current = Path.GetDirectoryName(folderName);
        //    files.ToList().ForEach(file => ZipFile(file, zipOutputStream, crc, current));

        //    //递归压缩子文件夹内的文件
        //    var folders = Directory.GetDirectories(folderName);
        //    folders.ToList().ForEach(folder => ZipDirectory(folder, zipOutputStream));
        //}

        ///// <summary>
        ///// 压缩指定文件至ZIP输出流
        ///// </summary>
        ///// <param name="fileName">文件路径</param>
        ///// <param name="zipOutputStream">Zip输出流</param>
        ///// <param name="crc">Entry管理器</param>
        ///// <param name="parentFolder">父级目录</param>
        //private static void ZipFile(string fileName, ZipOutputStream zipOutputStream, Crc32 crc, string parentFolder = "")
        //{
        //    if (!File.Exists(fileName))
        //        throw new FileNotFoundException(string.Format("请确认文件<{0}>是否存在!", fileName));
        //    using (var fileStream = File.OpenRead(fileName))
        //    {
        //        var buffer = new byte[fileStream.Length];
        //        fileStream.Read(buffer, 0, buffer.Length);
        //        var entryName = Path.Combine(parentFolder, Path.GetFileName(fileName));
        //        var entry = new ZipEntry(entryName);
        //        entry.DateTime = DateTime.Now;
        //        entry.Size = fileStream.Length;
        //        fileStream.Close();
        //        crc.Reset();
        //        crc.Update(buffer);
        //        entry.Crc = crc.Value;
        //        zipOutputStream.PutNextEntry(entry);
        //        zipOutputStream.Write(buffer, 0, buffer.Length);
        //    }
        //}

        ///// <summary>
        ///// 解压缩指定Zip文件
        ///// </summary>
        ///// <param name="zipFilePath"></param>
        ///// <param name="unZipDir"></param>
        //public static void UnZip(string zipFilePath, string unZipDir = "")
        //{
        //    if (zipFilePath == string.Empty)
        //        throw new Exception("压缩文件不能为空！");
        //    if (!File.Exists(zipFilePath))
        //        throw new FileNotFoundException("压缩文件不存在！");
        //    if (unZipDir == string.Empty)
        //        unZipDir = zipFilePath.Replace(Path.GetFileName(zipFilePath), Path.GetFileNameWithoutExtension(zipFilePath));
        //    if (!unZipDir.EndsWith("/"))
        //        unZipDir += "/";
        //    if (!Directory.Exists(unZipDir))
        //        Directory.CreateDirectory(unZipDir);

        //    using (var s = new ZipInputStream(File.OpenRead(zipFilePath)))
        //    {

        //        ZipEntry theEntry;
        //        while ((theEntry = s.GetNextEntry()) != null)
        //        {
        //            string directoryName = Path.GetDirectoryName(theEntry.Name);
        //            string fileName = Path.GetFileName(theEntry.Name);
        //            if (!string.IsNullOrEmpty(directoryName))
        //            {
        //                Directory.CreateDirectory(unZipDir + directoryName);
        //            }
        //            if (directoryName != null && !directoryName.EndsWith("/"))
        //            {
        //            }
        //            if (fileName != String.Empty)
        //            {
        //                using (FileStream streamWriter = File.Create(unZipDir + theEntry.Name))
        //                {

        //                    int size;
        //                    byte[] data = new byte[2048];
        //                    while (true)
        //                    {
        //                        size = s.Read(data, 0, data.Length);
        //                        if (size > 0)
        //                        {
        //                            streamWriter.Write(data, 0, size);
        //                        }
        //                        else
        //                        {
        //                            break;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// 解压缩
        /// </summary>
        /// <param name="zipFilePath">ZIP文件路径</param>
        /// <param name="unZipDir">解压路径，默认为ZIP文件同级目录</param>
        public static void UnZip(string zipFilePath, string unZipDir = "")
        {
            if (string.IsNullOrEmpty(unZipDir)) unZipDir = Path.GetDirectoryName(zipFilePath);
            System.IO.Compression.ZipFile.ExtractToDirectory(zipFilePath, unZipDir);
        }

        /// <summary>
        /// 压缩
        /// </summary>
        /// <param name="folderToZip">需要压缩的文件夹</param>
        /// <param name="zipedFileName">目标Zip文件</param>
        public static void Zip(string folderToZip, string zipedFileName)
        {
            if (!Directory.Exists(folderToZip)) throw new DirectoryNotFoundException("请确认指定目录是否存在。");
            if (File.Exists(zipedFileName)) File.Delete(zipedFileName);
            System.IO.Compression.ZipFile.CreateFromDirectory(folderToZip, zipedFileName,CompressionLevel.Fastest,false);
        }

        #region Add by Paul

        private static bool ZipDirectory(string folderPath, ZipOutputStream zipStream, string entryFolder)
        {
            bool result = true;
            string[] folders, files;
            ZipEntry ent = null;
            FileStream fs = null;
            Crc32 crc = new Crc32();

            try
            {
                if (folderPath[folderPath.Length - 1] != System.IO.Path.DirectorySeparatorChar)
                {
                    folderPath += System.IO.Path.DirectorySeparatorChar;
                }

                string tmpfolder = folderPath.Substring(folderPath.IndexOf(entryFolder));

                files = Directory.GetFiles(folderPath);
                foreach (string file in files)
                {
                    fs = File.OpenRead(file);

                    byte[] buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);
                    ent = new ZipEntry(Path.Combine(tmpfolder, Path.GetFileName(file)));
                    ent.DateTime = DateTime.Now;
                    ent.Size = fs.Length;
                    fs.Close();
                    zipStream.PutNextEntry(ent);
                    zipStream.Write(buffer, 0, buffer.Length);
                }

            }
            catch (Exception ex)
            {
                result = false;
                throw ex;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
                if (ent != null)
                {
                    ent = null;
                }
                GC.Collect();
                GC.Collect(1);
            }

            folders = Directory.GetDirectories(folderPath);
            foreach (string folder in folders)
            {
                if (!ZipDirectory(folder, zipStream, entryFolder))
                { return false; }
            }

            return result;
        }

        public static bool ZipFilesAndChildFiles(string folderPath, string zipFileName)
        {
            ZipOutputStream zipStream = null;
            try
            {
                if (!Directory.Exists(folderPath))
                {
                    Console.WriteLine("Cannot find directory '{0}'", folderPath);
                    return false;
                }

                if (folderPath[folderPath.Length - 1] != System.IO.Path.DirectorySeparatorChar)
                {
                    folderPath += System.IO.Path.DirectorySeparatorChar;
                }

                string tmp = folderPath.Substring(0, folderPath.LastIndexOf("\\"));
                string entryFolder = tmp.Substring(tmp.LastIndexOf("\\") + 1);


                if (!zipFileName.ToLower().EndsWith(ConstHelper.ZIPExtension))
                {
                    zipFileName += ConstHelper.ZIPExtension;
                }

                zipStream = new ZipOutputStream(File.Create(zipFileName));
                zipStream.SetLevel(9);

                ZipDirectory(folderPath, zipStream, entryFolder);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                zipStream?.Finish();
                zipStream?.Close();
            }
        }

        public static bool ZipFiles(string folderPath, string zipFileName)
        {
            if (!Directory.Exists(folderPath))
            {
                Console.WriteLine("Cannot find directory '{0}'", folderPath);
                return false;
            }

            try
            {
                if (!zipFileName.ToLower().EndsWith(ConstHelper.ZIPExtension))
                {
                    zipFileName += ConstHelper.ZIPExtension;
                }

                string[] filenames = Directory.GetFiles(folderPath);
                using (ZipOutputStream s = new ZipOutputStream(File.Create(zipFileName)))
                {
                    s.SetLevel(9); // 压缩级别 0-9
                    //s.Password = "123"; //Zip压缩文件密码
                    byte[] buffer = new byte[4096]; //缓冲区大小
                    foreach (string file in filenames)
                    {
                        ZipEntry entry = new ZipEntry(Path.GetFileName(file));
                        entry.DateTime = DateTime.Now;
                        s.PutNextEntry(entry);
                        using (FileStream fs = File.OpenRead(file))
                        {
                            int sourceBytes;
                            do
                            {
                                sourceBytes = fs.Read(buffer, 0, buffer.Length);
                                s.Write(buffer, 0, sourceBytes);
                            } while (sourceBytes > 0);
                        }
                    }
                    s.Finish();
                    s.Close();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
