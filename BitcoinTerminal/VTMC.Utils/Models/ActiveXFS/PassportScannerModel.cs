namespace VTMC.Utils.Models
{
    /// <summary>
    /// Chip
    /// </summary>
    public class PassportScannerChip
    {
        /// <summary>
        /// 背面照片路径
        /// </summary>
        public string back { get; set; }

        /// <summary>
        /// 背面状态
        /// </summary>
        public string backstatus { get; set; }


        /// <summary>
        /// 正面照片
        /// </summary>
        public string front { get; set; }

        /// <summary>
        /// 正面照片状态
        /// </summary>
        public string frontstatus { get; set; }

        /// <summary>
        /// 芯片状态
        /// </summary>
        public string chipstatus { get; set; }


        /// <summary>
        /// chip node
        /// </summary>
        public PassportScannerModel chip { get; set; }

    }
    /// <summary>
    /// 护照Model
    /// </summary>
    public class PassportScannerModel
    {
        /// <summary>
        /// DOCUMENT_NAME
        /// 证件类型（Unknown Document/Passport/HongKong ID, New ID）
        /// </summary>
        public string DocumentType { get; set; }
        /// <summary>
        /// @Issue_Date
        /// 证件发布日期
        /// </summary>
        public string IssueDate { get; set; }
        /// <summary>
        /// UV_STATUS
        /// 紫外线状态
        /// </summary>
        public string UVStatus { get; set; }
        /// <summary>
        /// MRZ(护照等)证件:签发机关
        /// </summary>
        public string MRZIssuer { get; set; }
        /// <summary>
        /// MRZ(护照等)证件:姓 
        /// </summary>
        public string MRZSurname { get; set; }
        /// <summary>
        /// MRZ(护照等)证件:名 
        /// </summary>
        public string MRZGivname { get; set; }
        /// <summary>
        /// MRZ(护照等)证件:ID 
        /// </summary>
        public string MRZDocumentID { get; set; }
        /// <summary>
        /// MRZ(护照等)证件:国籍
        /// </summary>
        public string MRZNationality { get; set; }
        /// <summary>
        /// MRZ(护照等)证件:生日
        /// </summary>
        public string MRZBirthday { get; set; }
        /// <summary>
        /// MRZ(护照等)证件:性别
        /// </summary>
        public string MRZSex { get; set; }
        /// <summary>
        /// MRZ(护照等)证件:有效日期
        /// </summary>
        public string MRZDateOfExpiry { get; set; }
        /// <summary>
        /// MRZ(护照等)证件:选填信息
        /// </summary>
        public string MRZOptionalData { get; set; }
        /// <summary>
        /// @mrz_line_1
        /// </summary>
        public string MRZLine1 { get; set; }
        /// <summary>
        /// @mrz_line_2
        /// </summary>
        public string MRZLine2 { get; set; }
        /// <summary>
        /// @mrz_line_3
        /// </summary>
        public string MRZLine3 { get; set; }
        /// <summary>
        /// 照片
        /// </summary>
        public string Photo { get; set; }

        #region Wosa Extension

        /// <summary>
        /// 英文姓名
        /// </summary>
        public string MRZFullname { get; set; }

        /// <summary>
        /// 签发地点
        /// </summary>
        public string MRZIssuePlace { get; set; }

        /// <summary>
        /// 出生地点
        /// </summary>
        public string MRZBirthplace { get; set; }

        /// <summary>
        /// 本国姓名
        /// </summary>
        public string NativeName { get; set; }

        /// <summary>
        /// MediaType
        /// </summary>
        public string MediaType { get; set; }

        /// <summary>
        /// 护照号码MRZ
        /// </summary>
        public string MRZDocumentIDMRZ { get; set; }

        #endregion
    }

    /// <summary>
    /// Wosa PassportScanner状态返回数据模型
    /// </summary>
    public class PassportScannerBufferStatusModel
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
