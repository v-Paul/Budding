using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTMC.Utils.Models
{
    /// <summary>
    /// Wosa返回数据模型
    /// </summary>
    public class WOSAHeadModel<T>
    {
        /// <summary>
        /// 执行结果
        /// WFS_SUCCESS:成功
        /// WFS_ERROR:失败
        /// </summary>
        public string hResult { get; set; }
        /// <summary>
        /// 当前命令的命令码
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Code { get; set; }
        /// <summary>
        /// 当前模块的名称
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
        /// <summary>
        /// 错误附加数据/信息
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public T Buffer { get; set; }
    }
}
