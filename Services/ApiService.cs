namespace users.Services
{
    public class ApiService : IApiService
    {
        public HttpClient Client { get; }

        public ApiService(HttpClient httpClient)
        {
            httpClient.BaseAddress = new Uri(DeviceInfo.Platform == DevicePlatform.Android
                ? "http://10.0.2.2:3011/api/v1/"
                : "http://localhost:3011/api/v1/");

            Client = httpClient;
        }
    }

}