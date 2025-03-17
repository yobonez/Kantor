using KantorLogic.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http.Formatting;

namespace KantorLogic.api
{
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
        public async Task<List<CurrencyTable>> GetAllNewestCurrencyTables()
        {
            foreach (string tableCode in tableCodes)
            {
                HttpResponseMessage response = await _httpClient.GetAsync(_apiUrl + $"exchangerates/tables/{tableCode}");
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    List<CurrencyTable> currencyTableResponse = await response.Content.ReadAsAsync<List<CurrencyTable>>();
                    currencyTables.Add(currencyTableResponse[0]);
                }
            }

            return currencyTables;
        }
        public async Task<CurrencyTable> GetNewestCurrencyTable(string tableCode)
        {
            if (currencyTables.Count != 3)
            {
                return (from currencyTable in currencyTables
                        where currencyTable.Table == tableCode
                        select currencyTable).FirstOrDefault();
            }

            HttpResponseMessage response = await _httpClient.GetAsync(_apiUrl + $"exchangerates/tables/{tableCode}");
            List<CurrencyTable> currencyTableResponse = new();
            if (response.StatusCode == HttpStatusCode.OK)
                currencyTableResponse = await response.Content.ReadAsAsync<List<CurrencyTable>>();

            return currencyTableResponse[0];
        }
    }
}
