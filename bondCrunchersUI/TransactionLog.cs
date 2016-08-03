using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Web.Script.Serialization;
namespace bondCrunchersUI
{
    [DataContract]
    class TransactionLog: Transaction
    {
        [DataMember]
        public int orderId { get; set; }
        [ScriptIgnore]
        public DateTime timeStampLog { get; set; }
        [ScriptIgnore]
        private DateTime settlementDateLog { get; set;}
        [ScriptIgnore]
        private DateTime tradeDateLog { get; set; }
        [ScriptIgnore]
        public string tradeDateShort { get; set; }
        [ScriptIgnore]
        public string settlementDateShort { get; set; }

        public void ConvertDates()
        {
            DateTime epoch = new DateTime(1970,1,1,0,0,0,0);
            if (base.settlementDate != 0)
            {
                settlementDateLog = epoch.AddMilliseconds(base.settlementDate).ToLocalTime();
                settlementDateShort = settlementDateLog.ToShortDateString();
            }
            if (base.timeStamp !=0)
                timeStampLog = epoch.AddMilliseconds(base.timeStamp).ToLocalTime();
            if (base.tradeDate != 0)
            {
                tradeDateLog = epoch.AddMilliseconds(base.tradeDate).ToLocalTime();
                tradeDateShort = tradeDateLog.ToShortDateString();
            }
        }
    }
}
