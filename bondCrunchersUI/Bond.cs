using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Web.Script.Serialization;

namespace bondCrunchersUI
{
    [DataContract]
    public class Bond
    {
        [DataMember]
        public int id { get; set; }
        [DataMember]
        public string isin { get; set; }
        [DataMember]
        public string maturityDate { get; set; }
        [DataMember]
        public string startDate { get; set; }
        [DataMember]
        public int bondDuration { get; set; }
        [DataMember]
        public decimal couponRate { get; set; }
        [DataMember]
        public string couponPeriod { get; set; }
        [DataMember]
        public decimal high { get; set; }
        [DataMember]
        public decimal low { get; set; }
        [DataMember]
        public decimal lastPrice { get; set; }
        [DataMember]
        public decimal changePrice { get; set; }
        [DataMember]
        public decimal yield { get;  set;}

        //public string Image = "th.png";

        public override string ToString()
        {
            return isin;
        }
    }

}
