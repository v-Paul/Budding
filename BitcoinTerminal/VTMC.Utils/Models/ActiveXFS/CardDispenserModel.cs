using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTMC.Utils.Models.ActiveXFS
{
    /// <summary>
    /// Status buffer information
    /// </summary>
    public class CRDStatusBufferModel
    {
        /// <summary>
        /// WFS_STAT_DEVOFFLINE
        /// </summary>
        public string fwDevice { get; set; }

        /// <summary>
        /// WFS_CRD_DEVICEINPOSITION
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string wDevicePosition { get; set; }

        /// <summary>
        /// WFS_CRD_DISPCUOK
        /// </summary>
        public string fwDispenser { get; set; }

        /// <summary>
        /// WFS_CRD_MEDIANOTPRESENT
        /// </summary>
        public string fwMedia { get; set; }

        /// <summary>
        /// 0
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string usPowerSaveRecoveryTime { get; set; }

        /// <summary>
        /// WFS_CRD_SHTCLOSED
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string fwShutter { get; set; }

        /// <summary>
        /// WFS_CRD_SHTCLOSED
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string fwTransport { get; set; }

        /// <summary>
        /// 扩展
        /// </summary>
        public string lpszExtra { get; set; }

    }


    /// <summary>
    /// 卡箱单元
    /// </summary>
    [Serializable]
    public class CardUnit
    {
        public CardUnit()
        {

        }
        public CardUnit(CardUnit CU)
        {
            this.bHardwareSensor = CU.bHardwareSensor;
            this.lpszCardName = CU.lpszCardName;
            this.ulCount = CU.ulCount;
            this.ulInitialCount = CU.ulInitialCount;
            this.ulRetainCount = CU.ulRetainCount;
            this.ulThreshold = CU.ulThreshold;
            this.usNumber = CU.usNumber;
            this.usStatus = CU.usStatus;
            this.usType = CU.usType;
        }


        /// <summary>
        /// 是否有硬件传感器
        /// </summary>
        public int bHardwareSensor { get; set; }
        /// <summary>
        /// 卡箱名
        /// </summary>
        public string lpszCardName { get; set; }
        /// <summary>
        /// 当前计数
        /// </summary>
        public int ulCount { get; set; }
        /// <summary>
        /// 初始计数
        /// </summary>
        public int ulInitialCount { get; set; }
        /// <summary>
        /// 回收计数
        /// </summary>
        public int ulRetainCount { get; set; }
        /// <summary>
        /// 阀值
        /// </summary>
        public int ulThreshold { get; set; }
        /// <summary>
        /// 卡箱编号
        /// </summary>
        public int usNumber { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string usStatus { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string usType { get; set; }  
    }

    /// <summary>
    /// 卡箱数据
    /// </summary>
    [Serializable]
    public class CardUnitsInfo
    {
        List<CardUnit> innerInfoList = new List<CardUnit>();
        /// <summary>
        /// 卡箱信息列表
        /// </summary>
        public  List<CardUnit> InfoList
        {
            get { return innerInfoList; }
            set { innerInfoList = value; }
        }
        /// <summary>
        /// 卡箱数
        /// </summary>
        public int usCount { get; set; }
    }
    [Serializable]
    public class CardUnitStatus
    {
        public string strStack1 {get; set;}
        public string strStack2 { get; set; }
        public string strStack3 { get; set; }
        public string strStack4 { get; set; }
    }

}
