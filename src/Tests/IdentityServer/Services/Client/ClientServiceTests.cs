using System.Threading.Tasks;
using IdentityServer.Models.Client;
using IdentityServer.Repository.Client;
using IdentityServer.Services.Client;
using IdentityServer.Tests.Cryptography;
using Moq;
using NUnit.Framework;

namespace IdentityServer.Tests.Services.Client
{
    [TestFixture]
    public class ClientServiceTests
    {
        private ClientService ClientService;
        private Mock<IClientDataAccess> ClientDataAccessMock;

        [SetUp]
        public void Setup()
        {
            ClientDataAccessMock = new Mock<IClientDataAccess>();
            ClientService = new ClientService(ClientDataAccessMock.Object, IdentityServerCryptographyTests.TestIdentityServerCryptography);
        }

        [Test]
        public void GivenItHasntAClient_WhenICallUpsertClient_ThenShouldCallInsert()
        {
            // Given
            var input = new ClientInputModel();
            ClientDataAccessMock
                .Setup(dataAccess => dataAccess.InsertAsync(It.IsAny<ClientData>()))
                .Returns(Task.CompletedTask);

            // When
            ClientService.UpsertClient(input).GetAwaiter().GetResult();

            // Then
            ClientDataAccessMock.Verify(dataAccess => dataAccess.InsertAsync(It.IsAny<ClientData>()), Times.Once);
        }
    }
}