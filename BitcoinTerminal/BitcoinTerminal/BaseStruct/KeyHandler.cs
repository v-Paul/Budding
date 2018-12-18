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
        private Dictionary<string, double> dickeyValue;
        private Dictionary<string, List<UTXO>> dickeysUtxoList;
        public KeyHandler()
        {
            this.dickeyValue = new Dictionary<string, double>();
            this.dickeysUtxoList = new Dictionary<string, List<UTXO>>();
        }

        public string   GernerateKeypairs()
        {
            DirectoryInfo KeyFolder = new DirectoryInfo(AppSettings.XXPKeysFolder);
            FileInfo[] files = KeyFolder.GetFiles("*.pem");
            int count = files.Length;

            //string str = files[0].FullName;
            string PubKeyname = string.Format("pubkey{0}.pem", count / 2);
            string PriKeyname = string.Format("prikey{0}.pem", count/2);
            string pubPath = Path.Combine(AppSettings.XXPKeysFolder, PubKeyname);
            string priPath = Path.Combine(AppSettings.XXPKeysFolder, PriKeyname);
       
            Cryptor.generateRSAKey2File(pubPath, priPath);
            return PubKeyname;
        }

        public void RefKVFromUtxopool(UTXOPool utxopool)
        {
            DirectoryInfo KeyFolder = new DirectoryInfo(AppSettings.XXPKeysFolder);
            FileInfo[] files = KeyFolder.GetFiles("pubkey?.pem");

            if(this.dickeyValue.Count == 0)
            {// 初始化
                foreach (FileInfo fi in files)
                {
                    this.dickeyValue.Add(fi.Name, 0);
                    List<UTXO> lstKeyUtxo = new List<UTXO>();
                    this.dickeysUtxoList.Add(fi.Name, lstKeyUtxo);
                }
            }
            else
            {//先清空，赋零，
                this.dickeysUtxoList.Clear();
                foreach (FileInfo fi in files)
                {
                    this.dickeyValue[fi.Name] = 0;                   
                    List<UTXO> lstKeyUtxo = new List<UTXO>();
                    this.dickeysUtxoList.Add(fi.Name, lstKeyUtxo);
                }
            }

            // 再根据utxopool重新计算每个key对应的value
            foreach (UTXO utxo in utxopool.getAllUTXO())
            {
                Output output =  utxopool.getTxOutput(utxo);

                foreach (FileInfo fi in files)
                {
                    string pukhash = pubkey2Hash(fi.FullName);
                    if (output.scriptPubKey.IndexOf(pukhash) >= 0)
                    {
                        if(this.dickeyValue.ContainsKey(fi.Name))
                        {
                            double dvalue = 0;
                            this.dickeyValue.TryGetValue(fi.Name, out dvalue);
                            this.dickeyValue[fi.Name] = dvalue + output.value;
                        }
                        else
                        {
                            this.dickeyValue[fi.Name] = output.value;
                        }
                        List<UTXO> lstTemp = new List<UTXO>();
                        this.dickeysUtxoList.TryGetValue(fi.Name, out lstTemp);
                        lstTemp.Add(utxo);
                        this.dickeysUtxoList[fi.Name] = lstTemp;

                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="utxoSinglePool"></param>
        /// <returns></returns>
        public List<PubKeyValue> RefKVFromSigUTxpool(UTXOPool utxoSinglePool)
        {
            DirectoryInfo KeyFolder = new DirectoryInfo(AppSettings.XXPKeysFolder);
            FileInfo[] files = KeyFolder.GetFiles("pubkey?.pem");
            List<PubKeyValue> lstPubKeyValue = new List<PubKeyValue>();
            foreach (UTXO utxo in utxoSinglePool.getAllUTXO())
            {
                Output output = utxoSinglePool.getTxOutput(utxo);

                foreach (FileInfo fi in files)
                {
                    string pukhash = pubkey2Hash(fi.FullName);
                    if (output.scriptPubKey.IndexOf(pukhash) >= 0)
                    {
                        PubKeyValue KV = new PubKeyValue(fi.Name, output.value);
                        lstPubKeyValue.Add(KV);
                        if (this.dickeyValue.ContainsKey(fi.Name))
                        {
                            double dvalue = 0;
                            this.dickeyValue.TryGetValue(fi.Name, out dvalue);
                            this.dickeyValue[fi.Name] = dvalue + output.value;
                        }
                        else
                        {
                            this.dickeyValue[fi.Name] = output.value;
                        }
                        List<UTXO> lstTemp = new List<UTXO>();
                        this.dickeysUtxoList.TryGetValue(fi.Name, out lstTemp);
                        lstTemp.Add(utxo);
                        this.dickeysUtxoList[fi.Name] = lstTemp;

                        break;
                    }
                }
            }

            return lstPubKeyValue;
        }

        public string pubkey2Hash(string pubKeyPath)
        {
            string strPubKeyValue = FileIOHelper.ReadFromText(pubKeyPath);
            string strKeyHash = Cryptor.SHA256(strPubKeyValue, strPubKeyValue.Length);
            return strKeyHash;
        }

        public string pubkey2Script(string pubKeyPath)
        {
           
            string strPubKeyValue = FileIOHelper.ReadFromText(pubKeyPath);
            string strKeyHash = Cryptor.SHA256(strPubKeyValue, strPubKeyValue.Length);
            string strPubScript = string.Format("OP_DUP OP_HASH160 {0} OP_EQUALVERIFY OP_CHECKSIG", strKeyHash);

            return strPubScript;
        }

        public string PubKeyHash2Script(string PubkeyHash)
        {
            string strPubScript = string.Format("OP_DUP OP_HASH160 {0} OP_EQUALVERIFY OP_CHECKSIG", PubkeyHash);

            return strPubScript;
        }

        public Dictionary<string, double> GetDicKeyValue()
        {
            return this.dickeyValue;
        }

        public double GetValue(string key)
        {
            double dVal = 0;
            if (key == ConstHelper.BC_All)
            {
                foreach (var dicItem in this.dickeyValue)
                {

                    dVal += dicItem.Value;
                }
            }
            else
            {
                 this.dickeyValue.TryGetValue(key, out dVal);
            }

            return dVal;
        }

        public string PkName2PriKeyName(string pubKeyName)
        {
            string strPrikeyName = "pri" + pubKeyName.Substring(3);
            return strPrikeyName;
        }
        public Dictionary<UTXO, keyPair> GetUtxoDic(string key)
        {
            Dictionary<UTXO, keyPair> dicUtxoPrikey = new Dictionary<UTXO, keyPair>();

            if (key == ConstHelper.BC_All)
            {
                foreach(var item in this.dickeysUtxoList)
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
                this.dickeysUtxoList.TryGetValue(key, out listUtxo);

                keyPair kpair = new keyPair();
                kpair.PubKeyNmae = key;
                kpair.PriKeyNmae = this.PkName2PriKeyName(key);
                foreach (UTXO utxo in listUtxo)
                {
                    dicUtxoPrikey.Add(utxo, kpair);
                }
            }
            
            return dicUtxoPrikey;
        }

        public string CheckBalance(string strKey, double dPaytoAmount)
        {
            string strRet = ConstHelper.BC_OK;
            if (strKey != ConstHelper.BC_All)
            {
                double dMyval = this.GetValue(strKey);
                if (dMyval < dPaytoAmount)
                {
                    // check all 
                    dMyval = this.GetValue(ConstHelper.BC_All);
                    if (dMyval < dPaytoAmount)
                    {
                        strRet = "not sufficient funds";  
                    }
                    else
                    {
                        strRet = @"Current key not sufficient funds,pls select key <All>";
                    }
                }
            }
            else
            {
                double dMyval = this.GetValue(strKey);
                if (dMyval < dPaytoAmount)
                {
                    strRet = "not sufficient funds";
                }
            }

            return strRet;
        }


        public Dictionary<UTXO, keyPair> FindInputUtxo(string key, double dPaytoAmount, UTXOPool utxopool,ref double inputTotalAmount)
        {
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

            return dicInputUtxo;

        }

    }
}
