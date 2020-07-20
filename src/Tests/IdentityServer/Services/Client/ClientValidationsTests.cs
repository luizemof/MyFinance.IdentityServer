using IdentityServer.Exceptions;
using IdentityServer.Models.Client;
using IdentityServer.Services.Client;
using NUnit.Framework;

namespace IdentityServer.Tests.Services.Client
{
    [TestFixture]
    public class ClientValidationsTests
    {
        [Test]
        public void WhenCallValidation_WithEmptyInput_ThenShouldThrowValidationExceptionWithAllMessages()
        {
            // Act
            var validationException = Assert.Throws<ValidationException>(() => ClientValidations.Validate(new ClientInputModel()));
            Assert.AreEqual(expected: 3, validationException.ModelStateDictionary.Count);
        }

        [Test]
        public void WhenCallValidation_WithoutEmptyInput_ThenShouldThrowValidationExceptionWithAllMessages()
        {
            // Arrange
            var clientInputModel = new ClientInputModel()
            {
                ClientId = "ClientId",
                ClientName = "Name",
                AllowedGrantTypes = new System.Collections.Generic.List<string>() { "ABC" },
            };

            // Act
            ClientValidations.Validate(clientInputModel);
        }
    }
}