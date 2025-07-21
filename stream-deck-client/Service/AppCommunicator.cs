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
        private const string EnvPath = "../../../.env";
        private const string VolumeCommand = "VOLUME";
        private const string StandardCommand = "COMMAND";

        private AppCommunicator(PortController portController, SpotifyController spotifyController)
        {
            _portController = portController;
            _spotifyController = spotifyController;
        }

        public static async Task<AppCommunicator> Create()
        {
            Env.Load(EnvPath);

            var portController = new PortController();
            portController.InitPort();

            var spotifyController = new SpotifyController(await SpotifyAuthHelper.Auth());

            return new AppCommunicator(portController, spotifyController);
        }

        private Task ListenForArduinoCommands()
        {
            return Task.Run(async () =>
            {
                while (true)
                {
                    string? input = _portController.ReadLine();

                    if (!CommandParser.TryParse(input, out string key, out string value))
                    {
                        Console.WriteLine($"Invalid input: {input}");
                        continue;
                    }

                    try
                    {
                        if (key == VolumeCommand && int.TryParse(value, out int volume))
                        {
                            await _spotifyController.ChangeVolumeAsync(volume);
                            continue;
                        }

                        if (key == StandardCommand && Enum.TryParse<CommandType>(value, true, out var commandType))
                        {
                            await _spotifyController.GetCommandAsync(commandType);
                            continue;
                        }

                        Console.WriteLine($"Unknown command key or value: {input}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing command: {ex.Message}");
                    }
                }
            });
        }

        public async Task PollSongUpdates()
        {
            while (true)
            {
                Song? song = await _spotifyController.GetCurrentSongAsync();

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