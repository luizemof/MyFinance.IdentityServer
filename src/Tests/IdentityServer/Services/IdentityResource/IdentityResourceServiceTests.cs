using System.Collections.Generic;
using System.Linq;
using IdentityServer.Models.IdentityResource;
using IdentityServer.Repository.IdentityResource;
using IdentityServer.Services.IdentityResource;
using Moq;
using NUnit.Framework;

namespace IdentityServer.Tests.Services.IdentityResource
{
    [TestFixture]
    public class IdentityResourceServiceTests
    {
        private IdentityResourceService IdentityResourceService;
        private Mock<IIdentityResourceDataAccess> IdentityResourceDataAccessMock;

        [SetUp]
        public void Setup()
        {
            IdentityResourceDataAccessMock = new Mock<IIdentityResourceDataAccess>();
            IdentityResourceService = new IdentityResourceService(IdentityResourceDataAccessMock.Object);
        }

        [Test]
        public void GivenItHasIdentityResource_WhenItCallsGetAllIdentityResource_ThenShouldReturnAll()
        {
            // Given
            var datas = new []
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
            Assert.IsTrue(datas.All(data => identities.Any(model =>  CheckDataAndIdentityModel(data, model))));
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