namespace stream_deck_client.Utility
{
    internal static class CommandParser
    {
        public static bool TryParse(string? input, out string key, out string value)
        {
            key = string.Empty;
            value = string.Empty;

            if (string.IsNullOrEmpty(input))
            {
                return false;
            }

            var parts = input.Split(':');

            if (parts.Length != 2)
            {
                return false;
            }

            key = parts[0].Trim();
            value = parts[1].Trim();
            return true;
        }
    }
}