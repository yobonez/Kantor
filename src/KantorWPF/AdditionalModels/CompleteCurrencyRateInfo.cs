using System.Windows.Controls;
using System.Windows.Media;

namespace KantorWPF.AdditionalModels
{
    public class CompleteCurrencyRateInfo
    {
        public System.Drawing.Image FlagImage { get; set; }
        public System.Drawing.Image UpDownImage { get; set; }
        public string Currency { get; set; }
        public string Code { get; set; }
        public Decimal? Mid { get; set; }
        public Decimal? Bid { get; set; }
        public Decimal? Ask { get; set; }
    }
}
