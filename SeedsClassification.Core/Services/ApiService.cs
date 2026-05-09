using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using SeedsClassification.Core.Models;

namespace SeedsClassification.Core.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<List<UserApi>> GetUsersAsync()
        {
            try
            {
                // 🔗 API publique
                string url = "https://dummyjson.com/users";

                // 📥 Requête HTTP
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    throw new Exception("Erreur API");

                var jsonString = await response.Content.ReadAsStringAsync();

                // 📊 Parse JSON
                using JsonDocument doc = JsonDocument.Parse(jsonString);

                var users = new List<UserApi>();

                foreach (var user in doc.RootElement.GetProperty("users").EnumerateArray())
                {
                    users.Add(new UserApi
                    {
                        Id = user.GetProperty("id").GetInt32(),
                        FirstName = user.GetProperty("firstName").GetString(),
                        LastName = user.GetProperty("lastName").GetString()
                    });
                }

                return users;
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur lors de l'appel API : " + ex.Message);
            }
        }
    }
}