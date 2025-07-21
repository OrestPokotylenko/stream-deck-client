using System.IO.Ports;
using DotNetEnv;

namespace stream_deck_client.Utility
{
    public class PortController
    {
        private static SerialPort? _port;
        private readonly LogUtility _log;

        public PortController(LogUtility logUtility)
        {
            _log = logUtility;
        }

        public void InitPort()
        {
            _port = new(Env.GetString("PORT_NAME"), Env.GetInt("PORT_BOUD_RATE"));

            try
            {
                _port.Open();
            }
            catch (Exception ex)
            {
                _log.WriteLog($"Failed to open port: {ex.Message}");
            }
        }

        public void ClosePort()
        {
            _port?.Close();
        }

        private void WriteToPort(string data)
        {
            _port?.WriteLine(data);
        }

        public void WriteSong(string name, string duration)
        {
            WriteToPort(name);
            WriteToPort(duration);
        }

        public string? ReadLine()
        {
            return _port?.ReadLine();
        }
    }
}