using SpotifyAPI.Web;
using stream_deck_client.DTO;
using stream_deck_client.Enums;
using System;

namespace stream_deck_client.Service
{
    public class SpotifyController
    {
        private readonly SpotifyClient _client;
        private readonly Dictionary<CommandType, Func<Task>> _commands;

        public SpotifyController(SpotifyClient client)
        {
            _client = client;
            _commands = new Dictionary<CommandType, Func<Task>>
            {
                { CommandType.PREVIOUS_TRACK, async () => await PreviousSong() },
                { CommandType.NEXT_TRACK, async () => await NextSong() },
                { CommandType.PLAY_PAUSE, async () => await PlayPause() }
            };
        }

        public async Task<Song?> GetCurrentSong()
        {
            CurrentlyPlayingContext playback = await _client.Player.GetCurrentPlayback();

            if (playback?.Item is FullTrack track)
            {
                return new Song(track.Name, TimeSpan.FromMilliseconds(track.DurationMs).ToString(@"mm\:ss"));
            }

            return null;
        }

        private async Task NextSong()
        {
            await _client.Player.SkipNext();
        }

        private async Task PreviousSong()
        {
            await _client.Player.SkipPrevious();
        }

        private async Task PlayPause()
        {
            CurrentlyPlayingContext? playback = await _client.Player.GetCurrentPlayback();

            if (playback == null)
            {
                // Try to resume playback anyway
                Console.WriteLine("No current playback info, trying to resume.");
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

        public async Task GetCommand(CommandType commandType)
        {
            await _commands[commandType]();
        }
    }
}