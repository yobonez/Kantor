using KantorLogic.api;
using KantorLogic.models;
using KantorWPF.AdditionalModels;
using KantorWPF.MVVM;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Windows;
using System.Windows.Navigation;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace KantorWPF.ViewModel
{
    /// <summary>
    /// View model that holds data from any table from NBP API
    /// </summary>
    internal class MainWindowViewModel : ViewModelBase
    {
        private System.Drawing.Image upImage = System.Drawing.Image.FromFile("../../../images/exchange_up.png");
        private System.Drawing.Image downImage = System.Drawing.Image.FromFile("../../../images/exchange_down.png");
        public RelayCommand LoadCommand => new RelayCommand(execute => LoadExchangeData());

        public string[] AvailableTables
        {
            get;
        } = ["A", "B", "C"];

        private string tableCode;
        public string TableCode
        {
            get { return tableCode; }
            set { tableCode = value; OnPropertyChanged(); }
        }

        private Visibility[] columnVisibility = [Visibility.Collapsed, Visibility.Collapsed, Visibility.Collapsed]; // mid, bid, ask
        public Visibility[] ColumnVisibility
        {
            get { return columnVisibility; }
            set { columnVisibility = value; OnPropertyChanged(); }
        }

        private CompleteCurrencyTable curTable = new();
        public CompleteCurrencyTable CurTable
        {
            get { return curTable; }
            set
            { curTable = value; OnPropertyChanged(); }
        }

        // top 10 druciarskich kodow
        private async Task<System.Drawing.Image> GetCurrencyFlag(string currencyCode)
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
                            System.Drawing.Image.FromStream(imgStream).Save($"{countryCode.ToLower()}.png");
                            return System.Drawing.Image.FromFile($"{countryCode.ToLower()}.png");

                            // ArgumentException ImageFormat.COKOLWIEK w Converterze
                            //using (var objImage = System.Drawing.Image.FromStream(imgStream))
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

        private System.Drawing.Image? UpOrDown(Decimal? newValue, Decimal? oldValue)
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
                    System.Drawing.Image flag = await GetCurrencyFlag(rate.Code);

                    System.Drawing.Image askUpDown = null, bidUpDown = null, midUpDown = null;
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
                        FlagImage = flag,
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
            }
            
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
