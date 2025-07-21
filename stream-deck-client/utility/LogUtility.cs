using Serilog;
using DotNetEnv;

namespace stream_deck_client.Utility
{
    public class LogUtility
    {
        public LogUtility()
        {
            string tempPath = Path.Combine(Path.GetTempPath(), Env.GetString("LOG_FILE"));
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(tempPath)
                .CreateLogger();
        }

        public void WriteLog(string message)
        {
            Log.Information(message);
        }
    }
}