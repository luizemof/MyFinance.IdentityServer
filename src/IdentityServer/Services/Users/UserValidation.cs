using IdentityServer.Exceptions;
using IdentityServer.Models.Users;

namespace IdentityServer.Services.Users
{
    public static class UserValidation
    {
        public const string PasswordConfirmationKey = "PasswordConfirmation";
        private const string PasswordConfirmationMessage = "Password confirmation don't match";

        public const string MissingIdKey = "MissingId";
        private const string MissingIdMessage = "User Id is missing";

        public static void InsertValidation(UserInputModel createUserRequest)
        {
            var validationException = new ValidationException();
            validationException.AddValidation(() => !createUserRequest.Password.Equals(createUserRequest.PasswordConfirmation), PasswordConfirmationKey, PasswordConfirmationMessage);
            validationException.ThrowIfHasError();
        }

        public static void UpdateValidaton(UserInputModel updateUserRequest)
        {
            var validationException = new ValidationException();
            validationException.AddValidation(() => !updateUserRequest.Password.Equals(updateUserRequest.PasswordConfirmation), PasswordConfirmationKey, PasswordConfirmationMessage);
            validationException.AddValidation(() => string.IsNullOrWhiteSpace(updateUserRequest.Id), MissingIdKey, MissingIdMessage);
            validationException.ThrowIfHasError();
        }
    }
}