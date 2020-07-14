using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace IdentityServer.Cryptography
{
    public sealed class IdentityServerCryptography
    {
        private readonly string Key;

        public IdentityServerCryptography(string key)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public string Encrypt(string plainText)
        {
            byte[] array;
            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(Key);
                aes.IV = new byte[16];
                var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                
                using (var memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }

        public string Dencrypt(string cipherText)
        {
            byte[] iv = new byte[16];  
            byte[] buffer = Convert.FromBase64String(cipherText);  
  
            using (var aes = Aes.Create())  
            {  
                aes.Key = Encoding.UTF8.GetBytes(Key);  
                aes.IV = iv;  
                var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);  
  
                using (var memoryStream = new MemoryStream(buffer))  
                {  
                    using (var cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))  
                    {  
                        using (var streamReader = new StreamReader((Stream)cryptoStream))  
                        {  
                            return streamReader.ReadToEnd();  
                        }  
                    }  
                }  
            }  
        }
    }
}