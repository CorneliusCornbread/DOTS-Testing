using OPS.Serialization.Byte;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OPS.Serialization.IO
{
    internal class ListSerializer : ISerializerComponent
    {
        public bool Serialize(Type _Type, object _Object, out byte[] _Bytes)
        {
            if (_Type.IsGenericType && _Type.GetGenericTypeDefinition() == typeof(List<>))
            {
                if (_Type.GetGenericArguments().Length == 1)
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

                    //

                    Type var_GenericItemType = _Type.GetGenericArguments()[0];

                    IList var_List = _Object as IList;

                    var_Writer.Write((UInt16)var_List.Count);

                    if (PrimitiveTypeMatcher.TypeDictionary.ContainsKey(_Type))
                    {
                        for (int i = 0; i < var_List.Count; i++)
                        {
                            var_Writer.Write(var_GenericItemType, var_List[i]);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < var_List.Count; i++)
                        {
                            byte[] var_Bytes = Serializer.Internal_Serialize(var_GenericItemType, var_List[i]);
                            var_Writer.WriteBytesAndSize(var_Bytes, var_Bytes.Length);
                        }
                    }

                    _Bytes = var_Writer.ToArray();
                    return true;
                }
            }

            _Bytes = null;
            return false;
        }

        public bool DeSerialize(Type _Type, byte[] _Bytes, out object _Object)
        {
            if (_Type.IsGenericType && _Type.GetGenericTypeDefinition() == typeof(List<>))
            {
                if (_Type.GetGenericArguments().Length == 1)
                {
                    //TODO: Kann man nicht _Type direkt nehemn :D
                    Type var_ListType = typeof(List<>);
                    Type var_GenericItemType = _Type.GetGenericArguments()[0];
                    Type var_GenericListType = var_ListType.MakeGenericType(var_GenericItemType);

                    IList var_List = Activator.CreateInstance(var_GenericListType) as IList;

                    ByteReader var_Reader = new ByteReader(_Bytes);

                    bool var_IsNull = var_Reader.ReadBoolean();
                    if (var_IsNull)
                    {
                        _Object = null;
                        return true;
                    }

                    //
                    int var_Count = var_Reader.ReadUInt16();

                    if (PrimitiveTypeMatcher.TypeDictionary.ContainsKey(_Type))
                    {
                        for (int i = 0; i < var_Count; i++)
                        {
                            var_List.Add(var_Reader.Read(var_GenericItemType));
                        }
                    }
                    else
                    {
                        for (int i = 0; i < var_Count; i++)
                        {
                            byte[] var_Bytes = var_Reader.ReadBytesAndSize();
                            Object var_Item = Serializer.DeSerialize(var_GenericItemType, var_Bytes);
                            var_List.Add(var_Item);
                        }
                    }

                    _Object = var_List;
                    return true;
                }
            }

            _Object = null;
            return false;
        }
    }
}
