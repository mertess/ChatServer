using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatTCPServer
{
    class Program
    {
        static string ip = "127.0.0.1";
        static int port = 8668;
        static void Main(string[] args)
        {
            Server server = new Server(ip, port);
            server.Listening();
        }
    }
}
