using Microsoft.AspNetCore.SignalR.Client;
namespace StockQuoteChat.Bot
{
    public class Sender
    {
        public static async Task Main()
        {
            HubConnection hubConnection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7173/chat")
                .Build();

            try
            {
                await hubConnection.StartAsync();
                Console.WriteLine("Connection started");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }

            await hubConnection.InvokeAsync("AddUserToRoom", new UserConnection("MyChat Bot", parsedBody!.Room));
            await hubConnection.InvokeAsync("SendMessage", parsedBody.Message);


            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }

}
}