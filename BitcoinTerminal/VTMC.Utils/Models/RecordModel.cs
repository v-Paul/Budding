using Newtonsoft.Json;
using System;

namespace VTMC.Utils
{
    /// <summary>
    /// 录屏状态
    /// </summary>
    public enum RecordStatus
    {
        /// <summary>
        /// 未启动
        /// </summary>
        NoOpen,
        /// <summary>
        /// 已启动未录制
        /// </summary>
        NotRecording,
        /// <summary>
        /// 录制中
        /// </summary>
        Recording,
        /// <summary>
        /// 暂停
        /// </summary>
        Pause,
        /// <summary>
        /// 视频生成中
        /// </summary>
        GeneratingVideo,
        /// <summary>
        /// 已经错误
        /// </summary>
        Error,
    }
    public static class RecordExecutiveCommand
    {
        public const string Open = "Open";
        public const string Start = "Start";
        public const string Pause = "Pause";
        public const string Continue = "Continue";
        public const string Stop = "Stop";
        public const string GetStatus = "GetStatus";
        public const string Close = "Close";
        public const string AddVideoStatus = "AddVideoStatus";

    }

    public static class RecordEvent
    {
        public const string StatusEvent = "StatusEvent";
        public const string ErrorEvent = "ErrorEvent";

        public const string StartComplete = "StartComplete";
        public const string StopComplete = "StopComplete";
        public const string OpenComplete = "OpenComplete";
    }

    /// <summary>
    /// 视频录制保存Json强类型
    /// </summary>
    [Serializable]
    public class RecordVideoInfor
    {
        /// <summary>
        /// 身份证号
        /// </summary>
        public string IDCardNumber { get; set; }
        /// <summary>
        /// 业务类型
        /// </summary>
        public string BusinessType { get; set; }
        /// <summary>
        /// 柜员编号
        /// </summary>
        public string StaffID { get; set; }
        /// <summary>
        /// MP4文件名称
        /// </summary>
        public string VideoFileName { get; set; }
        /// <summary>
        /// 视频时长
        /// </summary>
        public string Elapsed { get; set; }
        /// <summary>
        /// 录制开始时间
        /// </summary>
        public string StartRecordTime { get; set; }
        /// <summary>
        /// 其他数据
        /// </summary>
        public string Data { get; set; }
        /// <summary>
        /// 录屏文件状态
        /// </summary>
        public string VideoFileStats { get; set; }

        #region Json写入的时候需忽略字段
        /// <summary>
        /// MP4文件全路径
        /// </summary>
        [JsonIgnore]
        public string VideoFileFullPath { get; set; }
        /// <summary>
        /// Json文件名称
        /// </summary>
        [JsonIgnore]
        public string JsonFileFullPath { get; set; }
        #endregion
    }
    /// <summary>
    /// 录屏视频文件状态
    /// </summary>
    public enum VideoFileStats
    {
        /// <summary>
        /// 录屏文件状态位置
        /// </summary>
        Unknow,
       /// <summary>
       /// 录屏文件不可用
       /// </summary>
        Invalid,
        /// <summary>
        /// 录屏文件状态正常
        /// </summary>
        OK,
    }
}
