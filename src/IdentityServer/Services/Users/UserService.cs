using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer.Cryptography;
using IdentityServer.Exceptions;
using IdentityServer.Models.Users;
using IdentityServer.Repository.Users;
using MongoDB.Bson;
using MongoDB.Driver;

namespace IdentityServer.Services.Users
{
    public class UserService : IUserService
    {
        private readonly IUserDataAccess UserDataAccess;
        private readonly IdentityServerCryptography IdentityServerCryptography; 

        public UserService(IUserDataAccess userDataAccess, IdentityServerCryptography identityServerCryptography)
        {
            UserDataAccess = userDataAccess ?? throw new ArgumentNullException(nameof(userDataAccess));
            IdentityServerCryptography = identityServerCryptography ?? throw new ArgumentNullException(nameof(identityServerCryptography));
        }

        public async Task<UserModel> CreateUserAsync(UserInputModel user)
        {
            try
            {
                UserValidation.InsertValidation(user);
                var id = ObjectId.GenerateNewId().ToString();
                var password = IdentityServerCryptography.Encrypt(user.Password);
                var userData = new UserData(id, user.Name, user.Email, password);
                await UserDataAccess.InsertAsync(userData);

                var createdUserData = (await UserDataAccess.GetAsync(id)).Single();
                return new UserModel(createdUserData.Id, createdUserData.Name, createdUserData.Email, createdUserData.IsActive);
            }
            catch (MongoWriteException ex)
            {
                ex.ThrowIfDuplicateKey(nameof(user.Email), $"O email '{user.Email}' já existe.");;
                throw new Exception("An error occours when create a user.");
            }
        }

        public async Task<IEnumerable<UserModel>> GetInactiveUsersAsync()
        {
            var usersData = (await UserDataAccess.GetAsync(isActive: false));
            return FromUserDataToUser(usersData);
        }

        public Task<UserModel> GetUserAsync(string id)
        {
            return GetUser(id, isActive: true);
        }

        public async Task<IEnumerable<UserModel>> GetUsersAsync()
        {
            var users = await UserDataAccess.GetAsync();
            return FromUserDataToUser(users);
        }

        public Task<UserModel> DeactiveUserAsync(string id)
        {
            return UserActivate(id, isActive: false);
        }

        public Task<UserModel> ReactiveUserAsync(string id)
        {
            return UserActivate(id, isActive: true);
        }

        public async Task<UserModel> UpdateUserAsync(UserInputModel userInputModel)
        {
            try
            {
                UserValidation.UpdateValidaton(userInputModel);
                var userData = new UserData(userInputModel.Id, userInputModel.Name, userInputModel.Email, userInputModel.Password);
                var updatedUserData = await UserDataAccess.ReplaceAsync(userData);
                return FromUserDataToUserAPI(updatedUserData);
            }
            catch (MongoWriteException ex)
            {
                ex.ThrowIfDuplicateKey(nameof(userInputModel.Email), $"O email '{userInputModel.Email}' já existe.");

                throw new Exception("An error occours when update a user.");
            }
        }

        private async Task<UserModel> GetUser(string id, bool? isActive = true)
        {
            var userData = (await UserDataAccess.GetAsync(id, isActive))?.SingleOrDefault();
            return FromUserDataToUserAPI(userData);
        }

        private IEnumerable<UserModel> FromUserDataToUser(IEnumerable<UserData> usersData)
        {
            return usersData.Select(FromUserDataToUserAPI).Where(user => user != UserModel.Empty).ToList();
        }

        public static UserModel FromUserDataToUserAPI(UserData userData)
        {
            return userData != null ? new UserModel(userData.Id, userData.Name, userData.Email, userData.IsActive) : UserModel.Empty;
        }

        private async Task<UserModel> UserActivate(string id, bool isActive)
        {
            var updateDefinitionBuilder = new UpdateDefinitionBuilder<UserData>();
            var updateOption = updateDefinitionBuilder.Set(userData => userData.IsActive, isActive);
            var updated = await UserDataAccess.UpdateAsync(id, updateOption);

            return updated ? await GetUser(id, isActive) : UserModel.Empty;
        }
    }
}