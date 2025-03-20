using KantorLogic.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http.Formatting;
using System.Data;

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
        string dateForQuery = date.Year.ToString() + '-' +
                              date.Month.ToString() + '-' +
                              date.Day.ToString() 
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
        string dateStartForQuery = from.Year.ToString() + '-' +
                                   from.Month.ToString() + '-' +
                                   from.Day.ToString()
                                   ?? throw new Exception("Nie podano daty");

        string dateEndForQuery = to.Year.ToString() + '-' +
                                 to.Month.ToString() + '-' +
                                 to.Day.ToString()
                                 ?? throw new Exception("Nie podano daty");

        result = _apiUrl + $"{subPath}/{dateStartForQuery}/{dateEndForQuery}";

        return result;
    }
    public async Task<List<CurrencyTable>> GetAllCurrencyTablesNewest()
    {
        foreach (string tableCode in tableCodes)
        {
            HttpResponseMessage response = await _httpClient.GetAsync(BuildEndpointString(tableCode));
            if (response.StatusCode == HttpStatusCode.OK)
            {
                List<CurrencyTable> currencyTableResponse = await response.Content.ReadAsAsync<List<CurrencyTable>>();
                currencyTables.Add(currencyTableResponse[0]);
            }
        }

        return currencyTables;
    }
    //public async Task<List<CurrencyTable>> GetAllCurrencyTablesByDate(DateTime date)
    //{
    //    string strDate = date.Year.ToString() + '-' +
    //                     date.Month.ToString() + '-' +
    //                     date.Day.ToString();

    //    foreach (string tableCode in tableCodes)
    //    {
    //        HttpResponseMessage response = await _httpClient.GetAsync(_apiUrl + $"exchangerates/tables/{tableCode}");
    //        if (response.StatusCode == HttpStatusCode.OK)
    //        {
    //            List<CurrencyTable> currencyTableResponse = await response.Content.ReadAsAsync<List<CurrencyTable>>();
    //            currencyTables.Add(currencyTableResponse[0]);
    //        }
    //    }
    //    return currencyTables;
    //}
    public async Task<CurrencyTable> GetNewestCurrencyTable(string tableCode)
    {
        // nie wiem, czy to sie przyda...
        //if (currencyTables.Count == 3)
        //{
        //    return (from currencyTable in currencyTables
        //            where currencyTable.Table == tableCode
        //            select currencyTable).FirstOrDefault();
        //}

        HttpResponseMessage response = await _httpClient.GetAsync(BuildEndpointString(tableCode));
        List<CurrencyTable> currencyTableResponse = new();
        if (response.StatusCode == HttpStatusCode.OK)
            currencyTableResponse = await response.Content.ReadAsAsync<List<CurrencyTable>>();

        return currencyTableResponse[0];
    }
}
