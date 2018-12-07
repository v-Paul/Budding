using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace VTMC.Utils.Models
{
    /// <summary>
    /// Wosa设备运行参数模型
    /// </summary>
    [XmlRoot("WosaSetting")]
    public class WOSADeviceSettingModel
    {
        /// <summary>
        /// 设备运行参数列表
        /// </summary>
        [XmlArrayItem("Device")]
        public List<DeviceSettingModel> Devices { get; set; }
    }

    /// <summary>
    /// Wosa设备参数模型
    /// </summary>
    public class DeviceSettingModel
    {
        /// <summary>
        /// 设备名称
        /// </summary>
        [XmlAttribute("Name")]
        public string Name { get; set; }
        /// <summary>
        /// 硬件模块名
        /// </summary>
        public string Hardware { get; set; }
        /// <summary>
        /// 是否启用(0:不启用 1:启用)
        /// </summary>
        public int Enabled { get; set; }
        /// <summary>
        /// 逻辑名称
        /// </summary>
        public string LogicName { get; set; }
        /// <summary>
        /// IP地址
        /// </summary>
        public string IP { get; set; }
        /// <summary>
        /// 端口号
        /// </summary>
        public string PortNo { get; set; }
        /// <summary>
        /// 比特率
        /// </summary>
        public string Baud { get; set; }
        /// <summary>
        /// 摄像头分辨率
        /// </summary>
        public double Resolution { get; set; }
        /// <summary>
        /// 护照扫描仪灯设置(扫描仪专用)
        /// </summary>
        public string ScannerLights { get; set; }
        /// <summary>
        /// 摄像头向下偏移量从屏幕中间开始偏移(摄像头专用)
        /// </summary>
        public int TopDif { get; set; }
        /// <summary>
        /// 摄像头是否自动拍照时间(0为不自动)
        /// </summary>
        public int AutoTakingTime { get; set; }

        /// <summary>
        /// 音频播放模式
        /// once:播放一次
        /// 10:播放指定次数
        /// loop:循环播放
        /// </summary>
        public string AudioPlayMode { get; set; }

        /// <summary>
        /// 音频文件路径
        /// </summary>
        public string AudioFilePath { get; set; }
    }
}
