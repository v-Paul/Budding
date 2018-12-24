using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;
using System.Data;
using VTMC.Utils;
using System.Text.RegularExpressions;

namespace BaseSturct
{
    class Cryptor
    {

        [DllImport(@"TripleDesDll.dll", EntryPoint = "generateRSAKey2File", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public extern static IntPtr generateRSAKey2File(string strPubKeyPath, string strPriKeyPath);

        [DllImport(@"TripleDesDll.dll", EntryPoint = "sha256", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private extern static IntPtr  sha256(string strOriginal, int iLength);


        [DllImport(@"TripleDesDll.dll", EntryPoint = "rsa_pri_sign", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private extern static IntPtr rsaPriSign(string strText, int iTxtlen, string strPriKey, bool bIsKeybyPath);


        [DllImport(@"TripleDesDll.dll", EntryPoint = "rsa_pub_DecryptSignature", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private extern static IntPtr rsaPubDecrySign(string strSignature, int iTxtlen, string strPubKey, bool bIsKeybyPath);


        [DllImport(@"TripleDesDll.dll", EntryPoint = "calc24", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private extern static int calc24(int iNum0, int iNum1, int iNum2, int iNum3);


        public static string SHA256(string strOriginal, int iLength)
        {
            IntPtr IntRes = sha256(strOriginal, iLength);
            string str = Marshal.PtrToStringAnsi(IntRes);
            return str;
        }

        /// <summary>
        /// 私钥签名，私钥通过文件路径传进来
        /// </summary>
        /// <param name="strText"></param>
        /// <param name="strPriKeyPath"></param>
        /// <returns></returns>
        public static string rsaPriSign(string strText, string strPriKeyPath)
        {
            IntPtr IntRes =  rsaPriSign(strText, strText.Length, strPriKeyPath, true);
            string str = Marshal.PtrToStringAnsi(IntRes);
            return str;
        }

        /// <summary>
        /// 公钥验证验证签名，公钥通过string传进来
        /// </summary>
        /// <param name="strText"></param>
        /// <param name="strPubKeyPath"></param>
        /// <returns></returns>
        public static string rsaPubDecrySign(string strText, string strPubKeyPath, bool bIsKeybyPath)
        {
            IntPtr IntRes = rsaPubDecrySign(strText, strText.Length, strPubKeyPath, bIsKeybyPath);
            string str = Marshal.PtrToStringAnsi(IntRes);
            return str;
        }


        public static bool VerifySignature(scriptSig scriptSig, string strOpScript, string strOriginalTxt )
        {
            // string strDecodeTxt = rsaPubDecrySign(strSignature, strPubKey, false);
            // return string.Equals(strDecodeTxt, strOriginalTxt);
            string strDecodeTxt = string.Empty;

            Operation Oper = new Operation(scriptSig, strOpScript);
            bool bRet = Oper.RunScript(ref strDecodeTxt);
            if(bRet)
            {
                if (string.Equals(strDecodeTxt, strOriginalTxt))
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        public static bool Calc24(int[] lstNums)
        {
            int iOK = calc24(lstNums[0], lstNums[1], lstNums[2], lstNums[3]);
            if (iOK == 0)
                return true;
            else
                return false;           
        }

        public static bool Verify24Puzzel( int[] arrPuzzle,string strExpress)
        {
            try
            {
                if (arrPuzzle.Length != 4) return false;
                if (string.IsNullOrEmpty(strExpress)) return false;

                string temp = strExpress;
                var lsPuzzle = arrPuzzle.GroupBy(x => x).Select(g => new { name = g.Key.ToString(), count = g.Count() }).ToList();
                //List<string> lsMath = new List<string>() { "(", ")", "+", "-", "/", "*" };
                //foreach (var item in lsMath)
                //{
                //    temp = temp.Replace(item, "|");
                //}

                temp = Regex.Replace(temp, "[(]|[)]|[+]|[-]|[*]|[/]", "|");
                var lstexpress = (from x in temp.Split('|')
                               where x != ""
                               select x).ToList();
                var solve = lstexpress.GroupBy(x => x).Select(g => new { name = g.Key, count = g.Count() }).ToList();
                if (lsPuzzle.Count != solve.Count)
                {
                    return false;
                }

                foreach (var item in lsPuzzle)
                {
                    if (!solve.Contains(item))
                        return false;
                }

                DataTable dt = new DataTable();
                var Result = dt.Compute(strExpress, "");
                int iRet = Convert.ToInt32(Result);
                if (iRet == 24)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception ex)
            {
                LogHelper.WriteErrorLog(ex.Message);
                return false;
            }
        }

    }
}
