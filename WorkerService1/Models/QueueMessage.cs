using System.Text.Json.Serialization;

namespace StockQuoteChat.Bot.Models
{
    public class QueueMessage
    {
        [JsonConstructor]
        public QueueMessage()
        {
        }

        public QueueMessage(Room room, string stockCode, string quoteValue)
        {
            Message = $"{stockCode.ToUpper()} quote is ${quoteValue} per share"; ;
            Room = room;
        }

        public string Message { get; set; }
        public Room Room { get; set; }
    }
}
