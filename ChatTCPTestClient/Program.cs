using ChatTCPServer;
using ChatTCPServer.Services;
using Microsoft.SqlServer.Server;
using ServerBusinessLogic.Enums;
using ServerBusinessLogic.Enums.Transmission;
using ServerBusinessLogic.Models;
using ServerBusinessLogic.Models.ResponseModels.EncodingModels;
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
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management.Instrumentation;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace ChatTCPTestClient
{
    class Program
    {
        static string serverIp = "25.68.135.116";
        static int serverPort = 8668;
        static TcpClient tcpClient;
        static NetworkStream networkStream;
        static UserResponseModel user;
        static JsonStringSerializer serializer = new JsonStringSerializer();
        static DataManager DataManager = new DataManager();

        static List<ChatResponseModel> chats = new List<ChatResponseModel>();
        static List<UserListResponseModel> users = new List<UserListResponseModel>();
        static List<UserListResponseModel> friends = new List<UserListResponseModel>();
        //key = chatId, value - messages
        static Dictionary<int, List<MessageResponseModel>> messages = new Dictionary<int, List<MessageResponseModel>>();
        static List<NotificationResponseModel> notifications = new List<NotificationResponseModel>();

        static ChatTCPServer.Services.Encoder encoder;

        static void Main(string[] args)
        {
            try
            {
                Console.OutputEncoding = Encoding.Unicode;
                Console.InputEncoding = Encoding.Unicode;
                tcpClient = new TcpClient();
                tcpClient.Connect(IPAddress.Parse(serverIp), serverPort);

                encoder = new ChatTCPServer.Services.Encoder();
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
                DataManager.AddListener(ListenerType.UserInfoListener, UserInfoListener);
                DataManager.AddListener(ListenerType.UserUpdateProfileListener, UserUpdateProfileListener);

                Task.Run(() => RecieveMessages());
             
                while(true)
                {
                    var operation = Console.ReadLine();
                    switch (operation)
                    {
                        case "!registration":
                            Console.Clear();
                            Console.WriteLine("parameters: login, password");
                            var params1 = Console.ReadLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            Registration(params1[0], params1[1]);
                            Console.WriteLine();
                            break;
                        case "!authorization":
                            Console.Clear();
                            Console.WriteLine("parameters: login, password");
                            var params2 = Console.ReadLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            Authorization(params2[0], params2[1]);
                            Console.WriteLine();
                            break;
                        case "!get_users":
                            Console.Clear();
                            Console.WriteLine("parameters: page, searching username");
                            var parameters = Console.ReadLine();
                            GetUsers(new UserPaginationReceiveModel()
                            {
                                UserId = user.UserId,
                                Page = Convert.ToInt32(parameters.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0]),
                                SearchingUserName = parameters.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Count() == 2 ? parameters.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1] : string.Empty
                            });
                            Console.WriteLine();
                            break;
                        case "!update_profile":
                            Console.Clear();
                            Console.WriteLine("parameters: username, password, name, secondname, gender, phonenumber, country, city");
                            var parameters2 = Console.ReadLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            Console.WriteLine("FilePath :");
                            var filePath = Console.ReadLine();
                            FileModel fileModel = new FileModel()
                            {
                                    FileName = filePath.Replace(Path.GetExtension(filePath), "").Split(new char[] { '\\' }).Last(),
                                    Extension = Path.GetExtension(filePath),
                                    BinaryForm = File.ReadAllBytes(filePath)
                                };
                            UpdateProfile(new UserReceiveModel()
                            {
                                Id = user.UserId,
                                UserName = parameters2[0],
                                Password = parameters2[1],
                                Name = parameters2[2],
                                SecondName = parameters2[3],
                                Gender = (Gender)Enum.GetValues(typeof(Gender)).GetValue(Convert.ToInt32(parameters2[4])),
                                PhoneNumber = parameters2[5],
                                Country = (Country)Enum.GetValues(typeof(Country)).GetValue(Convert.ToInt32(parameters2[6])),
                                City = (City)Enum.GetValues(typeof(City)).GetValue(Convert.ToInt32(parameters2[7])),
                                File = fileModel
                            });
                            Console.WriteLine();
                            break;
                        case "!create_chat":
                            Console.Clear();
                            Console.WriteLine("parameters: chatname : chat users id...");
                            var parameters3 = Console.ReadLine();
                            var usersId = parameters3.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries)[1].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            var users1 = new List<ChatUserReceiveModel>();
                            foreach (var user in usersId)
                            {
                                users1.Add(new ChatUserReceiveModel()
                                {
                                    UserId = Convert.ToInt32(user)
                                });
                            }
                            //добавляем себя в этот список
                            users1.Add(new ChatUserReceiveModel()
                            {
                                UserId = user.UserId
                            });
                            CreateChat(new ChatReceiveModel()
                            {
                                ChatName = parameters3.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries)[0],
                                CreatorId = user.UserId,
                                DateOfCreation = DateTime.Now,
                                ChatUsers = users1
                            });
                            Console.WriteLine();
                            break;
                        case "!update_chat":
                            Console.Clear();
                            Console.WriteLine("parameters: chatid, new chatname : chat users id...");
                            var parameters4 = Console.ReadLine();
                            var currentChat = chats.FirstOrDefault(c => c.Id == Convert.ToInt32(parameters4.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries)[0].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0]));
                            var usersId2 = parameters4.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries)[1].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            var users2 = new List<ChatUserReceiveModel>();
                            foreach (var user in usersId2)
                                users2.Add(new ChatUserReceiveModel()
                                {
                                    UserId = Convert.ToInt32(user),
                                    ChatId = currentChat.Id
                                });
                            //добавляем себя в этот список
                            users2.Add(new ChatUserReceiveModel()
                            {
                                UserId = user.UserId,
                                ChatId = currentChat.Id
                            });
                            UpdateChat(new ChatReceiveModel()
                            {
                                Id = currentChat.Id,
                                ChatName = parameters4.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries)[0].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1],
                                CreatorId = user.UserId,
                                ChatUsers = users2
                            });
                            Console.WriteLine();
                            break;
                        case "!delete_chat":
                            Console.Clear();
                            Console.WriteLine("parameters: chatid");
                            var parameters5 = Console.ReadLine();
                            var currentChat2 = chats.FirstOrDefault(c => c.Id == Convert.ToInt32(parameters5));
                            DeleteChat(new ChatReceiveModel()
                            {
                                Id = currentChat2.Id,
                                ChatUsers = currentChat2.ChatUsers.Select(cu => new ChatUserReceiveModel()
                                {
                                    ChatId = currentChat2.Id,
                                    UserId = cu.Id
                                }).ToList()
                            });
                            Console.WriteLine();
                            break;
                        case "!get_chats":
                            Console.Clear();
                            Console.WriteLine("parameters: page");
                            var page = Convert.ToInt32(Console.ReadLine());
                            GetChats(new UserPaginationReceiveModel()
                            {
                                UserId = user.UserId,
                                Page = page
                            });
                            Console.WriteLine();
                            break;
                        case "!send_invite":
                            Console.Clear();
                            Console.WriteLine("parameters: user id");
                            var userId = Convert.ToInt32(Console.ReadLine());
                            SendInviteToFriend(new FriendReceiveModel()
                            {
                                UserId = user.UserId,
                                FriendId = userId
                            });
                            Console.WriteLine();
                            break;
                        case "!get_friends":
                            Console.Clear();
                            Console.WriteLine("parameters: page, searchname");
                            var parameters6 = Console.ReadLine();
                            GetFriends(new UserPaginationReceiveModel()
                            {
                                UserId = user.UserId,
                                Page = Convert.ToInt32(parameters6.Split(new char[]{ ' ' }, StringSplitOptions.RemoveEmptyEntries)[0]),
                                SearchingUserName = parameters6.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Count() == 2 ? parameters6.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1] : string.Empty
                            });
                            Console.WriteLine();
                            break;
                        case "!update_notification":
                            Console.Clear();
                            Console.WriteLine("paramenters: notification id, accept?");
                            var parameters7 = Console.ReadLine();
                            var notification = notifications.FirstOrDefault(n => n.Id == Convert.ToInt32(parameters7.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0]));
                            UpdateNotification(new NotificationReceiveModel()
                            {
                                Id = notification.Id,
                                FromUserId = notification.FromUserId,
                                ToUserId = user.UserId,
                                IsAccepted = Convert.ToBoolean(parameters7.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1])
                            });
                            Console.WriteLine();
                            break;
                        case "!get_notifications":
                            Console.Clear();
                            Console.WriteLine("paramenters: page");
                            var page3 = Convert.ToInt32(Console.ReadLine());
                            GetNotifications(new UserPaginationReceiveModel()
                            {
                                UserId = user.UserId,
                                Page = page3
                            });
                            Console.WriteLine();
                            break;
                        case "!send_message":
                            Console.Clear();
                            Console.WriteLine("paramenters: chatId, message, send file?");
                            var parameters8 = Console.ReadLine().Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                            FileModel model = default;
                            if(bool.Parse(parameters8[2]))
                            {
                                Console.WriteLine("filePath:");
                                var filePath1 = Console.ReadLine();
                                model = new FileModel()
                                {
                                    FileName = filePath1.Replace(Path.GetExtension(filePath1), "").Split(new char[] { '\\' }).Last(),
                                    Extension = Path.GetExtension(filePath1),
                                    BinaryForm = File.ReadAllBytes(filePath1)
                                };
                            }
                            SendMessageToChat(new MessageReceiveModel()
                            {
                                Date = DateTime.Now,
                                FromUserId = user.UserId,
                                UserMassage = parameters8[1],
                                ChatId = Convert.ToInt32(parameters8[0]),
                                File = model
                            });
                            Console.WriteLine();
                            break;
                        case "!get_messages":
                            Console.Clear();
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
                            Console.Clear();
                            Console.WriteLine("parameters: chatId, messageId, message, change file?");
                            var parameters10 = Console.ReadLine().Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                            FileModel fm = default;
                            if(bool.Parse(parameters10[2]))
                            {
                                Console.WriteLine("filePath:");
                                var filePath2 = Console.ReadLine();
                                fm = new FileModel()
                                {
                                    FileName = filePath2.Replace(Path.GetExtension(filePath2), "").Split(new char[] { '\\' }).Last(),
                                    Extension = Path.GetExtension(filePath2),
                                    BinaryForm = File.ReadAllBytes(filePath2)
                                };
                            }
                            UpdateMessage(new MessageReceiveModel()
                            {
                                //для получения списка пользователей чата при синхронизации
                                ChatId = Convert.ToInt32(parameters10[0].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0]),
                                //для того, чтобы исключить текущего пользователя из списка пользователей для синхронизации
                                FromUserId = user.UserId,
                                Id = Convert.ToInt32(parameters10[0].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1]),
                                UserMassage = parameters10[1],
                                File = fm
                            });
                            Console.WriteLine();
                            break;
                        case "!delete_message":
                            Console.Clear();
                            Console.WriteLine("paramenters: messageId, chatId");
                            var parameters11 = Console.ReadLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            DeleteMessage(new MessageReceiveModel()
                            {
                                Id = Convert.ToInt32(parameters11[0]),
                                ChatId = Convert.ToInt32(parameters11[1]),
                                FromUserId = user.UserId
                            });
                            Console.WriteLine();
                            break;
                        case "!delete_friend":
                            Console.Clear();
                            Console.WriteLine("parameters: friend id");
                            var friendId = Convert.ToInt32(Console.ReadLine());
                            DeleteFriend(new FriendReceiveModel()
                            {
                                UserId = user.UserId,
                                FriendId = friendId
                            });
                            break;
                        case "!get_user":
                            Console.Clear();
                            Console.WriteLine("parameters: user id");
                            var userInfoId = Convert.ToInt32(Console.ReadLine());
                            GetUser(new UserReceiveModel()
                            {
                                Id = userInfoId
                            });
                            Console.WriteLine();
                            break;
                        case "!help":
                            Console.Clear();
                            Console.WriteLine("All commands:");
                            Console.WriteLine("authorization +");
                            Console.WriteLine("registration +");
                            Console.WriteLine("get_users +");
                            Console.WriteLine("update_profile +");
                            Console.WriteLine("create_chat +");
                            Console.WriteLine("update_chat +");
                            Console.WriteLine("delete_chat +");
                            Console.WriteLine("get_chats +");
                            Console.WriteLine("send_invite +");
                            Console.WriteLine("get_friends +");
                            Console.WriteLine("update_notification +");
                            Console.WriteLine("get_notifications +");
                            Console.WriteLine("send_message +");
                            Console.WriteLine("get_messages +");
                            Console.WriteLine("show_messages");
                            Console.WriteLine("update_message +");
                            Console.WriteLine("delete_message +");
                            Console.WriteLine("delete_friend +");
                            Console.WriteLine("get_user +");
                            Console.WriteLine("show_users");
                            Console.WriteLine("show_chats");
                            Console.WriteLine("show_chat_info");
                            Console.WriteLine("show_friends");
                            Console.WriteLine("show_notifications");
                            Console.WriteLine();
                            break;
                        case "!show_users":
                            Console.Clear();
                            Console.WriteLine("users: ");
                            Console.ForegroundColor = ConsoleColor.Green;
                            for(int i = 0; i < users.Count(); i++)
                                Console.WriteLine($"#{i} user id: {users[i].UserId} username: {users[i].UserName}");
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine();
                            break;
                        case "!show_chats":
                            Console.Clear();
                            Console.WriteLine("your chats:");
                            Console.ForegroundColor = ConsoleColor.Green;
                            for (int i = 0; i < chats.Count(); i++)
                                Console.WriteLine($"#{i} chat id: {chats[i].Id} chat name: {chats[i].ChatName}");
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine();
                            break;
                        case "!show_chat_info":
                            Console.Clear();
                            Console.WriteLine("params: chat id");
                            Console.ForegroundColor = ConsoleColor.Green;
                            var chatId = Convert.ToInt32(Console.ReadLine());
                            var currentChat3 = chats.FirstOrDefault(c => c.Id == chatId);
                            Console.WriteLine($"chatId : {currentChat3.Id}, creatorId : {currentChat3.CreatorId}, count users: {currentChat3.CountUsers}");
                            Console.WriteLine("chat users: ");
                            for (int i = 0; i < currentChat3.ChatUsers.Count(); i++)
                                Console.WriteLine($"#{i} user id: {currentChat3.ChatUsers[i].Id} username: {currentChat3.ChatUsers[i].UserName}");
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine();
                            break;
                        case "!show_friends":
                            Console.Clear();
                            Console.WriteLine("Your friends:");
                            Console.ForegroundColor = ConsoleColor.Green;
                            for (int i = 0; i < friends.Count(); i++)
                                Console.WriteLine($"#{i}, friendId : {friends[i].UserId}, username : {friends[i].UserName}");
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine();
                            break;
                        case "!show_messages":
                            Console.Clear();
                            Console.WriteLine("parameters: chat id");
                            Console.ForegroundColor = ConsoleColor.Green;
                            var chatId4 = Convert.ToInt32(Console.ReadLine());
                            for (int i = 0; i < messages[chatId4].Count(); i++)
                                Console.WriteLine($"#{i}, message id : {messages[chatId4][i].Id}, userId : {messages[chatId4][i].UserId}, chatId : {messages[chatId4][i].ChatId}, message : {messages[chatId4][i].UserMassage}");
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine();
                            break;
                        case "!show_notifications":
                            Console.Clear();
                            Console.WriteLine("Your notifications");
                            Console.ForegroundColor = ConsoleColor.Green;
                            for (int i = 0; i < notifications.Count(); i++)
                                Console.WriteLine($"#{i}, notification id : {notifications[i].Id}, from user : {notifications[i].FromUserName}, message : {notifications[i].Message}");
                            Console.ForegroundColor = ConsoleColor.White;
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

        static void GetUser(UserReceiveModel userReceiveModel)
        {
            SendMessage(new ClientOperationMessage()
            {
                Operation = ClientOperations.GetUser,
                JsonData = serializer.Serialize(userReceiveModel)
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

        static void SendMessageToChat(MessageReceiveModel messageReceiveModel)
        {
            SendMessage(new ClientOperationMessage()
            {
                Operation = ClientOperations.SendMessage,
                JsonData = serializer.Serialize(messageReceiveModel)
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
            byte[] data = encoder.Encryption(serializer.Serialize(clientOperationMessage));
            
            Console.WriteLine("sended data : " + encoder.Encryption(serializer.Serialize(clientOperationMessage)));

            Console.WriteLine("data send length = " + data.Length);
            networkStream.Write(data, 0, data.Length);
        }

        static void RecieveMessages()
        {
            byte[] data = new byte[256];
            List<byte> byteMessage = new List<byte>();

            while (true)
            {
                do
                {
                    //!!! Считываем количество прочитанных байтов, а затем именно это кол-во переводим в строку!!!
                    //!!! Если переводить в строку постоянное колиество байтов из буффера - то в случае, если прочитанных байт будет меньше
                    //!!! чем мы переводим в строку - то мы получим пробелы, и вся десериализация провалится. 
                    int count = networkStream.Read(data, 0, 256);

                    for(int i = 0; i < count; i++)
                    {
                        byteMessage.Add(data[i]);
                    }
                } while (networkStream.DataAvailable);

                try
                {
                    OperationResultInfo obj = serializer.Deserialize<OperationResultInfo>(encoder.Decryption(byteMessage.ToArray()));
                    byteMessage.Clear();
                    DataManager.HandleData(obj.ToListener, obj);
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
            }
        }

        #region listeners
        static void AuthorizationListener(OperationResultInfo operationResultInfo)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Operation result = " + Enum.GetName(typeof(OperationsResults), operationResultInfo.OperationResult));
            Console.WriteLine("Error info = " + operationResultInfo.ErrorInfo);
            Console.WriteLine("Listener = " + Enum.GetName(typeof(ListenerType), operationResultInfo.ToListener));
            var json = operationResultInfo.JsonData.ToString();
            user = serializer.Deserialize<UserResponseModel>(json);
            if (user != null)
            {
                Console.WriteLine("UserId = " + user.UserId);
                Console.WriteLine("UserName = " + user.UserName);
                Console.WriteLine("Name = " + user.Name);
                Console.WriteLine("Second name = " + user.SecondName);
                Console.WriteLine("Phone  = " + user.PhoneNumber);
                Console.WriteLine("Gender = " + Enum.GetName(typeof(Gender), user.Gender));
                Console.WriteLine("City = " + Enum.GetName(typeof(City), user.City));
                Console.WriteLine("Country = " + Enum.GetName(typeof(Country), user.Country));
                Console.WriteLine("IsOnline = " + user.IsOnline);

                File.WriteAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, user.File.FileName + user.File.Extension), user.File.BinaryForm);
            }
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
        }

        static void RegistrationListener(OperationResultInfo operationResultInfo)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Reg Operation result = " + Enum.GetName(typeof(OperationsResults), operationResultInfo.OperationResult));
            Console.WriteLine("Error info = " + operationResultInfo.ErrorInfo);
            Console.WriteLine("Listener = " + Enum.GetName(typeof(ListenerType), operationResultInfo.ToListener));
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
        }

        static void MessageListListener(OperationResultInfo operationResultInfo)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Operation result = " + Enum.GetName(typeof(OperationsResults), operationResultInfo.OperationResult));
            Console.WriteLine("Error info = " + operationResultInfo.ErrorInfo);
            Console.WriteLine("Listener = " + Enum.GetName(typeof(ListenerType), operationResultInfo.ToListener));
            try
            {
                if (operationResultInfo.JsonData != null)
                {
                    Console.WriteLine("--Message received by synchronization from server : ");
                    var data = serializer.Deserialize<MessageResponseModel>(operationResultInfo.JsonData.ToString());
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

                    if (data.File != null)
                    {
                        File.WriteAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, data.File.FileName + data.File.Extension), data.File.BinaryForm); 
                    }
                }
                else
                    Console.WriteLine("JsonData = null");
            }
            catch
            {
                Console.WriteLine("--Messages received by user action : ");
                var data = serializer.Deserialize<List<MessageResponseModel>>(operationResultInfo.JsonData.ToString());
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
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
        }

        static void ChatListListener(OperationResultInfo operationResultInfo)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Operation result = " + Enum.GetName(typeof(OperationsResults), operationResultInfo.OperationResult));
            Console.WriteLine("Error info = " + operationResultInfo.ErrorInfo);
            Console.WriteLine("Listener = " + Enum.GetName(typeof(ListenerType), operationResultInfo.ToListener));
            try
            {
                if (operationResultInfo.JsonData != null)
                {
                    Console.WriteLine("--Chat received by synchronization from server : ");
                    var data = serializer.Deserialize<ChatResponseModel>(operationResultInfo.JsonData.ToString());
                    Console.WriteLine("ChatId = " + data.Id);
                    Console.WriteLine("ChatName = " + data.ChatName);
                    Console.WriteLine("CreatorId = " + data.CreatorId);
                    Console.WriteLine("CountUsers = " + data.CountUsers);
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
                var data = serializer.Deserialize<List<ChatResponseModel>>(operationResultInfo.JsonData.ToString());

                chats = data;

                foreach (var chat in data)
                {
                    Console.WriteLine("ChatId = " + chat.Id);
                    Console.WriteLine("ChatName = " + chat.ChatName);
                    Console.WriteLine("CreatorId = " + chat.CreatorId);
                    Console.WriteLine("CountUsers = " + chat.CountUsers);
                }
            }
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
        }

        static void ChatListDeleteListener(OperationResultInfo operationResultInfo)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Operation result = " + Enum.GetName(typeof(OperationsResults), operationResultInfo.OperationResult));
            Console.WriteLine("Error info = " + operationResultInfo.ErrorInfo);
            Console.WriteLine("Listener = " + Enum.GetName(typeof(ListenerType), operationResultInfo.ToListener));

            var data = serializer.Deserialize<ChatResponseModel>(operationResultInfo.JsonData.ToString());
            Console.WriteLine("--Chat for deleting received by synchronization from server : ");
            Console.WriteLine("ChatId = " + data.Id);
            Console.WriteLine("ChatName = " + data.ChatName);
            Console.WriteLine("CreatorId = " + data.CreatorId);
            Console.WriteLine("CountUsers = " + data.CountUsers);
            foreach (var user in data.ChatUsers)
                Console.WriteLine(user.Id + " " + user.UserName + " " + user.IsOnline);

            var deletingChat = chats.FirstOrDefault(c => c.Id == data.Id);
            if(deletingChat != null)
                chats.Remove(deletingChat);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
        }

        static void MessageDeleteListListener(OperationResultInfo operationResultInfo)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Message for deleting : ");
            Console.WriteLine("Operation result = " + Enum.GetName(typeof(OperationsResults), operationResultInfo.OperationResult));
            Console.WriteLine("Error info = " + operationResultInfo.ErrorInfo);
            Console.WriteLine("Listener = " + Enum.GetName(typeof(ListenerType), operationResultInfo.ToListener));
            try
            {
                if (operationResultInfo.JsonData != null)
                {
                    var data = serializer.Deserialize<MessageResponseModel>(operationResultInfo.JsonData.ToString());
                    Console.WriteLine("--Message for deleting received by synchronization from server : ");
                    Console.WriteLine("MessageId = " + data.Id);
                    Console.WriteLine("UserId = " + data.UserId);
                    Console.WriteLine("ChatId = " + data.ChatId);
                    Console.WriteLine("Message = " + data.UserMassage);

                    if (messages.ContainsKey(data.ChatId))
                    {
                        var deletingMessage = messages[data.ChatId].FirstOrDefault(m => m.Id == data.Id);
                        if (deletingMessage != null)
                            messages[data.ChatId].Remove(deletingMessage);
                    }
                }
            }
            catch { }
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
        }

        static void UserListListener(OperationResultInfo operationResultInfo)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Operation result = " + Enum.GetName(typeof(OperationsResults), operationResultInfo.OperationResult));
            Console.WriteLine("Error info = " + operationResultInfo.ErrorInfo);
            Console.WriteLine("Listener = " + Enum.GetName(typeof(ListenerType), operationResultInfo.ToListener));
            try
            {
                var data = serializer.Deserialize<List<UserListResponseModel>>(operationResultInfo.JsonData.ToString());
                Console.WriteLine("--Users received by user action : ");
                foreach (var user in data)
                {
                    Console.WriteLine("UserId = " + user.UserId);
                    Console.WriteLine("Username = " + user.UserName);
                }

                users = data;
            }
            catch { }
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
        }

        static void UserInfoListener(OperationResultInfo operationResultInfo)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Operation result = " + Enum.GetName(typeof(OperationsResults), operationResultInfo.OperationResult));
            Console.WriteLine("Error info = " + operationResultInfo.ErrorInfo);
            Console.WriteLine("Listener = " + Enum.GetName(typeof(ListenerType), operationResultInfo.ToListener));
            var data = serializer.Deserialize<UserResponseModel>(operationResultInfo.JsonData.ToString());
            Console.WriteLine();
            try
            {
                Console.WriteLine("--User received by user action : ");
                Console.WriteLine("user id = " + data.UserId);
                Console.WriteLine("username = " + data.UserName);
                Console.WriteLine("name = " + data.Name);
                Console.WriteLine("second name = " + data.SecondName);
                Console.WriteLine("phone = " + data.PhoneNumber);
                Console.WriteLine("gender = " + Enum.GetName(typeof(Gender), data.Gender));
                Console.WriteLine("city = " + Enum.GetName(typeof(City), data.City));
                Console.WriteLine("country = " + Enum.GetName(typeof(Country), data.Country));
                Console.WriteLine("is online = " + data.IsOnline);
            }
            catch { }

            File.WriteAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, data.File.FileName + data.File.Extension), data.File.BinaryForm);

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
        }

        static void FriendsListListener(OperationResultInfo operationResultInfo)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Operation result = " + Enum.GetName(typeof(OperationsResults), operationResultInfo.OperationResult));
            Console.WriteLine("Error info = " + operationResultInfo.ErrorInfo);
            Console.WriteLine("Listener = " + Enum.GetName(typeof(ListenerType), operationResultInfo.ToListener));
            try
            {
                if (operationResultInfo.JsonData != null)
                {
                    Console.WriteLine("Friend received by server synchronization, he accepted invite : ");
                    var data = serializer.Deserialize<UserListResponseModel>(operationResultInfo.JsonData.ToString());
                    friends.Add(data);
                    Console.WriteLine("User Id = " + data.UserId);
                    Console.WriteLine("Username = " + data.UserName);
                    Console.WriteLine("IsOnline = " + data.IsOnline);
                }
                else
                    Console.WriteLine("JsonData = null");
            }   
            catch
            {
                var data = serializer.Deserialize<List<UserListResponseModel>>(operationResultInfo.JsonData.ToString());
                friends = data;
                Console.WriteLine("--Friends received by user action : ");
                foreach (var user in data)
                {
                    Console.WriteLine("UserId = " + user.UserId);
                    Console.WriteLine("Username = " + user.UserName);
                    Console.WriteLine("IsOnline = " + user.IsOnline);
                }
            }
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
        }

        static void FriendsDeleteListListener(OperationResultInfo operationResultInfo)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Operation result = " + Enum.GetName(typeof(OperationsResults), operationResultInfo.OperationResult));
            Console.WriteLine("Error info = " + operationResultInfo.ErrorInfo);
            Console.WriteLine("Listener = " + Enum.GetName(typeof(ListenerType), operationResultInfo.ToListener));
            var data = serializer.Deserialize<UserListResponseModel>(operationResultInfo.JsonData.ToString());
            Console.WriteLine("--Friend for deleting received by synchronization from server : ");
            Console.WriteLine("UserId = " + data.UserId);
            Console.WriteLine("Username = " + data.UserName);

            friends.Remove(friends.FirstOrDefault(f => f.UserId == data.UserId));
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
        }

        static void NotificationListListener(OperationResultInfo operationResultInfo)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Operation result = " + Enum.GetName(typeof(OperationsResults), operationResultInfo.OperationResult));
            Console.WriteLine("Error info = " + operationResultInfo.ErrorInfo);
            Console.WriteLine("Listener = " + Enum.GetName(typeof(ListenerType), operationResultInfo.ToListener));
            try
            {
                if (operationResultInfo.JsonData != null)
                {
                    Console.WriteLine("--Notification received by synchronization from server : ");
                    var data = serializer.Deserialize<NotificationResponseModel>(operationResultInfo.JsonData.ToString());
                    Console.WriteLine("NotificationId = " + data.Id);
                    Console.WriteLine("FromUser = " + data.FromUserName);
                    Console.WriteLine("FromUserId = " + data.FromUserId);
                    Console.WriteLine("NotificationMessage = " + data.Message);

                    notifications.Add(data);
                }
                else
                    Console.WriteLine("JsonData = null");
            }
            catch
            {
                var data = serializer.Deserialize<List<NotificationResponseModel>>(operationResultInfo.JsonData.ToString());
                Console.WriteLine("--Notifications received by user action : ");
                foreach (var notification in data)
                {
                    Console.WriteLine("NotificationId = " + notification.Id);
                    Console.WriteLine("FromUser = " + notification.FromUserName);
                    Console.WriteLine("NotificationMessage = " + notification.Message);
                }
                notifications = data;
            }
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
        }

        static void UserUpdateProfileListener(OperationResultInfo operationResultInfo)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Operation result = " + Enum.GetName(typeof(OperationsResults), operationResultInfo.OperationResult));
            Console.WriteLine("Error info = " + operationResultInfo.ErrorInfo);
            Console.WriteLine("Listener = " + Enum.GetName(typeof(ListenerType), operationResultInfo.ToListener));
            try
            {
                if (operationResultInfo.JsonData != null)
                {
                    var data = serializer.Deserialize<UserResponseModel>(operationResultInfo.JsonData.ToString());
                    Console.WriteLine();
                    Console.WriteLine("--User received by user action : ");
                    Console.WriteLine("user id = " + data.UserId);
                    Console.WriteLine("username = " + data.UserName);
                    Console.WriteLine("name = " + data.Name);
                    Console.WriteLine("second name = " + data.SecondName);
                    Console.WriteLine("phone = " + data.PhoneNumber);
                    Console.WriteLine("gender = " + Enum.GetName(typeof(Gender), data.Gender));
                    Console.WriteLine("city = " + Enum.GetName(typeof(City), data.City));
                    Console.WriteLine("country = " + Enum.GetName(typeof(Country), data.Country));
                    Console.WriteLine("is online = " + data.IsOnline);
                }
            }
            catch { }
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
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
