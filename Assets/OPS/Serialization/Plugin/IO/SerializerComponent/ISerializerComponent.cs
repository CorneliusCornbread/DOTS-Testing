using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OPS.Serialization.IO
{
    /// <summary>
    /// Use this Interface to create custom Serialization and Deserialization solutions.
    /// </summary>
    public interface ISerializerComponent
    {
        /// <summary>
        /// Serialize an Object _Object of Type _Type to a _Byte array.
        /// Return true if this serializer can serialize Type _Type.
        /// Else false so the next serializer can check if it can serialize the Object.
        /// </summary>
        /// <param name="_Type"></param>
        /// <param name="_Object"></param>
        /// <param name="_Bytes"></param>
        /// <returns></returns>
        bool Serialize(Type _Type, Object _Object, out byte[] _Bytes);

        /// <summary>
        /// Deserialize a Type _Type with the bytes _Bytes to an Object _Object.
        /// Return true if this deserializer can deserialize Type _Type.
        /// Else false so the next deserializer can check if it can deserialize the Type.
        /// </summary>
        /// <param name="_Type"></param>
        /// <param name="_Bytes"></param>
        /// <param name="_Object"></param>
        /// <returns></returns>
        bool DeSerialize(Type _Type, byte[] _Bytes, out object _Object);
    }
}
