using IdentityServer.Cryptography;
using NUnit.Framework;

namespace IdentityServer.Tests.Cryptography
{
    public class IdentityServerCryptographyTests
    {
        public static IdentityServerCryptography TestIdentityServerCryptography = new IdentityServerCryptography("TXlGaW5hbmNlLUFQSS1QYXNzd29yZA==");

        [TestCase]
        public void WhenItCallEncrypt_TheShouldReturnTheValueEncrypted()
        {
            // When
            
            var encryptedValue = TestIdentityServerCryptography.Encrypt("123");

            // Then
            Assert.AreEqual(expected: "c49nHayoXPOsZRI1NPkAIA==", encryptedValue);
        }   

        [TestCase]
        public void WhenItCallDencrypt_TheShouldReturnTheValueEncrypted()
        {
            // When
            var dencryptedValue = TestIdentityServerCryptography.Dencrypt("c49nHayoXPOsZRI1NPkAIA==");

            // Then
            Assert.AreEqual(expected: "123", dencryptedValue);
        }   
    }
}