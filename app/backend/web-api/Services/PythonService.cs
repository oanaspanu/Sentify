using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace AnalysisAPI.Services
{
    public class PythonService
    {
        private readonly HttpClient _httpClient;

        public PythonService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> RunPythonScript(string text)
        {
            var content = new StringContent(JsonConvert.SerializeObject(new { text = text }), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("http://localhost:8000/predict", content);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(json);

            return result.sentiment.ToString();
        }
    }
}
