namespace TasaDeCambios.ViewModels
{
    using GalaSoft.MvvmLight.Command;
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Net.Http;
    using System.Windows.Input;
    using TasaDeCambios.Models;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Xamarin.Forms;

    public class MainViewModel : INotifyPropertyChanged
    {

        #region Atributos

        bool _isRunning;
        bool _isEnabled;
        string _result;
        Rate _sourceRate;
        Rate _targetRate;
        ObservableCollection<Rate> _rates;

        #endregion

        #region Comandos

        public ICommand ConvertCommand { get { return new RelayCommand(Convert); } }

        async void Convert()
        {
            if (string.IsNullOrEmpty(Amount))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Debe entroducir un valor a convertir", "Accept"); //titulo, error como tal
                return;
            }

            decimal amount = 0;

            if (!decimal.TryParse(Amount, out amount))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Debe ingresar un valor numerico", "Accept"); //titulo, error como tal
                return;
            }

            if (SourceRate == null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Debe seleccionar una tasa de origen ", "Accept"); //titulo, error como tal
                return;
            }

            if (TargetRate == null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Debe seleccionar una tasa destino", "Accept"); //titulo, error como tal
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
            Result = "Cargando las tasas...";
            try
            {
                var client = new HttpClient(); //creamos el cliente
                client.BaseAddress = new Uri("http://apiexchangerates.azurewebsites.net"); //le asignamos la Url base
                var controller = "/api/Rates"; //creamos una variable x para el metodo del api
                var response = await client.GetAsync(controller); //creamos una respuesta que viene dada por un get asincrono
                var result = await response.Content.ReadAsStringAsync(); //leemos el resultado

                if (!response.IsSuccessStatusCode) //si no tiene respuesta
                {
                    IsRunning = false;
                    IsEnabled = true;
                    Result = result;
                }

                var rates = JsonConvert.DeserializeObject<List<Rate>>(result); //creamos una variable descerializadora tipo lista del resultado

                Rates = new ObservableCollection<Rate>(rates); //con mi lista de rates armo una obserbable colection de Rates

                IsRunning = false;
                Result = "Listo para Convertir";
                IsEnabled = true;
            }
            catch (Exception ex)
            {
                IsEnabled = true;
                IsRunning = false;
                Result = ex.Message;
            }
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
