using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Windows.Input;
using ristorante_frontend.Models;
using Newtonsoft.Json;
using System.Text;
using System.Windows;
using System; // Aggiunto per Exception e Console
using System.Collections.Generic; // Aggiunto per List<T>
using System.Threading.Tasks;
using ristorante_frontend.Services; // Aggiunto per Task

namespace ristorante_frontend.ViewModels
{
    public class PiattoViewModel : INotifyPropertyChanged
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private const string ApiBaseUrl = "http://localhost:5000/Piatto";
        private Jwt _token;

        public PiattoViewModel()
        {
            Piatti = new ObservableCollection<Piatto>();
            LoadPiattiCommand = new MyCommand(async () => await LoadPiatti());
            AddPiattoCommand = new MyCommand(async () => await AddPiatto());
            DeletePiattoCommand = new MyCommand(
                execute: async () => await DeletePiatto(),
                canExecute: () => SelectedPiatto != null);
            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            // Richiedo il JWT
            var tokenApiResult = await ApiService.GetJwtToken();
            if (tokenApiResult.Data == null)
            {
                MessageBox.Show($"ERRORE di login! {tokenApiResult.ErrorMessage}");
                return;
            }
            _token = tokenApiResult.Data;
            await LoadPiatti();
        }

        private ObservableCollection<Piatto> _piatti;
        public ObservableCollection<Piatto> Piatti
        {
            get => _piatti;
            set
            {
                _piatti = value;
                OnPropertyChanged(nameof(Piatti));
            }
        }

        private Piatto _selectedPiatto;
        public Piatto SelectedPiatto
        {
            get => _selectedPiatto;
            set
            {
                _selectedPiatto = value;
                OnPropertyChanged(nameof(SelectedPiatto));
                (DeletePiattoCommand as MyCommand)?.RaiseCanExecuteChanged();
            }
        }

        public ICommand LoadPiattiCommand { get; }
        public ICommand AddPiattoCommand { get; }
        public ICommand DeletePiattoCommand { get; }

        private async Task LoadPiatti()
        {
            try
            {
                var response = await _httpClient.GetAsync(ApiBaseUrl);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var piatti = JsonConvert.DeserializeObject<List<Piatto>>(content);
                Piatti = new ObservableCollection<Piatto>(piatti);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore nel caricamento dei piatti: {ex.Message}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task AddPiatto()
        {
            try
            {
                // creo un piatto in memoria
                Piatto newPiatto = new Piatto
                {
                    Nome = "Nuovo piatto",
                    Descrizione = "Descrizione",
                    Prezzo = 0.0
                };

                // chiamo l' API per inserire il piatto nel DB
                var createApiResult = await ApiService.CreatePiatto(newPiatto, _token);
                if (createApiResult.Data == null)
                {
                    MessageBox.Show($"Errore nell'aggiunta del piatto: {createApiResult.ErrorMessage}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                // Aggiorno la view solo in caso di successo (altrimenti sarei finito nel return di prima)
                await LoadPiatti();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore nell'aggiunta del piatto: {ex.Message}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task DeletePiatto()
        {
            if (SelectedPiatto == null)
            {
                MessageBox.Show("Seleziona un piatto da eliminare", "Attenzione", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var deleteApiResult = await ApiService.DeletePiatto(SelectedPiatto.Id, _token);
                if (deleteApiResult.Data == 0)
                {
                    MessageBox.Show($"Errore nell'eliminazione del piatto: {deleteApiResult.ErrorMessage}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                await LoadPiatti(); // Aggiorno il view model solo in caso di successo
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore nell'eliminazione del piatto: {ex.Message}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}