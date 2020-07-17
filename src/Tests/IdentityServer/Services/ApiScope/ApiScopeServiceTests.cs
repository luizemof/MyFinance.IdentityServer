using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using IdentityServer.Exceptions;
using IdentityServer.Models.ApiScope;
using IdentityServer.Repository.ApiScopes;
using IdentityServer.Services;
using IdentityServer.Services.ApiScope;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Clusters;
using MongoDB.Driver.Core.Connections;
using MongoDB.Driver.Core.Servers;
using Moq;
using NUnit.Framework;
using IdentityApiScope = IdentityServer4.Models.ApiScope;

namespace IdentityServer.Tests.Services.ApiScope
{
    [TestFixture]
    public class ApiScopeServiceTests
    {
        private IApiScopeService ApiScopeService;
        private Mock<IApiScopeDataAccess> ApiScopeDataAccessMock;
        private MongoWriteException MongoWriteException;

        private Expression<Func<ApiScopeData, bool>> ExpressionItsAny;

        [SetUp]
        public void Setup()
        {
            ApiScopeDataAccessMock = new Mock<IApiScopeDataAccess>();
            ApiScopeService = new ApiScopeService(ApiScopeDataAccessMock.Object);
            ExpressionItsAny = It.IsAny<Expression<Func<ApiScopeData, bool>>>();

            var connectionId = new ConnectionId(new ServerId(new ClusterId(1), new DnsEndPoint("localhost", 27017)), 2);
            var writeConcernErrorConstructor = typeof(WriteError).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)[0];
            var writeError = (WriteError)writeConcernErrorConstructor.Invoke(new object[] { ServerErrorCategory.DuplicateKey, 11000, "writeError", new BsonDocument("details", "writeError") });

            MongoWriteException = new MongoWriteException(connectionId, writeError, null, null);
        }

        [Test]
        public void GivenItHasAnApiScope_WhenItCallsGetAllApiScopes_ThenShouldReturnAllScopes()
        {
            // Given
            var apiScopesData = new[]
            {
                new ApiScopeData(id: "1", name: "API1", displayName: "API 1", description: "Description 1", true),
                new ApiScopeData(id: "1", name: "API2", displayName: "API 2", description: "Description 2", true)
            };

            ApiScopeDataAccessMock
                .Setup(apiScopeDataAccess => apiScopeDataAccess.GetAsync())
                .ReturnsAsync(apiScopesData.AsEnumerable());

            // When
            var apiScopes = ApiScopeService.GetAllApiScopesAsync().GetAwaiter().GetResult();

            // Then
            ApiScopeDataAccessMock.Verify(apiScopeDataAccess => apiScopeDataAccess.GetAsync(), Times.Once);
            Assert.IsNotNull(apiScopes);
            Assert.IsTrue(apiScopes.Count() == 2);
            Assert.IsTrue(apiScopesData.All(data => apiScopes.Any(apiScope => apiScope.Name == data.Name && apiScope.DisplayName == data.DisplayName)));
        }

        [Test]
        public void GivenItHasANewApiScope_WhenItCallsInsertApiScopes_ThenShouldInsert()
        {
            // Given
            var apiScopeInputModel = new ApiScopeInputModel()
            {
                Name = "API1",
                DisplayName = "API 1"
            };

            ApiScopeDataAccessMock
                .Setup(apiScopeDataAccess => apiScopeDataAccess.InsertAsync(It.IsAny<ApiScopeData>()))
                .Returns(Task.CompletedTask);

            // When
            ApiScopeService.UpsertApiScopeAsync(apiScopeInputModel).GetAwaiter().GetResult();

            // Then
            ApiScopeDataAccessMock.Verify(dataAccess => dataAccess.InsertAsync(It.IsAny<ApiScopeData>()), Times.Once);
            ApiScopeDataAccessMock.Verify(dataAccess => dataAccess.ReplaceAsync(It.IsAny<ApiScopeData>(), ExpressionItsAny), Times.Never);
        }

        [Test]
        public void GivenItHasApiScope_WhenItCallsUpsertApiScopes_ThenShouldCallReplace()
        {
            // Given
            var apiScopeInputModel = new ApiScopeInputModel()
            {
                Id = "1",
                Name = "API1",
                DisplayName = "API 1"
            };

            ApiScopeDataAccessMock
                .Setup(apiScopeDataAccess => apiScopeDataAccess.ReplaceAsync(It.IsAny<ApiScopeData>(), It.IsAny<Expression<Func<ApiScopeData, bool>>>()))
                .ReturnsAsync(true);

            // When
            ApiScopeService.UpsertApiScopeAsync(apiScopeInputModel).GetAwaiter().GetResult();

            // Then
            ApiScopeDataAccessMock.Verify(dataAccess => dataAccess.ReplaceAsync(It.IsAny<ApiScopeData>(), It.IsAny<Expression<Func<ApiScopeData, bool>>>()), Times.Once);
            ApiScopeDataAccessMock.Verify(dataAccess => dataAccess.InsertAsync(It.IsAny<ApiScopeData>()), Times.Never);
        }

        [Test]
        public void GivenItHasApiScope_WhenitCallsEnabled_AndApiScopeIsDesabled_ThenShouldEnable()
        {
            // Given
            var id = "1";
            var apiScopeData = new ApiScopeData(id, name: "ApiScope", displayName: "Api Scope", description: "Api Scope Description", enabled: true);

            ApiScopeDataAccessMock
                .Setup(dataAccess => dataAccess.GetByField(It.IsAny<Expression<Func<ApiScopeData, string>>>(), It.IsAny<string>()))
                .ReturnsAsync(apiScopeData);

            ApiScopeDataAccessMock
                .Setup(dataAccess => dataAccess.UpdateAsync(It.IsAny<Expression<Func<ApiScopeData, string>>>(), It.IsAny<string>(), It.IsAny<UpdateDefinition<ApiScopeData>>()))
                .ReturnsAsync(true);

            // When
            var apiScope = ApiScopeService.EnableApiScopeAsync(id).GetAwaiter().GetResult();

            // Then
            ApiScopeDataAccessMock.Verify(dataAccess => dataAccess.GetByField(It.IsAny<Expression<Func<ApiScopeData, string>>>(), It.IsAny<string>()), Times.Once);
            ApiScopeDataAccessMock.Verify(dataAccess => dataAccess.UpdateAsync(It.IsAny<Expression<Func<ApiScopeData, string>>>(), It.IsAny<string>(), It.IsAny<UpdateDefinition<ApiScopeData>>()), Times.Once);
            Assert.IsTrue(CheckApiScopeAndApiScopeData(apiScopeData, apiScope));
        }

        [TestCase]
        public void GivenItHasAnEmail_WhenICallUpdateUser_AndTheEmailAlreadyExists_ThenShouldThrowAlreadyExistsException()
        {
            // Given
            var apiScopeInputModel = new ApiScopeInputModel()
            {
                Id = "1",
                Name = "Name",
                DisplayName = "DisplayName"
            };

            ApiScopeDataAccessMock
                .Setup(dataAccess => dataAccess.ReplaceAsync(It.IsAny<ApiScopeData>(), It.IsAny<Expression<Func<ApiScopeData, bool>>>()))
                .Throws(MongoWriteException);

            // When
            Assert.Throws(typeof(AlreadyExistsException), () => ApiScopeService.UpsertApiScopeAsync(apiScopeInputModel).GetAwaiter().GetResult());
        }

        [TestCase]
        public void GivenItHasAnEmail_WhenICallInserUser_AndTheEmailAlreadyExists_ThenShouldThrowAlreadyExistsException()
        {
            // Given
            var apiScopeInputModel = new ApiScopeInputModel()
            {
                Name = "Name",
                DisplayName = "DisplayName"
            };

            ApiScopeDataAccessMock
                .Setup(dataAccess => dataAccess.InsertAsync(It.IsAny<ApiScopeData>()))
                .Throws(MongoWriteException);

            // When
            Assert.Throws(typeof(AlreadyExistsException), () => ApiScopeService.UpsertApiScopeAsync(apiScopeInputModel).GetAwaiter().GetResult());
        }

        private bool CheckApiScopeAndApiScopeData(ApiScopeData data, IdentityApiScope apiScope)
        {
            return
                data != null
                &&
                apiScope != null
                &&
                data.Name == apiScope.Name
                &&
                data.DisplayName == apiScope.DisplayName
                &&
                data.Description == apiScope.Description
                &&
                data.Enabled == apiScope.Enabled;
        }
    }
}