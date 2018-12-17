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

        private List<Transaction> lstPoolTx;
        // 自己创建的交易 直接加进PoolTx，收到的交易，先加进tempPoolTx
        private HashSet<Transaction> hashsetPoolTx;

        private Block mLastBlock;

        public string strPuzzle { get; set; } = string.Empty;

        public BkHandler()
        {
            
            this.lstPoolTx = new List<Transaction>();
            this.hashsetPoolTx = new HashSet<Transaction>();

            this.mLastBlock = new Block();

    }

        public Block GetLastBlock()
        {

           
            string strRet = LeveldbOperator.OpenDB(AppSettings.XXPDBFolder);
           
            if (strRet == ConstHelper.BC_OK)
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

        public void RefreshLastBlock(Block newLastBlock)
        {
            if(newLastBlock != null)
            {
                this.mLastBlock = newLastBlock;
                this.strPuzzle = this.mLastBlock.Header.PuzzToStr();
            }
            
        }




        public Transaction CreatCoinBaseTX(string strAddress)
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



        public Block CreatBlock(string strNounce, string sBaseCoinScript)
        { 

            try
            {
                Block block = new Block();

                if (!Cryptor.Verify24Puzzel(this.mLastBlock.Header.Puzzle, strNounce))
                    return block;

                // mutex todo 181215
                Transaction basecoinTrans = this.CreatCoinBaseTX(sBaseCoinScript);
                this.AddTransaction(basecoinTrans);
                this.HashsetPool2list();

                block.listTransactions = this.GetlstPoolTx();
                block.SetTransInfo();
                block.SetBlockHeader(this.mLastBlock.Hash, this.mLastBlock.Header.Height);

                block.SetNonce(strNounce);
                block.SetBlockHash();

                string jsonblock = JsonHelper.Serializer<Block>(block);
                LogHelper.WriteInfoLog(jsonblock);
                return block;
            }
            catch(Exception ex)
            {
                LogHelper.WriteErrorLog(ex.Message);
                Block block = new Block();
                return block;
            }
        }

        // Add mutex todo
        public string AddTransaction(Transaction tran)
        {
            if(!this.hashsetPoolTx.Contains(tran))
            {
                this.hashsetPoolTx.Add(tran);
                return Decision.Accept;
            }
            else
            {
                return Decision.Accepted;
            }

        }

        public bool AddTransaction(Transaction[] arrTrans)
        {
            try
            {
                foreach (Transaction tran in arrTrans )
                {
                    AddTransaction(tran);
                }
                
                return true;
            }
            catch(Exception ex)
            {
                LogHelper.WriteErrorLog(ex.Message);
                return false;
            }
            
        }

        //public bool InsertBasecoin(Transaction basecoinTrans)
        //{
        //    try
        //    {   // one block only has one basecoin 
        //        if(!this.lstPoolTx.Contains(basecoinTrans))
        //        {
        //            this.lstPoolTx.Insert(0, basecoinTrans);
        //            return true;

        //        }
        //        else
        //        {
        //            return false;
        //        }
                
        //    }
        //    catch (Exception ex)
        //    {
        //        LogHelper.WriteErrorLog(ex.Message);
        //        return false;
        //    }
        //}

        public string WriteLastblock(Block block)
        {
            if(block == null)
            {
                return "Invalid block";
            }
            string jsonblock = JsonHelper.Serializer<Block>(block);

            string strRet = LeveldbOperator.OpenDB(AppSettings.XXPDBFolder);
            if (strRet != ConstHelper.BC_OK)
            {
                return "Open DB fail";
            }
            strRet = LeveldbOperator.PutKeyValue(block.Hash, jsonblock);
            strRet = LeveldbOperator.PutKeyValue(ConstHelper.BC_LastKey, jsonblock);
            LeveldbOperator.CloseDB();
            if (strRet != ConstHelper.BC_OK)
            {
                return "Write KeyValue fail";
            }
            return ConstHelper.BC_OK;
        }

        public bool bIsValidBlock(Block block)
        {
            if (this.mLastBlock.Hash == block.Header.PreHash)
            {
                // check trans todo 181212
                return true;
            }
            else
                return false;
        } 


        public void HashsetPool2list()
        {
            foreach (var item in this.hashsetPoolTx)
            {
                this.lstPoolTx.Add(item);
            }
            this.hashsetPoolTx.Clear();
        }
        public List<Transaction> GetlstPoolTx()
        {
            List<Transaction> lstTx = new List<Transaction>();
            foreach (var item in this.lstPoolTx)
            {
                lstTx.Add(item);
            }
            this.lstPoolTx.Clear();
            return lstTx;
        }

         


    }
}
