using StockQuoteChat.Bot.Models.Requests;
using StockQuoteChat.Bot.Models.Responses;
using System.Text.Json;
using System.Text;
using StockQuoteChat.Bot.Models;
using StockQuoteChat.Bot.Services.Interfaces;

namespace StockQuoteChat.Bot.Services
{
    public class BotService : IBotService
    {
        private readonly HttpClient _httpClient;
        private readonly IMessageManager _messageManager;
        private readonly string _apiUrl = "https://stooq.com/q/l/?s={stockCode}&f=sd2t2ohlcv&h&e=csv";

        public BotService(IMessageManager messageManager)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://localhost:5230");
            _messageManager = messageManager;
        }

        public async Task<ApiResponseDto<LoginResponseDto>> Authenticate()
        {
            LoginRequestDto loginRequestDto = new LoginRequestDto
            {
                Email = "mychatbot@chatmail.com",
                Password = "botexamplepassword"
            };

            var requestContent = new StringContent(JsonSerializer.Serialize(loginRequestDto), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("user/login", requestContent);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var bodyResponse = JsonSerializer.Deserialize<ApiResponseDto<LoginResponseDto>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return bodyResponse!;
            }

            return new ApiResponseDto<LoginResponseDto>();
        }

        public async Task<bool> ProcessMessage(string stockCode, Room room)
        {
            // Get stock_code csv
            var httpClient = new HttpClient();
            var apiResponse = await httpClient.GetStringAsync(_apiUrl.Replace("{stockCode}", stockCode));

            // Get quote value
            var quoteValue = ReadCsvContent(apiResponse);
            if (double.TryParse(quoteValue, out _))
            {
                var queueMessage = new QueueMessage(room, stockCode, quoteValue);

                _messageManager.SendMessage(queueMessage);

                return true;
            }

            return false;
        }

        private string ReadCsvContent(string fileContent)
        {
            var lines = fileContent.Replace("\r", "").Split("\n");

            // Line 0 is the header, and line 1 contains the values
            // We want the value of the "open" header
            var headers = lines[0].ToLower().Split(",");
            var values = lines[1].Split(",");
            var openIndex = Array.IndexOf(headers, "open");

            return values[openIndex];
        }
    }
}
