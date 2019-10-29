using OPS.Serialization.Byte;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OPS.Serialization.IO
{
    internal class PrimitiveSerializer : ISerializerComponent
    {
        public bool Serialize(Type _Type, object _Object, out byte[] _Bytes)
        {
            if (!PrimitiveTypeMatcher.TypeDictionary.ContainsKey(_Type))
            {
                _Bytes = null;
                return false;
            }

            ByteWriter var_Writer = new ByteWriter();
            var_Writer.Write(_Type, _Object);
            _Bytes = var_Writer.ToArray();
            return true;
        }

        public bool DeSerialize(Type _Type, byte[] _Bytes, out object _Object)
        {
            if (!PrimitiveTypeMatcher.TypeDictionary.ContainsKey(_Type))
            {
                _Object = null;
                return false;
            }

            ByteReader var_Reader = new ByteReader(_Bytes);
            _Object = var_Reader.Read(_Type);
            return true;
        }
    }
}
