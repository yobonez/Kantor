using KantorLogic.models;
using System.ComponentModel.DataAnnotations;
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
    private string BuildEndpointString(string tableCode, 
                                       int topCount = 0,
                                       string currencyCode = null)
    {
        string result = string.Empty;
        string subPath = (tableCode == "gold") 
                         ? "cenyzlota" 
                         : $"exchangerates/tables/{tableCode}";

        if (currencyCode != null && tableCode != "gold")
        {
            if (topCount > 0) result = _apiUrl + $"{subPath}/{currencyCode}/last/{topCount}";
            else result = _apiUrl + $"{subPath}/{currencyCode}";
        }
        if (topCount > 0) result = _apiUrl + $"{subPath}/last/{topCount}";
        else result = _apiUrl + $"{subPath}";

        return result;
    }
    private string BuildEndpointString(string tableCode,
                                       DateTime date,
                                       string currencyCode = null)
    {
        string subPath = (tableCode == "gold")
                         ? "cenyzlota"
                         : $"exchangerates/tables/{tableCode}";

        string result = string.Empty;

        string day = (date.Day < 10) ? "0" + date.Day.ToString() : date.Day.ToString();
        string month = (date.Month < 10) ? "0" + date.Month.ToString() : date.Month.ToString();
        string dateForQuery = date.Year.ToString() + '-' +
                              month + '-' +
                              day
                              ?? "today";

        result = _apiUrl + $"{subPath}/{dateForQuery}";

        return result;
    }
    private string BuildEndpointString(string tableCode,
                                       DateTime from, 
                                       DateTime to,
                                       string currencyCode = null)
    {
        string subPath = (tableCode == "gold")
                         ? "cenyzlota"
                         : $"exchangerates/tables/{tableCode}";
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

        result = _apiUrl + $"{subPath}/{dateStartForQuery}/{dateEndForQuery}";

        return result;
    }
    public async Task<List<CurrencyTable>> GetAllCurrencyTablesNewest()
    {
        foreach (string tableCode in tableCodes)
        {
            string endpoint = BuildEndpointString(tableCode);
            HttpResponseMessage response = await _httpClient.GetAsync(endpoint);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                List<CurrencyTable> currencyTableResponse = await response.Content.ReadAsAsync<List<CurrencyTable>>();
                currencyTables.Add(currencyTableResponse[0]);
            }
        }

        return currencyTables;
    }
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
    // to moze tez przeladowac?
    //public async Task<CurrencyTable> GetCurrencyTableInDate(DateTime date)
    //{
    //    string endpoint = BuildEndpointString(tableCode);

    //    HttpResponseMessage response = await _httpClient.GetAsync(endpoint);
    //    List<CurrencyTable> currencyTableResponse = new();
    //    if (response.StatusCode == HttpStatusCode.OK)
    //        currencyTableResponse = await response.Content.ReadAsAsync<List<CurrencyTable>>();
    //    else throw new HttpRequestException(response.ReasonPhrase + "\n" + endpoint);

    //    return currencyTableResponse[0];
    //}
    public async Task<List<CurrencyTable>> GetAllCurrencyTablesInDate(DateTime date)
    {
        foreach (string tableCode in tableCodes)
        {
            string endpoint = BuildEndpointString(tableCode, date);

            HttpResponseMessage response = await _httpClient.GetAsync(endpoint);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                List<CurrencyTable> currencyTableResponse = await response.Content.ReadAsAsync<List<CurrencyTable>>();
                currencyTables.Add(currencyTableResponse[0]);
            }
            else throw new HttpRequestException(response.ReasonPhrase + "\n" + endpoint);
        }
        return currencyTables;
    }
    public async Task<List<CurrencyTable>> GetAllCurrencyTablesInDateRange(DateTime from, DateTime to)
    {
        foreach (string tableCode in tableCodes)
        {
            string endpoint = BuildEndpointString(tableCode, from, to);

            HttpResponseMessage response = await _httpClient.GetAsync(endpoint);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                List<CurrencyTable> currencyTablesResponse = await response.Content.ReadAsAsync<List<CurrencyTable>>();
                foreach(CurrencyTable currencyTable in currencyTablesResponse)
                    { currencyTables.Add(currencyTable); }
            }
            else throw new HttpRequestException(response.ReasonPhrase + "\n" + endpoint);
        }
        return currencyTables;
    }

}
