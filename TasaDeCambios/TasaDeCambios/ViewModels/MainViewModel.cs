namespace TasaDeCambios.ViewModels
{
    using GalaSoft.MvvmLight.Command;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows.Input;
    using TasaDeCambios.Models;
    using System.Collections.Generic;
    using Xamarin.Forms;
    using TasaDeCambios.Helpers;
    using TasaDeCambios.Services;
    using System;
    using System.Threading.Tasks;

    public class MainViewModel : INotifyPropertyChanged
    {
        #region Servicios
        ApiService apiService;
        DialogService dialogService;
        DataService dataService;
        #endregion
        #region Atributos

        bool _isRunning;
        bool _isEnabled;
        string _result;
        Rate _sourceRate;
        Rate _targetRate;
        string _status;
        List<Rate> rates;
        ObservableCollection<Rate> _rates;

        #endregion

        #region Comandos

        public ICommand ConvertCommand { get { return new RelayCommand(Convert); } }

        async void Convert()
        {
            if (string.IsNullOrEmpty(Amount))
            {
                await dialogService.ShowMessage(Lenguages.Error, "Debe entroducir un valor a convertir"); //titulo, error como tal
                return;
            }

            decimal amount = 0;

            if (!decimal.TryParse(Amount, out amount))
            {
                await dialogService.ShowMessage(Lenguages.Error, "Debe ingresar un valor numerico"); //titulo, error como tal
                return;
            }

            if (SourceRate == null)
            {
                await dialogService.ShowMessage(Lenguages.Error, "Debe seleccionar una tasa de origen "); //titulo, error como tal
                return;
            }

            if (TargetRate == null)
            {
                await dialogService.ShowMessage(Lenguages.Error, "Debe seleccionar una tasa destino"); //titulo, error como tal
                return;
            }

            var amountConverted = amount /
                                        (decimal)SourceRate.TaxRate *
                                        (decimal)TargetRate.TaxRate;

            Result = string.Format("{0} {1:C2} = {2} {3:C2}",
                                    SourceRate.Code,
                                    amount,
                                    TargetRate.Code,
                                    amountConverted);
        }

        public ICommand SwitchCommand { get { return new RelayCommand(Switch); } }

        void Switch()
        {
            var aux = SourceRate;
            SourceRate = TargetRate;
            TargetRate = aux;
            Convert();
        }

        #endregion

        #region Constructores

        public MainViewModel()
        {
            apiService = new ApiService();
            dialogService = new DialogService();
            dataService = new DataService();
            LoadRates();
        }

        #endregion

        #region Eventos

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion     

        #region Metodos

        async void LoadRates()
        {
            IsEnabled = false;
            IsRunning = true;
            Result = Lenguages.Loading;

            var conection = await apiService.CheckConnection();
            if (!conection.IsSuccess)
            {
                //IsRunning = false;
                //Result = conection.Message;
                //return;
                LoadLocalData();
            }
            else
            {
                await LoadDataFromAPI();
            }
            
            if (rates.Count == 0)
            {
                IsRunning = false;
                IsEnabled = false;
                Result = "There are not internet connection and not load previously rates. Please try again with internet connection";
                Status = "Nohay tasas cargadas";
                return;
            }

            Rates = new ObservableCollection<Rate>(rates);
            
            IsRunning = false;
            IsEnabled = true;
            Result = Lenguages.Ready;
           

        }

        async Task LoadDataFromAPI()
        {
            var url = Application.Current.Resources["URLAPI"].ToString();
            var response = await apiService.GetList<Rate>(
                url,
                "/api/Rates");

            if (!response.IsSuccess)
            {
                LoadLocalData();
                return;
            }

            rates = (List<Rate>)response.Result; //tasas es lo que devolcio el servicio
            dataService.DeleteAll<Rate>(); //borra todo el modelo de tasas
            dataService.Save(rates); //graba en tasas lo que llego

            Status = "Tasas Cargadas de Internet";
        }

        void LoadLocalData()
        {
            rates = dataService.Get<Rate>(false);
            Status = "Tasas Cargadas desde la data local";
        }
        #endregion

        #region Propiedades

        public string Amount { get; set; }

        public ObservableCollection<Rate> Rates
        {
            get { return _rates; }
            set {
                if (_rates != value)
                {
                    _rates = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Rates)));
                }
            }
        }

        //public Rate SourceRate { get; set; }

        //public Rate TargetRate { get; set; }

        public string Status
        {
            get { return _status; }
            set {
                if (_status != value)
                {
                    _status = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Status)));
                }
            }
        }

        public Rate SourceRate
        {
            get { return _sourceRate; }
            set {
                if (_sourceRate != value)
                {
                    _sourceRate = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SourceRate)));
                }
            }
        }

        public Rate TargetRate
        {
            get { return _targetRate; }
            set {
                if (_targetRate != value)
                {
                    _targetRate = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TargetRate)));
                }
            }
        }

        public bool IsRunning
        {
            get { return _isRunning; }
            set {
                if (_isRunning != value)
                {
                    _isRunning = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsRunning)));
                }
            }
        }

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsEnabled)));
                }
            }
        }

        public string Result
        {
            get { return _result; }
            set {
                if (_result != value)
                {
                    _result = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Result)));
                }
            }
        }

        #endregion

    }
}
