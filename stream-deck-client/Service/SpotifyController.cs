using SpotifyAPI.Web;
using stream_deck_client.DTO;
using stream_deck_client.Enums;

namespace stream_deck_client.Service
{
    internal class SpotifyController
    {
        private readonly SpotifyClient _client;
        private readonly Dictionary<CommandType, Func<Task>> _commands;

        public SpotifyController(SpotifyClient client)
        {
            _client = client;
            _commands = new Dictionary<CommandType, Func<Task>>
            {
                { CommandType.PREVIOUS_TRACK, async () => await PreviousSongAsync() },
                { CommandType.NEXT_TRACK, async () => await NextSongAsync() },
                { CommandType.PLAY_PAUSE, async () => await PlayPauseAsync() }
            };
        }

        public async Task<Song?> GetCurrentSongAsync()
        {
            CurrentlyPlayingContext playback = await _client.Player.GetCurrentPlayback();

            if (playback?.Item is FullTrack track)
            {
                return new Song(track.Name, TimeSpan.FromMilliseconds(track.DurationMs).ToString(@"mm\:ss"));
            }

            return null;
        }

        private async Task NextSongAsync()
        {
            await _client.Player.SkipNext();
        }

        private async Task PreviousSongAsync()
        {
            await _client.Player.SkipPrevious();
        }

        private async Task PlayPauseAsync()
        {
            CurrentlyPlayingContext? playback = await _client.Player.GetCurrentPlayback();

            if (playback == null)
            {
                await _client.Player.ResumePlayback();
            }
            else if (playback.IsPlaying)
            {
                await _client.Player.PausePlayback();
            }
            else
            {
                await _client.Player.ResumePlayback();
            }
        }

        public async Task ChangeVolumeAsync(int volume)
        {
            await _client.Player.SetVolume(new PlayerVolumeRequest(volume));
        }

        public async Task GetCommandAsync(CommandType commandType)
        {
            await _commands[commandType]();
        }
    }
}