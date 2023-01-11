using StockQuoteChat.Bot.Models.Responses;

namespace StockQuoteChat.Bot.Models
{
    public class User
    {
        public User()
        {
            Id = Guid.NewGuid();
        }

        public User(LoginResponseDto loginResponseDto)
        {
            Id = loginResponseDto.Id;
            FirstName = loginResponseDto.FirstName;
            LastName = loginResponseDto.LastName;
            Email = loginResponseDto.Email;
        }

        public string GetUserName()
        {
            return $"{FirstName} {LastName}";
        }

        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}
