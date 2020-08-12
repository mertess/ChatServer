using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatTCPServer
{
    public enum OperationsResults
    {
        SuccessfullyRegistration,
        UnsuccessfullyRegistration,

        SuccessfullyAuthorization,
        UnsuccessfullyAuthorization,

        SuccessfullyProfileUpdate,
        UnsuccessfullyProfileUpdate,

        SuccessfullyChatCreate,
        UnsuccessfullyChatCreate,

        SuccessfullyChatUpdate,
        UnsuccessfullyChatUpdate,

        SuccessfullyChatRemove,
        UnsuccessfullyChatRemove,

        SuccessfullyAddUserToChat,
        UnsuccessfullyAddUserToChat,

        SuccessfullyRemoveUserFromChat,
        UnsuccessfullyRemoveUserFromChat
    }
}
