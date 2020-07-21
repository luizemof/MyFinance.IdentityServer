using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using IdentityServer.Exceptions;
using IdentityServer.Models.Users;
using IdentityServer.Repository.Users;
using IdentityServer.Services.Users;
using IdentityServer.Tests.Cryptography;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Clusters;
using MongoDB.Driver.Core.Connections;
using MongoDB.Driver.Core.Servers;
using Moq;
using NUnit.Framework;

namespace IdentityServer.Tests.Services.Users
{
    [TestFixture]
    public class UserServiceTests
    {
        private Mock<IUserDataAccess> UserDataAccessMock;
        private MongoWriteException MongoWriteException;
        private UserService UserService;

        [SetUp]
        public void Setup()
        {
            UserDataAccessMock = new Mock<IUserDataAccess>();

            var connectionId = new ConnectionId(new ServerId(new ClusterId(1), new DnsEndPoint("localhost", 27017)), 2);
            var writeConcernErrorConstructor = typeof(WriteError).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)[0];
            var writeError = (WriteError)writeConcernErrorConstructor.Invoke(new object[] { ServerErrorCategory.DuplicateKey, 11000, "writeError", new BsonDocument("details", "writeError") });
            
            MongoWriteException = new MongoWriteException(connectionId, writeError, null, null);
            
            UserService = new UserService(UserDataAccessMock.Object, IdentityServerCryptographyTests.TestIdentityServerCryptography);
        }

        [TestCase]
        public void GivenItHasntAnUser_WhenICallCreateUser_TheShouldCreateANewUser()
        {
            // Given
            Func<string, bool> checkGetAsyncInputMock = id => !string.IsNullOrEmpty(id);
            IEnumerable<UserData> users = new[] { new UserData(string.Empty, string.Empty, string.Empty, string.Empty) };

            UserDataAccessMock
                .Setup(userDataAccess => userDataAccess.GetAsync(It.Is<string>(id => checkGetAsyncInputMock(id)), It.IsAny<bool>()))
                .Returns(Task.FromResult(users));

            // When
            var userData = UserService.CreateUserAsync(new UserInputModel() { Password = string.Empty, PasswordConfirmation = string.Empty }).GetAwaiter().GetResult();

            // Then
            Assert.IsFalse(userData == UserModel.Empty);
            UserDataAccessMock.Verify(userDataAccess => userDataAccess.InsertAsync(It.IsAny<UserData>()), Times.Once);
            UserDataAccessMock.Verify(userDataAccess => userDataAccess.GetAsync(It.Is<string>(id => checkGetAsyncInputMock(id)), It.IsAny<bool>()), Times.Once);
        }

        [TestCase]
        public void GivenItHasAnEmail_WhenICallCreateUser_ThenShouldThrowAlreadyExistsException()
        {
            // Given
            var createUserRequest = new UserInputModel()
            {
                Password = string.Empty,
                PasswordConfirmation = string.Empty
            };

            UserDataAccessMock
                .Setup(userDataAccess => userDataAccess.InsertAsync(It.IsAny<UserData>()))
                .Throws(MongoWriteException);

            // When
            Assert.Throws(typeof(AlreadyExistsException), () => UserService.CreateUserAsync(createUserRequest).GetAwaiter().GetResult());
        }

        [TestCase]
        public void GivenItHasAnUserId_WhenICallGetUser_ThenShouldReturn()
        {
            // Given
            var userId = "1";
            var name = "name";
            var email = "email";
            
            Func<string, bool> checkId = (id) => id == userId;
            IEnumerable<UserData> users = new[] { new UserData(userId, name, email, null) };

            UserDataAccessMock
                .Setup(dataAccess => dataAccess.GetAsync(It.Is<string>(id => checkId(id)), It.Is<bool>(isActive => isActive)))
                .Returns(Task.FromResult(users));

            // When
            var user = UserService.GetUserAsync(userId).GetAwaiter().GetResult();

            // Then
            Assert.IsFalse(user == UserModel.Empty);
            Assert.AreEqual(userId, user.Id);
            Assert.AreEqual(name, user.Name);
            Assert.AreEqual(email, user.Email);
            Assert.IsTrue(user.IsActive);
        }

        [TestCase]
        public void GivenItHasUsers_WhenICallGetUser_AndNotFound_ThenShouldReturnNull()
        {
            // Given
            string userId = "1";
            string name = "name";
            string email = "email";

            IEnumerable<UserData> users = new[] { new UserData(userId, name, email, null) };
            Func<string, bool> checkId = (id) => id != userId;
            var getAsyncMockInput = It.Is<string>(id => checkId(id));

            UserDataAccessMock
                .Setup(dataAccess => dataAccess.GetAsync(getAsyncMockInput, It.Is<bool>(isActive => isActive)))
                .Returns(Task.FromResult(users));

            // When
            var user = UserService.GetUserAsync(userId).GetAwaiter().GetResult();

            // Then
            Assert.IsTrue(user == UserModel.Empty);
        }

        [TestCase]
        public void GivenItHasInactiveUsers_WhenICallGetInactiveUsers_ThenShouldReturnAllUsersInactive()
        {
            // When
            var users = UserService.GetInactiveUsersAsync().GetAwaiter().GetResult();

            // Then
            Assert.IsNotNull(users);
            UserDataAccessMock.Verify(userDataAccess => userDataAccess.GetAsync(It.Is<string>(id => string.IsNullOrWhiteSpace(id)), It.Is<bool>(isActive => !isActive)), Times.Once);
        }

        [TestCase]
        public void GivenItHasActiveUser_WhenICallInactiveUser_ThenShouldInactivate()
        {
            // Given
            var userId = "1";
            var userData = new UserData(string.Empty, string.Empty);
            
            IEnumerable<UserData> users = new[] { userData };
            
            Func<string, bool> checkIdUpdateInput = id => id == userId;
            Func<bool, bool> checkIsActiveGetInput = isActive => !isActive;
            
            UserDataAccessMock
                .Setup(userDataAccess => userDataAccess.UpdateAsync(It.Is<string>(id => checkIdUpdateInput(id)), It.IsAny<UpdateDefinition<UserData>>()))
                .Returns(Task.FromResult(true));

            UserDataAccessMock
                .Setup(userDataAccess => userDataAccess.GetAsync(It.Is<string>(id => checkIdUpdateInput(id)), It.Is<bool>(isActive => checkIsActiveGetInput(isActive))))
                .Returns(Task.FromResult(users));

            // When
            var user = UserService.DeactiveUserAsync(userId).GetAwaiter().GetResult();

            // Then
            Assert.IsFalse(user == UserModel.Empty);
            UserDataAccessMock.Verify(userDataAccess => userDataAccess.UpdateAsync(It.Is<string>(id => checkIdUpdateInput(id)), It.IsAny<UpdateDefinition<UserData>>()), Times.Once);
            UserDataAccessMock.Verify(userDataAccess => userDataAccess.GetAsync(It.Is<string>(id => checkIdUpdateInput(id)), It.Is<bool>(isActive => checkIsActiveGetInput(isActive))), Times.Once);
        }

        [TestCase]
        public void GivenItHasntUser_WhenICallInactiveUser_ThenShouldReturnEmpty()
        {
            // Given
            string userId = "1";
            Func<string, bool> checkIdUpdateInput = id => id == userId;
            UserDataAccessMock
                .Setup(userDataAccess => userDataAccess.UpdateAsync(It.Is<string>(id => checkIdUpdateInput(id)), It.IsAny<UpdateDefinition<UserData>>()))
                .Returns(Task.FromResult(false));

            // When
            var user = UserService.DeactiveUserAsync(userId).GetAwaiter().GetResult();

            // Then
            Assert.IsTrue(user == UserModel.Empty);
            UserDataAccessMock.Verify(userDataAccess => userDataAccess.UpdateAsync(It.Is<string>(id => checkIdUpdateInput(id)), It.IsAny<UpdateDefinition<UserData>>()), Times.Once);
            UserDataAccessMock.Verify(userDataAccess => userDataAccess.GetAsync(It.IsAny<string>(), It.IsAny<bool>()), Times.Never);
        }

        [TestCase]
        public void GivenItHasInactiveUser_WhenICallReactiveUser_ThenUserShouldBeActivat()
        {
            // Given
            var userId = "1";
            var userData = new UserData(string.Empty, string.Empty);
            
            IEnumerable<UserData> users = new[] { userData };
            
            Func<string, bool> checkIdUpdateInput = id => id == userId;
            Func<bool, bool> checkIsActiveGetInput = isActive => isActive;
            
            UserDataAccessMock
                .Setup(userDataAccess => userDataAccess.UpdateAsync(It.Is<string>(id => checkIdUpdateInput(id)), It.IsAny<UpdateDefinition<UserData>>()))
                .Returns(Task.FromResult(true));

            UserDataAccessMock
                .Setup(userDataAccess => userDataAccess.GetAsync(It.Is<string>(id => checkIdUpdateInput(id)), It.Is<bool>(isActive => checkIsActiveGetInput(isActive))))
                .Returns(Task.FromResult(users));

            // When
            var user = UserService.ReactiveUserAsync(userId).GetAwaiter().GetResult();

            // Then
            Assert.IsFalse(user == UserModel.Empty);
            UserDataAccessMock.Verify(userDataAccess => userDataAccess.UpdateAsync(It.Is<string>(id => checkIdUpdateInput(id)), It.IsAny<UpdateDefinition<UserData>>()), Times.Once);
            UserDataAccessMock.Verify(userDataAccess => userDataAccess.GetAsync(It.Is<string>(id => checkIdUpdateInput(id)), It.Is<bool>(isActive => checkIsActiveGetInput(isActive))), Times.Once);
        }

        [TestCase]
        public void GivenItHasntUser_WhenICallReactiveUser_ThenShouldReturnEmpty()
        {
            // Given
            string userId = "1";
            Func<string, bool> checkIdUpdateInput = id => id == userId;
            UserDataAccessMock
                .Setup(userDataAccess => userDataAccess.UpdateAsync(It.Is<string>(id => checkIdUpdateInput(id)), It.IsAny<UpdateDefinition<UserData>>()))
                .Returns(Task.FromResult(false));

            // When
            var user = UserService.ReactiveUserAsync(userId).GetAwaiter().GetResult();

            // Then
            Assert.IsTrue(user == UserModel.Empty);
            UserDataAccessMock.Verify(userDataAccess => userDataAccess.UpdateAsync(It.Is<string>(id => checkIdUpdateInput(id)), It.IsAny<UpdateDefinition<UserData>>()), Times.Once);
            UserDataAccessMock.Verify(userDataAccess => userDataAccess.GetAsync(It.IsAny<string>(), It.IsAny<bool>()), Times.Never);
        }

        [TestCase]
        public void GivenItHasAUser_WhenICallUpdateUser_ThenShouldReturnUpdatedUser()
        {
            // Given
            var id = "1";
            var name = "name";
            var email = "email";
            var password = "password";
            var updatedUserRequest = new UserInputModel()
            {
                Id = id,
                Name = name,
                Email = email,
                Password = password,
                PasswordConfirmation = password
            };

            Func<UserData, bool> checkUpdateUserInput = userData => userData.Id == updatedUserRequest.Id
                                                                    &&
                                                                    userData.Name == updatedUserRequest.Name
                                                                    &&
                                                                    userData.Password == updatedUserRequest.Password;

            UserDataAccessMock
                .Setup(userDataAccess => userDataAccess.ReplaceAsync(It.Is<UserData>(userData => checkUpdateUserInput(userData))))
                .Returns(Task.FromResult(new UserData(id, name, email, password)));

            // When
            var updatedUser = UserService.UpdateUserAsync(updatedUserRequest).GetAwaiter().GetResult();

            // Then
            Assert.IsFalse(updatedUser == UserModel.Empty);
            UserDataAccessMock.Verify(userDataAccess => userDataAccess.ReplaceAsync(It.Is<UserData>(userData => checkUpdateUserInput(userData))), Times.Once);
        }

        [TestCase]
        public void GivenItHasAnEmail_WhenICallUpdateUser_AndTheEmailAlreadyExists_ThenShouldThrowAlreadyExistsException()
        {
            // Given
            var updateUserRequest = new UserInputModel()
            {
                Id = "1",
                Password = string.Empty,
                PasswordConfirmation = string.Empty
            };

            UserDataAccessMock
                .Setup(userDataAccess => userDataAccess.ReplaceAsync(It.IsAny<UserData>()))
                .Throws(MongoWriteException);

            // When
            Assert.Throws(typeof(AlreadyExistsException), () => UserService.UpdateUserAsync(updateUserRequest).GetAwaiter().GetResult());
        }

        [TestCase]
        public void GivenItHasAnUser_WhenICallGetUserByEmail_ThenShouldReturnUser()
        {
            // Given
            this.UserDataAccessMock
                .Setup(dataAccess => dataAccess.GetByField(It.IsAny<Expression<Func<UserData, string>>>(), It.IsAny<string>()))
                .ReturnsAsync(new UserData(name: "name", email: "email"));

            // When
            var userModel = this.UserService.GetUserByEmail(email: "email").GetAwaiter().GetResult();

            // Then
            Assert.IsNotNull(userModel);
            this.UserDataAccessMock.Verify(dataAccess => dataAccess.GetByField(It.IsAny<Expression<Func<UserData, string>>>(), It.IsAny<string>()), Times.Once);
        }
    }
}