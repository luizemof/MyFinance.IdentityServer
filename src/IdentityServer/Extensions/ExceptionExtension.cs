using System;
using IdentityServer.Exceptions;

namespace IdentityServer.Exceptions
{
    public static class ValidationExceptionExtension
    {
        public static void AddValidation(this ValidationException validationException, Func<bool> verifyNotValid, string key, string message)
        {
            if (verifyNotValid())
                validationException.ModelStateDictionary.AddModelError(key, message);
        }

        public static void ThrowIfHasError(this ValidationException validationException)
        {
            if (validationException.ModelStateDictionary.Count > 0)
                throw validationException;
        }
    }
}

namespace MongoDB.Driver
{
    public static class MongoDriveExtensions
    {
        public static void ThrowIfDuplicateKey(this MongoWriteException ex, string fieldKey, string errorMessage)
        {
            if (ex.WriteError != null && ex.WriteError.Category == ServerErrorCategory.DuplicateKey && ex.WriteError.Code == 11000)
            {
                var alreadyExistsException = new AlreadyExistsException();
                alreadyExistsException.ModelStateDictionary.AddModelError(fieldKey, errorMessage);
                throw alreadyExistsException;
            }
        }
    }
}