using ServerBusinessLogic.BusinessLogic;
using ServerBusinessLogic.Interfaces;
using ServerBusinessLogic.ReceiveModels.MessageModels;
using ServerBusinessLogic.ReceiveModels.NotificationModels;
using ServerBusinessLogic.TransmissionModels;
using ServerBusinessLogic.Enums.Transmission;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
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
        /// Main logic working with database tables
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
        /// Synchronization users chats 
        /// </summary>
        public void SynchronizeChats(MessageReceiveModel message)
        {
            var messageFromDb = _mainLogic.GetMessage(new MessageUserReceiveModel() { UserId = message.FromUserId, ChatId = message.ChatId, Message = message.UserMassage });
            var chatUsers = _mainLogic.GetChatUsers(message.ChatId);
            var onlineUsers = _connectedClients.Where(ou => chatUsers.FirstOrDefault(u => u.Id == ou.Id) != null && ou.Id != message.FromUserId);
            
            var count = onlineUsers.Count();
            Parallel.ForEach(onlineUsers, (ou) => 
            {
                var responseJson = _serializer.Serialize(new OperationResultInfo()
                {
                    ErrorInfo = string.Empty,
                    ToListener = ListenerType.ChatMessagesListener,
                    OperationResult = OperationsResults.Successfully,
                    JsonData = _serializer.Serialize(messageFromDb)
                });
                ou.SendMessage(responseJson);
            });
        }

        /// <summary>
        /// Synchronization user notifications 
        /// </summary>
        public void SynchronizeNotifications(NotificationReceiveModel notification = null)
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
