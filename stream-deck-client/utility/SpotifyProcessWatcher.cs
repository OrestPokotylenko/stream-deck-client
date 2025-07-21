using System.Diagnostics;

namespace stream_deck_client.Utility
{
    internal static class SpotifyProcessWatcher
    {
        public static bool IsSpotifyRunning()
        {
            return Process.GetProcessesByName("Spotify").Length != 0;
        }

        public static async Task WaitForSpotifyAsync(int pollIntervalMs = 2000)
        {
            while (!IsSpotifyRunning())
            {
                await Task.Delay(pollIntervalMs);
            }
        }
    }
}