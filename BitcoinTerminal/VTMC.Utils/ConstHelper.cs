/*************************************************
*Author:zhangdanhong
*Date:2016/12/6 18:48:37
*Des:常量
************************************************/

namespace VTMC.Utils
{
    /// <summary>
    /// 系统常量定义
    /// </summary>
    public static class ConstHelper
    {

        //bitcoin const

        public const string BC_OK = "OK";
        public const string BC_LastKey = "LastKey";
        public const string BC_All = "All";
        public const string BC_InputAmount = "InputAmount";
        public const string BC_BaseCoinInputTxHash = "0000000000000000000000000000000000000000000000000000000000000000";
        public const string BC_RequestHandshake = "星宿老仙，法力无边";
        public const string BC_ReturnHandshake = "神通广大，法驾中原";
        public const string BC_DBZipName = "DBZipName.zip";
        /// <summary>
        ///  ZIP文件扩展名[.zip]
        /// </summary>
        public const string ZIPExtension = ".zip";
        /// <summary>
        /// token key
        /// </summary>
        public const string CNT_TOKEN_KEY = "Authorization";
        /// <summary>
        /// 超时时间
        /// </summary>
        public const uint CNT_TIME_OUT = 30000;
        ///// <summary>
        ///// Token 取得字符
        ///// </summary>
        //public const string CNT_VTM_TOKEN_KEY = "VTM-TOKEN-KEY";
        /// <summary>
        /// 系统名称
        /// </summary>
        public const string ProjectName = "MMK";
        /// <summary>
        /// 执行成功返回字符内容
        /// </summary>
        public const string CNT_SUCCESS = "WFS_SUCCESS";
        /// <summary>
        /// 执行错误自定义内容
        /// </summary>
        public const string CNT_ERROR = "WFS_ERROR";
        /// <summary>
        /// 执行取消自定义内容
        /// </summary>
        public const string CNT_CANCEL = "WFS_CANCEL";
        /// <summary>
        /// Wosa执行正常
        /// </summary>
        public const string CNT_OK = "ok";
        /// <summary>
        /// Wosa执行正常
        /// </summary>
        public const string CNT_MISSING = "missing";
        /// <summary>
        /// 返回给前端正常时的返回值
        /// </summary>
        public const string CNT_REF_SUCCESS = "{\"hResult\":\"" + ConstHelper.CNT_SUCCESS + "\"," + "\"data\":null," + "\"errorCode\":null," + "\"message\":null" + "}";

        /// <summary>
        /// 页面加载正确时title
        /// </summary>
        public const string VTMTitle = "VTM-PRD";

        /// <summary>
        /// 页面加载失败时title
        /// </summary>
        public const string NavigateCancel = "Navigation Canceled";
        /// <summary>
        /// 页面加载失败时title(ZH-CN)
        /// </summary>
        public const string NavigateCancelZHCN = @"导航已取消";

        #region 异常Code常量

        /// <summary>
        /// 错误码
        /// </summary>
        public const string CNT_errorCode = "errorCode";

        /// <summary>
        /// 异常信息
        /// </summary>
        public const string CNT_message = "message";
        #endregion

        #region 文件扩展名
        /// <summary>
        /// PDF文件扩展名[.pdf]
        /// </summary>
        public const string PDFExtension = ".pdf";

        /// <summary>
        /// JPG文件扩展名[.jpg]
        /// </summary>
        public const string JPGExtension = ".jpg";

        /// <summary>
        /// PNG文件扩展名[.png]
        /// </summary>
        public const string PNGExtension = ".png";

        /// <summary>
        /// TIF文件扩展名[.tif]
        /// </summary>
        public const string TIFExtension = ".tif";

        /// <summary>
        /// PDF版本文件扩展名[.ver]
        /// </summary>
        public const string PDFVerExtension = ".ver";

        /// <summary>
        /// Json文件扩展名[.json]
        /// </summary>
        public const string JSONExtension = ".json";

        /// <summary>
        /// MP4文件扩展名[.mp4]
        /// </summary>
        public const string MP4Extension = ".mp4";
        /// <summary>
        /// XML文件扩展名[.xml]
        /// </summary>
        public const string XMLExtension = ".xml";

        /// <summary>
        /// log文件扩展名[.log]
        /// </summary>
        public const string LogExtension = ".log";
        #endregion

        #region Dir

        /// <summary>
        /// 证书目录
        /// </summary>
        public const string CERTIFICATE_DIR = @"./Certificate/";
        /// <summary>
        /// MYKAD 大马卡图片、模板报错目录
        /// </summary>
        public const string ThumbPath = "C:\\MYKAD\\";
        /// <summary>
        /// Wosa设定文件地址
        /// </summary>
        public const string WosaSettingFilePath = @"./WosaSetting/WosaSetting.xml";
        #endregion

        #region 文件名

        /// <summary>
        /// 业务Json保存文件名
        /// </summary>
        public const string BusinessJsonName = "BusInfo.json";
        /// <summary>
        /// 业务办理中生成的文件的保存文件夹名称
        /// </summary>
        public const string BusinessFilesDirName = "PersonalFile";
        /// <summary>
        /// SMP生成PDF标致文件
        /// </summary>
        public const string ReadyPDFmvtm = "readyPDF.mvtm";

        /// <summary>
        /// 上传是PDF文件名
        /// </summary>
        public const string UploadSMPPDFName = "SmartForm_signed";

        /// <summary>
        /// 上传是PDF签名图片名字
        /// </summary>
        public const string UploadSMPPDFSignName = "customer_signature.tif";

        #endregion

        #region VideoRecord启动arg

        /// <summary>
        /// VideoRecord启动arg
        /// </summary>
        public const string VideoRecordArg = @"StartFromMVTM";

        /// <summary>
        /// Socket监控Timer时间间隔(s)
        /// </summary>
        public const int SocketMonitorTimerInterval = 5;

        /// <summary>
        /// RecordTool.exe名称
        /// </summary>
        public const string RecordToolName = @"VideoRecordTool";

        /// <summary>
        /// 不换行空格
        /// </summary>
        public const string NoBreakSpace = @"&nbsp;";
        #endregion

        #region Logger的name

        /// <summary>
        /// MVTMClient日志的logger name
        /// </summary>
        public const string MVTMClientLoggerName = @"log4net";

        /// <summary>
        /// 前端日志的logger name
        /// </summary>
        public const string FSLoggerName = @"fslog4net";

        /// <summary>
        /// RecordTool.exe的logger name
        /// </summary>
        public const string RecordToolLoggerName = @"recordtoollog4net";
        #endregion

        #region MYKAD Name
        /// <summary>
        /// MYKAD Name，仅供保持与wosa其他模块结构一致
        /// </summary>
        public const string MYKADReaderName = "MYKADReader";
        #endregion
    }
}
