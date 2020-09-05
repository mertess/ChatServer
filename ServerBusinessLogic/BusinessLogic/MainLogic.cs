using ServerBusinessLogic.Interfaces;
using ServerBusinessLogic.HelperModels;
using ServerBusinessLogic.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerBusinessLogic.ReceiveModels;

namespace ServerBusinessLogic.BusinessLogic
{
    public class MainLogic
    {
        private readonly IChatLogic chatLogic_;
        private readonly IUserLogic userLogic_;
        private readonly IRelatChatUserLogic relatChatUserLogic_;
        private readonly IMessageLogic messageLogic_;

        public MainLogic(
            IChatLogic chatLogic,
            IUserLogic userLogic,
            IRelatChatUserLogic relatChatUserLogic,
            IMessageLogic messageLogic)
        {
            chatLogic_ = chatLogic;
            userLogic_ = userLogic;
            relatChatUserLogic_ = relatChatUserLogic;
            messageLogic_ = messageLogic;
        }

        #region UserOperations
        public OperationResultInfo UserRegistration(UserReceiveModel userModel)
        {
            try
            {
                userLogic_.Create(userModel);
                return new OperationResultInfo()
                {
                    ToListener = ListenerType.RegistrationListener,
                    Info = Enum.GetName(typeof(OperationsResults), OperationsResults.SuccessfullyRegistration),
                    OperationResult = OperationsResults.SuccessfullyRegistration,
                    Data = string.Empty
                };
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new OperationResultInfo()
                {
                    ToListener = ListenerType.RegistrationListener,
                    Info = ex.Message,
                    OperationResult = OperationsResults.UnsuccessfullyRegistration,
                    Data = string.Empty
                };
            }
        }

        public OperationResultInfo UserAuthorization(UserReceiveModel userModel)
        {
            var userList = userLogic_.Read(userModel);
            if (userList.Count != 0 && userList[0] != null)
            {
                return new OperationResultInfo()
                {
                    ToListener = ListenerType.AuthorizationListener,
                    Info = Enum.GetName(typeof(OperationsResults), OperationsResults.SuccessfullyAuthorization),
                    OperationResult = OperationsResults.SuccessfullyAuthorization,
                    Data = string.Empty
                };
            }
            return new OperationResultInfo()
            {
                ToListener = ListenerType.AuthorizationListener,
                Info = "Неправильный логин или пароль",
                OperationResult = OperationsResults.UnsuccessfullyAuthorization,
                Data = string.Empty
            };
        }

        public OperationResultInfo UserProfileUpdate(UserReceiveModel userModel)
        {
            try
            {
                userLogic_.Update(userModel);
                return new OperationResultInfo()
                {
                    Info = Enum.GetName(typeof(OperationsResults), OperationsResults.SuccessfullyProfileUpdate),
                    OperationResult = OperationsResults.SuccessfullyProfileUpdate
                };
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new OperationResultInfo()
                {
                    Info = ex.Message,
                    OperationResult = OperationsResults.UnsuccessfullyProfileUpdate
                };
            }
        }

        /* public OperationsResults UserProfileDelete(User user)
         {
             try
             {

             }
             catch(Exception ex)
             {
                 Console.WriteLine(ex.Message);
             }
         }*/
        #endregion
        #region ChatOperations

        public OperationResultInfo CreateChat(ChatReceiveModel chatModel)
        {
            try
            {
                chatLogic_.Create(chatModel);
                return new OperationResultInfo()
                {
                    Info = Enum.GetName(typeof(OperationsResults), OperationsResults.SuccessfullyChatCreate),
                    OperationResult = OperationsResults.SuccessfullyChatCreate
                };
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new OperationResultInfo()
                {
                    Info = ex.Message,
                    OperationResult = OperationsResults.UnsuccessfullyChatCreate
                };
            }
        }

        public OperationResultInfo ChatUpdate(ChatReceiveModel chatModel)
        {
            try
            {
                chatLogic_.Update(chatModel);
                return new OperationResultInfo()
                {
                    Info = Enum.GetName(typeof(OperationsResults), OperationsResults.SuccessfullyChatUpdate),
                    OperationResult = OperationsResults.SuccessfullyChatUpdate
                };
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new OperationResultInfo()
                {
                    Info = ex.Message,
                    OperationResult = OperationsResults.UnsuccessfullyChatUpdate
                };
            }
        }

        public OperationResultInfo ChatDelete(ChatReceiveModel chatModel)
        {
            try
            {
                chatLogic_.Delete(chatModel);
                return new OperationResultInfo()
                {
                    Info = Enum.GetName(typeof(OperationsResults), OperationsResults.SuccessfullyChatRemove),
                    OperationResult = OperationsResults.SuccessfullyChatRemove
                };
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new OperationResultInfo()
                {
                    Info = ex.Message,
                    OperationResult = OperationsResults.UnsuccessfullyChatRemove
                };
            }
        }
        #endregion
        #region RelatChatUserOperations

        public OperationResultInfo AddUserToChat(UserReceiveModel userModel, ChatReceiveModel chatModel)
        {
            try
            {
                relatChatUserLogic_.AddUserToChat(userModel, chatModel);
                return new OperationResultInfo()
                {
                    Info = Enum.GetName(typeof(OperationsResults), OperationsResults.SuccessfullyAddUserToChat),
                    OperationResult = OperationsResults.SuccessfullyAddUserToChat
                };
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new OperationResultInfo()
                {
                    Info = ex.Message,
                    OperationResult = OperationsResults.UnsuccessfullyAddUserToChat
                };
            }
        }

        public OperationResultInfo RemoveUserFromChat(UserReceiveModel userModel, ChatReceiveModel chatModel)
        {
            try
            {
                relatChatUserLogic_.RemoveUserFromChat(userModel, chatModel);
                return new OperationResultInfo()
                {
                    Info = Enum.GetName(typeof(OperationsResults), OperationsResults.SuccessfullyRemoveUserFromChat),
                    OperationResult = OperationsResults.SuccessfullyRemoveUserFromChat
                };
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new OperationResultInfo()
                {
                    Info = ex.Message,
                    OperationResult = OperationsResults.UnsuccessfullyRemoveUserFromChat
                };
            }
        }

        #endregion
        #region ServerDBOperations

        public void AddMessage(MessageReceiveModel messageModel) => messageLogic_.AddMessage(messageModel);
        public OperationResultInfo DeleteMessage(MessageReceiveModel messageModel)
        {
            try
            {
                messageLogic_.DeleteMessage(messageModel);
                return new OperationResultInfo()
                {
                    Info = Enum.GetName(typeof(OperationsResults), OperationsResults.SuccessfullyRemoveMessage),
                    OperationResult = OperationsResults.SuccessfullyRemoveMessage
                };
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new OperationResultInfo()
                {
                    Info = ex.Message,
                    OperationResult = OperationsResults.UnsuccessfullyRemoveMessage
                };
            }
        }

        #endregion
    }
}
