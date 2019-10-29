using OPS.Serialization.Byte;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OPS.Serialization.IO
{
    internal class NullAbleSerializer : ISerializerComponent
    {
        public bool Serialize(Type _Type, object _Object, out byte[] _Bytes)
        {
            if (!_Type.IsNullable())
            {
                _Bytes = null;
                return false;
            }

            Type var_UnderlyingType = _Type.GetNullableUnderlyingType();

            ByteWriter var_Writer = new ByteWriter();
            if(_Object == null)
            {
                var_Writer.Write(true);
                _Bytes = var_Writer.ToArray();
                return true;
            }
            else
            {
                var_Writer.Write(false);

                if (PrimitiveTypeMatcher.TypeDictionary.ContainsKey(var_UnderlyingType))
                {
                    var_Writer.Write(var_UnderlyingType, _Object);
                }
                else
                {
                    byte[] var_Bytes = Serializer.Internal_Serialize(var_UnderlyingType, _Object);
                    var_Writer.WriteBytesAndSize(var_Bytes, var_Bytes.Length);
                }

                _Bytes = var_Writer.ToArray();
            }

            return true;
        }

        public bool DeSerialize(Type _Type, byte[] _Bytes, out object _Object)
        {
            if (!_Type.IsNullable())
            {
                _Object = null;
                return false;
            }

            Type var_UnderlyingType = _Type.GetNullableUnderlyingType();

            ByteReader var_Reader = new ByteReader(_Bytes);

            bool var_IsNull = var_Reader.ReadBoolean();

            if(var_IsNull)
            {
                _Object = null;
                return true;
            }

            //
            if (PrimitiveTypeMatcher.TypeDictionary.ContainsKey(var_UnderlyingType))
            {
                _Object = var_Reader.Read(var_UnderlyingType);
            }
            else
            {
                byte[] var_Bytes = var_Reader.ReadBytesAndSize();
                _Object = Serializer.DeSerialize(var_UnderlyingType, var_Bytes);
            }

            return true;
        }
    }
}
