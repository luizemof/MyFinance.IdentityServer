using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace IdentityServer.Exceptions
{
    [System.Serializable]
    public class ValidationException : Exception
    {
        public ModelStateDictionary ModelStateDictionary { get; }

        public ValidationException()
        {
            ModelStateDictionary = new ModelStateDictionary();
        }
    }
}