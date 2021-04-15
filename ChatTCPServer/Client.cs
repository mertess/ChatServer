using ChatTCPServer.Services;
using NLog;
using ServerBusinessLogic.BusinessLogic;
using ServerDatabaseSystem.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.ServiceModel.Configuration;
using System.Text;

namespace ChatTCPServer
{
    public class Client
    {
        private readonly TcpClient _tcpClient;

        private readonly Server _server;

        private readonly NetworkStream _networkStream;

        private readonly RequestHandler _requestHandler;

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public int Id { set; get; }

        private bool _isDisconnected;

        public Client(TcpClient tcpClient, Server server)
        {
            //wtf
            _requestHandler = new RequestHandler(
                new JsonStringSerializer(),
                this,
                server.ConnectedClients);

            _tcpClient = tcpClient;
            _server = server;
            _networkStream = tcpClient.GetStream();
            _server.AddConnection(this);
            Console.WriteLine("client connected");
        }

        //обработка подключения пользователя
        public void Process()
        {
            try
            {
                while (true)
                {
                    _requestHandler.HandleRequest(GetMessage());

                    if (_isDisconnected)
                        return;
                }
            }
            catch (Exception e)
            {
                _logger.Warn(e.Message);
                Console.WriteLine(Id + " " + e.Message);
            }
            finally
            {
                _logger.Info($"user {Id} has disconnected");
                _requestHandler.HandleDisconnect();
                Disconnect();
            }
        }

        private byte[] GetMessage()
        {
            byte[] data = new byte[256];
            List<byte> byteMessage = new List<byte>();
            do
            {
                var count = _networkStream.Read(data, 0, 256);

                for(int i = 0; i < count; i++)
                {
                    byteMessage.Add(data[i]);
                }
            } while (_networkStream.DataAvailable);

            return byteMessage.ToArray();
        }

        public void SendMessage(byte[] message)
        {
            try
            {
                _networkStream.Write(message, 0, message.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine(Id + ": " + ex.Message);
            }
        }

        public void Disconnect()
        {
            if (_tcpClient != null)
                _tcpClient.Close();
            if (_networkStream != null)
                _networkStream.Close();
            _server.RemoveConnection(this);

            _isDisconnected = true;
        }
    }
}
