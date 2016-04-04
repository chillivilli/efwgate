using Gateways.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EfawateerGateway
{
    public class PaymentResult
    {
        public string JoebppsTrx { get; set; }
        public int Error { get; set; }
        public DateTime TimeStamp { get; set; }
        public StringList Params { get; set; }

        public string State { get; set; }
    }
}
