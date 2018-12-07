/*************************************************
*Author:Paul Wang
*Date:4/28/2017 3:17:03 PM
*Des:  
************************************************/
using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace VTMC.Utils
{
    /// <summary>
    /// 自动更新Token
    /// </summary>
    public static class UpdateToken
    {
        #region Consts
        #endregion

        #region Private Members
        /// <summary>
        /// 前次更新时间
        /// </summary>
        private static DateTime UpdateTime;
        #endregion

        #region Properties
        #endregion

        #region Public Methods
        /// <summary>
        /// 更新TokenKey值
        /// </summary>
        public static void UpdateTokenKey()
        {
            LogHelper.WriteMethodLog(true);

            TimeSpan timeSpan = UpdateTime - DateTime.Now;
            //if (timeSpan.TotalMinutes < AppSettings.TokenTimeOut || string.IsNullOrEmpty(AppSettings._token))
            //{
            //    //if(!GetTokenKey())
            //    //{
            //    //    LogHelper.WriteInfoLog(MessageList.Error_002.MsgCode);
            //    //    MessageList.Error_002.Show();
            //    //    Application.Current.Shutdown();
            //    //}
            //}

            LogHelper.WriteMethodLog(false);
        }
        #endregion

        #region Private Methods
        ///// <summary>
        ///// 获取Token
        ///// </summary>
        ///// <returns></returns>
        //private static bool GetTokenKey()
        //{
        //    LogHelper.WriteMethodLog(true);

        //    string url = AppSettings.TokenRequestUrl.Replace("{JavaHost}", AppSettings.JavaHost);
        //    HttpWebResponse mHttpWebResponse = null;

        //    try
        //    {
        //        HttpWebRequest mTokenRequest = (HttpWebRequest)HttpWebRequest.Create(url);

        //        #region 追加证书文件
        //        HttpHelper.AddCertificateFile(mTokenRequest);
        //        #endregion

        //        mTokenRequest.Method = "POST";
        //        mTokenRequest.ContentType = "application/json";
        //        string key = AppSettings.KEY3DES;
        //        string data = TripleDESEncodeECB(Convert.FromBase64String(key), new byte[0], GetDate());
        //        mTokenRequest.Headers.Set("ENCRYPT-VTM-CLIENT-INFO", data);
        //        var mSendStream = mTokenRequest.GetRequestStream();
        //        mSendStream.Flush();
        //        mSendStream.Close();
        //        mHttpWebResponse = (HttpWebResponse)mTokenRequest.GetResponse();

        //        //解析body
        //        using (var httpStreamReader = new StreamReader(mHttpWebResponse.GetResponseStream(), Encoding.GetEncoding("utf-8")))
        //        {
        //            var mBuffer = httpStreamReader.ReadToEnd();
        //            LogHelper.WriteInfoLog("调用取得Token API，返回结果：" + mBuffer);

        //            if (JsonHelper.CheckJsonItemKey(mBuffer, "vtaAccount"))
        //            {
        //                UpdateTime = ConvertHelper.ToDateTime(JsonHelper.GetJsonItemValue(mBuffer, "expirationDate"));
        //                //AppSettings.AccountNo = JsonHelper.GetJsonItemValue(mBuffer, "vtaAccount");
        //                //AppSettings.BranchID = JsonHelper.GetJsonItemValue(mBuffer, "branch");
        //                //AppSettings.BranchName = JsonHelper.GetJsonItemValue(mBuffer, "branchName");
        //                //AppSettings.BranchAddress = JsonHelper.GetJsonItemValue(mBuffer, "branchAddress");
        //                AppSettings.VTCToken = mHttpWebResponse.GetResponseHeader(ConstHelper.CNT_VTM_TOKEN_KEY);
        //            }
        //            else
        //            {
        //                return false;
        //            }
        //        }

        //        return true;
        //    }
        //    catch (WebException webEx)
        //    {
        //        HttpWebResponse webResponse = (HttpWebResponse)webEx.Response;
        //        if (webResponse != null)
        //        {
        //            using (var tokenStreamReader = new StreamReader(webResponse.GetResponseStream(), Encoding.GetEncoding("utf-8")))
        //            {
        //                var bodyStr = tokenStreamReader.ReadToEnd();
        //                LogHelper.WriteErrorInfoLog("从服务器获取Token异常发生,服务器返回：" + bodyStr?.ToString(), webEx);
        //            }
        //        }
        //        else
        //        {
        //            LogHelper.WriteErrorInfoLog("从服务器获取Token异常发生。", webEx);
        //        }
        //        return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        LogHelper.WriteErrorInfoLog("从服务器获取Token异常发生。", ex);
        //        return false;
        //    }
        //    finally
        //    {
        //        mHttpWebResponse?.Close();
        //        LogHelper.WriteMethodLog(false);
        //    }
        //}

        /// <summary>
        /// DES3 ECB模式加密
        /// </summary>
        /// <param name="key">秘钥</param>
        /// <param name="iv">IV(当模式为ECB时，IV无用)</param>
        /// <param name="data">明文的byte数组</param>
        /// <returns>密文的Base64数组数组</returns>
        private static string TripleDESEncodeECB(byte[] key, byte[] iv, byte[] data)
        {
            LogHelper.WriteMethodLog(true);

            try
            {
                string EncryData = string.Empty;

                MemoryStream Stm = new MemoryStream();
                TripleDESCryptoServiceProvider Provider = new TripleDESCryptoServiceProvider();
                Provider.Mode = CipherMode.ECB;
                Provider.Padding = PaddingMode.PKCS7;
                CryptoStream crpStm = new CryptoStream(Stm, Provider.CreateEncryptor(key, iv), CryptoStreamMode.Write);
                crpStm.Write(data, 0, data.Length);
                crpStm.FlushFinalBlock();
                EncryData = Convert.ToBase64String(Stm.ToArray());
                crpStm.Close();

                return EncryData;
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorInfoLog("生成DES密码异常发生。",ex);
                return null;
            }
            finally
            {
                LogHelper.WriteMethodLog(false);
            }
        }

        /// <summary>
        /// 获取需要发送的数据
        /// </summary>
        /// <returns></returns>
        //private static byte[] GetDate()
        //{
        //    LogHelper.WriteMethodLog(true);

        //    string stp = ((UInt64)(DateTime.Now.Subtract(DateTime.Parse("1970-1-1")).TotalMilliseconds)).ToString();
        //    string ver = AppSettings.ProductVersion;
        //    string temp = AppSettings.VTMID + "," + stp + "," + ver;

        //    LogHelper.WriteMethodLog(false);
        //    return Encoding.UTF8.GetBytes(temp);
        //}
        #endregion
    }
}
