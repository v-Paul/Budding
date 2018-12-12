using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;
using System.Data;
using VTMC.Utils;
namespace BaseSturct
{
    class LeveldbOperator
    {


        [DllImport(@"LevelDBdll.dll", EntryPoint = "OpenDB", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private extern static IntPtr openDB(string strDbPath);

        [DllImport(@"LevelDBdll.dll", EntryPoint = "CloseDB", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private extern static IntPtr closeDB();

        [DllImport(@"LevelDBdll.dll", EntryPoint = "GetValue", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private extern static IntPtr getValue(string strkey);

        [DllImport(@"LevelDBdll.dll", EntryPoint = "PutKeyValue", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private extern static IntPtr putKeyValue(string strkey, string strValue);

        [DllImport(@"LevelDBdll.dll", EntryPoint = "DelItm", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private extern static IntPtr delItm(string strkey);

        [DllImport(@"LevelDBdll.dll", EntryPoint = "GetKey", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private extern static IntPtr  getKey(int iPos);

        [DllImport(@"LevelDBdll.dll", EntryPoint = "GetfirstKey", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private extern static IntPtr getfirstKey();

        [DllImport(@"LevelDBdll.dll", EntryPoint = "GetlastKey", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private extern static IntPtr getlastKey();

        [DllImport(@"LevelDBdll.dll", EntryPoint = "GetNextKey", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private extern static IntPtr getNextKey();



        #region Public fuction
        public static string OpenDB(string strDbPath)
        {
            IntPtr IntRes = openDB(strDbPath);
            string str = Marshal.PtrToStringAnsi(IntRes);
            return str;
        }

        public static void CloseDB( )
        {
            try
            {
                IntPtr IntRes = closeDB();
            }
            catch(Exception ex)
            {

            }
            
        }
        public static string GetValue(string strkey)
        {
            if (string.IsNullOrEmpty(strkey))
                return string.Empty;

            IntPtr IntRes = getValue(strkey);          
            string strValue = Marshal.PtrToStringAnsi(IntRes);
            
            return strValue;
        }

        public static string PutKeyValue(string strkey, string strValue)
        {
            IntPtr IntRes = putKeyValue(strkey, strValue);
            string str = Marshal.PtrToStringAnsi(IntRes);
            return str;
        }

        public static string DelItm(string strkey)
        {
            IntPtr IntRes = delItm(strkey);
            string str = Marshal.PtrToStringAnsi(IntRes);
            return str;
        }


        public static string GetKey(int iPos)
        {
            IntPtr IntRes = getKey(iPos);
            string str = Marshal.PtrToStringAnsi(IntRes);
            return str;
        }

        public static string GetfirstKey()
        {
            IntPtr IntRes = getfirstKey();
            string str = Marshal.PtrToStringAnsi(IntRes);
            return str;
        }
        public static string GetlastKey()
        {
            IntPtr IntRes = getlastKey();
            string str = Marshal.PtrToStringAnsi(IntRes);
            return str;
        }
        public static string GetNextKey()
        {
            IntPtr IntRes = getNextKey();
            string str = Marshal.PtrToStringAnsi(IntRes);
            return str;
        }


        public static string PrintAlldb()
        {
            try
            {
                string strRet = LeveldbOperator.OpenDB(AppSettings.XXPDBFolder);
                if (strRet != ConstHelper.BC_OK)
                {
                    return "open db fail";
                }
                strRet = LeveldbOperator.GetfirstKey();
                if (string.IsNullOrEmpty(strRet))
                {

                    return "empty DB";
                }
                string blockValue = LeveldbOperator.GetValue(strRet);
                string lastblock = string.Empty;
                LogHelper.WriteInfoLog(blockValue);
               

                while (true)
                {
                    strRet = LeveldbOperator.GetNextKey();
                    if (string.IsNullOrEmpty(strRet))
                    {
                        break;
                    }
                    else if(strRet == ConstHelper.BC_LastKey)
                    {
                        lastblock = LeveldbOperator.GetValue(strRet);
                    }
                    else
                    {
                        blockValue = LeveldbOperator.GetValue(strRet);
                        LogHelper.WriteInfoLog(blockValue);
                    }
                    

                }
                LogHelper.WriteInfoLog("LashBlock: " + lastblock);
                return ConstHelper.BC_OK;
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorLog(ex.Message);
                return "exception";
            }
            finally
            {
                LeveldbOperator.CloseDB();
            }

        }

        #endregion


    }
}
