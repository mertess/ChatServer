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
using ServerBusinessLogic.ResponseModels.NotificationModels;
using ServerBusinessLogic.ResponseModels.UserModels;
using ServerBusinessLogic.TransmissionModels;
using ServerDatabaseSystem.Implementation;
using System;
using System.Collections.Generic;

namespace ChatTCPServer.Services
{
    /// <summary>
    /// Request routing handler
    /// </summary>
    public class RequestHandler
    {
        private readonly ISerializer<string> _jsonStringSerializer;
        private readonly ISerializer<byte[]> _jsonBinarySerializer;
        private readonly MainLogic _mainLogic;
        private readonly Client _client;
        private readonly ClientsSynchronizer _clientsSynchronizer;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="jsonStringSerializer">
        /// <see cref="ISerializer"/>
        /// <seealso cref="JsonStringSerializer"/>
        /// </param>
        /// <param name="clientsSynchronizer"><see cref="ClientsSynchronizer"/></param>
        /// <param name="mainLogic"><see cref="MainLogic"/></param>
        /// <param name="client"><see cref="Client"/></param>
        public RequestHandler(
            ISerializer<string> jsonStringSerializer,
            ISerializer<byte[]> jsonBinarySerializer,
            Client client,
            List<Client> connectedClients)
        {
            _client = client;
            _mainLogic = new MainLogic(
                    new ChatLogic(),
                    new UserLogic(),
                    new MessageLogic(),
                    new FriendLogic(),
                    new NotificationLogic()
                );
            _jsonStringSerializer = jsonStringSerializer;
            _jsonBinarySerializer = jsonBinarySerializer;
            _clientsSynchronizer = new ClientsSynchronizer(connectedClients, _mainLogic, jsonStringSerializer);
        }

        /// <summary>
        /// Distribution of the received data across database services and sending the operation result
        /// and synchronizing another users chats
        /// </summary>
        /// <param name="clientOperationMessage"></param>
        public void HandleRequest(string messageJson)
        {
            var message = _jsonStringSerializer.Deserialize<ClientOperationMessage>(messageJson);

            //TODO : наверняка можно сделать лучше
            //switch will convert by compiler to hashtable
            if (message != null)
            {
                switch (message.Operation)
                {
                    case ClientOperations.Authorization:
                        var userReceiveModelAuthorization = _jsonStringSerializer.Deserialize<UserReceiveModel>(message.JsonData);
                        var authorizationResult = _mainLogic.GetUser(userReceiveModelAuthorization, true);
                        authorizationResult.ToListener = ListenerType.AuthorizationListener;

                        if (authorizationResult.OperationResult == OperationsResults.Successfully)
                        {
                            _client.Id = (authorizationResult.JsonData as UserResponseModel).Id;
                            Console.WriteLine(_client.Id + " Пользователь успешно авторизировался");

                            _clientsSynchronizer.SynchronizeOnlineStatus(authorizationResult.JsonData as UserResponseModel);
                            authorizationResult.JsonData = _jsonStringSerializer.Serialize(authorizationResult.JsonData as UserResponseModel);
                        }

                        var authorizationResultJson = _jsonStringSerializer.Serialize(authorizationResult);
                        _client.SendMessage(authorizationResultJson);
                        break;

                    case ClientOperations.Registration:
                        var userReceiveModelRegistration = _jsonStringSerializer.Deserialize<UserReceiveModel>(message.JsonData);
                        var registrationResult = _jsonStringSerializer.Serialize(_mainLogic.UserRegistration(userReceiveModelRegistration));
                        _client.SendMessage(registrationResult);
                        break;

                    //---

                    case ClientOperations.UpdateProfile:
                        var userReceiveModelUpdateProfile = _jsonStringSerializer.Deserialize<UserReceiveModel>(message.JsonData);
                        var updateProfileResult = _jsonStringSerializer.Serialize(_mainLogic.UserProfileUpdate(userReceiveModelUpdateProfile));
                        _client.SendMessage(updateProfileResult);
                        break;

                    case ClientOperations.GetUsers:
                        var userPaginationReceiveModel = _jsonStringSerializer.Deserialize<UserPaginationReceiveModel>(message.JsonData);
                        var getUsersResult = _mainLogic.GetPageOfUsers(userPaginationReceiveModel);

                        getUsersResult.JsonData = _jsonStringSerializer.Serialize(getUsersResult.JsonData as List<UserListResponseModel>);
                        var getUsersResultJson = _jsonStringSerializer.Serialize(getUsersResult);

                        _client.SendMessage(getUsersResultJson);
                        break;

                    case ClientOperations.SendMessage:
                        var messageReceiveModelSend = _jsonStringSerializer.Deserialize<MessageReceiveModel>(message.JsonData);
                        var messageSendResult = _mainLogic.AddMessage(messageReceiveModelSend);

                        if (messageSendResult.JsonData != null)
                        {
                            var messageResponseModel = messageSendResult.JsonData as MessageResponseModel;
                            _clientsSynchronizer.SynchronizeChatsMessages(messageResponseModel);
                            messageSendResult.JsonData = _jsonStringSerializer.Serialize(messageResponseModel);
                        }

                        _client.SendMessage(_jsonStringSerializer.Serialize(messageSendResult));
                        break;

                    case ClientOperations.CreateChat:
                        var chatReceiveModelCreate = _jsonStringSerializer.Deserialize<ChatReceiveModel>(message.JsonData);
                        var chatCreateResult = _mainLogic.ChatCreate(chatReceiveModelCreate);

                        if (chatCreateResult.JsonData != null)
                        {
                            var chatResponseModel = chatCreateResult.JsonData as ChatResponseModel;
                            _clientsSynchronizer.SynchronizeCreatingChat(chatResponseModel);
                            chatCreateResult.JsonData = _jsonStringSerializer.Serialize(chatResponseModel);
                        }
                        _client.SendMessage(_jsonStringSerializer.Serialize(chatCreateResult));
                        break;

                    case ClientOperations.UpdateChat:
                        var chatReceiveModelUpdate = _jsonStringSerializer.Deserialize<ChatReceiveModel>(message.JsonData);
                        var chatBeforeUpdate = _mainLogic.GetChat(chatReceiveModelUpdate);
                        var chatUpdateResult = _mainLogic.ChatUpdate(chatReceiveModelUpdate);

                        if (chatUpdateResult.JsonData != null)
                        {
                            var chatResponseModelAfterUpdate = chatUpdateResult.JsonData as ChatResponseModel;
                            _clientsSynchronizer.SynchronizeUpdatingChat(chatResponseModelAfterUpdate, chatBeforeUpdate);
                            chatUpdateResult.JsonData = _jsonStringSerializer.Serialize(chatResponseModelAfterUpdate);
                        }
                        _client.SendMessage(_jsonStringSerializer.Serialize(chatUpdateResult));
                        break;

                    case ClientOperations.DeleteChat:
                        var chatReceiveModelDelete = _jsonStringSerializer.Deserialize<ChatReceiveModel>(message.JsonData);
                        var chatDeleteResult = _mainLogic.ChatDelete(chatReceiveModelDelete);
                        _client.SendMessage(_jsonStringSerializer.Serialize(chatDeleteResult));

                        if (chatDeleteResult.OperationResult == OperationsResults.Successfully)
                            _clientsSynchronizer.SynchronizeDeletingChat(chatReceiveModelDelete);
                        break;

                    case ClientOperations.GetChats:
                        var userPaginationModetGetChats = _jsonStringSerializer.Deserialize<UserPaginationReceiveModel>(message.JsonData);
                        var getChatsResult = _mainLogic.GetPageOfChats(userPaginationModetGetChats);
                        getChatsResult.JsonData = _jsonStringSerializer.Serialize(getChatsResult.JsonData as List<ChatResponseModel>);
                        _client.SendMessage(_jsonStringSerializer.Serialize(getChatsResult));
                        break;
                    //---
                    case ClientOperations.UpdateMessage:
                        var messageReceiveModelUpdate = _jsonStringSerializer.Deserialize<MessageReceiveModel>(message.JsonData);
                        var updateMessageResult = _mainLogic.UpdateMessage(messageReceiveModelUpdate);

                        if (updateMessageResult.JsonData != null)
                        {
                            var messageResponseModel = updateMessageResult.JsonData as MessageResponseModel;
                            _clientsSynchronizer.SynchronizeChatsMessages(messageResponseModel);
                            updateMessageResult.JsonData = _jsonStringSerializer.Serialize(messageResponseModel);
                        }
                        _client.SendMessage(_jsonStringSerializer.Serialize(updateMessageResult));
                        break;

                    case ClientOperations.DeleteMessage:
                        var messageReceiveModelDelete = _jsonStringSerializer.Deserialize<MessageReceiveModel>(message.JsonData);
                        var deleteMessageResult = _mainLogic.DeleteMessage(messageReceiveModelDelete);
                        _client.SendMessage(_jsonStringSerializer.Serialize(deleteMessageResult));

                        if (deleteMessageResult.OperationResult == OperationsResults.Successfully)
                            _clientsSynchronizer.SynchronizeChatsDeletingMessages(messageReceiveModelDelete);
                        break;

                    case ClientOperations.GetMessages:
                        var chatPaginationReceiveMessageGet = _jsonStringSerializer.Deserialize<ChatPaginationReceiveModel>(message.JsonData);
                        var getChatMessagesResult = _mainLogic.GetChatMessages(chatPaginationReceiveMessageGet);
                        getChatMessagesResult.JsonData = _jsonStringSerializer.Serialize(getChatMessagesResult.JsonData as List<MessageResponseModel>);
                        _client.SendMessage(_jsonStringSerializer.Serialize(getChatMessagesResult));
                        break;

                    case ClientOperations.AddFriend:
                        var friendReceiveModelAdd = _jsonStringSerializer.Deserialize<FriendReceiveModel>(message.JsonData);
                        _clientsSynchronizer.SynchronizeAddFriendNotifications(friendReceiveModelAdd);
                        break;

                    case ClientOperations.DeleteFriend:
                        var friendReceiveModelDelete = _jsonStringSerializer.Deserialize<FriendReceiveModel>(message.JsonData);
                        var friendDeleteResult = _mainLogic.DeleteFriend(friendReceiveModelDelete);
                        _client.SendMessage(_jsonStringSerializer.Serialize(friendDeleteResult));

                        if (friendDeleteResult.OperationResult == OperationsResults.Successfully)
                            _clientsSynchronizer.SynchronizeDeletingFriend(friendReceiveModelDelete);
                        break;

                    case ClientOperations.GetFriends:
                        var userPaginationReceiveModelGetFriends = _jsonStringSerializer.Deserialize<UserPaginationReceiveModel>(message.JsonData);
                        var getFriendsResult = _mainLogic.GetFriendsPage(userPaginationReceiveModelGetFriends);
                        getFriendsResult.JsonData = _jsonStringSerializer.Serialize(getFriendsResult.JsonData as List<UserListResponseModel>);
                        _client.SendMessage(_jsonStringSerializer.Serialize(getFriendsResult));
                        break;

                    case ClientOperations.UpdateNotification:
                        var notificationReceiveModelUpdate = _jsonStringSerializer.Deserialize<NotificationReceiveModel>(message.JsonData);
                        var notificationUpdateResult = _mainLogic.UpdateNotification(notificationReceiveModelUpdate);
                        _client.SendMessage(_jsonStringSerializer.Serialize(notificationUpdateResult));

                        if (notificationUpdateResult.OperationResult == OperationsResults.Successfully)
                            _clientsSynchronizer.SynchronizeAddingFriend(notificationReceiveModelUpdate);
                        break;
                    case ClientOperations.GetNotifications:
                        var notificationReceiveModelGetPage = _jsonStringSerializer.Deserialize<UserPaginationReceiveModel>(message.JsonData);
                        var getNotificationPageResult = _mainLogic.GetNotificationsPage(notificationReceiveModelGetPage);

                        if (getNotificationPageResult.JsonData != null)
                            getNotificationPageResult.JsonData = _jsonStringSerializer.Serialize(getNotificationPageResult.JsonData as List<NotificationResponseModel>);

                        _client.SendMessage(_jsonStringSerializer.Serialize(getNotificationPageResult));
                        break;
                    case ClientOperations.GetUser:
                        var userReceiveModelGetUser = _jsonStringSerializer.Deserialize<UserReceiveModel>(message.JsonData);
                        var getUserResult = _mainLogic.GetUser(userReceiveModelGetUser);

                        if (getUserResult.JsonData != null)
                            getUserResult.JsonData = _jsonStringSerializer.Serialize(getUserResult.JsonData as UserResponseModel);

                        _client.SendMessage(_jsonStringSerializer.Serialize(getUserResult));
                        break;
                    default:
                        _client.SendMessage(_jsonStringSerializer.Serialize(new OperationResultInfo()
                        {
                            ErrorInfo = "Незарегистрированная операция",
                            OperationResult = OperationsResults.Unsuccessfully,
                            JsonData = null
                        }));
                        break;
                }
            }
            else
            {
                Console.WriteLine("Ошибка целостности полученных данных");
            }
        }

        public void HandleDisconnect() => _clientsSynchronizer.SynchronizeOfflineStatus(_client);
    }
}
