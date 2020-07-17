using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using IdentityServer.Exceptions;
using IdentityServer.Models.IdentityResource;
using IdentityServer.Repository.IdentityResource;
using IdentityServer.Services.IdentityResource;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Clusters;
using MongoDB.Driver.Core.Connections;
using MongoDB.Driver.Core.Servers;
using Moq;
using NUnit.Framework;

namespace IdentityServer.Tests.Services.IdentityResource
{
    [TestFixture]
    public class IdentityResourceServiceTests
    {
        private IdentityResourceService IdentityResourceService;
        private Mock<IIdentityResourceDataAccess> IdentityResourceDataAccessMock;
        private MongoWriteException MongoWriteException;

        [SetUp]
        public void Setup()
        {
            IdentityResourceDataAccessMock = new Mock<IIdentityResourceDataAccess>();
            IdentityResourceService = new IdentityResourceService(IdentityResourceDataAccessMock.Object);

            var connectionId = new ConnectionId(new ServerId(new ClusterId(1), new DnsEndPoint("localhost", 27017)), 2);
            var writeConcernErrorConstructor = typeof(WriteError).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)[0];
            var writeError = (WriteError)writeConcernErrorConstructor.Invoke(new object[] { ServerErrorCategory.DuplicateKey, 11000, "writeError", new BsonDocument("details", "writeError") });
            
            MongoWriteException = new MongoWriteException(connectionId, writeError, null, null);
        }

        [Test]
        public void GivenItHasIdentityResource_WhenItCallsGetAllIdentityResource_ThenShouldReturnAll()
        {
            // Given
            var datas = new[]
            {
                new IdentityResourceData("1", "Identity1", "Identity 1", "Description Identity 1", new List<string>(){ "1", "2"}, true),
                new IdentityResourceData("2", "Identity2", "Identity 2", "Description Identity 2", new List<string>(){ "3", "4"}, true)
            };

            IdentityResourceDataAccessMock
                .Setup(dataAccess => dataAccess.GetAsync())
                .ReturnsAsync(datas.AsEnumerable());

            // When
            var identities = IdentityResourceService.GetAllIdentityResources().GetAwaiter().GetResult();

            // Then
            Assert.IsNotNull(identities);
            Assert.IsTrue(identities.Count() == 2);
            IdentityResourceDataAccessMock.Verify(dataAccess => dataAccess.GetAsync(), Times.Once);
            Assert.IsTrue(datas.All(data => identities.Any(model => CheckDataAndIdentityModel(data, model))));
        }


        [Test]
        public void GivenItHasntIdentityResource_WhenItCallsUpsert_ThenShouldInsert()
        {
            // Given
            var identityResourceInputModel = new IdentityResourceInputModel();


            IdentityResourceDataAccessMock
                .Setup(dataAccess => dataAccess.InsertAsync(It.IsAny<IdentityResourceData>()))
                .Returns(Task.CompletedTask);

            // When
            IdentityResourceService.UpsertIdentityResource(identityResourceInputModel).GetAwaiter().GetResult();

            // Then
            IdentityResourceDataAccessMock.Verify(dataAccess => dataAccess.InsertAsync(It.IsAny<IdentityResourceData>()), Times.Once);
        }

        [Test]
        public void GivenItHasIdentityResource_WhenItCallsUpsert_ThenShouldReplace()
        {
            // Given
            var identityResourceInputModel = new IdentityResourceInputModel() { Id = "1" };

            IdentityResourceDataAccessMock
                .Setup(dataAccess => dataAccess.ReplaceAsync(It.IsAny<IdentityResourceData>(), It.IsAny<Expression<Func<IdentityResourceData, bool>>>()))
                .ReturnsAsync(true);

            // When
            IdentityResourceService.UpsertIdentityResource(identityResourceInputModel).GetAwaiter().GetResult();

            // Then
            IdentityResourceDataAccessMock.Verify(dataAccess => dataAccess.ReplaceAsync(It.IsAny<IdentityResourceData>(), It.IsAny<Expression<Func<IdentityResourceData, bool>>>()), Times.Once);
        }

        [Test]
        public void GivenItHasIdentityResource_WhenItCallsUpsert_AndAlreadyExistsName_ThenShouldThrowAlreadyExistsException()
        {
            // Given
            var identityResourceInputModel = new IdentityResourceInputModel() { Id = "1" };

            IdentityResourceDataAccessMock
                .Setup(dataAccess => dataAccess.ReplaceAsync(It.IsAny<IdentityResourceData>(), It.IsAny<Expression<Func<IdentityResourceData, bool>>>()))
                .Throws(MongoWriteException);

            // When
            Assert.Throws(typeof(AlreadyExistsException), () => IdentityResourceService.UpsertIdentityResource(identityResourceInputModel).GetAwaiter().GetResult());

            // Then
            IdentityResourceDataAccessMock.Verify(dataAccess => dataAccess.ReplaceAsync(It.IsAny<IdentityResourceData>(), It.IsAny<Expression<Func<IdentityResourceData, bool>>>()), Times.Once);
        }

        [Test]
        public void GivenItHasnotIdentityResource_WhenItCallsUpsert_AndAlreadyExistsName_ThenShouldThrowAlreadyExistsException()
        {
            // Given
            var identityResourceInputModel = new IdentityResourceInputModel();

            IdentityResourceDataAccessMock
                .Setup(dataAccess => dataAccess.InsertAsync(It.IsAny<IdentityResourceData>()))
                .Throws(MongoWriteException);

            // When
            Assert.Throws(typeof(AlreadyExistsException), () => IdentityResourceService.UpsertIdentityResource(identityResourceInputModel).GetAwaiter().GetResult());

            // Then
            IdentityResourceDataAccessMock.Verify(dataAccess => dataAccess.InsertAsync(It.IsAny<IdentityResourceData>()), Times.Once);
        }

        private bool CheckDataAndIdentityModel(IdentityResourceData data, IdentityResourceModel model)
        {
            return
                data != null
                &&
                model != null
                &&
                data.Name == model.Name
                &&
                data.DisplayName == model.DisplayName
                &&
                data.Description == model.Description
                &&
                data.Enabled == model.Enabled
                &&
                data.Id == model.Id
                &&
                data.UserClaims.All(dataUserClaim => model.UserClaims.Contains(dataUserClaim));
        }
    }
}