using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer.Cryptography;
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

        public async Task CreateUserAsync(UserInputModel user)
        {
            try
            {
                UserValidation.InsertValidation(user);
                var userData = user.ToData(IdentityServerCryptography);
                await UserDataAccess.InsertAsync(userData);
            }
            catch (MongoWriteException ex)
            {
                ex.ThrowIfDuplicateKey(nameof(user.Email), $"O email '{user.Email}' já existe."); ;
                throw new Exception("An error occours when create a user.");
            }
        }

        public async Task<IEnumerable<UserModel>> GetInactiveUsersAsync()
        {
            var usersData = (await UserDataAccess.GetAsync(isActive: false));
            return FromUserDataToUserModel(usersData);
        }

        public Task<UserModel> GetUserAsync(string id)
        {
            return GetUser(id, isActive: true);
        }

        public async Task<IEnumerable<UserModel>> GetUsersAsync()
        {
            var users = await UserDataAccess.GetAsync();
            return FromUserDataToUserModel(users);
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
                var userData = userInputModel.ToData(this.IdentityServerCryptography);
                var updatedUserData = await this.UserDataAccess.ReplaceAsync(userData);
                return updatedUserData.ToModel(this.IdentityServerCryptography);
            }
            catch (MongoWriteException ex)
            {
                ex.ThrowIfDuplicateKey(nameof(userInputModel.Email), $"O email '{userInputModel.Email}' já existe.");

                throw new Exception("An error occours when update a user.");
            }
        }

        public async Task<UserModel> GetUserByEmail(string email)
        {
            var userData = await this.UserDataAccess.GetByField(userData => userData.Email, email);

            return userData.ToModel(this.IdentityServerCryptography);
        }

        private async Task<UserModel> GetUser(string id, bool? isActive = true)
        {
            var userData = (await UserDataAccess.GetAsync(id, isActive))?.SingleOrDefault();
            return userData.ToModel(this.IdentityServerCryptography);
        }

        private IEnumerable<UserModel> FromUserDataToUserModel(IEnumerable<UserData> usersData)
        {
            return usersData.Select(data => data.ToModel(this.IdentityServerCryptography)).Where(user => user != UserModel.Empty).ToList();
        }

        private async Task<UserModel> UserActivate(string id, bool isActive)
        {
            var updateDefinitionBuilder = new UpdateDefinitionBuilder<UserData>();
            var updateOption = updateDefinitionBuilder.Set(userData => userData.IsActive, isActive);
            var updated = await UserDataAccess.UpdateAsync(id, updateOption);

            return updated ? await GetUser(id, isActive) : UserModel.Empty;
        }

        public async Task<IEnumerable<string>> GetRolesAsync()
        {
            var filter = Builders<UserData>.Filter.Where(data => data.Roles != null && data.Roles.Count() > 0);
            var roles = new List<string>();
            var users = await this.UserDataAccess.GetAsync(filter);
            foreach (var user in users)
                roles.AddRange(user.Roles);

            return roles.ToHashSet();
        }
    }
}