using ChatTCPServer.Interfaces;
using ChatTCPServer.Models;
using ChatTCPServer.Models.DbModels;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace ChatTCPServer.Service
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
        public OperationResultInfo UserRegistration(User user)
        {
            try
            {
                userLogic_.Create(user);
                return new OperationResultInfo()
                {
                    Info = Enum.GetName(typeof(OperationsResults), OperationsResults.SuccessfullyRegistration),
                    OperationResult = OperationsResults.SuccessfullyRegistration
                };
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new OperationResultInfo()
                {
                    Info = ex.Message,
                    OperationResult = OperationsResults.UnsuccessfullyRegistration
                };
            }
        }

        public OperationResultInfo UserAuthorization(User user)
        {
            //some shit
            try
            {
                if (userLogic_.Read(user)[0] != null)
                {
                    return new OperationResultInfo()
                    {
                        Info = Enum.GetName(typeof(OperationsResults), OperationsResults.SuccessfullyAuthorization),
                        OperationResult = OperationsResults.SuccessfullyAuthorization
                    };
                }
                return new OperationResultInfo()
                {
                    Info = "Неправильный логин или пароль",
                    OperationResult = OperationsResults.UnsuccessfullyAuthorization
                };
            }
            catch (Exception)
            {
                return new OperationResultInfo()
                {
                    Info = "Неправильный логин или пароль",
                    OperationResult = OperationsResults.UnsuccessfullyAuthorization
                };
            }
        }

        public OperationResultInfo UserProfileUpdate(User user)
        {
            try
            {
                userLogic_.Update(user);
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

        public OperationResultInfo CreateChat(Chat chat)
        {
            try
            {
                chatLogic_.Create(chat);
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

        public OperationResultInfo ChatUpdate(Chat chat)
        {
            try
            {
                chatLogic_.Update(chat);
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

        public OperationResultInfo ChatDelete(Chat chat)
        {
            try
            {
                chatLogic_.Delete(chat);
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

        public OperationResultInfo AddUserToChat(User user, Chat chat)
        {
            try
            {
                relatChatUserLogic_.AddUserToChat(user, chat);
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

        public OperationResultInfo RemoveUserFromChat(User user, Chat chat)
        {
            try
            {
                relatChatUserLogic_.RemoveUserFromChat(user, chat);
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

        public void AddMessage(Message message) => messageLogic_.AddMessage(message);
        public OperationResultInfo DeleteMessage(Message message)
        {
            try
            {
                messageLogic_.DeleteMessage(message);
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
