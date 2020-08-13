using ChatTCPServer.Interfaces;
using ChatTCPServer.Models.DbModels;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatTCPServer.Service
{
    public class MainLogic
    {
        private readonly IChatLogic chatLogic_;
        private readonly IUserLogic userLogic_;
        private readonly IRelatChatUserLogic relatChatUserLogic_;

        public MainLogic(
            IChatLogic chatLogic,
            IUserLogic userLogic,
            IRelatChatUserLogic relatChatUserLogic)
        {
            chatLogic_ = chatLogic;
            userLogic_ = userLogic;
            relatChatUserLogic_ = relatChatUserLogic;
        }

        #region UserOperations
        public OperationsResults UserRegistration(User user)
        {
            try
            {
                userLogic_.Create(user);
                return OperationsResults.SuccessfullyRegistration;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return OperationsResults.UnsuccessfullyRegistration;
            }
        }

        public OperationsResults UserAuthorization(User user)
        {
            if(userLogic_.Read(user) != null)
            {
                return OperationsResults.SuccessfullyAuthorization;
            }
            return OperationsResults.UnsuccessfullyAuthorization;
        }

        public OperationsResults UserProfileUpdate(User user)
        {
            try
            {
                userLogic_.Update(user);
                return OperationsResults.SuccessfullyProfileUpdate;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return OperationsResults.UnsuccessfullyProfileUpdate;
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

        public OperationsResults CreateChat(Chat chat)
        {
            try
            {
                chatLogic_.Create(chat);
                return OperationsResults.SuccessfullyChatCreate;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return OperationsResults.UnsuccessfullyChatCreate;
            }
        }

        public OperationsResults ChatUpdate(Chat chat)
        {
            try
            {
                chatLogic_.Update(chat);
                return OperationsResults.SuccessfullyChatUpdate;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return OperationsResults.UnsuccessfullyChatUpdate;
            }
        }

        public OperationsResults ChatDelete(Chat chat)
        {
            try
            {
                chatLogic_.Delete(chat);
                return OperationsResults.SuccessfullyChatRemove;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return OperationsResults.UnsuccessfullyChatRemove;
            }
        }
        #endregion
        #region RelatChatUserOperations

        public OperationsResults AddUserToChat(User user, Chat chat)
        {
            try
            {
                relatChatUserLogic_.AddUserToChat(user, chat);
                return OperationsResults.SuccessfullyAddUserToChat;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return OperationsResults.UnsuccessfullyAddUserToChat;
            }
        }

        public OperationsResults RemoveUserFromChat(User user, Chat chat)
        {
            try
            {
                relatChatUserLogic_.RemoveUserFromChat(user, chat);
                return OperationsResults.SuccessfullyRemoveUserFromChat;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return OperationsResults.UnsuccessfullyRemoveUserFromChat;
            }
        }

        #endregion
    }
}
