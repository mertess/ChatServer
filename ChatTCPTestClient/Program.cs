using ChatTCPServer;
using ChatTCPServer.Services;
using ServerBusinessLogic.Enums;
using ServerBusinessLogic.Enums.Transmission;
using ServerBusinessLogic.ReceiveModels.ChatModels;
using ServerBusinessLogic.ReceiveModels.FriendModels;
using ServerBusinessLogic.ReceiveModels.MessageModels;
using ServerBusinessLogic.ReceiveModels.NotificationModels;
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

        static List<ChatResponseModel> chats = new List<ChatResponseModel>();
        static List<UserListResponseModel> users = new List<UserListResponseModel>();
        static List<UserListResponseModel> friends = new List<UserListResponseModel>();
        //key = chatId, value - messages
        static Dictionary<int, List<MessageResponseModel>> messages = new Dictionary<int, List<MessageResponseModel>>();
        static List<NotificationResponseModel> notifications = new List<NotificationResponseModel>();

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

                
                while(true)
                {
                    var operation = Console.ReadLine();
                    switch (operation)
                    {
                        case "!get_users":
                            Console.WriteLine("parameters: page, searching username");
                            var parameters = Console.ReadLine();
                            GetUsers(new UserPaginationReceiveModel()
                            {
                                UserId = user.Id,
                                Page = Convert.ToInt32(parameters.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0]),
                                SearchingUserName = parameters.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Count() == 2 ? parameters.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1] : string.Empty
                            });
                            Console.WriteLine();
                            break;
                        case "!update_profile":
                            Console.WriteLine("parameters: username, login, password, name, secondname, gender, phonenumber, country, city");
                            var parameters2 = Console.ReadLine();
                            UpdateProfile(new UserReceiveModel()
                            {
                                Id = user.Id,
                                UserName = parameters2.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0],
                                Login = parameters2.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1],
                                Password = parameters2.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[2],
                                Name = parameters2.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[3],
                                SecondName = parameters2.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[4],
                                Gender = (Gender)Enum.GetValues(typeof(Gender)).GetValue(Convert.ToInt32(parameters2.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[5])),
                                PhoneNumber = parameters2.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[6],
                                Country = (Country)Enum.GetValues(typeof(Country)).GetValue(Convert.ToInt32(parameters2.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[7])),
                                City = (City)Enum.GetValues(typeof(City)).GetValue(Convert.ToInt32(parameters2.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[8]))
                            });
                            Console.WriteLine();
                            break;
                            //TODO : установка ChatId на сервере, а так же возврат полной модели с сервера, а не только Id чата
                        case "!create_chat":
                            Console.WriteLine("parameters: chatname : chat users id...");
                            var parameters3 = Console.ReadLine();
                            var usersId = parameters3.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries)[1].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            var users1 = new List<ChatUserReceiveModel>();
                            foreach (var user in users1)
                                users1.Add(new ChatUserReceiveModel()
                                {
                                    UserId = Convert.ToInt32(user)
                                });
                            CreateChat(new ChatReceiveModel()
                            {
                                ChatName = parameters3.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries)[0],
                                CreatorId = user.Id,
                                DateOfCreation = DateTime.Now,
                                ChatUsers = users1
                            });
                            Console.WriteLine();
                            break;
                        case "!update_chat":
                            Console.WriteLine("parameters: chatid, new chatname : chat users id...");
                            var parameters4 = Console.ReadLine();
                            var currentChat = chats.FirstOrDefault(c => c.Id == Convert.ToInt32(parameters4.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries)[0].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0]));
                            var usersId2 = parameters4.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries)[1].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            var users2 = new List<ChatUserReceiveModel>();
                            foreach (var user in users2)
                                users2.Add(new ChatUserReceiveModel()
                                {
                                    UserId = Convert.ToInt32(user),
                                    ChatId = currentChat.Id
                                });
                            UpdateChat(new ChatReceiveModel()
                            {
                                Id = currentChat.Id,
                                ChatName = parameters4.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries)[1].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1],
                                CreatorId = user.Id,
                                ChatUsers = users2
                            });
                            Console.WriteLine();
                            break;
                        case "!delete_chat":
                            Console.WriteLine("parameters: chatid");
                            var parameters5 = Console.ReadLine();
                            var currentChat2 = chats.FirstOrDefault(c => c.Id == Convert.ToInt32(parameters5));
                            DeleteChat(new ChatReceiveModel()
                            {
                                Id = currentChat2.Id
                            });
                            Console.WriteLine();
                            break;
                        case "!get_chats":
                            Console.WriteLine("parameters: page");
                            var page = Convert.ToInt32(Console.ReadLine());
                            GetChats(new UserPaginationReceiveModel()
                            {
                                UserId = user.Id,
                                Page = page
                            });
                            Console.WriteLine();
                            break;
                        case "!send_invite":
                            Console.WriteLine("parameters: user id");
                            var userId = Convert.ToInt32(Console.ReadLine());
                            SendInviteToFriend(new FriendReceiveModel()
                            {
                                UserId = user.Id,
                                FriendId = userId
                            });
                            Console.WriteLine();
                            break;
                        case "!get_friends":
                            Console.WriteLine("parameters: page, searchname");
                            var parameters6 = Console.ReadLine();
                            GetFriends(new UserPaginationReceiveModel()
                            {
                                UserId = user.Id,
                                Page = Convert.ToInt32(parameters6.Split(new char[]{ ' ' }, StringSplitOptions.RemoveEmptyEntries)[0]),
                                SearchingUserName = parameters6.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Count() == 2 ? parameters6.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1] : string.Empty
                            });
                            Console.WriteLine();
                            break;
                        case "!update_notification":
                            Console.WriteLine("paramenters: notification id, accept?");
                            var parameters7 = Console.ReadLine();
                            UpdateNotification(new NotificationReceiveModel()
                            {
                                Id = notifications.FirstOrDefault(n => n.Id == Convert.ToInt32(parameters7.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0])).Id,
                                IsAccepted = Convert.ToBoolean(parameters7.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1])
                            });
                            Console.WriteLine();
                            break;
                        case "!get_notifications":
                            Console.WriteLine("paramenters: page");
                            var page3 = Convert.ToInt32(Console.ReadLine());
                            GetNotifications(new UserPaginationReceiveModel()
                            {
                                UserId = user.Id,
                                Page = page3
                            });
                            Console.WriteLine();
                            break;
                        case "!send_message":
                            Console.WriteLine("paramenters: chatId, message");
                            var parameters8 = Console.ReadLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            SendMessageToChat(parameters8[1], Convert.ToInt32(parameters8[0]));
                            Console.WriteLine();
                            break;
                        case "!get_messages":
                            Console.WriteLine("paramenters: chatId, page");
                            var parameters9 = Console.ReadLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            GetMessages(new ChatPaginationReceiveModel()
                            {
                                ChatId = Convert.ToInt32(parameters9[0]),
                                Page = Convert.ToInt32(parameters9[1])
                            });
                            Console.WriteLine();
                            break;
                        case "!update_message":
                            Console.WriteLine("paramenters: chatId, messageId, message");
                            var parameters10 = Console.ReadLine().Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                            UpdateMessage(new MessageReceiveModel()
                            {
                                ChatId = Convert.ToInt32(parameters10[0]),
                                FromUserId = user.Id,
                                Id = Convert.ToInt32(parameters10[1]),
                                UserMassage = parameters10[2]
                            });
                            Console.WriteLine();
                            break;
                        case "!delete_message":
                            Console.WriteLine("paramenters: messageId");
                            var messageId = Convert.ToInt32(Console.ReadLine());
                            DeleteMessage(new MessageReceiveModel()
                            {
                                Id = messageId
                            });
                            Console.WriteLine();
                            break;
                        case "!help":
                            Console.WriteLine("All commands:");
                            Console.WriteLine("get_users");
                            Console.WriteLine("update_profile");
                            Console.WriteLine("create_chat");
                            Console.WriteLine("update_chat");
                            Console.WriteLine("delete_chat");
                            Console.WriteLine("get_chats");
                            Console.WriteLine("send_invite");
                            Console.WriteLine("get_friends");
                            Console.WriteLine("update_notification");
                            Console.WriteLine("get_notification");
                            Console.WriteLine("send_message");
                            Console.WriteLine("get_messages");
                            Console.WriteLine("show_messages");
                            Console.WriteLine("update_message");
                            Console.WriteLine("delete_message");
                            Console.WriteLine("show_users");
                            Console.WriteLine("show_chats");
                            Console.WriteLine("show_chat_info");
                            Console.WriteLine("show_friends");
                            Console.WriteLine("show_notifications");
                            Console.WriteLine();
                            break;
                        case "!show_users":
                            Console.WriteLine("users: ");
                            for(int i = 0; i < users.Count(); i++)
                                Console.WriteLine($"#{i} user id: {users[i].Id} username: {users[i].UserName}");
                            Console.WriteLine();
                            break;
                        case "!show_chats":
                            Console.WriteLine("your chats:");
                            for (int i = 0; i < chats.Count(); i++)
                                Console.WriteLine($"#{i} chat id: {chats[i].Id} chat name: {chats[i].ChatName}");
                            Console.WriteLine();
                            break;
                        case "!show_chat_info":
                            Console.WriteLine("params: chat id");
                            var chatId = Convert.ToInt32(Console.ReadLine());
                            var currentChat3 = chats.FirstOrDefault(c => c.Id == chatId);
                            Console.WriteLine($"chatId : {currentChat3.Id}, creatorId : {currentChat3.CreatorId}, count users: {currentChat3.CountUsers}");
                            Console.WriteLine("chat users: ");
                            for (int i = 0; i < currentChat3.ChatUsers.Count(); i++)
                                Console.WriteLine($"#{i} user id: {currentChat3.ChatUsers[i].Id} username: {currentChat3.ChatUsers[i].UserName}");
                            Console.WriteLine();
                            break;
                        case "!show_friends":
                            Console.WriteLine("Your friends:");
                            for (int i = 0; i < friends.Count(); i++)
                                Console.WriteLine($"#{i}, friendId : {friends[i].Id}, username : {friends[i].UserName}");
                            Console.WriteLine();
                            break;
                        case "!show_messages":
                            Console.WriteLine("parameters: chat id");
                            var chatId4 = Convert.ToInt32(Console.ReadLine());
                            for (int i = 0; i < messages[chatId4].Count(); i++)
                                Console.WriteLine($"#{i}, message id : {messages[chatId4][i].Id}, userId : {messages[chatId4][i].UserId}, chatId : {messages[chatId4][i].ChatId}, message : {messages[chatId4][i].UserMassage}");
                            Console.WriteLine();
                            break;
                        case "!show_notifications":
                            Console.WriteLine("Your notifications");
                            for (int i = 0; i < notifications.Count(); i++)
                                Console.WriteLine($"#{i}, notification id : {notifications[i].Id}, from user : {notifications[i].FromUserName}, message : {notifications[i].Message}");
                            Console.WriteLine();
                            break;
                    }
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

        #region user operations
        #region users
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

        static void UpdateProfile(UserReceiveModel model)
        {
            SendMessage(new ClientOperationMessage()
            {
                Operation = ClientOperations.UpdateProfile,
                JsonData = serializer.Serialize(model)
            });
        }
        
        static void GetUsers(UserPaginationReceiveModel userPaginationReceiveModel)
        {
            SendMessage(new ClientOperationMessage()
            {
                Operation = ClientOperations.GetUsers,
                JsonData = serializer.Serialize(userPaginationReceiveModel)
            });
        }
        #endregion
        #region messages 
        static void GetMessages(ChatPaginationReceiveModel chatPaginationReceiveModel)
        {
            SendMessage(new ClientOperationMessage()
            {
                Operation = ClientOperations.GetMessages,
                JsonData = serializer.Serialize(chatPaginationReceiveModel)
            });
        }

        static void SendMessageToChat(string message, int chatId)
        {
            SendMessage(new ClientOperationMessage()
            {
                Operation = ClientOperations.SendMessage,
                JsonData = serializer.Serialize(new MessageReceiveModel()
                {
                    Date = DateTime.Now,
                    FromUserId = user.Id,
                    UserMassage = message,
                    ChatId = chatId
                })
            });
        }

        static void UpdateMessage(MessageReceiveModel messageReceiveModel)
        {
            SendMessage(new ClientOperationMessage()
            {
                Operation = ClientOperations.UpdateMessage,
                JsonData = serializer.Serialize(messageReceiveModel)
            });
        }

        static void DeleteMessage(MessageReceiveModel messageReceiveModel)
        {
            SendMessage(new ClientOperationMessage()
            {
                Operation = ClientOperations.DeleteMessage,
                JsonData = serializer.Serialize(messageReceiveModel)
            });
        }
        #endregion
        #region chats
        static void CreateChat(ChatReceiveModel chatReceiveModel)
        {
            SendMessage(new ClientOperationMessage()
            {
                Operation = ClientOperations.CreateChat,
                JsonData = serializer.Serialize(chatReceiveModel)
            });
        }

        static void UpdateChat(ChatReceiveModel chatReceiveModel)
        {
            SendMessage(new ClientOperationMessage()
            {
                Operation = ClientOperations.UpdateChat,
                JsonData = serializer.Serialize(chatReceiveModel)
            });
        }

        static void DeleteChat(ChatReceiveModel chatReceiveModel)
        {
            SendMessage(new ClientOperationMessage()
            {
                Operation = ClientOperations.DeleteChat,
                JsonData = serializer.Serialize(chatReceiveModel)
            });
        }

        static void GetChats(UserPaginationReceiveModel userPaginationReceiveModel)
        {
            SendMessage(new ClientOperationMessage()
            {
                Operation = ClientOperations.GetChats,
                JsonData = serializer.Serialize(userPaginationReceiveModel)
            });
        }
        #endregion
        #region friends 
        static void SendInviteToFriend(FriendReceiveModel friendReceiveModel)
        {
            SendMessage(new ClientOperationMessage()
            {
                Operation = ClientOperations.AddFriend,
                JsonData = serializer.Serialize(friendReceiveModel)
            });
        }

        static void GetFriends(UserPaginationReceiveModel userPaginationReceiveModel)
        {
            SendMessage(new ClientOperationMessage()
            {
                Operation = ClientOperations.GetFriends,
                JsonData = serializer.Serialize(userPaginationReceiveModel)
            });
        }

        //not implemented 
        static void DeleteFriend(FriendReceiveModel friendReceiveModel)
        {
            SendMessage(new ClientOperationMessage()
            {
                Operation = ClientOperations.DeleteFriend,
                JsonData = serializer.Serialize(friendReceiveModel)
            });
        }
        #endregion
        #region notifications 
        static void UpdateNotification(NotificationReceiveModel notificationReceiveModel)
        {
            SendMessage(new ClientOperationMessage()
            {
                Operation = ClientOperations.UpdateNotification,
                JsonData = serializer.Serialize(notificationReceiveModel)
            });
        }

        //not implemented 
        static void GetNotifications(UserPaginationReceiveModel userPaginationReceiveModel)
        {
            SendMessage(new ClientOperationMessage()
            {
                Operation = ClientOperations.GetNotifications,
                JsonData = serializer.Serialize(userPaginationReceiveModel)
            });
        }
        #endregion
        #endregion

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

        #region listeners
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
            try
            {
                if (operationResultInfo.JsonData != null)
                {
                    Console.WriteLine("--Message received by synchronization from server : ");
                    var data = serializer.Deserialize<MessageResponseModel>(operationResultInfo.JsonData as string);
                    Console.WriteLine("MessageId = " + data.Id);
                    Console.WriteLine("UserId = " + data.UserId);
                    Console.WriteLine("ChatId = " + data.ChatId);
                    Console.WriteLine("Message = " + data.UserMassage);
                    Console.WriteLine("Date = " + data.Date);

                    if (!messages.ContainsKey(data.ChatId))
                    {
                        messages.Add(data.ChatId, new List<MessageResponseModel>
                        {
                            data
                        });
                    }
                    else
                    {
                        var message = messages[data.ChatId].FirstOrDefault(m => m.Id == data.Id 
                        || m.Date.Equals(data.Date) && m.UserId == data.UserId && m.UserMassage.Equals(data.UserMassage));
                        if (message == null)
                            messages[data.ChatId].Add(data);
                        else
                            message = data;
                    }
                }
                else
                    Console.WriteLine("JsonData = null");
            }
            catch
            {
                Console.WriteLine("--Messages received by user action : ");
                var data = serializer.Deserialize<List<MessageResponseModel>>(operationResultInfo.JsonData as string);
                var firstMessage = data.FirstOrDefault();
                if (firstMessage != null)
                    if (!messages.ContainsKey(firstMessage.ChatId))
                        messages.Add(firstMessage.ChatId, data);
                    else
                        messages[firstMessage.ChatId] = data;
                foreach(var message in data)
                {
                    Console.WriteLine("MessageId = " + message.Id);
                    Console.WriteLine("UserId = " + message.UserId);
                    Console.WriteLine("ChatId = " + message.ChatId);
                    Console.WriteLine("Message = " + message.UserMassage);
                }
            }
        }

        static void ChatListListener(OperationResultInfo operationResultInfo)
        {
            Console.WriteLine("Operation result = " + Enum.GetName(typeof(OperationsResults), operationResultInfo.OperationResult));
            Console.WriteLine("Error info = " + operationResultInfo.ErrorInfo);
            Console.WriteLine("Listener = " + Enum.GetName(typeof(ListenerType), operationResultInfo.ToListener));
            try
            {
                if (operationResultInfo.JsonData != null)
                {
                    Console.WriteLine("--Chat received by synchronization from server : ");
                    var data = serializer.Deserialize<ChatResponseModel>(operationResultInfo.JsonData as string);
                    Console.WriteLine("ChatId" + data.Id);
                    Console.WriteLine("ChatName" + data.ChatName);
                    Console.WriteLine("CreatorId" + data.CreatorId);
                    Console.WriteLine("CountUsers" + data.CountUsers);
                    foreach (var user in data.ChatUsers)
                        Console.WriteLine(user.Id + " " + user.UserName + " " + user.IsOnline);

                    var chat = chats.FirstOrDefault(c => c.Id == data.Id
                    || c.ChatName == data.ChatName && c.CreatorId == data.CreatorId && c.CountUsers == data.CountUsers);

                    if (chat == null)
                        chats.Add(data);
                    else
                        chat = data;

                }
                else
                    Console.WriteLine("JsonData = null");
            }
            catch
            {
                Console.WriteLine("--Chat received by user action : ");
                var data = serializer.Deserialize<List<ChatResponseModel>>(operationResultInfo.JsonData as string);

                chats = data;

                foreach (var chat in data)
                {
                    Console.WriteLine("ChatId" + chat.Id);
                    Console.WriteLine("ChatName" + chat.ChatName);
                    Console.WriteLine("CreatorId" + chat.CreatorId);
                    Console.WriteLine("CountUsers" + chat.CountUsers);
                }
            }
        }

        static void ChatListDeleteListener(OperationResultInfo operationResultInfo)
        {
            Console.WriteLine("Operation result = " + Enum.GetName(typeof(OperationsResults), operationResultInfo.OperationResult));
            Console.WriteLine("Error info = " + operationResultInfo.ErrorInfo);
            Console.WriteLine("Listener = " + Enum.GetName(typeof(ListenerType), operationResultInfo.ToListener));

            var data = serializer.Deserialize<ChatResponseModel>(operationResultInfo.JsonData as string);
            Console.WriteLine("--Chat for deleting received by synchronization from server : ");
            Console.WriteLine("ChatId" + data.Id);
            Console.WriteLine("ChatName" + data.ChatName);
            Console.WriteLine("CreatorId" + data.CreatorId);
            Console.WriteLine("CountUsers" + data.CountUsers);
            foreach (var user in data.ChatUsers)
                Console.WriteLine(user.Id + " " + user.UserName + " " + user.IsOnline);

            var deletingChat = chats.FirstOrDefault(c => c.Id == data.Id);
            if(deletingChat != null)
                chats.Remove(deletingChat);
        }

        static void MessageDeleteListListener(OperationResultInfo operationResultInfo)
        {
            Console.WriteLine("Message for deleting : ");
            Console.WriteLine("Operation result = " + Enum.GetName(typeof(OperationsResults), operationResultInfo.OperationResult));
            Console.WriteLine("Error info = " + operationResultInfo.ErrorInfo);
            Console.WriteLine("Listener = " + Enum.GetName(typeof(ListenerType), operationResultInfo.ToListener));
            var data = serializer.Deserialize<MessageResponseModel>(operationResultInfo.JsonData as string);
            Console.WriteLine("--Message for deleting received by synchronization from server : ");
            Console.WriteLine("MessageId = " + data.Id);
            Console.WriteLine("UserId = " + data.UserId);
            Console.WriteLine("ChatId = " + data.ChatId);
            Console.WriteLine("Message = " + data.UserMassage);

            var deletingMessage = messages[data.ChatId].FirstOrDefault(m => m.Id == data.Id);
            if (deletingMessage != null)
                messages[data.ChatId].Remove(deletingMessage);
        }

        static void UserListListener(OperationResultInfo operationResultInfo)
        {
            Console.WriteLine("Operation result = " + Enum.GetName(typeof(OperationsResults), operationResultInfo.OperationResult));
            Console.WriteLine("Error info = " + operationResultInfo.ErrorInfo);
            Console.WriteLine("Listener = " + Enum.GetName(typeof(ListenerType), operationResultInfo.ToListener));
            var data = serializer.Deserialize<List<UserListResponseModel>>(operationResultInfo.JsonData as string);
            Console.WriteLine("--Users received by user action : ");
            foreach (var user in data)
            {
                Console.WriteLine("UserId = " + user.Id);
                Console.WriteLine("Username = " + user.UserName);
            }

            users = data;
        }

        static void FriendsListListener(OperationResultInfo operationResultInfo)
        {
            Console.WriteLine("Operation result = " + Enum.GetName(typeof(OperationsResults), operationResultInfo.OperationResult));
            Console.WriteLine("Error info = " + operationResultInfo.ErrorInfo);
            Console.WriteLine("Listener = " + Enum.GetName(typeof(ListenerType), operationResultInfo.ToListener));
            try
            {
                if (operationResultInfo.JsonData != null)
                {
                    Console.WriteLine("Friend received by server synchronization, he accepted invite : ");
                    var data = serializer.Deserialize<UserListResponseModel>(operationResultInfo.JsonData as string);
                    friends.Add(data);
                    Console.WriteLine("User Id : " + data.Id);
                    Console.WriteLine("Username : " + data.UserName);
                }
                else
                    Console.WriteLine("JsonData = null");
            }   
            catch
            {
                var data = serializer.Deserialize<List<UserListResponseModel>>(operationResultInfo.JsonData as string);
                friends = data;
                Console.WriteLine("--Friends received by user action : ");
                foreach (var user in data)
                {
                    Console.WriteLine("UserId = " + user.Id);
                    Console.WriteLine("Username = " + user.UserName);
                }
            }
        }

        static void FriendsDeleteListListener(OperationResultInfo operationResultInfo)
        {
            Console.WriteLine("Operation result = " + Enum.GetName(typeof(OperationsResults), operationResultInfo.OperationResult));
            Console.WriteLine("Error info = " + operationResultInfo.ErrorInfo);
            Console.WriteLine("Listener = " + Enum.GetName(typeof(ListenerType), operationResultInfo.ToListener));
            var data = serializer.Deserialize<UserListResponseModel>(operationResultInfo.JsonData as string);
            Console.WriteLine("--Friend for deleting received by synchronization from server : ");
            Console.WriteLine("UserId = " + data.Id);
            Console.WriteLine("Username = " + data.UserName);
        }

        static void NotificationListListener(OperationResultInfo operationResultInfo)
        {
            Console.WriteLine("Operation result = " + Enum.GetName(typeof(OperationsResults), operationResultInfo.OperationResult));
            Console.WriteLine("Error info = " + operationResultInfo.ErrorInfo);
            Console.WriteLine("Listener = " + Enum.GetName(typeof(ListenerType), operationResultInfo.ToListener));
            try
            {
                if (operationResultInfo.JsonData != null)
                {
                    Console.WriteLine("--Notification received by synchronization from server : ");
                    var data = serializer.Deserialize<NotificationResponseModel>(operationResultInfo.JsonData as string);
                    Console.WriteLine("NotificationId " + data.Id);
                    Console.WriteLine("FromUser " + data.FromUserName);
                    Console.WriteLine("NotificationMessage " + data.Message);

                    notifications.Add(data);
                }
                else
                    Console.WriteLine("JsonData = null");
            }
            catch
            {
                var data = serializer.Deserialize<List<NotificationResponseModel>>(operationResultInfo.JsonData as string);
                Console.WriteLine("--Notifications received by user action : ");
                foreach (var notification in data)
                {
                    Console.WriteLine("NotificationId " + notification.Id);
                    Console.WriteLine("FromUser " + notification.FromUserName);
                    Console.WriteLine("NotificationMessage " + notification.Message);
                }
                notifications = data;
            }
        }
        #endregion

        static void Disconnect()
        {
            if (networkStream != null)
                networkStream.Close();
            tcpClient.Close();
        }
    }
}
