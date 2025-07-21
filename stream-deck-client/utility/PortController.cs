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

        private bool TryOpenPort()
        {
            _port ??= new(Env.GetString("PORT_NAME"), Env.GetInt("PORT_BOUD_RATE"));

            if (!_port.IsOpen)
            {
                try
                {
                    _port.Open();
                    return true;
                }
                catch (Exception ex)
                {
                    _log.WriteLog($"Failed to open port: {ex.Message}");
                    return false;
                }
            }

            return true;
        }

        public void ClosePort()
        {
            if (_port is not null && _port.IsOpen)
            {
                _port?.Close();
            }
        }

        private bool WriteToPort(string data)
        {
            if (TryOpenPort())
            {
                _port?.WriteLine(data);
                return true;
            }

            return false;
        }

        public bool WriteSong(string name, string duration)
        {
            bool nameWritten = WriteToPort(name);
            bool durationWritten = WriteToPort(duration);

            return nameWritten && durationWritten;
        }

        public string? ReadLine()
        {
            if (TryOpenPort())
            {
                return _port?.ReadLine();
            }

            return null;
        }
    }
}