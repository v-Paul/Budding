using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace VTMC.Utils
{



    /// <summary>
    /// Termina Info Json强类型
    /// </summary>
    [Serializable]
    public class TerminalInfo
    {     
        /// <summary>
        /// 计算机型号
        /// </summary>
        public string ComputerProductName { get; set; }
        /// <summary>
        /// 系统用户名
        /// </summary>
        public string HostName { get; set; }
        /// <summary>
        /// IP 地址列表
        /// </summary>
        public List<string> lisIPAdress { get; set; }
        /// <summary>
        /// MAC地址列表
        /// </summary>
        public List<string> lisMACAdress { get; set; }
        /// <summary>
        /// 操作系统位数
        /// </summary>
        public string OSBit { get; set; }
        /// <summary>
        /// 操作系统版本
        /// </summary>
        public string OSVersion { get; set; }
        /// <summary>
        /// MVTM产品名称
        /// </summary>
        public string MVTMProductName { get; set; }
        /// <summary>
        /// MVTM产品版本
        /// </summary>
        public string MVTMVersion { get; set; }
        /// <summary>
        /// MVTMTitle
        /// </summary>
        [JsonIgnore]
        public string MVTMTitle { get; set; }


    }


}
