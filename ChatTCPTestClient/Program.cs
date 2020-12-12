using ChatTCPServer;
using ChatTCPServer.Services;
using ServerBusinessLogic.Enums.Transmission;
using ServerBusinessLogic.ReceiveModels.MessageModels;
using ServerBusinessLogic.ReceiveModels.UserModels;
using ServerBusinessLogic.ResponseModels.ChatModels;
using ServerBusinessLogic.ResponseModels.MessageModels;
using ServerBusinessLogic.ResponseModels.NotificationModels;
using ServerBusinessLogic.ResponseModels.UserModels;
using ServerBusinessLogic.TransmissionModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Management.Instrumentation;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatTCPTestClient
{
    class Program
    {
        static string serverIp = "127.0.0.1";
        static int serverPort = 8668;
        static TcpClient tcpClient;
        static NetworkStream networkStream;
        static UserResponseModel user;
        static Serializer serializer = new Serializer();
        static DataManager DataManager = new DataManager();
        static void Main(string[] args)
        {
            try
            {
                Console.OutputEncoding = Encoding.Unicode;
                Console.InputEncoding = Encoding.Unicode;
                tcpClient = new TcpClient();
                tcpClient.Connect(IPAddress.Parse(serverIp), serverPort);
                networkStream = tcpClient.GetStream();

                DataManager.AddListener(ListenerType.AuthorizationListener, AuthorizationListener);
                DataManager.AddListener(ListenerType.RegistrationListener, RegistrationListener);
                DataManager.AddListener(ListenerType.ChatsMessagesListener, MessageListListener);
                DataManager.AddListener(ListenerType.ChatsMessagesDeleteListener, MessageDeleteListListener);
                DataManager.AddListener(ListenerType.ChatListListener, ChatListListener);
                DataManager.AddListener(ListenerType.ChatListDeleteListener, ChatListDeleteListener);
                DataManager.AddListener(ListenerType.UserListListener, UserListListener);
                DataManager.AddListener(ListenerType.FriendListListener, FriendsListListener);
                DataManager.AddListener(ListenerType.FriendListDeleteListener, FriendsDeleteListListener);
                DataManager.AddListener(ListenerType.NotificationListListener, NotificationListListener);


                Task.Run(() => RecieveMessages());

                Authorization("mertess3", "123");

                while (true)
                {
                    var message = Console.ReadLine();
                    SendMessageToChat(message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Disconnect();
            }
        }

        static void Registration(string login, string password)
        {
            SendMessage(new ClientOperationMessage()
            {
                Operation = ClientOperations.Registration,
                JsonData = serializer.Serialize(new UserReceiveModel()
                {
                    Login = login,
                    Password = password,
                    UserName = login,
                    Name = login,
                    SecondName = login
                })
            });
        }

        static void SendMessageToChat(string message)
        {
            SendMessage(new ClientOperationMessage()
            {
                Operation = ClientOperations.SendMessage,
                JsonData = serializer.Serialize(new MessageReceiveModel()
                {
                    Date = DateTime.Now,
                    FromUserId = user.Id,
                    UserMassage = message,
                    ChatId = 1
                })
            });
        }

        static void Authorization(string login, string password)
        {
            SendMessage(new ClientOperationMessage()
            {
                Operation = ClientOperations.Authorization,
                JsonData = serializer.Serialize(new UserReceiveModel()
                {
                    Login = login,
                    Password = password
                })
            });
        }

        static void SendMessage(ClientOperationMessage clientOperationMessage)
        {
            byte[] data = Encoding.UTF8.GetBytes(serializer.Serialize(clientOperationMessage));
            Console.WriteLine("data send length = " + data.Length);
            networkStream.Write(data, 0, data.Length);
        }

        static void RecieveMessages()
        {
            byte[] data = new byte[256];
            while (true)
            {
                StringBuilder stringBuilder = new StringBuilder();
                do
                {
                    networkStream.Read(data, 0, 256);
                    stringBuilder.Append(Encoding.UTF8.GetString(data, 0, 256));
                } while (networkStream.DataAvailable);

                data = new byte[256];

                try
                {
                    var obj = serializer.Deserialize<OperationResultInfo>(stringBuilder.ToString());
                    DataManager.HandleData(obj.ToListener, obj);
                }
                catch (Exception) { }
            }
        }

        static void AuthorizationListener(OperationResultInfo operationResultInfo)
        {
            Console.WriteLine("Operation result = " + Enum.GetName(typeof(OperationsResults), operationResultInfo.OperationResult));
            Console.WriteLine("Error info = " + operationResultInfo.ErrorInfo);
            Console.WriteLine("Listener = " + Enum.GetName(typeof(ListenerType), operationResultInfo.ToListener));
            user = serializer.Deserialize<UserResponseModel>(operationResultInfo.JsonData as string);
            Console.WriteLine("UserId = " + user?.Id);
            Console.WriteLine("UserName = " + user?.UserName);
        }

        static void RegistrationListener(OperationResultInfo operationResultInfo)
        {
            Console.WriteLine("Reg Operation result = " + Enum.GetName(typeof(OperationsResults), operationResultInfo.OperationResult));
            Console.WriteLine("Error info = " + operationResultInfo.ErrorInfo);
            Console.WriteLine("Listener = " + Enum.GetName(typeof(ListenerType), operationResultInfo.ToListener));
        }

        static void MessageListListener(OperationResultInfo operationResultInfo)
        {
            Console.WriteLine("Operation result = " + Enum.GetName(typeof(OperationsResults), operationResultInfo.OperationResult));
            Console.WriteLine("Error info = " + operationResultInfo.ErrorInfo);
            Console.WriteLine("Listener = " + Enum.GetName(typeof(ListenerType), operationResultInfo.ToListener));
            var data = serializer.Deserialize<MessageResponseModel>(operationResultInfo.JsonData as string);
            Console.WriteLine("MessageId = " + data.Id);
            Console.WriteLine("UserId = " + data.UserId);
            Console.WriteLine("ChatId = " + data.ChatId);
            Console.WriteLine("Message = " + data.UserMassage);
        }

        static void ChatListListener(OperationResultInfo operationResultInfo)
        {
            Console.WriteLine("Operation result = " + Enum.GetName(typeof(OperationsResults), operationResultInfo.OperationResult));
            Console.WriteLine("Error info = " + operationResultInfo.ErrorInfo);
            Console.WriteLine("Listener = " + Enum.GetName(typeof(ListenerType), operationResultInfo.ToListener));
            var data = serializer.Deserialize<ChatResponseModel>(operationResultInfo.JsonData as string);
            Console.WriteLine("ChatId" + data.Id);
            Console.WriteLine("ChatName" + data.ChatName);
            Console.WriteLine("CreatorId" + data.CreatorId);
            Console.WriteLine("CountUsers" + data.CountUsers);
            foreach (var user in data.ChatUsers)
                Console.WriteLine(user.Id + " " + user.UserName + " " + user.IsOnline);
        }

        static void ChatListDeleteListener(OperationResultInfo operationResultInfo)
        {
            Console.WriteLine("Operation result = " + Enum.GetName(typeof(OperationsResults), operationResultInfo.OperationResult));
            Console.WriteLine("Error info = " + operationResultInfo.ErrorInfo);
            Console.WriteLine("Listener = " + Enum.GetName(typeof(ListenerType), operationResultInfo.ToListener));
            var data = serializer.Deserialize<ChatResponseModel>(operationResultInfo.JsonData as string);
            Console.WriteLine("ChatId" + data.Id);
            Console.WriteLine("ChatName" + data.ChatName);
            Console.WriteLine("CreatorId" + data.CreatorId);
            Console.WriteLine("CountUsers" + data.CountUsers);
            foreach (var user in data.ChatUsers)
                Console.WriteLine(user.Id + " " + user.UserName + " " + user.IsOnline);
        }

        static void MessageDeleteListListener(OperationResultInfo operationResultInfo)
        {
            Console.WriteLine("Operation result = " + Enum.GetName(typeof(OperationsResults), operationResultInfo.OperationResult));
            Console.WriteLine("Error info = " + operationResultInfo.ErrorInfo);
            Console.WriteLine("Listener = " + Enum.GetName(typeof(ListenerType), operationResultInfo.ToListener));
            var data = serializer.Deserialize<MessageResponseModel>(operationResultInfo.JsonData as string);
            Console.WriteLine("MessageId = " + data.Id);
            Console.WriteLine("UserId = " + data.UserId);
            Console.WriteLine("ChatId = " + data.ChatId);
            Console.WriteLine("Message = " + data.UserMassage);
        }

        static void UserListListener(OperationResultInfo operationResultInfo)
        {
            Console.WriteLine("Operation result = " + Enum.GetName(typeof(OperationsResults), operationResultInfo.OperationResult));
            Console.WriteLine("Error info = " + operationResultInfo.ErrorInfo);
            Console.WriteLine("Listener = " + Enum.GetName(typeof(ListenerType), operationResultInfo.ToListener));
            var data = serializer.Deserialize<List<UserListResponseModel>>(operationResultInfo.JsonData as string);
            foreach(var user in data)
            {
                Console.WriteLine("UserId = " + user.Id);
                Console.WriteLine("Username = " + user.UserName);
            }
        }

        static void FriendsListListener(OperationResultInfo operationResultInfo)
        {
            Console.WriteLine("Operation result = " + Enum.GetName(typeof(OperationsResults), operationResultInfo.OperationResult));
            Console.WriteLine("Error info = " + operationResultInfo.ErrorInfo);
            Console.WriteLine("Listener = " + Enum.GetName(typeof(ListenerType), operationResultInfo.ToListener));
            var data = serializer.Deserialize<List<UserListResponseModel>>(operationResultInfo.JsonData as string);
            foreach (var user in data)
            {
                Console.WriteLine("UserId = " + user.Id);
                Console.WriteLine("Username = " + user.UserName);
            }
        }

        static void FriendsDeleteListListener(OperationResultInfo operationResultInfo)
        {
            Console.WriteLine("Operation result = " + Enum.GetName(typeof(OperationsResults), operationResultInfo.OperationResult));
            Console.WriteLine("Error info = " + operationResultInfo.ErrorInfo);
            Console.WriteLine("Listener = " + Enum.GetName(typeof(ListenerType), operationResultInfo.ToListener));
            var data = serializer.Deserialize<List<UserListResponseModel>>(operationResultInfo.JsonData as string);
            foreach (var user in data)
            {
                Console.WriteLine("UserId = " + user.Id);
                Console.WriteLine("Username = " + user.UserName);
            }
        }

        static void NotificationListListener(OperationResultInfo operationResultInfo)
        {
            Console.WriteLine("Operation result = " + Enum.GetName(typeof(OperationsResults), operationResultInfo.OperationResult));
            Console.WriteLine("Error info = " + operationResultInfo.ErrorInfo);
            Console.WriteLine("Listener = " + Enum.GetName(typeof(ListenerType), operationResultInfo.ToListener));
            var data = serializer.Deserialize<NotificationResponseModel>(operationResultInfo.JsonData as string);
            Console.WriteLine("NotificationId " + data.Id);
            Console.WriteLine("FromUser " + data.FromUserName);
            Console.WriteLine("NotificationMessage " + data.Message);
        }

        static void Disconnect()
        {
            if (networkStream != null)
                networkStream.Close();
            tcpClient.Close();
        }
    }
}
