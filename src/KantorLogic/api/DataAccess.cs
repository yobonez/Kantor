using KantorLogic.models;
using System.Net;

namespace KantorLogic.api;

public class DataAccess
{
    private HttpClient _httpClient;
    private readonly string _apiUrl = "https://api.nbp.pl/api/";
    private List<Currencies> currencyTables;

    private readonly string[] tableCodes = { "A", "B", "C" };
    public DataAccess()
    {
        _httpClient = new HttpClient();
        currencyTables = new List<Currencies>();
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
    // single currency retrieval
    public async Task<CurrencyValues> GetCurrencyNewest(string tableCode, string? currencyCode)
    {
        string endpoint = BuildEndpointString(tableCode, currencyCode: currencyCode);

        HttpResponseMessage response = await _httpClient.GetAsync(endpoint);
        CurrencyValues currencyValuesResponse = new();
        if (response.StatusCode == HttpStatusCode.OK)
            currencyValuesResponse = await response.Content.ReadAsAsync<CurrencyValues>();
        else throw new HttpRequestException(response.ReasonPhrase + "\n" + endpoint);

        return currencyValuesResponse;
    }
    public async Task<CurrencyValues> GetCurrencyNewestCount(string tableCode, string? currencyCode, int last)
    {
        string endpoint = BuildEndpointString(tableCode, currencyCode: currencyCode, topCount: last);

        HttpResponseMessage response = await _httpClient.GetAsync(endpoint);
        CurrencyValues currencyValuesResponse = new();
        if (response.StatusCode == HttpStatusCode.OK)
            currencyValuesResponse = await response.Content.ReadAsAsync<CurrencyValues>();
        else throw new HttpRequestException(response.ReasonPhrase + "\n" + endpoint);

        return currencyValuesResponse;
    }
    public async Task<CurrencyValues> GetCurrencyInDate(string tableCode, string? currencyCode, DateTime date)
    {
        string endpoint = BuildEndpointString(tableCode, date, currencyCode: currencyCode);

        HttpResponseMessage response = await _httpClient.GetAsync(endpoint);
        CurrencyValues currencyValuesResponse = new();
        if (response.StatusCode == HttpStatusCode.OK)
            currencyValuesResponse = await response.Content.ReadAsAsync<CurrencyValues>();
        else throw new HttpRequestException(response.ReasonPhrase + "\n" + endpoint);

        return currencyValuesResponse;
    }
    public async Task<CurrencyValues> GetCurrencyInDateRange(string tableCode, string? currencyCode, DateTime from, DateTime to)
    {
        string endpoint = BuildEndpointString(tableCode, from, to, currencyCode: currencyCode);

        HttpResponseMessage response = await _httpClient.GetAsync(endpoint);
        CurrencyValues currencyValuesResponse = new();
        if (response.StatusCode == HttpStatusCode.OK)
            currencyValuesResponse = await response.Content.ReadAsAsync<CurrencyValues>();
        else throw new HttpRequestException(response.ReasonPhrase + "\n" + endpoint);

        return currencyValuesResponse;
    }
    public async Task<List<GoldRate>> GetGoldPriceNewest()
    {
        string endpoint = BuildEndpointString("gold");

        HttpResponseMessage response = await _httpClient.GetAsync(endpoint);
        List<GoldRate> goldRatesResponse = new();
        if (response.StatusCode == HttpStatusCode.OK)
            goldRatesResponse = await response.Content.ReadAsAsync<List<GoldRate>>();
        else throw new HttpRequestException(response.ReasonPhrase + "\n" + endpoint);

        return goldRatesResponse;
    }
    public async Task<List<GoldRate>> GetGoldPriceInDate(DateTime date)
    {
        string endpoint = BuildEndpointString("gold", date);

        HttpResponseMessage response = await _httpClient.GetAsync(endpoint);
        List<GoldRate> goldValuesResponse = new();
        if (response.StatusCode == HttpStatusCode.OK)
            goldValuesResponse = await response.Content.ReadAsAsync<List<GoldRate>>();
        else throw new HttpRequestException(response.ReasonPhrase + "\n" + endpoint);

        return goldValuesResponse;
    }
    public async Task<List<GoldRate>> GetGoldPriceInDateRange(DateTime from, DateTime to)
    {
        string endpoint = BuildEndpointString("gold", from, to);

        HttpResponseMessage response = await _httpClient.GetAsync(endpoint);
        List<GoldRate> goldValuesResponse = new();
        if (response.StatusCode == HttpStatusCode.OK)
            goldValuesResponse = await response.Content.ReadAsAsync<List<GoldRate>>();
        else throw new HttpRequestException(response.ReasonPhrase + "\n" + endpoint);

        return goldValuesResponse;
    }

    // retrieval of multiple currencies
    public async Task<Currencies> GetCurrencyTableNewest(string tableCode)
    {
        string endpoint = BuildEndpointString(tableCode);

        HttpResponseMessage response = await _httpClient.GetAsync(endpoint);
        List<Currencies> currencyTableResponse = new();
        if (response.StatusCode == HttpStatusCode.OK)
            currencyTableResponse = await response.Content.ReadAsAsync<List<Currencies>>();
        else throw new HttpRequestException(response.ReasonPhrase + "\n" + endpoint);

        return currencyTableResponse[0];
    }
    public async Task<Currencies> GetCurrencyTableInDate(string tableCode, DateTime date)
    {
        string endpoint = BuildEndpointString(tableCode, date);

        HttpResponseMessage response = await _httpClient.GetAsync(endpoint);
        List<Currencies> currencyTableResponse = new();
        if (response.StatusCode == HttpStatusCode.OK)
            currencyTableResponse = await response.Content.ReadAsAsync<List<Currencies>>();
        else throw new HttpRequestException(response.ReasonPhrase + "\n" + endpoint);

        return currencyTableResponse[0];
    }
    public async Task<List<Currencies>> GetAllCurrencyTablesNewest()
    {
        foreach (string tableCode in tableCodes)
        {
            string endpoint = BuildEndpointString(tableCode);
            HttpResponseMessage response = await _httpClient.GetAsync(endpoint);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                List<Currencies> currencyTableResponse = await response.Content.ReadAsAsync<List<Currencies>>();
                currencyTables.Add(currencyTableResponse[0]);
            }
        }

        return currencyTables;
    }
    public async Task<Currencies> GetCurrencyTableInDateRange(string tableCode, DateTime from, DateTime to)
    {
        string endpoint = BuildEndpointString(tableCode, from, to);

        HttpResponseMessage response = await _httpClient.GetAsync(endpoint);
        List<Currencies> currencyTableResponse = new();
        if (response.StatusCode == HttpStatusCode.OK)
            currencyTableResponse = await response.Content.ReadAsAsync<List<Currencies>>();
        else throw new HttpRequestException(response.ReasonPhrase + "\n" + endpoint);

        return currencyTableResponse[0];
    }
    public async Task<List<Currencies>> GetAllCurrencyTablesInDate(DateTime date)
    {
        foreach (string tableCode in tableCodes)
        {
            string endpoint = BuildEndpointString(tableCode, date);

            HttpResponseMessage response = await _httpClient.GetAsync(endpoint);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                List<Currencies> currencyTableResponse = await response.Content.ReadAsAsync<List<Currencies>>();
                currencyTables.Add(currencyTableResponse[0]);
            }
            else throw new HttpRequestException(response.ReasonPhrase + "\n" + endpoint);
        }
        return currencyTables;
    }
    public async Task<List<Currencies>> GetAllCurrencyTablesInDateRange(DateTime from, DateTime to)
    {
        foreach (string tableCode in tableCodes)
        {
            string endpoint = BuildEndpointString(tableCode, from, to);

            HttpResponseMessage response = await _httpClient.GetAsync(endpoint);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                List<Currencies> currencyTablesResponse = await response.Content.ReadAsAsync<List<Currencies>>();
                foreach(Currencies currencyTable in currencyTablesResponse)
                    { currencyTables.Add(currencyTable); }
            }
            else throw new HttpRequestException(response.ReasonPhrase + "\n" + endpoint);
        }
        return currencyTables;
    }

}
