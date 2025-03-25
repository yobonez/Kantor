using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KantorLogic.models;

public class Currencies
{
    public string Table { get; set; }
    public string No { get; set; }
    public DateTime EffectiveDate { get; set; }
    public List<Rate> Rates { get; set; }
}
