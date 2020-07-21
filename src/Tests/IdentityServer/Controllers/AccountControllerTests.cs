using System;
using IdentityServer.Constants;
using IdentityServer.Controllers;
using IdentityServer.Controllers.Account;
using IdentityServer.Models.Account;
using IdentityServer.Services;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Moq;
using NUnit.Framework;

namespace IdentityServer.Tests.Controllers
{
    [TestFixture]
    public class AccountControllerTests
    {
        private AccountController AccountController;

        private Mock<IAccountService> AccountServiceMock;

        private Mock<IUserService> UserServiceMock;

        private Mock<IIdentityServerInteractionService> InteractionServiceMock;

        private Mock<IHttpContextWrapper> HttpContextMock;

        private Mock<IUrlHelper> UlrHelperMock;

        [SetUp]
        public void Setup()
        {
            UlrHelperMock = new Mock<IUrlHelper>();
            HttpContextMock = new Mock<IHttpContextWrapper>();
            AccountServiceMock = new Mock<IAccountService>();
            UserServiceMock = new Mock<IUserService>();
            InteractionServiceMock = new Mock<IIdentityServerInteractionService>();

            AccountController = new AccountController(AccountServiceMock.Object, UserServiceMock.Object, InteractionServiceMock.Object, HttpContextMock.Object);
            AccountController.Url = UlrHelperMock.Object;
        }

        [Test]
        public void WhenCallLogin_AndModelIsNotValid_ThenShouldReturnLoginView()
        {
            // Act
            AccountController.ModelState.AddModelError("SomeErrorKey", "SomeError");
            var viewResult = AccountController.Login(new LoginModel()).GetAwaiter().GetResult() as ViewResult;
            var model = viewResult?.Model as LoginModel;

            // Assert
            Assert.IsNotNull(viewResult);
            Assert.AreEqual(expected: ControllerConstants.LOGIN, viewResult.ViewName);

            Assert.IsNotNull(model);
        }

        [Test]
        public void WhenCallLogin_AndModelIsValid_AndURLIsLocal_ThenShouldReturnLoginView()
        {
            // Arrange
            var url = "http://localhost:5001/UrlToRedirect";
            var loginModel = new LoginModel() { RedirectURL = url };

            UserServiceMock.Setup(service => service.GetUserByEmail(It.IsAny<string>())).ReturnsAsync(new Models.Users.UserModel("1", "Name", "email", true));
            AccountServiceMock.Setup(service => service.ValidateCredentials(It.IsAny<LoginModel>())).ReturnsAsync(true);
            UlrHelperMock.Setup(url => url.IsLocalUrl(It.IsAny<string>())).Returns(true);

            // Act
            var viewResult = AccountController.Login(loginModel).GetAwaiter().GetResult() as RedirectResult;

            // Assert
            Assert.IsNotNull(viewResult);
            Assert.AreEqual(expected: url, viewResult.Url);
            UserServiceMock.Verify(service => service.GetUserByEmail(It.IsAny<string>()), Times.Once);
            AccountServiceMock.Verify(service => service.ValidateCredentials(It.IsAny<LoginModel>()), Times.Once);
            UlrHelperMock.Verify(url => url.IsLocalUrl(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void WhenCallLogin_AndModelIsValid_AndInteractContextIsValid_ThenShouldReturnLoginView()
        {
            // Arrange
            var url = "http://localhost:5001/UrlToRedirect";
            var loginModel = new LoginModel() { RedirectURL = url };

            UserServiceMock.Setup(service => service.GetUserByEmail(It.IsAny<string>())).ReturnsAsync(new Models.Users.UserModel("1", "Name", "email", true));
            AccountServiceMock.Setup(service => service.ValidateCredentials(It.IsAny<LoginModel>())).ReturnsAsync(true);
            InteractionServiceMock.Setup(interact => interact.GetAuthorizationContextAsync(It.IsAny<string>())).ReturnsAsync(new AuthorizationRequest());

            // Act
            var viewResult = AccountController.Login(loginModel).GetAwaiter().GetResult() as RedirectResult;

            // Assert
            Assert.IsNotNull(viewResult);
            Assert.AreEqual(expected: url, viewResult.Url);
            UserServiceMock.Verify(service => service.GetUserByEmail(It.IsAny<string>()), Times.Once);
            AccountServiceMock.Verify(service => service.ValidateCredentials(It.IsAny<LoginModel>()), Times.Once);
            UlrHelperMock.Verify(url => url.IsLocalUrl(It.IsAny<string>()), Times.Never);
            InteractionServiceMock.Verify(interact => interact.GetAuthorizationContextAsync(It.IsAny<string>()), Times.Once);
        }
        
        [Test]
        public void WhenCallLogin_AndModelIsValid_AndRedirectURLISEmpty_ThenShouldReturnLoginView()
        {
            // Arrange
            var loginModel = new LoginModel();

            UserServiceMock.Setup(service => service.GetUserByEmail(It.IsAny<string>())).ReturnsAsync(new Models.Users.UserModel("1", "Name", "email", true));
            AccountServiceMock.Setup(service => service.ValidateCredentials(It.IsAny<LoginModel>())).ReturnsAsync(true);
            UlrHelperMock.Setup(url => url.IsLocalUrl(It.IsAny<string>())).Returns(false);

            // Act
            var viewResult = AccountController.Login(loginModel).GetAwaiter().GetResult() as RedirectResult;

            // Assert
            Assert.IsNotNull(viewResult);
            Assert.IsFalse(string.IsNullOrWhiteSpace(viewResult.Url));
            UserServiceMock.Verify(service => service.GetUserByEmail(It.IsAny<string>()), Times.Once);
            AccountServiceMock.Verify(service => service.ValidateCredentials(It.IsAny<LoginModel>()), Times.Once);
            UlrHelperMock.Verify(url => url.IsLocalUrl(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void WhenCallLogin_AndModelIsValid_AndURLIsMalicious_ThenShouldThrowException()
        {
            // Arrange
            var url = "http://localhost:5001/UrlToRedirect";
            var loginModel = new LoginModel() { RedirectURL = url };

            UserServiceMock.Setup(service => service.GetUserByEmail(It.IsAny<string>())).ReturnsAsync(new Models.Users.UserModel("1", "Name", "email", true));
            AccountServiceMock.Setup(service => service.ValidateCredentials(It.IsAny<LoginModel>())).ReturnsAsync(true);
            UlrHelperMock.Setup(url => url.IsLocalUrl(It.IsAny<string>())).Returns(false);

            // Act & Assert
            Assert.Throws(typeof(Exception), () => AccountController.Login(loginModel).GetAwaiter().GetResult());
            UserServiceMock.Verify(service => service.GetUserByEmail(It.IsAny<string>()), Times.Once);
            AccountServiceMock.Verify(service => service.ValidateCredentials(It.IsAny<LoginModel>()), Times.Once);
            UlrHelperMock.Verify(url => url.IsLocalUrl(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void WhenCallLogin_AndModelIsValid_ButCredentialsIsInvalid_ThenShouldReturnLoginView()
        {
            // Act
            var viewResult = AccountController.Login(new LoginModel()).GetAwaiter().GetResult() as ViewResult;
            var model = viewResult?.Model as LoginModel;

            // Assert
            Assert.IsNotNull(viewResult);
            Assert.AreEqual(expected: ControllerConstants.LOGIN, viewResult.ViewName);

            Assert.IsNotNull(model);
        }
    }
}