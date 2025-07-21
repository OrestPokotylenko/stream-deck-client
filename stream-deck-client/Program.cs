using DotNetEnv;
using stream_deck_client.Service;
using stream_deck_client.Utility;

namespace stream_deck_client
{
    internal class Program
    {
        private const string EnvPath = "../../../.env";

        static async Task Main(string[] args)
        {
            Env.Load(EnvPath);
            LogUtility logUtility = new();
            AppCommunicator appCommunicator = await AppCommunicator.Create(logUtility);
            await appCommunicator.ListenForCommands();
        }
    }
}