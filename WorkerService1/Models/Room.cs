namespace StockQuoteChat.Bot.Models
{
    public class Room
    {
        public Room()
        {
            Id = Guid.NewGuid();
        }

        public Room(string name)
        {
            Id = Guid.NewGuid();
            Name = name;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
