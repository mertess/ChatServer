using ChatTCPServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ChatTCPServer
{
    public class Client
    {
        private TcpClient tcpClient_;
        private Server server_;
        private NetworkStream networkStream_;
        private string userName_;
        public string Id { private set; get; }

        public Client(TcpClient tcpClient, Server server)
        {
            tcpClient_ = tcpClient;
            server_ = server;
            networkStream_ = tcpClient.GetStream();
            Id = Guid.NewGuid().ToString();
            server_.AddConnection(this);
            Console.WriteLine("new Client = " + Id);
        }

        //обработка подключения пользователя
        public void Process()
        {
            try
            {
                var data = GetMessage().Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                if (server_.ClientAuthorization(data[0], data[1]))
                {
                    userName_ = data[2];
                    server_.BroadCastSend(userName_ + " online", Id);
                    while (true)
                    {
                        server_.BroadCastSend(GetMessage(), Id);
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(userName_ + " " + e.Message);
            }
            finally
            {
                Disconnect();
            }
        }

        private string GetMessage()
        {
            byte[] data = new byte[64];
            StringBuilder stringBuilder = new StringBuilder();
            do
            {
                networkStream_.Read(data, 0, 64);
                stringBuilder.Append(Encoding.UTF8.GetString(data, 0, 64));
            } while (networkStream_.DataAvailable);

            return stringBuilder.ToString().Trim(' ');
        }

        public void SendMessage(string message)
        {
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(message);
                networkStream_.Write(data, 0, data.Length);
            }
            catch(Exception ex)
            {
                Console.WriteLine(userName_ + ": " + ex.Message);
            }
        }

        public void Disconnect()
        {
            if (tcpClient_ != null)
                tcpClient_.Close();
            if (networkStream_ != null)
                networkStream_.Close();

            server_.BroadCastSend(userName_ + " disconnected", Id);
            server_.RemoveConnection(this);
        }
    }
}
