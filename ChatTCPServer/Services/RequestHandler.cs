using Microsoft.Identity.Client;
using ServerBusinessLogic.BusinessLogic;
using ServerBusinessLogic.Enums.Transmission;
using ServerBusinessLogic.Interfaces;
using ServerBusinessLogic.ReceiveModels.ChatModels;
using ServerBusinessLogic.ReceiveModels.FriendModels;
using ServerBusinessLogic.ReceiveModels.MessageModels;
using ServerBusinessLogic.ReceiveModels.UserModels;
using ServerBusinessLogic.ResponseModels.ChatModels;
using ServerBusinessLogic.ResponseModels.MessageModels;
using ServerBusinessLogic.ResponseModels.UserModels;
using ServerBusinessLogic.ReceiveModels.NotificationModels;
using ServerBusinessLogic.TransmissionModels;
using System;
using System.Collections.Generic;
using System.Management.Instrumentation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;

namespace ChatTCPServer.Services
{
    /// <summary>
    /// Request routing handler
    /// </summary>
    public class RequestHandler
    {
        private readonly ISerializer _serializer;
        private readonly MainLogic _mainLogic;
        private readonly Client _client;
        private readonly ClientsSynchronizer _clientsSynchronizer;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serializer">
        /// <see cref="ISerializer"/>
        /// <seealso cref="Serializer"/>
        /// </param>
        /// <param name="clientsSynchronizer"><see cref="ClientsSynchronizer"/></param>
        /// <param name="mainLogic"><see cref="MainLogic"/></param>
        /// <param name="client"><see cref="Client"/></param>
        public RequestHandler(
            ISerializer serializer, 
            Client client,
            MainLogic mainLogic,
            List<Client> connectedClients)
        {
            _client = client;
            _mainLogic = mainLogic;
            _serializer = serializer;
            _clientsSynchronizer = new ClientsSynchronizer(connectedClients, mainLogic, serializer);
        }

        /// <summary>
        /// Distribution of the received data across database services and sending the operation result
        /// and synchronizing another users chats
        /// </summary>
        /// <param name="clientOperationMessage"></param>
        public void HandleRequest(string messageJson)
        {
            var message = _serializer.Deserialize<ClientOperationMessage>(messageJson);

            //TODO : наверняка можно сделать лучше
            //switch will convert by compiler to hashtable
            switch(message.Operation)
            {
                case ClientOperations.Authorization:
                    var userReceiveModelAuthorization = _serializer.Deserialize<UserReceiveModel>(message.JsonData);
                    var authorizationResult = _mainLogic.UserAuthorization(userReceiveModelAuthorization);

                    if(authorizationResult.OperationResult == OperationsResults.Successfully)
                    {
                        _client.Id = (authorizationResult.JsonData as UserResponseModel).Id;
                        Console.WriteLine(_client.Id + " Пользователь успешно авторизировался");
                    }

                    authorizationResult.JsonData = _serializer.Serialize(authorizationResult.JsonData as UserResponseModel);

                    var authorizationResultJson = _serializer.Serialize(authorizationResult);
                    _client.SendMessage(authorizationResultJson);
                    break;

                case ClientOperations.Registration:
                    var userReceiveModelRegistration = _serializer.Deserialize<UserReceiveModel>(message.JsonData);
                    var registrationResult = _serializer.Serialize(_mainLogic.UserRegistration(userReceiveModelRegistration));
                    _client.SendMessage(registrationResult);
                    break;

//---

                case ClientOperations.UpdateProfile:
                    var userReceiveModelUpdateProfile = _serializer.Deserialize<UserReceiveModel>(message.JsonData);
                    var updateProfileResult = _serializer.Serialize(_mainLogic.UserProfileUpdate(userReceiveModelUpdateProfile));
                    _client.SendMessage(updateProfileResult);
                    break;

                case ClientOperations.GetUsers:
                    var userPaginationReceiveModel = _serializer.Deserialize<UserPaginationReceiveModel>(message.JsonData);
                    var getUsersResult = _mainLogic.GetPageOfUsers(userPaginationReceiveModel);

                    getUsersResult.JsonData = _serializer.Serialize(getUsersResult.JsonData as List<UserListResponseModel>);
                    var getUsersResultJson = _serializer.Serialize(getUsersResult);

                    _client.SendMessage(getUsersResultJson);
                    break;

                case ClientOperations.SendMessage:
                    var messageReceiveModelSend = _serializer.Deserialize<MessageReceiveModel>(message.JsonData);
                    var messageSendResult = _mainLogic.AddMessage(messageReceiveModelSend);

                    if (messageSendResult.JsonData != null)
                    {
                        var messageResponseModel = messageSendResult.JsonData as MessageResponseModel;
                        _clientsSynchronizer.SynchronizeChatsMessages(messageResponseModel);
                        messageSendResult.JsonData = _serializer.Serialize(messageResponseModel);
                    }

                    _client.SendMessage(_serializer.Serialize(messageSendResult));
                    break;

                case ClientOperations.CreateChat:
                    var chatReceiveModelCreate = _serializer.Deserialize<ChatReceiveModel>(message.JsonData);
                    var chatCreateResult = _mainLogic.ChatCreate(chatReceiveModelCreate);

                    if (chatCreateResult.JsonData != null)
                    {
                        var chatResponseModel = chatCreateResult.JsonData as ChatResponseModel;
                        _clientsSynchronizer.SynchronizeCreatingChat(chatResponseModel);
                        chatCreateResult.JsonData = _serializer.Serialize(chatResponseModel);
                    }
                    _client.SendMessage(_serializer.Serialize(chatCreateResult));
                    break;

                case ClientOperations.UpdateChat:
                    var chatReceiveModelUpdate = _serializer.Deserialize<ChatReceiveModel>(message.JsonData);
                    var chatBeforeUpdate = _mainLogic.GetChat(chatReceiveModelUpdate);
                    var chatUpdateResult = _mainLogic.ChatUpdate(chatReceiveModelUpdate);

                    if(chatUpdateResult.JsonData != null)
                    {
                        var chatResponseModelAfterUpdate = chatUpdateResult.JsonData as ChatResponseModel;
                        _clientsSynchronizer.SynchronizeUpdatingChat(chatResponseModelAfterUpdate, chatBeforeUpdate);
                        chatUpdateResult.JsonData = _serializer.Serialize(chatResponseModelAfterUpdate);
                    }
                    _client.SendMessage(_serializer.Serialize(chatUpdateResult));
                    break;

                case ClientOperations.DeleteChat:
                    var chatReceiveModelDelete = _serializer.Deserialize<ChatReceiveModel>(message.JsonData);
                    var chatDeleteResult = _mainLogic.ChatDelete(chatReceiveModelDelete);
                    _client.SendMessage(_serializer.Serialize(chatDeleteResult));

                    if (chatDeleteResult.OperationResult == OperationsResults.Successfully)
                        _clientsSynchronizer.SynchronizeDeletingChat(chatReceiveModelDelete);
                    break;

                case ClientOperations.GetChats:
                    var userPaginationModetGetChats = _serializer.Deserialize<UserPaginationReceiveModel>(message.JsonData);
                    var getChatsResult = _mainLogic.GetPageOfChats(userPaginationModetGetChats);
                    getChatsResult.JsonData = _serializer.Serialize(getChatsResult.JsonData as List<ChatResponseModel>);
                    _client.SendMessage(_serializer.Serialize(getChatsResult));
                    break;
//---
                case ClientOperations.UpdateMessage:
                    var messageReceiveModelUpdate = _serializer.Deserialize<MessageReceiveModel>(message.JsonData);
                    var updateMessageResult = _mainLogic.UpdateMessage(messageReceiveModelUpdate);

                    if(updateMessageResult.JsonData != null)
                    {
                        var messageResponseModel = updateMessageResult.JsonData as MessageResponseModel;
                        _clientsSynchronizer.SynchronizeChatsMessages(messageResponseModel);
                        updateMessageResult.JsonData = _serializer.Serialize(messageResponseModel);
                    }
                    _client.SendMessage(_serializer.Serialize(updateMessageResult));
                    break;

                case ClientOperations.DeleteMessage:
                    var messageReceiveModelDelete = _serializer.Deserialize<MessageReceiveModel>(message.JsonData);
                    var deleteMessageResult = _mainLogic.DeleteMessage(messageReceiveModelDelete);
                    _client.SendMessage(_serializer.Serialize(deleteMessageResult));

                    if (deleteMessageResult.OperationResult == OperationsResults.Successfully)
                        _clientsSynchronizer.SynchronizeChatsDeletingMessages(messageReceiveModelDelete);
                    break;

                case ClientOperations.GetMessages:
                    var chatPaginationReceiveMessageGet = _serializer.Deserialize<ChatPaginationReceiveModel>(message.JsonData);
                    var getChatMessagesResult = _mainLogic.GetChatMessages(chatPaginationReceiveMessageGet);
                    getChatMessagesResult.JsonData = _serializer.Serialize(getChatMessagesResult.JsonData as List<MessageResponseModel>);
                    _client.SendMessage(_serializer.Serialize(getChatMessagesResult));
                    break;

                case ClientOperations.AddFriend:
                    var friendReceiveModelAdd = _serializer.Deserialize<FriendReceiveModel>(message.JsonData);
                    _clientsSynchronizer.SynchronizeAddFriendNotifications(friendReceiveModelAdd);
                    break;

                case ClientOperations.DeleteFriend:
                    var friendReceiveModelDelete = _serializer.Deserialize<FriendReceiveModel>(message.JsonData);
                    var friendDeleteResult = _mainLogic.DeleteFriend(friendReceiveModelDelete);
                    _client.SendMessage(_serializer.Serialize(friendDeleteResult));

                    if(friendDeleteResult.OperationResult == OperationsResults.Successfully)
                        _clientsSynchronizer.SynchronizeDeletingFriend(friendReceiveModelDelete);
                    break;

                case ClientOperations.GetFriends:
                    var userPaginationReceiveModelGetFriends = _serializer.Deserialize<UserPaginationReceiveModel>(message.JsonData);
                    var getFriendsResult = _mainLogic.GetFriendsPage(userPaginationReceiveModelGetFriends);
                    getFriendsResult.JsonData = _serializer.Serialize(getFriendsResult.JsonData as List<UserListResponseModel>);
                    _client.SendMessage(_serializer.Serialize(getFriendsResult));
                    break;

                case ClientOperations.UpdateNotification:
                    var notificationReceiveModelUpdate = _serializer.Deserialize<NotificationReceiveModel>(message.JsonData);
                    var notificationUpdateResult = _mainLogic.UpdateNotification(notificationReceiveModelUpdate);
                    _client.SendMessage(_serializer.Serialize(notificationUpdateResult));

                    if(notificationUpdateResult.OperationResult == OperationsResults.Successfully)
                        _clientsSynchronizer.SynchronizeAddingFriend(notificationReceiveModelUpdate);
                    break;

                default:
                    _client.SendMessage(_serializer.Serialize(new OperationResultInfo()
                    {
                        ErrorInfo = "Незарегистрированная операция",
                        OperationResult = OperationsResults.Unsuccessfully,
                        JsonData = null
                    }));
                    break;
            }
        }
    }
}
