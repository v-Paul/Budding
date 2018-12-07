using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace VTMC.Utils
{
    /// <summary>
    /// DES加密
    /// </summary>
    public static class EncryptHelper
    {

        #region 3DES加解密

        /// <summary>
        /// 3Des加密
        /// </summary>
        /// <param name="strQ"></param>
        /// <param name="rgbKey"></param>
        /// <param name="rgbIV"></param>
        /// <returns></returns>
        public static string TriEncrypt(string strQ, string rgbKey, string rgbIV)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(strQ);
            byte[] bKey = HexToBytes(rgbKey);
            byte[] bIV = HexToBytes(rgbIV);

            MemoryStream ms = new MemoryStream();
            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();

            CryptoStream encStream = new CryptoStream(ms, tdes.CreateEncryptor(bKey, bIV), CryptoStreamMode.Write);
            encStream.Write(buffer, 0, buffer.Length);
            encStream.FlushFinalBlock();
            return BytesToHex(ms.ToArray());
        }


        /// <summary>
        /// 3Des解密
        /// </summary>
        /// <param name="strQ"></param>
        /// <param name="rgbKey"></param>
        /// <param name="rgbIV"></param>
        /// <returns></returns>
        public static string TriDecrypt(string strQ, string rgbKey, string rgbIV)
        {
            byte[] buffer = HexToBytes(strQ);
            byte[] bKey = HexToBytes(rgbKey);
            byte[] bIV = HexToBytes(rgbIV);
            MemoryStream ms = new MemoryStream();
            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            CryptoStream encStream = new CryptoStream(ms, tdes.CreateDecryptor(bKey, bIV), CryptoStreamMode.Write);
            encStream.Write(buffer, 0, buffer.Length);
            encStream.FlushFinalBlock();
            return Encoding.UTF8.GetString(ms.ToArray());
        }

        /// <summary>
        /// 3DES加密，CBC
        /// </summary>
        /// <param name="data"></param>
        /// <param name="Key"></param>
        /// <param name="IV"></param>
        /// <returns></returns>
        public static byte[] DESEncryptCBC(byte[] data, byte[] Key, byte[] IV)
        {
            // Create a MemoryStream.
            using (MemoryStream mStream = new MemoryStream())
            {
                // Create a CryptoStream using the MemoryStream 
                // and the passed key and initialization vector (IV).
                var tripleDESCryptoServiceProvider = new TripleDESCryptoServiceProvider();
                tripleDESCryptoServiceProvider.Mode = CipherMode.CBC;
                tripleDESCryptoServiceProvider.Padding = PaddingMode.None;

                using (CryptoStream cStream = new CryptoStream(mStream,
                    tripleDESCryptoServiceProvider.CreateEncryptor(Key, IV),
                    CryptoStreamMode.Write))
                {

                    // Convert the passed string to a byte array.
                    byte[] toEncrypt = data;

                    // Write the byte array to the crypto stream and flush it.
                    cStream.Write(toEncrypt, 0, toEncrypt.Length);
                    cStream.FlushFinalBlock();

                    // Get an array of bytes from the 
                    // MemoryStream that holds the k
                    // encrypted data.
                    byte[] ret = mStream.ToArray();
                    // Return the encrypted buffer.
                    return ret;
                }
            }
        }
        /// <summary>
        /// 3DES解密CBC
        /// </summary>
        /// <param name="data"></param>
        /// <param name="Key"></param>
        /// <param name="IV"></param>
        /// <returns></returns>
        public static byte[] DESDecryptCBC(byte[] data, byte[] Key, byte[] IV)
        {

            // Create a new MemoryStream using the passed 
            // array of encrypted data.
            MemoryStream msDecrypt = new MemoryStream(data);

            // Create a CryptoStream using the MemoryStream 
            // and the passed key and initialization vector (IV).
            var tripleDESCryptoServiceProvider = new TripleDESCryptoServiceProvider();
            tripleDESCryptoServiceProvider.Mode = CipherMode.CBC;
            tripleDESCryptoServiceProvider.Padding = PaddingMode.None;

            CryptoStream csDecrypt = new CryptoStream(msDecrypt,
                tripleDESCryptoServiceProvider.CreateDecryptor(Key, IV),
                CryptoStreamMode.Read);

            // Create buffer to hold the decrypted data.
            byte[] fromEncrypt = new byte[data.Length];

            // Read the decrypted data out of the crypto stream
            // and place it into the temporary buffer.
            csDecrypt.Read(fromEncrypt, 0, fromEncrypt.Length);

            //Convert the buffer into a string and return it.
            //return new ASCIIEncoding().GetString(fromEncrypt);
            return fromEncrypt;
        }

 

        /// <summary>  
        /// MAC计算 (ANSI-X9.9-MAC)  
        /// </summary>  
        /// <param name="data">数据</param>  
        /// <returns>返回该数据MAC值</returns>  
        public static byte[] GetMAC(byte[] data, byte[] Key, byte[] IV)
        {
            try
            {
                int iGroup = 0;
                byte[] bKey = Key;
                byte[] bIV = IV;
                byte[] bTmpBuf1 = new byte[8];
                byte[] bTmpBuf2 = new byte[8];
                bool bIs8bit = false;
                // init  
                Array.Copy(bIV, bTmpBuf1, 8);
                if ((data.Length % 8 == 0))
                {
                    bIs8bit = true;
                    iGroup = data.Length / 8;
                }
                else
                {
                    bIs8bit = false;
                    iGroup = data.Length / 8 + 1;
                }
                int i = 0;
                int j = 0;
                for (i = 0; i < iGroup; i++)
                {
                    if ( i == iGroup-1 && (!bIs8bit) )
                    {
                        bTmpBuf2 = new byte[8];
                        Array.Copy(data, 8 * i, bTmpBuf2, 0, data.Length%8);
                    }
                    else
                    {
                        Array.Copy(data, 8 * i, bTmpBuf2, 0, 8);
                    }
                    

                    for (j = 0; j < 8; j++)
                    {
                        bTmpBuf1[j] = (byte)(bTmpBuf1[j] ^ bTmpBuf2[j]);
                    }
                    bTmpBuf2 = DESEncryptCBC(bTmpBuf1, bKey, bIV);
                    Array.Copy(bTmpBuf2, bTmpBuf1, 8);
                }
                return bTmpBuf2;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #endregion

        #region DES加解密

        /// <summary>
        /// 字符串加密
        /// </summary>
        /// <param name="strQ"></param>
        /// <param name="rgbKey"></param>
        /// <param name="rgbIV"></param>
        /// <returns></returns>
        public static string Encrypt(string strQ, string rgbKey, string rgbIV)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(strQ);
            MemoryStream ms = new MemoryStream();
            DESCryptoServiceProvider tdes = new DESCryptoServiceProvider();
            CryptoStream encStream = new CryptoStream(ms, tdes.CreateEncryptor(Encoding.UTF8.GetBytes(rgbKey), Encoding.UTF8.GetBytes(rgbIV)), CryptoStreamMode.Write);
            encStream.Write(buffer, 0, buffer.Length);
            encStream.FlushFinalBlock();
            return Convert.ToBase64String(ms.ToArray()).Replace("+", "%");
        }

        /// <summary>
        /// 字符串解密
        /// </summary>
        /// <param name="strQ"></param>
        /// <param name="rgbKey"></param>
        /// <param name="rgbIV"></param>
        /// <returns></returns>
        public static string Decrypt(string strQ, string rgbKey, string rgbIV)
        {

            try
            {
                strQ = strQ.Replace("%", "+");
                byte[] buffer = Convert.FromBase64String(strQ);
                byte[] bKey = Encoding.UTF8.GetBytes(rgbKey);
                byte[] bIV = Encoding.UTF8.GetBytes(rgbIV);

                MemoryStream ms = new MemoryStream();
                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
                CryptoStream encStream = new CryptoStream(ms, tdes.CreateDecryptor(bKey, bIV), CryptoStreamMode.Write);
                encStream.Write(buffer, 0, buffer.Length);
                encStream.FlushFinalBlock();
                return Encoding.UTF8.GetString(ms.ToArray());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        ///// <summary>
        ///// 生成ZD Key
        ///// </summary>
        ///// <returns></returns>
        //public static List<string> DecryptZDKey()
        //{
        //    List<string> lst = new List<string>();

        //    lst.Add(TriDecrypt("Lp6YsVNSIElj8gorW5nKAi/rk77b/ru0", AppSettings.rbgKey, AppSettings.rbgIV));
        //    lst.Add(TriDecrypt("9WQ8l%fl60sx6rYM1z2ND3fb5yKDVv1uj6c0ompIBl0=", AppSettings.rbgKey, AppSettings.rbgIV));
        //    lst.Add(TriDecrypt("zimxl3A8x4Mgb72JVo2kKvA/wQ5VeTKmXOYwWpTgLNI=", AppSettings.rbgKey, AppSettings.rbgIV));

        //    return lst;
        //}

        #region 凯撒密码(字符串移位)
        /// <summary>
        /// 根据凯撒密码做字符串右移
        /// </summary>
        /// <param name="str">源字符串</param>
        /// <param name="caesarBit">凯撒密码(移动位数)</param>
        /// <returns>移位后的字符串</returns>
        public static string CaesarDecrypt(string str, int caesarBit)
        {
            if (str.Contains(ConstHelper.NoBreakSpace))
            {
                str = str.Replace(ConstHelper.NoBreakSpace, " ");
            }
            byte[] array = Encoding.UTF8.GetBytes(str);
            for (int i = 0; i< array.Length; i++)
            {
                array[i] = (byte)(array[i] + caesarBit);
            }

            return Encoding.UTF8.GetString(array);
        }
        /// <summary>
        /// 根据凯撒密码做字符串左移
        /// </summary>
        /// <param name="str">源字符串</param>
        /// <param name="caesarBit">源字符串</param>
        /// <returns></returns>
        public static string CaesarEncrypt(string str, int caesarBit)
        {
            byte[] array = Encoding.UTF8.GetBytes(str);
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = (byte)(array[i] - caesarBit);
            }

            string tempStr = Encoding.UTF8.GetString(array);
            if (tempStr.Contains(" "))
            {
                tempStr = tempStr.Replace(" ", ConstHelper.NoBreakSpace);
            }
            return tempStr;
        }
        #endregion
        #endregion



        #region RSA加解密
        /// <summary>
        /// RSA加密
        /// </summary>
        public static byte[] RSAEncrypt(string publickey, byte[] data)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            byte[] cipherbytes;
            rsa.FromXmlString(publickey);
            cipherbytes = rsa.Encrypt(data, false);
            return cipherbytes;
        }

        /// <summary>
        /// RSA解密
        /// </summary>
        /// <param name="PrivateKey"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] RSADecrypt(string PrivateKey, byte[] data)
        {
            try
            {
                byte[] DypherTextBArray;
                System.Security.Cryptography.RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.FromXmlString(PrivateKey);
                DypherTextBArray = rsa.Decrypt(data, false);
                return DypherTextBArray;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region  PRD SetPin Key加密
        /// <summary>
        /// 用Key加密卡密码
        /// </summary>
        public static string GetCardSecretKey(string strPassword, string strCardNo)
        {
            try
            {
                LogHelper.WriteMethodLog(true);
                byte[] IV = { 1, 2, 3, 4, 5, 6, 7, 8 };
                string strPINHex = strPassword.PadRight(16, 'F');
                byte[] bPIN = HexToBytes(strPINHex);

                byte[] key = KeyGenerate(16);
                byte[] ePIN = EncryptTextToMemory(bPIN, key, IV);

                string publickey = ReadConsumerPinKey(AppSettings.ConsumerPinKey);
                byte[] eKey = RSAEncrypt(publickey, key);
                byte[] bytes = new byte[ePIN.Length + eKey.Length];
                ePIN.CopyTo(bytes, 0);
                eKey.CopyTo(bytes, ePIN.Length);
                return BytesToHex(bytes);
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorInfoLog("GetCardSecretKey", ex);
                //delete by fdp 180126 异常上抛pinbridge 定义错误码后返回给前端
                //return string.Empty;
                throw ex;
            }
            finally
            {
                LogHelper.WriteMethodLog(false);
            }
        }

        /// <summary>
        /// 用Key加密电话银行密码
        /// </summary>
        public static string GetTeleSecretKey(string strPassword, string PBN)
        {
            try
            {
                LogHelper.WriteMethodLog(true);

                byte[] IV = { 1, 2, 3, 4, 5, 6, 7, 8 };
                string strPBN = PBN.PadLeft(16, '0');
                byte[] bPBN = HexToBytes(strPBN);
                byte[] bPBNPIN = HexToBytes(strPassword + PBN);

                byte[] key = KeyGenerate(16);
                byte[] ePBN = EncryptTextToMemory(bPBN, key, IV);
                byte[] ePBNPIN = EncryptTextToMemory(bPBNPIN, key, IV);

                string publickey = ReadConsumerPinKey(AppSettings.ConsumerPinKey);
                byte[] eKey = RSAEncrypt(publickey, key);
                byte[] bytes = new byte[ePBN.Length + ePBNPIN.Length + eKey.Length];
                ePBN.CopyTo(bytes, 0);
                ePBNPIN.CopyTo(bytes, ePBN.Length);
                eKey.CopyTo(bytes, ePBN.Length + ePBNPIN.Length);
                return BytesToHex(bytes);
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorInfoLog("GetTeleSecretKey", ex);
                //delete by fdp 180126 异常上抛pinbridge 定义错误码后返回给前端
                //return string.Empty;
                throw ex;
            }
            finally
            {
                LogHelper.WriteMethodLog(false);
            }
        }
        #endregion

        #region 基本方法
        /// <summary>
        /// 十六进制转Byte
        /// </summary>
        public static byte[] HexToBytes(string strHex)
        {
            byte[] ordData = new byte[strHex.Length / 2];
            for (int i = 0; i < ordData.Length; i++)
            {
                ordData[i] = Convert.ToByte(strHex.Substring(i * 2, 2), 16);
            }
            return ordData;
        }

        /// <summary>
        /// Bytes转十六进制
        /// </summary>
        public static string BytesToHex(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
            {
                sb.AppendFormat("{0:X2}", b);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 生成随机KEY
        /// </summary>
        /// <param name="iKeyLength">传入key长度，8，16，24，48</param>
        /// <returns></returns>
        public static byte[] KeyGenerate(int iKeyLength)
        {
            //产生一个16byte的key
            TripleDESCryptoServiceProvider tDESalg = new TripleDESCryptoServiceProvider();
            byte[] key = tDESalg.Key;
            Array.Resize(ref key, iKeyLength);
            return key;
        }

        /// <summary>
        /// 对比数组是否相同
        /// </summary>
        /// <param name="b1"></param>
        /// <param name="b2"></param>
        /// <returns></returns>
        public static bool IsEquals(byte[] b1, byte[] b2)
        {
            if (b1 == null || b2 == null) return false;
            if (b1.Length != b2.Length) return false;
            for (int i = 0; i < b1.Length; i++)
                if (b1[i] != b2[i])
                    return false;
            return true;
        }

        /// <summary>
        /// 读取Key文件
        /// </summary>
        private static string ReadConsumerPinKey(String filename)
        {
            //remove java key ASN.1
            byte[] binBufferKey = File.ReadAllBytes(filename);
            LogHelper.WriteInfoLog("PinKey Length:" + binBufferKey.Length);
            byte[] binBuffer = binBufferKey.Skip(29).Take(128).ToArray();
            
            int base64ArraySize = (int)Math.Ceiling(binBuffer.Length / 3d) * 4;
            char[] charBuffer = new char[base64ArraySize];
            Convert.ToBase64CharArray(binBuffer, 0, binBuffer.Length, charBuffer, 0);
            string s = new string(charBuffer);
            string publickey = @"<RSAKeyValue><Modulus>" + s + @"</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
            return publickey;
        }

        /// <summary>
        /// 把Text加密为encrypted byte[]
        /// </summary>
        private static byte[] EncryptTextToMemory(byte[] Data, byte[] Key, byte[] IV)
        {
            try
            {
                LogHelper.WriteMethodLog(true);
                // Create a MemoryStream.
                using (MemoryStream mStream = new MemoryStream())
                {
                    // Create a CryptoStream using the MemoryStream 
                    // and the passed key and initialization vector (IV).
                    var tripleDESCryptoServiceProvider = new TripleDESCryptoServiceProvider();
                    tripleDESCryptoServiceProvider.Mode = CipherMode.ECB;
                    tripleDESCryptoServiceProvider.Padding = PaddingMode.None;

                    using (CryptoStream cStream = new CryptoStream(mStream,
                        tripleDESCryptoServiceProvider.CreateEncryptor(Key, IV),
                        CryptoStreamMode.Write))
                    {

                        // Convert the passed string to a byte array.
                        //byte[] toEncrypt = new ASCIIEncoding().GetBytes(Data);
                        byte[] toEncrypt = Data;

                        // Write the byte array to the crypto stream and flush it.
                        cStream.Write(toEncrypt, 0, toEncrypt.Length);
                        cStream.FlushFinalBlock();

                        // Get an array of bytes from the 
                        // MemoryStream that holds the k
                        // encrypted data.
                        byte[] ret = mStream.ToArray();
                        // Return the encrypted buffer.
                        return ret;
                    }
                }
            }
            catch (CryptographicException e)
            {
                LogHelper.WriteErrorInfoLog("A Cryptographic error occurred:", e);
                //return null;
                throw e;
            }
            finally
            {
                LogHelper.WriteMethodLog(false);
            }
        }

        /// <summary>
        /// 获取文件SHA256值
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetSHA256(string filePath)
        {
            try
            {
                byte[] retval = null;
                using (FileStream file = new FileStream(filePath, FileMode.Open))
                {
                    SHA256 sha256 = new SHA256CryptoServiceProvider();
                    retval = sha256.ComputeHash(file);
                }
                return BytesToHex(retval);
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorInfoLog(ex.Message, ex);
                return null;
            }
        }

        #endregion

        #region 生成校验Data-MAC-RSAenKey

        /// <summary>
        /// 生成mac数据
        /// </summary>
        /// <param name="strOriData">原始数据</param>
        /// <param name="RsaPubKey">Ras公钥</param>
        /// <returns></returns>
        public static  string GenEncDataMac(string strOriData, string RsaPubKey)
        {
            LogHelper.WriteInfoLog("GenEncDataMac strOriData.length: " + strOriData.Length);

            byte[] IV = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            byte[] desKey = new byte[24];
            desKey = EncryptHelper.KeyGenerate(24);            
            byte[] data = System.Text.Encoding.UTF8.GetBytes(strOriData);
            byte[] MacData = EncryptHelper.GetMAC(data, desKey, IV);
            byte[] rsaData = EncryptHelper.RSAEncrypt(RsaPubKey, desKey);

            byte[] endData = new byte[data.Length + MacData.Length + rsaData.Length];
            data.CopyTo(endData, 0);
            MacData.CopyTo(endData, data.Length);
            rsaData.CopyTo(endData, data.Length + MacData.Length);
            
            return EncryptHelper.BytesToHex(endData);

        }

        /// <summary>
        /// 检测MAC结果，一致true，不一致false
        /// </summary>
        /// <param name="strEncData">加密后数据</param>
        /// <param name="RsaPriKey">Rsa私钥</param>
        /// <returns></returns>
        public static bool CheckDataMac(string strEncData, string RsaPriKey, ref string strOriData)
        {
            LogHelper.WriteInfoLog("CheckDataMac strEncData: " + strEncData);
            byte[] IV = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            byte[] endData = EncryptHelper.HexToBytes(strEncData);

            byte[] rsaData = new byte[128];
            byte[] macData = new byte[8];
            byte[] Data = new byte[endData.Length - 128 - 8];

            Array.Copy(endData, endData.Length - 128, rsaData, 0, 128);
            Array.Copy(endData, endData.Length - 128 - 8, macData, 0, 8);
            Array.Copy(endData, Data, endData.Length - 128 - 8);

            byte[] desKey = EncryptHelper.RSADecrypt(RsaPriKey, rsaData);
           

            byte[] MacDataNow = EncryptHelper.GetMAC(Data, desKey, IV);
            if (EncryptHelper.IsEquals(macData, MacDataNow))
            {
                strOriData = System.Text.Encoding.UTF8.GetString(Data).Trim();
                return true;
            }
            else
            {
                strOriData = string.Empty;
                return false;
            }

        }

        #endregion

        //public static byte[] DecryptTextFromMemory(byte[] Data, byte[] Key, byte[] IV, CipherMode m1, PaddingMode m2)
        //{
        //    try
        //    {
        //        // Create a new MemoryStream using the passed 
        //        // array of encrypted data.
        //        MemoryStream msDecrypt = new MemoryStream(Data);

        //        // Create a CryptoStream using the MemoryStream 
        //        // and the passed key and initialization vector (IV).
        //        var tripleDESCryptoServiceProvider = new TripleDESCryptoServiceProvider();
        //        tripleDESCryptoServiceProvider.Mode = CipherMode.ECB;
        //        tripleDESCryptoServiceProvider.Padding = PaddingMode.None;
        //        //tripleDESCryptoServiceProvider.Mode = m1;
        //        //tripleDESCryptoServiceProvider.Padding = m2;

        //        CryptoStream csDecrypt = new CryptoStream(msDecrypt,
        //            tripleDESCryptoServiceProvider.CreateDecryptor(Key, IV),
        //            CryptoStreamMode.Read);

        //        // Create buffer to hold the decrypted data.
        //        byte[] fromEncrypt = new byte[Data.Length];

        //        // Read the decrypted data out of the crypto stream
        //        // and place it into the temporary buffer.
        //        csDecrypt.Read(fromEncrypt, 0, fromEncrypt.Length);

        //        //Convert the buffer into a string and return it.
        //        //return new ASCIIEncoding().GetString(fromEncrypt);
        //        return fromEncrypt;
        //    }
        //    catch (CryptographicException e)
        //    {
        //        //Console.WriteLine("A Cryptographic error occurred: {0}", e.Message);
        //        return null;
        //    }
        //}

    }
}
