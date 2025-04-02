using KantorLogic.api;
using KantorLogic.models;
using KantorWPF.AdditionalModels;
using KantorWPF.MVVM;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.NetworkInformation;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace KantorWPF.ViewModel
{
    /// <summary>
    /// View model that holds data from any table from NBP API
    /// </summary>
    internal class MainWindowViewModel : ViewModelBase
    {
        public RelayCommand LoadCommand => new RelayCommand(execute => LoadExchangeData());

        //private CurrencyTable curTable2 = new();
        //public CurrencyTable CurTable2
        //{
        //    get { return curTable2; }
        //    set
        //    {
        //        curTable2 = value;
        //        OnPropertyChanged();
        //    }
        //}

        private CompleteCurrencyTable curTable = new();
        public CompleteCurrencyTable CurTable
        {
            get { return curTable; }
            set
            {
                curTable = value;
                OnPropertyChanged();
            }
        }

        private async Task LoadExchangeData()
        {

            DataAccess exchangeDA = new();
            CurrencyTable tempTable = await exchangeDA.GetCurrencyTableNewest("C");
            CompleteCurrencyTable tempCompleteCurrTable = new();
            tempCompleteCurrTable.Rates = new();

            try
            {
                tempCompleteCurrTable.EffectiveDate = tempTable.EffectiveDate;
                tempCompleteCurrTable.Table = tempTable.Table;
                tempCompleteCurrTable.TradingRate = tempTable.TradingRate;
                tempCompleteCurrTable.No = tempTable.No;
                foreach (CodedRate rate in tempTable.Rates)
                {
                    //Image flag = new Image
                    //{
                    //    Source = new BitmapImage(new Uri($"https://wise.com/public-resources/assets/flags/rectangle/{rate.Code.ToLower()}.png"))
                    //},
                    //updown = new Image
                    //{
                    //    Source = new BitmapImage(new Uri(""))
                    //};
                    //System.Drawing.Image flag
//https://wise.com/public-resources/assets/flags/rectangle/{currencyCode}.png
                    System.Drawing.Image flag = null;
                    using (var httpClient = new HttpClient())
                    {
                        string currencyCode = rate.Code;
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
                                    flag = System.Drawing.Image.FromFile($"{countryCode.ToLower()}.png");

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

                    // TODO: if spada to down if leci w gore up es
                    System.Drawing.Image updown = System.Drawing.Image.FromFile("../../../images/exchange_up.png");
                    
                    CompleteCurrencyRateInfo toAdd = new()
                    {
                        FlagImage = flag,
                        UpDownImage = updown,
                        Code = rate.Code,
                        Currency = rate.Currency,
                        Ask = rate.Ask,
                        Bid = rate.Bid,
                        Mid = rate.Mid,
                    };
                    tempCompleteCurrTable.Rates.Add(toAdd);
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
