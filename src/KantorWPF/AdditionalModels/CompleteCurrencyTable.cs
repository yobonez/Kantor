using KantorWPF.MVVM;

namespace KantorWPF.AdditionalModels
{
    class CompleteCurrencyTable : ViewModelBase
    {
        public string Table { get; set; }
        public string No { get; set; }
        public DateTime? TradingRate { get; set; }
        public DateTime EffectiveDate { get; set; }
        public List<CompleteCurrencyRateInfo> Rates { get; set; }
    }
}
