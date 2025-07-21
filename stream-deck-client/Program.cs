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

            while (true)
            {
                await SpotifyProcessWatcher.WaitForSpotifyAsync();

                using var cts = new CancellationTokenSource();
                var mainTask = Task.Run(() => appCommunicator.ListenForCommands(cts.Token), cts.Token);

                while (SpotifyProcessWatcher.IsSpotifyRunning())
                {
                    await Task.Delay(2000);
                }

                cts.Cancel();
            }
        }
    }
}