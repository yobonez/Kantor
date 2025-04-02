namespace KantorLogic.models;

public class CurrencyTable
{
    public string Table { get; set; }
    public string No { get; set; }
    public DateTime? TradingRate { get; set; }
    public DateTime EffectiveDate { get; set; }
    public List<CodedRate> Rates { get; set; }
}
