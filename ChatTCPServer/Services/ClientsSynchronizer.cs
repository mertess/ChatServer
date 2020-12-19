using ServerBusinessLogic.BusinessLogic;
using ServerBusinessLogic.Enums.Transmission;
using ServerBusinessLogic.Interfaces;
using ServerBusinessLogic.ReceiveModels.ChatModels;
using ServerBusinessLogic.ReceiveModels.FriendModels;
using ServerBusinessLogic.ReceiveModels.MessageModels;
using ServerBusinessLogic.ReceiveModels.NotificationModels;
using ServerBusinessLogic.ReceiveModels.UserModels;
using ServerBusinessLogic.ResponseModels.ChatModels;
using ServerBusinessLogic.ResponseModels.MessageModels;
using ServerBusinessLogic.ResponseModels.UserModels;
using ServerBusinessLogic.TransmissionModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatTCPServer.Services
{
    /// <summary>
    /// Client operations synchronizer
    /// </summary>
    public class ClientsSynchronizer
    {
        /// <summary>
        /// Clients connections 
        /// <see cref="Server"/>
        /// </summary>
        private readonly List<Client> _connectedClients;

        /// <summary>
        /// Main logic of working with database tables
        /// <see cref="MainLogic"/>
        /// </summary>
        private readonly MainLogic _mainLogic;

        /// <summary>
        /// <see cref="ISerializer"/>
        /// </summary>
        private readonly ISerializer _serializer;


        public ClientsSynchronizer(List<Client> connectedClients, MainLogic mainLogic, ISerializer serializer)
        {
            _mainLogic = mainLogic;
            _connectedClients = connectedClients;
            _serializer = serializer;
        }

        /// <summary>
        /// Synchronization users messages 
        /// </summary>
        public void SynchronizeChatsMessages(MessageResponseModel message)
        {
            var chatUsers = _mainLogic.GetChatUsers(message.ChatId);
            var onlineUsers = _connectedClients.Where(ou => ou.Id != message.UserId && chatUsers.FirstOrDefault(u => u.Id == ou.Id) != null);

            Parallel.ForEach(onlineUsers, (ou) =>
            {
                var responseJson = _serializer.Serialize(new OperationResultInfo()
                {
                    ErrorInfo = string.Empty,
                    ToListener = ListenerType.ChatsMessagesListener,
                    OperationResult = OperationsResults.Successfully,
                    JsonData = _serializer.Serialize(message)
                });
                ou.SendMessage(responseJson);
            });
        }

        /// <summary>
        /// Synchronization users deleting messages
        /// </summary>
        public void SynchronizeChatsDeletingMessages(MessageReceiveModel message)
        {
            var chatUsers = _mainLogic.GetChatUsers(message.ChatId);
            var onlineUsers = _connectedClients.Where(ou => ou.Id != message.FromUserId && chatUsers.FirstOrDefault(u => u.Id == ou.Id) != null);

            Parallel.ForEach(onlineUsers, (ou) =>
            {
                var responseJson = _serializer.Serialize(new OperationResultInfo()
                {
                    ErrorInfo = string.Empty,
                    ToListener = ListenerType.ChatsMessagesDeleteListener,
                    OperationResult = OperationsResults.Successfully,
                    JsonData = _serializer.Serialize(new MessageResponseModel() 
                    {
                        Id = message.Id.Value,
                        ChatId = message.ChatId
                    })
                });
                ou.SendMessage(responseJson);
            });
        }

        /// <summary>
        /// Synchronization chats creatings
        /// </summary>
        /// <param name="chatReceiveModel"><see cref="ChatReceiveModel"/></param>
        public void SynchronizeCreatingChat(ChatResponseModel chatResponseModel)
        {
            var chatUsers = _connectedClients.Where(connClient => connClient.Id != chatResponseModel.CreatorId && chatResponseModel.ChatUsers.FirstOrDefault(cu => cu.Id == connClient.Id) != null);

            Parallel.ForEach(chatUsers, (cu) =>
            {
                var responseJson = _serializer.Serialize(new OperationResultInfo()
                {
                    ErrorInfo = string.Empty,
                    OperationResult = OperationsResults.Successfully,
                    JsonData = _serializer.Serialize(chatResponseModel),
                    ToListener = ListenerType.ChatListListener
                });

                cu.SendMessage(responseJson);
            });
        }

        /// <summary>
        /// Synchronization chats updatings
        /// </summary>
        /// <param name="chatResponseModelAfterUpdate"><see cref="ChatReceiveModel"/></param>
        public void SynchronizeUpdatingChat(ChatResponseModel chatResponseModelAfterUpdate, ChatResponseModel chatResponseModelBeforeUpdate)
        {
            var deletedUsers = chatResponseModelBeforeUpdate.ChatUsers.Where(cu => chatResponseModelAfterUpdate.ChatUsers.FirstOrDefault(newChatUser => newChatUser.Id == cu.Id) == null);
            var deletedOnlineChatUsers = _connectedClients.Where(cc => deletedUsers.FirstOrDefault(du => du.Id == cc.Id) != null);

            var responseForDeletedUsers = new OperationResultInfo()
            {
                ErrorInfo = string.Empty,
                OperationResult = OperationsResults.Successfully,
                JsonData = _serializer.Serialize(chatResponseModelAfterUpdate),
                ToListener = ListenerType.ChatListDeleteListener
            };
            var responseForDeletedUsersJson = _serializer.Serialize(responseForDeletedUsers);

            //send responses for deleted users from chat
            Parallel.ForEach(deletedOnlineChatUsers, (cu) => cu.SendMessage(responseForDeletedUsersJson));

            var onlineChatUsers = _connectedClients.Where(cc => cc.Id != chatResponseModelAfterUpdate.CreatorId 
                && chatResponseModelAfterUpdate.ChatUsers.FirstOrDefault(cu => cu.Id == cc.Id) != null);

            responseForDeletedUsers.ToListener = ListenerType.ChatListListener;
            var responseJson = _serializer.Serialize(responseForDeletedUsers);

            //send responses for users of it's chat with info about updating
            Parallel.ForEach(onlineChatUsers, (cu) => cu.SendMessage(responseJson));
        }

        /// <summary>
        /// Syncronization deleting chats 
        /// </summary>
        /// <param name="chatReceiveModel"></param>
        public void SynchronizeDeletingChat(ChatReceiveModel chatReceiveModel)
        {
            var chatUsers = _connectedClients.Where(connClient => connClient.Id != chatReceiveModel.CreatorId && chatReceiveModel.ChatUsers.FirstOrDefault(cu => cu.UserId == connClient.Id) != null);

            Parallel.ForEach(chatUsers, (cu) =>
            {
                var responseJson = _serializer.Serialize(new OperationResultInfo()
                {
                    ErrorInfo = string.Empty,
                    OperationResult = OperationsResults.Successfully,
                    JsonData = _serializer.Serialize(chatReceiveModel),
                    ToListener = ListenerType.ChatListDeleteListener
                });

                cu.SendMessage(responseJson);
            });
        }

        /// <summary>
        /// Synchronization users add to friends notifications 
        /// </summary>
        public void SynchronizeAddFriendNotifications(FriendReceiveModel friendReceiveModel)
        {
            var notification = new NotificationReceiveModel()
            {
                FromUserId = friendReceiveModel.UserId,
                ToUserId = friendReceiveModel.FriendId
            };

            var notificationDb = _mainLogic.GetNotification(notification);

            if (notificationDb == null)
            {
                notification.IsAccepted = false;
                notification.Message = $"Пользователь {_mainLogic.GetUser(new UserReceiveModel() { Id = notification.FromUserId }).UserName} хочет добавить вас в друзья";
                _mainLogic.AddNotification(notification);

                notificationDb = _mainLogic.GetNotification(notification);

                var endClient = _connectedClients.FirstOrDefault(c => c.Id == notification.ToUserId);

                if (endClient != null)
                {
                    Task.Run(() =>
                    {
                        endClient.SendMessage(_serializer.Serialize(new OperationResultInfo()
                        {
                            ErrorInfo = string.Empty,
                            OperationResult = OperationsResults.Successfully,
                            ToListener = ListenerType.NotificationListListener,
                            JsonData = _serializer.Serialize(notificationDb)
                        }));
                    });
                }
            }
        }

        //TODO : можно сделать лучше
        public void SynchronizeAddingFriend(NotificationReceiveModel notificationReceiveModel)
        {
            if (notificationReceiveModel.IsAccepted == false)
                _mainLogic.DeleteNotification(notificationReceiveModel);
            else
            {
                //binding users 
                _mainLogic.AddFriend(new FriendReceiveModel()
                {
                    UserId = notificationReceiveModel.FromUserId,
                    FriendId = notificationReceiveModel.ToUserId
                });

                _mainLogic.AddFriend(new FriendReceiveModel()
                {
                    UserId = notificationReceiveModel.ToUserId,
                    FriendId = notificationReceiveModel.FromUserId
                });

                var user1 = _mainLogic.GetUser(new UserReceiveModel() { Id = notificationReceiveModel.FromUserId });
                var user2 = _mainLogic.GetUser(new UserReceiveModel() { Id = notificationReceiveModel.ToUserId });

                if (user1 != null)
                {
                    var connectedUser1 = _connectedClients.FirstOrDefault(c => c.Id == user1.Id);
                    if (connectedUser1 != null)
                    {
                        Task.Run(() =>
                        {
                            connectedUser1.SendMessage(_serializer.Serialize(new OperationResultInfo()
                            {
                                ErrorInfo = string.Empty,
                                OperationResult = OperationsResults.Successfully,
                                ToListener = ListenerType.FriendListListener,
                                JsonData = _serializer.Serialize(new UserListResponseModel()
                                {
                                    Id = user2.Id,
                                    UserName = user2.UserName,
                                    Picture = user2.Picture
                                })
                            }));
                        });
                    }
                }

                if (user2 != null)
                {
                    var connectedUser2 = _connectedClients.FirstOrDefault(c => c.Id == user2.Id);
                    if (connectedUser2 != null)
                    {
                        Task.Run(() =>
                        {
                            connectedUser2.SendMessage(_serializer.Serialize(new OperationResultInfo()
                            {
                                ErrorInfo = string.Empty,
                                OperationResult = OperationsResults.Successfully,
                                ToListener = ListenerType.FriendListListener,
                                JsonData = _serializer.Serialize(new UserListResponseModel()
                                {
                                    Id = user1.Id,
                                    UserName = user1.UserName,
                                    Picture = user1.Picture
                                })
                            }));
                        });
                    }
                }
            }
        }

        public void SynchronizeDeletingFriend(FriendReceiveModel friendReceiveModel)
        {

        }

        /// <summary>
        /// Syncronization user online status
        /// </summary>
        public void SynchronizeOnlineStatus()
        {

        }
    }
}
