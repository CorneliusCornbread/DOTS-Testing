using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OPS.Serialization.Attributes
{
    /// <summary>
    /// Attach this Attribute to a field in a serializeable class to mark it as serializeable.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class SerializeAbleFieldAttribute : Attribute
    {
        private int index;
        public int Index
        {
            get
            {
                return this.index;
            }
        }

        public SerializeAbleFieldAttribute(int _Index)
        {
            this.index = _Index;
        }
    }
}
