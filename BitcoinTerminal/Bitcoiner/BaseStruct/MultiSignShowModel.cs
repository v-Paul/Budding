using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace BaseSturct
{
    public class MultiSignShowModel : INotifyPropertyChanged
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
            set { value = mvalue; OnPropertyChanged("Value"); }
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
        public ObservableCollection<MultiSignShowModel> MultiSignShows { get; set; }
        public MultiSignViewModel()
        {
            MultiSignShows = new ObservableCollection<MultiSignShowModel>();
        }

        public void  AddItem(MultiSignShowModel msSM)
        {
            this.MultiSignShows.Add(msSM);
        }

        public ObservableCollection<MultiSignShowModel> GetMultiSignShows()
        {
            return this.MultiSignShows;
        }
    }
}
