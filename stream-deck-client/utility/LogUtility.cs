using Serilog;

namespace stream_deck_client.Utility
{
    public class LogUtility
    {
        public LogUtility()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("log.txt")
                .CreateLogger();
        }

        public void WriteLog(string message)
        {
            Log.Information(message);
        }
    }
}