using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;
using System.Data;
using VTMC.Utils;
using System.IO;

namespace BaseSturct
{
    class BkHandler
    {

        private List<Transaction> poolTx;
        //private string mLastBlockHash;
        //private int[] lastPuzzle;
        private Block mLastBlock;
        public string strPuzzle { get; set; }

        public BkHandler()
        {
            
            this.poolTx = new List<Transaction>();
            //this.mLastBlockHash = string.Empty;
            //this.lastPuzzle = new int[4];
            this.mLastBlock = new Block();

    }








        //public static bool Verify24Puzzel(int[] arrPuzzle, string strExpress);
        public Block GetLastBlock()
        {

           
            string strRet = LeveldbOperator.OpenDB(AppSettings.XXPDBFolder);
           
            if (!string.IsNullOrEmpty(strRet))
            {
                string strlastBlock = LeveldbOperator.GetValue(ConstHelper.BC_LastKey);
                if (!string.IsNullOrEmpty(strlastBlock))
                {
                    this.mLastBlock = JsonHelper.Deserialize<Block>(strlastBlock);  
                    this.strPuzzle = this.mLastBlock.Header.PuzzToStr();
                    //this.mLastBlockHash = this.mLastBlock.Hash;
                }
            }
            LeveldbOperator.CloseDB();
            return mLastBlock;
        }


        public bool CreatBaseCoinBlock(string strAddress)
        {
            
            Transaction basecoinTrans = this.CoinBaseTX(strAddress);
            this.poolTx.Add(basecoinTrans);
            Block baseBlock = new Block();
            //baseBlock.listTransactions.Add(FirstTrans);
            baseBlock.listTransactions = this.poolTx;
            baseBlock.SetTransInfo();
            baseBlock.SetBlockHeader("0000000000000000000000000000000000000000000000000000000000000000", -1);
            string strnounce = "";
            baseBlock.SetNonce(strnounce);
            baseBlock.SetBlockHash();

            string jsonblock = JsonHelper.Serializer<Block>(baseBlock);
           
            string strRet = LeveldbOperator.OpenDB(AppSettings.XXPDBFolder);
            strRet = LeveldbOperator.PutKeyValue(baseBlock.Hash, jsonblock);
            strRet = LeveldbOperator.PutKeyValue(ConstHelper.BC_LastKey, jsonblock);
            bool breadOK = false;
            string readout = LeveldbOperator.GetValue(baseBlock.Hash);
            LogHelper.WriteInfoLog(readout);
            LeveldbOperator.CloseDB();

            if (!string.IsNullOrEmpty(readout))
            {
                breadOK = true;
            }

            this.GetLastBlock();

            return breadOK;


        }

        public Transaction CoinBaseTX(string strAddress)
        {
            Transaction basecoinTrans = new Transaction();
            string basecoinPrehash = "0000000000000000000000000000000000000000000000000000000000000000";

            int basecoinIndex = -1;
            Input basecoinInput = new Input(basecoinPrehash, basecoinIndex);
            scriptSig bassecoinSS = new scriptSig();
            string nowTime = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            bassecoinSS.Signature = Cryptor.SHA256(nowTime, nowTime.Length);
            bassecoinSS.PubKey = Cryptor.SHA256(bassecoinSS.Signature, bassecoinSS.Signature.Length);
            basecoinInput.addSignature(bassecoinSS);
            basecoinTrans.addInput(basecoinInput);

            basecoinTrans.addOutput(24, strAddress);
            basecoinTrans.FinalSetTrans();
            return basecoinTrans;
            //this.poolTx.Add(RewardTrans);
        }



        public string  CreatBlock(string strNounce)
        { 

            try
            {
                if (!Cryptor.Verify24Puzzel(this.mLastBlock.Header.Puzzle, strNounce))
                    return "Verify Puzzle fail";

                Block Block = new Block();
                Block.listTransactions = this.poolTx;
                Block.SetTransInfo();
                Block.SetBlockHeader(this.mLastBlock.Hash, this.mLastBlock.Header.Height);

                Block.SetNonce(strNounce);
                Block.SetBlockHash();

                string jsonblock = JsonHelper.Serializer<Block>(Block);
                LogHelper.WriteInfoLog(jsonblock);
                string strRet = LeveldbOperator.OpenDB(AppSettings.XXPDBFolder);
                if(strRet!= ConstHelper.BC_OK)
                {
                    return "Open DB fail";
                }
                strRet = LeveldbOperator.PutKeyValue(Block.Hash, jsonblock);
                strRet = LeveldbOperator.PutKeyValue(ConstHelper.BC_LastKey, jsonblock);
                LeveldbOperator.CloseDB();
                if (strRet != ConstHelper.BC_OK)
                {
                    return "Write KeyValue fail";
                }
                return ConstHelper.BC_OK;
            }
            catch(Exception ex)
            {
                LogHelper.WriteErrorLog(ex.Message);
                return "CreatBlock catch an exception";
            }
        }

        public bool AddTransaction(Transaction tran)
        {
            try
            {
                this.poolTx.Add(tran);

                return true;
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorLog(ex.Message);
                return false;
            }

        }

        public bool AddTransaction(Transaction[] arrTrans)
        {
            try
            {
                foreach (Transaction tran in arrTrans )
                {
                    this.poolTx.Add(tran);
                }
                
                return true;
            }
            catch(Exception ex)
            {
                LogHelper.WriteErrorLog(ex.Message);
                return false;
            }
            
        }

        public bool InsertBasecoin(Transaction basecoinTrans)
        {
            try
            {   // one block only has one basecoin 
                if(!this.poolTx.Contains(basecoinTrans))
                {
                    this.poolTx.Insert(0, basecoinTrans);
                    return true;

                }
                else
                {
                    return false;
                }
                
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorLog(ex.Message);
                return false;
            }
        }

        public void ClearTransPool()
        {
            this.poolTx.Clear();
        }




    }
}
