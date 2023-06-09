﻿using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SyslogReceive
{
    internal class Program
    {
        static void Log(string dir, IPEndPoint remoteEP, string message)
        {
            string address = remoteEP.Address.ToString();
            string filename = Path.Join(dir, address + ".txt");
            using (FileStream fs = File.Open(filename, FileMode.Append, FileAccess.Write, FileShare.Read))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    var localDate = DateTime.Now;
                    var timestamp = localDate.ToString("yyyy/MM/dd HH:mm:ss.fff UTCzzz");
                    string logMessage = $"[{timestamp}][{address}] : {message}";
                    Console.WriteLine(logMessage);
                    sw.WriteLine(logMessage);
                }
            }
        }

        static void Main(string[] args)
        {
            var localDate = DateTime.Now;
            var timestamp = localDate.ToString("yyyyMMdd-HHmmss.fff");
            var dir = $"log_{timestamp}";
            Directory.CreateDirectory(dir);

            var localPort = 514;
            var localEP = new IPEndPoint(0, localPort);
            using (UdpClient udp = new UdpClient(localEP))
            {
                while (true)
                {
                    IPEndPoint? remoteEP = null;
                    byte[] data = udp.Receive(ref remoteEP);
                    string message = System.Text.Encoding.UTF8.GetString(data);

                    if (remoteEP != null)
                    {
                        Log(dir, remoteEP, message);
                    }
                }
            }
        }
    }
}
