using System.Net.Http.Json;
using users.Models;
namespace users.Services
{
    public class UserService
    {
        private readonly HttpClient _httpClient;

        public UserService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(BaseAddress);
        }
        public static string BaseAddress = "http://10.0.2.2:3011"; //DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:3011" : "http://localhost:3011";
    
        public async Task<List<User>> GetUsersAsync()
        {
            try
            {
                var users = await _httpClient.GetFromJsonAsync<List<User>>("/api/v1/users");
                return users ?? [];
            }
            catch
            {
                // Handle/log error
                //Console.WriteLine($"Error fetching users: {ex.Message}");
                //return [];
                throw;
            }
        }
    }

}