using System.IO.Ports;
using DotNetEnv;

namespace stream_deck_client.Utility
{
    public class PortController
    {
        private static SerialPort? _port;

        public void InitPort()
        {
            _port = new(Env.GetString("PORT_NAME"), Env.GetInt("PORT_BOUD_RATE"));
            _port.Open();
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