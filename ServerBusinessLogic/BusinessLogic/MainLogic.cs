﻿using ServerBusinessLogic.Interfaces;
using ServerBusinessLogic.TransmissionModels;
using ServerBusinessLogic.Enums.Transmission;
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
                    ErrorInfo = string.Empty,
                    OperationResult = OperationsResults.Successfully,
                    Data = string.Empty
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
                    Data = string.Empty
                };
            }
        }

        public OperationResultInfo UserAuthorization(UserReceiveModel userModel)
        {
            var user = userLogic_.Read(userModel)?[0];
            if (user != null)
            {
                return new OperationResultInfo()
                {
                    ToListener = ListenerType.AuthorizationListener,
                    ErrorInfo = string.Empty,
                    OperationResult = OperationsResults.Successfully,
                    Data = string.Empty
                };
            }
            return new OperationResultInfo()
            {
                ToListener = ListenerType.AuthorizationListener,
                ErrorInfo = "Неправильный логин или пароль",
                OperationResult = OperationsResults.Unsuccessfully,
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
                chatLogic_.Update(chatModel);
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
                chatLogic_.Delete(chatModel);
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
        #region RelatChatUserOperations

        public OperationResultInfo AddUserToChat(UserReceiveModel userModel, ChatReceiveModel chatModel)
        {
            try
            {
                relatChatUserLogic_.AddUserToChat(userModel, chatModel);
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

        public OperationResultInfo RemoveUserFromChat(UserReceiveModel userModel, ChatReceiveModel chatModel)
        {
            try
            {
                relatChatUserLogic_.RemoveUserFromChat(userModel, chatModel);
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
        #region ServerDBOperations

        public void AddMessage(MessageReceiveModel messageModel) => messageLogic_.AddMessage(messageModel);
        public OperationResultInfo DeleteMessage(MessageReceiveModel messageModel)
        {
            try
            {
                messageLogic_.DeleteMessage(messageModel);
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