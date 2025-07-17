using DotNetEnv;
using stream_deck_client.DTO;
using stream_deck_client.Enums;
using stream_deck_client.Utility;

namespace stream_deck_client.Service
{
    public class AppCommunicator
    {
        private readonly PortController _portController;
        private readonly SpotifyController _spotifyController;
        private string _lastSongName = "";
        private const string ENV_PATH = "../../../.env";

        private AppCommunicator(PortController portController, SpotifyController spotifyController)
        {
            _portController = portController;
            _spotifyController = spotifyController;
        }

        public static async Task<AppCommunicator> Create()
        {
            Env.Load(ENV_PATH);

            var portController = new PortController();
            portController.InitPort();

            var spotifyController = new SpotifyController(await SpotifyAuthHelper.Auth());

            return new AppCommunicator(portController, spotifyController);
        }

        private Task ListenForArduinoCommands()
        {
            return Task.Run(() =>
            {
                while (true)
                {
                    string command = _portController.ReadLine();
                    Console.WriteLine($"Command: {command}");

                    if (string.IsNullOrEmpty(command))
                    {
                        continue;
                    }

                    if (Enum.TryParse<CommandType>(command, out var commandType))
                    {
                        _spotifyController.GetCommand(commandType).Wait();
                    }
                    else
                    {
                        Console.WriteLine($"Invalid command: {command}");
                    }
                }
            });
        }

        public async Task PollSongUpdates()
        {
            while (true)
            {
                Song? song = await _spotifyController.GetCurrentSong();

                if (song is not null && song._name != _lastSongName)
                {
                    _portController.WriteSong(song._name, song._duration);
                    Console.WriteLine(song._name);
                    _lastSongName = song._name;
                }

                await Task.Delay(3000);
            }
        }

        public async Task ListenForCommands()
        {
            var commandTask = ListenForArduinoCommands();
            var songTask = PollSongUpdates();

            await Task.WhenAll(songTask, commandTask);

            _portController.ClosePort();
        }
    }
}