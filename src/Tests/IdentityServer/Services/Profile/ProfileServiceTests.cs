using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using IdentityModel;
using IdentityServer.Attributes;
using IdentityServer.Models.Users;
using IdentityServer.Services;
using IdentityServer.Services.Profile;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace IdentityServer.Tests.Services.Profile
{
    [TestFixture]
    public class ProfileServiceTests
    {
        private ProfileService ProfileService;
        private Mock<ILogger<DefaultProfileService>> LoggerMock;
        private Mock<IUserService> UserServiceMock;

        [SetUp]
        public void Setup()
        {
            this.LoggerMock = new Mock<ILogger<DefaultProfileService>>();
            this.UserServiceMock = new Mock<IUserService>();
            this.ProfileService = new ProfileService(this.LoggerMock.Object, this.UserServiceMock.Object);
        }


        [Test]
        public void WhenCallGetProfile_ThenShouldAddTheUserClaims()
        {
            // Arrange
            var id = "1";
            var userModel = new UserModel(id, "name", "email", "password", true, new[] { "Role 1", "Role 2", "Role 3" });
            var identity = new ClaimsIdentity(new[] { new Claim(JwtClaimTypes.Subject, id) });
            var profileDataRequestContext = new ProfileDataRequestContext()
            {
                Subject = new ClaimsPrincipal(identity)
            };

            this.UserServiceMock.Setup(userSevice => userSevice.GetUserAsync(id)).ReturnsAsync(userModel);

            // Act
            this.ProfileService.GetProfileDataAsync(profileDataRequestContext).GetAwaiter().GetResult();

            // Assert
            this.UserServiceMock.Verify(userSevice => userSevice.GetUserAsync(id), Times.Once);
            Assert.AreEqual(expected: 7, profileDataRequestContext.IssuedClaims?.Count);
            AssertClaims(profileDataRequestContext.IssuedClaims, userModel);
        }

        private void AssertClaims(List<Claim> claims, UserModel userModel)
        {
            var profileProperties = userModel.GetType().GetProperties()?.Where(prop => Attribute.IsDefined(prop, typeof(ProfileAttribute)));

            if (profileProperties?.Count() > 0)
            {
                foreach (var property in profileProperties)
                {
                    var attribute = property.GetCustomAttributes(typeof(ProfileAttribute), true).FirstOrDefault() as ProfileAttribute;
                    Assert.IsTrue(claims.Any(c => c.Type == attribute.Name), $"{attribute.Name} not found in Claims");
                }
            }
        }
    }
}