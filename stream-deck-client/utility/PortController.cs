using System.IO.Ports;
using DotNetEnv;

namespace stream_deck_client.Utility
{
    public class PortController
    {
        private static SerialPort? port;

        public void InitPort()
        {
            string songName = "Stan";

            port = new(Env.GetString("PORT_NAME"), Env.GetInt("PORT_BOUD_RATE"));
            port.Open();

            Console.WriteLine("Sending data...");
            port.WriteLine(songName);
            port.WriteLine("4:15");
            Console.WriteLine("Data sent. Waiting for response...");
        }

        public void ClosePort()
        {
            port.Close();
        }

        private void WriteToPort(string data)
        {
            port.WriteLine(data);
        }

        public void WriteSong(string name, string duration)
        {
            WriteToPort(name);
            WriteToPort(duration);
        }
    }
}