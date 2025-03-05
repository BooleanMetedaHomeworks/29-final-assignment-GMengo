using ristorante_frontend.Models;
using System.Net.Http.Json;
using System.Net.Http;
using System.Security.Claims;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;

namespace ristorante_frontend.Services
{

    public enum ApiServiceResultType
    {
        Success,
        Error
    }
    public static class ApiService
    {
        public const string API_URL = "http://localhost:5000";
        public static string Email { get; set; }
        public static string Password { get; set; }
        public static async Task<ApiServiceResult<bool>> Register()
        {
            try
            {
                using HttpClient client = new HttpClient();
                var response = await client.PostAsync($"{API_URL}/Account/Register",
                    JsonContent.Create(new { Email = Email, Password = Password }));

                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return new ApiServiceResult<bool>(true);
                }
                else
                {
                    // Tentativo di estrarre il messaggio d'errore direttamente
                    string errorMessage = "Registrazione fallita";
                    try
                    {
                        var errorObject = JsonDocument.Parse(responseBody);
                        if (errorObject.RootElement.TryGetProperty("message", out JsonElement messageProp))
                        {
                            errorMessage = messageProp.GetString() ?? errorMessage;
                        }
                    }
                    catch { /* Se il parsing fallisce, mantieni il messaggio di default */ }

                    return new ApiServiceResult<bool>(new Exception(errorMessage));
                }
            }
            catch (Exception ex)
            {
                return new ApiServiceResult<bool>(ex);
            }
        }

        public static async Task<ApiServiceResult<Jwt>> GetJwtToken()
        {
            try
            {
                using HttpClient client = new HttpClient();
                var httpResult = await client.PostAsync($"{API_URL}/Account/Login",
                    JsonContent.Create(new { Email = Email, Password = Password }));

                var resultBody = await httpResult.Content.ReadAsStringAsync();

                if (!httpResult.IsSuccessStatusCode)
                {
                    // Gestione errori dal backend
                    string errorMessage = "Login fallito";
                    try
                    {
                        var errorObject = JsonDocument.Parse(resultBody);
                        if (errorObject.RootElement.TryGetProperty("message", out JsonElement messageProp))
                        {
                            errorMessage = messageProp.GetString() ?? errorMessage;
                        }
                    }
                    catch { /* Ignora errori di parsing */ }

                    return new ApiServiceResult<Jwt>(new Exception(errorMessage));
                }

                var data = JsonConvert.DeserializeObject<Jwt>(resultBody);

                if (string.IsNullOrEmpty(data?.Token))
                {
                    return new ApiServiceResult<Jwt>(new Exception("Token non valido"));
                }

                AddRolesToJwt(data);
                return new ApiServiceResult<Jwt>(data);
            }
            catch (Exception e)
            {
                return new ApiServiceResult<Jwt>(e);
            }
        }
        private static void AddRolesToJwt(Jwt jwt)
        {
            try
            {
                // Decodifichiamo il JWT
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(jwt.Token);

                // Vediamo se ci sono ruoli nel JWT
                var roles = jwtToken.Claims
                    .Where((Claim c) => c.Type == "role")
                    .Select(c => c.Value).ToList();

                // Aggiungiamoli nel nostro DTO (data transfer object) rappresentante il JWT
                jwt.Roles = roles;
            }
            catch { } // Se succede qualcosa non facciamo nulla
        }
    }
}