using KantorLogic.api;
using KantorLogic.models;
using KantorWPF.AdditionalModels;
using KantorWPF.MVVM;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows;
using System.Drawing;

namespace KantorWPF.ViewModel
{
    /// <summary>
    /// View model that holds data from any table from NBP API
    /// </summary>
    internal class MainWindowViewModel : ViewModelBase
    {
        private Image upImage = Image.FromFile("../../../images/exchange_up.png");
        private Image downImage = Image.FromFile("../../../images/exchange_down.png");
        public RelayCommand LoadCommand => new RelayCommand(execute => LoadExchangeData());
        //public RelayCommand SearchCommand => new RelayCommand(execute => SearchData());

        public async Task SearchData()
        {
            if (SearchPhrase == "Szukaj...") return;

            CurTable = fallbackCurTable;
            CompleteCurrencyTable results = new CompleteCurrencyTable();

            results.No = CurTable.No;
            results.TradingRate = CurTable.TradingRate;
            results.EffectiveDate = CurTable.EffectiveDate;
            results.Table = CurTable.Table;
            results.Rates = new List<CompleteCurrencyRateInfo>();

            foreach(var item in CurTable.Rates)
            {
                if(item.Code.ToLower().Contains(SearchPhrase.ToLower()) ||
                   item.Currency.ToLower().Contains(SearchPhrase.ToLower()) )
                {
                    results.Rates.Add(item);
                }
            }

            CurTable = results;
        }

        public string[] AvailableTables { get; } =  ["A", "B", "C"];

        private string tableCode;
        public string TableCode
        {
            get { return tableCode; }
            set { tableCode = value; OnPropertyChanged(); }
        }

        private string searchPhrase = "Szukaj...";
        public string SearchPhrase
        {
            get { return searchPhrase; }
            set { searchPhrase = value; OnPropertyChanged(); }
        }

        private Visibility[] columnVisibility = [Visibility.Collapsed, Visibility.Collapsed, Visibility.Collapsed]; // mid, bid, ask
        public Visibility[] ColumnVisibility
        {
            get { return columnVisibility; }
            set { columnVisibility = value; OnPropertyChanged(); }
        }

        private CompleteCurrencyTable fallbackCurTable;
        private CompleteCurrencyTable curTable = new();
        public CompleteCurrencyTable CurTable
        {
            get { return curTable; }
            set
            { curTable = value; OnPropertyChanged(); }
        }

        // top 10 druciarskich kodow
        private async Task<Image> GetCurrencyFlag(string currencyCode)
        {
            using (var httpClient = new HttpClient())
            {
                string jsonCurrencyMapping = File.ReadAllText("currencies.json");
                Dictionary<string, string> currencyMappings = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonCurrencyMapping);

                string countryCode = null;
                currencyMappings.TryGetValue(currencyCode, out countryCode);
                if (countryCode != null)
                {
                    var flagResponse = await httpClient.GetAsync($"https://flagcdn.com/h40/{countryCode.ToLower()}.png");
                    if (flagResponse.StatusCode == HttpStatusCode.OK)
                    {
                        using (MemoryStream imgStream = (MemoryStream)flagResponse.Content.ReadAsStream())
                        {
                            string filename = $"{countryCode.ToLower()}.png";
                            if (!File.Exists(filename)) 
                                Image.FromStream(imgStream).Save(filename);
                            return Image.FromFile(filename);

                            // ArgumentException ImageFormat.COKOLWIEK w Converterze
                            //using (var objImage = Image.FromStream(imgStream))
                            //{
                            //    using (var bmp = new Bitmap(objImage))
                            //    {
                            //        flag = bmp;
                            //    }
                            //}
                        }
                    }
                }
            }

            return null;
        }

        private Image? UpOrDown(Decimal? newValue, Decimal? oldValue)
        {
            if (newValue > oldValue) { return upImage; }
            else { return downImage; }
        }
        private async Task LoadExchangeData()
        {
            for(int i = 0; i < 3; i++) { ColumnVisibility[i] = Visibility.Collapsed; }
            if (TableCode == "A" || TableCode == "B")  { ColumnVisibility[0] = Visibility.Visible; }
            else if (TableCode == "C")                 { ColumnVisibility[1] = Visibility.Visible; 
                                                         ColumnVisibility[2] = Visibility.Visible; }

            DataAccess exchangeDA = new();
            List<CurrencyTable> tempTables = await exchangeDA.GetCurrencyTableNewestCount(TableCode, 2);
            CompleteCurrencyTable tempCompleteCurrTable = new();
            tempCompleteCurrTable.Rates = new();

            try
            {
                tempCompleteCurrTable.EffectiveDate = tempTables[0].EffectiveDate;
                tempCompleteCurrTable.Table = tempTables[0].Table;
                tempCompleteCurrTable.TradingRate = tempTables[0].TradingRate;
                tempCompleteCurrTable.No = tempTables[0].No;

                int auxRateCount = 0;
                foreach (CodedRate rate in tempTables[0].Rates)
                { 

                    Image askUpDown = null, bidUpDown = null, midUpDown = null;
                    if (TableCode == "A" || TableCode == "B") {
                        midUpDown = UpOrDown(tempTables[1].Rates[auxRateCount].Mid, rate.Mid);
                    }
                    else if (TableCode == "C")
                    {
                        bidUpDown = UpOrDown(tempTables[1].Rates[auxRateCount].Bid, rate.Bid);
                        askUpDown = UpOrDown(tempTables[1].Rates[auxRateCount].Ask, rate.Ask);
                    }
                    // else - cenyzlota

                    CompleteCurrencyRateInfo toAdd = new()
                    {
                        FlagImage = null,
                        Code = rate.Code,
                        Currency = rate.Currency,
                        Bid = rate.Bid,
                        UpDownBidImage = bidUpDown,
                        Ask = rate.Ask,
                        UpDownAskImage = askUpDown,
                        Mid = rate.Mid,
                        UpDownMidImage = midUpDown,
                    };
                    tempCompleteCurrTable.Rates.Add(toAdd);
                    auxRateCount++;
                }

                CurTable = tempCompleteCurrTable;
                fallbackCurTable = tempCompleteCurrTable;

                foreach(var rate in tempCompleteCurrTable.Rates)
                {
                    Image flag = await GetCurrencyFlag(rate.Code);
                    rate.FlagImage = flag;
                }
                CurTable = tempCompleteCurrTable;
                fallbackCurTable = tempCompleteCurrTable;
            }
            
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
