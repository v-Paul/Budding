using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BaseSturct
{
    [Serializable]
    public class MultiSignShowModel
    {     
        public string ID { get; set; }
        public string TxHash { get; set; }
        public int OutputIndex { get; set; }
        public double Value { get; set; }    
        public bool bIsAdd2PriTx { get; set; }
        public string OutScriptPKHash { get; set; }

    }
    public class MultiSignViewModel
    {
        public List<MultiSignShowModel> MultiSignShows { get; set; }
        public MultiSignViewModel()
        {
            MultiSignShows = new List<MultiSignShowModel>();
        }

        public void  AddItem(MultiSignShowModel msSM)
        {
            this.MultiSignShows.Add(msSM);
        }

        public List<MultiSignShowModel> GetMultiSignShows()
        {
            return this.MultiSignShows;
        }
    }
}
