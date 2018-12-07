using Renci.SshNet;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace VTMC.Utils
{
    /// <summary>
    /// SFTP通讯类
    /// </summary>
    public class SFTPHelper
    {
        #region Consts
        #endregion

        #region Fileds
        /// <summary>
        /// 回调事件
        /// </summary>
        public Action<ulong> SFtpExecuteCallBack;
        /// <summary>
        /// Sockets通讯监控模块
        /// </summary>
        private SftpClient server;
        #endregion

        #region Property
        #endregion

        #region Public Function

        /// <summary>
        /// 连接FTP服务器
        /// </summary>
        /// <param name="hostName">服务器地址</param>
        /// <param name="portNo">端口号</param>
        /// <param name="userId">用户名</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        public bool Connect(string hostName, int portNo, string userId, string password)
        {
            this.server = new SftpClient(hostName, portNo, userId, password);
            this.server.Connect();

            return true;
        }

        /// <summary>
        /// 文件上传接
        /// </summary>
        /// <param name="path">本地文件路径</param>
        /// <param name="saveDirectory">服务器保存路径</param>
        /// <param name="fileName">上传文件名</param>
        /// <returns></returns>
        public bool UploadFile(string path,string saveDirectory, string fileName)
        {
            this.server.ChangeDirectory(saveDirectory);
            using (var file = File.OpenRead(path))
            {
                this.server.BufferSize = 100 * 1024;
                this.server.UploadFile(file, fileName, true, UploadCallBack);
            }

            return true;
        }

        /// <summary>
        /// 获取是否连接上服务器
        /// </summary>
        /// <returns></returns>
        public bool GetIsConnected()
        {
            if(this.server == null)
            {
                return false;
            }
            else
            {
                return this.server.IsConnected;
            }
        }

        /// <summary>
        /// 上传字节回调
        /// </summary>
        /// <param name="len">文件上传完成字节数</param>
        private void UploadCallBack(ulong len)
        {
            this.SFtpExecuteCallBack(len);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            LogHelper.WriteMethodLog(true);
            if (this.server != null)
            {
                this.server.Disconnect();
                this.server.Dispose();
                this.server = null;
            }
            LogHelper.WriteMethodLog(false);
        }
        #endregion
    }
}
