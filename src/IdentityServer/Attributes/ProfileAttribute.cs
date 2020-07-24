using System;

namespace IdentityServer.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ProfileAttribute : Attribute
    { }
}