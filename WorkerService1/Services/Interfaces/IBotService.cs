using StockQuoteChat.Bot.Models;
using StockQuoteChat.Bot.Models.Responses;

namespace StockQuoteChat.Bot.Services.Interfaces
{
    public interface IBotService
    {
        Task<ApiResponseDto<LoginResponseDto>> Authenticate();
        Task<bool> ProcessMessage(string stockCode, Room room);
    }
}
