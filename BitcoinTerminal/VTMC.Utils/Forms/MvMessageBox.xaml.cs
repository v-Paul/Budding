using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace VTMC.Utils
{
    /// <summary>
    /// MessageBox Class
    /// </summary>
    public partial class MvMessageBox : Window, IDisposable
    {
        #region 常量


        #endregion

        #region 变量
        /// <summary>
        /// PDF超时倒计时器
        /// </summary>
        private Timer timer = null;

        private string Status = "BackHome";
        /// <summary>
        /// 回调传状态给(BackHome/Continue)Bridge
        /// </summary>
        public Action<string> msgCallBack;

        #endregion

        #region 属性
        public string Caption
        {
            get { return this.Title; }
            set { this.Title = value; }
        }

        public string Message
        {
            get { return this.lblMessage.Text; }
            set { this.lblMessage.Text = value; }
        }

        public string ErrorCode
        {
            get;
            internal set;
        }

        public MessageBoxButton MsgButton { set; get; }

        public MessageBoxType MsgIcon { set; get; }

        public MessageBoxResult MsgResult
        {
            internal set;
            get;
        }

        /// <summary>
        /// 倒计时秒数
        /// </summary>
        public int Countdown { get; set; }

        #endregion

        #region 构造函数

        /// <summary>
        /// 消息构造函数
        /// </summary>
        /// <param name="owner">父窗口</param>
        /// <param name="errorcode">错误code</param>
        /// <param name="strmessage">错误message</param>
        /// <param name="caption">标题</param>
        /// <param name="button">按钮类型</param>
        /// <param name="icon"></param>
        public MvMessageBox(Window owner, string errorcode, string strmessage, string caption, MessageBoxButton button, MessageBoxType icon)
        {
            InitializeComponent();
            this.Caption = caption;
            this.ErrorCode = errorcode;
            this.Message = strmessage;
            this.MsgButton = button;
            this.MsgIcon = icon;
            if (owner != null)
            {
                this.Owner = owner;
                if (!string.IsNullOrEmpty(errorcode))
                    owner.Tag = this;
            }
            SetUIText();
            SetIcon();
            SetMessageType();
        }

        #endregion

        #region 事件

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.MsgIcon == MessageBoxType.QTimeOut)
                {
                    timer = new Timer();
                    timer.Interval = 1000;
                    timer.Tick += Timer_Tick;
                    this.btnCancel.Content = ResourceHelper.GetResourceInfo("M_Button_BackHome") + "(" + this.Countdown + ")";
                    timer.Start();
                }

                AppSettings.messageform = this;
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorInfoLog(ex.Message, ex);
                throw;
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.MsgResult = MessageBoxResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorInfoLog(ex.Message, ex);
                throw;
            }

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.MsgIcon == MessageBoxType.QYesAndNo)
                {
                    this.MsgResult = MessageBoxResult.No;
                }
                else if (this.MsgIcon == MessageBoxType.QCancelAndOk)
                {
                    this.MsgResult = MessageBoxResult.Cancel;
                }
                else if (this.MsgIcon == MessageBoxType.QCancelAndAmend)
                {
                    this.MsgResult = MessageBoxResult.No;
                }
                else
                {
                    this.MsgResult = MessageBoxResult.Cancel;
                }
                if (this.MsgIcon == MessageBoxType.QTimeOut)
                {
                    Status = "BackHome";
                }
                this.Close();
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorInfoLog(ex.Message, ex);
                throw;
            }
        }

        private void btnCancelOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.MsgIcon == MessageBoxType.QYesAndNo)
                {
                    this.MsgResult = MessageBoxResult.Yes;
                }
                else if (this.MsgIcon == MessageBoxType.QCancelAndOk)
                {
                    this.MsgResult = MessageBoxResult.OK;
                }
                else if (this.MsgIcon == MessageBoxType.QCancelAndAmend)
                {
                    this.MsgResult = MessageBoxResult.Yes;
                }
                else
                {
                    this.MsgResult = MessageBoxResult.OK;
                }

                if (this.MsgIcon == MessageBoxType.QTimeOut)
                {
                    Status = "Continue";
                }
                this.Close();
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorInfoLog(ex.Message, ex);
                throw;
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (this.Countdown <= 1)
                {
                    Status = "BackHome";
                    this.timer?.Stop();
                    this.timer = null;
                    this.Close();
                }
                else
                {
                    this.btnCancel.Content = ResourceHelper.GetResourceInfo("M_Button_BackHome") + "(" + --this.Countdown + ")";
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorInfoLog(ex.Message, ex);
                throw;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            try
            {
                this.Owner?.Close();
                this.timer?.Stop();
                this.timer = null;
                this.msgCallBack?.Invoke(Status);
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorInfoLog(ex.Message, ex);
                throw;
            }
        }

        #endregion

        #region 方法
        private void SetIcon()
        {
            switch (this.MsgIcon)
            {
                case MessageBoxType.QCancelAndOk:
                case MessageBoxType.QYesAndNo:
                case MessageBoxType.QCancelAndAmend:
                    this.lblCaption.Content = ResourceHelper.GetResourceInfo("M_Caption_Question");
                    this.lblErrorCode.Visibility = Visibility.Collapsed;
                    break;
                case MessageBoxType.Error:
                    this.lblCaption.Content = ResourceHelper.GetResourceInfo("M_Caption_Error");
                    break;
                case MessageBoxType.Warning:
                    this.lblCaption.Content = ResourceHelper.GetResourceInfo("M_Caption_Attention");
                    break;
                case MessageBoxType.Information:
                    this.lblCaption.Content = ResourceHelper.GetResourceInfo("M_Caption_Info");
                    break;
                case MessageBoxType.QTimeOut:
                    //操作超时
                    this.lblCaption.Content = ResourceHelper.GetResourceInfo("M_Label_Timeout");
                    break;

                default:
                    break;
            }
        }

        private void SetMessageType()
        {
            switch (this.MsgButton)
            {
                case MessageBoxButton.OK:
                    this.grdBtn.Visibility = Visibility.Collapsed;
                    this.btnOK.Visibility = Visibility.Visible;
                    break;
                case MessageBoxButton.OKCancel:
                case MessageBoxButton.YesNo:
                    this.grdBtn.Visibility = Visibility.Visible;
                    this.btnOK.Visibility = Visibility.Collapsed;
                    break;
                default:
                    this.grdBtn.Visibility = Visibility.Collapsed;
                    this.btnOK.Visibility = Visibility.Visible;
                    break;
            }

        }

        private void SetUIText()
        {
            this.btnOK.Content = ResourceHelper.GetResourceInfo("M_Button_OK");
            if (this.MsgIcon == MessageBoxType.QYesAndNo)
            {
                this.btnCancel.Content = ResourceHelper.GetResourceInfo("M_Button_No");
                this.btnCancelOK.Content = ResourceHelper.GetResourceInfo("M_Button_Yes");
            }
            else if (this.MsgIcon == MessageBoxType.QTimeOut)
            {
                //返回首页
                string backHome = ResourceHelper.GetResourceInfo("M_Button_BackHome");
                this.btnCancel.Width = 150;
                if (AppSettings.Language == "en-US")
                {
                    this.btnCancel.Width += 30;
                }
                this.btnCancel.Content = backHome + "({0})";
                this.btnCancel.Style = (Style)this.FindResource("TimeOut");
                //继续
                string btnContinue = ResourceHelper.GetResourceInfo("M_Button_Continue");
                this.btnCancelOK.Content = btnContinue;

                this.lblErrorCode.Visibility = Visibility.Collapsed;

            }
            else if (this.MsgIcon == MessageBoxType.QRetryAndCancel)
            {
                this.btnCancel.Content = ResourceHelper.GetResourceInfo("M_Button_Cancel");
                this.btnCancelOK.Content = ResourceHelper.GetResourceInfo("M_Button_Retry");
            }
            else if (this.MsgIcon == MessageBoxType.QCancelAndOk)
            {
                this.btnCancel.Content = ResourceHelper.GetResourceInfo("M_Button_Cancel");
                this.btnCancelOK.Content = ResourceHelper.GetResourceInfo("M_Button_CancelOK");
            }
            else if (this.MsgIcon == MessageBoxType.QCancelAndAmend)
            {
                this.btnCancel.Content = ResourceHelper.GetResourceInfo("M_Button_Cancel");
                this.btnCancelOK.Content = ResourceHelper.GetResourceInfo("M_Button_Amend");
            }

            this.btnCancel.FontFamily = new FontFamily(AppSettings.FamilyName);
            this.btnCancelOK.FontFamily = new FontFamily(AppSettings.FamilyName);
            this.btnOK.FontFamily = new FontFamily(AppSettings.FamilyName);
            this.lblMessage.FontFamily = new FontFamily(AppSettings.FamilyName);

            this.lblErrorCode.Content = string.Format(this.lblErrorCode.Content.ToString(), this.ErrorCode);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Dispose managed resources
                if (timer != null)
                    timer.Dispose();
            }

            // Free native resources
        }
        #endregion
    }

    /// <summary>
    /// 自定义Message类型
    /// </summary>
    public enum MessageBoxType
    {
        /// <summary>
        /// 超时询问类型
        /// </summary>
        QTimeOut,

        /// <summary>
        /// 是/否询问类型
        /// </summary>
        QYesAndNo,

        /// <summary>
        /// 确定/取消询问累心
        /// </summary>
        QCancelAndOk,

        /// <summary>
        /// 重试询问类型
        /// </summary>
        QRetryAndCancel,

        /// <summary>
        /// Redo寻味类型
        /// </summary>
        QCancelAndAmend,

        /// <summary>
        /// 错误类型
        /// </summary>
        Error,

        /// <summary>
        /// 警告类型
        /// </summary>
        Warning,

        /// <summary>
        /// 信息类型
        /// </summary>
        Information,
    }
}
