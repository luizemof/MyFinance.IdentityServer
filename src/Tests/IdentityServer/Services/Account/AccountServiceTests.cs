using System;
using System.Linq.Expressions;
using IdentityServer.Models.Account;
using IdentityServer.Repository.Users;
using IdentityServer.Services.Account;
using IdentityServer.Tests.Cryptography;
using Moq;
using NUnit.Framework;

namespace IdentityServer.Tests.Services.Account
{
    [TestFixture]
    public class AccountServiceTests
    {
        private AccountService AccountService;
        private Mock<IUserDataAccess> UserDataAccessMock;

        [SetUp]
        public void Setup()
        {
            this.UserDataAccessMock = new Mock<IUserDataAccess>();
            this.AccountService = new AccountService(this.UserDataAccessMock.Object, IdentityServerCryptographyTests.TestIdentityServerCryptography);
        }

        [Test]
        public void WhenCallValidateCredentials_AndCredentialsIsValid_ThenShouldReturnTrue()
        {
            // Assert
            var password = "c49nHayoXPOsZRI1NPkAIA==";
            var userData = new UserData(id: "1", name: "name", email: "email", password, isActive: true);
            this.UserDataAccessMock.Setup(dataAccess => dataAccess.GetByField(It.IsAny<Expression<Func<UserData, string>>>(), It.IsAny<string>())).ReturnsAsync(userData);

            // Act
            var isValid = this.AccountService.ValidateCredentials(new LoginModel() { Password = "123" }).GetAwaiter().GetResult();

            // Assert
            Assert.IsTrue(isValid);
            this.UserDataAccessMock.Verify(dataAccess => dataAccess.GetByField(It.IsAny<Expression<Func<UserData, string>>>(), It.IsAny<string>()), Times.Once);
        }
        
        [Test]
        public void WhenCallValidateCredentials_AndCredentialsIsNotValid_ThenShouldReturnFalse()
        {
            // Assert
            var password = "c49nHayoXPOsZRI1NPkAIA==";
            var userData = new UserData(id: "1", name: "name", email: "email", password, isActive: true);
            this.UserDataAccessMock.Setup(dataAccess => dataAccess.GetByField(It.IsAny<Expression<Func<UserData, string>>>(), It.IsAny<string>())).ReturnsAsync(userData);

            // Act
            var isValid = this.AccountService.ValidateCredentials(new LoginModel() { Password = "1234" }).GetAwaiter().GetResult();

            // Assert
            Assert.IsFalse(isValid);
            this.UserDataAccessMock.Verify(dataAccess => dataAccess.GetByField(It.IsAny<Expression<Func<UserData, string>>>(), It.IsAny<string>()), Times.Once);
        }
        
        [Test]
        public void WhenCallValidateCredentials_AndUserIsNotAcitve_ThenShouldReturnFalse()
        {
            // Assert
            var password = "c49nHayoXPOsZRI1NPkAIA==";
            var userData = new UserData(id: "1", name: "name", email: "email", password, isActive: false);
            this.UserDataAccessMock.Setup(dataAccess => dataAccess.GetByField(It.IsAny<Expression<Func<UserData, string>>>(), It.IsAny<string>())).ReturnsAsync(userData);

            // Act
            var isValid = this.AccountService.ValidateCredentials(new LoginModel() { Password = "123" }).GetAwaiter().GetResult();

            // Assert
            Assert.IsFalse(isValid);
            this.UserDataAccessMock.Verify(dataAccess => dataAccess.GetByField(It.IsAny<Expression<Func<UserData, string>>>(), It.IsAny<string>()), Times.Once);
        }
        
        [Test]
        public void WhenCallValidateCredentials_AndNotFoundUser_ThenShouldReturnFalse()
        {
            // Assert
            var password = "c49nHayoXPOsZRI1NPkAIA==";
            var userData = new UserData(id: "1", name: "name", email: "email", password, isActive: true);

            // Act
            var isValid = this.AccountService.ValidateCredentials(new LoginModel() { Password = "123" }).GetAwaiter().GetResult();

            // Assert
            Assert.IsFalse(isValid);
            this.UserDataAccessMock.Verify(dataAccess => dataAccess.GetByField(It.IsAny<Expression<Func<UserData, string>>>(), It.IsAny<string>()), Times.Once);
        }
    }
}