using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServerDatabaseSystem.Implementation;
using ServerBusinessLogic.TransmissionModels;
using ServerDatabaseSystem.DbModels;
using System.Runtime.CompilerServices;
using ServerBusinessLogic.BusinessLogic;
using ServerBusinessLogic.ReceiveModels;
using ServerBusinessLogic.ReceiveModels.UserModels;

namespace ChatTCPServer
{
    public class Server
    {
        private readonly TcpListener tcpListener_;
        public List<Client> ConnectedClients { get; private set; }
        
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

        private void DisconnectServer()
        {
            foreach (var client in ConnectedClients)
            {
                //client.SendMessage("Server stopped");
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
