using System;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using IdentityServer.Exceptions;
using IdentityServer.Models.Client;
using IdentityServer.Repository.Client;
using IdentityServer.Services.Client;
using IdentityServer.Tests.Cryptography;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Clusters;
using MongoDB.Driver.Core.Connections;
using MongoDB.Driver.Core.Servers;
using Moq;
using NUnit.Framework;

namespace IdentityServer.Tests.Services.Client
{
    [TestFixture]
    public class ClientServiceTests
    {
        private ClientService ClientService;
        private Mock<IClientDataAccess> ClientDataAccessMock;
        private MongoWriteException MongoWriteException;

        [SetUp]
        public void Setup()
        {
            ClientDataAccessMock = new Mock<IClientDataAccess>();
            ClientService = new ClientService(ClientDataAccessMock.Object, IdentityServerCryptographyTests.TestIdentityServerCryptography);

            var connectionId = new ConnectionId(new ServerId(new ClusterId(1), new DnsEndPoint("localhost", 27017)), 2);
            var writeConcernErrorConstructor = typeof(WriteError).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)[0];
            var writeError = (WriteError)writeConcernErrorConstructor.Invoke(new object[] { ServerErrorCategory.DuplicateKey, 11000, "writeError", new BsonDocument("details", "writeError") });

            MongoWriteException = new MongoWriteException(connectionId, writeError, null, null);
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
            ClientService.UpsertClientAsync(input).GetAwaiter().GetResult();

            // Then
            ClientDataAccessMock.Verify(dataAccess => dataAccess.InsertAsync(It.IsAny<ClientData>()), Times.Once);
            ClientDataAccessMock.Verify(dataAccess => dataAccess.ReplaceAsync(It.IsAny<ClientData>(), It.IsAny<Expression<Func<ClientData, bool>>>()), Times.Never);
        }

        [Test]
        public void GivenIHasAClientId_WhenCallUpsertToInsertNewClient_ThenShouldThrowAlreadyExistsException()
        {
            // Given
            ClientDataAccessMock.Setup(dataAccess => dataAccess.InsertAsync(It.IsAny<ClientData>())).Throws(MongoWriteException);

            // When
            Assert.Throws(typeof(AlreadyExistsException), () => ClientService.UpsertClientAsync(new ClientInputModel()).GetAwaiter().GetResult());
        }

        [Test]
        public void GivenItHasClient_WhenCallUpsertToUpdateClient_ThenShouldCallReplace()
        {
            // Given
            var input = new ClientInputModel(){ Id = "1˝"};
            ClientDataAccessMock
                .Setup(dataAccess => dataAccess.ReplaceAsync(It.IsAny<ClientData>(), It.IsAny<Expression<Func<ClientData, bool>>>()))
                .ReturnsAsync(true);

            // When
            ClientService.UpsertClientAsync(input).GetAwaiter().GetResult();

            // Then
            ClientDataAccessMock.Verify(dataAccess => dataAccess.InsertAsync(It.IsAny<ClientData>()), Times.Never);
            ClientDataAccessMock.Verify(dataAccess => dataAccess.ReplaceAsync(It.IsAny<ClientData>(), It.IsAny<Expression<Func<ClientData, bool>>>()), Times.Once);
        }
    }
}