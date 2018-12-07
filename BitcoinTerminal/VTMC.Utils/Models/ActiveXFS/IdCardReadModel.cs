namespace VTMC.Utils.Models
{

    /// <summary>
    /// WosaIdCardRead返回数据模型
    /// </summary>
    public class IdCardReadBufferMKModel
    {
       public string back { get; set; }
        public string backstatus { get; set; }
        public IdCardReadBufferChipMKModel chip { get; set; }
        public string chipstatus { get; set; }
        public string front { get; set; }
        public string frontstatus { get; set; }
    }

    public class IdCardReadBufferChipMKModel
    {
        public int CardType { get; set; }
        public string PhotoFileName { get; set; }
        public string LeftFinger { get; set; }
        public string RightFinger { get; set; }
        public string MKAddress1 { get; set; }
        public string MKAddress2 { get; set; }
        public string MKAddress3 { get; set; }
        public string MKBirthDate { get; set; }
        public string MKBirthPlace { get; set; }
        public string MKCitizenship { get; set; }
        public string MKCity { get; set; }
        public string MKGender { get; set; }
        public string MKIDNumber { get; set; }
        public string MKName { get; set; }
        public string MKOldIDNumber { get; set; }
        public string MKOriName { get; set; }
        public string MKOtherID { get; set; }
        public string MKOtherIDType { get; set; }
        public string MKPostcode { get; set; }
        public string MKRace { get; set; }
        public string MKReligion { get; set; }
        public string MKShortName { get; set; }
        public string MKState { get; set; }
        public string MKVersion { get; set; }
        public string ReaderName { get; set; }
    }

    /// <summary>
    /// WosaIdCardRead状态返回数据模型
    /// </summary>
    public class IdCardReadBufferStatusModel
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
    }
}
