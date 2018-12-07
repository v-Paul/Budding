using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTMC.Utils.Models
{
    /// <summary>
    /// 文件模型
    /// </summary>
    public class FileModel
    {
        /// <summary>
        /// 文件列表
        /// </summary>
        public List<FileInfo> files { get; set; }
    }

    /// <summary>
    /// 文件信息
    /// </summary>
    [Serializable]
    public class FileInfo
    {
        /// <summary>
        /// 文件名字
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 文件路径
        /// </summary>
        public string path { get; set; }

        /// <summary>
        /// base64字符串
        /// </summary>
        [JsonIgnore]
        public string base64String { get; set; }

        /// <summary>
        /// 文件大小（bytes）
        /// </summary>
        [JsonIgnore]
        public long length { get; set; }

        /// <summary>
        /// 视频文件录制时长
        /// </summary>
        [JsonIgnore]
        public string duration { get; set; }

        /// <summary>
        /// 文件描述
        /// </summary>
        [JsonIgnore]
        public string description { get; set; }

        /// <summary>
        /// 文件业务类型
        /// </summary>
        public string fileType { get; set; }
    }
}
