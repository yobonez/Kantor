namespace KantorLogic.models;


public class GoldValues
{
    public List<GoldRate> CenaZlota { get; set; }
}
public class GoldRate
{
    public DateTime Data { get; set; }
    public Decimal Cena { get; set; }
}
public class CurrencyValues
{
    public string Table { get; set; }
    public string Currency { get; set; }
    public string Code { get; set; }
    public List<RateComponent> Rates { get; set; }
}
