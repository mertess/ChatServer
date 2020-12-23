using ChatTCPServer.Services;
using ServerBusinessLogic.BusinessLogic;
using ServerDatabaseSystem.Implementation;
using System;
using System.Net.Sockets;
using System.Text;

namespace ChatTCPServer
{
    public class Client
    {
        private readonly TcpClient _tcpClient;
        private readonly Server _server;
        private readonly NetworkStream _networkStream;
        private readonly RequestHandler _requestHandler;
        public int Id { set; get; }

        public Client(TcpClient tcpClient, Server server)
        {
            //wtf
            _requestHandler = new RequestHandler(
                new Serializer(),
                this,
                new MainLogic(
                    new ChatLogic(),
                    new UserLogic(),
                    new MessageLogic(),
                    new FriendLogic(),
                    new NotificationLogic()
                ),
                server.ConnectedClients);

            _tcpClient = tcpClient;
            _server = server;
            _networkStream = tcpClient.GetStream();
            //Id = Guid.NewGuid().ToString();
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
                    ProcessClientOperation(GetMessage());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(Id + " " + e.Message);
            }
            finally
            {
                Disconnect();
            }
        }

        private void ProcessClientOperation(string messageJson) => _requestHandler.HandleRequest(messageJson);

        private string GetMessage()
        {
            byte[] data = new byte[256];
            StringBuilder stringBuilder = new StringBuilder();
            do
            {
                int countBytes = _networkStream.Read(data, 0, 256);
                stringBuilder.Append(Encoding.UTF8.GetString(data, 0, countBytes));
            } while (_networkStream.DataAvailable);
            Console.WriteLine("Получено сообщение - " + Id + " " + stringBuilder.ToString());

            return stringBuilder.ToString();
        }

        public void SendMessage(string message)
        {
            try
            {
                Console.WriteLine("Отправлено сообщение - " + Id + " " + message);
                byte[] data = Encoding.UTF8.GetBytes(message);
                _networkStream.Write(data, 0, data.Length);
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
        }
    }
}
