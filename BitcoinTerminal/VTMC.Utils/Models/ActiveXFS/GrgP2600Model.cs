using System.Collections.Generic;

namespace VTMC.Utils.Models
{

    public class P2600Status
    {
        public string fwDevice { get; set; }
        public string TransactionStatus { get; set; }
    }

    public class NoteNumber
    {
        public int usNoteID { get; set; }
        public int ulCount { get; set; }
    }

    public class P2600CountResult
    {

        List<NoteNumber> noteNumberList = new List<NoteNumber>();
        public List<NoteNumber> NoteNumberList
        {
            get { return noteNumberList; }
            set { noteNumberList = value; }
        }
        public string Currency { get; set; }
        public int Amount { get; set; }
    }
}
