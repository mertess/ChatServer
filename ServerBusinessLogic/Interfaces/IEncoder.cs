using System;
using System.Collections.Generic;
using System.Text;

namespace ServerBusinessLogic.Interfaces
{
    public interface IEncoder
    {
        byte[] Encryption(string message);

        string Decryption(byte[] byteMessage);
    }
}
