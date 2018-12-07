/*************************************************
*Author:Zhang danhong
*Date:2017/06/05 18:48:37
*Des:文件路径监控工具类
************************************************/
using System;
using System.IO;

namespace VTMC.Utils
{
    /// <summary>
    /// 文件路径监控
    /// </summary>
    public class FileWatcherHelper : IDisposable
    {
        #region Const
        #endregion

        #region Filed
        /// <summary>
        /// 监听文件系统目录变化
        /// </summary>
        private FileSystemWatcher watcher;
        #endregion

        #region CallBacks
        /// <summary>
        /// OCX执行回调统一函数
        /// </summary>
        public Action<FileWatcherExecuteResultModel> OcxExecuteCallBack;

        /// <summary>
        /// 文件监控类型
        /// </summary>
        private string Filter = string.Empty;
        #endregion

        #region Property
        #endregion

        #region Event
        /// <summary>
        /// 文件系统创建文件事件
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void OnProcess(object source, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Created)
            {
                bool Check = true;
                System.Threading.Thread.Sleep(200);

                if (!string.IsNullOrEmpty(this.Filter))
                {
                    Check = false;
                    string[] fit = this.Filter.Split('|');
                    foreach (var item in fit)
                    {
                        if (System.IO.Path.GetExtension(e.FullPath) == item)
                        {
                            Check = true;
                            break;
                        }
                    }
                }

                if (Check)
                {
                    FileWatcherExecuteResultModel resultMod = new FileWatcherExecuteResultModel();

                    resultMod.hResult = ConstHelper.CNT_SUCCESS;
                    resultMod.ReturnMessage = "文件获取成功";
                    //System.Drawing.Image img = System.Drawing.Image.FromFile(e.FullPath);
                    resultMod.FilePath = e.FullPath;
                    FileInfo file = new FileInfo(e.FullPath);
                    resultMod.FileSize = file.Length / 1024.0;
                    resultMod.ImageBase64String = ImageHelper.ImgToBase64String(e.FullPath);
                    GC.Collect();

                    this.OcxExecuteCallBack?.Invoke(resultMod);
                }
            }
            //else if (e.ChangeType == WatcherChangeTypes.Changed)
            //{
            //    OnChanged(source, e);
            //}
            //else if (e.ChangeType == WatcherChangeTypes.Deleted)
            //{
            //    OnDeleted(source, e);
            //}
        }
        /// <summary>
        /// 文件系统重命名事件
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void OnRenamed(object source, RenamedEventArgs e)
        {
            Console.WriteLine("文件重命名事件处理逻辑{0}  {1}  {2}", e.ChangeType, e.FullPath, e.Name);
        }
        #endregion

        #region Public Function
        /// <summary>
        /// 开始监听文件系统
        /// </summary>
        /// <param name="path">监听的路径</param>
        /// <param name="filter">文件类型</param>
        /// <returns></returns>
        public string StartWatcher(string dir, string filter = "")
        {
            LogHelper.WriteMethodLog(true);

            try
            {
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                    //LogHelper.WriteErrorLog("监控文件夹目录不存在。");
                    //throw new MVTMException(ErrorCodes.E22_703, ResourceHelper.GetResourceInfo(ErrorCodes.E22_703));
                }

                this.Filter = filter;
                this.watcher = new FileSystemWatcher();
                this.watcher.Path = dir;
                //if (!string.IsNullOrEmpty(filter)) this.watcher.Filter = filter;
                this.watcher.Created += new FileSystemEventHandler(OnProcess);
                //watcher.Changed += new FileSystemEventHandler(OnProcess);
                //watcher.Deleted += new FileSystemEventHandler(OnProcess);
                //watcher.Renamed += new RenamedEventHandler(OnRenamed);
                this.watcher.EnableRaisingEvents = true;
                this.watcher.NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastAccess
                                       | NotifyFilters.LastWrite | NotifyFilters.Security | NotifyFilters.Size;
                this.watcher.IncludeSubdirectories = false;

                return ConstHelper.CNT_REF_SUCCESS;
            }
            catch (MVTMException mvtmEx)
            {
                LogHelper.WriteErrorLog(mvtmEx.RE.errorCode + ":" + mvtmEx.RE.message);
                throw;
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorCodeLog(ErrorCodes.E22_701, ex);
                throw new MVTMException(ErrorCodes.E22_701);
            }
            finally
            {
                LogHelper.WriteMethodLog(false);
            }
        }
        /// <summary>
        /// 停止监听文件系统
        /// </summary>
        /// <returns></returns>
        public string StopWatcher()
        {
            LogHelper.WriteMethodLog(true);

            try
            {
                if (this.watcher != null)
                {
                    this.watcher.EnableRaisingEvents = false;
                    this.watcher.Dispose();
                }

                return ConstHelper.CNT_REF_SUCCESS;
            }
            catch (MVTMException mvtmEx)
            {
                LogHelper.WriteErrorLog(mvtmEx.RE.errorCode + ":" + mvtmEx.RE.message);
                throw;
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorCodeLog(ErrorCodes.E22_702, ex);
                throw new MVTMException(ErrorCodes.E22_702);
            }
            finally
            {
                LogHelper.WriteMethodLog(false);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Dispose managed resources
                if (watcher != null)
                    watcher.Dispose();
            }

            // Free native resources
        }

 

        #endregion

        #region Private Function

        #endregion
    }

    #region Models
    /// <summary>
    /// OCX执行，返回结果
    /// </summary>
    public class FileWatcherExecuteResultModel
    {
        /// <summary>
        /// 执行结果
        /// </summary>
        public string hResult { get; set; }
        /// <summary>
        /// 返回字符串
        /// </summary>
        public string ReturnMessage { get; set; }
        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath { get; set; }
        /// <summary>
        /// 文件大小
        /// </summary>
        public double FileSize { get; set; }
        /// <summary>
        /// 照片
        /// </summary>
        public string ImageBase64String { get; set; }
        /// <summary>
        /// 错误Code
        /// </summary>
        public string errorCode { get; set; }
        /// <summary>
        /// 错误消息
        /// </summary>
        public string message { get; set; }
    }
    #endregion
}
