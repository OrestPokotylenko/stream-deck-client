using DotNetEnv;
using stream_deck_client.DTO;
using stream_deck_client.Enums;
using stream_deck_client.Utility;
using System.Threading;

namespace stream_deck_client.Service
{
    internal class AppCommunicator
    {
        private readonly PortController _portController;
        private readonly SpotifyController _spotifyController;
        private readonly LogUtility _log;

        private string _lastSongName = "";
        private bool lastSongWritten = false;

        private const string VolumeCommand = "VOLUME";
        private const string StandardCommand = "COMMAND";

        private AppCommunicator(PortController portController, SpotifyController spotifyController, LogUtility log)
        {
            _portController = portController;
            _spotifyController = spotifyController;
            _log = log;
        }

        public static async Task<AppCommunicator> Create(LogUtility logUtility)
        {
            PortController portController = new(logUtility);

            SpotifyAuthHelper spotifyAuthHelper = new(logUtility);
            var spotifyController = new SpotifyController(await spotifyAuthHelper.Auth());

            return new AppCommunicator(portController, spotifyController, logUtility);
        }

        private Task ListenForArduinoCommands(CancellationToken cancellationToken)
        {
            return Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    string? input = _portController.ReadLine();

                    if (!CommandParser.TryParse(input, out string key, out string value))
                    {
                        _log.WriteLog($"Invalid input: {input}");
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

                        _log.WriteLog($"Unknown command key or value: {input}");
                    }
                    catch (Exception ex)
                    {
                        _log.WriteLog($"Error processing command: {ex.Message}");
                    }
                }
            }, cancellationToken);
        }

        public async Task PollSongUpdates(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                Song? song = await _spotifyController.GetCurrentSongAsync();

                if (song is not null && (song._name != _lastSongName || !lastSongWritten))
                {
                    lastSongWritten = _portController.WriteSong(song._name, song._duration);
                    _lastSongName = song._name;
                }

                await Task.Delay(3000, cancellationToken);
            }
        }

        public async Task ListenForCommands(CancellationToken cancellationToken)
        {
            var commandTask = ListenForArduinoCommands(cancellationToken);
            var songTask = PollSongUpdates(cancellationToken);

            await Task.WhenAll(songTask, commandTask);

            _portController.ClosePort();
        }
    }
}