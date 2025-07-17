using stream_deck_client.Service;

namespace stream_deck_client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            AppCommunicator appCommunicator = await AppCommunicator.Create();
            await appCommunicator.ListenForCommands();
        }
    }
}