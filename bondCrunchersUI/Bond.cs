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
        public string issuerName { get; set; }
        [DataMember]
        public long maturityDate { get; set; }
        [DataMember]
        public long startDate { get; set; }
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
        public decimal currentYield { get; set; }
        [DataMember]
        public double pieceSize { get; set; }
        [DataMember]
        public string fitch { get; set; }
        [DataMember]
        public string moodys { get; set; }
        [DataMember]
        public string snP { get; set; }

        //public string Image = "th.png";

        [IgnoreDataMember]
        public DateTime Maturity { get; set; }
        [IgnoreDataMember]
        public DateTime Start { get; set; }
        [IgnoreDataMember]
        public string maturityShortDate { get; set; }
        [IgnoreDataMember]
        public string startShortDate { get; set; }
        public override string ToString()
        {
            return isin;
        }

        public void ConvertDates()
        {
            {
                DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                if (maturityDate != 0)
                {
                    Maturity = epoch.AddMilliseconds(maturityDate).ToLocalTime();
                    maturityShortDate = Maturity.ToShortDateString();
                }
                if (startDate != 0)
                {
                    Start = epoch.AddMilliseconds(startDate).ToLocalTime();
                    startShortDate = Start.ToShortDateString();
                }                
            }
        }
    }

}
