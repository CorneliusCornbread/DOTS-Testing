using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using OPS.Serialization.Attributes;
using OPS.Serialization.Byte;

namespace OPS.Serialization.IO
{
    /// <summary>
    /// Core Component. Use it to Serialize or Deserialize Objects.
    /// You can even attach here custom Serialization and Deserialization solutions basing on ISerializerComponent.
    /// </summary>
    public static class Serializer
    {
        static Serializer()
        {
            serializerComponentList = new List<ISerializerComponent>();

            //
            RegisterSerializerComponent(new PrimitiveSerializer());
            RegisterSerializerComponent(new NullAbleSerializer());
            RegisterSerializerComponent(new EnumSerializer());
            RegisterSerializerComponent(new ClassSerializer());
            RegisterSerializerComponent(new UnitySerializer());
            RegisterSerializerComponent(new ColliderSerializer());
            RegisterSerializerComponent(new Collider2DSerializer());
            RegisterSerializerComponent(new ArraySerializer());
            RegisterSerializerComponent(new ListSerializer());
            RegisterSerializerComponent(new MeshSerializer());
            RegisterSerializerComponent(new HashSetSerializer());
            RegisterSerializerComponent(new QueueSerializer());
            RegisterSerializerComponent(new DictionarySerializer());
        }

        private static List<ISerializerComponent> serializerComponentList;

        /// <summary>
        /// Register custom Serialization and Deserialization solutions basing on ISerializerComponent.
        /// </summary>
        /// <param name="_SerializerComponent"></param>
        public static void RegisterSerializerComponent(ISerializerComponent _SerializerComponent)
        {
            serializerComponentList.Add(_SerializerComponent);
        }

        /// <summary>
        /// Serialize an _Object to an byte array.
        /// Optional you can compress and encrypt the serialized _Object.
        /// </summary>
        /// <param name="_Object"></param>
        /// <param name="_Compress"></param>
        /// <param name="_Encrypt"></param>
        /// <param name="_EncryptionKey"></param>
        /// <returns></returns>
        public static byte[] Serialize(Object _Object, bool _Compress = false, bool _Encrypt = false, String _EncryptionKey = "")
        {
            if (_Object == null)
            {
                throw new Exception("_Object is null!");
            }

            Type var_Type = _Object.GetType();

            byte[] var_Result = Internal_Serialize(var_Type, _Object);
            if (_Compress)
            {
                var_Result = Compress.CompressionHelper.Compress(var_Result);
            }
            if (_Encrypt)
            {
                bool var_NoError = Security.SecurityHelper.Encrypt(var_Result, _EncryptionKey);
                if (!var_NoError)
                {
                    throw new Exception("Cannot Encrypt Bytes! Bytes or Key are empty!");
                }
            }

            return var_Result;
        }

        /// <summary>
        /// Serialize an _Object to a stream.
        /// Optional you can compress and encrypt the serialized _Object.
        /// </summary>
        /// <param name="_WriteToStream"></param>
        /// <param name="_Object"></param>
        /// <param name="_Compress"></param>
        /// <param name="_Encrypt"></param>
        /// <param name="_EncryptionKey"></param>
        public static void SerializeToStream(Stream _WriteToStream, Object _Object, bool _Compress = false, bool _Encrypt = false, String _EncryptionKey = "")
        {
            if (_Object == null)
            {
                throw new Exception("_Object is null!");
            }

            Type var_Type = _Object.GetType();

            byte[] var_Result = Internal_Serialize(var_Type, _Object);
            if (_Compress)
            {
                var_Result = Compress.CompressionHelper.Compress(var_Result);
            }
            if (_Encrypt)
            {
                bool var_NoError = Security.SecurityHelper.Encrypt(var_Result, _EncryptionKey);
                if (!var_NoError)
                {
                    throw new Exception("Cannot Encrypt Bytes! Bytes or Key are empty!");
                }
            }

            _WriteToStream.Write(var_Result, 0, var_Result.Length);
        }

        /// <summary>
        /// _Type will not be null if called through ClassSerializer, there is _Type the FieldType.
        /// Checks not for _Object is null!
        /// So every ISerializer has to return something if Object is null!
        /// </summary>
        /// <param name="_Type"></param>
        /// <param name="_Object"></param>
        /// <returns></returns>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [ObsoleteAttribute("Use Serialize(Object)!", false)]
        internal static byte[] Internal_Serialize(Type _Type, Object _Object)
        {
            if(_Type == null)
            {
                throw new Exception("_Type is null!");
            }
            if (_Object == null)
            {
                throw new Exception("_Object is null!");
            }

            try
            {
                byte[] var_Bytes = null;
                for (int i = 0; i < serializerComponentList.Count; i++)
                {
                    if (serializerComponentList[i].Serialize(_Type, _Object, out var_Bytes))
                    {
                        return var_Bytes;
                    }
                }
                return var_Bytes;
            }
            catch(Exception e)
            {
                throw new Exception("Could not serialize " + _Type.ToString() + " Error: " + e.ToString());
            }
        }

        //

        /// <summary>
        /// Deserialize an serialized Object of Type T from a byte array.
        /// Optional if the serialized Object got compressed or encrypted apply the belonging parameter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_Bytes"></param>
        /// <param name="_DeCompress"></param>
        /// <param name="_Decrypt"></param>
        /// <param name="_DecryptionKey"></param>
        /// <returns></returns>
        public static T DeSerialize<T>(byte[] _Bytes, bool _DeCompress = false, bool _Decrypt = false, String _DecryptionKey = "")
        {
            if (_Bytes == null)
            {
                throw new Exception("_Bytes are null!");
            }

            Type var_Type = typeof(T);

            return (T)DeSerialize(var_Type, _Bytes, _DeCompress, _Decrypt, _DecryptionKey);
        }

        /// <summary>
        /// Deserialize an serialized Object of Type T from a stream.
        /// Optional if the serialized Object got compressed or encrypted apply the belonging parameter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_ReadFromStream"></param>
        /// <param name="_DeCompress"></param>
        /// <param name="_Decrypt"></param>
        /// <param name="_DecryptionKey"></param>
        /// <returns></returns>
        public static T DeSerializeFromStream<T>(Stream _ReadFromStream, bool _DeCompress = false, bool _Decrypt = false, String _DecryptionKey = "")
        {
            if (_ReadFromStream == null)
            {
                throw new Exception("_ReadFromStream is null!");
            }

            Type var_Type = typeof(T);

            int var_Length = (int)_ReadFromStream.Length;
            byte[] var_Bytes = new byte[var_Length];
            var_Length = _ReadFromStream.Read(var_Bytes, 0, var_Length);

            byte[] var_BytesCopyTo = new byte[var_Length];
            Array.Copy(var_Bytes, var_BytesCopyTo, var_Length);

            return (T)DeSerialize(var_Type, var_BytesCopyTo, _DeCompress, _Decrypt, _DecryptionKey);
        }

        /// <summary>
        /// Deserialize an serialized Object of Type _Type from a byte array.
        /// Optional if the serialized Object got compressed or encrypted apply the belonging parameter.
        /// </summary>
        /// <param name="_Type"></param>
        /// <param name="_Bytes"></param>
        /// <param name="_DeCompress"></param>
        /// <param name="_Decrypt"></param>
        /// <param name="_DecryptionKey"></param>
        /// <returns></returns>
        public static System.Object DeSerialize(Type _Type, byte[] _Bytes, bool _DeCompress = false, bool _Decrypt = false, String _DecryptionKey = "")
        {
            if (_Type == null)
            {
                throw new Exception("_Type is null!");
            }
            if (_Bytes == null)
            {
                throw new Exception("_Bytes are null!");
            }

            if (_Decrypt)
            {
                bool var_NoError = Security.SecurityHelper.Decrypt(_Bytes, _DecryptionKey);
                if (!var_NoError)
                {
                    throw new Exception("Cannot Decrypt Bytes! Bytes or Key are empty!");
                }
            }
            if (_DeCompress)
            {
                _Bytes = Compress.CompressionHelper.DeCompress(_Bytes);
            }

            try
            {
                Object var_Object = null;
                for (int i = 0; i < serializerComponentList.Count; i++)
                {
                    if (serializerComponentList[i].DeSerialize(_Type, _Bytes, out var_Object))
                    {
                        return var_Object;
                    }
                }
                return var_Object;
            }
            catch(Exception e)
            {
                throw new Exception("Could not deserialize " + _Type.ToString() + " Error: " + e.ToString());
            }
        }
    }
}
