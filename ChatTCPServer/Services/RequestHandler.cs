using Microsoft.Identity.Client;
using ServerBusinessLogic.BusinessLogic;
using ServerBusinessLogic.Enums.Transmission;
using ServerBusinessLogic.Interfaces;
using ServerBusinessLogic.ReceiveModels.MessageModels;
using ServerBusinessLogic.ReceiveModels.UserModels;
using ServerBusinessLogic.ResponseModels.UserModels;
using ServerBusinessLogic.TransmissionModels;
using ServerDatabaseSystem.Implementation;
using System;
using System.Collections.Generic;
using System.Management.Instrumentation;
using System.Net.Sockets;
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
        private readonly ClientsSynchronizer _clienstSynchronizer;

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
            _clienstSynchronizer = new ClientsSynchronizer(connectedClients, mainLogic, serializer);
        }

        /// <summary>
        /// Distribution of the received data across database services and sending the operation result
        /// and synchronizing another users chats
        /// </summary>
        /// <param name="clientOperationMessage"></param>
        public void HandleRequest(string messageJson)
        {
            var message = _serializer.Deserialize<ClientOperationMessage>(messageJson);

            switch(message.Operation)
            {
                case ClientOperations.Authorization:
                    var userReceiveModelAuthorization = _serializer.Deserialize<UserReceiveModel>(message.JsonData);
                    var authorizationResult = _mainLogic.UserAuthorization(userReceiveModelAuthorization);

                    //TODO : review deserialization
                    if(authorizationResult.OperationResult == OperationsResults.Successfully)
                    {
                        _client.Id = _serializer.Deserialize<UserResponseModel>(authorizationResult.JsonData).Id;
                        Console.WriteLine(_client.Id + " Пользователь успешно авторизировался");
                    }

                    var authorizationResultJson = _serializer.Serialize(authorizationResult);
                    _client.SendMessage(authorizationResultJson);
                    break;

                case ClientOperations.Registration:
                    var userReceiveModelRegistration = _serializer.Deserialize<UserReceiveModel>(message.JsonData);
                    var registrationResult = _serializer.Serialize(_mainLogic.UserRegistration(userReceiveModelRegistration));
                    _client.SendMessage(registrationResult);
                    break;

                case ClientOperations.SendMessage:
                    var messageReceiveModelSend = _serializer.Deserialize<MessageReceiveModel>(message.JsonData);
                    var messageSendResult = _serializer.Serialize(_mainLogic.AddMessage(messageReceiveModelSend));
                    _client.SendMessage(messageSendResult);
                    _clienstSynchronizer.SynchronizeChats(messageReceiveModelSend);
                    break;

                default:
                    throw new Exception("Незарегистрированная операция");
            }
        }
    }
}
