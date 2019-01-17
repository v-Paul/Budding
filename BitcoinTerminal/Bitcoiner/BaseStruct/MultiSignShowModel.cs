using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace BaseSturct
{
    public class MultiSignModel : INotifyPropertyChanged
    {
        private string id;

        public string ID
        {
            get { return id; }
            set { id = value; OnPropertyChanged("ID"); }
        }

        private string txHash;

        public string TxHash
        {
            get { return txHash; }
            set { txHash = value; OnPropertyChanged("TxHash"); }
        }

        private int otputIndex;
        public int OutputIndex
        {
            get { return otputIndex; }
            set { otputIndex = value; OnPropertyChanged("OutputIndex"); }
        }

        private double mvalue;
        public double Value
        {
            get { return mvalue; }
            set { mvalue = value; OnPropertyChanged("Value"); }
        }

        private bool bIsAdd2PriTx;
        public bool BIsAdd2PriTx
        {
            get { return bIsAdd2PriTx; }
            set { bIsAdd2PriTx = value; OnPropertyChanged("BIsAdd2PriTx"); }
        }

        private string outScriptPKHash;

        public string OutScriptPKHash
        {
            get { return outScriptPKHash; }
            set { outScriptPKHash = value; OnPropertyChanged("OutScriptPKHash"); }
        }

        private bool isCheckbBoxEnable = true;

        public bool IsCheckbBoxEnable
        {
            get { return isCheckbBoxEnable; }
            set { isCheckbBoxEnable = value; OnPropertyChanged("IsCheckbBoxEnable"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName = "")
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
    public class MultiSignViewModel
    {
        public ObservableCollection<MultiSignModel> MultiSignShows { get; set; }
        public MultiSignViewModel()
        {
            MultiSignShows = new ObservableCollection<MultiSignModel>();
        }

        public void  AddItem(MultiSignModel msSM)
        {
            this.MultiSignShows.Add(msSM);
        }

        public ObservableCollection<MultiSignModel> GetMultiSignShows()
        {
            return this.MultiSignShows;
        }
        public void SortbyScriptPKHashs()
        {
            var cc = (from x in this.MultiSignShows
                      orderby x.ID
                      select x).ToList();

            this.MultiSignShows = new ObservableCollection<MultiSignModel>(cc);
        }
        public string GetOutScriptPKHash(string TxHash)
        {
            //var cc = (from x in this.MultiSignShows
            //          where x.TxHash == TxHash
            //          select x.OutScriptPKHash);
            var mod = this.MultiSignShows.FirstOrDefault(x => x.TxHash == TxHash);

            return mod != null?mod.OutScriptPKHash:"";
        }

        public int GetOutScriptPKHashCount(string TxHash)
        {

            var mod = this.MultiSignShows.FirstOrDefault(x => x.TxHash == TxHash);
            if(mod!=null)
            {
                char ch = '\r';
                int i = mod.OutScriptPKHash.Count(x => x == ch);
                var ls = mod.OutScriptPKHash.Split(ch);
                return ls.Length;
            }
            return 0;
             
        }

        public bool bContainUtxo(string strHash, int index)
        {
            int Count = 0;
            Count = this.MultiSignShows.Count(xx => xx.TxHash == strHash && xx.OutputIndex==index);
            if(Count>0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool bContainSetChecked(string strHash, int index)
        {
            bool bContain = false;
            foreach (var item in this.MultiSignShows)
            {
                if(item.TxHash == strHash && item.OutputIndex == index)
                {
                    item.BIsAdd2PriTx = true;
                    bContain = true;
                }
            }

            return bContain;
        }

    }
}
