﻿using ChatTCPServer.Enums;
using ChatTCPServer.Models;
using ChatTCPServer.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
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
                while(true)
                {
                    ProcessClientOperation(GetMessage());
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

        private void ProcessClientOperation(ClientOperationMessage clientOperationMessage)
        {
            string[] data = clientOperationMessage.Data.ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            switch(clientOperationMessage.Operation)
            {
                case ClientOperations.Authorization:
                    SendMessage(server_.ClientAuthorization(data[0], data[1]));
                    break;
                case ClientOperations.Registration:
                    SendMessage(server_.ClientRegistration(data[0], data[1]));
                    break;
            }
        }

        private ClientOperationMessage GetMessage()
        {
            byte[] data = new byte[64];
            StringBuilder stringBuilder = new StringBuilder();
            do
            {
                networkStream_.Read(data, 0, 64);
                stringBuilder.Append(Encoding.UTF8.GetString(data, 0, 64));
            } while (networkStream_.DataAvailable);
            Console.WriteLine("Получено сообщение - " + Id + " " + stringBuilder.ToString());
            var clientOperation = stringBuilder.ToString().Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            return new ClientOperationMessage()
            {
                Operation = (ClientOperations)Enum.Parse(typeof(ClientOperations), clientOperation[0]),
                Data = clientOperation[1]
            };
        }

        public void SendMessage(OperationResultInfo operationResultInfo)
        {
            try
            {
                Console.WriteLine("Отправлено сообщение - " + Id + " " + operationResultInfo.Info);
                byte[] data = Encoding.UTF8.GetBytes(operationResultInfo.ToString());
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
