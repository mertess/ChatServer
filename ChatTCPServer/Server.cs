using NLog;
using NLog.Fluent;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ChatTCPServer
{
    public class Server
    {
        private readonly TcpListener tcpListener_;
        public List<Client> ConnectedClients { get; private set; }

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public Server(string Address, int Port)
        {
            tcpListener_ = new TcpListener(IPAddress.Parse(Address), Port);
            ConnectedClients = new List<Client>();
        }

        public void Listening()
        {
            try
            {
                Console.WriteLine("Server started....");
                tcpListener_.Start();
                while (true)
                {
                    var tcpClient = tcpListener_.AcceptTcpClient();
                    _logger.Info($"Getting connection by IP:{IPAddress.Parse(((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address.ToString())}");
                    Client client = new Client(tcpClient, this);
                    Thread thread = new Thread(() => client.Process());
                    thread.Start();
                }
            }
            catch (Exception ex)
            {
                _logger.Warn(ex.Message);
                Console.WriteLine(ex.Message);
            }
            finally
            {
                _logger.Info("Server has stopped");
                DisconnectServer();
            }
        }

        private void DisconnectServer()
        {
            foreach (var client in ConnectedClients)
            {
                client.Disconnect();
            }
            Console.WriteLine("Server shutdown...");
            tcpListener_.Stop();
        }

        #region ClientInterface

        public void AddConnection(Client client) => ConnectedClients.Add(client);

        public void RemoveConnection(Client client) => ConnectedClients.Remove(client);

        #endregion
    }
}
