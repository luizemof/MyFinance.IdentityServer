using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace IdentityServer.Exceptions
{
    [Serializable]
    public class AlreadyExistsException : Exception
    {
        public ModelStateDictionary ModelStateDictionary { get; }

        public AlreadyExistsException()
        {
            ModelStateDictionary = new ModelStateDictionary();
        }
    }
}