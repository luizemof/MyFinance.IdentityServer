using System.Collections.Generic;
using IdentityServer.Exceptions;
using IdentityServer.Models.Client;

namespace IdentityServer.Services.Client
{
    public static class ClientValidations
    {
        public const string ALLOWED_GRANT_TYPES_VALIDATION_MESSAGE = "It is required at least one Grant Type";

        public const string CLIENT_NAME_VALIDATION_MESSAGE = "The Client Name is required.";

        public const string CLIENT_ID_VALIDATION_MESSAGE = "The Client Id is required.";

        public static void Validate(this ClientInputModel inputModel)
        {
            var validationException = new ValidationException();
            
            validationException.AddValidation(() => inputModel.AllowedGrantTypes.IsNullOrEmpty(), nameof(inputModel.AllowedGrantTypes), ALLOWED_GRANT_TYPES_VALIDATION_MESSAGE); 
            validationException.AddValidation(() => string.IsNullOrWhiteSpace(inputModel.ClientName), nameof(inputModel.ClientName), CLIENT_NAME_VALIDATION_MESSAGE);
            validationException.AddValidation(() => string.IsNullOrWhiteSpace(inputModel.ClientId), nameof(inputModel.ClientId), CLIENT_ID_VALIDATION_MESSAGE);

            validationException.ThrowIfHasError();
        }
    }
}