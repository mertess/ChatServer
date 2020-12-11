using ServerBusinessLogic.Interfaces.DataServices;
using ServerBusinessLogic.TransmissionModels;
using ServerBusinessLogic.Enums.Transmission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerBusinessLogic.ReceiveModels;
using ServerBusinessLogic.ReceiveModels.UserModels;
using ServerBusinessLogic.ReceiveModels.ChatModels;
using ServerBusinessLogic.ReceiveModels.MessageModels;
using System.Runtime.CompilerServices;
using ServerBusinessLogic.ResponseModels.MessageModels;
using ServerBusinessLogic.ResponseModels.ChatModels;
using ServerBusinessLogic.ResponseModels.UserModels;
using ServerBusinessLogic.Interfaces;
using System.Diagnostics;

namespace ServerBusinessLogic.BusinessLogic
{
    public class MainLogic
    {
        private readonly IChatLogic _chatLogic;
        private readonly IUserLogic _userLogic;
        private readonly IMessageLogic _messageLogic;
        private readonly IFriendLogic _friendLogic;
        private readonly INotificationLogic _notificationLogic;

        public MainLogic(
            IChatLogic chatLogic,
            IUserLogic userLogic,
            IMessageLogic messageLogic,
            IFriendLogic friendLogic,
            INotificationLogic notificationLogic)
        {
            _chatLogic = chatLogic;
            _userLogic = userLogic;
            _messageLogic = messageLogic;
            _friendLogic = friendLogic;
            _notificationLogic = notificationLogic;
        }

        #region UserLogicOperations
        public OperationResultInfo UserRegistration(UserReceiveModel userModel)
        {
            try
            {
                _userLogic.Create(userModel);
                return new OperationResultInfo()
                {
                    ToListener = ListenerType.RegistrationListener,
                    ErrorInfo = string.Empty,
                    OperationResult = OperationsResults.Successfully
                };
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new OperationResultInfo()
                {
                    ToListener = ListenerType.RegistrationListener,
                    ErrorInfo = ex.Message,
                    OperationResult = OperationsResults.Unsuccessfully
                };
            }
        }

        public OperationResultInfo UserAuthorization(UserReceiveModel userModel)
        {
            try
            {
                var user = _userLogic.GetUser(userModel);

                return new OperationResultInfo()
                {
                    ToListener = ListenerType.AuthorizationListener,
                    ErrorInfo = string.Empty,
                    OperationResult = OperationsResults.Successfully,
                    JsonData = user
                };
            }
            catch(Exception ex)
            {
                return new OperationResultInfo()
                {
                    ToListener = ListenerType.AuthorizationListener,
                    ErrorInfo = ex.Message,
                    OperationResult = OperationsResults.Unsuccessfully
                };
            }
        }

        public OperationResultInfo UserProfileUpdate(UserReceiveModel userModel)
        {
            try
            {
                _userLogic.Update(userModel);
                return new OperationResultInfo()
                {
                    ErrorInfo = string.Empty,
                    OperationResult = OperationsResults.Successfully
                };
            }
            catch(Exception ex)
            {
                return new OperationResultInfo()
                {
                    ErrorInfo = ex.Message,
                    OperationResult = OperationsResults.Unsuccessfully
                };
            }
        }

        public OperationResultInfo GetPageOfUsers(UserPaginationReceiveModel model)
        {
            try
            {
                var users = _userLogic.ReadPage(model);

                return new OperationResultInfo()
                {
                    ErrorInfo = string.Empty,
                    OperationResult = OperationsResults.Successfully,
                    ToListener = ListenerType.UserListListener,
                    JsonData = users
                };
            }
            catch(Exception ex)
            {
                return new OperationResultInfo()
                {
                    ErrorInfo = ex.Message,
                    OperationResult = OperationsResults.Unsuccessfully,
                    ToListener = ListenerType.UserListListener
                };
            }
        }

        
        #endregion
        #region ChatLogicOperations

        public OperationResultInfo ChatCreate(ChatReceiveModel chatModel)
        {
            try
            {
                var chatResponseModelResult = _chatLogic.Create(chatModel);
                return new OperationResultInfo()
                {
                    ErrorInfo = string.Empty,
                    OperationResult = OperationsResults.Successfully,
                    ToListener = ListenerType.ChatListListener,
                    JsonData = chatResponseModelResult
                };
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new OperationResultInfo()
                {
                    ErrorInfo = ex.Message,
                    ToListener = ListenerType.ChatListListener,
                    OperationResult = OperationsResults.Unsuccessfully
                };
            }
        }

        public OperationResultInfo ChatUpdate(ChatReceiveModel chatModel)
        {
            try
            {
                _chatLogic.Update(chatModel);
                return new OperationResultInfo()
                {
                    ErrorInfo = string.Empty,
                    ToListener = ListenerType.ChatListListener,
                    OperationResult = OperationsResults.Successfully
                };
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new OperationResultInfo()
                {
                    ErrorInfo = ex.Message,
                    ToListener = ListenerType.ChatListListener,
                    OperationResult = OperationsResults.Unsuccessfully
                };
            }
        }

        public OperationResultInfo ChatDelete(ChatReceiveModel chatModel)
        {
            try
            {
                _chatLogic.Delete(chatModel);
                return new OperationResultInfo()
                {
                    ErrorInfo = string.Empty,
                    ToListener = ListenerType.ChatListDeleteListener,
                    OperationResult = OperationsResults.Successfully
                };
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new OperationResultInfo()
                {
                    ErrorInfo = ex.Message,
                    ToListener = ListenerType.ChatListDeleteListener,
                    OperationResult = OperationsResults.Unsuccessfully
                };
            }
        }

        public OperationResultInfo GetChatUsers(int chatId)
        {
            try
            {
                var users = _chatLogic.GetChatUsers(chatId);
                return new OperationResultInfo()
                {
                    ErrorInfo = string.Empty,
                    OperationResult = OperationsResults.Successfully,
                    JsonData = users
                };
            }
            catch(Exception ex)
            {
                return new OperationResultInfo()
                {
                    ErrorInfo = ex.Message,
                    OperationResult = OperationsResults.Unsuccessfully,
                    JsonData = null
                };
            }
        }

        public ChatResponseModel GetChat(ChatReceiveModel model)
        {
            try
            {
                return _chatLogic.GetChat(model);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public OperationResultInfo GetPageOfChats(UserPaginationReceiveModel model)
        {
            try
            {
                var chatsPage = _chatLogic.ReadPage(model);
                return new OperationResultInfo()
                {
                    ErrorInfo = string.Empty,
                    OperationResult = OperationsResults.Successfully,
                    JsonData = chatsPage,
                    ToListener = ListenerType.ChatListListener
                };
            }
            catch(Exception ex)
            {
                return new OperationResultInfo()
                {
                    ErrorInfo = ex.Message,
                    OperationResult = OperationsResults.Unsuccessfully,
                    ToListener = ListenerType.ChatListListener
                };
            }
        }

        #endregion
        #region MessageLogicOperations

        public OperationResultInfo AddMessage(MessageReceiveModel messageModel)
        {
            try
            {
                _messageLogic.AddMessage(messageModel);

                return new OperationResultInfo()
                {
                    ErrorInfo = string.Empty,
                    OperationResult = OperationsResults.Successfully,
                    ToListener = ListenerType.ChatsMessagesListener
                };
            }
            catch(Exception ex)
            {
                return new OperationResultInfo()
                {
                    ErrorInfo = ex.Message,
                    OperationResult = OperationsResults.Unsuccessfully,
                    ToListener = ListenerType.ChatsMessagesListener
                };
            }
        }

        public MessageResponseModel GetMessage(MessageUserReceiveModel model)
        {
            try
            {
                return _messageLogic.ReadMessage(model);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public OperationResultInfo DeleteMessage(MessageReceiveModel messageModel)
        {
            try
            {
                _messageLogic.DeleteMessage(messageModel);
                return new OperationResultInfo()
                {
                    ErrorInfo = string.Empty,
                    OperationResult = OperationsResults.Successfully
                };
            }
            catch(Exception ex)
            {
                return new OperationResultInfo()
                {
                    ErrorInfo = ex.Message,
                    OperationResult = OperationsResults.Unsuccessfully
                };
            }
        }

        public OperationResultInfo UpdateMessage(MessageReceiveModel messageModel)
        {
            try
            {
                //_messageLogic.Message(messageModel);
                return new OperationResultInfo()
                {
                    ErrorInfo = string.Empty,
                    OperationResult = OperationsResults.Successfully
                };
            }
            catch (Exception ex)
            {
                return new OperationResultInfo()
                {
                    ErrorInfo = ex.Message,
                    OperationResult = OperationsResults.Unsuccessfully
                };
            }
        }

        public OperationResultInfo GetChatMessages(ChatPaginationReceiveModel model)
        {
            try
            {
                //_messageLogic.Message(messageModel);
                return new OperationResultInfo()
                {
                    ErrorInfo = string.Empty,
                    OperationResult = OperationsResults.Successfully
                };
            }
            catch (Exception ex)
            {
                return new OperationResultInfo()
                {
                    ErrorInfo = ex.Message,
                    OperationResult = OperationsResults.Unsuccessfully
                };
            }
        }

        #endregion
    }
}
