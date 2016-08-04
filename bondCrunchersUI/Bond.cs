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
    class Bond
    {
        public string ISIN { get; private set; }
    }
}
