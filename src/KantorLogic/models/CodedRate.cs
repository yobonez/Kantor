namespace KantorLogic.models;

public class CodedRate
{
    public string Currency { get; set; }
    public string Code { get; set; }
    public Decimal? Mid { get; set; }
    public Decimal? Bid { get; set; }
    public Decimal? Ask { get; set; }
}
