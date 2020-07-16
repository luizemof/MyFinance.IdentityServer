using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer.Models.ApiScope;
using IdentityServer.Repository.ApiScopes;
using IdentityServer.Services;
using IdentityServer.Services.ApiScope;
using MongoDB.Driver;
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

        [SetUp]
        public void Setup()
        {
            ApiScopeDataAccessMock = new Mock<IApiScopeDataAccess>();
            ApiScopeService = new ApiScopeService(ApiScopeDataAccessMock.Object);
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
            ApiScopeDataAccessMock.Verify(dataAccess => dataAccess.ReplaceAsync(It.IsAny<ApiScopeData>()), Times.Never);
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
                .Setup(apiScopeDataAccess => apiScopeDataAccess.ReplaceAsync(It.IsAny<ApiScopeData>()))
                .Returns(Task.CompletedTask);

            // When
            ApiScopeService.UpsertApiScopeAsync(apiScopeInputModel).GetAwaiter().GetResult();

            // Then
            ApiScopeDataAccessMock.Verify(dataAccess => dataAccess.ReplaceAsync(It.IsAny<ApiScopeData>()), Times.Once);
            ApiScopeDataAccessMock.Verify(dataAccess => dataAccess.InsertAsync(It.IsAny<ApiScopeData>()), Times.Never);
        }

        [Test]
        public void GivenItHasApiScope_WhenitCallsEnabled_AndApiScopeIsDesabled_ThenShouldEnable()
        {
            // Given
            var id = "1";
            var apiScopeData = new ApiScopeData(id, name: "ApiScope", displayName: "Api Scope", description: "Api Scope Description", enabled: true);
            Func<string, bool> checkId = (dataId) => dataId == id;

            ApiScopeDataAccessMock
                .Setup(dataAccess => dataAccess.GetAsync(It.IsAny<FilterDefinition<ApiScopeData>>()))
                .ReturnsAsync(new[] { apiScopeData }.AsEnumerable());

            ApiScopeDataAccessMock
                .Setup(dataAccess => dataAccess.UpdateAsync(It.Is<string>(dataId => checkId(dataId)), It.IsAny<UpdateDefinition<ApiScopeData>>()))
                .ReturnsAsync(true);

            // When
            var apiScope = ApiScopeService.EnableApiScopeAsync(id).GetAwaiter().GetResult();

            // Then
            ApiScopeDataAccessMock.Verify(dataAccess => dataAccess.GetAsync(It.IsAny<FilterDefinition<ApiScopeData>>()), Times.Once);
            ApiScopeDataAccessMock.Verify(dataAccess => dataAccess.UpdateAsync(It.Is<string>(dataId => checkId(dataId)), It.IsAny<UpdateDefinition<ApiScopeData>>()), Times.Once);
            Assert.IsTrue(CheckApiScopeAndApiScopeData(apiScopeData, apiScope));
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