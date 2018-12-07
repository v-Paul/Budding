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
            this.Header.HashMerkleRoot = this.listTransactions[0].strTransHash;
            this.SetBlockSize();
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
            //version prev_block mrkl_root time bits nonce
            //string strTemp = string.Format("{0}{1}{2}{3}{4}{5}", this.Header.Version,this.Header.PreHash,
            //                                this.Header.HashMerkleRoot,this.Header.TimeStamp,
            //                                this.Header.PuzzToStr(),this.Header.nonce );
            string strHeader = this.Header.HeaderToStr();
            // 计算两遍sha
            string strhash = Cryptor.SHA256(strHeader, strHeader.Length);
            this.Hash = Cryptor.SHA256(strhash, strhash.Length);


        }

        private void SetBlockSize()
        {
            //C# 无法计算出list对象详细的内存大小，粗略用Count*一个的大小
            this.Size = this.listTransactions.Count * 100;//* Marshal.SizeOf(new Transaction());
        }





    }
}
