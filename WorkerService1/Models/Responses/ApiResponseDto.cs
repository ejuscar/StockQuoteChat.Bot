using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockQuoteChat.Bot.Models.Responses
{
    public class ApiResponseDto<T>
    {
        public ApiResponseDto()
        {
            Data = default;
            Success = false;
            Error = string.Empty;
        }

        public ApiResponseDto(T data, bool success, string error)
        {
            Data = data;
            Success = success;
            Error = error;
        }

        public T? Data { get; set; }
        public bool Success { get; set; }
        public string Error { get; set; }
    }
}
