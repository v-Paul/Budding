using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;
using System.Data;
using System.IO;
using VTMC.Utils;
namespace BaseSturct
{

    public class keyPair
    {
        public keyPair()
        {
            PubKeyNmae = string.Empty;
            PriKeyNmae = string.Empty;
        }
        public keyPair(string PKN, string PvKN)
        {
            PubKeyNmae = PKN;
            PriKeyNmae = PvKN;
        }
        public string PubKeyNmae { get; set; }
        public string PriKeyNmae { get; set; }
    }

    public class PubKeyValue
    {
        public string PubKeyNmae { get; set; }
        public double Value { get; set; }

        public PubKeyValue()
        {

        }
        public PubKeyValue(string name, double value)
        {
            PubKeyNmae = name;
            Value = value;
        }
    }

    class KeyHandler
    {
        //private Dictionary<string, double> dickeyValue;
        private Dictionary<string, List<UTXO>> dicComitkeysUtxoList;
        private Dictionary<string, List<UTXO>> dicUnComitkeysUtxoList;
        private Dictionary<string, string> dicKeyHash;
        public KeyHandler()
        {
            //this.dickeyValue = new Dictionary<string, double>();
            this.dicComitkeysUtxoList = new Dictionary<string, List<UTXO>>();
            this.dicUnComitkeysUtxoList = new Dictionary<string, List<UTXO>>();
            this.dicKeyHash = new Dictionary<string, string>();

        }

        public string   GernerateKeypairs()
        {
            LogHelper.WriteMethodLog(true);
            DirectoryInfo KeyFolder = new DirectoryInfo(AppSettings.XXPKeysFolder);
            FileInfo[] files = KeyFolder.GetFiles("*.pem");
            int count = files.Length;

            //string str = files[0].FullName;
            string PubKeyname = string.Format("pubkey{0}.pem", count / 2);
            string PriKeyname = string.Format("prikey{0}.pem", count/2);
            string pubPath = Path.Combine(AppSettings.XXPKeysFolder, PubKeyname);
            string priPath = Path.Combine(AppSettings.XXPKeysFolder, PriKeyname);
       
            Cryptor.generateRSAKey2File(pubPath, priPath);
            this.dicKeyHash.Add(PubKeyname, pubkey2Hash(pubPath));
            List<UTXO> lsteTmp = new List<UTXO>();
            this.dicComitkeysUtxoList.Add(PubKeyname, lsteTmp);
            this.dicUnComitkeysUtxoList.Add(PubKeyname, lsteTmp);
            LogHelper.WriteInfoLog("GernerateKeypairs Create:" + PubKeyname);
            return PubKeyname;
        }
        public string pubkey2Hash(string pubKeyPath)
        {
            LogHelper.WriteMethodLog(true);
            string strPubKeyValue = FileIOHelper.ReadFromText(pubKeyPath);
            string strKeyHash = Cryptor.SHA256(strPubKeyValue, strPubKeyValue.Length);
            LogHelper.WriteMethodLog(false);
            return strKeyHash;
        }

        public string pubkey2Script(string pubKeyPath)
        {
            LogHelper.WriteMethodLog(true);
            string strPubKeyValue = FileIOHelper.ReadFromText(pubKeyPath);
            string strKeyHash = Cryptor.SHA256(strPubKeyValue, strPubKeyValue.Length);
            string strPubScript = string.Format("OP_DUP OP_HASH160 {0} OP_EQUALVERIFY OP_CHECKSIG", strKeyHash);
            LogHelper.WriteMethodLog(false);
            return strPubScript;
        }

        public string PubKeyHash2Script(string PubkeyHash)
        {
            LogHelper.WriteMethodLog(true);
            string strPubScript = string.Format("OP_DUP OP_HASH160 {0} OP_EQUALVERIFY OP_CHECKSIG", PubkeyHash);
            LogHelper.WriteMethodLog(false);
            return strPubScript;
        }

        public string PkName2PriKeyName(string pubKeyName)
        {
            LogHelper.WriteMethodLog(true);
            string strPrikeyName = "pri" + pubKeyName.Substring(3);
            LogHelper.WriteMethodLog(false);
            return strPrikeyName;

        }
        public void RefKUtxoList(bool bCommited, UTXOPool utxopool)
        {
            LogHelper.WriteMethodLog(true);
            Dictionary<string, List<UTXO>> keysUtxoList = new Dictionary<string, List<UTXO>>();
            if(bCommited)
            { keysUtxoList = this.dicComitkeysUtxoList; }
            else
            { keysUtxoList = this.dicUnComitkeysUtxoList; }



            DirectoryInfo KeyFolder = new DirectoryInfo(AppSettings.XXPKeysFolder);
            FileInfo[] files = KeyFolder.GetFiles("pubkey?.pem");

            if(keysUtxoList.Count == 0)
            {// 初始化
                foreach (FileInfo fi in files)
                {
                    //this.dickeyValue.Add(fi.Name, 0);
                    if(!this.dicKeyHash.ContainsKey(fi.Name))
                    {
                        this.dicKeyHash.Add(fi.Name, this.pubkey2Hash(fi.FullName));
                    }
                   
                    List<UTXO> lstKeyUtxo = new List<UTXO>();
                    keysUtxoList.Add(fi.Name, lstKeyUtxo);
                }
            }
            else
            {//先清空，赋零，
                keysUtxoList.Clear();
                foreach (FileInfo fi in files)
                {
                    //this.dickeyValue[fi.Name] = 0;                   
                    List<UTXO> lstKeyUtxo = new List<UTXO>();
                    keysUtxoList.Add(fi.Name, lstKeyUtxo);
                }
            }

            // 再根据utxopool重新计算每个key对应的value
            foreach (UTXO utxo in utxopool.getAllUTXO())
            {
                Output output =  utxopool.getTxOutput(utxo);

                foreach (FileInfo fi in files)
                {
                    string pukhash = string.Empty;
                    this.dicKeyHash.TryGetValue(fi.Name, out pukhash);
                    if (output.scriptPubKey.IndexOf(pukhash) >= 0 && output.scriptPubKey.IndexOf("OP_CHECKMULTISIG") <0 )
                    {
                        //deleted by fdp 181224 key corresponding value calculate from utxo list
                        //if(this.dickeyValue.ContainsKey(fi.Name))
                        //{
                        //    double dvalue = 0;
                        //    this.dickeyValue.TryGetValue(fi.Name, out dvalue);
                        //    this.dickeyValue[fi.Name] = dvalue + output.value;
                        //}
                        //else
                        //{
                        //    this.dickeyValue[fi.Name] = output.value;
                        //}
                        List<UTXO> lstTemp = new List<UTXO>();
                        keysUtxoList.TryGetValue(fi.Name, out lstTemp);
                        lstTemp.Add(utxo);
                        keysUtxoList[fi.Name] = lstTemp;

                        break;
                    }
                }
            }



            LogHelper.WriteMethodLog(false);
        }

        /// <summary>
        /// 仅用来提示哪个key在这个block里收到多少钱
        /// </summary>
        /// <param name="utxoSinglePool"></param>
        /// <returns></returns>
        //public List<PubKeyValue> RefKVFromSigUTxpool(UTXOPool utxoSinglePool)
        //{
        //    LogHelper.WriteMethodLog(true);
        //    DirectoryInfo KeyFolder = new DirectoryInfo(AppSettings.XXPKeysFolder);
        //    FileInfo[] files = KeyFolder.GetFiles("pubkey?.pem");
        //    List<PubKeyValue> lstPubKeyValue = new List<PubKeyValue>();
        //    foreach (UTXO utxo in utxoSinglePool.getAllUTXO())
        //    {
        //        Output output = utxoSinglePool.getTxOutput(utxo);

        //        foreach (FileInfo fi in files)
        //        {
        //            string pukhash = string.Empty;
        //            this.dicKey2Hash.TryGetValue(fi.Name, out pukhash);
        //            if (output.scriptPubKey.IndexOf(pukhash) >= 0)
        //            {
        //                PubKeyValue KV = new PubKeyValue(fi.Name, output.value);
        //                lstPubKeyValue.Add(KV);
        //                if (this.dickeyValue.ContainsKey(fi.Name))
        //                {
        //                    double dvalue = 0;
        //                    this.dickeyValue.TryGetValue(fi.Name, out dvalue);
        //                    this.dickeyValue[fi.Name] = dvalue + output.value;
        //                }
        //                else
        //                {
        //                    this.dickeyValue[fi.Name] = output.value;
        //                }
        //                // deleted by fdp 这样会导致key对应的utxo list 只增没减，导致金额显示不平
        //                //List<UTXO> lstTemp = new List<UTXO>();
        //                //this.dickeysUtxoList.TryGetValue(fi.Name, out lstTemp);
        //                //lstTemp.Add(utxo);
        //                //this.dickeysUtxoList[fi.Name] = lstTemp;

        //                break;
        //            }
        //        }
        //    }
        //    LogHelper.WriteMethodLog(false);
        //    return lstPubKeyValue;
        //}

        public Dictionary<string, double> GetDicKeyValue(bool bCommited, UTXOPool utxopool)
        {
            LogHelper.WriteMethodLog(true);
            Dictionary<string, double> dickeyValue = new Dictionary<string, double>();
            Dictionary<string, List<UTXO>> keysUtxoList = new Dictionary<string, List<UTXO>>();
            if (bCommited)
            { keysUtxoList = this.dicComitkeysUtxoList; }
            else
            { keysUtxoList = this.dicUnComitkeysUtxoList; }
            foreach (var item in keysUtxoList)
            {
                double dValue = 0;
                foreach (var utxo in item.Value)
                {
                    Output output = utxopool.getTxOutput(utxo);
                    dValue += output.value;
                }
                dickeyValue.Add(item.Key, dValue);
            }

            string logKeyValue = string.Empty;
            foreach (var item in dickeyValue)
            {
                logKeyValue += string.Format("{0}:{1}", item.Key, item.Value.ToString("0.0000")) + Environment.NewLine;
            }
            LogHelper.WriteInfoLog(logKeyValue);

            return dickeyValue;
        }

        public double GetValue(bool bCommited, string key, UTXOPool utxopool)
        {
            LogHelper.WriteMethodLog(true);
            double dVal = 0;
            if (key == ConstHelper.BC_All)
            {
                
                Dictionary<string, double> dickeyValue = GetDicKeyValue(bCommited, utxopool);
                foreach (var dicItem in dickeyValue)
                {
                    dVal += dicItem.Value;
                }
            }
            else
            {
                List<UTXO> UtxoList = new List<UTXO>();
                if (bCommited)
                { this.dicComitkeysUtxoList.TryGetValue(key, out UtxoList); }
                else
                { this.dicUnComitkeysUtxoList.TryGetValue(key, out UtxoList); }

                foreach (var item in UtxoList)
                {
                    Output output = utxopool.getTxOutput(item);
                    dVal += output.value;
                }

            }

            LogHelper.WriteInfoLog(string.Format("IsCommited{0} {1}:{2} ", bCommited, key, dVal));
            return dVal;
        }


        public Dictionary<UTXO, keyPair> GetUtxoDic(string key)
        {
            LogHelper.WriteMethodLog(true);
            Dictionary<UTXO, keyPair> dicUtxoPrikey = new Dictionary<UTXO, keyPair>();

            if (key == ConstHelper.BC_All)
            {
                foreach(var item in this.dicComitkeysUtxoList)
                {
                    keyPair kpair = new keyPair();
                    kpair.PubKeyNmae = item.Key;
                    kpair.PriKeyNmae = this.PkName2PriKeyName(item.Key);
                    foreach (UTXO utxo in item.Value)
                    {
                        dicUtxoPrikey.Add(utxo, kpair);
                    }
                }
            }
            else
            {
                List<UTXO> listUtxo = new List<UTXO>();
                this.dicComitkeysUtxoList.TryGetValue(key, out listUtxo);

                keyPair kpair = new keyPair();
                kpair.PubKeyNmae = key;
                kpair.PriKeyNmae = this.PkName2PriKeyName(key);
                foreach (UTXO utxo in listUtxo)
                {
                    dicUtxoPrikey.Add(utxo, kpair);
                }
            }

            LogHelper.WriteMethodLog(false);
            return dicUtxoPrikey;
        }

        public string CheckBalance(string strKey, double dPaytoAmount, UTXOPool utxopool)
        {
            LogHelper.WriteMethodLog(true);
            string strRet = ConstHelper.BC_OK;
            if(dPaytoAmount == 0)
            {
                strRet = "You want to trap me, NOWAY!";
            }

            if (strKey != ConstHelper.BC_All)
            {
                double dMyval = this.GetValue(true, strKey, utxopool);
                if (dMyval < dPaytoAmount)
                {
                    // check all 
                    dMyval = this.GetValue(true, ConstHelper.BC_All, utxopool);
                    if (dMyval < dPaytoAmount)
                    {
                        strRet = "Insufficient balance";  
                    }
                    else
                    {
                        strRet = @"Current key's balance is insufficient,pls select key <All>";
                    }
                }
            }
            else
            {
                double dMyval = this.GetValue(true, strKey, utxopool);
                if (dMyval < dPaytoAmount)
                {
                    strRet = "Insufficient balance";
                }
            }
            LogHelper.WriteInfoLog("CheckBalance result: " + strRet);
            return strRet;
        }


        public Dictionary<UTXO, keyPair> FindInputUtxo(string key, double dPaytoAmount, UTXOPool utxopool,ref double inputTotalAmount)
        {
            LogHelper.WriteMethodLog(true);
            Dictionary<UTXO, keyPair> dicKeyAllUtxo = this.GetUtxoDic(key);
            Dictionary<UTXO, keyPair> dicInputUtxo = new Dictionary<UTXO, keyPair>();
            double sumValue = 0;
            foreach(var item in dicKeyAllUtxo)
            {
                dicInputUtxo.Add(item.Key, item.Value);

                Output op = utxopool.getTxOutput(item.Key);
                sumValue += op.value;
                if (sumValue >= dPaytoAmount)
                {
                    inputTotalAmount = sumValue;
                    break;
                }
            }
            LogHelper.WriteMethodLog(false);
            return dicInputUtxo;

        }

        public Dictionary<string, string > GetDicKeyHash()
        {
            return this.dicKeyHash;
        }
        public int GetDicKeyCount()
        {
            return this.dicKeyHash.Count;
        }

        public string GetKeyHash(string keyname)
        {
            LogHelper.WriteMethodInfoLog(keyname);
            string hash = string.Empty;
            if (keyname == ConstHelper.BC_All)
            {
                if( this.dicKeyHash.Count != 0)
                {
                    KeyValuePair<string, string> firstKV = this.dicKeyHash.First();
                    hash = firstKV.Value;
                }
                
            }
            else
            {
                this.dicKeyHash.TryGetValue(keyname, out hash);
            }
            LogHelper.WriteInfoLog(hash);
            return hash;
        }

        public int GetdicPkHKeypair(ref Dictionary<string, keyPair> dicPkHKeypair)
        {
            foreach (var item in this.dicKeyHash)
            {

                keyPair kp =  new keyPair(item.Key, this.PkName2PriKeyName(item.Key));
                dicPkHKeypair.Add(item.Value, kp);
            }
            return 0;
        }
    }
}
