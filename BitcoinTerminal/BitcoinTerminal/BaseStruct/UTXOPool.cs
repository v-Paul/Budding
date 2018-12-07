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
    public class UTXOPool
    {
        // The current collection of UTXOs, with each one mapped to its corresponding transaction output
        private Dictionary<UTXO, Output> dicUtxoPool;

        // Creates a new empty UTXOPool 
        public UTXOPool()
        {
            dicUtxoPool = new Dictionary<UTXO, Output>();
        }

        // Creates a new UTXOPool that is a copy of {@code uPool} 
        public UTXOPool(UTXOPool uPool)
        {
            dicUtxoPool = new  Dictionary<UTXO, Output>(uPool.dicUtxoPool);
        }


       

        // Adds a mapping from UTXO {@code utxo} to transaction output @code{txOut} to the pool 
        public void addUTXO(UTXO utxo, Output txOut)
        {
            try
            {
                dicUtxoPool.Add(utxo, txOut);
            }
            catch(Exception ex)
            {
                string str = JsonHelper.Serializer<UTXO>(utxo);
                LogHelper.WriteErrorLog(str);
                LogHelper.WriteErrorLog(ex.Message);
            }
           
        }

        // Removes the UTXO {@code utxo} from the pool 
        public void removeUTXO(UTXO utxo)
        {
            dicUtxoPool.Remove(utxo);
        }

        //
        // @return the transaction output corresponding to UTXO {@code utxo}, or null if {@code utxo} is
        //         not in the pool.

        public Output getTxOutput(UTXO ut)
        {
            Output optemp = new Output();

            if( dicUtxoPool.TryGetValue(ut, out optemp) )
            {
                return optemp;
            }

            return null;

            
        }

        // @return true if UTXO {@code utxo} is in the pool and false otherwise 
        public bool contains(UTXO utxo)
        {
            return dicUtxoPool.ContainsKey(utxo);
        }

        // Returns an {@code ArrayList} of all UTXOs in the pool 
        public List<UTXO> getAllUTXO()
        {
            List<UTXO> allUTXO = new List<UTXO>();
            Dictionary<UTXO, Output>.KeyCollection keyCol = dicUtxoPool.Keys;
            foreach (UTXO ut in keyCol)
            {
                allUTXO.Add(ut);
            }
            return allUTXO;
        }

        public  int getUTXOPollCount()
        {
            return this.dicUtxoPool.Count;
        }
    }
}
