using System.Configuration;
using System.Data;
using System.Net.Http;
using System.Windows;

namespace ristorante_frontend
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static HttpClient HttpClient { get; } = new HttpClient { BaseAddress = new Uri("http://localhost:5000/") };
    }

}
