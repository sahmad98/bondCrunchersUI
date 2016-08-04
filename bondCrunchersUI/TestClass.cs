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
    class TestClass
    {
        [DataMember]
        public string isin { get; set; }
        [DataMember]
        public string Description { get; set; }

        public TestClass(string isin, string description)
        {
            this.isin = isin;
            this.Description = description;
        }

        public override string ToString()
        {
            return "ISIN: "+isin+", Description: "+Description;
        }
    }
}
