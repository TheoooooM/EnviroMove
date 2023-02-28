using System;

namespace Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ServiceDependency : Attribute
    {
    }
}