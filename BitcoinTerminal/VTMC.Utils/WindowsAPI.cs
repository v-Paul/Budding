/*************************************************
*Author:Paul Wang
*Date:6/9/2017 2:39:35 PM
*Des:  
************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace VTMC.Utils
{
    public class WindowsAPI
    {
        public static void SetProcessDPIAware()
        {
            NativeMethods.SetProcessDPIAware();
        }

        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);    //窗体置顶
        private static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);    //取消窗体置顶
        private const uint SWP_NOMOVE = 0x0002;    //不调整窗体位置
        private const uint SWP_NOSIZE = 0x0001;    //不调整窗体大小

        /// <summary>
        /// 设置/取消窗体置顶
        /// </summary>
        /// <param name="isTopmost">是否置顶</param>
        /// <param name="windowText">需要置顶的窗体的名字</param>
        public static void SetFormTopmost(bool isTopmost, string windowName)
        {
            if (isTopmost)
            {
                IntPtr CustomBar = NativeMethods.FindWindow(null, windowName);
                if (CustomBar != null && CustomBar != IntPtr.Zero)
                {
                    NativeMethods.SetWindowPos(CustomBar, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
                }
            }
            else
            {
                IntPtr CustomBar = NativeMethods.FindWindow(null, windowName);
                if (CustomBar != null && CustomBar != IntPtr.Zero)
                {
                    NativeMethods.SetWindowPos(CustomBar, HWND_NOTOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
                }
            }
        }

        /// <summary>
        /// C++解密
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string DecryptCBCByCpp(string data)
        {
            IntPtr IntRes = NativeMethods.DecryptCBCByCpp(data);
            string str = Marshal.PtrToStringAnsi(IntRes);
            return str;
        }

    }

    internal static class NativeMethods
    {
        /// <summary>
        /// 设置画面在DPI下自动缩放
        /// </summary>
        [DllImport("user32.dll")]
        internal static extern void SetProcessDPIAware();


        [DllImport("user32.dll")]
        internal static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        internal static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        #region Dpi Helper Use
        [DllImport("User32.dll")]
        internal static extern IntPtr GetDC(HandleRef hWnd);

        [DllImport("User32.dll")]
        internal static extern int ReleaseDC(HandleRef hWnd, HandleRef hDC);

        [DllImport("GDI32.dll")]
        internal static extern int GetDeviceCaps(HandleRef hDC, int nIndex);
        #endregion

        #region C++加解密

        [DllImport(@"TripleDesDll.dll", EntryPoint = "TRIPLE_CBC_EncryptDefaultKeyIV", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal extern static IntPtr EncryptCBCByCpp(string source);


        [DllImport(@"TripleDesDll.dll", EntryPoint = "TRIPLE_CBC_DecryptDefaultKeyIV", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal extern static IntPtr DecryptCBCByCpp(string source);

        #endregion

    }

}
