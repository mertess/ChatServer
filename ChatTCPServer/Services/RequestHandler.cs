using NLog;
using ServerBusinessLogic.BusinessLogic;
using ServerBusinessLogic.Enums;
using ServerBusinessLogic.Enums.Transmission;
using ServerBusinessLogic.Interfaces;
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
using ServerDatabaseSystem.Implementation;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace ChatTCPServer.Services
{
    /// <summary>
    /// Request routing handler
    /// </summary>
    public class RequestHandler
    {
        private readonly ISerializer<string> _jsonStringSerializer;

        private readonly MainLogic _mainLogic;

        private readonly Client _client;

        private ClientsSynchronizer _clientsSynchronizer;

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private IEncoder _encoder;

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
            _encoder = new Encoder();
            _clientsSynchronizer = new ClientsSynchronizer(connectedClients, _mainLogic, jsonStringSerializer, _encoder);
        }

        /// <summary>
        /// Distribution of the received data across database services and sending the operation result
        /// and synchronizing another users chats
        /// </summary>
        /// <param name="clientOperationMessage"></param>
        public void HandleRequest(byte[] byteMessage)
        {

            var stringDecryptedMessage = _encoder.Decryption(byteMessage);
            Console.WriteLine("received decrypted message : " + stringDecryptedMessage);
            _logger.Info($"Received decrypted message from user {_client.Id} : {stringDecryptedMessage}");
            var message = _jsonStringSerializer.Deserialize<ClientOperationMessage>(stringDecryptedMessage);


            //TODO : наверняка можно сделать лучше
            //switch will convert by compiler to hashtable
            if (message != null)
            {
                _logger.Info($"Getted correct message from user {_client.Id}");
                switch (message.Operation)
                {
                    case ClientOperations.Authorization:
                        var userReceiveModelAuthorization = _jsonStringSerializer.Deserialize<UserReceiveModel>(message.JsonData);
                        var authorizationResult = _mainLogic.GetUser(userReceiveModelAuthorization, true);
                        authorizationResult.ToListener = ListenerType.AuthorizationListener;

                        if (authorizationResult.OperationResult == OperationsResults.Successfully)
                        {
                            _client.Id = (authorizationResult.JsonData as UserResponseModel).UserId;
                            Console.WriteLine(_client.Id + " Пользователь успешно авторизировался");

                            _logger.Info($"User {_client.Id} authorization is successfully");

                            _clientsSynchronizer.SynchronizeOnlineStatus(authorizationResult.JsonData as UserResponseModel);
                            authorizationResult.JsonData = _jsonStringSerializer.Serialize(authorizationResult.JsonData as UserResponseModel);
                        }
                        else
                            _logger.Info($"User {_client.Id} authorization is unseccessfully");

                        var authorizationResultJson = _jsonStringSerializer.Serialize(authorizationResult);
                        _client.SendMessage(_encoder.Encryption(authorizationResultJson));
                        //_client.SendMessage(authorizationResultJson);
                        break;

                    case ClientOperations.Registration:
                        var userReceiveModelRegistration = _jsonStringSerializer.Deserialize<UserReceiveModel>(message.JsonData);
                        var registrationResult = _mainLogic.UserRegistration(userReceiveModelRegistration);
                        var registrationResultJson = _jsonStringSerializer.Serialize(registrationResult);

                        _logger.Info($"User registration - login: {userReceiveModelRegistration.Login} " +
                            $" password: {userReceiveModelRegistration.Password} " +
                            $"result: {Enum.GetName(typeof(OperationsResults), registrationResult.OperationResult)} " +
                            $"Error info : {registrationResult.ErrorInfo}");

                        _client.SendMessage(_encoder.Encryption(registrationResultJson));
                        //_client.SendMessage(registrationResultJson);
                        break;

                    case ClientOperations.UpdateProfile:
                        var userReceiveModelUpdateProfile = _jsonStringSerializer.Deserialize<UserReceiveModel>(message.JsonData);
                        var updateProfileResult = _mainLogic.UserProfileUpdate(userReceiveModelUpdateProfile);
                        var updateProfileResultJson = _jsonStringSerializer.Serialize(updateProfileResult);

                        _logger.Info($"User {_client.Id} - username: {userReceiveModelUpdateProfile.UserName} " +
                            $" password: {userReceiveModelUpdateProfile.Password} " +
                            $" name: {userReceiveModelUpdateProfile.Name} " +
                            $" secondname: {userReceiveModelUpdateProfile.SecondName} " +
                            $" gender: {userReceiveModelUpdateProfile.Gender} " +
                            $" phoneNumber: {userReceiveModelUpdateProfile.PhoneNumber} " +
                            $" country: {userReceiveModelUpdateProfile.Country} " +
                            $" city: {userReceiveModelUpdateProfile.City} " +
                            $" name: {userReceiveModelUpdateProfile.Name} " +
                            $" result: {Enum.GetName(typeof(OperationsResults), updateProfileResult.OperationResult)}" +
                            $" Error info : {updateProfileResult.ErrorInfo}");

                        _client.SendMessage(_encoder.Encryption(updateProfileResultJson));
                        //_client.SendMessage(updateProfileResultJson);
                        break;

                    case ClientOperations.GetUsers:
                        var userPaginationReceiveModel = _jsonStringSerializer.Deserialize<UserPaginationReceiveModel>(message.JsonData);
                        var getUsersResult = _mainLogic.GetPageOfUsers(userPaginationReceiveModel);

                        _logger.Info($"User {_client.Id} has get users " +
                            $"result: {Enum.GetName(typeof(OperationsResults), getUsersResult.OperationResult)} " +
                            $"error info: {getUsersResult.ErrorInfo}");

                        getUsersResult.JsonData = _jsonStringSerializer.Serialize(getUsersResult.JsonData as List<UserListResponseModel>);
                        var getUsersResultJson = _jsonStringSerializer.Serialize(getUsersResult);

                        _client.SendMessage(_encoder.Encryption(getUsersResultJson));
                        //_client.SendMessage(getUsersResultJson);
                        break;

                    case ClientOperations.SendMessage:
                        var messageReceiveModelSend = _jsonStringSerializer.Deserialize<MessageReceiveModel>(message.JsonData);
                        var messageSendResult = _mainLogic.AddMessage(messageReceiveModelSend);

                        if (messageSendResult.JsonData != null)
                        {

                            var messageResponseModel = messageSendResult.JsonData as MessageResponseModel;

                            _logger.Info($"User {_client.Id} has send message " +
                                $"chat id: {messageResponseModel.ChatId} " +
                                $"message: {messageResponseModel.UserMassage}");

                            _clientsSynchronizer.SynchronizeChatsMessages(messageResponseModel);
                            messageSendResult.JsonData = _jsonStringSerializer.Serialize(messageResponseModel);
                        }
                        else
                            _logger.Info($"User {_client.Id} has not send message " +
                                $"result: {Enum.GetName(typeof(OperationsResults), messageSendResult.OperationResult)} " +
                                $"error info: {messageSendResult.ErrorInfo}");

                        _client.SendMessage(_encoder.Encryption(_jsonStringSerializer.Serialize(messageSendResult)));
                        //_client.SendMessage(_jsonStringSerializer.Serialize(messageSendResult));
                        break;

                    case ClientOperations.CreateChat:
                        var chatReceiveModelCreate = _jsonStringSerializer.Deserialize<ChatReceiveModel>(message.JsonData);
                        var chatCreateResult = _mainLogic.ChatCreate(chatReceiveModelCreate);

                        if (chatCreateResult.JsonData != null)
                        {
                            var chatResponseModel = chatCreateResult.JsonData as ChatResponseModel;

                            _logger.Info($"User {_client.Id} has create chat " +
                                $"chat id: {chatResponseModel.Id} " +
                                $"creator id: {chatResponseModel.CreatorId} " +
                                $"chatname: {chatResponseModel.ChatName} " +
                                $"message: {chatResponseModel.CountUsers}");

                            _clientsSynchronizer.SynchronizeCreatingChat(chatResponseModel);
                            chatCreateResult.JsonData = _jsonStringSerializer.Serialize(chatResponseModel);
                        }
                        else
                            _logger.Info($"User {_client.Id} has not create chat " +
                                $"result: {Enum.GetName(typeof(OperationsResults), chatCreateResult.OperationResult)} " +
                                $"error info: {chatCreateResult.ErrorInfo}");

                        _client.SendMessage(_encoder.Encryption(_jsonStringSerializer.Serialize(chatCreateResult)));
                        //_client.SendMessage(_jsonStringSerializer.Serialize(chatCreateResult));
                        break;

                    case ClientOperations.UpdateChat:
                        var chatReceiveModelUpdate = _jsonStringSerializer.Deserialize<ChatReceiveModel>(message.JsonData);
                        var chatBeforeUpdate = _mainLogic.GetChat(chatReceiveModelUpdate);
                        var chatUpdateResult = _mainLogic.ChatUpdate(chatReceiveModelUpdate);

                        if (chatUpdateResult.JsonData != null)
                        {
                            var chatResponseModelAfterUpdate = chatUpdateResult.JsonData as ChatResponseModel;

                            _logger.Info($"User {_client.Id} has update chat " +
                                $"chat id: {chatResponseModelAfterUpdate.Id} " +
                                $"creator id: {chatResponseModelAfterUpdate.CreatorId} " +
                                $"chatname: {chatResponseModelAfterUpdate.ChatName} " +
                                $"count users: {chatResponseModelAfterUpdate.CountUsers}");

                            if (chatBeforeUpdate.CountUsers == 2 && chatResponseModelAfterUpdate.CountUsers > 2)
                                _clientsSynchronizer.SynchronizeCreatingChat(chatResponseModelAfterUpdate);
                            else
                                _clientsSynchronizer.SynchronizeUpdatingChat(chatResponseModelAfterUpdate, chatBeforeUpdate);

                            if (chatResponseModelAfterUpdate.CreatorId != _client.Id)
                                chatUpdateResult.JsonData = null;
                            else
                                chatUpdateResult.JsonData = _jsonStringSerializer.Serialize(chatResponseModelAfterUpdate);
                        }
                        else
                            _logger.Info($"User {_client.Id} has not update chat " +
                                $"result: {Enum.GetName(typeof(OperationsResults), chatUpdateResult.OperationResult)} " +
                                $"error info: {chatUpdateResult.ErrorInfo}");

                        _client.SendMessage(_encoder.Encryption(_jsonStringSerializer.Serialize(chatUpdateResult)));
                        //_client.SendMessage(_jsonStringSerializer.Serialize(chatUpdateResult));
                        break;

                    case ClientOperations.DeleteChat:
                        var chatReceiveModelDelete = _jsonStringSerializer.Deserialize<ChatReceiveModel>(message.JsonData);
                        var chatDeleteResult = _mainLogic.ChatDelete(chatReceiveModelDelete);

                        _client.SendMessage(_encoder.Encryption(_jsonStringSerializer.Serialize(chatDeleteResult)));
                        //_client.SendMessage(_jsonStringSerializer.Serialize(chatDeleteResult));

                        _logger.Info($"User {_client.Id} has delete chat " +
                                $"result: {Enum.GetName(typeof(OperationsResults), chatDeleteResult.OperationResult)} " +
                                $"error info: {chatDeleteResult.ErrorInfo}");

                        if (chatDeleteResult.OperationResult == OperationsResults.Successfully)
                            _clientsSynchronizer.SynchronizeDeletingChat(chatReceiveModelDelete);
                        break;

                    case ClientOperations.GetChats:
                        var userPaginationModetGetChats = _jsonStringSerializer.Deserialize<UserPaginationReceiveModel>(message.JsonData);
                        var getChatsResult = _mainLogic.GetPageOfChats(userPaginationModetGetChats);

                        _logger.Info($"User {_client.Id} has get chats " +
                                $"result: {Enum.GetName(typeof(OperationsResults), getChatsResult.OperationResult)} " +
                                $"error info: {getChatsResult.ErrorInfo}");

                        getChatsResult.JsonData = _jsonStringSerializer.Serialize(getChatsResult.JsonData as List<ChatResponseModel>);
                        _client.SendMessage(_encoder.Encryption(_jsonStringSerializer.Serialize(getChatsResult)));
                        //_client.SendMessage(_jsonStringSerializer.Serialize(getChatsResult));
                        break;
                    //---
                    case ClientOperations.UpdateMessage:
                        var messageReceiveModelUpdate = _jsonStringSerializer.Deserialize<MessageReceiveModel>(message.JsonData);
                        var updateMessageResult = _mainLogic.UpdateMessage(messageReceiveModelUpdate);

                        if (updateMessageResult.JsonData != null)
                        {
                            var messageResponseModel = updateMessageResult.JsonData as MessageResponseModel;

                            _logger.Info($"User {_client.Id} has update message " +
                                $"message id: {messageResponseModel.Id} " +
                                $"user id: {messageResponseModel.UserId} " +
                                $"message: {messageResponseModel.UserMassage}");

                            _clientsSynchronizer.SynchronizeChatsMessages(messageResponseModel);
                            updateMessageResult.JsonData = _jsonStringSerializer.Serialize(messageResponseModel);
                        }
                        else
                            _logger.Info($"User {_client.Id} has not update message " +
                                $"result: {Enum.GetName(typeof(OperationsResults), updateMessageResult.OperationResult)} " +
                                $"error info: {updateMessageResult.ErrorInfo}");

                        _client.SendMessage(_encoder.Encryption(_jsonStringSerializer.Serialize(updateMessageResult)));
                        //_client.SendMessage(_jsonStringSerializer.Serialize(updateMessageResult));
                        break;

                    case ClientOperations.DeleteMessage:
                        var messageReceiveModelDelete = _jsonStringSerializer.Deserialize<MessageReceiveModel>(message.JsonData);
                        var deleteMessageResult = _mainLogic.DeleteMessage(messageReceiveModelDelete);
                        _client.SendMessage(_encoder.Encryption(_jsonStringSerializer.Serialize(deleteMessageResult)));
                        //_client.SendMessage(_jsonStringSerializer.Serialize(deleteMessageResult));

                        _logger.Info($"User {_client.Id} has delete message " +
                                $"message id: {messageReceiveModelDelete.Id} " +
                                $"chat id: {messageReceiveModelDelete.ChatId} " +
                                $"message: {messageReceiveModelDelete.UserMassage} " +
                                $"result: {Enum.GetName(typeof(OperationsResults), deleteMessageResult.OperationResult)} " +
                                $"error info: {deleteMessageResult.ErrorInfo}");

                        if (deleteMessageResult.OperationResult == OperationsResults.Successfully)
                            _clientsSynchronizer.SynchronizeChatsDeletingMessages(messageReceiveModelDelete);
                        break;

                    case ClientOperations.GetMessages:
                        var chatPaginationReceiveMessageGet = _jsonStringSerializer.Deserialize<ChatPaginationReceiveModel>(message.JsonData);
                        var getChatMessagesResult = _mainLogic.GetChatMessages(chatPaginationReceiveMessageGet);

                        _logger.Info($"User {_client.Id} has get messages " +
                               $"result: {Enum.GetName(typeof(OperationsResults), getChatMessagesResult.OperationResult)} " +
                               $"error info: {getChatMessagesResult.ErrorInfo}");

                        getChatMessagesResult.JsonData = _jsonStringSerializer.Serialize(getChatMessagesResult.JsonData as List<MessageResponseModel>);
                        _client.SendMessage(_encoder.Encryption(_jsonStringSerializer.Serialize(getChatMessagesResult)));
                        //_client.SendMessage(_jsonStringSerializer.Serialize(getChatMessagesResult));

                        break;

                    case ClientOperations.AddFriend:
                        var friendReceiveModelAdd = _jsonStringSerializer.Deserialize<FriendReceiveModel>(message.JsonData);

                        _logger.Info($"User {_client.Id} has send invite to {friendReceiveModelAdd.FriendId} for friend");

                        _clientsSynchronizer.SynchronizeAddFriendNotifications(friendReceiveModelAdd);
                        break;

                    case ClientOperations.DeleteFriend:
                        var friendReceiveModelDelete = _jsonStringSerializer.Deserialize<FriendReceiveModel>(message.JsonData);
                        var friendDeleteResult = _mainLogic.DeleteFriend(friendReceiveModelDelete);
                        _client.SendMessage(_encoder.Encryption(_jsonStringSerializer.Serialize(friendDeleteResult)));
                        //_client.SendMessage(_jsonStringSerializer.Serialize(friendDeleteResult));

                        _logger.Info($"User {_client.Id} has delete friend {friendReceiveModelDelete.FriendId} " +
                               $"result: {Enum.GetName(typeof(OperationsResults), friendDeleteResult.OperationResult)} " +
                               $"error info: {friendDeleteResult.ErrorInfo}");

                        if (friendDeleteResult.OperationResult == OperationsResults.Successfully)
                            _clientsSynchronizer.SynchronizeDeletingFriend(friendReceiveModelDelete);
                        break;

                    case ClientOperations.GetFriends:
                        var userPaginationReceiveModelGetFriends = _jsonStringSerializer.Deserialize<UserPaginationReceiveModel>(message.JsonData);
                        var getFriendsResult = _mainLogic.GetFriendsPage(userPaginationReceiveModelGetFriends);

                        _logger.Info($"User {_client.Id} has get friends " +
                               $"result: {Enum.GetName(typeof(OperationsResults), getFriendsResult.OperationResult)} " +
                               $"error info: {getFriendsResult.ErrorInfo}");

                        getFriendsResult.JsonData = _jsonStringSerializer.Serialize(getFriendsResult.JsonData as List<UserListResponseModel>);
                        _client.SendMessage(_encoder.Encryption(_jsonStringSerializer.Serialize(getFriendsResult)));
                        //_client.SendMessage(_jsonStringSerializer.Serialize(getFriendsResult));

                        break;

                    case ClientOperations.UpdateNotification:
                        var notificationReceiveModelUpdate = _jsonStringSerializer.Deserialize<NotificationReceiveModel>(message.JsonData);
                        var notificationUpdateResult = _mainLogic.UpdateNotification(notificationReceiveModelUpdate);
                        _client.SendMessage(_encoder.Encryption(_jsonStringSerializer.Serialize(notificationUpdateResult)));
                        //_client.SendMessage(_jsonStringSerializer.Serialize(notificationUpdateResult));

                        _logger.Info($"User {_client.Id} has update notification " +
                               $"notification id: {notificationReceiveModelUpdate.Id} " +
                               $"from user id: {notificationReceiveModelUpdate.FromUserId} " +
                               $"to user id: {notificationReceiveModelUpdate.ToUserId} " +
                               $"message: {notificationReceiveModelUpdate.Message} " + 
                               $"is accepted: {notificationReceiveModelUpdate.IsAccepted} " + 
                               $"result: {Enum.GetName(typeof(OperationsResults), notificationUpdateResult.OperationResult)} " +
                               $"error info: {notificationUpdateResult.ErrorInfo}");

                        if (notificationUpdateResult.OperationResult == OperationsResults.Successfully)
                            _clientsSynchronizer.SynchronizeAddingFriend(notificationReceiveModelUpdate);
                        break;
                    case ClientOperations.GetNotifications:
                        var notificationReceiveModelGetPage = _jsonStringSerializer.Deserialize<UserPaginationReceiveModel>(message.JsonData);
                        var getNotificationPageResult = _mainLogic.GetNotificationsPage(notificationReceiveModelGetPage);

                        _logger.Info($"User {_client.Id} has get notifications " +
                               $"result: {Enum.GetName(typeof(OperationsResults), getNotificationPageResult.OperationResult)} " +
                               $"error info: {getNotificationPageResult.ErrorInfo}");

                        if (getNotificationPageResult.JsonData != null)
                            getNotificationPageResult.JsonData = _jsonStringSerializer.Serialize(getNotificationPageResult.JsonData as List<NotificationResponseModel>);

                        _client.SendMessage(_encoder.Encryption(_jsonStringSerializer.Serialize(getNotificationPageResult)));
                        //_client.SendMessage(_jsonStringSerializer.Serialize(getNotificationPageResult));

                        break;
                    case ClientOperations.GetUser:
                        var userReceiveModelGetUser = _jsonStringSerializer.Deserialize<UserReceiveModel>(message.JsonData);
                        var getUserResult = _mainLogic.GetUser(userReceiveModelGetUser);

                        _logger.Info($"User {_client.Id} has get user {userReceiveModelGetUser.Id} info " +
                               $"result: {Enum.GetName(typeof(OperationsResults), getUserResult.OperationResult)} " +
                               $"error info: {getUserResult.ErrorInfo}");

                        if (getUserResult.JsonData != null)
                            getUserResult.JsonData = _jsonStringSerializer.Serialize(getUserResult.JsonData as UserResponseModel);

                        _client.SendMessage(_encoder.Encryption(_jsonStringSerializer.Serialize(getUserResult)));
                        //_client.SendMessage(_jsonStringSerializer.Serialize(getUserResult));
                        break;
                    default:
                        _client.SendMessage(_encoder.Encryption(_jsonStringSerializer.Serialize(new OperationResultInfo()
                        {
                            ErrorInfo = "Незарегистрированная операция",
                            OperationResult = OperationsResults.Unsuccessfully,
                            JsonData = null
                        })));
                        break;
                }
            }
            else
            {
                Console.WriteLine("Ошибка целостности полученных данных");
                _logger.Warn($"User {_client.Id} ошибка целостности полученных данных");
            }
        }

        public void HandleDisconnect() => _clientsSynchronizer.SynchronizeOfflineStatus(_client);
    }
}
