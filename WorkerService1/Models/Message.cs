namespace StockQuoteChat.Bot.Models
{
    public class Message
    {
        public Message()
        {
            Id = Guid.NewGuid();
            Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
        }

        public Message(string body, UserConnection userConnection)
        {
            Id = Guid.NewGuid();
            Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            Body = body;
            UserId = userConnection.User.Id;
            RoomId = userConnection.Room.Id;
        }

        public Guid Id { get; set; }
        public long Timestamp { get; set; }
        public string Body { get; set; }
        public Guid UserId { get; set; }
        public Guid RoomId { get; set; }
    }
}
