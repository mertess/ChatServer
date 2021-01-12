using NLog;
using System;
using System.IO;
using System.Text;

namespace ChatTCPServer
{
    class Program
    {
        static string ip = "25.68.135.116";
        static int port = 8668;
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
