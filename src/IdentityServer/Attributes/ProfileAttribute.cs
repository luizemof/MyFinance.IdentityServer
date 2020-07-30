using System;

namespace IdentityServer.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ProfileAttribute : Attribute
    {
        public string Name { get; private set; }

        public ProfileAttribute(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(nameof(name));

            this.Name = name;
        }
    }
}