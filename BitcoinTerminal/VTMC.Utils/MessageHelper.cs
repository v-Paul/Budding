using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Media;

namespace VTMC.Utils
{
    /// <summary>
    /// 系统消息处理
    /// </summary>
    public static class MessageHelper
    {
        #region 构造函数

        #endregion

        #region 信息
        /// <summary>
        /// 空消息内容
        /// </summary>
        public static MessageInfo Info_001 = Factory(MessageInfoType.InformationMessageInfo, ErrorCodes.E24_9101);

        #endregion

        #region 警告
        /// <summary>
        /// 空消息内容
        /// </summary>
        public static MessageInfo Warn_001 = Factory(MessageInfoType.WarnMessageInfo, ErrorCodes.E23_9201);

        #endregion

        #region 错误
        /// <summary>
        /// 空消息内容
        /// </summary>
        public static MessageInfo Error_001 = Factory(MessageInfoType.ErrorMessageInfo, ErrorCodes.E21_9301);


        /// <summary>
        /// Web服务器连接失败，请检查服务器链接地址或网络环境。
        /// </summary>
        public static MessageInfo Error_997 = Factory(MessageInfoType.ErrorMessageInfo, ErrorCodes.E21_9397);
        #endregion

        #region 询问
        /// <summary>
        /// 空消息内容
        /// </summary>
        public static MessageInfo Question_001 = Factory(MessageInfoType.QuestionMessageInfo, ErrorCodes.E24_9401);

        #endregion

        #region TimeOut
        /// <summary>
        /// 回话即将超时，请问您是否需要继续？
        /// </summary>
        //public static MessageInfo Timeout_001 = Factory(MessageInfoType.TimeOutMessageInfo, string.Empty);

        #endregion

        #region Retry
        /// <summary>
        /// AD Check失败。
        /// </summary>
        //public static MessageInfo Retry_001 = Factory(MessageInfoType.RetryMessageInfo, ErrorCodes.E23_9501);

        #endregion

        /// <summary>
        /// 消息工厂
        /// </summary>
        /// <param name="msgType">消息类型</param>
        /// <param name="strMsgCode">消息Code</param>
        /// <param name="msgIcon">询问消息类型默认为Yes/N0</param>
        /// <returns></returns>
        public static MessageInfo Factory(MessageInfoType msgType, string strMsgCode, MessageBoxType msgIcon = MessageBoxType.QYesAndNo)
        {
            MessageInfo msg;
            switch (msgType)
            {
                case MessageInfoType.InformationMessageInfo:
                    msg = new InformationMessageInfo(strMsgCode);
                    break;
                case MessageInfoType.ErrorMessageInfo:
                    msg = new ErrorMessageInfo(strMsgCode);
                    break;
                case MessageInfoType.WarnMessageInfo:
                    msg = new WarnMessageInfo(strMsgCode);
                    break;
                case MessageInfoType.QuestionMessageInfo:
                    msg = new QuestionMessageInfo(msgIcon, strMsgCode);
                    break;
                case MessageInfoType.TimeOutMessageInfo:
                    msg = new TimeOutMessageInfo(strMsgCode);
                    break;
                case MessageInfoType.RetryMessageInfo:
                    msg = new RetryMessageInfo(strMsgCode);
                    break;
                default:
                    msg = new InformationMessageInfo(strMsgCode);
                    break;
            }
            return msg;
        }
    }

    /// <summary>
    /// Message类型
    /// </summary>
    public enum MessageInfoType
    {
        InformationMessageInfo,
        ErrorMessageInfo,
        WarnMessageInfo,
        QuestionMessageInfo,
        TimeOutMessageInfo,
        RetryMessageInfo
    }

    /// <summary>
    /// 信息消息
    /// </summary>
    internal class InformationMessageInfo : MessageInfo
    {
        public InformationMessageInfo(string strMsgCode)
        {
            this.MsgCode = strMsgCode;
            this.MsgButton = MessageBoxButton.OK;
            this.MsgIcon = MessageBoxType.Information;
        }
    }
    /// <summary>
    /// 错误消息
    /// </summary>
    internal class ErrorMessageInfo : MessageInfo
    {
        public ErrorMessageInfo(string strMsgCode)
        {
            this.MsgCode = strMsgCode;
            this.MsgButton = MessageBoxButton.OK;
            this.MsgIcon = MessageBoxType.Error;
        }
    }
    /// <summary>
    /// 警告消息
    /// </summary>
    internal class WarnMessageInfo : MessageInfo
    {
        public WarnMessageInfo(string strMsgCode)
        {
            this.MsgCode = strMsgCode;
            this.MsgButton = MessageBoxButton.OK;
            this.MsgIcon = MessageBoxType.Warning;
        }
    }
    /// <summary>
    /// 问题消息
    /// </summary>
    internal class QuestionMessageInfo : MessageInfo
    {
        public QuestionMessageInfo(MessageBoxType msgIcon, string strMsgCode)
        {
            this.MsgCode = strMsgCode;
            this.MsgButton = MessageBoxButton.YesNo;
            this.MsgIcon = msgIcon;
        }
    }
    /// <summary>
    /// 超时消息
    /// </summary>
    internal class TimeOutMessageInfo : MessageInfo
    {
        public TimeOutMessageInfo(string strMsgCode)
        {
            this.MsgCode = strMsgCode;
            this.MsgButton = MessageBoxButton.OKCancel;
            this.MsgIcon = MessageBoxType.QTimeOut;
        }
    }
    /// <summary>
    /// 重试消息
    /// </summary>
    internal class RetryMessageInfo : MessageInfo
    {
        public RetryMessageInfo(string strMsgCode)
        {
            this.MsgCode = strMsgCode;
            this.MsgButton = MessageBoxButton.OKCancel;
            this.MsgIcon = MessageBoxType.QRetryAndCancel;
        }
    }

    /// <summary>
    /// 消息Base类
    /// </summary>
    public class MessageInfo
    {
        public string MsgCode { get; set; }
        public MessageBoxType MsgIcon { get; set; }
        public MessageBoxButton MsgButton { get; set; }
        public string MsgInfo { get { return ResourceHelper.GetResourceInfo(this.MsgCode); } }

        /// <summary>
        /// 弹出消息提示框
        /// </summary>
        /// <returns></returns>
        public MessageBoxResult Show()
        {
            return Show(this.MsgInfo);
        }

        /// <summary>
        /// 弹出消息提示框
        /// </summary>
        /// <param name="strMessage"></param>
        /// <returns></returns>
        public MessageBoxResult Show(string strMessage)
        {
            MvMessageBox mvMsgBox = new MvMessageBox(this.MsgCode, strMessage, ConstHelper.ProjectName, this.MsgButton, this.MsgIcon);
            mvMsgBox.ShowDialog();
            return mvMsgBox.MsgResult;
        }

        /// <summary>
        /// 弹出消息提示框
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public MessageBoxResult Show(object[] args)
        {
            string strMessage = string.Format(this.MsgInfo, args);
            return Show(strMessage);
        }

        /// <summary>
        /// 弹出PDF超时Message
        /// </summary>
        /// <param name="strMessage"></param>
        /// <param name="time"></param>
        public MvMessageBox ShowEx(string strMessage, int time)
        {
            MvMessageBox mvMsgBox = new MvMessageBox(this.MsgCode, strMessage, ConstHelper.ProjectName, this.MsgButton, this.MsgIcon);
            mvMsgBox.Countdown = time;
            mvMsgBox.Show();
            return mvMsgBox;
        }

    }

}
