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
        public string maturity_Date { get; set; }
        [DataMember]
        public string start_Date { get; set; }
        [DataMember]
        public int bond_Duration { get; set; }
        [DataMember]
        public decimal coupon_Rate { get; set; }
        [DataMember]
        public string coupon_Period { get; set; }
        [DataMember]
        public decimal high { get; set; }
        [DataMember]
        public decimal low { get; set; }
        [DataMember]
        public decimal last { get; set; }
        [DataMember]
        public decimal change { get; set; }
        [DataMember]
        public decimal yield { get;  set;}

        //public string Image = "th.png";

        public override string ToString()
        {
            return isin;
        }
    }

}
