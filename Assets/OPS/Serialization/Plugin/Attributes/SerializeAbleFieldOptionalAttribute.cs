using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OPS.Serialization.Attributes
{
    /// <summary>
    /// Attach this Attribute to a field in a serializeable class to mark it as serializeable.
    /// The optional means, when an serialized object gets deserialized and this serialized object does not contain a 
    /// value for the field marked as optional, there is no error. For not optional errors there will be a error!
    /// Good for versioning.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class SerializeAbleFieldOptionalAttribute : Attribute
    {
        private int index;
        public int Index
        {
            get
            {
                return this.index;
            }
        }

        public SerializeAbleFieldOptionalAttribute(int _Index)
        {
            this.index = _Index;
        }
    }
}
