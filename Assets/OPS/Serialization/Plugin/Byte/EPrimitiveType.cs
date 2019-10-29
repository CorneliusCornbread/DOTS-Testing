using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OPS.Serialization.Byte
{
    internal enum EPrimitiveType : byte
    {
        BYTE,

        BOOL,

        INT16,
        INT32,
        INT64,
        UINT16,
        UINT32,
        UINT64,

        FLOAT,
        DOUBLE,
        DECIMAL,
        
        CHAR,

        COLOR,
        COLOR32,

        VECTOR2,
        VECTOR3,
        VECTOR4,
        QUATERNION,
        MATRIX4X4,

        PLANE,
        RAY,
        RECT,

        STRING
    }
}
