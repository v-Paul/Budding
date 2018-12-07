using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace VTMC.Utils.Models
{
    /// <summary>
    /// Http访问模型
    /// </summary>
    public class HttpRequestModel
    {
        /// <summary>
        /// Http访问方式
        /// </summary>
        public HttpMethod Method { get; set; }
        /// <summary>
        /// 访问Url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Http访问Header信息
        /// </summary>
        public Dictionary<string, string> Headers { get; set; }
        /// <summary>
        /// ContentType
        /// </summary>
        public string ContentType { get; set; } = "application/json";
        /// <summary>
        /// Http的Body内容
        /// </summary>
        public string BodyContent { get; set; }
    }

    /// <summary>
    /// Http访问返回结果模型
    /// </summary>
    public class HttpResponseModel
    {
        /// <summary>
        /// Http返回状态Code
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }
        /// <summary>
        /// Response返回内容
        /// </summary>
        public string HttpResponseInfo { get; set; }
    }

    /// <summary>
    /// Http上传模型
    /// </summary>
    public class HttpUploadModel : HttpRequestModel
    {
        public List<HttpUploadFileModel> Files { get; set; }
    }

    /// <summary>
    /// 上传文件信息
    /// </summary>
    [Serializable]
    public class HttpUploadFileModel
    {
        /// <summary>
        /// 文件名字
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 文件路径
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// 文件附件说明
        /// </summary>
        public Dictionary<string, string> Attachments { get; set; }
    }

    /// <summary>
    /// 上传文件信息
    /// </summary>
    [Serializable]
    public class HttpDownloadModel : HttpRequestModel
    {
        /// <summary>
        /// 文件名字
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 文件路径(带文件名)
        /// </summary>
        public string Path { get; set; }
    }
}
