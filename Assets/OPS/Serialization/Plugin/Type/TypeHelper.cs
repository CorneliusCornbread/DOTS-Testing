using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OPS.Serialization
{
    internal static class TypeHelper
    {
        public static bool IsNullable(this Type type)
        {
            return Nullable.GetUnderlyingType(type) != null;
        }

        public static Type GetNullableUnderlyingType(this Type type)
        {
            return Nullable.GetUnderlyingType(type);
        }
    }
}
