using System;

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