using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;

namespace BaseSturct
{
    //public class UTXO implements Comparable<UTXO>
    [Serializable]
    public class UTXO
    {
        // Hash of the transaction from which this UTXO originates 
        private string strTxHash;

        // Index of the corresponding output in said transaction 
        private uint index;

        
         //Creates a new UTXO corresponding to the output with index <index> in the transaction whose
         //hash is {@code txHash}       
        public UTXO(string strtxHash, uint index)
        {
            this.strTxHash = strtxHash;
            this.index = index;
        }

        public UTXO()
        {
            this.strTxHash =string.Empty;
            this.index = 0;
        }
        /** @return the transaction hash of this UTXO */
        public string getTxHash()
        {
            return strTxHash;
        }

        /** @return the index of this UTXO */
        public uint getIndex()
        {
            return index;
        }


        //Compares this UTXO to the one specified by {@code other}, considering them equal if they have
        // {@code txHash} arrays with equal contents and equal {@code index} values        
        public override bool Equals(Object other)
        {
            if (other == null)
            {
                return false;
            }
            UTXO otherUtxo = (UTXO)other;
            // 直接对比hash是否一致
            if (this.utoxHashCode() != otherUtxo.utoxHashCode())
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
            return (strTxHash + index).GetHashCode();
        }


        // hash256(hsah + index) 
        public string utoxHashCode()
        {
            string strUtox = this.strTxHash + this.index;
            string strHash =Cryptor.SHA256(strUtox, strUtox.Length);
            return strHash;
        }

        // Compares this UTXO to the one specified by {@code utxo} 
        //public int compareTo(UTXO utxo)
        //{
        //    byte[] hash = utxo.txHash;
        //    int in = utxo.index;
        //    if (in > index)
        //    return -1;
        //else if (in < index)
        //    return 1;
        //else {
        //        int len1 = txHash.length;
        //        int len2 = hash.length;
        //        if (len2 > len1)
        //            return -1;
        //        else if (len2 < len1)
        //            return 1;
        //        else
        //        {
        //            for (int i = 0; i < len1; i++)
        //            {
        //                if (hash[i] > txHash[i])
        //                    return -1;
        //                else if (hash[i] < txHash[i])
        //                    return 1;
        //            }
        //            return 0;
        //        }
        //    }
        //}
    }
}
