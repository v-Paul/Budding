using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace VTMC.Utils
{
    public class WaitBoxHelper
    {
        #region Const
        #endregion

        #region Filed
        private static WaitBoxHelper _instance;
        private static readonly Object syncLock = new Object();
        private Thread waitThread;
        //private static WaitBox2 waitForm;
        //private static WaitBlackspot waitForm;
        private static WaitBox waitForm;
        #endregion

        #region CallBacks

        #endregion

        #region Property

        /// <summary>    
        /// 单例模式    
        /// </summary>    
        public static WaitBoxHelper Instance
        {
            get
            {
                if (WaitBoxHelper._instance == null)
                {
                    lock (syncLock)
                    {
                        if (WaitBoxHelper._instance == null)
                        {
                            WaitBoxHelper._instance = new WaitBoxHelper();
                        }
                    }
                }
                return WaitBoxHelper._instance;
            }
        }

        #endregion

        #region Event

        #endregion

        #region Public Function
        /// <summary>    
        /// 显示等待窗体    
        /// </summary>    
        public static void Show()
        {
            WaitBoxHelper.Instance._CreateForm();
        }

        /// <summary>    
        /// 关闭等待窗体    
        /// </summary>    
        public static void Close()
        {
            Thread.Sleep(300);
            WaitBoxHelper.Instance._CloseForm();
        }
        #endregion

        #region Private Function

        /// <summary>    
        /// 为了单例模式防止new 实例化..    
        /// </summary>    
        private WaitBoxHelper()
        {
        }

        /// <summary>    
        /// 创建等待窗体    
        /// </summary>    
        public void _CreateForm()
        {
            waitForm = null;
            waitThread = new Thread(new ThreadStart(this._ShowWaitForm));
            waitThread.SetApartmentState(ApartmentState.STA);
            waitThread.Start();
        }

        private void _ShowWaitForm()
        {
            try
            {
                //waitForm = new WaitBox2();
                waitForm = new WaitBox();// new WaitBlackspot();
                waitForm.ShowDialog();
            }
            catch
            {
                waitForm = null;
                Thread.ResetAbort();
            }
        }

        /// <summary>    
        /// 关闭窗体    
        /// </summary>    
        private void _CloseForm()
        {
            if (waitThread != null && waitForm != null && waitForm.IsVisible)
            {
                System.Windows.Forms.Application.DoEvents();
                waitForm.Dispatcher.Invoke(() =>
                {
                    waitForm.Close();
                });
            }
        }
        #endregion
    }
}
