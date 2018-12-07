using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTMC.Utils.Models
{
    /// <summary>
    /// 保存到每个User目录下客户端数据
    /// </summary>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public class UserConfiguration
    {
        /// <summary>
        /// 前端JS需要保存到客户端的信息
        /// </summary>
        public JSInformation JSInformation = new JSInformation();
    }

    /// <summary>
    /// 前端JS需要保存到客户端的信息
    /// </summary>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public class JSInformation
    {
        /// <summary>
        /// 当前加载的JS版本
        /// </summary>
        public string JSVersion = string.Empty;
    }
}
