using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ristorante_frontend.Views
{
    public partial class RegisterPage : Page
    {
        public RegisterPage()
        {
            InitializeComponent();
        }

        private async void OnRegisterBtnClick(object sender, RoutedEventArgs e)
        {
            string email = EmailTxt.Text;
            string password = PasswordTxt.Password;
            string confirmPassword = ConfirmPasswordTxt.Password;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Inserisci tutti i campi.", "Errore", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Le password non coincidono.", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var user = new { Email = email, Password = password };
            string json = JsonSerializer.Serialize(user);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await App.HttpClient.PostAsync("Account/Register", content);
                string responseText = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Registrazione avvenuta con successo!", "Successo", MessageBoxButton.OK, MessageBoxImage.Information);
                    NavigationService?.Navigate(new Uri("Views/HomePage.xaml", UriKind.Relative));
                }
                else
                {
                    MessageBox.Show($"Errore: {responseText}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore di connessione: {ex.Message}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnNavigateToLoginClick(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("Views/LoginPage.xaml", UriKind.Relative));
        }
    }
}
