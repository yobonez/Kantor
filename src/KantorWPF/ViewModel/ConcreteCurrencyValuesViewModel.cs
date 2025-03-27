using KantorLogic.api;
using KantorLogic.models;
using KantorWPF.MVVM;

namespace KantorWPF.ViewModel
{
    /// <summary>
    /// View model that holds data from any table from NBP API
    /// </summary>
    internal class ConcreteCurrencyValuesViewModel : ViewModelBase
    {
        private ConcreteCurrencyValues ccvEntry;
        public ConcreteCurrencyValues CCVEntry
        {
            get { return ccvEntry; }
            set { 
                ccvEntry = value;
                OnPropertyChanged();
            }
        }
        //najważniejsze: ogarnąć w tym patternie gdzie odbierać dane

        // parent by miał CCV, a tutaj by bhły te pojedyncze entry
        // moze skminić, jak zrobić swój własny atrybut ItemsSource na jakimś parencie (datagrid nie przejdzie,
        // bo chce populować grida usercontrolami)????
        // ItemsSource z ListBox może???????

        // tu możliwe, że będzie potrzebny kolejny model, trochę to będzie pogmatwane, modele z tego i tego
        // ogarnąć relaycommandy (execute, canexecute i te inne fajne rzeczy)
        // np. na loadzie elementu parentowego dać event, który załaduje tabelę, potem
        // 
        //public ConcreteCurrencyValuesViewModel(string currencyCode)
        //{
        //    DataAccess exchangeDA = new();
        //    CCVEntry = exchangeDA.GetCurrencyNewest("C", currencyCode);
        //}
    }
}
