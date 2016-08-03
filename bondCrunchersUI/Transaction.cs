using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace bondCrunchersUI
{
    [DataContract]
    class Transaction
    {
        [DataMember]
        public string isin { get; set; }
        [DataMember]
        public long timeStamp { get; set; }
        [DataMember]
        public long settlementDate { get; set; }
        [DataMember]
        public decimal cleanPrice { get; set; }
        [DataMember]
        public decimal dirtyPrice { get; set; }
        [DataMember]
        public decimal accruedAmount { get; set; }
        [DataMember]
        public decimal settlementAmount { get; set; }
        [DataMember]
        public decimal tradeYield { get; set; }
    }
}
