using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ChatTCPServer.Services
{
    /// <summary>
    /// Service for encryption/decryption messages 
    /// </summary>
    public class Encoder
    {
        //[0] - ?, [1] - product of two generated numbers
        private int[] _publicClientKey;

        private int[] _privateServerKey;

        private const int OFFSET = 50;

        public Encoder(int[] publicClientKey, int[] privateServerKey)
        {
            _publicClientKey = publicClientKey;
            _privateServerKey = privateServerKey;
        }

        public string Encryption(string message)
        {
            StringBuilder stringBuilderResult = new StringBuilder();
            int[] tmpEncrypCharsArr = new int[message.Length];

            for (int i = 0; i < message.Length; i++)
            {
                stringBuilderResult.Append(GetDegree(message[i], _publicClientKey[0]) % _publicClientKey[1]);
                if (i == 0)
                {
                    //stringBuilderResult.Append(GetDegree(message[i], _publicClientKey[0]) % _publicClientKey[1]);
                    //tmpEncrypCharsArr[i] = message[i];
                }
                else
                {
                    //tmpEncrypCharsArr[i] = (tmpEncrypCharsArr[i - 1] + message[i]) % _publicClientKey[1];
                    //stringBuilderResult.Append(GetDegree(tmpEncrypCharsArr[i], _publicClientKey[0]) % _publicClientKey[1]);
                }
                stringBuilderResult.Append(' ');
            }
            return stringBuilderResult.ToString();
        }

        public string Decryption(string message)
        {
            StringBuilder stringBuilderResult = new StringBuilder();
            var splitMessage = message.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var tmpDecrypCharsArr = new int[splitMessage.Length];
            for(int i = 0; i < splitMessage.Length; i++)
            {
                stringBuilderResult.Append((char)(GetDegree(Convert.ToInt32(splitMessage[i]), _privateServerKey[0]) % _privateServerKey[1]));
                if (i == 0)
                {
                    //tmpDecrypCharsArr[i] = (int)(GetDegree(Convert.ToInt32(splitMessage[i]), _privateServerKey[0]) % _privateServerKey[1]);
                    //Console.WriteLine(tmpDecrypCharsArr[i]);
                    //stringBuilderResult.Append((char)tmpDecrypCharsArr[i]);
                }
                else
                {
                    //tmpDecrypCharsArr[i] = (int)(GetDegree(Convert.ToInt32(splitMessage[i]), _privateServerKey[0]) % _privateServerKey[1]);
                    //Console.WriteLine("decrypted: " + tmpDecrypCharsArr[i]);
                    //var tmpDecryptValue = tmpDecrypCharsArr[i] - tmpDecrypCharsArr[i - 1];
                    //Console.WriteLine(tmpDecryptValue);
                    //if(tmpDecryptValue < 0)
                    //    stringBuilderResult.Append((char)(_privateServerKey[1] + tmpDecryptValue));
                    //else
                    //    stringBuilderResult.Append((char)((tmpDecrypCharsArr[i] - tmpDecrypCharsArr[i - 1]) % _privateServerKey[1]));
                }
            }

            return stringBuilderResult.ToString();
        }

        private BigInteger GetDegree(int value, int degree)
        {
            BigInteger result = 1;
            for (int i = 0; i < degree; i++)
                result *= value;
            return result;
        }
    }
}
