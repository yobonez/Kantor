namespace KantorLogic.models;



public class ConcreteCurrencyValues
{
    public string Table { get; set; }
    public string Currency { get; set; }
    public string Code { get; set; }
    public List<RateComponent> Rates { get; set; }
}
