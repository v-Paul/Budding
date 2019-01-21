using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;
using VTMC.Utils;
using System.IO;
namespace BaseSturct
{

    public class TxHandler
    {

        /**
         * Creates a public ledger whose current UTXOPool (collection of unspent transaction outputs) is
         * {@code utxoPool}. This should make a copy of utxoPool by using the UTXOPool(UTXOPool uPool)
         * constructor.
         */
        private UTXOPool CommitedUtxoPool;
        private UTXOPool UnCommitedUtxoPool;
        private List<UTXO> tempUTXOList;
        public TxHandler()
        {
            // IMPLEMENT THIS
            this.CommitedUtxoPool = new UTXOPool();
            this.UnCommitedUtxoPool = new UTXOPool();
            this.tempUTXOList = new List<UTXO>();
        }


        public string CreatUTPoolFromDB(string strDBpath)
        {
            try
            {
                string strRet = LeveldbOperator.OpenDB(strDBpath);
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
                Block tempBlock = JsonHelper.Deserialize<Block>(blockValue);
                this.BlockData2UTXOPool(true, tempBlock);

                while (true)
                {
                    strRet = LeveldbOperator.GetNextKey();
                    if (string.IsNullOrEmpty(strRet))
                    {
                        break;
                    }
                    blockValue = LeveldbOperator.GetValue(strRet);
                    tempBlock = JsonHelper.Deserialize<Block>(blockValue);
                    this.BlockData2UTXOPool(true, tempBlock);
                }

                if (this.tempUTXOList.Count != 0)
                {
                    foreach (UTXO utxo in this.tempUTXOList)
                    {
                        if (this.CommitedUtxoPool.contains(utxo))
                        {
                            this.CommitedUtxoPool.removeUTXO(utxo);
                        }
                    }
                }

                return ConstHelper.BC_OK;
            }
            catch(Exception ex)
            {
                LogHelper.WriteErrorLog(ex.Message);
                return "exception";
            }
            finally
            {
                LeveldbOperator.CloseDB();
            }

            
        }

        public void RefreshUTPoolByBlock(Block block)
        {
            LogHelper.WriteMethodInfoLog(true);
            this.BlockData2UTXOPool(true, block);
            LogHelper.WriteMethodInfoLog(false);
        }


        public UTXOPool GetUtxoPool(bool Iscommited)
        {
            if(Iscommited)
            {
                return this.CommitedUtxoPool;
            }
            else
            {
                return this.UnCommitedUtxoPool;
            }
           
        }


        public UTXO input2UTXO(Input einput)
        {
            UTXO utxo = new UTXO(einput.PreTxHash, (uint)einput.OutputIndex);
            return utxo;
        }


        /// <summary>
        /// 根据block信息更新UTXO pool，同时返回该block生成的utxo list，用来提示该block收到了多少钱
        /// </summary>
        /// <param name="block"></param>
        /// <returns>just for update key corresponding value</returns>
        public UTXOPool BlockData2UTXOPool(bool bCommitedPool, Block block)
        {
            LogHelper.WriteMethodLog(true);
            LogHelper.WriteInfoLog("bCommitedPool: " + bCommitedPool.ToString());
            UTXOPool utxopoll = new UTXOPool();
            if (bCommitedPool)
            { utxopoll = this.CommitedUtxoPool; }
            else
            { utxopoll = this.UnCommitedUtxoPool; }



            UTXOPool sigleBlockPool = new UTXOPool();
            foreach (Transaction eTransaction in block.listTransactions)
            {
                foreach (Input eInput in eTransaction.listInputs)
                {
                    UTXO utxo = this.input2UTXO(eInput);
                    if (utxopoll.contains(utxo))
                    {
                        utxopoll.removeUTXO(utxo);
                        
                    }
                    else
                    {
                        // 初始化时由于leveldb 树形结构第一个可能创世块，所以先记下来在后面再remove
                        if(utxo.getTxHash() != ConstHelper.BC_BaseCoinInputTxHash)
                        {
                            tempUTXOList.Add(utxo);
                        }                       
                    }

                }
                
                for (int i = 0; i < eTransaction.listOutputs.Count; i++)
                {
                    // hash is transaction hash
                    UTXO utxo1 = new UTXO(eTransaction.getHash(), (uint)i);

                    utxopoll.addUTXO(utxo1, eTransaction.listOutputs[i]);
                    sigleBlockPool.addUTXO(utxo1, eTransaction.listOutputs[i]);
                }
            }
            LogHelper.WriteMethodLog(false);
            return sigleBlockPool;
        }





        /// <summary>
        /// abandon
        /// </summary>
        /// <param name="strPreHash"></param>
        /// <param name="iIndex"></param>
        /// <param name="dValue"></param>
        /// <param name="strPaytoPKHash"></param>
        /// <param name="myPriKeyPath"></param>
        /// <param name="myPubkeyPath"></param>
        /// <returns></returns>
        //public Transaction CreatTransaction(string strPreHash, uint iIndex, double dValue, 
        //                                    string strPaytoPKHash,string myPriKeyPath,string myPubkeyPath)
        //{
        //    LogHelper.WriteMethodLog(true);
            
        //    string strPubKeyValuem = FileIOHelper.ReadFromText(myPubkeyPath);
        //    string strKeyHash = Cryptor.SHA256(strPubKeyValuem, strPubKeyValuem.Length);
        //    string myPubScript = string.Format("OP_DUP OP_HASH160 {0} OP_EQUALVERIFY OP_CHECKSIG", strKeyHash);


        //    UTXO utxo = new UTXO(strPreHash, iIndex);
        //    if (!this.CommitedUtxoPool.contains(utxo))
        //    {
        //        LogHelper.WriteErrorLog("not in utxo");
        //        return null;
        //    }

        //    Transaction spendTrans = new Transaction();
        //    spendTrans.addInput(strPreHash, (int)iIndex);
        //    spendTrans.addOutput(dValue, strPaytoPKHash);

        //    Output preOutput = this.CommitedUtxoPool.getTxOutput(utxo);
        //    if (dValue < preOutput.value)
        //    {
        //        Output ctOutput = new Output();
        //        ctOutput.value = preOutput.value - dValue;
        //        ctOutput.scriptPubKey = myPubScript;
        //        spendTrans.addOutput(ctOutput);
        //    }
        //    spendTrans.signTrans(myPriKeyPath, strPubKeyValuem);
        //    spendTrans.FinalSetTrans();

        //    LogHelper.WriteInfoLog("CreatTransaction: " + spendTrans.TxHash);
        //    return spendTrans;
        //}


        public Transaction CreatTransaction(Dictionary<UTXO, keyPair> dicUTXO, double dInputAmount, double dPayToValue,
                                            string strPaytoPKHash, string changePubScript)
        {
            LogHelper.WriteMethodLog(true);

            Transaction spendTrans = new Transaction();

            // 为了计算sign方便add output
            spendTrans.addOutput(dPayToValue, strPaytoPKHash);

            if (dPayToValue < dInputAmount)
            {
                Output ctOutput = new Output();
                ctOutput.value = dInputAmount - dPayToValue;
                ctOutput.scriptPubKey = changePubScript;
                spendTrans.addOutput(ctOutput);
            }

            foreach (var item in dicUTXO)
            {
                if (!this.CommitedUtxoPool.contains(item.Key))
                {
                    LogHelper.WriteErrorLog("not in utxo");
                    LogHelper.WriteErrorLog(JsonHelper.Serializer<UTXO>(item.Key));
                    return null;
                }
                else
                {
                    Input input = new Input(item.Key.getTxHash(), (int)item.Key.getIndex());
                    spendTrans.addInput(input);
                    string strPriKPath = Path.Combine(AppSettings.XXPKeysFolder, item.Value.PriKeyNmae);
                    string strPubKPath = Path.Combine(AppSettings.XXPKeysFolder, item.Value.PubKeyNmae);
                    string strPubValue = FileIOHelper.ReadFromText(strPubKPath);
                    spendTrans.signTrans(strPriKPath, strPubValue, spendTrans.listInputs.IndexOf(input));
                }
            }

            
            spendTrans.FinalSetTrans();
            LogHelper.WriteInfoLog("CreatTransaction: " + spendTrans.TxHash);
            return spendTrans;
        }




        /**
         * @return true if:
         * (1) all outputs claimed by {@code tx} are in the current UTXO pool, 
         * (2) the signatures on each input of {@code tx} are valid, 
         * (3) no UTXO is claimed multiple times by {@code tx},
         * (4) all of {@code tx}s output values are non-negative, and
         * (5) the sum of {@code tx}s input values is greater than or equal to the sum of its output
         *     values; and false otherwise.
         */
        public bool isValidTx(Transaction tx)
        {
            LogHelper.WriteMethodLog(true);
            if(tx.listInputs.Count==0 || tx.listOutputs.Count == 0)
            {
                LogHelper.WriteInfoLog("empty input|output");
                return false;
            }

            // IMPLEMENT THIS
            double sumOut = 0;
            double sumIn = 0;

            Dictionary<string, UTXO> dicUsed = new Dictionary<string, UTXO>();

            for (int i = 0; i < tx.numInputs(); i++)
            {
                Input input = tx.getInput(i);

                UTXO utxo = new UTXO(input.PreTxHash, (uint)input.OutputIndex);
                if (!CommitedUtxoPool.contains(utxo))
                {
                    LogHelper.WriteInfoLog(" utxoPool not contain utxo:");
                    return false; //check (1),utox 包含该交易返回false
                }

                Output PreOutput = CommitedUtxoPool.getTxOutput(utxo);// the consume coin correspond prev output coin;
                sumIn += PreOutput.value;//(5) 计算input 指向的pre output 的value，最后保证输入的value等于该笔交易输出的
                string strOriginalTxt = tx.getRawDataToSign(i);

                // add by fdp compatible MultiSign 190114
                bool bVfRet = false;
                if(input.ScriptSig != null)
                {
                    bVfRet = Cryptor.VerifySignature(input.ScriptSig, PreOutput.scriptPubKey, strOriginalTxt);
                }
                else if(input.lstScriptSig != null)
                {
                    bVfRet = Cryptor.VerifyMultiSignature(input.lstScriptSig, PreOutput.scriptPubKey, strOriginalTxt);
                }
                if (!bVfRet)
                {
                    LogHelper.WriteInfoLog(" VerifySignature fail");
                    return false;//check(2) 
                }


                bool bIsContain = dicUsed.ContainsKey(utxo.utoxHashCode());
                if(!bIsContain) // UTXO不会被重复添加
                {
                    dicUsed.Add(utxo.utoxHashCode(), utxo);
                }
                else
                {
                    LogHelper.WriteInfoLog(" double spend :" + utxo.utoxHashCode());
                    return false;
                }   
            }
            foreach (Output output in tx.getOutputs())
            {
                if (output.value < 0)
                {
                    LogHelper.WriteInfoLog(" output.value < 0 ");
                    return false;//check(5)
                }
                sumOut += output.value;
            }
            if (sumIn < sumOut)
            {
                LogHelper.WriteInfoLog(" sumIn < sumOut ");
                return false;//check(5);
            }
            LogHelper.WriteInfoLog("Valid Tx");
            return true;
        }



        /**
         * Handles each epoch by receiving an unordered array of proposed transactions, checking each
         * transaction for correctness, returning a mutually valid array of accepted transactions, and
         * updating the current UTXO pool as appropriate.
         */
        public Transaction[] handleTxs(List<Transaction> possibleTxs)
        {

            // IMPLEMENT THIS    	
            HashSet<Transaction> txVis = new HashSet<Transaction>();
            //fixed point algorithm,iter untill no new transaction is valid
            while (true)
            {
                bool updated = false;
                foreach (Transaction tx in possibleTxs)
                {
                    if (txVis.Contains(tx)) continue;
                    if (isValidTx(tx))
                    {
                        txVis.Add(tx);
                        updated = true;
                        //add unspent coin
                        for (uint i = 0; i < tx.numOutputs(); ++i)
                        {
                            UTXO utxo = new UTXO(tx.getHash(), i);
                            CommitedUtxoPool.addUTXO(utxo, tx.getOutput((int)i));
                        }
                        //delete spent coin
                        for (int i = 0; i < tx.numInputs(); ++i)
                        {
                            Input input = tx.getInput(i);
                            UTXO utxo = new UTXO(input.PreTxHash, (uint)input.OutputIndex);
                            CommitedUtxoPool.removeUTXO(utxo);
                        }
                    }
                }
                if (!updated) break;
            };
            Transaction[] ret = new Transaction[txVis.Count()];
            int idx = 0;
            foreach (Transaction tx in txVis)
                ret[idx++] = tx;
            return ret;

        }

        public string  handleTxs(Transaction tx)
        {
            try
            {
                string sRet = ConstHelper.BC_OK;
                if (isValidTx(tx))
                {
                    //add unspent coin
                    for (uint i = 0; i < tx.numOutputs(); ++i)
                    {
                        UTXO utxo = new UTXO(tx.getHash(), i);
                        UnCommitedUtxoPool.addUTXO(utxo, tx.getOutput((int)i));
                        LogHelper.WriteInfoLog(string.Format("Add utxo to uncommited pool, utxoHash:{0}", utxo.utoxHashCode() ));
                    }
                    //delete spent coin
                    for (int i = 0; i < tx.numInputs(); ++i)
                    {
                        Input input = tx.getInput(i);
                        UTXO utxo = new UTXO(input.PreTxHash, (uint)input.OutputIndex);
                        CommitedUtxoPool.removeUTXO(utxo);
                        LogHelper.WriteInfoLog(string.Format("Remove utxo from commited pool, utxoHash:{0}",  utxo.utoxHashCode()));
                    }
                }
                else
                {
                    sRet = "invalid transaction";
                }
                LogHelper.WriteInfoLog("handleTxs result:" + sRet);
                return sRet;
            }
            catch(Exception ex)
            {
                LogHelper.WriteErrorLog(ex.Message);
                return "HandleTxs catch an exception ";
            }
    
        }



        public void AddBaseCoin2UTxoPool(Transaction basetx)
        {
            try
            {

                UTXO utxo = new UTXO(basetx.getHash(), 0);
                CommitedUtxoPool.addUTXO(utxo, basetx.getOutput((int)0));
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorLog(ex.Message);
            }

        }

        public void ClearUnCommitUtxoPool()
        {
            this.UnCommitedUtxoPool.clearUtxoPool();
        }


        #region MutiSign function
        /// <summary>
        /// 
        /// </summary>
        /// <param name="N"></param>
        /// <param name="M"></param>
        /// <param name="lstPKHash"></param>
        /// <param name="Script"></param>
        /// <returns>0：成功 -1：PKHash个数和M不相等 -2：不满足 1<N<M<10 -3:PkHash 长度错 </returns>
        public string  AssumbleMutiSignScript(int N, int M, List<string> lstPKHash, ref string Script)
        {
            Script = string.Empty;
            if(M != lstPKHash.Count)
            {
                return "The number of PkHash is not equal to M, multiple PkHash , please separate by space ";
            }

            if (1 < N && N <= M && M < 10)
            {
                Script = "OP_" + N.ToString();
                foreach (var item in lstPKHash)
                {
                    if(item.Length != 64)
                    {
                        Script = string.Empty;
                        return string.Format("{0} \r\n PkHash length error", item);
                    }
                    Script += " " + item;
                }
                Script += " " + "OP_" + M.ToString();
                Script += " " + "OP_CHECKMULTISIG";
            }
            else
            {
                return "You are creating a MultiSing Tx, N should less-than M, or set N=M=0,create a single sign Tx  ";
            }

            return ConstHelper.BC_OK;
        }

        #region 暂时不用
        // 暂不使用
        public bool CreateInputChangeOutput(Dictionary<UTXO, keyPair> dicUTXO, double dInputAmount, double dPayToValue,
                                           string strPaytoPKHash, string changePubScript, 
                                           ref List<Input>lstInput, ref List<Output>lstOutput)
        {
            LogHelper.WriteMethodLog(true);

            foreach (var item in dicUTXO)
            {
                if (!this.CommitedUtxoPool.contains(item.Key))
                {
                    LogHelper.WriteErrorLog("not in utxo pool");
                    LogHelper.WriteErrorLog(JsonHelper.Serializer<UTXO>(item.Key));
                    return false;
                }
                else
                {
                    Input input = new Input(item.Key.getTxHash(), (int)item.Key.getIndex());
                    lstInput.Add(input);
                }
            }

            Output eOutPut = new Output(dPayToValue, strPaytoPKHash);
            lstOutput.Add(eOutPut);
            if (dPayToValue < dInputAmount)
            {
                Output ctOutput = new Output();
                ctOutput.value = dInputAmount - dPayToValue;
                ctOutput.scriptPubKey = changePubScript;
                lstOutput.Add(ctOutput);
            }

            LogHelper.WriteInfoLog("Input list: " + JsonHelper.Serializer<List<Input>>(lstInput));
            LogHelper.WriteInfoLog("Output list: " + JsonHelper.Serializer<List<Output>>(lstOutput));
            LogHelper.WriteMethodLog(false);
            return true;
        }

        //暂时不用
        public bool SignPrimitiveTx(Dictionary<UTXO, keyPair> dicUTXO, ref Transaction PrimitiveTx)
        {
            int iInputCount = PrimitiveTx.listInputs.Count;
            for (int i = 0; i < iInputCount; i++)
            {
                if (PrimitiveTx.listInputs[i].ScriptSig == null)
                {
                    UTXO utxo = new UTXO(PrimitiveTx.listInputs[i].PreTxHash, (uint)PrimitiveTx.listInputs[i].OutputIndex);
                    if (dicUTXO.ContainsKey(utxo))
                    {
                        keyPair kp = new keyPair();
                        dicUTXO.TryGetValue(utxo, out kp);
                        string strPriKPath = Path.Combine(AppSettings.XXPKeysFolder, kp.PriKeyNmae);
                        string strPubKPath = Path.Combine(AppSettings.XXPKeysFolder, kp.PubKeyNmae);
                        string strPubValue = FileIOHelper.ReadFromText(strPubKPath);

                        PrimitiveTx.signTrans(strPriKPath, strPubValue, i);
                    }
                }
            }

            return true;
        }

        #endregion

        public string CreatPrimitiveTx( List<Input> lstInput, double dValue, string strPay2Hash, ref Transaction PrimitiveTx)
        {
            LogHelper.WriteMethodLog(true);

           
            double inputValue = 0;
            foreach (var item in lstInput)
            {
                UTXO utxo = new UTXO(item.PreTxHash, (uint)item.OutputIndex);
                Output tempoutput = this.CommitedUtxoPool.getTxOutput(utxo);
                inputValue += tempoutput.value;
            }
            if(inputValue != dValue)
            {
                LogHelper.WriteErrorLog(string.Format("Input Utxo's value not equal with output value, inputV:{0}, outputV:{1}", inputValue, dValue));
                return "Input Utxo's value not equal with output value";
            }
          
            PrimitiveTx.listInputs = lstInput;
            string Pay2Script = string.Format("OP_DUP OP_HASH160 {0} OP_EQUALVERIFY OP_CHECKSIG", strPay2Hash);
            Output output = new Output(dValue, Pay2Script);
            PrimitiveTx.addOutput(output);

            LogHelper.WriteInfoLog("MultiSign TX: " + JsonHelper.Serializer<Transaction>(PrimitiveTx));
            LogHelper.WriteMethodLog(false);
            return ConstHelper.BC_OK;
        }


        public string SignPrimitiveTx(Dictionary<string, keyPair> KeyHashKeypair, ref Transaction PrimitiveTx)
        {
            LogHelper.WriteMethodLog(true);
            bool bHaveSigned = true;
            foreach (var item in PrimitiveTx.listInputs)
            {
                // 只要input有一个没有签名list，就认为没有签名
                if(item.lstScriptSig == null)
                {
                    bHaveSigned = false;
                    break;
                }
                else
                {   // 有签名list后 判断自己是否已经签名，
                    bHaveSigned = false;
                    foreach (var item1 in item.lstScriptSig)
                    {
                        string strKeyHash = Cryptor.SHA256(item1.PubKey, item1.PubKey.Length);
                        if(KeyHashKeypair.ContainsKey(strKeyHash))
                        {
                            bHaveSigned = true;
                        }
                    }
                }
            }
            if(bHaveSigned)
            {
                return "Current Primitive Tx'inputs Have signed";
            }

            int iInputCount = PrimitiveTx.listInputs.Count;

            for (int i = 0; i < iInputCount; i++)
            {
                UTXO utxo = new UTXO(PrimitiveTx.listInputs[i].PreTxHash, (uint)PrimitiveTx.listInputs[i].OutputIndex);

                if (!CommitedUtxoPool.contains(utxo))
                {
                    LogHelper.WriteInfoLog(" utxoPool not contain utxo:");
                    return "Invalid utxo"; //check (1),utox 包含该交易返回false
                }
                Output output = CommitedUtxoPool.getTxOutput(utxo);
                List<string> lstScript = output.scriptPubKey.Split(' ').ToList<string>();

                var lstPKHash = (from x in lstScript
                                 where x.Substring(0,3) != "OP_"
                                 select x).ToList();


                foreach (var item in lstPKHash)
                {
                    if(KeyHashKeypair.ContainsKey(item))
                    {
                        keyPair kp = new keyPair();
                        KeyHashKeypair.TryGetValue(item, out kp);
                        string strPriKPath = Path.Combine(AppSettings.XXPKeysFolder, kp.PriKeyNmae);
                        string strPubKPath = Path.Combine(AppSettings.XXPKeysFolder, kp.PubKeyNmae);
                        string strPubValue = FileIOHelper.ReadFromText(strPubKPath);
                        PrimitiveTx.MultiSignTrans(strPriKPath, strPubValue, i);
                    }
                    
                }
            }
            LogHelper.WriteMethodLog(false);
            return ConstHelper.BC_OK;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="signedPrimitiveTx"></param>
        /// <returns>0:succ, -1: not multisign, -2:</returns>
        public string CreateRedeemTx(ref Transaction signedPrimitiveTx)
        {
            LogHelper.WriteMethodLog(true);
            string strErrorMsg = string.Empty;
            int iRet = this.CheckMultiSignCount(signedPrimitiveTx, ref strErrorMsg);
            if(iRet != 0)
            {
                return strErrorMsg;
            }

            signedPrimitiveTx.FinalSetTrans();

            LogHelper.WriteMethodLog(false);
            return ConstHelper.BC_OK;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="signedPrimitiveTx"></param>
        /// <returns>0:succ, -1:not multiSign, -2:sign not enough -3: signature more than M </returns>
        public int CheckMultiSignCount(Transaction signedPrimitiveTx, ref string errorMsg)
        {
            int i = 0;
            foreach (var item in signedPrimitiveTx.listInputs)
            {

                UTXO utxo1 = new UTXO(item.PreTxHash, (uint)item.OutputIndex);
                Output output1 = this.CommitedUtxoPool.getTxOutput(utxo1);
                var lstScript = output1.scriptPubKey.Split(' ').ToList<string>();
                if (lstScript.Last() != "OP_CHECKMULTISIG")
                {
                    LogHelper.WriteErrorLog("input isn't MultiSign Tx ");
                    errorMsg = "one of inputs isn't MultiSign Tx";
                    return -1;
                }
                string OP_N = lstScript.First();
                string OP_M = lstScript[lstScript.Count - 2];
                int N = int.Parse(OP_N.Substring(3));
                int M = int.Parse(OP_M.Substring(3));
                if (item.lstScriptSig.Count < N )
                {
                    LogHelper.WriteErrorLog(string.Format("input[{0}] signature count not meet mini request",i));
                    errorMsg = string.Format("input:[{0}]\r\n is a {1}/{2} Tx.\r\nThe number of signatures({3}) did not meet the minimum requirements",
                                             item.PreTxHash, N,M, item.lstScriptSig.Count);
                    return -2;
                }
                else if(item.lstScriptSig.Count > M)
                {
                    LogHelper.WriteErrorLog(string.Format("input[{0}] signature more than M", i));
                    errorMsg = "The number of signatures more than M";
                    return -3;
                }
                i++;
            }

            return 0;
        }
        #endregion


    }
}
