using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatTCPServer
{
    public class Server
    {
        private TcpListener tcpListener_;
        private List<Client> clients_;
        
        public Server(string Address, int Port)
        {
            tcpListener_ = new TcpListener(IPAddress.Parse(Address), Port);
            clients_ = new List<Client>();
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
                    Client client = new Client(tcpClient, this);
                    Thread thread = new Thread(() => client.Process());
                    thread.Start();
                }
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                DisconnectServer();
            }
        }

        public void BroadCastSend(string message, string id)
        {
            try
            {
                Console.WriteLine(message);
                foreach (var client in clients_)
                {
                    if(client.Id != id)
                        client.SendMessage(message);
                }
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void DisconnectServer()
        {
            foreach(var client in clients_)
            {
                client.SendMessage("Server stopped");
                client.Disconnect();
            }
            Console.WriteLine("Server shutdown...");
            tcpListener_.Stop();
        }

        public void AddConnection(Client client) => clients_.Add(client);
        public void RemoveConnection(Client client) => clients_.Remove(client);
    }
}
