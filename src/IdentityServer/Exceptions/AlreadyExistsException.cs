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

        public void AddModelError(string key, string message)
        {
            this.ModelStateDictionary.AddModelError(key, message);
        }
    }
}