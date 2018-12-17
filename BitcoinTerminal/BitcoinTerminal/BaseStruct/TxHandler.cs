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
        private UTXOPool utxoPool;
        private List<UTXO> tempUTXOList;
        public TxHandler()
        {
            // IMPLEMENT THIS
            this.utxoPool = new UTXOPool();
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
                this.BlockData2UTXOPool(tempBlock);

                while (true)
                {
                    strRet = LeveldbOperator.GetNextKey();
                    if (string.IsNullOrEmpty(strRet))
                    {
                        break;
                    }
                    blockValue = LeveldbOperator.GetValue(strRet);
                    tempBlock = JsonHelper.Deserialize<Block>(blockValue);
                    this.BlockData2UTXOPool(tempBlock);
                }

                if (this.tempUTXOList.Count != 0)
                {
                    foreach (UTXO utxo in this.tempUTXOList)
                    {
                        if (this.utxoPool.contains(utxo))
                        {
                            this.utxoPool.removeUTXO(utxo);
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
            this.BlockData2UTXOPool(block);       
        }


        public UTXOPool GetUtxoPool()
        {
            return this.utxoPool;
        }


        public UTXO input2UTXO(Input einput)
        {
            UTXO utxo = new UTXO(einput.strpreTxHash, (uint)einput.outputIndex);
            return utxo;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="block"></param>
        /// <returns>just for update key corresponding value</returns>
        public UTXOPool BlockData2UTXOPool(Block block)
        {
            UTXOPool sigleBlockPool = new UTXOPool();
            foreach (Transaction eTransaction in block.listTransactions)
            {
                foreach (Input eInput in eTransaction.listInputs)
                {
                    UTXO utxo = this.input2UTXO(eInput);
                    if (utxoPool.contains(utxo))
                    {
                        utxoPool.removeUTXO(utxo);
                        
                    }
                    else
                    {
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
                   
                    this.utxoPool.addUTXO(utxo1, eTransaction.listOutputs[i]);
                    sigleBlockPool.addUTXO(utxo1, eTransaction.listOutputs[i]);
                }
            }

            return sigleBlockPool;
        }






        public Transaction CreatTransaction(string strPreHash, uint iIndex, double dValue, 
                                            string strPaytoPKHash,string myPriKeyPath,string myPubkeyPath)
        {

            
            string strPubKeyValuem = FileIOHelper.ReadFromText(myPubkeyPath);
            string strKeyHash = Cryptor.SHA256(strPubKeyValuem, strPubKeyValuem.Length);
            string myPubScript = string.Format("OP_DUP OP_HASH160 {0} OP_EQUALVERIFY OP_CHECKSIG", strKeyHash);


            UTXO utxo = new UTXO(strPreHash, iIndex);
            if (!this.utxoPool.contains(utxo))
            {
                LogHelper.WriteErrorLog("not in utxo");
                return null;
            }

            Transaction spendTrans = new Transaction();
            spendTrans.addInput(strPreHash, (int)iIndex);
            spendTrans.addOutput(dValue, strPaytoPKHash);

            Output preOutput = this.utxoPool.getTxOutput(utxo);
            if (dValue < preOutput.value)
            {
                Output ctOutput = new Output();
                ctOutput.value = preOutput.value - dValue;
                ctOutput.scriptPubKey = myPubScript;
                spendTrans.addOutput(ctOutput);
            }
            spendTrans.signTrans(myPriKeyPath, strPubKeyValuem);
            spendTrans.FinalSetTrans();
           
            return spendTrans;
        }


        public Transaction CreatTransaction(Dictionary<UTXO, keyPair> dicUTXO, double dInputAmount, double dPayToValue,
                                            string strPaytoPKHash, string changePubScript)
        {


            //string strPubKeyValuem = FileIOHelper.ReadFromText(myPubkeyPath);
            //string strKeyHash = Cryptor.SHA256(strPubKeyValuem, strPubKeyValuem.Length);
            //string myPubScript = string.Format("OP_DUP OP_HASH160 {0} OP_EQUALVERIFY OP_CHECKSIG", strKeyHash);

            Transaction spendTrans = new Transaction();

            // 为了计算sign方便add output
            spendTrans.addOutput(dPayToValue, strPaytoPKHash);

            //Output preOutput = this.utxoPool.getTxOutput(utxo);
            if (dPayToValue < dInputAmount)
            {
                Output ctOutput = new Output();
                ctOutput.value = dInputAmount - dPayToValue;
                ctOutput.scriptPubKey = changePubScript;
                spendTrans.addOutput(ctOutput);
            }

            //List<string> lstPrikeysPath = new List<string>();
            foreach (var item in dicUTXO)
            {
                if (!this.utxoPool.contains(item.Key))
                {
                    LogHelper.WriteErrorLog("not in utxo");
                    LogHelper.WriteErrorLog(JsonHelper.Serializer<UTXO>(item.Key));
                    return null;
                }
                else
                {
                    Input input = new Input(item.Key.getTxHash(), (int)item.Key.getIndex());
                    spendTrans.addInput(input);
                    //spendTrans.addInput(item.Key.getTxHash(), (int)item.Key.getIndex());
                    string strPriKPath = Path.Combine(AppSettings.XXPKeysFolder, item.Value.PriKeyNmae);
                    string strPubKPath = Path.Combine(AppSettings.XXPKeysFolder, item.Value.PubKeyNmae);
                    string strPubValue = FileIOHelper.ReadFromText(strPubKPath);
                    spendTrans.signTrans(strPriKPath, strPubValue, spendTrans.listInputs.IndexOf(input));
                }
            }

            
            //for(int i=0; i< lstPrikeysPath.Count; i++)
            //{
            //    spendTrans.signTrans(lstPrikeysPath[i], changePubScript, i);
            //}
            
            spendTrans.FinalSetTrans();

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
            if(tx==null)
            {
                return false;
            }

            // IMPLEMENT THIS
            double sumOut = 0;
            double sumIn = 0;

            Dictionary<string, UTXO> dicUsed = new Dictionary<string, UTXO>();

            for (int i = 0; i < tx.numInputs(); i++)
            {
                Input input = tx.getInput(i);

                UTXO utxo = new UTXO(input.strpreTxHash, (uint)input.outputIndex);
                if (!utxoPool.contains(utxo)) return false; //check (1),utox 包含该交易返回false

                Output PreOutput = utxoPool.getTxOutput(utxo);// the consume coin correspond prev output coin;
                sumIn += PreOutput.value;//(5) 计算input 指向的pre output 的value，最后保证输入的value等于该笔交易输出的
                string strOriginalTxt = tx.getRawDataToSign(i);
                if (! Cryptor.VerifySignature(input.scriptSig, PreOutput.scriptPubKey, strOriginalTxt) )
                    return false;//check(2) 
                bool bIsContain = dicUsed.ContainsKey(utxo.getTxHash());
                if(!bIsContain) // UTXO不会被重复添加
                {
                    dicUsed.Add(utxo.getTxHash(), utxo);
                }
                else
                { return false; }   
            }
            foreach (Output output in tx.getOutputs())
            {
                if (output.value < 0) return false;//check(5)
                sumOut += output.value;
            }
            if (sumIn < sumOut) return false;//check(5);
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
                            utxoPool.addUTXO(utxo, tx.getOutput((int)i));
                        }
                        //delete spent coin
                        for (int i = 0; i < tx.numInputs(); ++i)
                        {
                            Input input = tx.getInput(i);
                            UTXO utxo = new UTXO(input.strpreTxHash, (uint)input.outputIndex);
                            utxoPool.removeUTXO(utxo);
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
                        utxoPool.addUTXO(utxo, tx.getOutput((int)i));
                    }
                    //delete spent coin
                    for (int i = 0; i < tx.numInputs(); ++i)
                    {
                        Input input = tx.getInput(i);
                        UTXO utxo = new UTXO(input.strpreTxHash, (uint)input.outputIndex);
                        utxoPool.removeUTXO(utxo);
                    }
                }
                else
                {
                    sRet = "invalid transaction";
                }

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
                utxoPool.addUTXO(utxo, basetx.getOutput((int)0));
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorLog(ex.Message);
            }

        }




    }
}
