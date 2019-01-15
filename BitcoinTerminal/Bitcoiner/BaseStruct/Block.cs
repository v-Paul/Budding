using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;
using VTMC.Utils;

namespace BaseSturct
{
    [Serializable]
    public class Head
    {
        public string Version { get; set; }

        public string PreHash { get; set; }

        public int Height { get; set; }

        // 0.?版本，先以basecoin hash代替
        public string HashMerkleRoot { get; set; }

        public string TimeStamp { get; set; }

        public int[] Puzzle;
        public string nonce;
        public Head()
        {
            this.CreatPuzzle();
        }

        private void CreatPuzzle()
        {
            this.Puzzle = new int[4];
            Random rd = new Random();
           
            bool bCreatPuzzle = false;
            while (!bCreatPuzzle)
            {
                for (int i = 0; i < 4; i++)
                {
                    this.Puzzle[i] = rd.Next(1, 11);
                }
                bCreatPuzzle = Cryptor.Calc24(this.Puzzle);
            }            
        }

        public string PuzzToStr()
        {
            string str = string.Format("{0} {1} {2} {3}", Puzzle[0], Puzzle[1], Puzzle[2], Puzzle[3]);
            return str; 
        }

        public string HeaderToStr()
        {
            string strTemp = string.Format("{0}{1}{2}{3}{4}{5}", this.Version, this.PreHash,
                                           this.HashMerkleRoot, this.TimeStamp,
                                           this.PuzzToStr(), this.nonce);

            return strTemp;
        }

    }

    [Serializable]
   public class Block
    {
        public string Hash { get; set; }
        public string Magic { get; set; }
        public int Size { get; set; }
        public Head Header;

        public int TransCount { get; set; }
        //Transaction Transactions;
        public List<Transaction> listTransactions;

        public Block()
        {
            this.Header = new Head();
            this.listTransactions = new List<Transaction>();
            this.Magic = "xi-xia-pu";
        }

        public void SetTransInfo()
        {

            this.TransCount = this.listTransactions.Count;
            this.Header.HashMerkleRoot = this.CalMerkleRoot(GetTxsHashList());
            //this.SetBlockSize();
        }

        public List<string> GetTxsHashList( )
        {
            //step1: 对数据块做hash运算，Node0i = hash(Data0i), i = 1,2,…,9
            //step2: 相邻两个hash块串联，然后做hash运算，Node1((i + 1) / 2) = hash(Node0i + Node0(i + 1)), 
            //       i = 1,3,5,7; 对于i = 9, Node1((i + 1) / 2) = hash(Node0i)
            //step3: 重复step2
            //step4: 重复step2
            //step5: 重复step2，生成Merkle Tree Root
            List<string> lstTxsHash = new List<string>();
            foreach (var item in this.listTransactions)
            {
                lstTxsHash.Add(item.CalTransHash());
            }
            return lstTxsHash;
        }

        public string CalMerkleRoot(List<string> lstHash) 
        {

            //step2: 相邻两个hash块串联，然后做hash运算，Node1((i + 1) / 2) = hash(Node0i + Node0(i + 1)), 
            //       i = 1,3,5,7; 对于i = 9, Node1((i + 1) / 2) = hash(Node0i)
            //step3: 重复step2
            //step4: 重复step2
            //step5: 重复step2，生成Merkle Tree Root
            List<string> lstTemp = new List<string>();

            int index = 0;
            while (index < lstHash.Count())
            {
                // left
                String left = lstHash[index];
                index++;
                // right
                String right = left;
                if (index != lstHash.Count())
                {
                    right = lstHash[index];
                }
                // sha2 hex value
                string LeftAndRight = left + right;
                String sha2HexValue = Cryptor.SHA256(LeftAndRight, LeftAndRight.Length);
                lstTemp.Add(sha2HexValue);
                index++;
            }
            if(lstTemp.Count>1)
            {
                CalMerkleRoot(lstTemp);
            }

            return lstTemp[0];
        }
        public void SetBlockHeader(string strPreHash, int iPrebkheight)
        {
            this.Header.Version = AppSettings.ProductVersion;
            this.Header.TimeStamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            this.Header.PreHash = strPreHash;
            this.Header.Height = iPrebkheight + 1;
        }

        public void SetNonce(string strNonce)
        {
            this.Header.nonce = strNonce;
        }

        public void SetBlockHash()
        {
            this.Hash = CalBlockHash();


        }

        public string CalBlockHash()
        {
            string strHeader = this.Header.HeaderToStr();
            // 计算两遍sha
            string strhash = Cryptor.SHA256(strHeader, strHeader.Length);
            strhash = Cryptor.SHA256(strhash, strhash.Length);
            return strhash;
        }

        public void SetBlockSize(int iSize)
        {
            //存储是已json字符串形式存储block的，所以这里size设置为block json字符串的长度
            //this.Size = this.listTransactions.Count * 100;//* Marshal.SizeOf(new Transaction());
            this.Size = iSize;
        }

        public Transaction GetBaseCoinTx()
        {           
            var Basetx = this.listTransactions.FirstOrDefault(x=>x.listInputs[0].PreTxHash == ConstHelper.BC_BaseCoinInputTxHash);
            return Basetx;
        }



    }
}
