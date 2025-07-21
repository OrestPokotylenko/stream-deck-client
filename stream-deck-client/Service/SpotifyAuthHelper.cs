using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using stream_deck_client.Utility;
using System.Diagnostics;
using DotNetEnv;

namespace stream_deck_client.Service
{
    internal class SpotifyAuthHelper
    {
        private readonly LogUtility _log;

        public SpotifyAuthHelper(LogUtility log)
        {
            _log = log;
        }

        private async Task<(SpotifyClient, string)> AuthorizeWithPKCE()
        {
            // Create PKCE verifier & challenge
            var (verifier, challenge) = PKCEUtil.GenerateCodes(120);

            var server = new EmbedIOAuthServer(new Uri(Env.GetString("CALLBACK_URL")), Env.GetInt("CALLBACK_PORT"));
            await server.Start();

            // Prepare a task that completes when we get the auth code
            var tcs = new TaskCompletionSource<(SpotifyClient, string)>();

            // Handle the callback when Spotify redirects with code
            server.AuthorizationCodeReceived += async (sender, response) =>
            {
                await server.Stop();
                var token = await new OAuthClient().RequestToken(
                    new PKCETokenRequest(Env.GetString("SPOTIFY_CLIENT_ID"), response.Code, server.BaseUri, verifier)
                );
                var spotifyClient = new SpotifyClient(token.AccessToken);
                tcs.SetResult((spotifyClient, token.RefreshToken));
            };

            // Build authorization URL with PKCE
            var request = new LoginRequest(server.BaseUri, Env.GetString("SPOTIFY_CLIENT_ID"), LoginRequest.ResponseType.Code)
            {
                CodeChallengeMethod = "S256",
                CodeChallenge = challenge,
                Scope = [Scopes.UserReadPlaybackState, Scopes.UserModifyPlaybackState]
            };

            try
            {
                System.Diagnostics.Process.Start(new ProcessStartInfo
                {
                    FileName = request.ToUri().ToString(),
                    UseShellExecute = true
                });
            }
            catch (Exception)
            {
                _log.WriteLog($"Couldn't open browser automatically. Please open this URL manually: {request.ToUri()}");
            }

            // Wait until the callback handler completes
            return await tcs.Task;
        }

        private async Task<SpotifyClient> AuthWithRefreshToken(string refreshToken)
        {
            var newToken = await new OAuthClient().RequestToken(
                new PKCETokenRefreshRequest(Env.GetString("SPOTIFY_CLIENT_ID"), refreshToken));

            return new SpotifyClient(newToken.AccessToken);
        }

        public async Task<SpotifyClient> Auth()
        {
            string refreshToken = FileUtility.ReadFile("token_storage.txt");

            if (!string.IsNullOrEmpty(refreshToken))
            {
                try
                {
                    var newToken = await new OAuthClient().RequestToken(
                        new PKCETokenRefreshRequest(Env.GetString("SPOTIFY_CLIENT_ID"), refreshToken)
                    );

                    // Spotify may rotate the refresh token - always save it if provided
                    if (!string.IsNullOrEmpty(newToken.RefreshToken))
                    {
                        FileUtility.WriteFile("token_storage.txt", newToken.RefreshToken);
                    }

                    return new SpotifyClient(newToken.AccessToken);
                }
                catch (APIException e) when (e.Message.Contains("invalid_grant"))
                {
                    _log.WriteLog("Refresh token invalid or expired, falling back to full browser login...");
                }
                catch (APIException e)
                {
                    _log.WriteLog($"Spotify API error during refresh: {e.Message}, falling back to full browser login...");
                }
            }

            (var spotifyClient, var newRefreshToken) = await AuthorizeWithPKCE();
            FileUtility.WriteFile(Env.GetString("TOKEN_STORAGE_FILE"), newRefreshToken);

            return spotifyClient;
        }
    }
}
