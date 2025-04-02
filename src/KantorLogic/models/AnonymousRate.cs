namespace KantorLogic.models;

public class AnonymousRate
{
    public string No { get; set; }
    public DateTime EffectiveDate { get; set; }
    public Decimal? Mid { get; set; }
    public Decimal? Bid { get; set; }
    public Decimal? Ask { get; set; }
}
