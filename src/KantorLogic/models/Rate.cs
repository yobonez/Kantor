using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KantorLogic.models
{
    public class Rate
    {
        public string Currency { get; set; }
        public string Code { get; set; }
        public Decimal? Mid { get; set; }
        public Decimal? Bid { get; set; }
        public Decimal? Ask { get; set; }
    }
}
