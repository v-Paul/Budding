/*************************************************
*Author:zhang danhong
*Date:2016/12/6 18:29:02
*Des:Js桥接工具类
************************************************/

using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;

namespace VTMC.Utils
{
    /// <summary>
    /// Js桥接工具类
    /// </summary>
    public static class InvokeScriptHelper
    {
        /// <summary>
        /// 给JS注入事件
        /// </summary>
        /// <param name="webBrowser">webBrowser对象</param>
        /// <param name="eventName">JS事件名称</param>
        /// <param name="objects">对象组</param>
        public static void InvokeJavaScript(System.Windows.Forms.WebBrowser webBrowser, string eventName, object[] objects)
        {
            LogHelper.WriteMethodLog(true);
            try
            {
                webBrowser.Document.InvokeScript(eventName, objects);
                
                AppSettings.mainForm.Focus();
                AppSettings.mainForm.Activate();
                webBrowser.Focus();
                //System.Windows.Forms.Application.DoEvents();
            }
            catch (System.Exception ex)
            {
                LogHelper.WriteErrorInfoLog(ex.Message, ex);
            }
            LogHelper.WriteMethodLog(false);
        }

        public static void InvokeJavaScriptNoFocus(System.Windows.Forms.WebBrowser webBrowser, string eventName, object[] objects)
        {
            LogHelper.WriteMethodLog(true);
            try
            {
                webBrowser.Document.InvokeScript(eventName, objects);
            }
            catch (System.Exception ex)
            {
                LogHelper.WriteErrorInfoLog(ex.Message, ex);
            }
            LogHelper.WriteMethodLog(false);
        }

        /// <summary>
        /// 给JS注入事件
        /// </summary>
        /// <param name="webBrowser">webBrowser对象</param>
        /// <param name="eventName">JS事件名称</param>
        /// <param name="objects">对象组</param>
        public static void AsyncInvokeJavaScript(System.Windows.Forms.WebBrowser webBrowser, string eventName, object[] objects)
        {
            LogHelper.WriteMethodLog(true);
            try
            {
                Application.Current.Dispatcher.Invoke(new System.Action(() =>
                {
                    if (!webBrowser.IsDisposed)
                    {
                        webBrowser?.Document?.InvokeScript(eventName, objects);
                        AppSettings.mainForm?.Focus();
                        AppSettings.mainForm?.Activate();
                        webBrowser?.Focus();
                    }
                    //System.Windows.Forms.Application.DoEvents();
                }));
            }
            catch (System.Exception ex)
            {
                LogHelper.WriteErrorInfoLog(ex.Message, ex);
            }


            LogHelper.WriteMethodLog(false);
        }

        /// <summary>
        /// 给JS注入事件
        /// </summary>
        /// <param name="webBrowser"></param>
        /// <param name="eventName"></param>
        public static void InvokeJavaScript(WebBrowser webBrowser, string eventName)
        {
            LogHelper.WriteMethodLog(true);
            ComponentDispatcher.PushModal();
            Application.Current.Dispatcher.Invoke(new System.Action(() =>
            {
                webBrowser.InvokeScript(eventName);
            }));
            LogHelper.WriteMethodLog(false);
        }
        /// <summary>
        /// 给JS注入事件
        /// </summary>
        /// <param name="webBrowser"></param>
        /// <param name="eventName"></param>
        /// <returns></returns>
        public static object InvokeJavaScriptWithReturnValue(WebBrowser webBrowser, string eventName)
        {
            LogHelper.WriteMethodLog(true);
            try
            {
                return webBrowser.InvokeScript(eventName);
            }
            finally
            {
                LogHelper.WriteMethodLog(false);
            }
        }
        /// <summary>
        /// 给JS注入事件
        /// </summary>
        /// <param name="webBrowser"></param>
        /// <param name="eventName"></param>
        /// <param name="objects"></param>
        /// <returns></returns>
        public static object InvokeJavaScriptWithReturnValue(WebBrowser webBrowser, string eventName, object[] objects)
        {
            LogHelper.WriteMethodLog(true);
            try
            {
                return webBrowser.InvokeScript(eventName, objects);
            }
            finally
            {
                LogHelper.WriteMethodLog(false);
            }
        }
    }
}