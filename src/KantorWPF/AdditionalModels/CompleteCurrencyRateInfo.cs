namespace KantorWPF.AdditionalModels
{
    public class CompleteCurrencyRateInfo
    {
        public System.Drawing.Image? FlagImage { get; set; }
        public string Currency { get; set; }
        public string Code { get; set; }
        public Decimal? Mid { get; set; }
        public System.Drawing.Image? UpDownMidImage { get; set; }
        public Decimal? Bid { get; set; }
        public System.Drawing.Image? UpDownBidImage { get; set; }
        public Decimal? Ask { get; set; }
        public System.Drawing.Image? UpDownAskImage { get; set; }
    }
}
