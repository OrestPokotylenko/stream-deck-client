namespace stream_deck_client.Utility
{
    public static class FileUtility
    {
        public static void WriteFile(string fileName, string input)
        {
            File.WriteAllText(fileName, input);
        }

        public static string ReadFile(string fileName) 
        {
            if (File.Exists(fileName))
            {
                return File.ReadAllText(fileName);
            }

            return "";
        }
    }
}