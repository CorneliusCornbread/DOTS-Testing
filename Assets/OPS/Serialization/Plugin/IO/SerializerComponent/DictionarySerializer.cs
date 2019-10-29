using OPS.Serialization.Byte;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OPS.Serialization.IO
{
    internal class DictionarySerializer : ISerializerComponent
    {
        public bool Serialize(Type _Type, object _Object, out byte[] _Bytes)
        {
            //Check if is a Dictionary.
            if (_Type.IsGenericType && _Type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                ByteWriter var_Writer = new ByteWriter();

                if (_Object == null)
                {
                    var_Writer.Write((bool)true);
                    _Bytes = var_Writer.ToArray();
                    return true;
                }
                else
                {
                    var_Writer.Write((bool)false);
                }

                //Get Generic Types
                Type var_GenericItemType_Key = _Type.GetGenericArguments()[0];
                Type var_GenericItemType_Value = _Type.GetGenericArguments()[1];

                //Get Dictionary
                IDictionary var_Dictionary = _Object as IDictionary;

                //Length
                int var_DictionaryElementLength = var_Dictionary.Count;

                //Create Arrays for Key and Values.
                Array var_KeyArray = Array.CreateInstance(var_GenericItemType_Key, var_DictionaryElementLength);
                Array var_ValueArray = Array.CreateInstance(var_GenericItemType_Value, var_DictionaryElementLength);

                //Iterate Keys and Values add to array.
                int var_ArrayIndex = 0;
                IDictionaryEnumerator var_Enumerator = var_Dictionary.GetEnumerator();
                while (var_Enumerator.MoveNext())
                {
                    Object var_CurrentKey = var_Enumerator.Key;
                    Object var_CurrentValue = var_Enumerator.Value;

                    var_KeyArray.SetValue(var_CurrentKey, var_ArrayIndex);
                    var_ValueArray.SetValue(var_CurrentValue, var_ArrayIndex);

                    var_ArrayIndex += 1;
                }

                //Write Length
                var_Writer.Write(var_DictionaryElementLength);

                //Write Arrays
                byte[] var_KeyByteArray = Serializer.Serialize(var_KeyArray);
                byte[] var_ValueByteArray = Serializer.Serialize(var_ValueArray);

                var_Writer.WriteBytesAndSize(var_KeyByteArray, var_KeyByteArray.Length);
                var_Writer.WriteBytesAndSize(var_ValueByteArray, var_ValueByteArray.Length);

                _Bytes = var_Writer.ToArray();
                return true;
            }

            _Bytes = null;
            return false;
        }

        public bool DeSerialize(Type _Type, byte[] _Bytes, out object _Object)
        {
            //Check if is a Dictionary.
            if (_Type.IsGenericType && _Type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                //Setup Types
                Type var_DictionaryType = typeof(Dictionary<,>);
                Type var_GenericItemType_Key = _Type.GetGenericArguments()[0];
                Type var_GenericItemType_Value = _Type.GetGenericArguments()[1];
                Type var_GenericDictionaryType = var_DictionaryType.MakeGenericType(var_GenericItemType_Key, var_GenericItemType_Value);

                ByteReader var_Reader = new ByteReader(_Bytes);

                bool var_IsNull = var_Reader.ReadBoolean();
                if (var_IsNull)
                {
                    _Object = null;
                    return true;
                }

                //Read Length
                int var_ItemCount = var_Reader.ReadInt32();

                //Read Arrays
                byte[] var_KeyByteArray = var_Reader.ReadBytesAndSize();
                byte[] var_ValueByteArray = var_Reader.ReadBytesAndSize();

                Array var_KeyArray = Serializer.DeSerialize(var_GenericItemType_Key.MakeArrayType(), var_KeyByteArray) as Array;
                Array var_ValueArray = Serializer.DeSerialize(var_GenericItemType_Value.MakeArrayType(), var_ValueByteArray) as Array;

                //Create Dictionary
                IDictionary var_Dictionary = Activator.CreateInstance(var_GenericDictionaryType) as IDictionary;

                //Set Dictionary Keys and Values
                for(int i = 0; i < var_ItemCount; i++)
                {
                    var_Dictionary.Add(var_KeyArray.GetValue(i), var_ValueArray.GetValue(i));
                }

                _Object = var_Dictionary;
                return true;
            }

            _Object = null;
            return false;
        }
    }
}
