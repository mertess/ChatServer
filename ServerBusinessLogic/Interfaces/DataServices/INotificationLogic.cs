using System;
using System.Collections.Generic;
using System.Text;

namespace ServerBusinessLogic.Interfaces.DataServices
{
    public interface INotificationLogic
    {
        void Create();

        void Update();

        void Delete();

        void Read();
    }
}
