using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTMC.Utils.Models
{
    /// <summary>
    /// 文件上传状态
    /// </summary>
    public class SFTPUploadStatus
    {
        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 文件总数
        /// </summary>
        public int FilesCount { get; set; }
        /// <summary>
        /// 等待上传件数
        /// </summary>
        public int WaitCount { get; set; }
        /// <summary>
        /// 上传错误件数
        /// </summary>
        public int ErrorCount { get; set; }
        /// <summary>
        /// 上传完成件数
        /// </summary>
        public int UploadCount { get; set; }
        /// <summary>
        /// 文件损坏件数
        /// </summary>
        public int FileDamageCount { get; set; }
    }

    /// <summary>
    /// 上传文件信息列表模型
    /// </summary>
    public class SFTPUploadFilesModel
    {
        /// <summary>
        /// 文件列表
        /// </summary>
        public ConcurrentBag<UploadFileInfo> files { get; set; }
    }

    /// <summary>
    /// 上传文件信息
    /// </summary>
    public class UploadFileInfo
    {
        /// <summary>
        /// 上传优先度
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 视频文件名字
        /// </summary>
        public string VideoName { get; set; }
        /// <summary>
        /// Json文件名字
        /// </summary>
        public string JsonName { get; set; }
        /// <summary>
        /// 文件路径(不含文件名)
        /// </summary>
        public string Directory { get; set; }
        /// <summary>
        /// 上传状态
        /// </summary>
        public UploadFileStats Status { get; set; }
        /// <summary>
        /// 文件大小 字节数
        /// </summary>
        public long Size { get; set; }
        /// <summary>
        /// 已上传大小 字节数
        /// </summary>
        public long UpLoadSize { get; set; }
        /// <summary>
        /// 错误内容
        /// </summary>
        public string ErrorInfo { get; set; }
        /// <summary>
        /// 错误次数
        /// </summary>
        public int ErrorQty { get; set; }

        /// <summary>
        /// 身份号码
        /// </summary>
        public string IdCardNo { get; set; }
    }

    /// <summary>
    /// 录屏视频文件状态
    /// </summary>
    public enum UploadFileStats
    {
        /// <summary>
        /// 未开始
        /// </summary>
        None,
        /// <summary>
        /// 上传中
        /// </summary>
        Uploading,
        /// <summary>
        /// 上传完成
        /// </summary>
        UploadComplete,
        /// <summary>
        /// 文件损坏
        /// </summary>
        FileDamage,
        /// <summary>
        /// 上传出错
        /// </summary>
        Error,
    }
}
