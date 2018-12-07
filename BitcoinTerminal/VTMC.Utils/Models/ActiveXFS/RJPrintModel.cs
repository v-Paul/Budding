namespace VTMC.Utils.Models
{
    /// <summary>
    /// WosaRJPrintModel返回数据模型
    /// </summary>
    public class RJPrintBufferModel
    {
    }

    /// <summary>
    /// WosaRJPrintModel状态返回数据模型
    /// </summary>
    public class RJPrintBufferStatusModel
    {
        /// <summary>
        /// Extra
        /// </summary>
        public string Extra { get; set; }
        /// <summary>
        /// fwDevice
        /// </summary>
        public string fwDevice { get; set; }
        /// <summary>
        /// fwMedia
        /// </summary>
        public string fwMedia { get; set; }
        /// <summary>
        /// ChipPower
        /// </summary>
        public string fwPaper { get; set; }
    }
}
