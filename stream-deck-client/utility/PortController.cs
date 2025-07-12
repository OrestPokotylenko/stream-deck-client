using System;
using System.IO.Ports;

namespace stream_deck_client.utility
{
    public class PortController
    {
        private static SerialPort port;

        public void InitPort()
        {
            string songName = "Stan";

            port = new("COM3", 115200);
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