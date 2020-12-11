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
using ServerBusinessLogic.ReceiveModels.ChatModels;
using ServerBusinessLogic.ResponseModels.ChatModels;

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
        public void SynchronizeChatsMessages(MessageReceiveModel message)
        {
            var messageFromDb = _mainLogic.GetMessage(new MessageUserReceiveModel() { UserId = message.FromUserId, ChatId = message.ChatId, Message = message.UserMassage });
            var chatUsers = _mainLogic.GetChatUsers(message.ChatId).JsonData as List<ChatUserResponseModel>;
            var onlineUsers = _connectedClients.Where(ou => chatUsers.FirstOrDefault(u => u.Id == ou.Id) != null && ou.Id != message.FromUserId);

            Parallel.ForEach(onlineUsers, (ou) => 
            {
                var responseJson = _serializer.Serialize(new OperationResultInfo()
                {
                    ErrorInfo = string.Empty,
                    ToListener = ListenerType.ChatsMessagesListener,
                    OperationResult = OperationsResults.Successfully,
                    JsonData = _serializer.Serialize(messageFromDb)
                });
                ou.SendMessage(responseJson);
            });
        }

        /// <summary>
        /// Synchronization chats updatings
        /// </summary>
        /// <param name="chatReceiveModel"></param>
        public void SynchronizeUpdateChats(ChatReceiveModel chatReceiveModel)
        {
            var chatFromDb = _mainLogic.GetChat(chatReceiveModel);

            var chatUsers = _connectedClients.Where(connClient => connClient.Id != chatFromDb.CreatorId && chatFromDb.ChatUsers.FirstOrDefault(cu => cu.Id == connClient.Id) != null);

            Parallel.ForEach(chatUsers, (cu) =>
            {
                var responseJson = _serializer.Serialize(new OperationResultInfo()
                {
                    ErrorInfo = string.Empty,
                    OperationResult = OperationsResults.Successfully,
                    JsonData = _serializer.Serialize(chatFromDb),
                    ToListener = ListenerType.ChatListListener
                });

                cu.SendMessage(responseJson);
            });
        }

        /// <summary>
        /// Syncronization deleting chats 
        /// </summary>
        /// <param name="chatReceiveModel"></param>
        public void SynchronizeDeleteChats(ChatReceiveModel chatReceiveModel)
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
