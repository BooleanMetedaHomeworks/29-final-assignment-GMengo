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
using System.Threading.Tasks; // Aggiunto per Task

namespace ristorante_frontend.ViewModels
{
    public class PiattoViewModel : INotifyPropertyChanged
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private const string ApiBaseUrl = "http://localhost:5000/Piatto";

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
                var newPiatto = new Piatto
                {
                    Nome = "Nuovo piatto",
                    Descrizione = "Descrizione",
                    Prezzo = 0.0
                };

                var json = JsonConvert.SerializeObject(newPiatto);
                var response = await _httpClient.PostAsync(
                    ApiBaseUrl,
                    new StringContent(json, Encoding.UTF8, "application/json"));

                response.EnsureSuccessStatusCode();
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
                var response = await _httpClient.DeleteAsync($"{ApiBaseUrl}/{SelectedPiatto.Id}");
                response.EnsureSuccessStatusCode();
                Piatti.Remove(SelectedPiatto);
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