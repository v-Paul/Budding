using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTMC.Utils.Models
{
    /// <summary>
    /// 保存到开始程序的配置
    /// </summary>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public class StartupConfiguration
    {
        /// <summary>
        /// 进程启动信息列表
        /// </summary>
        public List<ProcessStartInfo> ProcessList;
    }

    /// <summary>
    /// 保存到每个User目录下客户端数据
    /// </summary>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public class CommonConfiguration
    {
        /// <summary>
        /// 进程启动信息列表
        /// </summary>
        public List<ProcessStartInfo> ProcessList;
    }

    /// <summary>
    /// 进程启动信息
    /// </summary>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public class ProcessStartInfo
    {
        /// <summary>
        /// 当前加载的JS版本
        /// </summary>
        public string RunningMethod = string.Empty;
        /// <summary>
        /// 当前加载的JS版本
        /// </summary>
        public string FileName = string.Empty;
        /// <summary>
        /// 当前加载的JS版本
        /// </summary>
        public bool WaitForExit = false;
        /// <summary>
        /// 进程启动参数
        /// </summary>
        public string Args = string.Empty;
        /// <summary>
        /// 进程工作目录
        /// </summary>
        public string WorkingDir = string.Empty;
        /// <summary>
        /// 启动前是否Kill进程0/1
        /// </summary>
        public string KillProcessBeforeStart = "0";
    }
}
