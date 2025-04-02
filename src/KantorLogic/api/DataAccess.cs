using KantorLogic.models;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace KantorLogic.api;

public class DataAccess
{
    private HttpClient _httpClient;
    private readonly string _apiUrl = "https://api.nbp.pl/api/";
    private List<CurrencyTable> currencyTables;

    private readonly string[] tableCodes = { "A", "B", "C" };
    public DataAccess()
    {
        _httpClient = new HttpClient();
        currencyTables = new List<CurrencyTable>();
    }

    // this code does too much, move BuildEndpointString to different class called "EndpointStringBuilder"
    // or something idk
    private string BuildEndpointString(string tableCode, 
                                       int topCount = 0,
                                       string? currencyCode = null)
    {
        string tablesRates = (currencyCode != null) ? "rates" : "tables";
        string subPath = (tableCode == "gold") 
                         ? "cenyzlota" 
                         : $"exchangerates/{tablesRates}/{tableCode}";

        string result = string.Empty;

        if (currencyCode != null && tableCode != "gold")
        {
            if (topCount > 0) result = _apiUrl + $"{subPath}/{currencyCode}/last/{topCount}";
            else result = _apiUrl + $"{subPath}/{currencyCode}";

            return result;
        }
        if (topCount > 0) result = _apiUrl + $"{subPath}/last/{topCount}";
        else result = _apiUrl + $"{subPath}";

        return result;
    }
    private string BuildEndpointString(string tableCode,
                                       DateTime date,
                                       string? currencyCode = null)
    {
        string tablesRates = (currencyCode != null) ? "rates" : "tables";
        string subPath = (tableCode == "gold")
                         ? "cenyzlota"
                         : $"exchangerates/{tablesRates}/{tableCode}";

        string result = string.Empty;

        string day = (date.Day < 10) ? "0" + date.Day.ToString() : date.Day.ToString();
        string month = (date.Month < 10) ? "0" + date.Month.ToString() : date.Month.ToString();
        string dateForQuery = date.Year.ToString() + '-' +
                              month + '-' +
                              day
                              ?? "today";
        if (currencyCode != null)
        { result = _apiUrl + $"{subPath}/{currencyCode}/{dateForQuery}"; return result; }

        result = _apiUrl + $"{subPath}/{dateForQuery}";

        return result;
    }
    private string BuildEndpointString(string tableCode,
                                       DateTime from, 
                                       DateTime to,
                                       string? currencyCode = null)
    {
        string tablesRates = (currencyCode != null) ? "rates" : "tables";
        string subPath = (tableCode == "gold")
                         ? "cenyzlota"
                         : $"exchangerates/{tablesRates}/{tableCode}";

        string result = string.Empty;

        string dStart = (from.Day < 10) ? "0" + from.Day.ToString() : from.Day.ToString();
        string mStart = (from.Month < 10) ? "0" + from.Month.ToString() : from.Month.ToString();
        string dateStartForQuery = from.Year.ToString() + '-' +
                                   mStart + '-' +
                                   dStart
                                   ?? throw new Exception("Nie podano daty");

        string dEnd = (to.Day < 10) ? "0" + to.Day.ToString() : to.Day.ToString();
        string mEnd = (to.Month < 10) ? "0" + to.Month.ToString() : to.Month.ToString();
        string dateEndForQuery = to.Year.ToString() + '-' +
                                 mEnd + '-' +
                                 dEnd
                                 ?? throw new Exception("Nie podano daty");

        if (currencyCode != null)
        { result = _apiUrl + $"{subPath}/{currencyCode}/{dateStartForQuery}/{dateEndForQuery}"; return result; }

        result = _apiUrl + $"{subPath}/{dateStartForQuery}/{dateEndForQuery}";

        return result;
    }

    // TODO: refactoring
    private async Task<ConcreteCurrencyValues> CCVGetter(string endpoint)
    {
        HttpResponseMessage response = await _httpClient.GetAsync(endpoint);
        ConcreteCurrencyValues currencyValuesResponse = new();
        if (response.StatusCode == HttpStatusCode.OK)
            return await response.Content.ReadAsAsync<ConcreteCurrencyValues>();
        else throw new HttpRequestException(response.ReasonPhrase + "\n" + endpoint);
    }
    private async Task<List<GoldRate>> GoldGetter(string endpoint)
    {
        HttpResponseMessage response = await _httpClient.GetAsync(endpoint);
        List<GoldRate> goldRatesResponse = new();
        if (response.StatusCode == HttpStatusCode.OK)
            return await response.Content.ReadAsAsync<List<GoldRate>>();
        else throw new HttpRequestException(response.ReasonPhrase + "\n" + endpoint);
    }
    private async Task<List<CurrencyTable>> CTGetterMultiple(string endpoint)
    {
        HttpResponseMessage response = await _httpClient.GetAsync(endpoint);
        List<CurrencyTable> currencyTableResponse = new();
        if (response.StatusCode == HttpStatusCode.OK)
            return await response.Content.ReadAsAsync<List<CurrencyTable>>();
        else throw new HttpRequestException(response.ReasonPhrase + "\n" + endpoint);
    }

    // single currency retrieval
    public async Task<ConcreteCurrencyValues> GetCurrencyNewest(string tableCode, string? currencyCode)
    {
        string endpoint = BuildEndpointString(tableCode, currencyCode: currencyCode);
        return await CCVGetter(endpoint);
    }
    public async Task<ConcreteCurrencyValues> GetCurrencyNewestCount(string tableCode, string? currencyCode, int last)
    {
        string endpoint = BuildEndpointString(tableCode, currencyCode: currencyCode, topCount: last);
        return await CCVGetter(endpoint);
    }
    public async Task<ConcreteCurrencyValues> GetCurrencyInDate(string tableCode, string? currencyCode, DateTime date)
    {
        string endpoint = BuildEndpointString(tableCode, date, currencyCode: currencyCode);
        return await CCVGetter(endpoint);
    }
    public async Task<ConcreteCurrencyValues> GetCurrencyInDateRange(string tableCode, string? currencyCode, DateTime from, DateTime to)
    {
        string endpoint = BuildEndpointString(tableCode, from, to, currencyCode: currencyCode);
        return await CCVGetter(endpoint);
    }
    public async Task<List<GoldRate>> GetGoldPriceNewest()
    {
        string endpoint = BuildEndpointString("gold");
        return await GoldGetter(endpoint);
    }
    public async Task<List<GoldRate>> GetGoldPriceInDate(DateTime date)
    {
        string endpoint = BuildEndpointString("gold", date);
        return await GoldGetter(endpoint);
    }
    public async Task<List<GoldRate>> GetGoldPriceInDateRange(DateTime from, DateTime to)
    {
        string endpoint = BuildEndpointString("gold", from, to);
        return await GoldGetter(endpoint);
    }

    // retrieval of multiple currencies
    public async Task<CurrencyTable> GetCurrencyTableNewest(string tableCode)
    {
        string endpoint = BuildEndpointString(tableCode);

        HttpResponseMessage response = await _httpClient.GetAsync(endpoint);
        List<CurrencyTable> currencyTableResponse = new();
        if (response.StatusCode == HttpStatusCode.OK)
            currencyTableResponse = await response.Content.ReadAsAsync<List<CurrencyTable>>();
        else throw new HttpRequestException(response.ReasonPhrase + "\n" + endpoint);

        return currencyTableResponse[0];
    }
    public async Task<List<CurrencyTable>> GetCurrencyTableNewestCount(string tableCode, int last)
    {
        string endpoint = BuildEndpointString(tableCode, topCount: last);

        HttpResponseMessage response = await _httpClient.GetAsync(endpoint);
        List<CurrencyTable> currencyTableResponse = new();
        if (response.StatusCode == HttpStatusCode.OK)
            currencyTableResponse = await response.Content.ReadAsAsync<List<CurrencyTable>>();
        else throw new HttpRequestException(response.ReasonPhrase + "\n" + endpoint);

        return currencyTableResponse;
    }


    public async Task<CurrencyTable> GetCurrencyTableInDate(string tableCode, DateTime date)
    {
        string endpoint = BuildEndpointString(tableCode, date);
        List<CurrencyTable> ctMultiple = await CTGetterMultiple(endpoint);
        return ctMultiple[0];
    }
    public async Task<CurrencyTable> GetCurrencyTableInDateRange(string tableCode, DateTime from, DateTime to)
    {
        string endpoint = BuildEndpointString(tableCode, from, to);
        List<CurrencyTable> ctMultiple = await CTGetterMultiple(endpoint);
        return ctMultiple[0];
    }

    public async Task<List<CurrencyTable>> GetAllCurrencyTablesNewest()
    {
        foreach (string tableCode in tableCodes)
        {
            string endpoint = BuildEndpointString(tableCode);
            List<CurrencyTable> currencyTableResponse = await CTGetterMultiple(endpoint);
            currencyTables.Add(currencyTableResponse[0]);
        }

        return currencyTables;
    }
    public async Task<List<CurrencyTable>> GetAllCurrencyTablesNewestCount(int last)
    {
        foreach (string tableCode in tableCodes)
        {
            try
            {
                string endpoint = BuildEndpointString(tableCode, topCount: last);
                List<CurrencyTable> currencyTableResponse = await CTGetterMultiple(endpoint);

                foreach (CurrencyTable currencyTable in currencyTableResponse)
                { currencyTables.Add(currencyTable); }
            }
            catch (Exception ex) { 
                Console.WriteLine(ex.ToString()); // it would be nice to get a road for log from class lib to wpf,
                // thats doable, but no time ig
            }
        }

        return currencyTables;
    }
    public async Task<List<CurrencyTable>> GetAllCurrencyTablesInDate(DateTime date)
    {
        foreach (string tableCode in tableCodes)
        {
            string endpoint = BuildEndpointString(tableCode, date);
            List<CurrencyTable> currencyTableResponse = await CTGetterMultiple(endpoint);
            currencyTables.Add(currencyTableResponse[0]);
        }
        return currencyTables;
    }
    public async Task<List<CurrencyTable>> GetAllCurrencyTablesInDateRange(DateTime from, DateTime to)
    {
        foreach (string tableCode in tableCodes)
        {
            string endpoint = BuildEndpointString(tableCode, from, to);
            List<CurrencyTable> currencyTablesResponse = await CTGetterMultiple(endpoint);
            foreach(CurrencyTable currencyTable in currencyTablesResponse)
                { currencyTables.Add(currencyTable); }
        }
        return currencyTables;
    }

}
