using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OPS.Serialization.Attributes
{
    /// <summary>
    /// Attach this Attribute to a class to mark it as serializeable.
    /// The class must have an empty constructor!
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    public class SerializeAbleClassAttribute : Attribute
    {
        public SerializeAbleClassAttribute()
        {
        }
    }
}
