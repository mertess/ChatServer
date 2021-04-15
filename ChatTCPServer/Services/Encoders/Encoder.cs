using ServerBusinessLogic.Interfaces;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ChatTCPServer.Services.Encoders
{
    /// <summary>
    /// Service for encryption/decryption messages 
    /// </summary>
    public class Encoder : IEncoder
    {
        private readonly byte[] _key = { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 };

        public byte[] Encryption(string message)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (Aes aes = Aes.Create())
                {
                    //private key
                    aes.Key = _key;

                    byte[] iv = aes.IV;

                    Console.WriteLine(aes.KeySize);

                    ms.Write(iv, 0, iv.Length);

                    using (CryptoStream cryptoStream = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        using (StreamWriter writer = new StreamWriter(cryptoStream))
                        {
                            writer.Write(message);
                        }
                    }
                }

                return ms.ToArray();
            }
        }

        public string Decryption(byte[] message)
        {
            using (MemoryStream ms = new MemoryStream(message))
            {
                using (Aes aes = Aes.Create())
                {
                    byte[] iv = new byte[aes.IV.Length];

                    ms.Read(iv, 0, iv.Length);

                    using (CryptoStream cryptoStream = new CryptoStream(ms, aes.CreateDecryptor(_key, iv), CryptoStreamMode.Read))
                    {
                        using (StreamReader reader = new StreamReader(cryptoStream))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
