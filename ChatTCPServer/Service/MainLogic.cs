using ChatTCPServer.Interfaces;
using ChatTCPServer.Models;
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
        public OperationsResults UserRegistration(string Login, string Password)
        {
            try
            {
                userLogic_.Create(new User() { Login = Login, Password = Password });
                return OperationsResults.SuccessfullyRegistration;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return OperationsResults.UnsuccessfullyRegistration;
            }
        }

        public OperationsResults UserAuthorization(string Login, string Password)
        {
            if(userLogic_.Read(new User() { Login = Login, Password = Password }) != null)
            {
                return OperationsResults.SuccessfullyAuthorization;
            }
            return OperationsResults.UnsuccessfullyAuthorization;
        }
        #endregion
    }
}
