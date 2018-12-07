using System;
using System.Runtime.InteropServices;

namespace VTMC.Utils
{
    /// <summary>
    /// 高分屏DPI模式工具类
    /// </summary>
    public static class DpiHelper
    {
        #region Consts
        #endregion

        #region Fileds
        #endregion

        #region Property
        #endregion

        #region Public Function

        /// <summary>
        /// 获取系统DPI设定值(默认96)
        /// </summary>
        public static int DPI
        {
            get
            {
                int _dpi = 0;
                if (_dpi == 0)
                {
                    HandleRef desktopHwnd = new HandleRef(null, IntPtr.Zero);
                    HandleRef desktopDC = new HandleRef(null, NativeMethods.GetDC(desktopHwnd));

                    _dpi = NativeMethods.GetDeviceCaps(desktopDC, 88 /*LOGPIXELSX*/);

                    NativeMethods.ReleaseDC(desktopHwnd, desktopDC);
                }

                return _dpi;
            }
        }

        /// <summary>
        /// 显示值和DPI之间的转化
        /// </summary>
        /// <param name="pixels"></param>
        /// <returns></returns>
        public static int ConvertPixelsToDIPixels(int pixels)
        {
            return pixels * 96 / DPI;
        }

        public static int ConvertDIPixelsToPixels(int pixels)
        {
            return pixels * DPI / 96;
        }
        #endregion
    }
}
