using OPS.Serialization.Byte;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OPS.Serialization.IO
{
    internal class EnumSerializer : ISerializerComponent
    {
        public bool Serialize(Type _Type, object _Object, out byte[] _Bytes)
        {
            if (!_Type.IsEnum)
            {
                _Bytes = null;
                return false;
            }

            System.Type var_EnumUnderlyingType = System.Enum.GetUnderlyingType(_Type);

            ByteWriter var_Writer = new ByteWriter();
            var_Writer.Write(var_EnumUnderlyingType, _Object);

            _Bytes = var_Writer.ToArray();
            return true;
        }

        public bool DeSerialize(Type _Type, byte[] _Bytes, out object _Object)
        {
            if(!_Type.IsEnum)
            {
                _Object = null;
                return false;
            }

            System.Type var_EnumUnderlyingType = System.Enum.GetUnderlyingType(_Type);

            ByteReader var_Reader = new ByteReader(_Bytes);

            _Object = var_Reader.Read(var_EnumUnderlyingType);

            _Object = Enum.ToObject(_Type, _Object);

            return true;
        }
    }
}
