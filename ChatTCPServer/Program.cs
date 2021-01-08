using NLog;
using System;
using System.IO;
using System.Text;

namespace ChatTCPServer
{
    class Program
    {
        static string ip = "127.0.0.1";
        static int port = 8667;
        static Logger logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.Unicode;
            CreateLogsDir();
            Server server = new Server(ip, port);
            logger.Info("Server has started");
            server.Listening();
        }

        static void CreateLogsDir()
        {
            if (!Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs")))
                Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs"));
        }
    }
}
