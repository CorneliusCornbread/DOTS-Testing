using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace OPS.Serialization.Byte
{
    internal static class PrimitiveTypeMatcher
    {
        public static Dictionary<Type, EPrimitiveType> TypeDictionary = new Dictionary<Type, EPrimitiveType>()
        {
            { typeof(byte), EPrimitiveType.BYTE },

            { typeof(bool), EPrimitiveType.BOOL },

            { typeof(Int16), EPrimitiveType.INT16 },
            { typeof(Int32), EPrimitiveType.INT32 },
            { typeof(Int64), EPrimitiveType.INT64 },
            { typeof(UInt16), EPrimitiveType.UINT16 },
            { typeof(UInt32), EPrimitiveType.UINT32 },
            { typeof(UInt64), EPrimitiveType.UINT64 },

            { typeof(float), EPrimitiveType.FLOAT },
            { typeof(double), EPrimitiveType.DOUBLE },
            { typeof(decimal), EPrimitiveType.DECIMAL },

            { typeof(char), EPrimitiveType.CHAR },

            { typeof(Color), EPrimitiveType.COLOR },
            { typeof(Color32), EPrimitiveType.COLOR32 },

            { typeof(Vector2), EPrimitiveType.VECTOR2 },
            { typeof(Vector3), EPrimitiveType.VECTOR3 },
            { typeof(Vector4), EPrimitiveType.VECTOR4 },
            { typeof(Quaternion), EPrimitiveType.QUATERNION },
            { typeof(Matrix4x4), EPrimitiveType.MATRIX4X4 },
            
            { typeof(Plane), EPrimitiveType.PLANE },
            { typeof(Ray), EPrimitiveType.RAY },
            { typeof(Rect), EPrimitiveType.RECT },

            { typeof(String), EPrimitiveType.STRING },
        };
    }
}
