namespace stream_deck_client
{
    using utility;

    internal class Program
    {
        static void Main(string[] args)
        {
            PortController portController = new();
            portController.InitPort();
            portController.WriteSong("Test", "3:29");
            portController.ClosePort();
        }
    }
}
