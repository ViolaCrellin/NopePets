using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Server.Configuration;

namespace Server.Util
{
    public class Encrypter
    {
        private readonly string _key;
        private readonly string _vector;
        private readonly DESCryptoServiceProvider _desCryptoServiceProvider;

        public Encrypter(IConfiguration config)
        {
            _key = config.Key;
            _vector = config.Vector;
            _desCryptoServiceProvider = new DESCryptoServiceProvider();
        }

        public string Encrypt(string textToEncrypt)
        {
            if (textToEncrypt == null)
                return null;

            var vectorBytes = Encoding.UTF8.GetBytes(_vector.Substring(0, 8));
            var keyBytes = Encoding.UTF8.GetBytes(_key.Substring(0, 8));
            var inputBytes = Encoding.UTF8.GetBytes(textToEncrypt);
            var memoryStream = new MemoryStream();
            var cryptoStream = new CryptoStream(memoryStream,
                _desCryptoServiceProvider.CreateEncryptor(keyBytes, vectorBytes),
                CryptoStreamMode.Write);

            cryptoStream.Write(inputBytes, 0, inputBytes.Length);
            cryptoStream.FlushFinalBlock();
            return Convert.ToBase64String(memoryStream.ToArray());
        }

        public string Decrypt(string textToDecrypt)
        {
            if (textToDecrypt == null)
                return null;

            var vectorBytes = Encoding.UTF8.GetBytes(_vector.Substring(0, 8));
            var keyBytes = Encoding.UTF8.GetBytes(_key.Substring(0, 8));
            var cleanInput = textToDecrypt.Replace(" ", "+");
            var inputBytes = Convert.FromBase64String(cleanInput);
            var memoryStream = new MemoryStream();
            var cryptoStream = new CryptoStream(memoryStream,
                _desCryptoServiceProvider.CreateDecryptor(keyBytes, vectorBytes), CryptoStreamMode.Write);
            cryptoStream.Write(inputBytes, 0, inputBytes.Length);
            cryptoStream.FlushFinalBlock();
            return Encoding.UTF8.GetString(memoryStream.ToArray());

        }
    }

}