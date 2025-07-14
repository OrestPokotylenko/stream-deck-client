using stream_deck_client.DTO;
using stream_deck_client.Enum;
using stream_deck_client.Service;
using stream_deck_client.Utility;
using DotNetEnv;

namespace stream_deck_client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Env.Load("../../../.env");
            //PortController portController = new();
            //portController.InitPort();

            var spotify = new SpotifyController(await SpotifyAuthHelper.Auth());

            Song? song = await spotify.GetCurrentSong();

            //if (song is not null)
            //{
            //    portController.WriteSong(song._name, song._duration);
            //}
            //else
            //{
            //    portController.WriteSong("Nothing", "00:00");
            //}

            //await TestApi(spotify);

            //portController.ClosePort();
        }

        static async Task TestApi(SpotifyController spotify)
        {
            await spotify.GetCommand(CommandType.PREVIOUS_TRACK);
        }
    }
}
