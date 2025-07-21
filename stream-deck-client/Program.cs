using stream_deck_client.Service;
using stream_deck_client.Utility;

namespace stream_deck_client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            LogUtility logUtility = new();
            AppCommunicator appCommunicator = await AppCommunicator.Create(logUtility);
            await appCommunicator.ListenForCommands();
        }
    }
}