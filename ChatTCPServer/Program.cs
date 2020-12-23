using System;
using System.Text;

namespace ChatTCPServer
{
    class Program
    {
        static string ip = "127.0.0.1";
        static int port = 8668;
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.Unicode;
            Server server = new Server(ip, port);
            server.Listening();
        }
    }
}
