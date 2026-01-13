using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Barbershop.NetworkingLayer
{
    internal sealed class EmailVerifier: IEmailVerifier
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        public async Task<bool> IsValidEmailAsync(string email)
        {
            try
            {
                string url = $"https://rapid-email-verifier.fly.dev/api/validate?email={email}";
                HttpResponseMessage response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return false;
                }

                string jsonString = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<ApiResponse>(jsonString);
                return result != null && result.Status == "VALID";
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
