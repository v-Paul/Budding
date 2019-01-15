using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;
using VTMC.Utils;
using Newtonsoft.Json;

namespace BaseSturct
{
    /// <summary>
    /// Transaction 中的输入
    /// </summary>
    [Serializable]
    public class Input
    {
        //hash of the Transaction whose output is being used 
        public string  PreTxHash;
        //used output's index in the previous transaction 
        public int OutputIndex;
        // the signature produced to check validity
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public scriptSig ScriptSig;

        // add by fdp 190110, for mutisign
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<scriptSig> lstScriptSig;

        
        public Input(string strHash, int index)
        {

            if (string.IsNullOrEmpty(strHash))
                PreTxHash = string.Empty;
            else
                PreTxHash = strHash;
            OutputIndex = index;

            //ScriptSig = new scriptSig();
            //this.lstScriptSig = new List<scriptSig>();
        }

        public void addSignature(scriptSig Sig)
        {
            if(this.ScriptSig == null)
            {
                ScriptSig = new scriptSig();
            }
            this.ScriptSig = Sig;
        }
        public void addMutiSignTolst(scriptSig Sig)
        {
            if(this.lstScriptSig == null)
            {
                this.lstScriptSig = new List<scriptSig>();
            }
            this.lstScriptSig.Add(Sig);
        }
        public string getrawdata()
        {
            return PreTxHash + OutputIndex.ToString();
        }
       

    }

    [Serializable]
    public class scriptSig
    {
        public string Signature { get; set; }
        public string PubKey { get; set; }
    }

    /// <summary>
    /// Transaction 中的输出
    /// </summary>
    [Serializable]
    public class Output
    {
        /** value in bitcoins of the output */
        public double value;
        /** the address or public key of the recipient */
        public string scriptPubKey;

        public Output(double v, string strAddr)
        {
            value = v;
            scriptPubKey = strAddr;
        }
        public Output()
        {
            value = 0;
            scriptPubKey = string.Empty;
        }

        public List<string> Script2list()
        {
            List<string> lstScript = new List<string>();
            lstScript = this.scriptPubKey.Split(' ').ToList<string>();
            return lstScript;
        }

        public string getPKHashFromScript()
        {
            List<string> lstScript = Script2list();
            var lstPKHash = (from x in lstScript
                             where x.Substring(0, 3) != "OP_"
                             select x).ToList();

            string strPKHash = string.Empty;
            foreach (var item in lstPKHash)
            {
                strPKHash += item + Environment.NewLine;
            }
            return strPKHash;
        }



    }


    /// <summary>
    /// 交易类
    /// </summary>
    [Serializable]
    public class Transaction
    {
        /** hash of the transaction, its unique id */
        public string TxHash;

        public string Version;
        // input tansactions list
        public int inputCount = 0;
        public List<Input> listInputs;
        // output tanscations list
        public int outputCount = 0;
        public List<Output> listOutputs;

        public Transaction()
        {
            listInputs = new List<Input>();
            listOutputs = new List<Output>();
        }

        public Transaction(Transaction tx)
        {
            TxHash = tx.TxHash;
            listInputs = new List<Input>(tx.listInputs);
            listOutputs = new List<Output>(tx.listOutputs);
        }

        public void addInput(string prevTxHash, int outputIndex)
        {
            Input ipEnty = new Input(prevTxHash, outputIndex);

            listInputs.Add(ipEnty);
           
        }

        public void addInput(Input input)
        {
            

            listInputs.Add(input);

        }
        public void addOutput(Output op)
        {
            
            listOutputs.Add(op);
        }
        public void addOutput(double value, string strAddress)
        {
            Output opEnty = new Output(value, strAddress);
            listOutputs.Add(opEnty);
        }

        public void removeInput(int index)
        {
            listInputs.RemoveAt(index);
        }

        public void removeInput(UTXO ut)
        {
            for (int i = 0; i < listInputs.Count(); i++)
            {
                Input inEnty = listInputs[i];
                UTXO u = new UTXO(inEnty.PreTxHash, (uint)inEnty.OutputIndex);
                if (u.Equals(ut))
                {
                    listInputs.RemoveAt(i);            
                    return;
                }
            }
        }

        /// <summary>
        /// input.hash + index + (该笔交易的所有output的value+pub)
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string getRawDataToSign(int index)
        {
            // ith input and all outputs
            // sigdata = input.hash + index + (该笔交易的所有output的value+pub)
            string strSigData;
            if (index > listInputs.Count())
                return null;
            Input inEnty = listInputs[index];
            strSigData = inEnty.PreTxHash + inEnty.OutputIndex;

            string strAllOutput=string.Empty;
            foreach (Output opEnty in listOutputs)
            {
                strAllOutput += opEnty.value + opEnty.scriptPubKey;
            }
            strSigData += strAllOutput;

            string sigDataHash = Cryptor.SHA256(strSigData, strSigData.Length);
            return sigDataHash;
        }

        public void addSignature(scriptSig signature, int index)
        {
            listInputs[index].addSignature(signature);
        }

        /// <summary>
        /// 获取整个交易的string，就当把所有的transaction字段加在一起
        /// </summary>
        /// <returns></returns>
        public string getRawTx()
        {
            string strRawTx = string.Empty;
            string strAllInput = string.Empty;
            foreach (Input input in listInputs)
            {
                // modified by fdp compatiable MultiSign
                if(input.ScriptSig != null)
                {
                    strAllInput += input.PreTxHash + input.OutputIndex + input.ScriptSig.Signature + input.ScriptSig.PubKey;
                }
                else if(input.lstScriptSig != null)
                {
                    strAllInput += input.PreTxHash + input.OutputIndex;
                    foreach (var item in input.lstScriptSig)
                    {
                        string Sign = item.Signature + item.PubKey;
                        strAllInput += Sign;
                    }
                }
                
            }

            string strAllOutput = string.Empty;
            foreach (Output opEnty in listOutputs)
            {
                strAllOutput += opEnty.value + opEnty.scriptPubKey;             
            }
            strRawTx = strAllInput + strAllOutput;
            return strRawTx;
        }

        /// <summary>
        /// 计算当前交易的hash
        /// </summary>
        public void SetTransHash()
        {
            try
            {
                string strTransction = getRawTx();
                this.TxHash = Cryptor.SHA256(strTransction, strTransction.Length);
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorLog(ex.Message);
            }
        }

        public string CalTransHash()
        {
            string strTransction = getRawTx();
            string strTransHash = Cryptor.SHA256(strTransction, strTransction.Length);
            return strTransHash;
        }

        public void setHash(string h)
        {
            this.TxHash = h;
        }

        public string getHash()
        {
            return this.TxHash;
        }

        public List<Input> getInputs()
        {
            return listInputs;
        }

        public List<Output> getOutputs()
        {
            return listOutputs;
        }

        public Input getInput(int index)
        {
            if (index < listInputs.Count())
            {
                return listInputs[index];
            }
            return null;
        }

        public Output getOutput(int index)
        {
            if (index < listOutputs.Count())
            {
                return listOutputs[index];
            }
            return null;
        }

        public int numInputs()
        {
            return listInputs.Count();
        }

        public int numOutputs()
        {
            return listOutputs.Count();
        }



        public void signTrans(string strmyPriKeyPath, string strmyPubkeyValue, int InputIndex)
        {
            string strRawdata = this.getRawDataToSign(InputIndex);
            scriptSig tempScriptSig = new scriptSig();
            tempScriptSig.Signature = Cryptor.rsaPriSign(strRawdata, strmyPriKeyPath);
            tempScriptSig.PubKey = strmyPubkeyValue;
            
            // modified by fdp 190110
            listInputs[InputIndex].addSignature(tempScriptSig);          
            //listInputs[InputIndex].ScriptSig.Signature = Cryptor.rsaPriSign(strRawdata, strmyPriKeyPath);
            //listInputs[InputIndex].ScriptSig.PubKey = strmyPubkeyValue;

        }

        public void MultiSignTrans(string strmyPriKeyPath, string strmyPubkeyValue, int InputIndex)
        {
            string strRawdata = this.getRawDataToSign(InputIndex);
            scriptSig tempScriptSig = new scriptSig();
            tempScriptSig.Signature = Cryptor.rsaPriSign(strRawdata, strmyPriKeyPath);
            tempScriptSig.PubKey = strmyPubkeyValue;
            listInputs[InputIndex].addMutiSignTolst(tempScriptSig);


        }
        public void FinalSetTrans()
        {
            this.Version = AppSettings.ProductVersion;
            this.SetTransHash();
            this.inputCount = this.numInputs();
            this.outputCount = this.numOutputs();

        }

        public override bool Equals(Object other)
        {
            if(other.GetType().Name != typeof(Transaction).Name)
            {
                return false;
            }

            if (other == null)
            {
                return false;
            }
            Transaction otherTx = (Transaction)other;
            // 直接对比hash是否一致
            if (this.getHash() != otherTx.getHash())
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public override int GetHashCode()
        {
            return (TxHash).GetHashCode();
        }


    }
}
