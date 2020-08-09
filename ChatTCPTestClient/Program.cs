using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatTCPTestClient
{
    class Program
    {
        static string serverIp = "127.0.0.1";
        static int serverPort = 8668;
        static TcpClient tcpClient;
        static NetworkStream networkStream;
        static string userName;
        static void Main(string[] args)
        {
            try
            {
                tcpClient = new TcpClient();
                tcpClient.Connect(IPAddress.Parse(serverIp), serverPort);
                networkStream = tcpClient.GetStream();

                Task.Run(() => RecieveMessages());

                Console.Write("Enter your nickname:");
                userName = Console.ReadLine();
                SendMessage(userName);
                Console.WriteLine("Enter the messages:");
                while (true)
                {
                    var message = Console.ReadLine();
                    SendMessage(message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Disconnect();
            }
        }

        static void SendMessage(string message)
        {
            var finalMessage = userName + ": " + message;
            byte[] data = Encoding.UTF8.GetBytes(finalMessage);
            networkStream.Write(data, 0, data.Length);
        }

        static void RecieveMessages()
        {
            byte[] data = new byte[64];
            while (true)
            {
                StringBuilder stringBuilder = new StringBuilder();
                do
                {
                    networkStream.Read(data, 0, 64);
                    stringBuilder.Append(Encoding.UTF8.GetString(data, 0, 64));
                } while (networkStream.DataAvailable);

                Console.WriteLine(stringBuilder.ToString());
            }
        }

        static void Disconnect()
        {
            if (networkStream != null)
                networkStream.Close();
            tcpClient.Close();
        }
    }
}
