using System.Linq;
using System.Threading.Tasks;
using IdentityServer.Repository.ApiScopes;
using IdentityServer.Services;
using IdentityServer.Services.ApiScope;
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
            var apiScopes = ApiScopeService.GetAllScopesAsync().GetAwaiter().GetResult();

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
            var apiScope =  new IdentityApiScope(name: "API1", displayName: "API 1");

            ApiScopeDataAccessMock
                .Setup(apiScopeDataAccess => apiScopeDataAccess.InsertAsync(It.Is<ApiScopeData>(data => CheckApiScopeAndApiScopeData(data, apiScope))))
                .Returns(Task.CompletedTask);

            // When
            ApiScopeService.InsertApiScopeAsync(apiScope).GetAwaiter().GetResult();

            // Then
            ApiScopeDataAccessMock.Verify(dataAccess => dataAccess.InsertAsync(It.Is<ApiScopeData>(data => CheckApiScopeAndApiScopeData(data, apiScope))), Times.Once);
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