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

namespace ServerBusinessLogic.BusinessLogic
{
    public class MainLogic
    {
        private readonly IChatLogic _chatLogic;
        private readonly IUserLogic _userLogic;
        private readonly IMessageLogic _messageLogic;
        private readonly IFriendLogic _friendLogic;
        private readonly INotificationLogic _notificationLogic;
        private readonly ISerializer _serializer;

        public MainLogic(
            IChatLogic chatLogic,
            IUserLogic userLogic,
            IMessageLogic messageLogic,
            IFriendLogic friendLogic,
            INotificationLogic notificationLogic,
            ISerializer serializer)
        {
            _chatLogic = chatLogic;
            _userLogic = userLogic;
            _messageLogic = messageLogic;
            _friendLogic = friendLogic;
            _notificationLogic = notificationLogic;
            _serializer = serializer;
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
                    OperationResult = OperationsResults.Successfully,
                    JsonData = null
                };
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new OperationResultInfo()
                {
                    ToListener = ListenerType.RegistrationListener,
                    ErrorInfo = ex.Message,
                    OperationResult = OperationsResults.Unsuccessfully,
                    JsonData = null
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
                    JsonData = _serializer.Serialize(user)
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
                Console.WriteLine(ex.Message);
                return new OperationResultInfo()
                {
                    ErrorInfo = ex.Message,
                    OperationResult = OperationsResults.Unsuccessfully
                };
            }
        }

        
        #endregion
        #region ChatLogicOperations

        public OperationResultInfo CreateChat(ChatReceiveModel chatModel)
        {
            try
            {
                _chatLogic.Create(chatModel);
                return new OperationResultInfo()
                {
                    ErrorInfo = string.Empty,
                    OperationResult = OperationsResults.Successfully
                };
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new OperationResultInfo()
                {
                    ErrorInfo = ex.Message,
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
                    OperationResult = OperationsResults.Successfully
                };
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new OperationResultInfo()
                {
                    ErrorInfo = ex.Message,
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
                    OperationResult = OperationsResults.Successfully
                };
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new OperationResultInfo()
                {
                    ErrorInfo = ex.Message,
                    OperationResult = OperationsResults.Unsuccessfully
                };
            }
        }

        public List<UserListResponseModel> GetChatUsers(int chatId)
        {
            try
            {
                return _chatLogic.GetChatUsers(chatId);
            }
            catch(Exception)
            {
                throw new Exception("Ошибка получения пользователей чата");
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

        public MessageResponseModel GetMessage(MessageUserReceiveModel model)
        {
            try
            {
                return _messageLogic.ReadMessage(model);
            }
            catch(Exception)
            {
                throw;
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
                Console.WriteLine(ex.Message);
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
