using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace MospolitechProject.Services
{
    public class ApiService
    {
        private readonly HttpClient _client = new HttpClient();

        public async Task<string> CheckApiConnection()
        {
            try
            {
                _client.Timeout = TimeSpan.FromSeconds(5);
                // Стучимся на Яндекс, он точно должен быть доступен
                var result = await _client.GetStringAsync("https://yandex.ru");

                return "Сеть доступна! API (через HttpClient) работает.";
            }
            catch (Exception ex)
            {
                return $"Ошибка: {ex.Message}";
            }
        }
    }
}
